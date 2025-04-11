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
        
        [Networked, Capacity(75)]
        private NetworkArray<int> GeneratedNumbers { get; }
        
        private int currentNumberIndex = 0;
        private const int MAX_NUMBERS = 75;

        public override void Spawned()
        {
            if (Object.HasStateAuthority)
            {
                // Generate a new seed only on the host
                NetworkSeed = UnityEngine.Random.Range(0, int.MaxValue);
                Debug.Log($"Host generated seed: {NetworkSeed}");
                
                // Generate all numbers upfront
                GenerateAllNumbers();
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
            Debug.Log($"Initialized RNG with seed: {NetworkSeed}");
        }

        private void GenerateAllNumbers()
        {
            var numbers = new List<int>();
            var usedNumbers = new HashSet<int>();

            // Generate all possible numbers (1-75)
            while (numbers.Count < MAX_NUMBERS)
            {
                int number = rng.Next(1, MAX_NUMBERS + 1);
                if (usedNumbers.Add(number))
                {
                    numbers.Add(number);
                }
            }

            // Store in NetworkArray
            for (int i = 0; i < numbers.Count; i++)
            {
                GeneratedNumbers.Set(i, numbers[i]);
            }

            string numbersStr = string.Join(", ", numbers);
            Debug.Log($"Generated all numbers: {numbersStr}");
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

            // Print the bingo card in a grid format
            string gridDebug = "\nBINGO Card Grid:\n";
            gridDebug += "B    I    N    G    O\n";
            gridDebug += "---------------------\n";
            for (int row = 0; row < 5; row++)
            {
                for (int col = 0; col < 5; col++)
                {
                    int number = card[row * 5 + col];
                    gridDebug += number == 0 ? "FREE".PadRight(5) : number.ToString().PadRight(5);
                }
                gridDebug += "\n";
            }
            Debug.Log(gridDebug);
            
            return card;
        }

        public int GetNextNumber()
        {
            if (currentNumberIndex >= MAX_NUMBERS)
            {
                Debug.LogWarning("No more numbers available!");
                return -1;
            }

            return GeneratedNumbers.Get(currentNumberIndex++);
        }
    }
} 