using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using AudioKit;
using Config;
using FrameWork.AssetBundlePackage;
using GameData.DLC;
using GameData.Utilities;
using Steamworks;
using UnityEngine;
using UnityEngine.U2D;

// Token: 0x02000015 RID: 21
public class DlcManager : ISingletonInit, IDisposable
{
	// Token: 0x1700001D RID: 29
	// (get) Token: 0x06000089 RID: 137 RVA: 0x00003CC2 File Offset: 0x00001EC2
	public static uint DlcIdInteractOfLove
	{
		get
		{
			return 2305890U;
		}
	}

	// Token: 0x1700001E RID: 30
	// (get) Token: 0x0600008A RID: 138 RVA: 0x00003CC9 File Offset: 0x00001EC9
	public static uint DlcIdGiftFromConchShip1
	{
		get
		{
			return 2241120U;
		}
	}

	// Token: 0x1700001F RID: 31
	// (get) Token: 0x0600008B RID: 139 RVA: 0x00003CD0 File Offset: 0x00001ED0
	public static uint DlcIdGiftFromConchShip2
	{
		get
		{
			return 2172690U;
		}
	}

	// Token: 0x17000020 RID: 32
	// (get) Token: 0x0600008C RID: 140 RVA: 0x00003CD7 File Offset: 0x00001ED7
	public static uint DlcIdFiveLoong
	{
		get
		{
			return 2764950U;
		}
	}

	// Token: 0x17000021 RID: 33
	// (get) Token: 0x0600008D RID: 141 RVA: 0x00003CDE File Offset: 0x00001EDE
	public static uint DlcIdHappyNewYear2024
	{
		get
		{
			return 2764960U;
		}
	}

	// Token: 0x17000022 RID: 34
	// (get) Token: 0x0600008E RID: 142 RVA: 0x00003CE5 File Offset: 0x00001EE5
	public static uint DlcIdYearOfSnakeCloth
	{
		get
		{
			return 3464590U;
		}
	}

	// Token: 0x17000023 RID: 35
	// (get) Token: 0x0600008F RID: 143 RVA: 0x00003CEC File Offset: 0x00001EEC
	public static uint DlcIdHappyNewYear2026
	{
		get
		{
			return 4395170U;
		}
	}

	// Token: 0x17000024 RID: 36
	// (get) Token: 0x06000090 RID: 144 RVA: 0x00003CF3 File Offset: 0x00001EF3
	public static uint DlcIdCricketPolymorph
	{
		get
		{
			return 4528730U;
		}
	}

	// Token: 0x17000025 RID: 37
	// (get) Token: 0x06000091 RID: 145 RVA: 0x00003CFA File Offset: 0x00001EFA
	public static uint DlcIdEightYears
	{
		get
		{
			return 4834440U;
		}
	}

	// Token: 0x17000026 RID: 38
	// (get) Token: 0x06000092 RID: 146 RVA: 0x00003D01 File Offset: 0x00001F01
	public static uint DlcIdGreenHillsRemain
	{
		get
		{
			return 4834450U;
		}
	}

	// Token: 0x06000093 RID: 147 RVA: 0x00003D08 File Offset: 0x00001F08
	public static string GetDlcLocalPath(string dlcName)
	{
		return "Assets/Dlc/" + dlcName;
	}

	// Token: 0x06000094 RID: 148 RVA: 0x00003D28 File Offset: 0x00001F28
	public static ImplementedDlcItem GetDlcConfigItem(uint dlcAppId)
	{
		foreach (ImplementedDlcItem dlcConfig in ((IEnumerable<ImplementedDlcItem>)ImplementedDlc.Instance))
		{
			bool flag = dlcConfig.AppId == dlcAppId;
			if (flag)
			{
				return dlcConfig;
			}
		}
		return null;
	}

