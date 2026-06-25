using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.UI;

// Token: 0x0200006D RID: 109
[RequireComponent(typeof(LoopScrollRect))]
public class GroupedInfinityScroll : MonoBehaviour
{
	// Token: 0x17000068 RID: 104
	// (get) Token: 0x060003C2 RID: 962 RVA: 0x00016F0C File Offset: 0x0001510C
	private string ItemPrefabKey
	{
		get
		{
			return "GroupedInfinityScroll_Item" + this.ContentItemTemplate.gameObject.GetInstanceID().ToString();
		}
	}

	// Token: 0x17000069 RID: 105
	// (get) Token: 0x060003C3 RID: 963 RVA: 0x00016F3B File Offset: 0x0001513B
	public bool IsVertical
	{
		get
		{
			return base.GetComponent<LoopScrollRect>().vertical;
		}
	}

	// Token: 0x1700006A RID: 106
	// (get) Token: 0x060003C4 RID: 964 RVA: 0x00016F48 File Offset: 0x00015148
	public IReadOnlyDictionary<GameObject, GroupedInfinityScroll.LineData> RenderingItems
	{
		get
		{
			return this._renderingItems;
		}
	}

	// Token: 0x1700006B RID: 107
	// (get) Token: 0x060003C5 RID: 965 RVA: 0x00016F50 File Offset: 0x00015150
	public LoopScrollRect LoopScroll
	{
		get
		{
			return this._loopScroll;
		}
	}

	// Token: 0x060003C6 RID: 966 RVA: 0x00016F58 File Offset: 0x00015158
	public void Init()
	{
		this._loopScroll = base.GetComponent<LoopScrollRect>();
		this.InitItemSize();
		this.LineTemplate.gameObject.SetActive(false);
		this.ContentItemTemplate.gameObject.SetActive(false);
	}

	// Token: 0x060003C7 RID: 967 RVA: 0x00016F94 File Offset: 0x00015194
	public RectTransform GetViewport()
	{
		return this._loopScroll.viewport;
	}

	// Token: 0x060003C8 RID: 968 RVA: 0x00016FB1 File Offset: 0x000151B1
	public void AddOnScrollEvent(Action action)
	{
		this._loopScroll.OnScrollEvent += action;
	}

	// Token: 0x060003C9 RID: 969 RVA: 0x00016FC1 File Offset: 0x000151C1
	public void RemoveOnScrollEvent(Action action)
	{
		this._loopScroll.OnScrollEvent -= action;
	}

	// Token: 0x060003CA RID: 970 RVA: 0x00016FD4 File Offset: 0x000151D4
	private void InitItemSize()
	{
		Transform content = this.LineTemplate.transform.Find("Content");
		Transform title = this.LineTemplate.transform.Find("Title");
		bool isVertical = this.IsVertical;
		if (isVertical)
		{
			this._contentSize = content.GetComponent<RectTransform>().rect.height;
			this._titleSize = title.GetComponent<RectTransform>().rect.height;
		}
		else
		{
			this._contentSize = content.GetComponent<RectTransform>().rect.width;
			this._titleSize = title.GetComponent<RectTransform>().rect.width;
		}
	}

	// Token: 0x060003CB RID: 971 RVA: 0x00017084 File Offset: 0x00015284
	public void UpdateData(List<GroupedInfinityScroll.GroupItem> groups, bool reset)
	{
		bool flag = !this._loopScroll;
		if (!flag)
		{
			this.GenerateLineDataList(groups);
			bool flag2 = reset || this._loopScroll.totalCount == 0;
			if (flag2)
			{
				this._loopScroll.InitLoop(this.LineTemplate.gameObject, this._lineDataList.Count, new Action<Transform, int>(this.OnRenderLineItem), null);
			}
			else
			{
				this._loopScroll.totalCount = this._lineDataList.Count;
				this._loopScroll.RefillCellsAtCurrentPosition();
			}
		}
	}

	// Token: 0x060003CC RID: 972 RVA: 0x0001711D File Offset: 0x0001531D
	public void ReRender()
	{
		this._isNeedReRender = true;
	}

