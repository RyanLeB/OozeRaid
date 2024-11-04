using UnityEngine;
using TMPro;
using System.IO;


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
    
    
    public TMP_Text healthUpgradePriceText;
    public TMP_Text speedUpgradePriceText;
    public TMP_Text damageUpgradePriceText;
    

    private PlayerCurrency playerCurrency;

    
    
    
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
    }
    
    private void UpdateUpgradeImages()
    {
        healthUpgradeImages.UpdateImages(healthUpgradesBought);
        speedUpgradeImages.UpdateImages(speedUpgradesBought);
        damageUpgradeImages.UpdateImages(damageUpgradesBought);
    }
    
    
    public void SaveData()
    {
        PlayerData data = new PlayerData
        {
            healthUpgradesBought = healthUpgradesBought,
            speedUpgradesBought = speedUpgradesBought,
            damageUpgradesBought = damageUpgradesBought,
            upgradesBought = upgradesBought,
            currency = GetComponent<PlayerCurrency>().currency
        };

        string json = JsonUtility.ToJson(data);
        File.WriteAllText(Application.persistentDataPath + "/playerData.json", json);
    }

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
            
            ApplyUpgrades();
            
            
            UpdateUpgradeTexts();
            UpdateUpgradeImages();
            UpdateUpgradePrices();
        }
    }
    
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
    }
    
    
    
    
}