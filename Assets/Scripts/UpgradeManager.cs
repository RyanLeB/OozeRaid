using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UpgradeManager : MonoBehaviour
{
    public Button healthUpgradeButton; 
    public Button speedUpgradeButton; 
    public Button damageUpgradeButton;

    public Slider progressBar; 
    public int totalUpgrades = 10; 
    private int upgradesBought = 0; 

    public PlayerHealth playerHealth; 
    public PlayerMovement playerMovement; 
    public PlayerGun playerGun; 

    void Start()
    {
        // Initialize the progress bar
        progressBar.maxValue = totalUpgrades;
        progressBar.value = upgradesBought;

        // Add listeners to the buttons
        healthUpgradeButton.onClick.AddListener(OnHealthUpgradeButtonClicked);
        speedUpgradeButton.onClick.AddListener(OnSpeedUpgradeButtonClicked);
        damageUpgradeButton.onClick.AddListener(OnDamageUpgradeButtonClicked);
    }

    void OnHealthUpgradeButtonClicked()
    {
        if (upgradesBought < totalUpgrades)
        {
            upgradesBought++;
            progressBar.value = upgradesBought;
            playerHealth.IncreaseMaxHealth(25);
        }
    }

    void OnSpeedUpgradeButtonClicked()
    {
        if (upgradesBought < totalUpgrades)
        {
            upgradesBought++;
            progressBar.value = upgradesBought;
            playerMovement.IncreaseSpeed(1f);
        }
    }

    void OnDamageUpgradeButtonClicked()
    {
        if (upgradesBought < totalUpgrades)
        {
            upgradesBought++;
            progressBar.value = upgradesBought;
            playerGun.IncreaseDamage(5);
        }
    }
}

