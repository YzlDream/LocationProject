﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RTEditor;
using Location.WCFServiceReferences.LocationServices;
using Assets.M_Plugins.Helpers.Utils;

public class DeviceEditUIManager : MonoBehaviour
{
    public static DeviceEditUIManager Instacne;
    /// <summary>
    /// 设备编辑部分
    /// </summary>
    public DeviceEdit EditPart;
    /// <summary>
    /// 设备名称（漂浮UI）
    /// </summary>
    private DevInfoFollowUI DevInfo;
    /// <summary>
    /// UI跟随名称
    /// </summary>
    private string followName = "DeviceEditUI";
    /// <summary>
    /// 物体顶部TitleObj
    /// </summary>
    private string TitleObjName = "TitleObj";
    /// <summary>
    /// 跟随物体
    /// </summary>
    private GameObject _followObject;
    /// <summary>
    /// 跟随物体预设
    /// </summary>
    public GameObject FollowPrefab;

    /// <summary>
    /// 当前多选设备
    /// </summary>
    private List<DevNode> CurrentDevList = new List<DevNode>();
    /// <summary>
    /// 窗体
    /// </summary>
    public GameObject Window;

    /// <summary>
    /// 漂浮UI集合
    /// </summary>
    private List<GameObject> FollowObject = new List<GameObject>();
    // Use this for initialization
    void Start()
    {
        Instacne = this;
    }
    /// <summary>
    /// 显示编辑窗体
    /// </summary>
    public void Show(DevNode dev)
    {
        if (_followObject != null)
        {
            _followObject.SetActive(true);
        }
        Window.SetActive(true);
        CurrentDevList.Clear();
        CurrentDevList.Add(dev);
        SetFollowTarget(dev.gameObject);
        EditPart.SetDeviceInfo(dev);
        DevInfo.Show(dev);
        ShowArchorPart(dev);
    }
    /// <summary>
    /// 多选设备
    /// </summary>
    /// <param name="devList"></param>
    public void ShowMultiDev(List<DevNode> devList)
    {
        Close();//关闭单个设备的界面
        HideMultiDev();
        CurrentDevList.Clear();
        SetFollowTarget(devList);
    }
    /// <summary>
    /// 关闭设备多选情况
    /// </summary>
    public void HideMultiDev()
    {
        foreach (var item in FollowObject)
        {
            item.SetActive(false);
        }
    }
    /// <summary>
    /// 刷新设备编辑Gizmo位置
    /// </summary>
    public void RefleshGizmoPosition()
    {
        Gizmo activeGizmo = EditorGizmoSystem.Instance.ActiveGizmo;
        if (activeGizmo == null)
        {
            Debug.Log("Active Gizmo is null...");
            return;
        }
        activeGizmo.transform.position = EditorObjectSelection.Instance.GetSelectionWorldCenter();
    }
    /// <summary>
    /// 更改设备漂浮信息
    /// </summary>
    /// <param name="dev"></param>
    public void ChangeDevFollowInfo(DevNode dev)
    {
        DevInfo.Show(dev);
    }
    /// <summary>
    /// 关闭编辑窗体
    /// </summary>
    public void Close()
    {
        if(Window.activeInHierarchy)
        {
            CloseArchorTool();
            Window.SetActive(false);
            if (_followObject)
                _followObject.SetActive(false);
        }             
    }


    /// <summary>
    /// 删除设备
    /// </summary>
    public void DeleteDev(DevNode dev)
    {
        DevNode currentDev = CurrentDevList.Find(item=>item==dev);
        if (currentDev != null && currentDev.Info != null)
        {
            CommunicationObject service = CommunicationObject.Instance;
            if (service)
            {
                if(!ClearSubDev(currentDev))
                {
                    service.DeleteDevInfo(currentDev.Info);
                }
                DeleteDevInfo(currentDev,service);
                Close();
                //RemoveFollowObject(currentDev.gameObject);
                RemoveObjectFromSelection(currentDev.gameObject);
                DestroyImmediate(currentDev.gameObject);
                CurrentDevList.Remove(currentDev);
                if (CurrentDevList.Count == 0)
                {
                    ClearSelection();
                }
                else
                {
                    RefleshGizmoPosition();
                }
            }
        }
    }

