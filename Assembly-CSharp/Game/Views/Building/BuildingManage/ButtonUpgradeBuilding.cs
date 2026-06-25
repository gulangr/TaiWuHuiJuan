using System;
using System.Collections.Generic;
using Config;
using FrameWork;
using FrameWork.UISystem.UIElements;
using TMPro;
using UnityEngine;

namespace Game.Views.Building.BuildingManage
{
	// Token: 0x02000C07 RID: 3079
	public class ButtonUpgradeBuilding : MonoBehaviour
	{
		// Token: 0x06009C77 RID: 40055 RVA: 0x004946B3 File Offset: 0x004928B3
		public void Init(int index, Action<ButtonUpgradeBuilding> onConfirm)
		{
			this.Index = index;
			this._onConfirm = onConfirm;
			base.GetComponent<CButton>().ClearAndAddListener(new Action(this.OnClickButton));
		}

		// Token: 0x06009C78 RID: 40056 RVA: 0x004946DC File Offset: 0x004928DC
		public void Refresh(ResourceInfo cost, int own)
		{
			bool enough = own >= cost.ResourceCount;
			this._type = cost.ResourceType;
			this._own = own;
			this._cost = cost.ResourceCount;
			this.resourceIcon.SetSprite(string.Format("{0}{1}", "ui9_icon_resource_bar_", cost.ResourceType), false, null);
			this.resourceValue.text = CommonUtils.GetDisplayStringForNum(own, 100000).SetColor(enough ? "brightblue" : "brightred") + "/" + CommonUtils.GetDisplayStringForNum(cost.ResourceCount, 100000);
			this.hint.SetActive(!enough);
		}

		// Token: 0x06009C79 RID: 40057 RVA: 0x00494794 File Offset: 0x00492994
		private void OnClickButton()
		{
			ConfirmDialogCmd cmd = new ConfirmDialogCmd
			{
				Title = LanguageKey.LK_ResourceConfirm_Title.Tr(),
				ContentUpper = LanguageKey.LK_Building_UpgradeBuildingSlot_Confirm_Desc1.Tr(),
				ContentLower = LanguageKey.LK_Building_UpgradeBuildingSlot_Confirm_Desc2.Tr(),
				ConfirmDialogCost = new List<ConfirmDialogCost>
				{
					new ConfirmDialogCost
					{
						Type = ViewConfirmDialog.ResourceTypeToEnum[this._type],
						ValueCost = this._cost,
						ValueHave = this._own
					}
				},
				Yes = delegate()
				{
					this._onConfirm(this);
				}
			};
			UIElement.ConfirmDialog.SetOnInitArgs(EasyPool.Get<ArgumentBox>().SetObject("Cmd", cmd));
			UIManager.Instance.MaskUI(UIElement.ConfirmDialog);
		}

		// Token: 0x0400793D RID: 31037
		public GameObject hint;

		// Token: 0x0400793E RID: 31038
		public CImage resourceIcon;

		// Token: 0x0400793F RID: 31039
		public TextMeshProUGUI resourceValue;

		// Token: 0x04007940 RID: 31040
		[NonSerialized]
		public int Index;

		// Token: 0x04007941 RID: 31041
		private sbyte _type;

		// Token: 0x04007942 RID: 31042
		private int _own;

		// Token: 0x04007943 RID: 31043
		private int _cost;

		// Token: 0x04007944 RID: 31044
		private Action<ButtonUpgradeBuilding> _onConfirm;
	}
}
