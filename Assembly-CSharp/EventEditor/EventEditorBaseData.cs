using System;
using System.Collections.Generic;
using System.Reflection;
using GameData.Serializer;

namespace EventEditor
{
	// Token: 0x02000624 RID: 1572
	public class EventEditorBaseData : GameData.Serializer.ICommonObjectSerializationAware
	{
		// Token: 0x06004A6C RID: 19052 RVA: 0x0022DF3D File Offset: 0x0022C13D
		public void FinishedDeserialization()
		{
			if (this.Options == null)
			{
				this.Options = new Dictionary<int, string>();
			}
		}

		// Token: 0x06004A6D RID: 19053 RVA: 0x0022DF54 File Offset: 0x0022C154
		public bool SkipMember(MemberInfo member, bool deserializing)
		{
			bool flag = !deserializing;
			if (flag)
			{
				string text = (member != null) ? member.Name : null;
				string a = text;
				if (a == "Options")
				{
					return this.Options.Count == 0;
				}
			}
			return false;
		}

		// Token: 0x06004A6E RID: 19054 RVA: 0x0022DFA0 File Offset: 0x0022C1A0
		public bool DeserializingUnknownField(string name, out GameData.Serializer.CommonObjectSerializationMember proc)
		{
			int idx;
			bool flag = int.TryParse(name, out idx);
			bool result;
			if (flag)
			{
				proc = GameData.Serializer.CommonObjectSerializationMember.MakeSetOnly<string>(name, delegate(string v)
				{
					if (this.Options == null)
					{
						this.Options = new Dictionary<int, string>();
					}
					this.Options[idx] = v;
				});
				result = true;
			}
			else
			{
				proc = default(GameData.Serializer.CommonObjectSerializationMember);
				result = false;
			}
			return result;
		}

		// Token: 0x0400338A RID: 13194
		public string EventName;

		// Token: 0x0400338B RID: 13195
		public string EventContent;

		// Token: 0x0400338C RID: 13196
		public Dictionary<int, string> Options = new Dictionary<int, string>();
	}
}
