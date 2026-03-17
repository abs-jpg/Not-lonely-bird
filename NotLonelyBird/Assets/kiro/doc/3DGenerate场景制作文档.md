# 3DGenerate 场景制作文档

## 场景概述

3DGenerate（虚实绘景）是一个 XR 创意场景，集成 Tripo AI SDK 实现3D模型生成。用户可通过三种方式创建3D模型：画板绘图转3D（ImageToModel）、文字描述转3D（TextToModel）、语音输入转3D（ASR + TextToModel）。场景使用 Rokid SDK 提供手势射线交互，生成的3D模型支持手势抓取。

场景路径：`Assets/kiro/3DGenerate.unity`

---

## 场景层级结构

```
3DGenerate.unity
├── Directional Light              — 方向光 (0,3,0) Rot(50,-30,0)
├── [RKInput] (Prefab)             — XR 输入系统
├── RKCameraRig (Prefab)           — XR 相机
├── PointableUI (Prefab)           — World Space Canvas (0,0,0.5)
│   └── Panel                      — Tripo 设置面板 (Canvas 子节点)
│       ├── Panel (背景)           — 半透明黑色背景 Image
│       ├── Tripo_logo             — Logo 图片
│       ├── API_Key_Text           — "API Key" 标签
│       ├── API_Key_InputField     — API Key 输入框
│       ├── API_Key_Text (1)       — "Model Version" 标签
│       ├── API_Key_Confirm        — 确认按钮
│       │   └── Text (Legacy)      — "Confirm"
│       ├── TextToModel_InputField — 文字描述输入框
│       ├── TextToModel_Text       — "Text to Model" 标签
│       ├── TextToModelPercentageText — 文字转模型进度百分比
│       ├── TextToModel_Generate   — 文字生成按钮
│       │   └── Text (Legacy)      — "Generate"
│       ├── ImageToModel_Generate  — 图片生成按钮
│       │   └── Text (Legacy)      — "Generate"
│       ├── ImageToModel_Text      — "Image to Model" 标签
│       ├── imagePath_InputField   — 图片路径输入框
│       │   ├── Placeholder        — "Image path..."
│       │   └── Text (Legacy)      — 输入文字
│       ├── PreviewImageButton     — 预览图片按钮
│       │   └── Text (Legacy)      — 按钮文字
│       ├── PreviewImage           — 图片预览区域
│       ├── ImageToModelPercentageText — 图片转模型进度百分比
│       ├── API_Key_Text (2)       — 附加标签
│       └── ModelVersionDropdown   — 模型版本下拉框
│           ├── Label              — 当前选项文字
│           ├── Arrow              — 下拉箭头
│           └── Template           — 下拉模板
│               └── Viewport → Content → Item
├── EventSystem                    — Unity 事件系统
├── SimpleGameObject               — 3D模型容器 (0,1,-6.5)
│   ├── MeshRenderer               — 网格渲染器
│   ├── TripoModelLoader           — Tripo 模型加载器
│   └── GltfImport                 — glTF 导入组件
└── Tripo_Manager                  — Tripo SDK 管理器
    ├── TripoSimpleUI_Manager      — Tripo UI 管理脚本
    └── TripoRuntimeCore           — Tripo 运行时核心
```

### 待添加的 GameObject（空物体 + 脚本）

以下三个空物体需要手动创建并挂载对应脚本：

```
├── ASRmanager                     — 语音识别管理器
│   ├── ExternalInputManager       — ASR 录音/上传/识别脚本
│   └── AudioSource                — 音频源组件
├── BordController                 — 画板控制器
│   ├── DrawingScreenshotter       — 截图生成3D脚本
│   └── ScenesChange               — 场景切换脚本
└── GlobalUIManager                — UI 总管单例
    └── GlobalUIManager            — UI 管理脚本
```

---

## 业务逻辑层

### GlobalUIManager.cs — UI 总管单例

脚本路径：`Assets/kiro/Scripts/3DGenerate/GlobalUIManager.cs`
GUID: `ad6e8b055daf4ca49b0ce3e0874fa18c`

#### 职责

| 功能 | 说明 |
|------|------|
| 单例管理 | 通过 `GlobalUIManager.Instance` 全局访问 |
| 面板显隐 | 统一控制所有 UI CanvasGroup 的 alpha/interactable/blocksRaycasts |
| 3D物体显隐 | 统一控制 managed3DObjects 的 SetActive |
| 进度条 | 通过 tripoSlider 更新生成进度 (0~1) |

#### Inspector 绑定

