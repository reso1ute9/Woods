# forst
forst -- 3d survival type game demo

![Image text](Assets/Images/images_menu.png)




视频演示:【Unity个人自学demo】 https://www.bilibili.com/video/BV1Qc411c7xu/?share_source=copy_web&vd_source=cf326a22ef42c23caf731f27146e385a


----

更新日志:
- 2023.9.21:
    - AI系统:
        1. 完善AI追击状态逻辑, 当AI追击到其他地图块时需要迁移数据并更换AI物体的父物体
    - UI系统:
        1. 重构进度条逻辑, 之前是根据地图块加载进度来显示进度条数字，有时会导致刚加载好的地图块上AI会发出声音

- 2023.9.20:
    - AI系统:
        1. 添加AI待机/巡逻状态，在巡逻状态时保存位置坐标, 当存档恢复时根据结束游戏时存档恢复AI物体位置
        2. 添加检测玩家攻击AI的机制
        3. 添加AI脚步音效
        4. 添加AI受伤状态, 包括受伤音效、受伤动画、以及受伤后会切换到追击状态
        5. 添加AI追击状态, 目前仅需追着玩家跑即可

- 2023.9.19:
    - AI系统:
        1. 添加AI物体存档和恢复存档机制
        2. 添加AI物体生成逻辑(仅生成物体未添加行为逻辑)
        

- 2023.9.18:
    - AI系统:
        1. 添加导航烘焙
        2. 添加AI物体配置

- 2023.9.17:
    - 建筑系统:
        1. 添加篝火数据存档功能
        2. 添加往篝火中放入木材增加燃烧时间的功能
        3. 添加篝火烧烤食物的功能, 目前仅支持制作熟浆果种子和熟蘑菇

- 2023.9.16:
    - 建筑系统:
        1. 添加篝火预制体和配置文件
        2. 更新篝火功能, 目前篝火会随着时间变化燃烧, 一定时间后彻底熄灭
        
- 2023.9.15:
    - 建造系统：
        1. 新增建筑物科学机器数据接口及设置建造条件
        2. 设置科学机器影响建筑物UI界面, 当前置条件满足后才会在合成/建造页面中显示对应物体的建造条件和介绍
        3. 添加2级储物箱

- 2023.9.14：
    - 建造系统: 
        1. 新增储物箱背包UI窗口
        2. 实现在地图中点击储物箱后出现窗口的功能
        3. 添加储物箱打开关闭音效
        4. 限制玩家交互据距离, 当玩家距离储物箱过远时关闭储物箱
        5. 禁止物品扔到储物箱上

- 2023.9.13:
    - 背包系统: 重构背包系统代码, 抽象出背包窗口基类

- 2023.9.12:
    - 添加物品: 浆果种子, 浆果丛掉落, 右键使用后恢复饥饿值, 后续添加浆果种子种植功能
    - 建造系统: 添加种植浆果的功能
    - 背包系统: 添加背包管理器, 为以后添加其他背包做准备

- 2023.9.11:
    - 建造系统:
        1. 完善预览建筑物碰撞检测功能
        2. 在建造时屏蔽其他UI,完成建造或取消建造时恢复
        3. 完善浆果存档机制, 在采摘完浆果后及时进行存档
    - fix bug:
        1. 修复物品掉落配置

- 2023.9.10:
    - 建造系统: 
        1. 增加建筑物预览时需要有格子吸附的效果
        2. 当建筑物在合适的地面上进行摆放时是绿色半透明, 如果摆放的格子上存在其他地图物品则变为红色半透明(设置物体材质MeshRender中RenderingMode)

- 2023.9.9:
    - 建造系统: 当资源充足并点击建造按钮后生成建筑物的预览效果

- 2023.9.8：
    - 建造系统: 添加储物箱配置, 同步更新旧配置代码

- 2023.9.7:
    - 合成系统: 1. 合成后的物品经过检验后放入物品栏; 2. 整个二三级合成面板关闭逻辑优化; 3. 合成完毕后减少物品栏中对应的物品并更新页面状态

- 2023.9.6：
    - 合成系统: 实现三级合成面板物品列表展示, 1. 显示/关闭列表

- 2023.9.5:
    - 合成系统: 实现二级合成面板中选项后的内部逻辑, 1. 显示/关闭列表; 2. 切换列表; 3. 检查合成物品是否满足合成条件并且在列表中优先展示满足条件的物品;

- 2023.9.4:
    - 合成系统: 添加物品合成配置, 例如到选中二级菜单中某个选项时三级面板需要根据这个配置来显示合成细节

