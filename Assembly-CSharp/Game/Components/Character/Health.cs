using System;
using Game.Components.Common;
using GameData.Domains.Character;
using GameData.Domains.Character.Display;
using UnityEngine;

namespace Game.Components.Character
{
	// Token: 0x02000F29 RID: 3881
	public class Health : MonoBehaviour
	{
		// Token: 0x0600B28E RID: 45710 RVA: 0x00514058 File Offset: 0x00512258
		public void Set(CharacterDisplayData data)
		{
			bool flag = data == null;
			if (flag)
			{
				this.SetEmpty();
			}
			else
			{
				this._currentData = data;
				this.Set(data.Health, data.LeftMaxHealth, data.CharacterId);
			}
		}

		// Token: 0x0600B28F RID: 45711 RVA: 0x00514098 File Offset: 0x00512298
		public void Set(short health, short leftMaxHealth, int characterId = -1)
		{
			bool flag = this.propertyItem != null;
			if (flag)
			{
				EHealthType healthType = CommonUtils.GetHealthType(health, leftMaxHealth, characterId);
				int iconIndex = Health.GetHealthIconIndex(healthType);
				string text = CommonUtils.GetHealthString(healthType);
				this.propertyItem.Set(this.sprites[iconIndex], LanguageKey.LK_Health.Tr(), text, null, false);
			}
		}

		// Token: 0x0600B290 RID: 45712 RVA: 0x005140FC File Offset: 0x005122FC
		public void SetEmpty()
		{
			bool flag = this.propertyItem != null;
			if (flag)
			{
				this.propertyItem.Set(string.Empty, string.Empty, string.Empty, null, false);
			}
		}

		// Token: 0x0600B291 RID: 45713 RVA: 0x00514140 File Offset: 0x00512340
		private static int GetHealthIconIndex(EHealthType healthType)
		{
			if (!true)
			{
			}
			int result;
			switch (healthType)
			{
			case EHealthType.Dying:
				result = 4;
				break;
			case EHealthType.CriticallyIll:
				result = 3;
				break;
			case EHealthType.Weak:
				result = 2;
				break;
			case EHealthType.Sick:
				result = 1;
				break;
			case EHealthType.Healthy:
				result = 0;
				break;
			default:
				result = 0;
				break;
			}
			if (!true)
			{
			}
			return result;
		}

		// Token: 0x04008A8E RID: 35470
		[SerializeField]
		private PropertyItem propertyItem;

		// Token: 0x04008A8F RID: 35471
		[Header("健康图标")]
		[SerializeField]
		private Sprite[] sprites;

		// Token: 0x04008A90 RID: 35472
		private CharacterDisplayData _currentData;
	}
}
