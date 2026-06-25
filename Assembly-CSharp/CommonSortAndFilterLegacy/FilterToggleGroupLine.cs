using System;
using System.Collections.Generic;

namespace CommonSortAndFilterLegacy
{
	// Token: 0x02000440 RID: 1088
	public abstract class FilterToggleGroupLine<TData> : FilterLineBase<TData>
	{
		// Token: 0x06004013 RID: 16403 RVA: 0x001FD228 File Offset: 0x001FB428
		public sealed override LineConfig GenerateConfig()
		{
			LineConfig lineConfig = LineConfig.CreateToggleGroupLineConfig(new ToggleGroupLineConfig
			{
				Config = new FilterToggleGroupConfig
				{
					FilterToggleConfigs = this.GetFilterToggleConfigs()
				}
			});
			lineConfig.Id = this.Id;
			List<ToggleIdIndex> dependencies = this.GetActiveDependencies();
			lineConfig.ActiveCondition = ((dependencies == null) ? null : new ActiveCondition?(new ActiveCondition(dependencies)));
			lineConfig.Level = this.Level;
			lineConfig.IndividualLine = this.IndividualLine;
			lineConfig.DefaultActive = this.DefaultActive;
			return lineConfig;
		}

		// Token: 0x06004014 RID: 16404
		protected abstract List<FilterToggleConfig> GetFilterToggleConfigs();

		// Token: 0x06004015 RID: 16405
		protected abstract List<ToggleIdIndex> GetActiveDependencies();

		// Token: 0x17000694 RID: 1684
		// (get) Token: 0x06004016 RID: 16406
		protected abstract int Level { get; }

		// Token: 0x17000695 RID: 1685
		// (get) Token: 0x06004017 RID: 16407 RVA: 0x001FD2BA File Offset: 0x001FB4BA
		protected virtual bool DefaultActive
		{
			get
			{
				return true;
			}
		}

		// Token: 0x17000696 RID: 1686
		// (get) Token: 0x06004018 RID: 16408 RVA: 0x001FD2BD File Offset: 0x001FB4BD
		protected virtual bool IndividualLine
		{
			get
			{
				return false;
			}
		}

		// Token: 0x06004019 RID: 16409 RVA: 0x001FD2C0 File Offset: 0x001FB4C0
		public override DynamicLineConfig GenerateDynamicConfig(List<TData> dataList)
		{
			return null;
		}
	}
}
