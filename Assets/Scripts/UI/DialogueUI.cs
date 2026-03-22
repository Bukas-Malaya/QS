using UnityEngine;
using UnityEngine.UI;
using TMPro;
using WhereFirefliesReturn.Narrative;

namespace WhereFirefliesReturn.UI
{
    public class DialogueUI : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] private GameObject dialoguePanel;
        [SerializeField] private TMP_Text speakerNameText;
        [SerializeField] private TMP_Text dialogueText;
        
        private bool isSubscribed = false;

        private void Awake()
        {
            // Hide UI immediately
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
                dialoguePanel.SetActive(true);
        }

        private void Hide()
        {
            if (dialoguePanel != null)
                dialoguePanel.SetActive(false);
        }
    }
}