using UnityEngine;
using Fusion;
using System.Threading.Tasks;

namespace BingoGame.Network
{
    public class NetworkManager : MonoBehaviour
    {
        private NetworkRunner _runner;
        private NetworkSceneManagerDefault _sceneManager;

        // Initialize Photon Fusion in Shared Mode
        public async Task StartSharedMode()
        {
            // Create runner
            _runner = gameObject.AddComponent<NetworkRunner>();
            _runner.ProvideInput = true;

            // Create scene manager
            _sceneManager = gameObject.AddComponent<NetworkSceneManagerDefault>();

            // Start game in Shared Mode
            var startGameArgs = new StartGameArgs()
            {
                GameMode = GameMode.Shared,
                SessionName = "BingoGame",
                SceneManager = _sceneManager,
                PlayerCount = 4 // Maximum 4 players per room
            };

            // Start the game
            var result = await _runner.StartGame(startGameArgs);

            // Check if connection was successful
            if (!result.Ok)
            {
                throw new System.Exception($"Failed to start game: {result.ShutdownReason}");
            }
        }

        private void OnDestroy()
        {
            // Clean up
            if (_runner != null)
            {
                _runner.Shutdown();
            }
        }
    }
} 