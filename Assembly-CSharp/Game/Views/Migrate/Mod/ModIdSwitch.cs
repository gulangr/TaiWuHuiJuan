using System;
using FrameWork.UISystem.UIElements;
using GameData.Domains.Character.AvatarSystem;
using TMPro;
using UnityEngine;
using UnityEngine.Events;

namespace Game.Views.Migrate.Mod
{
	// Token: 0x0200091A RID: 2330
	public class ModIdSwitch : MonoBehaviour
	{
		// Token: 0x06006DA1 RID: 28065 RVA: 0x0032B303 File Offset: 0x00329503
		public void Init(int value, int maxValue, int minValue = 1)
		{
			this.MaxValue = maxValue;
			this.MinValue = minValue;
			this.SetValueAndRefresh(value);
		}

		// Token: 0x06006DA2 RID: 28066 RVA: 0x0032B31C File Offset: 0x0032951C
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

		// Token: 0x06006DA3 RID: 28067 RVA: 0x0032B388 File Offset: 0x00329588
		public void SetValueAndRefresh(int value)
		{
			this.Value = Mathf.Clamp(value, this.MinValue, this.MaxValue);
			this.Refresh();
			this.UpdateBtnInteractable();
		}

		// Token: 0x06006DA4 RID: 28068 RVA: 0x0032B3B4 File Offset: 0x003295B4
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

		// Token: 0x06006DA5 RID: 28069 RVA: 0x0032B4E6 File Offset: 0x003296E6
		private void Start()
		{
			this.ResetBtnEvents();
		}

		// Token: 0x06006DA6 RID: 28070 RVA: 0x0032B4F0 File Offset: 0x003296F0
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

		// Token: 0x06006DA7 RID: 28071 RVA: 0x0032B588 File Offset: 0x00329788
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

		// Token: 0x06006DA8 RID: 28072 RVA: 0x0032B620 File Offset: 0x00329820
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

		// Token: 0x06006DA9 RID: 28073 RVA: 0x0032B678 File Offset: 0x00329878
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

		// Token: 0x06006DAA RID: 28074 RVA: 0x0032B6D0 File Offset: 0x003298D0
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

		// Token: 0x06006DAB RID: 28075 RVA: 0x0032B79C File Offset: 0x0032999C
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

		// Token: 0x06006DAC RID: 28076 RVA: 0x0032B814 File Offset: 0x00329A14
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

		// Token: 0x06006DAD RID: 28077 RVA: 0x0032B88C File Offset: 0x00329A8C
		private void InvokeToggleValueChanged()
		{
			sbyte featureMirrorType = AvatarFeatureMirrorType.ToggleToType(this.FeatureLeftToggle.isOn, this.FeatureRightToggle.isOn);
			Action<sbyte> onToggleValueChanged = this.OnToggleValueChanged;
			if (onToggleValueChanged != null)
			{
				onToggleValueChanged(featureMirrorType);
			}
		}

		// Token: 0x040050C6 RID: 20678
		public int Value;

		// Token: 0x040050C7 RID: 20679
		public int MaxValue;

		// Token: 0x040050C8 RID: 20680
		public int MinValue;

		// Token: 0x040050C9 RID: 20681
		public bool Loop;

		// Token: 0x040050CA RID: 20682
		public Action<int> OnValueChanged;

		// Token: 0x040050CB RID: 20683
		public Action<sbyte> OnToggleValueChanged;

		// Token: 0x040050CC RID: 20684
		public Func<int, int, string> StyleCheck;

		// Token: 0x040050CD RID: 20685
		public TextMeshProUGUI IdValue;

		// Token: 0x040050CE RID: 20686
		public CButton BtnPrevId;

		// Token: 0x040050CF RID: 20687
		public CButton BtnNextId;

		// Token: 0x040050D0 RID: 20688
		public CButton BtnMin;

		// Token: 0x040050D1 RID: 20689
		public CButton BtnMax;

		// Token: 0x040050D2 RID: 20690
		public CToggle FeatureLeftToggle;

		// Token: 0x040050D3 RID: 20691
		public CToggle FeatureRightToggle;
	}
}
