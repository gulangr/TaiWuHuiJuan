using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using Config;
using FrameWork;
using FrameWork.ExternalTexture;
using FrameWork.ModSystem;
using GameData.Domains.Global;
using GameData.Domains.Mod;
using GameData.Utilities;
using MoonSharp.Interpreter;
using MoonSharp.Interpreter.Serialization;
using Steamworks;
using TaiwuModdingLib.Core.Plugin;
using UnityEngine;

// Token: 0x02000139 RID: 313
public static class ModManager
{
	// Token: 0x170001C1 RID: 449
	// (get) Token: 0x06001003 RID: 4099 RVA: 0x0005F7B3 File Offset: 0x0005D9B3
	public static bool IsPlatformAvailable
	{
		get
		{
			return GameApp.IsConnectedToInternet() && SteamManager.IsLoggedOn;
		}
	}

	// Token: 0x170001C2 RID: 450
	// (get) Token: 0x06001004 RID: 4100 RVA: 0x0005F7C4 File Offset: 0x0005D9C4
	public static IReadOnlyDictionary<string, ModInfoWithDisplayData> LocalMods
	{
		get
		{
			return ModManager._localMods;
		}
	}

	// Token: 0x170001C3 RID: 451
	// (get) Token: 0x06001005 RID: 4101 RVA: 0x0005F7CB File Offset: 0x0005D9CB
	public static Dictionary<string, ulong> ModNameToTempFileId
	{
		get
		{
			return ModManager._modNameToTempFileId;
		}
	}

	// Token: 0x170001C4 RID: 452
	// (get) Token: 0x06001006 RID: 4102 RVA: 0x0005F7D2 File Offset: 0x0005D9D2
	public static IReadOnlyCollection<string> WhitelistMods
	{
		get
		{
			return ModManager._whitelistMods;
		}
	}

	// Token: 0x170001C5 RID: 453
	// (get) Token: 0x06001007 RID: 4103 RVA: 0x0005F7D9 File Offset: 0x0005D9D9
	public static IReadOnlyDictionary<string, int> ModOrder
	{
		get
		{
			return ModManager._modOrder;
		}
	}

	// Token: 0x06001008 RID: 4104 RVA: 0x0005F7E0 File Offset: 0x0005D9E0
	public static IEnumerator Init()
	{
		bool flag = !LuaGame.LuaReady;
		if (flag)
		{
			Debug.LogWarning("LuaGame not ready.");
			yield break;
		}
		ModManager._localMods = new Dictionary<string, ModInfoWithDisplayData>();
		ModManager._loadedMods = new List<ModId>();
		ModManager._whitelistMods = new HashSet<string>();
		ModManager.EnabledMods = new List<ModId>();
		ModManager._loadedPlugins = new Dictionary<ModId, List<TaiwuRemakePlugin>>();
		ModManager._modNameToTempFileId = new Dictionary<string, ulong>();
		ModManager._modCoverTextureGroup = new ModTextureGroup();
		ModManager._modNameCache = new Dictionary<string, string>();
		ModManager._configItemsToBeLoaded = new Dictionary<string, object>[ConfigCollection.Items.Length];
		int num;
		for (int i = 0; i < ModManager._configItemsToBeLoaded.Length; i = num + 1)
		{
			ModManager._configItemsToBeLoaded[i] = new Dictionary<string, object>();
			num = i;
		}
		ModManager.InitPath();
		IEnumerator item = ModManager.UpdateModListImpl();
		while (item.MoveNext())
		{
			yield return item;
		}
		yield break;
	}

	// Token: 0x06001009 RID: 4105 RVA: 0x0005F7E8 File Offset: 0x0005D9E8
	public static void Clear()
	{
		ModManager.ResetRegisterCallback();
		SteamManager.Clear();
		ModManager._localMods.Clear();
		ModManager.PlatformMods.Clear();
		ModManager.EnabledMods.Clear();
		ModManager.ExternalMods.Clear();
		ModManager.UploadedMods.Clear();
		ModManager._modNameToTempFileId.Clear();
		ModManager._whitelistMods.Clear();
		ModManager.DisposeModTexture();
	}

	// Token: 0x0600100A RID: 4106 RVA: 0x0005F855 File Offset: 0x0005DA55
	public static void DisposeModTexture()
	{
		ModManager._modCoverTextureGroup.Dispose();
	}

	// Token: 0x0600100B RID: 4107 RVA: 0x0005F863 File Offset: 0x0005DA63
	public static void UpdateModList(Action callback = null)
	{
		SingletonObject.getInstance<YieldHelper>().StartYield(ModManager.<UpdateModList>g__Co|39_0(callback));
	}

	// Token: 0x0600100C RID: 4108 RVA: 0x0005F878 File Offset: 0x0005DA78
	private static void ShowDeleteOrContinueDialog(string configPath)
	{
		UIElement dialog = UIElement.Dialog;
		ArgumentBox argumentBox = EasyPool.Get<ArgumentBox>();
		string key = "Cmd";
		DialogCmd dialogCmd = new DialogCmd();
		dialogCmd.Title = LanguageKey.LK_Mod_Load_Error_Title.Tr();
		dialogCmd.Content = LanguageKey.LK_Mod_Load_Error_Desc.TrFormat(configPath).ColorReplace();
		dialogCmd.GroupYesText = LanguageKey.LK_Mod_Load_Error_AutoMode.Tr();
		dialogCmd.GroupNoText = LanguageKey.LK_Mod_Load_Error_ManualMode.Tr();
		dialogCmd.Yes = delegate()
		{
			File.Delete(configPath);
			bool flag = ModManager._next != 0;
			if (!flag)
			{
				ModManager._next = 2;
				UIElement.Dialog.Hide(false);
			}
		};
		dialogCmd.No = delegate()
		{
			bool flag = ModManager._next != 0;
			if (!flag)
			{
				ModManager._next = 1;
				UIElement.Dialog.Hide(false);
			}
		};
		dialog.SetOnInitArgs(argumentBox.SetObject(key, dialogCmd));
		UIElement.Dialog.Show();
	}

	// Token: 0x0600100D RID: 4109 RVA: 0x0005F940 File Offset: 0x0005DB40
	private static void ShowModLoadErrorDialog(ModInfoWithDisplayData modData)
	{
		new DialogCmdHuge
		{
			Title = LanguageKey.LK_Mod_Load_Error_Loading_Title.Tr(),
			Content = LanguageKey.LK_Mod_Load_Error_Loading_Desc.TrFormat(modData.Title).ColorReplace(),
			LeftText = LanguageKey.LK_Mod_Load_Error_Loading_Left.Tr(),
			MiddleText = LanguageKey.LK_Mod_Load_Error_Loading_Middle.Tr(),
			RightText = LanguageKey.LK_Mod_Load_Error_Loading_Right.Tr(),
			LeftTips = LanguageKey.LK_Mod_Load_Error_Loading_Left_Tips.Tr(),
			MiddleTips = LanguageKey.LK_Mod_Load_Error_Loading_Middle_Tips.Tr(),
			RightTips = LanguageKey.LK_Mod_Load_Error_Loading_Right_Tips.Tr(),
			Left = delegate()
			{
				bool flag = ModManager._next != 0;
				if (!flag)
				{
					ModManager.SetModEnabled(modData.ModId, false);
					ModManager.EnabledMods.Clear();
					ModManager.SaveModSettings(false);
					GameApp.Instance.ReStart();
					UIElement.Dialog.Hide(false);
				}
			},
			Middle = delegate()
			{
				bool flag = ModManager._next != 0;
				if (!flag)
				{
					ModManager.SetModEnabled(modData.ModId, false);
					ModManager.EnabledMods.Remove(modData.ModId);
					ModManager.SaveModSettings(false);
					GameApp.Instance.ReStart();
					UIElement.Dialog.Hide(false);
				}
			},
			Right = delegate()
			{
				bool flag = ModManager._next != 0;
				if (!flag)
				{
					ModManager.SetModEnabled(modData.ModId, false);
					ModManager.EnabledMods.Remove(modData.ModId);
					ModManager.SaveModSettings(false);
					ModManager._next = 2;
					UIElement.Dialog.Hide(false);
				}
			}
		}.Show();
	}

	// Token: 0x0600100E RID: 4110 RVA: 0x0005FA2C File Offset: 0x0005DC2C
	private static void ShowModLoadErrorSummaryDialog()
	{
		UIElement dialog = UIElement.Dialog;
		ArgumentBox argumentBox = EasyPool.Get<ArgumentBox>();
		string key = "Cmd";
		DialogCmd dialogCmd = new DialogCmd();
		dialogCmd.Title = LanguageKey.LK_Mod_Load_Error_Loading_Summary_Title.Tr();
		dialogCmd.Content = LanguageKey.LK_Mod_Load_Error_Loading_Summary_Desc.Tr().ColorReplace();
		dialogCmd.GroupYesText = LanguageKey.LK_Mod_Load_Error_Loading_Summary_Yes.Tr();
		dialogCmd.GroupNoText = LanguageKey.LK_Mod_Load_Error_Loading_Summary_No.Tr();
		dialogCmd.Yes = delegate()
		{
			GameApp.Instance.ReStart();
			UIElement.Dialog.Hide(false);
		};
		dialogCmd.No = delegate()
		{
			bool flag = ModManager._next != 0;
			if (!flag)
			{
				ModManager._next = 2;
				UIElement.Dialog.Hide(false);
			}
		};
		dialog.SetOnInitArgs(argumentBox.SetObject(key, dialogCmd));
		UIElement.Dialog.Show();
	}

	// Token: 0x0600100F RID: 4111 RVA: 0x0005FAF3 File Offset: 0x0005DCF3
	public static IEnumerator UpdateModListImpl()
	{
		ModManager.EnabledMods.Clear();
		ModManager._modNameToTempFileId.Clear();
		ModManager._whitelistMods.Clear();
		SteamManager.ReadSubscribedItems(ModManager.PlatformMods, ModManager._localMods);
		string configPath = Path.Combine(GameApp.GetArchiveDirPath(), "ModSettings.Lua");
		bool flag = !File.Exists(configPath);
		if (flag)
		{
			ModManager.ReadLocalMods();
			yield break;
		}
		Table luaTable = null;
		for (;;)
		{
			ModManager._next = 0;
			string text = File.ReadAllText(configPath);
			try
			{
				luaTable = LuaGame.Instance.ReadMoonSharpTable(text);
				break;
			}
			catch (Exception ex)
			{
				Debug.LogWarning("Invalid LuaTable: \n" + text);
				luaTable = null;
				ModManager.ShowDeleteOrContinueDialog(configPath);
			}
			yield return new WaitUntil(() => ModManager._next != 0);
			bool flag2 = ModManager._next == 2;
			if (flag2)
			{
				break;
			}
			text = null;
		}
		bool flag3 = luaTable != null;
		if (flag3)
		{
			Table modNameToTempFileIdTable;
			luaTable.Load("ModNameToTempFileIdCache", out modNameToTempFileIdTable);
			bool flag4 = modNameToTempFileIdTable != null;
			if (flag4)
			{
				foreach (TablePair pair in modNameToTempFileIdTable.Pairs)
				{
					ulong fileId = pair.Value.ToObject<ulong>();
					bool flag5 = fileId <= 0UL;
					if (flag5)
					{
						fileId = 1UL + modNameToTempFileIdTable.Pairs.Max((TablePair p) => p.Value.ToObject<ulong>());
					}
					ModManager._modNameToTempFileId[pair.Key.String] = fileId;
					pair = default(TablePair);
				}
				IEnumerator<TablePair> enumerator = null;
			}
			Table whitelistModsTable;
			luaTable.Load("WhitelistMods", out whitelistModsTable);
			bool flag6 = whitelistModsTable != null;
			if (flag6)
			{
				foreach (TablePair pair2 in whitelistModsTable.Pairs)
				{
					string value = pair2.Value.String;
					bool flag7 = string.IsNullOrEmpty(value);
					if (!flag7)
					{
						ModManager._whitelistMods.Add(value);
						value = null;
						pair2 = default(TablePair);
					}
				}
				IEnumerator<TablePair> enumerator2 = null;
			}
			Table modOrderTable;
			luaTable.Load("ModOrder", out modOrderTable);
			bool flag8 = modOrderTable != null;
			if (flag8)
			{
				foreach (TablePair pair3 in modOrderTable.Pairs)
				{
					ModManager._modOrder[pair3.Key.String] = Math.Clamp(pair3.Value.ToObject<int>(), 0, 9999);
					pair3 = default(TablePair);
				}
				IEnumerator<TablePair> enumerator3 = null;
			}
			ModManager.ReadLocalMods();
			ModManager.LoadEnabledModsFromLuaTable(luaTable);
			modNameToTempFileIdTable = null;
			whitelistModsTable = null;
			modOrderTable = null;
		}
		else
		{
			ModManager.ReadLocalMods();
		}
		yield break;
	}

