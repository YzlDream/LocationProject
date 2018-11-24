using BestHTTP.SignalR;
using BestHTTP.SignalR.Hubs;
using BestHTTP.SignalR.JsonEncoders;
using BestHTTP.SignalR.Messages;
using LitJson;
//using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Net;
using Unity.Common.Utils;
using UnityEngine;

public class CommunicationCallbackClient : MonoSingletonBase<CommunicationCallbackClient>
{  
    /// <summary>
    /// IP地址
    /// </summary>
    public string Ip;
    /// <summary>
    /// 端口
    /// </summary>
    public int Port;
    /// <summary>
    /// SignalR服务地址
    /// </summary>
    Uri URI;

    /// <summary>
    /// The SignalR connection instance
    /// </summary>
    Connection signalRConnection;

    /// <summary>
    /// 告警Hub
    /// </summary>
    public AlarmHub alarmHub = new AlarmHub();

    /// <summary>
    /// EchoHub
    /// </summary>
    public EchoHub echoHub=new EchoHub();

    /// <summary>
    /// 门禁信息Hub
    /// </summary>
    public DoorAccessHub doorAccessHub = new DoorAccessHub();
    // Use this for initialization
    void Start () {

        string PortT = Port.ToString();

#if !UNITY_EDITOR
        Ip = SystemSettingHelper.communicationSetting.Ip2;
        PortT = SystemSettingHelper.communicationSetting.Port2;
#endif

        RegisterImporter();
        //if(alarmHub==null)alarmHub = new AlarmHub();
        //if(echoHub==null) echoHub = new EchoHubT();
        URI = new Uri(string.Format("http://{0}:{1}/realtime",Ip, PortT));
        // Create the SignalR connection, passing all the three hubs to it
        signalRConnection = new Connection(URI, alarmHub, echoHub,doorAccessHub);

        signalRConnection.JsonEncoder = new LitJsonEncoder();        

        signalRConnection.OnStateChanged += signalRConnection_OnStateChanged;
        signalRConnection.OnError += signalRConnection_OnError;
        signalRConnection.OnNonHubMessage += signaleRConnection_OnNoHubMsg;
        signalRConnection.OnConnected += (connection) =>
        {
            // Call the demo functions
            echoHub.Send("Start Connect...");
        };

        // Start opening the signalR connection
        signalRConnection.Open();
    }
    /// <summary>
    /// 注册类型转换器
    /// </summary>
    private void RegisterImporter()
    {
        JsonMapper.RegisterImporter<int, long>((int value) =>
        {
            return (long)value;
        });
        JsonMapper.RegisterImporter<double, float>((double value) =>
        {
            return (float)value;
        });
    }
    // Update is called once per frame
    void Update () {

	}
    /// <summary>
    /// Display state changes
    /// </summary>
    void signalRConnection_OnStateChanged(Connection connection, ConnectionStates oldState, ConnectionStates newState)
    {
        Debug.Log(string.Format("[State Change] {0} => {1}", oldState, newState));
    }

    /// <summary>
    /// Display errors.
    /// </summary>
    void signalRConnection_OnError(Connection connection, string error)
    {
        Debug.Log("[Error] " + error);
    }
    void signaleRConnection_OnNoHubMsg(Connection connection, object data)
    {
        string arg0 = JsonMapper.ToJson(data);
        Debug.Log("NoHub Msg:"+arg0);
    }
    void OnDestroy()
    {
        // Close the connection when we are closing this sample
        if (signalRConnection != null)
        {
            signalRConnection.Close();
        }
    }
   
}
