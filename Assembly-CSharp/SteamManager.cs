using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using FrameWork;
using FrameWork.ModSystem;
using Game.Views.Mod;
using GameData.Domains.Mod;
using MoonSharp.Interpreter;
using MoonSharp.Interpreter.Serialization;
using Steamworks;
using UnityEngine;

// Token: 0x02000141 RID: 321
internal static class SteamManager
{
	// Token: 0x170001DE RID: 478
	// (get) Token: 0x0600111A RID: 4378 RVA: 0x000671B2 File Offset: 0x000653B2
	// (set) Token: 0x0600111B RID: 4379 RVA: 0x000671B9 File Offset: 0x000653B9
	public static bool Initialized { get; private set; }

	// Token: 0x170001DF RID: 479
	// (get) Token: 0x0600111C RID: 4380 RVA: 0x000671C1 File Offset: 0x000653C1
	public static bool IsLoggedOn
	{
		get
		{
			return SteamManager.Initialized && SteamUser.BLoggedOn();
		}
	}

	// Token: 0x170001E0 RID: 480
	// (get) Token: 0x0600111D RID: 4381 RVA: 0x000671D2 File Offset: 0x000653D2
	// (set) Token: 0x0600111E RID: 4382 RVA: 0x000671D9 File Offset: 0x000653D9
	public static string Branch { get; private set; }

	// Token: 0x170001E1 RID: 481
	// (get) Token: 0x0600111F RID: 4383 RVA: 0x000671E1 File Offset: 0x000653E1
	// (set) Token: 0x06001120 RID: 4384 RVA: 0x000671E8 File Offset: 0x000653E8
	public static string Language { get; set; }

	// Token: 0x170001E2 RID: 482
	// (get) Token: 0x06001121 RID: 4385 RVA: 0x000671F0 File Offset: 0x000653F0
	public static ulong SteamId
	{
		get
		{
			return SteamManager._steamId.m_SteamID;
		}
	}

	// Token: 0x170001E3 RID: 483
	// (get) Token: 0x06001122 RID: 4386 RVA: 0x000671FC File Offset: 0x000653FC
	private static EUGCQuery WorkshopModQueryType
	{
		get
		{
			EUGCQuery result;
			switch (ModSubPageShop.CurWorkshopSortToggleKey)
			{
			case ModSubPageShop.WorkshopSortToggleKey.MostPopular:
				result = ((ModSubPageShop.CurWorkshopTimeToggleKey == ModSubPageShop.WorkshopTimeToggleKey.All) ? EUGCQuery.k_EUGCQuery_RankedByVote : EUGCQuery.k_EUGCQuery_RankedByTrend);
				break;
			case ModSubPageShop.WorkshopSortToggleKey.MostRated:
				result = EUGCQuery.k_EUGCQuery_RankedByVotesUp;
				break;
			case ModSubPageShop.WorkshopSortToggleKey.LatestUpdate:
				result = EUGCQuery.k_EUGCQuery_RankedByLastUpdatedDate;
				break;
			case ModSubPageShop.WorkshopSortToggleKey.LatestUpload:
				result = EUGCQuery.k_EUGCQuery_RankedByPublicationDate;
				break;
			default:
				result = EUGCQuery.k_EUGCQuery_RankedByVote;
				break;
			}
			return result;
		}
	}

	// Token: 0x170001E4 RID: 484
	// (get) Token: 0x06001123 RID: 4387 RVA: 0x0006724C File Offset: 0x0006544C
	public static List<ModId> CurWorkshopList
	{
		get
		{
			List<ModId> list;
			bool flag = SteamManager.QueryWorkshopModByTimeDict.TryGetValue(ModSubPageShop.CurWorkshopTimeToggleKey, out list);
			List<ModId> result;
			if (flag)
			{
				result = list;
			}
			else
			{
				result = null;
			}
			return result;
		}
	}

	// Token: 0x06001124 RID: 4388 RVA: 0x00067278 File Offset: 0x00065478
	public static void Initialize()
	{
		bool flag = !Packsize.Test();
		if (flag)
		{
			Debug.LogError("[Steamworks.NET] Packsize Test returned false, the wrong version of Steamworks.NET is being run in this platform.");
		}
		bool flag2 = !DllCheck.Test();
		if (flag2)
		{
			Debug.LogError("[Steamworks.NET] DllCheck Test returned false, One or more of the Steamworks binaries seems to be the wrong version.");
		}
		SteamManager.Initialized = SteamAPI.Init();
		bool flag3 = !SteamManager.Initialized;
		if (flag3)
		{
			throw new Exception("Failed to initialize steam API.");
		}
		SteamManager._steamApiWarningMessageHook = new SteamAPIWarningMessageHook_t(SteamManager.LogWarn);
		SteamClient.SetWarningMessageHook(SteamManager._steamApiWarningMessageHook);
		SteamManager._steamId = SteamUser.GetSteamID();
		EUserHasLicenseForAppResult hasLicense = SteamUser.UserHasLicenseForApp(SteamManager._steamId, SteamManager._appId);
		bool flag4 = hasLicense == EUserHasLicenseForAppResult.k_EUserHasLicenseResultDoesNotHaveLicense;
		if (flag4)
		{
			throw new Exception(string.Format("Current user {0} does not have license for appid {1}, license status: {2}.", SteamManager._steamId, SteamManager._appId, hasLicense));
		}
		string pchName;
		bool currentBetaName = SteamApps.GetCurrentBetaName(out pchName, 255);
		if (currentBetaName)
		{
			SteamManager.Branch = pchName;
		}
		SteamManager.Language = SteamApps.GetCurrentGameLanguage();
		string language = SteamManager.Language;
		if (!true)
		{
		}
		string language2;
		if (!(language == "schinese"))
		{
			if (!(language == "english"))
			{
				if (!(language == "koreana"))
				{
					if (!(language == "tchinese"))
					{
						if (!(language == "japanese"))
						{
							language2 = SteamManager.Language;
						}
						else
						{
							language2 = "JP";
						}
					}
					else
					{
						language2 = "CNH";
					}
				}
				else
				{
					language2 = "KO";
				}
			}
			else
			{
				language2 = "EN";
			}
		}
		else
		{
			language2 = "CN";
		}
		if (!true)
		{
		}
		SteamManager.Language = language2;
		Debug.Log("Verifying steam user successful! steam id " + SteamManager._steamId.ToString());
		Debug.Log("Current branch: " + SteamManager.Branch);
	}

	// Token: 0x06001125 RID: 4389 RVA: 0x00067438 File Offset: 0x00065638
	public static void Update()
	{
		bool flag = !SteamManager.Initialized;
		if (!flag)
		{
			SteamAPI.RunCallbacks();
		}
	}

	// Token: 0x06001126 RID: 4390 RVA: 0x0006745A File Offset: 0x0006565A
	public static void AddAchievement(string achievementName)
	{
		SteamUserStats.SetAchievement(achievementName);
	}

	// Token: 0x06001127 RID: 4391 RVA: 0x00067464 File Offset: 0x00065664
	public static void IsTestVersion()
	{
	}

	// Token: 0x06001128 RID: 4392 RVA: 0x00067468 File Offset: 0x00065668
	public static bool IsDlcInstalled(uint dlcAppId)
	{
		return SteamManager.Initialized && SteamApps.BIsDlcInstalled(new AppId_t(dlcAppId));
	}

