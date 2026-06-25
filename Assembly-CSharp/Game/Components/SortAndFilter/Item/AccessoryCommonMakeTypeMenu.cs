using System;
using System.Collections.Generic;
using Config;
using GameData.Domains.Item.Display;

namespace Game.Components.SortAndFilter.Item
{
	// Token: 0x02000D6C RID: 3436
	public abstract class AccessoryCommonMakeTypeMenu : DetailedFilterMenuLogic<ITradeableContent>
	{
		// Token: 0x17001211 RID: 4625
		// (get) Token: 0x0600A856 RID: 43094 RVA: 0x004E0A33 File Offset: 0x004DEC33
		public override EFilterLogic LogicType
		{
			get
			{
				return EFilterLogic.Or;
			}
		}

		// Token: 0x17001212 RID: 4626
		// (get) Token: 0x0600A857 RID: 43095
		protected abstract sbyte MyResourceType { get; }

		// Token: 0x0600A858 RID: 43096 RVA: 0x004E0A38 File Offset: 0x004DEC38
		public override void OnInit()
		{
			this._optionNames.Clear();
			foreach (AccessoryItem accessoryConfig in ((IEnumerable<AccessoryItem>)Accessory.Instance))
			{
				bool flag = accessoryConfig.ResourceType != this.MyResourceType;
				if (!flag)
				{
					short makeItemSubType = accessoryConfig.MakeItemSubType;
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

		// Token: 0x0600A859 RID: 43097 RVA: 0x004E0B10 File Offset: 0x004DED10
		public override StringKey GetMenuBarLabel()
		{
			return LanguageKey.LK_CommonSortAndFilter_Filter_Item_Third_Title_Hardness;
		}

		// Token: 0x0600A85A RID: 43098 RVA: 0x004E0B2C File Offset: 0x004DED2C
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

		// Token: 0x0600A85B RID: 43099 RVA: 0x004E0BA8 File Offset: 0x004DEDA8
		public override bool IsDataMatch(ITradeableContent data, IReadOnlyCollection<int> selectedIndices)
		{
			AccessoryItem accessoryConfig = Accessory.Instance[data.Key.TemplateId];
			bool flag = accessoryConfig == null;
			bool result;
			if (flag)
			{
				result = false;
			}
			else
			{
				short makeItemSubType = accessoryConfig.MakeItemSubType;
				foreach (int index in selectedIndices)
				{
					bool flag2 = index < this._optionNames.Count;
					if (flag2)
					{
						string selectedOptionName = this._optionNames[index];
						bool flag3 = makeItemSubType < 0 || (int)makeItemSubType >= MakeItemSubType.Instance.Count;
						if (!flag3)
						{
							bool flag4 = MakeItemSubType.Instance[makeItemSubType].FilterName == selectedOptionName;
							if (flag4)
							{
								return true;
							}
						}
					}
				}
				result = false;
			}
			return result;
		}

		// Token: 0x040083E5 RID: 33765
		private readonly List<string> _optionNames = new List<string>();
	}
}
