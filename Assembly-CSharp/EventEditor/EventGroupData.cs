using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Text;
using GameData.Serializer;

namespace EventEditor
{
	// Token: 0x02000629 RID: 1577
	public class EventGroupData : GameData.Serializer.ICommonObjectSerializationAware
	{
		// Token: 0x17000955 RID: 2389
		// (get) Token: 0x06004AD6 RID: 19158 RVA: 0x0023168C File Offset: 0x0022F88C
		// (set) Token: 0x06004AD7 RID: 19159 RVA: 0x00231694 File Offset: 0x0022F894
		public string GroupDirectory { get; private set; }

		// Token: 0x17000956 RID: 2390
		// (get) Token: 0x06004AD8 RID: 19160 RVA: 0x0023169D File Offset: 0x0022F89D
		// (set) Token: 0x06004AD9 RID: 19161 RVA: 0x002316A5 File Offset: 0x0022F8A5
		public int EventCount { get; private set; }

		// Token: 0x17000957 RID: 2391
		// (get) Token: 0x06004ADA RID: 19162 RVA: 0x002316AE File Offset: 0x0022F8AE
		// (set) Token: 0x06004ADB RID: 19163 RVA: 0x002316B6 File Offset: 0x0022F8B6
		public Dictionary<string, EventEditorBaseData> AllEventContent { get; private set; }

		// Token: 0x06004ADC RID: 19164 RVA: 0x002316C0 File Offset: 0x0022F8C0
		public static EventGroupData CreateOrLoad(string indexFilePath)
		{
			string groupDir = new FileInfo(indexFilePath).DirectoryName;
			bool flag = !string.IsNullOrEmpty(groupDir) && !Directory.Exists(groupDir);
			if (flag)
			{
				Directory.CreateDirectory(groupDir);
			}
			bool flag2 = File.Exists(indexFilePath);
			if (flag2)
			{
				try
				{
					EventGroupData groupData;
					GameData.Serializer.CommonObjectSerializer.Deserialize<EventGroupData>(File.ReadAllText(indexFilePath), out groupData, GameData.Serializer.CommonObjectSerializer.MarshalFormat.LuaWithReturnPrefix);
					groupData.EventCount = groupData.AllEventContent.Count;
					groupData.GroupDirectory = groupDir;
					groupData.Compile = false;
					return groupData;
				}
				catch (Exception e)
				{
					GLog.Warn(indexFilePath);
					GLog.Error(e, Array.Empty<object>());
				}
			}
			return new EventGroupData
			{
				AllEventContent = new Dictionary<string, EventEditorBaseData>(),
				EventCount = 0,
				GroupDirectory = groupDir
			};
		}

		// Token: 0x06004ADD RID: 19165 RVA: 0x0023178C File Offset: 0x0022F98C
		private EventGroupData()
		{
		}

		// Token: 0x06004ADE RID: 19166 RVA: 0x002317A4 File Offset: 0x0022F9A4
		public bool HasEvent(string guid)
		{
			return this.AllEventContent.ContainsKey(guid);
		}

		// Token: 0x06004ADF RID: 19167 RVA: 0x002317C4 File Offset: 0x0022F9C4
		public EventEditorData GetEvent(string guid)
		{
			EventEditorData eventData;
			this._allGroupEvents.TryGetValue(guid, out eventData);
			bool flag = eventData == null && this.AllEventContent.ContainsKey(guid);
			if (flag)
			{
				string eventFilePath = Path.Combine(this.GroupDirectory, guid, guid + ".twe").PathFix();
				bool flag2 = File.Exists(eventFilePath);
				if (flag2)
				{
					eventData = SingletonObject.getInstance<Load>().LoadEvent(eventFilePath);
				}
				this._allGroupEvents[guid] = eventData;
			}
			bool flag3 = eventData != null;
			if (flag3)
			{
				EventEditorBaseData table;
				bool flag4 = this.AllEventContent.TryGetValue(guid, out table);
				if (flag4)
				{
					eventData.EventContent = table.EventContent;
					eventData.EventName = table.EventName;
					foreach (KeyValuePair<int, string> pair in table.Options)
					{
						EventEditorData.Option option;
						bool flag5 = !eventData.Options.TryGetValue(pair.Key, out option);
						if (flag5)
						{
							eventData.Options.Add(pair.Key, option = new EventEditorData.Option());
						}
						option.InternalContent = pair.Value;
					}
				}
			}
			return eventData;
		}

