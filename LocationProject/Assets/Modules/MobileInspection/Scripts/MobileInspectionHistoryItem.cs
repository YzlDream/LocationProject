using Location.WCFServiceReferences.LocationServices;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MobileInspectionHistoryItem : MonoBehaviour {

    public PersonnelMobileInspectionHistory info;//信息

    public Text txtNumber;//编号
    public Text txtRoutingInspectionPersonName;//巡检人员
    public Text txtRoutingInspectionLineName;//签发人
    public Text txtStartTime;//巡检开始时间
    public Text txtEndTime;//巡检结束时间
    public Text txtState;//状态
    //public Text txtWorkPermitPerson;//工作许可人
                                    //public Text txtDetails;//详情
    public Button detailBtn;//详情按钮

    // Use this for initialization
    void Start()
    {
        detailBtn.onClick.AddListener(DetailBtn_OnClick);
    }

    // Update is called once per frame
    void Update()
    {

    }

    /// <summary>
    /// 初始化数据
    /// </summary>
    public void Init(PersonnelMobileInspectionHistory infoT)
    {
        info = infoT;

        txtNumber.text = info.MobileInspectionId.ToString();
        txtRoutingInspectionPersonName.text = info.PersonnelName;
        txtRoutingInspectionLineName.text = info.MobileInspectionName;
        if (info.StartTime != null)
        {
            DateTime timeN = (DateTime)info.StartTime;
            txtStartTime.text = timeN.ToString("yyyy/MM/dd HH:mm");
        }
        else
        {
            txtStartTime.text = "";
        }
        if (info.EndTime != null)
        {
            DateTime timeN = (DateTime)info.EndTime;
            txtEndTime.text = timeN.ToString("yyyy/MM/dd HH:mm");
            txtState.text = "完成";
            txtState.color = Color.white;
        }
        else
        {
            txtEndTime.text = "";
            txtState.text = "<color=#FFA1ED>未完成</color>";
        }
        //txtEndTime.text = info.EndTime.ToString("yyyy/MM/dd HH:mm");
        //txtState.text = info.WorkCondition;
        //txtWorkPermitPerson.text = info.Licensor;
        //txtDetails.text = info.No;
    }

    /// <summary>
    /// 详情按钮事件触发
    /// </summary>
    public void DetailBtn_OnClick()
    {
        Debug.Log("DetailBtn_OnClick!");
        MobileInspectionHistory_N.Instance.ShowOperationTicketHistoryDetailsUI(info);
    }
}
