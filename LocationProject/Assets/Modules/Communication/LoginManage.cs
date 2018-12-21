using Location.WCFServiceReferences.LocationServices;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 登录管理脚本
/// </summary>
public class LoginManage : MonoBehaviour {
    public static LoginManage Instance;
    /// <summary>
    /// 当前用户登录相关信息
    /// </summary>
    public LoginInfo info;
    /// <summary>
    /// 登录成功后需要显示的物体
    /// </summary>
    public List<GameObject> LoginShowObjs;
    /// <summary>
    /// 是否登录成功
    /// </summary>
    public bool isLoginSucceed;
    /// <summary>
    /// 是否进行登录成功后初始化
    /// </summary>
    private bool isAfterLoginInit;

    private Action AfterLoginSuccessfullyAction;

    private CommunicationObject communicationObject;

    private string IP;
    private string Port;
    // Use this for initialization
    void Start () {
        Instance = this;
        if (communicationObject == null)
        {
            communicationObject = GetComponent<CommunicationObject>();
        }
	}
	
	// Update is called once per frame
	void Update () {

        AfterLoginInit();

    }

    /// <summary>
    /// 登录测试
    /// </summary>
    [ContextMenu("LoginTest")]
    public void LoginTest()
    {
        Login(communicationObject.ip, communicationObject.port, "Admin", "Admin");
    }

    /// <summary>
    /// 登录
    /// </summary>
    public void Login(string ipT,string portT, string username,string passward)
    {
        info = new LoginInfo();
        info.UserName = username;
        info.Password = passward;
        IP = ipT;
        Port = portT;
        communicationObject.Login(ipT, portT, info,(sender, e)=>{
            if (e.Error == null)
            {
                info = e.Result;
                isLoginSucceed = true;               
                Debug.LogFormat("登录成功！  ip:{0}:{1}  用户:{2}", ipT, portT, info.UserName);            
            }
            else
            {
                isLoginSucceed = false;
                Debug.LogFormat("登录失败！  ip:{0}:{1}  用户:{2}", ipT, portT, info.UserName);
                //AfterLoginFailed();
            }

            isAfterLoginInit = true;

        });

    }

    /// <summary>
    /// 登录之后初始化
    /// </summary>
    public void AfterLoginInit()
    {
        
        if (isAfterLoginInit)
        {
            if (isLoginSucceed)
            {
                //AfterLoginSuccessfully();
                OpenSignalR(IP,Port);
                Invoke("AfterLoginSuccessfully", 2f);//延迟两秒可以使（正在登录...）动画更流畅一点。
            }
            else
            {
                AfterLoginFailed();
            }
            isAfterLoginInit = false;
        }
    }
    /// <summary>
    /// 连接SignalR服务端
    /// </summary>
    /// <param name="ip"></param>
    /// <param name="port"></param>
    private void OpenSignalR(string ip,string port)
    {
        CommunicationCallbackClient clientCallback = CommunicationCallbackClient.Instance;
        if(clientCallback)
        {
            clientCallback.Login(ip,port);
        }
    }
    /// <summary>
    /// 登录成功之后
    /// </summary>
    public void AfterLoginSuccessfully()
    {
        Debug.LogFormat("登录成功后初始化......");
        if (LoginShowObjs != null)
        {
            foreach (GameObject o in LoginShowObjs)
            {
                o.SetActive(true);
            }
        }

        if (AfterLoginSuccessfullyAction != null)
        {
            AfterLoginSuccessfullyAction();
        }
         DianChangLogin.Instance.CloseLogin();
    }

    /// <summary>
    /// 登录失败之后
    /// </summary>
    public void AfterLoginFailed()
    {
        Debug.LogFormat("登录失败后......");
        DianChangLogin.Instance.LoginFail();
    }

    /// <summary>
    /// 退出登录
    /// </summary>
    [ContextMenu("Logout")]
    public void Logout()
    {
        communicationObject.LoginOut(info);
        isLoginSucceed = false;
        Debug.Log("退出登录！");
    }

    /// <summary>
    /// 添加登录成功之后绑定事件
    /// </summary>
    public void AddAfterLoginSuccessfullyAction(Action actionT)
    {
        AfterLoginSuccessfullyAction += actionT;
    }

    /// <summary>
    /// 移除登录成功之后绑定事件
    /// </summary>
    public void RemoveAfterLoginSuccessfullyAction(Action actionT)
    {
        AfterLoginSuccessfullyAction -= actionT;
    }
}
