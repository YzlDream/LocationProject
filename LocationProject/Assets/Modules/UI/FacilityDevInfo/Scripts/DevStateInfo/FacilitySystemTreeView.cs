using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UIWidgets;
using UnityEngine.UI;

public class FacilitySystemTreeView : TreeView
{

    public Sprite HighlightImage;
    public Sprite SelectColoringImage;
    public Sprite DefaultImage;
    /// <summary>
    /// 文本框宽度
    /// </summary>
    private float NormalTextWidth = 410;
    /// <summary>
    /// 更新树窗体
    /// </summary>
    public void ResizeContent()
    {
        base.Resize();
    }
    protected override void SetData(TreeViewComponent component, ListNode<TreeViewItem> item)
    {
        base.SetData(component, item);
        ChangeItemValue(component,item);
    }
    private void ChangeItemValue(TreeViewComponent component, ListNode<TreeViewItem> item)
    {
        FacilityDevTreeItem DevItem = component.GetComponent<FacilityDevTreeItem>();
        if (DevItem) DevItem.SetValue(item.Node.Item.Tag as FacilitySystem);
        float offset = item.Depth * component.PaddingPerLevel;
        //if (offset == 0) return;
        LayoutElement element = component.Text.GetComponent<LayoutElement>();
        //Debug.Log(item.Node.Item.Name+" Nodes: "+item.Node.Nodes.Count);
        if(item.Node.Nodes!=null&&item.Node.Nodes.Count!=0)
        {
            element.preferredWidth = NormalTextWidth - offset;
        }
        else
        {
            float toggleSize = component.Toggle.GetComponent<LayoutElement>().preferredWidth;
            element.preferredWidth = NormalTextWidth - offset+toggleSize;
        }        
    }

    protected override void HighlightColoring(TreeViewComponent component)
    {
        base.HighlightColoring(component);

        component.Background.sprite = HighlightImage;
    }
    protected override void SelectColoring(TreeViewComponent component)
    {
        base.SelectColoring(component);
        component.Background.sprite = SelectColoringImage;
    }


    protected override void DefaultColoring(TreeViewComponent component)
    {
        if (component == null)
        {
            return;
        }
        base.DefaultColoring(component);
        if (component.Text != null)
        {

            component.Background.sprite = DefaultImage;
        }
    }
}
