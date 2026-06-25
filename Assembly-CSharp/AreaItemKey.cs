using System;
using GameData.Domains.Item;

// Token: 0x0200039F RID: 927
internal readonly struct AreaItemKey
{
	// Token: 0x060037D2 RID: 14290 RVA: 0x001C151C File Offset: 0x001BF71C
	public AreaItemKey(sbyte type, short templateId, int amount)
	{
		this.Type = type;
		this.TemplateId = templateId;
		this.Amount = amount;
	}

	// Token: 0x060037D3 RID: 14291 RVA: 0x001C1534 File Offset: 0x001BF734
	public AreaItemKey(TemplateKey key, int amount)
	{
		this.Type = key.ItemType;
		this.TemplateId = key.TemplateId;
		this.Amount = amount;
	}

	// Token: 0x060037D4 RID: 14292 RVA: 0x001C1558 File Offset: 0x001BF758
	public TemplateKey ToTemplateKey()
	{
		return new TemplateKey(this.Type, this.TemplateId);
	}

	// Token: 0x060037D5 RID: 14293 RVA: 0x001C157C File Offset: 0x001BF77C
	public string GetName()
	{
		return ItemTemplateHelper.GetName(this.Type, this.TemplateId);
	}

	// Token: 0x060037D6 RID: 14294 RVA: 0x001C15A0 File Offset: 0x001BF7A0
	public sbyte GetGrade()
	{
		return ItemTemplateHelper.GetGrade(this.Type, this.TemplateId);
	}

	// Token: 0x060037D7 RID: 14295 RVA: 0x001C15C4 File Offset: 0x001BF7C4
	public int GetValue()
	{
		return ItemTemplateHelper.GetBaseValue(this.Type, this.TemplateId);
	}

	// Token: 0x060037D8 RID: 14296 RVA: 0x001C15E8 File Offset: 0x001BF7E8
	public int GetWeight()
	{
		return ItemTemplateHelper.GetBaseWeight(this.Type, this.TemplateId);
	}

	// Token: 0x04002861 RID: 10337
	public readonly sbyte Type;

	// Token: 0x04002862 RID: 10338
	public readonly short TemplateId;

	// Token: 0x04002863 RID: 10339
	public readonly int Amount;
}
