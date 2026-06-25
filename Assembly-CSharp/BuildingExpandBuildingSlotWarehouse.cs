using System;
using System.Collections.Generic;
using Config;

// Token: 0x020001A5 RID: 421
public class BuildingExpandBuildingSlotWarehouse : BuildingExpandBuildingSlotCommon
{
	// Token: 0x060017D6 RID: 6102 RVA: 0x000928F8 File Offset: 0x00090AF8
	protected override List<ResourceInfo> GetConfig()
	{
		return GlobalConfig.Instance.WarehouseUnlockCost;
	}

	// Token: 0x17000292 RID: 658
	// (get) Token: 0x060017D7 RID: 6103 RVA: 0x00092914 File Offset: 0x00090B14
	protected override BuildingExpandBuildingSlotCommon.EParticle ParticleType
	{
		get
		{
			return BuildingExpandBuildingSlotCommon.EParticle.NormalSize;
		}
	}
}
