using System.Security.Cryptography.X509Certificates;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapChunkController : MonoBehaviour
{
    public Vector3 centrePosition { get; private set; }
    public Vector2Int chunkIndex { get; private set; }
    private bool isActive = false;

    public void Init(Vector2Int chunkIndex, Vector3 centrePosition) {
        this.centrePosition = centrePosition;
        this.chunkIndex = chunkIndex;
    }

    public void SetActive(bool active) {
        if (isActive != active) {
            isActive = active;
            gameObject.SetActive(isActive);
            // 基于对象池生成所有对象
        }
    }
}
