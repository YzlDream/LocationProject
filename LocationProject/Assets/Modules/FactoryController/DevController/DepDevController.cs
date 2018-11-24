using Location.WCFServiceReferences.LocationServices;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mogoson.CameraExtension;
public class DepDevController : DevNode
{
	// Use this for initialization
	public override void Start ()
	{
	    base.Start();
        DoubleClickEventTrigger_u3d trigger = DoubleClickEventTrigger_u3d.Get(gameObject);
        trigger.onClick = OnClick;
        trigger.onDoubleClick = OnDoubleClick;
    }

    private void OnClick()
    {
        if (DevSubsystemManage.IsRoamState) return;
        HighlightOn();
        Debug.Log("Click  ID: " + Info.Id+" DevID: "+Info.DevID);
    }
    private void OnDoubleClick()
    {
        if (DevSubsystemManage.IsRoamState) return;
        FocusDev();
    }
    /// <summary>
    /// 聚焦/取消聚焦
    /// </summary>
    private void FocusDev()
    {
        if (!IsFocus) FocusOn();
        else FocusOff();
    }
    
}
