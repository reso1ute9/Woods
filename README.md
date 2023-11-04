# forst
forst -- 3d survival type game demo

![Image text](Assets/Images/images_menu.png)




视频演示:【Unity个人自学demo】 https://www.bilibili.com/video/BV1Qc411c7xu/?share_source=copy_web&vd_source=cf326a22ef42c23caf731f27146e385a

----
### 游戏介绍
这是一款生存冒险类游戏，游戏玩法参考自《饥荒》，女主角因不明原因被魔法传送到了一块神秘森林中，在迷雾缭绕的森林里她必须躲过危险的蜘蛛和野猪，不断搜集资源，利用资源建造建筑、制作武器和食物，以此来帮助自己渡过一轮又一轮难关，直到找到森林的出口

### 游戏Demo下载地址
[待补充]

### 游戏设计
游戏中主要包含以下几类功能，其中配置模块、UI模块等非重点模块此处不一一列举，详细文档请见 **[forest/Document](./Document)**
- **地图系统：** 
  - **地图设计：** 整个地图分为三个层级，从整体到局部分别是整张地图、地图块、地图单元格。其中地图单元格作为标准单元形状为矩形，包含了若干Unity小网格，在地图单元格上分布着不同的地图对象和不同地面类型
  - **地图对象：** 游戏中设置了类型丰富的地图对象，例如树木、石头、蘑菇等，并且还抽象出地图对象基类降低了模块间的耦合提升了开发效率
  - **地图生成：** 根据玩家设置的地图生成种子随机生成地图，在生成地图时使用柏林噪声确定地图块类型，然后使用War3地面贴图算法实时绘制森林和沼泽，对于每个地图单元格可以根据配置文件随机生成地图对象和AI对象，最后整体设置地图碰撞体和烘焙导航网络
  - **地图刷新：** 利用九宫格地图加载方式加载玩家可视范围内的地图信息并刷新地图物体，在保证显示效果的同时降低了资源消耗
  - **小地图：** 将小地图UI与玩家探索地图内容绑定，能实时显示当前玩家探索区域和对应的地图资源
- **背包系统：** 
  - **物品分类/存储：** 分析游戏玩法后将游戏中玩家使用的物品分为消耗品、材料、武器三大类，其中武器设置耐久度特性，使得玩家在游戏过程中需要不断制作武器，提升游戏紧张感。消耗品、材料设置了可堆叠属性，保证玩家在捡到重复物品时能更合理利用背包空间，避免冗余操作；将背包系统设置为背包管理器和格子管理器两层结构，其中格子管理器负责当前格子数据存储、UI显示、格子使用逻辑，而背包管理器则在更高一层维护背包整体数据处理逻辑和控制背包UI显示
  - **物品使用：** 目前支持右键点击消耗品和武器，点击消耗品时会恢复玩家血量/饱食度，同时触发游戏内玩家UI更新。点击武器后会自动将武器装备到玩家身上，并将武器模型挂在到玩家模型身上
  - **物品拖拽：** 支持将物品拖拽到其他格子、地面、其他背包中，当物品拖拽到其他格子时会完成两个格子中数据交换和更新UI，当物品拖拽到地面时会通知地图系统生成对应地图对象并删除当前格子内容，当物品拖拽到其他背包中时会在不同的背包管理器间完成数据交换并更新UI
  - **储物箱建筑：** 在游戏中设置了储物箱作为玩家背包的扩展，方便玩家在收集到足够的资源时创建更大的储物箱储存物品
- **合成/建造系统：** 
  - **合成物品：** 开发合成物体配置，设置合成物体约束条件，如当前所需资源和前置科技。当玩家合成物品时会与背包系统联动，消耗掉背包中的资源并将新合成的物品放到背包第一个空格中。
  - **建造物体：** 在建造物体时设置了预览模式，进入预览模式时会根据建造地点是否有障碍物动态变换建造物体颜色，只有当周围触碰不到其他物体时才可以建造。当确定建造后会与背包系统和地图系统联动，消耗背包中的资源在当前地图点击位置生成一个地图对象
