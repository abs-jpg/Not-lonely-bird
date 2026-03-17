using System;
using System.Collections;
using System.IO;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;
using TMPro;
using MemoryGameTools;

/// <summary>
/// 语音识别管理器
/// 录音 → WAV 保存 → 上传服务器 → 获取识别结果
/// </summary>
public class ASRManager : MonoBehaviour
{
    [Header("服务器")]
    public string uploadURL;

    [Header("UI 引用")]
    public TextMeshProUGUI asrResultText;
    public Button recordButton;
    public TextMeshProUGUI buttonText;

    [Header("提示文字")]
    public string text_StartRecording = "按一下说话";
    public string text_StopRecording = "按一下识别";
    public string text_Processing = "处理中...";
    public string text_Ready = "准备就绪";
    public string text_UploadFailed = "上传失败: ";

    /// <summary>
    /// ASR 识别结果回调事件，参数为识别文本
    /// </summary>
    public event Action<string> OnASRResultReady;

    private AudioClip _recordingClip;
    private bool _isRecording;
    private AudioSource _audioSource;

    private void Start()
    {
        _audioSource = GetComponent<AudioSource>();

        if (recordButton != null)
            recordButton.onClick.AddListener(OnRecordButtonPressed);

        if (buttonText != null)
            buttonText.text = text_Ready;
    }

    public void OnRecordButtonPressed()
    {
        if (_isRecording)
            StopRecording();
        else
            StartRecording();
    }

    private void StartRecording()
    {
        if (Microphone.devices.Length == 0)
        {
            Debug.LogWarning("ASRManager: 未检测到麦克风设备");
            return;
        }

        _isRecording = true;
        _recordingClip = Microphone.Start(null, false, 30, 16000);

        if (buttonText != null)
            buttonText.text = text_StopRecording;
    }

    private void StopRecording()
    {
        _isRecording = false;
        Microphone.End(null);

        if (buttonText != null)
            buttonText.text = text_Processing;

        // 保存 WAV 并上传
        string filePath = Path.Combine(Application.persistentDataPath, "asr_recording.wav");
        SavWav.Save(filePath, _recordingClip);
        StartCoroutine(UploadAudio(filePath));
    }

    private IEnumerator UploadAudio(string filePath)
    {
        byte[] audioData = File.ReadAllBytes(filePath);

        WWWForm form = new WWWForm();
        form.AddBinaryData("file", audioData, "recording.wav", "audio/wav");

        using (UnityWebRequest www = UnityWebRequest.Post(uploadURL, form))
        {
            yield return www.SendWebRequest();

            if (www.result == UnityWebRequest.Result.Success)
            {
                string response = www.downloadHandler.text;
                // 解析服务器返回的 raw_transcription 字段
                string result = ParseTranscription(response);

                if (asrResultText != null)
                    asrResultText.text = "语音结果：" + result;

                OnASRResultReady?.Invoke(result);
            }
            else
            {
                Debug.LogError(text_UploadFailed + www.error);
                if (asrResultText != null)
                    asrResultText.text = text_UploadFailed + www.error;
            }

            if (buttonText != null)
                buttonText.text = text_StartRecording;
        }
    }

    /// <summary>
    /// 简易 JSON 解析，提取 raw_transcription 字段
    /// </summary>
    private string ParseTranscription(string json)
    {
        string key = "\"raw_transcription\"";
        int idx = json.IndexOf(key, StringComparison.Ordinal);
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
