using System;
using System.Collections.Generic;
using System.Linq;
using FrameWork.UISystem.Components;

namespace CommonSortAndFilterLegacy
{
	// Token: 0x02000433 RID: 1075
	public abstract class DetailedFilterLine<TData> : FilterLineBase<TData>
	{
		// Token: 0x06003FC8 RID: 16328 RVA: 0x001FB978 File Offset: 0x001F9B78
		public sealed override LineConfig GenerateConfig()
		{
			List<MultiSelectDropdownConfig<DetailFilterMultiSelectDropdownMenuBarConfig, DetailFilterMultiSelectDropdownItemConfig>> menuConfigs = new List<MultiSelectDropdownConfig<DetailFilterMultiSelectDropdownMenuBarConfig, DetailFilterMultiSelectDropdownItemConfig>>();
			this._menus.AddRange(this.GenerateMenus());
			foreach (DetailedFilterMenuBase<TData> menu in this._menus)
			{
				menu.OnInit();
				List<DetailFilterMultiSelectDropdownItemConfig> menuItemConfigs = menu.GetMenuConfigs();
				menuConfigs.Add(new MultiSelectDropdownConfig<DetailFilterMultiSelectDropdownMenuBarConfig, DetailFilterMultiSelectDropdownItemConfig>
				{
					MenuBarConfig = menu.GetMenuBarConfig(),
					MenuItemConfigs = menuItemConfigs,
					Id = menu.Id,
					Dependency = menu.Dependency,
					Version = menu.Version,
					EnableDragHold = menu.EnableDragHold,
					LogicType = menu.LogicType
				});
			}
			LineConfig lineConfig = this.CreateConfig(menuConfigs);
			lineConfig.Id = this.Id;
			lineConfig.Level = this.Level;
			lineConfig.IndividualLine = this.IndividualLine;
			lineConfig.DefaultActive = this.DefaultActive;
			List<ToggleIdIndex> dependencies = this.GetActiveDependencies();
			lineConfig.ActiveCondition = ((dependencies == null) ? null : new ActiveCondition?(new ActiveCondition(dependencies)));
			return lineConfig;
		}

		// Token: 0x06003FC9 RID: 16329 RVA: 0x001FBAD0 File Offset: 0x001F9CD0
		protected virtual LineConfig CreateConfig(List<MultiSelectDropdownConfig<DetailFilterMultiSelectDropdownMenuBarConfig, DetailFilterMultiSelectDropdownItemConfig>> menuConfigs)
		{
			return LineConfig.CreateDetailedFilterLineConfig(new DetailedFilterLineConfig(new DetailedFilterConfig
			{
				MenuConfigs = menuConfigs
			}));
		}

		// Token: 0x06003FCA RID: 16330 RVA: 0x001FBB00 File Offset: 0x001F9D00
		public sealed override bool IsDataMatch(TData data, LineState lineState)
		{
			Dictionary<int, DetailedFilterMenuState> menuStates = lineState.DetailedFilterState.State.MenuStateDict;
			bool hasDerivative = false;
			bool derivativeApprove = false;
			using (List<DetailedFilterMenuBase<TData>>.Enumerator enumerator = this._menus.GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					DetailedFilterMenuBase<TData> menu = enumerator.Current;
					DetailedFilterMenuState menuState = menuStates[menu.Id];
					bool flag = !menuState.IsActive;
					if (!flag)
					{
						IReadOnlyCollection<int> selections = menuState.SelectedIndices;
						bool flag2 = selections.Count == 0;
						if (!flag2)
						{
							bool flag3 = menu.Dependency != null;
							if (flag3)
							{
								this._tempSelectedIndices.Clear();
								this._tempSelectedIndices.Add(menu.Dependency.Value.OptionIndex);
								bool flag4 = this._menus.Any((DetailedFilterMenuBase<TData> t) => t.Id == menu.Dependency.Value.MenuId && t.IsDataMatch(data, this._tempSelectedIndices));
								if (flag4)
								{
									hasDerivative = true;
									derivativeApprove |= menu.IsDataMatch(data, selections);
								}
							}
							else
							{
								bool flag5 = !menu.IsDataMatch(data, selections);
								if (flag5)
								{
									return false;
								}
							}
						}
					}
				}
			}
			return !hasDerivative || derivativeApprove;
		}

