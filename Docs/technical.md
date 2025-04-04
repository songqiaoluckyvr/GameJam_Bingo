# Multiplayer Bingo Game - Technical Specification

## Technology Stack
- Unity 6000.0.37f1 LTS
- Photon Fusion (for networking)
- HOTween v2 (DOTween) (for animations)
- TextMeshPro (for UI text)

## Project Structure
```
Assets/
├── _Project/
│   ├── Animations/
│   │   ├── UI/
│   │   └── Effects/
│   ├── Audio/
│   │   ├── SFX/
│   │   └── Music/
│   ├── Materials/
│   │   └── UI/
│   ├── Prefabs/
│   │   ├── UI/
│   │   └── Game/
│   ├── Resources/
│   │   └── FusionPrefabs/
│   ├── Scenes/
│   │   ├── Boot.unity
│   │   ├── Menu.unity
│   │   └── Game.unity
│   ├── Scripts/
│   │   ├── Core/
│   │   ├── Game/
│   │   ├── Networking/
│   │   ├── UI/
│   │   └── Utils/
│   ├── Settings/
│   └── UI/
│       ├── Sprites/
│       └── Fonts/
└── Plugins/
    └── Photon/
```

## Architecture Overview

### Core Systems

1. **Game State Management**
```csharp
// Manages high-level game state using state pattern
public class GameStateManager : NetworkBehaviour
{
    public enum GameState { Lobby, PreGame, Playing, GameOver }
    [Networked] private GameState CurrentState { get; set; }
}
```

2. **Network Manager**
```csharp
// Handles Fusion connection and room management
public class NetworkManager : MonoBehaviour
{
    private NetworkRunner runner;
    public async Task StartGame(GameMode mode, string roomName)
    public void OnPlayerJoined(PlayerRef player)
}
```

3. **Room Management**
```csharp
// Manages room creation and listing
public class RoomManager : NetworkBehaviour
{
    [Networked] public string RoomName { get; set; }
    public void CreateRoom(string roomName)
    public void JoinRoom(string roomName)
}
```

### Game Systems

1. **Bingo Card System**
```csharp
// Handles card generation and validation
public class BingoCard : NetworkBehaviour
{
    [Networked] private int[,] Numbers { get; set; }
    [Networked] private bool[,] Marked { get; set; }
    
    public void GenerateCard()
    public void MarkNumber(int number)
    public bool ValidateWin(WinPattern pattern)
}
```

2. **Number Generator**
```csharp
// Master client handles number generation
public class NumberGenerator : NetworkBehaviour
{
    [Networked] private HashSet<int> DrawnNumbers { get; set; }
    [Networked] private float DrawInterval { get; set; } = 30f;
    
    [Rpc]
    private void RPC_BroadcastNumber(int number)
}
```

3. **Win Condition Checker**
```csharp
public class WinChecker : NetworkBehaviour
{
    public enum WinPattern { Horizontal, Vertical, Diagonal }
    
    [Rpc]
    public void RPC_ValidateWinClaim(PlayerRef player, WinPattern pattern)
}
```

## Networking Implementation

### Networked Properties
1. **Player Properties**
```csharp
public class PlayerData : NetworkBehaviour
{
    [Networked] public string PlayerName { get; set; }
    [Networked] public bool IsReady { get; set; }
    [Networked] public BingoCard Card { get; set; }
}
```

2. **Room Properties**
```csharp
public class RoomData : NetworkBehaviour
{
    [Networked] public GameState CurrentState { get; set; }
    [Networked] public HashSet<int> DrawnNumbers { get; set; }
}
```

### Network Events
```csharp
public class NetworkEvents
{
    public const int MARK_NUMBER = 1;
    public const int CLAIM_WIN = 2;
    public const int GAME_START = 3;
    public const int DRAW_NUMBER = 4;
}
```

## Scene Flow
1. **Boot Scene**
   - Initialize core systems
   - Connect to Photon
   - Load Menu scene

2. **Menu Scene**
   - Room creation/listing
   - Player customization
   - Settings

3. **Game Scene**
   - Bingo card display
   - Number announcements
   - Game UI
   - Win conditions

## UI System
Using Unity's UI Toolkit for modern, responsive UI:

1. **Main Menu UI**
```csharp
public class MainMenuUI : MonoBehaviour
{
    public void OnCreateRoomClick()
    public void OnJoinRoomClick()
    public void UpdateRoomList(List<RoomInfo> rooms)
}
```

2. **Game UI**
```csharp
public class GameUI : MonoBehaviour
{
    public void UpdateDrawnNumber(int number)
    public void ShowWinScreen(Player winner)
    public void UpdateTimer(float time)
}
```

## Implementation Plan

### Phase 1: Core Setup (Week 1)
1. Project structure setup
2. PUN integration
3. Basic networking system
4. Scene management

### Phase 2: Room System (Week 1)
1. Room creation
2. Room listing
3. Room joining
4. Player synchronization

### Phase 3: Game Mechanics (Week 2)
1. Bingo card generation
2. Number drawing system
3. Card marking
4. Win condition validation

### Phase 4: UI Implementation (Week 2)
1. Menu UI
2. Game UI
3. Animations
4. Visual feedback

### Phase 5: Polish (Week 3)
1. Sound effects
2. Visual effects
3. Error handling
4. Network optimization

### Phase 6: Testing (Week 3)
1. Multiplayer testing
2. Network condition testing
3. Bug fixing
4. Performance optimization

## Best Practices

### Network Optimization
1. Use PUN's built-in room properties for game state
2. Minimize RPCs by batching updates
3. Use PhotonView observers wisely
4. Implement proper fallbacks for network issues

### Unity Best Practices
1. Object pooling for instantiated elements
2. Proper use of MonoBehaviour lifecycle
3. Scriptable Objects for configuration
4. Event-driven communication

### Code Organization
1. Namespace organization:
```csharp
namespace BingoGame
{
    namespace Core { }
    namespace Network { }
    namespace UI { }
    namespace Game { }
    namespace Utils { }
}
```

2. Dependency injection where applicable
3. Clear separation of concerns
4. Interface-driven development

## Testing Strategy
1. Unit tests for game logic
2. Integration tests for network communication
3. Stress tests for concurrent players
4. Different network condition testing

## Deployment Checklist
1. Photon App ID configuration
2. Build settings verification
3. Scene inclusion check
4. Resource compression settings
5. Performance profiling
6. Network testing in build

## Monitoring and Analytics
1. Photon Dashboard integration
2. Basic analytics implementation
3. Error logging system
4. Performance metrics tracking 