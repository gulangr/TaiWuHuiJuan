using System;
using GameData.Domains.CombatSkill;
using GameData.Domains.Item.Display;
using GameData.Utilities;
using TMPro;
using UnityEngine;

namespace Game.Views.MouseTips.Item
{
	// Token: 0x0200089A RID: 2202
	public class TooltipBookCombatPageDetail : MonoBehaviour
	{
		// Token: 0x06006963 RID: 26979 RVA: 0x003070D0 File Offset: 0x003052D0
		public void Refresh(int index, SkillBookPageDisplayData displayData)
		{
			this.textName.text = TooltipBookPage.GetPageName(true, index);
			bool isFirst = index == 0;
			int count = isFirst ? 5 : 2;
			TooltipBookPage[] pages = this.layoutPage.GetComponentsInChildren<TooltipBookPage>();
			sbyte i = 0;
			while ((int)i < count)
			{
				sbyte type = isFirst ? i : ((i == 0) ? 0 : 1);
				byte internalIndex = CombatSkillStateHelper.GetPageInternalIndex(type, type, (byte)index);
				sbyte? b;
				if (displayData == null)
				{
					b = null;
				}
				else
				{
					sbyte[] combatSkillAllReadingProgress = displayData.CombatSkillAllReadingProgress;
					b = ((combatSkillAllReadingProgress != null) ? new sbyte?(combatSkillAllReadingProgress.GetOrDefault((int)internalIndex)) : null);
				}
				sbyte? b2 = b;
				sbyte progress = b2.GetValueOrDefault();
				pages[(int)i].Refresh(true, index, type, progress, 0);
				pages[(int)i].gameObject.SetActive(true);
				i += 1;
			}
			for (int j = count; j < pages.Length; j++)
			{
				pages[j].gameObject.SetActive(false);
			}
		}

		// Token: 0x04004BAA RID: 19370
		[SerializeField]
		private TextMeshProUGUI textName;

		// Token: 0x04004BAB RID: 19371
		[SerializeField]
		private Transform layoutPage;
	}
}
