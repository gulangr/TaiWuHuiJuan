using System;
using UnityEngine.EventSystems;
using UnityEngine.UI;

// Token: 0x020000D2 RID: 210
[Obsolete]
public class CSliderLegacy : Slider
{
	// Token: 0x170000B6 RID: 182
	// (get) Token: 0x0600076B RID: 1899 RVA: 0x000346AA File Offset: 0x000328AA
	// (set) Token: 0x0600076C RID: 1900 RVA: 0x000346B2 File Offset: 0x000328B2
	public new bool interactable
	{
		get
		{
			return base.interactable;
		}
		set
		{
			base.interactable = value;
			CButtonObsolete.ButtonEvent onInteractableChange = this.OnInteractableChange;
			if (onInteractableChange != null)
			{
				onInteractableChange.Invoke(value);
			}
			this.OnInteractableChangeInternal(value);
			CButtonObsolete.ButtonEvent onInteractableChangeReverse = this.OnInteractableChangeReverse;
			if (onInteractableChangeReverse != null)
			{
				onInteractableChangeReverse.Invoke(!value);
			}
		}
	}

	// Token: 0x0600076D RID: 1901 RVA: 0x000346EE File Offset: 0x000328EE
	protected virtual void OnInteractableChangeInternal(bool value)
	{
	}

	// Token: 0x0600076E RID: 1902 RVA: 0x000346F4 File Offset: 0x000328F4
	protected override void Start()
	{
		base.Start();
		bool flag = null != this.BtnMin;
		if (flag)
		{
			this.BtnMin.ClearAndAddListener(delegate
			{
				this.value = base.minValue;
			});
		}
		bool flag2 = null != this.BtnMax;
		if (flag2)
		{
			this.BtnMax.ClearAndAddListener(delegate
			{
				this.value = base.maxValue;
			});
		}
		base.onValueChanged.AddListener(delegate(float newValue)
		{
			bool flag3 = null != this.BtnMax;
			if (flag3)
			{
				this.BtnMax.interactable = (newValue < base.maxValue);
			}
			bool flag4 = null != this.BtnMin;
			if (flag4)
			{
				this.BtnMin.interactable = (newValue > base.minValue);
			}
		});
	}

	// Token: 0x0600076F RID: 1903 RVA: 0x00034771 File Offset: 0x00032971
	public override void OnPointerDown(PointerEventData eventData)
	{
		base.OnPointerDown(eventData);
		AudioManager.Instance.PlaySound(this.ClickAudioKey, false, false);
	}

	// Token: 0x06000770 RID: 1904 RVA: 0x0003478F File Offset: 0x0003298F
	public override void OnPointerUp(PointerEventData eventData)
	{
		base.OnPointerUp(eventData);
	}

	// Token: 0x06000771 RID: 1905 RVA: 0x0003479C File Offset: 0x0003299C
	public void UpdateInteractableByValueRange()
	{
		bool result = base.minValue < base.maxValue;
		this.interactable = result;
		bool flag = null != this.BtnMin;
		if (flag)
		{
			this.BtnMin.interactable = (result && this.value > base.minValue);
		}
		bool flag2 = null != this.BtnMax;
		if (flag2)
		{
			this.BtnMax.interactable = (result && this.value < base.maxValue);
		}
	}

	// Token: 0x040007AA RID: 1962
	public string ClickAudioKey;

	// Token: 0x040007AB RID: 1963
	public CButtonObsolete BtnMin;

	// Token: 0x040007AC RID: 1964
	public CButtonObsolete BtnMax;

	// Token: 0x040007AD RID: 1965
	public CButtonObsolete.ButtonEvent OnInteractableChange;

	// Token: 0x040007AE RID: 1966
	public CButtonObsolete.ButtonEvent OnInteractableChangeReverse;
}
