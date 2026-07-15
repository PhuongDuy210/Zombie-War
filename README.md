# 🧟 Zombie War

Zombie War is a Unity‑based survival shooter where players battle waves of relentless zombies. The project demonstrates core gameplay mechanics such as health management, spawning systems, animation control, and wave progression.

---

## 🎮 Gameplay Overview
- **Objective**: Survive as long as possible against incoming zombie waves.
- **Player Mechanics**:
  - Health system with visual feedback (flashing colors when hurt or invincible).
  - Damage cooldown and invincibility indicator.
  - Smooth spawning system for enemies with configurable wave entries.
- **Enemies**:
  - Configurable types and spawn amounts.
  - Global spawn cap to balance difficulty.
- **Weapons**:
  - **Assault Rifle** – versatile, balanced fire rate and damage.  
  - **Shotgun** – close‑range high power, wide spread.
  - **Grenade** – explosive area damage, has cooldown timer.
- **Audio/Visuals**:
  - SFX for actions and events.
  - Sprites, shaders, and animations for immersive gameplay.

---

## 🛠️ Tech Stack
- **Engine**: Unity (tested with Unity 6.3 LTS)
- **Language**: C# scripts
- **Platform**: Android

---

## 📂 Project Assets Structure
- `Animations/` → Player and enemy animations.
- `Materials/` → Custom Materials used in scenes and prefabs.
- `Packages/` → Unity package dependencies.
- `Prefabs/` → Player, enemy, and environment prefabs.
- `Resources/` → Configs and runtime‑loaded assets.
- `SFX/` → Sound effects for gameplay.
- `Scenes/` → Game scenes (main levels).
- `Scripts/` → Core C# scripts (Player, SpawnRoutine, Enemy logic).
- `Settings/` → Project settings.
- `Shaders/` → Custom shaders for effects.
- `Sprites/` → 2D art assets.

---

## 🔮 Future Improvements
While the current build demonstrates the core gameplay loop and mechanics, there are several areas we plan to enhance in future iterations:

- **Smoother and more precise controls** – refine player movement and aiming for a more responsive feel.  
- **Expanded enemy behaviors** – introduce varied attack patterns, smarter AI, and unique zombie types.  
- **Player‑friendly UI** – improve HUD clarity, add intuitive menus, and enhance accessibility.  
- **Sound design** – richer audio feedback, immersive ambient sounds, and weapon SFX improvements.  
- **Level design** – more diverse environments, progressive difficulty scaling, and interactive elements.

---

## 🚀 How to Run
1. Clone the repository:
   ```bash
   git clone https://github.com/PhuongDuy210/Zombie-War.git
