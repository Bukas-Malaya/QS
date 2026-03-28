using System.Collections;
using UnityEngine;
using UnityEngine.Events;
using WhereFirefliesReturn.Environment;

namespace WhereFirefliesReturn.Resources
{
    /// <summary>
    /// Tracks all farm_beds in the first field section.
    /// When every bed is correctly companion-planted, triggers the dialogue/cutscene.
    ///
    /// Setup:
    ///   - Assign all farm_bed objects in the 'beds' array
    ///   - Hook OnPuzzleSolved UnityEvent to your DialogueStarter or cutscene trigger
    ///   - Optionally assign a solvedVFX particle system for a field-wide celebration burst
    /// </summary>
    public class FieldPuzzleManager : MonoBehaviour
    {
        public static FieldPuzzleManager Instance { get; private set; }

        [Header("Puzzle Beds")]
        [Tooltip("All farm_bed objects in this section of the field.")]
        [SerializeField] private farm_bed[] beds;

        [Header("Completion")]
        [SerializeField] private float completionCheckDelay = 0.5f; // slight delay for particles to settle
        [SerializeField] private ParticleSystem solvedVFX;          // optional celebration burst
        [SerializeField] private AudioClip solvedSound;
        private AudioSource audioSource;

        [Header("Events")]
        public UnityEvent OnPuzzleSolved; // wire this to DialogueStarter.TriggerDialogue() in Inspector

        [Header("Particles")]
        [SerializeField] private ParticleSystem FireFlyParticles;

        private bool isSolved = false;

        void Awake()
        {
            if (Instance != null && Instance != this) { Destroy(gameObject); return; }
            Instance = this;
            audioSource = GetComponent<AudioSource>();
        }

        void Start()
        {
            if (FireFlyParticles != null)
                FireFlyParticles.Stop();
        }

        /// <summary>Called by farm_bed whenever a crop is planted.</summary>
        public void OnBedPlanted()
        {
            if (isSolved) return;
            StartCoroutine(CheckCompletionDelayed());
        }

        IEnumerator CheckCompletionDelayed()
        {
            Debug.Log("[FieldPuzzleManager] Checking puzzle completion...");
            //check if 80% of the beds are planted before doing the more expensive companion check
             int plantedCount = 0;
            yield return new WaitForSeconds(completionCheckDelay);

            // All beds must be planted
            foreach (var bed in beds)
                if (bed == null || !bed.isPlanted) yield break;

            // All beds must be correctly placed
            foreach (var bed in beds)
                if (bed != null && bed.IsCorrectlyPlaced())
                {
                    plantedCount++;
                    
                }
            if (plantedCount < beds.Length * 0.7f)
                yield return isSolved = false;
            // ✓ Puzzle solved!
            isSolved = true;
            TriggerSolved();
            EnvironmentMeter.Instance?.Adjust(plantedCount);
        }

        void TriggerSolved()
        {
            Debug.Log("[FieldPuzzleManager] Puzzle solved! The field is restored.");

            // Play celebration VFX
            if (solvedVFX != null)
            {
                solvedVFX.transform.position = GetFieldCenter();
                solvedVFX.Play();
            }

            // Play sound
            if (solvedSound != null && audioSource != null)
                audioSource.PlayOneShot(solvedSound);

            Debug.Log("[FieldPuzzleManager] Triggering OnPuzzleSolved event.");
            // Trigger dialogue/cutscene (wired in Inspector)
            OnPuzzleSolved?.Invoke();
            FireFlyParticles?.Play();
        }

        Vector3 GetFieldCenter()
        {
            if (beds.Length == 0) return transform.position;
            Vector3 sum = Vector3.zero;
            foreach (var b in beds) if (b != null) sum += b.transform.position;
            return sum / beds.Length;
        }

        // ─── Editor helper: visualize neighbor connections ───────────────
        #if UNITY_EDITOR
        void OnDrawGizmosSelected()
        {
            if (beds == null) return;
            Gizmos.color = Color.green;
            foreach (var bed in beds)
                if (bed != null)
                    Gizmos.DrawWireCube(bed.transform.position, Vector3.one * 0.9f);
        }
        #endif
    }
}
