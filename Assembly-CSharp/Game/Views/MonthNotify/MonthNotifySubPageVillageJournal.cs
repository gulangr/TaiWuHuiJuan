using System;
using System.Collections.Generic;
using FrameWork;
using FrameWork.UISystem.UIElements;
using Game.Components.Common;
using GameData.Domains.World.Notification;
using TMPro;
using UILogic.DisplayDataStructure;
using UnityEngine;

namespace Game.Views.MonthNotify
{
	// Token: 0x020008C4 RID: 2244
	public class MonthNotifySubPageVillageJournal : MonoBehaviour
	{
		// Token: 0x17000C9F RID: 3231
		// (get) Token: 0x06006AEE RID: 27374 RVA: 0x00316AD4 File Offset: 0x00314CD4
		private static MonthlyNotificationSortingGroups MonthlyNotificationSortingGroups
		{
			get
			{
				return SingletonObject.getInstance<MonthlyNotificationSortingGroups>();
			}
		}

		// Token: 0x06006AEF RID: 27375 RVA: 0x00316ADB File Offset: 0x00314CDB
		public void Init(Action<NotificationItem, ArgumentBox> action, Action closeAction)
		{
			this.scroll.Init(action, MonthNotifySubPageVillageJournal.MonthlyNotificationSortingGroups.ReviewTitles[EMonthlyNotificationSectionType.TaiwuVillage]);
			this.btnClose.ClearAndAddListener(closeAction);
		}

		// Token: 0x06006AF0 RID: 27376 RVA: 0x00316B08 File Offset: 0x00314D08
		public void Set(MonthNotify data, List<NotificationItem> notificationData)
		{
			this._data = data;
			this._notificationData = notificationData;
			this.Refresh();
		}

		// Token: 0x06006AF1 RID: 27377 RVA: 0x00316B20 File Offset: 0x00314D20
		private unsafe void Refresh()
		{
			for (int i = 0; i < 8; i++)
			{
				PropertyItem item = this.resourceBarParent.GetChild(i).GetComponent<PropertyItem>();
				int value = *this._data.Resources[i];
				int delta = this._data.ResourceDelta[i];
				item.Set(string.Format("ui9_btn_resource_bar_{0}_0", i), CommonUtils.GetDisplayStringForNum(value, 100000), this.GetNumberString(delta, false), null, false);
			}
			bool showBuy = this._data.GainItem <= 0 && this._data.PawnShopItem > 0;
			this.gainItem.SetActive(!showBuy);
			this.buyItem.SetActive(showBuy);
			this.gainMoneyLabel.text = this.GetNumberString(this._data.GainMoney, true);
			this.gainAuthorityLabel.text = this.GetNumberString(this._data.GainAuthority, true);
			this.gainItemLabel.text = this.GetNumberString(this._data.GainItem, true);
			this.buyItemLabel.text = this.GetNumberString(this._data.PawnShopItem, true);
			this.villagerLabel.text = this.GetNumberString(this._data.GainVillager, true);
			this.villagerCountLabel.text = this._data.VillagerCount.ToString();
			this.youthCountLabel.text = this._data.YouthCount.ToString();
			this.idleCountLabel.text = this._data.IdleCount.ToString();
			this.managingCountLabel.text = this._data.ManagingCount.ToString();
			this.dispatchCountLabel.text = this._data.DispatchCount.ToString();
			this.roleCountLabel.text = this._data.RoleCount.ToString();
			this.houseCapacityLabel.text = this._data.HouseCapacity.ToString();
			this.stoneLabel.text = string.Format("{0}/{1}", this._data.StoneCurr, this._data.StoneMax);
			this.warehouseLabel.text = string.Format("{0:0.##}/{1:0.##}", (float)this._data.WarehouseCurr / 100f, (float)this._data.WarehouseMax / 100f);
			this.cultureLabel.text = string.Format("{0}/{1}", this._data.CultureCurr, this._data.CultureMax);
			this.safetyLabel.text = string.Format("{0}/{1}", this._data.SafetyCurr, this._data.SafetyMax);
			this.buildingCapacityLabel.text = string.Format("{0}/{1}", this._data.BuildingCount, this._data.BuildingCapacity);
			this.scroll.Set(this._notificationData);
		}

