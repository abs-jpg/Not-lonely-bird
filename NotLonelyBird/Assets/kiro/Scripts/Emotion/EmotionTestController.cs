using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class EmotionTestController : MonoBehaviour
{
    [Header("测试设置")]
    public int totalQuestions = 10;
    public float displayTime = 3.0f;

    [Tooltip("用户回答后，到下一题开始前的等待时间")]
    public float delayBetweenQuestions = 1.5f;

    [Header("模型与动画")]
    public GameObject characterModel;
    public Animator modelAnimator;

    [Header("UI 元素")]
    public Button[] emotionButtons;

    public GameObject startPanel;
    public GameObject gamePanel;
    public GameObject overPanel;

    public TextMeshProUGUI resultText;
    public TextMeshProUGUI tfText;

    public RVPSettlementScreen settings;

    private int correct;
    private int incorrect;

    private Coroutine runningCoroutine;

    private int currentQuestionIndex = 0;
    private string currentEmotion;
    private List<string> emotions = new List<string> { "smiling", "sad", "angry", "fear" };

    void Start()
    {
        if (AllSettingCtr.Instance != null)
        {
            totalQuestions = AllSettingCtr.Instance.emotionCount;
            displayTime = AllSettingCtr.Instance.emotionDisplayTime;
        }
        else
        {
            totalQuestions = 10;
            displayTime = 3.0f;
        }

        startPanel.SetActive(true);
        gamePanel.SetActive(false);
        overPanel.SetActive(false);
        characterModel.SetActive(false);
    }

    public void StartGame()
    {
        startPanel.SetActive(false);
        gamePanel.SetActive(true);
        overPanel.SetActive(false);
        characterModel.SetActive(true);

        SetButtonsInteractable(false);
        StartCoroutine(NextQuestionRoutine());
    }

    void StartNewQuestion()
    {
        if (currentQuestionIndex < totalQuestions)
        {
            currentQuestionIndex++;
            StartCoroutine(ShowEmotion());
        }
        else
        {
            characterModel.SetActive(false);
            startPanel.SetActive(false);
            gamePanel.SetActive(false);
            overPanel.SetActive(true);

            float accuracy = 0f;
            if (totalQuestions > 0)
                accuracy = ((float)correct / totalQuestions) * 100;

            float historyScore = 0f;
            if (settings != null)
                historyScore = settings.LoadBestScore("Emotion");

            resultText.text = $"测试结束!请点击返回按钮\n\n正确率:{accuracy:F0}%\n最佳记录:{historyScore:F0}%";

            if (settings != null)
                settings.SaveScore("Emotion", accuracy);
        }
    }

    IEnumerator ShowEmotion()
    {
        currentEmotion = emotions[Random.Range(0, emotions.Count)];

        characterModel.SetActive(true);
        modelAnimator.Play(currentEmotion);

        yield return new WaitForSeconds(displayTime);

        modelAnimator.SetTrigger("DoIdle");

        yield return new WaitForSeconds(0.5f);

        SetButtonsInteractable(true);
    }

    public void OnEmotionSelected(string selectedEmotion)
    {
        if (selectedEmotion == currentEmotion)
        {
            ShowTemporaryText("正确!", Color.white);
            correct++;
        }
        else
        {
            ShowTemporaryText("错误!", Color.white);
            incorrect++;
        }

        SetButtonsInteractable(false);
        StartCoroutine(NextQuestionRoutine());
    }

    public void ShowTemporaryText(string message, Color textColor)
    {
        if (runningCoroutine != null)
            StopCoroutine(runningCoroutine);
        runningCoroutine = StartCoroutine(ShowTextCoroutine(message, textColor));
    }

    private IEnumerator ShowTextCoroutine(string message, Color textColor)
    {
        tfText.text = message;
        tfText.color = textColor;
        yield return new WaitForSeconds(1f);
        tfText.text = string.Empty;
        runningCoroutine = null;
    }

    IEnumerator NextQuestionRoutine()
    {
        yield return new WaitForSeconds(delayBetweenQuestions);
        StartNewQuestion();
    }

    void SetButtonsInteractable(bool isInteractable)
    {
        foreach (Button button in emotionButtons)
            button.interactable = isInteractable;
    }
}
