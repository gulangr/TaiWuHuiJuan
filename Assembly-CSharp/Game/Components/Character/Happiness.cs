using System;
using Game.Components.Common;
using GameData.Domains.Character;
using GameData.Domains.Character.Display;
using UnityEngine;

namespace Game.Components.Character
{
	// Token: 0x02000F28 RID: 3880
	public class Happiness : MonoBehaviour
	{
		// Token: 0x0600B289 RID: 45705 RVA: 0x00513F4C File Offset: 0x0051214C
		public void Set(CharacterDisplayData data, bool isShowBack = true)
		{
			bool flag = data == null;
			if (flag)
			{
				this.SetEmpty();
			}
			else
			{
				this.Set(data.Happiness);
				this.propertyItem.SetShowBack(isShowBack);
			}
		}

		// Token: 0x0600B28A RID: 45706 RVA: 0x00513F88 File Offset: 0x00512188
		public void Set(sbyte happiness)
		{
			bool flag = this.propertyItem != null;
			if (flag)
			{
				this.SetType(HappinessType.GetHappinessType(happiness));
			}
		}

		// Token: 0x0600B28B RID: 45707 RVA: 0x00513FB4 File Offset: 0x005121B4
		public void SetType(sbyte happinessType)
		{
			bool flag = this.propertyItem == null;
			if (!flag)
			{
				int iconIndex = CommonUtils.GetHappinessIconIndex(happinessType);
				string text = CommonUtils.GetHappinessString(happinessType);
				this.propertyItem.Set(this.sprites[iconIndex], LanguageKey.LK_Main_SummaryInfo_Happiness.Tr(), text, null, false);
			}
		}

		// Token: 0x0600B28C RID: 45708 RVA: 0x0051400C File Offset: 0x0051220C
		public void SetEmpty()
		{
			bool flag = this.propertyItem != null;
			if (flag)
			{
				this.propertyItem.Set(string.Empty, string.Empty, string.Empty, null, false);
			}
		}

		// Token: 0x04008A8C RID: 35468
		[Header("心情组件")]
		[SerializeField]
		private PropertyItem propertyItem;

		// Token: 0x04008A8D RID: 35469
		[Header("心情图标")]
		[SerializeField]
		private Sprite[] sprites;
	}
}