| 字段 | 类型 | 绑定目标 |
|------|------|----------|
| TripoRuntimeCore | MonoBehaviour | Tripo_Manager 上的 TripoRuntimeCore 组件 (222279801) |
| tripoSlider | Slider | 进度条 Slider（待创建） |
| managedUICanvasGroups | CanvasGroup[] | [0]=null(占位), [1]=画板面板 CanvasGroup（待创建） |
| managed3DObjects | GameObject[] | [0]=画笔模型(null), [1]=Board(画板), [2]=Gear(齿轮)（待创建） |

#### 核心方法

```csharp
HideAllManagedItems()   // 隐藏所有面板(alpha=0) + 3D物体(SetActive=false)
ShowAllManagedItems()   // 恢复所有面板和物体可见
UpdateSlider(float)     // 更新进度条值
SetSliderActive(bool)   // 显隐进度条
```

---

### DrawingScreenshotter.cs — 截图生成3D

脚本路径：`Assets/kiro/Scripts/3DGenerate/DrawingScreenshotter.cs`
GUID: `1ce8e9af3b08e1247a154bdf842bfdc2`

#### 职责

| 功能 | 说明 |
|------|------|
| 画板截图 | 通过反射获取 DrawingBoard 的 drawingTexture，保存为 PNG |
| 调用生成 | 调用 TripoRuntimeCore.SetImagePath + ImageToModel |
| 完成回调 | ChangeBool() 被 TripoRuntimeCore.OnModelGenerateComplete 调用 |
| 模型交互 | 生成完成后启用 SimpleGameObject 的 BoxCollider |

#### Inspector 绑定

| 字段 | 类型 | 绑定目标 |
|------|------|----------|
| progressSlider | Slider | 进度条 Slider（待创建） |
| simpleModel | GameObject | SimpleGameObject (2107462275) |

#### 生成流程

```
CaptureAndSaveToFile():
  ├── 反射获取 DrawingBoard.drawingTexture
  ├── EncodeToPNG → board_capture.png
  ├── TripoRuntimeCore.SetImagePath(filePath)
  └── TripoRuntimeCore.ImageToModel()

ChangeBool() (生成完成回调):
  ├── isGenerating = false
  └── SimpleGameObject.BoxCollider.enabled = true
```

---

### ExternalInputManager.cs — ASR 语音识别管理器

脚本路径：`Assets/kiro/Scripts/3DGenerate/ExternalInputManager.cs`
GUID: `f3e1a662f8d65024bb5a1920ca2959c2`

#### 职责

| 功能 | 说明 |
|------|------|
| API Key 自动配置 | Start() 时自动设置 apiKey 到 TripoSimpleUI_Manager 并触发确认 |
| 录音控制 | ToggleRecording() 开始/停止麦克风录音 (16kHz, 30秒) |
| ASR 上传 | 将 WAV 文件 POST 到 ASR 服务器 |
| 文字生成 | 解析识别结果 → 隐藏 UI → 设置文字 → 触发 TextToModel |

#### Inspector 绑定

| 字段 | 类型 | 绑定目标 |
|------|------|----------|
| apiKey | string | Tripo API Key（手动填写） |
| targetUIManager | MonoBehaviour | Tripo_Manager 上的 TripoSimpleUI_Manager (222279800) |
| screenshotter | DrawingScreenshotter | BordController 上的 DrawingScreenshotter |
| asrUploadURL | string | "http://110.40.170.159/upload" |

#### 语音生成流程

```
Start() → SetApiKeyAndConfirm():
  ├── 反射设置 ApiKeyInputField.text = apiKey
  └── 反射触发 btnConfirmApiKey.onClick

ToggleRecording():
  ├── 首次 → Microphone.Start(16kHz, 30s)
  └── 再次 → Microphone.End() → SavWav.Save()
        └── UploadAndGenerate():
              ├── POST WAV → ASR 服务器
              ├── 解析 raw_transcription
              ├── GlobalUIManager.HideAllManagedItems()
              └── SetTextAndGenerate(text):
                    ├── 设置 TextPromptInputField.text
                    └── 触发 btnTextToModelGenerate.onClick
```

---

## 已有组件层（Tripo SDK，用户已手动绑定）

### Tripo_Manager (222279799)

| 组件 | fileID | GUID | 说明 |
|------|--------|------|------|
| Transform | 222279802 | — | Position (0,0,0) |
| TripoSimpleUI_Manager | 222279800 | `09f032cc6eccbc44bad744f19f10d60f` | UI 管理 |
| TripoRuntimeCore | 222279801 | `73b0d52bf358f9245996499886128aae` | 运行时核心 |

#### TripoSimpleUI_Manager 绑定

