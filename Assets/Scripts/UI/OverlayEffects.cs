using UnityEngine;
using UnityEngine.UI;
using TMPro;
public class OverlayEffects : MonoBehaviour
{
    // This class is a placeholder for any screen overlay effects (e.g. fade in/out, damage flash, etc.)
    [SerializeField] private Image overlayImage;
    [SerializeField] private float fadeDuration = 1f;
    void Start() {
        
        StartCoroutine(FadeIn(fadeDuration));
    }
    private System.Collections.IEnumerator FadeIn(float duration = 1f) {
        overlayImage.color = new Color(0, 0, 0, 1);
        float elapsed = 0f;
        while (elapsed < duration) {
            elapsed += Time.deltaTime;
            float alpha = Mathf.Lerp(1, 0, elapsed / duration);
            overlayImage.color = new Color(0, 0, 0, alpha);
            yield return null;
        }
        overlayImage.color = new Color(0, 0, 0, 0);
        
    }

    private System.Collections.IEnumerator FadeOut(float duration, System.Action onComplete)
    {
        overlayImage.color = new Color(0, 0, 0, 0);
        float elapsed = 0f;
        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            overlayImage.color = new Color(0, 0, 0, Mathf.Lerp(0, 1, elapsed / duration));
            yield return null;
        }
        overlayImage.color = new Color(0, 0, 0, 1);
        onComplete?.Invoke(); // fires when fully black
    }

    public void TriggerFadeIn()
    {
        StartCoroutine(FadeIn(fadeDuration));
    }

    public void TriggerFadeOut(System.Action onComplete = null)
    {
        StartCoroutine(FadeOut(fadeDuration, onComplete));
    }

}
