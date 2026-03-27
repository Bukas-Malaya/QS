using UnityEngine;

namespace WhereFirefliesReturn.UI
{
    /// <summary>
    /// Configures and drives a ParticleSystem entirely in code.
    /// Two modes: Ambient (background swarm) and Focal (title burst cluster).
    /// Called by MainMenuAnimator at the right moment in the opening sequence.
    /// </summary>
    [RequireComponent(typeof(ParticleSystem))]
    public class FireflyEmitter : MonoBehaviour
    {
        public enum EmitterMode { Ambient, Focal }

        [Header("Mode")]
        [SerializeField] private EmitterMode mode = EmitterMode.Ambient;

        private ParticleSystem ps;

        // ── Colours ────────────────────────────────────────────────────────────
        private static readonly Color ColDim    = new Color(0.84f, 0.96f, 0.48f, 0.00f); // #D4F57A transparent
        private static readonly Color ColBright = new Color(1.00f, 0.99f, 0.66f, 1.00f); // #FFFCA8 opaque

        // ── Unity lifecycle ────────────────────────────────────────────────────
        void Awake()
        {
            ps = GetComponent<ParticleSystem>();
            Configure();
            ps.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);
        }

        // ── Public API ─────────────────────────────────────────────────────────
        public void StartEmitting() => ps?.Play();

        public void TriggerBurst()
        {
            if (ps == null) return;
            int count = mode == EmitterMode.Focal ? 18 : 35;
            ps.Emit(count);
            ps.Play();
        }

        public void StopEmitting() => ps?.Stop(false, ParticleSystemStopBehavior.StopEmitting);

        // ── Configuration ──────────────────────────────────────────────────────
        void Configure()
        {
            bool isAmbient = mode == EmitterMode.Ambient;

            // Main module
            var main = ps.main;
            main.loop                   = true;
            main.playOnAwake            = false;
            main.simulationSpace        = ParticleSystemSimulationSpace.World;
            main.gravityModifier        = new ParticleSystem.MinMaxCurve(-0.015f); // drift upward
            main.maxParticles           = isAmbient ? 90 : 25;
            main.startLifetime          = new ParticleSystem.MinMaxCurve(
                                              isAmbient ? 4f : 2f,
                                              isAmbient ? 7f : 4f);
            main.startSpeed             = new ParticleSystem.MinMaxCurve(0.04f, 0.20f);
            main.startSize              = new ParticleSystem.MinMaxCurve(
                                              isAmbient ? 0.006f : 0.012f,
                                              isAmbient ? 0.016f : 0.028f);
            main.startColor             = new ParticleSystem.MinMaxGradient(
                                              new Color(0.84f, 0.96f, 0.48f, 0.85f),
                                              new Color(1.00f, 0.99f, 0.66f, 1.00f));

            // Emission
            var em = ps.emission;
            em.enabled        = true;
            em.rateOverTime   = isAmbient ? 7f : 2f;

            // Shape — flat rectangle covering screen area
            var sh = ps.shape;
            sh.enabled    = true;
            sh.shapeType  = ParticleSystemShapeType.Rectangle;
            sh.position   = isAmbient ? new Vector3(0f, -1.5f, 0f) : new Vector3(0f, 0.5f, 0f);
            sh.scale      = isAmbient ? new Vector3(20f, 8f, 0f) : new Vector3(7f, 2.5f, 0f);

            // Velocity over lifetime — horizontal sine sway
            var vel = ps.velocityOverLifetime;
            vel.enabled = true;
            vel.space   = ParticleSystemSimulationSpace.Local;
            vel.x = new ParticleSystem.MinMaxCurve(1f, BuildSineCurve(amplitude: 0.09f));
            vel.y = new ParticleSystem.MinMaxCurve(1f, BuildDriftCurve());

            // Color over lifetime — fade in, hold, fade out
            var col = ps.colorOverLifetime;
            col.enabled = true;
            var grad    = new Gradient();
            grad.SetKeys(
                new GradientColorKey[]
                {
                    new GradientColorKey(new Color(0.84f, 0.96f, 0.48f), 0.00f),
                    new GradientColorKey(new Color(1.00f, 0.99f, 0.66f), 0.45f),
                    new GradientColorKey(new Color(1.00f, 0.99f, 0.66f), 0.75f),
                    new GradientColorKey(new Color(0.84f, 0.96f, 0.48f), 1.00f),
                },
                new GradientAlphaKey[]
                {
                    new GradientAlphaKey(0.00f, 0.00f),
                    new GradientAlphaKey(1.00f, 0.18f),
                    new GradientAlphaKey(0.80f, 0.70f),
                    new GradientAlphaKey(0.00f, 1.00f),
                }
            );
            col.color = new ParticleSystem.MinMaxGradient(grad);

            // Size over lifetime — flicker pulse
            var sz = ps.sizeOverLifetime;
            sz.enabled = true;
            sz.size    = new ParticleSystem.MinMaxCurve(1f, BuildFlickerCurve());

            // Renderer
            var rend = GetComponent<ParticleSystemRenderer>();
            rend.renderMode   = ParticleSystemRenderMode.Billboard;
            rend.sortingOrder = isAmbient ? 5 : 6;

            Material mat = TryGetFireflyMaterial();
            if (mat != null) rend.material = mat;
        }

        // ── Curve helpers ──────────────────────────────────────────────────────
        static AnimationCurve BuildSineCurve(float amplitude)
        {
            return new AnimationCurve(
                new Keyframe(0.00f,  0f),
                new Keyframe(0.25f,  amplitude),
                new Keyframe(0.50f,  0f),
                new Keyframe(0.75f, -amplitude),
                new Keyframe(1.00f,  0f)
            );
        }

        static AnimationCurve BuildDriftCurve()
        {
            // Gentle upward acceleration then ease off
            return new AnimationCurve(
                new Keyframe(0.0f, 0.02f),
                new Keyframe(0.5f, 0.06f),
                new Keyframe(1.0f, 0.04f)
            );
        }

        static AnimationCurve BuildFlickerCurve()
        {
            return new AnimationCurve(
                new Keyframe(0.00f, 0.00f),
                new Keyframe(0.15f, 1.00f),
                new Keyframe(0.40f, 0.65f),
                new Keyframe(0.55f, 1.00f),
                new Keyframe(0.72f, 0.70f),
                new Keyframe(0.88f, 0.90f),
                new Keyframe(1.00f, 0.00f)
            );
        }

        // ── Material ───────────────────────────────────────────────────────────
        static Material TryGetFireflyMaterial()
        {
            // Try URP particles shader first
            Shader sh = Shader.Find("Universal Render Pipeline/Particles/Unlit");
            if (sh == null) sh = Shader.Find("Particles/Standard Unlit");
            if (sh == null) sh = Shader.Find("Legacy Shaders/Particles/Additive");
            if (sh == null) return null;

            var mat = new Material(sh);

            // Additive / transparent blending so fireflies glow
            mat.SetFloat("_Surface", 1f);          // Transparent
            mat.SetFloat("_Blend", 2f);             // Additive (URP Particles/Unlit enum)
            mat.SetFloat("_BlendModePreserveSpecular", 0f);
            mat.EnableKeyword("_SURFACE_TYPE_TRANSPARENT");
            mat.EnableKeyword("_BLENDMODE_ADDITIVE");
            mat.renderQueue = 3100; // just above transparent

            return mat;
        }
    }
}