	// Token: 0x06001129 RID: 4393 RVA: 0x0006748F File Offset: 0x0006568F
	public static void LogWarn(int nSeverity, StringBuilder pchDebugText)
	{
		Debug.LogWarning(pchDebugText);
	}

	// Token: 0x0600112A RID: 4394 RVA: 0x0006749C File Offset: 0x0006569C
	public static void UnInitialize()
	{
		bool flag = !SteamManager.Initialized;
		if (!flag)
		{
			SteamAPI.Shutdown();
		}
	}

	// Token: 0x0600112B RID: 4395 RVA: 0x000674C0 File Offset: 0x000656C0
	public static void UpdateSubscribedItems(List<ModId> modIds, Dictionary<string, ModInfoWithDisplayData> modInfoDict, Dictionary<string, string> modNameDict)
	{
		SteamManager.<>c__DisplayClass38_0 CS$<>8__locals1 = new SteamManager.<>c__DisplayClass38_0();
		CS$<>8__locals1.modIds = modIds;
		CS$<>8__locals1.modInfoDict = modInfoDict;
		CS$<>8__locals1.modNameDict = modNameDict;
		bool flag = !SteamManager.Initialized;
		if (flag)
		{
			Action<Dictionary<ModId, bool>> onUpdateSubscribedItems = ModManager.OnUpdateSubscribedItems;
			if (onUpdateSubscribedItems != null)
			{
				onUpdateSubscribedItems(null);
			}
		}
		else
		{
			CS$<>8__locals1.pageCount = 1U;
			CS$<>8__locals1.dependenciesChangeStateDict = new Dictionary<ModId, bool>();
			CS$<>8__locals1.dependencies = null;
			CS$<>8__locals1.itemCount = 0U;
			CS$<>8__locals1.<UpdateSubscribedItems>g__Send|0();
		}
	}

	// Token: 0x0600112C RID: 4396 RVA: 0x00067534 File Offset: 0x00065734
	public static void UpdateAllItems(List<ModId> modIds, Dictionary<string, ModInfoWithDisplayData> modInfoDict, Dictionary<string, string> modNameDict, string searchText, List<string> searchTags, uint pageCount)
	{
		SteamManager.<>c__DisplayClass39_0 CS$<>8__locals1 = new SteamManager.<>c__DisplayClass39_0();
		CS$<>8__locals1.modIds = modIds;
		CS$<>8__locals1.modInfoDict = modInfoDict;
		CS$<>8__locals1.modNameDict = modNameDict;
		CS$<>8__locals1.pageCount = pageCount;
		bool flag = !SteamManager.Initialized;
		if (flag)
		{
			Action<uint, List<ModId>> onUpdateAllItems = ModManager.OnUpdateAllItems;
			if (onUpdateAllItems != null)
			{
				onUpdateAllItems(0U, null);
			}
		}
		else
		{
			CallResult<SteamUGCQueryCompleted_t> callResult = CallResult<SteamUGCQueryCompleted_t>.Create(null);
			SteamManager._ugcQueryHandle = SteamUGC.CreateQueryAllUGCRequest(SteamManager.WorkshopModQueryType, EUGCMatchingUGCType.k_EUGCMatchingUGCType_Items_ReadyToUse, SteamManager._appId, SteamManager._appId, CS$<>8__locals1.pageCount);
			bool flag2 = SteamManager.WorkshopModQueryType == EUGCQuery.k_EUGCQuery_RankedByTrend;
			if (flag2)
			{
				SteamUGC.SetRankedByTrendDays(SteamManager._ugcQueryHandle, ModSubPageShop.CurWorkshopTime);
			}
			SteamUGC.SetSearchText(SteamManager._ugcQueryHandle, searchText);
			bool flag3 = searchTags.Count > 0;
			if (flag3)
			{
				foreach (string tag in searchTags)
				{
					SteamUGC.AddRequiredTag(SteamManager._ugcQueryHandle, tag);
				}
			}
			SteamUGC.SetReturnChildren(SteamManager._ugcQueryHandle, true);
			SteamUGC.SetReturnLongDescription(SteamManager._ugcQueryHandle, true);
			SteamUGC.SetReturnAdditionalPreviews(SteamManager._ugcQueryHandle, true);
			SteamAPICall_t steamAPICall = SteamUGC.SendQueryUGCRequest(SteamManager._ugcQueryHandle);
			callResult.Set(steamAPICall, delegate(SteamUGCQueryCompleted_t t, bool failure)
			{
				SteamManager.<>c__DisplayClass39_1 CS$<>8__locals2 = new SteamManager.<>c__DisplayClass39_1();
				CS$<>8__locals2.CS$<>8__locals1 = CS$<>8__locals1;
				CS$<>8__locals2.t = t;
				CS$<>8__locals2.failure = failure;
				SteamManager.OnSteamCallResult(new Action(CS$<>8__locals2.<UpdateAllItems>g__OnExecute|2), new Action<bool>(CS$<>8__locals2.<UpdateAllItems>g__OnFailed|1));
			});
		}
	}

	// Token: 0x0600112D RID: 4397 RVA: 0x0006767C File Offset: 0x0006587C
	public static void UpdateUploadedItems(List<ModId> modIds, Dictionary<string, ModInfoWithDisplayData> modInfoDict, Dictionary<string, string> modNameDict)
	{
		SteamManager.<>c__DisplayClass40_0 CS$<>8__locals1 = new SteamManager.<>c__DisplayClass40_0();
		CS$<>8__locals1.modIds = modIds;
		CS$<>8__locals1.modInfoDict = modInfoDict;
		CS$<>8__locals1.modNameDict = modNameDict;
		bool flag = !SteamManager.Initialized;
		if (flag)
		{
			Action<Dictionary<ModId, bool>> onUpdateUploadedItems = ModManager.OnUpdateUploadedItems;
			if (onUpdateUploadedItems != null)
			{
				onUpdateUploadedItems(null);
			}
		}
		else
		{
			CS$<>8__locals1.dependenciesChangeStateDict = new Dictionary<ModId, bool>();
			CS$<>8__locals1.dependencies = null;
			CS$<>8__locals1.pageCount = 1U;
			CS$<>8__locals1.itemCount = 0U;
			CS$<>8__locals1.<UpdateUploadedItems>g__Send|0();
		}
	}

	// Token: 0x0600112E RID: 4398 RVA: 0x000676F0 File Offset: 0x000658F0
	public static void UpdateTargetItems(List<ModId> modIds, Dictionary<string, ModInfoWithDisplayData> modInfoDict, Dictionary<string, string> modNameDict, IReadOnlyList<ulong> targetFileIdList, List<ulong> missingList)
	{
		SteamManager.<>c__DisplayClass41_0 CS$<>8__locals1 = new SteamManager.<>c__DisplayClass41_0();
		CS$<>8__locals1.modIds = modIds;
		CS$<>8__locals1.modInfoDict = modInfoDict;
		CS$<>8__locals1.modNameDict = modNameDict;
		CS$<>8__locals1.missingList = missingList;
		bool flag = !SteamManager.Initialized;
		if (flag)
		{
			Action<Dictionary<ModId, bool>> onUpdateTargetItems = ModManager.OnUpdateTargetItems;
			if (onUpdateTargetItems != null)
			{
				onUpdateTargetItems(null);
			}
		}
		else
		{
			CS$<>8__locals1.dependenciesChangeStateDict = new Dictionary<ModId, bool>();
			CS$<>8__locals1.<UpdateTargetItems>g__Send|0(targetFileIdList);
		}
	}

