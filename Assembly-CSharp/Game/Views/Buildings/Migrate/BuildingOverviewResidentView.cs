using System;
using System.Collections.Generic;
using FrameWork;
using FrameWork.UISystem.UIElements;
using Game.Components.Avatar;
using GameData.Domains.Building;
using TMPro;
using UnityEngine;

namespace Game.Views.Buildings.Migrate
{
	// Token: 0x02000BCD RID: 3021
	[Obsolete]
	public class BuildingOverviewResidentView : MonoBehaviour
	{
		// Token: 0x17001050 RID: 4176
		// (get) Token: 0x06009840 RID: 38976 RVA: 0x0046F42C File Offset: 0x0046D62C
		private BuildingModel _buildingModel
		{
			get
			{
				return SingletonObject.getInstance<BuildingModel>();
			}
		}

		// Token: 0x06009841 RID: 38977 RVA: 0x0046F434 File Offset: 0x0046D634
		public void RenderShopCharInfo(int charId)
		{
			this._charId = charId;
			this.showMainCharacterMenu.ClearAndAddListener(delegate
			{
				this.ShowCharacterMenu(charId);
			});
			this.SetLockToggleState(charId);
			this.happiness.CharacterId = charId;
			this.favor.CharacterId = charId;
			this.gender.CharacterId = charId;
			this.health.CharacterId = charId;
			this.changeButton.gameObject.SetActive(false);
			this.selectCharBack.SetActive(charId == -1);
			this.charInfoHolder.SetActive(charId != -1);
			this.functionHolder.SetActive(charId != -1);
			this.InitMouseTipDisplayer();
		}

		// Token: 0x06009842 RID: 38978 RVA: 0x0046F530 File Offset: 0x0046D730
		private void InitMouseTipDisplayer()
		{
			TooltipInvoker mouseTipDisplayer = base.GetComponent<TooltipInvoker>();
			bool flag = mouseTipDisplayer == null;
			if (!flag)
			{
				mouseTipDisplayer.enabled = (this._charId >= 0);
				bool flag2 = this._charId < 0;
				if (!flag2)
				{
					mouseTipDisplayer.Type = TipType.CharacterOnMapBlock;
					TooltipInvoker tooltipInvoker = mouseTipDisplayer;
					if (tooltipInvoker.RuntimeParam == null)
					{
						tooltipInvoker.RuntimeParam = new ArgumentBox();
					}
					mouseTipDisplayer.RuntimeParam.Set("CharId", this._charId);
				}
			}
		}

		// Token: 0x06009843 RID: 38979 RVA: 0x0046F5AC File Offset: 0x0046D7AC
		private void SetLockToggleState(int charId)
		{
			bool flag = charId == -1;
			if (!flag)
			{
				this.lockToggle.onValueChanged.RemoveAllListeners();
				this.lockToggle.isOn = (!BuildingOverviewResidentView._unlockedWorkingList.Contains(charId) || !this._buildingModel.VillagerWork.ContainsKey(charId));
				bool flag2 = !this._buildingModel.VillagerWork.ContainsKey(charId);
				if (flag2)
				{
					BuildingDomainMethod.Call.SetUnlockedWorkingVillagers(charId, false);
				}
				this.lockToggle.onValueChanged.AddListener(delegate(bool isOn)
				{
					this.OnLockToggleChange(isOn, charId);
				});
			}
		}

		// Token: 0x06009844 RID: 38980 RVA: 0x0046F674 File Offset: 0x0046D874
		private void OnLockToggleChange(bool isOn, int charId)
		{
			BuildingDomainMethod.Call.SetUnlockedWorkingVillagers(charId, !isOn);
		}

		// Token: 0x06009845 RID: 38981 RVA: 0x0046F684 File Offset: 0x0046D884
		private void ShowCharacterMenu(int charId)
		{
			ArgumentBox argBox = EasyPool.Get<ArgumentBox>();
			argBox.Set("CharacterId", charId);
			UIElement.CharacterMenu.SetOnInitArgs(argBox);
			UIManager.Instance.ShowUI(UIElement.CharacterMenu, true);
		}

		// Token: 0x04007502 RID: 29954
		public GameObject selectCharBack;

		// Token: 0x04007503 RID: 29955
		public CButton showMainCharacterMenu;

		// Token: 0x04007504 RID: 29956
		public GameObject charInfoHolder;

		// Token: 0x04007505 RID: 29957
		public CButton selectCharBtn;

		// Token: 0x04007506 RID: 29958
		public TextMeshProUGUI nameText;

		// Token: 0x04007507 RID: 29959
		public BuildingOverviewCharacterHappiness happiness;

		// Token: 0x04007508 RID: 29960
		public BuildingOverviewCharacterFavorability favor;

		// Token: 0x04007509 RID: 29961
		public BuildingOverviewCharacterGender gender;

		// Token: 0x0400750A RID: 29962
		public BuildingOverviewCharacterHealth health;

		// Token: 0x0400750B RID: 29963
		public TextMeshProUGUI personalityCount;

		// Token: 0x0400750C RID: 29964
		public TextMeshProUGUI workTypeText;

		// Token: 0x0400750D RID: 29965
		public GameObject functionHolder;

		// Token: 0x0400750E RID: 29966
		public CToggle lockToggle;

		// Token: 0x0400750F RID: 29967
		public Game.Components.Avatar.Avatar avatar;

		// Token: 0x04007510 RID: 29968
		public CButton changeButton;

		// Token: 0x04007511 RID: 29969
		private static List<int> _unlockedWorkingList = new List<int>();

		// Token: 0x04007512 RID: 29970
		private int _charId = -1;
	}
}
