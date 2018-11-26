using Location.WCFServiceReferences.LocationServices;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ServiceModel;
using UnityEngine;
using System.Linq;
using System.Threading;

public class CommunicationObject : MonoBehaviour
{
    public static CommunicationObject Instance;

    private LocationServiceClient client;
    /// <summary>
    /// 服务器IP地址
    /// </summary>
    public string ip = "192.168.1.155";//localhost
    /// <summary>
    /// 服务器端口号
    /// </summary>
    public string port = "8733";

    private void Awake()
    {
        Instance = this;
    }

    [ContextMenu("Start")]
    public void Start()
    {

        //Hello();
        //GetUser();
        //GetUsers();
        //GetTags();
        //GetMaps();
        //GetHistoryPositons();

        //DateTime end = DateTime.Now;
        //DateTime start = end.AddDays(-1);
        //DateTime end = new DateTime(2018, 6, 13, 13, 43, 0);
        //DateTime start = end.AddMinutes(-1);
        //GetHistoryPositonsByTime(start, end);
#if !UNITY_EDITOR
        ip = SystemSettingHelper.communicationSetting.Ip1;
        port = SystemSettingHelper.communicationSetting.Port1;
#endif
        GetTopoTree();
    }

    public PhysicalTopology GetTopoTree()
    {
        Debug.Log("->GetTopoTree");
        client = GetClient();
        int view = 0; //0:基本数据; 1:设备信息; 2:人员信息; 3:设备信息 + 人员信息
        PhysicalTopology topoRoot=client.GetPhysicalTopologyTree(view);
        if (topoRoot == null)
        {
            Log.Error("GetTopoTree topoRoot == null");
        }
        string txt = ShowTopo(topoRoot,0);
        Debug.Log(txt);
        return topoRoot;
    }

    private string ShowTopo(PhysicalTopology dep, int layer)
    {
        string whitespace = GetWhiteSpace(layer);
        if (dep == null) return "";
        string txt = whitespace + layer + ":" + dep.Name + "\n";
        if (dep.Children != null)
        {
            //txt+=whitespace + "length:" + dep.Children.Length+"\n";
            foreach (PhysicalTopology child in dep.Children)
            {
                txt += ShowTopo(child, layer + 1);
            }
        }
        else
        {
            //txt += whitespace + "children==null\n";
        }
        return txt;
    }

    // Update is called once per frame
    void Update()
    {

    }

    public LocationServiceClient CreateServiceClient()
    {
        string hostName = ip;
        string portNum = port;
        System.ServiceModel.Channels.Binding wsBinding = new BasicHttpBinding();
        string url =
            string.Format("http://{0}:{1}/LocationService/",
                hostName, portNum);
        Log.Info("Create Service Client:" + url);
        EndpointAddress endpointAddress = new EndpointAddress(url);

        return new LocationServiceClient(wsBinding, endpointAddress);
    }

    public LocationServiceClient GetClient()
    {
        if (client == null)
        {
            if (client != null)
            {
                if (client.State == CommunicationState.Opened)
                {
                    client.Close();
                }
            }
            client = CreateServiceClient();
        }
        return client;
    }

    //private LocationServiceClient client;

    public void Hello(string msg)
    {
        Debug.Log("->Hello");
        client = GetClient();
        lock (client)
        {
            string hello = client.Hello(msg);
            Debug.Log("Hello:" + hello);
        }
        
    }

    //public void GetUser()
    //{
    //    Debug.Log("->GetUser");
    //    client = GetClient();
    //    lock (client)
    //    {
    //        User user = client.GetUser();
    //        Debug.Log("Id:" + user.Id);
    //        Debug.Log("Name:" + user.Name);
    //    }
    //}

    //public void GetUsers()
    //{
    //    Debug.Log("->GetUsers");
    //    client = GetClient();
    //    lock (client)
    //    {
            
        
    //        User[] list = client.GetUsers();
    //        for (int i = 0; i < list.Length; i++)
    //        {
    //            Debug.Log("i:" + i);
    //            Debug.Log("Id:" + list[i].Id);
    //            Debug.Log("Name:" + list[i].Name);
    //        }
    //    }
    //}

