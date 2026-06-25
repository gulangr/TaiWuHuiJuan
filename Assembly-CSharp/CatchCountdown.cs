using System;
using DG.Tweening;
using FrameWork.UISystem.Components;
using UnityEngine;

// Token: 0x020001C0 RID: 448
public class CatchCountdown : MonoBehaviour
{
	// Token: 0x170002D7 RID: 727
	// (get) Token: 0x06001BD6 RID: 7126 RVA: 0x000C08C5 File Offset: 0x000BEAC5
	private CanvasGroup EffectCanvas
	{
		get
		{
			return this.effect.GetComponent<CanvasGroup>();
		}
	}

	// Token: 0x06001BD7 RID: 7127 RVA: 0x000C08D2 File Offset: 0x000BEAD2
	private bool IsNearbyEnd(int countdown)
	{
		return countdown <= 10;
	}

	// Token: 0x06001BD8 RID: 7128 RVA: 0x000C08DC File Offset: 0x000BEADC
	public void Set(int countdown)
	{
		bool flag = this._lastCountdown == countdown;
		if (!flag)
		{
			this.normal.Set(countdown);
			this.effect.Set(countdown);
			bool flag2 = this.IsNearbyEnd(this._lastCountdown) != this.IsNearbyEnd(countdown);
			if (flag2)
			{
				this.UpdateColor(this.IsNearbyEnd(countdown));
			}
			this.PlayEffect(this.IsNearbyEnd(countdown));
			this._lastCountdown = countdown;
		}
	}

	// Token: 0x06001BD9 RID: 7129 RVA: 0x000C0954 File Offset: 0x000BEB54
	private void UpdateColor(bool isNearbyEnd)
	{
		Color color = isNearbyEnd ? "#f66751".HexStringToColor() : Color.white;
		this.normal.SetColor(color);
		this.effect.SetColor(color);
	}

	// Token: 0x06001BDA RID: 7130 RVA: 0x000C0994 File Offset: 0x000BEB94
	private void PlayEffect(bool isNearbyEnd)
	{
		this.ClearEffect();
		float scaleRate = isNearbyEnd ? 1.5f : 1f;
		this.normal.transform.localScale = Vector3.one * scaleRate;
		this.effect.transform.localScale = Vector3.one * scaleRate;
		bool flag = !isNearbyEnd;
		if (!flag)
		{
			this.EffectCanvas.alpha = 1f;
			this.EffectCanvas.DOFade(0f, 0.4f);
			this.effect.transform.DOScale(3f * scaleRate, 0.4f);
		}
	}

	// Token: 0x06001BDB RID: 7131 RVA: 0x000C0A3F File Offset: 0x000BEC3F
	private void ClearEffect()
	{
		this.EffectCanvas.DOKill(false);
		this.EffectCanvas.alpha = 0f;
		this.effect.transform.DOKill(false);
	}

	// Token: 0x040015B5 RID: 5557
	[SerializeField]
	private ImageNumber normal;

	// Token: 0x040015B6 RID: 5558
	[SerializeField]
	private ImageNumber effect;

	// Token: 0x040015B7 RID: 5559
	private int _lastCountdown;
}
