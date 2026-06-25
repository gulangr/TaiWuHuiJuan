using System;
using System.Collections.Generic;
using System.Linq;
using Game.Components.ListStyleGeneralScroll;
using Game.Components.ListStyleGeneralScroll.Item;
using Game.Components.SortAndFilter.Item.Apply;
using GameData.Domains.Character;
using GameData.Domains.Item;
using GameData.Domains.Item.Display;
using GameData.Utilities;
using TMPro;
using UnityEngine;

namespace Game.Views.SettlementTreasury
{
	// Token: 0x02000785 RID: 1925
	public class ReplenishLayer : MonoBehaviour
	{
		// Token: 0x06005D41 RID: 23873 RVA: 0x002AE134 File Offset: 0x002AC334
		public void Set(int layer, Inventory inventory, sbyte[] supplyCounts)
		{
			LanguageKey titleKey = LanguageKey.LK_Building_Treasury_Replenish_Layer0 + layer;
			this.textTitle.text = titleKey.Tr();
			for (sbyte i = 0; i < 9; i += 1)
			{
				bool flag = (int)(i / 3) == layer;
				if (flag)
				{
					this._gradeList.Add(i);
				}
			}
			List<ItemDisplayData> itemList = this.GetItemList(inventory);
			this._typeChanceList.Clear();
			this._bookChanceList.Clear();
			using (List<sbyte>.Enumerator enumerator = this._gradeList.GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					sbyte grade = enumerator.Current;
					Dictionary<sbyte, int> typeChanceDict = (from d in itemList
					where d.Grade == grade
					group d by d.RealKey.ItemType).ToDictionary((IGrouping<sbyte, ItemDisplayData> g) => g.Key, (IGrouping<sbyte, ItemDisplayData> g) => g.Sum((ItemDisplayData d) => d.Amount));
					this._typeChanceList.Add(typeChanceDict);
					Dictionary<short, int> bookChanceDict = (from d in itemList
					where d.Grade == grade && d.RealKey.ItemType == 10
					group d by ItemTemplateHelper.GetItemSubType(d.RealKey.ItemType, d.RealKey.TemplateId)).ToDictionary((IGrouping<short, ItemDisplayData> g) => g.Key, (IGrouping<short, ItemDisplayData> g) => g.Sum((ItemDisplayData d) => d.Amount));
					this._bookChanceList.Add(new IntPair(bookChanceDict.GetValueOrDefault(1001, 0), bookChanceDict.GetValueOrDefault(1000, 0)));
				}
			}
			foreach (ItemDisplayData itemData in itemList)
			{
				int gradeIndex = this._gradeList.IndexOf(itemData.Grade);
				bool flag2 = gradeIndex >= 0;
				if (flag2)
				{
					int chanceDivider = (itemData.RealKey.ItemType == 10) ? ((ItemTemplateHelper.GetItemSubType(itemData.RealKey.ItemType, itemData.RealKey.TemplateId) == 1001) ? this._bookChanceList[gradeIndex].First : this._bookChanceList[gradeIndex].Second) : this._typeChanceList[gradeIndex][itemData.RealKey.ItemType];
					int chance = itemData.Amount * 100 / chanceDivider;
					itemData.SpecialArg = chance;
				}
			}
			this._gradeTitleList = this._gradeList.Select(delegate(sbyte grade)
			{
				string gradeName = CommonUtils.GetPreGradeText(grade);
				string gradeNumber = CommonUtils.GetShortGradeText((int)grade, false);
				sbyte count = supplyCounts[(int)grade];
				return LanguageKey.LK_Building_Treasury_Replenish_GroupTitle.TrFormat(gradeName, gradeNumber, count);
			}).ToList<string>();
			this.itemListScroll.SetCustomBuildGroup(new Action(this.CustomBuildGroup), false);
			this.itemListScroll.SetItemList(itemList);
		}

		// Token: 0x06005D42 RID: 23874 RVA: 0x002AE49C File Offset: 0x002AC69C
		private void Awake()
		{
			IEnumerable<ColumnDefinition> columnDefinitions = this.GenerateColumnDefinitions(this.itemListScroll);
			this.itemListScroll.Init("SettlementTreasuryReplenishLayer", ESortAndFilterControllerType.Item, true, new Action<ITradeableContent, RowItemLine>(this.OnRenderItem), null, ItemListScroll.EColumnType.IconAndName | ItemListScroll.EColumnType.Amount | ItemListScroll.EColumnType.Type | ItemListScroll.EColumnType.Weight | ItemListScroll.EColumnType.Value | ItemListScroll.EColumnType.Durability, null, columnDefinitions, new Action<RowItem>(this.PrepareCustomRowTemplateContainers));
		}

