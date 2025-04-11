using UnityEngine;
using UnityEngine.UIElements;
using Fusion;
using System.Threading.Tasks;
using BingoGame.Network;
using UnityEngine.SceneManagement;

namespace BingoGame.UI.Controllers
{
    public class LoginUIController : BaseUIController
    {
        // UI Elements
        private TextField nameField;
        private Button connectButton;
        private Label statusLabel;

        // Network Manager reference
        private NetworkManager networkManager;

        protected override void Awake()
        {
            base.Awake();
            
            // Get UI elements
            nameField = root.Q<TextField>("playerName");
            connectButton = root.Q<Button>("connectButton");
            statusLabel = root.Q<Label>("statusMessage");

            // Get NetworkManager
            networkManager = FindObjectOfType<NetworkManager>();
            if (networkManager == null)
            {
                Debug.LogError("NetworkManager not found in scene!");
                return;
            }

            // Add click event
            connectButton.clicked += OnConnectClicked;
        }

        private void OnConnectClicked()
        {
            string playerName = nameField.value.Trim();
            
            // Validate player name
            if (string.IsNullOrEmpty(playerName))
            {
                ShowError("Please enter a player name");
                return;
            }

            if (playerName.Length < 3)
            {
                ShowError("Player name must be at least 3 characters");
                return;
            }

            // Disable UI elements while connecting
            SetUIInteractable(false);
            ShowStatus("Connecting...");

            // Start connection process
            ConnectToServer(playerName);
        }

        private async void ConnectToServer(string playerName)
        {
            try
            {
                // Start network connection
                await networkManager.StartSharedMode();
                
                // If successful, transition to lobby scene
                ShowStatus("Connected! Loading lobby...");
                await Task.Delay(1000); // Give time to read the message
                SceneManager.LoadScene("Lobby");
            }
            catch (System.Exception e)
            {
                // Handle connection error
                ShowError($"Connection failed: {e.Message}");
                SetUIInteractable(true);
            }
        }

        private void ShowError(string message)
        {
            statusLabel.text = message;
            statusLabel.style.color = new StyleColor(Color.red);
        }

        private void ShowStatus(string message)
        {
            statusLabel.text = message;
            statusLabel.style.color = new StyleColor(Color.white);
        }

        private void SetUIInteractable(bool interactable)
        {
            nameField.SetEnabled(interactable);
            connectButton.SetEnabled(interactable);
        }

        private void OnDestroy()
        {
            // Clean up event subscription
            if (connectButton != null)
            {
                connectButton.clicked -= OnConnectClicked;
            }
        }
    }
} 