using UnityEngine;
using UnityEngine.UI;
using TMPro;
using WhereFirefliesReturn.Environment;
using WhereFirefliesReturn.Resources;
using WhereFirefliesReturn.Farm;

namespace WhereFirefliesReturn.UI
{
    /// <summary>
    /// HUD Manager — auto-generates a Canvas HUD at runtime if no UI is assigned.
    /// Attach to any GameObject in your GameWorld scene and press Play to see the HUD.
    /// </summary>
    public class UIManager : MonoBehaviour
    {
        public static UIManager Instance { get; private set; }

        // ── Serialized refs (assign in editor or leave null to auto-generate) ──
        [Header("Environment Bar")]
        [SerializeField] Image envFillBar;
        [SerializeField] TMP_Text envValueText;

        [Header("Resources")]
        [SerializeField] TMP_Text waterText;
        [SerializeField] TMP_Text seedsText;
        [SerializeField] TMP_Text energyText;

        [Header("Cycle")]
        [SerializeField] TMP_Text cycleText;

        [Header("Interaction Prompt")]
        [SerializeField] TMP_Text interactionPrompt;

        // ── Runtime-generated canvas ──
        Canvas _canvas;

        // ── Color palette ──
        static readonly Color BgDark       = new Color(0.08f, 0.10f, 0.08f, 0.88f);
        static readonly Color GreenFull    = new Color(0.24f, 0.72f, 0.35f);
        static readonly Color YellowMid    = new Color(0.95f, 0.77f, 0.20f);
        static readonly Color RedLow       = new Color(0.85f, 0.22f, 0.18f);
        static readonly Color TextPrimary  = new Color(0.92f, 0.95f, 0.88f);
        static readonly Color TextMuted    = new Color(0.55f, 0.65f, 0.52f);
        static readonly Color WaterBlue    = new Color(0.35f, 0.72f, 0.95f);
        static readonly Color SeedGreen    = new Color(0.55f, 0.88f, 0.40f);
        static readonly Color EnergyYellow = new Color(0.98f, 0.84f, 0.30f);

        void Awake()
        {
            if (Instance != null && Instance != this) { Destroy(gameObject); return; }
            Instance = this;
        }

        void Start()
        {
            if (envFillBar == null) BuildHUD();

            SubscribeToEvents();
            RefreshAll();
        }

        // ─────────────────────────────────────────────────────────────────────
        // Event subscriptions
        // ─────────────────────────────────────────────────────────────────────

        void SubscribeToEvents()
        {
            if (EnvironmentMeter.Instance != null)
                EnvironmentMeter.Instance.OnValueChanged += OnEnvChanged;

            if (ResourceManager.Instance != null)
                ResourceManager.Instance.OnResourceChanged += OnResourceChanged;

            if (CycleManager.Instance != null)
            {
                CycleManager.Instance.OnCycleStart += OnCycleStart;
            }
        }

        void OnDestroy()
        {
            if (EnvironmentMeter.Instance != null)
                EnvironmentMeter.Instance.OnValueChanged -= OnEnvChanged;
            if (ResourceManager.Instance != null)
                ResourceManager.Instance.OnResourceChanged -= OnResourceChanged;
            if (CycleManager.Instance != null)
                CycleManager.Instance.OnCycleStart -= OnCycleStart;
        }

        // ─────────────────────────────────────────────────────────────────────
        // Public API
        // ─────────────────────────────────────────────────────────────────────

        public void ShowInteractionPrompt(string message)
        {
            if (interactionPrompt == null) return;
            interactionPrompt.text = message;
            interactionPrompt.gameObject.SetActive(true);
        }

        public void HideInteractionPrompt()
        {
            interactionPrompt?.gameObject.SetActive(false);
        }

        // ─────────────────────────────────────────────────────────────────────
        // Refresh
        // ─────────────────────────────────────────────────────────────────────

