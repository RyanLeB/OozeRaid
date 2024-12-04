using System.Collections;

// ---- PlayerData class to store player data ----
    [System.Serializable]
    public class PlayerData
    {
        public int healthUpgradesBought;
        public int speedUpgradesBought;
        public int damageUpgradesBought;
        public int upgradesBought;
        public int currency;
        public bool isHoldToClickUnlocked;
        public bool isAbilityUnlocked;
        public int critRateUpgradesBought;
        public bool isExtremeModeUnlocked;
    }