- **角色系统：** 
  - **角色动画/音效/特效：** 游戏中导入了多种角色动画和音效，根据不同武器类型设置了专属的攻击动画、攻击特效和攻击事件，音效部分包含了攻击音效、走路音效
  - **角色状态：** 设置了5中角色状态，分别是待机、移动、攻击、受击、死亡，通过抽象出状态基类设置各类状态进入、更新、退出方法，并使用有限状态机控制角色间状态切换。其中在玩家执行攻击动作时会根据手上拿的武器类型和攻击对象类型显示不同效果和执行不同的计算逻辑
- **AI系统：** 
  - **AI物体：** 目前游戏中仅包含两类AI物体，分别是野猪和蜘蛛。还为AI物体配置了物品掉落机制，当玩家击倒AI物体后会根据配置掉落对应的奖励
  - **AI动画/状态：** 设置了多种AI动画和状态，在AI动画中添加了相对应的动画事件，用于触发相关逻辑，例如攻击动画事件有开始攻击、攻击结束、停止动作等。AI状态类型也较为丰富，包含追击、巡逻、攻击等。其中野猪没有仇恨范围，蜘蛛具有仇恨范围，当进入蜘蛛林地时蜘蛛会追击玩家，当玩家进入蜘蛛攻击范围蜘蛛则会进行攻击，如果玩家快速远离蜘蛛则不再进行追击
  - **AI导航：** 使用NavMesh为AI物体提供自动寻路功能，通过NavMesh内置的多种方法实现让AI物体在不同地图块间进行巡逻，目前游戏中只设置了静态物体障碍
- **时间系统：**
  - **昼夜系统：** 设置了游戏时间阶段，目前分为5个阶段，即早晨、中午、下午、晚上、深夜，不同时间阶段设置了不同的持续时间、光照强度、光照颜色、光照角度以及是否出现迷雾
  - **其他效果：** 时间系统还会影响其他系统，例如地面上的物体间隔一段时间后就会销毁，浆果经过一段时间后可以成熟
- **存档系统：** 
  - **序列化/反序列化：** 针对在存档过程中遇到的无法序列化/反序列化的数据结构，设计特定方法将其转化成多个可以支持序列化/反序列化的C#内置对象类型
  - **存档机制：** 在不同的功能模块中添加存档事件，当游戏结束时统一调用所有存档事件，完成对地图数据、玩家数据、背包数据、科技树数据等数据的储存。在游戏开始时则通过存档系统加载本地数据文件，恢复游戏中的地图、角色等内容
- **科技树：** 设置科技树系统，在建造建筑物和合成物品前先查看是否解锁对应科技
- **特殊地图对象：** 完成了科学机器、篝火、浆果灌木丛等特殊建筑，其中篝火支持木材续火和烤制食物，浆果灌木丛作为农作物可以种植，并且间隔一段时间果实成熟后可以采摘

--- 

### Bug记录
- 已清空

---

### 后续更新计划
- 天气系统: 添加天气系统, 支持下雨下雪打雷等天气
- 角色: 当角色进入采摘状态后直接移动会出现动画效果问题
- 音效: 在游戏彻底载入后再播放背景音效
- 音效: 篝火音效音量即使将volume设置为1仍然很小
- AI系统: 优化野猪攻击动画效果
- 背包系统: 左键拖拽物体直接放入储物箱中
- 采摘优化: 采摘时隐藏武器, 采摘完显示武器
- 地图扩展: 1. 扩展当前地上场景; 2. 加入地下洞穴场景
- 联机模式: 加入联机模式
- 地图生成: 使用四叉树动态生成地图

---


### 更新日志

2023.9.26:
- fix bug: 游戏加载页面无法覆盖全整个游戏画面导致四周出现缝隙可以看到地图加载内容

2023.9.25:
- fix bug: 修复状态机封装代码bug
  
2023.9.24:
- 存档系统: 重构部分逻辑
    1. 玩家按ESC后进入暂停窗口并返回主菜单
    2. 存档保存情况处理: 1. 正常情况, 例如玩家死亡后直接删档; 2. 非正常情况, 例如直接按Alt+F4或者是直接杀掉程序

2023.9.23:
- AI系统:
    1. 添加AI刷新机制, 当天数变更时刷新地图上的AI(每个地图块上限制1只AI)
    2. 添加蜘蛛, 蜘蛛有领地意识, 当玩家靠近蜘蛛时会使蜘蛛进行追击, 当玩家远离蜘蛛一定距离后蜘蛛才会放弃攻击
- 音效:
  1. 添加篝火音效
