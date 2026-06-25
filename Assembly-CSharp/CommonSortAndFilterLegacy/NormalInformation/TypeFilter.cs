using System;
using System.Collections.Generic;
using System.Linq;
using Config;
using GameData.Domains.Information;
using UICommon.Character.Elements;

namespace CommonSortAndFilterLegacy.NormalInformation
{
	// Token: 0x02000488 RID: 1160
	public class TypeFilter : FilterToggleGroupLine<NormalInformationDisplayData>
	{
		// Token: 0x170006F4 RID: 1780
		// (get) Token: 0x06004124 RID: 16676 RVA: 0x00200E86 File Offset: 0x001FF086
		public override int Id
		{
			get
			{
				return 0;
			}
		}

		// Token: 0x06004125 RID: 16677 RVA: 0x00200E8C File Offset: 0x001FF08C
		public override bool IsDataMatch(NormalInformationDisplayData data, LineState lineState)
		{
			ToggleKey toggleState = lineState.ToggleGroupState;
			bool isAll = toggleState.IsAll;
			bool result;
			if (isAll)
			{
				result = true;
			}
			else
			{
				InformationItem informationTypeConfigItem = Information.Instance[data.NormalInformation.TemplateId];
				sbyte type = informationTypeConfigItem.Type;
				result = (toggleState.Index == (int)type);
			}
			return result;
		}

		// Token: 0x06004126 RID: 16678 RVA: 0x00200EDC File Offset: 0x001FF0DC
		protected override List<FilterToggleConfig> GetFilterToggleConfigs()
		{
			return (from informationTypeConfigItem in InformationType.Instance
			select new FilterToggleConfig
			{
				IconNames = ToggleTransitionIconSpriteNames.Default(),
				TipContent = StringKey.CreateDirect(informationTypeConfigItem.Name)
			}).ToList<FilterToggleConfig>();
		}

		// Token: 0x06004127 RID: 16679 RVA: 0x00200F1C File Offset: 0x001FF11C
		protected override List<ToggleIdIndex> GetActiveDependencies()
		{
			return null;
		}

		// Token: 0x170006F5 RID: 1781
		// (get) Token: 0x06004128 RID: 16680 RVA: 0x00200F2F File Offset: 0x001FF12F
		protected override int Level
		{
			get
			{
				return 0;
			}
		}
	}
}
