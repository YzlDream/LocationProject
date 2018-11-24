using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Location.WCFServiceReferences.LocationServices;
public class FollowTargetManage : MonoBehaviour
{
    public static FollowTargetManage Instance;

    /// <summary>
    /// 建筑信息，UI预设
    /// </summary>
    public GameObject BuildingNameUIPrefab;
    /// <summary>
    /// 建筑UI集合名称
    /// </summary>
    [HideInInspector]
    public string BuildingListName = "BuildingNameUI";

    /// <summary>
    /// 摄像头漂浮UI 预设
    /// </summary>
    public GameObject CameraUIPrefab;
    /// <summary>
    /// 摄像头,漂浮UI所属组名
    /// </summary>
    [HideInInspector]
    private string CameraListName = "CameraNameUI";
    /// <summary>
    /// 摄像头，所在区域名称
    /// </summary>
    private List<string> CameraDepNameList = new List<string>();


    /// <summary>
    /// 设备信息漂浮UI
    /// </summary>
    public GameObject DevUIPrefab;
    /// <summary>
    /// 设备信息，漂浮UI所属组名
    /// </summary>
    private string DevListName = "DevNameUI";
    /// <summary>
    /// 设备所属区域
    /// </summary>
    private List<string> DevDepNameList = new List<string>();

    /// <summary>
    /// UI跟随管理脚本
    /// </summary>
    private UGUIFollowManage UIFollowManager;

    // Use this for initialization
    void Awake()
    {
        Instance = this;
    }
    void Start()
    {
        SceneEvents.FullViewStateChange += OnFullViewChange;
        SceneEvents.DepNodeChanged += OnDepTypeChange;
    }

