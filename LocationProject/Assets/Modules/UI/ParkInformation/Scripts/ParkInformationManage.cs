using Location.WCFServiceReferences.LocationServices;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ParkInformationManage : MonoBehaviour
{
    public static ParkInformationManage Instance;
    public GameObject ParkInfoUI;//园区信息统计界面
    public GameObject InfoType;//统计数据类型
    public GameObject BaseInfo;//底部信息栏
    public GameObject ArrowDot;//箭头中间的点
    public GameObject ArrowUp;//上箭头
    public GameObject ArrowDown;//朝下箭头
    public Toggle ArrowTog;
    public int CurrentNode;
    /// <summary>
    /// 统计类型
    /// </summary>
    public Text TitleText;
    /// <summary>
    /// 人员总数
    /// </summary>
    public Text PersonnelNumText;
    /// <summary>
    /// 设备总数
    /// </summary>
    public Text DevNumText;
    /// <summary>
    /// 定位告警总数
    /// </summary>
    public Text PosAlarmText;
    /// <summary>
    /// 设备告警总数
    /// </summary>
    public Text DevAlarmText;
    bool isRefresh;
    Color ArrowDotColor = new Color(255 / 255f, 255 / 255f, 255 / 255f, 102 / 255f);
    public AreaStatistics AreaInfo;
    void Start()
    {

        Instance = this;
        SceneEvents.OnDepCreateComplete += OnRoomCreateComplete;
        SceneEvents.FullViewStateChange += OnFullViewStateChange;
        ArrowTog.onValueChanged.AddListener(OnArrowTogChange);
        ChangeArrowDotColor();

    }
    public void OnFullViewStateChange(bool b)
    {
        if (b)
        {
            ShowParkInfoUI(false);
        }
        else
        {

            DepNode dep = FactoryDepManager.Instance;
            //ParkInfoUI.SetActive(true);
            TitleText.text = dep.NodeName.ToString();
            RefreshParkInfo(dep.NodeID);
        }
    }

    public void OnRoomCreateComplete(DepNode dep)
    {

        FullViewController mainPage = FullViewController.Instance;
        if (mainPage && mainPage.IsFullView)
        {
            ShowParkInfoUI(false);
            return;
        }
        else
        {
            if (PersonSubsystemManage.Instance.IsOnBenchmarking == false && PersonSubsystemManage.Instance.IsOnEditArea == false && DevSubsystemManage.Instance.isDevEdit == false&& PersonSubsystemManage.Instance.IsHistorical == false)
            {
                TitleText.text = dep.NodeName.ToString();
                //GetParkDataInfo(dep.NodeID);
                RefreshParkInfo(dep.NodeID);
            }

        }

    }
    public void RefreshParkInfo(int dep)
    {
        CurrentNode = dep;
        if (!IsInvoking("StartRefreshData"))
        {
            //注释：用于崩溃测试；
            InvokeRepeating("StartRefreshData", 0, 1f);//todo:定时获取
            //Invoke("StartRefreshData", 0);
        }
        //    GetParkDataInfo(CurrentNode);
    }
    public void StartRefreshData()
    {
        GetParkDataInfo(CurrentNode);
        Debug.Log("园区统计刷新");
    }

    public void GetParkDataInfo(int dep)
    {
        if (isRefresh) return;
        isRefresh = true;
        ParkInfoUI.SetActive(true);
        ThreadManager.Run(() =>
        {
            AreaInfo = CommunicationObject.Instance.GetAreaStatistics(dep);
        }, () =>
         {
             PersonnelNumText.text = AreaInfo.PersonNum.ToString();
             DevNumText.text = AreaInfo.DevNum.ToString();
             PosAlarmText.text = AreaInfo.LocationAlarmNum.ToString();
             DevAlarmText.text = AreaInfo.DevAlarmNum.ToString();
             isRefresh = false;
         }, "GetParkDataInfo");

       
        

    }



    public void OnArrowTogChange(bool isOn)
    {
        if (ArrowTog.isOn = isOn)
        {
            ArrowTog.GetComponent<Image>().color = Color.white;
            InfoType.SetActive(true);
            BaseInfo.SetActive(true);
            ArrowUp.SetActive(false);
            ArrowDown.SetActive(false);
        }
        else
        {
            ArrowTog.GetComponent<Image>().color = new Color(255 / 255f, 255 / 255f, 255 / 255f, 51 / 255f);
            InfoType.SetActive(false);
            BaseInfo.SetActive(false);
            ArrowUp.SetActive(false);
            ArrowDown.SetActive(true);
        }
    }
    public void ChangeArrowDotColor()
    {
        EventTriggerListener ArrowDotColor = EventTriggerListener.Get(ArrowTog.gameObject);
        ArrowDotColor.onEnter = Arrow_Up;
        ArrowDotColor.onExit = Arrow_Exit;


    }
    /// <summary>
    /// 鼠标放上后
    /// </summary>
    /// <param name="ArrowDot"></param>
	public void Arrow_Up(GameObject image)
    {
        ArrowDot.GetComponent<Image>().color = Color.white;
        ArrowUp.GetComponent<Image>().color = Color.white;
        ArrowDown.GetComponent<Image>().color = Color.white;
    }
    /// <summary>
    /// 鼠标离开
    /// </summary>
    /// <param name="ArrowDot"></param>
    public void Arrow_Exit(GameObject image)
    {
        ArrowDot.GetComponent<Image>().color = ArrowDotColor;
        ArrowUp.GetComponent<Image>().color = ArrowDotColor;
        ArrowDown.GetComponent<Image>().color = ArrowDotColor;
    }
    void Update()
    {

    }
    /// <summary>
    /// 是否打开园区统计信息界面
    /// </summary>
    /// <param name="isOn"></param>
    public void ShowParkInfoUI(bool isOn)
    {

        if (isOn)
        {
            FullViewController mainPage = FullViewController.Instance;
            if (mainPage && mainPage.IsFullView)
            {
                if (IsInvoking("StartRefreshData"))
                {
                    CancelInvoke("StartRefreshData");

                }
                ParkInfoUI.SetActive(false);
                return;
            }
            else
            {
                ParkInfoUI.SetActive(true);
                if (!IsInvoking("StartRefreshData"))
                {
                    //注释：用于崩溃测试；
                    InvokeRepeating("StartRefreshData", 0, 1f);//todo:定时获取
                    //Invoke("StartRefreshData", 0);
                }
            }

        }
        else
        {
            if (IsInvoking("StartRefreshData"))
            {
                CancelInvoke("StartRefreshData");
            }
            ParkInfoUI.SetActive(false);
        }
    }



}
