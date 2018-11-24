using System.Collections;
using System.Collections.Generic;
using UIWidgets;
using UnityEngine;
using UnityEngine.UI;
public class FacilityDevTreeItem : MonoBehaviour {

    /// <summary>
    /// 状态
    /// </summary>
    public Text StatusText;
    /// <summary>
    /// 值
    /// </summary>
    public Text ValueText;
	// Use this for initialization
	void Start () {

    }
    public void SetValue(FacilitySystem SystemInfo)
    {
        if(string.IsNullOrEmpty(SystemInfo.Value))
        {
            ValueText.text = "/";
        }
        else
        {
            ValueText.text = SystemInfo.Value;
        }
        StatusText.text = SystemInfo.Status;
    }
}