    /// <summary>
    /// 进入/退出首页
    /// </summary>
    /// <param name="isFullView"></param>
    private void OnFullViewChange(bool isFullView)
    {
        if (UIFollowManager == null)
        {
            UIFollowManager = UGUIFollowManage.Instance;
            if (UIFollowManager == null)
            {
                Debug.LogError("UGUIFollowManage.Instance is null.");
                return;
            }
        }
        if (isFullView)
        {
            HideBuildingUI();
            HideDevInfoUI();
            HideCameraUI();
        }
        else
        {
            ShowBuidingUI();
            ShowDevInfoUI();
            ShowCameraUI();
        }
    }
    private void OnDepTypeChange(DepNode oldDep, DepNode currentDep)
    {
        if (UIFollowManager == null)
        {
            UIFollowManager = UGUIFollowManage.Instance;
            if (UIFollowManager == null)
            {
                Debug.LogError("UGUIFollowManage.Instance is null.");
                return;
            }
        }
        if (currentDep as FactoryDepManager && !FullViewController.Instance.IsFullView)
        {
            ShowBuidingUI();
        }
        else
        {
            HideBuildingUI();
        }
        HideCameraUI(oldDep);
        ShowCameraUI(currentDep);
        
        HideDevInfoUI(oldDep);
        ShowDevInfoUI(currentDep);
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
    #region 建筑信息
    /// <summary>
    /// 显示建筑漂浮信息
    /// </summary>
    public void ShowBuidingUI()
    {
        DepNode node = FactoryDepManager.currentDep;
        if (!FunctionSwitchBarManage.Instance.BuildingToggle.ison|| node==null||node as BuildingController||node as FloorController||node as RoomController) return;
        UIFollowManager.SetGroupUIbyName(BuildingListName, true);
    }
    /// <summary>
    /// 隐藏建筑漂浮信息
    /// </summary>
    public void HideBuildingUI()
    {
        UIFollowManager.SetGroupUIbyName(BuildingListName, false);
    }
    #endregion
    #region 摄像头创建UI部分

    public void CreateCameraUI(GameObject cameraDev,DepNode devDep,DevNode info)
    {
        GameObject targetTagObj = UGUIFollowTarget.CreateTitleTag(cameraDev, Vector3.zero);
        if (UGUIFollowManage.Instance == null)
        {
            Debug.LogError("UGUIFollowManage.Instance==null");
            return;
        }
        Camera mainCamera = GetMainCamera();
        if (mainCamera == null) return;
        string cameraDepName = GetDepNodeId(devDep) + CameraListName;
        if (!CameraDepNameList.Contains(cameraDepName)) CameraDepNameList.Add(cameraDepName);
        GameObject name = UGUIFollowManage.Instance.CreateItem(CameraUIPrefab, targetTagObj, cameraDepName, mainCamera);
        CameraMonitorFollowUI cameraInfo = name.GetComponent<CameraMonitorFollowUI>();
        if (cameraInfo != null)
        {
            cameraInfo.SetInfo(info);
        }
        if (DevSubsystemManage.IsRoamState || !FunctionSwitchBarManage.Instance.CameraToggle.ison)
        {
            UGUIFollowManage.Instance.SetGroupUIbyName(cameraDepName, false);
        }
    }
   
    /// <summary>
    /// 隐藏摄像机漂浮UI
    /// </summary>
    public void HideCameraUI(DepNode dep=null)
    {
        if (FactoryDepManager.currentDep == null) return;
        DepNode depToShow = dep == null ? FactoryDepManager.currentDep : dep;
        string cameraDepName = GetDepNodeId(depToShow) + CameraListName;
        UGUIFollowManage.Instance.SetGroupUIbyName(cameraDepName, false);
    }
    /// <summary>
    /// 显示摄像机漂浮UI
    /// </summary>
    public void ShowCameraUI(DepNode dep = null)
    {
        if (!FunctionSwitchBarManage.Instance.CameraToggle.ison||FactoryDepManager.currentDep==null) return;
        DepNode depToShow = dep == null ? FactoryDepManager.currentDep : dep;
        string cameraDepName = GetDepNodeId(depToShow) + CameraListName;
        UGUIFollowManage.Instance.SetGroupUIbyName(cameraDepName, true);
    }
    #endregion

    #region 创建设备漂浮UI
    /// <summary>
    /// 创建设备漂浮UI
    /// </summary>
    /// <param name="sisDev"></param>
    /// <param name="info"></param>
    /// <param name="isShow">是否显示</param>
    public void CreateDevFollowUI(GameObject sisDev,DepNode devDep, DevNode info)
    {
        GameObject targetTagObj = UGUIFollowTarget.CreateTitleTag(sisDev, Vector3.zero);
        if (UGUIFollowManage.Instance == null)
        {
            Debug.LogError("UGUIFollowManage.Instance==null");
            return;
        }
        Camera mainCamera = GetMainCamera();
        if (mainCamera == null) return;
        string devDepName = GetDepNodeId(devDep) + DevListName;
        if (!DevDepNameList.Contains(devDepName)) DevDepNameList.Add(devDepName);
        GameObject name = UGUIFollowManage.Instance.CreateItem(DevUIPrefab, targetTagObj, devDepName, mainCamera);
        UGUIFollowTarget followTarget = name.GetComponent<UGUIFollowTarget>();
        followTarget.SetEnableDistace(true,60);
        DeviceFollowUI cameraInfo = name.GetComponent<DeviceFollowUI>();
        if (cameraInfo != null)
        {
            cameraInfo.SetInfo(info);
        }
        if (DevSubsystemManage.IsRoamState||!FunctionSwitchBarManage.Instance.DevInfoToggle.ison)
        {
            UGUIFollowManage.Instance.SetGroupUIbyName(devDepName, false);
        }        
    }
    /// <summary>
    /// 隐藏设备漂浮UI
    /// </summary>
    public void HideDevInfoUI(DepNode dep=null)
    {
        if (FactoryDepManager.currentDep == null) return;
        DepNode depToShow = dep == null ? FactoryDepManager.currentDep : dep;
        string devDepName = GetDepNodeId(depToShow) + DevListName;
        UGUIFollowManage.Instance.SetGroupUIbyName(devDepName, false);
    }
    /// <summary>
    /// 显示设备漂浮UI
    /// </summary>
    public void ShowDevInfoUI(DepNode dep=null)
    {
        if (!FunctionSwitchBarManage.Instance.DevInfoToggle.ison|| FactoryDepManager.currentDep == null) return;
        DepNode depToShow = dep == null ? FactoryDepManager.currentDep : dep;
        string devDepName = GetDepNodeId(depToShow) + DevListName;
        UGUIFollowManage.Instance.SetGroupUIbyName(devDepName, true);
    }
    #endregion

    /// <summary>
    /// 获取区域Id
    /// </summary>
    /// <param name="dep"></param>
    /// <returns></returns>
    private string GetDepNodeId(DepNode dep)
    {
        if (dep as RoomController) return dep.ParentNode.NodeID.ToString();
        return dep.NodeID.ToString();
    }
    void OnDestroy()
    {
        SceneEvents.DepNodeChanged -= OnDepTypeChange;
        SceneEvents.FullViewStateChange -= OnFullViewChange;
    }
}
