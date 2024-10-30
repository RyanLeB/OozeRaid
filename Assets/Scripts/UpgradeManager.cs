using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UpgradeManager : MonoBehaviour
{
    // ---- Buttons ----
    
    public Button healthUpgradeButton; 
    public Button speedUpgradeButton; 
    public Button damageUpgradeButton;
    
    
    // ---- Progress bar & Upgrades bought---- 
    
    public Slider progressBar; 
    public int totalUpgrades = 10; 
    private int upgradesBought = 0; 
    
    
    // ---- Player references ----
    
    public PlayerHealth playerHealth; 
    public PlayerMovement playerMovement; 
    public PlayerGun playerGun; 

    void Start()
    {
        // ---- Initialize the progress bar ----
        progressBar.maxValue = totalUpgrades;
        progressBar.value = upgradesBought;

        //  ---- Button listeners ---- 
        healthUpgradeButton.onClick.AddListener(OnHealthUpgradeButtonClicked);
        speedUpgradeButton.onClick.AddListener(OnSpeedUpgradeButtonClicked);
        damageUpgradeButton.onClick.AddListener(OnDamageUpgradeButtonClicked);
    }

    
    // ---- Increase player health ----
    void OnHealthUpgradeButtonClicked()
    {
        if (upgradesBought < totalUpgrades)
        {
            upgradesBought++;
            progressBar.value = upgradesBought;
            playerHealth.IncreaseMaxHealth(25);
        }
    }

    
    
    // ---- Increase player speed ----
    void OnSpeedUpgradeButtonClicked()
    {
        if (upgradesBought < totalUpgrades)
        {
            upgradesBought++;
            progressBar.value = upgradesBought;
            playerMovement.IncreaseSpeed(1f);
        }
    }

    
    
    // ---- Increase player damage ----
    void OnDamageUpgradeButtonClicked()
    {
        if (upgradesBought < totalUpgrades)
        {
            upgradesBought++;
            progressBar.value = upgradesBought;
            playerGun.IncreaseDamage(10);
        }
    }
}

