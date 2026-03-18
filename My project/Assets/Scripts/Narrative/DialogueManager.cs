using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using TMPro;

namespace WhereFirefliesReturn.Narrative
{
    [System.Serializable]
    public class DialogueLine
    {
        public string speakerName;
        [TextArea(2, 5)] public string text;
    }

    public class DialogueManager : MonoBehaviour
    {
        public static DialogueManager Instance { get; private set; }

        [Header("UI References")]
        [SerializeField] GameObject dialoguePanel;
        [SerializeField] TMP_Text speakerText;
        [SerializeField] TMP_Text bodyText;

        [Header("Typewriter")]
        [SerializeField] float charDelay = 0.03f;

        DialogueLine[] _lines;
        int _index;
        bool _isTyping;
        bool _isActive;

        public event System.Action OnDialogueEnd;

        void Awake()
        {
            if (Instance != null && Instance != this) { Destroy(gameObject); return; }
            Instance = this;
            dialoguePanel?.SetActive(false);
        }

        void Update()
        {
            if (!_isActive) return;
            if (Input.GetKeyDown(KeyCode.Space) || Input.GetMouseButtonDown(0))
                Advance();
        }

        public void StartDialogue(DialogueLine[] lines)
        {
            _lines = lines;
            _index = 0;
            _isActive = true;
            dialoguePanel?.SetActive(true);
            ShowLine(_lines[_index]);
        }

        void Advance()
        {
            if (_isTyping) { SkipTypewriter(); return; }

            _index++;
            if (_index >= _lines.Length) { EndDialogue(); return; }

            ShowLine(_lines[_index]);
        }

        void ShowLine(DialogueLine line)
        {
            StopAllCoroutines();
            if (speakerText) speakerText.text = line.speakerName;
            StartCoroutine(Typewrite(line.text));
        }

        IEnumerator Typewrite(string text)
        {
            _isTyping = true;
            bodyText.text = "";
            foreach (char c in text)
            {
                bodyText.text += c;
                yield return new WaitForSeconds(charDelay);
            }
            _isTyping = false;
        }

        void SkipTypewriter()
        {
            StopAllCoroutines();
            _isTyping = false;
            if (bodyText) bodyText.text = _lines[_index].text;
        }

        void EndDialogue()
        {
            _isActive = false;
            dialoguePanel?.SetActive(false);
            OnDialogueEnd?.Invoke();
        }
    }
}
