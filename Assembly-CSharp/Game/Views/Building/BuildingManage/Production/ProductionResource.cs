using System;
using System.Collections.Generic;
using Config;
using FrameWork.UISystem.UIElements;
using GameData.Domains.Building;
using GameData.Utilities;
using UnityEngine;

namespace Game.Views.Building.BuildingManage.Production
{
	// Token: 0x02000C1B RID: 3099
	public class ProductionResource : MonoBehaviour, IProductionComponent
	{
		// Token: 0x170010AD RID: 4269
		// (get) Token: 0x06009D7E RID: 40318 RVA: 0x0049C229 File Offset: 0x0049A429
		private BuildingModel Model
		{
			get
			{
				return SingletonObject.getInstance<BuildingModel>();
			}
		}

		// Token: 0x170010AE RID: 4270
		// (get) Token: 0x06009D7F RID: 40319 RVA: 0x0049C230 File Offset: 0x0049A430
		private bool AnyOutput
		{
			get
			{
				IReadOnlyList<sbyte> resourceTypes = this.ResourceTypes;
				return resourceTypes != null && resourceTypes.Count > 0;
			}
		}

		// Token: 0x170010AF RID: 4271
		// (get) Token: 0x06009D80 RID: 40320 RVA: 0x0049C253 File Offset: 0x0049A453
		private IReadOnlyList<sbyte> ResourceTypes
		{
			get
			{
				IProductionHandler handler = this._handler;
				IReadOnlyList<sbyte> result;
				if (handler == null)
				{
					result = null;
				}
				else
				{
					ShopEventItem shopEvent = handler.ShopEvent;
					result = ((shopEvent != null) ? shopEvent.ResourceList : null);
				}
				return result;
			}
		}

		// Token: 0x06009D81 RID: 40321 RVA: 0x0049C273 File Offset: 0x0049A473
		public void Setup(IProductionHandler handler)
		{
			this._handler = handler;
		}

		// Token: 0x06009D82 RID: 40322 RVA: 0x0049C280 File Offset: 0x0049A480
		public void Refresh()
		{
			bool flag = this.AnyOutput != base.gameObject.activeSelf;
			if (flag)
			{
				base.gameObject.SetActive(this.AnyOutput);
			}
			bool flag2 = !this.AnyOutput;
			if (!flag2)
			{
				this.resourceGroup.OnActiveIndexChange -= this.OnActiveChanged;
				this.resourceGroup.Clear();
				Transform resourceGroupTransform = this.resourceGroup.transform;
				for (int i = 0; i < this.ResourceTypes.Count; i++)
				{
					bool flag3 = i == resourceGroupTransform.childCount;
					if (flag3)
					{
						Object.Instantiate<Transform>(resourceGroupTransform.GetChild(0), resourceGroupTransform);
					}
					ProductionResourceItem resourceItem = this.RefreshItem(i);
					bool flag4 = !resourceItem.gameObject.activeSelf;
					if (flag4)
					{
						resourceItem.gameObject.SetActive(true);
					}
					this.resourceGroup.Add(resourceItem.toggle);
				}
				for (int j = this.ResourceTypes.Count; j < resourceGroupTransform.childCount; j++)
				{
					Transform resourceItem2 = resourceGroupTransform.GetChild(j);
					bool activeSelf = resourceItem2.gameObject.activeSelf;
					if (activeSelf)
					{
						resourceItem2.gameObject.SetActive(false);
					}
				}
				int activeIndex = this.CalcActiveIndex();
				this.resourceGroup.Set(activeIndex, false);
				this.resourceGroup.OnActiveIndexChange += this.OnActiveChanged;
			}
		}

		// Token: 0x06009D83 RID: 40323 RVA: 0x0049C3FC File Offset: 0x0049A5FC
		private ProductionResourceItem RefreshItem(int i)
		{
			sbyte type = this.ResourceTypes[i];
			ProductionResourceItem resourceItem = this.resourceGroup.transform.GetChild(i).GetComponent<ProductionResourceItem>();
			Dictionary<sbyte, int> resourceOutputValue = this._handler.Data.ResourceOutputValue;
			int addValue = (resourceOutputValue != null) ? resourceOutputValue.GetOrDefault(type) : 0;
			resourceItem.Set(type, this.Model.GetResourceCount(type), addValue);
			return resourceItem;
		}

		// Token: 0x06009D84 RID: 40324 RVA: 0x0049C468 File Offset: 0x0049A668
		private int CalcActiveIndex()
		{
			bool flag = this.ResourceTypes == null || this.ResourceTypes.Count <= 1;
			int result;
			if (flag)
			{
				result = 0;
			}
			else
			{
				sbyte type = this.Model.GetCollectBuildingResourceTypeReal(this._handler.Key, this._handler.Data.BlockData);
				int index = this.ResourceTypes.IndexOf(type);
				result = ((index < 0) ? 0 : index);
			}
			return result;
		}

		// Token: 0x06009D85 RID: 40325 RVA: 0x0049C4DC File Offset: 0x0049A6DC
		private void OnActiveChanged(int newIndex, int oldIndex)
		{
			sbyte newType = this.ResourceTypes[newIndex];
			BuildingDomainMethod.Call.SetCollectBuildingResourceType(this._handler.Key, newType);
			this._handler.Reload();
		}

		// Token: 0x040079F5 RID: 31221
		[SerializeField]
		private CToggleGroup resourceGroup;

		// Token: 0x040079F6 RID: 31222
		private IProductionHandler _handler;
	}
}
