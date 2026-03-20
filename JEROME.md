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

## 3. Scripts to Build

### A. `ResourceNode.cs` — `Assets/Scripts/Resources/`

Interactable object in the world that gives the player a resource when picked up.

- [ ] Create `ResourceNode.cs` in `Scripts/Resources/`
- [ ] Add a `ResourceType` enum field (Water / Seeds / CleanEnergy)
- [ ] Add an `amount` int field (how much to give)
- [ ] On interact: call `ResourceManager.Instance.Collect(type, amount)`
- [ ] Disable the GameObject after pickup (or play a pickup animation)
- [ ] Use namespace `WhereFirefliesReturn.Resources`

### B. `PlayerInteraction.cs` — `Assets/Scripts/Player/`

Raycasts from the player camera to detect nearby `ResourceNode` objects.

- [ ] Create `PlayerInteraction.cs` in `Scripts/Player/`
- [ ] Raycast forward from the camera (max distance ~3f)
- [ ] On `E` key press: call `Interact()` on the hit object if it has a `ResourceNode`
- [ ] Show/hide a "Press E to collect" UI prompt when a node is in range
- [ ] Use namespace `WhereFirefliesReturn.Player`

### C. Decision UI Panel — `Assets/Scripts/UI/`

Shows 2–3 choices at the end of each cycle. Julie provides the decision text.

- [ ] Create `DecisionPanelUI.cs` in `Scripts/UI/`
- [ ] Subscribe to `CycleManager`'s cycle-end event
- [ ] Show a panel with 2–3 `Decision` buttons (text + resource cost + env impact)
- [ ] On button click: call `DecisionSystem.Instance.ApplyDecision(decision)`
- [ ] Hide the panel after a decision is made
- [ ] Use namespace `WhereFirefliesReturn.UI`

### D. End Screen Logic — `Assets/Scripts/Core/` or `Scripts/UI/`

Reads the final `EnvironmentMeter` value and shows the correct ending.

- [ ] Create `EndScreenManager.cs`
- [ ] On scene load: read `EnvironmentMeter.Instance.CurrentValue`
- [ ] If value ≥ 80 → show **restoration ending** (fireflies return)
- [ ] If value ≤ 20 → show **collapse ending**
- [ ] Otherwise → show **neutral/partial ending**
- [ ] Use namespace `WhereFirefliesReturn.Core`

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
