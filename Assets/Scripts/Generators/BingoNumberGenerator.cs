using UnityEngine;
using System;
using Fusion;
using System.Collections.Generic;

namespace BrilliantBingo.Code.Infrastructure.Generators
{
    public class BingoNumberGenerator : NetworkBehaviour
    {
        private System.Random rng;
        [Networked] private int NetworkSeed { get; set; }
        
        // Store the numbers in a networked array
        [Networked, Capacity(75)]
        private NetworkArray<int> NumberPool { get; }
        
        // Track available numbers count
        [Networked] private int AvailableNumbersCount { get; set; }
        
        private const int MAX_NUMBERS = 75;

        public override void Spawned()
        {
            if (Object.HasStateAuthority)
            {
                // Generate a new seed only on the host
                NetworkSeed = UnityEngine.Random.Range(0, int.MaxValue);
                Debug.Log($"Host generated seed: {NetworkSeed}");
                
                // Initialize the number pool
                InitializeNumberPool();
            }

            InitializeRNG();
        }

        private void InitializeRNG()
        {
            if (NetworkSeed == 0)
            {
                Debug.LogWarning("NetworkSeed is 0, using fallback seed");
                NetworkSeed = (int)DateTime.Now.Ticks;
            }
            rng = new System.Random(NetworkSeed);
        }

        // Initialize the pool with all numbers 1-75 in shuffled order
        private void InitializeNumberPool()
        {
            // Create a local list for shuffling
            List<int> allNumbers = new List<int>(MAX_NUMBERS);
            for (int i = 1; i <= MAX_NUMBERS; i++)
            {
                allNumbers.Add(i);
            }
            
            // Shuffle the numbers using Fisher-Yates algorithm
            for (int i = allNumbers.Count - 1; i > 0; i--)
            {
                int j = rng.Next(0, i + 1);
                // Swap elements
                int temp = allNumbers[i];
                allNumbers[i] = allNumbers[j];
                allNumbers[j] = temp;
            }
            
            // Store in NetworkArray
            for (int i = 0; i < allNumbers.Count; i++)
            {
                NumberPool.Set(i, allNumbers[i]);
            }
            
            // Set available numbers count
            AvailableNumbersCount = MAX_NUMBERS;
            
            Debug.Log($"Number pool initialized with {AvailableNumbersCount} numbers");
        }

        public int[] GenerateBingoCard()
        {
            if (rng == null)
            {
                Debug.LogWarning("RNG not initialized, initializing now");
                InitializeRNG();
            }

            int[] card = new int[25];
            bool[] usedNumbers = new bool[76]; // 1-75 for Bingo numbers

            // Set center space as FREE (0)
            card[12] = 0;
            usedNumbers[0] = true;

            // Generate numbers for each column
            for (int col = 0; col < 5; col++)
            {
                int min = col * 15 + 1;
                int max = min + 14;
                
                for (int row = 0; row < 5; row++)
                {
                    if (row == 2 && col == 2) continue; // Skip center space

                    int number;
                    do
                    {
                        number = rng.Next(min, max + 1);
                    } while (usedNumbers[number]);

                    usedNumbers[number] = true;
                    card[row * 5 + col] = number;
                }
            }
            
            return card;
        }

        // Get the next number from the pool
        public int GetNextNumber()
        {
            if (AvailableNumbersCount <= 0)
            {
                Debug.LogWarning("No more numbers available in the pool!");
                return -1;
            }
            
            // Get the next number from the pool (last available index)
            int nextNumber = NumberPool.Get(AvailableNumbersCount - 1);
            
            // Decrement available count
            AvailableNumbersCount--;
            
            Debug.Log($"Drew number {nextNumber} from pool. {AvailableNumbersCount} numbers remain.");
            return nextNumber;
        }
        
        // Reset the number generator to start a new round of bingo
        public void ResetNumberGenerator()
        {
            if (Object.HasStateAuthority)
            {
                // Generate a new seed for the next round
                NetworkSeed = UnityEngine.Random.Range(0, int.MaxValue);
                Debug.Log($"Host generated new seed for next round: {NetworkSeed}");
                
                // Re-initialize RNG with new seed
                InitializeRNG();
                
                // Refill and reshuffle the number pool
                InitializeNumberPool();
            }
        }
    }
} 