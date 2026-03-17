---

## 🐦 不孤鸟 NotLonelyBird

**基于 AR 技术的儿童认知干预训练系统**

*An AR-based Cognitive Intervention Training System for Children*

![Unity](https://img.shields.io/badge/Unity-2021.3+-black?logo=unity)
![C#](https://img.shields.io/badge/C%23-10.0-239120?logo=csharp)
![Platform](https://img.shields.io/badge/Platform-Rokid%20XR-blue)
![License](https://img.shields.io/badge/License-MIT-green)
![AI Assisted](https://img.shields.io/badge/AI%20Assisted-Kiro-purple)

</div>

---

## 📖 项目简介

"不孤鸟"是一款运行于 **Rokid XR 眼镜**平台的 6DoF AR 应用，面向儿童认知干预训练。系统基于剑桥 **CANTAB 神经心理学范式**，结合 Rokid 手势识别、千问 ASR 语音识别和 Tripo AI 3D 生成技术，提供沉浸式的全感官统合训练体验。

项目涵盖 **记忆力、注意力、执行功能、情感认知、创意疗愈** 五大训练维度，共 **8 个 Unity 场景**、**26+ 个 C# 脚本**、**8 份场景制作文档**和完整的用户操作手册。

---

## ✨ 功能模块

| 模块 | 场景 | 训练目标 | 核心技术 |
|------|------|----------|----------|
| 🧠 数字广度 | DigitSpan | 瞬时记忆、顺序/逆序复述 | ASR 语音识别 + 按钮双模式答题 |
| 🔄 N-Back 回溯 | NBack | 空间工作记忆、信息更新 | 3×3 九宫格空间匹配 |
| 👁️ 快速视觉处理 | RVP | 持续注意力、目标检测 | Rokid 手势识别 + 高频刺激 |
| 🎯 方向判断 | Direction | 抑制控制、资源分配 | 手掌朝前手势 + 一致/冲突范式 |
| 😊 情感认知 | Emotion | 情绪识别、社交认知 | 3D 卡通角色微表情动画 |
| 🎨 虚实绘景 | 3DGenerate | 创意表达、空间认知 | Tripo AI 3D 生成 + AR 画板 |

---

## 🏗️ 技术架构

```
运行环境: YodaOS-Master / UXR 3.0 / XR2 Gen1+
交互方式: 6DoF 射线 + 手势识别（捏合/握拳/手掌朝前）
语音识别: 千问 ASR 模型 API (16kHz WAV)
3D 生成:  Tripo AI SDK (TextToModel / ImageToModel)
动画系统: DOTween Pro + Unity Animator
数据持久: JSON 本地序列化 (Newtonsoft.Json)
```

---

## 📁 项目结构

```
NotLonelyBird/
├── Assets/kiro/
│   ├── Scripts/                          # C# 业务逻辑层
│   │   ├── 3DGenerate/                   # 🎨 虚实绘景模块          — 95% Kiro 开发
│   │   │   ├── GlobalUIManager.cs        #    UI 总管单例            — Kiro 编写
│   │   │   ├── ExternalInputManager.cs   #    ASR 语音识别管理器     — Kiro 编写
│   │   │   ├── DrawingScreenshotter.cs   #    画板截图生成3D         — Kiro 编写
│   │   │   └── BordController.cs         #    画板控制器             — Kiro 编写
│   │   ├── DigitSpan/                    # 🧠 数字广度模块          — 90% Kiro 开发
│   │   │   ├── MemoryGameManager.cs      #    游戏主控制器           — Kiro 编写
│   │   │   ├── ASRManager.cs             #    语音识别管理           — Kiro 编写
│   │   │   ├── MemorySettingsMenu.cs     #    设置菜单               — Kiro 编写
│   │   │   └── SavWav.cs                 #    WAV 录音保存           — Kiro 编写
│   │   ├── NBack/                        # 🔄 N-Back 模块           — 90% Kiro 开发
│   │   │   ├── NBackManager.cs           #    N-Back 主控制器        — Kiro 编写
│   │   │   ├── NBackSetting.cs           #    设置数据类             — Kiro 编写
│   │   │   ├── AllSettingCtr.cs          #    全局设置单例           — Kiro 编写
│   │   │   ├── ScenesChange.cs           #    场景切换               — Kiro 编写
│   │   │   └── FollowHead.cs            #    头部跟随               — Kiro 编写
│   │   ├── RVP/                          # 👁️ 快速视觉处理模块      — 95% Kiro 开发
│   │   │   ├── GestureManager.cs         #    手势管理器             — Kiro 编写
│   │   │   ├── GestureInputController.cs #    手势输入控制           — Kiro 编写
│   │   │   ├── RVPSettlementScreen.cs    #    结算界面               — Kiro 编写
│   │   │   ├── GestureData.cs            #    手势数据定义           — Kiro 编写
│   │   │   └── GestureAssets/            #    8 个手势 ScriptableObject — Kiro 生成
│   │   ├── Direction/                    # 🎯 方向判断模块          — 95% Kiro 开发
│   │   │   ├── DirectionManager.cs       #    方向判断主控制器       — Kiro 编写
│   │   │   └── DirectionSettings.cs      #    设置数据类             — Kiro 编写
│   │   ├── Emotion/                      # 😊 情感认知模块          — 90% Kiro 开发
│   │   │   └── EmotionTestController.cs  #    情感测试控制器         — Kiro 编写
│   │   ├── Json/                         # 📦 通用数据层            — 90% Kiro 开发
│   │   │   ├── JsonNetDataService.cs     #    JSON 序列化服务        — Kiro 编写
│   │   │   └── SettlementScreen.cs       #    通用结算界面           — Kiro 编写
│   │   ├── BirdStart.cs                  #    启动页控制器           — Kiro 编写
│   │   └── InfiniteScrollPanel.cs        #    无限滚动面板           — Kiro 编写
│   │
│   ├── *.unity (×8)                      # 🎬 Unity 场景文件        — 85% Kiro 搭建
│   │   ├── Load.unity                    #    启动加载场景
│   │   ├── 主界面.unity                   #    主界面场景
│   │   ├── DigitSpan.unity               #    数字广度场景
│   │   ├── NBack.unity                   #    N-Back 场景
│   │   ├── RVP.unity                     #    快速视觉处理场景
│   │   ├── Direction.unity               #    方向判断场景
│   │   ├── Emotion.unity                 #    情感认知场景
│   │   └── 3DGenerate.unity              #    虚实绘景场景
│   │
│   ├── doc/                              # 📝 场景制作文档          — 100% Kiro 编写
│   │   ├── Load场景制作文档.md
│   │   ├── 主界面场景制作文档.md
│   │   ├── DigitSpan场景制作文档.md
│   │   ├── NBack场景制作文档.md
│   │   ├── RVP场景制作文档.md
│   │   ├── Direction场景制作文档.md
│   │   ├── Emotion场景制作文档.md
│   │   └── 3DGenerate场景制作文档.md
│   │
│   ├── 操作手册/                          # 📘 用户操作手册          — 100% Kiro 编写
│   │   └── 不孤鸟操作手册.md
│   │
│   ├── Prefabs/                          # 🧩 预制体
│   │   ├── 画板预制体.prefab             #    AR 绘画画板
│   │   ├── 画笔.prefab                   #    画笔工具
│   │   ├── 星星.prefab                   #    情感角色
│   │   ├── RVP图像.prefab               #    手势图片容器
│   │   └── ProgressBarAuto_RoundOutline  #    进度条
│   │
│   ├── ART/                              # 🎨 美术资源
│   ├── Font/                             # 🔤 字体资源
│   ├── Voice/                            # 🔊 语音提示音频
│   └── 0-9audio/                         # 🔢 数字音频 (0-9 + 提示音)
│
├── Assets/AQY/Scripts/画板/               # 🖌️ 画板核心脚本
│   ├── DrawingBoard.cs                   #    画板绘图引擎 (410行)   — Kiro 编写
│   └── DrawingActions.cs                 #    绘图操作定义           — Kiro 编写
│
└── Assets/Plugins/Demigiant/             # 📦 DOTween Pro 动画插件
```

---

## 📊 Kiro 使用比例分析

本项目大量使用 **Kiro AI IDE** 进行开发，以下为各模块的代码贡献统计：

### 代码层

| 功能模块 | 总代码行数 | Kiro 贡献行数 | Kiro 使用比例 |
|----------|-----------|-------------|-------------|
| 3DGenerate 虚实绘景 | ~439 行 | ~417 行 | 95% |
| DigitSpan 数字广度 | ~536 行 | ~482 行 | 90% |
| NBack 回溯记忆 | ~316 行 | ~284 行 | 90% |
| RVP 快速视觉处理 | ~485 行 | ~461 行 | 95% |
| Direction 方向判断 | ~305 行 | ~290 行 | 95% |
| Emotion 情感认知 | ~129 行 | ~116 行 | 90% |
| Json 通用数据层 | ~124 行 | ~112 行 | 90% |
| 画板引擎 (AQY) | ~445 行 | ~423 行 | 95% |
| 通用脚本 | ~106 行 | ~95 行 | 90% |
| **C# 代码合计** | **~2,885 行** | **~2,680 行** | **92.9%** |

### 场景搭建层

| 场景文件 | YAML 行数 | Kiro 搭建比例 |
|----------|----------|-------------|
| 3DGenerate.unity | 5,772 行 | 80% |
| DigitSpan.unity | 4,704 行 | 90% |
| NBack.unity | 3,406 行 | 85% |
| Emotion.unity | 2,968 行 | 85% |
| 主界面.unity | 1,995 行 | 90% |
| RVP.unity | 1,818 行 | 90% |
| Direction.unity | 1,639 行 | 90% |
| Load.unity | 813 行 | 90% |
| **场景合计** | **~23,115 行** | **~87%** |

### 文档层

| 文档类型 | 总行数 | Kiro 编写比例 |
|----------|--------|-------------|
| 8 份场景制作文档 | ~1,650 行 | 100% |
| 用户操作手册 | ~231 行 | 100% |
| Spec 需求/设计文档 | ~200 行 | 100% |
| **文档合计** | **~2,081 行** | **100%** |

### 总计

| 类别 | 总行数 | Kiro 贡献行数 | Kiro 使用比例 |
|------|--------|-------------|-------------|
| C# 业务逻辑 | ~2,885 行 | ~2,680 行 | 92.9% |
| Unity 场景 (YAML) | ~23,115 行 | ~20,110 行 | 87.0% |
| 技术文档 | ~2,081 行 | ~2,081 行 | 100% |
| **项目总计** | **~28,081 行** | **~24,871 行** | **88.6%** |

---

## 🔧 开发环境

| 项目 | 版本/要求 |
|------|----------|
| Unity | 2021.3 LTS+ |
| .NET | 4.x |
| Rokid UXR SDK | 3.0 |
| Tripo AI SDK | Latest |
| DOTween Pro | 1.x |
| Newtonsoft.Json | 13.x |
| 目标设备 | Rokid AR 眼镜 (XR2 Gen1+, 12GB RAM) |

---

## 🚀 快速开始

1. **克隆仓库**
   ```bash
   git clone https://github.com/your-username/NotLonelyBird.git
   ```

2. **Unity 打开项目**
   - 使用 Unity 2021.3+ 打开项目根目录
   - 等待资源导入完成

3. **配置 SDK**
   - 导入 Rokid UXR SDK 3.0
   - 导入 Tripo AI SDK 并配置 API Key
   - 确认 DOTween Pro 已激活

4. **Build Settings**
   - 将以下场景按顺序添加到 Build Settings：
     ```
     0: Load
     1: 主界面
     2: DigitSpan
     3: NBack
     4: RVP
     5: Direction
     6: Emotion
     7: 3DGenerate
     ```

5. **打包部署**
   - Platform 切换为 Android
   - 连接 Rokid 设备，Build and Run

---

## 🗺️ 场景导航

```
Load (启动页)
  └── 主界面
        ├── 记忆力 → 设置页 → DigitSpan (数字广度)
        ├── 记忆力 → 设置页 → NBack (N-Back 回溯)
        ├── 注意力 → 设置页 → RVP (快速视觉处理)
        ├── 执行功能 → 设置页 → Direction (方向判断)
        ├── 情感识别 → Emotion (情感认知)
        ├── 虚实绘景 → 3DGenerate (创意画板)
        └── 退出应用
```

---

## 📝 文档索引

| 文档 | 路径 | 说明 |
|------|------|------|
| 用户操作手册 | `Assets/kiro/操作手册/不孤鸟操作手册.md` | 完整的用户使用指南 |
| Load 场景文档 | `Assets/kiro/doc/Load场景制作文档.md` | 启动页场景结构 |
| 主界面场景文档 | `Assets/kiro/doc/主界面场景制作文档.md` | 主界面场景结构 |
| DigitSpan 场景文档 | `Assets/kiro/doc/DigitSpan场景制作文档.md` | 数字广度场景结构 |
| NBack 场景文档 | `Assets/kiro/doc/NBack场景制作文档.md` | N-Back 场景结构 |
| RVP 场景文档 | `Assets/kiro/doc/RVP场景制作文档.md` | 快速视觉处理场景结构 |
| Direction 场景文档 | `Assets/kiro/doc/Direction场景制作文档.md` | 方向判断场景结构 |
| Emotion 场景文档 | `Assets/kiro/doc/Emotion场景制作文档.md` | 情感认知场景结构 |
| 3DGenerate 场景文档 | `Assets/kiro/doc/3DGenerate场景制作文档.md` | 虚实绘景场景结构 |

---

## 🤖 关于 Kiro AI 辅助开发

本项目全程使用 **Kiro AI IDE** 作为核心开发工具，Kiro 深度参与了从需求分析、架构设计到代码实现、场景搭建、文档编写的完整开发流程。

### Kiro 参与的工作内容

- **需求分析与设计**：通过 Kiro Spec 功能完成需求文档和技术设计
- **C# 脚本开发**：全部 26 个 C# 脚本中，92.9% 的代码由 Kiro 编写，涵盖游戏逻辑、ASR 语音识别、手势交互、UI 管理、数据持久化等
- **Unity 场景搭建**：8 个 Unity 场景的 GameObject 层级、组件配置、UI 布局均通过 Kiro 辅助完成
- **ScriptableObject 资产**：8 个手势数据资产文件由 Kiro 生成
- **技术文档**：全部 8 份场景制作文档 + 用户操作手册 100% 由 Kiro 编写
- **代码审查与调试**：利用 Kiro 的诊断能力进行实时代码检查和问题修复

---

<div align="center">

*Built with ❤️ and 🤖 Kiro AI IDE*

</div>

---

<img width="2560" height="1392" alt="14a8715f2dc6245e13b09466af69498f" src="https://github.com/user-attachments/assets/10f1eba0-a212-44e4-90ff-915442684cc7" />

<img width="657" height="463" alt="image" src="https://github.com/user-attachments/assets/ecfe20e7-c1be-4f40-9a8d-766df5422b13" />

