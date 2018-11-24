﻿using Location.WCFServiceReferences.LocationServices;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Vectrosity;

/// <summary>
/// 操作票人员历史
/// </summary>
public class OperationTicketHistoryPath : HistoryPath
{

    /// <summary>
    /// 人员信息
    /// </summary>
    protected Personnel personnel;

    // Use this for initialization
    protected override void Start()
    {
        StartInit();

        foreach (List<Vector3> splinePointsT in splinePointsList)
        {
            CreateHistoryPath(splinePointsT, splinePointsT.Count);
        }

        isCreatePathComplete = true;
    }

    protected override void Update()    {
        base.Update();
        //RefleshDrawLine();
    }

    protected override void LateUpdate()
    {
        base.LateUpdate();
    }

    /// <summary>
    /// 初始化
    /// </summary>
    public void Init(Personnel personnelT, Color colorT, List<Vector3> splinePointsT, List<DateTime> timelistT, int segmentsT, bool pathLoopT)
    {
        personnel = personnelT;
        segments = segmentsT;
        splinePoints = splinePointsT;
        timelist = timelistT;
        color = colorT;
        pathLoop = pathLoopT;
    }

    protected override void StartInit()
    {
        lines = new List<VectorLine>();
        dottedlines = new List<VectorLine>();
        splinePointsList = new List<List<Vector3>>();        timelistLsit = new List<List<DateTime>>();        CreatePathParent();
        //LocationHistoryManager.Instance.AddHistoryPath(this as LocationHistoryPath);
        //transform.SetParent(pathParent);
        if (splinePoints.Count <= 1) return;

        GroupingLine();
    }


    public void CreatePathParent()
    {
        GameObject historyPathParent = GameObject.Find("OperationTicketHistoryPathParent");
        if (historyPathParent == null)
        {
            historyPathParent = new GameObject("OperationTicketHistoryPathParent");

        }
        if (pathParent == null)
        {
            pathParent = new GameObject("pathParent" + personnel.Name).transform;
            pathParent.SetParent(historyPathParent.transform);
        }

        transform.SetParent(pathParent);
    }
}
