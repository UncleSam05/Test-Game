using UnityEngine;

public class Ore : MonoBehaviour
{
    [Tooltip("List of pre-fractured ore pieces (fragments).")]
    public GameObject[] fracturedPieces;

    // Called when the drill mines a specific fragment.
    public void MineFragment(GameObject fragment)
    {
        bool found = false;
        for (int i = 0; i < fracturedPieces.Length; i++)
        {
            if (fracturedPieces[i] == fragment && fracturedPieces[i].activeSelf)
            {
                fracturedPieces[i].SetActive(false);
                found = true;
                Debug.Log("Mined fragment: " + fragment.name);
                break;
            }
        }

        if (!found)
        {
            Debug.Log("Fragment not found or already mined: " + fragment.name);
        }

        // Check if all fragments are disabled.
        bool allMined = true;
        foreach (GameObject frag in fracturedPieces)
        {
            if (frag.activeSelf)
            {
                allMined = false;
                break;
            }
        }

        if (allMined)
        {
            Debug.Log("Ore fully mined!");
            Destroy(gameObject);
        }
    }
}


