using System;
using TMPro;

// Token: 0x02000341 RID: 833
public class CommonChooseAmount : Refers
{
	// Token: 0x1700055B RID: 1371
	// (set) Token: 0x060030DE RID: 12510 RVA: 0x0017FB00 File Offset: 0x0017DD00
	public int MaxValue
	{
		set
		{
			this.max.text = (this._needDisplayStringForNum ? (CommonUtils.GetDisplayStringForNum(value, 100000) + "/") : string.Format("{0}/", value));
			this.slider.maxValue = (float)value;
		}
	}

	// Token: 0x1700055C RID: 1372
	// (set) Token: 0x060030DF RID: 12511 RVA: 0x0017FB57 File Offset: 0x0017DD57
	public int MinValue
	{
		set
		{
			this.slider.minValue = (float)value;
		}
	}

	// Token: 0x1700055D RID: 1373
	// (set) Token: 0x060030E0 RID: 12512 RVA: 0x0017FB68 File Offset: 0x0017DD68
	public int CurrValue
	{
		set
		{
			bool flag = this._currValue == value;
			if (!flag)
			{
				this._currValue = value;
				this.inputField.text = value.ToString();
				this.slider.value = (float)value;
			}
		}
	}

	// Token: 0x060030E1 RID: 12513 RVA: 0x0017FBAD File Offset: 0x0017DDAD
	public void Init(int maxValue, int minValue = 1, int currValue = 1, bool needDisplayStringForNum = false)
	{
		this._needDisplayStringForNum = needDisplayStringForNum;
		this.MaxValue = maxValue;
		this.MinValue = minValue;
		this.CurrValue = currValue;
	}

	// Token: 0x060030E2 RID: 12514 RVA: 0x0017FBD0 File Offset: 0x0017DDD0
	public void OnSliderValueChanged()
	{
		this.CurrValue = (int)this.slider.value;
	}

	// Token: 0x060030E3 RID: 12515 RVA: 0x0017FBE8 File Offset: 0x0017DDE8
	public void OnInputFieldValueChanged()
	{
		int value;
		bool flag = int.TryParse(this.inputField.text, out value);
		if (flag)
		{
			this.CurrValue = value;
		}
	}

	// Token: 0x040023B6 RID: 9142
	public TextMeshProUGUI max;

	// Token: 0x040023B7 RID: 9143
	public TMP_InputField inputField;

	// Token: 0x040023B8 RID: 9144
	public CSliderLegacy slider;

	// Token: 0x040023B9 RID: 9145
	private int _currValue;

	// Token: 0x040023BA RID: 9146
	private bool _needDisplayStringForNum;
}
