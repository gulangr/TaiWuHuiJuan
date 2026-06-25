using System;
using System.Collections.Generic;
using Config;
using GameData.Domains.Item.Display;

namespace Game.Components.SortAndFilter.Item
{
	// Token: 0x02000D44 RID: 3396
	public abstract class WeaponCommonMakeTypeMenu : DetailedFilterMenuLogic<ITradeableContent>
	{
		// Token: 0x170011CA RID: 4554
		// (get) Token: 0x0600A7D7 RID: 42967
		protected abstract sbyte MyResourceType { get; }

		// Token: 0x170011CB RID: 4555
		// (get) Token: 0x0600A7D8 RID: 42968 RVA: 0x004DFC75 File Offset: 0x004DDE75
		public override EFilterLogic LogicType
		{
			get
			{
				return EFilterLogic.Or;
			}
		}

		// Token: 0x0600A7D9 RID: 42969 RVA: 0x004DFC78 File Offset: 0x004DDE78
		public override List<FilterDropdownItemConfig> GetMenuItemConfigs()
		{
			this._optionNames.Clear();
			foreach (WeaponItem weaponConfig in ((IEnumerable<WeaponItem>)Weapon.Instance))
			{
				bool flag = weaponConfig.ResourceType != this.MyResourceType;
				if (!flag)
				{
					short makeItemSubType = weaponConfig.MakeItemSubType;
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

		// Token: 0x0600A7DA RID: 42970 RVA: 0x004DFDC4 File Offset: 0x004DDFC4
		public override bool IsDataMatch(ITradeableContent data, IReadOnlyCollection<int> selectedIndices)
		{
			WeaponItem weaponConfig = Weapon.Instance[data.Key.TemplateId];
			bool flag = weaponConfig == null;
			bool result;
			if (flag)
			{
				result = false;
			}
			else
			{
				short makeItemSubType = weaponConfig.MakeItemSubType;
				bool flag2 = makeItemSubType < 0 || (int)makeItemSubType >= MakeItemSubType.Instance.Count;
				if (flag2)
				{
					result = false;
				}
				else
				{
					foreach (int index in selectedIndices)
					{
						bool flag3 = index >= this._optionNames.Count;
						if (!flag3)
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

		// Token: 0x040083A4 RID: 33700
		private readonly List<string> _optionNames = new List<string>();
	}
}
