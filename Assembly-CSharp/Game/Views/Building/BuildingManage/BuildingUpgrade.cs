using System;
using System.Collections.Generic;
using Config;
using GameData.Domains.Building;
using UnityEngine;

namespace Game.Views.Building.BuildingManage
{
	// Token: 0x02000C06 RID: 3078
	public class BuildingUpgrade : MonoBehaviour
	{
		// Token: 0x1700108C RID: 4236
		// (get) Token: 0x06009C73 RID: 40051 RVA: 0x0049458C File Offset: 0x0049278C
		private BuildingModel BuildingModel
		{
			get
			{
				return SingletonObject.getInstance<BuildingModel>();
			}
		}

		// Token: 0x06009C74 RID: 40052 RVA: 0x00494594 File Offset: 0x00492794
		public void Init(Action<ButtonUpgradeBuilding> onConfirm)
		{
			for (int i = 0; i < this.buttons.childCount; i++)
			{
				this.buttons.GetChild(i).GetComponent<ButtonUpgradeBuilding>().Init(i, onConfirm);
			}
		}

		// Token: 0x06009C75 RID: 40053 RVA: 0x004945D8 File Offset: 0x004927D8
		public void Refresh(List<ResourceInfo> costList, BuildingManageDisplayData displayData)
		{
			for (int i = 0; i < this.slots.childCount; i++)
			{
				bool flag = displayData.BlockData.SlotIsUnlocked(i + 1);
				if (flag)
				{
					this.buttons.GetChild(i).gameObject.SetActive(false);
					this.slots.GetChild(i).gameObject.SetActive(true);
				}
				else
				{
					Transform obj = this.buttons.GetChild(i);
					ResourceInfo cost = costList[i];
					obj.GetComponent<ButtonUpgradeBuilding>().Refresh(cost, this.BuildingModel.GetResourceCount(cost.ResourceType));
					obj.gameObject.SetActive(true);
					this.slots.GetChild(i).gameObject.SetActive(false);
				}
			}
		}

		// Token: 0x0400793B RID: 31035
		public Transform slots;

		// Token: 0x0400793C RID: 31036
		public Transform buttons;
	}
}
