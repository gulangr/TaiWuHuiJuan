using System;
using System.Collections.Generic;
using GameData.Domains.Character.AvatarSystem;
using GameData.Domains.Character.AvatarSystem.AvatarRes;
using TMPro;

namespace Game.Components.Avatar
{
	// Token: 0x02000F7E RID: 3966
	public class AvatarAdjustItemNose : AvatarAdjustItemBase
	{
		// Token: 0x0600B622 RID: 46626 RVA: 0x0052F0C4 File Offset: 0x0052D2C4
		protected override void Start()
		{
			this.UpdateAssetCore();
			IdSwitch idSwitch = this.Refers.CGet<IdSwitch>("IDSwitch");
			idSwitch.OnValueChanged = (Action<int>)Delegate.Combine(idSwitch.OnValueChanged, new Action<int>(delegate(int delta)
			{
				this.OnQuickAdjustTriggered(0);
			}));
			bool flag = null != this.Controller;
			if (flag)
			{
				this.OnQuickAdjustTriggered(0);
			}
			base.Close(false);
		}

		// Token: 0x0600B623 RID: 46627 RVA: 0x0052F12A File Offset: 0x0052D32A
		public override void OnOpen(bool anim)
		{
			this.UpdateAssetCore();
			base.OnOpen(anim);
			this.UpdateHeightSlider();
			this.UpdateScaleSlider();
		}

		// Token: 0x0600B624 RID: 46628 RVA: 0x0052F14C File Offset: 0x0052D34C
		public override void BindArgUpdate()
		{
			base.RegisterOnArgUpdateListener(new Action(this.ArgsUpdateCallback));
			CSliderLegacy heightSlider = this.Refers.CGet<CSliderLegacy>("HeightSlider");
			heightSlider.wholeNumbers = true;
			heightSlider.minValue = 0f;
			heightSlider.maxValue = 100f;
			heightSlider.onValueChanged.RemoveAllListeners();
			heightSlider.onValueChanged.AddListener(delegate(float val)
			{
				base.Data.NoseHeight = AvatarData.CalcOffsetShortVal(base.Data.PositionConfig.NoseHeightRange, (int)val);
				Action onArgsUpdate = this.OnArgsUpdate;
				if (onArgsUpdate != null)
				{
					onArgsUpdate();
				}
			});
			CSliderLegacy scaleSlider = this.Refers.CGet<CSliderLegacy>("ScaleSlider");
			scaleSlider.wholeNumbers = true;
			scaleSlider.minValue = 0f;
			scaleSlider.maxValue = 100f;
			scaleSlider.onValueChanged.RemoveAllListeners();
			scaleSlider.onValueChanged.AddListener(delegate(float val)
			{
				base.Data.NoseScale = AvatarData.CalcOffsetShortVal(base.Data.PositionConfig.NoseScaleRange, (int)val);
				Action onArgsUpdate = this.OnArgsUpdate;
				if (onArgsUpdate != null)
				{
					onArgsUpdate();
				}
			});
		}

		// Token: 0x0600B625 RID: 46629 RVA: 0x0052F218 File Offset: 0x0052D418
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
					avatar.UpdateNose();
				}
			}
		}

		// Token: 0x0600B626 RID: 46630 RVA: 0x0052F294 File Offset: 0x0052D494
		private void UpdateAssetCore()
		{
			bool flag = null == this.Controller;
			if (!flag)
			{
				this._noseAssetList = this.Controller.AvatarGroup.NoseRes;
				bool flag2 = this.CustomAssetsFilterHandler != null;
				if (flag2)
				{
					this._noseAssetList = this.CustomAssetsFilterHandler(this._noseAssetList);
				}
				AvatarAsset noseAsset = this._noseAssetList.Find((AvatarAsset e) => e.Id == base.Data.NoseId);
				bool flag3 = noseAsset != null;
				if (flag3)
				{
					int selectIndex = this._noseAssetList.IndexOf(noseAsset) + 1;
					this.Refers.CGet<IdSwitch>("IDSwitch").Init(selectIndex, this._noseAssetList.Count, 1);
					this.Refers.CGet<IdSwitch>("IDSwitch").ResetBtnEvents();
				}
			}
		}

		// Token: 0x0600B627 RID: 46631 RVA: 0x0052F360 File Offset: 0x0052D560
		protected override void SetId(int newIndex)
		{
			newIndex--;
			bool flag = this._noseAssetList.CheckIndex(newIndex);
			if (flag)
			{
				base.Data.NoseId = this._noseAssetList[newIndex].Id;
				Action onArgsUpdate = this.OnArgsUpdate;
				if (onArgsUpdate != null)
				{
					onArgsUpdate();
				}
			}
		}

		// Token: 0x0600B628 RID: 46632 RVA: 0x0052F3B3 File Offset: 0x0052D5B3
		private void UpdateHeightSlider()
		{
			this.Refers.CGet<CSliderLegacy>("HeightSlider").value = (float)AvatarData.CalcOffsetShortValPercent(base.Data.PositionConfig.NoseHeightRange, base.Data.NoseHeight);
		}

		// Token: 0x0600B629 RID: 46633 RVA: 0x0052F3ED File Offset: 0x0052D5ED
		private void UpdateScaleSlider()
		{
			this.Refers.CGet<CSliderLegacy>("ScaleSlider").value = (float)AvatarData.CalcOffsetShortValPercent(base.Data.PositionConfig.NoseScaleRange, base.Data.NoseScale);
		}

		// Token: 0x0600B62A RID: 46634 RVA: 0x0052F428 File Offset: 0x0052D628
		public override void OnQuickAdjustTriggered(int delta)
		{
			this.Refers.CGet<IdSwitch>("IDSwitch").SetValueAndRefresh(this._noseAssetList.FindIndex((AvatarAsset p) => p.Id == base.Data.NoseId) + 1);
			bool flag = delta < 0;
			if (flag)
			{
				this.Refers.CGet<IdSwitch>("IDSwitch").BtnPrevId.onClick.Invoke();
			}
			else
			{
				bool flag2 = delta > 0;
				if (flag2)
				{
					this.Refers.CGet<IdSwitch>("IDSwitch").BtnNextId.onClick.Invoke();
				}
			}
			bool flag3 = this.Refers.Names.Contains("SimpleInfo");
			if (flag3)
			{
				this.Refers.CGet<TextMeshProUGUI>("SimpleInfo").SetText(this.Refers.CGet<IdSwitch>("IDSwitch").IdValue.text, true);
			}
		}

		// Token: 0x04008D67 RID: 36199
		private List<AvatarAsset> _noseAssetList;

		// Token: 0x04008D68 RID: 36200
		public Func<List<AvatarAsset>, List<AvatarAsset>> CustomAssetsFilterHandler;
	}
}
