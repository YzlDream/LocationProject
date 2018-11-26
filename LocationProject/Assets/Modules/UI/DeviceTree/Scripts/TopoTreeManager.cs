using Location.WCFServiceReferences.LocationServices;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UIWidgets;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using MonitorRange;

public class TopoTreeManager : MonoBehaviour
{
    public static TopoTreeManager Instance;
    public string RootName;
    /// <summary>
    /// 滑动条
    /// </summary>
    public ScrollRect scrollRect;
    public ChangeTreeView Tree;
    ObservableList<TreeNode<TreeViewItem>> nodes;
    public GameObject Window;
    /// <summary>
    /// 对应区域图片列表
    /// </summary>
    public List<Sprite> Icons;
    //TreeNode<TreeViewItem> rootNode;
    /// <summary>
    /// 收缩窗体动画
    /// </summary>
    private Tween ScaleWindowTween;
    /// <summary>
    /// 动画是否初始化
    /// </summary>
    private bool IsTweenInit;
    void Awake()
    {
        Instance = this;
    }
    void Start () {

        GetTopoTree();
        //Tree.Start();
        //Tree.Nodes = nodes;
        //SetListeners();
        //scrollRect.horizontal = true;

    }
    #region 窗体收缩动画部分
    /// <summary>
    /// 初始化动画
    /// </summary>
    private void InitTween()
    {
        IsTweenInit = true;
        RectTransform rect = transform.GetComponent<RectTransform>();
        Vector2 endValue = rect.sizeDelta - new Vector2(0,280);
        ScaleWindowTween = transform.GetComponent<RectTransform>().DOSizeDelta(endValue,0.3f);
        ScaleWindowTween.SetAutoKill(false);
        ScaleWindowTween.Pause();
    }
    /// <summary>
    /// 缩放窗体
    /// </summary>
    /// <param name="isExpand">是否扩大窗体</param>
    public void ScaleWindow(bool isExpand)
    {
        if(!IsTweenInit)
        {
            InitTween();
        }
        if(isExpand)
        {
            ScaleWindowTween.OnRewind(ResizeTree).PlayBackwards();
        }
        else
        {
            ScaleWindowTween.OnComplete(ResizeTree).PlayForward();
        }
    }
    /// <summary>
    /// 刷新树控件
    /// </summary>
    private void ResizeTree()
    {
        if (Tree != null)
        {
            Tree.ResizeContent();
        }
        else
        {
            Debug.LogError("TopoTree is null...");   
        }          
    }
    #endregion
    /// <summary>
    /// 获取区域数据
    /// </summary>
    public void GetTopoTree()
    {
        Loom.StartSingleThread(() =>
        {
            PhysicalTopology topoRoot = CommunicationObject.Instance.GetTopoTree();
            Loom.DispatchToMainThread(() =>
            {
                StructureTree(topoRoot);
                Tree.Start();
                Tree.Nodes = nodes;
                SetListeners();
                scrollRect.horizontal = true;

            });
        });
    }

    public void SetListeners()
    {
        Tree.NodeSelected.AddListener(NodeSelected);
        Tree.NodeDeselected.AddListener(NodeDeselected);
    }

    // called when node selected
    public void NodeSelected(TreeNode<TreeViewItem> node)
    {
        Debug.Log(node.Item.Name + " selected");
        PhysicalTopology topoNode = node.Item.Tag as PhysicalTopology;
        if (topoNode != null)
        {
            //if(RoomFactory.Instance)
            //{
            //    RoomFactory.Instance.FocusNode(topoNode);
            //}

            SceneEvents.OnTopoNodeChanged(topoNode);
        }
    }

    // called when node deselected
    public void NodeDeselected(TreeNode<TreeViewItem> node)
    {
        Debug.Log(node.Item.Name + " deselected");
    }



