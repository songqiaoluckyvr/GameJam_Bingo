# Bingo Game - Technical Design Document

## Technology Stack
- Unity 6000.0.37f1
- Photon Fusion 2.0.5
- UI Toolkit
- C# 8.0 (.NET Standard 2.1)

## Project Structure
```
Assets/
├── Scripts/
│   ├── Core/
│   │   ├── GameManager.cs
│   │   ├── NetworkManager.cs
│   │   └── SceneManager.cs
│   ├── Network/
│   │   ├── NetworkPlayer.cs
│   │   ├── NetworkRoom.cs
│   │   └── NetworkGameState.cs
│   ├── Game/
│   │   ├── BingoCard.cs
│   │   ├── BingoNumber.cs
│   │   ├── BingoPattern.cs
│   │   └── BingoGame.cs
│   └── UI/
│       ├── LoginUI.cs
│       ├── LobbyUI.cs
│       ├── GameRoomUI.cs
│       └── GameplayUI.cs
├── UI/
│   ├── Styles/
│   │   ├── Common.uss
│   │   ├── Login.uss
│   │   ├── Lobby.uss
│   │   ├── GameRoom.uss
│   │   └── Gameplay.uss
│   └── Layouts/
│       ├── Login.uxml
│       ├── Lobby.uxml
│       ├── GameRoom.uxml
│       └── Gameplay.uxml
└── Scenes/
    ├── Login.unity
    ├── Lobby.unity
    ├── GameRoom.unity
    └── Gameplay.unity
```

## Core Systems Implementation

### 1. Network System (Photon Fusion 2.0.5)
```csharp
// NetworkManager.cs
public class NetworkManager : NetworkBehaviour
{
    // Network callbacks
    public override void Spawned()
    {
        // Initialize network state
    }

    public override void FixedUpdateNetwork()
    {
        // Handle network updates
    }
}

// NetworkPlayer.cs
public class NetworkPlayer : NetworkBehaviour
{
    [Networked] public string PlayerName { get; set; }
    [Networked] public bool IsReady { get; set; }
    [Networked] public int PlayerId { get; set; }
}

// NetworkRoom.cs
public class NetworkRoom : NetworkBehaviour
{
    [Networked] public string RoomName { get; set; }
    [Networked] public int MaxPlayers { get; set; }
    [Networked] public NetworkArray<NetworkPlayer> Players { get; }
}
```

### 2. Game State Management
```csharp
// GameManager.cs
public class GameManager : MonoBehaviour
{
    private static GameManager _instance;
    public static GameManager Instance => _instance;

    private void Awake()
    {
        if (_instance == null)
        {
            _instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }
}

// BingoGame.cs
public class BingoGame : NetworkBehaviour
{
    [Networked] public NetworkArray<int> DrawnNumbers { get; }
    [Networked] public int CurrentNumber { get; set; }
    [Networked] public bool IsGameActive { get; set; }
}
```

### 3. UI System (UI Toolkit)
```csharp
// Base UI Controller
public abstract class BaseUIController : MonoBehaviour
{
    protected VisualElement root;
    protected UIDocument uiDocument;

    protected virtual void Awake()
    {
        uiDocument = GetComponent<UIDocument>();
        root = uiDocument.rootVisualElement;
    }
}

// LoginUI.cs
public class LoginUI : BaseUIController
{
    private TextField nameField;
    private Button connectButton;
    private Label statusLabel;

    private void Start()
    {
        nameField = root.Q<TextField>("name-field");
        connectButton = root.Q<Button>("connect-button");
        statusLabel = root.Q<Label>("status-label");
    }
}
```

## Scene Implementation Details

### 1. Login Scene
- **Network Setup**: Initialize Photon Fusion
- **UI Elements**: 
  - Name input field
  - Connect button
  - Status message
- **Transitions**: 
  - Success: Load Lobby scene
  - Error: Show error message

### 2. Lobby Scene
- **Network Features**:
  - Room list synchronization
  - Room creation/joining
- **UI Elements**:
  - Room list with pagination
  - Create room panel
  - Room info cards
- **Transitions**:
  - Join room: Load Game Room scene
  - Create room: Create new room and transition

### 3. Game Room Scene
- **Network Features**:
  - Player list synchronization
  - Ready state management
- **UI Elements**:
  - Player list
  - Ready button
  - Start game indicator
- **Transitions**:
  - All players ready: Load Gameplay scene

### 4. Gameplay Scene
- **Network Features**:
  - Number drawing synchronization
  - Bingo card state management
  - Win condition checking
- **UI Elements**:
  - Bingo card grid
  - Number display
  - Player list
  - BINGO button
- **Transitions**:
  - Game end: Show winner and return to lobby

## Performance Optimization

### 1. Network Optimization
- Use NetworkArray for efficient state synchronization
- Implement client-side prediction for number drawing
- Use Networked properties only when necessary

### 2. UI Optimization
- Use USS for styling instead of inline styles
- Implement object pooling for number displays
- Cache frequently accessed UI elements

### 3. Memory Management
- Implement proper cleanup in scene transitions
- Use weak references for UI event handlers
- Clear network callbacks on scene changes

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
                // Handle connection failure
                break;
            case NetworkError.RoomFull:
                // Handle room full error
                break;
            // Add more error cases
        }
    }
}
```

### 2. Game State Errors
```csharp
public class GameStateValidator
{
    public bool ValidateGameState(BingoGame game)
    {
        // Validate game state
        return true;
    }
}
```

## Testing Strategy

### 1. Unit Tests
- Network state synchronization
- Game logic validation
- UI interaction testing

### 2. Integration Tests
- Scene transitions
- Network room management
- Game flow testing

### 3. Performance Tests
- Network bandwidth usage
- UI rendering performance
- Memory usage monitoring

## Build and Deployment

### 1. Build Settings
- Target platform: Windows
- Graphics API: DirectX 11
- Scripting backend: Mono
- API compatibility level: .NET Standard 2.1

### 2. Development Workflow
1. Set up Photon Fusion project
2. Configure network settings
3. Implement core systems
4. Create UI layouts
5. Implement game logic
6. Test and optimize
7. Build and deploy

## Timeline
1. Network Setup (1 hour)
2. Core Systems (1 hour)
3. UI Implementation (1 hour)
4. Game Logic (30 minutes)
5. Testing and Optimization (30 minutes)
