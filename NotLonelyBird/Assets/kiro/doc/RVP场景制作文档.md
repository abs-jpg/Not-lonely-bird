# RVP 场景制作文档

## 场景概述

RVP（Rapid Visual Processing，快速视觉信息处理）是一个注意力测试场景。系统随机呈现手势图片，玩家需要在目标手势出现时模仿对应手势。支持 Rokid SDK 手势识别和键盘模拟两种输入方式。

场景路径：`Assets/kiro/RVP.unity`

---

## 场景层级结构

```
RVP.unity
├── Directional Light              — 方向光 (0,3,0) Rot(50,-30,0)
├── [RKInput] (Prefab)             — XR 输入系统
├── RKCameraRig (Prefab)           — XR 相机 + AllSettingCtr
├── PointableUI (Prefab)           — World Space Canvas (0,-0.05,0.5)
│   ├── 指令阶段                   — 指令面板
│   │   ├── Text (TMP)            — 指令文字 "以下图片出现时\n模仿手势:"
│   │   ├── GesturePanel           — 目标手势图片容器 (HorizontalLayoutGroup)
│   │   └── 1 (TMP)               — 倒计时文字 "5秒后开始..."
│   ├── 测试阶段                   — 测试面板
│   │   ├── 手势 (Image)          — 当前刺激手势图片 (300×300)
│   │   └── TF (TMP)              — 正误提示 ✓/✗
│   ├── 结束阶段                   — 结果面板
│   │   └── Text (TMP)            — 结果文字
│   └── 返回按钮 (Prefab)          — 返回 AttentionReady 场景
└── RVPMgr                         — 游戏管理器
    ├── GestureManager             — 核心游戏逻辑
    ├── GestureInputController     — 手势输入检测
    └── RVPSettlementScreen        — 测试配置参数
```

---

## 业务逻辑层

### GestureManager.cs — 核心游戏管理器

脚本路径：`Assets/kiro/Scripts/RVP/GestureManager.cs`
GUID: `79f8d4c30d5b0a84bb60540b0ee146da`

#### 职责

| 功能 | 说明 |
|------|------|
| 参数读取 | 从 AllSettingCtr 单例读取注意力测试参数 |
| 目标选取 | 从手势库随机选取 targetCount 个目标手势 |
| 序列生成 | 按 40% 目标概率生成刺激序列 |
| 游戏流程 | 协程驱动：指令 → 测试 → 结果 |
| 计分 | 命中/漏报/虚报/正确拒绝 + 反应时间 |
| UI 更新 | 切换三个面板，更新提示文字和图片 |

#### 游戏流程

```
Awake() → 读取 AllSettingCtr 参数（或使用默认值）
Start() → 绑定手势事件 → StartCoroutine(RunGame)

RunGame 协程:
  ├── 阶段1：指令
  │   ├── 显示指令面板
  │   ├── 随机选取目标手势
  │   ├── 在 GesturePanel 中动态生成目标手势图片
  │   └── 倒计时 5 秒
  ├── 阶段2：测试
  │   ├── 显示测试面板，启用手势检测
  │   ├── 生成刺激序列（40% 目标概率）
  │   └── for 每个刺激:
  │       ├── 显示手势图片
  │       ├── 等待 stimulusInterval 秒
  │       ├── 隐藏图片（闪烁）
  │       └── 判定：已响应 → 命中/虚报
  │                 未响应 → 漏报/正确拒绝
  └── 阶段3：结果
      ├── 显示结果面板
      └── 显示正确率、平均反应时间、各项统计
```

#### Inspector 绑定

| 字段 | 类型 | 绑定目标 (fileID) |
|------|------|-------------------|
| allGestures[0-7] | GestureData[] | 8 个 ScriptableObject 资产 |
| settings | RVPSettlementScreen | RVPMgr/RVPSettlementScreen (1103373574) |
| contentPanel | RectTransform | GesturePanel (32709722) |
| timeText | TextMeshProUGUI | 指令阶段/1 (1655661923) |
| tfHint | TextMeshProUGUI | 测试阶段/TF (1393169967) |
| stimulusImage | Image | 测试阶段/手势 (429490069) |
| instructionText | TextMeshProUGUI | 指令阶段/Text (2142538366) |
| resultText | TextMeshProUGUI | 结束阶段/Text (1002354649) |
| instructionPanel | GameObject | 指令阶段 (611165797) |
| gesturePanel | GameObject | 测试阶段 (838505371) |
| resultPanel | GameObject | 结束阶段 (451458576) |

