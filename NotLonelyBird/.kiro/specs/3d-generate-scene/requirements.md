# 需求文档 — 3DGenerate 场景

## 简介

3DGenerate（虚实绘景）是一个 XR 创意场景，用户可通过三种方式生成3D模型：画板绘图转3D、文字描述转3D、语音输入转3D。场景集成 Tripo AI SDK 进行模型生成，使用 Rokid SDK 提供手势射线交互，使用 BookPro 翻页组件提供多页设置界面。生成的3D模型支持手势抓取交互。

场景路径：`Assets/kiro/3DGenerate.unity`

## 术语表

- **Scene（场景）**: Unity 3DGenerate.unity 场景文件及其包含的所有 GameObject
- **DrawingBoard（画板）**: 第三方3D画板组件，用户可在其上绘制图案，挂载 DrawingBoard 脚本
- **TripoAPI**: Tripo AI SDK 提供的3D模型生成服务，包含 TripoRuntimeCore、TripoSimpleUI_Manager、TripoModelLoader 等组件
- **BookPro**: 第三方翻页书组件，提供 Book + AutoFlip 脚本实现多页翻转效果
- **ASR（语音识别）**: 自动语音识别服务，将录音上传至服务器获取文字转写结果
- **GlobalUIManager**: 单例 UI 管理器，集中控制所有 UI 面板和3D物体的显隐
- **DrawingScreenshotter**: 截取画板内容并调用 Tripo ImageToModel API 生成3D模型的脚本
- **ExternalInputManager**: ASR 语音录音、上传、识别并触发文字转3D模型的脚本
- **SimpleGameObject（模型容器）**: 生成的3D模型放置的容器对象，支持 OVRGrabbable 手势抓取
- **PointableUI**: Rokid SDK 提供的 World Space Canvas Prefab，用于 XR 射线交互 UI
- **RKCameraRig**: Rokid SDK 的 XR 相机 Prefab
- **RKInput**: Rokid SDK 的 XR 输入系统 Prefab
- **CanvasDragHandle**: 使 UI 元素可通过射线拖拽移动的脚本
- **Gear（齿轮按钮）**: 3D 齿轮图标，点击后打开/关闭设置面板（BookPro 翻页书）
- **AllSettingCtr**: 全局 DontDestroyOnLoad 单例，跨场景传递设置参数
- **ScenesChange**: 通用场景切换脚本，通过 SceneManager.LoadScene 跳转场景
- **BordController**: 挂载 DrawingScreenshotter 的空 GameObject，作为画板控制器

## 需求

### 需求 1：场景基础结构搭建

**用户故事：** 作为开发者，我需要搭建 3DGenerate 场景的基础层级结构，以便所有功能模块有正确的 GameObject 承载。

#### 验收标准

1. THE Scene SHALL 包含以下13个根级 GameObject：Directional Light、[RKInput]、PointableUI、RKCameraRig、Board、Gear、SimpleGameObject、Camera_Position_W、Tripo_Manager、ASRmanager、GlobalUIManager、BordController
2. WHEN Scene 加载时，THE [RKInput] SHALL 使用 Rokid SDK Prefab（GUID: `fa2ab4a52b98844a5beea51c6d5ab85a`）实例化
3. WHEN Scene 加载时，THE PointableUI SHALL 使用 Rokid SDK Prefab（GUID: `3c20833d81e354626b8365b459274912`）实例化，Position 设为 (0, 0, 0.5)
4. WHEN Scene 加载时，THE RKCameraRig SHALL 使用 Rokid SDK Prefab（GUID: `bc7bf2e56b74d4038af31f75d0b2d024`）实例化
5. THE Directional Light SHALL 配置为方向光，Position (0, 3, 0)，Rotation (50, -30, 0)

### 需求 2：BookPro 翻页设置界面

**用户故事：** 作为用户，我需要一个多页翻页设置界面，以便在不同页面中选择3D生成方式和配置参数。

#### 验收标准

1. THE PointableUI 内部 Canvas SHALL 包含一个名为 "book" 的子对象，挂载 Book 和 AutoFlip 组件
2. THE book SHALL 包含多个页面（Page），每个页面为 book 的子 RectTransform
3. WHEN 用户通过射线交互翻页时，THE AutoFlip SHALL 自动播放翻页动画切换到下一页或上一页
4. THE book 的第一页 SHALL 显示功能介绍或封面内容
5. THE book 的功能页 SHALL 包含"图片生成"按钮，点击后调用 DrawingScreenshotter.CaptureAndSaveToFile() 截取画板并生成3D模型
6. THE book 的功能页 SHALL 包含"文字生成"输入区域，用户输入文字描述后通过 TripoSimpleUI_Manager 触发 TextToModel 生成
7. THE book 的功能页 SHALL 包含"语音生成"按钮，点击后调用 ExternalInputManager.ToggleRecording() 开始/停止录音

