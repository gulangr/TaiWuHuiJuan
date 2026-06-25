using System;
using System.Collections.Generic;
using System.Linq;
using FrameWork.UISystem.UIElements;
using GameData.Domains.Building;
using TMPro;
using UnityEngine;
using UnityEngine.Events;

namespace Game.Views.Building.BuildingManage
{
	// Token: 0x02000BEE RID: 3054
	public class AutoSoldSettingPanel : MonoBehaviour
	{
		// Token: 0x17001065 RID: 4197
		// (get) Token: 0x06009B2B RID: 39723 RVA: 0x0048A509 File Offset: 0x00488709
		private BuildingOptionAutoAddSoldItemPreset CurrArrangementSettingPresetData
		{
			get
			{
				return this.parent.BuildingModel.GetBuildingSoldItemSetting(this.parent.BlockKey, this.parent.BlockData);
			}
		}

		// Token: 0x06009B2C RID: 39724 RVA: 0x0048A534 File Offset: 0x00488734
		private void Awake()
		{
			this.lowestGrade.Init(-1);
			this.highestGrade.Init(-1);
			this.lowestGrade.OnActiveIndexChange += delegate(int newTog, int _)
			{
				int i = this.highestGrade.Count();
				while (i-- > 0)
				{
					this.highestGrade.SetInteractable(i >= newTog, i);
				}
				this._arrangementSetting.MinGrade = (sbyte)newTog;
			};
			this.highestGrade.OnActiveIndexChange += delegate(int newTog, int _)
			{
				int i = this.lowestGrade.Count();
				while (i-- > 0)
				{
					this.lowestGrade.SetInteractable(i <= newTog, i);
				}
				this._arrangementSetting.MaxGrade = (sbyte)newTog;
			};
			this.gradeOrder.Init(-1);
			this.gradeOrder.OnActiveIndexChange += delegate(int newTog, int _)
			{
				this._arrangementSetting.GradeOrder = (sbyte)newTog;
			};
			this.goodsType.Init();
			this.goodsType.OnActiveIndexChange += delegate(int newTog, int oldTog)
			{
				BuildingOptionAutoAddSoldItemPreset arrangementSetting = this._arrangementSetting;
				if (arrangementSetting.ItemTypeList == null)
				{
					arrangementSetting.ItemTypeList = new List<sbyte>();
				}
				bool flag = newTog != -1 && !this._arrangementSetting.ItemTypeList.Contains((sbyte)newTog);
				if (flag)
				{
					this._arrangementSetting.ItemTypeList.Add((sbyte)newTog);
				}
				bool flag2 = oldTog != -1;
				if (flag2)
				{
					this._arrangementSetting.ItemTypeList.Remove((sbyte)oldTog);
				}
			};
			this.goodsProperty.Init();
			this.goodsProperty.OnActiveIndexChange += delegate(int newTog, int oldTog)
			{
				bool flag = newTog != -1;
				if (flag)
				{
					BuildingOptionAutoAddSoldItemPreset arrangementSetting = this._arrangementSetting;
					arrangementSetting.PropertyOrder |= (sbyte)(1 << newTog);
				}
				bool flag2 = oldTog != -1;
				if (flag2)
				{
					BuildingOptionAutoAddSoldItemPreset arrangementSetting2 = this._arrangementSetting;
					arrangementSetting2.PropertyOrder &= (sbyte)(~(sbyte)(1 << oldTog));
				}
			};
			this.confirm.onClick.ResetListener(new Action(this.Submit));
			this.confirm.onClick.AddListener(new UnityAction(this.Hide));
			this.cancel.onClick.ResetListener(new Action(this.Hide));
			this.mask.onClick.ResetListener(new Action(this.Hide));
		}

