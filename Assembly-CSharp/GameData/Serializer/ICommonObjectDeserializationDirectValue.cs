using System;

namespace GameData.Serializer
{
	// Token: 0x02000FD3 RID: 4051
	public interface ICommonObjectDeserializationDirectValue : ICommonObjectSerializationAware
	{
		// Token: 0x0600B94D RID: 47437
		void OnUnknownFieldGet(string name, object value);
	}
}
