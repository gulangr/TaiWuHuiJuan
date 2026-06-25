using System;
using Config;
using GameData.Domains.Character;
using TMPro;
using UnityEngine;

// Token: 0x020001D0 RID: 464
public class QiNumber : MonoBehaviour
{
	// Token: 0x06001CCB RID: 7371 RVA: 0x000C9C27 File Offset: 0x000C7E27
	public void Init(QiDisorderEffectItem config, QiNumber.EQiNumberState state = QiNumber.EQiNumberState.Value)
	{
		this.Number = (int)config.ThresholdMin;
		this.State = state;
		this.ResetLocation();
	}

	// Token: 0x06001CCC RID: 7372 RVA: 0x000C9C44 File Offset: 0x000C7E44
	public void ResetLocation()
	{
		Transform textTransform = this.TextTransform;
		Transform selfTransform = this.SelfTransform;
		Vector3 localScale = new Vector3((this.State == QiNumber.EQiNumberState.End) ? -1f : 1f, 1f, 1f);
		selfTransform.localScale = localScale;
		textTransform.localScale = localScale;
		RectTransform selfTransform2 = this.SelfTransform;
		QiNumber.EQiNumberState state = this.State;
		if (!true)
		{
		}
		ValueTuple<Vector2, int> valueTuple;
		if (state != QiNumber.EQiNumberState.Start)
		{
			if (state != QiNumber.EQiNumberState.End)
			{
				valueTuple = new ValueTuple<Vector2, int>(new Vector2((float)this.Number / (float)DisorderLevelOfQi.MaxValue, 0.5f), this.Number);
			}
			else
			{
				valueTuple = new ValueTuple<Vector2, int>(new Vector2(1f, 0.5f), (int)DisorderLevelOfQi.MaxValue);
			}
		}
		else
		{
			valueTuple = new ValueTuple<Vector2, int>(new Vector2(0f, 0.5f), 0);
		}
		if (!true)
		{
		}
		ValueTuple<Vector2, int> valueTuple2 = valueTuple;
		selfTransform2.anchorMin = valueTuple2.Item1;
		this.Number = valueTuple2.Item2;
		this.Text.text = string.Format("{0}", this.Number / 10);
		this.SelfTransform.anchorMax = this.SelfTransform.anchorMin;
		this.Self.enabled = (this.State == QiNumber.EQiNumberState.Value);
		this.Background.sprite = ((this.State == QiNumber.EQiNumberState.Value) ? this.Value : this.NonValue);
	}

	// Token: 0x06001CCD RID: 7373 RVA: 0x000C9DA1 File Offset: 0x000C7FA1
	public void ApplyValue(int value)
	{
		this.Self.sprite = ((value < this.Number) ? this.UnderControl : this.Overflow);
	}

	// Token: 0x0400165E RID: 5726
	public int Number;

	// Token: 0x0400165F RID: 5727
	public QiNumber.EQiNumberState State = QiNumber.EQiNumberState.Value;

	// Token: 0x04001660 RID: 5728
	public CImage Self;

	// Token: 0x04001661 RID: 5729
	public CImage Background;

	// Token: 0x04001662 RID: 5730
	public RectTransform SelfTransform;

	// Token: 0x04001663 RID: 5731
	public RectTransform TextTransform;

	// Token: 0x04001664 RID: 5732
	public TextMeshProUGUI Text;

	// Token: 0x04001665 RID: 5733
	public Sprite Overflow;

	// Token: 0x04001666 RID: 5734
	public Sprite UnderControl;

	// Token: 0x04001667 RID: 5735
	public Sprite Value;

	// Token: 0x04001668 RID: 5736
	public Sprite NonValue;

	// Token: 0x04001669 RID: 5737
	public TextStyle TextStyle;

	// Token: 0x020013DE RID: 5086
	public enum EQiNumberState
	{
		// Token: 0x04009F34 RID: 40756
		Start,
		// Token: 0x04009F35 RID: 40757
		Value,
		// Token: 0x04009F36 RID: 40758
		End
	}
}
