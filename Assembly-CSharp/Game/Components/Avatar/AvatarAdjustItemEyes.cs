using System;
using System.Collections.Generic;
using GameData.Domains.Character.AvatarSystem;
using GameData.Domains.Character.AvatarSystem.AvatarRes;
using TMPro;

namespace Game.Components.Avatar
{
	// Token: 0x02000F78 RID: 3960
	public class AvatarAdjustItemEyes : AvatarAdjustItemBase
	{
		// Token: 0x0600B5BD RID: 46525 RVA: 0x0052CA68 File Offset: 0x0052AC68
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
		}

		// Token: 0x0600B5BE RID: 46526 RVA: 0x0052CAF4 File Offset: 0x0052ACF4
		public override void OnOpen(bool anim)
		{
			this.UpdateAssetCore();
			base.OnOpen(anim);
			this.Refers.CGet<InfinityScrollLegacy>("ColorScroll").ReRender();
			this.UpdateDistanceSlider();
			this.UpdateHeightSlider();
			this.UpdateScaleSlider();
			this.UpdateAngleSlider();
		}

		// Token: 0x0600B5BF RID: 46527 RVA: 0x0052CB44 File Offset: 0x0052AD44
		public override void BindArgUpdate()
		{
			base.RegisterOnArgUpdateListener(new Action(this.ArgsUpdateCallback));
			CSliderLegacy distanceSlider = this.Refers.CGet<CSliderLegacy>("DistanceSlider");
			distanceSlider.wholeNumbers = true;
			distanceSlider.minValue = 0f;
			distanceSlider.maxValue = 100f;
			distanceSlider.onValueChanged.RemoveAllListeners();
			distanceSlider.onValueChanged.AddListener(delegate(float val)
			{
				base.Data.EyesDistance = AvatarData.CalcOffsetShortVal(base.Data.PositionConfig.EyeDistanceRange, (int)val);
				Action onArgsUpdate = this.OnArgsUpdate;
				if (onArgsUpdate != null)
				{
					onArgsUpdate();
				}
			});
			CSliderLegacy heightSlider = this.Refers.CGet<CSliderLegacy>("HeightSlider");
			heightSlider.wholeNumbers = true;
			heightSlider.minValue = 0f;
			heightSlider.maxValue = 100f;
			heightSlider.onValueChanged.RemoveAllListeners();
			heightSlider.onValueChanged.AddListener(delegate(float val)
			{
				base.Data.EyesHeight = AvatarData.CalcOffsetShortVal(base.Data.PositionConfig.EyeHeightRange, (int)val);
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
				base.Data.EyesScale = AvatarData.CalcOffsetShortVal(base.Data.PositionConfig.EyeScaleRange, (int)val);
				Action onArgsUpdate = this.OnArgsUpdate;
				if (onArgsUpdate != null)
				{
					onArgsUpdate();
				}
				GLog.LogWithTag("Data.EyesScale = " + base.Data.EyesScale.ToString() + "    val = " + val.ToString(), Array.Empty<object>());
			});
			CSliderLegacy angleSlider = this.Refers.CGet<CSliderLegacy>("AngleSlider");
			angleSlider.wholeNumbers = true;
			angleSlider.minValue = 0f;
			angleSlider.maxValue = 100f;
			angleSlider.onValueChanged.RemoveAllListeners();
			angleSlider.onValueChanged.AddListener(delegate(float val)
			{
				base.Data.EyesAngle = AvatarData.CalcOffsetShortVal(base.Data.PositionConfig.EyeAngleRange, (int)val);
				Action onArgsUpdate = this.OnArgsUpdate;
				if (onArgsUpdate != null)
				{
					onArgsUpdate();
				}
			});
			base.UpdateColorScroll(this.Refers.CGet<InfinityScrollLegacy>("ColorScroll"), AvatarAdjustController.EyeBallColors);
		}

