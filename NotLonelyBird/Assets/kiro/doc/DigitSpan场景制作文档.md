# DigitSpan 场景制作文档

## 场景概述

DigitSpan 是一个数字广度记忆测试游戏场景。系统播放一组随机数字音频，玩家通过数字按钮或语音识别回答听到的数字序列（顺序或逆序）。支持自动模式（连续答对3次切换逆序）、固定顺序、固定逆序三种模式，以及多种难度（2-5位数字）。

场景路径：`Assets/kiro/DigitSpan.unity`

---

## 场景层级结构

```
DigitSpan.unity
├── Directional Light              — 方向光 (0,3,0) Rot(50,-30,0)
├── [RKInput] (Prefab)             — XR 输入系统
├── RKCameraRig (Prefab)           — XR 相机
├── PointableUI (Prefab)           — World Space Canvas (0,0,0.5)
│   └── mainpanel                  — 内容面板 Scale(0.49,0.49,0.49) Stretch锚点
│       ├── 语音输出 (feedbackText) — TextMeshProUGUI "语音结果："
│       ├── subtitle               — TextMeshProUGUI "根据要求回答听到的数字"
│       ├── 开始 (startGameButton) — Button + Image 250×250
│       │   └── Text (TMP)        — "开始游戏"
│       ├── 录制 (recordButton)    — Button + Image 150×150
│       │   └── 录音 (buttonText) — "语音答题"
│       ├── 选择                   — GridLayoutGroup 容器
│       │   ├── 0 ~ 9             — 10个数字按钮 (Button + Image + Text)
│       ├── 计数器 (counterDisplay)— TextMeshProUGUI
│       └── 返回按钮               — Button + ScenesChange
│           └── Text (TMP)        — "返回"
└── GameObject                     — MemoryGameManager + ASRManager + AudioSource
                                     Position: (0.014, -0.016, 819.079)
```

---

## 业务逻辑层

### MemoryGameManager.cs — 核心游戏管理器

脚本路径：`Assets/kiro/Scripts/DigitSpan/MemoryGameManager.cs`
GUID: `8bf7db3235f58d14e959f0efac33f3b4`

#### 职责

| 功能 | 说明 |
|------|------|
| 参数读取 | 从 AllSettingCtr 单例读取 memoryMode、memoryDifficulty |
| 序列生成 | 随机生成 2-5 位数字序列 |
| 音频播放 | 依次播放数字音频 + 模式提示音 |
| 输入处理 | 数字按钮点击 + ASR 语音识别 |
| 答案校验 | 将输入转为中文与正确答案比较 |
| 自动模式 | 连续答对3次切换逆序，答错回到顺序 |

#### 游戏流程

```
Start() → 读取 AllSettingCtr 参数 → 绑定按钮/ASR事件 → 隐藏输入区域

StartGame() (点击开始按钮):
  ├── 隐藏开始按钮
  └── StartCoroutine(GameLoop)

GameLoop 协程:
  └── while gameRunning:
      ├── 每5分钟播放休息提示音
      └── PlayRound 协程:
          ├── 决定数字位数 (根据难度设置)
          ├── 生成随机数字序列
          ├── 生成正确答案 (中文, 顺序/逆序)
          ├── 播放模式提示音 (顺序.wav / 逆序.wav)
          ├── 依次播放数字音频 (间隔0.2秒)
          ├── 显示"请回答", 激活输入
          └── 等待玩家输入

CheckAnswer():
  ├── 正确 → 播放正确.wav, consecutiveCorrect++
  │   └── 自动模式: ≥3次 → 切换逆序
  ├── 错误 → 播放错误.wav, 显示正确答案
  │   └── 自动模式: 重置计数, 回到顺序
  └── 更新计数器 "你已经做了 X 道题！"
```

#### Inspector 绑定

| 字段 | 类型 | 绑定目标 |
|------|------|----------|
| asrManager | ASRManager | 同一 GameObject 上的 ASRManager |
| startGameButton | Button | 开始 按钮 |
| feedbackText | TextMeshProUGUI | 语音输出 |
| numberButtons[0-9] | Button[] | 数字按钮 0-9 |
| counterDisplay | TextMeshProUGUI | 计数器 |
| digitAudioClips[0-9] | AudioClip[] | 0-9.wav 数字音频 |
| audioClipOrder | AudioClip | 顺序.wav |
| audioClipReverse | AudioClip | 逆序.wav |
| audioClipCorrect | AudioClip | 正确.wav |
| audioClipWrong | AudioClip | 错误.wav |
| audioClipRest | AudioClip | 休息.wav |

