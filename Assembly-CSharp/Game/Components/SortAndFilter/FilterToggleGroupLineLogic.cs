using System;
using System.Collections.Generic;

namespace Game.Components.SortAndFilter
{
	// Token: 0x02000CA2 RID: 3234
	public abstract class FilterToggleGroupLineLogic<TData> : FilterLineBase<TData>
	{
		// Token: 0x0600A481 RID: 42113 RVA: 0x004CCA1C File Offset: 0x004CAC1C
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

		// Token: 0x0600A482 RID: 42114
		protected abstract List<FilterToggleConfig> GetFilterToggleConfigs();

		// Token: 0x0600A483 RID: 42115
		protected abstract List<ToggleIdIndex> GetActiveDependencies();

		// Token: 0x1700112D RID: 4397
		// (get) Token: 0x0600A484 RID: 42116
		protected abstract int Level { get; }

		// Token: 0x1700112E RID: 4398
		// (get) Token: 0x0600A485 RID: 42117 RVA: 0x004CCAAE File Offset: 0x004CACAE
		protected virtual bool DefaultActive
		{
			get
			{
				return true;
			}
		}

		// Token: 0x1700112F RID: 4399
		// (get) Token: 0x0600A486 RID: 42118 RVA: 0x004CCAB1 File Offset: 0x004CACB1
		protected virtual bool IndividualLine
		{
			get
			{
				return false;
			}
		}

		// Token: 0x0600A487 RID: 42119 RVA: 0x004CCAB4 File Offset: 0x004CACB4
		public override DynamicLineConfig GenerateDynamicConfig(IEnumerable<TData> dataList)
		{
			return null;
		}
	}
}
