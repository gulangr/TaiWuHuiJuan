using System;
using FrameWork;
using TMPro;
using UnityEngine;

// Token: 0x02000393 RID: 915
public class UI_NumberSetter : UIBase
{
	// Token: 0x060036A4 RID: 13988 RVA: 0x001B7C4C File Offset: 0x001B5E4C
	public override void OnInit(ArgumentBox argsBox)
	{
		int min;
		argsBox.Get("Min", out min);
		int max;
		argsBox.Get("Max", out max);
		int cur;
		argsBox.Get("Cur", out cur);
		argsBox.Get<Action>("CloseAction", out this._closeCallBack);
		argsBox.Get<Action<int>>("ConfirmAction", out this._confirmCallBack);
		argsBox.Get<Action<int>>("ValueChangeAction", out this._onValueChange);
		bool flag = !argsBox.Get("DisplayMultiplier", out this._displayMultiplier);
		if (flag)
		{
			this._displayMultiplier = 1;
		}
		bool flag2 = min > max;
		if (flag2)
		{
			Debug.LogError(string.Format("min({0}) greater than max({1})", min, max));
		}
		cur = Mathf.Clamp(cur, min, max);
		this.Slider.minValue = (float)min;
		this.Slider.maxValue = (float)max;
		this.Slider.value = (float)cur;
		this.ValueLabel.text = (cur * this._displayMultiplier).ToString();
		this.Slider.onValueChanged.RemoveAllListeners();
		this.Slider.onValueChanged.AddListener(delegate(float value)
		{
			this.ValueLabel.text = ((int)value * this._displayMultiplier).ToString();
		});
		bool flag3 = this._onValueChange != null;
		if (flag3)
		{
			this.Slider.onValueChanged.AddListener(delegate(float value)
			{
				this._onValueChange((int)value);
			});
		}
		this.ValueLabel.onEndEdit.RemoveAllListeners();
		this.ValueLabel.onEndEdit.AddListener(delegate(string str)
		{
			int value = int.Parse(str) / this._displayMultiplier;
			this.Slider.value = (float)Mathf.Clamp(value, min, max);
		});
	}

	// Token: 0x060036A5 RID: 13989 RVA: 0x001B7E12 File Offset: 0x001B6012
	public void OnClickClose()
	{
		Action closeCallBack = this._closeCallBack;
		if (closeCallBack != null)
		{
			closeCallBack();
		}
		UIManager.Instance.HideUI(this.Element);
	}

	// Token: 0x060036A6 RID: 13990 RVA: 0x001B7E38 File Offset: 0x001B6038
	public void OnClickConfirm()
	{
		Action<int> confirmCallBack = this._confirmCallBack;
		if (confirmCallBack != null)
		{
			confirmCallBack((int)this.Slider.value);
		}
		UIManager.Instance.HideUI(this.Element);
	}

	// Token: 0x040027A0 RID: 10144
	public CSliderLegacy Slider;

	// Token: 0x040027A1 RID: 10145
	public TMP_InputField ValueLabel;

	// Token: 0x040027A2 RID: 10146
	private Action _closeCallBack;

	// Token: 0x040027A3 RID: 10147
	private Action<int> _confirmCallBack;

	// Token: 0x040027A4 RID: 10148
	private Action<int> _onValueChange;

	// Token: 0x040027A5 RID: 10149
	private int _displayMultiplier;
}
