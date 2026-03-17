# NBack 场景制作文档

## 场景概述

NBack 是一个空间工作记忆测试游戏场景。玩家在 3×3 九宫格中观察方块出现的位置，判断当前位置是否与 N 步之前相同，通过点击"匹配"按钮作答。支持 N=0（判断是否在中心）到 N≥1 的多种难度模式。

场景路径：`Assets/kiro/NBack.unity`

---

## 场景层级结构

```
NBack.unity
├── Directional Light              — 方向光 (0,3,0) Rot(50,-30,0)
├── [RKInput] (Prefab)             — XR 输入系统
├── RKCameraRig (Prefab)           — XR 相机
├── PointableUI (Prefab)           — World Space Canvas (0,-0.099,0.4)
│   ├── 提示 (TextMeshProUGUI)     — 游戏提示/反馈文字
│   ├── 提示 (1) (TextMeshProUGUI) — 副提示文字
│   ├── 白底按钮1 (Button)         — 匹配按钮
│   │   └── Text (TMP)            — 按钮文字 "匹配时点击"
│   └── 返回按钮 (Button)          — 返回主界面按钮
│       └── Text (TMP)            — 按钮文字 "返回"
├── NBackMgr                       — 游戏管理器
│   ├── NBackManager               — 核心游戏逻辑
│   └── SettlementScreen           — 方块生成配置
├── 物体                           — 九宫格方块父节点 (-0.0386,-0.1952,0.449)
│   ├── 1 (Cube, inactive)
│   ├── 2 (Cube, inactive)
│   ├── 3 (Cube, inactive)
│   ├── 4 (Cube, inactive)
│   ├── 5 (Cube, inactive)
│   ├── 6 (Cube, inactive)
│   ├── 7 (Cube, inactive)
│   ├── 8 (Cube, inactive)
│   └── 9 (Cube, inactive)
├── NBackObj1                      — 3D 刺激物 Cube (Scale 0.03)
├── RootGrid                       — 底板网格父节点 (-0.0386,-0.1952,0.49)
│   └── TempTile × 9              — 底板格子
└── Score                          — 3D TextMeshPro 进度显示 (跟随头部)
```

---

## 业务逻辑层

### NBackManager.cs — 核心游戏管理器

脚本路径：`Assets/kiro/Scripts/NBack/NBackManager.cs`
GUID: `6d1d2e4692a8c7142b5c4e8a21d4099a`

#### 职责

| 功能 | 说明 |
|------|------|
| 参数读取 | 从 AllSettingCtr 单例读取 N 值、时长、试次数等 |
| 序列生成 | 按匹配概率生成位置序列 |
| 游戏流程 | 协程驱动：准备 → 逐试次展示 → 结算 |
| 计分 | 正确匹配/正确拒绝 +1 分 |
| UI 更新 | 实时更新提示文字、按钮文字、分数显示 |

#### 游戏流程

```
Awake() → 读取 AllSettingCtr 参数（或使用默认值）
Start() → 绑定按钮事件 → GenerateSequence() → StartCoroutine(RunGame)

RunGame 协程:
  ├── 显示 "准备开始..." (2秒)
  └── for 每个试次:
      ├── 移动刺激物到目标格子位置
      ├── 激活刺激物，显示试次信息
      ├── 等待 stimulusDuration 秒
      ├── 隐藏刺激物
      ├── 判定：已响应 → 按匹配结果计分
      │         未响应 → 非匹配则 +1（正确拒绝）
      ├── 更新分数
      └── 等待 interStimulusInterval 秒
  
OnGameEnd() → 显示正确率和得分
```

#### Inspector 绑定

| 字段 | 类型 | 绑定目标 |
|------|------|----------|
| gridCells[0-8] | GameObject[] | 物体/1 ~ 物体/9 |
| stimulus3DObject | GameObject | NBackObj1 |
| feedbackText | TextMeshProUGUI | 提示 |
| matchButton | Button | 白底按钮1 |
| buttonText | TextMeshProUGUI | 白底按钮1/Text |
| scoreText | TextMeshPro | Score |
| settings | SettlementScreen | 同 GameObject 上的 SettlementScreen |

#### 运行时默认参数

