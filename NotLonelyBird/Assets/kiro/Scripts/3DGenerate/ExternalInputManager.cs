using System.Collections;
using System.IO;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

/// <summary>
/// ASR 语音识别管理器 — 录音→上传→识别→文字转3D模型
/// 挂载在 ASRmanager 对象上
/// </summary>
public class ExternalInputManager : MonoBehaviour
{
    [Header("Tripo API")]
    public string apiKey;

    [Header("引用")]
    [Tooltip("Tripo_Manager 上的 TripoSimpleUI_Manager")]
    public MonoBehaviour targetUIManager;

    [Tooltip("BordController 上的 DrawingScreenshotter")]
    public DrawingScreenshotter screenshotter;

    [Header("ASR 服务器")]
    public string asrUploadURL ;

    private AudioClip _recordingClip;
    private bool _isRecording;
    private AudioSource _audioSource;

    private void Start()
    {
        _audioSource = GetComponent<AudioSource>();
        if (_audioSource == null)
            _audioSource = gameObject.AddComponent<AudioSource>();

        // 自动设置 API Key 并调用确认
        SetApiKeyAndConfirm();
    }

    /// <summary>
    /// 自动设置 API Key 到 TripoSimpleUI_Manager 并点击确认按钮
    /// </summary>
    private void SetApiKeyAndConfirm()
    {
        if (targetUIManager == null || string.IsNullOrEmpty(apiKey)) return;

        var uiType = targetUIManager.GetType();

        // 设置 ApiKeyInputField 的文本
        var apiKeyField = uiType.GetField("ApiKeyInputField",
            System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance);
        if (apiKeyField != null)
        {
            var inputField = apiKeyField.GetValue(targetUIManager);
            if (inputField != null)
            {
                // TMP_InputField
                var textProp = inputField.GetType().GetProperty("text");
                if (textProp != null)
                    textProp.SetValue(inputField, apiKey);
            }
        }

        // 点击确认按钮
        var confirmField = uiType.GetField("btnConfirmApiKey",
            System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance);
        if (confirmField != null)
        {
            var btn = confirmField.GetValue(targetUIManager) as Button;
            if (btn != null)
                btn.onClick.Invoke();
        }
    }

    /// <summary>
    /// 开始/停止录音（外部按钮调用）
    /// </summary>
    public void ToggleRecording()
    {
        if (_isRecording)
            StopRecordingAndProcess();
        else
            StartRecording();
    }

    private void StartRecording()
    {
        if (Microphone.devices.Length == 0)
        {
            Debug.LogWarning("ExternalInputManager: 未检测到麦克风设备");
            return;
        }

        _isRecording = true;
        _recordingClip = Microphone.Start(null, false, 30, 16000);
        Debug.Log("ExternalInputManager: 开始录音");
    }

    private void StopRecordingAndProcess()
    {
        _isRecording = false;
        Microphone.End(null);
        Debug.Log("ExternalInputManager: 停止录音，开始处理");

        string filePath = Path.Combine(Application.persistentDataPath, "asr_3d_recording.wav");

        // 使用 SavWav 保存（如果存在），否则简单保存
        var savWavType = System.Type.GetType("SavWav") ??
                         System.Type.GetType("MemoryGameTools.SavWav");
        if (savWavType != null)
        {
            var saveMethod = savWavType.GetMethod("Save",
                System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Static);
            if (saveMethod != null)
                saveMethod.Invoke(null, new object[] { filePath, _recordingClip });
        }

        StartCoroutine(UploadAndGenerate(filePath));
    }

    private IEnumerator UploadAndGenerate(string filePath)
    {
        if (!File.Exists(filePath))
        {
            Debug.LogError("ExternalInputManager: 录音文件不存在 " + filePath);
            yield break;
        }

        byte[] audioData = File.ReadAllBytes(filePath);
        WWWForm form = new WWWForm();
        form.AddBinaryData("file", audioData, "recording.wav", "audio/wav");

        using (UnityWebRequest www = UnityWebRequest.Post(asrUploadURL, form))
        {
            yield return www.SendWebRequest();

            if (www.result == UnityWebRequest.Result.Success)
            {
                string response = www.downloadHandler.text;
                string text = ParseTranscription(response);
                Debug.Log("ExternalInputManager: ASR识别结果 = " + text);

                // 隐藏画板等UI
                if (GlobalUIManager.Instance != null)
                    GlobalUIManager.Instance.HideAllManagedItems();

                // 设置文本到 TextPromptInputField 并触发生成
                SetTextAndGenerate(text);
            }
            else
            {
                Debug.LogError("ExternalInputManager: ASR上传失败 " + www.error);
            }
        }
    }

    private void SetTextAndGenerate(string text)
    {
        if (targetUIManager == null) return;

        var uiType = targetUIManager.GetType();

        // 设置 TextPromptInputField
        var textField = uiType.GetField("TextPromptInputField",
            System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance);
        if (textField != null)
        {
            var inputField = textField.GetValue(targetUIManager);
            if (inputField != null)
            {
                var textProp = inputField.GetType().GetProperty("text");
                if (textProp != null)
                    textProp.SetValue(inputField, text);
            }
        }

        // 点击 TextToModel 生成按钮
        var btnField = uiType.GetField("btnTextToModelGenerate",
            System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance);
        if (btnField != null)
        {
            var btn = btnField.GetValue(targetUIManager) as Button;
            if (btn != null)
                btn.onClick.Invoke();
        }
    }

    private string ParseTranscription(string json)
    {
        string key = "\"raw_transcription\"";
        int idx = json.IndexOf(key, System.StringComparison.Ordinal);
        if (idx < 0) return json;

        int colonIdx = json.IndexOf(':', idx + key.Length);
        if (colonIdx < 0) return json;

        int startQuote = json.IndexOf('"', colonIdx + 1);
        if (startQuote < 0) return json;

        int endQuote = json.IndexOf('"', startQuote + 1);
        if (endQuote < 0) return json;

        return json.Substring(startQuote + 1, endQuote - startQuote - 1);
    }
}
