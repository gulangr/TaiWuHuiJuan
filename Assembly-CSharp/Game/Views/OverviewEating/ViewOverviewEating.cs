using System;
using System.Collections.Generic;
using System.Linq;
using FrameWork;
using FrameWork.UISystem.Components;
using Game.Components.Character;
using Game.Views.UsingMedicine;
using GameData.Domains.Character;
using GameData.Domains.Character.Display;
using GameData.Domains.Item;
using GameData.Domains.Item.Display;
using GameData.GameDataBridge;
using GameData.Serializer;
using UnityEngine;

namespace Game.Views.OverviewEating
{
	// Token: 0x020007D0 RID: 2000
	public class ViewOverviewEating : UIBase
	{
		// Token: 0x060061C4 RID: 25028 RVA: 0x002CDE50 File Offset: 0x002CC050
		public override void OnInit(ArgumentBox argsBox)
		{
			bool flag = argsBox == null || !argsBox.Get("CurrentCharacterId", out this._charId);
			if (flag)
			{
				this._charId = SingletonObject.getInstance<BasicGameData>().TaiwuCharId;
			}
			this.NeedDataListenerId = true;
			UIElement element = this.Element;
			element.OnListenerIdReady = (Action)Delegate.Combine(element.OnListenerIdReady, new Action(this.OnListenerIdReady));
		}

		// Token: 0x060061C5 RID: 25029 RVA: 0x002CDEBA File Offset: 0x002CC0BA
		private void OnListenerIdReady()
		{
			CharacterDomainMethod.Call.GetCharacterOverviewEatingDisplayData(this.Element.GameDataListenerId, this._charId);
		}

		// Token: 0x060061C6 RID: 25030 RVA: 0x002CDED4 File Offset: 0x002CC0D4
		public override void OnNotifyGameData(List<NotificationWrapper> notifications)
		{
			foreach (NotificationWrapper wrapper in notifications)
			{
				Notification notification = wrapper.Notification;
				byte type = notification.Type;
				byte b = type;
				if (b == 1)
				{
					bool flag = notification.DomainId == 4;
					if (flag)
					{
						bool flag2 = notification.MethodId == 218;
						if (flag2)
						{
							Serializer.Deserialize(wrapper.DataPool, notification.ValueOffset, ref this._usingMedicineDisplayData);
							this.attributeAndInjury.Attribute.Set(this._usingMedicineDisplayData.AttributeDisplayData);
							this.attributeAndInjury.Injury.Set(this._usingMedicineDisplayData.InjuryDisplayData, true);
							this.attributeAndInjury.SwitchToInjury();
							this.Element.ShowAfterRefresh();
						}
					}
				}
			}
		}

		// Token: 0x060061C7 RID: 25031 RVA: 0x002CDFD8 File Offset: 0x002CC1D8
		private void Awake()
		{
			this.eatingScroll.OnItemRender += this.OnEatingItemRender;
		}

		// Token: 0x060061C8 RID: 25032 RVA: 0x002CDFF3 File Offset: 0x002CC1F3
		private void OnDestroy()
		{
			this.eatingScroll.OnItemRender -= this.OnEatingItemRender;
		}

		// Token: 0x060061C9 RID: 25033 RVA: 0x002CE010 File Offset: 0x002CC210
		protected override void OnClick(Transform btn)
		{
			base.OnClick(btn);
			string name = btn.name;
			string a = name;
			if (a == "ButtonCloseView")
			{
				this.QuickHide();
			}
		}

		// Token: 0x060061CA RID: 25034 RVA: 0x002CE048 File Offset: 0x002CC248
		private void RefreshEatingArea()
		{
			this._eatingItemKeyList.Clear();
			this._eatingItemKeyList.AddRange(from d in this._usingMedicineDisplayData.InjuryDisplayData.EatingItemDisplayDataArray
			where d.RealKey.IsValid()
			select d.RealKey);
			this.eatingScroll.SetDataCount(this._eatingItemKeyList.Count);
		}

		// Token: 0x060061CB RID: 25035 RVA: 0x002CE0E0 File Offset: 0x002CC2E0
		private void OnEatingItemRender(int index, GameObject obj)
		{
			ItemKey itemKey = this._eatingItemKeyList[index];
			EatingItem eatingItem = obj.GetComponent<EatingItem>();
			int duration = this._usingMedicineDisplayData.InjuryDisplayData.EatingItems.GetDuration(itemKey);
			eatingItem.Set(itemKey, duration);
		}

		// Token: 0x040043DB RID: 17371
		[SerializeField]
		private InfinityScroll eatingScroll;

		// Token: 0x040043DC RID: 17372
		[SerializeField]
		private AttributeAndInjuryDynamic attributeAndInjury;

		// Token: 0x040043DD RID: 17373
		private CharacterUsingMedicineDisplayData _usingMedicineDisplayData;

		// Token: 0x040043DE RID: 17374
		private readonly List<ItemKey> _eatingItemKeyList = new List<ItemKey>();

		// Token: 0x040043DF RID: 17375
		private int _charId;
	}
}
