﻿using Location.WCFServiceReferences.LocationServices;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UIWidgets;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class MultHistoryPlayUI : MonoBehaviour
{

    public static MultHistoryPlayUI Instance;
    /// <summary>
    /// 窗体
    /// </summary>
    public GameObject window;
    //日期
    public Text dayTxt;
    //关闭按钮
    public Button closeBtn;
    //是否播放
    public bool isPlay;
    //是否停止（不是暂停）
    public bool isStop;
    //停止按钮
    public Button StopBtn;

    public CalendarChange calendar;

    [HideInInspector]
    public float timeStart;    //时间起始播放值
    public DateTime datetimeStart;    //时间起始播放值
    public double timeLength;    //播放时间,单位秒
    public double timeSum;//时间和
    /// <summary>
    /// 人员信息数据
    /// </summary>
    private Personnel personnel;
    /// <summary>
    /// 如果加载数据成功
    /// </summary>
    public bool isLoadDataSuccessed;
    [HideInInspector]
    public float CurrentSpeed = 1;//当前播放速度倍数
    public bool IsMouseDragSlider = false;//鼠标拖动进度条

    private float progressbarLoadValue = 0;//数据加载进度条

    Dictionary<Personnel, List<Position>> personnel_Points;

    // Use this for initialization
    void Start()
    {
        Instance = this;
        closeBtn.onClick.AddListener(CloseBtn_OnClick);
        playBtn.onClick.AddListener(PlayBtn_OnClick);
        playBtn.onClick.AddListener(ChangeButtonSprite);
        StopBtn.onClick.AddListener(Stop);
        rateBtn.onClick.AddListener(RateBtn_OnClick);
        calendar.onDayClick.AddListener(Calendar_onDayClick);

        slider.OnValuesChange.AddListener(RangeSliderChanged);
        processSlider.onValueChanged.AddListener(ProcessSlider_ValueChanged);

        editPersonBtn.onClick.AddListener(EditPersonBtn_OnClick);
    }

    float m;

    // Update is called once per frame
    void Update()
    {
        //UGUI类似按钮的控件才能被选中
        if (EventSystem.current.currentSelectedGameObject != null && Input.GetMouseButton(0))
        {
            //print("currentSelectedGameObject:" + EventSystem.current.currentSelectedGameObject.name);
            if (!isStop)
            {
                Slider selectSlider = EventSystem.current.currentSelectedGameObject.GetComponentInParent<Slider>();
                if (selectSlider == processSlider)
                {
                    IsMouseDragSlider = true;
                }
                else
                {
                    IsMouseDragSlider = false;
                }
            }
        }
        else
        {
            IsMouseDragSlider = false;
        }

        m += Time.deltaTime;
        if (isPlay && isLoadDataSuccessed)
        {
            if (!IsMouseDragSlider)
            {
                //float timeT = Time.time - timeStart;
                timeSum += Time.deltaTime * CurrentSpeed;
                float valueT = (float)(timeSum / timeLength);
                SetProcessSliderValue(valueT);
                SetProcessCurrentTime((float)timeSum);
                //print("Time.time:" + Time.time);
                //print("Time.fixedTime:" + Time.fixedTime);
                //print("计算:" + m);
                //print("Time.deltaTime:" + Time.deltaTime);
                //print("Time.fixedDeltaTime:" + Time.fixedDeltaTime);
                if (processSlider.value >= 1)//播放结束
                {
                    ExecuteEvents.Execute<IPointerClickHandler>(StopBtn.gameObject, new PointerEventData(EventSystem.current), ExecuteEvents.pointerClickHandler);
                }
            }
            else
            {
                timeSum = timeLength * processSlider.value;
                //print(string.Format("HistoryPlayUI进度时间:{0},进度值:{1}", timeSum, processSlider.value));
                SetProcessCurrentTime((float)timeSum);
                SetHistoryPath();
            }
        }


    }

    /// <summary>
    /// 设置历史轨迹执行的值
    /// </summary>
    private void SetHistoryPath()
    {
        LocationHistoryManager.Instance.SetHistoryPath_M(processSlider.value);
    }

    [ContextMenu("On_Click1")]
    public void On_Click1()
    {
        ExecuteEvents.Execute<IPointerClickHandler>(StopBtn.gameObject, new PointerEventData(EventSystem.current), ExecuteEvents.pointerClickHandler);
    }
    [ContextMenu("On_Click2")]
    public void On_Click2()
    {
        //按钮点击的变色
        ExecuteEvents.Execute<ISubmitHandler>(StopBtn.gameObject, new PointerEventData(EventSystem.current), ExecuteEvents.submitHandler);
    }

    /// <summary>
    /// 初始化
    /// </summary>
    /// <param name="personnelT"></param>
    public void Init(Personnel personnelT)
    {
        personnel = personnelT;
    }

    /// <summary>
    /// 展示
    /// </summary>
    public void ShowT()
    {
        FunctionSwitchBarManage.Instance.SetTransparentToggle(true);
        PersonnelTreeManage.Instance.CloseWindow();
        SmallMapController.Instance.Hide();
        LocationManager.Instance.HideLocation();
        StartOutManage.Instance.Hide();
        LocationManager.Instance.RecoverBeforeFocusAlign();
        FactoryDepManager.Instance.SetAllColliderIgnoreRaycast(true);
        SetWindowActive(true);
    }

    /// <summary>
    /// 展示
    /// </summary>
    /// <param name="personnelT"></param>
    public void Show(Personnel personnelT)
    {
        SetIsStop(true);
        Init(personnelT);
        //ShowBaseInfo(personnelT.Name, personnelT.Pst.Name);
        SetWindowActive(true);
    }

    /// <summary>
    /// 关闭
    /// </summary>
    public void Hide()
    {
        if (window.activeInHierarchy)
        {
            FunctionSwitchBarManage.Instance.SetTransparentToggle(false);
            FunctionSwitchBarManage.Instance.SetlightToggle(true);
            Stop();
            SetWindowActive(false);
            PersonnelTreeManage.Instance.ShowWindow();
            SmallMapController.Instance.Show();
            StartOutManage.Instance.Show();
            LocationManager.Instance.ShowLocation();
            //ControlMenuController.Instance.ShowLocation();
            //LocationManager.Instance.RecoverBeforeFocusAlign();
            FactoryDepManager.Instance.SetAllColliderIgnoreRaycast(false);
        }
    }

    public void CloseBtn_OnClick()
    {
        PersonSubsystemManage.Instance.SetMultHistoryToggle(false);
    }

    /// <summary>
    /// 设置窗口Active
    /// </summary>
    public void SetWindowActive(bool b)
    {
        window.SetActive(b);
    }

    /// <summary>
    /// 播放历史
    /// </summary>
    public void Play()
    {
        SetIsStop(false);
        isPlay = !isPlay;
        if (isPlay)
        {
            timeLength = (slider.ValueMax - slider.ValueMin) * 3600;
            //bool b = LocationManager.Instance.IsCreateHistoryPath(personnel.Tag.Code);
            //if (!b)
            //{
            ShowHistoryData();
            //}
            FunctionSwitchBarManage.Instance.SetlightToggle(false);
        }
    }

    ///// <summary>
    ///// 暂停历史
    ///// </summary>
    //public void Pause()
    //{

    //}

    /// <summary>
    /// 停止历史(终止)
    /// </summary>
    public void Stop()
    {
        Debug.LogError("Stop!");
        RangeSliderChanged(slider.ValueMin, slider.ValueMax);
        //RefleshProcessSlider("00:00:00");
        //ClearHistoryPath();
        isLoadDataSuccessed = false;
        timeSum = 0;
        SetIsStop(true);
    }

    public bool IsLineTest = false;

    [ContextMenu("SetIsLineTest")]
    public void SetIsLineTest()
    {
        IsLineTest = !IsLineTest;
    }

    /// <summary>
    /// 显示某个标签的历史数据
    /// </summary>
    /// <param name="code"></param>
    public void ShowHistoryData()
    {
        if (isLoadDataSuccessed) return;
        //LocationManager.Instance.ClearHistoryPaths();
        //string code = "0002";

        List<HistoryPersonUIItem> historyPersonUIItems = personsGrid.GetComponentsInChildren<HistoryPersonUIItem>().ToList();

        DateTime end = GetEndTime();
        DateTime start = GetStartTime();
        List<List<Position>> psList = new List<List<Position>>();
        personnel_Points = new Dictionary<Personnel, List<Position>>();
        //List<Position> ps = new List<Position>();
        List<LocationHistoryPath_M> paths = new List<LocationHistoryPath_M>();

        progressbarLoadValue = 0;

        List<int> topoNodeIds = RoomFactory.Instance.GetCurrentDepNodeChildNodeIds(SceneEvents.DepNode);

        Loom.StartSingleThread(() =>
        {
            foreach (Personnel p in currentPersonnels)
            {
                List<Position> ps = GetHistoryData(p.Id, topoNodeIds, start, end);
                psList.Add(ps);
                if (personnel_Points.ContainsKey(p))
                {
                    personnel_Points[p] = ps;
                }
                else
                {
                    personnel_Points.Add(p, ps);
                }
            }
            Debug.Log("StartSingleThread1");
            Loom.DispatchToMainThread(() =>
            {
                ProgressbarLoad.Instance.Show(1);
                ProgressbarLoad.Instance.Hide();
                int k = 0;
                foreach (Personnel p in personnel_Points.Keys)
                {
                    List<Position> ps = personnel_Points[p];
                    Debug.LogError("点数：" + ps.Count);
                    if (ps.Count < 2) continue;
                    List<Vector3> pointlist = new List<Vector3>();
                    List<DateTime> timelist = new List<DateTime>();
                    for (int i = 0; i < ps.Count; i++)
                    {
                        Position pointT = ps[i];
                        Vector3 tempVector3 = new Vector3((float)pointT.X, (float)pointT.Y, (float)pointT.Z);
                        tempVector3 = LocationManager.GetRealVector(tempVector3);
                        pointlist.Add(tempVector3);
                        //DateTime t = LocationManager.GetTimestampToDateTime(pointT.Time);
                        DateTime t = pointT.DateTime;
                        timelist.Add(t);
                    }

                    Color colorT = colors[k % colors.Count];
                    HistoryPersonUIItem item = historyPersonUIItems.Find((i) => i.personnel.Id == p.Id);
                    if (item != null)
                    {
                        colorT = item.color;
                    }
                    
                    LocationHistoryPath_M histoyObj = LocationHistoryManager.Instance.ShowLocationHistoryPath_M(p, pointlist, pointlist.Count, colorT);
                    histoyObj.InitData(timeLength, timelist);
                    HistoryManController historyManController = histoyObj.gameObject.AddComponent<HistoryManController>();
                    historyManController.Init(colorT);
                    Debug.Log("StartSingleThread2");
                    k++;
                }
            });

            //Debug.Log("StartSingleThread3");
            Loom.DispatchToMainThread(() =>
            {
                isLoadDataSuccessed = true;
                //timeStart = Time.time;
                timeSum = 0;
                Debug.Log("StartSingleThread3");
            });

        });
    }

    /// <summary>
    /// 获取历史轨迹数据
    /// </summary>
    /// <param name="ps"></param>
    /// <param name="startT"></param>
    /// <param name="end"></param>
    /// <param name="intervalMinute">每次获取数据的时间长度，不超过改数值</param>
    public List<Position> GetHistoryData(int personnelID, List<int> topoNodeIdsT, DateTime startT, DateTime endT, float intervalMinute = 10f)
    {
        List<Position> ps = new List<Position>();
        double minutes = (endT - startT).TotalMinutes;
        float counts = (float)minutes / intervalMinute;
        //float valueT = 0;
        float sum = 0;
        while (sum < counts)
        {
            DateTime startTemp;
            DateTime endTemp;
            if (sum + 1 <= counts)
            {
                startTemp = startT.AddMinutes(intervalMinute * sum);
                endTemp = startT.AddMinutes(intervalMinute * (sum + 1));
                //List<Position> listT = CommunicationObject.Instance.GetHistoryPositonsByTime(code, startTemp, endTemp);
                List<Position> listT = CommunicationObject.Instance.GetHistoryPositonsByPersonnelID(personnelID, startTemp, endTemp);
                //List<Position> listT = CommunicationObject.Instance.GetHistoryPositonsByPidAndTopoNodeIds(personnelID, topoNodeIdsT, startTemp, endTemp);
                ps.AddRange(listT);
            }
            else
            {
                startTemp = startT.AddMinutes(intervalMinute * sum);
                endTemp = endT;
                List<Position> listT = CommunicationObject.Instance.GetHistoryPositonsByPersonnelID(personnelID, startTemp, endTemp);
                //List<Position> listT = CommunicationObject.Instance.GetHistoryPositonsByPidAndTopoNodeIds(personnelID, topoNodeIdsT, startTemp, endTemp);
                ps.AddRange(listT);
            }

            sum += 1;
            float valueT = 1f / counts;
            progressbarLoadValue += valueT / currentPersonnels.Count;
            //print("valueT:" + valueT);
            Loom.DispatchToMainThread(() =>
            {
                ProgressbarLoad.Instance.Show(progressbarLoadValue);
            });
        }
        //Loom.DispatchToMainThread(() =>
        //{
        //    ProgressbarLoad.Instance.Hide();
        //});
        return ps;
    }

    /// <summary>
    /// 获取人员历史轨迹数据根据人员信息
    /// </summary>
    public List<Position> GetPositionsByPersonnel(Personnel p)
    {
        if (personnel_Points.ContainsKey(p))
        {
            return personnel_Points[p];
        }
        else
        {
            return null;
        }
    }

    /// <summary>
    /// 清空历史轨迹
    /// </summary>
    public void ClearHistoryPath()
    {
        LocationHistoryManager.Instance.ClearHistoryPaths_M();
    }


    #region 选择时间Slider
    //RangeSlider
    public RangeSlider slider;
    /// <summary>
    /// 获取起始时间
    /// </summary>
    [ContextMenu("GetStartTime")]
    public DateTime GetStartTime()
    {
        //DateTime starttime = Convert.ToDateTime("2018年8月10日");//8/10/2018 12:00:00 AM,就是10日早上0点
        DateTime starttime = Convert.ToDateTime(dayTxt.text);
        starttime = starttime.AddHours(slider.ValueMin);

        //slider.ValueMin
        return starttime;
    }

    /// <summary>
    /// 获取结束时间
    /// </summary>
    public DateTime GetEndTime()
    {
        //DateTime endtime = Convert.ToDateTime("2018年8月10日");//8/10/2018 12:00:00 AM,就是10日早上0点
        DateTime endtime = Convert.ToDateTime(dayTxt.text);
        endtime = endtime.AddHours(slider.ValueMax);
        //slider.ValueMax
        return endtime;
    }

    /// <summary>
    /// 时间范围改变
    /// </summary>
    public void Calendar_onDayClick(DateTime dateTime)
    {
        RangeSliderChanged(slider.ValueMin, slider.ValueMax);
    }

    /// <summary>
    /// 时间范围改变
    /// </summary>
    public void RangeSliderChanged(int min, int max)
    {
        int s = max - min;
        string timestr = s.ToString();
        if (s < 9)
        {
            timestr = "0" + s + ":00:00";
        }
        else
        {
            timestr = s + ":00:00";
        }
        RefleshProcessSlider(timestr);
        isLoadDataSuccessed = false;
    }


    #endregion

    #region 播放按钮
    //播放按钮
    public Button playBtn;
    //播放进度条
    public Slider processSlider;
    //进度当前时间
    public Text processCurrentTime;
    //进度结束时间
    public Text processEndTime;
    //播放按钮相关图片信息
    public HistoryPlayUI_ButtonEntity playButtonEty;
    //暂停按钮相关图片信息
    public HistoryPlayUI_ButtonEntity pauseButtonEty;

    /// <summary>
    /// 播放按钮触发事件
    /// </summary>
    public void PlayBtn_OnClick()
    {
        if (currentPersonnels == null || currentPersonnels.Count == 0)
        {
            UGUIMessageBox.Show("请先添加人员！",
                ()=>
                {
                    EditPersonBtn_OnClick();
                }, null);
        }
        else
        {
            Play();
        }
    }

    /// <summary>
    /// 改变按钮图片
    /// </summary>
    public void ChangeButtonSprite()
    {
        if (isPlay)
        {
            playBtn.image.sprite = pauseButtonEty.sprite;
            SetButtonHighlightSprite(playBtn, pauseButtonEty.highlightedSprite);
        }
        else
        {
            playBtn.image.sprite = playButtonEty.sprite;
            SetButtonHighlightSprite(playBtn, playButtonEty.highlightedSprite);
        }
    }

    private void SetProcessCurrentTime(float timeT)
    {
        DateTime currentTime = GetStartTime().AddSeconds(timeT);
        string currentTimestr1 = currentTime.AddHours(-slider.ValueMin).ToString("HH:mm:ss");
        string currentTimestr2 = currentTime.ToString("HH:mm:ss");
        string currentTimestr = currentTimestr1 + "(" + currentTimestr2 + ")";
        SetProcessCurrentTime(currentTimestr);
    }

    /// <summary>
    /// 设置进度条结束时间
    /// </summary>
    public void SetProcessCurrentTime(string timestr)
    {
        processCurrentTime.text = timestr;
    }

    /// <summary>
    /// 设置进度条当前执行时间
    /// </summary>
    public void SetProcessEndTime(string timestr)
    {
        processEndTime.text = "/" + timestr;
    }

    /// <summary>
    /// 刷新时间进度条
    /// </summary>
    public void RefleshProcessSlider(string timeEndstr)
    {
        SetProcessEndTime(timeEndstr);
        processSlider.value = 0;
        SetProcessCurrentTime("00:00:00");
        if (isPlay)
        {
            //如果在播放就让它暂停
            ExecuteEvents.Execute<IPointerClickHandler>(playBtn.gameObject, new PointerEventData(EventSystem.current), ExecuteEvents.pointerClickHandler);
        }
        ClearHistoryPath();
        SetIsStop(true);
    }
    /// <summary>
    /// 设置进度条值改变
    /// </summary>
    public void SetProcessSliderValue(float v)
    {
        processSlider.value = v;
    }

    public void ProcessSlider_ValueChanged(float v)
    {
        if (IsMouseDragSlider)
        {
            //print("ProcessSlider_ValueChanged");
        }
    }

    #endregion

    /// <summary>
    /// 设置按钮的高亮图片
    /// </summary>
    /// <param name="btn"></param>
    /// <param name="pauseHighligterSprite"></param>
    private void SetButtonHighlightSprite(Button btn, Sprite pauseHighligterSprite)
    {
        SpriteState ss = new SpriteState();
        ss.disabledSprite = btn.spriteState.disabledSprite;
        ss.highlightedSprite = pauseHighligterSprite;
        ss.pressedSprite = btn.spriteState.pressedSprite;
        btn.spriteState = ss;
    }

    #region 倍数按钮
    //速率按钮
    public Button rateBtn;
    //相关倍数按钮的信息列表
    public List<HistoryPlayUI_SpeedButton> rateButtonEtys;
    [HideInInspector]
    public int rateIndex = 0;//当前倍数按钮索引

    /// <summary>
    /// 速率按钮触发事件
    /// </summary>
    public void RateBtn_OnClick()
    {
        rateIndex = rateIndex + 1;
        if (rateIndex >= rateButtonEtys.Count)
        {
            rateIndex = 0;
        }
        rateBtn.image.sprite = rateButtonEtys[rateIndex].sprite;
        CurrentSpeed = rateButtonEtys[rateIndex].speed;
        SetButtonHighlightSprite(rateBtn, rateButtonEtys[rateIndex].highlightedSprite);
    }

    #endregion

    #region 播放历史轨迹界面的人员列表

    public Button editPersonBtn;//编辑人员按钮
    public HistoryPersonUIItem PersonItemPrefab;//播放历史轨迹界面的人员列表
    public VerticalLayoutGroup personsGrid;
    public List<Color> colors = new List<Color>() { Color.red, Color.green, Color.blue, Color.yellow };
    public int limitPersonNum = 4;//限制显示历史轨迹人员数量
    public List<Personnel> currentPersonnels;//当前显示历史轨迹的人员信息
    public List<HistoryPersonUIItem> items;

    /// <summary>
    /// 添加按钮触发事件
    /// </summary>
    public void EditPersonBtn_OnClick()
    {
        if (currentPersonnels == null)
        {
            currentPersonnels = new List<Personnel>();
        }
        List<Personnel> currentPersonnelsT = new List<Personnel>(currentPersonnels);
        HistoryPersonsSearchUI.Instance.Show(currentPersonnelsT);
        if (isPlay)
        {
            //如果在播放就让它暂停
            ExecuteEvents.Execute<IPointerClickHandler>(playBtn.gameObject, new PointerEventData(EventSystem.current), ExecuteEvents.pointerClickHandler);
        }
    }

    public void ShowPersons(List<Personnel> personnelsT)
    {
        //如果在播放就让它终止
        ExecuteEvents.Execute<IPointerClickHandler>(MultHistoryPlayUI.Instance.StopBtn.gameObject, new PointerEventData(EventSystem.current), ExecuteEvents.pointerClickHandler);
        CreatePersons(personnelsT);
        items = new List<HistoryPersonUIItem>(personsGrid.GetComponentsInChildren<HistoryPersonUIItem>());
    }

    public void CreatePersons(List<Personnel> personnelsT)
    {
        ClearPersons();
        currentPersonnels = personnelsT;
        for (int i = 0; i < personnelsT.Count; i++)
        {
            int j = i / colors.Count;
            CreatePersonItem(personnelsT[i], colors[i]);
        }
    }

    /// <summary>
    /// 创建人员列表
    /// </summary>
    public HistoryPersonUIItem CreatePersonItem(Personnel personnelT, Color colorT)
    {
        HistoryPersonUIItem item = Instantiate(PersonItemPrefab);
        item.Init(personnelT, colorT);
        item.transform.SetParent(personsGrid.transform);
        item.transform.localPosition = Vector3.zero;
        item.transform.localScale = Vector3.one;
        item.gameObject.SetActive(true);
        return item;
    }

    /// <summary>
    /// 清除显示历史轨迹的人员
    /// </summary>
    public void ClearPersons()
    {
        currentPersonnels.Clear();
        int n = personsGrid.transform.childCount;
        for (int i = n - 1; i >= 0; i--)
        {
            DestroyImmediate(personsGrid.transform.GetChild(i).gameObject);
        }

    }

    /// <summary>
    /// 移除显示轨迹中的某个人员
    /// </summary>
    /// <param name="personnelT"></param>
    public void RemovePerson(Personnel personnelT)
    {
        currentPersonnels.Remove(personnelT);
        //List<HistoryPersonUIItem> pList = new List<HistoryPersonUIItem>(personsGrid.GetComponentsInChildren<HistoryPersonUIItem>());
        //HistoryPersonUIItem item = pList.Find((i) => i.personnel == personnelT);
        HistoryPersonUIItem item = items.Find((i) => i.personnel == personnelT);        
        DestroyImmediate(item.gameObject);
        items.Remove(item);
    }

    /// <summary>
    /// 设置人员历史所在区域
    /// </summary>
    public void SetItemArea(Personnel p ,string areaStr)
    {
        HistoryPersonUIItem item = items.Find((i) => i.personnel == p);
        if (item != null)
        {
            item.RefleshTxtPlace(areaStr);
        }
    }

    #endregion

    /// <summary>
    /// 设置isStop的值
    /// </summary>
    public void SetIsStop(bool isBool)
    {
        isStop = isBool;
        processSlider.interactable = !isBool;
    }
}
