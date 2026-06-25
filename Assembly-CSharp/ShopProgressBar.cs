using System;
using System.Linq;
using System.Text;
using Config;
using FrameWork;
using GameData.Domains.Merchant;
using UnityEngine;

// Token: 0x020002FF RID: 767
public class ShopProgressBar : MonoBehaviour
{
	// Token: 0x170004E3 RID: 1251
	// (get) Token: 0x06002CE6 RID: 11494 RVA: 0x001615BF File Offset: 0x0015F7BF
	// (set) Token: 0x06002CE7 RID: 11495 RVA: 0x001615C8 File Offset: 0x0015F7C8
	public int PreviewDelta
	{
		get
		{
			return this._previewDelta;
		}
		set
		{
			bool flag = this._previewDelta == value;
			if (!flag)
			{
				this.UpdatePreview(value);
				this._previewDelta = value;
			}
		}
	}

	// Token: 0x170004E4 RID: 1252
	// (get) Token: 0x06002CE8 RID: 11496 RVA: 0x001615F5 File Offset: 0x0015F7F5
	// (set) Token: 0x06002CE9 RID: 11497 RVA: 0x00161604 File Offset: 0x0015F804
	public int PreviewValue
	{
		get
		{
			return this._currentValue + this._previewDelta;
		}
		set
		{
			this.PreviewDelta = value - this._currentValue;
		}
	}

	// Token: 0x170004E5 RID: 1253
	// (get) Token: 0x06002CEA RID: 11498 RVA: 0x00161615 File Offset: 0x0015F815
	// (set) Token: 0x06002CEB RID: 11499 RVA: 0x00161620 File Offset: 0x0015F820
	public int CurrentValue
	{
		get
		{
			return this._currentValue;
		}
		set
		{
			bool flag = this._currentValue == value;
			if (!flag)
			{
				this.RefreshCurrentValue(value);
			}
		}
	}

	// Token: 0x170004E6 RID: 1254
	// (get) Token: 0x06002CEC RID: 11500 RVA: 0x00161648 File Offset: 0x0015F848
	public int ActiveValue
	{
		get
		{
			bool hasValue = this._hasValue;
			int result;
			if (hasValue)
			{
				result = this._activeValue;
			}
			else
			{
				Debug.LogWarning("ProgressBar has not received any CurrentValue yet.");
				result = 0;
			}
			return result;
		}
	}

	// Token: 0x06002CED RID: 11501 RVA: 0x0016167A File Offset: 0x0015F87A
	private void LateUpdate()
	{
		this.BreathEffect(Mathf.Abs(Mathf.Sin(Time.time)));
	}

	// Token: 0x06002CEE RID: 11502 RVA: 0x00161693 File Offset: 0x0015F893
	public void Init(int merchantTemplateId)
	{
		this._currentValue = -1;
		this._hasValue = false;
		this.MerchantTemplateId = merchantTemplateId;
	}

	// Token: 0x06002CEF RID: 11503 RVA: 0x001616AC File Offset: 0x0015F8AC
	public int InitWithIdAndFavor(int merchantTemplateId, int merchantFavorability)
	{
		this.Init(merchantTemplateId);
		this.CurrentValue = merchantFavorability;
		return this.ActiveValue;
	}

	// Token: 0x06002CF0 RID: 11504 RVA: 0x001616D4 File Offset: 0x0015F8D4
	public void RefreshCurrentValue(int value)
	{
		int worldProgressLimitedLevel;
		int worldProgressLimitedFavor;
		UI_Shop.IsReachProgressLimit(value, out worldProgressLimitedLevel, out worldProgressLimitedFavor);
		sbyte b = (sbyte)worldProgressLimitedLevel;
		MerchantItem merchantItem = Merchant.Instance[this.MerchantTemplateId];
		this.ReachProgressLimit = (b < ((merchantItem != null) ? merchantItem.Level : 0));
		this.UpdateCurrentBarAndDot(value, worldProgressLimitedFavor);
		this.UpdateLimitImage(value, worldProgressLimitedFavor);
		this.UpdateTipsDisplayer();
		int currDelta = this.PreviewValue - value;
		this._currentValue = value;
		bool flag = this._previewDelta != 0;
		if (flag)
		{
			this.PreviewDelta = currDelta;
		}
		this._hasValue = true;
	}

	// Token: 0x06002CF1 RID: 11505 RVA: 0x0016175C File Offset: 0x0015F95C
	public void UpdateLimitImage(int value, int worldProgressLimitedFavor)
	{
		bool flag = value > worldProgressLimitedFavor;
		if (flag)
		{
			int favor = Math.Min(value, worldProgressLimitedFavor);
			int level = SharedMethods.GetFavorLevel(favor);
			this.LimitImage.transform.position = this.DotCurrent[level].transform.position;
			this.LimitImage.gameObject.SetActive(true);
		}
		else
		{
			this.LimitImage.gameObject.SetActive(false);
		}
	}

