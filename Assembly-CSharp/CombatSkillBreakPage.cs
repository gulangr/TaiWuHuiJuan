using System;
using Game.Views.CharacterMenu;
using UnityEngine;
using UnityEngine.Events;

// Token: 0x020001DE RID: 478
public class CombatSkillBreakPage : Refers
{
	// Token: 0x17000327 RID: 807
	// (get) Token: 0x06001F6D RID: 8045 RVA: 0x000E575D File Offset: 0x000E395D
	private GameObject NoPage
	{
		get
		{
			return base.CGet<GameObject>("NoPage");
		}
	}

	// Token: 0x17000328 RID: 808
	// (get) Token: 0x06001F6E RID: 8046 RVA: 0x000E576A File Offset: 0x000E396A
	private TooltipInvoker NoPageTips
	{
		get
		{
			return base.CGet<TooltipInvoker>("NoPageTips");
		}
	}

	// Token: 0x17000329 RID: 809
	// (get) Token: 0x06001F6F RID: 8047 RVA: 0x000E5777 File Offset: 0x000E3977
	private GameObject NameBack
	{
		get
		{
			return base.CGet<GameObject>("NameBack");
		}
	}

	// Token: 0x1700032A RID: 810
	// (get) Token: 0x06001F70 RID: 8048 RVA: 0x000E5784 File Offset: 0x000E3984
	private TooltipInvoker PageTips
	{
		get
		{
			return base.CGet<TooltipInvoker>("PageTips");
		}
	}

	// Token: 0x1700032B RID: 811
	// (get) Token: 0x06001F71 RID: 8049 RVA: 0x000E5791 File Offset: 0x000E3991
	private GameObject HighLight
	{
		get
		{
			return base.CGet<GameObject>("HighLight");
		}
	}

	// Token: 0x1700032C RID: 812
	// (get) Token: 0x06001F72 RID: 8050 RVA: 0x000E579E File Offset: 0x000E399E
	private GameObject CanClickHighLight
	{
		get
		{
			return base.CGet<GameObject>("CanClickHighLight");
		}
	}

	// Token: 0x1700032D RID: 813
	// (get) Token: 0x06001F73 RID: 8051 RVA: 0x000E57AB File Offset: 0x000E39AB
	private GameObject MouseOver
	{
		get
		{
			return base.CGet<GameObject>("MouseOver");
		}
	}

	// Token: 0x1700032E RID: 814
	// (get) Token: 0x06001F74 RID: 8052 RVA: 0x000E57B8 File Offset: 0x000E39B8
	public CombatSkillBreakPage.EType Type
	{
		get
		{
			return this.pageType;
		}
	}

	// Token: 0x1700032F RID: 815
	// (get) Token: 0x06001F75 RID: 8053 RVA: 0x000E57C0 File Offset: 0x000E39C0
	public int Index
	{
		get
		{
			return this.pageIndex;
		}
	}

	// Token: 0x17000330 RID: 816
	// (get) Token: 0x06001F76 RID: 8054 RVA: 0x000E57C8 File Offset: 0x000E39C8
	public int InfoIndex
	{
		get
		{
			return this.pageInfoIndex;
		}
	}

	// Token: 0x06001F77 RID: 8055 RVA: 0x000E57D0 File Offset: 0x000E39D0
	private void Awake()
	{
		this._pageToggle = base.GetComponent<CToggleObsolete>();
		this._pointerTrigger = base.GetComponent<PointerTrigger>();
		this.MouseOver.SetActive(false);
		this.HighLight.SetActive(false);
		this._pointerTrigger.EnterEvent.AddListener(new UnityAction(this.PointerEnter));
		this._pointerTrigger.ExitEvent.AddListener(new UnityAction(this.PointerExit));
	}

	// Token: 0x06001F78 RID: 8056 RVA: 0x000E584C File Offset: 0x000E3A4C
	private void PointerEnter()
	{
		this._mouseOvering = true;
		bool interactable = this._pageToggle.interactable;
		if (interactable)
		{
			this.MouseOver.SetActive(true);
		}
	}

	// Token: 0x06001F79 RID: 8057 RVA: 0x000E5880 File Offset: 0x000E3A80
	private void PointerExit()
	{
		this._mouseOvering = false;
		bool flag = !this._pageToggle.isOn;
		if (flag)
		{
			this.MouseOver.SetActive(false);
		}
	}

	// Token: 0x06001F7A RID: 8058 RVA: 0x000E58B4 File Offset: 0x000E3AB4
	public void UpdateSkill(short skillTemplateId)
	{
		this._cachedSkillTemplateId = skillTemplateId;
		bool flag = this.pageType == CombatSkillBreakPage.EType.Outline;
		if (flag)
		{
			PracticeSkillPlatePageUtils.RefreshOutlinePageTip(this.PageTips, (sbyte)this.pageIndex, false, false);
		}
	}

	// Token: 0x06001F7B RID: 8059 RVA: 0x000E58ED File Offset: 0x000E3AED
	public void ChangeInteractable(bool interactable)
	{
		this._pageToggle.interactable = interactable;
	}

	// Token: 0x06001F7C RID: 8060 RVA: 0x000E5900 File Offset: 0x000E3B00
	public void ChangePageShow(bool showPage)
	{
		base.GetComponent<CImage>().SetAlpha(showPage ? 1f : 0.5f);
		this.NameBack.SetActive(showPage);
		this.NoPage.SetActive(!showPage);
		this.CanClickHighLight.SetActive(showPage);
		this.PageTips.gameObject.SetActive(showPage);
		this.NoPageTips.gameObject.SetActive(!showPage);
	}

	// Token: 0x06001F7D RID: 8061 RVA: 0x000E597A File Offset: 0x000E3B7A
	public void ChangeSelected(bool selected)
	{
		this.HighLight.SetActive(selected);
		this.MouseOver.SetActive(selected || this._mouseOvering);
	}

	// Token: 0x040017A8 RID: 6056
	private CToggleObsolete _pageToggle;

	// Token: 0x040017A9 RID: 6057
	private PointerTrigger _pointerTrigger;

	// Token: 0x040017AA RID: 6058
	[SerializeField]
	private CombatSkillBreakPage.EType pageType;

	// Token: 0x040017AB RID: 6059
	[SerializeField]
	private int pageIndex;

	// Token: 0x040017AC RID: 6060
	[SerializeField]
	private int pageInfoIndex;

	// Token: 0x040017AD RID: 6061
	private short _cachedSkillTemplateId;

	// Token: 0x040017AE RID: 6062
	private bool _mouseOvering;

	// Token: 0x02001466 RID: 5222
	public enum EType
	{
		// Token: 0x0400A13E RID: 41278
		Outline,
		// Token: 0x0400A13F RID: 41279
		NormalDirect,
		// Token: 0x0400A140 RID: 41280
		NormalReverse
	}
}
