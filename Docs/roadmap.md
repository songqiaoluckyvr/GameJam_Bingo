# Bingo Game - Development Roadmap

## Milestone 1: Basic UI Setup (1 hour)
### Step 1: Project Setup
- [x] Create new Unity project
- [x] Set up Photon Fusion 2.0.5
- [ ] Configure UI Toolkit
- [ ] Create basic folder structure

### Step 2: Login Scene UI
**User Story**: As a player, I want to enter my name and connect to the game server so I can join a game.

**Acceptance Criteria**:
- [ ] Create Login.uxml layout
- [ ] Implement Login.uss styles
- [ ] Add name input field
- [ ] Add connect button
- [ ] Add status message label
- [ ] Test: Scene should show all UI elements with correct styling

**Test Steps**:
1. Enter Play Mode
2. Verify all UI elements are visible
3. Verify input field accepts text
4. Verify button is clickable
5. Verify status label is visible

### Step 3: Lobby Scene UI
**User Story**: As a player, I want to see available rooms and create new ones so I can join or host a game.

**Acceptance Criteria**:
- [ ] Create Lobby.uxml layout
- [ ] Implement Lobby.uss styles
- [ ] Add room list container
- [ ] Add create room panel
- [ ] Add room card template
- [ ] Test: Scene should show room list and create room panel

**Test Steps**:
1. Enter Play Mode
2. Verify room list is visible
3. Verify create room panel is visible
4. Verify room card template is properly styled

## Milestone 2: Network Connection (1 hour)
### Step 1: Login Functionality
**User Story**: As a player, I want to connect to the game server with my name so I can access the lobby.

**Acceptance Criteria**:
- [ ] Implement NetworkManager
- [ ] Add Photon Fusion initialization
- [ ] Connect login UI to network
- [ ] Add error handling
- [ ] Test: Should connect to Photon and show success/error messages

**Test Steps**:
1. Enter Play Mode
2. Enter player name
3. Click connect
4. Verify connection status
5. Verify error messages for invalid input

### Step 2: Room Management UI
**User Story**: As a player, I want to see and interact with available rooms in real-time.

**Acceptance Criteria**:
- [ ] Implement room list updates
- [ ] Add room creation functionality
- [ ] Add room joining functionality
- [ ] Test: Should show real-time room updates

**Test Steps**:
1. Enter Play Mode
2. Create a new room
3. Verify room appears in list
4. Try to join a room
5. Verify room joining feedback

## Milestone 3: Game Room (1 hour)
### Step 1: Game Room UI
**User Story**: As a player, I want to see other players and indicate my readiness.

**Acceptance Criteria**:
- [ ] Create GameRoom.uxml layout
- [ ] Implement GameRoom.uss styles
- [ ] Add player list
- [ ] Add ready button
- [ ] Test: Should show player list and ready button

**Test Steps**:
1. Enter Play Mode
2. Verify player list is visible
3. Verify ready button is clickable
4. Verify player status indicators

### Step 2: Player Synchronization
**User Story**: As a player, I want to see other players' ready status in real-time.

**Acceptance Criteria**:
- [ ] Implement player list updates
- [ ] Add ready status synchronization
- [ ] Add start game conditions
- [ ] Test: Should show real-time player updates

**Test Steps**:
1. Enter Play Mode with multiple players
2. Toggle ready status
3. Verify status updates for all players
4. Verify game start when conditions met

## Milestone 4: Gameplay (1 hour)
### Step 1: Bingo Card UI
**User Story**: As a player, I want to see my bingo card and mark numbers.

**Acceptance Criteria**:
- [ ] Create Gameplay.uxml layout
- [ ] Implement Gameplay.uss styles
- [ ] Add bingo card grid
- [ ] Add number marking system
- [ ] Test: Should show bingo card and allow marking

**Test Steps**:
1. Enter Play Mode
2. Verify bingo card is visible
3. Try marking numbers
4. Verify marked numbers are highlighted

### Step 2: Number Drawing
**User Story**: As a player, I want to see numbers being drawn automatically.

**Acceptance Criteria**:
- [ ] Implement number display
- [ ] Add automatic number drawing
- [ ] Add drawn numbers history
- [ ] Test: Should show numbers being drawn

**Test Steps**:
1. Enter Play Mode
2. Verify number display
3. Wait for automatic drawing
4. Verify drawn numbers history

### Step 3: Win Condition
**User Story**: As a player, I want to know when I've won and be able to claim my victory.

**Acceptance Criteria**:
- [ ] Implement win condition checking
- [ ] Add BINGO button
- [ ] Add winner announcement
- [ ] Test: Should detect wins and allow claiming

**Test Steps**:
1. Enter Play Mode
2. Mark winning pattern
3. Verify BINGO button activates
4. Click BINGO button
5. Verify winner announcement

## Milestone 5: Polish (30 minutes)
### Step 1: Visual Feedback
**User Story**: As a player, I want to see clear visual feedback for all actions.

**Acceptance Criteria**:
- [ ] Add number drawing animations
- [ ] Add marking animations
- [ ] Add win celebration
- [ ] Test: Should show all animations

**Test Steps**:
1. Enter Play Mode
2. Verify all animations work
3. Verify performance is smooth

### Step 2: Error Handling
**User Story**: As a player, I want to see clear error messages when something goes wrong.

**Acceptance Criteria**:
- [ ] Add network error handling
- [ ] Add game state error handling
- [ ] Add UI error states
- [ ] Test: Should handle all error cases

**Test Steps**:
1. Enter Play Mode
2. Test various error scenarios
3. Verify error messages are clear

## Testing Checklist
After each milestone:
- [ ] Verify all UI elements are visible
- [ ] Test all interactive elements
- [ ] Check network synchronization
- [ ] Verify error handling
- [ ] Test performance in Play Mode
- [ ] Document any issues found
