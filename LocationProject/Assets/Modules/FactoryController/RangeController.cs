﻿using Mogoson.CameraExtension;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RangeController : DepNode
{
    /// <summary>
    /// 范围碰撞体
    /// </summary>
    public BoxCollider depCollider;
    ///// <summary>
    ///// 存放房间设备处
    ///// </summary>
    //private GameObject _roomDevContainer;
    ///// <summary>
    ///// 存放房间设备处
    ///// </summary>
    //public GameObject RoomDevContainer
    //{
    //    get
    //    {
    //        if (_roomDevContainer == null) InitDevContainer();
    //        return _roomDevContainer;
    //    }
    //}
    ///// <summary>
    ///// 设备是否创建
    ///// </summary>
    //private bool _isDevCreate;
    ///// <summary>
    ///// 机房设备是否创建
    ///// </summary>
    //public bool IsDevCreate
    //{
    //    get { return _isDevCreate; }
    //    set { _isDevCreate = value; }
    //}
    ///// <summary>
    ///// 设备创建完成
    ///// </summary>
    //private Action OnDevCreateComplete;
    // Use this for initialization
    void Start()
    {
        depType = DepType.Range;
    }

    // Update is called once per frame
    void Update()
    {

    }
    /// <summary>
    /// 设置监控区域
    /// </summary>
    /// <param name="oT"></param>
    public override void SetMonitorRangeObject(MonitorRangeObject oT)
    {
        monitorRangeObject = oT;
        //InitDevContainer();
    }

    /// <summary>
    /// 打开区域
    /// </summary>
    /// <param name="onComplete"></param>
    public override void OpenDep(Action onComplete = null, bool isFocusT = true)
    {
        HideFacotry();
        BuildingController building = ParentNode.ParentNode as BuildingController;
        if (building != null) building.LoadRoom(ParentNode, true, floor =>
        {
            //OnDevCreateComplete = onComplete;
            DepNode lastDep = FactoryDepManager.currentDep;
            FactoryDepManager.currentDep = this;
            SceneEvents.OnDepNodeChanged(lastDep, this);

            if (isFocusT)
            {
                //Todo:摄像头聚焦    
                FocusOn(() =>
                {
                    if (onComplete != null)
                    {
                        onComplete();
                    }
                    AfterRangeFocus(true);
                });
            }
            else
            {
                if (onComplete != null)
                {
                    onComplete();
                }
                AfterRangeFocus(true);
            }
        });
        else
        {
            DepNode lastDep = FactoryDepManager.currentDep;
            FactoryDepManager.currentDep = this;
            SceneEvents.OnDepNodeChanged(lastDep, this);

            if (isFocusT)
            {
                FocusOn(() =>
                {
                    if (onComplete != null)
                    {
                        onComplete();
                    }
                    AfterRangeFocus(true);
                });
            }
            else
            {
                if (onComplete != null)
                {
                    onComplete();
                }
                AfterRangeFocus(true);
            }
        }
    }

    public override void HideDep(Action onComplete = null)
    {
        FlashingOff();
        if (ParentNode != null)
        {
            ParentNode.HideDep();
        }
    }

    /// <summary>
    /// 房间聚焦之后
    /// </summary>
    private void AfterRangeFocus(bool isFlashing)
    {
        FlashingRoom();
        //CreateRoomDev(OnDevCreateComplete);
    }

    /// <summary>
    /// 隐藏厂区建筑物
    /// </summary>
    private void HideFacotry()
    {
        FactoryDepManager manager = FactoryDepManager.Instance;
        if (manager) manager.HideFacotry();
    }

    //private void InitDevContainer()
    //{
    //    if (_roomDevContainer != null) return;
    //    _roomDevContainer = new GameObject("RoomDevContainer");
    //    _roomDevContainer.transform.parent = transform;
    //    FloorController floor = ParentNode as FloorController;
    //    if (floor)
    //    {
    //        if (monitorRangeObject != null)
    //        {
    //            transform.position = monitorRangeObject.transform.position;
    //        }
    //        Transform parentCotainer = floor.RoomDevContainer.transform;
    //        _roomDevContainer.transform.position = parentCotainer.position;
    //        _roomDevContainer.transform.eulerAngles = parentCotainer.eulerAngles;
    //        _roomDevContainer.transform.localScale = parentCotainer.lossyScale;
    //    }
    //}
    ///// <summary>
    ///// 隐藏机房设备
    ///// </summary>
    //public void HideRoomDev()
    //{
    //    RoomDevContainer.SetActive(false);
    //}
    ///// <summary>
    ///// 显示机房设备
    ///// </summary>
    //public void ShowRoomDev()
    //{
    //    RoomDevContainer.SetActive(true);
    //}

    public void FocusOn(Action onDevCreateFinish = null)
    {
        IsFocus = true;
        //OnDevCreateComplete = onDevCreateFinish;
        DepNode lastDep = FactoryDepManager.currentDep;
        FactoryDepManager.currentDep = this;
        SceneEvents.OnDepNodeChanged(lastDep, this);
        //Todo:摄像头聚焦    
        AlignTarget alignTargetTemp;
        alignTargetTemp = monitorRangeObject != null ? GetTargetInfo(monitorRangeObject.gameObject) : GetTargetInfo(gameObject);
        CameraSceneManager camera = CameraSceneManager.Instance;
        //FlashingRoom();
        camera.FocusTargetWithTranslate(alignTargetTemp, AreaSize, AfterRoomFocus);
    }
    /// <summary>
    /// 房间聚焦之后
    /// </summary>
    private void AfterRoomFocus()
    {
        FlashingRoom();
        //CreateRoomDev(OnDevCreateComplete);
    }
    ///// <summary>
    ///// 创建机房设备
    ///// </summary>
    //public void CreateRoomDev(Action onDevCreateCompleteT = null)
    //{
    //    try
    //    {
    //        if (ParentNode != null)
    //        {
    //            FloorController controller = ParentNode as FloorController;
    //            controller.CreateFloorDev(onDevCreateCompleteT);
    //        }
    //    }
    //    catch (Exception e)
    //    {
    //        Debug.LogError("RoomController.CreateRoomdev :" + e.ToString());
    //        if (onDevCreateCompleteT != null) onDevCreateCompleteT();
    //    }
    //}
    /// <summary>
    /// 房间闪烁
    /// </summary>
    /// <param name="isShow">是否闪烁</param>
    private void FlashingRoom()
    {
        if (monitorRangeObject == null) return;
        monitorRangeObject.FlashingOn(Color.green, 2f);
        if (IsInvoking("FlashingOff"))
        {
            CancelInvoke("FlashingOff");
            Invoke("FlashingOff", 1.5f);
        }
        else
        {
            Invoke("FlashingOff", 1.5f);
        }
    }
    /// <summary>
    /// 关闭闪烁
    /// </summary>
    private void FlashingOff()
    {
        if (monitorRangeObject == null) return;
        monitorRangeObject.FlashingOff();
    }

    public void FocusOff()
    {
        FlashingOff();
        if (ParentNode != null)
        {
            ParentNode.HideDep();
            //FloorController controller = ParentNode as FloorController;
            //controller.RecoverBuilding();
        }
    }
    /// <summary>
    /// 是否在机房区域内
    /// </summary>
    /// <param name="devPos"></param>
    /// <returns></returns>
    public bool IsInDepField(Vector3 devPos)
    {
        if (depCollider == null) return false;
        depCollider.enabled = true;
        Bounds bounds = depCollider.bounds;
        bool rendererIsInsideTheBox = bounds.Contains(devPos);
        depCollider.enabled = false;
        return rendererIsInsideTheBox;
    }

    public Vector2 angleFocus = new Vector2(40, 270);
    public float camDistance = 10;
    [HideInInspector]
    public Range angleRange = new Range(0, 90);
    public Range disRange = new Range(2, 30);
    //拖动区域大小
    public Vector2 AreaSize = new Vector2(2, 2);
    /// <summary>
    /// 获取相机聚焦物体的信息
    /// </summary>
    /// <param name="obj"></param>
    /// <returns></returns>
    private AlignTarget GetTargetInfo(GameObject obj)
    {
        AlignTarget alignTargetTemp = new AlignTarget(obj.transform, angleFocus,
                               camDistance, angleRange, disRange);
        return alignTargetTemp;
    }
}
