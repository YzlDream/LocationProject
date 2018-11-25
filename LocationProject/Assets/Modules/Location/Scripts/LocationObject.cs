﻿using HighlightingSystem;
using Location.WCFServiceReferences.LocationServices;
using Mogoson.CameraExtension;
using StardardShader;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UIWidgets;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;

public class LocationObject : MonoBehaviour
{
    /// <summary>
    /// NavMeshAgent
    /// </summary>
    public NavMeshAgent agent;
    /// <summary>
    /// 定位卡信息
    /// </summary>
    public Tag Tag;
    /// <summary>
    /// 人员信息
    /// </summary>
    public Personnel personnel;
    /// <summary>
    /// 定位卡位置信息
    /// </summary>
    public TagPosition tagPosInfo;
    /// <summary>
    /// 目标位置
    /// </summary>
    private Vector3 targetPos;
    /// <summary>
    /// 保存3D位置信息协程
    /// </summary>
    private Coroutine coroutine;

    Thread thread;
    /// <summary>
    /// 当前位置
    /// </summary>
    public Vector3 currentPos;
    /// <summary>
    /// 摄像头聚交用
    /// </summary>
    public AlignTarget alignTarget;
    //[HideInInspector]
    public PersonInfoUI personInfoUI;

    ///// <summary>
    ///// 碰撞器
    ///// </summary>
    //private Collider myCollider;
    /// <summary>
    /// 渲染器
    /// </summary>
    private Renderer[] renders;

    /// <summary>
    /// 正常区域
    /// </summary>
    public List<MonitorRangeObject> normalAreas;

    /// <summary>
    /// 人员进入的定位区域集合
    /// </summary>
    public List<MonitorRangeObject> locationAreas;

    /// <summary>
    /// 人员进入的告警区域集合
    /// </summary>
    public List<MonitorRangeObject> alarmAreas;
    /// <summary>
    /// 当前人员所在的建筑节点,是根据基站计算位置来的
    /// </summary>
    public DepNode currentDepNode;

    /// <summary>
    /// 开始碰撞检测
    /// </summary>
    public bool isStartOnTrigger;

    /// <summary>
    /// 开始碰撞OnTriggerStay检测一次
    /// </summary>
    public bool isOnTriggerStayOnce;
    /// <summary>
    /// 人员是否显示了
    /// </summary>
    private bool isRenderEnable;
    public bool IsRenderEnable
    {
        get
        {
            return isRenderEnable;
        }
    }
    /// <summary>
    /// TagCode
    /// </summary>
    public string tagcode;


    private void Awake()
    {
        normalAreas = new List<MonitorRangeObject>();
        locationAreas = new List<MonitorRangeObject>();
        alarmAreas = new List<MonitorRangeObject>();
        //transform.position = targetPos;
        //alignTarget = new AlignTarget(transform, new Vector2(30, 0), 5, new Range(-90, 90), new Range(1, 10));

        GetAlignTarget();

    }

    /// <summary>
    /// 获取AlignTarget
    /// </summary>
    private AlignTarget GetAlignTarget()
    {
        Quaternion quaDir = Quaternion.LookRotation(-transform.forward, Vector3.up);
        alignTarget = new AlignTarget(transform, new Vector2(30, quaDir.eulerAngles.y), 5, new Range(5, 90), new Range(1, 40));
        return alignTarget;
    }

    void OnEnable()
    {
        //人员预设物体状态必须为未激活的 不然在AddComponent<LocationObject>()后 在Init()前 OnEnable就会被调用 此时Tag等数据还未设置
        transform.position = targetPos;
        //Debug.Log("OnEnable_LocationObject");
        //InvokeRepeating("SaveU3DHistoryPosition", 0, 0.5F);//这里是每隔20秒重复刷新显示
        //if (coroutine == null)
        //{
        //    coroutine = StartCoroutine(SaveU3DHistoryPosition_Coroutine());
        //}
        if (personInfoUI == null)
        {
            FollowUINormalOn();
        }
        else
        {

            //personInfoUI.gameObject.SetActive(true);
            SetFollowPersonInfoUIActive(isRenderEnable);

        }
    }

    // Use this for initialization
    void Start()
    {
        DoubleClickEventTrigger_u3d lis = DoubleClickEventTrigger_u3d.Get(gameObject);
        lis.onDoubleClick = On_DoubleClick;
        ////transform.localPosition = targetPos;
        //agent = gameObject.AddMissingComponent<NavMeshAgent>();
        ////agent.radius = 0.2f;
        //agent.height = 1.7f;
        //agent.speed = 3.5f;
        SetRendererEnable(true);
        //FollowUINormalOn();
    }