		// Token: 0x0600B5C0 RID: 46528 RVA: 0x0052CCD8 File Offset: 0x0052AED8
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
					avatar.UpdateEyes();
					avatar.UpdateEyebrows();
					avatar.UpdateNose();
					avatar.UpdateFeature();
				}
			}
			this.OnQuickAdjustTriggered(0);
		}

		// Token: 0x0600B5C1 RID: 46529 RVA: 0x0052CD70 File Offset: 0x0052AF70
		private void UpdateAssetCore()
		{
			bool flag = null == this.Controller;
			if (!flag)
			{
				this._eyesAssets = this.Controller.AvatarGroup.EyesGroup;
				bool flag2 = this.CustomAssetsFilterHandler != null;
				if (flag2)
				{
					Func<List<EyeRes>, List<EyeRes>> customAssetsFilterHandler = this.CustomAssetsFilterHandler;
					this._eyesAssets = ((customAssetsFilterHandler != null) ? customAssetsFilterHandler(this._eyesAssets) : null);
				}
				EyeRes eyeRes = this._eyesAssets.Find((EyeRes e) => e.Id == base.Data.EyesMainId && e.LeftEye.SubId == base.Data.EyesLeftId && e.RightEye.SubId == base.Data.EyesRightId);
				bool flag3 = eyeRes != null;
				if (flag3)
				{
					int selectIndex = this._eyesAssets.IndexOf(eyeRes) + 1;
					this.Refers.CGet<IdSwitch>("IDSwitch").Init(selectIndex, this._eyesAssets.Count, 1);
					this.Refers.CGet<IdSwitch>("IDSwitch").ResetBtnEvents();
				}
			}
		}

		// Token: 0x0600B5C2 RID: 46530 RVA: 0x0052CE40 File Offset: 0x0052B040
		protected override void SetId(int newIndex)
		{
			newIndex--;
			bool flag = this._eyesAssets.CheckIndex(newIndex);
			if (flag)
			{
				EyeRes eyeRes = this._eyesAssets[newIndex];
				base.Data.EyesMainId = eyeRes.Id;
				base.Data.EyesLeftId = (short)((byte)eyeRes.LeftEye.SubId);
				base.Data.EyesRightId = (short)((byte)eyeRes.RightEye.SubId);
				Action onArgsUpdate = this.OnArgsUpdate;
				if (onArgsUpdate != null)
				{
					onArgsUpdate();
				}
			}
		}

		// Token: 0x0600B5C3 RID: 46531 RVA: 0x0052CEC4 File Offset: 0x0052B0C4
		protected override int GetColorIndex()
		{
			for (int i = 0; i < AvatarAdjustController.EyeBallColors.Count; i++)
			{
				bool flag = AvatarAdjustController.EyeBallColors[i].Item1 == base.Data.ColorEyeballId;
				if (flag)
				{
					return i;
				}
			}
			return 0;
		}

		// Token: 0x0600B5C4 RID: 46532 RVA: 0x0052CF17 File Offset: 0x0052B117
		public override void SetColorIndex(int index)
		{
			base.Data.ColorEyeballId = AvatarAdjustController.EyeBallColors[index].Item1;
			Action onArgsUpdate = this.OnArgsUpdate;
			if (onArgsUpdate != null)
			{
				onArgsUpdate();
			}
		}

		// Token: 0x0600B5C5 RID: 46533 RVA: 0x0052CF47 File Offset: 0x0052B147
		private void UpdateDistanceSlider()
		{
			this.Refers.CGet<CSliderLegacy>("DistanceSlider").value = (float)AvatarData.CalcOffsetShortValPercent(base.Data.PositionConfig.EyeDistanceRange, base.Data.EyesDistance);
		}

		// Token: 0x0600B5C6 RID: 46534 RVA: 0x0052CF81 File Offset: 0x0052B181
		private void UpdateHeightSlider()
		{
			this.Refers.CGet<CSliderLegacy>("HeightSlider").value = (float)AvatarData.CalcOffsetShortValPercent(base.Data.PositionConfig.EyeHeightRange, base.Data.EyesHeight);
		}

		// Token: 0x0600B5C7 RID: 46535 RVA: 0x0052CFBB File Offset: 0x0052B1BB
		private void UpdateScaleSlider()
		{
			this.Refers.CGet<CSliderLegacy>("ScaleSlider").value = (float)AvatarData.CalcOffsetShortValPercent(base.Data.PositionConfig.EyeScaleRange, base.Data.EyesScale);
		}

		// Token: 0x0600B5C8 RID: 46536 RVA: 0x0052CFF5 File Offset: 0x0052B1F5
		private void UpdateAngleSlider()
		{
			this.Refers.CGet<CSliderLegacy>("AngleSlider").value = (float)AvatarData.CalcOffsetShortValPercent(base.Data.PositionConfig.EyeAngleRange, base.Data.EyesAngle);
		}

		// Token: 0x0600B5C9 RID: 46537 RVA: 0x0052D030 File Offset: 0x0052B230
		public override void OnQuickAdjustTriggered(int delta)
		{
			int currIndex = this.GetColorIndex();
			for (int i = 0; i < this._eyesAssets.Count; i++)
			{
				bool flag = this._eyesAssets[i].Id == base.Data.EyesMainId && this._eyesAssets[i].LeftEye.SubId == base.Data.EyesLeftId && this._eyesAssets[i].RightEye.SubId == base.Data.EyesRightId;
				if (flag)
				{
					this.Refers.CGet<IdSwitch>("IDSwitch").SetValueAndRefresh(i + 1);
				}
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

		// Token: 0x04008D57 RID: 36183
		private List<EyeRes> _eyesAssets;

		// Token: 0x04008D58 RID: 36184
		public Func<List<EyeRes>, List<EyeRes>> CustomAssetsFilterHandler;
	}
}
