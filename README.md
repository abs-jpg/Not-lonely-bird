---
title: 🐦 不孤鸟 NotLonelyBird
description: An AR-based Cognitive Intervention Training System for Autistic Children
repository: https://github.com/abs-jpg/Not-lonely-bird.git
tags: [Unity, C#, AR]
---

## 🐦 不孤鸟 NotLonelyBird

**基于 AR 技术的自闭症儿童认知干预训练系统**

*An AR-based Cognitive Intervention Training System for Children*

![Unity](https://img.shields.io/badge/Unity-2021.3+-black?logo=unity)
![C#](https://img.shields.io/badge/C%23-10.0-239120?logo=csharp)
![Platform](https://img.shields.io/badge/Platform-Rokid%20XR-blue)
![License](https://img.shields.io/badge/License-MIT-green)
![AI Assisted](https://img.shields.io/badge/AI%20Assisted-Kiro-purple)

</div>

## 📖 项目简介 / Project Introduction

**[中文]**
"不孤鸟"是一款运行于 **Rokid XR 眼镜**平台的 6DoF AR 应用，面向自闭症儿童认知干预训练。系统基于剑桥 **CANTAB 神经心理学范式**，结合 Rokid 手势识别、千问 ASR 语音识别和 Tripo AI 3D 生成技术，提供沉浸式的全感官统合训练体验。

项目涵盖 **记忆力、注意力、执行功能、情感认知、创意疗愈** 五大训练维度，共 **8 个 Unity 场景**、**26+ 个 C# 脚本**、**8 份场景制作文档**和完整的用户操作手册。

**[English]**
"NotLonelyBird" is a 6DoF AR application running on the **Rokid XR glasses** platform, specifically designed for cognitive intervention training in children with autism. Based on the Cambridge **CANTAB neuropsychological paradigm**, the system integrates Rokid gesture recognition, Qwen ASR (Automatic Speech Recognition), and Tripo AI 3D generation technology to provide an immersive, multi-sensory integration training experience.

The project covers five major training dimensions: **Memory, Attention, Executive Function, Emotional Cognition, and Creative Healing**. It features a total of **8 Unity scenes**, **26+ C# scripts**, **8 scene production documents**, and a comprehensive user operating manual.

---

## ✨ 功能模块 / Functional Modules

| 模块 / Module | 场景 / Scene | 训练目标 / Training Objective | 核心技术 / Core Technology |
|------|------|----------|----------|
| 🧠 **数字广度**<br>*Digit Span* | DigitSpan | **瞬时记忆、顺序/逆序复述**<br>*Instant memory, forward/backward repetition* | ASR 语音识别 + 按钮双模式答题<br>*ASR voice recognition + button dual-mode answering* |
| 🔄 **N-Back 回溯**<br>*N-Back Memory* | NBack | **空间工作记忆、信息更新**<br>*Spatial working memory, information updating* | 3×3 九宫格空间匹配<br>*3×3 grid spatial matching* |
| 👁️ **快速视觉处理**<br>*RVP* | RVP | **持续注意力、目标检测**<br>*Sustained attention, target detection* | Rokid 手势识别 + 高频刺激<br>*Rokid gesture recognition + high-frequency stimulation* |
| 🎯 **方向判断**<br>*Direction Judgment* | Direction | **抑制控制、资源分配**<br>*Inhibitory control, resource allocation* | 手掌朝前手势 + 一致/冲突范式<br>*Palm-forward gesture + congruent/incongruent paradigm* |
| 😊 **情感认知**<br>*Emotional Cognition* | Emotion | **情绪识别、社交认知**<br>*Emotion recognition, social cognition* | 3D 卡通角色微表情动画<br>*3D cartoon character micro-expression animation* |
| 🎨 **虚实绘景**<br>*3D Generation Drawing* | 3DGenerate | **创意表达、空间认知**<br>*Creative expression, spatial cognition* | Tripo AI 3D 生成 + AR 画板<br>*Tripo AI 3D generation + AR drawing board* |

---

## 🏗️ 技术架构 / Technical Frame

```
运行环境: YodaOS-Master / UXR 3.0 / XR2 Gen1+
交互方式: 6DoF 射线 + 手势识别（捏合/握拳/手掌朝前）
语音识别: 千问 ASR 模型 API (16kHz WAV)
3D 生成:  Tripo AI SDK (TextToModel / ImageToModel)
动画系统: DOTween Pro + Unity Animator
数据持久: JSON 本地序列化 (Newtonsoft.Json)
```
```
Operating Environment: YodaOS-Master / UXR 3.0 / XR2 Gen1+
Interaction: 6DoF Raycast + Gesture Recognition (Pinch / Fist / Palm Forward)
Speech Recognition: Qwen ASR Model API (16kHz WAV)
3D Generation: Tripo AI SDK (TextToModel / ImageToModel)
Animation System: DOTween Pro + Unity Animator
Data Persistence: Local JSON Serialization (Newtonsoft.Json)
```
---

## 📁 项目结构 / Project Structure

```
NotLonelyBird/
├── Assets/kiro/
│   ├── Scripts/                          # C# Business Logic Layer
│   │   ├── 3DGenerate/                   # 🎨 3D Generation Drawing Module   — 95% Kiro developed
│   │   │   ├── GlobalUIManager.cs        #    UI Manager Singleton           — Written by Kiro
│   │   │   ├── ExternalInputManager.cs   #    ASR Voice Recognition Manager  — Written by Kiro
│   │   │   ├── DrawingScreenshotter.cs   #    Drawing Board Screenshot to 3D — Written by Kiro
│   │   │   └── BordController.cs         #    Drawing Board Controller       — Written by Kiro
│   │   ├── DigitSpan/                    # 🧠 Digit Span Module              — 90% Kiro developed
│   │   │   ├── MemoryGameManager.cs      #    Main Game Controller           — Written by Kiro
│   │   │   ├── ASRManager.cs             #    Voice Recognition Manager      — Written by Kiro
│   │   │   ├── MemorySettingsMenu.cs     #    Settings Menu                  — Written by Kiro
│   │   │   └── SavWav.cs                 #    WAV Audio Recording Saving     — Written by Kiro
│   │   ├── NBack/                        # 🔄 N-Back Module                  — 90% Kiro developed
│   │   │   ├── NBackManager.cs           #    N-Back Main Controller         — Written by Kiro
│   │   │   ├── NBackSetting.cs           #    Settings Data Class            — Written by Kiro
│   │   │   ├── AllSettingCtr.cs          #    Global Settings Singleton      — Written by Kiro
│   │   │   ├── ScenesChange.cs           #    Scene Transition               — Written by Kiro
│   │   │   └── FollowHead.cs             #    Head Tracking                  — Written by Kiro
│   │   ├── RVP/                          # 👁️ Rapid Visual Processing Module — 95% Kiro developed
│   │   │   ├── GestureManager.cs         #    Gesture Manager                — Written by Kiro
│   │   │   ├── GestureInputController.cs #    Gesture Input Controller       — Written by Kiro
│   │   │   ├── RVPSettlementScreen.cs    #    Settlement Screen              — Written by Kiro
│   │   │   ├── GestureData.cs            #    Gesture Data Definition        — Written by Kiro
│   │   │   └── GestureAssets/            #    8 Gesture ScriptableObjects    — Generated by Kiro
│   │   ├── Direction/                    # 🎯 Direction Judgment Module      — 95% Kiro developed
│   │   │   ├── DirectionManager.cs       #    Direction Main Controller      — Written by Kiro
│   │   │   └── DirectionSettings.cs      #    Settings Data Class            — Written by Kiro
│   │   ├── Emotion/                      # 😊 Emotional Cognition Module     — 90% Kiro developed
│   │   │   └── EmotionTestController.cs  #    Emotion Test Controller        — Written by Kiro
│   │   ├── Json/                         # 📦 Universal Data Layer           — 90% Kiro developed
│   │   │   ├── JsonNetDataService.cs     #    JSON Serialization Service     — Written by Kiro
│   │   │   └── SettlementScreen.cs       #    Universal Settlement Screen    — Written by Kiro
│   │   ├── BirdStart.cs                  #    Splash Screen Controller       — Written by Kiro
│   │   └── InfiniteScrollPanel.cs        #    Infinite Scroll Panel          — Written by Kiro
│   │
│   ├── *.unity (×8)                      # 🎬 Unity Scene Files              — 85% Built by Kiro
│   │   ├── Load.unity                    #    Splash Load Scene
│   │   ├── 主界面.unity                    #    Main Menu Scene
│   │   ├── DigitSpan.unity               #    Digit Span Scene
│   │   ├── NBack.unity                   #    N-Back Scene
│   │   ├── RVP.unity                     #    Rapid Visual Processing Scene
│   │   ├── Direction.unity               #    Direction Judgment Scene
│   │   ├── Emotion.unity                 #    Emotional Cognition Scene
│   │   └── 3DGenerate.unity              #    3D Generation Drawing Scene
│   │
│   ├── doc/                              # 📝 Scene Production Docs          — 100% Written by Kiro
│   │   ├── Load场景制作文档.md
│   │   ├── 主界面场景制作文档.md
│   │   ├── DigitSpan场景制作文档.md
│   │   ├── NBack场景制作文档.md
│   │   ├── RVP场景制作文档.md
│   │   ├── Direction场景制作文档.md
│   │   ├── Emotion场景制作文档.md
│   │   └── 3DGenerate场景制作文档.md
│   │
│   ├── 操作手册/                          # 📘 User Operating Manual          — 100% Written by Kiro
│   │   └── 不孤鸟操作手册.md
│   │
│   ├── Prefabs/                          # 🧩 Prefabs
│   │   ├── 画板预制体.prefab               #    AR Drawing Board
│   │   ├── 画笔.prefab                    #    Brush Tool
│   │   ├── 星星.prefab                    #    Emotion Character
│   │   ├── RVP图像.prefab                 #    Gesture Image Container
│   │   └── ProgressBarAuto_RoundOutline  #    Progress Bar
│   │
│   ├── ART/                              # 🎨 Art Resources
│   ├── Font/                             # 🔤 Font Resources
│   ├── Voice/                            # 🔊 Voice Prompt Audio
│   └── 0-9audio/                         # 🔢 Number Audio (0-9 + Prompts)
│
├── Assets/AQY/Scripts/画板/                # 🖌️ Drawing Board Core Scripts
│   ├── DrawingBoard.cs                   #    Drawing Engine (410 lines)     — Written by Kiro
│   └── DrawingActions.cs                 #    Drawing Actions Definition     — Written by Kiro
│
└── Assets/Plugins/Demigiant/             # 📦 DOTween Pro Animation Plugin
```

---

## 📊 Kiro 使用比例分析 / Kiro Usage Proportion Analysis

**[中文]** 本项目大量使用 **Kiro AI IDE** 进行开发，以下为各模块的代码贡献统计：
**[English]** This project heavily utilized the **Kiro AI IDE** for development. Below are the code contribution statistics for each module:

### 代码层 / Code Layer

| 功能模块 / Functional Module | 总代码行数 / Total Lines | Kiro 贡献行数 / Kiro Lines | Kiro 使用比例 / Kiro Proportion |
|----------|-----------|-------------|-------------|
| 3DGenerate 虚实绘景 (*3D Generation*) | ~439 行 | ~417 行 | 95% |
| DigitSpan 数字广度 (*Digit Span*) | ~536 行 | ~482 行 | 90% |
| NBack 回溯记忆 (*N-Back Memory*) | ~316 行 | ~284 行 | 90% |
| RVP 快速视觉处理 (*Rapid Visual Processing*) | ~485 行 | ~461 行 | 95% |
| Direction 方向判断 (*Direction Judgment*) | ~305 行 | ~290 行 | 95% |
| Emotion 情感认知 (*Emotional Cognition*) | ~129 行 | ~116 行 | 90% |
| Json 通用数据层 (*Universal Data Layer*) | ~124 行 | ~112 行 | 90% |
| 画板引擎 AQY (*Drawing Engine AQY*) | ~445 行 | ~423 行 | 95% |
| 通用脚本 (*Universal Scripts*) | ~106 行 | ~95 行 | 90% |
| **C# 代码合计 / Total C# Code** | **~2,885 行** | **~2,680 行** | **92.9%** |

### 场景搭建层 / Scene Building Layer

| 场景文件 / Scene File | YAML 行数 / YAML Lines | Kiro 搭建比例 / Kiro Build Proportion |
|----------|----------|-------------|
| 3DGenerate.unity | 5,772 行 | 80% |
| DigitSpan.unity | 4,704 行 | 90% |
| NBack.unity | 3,406 行 | 85% |
| Emotion.unity | 2,968 行 | 85% |
| 主界面.unity (*Main Menu*) | 1,995 行 | 90% |
| RVP.unity | 1,818 行 | 90% |
| Direction.unity | 1,639 行 | 90% |
| Load.unity | 813 行 | 90% |
| **场景合计 / Total Scenes** | **~23,115 行** | **~87%** |

### 文档层 / Document Layer

| 文档类型 / Document Type | 总行数 / Total Lines | Kiro 编写比例 / Kiro Writing Proportion |
|----------|--------|-------------|
| 8 份场景制作文档 (*8 Scene Docs*) | ~1,650 行 | 100% |
| 用户操作手册 (*User Manual*) | ~231 行 | 100% |
| Spec 需求/设计文档 (*Spec Docs*) | ~200 行 | 100% |
| **文档合计 / Total Docs** | **~2,081 行** | **100%** |

### 总计 / Total Summary

| 类别 / Category | 总行数 / Total Lines | Kiro 贡献行数 / Kiro Lines | Kiro 使用比例 / Kiro Proportion |
|------|--------|-------------|-------------|
| C# 业务逻辑 (*C# Business Logic*) | ~2,885 行 | ~2,680 行 | 92.9% |
| Unity 场景 YAML (*Unity Scenes*) | ~23,115 行 | ~20,110 行 | 87.0% |
| 技术文档 (*Technical Documents*) | ~2,081 行 | ~2,081 行 | 100% |
| **项目总计 / Project Total** | **~28,081 行** | **~24,871 行** | **88.6%** |

---

## 🔧 开发环境 / Development Environment

| 项目 / Item | 版本/要求 / Version/Requirement |
|------|----------|
| **Unity** | 2021.3 LTS+ |
| **.NET** | 4.x |
| **Rokid UXR SDK** | 3.0 |
| **Tripo AI SDK** | Latest |
| **DOTween Pro** | 1.x |
| **Newtonsoft.Json** | 13.x |
| **目标设备 / Target Device** | Rokid AR 眼镜 / Glasses (XR2 Gen1+, 12GB RAM) |

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

5. **5. 打包部署 / Build and Deploy**
- **[中文]** Platform 切换为 Android，连接 Rokid 设备，Build and Run。
- **[English]** Switch Platform to Android, connect the Rokid device, and select Build and Run.

---

## 🗺️ 场景导航 / Scene Navigation

```text
Load (启动页 / Splash Screen)
  └── 主界面 (Main Menu)
        ├── 记忆力 (Memory) → 设置页 (Settings) → DigitSpan (数字广度 / Digit Span)
        ├── 记忆力 (Memory) → 设置页 (Settings) → NBack (N-Back 回溯 / N-Back Memory)
        ├── 注意力 (Attention) → 设置页 (Settings) → RVP (快速视觉处理 / Rapid Visual Processing)
        ├── 执行功能 (Executive) → 设置页 (Settings) → Direction (方向判断 / Direction Judgment)
        ├── 情感识别 (Emotion) → Emotion (情感认知 / Emotional Cognition)
        ├── 虚实绘景 (Drawing) → 3DGenerate (创意画板 / 3D AR Board)
        └── 退出应用 (Exit App)
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