- 2023.9.3:
    - 合成系统: 创建UI建造窗口一级菜单, 编写菜单逻辑

- 2023.9.2:
    - 添加物体在地图上的腐烂机制, 例如武器丢到地上一段时间后会销毁
    - 完善地图刷新机制, 使用天数触发地图刷新机制

- 2023.9.1：
    - 添加地图刷新机制, 每个早晨都刷新一次每个地图块上的物品: 1. 一个地图网格顶点如果已有物品则无法刷新; 2. 重构创建地图的部分功能 

- 2023.8.31:
    - 完善丢弃物品功能: 1. 增加物品资源补齐缺失配置. 目前新添加武器、蘑菇、浆果扔到地面上; 2. 禁止将物品扔到树木、石头、灌木上
    - 修复小地图显示bug
    - 修复存档恢复时地面上物体直接下坠的bug

- 2023.8.30:
    - 完善丢弃物品功能, 由于其他物品没有现成的预制体, 因此目前仅支持将小石头、木材、树枝扔到地面上
    - 修复堆叠物品丢弃时图标显示出错的问题

- 2023.8.29:
    - 新增树枝掉落功能, 可以通过砍树或者是砍灌木获得树枝
    - 新增石头掉落功能, 可以通过采石或者在地上拾取获得小石头

- 2023.8.28:
    - 完善物品掉落功能, 目前支持砍树后掉落木材

- 2023.8.27:
    - 开发简单的物品掉落功能: 1. 当物体死亡时物品可掉落; 2. 查找掉落物品id; 3. 计算掉落概率(如果单个物体掉落多个物品概率单独计算)并根据计算结果决定是否将掉落物实例化

- 2023.8.25:
    - 物品掉落配置: 添加物品掉落配置(未实现该功能), 目前是通过id去查找掉落物品

- 2023.8.24:
    - 添加摘取浆果功能, 并且浆果采摘后可以砍掉

- 2023.8.23:
    - 添加收集碎石功能

- 2023.8.21:
    - 添加手摘功能
    - 添加采摘蘑菇功能

- 2023.8.20:
    - 修复地图数据保存bug, 即地图重新载入档案后物体未按照预期消失, 例如砍伐完灌木后重新载入存档灌木仍然存在

- 2023.8.19:
    - 设置镰刀攻击动作、音效以及攻击检测逻辑
    - 设置灌木受击动作、受击逻辑

- 2023.8.18:
    - 优化攻击逻辑和砸石头: 1. ; 2. 铁镐攻击及石头销毁

- 2023.8.16:
    - 添加石头物体配置: 添加石头碰撞检测、受击音效、受击动画
    
- 2023.8.14:
    - 重构地图对象存档数据结构: 梳理存档结构和逻辑, 替换掉其中使用的字典改为自定义可序列化字典, 支持在地图上移除地图物品的功能(例如砍树后树木需要在地图上消失)
    - UI小地图物体销毁同步更新

- 2023.8.13:
    - 武器耐久度降低及损坏: 根据配置文件中每次攻击降低耐久度数值去更新当前存档中武器耐久度数据和更新UI数值
    - 添加树木受击动画: 添加树木受伤动作; 扣除树木生命值; 树木死亡时销毁模型
    - 字典持久化: 字典作为内置类型没有方便的持久化方法, 为了方便修改后续存档模块, 因此需要实现字典持久化功能

- 2023.8.12:
    - 添加InputManager: 负责检测单位是否被选中
    - 添加主角挥击武器逻辑: 具体逻辑根据玩家手中武器和鼠标点击物体类型决定
    - 设置主角挥击武器动作: 设置一个动作帧中前后摇及具体攻击帧. 设置攻击方向, 当玩家需要转向时需要平滑过渡
    - 武器伤害检测: 创建触发器并且进行监听, 简单测试功能是否能正常运行

- 2023.8.11:
    - 配置武器: 完善物品类型设置, 添加武器类型、耐久、攻击力、图标、prefab、animator等配置
    - 武器槽改变时更新角色数值、模型、动画等相关设置

- 2023.8.10:
    - 优化物品栏功能: 1. 右键点击武器可以进行装备, 如果当前武器栏有装备则进行交换武器; 2. 右键点击消耗品会加血或者增加饱食度; 3. 材料目前无法右键点击使用

- 2023.8.7:
    - 增加物品栏添加物品音效
    - 修复物品栏代码bug

- 2023.8.6:
    - 优化物品栏代码逻辑

