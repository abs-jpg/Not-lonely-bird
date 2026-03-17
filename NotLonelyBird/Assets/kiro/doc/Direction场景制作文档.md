# Direction 场景制作文档

## 场景概述

Direction（方向判断）是一个执行功能测试游戏场景。屏幕左侧或右侧会出现一个箭头图标，箭头指向左或右。玩家需要根据箭头指向（而非箭头出现的位置）举起对应的手掌来回答。支持一致性模式（箭头方向=出现位置）和随机/困难模式（箭头方向与位置无关）。

场景路径：`Assets/kiro/Direction.unity`

---

## 场景层级结构

```
Direction.unity
├── Directional Light              — 方向光 (0,3,0) Rot(50,-30,0)
├── DirectionMgr                   — 游戏管理器 (0.014,-0.016,819.079)
│   ├── DirectionManager           — 核心游戏逻辑
│   ├── AudioSource                — 音频播放
│   └── RVPSettlementScreen        — 结算/存档
├── [RKInput] (Prefab)             — XR 输入系统
├── RKCameraRig (Prefab)           — XR 相机
└── PointableUI (Prefab)           — World Space Canvas (0,0,0.5)
    └── Canvas (Prefab内部)
        └── MianP                  — 主面板 (Scale 0.5, 拉伸填满)
            ├── LL                 — 左侧-指左 箭头图标
            ├── LR                 — 左侧-指右 箭头图标
            ├── RL                 — 右侧-指左 箭头图标
            ├── RR                 — 右侧-指右 箭头图标
            ├── CD                 — 倒计时/状态文本
            ├── start              — 开始按钮
            │   └── Text (TMP)    — "开始游戏"
            └── 白底按钮           — 返回按钮
                └── Text (TMP)    — "返回"
```

---

## 业务逻辑层

### DirectionManager.cs — 核心游戏管理器

脚本路径：`Assets/kiro/Scripts/Direction/DirectionManager.cs`
GUID: `5df775dc986e03c438b253bace3a59d3`

#### 职责

| 功能 | 说明 |
|------|------|
| 参数读取 | 从 AllSettingCtr 单例读取时长、回合数、是否随机模式 |
| 题目生成 | 随机选择箭头出现位置和方向，一致性/随机模式 |
| 手势检测 | 通过反射调用 Rokid SDK 检测手掌张开，键盘 A/D 作为备用 |
| 计分 | 正确回答 +1 分，超时算错误 |
| UI 更新 | 实时更新倒计时文字、正确/错误反馈 |
| 结算 | 计算正确率，保存/读取历史最佳记录 |

#### 游戏流程

```
Start() → 读取 AllSettingCtr 参数 → HideAllCues() → EnterReady()

EnterReady:
  ├── 显示开始按钮
  └── 文本: "准备好了吗？\n开始之后保持握拳\n直到题目出现"

OnStartButtonPressed():
  ├── 重置 trialsCompleted=0, score=0
  ├── 隐藏开始按钮
  ├── 播放开始音效
  └── StartNewRound()

StartNewRound():
  ├── 检查是否完成所有回合 → EnterGameOver()
  ├── 隐藏所有箭头
  ├── 重置倒计时 = gameDuration
  ├── 决定题目:
  │   ├── 一致性 (非随机 或 前3回合): 箭头方向 = 出现位置
  │   └── 随机: 箭头方向与位置无关
  ├── 随机选择左/右侧显示
  └── 激活对应箭头图标 (LL/LR/RL/RR 之一)

Update() 每帧:
  ├── 倒计时递减，更新文本
  ├── 时间到 → ProcessAnswer(false)
  └── DetectGesture():
      ├── Rokid SDK: 左手掌张开=左, 右手掌张开=右
      ├── 键盘: A=左, D=右
      └── 双手都张开 → 忽略

ProcessAnswer(isCorrect):
  ├── trialsCompleted++
  ├── 正确: score++, 显示"正确!", 播放正确音效
  ├── 错误: 显示"错误!", 播放错误音效
  └── 等待 feedbackDelay(2秒) → 下一回合

EnterGameOver():
  ├── 显示开始按钮
  ├── 计算正确率
  ├── 读取/保存历史最佳
  └── 显示结算信息
```

#### Inspector 绑定

| 字段 | 类型 | 绑定目标 |
|------|------|----------|
| startButton | GameObject | start |
| countdownText | TextMeshProUGUI | CD |
| cueLeftLeft | Image | LL |
| cueLeftRight | Image | LR |
| cueRightLeft | Image | RL |
| cueRightRight | Image | RR |
| audioSource | AudioSource | 自身 AudioSource |
| gameStartSound | AudioClip | 未赋值 |
| correctSound | AudioClip | 未赋值 |
| incorrectSound | AudioClip | 未赋值 |
| settings | RVPSettlementScreen | 自身 RVPSettlementScreen |

#### 运行时默认参数

