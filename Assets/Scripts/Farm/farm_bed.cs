using UnityEngine;
using WhereFirefliesReturn.Resources;
using System.Collections;

namespace WhereFirefliesReturn.Resources
{
    public class farm_bed : ResourceNode
    {
        [Header("Soil")]
        [SerializeField] private Color fertileColor = new Color(0.18f, 0.12f, 0.06f);
        [SerializeField] private Color infertileColor = new Color(0.55f, 0.45f, 0.38f);
        [SerializeField] float FertileState = 10f; //A value representing how fertile the soil is

        [Header("Crop")]
        [SerializeField] private bool isEdgeBed = false; // hint: water spinach prefers edges
        public CropType plantedCrop = CropType.None;

        public GameObject CurrentCropObject { get; private set; }
 
        [Header("Prefabs per crop type")]
        [SerializeField] private GameObject sweetPotatoPrefab;
        [SerializeField] private GameObject cornPrefab;
        [SerializeField] private GameObject waterSpinachPrefab;
        [SerializeField] private GameObject marigoldPrefab;
 
        [Header("Neighbors (assign in Inspector)")]
        [SerializeField] private farm_bed[] neighbors;
 
        [Header("Particles")]
        [SerializeField] private ParticleSystem mismatchParticles;
        [SerializeField] private ParticleSystem companionParticles;

        [Header("Interaction")]
        [SerializeField] private string emptyPrompt = "Press E to plant crop";
        private MaterialPropertyBlock propertyBlock;
        private Renderer renderer;
        [SerializeField] public bool isPlanted = false; //Whether the crop is planted or not
        public Color BaseColor { get { return propertyBlock.GetColor("_BaseColor"); } }
        public override bool IsCollected { get => false; protected set => isPlanted = value; }
        
        void Start()
        {
            renderer = GetComponent<Renderer>();
            propertyBlock = new MaterialPropertyBlock();
            ApplySoilColor(infertileColor);
            PromptText = emptyPrompt;

            if (mismatchParticles != null) mismatchParticles.Stop();
            if (companionParticles != null) companionParticles.Stop();

            // Pre-plant if a crop is already assigned in the Inspector
            if (plantedCrop != CropType.None)
                PlantDirectly(plantedCrop);
                }

        public override void Collect()
        {
            if (isPlanted)
            {
                var selected = Hotbar.Instance?.SelectedItem;
                Debug.Log($"Interacting with planted bed. Selected item: {(selected != null ? selected.itemName : "None")}");
                if (selected != null && selected.category == ItemCategory.Tool && selected.itemName == "Shovel")
                    Uproot();
                else
                    StartCoroutine(TempPrompt("Equip a shovel to remove crop"));
                return;
            }
            Plant();
        }
        public void Uproot()
        {
            if (!isPlanted) return;

            // Destroy the crop GameObject (find it above the bed)
            // Assumes the crop is a direct child or you can find it by position
            if (CurrentCropObject != null) Destroy(CurrentCropObject);
            CurrentCropObject = null;

            plantedCrop = CropType.None;
            isPlanted = false;
            PromptText = emptyPrompt;
            ApplySoilColor(infertileColor);

            SetMismatchParticles(false);
            SetCompanionParticles(false);

            // Re-evaluate neighbors
            foreach (var n in neighbors)
                if (n != null) n.EvaluateCompanions();

            FieldPuzzleManager.Instance?.OnBedPlanted();
        }