	// Token: 0x0600112F RID: 4399 RVA: 0x00067758 File Offset: 0x00065958
	private static bool UpdateItem(uint index, List<ModId> modIds, Dictionary<string, ModInfoWithDisplayData> modInfoDict, Dictionary<string, string> modNameDict, out ModInfoWithDisplayData modInfo, out uint childCount, out EItemState itemState, out bool isMissing)
	{
		modInfo = null;
		childCount = 0U;
		itemState = EItemState.k_EItemStateNone;
		isMissing = false;
		SteamUGCDetails_t details;
		SteamUGC.GetQueryUGCResult(SteamManager._ugcQueryHandle, index, out details);
		bool flag = details.m_eResult != EResult.k_EResultOK;
		bool result;
		if (flag)
		{
			bool flag2 = details.m_eResult == EResult.k_EResultFileNotFound;
			if (flag2)
			{
				isMissing = true;
			}
			result = false;
		}
		else
		{
			ulong fileId = details.m_nPublishedFileId.m_PublishedFileId;
			modNameDict[string.Format("{0}_{1}", 1, fileId)] = details.m_pchFileName;
			int curIndex = modIds.FindIndex((ModId m) => m.FileId == fileId);
			ModId modId = (curIndex < 0) ? new ModId(fileId, 0UL, 1) : modIds[curIndex];
			bool flag3 = !modInfoDict.TryGetValue(modId.ToString(), out modInfo);
			if (flag3)
			{
				modInfo = new ModInfoWithDisplayData
				{
					ModId = modId
				};
				modInfoDict[modId.ToString()] = modInfo;
			}
			bool flag4 = curIndex < 0;
			if (flag4)
			{
				modIds.Add(modId);
			}
			modInfo.Title = details.m_rgchTitle;
			modInfo.Description = details.m_rgchDescription;
			modInfo.OriginScore = details.m_flScore;
			modInfo.VoteCount = details.m_unVotesUp + details.m_unVotesDown;
			modInfo.FileSize = details.m_nFileSize;
			modInfo.CreateData = details.m_rtimeCreated;
			modInfo.UpdateData = details.m_rtimeUpdated;
			modInfo.Visibility = (EModVisibility)details.m_eVisibility;
			childCount = details.m_unNumChildren;
			ModInfoWithDisplayData modInfoWithDisplayData = modInfo;
			if (modInfoWithDisplayData.TagList == null)
			{
				modInfoWithDisplayData.TagList = new List<string>();
			}
			modInfo.TagList.Clear();
			string[] tagArray = details.m_rgchTags.Split(',', StringSplitOptions.None);
			modInfo.TagList.AddRange(from t in tagArray
			where !t.IsNullOrEmpty()
			select t);
			ulong subscribeCount;
			SteamUGC.GetQueryUGCStatistic(SteamManager._ugcQueryHandle, index, EItemStatistic.k_EItemStatistic_NumUniqueSubscriptions, out subscribeCount);
			ulong favoriteCount;
			SteamUGC.GetQueryUGCStatistic(SteamManager._ugcQueryHandle, index, EItemStatistic.k_EItemStatistic_NumUniqueFavorites, out favoriteCount);
			modInfo.FavoriteCount = favoriteCount;
			modInfo.SubscribeCount = subscribeCount;
			modInfo.PreviewFileHandle = details.m_hPreviewFile;
			string metadata;
			SteamUGC.GetQueryUGCMetadata(SteamManager._ugcQueryHandle, index, out metadata, 5000U);
			Table table = LuaGame.Instance.ReadMoonSharpTable(metadata);
			bool flag5 = table != null;
			if (flag5)
			{
				table.Load("Author", out modInfo.Author);
				table.Load("ChangeConfig", out modInfo.ChangeConfig);
				table.Load("HasArchive", out modInfo.HasArchive);
				table.Load("NeedRestart", out modInfo.NeedRestartWhenSettingChanged);
			}
			else
			{
				modInfo.Author = metadata;
			}
			bool flag6 = modInfo.Author.IsNullOrEmpty();
			if (flag6)
			{
				modInfo.Author = SteamFriends.GetFriendPersonaName(new CSteamID(details.m_ulSteamIDOwner));
			}
			bool flag7 = modInfo.Author.IsNullOrEmpty() || modInfo.Author.Contains("unknown");
			if (flag7)
			{
				modInfo.Author = details.m_ulSteamIDOwner.ToString();
			}
			itemState = (EItemState)SteamUGC.GetItemState(details.m_nPublishedFileId);
			modInfo.IsSubscribed = SteamManager.IsItemStateActive(itemState, EItemState.k_EItemStateSubscribed);
			uint previewsCount = SteamUGC.GetQueryUGCNumAdditionalPreviews(SteamManager._ugcQueryHandle, index);
			modInfo.DetailFileCount = (int)previewsCount;
			result = true;
		}
		return result;
	}

	// Token: 0x06001130 RID: 4400 RVA: 0x00067AE0 File Offset: 0x00065CE0
	private static void UpdateItemDependencies(Dictionary<string, ModInfoWithDisplayData> modInfoDict, ModInfoWithDisplayData modInfo, uint index, uint childCount)
	{
		bool flag = modInfo == null;
		if (!flag)
		{
			modInfo.Dependencies.Clear();
			modInfo.RemoteDependencies.Clear();
			bool flag2 = childCount > 0U;
			if (flag2)
			{
				PublishedFileId_t[] childPublishedFileIds = new PublishedFileId_t[childCount];
				bool queryUGCChildren = SteamUGC.GetQueryUGCChildren(SteamManager._ugcQueryHandle, index, childPublishedFileIds, childCount);
				if (queryUGCChildren)
				{
					foreach (PublishedFileId_t publishedFileId in childPublishedFileIds)
					{
						modInfo.Dependencies.Add(publishedFileId.m_PublishedFileId);
						modInfo.RemoteDependencies.Add(publishedFileId.m_PublishedFileId);
					}
				}
			}
			ModId modId = modInfo.ModId;
			modId.Source = 0;
			ModInfoWithDisplayData localModInfo;
			bool flag3 = modInfoDict.TryGetValue(modId.ToString(), out localModInfo);
			if (flag3)
			{
				localModInfo.RemoteDependencies.Clear();
				localModInfo.RemoteDependencies.AddRange(modInfo.RemoteDependencies);
			}
		}
	}

	// Token: 0x06001131 RID: 4401 RVA: 0x00067BD0 File Offset: 0x00065DD0
	private static bool CheckModDependencyHasChanged(uint index, ModInfoWithDisplayData modInfo, uint childNumber, ref List<ulong> dependencies)
	{
		if (dependencies == null)
		{
			dependencies = new List<ulong>();
		}
		dependencies.Clear();
		bool flag = childNumber > 0U;
		if (flag)
		{
			PublishedFileId_t[] childPublishedFileIds = new PublishedFileId_t[childNumber];
			bool queryUGCChildren = SteamUGC.GetQueryUGCChildren(SteamManager._ugcQueryHandle, index, childPublishedFileIds, childNumber);
			if (queryUGCChildren)
			{
				foreach (PublishedFileId_t childId in childPublishedFileIds)
				{
					dependencies.Add(childId.m_PublishedFileId);
				}
			}
		}
		return dependencies.ContentIsDifferent(modInfo.Dependencies);
	}