	// Token: 0x170001C6 RID: 454
	// (get) Token: 0x06001010 RID: 4112 RVA: 0x0005FAFB File Offset: 0x0005DCFB
	// (set) Token: 0x06001011 RID: 4113 RVA: 0x0005FB02 File Offset: 0x0005DD02
	public static Action<Dictionary<ModId, bool>> OnUpdateSubscribedItems { get; private set; }

	// Token: 0x170001C7 RID: 455
	// (get) Token: 0x06001012 RID: 4114 RVA: 0x0005FB0A File Offset: 0x0005DD0A
	// (set) Token: 0x06001013 RID: 4115 RVA: 0x0005FB11 File Offset: 0x0005DD11
	public static Action<uint, List<ModId>> OnUpdateAllItems { get; private set; }

	// Token: 0x170001C8 RID: 456
	// (get) Token: 0x06001014 RID: 4116 RVA: 0x0005FB19 File Offset: 0x0005DD19
	// (set) Token: 0x06001015 RID: 4117 RVA: 0x0005FB20 File Offset: 0x0005DD20
	public static Action<Dictionary<ModId, bool>> OnUpdateUploadedItems { get; private set; }

	// Token: 0x170001C9 RID: 457
	// (get) Token: 0x06001016 RID: 4118 RVA: 0x0005FB28 File Offset: 0x0005DD28
	// (set) Token: 0x06001017 RID: 4119 RVA: 0x0005FB2F File Offset: 0x0005DD2F
	public static Action<Dictionary<ModId, bool>> OnUpdateTargetItems { get; private set; }

	// Token: 0x06001018 RID: 4120 RVA: 0x0005FB37 File Offset: 0x0005DD37
	public static void UpdateSubscribedItems(Action<Dictionary<ModId, bool>> onFinished)
	{
		ModManager.OnUpdateSubscribedItems = onFinished;
		ModManager.SubscribedMods.Clear();
		SteamManager.UpdateSubscribedItems(ModManager.PlatformMods, ModManager._localMods, ModManager._modNameCache);
	}

	// Token: 0x06001019 RID: 4121 RVA: 0x0005FB61 File Offset: 0x0005DD61
	public static void UpdateAllItems(string searchText, List<string> searchTags, uint pageCount, Action<uint, List<ModId>> onFinished)
	{
		ModManager.OnUpdateAllItems = onFinished;
		SteamManager.UpdateAllItems(ModManager.PlatformMods, ModManager._localMods, ModManager._modNameCache, searchText, searchTags, pageCount);
	}

	// Token: 0x0600101A RID: 4122 RVA: 0x0005FB83 File Offset: 0x0005DD83
	public static void UpdateUploadedItems(Action<Dictionary<ModId, bool>> onFinished)
	{
		ModManager.OnUpdateUploadedItems = onFinished;
		ModManager.UploadedMods.Clear();
		SteamManager.UpdateUploadedItems(ModManager.PlatformMods, ModManager._localMods, ModManager._modNameCache);
	}

	// Token: 0x0600101B RID: 4123 RVA: 0x0005FBAD File Offset: 0x0005DDAD
	public static void UpdateTargetItems(IReadOnlyList<ulong> targetList, Action<Dictionary<ModId, bool>> onFinished)
	{
		ModManager.OnUpdateTargetItems = onFinished;
		SteamManager.UpdateTargetItems(ModManager.PlatformMods, ModManager._localMods, ModManager._modNameCache, targetList, null);
	}

	// Token: 0x0600101C RID: 4124 RVA: 0x0005FBCE File Offset: 0x0005DDCE
	public static void UpdateTargetItems(IReadOnlyList<ulong> targetList, List<ulong> missingList, Action<Dictionary<ModId, bool>> onFinished)
	{
		ModManager.OnUpdateTargetItems = onFinished;
		SteamManager.UpdateTargetItems(ModManager.PlatformMods, ModManager._localMods, ModManager._modNameCache, targetList, missingList);
	}

	// Token: 0x0600101D RID: 4125 RVA: 0x0005FBEF File Offset: 0x0005DDEF
	internal static void ResetRegisterCallback()
	{
		ModManager.OnUpdateSubscribedItems = null;
		ModManager.OnUpdateAllItems = null;
		ModManager.OnUpdateUploadedItems = null;
		ModManager.OnUpdateTargetItems = null;
	}

	// Token: 0x0600101E RID: 4126 RVA: 0x0005FC0E File Offset: 0x0005DE0E
	public static IEnumerator RestoreModConfigImpl()
	{
		string configPath = Path.Combine(GameApp.GetArchiveDirPath(), "ModSettings.Lua");
		bool flag = !File.Exists(configPath);
		if (flag)
		{
			yield break;
		}
		string text = File.ReadAllText(configPath);
		Table luaTable = null;
		bool flag2;
		do
		{
			ModManager._next = 0;
			try
			{
				luaTable = LuaGame.Instance.ReadMoonSharpTable(text);
				break;
			}
			catch (Exception ex)
			{
				Debug.LogWarning("Invalid LuaTable: \n" + text);
				luaTable = null;
				ModManager.ShowDeleteOrContinueDialog(configPath);
			}
			yield return new WaitUntil(() => ModManager._next != 0);
			flag2 = (ModManager._next == 2);
		}
		while (!flag2);
		bool flag3 = luaTable != null;
		if (flag3)
		{
			ModManager.LoadEnabledModsFromLuaTable(luaTable);
		}
		else
		{
			ModManager.EnabledMods.Clear();
		}
		foreach (ModInfoWithDisplayData modInfo in ModManager._localMods.Values)
		{
			modInfo.RestoreSettings();
			modInfo = null;
		}
		Dictionary<string, ModInfoWithDisplayData>.ValueCollection.Enumerator enumerator = default(Dictionary<string, ModInfoWithDisplayData>.ValueCollection.Enumerator);
		yield break;
	}

	// Token: 0x0600101F RID: 4127 RVA: 0x0005FC18 File Offset: 0x0005DE18
	public static bool IsModOutdated(ModInfoWithDisplayData modInfo)
	{
		bool flag = modInfo.GameVersion == null || modInfo.GameVersion < ModManager.CutVersion;
		bool result;
		if (flag)
		{
			result = true;
		}
		else
		{
			Version maxVersion = GameApp.Instance.ParsedGameVersion;
			bool flag2 = maxVersion == null;
			if (flag2)
			{
				result = false;
			}
			else
			{
				bool flag3 = maxVersion.Major != modInfo.GameVersion.Major || maxVersion.Minor != modInfo.GameVersion.Minor;
				if (flag3)
				{
					result = true;
				}
				else
				{
					int build = maxVersion.Build;
					List<string> list = modInfo.BackendPluginsLegacy;
					bool flag4;
					if (list == null || list.Count <= 0)
					{
						list = modInfo.FrontendPluginsLegacy;
						flag4 = (list != null && list.Count > 0);
					}
					else
					{
						flag4 = true;
					}
					bool flag5 = flag4;
					if (flag5)
					{
						build++;
					}
					result = (modInfo.GameVersion.Build > build);
				}
			}
		}
		return result;
	}

	// Token: 0x06001020 RID: 4128 RVA: 0x0005FCFC File Offset: 0x0005DEFC
	public static bool IsModUseLegacy(ModInfoWithDisplayData modInfo)
	{
		bool flag = modInfo.GameVersion == null;
		bool result;
		if (flag)
		{
			result = false;
		}
		else
		{
			Version gameVersion = GameApp.Instance.ParsedGameVersion;
			bool flag2 = gameVersion == null;
			result = (!flag2 && modInfo.GameVersion > gameVersion);
		}
		return result;
	}

	// Token: 0x06001021 RID: 4129 RVA: 0x0005FD48 File Offset: 0x0005DF48
	private static void LoadEnabledModsFromLuaTable(Table table)
	{
		ModManager.EnabledMods.Clear();
		bool flag = table.ContainsKey("EnabledWorkshopMods");
		if (flag)
		{
			List<string> mods;
			table.Load("EnabledWorkshopMods", out mods);
			foreach (string modIdStr in mods)
			{
				ModInfoWithDisplayData modInfo;
				bool flag2 = !ModManager._localMods.TryGetValue(modIdStr, out modInfo) || ModManager.EnabledMods.Contains(ModManager._localMods[modIdStr].ModId);
				if (!flag2)
				{
					bool flag3 = !ModManager.IsModOutdated(modInfo);
					if (flag3)
					{
						ModManager.RemoveFromWhitelist(modInfo);
					}
					else
					{
						bool flag4 = !ModManager._whitelistMods.Contains(modIdStr);
						if (flag4)
						{
							continue;
						}
					}
					ModManager.EnabledMods.Add(modInfo.ModId);
				}
			}
		}
		bool flag5 = table.ContainsKey("EnabledExternalMods");
		if (flag5)
		{
			List<string> mods2;
			table.Load("EnabledExternalMods", out mods2);
			foreach (string modName in mods2)
			{
				ulong tempFileId;
				bool flag6 = !ModManager._modNameToTempFileId.TryGetValue(modName, out tempFileId);
				if (flag6)
				{
					Debug.LogWarning("Mod " + modName + " can't be found as a external mod.");
				}
				else
				{
					ModId modId = new ModId(tempFileId, 0UL, 0);
					string modIdStr2 = modId.ToString();
					ModInfoWithDisplayData modInfo2;
					bool flag7 = !ModManager._localMods.TryGetValue(modIdStr2, out modInfo2) || ModManager.EnabledMods.Contains(modId);
					if (!flag7)
					{
						modId = modInfo2.ModId;
						bool flag8 = !ModManager.IsModOutdated(modInfo2);
						if (flag8)
						{
							ModManager.RemoveFromWhitelist(modInfo2);
						}
						else
						{
							bool flag9 = !ModManager._whitelistMods.Contains(modIdStr2);
							if (flag9)
							{
								continue;
							}
						}
						ModManager.EnabledMods.Add(modId);
					}
				}
			}
		}
	}

	// Token: 0x06001022 RID: 4130 RVA: 0x0005FF64 File Offset: 0x0005E164
	private static void RemoveFromWhitelist(ModInfo modInfo)
	{
		bool remove = ModManager._whitelistMods.Remove(modInfo.ModId.ToString());
		bool flag = !remove;
		if (!flag)
		{
			foreach (KeyValuePair<string, ModInfoWithDisplayData> keyValuePair in ModManager._localMods)
			{
				string text;
				ModInfoWithDisplayData modInfoWithDisplayData;
				keyValuePair.Deconstruct(out text, out modInfoWithDisplayData);
				ModInfoWithDisplayData value = modInfoWithDisplayData;
				bool flag2 = value.ModId.Source != modInfo.ModId.Source;
				if (!flag2)
				{
					List<ulong> dependencies = value.Dependencies;
					bool flag3 = dependencies != null && dependencies.Contains(modInfo.ModId.FileId);
					if (flag3)
					{
						ModManager.RemoveFromWhitelist(value);
					}
				}
			}
		}
	}

