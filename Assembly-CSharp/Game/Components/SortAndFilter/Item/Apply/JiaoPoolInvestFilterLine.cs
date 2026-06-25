using System;
using System.Collections.Generic;
using GameData.DLC.FiveLoong;
using GameData.Domains.Item;
using GameData.Domains.Item.Display;

namespace Game.Components.SortAndFilter.Item.Apply
{
	// Token: 0x02000DF3 RID: 3571
	internal sealed class JiaoPoolInvestFilterLine : FilterToggleGroupLineLogic<ITradeableContent>
	{
		// Token: 0x0600AA9E RID: 43678 RVA: 0x004E8E10 File Offset: 0x004E7010
		public JiaoPoolInvestFilterLine(IReadOnlyDictionary<ItemKey, Jiao> jiaoByKey)
		{
			this._jiaoByKey = jiaoByKey;
		}

		// Token: 0x170012C0 RID: 4800
		// (get) Token: 0x0600AA9F RID: 43679 RVA: 0x004E8E20 File Offset: 0x004E7020
		public override int Id
		{
			get
			{
				return 46;
			}
		}

		// Token: 0x170012C1 RID: 4801
		// (get) Token: 0x0600AAA0 RID: 43680 RVA: 0x004E8E24 File Offset: 0x004E7024
		protected override int Level
		{
			get
			{
				return 0;
			}
		}

		// Token: 0x0600AAA1 RID: 43681 RVA: 0x004E8E27 File Offset: 0x004E7027
		protected override List<FilterToggleConfig> GetFilterToggleConfigs()
		{
			return new List<FilterToggleConfig>
			{
				new FilterToggleConfig("ui9_btn_filter_misc", LanguageKey.LK_Jiao),
				new FilterToggleConfig("ui9_btn_filter_material", LanguageKey.LK_JiaoLuan)
			};
		}

		// Token: 0x0600AAA2 RID: 43682 RVA: 0x004E8E64 File Offset: 0x004E7064
		protected override List<ToggleIdIndex> GetActiveDependencies()
		{
			return null;
		}

		// Token: 0x0600AAA3 RID: 43683 RVA: 0x004E8E68 File Offset: 0x004E7068
		public override bool IsDataMatch(ITradeableContent data, LineState lineState)
		{
			ToggleKey toggleState = lineState.ToggleGroupState;
			Jiao jiao;
			bool flag = toggleState.IsAll || !this._jiaoByKey.TryGetValue(data.Key, out jiao);
			bool result;
			if (flag)
			{
				result = toggleState.IsAll;
			}
			else
			{
				int index = toggleState.Index;
				if (!true)
				{
				}
				bool flag2;
				if (index != 0)
				{
					flag2 = (index != 1 || jiao.GrowthStage == 0);
				}
				else
				{
					flag2 = (jiao.GrowthStage != 0);
				}
				if (!true)
				{
				}
				result = flag2;
			}
			return result;
		}

		// Token: 0x040084D8 RID: 34008
		private readonly IReadOnlyDictionary<ItemKey, Jiao> _jiaoByKey;
	}
}
