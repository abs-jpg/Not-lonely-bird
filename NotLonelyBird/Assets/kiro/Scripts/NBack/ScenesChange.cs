using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// 场景切换工具
/// </summary>
public class ScenesChange : MonoBehaviour
{
    public string sceneName;

    public void SceneChange(string sceneName)
    {
        SceneManager.LoadScene(sceneName);
    }
}
    