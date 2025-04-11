using UnityEngine;
using Fusion;
using System.Collections.Generic;
using BrilliantBingo.Code.Infrastructure.Generators;

public class GameManager : NetworkBehaviour
{
    [Header("Game Settings")]
    [SerializeField] private float numberAnnounceInterval = 10f;
    
    [Header("Debug")]
    [SerializeField] private bool enableDebugLogging = false;
    
    [Header("References")]
    [SerializeField] private BingoNumberGenerator numberGenerator;
    [SerializeField] private GameplayUI gameplayUI;
    [SerializeField] private BingoNumberAnnouncer numberAnnouncer;

    [Networked] private NetworkBool IsGameStarted { get; set; }
    [Networked] private TickTimer NumberAnnounceTimer { get; set; }
    [Networked] private int CurrentNumber { get; set; }
    [Networked] private NetworkArray<int> DrawnNumbers { get; }
    
    private HashSet<int> drawnNumbersSet = new HashSet<int>();
    private const int MAX_BINGO_NUMBER = 75;
    private int announcementCount = 0;

    private void Awake()
    {
        // Find references if not set in inspector
        if (numberGenerator == null) numberGenerator = GetComponent<BingoNumberGenerator>();
        if (gameplayUI == null) gameplayUI = FindObjectOfType<GameplayUI>();
        if (numberAnnouncer == null) numberAnnouncer = GetComponent<BingoNumberAnnouncer>();
        
        if (numberAnnouncer == null)
        {
            LogError("BingoNumberAnnouncer reference is missing!");
        }
    }

    public override void Spawned()
    {
        LogDebug($"GameManager Spawned. HasStateAuthority: {Object.HasStateAuthority}");
        
        if (Object.HasStateAuthority)
        {
            // Initialize game state
            IsGameStarted = true; // Set to true to start announcing numbers
            CurrentNumber = 0;
            drawnNumbersSet.Clear();
            
            // Generate initial bingo cards for all players
            RPC_GenerateBingoCards();
            LogDebug("Generated bingo cards for players");
            
            // Start by announcing the first number
            AnnounceNextNumber();
            
            // Start the timer for next number - ensure it's created properly
            NumberAnnounceTimer = TickTimer.CreateFromSeconds(Runner, numberAnnounceInterval);
            LogDebug($"Started timer for next number: {numberAnnounceInterval} seconds");
        }
    }

