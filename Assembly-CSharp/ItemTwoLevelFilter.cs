using System;
using UnityEngine;

// Token: 0x02000353 RID: 851
public class ItemTwoLevelFilter : Refers
{
	// Token: 0x17000566 RID: 1382
	// (get) Token: 0x060031B3 RID: 12723 RVA: 0x0018926A File Offset: 0x0018746A
	public SimpleItemMainFilter.ItemFilterType MainFilterType
	{
		get
		{
			return this._simpleItemMainFilter.SelectedType;
		}
	}

	// Token: 0x17000567 RID: 1383
	// (get) Token: 0x060031B4 RID: 12724 RVA: 0x00189277 File Offset: 0x00187477
	public Enum SubFilterType
	{
		get
		{
			return this._simpleItemSubFilter.SelectedSubType;
		}
	}

	// Token: 0x060031B5 RID: 12725 RVA: 0x00189284 File Offset: 0x00187484
	public void Init()
	{
		this.InitRefers();
		this._simpleItemMainFilter.Init();
		this._simpleItemSubFilter.Init();
		this._simpleItemMainFilter.SetCallback(new Action(this.OnMainFilterTypeChanged));
		this._simpleItemSubFilter.SetCallback(new Action(this.OnSubFilterTypeChanged));
	}

	// Token: 0x060031B6 RID: 12726 RVA: 0x001892E1 File Offset: 0x001874E1
	public void SetCallback(Action onSelectedTypeChanged)
	{
		this._onSelectedTypeChanged = onSelectedTypeChanged;
	}

	// Token: 0x060031B7 RID: 12727 RVA: 0x001892EB File Offset: 0x001874EB
	public void SetFilterType(SimpleItemMainFilter.ItemFilterType filtertype)
	{
		this._simpleItemMainFilter.SelectedType = filtertype;
	}

	// Token: 0x060031B8 RID: 12728 RVA: 0x001892FB File Offset: 0x001874FB
	private void OnMainFilterTypeChanged()
	{
		this._simpleItemSubFilter.Refresh(this._simpleItemMainFilter.SelectedType);
	}

	// Token: 0x060031B9 RID: 12729 RVA: 0x00189315 File Offset: 0x00187515
	private void OnSubFilterTypeChanged()
	{
		Action onSelectedTypeChanged = this._onSelectedTypeChanged;
		if (onSelectedTypeChanged != null)
		{
			onSelectedTypeChanged();
		}
	}

	// Token: 0x060031BA RID: 12730 RVA: 0x0018932C File Offset: 0x0018752C
	public bool IsItemMatch(sbyte itemType, short templateId)
	{
		bool mainMatch = this._simpleItemMainFilter.IsItemTypeMatch(itemType);
		bool flag = !mainMatch;
		bool result;
		if (flag)
		{
			result = false;
		}
		else
		{
			bool flag2 = this._simpleItemMainFilter.SelectedType == SimpleItemMainFilter.ItemFilterType.All;
			if (flag2)
			{
				result = true;
			}
			else
			{
				bool subMatch = this._simpleItemSubFilter.IsItemMatch(itemType, templateId);
				result = subMatch;
			}
		}
		return result;
	}

	// Token: 0x060031BB RID: 12731 RVA: 0x00189380 File Offset: 0x00187580
	public int GetSubFilterActiveToggleCount()
	{
		Transform parent = this._simpleItemSubFilter.transform;
		int activeChildCount = 0;
		for (int i = 0; i < parent.childCount; i++)
		{
			bool activeSelf = parent.GetChild(i).gameObject.activeSelf;
			if (activeSelf)
			{
				activeChildCount++;
			}
		}
		return activeChildCount;
	}

	// Token: 0x060031BC RID: 12732 RVA: 0x001893D8 File Offset: 0x001875D8
	public RectTransform GetSelectedMainFilterToggleTransform()
	{
		return this._simpleItemMainFilter.GetSelectedToggleTransform();
	}

	// Token: 0x17000568 RID: 1384
	// (get) Token: 0x060031BD RID: 12733 RVA: 0x001893F5 File Offset: 0x001875F5
	public RectTransform MainFilterTransform
	{
		get
		{
			return this._simpleItemMainFilter.GetComponent<RectTransform>();
		}
	}

	// Token: 0x17000569 RID: 1385
	// (get) Token: 0x060031BE RID: 12734 RVA: 0x00189402 File Offset: 0x00187602
	public RectTransform SubFilterTransform
	{
		get
		{
			return this._simpleItemSubFilter.GetComponent<RectTransform>();
		}
	}

	// Token: 0x060031BF RID: 12735 RVA: 0x0018940F File Offset: 0x0018760F
	private void InitRefers()
	{
		this._simpleItemMainFilter = base.CGet<SimpleItemMainFilter>("SimpleItemMainFilter");
		this._simpleItemSubFilter = base.CGet<SimpleItemSubFilter>("SimpleItemSubFilter");
	}

	// Token: 0x04002473 RID: 9331
	private Action _onSelectedTypeChanged;

	// Token: 0x04002474 RID: 9332
	private SimpleItemMainFilter _simpleItemMainFilter;

	// Token: 0x04002475 RID: 9333
	private SimpleItemSubFilter _simpleItemSubFilter;
}
