using UnityEngine;
using Fusion;
using System.Collections.Generic;
using BrilliantBingo.Code.Infrastructure.Generators;

public class GameManager : NetworkBehaviour
{
    [Header("Game Settings")]
    [SerializeField] private float numberAnnounceInterval = 5f;
    
    [Header("References")]
    [SerializeField] private BingoNumberGenerator numberGenerator;
    [SerializeField] private GameplayUI gameplayUI;

    [Networked] private NetworkBool IsGameStarted { get; set; }
    [Networked] private TickTimer NumberAnnounceTimer { get; set; }
    [Networked] private int CurrentNumber { get; set; }
    [Networked] private NetworkArray<int> DrawnNumbers { get; }
    
    private HashSet<int> drawnNumbersSet = new HashSet<int>();
    private const int MAX_BINGO_NUMBER = 75;

    public override void Spawned()
    {
        if (Object.HasStateAuthority)
        {
            // Initialize game state
            IsGameStarted = false;
            NumberAnnounceTimer = TickTimer.None;
            CurrentNumber = 0;
            drawnNumbersSet.Clear();
            
            // Generate initial bingo cards for all players
            RPC_GenerateBingoCards();
        }
    }

    public override void FixedUpdateNetwork()
    {
        if (!IsGameStarted || !Object.HasStateAuthority) return;

        // Handle number announcement timer
        if (NumberAnnounceTimer.Expired(Runner))
        {
            AnnounceNextNumber();
            NumberAnnounceTimer = TickTimer.CreateFromSeconds(Runner, numberAnnounceInterval);
        }
    }

    [Rpc(RpcSources.StateAuthority, RpcTargets.All)]
    private void RPC_GenerateBingoCards()
    {
        if (numberGenerator != null && gameplayUI != null)
        {
            int[] card = numberGenerator.GenerateBingoCard();
            gameplayUI.RPC_UpdateBingoCard(card);
        }
    }

    [Rpc(RpcSources.StateAuthority, RpcTargets.All)]
    private void RPC_StartGame()
    {
        IsGameStarted = true;
        if (Object.HasStateAuthority)
        {
            NumberAnnounceTimer = TickTimer.CreateFromSeconds(Runner, numberAnnounceInterval);
        }
    }

    private void AnnounceNextNumber()
    {
        if (!Object.HasStateAuthority) return;

        // Generate a new number that hasn't been drawn yet
        int newNumber;
        do
        {
            newNumber = Random.Range(1, MAX_BINGO_NUMBER + 1);
        } while (drawnNumbersSet.Contains(newNumber));

        // Add to drawn numbers
        drawnNumbersSet.Add(newNumber);
        CurrentNumber = newNumber;

        // Notify all clients
        RPC_AnnounceNumber(newNumber);
    }

    [Rpc(RpcSources.StateAuthority, RpcTargets.All)]
    private void RPC_AnnounceNumber(int number)
    {
        if (gameplayUI != null)
        {
            gameplayUI.UpdateCurrentNumber(number);
        }
    }

    // Public method to start the game (can be called from UI)
    public void StartGame()
    {
        if (Object.HasStateAuthority)
        {
            RPC_StartGame();
        }
    }
} 