	// Token: 0x06001132 RID: 4402 RVA: 0x00067C5B File Offset: 0x00065E5B
	private static void OnReceiveModInfoList(SteamUGCQueryCompleted_t ugcQueryCompleted, bool failure)
	{
	}

	// Token: 0x06001133 RID: 4403 RVA: 0x00067C60 File Offset: 0x00065E60
	public static void CreateItem(string path, ModInfoWithDisplayData modInfo, EWorkshopFileType fileType, ERemoteStoragePublishedFileVisibility visibility, List<string> logList, Action<UGCUpdateHandle_t> onSucceed, Action onFailed)
	{
		SteamAPICall_t apiCall = SteamUGC.CreateItem(SteamManager._appId, fileType);
		CallResult<CreateItemResult_t> callResult = CallResult<CreateItemResult_t>.Create(null);
		callResult.Set(apiCall, delegate(CreateItemResult_t result, bool failed)
		{
			bool flag = SteamManager.HandleCreateModResult(result.m_nPublishedFileId, result.m_eResult, result.m_bUserNeedsToAcceptWorkshopLegalAgreement);
			if (flag)
			{
				SteamManager.UploadItemUpdate(result.m_nPublishedFileId.m_PublishedFileId, path, modInfo, visibility, logList, onSucceed, onFailed);
			}
			else
			{
				onFailed();
			}
		});
	}

	// Token: 0x06001134 RID: 4404 RVA: 0x00067CC8 File Offset: 0x00065EC8
	public static void UploadItemUpdate(ulong createdFileId, string path, ModInfoWithDisplayData modInfo, ERemoteStoragePublishedFileVisibility visibility, List<string> logList, Action<UGCUpdateHandle_t> onSucceed, Action onFailed)
	{
		bool isCreate = createdFileId > 0UL;
		ulong realFileId = isCreate ? createdFileId : modInfo.ModId.FileId;
		PublishedFileId_t fileId = new PublishedFileId_t(realFileId);
		UGCUpdateHandle_t updateHandle = SteamUGC.StartItemUpdate(SteamManager._appId, fileId);
		bool flag = modInfo.GameVersionStr != GameApp.Instance.GameVersion;
		if (flag)
		{
			ModManager.WriteModInfo(modInfo, Path.Combine(path, "Config.Lua"));
		}
		bool flag2 = !SteamUGC.SetItemTitle(updateHandle, modInfo.Title);
		if (flag2)
		{
			SteamManager.ShowFailedDialog("Fail to set title " + modInfo.Title);
			onFailed();
		}
		else
		{
			bool flag3 = !SteamUGC.SetItemDescription(updateHandle, modInfo.Description);
			if (flag3)
			{
				SteamManager.ShowFailedDialog("Fail to set description " + modInfo.Description);
				onFailed();
			}
			else
			{
				bool flag4 = !SteamUGC.SetItemContent(updateHandle, path);
				if (flag4)
				{
					SteamManager.ShowFailedDialog("Fail to set content at " + path);
					onFailed();
				}
				else
				{
					bool flag5 = !SteamUGC.SetItemTags(updateHandle, modInfo.TagList);
					if (flag5)
					{
						SteamManager.ShowFailedDialog(string.Format("Fail to set tags of count {0}", modInfo.TagList.Count));
						onFailed();
					}
					else
					{
						bool flag6 = !SteamUGC.SetItemVisibility(updateHandle, visibility);
						if (flag6)
						{
							SteamManager.ShowFailedDialog(string.Format("Fail to set visibility {0}", visibility));
							onFailed();
						}
						else
						{
							Table table = new Table(null);
							table.Save("Author", modInfo.Author);
							table.Save("ChangeConfig", modInfo.ChangeConfig);
							table.Save("HasArchive", modInfo.HasArchive);
							table.Save("NeedRestart", modInfo.NeedRestartWhenSettingChanged);
							string str = table.Serialize(true, 0);
							bool flag7 = !SteamUGC.SetItemMetadata(updateHandle, str);
							if (flag7)
							{
								SteamManager.ShowFailedDialog("Fail to set metadata: " + str);
								onFailed();
							}
							else
							{
								string coverName = string.IsNullOrEmpty(modInfo.WorkshopCover) ? modInfo.Cover : modInfo.WorkshopCover;
								bool flag8 = !string.IsNullOrEmpty(coverName);
								if (flag8)
								{
									string coverPath = Path.Combine(path, coverName);
									bool flag9 = File.Exists(coverPath) && !SteamUGC.SetItemPreview(updateHandle, coverPath);
									if (flag9)
									{
										SteamManager.ShowFailedDialog("Fail to set cover at " + coverPath);
										onFailed();
										return;
									}
								}
								int existIndex = ModManager.UploadedMods.FindIndex((ModId m) => m.FileId == realFileId);
								bool flag10 = existIndex >= 0;
								if (flag10)
								{
									ModInfoWithDisplayData existModInfo = ModManager.GetModInfo(ModManager.UploadedMods[existIndex]);
									modInfo.DetailFileCount = existModInfo.DetailFileCount;
								}
								for (int index = 0; index < modInfo.DetailImageList.Count; index++)
								{
									bool exist = index < modInfo.DetailFileCount;
									string fileName = modInfo.DetailImageList[index];
									string filePath = Path.Combine(path, fileName);
									bool flag11 = File.Exists(filePath);
									if (flag11)
									{
										bool flag12 = exist && !SteamUGC.UpdateItemPreviewFile(updateHandle, (uint)index, filePath);
										if (flag12)
										{
											SteamManager.ShowFailedDialog(string.Format("Fail to update preview file at {0} of index {1}", filePath, index));
											onFailed();
											return;
										}
										bool flag13 = !exist && !SteamUGC.AddItemPreviewFile(updateHandle, filePath, EItemPreviewType.k_EItemPreviewType_Image);
										if (flag13)
										{
											SteamManager.ShowFailedDialog("Fail to add preview file at " + filePath);
											onFailed();
											return;
										}
									}
								}
								foreach (ulong dependency in modInfo.RemoteDependencies)
								{
									SteamUGC.RemoveDependency(fileId, new PublishedFileId_t(dependency));
								}
								foreach (ulong dependency2 in modInfo.Dependencies)
								{
									SteamUGC.AddDependency(fileId, new PublishedFileId_t(dependency2));
								}
								long time = DateTimeOffset.UtcNow.ToUnixTimeSeconds();
								ModInfoWithDisplayData modInfo2 = modInfo;
								if (modInfo2.UpdateLogList == null)
								{
									modInfo2.UpdateLogList = new List<ModInfoWithDisplayData.UpdateLog>();
								}
								ModInfoWithDisplayData.UpdateLog updateLog = new ModInfoWithDisplayData.UpdateLog
								{
									LogList = new List<string>(logList),
									Timestamp = time
								};
								modInfo.UpdateLogList.Add(updateLog);
								ulong originFileId = modInfo.ModId.FileId;
								byte originSource = modInfo.ModId.Source;
								modInfo.ModId.FileId = realFileId;
								modInfo.ModId.Source = 1;
								ModManager.SaveModInfo(modInfo);
								ModManager.SaveModSettings(false);
								string changeNote = UI_ModPanel.GetUpdateLog(logList);
								SteamAPICall_t submitCall = SteamUGC.SubmitItemUpdate(updateHandle, changeNote);
								bool flag14 = submitCall == SteamAPICall_t.Invalid;
								if (flag14)
								{
									modInfo.ModId.FileId = originFileId;
									modInfo.ModId.Source = originSource;
									ModManager.SaveModInfo(modInfo);
									ModManager.SaveModSettings(false);
									SteamManager.ShowFailedDialog("Fail to submit item update.");
									onFailed();
								}
								else
								{
									CallResult<SubmitItemUpdateResult_t> callResult = CallResult<SubmitItemUpdateResult_t>.Create(null);
									callResult.Set(submitCall, delegate(SubmitItemUpdateResult_t submitUpdateResult, bool failure)
									{
										bool result = SteamManager.HandleUploadModResult(submitUpdateResult.m_nPublishedFileId, submitUpdateResult.m_eResult, isCreate, submitUpdateResult.m_bUserNeedsToAcceptWorkshopLegalAgreement);
										bool flag15 = result;
										if (flag15)
										{
											onSucceed(updateHandle);
										}
										else
										{
											modInfo.ModId.FileId = originFileId;
											modInfo.ModId.Source = originSource;
											ModManager.SaveModInfo(modInfo);
											ModManager.SaveModSettings(false);
											onFailed();
										}
									});
								}
							}
						}
					}
				}
			}
		}
	}

