using HighlightingSystem;
using Location.WCFServiceReferences.LocationServices;
using Mogoson.CameraExtension;
using MonitorRange;
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
    /// 告警信息列表
    /// </summary>
    public List<LocationAlarm> alarmList;
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
    /// <summary>
    /// 人员动画控制器
    /// </summary>
    public PersonAnimationController personAnimationController;

    /// <summary>
    /// 位置点是否在所在区域范围内部
    /// </summary>
    public bool isInCurrentRange = true;
    /// <summary>
    /// 是否处于告警中
    /// </summary>
    public bool isAlarming;

    private void Awake()
    {
        normalAreas = new List<MonitorRangeObject>();
        locationAreas = new List<MonitorRangeObject>();
        alarmAreas = new List<MonitorRangeObject>();
        alarmList = new List<LocationAlarm>();
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
        //if (Tag.Code.Contains("0997"))
        //{
        //    int i = 0;
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
        if (personAnimationController == null)
        {
            personAnimationController = GetComponent<PersonAnimationController>();
        }
        ////transform.localPosition = targetPos;
        agent = gameObject.GetComponent<NavMeshAgent>();
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
        FlashingOffArchors();
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
                if (isRenderEnable==false) return;
                if (!LocationManager.Instance.isShowLeavePerson)
                {
                    SetRendererEnable(false);
                }
                else
                {
                    bool b = LocationManager.IsBelongtoCurrentDep(this);
                    if (!b)
                    {
                        SetRendererEnable(false);
                    }
                }
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
        InitPersonnel();
        tagcode = Tag.Code;
    }

    /// <summary>
    /// 初始化人员信息
    /// </summary>
    public void InitPersonnel()
    {
        personnel = PersonnelTreeManage.Instance.departmentDivideTree.personnels.Find((item) => item.TagId == Tag.Id);
        //if (personnel == null)
        //{
        //    if (Tag.Code == "55555")
        //    {
        //        personnel = new Personnel();
        //        FollowUINormalOn();
        //    }
        //}

        if (personInfoUI == null)//如果跟随UI没创建，需要创建
        {
            FollowUINormalOn();
            SetFollowPersonInfoUIActive(isRenderEnable);
        }

        if (personnel == null)
        {

            Log.Alarm("LocationObject.Init", personnel == null);
            gameObject.name = Tag.Name + Tag.Code;
        }
        else
        {
            gameObject.name = Tag.Name + Tag.Code + personnel.Name;
        }
    }

    /// <summary>
    /// 设置位置信息
    /// </summary>
    public void SetPositionInfo(TagPosition tagPos)
    {
        tagPosInfo = tagPos;
        SetState(tagPosInfo);
        //if (personInfoUI != null && personInfoUI.state == PersonInfoUIState.Leave) return; //人员处于离开状态，就不移动了
        //tagPosInfo = tagPos;
        SetisInRange(true);
        Vector3 offset = LocationManager.Instance.GetPosOffset();
        targetPos = new Vector3((float)tagPosInfo.X, (float)tagPosInfo.Y, (float)tagPosInfo.Z);

        targetPos = LocationManager.GetRealVector(targetPos);

        if (tagPos.Tag == "097F"|| tagPos.Tag == "0986")
        {
            //Debug.Log(tagPos.Tag + ":" + tagPos.TopoNodes);
            int i = 0;
        }
        //SetState(tagPosInfo);

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
            Transform floorCubeT = GetFloorCube(depnode);
            if (currentDepNode != depnode)
            {
                isOnTriggerStayOnce = false;
            }
            currentDepNode = depnode;
            if (currentDepNode == null)
            {
                currentDepNode = FactoryDepManager.Instance;//如果人员的区域节点为空，就默认把他设为园区节点
            }

            if (FactoryDepManager.currentDep != currentDepNode)
            {
                RoomFactory.Instance.FocusNodeForFocusPerson(currentDepNode, () =>
                {
                    //FocusPerson(locationObjectT.alignTarget);

                    //if (locationObjectT.personInfoUI != null)
                    //{
                    //    locationObjectT.personInfoUI.SetOpenOrClose(true);
                    //}
                }, false);
            }
            //Debug.LogFormat("名称:{0},类型:{1}", depnode.name, depnode.NodeObject);
            if (depnode != null && floorCubeT != null)//二层267
            {
                if (floorCubeT != null)
                {
                    Vector3 targetPosT = new Vector3(targetPos.x, halfHeight + floorCubeT.position.y, targetPos.z);
                    if (currentDepNode.monitorRangeObject)
                    {
                        if (Tag.Code == "0995" || Tag.Code == "097F")
                        {
                            int I = 0;
                        }
                        bool isInRangeT = currentDepNode.monitorRangeObject.IsInRange(targetPosT.x, targetPosT.z);
                        //if (isInRangeT)
                        //{
                        //    isInRangeT = currentDepNode.monitorRangeObject.IsOnLocationArea;
                        //}

                        SetisInRange(isInRangeT);

                    }
                    targetPos = targetPosT;
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
            else
            {
                currentDepNode = FactoryDepManager.Instance;//如果人员的区域节点为空，就默认把他设为园区节点
            }

            targetPos = new Vector3(targetPos.x, LocationManager.Instance.axisZero.y + halfHeight, targetPos.z);
        }
        isStartOnTrigger = true;
        //targetPos = new Vector3(-targetPos.z, targetPos.y, targetPos.x);

        //targetPos = targetPos + offset;
        //targetPos = targetPos;
        //print(string.Format("name:{0}||位置:x({1}),y({2}),z({3})", name, targetPos.x, targetPos.y, targetPos.z));

        if (LocationManager.Instance.currentLocationFocusObj == this)
        {
            ShowArchors();
        }
        else
        {
            FlashingOffArchors();
        }
    }

    /// <summary>
    /// 获取该区域节点计算位置的平面
    /// </summary>
    /// <param name="depnode"></param>
    public Transform GetFloorCube(DepNode depnode)
    {
        //Transform floorcube = null;
        if (depnode == null || depnode.TopoNode == null) return null;
        if (depnode.TopoNode.Type == AreaTypes.范围 || depnode.TopoNode.Type == AreaTypes.机房)
        {
            return FilterFloorCube(depnode);
        }
        else
        {
            return depnode.floorCube;
        }
    }

    /// <summary>
    /// 过滤该区域节点计算位置的平面
    /// </summary>
    /// <param name="depnode"></param>
    public Transform FilterFloorCube(DepNode depnode)
    {
        if (depnode == null) return null;
        if (depnode.TopoNode.Type == AreaTypes.楼层)
        {
            return depnode.floorCube;
        }
        else
        {
            return FilterFloorCube(depnode.ParentNode);
        }
    }

    public void SetPosition()
    {
        if (SystemSettingHelper.systemSetting.IsDebug)
        {
            ShowPositionSphereTest(targetPos);
        }

        if (!LocationManager.Instance.isShowLeavePerson)
        {
            if (personInfoUI != null && personInfoUI.state == PersonInfoUIState.Leave) return; //人员处于离开状态，就不移动了
        }
        //if (isInLocationRange == false) return;//如果位置点不在当前所在区域范围内部，就不设置点
        if (isInCurrentRange == false)//如果位置点不在当前所在区域范围内部
        {
            if (currentDepNode.monitorRangeObject && currentDepNode.monitorRangeObject.IsOnLocationArea)
            {
                if (!LocationManager.Instance.isShowLeavePerson)
                {
                    Vector2 v = currentDepNode.monitorRangeObject.PointForPointToPolygon(new Vector2(targetPos.x, targetPos.z));
                    targetPos = new Vector3(v.x, targetPos.y, v.y);
                }
            }
            else
            {
                return;
            }
        }
        else
        {
            if (currentDepNode == null) return;
            if (!LocationManager.Instance.isShowLeavePerson)
            {
                //如果位置点在当前所在区域范围内部,但是当前区域不是定位区域返回
                if (currentDepNode.monitorRangeObject == null || currentDepNode.monitorRangeObject.IsOnLocationArea == false) return;
            }
        }

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
            if (ThroughWallsManage.Instance && ThroughWallsManage.Instance.isColliderThroughWallsTest)
            {
                PersonMove move = gameObject.GetComponent<PersonMove>();
                if (move != null)
                {
                    move.SetPosition(targetPos);
                }
            }
            else if (ThroughWallsManage.Instance && ThroughWallsManage.Instance.isNavMeshThroughWallsTest)
            {
                if (agent)
                {
                    agent.SetDestination(targetPos);
                }
            }
            else
            {
                transform.position = Vector3.Lerp(transform.position, targetPos, LocationManager.Instance.damper * Time.deltaTime);
            }
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

                //三维里通过碰撞检测，来触发人员告警，这里注释是改成服务端发送过来触发告警（下面的ShowAlarm方法）
                //if (tranM.IsOnAlarmArea)
                //{
                //    if (!alarmAreas.Contains(mapAreaObject) && isSameFloor)
                //    {
                //        if (alarmAreas.Count == 0)//如果人员未处于告警状态
                //        {
                //            FollowUIAlarmOn(mapAreaObject.info.Name);
                //        }
                //        alarmAreas.Add(mapAreaObject);
                //    }
                //}

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
                    if (!LocationManager.Instance.isShowLeavePerson)
                    {
                        SetRendererEnable(false);
                    }
                }
            }

            //三维里通过碰撞检测来检测人员是否在告警区域内部，来关闭人员告警，这里注释是改成服务端发送过来关闭告警（下面的HideAlarm方法）
            //if (alarmAreas.Contains(areaObject))
            //{
            //    alarmAreas.Remove(areaObject);
            //    if (alarmAreas.Count == 0)
            //    {
            //        FollowUINormalOn();
            //    }
            //}

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
        personInfoUI.Init(personnel, this);
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
        if (SystemSettingHelper.systemSetting.IsDebug)
        {
            SetPosSphereActive(isEnable);
        }
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
    public void HighlightOnByFocus()
    {
        //Highlighter h = gameObject.AddMissingComponent<Highlighter>();
        //h.ConstantOn(Color.green);
        if (personInfoUI.state != PersonInfoUIState.StandbyLong)
        {
            HighlightOn(Color.green);
        }

    }
    /// <summary>
    /// 开启高亮
    /// </summary>
    public void HighlightOn(Color color)
    {
        if (isAlarming) return;
        Highlighter h = gameObject.AddMissingComponent<Highlighter>();
        h.ConstantOn(color);
    }

    /// <summary>
    /// 关闭高亮
    /// </summary>
    [ContextMenu("HighlightOff")]
    public void HighlightOff()
    {
        Highlighter h = gameObject.AddMissingComponent<Highlighter>();
        h.ConstantOff();
    }

    /// <summary>
    /// 开启高亮闪烁
    /// </summary>
    public void FlashingOn(Color color)
    {
        Highlighter h = gameObject.AddMissingComponent<Highlighter>();
        h.FlashingOn(new Color(color.r, color.g, color.b, 0), new Color(color.r, color.g, color.b, 1));
    }

    /// <summary>
    /// 关闭高亮闪烁
    /// </summary>
    [ContextMenu("FlashingOff")]
    public void FlashingOff()
    {
        Highlighter h = gameObject.AddMissingComponent<Highlighter>();
        h.ConstantOff();
    }

    /// <summary>
    /// 黄色高亮:长时间不动
    /// </summary>
    [ContextMenu("HighlightOnStandbyLong")]
    public void HighlightOnStandbyLong()
    {
        HighlightOn(Color.yellow);
    }

    /// <summary>
    /// 关闭因长时间不动黄色高亮
    /// </summary>
    public void HighlightOffStandbyLong()
    {
        if (LocationManager.Instance.currentLocationFocusObj == this)
        {
            HighlightOnByFocus();
        }
        else
        {
            HighlightOff();
        }
    }

    /// <summary>
    /// 展示告警
    /// </summary>
    public void ShowAlarm(LocationAlarm locationAlarm)
    {
        //if (!alarmAreas.Contains(mapAreaObject))
        //{
        //    if (alarmAreas.Count == 0)//如果人员未处于告警状态
        //    {
        //        if (isAlarming) return;
        //        FollowUIAlarmOn(mapAreaObject.info.Name);
        //        isAlarming = true;
        //        Debug.LogErrorFormat("区域：{0},告警了！", Tag.Code);
        //    }
        //    alarmAreas.Add(mapAreaObject);
        //}
        if (!alarmList.Contains(locationAlarm))
        {
            if (alarmList.Count == 0)//如果人员未处于告警状态
            {
                if (isAlarming) return;
                isAlarming = true;
                MonitorRangeObject monitorRangeObject = MonitorRangeManager.Instance.GetMonitorRangeObjectByAreaId(locationAlarm.AreaId);
                string nameT = "";
                if (monitorRangeObject != null)
                {
                    nameT = monitorRangeObject.info.Name;
                }
                FollowUIAlarmOn(nameT);
                FlashingOn(Color.red);
                Debug.LogErrorFormat("区域：{0},告警了！", Tag.Code);
            }
            alarmList.Add(locationAlarm);
        }
        
    }

    /// <summary>
    /// 关闭告警
    /// </summary>
    public void HideAlarm(LocationAlarm locationAlarm)
    {
        //if (alarmAreas.Contains(areaObject))
        //{
        //    alarmAreas.Remove(areaObject);
        //    if (alarmAreas.Count == 0)
        //    {
        //        if (isAlarming == false) return;
        //        FollowUINormalOn();
        //        isAlarming = false;
        //        Debug.LogErrorFormat("区域：{0},消警了！", Tag.Code);
        //    }
        //}

        if (alarmList.Contains(locationAlarm))
        {
            alarmList.Remove(locationAlarm);
            if (alarmList.Count == 0)
            {
                if (isAlarming == false) return;
                FollowUINormalOn();
                isAlarming = false;
                FlashingOff();
                Debug.LogErrorFormat("区域：{0},消警了！", Tag.Code);
            }
        }
    }

    #region 设置人物相关状态

    /// <summary>
    /// 设置人员状态
    /// </summary>
    public void SetState(TagPosition tagpT)
    {
        //if()
        if (personInfoUI == null) return;
        if (personAnimationController == null)
        {
            personAnimationController = GetComponent<PersonAnimationController>();
        }

        if (tagpT.PowerState == 0)
        {
            SetLowBatteryActive(false);
        }
        else
        {
            SetLowBatteryActive(true);
        }

        if (tagpT.AreaState == 1)//不在定位区域属于离开状态
        {
            SwitchLeave();
            personAnimationController.DoStop();
        }
        else//在定位区域
        {
            if (tagpT.MoveState == 0)//卡正常运动状态
            {
                SwitchNormal();
                personAnimationController.DoMove();
            }
            else if (tagpT.MoveState == 1 || tagpT.MoveState == 2)//待机状态
            {
                SwitchStandby();
                personAnimationController.DoStop();
            }
            else if (tagpT.MoveState == 3)//长时间不动状态
            {
                SwitchStandbyLong();
                personAnimationController.DoStop();
            }
            else
            {

            }
        }
    }

    /// <summary>
    /// 转换为正常状态
    /// </summary>
    [ContextMenu("SwitchNormal")]
    public void SwitchNormal()
    {
        personInfoUI.personnelNodeManage.SwitchNormal();
        RecoverTransparentLeave();
        HighlightOffStandbyLong();
        //infoUi.l
        personInfoUI.HideStandByTime();
    }

    /// <summary>
    /// 待机状态，包含静止状态（待机之后小于300秒）
    /// </summary>
    [ContextMenu("SwitchStandby")]
    public void SwitchStandby()
    {
        //SwitchStateSprite(PersonInfoUIState.Standby);
        personInfoUI.personnelNodeManage.SwitchStandby();
        RecoverTransparentLeave();
        HighlightOffStandbyLong();
        personInfoUI.ShowStandByTime();
    }

    /// <summary>
    /// 待机长时间不动
    /// </summary>
    [ContextMenu("SwitchStandby")]
    public void SwitchStandbyLong()
    {
        //SwitchStateSprite(PersonInfoUIState.Standby);
        personInfoUI.personnelNodeManage.SwitchStandbyLong();
        RecoverTransparentLeave();
        HighlightOnStandbyLong();
        personInfoUI.ShowStandByTime();
    }

    /// <summary>
    /// 设置为离开状态
    /// </summary>
    [ContextMenu("SwitchLeave")]
    public void SwitchLeave()
    {
        personInfoUI.personnelNodeManage.SwitchLeave();
        TransparentLeave();
        HighlightOffStandbyLong();
        personInfoUI.HideStandByTime();
    }

    /// <summary>
    /// 设置为弱电状态
    /// </summary>
    [ContextMenu("SetLowBattery")]
    public void SetLowBattery()
    {
        personInfoUI.personnelNodeManage.SetLowBattery();
    }

    /// <summary>
    /// 设置为弱电状态
    /// </summary>
    public void SetLowBatteryActive(bool isActive)
    {
        personInfoUI.personnelNodeManage.SetLowBatteryActive(isActive);
    }

    /// <summary>
    /// 离开时透明
    /// </summary>
    [ContextMenu("TransparentLeave")]
    public void TransparentLeave()
    {

        GameObjectMaterial.SetAllTransparent(gameObject, 0.5f);

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

    public void SetisInRange(bool b)
    {
        isInCurrentRange = b;
        if (Tag.Code == "0995" || Tag.Code == "097F")
        {
            //if (isInRange)
            //{
            //    Debug.LogError(Tag.Code + ":在范围内");
            //}
            //else
            //{
            //    Debug.LogError(Tag.Code + ":不在范围内");
            //}
        }
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

    #endregion

    #region 让参与计算的基站显示出来（测试）,及显示人员真实位置的测试小球
    //让参与计算的基站显示出来（测试）
    List<DevNode> archorObjs = new List<DevNode>();

    //public bool isShowArchor = false;//闪烁参与计算的基站

    /// <summary>
    /// 显示参与计算的基站
    /// </summary>
    public void ShowArchors()
    {
        if (SystemSettingHelper.systemSetting.IsDebug)
        {
            FlashingOnArchors();
        }
        //else
        //{
        //    FashingOffArchors();
        //}
    }

    /// <summary>
    /// 闪烁所有基站
    /// </summary>
    public void FlashingOnArchors()
    {
        FlashingOffArchors();

        archorObjs.Clear();

        if (tagPosInfo.Archors != null)
        {
            foreach (string astr in tagPosInfo.Archors)
            {
                Archor a = LocationManager.Instance.GetArchorByCode(astr);
                if (a == null) continue;
                int idT = a.DevInfoId;
                RoomFactory.Instance.GetDevByid(idT, (nodeT)
                    =>
                {
                    if (nodeT == null) return;
                    archorObjs.Add(nodeT);
                    nodeT.FlashingOn();
                });
            }
        }
    }

    /// <summary>
    /// 停止闪烁所有基站
    /// </summary>
    public void FlashingOffArchors()
    {
        if (SystemSettingHelper.systemSetting.IsDebug)
        {
            foreach (DevNode o in archorObjs)
            {
                o.FlashingOff();
            }
        }
    }

    public GameObject posSphere;
    public static Transform PositionSphereParent;
    /// <summary>
    /// 创建高亮测试小球显示人员的真实位置
    /// </summary>
    public void ShowPositionSphereTest(Vector3 p)
    {

        if (PositionSphereParent == null)
        {
            GameObject o = new GameObject();
            o.name = "PositionSphereParent";
            PositionSphereParent = o.transform;
        }
        if (posSphere == null)
        {
            posSphere = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            posSphere.transform.localScale = new Vector3(0.1f, 0.1f, 0.1f);
            posSphere.name = Tag.Code;
            posSphere.transform.SetParent(PositionSphereParent);
        }

        Highlighter h = posSphere.AddMissingComponent<Highlighter>();
        h.ConstantOn(Color.blue);
        posSphere.transform.position = p;
    }

    /// <summary>
    /// 设置位置球的显示和
    /// </summary>
    public void SetPosSphereActive(bool b)
    {
        if (posSphere == null) return;
        posSphere.SetActive(b);
    }

    #endregion


}
