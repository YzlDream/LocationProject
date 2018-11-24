using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DevAlarmType : MonoBehaviour {
    public static DevAlarmType instance;
    public Dropdown DevTypedropdownItem;

    List<string> tempNames;
    void Start()
    {
        instance = this;
        DevTypedropdownItem = GetComponent<Dropdown>();
        tempNames = new List<string>();
        AddName();
        SetDropdownData(tempNames);
        DevTypedropdownItem.onValueChanged.AddListener(DevAlarmListManage.Instance.ScrScreenDevAlarmType);
    }
   
    /// <summary>
    /// 设置数据
    /// </summary>
    /// <param name="showName"></param>
	private void SetDropdownData(List<string> showName)
    {
        DevTypedropdownItem.options.Clear();
        Dropdown.OptionData tempData;
        for (int i = 0; i < showName.Count; i++)
        {
            tempData = new Dropdown.OptionData();
            tempData.text = showName[i];
            DevTypedropdownItem.options.Add(tempData);
        }
        DevTypedropdownItem.captionText.text = showName[0];
    }
    public void ShowDropdownFirstData()
    {
        DevTypedropdownItem.captionText.text = "告警类型";
    }
    /// <summary>
    /// 添加名字
    /// </summary>
	public void AddName()
    {
        string n0 = "告警类型";
        string n1 = "区域告警";
        string n2 = "消失告警";
        string n3 = "低电告警";
        string n4 = "传感器告警";
        string n5 = "重启告警";
        string n6 = "非法拆卸";
        string n7 = "其他告警";
        tempNames.Add(n0);
        tempNames.Add(n1);
        tempNames.Add(n2);
        tempNames.Add(n3);
        tempNames.Add(n4);
        tempNames.Add(n5);
        tempNames.Add(n6);
        tempNames.Add(n7);
       
    }
}
