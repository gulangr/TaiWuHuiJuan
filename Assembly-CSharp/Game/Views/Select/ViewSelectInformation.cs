using System;
using System.Collections.Generic;
using Config;
using FrameWork;
using FrameWork.UISystem.Components;
using FrameWork.UISystem.UIElements;
using Game.Components.Information;
using Game.Components.SortAndFilter;
using Game.Components.SortAndFilter.Information;
using GameData.Domains.Information;
using GameData.Serializer;
using GameData.Utilities;
using UnityEngine;

namespace Game.Views.Select
{
	// Token: 0x020007AE RID: 1966
	public class ViewSelectInformation : UIBase
	{
		// Token: 0x06005F7A RID: 24442 RVA: 0x002BC6AC File Offset: 0x002BA8AC
		public override void OnInit(ArgumentBox argsBox)
		{
			bool flag = !argsBox.Get("SelectInformationType", out this._selectInformationType);
			if (flag)
			{
				this._selectInformationType = -1;
			}
			bool flag2 = !argsBox.Get("CharId", out this._charId);
			if (flag2)
			{
				this._charId = SingletonObject.getInstance<BasicGameData>().TaiwuCharId;
			}
			object callback;
			bool flag3 = argsBox.Get<object>("callback", out callback);
			if (flag3)
			{
				this._onNormalInformationConfirm = (callback as Action<NormalInformation>);
			}
			else
			{
				this._onNormalInformationConfirm = null;
			}
			bool flag4 = !argsBox.Get("IsLiteratiSkill3", out this._isLiteratiSkill3);
			if (flag4)
			{
				this._isLiteratiSkill3 = false;
			}
		}

		// Token: 0x06005F7B RID: 24443 RVA: 0x002BC748 File Offset: 0x002BA948
		public override void QuickHide()
		{
			Action<NormalInformation> onNormalInformationConfirm = this._onNormalInformationConfirm;
			if (onNormalInformationConfirm != null)
			{
				onNormalInformationConfirm(new NormalInformation(-1, -1));
			}
			base.QuickHide();
		}

		// Token: 0x06005F7C RID: 24444 RVA: 0x002BC76C File Offset: 0x002BA96C
		private void Awake()
		{
			this.toggleGroup.Init(new Action(this.OnSkillListChanged));
			this._sortAndFilterController = new InformationSortAndFilterController(this.sortAndFilter, false);
			this._sortAndFilterController.Init(new Action(this.OnSkillListChanged), "InformationSortAndFilter");
			this.scroll.OnItemRender += this.OnItemRender;
			this.btnClose.ClearAndAddListener(new Action(this.QuickHide));
			this.btnConfirm.ClearAndAddListener(new Action(this.OnConfirm));
		}

		// Token: 0x06005F7D RID: 24445 RVA: 0x002BC80A File Offset: 0x002BAA0A
		private void OnEnable()
		{
			this.RequestData();
		}

		// Token: 0x06005F7E RID: 24446 RVA: 0x002BC814 File Offset: 0x002BAA14
		private void RequestData()
		{
			InformationDomainMethod.AsyncCall.GetCharacterNormalInformation(this, this._charId, delegate(int offset, RawDataPool pool)
			{
				Serializer.Deserialize(pool, offset, ref this._normalInformationCollection);
				this.RefreshToggleGroupLabel();
				this.OnSkillListChanged();
			});
		}

		// Token: 0x06005F7F RID: 24447 RVA: 0x002BC830 File Offset: 0x002BAA30
		private void RefreshToggleGroupLabel()
		{
			foreach (InformationTypeItem item in ((IEnumerable<InformationTypeItem>)InformationType.Instance))
			{
				this._dataCount[item.TemplateId] = 0;
			}
			foreach (NormalInformation info in this._normalInformationCollection.GetList())
			{
				Dictionary<sbyte, int> dataCount = this._dataCount;
				sbyte type = Information.Instance[info.TemplateId].Type;
				int num = dataCount[type];
				dataCount[type] = num + 1;
			}
			foreach (InformationTypeItem item2 in ((IEnumerable<InformationTypeItem>)InformationType.Instance))
			{
				this.toggleGroup.Set(item2.TemplateId, this._dataCount[item2.TemplateId]);
			}
			bool flag = this._selectInformationType >= 0;
			if (flag)
			{
				foreach (InformationTypeItem config in ((IEnumerable<InformationTypeItem>)InformationType.Instance))
				{
					this.toggleGroup.SetVisible(config.TemplateId, config.TemplateId == this._selectInformationType);
				}
			}
			else
			{
				bool isLiteratiSkill = this._isLiteratiSkill3;
				if (isLiteratiSkill)
				{
					foreach (InformationTypeItem config2 in ((IEnumerable<InformationTypeItem>)InformationType.Instance))
					{
						this.toggleGroup.SetVisible(config2.TemplateId, config2.TemplateId != 6 && config2.TemplateId != 5);
					}
				}
				else
				{
					foreach (InformationTypeItem config3 in ((IEnumerable<InformationTypeItem>)InformationType.Instance))
					{
						this.toggleGroup.SetVisible(config3.TemplateId, true);
					}
				}
			}
		}