    void OnDisable()
    {
        ClearAreas();

        FollowUIOff();
        try
        {
            if (thread != null)
            {
                thread.Abort();
            }
        }
        catch (Exception ex)
        {
            Log.Error("LocationObject.OnDisable", ex.ToString());
        }
    }

    public void On_DoubleClick()
    {
        if (LocationManager.Instance.currentLocationFocusObj != this)
        {
            LocationManager.Instance.FocusPersonAndShowInfo((int)personnel.TagId);
        }
        else
        {
            LocationManager.Instance.RecoverBeforeFocusAlign();
        }
    }

    /// <summary>
    /// 清除人员所在区域范围的相关信息
    /// </summary>
    private void ClearAreas()
    {
        if (normalAreas != null)
        {
            //foreach (MonitorRangeObject obj in normalAreas)
            //{
            //    obj.OnTriggerExitEx(this);
            //}
            normalAreas.Clear();
        }

        if (alarmAreas != null)
        {
            foreach (MonitorRangeObject obj in alarmAreas)
            {
                obj.OnTriggerExitEx(this);
            }
            alarmAreas.Clear();
        }

        if (locationAreas != null)
        {
            foreach (MonitorRangeObject obj in locationAreas)
            {
                obj.OnTriggerExitEx(this);
            }
            locationAreas.Clear();
        }
    }

    // Update is called once per frame
    void Update()
    {
        GetAlignTarget();
        SetPosition();
        currentPos = transform.position;
    }



    void LateUpdate()
    {
        if (!LocationManager.Instance.IsFocus)//摄像机不属于人员聚焦状态
        {
            if (locationAreas.Count == 0)
            {
                SetRendererEnable(false);
            }
        }
    }

    /// <summary>
    /// 初始化
    /// </summary>
    public void Init(Tag t)
    {
        //Log.Info("LocationObject.Init","name:"+ t.Name);
        Tag = t;
        personnel = PersonnelTreeManage.Instance.departmentDivideTree.personnels.Find((item) => item.TagId == Tag.Id);
        if (personnel == null)
        {
            Log.Alarm("LocationObject.Init", personnel == null);
            gameObject.name = Tag.Name + Tag.Code;
        }
        else
        {
            gameObject.name = Tag.Name + Tag.Code + personnel.Name;
        }
        tagcode = Tag.Code;
    }

    /// <summary>
    /// 设置位置信息
    /// </summary>
    public void SetPositionInfo(TagPosition tagPos)
    {
        tagPosInfo = tagPos;
        Vector3 offset = LocationManager.Instance.GetPosOffset();
        targetPos = new Vector3((float)tagPosInfo.X, (float)tagPosInfo.Y, (float)tagPosInfo.Z);

        targetPos = LocationManager.GetRealVector(targetPos);

        //if (tagPos.Tag == "0008" || tagPos.Tag == "0009")
        //{
        //    Debug.Log(tagPos.Tag + ":" + tagPos.TopoNodes);
        //}
        //if (gameObject.name == "标签30004陈先生")
        //{
        //    int i = 0;
        //}

        if (gameObject.name.Contains("0007"))
        {
            int i = 0;
        }
        //tagPos.AreaId = 260;
        float halfHeight = gameObject.GetSize().y / 2;//当胶囊体时
        halfHeight = 0;//当人物模型时
        if (gameObject.name.Contains("0003"))
        {
            //Debug.LogErrorFormat("标签：{0},AreaId:{1}", tagPos.Tag, tagPos.AreaId);
        }
        if (tagPos.AreaId != null)
        {

            DepNode depnode = RoomFactory.Instance.GetDepNodeById((int)tagPos.AreaId);
            if (currentDepNode != depnode)
            {
                isOnTriggerStayOnce = false;
            }
            currentDepNode = depnode;
            if (currentDepNode == null)
            {
                int i = 0;
            }
            //Debug.LogFormat("名称:{0},类型:{1}", depnode.name, depnode.NodeObject);
            if (depnode != null && depnode.floorCube != null)//二层267
            {
                if (depnode.floorCube != null)
                {
                    targetPos = new Vector3(targetPos.x, halfHeight + depnode.floorCube.position.y, targetPos.z);
                }
                else
                {
                    Debug.LogError("建筑物没有加楼层地板！！！");
                }
            }
            else
            {
                //int i = 0;
                targetPos = new Vector3(targetPos.x, LocationManager.Instance.axisZero.y + halfHeight, targetPos.z);
            }


            //else//一层259
            //{
            //    targetPos = new Vector3(targetPos.x, LocationManager.Instance.axisZero.y + gameObject.GetSize().y / 2, targetPos.z);
            //}
            //}
        }
        else
        {
            if (currentDepNode != null)
            {
                currentDepNode = null;
                //isOnTriggerStayOnce = false;//大数据测试修改
            }
            targetPos = new Vector3(targetPos.x, LocationManager.Instance.axisZero.y + halfHeight, targetPos.z);
        }
        isStartOnTrigger = true;
        //targetPos = new Vector3(-targetPos.z, targetPos.y, targetPos.x);

        //targetPos = targetPos + offset;
        //targetPos = targetPos;
        //print(string.Format("name:{0}||位置:x({1}),y({2}),z({3})", name, targetPos.x, targetPos.y, targetPos.z));
    }

