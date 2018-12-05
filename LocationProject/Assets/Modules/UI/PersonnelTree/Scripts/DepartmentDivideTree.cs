using Location.WCFServiceReferences.LocationServices;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UIWidgets;
using UnityEngine;
using System;
using UnityEngine.UI;

public class DepartmentDivideTree : MonoBehaviour
{

    public Text DepartmentText;
    public string RootName;
    public ChangeTreeView Tree;
    ObservableList<TreeNode<TreeViewItem>> nodes;
    public AreaDivideTree areaDivideTree;
    /// <summary>
    /// 部门划分
    /// </summary>
    public GameObject DepartmentWindow;
    /// <summary>
    /// 对应区域图片列表
    /// </summary>
    public List<Sprite> Icons;
    /// <summary>
    /// 人员信息列表
    /// </summary>
    public List<Personnel> personnels;
    /// <summary>
    /// 部门人员树数据
    /// </summary>
    Department topoRoot;

    //public Toggle DepartmentDivideToggle;

    void Start()
    {
        if (personnels == null)
        {
            personnels = new List<Personnel>();
        }
        //DepartmentDivideToggle.onValueChanged.AddListener(ShowDepartmentWindow);
    }
    public void ShowDepartmentDivideTree()
    {
        Loom.StartSingleThread(() =>
        {
            RefleshData();
            Loom.DispatchToMainThread(() =>
            {
                StructureTree(topoRoot);
                Tree.Start();
                Tree.Nodes = nodes;
                SetListeners();
                InvokeRepeating("RefleshDataThread", 1, 1);
            });
        });

        //GetTopoTree();
        //Tree.Start();
        //Tree.Nodes = nodes;
        //SetListeners();
    }
    /// <summary>
    /// 获取区域数据
    /// </summary>
    public void GetTopoTree()
    {
        //topoRoot = CommunicationObject.Instance.GetDepTree();
        //StructureTree(topoRoot);
        //if (personnels == null)
        //{
        //    personnels = new List<Personnel>();
        //}
        //personnels = CommunicationObject.GetPersonnels(topoRoot);

        //Loom.StartSingleThread(() =>
        //{
        RefleshData();
        //    Loom.DispatchToMainThread(() =>
        //    {
        //        StructureTree(topoRoot);
        //    });
        //});

        //InvokeRepeating("RefleshDataThread", 1, 1);
    }

    /// <summary>
    /// 刷新树数据
    /// </summary>
    private void RefleshData()
    {
        topoRoot = CommunicationObject.Instance.GetDepTree();
        //StructureTree(topoRoot);
        if (personnels == null)
        {
            personnels = new List<Personnel>();
        }
        personnels = CommunicationObject.GetPersonnels(topoRoot);
        //return topoRoot;
    }

    /// <summary>
    /// 刷新树数据
    /// </summary>
    private void RefleshDataThread()
    {
        Loom.StartSingleThread(() =>
        {
            topoRoot = CommunicationObject.Instance.GetDepTree();
            //StructureTree(topoRoot);
            if (personnels == null)
            {
                personnels = new List<Personnel>();
            }
            personnels = CommunicationObject.GetPersonnels(topoRoot);
            //return topoRoot;
            Loom.DispatchToMainThread(() =>
            {
                //Debug.Log("RefleshDataThread:personnels");
            });
        });

    }

    /// <summary>
    /// 添加子节点
    /// </summary>
    /// <param name="topoNode"></param>
    /// <param name="treeNode"></param>
    public void AddNodes(Department topoNode, TreeNode<TreeViewItem> treeNode)
    {
        treeNode.Nodes = new ObservableList<TreeNode<TreeViewItem>>();
        if (topoNode.Children != null)
        {
            foreach (Department child in topoNode.Children)
            {
                var node = CreateTopoNode(child);
                treeNode.Nodes.Add(node);
                AddNodes(child, node);
            }
        }
        if (topoNode.LeafNodes != null)//添加子节点的子节点
            foreach (Personnel child in topoNode.LeafNodes)
            {
                var node = CreatePersonnalNode(child);
                treeNode.Nodes.Add(node);
            }
    }
    private TreeNode<TreeViewItem> CreatePersonnalNode(Personnel personnal)
    {
        Sprite icon = Icons[0];//设备图标 todo:以后可以判断是机柜还是设备，机柜则换上机柜图标
        var item = new TreeViewItem(personnal.Name, icon);
        item.Tag = personnal.Id;
        var node = new TreeNode<TreeViewItem>(item);
        return node;
    }
    /// <summary>
    /// 创建节点
    /// </summary>
    /// <param name="topoNode"></param>
    /// <returns></returns>
    private TreeNode<TreeViewItem> CreateTopoNode(Department topoNode)
    {
        string title = topoNode.Name;
        if (topoNode.LeafNodes != null)
        {
            title = string.Format("{0} ({1})", title, topoNode.LeafNodes.Length);
        }

        var item = new TreeViewItem(title);
        item.Tag = topoNode;
        var node = new TreeNode<TreeViewItem>(item);
        return node;
    }

