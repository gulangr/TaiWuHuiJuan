using System;
using System.Linq;
using FrameWork.UISystem.UIElements;
using GameData.Domains.Building;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace Game.Views.Building.BuildingManage
{
	// Token: 0x02000BED RID: 3053
	public class ArrangementSettingPanel : MonoBehaviour
	{
		// Token: 0x17001064 RID: 4196
		// (get) Token: 0x06009B13 RID: 39699 RVA: 0x00489D74 File Offset: 0x00487F74
		private BuildingOptionAutoGiveMemberPreset CurrArrangementSettingPresetData
		{
			get
			{
				return this.parent.BuildingModel.GetBuildingArrangementSetting(this.parent.BlockKey, this.parent.BlockData);
			}
		}

		// Token: 0x06009B14 RID: 39700 RVA: 0x00489D9C File Offset: 0x00487F9C
		private void Awake()
		{
			this.assignLeader.onValueChanged.ResetListener(delegate(bool isOn)
			{
				this._arrangementSetting.SetIsInfluenceLeader(isOn);
				this.leaderPickUp.SetInteractable(isOn);
				this.leaderRule.SetInteractable(isOn);
				this.leaderLock.interactable = isOn;
				this.leaderStyle.SetStyleEffect(!isOn, false);
			});
			this.assignMember.onValueChanged.ResetListener(delegate(bool isOn)
			{
				this._arrangementSetting.SetIsInfluenceMember(isOn);
				this.memberPickUp.SetInteractable(isOn);
				this.memberCount.SetInteractable(isOn);
				this.memberRule.SetInteractable(isOn);
				this.memberLock.interactable = isOn;
				this.memberStyle.SetStyleEffect(!isOn, false);
			});
			this.leaderPickUp.Init(-1);
			this.memberPickUp.Init(-1);
			this.leaderPickUp.OnActiveIndexChange += delegate(int newTog, int oldTog)
			{
				this._arrangementSetting.PickRuleForLeader = (sbyte)newTog;
			};
			this.memberPickUp.OnActiveIndexChange += delegate(int newTog, int oldTog)
			{
				this._arrangementSetting.PickRuleForMember = (sbyte)newTog;
			};
			this.leaderLock.onValueChanged.ResetListener(delegate(bool isOn)
			{
				this._arrangementSetting.LockCharForLeader = isOn;
			});
			this.memberLock.onValueChanged.ResetListener(delegate(bool isOn)
			{
				this._arrangementSetting.LockCharForMember = isOn;
			});
			this.memberCount.Init(-1);
			this.memberCount.OnActiveIndexChange += delegate(int newTog, int oldTog)
			{
				this._arrangementSetting.Amount = (int)((sbyte)(newTog + 1));
			};
			this.leaderRule.Init();
			this.memberRule.Init();
			this.leaderRule.OnActiveIndexChange += delegate(int newTog, int oldTog)
			{
				bool flag = newTog != -1;
				if (flag)
				{
					BuildingOptionAutoGiveMemberPreset arrangementSetting = this._arrangementSetting;
					arrangementSetting.RoleRuleForLeader |= (sbyte)(1 << newTog);
				}
				bool flag2 = oldTog != -1;
				if (flag2)
				{
					BuildingOptionAutoGiveMemberPreset arrangementSetting2 = this._arrangementSetting;
					arrangementSetting2.RoleRuleForLeader &= (sbyte)(~(sbyte)(1 << oldTog));
				}
			};
			this.memberRule.OnActiveIndexChange += delegate(int newTog, int oldTog)
			{
				bool flag = newTog != -1;
				if (flag)
				{
					BuildingOptionAutoGiveMemberPreset arrangementSetting = this._arrangementSetting;
					arrangementSetting.RoleRuleForMember |= (sbyte)(1 << newTog);
				}
				bool flag2 = oldTog != -1;
				if (flag2)
				{
					BuildingOptionAutoGiveMemberPreset arrangementSetting2 = this._arrangementSetting;
					arrangementSetting2.RoleRuleForMember &= (sbyte)(~(sbyte)(1 << oldTog));
				}
			};
			this.confirm.onClick.ResetListener(new Action(this.Submit));
			this.confirm.onClick.AddListener(new UnityAction(this.Hide));
			this.cancel.onClick.ResetListener(new Action(this.Hide));
			this.mask.onClick.ResetListener(new Action(this.Hide));
			this.sync.onClick.ResetListener(new Action(this.Sync));
			this.current.onValueChanged.ResetListener(delegate(bool isOn)
			{
				this.confirm.gameObject.SetActive(isOn);
				this.cancel.gameObject.SetActive(isOn);
				this.sync.gameObject.SetActive(!isOn);
				if (isOn)
				{
					this.presetOrCurrent.DeSelect(false);
				}
			});
			this.presetOrCurrent.Init(-1);
			this.presetOrCurrent.OnActiveIndexChange += delegate(int newTog, int _)
			{
				this.SavePreset();
				this._arrangementSettingIndex = newTog;
				Toggle toggle = this.current;
				int arrangementSettingIndex = this._arrangementSettingIndex;
				toggle.isOn = (arrangementSettingIndex < 0 || arrangementSettingIndex > 2);
				this.Refresh();
			};
		}

		// Token: 0x06009B15 RID: 39701 RVA: 0x00489FA8 File Offset: 0x004881A8
		public void RefreshPanel()
		{
			this.assignLeader.isOn = this._arrangementSetting.GetIsInfluenceLeader();
			this.assignMember.isOn = this._arrangementSetting.GetIsInfluenceMember();
			this.leaderPickUp.Set((int)this._arrangementSetting.PickRuleForLeader, false);
			this.memberPickUp.Set((int)this._arrangementSetting.PickRuleForMember, false);
			this.leaderLock.isOn = this._arrangementSetting.LockCharForLeader;
			this.memberLock.isOn = this._arrangementSetting.LockCharForMember;
			foreach (ValueTuple<CToggleGroupMultiSelect, int, bool> valueTuple in this.leaderRule.GetAll().Select((CToggle tog, int i) => new ValueTuple<CToggleGroupMultiSelect, int, bool>(this.leaderRule, i, ((int)this._arrangementSetting.RoleRuleForLeader & 1 << i) != 0)).Concat(this.memberRule.GetAll().Select((CToggle tog, int i) => new ValueTuple<CToggleGroupMultiSelect, int, bool>(this.memberRule, i, ((int)this._arrangementSetting.RoleRuleForMember & 1 << i) != 0))))
			{
				CToggleGroupMultiSelect tog2 = valueTuple.Item1;
				int j = valueTuple.Item2;
				bool isOn = valueTuple.Item3;
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
			this.memberCount.Set(this._arrangementSetting.Amount - 1, false);
		}

		// Token: 0x06009B16 RID: 39702 RVA: 0x0048A0FC File Offset: 0x004882FC
		public void Refresh()
		{
			int arrangementSettingIndex = this._arrangementSettingIndex;
			this._arrangementSetting = new BuildingOptionAutoGiveMemberPreset((arrangementSettingIndex >= 0 && arrangementSettingIndex <= 2) ? this.parent.BuildingModel.GetBuildingArrangementSettingPresetData(this._arrangementSettingIndex) : this.CurrArrangementSettingPresetData);
			this.RefreshPanel();
		}

		// Token: 0x06009B17 RID: 39703 RVA: 0x0048A149 File Offset: 0x00488349
		public void Show()
		{
			UIManager.Instance.SetEscHandler(new Action(this.Hide));
			this.current.isOn = true;
			this.Refresh();
			base.gameObject.SetActive(true);
		}

		// Token: 0x06009B18 RID: 39704 RVA: 0x0048A184 File Offset: 0x00488384
		public void Hide()
		{
			bool flag = UIManager.Instance.CheckEscHandler(new Action(this.Hide));
			if (flag)
			{
				UIManager.Instance.SetEscHandler(null);
			}
			this.SavePreset();
			base.gameObject.SetActive(false);
		}

		// Token: 0x06009B19 RID: 39705 RVA: 0x0048A1CC File Offset: 0x004883CC
		private void SavePreset()
		{
			int arrangementSettingIndex = this._arrangementSettingIndex;
			bool flag = arrangementSettingIndex >= 0 && arrangementSettingIndex <= 2;
			if (flag)
			{
				this.parent.BuildingModel.SetBuildingArrangementSettingPresetData(this._arrangementSettingIndex, this._arrangementSetting);
			}
		}

		// Token: 0x06009B1A RID: 39706 RVA: 0x0048A210 File Offset: 0x00488410
		public void Submit()
		{
			this.parent.BuildingModel.SetBuildingArrangementSetting(this.parent.BlockKey, this._arrangementSetting);
		}

		// Token: 0x06009B1B RID: 39707 RVA: 0x0048A234 File Offset: 0x00488434
		public void Confirm()
		{
			this.Submit();
			this.Hide();
		}

		// Token: 0x06009B1C RID: 39708 RVA: 0x0048A245 File Offset: 0x00488445
		public void Sync()
		{
			this.SavePreset();
			this.Confirm();
		}

		// Token: 0x040077EB RID: 30699
		[SerializeField]
		private BuildingManageSubPageShop parent;

		// Token: 0x040077EC RID: 30700
		[SerializeField]
		private CToggleGroup leaderPickUp;

		// Token: 0x040077ED RID: 30701
		[SerializeField]
		private CToggleGroup memberPickUp;

		// Token: 0x040077EE RID: 30702
		[SerializeField]
		private CToggleGroup memberCount;

		// Token: 0x040077EF RID: 30703
		[SerializeField]
		private CToggleGroup presetOrCurrent;

		// Token: 0x040077F0 RID: 30704
		[SerializeField]
		private CToggleGroupMultiSelect leaderRule;

		// Token: 0x040077F1 RID: 30705
		[SerializeField]
		private CToggleGroupMultiSelect memberRule;

		// Token: 0x040077F2 RID: 30706
		[SerializeField]
		private CButton confirm;

		// Token: 0x040077F3 RID: 30707
		[SerializeField]
		private CButton cancel;

		// Token: 0x040077F4 RID: 30708
		[SerializeField]
		private CButton mask;

		// Token: 0x040077F5 RID: 30709
		[SerializeField]
		private CButton sync;

		// Token: 0x040077F6 RID: 30710
		[SerializeField]
		private CToggle current;

		// Token: 0x040077F7 RID: 30711
		[SerializeField]
		private CToggle assignLeader;

		// Token: 0x040077F8 RID: 30712
		[SerializeField]
		private CToggle assignMember;

		// Token: 0x040077F9 RID: 30713
		[SerializeField]
		private CToggle leaderLock;

		// Token: 0x040077FA RID: 30714
		[SerializeField]
		private CToggle memberLock;

		// Token: 0x040077FB RID: 30715
		[SerializeField]
		private DisableStyleRoot leaderStyle;

		// Token: 0x040077FC RID: 30716
		[SerializeField]
		private DisableStyleRoot memberStyle;

		// Token: 0x040077FD RID: 30717
		private int _arrangementSettingIndex = -1;

		// Token: 0x040077FE RID: 30718
		private BuildingOptionAutoGiveMemberPreset _arrangementSetting;
	}
}
