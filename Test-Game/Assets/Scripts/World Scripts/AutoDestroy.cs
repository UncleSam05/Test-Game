using UnityEngine;

public class AutoDestroy : MonoBehaviour
{
    [SerializeField] private float lifeTime = 1f; // Adjust based on your animation length

    private void Start()
    {
        Destroy(gameObject, lifeTime);
    }
}
