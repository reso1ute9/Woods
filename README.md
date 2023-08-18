# forst
forst -- 3d survival type game demo

![Image text](Assets/Images/images_menu.png)




视频演示:【Unity个人自学demo】 https://www.bilibili.com/video/BV1Qc411c7xu/?share_source=copy_web&vd_source=cf326a22ef42c23caf731f27146e385a


----

更新日志:
- 2023/8/18:
    - 优化攻击逻辑和砸石头
    - 添加割灌木功能

- 2023/8/16:
    - 添加石头物体配置
    
- 2023/8/14:
    - 重构地图对象存档数据结构: 梳理存档结构和逻辑, 替换掉其中使用的字典改为自定义可序列化字典, 支持在地图上移除地图物品的功能(例如砍树后树木需要在地图上消失)
    - UI小地图物体销毁同步更新

- 2023/8/13:
    - 武器耐久度降低及损坏: 根据配置文件中每次攻击降低耐久度数值去更新当前存档中武器耐久度数据和更新UI数值
    - 添加树木受击动画: 添加树木受伤动作; 扣除树木生命值; 树木死亡时销毁模型
    - 字典持久化: 字典作为内置类型没有方便的持久化方法, 为了方便修改后续存档模块, 因此需要实现字典持久化功能

- 2023/8/12:
    - 添加InputManager: 负责检测单位是否被选中
    - 添加主角挥击武器逻辑: 具体逻辑根据玩家手中武器和鼠标点击物体类型决定
    - 设置主角挥击武器动作: 设置一个动作帧中前后摇及具体攻击帧. 设置攻击方向, 当玩家需要转向时需要平滑过渡
    - 武器伤害检测: 创建触发器并且进行监听, 简单测试功能是否能正常运行

- 2023/8/11:
    - 配置武器: 完善物品类型设置, 添加武器类型、耐久、攻击力、图标、prefab、animator等配置
    - 武器槽改变时更新角色数值、模型、动画等相关设置

- 2023/8/10:
    - 优化物品栏功能: 1. 右键点击武器可以进行装备, 如果当前武器栏有装备则进行交换武器; 2. 右键点击消耗品会加血或者增加饱食度; 3. 材料目前无法右键点击使用

- 2023/8/7:
    - 增加物品栏添加物品音效
    - 修复物品栏代码bug

- 2023/8/6:
    - 优化物品栏代码逻辑

- 2023/8/5:
    - 快捷栏物品添加、丢弃、使用逻辑优化

- 2023/8/2:
    - 物品添加逻辑优化

- 2023/8/1:
    - 创建角色配置数据(生命值、饱食度、生命值下降速度、饱食度下降速度等)和存档逻辑
    - 将角色配置数据与游戏场景中UI相关联, UI会随玩家饥饿值、生命值下降而变化
    - 添加通用音效配置, 为后续开发添加使用、捡起、丢弃物品音效功能做准备

- 2023/7/31:
    - fix bug: 1. 修复在加载页面时可以按"M"出现小地图的bug; 2. 修复部分地图块在加载存档时直接显示的bug
    - 重构游戏时间数据+配置逻辑, 便于存档
    - 重构时间管理器: 1. 白天或者野外更替时对外发出事件; 2. 天数变化时对外发出事件

- 2023/7/30:
    - 添加物品拖拽代码, 1. 支持物品在物品从快捷栏中任意一处拖拽到另一处; 2. 支持将物品拖拽到地面上; 3. 支持物品拖拽过程中的类型检测, 比如消耗品不能拖拽到武器栏上

- 2023/7/29:
    - 完善物品快捷栏代码, 经过简单测试后现有物品可以正确显示在物品快捷栏上 
    
- 2023/7/28: 
    - 完善物品配置代码, 创建物品数据类
    - 搭建物品快捷栏UI, 并设置物品快捷栏初始化及存档逻辑框架

- 2023/7/27: 
    - 添加游戏进入加载页面
    - 添加物品配置代码, 支持在Unity编辑器Project界面上添加对应配置文件, 例如配置武器、材料、消耗品等信息

- 之前: 未记录开发过程, 后续可能补充


--- 

Bug:
- 将装备栏中的武器拖拽到物品栏武器格子上时应该进行交换

TODO:
- 地图扩展: 1. 扩展当前地上场景; 2. 加入地下洞穴场景
- 联机模式: 加入联机模式
- 地图生成: 使用四叉树动态生成地图
- 拖拽: 物品无法拖拽到某些物体上, 例如树木石头
- 项目中仍然存在一些默认值没有配置化, 后续可以设置单独设置配置文件, 替换掉项目中的明文数值