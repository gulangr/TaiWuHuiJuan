using System;
using System.Collections.Generic;
using FrameWork;
using TMPro;
using UnityEngine;

// Token: 0x0200024D RID: 589
public class UI_LifeSkillCombatCardGroup : UIBase
{
	// Token: 0x0600272A RID: 10026 RVA: 0x001211FC File Offset: 0x0011F3FC
	public override void OnInit(ArgumentBox argsBox)
	{
		argsBox.Get<List<short>>("CardList", out this._cardList);
		int type;
		argsBox.Get("Type", out type);
		if (!true)
		{
		}
		LanguageKey languageKey;
		switch (type)
		{
		case 0:
			languageKey = LanguageKey.LK_LifeSkillCombat_CardGroup_Owned;
			break;
		case 1:
			languageKey = LanguageKey.LK_LifeSkillCombat_CardGroup_Used;
			break;
		case 2:
			languageKey = LanguageKey.LK_LifeSkillCombat_CardGroup_Expired;
			break;
		default:
			throw new ArgumentOutOfRangeException();
		}
		if (!true)
		{
		}
		LanguageKey titleKey = languageKey;
		base.CGet<TextMeshProUGUI>("Title").text = LocalStringManager.Get(titleKey);
		InfinityScrollLegacy scrollView = base.CGet<InfinityScrollLegacy>("ScrollView");
		scrollView.OnItemRender = new Action<int, Refers>(this.OnItemRender);
		scrollView.SetDataCount(this._cardList.Count);
	}

	// Token: 0x0600272B RID: 10027 RVA: 0x001212B0 File Offset: 0x0011F4B0
	private void OnItemRender(int index, Refers refers)
	{
		short templateId = this._cardList[index];
		LifeSkillCombatCardView carView = refers.GetComponent<LifeSkillCombatCardView>();
		carView.SetData(templateId, index);
		carView.SetEnabled(true, false);
		carView.SetInteractable(false);
		carView.SetPointerTrigger(false);
	}

	// Token: 0x0600272C RID: 10028 RVA: 0x001212F4 File Offset: 0x0011F4F4
	protected override void OnClick(Transform btn)
	{
		bool flag = btn.name == "ButtonClose";
		if (flag)
		{
			this.QuickHide();
		}
	}

	// Token: 0x0600272D RID: 10029 RVA: 0x0012131D File Offset: 0x0011F51D
	public override void QuickHide()
	{
		AudioManager.Instance.PlaySound("ui_default_cancel", false, false);
		base.QuickHide();
	}

	// Token: 0x04001C95 RID: 7317
	private List<short> _cardList;
}