	// Token: 0x06001023 RID: 4131 RVA: 0x00060044 File Offset: 0x0005E244
	public static void SaveModSettings(bool onlySaveCommonSetting = false)
	{
		Table table = new Table(null);
		string archiveDir = GameApp.GetArchiveDirPath();
		bool flag = !Directory.Exists(archiveDir);
		if (flag)
		{
			Directory.CreateDirectory(archiveDir);
		}
		StreamWriter writer = File.CreateText(Path.Combine(archiveDir, "ModSettings.Lua"));
		List<string> enabledWorkshopMods = new List<string>();
		List<string> enabledExternalMods = new List<string>();
		foreach (ModId modId in ModManager.EnabledMods)
		{
			bool flag2 = modId.Source == 1;
			if (flag2)
			{
				enabledWorkshopMods.Add(modId.ToString());
			}
			else
			{
				enabledExternalMods.Add(Path.GetFileName(ModManager._localMods[modId.ToString()].DirectoryName));
			}
		}
		Table modNameToTempFileIdTable = new Table(null);
		foreach (KeyValuePair<string, ulong> pair in ModManager._modNameToTempFileId)
		{
			modNameToTempFileIdTable.Save(pair.Key, pair.Value);
		}
		table.Save("EnabledWorkshopMods", enabledWorkshopMods);
		table.Save("EnabledExternalMods", enabledExternalMods);
		table.Save("WhitelistMods", ModManager._whitelistMods);
		table.Save("ModOrder", ModManager._modOrder);
		table.Save("ModNameToTempFileIdCache", modNameToTempFileIdTable);
		writer.Write(table.Serialize(true, 0));
		writer.Close();
		if (!onlySaveCommonSetting)
		{
			foreach (ModInfoWithDisplayData modInfo in ModManager._localMods.Values)
			{
				bool flag3 = modInfo.DirectoryName.IsNullOrEmpty();
				if (!flag3)
				{
					bool flag4 = !Directory.Exists(modInfo.DirectoryName);
					if (!flag4)
					{
						bool flag5 = modInfo.ModId.Source == 1 && !SteamManager.IsItemStateActive(new PublishedFileId_t(modInfo.ModId.FileId), EItemState.k_EItemStateInstalled);
						if (!flag5)
						{
							Table settingTable = new Table(null);
							modInfo.SaveSettingsToLuaTable(settingTable);
							writer = File.CreateText(Path.Combine(modInfo.DirectoryName, "Settings.Lua"));
							writer.Write(settingTable.Serialize(true, 0));
							writer.Close();
						}
					}
				}
			}
		}
	}

	// Token: 0x06001024 RID: 4132 RVA: 0x000602DC File Offset: 0x0005E4DC
	public static void CheckArchiveLoadedMods(ArchiveInfo archiveInfo, List<ModId> modsNotInstalled, List<ModId> modsVersionNotMatch, List<ModId> modsNotEnabled, List<ModId> modsToAdd)
	{
		List<ModId> modIds = new List<ModId>();
		modsNotInstalled.Clear();
		modsVersionNotMatch.Clear();
		modsNotEnabled.Clear();
		modsToAdd.Clear();
		foreach (ModId modId2 in modIds)
		{
			string fileIdStr = modId2.ToString();
			ModInfoWithDisplayData localMod;
			bool flag = !ModManager._localMods.TryGetValue(fileIdStr, out localMod);
			if (flag)
			{
				modsNotInstalled.Add(modId2);
			}
			else
			{
				bool flag2 = !localMod.ModId.Equals(modId2);
				if (flag2)
				{
					modsVersionNotMatch.Add(localMod.ModId);
				}
				else
				{
					bool flag3 = !ModManager._loadedMods.Contains(localMod.ModId);
					if (flag3)
					{
						modsNotEnabled.Add(localMod.ModId);
					}
				}
			}
		}
		using (List<ModId>.Enumerator enumerator2 = ModManager._loadedMods.GetEnumerator())
		{
			while (enumerator2.MoveNext())
			{
				ModId loadedMod = enumerator2.Current;
				bool flag4 = !modIds.Exists((ModId modId) => modId.FileId == loadedMod.FileId);
				if (flag4)
				{
					modsToAdd.Add(loadedMod);
				}
			}
		}
	}

	// Token: 0x06001025 RID: 4133 RVA: 0x00060444 File Offset: 0x0005E644
	public static ModInfoWithDisplayData GetModInfo(ModId modId)
	{
		string modIdStr = modId.ToString();
		return ModManager.GetModInfo(modIdStr);
	}

	// Token: 0x06001026 RID: 4134 RVA: 0x0006046C File Offset: 0x0005E66C
	public static ModInfoWithDisplayData GetModInfo(string modIdStr)
	{
		ModInfoWithDisplayData localMod;
		bool flag = ModManager._localMods.TryGetValue(modIdStr, out localMod);
		ModInfoWithDisplayData result;
		if (flag)
		{
			result = localMod;
		}
		else
		{
			result = null;
		}
		return result;
	}

	// Token: 0x06001027 RID: 4135 RVA: 0x00060494 File Offset: 0x0005E694
	public static string GetModName(string modIdStr)
	{
		string modName;
		bool flag = ModManager._modNameCache.TryGetValue(modIdStr, out modName);
		string result;
		if (flag)
		{
			result = modName;
		}
		else
		{
			ModInfoWithDisplayData modInfo = ModManager.GetModInfo(modIdStr);
			bool flag2 = modInfo != null;
			if (flag2)
			{
				result = modInfo.Title;
			}
			else
			{
				result = string.Empty;
			}
		}
		return result;
	}

	// Token: 0x06001028 RID: 4136 RVA: 0x000604DA File Offset: 0x0005E6DA
	public static void SetWorkingMod(string modName)
	{
		ModManager._workingModName = modName;
	}

	// Token: 0x06001029 RID: 4137 RVA: 0x000604E4 File Offset: 0x0005E6E4
	public static bool IsModEnabled(ModId modId)
	{
		return ModManager.EnabledMods.Contains(modId);
	}

	// Token: 0x0600102A RID: 4138 RVA: 0x00060504 File Offset: 0x0005E704
	public static bool IsModEnabled(string modIdStr)
	{
		ModInfoWithDisplayData value;
		bool flag = !ModManager._localMods.TryGetValue(modIdStr, out value);
		return !flag && ModManager.EnabledMods.Contains(value.ModId);
	}

	// Token: 0x0600102B RID: 4139 RVA: 0x00060540 File Offset: 0x0005E740
	public static void SetModEnabled(ModId modId, bool isEnabled)
	{
		if (isEnabled)
		{
			bool flag = ModManager.IsModEnabled(modId);
			if (flag)
			{
				Debug.LogWarning("Mod " + modId.ToString() + " is being enabled a second time.");
			}
			else
			{
				ModManager._whitelistMods.Add(modId.ToString());
				ModManager.EnabledMods.Add(modId);
			}
		}
		else
		{
			ModManager.EnabledMods.Remove(modId);
			ModManager._whitelistMods.Remove(modId.ToString());
		}
	}

	// Token: 0x0600102C RID: 4140 RVA: 0x000605CF File Offset: 0x0005E7CF
	public static IEnumerator LoadAllEnabledMods()
	{
		object obj = ModManager.OnLoadingModLock;
		lock (obj)
		{
			ModManager._hasError = false;
			ModManager.UnloadAllMods();
			ModManager.<>c__DisplayClass82_0 CS$<>8__locals1;
			CS$<>8__locals1.orderQueue = new Queue<ModId>();
			ModManager.EnabledMods.Sort(new Comparison<ModId>(ModManager.CompareMods));
			foreach (ModId modId in ModManager.EnabledMods)
			{
				ModManager.<LoadAllEnabledMods>g__HandleDependencyOrder|82_0(modId, ref CS$<>8__locals1);
				modId = default(ModId);
			}
			List<ModId>.Enumerator enumerator = default(List<ModId>.Enumerator);
			foreach (ModId modId2 in CS$<>8__locals1.orderQueue)
			{
				ModManager._next = 0;
				ModInfoWithDisplayData modInfo;
				bool flag2 = !ModManager._localMods.TryGetValue(modId2.ToString(), out modInfo);
				if (!flag2)
				{
					try
					{
						ModManager.LoadMod(modInfo);
						continue;
					}
					catch (Exception ex)
					{
						Exception e = ex;
						ModManager._hasError = true;
						string str = "Loading - ";
						string title = modInfo.Title;
						string str2 = "\n";
						Exception ex2 = e;
						Debug.LogError(str + title + str2 + ((ex2 != null) ? ex2.ToString() : null));
						ModManager.ShowModLoadErrorDialog(modInfo);
					}
					yield return new WaitUntil(() => ModManager._next != 0);
					bool flag3 = ModManager._next == 2;
					if (flag3)
					{
						break;
					}
					modInfo = null;
					modId2 = default(ModId);
				}
			}
			Queue<ModId>.Enumerator enumerator2 = default(Queue<ModId>.Enumerator);
			bool hasError = ModManager._hasError;
			if (hasError)
			{
				ModManager._next = 0;
				ModManager.ShowModLoadErrorSummaryDialog();
				yield return new WaitUntil(() => ModManager._next != 0);
			}
			ModManager._hasError = false;
			Debug.Log(string.Format("Total {0} mods loaded into the game.", ModManager._loadedMods.Count));
			CS$<>8__locals1 = default(ModManager.<>c__DisplayClass82_0);
		}
		obj = null;
		yield break;
		yield break;
	}

	// Token: 0x0600102D RID: 4141 RVA: 0x000605D8 File Offset: 0x0005E7D8
	public static ModInfoList GetLoadedModInfoList()
	{
		ModInfoList modInfoList = ModInfoList.Create();
		object onLoadingModLock = ModManager.OnLoadingModLock;
		lock (onLoadingModLock)
		{
			foreach (ModId modId in ModManager._loadedMods)
			{
				modInfoList.Items.Add(ModManager._localMods[modId.ToString()]);
			}
		}
		return modInfoList;
	}

	// Token: 0x0600102E RID: 4142 RVA: 0x00060684 File Offset: 0x0005E884
	public static void UpdateModSettingsInGame(ModId modId)
	{
		ModInfoWithDisplayData modInfo = ModManager._localMods[modId.ToString()];
		modInfo.ApplySettings();
		foreach (TaiwuRemakePlugin plugins in ModManager._loadedPlugins[modId])
		{
			plugins.OnModSettingUpdate();
		}
		ModDomainMethod.Call.UpdateModSettings(modId, modInfo.ModSettings);
	}

	// Token: 0x0600102F RID: 4143 RVA: 0x00060710 File Offset: 0x0005E910
	public static string GetTextureGroupKey(ModId modId)
	{
		return "ModTexture_" + modId.ToString();
	}

	// Token: 0x06001030 RID: 4144 RVA: 0x0006073C File Offset: 0x0005E93C
	public static string GetGraphicsTextureGroupKey(ModId modId)
	{
		return "ModGraphicsTexture_" + modId.ToString();
	}

	// Token: 0x06001031 RID: 4145 RVA: 0x00060768 File Offset: 0x0005E968
	public static Texture2D GetModCoverTexture(ModId modId)
	{
		return ModManager._modCoverTextureGroup.GetTexture(modId.ToString());
	}

	// Token: 0x06001032 RID: 4146 RVA: 0x00060791 File Offset: 0x0005E991
	public static void AddModCoverTexture(ModId modId, string path)
	{
		ModManager._modCoverTextureGroup.AddTexture(modId.ToString(), path);
	}

	// Token: 0x06001033 RID: 4147 RVA: 0x000607AC File Offset: 0x0005E9AC
	public static void RemoveModCoverTexture(ModId modId)
	{
		ModManager._modCoverTextureGroup.RemoveTexture(modId.ToString());
	}

