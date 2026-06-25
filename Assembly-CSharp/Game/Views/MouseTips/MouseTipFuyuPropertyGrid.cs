using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using GameData.Domains.Character;
using UnityEngine;

namespace Game.Views.MouseTips
{
	// Token: 0x02000862 RID: 2146
	public abstract class MouseTipFuyuPropertyGrid : MonoBehaviour
	{
		// Token: 0x060067B8 RID: 26552 RVA: 0x002F60F4 File Offset: 0x002F42F4
		protected void PrepareItems(int count)
		{
			CommonUtils.PrepareEnoughChildren(this.gridContainer, this.itemTemplate, count, null);
		}

		// Token: 0x060067B9 RID: 26553 RVA: 0x002F6120 File Offset: 0x002F4320
		protected MouseTipFuyuPropertyItem GetItem(int index)
		{
			return this.gridContainer.GetChild(index).GetComponent<MouseTipFuyuPropertyItem>();
		}

		// Token: 0x060067BA RID: 26554 RVA: 0x002F6143 File Offset: 0x002F4343
		protected IEnumerable<MouseTipFuyuPropertyItem> GetItemIterator()
		{
			bool flag = this.gridContainer == null;
			if (flag)
			{
				yield break;
			}
			foreach (object obj in this.gridContainer)
			{
				Transform child = (Transform)obj;
				yield return child.GetComponent<MouseTipFuyuPropertyItem>();
				child = null;
			}
			IEnumerator enumerator = null;
			yield break;
			yield break;
		}

		// Token: 0x060067BB RID: 26555 RVA: 0x002F6154 File Offset: 0x002F4354
		[return: TupleElementNames(new string[]
		{
			"value",
			"percent"
		})]
		protected static ValueTuple<int, int> GetBonusValue(List<CharacterPropertyBonus> bonuses, ECharacterPropertyReferencedType propertyType, bool checkProfessionSkill = true)
		{
			bool flag = bonuses == null || propertyType >= (ECharacterPropertyReferencedType)bonuses.Count;
			ValueTuple<int, int> result;
			if (flag)
			{
				result = new ValueTuple<int, int>(0, 0);
			}
			else
			{
				CharacterPropertyBonus bonus = bonuses[(int)propertyType];
				int addValue = bonus.AddValue;
				int addPercent = bonus.AddPercent;
				bool flag2 = checkProfessionSkill && SingletonObject.getInstance<ProfessionModel>().IsSkillEquipped(43) && addValue >= 0 && addPercent >= 0;
				if (flag2)
				{
					addValue = addValue * 150 / 100;
					addPercent = addPercent * 150 / 100;
				}
				result = new ValueTuple<int, int>(addValue, addPercent);
			}
			return result;
		}

		// Token: 0x060067BC RID: 26556 RVA: 0x002F61E4 File Offset: 0x002F43E4
		protected static bool HasBonus(List<CharacterPropertyBonus> bonuses, ECharacterPropertyReferencedType propertyType)
		{
			ValueTuple<int, int> bonusValue = MouseTipFuyuPropertyGrid.GetBonusValue(bonuses, propertyType, false);
			int value = bonusValue.Item1;
			int percent = bonusValue.Item2;
			return value != 0 || percent != 0;
		}

		// Token: 0x04004947 RID: 18759
		[SerializeField]
		protected GameObject itemTemplate;

		// Token: 0x04004948 RID: 18760
		[SerializeField]
		protected Transform gridContainer;
	}
}
