# CLAUDE.md — Where Fireflies Return

## Project Overview

Unity 3D narrative puzzle game developed for the **QS ImpACT Ideathon**.

- **SDG 15** — Life on Land (primary)
- **SDG 13** — Climate Action (secondary)
- **Engine:** Unity 6 (6000.3.11f1)
- **Platform:** PC (Windows / Mac)
- **Repo:** https://github.com/Bukas-Malaya/QS

## Team

| Role | Name |
|------|------|
| Project Manager | Ethan |
| Developer | Jerome |
| Designer | Matthew |
| Researcher / Writer | Julie |

## Core Concept

Players explore a degraded 3D farm environment, collecting resources (water, seeds, clean energy) and making farm management decisions each cycle. Every decision shifts the **Environment Meter** — pushing toward ecosystem collapse or full restoration. The fireflies return only when the land heals.

## Game Structure

- **Cycles:** 5 in-game cycles (configurable in `CycleManager`)
- **Environment Meter:** 0–100. Collapse threshold = 20. Restoration threshold = 80. Starts at 50.
- **Resources:** Water, Seeds, Clean Energy — collected by exploring, spent on decisions
- **Scenes:** MainMenu → GameWorld → EndScreen

## Script Architecture

All scripts use namespaces. Follow this pattern for new scripts:

```
namespace WhereFirefliesReturn.<FolderName>
{
    public class MyScript : MonoBehaviour { }
}
```

### Core Systems (already built)

| Script | Namespace | Purpose |
|--------|-----------|---------|
| `GameManager.cs` | Core | Game state machine (MainMenu / Playing / Paused / GameOver). Singleton, DontDestroyOnLoad. |
| `SceneController.cs` | Core | Scene loading wrapper. Singleton, DontDestroyOnLoad. |
| `EnvironmentMeter.cs` | Environment | Tracks ecosystem health 0–100. Fires events on collapse/restoration. |
| `CycleManager.cs` | Farm | Manages in-game cycle progression. Fires events on cycle start and completion. |
| `DecisionSystem.cs` | Farm | Applies a Decision's environment impact and resource cost, then advances the cycle. |
| `ResourceManager.cs` | Resources | Tracks Water/Seeds/CleanEnergy. Exposes Collect() and Spend(). |
| `PlayerController.cs` | Player | 3D CharacterController movement relative to camera. WASD + gravity. |
| `DialogueManager.cs` | Narrative | Typewriter-effect dialogue runner. Advances on Space or mouse click. |

### Folder Structure

```
Assets/
├── Scripts/
│   ├── Core/          GameManager, SceneController
│   ├── Player/        PlayerController
│   ├── Resources/     ResourceManager
│   ├── Farm/          CycleManager, DecisionSystem
│   ├── Environment/   EnvironmentMeter
│   ├── Narrative/     DialogueManager
│   └── UI/            (empty — build UIManager here)
├── Scenes/            MainMenu, GameWorld, EndScreen
├── Art/
│   ├── Models/        FBX files from Matthew (export as FBX)
│   ├── Sprites/
│   ├── Backgrounds/
│   └── UI/
├── Audio/
│   ├── Music/
│   └── SFX/
├── Prefabs/
│   ├── Player/
│   ├── Resources/
│   └── UI/
└── Data/
    ├── Dialogue/      ScriptableObjects for dialogue lines
    └── LevelConfig/   ScriptableObjects for level/cycle config
```

## Git Rules

- **Git LFS is required.** Run `git lfs install` before cloning.
- Binary assets (FBX, PNG, WAV, .unity, .prefab) are tracked via LFS — never commit without it.
- **One person owns each scene file.** Two people editing the same `.unity` = merge conflict.
- Branch per feature. PR to main.

## What Needs to Be Built Next

- [ ] `UIManager.cs` — HUD displaying Environment Meter, resource counts, cycle number
- [ ] `ResourceNode.cs` — interactable objects in the world that give resources on pickup
- [ ] `PlayerInteraction.cs` — raycasting to detect and interact with ResourceNodes
- [ ] Decision UI panel — shows 2–3 choices at end of each cycle (Julie provides the text)
- [ ] End screen logic — reads final EnvironmentMeter value, shows collapse or restoration ending
- [ ] Wire up all systems in the GameWorld scene via the Unity Editor

## Do Not

- Do not use `delay()` or block the main thread — use coroutines or `Invoke`
- Do not hardcode dialogue text in scripts — use `DialogueLine[]` arrays or ScriptableObjects
- Do not edit `.unity` scene files in a text editor — always use the Unity Editor
- Do not commit the `Library/` or `Temp/` folders — they are gitignored

## Local AI Infrastructure (Ethan's machine only — not in repo)

Stored in `QS/.claude/` — gitignored, never committed.

### Agents

| Agent | Model | Invoke With | Purpose |
|-------|-------|-------------|---------|
| `unity-dev` | Sonnet | `/agent:unity-dev` | Write Unity C# scripts, knows all existing systems |
| `game-designer` | Haiku | `/agent:game-designer` | Mechanic balance, decision design, cycle pacing |
| `sdg-advisor` | Haiku | `/agent:sdg-advisor` | SDG 15/13 accuracy, validates decisions and narrative |

### Skills

| Skill | Trigger | Purpose |
|-------|---------|---------|
| `qs-dev` | `/qs-dev` | Loads full project context — use at start of every session |
| `unity-feature` | `/unity-feature <desc>` | Full feature workflow: design → C# script → Jerome wiring checklist |

### Session Startup

Always begin a new Claude Code session in this project with:
```
/qs-dev
```
This loads the full game state so you never need to re-explain the project.

## Mistakes & Corrections
