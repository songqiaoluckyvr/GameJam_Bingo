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
    [SerializeField] private BingoNumberGenerator numberGenerator;
    private int[] localBingoCard;
    private bool[] markedNumbers;
    private List<int> drawnNumbers = new List<int>();
    private Button[,] gridCells;

    [Networked] private int CurrentNumber { get; set; }

    private void Awake()
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
            
            // Disable further marking 
            DisableCardInteraction();
            
            // TODO: Implement network win condition notification
            if (Object.HasStateAuthority)
            {
                // Host would notify all players about the win
                // RPC_NotifyBingoWin(Object.InputAuthority);
            }
        }
        else
        {
            Debug.Log("Not a valid BINGO pattern!");
            
            // Visual indication that bingo claim is invalid
            bingoButton.AddToClassList("bingo-invalid");
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
        int previousNumber = CurrentNumber;
        CurrentNumber = number;
        currentNumberLabel.text = number.ToString("00");
        
        Debug.Log($"<color=cyan>NEW NUMBER</color>: Changed from {previousNumber} to {number}");
        
        if (Object.HasStateAuthority)
        {
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
        foreach (int number in drawnNumbers.Reverse<int>())
        {
            Label numberLabel = new Label(number.ToString());
            numberLabel.AddToClassList("history-number");
            numberHistory.Add(numberLabel);
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
}