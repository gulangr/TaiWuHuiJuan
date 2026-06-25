using System;

namespace Map.RenderSystem
{
	// Token: 0x020006D0 RID: 1744
	public static class EMapBlockSpriteFixTypeExtensions
	{
		// Token: 0x06005317 RID: 21271 RVA: 0x00266ACC File Offset: 0x00264CCC
		public static bool IsFix(this EMapBlockSpriteFixType type)
		{
			return type == EMapBlockSpriteFixType.FixSingle || type == EMapBlockSpriteFixType.FixFullSize;
		}
	}
}