	// Token: 0x06001135 RID: 4405 RVA: 0x00068364 File Offset: 0x00066564
	private static bool HandleUploadModResult(PublishedFileId_t fileId, EResult eResult, bool isCreate, bool needToAcceptAgreement)
	{
		bool flag = eResult != EResult.k_EResultOK;
		bool result;
		if (flag)
		{
			string pre = "LK_Mod_SubmitItemUpdateResult_";
			string resultName = eResult.ToString();
			string resultKey = pre + resultName;
			LanguageKey languageKey;
			string resultContent = (!Enum.TryParse<LanguageKey>(resultKey, out languageKey)) ? resultName : LocalStringManager.Get(resultKey);
			string operationName = LocalStringManager.Get(isCreate ? LanguageKey.LK_Steam_Operation_CreateItem : LanguageKey.LK_Steam_Operation_UpdateItem);
			string content = LocalStringManager.GetFormat(LanguageKey.LK_Steam_Fail_Content, operationName, resultContent);
			SteamManager.ShowFailedDialog(content);
			if (isCreate)
			{
				SteamManager.DeleteItem(fileId);
			}
			result = false;
		}
		else if (needToAcceptAgreement)
		{
			SteamManager.HandleAgreement(fileId);
			result = false;
		}
		else
		{
			result = true;
		}
		return result;
	}

	// Token: 0x06001136 RID: 4406 RVA: 0x0006840C File Offset: 0x0006660C
	private static bool HandleCreateModResult(PublishedFileId_t fileId, EResult eResult, bool needToAcceptAgreement)
	{
		bool flag = eResult != EResult.k_EResultOK;
		bool result;
		if (flag)
		{
			string pre = "LK_Mod_CreateItemResult_";
			string resultName = eResult.ToString();
			string resultKey = pre + resultName;
			LanguageKey languageKey;
			string resultContent = (!Enum.TryParse<LanguageKey>(resultKey, out languageKey)) ? resultName : LocalStringManager.Get(resultKey);
			string operationName = LocalStringManager.Get(LanguageKey.LK_Steam_Operation_CreateItem);
			string content = LocalStringManager.GetFormat(LanguageKey.LK_Steam_Fail_Content, operationName, resultContent);
			SteamManager.ShowFailedDialog(content);
			result = false;
		}
		else if (needToAcceptAgreement)
		{
			SteamManager.HandleAgreement(fileId);
			result = false;
		}
		else
		{
			result = true;
		}
		return result;
	}

	// Token: 0x06001137 RID: 4407 RVA: 0x0006849C File Offset: 0x0006669C
	private static void HandleAgreement(PublishedFileId_t fileId)
	{
		string title = LocalStringManager.Get(LanguageKey.LK_Steam_Need_Workshop_Agreement_Title);
		string content = LocalStringManager.Get(LanguageKey.LK_Steam_Need_Workshop_Agreement_Content);
		string pchURL = string.Format("steam://url/CommunityFilePage/{0}", fileId);
		CommonUtils.ShowConfirmDialog(title, content, delegate
		{
			SteamFriends.ActivateGameOverlayToWebPage(pchURL, EActivateGameOverlayToWebPageMode.k_EActivateGameOverlayToWebPageMode_Default);
		}, null, EDialogType.None);
	}

	// Token: 0x06001138 RID: 4408 RVA: 0x000684F4 File Offset: 0x000666F4
	private static void ShowFailedDialog(string content)
	{
		string title = LocalStringManager.Get(LanguageKey.LK_Steam_Fail_Title);
		CommonUtils.ShowDialog(title, content, null, EDialogType.None);
		Debug.LogWarning(content);
	}

	// Token: 0x06001139 RID: 4409 RVA: 0x0006851E File Offset: 0x0006671E
	public static void SubscribeItem(PublishedFileId_t fileId)
	{
		SteamUGC.SubscribeItem(fileId);
	}

	// Token: 0x0600113A RID: 4410 RVA: 0x00068528 File Offset: 0x00066728
	public static void UnSubscribeItem(PublishedFileId_t fileId)
	{
		SteamUGC.UnsubscribeItem(fileId);
	}

	// Token: 0x0600113B RID: 4411 RVA: 0x00068534 File Offset: 0x00066734
	public static void ReadSubscribedItems(List<ModId> modIds, Dictionary<string, ModInfoWithDisplayData> modInfoDict)
	{
		bool flag = !SteamManager.Initialized;
		if (!flag)
		{
			uint numSubscribedItems = SteamUGC.GetNumSubscribedItems();
			PublishedFileId_t[] publishedFileIds = new PublishedFileId_t[numSubscribedItems];
			SteamUGC.GetSubscribedItems(publishedFileIds, numSubscribedItems);
			uint i = 0U;
			while ((ulong)i < (ulong)((long)publishedFileIds.Length))
			{
				PublishedFileId_t fileId = publishedFileIds[(int)i];
				EItemState itemState = (EItemState)SteamUGC.GetItemState(fileId);
				ulong sizeOnDisc = 0UL;
				string folderPath = string.Empty;
				bool flag2 = SteamManager.IsItemStateActive(itemState, EItemState.k_EItemStateInstalled);
				if (flag2)
				{
					uint num;
					SteamUGC.GetItemInstallInfo(fileId, out sizeOnDisc, out folderPath, 400U, out num);
					bool flag3 = string.IsNullOrEmpty(folderPath);
					if (!flag3)
					{
						string configPath = Path.Combine(folderPath, "Config.Lua");
						string modSettingsPath = Path.Combine(folderPath, "Settings.Lua");
						ModInfoWithDisplayData modInfo = ModManager.ReadModInfo(configPath, modSettingsPath, true, false);
						bool flag4 = modInfo != null;
						if (flag4)
						{
							modInfo.ModId.FileId = fileId.m_PublishedFileId;
							int index = modIds.FindIndex((ModId m) => m.FileId == modInfo.ModId.FileId);
							bool flag5 = index >= 0;
							if (flag5)
							{
								modIds[index] = modInfo.ModId;
							}
							else
							{
								modIds.Add(modInfo.ModId);
							}
							modInfoDict[modInfo.ModId.ToString()] = modInfo;
							modInfo.DirectoryName = folderPath;
							modInfo.IsSubscribed = true;
						}
					}
				}
				else
				{
					bool flag6 = SteamManager.IsItemStateActive(itemState, EItemState.k_EItemStateSubscribed);
					if (flag6)
					{
						SteamUGC.DownloadItem(fileId, true);
					}
				}
				IL_18F:
				i += 1U;
				continue;
				goto IL_18F;
			}
		}
	}

