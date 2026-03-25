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

        public UnityEvent<DialogueLine> OnLineStarted;
        public UnityEvent<string> OnCharacterTyped;
        public UnityEvent OnDialogueComplete;

        private bool _skipRequested = false;

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
            _skipRequested = false;
            StartCoroutine(RunDialogue(lines));
        }

        private IEnumerator RunDialogue(DialogueLine[] lines)
        {
            IsPlaying = true;

            foreach (var line in lines) {
                if (_skipRequested) break;
                
                OnLineStarted?.Invoke(line);
                foreach (char c in line.text) {
                    if (_skipRequested) {
                        break;
                    }
                    OnCharacterTyped?.Invoke(c.ToString());
                    yield return new WaitForSeconds(charDelay);
                }
                
                if (_skipRequested) break;
                
                yield return new WaitUntil(() => {
                    if (Input.GetKeyDown(KeyCode.Space) || Input.GetMouseButtonDown(0)) {
                        _skipRequested = true;
                        return true;
                    }
                    return false;
                });
            }

            IsPlaying = false;
            OnDialogueComplete?.Invoke();
        }
    }
}
