using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace GameData.Serializer
{
	// Token: 0x02000FD2 RID: 4050
	public interface ICommonObjectSerializationAware
	{
		// Token: 0x0600B947 RID: 47431 RVA: 0x00546D89 File Offset: 0x00544F89
		bool SkipMember(MemberInfo member, bool deserializing)
		{
			return false;
		}

		// Token: 0x0600B948 RID: 47432 RVA: 0x00546D8C File Offset: 0x00544F8C
		IEnumerable<CommonObjectSerializationMember> ExtraMembers(bool deserializing)
		{
			return Enumerable.Empty<CommonObjectSerializationMember>();
		}

		// Token: 0x0600B949 RID: 47433 RVA: 0x00546D94 File Offset: 0x00544F94
		bool DeserializingUnknownField(string name, out CommonObjectSerializationMember proc)
		{
			proc = default(CommonObjectSerializationMember);
			return false;
		}

		// Token: 0x0600B94A RID: 47434 RVA: 0x00546DAE File Offset: 0x00544FAE
		void DeserializingMissingField(CommonObjectSerializationMember member)
		{
		}

		// Token: 0x0600B94B RID: 47435 RVA: 0x00546DB1 File Offset: 0x00544FB1
		void InitializeOnDeserializing()
		{
		}

		// Token: 0x0600B94C RID: 47436 RVA: 0x00546DB4 File Offset: 0x00544FB4
		void FinishedDeserialization()
		{
		}
	}
}