    public void SetPosition()
    {
        float dis = Vector3.Distance(transform.position, targetPos);
        //if (dis > 1)
        //{

        //获取方向
        Vector3 dir = targetPos - transform.position;
        dir = new Vector3(dir.x, 0, dir.z);
        if (dir != Vector3.zero)
        {
            //将方向转换为四元数
            Quaternion quaDir = Quaternion.LookRotation(dir, Vector3.up);
            //缓慢转动到目标点
            transform.rotation = Quaternion.Lerp(transform.rotation, quaDir, Time.fixedDeltaTime * 10);
        }

        //agent.enabled = false;

        if (BuildingController.isTweening)
        {
            if (currentDepNode != null && currentDepNode.floorCube != null)
            {
                //Debug.LogError("isTweening");
                transform.position = targetPos;
            }
        }
        else
        {
            transform.position = Vector3.Lerp(transform.position, targetPos, LocationManager.Instance.damper * Time.deltaTime);
        }

        //agent.enabled = true;
        //agent.speed = 5f + dis;
        //}
        //else
        //{
        //    agent.speed = 3.5f;
        //    agent.SetDestination(targetPos);
        //}

    }

    private void OnTriggerEnter(Collider other)
    {
        if (!isStartOnTrigger) return;
        if (gameObject.name == "标签60007赵一男")
        {
            int i = 0;
        }
        //MonitorRangeObject mapAreaObject = other.gameObject.GetComponent<MonitorRangeObject>();
        //if (mapAreaObject)
        //{
        //    FollowUIAlarmOn(mapAreaObject.info.Name);
        //}

        //Debug.LogError("OnTriggerEnter");
        //Debug.LogFormat("code:{0},区域名称:{1},OnTriggerEnter", Tag.Code, other.name);

        if (gameObject.name == "标签30004陈先生")
        {
            int i = 0;
        }
        if (gameObject.name == "标签50006邱先生")
        {
            int i = 0;
        }


        MonitorRangeObject mapAreaObject = other.gameObject.GetComponent<MonitorRangeObject>();
        if (mapAreaObject)
        {
            TransformM tranM = mapAreaObject.info.Transfrom;
            if (tranM != null)
            {
                bool isSameFloor = mapAreaObject.CheckFloorDepNode(currentDepNode);
                isSameFloor = true;//以后这个判断就不要了，在做完编辑区域之后
                if (tranM.IsOnLocationArea)
                {
                    if (!locationAreas.Contains(mapAreaObject) && isSameFloor)
                    {
                        if (locationAreas.Count == 0)//如果人员未处于显示状态
                        {
                            SetRendererEnable(true);
                        }
                        locationAreas.Add(mapAreaObject);

                    }
                }

                if (tranM.IsOnAlarmArea)
                {
                    if (!alarmAreas.Contains(mapAreaObject) && isSameFloor)
                    {
                        if (alarmAreas.Count == 0)//如果人员未处于告警状态
                        {
                            FollowUIAlarmOn(mapAreaObject.info.Name);
                        }
                        alarmAreas.Add(mapAreaObject);
                    }
                }

                if (!normalAreas.Contains(mapAreaObject))
                {
                    normalAreas.Add(mapAreaObject);
                    SetArea();
                }
            }
        }


    }