| 字段 | 绑定 fileID | 说明 |
|------|-------------|------|
| btnConfirmApiKey | 1975150345 | API_Key_Confirm 按钮 |
| btnTextToModelGenerate | 1947837527 | TextToModel_Generate 按钮 |
| btnLoadImage | 139469118 | PreviewImageButton 按钮 |
| btnImageToMdelGenerate | 2038482499 | ImageToModel_Generate 按钮 |
| ApiKeyInputField | 1742463204 | API Key 输入框 |
| TextPromptInputField | 1566225858 | 文字描述输入框 |
| ImagePathInputField | 50765620 | 图片路径输入框 |
| SimpleModel | 2107462275 | SimpleGameObject |
| ModelVersionDropdown | 1381583980 | 模型版本下拉框 |
| ModelRotationSpeed | 50 | 模型旋转速度 |

#### TripoRuntimeCore 参数

| 字段 | 值 | 说明 |
|------|-----|------|
| modelVersion | 3 | 模型版本 |
| face_limit | 10000 | 面数限制 |
| pbr_optional | 1 | 启用 PBR |
| OnModelGenerateComplete | — | 待绑定 DrawingScreenshotter.ChangeBool() |

### SimpleGameObject (2107462275)

| 组件 | fileID | GUID | 说明 |
|------|--------|------|------|
| Transform | 2107462279 | — | Position (0,1,-6.5) |
| MeshRenderer | 2107462276 | — | 网格渲染 |
| TripoModelLoader | 2107462277 | `7fc354af441a35b4eab3dd86d3f8d513` | 模型加载器 |
| GltfImport | 2107462278 | `b781fe673a5534e91b1e802df4b9362e` | glTF 导入 |

---

## Prefab 实例

### PointableUI

| 属性 | 值 |
|------|-----|
| PrefabInstance fileID | 520401423 |
| Canvas RectTransform (stripped) | 520401424 |
| Prefab GUID | `3c20833d81e354626b8365b459274912` |
| Position | (0, 0, 0.5) |
| 子节点 | Panel (1537883603) 通过 m_AddedGameObjects 挂载 |

### [RKInput]

| 属性 | 值 |
|------|-----|
| PrefabInstance fileID | 520401935 |
| Prefab GUID | `fa2ab4a52b98844a5beea51c6d5ab85a` |

### RKCameraRig

| 属性 | 值 |
|------|-----|
| PrefabInstance fileID | 520402340 |
| Prefab GUID | `bc7bf2e56b74d4038af31f75d0b2d024` |

---

## UI 组件层

### Panel (Tripo 设置面板)

Panel (1537883603) 是 PointableUI Canvas 的子节点，包含 Tripo SDK 的全部 UI 控件。

| 属性 | 值 |
|------|-----|
| fileID | 1537883603 (GO), 1537883607 (RT) |
| Scale | (0, 0, 0) — 默认隐藏 |
| Anchor | (0,0)-(0,0) |
| AnchoredPosition | (959.99994, 540) |
| Canvas | 含 Canvas + CanvasScaler + GraphicRaycaster |
| 子节点数 | 18 个 UI 元素 |

#### Panel 内 UI 元素一览

| 名称 | fileID | 类型 | 说明 |
|------|--------|------|------|
| Panel (背景) | 895189710 | Image | 半透明黑色背景 (alpha=0.392) |
| Tripo_logo | 221302112 | Image | Logo 图片 |
| API_Key_Text | 1984256872 | Text (Legacy) | "API Key" |
| API_Key_InputField | 1742463202 | InputField | API Key 输入 |
| API_Key_Text (1) | 999909738 | Text (Legacy) | "Model Version" |
| API_Key_Confirm | 1975150343 | Button | 确认按钮 |
| TextToModel_InputField | 1566225856 | InputField | 文字描述输入 |
| TextToModel_Text | 1885668375 | Text (Legacy) | "Text to Model" |
| TextToModelPercentageText | 2145960175 | Text (Legacy) | 进度百分比 |
| TextToModel_Generate | 1947837525 | Button | 文字生成按钮 |
| ImageToModel_Generate | 2038482497 | Button | 图片生成按钮 |
| ImageToModel_Text | 173414319 | Text (Legacy) | "Image to Model" |
| imagePath_InputField | 50765618 | InputField | 图片路径输入 |
| PreviewImageButton | 139469116 | Button | 预览图片按钮 |
| PreviewImage | 643623799 | Image | 图片预览 |
| ImageToModelPercentageText | 10857588 | Text (Legacy) | 进度百分比 |
| API_Key_Text (2) | 1738908917 | Text (Legacy) | 附加标签 |
| ModelVersionDropdown | 1381583978 | Dropdown | 模型版本选择 |

---

## 脚本 GUID 对照

