using UnityEngine;

public class DrillMining : MonoBehaviour
{
    [Tooltip("Seconds the player must hold left click to mine the ore.")]
    public float requiredHoldTime = 3f;

    private float holdTimer = 0f;
    private Ore currentOre = null;
    private GameInput gameInput;

    private void Awake()
    {
        // Locate the GameInput instance in the scene.
        gameInput = FindObjectOfType<GameInput>();
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
        // Only proceed if an ore is nearby and the mine input is active.
        if (currentOre != null && gameInput != null && gameInput.IsMinePressed)
        {
            holdTimer += Time.deltaTime;
            if (holdTimer >= requiredHoldTime)
            {
                currentOre.Mine();
                ResetMining();
            }
        }
        else
        {
            // If the player stops holding the button or no ore is in range, reset the timer.
            ResetMining();
        }
    }

    private void ResetMining()
    {
        holdTimer = 0f;
    }
}

