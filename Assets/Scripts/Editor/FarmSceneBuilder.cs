// Editor-only script. Safe to ship — Unity strips the Editor folder from builds.
#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;

namespace WhereFirefliesReturn.Editor
{
    public static class FarmSceneBuilder
    {
        // ── Entry point ────────────────────────────────────────────────────────
        [MenuItem("Fireflies/Build Farm Scene")]
        public static void BuildFarmScene()
        {
            ClearExistingFarm();
            EnsureCamera();
            SetupLighting();
            SetupFog();

            GameObject root = new GameObject("=== FARM SCENE ===");

            BuildGround(root);
            BuildBarn(root, new Vector3(8f, 0f, 10f));
            BuildSilo(root, new Vector3(13f, 0f, 10f));
            BuildFenceRow(root, new Vector3(-6f, 0f, 0f), 12, Vector3.right);
            BuildFenceRow(root, new Vector3(-6f, 0f, 0f), 6, Vector3.forward);
            BuildTree(root, new Vector3(-4f, 0f, 8f), 1.2f);
            BuildTree(root, new Vector3(-7f, 0f, 12f), 1.6f);
            BuildTree(root, new Vector3(18f, 0f, 6f), 1.0f);
            BuildHayBale(root, new Vector3(3f, 0f, 5f));
            BuildHayBale(root, new Vector3(4.5f, 0f, 5f));
            BuildWell(root, new Vector3(-2f, 0f, 10f));

            Debug.Log("[FarmSceneBuilder] Farm built. Run 'Fireflies > Setup Main Menu Scene' next.");
        }

        [MenuItem("Fireflies/Clear Farm Scene")]
        public static void ClearExistingFarm()
        {
            var existing = GameObject.Find("=== FARM SCENE ===");
            if (existing != null)
            {
                Object.DestroyImmediate(existing);
                Debug.Log("[FarmSceneBuilder] Cleared existing farm.");
            }
        }

        // ── Camera ─────────────────────────────────────────────────────────────
        static void EnsureCamera()
        {
            if (Camera.main != null) return;

            var camGo = new GameObject("Main Camera");
            camGo.tag = "MainCamera";
            var cam = camGo.AddComponent<Camera>();
            cam.clearFlags = CameraClearFlags.SolidColor;
            cam.backgroundColor = new Color(0.04f, 0.05f, 0.10f);
            cam.transform.position = new Vector3(0f, 3f, -8f);
            cam.transform.rotation = Quaternion.Euler(8f, 0f, 0f);
            camGo.AddComponent<AudioListener>();

            Debug.Log("[FarmSceneBuilder] Camera created.");
        }

        // ── Lighting & Fog ─────────────────────────────────────────────────────
        static void SetupLighting()
        {
            // Dusk/night mood — dim warm directional light
            var light = Object.FindFirstObjectByType<Light>();
            if (light != null)
            {
                light.color = new Color(0.18f, 0.14f, 0.22f); // deep purple dusk
                light.intensity = 0.4f;
                light.transform.rotation = Quaternion.Euler(35f, -60f, 0f);
            }

            // Camera background — dark navy sky
            var cam = Camera.main;
            if (cam != null)
            {
                cam.backgroundColor = new Color(0.04f, 0.05f, 0.10f);
                cam.clearFlags = CameraClearFlags.SolidColor;
                cam.transform.position = new Vector3(0f, 3f, -8f);
                cam.transform.rotation = Quaternion.Euler(8f, 0f, 0f);
            }
        }

        static void SetupFog()
        {
            RenderSettings.fog = true;
            RenderSettings.fogMode = FogMode.Exponential;
            RenderSettings.fogColor = new Color(0.06f, 0.07f, 0.12f);
            RenderSettings.fogDensity = 0.04f;
            RenderSettings.ambientLight = new Color(0.05f, 0.06f, 0.10f);
        }

