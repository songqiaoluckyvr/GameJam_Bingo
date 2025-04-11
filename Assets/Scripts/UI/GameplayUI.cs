using UnityEngine;
using UnityEngine.UIElements;
using System.Collections.Generic;
using System.Linq;
using Fusion;
using BrilliantBingo.Code.Infrastructure.Generators;

public class GameplayUI : NetworkBehaviour
{
    private VisualElement root;
    private Label currentNumberLabel;
    private VisualElement gridNumbers;
    private Button bingoButton;
    private VisualElement playerList;
    private VisualElement numberHistory;
    private VisualElement popupNotification;
    [SerializeField] private BingoNumberGenerator numberGenerator;
    private int[] localBingoCard;
    private bool[] markedNumbers;
    private List<int> drawnNumbers = new List<int>();
    private Button[,] gridCells;

    [Networked] private int CurrentNumber { get; set; }

    private void Awake()
    {
        InitializeUI();
    }

    private void InitializeUI()
    {
        root = GetComponent<UIDocument>().rootVisualElement;
        markedNumbers = new bool[25];
        gridCells = new Button[5,5];
        
        // Initialize UI elements
        currentNumberLabel = root.Q<Label>("current-number");
        gridNumbers = root.Q<VisualElement>("grid-numbers");
        bingoButton = root.Q<Button>("bingo-button");
        playerList = root.Q<VisualElement>("player-list");
        numberHistory = root.Q<VisualElement>("number-history");
        
        // Create popup notification element if it doesn't exist
        popupNotification = root.Q<VisualElement>("popup-notification");
        if (popupNotification == null)
        {
            // Create popup container
            popupNotification = new VisualElement();
            popupNotification.name = "popup-notification";
            popupNotification.AddToClassList("popup-notification");
            popupNotification.style.visibility = Visibility.Hidden;
            
            // Add a centered message label
            var messageLabel = new Label();
            messageLabel.name = "popup-message";
            messageLabel.AddToClassList("popup-message");
            popupNotification.Add(messageLabel);
            
            // Add popup to the root
            root.Add(popupNotification);
        }

        // Apply additional styling to number history
        if (numberHistory != null)
        {
            numberHistory.style.flexDirection = FlexDirection.Column;
            numberHistory.style.paddingTop = 5;
            numberHistory.style.paddingBottom = 5;
        }

        // Check if any key elements are missing
        if (currentNumberLabel == null)
        {
            Debug.LogError("Failed to find current-number label!");
        }

        // Initialize grid cells
        for (int i = 0; i < 5; i++)
        {
            for (int j = 0; j < 5; j++)
            {
                string cellName = $"cell-{i}-{j}";
                Button cellButton = root.Q<Button>(cellName);
                
                if (cellButton != null)
                {
                    gridCells[i,j] = cellButton;
                    
                    // Create local copies of loop variables to capture correctly
                    int capturedRow = i;
                    int capturedCol = j;
                    int capturedIndex = capturedRow * 5 + capturedCol;
                    
                    // Use the captured values in the lambda
                    cellButton.clicked += () => OnCellClicked(capturedIndex);
                }
                else
                {
                    Debug.LogError($"Could not find button with name: {cellName}");
                }
            }
        }

        // Set up event handlers
        bingoButton.clicked += OnBingoButtonClicked;

        // Mark the free space
        int centerIndex = 12; // Center cell (2,2)
        markedNumbers[centerIndex] = true;
    }

    public override void Spawned()
    {
        if (Object.HasStateAuthority)
        {
            // Only the host generates the initial card
            if (numberGenerator != null)
            {
                localBingoCard = numberGenerator.GenerateBingoCard();
            }
            else
            {
                Debug.LogError("BingoNumberGenerator reference is missing!");
            }
            RPC_UpdateBingoCard(localBingoCard);
        }
    }

    [Rpc(RpcSources.StateAuthority, RpcTargets.All)]
    public void RPC_UpdateBingoCard(int[] cardNumbers)
    {
        localBingoCard = cardNumbers;
        UpdateBingoGrid();
    }

    private void UpdateBingoGrid()
    {
        for (int i = 0; i < 5; i++)
        {
            for (int j = 0; j < 5; j++)
            {
                int index = i * 5 + j;
                int number = localBingoCard[index];
                
                if (gridCells[i,j] != null)
                {
                    // First enable all cells when creating a new grid
                    gridCells[i,j].SetEnabled(true);
                    gridCells[i,j].RemoveFromClassList("marked");
                    
                    if (number == 0)
                    {
                        gridCells[i,j].text = "FREE";
                        gridCells[i,j].AddToClassList("free-space");
                        gridCells[i,j].AddToClassList("marked"); // Mark free space as marked
                        markedNumbers[index] = true;
                        
                        // Disable the free space cell since it's always marked
                        gridCells[i,j].SetEnabled(false);
                    }
                    else
                    {
                        gridCells[i,j].text = number.ToString();
                    }
                }
                else
                {
                    Debug.LogError($"Grid cell [{i},{j}] is null!");
                }
            }
        }
        
        // Reset bingo button state
        bingoButton.RemoveFromClassList("bingo-win");
        bingoButton.RemoveFromClassList("bingo-invalid");
        bingoButton.RemoveFromClassList("bingo-available");
    }

