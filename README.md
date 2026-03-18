# Where Fireflies Return

> A 3D narrative puzzle game developed for the **QS ImpACT Ideathon**

## SDG Focus
- **Primary:** SDG 15 — Life on Land
- **Secondary:** SDG 13 — Climate Action

## Concept
Players explore a degraded farm ecosystem, collecting resources (water, seeds, clean energy) and making management decisions each cycle. Every choice shifts an **Environment Meter** — pushing toward ecosystem collapse or full restoration. The fireflies return only when the land heals.

## Team — Bukas Malaya
| Role | Name |
|------|------|
| Project Manager | Ethan |
| Developer | Jerome |
| Designer | Matthew |
| Researcher / Writer | Julie |

## Tech Stack
- **Engine:** Unity 2022.3 LTS
- **Render Pipeline:** Universal Render Pipeline (URP)
- **Platform:** PC (Windows / Mac)
- **Version Control:** Git + Git LFS

## Project Structure
```
Assets/
├── Scripts/       # All C# game logic
├── Scenes/        # Unity scenes (MainMenu, GameWorld, EndScreen)
├── Art/           # Sprites, 3D models, backgrounds, UI art
├── Audio/         # Music and SFX
├── Prefabs/       # Reusable Unity prefabs
└── Data/          # ScriptableObjects (resources, dialogue, level config)
```

## Getting Started
1. Install **Unity Hub** and **Unity 2022.3 LTS**
2. Clone this repo: `git clone https://github.com/Bukas-Malaya/QS.git`
3. Open the project in Unity Hub via **Open > Add project from disk**
4. Open `Assets/Scenes/MainMenu` to start

> Git LFS is required. Run `git lfs install` before cloning.
