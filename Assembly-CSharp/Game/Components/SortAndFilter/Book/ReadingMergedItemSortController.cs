using System;
using System.Collections.Generic;
using Config;
using Game.Components.SortAndFilter.Item;
using GameData.Domains.Item;
using GameData.Domains.Item.Display;
using GameData.Utilities;

namespace Game.Components.SortAndFilter.Book
{
	// Token: 0x02000E84 RID: 3716
	public class ReadingMergedItemSortController : ItemSortController
	{
		// Token: 0x17001377 RID: 4983
		// (get) Token: 0x0600ACDE RID: 44254 RVA: 0x004EF9D8 File Offset: 0x004EDBD8
		// (set) Token: 0x0600ACDF RID: 44255 RVA: 0x004EF9E0 File Offset: 0x004EDBE0
		public IReadOnlyDictionary<ItemKey, int> BonusSpeedDict { get; set; }

		// Token: 0x0600ACE0 RID: 44256 RVA: 0x004EF9E9 File Offset: 0x004EDBE9
		public ReadingMergedItemSortController(Comparison<ITradeableContent> businessComparer)
		{
			this._businessComparer = businessComparer;
		}

		// Token: 0x0600ACE1 RID: 44257 RVA: 0x004EF9FC File Offset: 0x004EDBFC
		public override Comparison<ITradeableContent> GenerateComparer(SortStateData sortData)
		{
			SortItemState? efficiencyState = null;
			SortItemState? inspirationState = null;
			List<SortItemState> remainingStates = new List<SortItemState>();
			bool flag = ((sortData != null) ? sortData.ItemStates : null) != null;
			if (flag)
			{
				foreach (SortItemState state in sortData.ItemStates)
				{
					bool flag2 = state.SortId == 214;
					if (flag2)
					{
						efficiencyState = new SortItemState?(state);
					}
					else
					{
						bool flag3 = state.SortId == 215;
						if (flag3)
						{
							inspirationState = new SortItemState?(state);
						}
						else
						{
							remainingStates.Add(state);
						}
					}
				}
			}
			SortStateData filteredSortData = new SortStateData
			{
				ItemStates = remainingStates
			};
			Comparison<ITradeableContent> itemComparer = base.GenerateComparer(filteredSortData);
			bool hasSortState = remainingStates.Count > 0;
			return delegate(ITradeableContent x, ITradeableContent y)
			{
				bool flag4 = efficiencyState != null;
				if (flag4)
				{
					int result = this.CompareEfficiency(x, y);
					bool flag5 = result != 0;
					if (flag5)
					{
						return (efficiencyState.Value.SortDirection == ESortDirection.Ascending) ? result : (-result);
					}
				}
				bool flag6 = inspirationState != null;
				if (flag6)
				{
					int result2 = ReadingMergedItemSortController.CompareInspiration(x, y);
					bool flag7 = result2 != 0;
					if (flag7)
					{
						return (inspirationState.Value.SortDirection == ESortDirection.Ascending) ? result2 : (-result2);
					}
				}
				bool hasSortState = hasSortState;
				int result3;
				if (hasSortState)
				{
					Comparison<ITradeableContent> itemComparer = itemComparer;
					int itemResult = (itemComparer != null) ? itemComparer(x, y) : 0;
					bool flag8 = itemResult != 0;
					if (flag8)
					{
						result3 = itemResult;
					}
					else
					{
						Comparison<ITradeableContent> businessComparer = this._businessComparer;
						result3 = ((businessComparer != null) ? businessComparer(x, y) : 0);
					}
				}
				else
				{
					Comparison<ITradeableContent> businessComparer2 = this._businessComparer;
					int businessResult = (businessComparer2 != null) ? businessComparer2(x, y) : 0;
					bool flag9 = businessResult != 0;
					if (flag9)
					{
						result3 = businessResult;
					}
					else
					{
						Comparison<ITradeableContent> itemComparer;
						Comparison<ITradeableContent> itemComparer2 = itemComparer;
						result3 = ((itemComparer2 != null) ? itemComparer2(x, y) : 0);
					}
				}
				return result3;
			};
		}

		// Token: 0x0600ACE2 RID: 44258 RVA: 0x004EFB1C File Offset: 0x004EDD1C
		private int CompareEfficiency(ITradeableContent x, ITradeableContent y)
		{
			bool flag = this.BonusSpeedDict == null;
			int result;
			if (flag)
			{
				result = 0;
			}
			else
			{
				int xSpeed = this.BonusSpeedDict.GetOrDefault(x.Key);
				int ySpeed = this.BonusSpeedDict.GetOrDefault(y.Key);
				result = xSpeed.CompareTo(ySpeed);
			}
			return result;
		}

		// Token: 0x0600ACE3 RID: 44259 RVA: 0x004EFB6C File Offset: 0x004EDD6C
		private static int CompareInspiration(ITradeableContent x, ITradeableContent y)
		{
			int xInspiration = ReadingMergedItemSortController.GetInspiration(x.Key);
			int yInspiration = ReadingMergedItemSortController.GetInspiration(y.Key);
			return xInspiration.CompareTo(yInspiration);
		}

		// Token: 0x0600ACE4 RID: 44260 RVA: 0x004EFBA0 File Offset: 0x004EDDA0
		private static int GetInspiration(ItemKey itemKey)
		{
			SkillBookItem configData = SkillBook.Instance[itemKey.TemplateId];
			return (configData.ItemSubType == 1000) ? LifeSkill.Instance[configData.LifeSkillTemplateId].ReadingEventBonusRate : ((int)CombatSkill.Instance[configData.CombatSkillTemplateId].QiArtStrategyGenerateProbability);
		}

		// Token: 0x0600ACE5 RID: 44261 RVA: 0x004EFBFC File Offset: 0x004EDDFC
		public override SortUiConfig GenerateConfig()
		{
			SortUiConfig config = base.GenerateConfig();
			int weightIndex = config.SortIds.IndexOf(6);
			bool flag = weightIndex >= 0;
			if (flag)
			{
				config.SortIds.RemoveAt(weightIndex);
				config.SortNameIndexList.RemoveAt(weightIndex);
			}
			config.DefaultSortIds.Remove(6);
			foreach (List<short> list in config.FilterTypeDic.Values)
			{
				list.Remove(6);
			}
			config.SortIds.Add(214);
			config.SortNameIndexList.Add(0);
			config.SortIds.Add(215);
			config.SortNameIndexList.Add(0);
			return config;
		}

		// Token: 0x040085BF RID: 34239
		private readonly Comparison<ITradeableContent> _businessComparer;
	}
}