    /// <summary>
    /// 获取定位卡相信息
    /// </summary>
    /// <returns></returns>
    public Tag[] GetTags()
    {
        //Debug.Log("->GetTags");
        client = GetClient();
        lock (client)
        {
            Tag[] list = client.GetTags();
            //for (int i = 0; i < list.Length; i++)
            //{
            //    Debug.Log("i:" + i);
            //    Debug.Log("Id:" + list[i].Id);
            //    Debug.Log("Name:" + list[i].Name);
            //}

            return list;
        }
    }

    //public Map[] GetMaps()
    //{
    //    client = GetClient();
    //    lock (client)
    //    {
    //        Debug.Log("->GetMaps");
    //        Map[] list = client.GetMaps(null);
    //        //List< Map > list= client.GetMaps(null);

    //        return list;
    //    }
    //}

    public Department GetDepTree()
    {
        client = GetClient();
        lock (client)
        {
            //Debug.Log("->GetDepTree");
            Department dep = client.GetDepartmentTree();
            //string txt = ShowDep(dep, 0);
            //Debug.Log(txt);
            return dep;
        }
    }

    public AreaNode GetPersonTree()
    {
        client = GetClient();
        lock (client)
        {
            Debug.Log("->GetPersonTree");
            int view = 2;//0:基本数据;1:基本设备信息;2:基本人员信息;3:基本设备信息+基本人员信息;4:只显示设备的节点;5:只显示人员的节点;6:只显示人员或设备的节点
            AreaNode root = client.GetPhysicalTopologyTreeNode(view);
            return root;
        }
    }

    /// <summary>
    /// 获取人员列表
    /// </summary>
    public Personnel[] GetPersonnels()
    {
        //Department topoRoot = GetDepTree();
        //return GetPersonnels(topoRoot);

        client = GetClient();
        lock (client)
        {
            return client.GetPersonList();
        }
    }

    /// <summary>
    /// 获取人员列表
    /// </summary>
    public static List<Personnel> GetPersonnels(Department topoRoot)
    {
        List<Personnel> personnelsT = new List<Personnel>();
        if (topoRoot == null) return personnelsT;
        if (topoRoot.Children == null) return personnelsT;
        foreach (Department child in topoRoot.Children)
        {
            if (child.LeafNodes != null)
            {
                personnelsT.AddRange(child.LeafNodes);
            }
            personnelsT.AddRange(GetPersonnels(child));
        }
        return personnelsT;
    }

    private string GetWhiteSpace(int count)
    {
        string space = "";
        for (int i = 0; i < count; i++)
        {
            space += "  ";
        }
        return space;
    }

    private string ShowDep(Department dep, int layer)
    {
        string whitespace = GetWhiteSpace(layer);
        string txt = whitespace + layer + ":" + dep.Name + "\n";
        if (dep.Children != null)
        {
            //txt+=whitespace + "length:" + dep.Children.Length+"\n";
            foreach (Department child in dep.Children)
            {
                txt += ShowDep(child, layer + 1);
            }
        }
        else
        {
            //txt += whitespace + "children==null\n";
        }
        return txt;
    }
    /// <summary>
    /// 获取定位卡位置信息
    /// </summary>
    /// <returns></returns>
    public List<TagPosition> GetTagsPosition()
    {
        client = GetClient();
        lock (client)
        {
            //Log.Info("GetTagsPosition Start");
            TagPosition[] arr = client.GetRealPositons();
            List<TagPosition> list = new List<TagPosition>();
            if (arr != null)
            {
                list.AddRange(arr);
            }
            //Log.Info("GetTagsPosition End");
            return list;
        }
    }
    /// <summary>
    /// 获取取定位卡历史位置信息
    /// </summary>
    /// <returns></returns>
    public List<Position> GetHistoryPositons()
    {
        client = GetClient();
        lock (client)
        {
            Position[] arr = client.GetHistoryPositons();
            List<Position> list = new List<Position>();
            if (arr != null)
            {
                list.AddRange(arr);
            }
            return list;
        }
    }

