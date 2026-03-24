using UnityEngine;

public class farm_bed : MonoBehaviour
{
    [SerializeField] float FertileState = 10f; //A value representing how fertile the soil is
    [SerializeField] bool isPlanted = false; //Whether the crop is planted or not
    [SerializeField] GameObject cropPrefab; //The prefab of the crop to be planted
    public string currentCrop = "None"; //The name of the current crop planted
    
    private MaterialPropertyBlock propertyBlock;
    private Renderer renderer;

    public Color BaseColor { get { return propertyBlock.GetColor("_BaseColor"); } }
    public bool IsPlanted { get { return isPlanted; } }
    void Start()
    {
        renderer = GetComponent<Renderer>();
        propertyBlock = new MaterialPropertyBlock();
        renderer.GetPropertyBlock(propertyBlock);
        propertyBlock.SetColor("_BaseColor", BaseColor);
        renderer.SetPropertyBlock(propertyBlock);
    }


    public void PlantCrop() {
        if (!isPlanted) {
            Instantiate(cropPrefab, transform.position + Vector3.up * 0.5f, Quaternion.identity);
            isPlanted = true;
        }
    }

    void Update()
    {
        renderer.GetPropertyBlock(propertyBlock);
        propertyBlock.SetColor("_BaseColor", FertileState > 10f ? BaseColor : BaseColor * (FertileState / 5f));
        renderer.SetPropertyBlock(propertyBlock);
    }
}
