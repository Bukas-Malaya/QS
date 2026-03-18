using UnityEngine;
using UnityEditor;
using WhereFirefliesReturn.Core;
using WhereFirefliesReturn.Environment;
using WhereFirefliesReturn.Resources;
using WhereFirefliesReturn.Farm;
using WhereFirefliesReturn.UI;
using WhereFirefliesReturn.Narrative;

namespace WhereFirefliesReturn.Editor
{
    public static class SceneBootstrapper
    {
        [MenuItem("Fireflies/Setup Scene Systems")]
        static void SetupSystems()
        {
            // Remove existing Systems object if present
            var existing = GameObject.Find("Systems");
            if (existing != null)
            {
                Undo.DestroyObjectImmediate(existing);
            }

            var systems = new GameObject("Systems");
            Undo.RegisterCreatedObjectUndo(systems, "Setup Scene Systems");

            systems.AddComponent<GameManager>();
            systems.AddComponent<SceneController>();
            systems.AddComponent<EnvironmentMeter>();
            systems.AddComponent<ResourceManager>();
            systems.AddComponent<CycleManager>();
            systems.AddComponent<DecisionSystem>();
            systems.AddComponent<UIManager>();
            systems.AddComponent<FireflySpawner>();

            // Auto-start game state so player movement works
            Selection.activeGameObject = systems;
            Debug.Log("[Fireflies] Scene systems created. Press Play to see the HUD + fireflies.");
        }
    }
}
