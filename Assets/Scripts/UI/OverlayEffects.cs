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

    void Update()
    {
        
    }
}
