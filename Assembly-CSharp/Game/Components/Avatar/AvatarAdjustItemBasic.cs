using System;
using Config;
using TMPro;
using UnityEngine.Events;

namespace Game.Components.Avatar
{
	// Token: 0x02000F72 RID: 3954
	public class AvatarAdjustItemBasic : AvatarAdjustItemBase
	{
		// Token: 0x0600B564 RID: 46436 RVA: 0x0052AA24 File Offset: 0x00528C24
		public override void OnQuickAdjustTriggered(int delta)
		{
			Action<int> onQuickAdjust = this.OnQuickAdjust;
			if (onQuickAdjust != null)
			{
				onQuickAdjust(delta);
			}
			Action onRefreshDisplay = this.OnRefreshDisplay;
			if (onRefreshDisplay != null)
			{
				onRefreshDisplay();
			}
		}

		// Token: 0x0600B565 RID: 46437 RVA: 0x0052AA4C File Offset: 0x00528C4C
		protected override void Awake()
		{
			base.Awake();
			this.Closed = false;
			this.AgeSlider.minValue = 16f;
			this.AgeSlider.maxValue = (float)GlobalConfig.Instance.MaxAgeOfCreatingChar;
		}

		// Token: 0x0600B566 RID: 46438 RVA: 0x0052AA88 File Offset: 0x00528C88
		protected override void Start()
		{
			base.Start();
			this.AgeSlider.onValueChanged.AddListener(new UnityAction<float>(this.OnAgeLabelSliderValueChanged));
			this.BornMonthSlider.onValueChanged.AddListener(new UnityAction<float>(this.OnBornMonthSliderValueChanged));
			this.UpdateBornMonthInfo(this.BornMonthSlider.value);
			this.OnOpen(false);
		}

		// Token: 0x0600B567 RID: 46439 RVA: 0x0052AAF4 File Offset: 0x00528CF4
		private void OnEnable()
		{
			bool flag = null == this.Controller;
			if (!flag)
			{
				this.AgeSlider.value = (float)this.Controller.GetAge();
				this.BornMonthSlider.value = (float)this.Controller.BornMonth;
			}
		}

		// Token: 0x0600B568 RID: 46440 RVA: 0x0052AB44 File Offset: 0x00528D44
		public override void OnOpen(bool anim)
		{
			this.AgeSlider.SetValueWithoutNotify((float)this.Controller.GetAge());
			this.BornMonthSlider.SetValueWithoutNotify((float)this.Controller.BornMonth);
			this.UpdateAgeInfo((float)this.Controller.GetAge());
			this.UpdateBornMonthInfo((float)this.Controller.BornMonth);
		}

		// Token: 0x0600B569 RID: 46441 RVA: 0x0052ABA8 File Offset: 0x00528DA8
		private void UpdateAgeInfo(float value)
		{
			this._autoSettingAgeSlider = true;
			this.AgeSlider.value = value;
			this.OnAgeLabelSliderValueChanged(value);
			this._autoSettingAgeSlider = false;
		}

		// Token: 0x0600B56A RID: 46442 RVA: 0x0052ABD0 File Offset: 0x00528DD0
		private void OnAgeLabelSliderValueChanged(float value)
		{
			bool flag = !this._autoSettingAgeSlider;
			if (flag)
			{
				this.Controller.SetAge((short)value);
			}
			this.AgeLabel.text = LocalStringManager.GetFormat(LanguageKey.LK_Age, (short)value);
			Action onRefreshDisplay = this.OnRefreshDisplay;
			if (onRefreshDisplay != null)
			{
				onRefreshDisplay();
			}
		}

		// Token: 0x0600B56B RID: 46443 RVA: 0x0052AC28 File Offset: 0x00528E28
		private void UpdateBornMonthInfo(float value)
		{
			this._autoSettingBornMonthSlider = true;
			this.BornMonthSlider.value = value;
			this.OnBornMonthSliderValueChanged(value);
			this._autoSettingBornMonthSlider = false;
		}

		// Token: 0x0600B56C RID: 46444 RVA: 0x0052AC50 File Offset: 0x00528E50
		private void OnBornMonthSliderValueChanged(float value)
		{
			sbyte month = (sbyte)value;
			bool flag = !this._autoSettingBornMonthSlider;
			if (flag)
			{
				this.Controller.SetBornMonth(month);
			}
			MonthItem config = Month.Instance.GetItem(month);
			bool flag2 = config == null;
			if (flag2)
			{
				throw new Exception("failed to get month config by value: " + value.ToString());
			}
			this.FiveElementsIcon.SetSprite(CommonUtils.GetFiveElementsIconByType(config.FiveElementsType), false, null);
			this.BornMonthLabel.text = LocalStringManager.GetFormat(LanguageKey.UI_NewGame_BornDateInfo, config.Name);
			Action onRefreshDisplay = this.OnRefreshDisplay;
			if (onRefreshDisplay != null)
			{
				onRefreshDisplay();
			}
		}

		// Token: 0x04008D43 RID: 36163
		public TextMeshProUGUI AgeLabel;

		// Token: 0x04008D44 RID: 36164
		public CSliderLegacy AgeSlider;

		// Token: 0x04008D45 RID: 36165
		public TextMeshProUGUI BornMonthLabel;

		// Token: 0x04008D46 RID: 36166
		public CImage FiveElementsIcon;

		// Token: 0x04008D47 RID: 36167
		public CSliderLegacy BornMonthSlider;

		// Token: 0x04008D48 RID: 36168
		public Action OnRefreshDisplay;

		// Token: 0x04008D49 RID: 36169
		public Action<int> OnQuickAdjust;

		// Token: 0x04008D4A RID: 36170
		private bool _autoSettingAgeSlider = false;

		// Token: 0x04008D4B RID: 36171
		private bool _autoSettingBornMonthSlider = false;
	}
}