	// Token: 0x060003CD RID: 973 RVA: 0x00017128 File Offset: 0x00015328
	private void LateUpdate()
	{
		bool isNeedReRender = this._isNeedReRender;
		if (isNeedReRender)
		{
			this._loopScroll.RefreshCells();
			this._isNeedReRender = false;
		}
	}

	// Token: 0x060003CE RID: 974 RVA: 0x00017158 File Offset: 0x00015358
	private void OnRenderLineItem(Transform item, int lineIndex)
	{
		bool flag = lineIndex > this._lineDataList.Count - 1;
		if (!flag)
		{
			GroupedInfinityScroll.LineData data = this._lineDataList[lineIndex];
			item.gameObject.name = lineIndex.ToString();
			this._renderingItems[item.gameObject] = data;
			Transform title = item.Find("Title");
			Transform content = item.Find("Content");
			bool isOneGroupOneLine = this.IsOneGroupOneLine;
			if (isOneGroupOneLine)
			{
				VerticalLayoutGroup verticalLayoutGroup = content.GetComponent<VerticalLayoutGroup>();
				bool flag2 = verticalLayoutGroup;
				if (flag2)
				{
					verticalLayoutGroup.spacing = this.InLineSpacing;
				}
				Action<int, Refers> onGroupTitleRender = this.OnGroupTitleRender;
				if (onGroupTitleRender != null)
				{
					onGroupTitleRender(data.GroupIndex, title.GetComponent<Refers>());
				}
				int offset = this.PrepareEnoughChildrenWithName(content, this.ContentItemTemplate.gameObject, data.ItemCount, this.ContentItemTemplate.gameObject.name);
				for (int i = 0; i < data.ItemCount; i++)
				{
					int index = data.StartIndex + i;
					Transform itemTransform = content.GetChild(i + offset);
					Action<int, int, Refers> onItemRender = this.OnItemRender;
					if (onItemRender != null)
					{
						onItemRender(data.GroupIndex, index, itemTransform.GetComponent<Refers>());
					}
				}
				bool doForceRebuildLayoutOnRender = this.DoForceRebuildLayoutOnRender;
				if (doForceRebuildLayoutOnRender)
				{
					RectTransform itemRect = item.GetComponent<RectTransform>();
					LayoutRebuilder.ForceRebuildLayoutImmediate(itemRect);
				}
			}
			else
			{
				bool isTitle = data.IsTitle;
				LayoutElement layoutElement = item.GetComponent<LayoutElement>();
				float ph = isTitle ? this._titleSize : this._contentSize;
				layoutElement.preferredHeight = ph;
				bool useFakeActive = this.UseFakeActive;
				if (useFakeActive)
				{
					title.localScale = (isTitle ? Vector3.one : Vector3.zero);
					content.localScale = ((!isTitle) ? Vector3.one : Vector3.zero);
				}
				else
				{
					title.gameObject.SetActive(isTitle);
					content.gameObject.SetActive(!isTitle);
				}
				bool isVertical = this.IsVertical;
				if (isVertical)
				{
					content.GetComponent<HorizontalLayoutGroup>().spacing = this.InLineSpacing;
				}
				else
				{
					content.GetComponent<VerticalLayoutGroup>().spacing = this.InLineSpacing;
				}
				bool flag3 = isTitle;
				if (flag3)
				{
					Action<int, Refers> onGroupTitleRender2 = this.OnGroupTitleRender;
					if (onGroupTitleRender2 != null)
					{
						onGroupTitleRender2(data.GroupIndex, title.GetComponent<Refers>());
					}
				}
				else
				{
					int offset2 = this.PrepareEnoughChildrenWithName(content, this.ContentItemTemplate.gameObject, data.ItemCount, this.ContentItemTemplate.gameObject.name);
					for (int j = 0; j < data.ItemCount; j++)
					{
						int index2 = data.StartIndex + j;
						Transform itemTransform2 = content.GetChild(j + offset2);
						Action<int, int, Refers> onItemRender2 = this.OnItemRender;
						if (onItemRender2 != null)
						{
							onItemRender2(data.GroupIndex, index2, itemTransform2.GetComponent<Refers>());
						}
					}
				}
			}
		}
	}

