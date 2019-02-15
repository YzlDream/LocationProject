using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System;
using Location.WCFServiceReferences.LocationServices;

public class FileDownLoad : MonoBehaviour {

    //  public AudioSource audioSource;
    string urlPath;//资源网络路径
    string file_SaveUrl;//资源保存路径

    public static FileDownLoad Instance;

    private WWW downloadOperation;
    private bool isDownLoadStart;


    private void Awake()
    {
        Instance = this;
    }
    private void Start()
    {

    }

    void Update()
    {
        if(isDownLoadStart)
        {
            //判断异步对象并且异步对象没有加载完毕，显示进度    
            if (downloadOperation != null && !downloadOperation.isDone)
            {
                //Debug.Log(string.Format("下载进度:{0:F}%", downloadOperation.progress * 100.0));
                ShowDownloadProgress(true,downloadOperation.progress);
            }
        } 
    }

    //private string fileName= "file:///"+ @"C:\Users\Administrator\Desktop\LocationSystem.exe";
    private string fileName = "http://127.0.0.1:8013/电厂/LocationSystem.exe";


    public void Download(string serverURL)
    {
        string path = serverURL;
        if (string.IsNullOrEmpty(path))
        {
            Debug.Log("路径输入错误");
            UGUIMessageBox.Show("Version path is empty,please check!");         
            return;
        }
        urlPath = path;
        //file_SaveUrl = @"C:\Users\Administrator\Desktop\LocationSystem.exe";
        file_SaveUrl = Application.dataPath + @"\LocationSystem.exe";
        Debug.Log("urlPath : " + urlPath);
        FileInfo file = new FileInfo(file_SaveUrl);    //每次都重新计算
        byte[] bytes = new byte[1024];                  //

        if (File.Exists(file_SaveUrl))//本地存在，删除重新下载
        {
            try
            {
                File.Delete(file_SaveUrl);
            }
            catch (Exception e)
            {
                Debug.Log(e.ToString());
            }
        }
        StartCoroutine(DownFile(urlPath, file_SaveUrl, file, bytes));
    }
    IEnumerator DownFile(string url,string file_SaveUrl, FileInfo file, byte[] bytes)
    {
        downloadOperation = new WWW(url);
        isDownLoadStart = true;
        yield return downloadOperation;
        if (downloadOperation.error != null)
        {
            Debug.Log(downloadOperation.error);
            isDownLoadStart = false;
            UGUIMessageBox.Show(downloadOperation.error);
            yield break;
        }        
        if (downloadOperation.isDone)
        {
            isDownLoadStart = false;
            ShowDownloadProgress(true,1);
            Debug.Log("下载完成，文件大小 : " + downloadOperation.bytes.Length);
            bytes = downloadOperation.bytes;
            CreatFile(bytes, file);
            Debug.Log("文件创建完成...");
            Application.OpenURL(file_SaveUrl);
            Invoke("Quit",3f);
        }
    }
    private void ShowDownloadProgress(bool isActive,float value)
    {
        DownloadProgressBar progress = DownloadProgressBar.Instance;
        if (progress)
        {
            if (isActive) progress.Show(value);
            else progress.Hide();
        }
    }
    /// <summary>
    /// 退出程序
    /// </summary>
    private void Quit()
    {
        Application.Quit();
    }

    private void OnDisable()
    {
        isDownLoadStart = false;
    }
    /// <summary>
    /// 文件流创建文件
    /// </summary>
    /// <param name="bytes"></param>
    void CreatFile(byte[] bytes, FileInfo file)
    {
        Stream stream;
        stream = file.Create();
        stream.Write(bytes, 0, bytes.Length);
        stream.Close();
        stream.Dispose();
    }
}
