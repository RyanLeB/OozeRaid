using UnityEngine;
using UnityEngine.UI;
using TMPro;

using System.Collections;

public class UpgradeManager : MonoBehaviour
{
    // ---- Buttons for each upgrade type ----
    public Button healthUpgradeButton;
    public Button speedUpgradeButton;
    public Button damageUpgradeButton;
    public Button holdToClickUpgradeButton;
    public Button abilityUpgradeButton;
    public Button critRateUpgradeButton;

    public TMP_Text hoverMessageText;

    public GameObject upgradePanel;
    
    // ---- References to other scripts ----
    public PlayerUpgrades playerUpgrades;

    void Start()
    {
        // ---- Add listeners to the buttons ----
        healthUpgradeButton.onClick.AddListener(OnHealthUpgradeButtonClicked);
        speedUpgradeButton.onClick.AddListener(OnSpeedUpgradeButtonClicked);
        damageUpgradeButton.onClick.AddListener(OnDamageUpgradeButtonClicked);
        holdToClickUpgradeButton.onClick.AddListener(OnHoldToClickUpgradeButtonClicked);
        abilityUpgradeButton.onClick.AddListener(OnAbilityUpgradeButtonClicked);
        critRateUpgradeButton.onClick.AddListener(OnCritRateUpgradeButtonClicked);
    }

    
    void Update()
    {
        if (upgradePanel.activeSelf)
        {
            UpdateButtonTexts();
            UpdateUpgradeTexts();
            UpdateUpgradeImages();
            UpdateUpgradePrices();
        }
    }
    
    
    
    
    public void OnHealthUpgradeButtonClicked()
    {
        if (playerUpgrades.CanBuyHealthUpgrade())
        {
            playerUpgrades.BuyHealthUpgrade();
            hoverMessageText.text = "";
        }
        else
        {
            hoverMessageText.text = "CANNOT PURCHASE";
            StartCoroutine(ResetHoverMessageText());
        }
        UpdateButtonTexts();
    }

    public void OnSpeedUpgradeButtonClicked()
    {
        if (playerUpgrades.CanBuySpeedUpgrade())
        {
            playerUpgrades.BuySpeedUpgrade();
            hoverMessageText.text = "";
        }
        else
        {
            hoverMessageText.text = "CANNOT PURCHASE";
            StartCoroutine(ResetHoverMessageText());
        }
        UpdateButtonTexts();
    }

    public void OnDamageUpgradeButtonClicked()
    {
        if (playerUpgrades.CanBuyDamageUpgrade())
        {
            playerUpgrades.BuyDamageUpgrade();
            hoverMessageText.text = "";
        }
        else
        {
            hoverMessageText.text = "CANNOT PURCHASE";
            StartCoroutine(ResetHoverMessageText());
        }
        UpdateButtonTexts();
    }

    public void OnHoldToClickUpgradeButtonClicked()
    {
        if (playerUpgrades.CanBuyHoldToClickUpgrade())
        {
            playerUpgrades.BuyHoldToClickUpgrade();
            hoverMessageText.text = "";
        }
        else
        {
            hoverMessageText.text = "CANNOT PURCHASE";
            StartCoroutine(ResetHoverMessageText());
        }
        UpdateButtonTexts();
    }

    public void OnAbilityUpgradeButtonClicked()
    {
        if (playerUpgrades.CanBuyAbilityUpgrade())
        {
            playerUpgrades.BuyAbilityUpgrade();
            hoverMessageText.text = "";
        }
        else
        {
            hoverMessageText.text = "CANNOT PURCHASE";
            StartCoroutine(ResetHoverMessageText());
        }
        UpdateButtonTexts();
    }

    public void OnCritRateUpgradeButtonClicked()
    {
        if (playerUpgrades.CanBuyCritRateUpgrade())
        {
            playerUpgrades.BuyCritRateUpgrade();
            hoverMessageText.text = "";
        }
        else
        {
            hoverMessageText.text = "CANNOT PURCHASE";
            StartCoroutine(ResetHoverMessageText());
        }
        UpdateButtonTexts();
    }

    private IEnumerator ResetHoverMessageText()
    {
        yield return new WaitForSeconds(2f);
        hoverMessageText.text = "";
    }
    
    private void UpdateButtonTexts()
    {
        if (playerUpgrades.healthUpgradesBought >= playerUpgrades.maxHealthUpgrades)
        {
            healthUpgradeButton.GetComponentInChildren<TMP_Text>().text = "MAX";
        }
        if (playerUpgrades.speedUpgradesBought >= playerUpgrades.maxSpeedUpgrades)
        {
            speedUpgradeButton.GetComponentInChildren<TMP_Text>().text = "MAX";
        }
        if (playerUpgrades.damageUpgradesBought >= playerUpgrades.maxDamageUpgrades)
        {
            damageUpgradeButton.GetComponentInChildren<TMP_Text>().text = "MAX";
        }
        if (playerUpgrades.isHoldToClickUnlocked)
        {
            holdToClickUpgradeButton.GetComponentInChildren<TMP_Text>().text = "MAX";
        }
        if (playerUpgrades.isAbilityUnlocked)
        {
            abilityUpgradeButton.GetComponentInChildren<TMP_Text>().text = "MAX";
        }
        if (playerUpgrades.critRateUpgradesBought >= playerUpgrades.maxCritRateUpgrades)
        {
            critRateUpgradeButton.GetComponentInChildren<TMP_Text>().text = "MAX";
        }
    }
    
    private void UpdateUpgradeTexts()
    {
        if (playerUpgrades != null)
        {
            playerUpgrades.UpdateUpgradeTexts();
        }
    }

    private void UpdateUpgradeImages()
    {
        if (playerUpgrades != null)
        {
            playerUpgrades.UpdateUpgradeImages();
        }
    }

    private void UpdateUpgradePrices()
    {
        if (playerUpgrades != null)
        {
            playerUpgrades.UpdateUpgradePrices();
        }
    }
    
}