# forst
forst -- 3d survival type game demo

![Image text](Assets/Images/images_menu.png)




视频演示:【Unity个人自学demo】 https://www.bilibili.com/video/BV1Qc411c7xu/?share_source=copy_web&vd_source=cf326a22ef42c23caf731f27146e385a


----

更新日志: 
- 2023/7/31:
    - fix bug: 1. 修复在加载页面时可以按"M"出现小地图的bug; 2. 修复部分地图块在加载存档时直接显示的bug

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

TODO:
- 拖拽: 物品无法拖拽到某些物体上, 例如树木石头
- 项目中仍然存在一些默认值没有配置化, 后续可以设置单独设置配置文件, 替换掉项目中的明文数值