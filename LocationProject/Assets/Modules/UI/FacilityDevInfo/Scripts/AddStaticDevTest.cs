using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Location.WCFServiceReferences.LocationServices;
using System;
using Assets.M_Plugins.Helpers.Utils;

public class AddStaticDevTest : MonoBehaviour {
	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}
    [ContextMenu("AddController")]
    public void AddScprits()
    {
        int childCount = transform.childCount;
        for(int i=0;i<childCount;i++)
        {
            GameObject obj = transform.GetChild(i).gameObject;
            BoxCollider col = obj.GetComponent<BoxCollider>();
            if(col==null)obj.AddCollider();
            obj.AddMissingComponent<FacilityDevController>();
        }
    }
    [ContextMenu("AddDevInfo")]
    public void SaveDevInfo()
    {
        int childCount = transform.childCount;
        for (int i = 0; i < childCount; i++)
        {
            GameObject obj = transform.GetChild(i).gameObject;
            StartCoroutine(SaveDevInfo(obj));
        }
    }
    IEnumerator SaveDevInfo(GameObject model)
    {
        CommunicationObject service = CommunicationObject.Instance;
        if (service)
        {
            DevInfo dev = GetDevInfo(model);
            service.AddDevInfo(ref dev);
            yield return null;
            Debug.LogError("Dev:" + dev.Id);
        }
    }
    private DevInfo GetDevInfo(GameObject model)
    {
        DevInfo dev = new DevInfo();       
        dev.DevID = Guid.NewGuid().ToString();
        dev.IP = "";
        dev.CreateTime = DateTime.Now;
        dev.ModifyTime = DateTime.Now;
        dev.Name = "设备";
        dev.ModelName = model.name;
        dev.Status = 0;
        dev.ParentId = GetPID(model);
        try
        {
            dev.TypeCode = int.Parse(TypeCodeHelper.StaticDevTypeCode);
        }
        catch (Exception e)
        {
            dev.TypeCode = 0;
        }
        dev.UserName = "admin";
        return dev;
    }
    private int? GetPID(GameObject model)
    {
        DepNode parentNode = model.GetComponentInParent<DepNode>();
        return parentNode.NodeID;
    }
}