		// Token: 0x06009B2D RID: 39725 RVA: 0x0048A670 File Offset: 0x00488870
		public void RefreshPanel()
		{
			short itemSubType;
			List<sbyte> soldItemSettingTypeList = SharedMethods.GetBuildingCanSoldItemTypeList(this.parent.BlockData.ConfigData, out itemSubType);
			foreach (ValueTuple<CToggle, int> valueTuple in this.goodsType.GetAll().Select((CToggle tog, int i) => new ValueTuple<CToggle, int>(tog, i)))
			{
				CToggle tog3 = valueTuple.Item1;
				int itemType = valueTuple.Item2;
				tog3.gameObject.SetActive(soldItemSettingTypeList.Contains((sbyte)itemType));
				string text = (itemSubType == -1) ? LocalStringManager.Get(string.Format("LK_ItemType_{0}", itemType)) : LocalStringManager.Get(string.Format("LK_ItemSubType_{0}", itemSubType));
				tog3.GetComponentInChildren<TMP_Text>().text = text;
				TooltipInvoker tips = tog3.GetComponent<TooltipInvoker>();
				tips.Type = TipType.SingleDesc;
				tips.IsLanguageKey = false;
				tips.PresetParam[0] = LanguageKey.LK_Building_SoldSetting_Item_Type_Tip.TrFormat(text);
			}
			this.lowestGrade.Set((int)this._arrangementSetting.MinGrade, false);
			this.highestGrade.Set((int)this._arrangementSetting.MaxGrade, false);
			this.gradeOrder.Set((int)this._arrangementSetting.GradeOrder, false);
			foreach (ValueTuple<CToggleGroupMultiSelect, int, bool> valueTuple2 in this.goodsType.GetAll().Select(delegate(CToggle tog, int i)
			{
				CToggleGroupMultiSelect item = this.goodsType;
				List<sbyte> itemTypeList = this._arrangementSetting.ItemTypeList;
				return new ValueTuple<CToggleGroupMultiSelect, int, bool>(item, i, itemTypeList != null && itemTypeList.Contains((sbyte)i));
			}).Concat(this.goodsProperty.GetAll().Select((CToggle tog, int i) => new ValueTuple<CToggleGroupMultiSelect, int, bool>(this.goodsProperty, i, ((int)this._arrangementSetting.PropertyOrder & 1 << i) != 0))))
			{
				CToggleGroupMultiSelect tog2 = valueTuple2.Item1;
				int j = valueTuple2.Item2;
				bool isOn = valueTuple2.Item3;
				bool flag = isOn;
				if (flag)
				{
					tog2.Select(j, false);
				}
				else
				{
					tog2.DeSelect(j, false);
				}
			}
		}

		// Token: 0x06009B2E RID: 39726 RVA: 0x0048A884 File Offset: 0x00488A84
		public void Refresh()
		{
			this._arrangementSetting = this.CurrArrangementSettingPresetData;
			this.RefreshPanel();
		}

		// Token: 0x06009B2F RID: 39727 RVA: 0x0048A89A File Offset: 0x00488A9A
		public void Show()
		{
			UIManager.Instance.SetEscHandler(new Action(this.Hide));
			this.Refresh();
			base.gameObject.SetActive(true);
		}

		// Token: 0x06009B30 RID: 39728 RVA: 0x0048A8C8 File Offset: 0x00488AC8
		public void Hide()
		{
			bool flag = UIManager.Instance.CheckEscHandler(new Action(this.Hide));
			if (flag)
			{
				UIManager.Instance.SetEscHandler(null);
			}
			base.gameObject.SetActive(false);
		}

		// Token: 0x06009B31 RID: 39729 RVA: 0x0048A909 File Offset: 0x00488B09
		public void Submit()
		{
			this.parent.BuildingModel.SetBuildingSoldItemSetting(this.parent.BlockKey, this._arrangementSetting);
		}

		// Token: 0x06009B32 RID: 39730 RVA: 0x0048A92D File Offset: 0x00488B2D
		public void Confirm()
		{
			this.Submit();
			this.Hide();
		}

		// Token: 0x06009B33 RID: 39731 RVA: 0x0048A93E File Offset: 0x00488B3E
		public void Sync()
		{
			this.Refresh();
			this.Submit();
		}

		// Token: 0x040077FF RID: 30719
		[SerializeField]
		private BuildingManageSubPageProduction parent;

		// Token: 0x04007800 RID: 30720
		[SerializeField]
		private CToggleGroup lowestGrade;

		// Token: 0x04007801 RID: 30721
		[SerializeField]
		private CToggleGroup highestGrade;

		// Token: 0x04007802 RID: 30722
		[SerializeField]
		private CToggleGroup gradeOrder;

		// Token: 0x04007803 RID: 30723
		[SerializeField]
		private CToggleGroupMultiSelect goodsType;

		// Token: 0x04007804 RID: 30724
		[SerializeField]
		private CToggleGroupMultiSelect goodsProperty;

		// Token: 0x04007805 RID: 30725
		[SerializeField]
		private CButton confirm;

		// Token: 0x04007806 RID: 30726
		[SerializeField]
		private CButton cancel;

		// Token: 0x04007807 RID: 30727
		[SerializeField]
		private CButton mask;

		// Token: 0x04007808 RID: 30728
		private BuildingOptionAutoAddSoldItemPreset _arrangementSetting;
	}
}