		// Token: 0x06003FCB RID: 16331
		protected abstract IEnumerable<DetailedFilterMenuBase<TData>> GenerateMenus();

		// Token: 0x06003FCC RID: 16332 RVA: 0x001FBCB0 File Offset: 0x001F9EB0
		public List<DetailedFilterMenuBase<TData>> GetMenus()
		{
			return this.GenerateMenus().ToList<DetailedFilterMenuBase<TData>>();
		}

		// Token: 0x17000681 RID: 1665
		// (get) Token: 0x06003FCD RID: 16333
		protected abstract int Level { get; }

		// Token: 0x17000682 RID: 1666
		// (get) Token: 0x06003FCE RID: 16334 RVA: 0x001FBCBD File Offset: 0x001F9EBD
		protected virtual bool DefaultActive
		{
			get
			{
				return true;
			}
		}

		// Token: 0x17000683 RID: 1667
		// (get) Token: 0x06003FCF RID: 16335 RVA: 0x001FBCC0 File Offset: 0x001F9EC0
		protected virtual bool IndividualLine
		{
			get
			{
				return false;
			}
		}

		// Token: 0x06003FD0 RID: 16336
		protected abstract List<ToggleIdIndex> GetActiveDependencies();

		// Token: 0x06003FD1 RID: 16337 RVA: 0x001FBCC4 File Offset: 0x001F9EC4
		public override DynamicLineConfig GenerateDynamicConfig(List<TData> dataList)
		{
			Dictionary<int, MultiSelectDropdownConfig<DetailFilterMultiSelectDropdownMenuBarConfig, DetailFilterMultiSelectDropdownItemConfig>> changedMenuConfigs = new Dictionary<int, MultiSelectDropdownConfig<DetailFilterMultiSelectDropdownMenuBarConfig, DetailFilterMultiSelectDropdownItemConfig>>();
			foreach (DetailedFilterMenuBase<TData> menu in this._menus)
			{
				DynamicDetailedFilterMenuBase<TData> dynamicMenu = menu as DynamicDetailedFilterMenuBase<TData>;
				bool flag = dynamicMenu == null;
				if (!flag)
				{
					List<DetailFilterMultiSelectDropdownItemConfig> menuConfig = dynamicMenu.GetDynamicMenuConfigs(dataList);
					MultiSelectDropdownConfig<DetailFilterMultiSelectDropdownMenuBarConfig, DetailFilterMultiSelectDropdownItemConfig> dynamicMenuConfig = new MultiSelectDropdownConfig<DetailFilterMultiSelectDropdownMenuBarConfig, DetailFilterMultiSelectDropdownItemConfig>
					{
						MenuBarConfig = menu.GetMenuBarConfig(),
						MenuItemConfigs = menuConfig,
						Id = menu.Id,
						Dependency = menu.Dependency,
						Version = menu.Version,
						LogicType = menu.LogicType
					};
					changedMenuConfigs.Add(menu.Id, dynamicMenuConfig);
				}
			}
			return new DynamicLineConfig
			{
				Id = this.Id,
				Type = ESortAndFilterOneLineType.DetailedFilter,
				DetailedFilterLineConfig = new DynamicDetailedFilterLineConfig
				{
					ChangedMenuConfigs = changedMenuConfigs
				}
			};
		}

		// Token: 0x04002D99 RID: 11673
		private readonly List<DetailedFilterMenuBase<TData>> _menus = new List<DetailedFilterMenuBase<TData>>();

		// Token: 0x04002D9A RID: 11674
		private HashSet<int> _tempSelectedIndices = new HashSet<int>();
	}
}
