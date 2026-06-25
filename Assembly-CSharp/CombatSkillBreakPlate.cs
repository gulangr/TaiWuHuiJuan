using System;
using System.Collections.Generic;
using Game.Views.CharacterMenu;
using GameData.Domains.CombatSkill;
using GameData.Domains.Item.Display;
using UnityEngine;

// Token: 0x020001DF RID: 479
public class CombatSkillBreakPlate : Refers
{
	// Token: 0x17000331 RID: 817
	// (get) Token: 0x06001F7F RID: 8063 RVA: 0x000E59AC File Offset: 0x000E3BAC
	public Dictionary<int, CombatSkillBreakPlate.PageInfo> PageInfoDict
	{
		get
		{
			return this._pageInfoDict;
		}
	}

	// Token: 0x17000332 RID: 818
	// (get) Token: 0x06001F80 RID: 8064 RVA: 0x000E59C4 File Offset: 0x000E3BC4
	public Dictionary<int, CombatSkillBreakPlate.PageInfo> PageInfoDictCache
	{
		get
		{
			return this._pageInfoDictCache;
		}
	}

	// Token: 0x17000333 RID: 819
	// (get) Token: 0x06001F81 RID: 8065 RVA: 0x000E59CC File Offset: 0x000E3BCC
	private CToggleGroupObsolete OutlineToggleGroup
	{
		get
		{
			return base.CGet<CToggleGroupObsolete>("OutlineTogGroup");
		}
	}

	// Token: 0x17000334 RID: 820
	// (get) Token: 0x06001F82 RID: 8066 RVA: 0x000E59D9 File Offset: 0x000E3BD9
	private CToggleGroupObsolete NormalToggleGroup
	{
		get
		{
			return base.CGet<CToggleGroupObsolete>("OtherPageTogGroup");
		}
	}

	// Token: 0x17000335 RID: 821
	// (get) Token: 0x06001F83 RID: 8067 RVA: 0x000E59E6 File Offset: 0x000E3BE6
	private GameObject NoSelectImg
	{
		get
		{
			return base.CGet<GameObject>("NoSelectImg");
		}
	}

	// Token: 0x17000336 RID: 822
	// (get) Token: 0x06001F84 RID: 8068 RVA: 0x000E59F3 File Offset: 0x000E3BF3
	private CombatSkillPageInfo CombatSkillPageInfo
	{
		get
		{
			return base.CGet<CombatSkillPageInfo>("CombatSkillPageInfo");
		}
	}

	// Token: 0x06001F85 RID: 8069 RVA: 0x000E5A00 File Offset: 0x000E3C00
	private void Awake()
	{
		bool callbackRegistered = this._callbackRegistered;
		if (!callbackRegistered)
		{
			this._callbackRegistered = true;
			this.OutlineToggleGroup.InitPreOnToggle(-1);
			CToggleGroupObsolete outlineToggleGroup = this.OutlineToggleGroup;
			outlineToggleGroup.OnActiveToggleChange = (Action<CToggleObsolete, CToggleObsolete>)Delegate.Combine(outlineToggleGroup.OnActiveToggleChange, new Action<CToggleObsolete, CToggleObsolete>(this.OnActiveToggleChangedOutline));
			this.InitNormalPageReverseMap();
			this.NormalToggleGroup.InitPreOnToggle(-1);
			CToggleGroupObsolete normalToggleGroup = this.NormalToggleGroup;
			normalToggleGroup.OnActiveToggleChange = (Action<CToggleObsolete, CToggleObsolete>)Delegate.Combine(normalToggleGroup.OnActiveToggleChange, new Action<CToggleObsolete, CToggleObsolete>(this.OnActiveToggleChangeNormal));
			this.CombatSkillPageInfo.Init();
		}
	}

	// Token: 0x06001F86 RID: 8070 RVA: 0x000E5AA0 File Offset: 0x000E3CA0
	private void InitNormalPageReverseMap()
	{
		List<CToggleObsolete> pageTogList = this.NormalToggleGroup.GetAll();
		for (byte i = 1; i <= 5; i += 1)
		{
			int directKey = (int)CombatSkillStateHelper.GetNormalPageInternalIndex(0, i);
			int reverseKey = (int)CombatSkillStateHelper.GetNormalPageInternalIndex(1, i);
			pageTogList[(int)(i - 1)].Key = directKey;
			pageTogList[(int)(5 + i - 1)].Key = reverseKey;
			this._reverseOtherPageDict.Add(directKey, reverseKey);
			this._reverseOtherPageDict.Add(reverseKey, directKey);
		}
	}

