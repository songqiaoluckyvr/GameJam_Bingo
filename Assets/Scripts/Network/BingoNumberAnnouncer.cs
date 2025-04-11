using UnityEngine;
using Fusion;
using System.Collections.Generic;
using BrilliantBingo.Code.Infrastructure.Generators;

public class BingoNumberAnnouncer : NetworkBehaviour
{
    [Networked]
    private NetworkArray<int> DrawnNumbers { get; }
    
    [Networked]
    public int CurrentNumber { get; set; }
    
    private BingoNumberGenerator numberGenerator;
    private GameplayUI gameplayUI;
    
    private int previousCurrentNumber;
    
    // Debug properties
    [SerializeField] private bool enableDebugLogging = true;
    private float lastAnnouncementTime;
    private int announcementCount = 0;

    private void Awake()
    {
        numberGenerator = GetComponent<BingoNumberGenerator>();
        gameplayUI = FindObjectOfType<GameplayUI>();
        lastAnnouncementTime = Time.time;
    }

    public override void Spawned()
    {
        LogDebug($"BingoNumberAnnouncer Spawned. HasStateAuthority: {HasStateAuthority}");
        
        // Find the GameplayUI reference if not set
        if (gameplayUI == null)
        {
            gameplayUI = FindObjectOfType<GameplayUI>();
            LogDebug($"Found GameplayUI reference: {(gameplayUI != null)}");
        }
        
        // Initialize drawn numbers array (only on host)
        if (HasStateAuthority)
        {
            LogDebug("Initializing drawn numbers array");
            for (int i = 0; i < DrawnNumbers.Length; i++)
            {
                DrawnNumbers.Set(i, 0);
            }
        }
    }
    
    public override void FixedUpdateNetwork()
    {
        // Check if the current number has changed
        if (CurrentNumber != previousCurrentNumber && CurrentNumber > 0)
        {
            previousCurrentNumber = CurrentNumber;
            
            // Notify UI with the new number
            if (gameplayUI != null)
            {
                gameplayUI.UpdateCurrentNumber(CurrentNumber);
                LogDebug($"Updated UI with current number: {CurrentNumber}");
            }
        }
        
        // Debug timer check - For host only
        if (HasStateAuthority && Time.time - lastAnnouncementTime > 10f)
        {
            LogDebug($"WARNING: 10 seconds passed without announcement. Current/Prev: {CurrentNumber}/{previousCurrentNumber}");
            LogDebug($"Total announcements so far: {announcementCount}");
            lastAnnouncementTime = Time.time;
        }
    }

    public void AnnounceNextNumber()
    {
        if (!HasStateAuthority)
        {
            LogDebug("AnnounceNextNumber called but doesn't have state authority");
            return;
        }

        // Generate a new number that hasn't been drawn yet
        int nextNumber;
        do
        {
            nextNumber = Random.Range(1, 76);
        } while (IsNumberDrawn(nextNumber));
        
        // Log announcement
        announcementCount++;
        lastAnnouncementTime = Time.time;
        LogDebug($"Announcing number #{announcementCount}: {nextNumber}");
        
        // Set the current number - this will be synced across the network
        CurrentNumber = nextNumber;
        
        // Add to drawn numbers array
        for (int i = 0; i < DrawnNumbers.Length; i++)
        {
            if (DrawnNumbers[i] == 0)
            {
                DrawnNumbers.Set(i, nextNumber);
                break;
            }
        }
        
        // Force immediate UI update on all clients
        RPC_UpdateNumberDisplay(nextNumber);
    }
    
    [Rpc(RpcSources.StateAuthority, RpcTargets.All)]
    private void RPC_UpdateNumberDisplay(int number)
    {
        // Ensure number is valid
        if (number <= 0)
        {
            LogDebug($"Attempted to announce invalid number: {number}");
            return;
        }
        
        // Update our networked property
        CurrentNumber = number;
        LogDebug($"RPC_UpdateNumberDisplay received: {number}");
        
        // Try to update UI if available
        if (gameplayUI != null)
        {
            gameplayUI.UpdateCurrentNumber(number);
            LogDebug("Updated UI via RPC");
        }
        else
        {
            // Try to find the GameplayUI
            LogDebug("GameplayUI is null in RPC, trying to find it");
            gameplayUI = FindObjectOfType<GameplayUI>();
            if (gameplayUI != null)
            {
                gameplayUI.UpdateCurrentNumber(number);
                LogDebug("Found and updated UI via RPC");
            }
            else
            {
                LogDebug("Could not find GameplayUI component in the scene");
            }
        }
    }

    public bool IsNumberDrawn(int number)
    {
        for (int i = 0; i < DrawnNumbers.Length; i++)
        {
            if (DrawnNumbers[i] == number)
            {
                return true;
            }
        }
        return false;
    }
    
    private void LogDebug(string message)
    {
        if (enableDebugLogging)
        {
            Debug.Log($"[BingoAnnouncer] {message}");
        }
    }
    
    // Debug method to print all drawn numbers
    public void DebugLogDrawnNumbers()
    {
        string numbers = "Drawn numbers: ";
        int count = 0;
        
        for (int i = 0; i < DrawnNumbers.Length; i++)
        {
            if (DrawnNumbers[i] > 0)
            {
                numbers += DrawnNumbers[i] + ", ";
                count++;
            }
        }
        
        LogDebug($"{numbers} (Total: {count})");
    }
}