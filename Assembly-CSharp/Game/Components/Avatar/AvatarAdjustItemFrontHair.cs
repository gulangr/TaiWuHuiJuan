using System;
using System.Collections.Generic;
using GameData.Domains.Character.AvatarSystem.AvatarRes;
using TMPro;
using UnityEngine.Events;

namespace Game.Components.Avatar
{
	// Token: 0x02000F7B RID: 3963
	public class AvatarAdjustItemFrontHair : AvatarAdjustItemBase
	{
		// Token: 0x0600B5F8 RID: 46584 RVA: 0x0052E2D4 File Offset: 0x0052C4D4
		protected override void Start()
		{
			this.OnlyCreateRes = true;
			this.UpdateAssetCore();
			IdSwitch idSwitch = this.Refers.CGet<IdSwitch>("IDSwitch");
			idSwitch.OnValueChanged = (Action<int>)Delegate.Combine(idSwitch.OnValueChanged, new Action<int>(delegate(int delta)
			{
				this.OnQuickAdjustTriggered(0);
			}));
			this.Refers.CGet<InfinityScrollLegacy>("ColorScroll").GetComponent<CToggleGroupObsolete>().OnActiveToggleChange = delegate(CToggleObsolete n, CToggleObsolete o)
			{
				this.OnQuickAdjustTriggered(0);
			};
			bool flag = null != this.Controller;
			if (flag)
			{
				this.OnQuickAdjustTriggered(0);
			}
			base.Close(false);
			bool flag2 = null != this.ShaveBaldToggle;
			if (flag2)
			{
				this.SetDisableStyle(this.ShaveBaldToggle.interactable);
				this.ShaveBaldToggle.onValueChanged.AddListener(new UnityAction<bool>(this.OnShaveBaldValueChanged));
			}
		}

		// Token: 0x0600B5F9 RID: 46585 RVA: 0x0052E3A8 File Offset: 0x0052C5A8
		public override void OnOpen(bool anim)
		{
			this.UpdateAssetCore();
			base.OnOpen(anim);
			this.Refers.CGet<InfinityScrollLegacy>("ColorScroll").ReRender();
		}

		// Token: 0x0600B5FA RID: 46586 RVA: 0x0052E3D0 File Offset: 0x0052C5D0
		public override void BindArgUpdate()
		{
			base.RegisterOnArgUpdateListener(new Action(this.ArgsUpdateCallback));
			this.Refers.CGet<CToggleObsolete>("LockSwitch").onValueChanged.RemoveAllListeners();
			this.Refers.CGet<CToggleObsolete>("LockSwitch").isOn = true;
			this.Refers.CGet<CToggleObsolete>("LockSwitch").onValueChanged.AddListener(delegate(bool val)
			{
				this.Controller.UpdateColorLockGroup(this);
			});
			base.UpdateColorScroll(this.Refers.CGet<InfinityScrollLegacy>("ColorScroll"), AvatarAdjustController.HairColors);
		}

		// Token: 0x0600B5FB RID: 46587 RVA: 0x0052E468 File Offset: 0x0052C668
		private void ArgsUpdateCallback()
		{
			foreach (Avatar avatar in this.Controller.AvatarList)
			{
				bool flag = this.Controller.GetAge() < 16;
				if (flag)
				{
					avatar.Refresh();
				}
				else
				{
					avatar.UpdateBackHair();
					avatar.UpdateFrontHair();
				}
			}
			this.OnQuickAdjustTriggered(0);
		}

		// Token: 0x0600B5FC RID: 46588 RVA: 0x0052E4F4 File Offset: 0x0052C6F4
		private void UpdateAssetCore()
		{
			bool flag = null == this.Controller;
			if (!flag)
			{
				this._frontHairs = this.Controller.AvatarGroup.Hair1Res.FindAll((HairRes e) => !this.OnlyCreateRes || e.Hair.Config.CanCreate);
				bool flag2 = this.CustomAssetsFilterHandler != null;
				if (flag2)
				{
					this._frontHairs = this.CustomAssetsFilterHandler(this._frontHairs);
				}
				HairRes hairRes = this._frontHairs.Find((HairRes e) => e.Id == base.Data.FrontHairId);
				bool flag3 = hairRes != null;
				if (flag3)
				{
					int selectIndex = this._frontHairs.IndexOf(hairRes) + 1;
					this.Refers.CGet<IdSwitch>("IDSwitch").Init(selectIndex, this._frontHairs.Count, 1);
					this.Refers.CGet<IdSwitch>("IDSwitch").ResetBtnEvents();
				}
			}
		}