| 参数 | 默认值 | 说明 |
|------|--------|------|
| nValue | 1 | N-Back 的 N 值 |
| stimulusDuration | 2.0s | 刺激显示时长 |
| interStimulusInterval | 2.5s | 刺激间隔 |
| totalTrials | 40 | 总试次 |
| targetIndexN0 | 4 | N=0 目标位置（中心） |
| matchProbability | 0.33 | 匹配概率 |

---

### SettlementScreen.cs — 方块生成配置

脚本路径：`Assets/kiro/Scripts/Json/SettlementScreen.cs`
GUID: `8f6a5c49553757149a071d26d614379c`

Inspector 可调参数，用于配置九宫格方块的生成规则。

| 字段 | 类型 | 默认值 | 说明 |
|------|------|--------|------|
| rows | int | 3 | 行数 |
| columns | int | 3 | 列数 |
| spacing | float | 0.05 | 格子间距 |
| cubeScale | Vector3 | (0.03, 0.03, 0.03) | 方块缩放 |
| cubeMaterial | Material | null | 方块材质（留空用默认） |
| cubePrefab | GameObject | null | 自定义方块 Prefab |
| tilePrefab | GameObject | null | 底板 Prefab |
| tileParentScale | Vector3 | (0.05, 0, 0.05) | 底板父节点缩放 |

提供两个方法：
- `GenerateGrid(Transform parent)` → 生成九宫格方块，返回 GameObject[]
- `GenerateTiles(Transform parent)` → 生成底板网格

---

### ScenesChange.cs — 场景切换

脚本路径：`Assets/kiro/Scripts/NBack/ScenesChange.cs`
GUID: `2bc350ca4d061aa4284be68f689cc26b`

```csharp
public string sceneName;
public void SceneChange(string sceneName)
{
    SceneManager.LoadScene(sceneName);
}
```

返回按钮挂载此脚本，`sceneName` 设为 "主界面"，Button OnClick 绑定 `SceneChange("主界面")`。

---

### FollowHead.cs — 头部跟随

脚本路径：`Assets/kiro/Scripts/NBack/FollowHead.cs`
GUID: `0b0bba19870aa524c90ac965f01a9685`

挂载在 Score 物体上，使 3D 文字始终跟随 XR 头部（主摄像机）。

| 字段 | 类型 | 默认值 | 说明 |
|------|------|--------|------|
| offsetPosition | Vector3 | (16, 9.5, 60) | 相对头部的偏移量 |

LateUpdate 中：
```
position = camera.position + camera.TransformDirection(offset)
rotation = camera.rotation
```

---

## UI 组件层

### PointableUI (World Space Canvas)

- Prefab GUID: `3c20833d81e354626b8365b459274912`
- Position: (0, -0.099, 0.4)
- 4 个子物体通过 AddedGameObjects 挂载

### 提示 (feedbackText)

| 属性 | 值 |
|------|-----|
| 类型 | TextMeshProUGUI |
| AnchoredPosition | (0, 298) |
| SizeDelta | 821.9 × 110.2 |
| 字号 | 30 |
| 字体颜色 | 白色 |
| 对齐 | 居中 |
| 默认文本 | "↓↓ 向下低头看向平面进行游戏 ↓↓" |
| 字体 | guid: `9f11f16ef08ecf04585e68528cac76fe` |

运行时文本变化：
- 游戏开始前："准备开始..."
- 游戏中："试次 X / 40"
- 游戏结束："游戏结束!\n正确率: XX%\n得分: X / 40"

### 白底按钮1 (matchButton)

| 属性 | 值 |
|------|-----|
| AnchoredPosition | (231.5, 167.1) |
| SizeDelta | 204.9 × 80 |
| Scale | (1, 1, 1) |
| Image 颜色 | RGB(0.655, 0.525, 0.875) 淡紫色 |
| Button TargetGraphic | 自身 Image |
| OnClick | 代码绑定 NBackManager.OnMatchButtonClicked |

子物体 Text：
- 默认文本："匹配时点击"
- 运行时变化："正确!" / "错误!" / "已结束"

### 返回按钮

