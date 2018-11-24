using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Location.WCFServiceReferences.LocationServices;
public class CameraMonitorFollowUI : MonoBehaviour
{
    /// <summary>
    /// 当前展开弹窗的UI
    /// </summary>
    public static CameraMonitorFollowUI CurrentMonitor;

    //public Sprite NormalSprite;
    //public Sprite HoverSprite;

    //public Sprite AlarmNormalSprite;
    //public Sprite AlarmHoverSprite;

    //public Sprite NormalBg;
    //public Sprite AlarmBg;

    /// <summary>
    /// 视频监控按钮
    /// </summary>
    public Button VideoMonitorButton;
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
    /// <summary>
    /// 摄像头信息
    /// </summary>
    private DevNode CameraDev;
    /// <summary>
    /// 是否显示
    /// </summary>
    private bool isShow;
	// Use this for initialization
	void Start ()
	{
	    VideoMonitorButton.onClick.AddListener(ShowMonitor);
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
        if (CurrentMonitor != null&&CurrentMonitor!=this) CurrentMonitor.Hide();
        CurrentMonitor = this;
        isShow = true;
        CameraDev.HighlightOn();
        InfoBg.SetActive(true);
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
    public void SetInfo(DevNode devNode)
    {
        CameraDev = devNode;
        DevInfo devInfo = CameraDev.Info;
        TitleText.text = devInfo.Name;
        string info="";
        if (devInfo.ParentId != null)
        {
            DepNode node = RoomFactory.Instance.GetDepNodeById((int)devInfo.ParentId);
            if (node != null) info = node.NodeName + "/";
        }
        info += devInfo.KKSCode;
        InfoText.text = info;
    }
    /// <summary>
    /// 显示监控信息
    /// </summary>
    private void ShowMonitor()
    {
        if (CameraDev != null)
        {
            CameraVideoManage.Instance.ShowVideo(CameraDev.Info.KKSCode);
        }
        else
        {
            //Todo:提示错误信息
            Debug.LogError("VideoMonitor devInfo is null...");
        }
    }
}