    /// <summary>
    /// 获取取定位卡历史位置信息,根据时间
    /// </summary>
    /// <returns></returns>
    public List<Position> GetHistoryPositonsByTime(string tagcode, DateTime start, DateTime end)
    {
        client = GetClient();
        lock (client)
        {
            Position[] arr = client.GetHistoryPositonsByTime(tagcode, start, end);
            List<Position> list = new List<Position>();
            if (arr != null)
            {
                list.AddRange(arr);
            }

            DateTime startTime = TimeZone.CurrentTimeZone.ToLocalTime(new DateTime(1970, 1, 1, 0, 0, 0, 0));
            foreach (Position p in list)
            {
                DateTime time = startTime.AddMilliseconds(p.Time);
                //print(time.ToString("yyyy/MM/dd HH:mm:ss:ffff"));
            }
            return list;
        }
    }

    /// <summary>
    /// 获取取定位卡历史位置信息,根据时间和人员Id
    /// </summary>
    /// <returns></returns>
    public List<Position> GetHistoryPositonsByPersonnelID(int personnelID, DateTime start, DateTime end)
    {
        client = GetClient();
        lock (client)
        {
            Position[] arr = client.GetHistoryPositonsByPersonnelID(personnelID, start, end);
            List<Position> list = new List<Position>();
            if (arr != null)
            {
                list.AddRange(arr);
            }

            DateTime startTime = TimeZone.CurrentTimeZone.ToLocalTime(new DateTime(1970, 1, 1, 0, 0, 0, 0));
            foreach (Position p in list)
            {
                DateTime time = startTime.AddMilliseconds(p.Time);
                //print(time.ToString("yyyy/MM/dd HH:mm:ss:ffff"));
            }
            return list;
        }
    }

    /// <summary>
    /// 获取取定位卡历史位置信息,根据时间和和TopoNodeId建筑id列表(人员所在的区域)
    /// </summary>
    /// <returns></returns>
    public List<Position> GetHistoryPositonsByPidAndTopoNodeIds(int personnelID,List<int> topoNodeIdsT, DateTime start, DateTime end)
    {
        client = GetClient();
        lock (client)
        {
            Position[] arr = client.GetHistoryPositonsByPidAndTopoNodeIds(personnelID, topoNodeIdsT.ToArray(), start, end);
            List<Position> list = new List<Position>();
            if (arr != null)
            {
                list.AddRange(arr);
            }

            DateTime startTime = TimeZone.CurrentTimeZone.ToLocalTime(new DateTime(1970, 1, 1, 0, 0, 0, 0));
            foreach (Position p in list)
            {
                DateTime time = startTime.AddMilliseconds(p.Time);
                //print(time.ToString("yyyy/MM/dd HH:mm:ss:ffff"));
            }
            return list;
        }
    }

    /// <summary>
    /// 获取大数据量测试
    /// </summary>
    public void GetStrsTest()
    {
        client = GetClient();
        lock (client)
        {
            DateTime datetimeStart = DateTime.Now;

            //string s = client.GetStrs(100000);
            DateTime datetimeEnd = DateTime.Now;
            float t = (float)(datetimeEnd - datetimeStart).TotalSeconds;
            Debug.LogError("GetStrsTest:" + t);
        }
    }

    ///// <summary>
    ///// 编辑区域
    ///// </summary>
    //public void EditArea(Area area)
    //{
    //    client = GetClient();
    //    lock (client)
    //    {
    //        client.EditArea(area);
    //    }
    //}