        void RefreshAll()
        {
            if (EnvironmentMeter.Instance != null) OnEnvChanged(EnvironmentMeter.Instance.Value);
            if (ResourceManager.Instance != null)
            {
                OnResourceChanged(ResourceType.Water, ResourceManager.Instance.Water);
                OnResourceChanged(ResourceType.Seeds, ResourceManager.Instance.Seeds);
                OnResourceChanged(ResourceType.CleanEnergy, ResourceManager.Instance.CleanEnergy);
            }
            if (CycleManager.Instance != null) OnCycleStart(CycleManager.Instance.CurrentCycle);
        }

        void OnEnvChanged(float value)
        {
            if (envFillBar != null)
            {
                envFillBar.fillAmount = value / 100f;
                envFillBar.color = Color.Lerp(
                    Color.Lerp(RedLow, YellowMid, Mathf.InverseLerp(0f, 50f, value)),
                    GreenFull,
                    Mathf.InverseLerp(50f, 100f, value)
                );
            }
            if (envValueText != null) envValueText.text = $"{Mathf.RoundToInt(value)}";
        }

        void OnResourceChanged(ResourceType type, int amount)
        {
            switch (type)
            {
                case ResourceType.Water:       if (waterText)  waterText.text  = $"{amount}"; break;
                case ResourceType.Seeds:       if (seedsText)  seedsText.text  = $"{amount}"; break;
                case ResourceType.CleanEnergy: if (energyText) energyText.text = $"{amount}"; break;
            }
        }

        void OnCycleStart(int cycle)
        {
            if (cycleText != null && CycleManager.Instance != null)
                cycleText.text = $"Cycle {cycle} / {CycleManager.Instance.TotalCycles}";
        }

        // ─────────────────────────────────────────────────────────────────────
        // Auto-generated HUD (runs when no UI refs are assigned)
        // ─────────────────────────────────────────────────────────────────────

