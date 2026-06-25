using System;
using TMPro;
using UnityEngine;

namespace Game.Components.Combat
{
	// Token: 0x02000F05 RID: 3845
	public class CombatDamageDetailPropertyItem : MonoBehaviour
	{
		// Token: 0x0600B151 RID: 45393 RVA: 0x0050CC14 File Offset: 0x0050AE14
		public void Set(string iconName, string title, string value, bool showWinMark = false)
		{
			this.icon.SetSprite(iconName, false, null);
			this.titleLabel.text = title;
			this.valueLabel.text = value;
			this.winMark.enabled = showWinMark;
		}

		// Token: 0x0600B152 RID: 45394 RVA: 0x0050CC4E File Offset: 0x0050AE4E
		public void Set(Sprite iconSprite, string title, string value, bool showWinMark = false)
		{
			this.icon.sprite = iconSprite;
			this.titleLabel.text = title;
			this.valueLabel.text = value;
			this.winMark.enabled = showWinMark;
		}

		// Token: 0x0600B153 RID: 45395 RVA: 0x0050CC86 File Offset: 0x0050AE86
		public void SetValueColor(Color color)
		{
			this.valueLabel.color = color;
		}

		// Token: 0x0400896B RID: 35179
		[SerializeField]
		private CImage icon;

		// Token: 0x0400896C RID: 35180
		[SerializeField]
		private TextMeshProUGUI titleLabel;

		// Token: 0x0400896D RID: 35181
		[SerializeField]
		private TextMeshProUGUI valueLabel;

		// Token: 0x0400896E RID: 35182
		[SerializeField]
		private CImage winMark;
	}
}
