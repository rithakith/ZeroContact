# Fixing Missing Script References

## Quick Fix Options:

### Option 1: Remove Missing Scripts (Fastest)
1. In Unity, go to **Tools > Remove All Missing Scripts**
2. This will clean up all missing script references
3. Your scene should work normally after this

### Option 2: Rebuild Scene (Cleanest)
1. Go to **Tools > Rebuild Controls Scene Clean**
2. Click "Yes, Rebuild"
3. The scene will be recreated fresh
4. **Important**: Assign your Player prefab to the ControlsTutorialManager after rebuild

### Option 3: Manual Fix
1. Open the ControlsScene
2. Look for GameObjects with missing scripts (they'll have a warning icon)
3. Click on the GameObject
4. In Inspector, click the gear icon next to the missing script
5. Select "Remove Component"

## After Fixing:

### Assign Player Prefab
1. Select "ControlsTutorialManager" in the Hierarchy
2. In Inspector, find "Player Prefab" field
3. Drag your Player prefab from: `Assets/Characters/Player/Prefabs/Player.prefab`

### Test the Scene
1. Enter Play mode
2. Click through the tutorial screens
3. Verify demos work on screens 3, 4, 6, 7, 8

## What Happened?
- We renamed some scripts that were causing compilation errors
- Unity still had references to the old scripts
- The new scripts (ControlsTutorialManagerSimple) need to be assigned

## The Working Scripts:
- **ControlsTutorialManagerSimple** - Main tutorial controller
- **ControlsSceneBuilderEnhanced** - Builds the scene with split layout
- **PlayerDemonstration** - Handles player demos
- **ControlsButtonEnhancer** - Button hover effects