        public override void Plant()
        {
            CropType selected = Hotbar.Instance != null ? Hotbar.Instance.SelectedCrop : CropType.None;

            if (selected == CropType.None)
            {
                StartCoroutine(TempPrompt(""));
                return;
            }

            if (Hotbar.Instance != null && !Hotbar.Instance.TrySpendSelected())
            {
                StartCoroutine(TempPrompt("Selected crop not equipped"));
                return;
            }
            if (ResourceManager.Instance?.CanAfford(0,1,0) == false) {
                StartCoroutine(TempPrompt("Not enough seeds to plant"));
                return;
            }
            if (!isPlanted)
            {
                CurrentCropObject = Instantiate(GetCropPrefab(selected), transform.position + Vector3.up * 1f, Quaternion.Euler(45, -90, 0));
                isPlanted = true;
                plantedCrop = selected;
                PromptText = "Already planted";
                EvaluateCompanions();

                foreach (var n in neighbors)
                    if (n != null) n.EvaluateCompanions();
                
                FieldPuzzleManager.Instance?.OnBedPlanted();
            }
        }
        private IEnumerator TempPrompt(string message)
            {
                string original = PromptText;
                PromptText = message;
                yield return new WaitForSeconds(2f);
                PromptText = original;
            }


        private void PlantDirectly(CropType crop)
        {
            if (isPlanted) return;
            var prefab = GetCropPrefab(crop);
            if (prefab == null) return;

            CurrentCropObject = Instantiate(prefab, transform.position + Vector3.up * 1f, Quaternion.Euler(45, -90, 0));
            isPlanted = true;
            plantedCrop = crop;
            PromptText = "Already planted";
            EvaluateCompanions();
        }
        public void EvaluateCompanions()
        {
            if (!isPlanted) return;
            Debug.Log($"[{name}] Evaluating - crop: {plantedCrop}, neighbors: {neighbors.Length}");
 
            bool hasCompanion = false;
            bool hasCompetitor = false;
 
            foreach (var neighbor in neighbors)
            {
                if (neighbor == null || !neighbor.isPlanted) continue;
 
                if (CompanionData.AreCompanions(plantedCrop, neighbor.plantedCrop))
                    hasCompanion = true;
                if (CompanionData.AreCompetitors(plantedCrop, neighbor.plantedCrop))
                    hasCompetitor = true;
            }
 
            if (isEdgeBed && plantedCrop == CropType.WaterSpinach)
                hasCompanion = true;
 
            // Apply visual feedback
            SetMismatchParticles(hasCompetitor && !hasCompanion);
            SetCompanionParticles(hasCompanion);
            ApplySoilColor(hasCompanion ? fertileColor : infertileColor);
 
            FertileState = hasCompanion ? 10f : hasCompetitor ? 3f : 6f;
        }

        public bool IsCorrectlyPlaced()
        {
            if (!isPlanted) return false;
            foreach (var neighbor in neighbors)
            {
                if (neighbor == null || !neighbor.isPlanted) continue;
                if (CompanionData.AreCompanions(plantedCrop, neighbor.plantedCrop))
                    return true;
            }
            // Edge water spinach counts as correct on its own
            if (isEdgeBed && plantedCrop == CropType.WaterSpinach) return true;
            // Marigold on border is self-sufficient
            if (isEdgeBed && plantedCrop == CropType.Marigold) return true;
            return false;
            
        }
        GameObject GetCropPrefab(CropType type)
        {
            switch (type)
            {
                case CropType.SweetPotato: return sweetPotatoPrefab;
                case CropType.Corn: return cornPrefab;
                case CropType.WaterSpinach: return waterSpinachPrefab;
                case CropType.Marigold: return marigoldPrefab;
                default: return null;
            }
        }

        void Update()
        {

        }
        void ApplySoilColor(Color color)
        {
            renderer.GetPropertyBlock(propertyBlock);
            propertyBlock.SetColor("_BaseColor", color);
            renderer.SetPropertyBlock(propertyBlock);
        }

        void SetMismatchParticles(bool active) {
            //Debug.Log($"[{name}] Setting mismatch particles: {active}");
            if (mismatchParticles == null) return;
            if (active && !mismatchParticles.isPlaying) mismatchParticles.Play();
            else if (!active && mismatchParticles.isPlaying) mismatchParticles.Stop();
        }
        void SetCompanionParticles(bool active) {
            //Debug.Log($"[{name}] Setting companion particles: {active}");
            if (companionParticles == null) return;
            if (active && !companionParticles.isPlaying) companionParticles.Play();
            else if (!active && companionParticles.isPlaying) companionParticles.Stop();
        }
    }
}
