
using Location.WCFServiceReferences.LocationServices;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class PersonnelAlarmList : MonoBehaviour
{
    public static PersonnelAlarmList Instance;
    /// <summary>
    /// 每页显示的条数
    /// </summary>
    public int pageLine = 10;
    /// <summary>
    /// 数据
    /// </summary>
    private int StartPageNum = 0;
    /// <summary>
    /// 页数
    /// </summary>
    private int PageNum = 1;
    /// <summary>
    /// 总页数
    /// </summary>
    public Text pegeTotalText;

    public InputField pegeNumText;//输入选中页
    public Text promptText;
    public Button AddPageBut;
    public Button MinusPageBut;
    /// <summary>
    /// 行的模板
    /// </summary>
    public GameObject TemplateInformation;
    /// 存放预设生成的集合
    /// </summary>
    public GridLayoutGroup grid;
    /// <summary>
    /// 10条数据存放的列表
    /// </summary>
    public List<LocationAlarm> newPerAlarmList;
    /// <summary>
    /// 人员告警界面
    /// </summary>
    public GameObject personAlarmUI;
    public Sprite DoubleImage;
    public Sprite OddImage;

    public Button CloseBut;//关闭
    public Button SearchBut;
    public Text StartTimeText;
    public Text DealTimeText;
    public InputField InputPerAlarm;//输入筛选内容
    public CalendarChange StartcalendarDay;
    public CalendarChange DealcalendarDay;
    private string num;
    private string nameT;
    private string job;
    private string content;
    private string startTime;
    private string endTime;
    private string PerID;

    public AlarmSearchArg perAlarmData;
    public LocationAlarm[] perAlarmInfo;
    public List<LocationAlarm> PerAlarmList;
    List<LocationAlarm> ScreenAlarmTime;
    List<LocationAlarm> ScreenAlarmItem;
    public StartTime showStartTime;
    void Start()
    {
        Instance = this;
        perAlarmData = new AlarmSearchArg();
        ScreenAlarmTime = new List<LocationAlarm>();
        LoadData();
        //StartPerAlarmUI();
        AddPageBut.onClick.AddListener(AddPerAlarmPage);
        MinusPageBut.onClick.AddListener(MinPerAlarmPage);
        CloseBut.onClick.AddListener(ClosePersonAlarmUI);
        StartcalendarDay.onDayClick.AddListener(ScreeningStartTimeAlaim);
        DealcalendarDay.onDayClick.AddListener(ScreeningSecondTimeAlarm);
        SearchBut.onClick.AddListener(PerAlarmSearchBut_Click);
        pegeNumText.onValueChanged.AddListener(InputPersonnelPage);
        SearchBut.onClick.AddListener(SetPersonnelAlarm_Click);
    }

    private void LoadData()
    {
        var personnelAlarm = CommunicationObject.Instance.GetLocationAlarms(perAlarmData);
        PerAlarmList = new List<LocationAlarm>(personnelAlarm);
        ScreenAlarmItem = new List<LocationAlarm>(personnelAlarm); ;
    }

    /// <summary>
    /// 打开人员告警界面
    /// </summary>
    public void StartPerAlarmUI()
    {
        LoadData();

        StartPageNum = 0;
        PageNum = 1;
        GetPersonnelAlarmPage(PerAlarmList);
        TotaiLine(PerAlarmList);
        pegeNumText.text = "1";
        InputPerAlarm.text = "";
        DateTime CurrentTime = System.DateTime.Now;
        string currenttime = CurrentTime.ToString("yyyy年MM月dd日");
        StartTimeText.text = currenttime;
        DealTimeText.text = currenttime;
        promptText.gameObject.SetActive(false);
        showStartTime.ShowStartTime();

}
/// <summary>
/// 生成多少页
/// </summary>
public void TotaiLine(List<LocationAlarm> date)
    {

        if (date.Count % pageLine == 0)
        {
            pegeTotalText.text = (date.Count / pageLine).ToString();
        }
        else
        {
            pegeTotalText.text = Convert.ToString(Math.Ceiling((double)date.Count / (double)pageLine));
        }
    }
    /// <summary>
    /// 上一页信息
    /// </summary>
    public void AddPerAlarmPage()
    {
        StartPageNum += 1;
        double  a= Math.Ceiling((double)ScreenAlarmItem.Count / (double)pageLine);
        int m = (int)a;
        if (StartPageNum <= m)
        {
            PageNum += 1;
            pegeNumText.text = PageNum.ToString();
            GetPersonnelAlarmPage(ScreenAlarmItem);
        }
    }
    public void MinPerAlarmPage()
    {
        if (StartPageNum > 0)
        {
            StartPageNum--;
            PageNum -= 1;
            if (PageNum == 0)
            {
                pegeNumText.text = "1";
            }
           else
            {
                pegeNumText.text = PageNum.ToString();
            }
            GetPersonnelAlarmPage(ScreenAlarmItem);
        }
    }
    public void SetPersonnelAlarm_Click()
    {
        StartPageNum = 0;
        PageNum = 1;
    
        ScreenAlarmItem.Clear();
        string key = InputPerAlarm.text.ToString();
        SaveSelection();
        for (int i=0;i < PerAlarmList.Count;i++)
        {   
                string AlarmName = PerAlarmList[i].Personnel.Name.ToString();
                string AlarmNum = PerAlarmList[i].Personnel.WorkNumber.ToString();
                if (AlarmName.Contains(key) || AlarmNum.Contains(key))
                {
                    ScreenAlarmItem.Add(PerAlarmList[i]);
                }
            }
        if (ScreenAlarmItem.Count==0)
        {
            promptText.gameObject.SetActive(true);
            pegeNumText.text = "1";
            pegeTotalText.text = "1";
        }
        else
        {
            promptText.gameObject.SetActive(false );
            TotaiLine(ScreenAlarmItem);
            GetPersonnelAlarmPage(ScreenAlarmItem);
        }
       
       
    }
    /// <summary>
    /// 输入搜索页
    /// </summary>
    /// <param name="value"></param>
    public void InputPersonnelPage(string value)
    {
        int currentPage = int.Parse(pegeNumText.text );
        int MaxPage = (int)Math.Ceiling((double)ScreenAlarmItem.Count / (double)pageLine);
        if (currentPage> MaxPage)
        {
            currentPage = MaxPage;
            pegeNumText.text = currentPage.ToString();
        }
        if (currentPage <= 0)
        {
            currentPage = 1;
            pegeNumText.text = currentPage.ToString ();
        }
        StartPageNum = currentPage - 1;
        PageNum = currentPage;
        GetPersonnelAlarmPage(ScreenAlarmItem);
    }
    /// <summary>
    /// 得到人员告警数据
    /// </summary>
    public void GetPersonnelAlarmData()
    {
        for (int i = 0; i < newPerAlarmList.Count; i++)
        {
            if (newPerAlarmList[i].Personnel == null)
            {
                num = "";
                nameT = "";
                job = "";
            }
            else
            {
                num = newPerAlarmList[i].Personnel.WorkNumber.ToString();
                nameT = newPerAlarmList[i].Personnel.Name.ToString();
                job = newPerAlarmList[i].Personnel.Pst.ToString();
            }
           
            
            
           
            
            content = newPerAlarmList[i].Content;
            string startTime1 = newPerAlarmList[i].CreateTime.ToString();
            if (startTime1 == "1/1/0001 12:00:00 AM")
            {
                startTime = "";
            }
            else
            {
                DateTime NewTime = Convert.ToDateTime(startTime1);
                startTime = NewTime.ToString("yyyy年MM月dd日 HH:mm:ss");
            }

            string endTime1 = newPerAlarmList[i].HandleTime.ToString();
            if (endTime1 == "1/1/0001 12:00:00 AM")
            {
                endTime = "";
            }
            else
            {
                DateTime NewTime = Convert.ToDateTime(endTime1);
                endTime = NewTime.ToString("yyyy年MM月dd日 HH:mm:ss");
            }
            PerID = newPerAlarmList[i].TagId.ToString();
            SetInstantiateLine(newPerAlarmList.Count);
            SetPersonnelAlarmData(i, num, nameT, job, content, startTime, endTime, PerID);
        }
    }
    /// <summary>
    /// 生成的页数
    /// </summary>
    public void GetPersonnelAlarmPage(List<LocationAlarm> data)
    {
        newPerAlarmList.Clear();
        if (StartPageNum * pageLine < data.Count)
        {
            var QueryData = data.Skip(pageLine * StartPageNum).Take(pageLine);
            foreach (var devAlarm in QueryData)
            {
                newPerAlarmList.Add(devAlarm);
            }
            GetPersonnelAlarmData();
        }
    }
    public void SetPersonnelAlarmData(int i, string num, string name, string job, string content, string startTime, string endTime, string PerID)
    {
        Transform line = grid.transform.GetChild(i);
        line.GetChild(0).GetComponent<Text>().text = num;
        line.GetChild(1).GetComponent<Text>().text = name;
        line.GetChild(2).GetComponent<Text>().text = job;
        line.GetChild(3).GetComponent<Text>().text = content;
        line.GetChild(4).GetComponent<Text>().text = startTime;
        line.GetChild(5).GetComponent<Text>().text = endTime;
        Button but = line.GetChild(6).GetChild(0).GetComponent<Button>();
        but.onClick.RemoveAllListeners();
        but.onClick.AddListener(() =>
  {
      PerAlarmBut_Click(PerID);
  });
        if (i % 2 == 0)
        {
            line.GetComponent<Image>().sprite = DoubleImage;
        }
        else
        {
            line.GetComponent<Image>().sprite = OddImage;
        }
    }
    /// <summary>
    /// 人员订位
    /// </summary>
    /// <param name="tagNum"></param>
    public void PerAlarmBut_Click(string tagNum)
    {
        ParkInformationManage.Instance.ShowParkInfoUI(false );
        int tagID = int.Parse(tagNum);
        LocationManager.Instance.FocusPersonAndShowInfo(tagID);
        PersonSubsystemManage.Instance.ChangeImage(false, PersonSubsystemManage.Instance.PersonnelAlamToggle);
        PersonSubsystemManage.Instance.PersonnelAlamToggle.isOn = false;
        personAlarmUI.SetActive(false);

    }
    /// <summary>
    /// 每一行的预设
    /// </summary>
    /// <param name="portList"></param>
    public void InstantiateLine()
    {
        GameObject o = Instantiate(TemplateInformation);
        o.SetActive(true);
        o.transform.parent = grid.transform;
        o.transform.localScale = Vector3.one;
        o.transform.localPosition = new Vector3(o.transform.localPosition.x, o.transform.localPosition.y, 0);
    }
    /// <summary>
    /// 生成多少预设
    /// </summary>
    /// <param name="num"></param>
    public void SetInstantiateLine(int num)
    {
        if (grid.transform.childCount < num)
        {
            InstantiateLine();
        }
        else
        {
            for (int j = grid.transform.childCount - 1; j > num; j--)
            {
                DestroyImmediate(grid.transform.GetChild(j).gameObject);
            }
        }
    }
    /// <summary>
    /// 人员告警界面
    /// </summary>
    /// <param name="b"></param>
    public void ShowPersonAlarmUI()
    {
        personAlarmUI.SetActive(true);
        StartPerAlarmUI();
    }
    /// <summary>
    /// 关闭人员告警界面
    /// </summary>
    public void ClosePersonAlarmUI()
    {
        personAlarmUI.SetActive(false);
        PersonSubsystemManage.Instance.ChangeImage(false, PersonSubsystemManage.Instance.PersonnelAlamToggle);
        PersonSubsystemManage.Instance.PersonnelAlamToggle.isOn = false;
    }
    //List<LocationAlarm> ScreenAlarmTime = new List<LocationAlarm>();
    /// <summary>
    /// 第二个筛选时间
    /// </summary>
    /// <param name="dateTime"></param>
    public void ScreeningSecondTimeAlarm(DateTime dateTime)
    {
        SaveSelection();
        string StartTime = StartTimeText.GetComponent<Text>().text;
        DateTime NewStartTime = Convert.ToDateTime(StartTime);
        DateTime NewDealTime = Convert.ToDateTime(dateTime);
        for (int i = 0; i < PerAlarmList.Count; i++)
        {
            DateTime AlarmTime = PerAlarmList[i].CreateTime;
            if (DateTime.Compare(NewStartTime, NewDealTime) >= 0)
            {
                if (DateTime.Compare(NewStartTime, AlarmTime) >= 0 && DateTime.Compare(NewDealTime, AlarmTime) <= 0)
                {
                    ScreenAlarmTime.Add(PerAlarmList[i]);
                }
            }
            else
            {
                if (DateTime.Compare(NewStartTime, AlarmTime) <= 0 && DateTime.Compare(NewDealTime, AlarmTime) >= 0)
                {
                    ScreenAlarmItem.Add(PerAlarmList[i]);
                }
            }

        }
        GetPersonnelAlarmPage(ScreenAlarmItem);
        ScreenAlarmTime.Clear();
    }
    /// <summary>
    ///告警时间筛选
    /// </summary>
    public void ScreeningStartTimeAlaim(DateTime dateTime)
    {
        SaveSelection();
        string DealTime = DealTimeText.GetComponent<Text>().text;
        DateTime NewStartTime = Convert.ToDateTime(dateTime);
        DateTime NewEndTime = Convert.ToDateTime(DealTime);
        for (int i = 0; i < PerAlarmList.Count; i++)
        {
            DateTime AlarmTime = PerAlarmList[i].CreateTime;
            if (DateTime.Compare(NewStartTime, NewEndTime) >= 0)
            {

                if (DateTime.Compare(NewStartTime, AlarmTime) >= 0 && DateTime.Compare(NewEndTime, AlarmTime) <= 0)
                {
                    ScreenAlarmItem.Add(PerAlarmList[i]);
                }
            }
            else
            {

                if (DateTime.Compare(NewStartTime, AlarmTime) <= 0 && DateTime.Compare(NewEndTime, AlarmTime) >= 0)
                {
                    ScreenAlarmItem.Add(PerAlarmList[i]);
                }
            }
        }
        GetPersonnelAlarmPage(ScreenAlarmTime);
        ScreenAlarmTime.Clear();
    }
    /// <summary>
    /// 保留选中项
    /// </summary>
    public void SaveSelection()
    {
        for (int j = grid.transform.childCount - 1; j >= 0; j--)
        {
            DestroyImmediate(grid.transform.GetChild(j).gameObject);
        }
    }

    //List<LocationAlarm> SeachPerName = new List<LocationAlarm>();

    /// <summary>
    /// 搜索人员
    /// </summary>
    public void PerAlarmSearchBut_Click()
    {
        List<LocationAlarm> SeachPerName = new List<LocationAlarm>();
        SeachPerName.Clear();
        string key = InputPerAlarm.text.ToString();
        for (int i = 0; i < PerAlarmList.Count; i++)
        {
            if (key == PerAlarmList[i].Id.ToString() || key == PerAlarmList[i].TypeName)
            {
                SaveSelection();

                SeachPerName.Add(PerAlarmList[i]);

            }
        }
        GetPersonnelAlarmPage(SeachPerName);
    }
}
