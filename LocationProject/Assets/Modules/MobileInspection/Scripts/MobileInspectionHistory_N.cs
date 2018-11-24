using Location.WCFServiceReferences.LocationServices;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MobileInspectionHistory_N : MonoBehaviour {

    //enum State
    //{
    //    工作票,
    //    操作票,
    //}
    //State curentState = State.工作票;

    public static MobileInspectionHistory_N Instance;

    public GameObject window;
    //public Dropdown titleDropdown;//标题选择下拉列表框
    public Text txtTitle;//标题文本

    public CalendarChange calendarStart;//开始时间
    public CalendarChange calendarEnd;//开始时间
    private DateTime startTime;//起始时间
    private DateTime endTime;//结束时间

    public Button closeBtn;//关闭Button

    [HideInInspector]
    public List<PersonnelMobileInspectionHistory> mobileInspectionHistoryList;//工作票历史记录
    //[HideInInspector]
    //public List<OperationTicketHistory> operationTicketHistoryList;//操作票历史记录

    public MobileInspectionHistoryGrid mobileInspectionHistoryGrid;//工作票历史列表
    //public OperationTicketHistoryGrid operationTicketHistoryGrid;//操作票历史列表

    public MobileInspectionHistoryDetailsUI mobileInspectionHistoryDetailsUI;//工作票历史项详情界面
    //public OperationTicketHistoryDetailsUI operationTicketHistoryDetailsUI;//操作票历史项详情界面

    // Use this for initialization
    void Start()
    {
        Instance = this;
        //titleDropdown.onValueChanged.AddListener(TitleDropdown_OnValueChanged);
        searchInput.onEndEdit.AddListener(SearchInput_OnEndEdit);
        searchInput.onValueChanged.AddListener(SearchInput_OnValueChanged);
        searchBtn.onClick.AddListener(SearchBtn_OnClick);
        calendarStart.onDayClick.AddListener(CalendarStart_onDayClick);
        calendarEnd.onDayClick.AddListener(CalendarEnd_onDayClick);
        closeBtn.onClick.AddListener(CloseBtn_OnClick);

        SetCalendarOpenAndClose();
    }

    /// <summary>
    /// 日历的打开和关闭相关设置
    /// </summary>
    private void SetCalendarOpenAndClose()
    {
        calendarStart.transform.parent.Find("PickButton").GetComponent<Button>().onClick.AddListener(() =>
        {
            if (calendarEnd.gameObject.activeInHierarchy)
            {
                calendarEnd.gameObject.SetActive(false);
            }
        });

        calendarEnd.transform.parent.Find("PickButton").GetComponent<Button>().onClick.AddListener(() =>
        {
            if (calendarStart.gameObject.activeInHierarchy)
            {
                calendarStart.gameObject.SetActive(false);
            }
        });
    }

    // Update is called once per frame
    void Update()
    {

    }

    /// <summary>
    /// SetContentActive
    /// </summary>
    /// <param name="isActive"></param>
    public void SetContentActive(bool isActive)
    {
        window.SetActive(isActive);
    }

    /// <summary>
    /// 标题下拉列表框
    /// </summary>
    public void TitleDropdown_OnValueChanged(int i)
    {
        //txtTitle.text = titleDropdown.options[i].text;
        //ChangeState((State)i);
        Show();
    }

    public InputField searchInput;//搜索关键字输入框   
    public Button searchBtn;//搜索按钮
    private List<WorkTicketHistory> workTicketHistorySearchList;//当前搜索出来的工作票历史记录
    private List<OperationItemHistory> operationItemHistorySearchList;//当前搜索出来的操作票历史记录

    /// <summary>
    /// 显示
    /// </summary>
    public void Show()
    {
        SetContentActive(true);
        searchInput.text = "";
        Loom.StartSingleThread((System.Threading.ThreadStart)(() =>
        {
            //if (curentState == State.工作票)
            //{
                GetWorkTicketHistoryData();
                Loom.DispatchToMainThread((Frankfort.Threading.ThreadDispatchDelegate)(() =>
                {
                    CreateWorkTicketGrid();
                }));
            //}
            //else if (curentState == State.操作票)
            //{
            //    GetOperationTicketHistoryData();
            //    Loom.DispatchToMainThread(() =>
            //    {
            //        CreateOperationTicketGrid();
            //    });
            //}

        }));
    }

    ///// <summary>
    ///// 改变状态
    ///// </summary>
    //private void ChangeState(State stateT)
    //{
    //    curentState = stateT;
    //}

    /// <summary>
    /// 加载工作票历史数据
    /// </summary>
    public void GetWorkTicketHistoryData()
    {
        //后期获取数据加上时间
        mobileInspectionHistoryList = CommunicationObject.Instance.GetPersonnelMobileInspectionHistoryList();
    }

    ///// <summary>
    ///// 加载工作票历史数据
    ///// </summary>
    //public void GetOperationTicketHistoryData()
    //{
    //    //后期获取数据加上时间
    //    operationTicketHistoryList = CommunicationObject.Instance.GetOperationTicketHistoryList();
    //}

    /// <summary>
    /// 创建工作票列表
    /// </summary>
    public void CreateWorkTicketGrid()
    {
        //operationTicketHistoryGrid.gameObject.SetActive(false);
        mobileInspectionHistoryGrid.gameObject.SetActive(true);
        mobileInspectionHistoryGrid.Search();
    }

    ///// <summary>
    ///// 创建操作票列表
    ///// </summary>
    //public void CreateOperationTicketGrid()
    //{
    //    mobileInspectionHistoryGrid.gameObject.SetActive(false);
    //    operationTicketHistoryGrid.gameObject.SetActive(true);
    //    operationTicketHistoryGrid.gameObject.SetActive(true);
    //    operationTicketHistoryGrid.Search();
    //}

    /// <summary>
    /// 搜索
    /// </summary>
    public void Search()
    {
        //if (curentState == State.工作票)
        //{
            //WorkTicketHistorySearch();
            CreateWorkTicketGrid();
        //}
        //else if (curentState == State.操作票)
        //{
        //    //OperationItemHistorySearch();
        //    CreateOperationTicketGrid();
        //}
    }

    /// <summary>
    /// 搜索框编辑结束触发事件
    /// </summary>
    /// <param name="txt"></param>
    public void SearchInput_OnEndEdit(string txt)
    {
        if (Input.GetKeyDown(KeyCode.Return))
        {
            Debug.Log("SearchInput_OnEndEdit!");
            //currentPageNum = 0;
            Search();
        }

    }

    /// <summary>
    /// 搜索框改变触发事件
    /// </summary>
    /// <param name="txt"></param>
    public void SearchInput_OnValueChanged(string txt)
    {
        Debug.Log("SearchInput_OnValueChanged!");
    }

    /// <summary>
    /// 搜索按钮触发事件
    /// </summary>
    public void SearchBtn_OnClick()
    {
        Debug.Log("SearchBtn_OnClick!");
        Search();
    }


    public void CalendarStart_onDayClick(DateTime dateTimeT)
    {
        //DateTime endtime = Convert.ToDateTime("2018年8月10日");//8/10/2018 12:00:00 AM,就是10日早上0点
        startTime = dateTimeT;
        Search();
    }
    public void CalendarEnd_onDayClick(DateTime dateTimeT)
    {
        endTime = dateTimeT.AddHours(24);
        Search();
    }

    /// <summary>
    /// 关闭
    /// </summary>
    public void CloseBtn_OnClick()
    {
        //SetContentActive(false);
        MobileInspectionSubBar.Instance.SetHistoryToggle(false);
    }

    ///// <summary>
    ///// 展示工作票历史项详情
    ///// </summary>
    //public void ShowWorkTicketHistoryDetailsUI(WorkTicketHistory workTicketHistoryT)
    //{
    //    SetContentActive(false);
    //    //workTicketHistoryDetailsUI.Show(workTicketHistoryT);
    //}

    /// <summary>
    /// 展示操作票历史项详情
    /// </summary>
    public void ShowOperationTicketHistoryDetailsUI(PersonnelMobileInspectionHistory personnelMobileInspectionHistoryT)
    {
        SetContentActive(false);
        //operationTicketHistoryDetailsUI.Show(operationTicketHistoryT);
        mobileInspectionHistoryDetailsUI.Show(personnelMobileInspectionHistoryT);
    }
}

