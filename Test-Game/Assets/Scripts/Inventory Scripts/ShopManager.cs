using UnityEngine;
using UnityEngine.UI;

public class ShopManager : MonoBehaviour
{
    [Header("UI References")]
    [SerializeField] private GameObject shopPanel;
    [SerializeField] private Button drillUpgradeButton;

    [Header("Drill Upgrade Settings")]
    [SerializeField] private int drillUpgradeCost = 2;
    [SerializeField] private GameObject drillVisual;

    [Header("References")]
    [SerializeField] private InventoryManager inventoryManager;

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
                    Debug.Log("Drill upgrade purchased and drill activated!");
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