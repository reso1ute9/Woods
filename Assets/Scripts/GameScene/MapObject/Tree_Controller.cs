using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tree_Controller : MapObjectBase {
    public void Hurt(float damage) {
        UnityEngine.Debug.Log("树受伤了:" + damage);
    }

}