    /// <summary>
    /// 3D保存历史数据
    /// </summary>
    public bool AddU3DPosition(U3DPosition position)
    {
        try
        {
            client = GetClient();
            lock (client)//不用线程使用同一个client发送消息会相互干扰
            {
                //Debug.Log(string.Format("---------》》》》》》》{0}---------", position.Tag));
                Debug.Log(string.Format("[AddU3DPosition] Tag:{0}", position.Tag));
                List<U3DPosition> pList = new List<U3DPosition>();
                pList.Add(position);
                //client.AddU3DPosition(position);
                AddU3DPosition(pList);
                //Debug.Log(string.Format("---------《《《《《《《《{0}---------", position.Tag));
            }
            return true;
        }
        catch (Exception ex)
        {
            Debug.LogError(ex.ToString());
            return false;
        }

        //try
        //{
        //    //Debug.Log(string.Format("---------》》》》》》》{0}---------", position.Tag));
        //    Debug.Log(string.Format("[AddU3DPosition] Tag:{0}", position.Tag));
        //    LocationServiceClient c = CreateServiceClient();
        //    c.AddU3DPosition(position);
        //    //Debug.Log(string.Format("---------《《《《《《《《{0}---------", position.Tag));
        //    return true;
        //}
        //catch (Exception ex)
        //{
        //    Debug.LogError(ex.ToString());
        //    return false;
        //}
    }

    /// <summary>
    /// 3D保存历史数据
    /// </summary>
    public bool AddU3DPosition(List<U3DPosition> pList)
    {
        try
        {
            client = GetClient();
            lock (client)//不用线程使用同一个client发送消息会相互干扰
            {
                //Debug.Log(string.Format("---------》》》》》》》{0}---------", position.Tag));
                //Debug.Log(string.Format("[AddU3DPosition] Tag:{0}", position.Tag));
                client.AddU3DPositions(pList.ToArray());
                //Debug.Log(string.Format("---------《《《《《《《《{0}---------", position.Tag));
            }
            return true;
        }
        catch (Exception ex)
        {
            Thread.CurrentThread.Abort();
            Debug.LogError(ex.ToString());
            return false;
        }

        //try
        //{
        //    //Debug.Log(string.Format("---------》》》》》》》{0}---------", position.Tag));
        //    Debug.Log(string.Format("[AddU3DPosition] Tag:{0}", position.Tag));
        //    LocationServiceClient c = CreateServiceClient();
        //    c.AddU3DPosition(position);
        //    //Debug.Log(string.Format("---------《《《《《《《《{0}---------", position.Tag));
        //    return true;
        //}
        //catch (Exception ex)
        //{
        //    Debug.LogError(ex.ToString());
        //    return false;
        //}
    }

