using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using JKFrame;
using Unity.VisualScripting;
using Sirenix.Utilities;


// 合成窗口的二级菜单
public class UI_BuildWindow_SecondaryMenu : MonoBehaviour
{
    [SerializeField] Transform itemParent;
    [SerializeField] GameObject itemPrefab;

    private Dictionary<BuildType, List<BuildConfig>> buildConfigDict;               // 所有的BuildConfig按照类型分类 
    private UI_BuildWindow_SecondaryMenuItem currentSecondaryMenuItem;              // 当前选中的二级菜单选项
    private List<UI_BuildWindow_SecondaryMenuItem> currentSecondaryMenuItemList;    // 当前显示的二级菜单列表
    private List<BuildConfig> meetTheConditionList;                                 // 当前满足条件的配置
    private List<BuildConfig> failToMeetConditionList;                              // 当前不满足条件的配置
    public UI_BuildWindow_BuildPanel currentBuildPanel;                            // 当前三级窗口(二级菜单选项说明窗口)
    private BuildType currentBuildType;

    public void Init() {
        // 构建配置文件
        buildConfigDict = new Dictionary<BuildType, List<BuildConfig>>(3);
        buildConfigDict.Add(BuildType.Weapon, new List<BuildConfig>());
        buildConfigDict.Add(BuildType.Building, new List<BuildConfig>());
        buildConfigDict.Add(BuildType.Crop, new List<BuildConfig>());

        Dictionary<int, ConfigBase> buildConfigs = ConfigManager.Instance.GetConfigs(ConfigName.Build);
        foreach (ConfigBase config in buildConfigs.Values) {
            BuildConfig buildConfig = (BuildConfig)config;
            buildConfigDict[buildConfig.buildType].Add(buildConfig);
        }
        currentSecondaryMenuItemList = new List<UI_BuildWindow_SecondaryMenuItem>(10);
        meetTheConditionList = new List<BuildConfig>(10);
        failToMeetConditionList = new List<BuildConfig>(10);
        Close();
        // 初始化三级窗口
        currentBuildPanel.Init(this);
    }

    // 根据合成类型显示不同列表
    public void Show(BuildType buildType) {
        this.currentBuildType = buildType;
        // 检查旧列表是否存在, 如果存在则需要把列表中的对象放入对象池
        for (int i = 0; i < currentSecondaryMenuItemList.Count; i ++) {
            currentSecondaryMenuItemList[i].JKGameObjectPushPool();
        }
        currentSecondaryMenuItemList.Clear();
        meetTheConditionList.Clear();
        failToMeetConditionList.Clear();
        // 当前类型的配置
        List<BuildConfig> buildConfigList = buildConfigDict[buildType];
        // 对配置进行分类显示, 分为满足条件和不满足条件
        for (int i = 0; i < buildConfigList.Count; i++) {
            // 如果没有满足前置科技条件则不显示物品建造配置
            if (buildConfigList[i].CheckPreconditionScienceId() == false) {
                continue;
            }
            if (buildConfigList[i].CheckBuildConfigCondition() == true) {
                meetTheConditionList.Add(buildConfigList[i]);
            } else {
                failToMeetConditionList.Add(buildConfigList[i]);
            }
        }
        // 添加满足条件和不满足条件对应的二级菜单选项
        for (int i = 0; i < meetTheConditionList.Count; i++) {
            AddSecondaryMenuItem(meetTheConditionList[i]);
        }
        for (int i = 0; i < failToMeetConditionList.Count; i++) {
            AddSecondaryMenuItem(failToMeetConditionList[i]);
        }
        gameObject.SetActive(true);
    }

    // 刷新当前二级菜单视图
    public void RefreshView() {
        Show(this.currentBuildType);
        for (int i = 0; i < currentSecondaryMenuItemList.Count; i++) {
            if (currentSecondaryMenuItemList[i].buildConfig == currentSecondaryMenuItem.buildConfig) {
                currentSecondaryMenuItemList[i].Select();
            }
        }
    }

    // 获取二级菜单选项
    private void AddSecondaryMenuItem(BuildConfig buildConfig) {
        // 从对象池中获取菜单选项
        UI_BuildWindow_SecondaryMenuItem menuItem = PoolManager.Instance.GetGameObject<UI_BuildWindow_SecondaryMenuItem>(itemPrefab, itemParent);
        // 放到当前list中
        currentSecondaryMenuItemList.Add(menuItem);
        menuItem.Init(buildConfig, this);
    }

    // 选中了某个二级菜单选项
    public void SelectSecondaryMenuItem(UI_BuildWindow_SecondaryMenuItem secondaryMenuItem) {
        if (currentSecondaryMenuItem != null) {
            currentSecondaryMenuItem.UnSelect();
        }
        currentSecondaryMenuItem = secondaryMenuItem;
        currentSecondaryMenuItem.Select();
        // 显示三级窗口
        currentBuildPanel.Show(currentSecondaryMenuItem.buildConfig);
    }

    public void Close() {
        gameObject.SetActive(false);
        // 关闭三级窗口
        currentBuildPanel.Close();
    }
}
