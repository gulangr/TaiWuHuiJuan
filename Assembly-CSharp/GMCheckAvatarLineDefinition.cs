using System;

// Token: 0x0200020C RID: 524
public readonly struct GMCheckAvatarLineDefinition
{
	// Token: 0x06002148 RID: 8520 RVA: 0x000F23FD File Offset: 0x000F05FD
	public GMCheckAvatarLineDefinition(GMCheckAvatarComponentType componentType, string nameKey, bool canClear)
	{
		this.ComponentType = componentType;
		this.NameKey = nameKey;
		this.CanClear = canClear;
	}

	// Token: 0x17000356 RID: 854
	// (get) Token: 0x06002149 RID: 8521 RVA: 0x000F2415 File Offset: 0x000F0615
	public GMCheckAvatarComponentType ComponentType { get; }

	// Token: 0x17000357 RID: 855
	// (get) Token: 0x0600214A RID: 8522 RVA: 0x000F241D File Offset: 0x000F061D
	public string NameKey { get; }

	// Token: 0x17000358 RID: 856
	// (get) Token: 0x0600214B RID: 8523 RVA: 0x000F2425 File Offset: 0x000F0625
	public bool CanClear { get; }
}
