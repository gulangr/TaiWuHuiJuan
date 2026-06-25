using System;
using TMPro;
using UnityEngine;
using UnityEngine.Events;

// Token: 0x02000206 RID: 518
public class BarWithGrayBg : MonoBehaviour
{
	// Token: 0x17000353 RID: 851
	// (get) Token: 0x0600210A RID: 8458 RVA: 0x000F0794 File Offset: 0x000EE994
	public float BgWidth
	{
		get
		{
			return this.bg.rect.width;
		}
	}

	// Token: 0x17000354 RID: 852
	// (get) Token: 0x0600210B RID: 8459 RVA: 0x000F07B4 File Offset: 0x000EE9B4
	// (set) Token: 0x0600210C RID: 8460 RVA: 0x000F07E2 File Offset: 0x000EE9E2
	public int Value
	{
		get
		{
			return (int)Math.Min(this.cs.value * (this.selectable + 1U) + (float)this.minValue, this.selectable);
		}
		set
		{
			this.cs.value = Math.Clamp((float)(value - this.minValue) / Math.Max(1f, this.selectable), 0f, 1f);
		}
	}

	// Token: 0x0600210D RID: 8461 RVA: 0x000F081B File Offset: 0x000EEA1B
	public void OnEnable()
	{
		this._oldValue = -1;
	}

	// Token: 0x0600210E RID: 8462 RVA: 0x000F0824 File Offset: 0x000EEA24
	public void RefreshGrayArea()
	{
		uint sum = this.lVal + this.rVal + this.selectable;
		bool flag = sum > 0U;
		if (flag)
		{
			this.bar.gameObject.SetActive(true);
			this.bar.anchorMin = this.bar.anchorMin.SetX(this.lVal / sum);
			this.bar.anchorMax = this.bar.anchorMin.SetX(1f - this.rVal / sum);
		}
		else
		{
			this.bar.gameObject.SetActive(false);
		}
	}

	// Token: 0x0600210F RID: 8463 RVA: 0x000F08D0 File Offset: 0x000EEAD0
	public void UpdateValue(float value)
	{
		bool updating = this._updating;
		if (!updating)
		{
			this._updating = true;
			value = Math.Max(Math.Clamp((float)(this.start - this.minValue) / Math.Max(1f, this.selectable), 0f, 1f), value);
			bool flag = !Mathf.Approximately(this.cs.value, value);
			if (flag)
			{
				this.cs.value = value;
			}
			bool flag2 = this.Value < this.start;
			if (flag2)
			{
				this.Value = this.start;
			}
			bool flag3 = this._oldValue != this.Value;
			if (flag3)
			{
				this._oldValue = this.Value;
				UnityEvent<int> unityEvent = this.onValueChanged;
				if (unityEvent != null)
				{
					unityEvent.Invoke(this.Value);
				}
			}
			bool flag4 = this.inputField;
			if (flag4)
			{
				this.inputField.SetTextWithoutNotify(this._oldValue.ToString());
			}
			bool flag5 = this.dec;
			if (flag5)
			{
				this.dec.interactable = (this._oldValue > 1);
			}
			bool flag6 = this.inc;
			if (flag6)
			{
				this.inc.interactable = ((long)this._oldValue < (long)((ulong)this.selectable));
			}
			this._updating = false;
		}
	}

	// Token: 0x06002110 RID: 8464 RVA: 0x000F0A30 File Offset: 0x000EEC30
	public void UpdateValue(int value, bool force = false)
	{
		bool flag = this._updating || (this.Value == value && !force);
		if (!flag)
		{
			this._updating = true;
			bool flag2 = this.selectable > 0U;
			if (flag2)
			{
				value = (this.Value = Math.Clamp(value, this.start, this.start + (int)this.selectable - 1));
			}
			bool flag3 = this._oldValue != value || force;
			if (flag3)
			{
				this._oldValue = value;
				UnityEvent<int> unityEvent = this.onValueChanged;
				if (unityEvent != null)
				{
					unityEvent.Invoke(value);
				}
			}
			bool flag4 = this.inputField;
			if (flag4)
			{
				this.inputField.SetTextWithoutNotify(this._oldValue.ToString());
			}
			bool flag5 = this.dec;
			if (flag5)
			{
				this.dec.interactable = (value > 1);
			}
			bool flag6 = this.inc;
			if (flag6)
			{
				this.inc.interactable = ((long)value < (long)((ulong)this.selectable));
			}
			this._updating = false;
		}
	}

	// Token: 0x06002111 RID: 8465 RVA: 0x000F0B40 File Offset: 0x000EED40
	public void UpdateValue(string value)
	{
		int val;
		bool flag = int.TryParse(value, out val);
		if (flag)
		{
			this.UpdateValue(val, false);
		}
		else
		{
			this._oldValue = -1;
			this.UpdateValue(this.Value, false);
		}
	}

	// Token: 0x06002112 RID: 8466 RVA: 0x000F0B7C File Offset: 0x000EED7C
	public void Inc()
	{
		this.Value++;
	}

	// Token: 0x06002113 RID: 8467 RVA: 0x000F0B8D File Offset: 0x000EED8D
	public void Dec()
	{
		this.Value--;
	}

	// Token: 0x06002114 RID: 8468 RVA: 0x000F0B9E File Offset: 0x000EED9E
	public void SetValues(int left, int middle, int right, int startVal = 1)
	{
		this.lVal = (uint)Math.Max(0, left);
		this.selectable = (uint)Math.Max(0, middle);
		this.rVal = (uint)Math.Max(0, right);
		this.start = startVal;
		this.RefreshGrayArea();
	}

	// Token: 0x04001985 RID: 6533
	[SerializeField]
	private RectTransform bg;

	// Token: 0x04001986 RID: 6534
	[SerializeField]
	private RectTransform bar;

	// Token: 0x04001987 RID: 6535
	[SerializeField]
	private CSliderLegacy cs;

	// Token: 0x04001988 RID: 6536
	[SerializeField]
	private uint lVal;

	// Token: 0x04001989 RID: 6537
	[SerializeField]
	private uint rVal;

	// Token: 0x0400198A RID: 6538
	[SerializeField]
	private uint selectable;

	// Token: 0x0400198B RID: 6539
	[SerializeField]
	private int start;

	// Token: 0x0400198C RID: 6540
	[SerializeField]
	private int minValue = 0;

	// Token: 0x0400198D RID: 6541
	[SerializeField]
	private TMP_InputField inputField;

	// Token: 0x0400198E RID: 6542
	[SerializeField]
	private CButtonObsolete dec;

	// Token: 0x0400198F RID: 6543
	[SerializeField]
	private CButtonObsolete inc;

	// Token: 0x04001990 RID: 6544
	private bool _updating;

	// Token: 0x04001991 RID: 6545
	private int _oldValue;

	// Token: 0x04001992 RID: 6546
	public UnityEvent<int> onValueChanged;
}
