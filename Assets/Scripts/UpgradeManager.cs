using UnityEngine;
using UnityEngine.UI;

public class UpgradeManager : MonoBehaviour
{
    // ---- Buttons for each upgrade type ----
    
    public Button healthUpgradeButton;
    public Button speedUpgradeButton;
    public Button damageUpgradeButton;

    // ---- References to other scripts ----
    public PlayerUpgrades playerUpgrades;
    public PlayerHealth playerHealth;
    public PlayerMovement playerMovement;
    public PlayerGun playerGun;

    void Start()
    {
        // ---- Add listeners to the buttons ----
        healthUpgradeButton.onClick.AddListener(OnHealthUpgradeButtonClicked);
        speedUpgradeButton.onClick.AddListener(OnSpeedUpgradeButtonClicked);
        damageUpgradeButton.onClick.AddListener(OnDamageUpgradeButtonClicked);
    }

    
    // ---- Methods to handle button clicks ----
    void OnHealthUpgradeButtonClicked()
    {
        if (playerUpgrades.CanBuyHealthUpgrade())
        {
            playerUpgrades.BuyHealthUpgrade();
            playerHealth.IncreaseMaxHealth(25);
        }
    }

    void OnSpeedUpgradeButtonClicked()
    {
        if (playerUpgrades.CanBuySpeedUpgrade())
        {
            playerUpgrades.BuySpeedUpgrade();
            playerMovement.IncreaseSpeed(1f);
        }
    }

    void OnDamageUpgradeButtonClicked()
    {
        if (playerUpgrades.CanBuyDamageUpgrade())
        {
            playerUpgrades.BuyDamageUpgrade();
            playerGun.IncreaseDamage(8);
        }
    }
}