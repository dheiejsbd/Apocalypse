using UnityEngine;

public class DestroyWithDelay : MonoBehaviour
{
    [SerializeField]
    [Range(0f,1000f)]
    private float m_Delay;


    private void Start()
    {
        Destroy(gameObject, m_Delay);
    }
}