    public override void FixedUpdateNetwork()
    {
        if (!Object.HasStateAuthority) return;
        
        // Reduce frequency of game state logging to every ~30 seconds
        if (enableDebugLogging && Runner.Tick % 1800 == 0) 
        {
            LogDebug($"Game State - IsStarted: {IsGameStarted}, CurrentNumber: {CurrentNumber}");
            LogDebug($"Timer Status: {(NumberAnnounceTimer.Expired(Runner) ? "Expired" : $"Time left: {NumberAnnounceTimer.RemainingTime(Runner)?.ToString() ?? "unknown"}")}");
        }

        if (!IsGameStarted) return;

        // Handle number announcement timer
        if (NumberAnnounceTimer.Expired(Runner))
        {
            LogDebug("Timer expired! Announcing next number...");
            AnnounceNextNumber();
            
            // Important: Create a new timer immediately
            NumberAnnounceTimer = TickTimer.CreateFromSeconds(Runner, numberAnnounceInterval);
            LogDebug($"Reset timer for next number: {numberAnnounceInterval} seconds");
        }
        else if (!NumberAnnounceTimer.IsRunning)
        {
            // Safeguard: If timer is not running, create it
            LogWarning("Timer was not running, reinitializing...");
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
        LogDebug("Game started via RPC");
        
        if (Object.HasStateAuthority)
        {
            NumberAnnounceTimer = TickTimer.CreateFromSeconds(Runner, numberAnnounceInterval);
            LogDebug($"Started timer for next number: {numberAnnounceInterval} seconds");
        }
    }

    private void AnnounceNextNumber()
    {
        if (!Object.HasStateAuthority) return;
        
        announcementCount++;
        LogDebug($"AnnounceNextNumber called (#{announcementCount})");

        // Check if we should use the BingoNumberAnnouncer component
        if (numberAnnouncer != null)
        {
            LogDebug("Using BingoNumberAnnouncer to announce next number");
            // Let the BingoNumberAnnouncer handle it
            numberAnnouncer.AnnounceNextNumber();
            
            // Update our local state to stay in sync
            // This assumes BingoNumberAnnouncer.CurrentNumber is updated in AnnounceNextNumber()
            int announcedNumber = numberAnnouncer.CurrentNumber;
            LogDebug($"BingoNumberAnnouncer announced: {announcedNumber}");
            
            if (announcedNumber > 0)
            {
                drawnNumbersSet.Add(announcedNumber);
                CurrentNumber = announcedNumber;
                LogDebug($"Updated CurrentNumber to {CurrentNumber}");
            }
            else
            {
                LogError("BingoNumberAnnouncer returned an invalid number (0 or negative)");
            }
        }
        else
        {
            LogDebug("No BingoNumberAnnouncer found, using fallback implementation");
            // Fall back to our own implementation
            // Generate a new number that hasn't been drawn yet
            int newNumber;
            do
            {
                newNumber = Random.Range(1, MAX_BINGO_NUMBER + 1);
            } while (drawnNumbersSet.Contains(newNumber));

            // Add to drawn numbers
            drawnNumbersSet.Add(newNumber);
            CurrentNumber = newNumber;
            LogDebug($"Generated new number: {newNumber}");

            // Notify all clients via the GameplayUI
            RPC_AnnounceNumber(newNumber);
        }
        
        LogDebug($"Total numbers drawn so far: {drawnNumbersSet.Count}");
        
        // Log all drawn numbers for debugging (reduced frequency to every 10 draws)
        if (announcementCount % 10 == 0 && enableDebugLogging)
        {
            string allNumbers = string.Join(", ", drawnNumbersSet);
            LogDebug($"All drawn numbers: {allNumbers}");
        }
    }

    [Rpc(RpcSources.StateAuthority, RpcTargets.All)]
    private void RPC_AnnounceNumber(int number)
    {
        LogDebug($"RPC_AnnounceNumber received: {number}");
        
        if (gameplayUI != null)
        {
            gameplayUI.UpdateCurrentNumber(number);
            LogDebug("Updated GameplayUI with new number");
        }
        else
        {
            LogError("GameplayUI reference is null when updating number!");
            gameplayUI = FindObjectOfType<GameplayUI>();
            if (gameplayUI != null)
            {
                gameplayUI.UpdateCurrentNumber(number);
                LogDebug("Found and updated GameplayUI");
            }
        }
    }

    // Public method to start the game (can be called from UI)
    public void StartGame()
    {
        if (Object.HasStateAuthority)
        {
            LogDebug("StartGame called");
            RPC_StartGame();
        }
        else
        {
            LogDebug("StartGame called but doesn't have state authority");
        }
    }
    
    // Method to reset the game for a new round
    public void ResetGame()
    {
        if (!Object.HasStateAuthority) return;
        
        LogDebug("Resetting game for a new round");
        
        // Reset game state
        CurrentNumber = 0;
        drawnNumbersSet.Clear();
        
        // Reset the number generator
        if (numberGenerator != null)
        {
            numberGenerator.ResetNumberGenerator();
            LogDebug("Reset BingoNumberGenerator");
        }
        else
        {
            LogError("BingoNumberGenerator reference is missing during reset!");
        }
        
        // Reset the number announcer
        if (numberAnnouncer != null)
        {
            numberAnnouncer.ResetDrawnNumbers();
            LogDebug("Reset BingoNumberAnnouncer");
        }
        else
        {
            LogWarning("BingoNumberAnnouncer reference is missing during reset!");
        }
        
        // Generate new bingo cards for all players
        RPC_GenerateBingoCards();
        LogDebug("Generated new bingo cards for all players");
        
        // Reset the announcement timer
        NumberAnnounceTimer = TickTimer.CreateFromSeconds(Runner, numberAnnounceInterval);
        LogDebug($"Reset number announcement timer: {numberAnnounceInterval} seconds");
        
        // Call RPC to ensure all clients reset their state
        RPC_ResetGame();
    }
    
    [Rpc(RpcSources.StateAuthority, RpcTargets.All)]
    private void RPC_ResetGame()
    {
        LogDebug("Received RPC to reset game");
        
        // Reset local game state
        CurrentNumber = 0;
        drawnNumbersSet.Clear();
        
        // Reset UI if available
        if (gameplayUI != null)
        {
            // Update the UI to show game reset
            gameplayUI.ResetBingoCard();
            LogDebug("Reset GameplayUI");
        }
    }
    
    private void LogDebug(string message)
    {
        if (enableDebugLogging)
        {
            Debug.Log($"[GameManager] {message}");
        }
    }
    
    private void LogWarning(string message)
    {
        Debug.LogWarning($"[GameManager] {message}");
    }
    
    private void LogError(string message)
    {
        Debug.LogError($"[GameManager] {message}");
    }
} 