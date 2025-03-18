using UnityEngine;
using UnityEngine.UI;

public class InventoryManager : MonoBehaviour
{
    public static InventoryManager Instance { get; private set; }

    // The background images for each inventory slot (always visible).
    [SerializeField] private Image[] slotFrames;
    // The child images that display the scrap icon (initially disabled).
    [SerializeField] private Image[] scrapIcons;
    // The sprite to display when a scrap is collected.
    [SerializeField] private Sprite scrapSprite;

    private int scrapCount = 0;

    private void Awake()
    {
        if (Instance != null && Instance != this)
            Destroy(gameObject);
        else
            Instance = this;
    }

    public bool IsFull()
    {
        return scrapCount >= slotFrames.Length;
    }

    public bool AddScrap()
    {
        if (scrapCount < slotFrames.Length)
        {
            // Make sure the scrap icon exists for this slot.
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
}