		// Token: 0x0600B5FD RID: 46589 RVA: 0x0052E5D0 File Offset: 0x0052C7D0
		protected override void SetId(int newIndex)
		{
			newIndex--;
			bool flag = this._frontHairs.CheckIndex(newIndex);
			if (flag)
			{
				base.Data.FrontHairId = this._frontHairs[newIndex].Id;
				Action onFrontHairIdChange = this.OnFrontHairIdChange;
				if (onFrontHairIdChange != null)
				{
					onFrontHairIdChange();
				}
				Action onArgsUpdate = this.OnArgsUpdate;
				if (onArgsUpdate != null)
				{
					onArgsUpdate();
				}
			}
		}

		// Token: 0x0600B5FE RID: 46590 RVA: 0x0052E638 File Offset: 0x0052C838
		protected override int GetColorIndex()
		{
			for (int i = 0; i < AvatarAdjustController.HairColors.Count; i++)
			{
				bool flag = AvatarAdjustController.HairColors[i].Item1 == base.Data.ColorFrontHairId;
				if (flag)
				{
					return i;
				}
			}
			return 0;
		}

		// Token: 0x0600B5FF RID: 46591 RVA: 0x0052E68B File Offset: 0x0052C88B
		public override void SetColorIndex(int index)
		{
			base.Data.ColorFrontHairId = AvatarAdjustController.HairColors[index].Item1;
			Action onArgsUpdate = this.OnArgsUpdate;
			if (onArgsUpdate != null)
			{
				onArgsUpdate();
			}
		}

		// Token: 0x0600B600 RID: 46592 RVA: 0x0052E6BC File Offset: 0x0052C8BC
		public override void OnQuickAdjustTriggered(int delta)
		{
			int currIndex = this.GetColorIndex();
			bool flag = this._frontHairs != null;
			if (flag)
			{
				this.Refers.CGet<IdSwitch>("IDSwitch").SetValueAndRefresh(this._frontHairs.FindIndex((HairRes p) => p.Id == base.Data.FrontHairId) + 1);
			}
			bool flag2 = delta < 0;
			if (flag2)
			{
				this.Refers.CGet<IdSwitch>("IDSwitch").BtnPrevId.onClick.Invoke();
			}
			else
			{
				bool flag3 = delta > 0;
				if (flag3)
				{
					this.Refers.CGet<IdSwitch>("IDSwitch").BtnNextId.onClick.Invoke();
				}
			}
			Refers color = this.Refers.CGet<Refers>("ColorPrefab");
			bool flag4 = color != null;
			if (flag4)
			{
				base.OnColorPrefabRender(currIndex, color);
			}
			bool flag5 = this.Refers.Names.Contains("SimpleInfo");
			if (flag5)
			{
				this.Refers.CGet<TextMeshProUGUI>("SimpleInfo").SetText(this.Refers.CGet<IdSwitch>("IDSwitch").IdValue.text, true);
			}
			this.Refers.CGet<InfinityScrollLegacy>("ColorScroll").ReRender();
		}

		// Token: 0x0600B601 RID: 46593 RVA: 0x0052E7EC File Offset: 0x0052C9EC
		public void SetDisableStyle(bool isEnable)
		{
			this._shaveBaldDisableStyleRoot = this.ShaveBaldToggle.GetComponent<DisableStyleRoot>();
			bool flag = null != this._shaveBaldDisableStyleRoot;
			if (flag)
			{
				this._shaveBaldDisableStyleRoot.SetStyleEffect(!isEnable, false);
			}
			TooltipInvoker mouseTip = this.ShaveBaldToggle.GetComponent<TooltipInvoker>();
			bool flag2 = null != mouseTip;
			if (flag2)
			{
				mouseTip.PresetParam[1] = (isEnable ? "UI_LK_ShaveBald_Enable" : "UI_LK_ShaveBald_Disable");
			}
		}

		// Token: 0x0600B602 RID: 46594 RVA: 0x0052E85C File Offset: 0x0052CA5C
		private void OnShaveBaldValueChanged(bool isOn)
		{
			base.Data.SetGrowableElementShowingState(0, !isOn);
			bool flag = null != this.Controller.AvatarAdjustItemBackHair && null != this.Controller.AvatarAdjustItemBackHair.ShaveBaldToggle;
			if (flag)
			{
				this.Controller.AvatarAdjustItemBackHair.ShaveBaldToggle.SetIsOnWithoutNotify(isOn);
			}
			Action onArgsUpdate = this.OnArgsUpdate;
			if (onArgsUpdate != null)
			{
				onArgsUpdate();
			}
		}

		// Token: 0x04008D5D RID: 36189
		public bool OnlyCreateRes;

		// Token: 0x04008D5E RID: 36190
		public Action OnFrontHairIdChange;

		// Token: 0x04008D5F RID: 36191
		public Func<List<HairRes>, List<HairRes>> CustomAssetsFilterHandler;

		// Token: 0x04008D60 RID: 36192
		private List<HairRes> _frontHairs;
	}
}