    /// <summary>
    ///  获取标签3D历史位置
    /// </summary>
    /// <param name="tagcode"></param>
    /// <param name="start"></param>
    /// <param name="end"></param>
    /// <returns></returns>
    public List<U3DPosition> GetHistoryU3DPositonsByTime(string tagcode, DateTime start, DateTime end)
    {
        client = GetClient();
        lock (client)
        {
            U3DPosition[] arr = client.GetHistoryU3DPositonsByTime(tagcode, start, end);
            List<U3DPosition> list = new List<U3DPosition>();
            if (arr != null)
            {
                list.AddRange(arr);
            }

            DateTime startTime = TimeZone.CurrentTimeZone.ToLocalTime(new DateTime(1970, 1, 1, 0, 0, 0, 0));
            foreach (U3DPosition p in list)
            {
                DateTime time = startTime.AddMilliseconds(p.Time);
                print(time.ToString("yyyy/MM/dd HH:mm:ss:ffff"));
            }
            return list;
        }
    }
    /// <summary>
    /// 获取模型添加列表
    /// </summary>
    public List<ObjectAddList_Type> GetModelAddList()
    {
        client = GetClient();
        lock (client)
        {
            ObjectAddList_Type[] TypeList = client.GetObjectAddList();
            if (TypeList == null)
            {
                Log.Error("GetModelAddList TypeList == null");
                return null;
            }
            return TypeList.ToList();
        }
    }
    /// <summary>
    /// 删除设备信息
    /// </summary>
    /// <param name="dev"></param>
    public void DeleteDevInfo(DevInfo dev)
    {
        client = GetClient();
        lock (client)
        {
            bool value = client.DeleteDevInfo(dev);
            Debug.Log("DeleteDevInfo result:" + value);
        }
    }
    /// <summary>
    /// 保存设备信息
    /// </summary>
    public void AddDevInfo(ref DevInfo dev)
    {
        client = GetClient();
        lock (client)
        {
            //DevInfo value = client.AddDevInfo(dev);
            dev = client.AddDevInfo(dev);
        }
    }
    /// <summary>
    /// 添加设备
    /// </summary>
    /// <param name="devInfos"></param>
    public void AddDevInfo(List<DevInfo>devInfos)
    {
        client = GetClient();
        lock (client)
        {
            DevInfo[] devs = devInfos.ToArray();
            bool value = client.AddDevInfoByList(devs);
            Debug.Log("AddDevInfoByList result:" + value);
        }
    }
    /// <summary>
    /// 修改设备信息
    /// </summary>
    /// <param name="Info"></param>
    public void ModifyDevInfo(DevInfo Info)
    {
        client = GetClient();
        lock (client)
        {
            bool value = client.ModifyDevInfo(Info);
            Debug.Log("ModifyDevInfo result:" + value);
        }
    }
    /// <summary>
    /// 修改设备位置信息
    /// </summary>
    /// <param name="PosInfo"></param>
    public void ModifyDevPos(DevPos PosInfo)
    {
        client = GetClient();
        lock (client)
        {
            bool value = client.ModifyPosInfo(PosInfo);
            //Debug.Log("ModifyDevPos result:" + value);
        }
    }
    /// <summary>
    /// 修改设备位置信息
    /// </summary>
    /// <param name="posList"></param>
    public void ModifyDevPosByList(List<DevPos> posList)
    {
        client = GetClient();
        lock (client)
        {
            bool value = client.ModifyPosByList(posList.ToArray());
            Debug.Log("ModifyDevPos result:" + value);
        }
    }
    /// <summary>
    /// 添加设备位置信息
    /// </summary>
    /// <param name="devPos"></param>
    public void AddDevPosInfo(DevPos devPos)
    {
        client = GetClient();
        lock (client)
        {
            bool value = client.AddDevPosInfo(devPos);
            Debug.Log("AddDevPos result:" + value);
        }
    }
    /// <summary>
    /// 获取所有设备信息
    /// </summary>
    /// <returns></returns>
    public List<DevInfo> GetAllDevInfos()
    {
        client = GetClient();
        lock (client)
        {
            DevInfo[] infoList = client.GetAllDevInfos();
            if (infoList == null) return new List<DevInfo>();
            return infoList.ToList();
        }
    }
    /// <summary>
    /// 获取所有设备信息
    /// </summary>
    /// <returns></returns>
    public List<DevInfo> GetDevInfos(int[] typeList)
    {
        client = GetClient();
        lock (client)
        {
            DevInfo[] infoList = client.GetDevInfos(typeList);
            if (infoList == null) return new List<DevInfo>();
            return infoList.ToList();
        }
    }
    /// <summary>
    /// 通过设备Id,获取设备信息
    /// </summary>
    /// <param name="devId"></param>
    public DevInfo GetDevByDevId(string devId)
    {
        client = GetClient();
        lock (client)
        {
            DevInfo devInfo = client.GetDevByID(devId);
            return devInfo;
        }
    }

    /// <summary>
    /// 通过设备Id,获取设备信息
    /// </summary>
    /// <param name="devId"></param>
    public DevInfo GetDevByid(int id)
    {
        client = GetClient();
        lock (client)
        {
            DevInfo devInfo = client.GetDevByiId(id);
            return devInfo;
        }
    }
    
    /// <summary>
    /// 获取所有设备信息
    /// </summary>
    /// <returns></returns>
    public List<DevInfo> FindDevInfos(string key)
    {
        client = GetClient();
        lock (client)
        {
            DevInfo[] infoList = client.FindDevInfos(key);
            if (infoList == null) return new List<DevInfo>();
            return infoList.ToList();
        }
    }
    /// <summary>
    /// 获取所有设备,位置信息
    /// </summary>
    /// <returns></returns>
    public List<DevPos> GetAllPosInfo()
    {
        client = GetClient();
        lock (client)
        {
            DevPos[] infoList = client.GetDevPositions();
            if (infoList == null) return null;
            return infoList.ToList();
        }
    }
    public List<DevInfo> GetDevInfoByParent(List<int> idList)
    {
        client = GetClient();
        lock (client)
        {
            DevInfo[] infoList = client.GetDevInfoByParent(idList.ToArray());
            if (infoList == null)
            {
                return new List<DevInfo>();
            }
            return infoList.ToList();
        }
    }

