using UnityEngine;
using UnityEngine.UI;
using TMPro;

/// <summary>
/// Equipment selection panel controller.
/// Allows players to select and equip different equipment (bats, power-ups, etc.)
/// </summary>
public class EquipmentUI : MonoBehaviour
{
    [Header("UI References")]
    [SerializeField] private Button backButton;
    [SerializeField] private Transform equipmentGridContainer;
    [SerializeField] private GameObject equipmentItemPrefab;
    [SerializeField] private Image previewImage;
    [SerializeField] private TextMeshProUGUI equipmentNameText;
    [SerializeField] private TextMeshProUGUI equipmentDescriptionText;
    [SerializeField] private TextMeshProUGUI statsText;
    [SerializeField] private Button equipButton;

    [Header("Category Tabs")]
    [SerializeField] private Button batsTabButton;
    [SerializeField] private Button powerUpsTabButton;
    [SerializeField] private Button skinsTabButton;

    [Header("Equipment Data")]
    [SerializeField] private EquipmentData[] bats;
    [SerializeField] private EquipmentData[] powerUps;
    [SerializeField] private EquipmentData[] skins;

    private EquipmentCategory currentCategory = EquipmentCategory.Bats;
    private int selectedItemIndex = -1;
    private int[] equippedItems = new int[3]; // One for each category

    public enum EquipmentCategory
    {
        Bats = 0,
        PowerUps = 1,
        Skins = 2
    }

    [System.Serializable]
    public class EquipmentData
    {
        public string equipmentName;
        public string description;
        public Sprite icon;
        public float powerBonus = 0f;
        public float speedBonus = 0f;
        public bool isUnlocked = true;
        public int unlockCost = 0;
    }

    private void Start()
    {
        SetupButtons();
        LoadEquippedItems();
        SwitchCategory(EquipmentCategory.Bats);
    }

    private void SetupButtons()
    {
        if (backButton != null)
            backButton.onClick.AddListener(OnBackClicked);

        if (equipButton != null)
            equipButton.onClick.AddListener(OnEquipClicked);

        if (batsTabButton != null)
            batsTabButton.onClick.AddListener(() => SwitchCategory(EquipmentCategory.Bats));

        if (powerUpsTabButton != null)
            powerUpsTabButton.onClick.AddListener(() => SwitchCategory(EquipmentCategory.PowerUps));

        if (skinsTabButton != null)
            skinsTabButton.onClick.AddListener(() => SwitchCategory(EquipmentCategory.Skins));
    }

    private void LoadEquippedItems()
    {
        equippedItems[0] = PlayerPrefs.GetInt("EquippedBat", 0);
        equippedItems[1] = PlayerPrefs.GetInt("EquippedPowerUp", -1); // -1 means none
        equippedItems[2] = PlayerPrefs.GetInt("EquippedSkin", 0);
    }

    private void SaveEquippedItems()
    {
        PlayerPrefs.SetInt("EquippedBat", equippedItems[0]);
        PlayerPrefs.SetInt("EquippedPowerUp", equippedItems[1]);
        PlayerPrefs.SetInt("EquippedSkin", equippedItems[2]);
        PlayerPrefs.Save();
    }

    private void SwitchCategory(EquipmentCategory category)
    {
        currentCategory = category;
        selectedItemIndex = -1;

        // Update tab visuals
        UpdateTabVisuals();

        // Generate items for selected category
        GenerateEquipmentItems();

        // Select currently equipped item
        int equippedIndex = equippedItems[(int)category];
        if (equippedIndex >= 0)
        {
            SelectItem(equippedIndex);
        }
    }

    private void UpdateTabVisuals()
    {
        Color activeColor = Color.white;
        Color inactiveColor = new Color(0.7f, 0.7f, 0.7f);

        if (batsTabButton != null)
        {
            batsTabButton.GetComponent<Image>().color = 
                currentCategory == EquipmentCategory.Bats ? activeColor : inactiveColor;
        }

        if (powerUpsTabButton != null)
        {
            powerUpsTabButton.GetComponent<Image>().color = 
                currentCategory == EquipmentCategory.PowerUps ? activeColor : inactiveColor;
        }

        if (skinsTabButton != null)
        {
            skinsTabButton.GetComponent<Image>().color = 
                currentCategory == EquipmentCategory.Skins ? activeColor : inactiveColor;
        }
    }

    private EquipmentData[] GetCurrentCategoryData()
    {
        switch (currentCategory)
        {
            case EquipmentCategory.Bats: return bats;
            case EquipmentCategory.PowerUps: return powerUps;
            case EquipmentCategory.Skins: return skins;
            default: return bats;
        }
    }

