using System;
using System.Collections.Generic;
using FrameWork;
using FrameWork.UISystem.UIElements;
using GameData.Domains.World;
using TMPro;
using UnityEngine;

namespace Game.Views.LegacyPassing
{
	// Token: 0x02000999 RID: 2457
	public class ViewSelectLegacyRewardGroup : UIBase
	{
		// Token: 0x17000D4E RID: 3406
		// (get) Token: 0x0600765C RID: 30300 RVA: 0x00372FCD File Offset: 0x003711CD
		private bool HasEnoughLegacyPoint
		{
			get
			{
				return this._legacyPoint >= this._cost;
			}
		}

		// Token: 0x0600765D RID: 30301 RVA: 0x00372FE0 File Offset: 0x003711E0
		private void Awake()
		{
			this.confirm.onClick.ResetListener(new Action(this.OnConfirmSelect));
			this.cancel.onClick.ResetListener(new Action(this.QuickHide));
			this.closeButton.onClick.ResetListener(new Action(this.QuickHide));
			this.toggleGroup.Init(-1);
			this.toggleGroup.OnActiveIndexChange += this.OnActiveToggleChanged;
		}

		// Token: 0x0600765E RID: 30302 RVA: 0x0037306C File Offset: 0x0037126C
		public override void OnInit(ArgumentBox argsBox)
		{
			argsBox.Get<WorldCreationInfo>("WorldCreationInfo", out this._worldCreationInfo);
			argsBox.Get("Cost", out this._cost);
			argsBox.Get("LegacyPoint", out this._legacyPoint);
			argsBox.Get<Action<short>>("OnSelectLegacy", out this._onSelectReward);
			argsBox.Get<Action>("OnCreateRandomLegacy", out this._onCreateRandomLegacy);
			List<CToggle> toggles = this.toggleGroup.GetAll();
			sbyte i = 0;
			while ((int)i < toggles.Count)
			{
				toggles[(int)i].GetComponent<LegacyGroup>().Set(this._worldCreationInfo.GetGroupLevel(i));
				i += 1;
			}
			this.UpdateCost();
			this.UpdateConfirmButton();
		}

		// Token: 0x0600765F RID: 30303 RVA: 0x00373124 File Offset: 0x00371324
		private void UpdateCost()
		{
			string value = this._legacyPoint.ToString().SetColor(this.HasEnoughLegacyPoint ? "brightblue" : "brightred");
			this.costValue.SetText(value ?? "", true);
			this.costValueNeed.SetText(string.Format("/{0}", this._cost), true);
		}

		// Token: 0x06007660 RID: 30304 RVA: 0x00373190 File Offset: 0x00371390
		private void UpdateConfirmButton()
		{
			this.tips.SetActive(!this.toggleGroup.AnyTogglesOn());
			this.disableMouseTip.enabled = !(this.confirm.interactable = (this.toggleGroup.AnyTogglesOn() && this.HasEnoughLegacyPoint));
			bool flag = !this.HasEnoughLegacyPoint;
			if (flag)
			{
				this.disableMouseTip.PresetParam[0] = LanguageKey.LK_Legacy_NotEnoughLegacyPoints.Tr();
			}
			else
			{
				bool flag2 = !this.toggleGroup.AnyTogglesOn();
				if (flag2)
				{
					this.disableMouseTip.PresetParam[0] = LanguageKey.LK_Legacy_NeedSelectRandomBonus.Tr();
				}
			}
		}

		// Token: 0x06007661 RID: 30305 RVA: 0x0037323C File Offset: 0x0037143C
		private void OnConfirmSelect()
		{
			this._onCreateRandomLegacy();
			this.QuickHide();
			ArgumentBox argBox = EasyPool.Get<ArgumentBox>().Set("GroupId", this.toggleGroup.GetActiveIndex()).SetObject("OnSelectLegacy", this._onSelectReward);
			UIElement.SelectRandomLegacyReward.SetOnInitArgs(argBox);
			UIManager.Instance.MaskUI(UIElement.SelectRandomLegacyReward);
		}

		// Token: 0x06007662 RID: 30306 RVA: 0x003732A4 File Offset: 0x003714A4
		private void OnActiveToggleChanged(int curr, int prev)
		{
			this.UpdateCost();
			this.UpdateConfirmButton();
		}

		// Token: 0x0400593F RID: 22847
		[SerializeField]
		private TooltipInvoker disableMouseTip;

		// Token: 0x04005940 RID: 22848
		[SerializeField]
		private TMP_Text costValue;

		// Token: 0x04005941 RID: 22849
		[SerializeField]
		private TMP_Text costValueNeed;

		// Token: 0x04005942 RID: 22850
		[SerializeField]
		private CToggleGroup toggleGroup;

		// Token: 0x04005943 RID: 22851
		[SerializeField]
		private CButton confirm;

		// Token: 0x04005944 RID: 22852
		[SerializeField]
		private CButton cancel;

		// Token: 0x04005945 RID: 22853
		[SerializeField]
		private CButton closeButton;

		// Token: 0x04005946 RID: 22854
		[SerializeField]
		private GameObject tips;

		// Token: 0x04005947 RID: 22855
		private WorldCreationInfo _worldCreationInfo;

		// Token: 0x04005948 RID: 22856
		private int _legacyPoint;

		// Token: 0x04005949 RID: 22857
		private Action<short> _onSelectReward;

		// Token: 0x0400594A RID: 22858
		private Action _onCreateRandomLegacy;

		// Token: 0x0400594B RID: 22859
		private int _cost;
	}
}
