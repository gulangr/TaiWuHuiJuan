using System;
using GameData.Utilities;
using UnityEngine;

namespace GameDataExtensions
{
	// Token: 0x020006D3 RID: 1747
	public static class ByteCoordinateHelper
	{
		// Token: 0x06005359 RID: 21337 RVA: 0x00269EAC File Offset: 0x002680AC
		public static Vector2Int ToVector2Int(this ByteCoordinate coordinate)
		{
			return new Vector2Int((int)coordinate.X, (int)coordinate.Y);
		}

		// Token: 0x0600535A RID: 21338 RVA: 0x00269ED0 File Offset: 0x002680D0
		public static IntPos2 ToIntPos(this ByteCoordinate coordinate)
		{
			return new IntPos2
			{
				X = (int)coordinate.X,
				Y = (int)coordinate.Y
			};
		}

		// Token: 0x0600535B RID: 21339 RVA: 0x00269F08 File Offset: 0x00268108
		public static ByteCoordinate ToByteCoordinate(this IntPos2 pos)
		{
			return new ByteCoordinate((byte)pos.X, (byte)pos.Y);
		}
	}
}
