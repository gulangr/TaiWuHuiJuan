using System;

// Token: 0x020001FF RID: 511
public class CustomizeTableColumeConfig
{
	// Token: 0x17000352 RID: 850
	// (get) Token: 0x060020FB RID: 8443 RVA: 0x000F05ED File Offset: 0x000EE7ED
	public string ElementLocalName
	{
		get
		{
			return string.IsNullOrEmpty(this.ElementConfigName) ? ((this.ElementNameKey == LanguageKey.Invalid) ? string.Empty : LocalStringManager.Get(this.ElementNameKey)) : this.ElementConfigName;
		}
	}

	// Token: 0x060020FC RID: 8444 RVA: 0x000F061F File Offset: 0x000EE81F
	public CustomizeTableColumeConfig(int columeId, int elementType, LanguageKey elementNameKey, float width, bool canSort = true)
	{
		this.ColumeId = columeId;
		this.ElementType = elementType;
		this.ElementNameKey = elementNameKey;
		this.Width = width;
		this.CanSort = canSort;
		this.ElementConfigName = string.Empty;
	}

	// Token: 0x060020FD RID: 8445 RVA: 0x000F0659 File Offset: 0x000EE859
	public CustomizeTableColumeConfig(int columeId, int elementType, string elementConfigName, float width, bool canSort = true)
	{
		this.ColumeId = columeId;
		this.ElementType = elementType;
		this.ElementConfigName = elementConfigName;
		this.Width = width;
		this.CanSort = canSort;
	}

	// Token: 0x04001957 RID: 6487
	public int ColumeId;

	// Token: 0x04001958 RID: 6488
	public int ElementType;

	// Token: 0x04001959 RID: 6489
	public LanguageKey ElementNameKey;

	// Token: 0x0400195A RID: 6490
	public float Width;

	// Token: 0x0400195B RID: 6491
	public bool CanSort;

	// Token: 0x0400195C RID: 6492
	public string ElementConfigName;
}
