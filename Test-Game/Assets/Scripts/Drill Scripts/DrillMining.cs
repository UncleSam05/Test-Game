using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem; // For the new Input System

public class DrillController : MonoBehaviour
{
    [Tooltip("Time interval (in seconds) between mining each fragment.")]
    public float chunkMiningInterval = 1f;
    [Tooltip("Rotation offset (in degrees) to add to the drill's rotation.")]
    public float rotationOffset = 0f;

    private float holdTimer = 0f;
    // This will hold the ore fragment that is currently colliding with the drill.
    private GameObject currentFragment = null;
    private GameInput gameInput;
    private Animator drillAnimator;

    // Drill state flags.
    private bool drillActive = false;       // True if drill is activated (after activation animation).
    private bool isTransitioning = false;     // True while activation/deactivation animation is playing.

    private void Awake()
    {
        gameInput = FindFirstObjectByType<GameInput>();
        drillAnimator = GetComponent<Animator>();
    }

    private void Update()
    {
        // Toggle drill activation/deactivation with the X key.
        if (gameInput != null && gameInput.ToggleDrillTriggered && !isTransitioning)
        {
            if (!drillActive)
            {
                StartCoroutine(ActivateDrill());
            }
            else
            {
                StartCoroutine(DeactivateDrill());
            }
        }

        // Only allow drilling and rotation if the drill is active and not transitioning.
        if (drillActive && !isTransitioning)
        {
            // --- Drilling Logic ---
            if (gameInput != null && gameInput.IsMinePressed)
            {
                if (drillAnimator != null)
                    drillAnimator.SetBool("IsDrilling", true);

                // If the drill is touching an ore fragment, accumulate time.
                if (currentFragment != null)
                {
                    holdTimer += Time.deltaTime;
                    if (holdTimer >= chunkMiningInterval)
                    {
                        // Get the Ore component from the fragment's parent.
                        Ore ore = currentFragment.transform.parent.GetComponent<Ore>();
                        if (ore != null)
                        {
                            ore.MineFragment(currentFragment);
                        }
                        // Reset the fragment and timer after mining.
                        currentFragment = null;
                        holdTimer = 0f;
                    }
                }
            }
            else
            {
                if (drillAnimator != null)
                    drillAnimator.SetBool("IsDrilling", false);
                ResetMining();
            }

            // --- Rotation Logic ---
            if (Mouse.current != null)
            {
                Vector2 mousePos = Camera.main.ScreenToWorldPoint(Mouse.current.position.ReadValue());
                Vector2 direction = mousePos - (Vector2)transform.position;
                float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
                transform.rotation = Quaternion.Euler(0, 0, angle + rotationOffset);
            }
        }
        else
        {
            if (drillAnimator != null)
                drillAnimator.SetBool("IsDrilling", false);
        }
    }

    private IEnumerator ActivateDrill()
    {
        isTransitioning = true;
        if (drillAnimator != null)
            drillAnimator.SetTrigger("ActivateDrill");
        yield return new WaitForSeconds(1f);
        drillActive = true;
        isTransitioning = false;
    }

    private IEnumerator DeactivateDrill()
    {
        isTransitioning = true;
        if (drillAnimator != null)
            drillAnimator.SetTrigger("DeactivateDrill");
        yield return new WaitForSeconds(1f);
        drillActive = false;
        isTransitioning = false;
        transform.rotation = Quaternion.Euler(0, 0, 0);
    }

    private void ResetMining()
    {
        holdTimer = 0f;
    }

    // These methods are called by the trigger handler on the drill child.
    public void SetCurrentFragment(GameObject fragment)
    {
        currentFragment = fragment;
    }

    public void ClearCurrentFragment(GameObject fragment)
    {
        if (currentFragment == fragment)
        {
            currentFragment = null;
            ResetMining();
        }
    }
}
