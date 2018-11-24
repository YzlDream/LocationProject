using UnityEngine;
using System.Collections;
using UnityEditor;
using UnityEngine.UI;
public class MyTools  {

    /// <summary>
    /// 自动计算所有子对象包围盒
    /// </summary>
    [MenuItem("Tools/AddBoxCollider")]
    public static void AddBoxCollider()
    {
        Transform parent = Selection.activeGameObject.transform;
        ColliderHelper.CreateBoxCollider(parent);
    }

    //[MenuItem("Tools/AddBoxColliderOld")]
    //public static void AddBoxColliderOld()
    //{
    //    Transform parent = Selection.activeGameObject.transform;
    //    ColliderHelper.CreateBoxColliderOld(parent);
    //}

 //   [MenuItem("Tools/Sockpol")]
	//public static void StartSockpol()
	//{
	//	//System.Diagnostics.Process.Start ("Tools\\SocketPolicyServer\\start.bat");
	//	System.IO.FileInfo file = new System.IO.FileInfo (Application.dataPath+@"\..\Tools\SocketPolicyServer\sockpol.exe");
	//	Debug.Log (file.FullName);
	//	System.Diagnostics.Process.Start(file.FullName, "--all");
	//}
    ///// <summary>
    ///// 添加输入框的聚焦检测
    ///// </summary>
    //[MenuItem("Tools/AddInputFocus")]
    //public static void AddInputFocus()
    //{
    //    Transform parent = Selection.activeGameObject.transform;
    //    InputField[] Inputs = parent.GetComponentsInChildren<InputField>(true);
    //    Debug.LogError("InputFiled Count:"+Inputs.Length);
    //    foreach(var item in Inputs)
    //    {
    //        item.gameObject.AddMissingComponent<InputFocus>();
    //        //Debug.LogError(item.transform.parent.name);
    //    }
    //}
}
