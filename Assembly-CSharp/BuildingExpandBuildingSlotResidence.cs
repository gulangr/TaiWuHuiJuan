using System;
using System.Collections.Generic;
using Config;

// Token: 0x020001A4 RID: 420
public class BuildingExpandBuildingSlotResidence : BuildingExpandBuildingSlotCommon
{
	// Token: 0x060017D3 RID: 6099 RVA: 0x000928D0 File Offset: 0x00090AD0
	protected override List<ResourceInfo> GetConfig()
	{
		return GlobalConfig.Instance.ResidentUnlockCost;
	}

	// Token: 0x17000291 RID: 657
	// (get) Token: 0x060017D4 RID: 6100 RVA: 0x000928EC File Offset: 0x00090AEC
	protected override BuildingExpandBuildingSlotCommon.EParticle ParticleType
	{
		get
		{
			return BuildingExpandBuildingSlotCommon.EParticle.NormalSize;
		}
	}
}
