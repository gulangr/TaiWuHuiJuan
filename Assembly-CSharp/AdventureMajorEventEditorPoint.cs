using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using AdventureEditor.Beta;
using FrameWork.UISystem.UIElements;
using GameData.Adventure;
using GameData.Adventure.Editor;
using TMPro;
using UnityEngine;

// Token: 0x0200018A RID: 394
public class AdventureMajorEventEditorPoint : MonoBehaviour
{
	// Token: 0x17000274 RID: 628
	// (get) Token: 0x0600162A RID: 5674 RVA: 0x000891E6 File Offset: 0x000873E6
	public AdventureMajorEventNodeSnapshot Data
	{
		get
		{
			return this.parent.Snapshot.Nodes[this.Index];
		}
	}

	// Token: 0x0600162B RID: 5675 RVA: 0x00089203 File Offset: 0x00087403
	public void Init(int index)
	{
		this.Index = index;
		base.gameObject.SetActive(true);
		this.editPanelBtn.isOn = false;
	}

	// Token: 0x0600162C RID: 5676 RVA: 0x00089228 File Offset: 0x00087428
	public void SetImg(AdventureMajorEventNodeSnapshot node)
	{
		string sprite = AdventureMajorEventTool.GetAdventureMajorEventNodeIcon(node.Type, node.Style, 0);
		this.icon.SetSprite(sprite, false, null);
		this.icon.SetNativeSize();
		this.decorate.SetSprite(AdventureMajorEventTool.GetNodeDecorateName(node.Type), false, null);
		this.decorate.SetNativeSize();
	}

	// Token: 0x0600162D RID: 5677 RVA: 0x0008928C File Offset: 0x0008748C
	public void SetPointInfo(AdventureMajorEventNodeSnapshot node, StringBuilder sb)
	{
		this.SetImg(node);
		base.GetComponent<RectTransform>().localPosition = new Vector3(node.X, node.Y, 0f);
		bool flag = node.Type == EAdventureMajorEventNodeType.Check;
		if (flag)
		{
			this.nodeName.SetText(node.Name.ToString() + "\n" + node.Type.ToString(), true);
		}
		else
		{
			this.nodeName.SetText(node.Name, true);
		}
		sb.Clear();
		sb.AppendJoin<string>(", ", from x in node.NextNodes
		select this.parent.Snapshot.Nodes[x].Key);
		string nextNodes = sb.ToString();
		sb.Clear();
		sb.AppendFormat("Key:{0}\n  NextNodeKey:{1}", node.Key, nextNodes);
		this.keyInfo.SetText(sb);
		this.dragNode.OnDragEnd = new Action(this.parent.PointOnDragEnd);
	}

	// Token: 0x0600162E RID: 5678 RVA: 0x0008939C File Offset: 0x0008759C
	public void DeleteNode(int index)
	{
		this.parent.AddNextNodePoint = null;
		Func<int, bool> <>9__0;
		Func<int, int> <>9__1;
		foreach (AdventureMajorEventNodeSnapshot node in this.parent.Snapshot.Nodes)
		{
			IEnumerable<int> nextNodes = node.NextNodes;
			Func<int, bool> predicate;
			if ((predicate = <>9__0) == null)
			{
				predicate = (<>9__0 = ((int n) => n != index));
			}
			IEnumerable<int> source = nextNodes.Where(predicate);
			Func<int, int> selector;
			if ((selector = <>9__1) == null)
			{
				selector = (<>9__1 = ((int n) => (n > index) ? (n - 1) : n));
			}
			int[] nodes = source.Select(selector).ToArray<int>();
			node.NextNodes.Clear();
			node.NextNodes.AddRange(nodes);
		}
		this.parent.Snapshot.Nodes.RemoveAt(index);
		for (int nodeIndex = 0; nodeIndex < this.parent.Snapshot.Nodes.Count; nodeIndex++)
		{
			this.parent.PointDict[nodeIndex].Index = nodeIndex;
		}
		this.parent.RefreshShow();
	}

	// Token: 0x0600162F RID: 5679 RVA: 0x000894F0 File Offset: 0x000876F0
	public void DeleteNode()
	{
		this.DeleteNode(this.Index);
	}

	// Token: 0x17000275 RID: 629
	// (get) Token: 0x06001630 RID: 5680 RVA: 0x000894FF File Offset: 0x000876FF
	// (set) Token: 0x06001631 RID: 5681 RVA: 0x0008950C File Offset: 0x0008770C
	public bool Editing
	{
		get
		{
			return this.editPanelBtn.isOn;
		}
		set
		{
			this.editPanelBtn.isOn = value;
		}
	}

	// Token: 0x06001632 RID: 5682 RVA: 0x0008951B File Offset: 0x0008771B
	public void BeginAddSubNodes()
	{
		this.parent.BeginAddSubNodes(this);
	}

	// Token: 0x06001633 RID: 5683 RVA: 0x0008952C File Offset: 0x0008772C
	public void RecordAddNextNode()
	{
		AdventureMajorEventEditorPoint point = this.parent.AddNextNodePoint;
		bool flag = point == null || point.Data.NextNodes.Contains(this.Index);
		if (!flag)
		{
			point.AddNextNode(this.Index);
			this.parent.RefreshShow();
		}
	}

	// Token: 0x06001634 RID: 5684 RVA: 0x00089588 File Offset: 0x00087788
	internal void AddNextNode(string nodeKey)
	{
		AdventureMajorEventEditorPoint link = this.parent.GetEditorPointByKey(nodeKey);
		bool flag = link == null;
		if (flag)
		{
			Debug.LogWarning(string.Concat(new string[]
			{
				"Error: input key [",
				nodeKey,
				"] does not exist, skipping Add ",
				nodeKey,
				" as a NextNode of ",
				this.Data.Key
			}));
		}
		else
		{
			this.Data.NextNodes.Add(link.Index);
			this.parent.Snapshot.Nodes[link.Index].Type = this.parent.GetNodeTypeByParent(this.Data, this.parent.Snapshot.Nodes[link.Index]);
		}
	}

	// Token: 0x06001635 RID: 5685 RVA: 0x00089654 File Offset: 0x00087854
	internal void AddNextNode(int nodeIndex)
	{
		this.Data.NextNodes.Add(nodeIndex);
		this.parent.Snapshot.Nodes[nodeIndex].Type = this.parent.GetNodeTypeByParent(this.Data, this.parent.Snapshot.Nodes[nodeIndex]);
	}

	// Token: 0x0400121F RID: 4639
	public int Index;

	// Token: 0x04001220 RID: 4640
	[SerializeField]
	private UI_AdventureMajorEventEditor parent;

	// Token: 0x04001221 RID: 4641
	[SerializeField]
	private GameObject editPanel;

	// Token: 0x04001222 RID: 4642
	[SerializeField]
	private CToggle editPanelBtn;

	// Token: 0x04001223 RID: 4643
	[SerializeField]
	private AdventureEditorDragNode dragNode;

	// Token: 0x04001224 RID: 4644
	[SerializeField]
	private CImage icon;

	// Token: 0x04001225 RID: 4645
	[SerializeField]
	private CImage decorate;

	// Token: 0x04001226 RID: 4646
	[SerializeField]
	private TMP_Text keyInfo;

	// Token: 0x04001227 RID: 4647
	[SerializeField]
	private TMP_Text nodeName;

	// Token: 0x04001228 RID: 4648
	[SerializeField]
	public CRawImage eventImg;
}
