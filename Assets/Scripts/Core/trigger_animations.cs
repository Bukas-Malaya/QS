using UnityEngine;

public class trigger_animations : MonoBehaviour
{
    [SerializeField] private Animator anim;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        anim.Play("isagani_idle");
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
