using UnityEngine;
using UnityEngine.UI;
using TMPro;

/// <summary>
/// Ball selection panel controller.
/// Allows players to select different ball types.
/// </summary>
public class BallSelectUI : MonoBehaviour
{
    [Header("UI References")]
    [SerializeField] private Button backButton;
    [SerializeField] private Transform ballGridContainer;
    [SerializeField] private GameObject ballItemPrefab;
    [SerializeField] private Image previewImage;
    [SerializeField] private TextMeshProUGUI ballNameText;
    [SerializeField] private TextMeshProUGUI ballDescriptionText;
    [SerializeField] private Button selectButton;

    [Header("Ball Data")]
    [SerializeField] private BallData[] availableBalls;

    private int selectedBallIndex = -1;
    private int currentlyEquippedIndex = 0;

    [System.Serializable]
    public class BallData
    {
        public string ballName;
        public string description;
        public Sprite icon;
        public Color ballColor = Color.white;
        public bool isUnlocked = true;
        public int unlockCost = 0;
    }

    private void Start()
    {
        if (backButton != null)
            backButton.onClick.AddListener(OnBackClicked);

        if (selectButton != null)
            selectButton.onClick.AddListener(OnSelectClicked);

        // Load currently equipped ball
        currentlyEquippedIndex = PlayerPrefs.GetInt("EquippedBall", 0);

        GenerateBallItems();
        
        // Select the currently equipped ball by default
        if (availableBalls != null && availableBalls.Length > 0)
        {
            SelectBall(currentlyEquippedIndex);
        }
    }

    private void GenerateBallItems()
    {
        if (ballGridContainer == null || ballItemPrefab == null || availableBalls == null)
        {
            Debug.LogWarning("BallSelectUI: Missing required references!");
            return;
        }

        // Clear existing items
        foreach (Transform child in ballGridContainer)
        {
            Destroy(child.gameObject);
        }

        // Generate ball items
        for (int i = 0; i < availableBalls.Length; i++)
        {
            GameObject itemObj = Instantiate(ballItemPrefab, ballGridContainer);
            itemObj.name = $"Ball_{availableBalls[i].ballName}";

            int ballIndex = i; // Capture for closure

            // Setup button
            Button button = itemObj.GetComponent<Button>();
            if (button != null)
            {
                button.onClick.AddListener(() => SelectBall(ballIndex));
            }

            // Setup icon
            Image iconImage = itemObj.transform.Find("Icon")?.GetComponent<Image>();
            Debug.Log($"[BallSelectUI] Ball {i}: {availableBalls[i].ballName}, Icon: {(availableBalls[i].icon != null ? availableBalls[i].icon.name : "NULL")}, IconImage found: {iconImage != null}");
            
            if (iconImage != null && availableBalls[i].icon != null)
            {
                iconImage.sprite = availableBalls[i].icon;
                iconImage.color = availableBalls[i].ballColor;
                Debug.Log($"[BallSelectUI] Set sprite for {availableBalls[i].ballName} to {iconImage.sprite.name}");
            }
            else if (iconImage == null)
            {
                Debug.LogError($"[BallSelectUI] Could not find 'Icon' child in prefab for {availableBalls[i].ballName}");
            }
            else if (availableBalls[i].icon == null)
            {
                Debug.LogError($"[BallSelectUI] Icon is NULL for {availableBalls[i].ballName} - check Inspector!");
            }

            // Setup name text
            TextMeshProUGUI nameText = itemObj.GetComponentInChildren<TextMeshProUGUI>();
            if (nameText != null)
            {
                nameText.text = availableBalls[i].ballName;
            }

            // Check if ball is unlocked
            bool isUnlocked = IsBallUnlocked(ballIndex);
            button.interactable = isUnlocked;

            // Show equipped indicator
            if (ballIndex == currentlyEquippedIndex)
            {
                Transform equippedIndicator = itemObj.transform.Find("EquippedIndicator");
                if (equippedIndicator != null)
                {
                    equippedIndicator.gameObject.SetActive(true);
                }
            }

            // Visual feedback for locked balls
            if (!isUnlocked)
            {
                Image itemImage = itemObj.GetComponent<Image>();
                if (itemImage != null)
                {
                    itemImage.color = new Color(0.5f, 0.5f, 0.5f, 0.7f);
                }

                // Show lock icon
                Transform lockIcon = itemObj.transform.Find("LockIcon");
                if (lockIcon != null)
                {
                    lockIcon.gameObject.SetActive(true);
                }
            }
        }
    }

    private bool IsBallUnlocked(int ballIndex)
    {
        if (ballIndex == 0) return true; // Default ball is always unlocked
        if (availableBalls[ballIndex].isUnlocked) return true;
        return PlayerPrefs.GetInt($"Ball_{ballIndex}_Unlocked", 0) == 1;
    }

    private void SelectBall(int ballIndex)
    {
        if (ballIndex < 0 || ballIndex >= availableBalls.Length) return;

        selectedBallIndex = ballIndex;
        BallData ball = availableBalls[ballIndex];

        // Update preview
        if (previewImage != null && ball.icon != null)
        {
            previewImage.sprite = ball.icon;
            previewImage.color = ball.ballColor;
        }

        if (ballNameText != null)
        {
            ballNameText.text = ball.ballName;
        }

        if (ballDescriptionText != null)
        {
            ballDescriptionText.text = ball.description;
        }

        // Update select button
        if (selectButton != null)
        {
            bool isEquipped = ballIndex == currentlyEquippedIndex;
            selectButton.interactable = !isEquipped;
            
            TextMeshProUGUI buttonText = selectButton.GetComponentInChildren<TextMeshProUGUI>();
            if (buttonText != null)
            {
                buttonText.text = isEquipped ? "Equipped" : "Select";
            }
        }
    }

    private void OnSelectClicked()
    {
        if (selectedBallIndex < 0 || selectedBallIndex >= availableBalls.Length) return;

        // Equip the selected ball
        currentlyEquippedIndex = selectedBallIndex;
        PlayerPrefs.SetInt("EquippedBall", currentlyEquippedIndex);
        PlayerPrefs.Save();

        // Refresh the UI
        GenerateBallItems();
        SelectBall(selectedBallIndex);

        Debug.Log($"Equipped ball: {availableBalls[currentlyEquippedIndex].ballName}");
    }

    private void OnBackClicked()
    {
        UIManager.Instance.ShowMainMenu();
    }

    private void OnDestroy()
    {
        if (backButton != null) backButton.onClick.RemoveAllListeners();
        if (selectButton != null) selectButton.onClick.RemoveAllListeners();
    }

    /// <summary>
    /// Get the currently equipped ball index
    /// </summary>
    public static int GetEquippedBallIndex()
    {
        return PlayerPrefs.GetInt("EquippedBall", 0);
    }

    /// <summary>
    /// Unlock a ball
    /// </summary>
    public static void UnlockBall(int ballIndex)
    {
        PlayerPrefs.SetInt($"Ball_{ballIndex}_Unlocked", 1);
        PlayerPrefs.Save();
    }
}
