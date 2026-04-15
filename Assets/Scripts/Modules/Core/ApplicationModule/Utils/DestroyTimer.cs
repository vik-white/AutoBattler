using UnityEngine;

public class DestroyTimer : MonoBehaviour
{
    public float Time;
    
    void Awake()
    {
        Destroy(gameObject, Time);
    }
}
