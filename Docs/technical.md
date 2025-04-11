# Bingo Game - Technical Design Document

## Technology Stack
- Unity 6000.0.37f1
- Photon Fusion 2.0.5 (Shared Mode)
- UI Toolkit
- C# 8.0 (.NET Standard 2.1)

## Project Structure
```
Assets/
├── Scripts/
│   ├── Network/
│   │   ├── NetworkManager.cs
│   │   ├── NetworkPlayer.cs
│   │   └── NetworkRoom.cs
│   ├── Game/
│   │   ├── GameManager.cs
│   │   ├── BingoGame.cs
│   │   └── BingoCard.cs
│   ├── UI/
│   │   ├── Controllers/
│   │   │   ├── BaseUIController.cs
│   │   │   ├── LoginUIController.cs
│   │   │   ├── LobbyUIController.cs
│   │   │   ├── GameRoomUIController.cs
│   │   │   └── GameplayUIController.cs
│   │   └── Components/
│   │       ├── ButtonComponent.cs
│   │       ├── InputFieldComponent.cs
│   │       └── CardComponent.cs
│   └── Utils/
│       ├── ErrorHandler.cs
│       └── AudioManager.cs
├── UI/
│   ├── Layouts/
│   │   ├── Common/
│   │   │   ├── Button.uxml
│   │   │   ├── InputField.uxml
│   │   │   └── Card.uxml
│   │   └── Screens/
│   │       ├── Login.uxml
│   │       ├── Lobby.uxml
│   │       ├── GameRoom.uxml
│   │       └── Gameplay.uxml
│   ├── Styles/
│   │   ├── Common/
│   │   │   ├── Variables.uss
│   │   │   ├── Typography.uss
│   │   │   ├── Colors.uss
│   │   │   └── Animations.uss
│   │   └── Screens/
│   │       ├── Login.uss
│   │       ├── Lobby.uss
│   │       ├── GameRoom.uss
│   │       └── Gameplay.uss
│   └── Resources/
│       ├── Icons/
│       └── Fonts/
└── Scenes/
    ├── Login.unity
    ├── Lobby.unity
    ├── GameRoom.unity
    └── Gameplay.unity
```

## Core Systems Implementation

### 1. Network System (Photon Fusion - Shared Mode)
```csharp
// NetworkManager.cs
public class NetworkManager : MonoBehaviour
{
    private NetworkRunner _runner;
    private NetworkSceneManagerDefault _sceneManager;

    // Initialize Photon Fusion in Shared Mode
    public async void StartSharedMode()
    {
        _runner = gameObject.AddComponent<NetworkRunner>();
        _sceneManager = gameObject.AddComponent<NetworkSceneManagerDefault>();
        
        var startGameArgs = new StartGameArgs()
        {
            GameMode = GameMode.Shared,
            SessionName = "BingoGame",
            SceneManager = _sceneManager
        };
        
        await _runner.StartGame(startGameArgs);
    }
}

// NetworkPlayer.cs
public struct NetworkPlayer : INetworkStruct
{
    [Networked] public string PlayerName { get; set; }
    [Networked] public bool IsReady { get; set; }
    [Networked] public PlayerRef PlayerRef { get; set; }
}

// NetworkRoom.cs
public struct NetworkRoom : INetworkStruct
{
    [Networked] public string RoomName { get; set; }
    [Networked] public NetworkPlayer[] Players { get; set; }
    [Networked] public int MaxPlayers { get; set; }
}
```

### 2. Game State Management
```csharp
// GameManager.cs
public class GameManager : NetworkBehaviour
{
    public static GameManager Instance { get; private set; }
    
    [Networked] public NetworkDictionary<PlayerRef, NetworkPlayer> Players { get; }
    [Networked] public GameState CurrentState { get; set; }
    
    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
    }
}

// BingoGame.cs
public class BingoGame : NetworkBehaviour
{
    [Networked] public int[] DrawnNumbers { get; set; }
    [Networked] public int CurrentNumber { get; set; }
    [Networked] public bool IsGameActive { get; set; }
    
    public void DrawNumber()
    {
        if (!IsGameActive) return;
        
        // Server-side number drawing logic
        if (Runner.IsServer)
        {
            int newNumber = GetRandomUnusedNumber();
            CurrentNumber = newNumber;
            DrawnNumbers = DrawnNumbers.Append(newNumber).ToArray();
        }
    }
}
```

