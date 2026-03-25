# Jerome's Wiring Checklist
> Pull latest main before starting. Check off each item as you go.

---

## 0. Build Settings (do this first)
File → Build Settings → Add Open Scenes for each:
- [ ] `Assets/Scenes/MainMenu`
- [ ] `Assets/Scenes/GameWorld`
- [ ] `Assets/Scenes/EndScreen`

Make sure **MainMenu is index 0** (drag it to the top).

---

## 1. Persistent Managers (MainMenu scene)

Create an empty GameObject named `[Persistent Managers]`.
Attach these scripts:
- [ ] `GameManager`
- [ ] `SceneController`

> These two have `DontDestroyOnLoad` — they persist across all scenes automatically.

---

## 2. GameWorld Scene — Manager GameObject

Create an empty GameObject named `[GameWorld Managers]`.
Attach these scripts:
- [ ] `EnvironmentMeter`
- [ ] `CycleManager`
- [ ] `DecisionSystem`
- [ ] `ResourceManager`

> `EnvironmentMeter` also has `DontDestroyOnLoad` — it carries the final health value to EndScreen.

---

## 3. Player Setup (GameWorld)

On the Player GameObject:
- [ ] `PlayerController` attached — assign `cameraTransform` field to Main Camera
- [ ] `PlayerInteraction` attached — set `InteractLayer` to whatever layer your ResourceNodes are on
- [ ] Tag the Player GameObject as **"Player"**

On the Main Camera:
- [ ] Attach `CameraFollow`
- [ ] Set `Target` field to the Player Transform
- [ ] Tune `Offset` (suggested: `0, 6, -8`) and `Follow Speed` (`5`)

---

## 4. Canvas — HUD (GameWorld)

Create a Canvas. Add a child GameObject named `HUD`. Attach `UIManager`. Wire:
- [ ] `Env Meter Slider` → a UI Slider for the environment bar
- [ ] `Water Text` → TMP text
- [ ] `Seeds Text` → TMP text
- [ ] `Clean Energy Text` → TMP text
- [ ] `Cycle Text` → TMP text

---

## 5. Canvas — Decision Panel (GameWorld)

On the same Canvas, create a child `DecisionPanel`. Attach `DecisionPanelUI`. Wire:
- [ ] `Panel` → the DecisionPanel GameObject itself
- [ ] `Cycle Prompt Text` → TMP text (e.g. "Cycle 1 — Choose your action:")
- [ ] `Decision Buttons` → array of 3 UI Buttons
- [ ] `Button Labels` → array of 3 TMP texts (the button label text)
- [ ] `Button Cost Labels` → array of 3 TMP texts (the cost display text)
- [ ] `Cycle Decisions` → fill 5 entries (one per cycle), each with 2–3 decisions:
  - `label` — short name (e.g. "Plant Native Seeds")
  - `description` — one sentence Julie wrote
  - `environmentImpact` — positive = good (e.g. `+15`), negative = bad (e.g. `-10`)
  - `cost` — water / seeds / cleanEnergy integers

---

## 6. Canvas — Dialogue UI (GameWorld)

Create a child `DialoguePanel`. Attach `DialogueUI`. Wire:
- [ ] `Dialogue Panel` → the DialoguePanel GameObject
- [ ] `Speaker Name Text` → TMP text
- [ ] `Dialogue Text` → TMP text

---

## 7. End Day Trigger (GameWorld)

Near the barn entrance:
- [ ] Create empty GameObject named `EndDayTrigger`
- [ ] Add a **Box Collider** → check **Is Trigger**
- [ ] Attach the `EndDayTrigger` script
- [ ] Size the collider so the player walks through it naturally

---

## 8. Event Wiring (GameWorld — on the [GameWorld Managers] GameObject)

### CycleManager → OnAllCyclesComplete
- [ ] Click `+` → drag `[Persistent Managers]` → select `GameManager.EndGame()`

### EnvironmentMeter → OnEcosystemCollapse
- [ ] Click `+` → drag `[Persistent Managers]` → select `GameManager.EndGame()`

### EnvironmentMeter → OnEcosystemRestored
- [ ] Click `+` → drag `[Persistent Managers]` → select `GameManager.EndGame()`

> `EndGame()` reads the current EnvironmentMeter value automatically — no bool needed.

---

## 9. MainMenu Scene

- [ ] Create `[Persistent Managers]` GO → attach `GameManager` + `SceneController`
- [ ] Create a Canvas with a Start button and a Quit button
- [ ] Create an empty GO → attach `MainMenuUI`
- [ ] Wire `startButton` → your Start button
- [ ] Wire `quitButton` → your Quit button

---

## 10. EndScreen Scene

- [ ] Create a Canvas with 3 panels: `RestorationPanel`, `CollapsePanel`, `NeutralPanel`
- [ ] Create an empty GO → attach `EndScreenManager`
- [ ] Wire:
  - `restorationPanel` → RestorationPanel GO
  - `collapsePanel` → CollapsePanel GO
  - `neutralPanel` → NeutralPanel GO
  - `scoreText` → a TMP text
  - `playAgainButton` → Play Again button
  - `mainMenuButton` → Main Menu button

---

## 11. Final Check

- [ ] Play from MainMenu scene
- [ ] Start button loads GameWorld
- [ ] Player can move and interact with ResourceNodes (press E)
- [ ] Walking into barn trigger opens Decision Panel
- [ ] Picking a decision advances cycle + adjusts EnvironmentMeter
- [ ] After 5 cycles → EndScreen loads
- [ ] EndScreen shows correct panel based on EnvironmentMeter value
