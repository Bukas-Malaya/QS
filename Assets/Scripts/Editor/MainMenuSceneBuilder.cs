#if UNITY_EDITOR
// ──────────────────────────────────────────────────────────────────────────────
// MainMenuSceneBuilder.cs
// Menu: Fireflies ▸ Setup Main Menu Scene
//
// Builds the entire MainMenu scene canvas hierarchy in one click.
// Jerome runs this, hits Play, and the opening sequence plays.
// No manual Inspector wiring needed.
// ──────────────────────────────────────────────────────────────────────────────
using UnityEngine;
using UnityEngine.UI;
using UnityEditor;
using TMPro;
using WhereFirefliesReturn.UI;
using WhereFirefliesReturn.Core;

namespace WhereFirefliesReturn.Editor
{
    public static class MainMenuSceneBuilder
    {
        // ── Font asset paths ───────────────────────────────────────────────────
        private const string FontPinyonPath =
            "Assets/Art/Fonts/Pinyon_Script/PinyonScript-Regular SDF.asset";
        private const string FontDMSansPath =
            "Assets/Art/Fonts/DM_Sans/DMSans-VariableFont_opsz,wght SDF.asset";
        private const string FontDMSansItalicPath =
            "Assets/Art/Fonts/DM_Sans/DMSans-Italic-VariableFont_opsz,wght SDF.asset";

        // ── Palette ────────────────────────────────────────────────────────────
        private static readonly Color BgBottom     = Hex("#0D1F0D");
        private static readonly Color BgTop        = Hex("#1A3A4A");
        private static readonly Color MoonWhite    = Hex("#FFFFFF");
        private static readonly Color TitleCream   = Hex("#F5ECD7");
        private static readonly Color TaglineGold  = Hex("#C8A96E");
        private static readonly Color BtnBg        = Hex("#3D2B1F");
        private static readonly Color BtnText      = Hex("#F5ECD7");
        private static readonly Color GlowYellow   = Hex("#C8A96E");
        private static readonly Color CreditsDim   = Hex("#8A7A60");

        // ── Tagline ────────────────────────────────────────────────────────────
        private const string Tagline = "Five seasons. One land. Bring them home.";
        private const string Credits = "A QS ImpACT Ideathon entry  ·  Team Bukas-Malaya";

