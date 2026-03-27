using UnityEngine;
using UnityEngine.UI;
using TMPro;
using WhereFirefliesReturn.Narrative;
using System.Collections;

namespace WhereFirefliesReturn.UI
{
    public class DialogueUI : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] private GameObject dialoguePanel;
        [SerializeField] private TMP_Text speakerNameText;
        [SerializeField] private TMP_Text dialogueText;
        
        [Header("Animation Settings")]
        [SerializeField] private float tweenDuration = 0.3f;
        [SerializeField] private float panDistance = 50f; // How far to pan up/down
         
        private bool isSubscribed = false;
        private RectTransform panelRectTransform;
        private Vector2 originalPosition;
        private Coroutine currentTween;

        private void Awake()
        {
            // Cache the RectTransform for tweening
            if (dialoguePanel != null)
            {
                panelRectTransform = dialoguePanel.GetComponent<RectTransform>();
                if (panelRectTransform != null)
                {
                    originalPosition = panelRectTransform.anchoredPosition;
                }
            }
            
            // Hide UI immediately (start positioned off-screen)
            Hide();
            
            // Try to subscribe if DialogueManager is already initialized
            TrySubscribe();
        }

        private void Start()
        {
            // Try to subscribe again in case DialogueManager initialized after Awake
            TrySubscribe();
        }

        private void OnDestroy()
        {
            // Unsubscribe from dialogue events if we were subscribed
            if (isSubscribed && DialogueManager.Instance != null)
            {
                DialogueManager.Instance.OnLineStarted.RemoveListener(HandleLineStarted);
                DialogueManager.Instance.OnCharacterTyped.RemoveListener(HandleCharacterTyped);
                DialogueManager.Instance.OnDialogueComplete.RemoveListener(HandleDialogueComplete);
                isSubscribed = false;
            }
        }

        private void TrySubscribe()
        {
            if (isSubscribed)
                return;
                
            // Check if DialogueManager instance exists
            if (DialogueManager.Instance != null)
            {
                // Subscribe to dialogue events
                DialogueManager.Instance.OnLineStarted.AddListener(HandleLineStarted);
                DialogueManager.Instance.OnCharacterTyped.AddListener(HandleCharacterTyped);
                DialogueManager.Instance.OnDialogueComplete.AddListener(HandleDialogueComplete);
                isSubscribed = true;
                Debug.Log("DialogueUI: Successfully subscribed to dialogue events");
            }
            else
            {
                // Try to find DialogueManager in the scene
                DialogueManager manager = FindObjectOfType<DialogueManager>();
                if (manager != null)
                {
                    // Subscribe to dialogue events
                    manager.OnLineStarted.AddListener(HandleLineStarted);
                    manager.OnCharacterTyped.AddListener(HandleCharacterTyped);
                    manager.OnDialogueComplete.AddListener(HandleDialogueComplete);
                    isSubscribed = true;
                    Debug.Log("DialogueUI: Found and subscribed to DialogueManager via FindObjectOfType");
                }
                else
                {
                    Debug.LogWarning("DialogueUI: DialogueManager instance not found, will retry in Start()");
                }
            }
        }

        private void HandleLineStarted(DialogueLine line)
        {
            Debug.Log($"DialogueUI: New line started - Speaker: {line.speakerName}, Text: {line.text}");
            if (speakerNameText != null)
                speakerNameText.text = line.speakerName;
            if (dialogueText != null)
                dialogueText.text = "";
            Show();
        }

        private void HandleCharacterTyped(string character)
        {
            if (dialogueText != null)
                dialogueText.text += character;
        }

        private void HandleDialogueComplete()
        {
            Hide();
        }

        private void Show()
{
    if (dialoguePanel != null)
    {
        dialoguePanel.SetActive(true);
        // Start panning up tween: from below to original position
        if (currentTween != null)
            StopCoroutine(currentTween);
        currentTween = StartCoroutine(TweenPanel(originalPosition - new Vector2(0, panDistance), originalPosition));
    }
}

private void Hide()
{
    if (dialoguePanel != null)
    {
        // Start panning down tween: from original position to below, then hide
        if (currentTween != null)
            StopCoroutine(currentTween);
        currentTween = StartCoroutine(TweenPanel(originalPosition, originalPosition - new Vector2(0, panDistance), () => 
        {
            dialoguePanel.SetActive(false);
        }));
    }
}

private IEnumerator TweenPanel(Vector2 startPos, Vector2 endPos, System.Action onComplete = null)
{
    float elapsedTime = 0f;
    
    while (elapsedTime < tweenDuration)
    {
        elapsedTime += Time.deltaTime;
        float t = Mathf.Clamp01(elapsedTime / tweenDuration);
        // Optional: use easing function for smoother animation
        t = t * t * (3f - 2f * t); // Smoothstep easing
        
        if (panelRectTransform != null)
        {
            panelRectTransform.anchoredPosition = Vector2.Lerp(startPos, endPos, t);
        }
        
        yield return null;
    }
    
    // Ensure we end at the exact target position
    if (panelRectTransform != null)
    {
        panelRectTransform.anchoredPosition = endPos;
    }
    
    onComplete?.Invoke();
    currentTween = null;
}

private IEnumerator TweenPanel(Vector2 startPos, Vector2 endPos)
{
    return TweenPanel(startPos, endPos, null);
}
    }
}