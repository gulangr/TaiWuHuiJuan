using System;
using System.Collections.Generic;
using Config;
using GameData.Domains.Character.Display;
using TMPro;
using UnityEngine;

namespace Game.Components.Character
{
	// Token: 0x02000F45 RID: 3909
	public class Title : MonoBehaviour
	{
		// Token: 0x0600B374 RID: 45940 RVA: 0x0051AC87 File Offset: 0x00518E87
		public void Set(CharacterDisplayData displayData, bool isHideWhenNone = false)
		{
			this.Set(displayData.TitleIds, isHideWhenNone);
		}

		// Token: 0x0600B375 RID: 45941 RVA: 0x0051AC98 File Offset: 0x00518E98
		public void Set(List<short> titleIds, bool isHideWhenNone = false)
		{
			bool flag = titleIds == null || titleIds.Count <= 0;
			if (flag)
			{
				this.label.text = LocalStringManager.Get(LanguageKey.LK_None);
			}
			else
			{
				CharacterTitleItem config = CharacterTitle.Instance[titleIds[0]];
				this.label.text = config.Name;
			}
			if (isHideWhenNone)
			{
				this.RefreshHideWhenNone(titleIds);
			}
			this.RefreshTips(titleIds);
		}

		// Token: 0x0600B376 RID: 45942 RVA: 0x0051AD11 File Offset: 0x00518F11
		private void RefreshHideWhenNone(List<short> titleIds)
		{
			this.label.gameObject.SetActive(titleIds != null && titleIds.Count > 0);
		}

		// Token: 0x0600B377 RID: 45943 RVA: 0x0051AD34 File Offset: 0x00518F34
		private void RefreshTips(List<short> titleIds)
		{
			bool flag = this.mouseTip == null;
			if (!flag)
			{
				this.mouseTip.enabled = true;
				this.mouseTip.IsLanguageKey = false;
				this.mouseTip.Type = TipType.Simple;
				string tipTitle = LocalStringManager.Get(LanguageKey.LK_Main_SummaryInfo_Title);
				string tipContent = LocalStringManager.Get(LanguageKey.LK_Main_SummaryInfo_Title_TipContent);
				bool flag2 = titleIds != null && titleIds.Count > 0;
				if (flag2)
				{
					foreach (short titleId in titleIds)
					{
						tipContent = tipContent + "·" + CharacterTitle.Instance[titleId].Name;
					}
				}
				this.mouseTip.PresetParam = new string[]
				{
					tipTitle,
					tipContent
				};
			}
		}

		// Token: 0x04008B62 RID: 35682
		[SerializeField]
		private TextMeshProUGUI label;

		// Token: 0x04008B63 RID: 35683
		[SerializeField]
		private TooltipInvoker mouseTip;
	}
}
