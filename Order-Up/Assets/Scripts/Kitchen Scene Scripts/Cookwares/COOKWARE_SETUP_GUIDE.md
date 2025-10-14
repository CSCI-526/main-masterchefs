# Cookware System Setup Guide

## Overview
This guide will help you set up the cookware system for your Air Fryer, Oven, Pot, and Pan prefabs.

## Components Required

### 1. Cookware GameObject Setup
Each cookware prefab should have:
- **SpriteRenderer** - Displays the cookware sprite
- **PolygonCollider2D** - Detects when ingredients enter (automatically set as trigger)
- **Cookwares.cs** script

### 2. UI Setup for Each Cookware

Create a Canvas for each cookware with the following structure:

```
CookwareCanvas (Canvas - World Space)
└── SliderPanel (Panel)
    ├── CookingTimeSlider (Slider)
    │   ├── Background
    │   ├── Fill Area
    │   │   └── Fill
    │   └── Handle Slide Area
    │       └── Handle
    ├── TimerText (Text)
    ├── StartButton (Button)
    └── StopButton (Button)
```

## Step-by-Step Setup Instructions

### Step 1: Prepare Your Cookware Prefab

1. Select your cookware prefab (e.g., AirFryer)
2. Ensure it has a **SpriteRenderer** with your cookware sprite
3. Add a **PolygonCollider2D** if not already present
4. Add the **Cookwares.cs** script to the prefab

### Step 2: Create the UI Canvas

1. Right-click on your cookware in the hierarchy
2. Select **UI > Canvas**
3. Rename it to "CookwareCanvas"
4. In the Canvas component:
   - Set **Render Mode** to **World Space**
   - Set **Width** and **Height** (e.g., 200 x 100)
   - Position it above your cookware sprite
   - Adjust **Scale** if needed (e.g., 0.01, 0.01, 0.01)

### Step 3: Create the Slider Panel

1. Right-click on CookwareCanvas
2. Select **UI > Panel**
3. Rename it to "SliderPanel"
4. Adjust its size and position

### Step 4: Add the Slider

1. Right-click on SliderPanel
2. Select **UI > Slider**
3. Rename it to "CookingTimeSlider"
4. Configure the slider:
   - Min Value: 1
   - Max Value: 20
   - Whole Numbers: Unchecked
   - Value: 1

### Step 5: Add Timer Text (Optional)

1. Right-click on SliderPanel
2. Select **UI > Text - TextMeshPro** (or Legacy Text)
3. Rename it to "TimerText"
4. Set default text: "Cook Time: 1.0s"

### Step 6: Add Start/Stop Buttons

1. Right-click on SliderPanel
2. Select **UI > Button**
3. Rename to "StartButton"
4. Change button text to "Start Cooking"
5. Repeat for "StopButton" with text "Stop"

### Step 7: Connect Script References

1. Select your cookware GameObject
2. In the **Cookwares** script inspector:
   - **Cookware Name**: Enter the name (e.g., "Air Fryer")
   - **Slider Panel**: Drag the SliderPanel GameObject here
   - **Cooking Time Slider**: Drag the CookingTimeSlider here
   - **Timer Display Text**: Drag the TimerText here (optional)
   - **Min Cooking Time**: 1
   - **Max Cooking Time**: 20

### Step 8: Add CookwareUI Script (Optional)

1. Add the **CookwareUI.cs** script to the SliderPanel or Canvas
2. In the inspector:
   - **Cookware**: Drag your cookware GameObject here
   - **Start Cooking Button**: Drag the StartButton here
   - **Stop Cooking Button**: Drag the StopButton here

### Step 9: Tag Your Ingredients

For the cookware to detect ingredients:
1. Select all your ingredient prefabs
2. In the Inspector, set **Tag** to "Ingredient" (create this tag if it doesn't exist)
3. Alternatively, ensure ingredients have a **DraggableIngredient** component

## How It Works

### Player Interaction Flow:

1. **Click on Cookware** → Slider panel appears
2. **Drag Ingredient into Cookware** → Slider becomes interactive
3. **Adjust Slider** → Set cooking time (1-20 seconds)
4. **Click "Start Cooking"** → Timer begins counting
5. **Wait for Timer** → Cooking completes automatically
6. **OR Click "Stop"** → Cancel cooking early

### Visual States:

- **No Ingredients**: Slider visible but grayed out (not interactable)
- **Has Ingredients**: Slider becomes interactive with full colors
- **Cooking**: Slider locked, timer counts up, stop button appears
- **Finished**: Ingredients marked as cooked, slider becomes interactive again

## Important Notes

### Collision Detection
- The PolygonCollider2D is automatically set as a **Trigger**
- Ingredients must have a **Collider2D** component
- Ingredients must be tagged as "Ingredient" OR have a DraggableIngredient component

### Multiple Cookwares
- Each cookware has its own slider panel
- Only the clicked cookware's panel will show
- Click on a different cookware to hide the current one and show the new one

### Customization
You can adjust:
- Min/Max cooking times in the inspector
- Slider appearance (colors, size)
- Timer display format in the code
- Visual feedback when cooking completes

## Testing Checklist

- [ ] SpriteRenderer displays cookware correctly
- [ ] PolygonCollider2D covers the cookware area
- [ ] Clicking cookware shows/hides slider panel
- [ ] Slider is disabled when no ingredients present
- [ ] Dragging ingredient into cookware enables slider
- [ ] Slider adjusts cooking time (1-20 seconds)
- [ ] Start button begins cooking timer
- [ ] Timer counts correctly and displays progress
- [ ] Cooking finishes at selected time
- [ ] Stop button cancels cooking
- [ ] Removing ingredients disables slider

## Troubleshooting

**Slider doesn't enable when ingredient enters:**
- Check if ingredient has "Ingredient" tag
- Verify ingredient has a Collider2D component
- Ensure cookware's PolygonCollider2D is set as Trigger

**Slider panel doesn't appear on click:**
- Verify SliderPanel is assigned in Cookwares script
- Check if SliderPanel is a child of a World Space Canvas
- Ensure cookware has a Collider2D for mouse detection

**Ingredients don't enter the cookware:**
- Both objects need Collider2D components
- At least one must be a trigger
- Check if ingredients are on the correct layer

## Next Steps

Consider adding:
- Visual feedback (color change) when cooking
- Particle effects during cooking
- Sound effects for start/stop cooking
- Overcooking detection (burning)
- Different cooking times for different cookware types
- Ingredient-specific cooking requirements
