using System;
using System.Collections.Generic;
using GameData.Domains.Mod;
using MoonSharp.Interpreter;
using Steamworks;
using UnityEngine;

namespace FrameWork.ModSystem
{
	// Token: 0x02001047 RID: 4167
	public class ModInfoWithDisplayData : ModInfo
	{
		// Token: 0x17001569 RID: 5481
		// (get) Token: 0x0600BE18 RID: 48664 RVA: 0x00564015 File Offset: 0x00562215
		public int Score
		{
			get
			{
				return Mathf.CeilToInt(this.OriginScore * 5f);
			}
		}

		// Token: 0x0600BE19 RID: 48665 RVA: 0x00564028 File Offset: 0x00562228
		public void SaveUpdateLog(Table table)
		{
			bool flag = this.UpdateLogList != null;
			if (flag)
			{
				for (int i = 0; i < this.UpdateLogList.Count; i++)
				{
					int index = i + 1;
					Table structTable = new Table(null);
					ModInfoWithDisplayData.UpdateLog updateLog = this.UpdateLogList[i];
					structTable.Save("Timestamp", updateLog.Timestamp);
					structTable.Save("LogList", updateLog.LogList);
					table.Save(index, structTable);
				}
			}
		}

		// Token: 0x0600BE1A RID: 48666 RVA: 0x005640AC File Offset: 0x005622AC
		public void LoadUpdateLog(Table updateLogListTable)
		{
			if (this.UpdateLogList == null)
			{
				this.UpdateLogList = new List<ModInfoWithDisplayData.UpdateLog>();
			}
			this.UpdateLogList.Clear();
			for (int i = 1; i <= updateLogListTable.Length; i++)
			{
				Table updateLogTable;
				updateLogListTable.Load(i, out updateLogTable);
				ModInfoWithDisplayData.UpdateLog updateLog = default(ModInfoWithDisplayData.UpdateLog);
				updateLogTable.Load("Timestamp", out updateLog.Timestamp);
				updateLogTable.Load("LogList", out updateLog.LogList);
				this.UpdateLogList.Add(updateLog);
			}
		}

		// Token: 0x0600BE1B RID: 48667 RVA: 0x00564138 File Offset: 0x00562338
		public void ApplySettings()
		{
			this.ModSettings.Clear();
			foreach (SettingEntry settingEntry in this.ModSettingEntries)
			{
				settingEntry.SaveToSerializableModData(this.ModSettings);
			}
		}

		// Token: 0x0600BE1C RID: 48668 RVA: 0x005641A0 File Offset: 0x005623A0
		public void RestoreSettings()
		{
			foreach (SettingEntry settingEntry in this.ModSettingEntries)
			{
				settingEntry.RestoreFromSerializableModData(this.ModSettings);
			}
		}

		// Token: 0x0600BE1D RID: 48669 RVA: 0x005641FC File Offset: 0x005623FC
		public void SaveSettingsToLuaTable(Table luaTable)
		{
			foreach (SettingEntry settingEntry in this.ModSettingEntries)
			{
				settingEntry.SaveToLuaTable(luaTable);
			}
		}

		// Token: 0x0600BE1E RID: 48670 RVA: 0x00564254 File Offset: 0x00562454
		public void LoadSettingsFromLuaTable(Table luaTable)
		{
			foreach (SettingEntry settingEntry in this.ModSettingEntries)
			{
				settingEntry.LoadFromLuaTable(luaTable);
			}
		}

		// Token: 0x0600BE1F RID: 48671 RVA: 0x005642AC File Offset: 0x005624AC
		public void SaveDefaultSettings(Table luaTable)
		{
			for (int index = 0; index < this.ModSettingEntries.Count; index++)
			{
				SettingEntry settingEntry = this.ModSettingEntries[index];
				Table defaultSettingsTable = new Table(null);
				settingEntry.SaveDefaultSetting(defaultSettingsTable);
				luaTable.Save(index + 1, defaultSettingsTable);
			}
		}

