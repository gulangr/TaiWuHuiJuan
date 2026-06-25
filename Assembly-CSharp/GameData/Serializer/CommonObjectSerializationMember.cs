using System;
using System.Collections.Generic;
using System.Reflection;
using JetBrains.Annotations;

namespace GameData.Serializer
{
	// Token: 0x02000FD1 RID: 4049
	public readonly struct CommonObjectSerializationMember
	{
		// Token: 0x0600B942 RID: 47426 RVA: 0x00546CBD File Offset: 0x00544EBD
		public CommonObjectSerializationMember(string name, Func<object> getter, Action<object> setter, Type typeHint, [CanBeNull] MemberInfo memberInfo)
		{
			this.Name = name;
			this.Getter = getter;
			this.Setter = setter;
			this.TypeHint = typeHint;
			this.MemberInfo = memberInfo;
		}

		// Token: 0x0600B943 RID: 47427 RVA: 0x00546CE8 File Offset: 0x00544EE8
		public static CommonObjectSerializationMember Make<T>(string name, Func<T> getter, Action<T> setter)
		{
			return new CommonObjectSerializationMember(name, () => getter(), delegate(object v)
			{
				setter((T)((object)v));
			}, typeof(T), null);
		}

		// Token: 0x0600B944 RID: 47428 RVA: 0x00546D32 File Offset: 0x00544F32
		public static CommonObjectSerializationMember MakeSetOnly<T>(string name, Action<T> setter)
		{
			return CommonObjectSerializationMember.Make<T>(name, null, setter);
		}

		// Token: 0x0600B945 RID: 47429 RVA: 0x00546D3C File Offset: 0x00544F3C
		public static CommonObjectSerializationMember MakeListRefill<T>(string key, List<T> target)
		{
			return CommonObjectSerializationMember.Make<List<T>>(key, () => target, delegate(List<T> v)
			{
				target.Clear();
				target.AddRange(v);
			});
		}

		// Token: 0x0600B946 RID: 47430 RVA: 0x00546D74 File Offset: 0x00544F74
		public static CommonObjectSerializationMember MakeTypeHintOnly<T>(string name)
		{
			return new CommonObjectSerializationMember(name, null, null, typeof(T), null);
		}

		// Token: 0x04008F7B RID: 36731
		public readonly string Name;

		// Token: 0x04008F7C RID: 36732
		public readonly Func<object> Getter;

		// Token: 0x04008F7D RID: 36733
		public readonly Action<object> Setter;

		// Token: 0x04008F7E RID: 36734
		public readonly Type TypeHint;

		// Token: 0x04008F7F RID: 36735
		[CanBeNull]
		public readonly MemberInfo MemberInfo;
	}
}
