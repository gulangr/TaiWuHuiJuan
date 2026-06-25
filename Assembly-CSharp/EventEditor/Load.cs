using System;
using System.Collections.Generic;
using System.IO;
using GameData.Domains.TaiwuEvent.Enum;
using GameData.Serializer;
using UnityEngine;

namespace EventEditor
{
	// Token: 0x02000630 RID: 1584
	public class Load
	{
		// Token: 0x06004B0C RID: 19212 RVA: 0x00234A4C File Offset: 0x00232C4C
		public EventEditorData LoadEvent(string filePath)
		{
			bool flag = !File.Exists(filePath);
			EventEditorData result;
			if (flag)
			{
				result = null;
			}
			else
			{
				string fileContent = File.ReadAllText(filePath);
				try
				{
					EventEditorData eventData;
					GameData.Serializer.CommonObjectSerializer.Deserialize<EventEditorData>(fileContent, out eventData, GameData.Serializer.CommonObjectSerializer.MarshalFormat.LuaWithReturnPrefix);
					string savedVersion = eventData.SaveVersion;
					for (;;)
					{
						bool flag2 = savedVersion == Save.LatestVersion;
						if (flag2)
						{
							break;
						}
						string handlerKey = string.Empty;
						bool flag3 = string.IsNullOrEmpty(savedVersion);
						if (flag3)
						{
							handlerKey = "NoneToAlpha_0";
						}
						int index = Array.IndexOf<string>(Save.Versions, savedVersion);
						bool flag4 = index > -1;
						if (flag4)
						{
							handlerKey = savedVersion + "_To_" + Save.Versions[index + 1];
						}
						Action<string, EventEditorData> upgradeHandler;
						bool flag5 = !this._versionUpgradeHandlers.TryGetValue(handlerKey, out upgradeHandler);
						if (flag5)
						{
							goto Block_8;
						}
						upgradeHandler(filePath, eventData);
						savedVersion = Save.Versions[index + 1];
						eventData.SaveVersion = savedVersion;
					}
					Action<EventEditorData> handler;
					bool flag6 = this._versionOnloadHandler.TryGetValue(savedVersion, out handler);
					if (flag6)
					{
						handler(eventData);
					}
					Block_8:
					result = eventData;
				}
				catch (Exception e)
				{
					Debug.LogError("Error occurs when loading event from file " + filePath);
					Debug.LogError(e.ToString());
					result = null;
				}
			}
			return result;
		}

		// Token: 0x06004B0D RID: 19213 RVA: 0x00234B90 File Offset: 0x00232D90
		private static void NoneToAlpha_0(string filePath, EventEditorData eventData)
		{
			EventEditorData.LanguageTable<string, string> languageCn = eventData.Language[LanguageTableKeys.Cn];
			string eventContent = eventData.EventContent;
			languageCn.EventContent = eventContent;
			eventData.EventContent = null;
			eventData.TmEdit = Save.GetTimeStamp();
			string saveDir = PlayerPrefs.GetString("EventEditorSaveDataPath");
			string eventGuid = eventData.EventGuid;
			string eventDir = Path.Combine(saveDir, eventGuid);
			string saveUrl = Path.Combine(eventDir, eventGuid + ".twe");
			bool flag = !File.Exists(saveUrl);
			if (flag)
			{
				bool flag2 = !Directory.Exists(eventDir);
				if (flag2)
				{
					Directory.CreateDirectory(eventDir);
				}
				File.Move(filePath, saveUrl);
			}
		}

		// Token: 0x06004B0E RID: 19214 RVA: 0x00234C31 File Offset: 0x00232E31
		private static void InitEvent_Alpha_0(EventEditorData eventData)
		{
		}

		// Token: 0x06004B0F RID: 19215 RVA: 0x00234C34 File Offset: 0x00232E34
		private static void Alpha_0_To_Alpha_1(string filePath, EventEditorData eventData)
		{
			EventEditorData.LanguageTable<string, string> languageTable = eventData.GetDefaultEditLanguageTable();
			eventData.EventContent = languageTable.EventContent;
			Dictionary<int, EventEditorData.Option> options = eventData.Options ?? new Dictionary<int, EventEditorData.Option>();
			foreach (EventEditorData.Option element in options.Values)
			{
				element.InternalContent = languageTable.GetValueOrDefault(element.OptionKey);
			}
		}

		// Token: 0x06004B10 RID: 19216 RVA: 0x00234CBC File Offset: 0x00232EBC
		private static void InitEvent_Alpha_1(EventEditorData eventData)
		{
			bool flag = string.IsNullOrEmpty(eventData.TriggerType) || eventData.TriggerType == "0";
			if (flag)
			{
				eventData.TriggerType = "None";
			}
			string eventType = eventData.EventType;
			bool flag2 = !string.IsNullOrEmpty(eventType);
			if (flag2)
			{
				eventData.EventType = ((EEventType)Enum.Parse(typeof(EEventType), eventType)).ToString();
			}
			Dictionary<int, EventEditorData.Option> options = eventData.Options ?? new Dictionary<int, EventEditorData.Option>();
			foreach (KeyValuePair<int, EventEditorData.Option> keyValuePair in options)
			{
				int num;
				EventEditorData.Option option;
				keyValuePair.Deconstruct(out num, out option);
				EventEditorData.Option optionCellTable = option;
				string optionKey = optionCellTable.OptionKey;
				string guidString = optionCellTable.Guid;
				bool flag3 = string.IsNullOrEmpty(optionKey) && !string.IsNullOrEmpty(guidString);
				if (flag3)
				{
					optionCellTable.OptionKey = string.Format("Option_{0}", guidString.GetHashCode());
				}
			}
		}

		// Token: 0x04003417 RID: 13335
		private readonly Dictionary<string, Action<string, EventEditorData>> _versionUpgradeHandlers = new Dictionary<string, Action<string, EventEditorData>>
		{
			{
				"NoneToAlpha_0",
				new Action<string, EventEditorData>(Load.NoneToAlpha_0)
			},
			{
				"Alpha_0_To_Alpha_1",
				new Action<string, EventEditorData>(Load.Alpha_0_To_Alpha_1)
			}
		};

		// Token: 0x04003418 RID: 13336
		private readonly Dictionary<string, Action<EventEditorData>> _versionOnloadHandler = new Dictionary<string, Action<EventEditorData>>
		{
			{
				Save.Versions[0],
				new Action<EventEditorData>(Load.InitEvent_Alpha_0)
			},
			{
				Save.Versions[1],
				new Action<EventEditorData>(Load.InitEvent_Alpha_1)
			}
		};
	}
}