		// Token: 0x0600BE20 RID: 48672 RVA: 0x00564300 File Offset: 0x00562500
		public void LoadDefaultSettings(Table luaTable)
		{
			foreach (SettingEntry settingEntry in this.ModSettingEntries)
			{
				settingEntry.LoadDefaultSetting(luaTable);
			}
		}

		// Token: 0x0600BE21 RID: 48673 RVA: 0x00564358 File Offset: 0x00562558
		public ModInfoWithDisplayData()
		{
			this.FrontendPlugins = new List<string>();
			this.FrontendPluginsLegacy = new List<string>();
			this.FrontendPatches = new List<string>();
			this.ModSettingEntries = new List<SettingEntry>();
			this.Dependencies = new List<ulong>();
			this.RemoteDependencies = new List<ulong>();
			this.TagList = new List<string>();
			this.DetailImageList = new List<string>();
			this.ModSettingGroups = new List<string>();
		}

		// Token: 0x04009223 RID: 37411
		public string Author;

		// Token: 0x04009224 RID: 37412
		public string Description;

		// Token: 0x04009225 RID: 37413
		public string Cover;

		// Token: 0x04009226 RID: 37414
		public string WorkshopCover;

		// Token: 0x04009227 RID: 37415
		public List<string> DetailImageList;

		// Token: 0x04009228 RID: 37416
		public int DetailFileCount;

		// Token: 0x04009229 RID: 37417
		public float OriginScore;

		// Token: 0x0400922A RID: 37418
		public uint VoteCount;

		// Token: 0x0400922B RID: 37419
		public ulong SubscribeCount;

		// Token: 0x0400922C RID: 37420
		public ulong FavoriteCount;

		// Token: 0x0400922D RID: 37421
		public int FileSize;

		// Token: 0x0400922E RID: 37422
		public UGCHandle_t PreviewFileHandle = UGCHandle_t.Invalid;

		// Token: 0x0400922F RID: 37423
		public uint CreateData;

		// Token: 0x04009230 RID: 37424
		public uint UpdateData;

		// Token: 0x04009231 RID: 37425
		public List<string> TagList;

		// Token: 0x04009232 RID: 37426
		public bool IsSubscribed;

		// Token: 0x04009233 RID: 37427
		public EModVisibility Visibility = EModVisibility.Public;

		// Token: 0x04009234 RID: 37428
		public List<ulong> Dependencies;

		// Token: 0x04009235 RID: 37429
		public List<ulong> RemoteDependencies;

		// Token: 0x04009236 RID: 37430
		public Table SourceLuaTable;

		// Token: 0x04009237 RID: 37431
		public List<string> FrontendPlugins;

		// Token: 0x04009238 RID: 37432
		public List<string> FrontendPluginsLegacy;

		// Token: 0x04009239 RID: 37433
		public List<string> FrontendPatches;

		// Token: 0x0400923A RID: 37434
		public List<SettingEntry> ModSettingEntries;

		// Token: 0x0400923B RID: 37435
		public List<string> ModSettingGroups;

		// Token: 0x0400923C RID: 37436
		public string GameVersionStr;

		// Token: 0x0400923D RID: 37437
		public Version GameVersion;

		// Token: 0x0400923E RID: 37438
		public List<ModInfoWithDisplayData.UpdateLog> UpdateLogList;

		// Token: 0x0400923F RID: 37439
		public bool ChangeConfig;

		// Token: 0x04009240 RID: 37440
		public bool HasArchive;

		// Token: 0x04009241 RID: 37441
		public bool NeedRestartWhenSettingChanged;

		// Token: 0x02002680 RID: 9856
		public struct UpdateLog
		{
			// Token: 0x0400EAE5 RID: 60133
			public long Timestamp;

			// Token: 0x0400EAE6 RID: 60134
			public List<string> LogList;
		}
	}
}