	// Token: 0x06000095 RID: 149 RVA: 0x00003D8C File Offset: 0x00001F8C
	public void TryLoadDlc(uint dlcAppId)
	{
		bool flag = !SteamManager.IsDlcInstalled(dlcAppId);
		if (!flag)
		{
			DlcPackageInfo dlcPackageInfo = this.LoadDlcPackageInfo(dlcAppId);
			bool flag2 = dlcPackageInfo == null;
			if (!flag2)
			{
				if (this._dlcList == null)
				{
					this._dlcList = new List<DlcPackageInfo>();
				}
				bool flag3 = this._dlcList.Count > 0;
				if (flag3)
				{
					int count = this._dlcList.Count;
					for (int i = 0; i < count; i++)
					{
						bool flag4 = this._dlcList[i].SortingOrder < dlcPackageInfo.SortingOrder;
						if (flag4)
						{
							this._dlcList.Insert(i, dlcPackageInfo);
							break;
						}
					}
					bool flag5 = this._dlcList.Count == count;
					if (flag5)
					{
						this._dlcList.Add(dlcPackageInfo);
					}
				}
				else
				{
					this._dlcList.Add(dlcPackageInfo);
				}
				ValueTuple<ushort, ushort, ushort, ushort> valueTuple = BitOperation.UnpackVersion(dlcPackageInfo.Version);
				ushort major = valueTuple.Item1;
				ushort minor = valueTuple.Item2;
				ushort build = valueTuple.Item3;
				ushort revision = valueTuple.Item4;
				AdaptableLog.Info(string.Format("Dlc {0} {1}.{2}.{3}.{4} loaded ...", new object[]
				{
					dlcPackageInfo.Name,
					major,
					minor,
					build,
					revision
				}));
				bool flag6 = !this._dlcDataMap.TryAdd(dlcPackageInfo.Id, dlcPackageInfo);
				if (flag6)
				{
					Debug.LogWarning("Dlc " + dlcPackageInfo.Name + " has already exist!");
				}
				this.LoadAtlasInfo(dlcPackageInfo.ResourceDirectory, dlcPackageInfo);
				this.LoadAudioInfo(dlcPackageInfo.ResourceDirectory, dlcPackageInfo);
			}
		}
	}

	// Token: 0x06000096 RID: 150 RVA: 0x00003F38 File Offset: 0x00002138
	private DlcPackageInfo LoadDlcPackageInfo(uint dlcAppId)
	{
		ImplementedDlcItem implementedDlcInfo = DlcManager.GetDlcConfigItem(dlcAppId);
		bool flag = implementedDlcInfo == null;
		DlcPackageInfo result;
		if (flag)
		{
			result = null;
		}
		else
		{
			string dlcName = implementedDlcInfo.Name;
			string dlcEntryDirectory = Application.dataPath + "/" + dlcName;
			Version version = this.DetermineDlcVersionToLoad(GameApp.Instance.ParsedGameVersion, new DirectoryInfo(dlcEntryDirectory));
			string assetBundleDirectory = Path.Combine(dlcEntryDirectory, version.ToString(), "DlcResources");
			string infoPath = Path.Combine(assetBundleDirectory, DlcManager.MakeInfoAbName(dlcName));
			bool flag2 = !File.Exists(infoPath);
			if (flag2)
			{
				result = null;
			}
			else
			{
				AssetBundle infoBundle = AssetBundle.LoadFromFile(infoPath);
				DlcPackageInfo dlcPackageInfo = infoBundle.LoadAsset<DlcPackageInfo>("Info");
				dlcPackageInfo.Id = dlcAppId;
				dlcPackageInfo.ResourceDirectory = assetBundleDirectory;
				ResourcePackage package = SingletonObject.getInstance<ResLoader>().AddDlcResourcePackage(dlcName, assetBundleDirectory);
				bool flag3 = package == null;
				if (flag3)
				{
					result = null;
				}
				else
				{
					package.PackageName = dlcPackageInfo.Identifier;
					infoBundle.Unload(false);
					result = dlcPackageInfo;
				}
			}
		}
		return result;
	}