    /// <summary>
    /// 删除设备信息
    /// </summary>
    /// <param name="dev"></param>
    private void DeleteDevInfo(DevNode dev,CommunicationObject service)
    {
        if(RoomFactory.Instance)
        {
            RoomFactory.Instance.RemoveDevInfo(dev);
        }
        if(CameraSceneManager.Instance)
        {
            if(CameraSceneManager.Instance.alignCamera.target == dev.gameObject.transform)
            {
                FactoryDepManager.currentDep.FocusOn();
            }
        }
        if(TypeCodeHelper.IsLocationDev(dev.Info.TypeCode.ToString()))
        {
            Archor archor = service.GetArchorByDevId(dev.Info.Id);
            if(archor!=null)service.DeleteArchor(archor.Id);
        }
    }
    /// <summary>
    /// 删除子设备（门禁、基站等）
    /// </summary>
    /// <param name="dev"></param>
    /// <returns></returns>
    private bool ClearSubDev(DevNode dev)
    {
        if(dev as DoorAccessDevController)
        {
            DoorAccessDevController controller = dev as DoorAccessDevController;
            List<Dev_DoorAccess> doorAccessList = new List<Dev_DoorAccess>();
            doorAccessList.Add(controller.DoorAccessInfo);
            bool value = CommunicationObject.Instance.DeleteDoorAccess(doorAccessList);
            return true;
        }
        else
        {
            return false;
        }
    }
    /// <summary>
    /// 移除即将删除的设备
    /// </summary>
    /// <param name="dev"></param>
    private void RemoveObjectFromSelection(GameObject dev)
    {
        EditorObjectSelection selection = EditorObjectSelection.Instance;
        if (selection)
        {
            bool result = selection.RemoveObjectFromSelection(dev,false);
            //Debug.Log("Remove from selection :"+result);
        }
    }
    /// <summary>
    /// 清除设备选中
    /// </summary>
    private void ClearSelection()
    {
        EditorObjectSelection selection = EditorObjectSelection.Instance;
        if (selection)
        {
            selection.ClearSelection(false);
        }
    }
    /// <summary>
    /// 移除漂浮UI
    /// </summary>
    /// <param name="obj"></param>
    public void RemoveFollowUI(GameObject obj)
    {
        if (FollowObject.Contains(obj))
        {
            FollowObject.Remove(obj);
            //Debug.Log("Remove Follow UI Success...");
        }
    }
    /// <summary>
    /// 显示定位修改工具
    /// </summary>
    /// <param name="dev"></param>
    private void ShowArchorPart(DevNode dev)
    {
        if(TypeCodeHelper.IsLocationDev(dev.Info.TypeCode.ToString()))
        {
            ArchorToolManage.Instance.Show(dev);
        }
    }
    /// <summary>
    /// 关闭定位测试工具
    /// </summary>
    private void CloseArchorTool()
    {
        if (ArchorToolManage.Instance) ArchorToolManage.Instance.Close();
    }
    #region 漂浮UI部分

    private void SetFollowTarget(List<DevNode> devList)
    {
        //int devCout = devList.Count;
        int followUiCout = FollowObject.Count;
        for (int i = 0; i < devList.Count; i++)
        {
            if (!CurrentDevList.Contains(devList[i])) CurrentDevList.Add(devList[i]);
            if (i < followUiCout)
            {
     
                UGUIFollowTarget followTarget = FollowObject[i].GetComponent<UGUIFollowTarget>();
                followTarget.Target = devList[i].gameObject;
                DevInfoFollowUI followItem = followTarget.GetComponent<DevInfoFollowUI>();
                followItem.Show(devList[i]);
                followTarget.gameObject.SetActive(true);
            }
            else
            {
                GameObject dev = devList[i].gameObject;
                GameObject followTarget = InitFollowTarget(dev);
                FollowObject.Add(followTarget);
                DevInfoFollowUI followItem = followTarget.GetComponent<DevInfoFollowUI>();
                followItem.Show(devList[i]);
                followTarget.gameObject.SetActive(true);
            }
        }
    }
    /// <summary>
    /// 设置跟随UI
    /// </summary>
    /// <param name="dev"></param>
    private void SetFollowTarget(GameObject dev)
    {
        if (_followObject == null)
        {
            _followObject = InitFollowTarget(dev);
        }
        else
        {
            UGUIFollowTarget followTarget = _followObject.GetComponent<UGUIFollowTarget>();
            GameObject targetTagObj = GetTitleObj(dev);
            if (followTarget != null)
            {
                followTarget.Target = targetTagObj;
            }
        }
        if (DevInfo == null)
            DevInfo = _followObject.GetComponent<DevInfoFollowUI>();
    }

    private GameObject InitFollowTarget(GameObject dev)
    {
        if (UGUIFollowManage.Instance == null)
        {
            Debug.LogError("UGUIFollowManage.Instance==null");
            return null;
        }
        GameObject targetTagObj = GetTitleObj(dev);
        Camera mainCamera = GetMainCamera();
        if (mainCamera == null) return null;
        GameObject followItem = UGUIFollowManage.Instance.CreateItem(FollowPrefab, targetTagObj, followName, null, false, true);
        return followItem;
    }
    /// <summary>
    /// 获取设备下的TitleObj
    /// </summary>
    /// <param name="device"></param>
    /// <returns></returns>
    private GameObject GetTitleObj(GameObject device)
    {
        Transform titleObj = device.transform.Find(TitleObjName);
        if (titleObj == null)
        {
            GameObject titleObjTemp = new GameObject();
            titleObjTemp.transform.parent = device.transform;
            float objHeight = device.transform.gameObject.GetSize().y / 2 + 0.2f;
            titleObjTemp.transform.localPosition = new Vector3(0, objHeight, 0);
            return titleObjTemp;
        }
        else
        {
            return titleObj.gameObject;
        }
    }
    /// <summary>
    /// 获取主相机
    /// </summary>
    /// <returns></returns>
    private Camera GetMainCamera()
    {
        CameraSceneManager manager = CameraSceneManager.Instance;
        if (manager && manager.MainCamera != null)
        {
            return manager.MainCamera;
        }
        else
        {
            Debug.LogError("CameraSceneManager.MainCamera is null!");
            return null;
        }
    }
    #endregion
}