	// Token: 0x06001F87 RID: 8071 RVA: 0x000E5B20 File Offset: 0x000E3D20
	private void OnActiveToggleChangedOutline(CToggleObsolete togNew, CToggleObsolete togOld)
	{
		bool flag = togOld != null;
		if (flag)
		{
			CombatSkillBreakPage combatSkillBreakPage = togOld.GetComponent<CombatSkillBreakPage>();
			combatSkillBreakPage.ChangeSelected(false);
			this._pageIndexesTemp.Remove(togOld.Key);
		}
		bool flag2 = togNew != null;
		if (flag2)
		{
			CombatSkillBreakPage combatSkillBreakPage2 = togNew.GetComponent<CombatSkillBreakPage>();
			combatSkillBreakPage2.ChangeSelected(true);
			this._pageIndexesTemp.Add(togNew.Key);
			int direction = 0;
			CombatSkillBreakPage.EType type = combatSkillBreakPage2.Type;
			CombatSkillBreakPage.EType etype = type;
			if (etype != CombatSkillBreakPage.EType.NormalDirect)
			{
				if (etype == CombatSkillBreakPage.EType.NormalReverse)
				{
					direction = 1;
				}
			}
			else
			{
				direction = 0;
			}
			CombatSkillBreakPlate.PageInfo pageInfo = new CombatSkillBreakPlate.PageInfo
			{
				key = togNew.Key,
				direction = direction,
				index = combatSkillBreakPage2.Index
			};
			this._pageInfoDict[combatSkillBreakPage2.InfoIndex] = pageInfo;
			this.CombatSkillPageInfo.UpdatePagesInfo(combatSkillBreakPage2.InfoIndex, pageInfo);
		}
		this.CombatSkillPageInfo.UpdatePagesInfo(this.IsPageTypeChange());
		this.UpdateAllOutlinePageInteractable();
	}

	// Token: 0x06001F88 RID: 8072 RVA: 0x000E5C24 File Offset: 0x000E3E24
	private void OnActiveToggleChangeNormal(CToggleObsolete togNew, CToggleObsolete togOld)
	{
		this.OnActiveToggleChangedOutline(togNew, togOld);
		bool flag = togNew != null;
		if (flag)
		{
			int reverseKey = this._reverseOtherPageDict[togNew.Key];
			this.NormalToggleGroup.Set(reverseKey, false, false);
		}
		this.UpdateAllNormalPageInteractable();
	}

	// Token: 0x06001F89 RID: 8073 RVA: 0x000E5C70 File Offset: 0x000E3E70
	public void InitProvider(CombatSkillBreakPlate.ActivatedProvider activatedProvider, CombatSkillBreakPlate.PageExistProvider pageExistProvider, Func<bool> isOutlineTypeProvider, CombatSkillBreakPlate.PageExistProvider pageShowProvider)
	{
		this._activatedProvider = activatedProvider;
		this._pageExistProvider = pageExistProvider;
		this._interactableProvider = pageShowProvider;
		this.IsOutlineTypeProvider = isOutlineTypeProvider;
	}

	// Token: 0x06001F8A RID: 8074 RVA: 0x000E5C90 File Offset: 0x000E3E90
	public void SetSkillTemplateData(SkillBookModifyDisplayData selectingData)
	{
		this._cachedSelectingSkillData = selectingData;
	}