---

### ASRManager.cs — 语音识别管理器

脚本路径：`Assets/kiro/Scripts/DigitSpan/ASRManager.cs`
GUID: `c666479d7f17cda4580134b1990bc056`

#### 职责

| 功能 | 说明 |
|------|------|
| 录音控制 | Microphone.Start/End, 16kHz 采样 |
| WAV 保存 | 通过 SavWav 保存到 persistentDataPath |
| 上传识别 | POST 到 ASR 服务器, 解析 raw_transcription |
| 事件通知 | OnASRResultReady 事件通知 MemoryGameManager |

#### Inspector 绑定

| 字段 | 类型 | 绑定目标 |
|------|------|----------|
| uploadURL | string | "服务器隐私" |
| asrResultText | TextMeshProUGUI | 语音输出 (与 MemoryGameManager 共用) |
| recordButton | Button | 录制 按钮 |
| buttonText | TextMeshProUGUI | 录音 文字 |

#### 语音识别流程

```
OnRecordButtonPressed():
  ├── 首次点击 → StartRecording() → Microphone.Start(16kHz)
  └── 再次点击 → StopRecording() → Microphone.End()
        ├── SavWav.Save() → asr_recording.wav
        └── UploadAudio() → POST 到服务器
              ├── 成功 → 解析 raw_transcription → OnASRResultReady
              └── 失败 → 显示错误信息
```

---

### SavWav.cs — WAV 文件保存工具

脚本路径：`Assets/kiro/Scripts/DigitSpan/SavWav.cs`
命名空间：`MemoryGameTools`

纯静态工具类，将 AudioClip 保存为标准 16-bit PCM WAV 文件。

---

### MemorySettingsMenu.cs — 设置界面

脚本路径：`Assets/kiro/Scripts/DigitSpan/MemorySettingsMenu.cs`

在 MemoryReady 设置场景中使用（不在 DigitSpan 场景中直接挂载）。

| 功能 | 说明 |
|------|------|
| 模式选择 | ToggleGroup: 自动(0) / 顺序(1) / 逆序(2) |
| 难度选择 | Slider: 随机(0) / 2位(1) / 3位(2) / 4位(3) / 5位(4) |
| 参数写入 | 修改 AllSettingCtr.Instance.memoryMode/memoryDifficulty |

---

### AllSettingCtr.cs — 全局单例 (已扩展)

脚本路径：`Assets/kiro/Scripts/NBack/AllSettingCtr.cs`

新增 DigitSpan 相关字段：

| 字段 | 类型 | 默认值 | 说明 |
|------|------|--------|------|
| memoryMode | int | 0 | 0=自动, 1=固定顺序, 2=固定逆序 |
| memoryDifficulty | int | 0 | 0=随机(2-5位), 1→2位, 2→3位, 3→4位, 4→5位 |

---

## UI 组件层

### PointableUI (World Space Canvas)

- Prefab GUID: `3c20833d81e354626b8365b459274912`
- Position: (0, 0, 0.5)
- mainpanel 通过 AddedGameObjects 挂载到 Canvas 的 RectTransform 下

### mainpanel

| 属性 | 值 |
|------|-----|
| Scale | (0.49, 0.49, 0.49) |
| Anchors | Stretch (min 0,0 max 1,1) |
| Image | 透明背景 (alpha=0), Sprite: 内置 Knob |

### 语音输出 (feedbackText / asrResultText)

| 属性 | 值 |
|------|-----|
| AnchoredPosition | (0, 236) |
| SizeDelta | 748.8 × 107.3 |
| 字号 | 45 |
| 颜色 | 白色 |
| 对齐 | 左对齐, 顶部对齐 |
| 默认文字 | "语音结果：" |
| 字体 GUID | `3949d331f3833e340a2575a0496ff63d` |

### subtitle (标题)

| 属性 | 值 |
|------|-----|
| AnchoredPosition | (0, 410) |
| SizeDelta | 1077.3 × 145.1 |
| 字号 | 85.74 |
| 颜色 | 白色 |
| 对齐 | 居中, 顶部对齐 |
| 默认文字 | "根据要求回答听到的数字" |

### 开始 (startGameButton)

