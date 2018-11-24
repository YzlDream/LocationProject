using UnityEngine;
using System.Collections;
//using U3D.DataAccess.DAs;
using Location.WCFServiceReferences.LocationServices;
using System.Collections.Generic;
using RTEditor;
/// <summary>
/// 物体添加列表管理
/// </summary>
public class ObjectAddListManage : MonoBehaviour {

    public static ObjectAddListManage Instance;
    /// <summary>
    /// 是否在设备编辑状态
    /// </summary>
    public static bool IsEditMode;
    /// <summary>
    /// 窗口界面
    /// </summary>
    public GameObject window;
    /// <summary>
    /// 物体添加列表信息
    /// </summary>
    public List<ObjectAddList_Type> info;


    // Use this for initialization
    void Start () {
        Instance = this;
        //InitObjectAddList();
    }
	
	// Update is called once per frame
	void Update () {
	
	}

    private void OnEnable()
    {
        //InitObjectAddList();
    }

    /// <summary>
    /// 初始化物体添加列表
    /// </summary>
    public void InitObjectAddList()
    {
        ThreadManager.Run(() =>
        {
            info = GetObjectAddListInfo();

        }, () =>
        {
            //if (RoomState.IsObjectMode)
            //{
            //    ObjectListToolbar.Instance.SetObjectToolbar(false);
            //}
            //else
            //{
            //    ObjectListToolbar.Instance.SetObjectToolbar(true);
            //}
            ObjectListToolbar.Instance.SetObjectToolbar(true);

            ObjectListToolbar.Instance.powerDeviceToggle.isOn = true;
            ObjectListToolbar.Instance.PowerDeviceToggle_ValueChanged(true);
        }, "获取设备数据");
    }

    /// <summary>
    /// 获取物体添加列表的信息
    /// </summary>
    public List<ObjectAddList_Type> GetObjectAddListInfo()
    {
        CommunicationObject access = CommunicationObject.Instance;
        List<ObjectAddList_Type> modelList =access.GetModelAddList();
        return modelList;
    }

    /// <summary>
    /// 根据大类名称获取大类相关信息
    /// </summary>
    /// <returns></returns>
    public ObjectAddList_Type GetObjectAddListTypeInfoByName(string name)
    {
        ObjectAddList_Type objectAddList_Type = info.Find((item) => item.typeName == name);
        return objectAddList_Type;
    }

    public void Show()
    {
        window.SetActive(true);
        SetEditMode(true);
        Reflesh();
    }

    public void Hide()
    {
        window.SetActive(false);
        SetEditMode(false);
    }
    /// <summary>
    /// 设置编辑模式
    /// </summary>
    /// <param name="isOn">是否编辑模式</param>
    private void SetEditMode(bool isOn)
    {
        EditorObjectSelection objectEditor = EditorObjectSelection.Instance;
        if (isOn)
        {
            IsEditMode = true;
            
        }
        else
        {
            IsEditMode = false;
            //EditorObjectSelection objectEditor = EditorObjectSelection.Instance;
            if (objectEditor)
            {
                if(DeviceEditUIManager.Instacne)DeviceEditUIManager.Instacne.HideMultiDev();
                objectEditor.ClearSelection(false);
            }
        }
    }
    /// <summary>
    /// 刷新列表
    /// </summary>
    public void Reflesh()
    {
        if (window.activeInHierarchy)
        {
            InitObjectAddList();
        }
    }
}
