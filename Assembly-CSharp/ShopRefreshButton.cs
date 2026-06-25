using System;
using TMPro;
using UnityEngine;

// Token: 0x02000300 RID: 768
public class ShopRefreshButton : MonoBehaviour
{
	// Token: 0x06002CF8 RID: 11512 RVA: 0x00161B10 File Offset: 0x0015FD10
	private void Awake()
	{
		IShopRefresh parent;
		bool flag;
		if (this.Parent)
		{
			parent = (this.Parent as IShopRefresh);
			flag = (parent != null);
		}
		else
		{
			flag = false;
		}
		bool flag2 = flag;
		if (flag2)
		{
			this._parent = parent;
		}
		else
		{
			GLog.Error("Parent is null or invalid");
		}
	}

	// Token: 0x06002CF9 RID: 11513 RVA: 0x00161B5C File Offset: 0x0015FD5C
	private void LateUpdate()
	{
		bool enable = this._parent.CanRefreshCurrentGoods && !this._parent.Protected;
		bool flag = this.Button.interactable ^ enable;
		if (flag)
		{
			this.Button.interactable = enable;
		}
		this.Enable.SetActive(enable);
		this.Disable.SetActive(!enable);
	}

	// Token: 0x06002CFA RID: 11514 RVA: 0x00161BC8 File Offset: 0x0015FDC8
	private void OnEnable()
	{
		this._parent.InitShopRefresh(new Action(this.RefreshCount), new Action<bool>(this.RefreshActiveStates), new Action<bool>(this.RefreshTips));
		this.MouseTipDisplayer.PresetParam[0] = this._parent.RefreshTipTitle;
		this.Count.gameObject.SetActive(false);
	}

	// Token: 0x06002CFB RID: 11515 RVA: 0x00161C30 File Offset: 0x0015FE30
	public void RefreshActiveStates(bool active)
	{
		base.gameObject.SetActive(active);
	}

	// Token: 0x06002CFC RID: 11516 RVA: 0x00161C40 File Offset: 0x0015FE40
	public void OnClick(string str)
	{
		bool canRefreshCurrentGoods = this._parent.CanRefreshCurrentGoods;
		if (canRefreshCurrentGoods)
		{
			this._parent.RefreshCurrentGoods();
		}
		else
		{
			this._parent.NoticeCannotRefreshCurrentGoods();
		}
	}

	// Token: 0x06002CFD RID: 11517 RVA: 0x00161C7B File Offset: 0x0015FE7B
	public void OnHoverStart(string _)
	{
	}

	// Token: 0x06002CFE RID: 11518 RVA: 0x00161C7E File Offset: 0x0015FE7E
	public void OnHoverEnd(string _)
	{
	}

	// Token: 0x06002CFF RID: 11519 RVA: 0x00161C84 File Offset: 0x0015FE84
	public void RefreshTips(bool needClear)
	{
		this.MouseTipDisplayer.PresetParam[1] = this._parent.RefreshTips(needClear);
		this.MouseTipDisplayer.Refresh(false, -1);
		base.gameObject.SetActive(this._parent.CanShow);
	}

	// Token: 0x06002D00 RID: 11520 RVA: 0x00161CD0 File Offset: 0x0015FED0
	public void RefreshCount()
	{
	}

	// Token: 0x040020A3 RID: 8355
	public MonoBehaviour Parent;

	// Token: 0x040020A4 RID: 8356
	public TextMeshProUGUI Count;

	// Token: 0x040020A5 RID: 8357
	public TooltipInvoker MouseTipDisplayer;

	// Token: 0x040020A6 RID: 8358
	public GameObject Enable;

	// Token: 0x040020A7 RID: 8359
	public GameObject Disable;

	// Token: 0x040020A8 RID: 8360
	public CButtonObsolete Button;

	// Token: 0x040020A9 RID: 8361
	private IShopRefresh _parent;
}
