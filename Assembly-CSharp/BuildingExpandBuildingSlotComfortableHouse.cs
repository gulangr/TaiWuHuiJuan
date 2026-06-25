using System;
using System.Collections.Generic;
using Config;

// Token: 0x020001A2 RID: 418
public class BuildingExpandBuildingSlotComfortableHouse : BuildingExpandBuildingSlotCommon
{
	// Token: 0x060017BB RID: 6075 RVA: 0x00092234 File Offset: 0x00090434
	protected override List<ResourceInfo> GetConfig()
	{
		return GlobalConfig.Instance.ComfortableHouseUnlockCost;
	}

	// Token: 0x1700028E RID: 654
	// (get) Token: 0x060017BC RID: 6076 RVA: 0x00092250 File Offset: 0x00090450
	protected override BuildingExpandBuildingSlotCommon.EParticle ParticleType
	{
		get
		{
			return BuildingExpandBuildingSlotCommon.EParticle.BigSize;
		}
	}
}
