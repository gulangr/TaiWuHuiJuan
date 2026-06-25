using System;
using System.Collections.Generic;
using FrameWork;
using FrameWork.UISystem.Components;
using TMPro;
using UnityEngine;

namespace Game.Views.Debate
{
	// Token: 0x02000AA8 RID: 2728
	public class ViewDebateCardGroup : UIBase
	{
		// Token: 0x06008620 RID: 34336 RVA: 0x003E6D60 File Offset: 0x003E4F60
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
			this.textTitle.text = LocalStringManager.Get(titleKey);
			this.infinityScroll.OnItemRender -= this.OnItemRender;
			this.infinityScroll.OnItemRender += this.OnItemRender;
			this.infinityScroll.SetDataCount(this._cardList.Count);
		}

		// Token: 0x06008621 RID: 34337 RVA: 0x003E6E24 File Offset: 0x003E5024
		private void OnItemRender(int index, GameObject obj)
		{
			short templateId = this._cardList[index];
			DebateCardView carView = obj.GetComponentInChildren<DebateCardView>();
			carView.SetData(templateId, index);
			carView.SetEnabled(true, true);
			carView.SetInteractable(false);
			carView.SetPointerTrigger(false);
		}

		// Token: 0x06008622 RID: 34338 RVA: 0x003E6E68 File Offset: 0x003E5068
		protected override void OnClick(Transform btn)
		{
			bool flag = btn.name == "ButtonClose";
			if (flag)
			{
				this.QuickHide();
			}
		}

		// Token: 0x040066F1 RID: 26353
		[SerializeField]
		private TextMeshProUGUI textTitle;

		// Token: 0x040066F2 RID: 26354
		[SerializeField]
		private InfinityScroll infinityScroll;

		// Token: 0x040066F3 RID: 26355
		private List<short> _cardList;
	}
}
