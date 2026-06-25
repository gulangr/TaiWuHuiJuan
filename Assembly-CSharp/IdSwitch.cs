using System;
using GameData.Domains.Character.AvatarSystem;
using TMPro;
using UnityEngine;
using UnityEngine.Events;

// Token: 0x02000069 RID: 105
public class IdSwitch : MonoBehaviour
{
	// Token: 0x06000399 RID: 921 RVA: 0x00016567 File Offset: 0x00014767
	public void Init(int value, int maxValue, int minValue = 1)
	{
		this.MaxValue = maxValue;
		this.MinValue = minValue;
		this.SetValueAndRefresh(value);
	}

	// Token: 0x0600039A RID: 922 RVA: 0x00016580 File Offset: 0x00014780
	public void Refresh()
	{
		bool flag = null != this.IdValue;
		if (flag)
		{
			TMP_Text idValue = this.IdValue;
			Func<int, int, string> styleCheck = this.StyleCheck;
			idValue.text = (((styleCheck != null) ? styleCheck(this.Value, this.MaxValue) : null) ?? string.Format("{0}/{1}", this.Value, this.MaxValue));
		}
	}

	// Token: 0x0600039B RID: 923 RVA: 0x000165EC File Offset: 0x000147EC
	public void SetValueAndRefresh(int value)
	{
		this.Value = Mathf.Clamp(value, this.MinValue, this.MaxValue);
		this.Refresh();
		this.UpdateBtnInteractable();
	}

	// Token: 0x0600039C RID: 924 RVA: 0x00016618 File Offset: 0x00014818
	public void ResetBtnEvents()
	{
		bool flag = null != this.BtnPrevId;
		if (flag)
		{
			this.BtnPrevId.ClearAndAddListener(new Action(this.OnIdPrev));
		}
		bool flag2 = null != this.BtnNextId;
		if (flag2)
		{
			this.BtnNextId.ClearAndAddListener(new Action(this.OnIdNext));
		}
		bool flag3 = null != this.BtnMax;
		if (flag3)
		{
			this.BtnMax.ClearAndAddListener(new Action(this.OnIdMax));
		}
		bool flag4 = null != this.BtnMin;
		if (flag4)
		{
			this.BtnMin.ClearAndAddListener(new Action(this.OnIdMin));
		}
		bool flag5 = null != this.FeatureLeftToggle;
		if (flag5)
		{
			this.FeatureLeftToggle.onValueChanged.RemoveAllListeners();
			this.FeatureLeftToggle.onValueChanged.AddListener(new UnityAction<bool>(this.OnLeftToggleChange));
		}
		bool flag6 = null != this.FeatureRightToggle;
		if (flag6)
		{
			this.FeatureRightToggle.onValueChanged.RemoveAllListeners();
			this.FeatureRightToggle.onValueChanged.AddListener(new UnityAction<bool>(this.OnRightToggleChange));
		}
	}

	// Token: 0x0600039D RID: 925 RVA: 0x0001674A File Offset: 0x0001494A
	private void Start()
	{
		this.ResetBtnEvents();
	}

	// Token: 0x0600039E RID: 926 RVA: 0x00016754 File Offset: 0x00014954
	private void OnIdPrev()
	{
		bool flag = !this.Loop;
		int newValue;
		if (flag)
		{
			newValue = Mathf.Clamp(this.Value - 1, this.MinValue, this.MaxValue);
		}
		else
		{
			newValue = ((this.Value - 1 >= this.MinValue) ? (this.Value - 1) : this.MaxValue);
		}
		bool flag2 = newValue != this.Value;
		if (flag2)
		{
			this.Value = newValue;
			this.Refresh();
			Action<int> onValueChanged = this.OnValueChanged;
			if (onValueChanged != null)
			{
				onValueChanged(this.Value);
			}
			this.UpdateBtnInteractable();
		}
	}

	// Token: 0x0600039F RID: 927 RVA: 0x000167EC File Offset: 0x000149EC
	private void OnIdNext()
	{
		bool flag = !this.Loop;
		int newValue;
		if (flag)
		{
			newValue = Mathf.Clamp(this.Value + 1, this.MinValue, this.MaxValue);
		}
		else
		{
			newValue = ((this.Value + 1 <= this.MaxValue) ? (this.Value + 1) : this.MinValue);
		}
		bool flag2 = newValue != this.Value;
		if (flag2)
		{
			this.Value = newValue;
			this.Refresh();
			Action<int> onValueChanged = this.OnValueChanged;
			if (onValueChanged != null)
			{
				onValueChanged(this.Value);
			}
			this.UpdateBtnInteractable();
		}
	}