| 脚本 | GUID |
|------|------|
| GlobalUIManager | `ad6e8b055daf4ca49b0ce3e0874fa18c` |
| DrawingScreenshotter | `1ce8e9af3b08e1247a154bdf842bfdc2` |
| ExternalInputManager | `f3e1a662f8d65024bb5a1920ca2959c2` |
| ScenesChange | `2bc350ca4d061aa4284be68f689cc26b` |
| TripoSimpleUI_Manager | `09f032cc6eccbc44bad744f19f10d60f` |
| TripoRuntimeCore | `73b0d52bf358f9245996499886128aae` |
| TripoModelLoader | `7fc354af441a35b4eab3dd86d3f8d513` |
| GltfImport | `b781fe673a5534e91b1e802df4b9362e` |
| EventSystem | `76c392e42b5098c458856cdf6ecaaaa1` |
| StandaloneInputModule | `4f231c4fb786f3946a6b90b886c48677` |
| Image (Unity) | `fe87c0e1cc204ed48ad3b37840f39efc` |
| Text (Legacy) | `5f7201a12d95ffc409449d95f23cf332` |
| Button (Unity) | `4e29b1a8efbd4b44bb3f3716e73f07ff` |
| InputField (Legacy) | `d199490a83bb2b844b9695cbf13b01ef` |

---

## 场景 fileID 索引

| fileID | 物体/组件 |
|--------|-----------|
| 520400733 | Directional Light (GO) |
| 520400734 | Directional Light (Light) |
| 520400735 | Directional Light (Transform) |
| 520401423 | PointableUI (PrefabInstance) |
| 520401424 | PointableUI Canvas (RectTransform, stripped) |
| 520401935 | [RKInput] (PrefabInstance) |
| 520402340 | RKCameraRig (PrefabInstance) |
| 1984809885 | EventSystem (GO) |
| 222279799 | Tripo_Manager (GO) |
| 222279800 | TripoSimpleUI_Manager (组件) |
| 222279801 | TripoRuntimeCore (组件) |
| 222279802 | Tripo_Manager (Transform) |
| 2107462275 | SimpleGameObject (GO) |
| 2107462279 | SimpleGameObject (Transform) |
| 2107462277 | TripoModelLoader (组件) |
| 2107462278 | GltfImport (组件) |
| 1537883603 | Panel — Tripo 设置面板 (GO) |
| 1537883607 | Panel — Tripo 设置面板 (RectTransform) |

---

## 待完成事项

### 1. 创建空物体并挂载脚本

在场景根级创建以下三个空 GameObject：

| 名称 | 脚本 | 说明 |
|------|------|------|
| ASRmanager | ExternalInputManager + AudioSource | 语音识别管理器 |
| BordController | DrawingScreenshotter + ScenesChange | 画板控制器 |
| GlobalUIManager | GlobalUIManager | UI 总管单例 |

### 2. Inspector 绑定

创建完空物体后，需在 Inspector 中完成以下绑定：

**ExternalInputManager (ASRmanager 上):**
- `targetUIManager` → Tripo_Manager 上的 TripoSimpleUI_Manager
- `screenshotter` → BordController 上的 DrawingScreenshotter
- `apiKey` → 填写 Tripo API Key
- `asrUploadURL` → `隐私`

**DrawingScreenshotter (BordController 上):**
- `simpleModel` → SimpleGameObject
- `progressSlider` → 进度条 Slider（如有）

**GlobalUIManager:**
- `TripoRuntimeCore` → Tripo_Manager 上的 TripoRuntimeCore
- `tripoSlider` → 进度条 Slider（如有）
- `managedUICanvasGroups` → 按需配置
- `managed3DObjects` → 按需配置

**TripoRuntimeCore.OnModelGenerateComplete:**
- 添加回调 → BordController 上的 DrawingScreenshotter.ChangeBool()

### 3. 可选扩展

以下功能可根据需要后续添加：

- Board（3D画板）— 挂载第三方 DrawingBoard 脚本
- Gear（齿轮按钮）— 挂载 CanvasDragHandle，点击打开设置面板
- BookPro 翻页书 — 多页设置界面
- 画板面板 UI — 画笔颜色/大小控制
- Slider 进度条 — 显示生成进度
- Camera_Position_W — 相机位置标记

---

## 迁移注意事项

1. 脚本路径为 `Assets/kiro/Scripts/3DGenerate/`（不是原项目的 `AZ/3DGenerate/Scripts/`）
2. Tripo SDK 组件（TripoSimpleUI_Manager、TripoRuntimeCore）已由用户手动绑定完成
3. Panel 内的 Tripo UI 控件（InputField、Button、Dropdown 等）已全部搭建完成
4. SimpleGameObject 当前无 BoxCollider，DrawingScreenshotter 会在运行时通过 GetComponent 查找
5. ASR 服务器地址: `隐私`
6. 确保 AllSettingCtr 单例在主界面场景的 RKCameraRig 上已挂载（DontDestroyOnLoad）
7. 在 Build Settings 中添加 3DGenerate 场景
