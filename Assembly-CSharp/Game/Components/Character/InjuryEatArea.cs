using System;
using GameData.Domains.Character;
using GameData.Domains.Character.Display;
using GameData.Domains.Item;
using GameData.Domains.Item.Display;
using UnityEngine;

namespace Game.Components.Character
{
	// Token: 0x02000F2E RID: 3886
	public class InjuryEatArea : MonoBehaviour
	{
		// Token: 0x0600B2DA RID: 45786 RVA: 0x00516C90 File Offset: 0x00514E90
		public void Set(CharacterInjuryDisplayData displayData)
		{
			this._characterInjuryDisplayData = displayData;
			for (int i = 0; i < this.injuryEatItems.Length; i++)
			{
				ItemDisplayData itemData = displayData.EatingItemDisplayDataArray[i];
				bool isUnlocked = i < (int)displayData.CanEatingMaxCount || itemData != null;
				short duration = displayData.EatingItems.GetDuration(i);
				this.injuryEatItems[i].Set(displayData.CharacterId, isUnlocked, itemData, duration, true);
			}
		}

		// Token: 0x0600B2DB RID: 45787 RVA: 0x00516D00 File Offset: 0x00514F00
		public void ShowEatNotice(ITradeableContent itemDisplayData, int amount = 1)
		{
			bool isWug = EatingItems.IsWug(itemDisplayData.Key);
			bool isWugKing = isWug && EatingItems.IsWugKing(itemDisplayData.Key);
			int begin = isWug ? 8 : 0;
			int end = isWug ? 0 : 8;
			int step = isWug ? -1 : 1;
			int preferIndex = -1;
			int normalIndex = -1;
			for (int i = begin; i != end; i += step)
			{
				ItemKey existItemKey = this._characterInjuryDisplayData.EatingItems.GetItem(i);
				bool flag = !EatingItems.IsValid(existItemKey) && normalIndex < 0;
				if (flag)
				{
					normalIndex = i;
				}
				bool flag2 = EatingItems.IsWugKing(existItemKey) && isWugKing && preferIndex < 0;
				if (flag2)
				{
					preferIndex = i;
				}
			}
			int finalIndex = (preferIndex >= 0) ? preferIndex : normalIndex;
			for (int j = 0; j < (int)this._characterInjuryDisplayData.CanEatingMaxCount; j++)
			{
				ItemKey existItemKey2 = this._characterInjuryDisplayData.EatingItems.GetItem(j);
				bool hasItem = EatingItems.IsValid(existItemKey2) || EatingItems.IsWugKing(existItemKey2);
				bool show = j >= finalIndex && amount > 0 && !hasItem;
				bool flag3 = show;
				if (flag3)
				{
					amount -= ItemTemplateHelper.GetItemCountUnit(itemDisplayData.Key.ItemType, itemDisplayData.Key.TemplateId);
				}
				this.injuryEatItems[j].Highlight.SetActive(show);
			}
		}

		// Token: 0x0600B2DC RID: 45788 RVA: 0x00516E64 File Offset: 0x00515064
		public void SetAreaInteract(Action onClickEatArea)
		{
			foreach (InjuryEatItem t in this.injuryEatItems)
			{
				t.SetAreaInteract(onClickEatArea);
			}
		}

		// Token: 0x04008ABE RID: 35518
		[SerializeField]
		private InjuryEatItem[] injuryEatItems;

		// Token: 0x04008ABF RID: 35519
		private CharacterInjuryDisplayData _characterInjuryDisplayData;
	}
}