### 需求 3：3D 画板绘图功能

**用户故事：** 作为用户，我需要一个3D画板在 XR 空间中绘图，以便将绘制的图案转换为3D模型。

#### 验收标准

1. THE Board SHALL 为一个3D平面对象，挂载第三方 DrawingBoard 脚本，设置在 Layer 6
2. THE Board SHALL 支持用户通过 XR 射线在画板表面绘制图案
3. THE PointableUI Canvas SHALL 包含一个名为 "画板面板" 的子面板，提供画笔颜色和画笔大小的控制 UI
4. THE 画板面板 SHALL 包含颜色选择按钮，允许用户切换画笔颜色
5. THE 画板面板 SHALL 包含画笔大小调节控件，允许用户调整画笔粗细
6. WHEN 用户点击 book 中的"图片生成"按钮时，THE DrawingScreenshotter SHALL 截取 DrawingBoard 的 drawingTexture 并保存为 PNG 文件
7. WHEN 截图保存成功后，THE DrawingScreenshotter SHALL 调用 TripoRuntimeCore.SetImagePath 设置图片路径并调用 ImageToModel 开始生成3D模型

### 需求 4：文字转3D模型功能

**用户故事：** 作为用户，我需要通过输入文字描述来生成3D模型，以便快速创建想要的3D内容。

#### 验收标准

1. THE book 功能页 SHALL 包含一个 TMP_InputField 供用户输入文字描述
2. WHEN 用户输入文字并点击生成按钮时，THE TripoSimpleUI_Manager SHALL 将文字设置到 TextPromptInputField 并触发 btnTextToModelGenerate 的 onClick 事件
3. WHEN 文字转3D生成开始时，THE GlobalUIManager SHALL 调用 HideAllManagedItems() 隐藏画板面板和3D画板等对象
4. WHEN 3D模型生成过程中，THE GlobalUIManager SHALL 通过 tripoSlider 显示生成进度（0~1）

### 需求 5：语音转3D模型功能

**用户故事：** 作为用户，我需要通过语音描述来生成3D模型，以便在 XR 环境中免手动输入。

#### 验收标准

1. THE ASRmanager SHALL 挂载 ExternalInputManager 脚本和 AudioSource 组件
2. WHEN 用户点击语音生成按钮时，THE ExternalInputManager SHALL 调用 ToggleRecording() 开始麦克风录音（16000Hz，最长30秒）
3. WHEN 用户再次点击语音按钮时，THE ExternalInputManager SHALL 停止录音并将音频保存为 WAV 文件
4. WHEN 录音保存完成后，THE ExternalInputManager SHALL 将 WAV 文件上传至 ASR 服务器（asrUploadURL）
5. WHEN ASR 服务器返回识别结果时，THE ExternalInputManager SHALL 解析 raw_transcription 字段获取文字
6. WHEN 文字获取成功后，THE ExternalInputManager SHALL 调用 GlobalUIManager.HideAllManagedItems() 隐藏 UI，然后将文字设置到 TripoSimpleUI_Manager 的 TextPromptInputField 并触发生成
7. IF ASR 上传失败，THEN THE ExternalInputManager SHALL 在 Debug.LogError 中记录错误信息

### 需求 6：3D模型生成与展示

**用户故事：** 作为用户，我需要生成的3D模型能正确加载并展示在场景中，以便查看和交互。

#### 验收标准

1. THE Tripo_Manager SHALL 挂载 TripoRuntimeCore、TripoSimpleUI_Manager、TripoModelLoader 组件
2. THE Tripo_Manager SHALL 挂载 GltfImport 组件用于加载 glTF 格式的3D模型
3. WHEN 3D模型生成完成时，THE TripoModelLoader SHALL 将模型加载到 SimpleGameObject 容器中
4. THE SimpleGameObject SHALL 初始状态下 BoxCollider 设为 disabled
5. WHEN 模型生成完成回调 DrawingScreenshotter.ChangeBool() 被调用时，THE SimpleGameObject 的 BoxCollider SHALL 设为 enabled，允许用户抓取
6. THE Scene SHALL 包含 glTF-FastLit.shader 的材质依赖，用于渲染生成的3D模型

### 需求 7：XR 手势抓取交互

**用户故事：** 作为用户，我需要能用手势抓取生成的3D模型，以便在 XR 空间中自由移动和查看模型。

#### 验收标准

