using System;
using GameData.Domains.Character.Display;
using TMPro;
using UnityEngine;

namespace Game.Components.Character
{
	// Token: 0x02000F38 RID: 3896
	public class Name : MonoBehaviour
	{
		// Token: 0x0600B329 RID: 45865 RVA: 0x00519134 File Offset: 0x00517334
		public void Set(CharacterDisplayData displayData, bool isTaiwu = false)
		{
			NameRelatedData nameRelatedData = NameCenter.GetNameRelatedData(displayData);
			this.Set(nameRelatedData, isTaiwu);
		}

		// Token: 0x0600B32A RID: 45866 RVA: 0x00519154 File Offset: 0x00517354
		public void Set(NameRelatedData nameRelatedData, bool isTaiwu = false)
		{
			string nameStr = NameCenter.GetMonasticTitleOrDisplayName(ref nameRelatedData, isTaiwu, false);
			this.label.text = nameStr;
			this.RefreshTips();
		}

		// Token: 0x0600B32B RID: 45867 RVA: 0x00519180 File Offset: 0x00517380
		private void RefreshTips()
		{
			bool flag = this.mouseTip == null;
			if (!flag)
			{
				this.mouseTip.enabled = true;
				this.mouseTip.Type = TipType.Simple;
				this.mouseTip.IsLanguageKey = false;
				this.mouseTip.PresetParam = new string[]
				{
					LocalStringManager.Get(LanguageKey.LK_Char_Name),
					LocalStringManager.Get(LanguageKey.LK_Main_SummaryInfo_Char_Name_TipContent)
				};
			}
		}

		// Token: 0x04008B2F RID: 35631
		[SerializeField]
		private TextMeshProUGUI label;

		// Token: 0x04008B30 RID: 35632
		[SerializeField]
		private TooltipInvoker mouseTip;
	}
}
