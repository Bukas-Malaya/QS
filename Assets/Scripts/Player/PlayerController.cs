using UnityEngine;

namespace WhereFirefliesReturn.Player
{
    [RequireComponent(typeof(CharacterController))]
    public class PlayerController : MonoBehaviour
    {
        [Header("Movement")]
        [SerializeField] private float moveSpeed = 8f;
        [SerializeField] private float rotationSpeed = 10f;

        [Header("Jump")]
        [SerializeField] private float jumpForce = 4f;
        [SerializeField] private float fallMultiplier = 3f;
        [SerializeField] private float lowJumpMultiplier = 2f;

        [Header("Camera")]
        [SerializeField] private Transform cameraTransform;

        [Header("Effects")]
        [SerializeField] private ParticleSystem trailEffect;

        private CharacterController _controller;
        private Vector3 _velocity;
        private bool _isJumping;
        private float _originalJumpForce;

        public bool IsGrounded => _controller.isGrounded;
        public bool CanMove = true;

        void Awake()
        {
            _controller = GetComponent<CharacterController>();
            if (cameraTransform == null)
                cameraTransform = Camera.main?.transform;
        }

        void Update()
        {
            if (CanMove) {
                if (Input.GetKeyDown(KeyCode.Space)) {
                    Jump();
                }
                if (Input.GetKeyUp(KeyCode.Space) && _velocity.y > 0) {
                    _velocity.y *= 0.5f; // Cut jump short if button released early
                } 
                Move();
            }
            
            
            
            ApplyGravity();
        }

        void Move()
        {
            float h = Input.GetAxisRaw("Horizontal");
            float v = Input.GetAxisRaw("Vertical");

            Vector3 direction = new Vector3(h, 0f, v).normalized;

            if (trailEffect != null)
            {
                if (h != 0 || v != 0 && _controller.isGrounded) {
                    if (!trailEffect.isPlaying) trailEffect.Play();
                }
                else {
                    if (trailEffect.isPlaying) trailEffect.Stop();
                }
            }

            // Move relative to camera facing direction
            if (cameraTransform != null)
            {
                Vector3 camForward = cameraTransform.forward;
                Vector3 camRight = cameraTransform.right;
                camForward.y = 0f;
                camRight.y = 0f;
                direction = (camForward * v + camRight * h).normalized;
            }

            // Apply movement
            _controller.Move(direction * moveSpeed * Time.deltaTime);

            // Rotate player to face movement direction (only when moving)
            if (h != 0 || v != 0)
            {
                Quaternion targetRotation = Quaternion.LookRotation(direction);
                transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
            }
        }

        void ApplyGravity()
        {
            if (_controller.isGrounded)
            {
                // Only apply ground stickiness if we're not jumping
                if (!(_isJumping && _velocity.y > 0))
                {
                    _velocity.y = -2f;
                }
                _isJumping = false;
            }
            else {
                if (trailEffect != null && _isJumping) {
                    if (trailEffect.isPlaying) trailEffect.Stop();
                }
                if (_velocity.y < 0)
                {
                    // Falling - apply stronger gravity
                    _velocity.y += Physics.gravity.y * (fallMultiplier - 1) * Time.deltaTime;
                }
                else if (_velocity.y > 0 && !Input.GetKey(KeyCode.Space)) {
                    // Ascending but jump button released - apply reduced gravity for lower jump
                    _velocity.y += Physics.gravity.y * (lowJumpMultiplier - 1) * Time.deltaTime;
                }
                
                // Always apply base gravity
                _velocity.y += Physics.gravity.y * Time.deltaTime;
            }
            
            _controller.Move(_velocity * (Time.deltaTime * 2));
        }

        void Jump() {
            if (_controller.isGrounded) {
                _velocity.y = jumpForce;
                _isJumping = true;
            }
        }

        public void PlayerStartedDialogue(){
            CanMove = false;
            _velocity = Vector3.zero;
            if (trailEffect != null && trailEffect.isPlaying) trailEffect.Stop();
        }

        public void PlayerEndedDialogue(){
            CanMove = true;
        }
    }
}