	// Token: 0x06001034 RID: 4148 RVA: 0x000607C6 File Offset: 0x0005E9C6
	public static void AddPreviewModCoverTexture(ModId modId, Texture2D texture)
	{
		ModManager._modCoverTextureGroup.AddPreviewTextureCache(modId, texture);
	}

	// Token: 0x06001035 RID: 4149 RVA: 0x000607D5 File Offset: 0x0005E9D5
	public static Texture2D GetPreviewModCoverTexture(ModId modId)
	{
		return ModManager._modCoverTextureGroup.GetPreviewTextureCache(modId);
	}

	// Token: 0x06001036 RID: 4150 RVA: 0x000607E2 File Offset: 0x0005E9E2
	public static bool HasPreviewModCoverTexture(ModId modId)
	{
		return ModManager._modCoverTextureGroup.HasPreviewTextureCache(modId);
	}

	// Token: 0x06001037 RID: 4151 RVA: 0x000607EF File Offset: 0x0005E9EF
	public static Texture2D GetModDetailTexture(ModId modId, string imageName)
	{
		return ModManager._modCoverTextureGroup.GetTexture(modId.ToString() + imageName);
	}

	// Token: 0x06001038 RID: 4152 RVA: 0x0006080E File Offset: 0x0005EA0E
	public static void AddDetailModCoverTexture(ModId modId, Texture2D texture)
	{
		ModManager._modCoverTextureGroup.AddDetailTextureCache(modId, texture);
	}

	// Token: 0x06001039 RID: 4153 RVA: 0x0006081D File Offset: 0x0005EA1D
	public static IReadOnlyList<Texture2D> GetDetailModCoverTexture(ModId modId)
	{
		return ModManager._modCoverTextureGroup.GetDetailTextureCache(modId);
	}

	// Token: 0x0600103A RID: 4154 RVA: 0x0006082C File Offset: 0x0005EA2C
	public static ModId CreateTempModId(string modName, bool forceCreate = false)
	{
		ulong tempFileId;
		bool flag = !forceCreate && ModManager._modNameToTempFileId.TryGetValue(modName, out tempFileId) && tempFileId > 0UL;
		if (!flag)
		{
			for (tempFileId = 1UL; tempFileId < 18446744073709551615UL; tempFileId += 1UL)
			{
				bool flag2 = ModManager._modNameToTempFileId.ContainsValue(tempFileId);
				if (!flag2)
				{
					ModManager._modNameToTempFileId[modName] = tempFileId;
					return new ModId(tempFileId, 0UL, 0);
				}
			}
			throw new Exception("Maximum mod count reached.");
		}
		return new ModId(tempFileId, 0UL, 0);
	}

	// Token: 0x0600103B RID: 4155 RVA: 0x000608B0 File Offset: 0x0005EAB0
	private static void ReadLocalMods()
	{
		ModManager.ExternalMods.Clear();
		List<ModId> modIds = new List<ModId>();
		string[] directories = Directory.GetDirectories(ModManager.GetModRootFolder());
		foreach (string directory in directories)
		{
			string modName = new DirectoryInfo(directory).Name;
			string modConfigPath = Path.Combine(directory, "Config.Lua").PathFix();
			string modSettingPath = Path.Combine(directory, "Settings.Lua").PathFix();
			bool flag = !File.Exists(modConfigPath);
			if (!flag)
			{
				ModInfoWithDisplayData modInfo = ModManager.ReadModInfo(modConfigPath, modSettingPath, true, false);
				bool flag2 = modInfo == null;
				if (!flag2)
				{
					bool flag3 = modInfo.ModId.Source > 0;
					if (flag3)
					{
						modInfo.ModId.Source = 0;
						ModManager.SaveModInfo(modInfo);
					}
					modIds.Add(modInfo.ModId);
					ModManager._localMods[modInfo.ModId.ToString()] = modInfo;
					ModManager._modNameCache[modInfo.ModId.ToString()] = modInfo.Title;
				}
			}
		}
		List<ModId> ordered = (from modId in modIds
		orderby modId.FileId
		select modId).ToList<ModId>();
		ModManager.ExternalMods.AddRange(ordered);
		Debug.Log(string.Format("Total {0} local mods found.", ModManager._localMods.Count));
	}

	// Token: 0x0600103C RID: 4156 RVA: 0x00060A34 File Offset: 0x0005EC34
	private static void UnloadAllMods()
	{
		object onLoadingModLock = ModManager.OnLoadingModLock;
		lock (onLoadingModLock)
		{
			bool flag2 = ModManager._loadedMods.Count == 0;
			if (!flag2)
			{
				for (int i = ModManager._loadedMods.Count - 1; i >= 0; i--)
				{
					ModManager.UnloadMod(i);
				}
				ModManager._loadedMods.Clear();
				ModManager._loadedPlugins.Clear();
			}
		}
	}

	// Token: 0x0600103D RID: 4157 RVA: 0x00060AC4 File Offset: 0x0005ECC4
	public static ulong VersionStringToUlong(string versionStr)
	{
		Version version;
		bool flag = !Version.TryParse(versionStr, out version);
		ulong result;
		if (flag)
		{
			result = 0UL;
		}
		else
		{
			ulong versionUlong = 0UL;
			versionUlong = BitOperation.SetSubUlong(versionUlong, 0, 16, (ulong)ModManager.ParseVersionNumber(version.Major));
			versionUlong = BitOperation.SetSubUlong(versionUlong, 16, 16, (ulong)ModManager.ParseVersionNumber(version.Minor));
			versionUlong = BitOperation.SetSubUlong(versionUlong, 32, 16, (ulong)ModManager.ParseVersionNumber(version.Build));
			versionUlong = BitOperation.SetSubUlong(versionUlong, 48, 16, (ulong)ModManager.ParseVersionNumber(version.Revision));
			result = versionUlong;
		}
		return result;
	}

	// Token: 0x0600103E RID: 4158 RVA: 0x00060B4C File Offset: 0x0005ED4C
	private static ushort ParseVersionNumber(int versionNumber)
	{
		bool flag = versionNumber < 0 || versionNumber > 65535;
		ushort result;
		if (flag)
		{
			result = 0;
		}
		else
		{
			result = (ushort)versionNumber;
		}
		return result;
	}

	// Token: 0x0600103F RID: 4159 RVA: 0x00060B78 File Offset: 0x0005ED78
	public static string VersionUlongToString(ulong versionUlong)
	{
		ulong major = BitOperation.GetSubUlong(versionUlong, 0, 16);
		ulong minor = BitOperation.GetSubUlong(versionUlong, 16, 16);
		ulong build = BitOperation.GetSubUlong(versionUlong, 32, 16);
		ulong revision = BitOperation.GetSubUlong(versionUlong, 48, 16);
		Version version = new Version((int)((ushort)major), (int)((ushort)minor), (int)((ushort)build), (int)((ushort)revision));
		return version.ToString();
	}

	// Token: 0x06001040 RID: 4160 RVA: 0x00060BD0 File Offset: 0x0005EDD0
	public static int GetModOrder(ModId modId)
	{
		return ModManager._modOrder.GetValueOrDefault(modId.ToString(), 0);
	}

	// Token: 0x06001041 RID: 4161 RVA: 0x00060BFA File Offset: 0x0005EDFA
	public static void SetModOrder(ModId modId, int value)
	{
		ModManager._modOrder[modId.ToString()] = value;
	}

	// Token: 0x06001042 RID: 4162 RVA: 0x00060C18 File Offset: 0x0005EE18
	public static ModInfoWithDisplayData ReadModInfo(string configPath, string modSettingsPath, bool loadOnRead = true, bool initModId = false)
	{
		Table table = null;
		bool flag = !File.Exists(configPath);
		ModInfoWithDisplayData result;
		if (flag)
		{
			result = null;
		}
		else
		{
			string fileText = File.ReadAllText(configPath);
			try
			{
				table = LuaGame.Instance.ReadMoonSharpTable(fileText);
				bool flag2 = table == null;
				if (flag2)
				{
					return null;
				}
			}
			catch (Exception e)
			{
				Debug.LogWarning("Invalid LuaTable: \n" + fileText);
				PredefinedLog.Show(2, configPath, e.Message);
				return null;
			}
			ModInfoWithDisplayData modInfo = null;
			try
			{
				modInfo = ModManager.ReadModInfoFromTable(table, configPath, loadOnRead, initModId);
			}
			catch (Exception e2)
			{
				Debug.LogWarning("LuaTable with unrecognized info: \n" + fileText);
				bool flag3 = table.ContainsKey("Title");
				if (flag3)
				{
					PredefinedLog.Show(1, table["Title"], configPath, e2.Message);
				}
				else
				{
					PredefinedLog.Show(2, configPath, e2.Message);
				}
				return null;
			}
			bool flag4 = loadOnRead && File.Exists(modSettingsPath);
			if (flag4)
			{
				string text = File.ReadAllText(modSettingsPath);
				try
				{
					Table settings = LuaGame.Instance.ReadMoonSharpTable(text);
					bool flag5 = settings != null;
					if (flag5)
					{
						modInfo.LoadSettingsFromLuaTable(settings);
					}
				}
				catch (Exception e3)
				{
					Debug.LogWarning("Invalid LuaTable: \n" + text);
					PredefinedLog.Show(3, modInfo.Title, modSettingsPath, e3.Message);
				}
			}
			if (loadOnRead)
			{
				modInfo.ApplySettings();
			}
			result = modInfo;
		}
		return result;
	}

