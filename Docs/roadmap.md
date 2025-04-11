# Bingo Game - Development Roadmap

## Milestone 1: Basic UI Setup (1 hour)
### Step 1: Project Setup
- [x] Create new Unity project
- [x] Set up Photon Fusion 2.0.5 (Shared Mode)
- [x] Configure UI Toolkit
  - [x] Install UI Toolkit package
  - [x] Create UI Document for each scene
    - [x] Login Scene
      - [x] Create Login.uxml in UI/Layouts/Screens
      - [x] Add container with flex layout
      - [x] Add title "BINGO" (centered)
      - [x] Add name input field
      - [x] Add connect button
      - [x] Add status message label
      - [x] Link Login.uss style sheet
      - [x] Test: Verify layout in UI Builder
      - [x] Create LoginUI.cs script
      - [x] Implement network connection
      - [x] Add scene transition logic
    - [ ] Lobby Scene
      - [ ] Create Lobby.uxml in UI/Layouts/Screens
      - [ ] Add two-column layout
      - [ ] Left column: Room List (ListView)
      - [ ] Right column: Create Room Panel
      - [ ] Add room card template
      - [ ] Add create room form
      - [ ] Link Lobby.uss style sheet
      - [ ] Test: Verify layout in UI Builder
    - [ ] Game Room Scene
      - [ ] Create GameRoom.uxml in UI/Layouts/Screens
      - [ ] Add player list container
      - [ ] Add ready button
      - [ ] Add player status indicators
      - [ ] Add start game message
      - [ ] Link GameRoom.uss style sheet
      - [ ] Test: Verify layout in UI Builder
    - [ ] Gameplay Scene
      - [ ] Create Gameplay.uxml in UI/Layouts/Screens
      - [ ] Add bingo card grid (5x5)
      - [ ] Add number display area
      - [ ] Add drawn numbers history
      - [ ] Add BINGO button
      - [ ] Link Gameplay.uss style sheet
      - [ ] Test: Verify layout in UI Builder
  - [x] Set up USS style sheets
    - [x] Create Common Styles
      - [x] Create Variables.uss
        - [x] Define color variables
        - [x] Define spacing variables
        - [x] Define font variables
        - [x] Define animation variables
      - [x] Create Typography.uss
        - [x] Define heading styles
        - [x] Define body text styles
        - [x] Define button text styles
        - [x] Define label styles
      - [x] Create Colors.uss
        - [x] Define primary colors
        - [x] Define background colors
        - [x] Define text colors
        - [x] Define state colors (hover, active, disabled)
      - [x] Create Animations.uss
        - [x] Define transition animations
        - [x] Define hover effects
        - [x] Define click effects
        - [x] Define win animations
    - [ ] Create Screen-Specific Styles
      - [x] Login.uss
        - [x] Style login container
        - [x] Style input field
        - [x] Style connect button
        - [x] Style status message
      - [ ] Lobby.uss
        - [ ] Style room list
        - [ ] Style room card
        - [ ] Style create room panel
        - [ ] Style room buttons
      - [ ] GameRoom.uss
        - [ ] Style player list
        - [ ] Style ready button
        - [ ] Style status indicators
        - [ ] Style start message
      - [ ] Gameplay.uss
        - [ ] Style bingo card grid
        - [ ] Style number cells
        - [ ] Style number display
        - [ ] Style BINGO button
    - [ ] Test: Verify all styles in UI Builder
  - [ ] Configure UI Builder
  - [ ] Create common UI components
    - [ ] Create Button Components
      - [ ] Create PrimaryButton.uxml
        - [ ] Add button container
        - [ ] Add label for text
        - [ ] Add hover state
        - [ ] Add active state
        - [ ] Add disabled state
      - [ ] Create SecondaryButton.uxml
        - [ ] Add button container
        - [ ] Add label for text
        - [ ] Add hover state
        - [ ] Add active state
        - [ ] Add disabled state
    - [ ] Create Input Components
      - [ ] Create TextInput.uxml
        - [ ] Add input container
        - [ ] Add text field
        - [ ] Add placeholder text
        - [ ] Add error state
      - [ ] Create NumberInput.uxml
        - [ ] Add input container
        - [ ] Add integer field
        - [ ] Add validation
        - [ ] Add error state
    - [ ] Create Card Components
      - [ ] Create RoomCard.uxml
        - [ ] Add card container
        - [ ] Add room info section
        - [ ] Add player count
        - [ ] Add join button
      - [ ] Create PlayerCard.uxml
        - [ ] Add card container
        - [ ] Add player name
        - [ ] Add ready status
        - [ ] Add host indicator
    - [ ] Create Grid Components
      - [ ] Create BingoGrid.uxml
        - [ ] Add grid container
        - [ ] Add cell template
        - [ ] Add number display
        - [ ] Add marking system
    - [ ] Create List Components
      - [ ] Create RoomList.uxml
        - [ ] Add list container
        - [ ] Add scroll view
        - [ ] Add empty state
      - [ ] Create PlayerList.uxml
        - [ ] Add list container
        - [ ] Add scroll view
        - [ ] Add empty state
    - [ ] Test: Verify all components in UI Builder
  - [ ] Test: Verify UI Toolkit is working in Play Mode
- [ ] Create basic folder structure

### Step 2: Login Scene UI
**User Story**: As a player, I want to enter my name and connect to the game server so I can access the lobby where I can see available rooms, create new ones, or join existing ones.

**Acceptance Criteria**:
- [x] Create Login.uxml layout
- [x] Implement Login.uss styles
- [x] Add name input field
- [x] Add connect button
- [x] Add status message label
- [x] Add scene transition to lobby
- [x] Test: Scene should show all UI elements and transition to lobby on successful connection

**Test Steps**:
1. Enter Play Mode
2. Verify all UI elements are visible
3. Verify input field accepts text
4. Verify button is clickable
5. Verify status label is visible
6. Enter player name and click connect
7. Verify scene transitions to lobby scene
8. Verify player name persists in lobby scene

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
- [x] Implement NetworkManager (Shared Mode)
- [x] Add Photon Fusion initialization (Shared Mode)
- [x] Connect login UI to network
- [x] Add error handling
- [x] Test: Should connect to Photon and show success/error messages

**Test Steps**:
1. Enter Play Mode
2. Enter player name
3. Click connect
4. Verify connection status
5. Verify error messages for invalid input

### Step 2: Room Management UI
**User Story**: As a player, I want to see and interact with available rooms in real-time.

**Acceptance Criteria**:
- [ ] Implement room list updates (Shared Mode)
- [ ] Add room creation functionality (Shared Mode)
- [ ] Add room joining functionality (Shared Mode)
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
- [ ] Implement player list updates (Shared Mode)
- [ ] Add ready status synchronization (Shared Mode)
- [ ] Add start game conditions (Shared Mode)
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
- [ ] Implement number display (Shared Mode)
- [ ] Add automatic number drawing (Shared Mode)
- [ ] Add drawn numbers history (Shared Mode)
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
