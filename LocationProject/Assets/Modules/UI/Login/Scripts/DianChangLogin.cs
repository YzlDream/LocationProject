using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class DianChangLogin : MonoBehaviour
{
    public static DianChangLogin Instance;

    public string ip= "127.0.0.1";
    public string port = "8733";
    public string username = "Admin";
    public string password = "Admin";

    public InputField IpInput;
    public InputField PortInput;
    public InputField NameInput;
    public InputField PasswordInput;
    /// <summary>
    /// 点击登陆
    /// </summary>
    public Button LoginBut;
    public GameObject LoginWindow;


    public GameObject  loginText;
    public GameObject FailObj;//失败标志图
    public GameObject LoginObj;//加载过程
    public Text progressText;
    public Text progressTag;
    void Start()
    {
        Instance = this;
        LoginBut.onClick.AddListener(Login_Click);
        StartShowLogin();

    }

    /// <summary>
    /// 设置Ip，用户信息
    /// </summary>
    /// <param name="ipT"></param>
    /// <param name="portT"></param>
    /// <param name="usernameT"></param>
    /// <param name="passwardT"></param>
    public void SetInfo(string ipT, string portT, string usernameT, string passwardT)
    {
        ip = ipT;
        port = portT;
        username = usernameT;
        password = passwardT;
        StartShowLogin();
    }

    public void StartShowLogin()
    {
        IpInput.text = ip;
        PortInput.text = port;
        NameInput.text = username;
        PasswordInput.text = password;
    }
    /// <summary>
    /// 点击登陆界面
    /// </summary>
    public void Login_Click()
    {
        string ipstr = IpInput.text;
        string portstr = PortInput.text;
        string namestr = NameInput.text;
        string passwordstr = PasswordInput.text;
        if (ipstr.Trim() == ""|| portstr.Trim() == "")
        {
            LoginFail();
            return;
        }
        LoginManage.Instance.Login(ipstr, portstr, namestr, passwordstr);
    }
    public void LoginProcess()
    {
        progressText.text = "加载过程中";
        progressTag.text = "";
        FailObj.SetActive(false);
        loginText.SetActive(false);    
        showLoginProcess();
        StartCoroutine(OneLoginProcess());
    }
    IEnumerator OneLoginProcess()
    {
        Debug.Log("登录界面");
        yield return new WaitForSeconds(0.5f);
        progressTag.text = ".";
        yield return new WaitForSeconds(0.5f);
        progressTag.text = "..";
        yield return new WaitForSeconds(0.5f);
        progressTag.text = "...";
        StartCoroutine(OneLoginProcess());
    }
    public void LoginFail()
    {
        StopAllCoroutines(); 
        if (!loginText.activeInHierarchy)
        {
            loginText.SetActive(true);
        }   
       if (!FailObj.activeInHierarchy)
        {
            FailObj.SetActive(true);   
        } 
        progressText.text = "";
        progressTag.text = "";
        showLoginProcess();
        Invoke("CloseLoginProcess", 3f);
    }
    /// <summary>
    /// 打开电场登录界面
    /// </summary>
    public void ShowLogin()
    {
        LoginWindow.SetActive(true);
    }
    public void CloseLogin()
    {
        LoginWindow.SetActive(false);
        StopAllCoroutines();
        StartCommand.Instance.SetTestPanel(false);
    }
    /// <summary>
    /// 展示加载过程提示框
    /// </summary>
    public void showLoginProcess()
    {
        LoginObj.SetActive(true);
    }
  
    public void CloseLoginProcess()
    {
        LoginObj.SetActive(false);
    }

    public void LoginBtn_Trigger()
    {
        ExecuteEvents.Execute<ISubmitHandler>(LoginBut.gameObject, new PointerEventData(EventSystem.current), ExecuteEvents.submitHandler);
    }
}
