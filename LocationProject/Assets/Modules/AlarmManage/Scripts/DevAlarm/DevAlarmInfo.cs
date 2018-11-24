using HighlightingSystem;
using Location.WCFServiceReferences.LocationServices;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DevAlarmInfo : MonoBehaviour {

    /// <summary>
    /// 告警信息
    /// </summary>
    public DeviceAlarm AlarmInfo;
    /// <summary>
    /// 闪烁频率
    /// </summary>
    private float frequency = 2f;
	// Use this for initialization
	void Start () {
		
	}
    /// <summary>
    /// 初始化告警信息
    /// </summary>
    /// <param name="dev"></param>
    /// <param name="alarmContent"></param>
    public void InitAlarmInfo(DeviceAlarm alarmContent)
    {
        AlarmInfo = alarmContent;
    }
    /// <summary>
    /// 开始告警
    /// </summary>
	public void AlarmOn()
    {
        if (!DevAlarmManage.IsShowAlarm) return;
        Highlighter h = gameObject.AddMissingComponent<Highlighter>();
        Color colorStart = Color.red;
        Color colorEnd = new Color(colorStart.r, colorStart.g, colorStart.b, 0);
        h.FlashingOn(colorStart, colorEnd, frequency);
    }
    /// <summary>
    /// 结束告警
    /// </summary>
    public void AlarmOff()
    {
        Highlighter h = gameObject.AddMissingComponent<Highlighter>();
        h.FlashingOff();
    }
}
