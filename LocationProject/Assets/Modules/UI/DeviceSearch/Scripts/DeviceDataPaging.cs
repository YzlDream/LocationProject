using Location.WCFServiceReferences.LocationServices;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class DeviceDataPaging : MonoBehaviour {
    public static DeviceDataPaging Instance;
    /// <summary>
    /// 每页显示的条数
    /// </summary>
    public int pageSize = 10;
    /// <summary>
    /// 数据
    /// </summary>
   private  int StartPageNum = 0;
    /// <summary>
    /// 页数
    /// </summary>
    private int PageNum = 1;
    /// <summary>
    /// 总页数
    /// </summary>
    public Text pageTotalText;
    /// <summary>
    /// 输入页数
    /// </summary>
    public InputField pageNumText;
    /// <summary>
    /// 下一页
    /// </summary>
    public Button AddPageBut;
    /// <summary>
    /// 上一页
    /// </summary>
    public Button MinusPageBut;
    /// <summary>
    /// 输入设备名称
    /// </summary>
    public InputField InputDev;
    /// <summary>
    /// 输入设备名称后，确定按钮
    /// </summary>
    public Button DevBut;
    /// <summary>
    /// 没有筛选时切页按钮
    /// </summary>
    public GameObject TotalPage;
    /// <summary>
    /// 关闭界面按钮
    /// </summary>
    public Button closeButton;
    /// <summary>
    /// 设备搜素界面
    /// </summary>
    public GameObject devSearchUI;

    public Text promptText;
    private string devType;
    private string devName;
    private string devRoute;
    private string devIP;
    private string devID;
    private int  depID;
    /// <summary>
    /// 行的模板
    /// </summary>
    public GameObject TemplateInformation;
    /// 存放预设生成的集合
    /// </summary>
    public GridLayoutGroup grid;
    /// <summary>
    /// 分页后数据存放
    /// </summary>
    List<DevInfo> NewPagingData = new List<DevInfo>();
    public List<DevInfo> devSearch;
    public List<DevInfo> SeachDevName;
    public Sprite DoubleImage;
    public Sprite OddImage;
    void Start () {
        Instance = this;
        devSearch = new List<DevInfo>(CommunicationObject.Instance.GetAllDevInfos());
        SeachDevName = devSearch;
        TotaiLine(devSearch);
        GetPageDevData(devSearch);
        AddPageBut.onClick.AddListener(AddDevPage);
        MinusPageBut.onClick.AddListener( MinusDevPage );
        pageNumText.onValueChanged.AddListener(InputDevPage);
        DevBut.onClick.AddListener(InputDevName);
        closeButton.onClick.AddListener(ClosedevSearchWindow);
        pageNumText.text = "1";
        StartPageNum = 0;
    }
    /// <summary>
    /// 刚打开时的界面 
    /// </summary>
    public void StartDevSeachUI()
    {
        SeachDevName = new List<DevInfo>(CommunicationObject.Instance.GetAllDevInfos());
        TotaiLine(devSearch);
        pageNumText.text = "1";
        PageNum = 1;
        StartPageNum = 0;
        InputDev.text = "";
        GetPageDevData(devSearch);
        ShowTotalPage(true);
        //  SearchDevInfo.Instance.SearchPageing.SetActive(false);
        promptText.gameObject.SetActive(false);


    }
    /// <summary>
    /// zongye
    /// </summary>
    /// <param name="DevData"></param>
    public void TotaiLine(List<DevInfo> devNum)
    {
        if (devNum.Count % pageSize == 0)
        {
            pageTotalText.text = (devNum.Count / pageSize).ToString();
        }
        else
        {
            pageTotalText.text = Convert.ToString(Math.Ceiling((double)devNum.Count / (double)pageSize));
        }
    }
    /// <summary>
    /// 获取设备数据
    /// </summary>
	public void GetDevData(List<DevInfo> dev)
    {
        for (int i=0;i < NewPagingData.Count;i ++)
        {
            devType = NewPagingData[i].TypeName.ToString();
            devName = NewPagingData[i].Name.ToString();
            devRoute = NewPagingData[i].Path.ToString();
            devIP = NewPagingData[i].IP.ToString();
            devID = NewPagingData[i].DevID.ToString();
            depID = (int)NewPagingData[i].ParentId;
            SetInstantiateLine(NewPagingData.Count);
            SetDevData(i, devType, devName, devRoute, devIP, devID,depID );
        }
    }
    /// <summary>
    /// 获取每页的数据
    /// </summary>
    public void GetPageDevData(List<DevInfo>  DevData)
    {
        NewPagingData.Clear();
        if (StartPageNum * pageSize < DevData.Count)
        {
            var QueryData = DevData.Skip(pageSize * StartPageNum).Take(pageSize);
            foreach (var dev in QueryData)
            {
                NewPagingData.Add(dev);
            }
          
            GetDevData(NewPagingData);
        }
        
    }
    
        /// <summary>
        /// 生成多少行预设
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
            for (int j = grid.transform.childCount - 1; j >= num; j--)
            {
                DestroyImmediate(grid.transform.GetChild(j).gameObject);
            }
        }
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
    /// 每条信息赋值
    /// </summary>
    /// <param name="i"></param>
   
    public void SetDevData(int i, string devType, string devName, string devRoute, string devIP, string devID,int depID)
    {
        Transform line = grid.transform.GetChild(i);
        line.GetChild(0).GetComponent<Text>().text = devType;
        line.GetChild(1).GetComponent<Text>().text = devName;
        line.GetChild(2).GetComponent<Text>().text = devRoute;
        line.GetChild(3).GetComponent<Text>().text = devIP;
        Button but = line.GetChild(4).GetChild(0).GetComponent<Button>();
        but.onClick.RemoveAllListeners();
        but.onClick.AddListener(() =>
        {
            DevBut_Click(devID, depID);

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
    public void AddDevPage()
    {
        StartPageNum += 1;
        if (StartPageNum <= SeachDevName.Count /pageSize)
        {
            PageNum += 1;
            pageNumText.text = PageNum.ToString();
            GetPageDevData(SeachDevName);
        }else
        {
            StartPageNum -= 1;
        }
    }
    /// <summary>
    /// 下一页信息
    /// </summary>
    public void MinusDevPage()
    {
        if (StartPageNum > 0)
        {
            StartPageNum--;
            PageNum -= 1;
            if (PageNum == 0)
            {
                pageNumText.text = "1";
            }
           else
            {
                pageNumText.text = PageNum.ToString();
            }
            GetPageDevData(SeachDevName);
        }
    }
    /// <summary>
    /// 输入页数
    /// </summary>
    /// <param name="value"></param>
    public void InputDevPage(string value)
    {
        int currentPage = int.Parse(pageNumText.text);
        int  maxPage = (int )Math.Ceiling((double)SeachDevName.Count / (double)pageSize);

        if (currentPage >maxPage)
        {
            currentPage = maxPage;
            pageNumText.text = currentPage.ToString();
        }
        if (currentPage <= 0)
        {
            currentPage = 1;
            pageNumText.text = currentPage.ToString ();
        }
        StartPageNum = currentPage - 1;
        PageNum = currentPage;
        GetPageDevData(SeachDevName);
    }
    /// <summary>
    /// 输入搜索名字
    /// </summary>
    public void InputDevName()
    {
      
        StartPageNum = 0;
        PageNum = 1;
        pageNumText.text = "1";
        SeachDevName.Clear();
        SaveSelection();
        string key = InputDev.text.ToString();
        for (int i=0;i< devSearch.Count; i++)
        {
            string devName = devSearch[i].Name;
            string devIP = devSearch[i].IP;
            if (devName.Contains (key )|| devIP.Contains (key ))
            {
              
                SeachDevName.Add(devSearch[i]);
              
            }
           
        }
        if (SeachDevName.Count == 0)
        {
            pageNumText.text = "1";
            pageTotalText.text = "1";
            promptText.gameObject.SetActive(true);
        }
        else
        {
            promptText.gameObject.SetActive(false );
            TotaiLine(SeachDevName);
            GetPageDevData(SeachDevName);
        }
      
    }
   
    /// <summary>
    /// 点击定位设备
    /// </summary>
    /// <param name="devId"></param>
    public void DevBut_Click(string devId,int DepID)
    {
       
        DevSubsystemManage.Instance.QueryToggle.isOn = false;
        RoomFactory.Instance.FocusDev(devId,DepID);   
      //  DeviceSearchTween.instance.ShowMinWindow(true);
        devSearchUI.SetActive(false);
        //  SearchDevInfo.Instance.SaveSelection();
        // SearchDevInfo.Instance.ExitSearchUI();
      

       // DevSubsystemManage.Instance.QueryToggle.isOn = false;
    }
    /// <summary>
    /// 没有筛选时的切页列表
    /// </summary>
    /// <param name="b"></param>
    public void ShowTotalPage(bool b)
    {
        if (b)
        {
            TotalPage.SetActive(true);
        }
        else
        {
            TotalPage.SetActive(false);
        }
    }
 
    /// <summary>
    /// 打开设备搜索界面
    /// </summary>
    public void ShowdevSearchWindow()
    {
        devSearchUI.SetActive(true);
      
    }
    /// <summary>
    /// 关闭设备搜索界面
    /// </summary>
    public void ClosedevSearchWindow()
    {
      
        devSearchUI.SetActive(false);
        DevSubsystemManage.Instance.ChangeImage(false, DevSubsystemManage.Instance.QueryToggle);
        DevSubsystemManage.Instance.QueryToggle.isOn = false;
       // SearchDevInfo.Instance.SaveSelection();
      //  SearchDevInfo.Instance.ExitSearchUI();
       // DeviceSearchTween.instance.ShowMinWindow(false);
    }
    /// <summary>
    /// 保留选中项
    /// </summary>
    public void SaveSelection()
    {
        for (int j = grid.transform.childCount - 1; j >=0; j--)
        {
            DestroyImmediate(grid.transform.GetChild(j).gameObject);
        }
    }
    
      
    
}
