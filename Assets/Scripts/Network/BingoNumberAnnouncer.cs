using UnityEngine;
using Fusion;
using System.Collections.Generic;
using BrilliantBingo.Code.Infrastructure.Generators;

public class BingoNumberAnnouncer : NetworkBehaviour
{
    [Networked] private NetworkArray<int> DrawnNumbers { get; }
    private BingoNumberGenerator numberGenerator;
    private GameplayUI gameplayUI;

    private void Awake()
    {
        numberGenerator = GetComponent<BingoNumberGenerator>();
        gameplayUI = GetComponent<GameplayUI>();
    }

    public override void Spawned()
    {
        if (HasStateAuthority)
        {
            // Initialize the drawn numbers array
            DrawnNumbers.Clear();
        }
    }

    public void AnnounceNextNumber()
    {
        if (!HasStateAuthority) return;

        // Keep trying until we find a number that hasn't been drawn yet
        int nextNumber;
        do
        {
            nextNumber = Random.Range(1, 76);
        } while (IsNumberDrawn(nextNumber));
        
        // Add to drawn numbers array
        for (int i = 0; i < DrawnNumbers.Length; i++)
        {
            if (DrawnNumbers[i] == 0)
            {
                DrawnNumbers.Set(i, nextNumber);
                break;
            }
        }
        
        // Update UI for all players
        RPC_UpdateNumberDisplay(nextNumber);
    }

    [Rpc(RpcSources.StateAuthority, RpcTargets.All)]
    private void RPC_UpdateNumberDisplay(int number)
    {
        if (gameplayUI != null)
        {
            gameplayUI.UpdateCurrentNumber(number);
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
}