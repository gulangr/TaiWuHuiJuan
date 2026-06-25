using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using GameData.Serializer;

namespace EventEditor
{
	// Token: 0x02000631 RID: 1585
	public class Save
	{
		// Token: 0x17000959 RID: 2393
		// (get) Token: 0x06004B12 RID: 19218 RVA: 0x00234E7A File Offset: 0x0023307A
		public static string LatestVersion
		{
			get
			{
				string[] versions = Save.Versions;
				return versions[versions.Length - 1];
			}
		}

		// Token: 0x06004B13 RID: 19219 RVA: 0x00234E88 File Offset: 0x00233088
		public string SaveEvent(EventEditorData eventData)
		{
			bool flag = eventData == null;
			string result;
			if (flag)
			{
				result = "Null eventTable, Save action failed!";
			}
			else
			{
				bool flag2 = this._utf8WithoutBom == null;
				if (flag2)
				{
					this._utf8WithoutBom = new UTF8Encoding(false);
				}
				bool flag3 = !string.IsNullOrEmpty(eventData.AudioName);
				if (flag3)
				{
					string audioFilePath = eventData.AudioName;
					bool flag4 = !string.IsNullOrEmpty(audioFilePath);
					if (flag4)
					{
						FileInfo fileInfo = new FileInfo(audioFilePath);
						string saveDataDir = Save.GetEventSaveDir(eventData);
						bool flag5 = saveDataDir != fileInfo.DirectoryName;
						if (flag5)
						{
							string fileName = fileInfo.Name;
							string filePath = Path.Combine(saveDataDir, fileName).PathFix();
							File.Copy(audioFilePath, filePath);
							eventData.AudioName = fileName;
							List<string> prevAudioFiles = new List<string>(Directory.GetFiles(saveDataDir, "*.ogg", SearchOption.TopDirectoryOnly));
							prevAudioFiles.AddRange(Directory.GetFiles(saveDataDir, "*.wav", SearchOption.TopDirectoryOnly));
							prevAudioFiles.ForEach(delegate(string e)
							{
								bool flag6 = e.PathFix() != filePath;
								if (flag6)
								{
									File.Delete(e);
								}
							});
						}
					}
				}
				eventData.SaveVersion = Save.LatestVersion;
				try
				{
					result = this.SaveAs_Alpha_1(eventData);
				}
				catch (Exception e)
				{
					Exception e2;
					result = string.Format("{0}{1}{2}", e2, Environment.NewLine, e2.Message);
				}
			}
			return result;
		}

		// Token: 0x06004B14 RID: 19220 RVA: 0x00234FDC File Offset: 0x002331DC
		public static long GetTimeStamp()
		{
			return (DateTime.Now.ToUniversalTime().Ticks - 621355968000000000L) / 10000000L;
		}

		// Token: 0x06004B15 RID: 19221 RVA: 0x00235014 File Offset: 0x00233214
		public static string GetEventSaveDir(EventEditorData eventData)
		{
			string saveDataDir = ModManager.GetModEventSaveCore();
			string eventGroup = eventData.EventGroup;
			string eventGuid = eventData.EventGuid;
			bool flag = string.IsNullOrEmpty(eventGroup);
			if (flag)
			{
				eventGroup = "None";
			}
			EventGroupData groupData = SingletonObject.getInstance<EventEditorModel>().GetGroupData(eventGroup);
			bool flag2 = groupData != null;
			string result;
			if (flag2)
			{
				result = Path.Combine(groupData.GroupDirectory, eventGuid).PathFix();
			}
			else
			{
				string groupName;
				bool flag3 = !SingletonObject.getInstance<EventEditorModel>().EventGroupInfoDic.TryGetValue(eventGroup, out groupName);
				if (flag3)
				{
					throw new Exception("EventGroupError:" + eventGuid + " can not find groupName folder of eventGroup:" + eventGroup);
				}
				bool flag4 = saveDataDir.EndsWith(groupName);
				if (flag4)
				{
					groupName = string.Empty;
				}
				result = Path.Combine(saveDataDir, groupName, eventGuid).PathFix();
			}
			return result;
		}

		// Token: 0x06004B16 RID: 19222 RVA: 0x002350D0 File Offset: 0x002332D0
		private string SaveAs_Alpha_1(EventEditorData eventData)
		{
			string eventGuid = eventData.EventGuid;
			string saveDirectory = Save.GetEventSaveDir(eventData);
			bool flag = !Directory.Exists(saveDirectory);
			if (flag)
			{
				Directory.CreateDirectory(saveDirectory);
			}
			string marshalData;
			GameData.Serializer.CommonObjectSerializer.Serialize<EventEditorData>(eventData, out marshalData, GameData.Serializer.CommonObjectSerializer.MarshalFormat.LuaWithReturnPrefix);
			File.WriteAllText(Path.Combine(saveDirectory, eventGuid + ".twe"), marshalData, this._utf8WithoutBom);
			return string.Empty;
		}

		// Token: 0x04003419 RID: 13337
		public static readonly string[] Versions = new string[]
		{
			"Alpha_0",
			"Alpha_1"
		};

		// Token: 0x0400341A RID: 13338
		private Encoding _utf8WithoutBom;
	}
}
