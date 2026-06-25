using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using GameData.Serializer;

namespace EventEditor
{
	// Token: 0x02000625 RID: 1573
	public class EventEditorData : GameData.Serializer.ICommonObjectSerializationAware
	{
		// Token: 0x06004A70 RID: 19056 RVA: 0x0022E008 File Offset: 0x0022C208
		public EventEditorData.LanguageTable<string, string> GetDefaultEditLanguageTable()
		{
			return this.Language[LanguageTableKeys.Cn];
		}

		// Token: 0x06004A71 RID: 19057 RVA: 0x0022E01C File Offset: 0x0022C21C
		public EventEditorData Duplicate()
		{
			EventEditorData duplicated = new EventEditorData();
			this.Refill(duplicated);
			return duplicated;
		}

		// Token: 0x06004A72 RID: 19058 RVA: 0x0022E040 File Offset: 0x0022C240
		internal void Refill(EventEditorData target)
		{
			string marshalData;
			GameData.Serializer.CommonObjectSerializer.Serialize<EventEditorData>(this, out marshalData, GameData.Serializer.CommonObjectSerializer.MarshalFormat.Lua);
			GameData.Serializer.CommonObjectSerializer.RestoreObject<EventEditorData>(marshalData, target, GameData.Serializer.CommonObjectSerializer.MarshalFormat.Lua);
			target.EventContent = this.EventContent;
			target.EventName = this.EventName;
			bool flag = this.Options != null;
			if (flag)
			{
				if (target.Options == null)
				{
					target.Options = new Dictionary<int, EventEditorData.Option>();
				}
				foreach (KeyValuePair<int, EventEditorData.Option> keyValuePair in this.Options)
				{
					int num;
					EventEditorData.Option option2;
					keyValuePair.Deconstruct(out num, out option2);
					int i = num;
					EventEditorData.Option option = option2;
					EventEditorData.Option newOption;
					bool flag2 = target.Options.TryGetValue(i, out newOption);
					if (flag2)
					{
						newOption.InternalContent = option.Content;
					}
				}
			}
		}

		// Token: 0x06004A73 RID: 19059 RVA: 0x0022E11C File Offset: 0x0022C31C
		public bool SkipMember(MemberInfo member, bool deserializing)
		{
			bool flag = !deserializing;
			if (flag)
			{
				string text = (member != null) ? member.Name : null;
				string a = text;
				if (a == "EventName" || a == "EventContent")
				{
					return true;
				}
				if (a == "Language")
				{
					Dictionary<string, EventEditorData.LanguageTable<string, string>> language = this.Language;
					bool result;
					if (language == null)
					{
						result = true;
					}
					else
					{
						result = language.All(delegate(KeyValuePair<string, EventEditorData.LanguageTable<string, string>> pack)
						{
							EventEditorData.LanguageTable<string, string> value = pack.Value;
							return value != null && value.Count == 0;
						});
					}
					return result;
				}
			}
			return false;
		}

		// Token: 0x06004A74 RID: 19060 RVA: 0x0022E1AC File Offset: 0x0022C3AC
		public bool DeserializingUnknownField(string name, out GameData.Serializer.CommonObjectSerializationMember proc)
		{
			bool result;
			if (!(name == "_eventName"))
			{
				if (!(name == "ChooseTitle"))
				{
					proc = default(GameData.Serializer.CommonObjectSerializationMember);
					result = false;
				}
				else
				{
					proc = GameData.Serializer.CommonObjectSerializationMember.MakeSetOnly<Dictionary<int, EventEditorData.Option>>(name, delegate(Dictionary<int, EventEditorData.Option> options)
					{
						foreach (EventEditorData.Option value in options.Values)
						{
							string content = value.ItemValue;
							value.ItemValue = null;
							string guidString = value.Guid;
							bool flag = string.IsNullOrEmpty(guidString);
							if (flag)
							{
								guidString = Guid.NewGuid().ToString();
								value.Guid = guidString;
							}
							string optionKey = string.Format("Option_{0}", guidString.GetHashCode());
							this.GetDefaultEditLanguageTable()[optionKey] = content;
							bool flag2 = string.IsNullOrEmpty(value.OptionKey);
							if (flag2)
							{
								value.OptionKey = optionKey;
							}
						}
						this.Options = options;
					});
					result = true;
				}
			}
			else
			{
				proc = GameData.Serializer.CommonObjectSerializationMember.MakeSetOnly<string>(name, delegate(string v)
				{
					this.EventName = v;
				});
				result = true;
			}
			return result;
		}

