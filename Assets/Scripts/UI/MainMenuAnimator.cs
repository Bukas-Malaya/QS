using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace WhereFirefliesReturn.UI
{
    /// summary
    /// Drives the full cinematic opening sequence for the Main Menu scene.
    /// Sequence: black → bgTop visible at bottom → bgBottom pans up from below →
    ///           fireflies spawn → title emerges → tagline word-reveal →
    ///           buttons slide up → everything breathes, indefinitely.
    ///
    ///   bgTop    — sky layer, stays mostly still, subtle upward drift (slower)
    ///   bgBottom — ground/forest layer, starts below screen, pans upward (faster)
    ///   Both images need to be taller than the canvas so they have room to scroll.
    ///   Set Image → Preserve Aspect off, let them fill width, height ~130–150% of canvas.
    /// </summary>
    public class MainMenuAnimator : MonoBehaviour
    {
        [Header("Background Layers")]
        [SerializeField] private Image bgBottom;        // ground / forest floor — pans up fast
        [SerializeField] private Image bgTop;           // sky layer — visible first, drifts slowly
        [SerializeField] private Image moonGlow;        // soft radial white

        [Header("Title")]
        [SerializeField] private TextMeshProUGUI titleText;

        [Header("Tagline")]
        [SerializeField] private TextMeshProUGUI taglineText;

        [Header("Buttons")]
        [SerializeField] private CanvasGroup buttonGroup;
        [SerializeField] private Image beginGlow;

        [Header("Fireflies")]
        [SerializeField] private FireflyEmitter ambientFireflies;
        [SerializeField] private FireflyEmitter focalFireflies;
        

        [Header("Sequence Timing")]
        [SerializeField] private float bgTopFadeDuration    = 1.00f;  // sky fades in first
        [SerializeField] private float bgBottomPanDelay     = 0.40f;  // delay before ground pans
        [SerializeField] private float bgBottomPanDuration  = 2.20f;  // ground pan-in duration
        [SerializeField] private float firstFireflyDelay    = 0.80f;
        [SerializeField] private float titleRevealStart     = 1.40f;
        [SerializeField] private float titleFadeDuration    = 1.30f;
        [SerializeField] private float taglineStartDelay    = 0.55f;
        [SerializeField] private float wordInterval         = 0.22f;
        [SerializeField] private float buttonRevealDelay    = 0.75f;
        [SerializeField] private float buttonSlideDuration  = 0.70f;
        [SerializeField] private float buttonSlideDistance  = 42f;

        [Header("Parallax")]
        [Tooltip("How far below (px) bgBottom starts before panning up")]
        [SerializeField] private float bgBottomStartOffset  = 300f;
        [Tooltip("Continuous upward drift speed after pan-in (px/sec) — bgBottom")]
        [SerializeField] private float bgTopStartOffset = 300f;
        [SerializeField] private float bgBottomDriftSpeed   = 6f;
        [Tooltip("Continuous upward drift speed after pan-in (px/sec) — bgTop (slower)")]
        [SerializeField] private float bgTopDriftSpeed      = 2.5f;
        [Tooltip("Max upward distance before both layers wrap/stop drifting")]
        [SerializeField] private float bgDriftMaxOffset     = 120f;

        [Header("Breathing Loop")]
        [SerializeField] private float breatheAmplitude    = 0.018f;
        [SerializeField] private float breathePeriod       = 3.60f;

        [Header("Glow Pulse")]
        [SerializeField] private float glowMin             = 0.12f;
        [SerializeField] private float glowMax             = 0.48f;
        [SerializeField] private float glowPeriod          = 2.10f;

        private Vector2 buttonGroupShown;
        private Vector2 buttonGroupHidden;
        private RectTransform buttonGroupRect;
        private RectTransform bgBottomRect;
        private RectTransform bgTopRect;
        private Vector2 bgBottomOrigin;   // resting position after pan-in
        private Vector2 bgTopOrigin;      // resting position of sky
        private string[] taglineWords;

        void Awake()
        {
            CacheRects();
            HideAll();
            CacheButtonPositions();
            CacheTaglineWords();
        }

        void Start()
        {
            StartCoroutine(OpeningSequence());
        }

        // sequence
        IEnumerator OpeningSequence()
        {
            // 1 — Sky (bgTop) fades in first at its resting position — bottom of image visible
            StartCoroutine(FadeIn(bgTop,    bgTopFadeDuration, 1.00f));
            StartCoroutine(FadeIn(moonGlow, bgTopFadeDuration * 0.6f, 0.12f));
            yield return Wait(bgBottomPanDelay);

            // 2 — Ground (bgBottom) pans up from below into view
            StartCoroutine(PanBgTopIn());
            yield return PanBgBottomIn();

            // 3 — Ambient fireflies drift up
            yield return Wait(firstFireflyDelay);
            ambientFireflies?.StartEmitting();

            // 4 — Title materialises
            yield return Wait(titleRevealStart);
            StartCoroutine(RevealTitle());
            yield return Wait(titleFadeDuration * 0.5f);
            focalFireflies?.TriggerBurst();
            yield return Wait(titleFadeDuration * 0.5f);

            // 5 — Tagline word by word
            yield return Wait(taglineStartDelay);
            yield return RevealTagline();

            // 6 — Buttons slide up
            yield return Wait(buttonRevealDelay);
            yield return RevealButtons();

            // 7 — Perpetual loops
            StartCoroutine(TitleBreathe());
            StartCoroutine(BeginGlowPulse());
            StartCoroutine(ParallaxDrift());
        }

        IEnumerator PanBgBottomIn()
        {
            if (bgBottomRect == null) yield break;

            Vector2 startPos = bgBottomOrigin - new Vector2(0f, bgBottomStartOffset);
            bgBottomRect.anchoredPosition = startPos;
            SetAlpha(bgBottom, 1f); // fully visible, just offscreen below

            float elapsed = 0f;
            while (elapsed < bgBottomPanDuration)
            {
                elapsed += Time.deltaTime;
                float t = Mathf.SmoothStep(0f, 1f, Mathf.Clamp01(elapsed / bgBottomPanDuration));
                bgBottomRect.anchoredPosition = Vector2.Lerp(startPos, bgBottomOrigin, t);
                yield return null;
            }
            bgBottomRect.anchoredPosition = bgBottomOrigin;
        }

        IEnumerator PanBgTopIn()
        {
            if (bgTopRect == null) yield break;

            Vector2 startPos = bgTopOrigin + new Vector2(0f, bgTopStartOffset); // starts above
            bgTopRect.anchoredPosition = startPos;
            SetAlpha(bgTop, 1f);

            float elapsed = 0f;
            while (elapsed < bgTopFadeDuration)
            {
                elapsed += Time.deltaTime;
                float t = Mathf.SmoothStep(0f, 1f, Mathf.Clamp01(elapsed / bgTopFadeDuration));
                bgTopRect.anchoredPosition = Vector2.Lerp(startPos, bgTopOrigin, t);
                yield return null;
            }
            bgTopRect.anchoredPosition = bgTopOrigin;
        }

        IEnumerator ParallaxDrift()
        {
            float bottomOffset = 0f;
            float topOffset    = 0f;

            while (true)
            {
                bottomOffset = Mathf.Min(bottomOffset + bgBottomDriftSpeed * Time.deltaTime, bgDriftMaxOffset);
                topOffset    = Mathf.Min(topOffset    + bgTopDriftSpeed    * Time.deltaTime, bgDriftMaxOffset * 0.5f);

                if (bgBottomRect != null)
                    bgBottomRect.anchoredPosition = bgBottomOrigin + new Vector2(0f, bottomOffset);
                if (bgTopRect != null)
                    bgTopRect.anchoredPosition    = bgTopOrigin    - new Vector2(0f, topOffset);

                yield return null;
            }
        }

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

            taglineText.text  = "";
            Color textColor   = taglineText.color;
            taglineText.color = new Color(textColor.r, textColor.g, textColor.b, 1f);

            string built = "";
            foreach (string word in taglineWords)
            {
                built += (built.Length > 0 ? " " : "") + word;
                taglineText.text = built;
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
            if (img == null) return;
            Color c = img.color;
            img.color = new Color(c.r, c.g, c.b, alpha);
        }

        void CacheRects()
        {
            if (bgBottom != null)
            {
                bgBottomRect   = bgBottom.GetComponent<RectTransform>();
                bgBottomOrigin = bgBottomRect.anchoredPosition;
            }
            if (bgTop != null)
            {
                bgTopRect   = bgTop.GetComponent<RectTransform>();
                bgTopOrigin = bgTopRect.anchoredPosition;
            }
        }

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