	// Token: 0x0600113C RID: 4412 RVA: 0x000686E8 File Offset: 0x000668E8
	public static bool CheckModHasChangeGameConfig(ModId modId)
	{
		PublishedFileId_t fileId = new PublishedFileId_t(modId.FileId);
		EItemState itemState = (EItemState)SteamUGC.GetItemState(fileId);
		bool flag = SteamManager.IsItemStateActive(itemState, EItemState.k_EItemStateInstalled);
		if (flag)
		{
			ulong num;
			string folderPath;
			uint num2;
			SteamUGC.GetItemInstallInfo(fileId, out num, out folderPath, 400U, out num2);
			bool flag2 = string.IsNullOrEmpty(folderPath);
			if (flag2)
			{
				return false;
			}
			string configPath = Path.Combine(folderPath, "Config");
			bool flag3 = Directory.Exists(configPath);
			if (flag3)
			{
				string[] files = Directory.GetFiles(configPath);
				bool flag4 = files.Length != 0;
				if (flag4)
				{
					return true;
				}
			}
		}
		return false;
	}

	// Token: 0x0600113D RID: 4413 RVA: 0x00068778 File Offset: 0x00066978
	public static void GetUserItemVote(PublishedFileId_t fileId)
	{
		SteamAPICall_t apiCall = SteamUGC.GetUserItemVote(fileId);
		CallResult<GetUserItemVoteResult_t> callResult = CallResult<GetUserItemVoteResult_t>.Create(null);
	}

	// Token: 0x0600113E RID: 4414 RVA: 0x00068794 File Offset: 0x00066994
	public static bool IsItemStateActive(EItemState currItemState, EItemState checkingState)
	{
		return checkingState == (currItemState & checkingState);
	}

	// Token: 0x0600113F RID: 4415 RVA: 0x000687AC File Offset: 0x000669AC
	public static bool IsItemStateActive(PublishedFileId_t fileId, EItemState checkingState)
	{
		EItemState currItemState = (EItemState)SteamUGC.GetItemState(fileId);
		return SteamManager.IsItemStateActive(currItemState, checkingState);
	}

	// Token: 0x06001140 RID: 4416 RVA: 0x000687CC File Offset: 0x000669CC
	public static bool IsItemStateActive(ulong fileId, EItemState checkingState)
	{
		EItemState currItemState = (EItemState)SteamUGC.GetItemState(new PublishedFileId_t(fileId));
		return SteamManager.IsItemStateActive(currItemState, checkingState);
	}

	// Token: 0x06001141 RID: 4417 RVA: 0x000687F4 File Offset: 0x000669F4
	public static void LoadSubscribedItems()
	{
		uint numSubscribedItems = SteamUGC.GetNumSubscribedItems();
		PublishedFileId_t[] publishedFileIds = new PublishedFileId_t[numSubscribedItems];
		SteamUGC.GetSubscribedItems(publishedFileIds, numSubscribedItems);
		for (int i = 0; i < publishedFileIds.Length; i++)
		{
		}
	}

	// Token: 0x06001142 RID: 4418 RVA: 0x0006882B File Offset: 0x00066A2B
	public static void DeleteItem(PublishedFileId_t fileId)
	{
		SteamUGC.DeleteItem(fileId);
	}

	// Token: 0x06001143 RID: 4419 RVA: 0x00068838 File Offset: 0x00066A38
	public static void DownloadPreviewCoverImage(ModInfoWithDisplayData modInfo)
	{
		SteamManager.<>c__DisplayClass63_0 CS$<>8__locals1 = new SteamManager.<>c__DisplayClass63_0();
		CS$<>8__locals1.modInfo = modInfo;
		CS$<>8__locals1.previewFileHandle = CS$<>8__locals1.modInfo.PreviewFileHandle;
		bool hasModCoverTextureCache = ModManager.HasPreviewModCoverTexture(CS$<>8__locals1.modInfo.ModId);
		bool flag = hasModCoverTextureCache;
		if (flag)
		{
			ArgumentBox argumentBox = EasyPool.Get<ArgumentBox>().SetObject("ModInfo", CS$<>8__locals1.modInfo);
			GEvent.OnEvent(UiEvents.WorkshopModPreviewImageHasDownloaded, argumentBox);
		}
		else
		{
			bool flag2 = CS$<>8__locals1.previewFileHandle != UGCHandle_t.Invalid;
			if (flag2)
			{
				SteamAPICall_t steamAPICall = SteamRemoteStorage.UGCDownload(CS$<>8__locals1.previewFileHandle, 0U);
				CallResult<RemoteStorageDownloadUGCResult_t> callResult = CallResult<RemoteStorageDownloadUGCResult_t>.Create(null);
				callResult.Set(steamAPICall, delegate(RemoteStorageDownloadUGCResult_t t, bool failure)
				{
					SteamManager.<>c__DisplayClass63_1 CS$<>8__locals2 = new SteamManager.<>c__DisplayClass63_1();
					CS$<>8__locals2.CS$<>8__locals1 = CS$<>8__locals1;
					CS$<>8__locals2.t = t;
					CS$<>8__locals2.failure = failure;
					SteamManager.OnSteamCallResult(new Action(CS$<>8__locals2.<DownloadPreviewCoverImage>g__OnExecute|2), new Action<bool>(CS$<>8__locals2.<DownloadPreviewCoverImage>g__OnFailed|1));
				});
			}
		}
	}

	// Token: 0x06001144 RID: 4420 RVA: 0x000688EC File Offset: 0x00066AEC
	public static void Clear()
	{
		foreach (KeyValuePair<ModSubPageShop.WorkshopTimeToggleKey, List<ModId>> pair in SteamManager.QueryWorkshopModByTimeDict)
		{
			List<ModId> value = pair.Value;
			if (value != null)
			{
				value.Clear();
			}
		}
	}

