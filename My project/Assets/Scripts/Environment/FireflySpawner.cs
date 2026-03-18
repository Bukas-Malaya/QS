using UnityEngine;

namespace WhereFirefliesReturn.Environment
{
    /// <summary>
    /// Spawns glowing firefly orbs that float and drift around the scene.
    /// Add to the Systems GameObject — no setup needed.
    /// </summary>
    public class FireflySpawner : MonoBehaviour
    {
        [Header("Spawn Settings")]
        [SerializeField] int count = 12;
        [SerializeField] float spawnRadius = 8f;
        [SerializeField] float minHeight = 0.4f;
        [SerializeField] float maxHeight = 3.5f;

        [Header("Movement")]
        [SerializeField] float driftSpeed = 0.6f;
        [SerializeField] float bobAmplitude = 0.4f;
        [SerializeField] float bobFrequency = 1.1f;

        [Header("Appearance")]
        [SerializeField] Color glowColor = new Color(0.6f, 1f, 0.3f);
        [SerializeField] float glowIntensity = 2.2f;
        [SerializeField] float orbSize = 0.08f;

        GameObject[] _fireflies;
        Vector3[] _origins;
        float[] _phaseOffsets;
        Vector3[] _driftDirs;

        void Start()
        {
            _fireflies   = new GameObject[count];
            _origins     = new Vector3[count];
            _phaseOffsets = new float[count];
            _driftDirs   = new Vector3[count];

            for (int i = 0; i < count; i++)
            {
                // Random position in a circle around the camera focus
                float angle = Random.Range(0f, 360f) * Mathf.Deg2Rad;
                float dist  = Random.Range(1.5f, spawnRadius);
                float y     = Random.Range(minHeight, maxHeight);
                Vector3 pos = new Vector3(Mathf.Cos(angle) * dist, y, Mathf.Sin(angle) * dist);

                _origins[i]      = pos;
                _phaseOffsets[i] = Random.Range(0f, Mathf.PI * 2f);
                _driftDirs[i]    = new Vector3(Random.Range(-1f, 1f), 0f, Random.Range(-1f, 1f)).normalized;

                _fireflies[i] = CreateOrb(pos, i);
            }
        }

        void Update()
        {
            float t = Time.time;

            for (int i = 0; i < count; i++)
            {
                if (_fireflies[i] == null) continue;

                float phase = _phaseOffsets[i];

                // Gentle drift in XZ
                Vector3 drift = _driftDirs[i] * Mathf.Sin(t * driftSpeed + phase) * 1.2f;

                // Bob up/down
                float bob = Mathf.Sin(t * bobFrequency + phase) * bobAmplitude;

                Vector3 target = _origins[i] + drift + Vector3.up * bob;
                _fireflies[i].transform.position = Vector3.Lerp(
                    _fireflies[i].transform.position, target, Time.deltaTime * 2f);

                // Pulse the emission brightness
                float pulse = 0.7f + 0.3f * Mathf.Sin(t * 2.5f + phase);
                var rend = _fireflies[i].GetComponent<Renderer>();
                if (rend != null)
                    rend.material.SetColor("_EmissionColor", glowColor * (glowIntensity * pulse));
            }
        }

        GameObject CreateOrb(Vector3 position, int index)
        {
            var go = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            go.name = $"Firefly_{index}";
            go.transform.position = position;
            go.transform.localScale = Vector3.one * orbSize;

            // Remove collider — fireflies are purely visual
            Destroy(go.GetComponent<Collider>());

            // Emissive material
            var mat = new Material(Shader.Find("Universal Render Pipeline/Lit"));
            if (mat.shader.name == "Hidden/InternalErrorShader")
                mat = new Material(Shader.Find("Standard")); // fallback

            mat.color = new Color(glowColor.r, glowColor.g, glowColor.b, 1f);
            mat.EnableKeyword("_EMISSION");
            mat.SetColor("_EmissionColor", glowColor * glowIntensity);
            go.GetComponent<Renderer>().material = mat;

            return go;
        }
    }
}
