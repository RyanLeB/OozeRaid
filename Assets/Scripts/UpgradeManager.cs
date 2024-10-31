using UnityEngine;
using UnityEngine.UI;

public class UpgradeManager : MonoBehaviour
{
    // ---- Button references ----
    
    public Button healthUpgradeButton;
    public Button speedUpgradeButton;
    public Button damageUpgradeButton;

    
    // ---- Reference to the player components ----
    public PlayerUpgrades playerUpgrades;
    public PlayerHealth playerHealth;
    public PlayerMovement playerMovement;
    public PlayerGun playerGun;

    void Start()
    {
        healthUpgradeButton.onClick.AddListener(OnHealthUpgradeButtonClicked);
        speedUpgradeButton.onClick.AddListener(OnSpeedUpgradeButtonClicked);
        damageUpgradeButton.onClick.AddListener(OnDamageUpgradeButtonClicked);
    }

    
    // ---- Button click events ----
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
            playerGun.IncreaseDamage(15);
        }
    }
}
