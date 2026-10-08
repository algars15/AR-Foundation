using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;

public class GameUIManager : MonoBehaviour
{
    public static GameUIManager Instance { get; private set; }

    [Header("Configuration")]
    [Tooltip("Si es cert, les instruccions es mostren primer i bloquegen tot fins premer Entes")]
    [SerializeField] private bool showInstructionsOnStart = true;

    [Header("Panels")]
    [SerializeField] public GameObject mainMenuPanel;
    [SerializeField] public GameObject instructionsPanel;
    [SerializeField] public GameObject gameHudPanel;
    [SerializeField] public GameObject gameOverPanel;

    [Header("Main Menu Controls")]
    [SerializeField] public Button playButton;
    [SerializeField] public Button instructionsButton;
    [SerializeField] public Button quitButton;

    [Header("Instructions Controls")]
    [SerializeField] public Button instructionsCloseButton;

    [Header("Game HUD Elements")]
    [SerializeField] public TextMeshProUGUI healthText;
    [SerializeField] public Slider healthSlider;
    [SerializeField] public TextMeshProUGUI roundText;
    [SerializeField] public TextMeshProUGUI enemiesText;
    [SerializeField] public TextMeshProUGUI statusPromptText;
    [SerializeField] public Button hudResetButton;
    [SerializeField] public Button hudHelpButton;

    [Header("Game Over / Victory Elements")]
    [SerializeField] public TextMeshProUGUI endTitleText;
    [SerializeField] public TextMeshProUGUI endSubtitleText;
    [SerializeField] public TextMeshProUGUI endStatsText;
    [SerializeField] public Image endBannerBackground;
    [SerializeField] public Button endRestartButton;
    [SerializeField] public Button endMainMenuButton;

    [Header("Colors (Opcional)")]
    [SerializeField] private Color victoryColor = new Color(0.2f, 0.8f, 0.3f, 1f);
    [SerializeField] private Color defeatColor = new Color(0.9f, 0.2f, 0.2f, 1f);

    [Header("Audio (Opcional)")]
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private AudioClip victorySfx;
    [SerializeField] private AudioClip defeatSfx;
    [SerializeField] private AudioClip buttonClickSfx;

    private CastleBehavior activeCastle;
    private int totalEnemiesDefeated = 0;
    private bool isInitialInstructions = true;
    private GameObject previousPanelBeforeInstructions;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;

