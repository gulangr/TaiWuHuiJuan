using System;
using System.Collections.Generic;
using System.Linq;
using FrameWork.UISystem.Components;

namespace CommonSortAndFilterLegacy.Character
{
	// Token: 0x020005A3 RID: 1443
	public abstract class OrganizationSettlementTypeBaseMenu<T> : DynamicDetailedFilterMenuBase<T> where T : ICharacterSortAndFilterData
	{
		// Token: 0x1700089B RID: 2203
		// (get) Token: 0x06004573 RID: 17779 RVA: 0x0020C392 File Offset: 0x0020A592
		public override FilterLogic LogicType
		{
			get
			{
				return FilterLogic.Or;
			}
		}

		// Token: 0x1700089C RID: 2204
		// (get) Token: 0x06004574 RID: 17780
		protected abstract LanguageKey MenuBarLabelKey { get; }

		// Token: 0x1700089D RID: 2205
		// (get) Token: 0x06004575 RID: 17781
		protected abstract sbyte OrganizationKey { get; }

		// Token: 0x1700089E RID: 2206
		// (get) Token: 0x06004576 RID: 17782
		protected abstract EOrganizationMenuOption DependencyMenuOption { get; }

		// Token: 0x1700089F RID: 2207
		// (get) Token: 0x06004577 RID: 17783 RVA: 0x0020C395 File Offset: 0x0020A595
		public sealed override MenuOptionIndex? Dependency
		{
			get
			{
				return new MenuOptionIndex?(new MenuOptionIndex(5, (int)this.DependencyMenuOption));
			}
		}

		// Token: 0x06004578 RID: 17784 RVA: 0x0020C3A8 File Offset: 0x0020A5A8
		public sealed override DetailFilterMultiSelectDropdownMenuBarConfig GetMenuBarConfig()
		{
			return new DetailFilterMultiSelectDropdownMenuBarConfig
			{
				MenuBarLabel = StringKey.CreateKey(this.MenuBarLabelKey)
			};
		}

		// Token: 0x06004579 RID: 17785 RVA: 0x0020C3D8 File Offset: 0x0020A5D8
		public sealed override List<DetailFilterMultiSelectDropdownItemConfig> GetDynamicMenuConfigs(List<T> dataList)
		{
			WorldMapModel worldMapModel = SingletonObject.getInstance<WorldMapModel>();
			HashSet<string> nameSet = new HashSet<string>();
			foreach (T data in dataList)
			{
				bool flag = data.Organization.OrgTemplateId != this.OrganizationKey;
				if (!flag)
				{
					string settlementName = worldMapModel.GetSettlementName(data.Organization);
					nameSet.Add(settlementName);
				}
			}
			this._options.AddRange(nameSet);
			this._options.Sort();
			return (from optionName in this._options
			select new DetailFilterMultiSelectDropdownItemConfig
			{
				IconPath = null,
				Text = StringKey.CreateDirect(optionName)
			}).ToList<DetailFilterMultiSelectDropdownItemConfig>();
		}

		// Token: 0x0600457A RID: 17786 RVA: 0x0020C4C4 File Offset: 0x0020A6C4
		public sealed override bool IsDataMatch(T data, IReadOnlyCollection<int> selectedIndices)
		{
			WorldMapModel worldMapModel = SingletonObject.getInstance<WorldMapModel>();
			return selectedIndices.Any((int index) => worldMapModel.GetSettlementName(data.Organization) == this._options[index]);
		}

		// Token: 0x0400306D RID: 12397
		private readonly List<string> _options = new List<string>();
	}
}