	// Token: 0x06001F8B RID: 8075 RVA: 0x000E5C9C File Offset: 0x000E3E9C
	public void UpdateBreakPlate()
	{
		bool flag = !this._selected;
		if (!flag)
		{
			this._pageInfoDict.Clear();
			this._pageInfoDictCache.Clear();
			CToggleObsolete checkedOutline = this.OutlineToggleGroup.GetActive();
			List<CToggleObsolete> checkedPages = this.NormalToggleGroup.GetAll();
			bool flag2 = checkedOutline != null;
			if (flag2)
			{
				this.OutlineToggleGroup.Set(checkedOutline, false);
			}
			foreach (CToggleObsolete pageTog in checkedPages)
			{
				this.NormalToggleGroup.Set(pageTog, false);
			}
			int outlineKey = this._activatedProvider(0);
			this.OutlineToggleGroup.Set(outlineKey, true, false);
			HashSet<int> normalKeys = new HashSet<int>();
			for (byte i = 1; i <= 5; i += 1)
			{
				int index = this._activatedProvider(i);
				this.NormalToggleGroup.Set(index, true, false);
				normalKeys.Add(index);
			}
			sbyte j = 0;
			while ((int)j < this.OutlineToggleGroup.Count())
			{
				CombatSkillBreakPage breakPage = this.OutlineToggleGroup.Get((int)j).GetComponent<CombatSkillBreakPage>();
				byte outlineIndex = CombatSkillStateHelper.GetOutlinePageInternalIndex(j);
				this.UpdateBreakPage(breakPage, outlineIndex);
				j += 1;
			}
			for (byte k = 1; k <= 5; k += 1)
			{
				byte directIndex = CombatSkillStateHelper.GetNormalPageInternalIndex(0, k);
				byte reverseIndex = CombatSkillStateHelper.GetNormalPageInternalIndex(1, k);
				CombatSkillBreakPage directBreakPage = this.NormalToggleGroup.Get((int)directIndex).GetComponent<CombatSkillBreakPage>();
				CombatSkillBreakPage reverseBreakPage = this.NormalToggleGroup.Get((int)reverseIndex).GetComponent<CombatSkillBreakPage>();
				TooltipInvoker directTip = directBreakPage.CGet<TooltipInvoker>("PageTips");
				TooltipInvoker directTipNoPage = directBreakPage.CGet<TooltipInvoker>("NoPageTips");
				TooltipInvoker reverseTip = reverseBreakPage.CGet<TooltipInvoker>("PageTips");
				TooltipInvoker reverseTipNoPage = reverseBreakPage.CGet<TooltipInvoker>("NoPageTips");
				this.UpdateBreakPage(directBreakPage, directIndex);
				this.UpdateBreakPage(reverseBreakPage, reverseIndex);
				PracticeSkillPlatePageUtils.RefreshOtherPageTips((int)(k - 1), directTip, this._cachedSelectingSkillData.TemplateId, true, false, false, false, null);
				PracticeSkillPlatePageUtils.RefreshOtherPageTips((int)(k - 1), directTipNoPage, this._cachedSelectingSkillData.TemplateId, true, false, false, false, null);
				PracticeSkillPlatePageUtils.RefreshOtherPageTips((int)(k - 1), reverseTip, this._cachedSelectingSkillData.TemplateId, false, false, false, false, null);
				PracticeSkillPlatePageUtils.RefreshOtherPageTips((int)(k - 1), reverseTipNoPage, this._cachedSelectingSkillData.TemplateId, false, false, false, false, null);
			}
			this._pageIndexes.Clear();
			this._pageIndexesTemp.Clear();
			this._pageIndexes.UnionWith(normalKeys);
			this._pageIndexes.Add(outlineKey);
			this._pageIndexesTemp.UnionWith(this._pageIndexes);
			this._pageInfoDictCache = new Dictionary<int, CombatSkillBreakPlate.PageInfo>(this._pageInfoDict);
			this.CombatSkillPageInfo.UpdatePagesInfo(this._cachedSelectingSkillData, this.IsOutlineTypeProvider());
			this.CombatSkillPageInfo.UpdatePagesInfo(this.IsPageTypeChange());
		}
	}

	// Token: 0x06001F8C RID: 8076 RVA: 0x000E5FA8 File Offset: 0x000E41A8
	public void ChangeSelected(bool selected)
	{
		bool flag = this._selected == selected;
		if (!flag)
		{
			this._selected = selected;
			this.OutlineToggleGroup.gameObject.SetActive(selected);
			this.NormalToggleGroup.gameObject.SetActive(selected);
			this.NoSelectImg.SetActive(!selected);
			this.CombatSkillPageInfo.gameObject.SetActive(selected);
		}
	}

	// Token: 0x06001F8D RID: 8077 RVA: 0x000E6014 File Offset: 0x000E4214
	private void UpdateBreakPage(CombatSkillBreakPage breakPage, byte internalIndex)
	{
		bool showPage = this._pageExistProvider(internalIndex);
		breakPage.ChangePageShow(showPage);
		breakPage.ChangeInteractable(showPage && this._interactableProvider(internalIndex));
		breakPage.UpdateSkill(this._cachedSelectingSkillData.TemplateId);
	}

