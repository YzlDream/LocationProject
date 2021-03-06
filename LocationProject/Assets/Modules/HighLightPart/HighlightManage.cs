﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HighlightManage : MonoBehaviour
{
    public static HighlightManage Instance;
	// Use this for initialization
	void Start ()
	{
	    Instance = this;
	}
	
	// Update is called once per frame
	void Update () {
		
	}
    /// <summary>
    /// 取消区域和设备高亮
    /// </summary>
    public void CancelHighLight()
    {
        HighLightDevOff();
        HighLightDepOff();
        DepNameUI.Instance.Close();
    }
    #region DevHighlight
    /// <summary>
    /// 当前高亮设备
    /// </summary>
    private DevNode currentHighlightDev;
    /// <summary>
    /// 设置高亮设备
    /// </summary>
    /// <param name="highLightDev"></param>
    public void SetHightLightDev(DevNode highLightDev)
    {
        HighLightDevOff(highLightDev);
        currentHighlightDev = highLightDev;
    }
    /// <summary>
    /// 取消设备高亮
    /// </summary>
    public void HighLightDevOff(DevNode highLightDev=null)
    {
        if (currentHighlightDev != null&&highLightDev!=currentHighlightDev)
        {
            currentHighlightDev.HighLightOff();
        }
    }

    #endregion
    #region DepHighLight
    /// <summary>
    /// 当前区域设备
    /// </summary>
    private DepNode currentHighlightDep;
    /// <summary>
    /// 设置区域设备
    /// </summary>
    /// <param name="highLightDep"></param>
    public void SetHightLightDep(DepNode highLightDep)
    {
        HighLightDepOff(highLightDep);
        currentHighlightDep = highLightDep;
    }
    /// <summary>
    /// 取消区域高亮
    /// </summary>
    public void HighLightDepOff(DepNode highLightDep = null)
    {
        if (currentHighlightDep != null && highLightDep != currentHighlightDep)
        {
            currentHighlightDep.HighLightOff();
        }
    }

    #endregion
}
