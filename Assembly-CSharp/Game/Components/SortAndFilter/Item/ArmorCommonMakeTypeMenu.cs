using System;
using System.Collections.Generic;
using Config;
using GameData.Domains.Item.Display;

namespace Game.Components.SortAndFilter.Item
{
	// Token: 0x02000D46 RID: 3398
	public abstract class ArmorCommonMakeTypeMenu : DetailedFilterMenuLogic<ITradeableContent>
	{
		// Token: 0x170011CD RID: 4557
		// (get) Token: 0x0600A7E1 RID: 42977 RVA: 0x004E000C File Offset: 0x004DE20C
		public override EFilterLogic LogicType
		{
			get
			{
				return EFilterLogic.Or;
			}
		}

		// Token: 0x170011CE RID: 4558
		// (get) Token: 0x0600A7E2 RID: 42978
		protected abstract sbyte MyResourceType { get; }

		// Token: 0x0600A7E3 RID: 42979 RVA: 0x004E0010 File Offset: 0x004DE210
		public override void OnInit()
		{
			this._optionNames.Clear();
			foreach (ArmorItem armorConfig in ((IEnumerable<ArmorItem>)Armor.Instance))
			{
				bool flag = armorConfig.ResourceType != this.MyResourceType;
				if (!flag)
				{
					short makeItemSubType = armorConfig.MakeItemSubType;
					bool flag2 = makeItemSubType < 0 || (int)makeItemSubType >= MakeItemSubType.Instance.Count;
					if (!flag2)
					{
						string filterName = MakeItemSubType.Instance[makeItemSubType].FilterName;
						bool flag3 = string.IsNullOrEmpty(filterName) || this._optionNames.Contains(filterName);
						if (!flag3)
						{
							this._optionNames.Add(filterName);
						}
					}
				}
			}
		}

		// Token: 0x0600A7E4 RID: 42980 RVA: 0x004E00E8 File Offset: 0x004DE2E8
		public override StringKey GetMenuBarLabel()
		{
			return LanguageKey.LK_CommonSortAndFilter_Filter_Item_Third_Title_Hardness;
		}

		// Token: 0x0600A7E5 RID: 42981 RVA: 0x004E0104 File Offset: 0x004DE304
		public override List<FilterDropdownItemConfig> GetMenuItemConfigs()
		{
			List<FilterDropdownItemConfig> configs = new List<FilterDropdownItemConfig>();
			foreach (string optionName in this._optionNames)
			{
				configs.Add(new FilterDropdownItemConfig
				{
					Text = StringKey.CreateDirect(optionName)
				});
			}
			return configs;
		}

		// Token: 0x0600A7E6 RID: 42982 RVA: 0x004E0180 File Offset: 0x004DE380
		public override bool IsDataMatch(ITradeableContent data, IReadOnlyCollection<int> selectedIndices)
		{
			ArmorItem armorConfig = Armor.Instance[data.Key.TemplateId];
			bool flag = armorConfig == null;
			bool result;
			if (flag)
			{
				result = false;
			}
			else
			{
				short makeItemSubType = armorConfig.MakeItemSubType;
				bool flag2 = makeItemSubType < 0 || (int)makeItemSubType >= MakeItemSubType.Instance.Count;
				if (flag2)
				{
					result = false;
				}
				else
				{
					foreach (int index in selectedIndices)
					{
						bool flag3 = index < this._optionNames.Count;
						if (flag3)
						{
							string selectedOptionName = this._optionNames[index];
							bool flag4 = MakeItemSubType.Instance[makeItemSubType].FilterName == selectedOptionName;
							if (flag4)
							{
								return true;
							}
						}
					}
					result = false;
				}
			}
			return result;
		}

		// Token: 0x040083A6 RID: 33702
		private readonly List<string> _optionNames = new List<string>();
	}
}
