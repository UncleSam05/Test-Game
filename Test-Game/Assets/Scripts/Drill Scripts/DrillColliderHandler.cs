using UnityEngine;

public class DrillFragmentTrigger : MonoBehaviour
{
    private DrillController drillController;

    private void Awake()
    {
        // Find the DrillController on a parent object.
        drillController = GetComponentInParent<DrillController>();
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        // Check if the collided object is tagged as "OreFragment".
        if (other.CompareTag("OreFragment"))
        {
            drillController.SetCurrentFragment(other.gameObject);
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("OreFragment"))
        {
            drillController.ClearCurrentFragment(other.gameObject);
        }
    }
}


