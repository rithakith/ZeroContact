# Controls Scene Setup Instructions

## Quick Setup Method (Recommended)
1. In Unity, go to **Tools > Create Controls Scene**
2. The scene will be automatically created with all necessary components
3. Remove the "SceneBuilder" GameObject after creation
4. Save the scene

## Manual Setup Method

### 1. Create New Scene
- File > New Scene
- Choose "Empty Scene"
- Save as: `Assets/Scenes/ControlsScene.unity`

### 2. Add Core Components
1. Create empty GameObject named "ControlsTutorialManager"
2. Add the `ControlsTutorialManager` script component
3. Create empty GameObject named "SceneBuilder"
4. Add the `ControlsSceneBuilder` script component
5. In the Inspector, check "Build Scene" checkbox
6. Delete the SceneBuilder GameObject after the scene is built

### 3. Configure Tutorial Manager
The ControlsTutorialManager should automatically have all 11 tutorial screens configured with:
- Screen 1: "YOUR CHILDHOOD FRIEND HAS BEEN TAKEN"
- Screen 2: "ZERO CONTACT SHIELD SYSTEM"
- Screen 3: "BASIC MOVEMENT - JUMPING" (with demo)
- Screen 4: "BASIC MOVEMENT - LEFT/RIGHT" (with demo)
- Screen 5: "ACTIVATING YOUR SHIELD"
- Screen 6: "SHIELD MODE: DEFLECT" (with demo)
- Screen 7: "SHIELD MODE: ABSORB" (with demo)
- Screen 8: "SHIELD MODE: BYPASS" (with demo)
- Screen 9: "ENEMY ATTACK PATTERNS"
- Screen 10: "ELEMENTAL DUNGEONS AHEAD"
- Screen 11: "TIME TO MOVE"

### 4. Create Player Demo Prefab
1. Create empty GameObject named "PlayerDemo"
2. Add Components:
   - SpriteRenderer (assign player sprite)
   - PlayerDemonstration script
   - Rigidbody2D (Gravity Scale: 2)
   - BoxCollider2D
   - Animator (optional, link to player animations)

3. Create child GameObject named "Shield"
   - Add SpriteRenderer
   - Assign a circular sprite
   - Set as inactive by default

4. Save as Prefab: `Assets/Prefabs/PlayerDemo.prefab`

### 5. Create Enemy Demo Prefab (Optional)
1. Create empty GameObject named "EnemyDemo"
2. Add SpriteRenderer (assign enemy sprite)
3. Add basic enemy behavior script
4. Save as Prefab: `Assets/Prefabs/EnemyDemo.prefab`

### 6. Link References
In ControlsTutorialManager:
- Player Demo Prefab: Drag PlayerDemo prefab
- Enemy Demo Prefab: Drag EnemyDemo prefab (if created)
- All UI references should be auto-linked by the builder

### 7. Enhance Next Button
1. Select the NextButton GameObject
2. Add `ControlsButtonEnhancer` component
3. Configure hover effects as desired

### 8. Test Scene
1. Play the scene
2. Click through all 11 screens
3. Verify demos play correctly
4. Ensure it returns to MainMenu after completion

## Styling Notes
The scene uses the same styling as the MainMenu:
- Dark gray buttons with 80% opacity
- White text using BoldPixels SDF font
- Button hover effects (scale 1.1x, move up 5px)
- Dark blue-gray background
- Smooth transitions between screens

## Controls
- **Next Button**: Progress to next screen
- **Enter/Space**: Alternative to clicking Next
- **Escape**: Return to Main Menu