	// Token: 0x060003CF RID: 975 RVA: 0x00017434 File Offset: 0x00015634
	private int PrepareEnoughChildrenWithName(Transform parent, GameObject template, int count, string checkName)
	{
		int availableCount = 0;
		int offset = 0;
		for (int i = 0; i < parent.childCount; i++)
		{
			Transform child = parent.GetChild(i);
			bool flag = child.name == checkName;
			if (flag)
			{
				availableCount++;
				child.gameObject.SetActive(true);
				bool useFakeActiveWhenPrepareChildren = this.UseFakeActiveWhenPrepareChildren;
				if (useFakeActiveWhenPrepareChildren)
				{
					GroupedInfinityScroll.<PrepareEnoughChildrenWithName>g__FakeActive|33_0(child.gameObject);
				}
			}
			else
			{
				child.SetAsFirstSibling();
				offset++;
			}
		}
		while (availableCount < count)
		{
			Transform child2 = Object.Instantiate<GameObject>(template, parent).transform;
			child2.gameObject.SetActive(true);
			child2.name = checkName;
			availableCount++;
		}
		for (int j = offset + count; j < parent.childCount; j++)
		{
			bool useFakeActiveWhenPrepareChildren2 = this.UseFakeActiveWhenPrepareChildren;
			if (useFakeActiveWhenPrepareChildren2)
			{
				GroupedInfinityScroll.<PrepareEnoughChildrenWithName>g__FakeDeActive|33_1(parent.GetChild(j).gameObject);
			}
			else
			{
				parent.GetChild(j).gameObject.SetActive(false);
			}
		}
		return offset;
	}

	// Token: 0x060003D0 RID: 976 RVA: 0x00017550 File Offset: 0x00015750
	private void GenerateLineDataList(List<GroupedInfinityScroll.GroupItem> groups)
	{
		this._lineDataList.Clear();
		bool flag = groups == null;
		if (!flag)
		{
			bool flag2 = groups.Count == 1 && groups[0].Id == -1;
			if (flag2)
			{
				this.GenerateContentLines(0, -1, groups[0].ItemCount);
			}
			else
			{
				for (int i = 0; i < groups.Count; i++)
				{
					bool isOneGroupOneLine = this.IsOneGroupOneLine;
					if (isOneGroupOneLine)
					{
						this.GenerateContentLines(i, groups[i].Id, groups[i].ItemCount);
					}
					else
					{
						this._lineDataList.Add(new GroupedInfinityScroll.LineData
						{
							IsTitle = true,
							GroupId = groups[i].Id,
							GroupIndex = i
						});
						this.GenerateContentLines(i, groups[i].Id, groups[i].ItemCount);
					}
				}
			}
		}
	}

	// Token: 0x060003D1 RID: 977 RVA: 0x00017658 File Offset: 0x00015858
	private void GenerateContentLines(int groupIndex, int groupId, int itemCount)
	{
		bool isOneGroupOneLine = this.IsOneGroupOneLine;
		if (isOneGroupOneLine)
		{
			this._lineDataList.Add(new GroupedInfinityScroll.LineData
			{
				StartIndex = 0,
				ItemCount = itemCount,
				GroupId = groupId,
				GroupIndex = groupIndex
			});
		}
		else
		{
			for (int startIndex = 0; startIndex < itemCount; startIndex += this.ContentItemCountPerLine)
			{
				this._lineDataList.Add(new GroupedInfinityScroll.LineData
				{
					IsTitle = false,
					StartIndex = startIndex,
					ItemCount = Mathf.Min(this.ContentItemCountPerLine, itemCount - startIndex),
					GroupId = groupId,
					GroupIndex = groupIndex
				});
			}
		}
	}

	// Token: 0x060003D2 RID: 978 RVA: 0x00017709 File Offset: 0x00015909
	public void RefillCellsFromEnd()
	{
		this._loopScroll.RefillCellsFromEnd(0, false);
	}

	// Token: 0x060003D3 RID: 979 RVA: 0x0001771C File Offset: 0x0001591C
	public void RefillCellsByGroupIndex(int groupIndex)
	{
		int index = this._lineDataList.FindIndex((GroupedInfinityScroll.LineData d) => d.GroupIndex == groupIndex);
		this.LoopScroll.RefillCells(index, false);
		bool flag = index == this._lineDataList.Count - 1;
		if (flag)
		{
			SingletonObject.getInstance<YieldHelper>().DelayFrameDo(2U, delegate
			{
				this.LoopScroll.verticalScrollbar.value = 1f;
			});
		}
		else
		{
			this.LoopScroll.SetContentAnchoredPosition(Vector2.zero.SetY(0.1f));
		}
	}