    /// <summary>
    /// 展示区域树
    /// </summary>
    /// <param name="root"></param>
    public void StructureTree(PhysicalTopology root)
    {
        if (root == null)
        {
            Log.Error("StructureTree root == null");
            return;
        }
        nodes = new ObservableList<TreeNode<TreeViewItem>>();

        //TreeNode<TreeViewItem> rootNode = AddRootNode(root.Name, root);
        //AddNodes(root,rootNode);
        //rootNode.IsExpanded = true;

        if (string.IsNullOrEmpty(RootName))
        {
            //不显示根几点，显示根节点下的第一级节点
            ShowFirstLayerNodes(root);
        }
        else
        {
            PhysicalTopology rootNode = root.Children.ToList().Find(i => i.Name == RootName);
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

    private void ShowFirstLayerNodes(PhysicalTopology root)
    {
        SceneEvents.TopoRootNode = root;
        foreach (PhysicalTopology child in root.Children)
        {
            var rootNode = CreateTopoNode(child);
            nodes.Add(rootNode);
            AddNodes(child, rootNode);
            //rootNode.IsExpanded = true;
        }
    }

    /// <summary>
    /// 添加根节点
    /// </summary>
    /// <param name="nodeName"></param>
    /// <param name="root"></param>
    /// <returns></returns>
    public TreeNode <TreeViewItem> AddRootNode(PhysicalTopology root)
    {
        if (root ==null)
        {
            return null;
        }
        var treeNode = CreateTopoNode(root); 
        nodes.Add(treeNode);
        var roomNodeFirst = nodes[0];
        return roomNodeFirst;
    }

    private TreeNode<TreeViewItem> CreateTopoNode(PhysicalTopology topoNode)
    {
        Sprite icon = GetTopoIcon(topoNode);
        var item = new TreeViewItem(topoNode.Name, icon);
        //var item = new TreeViewItem(string.Format("{0}[{1}]",topoNode.Name,topoNode.Transfrom!=null), icon);
        item.Tag = topoNode;
        var node = new TreeNode<TreeViewItem>(item);
        return node;
    }

    private TreeNode<TreeViewItem> CreateDevNode(DevInfo dev)
    {
        Sprite icon = Icons[5];//设备图标 todo:以后可以判断是机柜还是设备，机柜则换上机柜图标
        var item = new TreeViewItem(dev.Name, icon);
        item.Tag = dev;
        var node = new TreeNode<TreeViewItem>(item);
        return node;
    }

    /// <summary>
    /// 添加子节点
    /// </summary>
    /// <param name="topoNode"></param>
    /// <param name="treeNode"></param>
    public void AddNodes(PhysicalTopology topoNode,TreeNode<TreeViewItem> treeNode)
    {
        treeNode.Nodes = new ObservableList<TreeNode<TreeViewItem>>();   
        if (topoNode.Children != null)
        {
            foreach (PhysicalTopology child in topoNode.Children)
            {
                var node = CreateTopoNode(child);
                treeNode.Nodes.Add(node);
                AddNodes(child, node);
            }
        }

        if (topoNode.LeafNodes != null)
            foreach (DevInfo child in topoNode.LeafNodes)
            {
                var node = CreateDevNode(child);
                treeNode.Nodes.Add(node);
            }
    }
    /// <summary>
    /// 设置各物体的图标
    /// </summary>
    /// <param name="tpNode"></param>
    /// <returns></returns>
    public Sprite GetTopoIcon(PhysicalTopology tpNode)
    {
        Sprite icon = null;
        int typeNumber = (int)tpNode.Type - 1;
        if (typeNumber == -1)
        {
            icon = Icons[3];
        }
        else if (Icons.Count > typeNumber)
            icon = Icons[typeNumber];
        return icon;
    }
    /// <summary>
    /// 关闭设备拓朴树界面
    /// </summary>
    public void  CloseWindow()
    {
        if(Window.activeInHierarchy)
            Window.SetActive(false);
    }
    /// <summary>
    /// 打开设备拓朴树界面
    /// </summary>
    public void ShowWindow()
    {
        if (!Window.activeInHierarchy)
            Window.SetActive(true);
    }
    // Update is called once per frame
    void Update () {
		
	}
}
