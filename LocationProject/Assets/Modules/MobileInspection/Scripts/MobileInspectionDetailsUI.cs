using Location.WCFServiceReferences.LocationServices;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MobileInspectionDetailsUI : MonoBehaviour {
    public static MobileInspectionDetailsUI Instance;
    /// <summary>
    /// 窗体
    /// </summary>
    public GameObject window;

    public PersonnelMobileInspection info;//工作票信息

    public Text TxtNumber;//编号
                          //T1
    public Text TxtEstimatedStartingTime;//预计开始时间
    public Text TxtEstimatedEndingTime;//预计结束时间
                                       //T2
    public Text TxtRealStartTime;//实际开始时间
    public Text TxtRealEndTime;//实际结束时间
    //public Text TxtDutyOfficer;//值班负责人
    //public Text TxtDispatchingOfficer;//调度负责人

    public GameObject itemPrefab;//单项预设
    public VerticalLayoutGroup Grid;//列表


    // Use this for initialization
    void Start()
    {
        Instance = this;
        //closeBtn.onClick.AddListener(CloseBtn_OnClick);
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void Show(PersonnelMobileInspection infoT)
    {
        info = infoT;
        UpdateData();
        CreateMeasuresItems();
        SetWindowActive(true);
    }

    /// <summary>
    /// 刷新数据
    /// </summary>
    public void UpdateData()
    {
        TxtNumber.text = info.MobileInspectionId + "(" + info.PersonnelName + ")";
        TxtEstimatedStartingTime.text = info.PlanStartTime.ToString("yyyy/MM/dd HH:mm");
        TxtEstimatedEndingTime.text = info.PlanEndTime.ToString("yyyy/MM/dd HH:mm");

        if (info.StartTime != null)
        {
            DateTime startTime = (DateTime)info.StartTime;
            TxtRealStartTime.text = startTime.ToString("yyyy/MM/dd HH:mm");
        }
        else
        {
            TxtRealStartTime.text = "";
        }

        //DateTime startTime = (DateTime)info.StartTime;
        //TxtRealStartTime.text = startTime.ToString("yyyy/MM/dd HH:mm");
        TxtRealEndTime.text = "";
    }

    /// <summary>
    /// 创建措施列表
    /// </summary>
    public void CreateMeasuresItems()
    {
        ClearMeasuresItems();
        if (info.list == null || info.list.Length == 0) return;
        foreach (PersonnelMobileInspectionItem sm in info.list)
        {
            GameObject itemT = CreateMeasuresItem();
            Text[] ts = itemT.GetComponentsInChildren<Text>();
            if (ts.Length > 0)
            {
                ts[0].text = sm.nOrder.ToString();
            }
            if (ts.Length > 1)
            {

                if (sm.PunchTime != null)
                {
                    DateTime punchTime = (DateTime)sm.PunchTime;
                    ts[1].text = punchTime.ToString("yyyy/MM/dd HH:mm");
                }
                else
                {
                    ts[1].text = "";
                }
            }
        }
    }

    /// <summary>
    /// 创建措施项
    /// </summary>
    public GameObject CreateMeasuresItem()
    {
        GameObject itemT = Instantiate(itemPrefab);
        itemT.transform.SetParent(Grid.transform);
        itemT.transform.localPosition = Vector3.zero;
        itemT.transform.localScale = Vector3.one;
        LayoutElement layoutElement = itemT.GetComponent<LayoutElement>();
        layoutElement.ignoreLayout = false;
        itemT.SetActive(true);
        return itemT;
    }

    /// <summary>
    /// 清空措施列表
    /// </summary>
    public void ClearMeasuresItems()
    {
        int childCount = Grid.transform.childCount;
        for (int i = childCount - 1; i >= 0; i--)
        {
            DestroyImmediate(Grid.transform.GetChild(i).gameObject);
        }
    }

    /// <summary>
    /// 是否显示传统
    /// </summary>
    public void SetWindowActive(bool isActive)
    {
        window.SetActive(isActive);
    }

    /// <summary>
    /// 关闭按钮
    /// </summary>
    public void CloseBtn_OnClick()
    {
        SetWindowActive(false);
    }
}
