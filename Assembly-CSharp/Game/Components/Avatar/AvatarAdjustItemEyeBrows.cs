using System;
using System.Collections.Generic;
using GameData.Domains.Character.AvatarSystem;
using GameData.Domains.Character.AvatarSystem.AvatarRes;
using TMPro;
using UnityEngine.Events;

namespace Game.Components.Avatar
{
	// Token: 0x02000F77 RID: 3959
	public class AvatarAdjustItemEyeBrows : AvatarAdjustItemBase
	{
		// Token: 0x0600B5A5 RID: 46501 RVA: 0x0052C1AC File Offset: 0x0052A3AC
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

		// Token: 0x0600B5A6 RID: 46502 RVA: 0x0052C27C File Offset: 0x0052A47C
		public override void OnOpen(bool anim)
		{
			this.UpdateAssetCore();
			base.OnOpen(anim);
			this.Refers.CGet<InfinityScrollLegacy>("ColorScroll").ReRender();
			this.UpdateDistanceSlider();
			this.UpdateHeightSlider();
			this.UpdateAngleSlider();
			this.UpdateScaleSlider();
		}

		// Token: 0x0600B5A7 RID: 46503 RVA: 0x0052C2CC File Offset: 0x0052A4CC
		public override void BindArgUpdate()
		{
			base.RegisterOnArgUpdateListener(new Action(this.ArgsUpdateCallback));
			CSliderLegacy distanceSlider = this.Refers.CGet<CSliderLegacy>("DistanceSlider");
			distanceSlider.onValueChanged.RemoveAllListeners();
			distanceSlider.wholeNumbers = true;
			distanceSlider.minValue = 0f;
			distanceSlider.maxValue = 100f;
			distanceSlider.onValueChanged.AddListener(delegate(float val)
			{
				base.Data.EyebrowDistance = AvatarData.CalcOffsetShortVal(base.Data.PositionConfig.EyebrowDistanceRange, (int)val);
				Action onArgsUpdate = this.OnArgsUpdate;
				if (onArgsUpdate != null)
				{
					onArgsUpdate();
				}
			});
			CSliderLegacy heightSlider = this.Refers.CGet<CSliderLegacy>("HeightSlider");
			heightSlider.onValueChanged.RemoveAllListeners();
			distanceSlider.wholeNumbers = true;
			heightSlider.minValue = 0f;
			heightSlider.maxValue = 100f;
			this.Refers.CGet<CSliderLegacy>("HeightSlider").onValueChanged.AddListener(delegate(float val)
			{
				base.Data.EyebrowHeight = AvatarData.CalcOffsetShortVal(base.Data.PositionConfig.EyebrowHeightRange, (int)val);
				Action onArgsUpdate = this.OnArgsUpdate;
				if (onArgsUpdate != null)
				{
					onArgsUpdate();
				}
			});
			CSliderLegacy angleSlider = this.Refers.CGet<CSliderLegacy>("AngleSlider");
			angleSlider.onValueChanged.RemoveAllListeners();
			distanceSlider.wholeNumbers = true;
			angleSlider.minValue = 0f;
			angleSlider.maxValue = 100f;
			angleSlider.onValueChanged.AddListener(delegate(float val)
			{
				base.Data.EyebrowAngle = AvatarData.CalcOffsetShortVal(base.Data.PositionConfig.EyebrowAngleRange, (int)val);
				Action onArgsUpdate = this.OnArgsUpdate;
				if (onArgsUpdate != null)
				{
					onArgsUpdate();
				}
			});
			CSliderLegacy scaleSlider = this.Refers.CGet<CSliderLegacy>("ScaleSlider");
			scaleSlider.onValueChanged.RemoveAllListeners();
			scaleSlider.wholeNumbers = true;
			scaleSlider.minValue = 0f;
			scaleSlider.maxValue = 100f;
			scaleSlider.onValueChanged.AddListener(delegate(float val)
			{
				base.Data.EyebrowScale = AvatarData.CalcOffsetShortVal(base.Data.PositionConfig.EyebrowScaleRange, (int)val);
				Action onArgsUpdate = this.OnArgsUpdate;
				if (onArgsUpdate != null)
				{
					onArgsUpdate();
				}
			});
			base.UpdateColorScroll(this.Refers.CGet<InfinityScrollLegacy>("ColorScroll"), AvatarAdjustController.HairColors);
		}

