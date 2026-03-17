# Load 场景制作文档

## 场景概述

Load 是应用启动后的第一个场景，作为品牌展示 + 过渡加载页面。播放短铃声音效，显示应用图标渐显动画，动画结束后自动跳转到主界面。

场景路径：`Assets/kiro/Load.unity`

---

## 场景层级结构

```
Load.unity
├── Directional Light          — 方向光
├── RKCameraRig (Prefab)       — XR 相机
├── [RKInput] (Prefab)         — XR 输入系统
├── PointableUI (Prefab)       — World Space Canvas
│   └── Image                  — 应用图标容器
│       └── Text (TMP)         — 应用名称文字
└── Mgr                        — 管理器空物体
```

---

## 业务逻辑层

### BirdStart.cs

脚本路径：`Assets/kiro/Scripts/BirdStart.cs`

| 职责 | 说明 |
|------|------|
| 帧率设置 | `Application.targetFrameRate = 120`，`vSyncCount = 1` |
| 渐显动画 | 通过 CanvasGroup.alpha 实现 OutQuad 缓动渐显 |
| 音效播放 | 延迟 `musicTime` 秒后播放短铃声 |
| 场景跳转 | 动画结束后调用 `SceneManager.LoadScene("Main")` |

#### 生命周期流程

```
Start()
  ├── 设置帧率
  ├── canvasGroup.alpha = 0
  ├── Invoke(SoundLoading, musicTime)
  └── StartCoroutine(FadeSequence)
        ├── WaitForSecondsRealtime(delayBeforeFade)
        ├── 渐显循环 (OutQuad: t * (2 - t))
        └── LoadNextScene() → "Main"
```

#### Inspector 参数

| 字段 | 类型 | 默认值 | 说明 |
|------|------|--------|------|
| canvasGroup | CanvasGroup | — | 图标容器的 CanvasGroup |
| fadeInDuration | float | 1.8 | 渐显持续时间（秒） |
| delayBeforeFade | float | 1 | 开始渐显前的等待时间 |
| musicTime | float | 0.3 | 音效播放延迟 |
| audioSource | AudioSource | — | 音频源组件 |
| audioClip | AudioClip | — | 短铃声音频文件 |

---

## UI 组件层

### PointableUI (World Space Canvas)

- 类型：Prefab 实例 (guid: `3c20833d81e354626b8365b459274912`)
- Position: (0, 0, 0.5)
- 子物体通过 AddedGameObjects 挂载

### Image — 应用图标容器

| 属性 | 值 |
|------|-----|
| AnchoredPosition | (0, 41) |
| SizeDelta | 364.9 × 318.7 |
| Sprite | guid: `17462ffcb5272a24cb840d30841fc673` |
| PreserveAspect | true |
| CanvasGroup.alpha | 初始 0，运行时渐显到 1 |
| CanvasGroup.interactable | false |
| CanvasGroup.blocksRaycasts | false |

### Text (TMP) — 应用名称

| 属性 | 值 |
|------|-----|
| 文本内容 | "不       孤      鸟" |
| AnchoredPosition | (0, -217) |
| SizeDelta | 364.9 × 116 |
| 字号 | 50 |
| 字体颜色 | RGBA(1, 0.89, 0.89, 1) 浅粉色 |
| 字体样式 | Italic (m_fontStyle: 2) |
| 对齐 | 水平居中，垂直居中 |
| 字体 | guid: `9f11f16ef08ecf04585e68528cac76fe` |

### Mgr — 管理器

| 组件 | 说明 |
|------|------|
| BirdStart | 启动逻辑控制器 |
| AudioSource | 音频播放组件 |

Position: (-0.13, -0.05, 0.54)

---

## XR 基础 Prefab

| Prefab | GUID | 说明 |
|--------|------|------|
| RKCameraRig | `bc7bf2e56b74d4038af31f75d0b2d024` | XR 相机系统 |
| [RKInput] | `fa2ab4a52b98844a5beea51c6d5ab85a` | XR 输入模块 |
| PointableUI | `3c20833d81e354626b8365b459274912` | 可交互 World Space Canvas |

---

## 资源引用

| 资源 | GUID | 用途 |
|------|------|------|
| 应用图标 Sprite | `17462ffcb5272a24cb840d30841fc673` | Image 显示 |
| 短铃声音频 | `a8df4666669c48c4c931d9ff59af893e` | 启动音效 |
| 字体 | `9f11f16ef08ecf04585e68528cac76fe` | TextMeshPro 字体 |
