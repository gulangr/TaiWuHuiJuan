using System;
using Game.Components.Common;
using GameData.Domains.Character.Display;
using UnityEngine;

namespace Game.Components.Character
{
	// Token: 0x02000F22 RID: 3874
	public class Favorability : MonoBehaviour
	{
		// Token: 0x0600B261 RID: 45665 RVA: 0x00512E7C File Offset: 0x0051107C
		public void Set(CharacterMenuInfoDisplayData menuData, bool isShowBack = true)
		{
			bool flag = ((menuData != null) ? menuData.CharacterDisplayData : null) == null;
			if (flag)
			{
				this.SetEmpty();
			}
			else
			{
				CharacterDisplayData data = menuData.CharacterDisplayData;
				bool isTaiwu = menuData.TaiwuDisplayData == null;
				bool isInteracted = menuData.IsInteractedCharacter;
				this.Set(data.FavorabilityToTaiwu, isTaiwu, isInteracted);
				this.propertyItem.SetShowBack(isShowBack);
			}
		}

		// Token: 0x0600B262 RID: 45666 RVA: 0x00512EDC File Offset: 0x005110DC
		public void Set(short favorability, bool isTaiwu, bool isInteracted)
		{
			bool flag = this.propertyItem == null;
			if (!flag)
			{
				string title = LanguageKey.LK_Favorability.Tr();
				if (isTaiwu)
				{
					this.propertyItem.Set(this.sprites[2], title, "-", null, false);
				}
				else
				{
					bool flag2 = !isInteracted;
					if (flag2)
					{
						favorability = short.MinValue;
					}
					int iconIndex = CommonUtils.GetFavorabilityIconIndex(favorability, isInteracted);
					string text = CommonUtils.GetFavorString(favorability);
					this.propertyItem.Set(this.sprites[iconIndex], title, text, null, false);
				}
			}
		}

		// Token: 0x0600B263 RID: 45667 RVA: 0x00512F78 File Offset: 0x00511178
		public void SetEmpty()
		{
			bool flag = this.propertyItem != null;
			if (flag)
			{
				this.propertyItem.Set(string.Empty, string.Empty, string.Empty, null, false);
			}
		}

		// Token: 0x04008A64 RID: 35428
		[Header("好感组件")]
		[SerializeField]
		private PropertyItem propertyItem;

		// Token: 0x04008A65 RID: 35429
		[Header("好感图标")]
		[SerializeField]
		private Sprite[] sprites;
	}
}