        // ══════════════════════════════════════════════════════════════════════
        [MenuItem("Fireflies/Setup Main Menu Scene")]
        public static void BuildMainMenuScene()
        {
            Clear();

            // ── Load fonts ─────────────────────────────────────────────────────
            TMP_FontAsset fontPinyon     = AssetDatabase.LoadAssetAtPath<TMP_FontAsset>(FontPinyonPath);
            TMP_FontAsset fontDMSans     = AssetDatabase.LoadAssetAtPath<TMP_FontAsset>(FontDMSansPath);
            TMP_FontAsset fontDMSansItal = AssetDatabase.LoadAssetAtPath<TMP_FontAsset>(FontDMSansItalicPath);

            if (fontPinyon    == null) Debug.LogWarning("[MainMenuBuilder] Pinyon Script SDF not found at: " + FontPinyonPath);
            if (fontDMSans    == null) Debug.LogWarning("[MainMenuBuilder] DM Sans SDF not found at: " + FontDMSansPath);
            if (fontDMSansItal== null) Debug.LogWarning("[MainMenuBuilder] DM Sans Italic SDF not found at: " + FontDMSansItalicPath);

            // ── Camera ─────────────────────────────────────────────────────────
            Camera cam = BuildCamera();

            // ── Canvas ─────────────────────────────────────────────────────────
            GameObject root   = new GameObject("=== MAIN MENU ===");
            Canvas canvas     = BuildCanvas(root, cam);

            // ── Background layers ──────────────────────────────────────────────
            Image bgBottom = BuildBackground(root, "BG_Bottom", BgBottom, 0, Vector2.zero, Vector2.one);
            Image bgTop    = BuildBackground(root, "BG_Top",    BgTop,    1,
                                new Vector2(0f, 0.42f), Vector2.one,
                                topAlpha: 0.58f);
            Image moonGlow = BuildMoonGlow(root);

            // ── Firefly emitters (world-space GOs, visible through camera) ─────
            FireflyEmitter ambient = BuildFireflyEmitter("FireflyAmbient", FireflyEmitter.EmitterMode.Ambient);
            FireflyEmitter focal   = BuildFireflyEmitter("FireflyFocal",   FireflyEmitter.EmitterMode.Focal, new Vector3(0f, 1.2f, 0f));

            // ── Title group ────────────────────────────────────────────────────
            GameObject titleGroup = new GameObject("TitleGroup");
            titleGroup.transform.SetParent(root.transform, false);
            RectTransform titleGroupRT = titleGroup.AddComponent<RectTransform>();
            titleGroupRT.anchorMin        = new Vector2(0.5f, 0.58f);
            titleGroupRT.anchorMax        = new Vector2(0.5f, 0.58f);
            titleGroupRT.anchoredPosition = Vector2.zero;
            titleGroupRT.sizeDelta        = new Vector2(900f, 220f);
            var titleVLG = titleGroup.AddComponent<VerticalLayoutGroup>();
            titleVLG.childAlignment        = TextAnchor.MiddleCenter;
            titleVLG.spacing               = 14f;
            titleVLG.childControlWidth     = true;
            titleVLG.childControlHeight    = false;

            TextMeshProUGUI titleTMP = BuildTMP(titleGroup, "TitleText",
                "Where Fireflies Return",
                fontPinyon ?? fontDMSans, 78f, TitleCream,
                TextAlignmentOptions.Center);
            titleTMP.fontStyle = FontStyles.Normal;

            TextMeshProUGUI taglineTMP = BuildTMP(titleGroup, "TaglineText",
                Tagline,
                fontDMSansItal ?? fontDMSans, 26f, TaglineGold,
                TextAlignmentOptions.Center);
            taglineTMP.fontStyle    = FontStyles.Italic;
            taglineTMP.lineSpacing  = 4f;

            // ── Button group ───────────────────────────────────────────────────
            GameObject btnGroup = new GameObject("ButtonGroup");
            btnGroup.transform.SetParent(root.transform, false);
            RectTransform btnGroupRT = btnGroup.AddComponent<RectTransform>();
            btnGroupRT.anchorMin        = new Vector2(0.5f, 0.24f);
            btnGroupRT.anchorMax        = new Vector2(0.5f, 0.24f);
            btnGroupRT.anchoredPosition = Vector2.zero;
            btnGroupRT.sizeDelta        = new Vector2(260f, 130f);
            CanvasGroup btnCG = btnGroup.AddComponent<CanvasGroup>();
            var btnVLG = btnGroup.AddComponent<VerticalLayoutGroup>();
            btnVLG.childAlignment     = TextAnchor.MiddleCenter;
            btnVLG.spacing            = 16f;
            btnVLG.childControlWidth  = true;
            btnVLG.childControlHeight = false;

            // Begin glow — rendered behind Start button
            Image beginGlow = BuildBeginGlow(btnGroup);

            Button startBtn = BuildButton(btnGroup, "StartButton", "Begin",
                                fontDMSans, BtnBg, BtnText, height: 52f);
            Button quitBtn  = BuildButton(btnGroup, "QuitButton",  "Quit",
                                fontDMSans, new Color(0.18f, 0.18f, 0.18f), BtnText, height: 44f);

            // ── Credits ────────────────────────────────────────────────────────
            BuildCredits(root, fontDMSans, Credits);

            // ── Logic GameObject ───────────────────────────────────────────────
            GameObject logic = new GameObject("[MainMenuLogic]");
            logic.transform.SetParent(root.transform, false);

            // MainMenuUI — wire via SerializedObject so private fields are set
            MainMenuUI menuUI = logic.AddComponent<MainMenuUI>();
            WirePrivateField(menuUI, "startButton", startBtn);
            WirePrivateField(menuUI, "quitButton",  quitBtn);

            // MainMenuAnimator — wire all references
            MainMenuAnimator anim = logic.AddComponent<MainMenuAnimator>();
            WirePrivateField(anim, "bgBottom",         bgBottom);
            WirePrivateField(anim, "bgTop",            bgTop);
            WirePrivateField(anim, "moonGlow",         moonGlow);
            WirePrivateField(anim, "titleText",        titleTMP);
            WirePrivateField(anim, "taglineText",      taglineTMP);
            WirePrivateField(anim, "buttonGroup",      btnCG);
            WirePrivateField(anim, "beginGlow",        beginGlow);
            WirePrivateField(anim, "ambientFireflies", ambient);
            WirePrivateField(anim, "focalFireflies",   focal);

            // Mark scene dirty
            UnityEditor.SceneManagement.EditorSceneManager.MarkSceneDirty(
                UnityEditor.SceneManagement.EditorSceneManager.GetActiveScene());

            Debug.Log("[MainMenuBuilder] ✓ Main Menu scene built. Hit Play to preview the opening sequence.");
        }

        // ──────────────────────────────────────────────────────────────────────
        [MenuItem("Fireflies/Clear Main Menu Scene")]
        public static void Clear()
        {
            var existing = GameObject.Find("=== MAIN MENU ===");
            if (existing != null)
            {
                Object.DestroyImmediate(existing);
                Debug.Log("[MainMenuBuilder] Cleared existing Main Menu.");
            }
        }

