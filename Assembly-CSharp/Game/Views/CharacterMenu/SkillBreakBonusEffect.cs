using System;
using TMPro;
using UnityEngine;

namespace Game.Views.CharacterMenu
{
	// Token: 0x02000B7E RID: 2942
	public class SkillBreakBonusEffect : MonoBehaviour
	{
		// Token: 0x0600916C RID: 37228 RVA: 0x0043C980 File Offset: 0x0043AB80
		public void Refresh(SkillBreakBonusEffectDisplay display, SkillBreakBonusEffect.EBonusIconSize iconSize)
		{
			CImage cimage = this.icon;
			if (!true)
			{
			}
			string spriteName;
			if (iconSize != SkillBreakBonusEffect.EBonusIconSize.Small)
			{
				if (iconSize != SkillBreakBonusEffect.EBonusIconSize.Big)
				{
					throw new ArgumentOutOfRangeException("iconSize", iconSize, null);
				}
				spriteName = display.BigIcon;
			}
			else
			{
				spriteName = display.SmallIcon;
			}
			if (!true)
			{
			}
			cimage.SetSprite(spriteName, false, null);
			this.nameLabel.text = display.Name;
			this.valueLabel.text = display.Value;
		}

		// Token: 0x0600916D RID: 37229 RVA: 0x0043C9FA File Offset: 0x0043ABFA
		public void SetShowBack(bool showBack)
		{
			this.imageBack.enabled = showBack;
		}

		// Token: 0x04007018 RID: 28696
		public CImage imageBack;

		// Token: 0x04007019 RID: 28697
		[SerializeField]
		private CImage icon;

		// Token: 0x0400701A RID: 28698
		[SerializeField]
		private TextMeshProUGUI nameLabel;

		// Token: 0x0400701B RID: 28699
		[SerializeField]
		private TextMeshProUGUI valueLabel;

		// Token: 0x0200218E RID: 8590
		public enum EBonusIconSize
		{
			// Token: 0x0400D635 RID: 54837
			Small,
			// Token: 0x0400D636 RID: 54838
			Big
		}
	}
}
