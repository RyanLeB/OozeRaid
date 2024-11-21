using UnityEngine;
using TMPro;
using System.IO;


public class PlayerUpgrades : MonoBehaviour
{
    // ---- Variables to keep track of upgrades ----
    [Header("Total Upgrades")]
    public int totalUpgrades = 10;
    public int upgradesBought = 0;
    
    [Header("Number of upgrades bought")]
    public int healthUpgradesBought = 0;
    public int speedUpgradesBought = 0;
    public int damageUpgradesBought = 0;
    public int critRateUpgradesBought = 0;

    
    [Header("Max number of upgrades")]
    public int maxHealthUpgrades = 4;
    public int maxSpeedUpgrades = 4;
    public int maxDamageUpgrades = 4;
    public int maxCritRateUpgrades = 4;

    [Header("Upgrade costs")]
    public int healthUpgradeCost = 3000;
    public int speedUpgradeCost = 3000;
    public int damageUpgradeCost = 3000;
    public int critRateUpgradeCost = 4000;
    public int holdToClickUpgradeCost = 5000;
    public int abilityUpgradeCost = 8000;

    
    [Header("Upgrade Images")]
    public UpgradeImage healthUpgradeImages;
    public UpgradeImage speedUpgradeImages;
    public UpgradeImage damageUpgradeImages;
    public UpgradeImage critRateUpgradeImages;

    [Header("Upgrade Texts")]
    public TMP_Text healthUpgradeLevel;
    public TMP_Text speedUpgradeLevel;
    public TMP_Text damageUpgradeLevel;
    public TMP_Text critRateUpgradeLevel;
    
    [Header("Price Texts")]
    public TMP_Text healthUpgradePriceText;
    public TMP_Text speedUpgradePriceText;
    public TMP_Text damageUpgradePriceText;
    public TMP_Text critRateUpgradePriceText;
    
    
    
    
    private PlayerCurrency playerCurrency;

    // ---- Unlockable upgrades ----
    [Header("Unlockables")]
    public bool isHoldToClickUnlocked = false;
    public bool isAbilityUnlocked = false;

    
    
    public void Start()
    {
        playerCurrency = GetComponent<PlayerCurrency>();
        LoadData(); // ---- Load saved data ----

        healthUpgradeImages.Start();
        speedUpgradeImages.Start();
        damageUpgradeImages.Start();
        Debug.Log(Application.persistentDataPath);

        UpdateUpgradeTexts();
        UpdateUpgradeImages();
        UpdateUpgradePrices();
    }

    public void IncrementUpgradesBought()
    {
        if (upgradesBought < totalUpgrades)
        {
            upgradesBought++;
        }
    }

    
    
    public int CalculateUpgradeCost(int baseCost, int currentTier)
    {
        return Mathf.RoundToInt(baseCost * Mathf.Pow(1.5f, currentTier));
    }
    
    // ---- Check if the player can buy an upgrade ----
    public bool CanBuyHealthUpgrade()
    {
        int cost = CalculateUpgradeCost(healthUpgradeCost, healthUpgradesBought);
        return healthUpgradesBought < maxHealthUpgrades && playerCurrency.GetCurrency() >= cost;
    }

    public bool CanBuySpeedUpgrade()
    {
        int cost = CalculateUpgradeCost(speedUpgradeCost, speedUpgradesBought);
        return speedUpgradesBought < maxSpeedUpgrades && playerCurrency.GetCurrency() >= cost;
    }

    public bool CanBuyDamageUpgrade()
    {
        int cost = CalculateUpgradeCost(damageUpgradeCost, damageUpgradesBought);
        return damageUpgradesBought < maxDamageUpgrades && playerCurrency.GetCurrency() >= cost;
    }

    // ---- Buy an upgrade ----
    public void BuyHealthUpgrade()
    {
        if (CanBuyHealthUpgrade())
        {
            int cost = CalculateUpgradeCost(healthUpgradeCost, healthUpgradesBought);
            playerCurrency.AddCurrency(-cost);
            healthUpgradesBought++;
            IncrementUpgradesBought();
            UpdateUpgradeTexts();
            UpdateUpgradeImages();
            UpdateUpgradePrices();
            SaveData();
        }
    }