#### 运行时默认参数

| 参数 | 默认值 | 说明 |
|------|--------|------|
| testDuration | 60s | 测试总时长 |
| gesturesPerMinute | 30 | 每分钟呈现手势数 |
| targetCount | 3 | 目标手势数量 |
| flashDuration | 0.1s | 图片隐藏闪烁时长 |
| 目标概率 | 40% | 目标手势在序列中的出现概率 |

---

### GestureInputController.cs — 手势输入控制器

脚本路径：`Assets/kiro/Scripts/RVP/GestureInputController.cs`
GUID: `d7bf17fca0acf90448980ef63c6f3a74`

#### 职责

| 功能 | 说明 |
|------|------|
| Rokid SDK 检测 | 通过 `ROKID_SDK` 条件编译使用 Rokid 手势 API |
| 键盘模拟 | 无 SDK 时用键盘模拟（Q/W/E/R 左手，U/I/O/P 右手） |
| 事件触发 | 静态事件 `OnGesturePerformed(Hand, CustomGestureType)` |
| 启停控制 | `SetDetectionEnabled(bool)` 控制检测开关 |

#### 键盘映射

| 按键 | 手 | 手势 |
|------|-----|------|
| Q | 左 | 握拳 |
| W | 左 | 捏合 |
| E | 左 | 掌朝前 |
| R | 左 | 掌朝上 |
| U | 右 | 握拳 |
| I | 右 | 捏合 |
| O | 右 | 掌朝前 |
| P | 右 | 掌朝上 |

---

### GestureData.cs — 手势数据 ScriptableObject

脚本路径：`Assets/kiro/Scripts/RVP/GestureData.cs`
GUID: `16e6b0c881286e64d9342c58ca10e730`

#### 枚举定义

```csharp
enum Hand { Left = 0, Right = 1 }
enum CustomGestureType { None = 0, Grip = 1, Pinch = 2, PalmForward = 3, PalmUp = 4 }
```

#### 字段

| 字段 | 类型 | 说明 |
|------|------|------|
| gestureName | string | 手势名称 |
| gestureImage | Sprite | 手势图片 |
| hand | Hand | 左/右手 |
| gestureType | CustomGestureType | 手势类型 |

#### 8 个 ScriptableObject 资产

| 资产名 | GUID | 手 | 类型 | 图片 |
|--------|------|-----|------|------|
| 左握拳 | `46870e6b604fa5144a15098c6053590d` | Left | Grip | 左握拳.png |
| 左捏合 | `0f7b93139247b5a47b7383e4ceb0b02c` | Left | Pinch | 左捏合.png |
| 左手掌朝前 | `0c262722b339969439946deb56552873` | Left | PalmForward | 左手掌朝前.png |
| 左手掌朝上 | `6bfbdef9b5c70e24e8fade3a92fecfae` | Left | PalmUp | 左手掌朝上.png |
| 右握拳 | `a3d9080427a249148afd865de46fc3a3` | Right | Grip | 右握拳.png |
| 右捏合 | `42df1c5acc274254198c60182400553a` | Right | Pinch | 右捏合.png |
| 右手掌朝前 | `295b266546da52b459ea62945f980de0` | Right | PalmForward | 右手掌朝前.png |
| 右手掌朝上 | `a02c0a20e83d1514f89799a090e11ac7` | Right | PalmUp | 右手掌朝上.png |

---

### RVPSettlementScreen.cs — RVP 测试配置

脚本路径：`Assets/kiro/Scripts/RVP/RVPSettlementScreen.cs`
GUID: `2ce60687ae7aa56468d0fa2ad782e815`

Inspector 可调参数，用于配置 RVP 测试。

| 字段 | 类型 | 默认值 | 说明 |
|------|------|--------|------|
| testDuration | float | 60 | 测试时长（秒） |
| gesturesPerMinute | int | 30 | 每分钟手势数 |
| targetCount | int | 3 | 目标手势数量 |
| flashDuration | float | 0.1 | 闪烁时长（秒） |
| targetProbability | float | 0.4 | 目标出现概率 |

---

### AllSettingCtr.cs — 全局设置（RVP 相关字段）

脚本路径：`Assets/kiro/Scripts/NBack/AllSettingCtr.cs`

新增字段：

| 字段 | 类型 | 默认值 | 说明 |
|------|------|--------|------|
| attentionGesturesPerMinute | int | 40 | 每分钟手势呈现数 |
| attentionTargetCount | int | 3 | 目标手势数量 |
| attentionFlashDuration | float | 0.1 | 闪烁时长（秒） |

