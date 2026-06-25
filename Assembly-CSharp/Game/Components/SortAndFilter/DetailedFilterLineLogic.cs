using System;
using System.Collections.Generic;
using System.Linq;

namespace Game.Components.SortAndFilter
{
	// Token: 0x02000C8E RID: 3214
	public abstract class DetailedFilterLineLogic<TData> : FilterLineBase<TData>
	{
		// Token: 0x17001117 RID: 4375
		// (get) Token: 0x0600A3D7 RID: 41943 RVA: 0x004C9D18 File Offset: 0x004C7F18
		public IReadOnlyList<DetailedFilterMenuLogic<TData>> Menus
		{
			get
			{
				return this._menus;
			}
		}

		// Token: 0x0600A3D8 RID: 41944 RVA: 0x004C9D20 File Offset: 0x004C7F20
		public sealed override LineConfig GenerateConfig()
		{
			List<DetailedFilterMenuConfig> menuConfigs = new List<DetailedFilterMenuConfig>();
			this._menus.Clear();
			this._menus.AddRange(this.GenerateMenus());
			foreach (DetailedFilterMenuLogic<TData> menu in this._menus)
			{
				menu.OnInit();
				List<FilterDropdownItemConfig> itemConfigs = menu.GetMenuItemConfigs();
				FilterDropdownConfig dropdownConfig = new FilterDropdownConfig
				{
					MenuBarLabel = menu.GetMenuBarLabel(),
					ItemConfigs = itemConfigs,
					IsMultiSelect = true
				};
				FilterDropdownContext dropdownContext = new FilterDropdownContext
				{
					Id = menu.Id,
					Dependency = menu.Dependency,
					LogicType = menu.LogicType
				};
				menuConfigs.Add(new DetailedFilterMenuConfig
				{
					Id = menu.Id,
					DropdownConfig = dropdownConfig,
					DropdownContext = dropdownContext
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

		// Token: 0x0600A3D9 RID: 41945 RVA: 0x004C9EA8 File Offset: 0x004C80A8
		protected virtual LineConfig CreateConfig(List<DetailedFilterMenuConfig> menuConfigs)
		{
			return LineConfig.CreateDetailedFilterLineConfig(new DetailedFilterLineConfig(new DetailedFilterConfig
			{
				MenuConfigs = menuConfigs
			}));
		}

		// Token: 0x0600A3DA RID: 41946 RVA: 0x004C9ED8 File Offset: 0x004C80D8
		public sealed override bool IsDataMatch(TData data, LineState lineState)
		{
			Dictionary<int, DetailedFilterMenuState> menuStates = lineState.DetailedFilterState.State.MenuStateDict;
			bool hasDerivative = false;
			bool derivativeApprove = false;
			using (List<DetailedFilterMenuLogic<TData>>.Enumerator enumerator = this._menus.GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					DetailedFilterMenuLogic<TData> menu = enumerator.Current;
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
								bool flag4 = this._menus.Any((DetailedFilterMenuLogic<TData> t) => t.Id == menu.Dependency.Value.MenuId && t.IsDataMatch(data, this._tempSelectedIndices));
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

		// Token: 0x0600A3DB RID: 41947
		protected abstract IEnumerable<DetailedFilterMenuLogic<TData>> GenerateMenus();

		// Token: 0x0600A3DC RID: 41948 RVA: 0x004CA088 File Offset: 0x004C8288
		public List<DetailedFilterMenuLogic<TData>> GetMenus()
		{
			return this.GenerateMenus().ToList<DetailedFilterMenuLogic<TData>>();
		}

		// Token: 0x17001118 RID: 4376
		// (get) Token: 0x0600A3DD RID: 41949
		protected abstract int Level { get; }

		// Token: 0x17001119 RID: 4377
		// (get) Token: 0x0600A3DE RID: 41950 RVA: 0x004CA095 File Offset: 0x004C8295
		protected virtual bool DefaultActive
		{
			get
			{
				return true;
			}
		}

		// Token: 0x1700111A RID: 4378
		// (get) Token: 0x0600A3DF RID: 41951 RVA: 0x004CA098 File Offset: 0x004C8298
		protected virtual bool IndividualLine
		{
			get
			{
				return false;
			}
		}

		// Token: 0x0600A3E0 RID: 41952
		protected abstract List<ToggleIdIndex> GetActiveDependencies();

		// Token: 0x0600A3E1 RID: 41953 RVA: 0x004CA09C File Offset: 0x004C829C
		public override DynamicLineConfig GenerateDynamicConfig(IEnumerable<TData> dataList)
		{
			Dictionary<int, FilterDropdownConfig> changedMenuConfigs = new Dictionary<int, FilterDropdownConfig>();
			foreach (DetailedFilterMenuLogic<TData> menu in this._menus)
			{
				DynamicDetailedFilterMenuLogic<TData> dynamicMenu = menu as DynamicDetailedFilterMenuLogic<TData>;
				bool flag = dynamicMenu == null;
				if (!flag)
				{
					List<FilterDropdownItemConfig> menuConfig = dynamicMenu.GetDynamicMenuConfigs(dataList);
					FilterDropdownConfig dynamicMenuConfig = new FilterDropdownConfig
					{
						MenuBarLabel = menu.GetMenuBarLabel(),
						ItemConfigs = menuConfig,
						IsMultiSelect = true
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

		// Token: 0x040081CF RID: 33231
		private readonly List<DetailedFilterMenuLogic<TData>> _menus = new List<DetailedFilterMenuLogic<TData>>();

		// Token: 0x040081D0 RID: 33232
		private readonly HashSet<int> _tempSelectedIndices = new HashSet<int>();
	}
}