		// Token: 0x06004AE0 RID: 19168 RVA: 0x00231910 File Offset: 0x0022FB10
		public void AddEvent(EventEditorData eventData, bool saveGroup = true)
		{
			bool flag = eventData == null;
			if (!flag)
			{
				string eventName = eventData.EventName;
				string guid = eventData.EventGuid;
				bool flag2 = this.AllEventContent.ContainsKey(guid);
				if (flag2)
				{
					EventEditorBaseData baseData = new EventEditorBaseData
					{
						EventName = eventName,
						EventContent = eventData.EventContent
					};
					bool flag3 = eventData.Options != null;
					if (flag3)
					{
						foreach (KeyValuePair<int, EventEditorData.Option> pair in eventData.Options)
						{
							baseData.Options[pair.Key] = pair.Value.Content;
						}
					}
					this.AllEventContent[guid] = baseData;
				}
				string eventDir = Path.Combine(this.GroupDirectory, guid);
				bool flag4 = !Directory.Exists(eventDir);
				if (flag4)
				{
					Directory.CreateDirectory(eventDir);
				}
				if (saveGroup)
				{
					this.SaveGroup();
				}
				this._allGroupEvents.Add(eventData.EventGuid, eventData);
				this.EventCount = this.AllEventContent.Count;
			}
		}

		// Token: 0x06004AE1 RID: 19169 RVA: 0x00231A48 File Offset: 0x0022FC48
		public bool RemoveEvent(string guid, string newDirectoryPath)
		{
			this._allGroupEvents.Remove(guid);
			bool flag = this.AllEventContent.Remove(guid);
			bool result;
			if (flag)
			{
				string srcDirectory = Path.Combine(this.GroupDirectory, guid).PathFix();
				bool flag2 = string.IsNullOrEmpty(newDirectoryPath);
				if (flag2)
				{
					Directory.Delete(srcDirectory, true);
				}
				else
				{
					bool flag3 = Directory.Exists(newDirectoryPath);
					if (flag3)
					{
						Directory.Delete(newDirectoryPath);
					}
					Directory.Move(srcDirectory, newDirectoryPath);
				}
				this.SaveGroup();
				this.EventCount = this.AllEventContent.Count;
				result = true;
			}
			else
			{
				result = false;
			}
			return result;
		}

		// Token: 0x06004AE2 RID: 19170 RVA: 0x00231ADC File Offset: 0x0022FCDC
		public List<ValueTuple<string, string, string>> GetDisplayList(bool sort = false)
		{
			List<ValueTuple<string, string, string>> resultList = new List<ValueTuple<string, string, string>>();
			List<string> keyList = new List<string>(this.AllEventContent.Keys);
			keyList.ForEach(delegate(string guid)
			{
				string eventDir = Path.Combine(this.GroupDirectory, guid);
				bool flag = !Directory.Exists(eventDir);
				if (flag)
				{
					this.AllEventContent.Remove(guid);
					this.SaveGroup();
				}
				else
				{
					EventEditorBaseData eventData = this.AllEventContent[guid];
					string eventName = eventData.EventName;
					string eventContent = eventData.EventContent;
					resultList.Add(new ValueTuple<string, string, string>(guid, eventName, eventContent));
				}
			});
			if (sort)
			{
				resultList.Sort((ValueTuple<string, string, string> l, ValueTuple<string, string, string> r) => string.CompareOrdinal(l.Item2, r.Item2));
			}
			return resultList;
		}

		// Token: 0x06004AE3 RID: 19171 RVA: 0x00231B64 File Offset: 0x0022FD64
		public void UpdateContent()
		{
			List<string> loadedEventKeys = new List<string>(this._allGroupEvents.Keys);
			this._allGroupEvents.Clear();
			foreach (string guid in loadedEventKeys)
			{
				this.GetEvent(guid);
			}
		}