	// Token: 0x06000097 RID: 151 RVA: 0x0000402C File Offset: 0x0000222C
	public static string MakeInfoAbName(string dlcName)
	{
		return dlcName.ToLower() + "_info" + FrameCommon.AbSuffix;
	}

	// Token: 0x06000098 RID: 152 RVA: 0x00004054 File Offset: 0x00002254
	public bool LoadPacker(string packerName, Action<SpriteAtlas> onLoad)
	{
		bool flag = this._dlcList == null;
		bool result;
		if (flag)
		{
			result = false;
		}
		else
		{
			int i = 0;
			int max = this._dlcList.Count;
			while (i < max)
			{
				DlcPackageInfo dlcPackageInfo = this._dlcList[i];
				bool flag2 = null == dlcPackageInfo.AtlasInfo;
				if (!flag2)
				{
					bool flag3 = dlcPackageInfo.AtlasInfo.HasPacker(packerName);
					if (flag3)
					{
						dlcPackageInfo.AtlasInfo.LoadPacker(packerName, onLoad);
						return true;
					}
				}
				i++;
			}
			result = false;
		}
		return result;
	}

	// Token: 0x06000099 RID: 153 RVA: 0x000040E4 File Offset: 0x000022E4
	public bool SetImageSprite(CImage image, string spriteName, bool setSpriteOnly = false)
	{
		bool flag = this._dlcList == null;
		bool result;
		if (flag)
		{
			result = false;
		}
		else
		{
			int i = 0;
			int max = this._dlcList.Count;
			while (i < max)
			{
				DlcPackageInfo dlcPackageInfo = this._dlcList[i];
				bool flag2 = null == dlcPackageInfo.AtlasInfo;
				if (!flag2)
				{
					if (setSpriteOnly)
					{
						bool flag3 = dlcPackageInfo.AtlasInfo.SetImageSpriteOnly(image, spriteName);
						if (flag3)
						{
							return true;
						}
					}
					else
					{
						bool flag4 = dlcPackageInfo.AtlasInfo.SetImageSprite(image, spriteName);
						if (flag4)
						{
							return true;
						}
					}
				}
				i++;
			}
			result = false;
		}
		return result;
	}

	// Token: 0x0600009A RID: 154 RVA: 0x0000418C File Offset: 0x0000238C
	public bool SetEventBackTexture(CRawImage rawImage, string textureName)
	{
		bool flag = this._dlcList == null;
		bool result;
		if (flag)
		{
			result = false;
		}
		else
		{
			int i = 0;
			int max = this._dlcList.Count;
			Action<Texture2D> <>9__0;
			while (i < max)
			{
				DlcPackageInfo dlcPackageInfo = this._dlcList[i];
				string path = dlcPackageInfo.GetEventBackFilePath(textureName);
				bool flag2 = string.IsNullOrEmpty(path);
				if (!flag2)
				{
					string assetPath = path;
					Action<Texture2D> onLoad;
					if ((onLoad = <>9__0) == null)
					{
						onLoad = (<>9__0 = delegate(Texture2D tex)
						{
							rawImage.texture = tex;
							rawImage.enabled = true;
						});
					}
					ResLoader.Load<Texture2D>(assetPath, onLoad, null, false);
					return true;
				}
				i++;
			}
			result = false;
		}
		return result;
	}