        // ── Ground ─────────────────────────────────────────────────────────────
        static void BuildGround(GameObject parent)
        {
            var go = GameObject.CreatePrimitive(PrimitiveType.Plane);
            go.name = "Ground";
            go.transform.SetParent(parent.transform);
            go.transform.localScale = new Vector3(10f, 1f, 10f); // 100x100 units
            go.transform.position = Vector3.zero;
            ApplyColor(go, new Color(0.13f, 0.17f, 0.10f)); // dark muted grass
        }

        // ── Barn ───────────────────────────────────────────────────────────────
        static void BuildBarn(GameObject parent, Vector3 pos)
        {
            var barn = new GameObject("Barn");
            barn.transform.SetParent(parent.transform);
            barn.transform.position = pos;

            // Main body
            var body = MakeBox(barn, "Body", Vector3.zero, new Vector3(6f, 4f, 5f),
                new Color(0.35f, 0.12f, 0.08f)); // dark red barn

            // Roof left panel
            var roofL = MakeBox(barn, "Roof_L", new Vector3(-1.8f, 3.2f, 0f),
                new Vector3(3f, 0.25f, 5.4f), new Color(0.15f, 0.10f, 0.08f));
            roofL.transform.localRotation = Quaternion.Euler(0f, 0f, 35f);

            // Roof right panel
            var roofR = MakeBox(barn, "Roof_R", new Vector3(1.8f, 3.2f, 0f),
                new Vector3(3f, 0.25f, 5.4f), new Color(0.15f, 0.10f, 0.08f));
            roofR.transform.localRotation = Quaternion.Euler(0f, 0f, -35f);

            // Door (dark inset rectangle)
            MakeBox(barn, "Door", new Vector3(0f, -0.6f, 2.55f),
                new Vector3(1.4f, 2.8f, 0.1f), new Color(0.08f, 0.05f, 0.03f));

            // Window left
            MakeBox(barn, "Window_L", new Vector3(-1.8f, 0.8f, 2.55f),
                new Vector3(0.9f, 0.7f, 0.1f), new Color(0.08f, 0.05f, 0.03f));

            // Window right
            MakeBox(barn, "Window_R", new Vector3(1.8f, 0.8f, 2.55f),
                new Vector3(0.9f, 0.7f, 0.1f), new Color(0.08f, 0.05f, 0.03f));
        }

        // ── Silo ───────────────────────────────────────────────────────────────
        static void BuildSilo(GameObject parent, Vector3 pos)
        {
            var silo = new GameObject("Silo");
            silo.transform.SetParent(parent.transform);
            silo.transform.position = pos;

            // Cylinder body
            var body = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
            body.name = "SiloBody";
            body.transform.SetParent(silo.transform);
            body.transform.localPosition = new Vector3(0f, 3f, 0f);
            body.transform.localScale = new Vector3(1.8f, 3f, 1.8f);
            ApplyColor(body, new Color(0.55f, 0.50f, 0.38f)); // weathered concrete

            // Cone roof (scaled sphere squished flat)
            var top = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            top.name = "SiloTop";
            top.transform.SetParent(silo.transform);
            top.transform.localPosition = new Vector3(0f, 6.3f, 0f);
            top.transform.localScale = new Vector3(2f, 0.8f, 2f);
            ApplyColor(top, new Color(0.25f, 0.18f, 0.12f));
        }

        // ── Fence ──────────────────────────────────────────────────────────────
        static void BuildFenceRow(GameObject parent, Vector3 start, int count, Vector3 dir)
        {
            var fenceRoot = new GameObject("Fence");
            fenceRoot.transform.SetParent(parent.transform);

            for (int i = 0; i < count; i++)
            {
                Vector3 pos = start + dir * (i * 2f);

                // Post
                var post = MakeBox(fenceRoot, $"Post_{i}", pos + Vector3.up * 0.65f,
                    new Vector3(0.15f, 1.3f, 0.15f), new Color(0.4f, 0.28f, 0.15f));

                // Rail (horizontal bar between posts)
                if (i < count - 1)
                {
                    var rail = MakeBox(fenceRoot, $"Rail_{i}",
                        pos + dir + Vector3.up * 0.85f,
                        new Vector3(dir.z > 0 ? 0.12f : 2f, 0.12f, dir.z > 0 ? 2f : 0.12f),
                        new Color(0.38f, 0.26f, 0.13f));
                }
            }
        }

