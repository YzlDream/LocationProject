using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AfterEntranceGuardManage : MonoBehaviour {
    public static AfterEntranceGuardManage Instance;
    /// <summary>
    /// 关闭按钮
    /// </summary>
    public Button CloseButton;
    /// <summary>
    /// 经过门禁（24小时内）
    /// </summary>
    public GameObject Window;
    // Use this for initialization
    void Start () {
        Instance = this;
        CloseButton.onClick.AddListener(CloseWindow);
    }
	
	// Update is called once per frame
	void Update () {
		
	}
    public void ShowWindow()
    {
        Window.SetActive(true);
    }
    public void CloseWindow()
    {
        Window.SetActive(false);
    }
}
