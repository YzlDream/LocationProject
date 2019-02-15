using UnityEngine;
using System.Collections;
using Base.Common;
using UnityEngine.UI;
using System;
using System.IO;

public class SystemSettingHelper : MonoBehaviour
{
    [HideInInspector]
    public static SystemSetting systemSetting;//所有系统设置
    [HideInInspector]
    public static CinemachineSetting cinemachineSetting;
    public static CommunicationSetting communicationSetting;//通信相关设置
    public static VersionSetting versionSetting;//版本号设置
    /// <summary>
    /// 版本号
    /// </summary>
    [HideInInspector]
    public static string versionNum = "1.0.20";

    public static SystemSettingHelper instance;

    void Awake()
    {
        GetSystemSetting();
        instance = this;
    }

    // Use this for initialization
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    /// <summary>
    /// 获取系统设置
    /// </summary>
    public static void GetSystemSetting()
    {
        string path = Application.dataPath + "\\..\\SystemSetting.XML";
        if (!File.Exists(path))
        {
            CreateSystemSettingXml();
        }
        else
        {
            systemSetting = SerializeHelper.DeserializeFromFile<SystemSetting>(path);
        }

        cinemachineSetting = systemSetting.cinemachineSetting;
        communicationSetting = systemSetting.communicationSetting;
        versionSetting = systemSetting.versionSetting;
    }

    /// <summary>
    /// 保存系统设置
    /// </summary>
    public static void SaveSystemSetting()
    {
        string path = Application.dataPath + "\\..\\SystemSetting.XML";

        SerializeHelper.Save(systemSetting, path);

    }

    public static void CreateSystemSettingXml()
    {
        systemSetting = new SystemSetting();
        systemSetting.versionSetting.VersionNumber = versionNum;
        SaveSystemSetting();
    }

}
