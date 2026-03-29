using UnityEngine;
using UnityEngine.Events;
using System.Collections;

namespace WhereFirefliesReturn.Narrative
{
    
    [System.Serializable]
    public class DialogueLine // Represents a single line of dialogue
    {
        public string speakerName;
        [TextArea(2, 5)] public string text;
    }

    public class DialogueManager : MonoBehaviour
    {
        public static DialogueManager Instance { get; private set; }

        [Header("Settings")]
        [SerializeField] private float charDelay = 0.03f;

        public bool IsPlaying { get; private set; }

        public bool canInteract = true;

        public UnityEvent<DialogueLine> OnLineStarted;
        public UnityEvent<string> OnCharacterTyped;
        public UnityEvent OnDialogueComplete;

        void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }
            Instance = this;
        }

        public void PlayDialogue(DialogueLine[] lines)
        {
            if (IsPlaying) return;
            StartCoroutine(RunDialogue(lines));
        }

        private IEnumerator RunDialogue(DialogueLine[] lines)
        {
            IsPlaying = true;

            foreach (var line in lines) {
                OnLineStarted?.Invoke(line);
                
                foreach (char c in line.text) {
                    OnCharacterTyped?.Invoke(c.ToString());
                    
                    //if (Input.GetKeyDown(KeyCode.Space) || Input.GetMouseButtonDown(0)) {
                    //    string remainingText = new string(line.text.Substring(line.text.IndexOf(c) + 1));
                    //    if (!string.IsNullOrEmpty(remainingText)) {
                    //        OnCharacterTyped?.Invoke(remainingText);
                    //    }
                    //    break;
                    //}
                    
                    yield return new WaitForSeconds(charDelay);
                }
                
                yield return new WaitUntil(() => canInteract && (Input.GetKeyDown(KeyCode.Space) || Input.GetMouseButtonDown(0)));
            }

            IsPlaying = false;
            OnDialogueComplete?.Invoke();
        }
    }
}