		// Token: 0x06006AF2 RID: 27378 RVA: 0x00316E74 File Offset: 0x00315074
		private string GetNumberString(int delta, bool zeroOnly)
		{
			if (!true)
			{
			}
			string result;
			if (delta <= 0)
			{
				if (delta >= 0)
				{
					result = (zeroOnly ? "0" : "+0");
				}
				else
				{
					result = delta.ToString().SetColor("brightred");
				}
			}
			else
			{
				result = string.Format("+{0}", delta).SetColor("brightblue");
			}
			if (!true)
			{
			}
			return result;
		}

		// Token: 0x04004D8B RID: 19851
		[SerializeField]
		protected Transform resourceBarParent;

		// Token: 0x04004D8C RID: 19852
		[SerializeField]
		protected GameObject gainItem;

		// Token: 0x04004D8D RID: 19853
		[SerializeField]
		protected GameObject buyItem;

		// Token: 0x04004D8E RID: 19854
		[SerializeField]
		protected TextMeshProUGUI gainMoneyLabel;

		// Token: 0x04004D8F RID: 19855
		[SerializeField]
		protected TextMeshProUGUI gainAuthorityLabel;

		// Token: 0x04004D90 RID: 19856
		[SerializeField]
		protected TextMeshProUGUI gainItemLabel;

		// Token: 0x04004D91 RID: 19857
		[SerializeField]
		protected TextMeshProUGUI buyItemLabel;

		// Token: 0x04004D92 RID: 19858
		[SerializeField]
		protected TextMeshProUGUI villagerLabel;

		// Token: 0x04004D93 RID: 19859
		[SerializeField]
		protected TextMeshProUGUI villagerCountLabel;

		// Token: 0x04004D94 RID: 19860
		[SerializeField]
		protected TextMeshProUGUI youthCountLabel;

		// Token: 0x04004D95 RID: 19861
		[SerializeField]
		protected TextMeshProUGUI idleCountLabel;

		// Token: 0x04004D96 RID: 19862
		[SerializeField]
		protected TextMeshProUGUI managingCountLabel;

		// Token: 0x04004D97 RID: 19863
		[SerializeField]
		protected TextMeshProUGUI dispatchCountLabel;

		// Token: 0x04004D98 RID: 19864
		[SerializeField]
		protected TextMeshProUGUI roleCountLabel;

		// Token: 0x04004D99 RID: 19865
		[SerializeField]
		protected TextMeshProUGUI houseCapacityLabel;

		// Token: 0x04004D9A RID: 19866
		[SerializeField]
		protected TextMeshProUGUI stoneLabel;

		// Token: 0x04004D9B RID: 19867
		[SerializeField]
		protected TextMeshProUGUI warehouseLabel;

		// Token: 0x04004D9C RID: 19868
		[SerializeField]
		protected TextMeshProUGUI cultureLabel;

		// Token: 0x04004D9D RID: 19869
		[SerializeField]
		protected TextMeshProUGUI safetyLabel;

		// Token: 0x04004D9E RID: 19870
		[SerializeField]
		protected TextMeshProUGUI buildingCapacityLabel;

		// Token: 0x04004D9F RID: 19871
		[SerializeField]
		protected MonthNotifyReviewScroll scroll;

		// Token: 0x04004DA0 RID: 19872
		[SerializeField]
		protected CButton btnClose;

		// Token: 0x04004DA1 RID: 19873
		private MonthNotify _data;

		// Token: 0x04004DA2 RID: 19874
		private List<NotificationItem> _notificationData;
	}
}