	// Token: 0x060003D5 RID: 981 RVA: 0x00017808 File Offset: 0x00015A08
	[CompilerGenerated]
	internal static void <PrepareEnoughChildrenWithName>g__FakeActive|33_0(GameObject obj)
	{
		LayoutElement layoutElement = obj.GetComponent<LayoutElement>();
		CommonUtils.SetActiveByMove(obj.GetComponent<RectTransform>(), true);
		bool flag = layoutElement;
		if (flag)
		{
			layoutElement.ignoreLayout = false;
		}
	}

	// Token: 0x060003D6 RID: 982 RVA: 0x00017840 File Offset: 0x00015A40
	[CompilerGenerated]
	internal static void <PrepareEnoughChildrenWithName>g__FakeDeActive|33_1(GameObject obj)
	{
		LayoutElement layoutElement = obj.GetComponent<LayoutElement>();
		bool flag = layoutElement;
		if (flag)
		{
			layoutElement.ignoreLayout = true;
		}
		CommonUtils.SetActiveByMove(obj.GetComponent<RectTransform>(), false);
	}

	// Token: 0x0400024D RID: 589
	public LayoutElement LineTemplate;

	// Token: 0x0400024E RID: 590
	public Refers ContentItemTemplate;

	// Token: 0x0400024F RID: 591
	public int ContentItemCountPerLine = 1;

	// Token: 0x04000250 RID: 592
	public float InLineSpacing = 0f;

	// Token: 0x04000251 RID: 593
	public bool IsOneGroupOneLine;

	// Token: 0x04000252 RID: 594
	public Action<int, int, Refers> OnItemRender;

	// Token: 0x04000253 RID: 595
	public Action<int, Refers> OnGroupTitleRender;

	// Token: 0x04000254 RID: 596
	[Header("表现和性能相关选项")]
	[Tooltip("渲染完元素后，进行强制布局重建，目前只对IsOneGroupOneLine=true")]
	public bool DoForceRebuildLayoutOnRender = false;

	// Token: 0x04000255 RID: 597
	[Header("性能优化选项")]
	[Tooltip("使用假的Active来节省性能，目前只对IsOneGroupOneLine=false有效")]
	public bool UseFakeActive = false;

	// Token: 0x04000256 RID: 598
	[Tooltip("在准备子元素时使用假的Active来节省性能")]
	public bool UseFakeActiveWhenPrepareChildren = false;

	// Token: 0x04000257 RID: 599
	private float _contentSize;

	// Token: 0x04000258 RID: 600
	private float _titleSize;

	// Token: 0x04000259 RID: 601
	private readonly List<GroupedInfinityScroll.LineData> _lineDataList = new List<GroupedInfinityScroll.LineData>();

	// Token: 0x0400025A RID: 602
	private readonly Dictionary<GameObject, GroupedInfinityScroll.LineData> _renderingItems = new Dictionary<GameObject, GroupedInfinityScroll.LineData>();

	// Token: 0x0400025B RID: 603
	private LoopScrollRect _loopScroll;

	// Token: 0x0400025C RID: 604
	private bool _isNeedReRender;

	// Token: 0x020010DD RID: 4317
	public struct LineData
	{
		// Token: 0x04009498 RID: 38040
		public bool IsTitle;

		// Token: 0x04009499 RID: 38041
		public int GroupId;

		// Token: 0x0400949A RID: 38042
		public int GroupIndex;

		// Token: 0x0400949B RID: 38043
		public int StartIndex;

		// Token: 0x0400949C RID: 38044
		public int ItemCount;
	}

	// Token: 0x020010DE RID: 4318
	public struct GroupItem
	{
		// Token: 0x0600C0D7 RID: 49367 RVA: 0x0056C0A6 File Offset: 0x0056A2A6
		public GroupItem(int id, int itemCount)
		{
			this.Id = id;
			this.ItemCount = itemCount;
		}

		// Token: 0x0400949D RID: 38045
		public int Id;

		// Token: 0x0400949E RID: 38046
		public int ItemCount;
	}
}
