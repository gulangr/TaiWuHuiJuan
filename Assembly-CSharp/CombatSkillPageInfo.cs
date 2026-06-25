using System;
using System.Collections.Generic;
using GameData.Domains.Item;
using GameData.Domains.Item.Display;
using TMPro;
using UnityEngine;

// Token: 0x020001E3 RID: 483
public class CombatSkillPageInfo : Refers
{
	// Token: 0x06001FA5 RID: 8101 RVA: 0x000E67D0 File Offset: 0x000E49D0
	public void Init()
	{
		this._title = base.CGet<TextMeshProUGUI>("Title");
		this._infoHolder = base.CGet<GameObject>("InfoHolder");
		this._changeIconEnable = base.CGet<GameObject>("ChangeIconEnable");
		this._changeIconDisable = base.CGet<GameObject>("ChangeIconDisable");
		this._changeBtn = base.CGet<CButtonObsolete>("ChangeBtn");
		this._pageIconHolder = base.CGet<RectTransform>("PageIconHolder");
		this._pageInfos = this._infoHolder.GetComponentsInChildren<Refers>();
		this._pageIcons = this._pageIconHolder.GetComponentsInChildren<CImage>();
	}

	// Token: 0x06001FA6 RID: 8102 RVA: 0x000E6866 File Offset: 0x000E4A66
	public void UpdatePagesInfo(bool canChange)
	{
		this._changeIconDisable.SetActive(!canChange);
		this._changeIconEnable.SetActive(canChange);
		this._changeBtn.enabled = canChange;
	}

	// Token: 0x06001FA7 RID: 8103 RVA: 0x000E6894 File Offset: 0x000E4A94
	public void UpdatePagesInfo(SkillBookModifyDisplayData selectingData, bool isOutlineType)
	{
		this._selectingData = selectingData;
		this._title.text = (isOutlineType ? LocalStringManager.Get(LanguageKey.LK_CombatSkillModifyBook_Title_Outline) : LocalStringManager.Get(LanguageKey.LK_CombatSkillModifyBook_Title_Normal));
		for (int i = 0; i < this._pageInfos.Length; i++)
		{
			Refers page = this._pageInfos[i];
			sbyte state = SkillBookStateHelper.GetPageIncompleteState(this._selectingData.PageIncompleteState, (byte)i);
			bool flag = i == 0;
			string name;
			if (flag)
			{
				sbyte type = SkillBookStateHelper.GetOutlinePageType(this._selectingData.PageTypes);
				name = this.GetPageName(i, (int)type, 0);
			}
			else
			{
				sbyte direction = SkillBookStateHelper.GetNormalPageType(this._selectingData.PageTypes, (byte)i);
				name = this.GetPageName(i, i, (int)direction);
			}
			this.UpdatePageInfo(page, name, state);
		}
	}

	// Token: 0x06001FA8 RID: 8104 RVA: 0x000E6960 File Offset: 0x000E4B60
	public void UpdatePagesInfo(Dictionary<int, CombatSkillBreakPlate.PageInfo> pageInfoDict)
	{
		for (int i = 0; i < this._pageInfos.Length; i++)
		{
			CombatSkillBreakPlate.PageInfo pageInfo = pageInfoDict[i];
			this.UpdatePagesInfo(i, pageInfo);
		}
	}

	// Token: 0x06001FA9 RID: 8105 RVA: 0x000E6998 File Offset: 0x000E4B98
	public void UpdatePagesInfo(int pageInfoIndex, CombatSkillBreakPlate.PageInfo pageInfo)
	{
		Refers page = this._pageInfos[pageInfoIndex];
		string name = this.GetPageName(pageInfoIndex, pageInfo.index, pageInfo.direction);
		this.UpdatePageInfo(page, name);
	}

	// Token: 0x06001FAA RID: 8106 RVA: 0x000E69CC File Offset: 0x000E4BCC
	private string GetPageName(int pageInfoIndex, int pageIndex, int direction)
	{
		bool flag = pageInfoIndex == 0;
		string pageName;
		if (flag)
		{
			pageName = LocalStringManager.Get(string.Format("LK_CombatSkill_First_Page_Type_{0}", pageIndex)).SetColor("pinkyellow");
			this._pageIcons[pageInfoIndex].gameObject.SetActive(true);
		}
		else
		{
			bool direct = direction == 0;
			bool flag2 = direct;
			if (flag2)
			{
				pageName = LocalStringManager.Get(string.Format("LK_CombatSkill_Direct_Page_{0}", pageIndex - 1)).SetColor("brightblue");
			}
			else
			{
				pageName = LocalStringManager.Get(string.Format("LK_CombatSkill_Reverse_Page_{0}", pageIndex - 1)).SetColor("brightred");
			}
			this._pageIcons[pageInfoIndex].SetSprite(direct ? CombatSkillView.BrokenPageIcon[0] : CombatSkillView.BrokenPageIcon[1], false, null);
		}
		return pageName;
	}

	// Token: 0x06001FAB RID: 8107 RVA: 0x000E6A9C File Offset: 0x000E4C9C
	private void UpdatePageInfo(Refers page, string name, sbyte state)
	{
		this.UpdatePageInfo(page, name);
		page.CGet<GameObject>("CompleteTips").SetActive(state == 0);
		page.CGet<GameObject>("IncompleteTips").SetActive(state == 1);
		page.CGet<GameObject>("IncompleteTips").GetComponent<TextMeshProUGUI>().SetText(LocalStringManager.Get(LanguageKey.LK_Book_Page_State_Incomplete), true);
		page.CGet<GameObject>("LostTips").SetActive(state == 2);
	}

	// Token: 0x06001FAC RID: 8108 RVA: 0x000E6B13 File Offset: 0x000E4D13
	private void UpdatePageInfo(Refers page, string name)
	{
		page.CGet<TextMeshProUGUI>("PageName").text = name;
	}

	// Token: 0x040017CD RID: 6093
	private Refers[] _pageInfos;

	// Token: 0x040017CE RID: 6094
	private CImage[] _pageIcons;

	// Token: 0x040017CF RID: 6095
	private TextMeshProUGUI _title;

	// Token: 0x040017D0 RID: 6096
	private GameObject _infoHolder;

	// Token: 0x040017D1 RID: 6097
	private GameObject _changeIconEnable;

	// Token: 0x040017D2 RID: 6098
	private GameObject _changeIconDisable;

	// Token: 0x040017D3 RID: 6099
	private CButtonObsolete _changeBtn;

	// Token: 0x040017D4 RID: 6100
	private RectTransform _pageIconHolder;

	// Token: 0x040017D5 RID: 6101
	private SkillBookModifyDisplayData _selectingData;
}