### 3. UI System (UI Toolkit)
```csharp
// BaseUIController.cs
public abstract class BaseUIController : MonoBehaviour
{
    protected UIDocument document;
    protected VisualElement root;
    
    protected virtual void Awake()
    {
        document = GetComponent<UIDocument>();
        root = document.rootVisualElement;
    }
}

// LoginUI.cs
public class LoginUI : BaseUIController
{
    private TextField nameField;
    private Button connectButton;
    private Label statusLabel;
    
    protected override void Awake()
    {
        base.Awake();
        nameField = root.Q<TextField>("name-field");
        connectButton = root.Q<Button>("connect-button");
        statusLabel = root.Q<Label>("status-label");
        
        connectButton.clicked += OnConnectClicked;
    }
}
```

## Scene Implementation Details

### 1. Login Scene
- **Purpose**: Player authentication and network connection
- **Implementation**:
  - UI Toolkit document with login form
  - NetworkManager initialization
  - Player name validation
  - Connection status feedback

### 2. Lobby Scene
- **Purpose**: Room management and player gathering
- **Implementation**:
  - Room list using UI Toolkit ListView
  - Room creation panel
  - Network room synchronization
  - Player count display

### 3. Game Room Scene
- **Purpose**: Pre-game player gathering
- **Implementation**:
  - Player list with ready status
  - Ready button with network sync
  - Start game conditions check
  - Server-side validation

### 4. Gameplay Scene
- **Purpose**: Core Bingo gameplay
- **Implementation**:
  - Bingo card grid using UI Toolkit Grid
  - Number drawing system
  - Marking system with network sync
  - Win condition detection
  - Server-side validation

## Performance Optimization

### 1. Network Optimization
- Use Networked properties only when necessary
- Implement server-side validation
- Optimize RPC calls
- Use NetworkDictionary for player management

### 2. UI Optimization
- Use USS for styling instead of inline styles
- Implement object pooling for UI elements
- Use VisualElement recycling
- Optimize layout calculations

### 3. Memory Management
- Implement proper cleanup in OnDestroy
- Use NetworkObject pooling
- Clear unused resources
- Monitor memory usage

## Error Handling

### 1. Network Errors
```csharp
public class NetworkErrorHandler : MonoBehaviour
{
    public void HandleNetworkError(NetworkError error)
    {
        switch (error)
        {
            case NetworkError.ConnectionFailed:
                ShowError("Connection failed. Please try again.");
                break;
            case NetworkError.RoomFull:
                ShowError("Room is full. Please try another room.");
                break;
            // Add more error cases
        }
    }
}
```

### 2. Game State Validation
```csharp
public class GameStateValidator
{
    public bool ValidateGameState(GameState state)
    {
        // Server-side validation logic
        if (!Runner.IsServer) return false;
        
        // Add validation rules
        return true;
    }
}
```

## Build and Deployment

### 1. Build Settings
- Target platform: Windows
- Graphics API: DirectX 11
- Scripting backend: Mono
- API compatibility level: .NET Standard 2.1

### 2. Development Workflow
1. Set up Photon Fusion Shared Mode
2. Implement core network systems
3. Create UI layouts
4. Implement game logic
5. Add error handling
6. Optimize performance
7. Test and debug
8. Build and deploy

## Timeline
1. Project Setup (30 minutes)
2. Network Implementation (1 hour)
3. UI Development (1 hour)
4. Game Logic (1 hour)
5. Testing and Polish (30 minutes)

Total: 4 hours