		// Token: 0x0600B5A8 RID: 46504 RVA: 0x0052C46C File Offset: 0x0052A66C
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
					avatar.UpdateEyebrows();
				}
			}
			this.OnQuickAdjustTriggered(0);
		}

		// Token: 0x0600B5A9 RID: 46505 RVA: 0x0052C4F0 File Offset: 0x0052A6F0
		private void UpdateAssetCore()
		{
			bool flag = null == this.Controller;
			if (!flag)
			{
				this._eyebrowList = this.Controller.AvatarGroup.EyeBrowRes;
				bool flag2 = this.CustomAssetsFilterHandler != null;
				if (flag2)
				{
					this._eyebrowList = this.CustomAssetsFilterHandler(this._eyebrowList);
				}
				int selectIndex = 0;
				AvatarAsset eyebrowAsset = this._eyebrowList.Find((AvatarAsset e) => e.Id == base.Data.EyebrowId);
				bool flag3 = eyebrowAsset != null;
				if (flag3)
				{
					selectIndex = this._eyebrowList.IndexOf(eyebrowAsset) + 1;
				}
				this.Refers.CGet<IdSwitch>("IDSwitch").Init(selectIndex, this._eyebrowList.Count, 1);
				this.Refers.CGet<IdSwitch>("IDSwitch").ResetBtnEvents();
			}
		}

		// Token: 0x0600B5AA RID: 46506 RVA: 0x0052C5BC File Offset: 0x0052A7BC
		protected override void SetId(int newIndex)
		{
			newIndex--;
			bool flag = this._eyebrowList.CheckIndex(newIndex);
			if (flag)
			{
				base.Data.EyebrowId = this._eyebrowList[newIndex].Id;
				Action onArgsUpdate = this.OnArgsUpdate;
				if (onArgsUpdate != null)
				{
					onArgsUpdate();
				}
			}
		}

		// Token: 0x0600B5AB RID: 46507 RVA: 0x0052C60F File Offset: 0x0052A80F
		public override void SetColorIndex(int index)
		{
			base.Data.ColorEyebrowId = AvatarAdjustController.HairColors[index].Item1;
			Action onArgsUpdate = this.OnArgsUpdate;
			if (onArgsUpdate != null)
			{
				onArgsUpdate();
			}
		}

		// Token: 0x0600B5AC RID: 46508 RVA: 0x0052C640 File Offset: 0x0052A840
		protected override int GetColorIndex()
		{
			for (int i = 0; i < AvatarAdjustController.HairColors.Count; i++)
			{
				bool flag = AvatarAdjustController.HairColors[i].Item1 == base.Data.ColorEyebrowId;
				if (flag)
				{
					return i;
				}
			}
			return 0;
		}

		// Token: 0x0600B5AD RID: 46509 RVA: 0x0052C693 File Offset: 0x0052A893
		private void UpdateDistanceSlider()
		{
			this.Refers.CGet<CSliderLegacy>("DistanceSlider").value = (float)AvatarData.CalcOffsetShortValPercent(base.Data.PositionConfig.EyebrowDistanceRange, base.Data.EyebrowDistance);
		}

		// Token: 0x0600B5AE RID: 46510 RVA: 0x0052C6CD File Offset: 0x0052A8CD
		private void UpdateHeightSlider()
		{
			this.Refers.CGet<CSliderLegacy>("HeightSlider").value = (float)AvatarData.CalcOffsetShortValPercent(base.Data.PositionConfig.EyebrowHeightRange, base.Data.EyebrowHeight);
		}

		// Token: 0x0600B5AF RID: 46511 RVA: 0x0052C707 File Offset: 0x0052A907
		private void UpdateAngleSlider()
		{
			this.Refers.CGet<CSliderLegacy>("AngleSlider").value = (float)AvatarData.CalcOffsetShortValPercent(base.Data.PositionConfig.EyebrowAngleRange, base.Data.EyebrowAngle);
		}

		// Token: 0x0600B5B0 RID: 46512 RVA: 0x0052C741 File Offset: 0x0052A941
		private void UpdateScaleSlider()
		{
			this.Refers.CGet<CSliderLegacy>("ScaleSlider").value = (float)AvatarData.CalcOffsetShortValPercent(base.Data.PositionConfig.EyebrowScaleRange, base.Data.EyebrowScale);
		}

		// Token: 0x0600B5B1 RID: 46513 RVA: 0x0052C77C File Offset: 0x0052A97C
		public override void OnQuickAdjustTriggered(int delta)
		{
			int currIndex = this.GetColorIndex();
			bool flag = this._eyebrowList != null;
			if (flag)
			{
				this.Refers.CGet<IdSwitch>("IDSwitch").SetValueAndRefresh(this._eyebrowList.FindIndex((AvatarAsset p) => p.Id == base.Data.EyebrowId) + 1);
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

		// Token: 0x0600B5B2 RID: 46514 RVA: 0x0052C8AC File Offset: 0x0052AAAC
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

		// Token: 0x0600B5B3 RID: 46515 RVA: 0x0052C91A File Offset: 0x0052AB1A
		private void OnShaveBaldValueChanged(bool isOn)
		{
			base.Data.SetGrowableElementShowingState(6, !isOn);
			Action onArgsUpdate = this.OnArgsUpdate;
			if (onArgsUpdate != null)
			{
				onArgsUpdate();
			}
		}

		// Token: 0x04008D55 RID: 36181
		private List<AvatarAsset> _eyebrowList;

		// Token: 0x04008D56 RID: 36182
		public Func<List<AvatarAsset>, List<AvatarAsset>> CustomAssetsFilterHandler;
	}
}
