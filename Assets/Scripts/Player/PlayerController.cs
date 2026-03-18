using UnityEngine;

namespace WhereFirefliesReturn.Player
{
    [RequireComponent(typeof(CharacterController))]
    public class PlayerController : MonoBehaviour
    {
        [Header("Movement")]
        [SerializeField] private float moveSpeed = 5f;
        [SerializeField] private float rotationSpeed = 10f;

        [Header("Camera")]
        [SerializeField] private Transform cameraTransform;

        private CharacterController _controller;
        private Vector3 _velocity;
        private const float Gravity = -9.81f;

        void Awake()
        {
            _controller = GetComponent<CharacterController>();
            if (cameraTransform == null)
                cameraTransform = Camera.main?.transform;
        }

        void Update()
        {
            Move();
            ApplyGravity();
        }

        void Move()
        {
            float h = Input.GetAxisRaw("Horizontal");
            float v = Input.GetAxisRaw("Vertical");

            if (h == 0 && v == 0) return;

            Vector3 direction = new Vector3(h, 0f, v).normalized;

            // Move relative to camera facing direction
            if (cameraTransform != null)
            {
                Vector3 camForward = cameraTransform.forward;
                Vector3 camRight = cameraTransform.right;
                camForward.y = 0f;
                camRight.y = 0f;
                direction = (camForward * v + camRight * h).normalized;
            }

            _controller.Move(direction * moveSpeed * Time.deltaTime);

            // Rotate player to face movement direction
            Quaternion targetRotation = Quaternion.LookRotation(direction);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
        }

        void ApplyGravity()
        {
            if (_controller.isGrounded && _velocity.y < 0)
                _velocity.y = -2f;

            _velocity.y += Gravity * Time.deltaTime;
            _controller.Move(_velocity * Time.deltaTime);
        }
    }
}
