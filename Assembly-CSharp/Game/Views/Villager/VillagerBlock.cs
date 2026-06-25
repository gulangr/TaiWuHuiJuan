using System;
using System.Collections.Generic;
using System.Linq;
using FrameWork;
using FrameWork.UISystem.UIElements;
using Game.Components.Avatar;
using Game.Views.Bottom;
using Game.Views.Following;
using GameData.Domains.Building;
using GameData.Domains.Character.Display;
using GameData.Domains.Map;
using GameData.Domains.Merchant;
using GameData.Domains.Taiwu;
using GameData.Domains.Taiwu.Display;
using GameData.Utilities;
using TMPro;
using UnityEngine;

namespace Game.Views.Villager
{
	// Token: 0x02000741 RID: 1857
	public class VillagerBlock : MonoBehaviour
	{
		// Token: 0x06005A17 RID: 23063 RVA: 0x0029CDD8 File Offset: 0x0029AFD8
		private void Awake()
		{
			this.actionToggleGroup.Init(-1);
			this.actionToggleGroup.OnActiveIndexChange += delegate(int newTog, int _)
			{
				bool flag = this._charData == null;
				if (!flag)
				{
					bool flag2 = newTog == 0;
					if (flag2)
					{
						TaiwuDomainMethod.AsyncCall.SetVillagerIdleWork(this._parent, this._charData.Id, this._charLocation.AreaId, this._charLocation.BlockId, delegate(int _, RawDataPool _)
						{
							this._parent.RequestData(null);
						});
					}
					else
					{
						BlockButton.SelectGraveImpl(this._parent, this._charData.Id, this._charLocation, this._mapBlockData.BlockData.GraveSet ?? new HashSet<int>(), delegate(int _, RawDataPool _)
						{
							this._parent.RequestData(null);
						});
					}
				}
			};
			this.mapBlockButton.onClick.ResetListener(new Action(this.JumpToLocation));
			this.lockButton.onValueChanged.ResetListener(new Action<bool>(this.LockImpl));
			this.unassignButton.onClick.ResetListener(new Action(this.UnassignImpl));
			this.deleteButton.onClick.ResetListener(new Action(this.DeleteImpl));
			this.assignBtn.onClick.ResetListener(new Action(this.AssignImpl));
			this.infoButton.onClick.ResetListener(new Action(this.InfoImpl));
			this.changeBtn.onClick.ResetListener(new Action(this.ChangeImpl));
		}

		// Token: 0x06005A18 RID: 23064 RVA: 0x0029CED8 File Offset: 0x0029B0D8
		public void Refresh(IRequestData parent, CharacterLocationDisplayData mapBlockData, VillagerRoleCharacterDisplayData charData = null)
		{
			this._parent = parent;
			this._charLocation = mapBlockData.Location;
			Game.Views.Following.CharacterLocationItem characterLocationItem = this.locationItem;
			this._mapBlockData = mapBlockData;
			characterLocationItem.RefreshImpl(mapBlockData, (string x) => x);
			this.buttonBg.sprite = ((charData == null) ? this.emptySp : this.baseSp);
			WorldMapModel worldMapModel = SingletonObject.getInstance<WorldMapModel>();
			MapBlockData blockData = mapBlockData.BlockData;
			bool flag = blockData != null;
			if (flag)
			{
				this.blockTip.enabled = true;
				this.blockTip.Type = TipType.MapBlock;
				TooltipInvoker tooltipInvoker = this.blockTip;
				ArgumentBox argumentBox;
				if ((argumentBox = tooltipInvoker.RuntimeParam) == null)
				{
					argumentBox = (tooltipInvoker.RuntimeParam = EasyPool.Get<ArgumentBox>());
				}
				argumentBox.SetObject("MapBlockData", blockData).Set("IsUseFullName", true).SetObject("CaravanList", (from d in worldMapModel.CaravanData
				where d.PathInArea.GetCurrLocation() == new Location(blockData.AreaId, blockData.BlockId)
				select d).ToList<CaravanDisplayData>()).SetObject("VillagerWorkData", (charData != null) ? charData.VillagerWorkData : null);
			}
			else
			{
				this.blockTip.enabled = false;
			}
			this._charData = charData;
			bool flag2 = charData != null;
			if (flag2)
			{
				this.avatar.Refresh(charData.Avatar, charData.Name.CharTemplateId);
				this.charName.text = NameCenter.GetMonasticTitleOrDisplayName(ref charData.Name, false, false);
				this.lockButton.SetIsOnWithoutNotify(charData.AssignLocked);
				CToggleGroup ctoggleGroup = this.actionToggleGroup;
				VillagerWorkData villagerWorkData = charData.VillagerWorkData;
				sbyte? b = (villagerWorkData != null) ? new sbyte?(villagerWorkData.WorkType) : null;
				int? num = (b != null) ? new int?((int)b.GetValueOrDefault()) : null;
				int num2 = 12;
				ctoggleGroup.SetWithoutNotify((num.GetValueOrDefault() == num2 & num != null) ? 1 : 0);
				CToggleGroup ctoggleGroup2 = this.actionToggleGroup;
				Behaviour behaviour = this.keepGrave;
				MapBlockData blockData2 = mapBlockData.BlockData;
				HashSet<int> hashSet = (blockData2 != null) ? blockData2.GraveSet : null;
				ctoggleGroup2.SetInteractable(!(behaviour.enabled = (hashSet == null || hashSet.Count <= 0)), 1);
				this.mapBlockButton.interactable = (worldMapModel.GetStateId(this._charLocation.AreaId) == worldMapModel.CurrentStateId);
			}
			foreach (CToggle toggle in this.actionToggleGroup.GetAll())
			{
				toggle.gameObject.SetActive(charData != null);
			}
			this.avatar.gameObject.SetActive(charData != null);
			this.unassignButton.gameObject.SetActive(charData != null);
			this.lockButton.gameObject.SetActive(charData != null);
			this.infoButton.gameObject.SetActive(charData != null);
			this.nameBg.gameObject.SetActive(charData != null);
			this.assignBtn.gameObject.SetActive(charData == null);
		}

