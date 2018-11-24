﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;


public class EntranceManage : MonoBehaviour {
    public static EntranceManage instance;
    

    /// <summary>
    /// 第一个入口
    /// </summary>
    public GameObject FirstObj;
    /// <summary>
    /// 第二个入口
    /// </summary>
    public GameObject SecoundObj;
    /// <summary>
    /// 第三个入口
    /// </summary>
    public GameObject ThirdObj;
    /// <summary>
    /// 第四个入口
    /// </summary>
    public GameObject ForthObj;
    /// <summary>
    /// 第五个入口
    /// </summary>
    public GameObject FiveObj;

 /// <summary>
 /// 点击时入口图片
 /// </summary>
    public Sprite ClickImage;
    /// <summary>
    /// 不点击入口图片
    /// </summary>
    public Sprite NormalImage;
    /// <summary>
    /// 入口选择ui界面
    /// </summary>
    public GameObject Window;
   

    void Start () {
        instance = this;
        FirstObj.transform.GetChild(0).GetChild(1).GetComponent<Button>().onClick.AddListener(()=>
        {
            FirstBut_Click(FirstObj.transform);
        });
        SecoundObj.transform.GetChild(0).GetChild(1).GetComponent<Button>().onClick.AddListener(()=>
        {
            SecoundBut_Click(SecoundObj.transform);
        });

        ThirdObj.transform.GetChild(0).GetChild(1).GetComponent<Button>().onClick.AddListener(()=>
        {
            ThirdBut_Click(ThirdObj.transform);
        });

        ForthObj.transform.GetChild(0).GetChild(1).GetComponent<Button>().onClick.AddListener(()=>
        {
            ForthBut_Click(ForthObj.transform);
        });

        FiveObj.transform.GetChild(0).GetChild(1).GetComponent<Button>().onClick.AddListener(()=> 
        {
            FiveBut_Click(FiveObj.transform );
        });

        ClickChange(FirstObj);
        ClickChange(SecoundObj);
        ClickChange(ThirdObj);
        ClickChange(ForthObj);
        ClickChange(FiveObj);
       
    }
	/// <summary>
    /// 点击第一个入口
    /// </summary>
    public void FirstBut_Click(Transform obj)
    {
        ClickEntranceBut(obj);
        FPSMode.Instance.FPSController.transform.GetComponent<Transform>().localPosition = new Vector3(95, 0.65f, -68.2f);
    }
    /// <summary>
    /// 点击第二个入口
    /// </summary>
    public void SecoundBut_Click(Transform obj)
    {
        ClickEntranceBut(obj);
        FPSMode.Instance.FPSController.transform.GetComponent<Transform>().localPosition = new Vector3(95, 0.65f, 21.45F );
    }
    /// <summary>
    /// 点击第三个入口
    /// </summary>
    public void ThirdBut_Click(Transform obj)
    {
        ClickEntranceBut(obj);
        FPSMode.Instance.FPSController.transform.GetComponent<Transform>().localPosition = new Vector3(29, 0.65f, -11f);
    }
    /// <summary>
    /// 点击第四个入口
    /// </summary>
    public void ForthBut_Click(Transform obj)
    {
        ClickEntranceBut(obj);
        FPSMode.Instance.FPSController.transform.GetComponent<Transform>().localPosition = new Vector3(-53, 0.65f, -10.6f);
    }
    /// <summary>
    /// 点击第五个入口
    /// </summary>
    public void FiveBut_Click(Transform  obj)
    {
        ClickEntranceBut(obj);
        FPSMode.Instance.FPSController.transform.GetComponent<Transform>().localPosition = new Vector3(-115, 0.65f, -13f);
    }
    /// <summary>
    /// 鼠标点击入口按钮时的操作
    /// </summary>
    /// <param name="obj"></param>
    public void ClickEntranceBut(Transform obj)
    {
        EntranceTween.instance.SetButtonExitTweener(obj.GetChild(0));
        ShowWindow(false);
        FPSMode.Instance.SwitchTo(true);
        RoamManage.Instance. FPSController.ChangeGravityValue(1f);
        obj.GetChild(1).GetComponent<Image>().color = new Color(0, 0, 0, 0);
        obj.GetChild(0).GetChild(0).GetComponent<Text>().color = new Color(109f / 255, 236f / 255, 254f / 255, 255 / 255);
        obj.GetChild(0).GetComponent<Image>().sprite = NormalImage;
        RoamManage.Instance.EntranceRoam();
      //  FPSMode.Instance.SetBorder(true);
    }
    public void ClickChange(GameObject obj)
    {
        EventTriggerListener ObjColor = EventTriggerListener.Get(obj);
        ObjColor.onEnter  = UpButton;
        ObjColor.onExit = NoClickButton;
      
    }
    /// <summary>
    /// 鼠标放上ui界面时的改变
    /// </summary>
    /// <param name="obj"></param>
    public void UpButton(GameObject obj)
    {
        obj.transform.GetChild(1).GetComponent<Image>().color = new Color(255 / 255, 255 / 255, 255 / 255, 255 / 255);
        obj.transform.GetChild(0).GetChild(0).GetComponent<Text>().color = new Color(255 / 255, 255 / 255, 255 / 255, 255 / 255);
        obj.transform.GetChild(0).GetComponent<Image>().sprite = ClickImage;
        
         EntranceTween.instance.SetButtonEnterTweener(obj.transform.GetChild(0));
      
    }
   
    /// <summary>
    /// 鼠标离开时ui界面的改变
    /// </summary>
    /// <param name="obj"></param>
    public void NoClickButton(GameObject obj)
    {
        obj.transform.GetChild(1).GetComponent<Image>().color = new Color(0, 0, 0, 0);
        obj.transform.GetChild(0).GetChild(0).GetComponent<Text>().color = new Color(109f / 255, 236f / 255, 254f / 255, 255 / 255);
        obj.transform.GetChild(0).GetComponent<Image>().sprite = NormalImage;
        Debug.Log(12365);
        EntranceTween.instance.SetButtonExitTweener(obj.transform.GetChild(0));
        Debug.Log(98756);
    }
    /// <summary>
    /// 是否打开选择入口界面
    /// </summary>
    /// <param name="b"></param>
    public void ShowWindow(bool b)
    {
        if (b)
        {
            Window.SetActive(true);
           
        }
        else
        {
            Window.SetActive(false);
        }
    }
    void Update()
    {
        if (Window.activeInHierarchy)
        {
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                Window.SetActive(false);
                DevSubsystemManage.Instance.ChangeImage(false, DevSubsystemManage.Instance.RoamToggle);
                DevSubsystemManage.Instance.RoamToggle.isOn = false;
            }
        }
    } 
}