    private void OnCellClicked(int index)
    {
        // Don't allow re-marking already marked cells
        if (markedNumbers[index])
        {
            Debug.Log($"Cell already marked: [{index/5},{index%5}] with value {localBingoCard[index]}");
            return;
        }
        
        int row = index / 5;
        int col = index % 5;
        int cellValue = localBingoCard[index];
        
        Debug.Log($"Player clicked cell: [{row},{col}] with value {cellValue}, CurrentNumber: {CurrentNumber}");
        
        // Allow marking if it matches current number or if it's in drawn numbers list
        if (cellValue == CurrentNumber || (drawnNumbers != null && drawnNumbers.Contains(cellValue)))
        {
            // Permanently mark this cell for this game
            markedNumbers[index] = true;
            
            // Add visual marking class and disable button to prevent any further interaction
            gridCells[row,col].AddToClassList("marked");
            
            // Disable this specific cell to prevent any further interaction with it
            // This will permanently prevent the button from being clicked again this game
            gridCells[row,col].SetEnabled(false);
            
            Debug.Log($"<color=green>MARKED</color>: Player marked cell [{row},{col}] with value {cellValue}");
            
            // For network games, notify other players this cell was marked
            if (Object.HasInputAuthority && Runner != null)
            {
                // Comment out or implement RPC as needed
                // RPC_MarkCell(index);
            }
            
            // Check for bingo after marking
            if (CheckForBingo())
            {
                Debug.Log($"<color=yellow>BINGO CONDITION MET!</color> After marking {cellValue}");
                bingoButton.AddToClassList("bingo-available");
            }
        }
        else
        {
            Debug.Log($"<color=red>INVALID MARK ATTEMPT</color>: Cell value {cellValue} does not match current number {CurrentNumber} and is not in drawn numbers list");
            
            // Optional: provide feedback that this number hasn't been called yet
            gridCells[row,col].AddToClassList("invalid-selection");
            gridCells[row,col].schedule.Execute(() => 
            {
                gridCells[row,col].RemoveFromClassList("invalid-selection");
            }).StartingIn(300);
        }
    }

    private void OnBingoButtonClicked()
    {
        if (CheckForBingo())
        {
            Debug.Log("BINGO!");
            
            // Visual indication that bingo is valid
            bingoButton.AddToClassList("bingo-win");
            
            // Show valid bingo popup
            ShowPopup("BINGO!", true);
            
            // Disable further marking 
            DisableCardInteraction();
            
            // Notify other players about the win
            if (Runner != null && Object.HasInputAuthority)
            {
                RPC_NotifyBingoWin(Runner.LocalPlayer.PlayerId);
            }
        }
        else
        {
            Debug.Log("Not a valid BINGO pattern!");
            
            // Visual indication that bingo claim is invalid
            bingoButton.AddToClassList("bingo-invalid");
            
            // Show invalid bingo popup
            ShowPopup("Bad Bingo", false);
            
            bingoButton.schedule.Execute(() => 
            {
                bingoButton.RemoveFromClassList("bingo-invalid");
            }).StartingIn(500);
        }
    }
    
    private void DisableCardInteraction()
    {
        // Disable further interaction with the card
        for (int i = 0; i < 5; i++)
        {
            for (int j = 0; j < 5; j++)
            {
                if (gridCells[i,j] != null)
                {
                    gridCells[i,j].SetEnabled(false);
                }
            }
        }
    }

    private bool CheckForBingo()
    {
        // Check rows
        for (int i = 0; i < 5; i++)
        {
            if (Enumerable.Range(0, 5).All(j => markedNumbers[i * 5 + j]))
                return true;
        }

        // Check columns
        for (int j = 0; j < 5; j++)
        {
            if (Enumerable.Range(0, 5).All(i => markedNumbers[i * 5 + j]))
                return true;
        }

        // Check diagonals
        if (Enumerable.Range(0, 5).All(i => markedNumbers[i * 5 + i]))
            return true;
        if (Enumerable.Range(0, 5).All(i => markedNumbers[i * 5 + (4 - i)]))
            return true;

        return false;
    }

