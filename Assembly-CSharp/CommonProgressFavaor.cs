using System;
using DG.Tweening;
using GameData.Domains.Character.Relation;

// Token: 0x0200032F RID: 815
public class CommonProgressFavaor : Refers
{
	// Token: 0x17000533 RID: 1331
	// (get) Token: 0x06002F44 RID: 12100 RVA: 0x00173395 File Offset: 0x00171595
	private CImage _fillImage
	{
		get
		{
			return base.CGet<CImage>("Fill");
		}
	}

	// Token: 0x06002F45 RID: 12101 RVA: 0x001733A2 File Offset: 0x001715A2
	public void SetProgress(float progress, bool animation)
	{
		this._fillImage.DOFillAmount(progress, animation ? 0.3f : 0f);
	}

	// Token: 0x06002F46 RID: 12102 RVA: 0x001733C4 File Offset: 0x001715C4
	public void SetProgress(short favorabilityToTaiwu)
	{
		ValueTuple<short, short> favorabilityRange = FavorabilityType.GetFavorabilityRange(favorabilityToTaiwu);
		short min = favorabilityRange.Item1;
		short max = favorabilityRange.Item2;
		float progress = (float)(favorabilityToTaiwu - min) / (float)(max - min);
		this.SetProgress(progress, false);
	}

	// Token: 0x06002F47 RID: 12103 RVA: 0x001733F8 File Offset: 0x001715F8
	public void Setup(short favorabilityToTaiwu)
	{
		this._fillImage.SetSpriteOnly(this.GetFavorFillSpriteNameByFavorType(FavorabilityType.GetFavorabilityType(favorabilityToTaiwu)), false, null);
	}

	// Token: 0x06002F48 RID: 12104 RVA: 0x00173415 File Offset: 0x00171615
	public void SetupByFavorabilityType(sbyte favorabilityType)
	{
		this._fillImage.SetSpriteOnly(this.GetFavorFillSpriteNameByFavorType(favorabilityType), false, null);
	}

	// Token: 0x06002F49 RID: 12105 RVA: 0x00173430 File Offset: 0x00171630
	private string GetFavorFillSpriteNameByFavorType(sbyte favorType)
	{
		if (!true)
		{
		}
		string result;
		switch (favorType)
		{
		case -6:
			result = "ui_sp_progress_favor_10";
			break;
		case -5:
			result = "ui_sp_progress_favor_10";
			break;
		case -4:
			result = "ui_sp_progress_favor_10";
			break;
		case -3:
			result = "ui_sp_progress_favor_9";
			break;
		case -2:
			result = "ui_sp_progress_favor_9";
			break;
		case -1:
			result = "ui_sp_progress_favor_9";
			break;
		case 0:
			result = "ui_sp_progress_favor_8";
			break;
		case 1:
			result = "ui_sp_progress_favor_7";
			break;
		case 2:
			result = "ui_sp_progress_favor_6";
			break;
		case 3:
			result = "ui_sp_progress_favor_5";
			break;
		case 4:
			result = "ui_sp_progress_favor_4";
			break;
		case 5:
			result = "ui_sp_progress_favor_3";
			break;
		case 6:
			result = "ui_sp_progress_favor_2";
			break;
		default:
			result = "ui_sp_progress_favor_8";
			break;
		}
		if (!true)
		{
		}
		return result;
	}
}
