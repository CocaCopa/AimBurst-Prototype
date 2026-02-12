# AimBurst – Prototype

<img width="1579" height="881" alt="Screenshot 2026-01-03 121121" src="https://github.com/user-attachments/assets/77239a93-fc26-4c70-886d-513d953317cd" />
<img width="1578" height="885" alt="Screenshot 2026-01-03 120729" src="https://github.com/user-attachments/assets/1554b05c-3fe4-485e-a17b-7d82b7587869" />  

## Quick Overview  

**AimBurst** is a hyper-casual puzzle shooter prototype built in **Unity 6.2**, inspired by *This Is Blast*, a game originally created by **[Voodoo](https://voodoo.io/)**.

It was developed as a **reverse-engineering and learning exercise**, focused on recreating **gameplay feel, early-game mechanics, and level pacing** typical of hyper-casual mobile games.

The project emphasizes a **structured, scalable architecture** with inspector-driven configuration, allowing fast iteration and experimentation without hardcoded gameplay values.  

🔗 **Play (WebGL):**  
https://cocacopa.itch.io/aimburst-prototype

🎥 **Gameplay Showcase:**  
https://youtu.be/8T-2RKw7Vqc

---

## Gameplay Systems

The prototype includes the following core mechanics:

### Core Mechanics

- **Shooter Merging**  
  Three shooters of the same color merge into one, inheriting their combined remaining ammo.

- **Combat Friend Dependency**  
  Paired shooters can only move or exit slots together, and only after both have fully depleted their ammo.

- **Multi-Layer Target System**  
  Targets are stacked in vertical layers, with shooters prioritizing the highest available layer per column.

### Game State Conditions

Each condition triggers a **dedicated screen with distinct sprites and sound effects** based on the outcome.

- **Win**: Triggered when all targets in the level are destroyed.
- **Lose**: Triggered when all shooter slots are filled and no shooter has a valid target of its color.

---

## Level Creation Workflow (Designer-Focused)

Levels are authored using a **custom Spawner system** designed to support rapid content iteration.

- Level layouts are defined via **arrays configured in the Inspector**
- A **custom editor** improves:
  - Readability of level data
  - Speed of iteration
  - Reduction of configuration errors

This allows new levels to be created or adjusted **without modifying code**, aligning with designer-driven workflows.

---

## Architecture Overview  

In this project gameplay decisions are separated from Unity scene execution to keep rules isolated and iteration fast.  

**High-level view of the modular architecture used to separate pure C# logic from Unity-specific code.**  

<img width="327" height="432" alt="Screenshot 2026-01-03 120005" src="https://github.com/user-attachments/assets/c9934fdd-5d2e-4b40-a922-4fa17ff45d8a" />  

### Core Principle  

- Pure C# systems decide what should happen
- Unity MonoBehaviours handle how it appears in the scene

This prevents gameplay rules from being tightly coupled to scene objects and allows refactors without touching presentation code.

### Structural Separation

The project uses Assembly Definitions to enforce boundaries between:
- Runtime (Pure C#)
  - Game state transitions
  - Win/Lose evaluation
  - Target prioritization rules
  - Shooter merging logic

- Unity Layer
  - MonoBehaviours
  - Spawning
  - Visual feedback
  - Audio playback
  - Scene interaction

Unity components act as executors of decisions computed in the runtime layer.  

This is due to gameplay values and mechanics changing frequently during development until final expections are met.
- Rule changes require editing only runtime systems.
- Scene behaviour remains stable.
- Designers can rebalance via Inspector without modifying code.
- Risky refactors stay localized instead of spreading across unrelated scripts.

---

## Getting Started

1. Open the project using **Unity 6.2 (6000.2.12.f1)**
2. Navigate to **Assets/Scenes** folder
3. Load **Level_1** scene
4. Press **Play**

---

## Code Entry Points

If you are reviewing the code, the main systems can be found under **Assets/_Scripts/AimBurst**:

- `GameFlow`: core game loop and state transitions
- `LevelLayout`: level data and spawner logic
- `ShootersLayout`: shooter behaviour and targeting
- `PrefabRegistry`: centralized prefab access

---

## Disclaimer

This project is intended **for educational and portfolio purposes only**.  
All inspiration is credited, and no original assets, code, or proprietary content from the referenced game were used.

---

## Resources

Core assets (game cubes and environment) were **created by me using Blender**.

Additional visual effects, UI elements, and sound effects were sourced from **free Unity Asset Store packages** and **free sound libraries**, then **tweaked and adjusted** to achieve the final gameplay result.

All third-party assets listed below are **free to use** and **remain the property of their respective creators**.

### Unity Asset Store (Free)
- [Quick Outline](https://assetstore.unity.com/packages/tools/particles-effects/quick-outline-115488)
- [Fancy Footsteps](https://assetstore.unity.com/packages/vfx/particles/fancy-footsteps-201948)
- [2D Casual UI HD](https://assetstore.unity.com/packages/2d/gui/icons/2d-casual-ui-hd-82080)
- [Hints, Stars & Rewards SFX Lite Pack](https://assetstore.unity.com/packages/audio/sound-fx/hints-stars-points-rewards-sound-effects-lite-pack-295538)
- [Hyper Casual FX](https://assetstore.unity.com/packages/vfx/particles/hyper-casual-fx-200333)

### Pixabay (Free Sound Effects)
- [Selection Sounds](https://pixabay.com/sound-effects/selection-sounds-73225/)
- [Menu Click Sounds](https://pixabay.com/sound-effects/video-game-menu-click-sounds-148373/)
- [Mouse Double Click](https://pixabay.com/sound-effects/mouse-double-click-371217/)
