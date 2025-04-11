using UnityEngine;
using Fusion;
using System.Collections.Generic;
using BrilliantBingo.Code.Infrastructure.Generators;

public class BingoNumberAnnouncer : NetworkBehaviour
{
    // Store the numbers in a networked array
    [Networked, Capacity(75)]
    private NetworkArray<int> DrawnNumbers { get; }
    
    [Networked]
    public int CurrentNumber { get; set; }
    
    private BingoNumberGenerator numberGenerator;
    private GameplayUI gameplayUI;
    
    private int previousCurrentNumber;
    
    // Debug properties
    [SerializeField] private bool enableDebugLogging = true; // Set to true temporarily for debugging
    private float lastAnnouncementTime;
    private int announcementCount = 0;
    
    // Track how many retry attempts we've made
    private int retryAttempts = 0;
    private const int MAX_RETRY_ATTEMPTS = 3;

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
            ClearDrawnNumbersArray();
        }
    }
    
    private void ClearDrawnNumbersArray()
    {
        for (int i = 0; i < DrawnNumbers.Length; i++)
        {
            DrawnNumbers.Set(i, 0);
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
        
        // Debug timer check - only log if there's a significant delay (reduced frequency)
        if (HasStateAuthority && Time.time - lastAnnouncementTime > 20f)
        {
            LogDebug($"WARNING: 20 seconds passed without announcement. Current/Prev: {CurrentNumber}/{previousCurrentNumber}");
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

        // Print all currently drawn numbers for debugging
        DebugLogDrawnNumbers();
        
        // Get a number that hasn't been announced yet
        int nextNumber = GetUniqueNumber();
        
        // If we couldn't get a unique number after retries, handle failure
        if (nextNumber <= 0)
        {
            Debug.LogError("[BingoAnnouncer] Failed to get a unique number after multiple attempts. Check if all numbers have been drawn.");
            return;
        }
        
        // Reset retry counter
        retryAttempts = 0;
        
        // Log announcement
        announcementCount++;
        lastAnnouncementTime = Time.time;
        
        // Keep this important log about newly drawn number
        Debug.Log($"[BingoAnnouncer] Announcing number #{announcementCount}: {nextNumber}");
        
        // Set the current number - this will be synced across the network
        CurrentNumber = nextNumber;
        
        // Add to drawn numbers array
        StoreNumberInDrawnArray(nextNumber);
        
        // Force immediate UI update on all clients
        RPC_UpdateNumberDisplay(nextNumber);
    }
    
    // Get a unique number that hasn't been drawn yet
    private int GetUniqueNumber()
    {
        // Too many retries, something is wrong
        if (retryAttempts >= MAX_RETRY_ATTEMPTS)
        {
            Debug.LogError("[BingoAnnouncer] Maximum retry attempts reached. Unable to get a unique number.");
            return -1;
        }
        
        // Use the BingoNumberGenerator to get the next number from the pool
        int nextNumber = -1;
        if (numberGenerator != null)
        {
            nextNumber = numberGenerator.GetNextNumber();
            
            // Check if the number is valid
            if (nextNumber <= 0)
            {
                Debug.LogWarning("[BingoAnnouncer] NumberGenerator returned an invalid number. All numbers may have been drawn.");
                return -1;
            }
            
            // Check if this number has already been drawn
            if (IsNumberDrawn(nextNumber))
            {
                Debug.LogWarning($"[BingoAnnouncer] Number {nextNumber} has already been drawn! Trying again.");
                retryAttempts++;
                
                // Force reset the number generator if we're getting duplicates
                if (retryAttempts >= 2 && Object.HasStateAuthority)
                {
                    Debug.LogWarning("[BingoAnnouncer] Resetting number generator to resolve duplicate issue");
                    numberGenerator.ResetNumberGenerator();
                }
                
                // Try again recursively
                return GetUniqueNumber();
            }
        }
        else
        {
            LogDebug("BingoNumberGenerator reference is missing, trying to find it");
            numberGenerator = GetComponent<BingoNumberGenerator>();
            
            if (numberGenerator == null)
            {
                LogDebug("Could not find BingoNumberGenerator, falling back to random generation");
                // Fallback to old method as a last resort
                do
                {
                    nextNumber = Random.Range(1, 76);
                } while (IsNumberDrawn(nextNumber));
            }
            else
            {
                return GetUniqueNumber(); // Try again with the found generator
            }
        }
        
        return nextNumber;
    }
    
    // Helper method to store a number in the drawn numbers array
    private void StoreNumberInDrawnArray(int number)
    {
        // First check if it's already in the array (defensive)
        if (IsNumberDrawn(number))
        {
            Debug.LogWarning($"[BingoAnnouncer] Attempt to store already drawn number {number}. Skipping.");
            return;
        }
        
        // Find first empty slot and store the number
        for (int i = 0; i < DrawnNumbers.Length; i++)
        {
            if (DrawnNumbers[i] == 0)
            {
                DrawnNumbers.Set(i, number);
                LogDebug($"Stored number {number} in drawn numbers array at index {i}");
                return;
            }
        }
        
        // If we get here, there was no empty slot
        Debug.LogError($"[BingoAnnouncer] Could not store number {number} in drawn numbers array - array is full!");
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
    
    // Reset the drawn numbers array for a new round
    public void ResetDrawnNumbers()
    {
        if (!HasStateAuthority) return;
        
        LogDebug("Resetting drawn numbers array");
        
        // Clear the NetworkArray
        ClearDrawnNumbersArray();
        
        // Reset the current number
        CurrentNumber = 0;
        previousCurrentNumber = 0;
        
        // Reset local tracking
        announcementCount = 0;
        retryAttempts = 0;
        lastAnnouncementTime = Time.time;
        
        LogDebug("Successfully reset drawn numbers array");
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