        // ── Camera ─────────────────────────────────────────────────────────────
        static Camera BuildCamera()
        {
            Camera cam = Camera.main;
            if (cam == null)
            {
                var go   = new GameObject("Main Camera");
                go.tag   = "MainCamera";
                cam      = go.AddComponent<Camera>();
                go.AddComponent<AudioListener>();
            }

            cam.clearFlags      = CameraClearFlags.SolidColor;
            cam.backgroundColor = new Color(0.051f, 0.122f, 0.051f); // #0D1F0D — matches BgBottom
            cam.orthographic    = true;
            cam.orthographicSize = 5f;
            cam.nearClipPlane   = -10f;
            cam.farClipPlane    = 100f;
            cam.transform.position = new Vector3(0f, 0f, -10f);
            cam.transform.rotation = Quaternion.identity;

            return cam;
        }

        // ── Canvas ─────────────────────────────────────────────────────────────
        static Canvas BuildCanvas(GameObject parent, Camera cam)
        {
            Canvas canvas = parent.AddComponent<Canvas>();
            canvas.renderMode   = RenderMode.ScreenSpaceCamera;
            canvas.worldCamera  = cam;
            canvas.planeDistance = 5f;
            canvas.sortingOrder  = 0;

            var scaler = parent.AddComponent<CanvasScaler>();
            scaler.uiScaleMode         = CanvasScaler.ScaleMode.ScaleWithScreenSize;
            scaler.referenceResolution = new Vector2(1920f, 1080f);
            scaler.matchWidthOrHeight  = 0.5f;

            parent.AddComponent<GraphicRaycaster>();

            // EventSystem — create only if none exists
            if (Object.FindFirstObjectByType<UnityEngine.EventSystems.EventSystem>() == null)
            {
                var esGo = new GameObject("EventSystem");
                esGo.AddComponent<UnityEngine.EventSystems.EventSystem>();
                esGo.AddComponent<UnityEngine.EventSystems.StandaloneInputModule>();
            }

            return canvas;
        }

        // ── Background layers ───────────────────────────────────────────────────
        static Image BuildBackground(GameObject parent, string name, Color color,
            int siblingIndex, Vector2 anchorMin, Vector2 anchorMax, float topAlpha = 1f)
        {
            var go = new GameObject(name);
            go.transform.SetParent(parent.transform, false);
            var rt = go.AddComponent<RectTransform>();
            rt.anchorMin  = anchorMin;
            rt.anchorMax  = anchorMax;
            rt.offsetMin  = Vector2.zero;
            rt.offsetMax  = Vector2.zero;

            var img   = go.AddComponent<Image>();
            img.color = new Color(color.r, color.g, color.b, topAlpha);
            img.raycastTarget = false;
            go.transform.SetSiblingIndex(siblingIndex);
            return img;
        }

        static Image BuildMoonGlow(GameObject parent)
        {
            var go = new GameObject("MoonGlow");
            go.transform.SetParent(parent.transform, false);
            var rt = go.AddComponent<RectTransform>();
            rt.anchorMin        = new Vector2(0.5f, 0.72f);
            rt.anchorMax        = new Vector2(0.5f, 0.72f);
            rt.anchoredPosition = Vector2.zero;
            rt.sizeDelta        = new Vector2(420f, 300f);

            var img   = go.AddComponent<Image>();
            img.color = new Color(MoonWhite.r, MoonWhite.g, MoonWhite.b, 0.10f);
            img.raycastTarget = false;

            // Radial softness: use a tightly rounded sprite (default circle from Unity UI)
            // If no circle sprite is loaded, the soft alpha value still reads as atmospheric
            return img;
        }

        // ── Firefly emitters ────────────────────────────────────────────────────
        static FireflyEmitter BuildFireflyEmitter(string name,
            FireflyEmitter.EmitterMode mode, Vector3 offset = default)
        {
            var go = new GameObject(name);
            go.transform.position = offset;
            var ps   = go.AddComponent<ParticleSystem>();
            var em   = go.AddComponent<FireflyEmitter>();

            // Inject mode via SerializedObject before Awake runs in Editor
            WirePrivateField(em, "mode", (int)mode);
            return em;
        }

        // ── Text ────────────────────────────────────────────────────────────────
        static TextMeshProUGUI BuildTMP(GameObject parent, string name,
            string text, TMP_FontAsset font, float fontSize, Color color,
            TextAlignmentOptions alignment)
        {
            var go = new GameObject(name);
            go.transform.SetParent(parent.transform, false);
            var rt = go.AddComponent<RectTransform>();
            rt.sizeDelta = new Vector2(860f, fontSize * 1.6f + 20f);

            var tmp = go.AddComponent<TextMeshProUGUI>();
            tmp.text           = text;
            tmp.font           = font;
            tmp.fontSize       = fontSize;
            tmp.color          = color;
            tmp.alignment      = alignment;
            tmp.enableWordWrapping = true;
            tmp.overflowMode   = TextOverflowModes.Overflow;
            return tmp;
        }