	// Token: 0x06001043 RID: 4163 RVA: 0x00060DA8 File Offset: 0x0005EFA8
	private static ModInfoWithDisplayData ReadModInfoFromTable(Table table, string configPath, bool loadOnRead = true, bool initModId = false)
	{
		ModInfoWithDisplayData modInfo = new ModInfoWithDisplayData();
		modInfo.DirectoryName = Path.GetDirectoryName(configPath);
		modInfo.SourceLuaTable = table;
		string modName = Path.GetFileName(modInfo.DirectoryName);
		table.Load("Title", out modInfo.Title);
		bool flag = table.ContainsKey("Source");
		if (flag)
		{
			table.Load("Source", out modInfo.ModId.Source);
		}
		if (initModId)
		{
			modInfo.ModId = ModManager.CreateTempModId(modName, true);
		}
		else
		{
			bool flag2 = table.ContainsKey("FileId");
			if (flag2)
			{
				table.Load("FileId", out modInfo.ModId.FileId);
				bool flag3 = modInfo.ModId.Source == 0;
				if (flag3)
				{
					bool duplicated = false;
					foreach (KeyValuePair<string, ulong> keyValuePair in ModManager._modNameToTempFileId)
					{
						string text;
						ulong num;
						keyValuePair.Deconstruct(out text, out num);
						string name = text;
						ulong id = num;
						bool flag4 = name != modName && id == modInfo.ModId.FileId;
						if (flag4)
						{
							duplicated = true;
							break;
						}
					}
					bool flag5 = modInfo.ModId.FileId == 0UL || duplicated;
					if (flag5)
					{
						modInfo.ModId = ModManager.CreateTempModId(modName, true);
					}
					else
					{
						ulong tempFileId;
						bool flag6 = ModManager._modNameToTempFileId.TryGetValue(modName, out tempFileId) && modInfo.ModId.FileId != tempFileId;
						if (flag6)
						{
							modInfo.ModId.FileId = tempFileId;
						}
					}
					ModManager._modNameToTempFileId[modName] = modInfo.ModId.FileId;
				}
			}
			else if (loadOnRead)
			{
				modInfo.ModId = ModManager.CreateTempModId(modName, false);
			}
		}
		bool flag7 = table.ContainsKey("Version");
		if (flag7)
		{
			DynValue versionDynVal = table.Get("Version");
			bool flag8 = versionDynVal.String != null;
			if (flag8)
			{
				modInfo.ModId.Version = ModManager.VersionStringToUlong(versionDynVal.String);
			}
			else
			{
				modInfo.ModId.Version = versionDynVal.ToObject<ulong>();
			}
		}
		else
		{
			modInfo.ModId.Version = 0UL;
		}
		bool flag9 = table.ContainsKey("GameVersion");
		if (flag9)
		{
			table.Load("GameVersion", out modInfo.GameVersionStr);
			modInfo.GameVersion = ModManager.ParseGameVersion(modInfo.GameVersionStr);
		}
		List<string> backendPluginsLegacyPathList;
		table.Load("BackendPluginsLegacy", out backendPluginsLegacyPathList);
		modInfo.BackendPluginsLegacy = ModManager.GetLegacyPluginsNameList(backendPluginsLegacyPathList);
		bool flag10 = modInfo.BackendPluginsLegacy.Count > 0 && ModManager.IsModUseLegacy(modInfo);
		if (flag10)
		{
			modInfo.BackendPlugins = backendPluginsLegacyPathList;
		}
		else
		{
			table.Load("BackendPlugins", out modInfo.BackendPlugins);
		}
		table.Load("BackendPatches", out modInfo.BackendPatches);
		List<string> frontendPluginsLegacyPathList;
		table.Load("FrontendPluginsLegacy", out frontendPluginsLegacyPathList);
		modInfo.FrontendPluginsLegacy = ModManager.GetLegacyPluginsNameList(frontendPluginsLegacyPathList);
		bool flag11 = modInfo.FrontendPluginsLegacy.Count > 0 && ModManager.IsModUseLegacy(modInfo);
		if (flag11)
		{
			modInfo.FrontendPlugins = frontendPluginsLegacyPathList;
		}
		else
		{
			table.Load("FrontendPlugins", out modInfo.FrontendPlugins);
		}
		table.Load("FrontendPatches", out modInfo.FrontendPatches);
		table.Load("EventPackages", out modInfo.EventPackages);
		table.Load("Author", out modInfo.Author);
		table.Load("Description", out modInfo.Description);
		bool flag12 = table.ContainsKey("Cover");
		if (flag12)
		{
			table.Load("Cover", out modInfo.Cover);
			if (loadOnRead)
			{
				string path = Path.Combine(modInfo.DirectoryName, modInfo.Cover);
				string key = modInfo.ModId.ToString();
				ModManager._modCoverTextureGroup.RemoveTexture(key);
				ModManager._modCoverTextureGroup.AddTexture(key, path);
			}
		}
		bool flag13 = table.ContainsKey("DetailImageList");
		if (flag13)
		{
			table.Load("DetailImageList", out modInfo.DetailImageList);
			if (loadOnRead)
			{
				foreach (string image in modInfo.DetailImageList)
				{
					string path2 = Path.Combine(modInfo.DirectoryName, image);
					string key2 = modInfo.ModId.ToString() + image;
					ModManager._modCoverTextureGroup.RemoveTexture(key2);
					ModManager._modCoverTextureGroup.AddTexture(key2, path2);
				}
			}
		}
		bool flag14 = table.ContainsKey("WorkshopCover");
		if (flag14)
		{
			table.Load("WorkshopCover", out modInfo.WorkshopCover);
		}
		bool flag15 = table.ContainsKey("TagList");
		if (flag15)
		{
			table.Load("TagList", out modInfo.TagList);
		}
		bool flag16 = table.ContainsKey("Dependencies");
		if (flag16)
		{
			table.Load("Dependencies", out modInfo.Dependencies);
		}
		bool flag17 = table.ContainsKey("DefaultSettings") && loadOnRead;
		if (flag17)
		{
			Table settingsTable;
			table.Load("DefaultSettings", out settingsTable);
			for (int i = 1; i <= settingsTable.Length; i++)
			{
				Table settingEntryTable;
				settingsTable.Load(i, out settingEntryTable);
				string settingType;
				settingEntryTable.Load("SettingType", out settingType);
				if (!true)
				{
				}
				SettingEntry settingEntry;
				if (!(settingType == "Toggle"))
				{
					if (!(settingType == "ToggleGroup"))
					{
						if (!(settingType == "InputField"))
						{
							if (!(settingType == "Slider"))
							{
								if (!(settingType == "Dropdown"))
								{
									settingEntry = null;
								}
								else
								{
									settingEntry = new DropdownSetting();
								}
							}
							else
							{
								settingEntry = new SliderSetting();
							}
						}
						else
						{
							settingEntry = new InputFieldSetting();
						}
					}
					else
					{
						settingEntry = new ToggleGroupSetting();
					}
				}
				else
				{
					settingEntry = new ToggleSetting();
				}
				if (!true)
				{
				}
				SettingEntry setting = settingEntry;
				bool flag18 = setting == null;
				if (flag18)
				{
					throw new InvalidOperationException("Invalid Mod Setting Type");
				}
				setting.LoadDefaultSetting(settingEntryTable);
				modInfo.ModSettingEntries.Add(setting);
			}
		}
		bool flag19 = table.ContainsKey("SettingGroups");
		if (flag19)
		{
			table.Load("SettingGroups", out modInfo.ModSettingGroups);
		}
		bool flag20 = table.ContainsKey("Visibility");
		if (flag20)
		{
			table.Load("Visibility", out modInfo.Visibility);
		}
		bool flag21 = table.ContainsKey("UpdateLogList");
		if (flag21)
		{
			Table updateLogListTable;
			table.Load("UpdateLogList", out updateLogListTable);
			modInfo.LoadUpdateLog(updateLogListTable);
		}
		table.Load("ChangeConfig", out modInfo.ChangeConfig);
		table.Load("HasArchive", out modInfo.HasArchive);
		table.Load("NeedRestartWhenSettingChanged", out modInfo.NeedRestartWhenSettingChanged);
		return modInfo;
	}

	// Token: 0x06001044 RID: 4164 RVA: 0x00061488 File Offset: 0x0005F688
	public static void WriteModInfo(ModInfoWithDisplayData modInfo, string configPath)
	{
		if (modInfo.SourceLuaTable == null)
		{
			modInfo.SourceLuaTable = new Table(null);
		}
		modInfo.SourceLuaTable.Save("Title", modInfo.Title);
		modInfo.SourceLuaTable.Save("Source", modInfo.ModId.Source);
		modInfo.SourceLuaTable.Save("FileId", modInfo.ModId.FileId);
		modInfo.SourceLuaTable.Save("Version", ModManager.VersionUlongToString(modInfo.ModId.Version));
		modInfo.SourceLuaTable.Save("GameVersion", modInfo.GameVersionStr);
		modInfo.SourceLuaTable.Save("BackendPlugins", modInfo.BackendPlugins);
		modInfo.SourceLuaTable.Save("BackendPatches", modInfo.BackendPatches);
		List<string> backendPluginsLegacy = ModManager.GetLegacyPluginsPathList(modInfo.BackendPluginsLegacy);
		modInfo.SourceLuaTable.Save("BackendPluginsLegacy", backendPluginsLegacy);
		modInfo.SourceLuaTable.Save("FrontendPlugins", modInfo.FrontendPlugins);
		modInfo.SourceLuaTable.Save("FrontendPatches", modInfo.FrontendPatches);
		List<string> frontendPluginsLegacy = ModManager.GetLegacyPluginsPathList(modInfo.FrontendPluginsLegacy);
		modInfo.SourceLuaTable.Save("FrontendPluginsLegacy", frontendPluginsLegacy);
		modInfo.SourceLuaTable.Save("EventPackages", modInfo.EventPackages);
		modInfo.SourceLuaTable.Save("Author", modInfo.Author);
		modInfo.SourceLuaTable.Save("Description", modInfo.Description);
		modInfo.SourceLuaTable.Save("Cover", modInfo.Cover);
		modInfo.SourceLuaTable.Save("WorkshopCover", modInfo.WorkshopCover);
		modInfo.SourceLuaTable.Save("DetailImageList", modInfo.DetailImageList);
		modInfo.SourceLuaTable.Save("TagList", modInfo.TagList);
		modInfo.SourceLuaTable.Save("Dependencies", modInfo.Dependencies);
		modInfo.SourceLuaTable.Save("Visibility", modInfo.Visibility);
		Table defaultSettingsTable = new Table(null);
		modInfo.SaveDefaultSettings(defaultSettingsTable);
		modInfo.SourceLuaTable.Save("DefaultSettings", defaultSettingsTable);
		modInfo.SourceLuaTable.Save("SettingGroups", modInfo.ModSettingGroups);
		Table updateLogListTable = new Table(null);
		modInfo.SaveUpdateLog(updateLogListTable);
		modInfo.SourceLuaTable.Save("UpdateLogList", updateLogListTable);
		modInfo.SourceLuaTable.Save("ChangeConfig", modInfo.ChangeConfig);
		modInfo.SourceLuaTable.Save("HasArchive", modInfo.HasArchive);
		modInfo.SourceLuaTable.Save("NeedRestartWhenSettingChanged", modInfo.NeedRestartWhenSettingChanged);
		string str = modInfo.SourceLuaTable.Serialize(true, 0);
		StreamWriter writer = File.CreateText(configPath);
		writer.Write(str);
		writer.Close();
	}

	// Token: 0x06001045 RID: 4165 RVA: 0x00061764 File Offset: 0x0005F964
	private static Version ParseGameVersion(string gameVersion)
	{
		bool flag = string.IsNullOrEmpty(gameVersion);
		Version result;
		if (flag)
		{
			result = null;
		}
		else
		{
			bool flag2 = gameVersion[0] == 'V';
			if (flag2)
			{
				gameVersion = gameVersion.Substring(1);
			}
			Version version;
			bool flag3 = Version.TryParse(gameVersion, out version);
			if (flag3)
			{
				result = version;
			}
			else
			{
				int versionLength = gameVersion.IndexOf('-');
				bool flag4 = versionLength < 0;
				if (flag4)
				{
					result = null;
				}
				else
				{
					result = (Version.TryParse(gameVersion.Substring(0, versionLength), out version) ? version : null);
				}
			}
		}
		return result;
	}

	// Token: 0x06001046 RID: 4166 RVA: 0x000617DC File Offset: 0x0005F9DC
	public static void SaveModInfo(ModInfoWithDisplayData modInfo)
	{
		string configPath = Path.Combine(modInfo.DirectoryName, "Config.Lua");
		ModManager.WriteModInfo(modInfo, configPath);
		string modName = Path.GetFileName(modInfo.DirectoryName);
		bool flag = ModManager._modNameToTempFileId.ContainsKey(modName);
		if (flag)
		{
			ModManager._modNameToTempFileId[modName] = modInfo.ModId.FileId;
		}
	}

	// Token: 0x06001047 RID: 4167 RVA: 0x00061837 File Offset: 0x0005FA37
	private static void ReadModSettings()
	{
	}

	// Token: 0x06001048 RID: 4168 RVA: 0x0006183C File Offset: 0x0005FA3C
	private static void LoadMod(ModInfoWithDisplayData modInfo)
	{
		Debug.Log("Start loading mod " + modInfo.Title + " for frontend ...");
		bool flag = ModManager._loadedPlugins.ContainsKey(modInfo.ModId);
		if (flag)
		{
			throw new Exception(string.Format("Mod with FileId {0} is already loaded.", modInfo.ModId.FileId));
		}
		modInfo.ApplySettings();
		List<TaiwuRemakePlugin> plugins = new List<TaiwuRemakePlugin>();
		ModManager._loadedPlugins.Add(modInfo.ModId, plugins);
		ModManager._modConfigDataManager.LoadModConfig(modInfo);
		string modTextureGroupKey = ModManager.GetTextureGroupKey(modInfo.ModId);
		string texturesPath = Path.Combine(modInfo.DirectoryName, "ModResources", "Textures");
		SingletonObject.getInstance<TextureCenter>().LoadTextureGroupFromPath<PathKeyTextureGroup>(modTextureGroupKey, texturesPath);
		string graphicTexturePath = Path.Combine(modInfo.DirectoryName, "ModResources", "Graphics");
		string graphicsTextureGroupKey = ModManager.GetGraphicsTextureGroupKey(modInfo.ModId);
		SingletonObject.getInstance<TextureCenter>().LoadTextureGroupFromPath<NameKeyTextureGroup>(graphicsTextureGroupKey, graphicTexturePath);
		foreach (string pluginName in modInfo.FrontendPlugins)
		{
			string pluginDirPath = Path.Combine(modInfo.DirectoryName, "Plugins");
			Debug.Log(" - Loading plugin from " + pluginName);
			TaiwuRemakePlugin pluginInstance = PluginHelper.LoadPlugin(pluginDirPath, pluginName, modInfo.ModId.ToString());
			pluginInstance.OnModSettingUpdate();
			plugins.Add(pluginInstance);
		}
		ModManager._loadedMods.Add(modInfo.ModId);
	}

