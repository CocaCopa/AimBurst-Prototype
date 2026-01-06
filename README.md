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

## Architecture Overview  

The project uses a modular, systems-based architecture that isolates core logic from feature-level systems, improving scalability and stability.  

**High-level view of the modular architecture used to separate pure C# logic from Unity-specific code.**  

<img width="327" height="432" alt="Screenshot 2026-01-03 120005" src="https://github.com/user-attachments/assets/c9934fdd-5d2e-4b40-a922-4fa17ff45d8a" />  

### Design Intent

This was designed based on commonly observed constraints in hyper-casual development:

- Designers iterate frequently and should not depend on engineers for tuning
- Gameplay values change often during soft-launch testing
- Systems must be easy to extend or discard as mechanics evolve
- Risky refactors should be isolated and contained

### Implementation

- **Assembly Definitions (asmdefs)** are used to clearly separate:
  - Pure C# runtime logic
  - Unity-specific behaviour and presentation
- Core systems communicate through explicit boundaries rather than tight coupling
- **No hardcoded gameplay values**:
  - Behaviour is component-driven
  - All tuning is performed via prefabs and prefab variants
  - Designers can rebalance gameplay directly from the Inspector

This structure prioritizes **iteration speed, stability, and maintainability** over one-off scripting.

---

## Gameplay Systems

The prototype currently includes the following core mechanics:

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