        // ── Buttons ─────────────────────────────────────────────────────────────
        static Button BuildButton(GameObject parent, string name, string label,
            TMP_FontAsset font, Color bgColor, Color textColor, float height = 52f)
        {
            var go = new GameObject(name);
            go.transform.SetParent(parent.transform, false);
            var rt = go.AddComponent<RectTransform>();
            rt.sizeDelta = new Vector2(220f, height);

            var img = go.AddComponent<Image>();
            img.color = bgColor;

            // Rounded corners via sprite would need a sprite — use solid for now
            var btn = go.AddComponent<Button>();
            var colors = btn.colors;
            colors.normalColor      = bgColor;
            colors.highlightedColor = new Color(bgColor.r + 0.12f, bgColor.g + 0.07f, bgColor.b + 0.02f);
            colors.pressedColor     = new Color(bgColor.r - 0.08f, bgColor.g - 0.05f, bgColor.b - 0.02f);
            colors.fadeDuration     = 0.10f;
            btn.colors = colors;

            // Hover effect component
            go.AddComponent<ButtonHoverEffect>();

            // Label
            var labelGo = new GameObject("Label");
            labelGo.transform.SetParent(go.transform, false);
            var labelRT = labelGo.AddComponent<RectTransform>();
            labelRT.anchorMin  = Vector2.zero;
            labelRT.anchorMax  = Vector2.one;
            labelRT.offsetMin  = Vector2.zero;
            labelRT.offsetMax  = Vector2.zero;

            var tmp = labelGo.AddComponent<TextMeshProUGUI>();
            tmp.text      = label;
            tmp.font      = font;
            tmp.fontSize  = 20f;
            tmp.color     = textColor;
            tmp.fontStyle = FontStyles.Bold;
            tmp.alignment = TextAlignmentOptions.Center;

            return btn;
        }

        // ── Begin Glow ──────────────────────────────────────────────────────────
        static Image BuildBeginGlow(GameObject parent)
        {
            var go = new GameObject("BeginGlow");
            go.transform.SetParent(parent.transform, false);
            var rt = go.AddComponent<RectTransform>();
            rt.sizeDelta        = new Vector2(240f, 64f);
            rt.anchoredPosition = new Vector2(0f, -4f);

            var img = go.AddComponent<Image>();
            img.color         = new Color(GlowYellow.r, GlowYellow.g, GlowYellow.b, 0f);
            img.raycastTarget = false;
            return img;
        }

        // ── Credits ─────────────────────────────────────────────────────────────
        static void BuildCredits(GameObject parent, TMP_FontAsset font, string text)
        {
            var go = new GameObject("CreditsText");
            go.transform.SetParent(parent.transform, false);
            var rt = go.AddComponent<RectTransform>();
            rt.anchorMin        = new Vector2(0.5f, 0f);
            rt.anchorMax        = new Vector2(0.5f, 0f);
            rt.anchoredPosition = new Vector2(0f, 22f);
            rt.sizeDelta        = new Vector2(700f, 28f);

            var tmp = go.AddComponent<TextMeshProUGUI>();
            tmp.text      = text;
            tmp.font      = font;
            tmp.fontSize  = 13f;
            tmp.color     = CreditsDim;
            tmp.alignment = TextAlignmentOptions.Center;
            tmp.raycastTarget = false;
        }

        // ── Reflection helper — set [SerializeField] private fields ────────────
        static void WirePrivateField(Object target, string fieldName, object value)
        {
            var so   = new SerializedObject(target);
            var prop = so.FindProperty(fieldName);
            if (prop == null)
            {
                Debug.LogWarning($"[MainMenuBuilder] Field '{fieldName}' not found on {target.GetType().Name}");
                return;
            }

            switch (prop.propertyType)
            {
                case SerializedPropertyType.ObjectReference:
                    prop.objectReferenceValue = value as Object;
                    break;
                case SerializedPropertyType.Enum:
                    prop.enumValueIndex = (int)value;
                    break;
                case SerializedPropertyType.Integer:
                    prop.intValue = (int)value;
                    break;
                case SerializedPropertyType.Float:
                    prop.floatValue = (float)value;
                    break;
                case SerializedPropertyType.Boolean:
                    prop.boolValue = (bool)value;
                    break;
                default:
                    Debug.LogWarning($"[MainMenuBuilder] Unhandled property type '{prop.propertyType}' for '{fieldName}'");
                    return;
            }
            so.ApplyModifiedProperties();
        }

        // ── Colour helper ───────────────────────────────────────────────────────
        static Color Hex(string hex)
        {
            ColorUtility.TryParseHtmlString(hex, out Color c);
            return c;
        }
    }
}
#endif
