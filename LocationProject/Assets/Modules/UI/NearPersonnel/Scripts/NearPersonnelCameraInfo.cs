using Location.WCFServiceReferences.LocationServices;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class NearPersonnelCameraInfo : MonoBehaviour {

    public Text cameraName;
    public Text cameraId;
    public Text Distance;
    public Toggle camTog;
    public NearPerCameraRotation nearPerCameraRotation;
    void Start () {
        camTog.onValueChanged.AddListener(Click_Toggle);

    }
    public void showNearPersonnelCamInfo(NearbyDev devList)
    {
       if (devList.TypeName==null)
        {
            cameraName.text = "";
        }
       else
        {
            cameraName.text = devList.TypeName.ToString();
        }
           
            cameraId.text  = devList.id.ToString();

        Distance.text = "距离人员" + string.Format("{0:F}", devList.Distance);
        
    }

    public void Click_Toggle(bool ison)
    {
        if (ison)
        {
            nearPerCameraRotation.CameraPiontClick();
            nearPerCameraRotation.CaneraPoint.isOn = true;
        }
        else
        {
            nearPerCameraRotation.CameraPointExit();
            nearPerCameraRotation.CaneraPoint.isOn = false;
        }
    }
    void Update () {
		
	}
}
