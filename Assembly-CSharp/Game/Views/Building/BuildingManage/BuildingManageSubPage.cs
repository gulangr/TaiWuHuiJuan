using System;
using GameData.Domains.Building;
using UnityEngine;

namespace Game.Views.Building.BuildingManage
{
	// Token: 0x02000BF4 RID: 3060
	public class BuildingManageSubPage : MonoBehaviour
	{
		// Token: 0x17001069 RID: 4201
		// (get) Token: 0x06009B7F RID: 39807 RVA: 0x0048E22C File Offset: 0x0048C42C
		protected BuildingModel Model
		{
			get
			{
				return SingletonObject.getInstance<BuildingModel>();
			}
		}

		// Token: 0x06009B80 RID: 39808 RVA: 0x0048E233 File Offset: 0x0048C433
		public virtual void Init(ViewBuildingManage parentView)
		{
			this.ParentView = parentView;
		}

		// Token: 0x06009B81 RID: 39809 RVA: 0x0048E23D File Offset: 0x0048C43D
		public virtual void Refresh(BuildingManageDisplayData displayData)
		{
			this.DisplayData = displayData;
		}

		// Token: 0x06009B82 RID: 39810 RVA: 0x0048E247 File Offset: 0x0048C447
		public virtual void RequestData()
		{
		}

		// Token: 0x06009B83 RID: 39811 RVA: 0x0048E24C File Offset: 0x0048C44C
		public virtual bool QuickHide()
		{
			return false;
		}

		// Token: 0x04007885 RID: 30853
		protected BuildingManageDisplayData DisplayData;

		// Token: 0x04007886 RID: 30854
		protected ViewBuildingManage ParentView;
	}
}
