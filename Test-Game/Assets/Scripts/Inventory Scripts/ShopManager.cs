using UnityEngine;
using UnityEngine.UI;

public class ShopManager : MonoBehaviour
{
    [Header("UI References")]
    [SerializeField] private GameObject shopPanel;       // The panel containing your shop UI.
    [SerializeField] private Button armorUpgradeButton;    // The button for buying the armor upgrade.

    [Header("Upgrade Settings")]
    [SerializeField] private int armorUpgradeCost = 2;     // Cost in scrap to buy the armor upgrade.

    [Header("References")]
    [SerializeField] private InventoryManager inventoryManager; // Reference to your InventoryManager.
    [SerializeField] private PlayerHealth playerHealth;         // Reference to your PlayerHealth script.

    private bool shopOpen = false;

    private void Start()
    {
        if (shopPanel != null)
            shopPanel.SetActive(false);

        if (armorUpgradeButton != null)
            armorUpgradeButton.onClick.AddListener(BuyArmorUpgrade);
    }

    private void Update()
    {
        // Toggle shop with Tab key.
        if (Input.GetKeyDown(KeyCode.Tab))
        {
            ToggleShop();
        }
    }

    private void ToggleShop()
    {
        shopOpen = !shopOpen;
        if (shopOpen)
        {
            shopPanel.SetActive(true);
            Time.timeScale = 0f;  // Pause the game.
        }
        else
        {
            shopPanel.SetActive(false);
            Time.timeScale = 1f;  // Resume the game.
        }
    }

    private void BuyArmorUpgrade()
    {
        if (inventoryManager != null && playerHealth != null)
        {
            if (inventoryManager.SpendScrap(armorUpgradeCost))
            {
                playerHealth.IncreaseMaxHealth(1);
                Debug.Log("Armor upgrade purchased!");
                // Optionally, disable the button if only one upgrade is allowed.
                armorUpgradeButton.interactable = false;
            }
            else
            {
                Debug.Log("Not enough scrap to purchase armor upgrade!");
            }
        }
    }
}