		// Token: 0x06004AE4 RID: 19172 RVA: 0x00231BD8 File Offset: 0x0022FDD8
		public string GetEventGuidByEventName(string eventName)
		{
			bool flag = this.AllEventContent == null;
			string result;
			if (flag)
			{
				result = string.Empty;
			}
			else
			{
				string targetGuid = string.Empty;
				foreach (KeyValuePair<string, EventEditorBaseData> keyValuePair in this.AllEventContent)
				{
					string text;
					EventEditorBaseData eventEditorBaseData;
					keyValuePair.Deconstruct(out text, out eventEditorBaseData);
					string guid = text;
					EventEditorBaseData eventContentTable = eventEditorBaseData;
					bool flag2 = eventContentTable.EventName == eventName;
					if (flag2)
					{
						targetGuid = guid;
					}
				}
				result = targetGuid;
			}
			return result;
		}

		// Token: 0x06004AE5 RID: 19173 RVA: 0x00231C74 File Offset: 0x0022FE74
		public void SaveGroup()
		{
			bool flag = !Directory.Exists(this.GroupDirectory);
			if (flag)
			{
				EventEditorNotify.Instance.SetNotifyAndShow(LocalStringManager.Get(LanguageKey.LK_EventEditor_EventGroupDirectory_NotExist));
			}
			else
			{
				string marshalData;
				GameData.Serializer.CommonObjectSerializer.Serialize<EventGroupData>(this, out marshalData, GameData.Serializer.CommonObjectSerializer.MarshalFormat.LuaWithReturnPrefix);
				File.WriteAllText(Path.Combine(this.GroupDirectory, "index.lua").PathFix(), marshalData, Encoding.UTF8);
			}
		}

		// Token: 0x06004AE6 RID: 19174 RVA: 0x00231CD8 File Offset: 0x0022FED8
		public string SaveEvent(EventEditorData eventData, bool saveGroup = true)
		{
			bool flag = eventData == null;
			if (flag)
			{
				throw new Exception("can not save null event !");
			}
			string guid = eventData.EventGuid;
			EventEditorBaseData table = new EventEditorBaseData
			{
				EventName = eventData.EventName
			};
			string str = eventData.EventContent;
			string str2 = (str != null) ? str.Replace('\v', '\n') : null;
			string str3 = (str2 != null) ? str2.Replace("\r", string.Empty) : null;
			string content = (str3 != null) ? str3.Replace("\n", "<NL>") : null;
			table.EventContent = content;
			bool flag2 = eventData.Options != null;
			if (flag2)
			{
				foreach (KeyValuePair<int, EventEditorData.Option> pair in eventData.Options)
				{
					table.Options[pair.Key] = pair.Value.Content;
				}
			}
			this.AllEventContent[guid] = table;
			if (saveGroup)
			{
				this.SaveGroup();
			}
			return SingletonObject.getInstance<Save>().SaveEvent(eventData);
		}

		// Token: 0x06004AE7 RID: 19175 RVA: 0x00231E08 File Offset: 0x00230008
		public bool SkipMember(MemberInfo member, bool deserializing)
		{
			bool flag = !deserializing;
			if (flag)
			{
				string text = (member != null) ? member.Name : null;
				string a = text;
				if (a == "EventCount" || a == "GroupDirectory" || a == "Compile")
				{
					return true;
				}
			}
			return false;
		}

		// Token: 0x06004AE8 RID: 19176 RVA: 0x00231E64 File Offset: 0x00230064
		public bool DeserializingUnknownField(string name, out GameData.Serializer.CommonObjectSerializationMember proc)
		{
			bool flag = name == "Export";
			bool result;
			if (flag)
			{
				proc = GameData.Serializer.CommonObjectSerializationMember.MakeSetOnly<bool>(name, delegate(bool v)
				{
					this.ExportFlag = v;
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

		// Token: 0x040033D9 RID: 13273
		public const string IndexFileName = "index.lua";

		// Token: 0x040033DC RID: 13276
		public string Key;

		// Token: 0x040033DD RID: 13277
		public string Name;

		// Token: 0x040033DE RID: 13278
		public bool Compile;

		// Token: 0x040033DF RID: 13279
		public bool ExportFlag;

		// Token: 0x040033E1 RID: 13281
		private readonly Dictionary<string, EventEditorData> _allGroupEvents = new Dictionary<string, EventEditorData>();
	}
}
