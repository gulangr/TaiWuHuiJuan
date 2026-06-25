using System;

namespace Game.Views.Building.BuildingManage.Production
{
	// Token: 0x02000C15 RID: 3093
	public interface IProductionComponent
	{
		// Token: 0x06009D49 RID: 40265
		void Setup(IProductionHandler handler);

		// Token: 0x06009D4A RID: 40266
		void Refresh();
	}
}
