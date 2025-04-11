using UnityEngine;
using Fusion;
using System.Threading.Tasks;

public class NetworkRunnerHandler : MonoBehaviour
{
    [SerializeField] private NetworkRunner networkRunner;
    [SerializeField] private GameManager gameManager;

    private void Awake()
    {
        if (networkRunner == null)
            networkRunner = GetComponent<NetworkRunner>();
    }

    public async Task StartGame(GameMode mode)
    {
        var startGameArgs = new StartGameArgs()
        {
            GameMode = mode,
            SessionName = "BingoGame"
        };

        var result = await networkRunner.StartGame(startGameArgs);
        
        if (result.Ok)
        {
            Debug.Log($"Started game in {mode} mode");
        }
        else
        {
            Debug.LogError($"Failed to start game: {result.ShutdownReason}");
        }
    }
} 