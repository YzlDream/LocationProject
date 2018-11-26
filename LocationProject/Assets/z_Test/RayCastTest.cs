using Assets.z_Test.BackUpDevInfo;
using Base.Common;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RayCastTest : MonoBehaviour {

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
        //RayTest();
        loadXML();
    }
    private void loadXML()
    {
        if (Input.GetMouseButtonDown(0))
        {
            string filePath = Application.dataPath + "/DevInfoBackup.xml";
            DevInfoBackupList devInfo = SerializeHelper.LoadFromFile<DevInfoBackupList>(filePath);
            Debug.Log(devInfo.DevList.Count);
        }           
    }
    private void RayTest()
    {
        if(Input.GetMouseButtonDown(0))
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hitInfo;
            if(Physics.Raycast(ray,out hitInfo))
            {
                Debug.Log(hitInfo.transform.name);
            }
        }
    }
}
