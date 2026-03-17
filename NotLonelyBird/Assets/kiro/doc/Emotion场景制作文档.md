# Emotion 场景制作文档

## 场景概述

Emotion（情感识别）是一个情绪认知测试游戏场景。场景中有一个3D角色模型（星星），会随机播放不同情绪的动画表情。玩家需要在表情播放结束后，从4个情绪按钮中选择正确的情绪。支持通过 AllSettingCtr 单例配置题目数量和显示时间。

场景路径：`Assets/kiro/Emotion.unity`

---

## 场景层级结构

```
Emotion.unity
├── Directional Light              — 方向光 (0,3,0) Rot(50,-30,0)
├── GameObject                     — AudioSource 音频源 (0.014,-0.016,819.079)
├── [RKInput] (Prefab)             — XR 输入系统
├── PointableUI (Prefab)           — World Space Canvas (0,0,0.5)
│   └── Canvas (Prefab内部, AnchoredPos.y=-0.05)
│       ├── 介绍UI (startPanel)    — 开始面板
│       │   ├── Text (TMP)        — 介绍文字
│       │   └── 开始游戏 (Button)  — 开始按钮
│       │       └── Text (TMP)    — "开始！"
│       ├── 答题UI (gamePanel)     — 游戏面板
│       │   ├── 橙色实底按钮       — "开心" → smiling
│       │   ├── 橙色实底按钮 (1)   — "伤心" → sad
│       │   ├── 橙色实底按钮 (2)   — "愤怒" → angry
│       │   ├── 橙色实底按钮 (3)   — "惊讶" → fear
│       │   └── Text (TMP)        — 对错反馈 tfText
│       ├── 结算UI (overPanel)     — 结束面板
│       │   └── 结果              — resultText
│       └── 返回按钮              — onClick→SceneChange("Main")
├── RKCameraRig (Prefab)           — XR 相机
├── angGirl (Prefab, 隐藏)         — 备用3D角色 IsActive=0
├── EmotionMgr                     — 游戏管理器
│   ├── EmotionTestController      — 核心游戏逻辑
│   └── RVPSettlementScreen        — 结算/存档
└── 星星 (Prefab)                  — 主3D角色模型 带Animator
```

---

## 业务逻辑层

### EmotionTestController.cs — 核心游戏管理器

脚本路径：`Assets/kiro/Scripts/Emotion/EmotionTestController.cs`
GUID: `b79c0fd6170b8f8489c368a891c78a5f`

#### 职责

| 功能 | 说明 |
|------|------|
| 参数读取 | 从 AllSettingCtr 单例读取题目数量和显示时间 |
| 情绪生成 | 随机选择 smiling/sad/angry/fear 之一 |
| 动画播放 | 通过 Animator.Play() 播放对应情绪动画 |
| 按钮交互 | 4个情绪按钮，玩家选择后判定正误 |
| 反馈显示 | 正确/错误文字提示（tfText） |
| 结算 | 计算正确率，通过 RVPSettlementScreen 保存/读取历史最佳 |

#### 游戏流程

```
Start() → 读取 AllSettingCtr 参数 → 显示 startPanel → 隐藏 gamePanel/overPanel/characterModel

StartGame() (开始按钮 onClick):
  ├── 隐藏 startPanel
  ├── 显示 gamePanel
  ├── 显示 characterModel
  ├── 禁用所有按钮
  └── StartCoroutine(NextQuestionRoutine())

NextQuestionRoutine():
  ├── 等待 delayBetweenQuestions (1.5秒)
  └── StartNewQuestion()

StartNewQuestion():
  ├── 检查 currentQuestionIndex < totalQuestions
  │   ├── 是 → currentQuestionIndex++, StartCoroutine(ShowEmotion())
  │   └── 否 → 进入结算
  └── 结算:
      ├── 隐藏 characterModel, startPanel, gamePanel
      ├── 显示 overPanel
      ├── 计算正确率 = correct / totalQuestions * 100
      ├── 读取历史最佳 LoadBestScore("Emotion")
      ├── 显示结算文本
      └── SaveScore("Emotion", accuracy)

ShowEmotion():
  ├── 随机选择情绪 (smiling/sad/angry/fear)
  ├── 显示 characterModel
  ├── modelAnimator.Play(currentEmotion)
  ├── 等待 displayTime (3秒)
  ├── modelAnimator.SetTrigger("DoIdle") 回到待机
  ├── 等待 0.5秒
  └── 启用所有按钮

OnEmotionSelected(selectedEmotion):
  ├── 正确: ShowTemporaryText("正确!", white), correct++
  ├── 错误: ShowTemporaryText("错误!", white), incorrect++
  ├── 禁用所有按钮
  └── StartCoroutine(NextQuestionRoutine())
```