        void BuildHUD()
        {
            // Canvas
            var canvasGO = new GameObject("HUD_Canvas");
            _canvas = canvasGO.AddComponent<Canvas>();
            _canvas.renderMode = RenderMode.ScreenSpaceOverlay;
            _canvas.sortingOrder = 10;

            var scaler = canvasGO.AddComponent<CanvasScaler>();
            scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
            scaler.referenceResolution = new Vector2(1920, 1080);
            scaler.matchWidthOrHeight = 0.5f;

            canvasGO.AddComponent<GraphicRaycaster>();

            // ── Top bar ──────────────────────────────────────────────────────
            var topBar = MakePanel(canvasGO.transform, "TopBar",
                new Vector2(0, 1), new Vector2(1, 1), new Vector2(0, 1),
                new Vector2(0, -72), new Vector2(0, 0));
            topBar.GetComponent<Image>().color = BgDark;

            // Environment section (left)
            var envSection = MakeRect(topBar.transform, "EnvSection",
                new Vector2(0, 0), new Vector2(0, 0), new Vector2(0, 0.5f),
                new Vector2(16, 0), new Vector2(320, 72));

            MakeLabel(envSection.transform, "EnvLabel", "ECOSYSTEM",
                new Vector2(0, 1), new Vector2(0, 1), new Vector2(0, 1),
                new Vector2(0, -6), new Vector2(160, 22), 11f, TextMuted);

            var barBg = MakeRect(envSection.transform, "EnvBarBg",
                new Vector2(0, 0), new Vector2(0, 0), new Vector2(0, 0.5f),
                new Vector2(0, 0), new Vector2(260, 18));
            var barBgImg = barBg.AddComponent<Image>();
            barBgImg.color = new Color(0.15f, 0.18f, 0.14f);
            SetAnchors(barBg, new Vector2(0, 0.5f), new Vector2(0, 0.5f), new Vector2(0, 0.5f));
            barBg.GetComponent<RectTransform>().anchoredPosition = new Vector2(0, 6);
            barBg.GetComponent<RectTransform>().sizeDelta = new Vector2(260, 18);

            var barFill = MakeRect(barBg.transform, "EnvFill",
                new Vector2(0, 0), new Vector2(0, 0), new Vector2(0, 0.5f),
                Vector2.zero, new Vector2(260, 18));
            envFillBar = barFill.AddComponent<Image>();
            envFillBar.color = GreenFull;
            envFillBar.type = Image.Type.Filled;
            envFillBar.fillMethod = Image.FillMethod.Horizontal;
            envFillBar.fillAmount = 0.5f;
            SetAnchors(barFill, Vector2.zero, Vector2.one, new Vector2(0, 0.5f));
            barFill.GetComponent<RectTransform>().sizeDelta = Vector2.zero;
            barFill.GetComponent<RectTransform>().anchoredPosition = Vector2.zero;

            envValueText = MakeLabel(envSection.transform, "EnvValue", "50",
                new Vector2(0, 0.5f), new Vector2(0, 0.5f), new Vector2(0, 0.5f),
                new Vector2(270, 6), new Vector2(48, 24), 16f, TextPrimary);

            // Resources section (center-right)
            var resSection = MakeRect(topBar.transform, "ResourceSection",
                new Vector2(1, 0), new Vector2(1, 1), new Vector2(1, 0.5f),
                new Vector2(-16, 0), new Vector2(360, 0));
            resSection.GetComponent<RectTransform>().anchorMin = new Vector2(0.3f, 0);
            resSection.GetComponent<RectTransform>().anchorMax = new Vector2(1f, 1f);
            resSection.GetComponent<RectTransform>().sizeDelta = new Vector2(-32, 0);
            resSection.GetComponent<RectTransform>().anchoredPosition = Vector2.zero;

            var resLayout = resSection.AddComponent<HorizontalLayoutGroup>();
            resLayout.childAlignment = TextAnchor.MiddleRight;
            resLayout.spacing = 24f;
            resLayout.padding = new RectOffset(0, 16, 0, 0);
            resLayout.childForceExpandWidth = false;
            resLayout.childForceExpandHeight = true;

            waterText  = MakeResourceChip(resSection.transform, "WATER", "0", WaterBlue);
            seedsText  = MakeResourceChip(resSection.transform, "SEEDS", "0", SeedGreen);
            energyText = MakeResourceChip(resSection.transform, "ENERGY", "0", EnergyYellow);

            // Cycle badge (far right)
            var cycleBadge = MakeRect(topBar.transform, "CycleBadge",
                new Vector2(1, 0.5f), new Vector2(1, 0.5f), new Vector2(1, 0.5f),
                new Vector2(-130, 0), new Vector2(110, 40));
            var cycleImg = cycleBadge.AddComponent<Image>();
            cycleImg.color = new Color(0.18f, 0.24f, 0.16f, 0.9f);

            cycleText = MakeLabel(cycleBadge.transform, "CycleText", "Cycle 1 / 5",
                Vector2.zero, Vector2.one, new Vector2(0.5f, 0.5f),
                Vector2.zero, Vector2.zero, 13f, TextPrimary);
            cycleText.alignment = TextAlignmentOptions.Center;

            // ── Interaction prompt (bottom center) ───────────────────────────
            var promptRoot = MakeRect(canvasGO.transform, "InteractionPrompt",
                new Vector2(0.5f, 0), new Vector2(0.5f, 0), new Vector2(0.5f, 0),
                new Vector2(0, 60), new Vector2(340, 44));
            var promptImg = promptRoot.AddComponent<Image>();
            promptImg.color = new Color(0.05f, 0.08f, 0.05f, 0.80f);

            interactionPrompt = MakeLabel(promptRoot.transform, "PromptText", "[E] Collect",
                Vector2.zero, Vector2.one, new Vector2(0.5f, 0.5f),
                Vector2.zero, Vector2.zero, 14f, TextPrimary);
            interactionPrompt.alignment = TextAlignmentOptions.Center;
            promptRoot.SetActive(false);
        }

        // ─────────────────────────────────────────────────────────────────────
        // UI helpers
        // ─────────────────────────────────────────────────────────────────────

