using System;
using System.Collections.Generic;
using GameData.Domains.Character.AvatarSystem.AvatarRes;
using TMPro;
using UnityEngine.Events;

namespace Game.Components.Avatar
{
	// Token: 0x02000F74 RID: 3956
	public class AvatarAdjustItemBeard2 : AvatarAdjustItemBase
	{
		// Token: 0x0600B57F RID: 46463 RVA: 0x0052B2DC File Offset: 0x005294DC
		protected override void Start()
		{
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

		// Token: 0x0600B580 RID: 46464 RVA: 0x0052B3A9 File Offset: 0x005295A9
		public override void OnOpen(bool anim)
		{
			this.UpdateAssetCore();
			base.OnOpen(anim);
			this.Refers.CGet<InfinityScrollLegacy>("ColorScroll").ReRender();
		}

		// Token: 0x0600B581 RID: 46465 RVA: 0x0052B3D4 File Offset: 0x005295D4
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

		// Token: 0x0600B582 RID: 46466 RVA: 0x0052B46C File Offset: 0x0052966C
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
					avatar.UpdateBeard();
				}
			}
			this.OnQuickAdjustTriggered(0);
		}

		// Token: 0x0600B583 RID: 46467 RVA: 0x0052B4F0 File Offset: 0x005296F0
		private void UpdateAssetCore()
		{
			bool flag = null == this.Controller;
			if (!flag)
			{
				this._beard2Assets = this.Controller.AvatarGroup.Beard2Res;
				bool flag2 = this.CustomAssetsFilterHandler != null;
				if (flag2)
				{
					this._beard2Assets = this.CustomAssetsFilterHandler(this._beard2Assets);
				}
				AvatarAsset nowBeard2Asset = this._beard2Assets.Find((AvatarAsset e) => e.Id == base.Data.Beard2Id);
				bool flag3 = nowBeard2Asset != null;
				if (flag3)
				{
					int selectIndex = this._beard2Assets.IndexOf(nowBeard2Asset) + 1;
					this.Refers.CGet<IdSwitch>("IDSwitch").Init(selectIndex, this._beard2Assets.Count, 1);
					this.Refers.CGet<IdSwitch>("IDSwitch").ResetBtnEvents();
				}
			}
		}

		// Token: 0x0600B584 RID: 46468 RVA: 0x0052B5BC File Offset: 0x005297BC
		protected override void SetId(int newIndex)
		{
			newIndex--;
			bool flag = this._beard2Assets.CheckIndex(newIndex);
			if (flag)
			{
				base.Data.Beard2Id = this._beard2Assets[newIndex].Id;
				Action onArgsUpdate = this.OnArgsUpdate;
				if (onArgsUpdate != null)
				{
					onArgsUpdate();
				}
			}
		}

		// Token: 0x0600B585 RID: 46469 RVA: 0x0052B610 File Offset: 0x00529810
		protected override int GetColorIndex()
		{
			for (int i = 0; i < AvatarAdjustController.HairColors.Count; i++)
			{
				bool flag = AvatarAdjustController.HairColors[i].Item1 == base.Data.ColorBeard2Id;
				if (flag)
				{
					return i;
				}
			}
			return 0;
		}

		// Token: 0x0600B586 RID: 46470 RVA: 0x0052B663 File Offset: 0x00529863
		public override void SetColorIndex(int index)
		{
			base.Data.ColorBeard2Id = AvatarAdjustController.HairColors[index].Item1;
			Action onArgsUpdate = this.OnArgsUpdate;
			if (onArgsUpdate != null)
			{
				onArgsUpdate();
			}
		}

		// Token: 0x0600B587 RID: 46471 RVA: 0x0052B694 File Offset: 0x00529894
		public override void OnQuickAdjustTriggered(int delta)
		{
			int currIndex = this.GetColorIndex();
			bool flag = this._beard2Assets != null;
			if (flag)
			{
				this.Refers.CGet<IdSwitch>("IDSwitch").SetValueAndRefresh(this._beard2Assets.FindIndex((AvatarAsset p) => p.Id == base.Data.Beard2Id) + 1);
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

		// Token: 0x0600B588 RID: 46472 RVA: 0x0052B7C4 File Offset: 0x005299C4
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

		// Token: 0x0600B589 RID: 46473 RVA: 0x0052B832 File Offset: 0x00529A32
		private void OnShaveBaldValueChanged(bool isOn)
		{
			base.Data.SetGrowableElementShowingState(2, !isOn);
			Action onArgsUpdate = this.OnArgsUpdate;
			if (onArgsUpdate != null)
			{
				onArgsUpdate();
			}
		}

		// Token: 0x04008D4E RID: 36174
		private List<AvatarAsset> _beard2Assets;

		// Token: 0x04008D4F RID: 36175
		public Func<List<AvatarAsset>, List<AvatarAsset>> CustomAssetsFilterHandler;
	}
}
