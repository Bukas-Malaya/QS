using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace WhereFirefliesReturn.UI
{
    /// <summary>
    /// Drives the full cinematic opening sequence for the Main Menu scene.
    /// Sequence: black → background bleeds in → fireflies spawn →
    ///           title emerges → tagline word-reveal → buttons slide up →
    ///           everything breathes, indefinitely.
    ///
    /// All timing values are exposed in the Inspector so Jerome can tune
    /// the feel without touching code.
    /// </summary>
    public class MainMenuAnimator : MonoBehaviour
    {
        // ── UI References ──────────────────────────────────────────────────────
        [Header("Background Layers")]
        [SerializeField] private Image bgBottom;        // deep forest floor
        [SerializeField] private Image bgTop;           // midnight sky bleed
        [SerializeField] private Image moonGlow;        // soft radial white

        [Header("Title")]
        [SerializeField] private TextMeshProUGUI titleText;

        [Header("Tagline")]
        [SerializeField] private TextMeshProUGUI taglineText;

        [Header("Buttons")]
        [SerializeField] private CanvasGroup buttonGroup;
        [SerializeField] private Image beginGlow;       // pulse behind Start button

        [Header("Fireflies")]
        [SerializeField] private FireflyEmitter ambientFireflies;
        [SerializeField] private FireflyEmitter focalFireflies;

        // ── Timing (seconds) ───────────────────────────────────────────────────
        [Header("Sequence Timing")]
        [SerializeField] private float bgFadeDuration       = 1.60f;
        [SerializeField] private float firstFireflyDelay    = 0.80f;  // after bg fades
        [SerializeField] private float titleRevealStart     = 1.40f;  // after first firefly
        [SerializeField] private float titleFadeDuration    = 1.30f;
        [SerializeField] private float taglineStartDelay    = 0.55f;  // after title settles
        [SerializeField] private float wordInterval         = 0.22f;  // per word
        [SerializeField] private float wordFadeDuration     = 0.14f;
        [SerializeField] private float buttonRevealDelay    = 0.75f;  // after last tagline word
        [SerializeField] private float buttonSlideDuration  = 0.70f;
        [SerializeField] private float buttonSlideDistance  = 42f;    // px, slides up from below

        [Header("Breathing Loop")]
        [SerializeField] private float breatheAmplitude    = 0.018f;  // max scale offset
        [SerializeField] private float breathePeriod       = 3.60f;   // seconds per full cycle

        [Header("Glow Pulse")]
        [SerializeField] private float glowMin             = 0.12f;
        [SerializeField] private float glowMax             = 0.48f;
        [SerializeField] private float glowPeriod          = 2.10f;

        [Header("Background Oscillation")]
        [SerializeField] private float bgOscillationPeriod = 7.00f;

        // ── Background oscillation target colours ──────────────────────────────
        private static readonly Color BgBottomA = new Color(0.051f, 0.122f, 0.051f, 1f); // #0D1F0D
        private static readonly Color BgBottomB = new Color(0.035f, 0.090f, 0.090f, 1f); // teal shift

        // ── Runtime state ──────────────────────────────────────────────────────
        private Vector2 buttonGroupShown;
        private Vector2 buttonGroupHidden;
        private RectTransform buttonGroupRect;
        private string[] taglineWords;

        // ── Unity lifecycle ────────────────────────────────────────────────────
        void Awake()
        {
            HideAll();
            CacheButtonPositions();
            CacheTaglineWords();
        }

        void Start()
        {
            StartCoroutine(OpeningSequence());
        }

        // ── Master sequence ────────────────────────────────────────────────────
        IEnumerator OpeningSequence()
        {
            // 1 — Background bleeds in
            yield return FadeIn(bgBottom, bgFadeDuration, 1.00f);
            StartCoroutine(FadeIn(bgTop,    bgFadeDuration * 0.8f, 0.60f));
            StartCoroutine(FadeIn(moonGlow, bgFadeDuration * 0.6f, 0.12f));

            // 2 — Ambient fireflies drift up
            yield return Wait(firstFireflyDelay);
            ambientFireflies?.StartEmitting();

            // 3 — Title materialises; focal fireflies burst simultaneously
            yield return Wait(titleRevealStart);
            StartCoroutine(RevealTitle());
            yield return Wait(titleFadeDuration * 0.5f);
            focalFireflies?.TriggerBurst();
            yield return Wait(titleFadeDuration * 0.5f);

            // 4 — Tagline appears word by word
            yield return Wait(taglineStartDelay);
            yield return RevealTagline();

            // 5 — Buttons slide up into view
            yield return Wait(buttonRevealDelay);
            yield return RevealButtons();

            // 6 — Perpetual ambient loops
            StartCoroutine(TitleBreathe());
            StartCoroutine(BeginGlowPulse());
            StartCoroutine(BackgroundOscillate());
        }

        // ── Step coroutines ────────────────────────────────────────────────────
        IEnumerator RevealTitle()
        {
            if (titleText == null) yield break;

            float elapsed    = 0f;
            Color startColor = new Color(titleText.color.r, titleText.color.g, titleText.color.b, 0f);
            Vector3 startSc  = Vector3.one * 0.90f;
            titleText.color            = startColor;
            titleText.transform.localScale = startSc;

            while (elapsed < titleFadeDuration)
            {
                elapsed += Time.deltaTime;
                float t = Mathf.SmoothStep(0f, 1f, Mathf.Clamp01(elapsed / titleFadeDuration));
                titleText.color = new Color(startColor.r, startColor.g, startColor.b, t);
                titleText.transform.localScale = Vector3.Lerp(startSc, Vector3.one, t);
                yield return null;
            }
            titleText.color = new Color(startColor.r, startColor.g, startColor.b, 1f);
            titleText.transform.localScale = Vector3.one;
        }

        IEnumerator RevealTagline()
        {
            if (taglineText == null || taglineWords == null) yield break;

            // Restore full text, then clear and rebuild word-by-word
            taglineText.text  = "";
            Color textColor   = taglineText.color;
            taglineText.color = new Color(textColor.r, textColor.g, textColor.b, 1f);

            string built = "";
            foreach (string word in taglineWords)
            {
                built += (built.Length > 0 ? " " : "") + word;
                taglineText.text = built;

                // Brief per-word fade duration (feel without TMP vertex manipulation)
                yield return Wait(wordInterval);
            }
        }

        IEnumerator RevealButtons()
        {
            if (buttonGroup == null) yield break;

            float elapsed = 0f;
            while (elapsed < buttonSlideDuration)
            {
                elapsed += Time.deltaTime;
                float t = Mathf.SmoothStep(0f, 1f, Mathf.Clamp01(elapsed / buttonSlideDuration));
                buttonGroup.alpha = t;
                if (buttonGroupRect != null)
                    buttonGroupRect.anchoredPosition = Vector2.Lerp(buttonGroupHidden, buttonGroupShown, t);
                yield return null;
            }
            buttonGroup.alpha = 1f;
            if (buttonGroupRect != null) buttonGroupRect.anchoredPosition = buttonGroupShown;
            buttonGroup.interactable    = true;
            buttonGroup.blocksRaycasts  = true;
        }

        // ── Perpetual loops ────────────────────────────────────────────────────
        IEnumerator TitleBreathe()
        {
            if (titleText == null) yield break;
            while (true)
            {
                float t     = (Mathf.Sin(Time.time * Mathf.PI * 2f / breathePeriod) + 1f) * 0.5f;
                float scale = 1f + breatheAmplitude * t;
                titleText.transform.localScale = Vector3.one * scale;
                yield return null;
            }
        }

        IEnumerator BeginGlowPulse()
        {
            if (beginGlow == null) yield break;
            while (true)
            {
                float t     = (Mathf.Sin(Time.time * Mathf.PI * 2f / glowPeriod) + 1f) * 0.5f;
                float alpha = Mathf.Lerp(glowMin, glowMax, t);
                SetAlpha(beginGlow, alpha);
                yield return null;
            }
        }

        IEnumerator BackgroundOscillate()
        {
            if (bgBottom == null) yield break;
            while (true)
            {
                float t = (Mathf.Sin(Time.time * Mathf.PI * 2f / bgOscillationPeriod) + 1f) * 0.5f;
                Color c = Color.Lerp(BgBottomA, BgBottomB, t);
                c.a     = bgBottom.color.a;
                bgBottom.color = c;
                yield return null;
            }
        }

        // ── Fade helpers ───────────────────────────────────────────────────────
        IEnumerator FadeIn(Image img, float duration, float targetAlpha)
        {
            if (img == null) yield break;
            float elapsed = 0f;
            float start   = img.color.a;
            while (elapsed < duration)
            {
                elapsed += Time.deltaTime;
                SetAlpha(img, Mathf.Lerp(start, targetAlpha, Mathf.SmoothStep(0f, 1f, elapsed / duration)));
                yield return null;
            }
            SetAlpha(img, targetAlpha);
        }

        static IEnumerator Wait(float seconds)
        {
            float elapsed = 0f;
            while (elapsed < seconds) { elapsed += Time.deltaTime; yield return null; }
        }

        static void SetAlpha(Image img, float alpha)
        {
            Color c = img.color;
            img.color = new Color(c.r, c.g, c.b, alpha);
        }

        // ── Setup helpers ──────────────────────────────────────────────────────
        void HideAll()
        {
            SetAlpha(bgBottom, 0f);
            SetAlpha(bgTop,    0f);
            SetAlpha(moonGlow, 0f);
            SetAlpha(beginGlow, 0f);

            if (titleText != null)
            {
                Color c = titleText.color;
                titleText.color            = new Color(c.r, c.g, c.b, 0f);
                titleText.transform.localScale = Vector3.one * 0.90f;
            }
            if (taglineText != null)
            {
                Color c = taglineText.color;
                taglineText.color = new Color(c.r, c.g, c.b, 0f);
            }
            if (buttonGroup != null)
            {
                buttonGroup.alpha          = 0f;
                buttonGroup.interactable   = false;
                buttonGroup.blocksRaycasts = false;
            }
        }

        void CacheButtonPositions()
        {
            if (buttonGroup == null) return;
            buttonGroupRect = buttonGroup.GetComponent<RectTransform>();
            if (buttonGroupRect == null) return;
            buttonGroupShown  = buttonGroupRect.anchoredPosition;
            buttonGroupHidden = buttonGroupShown - new Vector2(0f, buttonSlideDistance);
            buttonGroupRect.anchoredPosition = buttonGroupHidden;
        }

        void CacheTaglineWords()
        {
            if (taglineText == null) return;
            taglineWords = taglineText.text.Split(' ');
        }
    }
}