		// Token: 0x06005F80 RID: 24448 RVA: 0x002BCAA0 File Offset: 0x002BACA0
		private void OnSkillListChanged()
		{
			this._filteredData.Clear();
			this._currIndex = -1;
			this.btnConfirm.interactable = false;
			int type = this.toggleGroup.Get();
			this._sortAndFilterController.SetVisibleDropdownMenus(0, new int[]
			{
				type
			});
			Func<InformationSortAndFilterData, bool> filter = this._sortAndFilterController.GenerateFilter();
			Comparison<InformationSortAndFilterData> comparer = this._sortAndFilterController.GenerateComparer(this._filteredData);
			List<InformationSortAndFilterData> allData = new List<InformationSortAndFilterData>();
			foreach (NormalInformation info in this._normalInformationCollection.GetList())
			{
				InformationItem config = Information.Instance[info.TemplateId];
				bool flag = this._isLiteratiSkill3 && !this.CheckLiteratiSkillNormalInformationUsable(info);
				if (!flag)
				{
					bool flag2 = (int)config.Type == type;
					if (flag2)
					{
						InformationSortAndFilterData item = new InformationSortAndFilterData
						{
							TemplateId = info.TemplateId,
							Level = info.Level,
							UsedCount = this._normalInformationCollection.GetUsedCount(info),
							UsedCountMax = this._normalInformationCollection.GetUsedCountMax(info)
						};
						allData.Add(item);
						bool flag3 = filter(item);
						if (flag3)
						{
							this._filteredData.Add(item);
						}
					}
				}
			}
			bool flag4 = comparer != null;
			if (flag4)
			{
				this._filteredData.Sort(comparer);
			}
			this._sortAndFilterController.AfterFilter(allData);
			this.scroll.SetDataCount(this._filteredData.Count);
			this.Element.ShowAfterRefresh();
		}

		// Token: 0x06005F81 RID: 24449 RVA: 0x002BCC5C File Offset: 0x002BAE5C
		private void OnItemRender(int index, GameObject obj)
		{
			obj.GetComponent<InformationCardItem>().Set(this._filteredData[index], true);
			obj.GetComponent<CToggle>().SetIsOnWithoutNotify(this._currIndex == index);
			obj.GetComponent<CToggle>().onValueChanged.ResetListener(delegate(bool value)
			{
				bool flag = this._currIndex >= 0;
				if (flag)
				{
					GameObject cell = this.scroll.GetActiveCell(this._currIndex);
					bool flag2 = cell;
					if (flag2)
					{
						cell.GetComponent<CToggle>().SetIsOnWithoutNotify(false);
					}
				}
				this._currIndex = (value ? index : -1);
				this.btnConfirm.interactable = (this._currIndex >= 0);
			});
		}

		// Token: 0x06005F82 RID: 24450 RVA: 0x002BCCD4 File Offset: 0x002BAED4
		private void OnConfirm()
		{
			Action<NormalInformation> onNormalInformationConfirm = this._onNormalInformationConfirm;
			if (onNormalInformationConfirm != null)
			{
				onNormalInformationConfirm(new NormalInformation(this._filteredData[this._currIndex].TemplateId, this._filteredData[this._currIndex].Level));
			}
			this.QuickHide();
		}

		// Token: 0x06005F83 RID: 24451 RVA: 0x002BCD2C File Offset: 0x002BAF2C
		private bool CheckLiteratiSkillNormalInformationUsable(NormalInformation normalInformation)
		{
			InformationItem config = Information.Instance.GetItem(normalInformation.TemplateId);
			InformationInfoItem info = InformationInfo.Instance.GetItem(config.InfoIds[(int)normalInformation.Level]);
			sbyte b = config.Type;
			bool result;
			if ((b == 0 || b == 1 || b == 2 || b == 3) && config.IsGeneral)
			{
				b = info.LifeSkillType;
				result = (b != 12 && b != 13);
			}
			else
			{
				result = false;
			}
			return result;
		}

		// Token: 0x06005F84 RID: 24452 RVA: 0x002BCDA4 File Offset: 0x002BAFA4
		private void Update()
		{
			bool flag = CommonCommandKit.Space.Check(this.Element, false, false, false, true, false) && this.btnConfirm.interactable;
			if (flag)
			{
				this.OnConfirm();
			}
		}

		// Token: 0x04004213 RID: 16915
		[SerializeField]
		private InformationTypeToggleGroup toggleGroup;

		// Token: 0x04004214 RID: 16916
		[SerializeField]
		private SortAndFilter sortAndFilter;

		// Token: 0x04004215 RID: 16917
		[SerializeField]
		private InfinityScroll scroll;

		// Token: 0x04004216 RID: 16918
		[SerializeField]
		private CButton btnClose;

		// Token: 0x04004217 RID: 16919
		[SerializeField]
		private CButton btnConfirm;

		// Token: 0x04004218 RID: 16920
		private NormalInformationCollection _normalInformationCollection;

		// Token: 0x04004219 RID: 16921
		private InformationSortAndFilterController _sortAndFilterController;

		// Token: 0x0400421A RID: 16922
		private List<InformationSortAndFilterData> _filteredData = new List<InformationSortAndFilterData>();

		// Token: 0x0400421B RID: 16923
		private Dictionary<sbyte, int> _dataCount = new Dictionary<sbyte, int>();

		// Token: 0x0400421C RID: 16924
		private int _charId;

		// Token: 0x0400421D RID: 16925
		private bool _isLiteratiSkill3;

		// Token: 0x0400421E RID: 16926
		private sbyte _selectInformationType;

		// Token: 0x0400421F RID: 16927
		private Action<NormalInformation> _onNormalInformationConfirm;

		// Token: 0x04004220 RID: 16928
		private int _currIndex;
	}
}