    /// <summary>
    /// 去掉节点
    /// </summary>
    /// <param name="node"></param>
    public void NodeDeselected(TreeNode<TreeViewItem> node)
    {
        Debug.Log(node.Item.Name + " deselected");
        //LocationManager.Instance.RecoverBeforeFocusAlign();
    }
    /// <summary>
    /// 选中节点
    /// </summary>
    /// <param name="node"></param>
    public void NodeSelected(TreeNode<TreeViewItem> node)
    {

        Debug.Log(node.Item.Name + " selected");
        if (node.Item == null || node.Item.Tag == null)
        {
            //LocationManager.Instance.RecoverBeforeFocusAlign();
            return;
        }
        if (node.Item.Tag is Department)
        {
            LocationManager.Instance.RecoverBeforeFocusAlign();
            return;
        }
        if (!(node.Item.Tag is int) && !(node.Item.Tag is string))
        {
            //LocationManager.Instance.RecoverBeforeFocusAlign();
            return;
        }
        //Personnel tagT = node.Item.Tag as Personnel;
        int tagT = (int)node.Item.Tag;
        if (tagT != null)
        {
            ParkInformationManage.Instance.ShowParkInfoUI(false);
            //LocationManager.Instance.FocusPerson(tagT.Id);
            LocationManager.Instance.FocusPersonAndShowInfo(tagT);
        }
    }
    /// <summary>
    /// 展示树信息
    /// </summary>
    /// <param name="root"></param>
    public void StructureTree(Department root)
    {
        if (root == null)
        {
            Log.Error("StructureTree root == null");
            return;
        }
        nodes = new ObservableList<TreeNode<TreeViewItem>>();
        if (string.IsNullOrEmpty(RootName))
        {
            //不显示根节点，显示根节点下的第一级节点
            ShowFirstLayerNodes(root);
        }
        else
        {
            Department rootNode = root.Children.ToList().Find(i => i.Name == RootName);
            if (rootNode != null)
            {
                ShowFirstLayerNodes(rootNode);//显示某一个一级节点下的内容
            }
            else
            {
                ShowFirstLayerNodes(root);
            }

        }
    }
    /// <summary>
    /// 展示第一层的节点
    /// </summary>
    /// <param name="root"></param>
    private void ShowFirstLayerNodes(Department root)
    {
        foreach (Department child in root.Children)
        {
            var rootNode = CreateTopoNode(child);
            nodes.Add(rootNode);
            AddNodes(child, rootNode);
            //rootNode.IsExpanded = true;
        }
    }
    public void SetListeners()
    {
        Tree.NodeSelected.AddListener(NodeSelected);
        Tree.NodeDeselected.AddListener(NodeDeselected);
    }
    /// <summary>
    /// 关闭部门划分
    /// </summary>
    public void CloseDepartmentWindow()
    {
        NoSelectedTextChange();
        DepartmentWindow.SetActive(false);


    }
    /// <summary>
    /// 打开设备拓朴树界面
    /// </summary>
    public void ShowDepartmentWindow(bool ison)
    {
        if (ison)
        {
            //areaDivideTree.ShowAreaDivideWindow(false);
            DepartmentWindow.SetActive(true);
            SelectedTextChange();
        }
        else
        {
            CloseDepartmentWindow();
        }
    }
    /// <summary>
    /// 选中后字体颜色改变
    /// </summary>
    public void SelectedTextChange()
    {
        DepartmentText.color = new Color(109 / 255f, 236 / 255f, 254 / 255f, 255 / 255f);
    }
    /// <summary>
    /// 没有选中时字体的颜色
    /// </summary>
    public void NoSelectedTextChange()
    {
        DepartmentText.color = new Color(109 / 255f, 236 / 255f, 254 / 255f, 100 / 255f);
    }

    ///// <summary>
    ///// 选中某个人员节点，根据Personnel
    ///// </summary>
    //public void SelectNodeByPersonnel(Personnel personnelT)
    //{
    //    //TreeNode<TreeViewItem> nodeT = nodes.Find((item) => item.Item.Tag == personnelT);
    //    TreeNode<TreeViewItem> nodeT = FindNodeByPersonnel(personnelT, new List<TreeNode<TreeViewItem>>(nodes));
    //    if (nodeT != null)
    //    {
    //        //Image image= nodeT.Item.
    //        Tree.SelectedNodes = new List<TreeNode<TreeViewItem>>() { nodeT };
    //        Tree.Refresh();
    //    }
    //    else
    //    {
    //        Debug.LogError("异常：该人员在拓扑树中找不到！");
    //    }
    //}

    //public TreeNode<TreeViewItem> FindNodeByPersonnel(Personnel personnelT, List<TreeNode<TreeViewItem>> nodesT)
    //{
    //    TreeNode<TreeViewItem> node = null;
    //    if (nodesT != null)
    //    {
    //        foreach (TreeNode<TreeViewItem> nodeT in nodesT)
    //        {
    //            if (nodeT == null) continue;
    //            if (nodeT.Item.Tag == personnelT)
    //            {
    //                node = nodeT;
    //                break;
    //            }
    //            else
    //            {
    //                if (nodeT.Nodes == null)
    //                {
    //                    //List<TreeNode<TreeViewItem>> nodesTT = new List<TreeNode<TreeViewItem>>();
    //                    //return FindNodeByPersonnel(personnelT, nodesTT);
    //                }
    //                else
    //                {
    //                    try
    //                    {
    //                        node = FindNodeByPersonnel(personnelT, new List<TreeNode<TreeViewItem>>(nodeT.Nodes));
    //                    }
    //                    catch
    //                    {
    //                        int I = 0;
    //                    }
    //                }
    //            }
    //        }
    //    }

    //    return node;

    //}
}