1. THE SimpleGameObject SHALL 挂载 OVRGrabbable 组件（或等效的 Meta Interaction SDK 抓取组件）
2. THE SimpleGameObject SHALL 挂载 GrabInteractable 和 ColliderSurface 组件，配合 Rokid SDK 手势射线实现抓取
3. WHEN 用户通过手势射线选中 SimpleGameObject 并执行抓取手势时，THE SimpleGameObject SHALL 跟随用户手部移动
4. WHEN 用户释放抓取手势时，THE SimpleGameObject SHALL 停留在释放位置

### 需求 8：Gear 齿轮设置按钮

**用户故事：** 作为用户，我需要一个可拖拽的齿轮按钮来打开设置面板，以便随时调整生成参数。

#### 验收标准

1. THE Gear SHALL 为一个3D对象，使用齿轮模型 Prefab（Assets/kiro/Prefabs/Gear/）
2. THE Gear SHALL 挂载 CanvasDragHandle 脚本，允许用户通过射线拖拽移动齿轮位置
3. WHEN 用户点击 Gear 时，THE Scene SHALL 切换 BookPro 翻页书（book）的显隐状态
4. THE Gear SHALL 在场景中始终可见，不受 GlobalUIManager.HideAllManagedItems() 影响

### 需求 9：GlobalUIManager 面板管理

**用户故事：** 作为开发者，我需要一个集中的 UI 管理器来控制所有面板和3D物体的显隐，以便在生成模型时统一切换界面状态。

#### 验收标准

1. THE GlobalUIManager SHALL 作为单例（Singleton）运行，通过 GlobalUIManager.Instance 访问
2. THE GlobalUIManager 的 managedUICanvasGroups 数组 SHALL 包含画板面板的 CanvasGroup（索引1）
3. THE GlobalUIManager 的 managed3DObjects 数组 SHALL 包含画笔模型（索引0）、Board 画板（索引1）、Gear 齿轮（索引2）
4. WHEN HideAllManagedItems() 被调用时，THE GlobalUIManager SHALL 将所有 managedUICanvasGroups 的 alpha 设为0、interactable 设为 false、blocksRaycasts 设为 false
5. WHEN HideAllManagedItems() 被调用时，THE GlobalUIManager SHALL 将所有 managed3DObjects 设为 SetActive(false)
6. WHEN ShowAllManagedItems() 被调用时，THE GlobalUIManager SHALL 恢复所有面板和物体的可见和交互状态
7. THE GlobalUIManager SHALL 持有 TripoRuntimeCore 的引用和 tripoSlider（进度条 Slider）的引用

### 需求 10：进度条 UI

**用户故事：** 作为用户，我需要看到3D模型生成的进度，以便了解当前生成状态。

#### 验收标准

1. THE PointableUI Canvas SHALL 包含一个 Slider 组件作为进度条
2. WHEN 3D模型生成过程中，THE GlobalUIManager SHALL 通过 UpdateSlider(float value) 更新进度条值（范围 0~1）
3. WHEN 生成未开始或已完成时，THE GlobalUIManager SHALL 通过 SetSliderActive(false) 隐藏进度条

### 需求 11：Tripo API Key 自动配置

**用户故事：** 作为开发者，我需要 API Key 在场景启动时自动配置到 Tripo SDK，以便用户无需手动输入即可使用生成功能。

#### 验收标准

1. THE ExternalInputManager SHALL 在 Inspector 中暴露 apiKey 字段供开发者配置
2. WHEN Scene 启动时，THE ExternalInputManager SHALL 自动将 apiKey 设置到 TripoSimpleUI_Manager 的 ApiKeyInputField 中
3. WHEN apiKey 设置完成后，THE ExternalInputManager SHALL 自动触发 btnConfirmApiKey 的 onClick 事件完成 API Key 确认

### 需求 12：场景返回导航

**用户故事：** 作为用户，我需要能从 3DGenerate 场景返回主界面，以便切换到其他功能。

#### 验收标准

1. THE PointableUI Canvas 内的 Panel SHALL 包含一个返回按钮
2. THE 返回按钮 SHALL 挂载 ScenesChange 脚本
3. WHEN 用户点击返回按钮时，THE ScenesChange SHALL 调用 SceneChange("Main") 跳转到主界面场景

### 需求 13：Camera_Position_W 相机位置标记

**用户故事：** 作为开发者，我需要一个相机位置标记对象，以便在特定功能中定位相机或作为参考点。

#### 验收标准

1. THE Camera_Position_W SHALL 为一个空 GameObject，放置在场景根级
2. THE Camera_Position_W SHALL 记录一个预设的世界坐标位置，供运行时脚本引用
