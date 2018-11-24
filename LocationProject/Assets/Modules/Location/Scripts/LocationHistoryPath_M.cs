using Location.WCFServiceReferences.LocationServices;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Vectrosity;

/// <summary>
/// 历史轨迹
/// </summary>
public class LocationHistoryPath_M : LocationHistoryPathBase
{
    protected override void Start()    {
        base.Start();
        LocationHistoryManager.Instance.AddHistoryPath_M(this as LocationHistoryPath_M);
        //timeStart = HistoryPlayUI.Instance.timeStart;
    }
    protected override void Update()    {
        base.Update();
        if (!isCreatePathComplete) return;
        //if (isSetHistoryPath) return;
        //if (line.drawEnd <= 0) return;
        if (MultHistoryPlayUI.Instance.isPlay == false) return;        if (currentPointIndex < 0) return;        if (MultHistoryPlayUI.Instance.processSlider.value < 1)
        {
            double timesum = MultHistoryPlayUI.Instance.timeSum;
            DateTime showPointTime = MultHistoryPlayUI.Instance.GetStartTime().AddSeconds(timesum);

            if (currentPointIndex < timelist.Count)
            {
                Debug.LogErrorFormat("timelist[currentPointIndex]:{0},showPointTime:{1}", timelist[currentPointIndex], showPointTime);
                if (timelist[currentPointIndex] < showPointTime)
                {
                    //double timesum2 = (timelist[currentPointIndex] - HistoryPlayUI.Instance.GetStartTime()).TotalSeconds;
                    //Debug.Log("timesum2:" + timesum2);
                    //progressTargetValue = (double)timesum2 / HistoryPlayUI.Instance.timeLength;
                    ExcuteHistoryPath(currentPointIndex);
                    Debug.LogError("currentPointIndex111:" + currentPointIndex);
                    currentPointIndex++;
                    Show();

                }
                else
                {
                    if (currentPointIndex - 1 >= 0)
                    {
                        ExcuteHistoryPath(currentPointIndex - 1);
                    }
                    Debug.LogError("currentPointIndex222:" + currentPointIndex);
                    double temp = (timelist[currentPointIndex] - showPointTime).TotalSeconds;
                    if (temp > 5f)//如果当前要执行历史点的值，超过播放时间值5秒，就认为这超过5秒时间里，没历史轨迹数据，则让人员消失
                    {
                        Hide();
                    }
                }
                //progressValue = Mathf.Lerp((float)progressValue, (float)progressTargetValue, 2 * Time.deltaTime);
                //transform.position = line.GetPoint3D01((float)progressValue);
                //ExcuteHistoryPath(currentPointIndex);
            }
            else
            {
                Hide();
            }
        }
        else
        {
            progressValue = 0;
            progressTargetValue = 0;
            currentPointIndex = 0;
        }

        ShowArea();
    }

    /// <summary>
    /// 获取离它最近的下一个播放点
    /// </summary>
    public override int GetNextPoint(float value)
    {
        double f = timeLength * value;
        DateTime startTimeT = MultHistoryPlayUI.Instance.GetStartTime();
        //相匹配的第一个元素,结果为-1表示没找到
        return timelist.FindIndex((item) =>
        {
            double timeT = (item - startTimeT).TotalSeconds;
            if (timeT > f)
            {
                return true;
            }
            else
            {
                return false;
            }
        });

    }


    /// <summary>
    /// 根据进度值，获取当前需要执行的点的索引
    /// </summary>
    /// <param name="f"></param>
    /// <param name="accuracy">精确度：时间相差accuracy秒</param>
    protected override int GetCompareTime(double f, float accuracy = 0.1f)
    {
        DateTime startTimeT = MultHistoryPlayUI.Instance.GetStartTime();
        //相匹配的第一个元素,结果为-1表示没找到
        return timelist.FindIndex((item) =>
        {
            double timeT = (item - startTimeT).TotalSeconds;
            if (Math.Abs(f - timeT) < accuracy)
            {
                return true;
            }
            else
            {
                return false;
            }
        });
    }

    /// <summary>
    /// 计算历史轨迹人员的所在区域
    /// </summary>
    public void ShowArea()
    {
        List<Position> ps = MultHistoryPlayUI.Instance.GetPositionsByPersonnel(personnel);
        if (ps != null)
        {
            if (currentPointIndex < ps.Count)
            {
                Position p = ps[currentPointIndex];
                DepNode depnode = RoomFactory.Instance.GetDepNodeById((int)p.TopoNodeId);
                MultHistoryPlayUI.Instance.SetItemArea(personnel, depnode.NodeName);
            }
        }
    }
}
