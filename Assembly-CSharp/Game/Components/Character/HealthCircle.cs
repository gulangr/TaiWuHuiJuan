using System;
using GameData.Domains.Character;
using GameData.Domains.Character.Display;
using TMPro;
using UnityEngine;

namespace Game.Components.Character
{
	// Token: 0x02000F2A RID: 3882
	public class HealthCircle : MonoBehaviour
	{
		// Token: 0x0600B293 RID: 45715 RVA: 0x00514198 File Offset: 0x00512398
		public void Set(CharacterDisplayData displayData)
		{
			this.Set(displayData.Health, displayData.LeftMaxHealth);
		}

		// Token: 0x0600B294 RID: 45716 RVA: 0x005141B0 File Offset: 0x005123B0
		public void Set(short health, short leftMaxHealth)
		{
			ValueTuple<string, float, int> info = CommonUtils.GetCharacterHealthInfo(health, leftMaxHealth, -1);
			EHealthType healthType = (EHealthType)info.Item3;
			bool flag = this.progress != null;
			if (flag)
			{
				this.progress.fillAmount = info.Item2;
			}
			bool flag2 = this.stateLabel != null;
			if (flag2)
			{
				this.stateLabel.text = info.Item1;
			}
			this.UpdateHealthStateIcons(healthType);
		}

		// Token: 0x0600B295 RID: 45717 RVA: 0x00514220 File Offset: 0x00512420
		private void UpdateHealthStateIcons(EHealthType healthType)
		{
			bool flag = this.healthStateIcon == null;
			if (!flag)
			{
				if (healthType != EHealthType.Dying)
				{
					if (healthType - EHealthType.Sick > 2)
					{
						this.healthStateIcon.sprite = this.healthWorseSprite;
					}
					else
					{
						this.healthStateIcon.sprite = this.healthBetterSprite;
					}
				}
				else
				{
					this.healthStateIcon.sprite = this.healthDyingSprite;
				}
			}
		}

		// Token: 0x04008A91 RID: 35473
		[SerializeField]
		private CImage progress;

		// Token: 0x04008A92 RID: 35474
		[SerializeField]
		private TextMeshProUGUI stateLabel;

		// Token: 0x04008A93 RID: 35475
		[SerializeField]
		private CImage healthStateIcon;

		// Token: 0x04008A94 RID: 35476
		[SerializeField]
		private Sprite healthBetterSprite;

		// Token: 0x04008A95 RID: 35477
		[SerializeField]
		private Sprite healthWorseSprite;

		// Token: 0x04008A96 RID: 35478
		[SerializeField]
		private Sprite healthDyingSprite;
	}
}
