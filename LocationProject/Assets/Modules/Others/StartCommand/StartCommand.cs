using UnityEngine;
using System.Collections;
using System;
using System.Linq;
using UnityEngine.UI;
using System.Text;

/// <summary>
///Unity3D-获取命令行启动
///用命令打开exe模式,控制脚本
/// </summary>
public class StartCommand : MonoBehaviour {

    public static StartCommand Instance;
    /// <summary>
    /// 是否启用：Unity3D-获取命令行启动
    /// </summary>
    public bool isEnableSetLogin;
    private bool isDoLogin = false;//

    public DianChangLogin login;

    public GameObject StartCommandPanelTest;
    public Text CommandLineText;
    public Text CommandLineArgsText;
    public Text IpText;
    public Text PortText;
    public Text UserNameText;
    public Text Passward;


    private void Awake()
    {
        Instance = this;
    }

    // Use this for initialization
    void Start()
    {
        if (SystemSettingHelper.systemSetting.IsDebug)
        {
            SetTestPanel(true);
        }
        else
        {
            SetTestPanel(false);
        }
        //初始化通信
        StartCommandRun();
   
    }

    public void SetTestPanel(bool isactive)
    {
        StartCommandPanelTest.SetActive(isactive);
    }

    /// <summary>
    /// 获取命令行相关参数
    /// </summary>
    public void StartCommandRun()
    {
        string CommandLine = Environment.CommandLine;//获取命令行
        string[] CommandLineArgs = Environment.GetCommandLineArgs();//获取命令相关参数
        string text1 = "CommandLine : " + CommandLine;
        string text2 = CommandLineArgs.Aggregate<string, string>(
            "CommandLineArgs : ",
            (a, b) => a + "|" + b);
        CommandLineText.text = text1;
        CommandLineArgsText.text = text2;
        string[] abs = text2.Split('|');
        if (abs.Length > 2)
        {
            string str = abs[2];
            string s = "locationsystem:";
            if (str.Contains("locationsystem:"))
            {
                str = str.Remove(0, s.Length);
            }

            IpText.text = str;

        }
        else
        {
            if (isEnableSetLogin)
            {
                IpText.text = "1";
                //IpText.text = "127.0.0.1%7C8733%7Cadmin%7Cadmin";
            }
        }
        if (abs.Length > 3)
        {
            PortText.text = abs[3];
        }
        else
        {
            if (isEnableSetLogin)
            {
                PortText.text = "2";
            }
        }

        if (abs.Length > 4)
        {
            UserNameText.text = abs[4];

        }
        else
        {
            if (isEnableSetLogin)
            {
                UserNameText.text = "";
            }
        }

        if (abs.Length > 5)
        {
            Passward.text = abs[5];
        }
        else
        {
            if (isEnableSetLogin)
            {
                Passward.text = "";
            }
        }

        if (isEnableSetLogin)
        {
            login.SetInfo(IpText.text.Trim(), PortText.text.Trim(), UserNameText.text.Trim(), Passward.text.Trim());
        }
    }


    // Update is called once per frame
    void Update()
    {
        if (isEnableSetLogin && !isDoLogin)
        {
            isDoLogin = true;
            login.LoginBtn_Trigger();
        }
    }

}
