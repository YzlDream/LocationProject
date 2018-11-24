using Assets.M_Plugins.Helpers.Utils;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class DevInfoFollowUI : MonoBehaviour
{
   
    /// <summary>
    /// 设备名称按钮
    /// </summary>
    public Text DevName;
    /// <summary>
    /// 删除设备按钮
    /// </summary>
    public Button DeleteButton;
    /// <summary>
    /// 设备编辑管理脚本
    /// </summary>
    private DeviceEditUIManager Manager;
    /// <summary>
    /// 当前设备
    /// </summary>
    public DevNode currentDev;
	// Use this for initialization
	void Start ()
	{
	    DeleteButton.onClick.AddListener(DeleteDev);
	}

    public void Show(DevNode dev)
    {
        InitManager();
        currentDev = dev;
        ShowDeleteButton(dev);
        //Window.SetActive(true);
        if (TypeCodeHelper.IsStaticDev(dev.Info.TypeCode.ToString()))
        {
            DevName.text = dev.Info != null ? dev.Info.Name+"   " : "";
        }
        else
        {
            DevName.text = dev.Info != null ? dev.Info.Name : "";
        }
    }

    private void InitManager()
    {
        if (Manager == null) Manager = DeviceEditUIManager.Instacne;
    }
    /// <summary>
    /// 是否显示删除按钮
    /// </summary>
    /// <param name="dev"></param>
    private void ShowDeleteButton(DevNode dev)
    {
        if (dev == null || dev.Info == null) return;
        if(TypeCodeHelper.IsStaticDev(dev.Info.TypeCode.ToString()))
        {
            DeleteButton.gameObject.SetActive(false);
        }
        else
        {
            DeleteButton.gameObject.SetActive(true);
        }
    }
    /// <summary>
    /// 删除设备
    /// </summary>
    private void DeleteDev()
    {
        if (Manager != null)
        {
            Manager.DeleteDev(currentDev);
        }
        else
        {
            Debug.LogError("DeviceEditUIManager.Instance is null...");
        }
    }

    void OnDestroy()
    {
        DeviceEditUIManager manager = DeviceEditUIManager.Instacne;
        if (manager)
        {
            manager.RemoveFollowUI(gameObject);
        }
    }
}
