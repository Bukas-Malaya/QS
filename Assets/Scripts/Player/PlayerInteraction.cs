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

        [SerializeField] private Camera _cam;
        [SerializeField] private ResourceNode _currentNode;

        void Awake()
        {
            _cam = Camera.main;
        }

        void Update()
        {
            ScanForNode();

            if (_currentNode != null && Input.GetKeyDown(KeyCode.E))
            {
                _currentNode.Collect();
                HidePrompt();
                _currentNode = null;
            }
        }

        void ScanForNode()
        {
            // Use overlap sphere to detect nearby resources
            Collider[] hits = Physics.OverlapSphere(transform.position, interactRange, interactLayer);

            ResourceNode closestNode = null;
            float closestDistance = Mathf.Infinity;

            foreach (var hit in hits)
            {
                ResourceNode node = hit.GetComponent<ResourceNode>();
                if (node != null && !node.IsCollected)
                {
                    float distance = Vector3.Distance(transform.position, hit.transform.position);
                    if (distance < closestDistance)
                    {
                        closestDistance = distance;
                        closestNode = node;
                    }
                }
            }

            if (closestNode != null)
            {
                _currentNode = closestNode;
                ShowPrompt(_currentNode.PromptText);
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
            if (promptLabel != null) promptLabel.text = "";
        }
    }
}