	// Token: 0x060003A0 RID: 928 RVA: 0x00016884 File Offset: 0x00014A84
	private void OnIdMin()
	{
		bool flag = this.MinValue != this.Value;
		if (flag)
		{
			this.Value = this.MinValue;
			this.Refresh();
			Action<int> onValueChanged = this.OnValueChanged;
			if (onValueChanged != null)
			{
				onValueChanged(this.Value);
			}
			this.UpdateBtnInteractable();
		}
	}

	// Token: 0x060003A1 RID: 929 RVA: 0x000168DC File Offset: 0x00014ADC
	private void OnIdMax()
	{
		bool flag = this.MaxValue != this.Value;
		if (flag)
		{
			this.Value = this.MaxValue;
			this.Refresh();
			Action<int> onValueChanged = this.OnValueChanged;
			if (onValueChanged != null)
			{
				onValueChanged(this.Value);
			}
			this.UpdateBtnInteractable();
		}
	}

	// Token: 0x060003A2 RID: 930 RVA: 0x00016934 File Offset: 0x00014B34
	private void UpdateBtnInteractable()
	{
		bool flag = null != this.BtnPrevId;
		if (flag)
		{
			this.BtnPrevId.interactable = (this.Loop || this.Value > this.MinValue);
		}
		bool flag2 = null != this.BtnNextId;
		if (flag2)
		{
			this.BtnNextId.interactable = (this.Loop || this.Value < this.MaxValue);
		}
		bool flag3 = null != this.BtnMin;
		if (flag3)
		{
			this.BtnMin.interactable = (this.Value > this.MinValue);
		}
		bool flag4 = null != this.BtnMax;
		if (flag4)
		{
			this.BtnMax.interactable = (this.Value < this.MaxValue);
		}
	}

	// Token: 0x060003A3 RID: 931 RVA: 0x00016A00 File Offset: 0x00014C00
	private void OnLeftToggleChange(bool isOn)
	{
		bool flag = !this.FeatureRightToggle.isOn && !isOn;
		if (flag)
		{
			this.FeatureLeftToggle.onValueChanged.RemoveAllListeners();
			this.FeatureLeftToggle.isOn = true;
			this.FeatureLeftToggle.onValueChanged.AddListener(new UnityAction<bool>(this.OnLeftToggleChange));
		}
		else
		{
			this.FeatureLeftToggle.isOn = isOn;
			this.InvokeToggleValueChanged();
		}
	}

	// Token: 0x060003A4 RID: 932 RVA: 0x00016A78 File Offset: 0x00014C78
	private void OnRightToggleChange(bool isOn)
	{
		bool flag = !this.FeatureLeftToggle.isOn && !isOn;
		if (flag)
		{
			this.FeatureRightToggle.onValueChanged.RemoveAllListeners();
			this.FeatureRightToggle.isOn = true;
			this.FeatureRightToggle.onValueChanged.AddListener(new UnityAction<bool>(this.OnRightToggleChange));
		}
		else
		{
			this.FeatureRightToggle.isOn = isOn;
			this.InvokeToggleValueChanged();
		}
	}

	// Token: 0x060003A5 RID: 933 RVA: 0x00016AF0 File Offset: 0x00014CF0
	private void InvokeToggleValueChanged()
	{
		sbyte featureMirrorType = AvatarFeatureMirrorType.ToggleToType(this.FeatureLeftToggle.isOn, this.FeatureRightToggle.isOn);
		Action<sbyte> onToggleValueChanged = this.OnToggleValueChanged;
		if (onToggleValueChanged != null)
		{
			onToggleValueChanged(featureMirrorType);
		}
	}

	// Token: 0x0400022C RID: 556
	public int Value;

	// Token: 0x0400022D RID: 557
	public int MaxValue;

	// Token: 0x0400022E RID: 558
	public int MinValue;

	// Token: 0x0400022F RID: 559
	public bool Loop;

	// Token: 0x04000230 RID: 560
	public Action<int> OnValueChanged;

	// Token: 0x04000231 RID: 561
	public Action<sbyte> OnToggleValueChanged;

	// Token: 0x04000232 RID: 562
	public Func<int, int, string> StyleCheck;

	// Token: 0x04000233 RID: 563
	public TextMeshProUGUI IdValue;

	// Token: 0x04000234 RID: 564
	public CButtonObsolete BtnPrevId;

	// Token: 0x04000235 RID: 565
	public CButtonObsolete BtnNextId;

	// Token: 0x04000236 RID: 566
	public CButtonObsolete BtnMin;

	// Token: 0x04000237 RID: 567
	public CButtonObsolete BtnMax;

	// Token: 0x04000238 RID: 568
	public CToggleObsolete FeatureLeftToggle;

	// Token: 0x04000239 RID: 569
	public CToggleObsolete FeatureRightToggle;
}