    public void BuySpeedUpgrade()
    {
        if (CanBuySpeedUpgrade())
        {
            int cost = CalculateUpgradeCost(speedUpgradeCost, speedUpgradesBought);
            playerCurrency.AddCurrency(-cost);
            speedUpgradesBought++;
            IncrementUpgradesBought();
            UpdateUpgradeTexts();
            UpdateUpgradeImages();
            UpdateUpgradePrices();
            SaveData();
        }
    }

    public void BuyDamageUpgrade()
    {
        if (CanBuyDamageUpgrade())
        {
            int cost = CalculateUpgradeCost(damageUpgradeCost, damageUpgradesBought);
            playerCurrency.AddCurrency(-cost);
            damageUpgradesBought++;
            IncrementUpgradesBought();
            UpdateUpgradeTexts();
            UpdateUpgradeImages();
            UpdateUpgradePrices();
            SaveData();
        }
    }
    
    
    public bool CanBuyHoldToClickUpgrade()
    {
        return !isHoldToClickUnlocked && playerCurrency.GetCurrency() >= holdToClickUpgradeCost;
    }

    public bool CanBuyAbilityUpgrade()
    {
        return !isAbilityUnlocked && playerCurrency.GetCurrency() >= abilityUpgradeCost;
    }

    public void BuyHoldToClickUpgrade()
    {
        if (CanBuyHoldToClickUpgrade())
        {
            playerCurrency.AddCurrency(-holdToClickUpgradeCost);
            isHoldToClickUnlocked = true;
            SaveData();
        }
    }

    public void BuyAbilityUpgrade()
    {
        if (CanBuyAbilityUpgrade())
        {
            playerCurrency.AddCurrency(-abilityUpgradeCost);
            isAbilityUnlocked = true;
            SaveData();
        }
    }
    
    public bool CanBuyCritRateUpgrade()
    {
        int cost = CalculateUpgradeCost(critRateUpgradeCost, critRateUpgradesBought);
        return critRateUpgradesBought < maxCritRateUpgrades && playerCurrency.GetCurrency() >= cost;
    }

    public void BuyCritRateUpgrade()
    {
        if (CanBuyCritRateUpgrade())
        {
            int cost = CalculateUpgradeCost(critRateUpgradeCost, critRateUpgradesBought);
            playerCurrency.AddCurrency(-cost);
            critRateUpgradesBought++;
            IncrementUpgradesBought();
            UpdateUpgradeTexts();
            UpdateUpgradeImages();
            UpdateUpgradePrices();
            SaveData();
            
            PlayerGun playerGun = GetComponentInChildren<PlayerGun>();
            if (playerGun != null)
            {
                playerGun.IncreaseCritRate(0.05f); // ---- 5% increase in crit rate ----
            }
        }
    }
    
    // ---- Checks if player can afford when hovering the purchase button ----
    public bool CanAffordUpgrade(int cost)
    {
        return playerCurrency.GetCurrency() >= cost;
    }
    
    
    
    // ---- Update the upgrade texts ----
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
        