    private void GenerateEquipmentItems()
    {
        if (equipmentGridContainer == null || equipmentItemPrefab == null)
        {
            Debug.LogWarning("EquipmentUI: Missing required references!");
            return;
        }

        // Clear existing items
        foreach (Transform child in equipmentGridContainer)
        {
            Destroy(child.gameObject);
        }

        EquipmentData[] items = GetCurrentCategoryData();
        if (items == null) return;

        int equippedIndex = equippedItems[(int)currentCategory];

        for (int i = 0; i < items.Length; i++)
        {
            GameObject itemObj = Instantiate(equipmentItemPrefab, equipmentGridContainer);
            itemObj.name = $"Equipment_{items[i].equipmentName}";

            int itemIndex = i; // Capture for closure

            // Setup button
            Button button = itemObj.GetComponent<Button>();
            if (button != null)
            {
                button.onClick.AddListener(() => SelectItem(itemIndex));
            }

            // Setup icon
            Image iconImage = itemObj.transform.Find("Icon")?.GetComponent<Image>();
            if (iconImage != null && items[i].icon != null)
            {
                iconImage.sprite = items[i].icon;
            }

            // Setup name
            TextMeshProUGUI nameText = itemObj.GetComponentInChildren<TextMeshProUGUI>();
            if (nameText != null)
            {
                nameText.text = items[i].equipmentName;
            }

            // Check unlock status
            bool isUnlocked = IsItemUnlocked(i);
            button.interactable = isUnlocked;

            // Show equipped indicator
            if (i == equippedIndex)
            {
                Transform equippedIndicator = itemObj.transform.Find("EquippedIndicator");
                if (equippedIndicator != null)
                {
                    equippedIndicator.gameObject.SetActive(true);
                }
            }

            // Visual feedback for locked items
            if (!isUnlocked)
            {
                Image itemImage = itemObj.GetComponent<Image>();
                if (itemImage != null)
                {
                    itemImage.color = new Color(0.5f, 0.5f, 0.5f, 0.7f);
                }

                Transform lockIcon = itemObj.transform.Find("LockIcon");
                if (lockIcon != null)
                {
                    lockIcon.gameObject.SetActive(true);
                }
            }
        }
    }

    private bool IsItemUnlocked(int itemIndex)
    {
        EquipmentData[] items = GetCurrentCategoryData();
        if (items == null || itemIndex >= items.Length) return false;

        if (itemIndex == 0) return true; // Default items always unlocked
        if (items[itemIndex].isUnlocked) return true;

        string key = $"Equipment_{currentCategory}_{itemIndex}_Unlocked";
        return PlayerPrefs.GetInt(key, 0) == 1;
    }

    private void SelectItem(int itemIndex)
    {
        EquipmentData[] items = GetCurrentCategoryData();
        if (items == null || itemIndex < 0 || itemIndex >= items.Length) return;

        selectedItemIndex = itemIndex;
        EquipmentData item = items[itemIndex];

        // Update preview
        if (previewImage != null && item.icon != null)
        {
            previewImage.sprite = item.icon;
        }

        if (equipmentNameText != null)
        {
            equipmentNameText.text = item.equipmentName;
        }

        if (equipmentDescriptionText != null)
        {
            equipmentDescriptionText.text = item.description;
        }

        if (statsText != null)
        {
            string stats = "";
            if (item.powerBonus != 0) stats += $"Power: +{item.powerBonus:F0}%\n";
            if (item.speedBonus != 0) stats += $"Speed: +{item.speedBonus:F0}%";
            statsText.text = stats;
        }

        // Update equip button
        if (equipButton != null)
        {
            bool isEquipped = itemIndex == equippedItems[(int)currentCategory];
            equipButton.interactable = !isEquipped;

            TextMeshProUGUI buttonText = equipButton.GetComponentInChildren<TextMeshProUGUI>();
            if (buttonText != null)
            {
                buttonText.text = isEquipped ? "Equipped" : "Equip";
            }
        }
    }

    private void OnEquipClicked()
    {
        if (selectedItemIndex < 0) return;

        equippedItems[(int)currentCategory] = selectedItemIndex;
        SaveEquippedItems();

        // Refresh UI
        GenerateEquipmentItems();
        SelectItem(selectedItemIndex);

        EquipmentData[] items = GetCurrentCategoryData();
        if (items != null && selectedItemIndex < items.Length)
        {
            Debug.Log($"Equipped: {items[selectedItemIndex].equipmentName}");
        }
    }

    private void OnBackClicked()
    {
        UIManager.Instance.ShowMainMenu();
    }

    private void OnDestroy()
    {
        if (backButton != null) backButton.onClick.RemoveAllListeners();
        if (equipButton != null) equipButton.onClick.RemoveAllListeners();
        if (batsTabButton != null) batsTabButton.onClick.RemoveAllListeners();
        if (powerUpsTabButton != null) powerUpsTabButton.onClick.RemoveAllListeners();
        if (skinsTabButton != null) skinsTabButton.onClick.RemoveAllListeners();
    }

    // Static methods for accessing equipped items from other scripts
    public static int GetEquippedBat() => PlayerPrefs.GetInt("EquippedBat", 0);
    public static int GetEquippedPowerUp() => PlayerPrefs.GetInt("EquippedPowerUp", -1);
    public static int GetEquippedSkin() => PlayerPrefs.GetInt("EquippedSkin", 0);
}
