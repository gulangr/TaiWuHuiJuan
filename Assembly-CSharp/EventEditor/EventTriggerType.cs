using System;
using System.Collections.Generic;
using System.Linq;
using GameData.Serializer;

namespace EventEditor
{
	// Token: 0x02000627 RID: 1575
	public struct EventTriggerType : GameData.Serializer.ICommonObjectSerializationAware
	{
		// Token: 0x06004A7A RID: 19066 RVA: 0x0022E3B1 File Offset: 0x0022C5B1
		public void InitializeOnDeserializing()
		{
			this.TriggerArgumentInfoList = new List<EventTriggerType.Argument>();
		}

		// Token: 0x06004A7B RID: 19067 RVA: 0x0022E3C0 File Offset: 0x0022C5C0
		public bool DeserializingUnknownField(string name, out GameData.Serializer.CommonObjectSerializationMember proc)
		{
			bool result;
			if (!(name == "NameSpace"))
			{
				if (!(name == "TriggerTypeLocal"))
				{
					if (!(name == "TriggerTypeCode"))
					{
						if (!(name == "PlayerVisible"))
						{
							if (!(name == "TriggerArgumentInfoList"))
							{
								proc = default(GameData.Serializer.CommonObjectSerializationMember);
								result = false;
							}
							else
							{
								proc = GameData.Serializer.CommonObjectSerializationMember.MakeSetOnly<Dictionary<int, EventTriggerType.Argument>>(name, new Action<Dictionary<int, EventTriggerType.Argument>>(this.SetTriggerArgumentInfoList));
								result = true;
							}
						}
						else
						{
							proc = GameData.Serializer.CommonObjectSerializationMember.MakeSetOnly<bool>(name, new Action<bool>(this.SetPlayerVisible));
							result = true;
						}
					}
					else
					{
						proc = GameData.Serializer.CommonObjectSerializationMember.MakeSetOnly<string>(name, new Action<string>(this.SetTriggerTypeCode));
						result = true;
					}
				}
				else
				{
					proc = GameData.Serializer.CommonObjectSerializationMember.MakeSetOnly<string>(name, new Action<string>(this.SetTriggerTypeLocal));
					result = true;
				}
			}
			else
			{
				proc = GameData.Serializer.CommonObjectSerializationMember.MakeSetOnly<string>(name, new Action<string>(this.SetNameSpace));
				result = true;
			}
			return result;
		}

		// Token: 0x06004A7C RID: 19068 RVA: 0x0022E4EB File Offset: 0x0022C6EB
		private void SetNameSpace(string v)
		{
			this.UsingNamespace = v;
		}

		// Token: 0x06004A7D RID: 19069 RVA: 0x0022E4F4 File Offset: 0x0022C6F4
		private void SetTriggerTypeLocal(string v)
		{
			this.TriggerTypeLocal = v;
		}

		// Token: 0x06004A7E RID: 19070 RVA: 0x0022E4FD File Offset: 0x0022C6FD
		private void SetTriggerTypeCode(string v)
		{
			this.TriggerTypeCode = v;
		}

		// Token: 0x06004A7F RID: 19071 RVA: 0x0022E506 File Offset: 0x0022C706
		private void SetPlayerVisible(bool v)
		{
			this.PlayerVisible = v;
		}

		// Token: 0x06004A80 RID: 19072 RVA: 0x0022E510 File Offset: 0x0022C710
		private void SetTriggerArgumentInfoList(Dictionary<int, EventTriggerType.Argument> v)
		{
			if (this.TriggerArgumentInfoList == null)
			{
				this.TriggerArgumentInfoList = new List<EventTriggerType.Argument>();
			}
			this.TriggerArgumentInfoList.Clear();
			this.TriggerArgumentInfoList.AddRange(from p in v
			orderby p.Key
			select p.Value);
		}

		// Token: 0x040033AA RID: 13226
		public short Id;

		// Token: 0x040033AB RID: 13227
		public string TriggerTypeLocal;

		// Token: 0x040033AC RID: 13228
		public string TriggerTypeCode;

		// Token: 0x040033AD RID: 13229
		public string UsingNamespace;

		// Token: 0x040033AE RID: 13230
		public bool PlayerVisible;

		// Token: 0x040033AF RID: 13231
		public List<EventTriggerType.Argument> TriggerArgumentInfoList;

		// Token: 0x02001A04 RID: 6660
		public struct Argument
		{
			// Token: 0x1700179C RID: 6044
			// (get) Token: 0x0600DCCB RID: 56523 RVA: 0x005CFFCB File Offset: 0x005CE1CB
			public string Item1
			{
				get
				{
					return this.Type;
				}
			}

			// Token: 0x1700179D RID: 6045
			// (get) Token: 0x0600DCCC RID: 56524 RVA: 0x005CFFD3 File Offset: 0x005CE1D3
			public string Item2
			{
				get
				{
					return this.Key;
				}
			}

			// Token: 0x1700179E RID: 6046
			// (get) Token: 0x0600DCCD RID: 56525 RVA: 0x005CFFDB File Offset: 0x005CE1DB
			public string Item3
			{
				get
				{
					return this.Desc;
				}
			}

			// Token: 0x0600DCCE RID: 56526 RVA: 0x005CFFE3 File Offset: 0x005CE1E3
			public static implicit operator ValueTuple<string, string, string>(EventTriggerType.Argument v)
			{
				return new ValueTuple<string, string, string>(v.Type, v.Key, v.Desc);
			}

			// Token: 0x0400B49C RID: 46236
			public string Type;

			// Token: 0x0400B49D RID: 46237
			public string Key;

			// Token: 0x0400B49E RID: 46238
			public string Desc;
		}
	}
}
