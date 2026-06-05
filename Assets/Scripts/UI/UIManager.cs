using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// Singleton UI Manager that handles panel transitions and general UI state.
/// Now supports PanelAnimator for smooth transitions.
/// </summary>
public class UIManager : MonoBehaviour
{
    public static UIManager Instance { get; private set; }

    [Header("Panels")]
    [SerializeField] private GameObject mainMenuPanel;
    [SerializeField] private GameObject levelSelectPanel;
    [SerializeField] private GameObject ballSelectPanel;
    [SerializeField] private GameObject equipmentPanel;
    [SerializeField] private GameObject settingsPanel;

    [Header("Animation Settings")]
    [SerializeField] private bool useAnimations = true;
    [SerializeField] private float panelFadeDuration = 0.3f;

    private GameObject currentActivePanel;
    private PanelAnimator currentAnimator;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
            return;
        }
    }

    private void Start()
    {
        // Hide all panels initially (without animation)
        HideAllPanelsInstant();
        
        // Show main menu with animation
        ShowMainMenu();
    }

    public void ShowMainMenu()
    {
        SwitchToPanel(mainMenuPanel);
    }

    public void ShowLevelSelect()
    {
        SwitchToPanel(levelSelectPanel);
    }

    public void ShowBallSelect()
    {
        SwitchToPanel(ballSelectPanel);
    }

    public void ShowEquipment()
    {
        SwitchToPanel(equipmentPanel);
    }

    public void ShowSettings()
    {
        SwitchToPanel(settingsPanel);
    }

    private void SwitchToPanel(GameObject targetPanel)
    {
        if (targetPanel == null) return;

        // If same panel, do nothing
        if (currentActivePanel == targetPanel) return;

        // Hide current panel
        if (currentActivePanel != null)
        {
            HidePanel(currentActivePanel);
        }

        // Show new panel
        ShowPanel(targetPanel);
    }

    private void ShowPanel(GameObject panel)
    {
        if (panel == null) return;

        currentActivePanel = panel;

        if (useAnimations)
        {
            PanelAnimator animator = panel.GetComponent<PanelAnimator>();
            if (animator != null)
            {
                currentAnimator = animator;
                animator.Show();
                return;
            }
        }

        // Fallback: just enable
        panel.SetActive(true);
    }

    private void HidePanel(GameObject panel)
    {
        if (panel == null) return;

        if (useAnimations)
        {
            PanelAnimator animator = panel.GetComponent<PanelAnimator>();
            if (animator != null)
            {
                animator.Hide();
                return;
            }
        }

        // Fallback: just disable
        panel.SetActive(false);
    }

    private void HideAllPanelsInstant()
    {
        HidePanelInstant(mainMenuPanel);
        HidePanelInstant(levelSelectPanel);
        HidePanelInstant(ballSelectPanel);
        HidePanelInstant(equipmentPanel);
        HidePanelInstant(settingsPanel);
    }

    private void HidePanelInstant(GameObject panel)
    {
        if (panel == null) return;

        PanelAnimator animator = panel.GetComponent<PanelAnimator>();
        if (animator != null)
        {
            animator.HideInstant();
        }
        else
        {
            panel.SetActive(false);
        }
    }

    public void LoadLevel(int levelIndex)
    {
        SceneManager.LoadScene(levelIndex);
    }

    public void LoadLevel(string levelName)
    {
        SceneManager.LoadScene(levelName);
    }

    public void QuitGame()
    {
        #if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
        #else
        Application.Quit();
        #endif
    }
}