#### Inspector 绑定

| 字段 | 类型 | 绑定目标 |
|------|------|----------|
| totalQuestions | int | 10 (AllSettingCtr 覆盖) |
| displayTime | float | 3.0 (AllSettingCtr 覆盖) |
| delayBetweenQuestions | float | 1.5 |
| characterModel | GameObject | 星星 Prefab 实例 (stripped GO: 700000010) |
| modelAnimator | Animator | 星星 Prefab 内部 Animator (stripped: 700000011) |
| emotionButtons | Button[4] | 橙色实底按钮 ×4 |
| startPanel | GameObject | 介绍UI (300000001) |
| gamePanel | GameObject | 答题UI (400000001) |
| overPanel | GameObject | 结算UI (500000001) |
| resultText | TextMeshProUGUI | 结果 (501000001) |
| tfText | TextMeshProUGUI | 答题UI 下 Text(TMP) (405000001) |
| settings | RVPSettlementScreen | EmotionMgr 上的 RVPSettlementScreen 组件 (200000004) |

---

### RVPSettlementScreen.cs — 通用结算组件

脚本路径：`Assets/kiro/Scripts/RVP/RVPSettlementScreen.cs`
GUID: `2ce60687ae7aa56468d0fa2ad782e815`

同时用于 RVP、Direction 和 Emotion 场景，保存/读取关卡正确率到 JSON 文件。

| 方法 | 说明 |
|------|------|
| SaveScore(levelKey, correctRate) | 保存正确率（仅当高于历史记录时更新） |
| LoadBestScore(levelKey) | 读取历史最佳记录 |

Emotion 场景使用 levelKey = "Emotion"。

---

### ScenesChange.cs — 场景切换

脚本路径：`Assets/kiro/Scripts/NBack/ScenesChange.cs`
GUID: `2bc350ca4d061aa4284be68f689cc26b`

返回按钮挂载此脚本，onClick 绑定 `SceneChange("Main")`。

---

## 3D 模型与 Animator

### 星星 Prefab

| 属性 | 值 |
|------|-----|
| Prefab GUID | `23885717072476e4a8b5ac7161857381` |
| 场景实例 fileID | 700000001 (PrefabInstance) |
| Stripped GO | 700000010 |
| Stripped Animator | 700000011 |
| 位置 | (-0.01, -0.53, 0.8) |
| 旋转 | (0, 180, 0) |
| 缩放 | (0.42, 0.42, 0.42) |
| 移除组件 | fileID 6823996120878167202 |

#### Animator 要求

模型 Animator Controller 需包含以下状态/触发器：

| 状态名 | 说明 |
|--------|------|
| smiling | 开心表情动画 |
| sad | 伤心表情动画 |
| angry | 愤怒表情动画 |
| fear | 惊讶表情动画 |
| DoIdle (Trigger) | 回到待机状态 |

脚本通过 `Animator.Play(stateName)` 直接播放情绪动画，通过 `Animator.SetTrigger("DoIdle")` 回到待机。

### angGirl Prefab (备用)

| 属性 | 值 |
|------|-----|
| Prefab GUID | `ded571af550c37247a35b7f03991a0af` |
| 场景实例 fileID | 800000001 (PrefabInstance) |
| IsActive | 0 (隐藏) |
| 位置 | (-0.01, -0.53, 0.8) |
| 旋转 | (0, 180, 0) |
| 缩放 | (0.61, 0.61, 0.61) |
| 移除组件 | 3个 |

---

## UI 组件层

### 介绍UI (startPanel) — fileID 300000001

| 属性 | 值 |
|------|-----|
| 父节点 | PointableUI 内部 Canvas |
| Anchor | 拉伸填满 (Min 0,0 / Max 1,1) |
| 背景 | Image 透明 (alpha=0) |

子节点：
- Text (TMP) (301000001): 介绍文字，字号36，白色，居中
- 开始游戏 (302000001): 按钮，onClick → EmotionTestController.StartGame()
  - Text (TMP) (303000001): "开始！" 字号24