- 武器系统:
   1. 优化武器攻击, 添加武器打击粒子
- 玩家:
   1. 增加玩家受伤和死亡状态

2023.9.22:
- UI系统:
    1. 重构进度条逻辑, 之前是根据地图块加载进度来显示进度条数字，有时会导致刚加载好的地图块上AI会发出声音
- AI系统:
    1. 添加AI攻击状态, 包括攻击音效、攻击动画、以及攻击后立即切换到追击状态
    2. 添加AI死亡状态, 包括死亡动画以及死亡时从场景中移除
    3. 添加AI死亡掉落机制
- 武器:
    1. 添加武器音效

2023.9.21:
- AI系统:
    1. 完善AI追击状态逻辑, 当AI追击到其他地图块时需要迁移数据并更换AI物体的父物体

2023.9.20:
- AI系统:
    1. 添加AI待机/巡逻状态，在巡逻状态时保存位置坐标, 当存档恢复时根据结束游戏时存档恢复AI物体位置
    2. 添加检测玩家攻击AI的机制
    3. 添加AI脚步音效
    4. 添加AI受伤状态, 包括受伤音效、受伤动画、以及受伤后会切换到追击状态
    5. 添加AI追击状态, 目前仅需追着玩家跑即可

2023.9.19:
- AI系统:
    1. 添加AI物体存档和恢复存档机制
    2. 添加AI物体生成逻辑(仅生成物体未添加行为逻辑)


2023.9.18:
- AI系统:
    1. 添加导航烘焙
    2. 添加AI物体配置

2023.9.17:
- 建筑系统:
    1. 添加篝火数据存档功能
    2. 添加往篝火中放入木材增加燃烧时间的功能
    3. 添加篝火烧烤食物的功能, 目前仅支持制作熟浆果种子和熟蘑菇

2023.9.16:
- 建筑系统:
    1. 添加篝火预制体和配置文件
    2. 更新篝火功能, 目前篝火会随着时间变化燃烧, 一定时间后彻底熄灭

2023.9.15:
- 建造系统：
    1. 新增建筑物科学机器数据接口及设置建造条件
    2. 设置科学机器影响建筑物UI界面, 当前置条件满足后才会在合成/建造页面中显示对应物体的建造条件和介绍
    3. 添加2级储物箱

2023.9.14：
- 建造系统:
    1. 新增储物箱背包UI窗口
    2. 实现在地图中点击储物箱后出现窗口的功能
    3. 添加储物箱打开关闭音效
    4. 限制玩家交互据距离, 当玩家距离储物箱过远时关闭储物箱
    5. 禁止物品扔到储物箱上

2023.9.13:
- 背包系统: 重构背包系统代码, 抽象出背包窗口基类

2023.9.12:
- 添加物品: 浆果种子, 浆果丛掉落, 右键使用后恢复饥饿值, 后续添加浆果种子种植功能
- 建造系统: 添加种植浆果的功能
- 背包系统: 添加背包管理器, 为以后添加其他背包做准备

2023.9.11:
- 建造系统:
    1. 完善预览建筑物碰撞检测功能
    2. 在建造时屏蔽其他UI,完成建造或取消建造时恢复
    3. 完善浆果存档机制, 在采摘完浆果后及时进行存档
- fix bug:
    1. 修复物品掉落配置

2023.9.10:
- 建造系统:
    1. 增加建筑物预览时需要有格子吸附的效果
    2. 当建筑物在合适的地面上进行摆放时是绿色半透明, 如果摆放的格子上存在其他地图物品则变为红色半透明(设置物体材质MeshRender中RenderingMode)

2023.9.9:
- 建造系统: 当资源充足并点击建造按钮后生成建筑物的预览效果

2023.9.8：
- 建造系统: 添加储物箱配置, 同步更新旧配置代码

2023.9.7:
- 合成系统: 1. 合成后的物品经过检验后放入物品栏; 2. 整个二三级合成面板关闭逻辑优化; 3. 合成完毕后减少物品栏中对应的物品并更新页面状态

2023.9.6：
- 合成系统: 实现三级合成面板物品列表展示, 1. 显示/关闭列表

2023.9.5:
- 合成系统: 实现二级合成面板中选项后的内部逻辑, 1. 显示/关闭列表; 2. 切换列表; 3. 检查合成物品是否满足合成条件并且在列表中优先展示满足条件的物品;