| 属性 | 值 |
|------|-----|
| AnchoredPosition | (0, 0) |
| SizeDelta | 250 × 250 |
| Image 颜色 | RGB(0.875, 0.893, 0.981) 浅蓝紫色 |
| Image Sprite GUID | `008cf15178552a240954abd842946aec` |
| Button Pressed | RGB(0.933, 0.733, 0.765) 粉色 |
| 子文字 | "开始游戏" 字号41.53 黑色 |

### 录制 (recordButton)

| 属性 | 值 |
|------|-----|
| AnchoredPosition | (688, -109) |
| SizeDelta | 150 × 150 |
| Image 颜色 | RGB(0.722, 0.757, 0.925) 浅蓝紫色 |
| Image Sprite GUID | `4fde89ca55388d04e9a06fe25e766e3a` |
| 子文字 | "语音答题" 字号45 白色 偏移(-105.4) |

### 选择 (数字按钮容器)

| 属性 | 值 |
|------|-----|
| AnchoredPosition | (0, -411) |
| SizeDelta | (0, -822) Stretch锚点 |
| GridLayoutGroup CellSize | 150 × 150 |
| GridLayoutGroup Spacing | (30, 0) |
| ChildAlignment | UpperCenter |
| 子物体 | 10个数字按钮 (0-9) |

每个数字按钮：150×150, 浅蓝紫色背景, 黑色数字文字 字号60

### 计数器 (counterDisplay)

| 属性 | 值 |
|------|-----|
| AnchoredPosition | (-501.4, -214) |
| SizeDelta | 752.8 × 136 |
| 字号 | 40 |
| 颜色 | 白色 |
| 运行时格式 | "你已经做了 X 道题！" |

### 返回按钮

| 属性 | 值 |
|------|-----|
| AnchoredPosition | (-740, 410) |
| SizeDelta | 186.7 × 128.82 |
| Image 颜色 | RGBA(0.742, 0.590, 1.0, 0.8) 紫色半透明 |
| ScenesChange | SceneChange("MemoryReady") |
| 子文字 | "返回" 字号60 黑色 |

---

## 音频资源

| 文件名 | GUID | 用途 |
|--------|------|------|
| 0.wav | `52879bca48eadf545b8613b78f9e670a` | 数字0音频 |
| 1.wav | `e1bb21320c665294bb9526c80c7a248f` | 数字1音频 |
| 2.wav | `ac3812032799b5142b2810a174c59b33` | 数字2音频 |
| 3.wav | `a88bbd0a27d2f3644a274067994732c5` | 数字3音频 |
| 4.wav | `6e1858c3cf4cb444eb4e71969f952e47` | 数字4音频 |
| 5.wav | `cd0d07e008209754a8d6f667203dfb93` | 数字5音频 |
| 6.wav | `aa9832d7b2f56de4f9fe0025aca9bdad` | 数字6音频 |
| 7.wav | `e562368f5e14bb74f9cc133f45cf8e78` | 数字7音频 |
| 8.wav | `daced8aa53bf7f94ea0fff0900c4c111` | 数字8音频 |
| 9.wav | `47c14ddd28b6a364c8ab6f9b0e8a8ead` | 数字9音频 |
| 顺序.wav | `5e5ed89d928b17a419efd555ba61a687` | 顺序模式提示 |
| 逆序.wav | `c6b58bc47c470f2478c222b775abaf35` | 逆序模式提示 |
| 正确.wav | `d01de851079ee934c854e0171d37ab90` | 回答正确提示 |
| 错误.wav | `40f82e324cf9b4c408083f6b04e47586` | 回答错误提示 |
| 休息.wav | `a8279b2834fc91f4fb695e4e4bc9e4eb` | 休息提醒 |

音频文件路径：`AZ/0-9audio/`（需从原项目复制）

---

## 脚本 GUID 对照

| 脚本 | GUID |
|------|------|
| MemoryGameManager | `8bf7db3235f58d14e959f0efac33f3b4` |
| ASRManager | `c666479d7f17cda4580134b1990bc056` |
| ScenesChange | `2bc350ca4d061aa4284be68f689cc26b` |
| AllSettingCtr | `a79441f348de89743a2939f4d699eac1` |
| Button (Unity) | `4e29b1a8efbd4b44bb3f5571e532d8e8` |
| Image (Unity) | `fe87c0e1cc204ed48ad3b37840f39efc` |
| TextMeshProUGUI | `f4688fdb7df04437aeb418b961361dc5` |
| GridLayoutGroup | `8a8695521f0d02e499659fee002a26c2` |

---

