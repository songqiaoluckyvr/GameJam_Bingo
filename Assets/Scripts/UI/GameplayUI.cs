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
    private VisualElement bingoGrid;
    private Button bingoButton;
    private VisualElement playerList;
    private VisualElement numberHistory;
    [SerializeField] private BingoNumberGenerator numberGenerator;
    private int[] localBingoCard;
    private bool[] markedNumbers;
    private List<int> drawnNumbers = new List<int>();

    [Networked] private int CurrentNumber { get; set; }

    private void Awake()
    {
        root = GetComponent<UIDocument>().rootVisualElement;
        markedNumbers = new bool[25];
        
        // Initialize UI elements
        currentNumberLabel = root.Q<Label>("current-number");
        bingoGrid = root.Q<VisualElement>("bingo-grid");
        bingoButton = root.Q<Button>("bingo-button");
        playerList = root.Q<VisualElement>("player-list");
        numberHistory = root.Q<VisualElement>("number-history");

        // Set up event handlers
        bingoButton.clicked += OnBingoButtonClicked;
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
        CreateBingoGrid();
    }

    private void CreateBingoGrid()
    {
        bingoGrid.Clear();
        for (int i = 0; i < 5; i++)
        {
            for (int j = 0; j < 5; j++)
            {
                int index = i * 5 + j;
                int number = localBingoCard[index];
                
                Button cell = new Button();
                cell.text = number == 0 ? "FREE" : number.ToString();
                cell.AddToClassList("bingo-cell");
                
                if (number == 0)
                {
                    cell.AddToClassList("marked");
                    markedNumbers[index] = true;
                }
                
                int capturedIndex = index;
                cell.clicked += () => OnCellClicked(capturedIndex);
                
                bingoGrid.Add(cell);
            }
        }
    }

    private void OnCellClicked(int index)
    {
        if (!markedNumbers[index] && localBingoCard[index] == CurrentNumber)
        {
            markedNumbers[index] = true;
            bingoGrid.Children().ElementAt(index).AddToClassList("marked");
        }
    }

    private void OnBingoButtonClicked()
    {
        if (CheckForBingo())
        {
            Debug.Log("BINGO!");
            // TODO: Implement win condition
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

    public void UpdateCurrentNumber(int number)
    {
        if (Object.HasStateAuthority)
        {
            CurrentNumber = number;
            drawnNumbers.Add(number);
            UpdateNumberHistory();
        }
    }

    private void UpdateNumberHistory()
    {
        numberHistory.Clear();
        foreach (int number in drawnNumbers)
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
}