	// Token: 0x0600009B RID: 155 RVA: 0x00004234 File Offset: 0x00002434
	public bool GetAudioClip(string clipName, bool isMusic, Action<AudioClip> onGetClip)
	{
		bool flag = this._dlcList == null || onGetClip == null;
		bool result;
		if (flag)
		{
			result = false;
		}
		else
		{
			int i = 0;
			int max = this._dlcList.Count;
			while (i < max)
			{
				DlcPackageInfo dlcPackageInfo = this._dlcList[i];
				bool flag2 = dlcPackageInfo.AudioInfo == null;
				if (!flag2)
				{
					bool flag3 = dlcPackageInfo.AudioInfo.HasClip(clipName, isMusic);
					if (flag3)
					{
						if (isMusic)
						{
							dlcPackageInfo.AudioInfo.GetMusic(clipName, onGetClip, null);
						}
						else
						{
							string packageName = dlcPackageInfo.AudioInfo.GetClipPackageName(clipName);
							dlcPackageInfo.AudioInfo.LoadPackage(packageName, delegate
							{
								AudioClip clip = dlcPackageInfo.AudioInfo.GetClip(clipName, "");
								onGetClip(clip);
							});
						}
						return true;
					}
				}
				i++;
			}
			result = false;
		}
		return result;
	}

	// Token: 0x0600009C RID: 156 RVA: 0x00004384 File Offset: 0x00002584
	public DlcInfoList GetDlcInfoList()
	{
		DlcInfoList dlcInfoList = DlcInfoList.Create();
		foreach (DlcInfo dlc in this._dlcInfoList)
		{
			dlcInfoList.Items.Add(dlc);
		}
		return dlcInfoList;
	}

	// Token: 0x0600009D RID: 157 RVA: 0x000043EC File Offset: 0x000025EC
	private void InitializeDlcInfoList()
	{
		bool flag = this._dlcList == null;
		if (!flag)
		{
			int i = 0;
			int max = this._dlcList.Count;
			while (i < max)
			{
				DlcPackageInfo dlcInfo = this._dlcList[i];
				ImplementedDlcItem implementedDlcInfo = DlcManager.GetDlcConfigItem(dlcInfo.Id);
				string dlcName = implementedDlcInfo.Name;
				string dlcEntryDirectory = Application.dataPath + "/" + dlcName;
				Version version = this.DetermineDlcVersionToLoad(GameApp.Instance.ParsedGameVersion, new DirectoryInfo(dlcEntryDirectory));
				string dlcDirWithVersion = Path.Combine(dlcEntryDirectory, version.ToString());
				bool eventsExistFlag = true;
				string eventsDir = Path.Combine(dlcDirWithVersion, "Events");
				foreach (string eventFile in dlcInfo.Events)
				{
					string eventFilePath = Path.Combine(eventsDir, "EventLib", eventFile).PathFix();
					bool flag2 = !File.Exists(eventFilePath);
					if (flag2)
					{
						eventsExistFlag = false;
						break;
					}
				}
				bool flag3 = eventsExistFlag;
				if (flag3)
				{
					this._dlcInfoList.Add(new DlcInfo((ulong)dlcInfo.Id, dlcInfo.Version, this.IsDlcInstalled(dlcInfo.Id), eventsDir));
				}
				i++;
			}
		}
	}

	// Token: 0x0600009E RID: 158 RVA: 0x0000454C File Offset: 0x0000274C
	private void LoadAtlasInfo(string directoryPath, DlcPackageInfo dlcPackageInfo)
	{
		AtlasInfo atlasInfo = dlcPackageInfo.AtlasInfo;
		bool flag = null != atlasInfo;
		if (flag)
		{
			atlasInfo.InitSelf();
		}
		dlcPackageInfo.AtlasInfo = atlasInfo;
	}

	// Token: 0x0600009F RID: 159 RVA: 0x0000457C File Offset: 0x0000277C
	private void LoadAudioInfo(string directoryPath, DlcPackageInfo dlcPackageInfo)
	{
		AudioInfos audioInfos = dlcPackageInfo.AudioInfo;
		bool flag = null != audioInfos;
		if (flag)
		{
			audioInfos.InitSelf();
		}
		dlcPackageInfo.AudioInfo = audioInfos;
	}

	// Token: 0x060000A0 RID: 160 RVA: 0x000045AC File Offset: 0x000027AC
	public bool IsDlcInstalled(uint dlcAppId)
	{
		return SteamManager.IsDlcInstalled(dlcAppId) && this._dlcDataMap.ContainsKey(dlcAppId);
	}

