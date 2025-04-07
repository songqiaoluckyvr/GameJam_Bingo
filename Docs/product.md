# Bingo Game Jam - Game Design Document

## Overview
A simple multiplayer Bingo game created for a 4-hour game jam. This is a minimum viable product (MVP) focused on core gameplay mechanics and basic multiplayer functionality.

## Game Flow

### 1. Login Scene
- **Purpose**: Player identification and network connection
- **Features**:
  - Text input field for player name
  - Connect button
  - Simple UI with minimal styling
  - Error handling for invalid names
  - Network connection status indicator

### 2. Lobby Scene
- **Purpose**: Room management and player gathering
- **Features**:
  - Room List Display
    - Room name
    - Room creator
    - Player count (current/maximum) format: "2/30"
    - Join button for each room
  - Room Creation
    - Create room button
    - Simple room name input
  - Room Joining
    - Click to join functionality
    - Error handling for full rooms

### 3. Game Room Scene
- **Purpose**: Pre-game player gathering and readiness check
- **Features**:
  - Player List Display
    - Player names
    - Ready status indicator
  - Ready Button
    - Toggle functionality
    - Visual feedback
  - Start Game Conditions
    - Minimum 2 players required
    - All players must be ready
    - Automatic transition to gameplay when conditions met

### 4. Gameplay Scene
- **Purpose**: Core Bingo gameplay
- **Features**:
  - Bingo Card
    - 5x5 grid
    - Numbers 1-75
    - Marking system
  - Number Drawing
    - Automatic every 30 seconds
    - Visual feedback (number display)
    - Audio feedback (draw sound)
  - Number Validation
    - Automatic validation of marked numbers
    - Visual feedback for correct/incorrect marks
  - Winning Conditions
    - Classic Bingo patterns only
    - Automatic pattern detection
    - BINGO! button activation when pattern complete
  - Game End
    - Winner announcement
    - Game conclusion when BINGO! button pressed

## Technical Requirements

### Network
- Photon Fusion 2 for multiplayer functionality
- Room-based matchmaking
- Player synchronization
- Game state management

### UI/UX
- Minimalist design
- Clear status indicators
- Responsive feedback
- Error messages for common scenarios

### Audio
- Basic sound effects for:
  - Number drawing
  - Marking numbers
  - Winning
  - Error notifications

## MVP Scope
- Core gameplay functionality
- Basic multiplayer support
- Essential UI elements
- Minimal visual polish
- Basic sound effects

## Out of Scope
- Advanced graphics
- Custom bingo patterns
- Chat system
- Player statistics
- Custom room settings
- Spectator mode
- Mobile support
- Leaderboards
- Achievements

## Success Criteria
- Functional multiplayer gameplay
- Working room system
- Complete bingo game mechanics
- Basic error handling
- Smooth transitions between scenes