    //大数据测试修改
    private void OnTriggerStay(Collider other)
    {
        if (!isStartOnTrigger) return;
        if (!isOnTriggerStayOnce)
        {
            OnTriggerEnter(other);
            isOnTriggerStayOnce = true;
        }
        //OnTriggerEnter(other);//大数据测试修改
        //Debug.LogFormat("code:{0},区域名称:{1},OnTriggerStay", Tag.Code, other.name);
    }

    private void OnTriggerExit(Collider other)
    {
        if (!isStartOnTrigger) return;
        //MonitorRangeObject mapAreaObject = other.gameObject.GetComponent<MonitorRangeObject>();
        //if (mapAreaObject)
        //{

        //    FollowUINormalOn();

        //}

        MonitorRangeObject areaObject = other.gameObject.GetComponent<MonitorRangeObject>();
        OnTriggerExitEx(areaObject);
    }

    public void OnTriggerExitEx(MonitorRangeObject areaObject)
    {
        if (areaObject)
        {
            Debug.LogFormat("code:{0},区域名称:{1},OnTriggerExit", Tag.Code, areaObject.name);
            if (locationAreas.Contains(areaObject))
            {
                locationAreas.Remove(areaObject);
                if (locationAreas.Count == 0)
                {
                    SetRendererEnable(false);
                }
            }

            if (alarmAreas.Contains(areaObject))
            {
                alarmAreas.Remove(areaObject);
                if (alarmAreas.Count == 0)
                {
                    FollowUINormalOn();
                }
            }

            if (normalAreas.Contains(areaObject))
            {
                normalAreas.Remove(areaObject);
                SetArea();
            }

            //TransformM tranM = mapAreaObject.info.Transfrom;
            //if (tranM != null)
            //{
            //    if (tranM.IsOnLocationArea)
            //    {
            //        SetRendererEnable(false);
            //        FollowUINormalOn();
            //    }
            //    else
            //    {

            //    }
            //}

        }
    }

    /// <summary>
    /// 创建跟随UI
    /// </summary>
    /// <returns></returns>
    public PersonInfoUI CreateFollowUI()
    {
        if (personnel == null) return null;
        if (personInfoUI != null) return personInfoUI;
        GameObject targetTagObj = UGUIFollowTarget.CreateTitleTag(gameObject, Vector3.zero);
        GameObject obj = UGUIFollowManage.Instance.CreateItem(LocationManager.Instance.PersonInfoUIPrefab.gameObject, targetTagObj, "LocationNameUI");
        personInfoUI = obj.GetComponent<PersonInfoUI>();
        if (Tag == null)
        {
            Log.Error("Location.CreateFollowUI", "Tag == null");
        }
        else if (personnel == null)
        {
            //Log.Error("Location.CreateFollowUI", "personnel == null：" + Tag.Name);
        }
        personInfoUI.Init(personnel);
        return personInfoUI;

    }

    /// <summary>
    /// 人员跟随UI，进入告警状态
    /// </summary>
    public void FollowUIAlarmOn(string areaname)
    {
        Debug.LogFormat("Tag:{0},区域:{1},告警！", Tag.Code, areaname);
        CreateFollowUI();
        if (personInfoUI != null)
        {
            personInfoUI.ShowAlarm();
            personInfoUI.SetTxtAlarmAreaName(areaname);
        }
    }


    /// <summary>
    /// 人员跟随UI开启
    /// </summary>
    public void FollowUINormalOn()
    {
        CreateFollowUI();
        if (personInfoUI != null)
        {
            personInfoUI.ShowNormal();
        }
    }

    /// <summary>
    /// 人员跟随UI开启
    /// </summary>
    public void FollowUIOn()
    {
        //Log.Info("LocationObject.FollowUIOn", "name:" + Tag.Name);
        Transform titleTag = transform.Find("TitleTag");
        if (titleTag != null)
        {
            UGUIFollowManage.Instance.SetUIbyTarget("LocationNameUI", titleTag.gameObject, true);
        }
    }

    /// <summary>
    /// 人员跟随UI关闭
    /// </summary>
    public void FollowUIOff()
    {
        //Log.Info("LocationObject.FollowUIOff", "name:" + Tag.Name);
        Transform titleTag = transform.Find("TitleTag");
        if (titleTag != null)
        {
            UGUIFollowManage.Instance.SetUIbyTarget("LocationNameUI", titleTag.gameObject, false);
        }
    }

