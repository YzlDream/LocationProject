using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Location.WCFServiceReferences.LocationServices;

public class DeviceFollowUI : MonoBehaviour {
    /// <summary>
    /// 当前展开弹窗的UI
    /// </summary>
    public static DeviceFollowUI CurrentMonitor;

    /// <summary>
    /// 视频监控按钮
    /// </summary>
    public Button DevInfoButton;
    /// <summary>
    /// 打开界面按钮
    /// </summary>
    public Button BgButton;

    /// <summary>
    /// 弹窗
    /// </summary>
    public GameObject InfoBg;
    /// <summary>
    /// 标题文本
    /// </summary>
    public Text TitleText;
    /// <summary>
    /// 信息文本
    /// </summary>
    public Text InfoText;

    private DevNode devNode;
    /// <summary>
    /// 是否显示
    /// </summary>
    private bool isShow;

    /// <summary>
    /// 温度信息模块
    /// </summary>
    public DeviceTemperatureInfo TemperatureInfo;
    // Use this for initialization
    void Start()
    {
        DevInfoButton.onClick.AddListener(ShowMonitor);
        BgButton.onClick.AddListener(ShowBg);

    }
    /// <summary>
    /// 显示/关闭 窗体
    /// </summary>
    private void ShowBg()
    {
        if (isShow) Hide();
        else Show();
    }
    /// <summary>
    /// 显示背景
    /// </summary>
    public void Show()
    {
        if (CurrentMonitor != null && CurrentMonitor != this) CurrentMonitor.Hide();
        CurrentMonitor = this;
        devNode.HighlightOn();
        isShow = true;
        InfoBg.SetActive(true);
        if (TitleText.text != devNode.Info.Name) TitleText.text = devNode.Info.Name;
    }
    /// <summary>
    /// 关闭背景
    /// </summary>
    public void Hide()
    {
        isShow = false;
        CurrentMonitor = null;
        InfoBg.SetActive(false);
    }
    /// <summary>
    /// 设置信息
    /// </summary>
    /// <param name="devInfo"></param>
    public void SetInfo(DevNode _devInfo)
    {
        devNode = _devInfo;
        DevInfo dev = _devInfo.Info;
        TitleText.text = dev.Name;
        string info = "";
        if (dev.ParentId != null&&RoomFactory.Instance)
        {
            DepNode node = RoomFactory.Instance.GetDepNodeById((int)dev.ParentId);
            //if (node != null) info = node.NodeName+ "/";
            if (node != null) info = node.NodeName;
        }
        //info += devInfo.KKSCode;
        if(string.IsNullOrEmpty(info)) info = "四会热电厂";
        InfoText.text = info;
        TemperatureInfo.Init();
    }
    /// <summary>
    /// 显示监控信息
    /// </summary>
    private void ShowMonitor()
    {
        FacilityInfoManage manager = FacilityInfoManage.Instance;
        if (manager)
        {
            manager.Show(devNode.Info);
        }
    }
}