		// Token: 0x06005A19 RID: 23065 RVA: 0x0029D230 File Offset: 0x0029B430
		private void JumpToLocation()
		{
			UIManager uiManager = UIManager.Instance;
			uiManager.HideAll();
			uiManager.ChangeToUI(UIElement.StateMainWorld);
			SingletonObject.getInstance<WorldMapModel>().JumpToTemporaryMark(this._charLocation, 0);
		}

		// Token: 0x06005A1A RID: 23066 RVA: 0x0029D269 File Offset: 0x0029B469
		private void LockImpl(bool _)
		{
			BlockButton.LockAssignImpl(this._parent, this._charData.Id, delegate
			{
				this._parent.RequestData(null);
			});
		}

		// Token: 0x06005A1B RID: 23067 RVA: 0x0029D290 File Offset: 0x0029B490
		private void UnassignImpl()
		{
			TaiwuDomainMethod.Call.StopVillagerWorkOptional(-1, this._charLocation.AreaId, this._charLocation.BlockId, 12, true);
			TaiwuDomainMethod.AsyncCall.StopVillagerWorkOptional(this._parent, this._charLocation.AreaId, this._charLocation.BlockId, 13, true, delegate(int _, RawDataPool _)
			{
				this._parent.RequestData(null);
			});
		}

		// Token: 0x06005A1C RID: 23068 RVA: 0x0029D2F0 File Offset: 0x0029B4F0
		private void DeleteImpl()
		{
			TaiwuDomainMethod.Call.StopVillagerWorkOptional(-1, this._charLocation.AreaId, this._charLocation.BlockId, 12, true);
			TaiwuDomainMethod.Call.StopVillagerWorkOptional(-1, this._charLocation.AreaId, this._charLocation.BlockId, 13, true);
			BuildingDomainMethod.AsyncCall.RemoveLocationMark(this._parent, this._charLocation, delegate(int _, RawDataPool _)
			{
				this._parent.RequestData(null);
			});
		}

		// Token: 0x06005A1D RID: 23069 RVA: 0x0029D35C File Offset: 0x0029B55C
		private void AssignImpl()
		{
			BlockButton.QuickAssignImpl(this._parent, delegate(int _, RawDataPool _)
			{
				this._parent.RequestData(null);
			}, this._charLocation, false);
		}

		// Token: 0x06005A1E RID: 23070 RVA: 0x0029D37D File Offset: 0x0029B57D
		private void ChangeImpl()
		{
			IAsyncMethodRequestHandler parent = this._parent;
			VillagerRoleCharacterDisplayData charData = this._charData;
			BlockButton.Template.OpenSelectWindow(parent, (charData != null) ? charData.Id : -1, this._charLocation, delegate
			{
				this._parent.RequestData(null);
			});
		}

		// Token: 0x06005A1F RID: 23071 RVA: 0x0029D3AF File Offset: 0x0029B5AF
		private void InfoImpl()
		{
			UIElement.CharacterMenu.SetOnInitArgs(EasyPool.Get<ArgumentBox>().Set("CharacterId", this._charData.Id));
			UIManager.Instance.ShowUI(UIElement.CharacterMenu, true);
		}

		// Token: 0x04003DF8 RID: 15864
		[SerializeField]
		private Game.Components.Avatar.Avatar avatar;

		// Token: 0x04003DF9 RID: 15865
		[SerializeField]
		private Game.Views.Following.CharacterLocationItem locationItem;

		// Token: 0x04003DFA RID: 15866
		[SerializeField]
		private Sprite baseSp;

		// Token: 0x04003DFB RID: 15867
		[SerializeField]
		private Sprite emptySp;

		// Token: 0x04003DFC RID: 15868
		[SerializeField]
		private CImage buttonBg;

		// Token: 0x04003DFD RID: 15869
		[SerializeField]
		private CImage nameBg;

		// Token: 0x04003DFE RID: 15870
		[SerializeField]
		private CButton mapBlockButton;

		// Token: 0x04003DFF RID: 15871
		[SerializeField]
		private CButton infoButton;

		// Token: 0x04003E00 RID: 15872
		[SerializeField]
		private CButton unassignButton;

		// Token: 0x04003E01 RID: 15873
		[SerializeField]
		private CButton deleteButton;

		// Token: 0x04003E02 RID: 15874
		[SerializeField]
		private CButton assignBtn;

		// Token: 0x04003E03 RID: 15875
		[SerializeField]
		private CButton changeBtn;

		// Token: 0x04003E04 RID: 15876
		[SerializeField]
		private CToggle lockButton;

		// Token: 0x04003E05 RID: 15877
		[SerializeField]
		private CToggleGroup actionToggleGroup;

		// Token: 0x04003E06 RID: 15878
		[SerializeField]
		private TMP_Text charName;

		// Token: 0x04003E07 RID: 15879
		[SerializeField]
		private TooltipInvoker keepGrave;

		// Token: 0x04003E08 RID: 15880
		[SerializeField]
		private TooltipInvoker blockTip;

		// Token: 0x04003E09 RID: 15881
		private IRequestData _parent;

		// Token: 0x04003E0A RID: 15882
		private Location _charLocation;

		// Token: 0x04003E0B RID: 15883
		private CharacterLocationDisplayData _mapBlockData;

		// Token: 0x04003E0C RID: 15884
		private VillagerRoleCharacterDisplayData _charData;
	}
}
