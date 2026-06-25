using System;
using FrameWork;
using FrameWork.UISystem.UIElements;
using TMPro;
using UnityEngine;

namespace Game.Views.Adventure
{
	// Token: 0x02000C61 RID: 3169
	public class AdventureAdvanceDays : MonoBehaviour
	{
		// Token: 0x0600A17C RID: 41340 RVA: 0x004B761C File Offset: 0x004B581C
		private void Awake()
		{
			this.confirm.onClick.ResetListener(new Action(this.Confirm));
			this.cancel.onClick.ResetListener(new Action(this.Hide));
			this.cancelPanel.onClick.ResetListener(new Action(this.Hide));
			this.sliderAdd.onClick.ResetListener(new Action(this.SliderAdd));
			this.sliderReduce.onClick.ResetListener(new Action(this.SliderReduce));
			this.slider.onValueChanged.ResetListener(delegate(float value)
			{
				this.inputField.text = value.ToString();
				this.confirm.interactable = (value > 0f);
			});
			this.inputField.onValueChanged.ResetListener(new Action<string>(this.OnEndEdit));
		}

		// Token: 0x0600A17D RID: 41341 RVA: 0x004B76F8 File Offset: 0x004B58F8
		public void Show()
		{
			int leftDays = SingletonObject.getInstance<TimeManager>().GetRemainingActionPointConvertToDays();
			this.slider.maxValue = (float)leftDays;
			this.slider.value = (this.slider.minValue = 1f);
			this.inputField.text = "1";
			this.maxValue.SetText("/" + this.slider.maxValue.ToString(), true);
			this.confirm.interactable = true;
		}

		// Token: 0x0600A17E RID: 41342 RVA: 0x004B7787 File Offset: 0x004B5987
		private void Hide()
		{
			base.gameObject.SetActive(false);
		}

		// Token: 0x0600A17F RID: 41343 RVA: 0x004B7798 File Offset: 0x004B5998
		private void SliderAdd()
		{
			this.slider.value = Math.Min(this.slider.value + 1f, this.slider.maxValue);
			this.inputField.text = this.slider.value.ToString();
		}

		// Token: 0x0600A180 RID: 41344 RVA: 0x004B77F4 File Offset: 0x004B59F4
		private void SliderReduce()
		{
			this.slider.value = Math.Max(this.slider.value - 1f, 0f);
			this.inputField.text = this.slider.value.ToString();
		}

		// Token: 0x0600A181 RID: 41345 RVA: 0x004B7848 File Offset: 0x004B5A48
		private void OnEndEdit(string value)
		{
			int result;
			int.TryParse(value, out result);
			result = (int)Math.Clamp((float)result, 1f, this.slider.maxValue);
			this.slider.SetValueWithoutNotify((float)result);
			this.inputField.SetTextWithoutNotify(result.ToString());
			this.confirm.interactable = (result > 0);
		}

		// Token: 0x0600A182 RID: 41346 RVA: 0x004B78A9 File Offset: 0x004B5AA9
		private void Confirm()
		{
			this.Hide();
			GEvent.OnEvent(UiEvents.AdventureAdvanceDaysSet, EasyPool.Get<ArgumentBox>().Set("AdvanceDays", (int)this.slider.value));
		}

		// Token: 0x04007D41 RID: 32065
		[SerializeField]
		private CSlider slider;

		// Token: 0x04007D42 RID: 32066
		[SerializeField]
		private CButton confirm;

		// Token: 0x04007D43 RID: 32067
		[SerializeField]
		private CButton cancel;

		// Token: 0x04007D44 RID: 32068
		[SerializeField]
		private CButton cancelPanel;

		// Token: 0x04007D45 RID: 32069
		[SerializeField]
		private CButton sliderAdd;

		// Token: 0x04007D46 RID: 32070
		[SerializeField]
		private CButton sliderReduce;

		// Token: 0x04007D47 RID: 32071
		[SerializeField]
		private TextMeshProUGUI maxValue;

		// Token: 0x04007D48 RID: 32072
		[SerializeField]
		private TMP_InputField inputField;
	}
}
