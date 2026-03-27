using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace WhereFirefliesReturn.UI
{
    /// <summary>
    /// Attach to any UI Button for hover scale + colour tint and a click punch.
    /// Works with Screen Space - Camera and Screen Space - Overlay canvases.
    /// </summary>
    [RequireComponent(typeof(RectTransform))]
    public class ButtonHoverEffect : MonoBehaviour,
        IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler
    {
        [Header("Hover")]
        [SerializeField] private float hoverScale      = 1.06f;
        [SerializeField] private float hoverDuration   = 0.12f;
        [SerializeField] private Color hoverTint       = new Color(1f, 0.92f, 0.70f, 1f); // warm gold

        [Header("Click Punch")]
        [SerializeField] private float clickScale      = 0.92f;
        [SerializeField] private float clickDuration   = 0.08f;

        private RectTransform rt;
        private Image img;
        private Color normalColor;
        private Vector3 normalScale;
        private Coroutine activeRoutine;

        void Awake()
        {
            rt          = GetComponent<RectTransform>();
            img         = GetComponent<Image>();
            normalScale = rt.localScale;
            normalColor = img != null ? img.color : Color.white;
        }

        // ── Pointer events ─────────────────────────────────────────────────────
        public void OnPointerEnter(PointerEventData _) => Animate(hoverScale, hoverDuration, hoverTint);
        public void OnPointerExit(PointerEventData _)  => Animate(1f, hoverDuration, normalColor);

        public void OnPointerClick(PointerEventData _)
        {
            StopActive();
            activeRoutine = StartCoroutine(ClickPunch());
        }

        // ── Coroutines ─────────────────────────────────────────────────────────
        void Animate(float targetScale, float duration, Color targetColor)
        {
            StopActive();
            activeRoutine = StartCoroutine(TweenTo(targetScale, duration, targetColor));
        }

        IEnumerator TweenTo(float targetScale, float duration, Color targetColor)
        {
            float elapsed       = 0f;
            Vector3 startScale  = rt.localScale;
            Color startColor    = img != null ? img.color : Color.white;
            Vector3 endScale    = normalScale * targetScale;

            while (elapsed < duration)
            {
                elapsed += Time.unscaledDeltaTime;
                float t  = Mathf.SmoothStep(0f, 1f, elapsed / duration);
                rt.localScale = Vector3.Lerp(startScale, endScale, t);
                if (img != null) img.color = Color.Lerp(startColor, targetColor, t);
                yield return null;
            }

            rt.localScale = endScale;
            if (img != null) img.color = targetColor;
        }

        IEnumerator ClickPunch()
        {
            // Shrink
            Vector3 start = rt.localScale;
            Vector3 small = normalScale * clickScale;
            float elapsed = 0f;

            while (elapsed < clickDuration)
            {
                elapsed += Time.unscaledDeltaTime;
                rt.localScale = Vector3.Lerp(start, small, elapsed / clickDuration);
                yield return null;
            }

            // Bounce back
            elapsed = 0f;
            while (elapsed < clickDuration)
            {
                elapsed += Time.unscaledDeltaTime;
                rt.localScale = Vector3.Lerp(small, normalScale, elapsed / clickDuration);
                yield return null;
            }

            rt.localScale = normalScale;
        }

        void StopActive()
        {
            if (activeRoutine != null)
            {
                StopCoroutine(activeRoutine);
                activeRoutine = null;
            }
        }
    }
}
