using Location.WCFServiceReferences.LocationServices;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MobileInspectionHistoryDetailsUI : MonoBehaviour {

    /// <summary>
    /// 窗体
    /// </summary>
    public GameObject window;

    public PersonnelMobileInspectionHistory info;//工作票信息

    public Text TxtNumber;//编号
                          //T1
    public Text TxtEstimatedStartingTime;//操作任务
    public Text TxtEstimatedEndingTime;//计划时间
    public Text TxtRealStartTime;//计划时间
                               //T2
    public Text TxtRealEndTime;//监护人
    public Text TxtPerson;//操作人


    public GameObject ItemPrefab;//措施单项
    public VerticalLayoutGroup Grid;//措施列表

    public Button closeBtn;//关闭

    // Use this for initialization
    void Start()
    {
        closeBtn.onClick.AddListener(CloseBtn_OnClick);
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void Show(PersonnelMobileInspectionHistory infoT)
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
        //TxtNumber.text = info.MobileInspectionId;
        //TxtOperationTask.text = info.OperationTask;
        //TxtPlanTimeStart.text = info.OperationStartTime.ToString("yyyy/MM/dd HH:mm");
        //TxtPlanTimeEnd.text = info.OperationEndTime.ToString("yyyy/MM/dd HH:mm");
        //TxtGuardian.text = info.Guardian;
        //TxtOperator.text = info.Operator;
        //TxtDutyOfficer.text = info.DutyOfficer;
        //TxtDispatchingOfficer.text = info.Dispatch;

        TxtNumber.text = info.MobileInspectionId.ToString() ;
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

        if (info.EndTime != null)
        {
            DateTime endtime = (DateTime)info.EndTime;
            TxtRealEndTime.text = endtime.ToString("yyyy/MM/dd HH:mm");
        }
        else
        {
            TxtRealEndTime.text = "";
        }

        //DateTime startTime = (DateTime)info.StartTime;
        //TxtRealStartTime.text = startTime.ToString("yyyy/MM/dd HH:mm");
        //TxtRealEndTime.text = "";
    }

    /// <summary>
    /// 创建措施列表
    /// </summary>
    public void CreateMeasuresItems()
    {
        ClearMeasuresItems();
        if (info.list == null || info.list.Length == 0) return;
        foreach (PersonnelMobileInspectionItemHistory sm in info.list)
        {
            GameObject itemT = CreateMeasuresItem();
            Text[] ts = itemT.GetComponentsInChildren<Text>();
            if (ts.Length > 0)
            {
                ts[0].text = sm.nOrder.ToString();
            }
            if (ts.Length > 1)
            {
                ts[1].text = sm.ItemName;
            }
            if (ts.Length > 2)
            {
                ts[2].text = sm.DevName;
            }
            if (ts.Length > 3)
            {
                if (sm.PunchTime != null)
                {
                    DateTime t = (DateTime)sm.PunchTime;
                    ts[3].text = t.ToString();
                }
                else
                {
                    ts[3].text = "";
                }
            }
        }
    }

    /// <summary>
    /// 创建措施项
    /// </summary>
    public GameObject CreateMeasuresItem()
    {
        GameObject itemT = Instantiate(ItemPrefab);
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
        MobileInspectionHistory_N.Instance.SetContentActive(true);
    }
}