	// Token: 0x06001049 RID: 4169 RVA: 0x000619D0 File Offset: 0x0005FBD0
	private static void UnloadMod(ModId modId)
	{
		ModManager.UnloadMod(ModManager._loadedMods.IndexOf(modId));
	}

	// Token: 0x0600104A RID: 4170 RVA: 0x000619E4 File Offset: 0x0005FBE4
	private static void UnloadMod(int index)
	{
		ModId modId = ModManager._loadedMods[index];
		string fileIdStr = modId.ToString();
		bool flag = ModManager._localMods.ContainsKey(fileIdStr);
		if (flag)
		{
			ModInfoWithDisplayData modInfo = ModManager._localMods[fileIdStr];
			Debug.Log("Start unloading mod " + modInfo.Title + " for frontend ...");
		}
		else
		{
			Debug.Log("Start unloading mod " + modId.ToString() + " for frontend ...");
		}
		bool flag2 = !ModManager._loadedPlugins.ContainsKey(modId);
		if (flag2)
		{
			throw new Exception(string.Format("Mod with FileId {0} is not loaded.", modId));
		}
		List<TaiwuRemakePlugin> plugins = ModManager._loadedPlugins[modId];
		for (int i = plugins.Count - 1; i >= 0; i--)
		{
			Debug.Log(" - Unloading plugin " + plugins[i].PluginName);
			plugins[i].Dispose();
			plugins.RemoveAt(i);
		}
		plugins.Clear();
		ModManager._loadedPlugins.Remove(modId);
		ModManager._loadedMods.RemoveAt(index);
	}

	// Token: 0x0600104B RID: 4171 RVA: 0x00061B18 File Offset: 0x0005FD18
	public static void CheckModDiff(WorldInfo worldInfo, HashSet<string> removedMods, HashSet<string> newEnabledMods)
	{
		removedMods.Clear();
		newEnabledMods.Clear();
		bool flag = worldInfo.ModIds != null;
		if (flag)
		{
			removedMods.UnionWith(from modId in worldInfo.ModIds
			select modId.ToString());
		}
		newEnabledMods.UnionWith(from modId in ModManager._loadedMods
		select modId.ToString());
		removedMods.ExceptWith(from modId in ModManager._loadedMods
		select modId.ToString());
		bool flag2 = worldInfo.ModIds != null;
		if (flag2)
		{
			newEnabledMods.ExceptWith(from modId in worldInfo.ModIds
			select modId.ToString());
		}
	}

	// Token: 0x0600104C RID: 4172 RVA: 0x00061C10 File Offset: 0x0005FE10
	public static void SubscribeItem(ModId modId, bool startDownload = true)
	{
		PublishedFileId_t publishedFileId = new PublishedFileId_t(modId.FileId);
		SteamManager.SubscribeItem(publishedFileId);
		ModInfoWithDisplayData modInfo = ModManager.GetModInfo(modId);
		bool flag = modInfo != null;
		if (flag)
		{
			modInfo.IsSubscribed = true;
		}
		if (startDownload)
		{
			SteamUGC.DownloadItem(publishedFileId, true);
		}
	}

	// Token: 0x0600104D RID: 4173 RVA: 0x00061C58 File Offset: 0x0005FE58
	public static void UnSubscribeItem(ModId modId)
	{
		ModInfoWithDisplayData modInfo = ModManager.GetModInfo(modId);
		bool flag = modInfo != null;
		if (flag)
		{
			modInfo.IsSubscribed = false;
		}
		SteamManager.UnSubscribeItem(new PublishedFileId_t(modId.FileId));
	}

	// Token: 0x0600104E RID: 4174 RVA: 0x00061C90 File Offset: 0x0005FE90
	public static void DeleteUploadedMod(ModId modId)
	{
		ModInfoWithDisplayData modInfo = ModManager.GetModInfo(modId);
		ModId newModId = ModManager.CreateTempModId(Path.GetFileName(modInfo.DirectoryName), true);
		ModManager.UploadedMods.Remove(modId);
		SteamManager.DeleteItem(new PublishedFileId_t(modId.FileId));
		ModManager._localMods.Remove(modId.ToString());
		modInfo.ModId = newModId;
		ModManager._localMods.Add(modId.ToString(), modInfo);
		List<ModInfoWithDisplayData.UpdateLog> updateLogList = modInfo.UpdateLogList;
		if (updateLogList != null)
		{
			updateLogList.Clear();
		}
		ModManager.SaveModInfo(modInfo);
		ModManager.SaveModSettings(false);
	}

	// Token: 0x0600104F RID: 4175 RVA: 0x00061D2C File Offset: 0x0005FF2C
	public static void DeleteLocalMod(ModInfoWithDisplayData modInfo)
	{
		bool flag = ModManager.ExternalMods.Contains(modInfo.ModId);
		if (flag)
		{
			ModManager.ExternalMods.Remove(modInfo.ModId);
		}
		bool flag2 = ModManager._localMods.ContainsKey(modInfo.ModId.ToString());
		if (flag2)
		{
			ModManager._localMods.Remove(modInfo.ModId.ToString());
			bool flag3 = Directory.Exists(modInfo.DirectoryName);
			if (flag3)
			{
				Directory.Delete(modInfo.DirectoryName, true);
			}
		}
		bool flag4 = !modInfo.Title.IsNullOrEmpty() && ModManager._modNameToTempFileId.ContainsKey(modInfo.Title);
		if (flag4)
		{
			ModManager._modNameToTempFileId.Remove(modInfo.Title);
		}
		ModManager.SaveModSettings(false);
	}