	// Token: 0x060000A1 RID: 161 RVA: 0x000045D8 File Offset: 0x000027D8
	public List<uint> GetInstalledDlcList()
	{
		List<uint> dlcList = new List<uint>();
		foreach (uint dlcId in this._dlcDataMap.Keys)
		{
			dlcList.Add(dlcId);
		}
		return dlcList;
	}

	// Token: 0x060000A2 RID: 162 RVA: 0x00004644 File Offset: 0x00002844
	public List<DlcId> GetDlcIdList()
	{
		return (from dlcInfo in this._dlcInfoList
		select dlcInfo.DlcId).ToList<DlcId>();
	}

	// Token: 0x060000A3 RID: 163 RVA: 0x00004688 File Offset: 0x00002888
	public string GetDlcName(DlcId dlcId)
	{
		string name;
		bool flag = DlcManager.DlcNameDict.TryGetValue((uint)dlcId.AppId, out name);
		string result;
		if (flag)
		{
			result = name;
		}
		else
		{
			DlcPackageInfo dlcInfo;
			bool flag2 = this._dlcDataMap.TryGetValue((uint)dlcId.AppId, out dlcInfo);
			if (flag2)
			{
				result = dlcInfo.Name;
			}
			else
			{
				result = string.Empty;
			}
		}
		return result;
	}

	// Token: 0x060000A4 RID: 164 RVA: 0x000046E0 File Offset: 0x000028E0
	public bool CheckDlcAssetBundle(uint dlcAppId, Version gameVersion)
	{
		string dlcName = DlcManager.GetDlcConfigItem(dlcAppId).Name;
		string entryDirectory = Application.dataPath + "/" + dlcName;
		DirectoryInfo dlcEntryDir = new DirectoryInfo(entryDirectory);
		bool flag = !dlcEntryDir.Exists;
		bool result;
		if (flag)
		{
			result = false;
		}
		else
		{
			Version dlcVersion = this.DetermineDlcVersionToLoad(gameVersion, dlcEntryDir);
			string directoryPath = Path.Combine(entryDirectory, dlcVersion.ToString());
			string abPath = Path.Combine(directoryPath, "DlcResources");
			string infoFilePath = Path.Combine(abPath, DlcManager.MakeInfoAbName(dlcName));
			string controllerFilePath = Path.Combine(abPath, dlcName.ToLower() + "_packagecontroller" + FrameCommon.AbSuffix);
			bool flag2 = !File.Exists(infoFilePath) || !File.Exists(controllerFilePath);
			if (flag2)
			{
				result = false;
			}
			else
			{
				AssetBundle infoBundle = AssetBundle.LoadFromFile(infoFilePath);
				foreach (string eventFile in infoBundle.LoadAsset<DlcPackageInfo>("Info").Events)
				{
					bool flag3 = !File.Exists(Path.Combine(directoryPath, "Events/EventLib", eventFile));
					if (flag3)
					{
						infoBundle.Unload(false);
						return false;
					}
				}
				infoBundle.Unload(false);
				AssetBundle controllerBundle = AssetBundle.LoadFromFile(controllerFilePath);
				PackageAssetBundleController controller = controllerBundle.LoadAsset<PackageAssetBundleController>("PackageAssetBundleController");
				controllerBundle.Unload(false);
				result = (controller != null && controller.CheckDlcAssetBundles(abPath));
			}
		}
		return result;
	}

	// Token: 0x060000A5 RID: 165 RVA: 0x00004868 File Offset: 0x00002A68
	private Version DetermineDlcVersionToLoad(Version gameVersion, DirectoryInfo dlcEntryDir)
	{
		Version maxVersion = new Version(0, 0, 0, 0);
		bool flag = gameVersion == null;
		Version result;
		if (flag)
		{
			result = maxVersion;
		}
		else
		{
			foreach (DirectoryInfo dir in dlcEntryDir.GetDirectories())
			{
				Version version;
				bool flag2 = Version.TryParse(dir.Name, out version);
				if (flag2)
				{
					bool flag3 = version > maxVersion && version <= gameVersion;
					if (flag3)
					{
						maxVersion = version;
					}
				}
			}
			result = maxVersion;
		}
		return result;
	}

