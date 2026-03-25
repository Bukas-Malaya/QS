using UnityEngine;
using WhereFirefliesReturn.Narrative;

namespace WhereFirefliesReturn.Narrative
{
    public class DialogueTrigger : MonoBehaviour
    {
        [Header("Dialogue Settings")]
        [Tooltip("Check to start dialogue on Awake")]
        [SerializeField] private bool startOnAwake = false;
        [SerializeField] private float delayBeforeStart = 0f;
        
        [Tooltip("Dialogue lines to play")]
        [SerializeField] private DialogueLine[] dialogueLines;
        [Tooltip("Should the dialogue only trigger once?")]
        [SerializeField] private bool triggerOnlyOnce = false;
        private bool hasTriggered = false;
        private void Awake()
        {
            if (startOnAwake && dialogueLines != null && dialogueLines.Length > 0)
            {
                PlaySampleDialogue();
            }
        }

        private void Start()
        {
            if (startOnAwake && delayBeforeStart > 0f)
            {
                Invoke(nameof(PlayDialogue), delayBeforeStart);
            }
        }

        private void OnTriggerEnter(Collider other)
        {
            // Check if the colliding object is the player (assuming player has tag "Player")
            if (other.CompareTag("Player") && !hasTriggered){
                PlayDialogue();
                if (triggerOnlyOnce){
                    hasTriggered = true;
                }
            }
        }

        public void PlaySampleDialogue()
        {
            if (dialogueLines == null || dialogueLines.Length == 0)
            {
                return;
            }
            
            DialogueManager.Instance?.PlayDialogue(dialogueLines);
        }

        public void PlayDialogue()
        {
            DialogueManager.Instance?.PlayDialogue(dialogueLines);
        }
    }
}