	// Token: 0x06001050 RID: 4176 RVA: 0x00061DF4 File Offset: 0x0005FFF4
	public static bool SyncCoverLocalMod(List<ModId> modIds)
	{
		bool result;
		try
		{
			using (List<ModId>.Enumerator enumerator = modIds.GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					ModId uploadedModId = enumerator.Current;
					ModInfoWithDisplayData uploadedModInfo = ModManager.GetModInfo(uploadedModId);
					string directoryName = uploadedModInfo.Title.RemoveColorTags();
					PublishedFileId_t fileId = new PublishedFileId_t(uploadedModId.FileId);
					ulong num;
					string folderPath;
					uint num2;
					SteamUGC.GetItemInstallInfo(fileId, out num, out folderPath, 400U, out num2);
					bool flag = string.IsNullOrEmpty(folderPath);
					if (!flag)
					{
						int index = ModManager.ExternalMods.FindIndex((ModId id) => id.FileId == uploadedModId.FileId);
						bool flag2 = index >= 0;
						if (flag2)
						{
							ModId localModId = ModManager.ExternalMods[index];
							ModInfoWithDisplayData localModInfo = ModManager.GetModInfo(localModId);
							string localModDirectoryName = localModInfo.DirectoryName.RemoveColorTags();
							bool flag3 = Directory.Exists(localModDirectoryName);
							if (flag3)
							{
								directoryName = Path.GetFileName(localModDirectoryName);
								bool flag4 = !ModManager.DeleteDirectory(localModDirectoryName, false);
								if (flag4)
								{
									string title = LocalStringManager.Get(LanguageKey.LK_Common_Attention);
									string content = LocalStringManager.Get(LanguageKey.LK_Mod_Warning_File_Occupied);
									CommonUtils.ShowDialog(title, content, null, EDialogType.None);
									return false;
								}
							}
						}
						bool flag5 = Directory.Exists(folderPath);
						if (flag5)
						{
							string sourceFolderPath = folderPath;
							string destinationFolderPath = Path.Combine(ModManager.GetModRootFolder(), directoryName);
							bool flag6 = !Directory.Exists(destinationFolderPath);
							if (flag6)
							{
								Directory.CreateDirectory(destinationFolderPath);
							}
							string[] rootFiles = Directory.GetFiles(sourceFolderPath, "*.*", SearchOption.AllDirectories);
							foreach (string file in rootFiles)
							{
								string relativePath = file.Substring(sourceFolderPath.Length).TrimStart(Path.DirectorySeparatorChar);
								string destFile = Path.Combine(destinationFolderPath, relativePath);
								bool flag7 = !Directory.Exists(Path.GetDirectoryName(destFile));
								if (flag7)
								{
									Directory.CreateDirectory(Path.GetDirectoryName(destFile));
								}
								File.Copy(file, destFile, true);
							}
							string modConfigPath = Path.Combine(destinationFolderPath, "Config.Lua").PathFix();
							string modSettingPath = Path.Combine(destinationFolderPath, "Settings.Lua").PathFix();
							bool flag8 = !File.Exists(modConfigPath);
							if (!flag8)
							{
								ModInfoWithDisplayData modInfo = ModManager.ReadModInfo(modConfigPath, modSettingPath, true, false);
								modInfo.ModId.Source = 0;
								ModManager._localMods[modInfo.ModId.ToString()] = modInfo;
								ModManager.SaveModInfo(modInfo);
							}
						}
					}
				}
			}
			ModManager.SaveModSettings(false);
			result = true;
		}
		catch (Exception e)
		{
			GLog.TagError("ModManager", e.ToString(), Array.Empty<object>());
			throw;
		}
		return result;
	}

	// Token: 0x06001051 RID: 4177 RVA: 0x000620C8 File Offset: 0x000602C8
	private static int CompareMods(ModId a, ModId b)
	{
		return ModManager._modOrder.GetValueOrDefault(a.ToString(), 0).CompareTo(ModManager._modOrder.GetValueOrDefault(b.ToString(), 0));
	}

	// Token: 0x06001052 RID: 4178 RVA: 0x00062110 File Offset: 0x00060310
	private static List<string> GetLegacyPluginsPathList(List<string> plugins)
	{
		List<string> pathList = new List<string>(plugins.Count);
		foreach (string fileName in plugins)
		{
			pathList.Add("../LegacyPlugins/" + fileName);
		}
		return pathList;
	}

	// Token: 0x06001053 RID: 4179 RVA: 0x00062180 File Offset: 0x00060380
	private static List<string> GetLegacyPluginsNameList(List<string> pathList)
	{
		List<string> nameList = new List<string>(pathList.Count);
		foreach (string path in pathList)
		{
			nameList.Add(Path.GetFileName(path));
		}
		return nameList;
	}

	// Token: 0x06001054 RID: 4180 RVA: 0x000621E8 File Offset: 0x000603E8
	private static bool DeleteDirectory(string path, bool deleteRoot)
	{
		bool flag = !Directory.Exists(path);
		bool result;
		if (flag)
		{
			result = false;
		}
		else
		{
			string[] files = Directory.GetFiles(path);
			foreach (string file in files)
			{
				bool flag2 = File.Exists(file);
				if (!flag2)
				{
					return false;
				}
				File.Delete(file);
			}
			string[] directories = Directory.GetDirectories(path);
			foreach (string directory in directories)
			{
				bool flag3 = !ModManager.DeleteDirectory(directory, true);
				if (flag3)
				{
					return false;
				}
			}
			if (deleteRoot)
			{
				Directory.Delete(path);
			}
			result = true;
		}
		return result;
	}

	// Token: 0x06001055 RID: 4181 RVA: 0x0006229C File Offset: 0x0006049C
	public static bool GetSetting(string modIdStr, string settingName, ref int val)
	{
		ModInfoWithDisplayData modInfo;
		return ModManager._localMods.TryGetValue(modIdStr, out modInfo) && modInfo.ModSettings.Get(settingName, out val);
	}

	// Token: 0x06001056 RID: 4182 RVA: 0x000622D0 File Offset: 0x000604D0
	public static bool GetSetting(string modIdStr, string settingName, ref float val)
	{
		ModInfoWithDisplayData modInfo;
		return ModManager._localMods.TryGetValue(modIdStr, out modInfo) && modInfo.ModSettings.Get(settingName, out val);
	}

	// Token: 0x06001057 RID: 4183 RVA: 0x00062304 File Offset: 0x00060504
	public static bool GetSetting(string modIdStr, string settingName, ref bool val)
	{
		ModInfoWithDisplayData modInfo;
		return ModManager._localMods.TryGetValue(modIdStr, out modInfo) && modInfo.ModSettings.Get(settingName, out val);
	}

	// Token: 0x06001058 RID: 4184 RVA: 0x00062338 File Offset: 0x00060538
	public static bool GetSetting(string modIdStr, string settingName, ref string val)
	{
		ModInfoWithDisplayData modInfo;
		return ModManager._localMods.TryGetValue(modIdStr, out modInfo) && modInfo.ModSettings.Get(settingName, out val);
	}

	// Token: 0x06001059 RID: 4185 RVA: 0x0006236C File Offset: 0x0006056C
	public static string GetModRootFolder()
	{
		DirectoryInfo directoryInfo = new DirectoryInfo(Application.dataPath).Parent;
		return ModManager.CheckDirectory(Path.Combine(directoryInfo.FullName, "Mod"));
	}

	// Token: 0x0600105A RID: 4186 RVA: 0x000623A3 File Offset: 0x000605A3
	private static void InitPath()
	{
		ModManager._workingModName = PlayerPrefs.GetString("LastWorkingModName", "FirstMod");
	}

	// Token: 0x0600105B RID: 4187 RVA: 0x000623BA File Offset: 0x000605BA
	public static void SetWorkingModName(string newWorkingModName)
	{
		ModManager._workingModName = newWorkingModName;
		PlayerPrefs.SetString("LastWorkingModName", ModManager._workingModName);
	}

	// Token: 0x0600105C RID: 4188 RVA: 0x000623D4 File Offset: 0x000605D4
	private static string CheckDirectory(string path)
	{
		bool flag = !Directory.Exists(path);
		if (flag)
		{
			Directory.CreateDirectory(path);
		}
		return path.PathFix();
	}

	// Token: 0x0600105D RID: 4189 RVA: 0x00062400 File Offset: 0x00060600
	public static string GetModFactoryRootFolder()
	{
		DirectoryInfo directoryInfo = new DirectoryInfo(ModManager._applicationDataPath).Parent;
		return Path.Combine(directoryInfo.FullName, "ModFactory");
	}

	// Token: 0x0600105E RID: 4190 RVA: 0x00062434 File Offset: 0x00060634
	public static string GetWorkingModFolder()
	{
		return ModManager.CheckDirectory(Path.Combine(ModManager.GetModFactoryRootFolder(), ModManager._workingModName));
	}

	// Token: 0x0600105F RID: 4191 RVA: 0x0006245C File Offset: 0x0006065C
	public static string GetWorkingModName()
	{
		return ModManager._workingModName;
	}

	// Token: 0x06001060 RID: 4192 RVA: 0x00062474 File Offset: 0x00060674
	public static string GetModWorkSpaceRoot()
	{
		return ModManager.CheckDirectory(Path.Combine(ModManager.GetWorkingModFolder(), "WorkSpace"));
	}

	// Token: 0x06001061 RID: 4193 RVA: 0x0006249C File Offset: 0x0006069C
	public static string GetModEditingConfigFolder()
	{
		return ModManager.CheckDirectory(Path.Combine(ModManager.GetModWorkSpaceRoot(), "CustomConfig"));
	}

	// Token: 0x06001062 RID: 4194 RVA: 0x000624C4 File Offset: 0x000606C4
	public static string GetModEventEditorDataFolder()
	{
		return ModManager.CheckDirectory(Path.Combine(ModManager.GetModWorkSpaceRoot(), "EventEditorData"));
	}

	// Token: 0x06001063 RID: 4195 RVA: 0x000624EC File Offset: 0x000606EC
	public static string GetModEventEditorSimulateEnvFolder()
	{
		return ModManager.CheckDirectory(Path.Combine(ModManager.GetModEventEditorDataFolder(), "SimulateEnvironment"));
	}

	// Token: 0x06001064 RID: 4196 RVA: 0x00062514 File Offset: 0x00060714
	public static string GetModEventEditorConfigFolder()
	{
		return ModManager.CheckDirectory(Path.Combine(ModManager.GetModEventEditorDataFolder(), "EventEditorConfig"));
	}

	// Token: 0x06001065 RID: 4197 RVA: 0x0006253C File Offset: 0x0006073C
	public static string GetModEventSaveCore()
	{
		return ModManager.CheckDirectory(Path.Combine(ModManager.GetModEventEditorDataFolder(), "EventCore"));
	}

	// Token: 0x06001066 RID: 4198 RVA: 0x00062564 File Offset: 0x00060764
	public static string GetModEventGlobalScriptsFolder()
	{
		return ModManager.CheckDirectory(Path.Combine(ModManager.GetModEventEditorDataFolder(), "GlobalScripts"));
	}

	// Token: 0x06001067 RID: 4199 RVA: 0x0006258C File Offset: 0x0006078C
	public static string GetModEventTexturesFolder()
	{
		return ModManager.CheckDirectory(Path.Combine(ModManager.GetModEventEditorDataFolder(), "EventTextures"));
	}

	// Token: 0x06001068 RID: 4200 RVA: 0x000625B4 File Offset: 0x000607B4
	public static string GetModEventExportCacheFolder()
	{
		return ModManager.CheckDirectory(Path.Combine(ModManager.GetModEventEditorDataFolder(), "ExportCacheFiles"));
	}

	// Token: 0x06001069 RID: 4201 RVA: 0x000625DC File Offset: 0x000607DC
	public static string GetModEventExportCsFilesFolder()
	{
		return ModManager.CheckDirectory(Path.Combine(ModManager.GetModEventExportCacheFolder(), "ExportEvents"));
	}

	// Token: 0x0600106A RID: 4202 RVA: 0x00062604 File Offset: 0x00060804
	public static string GetModEventExportLanguageFilesFolder()
	{
		return ModManager.CheckDirectory(Path.Combine(ModManager.GetModEventExportCacheFolder(), "EventLanguages"));
	}

	// Token: 0x0600106B RID: 4203 RVA: 0x0006262C File Offset: 0x0006082C
	public static string GetModEventExportCompiledGlobalScripts()
	{
		return ModManager.CheckDirectory(Path.Combine(ModManager.GetPublishEventsDllFolder(), "GlobalScriptCompiled"));
	}

	// Token: 0x0600106C RID: 4204 RVA: 0x00062654 File Offset: 0x00060854
	public static string GetModEventCompileDllFolder()
	{
		return ModManager.CheckDirectory(Path.Combine(ModManager.GetModEventExportCacheFolder(), "EventLib"));
	}

	// Token: 0x0600106D RID: 4205 RVA: 0x0006267C File Offset: 0x0006087C
	public static string GetModEventCompileScriptFolder()
	{
		return ModManager.CheckDirectory(Path.Combine(ModManager.GetModEventExportCacheFolder(), "EventScript"));
	}

	// Token: 0x0600106E RID: 4206 RVA: 0x000626A4 File Offset: 0x000608A4
	public static string GetModAdventureDataFolder()
	{
		return ModManager.CheckDirectory(Path.Combine(ModManager.GetModWorkSpaceRoot(), "AdventureData"));
	}

	// Token: 0x0600106F RID: 4207 RVA: 0x000626CC File Offset: 0x000608CC
	public static string GetPublishRootPath()
	{
		return ModManager.CheckDirectory(Path.Combine(ModManager.GetWorkingModFolder(), "Publish"));
	}

	// Token: 0x06001070 RID: 4208 RVA: 0x000626F4 File Offset: 0x000608F4
	public static string GetPublishAvatarAsset()
	{
		return ModManager.CheckDirectory(Path.Combine(ModManager.GetPublishRootPath(), "Avatar", "AvatarAssets"));
	}

	// Token: 0x06001071 RID: 4209 RVA: 0x00062720 File Offset: 0x00060920
	public static string GetPublishAvatarPreset()
	{
		return ModManager.CheckDirectory(Path.Combine(ModManager.GetPublishRootPath(), "Avatar", "AvatarPreset"));
	}

	// Token: 0x06001072 RID: 4210 RVA: 0x0006274C File Offset: 0x0006094C
	public static string GetPublishEventsRoot()
	{
		return ModManager.CheckDirectory(Path.Combine(ModManager.GetPublishRootPath(), "Events"));
	}

	// Token: 0x06001073 RID: 4211 RVA: 0x00062774 File Offset: 0x00060974
	public static string GetPublishEventsDllFolder()
	{
		return ModManager.CheckDirectory(Path.Combine(ModManager.GetPublishEventsRoot(), "EventLib"));
	}

	// Token: 0x06001074 RID: 4212 RVA: 0x0006279C File Offset: 0x0006099C
	public static string GetPublishEventsLanguageFolder()
	{
		return ModManager.CheckDirectory(Path.Combine(ModManager.GetPublishEventsRoot(), "EventLanguages"));
	}

	// Token: 0x06001075 RID: 4213 RVA: 0x000627C4 File Offset: 0x000609C4
	public static string GetPublishEventScriptFolder()
	{
		return ModManager.CheckDirectory(Path.Combine(ModManager.GetPublishEventsRoot(), "EventScript"));
	}

	// Token: 0x06001076 RID: 4214 RVA: 0x000627EC File Offset: 0x000609EC
	public static string GetPublishEventsTextureFolder()
	{
		return ModManager.CheckDirectory(Path.Combine(ModManager.GetPublishEventsRoot(), "EventTextures"));
	}

	// Token: 0x06001077 RID: 4215 RVA: 0x00062814 File Offset: 0x00060A14
	public static string GetPublishConfigFolder()
	{
		return ModManager.CheckDirectory(Path.Combine(ModManager.GetPublishRootPath(), "Config"));
	}

	// Token: 0x06001078 RID: 4216 RVA: 0x0006283C File Offset: 0x00060A3C
	internal static void InvokeModDisplayEvent(string modIdStr, string customData)
	{
		List<Action<string>> handlers;
		bool flag = !ModManager._modDisplayEventHandlers.TryGetValue(modIdStr, out handlers);
		if (!flag)
		{
			foreach (Action<string> handler in handlers)
			{
				handler(customData);
			}
		}
	}

	// Token: 0x06001079 RID: 4217 RVA: 0x000628A4 File Offset: 0x00060AA4
	public static void RegisterModDisplayEventHandler(string modIdStr, Action<string> handler)
	{
		ModManager._modDisplayEventHandlers.GetOrNew(modIdStr).Add(handler);
	}

	// Token: 0x0600107A RID: 4218 RVA: 0x000628BC File Offset: 0x00060ABC
	public static bool UnRegisterModDisplayEventHandler(string modIdStr)
	{
		return ModManager._modDisplayEventHandlers.Remove(modIdStr);
	}

	// Token: 0x0600107B RID: 4219 RVA: 0x000628DC File Offset: 0x00060ADC
	public static bool UnRegisterModDisplayEventHandler(string modIdStr, Action<string> handler)
	{
		List<Action<string>> handlers;
		return ModManager._modDisplayEventHandlers.TryGetValue(modIdStr, out handlers) && handlers.Remove(handler);
	}

	// Token: 0x0600107C RID: 4220 RVA: 0x00062908 File Offset: 0x00060B08
	public static string GetOverwriteTexturePath(string path)
	{
		foreach (ModId modId in ModManager._loadedMods)
		{
			ModInfoWithDisplayData modInfo = ModManager.GetModInfo(modId);
			bool flag = modInfo == null;
			if (!flag)
			{
				string pngFullPath = Path.Combine(modInfo.DirectoryName, "Textures", path + ".png");
				bool flag2 = File.Exists(pngFullPath);
				if (flag2)
				{
					return pngFullPath;
				}
				string jpgFullPath = Path.Combine(modInfo.DirectoryName, "Textures", path + ".jpg");
				bool flag3 = File.Exists(jpgFullPath);
				if (flag3)
				{
					return jpgFullPath;
				}
			}
		}
		return null;
	}

	// Token: 0x0600107D RID: 4221 RVA: 0x000629D4 File Offset: 0x00060BD4
	public static Texture2D GetOverwriteTexture(string path)
	{
		bool flag = ModManager._loadedMods == null;
		Texture2D result;
		if (flag)
		{
			result = null;
		}
		else
		{
			foreach (ModId modId in ModManager._loadedMods)
			{
				ModInfoWithDisplayData modInfo = ModManager.GetModInfo(modId);
				bool flag2 = modInfo == null;
				if (!flag2)
				{
					string modTextureGroupKey = ModManager.GetTextureGroupKey(modInfo.ModId);
					BaseTextureGroup textureGroup;
					bool flag3 = !SingletonObject.getInstance<TextureCenter>().TryGetTextureGroup(modTextureGroupKey, out textureGroup);
					if (!flag3)
					{
						Texture2D texture = textureGroup.GetTexture(path);
						bool flag4 = texture != null;
						if (flag4)
						{
							return texture;
						}
					}
				}
			}
			result = null;
		}
		return result;
	}

	// Token: 0x0600107E RID: 4222 RVA: 0x00062A98 File Offset: 0x00060C98
	public static Sprite GetOverwriteSprite(string path, Sprite referenceSprite)
	{
		Texture2D texture = ModManager.GetOverwriteTexture(path);
		bool flag = !texture;
		Sprite result;
		if (flag)
		{
			result = null;
		}
		else
		{
			result = SingletonObject.getInstance<TextureCenter>().GetOrCreateSprite(texture, referenceSprite);
		}
		return result;
	}

	// Token: 0x0600107F RID: 4223 RVA: 0x00062AD0 File Offset: 0x00060CD0
	public static Texture2D GetOverwriteGraphicsTexture(string name)
	{
		bool flag = ModManager._loadedMods == null;
		Texture2D result;
		if (flag)
		{
			result = null;
		}
		else
		{
			foreach (ModId modId in ModManager._loadedMods)
			{
				ModInfoWithDisplayData modInfo = ModManager.GetModInfo(modId);
				bool flag2 = modInfo == null;
				if (!flag2)
				{
					string modTextureGroupKey = ModManager.GetGraphicsTextureGroupKey(modInfo.ModId);
					BaseTextureGroup textureGroup;
					bool flag3 = !SingletonObject.getInstance<TextureCenter>().TryGetTextureGroup(modTextureGroupKey, out textureGroup);
					if (!flag3)
					{
						Texture2D texture = textureGroup.GetTexture(name);
						bool flag4 = texture;
						if (flag4)
						{
							return texture;
						}
					}
				}
			}
			result = null;
		}
		return result;
	}

	// Token: 0x06001080 RID: 4224 RVA: 0x00062B90 File Offset: 0x00060D90
	public static Sprite GetOverwriteGraphicsSprite(string name, Sprite referenceSprite = null)
	{
		Texture2D texture = ModManager.GetOverwriteGraphicsTexture(name);
		bool flag = !texture;
		Sprite result;
		if (flag)
		{
			result = null;
		}
		else
		{
			result = SingletonObject.getInstance<TextureCenter>().GetOrCreateSprite(texture, referenceSprite);
		}
		return result;
	}

	// Token: 0x06001082 RID: 4226 RVA: 0x00062C54 File Offset: 0x00060E54
	[CompilerGenerated]
	internal static IEnumerator <UpdateModList>g__Co|39_0(Action callback)
	{
		IEnumerator item = ModManager.UpdateModListImpl();
		while (item.MoveNext())
		{
			yield return item;
		}
		if (callback != null)
		{
			callback();
		}
		yield break;
	}

	// Token: 0x06001083 RID: 4227 RVA: 0x00062C74 File Offset: 0x00060E74
	[CompilerGenerated]
	internal static void <LoadAllEnabledMods>g__HandleDependencyOrder|82_0(ModId modId, ref ModManager.<>c__DisplayClass82_0 A_1)
	{
		ModInfoWithDisplayData modInfo;
		bool flag = !ModManager._localMods.TryGetValue(modId.ToString(), out modInfo);
		if (!flag)
		{
			List<ulong> dependencies = modInfo.Dependencies;
			ulong fileId = modInfo.ModId.FileId;
			bool flag2 = dependencies != null;
			if (flag2)
			{
				using (List<ulong>.Enumerator enumerator = dependencies.GetEnumerator())
				{
					while (enumerator.MoveNext())
					{
						ulong id = enumerator.Current;
						int index = ModManager.EnabledMods.FindIndex((ModId d) => d.FileId == id && d.FileId != fileId);
						bool flag3 = index > -1;
						if (flag3)
						{
							ModId dependentModId = ModManager.EnabledMods[index];
							ModManager.<LoadAllEnabledMods>g__HandleDependencyOrder|82_0(dependentModId, ref A_1);
						}
					}
				}
			}
			bool flag4 = !A_1.orderQueue.Contains(modId);
			if (flag4)
			{
				A_1.orderQueue.Enqueue(modId);
			}
		}
	}

	// Token: 0x04000E99 RID: 3737
	public static readonly List<ModId> ExternalMods = new List<ModId>();

	// Token: 0x04000E9A RID: 3738
	public static readonly List<ModId> PlatformMods = new List<ModId>();

	// Token: 0x04000E9B RID: 3739
	public static readonly List<ModId> UploadedMods = new List<ModId>();

	// Token: 0x04000E9C RID: 3740
	public static readonly List<ModId> SubscribedMods = new List<ModId>();

	// Token: 0x04000E9D RID: 3741
	public static List<ModId> EnabledMods;

	// Token: 0x04000E9E RID: 3742
	private static Dictionary<string, ModInfoWithDisplayData> _localMods;

	// Token: 0x04000E9F RID: 3743
	private static List<ModId> _loadedMods;

	// Token: 0x04000EA0 RID: 3744
	private static Dictionary<ModId, List<TaiwuRemakePlugin>> _loadedPlugins;

	// Token: 0x04000EA1 RID: 3745
	private static Dictionary<string, List<Action<string>>> _modDisplayEventHandlers = new Dictionary<string, List<Action<string>>>();

	// Token: 0x04000EA2 RID: 3746
	private static Dictionary<string, ulong> _modNameToTempFileId;

	// Token: 0x04000EA3 RID: 3747
	private static ModTextureGroup _modCoverTextureGroup;

	// Token: 0x04000EA4 RID: 3748
	private static Dictionary<string, object>[] _configItemsToBeLoaded;

	// Token: 0x04000EA5 RID: 3749
	private static HashSet<string> _whitelistMods;

	// Token: 0x04000EA6 RID: 3750
	private static Dictionary<string, int> _modOrder = new Dictionary<string, int>();

	// Token: 0x04000EA7 RID: 3751
	private static readonly Version CutVersion = new Version(0, 0, 79);

	// Token: 0x04000EA8 RID: 3752
	private static Dictionary<string, string> _modNameCache;

	// Token: 0x04000EA9 RID: 3753
	private static readonly ModConfigDataManager _modConfigDataManager = new ModConfigDataManager();

	// Token: 0x04000EAA RID: 3754
	public static readonly object OnLoadingModLock = new object();

	// Token: 0x04000EAB RID: 3755
	private const string ModSystemSettingsFile = "ModSettings.Lua";

	// Token: 0x04000EAC RID: 3756
	public const string ModConfigFile = "Config.Lua";

	// Token: 0x04000EAD RID: 3757
	public const string ModSettingsFile = "Settings.Lua";

	// Token: 0x04000EAE RID: 3758
	private const string ModTexturePrefix = "ModTexture";

	// Token: 0x04000EAF RID: 3759
	private const string ModGraphicsTexturePrefix = "ModGraphicsTexture";

	// Token: 0x04000EB0 RID: 3760
	public const string PluginsDirectoryName = "Plugins";

	// Token: 0x04000EB1 RID: 3761
	public const string LegacyPluginsDirectoryName = "LegacyPlugins";

	// Token: 0x04000EB2 RID: 3762
	public const string ConfigDirectoryName = "Config";

	// Token: 0x04000EB3 RID: 3763
	private static int _next = 0;

	// Token: 0x04000EB4 RID: 3764
	private static bool _hasError = false;

	// Token: 0x04000EB9 RID: 3769
	private const string ModRootFolder = "Mod";

	// Token: 0x04000EBA RID: 3770
	private static string _applicationDataPath = Application.dataPath;

	// Token: 0x04000EBB RID: 3771
	private const string ModFactoryRootFolder = "ModFactory";

	// Token: 0x04000EBC RID: 3772
	private static string _workingModName = "ConchShip";

	// Token: 0x04000EBD RID: 3773
	public const string LastWorkingModeName = "LastWorkingModName";

	// Token: 0x04000EBE RID: 3774
	private const string AvatarFolder = "Avatar";

	// Token: 0x04000EBF RID: 3775
	private const string AvatarAsset = "AvatarAssets";

	// Token: 0x04000EC0 RID: 3776
	private const string AvatarPreset = "AvatarPreset";

	// Token: 0x04000EC1 RID: 3777
	private const string WorkSpaceFolder = "WorkSpace";

	// Token: 0x04000EC2 RID: 3778
	private const string PublishFolder = "Publish";

	// Token: 0x04000EC3 RID: 3779
	private const string EventEditorDataFolder = "EventEditorData";

	// Token: 0x04000EC4 RID: 3780
	private const string EventSimulateEnvFolder = "SimulateEnvironment";

	// Token: 0x04000EC5 RID: 3781
	private const string EventEditorConfigFolder = "EventEditorConfig";

	// Token: 0x04000EC6 RID: 3782
	private const string EventSaveCore = "EventCore";

	// Token: 0x04000EC7 RID: 3783
	private const string EventTextures = "EventTextures";

	// Token: 0x04000EC8 RID: 3784
	private const string EventExportCacheFolder = "ExportCacheFiles";

	// Token: 0x04000EC9 RID: 3785
	private const string EventExportCsFilesFolder = "ExportEvents";

	// Token: 0x04000ECA RID: 3786
	private const string EventExportLanguageFilesFolder = "EventLanguages";

	// Token: 0x04000ECB RID: 3787
	private const string EventExportGlobalScriptsCompiledFolder = "GlobalScriptCompiled";

	// Token: 0x04000ECC RID: 3788
	private const string EventCompileDllFilesFolder = "EventLib";

	// Token: 0x04000ECD RID: 3789
	private const string EventCompileScriptFilesFolder = "EventScript";

	// Token: 0x04000ECE RID: 3790
	private const string EventsCoreRuntimeRoot = "Events";

	// Token: 0x04000ECF RID: 3791
	private const string EventGlobalScriptsFolder = "GlobalScripts";

	// Token: 0x04000ED0 RID: 3792
	private const string EditingConfig = "CustomConfig";

	// Token: 0x04000ED1 RID: 3793
	private const string PublishConfig = "Config";

	// Token: 0x04000ED2 RID: 3794
	private const string AdventureDataFolder = "AdventureData";
}