        // Cerca i vinculacio automatica per si falten referencies a l'Inspector
        AutoFindReferences();
    }

    private void Start()
    {
        // Desactiva elements de plantilles que puguin bloquejar raycasts
        DisableBlockingTemplateObjects();

        // Assegura que tots els botons tenen listeners
        SetupButtonListeners();

        if (showInstructionsOnStart && instructionsPanel != null)
        {
            isInitialInstructions = true;
            OpenInstructions();
        }
        else
        {
            isInitialInstructions = false;
            ShowMainMenu();
        }
    }

    private void DisableBlockingTemplateObjects()
    {
        Transform greeting = transform.Find("Greeting Prompt");
        if (greeting != null)
        {
            greeting.gameObject.SetActive(false);
        }
    }

    /// <summary>
    /// Auto-detecta panells, textos i botons en la jerarquia del Canvas si no estan assignats a l'Inspector.
    /// </summary>
    public void AutoFindReferences()
    {
        Transform canvasTr = transform;

        // 1. Panells
        if (mainMenuPanel == null)
        {
            Transform t = canvasTr.Find("MainMenuPanel");
            if (t != null) mainMenuPanel = t.gameObject;
        }
        if (instructionsPanel == null)
        {
            Transform t = canvasTr.Find("InstructionsPanel");
            if (t != null) instructionsPanel = t.gameObject;
        }
        if (gameHudPanel == null)
        {
            Transform t = canvasTr.Find("GameHudPanel");
            if (t != null) gameHudPanel = t.gameObject;
        }
        if (gameOverPanel == null)
        {
            Transform t = canvasTr.Find("GameOverPanel");
            if (t != null) gameOverPanel = t.gameObject;
        }

        // 2. Botons del Menu Principal
        if (mainMenuPanel != null)
        {
            if (playButton == null)
            {
                Transform t = mainMenuPanel.transform.Find("PlayButton");
                if (t != null) playButton = t.GetComponent<Button>();
            }
            if (instructionsButton == null)
            {
                Transform t = mainMenuPanel.transform.Find("InstructionsButton");
                if (t != null) instructionsButton = t.GetComponent<Button>();
            }
            if (quitButton == null)
            {
                Transform t = mainMenuPanel.transform.Find("QuitButton");
                if (t != null) quitButton = t.GetComponent<Button>();
            }
        }

        // 3. Boto de tancar instruccions
        if (instructionsPanel != null && instructionsCloseButton == null)
        {
            instructionsCloseButton = instructionsPanel.GetComponentInChildren<Button>(true);
        }

        // 4. Elements del HUD
        if (gameHudPanel != null)
        {
            if (hudResetButton == null)
            {
                Transform t = gameHudPanel.transform.Find("ResetButton");
                if (t != null) hudResetButton = t.GetComponent<Button>();
            }
            if (hudHelpButton == null)
            {
                Transform t = gameHudPanel.transform.Find("HelpButton");
                if (t != null) hudHelpButton = t.GetComponent<Button>();
            }
            if (statusPromptText == null)
            {
                Transform t = gameHudPanel.transform.Find("StatusPrompt");
                if (t != null) statusPromptText = t.GetComponent<TextMeshProUGUI>();
            }

            Transform topBar = gameHudPanel.transform.Find("TopBar");
            if (topBar != null)
            {
                if (healthText == null)
                {
                    Transform t = topBar.Find("HealthText");
                    if (t != null) healthText = t.GetComponent<TextMeshProUGUI>();
                }
                if (roundText == null)
                {
                    Transform t = topBar.Find("RoundText");
                    if (t != null) roundText = t.GetComponent<TextMeshProUGUI>();
                }
                if (enemiesText == null)
                {
                    Transform t = topBar.Find("EnemiesText");
                    if (t != null) enemiesText = t.GetComponent<TextMeshProUGUI>();
                }
            }
        }

        // 5. Elements de fi de partida
        if (gameOverPanel != null)
        {
            if (endRestartButton == null)
            {
                Transform t = gameOverPanel.transform.Find("Card/RestartButton");
                if (t == null) t = gameOverPanel.transform.Find("RestartButton");
                if (t != null) endRestartButton = t.GetComponent<Button>();
            }
            if (endMainMenuButton == null)
            {
                Transform t = gameOverPanel.transform.Find("Card/MainMenuButton");
                if (t == null) t = gameOverPanel.transform.Find("MainMenuButton");
                if (t != null) endMainMenuButton = t.GetComponent<Button>();
            }
            if (endTitleText == null)
            {
                Transform t = gameOverPanel.transform.Find("Card/Banner/EndTitle");
                if (t == null) t = gameOverPanel.transform.Find("EndTitle");
                if (t != null) endTitleText = t.GetComponent<TextMeshProUGUI>();
            }
            if (endStatsText == null)
            {
                Transform t = gameOverPanel.transform.Find("Card/EndStats");
                if (t != null) endStatsText = t.GetComponent<TextMeshProUGUI>();
            }
        }
    }

    private void SetupButtonListeners()
    {
        if (playButton != null)
        {
            playButton.onClick.RemoveListener(OnPlayClicked);
            playButton.onClick.AddListener(OnPlayClicked);
        }

        if (instructionsButton != null)
        {
            instructionsButton.onClick.RemoveListener(OpenInstructions);
            instructionsButton.onClick.AddListener(OpenInstructions);
        }

        if (quitButton != null)
        {
            quitButton.onClick.RemoveListener(QuitGame);
            quitButton.onClick.AddListener(QuitGame);
        }

        if (instructionsCloseButton != null)
        {
            instructionsCloseButton.onClick.RemoveListener(CloseInstructions);
            instructionsCloseButton.onClick.AddListener(CloseInstructions);
        }

        if (hudResetButton != null)
        {
            hudResetButton.onClick.RemoveListener(RestartGame);
            hudResetButton.onClick.AddListener(RestartGame);
        }

        if (hudHelpButton != null)
        {
            hudHelpButton.onClick.RemoveListener(OpenInstructions);
            hudHelpButton.onClick.AddListener(OpenInstructions);
        }

        if (endRestartButton != null)
        {
            endRestartButton.onClick.RemoveListener(RestartGame);
            endRestartButton.onClick.AddListener(RestartGame);
        }

        if (endMainMenuButton != null)
        {
            endMainMenuButton.onClick.RemoveListener(ShowMainMenu);
            endMainMenuButton.onClick.AddListener(ShowMainMenu);
        }
    }

    #region Navigation & Screen Flow

    public void ShowMainMenu()
    {
        PlayButtonSfx();

        if (instructionsPanel != null) instructionsPanel.SetActive(false);
        if (gameHudPanel != null) gameHudPanel.SetActive(false);
        if (gameOverPanel != null) gameOverPanel.SetActive(false);

        if (mainMenuPanel != null)
        {
            mainMenuPanel.SetActive(true);
            mainMenuPanel.transform.SetAsLastSibling(); // Portar al davant de tot
        }
    }

    public void OnPlayClicked()
    {
        PlayButtonSfx();

        if (mainMenuPanel != null) mainMenuPanel.SetActive(false);
        if (instructionsPanel != null) instructionsPanel.SetActive(false);
        if (gameOverPanel != null) gameOverPanel.SetActive(false);

        if (gameHudPanel != null)
        {
            gameHudPanel.SetActive(true);
            gameHudPanel.transform.SetAsLastSibling();
        }

        SetStatusPrompt(activeCastle == null
            ? "1. Apunta al terra i selecciona el Castell per col·locar-lo."
            : "Defensa el Castell. Escaneja la carta per invocar canons.");
    }

    public void OpenInstructions()
    {
        PlayButtonSfx();

        if (gameHudPanel != null && gameHudPanel.activeSelf)
        {
            previousPanelBeforeInstructions = gameHudPanel;
        }
        else
        {
            previousPanelBeforeInstructions = mainMenuPanel;
        }

        if (mainMenuPanel != null) mainMenuPanel.SetActive(false);
        if (gameHudPanel != null) gameHudPanel.SetActive(false);
        if (gameOverPanel != null) gameOverPanel.SetActive(false);

        if (instructionsPanel != null)
        {
            instructionsPanel.SetActive(true);
            instructionsPanel.transform.SetAsLastSibling(); // Portar al davant absolut
        }
    }

    public void CloseInstructions()
    {
        PlayButtonSfx();

        if (instructionsPanel != null)
        {
            instructionsPanel.SetActive(false);
        }

        if (isInitialInstructions)
        {
            isInitialInstructions = false;
            ShowMainMenu();
        }
        else
        {
            if (previousPanelBeforeInstructions != null)
            {
                previousPanelBeforeInstructions.SetActive(true);
                previousPanelBeforeInstructions.transform.SetAsLastSibling();
            }
            else
            {
                ShowMainMenu();
            }
        }
    }

    #endregion

    #region Gameplay HUD Updates

    public void RegisterCastle(CastleBehavior castle)
    {
        activeCastle = castle;
        SetStatusPrompt("Castell desplegat. Escaneja cartes per col·locar canons defensius.");
    }

    public void UpdateHealth(int currentHealth, int maxHealth)
    {
        if (healthText != null)
        {
            healthText.text = $"Vida: {currentHealth} / {maxHealth}";
        }

        if (healthSlider != null)
        {
            healthSlider.maxValue = maxHealth;
            healthSlider.value = currentHealth;
        }
    }

    public void UpdateWaveInfo(int currentRound, int totalRounds, int enemiesRemaining)
    {
        if (roundText != null)
        {
            roundText.text = $"Ronda: {currentRound} / {totalRounds}";
        }

        if (enemiesText != null)
        {
            enemiesText.text = $"Enemics: {enemiesRemaining}";
        }
    }

    public void SetStatusPrompt(string message)
    {
        if (statusPromptText != null)
        {
            statusPromptText.text = message;
        }
    }

    public void RegisterEnemyDefeated()
    {
        totalEnemiesDefeated++;
    }

    #endregion

    #region Game Over & Victory

    public void ShowGameOver(bool isVictory, int roundReached, int totalRounds)
    {
        // Oculta qualsevol altra pantalla activa
        if (gameHudPanel != null) gameHudPanel.SetActive(false);
        if (mainMenuPanel != null) mainMenuPanel.SetActive(false);
        if (instructionsPanel != null) instructionsPanel.SetActive(false);

        // Si el panell no existeix a l'escena, es construeix al vol
        if (gameOverPanel == null)
        {
            BuildRuntimeGameOverPanel();
        }

        if (gameOverPanel != null)
        {
            gameOverPanel.SetActive(true);
            gameOverPanel.transform.SetAsLastSibling(); // Portar al davant de tot
        }

        if (isVictory)
        {
            if (endTitleText != null)
            {
                endTitleText.text = "VICTORIA";
                endTitleText.color = Color.white;
            }
            if (endSubtitleText != null)
            {
                endSubtitleText.text = "Has defensat el castell amb exit.";
            }
            if (endBannerBackground != null)
            {
                endBannerBackground.color = victoryColor;
            }
            PlaySfx(victorySfx);
        }
        else
        {
            if (endTitleText != null)
            {
                endTitleText.text = "DERROTA";
                endTitleText.color = Color.white;
            }
            if (endSubtitleText != null)
            {
                endSubtitleText.text = "El castell ha estat destruit pels enemics.";
            }
            if (endBannerBackground != null)
            {
                endBannerBackground.color = defeatColor;
            }
            PlaySfx(defeatSfx);
        }

        if (endStatsText != null)
        {
            int rondesSuperades = Mathf.Max(0, roundReached - (isVictory ? 0 : 1));
            endStatsText.text = $"Rondes superades: {rondesSuperades} / {totalRounds}\n" +
                                $"Enemics derrotats: {totalEnemiesDefeated}";
        }
    }

    private void BuildRuntimeGameOverPanel()
    {
        Canvas canvas = GetComponent<Canvas>();
        if (canvas == null) canvas = FindAnyObjectByType<Canvas>();
        Transform parentTransform = canvas != null ? canvas.transform : transform;

        // 1. Panell de fons fosc que cobreix tota la pantalla
        GameObject goPanel = new GameObject("GameOverPanel", typeof(RectTransform));
        goPanel.transform.SetParent(parentTransform, false);
        RectTransform panelRt = goPanel.GetComponent<RectTransform>();
        panelRt.anchorMin = Vector2.zero;
        panelRt.anchorMax = Vector2.one;
        panelRt.anchoredPosition = Vector2.zero;
        panelRt.sizeDelta = Vector2.zero;

        Image overlay = goPanel.AddComponent<Image>();
        overlay.color = new Color(0.04f, 0.05f, 0.08f, 0.95f);
        overlay.raycastTarget = true;

        // 2. Targeta central
        GameObject cardObj = new GameObject("Card", typeof(RectTransform));
        cardObj.transform.SetParent(goPanel.transform, false);
        RectTransform cardRt = cardObj.GetComponent<RectTransform>();
        cardRt.anchorMin = new Vector2(0.5f, 0.5f);
        cardRt.anchorMax = new Vector2(0.5f, 0.5f);
        cardRt.anchoredPosition = Vector2.zero;
        cardRt.sizeDelta = new Vector2(850, 950);

        Image cardImg = cardObj.AddComponent<Image>();
        cardImg.color = new Color(0.11f, 0.15f, 0.22f, 1f);
        cardImg.raycastTarget = false;

        // 3. Banner superior amb color de victoria o derrota
        GameObject bannerObj = new GameObject("Banner", typeof(RectTransform));
        bannerObj.transform.SetParent(cardObj.transform, false);
        RectTransform bannerRt = bannerObj.GetComponent<RectTransform>();
        bannerRt.anchorMin = new Vector2(0, 0.82f);
        bannerRt.anchorMax = new Vector2(1, 1);
        bannerRt.anchoredPosition = Vector2.zero;
        bannerRt.sizeDelta = Vector2.zero;

        endBannerBackground = bannerObj.AddComponent<Image>();
        endBannerBackground.color = defeatColor;
        endBannerBackground.raycastTarget = false;

        // 4. Text de Titol
        endTitleText = CreateRuntimeText("EndTitle", bannerObj.transform, "DERROTA", 44, FontStyles.Bold, Color.white, TextAlignmentOptions.Center);
        SetRect(endTitleText.gameObject, Vector2.zero, Vector2.one, Vector2.zero, Vector2.zero);

        // 5. Text de Subtitol
        endSubtitleText = CreateRuntimeText("EndSubtitle", cardObj.transform, "El castell ha caigut davant dels enemics.", 26, FontStyles.Normal, new Color(0.8f, 0.85f, 0.9f, 1f), TextAlignmentOptions.Center);
        SetRect(endSubtitleText.gameObject, new Vector2(0.5f, 0.70f), new Vector2(0.5f, 0.70f), Vector2.zero, new Vector2(750, 70));

        // 6. Text d'Estadistiques
        endStatsText = CreateRuntimeText("EndStats", cardObj.transform, "Rondes superades: 0\nEnemics derrotats: 0", 28, FontStyles.Normal, Color.white, TextAlignmentOptions.Center);
        SetRect(endStatsText.gameObject, new Vector2(0.5f, 0.48f), new Vector2(0.5f, 0.48f), Vector2.zero, new Vector2(700, 150));

        // 7. Boto Tornar a Jugar
        endRestartButton = CreateRuntimeButton("RestartButton", cardObj.transform, "TORNAR A JUGAR", new Color(0.12f, 0.48f, 0.95f, 1f), Color.white, 32);
        SetRect(endRestartButton.gameObject, new Vector2(0.5f, 0.26f), new Vector2(0.5f, 0.26f), Vector2.zero, new Vector2(450, 90));
        endRestartButton.onClick.AddListener(RestartGame);

        // 8. Boto Menu Principal
        endMainMenuButton = CreateRuntimeButton("MainMenuButton", cardObj.transform, "MENU PRINCIPAL", new Color(0.20f, 0.26f, 0.36f, 1f), Color.white, 28);
        SetRect(endMainMenuButton.gameObject, new Vector2(0.5f, 0.12f), new Vector2(0.5f, 0.12f), Vector2.zero, new Vector2(450, 80));
        endMainMenuButton.onClick.AddListener(ShowMainMenu);

        gameOverPanel = goPanel;
    }

    private TextMeshProUGUI CreateRuntimeText(string name, Transform parent, string content, float fontSize, FontStyles style, Color color, TextAlignmentOptions alignment)
    {
        GameObject obj = new GameObject(name, typeof(RectTransform), typeof(TextMeshProUGUI));
        obj.transform.SetParent(parent, false);

        TextMeshProUGUI tmp = obj.GetComponent<TextMeshProUGUI>();
        tmp.text = content;
        tmp.fontSize = fontSize;
        tmp.fontStyle = style;
        tmp.color = color;
        tmp.alignment = alignment;
        tmp.textWrappingMode = TextWrappingModes.Normal;
        tmp.raycastTarget = false;
        return tmp;
    }

    private Button CreateRuntimeButton(string name, Transform parent, string label, Color bgColor, Color textColor, float fontSize)
    {
        GameObject btnObj = new GameObject(name, typeof(RectTransform), typeof(Image), typeof(Button));
        btnObj.transform.SetParent(parent, false);

        Image img = btnObj.GetComponent<Image>();
        img.color = bgColor;
        img.raycastTarget = true;

        Button btn = btnObj.GetComponent<Button>();
        btn.targetGraphic = img;

        ColorBlock colors = btn.colors;
        colors.normalColor = bgColor;
        colors.highlightedColor = bgColor * 1.15f;
        colors.pressedColor = bgColor * 0.85f;
        btn.colors = colors;

        TextMeshProUGUI textObj = CreateRuntimeText("Text", btnObj.transform, label, fontSize, FontStyles.Bold, textColor, TextAlignmentOptions.Center);
        RectTransform rt = textObj.GetComponent<RectTransform>();
        rt.anchorMin = Vector2.zero;
        rt.anchorMax = Vector2.one;
        rt.sizeDelta = Vector2.zero;
        rt.anchoredPosition = Vector2.zero;

        return btn;
    }

    private void SetRect(GameObject obj, Vector2 anchorMin, Vector2 anchorMax, Vector2 anchoredPos, Vector2 sizeDelta)
    {
        RectTransform rt = obj.GetComponent<RectTransform>();
        if (rt != null)
        {
            rt.anchorMin = anchorMin;
            rt.anchorMax = anchorMax;
            rt.anchoredPosition = anchoredPos;
            rt.sizeDelta = sizeDelta;
        }
    }

    #endregion

    #region Reset & Utility

    public void RestartGame()
    {
        PlayButtonSfx();

        if (gameOverPanel != null) gameOverPanel.SetActive(false);
        if (mainMenuPanel != null) mainMenuPanel.SetActive(false);
        if (instructionsPanel != null) instructionsPanel.SetActive(false);
        if (gameHudPanel != null)
        {
            gameHudPanel.SetActive(true);
            gameHudPanel.transform.SetAsLastSibling();
        }

        totalEnemiesDefeated = 0;

        //Neteja els coins i el comptador de canons
        if (CoinManager.Instance != null)
        {
            CoinManager.Instance.ResetEconomy();
        }

        // Neteja els enemics i les bales en escena
        SkeletonBehavior[] enemies = FindObjectsByType<SkeletonBehavior>();
        foreach (var enemy in enemies)
        {
            if (enemy != null) Destroy(enemy.gameObject);
        }

        Bullet[] bullets = FindObjectsByType<Bullet>();
        foreach (var bullet in bullets)
        {
            if (bullet != null) Destroy(bullet.gameObject);
        }

        // Reinicia els canons instanciats per marcadors
        ARImageReset imageReset = FindAnyObjectByType<ARImageReset>();
        if (imageReset != null)
        {
            imageReset.ResetCannon();
        }

        if (activeCastle == null)
        {
            activeCastle = FindAnyObjectByType<CastleBehavior>();
        }

        // Reinicia l'estat del castell
        if (activeCastle != null)
        {
            activeCastle.ResetCastle();
            SetStatusPrompt("Partida reiniciada. Prepara't per a la primera onada.");
        }
        else
        {
            SetStatusPrompt("Apunta al terra i col·loca el Castell per comencar.");
        }
    }

    public void ReloadScene()
    {
        PlayButtonSfx();
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public void QuitGame()
    {
        PlayButtonSfx();
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }

    private void PlayButtonSfx()
    {
        PlaySfx(buttonClickSfx);
    }

    private void PlaySfx(AudioClip clip)
    {
        if (audioSource != null && clip != null)
        {
            audioSource.PlayOneShot(clip);
        }
    }

    #endregion
}
