using UnityEngine;
using TMPro;

public class PlayerUpgrades : MonoBehaviour
{
    public int totalUpgrades = 10;
    public int upgradesBought = 0;

    public int healthUpgradesBought = 0;
    public int speedUpgradesBought = 0;
    public int damageUpgradesBought = 0;

    public int maxHealthUpgrades = 4;
    public int maxSpeedUpgrades = 4;
    public int maxDamageUpgrades = 4;

    public int healthUpgradeCost = 3000;
    public int speedUpgradeCost = 3000;
    public int damageUpgradeCost = 3000;

    public UpgradeImage healthUpgradeImages;
    public UpgradeImage speedUpgradeImages;
    public UpgradeImage damageUpgradeImages;

    public TMP_Text healthUpgradeLevel;
    public TMP_Text speedUpgradeLevel;
    public TMP_Text damageUpgradeLevel;

    private PlayerCurrency playerCurrency;

    public void Start()
    {
        playerCurrency = GetComponent<PlayerCurrency>();

        healthUpgradeImages.Start();
        speedUpgradeImages.Start();
        damageUpgradeImages.Start();

        UpdateUpgradeTexts();
        UpdateUpgradeImages();
    }

    public void IncrementUpgradesBought()
    {
        if (upgradesBought < totalUpgrades)
        {
            upgradesBought++;
        }
    }

    public bool CanBuyHealthUpgrade()
    {
        return healthUpgradesBought < maxHealthUpgrades && playerCurrency.GetCurrency() >= healthUpgradeCost;
    }

    public bool CanBuySpeedUpgrade()
    {
        return speedUpgradesBought < maxSpeedUpgrades && playerCurrency.GetCurrency() >= speedUpgradeCost;
    }

    public bool CanBuyDamageUpgrade()
    {
        return damageUpgradesBought < maxDamageUpgrades && playerCurrency.GetCurrency() >= damageUpgradeCost;
    }

    public void BuyHealthUpgrade()
    {
        if (CanBuyHealthUpgrade())
        {
            playerCurrency.AddCurrency(-healthUpgradeCost);
            healthUpgradesBought++;
            IncrementUpgradesBought();
            UpdateUpgradeTexts();
            UpdateUpgradeImages();
        }
    }

    public void BuySpeedUpgrade()
    {
        if (CanBuySpeedUpgrade())
        {
            playerCurrency.AddCurrency(-speedUpgradeCost);
            speedUpgradesBought++;
            IncrementUpgradesBought();
            UpdateUpgradeTexts();
            UpdateUpgradeImages();
        }
    }

    public void BuyDamageUpgrade()
    {
        if (CanBuyDamageUpgrade())
        {
            playerCurrency.AddCurrency(-damageUpgradeCost);
            damageUpgradesBought++;
            IncrementUpgradesBought();
            UpdateUpgradeTexts();
            UpdateUpgradeImages();
        }
    }

    private void UpdateUpgradeTexts()
    {
        if (healthUpgradeLevel != null)
        {
            healthUpgradeLevel.text = RomanNumeralConverter.ToRoman(healthUpgradesBought);
        }
        if (speedUpgradeLevel != null)
        {
            speedUpgradeLevel.text = RomanNumeralConverter.ToRoman(speedUpgradesBought);
        }
        if (damageUpgradeLevel != null)
        {
            damageUpgradeLevel.text = RomanNumeralConverter.ToRoman(damageUpgradesBought);
        }
    }

    private void UpdateUpgradeImages()
    {
        healthUpgradeImages.UpdateImages(healthUpgradesBought);
        speedUpgradeImages.UpdateImages(speedUpgradesBought);
        damageUpgradeImages.UpdateImages(damageUpgradesBought);
    }
}