    /// <summary>
    /// 获取园区下的监控范围
    /// </summary>
    /// <returns></returns>
    public IList<PhysicalTopology> GetParkMonitorRange()
    {
        client = GetClient();
        lock (client)
        {
            IList<PhysicalTopology> results = client.GetParkMonitorRange();
            return results;
        }
    }

    /// <summary>
    /// 获取楼层下的监控范围
    /// </summary>
    /// <returns></returns>
    public IList<PhysicalTopology> GetFloorMonitorRange()
    {
        client = GetClient();
        lock (client)
        {
            IList<PhysicalTopology> results = client.GetFloorMonitorRange();
            return results;
        }
    }

    /// <summary>
    /// 根据PhysicalTopology的Id获取楼层以下级别的监控范围
    /// </summary>
    /// <returns></returns>
    public IList<PhysicalTopology> GetFloorMonitorRangeById(int id)
    {
        client = GetClient();
        lock (client)
        {
            IList<PhysicalTopology> results = client.GetFloorMonitorRangeById(id);
            return results;
        }
    }

    /// <summary>
    /// 根据节点添加监控范围
    /// </summary>
    public bool EditMonitorRange(PhysicalTopology pt)
    {

        client = GetClient();
        lock (client)
        {
            return client.EditMonitorRange(pt);
        }
    }

    /// <summary>
    /// 根据节点添加子监控范围
    /// </summary>
    public bool AddMonitorRange(PhysicalTopology pt)
    {
        client = GetClient();
        lock (client)
        {
            return client.AddMonitorRange(pt);
        }
    }

    /// <summary>
    /// 获取配置信息
    /// </summary>
    /// <returns></returns>
    public ConfigArg GetConfigArgByKey(string key)
    {
        client = GetClient();
        lock (client)
        {
            return client.GetConfigArgByKey(key);
        }
    }

    /// <summary>
    /// 设置配置信息
    /// </summary>
    /// <returns></returns>
    public bool GetConfigArgByKey(ConfigArg config)
    {
        client = GetClient();
        lock (client)
        {
            return client.EditConfigArg(config);
        }
    }

    /// <summary>
    /// 获取坐标系转换配置信息
    /// </summary>
    /// <returns></returns>
    public TransferOfAxesConfig GetTransferOfAxesConfig()
    {
        client = GetClient();
        lock (client)
        {
            return client.GetTransferOfAxesConfig();
        }
    }

    /// <summary>
    /// 设置坐标系转换配置信息
    /// </summary>
    /// <param name="config"></param>
    /// <returns></returns>
    public bool SetTransferOfAxesConfig(TransferOfAxesConfig config)
    {
        client = GetClient();
        lock (client)
        {
            return client.SetTransferOfAxesConfig(config);
        }
    }
    /// <summary>
    /// 人员搜索（CaiCai）
    /// </summary>
    /// <param name="key"></param>
    /// <returns></returns>
    public List<Personnel> FindPersonnelData(string key)
    {
        client = GetClient();
        lock (client)
        {
            Personnel[] infoList = client.FindPersonList(key);
            if (infoList == null) return new List<Personnel>();
            return infoList.ToList();
        }
    }
    public Post[] GetPostList()
    {
        client = GetClient();
        lock (client)
        {
            return client.GetPostList();
        }
    }

    public LocationAlarm[] GetLocationAlarms(AlarmSearchArg arg)
    {
        client = GetClient();
        lock (client)
        {
            return client.GetLocationAlarms(arg);
        }
    }

