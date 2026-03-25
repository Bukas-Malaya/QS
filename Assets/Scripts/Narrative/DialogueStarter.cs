using UnityEngine;
using WhereFirefliesReturn.Narrative;
 
namespace WhereFirefliesReturn.Narrative
{
    public class DialogueStarter : MonoBehaviour
    {
        [Header("Dialogue Settings")]
        [Tooltip("Check to start dialogue on Awake")]
        [SerializeField] private bool startOnAwake = false;
        [SerializeField] private float delayBeforeStart = 0f;
        
        [Tooltip("Dialogue lines to play")]
        [SerializeField] private DialogueLine[] dialogueLines;

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

        private void Update()
        {
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