    ///// <summary>
    ///// 获取碰撞器
    ///// </summary>
    ///// <returns></returns>
    //public Collider GetCollider()
    //{
    //    if (myCollider == null)
    //    {
    //        myCollider = gameObject.GetComponent<Collider>();
    //    }
    //    return myCollider;
    //}

    ///// <summary>
    ///// 设置碰撞器是否启用
    ///// </summary>
    //public void SetColliderEnable(bool isEnable)
    //{
    //    GetCollider();
    //    myCollider.enabled = isEnable;
    //}

    /// <summary>
    /// 获取渲染器
    /// </summary>
    /// <returns></returns>
    public Renderer[] GetRenderer()
    {
        if (renders == null || renders.Length == 0)
        {
            renders = gameObject.GetComponentsInChildren<Renderer>();
        }
        return renders;
    }



    /// <summary>
    /// 设置渲染器是否启用
    /// </summary>
    public void SetRendererEnable(bool isEnable)
    {
        GetRenderer();
        if (gameObject.name == "标签60007赵一男")
        {
            int i = 0;
        }
        isRenderEnable = isEnable;
        if (renders != null)
        {
            renders.ForEach(i => i.enabled = isEnable);
        }


        if (isEnable)
        {
            FollowUIOn();
        }
        else
        {
            if (LocationManager.Instance.currentLocationFocusObj == this)
            {
                LocationManager.Instance.HideCurrentPersonInfoUI();
            }
            FollowUIOff();
        }
    }

    [ContextMenu("SetTTHide")]
    public void SetTTHide()
    {
        GetRenderer();
        if (gameObject.name == "标签60007赵一男")
        {
            int i = 0;
        }
        isRenderEnable = false;
        if (renders != null)
        {
            renders.ForEach(i => i.enabled = false);
        }
    }

    [ContextMenu("SetTTShow")]
    public void SetTTShow()
    {
        GetRenderer();
        if (gameObject.name == "标签60007赵一男")
        {
            int i = 0;
        }
        isRenderEnable = true;
        if (renders != null)
        {
            renders.ForEach(i => i.enabled = true);
        }
    }

    private U3DPosition CreateU3DPosition(TagPosition tp, Transform t)
    {
        U3DPosition u3dPos = new U3DPosition();
        u3dPos.Tag = tp.Tag;
        u3dPos.Time = tp.Time;
        u3dPos.Number = tp.Number;
        u3dPos.Power = tp.Power;
        u3dPos.Flag = tp.Flag;
        //Vector3 temp= LocationManager.GetDisRealVector(targetPos);
        u3dPos.X = t.position.x;
        u3dPos.Y = t.position.y;
        u3dPos.Z = t.position.z;
        return u3dPos;
    }

    private U3DPosition CreateU3DPosition(U3DPosition p)
    {
        U3DPosition u3dPos = new U3DPosition();
        u3dPos.Tag = p.Tag;
        u3dPos.Time = p.Time;
        u3dPos.Number = p.Number;
        u3dPos.Power = p.Power;
        u3dPos.Flag = p.Flag;
        //Vector3 temp= LocationManager.GetDisRealVector(targetPos);
        u3dPos.X = p.X;
        u3dPos.Y = p.Y;
        u3dPos.Z = p.Z;
        return u3dPos;
    }

    /// <summary>
    /// 保存3D位置信息
    /// </summary>
    public void SaveU3DHistoryPosition()
    {

        U3DPosition u3dPos = CreateU3DPosition(tagPosInfo, transform);

        if (!IsBusy)
        {
            Debug.Log(name + ":(" + Time.time + ")");
            IsBusy = true;
            u3dPos.Flag = u3dPos.Flag + "(" + name + ")";
            //u3dPos2.Flag = u3dPos2.Flag + "(" + name + ")";

            thread = new Thread(() =>
            {
                try
                {
                    //for (int i = 0; i < 10; i++)
                    //{
                    //    U3DPosition u3dPos2 = CreateU3DPosition(u3dPos);
                    //    u3dPos2.Tag += i;
                    //    Thread thread2 = new Thread(() =>
                    //    {
                    //        CommunicationObject.Instance.AddU3DPosition(u3dPos2);
                    //    });
                    //    thread2.Start();
                    //}

                    CommunicationObject.Instance.AddU3DPosition(u3dPos);

                }
                catch (Exception ex)
                {
                    Log.Error(ex.ToString());
                }
                IsBusy = false;

            });
            thread.Start();
        }
        else
        {
            Debug.Log(string.Format("Tag:{0},IsBusy:{1}", name, IsBusy));
        }

    }

