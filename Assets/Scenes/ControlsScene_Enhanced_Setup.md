# Enhanced Controls Scene Setup Guide

## Overview
The enhanced controls tutorial now features:
- Split-screen layout: Player demo on left, instructions on right
- Uses actual Player prefab from the game
- Simulates real player inputs for authentic demonstrations
- Maintains the main menu's visual style

## Quick Setup

### 1. Create the Scene
1. Create empty GameObject named "SceneBuilderEnhanced"
2. Add `ControlsSceneBuilderEnhanced` component
3. Check "Build Scene" checkbox
4. Delete the SceneBuilderEnhanced GameObject

### 2. Assign Player Prefab
1. Select "ControlsTutorialManager" GameObject
2. In Inspector, find "Player Prefab" field
3. Drag the Player prefab from: `Assets/Characters/Player/Prefabs/Player.prefab`

### 3. Test the Scene
1. Enter Play mode
2. Verify split-screen layout appears for demo screens
3. Check that player animations and shield effects work

## Layout Structure

### Non-Demo Screens (1, 2, 5, 9, 10, 11)
```
+------------------------------------------+
|              TITLE TEXT                  |
|                                          |
|                                          |
|           CONTENT TEXT                   |
|          (Centered)                      |
|                                          |
|                                          |
|              [NEXT]                      |
+------------------------------------------+
```

### Demo Screens (3, 4, 6, 7, 8)
```
+------------------------------------------+
|              TITLE TEXT                  |
+-------------------+----------------------+
|                   |                      |
|   PLAYER DEMO     |   INSTRUCTIONS       |
|   (Left Panel)    |   (Right Panel)      |
|                   |                      |
|                   |  "Press UP to JUMP"  |
|                   |                      |
+-------------------+----------------------+
|              [NEXT]                      |
+------------------------------------------+
```

## Demo Camera Setup
- Demo Camera renders to left panel viewport
- Position: (-5, 0, -10)
- Orthographic Size: 3
- Viewport Rect: (0.05, 0.2, 0.43, 0.5)

## Player Demo Features

### Movement Demos
- **Jump**: Player performs 3 jumps with proper timing
- **Move**: Player moves left, right, then returns to center

### Shield Demos
- **Deflect**: Quick shield activation with attack animation
- **Absorb**: Extended shield hold for 2 seconds
- **Bypass**: Double-tap shield for phase effect

## Customization

### Adjust Demo Timing
In `PlayerInputSimulator.cs`:
```csharp
public IEnumerator DemoJumpSequence(int jumpCount = 3)
public float demoMoveDistance = 3f;
public float moveSpeed = 5f;
```

### Change Panel Colors
In `ControlsSceneBuilderEnhanced.cs`:
```csharp
leftBg.color = new Color(0.1f, 0.1f, 0.15f, 0.3f);
rightBg.color = new Color(0.1f, 0.1f, 0.15f, 0.3f);
```

### Modify Instructions
Edit tutorial screens in `ControlsTutorialManager.cs`:
```csharp
new TutorialScreen
{
    title = "YOUR TITLE",
    content = "Your instructions here",
    hasDemo = true,
    demoType = DemoType.Jump
}
```

## Troubleshooting

### Player Not Appearing
- Ensure Player prefab is assigned to ControlsTutorialManager
- Check demo spawn point position (-5, 0, 0)
- Verify Player prefab has required components

### Shield Not Working
- Check if Shield child object exists in Player prefab
- Verify shieldAnimator is linked in PlayerController
- Ensure shield animations are set up

### Camera Issues
- Demo camera should be separate from main camera
- Check viewport rect matches panel position
- Ensure camera is activated for demo screens

## Input System Notes
- Player's actual InputSystem is disabled during demos
- PlayerInputSimulator creates synthetic input events
- Original input is restored when demo ends

## Next Steps
1. Create enemy prefabs for shield demos (optional)
2. Add sound effects for button clicks
3. Customize transition animations
4. Add particle effects for shield demonstrations