- 2023.8.5:
    - 快捷栏物品添加、丢弃、使用逻辑优化

- 2023.8.2:
    - 物品添加逻辑优化

- 2023.8.1:
    - 创建角色配置数据(生命值、饱食度、生命值下降速度、饱食度下降速度等)和存档逻辑
    - 将角色配置数据与游戏场景中UI相关联, UI会随玩家饥饿值、生命值下降而变化
    - 添加通用音效配置, 为后续开发添加使用、捡起、丢弃物品音效功能做准备

- 2023.7.31:
    - fix bug: 1. 修复在加载页面时可以按"M"出现小地图的bug; 2. 修复部分地图块在加载存档时直接显示的bug
    - 重构游戏时间数据+配置逻辑, 便于存档
    - 重构时间管理器: 1. 白天或者野外更替时对外发出事件; 2. 天数变化时对外发出事件

- 2023.7.30:
    - 添加物品拖拽代码, 1. 支持物品在物品从快捷栏中任意一处拖拽到另一处; 2. 支持将物品拖拽到地面上; 3. 支持物品拖拽过程中的类型检测, 比如消耗品不能拖拽到武器栏上

- 2023.7.29:
    - 完善物品快捷栏代码, 经过简单测试后现有物品可以正确显示在物品快捷栏上 
    
- 2023.7.28: 
    - 完善物品配置代码, 创建物品数据类
    - 搭建物品快捷栏UI, 并设置物品快捷栏初始化及存档逻辑框架

- 2023.7.27: 
    - 添加游戏进入加载页面
    - 添加物品配置代码, 支持在Unity编辑器Project界面上添加对应配置文件, 例如配置武器、材料、消耗品等信息

- 之前: 未记录开发过程, 后续可能补充


--- 

Bug:
- UI: 游戏加载页面无法覆盖全整个游戏画面导致四周出现缝隙可以看到地图加载内容
- UI: 堆叠物品丢弃时图标显示出错

---

TODO:
- 背包系统: 左键拖拽物体直接放入储物箱中
- 采摘优化: 采摘时隐藏武器, 采摘完显示武器
- 地图扩展: 1. 扩展当前地上场景; 2. 加入地下洞穴场景
- 联机模式: 加入联机模式
- 地图生成: 使用四叉树动态生成地图
- 拖拽: 物品无法拖拽到某些物体上, 例如树木石头
- 项目中仍然存在一些默认值没有配置化, 后续可以设置单独设置配置文件, 替换掉项目中的明文数值

---

Fixed:
- 2023.9.11 ~~物品掉落配置可能出现了错误, 击打石头后掉落的是草莓~~
    - 原因: 物品掉落配置中配置Id错误
    - 修复: 已修改配置内容并通过测试

- 2023.8.31 ~~存档恢复时地面上物体直接下坠的bug~~
    - 原因: 恢复存档期间地面碰撞体并未完全生成, 但是物品已经生成了或者是物品部分模型在一个未生成的地面时会导致坠落
    - 修复: 之前每个地图块中有一个碰撞体, 现在将整个地面设置成一整个碰撞体, 在初始化时先根据地图大小初始化出来该地面碰撞体解决这个问题

- 2023.8.31 ~~小地图中物品id初始化错误~~
    - 原因: 小地图物品数据初始化时id出现错误未能正确赋值, 因此导致按下M键后小地图UI无法初始化
    - 修复: 修复物品数据初始化时id问题

- 2023.8.30 ~~堆叠物品丢弃时图标显示出错~~
    - 原因: 结束拖拽时如果将物品扔到了地上在代码中未将物品对象icon位置复原
    - 修复: 当拖拽结束时如果物品扔到了地上首先恢复icon位置

- 2023.8.25 ~~采摘: 现象是当手拿镰刀采摘时会攻击浆果灌木丛~~ 
    - 原因: 当点击一次鼠标的时间内游戏会执行很多帧, 所以在执行采摘动作时可采摘标记已经被标记成false了, 此时玩家处于可攻击状态, 因此导致了在采摘时会攻击浆果灌木丛
    - 修复: 添加了一个标记变量记录当前mapobject

- 2023.8.25 ~~装备的采集武器攻击时耐久度出错, 单次攻击耐久度会减少很多~~: 
    - 原因: 攻击事件是在UIMapWindow中去注册的, 再检查地图物品时会调用该UI窗口, 导致事件注册出问题(待补充)
    - 修复: 暂时使用单例去修复物品快捷栏事件注册导致的该问题