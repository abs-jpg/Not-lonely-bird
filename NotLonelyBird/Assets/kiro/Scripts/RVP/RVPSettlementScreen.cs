using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 通用游戏结算组件 — 保存/读取关卡正确率到 JSON
/// 同时用于 RVP 和 Direction 场景
/// </summary>
public class RVPSettlementScreen : MonoBehaviour
{
    [System.Serializable]
    public class ScoreRecord
    {
        public Dictionary<string, float> bestScores = new Dictionary<string, float>();
    }

    private const string FileName = "settlement_scores.json";

    /// <summary>
    /// 保存正确率（仅当高于历史记录时更新）
    /// </summary>
    public void SaveScore(string levelKey, float correctRate)
    {
        var record = JsonNetDataService.LoadData<ScoreRecord>(FileName);
        if (record.bestScores.ContainsKey(levelKey))
        {
            if (correctRate > record.bestScores[levelKey])
                record.bestScores[levelKey] = correctRate;
        }
        else
        {
            record.bestScores[levelKey] = correctRate;
        }
        JsonNetDataService.SaveData(FileName, record);
    }

    /// <summary>
    /// 读取历史最佳记录
    /// </summary>
    public float LoadBestScore(string levelKey)
    {
        var record = JsonNetDataService.LoadData<ScoreRecord>(FileName);
        if (record.bestScores.ContainsKey(levelKey))
            return record.bestScores[levelKey];
        return 0f;
    }
}
