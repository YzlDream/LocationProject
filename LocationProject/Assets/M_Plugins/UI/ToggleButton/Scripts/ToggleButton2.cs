using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ToggleButton2 : MonoBehaviour {
    /// <summary>
    /// 按钮
    /// </summary>
    public Button btn;
    /// <summary>
    /// 图片
    /// </summary>
    public Image image;
    /// <summary>
    /// 正常图片
    /// </summary>
    public Sprite normalSprite;
    /// <summary>
    /// 选中图片
    /// </summary>
    public Sprite selectSprite;
    /// <summary>
    /// 是否选中
    /// </summary>
    public bool ison;

    /// <summary>
    /// 值改变触发事件
    /// </summary>
    public Action<bool> OnValueChanged;

    // Use this for initialization
    void Start () {
        if (btn == null)
        {
            btn = GetComponent<Button>();
        }
        if (image == null)
        {
            image = btn.image;
        }
        btn.onClick.AddListener(Btn_OnClick);
        SetImage(ison);
    }
	
	// Update is called once per frame
	void Update () {
		
	}

    /// <summary>
    /// 按钮触发事件
    /// </summary>
    public void Btn_OnClick()
    {
        ison = !ison;
        SetImage(ison);
        if (OnValueChanged != null)
        {
            OnValueChanged(ison);
        }
    }

    /// <summary>
    /// 设置按钮图片
    /// </summary>
    private void SetImage(bool ison)
    {
        if (ison)
        {
            image.sprite = selectSprite;
        }
        else
        {
            image.sprite = normalSprite;
        }
    }
}
