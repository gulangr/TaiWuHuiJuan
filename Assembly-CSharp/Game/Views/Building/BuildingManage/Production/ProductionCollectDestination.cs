using System;
using System.Collections.Generic;
using System.Linq;
using Config;
using FrameWork;
using FrameWork.UISystem.UIElements;
using GameData.Domains.Building;
using GameData.Domains.Taiwu;
using UnityEngine;
using UnityEngine.Events;

namespace Game.Views.Building.BuildingManage.Production
{
	// Token: 0x02000C18 RID: 3096
	public class ProductionCollectDestination : MonoBehaviour, IProductionComponent
	{
		// Token: 0x170010A9 RID: 4265
		// (get) Token: 0x06009D6C RID: 40300 RVA: 0x0049BD77 File Offset: 0x00499F77
		private BuildingModel Model
		{
			get
			{
				return SingletonObject.getInstance<BuildingModel>();
			}
		}

		// Token: 0x170010AA RID: 4266
		// (get) Token: 0x06009D6D RID: 40301 RVA: 0x0049BD80 File Offset: 0x00499F80
		private IReadOnlyList<TaiwuVillageStorageType> AllowTypes
		{
			get
			{
				EProductionCollectDestinationType eproductionCollectDestinationType = this.Type;
				if (!true)
				{
				}
				IReadOnlyList<TaiwuVillageStorageType> result;
				if (eproductionCollectDestinationType != EProductionCollectDestinationType.Resource)
				{
					if (eproductionCollectDestinationType != EProductionCollectDestinationType.Item)
					{
						throw new Exception(string.Format("Undefined type {0}", this.Type));
					}
					result = BuildingResourceOutputSetting.AllowedItemStorageTypes;
				}
				else
				{
					result = BuildingResourceOutputSetting.AllowedResourceStorageTypes;
				}
				if (!true)
				{
				}
				return result;
			}
		}

		// Token: 0x170010AB RID: 4267
		// (get) Token: 0x06009D6E RID: 40302 RVA: 0x0049BDD4 File Offset: 0x00499FD4
		private TaiwuVillageStorageType CurrentType
		{
			get
			{
				EProductionCollectDestinationType eproductionCollectDestinationType = this.Type;
				if (!true)
				{
				}
				TaiwuVillageStorageType result;
				if (eproductionCollectDestinationType != EProductionCollectDestinationType.Resource)
				{
					if (eproductionCollectDestinationType != EProductionCollectDestinationType.Item)
					{
						throw new Exception(string.Format("Undefined type {0}", this.Type));
					}
					result = this.Model.GetBuildingShopEventSetting((int)this._handler.BlockIndex).ItemStorage;
				}
				else
				{
					result = this.Model.GetBuildingShopEventSetting((int)this._handler.BlockIndex).ResourceStorage;
				}
				if (!true)
				{
				}
				return result;
			}
		}

		// Token: 0x170010AC RID: 4268
		// (get) Token: 0x06009D6F RID: 40303 RVA: 0x0049BE54 File Offset: 0x0049A054
		public EProductionCollectDestinationType Type
		{
			get
			{
				IProductionHandler handler = this._handler;
				bool flag;
				if (handler == null)
				{
					flag = false;
				}
				else
				{
					BuildingBlockItem template = handler.Template;
					bool? flag2 = (template != null) ? new bool?(template.ShowItemLocationAsResourceLocation) : null;
					bool flag3 = true;
					flag = (flag2.GetValueOrDefault() == flag3 & flag2 != null);
				}
				return flag ? EProductionCollectDestinationType.Resource : this.type;
			}
		}

		// Token: 0x06009D70 RID: 40304 RVA: 0x0049BEAC File Offset: 0x0049A0AC
		private void Awake()
		{
			this.dropdown.onValueChanged.AddListener(new UnityAction<int>(this.DoChangeType));
		}