	// Token: 0x06001145 RID: 4421 RVA: 0x00068950 File Offset: 0x00066B50
	public static void InitString()
	{
		SteamManager.AllTagList.Clear();
		SteamManager.AllTagList.Add(LocalStringManager.Get(LanguageKey.LK_Mod_Tag_Modifications));
		SteamManager.AllTagList.Add(LocalStringManager.Get(LanguageKey.LK_Mod_Tag_Stories));
		SteamManager.AllTagList.Add(LocalStringManager.Get(LanguageKey.LK_Mod_Tag_Frameworks));
		SteamManager.AllTagList.Add(LocalStringManager.Get(LanguageKey.LK_Mod_Tag_Arts));
		SteamManager.AllTagList.Add(LocalStringManager.Get(LanguageKey.LK_Mod_Tag_Optimizations));
		SteamManager.AllTagList.Add(LocalStringManager.Get(LanguageKey.LK_Mod_Tag_Display));
		SteamManager.AllTagList.Add(LocalStringManager.Get(LanguageKey.LK_Mod_Tag_Extensions));
		SteamManager.AllTagList.Add(LocalStringManager.Get(LanguageKey.LK_Mod_Tag_Configurations));
		SteamManager.AllTagList.Add(LocalStringManager.Get(LanguageKey.LK_Mod_Tag_Compatible));
		SteamManager.VisibilityOptionList.Clear();
		SteamManager.VisibilityOptionList.Add(LocalStringManager.Get(LanguageKey.LK_Visibility_Public));
		SteamManager.VisibilityOptionList.Add(LocalStringManager.Get(LanguageKey.LK_Visibility_FriendsOnly));
		SteamManager.VisibilityOptionList.Add(LocalStringManager.Get(LanguageKey.LK_Visibility_Private));
		SteamManager.VisibilityOptionList.Add(LocalStringManager.Get(LanguageKey.LK_Visibility_Unlisted));
	}

	// Token: 0x06001146 RID: 4422 RVA: 0x00068A88 File Offset: 0x00066C88
	public static List<string> GetTagContentList(List<string> tagArray)
	{
		List<string> result = new List<string>();
		foreach (string tag in tagArray)
		{
			string content = SteamManager.GetTagContent(tag);
			result.Add(content);
		}
		return result;
	}

	// Token: 0x06001147 RID: 4423 RVA: 0x00068AF0 File Offset: 0x00066CF0
	public static string GetTagContent(string tag)
	{
		string content = string.Empty;
		bool flag = SteamManager.SteamTagKeyDic[SteamManager.ESteamTag.Modifications].Contains(tag);
		if (flag)
		{
			content = LocalStringManager.Get(LanguageKey.LK_Mod_Tag_Modifications);
		}
		else
		{
			bool flag2 = SteamManager.SteamTagKeyDic[SteamManager.ESteamTag.Stories].Contains(tag);
			if (flag2)
			{
				content = LocalStringManager.Get(LanguageKey.LK_Mod_Tag_Stories);
			}
			else
			{
				bool flag3 = SteamManager.SteamTagKeyDic[SteamManager.ESteamTag.Frameworks].Contains(tag);
				if (flag3)
				{
					content = LocalStringManager.Get(LanguageKey.LK_Mod_Tag_Frameworks);
				}
				else
				{
					bool flag4 = SteamManager.SteamTagKeyDic[SteamManager.ESteamTag.Arts].Contains(tag);
					if (flag4)
					{
						content = LocalStringManager.Get(LanguageKey.LK_Mod_Tag_Arts);
					}
					else
					{
						bool flag5 = SteamManager.SteamTagKeyDic[SteamManager.ESteamTag.Optimizations].Contains(tag);
						if (flag5)
						{
							content = LocalStringManager.Get(LanguageKey.LK_Mod_Tag_Optimizations);
						}
						else
						{
							bool flag6 = SteamManager.SteamTagKeyDic[SteamManager.ESteamTag.Display].Contains(tag);
							if (flag6)
							{
								content = LocalStringManager.Get(LanguageKey.LK_Mod_Tag_Display);
							}
							else
							{
								bool flag7 = SteamManager.SteamTagKeyDic[SteamManager.ESteamTag.Extensions].Contains(tag);
								if (flag7)
								{
									content = LocalStringManager.Get(LanguageKey.LK_Mod_Tag_Extensions);
								}
								else
								{
									bool flag8 = SteamManager.SteamTagKeyDic[SteamManager.ESteamTag.Configurations].Contains(tag);
									if (flag8)
									{
										content = LocalStringManager.Get(LanguageKey.LK_Mod_Tag_Configurations);
									}
									else
									{
										bool flag9 = SteamManager.SteamTagKeyDic[SteamManager.ESteamTag.Compatible].Contains(tag);
										if (flag9)
										{
											content = LocalStringManager.Get(LanguageKey.LK_Mod_Tag_Compatible);
										}
									}
								}
							}
						}
					}
				}
			}
		}
		return content;
	}

	// Token: 0x06001148 RID: 4424 RVA: 0x00068C58 File Offset: 0x00066E58
	public static int GetTagMask(List<string> tagArray)
	{
		int result = 0;
		foreach (string tag in tagArray)
		{
			bool flag = SteamManager.SteamTagKeyDic[SteamManager.ESteamTag.Modifications].Contains(tag);
			if (flag)
			{
				result |= 1;
			}
			else
			{
				bool flag2 = SteamManager.SteamTagKeyDic[SteamManager.ESteamTag.Stories].Contains(tag);
				if (flag2)
				{
					result |= 2;
				}
				else
				{
					bool flag3 = SteamManager.SteamTagKeyDic[SteamManager.ESteamTag.Frameworks].Contains(tag);
					if (flag3)
					{
						result |= 4;
					}
					else
					{
						bool flag4 = SteamManager.SteamTagKeyDic[SteamManager.ESteamTag.Arts].Contains(tag);
						if (flag4)
						{
							result |= 8;
						}
						else
						{
							bool flag5 = SteamManager.SteamTagKeyDic[SteamManager.ESteamTag.Optimizations].Contains(tag);
							if (flag5)
							{
								result |= 16;
							}
							else
							{
								bool flag6 = SteamManager.SteamTagKeyDic[SteamManager.ESteamTag.Display].Contains(tag);
								if (flag6)
								{
									result |= 32;
								}
								else
								{
									bool flag7 = SteamManager.SteamTagKeyDic[SteamManager.ESteamTag.Extensions].Contains(tag);
									if (flag7)
									{
										result |= 64;
									}
									else
									{
										bool flag8 = SteamManager.SteamTagKeyDic[SteamManager.ESteamTag.Configurations].Contains(tag);
										if (flag8)
										{
											result |= 128;
										}
										else
										{
											bool flag9 = SteamManager.SteamTagKeyDic[SteamManager.ESteamTag.Compatible].Contains(tag);
											if (flag9)
											{
												result |= 256;
											}
										}
									}
								}
							}
						}
					}
				}
			}
		}
		return result;
	}

	// Token: 0x06001149 RID: 4425 RVA: 0x00068DD8 File Offset: 0x00066FD8
	public static List<string> GetTagKeyList(List<string> tagArray)
	{
		List<string> result = new List<string>();
		foreach (string tag in tagArray)
		{
			string key = SteamManager.GetTagKey(tag);
			result.Add(key);
		}
		return result;
	}

