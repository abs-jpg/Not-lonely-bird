using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class BirdStart : MonoBehaviour
{
    public CanvasGroup canvasGroup;

    [Header("动画设置")]
    public float fadeInDuration = 0.5f;
    public float delayBeforeFade = 1f;

    [Header("音效")]
    public float musicTime;
    public AudioSource audioSource;
    public AudioClip audioClip;

    private Coroutine _fadeCoroutine;

    private void Start()
    {
        Application.targetFrameRate = 120;
        QualitySettings.vSyncCount = 1;

        canvasGroup.alpha = 0f;

        Invoke(nameof(SoundLoading), musicTime);

        _fadeCoroutine = StartCoroutine(FadeSequence());
    }

    private IEnumerator FadeSequence()
    {
        // 延迟等待（不受 Time.timeScale 影响）
        yield return new WaitForSecondsRealtime(delayBeforeFade);

        Debug.Log("渐显动画开始");

        float elapsed = 0f;
        while (elapsed < fadeInDuration)
        {
            elapsed += Time.unscaledDeltaTime;
            float t = Mathf.Clamp01(elapsed / fadeInDuration);
            // OutQuad 缓动: t * (2 - t)
            canvasGroup.alpha = t * (2f - t);
            yield return null;
        }

        canvasGroup.alpha = 1f;
        LoadNextScene();
    }

    private void LoadNextScene()
    {
        SceneManager.LoadScene("Main");
    }

    private void SoundLoading()
    {
        audioSource.clip = audioClip;
        audioSource.Play();
    }

    private void OnDestroy()
    {
        if (_fadeCoroutine != null)
        {
            StopCoroutine(_fadeCoroutine);
        }
    }
}
