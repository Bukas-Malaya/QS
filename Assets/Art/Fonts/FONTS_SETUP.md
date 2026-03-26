# Fonts Setup — Colonial Quill Theme

## Recommended Pairing

Use **two fonts together**:
- **Pinyon Script** → titles, speaker names, dialogue headers, cycle labels
- **Cormorant Garamond Italic** → body text, dialogue lines, descriptions, cost labels

---

## Font 1: Pinyon Script (Headers / Titles)

**Download:** https://fonts.google.com/specimen/Pinyon+Script
**License:** SIL Open Font License — free, commercial use OK. No attribution needed.
**Vibe:** 18th century English roundhand. High quill pressure on downstrokes, hairline upstrokes. Closest to Hamilton title card lettering and the Declaration of Independence handwriting.

### Unity Install:
1. Download `PinyonScript-Regular.ttf`
2. Drop into `Assets/Art/Fonts/`
3. In Unity: **Window → TextMeshPro → Font Asset Creator**
4. Set Atlas Resolution to **4096×4096** (hairline strokes WILL break at 512/1024)
5. Name it `PinyonScript SDF`
6. Use for: speaker names, cycle titles, UI headers, end screen text

**Readability warning:** Use at 18px minimum. Headers only — not body copy.

---

## Font 2: Cormorant Garamond (Body / Dialogue)

**Download:** https://fonts.google.com/specimen/Cormorant+Garamond
**License:** SIL Open Font License — free, commercial use OK. No attribution needed.
**Vibe:** A revival of 16th–17th century Garamond — the dominant type of the colonial era. The italic reads like quill-printed parchment. Use the **Italic** weight.

### Unity Install:
1. Download `CormorantGaramond-Italic.ttf`
2. Drop into `Assets/Art/Fonts/`
3. Font Asset Creator → Atlas Resolution **2048×2048** is fine for this one
4. Name it `CormorantGaramond-Italic SDF`
5. Use for: dialogue text, decision descriptions, resource labels, all body copy

---

## Backup Options (if pairing above doesn't feel right)

| Font | Link | Best For |
|------|------|----------|
| IM Fell English | fonts.google.com/specimen/IM+Fell+English | Most historically accurate — actual 1686 Oxford printing type |
| Lovers Quarrel | fonts.google.com/specimen/Lovers+Quarrel | More ornate, decorative uppercase — good for title screen only |
| Almendra | fonts.google.com/specimen/Almendra | Bridges handcrafted + readable — good all-rounder |

---

## 3D Asset Pack: Voxel Farm

**URL:** https://assetstore.unity.com/packages/3d/environments/industrial/voxel-farm-179312
**Cost:** ~€5.51 (worth it for the exact Minecraft-farm aesthetic)
**License:** Standard Unity Asset Store EULA — commercial use OK.
**Includes:** Barn, tractor, windmill, scarecrow, fences, crops, plants (29 screenshots on the page)

> Note: Built-in RP only. If URP is needed, check the free alternative below.

**Free Alternative:** GameDev Starter Kit - Farming (Free Edition)
**URL:** https://assetstore.unity.com/packages/3d/environments/gamedev-starter-kit-farming-free-edition-243035
**Includes:** Crops (with growth stages), farm props, modular environment pieces. Supports URP + Unity 6.

---

## Visual Theme Brief

The game is **3D world, pixelated skin** — like Minecraft but a farm.
- All UI text uses Press Start 2P
- 3D models use blocky/voxel-style geometry and pixel textures
- No smooth gradients in UI — hard edges, pixel-perfect
- Dialogue box should look like a Minecraft chat box (dark panel, pixel font, no rounded corners)