	// Token: 0x0600114A RID: 4426 RVA: 0x00068E40 File Offset: 0x00067040
	public static string GetTagKey(string tagContent)
	{
		string key = string.Empty;
		bool flag = tagContent == LocalStringManager.Get(LanguageKey.LK_Mod_Tag_Modifications);
		if (flag)
		{
			key = SteamManager.SteamTagKeyDic[SteamManager.ESteamTag.Modifications];
		}
		else
		{
			bool flag2 = tagContent == LocalStringManager.Get(LanguageKey.LK_Mod_Tag_Stories);
			if (flag2)
			{
				key = SteamManager.SteamTagKeyDic[SteamManager.ESteamTag.Stories];
			}
			else
			{
				bool flag3 = tagContent == LocalStringManager.Get(LanguageKey.LK_Mod_Tag_Frameworks);
				if (flag3)
				{
					key = SteamManager.SteamTagKeyDic[SteamManager.ESteamTag.Frameworks];
				}
				else
				{
					bool flag4 = tagContent == LocalStringManager.Get(LanguageKey.LK_Mod_Tag_Arts);
					if (flag4)
					{
						key = SteamManager.SteamTagKeyDic[SteamManager.ESteamTag.Arts];
					}
					else
					{
						bool flag5 = tagContent == LocalStringManager.Get(LanguageKey.LK_Mod_Tag_Optimizations);
						if (flag5)
						{
							key = SteamManager.SteamTagKeyDic[SteamManager.ESteamTag.Optimizations];
						}
						else
						{
							bool flag6 = tagContent == LocalStringManager.Get(LanguageKey.LK_Mod_Tag_Display);
							if (flag6)
							{
								key = SteamManager.SteamTagKeyDic[SteamManager.ESteamTag.Display];
							}
							else
							{
								bool flag7 = tagContent == LocalStringManager.Get(LanguageKey.LK_Mod_Tag_Extensions);
								if (flag7)
								{
									key = SteamManager.SteamTagKeyDic[SteamManager.ESteamTag.Extensions];
								}
								else
								{
									bool flag8 = tagContent == LocalStringManager.Get(LanguageKey.LK_Mod_Tag_Configurations);
									if (flag8)
									{
										key = SteamManager.SteamTagKeyDic[SteamManager.ESteamTag.Configurations];
									}
									else
									{
										bool flag9 = tagContent == LocalStringManager.Get(LanguageKey.LK_Mod_Tag_Compatible);
										if (flag9)
										{
											key = SteamManager.SteamTagKeyDic[SteamManager.ESteamTag.Compatible];
										}
									}
								}
							}
						}
					}
				}
			}
		}
		return key;
	}

	// Token: 0x0600114B RID: 4427 RVA: 0x00068FA8 File Offset: 0x000671A8
	public static void ShowSteamOperationResult(LanguageKey operationNameKey, EResult eResult, bool hasException = false)
	{
		string resultName = eResult.ToString();
		string resultKey = "LK_Mod_" + resultName;
		LanguageKey languageKey;
		string resultContent = hasException ? LocalStringManager.Get(LanguageKey.LK_Mod_Result_Error) : ((!Enum.TryParse<LanguageKey>(resultKey, out languageKey)) ? resultName : LocalStringManager.Get(resultKey));
		string operationName = LocalStringManager.Get(operationNameKey);
		string content = LocalStringManager.GetFormat(LanguageKey.LK_Steam_Fail_Content, operationName, resultContent);
		SteamManager.ShowFailedDialog(content);
	}

	// Token: 0x0600114C RID: 4428 RVA: 0x00069010 File Offset: 0x00067210
	private static void OnSteamCallResult(Action action, Action<bool> failed)
	{
		try
		{
			action();
		}
		catch (Exception e)
		{
			GLog.Warn(e, Array.Empty<object>());
			failed(true);
			throw;
		}
	}

	// Token: 0x04000F05 RID: 3845
	private static readonly AppId_t _appId = new AppId_t(838350U);

	// Token: 0x04000F09 RID: 3849
	private static CSteamID _steamId;

	// Token: 0x04000F0A RID: 3850
	private static SteamAPIWarningMessageHook_t _steamApiWarningMessageHook;

	// Token: 0x04000F0B RID: 3851
	private static UGCQueryHandle_t _ugcQueryHandle;

	// Token: 0x04000F0C RID: 3852
	private static readonly Dictionary<ModSubPageShop.WorkshopTimeToggleKey, List<ModId>> QueryWorkshopModByTimeDict = new Dictionary<ModSubPageShop.WorkshopTimeToggleKey, List<ModId>>();

	// Token: 0x04000F0D RID: 3853
	public static readonly List<string> AllTagList = new List<string>();

	// Token: 0x04000F0E RID: 3854
	public static readonly List<string> VisibilityOptionList = new List<string>();

	// Token: 0x04000F0F RID: 3855
	public static readonly Dictionary<SteamManager.ESteamTag, LanguageKey> SteamTagLanguageKeyDic = new Dictionary<SteamManager.ESteamTag, LanguageKey>
	{
		{
			SteamManager.ESteamTag.Modifications,
			LanguageKey.LK_Mod_Tag_Modifications
		},
		{
			SteamManager.ESteamTag.Stories,
			LanguageKey.LK_Mod_Tag_Stories
		},
		{
			SteamManager.ESteamTag.Frameworks,
			LanguageKey.LK_Mod_Tag_Frameworks
		},
		{
			SteamManager.ESteamTag.Arts,
			LanguageKey.LK_Mod_Tag_Arts
		},
		{
			SteamManager.ESteamTag.Optimizations,
			LanguageKey.LK_Mod_Tag_Optimizations
		},
		{
			SteamManager.ESteamTag.Display,
			LanguageKey.LK_Mod_Tag_Display
		},
		{
			SteamManager.ESteamTag.Extensions,
			LanguageKey.LK_Mod_Tag_Extensions
		},
		{
			SteamManager.ESteamTag.Configurations,
			LanguageKey.LK_Mod_Tag_Configurations
		},
		{
			SteamManager.ESteamTag.Compatible,
			LanguageKey.LK_Mod_Tag_Compatible
		}
	};

	// Token: 0x04000F10 RID: 3856
	public static readonly Dictionary<SteamManager.ESteamTag, string> SteamTagKeyDic = new Dictionary<SteamManager.ESteamTag, string>
	{
		{
			SteamManager.ESteamTag.Modifications,
			"Modifications"
		},
		{
			SteamManager.ESteamTag.Stories,
			"Stories"
		},
		{
			SteamManager.ESteamTag.Frameworks,
			"Frameworks"
		},
		{
			SteamManager.ESteamTag.Arts,
			"Arts"
		},
		{
			SteamManager.ESteamTag.Optimizations,
			"Optimizations"
		},
		{
			SteamManager.ESteamTag.Display,
			"Display"
		},
		{
			SteamManager.ESteamTag.Extensions,
			"Extensions"
		},
		{
			SteamManager.ESteamTag.Configurations,
			"Configurations"
		},
		{
			SteamManager.ESteamTag.Compatible,
			"Compatible Mods"
		}
	};

	// Token: 0x04000F11 RID: 3857
	public static bool IsEditMod = true;

	// Token: 0x04000F12 RID: 3858
	public const int PathLength = 400;

	// Token: 0x02001201 RID: 4609
	public enum ESteamTag
	{
		// Token: 0x04009932 RID: 39218
		Modifications,
		// Token: 0x04009933 RID: 39219
		Stories,
		// Token: 0x04009934 RID: 39220
		Frameworks,
		// Token: 0x04009935 RID: 39221
		Arts,
		// Token: 0x04009936 RID: 39222
		Optimizations,
		// Token: 0x04009937 RID: 39223
		Display,
		// Token: 0x04009938 RID: 39224
		Extensions,
		// Token: 0x04009939 RID: 39225
		Configurations,
		// Token: 0x0400993A RID: 39226
		Compatible
	}
}
