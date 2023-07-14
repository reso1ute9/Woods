using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class Player_Model : MonoBehaviour {
    private Action<int> footstepAction;
    
    public void Init(Action<int> footstepAction) {
        this.footstepAction = footstepAction;
    }

    // 控制动画
    #region 动画事件
    private void Footstep(int index) {
        footstepAction?.Invoke(index);
    }
    #endregion
}
