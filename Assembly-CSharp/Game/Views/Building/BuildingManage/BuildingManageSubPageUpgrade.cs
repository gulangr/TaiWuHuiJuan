using System;
using System.Collections.Generic;
using Coffee.UIExtensions;
using Config;
using FrameWork.UISystem.Components.EffectPlayer;
using GameData.Domains.Building;
using GameData.Serializer;
using GameData.Utilities;
using UnityEngine;

namespace Game.Views.Building.BuildingManage
{
	// Token: 0x02000C04 RID: 3076
	public class BuildingManageSubPageUpgrade : BuildingManageSubPage
	{
		// Token: 0x1700108B RID: 4235
		// (get) Token: 0x06009C6E RID: 40046 RVA: 0x00494352 File Offset: 0x00492552
		private BuildingBlockKey BlockKey
		{
			get
			{
				return this.ParentView.BlockKey;
			}
		}

		// Token: 0x06009C6F RID: 40047 RVA: 0x00494360 File Offset: 0x00492560
		private void Awake()
		{
			this.residence.Init(new Action<ButtonUpgradeBuilding>(this.Upgrade));
			this.warehouse.Init(new Action<ButtonUpgradeBuilding>(this.Upgrade));
			this.feast.Init(new Action<ButtonUpgradeBuilding>(this.Upgrade));
		}

		// Token: 0x06009C70 RID: 40048 RVA: 0x004943B8 File Offset: 0x004925B8
		public override void Refresh(BuildingManageDisplayData displayData)
		{
			this.DisplayData = displayData;
			switch (this.DisplayData.BlockData.TemplateId)
			{
			case 46:
				this.residence.gameObject.SetActive(true);
				this.warehouse.gameObject.SetActive(false);
				this.feast.gameObject.SetActive(false);
				this._cost = GlobalConfig.Instance.ResidentUnlockCost;
				this._page = this.residence;
				this._particle = this.normalParticle;
				break;
			case 47:
				this.residence.gameObject.SetActive(false);
				this.warehouse.gameObject.SetActive(false);
				this.feast.gameObject.SetActive(true);
				this._cost = GlobalConfig.Instance.ComfortableHouseUnlockCost;
				this._page = this.feast;
				this._particle = this.bigParticle;
				break;
			case 48:
				this.residence.gameObject.SetActive(false);
				this.warehouse.gameObject.SetActive(true);
				this.feast.gameObject.SetActive(false);
				this._cost = GlobalConfig.Instance.WarehouseUnlockCost;
				this._page = this.warehouse;
				this._particle = this.normalParticle;
				break;
			}
			this._page.Refresh(this._cost, displayData);
		}

		// Token: 0x06009C71 RID: 40049 RVA: 0x00494530 File Offset: 0x00492730
		private void Upgrade(ButtonUpgradeBuilding obj)
		{
			BuildingDomainMethod.AsyncCall.UpgradeSlotBuilding(null, this.BlockKey, obj.Index + 1, delegate(int offset, RawDataPool pool)
			{
				bool success = false;
				Serializer.Deserialize(pool, offset, ref success);
				bool flag = success;
				if (flag)
				{
					SingletonObject.getInstance<BuildingModel>().RefreshResources();
					AudioManager.Instance.PlaySound("ui_industry_put", false, false);
					this._particle.transform.position = obj.transform.position;
					this._particlePlayHelper.PlayOnceParticle(this._particle, 1f, null);
					this.ParentView.RequestData();
				}
				else
				{
					Debug.LogError("upgrade slot building failed");
				}
			});
		}

		// Token: 0x04007921 RID: 31009
		[SerializeField]
		private BuildingUpgrade residence;

		// Token: 0x04007922 RID: 31010
		[SerializeField]
		private BuildingUpgrade warehouse;

		// Token: 0x04007923 RID: 31011
		[SerializeField]
		private BuildingUpgrade feast;

		// Token: 0x04007924 RID: 31012
		[SerializeField]
		private UIParticle bigParticle;

		// Token: 0x04007925 RID: 31013
		[SerializeField]
		private UIParticle normalParticle;

		// Token: 0x04007926 RID: 31014
		private List<ResourceInfo> _cost;

		// Token: 0x04007927 RID: 31015
		private BuildingUpgrade _page;

		// Token: 0x04007928 RID: 31016
		private UIParticle _particle;

		// Token: 0x04007929 RID: 31017
		private readonly UIParticlePlayHelper _particlePlayHelper = new UIParticlePlayHelper();
	}
}
