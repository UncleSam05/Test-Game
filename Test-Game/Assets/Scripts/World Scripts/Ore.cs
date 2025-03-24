using UnityEngine;

public class Ore : MonoBehaviour
{
    // Called when the ore is successfully mined.
    public void Mine()
    {
        Debug.Log("Ore successfully mined!");
        // Add additional logic like playing a sound, spawning resources, etc.
        Destroy(gameObject);
    }
}

