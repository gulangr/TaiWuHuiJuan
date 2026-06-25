using System;
using GameData.Domains.Building;
using UnityEngine;

namespace Game.Views.Building.BuildingManage
{
	// Token: 0x02000BFC RID: 3068
	public class BuildingManageSubPageInfoLive : BuildingManageSubPage
	{
		// Token: 0x06009BFD RID: 39933 RVA: 0x004917AC File Offset: 0x0048F9AC
		public override void Init(ViewBuildingManage parentView)
		{
			base.Init(parentView);
			bool flag = this._currentSubPage;
			if (flag)
			{
				this._currentSubPage.gameObject.SetActive(false);
			}
			else
			{
				this.resident.gameObject.SetActive(false);
				this.comfortableHouse.gameObject.SetActive(false);
			}
			short templateId = this.ParentView.ConfigData.TemplateId;
			if (!true)
			{
			}
			BuildingManageSubPage currentSubPage;
			if (templateId != 46)
			{
				if (templateId != 47)
				{
					currentSubPage = null;
				}
				else
				{
					currentSubPage = this.comfortableHouse;
				}
			}
			else
			{
				currentSubPage = this.resident;
			}
			if (!true)
			{
			}
			this._currentSubPage = currentSubPage;
			bool flag2 = this._currentSubPage;
			if (flag2)
			{
				this._currentSubPage.gameObject.SetActive(true);
				this._currentSubPage.Init(parentView);
			}
		}

		// Token: 0x06009BFE RID: 39934 RVA: 0x00491884 File Offset: 0x0048FA84
		public override void Refresh(BuildingManageDisplayData displayData)
		{
			base.Refresh(displayData);
			bool flag = this._currentSubPage;
			if (flag)
			{
				this._currentSubPage.Refresh(displayData);
			}
		}

		// Token: 0x040078D4 RID: 30932
		[SerializeField]
		private BuildingManageSubPage resident;

		// Token: 0x040078D5 RID: 30933
		[SerializeField]
		private BuildingManageSubPage comfortableHouse;

		// Token: 0x040078D6 RID: 30934
		private BuildingManageSubPage _currentSubPage;
	}
}
