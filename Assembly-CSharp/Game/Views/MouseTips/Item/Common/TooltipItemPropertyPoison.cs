using System;
using UnityEngine;

namespace Game.Views.MouseTips.Item.Common
{
	// Token: 0x020008B2 RID: 2226
	public class TooltipItemPropertyPoison : TooltipItemProperty
	{
		// Token: 0x06006A74 RID: 27252 RVA: 0x00311F14 File Offset: 0x00310114
		public void Set(string title, string value, bool isCondensed, string poisonTypeIconSprite, string poisonLevelIconSprite, int levelCount)
		{
			bool flag = this.titleText;
			if (flag)
			{
				this.titleText.text = title;
			}
			bool flag2 = this.valueText;
			if (flag2)
			{
				this.valueText.text = value;
			}
			bool flag3 = this.icon;
			if (flag3)
			{
				this.icon.SetSprite(poisonTypeIconSprite, false, null);
			}
			levelCount = Mathf.Clamp(levelCount, 0, this.poisonLevelIcons.Length);
			for (int i = 0; i < this.poisonLevelIcons.Length; i++)
			{
				bool shouldShow = i < levelCount;
				this.poisonLevelIcons[i].gameObject.SetActive(shouldShow);
				bool flag4 = shouldShow;
				if (flag4)
				{
					this.poisonLevelIcons[i].SetSprite(poisonLevelIconSprite, false, null);
				}
			}
		}

		// Token: 0x04004CE8 RID: 19688
		[SerializeField]
		private CImage[] poisonLevelIcons;
	}
}
