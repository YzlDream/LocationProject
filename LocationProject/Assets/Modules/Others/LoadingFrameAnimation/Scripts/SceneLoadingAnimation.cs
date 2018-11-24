using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SceneLoadingAnimation : MonoBehaviour {

    public static SceneLoadingAnimation Instance;
    public UGUISpriteAnimation uiAnimation;//ui动画控制脚本
    private GameObject window;//窗口
    //public Slider slider;//进度条
    public Text txtValue;//进度条

    void Awake()
    {
        Instance = this;
    }

    // Use this for initialization
    void Start()
    {
        window = uiAnimation.gameObject;

    }

    // Update is called once per frame
    void Update()
    {

    }

    //public void Test()
    //{

    //}

    /// <summary>
    /// 显示进度条
    /// </summary>
    /// <param name="v"></param>
    public void Show(float v)
    {
        v = (float)Math.Round(v, 2);
        txtValue.text = "" + v * 100;
        if (!window.activeInHierarchy)
        {
            uiAnimation.Play();
        }
        SetWindow(true);

    }

    /// <summary>
    /// 隐藏
    /// </summary>
    public void Hide()
    {
        SetWindow(false);
        uiAnimation.Stop();
    }
    

    /// <summary>
    /// 设置窗口的显示隐藏
    /// </summary>
    /// <param name="isActive"></param>
    public void SetWindow(bool isActive)
    {
        window.SetActive(isActive);
    }
}
