# Jerome's Onboarding Checklist — Where Fireflies Return

Welcome! This doc gets you up and running fast. All the core systems are already written — your job is to build the remaining scripts and wire everything up in Unity.

---

## 1. Setup (do this first)

- [ ] Install **Unity 6** (6000.3.11f1) from Unity Hub
- [ ] Install **Git LFS** — run `git lfs install` before cloning
- [ ] Clone the repo: `git clone https://github.com/Bukas-Malaya/QS.git`
- [ ] Open the project in Unity Hub → Add → select the `QS/` folder
- [ ] Let Unity import all assets (first open takes a few minutes)
- [ ] Confirm the three scenes exist: `Assets/Scenes/` → MainMenu, GameWorld, EndScreen

---

## 2. Read These Scripts First (already built — understand before touching)

These are the systems you'll be connecting to. Read them before writing anything new.

| Script | Location | What it does |
|--------|----------|--------------|
| `GameManager.cs` | `Scripts/Core/` | Game state machine — MainMenu / Playing / Paused / GameOver |
| `EnvironmentMeter.cs` | `Scripts/Environment/` | Tracks ecosystem health 0–100. Fires events on collapse/restoration |
| `ResourceManager.cs` | `Scripts/Resources/` | Tracks Water, Seeds, CleanEnergy. Use `Collect()` and `Spend()` |
| `CycleManager.cs` | `Scripts/Farm/` | Manages the 5 game cycles. Fires events on cycle start/end |
| `DecisionSystem.cs` | `Scripts/Farm/` | Applies a decision's cost + environment impact, advances cycle |
| `PlayerController.cs` | `Scripts/Player/` | WASD movement. Already handles gravity |
| `UIManager.cs` | `Scripts/UI/` | HUD — shows env meter bar, resource counts, cycle badge |

---

## 3. Scripts to Wire Up (all already written — find them in the repo)

All 5 remaining scripts are done. Jerome's job is to **attach them in Unity Editor and link the Inspector fields**.

### A. `ResourceNode.cs` — `Assets/Scripts/Resources/`

Inspector fields to set per node placed in the scene:
- `Water`, `Seeds`, `Clean Energy` — set how much each node gives (leave unused ones at 0)
- `Prompt Text` — defaults to "Press E to collect"
- Add a **Collider** to the GameObject so raycasts can hit it
- Assign it to the **Interact Layer** (create a layer called `Interactable` if it doesn't exist)

### B. `PlayerInteraction.cs` — `Assets/Scripts/Player/`

Attach to the **Player** GameObject. Inspector fields:
- `Interact Range` — default 3f
- `Interact Layer` — set to your `Interactable` layer mask
- `Prompt Panel` — drag in the UI GameObject that shows the "Press E" prompt
- `Prompt Label` — drag in the TextMeshPro text inside that panel

### C. `DecisionPanelUI.cs` — `Assets/Scripts/UI/`

Attach to a Canvas GameObject. Inspector fields:
- `Cycle Decisions` — array of 5 entries (one per cycle). Each entry holds 2–3 `Decision` objects. **Julie fills these in.**
- `Panel` — the root panel GameObject (hidden by default)
- `Cycle Prompt Text` — TMP text showing the cycle prompt
- `Decision Buttons` — drag in 2–3 Button GameObjects
- `Button Labels` — matching TMP texts for each button
- `Button Cost Labels` — matching TMP texts showing resource cost
- Call `DecisionPanelUI.Instance.Show()` from a UI button at end of exploration phase

### D. `EndScreenManager.cs` — `Assets/Scripts/Core/`

Attach to a GameObject in the **EndScreen** scene. Inspector fields:
- `Restoration Panel` — shown when env ≥ 80
- `Collapse Panel` — shown when env ≤ 20
- `Neutral Panel` — shown otherwise
- `Score Text` — TMP text showing final health value
- `Play Again Button` — restarts the game
- `Main Menu Button` — returns to main menu

---

## 4. Scene Wiring in Unity Editor (GameWorld scene)

Once scripts are written, wire them up in the scene.

- [ ] Open `GameWorld` scene
- [ ] Create an empty GameObject `--- MANAGERS ---`
  - [ ] Attach `GameManager`
  - [ ] Attach `SceneController`
  - [ ] Attach `EnvironmentMeter`
  - [ ] Attach `ResourceManager`
  - [ ] Attach `CycleManager`
  - [ ] Attach `DecisionSystem`
  - [ ] Attach `UIManager`
- [ ] Add a Canvas with the HUD elements (env meter bar, resource text, cycle badge)
- [ ] Link HUD UI elements to `UIManager` fields in the Inspector
- [ ] Place 3–5 `ResourceNode` prefabs in the scene with different resource types
- [ ] Attach `PlayerInteraction` to the Player GameObject
- [ ] Create the Decision Panel UI (hidden by default) and link to `DecisionPanelUI`

---

## 5. Code Rules (important)

- Use **namespaces**: `WhereFirefliesReturn.<FolderName>` on every script
- Never use `delay()` or block the main thread — use **coroutines** or `Invoke()`
- Never hardcode dialogue text — use `DialogueLine[]` arrays or ScriptableObjects
- Never edit `.unity` scene files in a text editor — Unity Editor only
- One person owns each scene file — coordinate with Ethan before editing a scene he's working in

---

## 6. Git Workflow

- Branch per feature: `git checkout -b feature/resource-node`
- PR to `main` when done
- **Git LFS must be installed** or binary files (FBX, PNG, .unity) will break

---

## Questions?

Ping Ethan. He has full context on all the existing systems.