    public DeviceAlarm[] GetDeviceAlarms(AlarmSearchArg arg)
    {
        client = GetClient();
        lock (client)
        {
            return client.GetDeviceAlarms(arg);
        }
    }
    /// <summary>
    /// 园区信息统计
    /// </summary>
    /// <param name="Id"></param>
    /// <returns></returns>
    public AreaStatistics GetAreaStatistics(int Id)
    {
        client = GetClient();
        lock (client)
        {
            return client.GetAreaStatistics(Id);
        }
        
    }
    /// <summary>
    /// 附近人员
    /// </summary>
    /// <param name="Id"></param>
    /// <returns></returns>
    public NearbyPerson [] GetNearbyPerson_Currency(int Id,float distance )
    {
        client = GetClient();
        lock (client)
        {  
            return client.GetNearbyPerson_Currency(Id,distance );
        }
    }
   public NearbyDev[] GetNearbyDev_Currency(int id,float distance,int nFlag)
    {
        client = GetClient();
        lock (client)
        {
            return client.GetNearbyDev_Currency(id ,distance , nFlag);
        }
    }
    /// <summary>
    /// 增加门禁设备
    /// </summary>
    /// <param name="doorAccessList"></param>
    /// <returns></returns>
    public bool AddDoorAccess(List<Dev_DoorAccess> doorAccessList)
    {
        client = GetClient();
        lock (client)
        {
            return client.AddDoorAccessByList(doorAccessList.ToArray());
        }
    }
    /// <summary>
    /// 添加门禁信息
    /// </summary>
    /// <param name="doorAccess"></param>
    /// <returns></returns>
    public bool AddDoorAccess(Dev_DoorAccess doorAccess)
    {
        client = GetClient();
        lock (client)
        {
            return client.AddDoorAccess(doorAccess);
        }
    }
    /// <summary>
    /// 删除门禁设备
    /// </summary>
    /// <param name="doorAccessList"></param>
    /// <returns></returns>
    public bool DeleteDoorAccess(List<Dev_DoorAccess> doorAccessList)
    {
        client = GetClient();
        lock (client)
        {
            return client.DeleteDoorAccess(doorAccessList.ToArray());
        }
    }
    /// <summary>
    /// 修改门禁信息
    /// </summary>
    /// <param name="doorAccessList"></param>
    /// <returns></returns>
    public bool ModifyDoorAccess(List<Dev_DoorAccess> doorAccessList)
    {
        client = GetClient();
        lock (client)
        {
            return client.ModifyDoorAccess(doorAccessList.ToArray());
        }
    }
    /// <summary>
    /// 通过区域ID，获取区域下门禁信息
    /// </summary>
    /// <param name="pidList"></param>
    /// <returns></returns>
    public List<Dev_DoorAccess> GetDoorAccessInfoByParent(List<int> pidList)
    {
        client = GetClient();
        lock (client)
        {
            List<Dev_DoorAccess> doorList;
            Dev_DoorAccess[] doors = client.GetDoorAccessInfoByParent(pidList.ToArray());
            if (doors == null) doorList = new List<Dev_DoorAccess>();
            else doorList = doors.ToList();
            return doorList;
        }
    }
    #region 基站编辑
    /// <summary>
    /// 获取所有基站信息
    /// </summary>
    /// <returns></returns>
    public List<Archor> GetArchors()
    {
        client = GetClient();
        lock (client)
        {

            List<Archor> archors = client.GetArchors().ToList();
            return archors;
        }
    }

    public Archor GetArchor(string archorId)
    {
        client = GetClient();
        lock (client)
        {
           
            Archor archor = client.GetArchor(archorId);
            return archor;
        }
    }

    public Archor GetArchorByDevId(int devId)
    {
        client = GetClient();
        lock (client)
        {

            Archor archor = client.GetArchorByDevId(devId);
            return archor;
        }
    }
    public bool AddArchor(Archor archor)
    {
        client = GetClient();
        lock (client)
        {
            return client.AddArchor(archor);
        }
    }
    public bool EditArchor(Archor archor, int parentId)
    {
        client = GetClient();
        lock (client)
        {
            bool value = client.EditArchor(archor, parentId);
            return value;
        }
    }
    public void DeleteArchor(int archorId)
    {
        client = GetClient();
        lock (client)
        {
            client.DeleteArchor(archorId);
        }
    }
    public bool EditBusAnchor(Archor busArchor, int parentId)
    {
        client = GetClient();
        lock (client)
        {
            bool value = client.EditArchor(busArchor, parentId);
            return value;
        }
    }
    #endregion
    #region 两票移动巡检