    // Called to update current drawn number from network
    public void UpdateCurrentNumber(int number)
    {
        if (number <= 0)
        {
            Debug.LogWarning($"Received invalid number: {number}, ignoring update");
            return;
        }
        
        int previousNumber = CurrentNumber;
        CurrentNumber = number;
        
        // Ensure the UI element is valid
        if (currentNumberLabel == null)
        {
            Debug.LogError("currentNumberLabel is null in GameplayUI!");
            currentNumberLabel = root?.Q<Label>("current-number");
            if (currentNumberLabel == null)
            {
                Debug.LogError("Still couldn't find current-number label!");
                return;
            }
        }
        
        // Update the display with leading zeros
        currentNumberLabel.text = number.ToString("00");
        
        Debug.Log($"<color=cyan>NEW NUMBER</color>: Changed from {previousNumber} to {number}");
        
        // Add to drawn numbers list if not already there
        if (!drawnNumbers.Contains(number))
        {
            drawnNumbers.Add(number);
            UpdateNumberHistory();
            Debug.Log($"Added {number} to drawn numbers list. Total drawn: {drawnNumbers.Count}");
        }
        else
        {
            Debug.Log($"Number {number} already in drawn numbers list");
        }
    }

    // Network RPC to mark a cell on all clients (optional implementation)
    [Rpc(RpcSources.InputAuthority, RpcTargets.All)]
    public void RPC_MarkCell(int cellIndex)
    {
        if (cellIndex >= 0 && cellIndex < 25 && !markedNumbers[cellIndex])
        {
            markedNumbers[cellIndex] = true;
            int row = cellIndex / 5;
            int col = cellIndex % 5;
            
            if (gridCells[row,col] != null)
            {
                gridCells[row,col].AddToClassList("marked");
                gridCells[row,col].SetEnabled(false); // Also disable the cell on RPC call
            }
        }
    }

    // For debugging/testing - call this to manually mark a test cell
    public void DebugMarkCell(int row, int col)
    {
        if (row >= 0 && row < 5 && col >= 0 && col < 5)
        {
            int index = row * 5 + col;
            markedNumbers[index] = true;
            gridCells[row,col].AddToClassList("marked");
            gridCells[row,col].SetEnabled(false);
            Debug.Log($"Debug marked cell [{row},{col}] with value {localBingoCard[index]}");
        }
    }

    private void UpdateNumberHistory()
    {
        numberHistory.Clear();
        
        // Create rows of 3 numbers each
        int numbersPerRow = 3;
        List<int> reversedNumbers = new List<int>(drawnNumbers);
        reversedNumbers.Reverse();
        
        for (int i = 0; i < reversedNumbers.Count; i += numbersPerRow)
        {
            // Create a horizontal container for each row
            VisualElement rowContainer = new VisualElement();
            rowContainer.AddToClassList("history-row");
            rowContainer.style.flexDirection = FlexDirection.Row;
            rowContainer.style.justifyContent = Justify.SpaceBetween;
            rowContainer.style.width = new StyleLength(new Length(100, LengthUnit.Percent));
            rowContainer.style.marginBottom = 5;
            
            // Add up to 3 numbers in this row
            int numbersInThisRow = Mathf.Min(numbersPerRow, reversedNumbers.Count - i);
            for (int j = 0; j < numbersInThisRow; j++)
            {
                int numberIndex = i + j;
                if (numberIndex < reversedNumbers.Count)
                {
                    int number = reversedNumbers[numberIndex];
                    
                    // Determine the column letter based on number range
                    string columnLetter = GetColumnLetterForNumber(number);
                    
                    // Create a container for each number
                    VisualElement numberContainer = new VisualElement();
                    numberContainer.style.flexGrow = 1;
                    numberContainer.style.flexDirection = FlexDirection.Row;
                    numberContainer.style.alignItems = Align.Center;
                    numberContainer.style.justifyContent = Justify.Center;
                    
                    // Create column letter label
                    Label letterLabel = new Label(columnLetter);
                    letterLabel.AddToClassList("history-letter");
                    letterLabel.style.fontSize = 14;
                    letterLabel.style.marginRight = 3;
                    letterLabel.style.color = GetColumnColor(columnLetter);
                    
                    // Create number label
                    Label numberLabel = new Label(number.ToString("00"));
                    numberLabel.AddToClassList("history-number");
                    
                    // Add both to container
                    numberContainer.Add(letterLabel);
                    numberContainer.Add(numberLabel);
                    
                    // Add to row
                    rowContainer.Add(numberContainer);
                }
            }
            
            numberHistory.Add(rowContainer);
        }
    }
    
