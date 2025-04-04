# Multiplayer Bingo Game - Development Roadmap

## Project Setup (Day 1)
1. **Initial Setup**
   - Create Unity project (6000.0.37f1 LTS)
   - Import essential packages:
     - Photon Fusion
     - HOTween v2 (DOTween)
     - TextMeshPro
   - Setup project structure as defined in technical.md
   - Configure version control
   - ✓ Playmode Test: Project opens without errors

2. **Fusion Integration**
   - Initialize Fusion
   - Configure NetworkRunner
   - Setup basic NetworkManager
   - Implement connection handling
   - Configure network prefabs
   - ✓ Playmode Test: 
     - Can enter playmode
     - NetworkRunner initializes
     - Basic connection to Photon servers works
     - No errors in console

3. **Basic Scene Setup**
   - Create and configure three scenes:
     - Boot.unity
     - Menu.unity
     - Game.unity
   - Setup basic scene navigation
   - Implement scene loading system
   - ✓ Playmode Test:
     - Boot scene loads automatically
     - Can navigate to Menu scene
     - Basic UI elements are visible
     - No scene transition errors

## Milestone 1: Room System (Days 2-4)
### Day 2: Room List UI
1. **Menu Scene UI**
   - Design and implement room list panel
   - Create room button
   - Room list item prefab
   - Basic styling and animations
   - Placeholder data for testing
   - ✓ Playmode Test:
     - Room list panel is visible
     - Create room button is clickable
     - Room list items show placeholder data
     - UI animations work smoothly
     - No UI errors in console

### Day 3: Player Name & Room Creation UI
1. **Player Name Input**
   - Name input field
   - Name validation
   - Name persistence
   - ✓ Playmode Test:
     - Can enter player name
     - Name validation works (min/max length)
     - Name persists between scene loads
     - Error messages show for invalid names

2. **Create Room Panel**
   - Room name input field
   - Create button
   - Cancel button
   - Error message display
   - ✓ Playmode Test:
     - Can enter room name
     - Create and Cancel buttons work
     - Error messages show for invalid room names
     - Panel opens/closes smoothly

### Day 4: Room Functionality
1. **Room Management**
   - Implement RoomManager
   - Room creation logic
   - Room listing updates
   - Room joining logic
   - Player name synchronization
   - ✓ Playmode Test:
     - Can create a new room
     - Room appears in room list
     - Can join an existing room
     - Player names sync between clients
     - Room list updates in real-time

## Milestone 2: Bingo Card System (Days 5-7)
### Day 5: Card UI
1. **Bingo Card Design**
   - Create card grid layout
   - Number cell prefab
   - Selection/marking visual states
   - Card header with player name
   - ✓ Playmode Test:
     - Card grid displays correctly
     - Cells are properly sized and spaced
     - Selection states work (hover, selected)
     - Player name shows in header
     - Card animations work smoothly

### Day 6: Card Generation
1. **Number Generation**
   - Implement BingoCard class
   - Random number generation logic
   - Number validation
   - ✓ Playmode Test:
     - Card generates with random numbers
     - Numbers are within valid range (1-75)
     - No duplicate numbers in same card
     - Numbers are properly distributed
     - Card generation is consistent

### Day 7: Network Sync
1. **Card Synchronization**
   - Implement card data serialization
   - Network instantiation
   - Player-card association
   - ✓ Playmode Test:
     - Cards sync between players
     - Each player sees their own card
     - Card data persists between scene loads
     - No desync issues between clients

## Milestone 3: Game Flow (Days 8-10)
### Day 8: Pre-game UI
1. **Ready System Interface**
   - Ready button
   - Player status display with names
   - Countdown timer
   - ✓ Playmode Test:
     - Ready button is clickable
     - Player status updates in real-time
     - Countdown timer works
     - All players ready triggers game start
     - Status syncs between clients

### Day 9: Number Drawing UI
1. **Number Announcement**
   - Current number display
   - Number history panel
   - Drawing countdown timer
   - Visual effects
   - ✓ Playmode Test:
     - Current number displays clearly
     - Number history updates correctly
     - Timer counts down properly
     - Visual effects work smoothly
     - All players see same numbers

### Day 10: Game Logic
1. **Core Game Flow**
   - Implement NumberGenerator
   - Auto-drawing system
   - Number validation
   - Game state management
   - ✓ Playmode Test:
     - Numbers draw automatically
     - No duplicate numbers
     - Game state transitions work
     - All players stay in sync
     - Can complete a full game round

## Milestone 4: Win Condition (Days 11-12)
### Day 11: Win UI
1. **Win Detection Interface**
   - "BINGO!" button
   - Win pattern highlights
   - Winner announcement screen with player name
   - ✓ Playmode Test:
     - BINGO button is clickable
     - Pattern highlights work
     - Winner screen shows correct player
     - Victory animations play
     - All players see winner announcement

### Day 12: Win Logic
1. **Win Validation**
   - Implement WinChecker
   - Pattern validation
   - Network win announcement
   - ✓ Playmode Test:
     - Valid patterns are recognized
     - Invalid patterns are rejected
     - Win state syncs between players
     - First valid claim wins
     - No false positives

## Milestone 5: Polish & Optimization (Days 13-15)
### Day 13: Visual Polish
1. **UI Enhancement**
   - Add animations
   - Implement transitions
   - Add visual feedback
   - ✓ Playmode Test:
     - All animations are smooth
     - Transitions work without glitches
     - Visual feedback is clear
     - UI remains responsive
     - No performance issues

### Day 14: Audio System
1. **Sound Implementation**
   - Add UI sound effects
   - Number call voices
   - Win celebration sounds
   - Background music
   - ✓ Playmode Test:
     - All sounds play correctly
     - Volume levels are appropriate
     - No audio glitches
     - Music transitions smoothly
     - Audio syncs between clients

### Day 15: Performance
1. **Optimization**
   - Network optimization
   - UI performance
   - Memory management
   - ✓ Playmode Test:
     - Game runs at target FPS
     - Network latency is acceptable
     - Memory usage is stable
     - No memory leaks
     - Performance is consistent across clients

## Testing Milestones
After each milestone:
1. **Unit Testing**
   - Core logic tests
   - UI interaction tests
   - Network behavior tests

2. **Integration Testing**
   - Cross-scene flow
   - Network synchronization
   - State management

3. **Multiplayer Testing**
   - 2-player sessions
   - Edge case testing
   - Network condition testing

## Duplicated Information Extracted
The following information was duplicated across product.md and technical.md:
1. Room Management features
2. Game flow description
3. Win conditions
4. Core gameplay mechanics

These have been consolidated in the product.md, while technical.md now focuses on implementation details.

## Definition of Done for Each Feature
1. UI implementation complete
2. Core functionality working
3. Network synchronization implemented
4. Playable in editor
5. Tested with multiple clients
6. No blocking bugs
7. Basic error handling
8. Performance acceptable

## Daily Routine
1. Morning: UI implementation
2. Afternoon: Core functionality
3. Evening: Testing and fixes
4. End of day: Playable build

## Risk Management
1. **Network Issues**
   - Early Fusion integration
   - Continuous multiplayer testing
   - Fallback mechanisms

2. **Performance**
   - Regular profiling
   - Optimization after each milestone
   - Load testing

3. **Scope Creep**
   - Stick to MVP features
   - Daily progress review
   - Clear milestone boundaries 