2023.9.4:
- 合成系统: 添加物品合成配置, 例如到选中二级菜单中某个选项时三级面板需要根据这个配置来显示合成细节

2023.9.3:
- 合成系统: 创建UI建造窗口一级菜单, 编写菜单逻辑

2023.9.2:
- 添加物体在地图上的腐烂机制, 例如武器丢到地上一段时间后会销毁
- 完善地图刷新机制, 使用天数触发地图刷新机制

2023.9.1：
- 添加地图刷新机制, 每个早晨都刷新一次每个地图块上的物品: 1. 一个地图网格顶点如果已有物品则无法刷新; 2. 重构创建地图的部分功能

2023.8.31:
- 完善丢弃物品功能: 1. 增加物品资源补齐缺失配置. 目前新添加武器、蘑菇、浆果扔到地面上; 2. 禁止将物品扔到树木、石头、灌木上
- 修复小地图显示bug
- 修复存档恢复时地面上物体直接下坠的bug

2023.8.30:
- 完善丢弃物品功能, 由于其他物品没有现成的预制体, 因此目前仅支持将小石头、木材、树枝扔到地面上
- 修复堆叠物品丢弃时图标显示出错的问题

2023.8.29:
- 新增树枝掉落功能, 可以通过砍树或者是砍灌木获得树枝
- 新增石头掉落功能, 可以通过采石或者在地上拾取获得小石头

2023.8.28:
- 完善物品掉落功能, 目前支持砍树后掉落木材

2023.8.27:
- 开发简单的物品掉落功能: 1. 当物体死亡时物品可掉落; 2. 查找掉落物品id; 3. 计算掉落概率(如果单个物体掉落多个物品概率单独计算)并根据计算结果决定是否将掉落物实例化

2023.8.25:
- 物品掉落配置: 添加物品掉落配置(未实现该功能), 目前是通过id去查找掉落物品

2023.8.24:
- 添加摘取浆果功能, 并且浆果采摘后可以砍掉

2023.8.23:
- 添加收集碎石功能

2023.8.21:
- 添加手摘功能
- 添加采摘蘑菇功能

2023.8.20:
- 修复地图数据保存bug, 即地图重新载入档案后物体未按照预期消失, 例如砍伐完灌木后重新载入存档灌木仍然存在

2023.8.19:
- 设置镰刀攻击动作、音效以及攻击检测逻辑
- 设置灌木受击动作、受击逻辑

2023.8.18:
- 优化攻击逻辑和砸石头: 1. ; 2. 铁镐攻击及石头销毁

2023.8.16:
- 添加石头物体配置: 添加石头碰撞检测、受击音效、受击动画

2023.8.14:
- 重构地图对象存档数据结构: 梳理存档结构和逻辑, 替换掉其中使用的字典改为自定义可序列化字典, 支持在地图上移除地图物品的功能(例如砍树后树木需要在地图上消失)
- UI小地图物体销毁同步更新

2023.8.13:
- 武器耐久度降低及损坏: 根据配置文件中每次攻击降低耐久度数值去更新当前存档中武器耐久度数据和更新UI数值
- 添加树木受击动画: 添加树木受伤动作; 扣除树木生命值; 树木死亡时销毁模型
- 字典持久化: 字典作为内置类型没有方便的持久化方法, 为了方便修改后续存档模块, 因此需要实现字典持久化功能

2023.8.12:
- 添加InputManager: 负责检测单位是否被选中
- 添加主角挥击武器逻辑: 具体逻辑根据玩家手中武器和鼠标点击物体类型决定
- 设置主角挥击武器动作: 设置一个动作帧中前后摇及具体攻击帧. 设置攻击方向, 当玩家需要转向时需要平滑过渡
- 武器伤害检测: 创建触发器并且进行监听, 简单测试功能是否能正常运行

2023.8.11:
- 配置武器: 完善物品类型设置, 添加武器类型、耐久、攻击力、图标、prefab、animator等配置
- 武器槽改变时更新角色数值、模型、动画等相关设置

2023.8.10:
- 优化物品栏功能: 1. 右键点击武器可以进行装备, 如果当前武器栏有装备则进行交换武器; 2. 右键点击消耗品会加血或者增加饱食度; 3. 材料目前无法右键点击使用

