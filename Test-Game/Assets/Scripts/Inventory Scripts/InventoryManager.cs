using UnityEngine;
using UnityEngine.UI;

public class InventoryManager : MonoBehaviour
{
    public static InventoryManager Instance { get; private set; }

    [Header("Inventory UI")]
    [SerializeField] private Image[] slotFrames;  // Background frames for each slot (always visible).
    [SerializeField] private Image[] scrapIcons;    // Child images that display scrap when collected.
    [SerializeField] private Sprite scrapSprite;    // The sprite to display for a scrap.

    private int scrapCount = 0;

    private void Awake()
    {
        if (Instance != null && Instance != this)
            Destroy(gameObject);
        else
            Instance = this;
    }

    public int ScrapCount { get { return scrapCount; } }

    public bool AddScrap()
    {
        if (scrapCount < slotFrames.Length)
        {
            if (scrapIcons != null && scrapIcons.Length > scrapCount)
            {
                scrapIcons[scrapCount].sprite = scrapSprite;
                scrapIcons[scrapCount].enabled = true;
            }
            scrapCount++;
            return true;
        }
        return false;
    }

    public bool SpendScrap(int amount)
    {
        if (scrapCount >= amount)
        {
            scrapCount -= amount;
            UpdateUI();
            return true;
        }
        return false;
    }

    private void UpdateUI()
    {
        // Reset all slots.
        for (int i = 0; i < scrapIcons.Length; i++)
        {
            scrapIcons[i].enabled = false;
        }
        // Enable only for the scraps that remain.
        for (int i = 0; i < scrapCount && i < scrapIcons.Length; i++)
        {
            scrapIcons[i].enabled = true;
        }
    }
    public bool IsFull()
    {
        return scrapCount >= slotFrames.Length;
    }

}