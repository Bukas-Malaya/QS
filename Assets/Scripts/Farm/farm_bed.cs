using UnityEngine;
using WhereFirefliesReturn.Resources;

namespace WhereFirefliesReturn.Resources
{
    public class farm_bed : ResourceNode
    {
        [SerializeField] float FertileState = 10f; //A value representing how fertile the soil is
        [SerializeField] bool isPlanted = false; //Whether the crop is planted or not
        [SerializeField] GameObject cropPrefab; //The prefab of the crop to be planted
        public string currentCrop = "None"; //The name of the current crop planted

        [Header("Interaction")]
        [SerializeField] private string text = "Press E to plant crop";

        private MaterialPropertyBlock propertyBlock;
        private Renderer renderer;

        public Color BaseColor { get { return propertyBlock.GetColor("_BaseColor"); } }
        public override bool IsCollected { get => isPlanted; protected set => isPlanted = value; }

        void Start()
        {
            renderer = GetComponent<Renderer>();
            propertyBlock = new MaterialPropertyBlock();
            renderer.GetPropertyBlock(propertyBlock);
            propertyBlock.SetColor("_BaseColor", BaseColor);
            renderer.SetPropertyBlock(propertyBlock);

            PromptText = text;
        }

        public override void Collect()
        {
            if (!isPlanted)
                Plant();
        }

        public override void Plant()
        {
            if (!isPlanted)
            {
                if (cropPrefab != null)
                    Instantiate(cropPrefab, transform.position + Vector3.up * 0.5f, Quaternion.identity);
                isPlanted = true;
                PromptText = "Already planted";
                Debug.Log($"[farm_bed] Planted on {gameObject.name}.");
            }
        }

        void Update()
        {
            renderer.GetPropertyBlock(propertyBlock);
            propertyBlock.SetColor("_BaseColor", FertileState > 10f ? BaseColor : BaseColor * (FertileState / 5f));
            renderer.SetPropertyBlock(propertyBlock);
        }
    }
}
