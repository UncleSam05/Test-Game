using UnityEngine;

public class DrillMining : MonoBehaviour
{
    [Tooltip("Seconds the player must hold left click to mine the ore.")]
    public float requiredHoldTime = 3f;

    private float holdTimer = 0f;
    private Ore currentOre = null;
    private GameInput gameInput;
    private Animator drillAnimator;

    private void Awake()
    {
        // Locate the GameInput instance in the scene.
        gameInput = FindFirstObjectByType<GameInput>();
        drillAnimator = GetComponent<Animator>();
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        Ore ore = other.GetComponent<Ore>();
        if (ore != null)
        {
            currentOre = ore;
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        Ore ore = other.GetComponent<Ore>();
        if (ore != null && ore == currentOre)
        {
            currentOre = null;
            ResetMining();
        }
    }

    private void Update()
    {
        // Check if left mouse is held via GameInput.
        if (gameInput != null && gameInput.IsMinePressed)
        {
            // Set animator parameter to trigger the drill loop animation.
            if (drillAnimator != null)
                drillAnimator.SetBool("IsDrilling", true);

            if (currentOre != null)
            {
                holdTimer += Time.deltaTime;
                if (holdTimer >= requiredHoldTime)
                {
                    currentOre.Mine();
                    ResetMining();
                }
            }
        }
        else
        {
            // When left click is released, stop the drill loop animation and reset timer.
            if (drillAnimator != null)
                drillAnimator.SetBool("IsDrilling", false);
            ResetMining();
        }
    }

    private void ResetMining()
    {
        holdTimer = 0f;
    }
}
