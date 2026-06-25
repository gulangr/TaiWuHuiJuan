using System;
using System.Collections.Generic;
using GameData.Domains.Character.AvatarSystem;
using GameData.Domains.Character.AvatarSystem.AvatarRes;
using TMPro;

namespace Game.Components.Avatar
{
	// Token: 0x02000F7D RID: 3965
	public class AvatarAdjustItemMouth : AvatarAdjustItemBase
	{
		// Token: 0x0600B610 RID: 46608 RVA: 0x0052EA9C File Offset: 0x0052CC9C
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

		// Token: 0x0600B611 RID: 46609 RVA: 0x0052EB28 File Offset: 0x0052CD28
		public override void OnOpen(bool anim)
		{
			this.UpdateAssetCore();
			base.OnOpen(anim);
			this.Refers.CGet<InfinityScrollLegacy>("ColorScroll").ReRender();
			this.UpdateHeightSlider();
			this.UpdateScaleSlider();
		}

		// Token: 0x0600B612 RID: 46610 RVA: 0x0052EB60 File Offset: 0x0052CD60
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
				base.Data.MouthHeight = AvatarData.CalcOffsetShortVal(base.Data.PositionConfig.MouthHeightRange, (int)val);
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
				base.Data.MouthScale = AvatarData.CalcOffsetShortVal(base.Data.PositionConfig.MouthScaleRange, (int)val);
				Action onArgsUpdate = this.OnArgsUpdate;
				if (onArgsUpdate != null)
				{
					onArgsUpdate();
				}
			});
			base.UpdateColorScroll(this.Refers.CGet<InfinityScrollLegacy>("ColorScroll"), AvatarAdjustController.LipColors);
		}

		// Token: 0x0600B613 RID: 46611 RVA: 0x0052EC48 File Offset: 0x0052CE48
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
					avatar.UpdateMouth();
					avatar.UpdateNose();
				}
			}
			this.OnQuickAdjustTriggered(0);
		}

		// Token: 0x0600B614 RID: 46612 RVA: 0x0052ECD4 File Offset: 0x0052CED4
		private void UpdateAssetCore()
		{
			bool flag = null == this.Controller;
			if (!flag)
			{
				this._mouthResList = this.Controller.AvatarGroup.MouthRes;
				bool flag2 = this.CustomAssetsFilterHandler != null;
				if (flag2)
				{
					this._mouthResList = this.CustomAssetsFilterHandler(this._mouthResList);
				}
				MouthRes mouthRes = this._mouthResList.Find((MouthRes e) => e.Id == base.Data.MouthId);
				bool flag3 = mouthRes != null;
				if (flag3)
				{
					int selectId = this._mouthResList.IndexOf(mouthRes) + 1;
					this.Refers.CGet<IdSwitch>("IDSwitch").Init(selectId, this._mouthResList.Count, 1);
					this.Refers.CGet<IdSwitch>("IDSwitch").ResetBtnEvents();
				}
			}
		}

		// Token: 0x0600B615 RID: 46613 RVA: 0x0052EDA0 File Offset: 0x0052CFA0
		protected override void SetId(int newIndex)
		{
			newIndex--;
			bool flag = this._mouthResList.CheckIndex(newIndex);
			if (flag)
			{
				base.Data.MouthId = this._mouthResList[newIndex].Id;
				Action onArgsUpdate = this.OnArgsUpdate;
				if (onArgsUpdate != null)
				{
					onArgsUpdate();
				}
			}
		}

		// Token: 0x0600B616 RID: 46614 RVA: 0x0052EDF3 File Offset: 0x0052CFF3
		public override void SetColorIndex(int index)
		{
			base.Data.ColorMouthId = AvatarAdjustController.LipColors[index].Item1;
			Action onArgsUpdate = this.OnArgsUpdate;
			if (onArgsUpdate != null)
			{
				onArgsUpdate();
			}
		}

		// Token: 0x0600B617 RID: 46615 RVA: 0x0052EE24 File Offset: 0x0052D024
		protected override int GetColorIndex()
		{
			for (int i = 0; i < AvatarAdjustController.LipColors.Count; i++)
			{
				bool flag = AvatarAdjustController.LipColors[i].Item1 == base.Data.ColorMouthId;
				if (flag)
				{
					return i;
				}
			}
			return 0;
		}

		// Token: 0x0600B618 RID: 46616 RVA: 0x0052EE77 File Offset: 0x0052D077
		private void UpdateHeightSlider()
		{
			this.Refers.CGet<CSliderLegacy>("HeightSlider").value = (float)AvatarData.CalcOffsetShortValPercent(base.Data.PositionConfig.MouthHeightRange, base.Data.MouthHeight);
		}

		// Token: 0x0600B619 RID: 46617 RVA: 0x0052EEB1 File Offset: 0x0052D0B1
		private void UpdateScaleSlider()
		{
			this.Refers.CGet<CSliderLegacy>("ScaleSlider").value = (float)AvatarData.CalcOffsetShortValPercent(base.Data.PositionConfig.MouthScaleRange, base.Data.MouthScale);
		}

		// Token: 0x0600B61A RID: 46618 RVA: 0x0052EEEC File Offset: 0x0052D0EC
		public override void OnQuickAdjustTriggered(int delta)
		{
			int currIndex = this.GetColorIndex();
			this.Refers.CGet<IdSwitch>("IDSwitch").SetValueAndRefresh(this._mouthResList.FindIndex((MouthRes p) => p.Id == base.Data.MouthId) + 1);
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
			Refers color = this.Refers.CGet<Refers>("ColorPrefab");
			bool flag3 = color != null;
			if (flag3)
			{
				base.OnColorPrefabRender(currIndex, color);
			}
			bool flag4 = this.Refers.Names.Contains("SimpleInfo");
			if (flag4)
			{
				this.Refers.CGet<TextMeshProUGUI>("SimpleInfo").SetText(this.Refers.CGet<IdSwitch>("IDSwitch").IdValue.text, true);
			}
			this.Refers.CGet<InfinityScrollLegacy>("ColorScroll").ReRender();
		}

		// Token: 0x04008D65 RID: 36197
		private List<MouthRes> _mouthResList;

		// Token: 0x04008D66 RID: 36198
		public Func<List<MouthRes>, List<MouthRes>> CustomAssetsFilterHandler;
	}
}
