using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ProgressbarLoad : MonoBehaviour {
    public static ProgressbarLoad Instance;
    public GameObject window;//窗口
    public Slider slider;//进度条
    public Text txtValue;//进度条

    void Awake()
    {
        Instance = this;
    } 

    // Use this for initialization
    void Start () {

        if (slider == null)
        {
            slider = gameObject.GetComponentInChildren<Slider>(true);
        }

        slider.onValueChanged.AddListener(Slider_OnValueChanged);

    }
	
	// Update is called once per frame
	void Update () {
		
	}

    /// <summary>
    /// 显示进度条
    /// </summary>
    /// <param name="v"></param>
    public void Show(float v)
    {
        //if (v >= 0.1f)
        //{
            slider.value = v;
        //}
        //   else
        //{
        //    slider.value = 0.1f;
        //}
        SetWindow(true);
    }

    /// <summary>
    /// 隐藏
    /// </summary>
    public void Hide()
    {
        SetWindow(false);
    }

    /// <summary>
    /// Slider_OnValueChanged
    /// </summary>
    /// <param name="v"></param>
    private void Slider_OnValueChanged(float v)
    {
        //if (v >= 0.1f)
        //{
            v = (float)Math.Round(v, 2);
            txtValue.text = "" + v * 100;
        //}
        
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
