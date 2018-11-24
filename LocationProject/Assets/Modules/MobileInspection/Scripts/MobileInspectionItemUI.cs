using Location.WCFServiceReferences.LocationServices;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MobileInspectionItemUI : MonoBehaviour {
    private Toggle itemToggle;//Item按钮
    public Text txtNumber;//编号
    public Text txtPerson;//负责人

    public PersonnelMobileInspection info;//操作票信息

    // Use this for initialization
    void Start()
    {
        itemToggle = GetComponent<Toggle>();
        if (itemToggle != null)
        {
            MobileInspectionUI_N.Instance.ToggleGroupAdd(itemToggle);
            itemToggle.onValueChanged.AddListener(ItemToggle_OnValueChanged);
        }
    }

    // Update is called once per frame
    void Update()
    {

    }

    /// <summary>
    /// 初始化数据
    /// </summary>
    /// <param name="numberStr"></param>
    /// <param name="personStr"></param>
    public void Init(PersonnelMobileInspection infoT)
    {
        info = infoT;
        UpdateData(info.MobileInspectionName, info.PersonnelName);
    }

    /// <summary>
    /// 初始化数据
    /// </summary>
    /// <param name="numberStr"></param>
    /// <param name="personStr"></param>
    public void UpdateData(string numberStr, string personStr)
    {
        txtNumber.text = numberStr;
        txtPerson.text = personStr;
    }

    /// <summary>
    /// Item按钮触发
    /// </summary>
    public void ItemToggle_OnValueChanged(bool ison)
    {
        if (ison)
        {
            print("ItemBtn_OnClick!");
            MobileInspectionDetailsUI.Instance.Show(info);
            MobileInspectionManage.Instance.ShowMobileInspectionPath(info);
            ToggleGroup toggleGroup = MobileInspectionUI_N.Instance.toggleGroup;
            FunctionSwitchBarManage.Instance.SetTransparentToggle(true);
        }
        else
        {
            //TwoTicketSystemManage.Instance.Hide();
            MobileInspectionManage.Instance.Hide();
            FunctionSwitchBarManage.Instance.SetTransparentToggle(false);
            MobileInspectionDetailsUI.Instance.SetWindowActive(false);
        }
    }
}