---

## UI 组件层

### PointableUI (World Space Canvas)

- Prefab GUID: `3c20833d81e354626b8365b459274912`
- Position: (0, -0.05, 0.5)
- 4 个子面板通过 AddedGameObjects 挂载

### 指令阶段 (instructionPanel)

| 属性 | 值 |
|------|-----|
| fileID | 611165797 |
| Anchor | Stretch (0,0)-(1,1) |
| SizeDelta | (-1044.8, -587) |
| Image | 禁用，半透明背景 |

子物体：
- Text (TMP) — 指令文字，字号 35，默认 "以下图片出现时\n模仿手势:"
- GesturePanel — HorizontalLayoutGroup，Spacing=10，运行时动态填充目标手势图片
- 1 (TMP) — 倒计时文字，字号 36，默认 "5秒后开始..."

### 测试阶段 (gesturePanel)

| 属性 | 值 |
|------|-----|
| fileID | 838505371 |
| Anchor | Stretch (0,0)-(1,1) |
| SizeDelta | (-1044.8, -587) |

子物体：
- 手势 (Image) — 300×300，显示当前刺激手势图片
- TF (TMP) — 正误提示，字号 36，运行时显示 ✓ 或 ✗

### 结束阶段 (resultPanel)

| 属性 | 值 |
|------|-----|
| fileID | 451458576 |
| Anchor | Stretch (0,0)-(1,1) |
| SizeDelta | (-1044.8, -587) |

子物体：
- Text (TMP) — 结果文字，字号 45，显示正确率、反应时间等统计

### 返回按钮

- Prefab GUID: `9e80c59a384a9d64c8b5810d6d1718e6`
- AnchoredPosition: (-370.3, 192.9)
- SizeDelta: (109.5, 70.3)
- ScenesChange → SceneChange("AttentionReady")

---

## 脚本 GUID 对照

| 脚本 | GUID |
|------|------|
| GestureManager | `79f8d4c30d5b0a84bb60540b0ee146da` |
| GestureInputController | `d7bf17fca0acf90448980ef63c6f3a74` |
| GestureData | `16e6b0c881286e64d9342c58ca10e730` |
| RVPSettlementScreen | `2ce60687ae7aa56468d0fa2ad782e815` |
| AllSettingCtr | `a79441f348de89743a2939f4d699eac1` |
| ScenesChange | `2bc350ca4d061aa4284be68f689cc26b` |
| TextMeshProUGUI | `f4688fdb7df04437aeb418b961361dc5` |
| Image (Unity) | `fe87c0e1cc204ed48ad3b37840f39efc` |
| HorizontalLayoutGroup | `59f8146938fff824cb5fd77236b75775` |

---

## 场景 fileID 索引

| fileID | 物体 |
|--------|------|
| 1103373569 | RVPMgr GameObject |
| 1103373571 | RVPMgr Transform |
| 1103373572 | GestureInputController 组件 |
| 1103373573 | GestureManager 组件 |
| 1103373574 | RVPSettlementScreen 组件 |
| 611165797 | 指令阶段 GameObject |
| 838505371 | 测试阶段 GameObject |
| 451458576 | 结束阶段 GameObject |
| 32709721 | GesturePanel GameObject |
| 429490067 | 手势 Image GameObject |
| 1393169964 | TF GameObject |
| 1655661921 | 倒计时文字 GameObject |
| 2142538364 | 指令文字 GameObject |
| 1002354647 | 结果文字 GameObject |
| 1828864901 | Directional Light |

---

## 制作过程记录

1. 场景 `RVP.unity` 已预先搭建好 UI 层级（指令/测试/结束三个面板、手势图片、返回按钮等）
2. 创建 `GestureData.cs` ScriptableObject 定义手势枚举和数据结构
3. 创建 8 个 GestureData 资产文件（左右手 × 握拳/捏合/掌朝前/掌朝上），GUID 与场景引用匹配
4. 创建 `GestureInputController.cs` 支持 Rokid SDK 手势检测（条件编译）和键盘模拟
5. 创建 `GestureManager.cs` 实现三阶段游戏流程，协程驱动
6. 创建 `RVPSettlementScreen.cs` 作为 RVP 专用配置组件（GUID 与 NBack 的 SettlementScreen 不同）
7. 在 `AllSettingCtr.cs` 中添加 RVP 相关设置字段
8. 所有 meta 文件 GUID 与场景中的引用一一对应
