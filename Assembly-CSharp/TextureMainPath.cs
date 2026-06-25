using System;

// Token: 0x020000E0 RID: 224
public class TextureMainPath
{
	// Token: 0x040008D2 RID: 2258
	public const string AdventureEnvironment = "Adventure_Environment";

	// Token: 0x040008D3 RID: 2259
	public const string Building = "Building";

	// Token: 0x040008D4 RID: 2260
	public const string CgTextures = "CGTextures";

	// Token: 0x040008D5 RID: 2261
	public const string EventBack = "EventBack";

	// Token: 0x040008D6 RID: 2262
	[ChildDirectory(typeof(NpcFaceSubPath))]
	public const string NpcFace = "NpcFace";
}
