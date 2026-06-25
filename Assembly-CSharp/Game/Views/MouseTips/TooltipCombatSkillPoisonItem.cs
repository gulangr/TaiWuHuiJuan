using System;
using TMPro;
using UnityEngine;

namespace Game.Views.MouseTips
{
	// Token: 0x0200088C RID: 2188
	public class TooltipCombatSkillPoisonItem : MonoBehaviour
	{
		// Token: 0x060068FB RID: 26875 RVA: 0x00303754 File Offset: 0x00301954
		public void Set(string typeName, string value, string poisonTypeIconSprite, string poisonLevelIconSprite, int levelCount)
		{
			this.typeText.text = typeName;
			this.valueText.text = value;
			this.poisonTypeIcon.SetSprite(poisonTypeIconSprite, false, null);
			levelCount = Mathf.Clamp(levelCount, 0, this.poisonLevelIcons.Length);
			for (int i = 0; i < this.poisonLevelIcons.Length; i++)
			{
				bool shouldShow = i < levelCount;
				this.poisonLevelIcons[i].gameObject.SetActive(shouldShow);
				bool flag = shouldShow;
				if (flag)
				{
					this.poisonLevelIcons[i].SetSprite(poisonLevelIconSprite, false, null);
				}
			}
		}

		// Token: 0x04004B1E RID: 19230
		[SerializeField]
		private TextMeshProUGUI typeText;

		// Token: 0x04004B1F RID: 19231
		[SerializeField]
		private TextMeshProUGUI valueText;

		// Token: 0x04004B20 RID: 19232
		[SerializeField]
		private CImage poisonTypeIcon;

		// Token: 0x04004B21 RID: 19233
		[SerializeField]
		private CImage[] poisonLevelIcons;
	}
}
