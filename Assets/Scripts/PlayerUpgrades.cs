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

    public UpgradeImage healthUpgradeImages;
    public UpgradeImage speedUpgradeImages;
    public UpgradeImage damageUpgradeImages;

    public TMP_Text healthUpgradeLevel;
    public TMP_Text speedUpgradeLevel;
    public TMP_Text damageUpgradeLevel;

    public void Start()
    {
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
        return healthUpgradesBought < maxHealthUpgrades;
    }

    public bool CanBuySpeedUpgrade()
    {
        return speedUpgradesBought < maxSpeedUpgrades;
    }

    public bool CanBuyDamageUpgrade()
    {
        return damageUpgradesBought < maxDamageUpgrades;
    }

    public void BuyHealthUpgrade()
    {
        if (CanBuyHealthUpgrade())
        {
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