        // ── Tree ───────────────────────────────────────────────────────────────
        static void BuildTree(GameObject parent, Vector3 pos, float scale)
        {
            var tree = new GameObject("Tree");
            tree.transform.SetParent(parent.transform);
            tree.transform.position = pos;

            // Trunk
            var trunk = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
            trunk.name = "Trunk";
            trunk.transform.SetParent(tree.transform);
            trunk.transform.localPosition = new Vector3(0f, 1.2f * scale, 0f);
            trunk.transform.localScale = new Vector3(0.3f * scale, 1.2f * scale, 0.3f * scale);
            ApplyColor(trunk, new Color(0.22f, 0.15f, 0.08f));

            // Canopy
            var canopy = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            canopy.name = "Canopy";
            canopy.transform.SetParent(tree.transform);
            canopy.transform.localPosition = new Vector3(0f, 3.2f * scale, 0f);
            canopy.transform.localScale = new Vector3(2f * scale, 2.2f * scale, 2f * scale);
            ApplyColor(canopy, new Color(0.08f, 0.18f, 0.08f)); // dark muted green
        }

        // ── Hay Bale ───────────────────────────────────────────────────────────
        static void BuildHayBale(GameObject parent, Vector3 pos)
        {
            var bale = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
            bale.name = "HayBale";
            bale.transform.SetParent(parent.transform);
            bale.transform.position = pos + Vector3.up * 0.55f;
            bale.transform.localScale = new Vector3(1.1f, 0.55f, 1.1f);
            bale.transform.rotation = Quaternion.Euler(90f, 0f, 0f);
            ApplyColor(bale, new Color(0.55f, 0.44f, 0.14f));
        }

        // ── Well ───────────────────────────────────────────────────────────────
        static void BuildWell(GameObject parent, Vector3 pos)
        {
            var well = new GameObject("Well");
            well.transform.SetParent(parent.transform);
            well.transform.position = pos;

            // Stone ring
            var ring = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
            ring.name = "Ring";
            ring.transform.SetParent(well.transform);
            ring.transform.localPosition = new Vector3(0f, 0.5f, 0f);
            ring.transform.localScale = new Vector3(1.2f, 0.5f, 1.2f);
            ApplyColor(ring, new Color(0.35f, 0.32f, 0.28f));

            // Left post
            MakeBox(well, "Post_L", new Vector3(-0.6f, 1.4f, 0f),
                new Vector3(0.12f, 1.8f, 0.12f), new Color(0.28f, 0.20f, 0.10f));

            // Right post
            MakeBox(well, "Post_R", new Vector3(0.6f, 1.4f, 0f),
                new Vector3(0.12f, 1.8f, 0.12f), new Color(0.28f, 0.20f, 0.10f));

            // Cross beam
            MakeBox(well, "Beam", new Vector3(0f, 2.35f, 0f),
                new Vector3(1.4f, 0.12f, 0.12f), new Color(0.28f, 0.20f, 0.10f));
        }

        // ── Helpers ────────────────────────────────────────────────────────────
        static GameObject MakeBox(GameObject parent, string name, Vector3 localPos,
            Vector3 scale, Color color)
        {
            var go = GameObject.CreatePrimitive(PrimitiveType.Cube);
            go.name = name;
            go.transform.SetParent(parent.transform);
            go.transform.localPosition = localPos;
            go.transform.localScale = scale;
            ApplyColor(go, color);
            return go;
        }

        static void ApplyColor(GameObject go, Color color)
        {
            var renderer = go.GetComponent<Renderer>();
            if (renderer == null) return;

            var mat = new Material(Shader.Find("Universal Render Pipeline/Lit"));
            if (mat.shader.name == "Hidden/InternalErrorShader")
                mat = new Material(Shader.Find("Standard")); // fallback if not URP

            mat.color = color;
            renderer.sharedMaterial = mat;
        }
    }
}
#endif