		// Token: 0x06005D43 RID: 23875 RVA: 0x002AE4E8 File Offset: 0x002AC6E8
		private void CustomBuildGroup()
		{
			List<ITradeableContent> sourceSet = new List<ITradeableContent>(this.itemListScroll.FilteredData);
			sourceSet.Sort((ITradeableContent a, ITradeableContent b) => b.SpecialArg.CompareTo(a.SpecialArg));
			for (int index = 0; index < this._gradeList.Count; index++)
			{
				sbyte grade = this._gradeList[index];
				string title = this._gradeTitleList[index];
				List<ITradeableContent> itemsToRemove = (from d in sourceSet
				where d.Grade == grade
				select d).ToList<ITradeableContent>();
				this.itemListScroll.AddGroup(index, title, itemsToRemove, sourceSet, true);
			}
		}

		// Token: 0x06005D44 RID: 23876 RVA: 0x002AE59B File Offset: 0x002AC79B
		private IEnumerable<ColumnDefinition> GenerateColumnDefinitions(ItemListScroll scroll)
		{
			LayoutOption option = default(LayoutOption);
			yield return scroll.ColumnIconAndName(option, false);
			yield return scroll.ColumnValue(option, false, LanguageKey.LK_ItemValue);
			option.PreferredWidth = 200f;
			yield return scroll.ColumnReplenishRate(option, false);
			yield break;
		}

		// Token: 0x06005D45 RID: 23877 RVA: 0x002AE5B2 File Offset: 0x002AC7B2
		public void PrepareCustomRowTemplateContainers(RowItem rowTemplate)
		{
			this.itemListScroll.PrepareRowTemplateContainers(ItemListScroll.EColumnType.IconAndName);
			this.itemListScroll.PrepareRowTemplateContainers(ItemListScroll.EColumnType.Value);
			this.itemListScroll.PrepareRowTemplateContainers(ItemListScroll.EColumnType.SupplyRate);
		}

		// Token: 0x06005D46 RID: 23878 RVA: 0x002AE5E4 File Offset: 0x002AC7E4
		private List<ItemDisplayData> GetItemList(Inventory inventory)
		{
			List<ItemDisplayData> itemList = new List<ItemDisplayData>();
			foreach (KeyValuePair<ItemKey, int> keyValuePair in inventory.Items)
			{
				ItemKey itemKey;
				int num;
				keyValuePair.Deconstruct(out itemKey, out num);
				ItemKey key = itemKey;
				int amount = num;
				sbyte grade = ItemTemplateHelper.GetGrade(key.ItemType, key.TemplateId);
				bool flag = !this._gradeList.Contains(grade);
				if (!flag)
				{
					ItemDisplayData itemData = new ItemDisplayData(key.ItemType, key.TemplateId)
					{
						Amount = amount
					};
					itemList.Add(itemData);
				}
			}
			return itemList;
		}

		// Token: 0x06005D47 RID: 23879 RVA: 0x002AE6A4 File Offset: 0x002AC8A4
		private void OnRenderItem(ITradeableContent itemData, RowItemLine rowItemLine)
		{
			RowItemMain rowItemMain = rowItemLine.GetComponentInChildren<RowItemMain>();
			rowItemMain.SetData(itemData);
			rowItemLine.Set(rowItemMain, true);
			rowItemLine.SetInteractable(false, true);
			rowItemLine.SetSelected(false);
		}

		// Token: 0x04004007 RID: 16391
		[SerializeField]
		private TextMeshProUGUI textTitle;

		// Token: 0x04004008 RID: 16392
		[SerializeField]
		private ItemListScroll itemListScroll;

		// Token: 0x04004009 RID: 16393
		private readonly List<Dictionary<sbyte, int>> _typeChanceList = new List<Dictionary<sbyte, int>>();

		// Token: 0x0400400A RID: 16394
		private readonly List<IntPair> _bookChanceList = new List<IntPair>();

		// Token: 0x0400400B RID: 16395
		private readonly List<sbyte> _gradeList = new List<sbyte>();

		// Token: 0x0400400C RID: 16396
		private List<string> _gradeTitleList = new List<string>();
	}
}