	// Token: 0x06001F8E RID: 8078 RVA: 0x000E6064 File Offset: 0x000E4264
	private void UpdateAllNormalPageInteractable()
	{
		for (byte i = 1; i <= 5; i += 1)
		{
			byte directIndex = CombatSkillStateHelper.GetNormalPageInternalIndex(0, i);
			byte reverseIndex = CombatSkillStateHelper.GetNormalPageInternalIndex(1, i);
			CombatSkillBreakPage directBreakPage = this.NormalToggleGroup.Get((int)directIndex).GetComponent<CombatSkillBreakPage>();
			CombatSkillBreakPage reverseBreakPage = this.NormalToggleGroup.Get((int)reverseIndex).GetComponent<CombatSkillBreakPage>();
			bool showDirectPage = this._pageExistProvider(directIndex);
			directBreakPage.ChangeInteractable(showDirectPage && this._interactableProvider(directIndex));
			bool showReversePage = this._pageExistProvider(reverseIndex);
			reverseBreakPage.ChangeInteractable(showReversePage && this._interactableProvider(reverseIndex));
		}
	}

	// Token: 0x06001F8F RID: 8079 RVA: 0x000E6118 File Offset: 0x000E4318
	private void UpdateAllOutlinePageInteractable()
	{
		sbyte i = 0;
		while ((int)i < this.OutlineToggleGroup.Count())
		{
			CombatSkillBreakPage breakPage = this.OutlineToggleGroup.Get((int)i).GetComponent<CombatSkillBreakPage>();
			byte outlineIndex = CombatSkillStateHelper.GetOutlinePageInternalIndex(i);
			bool showPage = this._pageExistProvider(outlineIndex);
			breakPage.ChangeInteractable(showPage && this._interactableProvider(outlineIndex));
			i += 1;
		}
	}

	// Token: 0x06001F90 RID: 8080 RVA: 0x000E6184 File Offset: 0x000E4384
	private bool IsPageTypeChange()
	{
		return !this._pageIndexes.SetEquals(this._pageIndexesTemp) && this._pageIndexes.Count == this._pageIndexesTemp.Count;
	}

	// Token: 0x040017AF RID: 6063
	public Func<bool> IsOutlineTypeProvider;

	// Token: 0x040017B0 RID: 6064
	private CombatSkillBreakPlate.ActivatedProvider _activatedProvider;

	// Token: 0x040017B1 RID: 6065
	private CombatSkillBreakPlate.PageExistProvider _pageExistProvider;

	// Token: 0x040017B2 RID: 6066
	private CombatSkillBreakPlate.PageExistProvider _interactableProvider;

	// Token: 0x040017B3 RID: 6067
	private readonly Dictionary<int, int> _reverseOtherPageDict = new Dictionary<int, int>();

	// Token: 0x040017B4 RID: 6068
	private SkillBookModifyDisplayData _cachedSelectingSkillData;

	// Token: 0x040017B5 RID: 6069
	private bool _selected = true;

	// Token: 0x040017B6 RID: 6070
	private bool _callbackRegistered;

	// Token: 0x040017B7 RID: 6071
	private HashSet<int> _pageIndexes = new HashSet<int>();

	// Token: 0x040017B8 RID: 6072
	private HashSet<int> _pageIndexesTemp = new HashSet<int>();

	// Token: 0x040017B9 RID: 6073
	private Dictionary<int, CombatSkillBreakPlate.PageInfo> _pageInfoDict = new Dictionary<int, CombatSkillBreakPlate.PageInfo>();

	// Token: 0x040017BA RID: 6074
	private Dictionary<int, CombatSkillBreakPlate.PageInfo> _pageInfoDictCache = new Dictionary<int, CombatSkillBreakPlate.PageInfo>();

	// Token: 0x02001467 RID: 5223
	// (Invoke) Token: 0x0600CBD8 RID: 52184
	public delegate int ActivatedProvider(byte page);

	// Token: 0x02001468 RID: 5224
	// (Invoke) Token: 0x0600CBDC RID: 52188
	public delegate bool PageExistProvider(byte pageInternalIndex);

	// Token: 0x02001469 RID: 5225
	public struct PageInfo
	{
		// Token: 0x0400A141 RID: 41281
		public int direction;

		// Token: 0x0400A142 RID: 41282
		public int key;

		// Token: 0x0400A143 RID: 41283
		public int index;
	}
}