2023.8.7:
- 增加物品栏添加物品音效
- 修复物品栏代码bug

2023.8.6:
- 优化物品栏代码逻辑

2023.8.5:
- 快捷栏物品添加、丢弃、使用逻辑优化

2023.8.2:
- 物品添加逻辑优化

2023.8.1:
- 创建角色配置数据(生命值、饱食度、生命值下降速度、饱食度下降速度等)和存档逻辑
- 将角色配置数据与游戏场景中UI相关联, UI会随玩家饥饿值、生命值下降而变化
- 添加通用音效配置, 为后续开发添加使用、捡起、丢弃物品音效功能做准备

2023.7.31:
- fix bug: 1. 修复在加载页面时可以按"M"出现小地图的bug; 2. 修复部分地图块在加载存档时直接显示的bug
- 重构游戏时间数据+配置逻辑, 便于存档
- 重构时间管理器: 1. 白天或者野外更替时对外发出事件; 2. 天数变化时对外发出事件

2023.7.30:
- 添加物品拖拽代码, 1. 支持物品在物品从快捷栏中任意一处拖拽到另一处; 2. 支持将物品拖拽到地面上; 3. 支持物品拖拽过程中的类型检测, 比如消耗品不能拖拽到武器栏上

2023.7.29:
- 完善物品快捷栏代码, 经过简单测试后现有物品可以正确显示在物品快捷栏上

2023.7.28:
- 完善物品配置代码, 创建物品数据类
- 搭建物品快捷栏UI, 并设置物品快捷栏初始化及存档逻辑框架

2023.7.27:
- 添加游戏进入加载页面
- 添加物品配置代码, 支持在Unity编辑器Project界面上添加对应配置文件, 例如配置武器、材料、消耗品等信息

之前: 未记录开发过程, 后续可能补充


---

### Bug修复记录

2023.9.26 游戏加载页面无法覆盖全整个游戏画面导致四周出现缝隙可以看到地图加载内容
- 原因: 游戏加载页面尺寸设置小了
- 修复: 修正游戏加载页面尺寸

2023.9.25 切换场景时AI执行Update时会访问到已经销毁的PlayerController.Instance导致出错
- 原因: 在状态机这里不注意给改成了"currStateObj == null"，这导致AI在Destory时停止状态机的时候状态机里的当前状态（currStateObje）没有退出和那些删除Update方法，所以后续在框架MonoManager里还会持续执行这些Update，然后访问了已经销毁的PlayerController.Instance才出的错
- 修复: 已修复并通过测试

2023.9.11 物品掉落配置可能出现了错误, 击打石头后掉落的是草莓
- 原因: 物品掉落配置中配置Id错误
- 修复: 已修改配置内容并通过测试

2023.8.31 存档恢复时地面上物体直接下坠的bug
- 原因: 恢复存档期间地面碰撞体并未完全生成, 但是物品已经生成了或者是物品部分模型在一个未生成的地面时会导致坠落
- 修复: 之前每个地图块中有一个碰撞体, 现在将整个地面设置成一整个碰撞体, 在初始化时先根据地图大小初始化出来该地面碰撞体解决这个问题

2023.8.31 小地图中物品id初始化错误
- 原因: 小地图物品数据初始化时id出现错误未能正确赋值, 因此导致按下M键后小地图UI无法初始化
- 修复: 修复物品数据初始化时id问题

2023.8.30 堆叠物品丢弃时图标显示出错
- 原因: 结束拖拽时如果将物品扔到了地上在代码中未将物品对象icon位置复原
- 修复: 当拖拽结束时如果物品扔到了地上首先恢复icon位置

2023.8.25 采摘: 现象是当手拿镰刀采摘时会攻击浆果灌木丛
- 原因: 当点击一次鼠标的时间内游戏会执行很多帧, 所以在执行采摘动作时可采摘标记已经被标记成false了, 此时玩家处于可攻击状态, 因此导致了在采摘时会攻击浆果灌木丛
- 修复: 添加了一个标记变量记录当前mapobject

2023.8.25 装备的采集武器攻击时耐久度出错, 单次攻击耐久度会减少很多~
- 原因: 攻击事件是在UIMapWindow中去注册的, 再检查地图物品时会调用该UI窗口, 导致事件注册出问题(待补充)
- 修复: 暂时使用单例去修复物品快捷栏事件注册导致的该问题