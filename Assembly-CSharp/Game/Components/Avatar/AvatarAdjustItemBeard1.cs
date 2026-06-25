using System;
using System.Collections.Generic;
using GameData.Domains.Character.AvatarSystem.AvatarRes;
using TMPro;
using UnityEngine.Events;

namespace Game.Components.Avatar
{
	// Token: 0x02000F73 RID: 3955
	public class AvatarAdjustItemBeard1 : AvatarAdjustItemBase
	{
		// Token: 0x0600B56E RID: 46446 RVA: 0x0052AD04 File Offset: 0x00528F04
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

		// Token: 0x0600B56F RID: 46447 RVA: 0x0052ADD1 File Offset: 0x00528FD1
		public override void OnOpen(bool anim)
		{
			this.UpdateAssetCore();
			base.OnOpen(anim);
			this.Refers.CGet<InfinityScrollLegacy>("ColorScroll").ReRender();
		}

		// Token: 0x0600B570 RID: 46448 RVA: 0x0052ADFC File Offset: 0x00528FFC
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

		// Token: 0x0600B571 RID: 46449 RVA: 0x0052AE94 File Offset: 0x00529094
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

		// Token: 0x0600B572 RID: 46450 RVA: 0x0052AF18 File Offset: 0x00529118
		private void UpdateAssetCore()
		{
			bool flag = null == this.Controller;
			if (!flag)
			{
				this._beard1Assets = this.Controller.AvatarGroup.Beard1Res;
				bool flag2 = this.CustomAssetsFilterHandler != null;
				if (flag2)
				{
					this._beard1Assets = this.CustomAssetsFilterHandler(this._beard1Assets);
				}
				AvatarAsset nowBeard1Asset = this._beard1Assets.Find((AvatarAsset e) => e.Id == base.Data.Beard1Id);
				bool flag3 = nowBeard1Asset != null;
				if (flag3)
				{
					int selectIndex = this._beard1Assets.IndexOf(nowBeard1Asset) + 1;
					this.Refers.CGet<IdSwitch>("IDSwitch").Init(selectIndex, this._beard1Assets.Count, 1);
					this.Refers.CGet<IdSwitch>("IDSwitch").ResetBtnEvents();
				}
			}
		}

		// Token: 0x0600B573 RID: 46451 RVA: 0x0052AFE4 File Offset: 0x005291E4
		protected override void SetId(int newIndex)
		{
			newIndex--;
			bool flag = this._beard1Assets.CheckIndex(newIndex);
			if (flag)
			{
				base.Data.Beard1Id = this._beard1Assets[newIndex].Id;
				Action onArgsUpdate = this.OnArgsUpdate;
				if (onArgsUpdate != null)
				{
					onArgsUpdate();
				}
			}
		}

		// Token: 0x0600B574 RID: 46452 RVA: 0x0052B038 File Offset: 0x00529238
		protected override int GetColorIndex()
		{
			for (int i = 0; i < AvatarAdjustController.HairColors.Count; i++)
			{
				bool flag = AvatarAdjustController.HairColors[i].Item1 == base.Data.ColorBeard1Id;
				if (flag)
				{
					return i;
				}
			}
			return 0;
		}

		// Token: 0x0600B575 RID: 46453 RVA: 0x0052B08B File Offset: 0x0052928B
		public override void SetColorIndex(int index)
		{
			base.Data.ColorBeard1Id = AvatarAdjustController.HairColors[index].Item1;
			Action onArgsUpdate = this.OnArgsUpdate;
			if (onArgsUpdate != null)
			{
				onArgsUpdate();
			}
		}

		// Token: 0x0600B576 RID: 46454 RVA: 0x0052B0BC File Offset: 0x005292BC
		public override void OnQuickAdjustTriggered(int delta)
		{
			int currIndex = this.GetColorIndex();
			bool flag = this._beard1Assets != null;
			if (flag)
			{
				this.Refers.CGet<IdSwitch>("IDSwitch").SetValueAndRefresh(this._beard1Assets.FindIndex((AvatarAsset p) => p.Id == base.Data.Beard1Id) + 1);
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

		// Token: 0x0600B577 RID: 46455 RVA: 0x0052B1EC File Offset: 0x005293EC
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

		// Token: 0x0600B578 RID: 46456 RVA: 0x0052B25A File Offset: 0x0052945A
		private void OnShaveBaldValueChanged(bool isOn)
		{
			base.Data.SetGrowableElementShowingState(1, !isOn);
			Action onArgsUpdate = this.OnArgsUpdate;
			if (onArgsUpdate != null)
			{
				onArgsUpdate();
			}
		}

		// Token: 0x04008D4C RID: 36172
		private List<AvatarAsset> _beard1Assets;

		// Token: 0x04008D4D RID: 36173
		public Func<List<AvatarAsset>, List<AvatarAsset>> CustomAssetsFilterHandler;
	}
}