    bool isBusy;

    public bool IsBusy
    {
        get
        {
            return isBusy;
        }

        set
        {
            isBusy = value;
        }
    }



    /// <summary>
    /// 设置跟随UI是否显示
    /// </summary>
    /// <param name="isActive"></param>
    public void SetFollowPersonInfoUIActive(bool isActive)
    {
        if (personInfoUI != null)
        {
            personInfoUI.gameObject.SetActive(isActive);
        }
    }

    /// <summary>
    /// 设置当前所在区域
    /// </summary>
    public void SetArea()
    {
        PhysicalTopology pty = GetAreaPhysicalTopologyByType(AreaTypes.机房);

        pty = pty == null ? GetAreaPhysicalTopologyByType(AreaTypes.楼层) : pty;
        pty = pty == null ? GetAreaPhysicalTopologyByType(AreaTypes.大楼) : pty;
        pty = pty == null ? GetAreaPhysicalTopologyByType(AreaTypes.区域) : pty;

        if (personInfoUI != null)
        {
            if (pty != null)
            {
                personInfoUI.SetTxtAreaName(pty.Name);
            }
            else
            {
                personInfoUI.SetTxtAreaName("该区域无法识别");
            }
        }
    }

    /// <summary>
    /// 获取区域节点信息通过类型
    /// </summary>
    public PhysicalTopology GetAreaPhysicalTopologyByType(AreaTypes typeT)
    {
        foreach (MonitorRangeObject robj in normalAreas)
        {
            if (robj.info.Type == typeT)
            {
                return robj.info;
            }
        }
        return null;
    }

    /// <summary>
    /// 开启高亮
    /// </summary>
    public void HighlightOn()
    {
        Highlighter h = gameObject.AddMissingComponent<Highlighter>();
        h.ConstantOn(Color.green);
    }

    /// <summary>
    /// 关闭高亮
    /// </summary>
    public void HighlightOff()
    {
        Highlighter h = gameObject.GetComponent<Highlighter>();
        h.ConstantOff();
    }

    /// <summary>
    /// 离开时透明
    /// </summary>
    [ContextMenu("TransparentLeave")]
    public void TransparentLeave()
    {

        GameObjectMaterial.SetAllTransparent(gameObject, 0.3f);

        //SkinnedMeshRenderer[] skinnedMeshRenderers = gameObject.GetComponentsInChildren<SkinnedMeshRenderer>();
        //foreach (SkinnedMeshRenderer render in skinnedMeshRenderers)
        //{
        //    Material[] mats = render.materials;
        //    foreach (Material m in mats)
        //    {
        //        m.color = new Color(m.color.r, m.color.g, m.color.b, 0.3f); 
        //    }
        //}
    }

    /// <summary>
    /// 恢复离开时透明状态到正常状态
    /// </summary>
    [ContextMenu("RecoverTransparentLeave")]
    public void RecoverTransparentLeave()
    {


        GameObjectMaterial.Recover(transform);
        //SkinnedMeshRenderer[] skinnedMeshRenderers = gameObject.GetComponentsInChildren<SkinnedMeshRenderer>();
        //foreach (SkinnedMeshRenderer render in skinnedMeshRenderers)
        //{
        //    Material[] mats = render.materials;
        //    foreach (Material m in mats)
        //    {
        //        m.color = new Color(m.color.r, m.color.g, m.color.b, 1f);
        //    }
        //}
    }

    [ContextMenu("SwitchNormal")]
    public void SwitchNormal()
    {
        personInfoUI.personnelNodeManage.SwitchNormal();
        RecoverTransparentLeave();
        //infoUi.l
    }


    [ContextMenu("SwitchStandby")]
    public void SwitchStandby()
    {
        //SwitchStateSprite(PersonInfoUIState.Standby);
        personInfoUI.personnelNodeManage.SwitchStandby();
        RecoverTransparentLeave();
    }

    [ContextMenu("SwitchLeave")]
    public void SwitchLeave()
    {
        personInfoUI.personnelNodeManage.SwitchLeave();
        TransparentLeave();
    }

    [ContextMenu("SwitchLowBattery")]
    public void SetLowBattery()
    {
        personInfoUI.personnelNodeManage.SetLowBattery();
    }
}