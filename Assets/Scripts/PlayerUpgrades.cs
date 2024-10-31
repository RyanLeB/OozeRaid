using UnityEngine;
using UnityEngine.UI;

public class PlayerUpgrades : MonoBehaviour
{
    public int totalUpgrades = 10;
    public int upgradesBought = 0;
    public Slider progressBar;

    // Counters for each upgrade type
    public int healthUpgradesBought = 0;
    public int speedUpgradesBought = 0;
    public int damageUpgradesBought = 0;

    // Limits for each upgrade type
    public int maxHealthUpgrades = 4;
    public int maxSpeedUpgrades = 4;
    public int maxDamageUpgrades = 4;

    void Start()
    {
        if (progressBar != null)
        {
            progressBar.maxValue = totalUpgrades;
            progressBar.value = upgradesBought;
        }
    }

    // ---- Increment the number of upgrades bought ----
    public void IncrementUpgradesBought()
    {
        if (upgradesBought < totalUpgrades)
        {
            upgradesBought++;
            if (progressBar != null)
            {
                progressBar.value = upgradesBought;
            }
        }
    }

    // ---- Can the player buy a health upgrade? ----
    public bool CanBuyHealthUpgrade()
    {
        return healthUpgradesBought < maxHealthUpgrades;
    }
    
    // ---- Can the player buy a speed upgrade? ----
    public bool CanBuySpeedUpgrade()
    {
        return speedUpgradesBought < maxSpeedUpgrades;
    }

    
    // ---- Can the player buy a damage upgrade? ----
    public bool CanBuyDamageUpgrade()
    {
        return damageUpgradesBought < maxDamageUpgrades;
    }

    
    // ---- Buy an upgrade for each type ----
    
    public void BuyHealthUpgrade()
    {
        if (CanBuyHealthUpgrade())
        {
            healthUpgradesBought++;
            IncrementUpgradesBought();
        }
    }

    public void BuySpeedUpgrade()
    {
        if (CanBuySpeedUpgrade())
        {
            speedUpgradesBought++;
            IncrementUpgradesBought();
        }
    }

    public void BuyDamageUpgrade()
    {
        if (CanBuyDamageUpgrade())
        {
            damageUpgradesBought++;
            IncrementUpgradesBought();
        }
    }
}