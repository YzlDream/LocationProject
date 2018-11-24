using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 如何让OnMouseDown忽略有碰撞器的物体
/// </summary>
public class OnMouseDownIgnoreRayCast : MonoBehaviour {

    // Use this for initialization
    void Start() {
        DoubleClickEventTrigger_u3d u3dTrigger = DoubleClickEventTrigger_u3d.Get(gameObject);
        u3dTrigger.onClick += OnClick;
    }

    // Update is called once per frame
    void Update() {

    }

    public void OnClick()
    {
        Debug.Log("OnClick!!!");
    }

    private void OnTriggerEnter(Collider other)
    {
        Debug.Log("OnTriggerEnter!!!");
    }

    private void OnTriggerStay(Collider other)
    {

    }

    private void OnTriggerExit(Collider other)
    {
        Debug.Log("OnTriggerExit!!!");
    }
}
