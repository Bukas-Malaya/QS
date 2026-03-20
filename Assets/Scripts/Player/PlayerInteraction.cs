using UnityEngine;
using TMPro;
using WhereFirefliesReturn.Resources;

namespace WhereFirefliesReturn.Player
{
    public class PlayerInteraction : MonoBehaviour
    {
        [Header("Raycast")]
        [SerializeField] private float interactRange = 3f;
        [SerializeField] private LayerMask interactLayer;

        [Header("UI")]
        [SerializeField] private GameObject promptPanel;
        [SerializeField] private TextMeshProUGUI promptLabel;

        private Camera _cam;
        private ResourceNode _currentNode;

        void Awake()
        {
            _cam = Camera.main;
        }

        void Update()
        {
            ScanForNode();

            if (_currentNode != null && Input.GetKeyDown(KeyCode.E))
                _currentNode.Collect();
        }

        void ScanForNode()
        {
            Ray ray = new Ray(_cam.transform.position, _cam.transform.forward);
            bool hit = Physics.Raycast(ray, out RaycastHit info, interactRange, interactLayer);

            ResourceNode node = hit ? info.collider.GetComponent<ResourceNode>() : null;

            if (node != null && !node.IsCollected)
            {
                _currentNode = node;
                ShowPrompt(node.PromptText);
            }
            else
            {
                _currentNode = null;
                HidePrompt();
            }
        }

        void ShowPrompt(string text)
        {
            if (promptPanel != null) promptPanel.SetActive(true);
            if (promptLabel != null) promptLabel.text = text;
        }

        void HidePrompt()
        {
            if (promptPanel != null) promptPanel.SetActive(false);
        }
    }
}