		// Token: 0x06004A75 RID: 19061 RVA: 0x0022E21E File Offset: 0x0022C41E
		public void FinishedDeserialization()
		{
			if (this.Options == null)
			{
				this.Options = new Dictionary<int, EventEditorData.Option>();
			}
		}

		// Token: 0x0400338D RID: 13197
		public string SaveVersion;

		// Token: 0x0400338E RID: 13198
		public string EventGroup;

		// Token: 0x0400338F RID: 13199
		public string EventName;

		// Token: 0x04003390 RID: 13200
		public string EventGuid;

		// Token: 0x04003391 RID: 13201
		public string EventContent;

		// Token: 0x04003392 RID: 13202
		public string EventType;

		// Token: 0x04003393 RID: 13203
		public string TriggerType;

		// Token: 0x04003394 RID: 13204
		public short EventOrder = 500;

		// Token: 0x04003395 RID: 13205
		public bool ForceSingle = false;

		// Token: 0x04003396 RID: 13206
		public bool ControlMask = false;

		// Token: 0x04003397 RID: 13207
		public byte ControlMaskCode = 0;

		// Token: 0x04003398 RID: 13208
		public string DecideRole;

		// Token: 0x04003399 RID: 13209
		public string TargetRole;

		// Token: 0x0400339A RID: 13210
		public bool InternalTexture;

		// Token: 0x0400339B RID: 13211
		public string EventTexture;

		// Token: 0x0400339C RID: 13212
		public string TexturePath;

		// Token: 0x0400339D RID: 13213
		public string EscOption;

		// Token: 0x0400339E RID: 13214
		public bool Export;

		// Token: 0x0400339F RID: 13215
		public Dictionary<int, EventEditorData.Option> Options = new Dictionary<int, EventEditorData.Option>();

		// Token: 0x040033A0 RID: 13216
		public readonly Dictionary<string, EventEditorData.LanguageTable<string, string>> Language = new Dictionary<string, EventEditorData.LanguageTable<string, string>>
		{
			{
				LanguageTableKeys.Cn,
				new EventEditorData.LanguageTable<string, string>()
			},
			{
				LanguageTableKeys.Cnh,
				new EventEditorData.LanguageTable<string, string>()
			},
			{
				LanguageTableKeys.En,
				new EventEditorData.LanguageTable<string, string>()
			},
			{
				LanguageTableKeys.Jp,
				new EventEditorData.LanguageTable<string, string>()
			}
		};

		// Token: 0x040033A1 RID: 13217
		public long TmEdit;

		// Token: 0x040033A2 RID: 13218
		public string AudioName;

		// Token: 0x040033A3 RID: 13219
		public float MaskTweenTime;

		// Token: 0x040033A4 RID: 13220
		internal bool Dirty;

		// Token: 0x02001A01 RID: 6657
		public class LanguageTable<TK, TV> : Dictionary<TK, TV>
		{
			// Token: 0x0400B490 RID: 46224
			public string EventContent;
		}

		// Token: 0x02001A02 RID: 6658
		public class Option
		{
			// Token: 0x1700179B RID: 6043
			// (get) Token: 0x0600DCC6 RID: 56518 RVA: 0x005CFF86 File Offset: 0x005CE186
			public string Content
			{
				get
				{
					return this.InternalContent;
				}
			}

			// Token: 0x0400B491 RID: 46225
			[Obsolete]
			public string ItemValue;

			// Token: 0x0400B492 RID: 46226
			public string Guid;

			// Token: 0x0400B493 RID: 46227
			public string OptionKey;

			// Token: 0x0400B494 RID: 46228
			public int Behavior = 0;

			// Token: 0x0400B495 RID: 46229
			public string DefaultState;

			// Token: 0x0400B496 RID: 46230
			public bool OneTimeOnly;

			// Token: 0x0400B497 RID: 46231
			public bool Important;

			// Token: 0x0400B498 RID: 46232
			[TupleElementNames(new string[]
			{
				"EventGuid",
				"OptionGuid"
			})]
			public ValueTuple<string, string> RedirectTargetOption;

			// Token: 0x0400B499 RID: 46233
			internal string InternalContent;
		}
	}
}