    /// <summary>
    ///  获取操作票列表
    /// </summary>
    /// <returns></returns>

    public List<OperationTicket> GetOperationTicketList()
    {
        client = GetClient();
        lock (client)
        {
            return client.GetOperationTicketList().ToList();
        }
        //var operationTickets = db.OperationTickets.ToList();
        //return operationTickets.ToWcfModelList();
    }

    /// <summary>
    /// 获取工作票列表
    /// </summary>
    /// <returns></returns>
    public List<WorkTicket> GetWorkTicketList()
    {
        client = GetClient();
        lock (client)
        {
            return client.GetWorkTicketList().ToList();
        }
    }

    /// <summary>
    /// 获取巡检设备列表
    /// </summary>
    public List<MobileInspectionDev> GetMobileInspectionDevList()
    {
        client = GetClient();
        lock (client)
        {
            return client.GetMobileInspectionDevList().ToList();
        };
    }

    /// <summary>
    /// 获取巡检轨迹列表
    /// </summary>
    /// <returns></returns>
    public List<MobileInspection> GetMobileInspectionList()
    {
        client = GetClient();
        lock (client)
        {
            return client.GetMobileInspectionList().ToList();
        };
    }

    /// <summary>
    /// 获取人员巡检轨迹列表
    /// </summary>
    /// <returns></returns>
    public List<PersonnelMobileInspection> GetPersonnelMobileInspectionList()
    {
        client = GetClient();
        lock (client)
        {
            PersonnelMobileInspection[] ps = client.GetPersonnelMobileInspectionList();
            if (ps == null)
            {
                List<PersonnelMobileInspection> pList = new List<PersonnelMobileInspection>();
                return pList;
            }
            else
            {
                return ps.ToList();
            }
        };
    }

    /// <summary>
    /// 获取操作票历史记录
    /// </summary>
    /// <returns></returns>
    public List<OperationTicketHistory> GetOperationTicketHistoryList()
    {
        client = GetClient();
        lock (client)
        {
            return client.GetOperationTicketHistoryList().ToList();
        };
    }

    /// <summary>
    /// 获取工作票历史记录
    /// </summary>
    /// <returns></returns>
    public List<WorkTicketHistory> GetWorkTicketHistoryList()
    {
        client = GetClient();
        lock (client)
        {
            return client.GetWorkTicketHistoryList().ToList();
        };
    }

    /// <summary>
    /// 获取人员巡检轨迹历史记录
    /// </summary>
    /// <returns></returns>
    public List<PersonnelMobileInspectionHistory> GetPersonnelMobileInspectionHistoryList()
    {
        client = GetClient();
        lock (client)
        {
            PersonnelMobileInspectionHistory[] ps = client.GetPersonnelMobileInspectionHistoryList();
            if (ps == null)
            {
                List<PersonnelMobileInspectionHistory> pList = new List<PersonnelMobileInspectionHistory>();
                return pList;
            }
            else
            {
                return ps.ToList();
            }
            //return client.GetPersonnelMobileInspectionHistoryList().ToList();
        };
    }

    #endregion
    #region 顶视图截图

    /// <summary>
    /// 编辑图片
    /// </summary>
    /// <param name="picture"></param>
    /// <returns></returns>
    public bool EditPictureInfo(Picture picture)
    {
        client = GetClient();
        lock (client)
        {           
            bool value = client.EditPictureInfo(picture);
            return value;
        }
    }
    /// <summary>
    /// 获取图片信息
    /// </summary>
    /// <param name="pictureName"></param>
    /// <returns></returns>
    public Picture GetPictureInfo(string pictureName)
    {
        client = GetClient();
        lock (client)
        {
            Picture value = client.GetPictureInfo(pictureName);
            return value;
        }
    }
    #endregion
}