        static GameObject MakePanel(Transform parent, string name,
            Vector2 anchorMin, Vector2 anchorMax, Vector2 pivot,
            Vector2 anchoredPos, Vector2 sizeDelta)
        {
            var go = new GameObject(name, typeof(RectTransform), typeof(Image));
            go.transform.SetParent(parent, false);
            var rt = go.GetComponent<RectTransform>();
            rt.anchorMin = anchorMin;
            rt.anchorMax = anchorMax;
            rt.pivot = pivot;
            rt.anchoredPosition = anchoredPos;
            rt.sizeDelta = sizeDelta;
            return go;
        }

        static GameObject MakeRect(Transform parent, string name,
            Vector2 anchorMin, Vector2 anchorMax, Vector2 pivot,
            Vector2 anchoredPos, Vector2 sizeDelta)
        {
            var go = new GameObject(name, typeof(RectTransform));
            go.transform.SetParent(parent, false);
            var rt = go.GetComponent<RectTransform>();
            rt.anchorMin = anchorMin;
            rt.anchorMax = anchorMax;
            rt.pivot = pivot;
            rt.anchoredPosition = anchoredPos;
            rt.sizeDelta = sizeDelta;
            return go;
        }

        static TMP_Text MakeLabel(Transform parent, string name, string text,
            Vector2 anchorMin, Vector2 anchorMax, Vector2 pivot,
            Vector2 anchoredPos, Vector2 sizeDelta,
            float fontSize, Color color)
        {
            var go = new GameObject(name, typeof(RectTransform));
            go.transform.SetParent(parent, false);
            var rt = go.GetComponent<RectTransform>();
            rt.anchorMin = anchorMin;
            rt.anchorMax = anchorMax;
            rt.pivot = pivot;
            rt.anchoredPosition = anchoredPos;
            rt.sizeDelta = sizeDelta;

            var tmp = go.AddComponent<TextMeshProUGUI>();
            tmp.text = text;
            tmp.fontSize = fontSize;
            tmp.color = color;
            tmp.fontStyle = FontStyles.Bold;
            tmp.enableWordWrapping = false;
            return tmp;
        }

        static TMP_Text MakeResourceChip(Transform parent, string icon, string value, Color accentColor)
        {
            var chip = new GameObject($"Chip_{icon}", typeof(RectTransform));
            chip.transform.SetParent(parent, false);
            var chipRt = chip.GetComponent<RectTransform>();
            chipRt.sizeDelta = new Vector2(80, 48);

            var layout = chip.AddComponent<HorizontalLayoutGroup>();
            layout.childAlignment = TextAnchor.MiddleCenter;
            layout.spacing = 4f;
            layout.childForceExpandWidth = false;
            layout.childForceExpandHeight = false;

            // Icon
            var iconGO = new GameObject("Icon", typeof(RectTransform));
            iconGO.transform.SetParent(chip.transform, false);
            var iconTmp = iconGO.AddComponent<TextMeshProUGUI>();
            iconTmp.text = icon;
            iconTmp.fontSize = 18f;
            iconTmp.enableWordWrapping = false;
            iconGO.GetComponent<RectTransform>().sizeDelta = new Vector2(26, 26);

            // Value
            var valGO = new GameObject("Value", typeof(RectTransform));
            valGO.transform.SetParent(chip.transform, false);
            var valTmp = valGO.AddComponent<TextMeshProUGUI>();
            valTmp.text = value;
            valTmp.fontSize = 18f;
            valTmp.color = accentColor;
            valTmp.fontStyle = FontStyles.Bold;
            valTmp.enableWordWrapping = false;
            valGO.GetComponent<RectTransform>().sizeDelta = new Vector2(48, 26);

            return valTmp;
        }

        static void SetAnchors(GameObject go, Vector2 min, Vector2 max, Vector2 pivot)
        {
            var rt = go.GetComponent<RectTransform>();
            rt.anchorMin = min;
            rt.anchorMax = max;
            rt.pivot = pivot;
        }
    }
}