		// Token: 0x06009D71 RID: 40305 RVA: 0x0049BECC File Offset: 0x0049A0CC
		private void OnEnable()
		{
			GEvent.Add(UiEvents.BuildingResourceOutputSettingsChanged, new GEvent.Callback(this.RefreshCall));
		}

		// Token: 0x06009D72 RID: 40306 RVA: 0x0049BEE8 File Offset: 0x0049A0E8
		private void OnDisable()
		{
			GEvent.Remove(UiEvents.BuildingResourceOutputSettingsChanged, new GEvent.Callback(this.RefreshCall));
		}

		// Token: 0x06009D73 RID: 40307 RVA: 0x0049BF04 File Offset: 0x0049A104
		private void RefreshCall(ArgumentBox _)
		{
			this.Refresh();
		}

		// Token: 0x06009D74 RID: 40308 RVA: 0x0049BF10 File Offset: 0x0049A110
		private CDropdown.OptionData ConvertToOption(TaiwuVillageStorageType arg)
		{
			if (!true)
			{
			}
			string text;
			switch (arg)
			{
			case TaiwuVillageStorageType.Inventory:
				text = LanguageKey.LK_Inventory.Tr();
				break;
			case TaiwuVillageStorageType.Warehouse:
				text = LanguageKey.LK_Warehouse.Tr();
				break;
			case TaiwuVillageStorageType.Treasury:
				text = LanguageKey.LK_Treasury.Tr();
				break;
			case TaiwuVillageStorageType.Stock:
				text = LanguageKey.LK_StockStorage.Tr();
				break;
			default:
				text = arg.ToString();
				break;
			}
			if (!true)
			{
			}
			return new CDropdown.OptionData(text);
		}

		// Token: 0x06009D75 RID: 40309 RVA: 0x0049BF8C File Offset: 0x0049A18C
		private void DoChangeType(int newIndex)
		{
			bool flag = this._handler == null;
			if (!flag)
			{
				TaiwuVillageStorageType newType = this.AllowTypes[newIndex];
				bool flag2 = newType == this.CurrentType;
				if (!flag2)
				{
					BuildingResourceOutputSetting settings = this.Model.GetBuildingShopEventSetting((int)this._handler.BlockIndex);
					bool flag3 = this.Type == EProductionCollectDestinationType.Resource;
					if (flag3)
					{
						settings.ResourceStorage = newType;
					}
					else
					{
						bool flag4 = this.Type == EProductionCollectDestinationType.Item;
						if (!flag4)
						{
							return;
						}
						settings.ItemStorage = newType;
					}
					this.Model.SetBuildingResourceOutputSetting((int)this._handler.BlockIndex, settings);
					this.Refresh();
				}
			}
		}

		// Token: 0x06009D76 RID: 40310 RVA: 0x0049C030 File Offset: 0x0049A230
		public void Setup(IProductionHandler handler)
		{
			this._handler = handler;
		}

		// Token: 0x06009D77 RID: 40311 RVA: 0x0049C03C File Offset: 0x0049A23C
		public void Refresh()
		{
			base.gameObject.SetActive((this._handler.Template.ShowItemStoreLocation && this.Type == EProductionCollectDestinationType.Item) || (this._handler.Template.ShowResourceStoreLocation && this.Type == EProductionCollectDestinationType.Resource));
			this.dropdown.options = new List<CDropdown.OptionData>(this.AllowTypes.Select(new Func<TaiwuVillageStorageType, CDropdown.OptionData>(this.ConvertToOption)));
			int storageTypeIndex = this.AllowTypes.IndexOf(this.CurrentType);
			this.dropdown.SetValueWithoutNotify(storageTypeIndex);
		}

		// Token: 0x040079ED RID: 31213
		[SerializeField]
		private CDropdown dropdown;

		// Token: 0x040079EE RID: 31214
		[SerializeField]
		private EProductionCollectDestinationType type;

		// Token: 0x040079EF RID: 31215
		private IProductionHandler _handler;
	}
}
