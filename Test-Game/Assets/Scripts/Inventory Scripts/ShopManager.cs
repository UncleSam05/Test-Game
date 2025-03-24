using UnityEngine;
using UnityEngine.UI;

public class ShopManager : MonoBehaviour
{
    [Header("UI References")]
    [SerializeField] private GameObject shopPanel;         // The panel containing your shop UI.
    [SerializeField] private Button drillUpgradeButton;      // The button for buying the drill upgrade.

    [Header("Drill Upgrade Settings")]
    [SerializeField] private int drillUpgradeCost = 2;       // Cost in scrap to buy the drill upgrade.
    [SerializeField] private GameObject drillVisual;         // Reference to the Drill Visual child on the player.

    [Header("References")]
    [SerializeField] private InventoryManager inventoryManager; // Reference to your InventoryManager.

    private bool shopOpen = false;

    private void Start()
    {
        if (shopPanel != null)
            shopPanel.SetActive(false);

        if (drillUpgradeButton != null)
            drillUpgradeButton.onClick.AddListener(BuyDrillUpgrade);

        // Ensure the drill visual is initially disabled.
        if (drillVisual != null)
            drillVisual.SetActive(false);
    }

    private void Update()
    {
        // Toggle shop with the Tab key.
        if (Input.GetKeyDown(KeyCode.Tab))
        {
            ToggleShop();
        }
    }

    private void ToggleShop()
    {
        shopOpen = !shopOpen;
        shopPanel.SetActive(shopOpen);
        Time.timeScale = shopOpen ? 0f : 1f;
    }

    private void BuyDrillUpgrade()
    {
        if (inventoryManager != null)
        {
            if (inventoryManager.SpendScrap(drillUpgradeCost))
            {
                if (drillVisual != null)
                {
                    drillVisual.SetActive(true);
                    Animator drillAnimator = drillVisual.GetComponent<Animator>();
                    if (drillAnimator != null)
                    {
                        // Trigger the emerge animation.
                        drillAnimator.SetTrigger("ActivateDrill");
                    }
                    Debug.Log("Drill upgrade purchased and activated!");
                }
                else
                {
                    Debug.LogWarning("Drill Visual reference not set in ShopManager.");
                }
                drillUpgradeButton.interactable = false;
            }
            else
            {
                Debug.Log("Not enough scrap to purchase drill upgrade!");
            }
        }
    }
}