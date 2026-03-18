using UnityEngine;

namespace WhereFirefliesReturn.Player
{
    [RequireComponent(typeof(CharacterController))]
    public class PlayerController : MonoBehaviour
    {
        [Header("Movement")]
        [SerializeField] float moveSpeed = 5f;
        [SerializeField] float gravity = -9.81f;
        [SerializeField] float jumpHeight = 1.2f;

        [Header("Camera")]
        [SerializeField] Transform cameraTransform;

        CharacterController _cc;
        Vector3 _velocity;
        bool _isGrounded;

        void Awake()
        {
            _cc = GetComponent<CharacterController>();
            if (cameraTransform == null && Camera.main != null)
                cameraTransform = Camera.main.transform;
        }

        void Update()
        {
            if (Core.GameManager.Instance?.CurrentState != Core.GameState.Playing) return;

            HandleMovement();
            HandleGravity();
        }

        void HandleMovement()
        {
            float h = Input.GetAxisRaw("Horizontal");
            float v = Input.GetAxisRaw("Vertical");

            Vector3 camForward = cameraTransform != null
                ? Vector3.ProjectOnPlane(cameraTransform.forward, Vector3.up).normalized
                : Vector3.forward;
            Vector3 camRight = cameraTransform != null
                ? Vector3.ProjectOnPlane(cameraTransform.right, Vector3.up).normalized
                : Vector3.right;

            Vector3 move = (camForward * v + camRight * h).normalized;
            _cc.Move(move * moveSpeed * Time.deltaTime);

            if (move != Vector3.zero)
                transform.forward = move;
        }

        void HandleGravity()
        {
            _isGrounded = _cc.isGrounded;
            if (_isGrounded && _velocity.y < 0f) _velocity.y = -2f;

            _velocity.y += gravity * Time.deltaTime;
            _cc.Move(_velocity * Time.deltaTime);
        }
    }
}