        if (critRateUpgradeLevel != null)
        {
            critRateUpgradeLevel.text = RomanNumeralConverter.ToRoman(critRateUpgradesBought);
        }
    }

    // ---- Update the upgrade prices ----
    private void UpdateUpgradePrices()
    {
        if (healthUpgradePriceText != null)
        {
            if (healthUpgradesBought >= maxHealthUpgrades)
            {
                healthUpgradePriceText.text = "MAX";
            }
            else
            {
                healthUpgradePriceText.text = CalculateUpgradeCost(healthUpgradeCost, healthUpgradesBought).ToString();
            }
        }
        if (speedUpgradePriceText != null)
        {
            if (speedUpgradesBought >= maxSpeedUpgrades)
            {
                speedUpgradePriceText.text = "MAX";
            }
            else
            {
                speedUpgradePriceText.text = CalculateUpgradeCost(speedUpgradeCost, speedUpgradesBought).ToString();
            }
        }
        if (damageUpgradePriceText != null)
        {
            if (damageUpgradesBought >= maxDamageUpgrades)
            {
                damageUpgradePriceText.text = "MAX";
            }
            else
            {
                damageUpgradePriceText.text = CalculateUpgradeCost(damageUpgradeCost, damageUpgradesBought).ToString();
            }
        }
        
        if (critRateUpgradePriceText != null)
        {
            if (critRateUpgradesBought >= maxCritRateUpgrades)
            {
                critRateUpgradePriceText.text = "MAX";
            }
            else
            {
                critRateUpgradePriceText.text = CalculateUpgradeCost(critRateUpgradeCost, critRateUpgradesBought).ToString();
            }
        }
    }
    // ---- Update the upgrade images ----
    private void UpdateUpgradeImages()
    {
        healthUpgradeImages.UpdateImages(healthUpgradesBought);
        speedUpgradeImages.UpdateImages(speedUpgradesBought);
        damageUpgradeImages.UpdateImages(damageUpgradesBought);
        critRateUpgradeImages.UpdateImages(critRateUpgradesBought);
    }
    
    // ---- Save the player data ----
    public void SaveData()
    {
        PlayerData data = new PlayerData
        {
            healthUpgradesBought = healthUpgradesBought,
            speedUpgradesBought = speedUpgradesBought,
            damageUpgradesBought = damageUpgradesBought,
            upgradesBought = upgradesBought,
            currency = GetComponent<PlayerCurrency>().currency,
            isHoldToClickUnlocked = isHoldToClickUnlocked,
            isAbilityUnlocked = isAbilityUnlocked, 
            critRateUpgradesBought = critRateUpgradesBought
        };

        string json = JsonUtility.ToJson(data);
        File.WriteAllText(Application.persistentDataPath + "/playerData.json", json);
    }

    // ---- Load the player data ----
    public void LoadData()
    {
        string path = Application.persistentDataPath + "/playerData.json";
        if (File.Exists(path))
        {
            string json = File.ReadAllText(path);
            PlayerData data = JsonUtility.FromJson<PlayerData>(json);

            healthUpgradesBought = data.healthUpgradesBought;
            speedUpgradesBought = data.speedUpgradesBought;
            damageUpgradesBought = data.damageUpgradesBought;
            upgradesBought = data.upgradesBought;
            GetComponent<PlayerCurrency>().currency = data.currency;
            isHoldToClickUnlocked = data.isHoldToClickUnlocked;
            isAbilityUnlocked = data.isAbilityUnlocked;
            critRateUpgradesBought = data.critRateUpgradesBought;
            ApplyUpgrades();
            
            
            UpdateUpgradeTexts();
            UpdateUpgradeImages();
            UpdateUpgradePrices();
        }
    }
    
    
    // ---- Apply the upgrades to the player ----
    private void ApplyUpgrades()
    {
        PlayerHealth playerHealth = GetComponent<PlayerHealth>();
        PlayerMovement playerMovement = GetComponent<PlayerMovement>();
        PlayerGun playerGun = GetComponentInChildren<PlayerGun>();

        UpdateUpgradeTexts();
        UpdateUpgradeImages();
        
        if (playerHealth != null)
        {
            playerHealth.IncreaseMaxHealth(healthUpgradesBought * 25);
        }

        if (playerMovement != null)
        {
            playerMovement.IncreaseSpeed(speedUpgradesBought * 1f); 
        }

        if (playerGun != null)
        {
            playerGun.IncreaseDamage(damageUpgradesBought * 8); 
        }
        
        if (playerGun != null)
        {
            playerGun.IncreaseCritRate(critRateUpgradesBought * 0.05f); // Apply crit rate upgrade
        }
    }
    
    
    
    
}