	// Token: 0x060000A6 RID: 166 RVA: 0x000048EA File Offset: 0x00002AEA
	public void Dispose()
	{
	}

	// Token: 0x060000A7 RID: 167 RVA: 0x000048F0 File Offset: 0x00002AF0
	public void Init()
	{
		ImplementedDlc.Instance.Init();
		this._dlcDataMap = new Dictionary<uint, DlcPackageInfo>();
		this._dlcInfoList = new List<DlcInfo>();
		Version gameVersion = GameApp.Instance.ParsedGameVersion;
		foreach (ImplementedDlcItem dlcConfig in ((IEnumerable<ImplementedDlcItem>)ImplementedDlc.Instance))
		{
			bool flag = !dlcConfig.OnlyDisplay;
			if (flag)
			{
				bool flag2 = this.CheckDlcAssetBundle(dlcConfig.AppId, gameVersion);
				if (flag2)
				{
					this.TryLoadDlc(dlcConfig.AppId);
				}
			}
		}
		this.InitializeDlcInfoList();
		DlcManager.DlcNameDict.Clear();
		bool initialized = SteamManager.Initialized;
		if (initialized)
		{
			int count = SteamApps.GetDLCCount();
			for (int i = 0; i < count; i++)
			{
				AppId_t appID;
				bool available;
				string name;
				SteamApps.BGetDLCDataByIndex(i, out appID, out available, out name, 128);
				DlcManager.DlcNameDict[appID.m_AppId] = name;
			}
		}
	}

	// Token: 0x04000041 RID: 65
	public const string DlcGitDirName = "Dlc";

	// Token: 0x04000042 RID: 66
	public const string DlcOutputDirName = "Dlc2";

	// Token: 0x04000043 RID: 67
	public const string DlcAssetsDirName = "Dlc";

	// Token: 0x04000044 RID: 68
	public const string DlcResourcesDirName = "DlcResources";

	// Token: 0x04000045 RID: 69
	public const string DlcEventsDirName = "Events";

	// Token: 0x04000046 RID: 70
	public const string DlcNameInteractOfLove = "InteractOfLove";

	// Token: 0x04000047 RID: 71
	public const string DlcNameGiftFromConchShip1 = "GiftFromConchShip1";

	// Token: 0x04000048 RID: 72
	public const string DlcNameGiftFromConchShip2 = "GiftFromConchShip2";

	// Token: 0x04000049 RID: 73
	public const string DlcNameFiveLoong = "FiveLoong";

	// Token: 0x0400004A RID: 74
	public const string DlcNameHappyNewYear2024 = "HappyNewYear2024";

	// Token: 0x0400004B RID: 75
	public const string DlcNameYearOfSnakeCloth = "YearOfSnakeCloth";

	// Token: 0x0400004C RID: 76
	public const string DlcNameHappyNewYear2026 = "HappyNewYear2026";

	// Token: 0x0400004D RID: 77
	public const string DlcNameCricketPolymorph = "CricketPolymorph";

	// Token: 0x0400004E RID: 78
	public const string DlcNameEightYears = "EightYears";

	// Token: 0x0400004F RID: 79
	public const string DlcNameGreenHillsRemain = "GreenHillsRemain";

	// Token: 0x04000050 RID: 80
	public static readonly Dictionary<uint, string> DlcNameDict = new Dictionary<uint, string>();

	// Token: 0x04000051 RID: 81
	private Dictionary<uint, DlcPackageInfo> _dlcDataMap;

	// Token: 0x04000052 RID: 82
	private List<DlcPackageInfo> _dlcList;

	// Token: 0x04000053 RID: 83
	private List<DlcInfo> _dlcInfoList;
}
