using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using JKFrame;

public class UI_BuildWindow_BuildPanelItem : MonoBehaviour
{
    static Color isMeetColor = Color.white;
    static Color NoMeetColor = new Color(0.9528f, 0,4809f, 0.4809f);

    [SerializeField] Image iconImage;
    [SerializeField] Text context;

    public void Show(int configId, int currentCount, int needCount) {
        ItemConfig itemConfig = ConfigManager.Instance.GetConfig<ItemConfig>(ConfigName.Item, configId);
        iconImage.sprite = itemConfig.itemIcon;
        context.text = currentCount.ToString() + "/" + needCount.ToString();
        if (currentCount >= needCount) {
            context.color = isMeetColor;
        } else {
            context.color = NoMeetColor;
        }
        gameObject.SetActive(true);
    }

    public void Close() {
        gameObject.SetActive(false);
    }
}
