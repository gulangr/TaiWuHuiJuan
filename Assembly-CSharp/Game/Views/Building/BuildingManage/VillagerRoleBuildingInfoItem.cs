using System;
using UnityEngine;

namespace Game.Views.Building.BuildingManage
{
	// Token: 0x02000C13 RID: 3091
	public class VillagerRoleBuildingInfoItem : BuildingInfoItem
	{
		// Token: 0x06009D39 RID: 40249 RVA: 0x0049ABFC File Offset: 0x00498DFC
		protected override void HandleDisableStyle(EBuildingNotAvailableType notAvailableType)
		{
			bool invalid = notAvailableType == EBuildingNotAvailableType.BuildConditionNotMet || notAvailableType == EBuildingNotAvailableType.Locked;
			this.objLock.SetActive(false);
			this.rootInvalid.SetActive(invalid);
			this.disbaleStyle.SetStyleEffect(notAvailableType > EBuildingNotAvailableType.None, false);
		}

		// Token: 0x040079E2 RID: 31202
		[SerializeField]
		private DisableStyleRoot disbaleStyle;
	}
}