	// Token: 0x06002CF2 RID: 11506 RVA: 0x001617D0 File Offset: 0x0015F9D0
	public void UpdateCurrentBarAndDot(int value, int worldProgressLimitedFavor)
	{
		bool flag = value < worldProgressLimitedFavor;
		if (flag)
		{
			this.ReachProgressLimit = false;
			this._activeValue = value;
			this.BarCurrent.fillAmount = this.GetRealFillAmount(value);
			this.BarActual.gameObject.SetActive(false);
		}
		else
		{
			this.ReachProgressLimit = true;
			this._activeValue = worldProgressLimitedFavor;
			this.BarCurrent.fillAmount = this.GetRealFillAmount(worldProgressLimitedFavor);
			this.BarActual.fillAmount = this.GetRealFillAmount(value);
			this.BarActual.gameObject.SetActive(true);
		}
		int activeLevel = SharedMethods.GetFavorLevel(this._activeValue);
		int level = SharedMethods.GetFavorLevel(value);
		for (int i = 0; i < this.DotCurrent.Length; i++)
		{
			this.DotCurrent[i].sprite = ((i > activeLevel) ? ((i > level) ? this.DotEmpty : this.DotInactive) : this.DotActive);
		}
	}

	// Token: 0x06002CF3 RID: 11507 RVA: 0x001618C0 File Offset: 0x0015FAC0
	private float GetRealFillAmount(int value)
	{
		float remainRate;
		int level = SharedMethods.GetFavorLevel(value, out remainRate);
		return ((float)level + remainRate) / 6f;
	}

	// Token: 0x06002CF4 RID: 11508 RVA: 0x001618E8 File Offset: 0x0015FAE8
	public void UpdateTipsDisplayer()
	{
		StringBuilder stringBuilder = EasyPool.Get<StringBuilder>();
		stringBuilder.AppendLine(LocalStringManager.Get(LanguageKey.LK_Shop_Favor_Tip));
		bool reachProgressLimit = this.ReachProgressLimit;
		if (reachProgressLimit)
		{
			stringBuilder.AppendLine(LocalStringManager.Get(LanguageKey.LK_Shop_Tog_Tip_XiangshuLevelLimit));
		}
		this.TipsDisplayer.PresetParam[0] = stringBuilder.ToString().ColorReplace();
	}

	// Token: 0x06002CF5 RID: 11509 RVA: 0x00161944 File Offset: 0x0015FB44
	public void UpdatePreview(int value)
	{
		bool flag = value == 0;
		if (flag)
		{
			this.PreviewContainer.SetActive(false);
		}
		else
		{
			int targetValue = this._currentValue + value;
			int num = Math.Min(this._currentValue, targetValue);
			int num2 = Math.Max(this._currentValue, targetValue);
			int start = num;
			int end = num2;
			Sprite image = (value > 0) ? this.DotPlus : this.DotMinus;
			this.BarPreview.sprite = ((value > 0) ? this.BarPlus : this.BarMinus);
			this.BarPreviewRect.anchorMin = new Vector2(this.GetRealFillAmount(start), 0f);
			this.BarPreviewRect.anchorMax = new Vector2(this.GetRealFillAmount(end), 1f);
			int startLevel = SharedMethods.GetFavorLevel(start);
			int endLevel = SharedMethods.GetFavorLevel(end);
			for (int index = 0; index < this.DotPreview.Length; index++)
			{
				CImage cImage = this.DotPreview[index];
				bool flag2 = index > startLevel && index <= endLevel;
				if (flag2)
				{
					cImage.sprite = image;
					cImage.gameObject.SetActive(true);
				}
				else
				{
					cImage.gameObject.SetActive(false);
				}
			}
			this.PreviewContainer.SetActive(true);
		}
	}

	// Token: 0x06002CF6 RID: 11510 RVA: 0x00161A90 File Offset: 0x0015FC90
	private void BreathEffect(float alpha)
	{
		bool flag = !this.PreviewContainer.activeSelf;
		if (!flag)
		{
			foreach (CImage image in this.DotPreview.Append(this.BarPreview))
			{
				image.SetAlpha(alpha);
			}
		}
	}

	// Token: 0x0400208B RID: 8331
	public Sprite DotActive;

	// Token: 0x0400208C RID: 8332
	public Sprite DotInactive;

	// Token: 0x0400208D RID: 8333
	public Sprite DotEmpty;

	// Token: 0x0400208E RID: 8334
	public Sprite DotMinus;

	// Token: 0x0400208F RID: 8335
	public Sprite DotPlus;

	// Token: 0x04002090 RID: 8336
	public Sprite BarActive;

	// Token: 0x04002091 RID: 8337
	public Sprite BarInactive;

	// Token: 0x04002092 RID: 8338
	public Sprite BarMinus;

	// Token: 0x04002093 RID: 8339
	public Sprite BarPlus;

	// Token: 0x04002094 RID: 8340
	public CImage[] DotCurrent;

	// Token: 0x04002095 RID: 8341
	public CImage[] DotPreview;

	// Token: 0x04002096 RID: 8342
	public CImage BarCurrent;

	// Token: 0x04002097 RID: 8343
	public CImage BarActual;

	// Token: 0x04002098 RID: 8344
	public CImage BarPreview;

	// Token: 0x04002099 RID: 8345
	public CImage LimitImage;

	// Token: 0x0400209A RID: 8346
	public RectTransform BarPreviewRect;

	// Token: 0x0400209B RID: 8347
	public GameObject PreviewContainer;

	// Token: 0x0400209C RID: 8348
	public TooltipInvoker TipsDisplayer;

	// Token: 0x0400209D RID: 8349
	public int MerchantTemplateId;

	// Token: 0x0400209E RID: 8350
	public bool ReachProgressLimit;

	// Token: 0x0400209F RID: 8351
	private int _activeValue;

	// Token: 0x040020A0 RID: 8352
	private int _currentValue;

	// Token: 0x040020A1 RID: 8353
	private bool _hasValue;

	// Token: 0x040020A2 RID: 8354
	private int _previewDelta;
}
