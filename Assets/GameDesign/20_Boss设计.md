---
name: boss-design
description: "让 AI 分析 boss 作为技能考试、情绪高峰、机制变体和叙事节点的综合设计。"
user-invocable: true
allowed-tools: Read, Glob, Grep, Write
---

# Boss 设计

## 核心锚点资料
- **Boss Up: Boss Battle Design Fundamentals and Retrospective** — https://www.gdcvault.com/play/1024921/Boss-Up-Boss-Battle-Design
- **Boss Up PDF** — https://media.gdcvault.com/gdc2018/presentations/Keren_Itay_BossUp.pdf
- **Crafting AI for Epic Boss Battles in Warframe** — https://www.gdcvault.com/play/1023408/Crafting-AI-for-Epic-Boss

## Boss设计本质
好 boss 不是更大的血条，而是对玩家已学技能的重新组合、扭转和放大。

## 核心概念
- Skill exam
- Move readability
- Phase structure
- Spectacle vs clarity
- True reason for difficulty

## 分析流程
1. 识别 boss 在测试玩家什么
2. 拆每个阶段新增了什么压力和变化
3. 分析招式可读性与失败归因
4. 分析 boss 是否有独特身份
5. 总结这是记忆型、适应型、耐久型还是混合 boss

## 输出模板
1. 核心目标/体验
2. 核心循环或核心对抗
3. 关键系统与资源
4. 主要决策点
5. 反馈与可读性
6. 学习曲线/难度结构
7. 设计支柱
8. 主要问题与建议

## 阶段变化分析（框架增强）
| # | 阶段 | 血量% | 新增压力 | 视觉变化 | 机制变化 |
|---|-----|------|---------|---------|---------|
| 1 | P1 | 100-60 | [压力] | [变化] | [变化] |
| 2 | P2 | 60-30 | [压力] | [变化] | [变化] |
| 3 | P3 | 30-0 | [压力] | [变化] | [变化] |

## 检查清单
- boss 是否在考此前学到的东西？
- 失败原因是否可归因？
- 视觉规模是否压倒信息可读性？

## 常见误区
- 高血量替代设计
- 阶段变化只是换皮加数值
- 失败原因不清楚

## 一句话记忆
好 boss 是被学习、被克服、且能被记住的考试。

## 框架输出位置
`design/gdd/boss-[boss-name].md`