    private string GetColumnLetterForNumber(int number)
    {
        if (number >= 1 && number <= 15) return "B";
        if (number >= 16 && number <= 30) return "I";
        if (number >= 31 && number <= 45) return "N";
        if (number >= 46 && number <= 60) return "G";
        if (number >= 61 && number <= 75) return "O";
        return "";
    }
    
    private Color GetColumnColor(string column)
    {
        switch (column)
        {
            case "B": return new Color(1f, 0.85f, 0f); // Gold
            case "I": return new Color(0.56f, 0.93f, 0.56f); // Light Green
            case "N": return new Color(0.87f, 0.63f, 0.87f); // Light Purple
            case "G": return new Color(1f, 0.71f, 0.76f); // Light Pink
            case "O": return new Color(1f, 0.42f, 0.42f); // Coral Red
            default: return Color.white;
        }
    }

    public void UpdatePlayerList(List<string> players)
    {
        playerList.Clear();
        foreach (string player in players)
        {
            Label playerLabel = new Label(player);
            playerLabel.AddToClassList("player-item");
            playerList.Add(playerLabel);
        }
    }

    public void ResetBingoCard()
    {
        // Reset all marking except free space
        for (int i = 0; i < markedNumbers.Length; i++)
        {
            // Skip the free space (center cell)
            if (i == 12) continue;
            
            markedNumbers[i] = false;
            int row = i / 5;
            int col = i % 5;
            
            if (gridCells[row,col] != null)
            {
                // Remove marking class
                gridCells[row,col].RemoveFromClassList("marked");
                
                // Re-enable interaction
                gridCells[row,col].SetEnabled(true);
            }
        }
        
        // Reset bingo button state
        bingoButton.RemoveFromClassList("bingo-win");
        bingoButton.RemoveFromClassList("bingo-invalid");
        
        // Clear drawn numbers
        drawnNumbers.Clear();
        CurrentNumber = 0;
        currentNumberLabel.text = "00";
        
        // Update number history display
        UpdateNumberHistory();
    }

    // Call this method to reset the UI state after a scene load or other events
    public void Reset()
    {
        InitializeUI();
        
        // If CurrentNumber is valid, display it
        if (CurrentNumber > 0)
        {
            UpdateCurrentNumber(CurrentNumber);
        }
        
        // If we have a bingo card, display it
        if (localBingoCard != null && localBingoCard.Length == 25)
        {
            UpdateBingoGrid();
        }
    }

    // Shows a popup notification message
    private void ShowPopup(string message, bool isSuccess)
    {
        if (popupNotification == null) return;
        
        // Get the message label
        var messageLabel = popupNotification.Q<Label>("popup-message");
        if (messageLabel != null)
        {
            messageLabel.text = message;
        }
        
        // Apply appropriate styling based on success/failure
        popupNotification.RemoveFromClassList("popup-success");
        popupNotification.RemoveFromClassList("popup-error");
        popupNotification.AddToClassList(isSuccess ? "popup-success" : "popup-error");
        
        // Make the popup visible
        popupNotification.style.visibility = Visibility.Visible;
        popupNotification.style.opacity = 1;
        
        // Animate popup
        popupNotification.experimental.animation.Scale(1.2f, 200).OnCompleted(() => {
            popupNotification.experimental.animation.Scale(1f, 200);
        });
        
        // Auto-hide the popup after delay (longer for success)
        popupNotification.schedule.Execute(() => {
            // Fade out by manually animating opacity
            const int steps = 10;
            const float duration = 500f; // milliseconds
            float stepTime = duration / steps;
            
            for (int i = 0; i < steps; i++)
            {
                float opacity = 1 - ((float)(i + 1) / steps);
                popupNotification.schedule.Execute(() => {
                    popupNotification.style.opacity = opacity;
                }).StartingIn((int)(i * stepTime));
            }
            
            // Hide after animation completes
            popupNotification.schedule.Execute(() => {
                popupNotification.style.visibility = Visibility.Hidden;
                popupNotification.style.opacity = 1;
            }).StartingIn((int)duration);
        }).StartingIn(isSuccess ? 3000 : 2000);
    }

    // Notify all players that someone got a valid bingo
    [Rpc(RpcSources.InputAuthority, RpcTargets.All)]
    public void RPC_NotifyBingoWin(int winnerPlayerId)
    {
        if (Object.HasInputAuthority && Runner.LocalPlayer.PlayerId == winnerPlayerId)
        {
            // Local player who won - already showing their win UI
            return;
        }
        
        // Show a notification that another player won
        string winnerName = "Player " + winnerPlayerId;
        ShowPopup(winnerName + " got BINGO!", true);
        
        // Disable interaction for other players
        DisableCardInteraction();
        
        // If you have more player information, you could show the winner's name here
        Debug.Log($"Player {winnerPlayerId} got BINGO!");
    }
}