### 答题UI (gamePanel) — fileID 400000001

| 属性 | 值 |
|------|-----|
| 父节点 | PointableUI 内部 Canvas |
| Anchor | 拉伸填满 (Min 0,0 / Max 1,1) |
| 背景 | Image 透明 (alpha=0) |

子节点：
- 橙色实底按钮 (401000001): "开心" → OnEmotionSelected("smiling")
  - Text (TMP) (401100001): "开心" 字号24
- 橙色实底按钮 (1) (402000001): "伤心" → OnEmotionSelected("sad")
  - Text (TMP) (402100001): "伤心" 字号24
- 橙色实底按钮 (2) (403000001): "愤怒" → OnEmotionSelected("angry")
  - Text (TMP) (403100001): "愤怒" 字号24
- 橙色实底按钮 (3) (404000001): "惊讶" → OnEmotionSelected("fear")
  - Text (TMP) (404100001): "惊讶" 字号24
- Text (TMP) (405000001): 对错反馈 tfText，字号48，居中

按钮颜色：橙色 (1, 0.584, 0, 1)，尺寸 200×80

### 结算UI (overPanel) — fileID 500000001

| 属性 | 值 |
|------|-----|
| 父节点 | PointableUI 内部 Canvas |
| Anchor | 拉伸填满 (Min 0,0 / Max 1,1) |
| 背景 | Image 透明 (alpha=0) |

子节点：
- 结果 (501000001): resultText，字号36，白色，居中

### 返回按钮 — fileID 600000001

| 属性 | 值 |
|------|-----|
| AnchoredPosition | (-388, 196) |
| SizeDelta | 186.7×128.82 |
| Scale | (0.5, 0.5, 0.5) |
| onClick | ScenesChange.SceneChange("Main") |
| 子文本 (601000001) | "返回" 字号60 |

---

## 脚本 GUID 对照

| 脚本 | GUID |
|------|------|
| EmotionTestController | `b79c0fd6170b8f8489c368a891c78a5f` |
| RVPSettlementScreen | `2ce60687ae7aa56468d0fa2ad782e815` |
| ScenesChange | `2bc350ca4d061aa4284be68f689cc26b` |
| AllSettingCtr | `a79441f348de89743a2939f4d699eac1` |

## Prefab GUID 对照

| Prefab | GUID |
|--------|------|
| PointableUI | `3c20833d81e354626b8365b459274912` |
| [RKInput] | `fa2ab4a52b98844a5beea51c6d5ab85a` |
| RKCameraRig | `bc7bf2e56b74d4038af31f75d0b2d024` |
| 星星 | `23885717072476e4a8b5ac7161857381` |
| angGirl | `ded571af550c37247a35b7f03991a0af` |

## 场景 fileID 索引

| fileID | 物体 |
|--------|------|
| 850700733 | Directional Light |
| 100000001 | GameObject (AudioSource) |
| 200000001 | EmotionMgr |
| 200000003 | EmotionTestController 组件 |
| 200000004 | RVPSettlementScreen 组件 |
| 300000001 | 介绍UI (startPanel) |
| 301000001 | 介绍文字 Text(TMP) |
| 302000001 | 开始游戏 按钮 |
| 303000001 | 开始按钮文字 |
| 400000001 | 答题UI (gamePanel) |
| 401000001 | 橙色实底按钮 — 开心 |
| 401100001 | 开心按钮文字 |
| 402000001 | 橙色实底按钮 (1) — 伤心 |
| 402100001 | 伤心按钮文字 |
| 403000001 | 橙色实底按钮 (2) — 愤怒 |
| 403100001 | 愤怒按钮文字 |
| 404000001 | 橙色实底按钮 (3) — 惊讶 |
| 404100001 | 惊讶按钮文字 |
| 405000001 | 对错反馈 tfText |
| 500000001 | 结算UI (overPanel) |
| 501000001 | 结果 resultText |
| 600000001 | 返回按钮 |
| 601000001 | 返回按钮文字 |
| 700000001 | 星星 PrefabInstance |
| 700000010 | 星星 Stripped GameObject |
| 700000011 | 星星 Stripped Animator |
| 800000001 | angGirl PrefabInstance |
| 850701423 | PointableUI PrefabInstance |
| 850701424 | PointableUI Canvas RectTransform (stripped) |
| 850701935 | [RKInput] PrefabInstance |
| 850702340 | RKCameraRig PrefabInstance |