| 参数 | 默认值 | 说明 |
|------|--------|------|
| gameDuration | 10s | 每回合时长 |
| gameRound | 40 | 总回合数 |
| isRandomMode | false | 是否随机模式 |
| feedbackDelay | 2s | 反馈显示时长 |

---

### DirectionSettings.cs — 设置页脚本

脚本路径：`Assets/kiro/Scripts/Direction/DirectionSettings.cs`

用于 ExcutiveReady 场景的设置 UI，通过 AllSettingCtr 单例传递参数。

| 方法 | 说明 |
|------|------|
| OnTimeSliderChanged(float) | 修改 directionGameDuration |
| OnRandomToggleChanged(bool) | 修改 directionIsRandomMode |
| OnGroundSliderChanged(float) | 修改 directionGameRounds |

---

### RVPSettlementScreen.cs — 通用结算组件

脚本路径：`Assets/kiro/Scripts/RVP/RVPSettlementScreen.cs`
GUID: `2ce60687ae7aa56468d0fa2ad782e815`

同时用于 RVP 和 Direction 场景，保存/读取关卡正确率到 JSON 文件。

| 方法 | 说明 |
|------|------|
| SaveScore(levelKey, correctRate) | 保存正确率（仅当高于历史记录时更新） |
| LoadBestScore(levelKey) | 读取历史最佳记录 |

Direction 场景使用 levelKey = "Direction"。

---

### ScenesChange.cs — 场景切换

脚本路径：`Assets/kiro/Scripts/NBack/ScenesChange.cs`
GUID: `2bc350ca4d061aa4284be68f689cc26b`

白底按钮挂载此脚本，onClick 绑定 `SceneChange("ExcutiveReady")`。

---

## UI 组件层

### MianP (主面板)

| 属性 | 值 |
|------|-----|
| 父节点 | PointableUI 内部 Canvas |
| Anchor | 拉伸填满 (Min 0,0 / Max 1,1) |
| Scale | (0.5, 0.5, 0.5) |
| 背景 | Image 透明 (alpha=0) |

### 箭头图标 (LL / LR / RL / RR)

| 图标 | 位置 | 尺寸 | Sprite |
|------|------|------|--------|
| LL (左侧-指左) | (-598, 0) | 454.5×443.3 | 左手掌朝前.png |
| LR (左侧-指右) | (-598, 0) | 454.51×443.3 | 右手掌朝前.png |
| RL (右侧-指左) | (640, 0) | 454.51×443.3 | 左手掌朝前.png |
| RR (右侧-指右) | (640, 0) | 454.51×443.3 | 右手掌朝前.png |

所有图标 PreserveAspect=true，初始 Active=true，脚本 Start 时 HideAllCues() 隐藏。

### CD (倒计时文本)

| 属性 | 值 |
|------|-----|
| AnchoredPosition | (0, 349) |
| SizeDelta | 842.59×225 |
| 字号 | 70 |
| 颜色 | 白色 |
| 对齐 | 居中 |
| 字体 GUID | `3949d331f3833e340a2575a0496ff63d` |

### start (开始按钮)

| 属性 | 值 |
|------|-----|
| AnchoredPosition | (0, 0) |
| SizeDelta | 200×200 |
| Image 颜色 | (0.875, 0.894, 0.980) 浅蓝紫色 |
| onClick | DirectionManager.OnStartButtonPressed() |
| 子文本 | "开始游戏" 字号24 深灰色 |

### 白底按钮 (返回按钮)

| 属性 | 值 |
|------|-----|
| AnchoredPosition | (-740, 410) |
| SizeDelta | 186.7×128.82 |
| Image 颜色 | (0.742, 0.590, 1.0, 0.8) 淡紫色半透明 |
| onClick | ScenesChange.SceneChange("ExcutiveReady") |
| 子文本 | "返回" 字号60 黑色 |

---

## 脚本 GUID 对照

| 脚本 | GUID |
|------|------|
| DirectionManager | `5df775dc986e03c438b253bace3a59d3` |
| RVPSettlementScreen | `2ce60687ae7aa56468d0fa2ad782e815` |
| ScenesChange | `2bc350ca4d061aa4284be68f689cc26b` |
| AllSettingCtr | `a79441f348de89743a2939f4d699eac1` |

## 场景 fileID 索引

| fileID | 物体 |
|--------|------|
| 100000001 | DirectionMgr GameObject |
| 100000003 | DirectionManager 组件 |
| 100000004 | AudioSource 组件 |
| 100000005 | RVPSettlementScreen 组件 |
| 200000001 | MianP 面板 |
| 300000001 | LL 图标 |
| 310000001 | LR 图标 |
| 320000001 | RL 图标 |
| 330000001 | RR 图标 |
| 400000001 | CD 文本 |
| 500000001 | start 按钮 |
| 600000001 | 白底按钮 |

## 图片资源

| 资源 | GUID | 用途 |
|------|------|------|
| 左手掌朝前.png | `e01c20dd9971054438785d2a5eaefaf9` | LL, RL |
| 右手掌朝前.png | `0bdd211247127d644a4d07b734737d3d` | LR, RR |