| 属性 | 值 |
|------|-----|
| AnchoredPosition | (-277, 383) |
| SizeDelta | 190 × 130 |
| Scale | (0.5, 0.5, 0.5) |
| Image 颜色 | RGB(0.655, 0.525, 0.875) 淡紫色 |
| ScenesChange.sceneName | "主界面" |
| Button OnClick | ScenesChange.SceneChange("主界面") |

子物体 Text：
- 文本："返回"
- 字号：40

### Score (3D TextMeshPro)

| 属性 | 值 |
|------|-----|
| 类型 | TextMeshPro (3D) |
| AnchoredPosition | (16, 9.5) |
| SizeDelta | 20 × 5 |
| LocalPosition.z | 60 |
| 字号 | 36 |
| 字体颜色 | 白色 |
| 默认文本 | "40/40" |
| FollowHead offset | (16, 9.5, 60) |

运行时格式："{当前得分} / {当前试次+1}"

---

## 3D 物体层

### 物体（九宫格父节点）

- Position: (-0.0386, -0.1952, 0.449)
- 包含 9 个 Cube 子物体，初始全部 inactive
- 每个 Cube：Scale (0.03, 0.03, 0.03)，带 MeshFilter + MeshRenderer + BoxCollider

#### 九宫格布局（LocalPosition）

| 格子 | X | Y | Z |
|------|-------|-------|-------|
| 1 | -0.05 | 0.015 | 0.09 |
| 2 | 0 | 0.015 | 0.09 |
| 3 | 0.05 | 0.015 | 0.09 |
| 4 | -0.05 | 0.015 | 0.045 |
| 5 | 0 | 0.015 | 0.045 |
| 6 | 0.05 | 0.015 | 0.045 |
| 7 | -0.05 | 0.015 | 0 |
| 8 | 0 | 0.015 | 0 |
| 9 | 0.05 | 0.015 | 0 |

### NBackObj1（刺激物）

- 独立 Cube，Scale (0.03, 0.03, 0.03)
- 运行时由 NBackManager 控制位置和激活状态
- 每个试次移动到目标格子的 position

### RootGrid（底板网格）

- Position: (-0.0386, -0.1952, 0.49)
- Scale: (0.05, 0, 0.05)
- 包含 9 个 TempTile 子物体

---

## 类型定义

### 脚本 GUID 对照

| 脚本 | GUID |
|------|------|
| NBackManager | `6d1d2e4692a8c7142b5c4e8a21d4099a` |
| SettlementScreen | `8f6a5c49553757149a071d26d614379c` |
| FollowHead | `0b0bba19870aa524c90ac965f01a9685` |
| ScenesChange | `2bc350ca4d061aa4284be68f689cc26b` |
| AllSettingCtr | `a79441f348de89743a2939f4d699eac1` |
| Button (Unity) | `4e29b1a8efbd4b44bb3f5571e532d8e8` |
| Image (Unity) | `fe87c0e1cc204ed48ad3b37840f39efc` |
| TextMeshProUGUI | `f4688fdb7df04437aeb418b961361dc5` |

### 场景 fileID 索引

| fileID | 物体 |
|--------|------|
| 500000001 | NBackMgr GameObject |
| 500000003 | NBackManager 组件 |
| 500000004 | SettlementScreen 组件 |
| 600000001~081 | 九宫格 Cube 1~9 (步长10) |
| 700000001 | NBackObj1 刺激物 |
| 747870313 | 返回按钮 |
| 747870318 | ScenesChange 组件 |
| 910000001 | 白底按钮1 |
| 910000005 | Button 组件 (匹配按钮) |
| 911000004 | buttonText (TextMeshProUGUI) |
| 930000004 | Score TextMeshPro 3D |

---

## 制作过程记录

1. 创建所有 C# 脚本（AllSettingCtr、NBackManager、NBackSetting、ScenesChange、FollowHead、SettlementScreen）
2. SettlementScreen 最初使用 JSON 持久化，后改为纯 Inspector 配置的方块生成器
3. ScenesChange 简化为一行实现，后由用户重写添加 `public string sceneName` 字段
4. 场景文件通过 Python 脚本 `_build_scene.py` 生成完整 YAML，包含所有 UI 绑定
5. 返回按钮的 Button OnClick 事件绑定 ScenesChange.SceneChange("主界面")
6. 匹配按钮的点击通过 NBackManager 代码中 `AddListener` 绑定
