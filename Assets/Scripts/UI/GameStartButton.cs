using UnityEngine;
using UnityEngine.UI;
using Fusion;

public class GameStartButton : MonoBehaviour
{
    [SerializeField] private GameManager gameManager;
    [SerializeField] private NetworkRunnerHandler networkRunner;
    private Button button;

    private void Awake()
    {
        button = GetComponent<Button>();
        button.onClick.AddListener(OnStartButtonClicked);
    }

    private async void OnStartButtonClicked()
    {
        if (networkRunner != null)
        {
            // Start as host
            await networkRunner.StartGame(GameMode.Host);
            
            if (gameManager != null)
            {
                gameManager.StartGame();
            }
        }
    }
} 