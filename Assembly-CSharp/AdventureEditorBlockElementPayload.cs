using System;
using System.Collections.Generic;

// Token: 0x02000172 RID: 370
public readonly struct AdventureEditorBlockElementPayload
{
	// Token: 0x17000248 RID: 584
	// (get) Token: 0x0600148D RID: 5261 RVA: 0x00080058 File Offset: 0x0007E258
	public AdventureEditorBlockElementPayload.PayloadKind Kind { get; }

	// Token: 0x17000249 RID: 585
	// (get) Token: 0x0600148E RID: 5262 RVA: 0x00080060 File Offset: 0x0007E260
	public IReadOnlyList<string> VirtualPathSegments { get; }

	// Token: 0x1700024A RID: 586
	// (get) Token: 0x0600148F RID: 5263 RVA: 0x00080068 File Offset: 0x0007E268
	public string IconName { get; }

	// Token: 0x06001490 RID: 5264 RVA: 0x00080070 File Offset: 0x0007E270
	private AdventureEditorBlockElementPayload(AdventureEditorBlockElementPayload.PayloadKind kind, IReadOnlyList<string> virtualPathSegments, string iconName)
	{
		this.Kind = kind;
		this.VirtualPathSegments = virtualPathSegments;
		this.IconName = iconName;
	}

	// Token: 0x06001491 RID: 5265 RVA: 0x00080088 File Offset: 0x0007E288
	public static AdventureEditorBlockElementPayload CreateDirectory(IReadOnlyList<string> virtualPathSegments)
	{
		return new AdventureEditorBlockElementPayload(AdventureEditorBlockElementPayload.PayloadKind.Directory, virtualPathSegments, null);
	}

	// Token: 0x06001492 RID: 5266 RVA: 0x00080092 File Offset: 0x0007E292
	public static AdventureEditorBlockElementPayload CreateIcon(string iconName)
	{
		return new AdventureEditorBlockElementPayload(AdventureEditorBlockElementPayload.PayloadKind.Icon, null, iconName);
	}

	// Token: 0x1700024B RID: 587
	// (get) Token: 0x06001493 RID: 5267 RVA: 0x0008009C File Offset: 0x0007E29C
	public static AdventureEditorBlockElementPayload None
	{
		get
		{
			return new AdventureEditorBlockElementPayload(AdventureEditorBlockElementPayload.PayloadKind.None, null, null);
		}
	}

	// Token: 0x0200127D RID: 4733
	public enum PayloadKind
	{
		// Token: 0x04009AA6 RID: 39590
		None,
		// Token: 0x04009AA7 RID: 39591
		Icon,
		// Token: 0x04009AA8 RID: 39592
		Directory
	}
}
