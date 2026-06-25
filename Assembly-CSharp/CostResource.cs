using System;
using System.Text;
using Config;
using TMPro;

// Token: 0x02000054 RID: 84
public class CostResource : Refers
{
	// Token: 0x060002D1 RID: 721 RVA: 0x00011109 File Offset: 0x0000F309
	public void SetInfo(sbyte resourceType, int amountNeeded, int amountOwned)
	{
		this.SetResourceTitle(resourceType);
		this.SetResourceAmount(amountNeeded, amountOwned);
	}

	// Token: 0x060002D2 RID: 722 RVA: 0x0001111D File Offset: 0x0000F31D
	public void SetInfo(int amountNeeded, int amountOwned)
	{
		this.SetResourceAmount(amountNeeded, amountOwned);
	}

	// Token: 0x060002D3 RID: 723 RVA: 0x0001112C File Offset: 0x0000F32C
	private void SetResourceAmount(int amountNeeded, int amountOwned)
	{
		this._stringBuilder.Clear();
		string color = (amountOwned >= amountNeeded && amountOwned > 0) ? "brightblue" : "brightred";
		this._stringBuilder.Append(CommonUtils.GetDisplayStringForNum(amountOwned, 100000).SetColor(color));
		this._stringBuilder.Append('/');
		this._stringBuilder.Append(amountNeeded);
		base.CGet<TextMeshProUGUI>("content").text = this._stringBuilder.ToString();
	}

	// Token: 0x060002D4 RID: 724 RVA: 0x000111B0 File Offset: 0x0000F3B0
	private void SetResourceTitle(sbyte resourceType)
	{
		this._stringBuilder.Clear();
		this._stringBuilder.Append(LocalStringManager.Get(LanguageKey.LK_Craftsman_Cost));
		ResourceTypeItem config = ResourceType.Instance[resourceType];
		this._stringBuilder.Append(config.Name);
		base.CGet<TextMeshProUGUI>("title").text = this._stringBuilder.ToString();
		base.CGet<CImage>("icon").SetSprite(config.Icon, false, null);
	}

	// Token: 0x0400017E RID: 382
	private readonly StringBuilder _stringBuilder = new StringBuilder();
}
