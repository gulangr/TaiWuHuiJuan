using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using FrameWork;
using GameData.Adventure.Editor;
using GameData.Utilities;
using UnityEngine;

// Token: 0x0200017C RID: 380
public static class AdventureEditorKit
{
	// Token: 0x1700025B RID: 603
	// (get) Token: 0x0600150B RID: 5387 RVA: 0x000825C5 File Offset: 0x000807C5
	public static bool GetControlKey
	{
		get
		{
			return AdventureEditorKit.GetControlKeyDirect;
		}
	}

	// Token: 0x1700025C RID: 604
	// (get) Token: 0x0600150C RID: 5388 RVA: 0x000825CC File Offset: 0x000807CC
	public static bool GetControlKeyDirect
	{
		get
		{
			return Input.GetKey(KeyCode.LeftControl) || Input.GetKey(KeyCode.RightControl);
		}
	}

	// Token: 0x1700025D RID: 605
	// (get) Token: 0x0600150D RID: 5389 RVA: 0x000825E7 File Offset: 0x000807E7
	public static bool GetShiftKey
	{
		get
		{
			return Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift);
		}
	}

	// Token: 0x1700025E RID: 606
	// (get) Token: 0x0600150E RID: 5390 RVA: 0x00082602 File Offset: 0x00080802
	public static string CorePath
	{
		get
		{
			return PlayerPrefs.GetString("ConchShip_PresetKey_AdventureCorePath");
		}
	}

	// Token: 0x1700025F RID: 607
	// (get) Token: 0x0600150F RID: 5391 RVA: 0x0008260E File Offset: 0x0008080E
	public static string CoreRoot
	{
		get
		{
			return AdventureEditorKit.GetCoreRoot();
		}
	}

	// Token: 0x17000260 RID: 608
	// (get) Token: 0x06001510 RID: 5392 RVA: 0x00082615 File Offset: 0x00080815
	public static string CoreDirectory
	{
		get
		{
			return AdventureEditorKit.GetCoreDirectory();
		}
	}

	// Token: 0x17000261 RID: 609
	// (get) Token: 0x06001511 RID: 5393 RVA: 0x0008261C File Offset: 0x0008081C
	public static string ElementDirectory
	{
		get
		{
			return AdventureEditorKit.GetElementDirectory();
		}
	}

	// Token: 0x17000262 RID: 610
	// (get) Token: 0x06001512 RID: 5394 RVA: 0x00082623 File Offset: 0x00080823
	public static string MajorEventDirectory
	{
		get
		{
			return AdventureEditorKit.GetMajorEventDirectory();
		}
	}

	// Token: 0x06001513 RID: 5395 RVA: 0x0008262C File Offset: 0x0008082C
	public static bool CheckCorePath()
	{
		string corePath = AdventureEditorKit.CorePath;
		bool invalid = string.IsNullOrEmpty(corePath) || !File.Exists(corePath);
		bool flag = invalid;
		if (flag)
		{
			corePath = AdventureEditorKit.ResetCorePath();
		}
		invalid = (string.IsNullOrEmpty(corePath) || !File.Exists(corePath));
		bool flag2 = invalid;
		if (flag2)
		{
			AdaptableLog.Error("Adventure core not found, some features will not work.");
		}
		return !invalid;
	}

	// Token: 0x06001514 RID: 5396 RVA: 0x00082690 File Offset: 0x00080890
	public static string ResetCorePath()
	{
		string corePath = LocalDialog.SelectLoadFilePath("Adventure Core Files(*.advcore)\0*.advcore\0", Application.persistentDataPath);
		bool flag = string.IsNullOrEmpty(corePath) || !File.Exists(corePath);
		if (flag)
		{
			corePath = string.Empty;
		}
		else
		{
			PlayerPrefs.SetString("ConchShip_PresetKey_AdventureCorePath", corePath);
			bool flag2 = !Directory.Exists(AdventureEditorKit.CoreDirectory);
			if (flag2)
			{
				Directory.CreateDirectory(AdventureEditorKit.CoreDirectory);
			}
			bool flag3 = !Directory.Exists(AdventureEditorKit.ElementDirectory);
			if (flag3)
			{
				Directory.CreateDirectory(AdventureEditorKit.ElementDirectory);
			}
		}
		return corePath;
	}

	// Token: 0x06001515 RID: 5397 RVA: 0x0008271C File Offset: 0x0008091C
	public static int CreateNewElement(string path)
	{
		AdventureElementSnapshot data = new AdventureElementSnapshot();
		int id = data.Id = AdventureEditorKit.GetNewElementId();
		AdventureElementSnapshot.SaveToFile(path, data);
		return id;
	}

	// Token: 0x06001516 RID: 5398 RVA: 0x0008274C File Offset: 0x0008094C
	public static string GetNewElementFilePath(string rootPath)
	{
		string fileName = LocalStringManager.Get(LanguageKey.LK_AdventureEditor_NewElement) + ".adve";
		return AdventureEditorKit.GetNewFilePath(rootPath, fileName);
	}

	// Token: 0x06001517 RID: 5399 RVA: 0x0008277C File Offset: 0x0008097C
	public static string GetNewElementDirectoryPath(string rootPath)
	{
		string directoryName = LocalStringManager.Get(LanguageKey.LK_AdventureEditor_NewDirectory);
		return AdventureEditorKit.GetNewDirectoryPath(rootPath, directoryName);
	}

	// Token: 0x06001518 RID: 5400 RVA: 0x000827A0 File Offset: 0x000809A0
	public static IEnumerable<AdventureSnapshot> GetAdventures()
	{
		bool flag = string.IsNullOrEmpty(AdventureEditorKit.CoreDirectory);
		if (flag)
		{
			yield break;
		}
		DirectoryInfo directory = new DirectoryInfo(AdventureEditorKit.CoreDirectory);
		foreach (FileInfo file in directory.EnumerateFiles("*.advbp", SearchOption.AllDirectories))
		{
			AdventureSnapshot data;
			bool flag2 = AdventureSnapshot.TryLoadFromFile(file.FullName, out data);
			if (flag2)
			{
				yield return data;
			}
			data = null;
			file = null;
		}
		IEnumerator<FileInfo> enumerator = null;
		yield break;
		yield break;
	}

	// Token: 0x06001519 RID: 5401 RVA: 0x000827A9 File Offset: 0x000809A9
	public static IEnumerable<AdventureMajorEventSnapshot> GetMajorEvents()
	{
		bool flag = string.IsNullOrEmpty(AdventureEditorKit.MajorEventDirectory);
		if (flag)
		{
			yield break;
		}
		DirectoryInfo directory = new DirectoryInfo(AdventureEditorKit.MajorEventDirectory);
		foreach (FileInfo file in directory.EnumerateFiles("*.advme", SearchOption.AllDirectories))
		{
			AdventureMajorEventSnapshot data;
			bool flag2 = AdventureMajorEventSnapshot.TryLoadFromFile(file.FullName, out data);
			if (flag2)
			{
				yield return data;
			}
			data = null;
			file = null;
		}
		IEnumerator<FileInfo> enumerator = null;
		yield break;
		yield break;
	}

	// Token: 0x0600151A RID: 5402 RVA: 0x000827B2 File Offset: 0x000809B2
	public static void DisableUpdateElementCache()
	{
		AdventureEditorKit._ignoreUpdateCache = true;
	}

	// Token: 0x0600151B RID: 5403 RVA: 0x000827BB File Offset: 0x000809BB
	public static void EnableUpdateElementCache()
	{
		AdventureEditorKit._ignoreUpdateCache = false;
	}

	// Token: 0x0600151C RID: 5404 RVA: 0x000827C4 File Offset: 0x000809C4
	public static void UpdateElementCache()
	{
		bool flag = string.IsNullOrEmpty(AdventureEditorKit.ElementDirectory) || AdventureEditorKit._ignoreUpdateCache;
		if (!flag)
		{
			if (AdventureEditorKit._elementCache == null)
			{
				AdventureEditorKit._elementCache = new Dictionary<int, ValueTuple<AdventureElementSnapshot, DateTime, string>>();
			}
			HashSet<string> loadedCache = EasyPool.Get<HashSet<string>>();
			List<int> invalidKeys = EasyPool.Get<List<int>>();
			foreach (KeyValuePair<int, ValueTuple<AdventureElementSnapshot, DateTime, string>> keyValuePair in AdventureEditorKit._elementCache)
			{
				int num;
				ValueTuple<AdventureElementSnapshot, DateTime, string> valueTuple;
				keyValuePair.Deconstruct(out num, out valueTuple);
				int id = num;
				ValueTuple<AdventureElementSnapshot, DateTime, string> tuple = valueTuple;
				bool flag2 = File.Exists(tuple.Item3) && File.GetLastWriteTime(tuple.Item3) == tuple.Item2;
				if (flag2)
				{
					loadedCache.Add(tuple.Item3);
				}
				else
				{
					invalidKeys.Add(id);
				}
			}
			foreach (int invalidKey in invalidKeys)
			{
				AdventureEditorKit._elementCache.Remove(invalidKey);
			}
			EasyPool.Free<List<int>>(invalidKeys);
			foreach (string path in Directory.EnumerateFiles(AdventureEditorKit.ElementDirectory, "*.adve", SearchOption.AllDirectories))
			{
				AdventureElementSnapshot data;
				bool flag3 = !loadedCache.Contains(path) && AdventureElementSnapshot.TryLoadFromFile(path, out data);
				if (flag3)
				{
					AdventureEditorKit._elementCache[data.Id] = new ValueTuple<AdventureElementSnapshot, DateTime, string>(data, File.GetLastWriteTime(path), path);
				}
			}
			EasyPool.Free<HashSet<string>>(loadedCache);
		}
	}

	// Token: 0x0600151D RID: 5405 RVA: 0x00082988 File Offset: 0x00080B88
	public static AdventureElementSnapshot GetElementFromCache(string elementDefinition)
	{
		bool flag = AdventureEditorKit._elementCache == null || string.IsNullOrEmpty(elementDefinition);
		AdventureElementSnapshot result;
		if (flag)
		{
			result = null;
		}
		else
		{
			AdventureElementSnapshot otherwiseData = null;
			foreach (ValueTuple<AdventureElementSnapshot, DateTime, string> value in AdventureEditorKit._elementCache.Values)
			{
				bool flag2 = value.Item1.Definition == elementDefinition;
				if (flag2)
				{
					return value.Item1;
				}
				bool flag3 = value.Item1.Name == elementDefinition;
				if (flag3)
				{
					otherwiseData = value.Item1;
				}
			}
			result = otherwiseData;
		}
		return result;
	}

	// Token: 0x0600151E RID: 5406 RVA: 0x00082A48 File Offset: 0x00080C48
	public static AdventureElementSnapshot GetElementFromCache(int elementId)
	{
		bool flag = AdventureEditorKit._elementCache == null;
		AdventureElementSnapshot result;
		if (flag)
		{
			result = null;
		}
		else
		{
			ValueTuple<AdventureElementSnapshot, DateTime, string> cache;
			bool flag2 = AdventureEditorKit._elementCache.TryGetValue(elementId, out cache) && File.Exists(cache.Item3) && File.GetLastWriteTime(cache.Item3) == cache.Item2;
			if (flag2)
			{
				result = cache.Item1;
			}
			else
			{
				result = null;
			}
		}
		return result;
	}

	// Token: 0x0600151F RID: 5407 RVA: 0x00082AB0 File Offset: 0x00080CB0
	public static string GetElementPathFromCache(int elementId)
	{
		bool flag = AdventureEditorKit._elementCache == null;
		string result;
		if (flag)
		{
			result = null;
		}
		else
		{
			ValueTuple<AdventureElementSnapshot, DateTime, string> cache;
			bool flag2 = AdventureEditorKit._elementCache.TryGetValue(elementId, out cache) && File.Exists(cache.Item3);
			if (flag2)
			{
				result = cache.Item3;
			}
			else
			{
				result = null;
			}
		}
		return result;
	}

	// Token: 0x06001520 RID: 5408 RVA: 0x00082B00 File Offset: 0x00080D00
	public static bool TryGetElementInAnywhere(int elementId, out AdventureElementSnapshot data)
	{
		data = null;
		bool flag = elementId <= 0;
		bool result;
		if (flag)
		{
			result = false;
		}
		else
		{
			Dictionary<int, ValueTuple<AdventureElementSnapshot, DateTime, string>> elementCache = AdventureEditorKit._elementCache;
			bool cacheUpdated = elementCache == null || elementCache.Count <= 0;
			bool flag2 = cacheUpdated;
			if (flag2)
			{
				AdventureEditorKit.UpdateElementCache();
			}
			data = AdventureEditorKit.GetElementFromCache(elementId);
			bool flag3 = cacheUpdated || data != null;
			if (flag3)
			{
				result = (data != null);
			}
			else
			{
				AdventureEditorKit.UpdateElementCache();
				data = AdventureEditorKit.GetElementFromCache(elementId);
				result = (data != null);
			}
		}
		return result;
	}

	// Token: 0x06001521 RID: 5409 RVA: 0x00082B7E File Offset: 0x00080D7E
	[return: TupleElementNames(new string[]
	{
		"Id",
		"path"
	})]
	public static IEnumerable<ValueTuple<int, string>> GetElementIds()
	{
		bool flag = string.IsNullOrEmpty(AdventureEditorKit.ElementDirectory);
		if (flag)
		{
			yield break;
		}
		DirectoryInfo directory = new DirectoryInfo(AdventureEditorKit.ElementDirectory);
		foreach (FileInfo file in directory.EnumerateFiles("*.adve", SearchOption.AllDirectories))
		{
			int id;
			bool flag2 = AdventureElementSnapshot.LoadHeaderFromFile(file.FullName, out id);
			if (flag2)
			{
				yield return new ValueTuple<int, string>(id, file.FullName);
			}
			file = null;
		}
		IEnumerator<FileInfo> enumerator = null;
		yield break;
		yield break;
	}

	// Token: 0x06001522 RID: 5410 RVA: 0x00082B88 File Offset: 0x00080D88
	public static void SetAdventureElementIcon(CImage image, string iconName)
	{
		image.SetSprite(iconName, false, delegate
		{
			bool flag = image.sprite == null;
			if (flag)
			{
				image.SetSprite("adventure-editor-back-element", false, null);
			}
		});
	}

	// Token: 0x06001523 RID: 5411 RVA: 0x00082BBD File Offset: 0x00080DBD
	private static int GetMinLinearId()
	{
		return 1000000000;
	}

	// Token: 0x06001524 RID: 5412 RVA: 0x00082BC4 File Offset: 0x00080DC4
	private static int GetTimeStampId()
	{
		DateTime now = DateTime.Now;
		int ts = now.DayOfYear * ((now.Hour * 60 + now.Minute) * 60 + now.Second) / 2;
		int tsId = ts * 100 + Random.Range(0, 100);
		bool flag = tsId <= 0;
		if (flag)
		{
			tsId = 1;
		}
		return tsId;
	}

	// Token: 0x06001525 RID: 5413 RVA: 0x00082C24 File Offset: 0x00080E24
	public static int GetNewBlueprintId()
	{
		bool flag = string.IsNullOrEmpty(AdventureEditorKit.CoreDirectory);
		int result;
		if (flag)
		{
			result = -1;
		}
		else
		{
			DirectoryInfo directory = new DirectoryInfo(AdventureEditorKit.CoreDirectory);
			HashSet<int> existIds = new HashSet<int>();
			foreach (FileInfo file in directory.EnumerateFiles("*.advbp", SearchOption.AllDirectories))
			{
				int id;
				bool flag2 = AdventureSnapshot.LoadHeaderFromFile(file.FullName, out id);
				if (flag2)
				{
					existIds.Add(id);
				}
			}
			foreach (FileInfo file2 in directory.EnumerateFiles("*.advme", SearchOption.AllDirectories))
			{
				int id2;
				bool flag3 = AdventureMajorEventSnapshot.LoadHeaderFromFile(file2.FullName, out id2);
				if (flag3)
				{
					existIds.Add(id2);
				}
			}
			int maxId = 1;
			int i = 0;
			while (i < 10 && existIds.Contains(maxId))
			{
				maxId = AdventureEditorKit.GetTimeStampId();
				i++;
			}
			bool flag4 = existIds.Contains(maxId);
			if (flag4)
			{
				maxId = existIds.Max() + 1;
				bool flag5 = maxId < AdventureEditorKit.GetMinLinearId();
				if (flag5)
				{
					maxId = AdventureEditorKit.GetMinLinearId();
				}
				string warning = "Time stamp id out of retry count, use random id " + maxId.ToString();
				Debug.LogWarning(warning);
			}
			result = maxId;
		}
		return result;
	}

	// Token: 0x06001526 RID: 5414 RVA: 0x00082D94 File Offset: 0x00080F94
	public static int GetNewElementId()
	{
		bool flag = string.IsNullOrEmpty(AdventureEditorKit.ElementDirectory);
		int result;
		if (flag)
		{
			result = -1;
		}
		else
		{
			Dictionary<int, ValueTuple<AdventureElementSnapshot, DateTime, string>> elementCache = AdventureEditorKit._elementCache;
			bool flag2 = elementCache == null || elementCache.Count <= 0;
			if (flag2)
			{
				AdventureEditorKit.UpdateElementCache();
			}
			int maxId = 1;
			int i = 0;
			while (i < 10 && AdventureEditorKit._elementCache.ContainsKey(maxId))
			{
				maxId = AdventureEditorKit.GetTimeStampId();
				i++;
			}
			bool flag3 = AdventureEditorKit._elementCache.ContainsKey(maxId);
			if (flag3)
			{
				maxId = AdventureEditorKit._elementCache.Keys.Max() + 1;
				bool flag4 = maxId < AdventureEditorKit.GetMinLinearId();
				if (flag4)
				{
					maxId = AdventureEditorKit.GetMinLinearId();
				}
				string warning = "Time stamp id out of retry count, use random id " + maxId.ToString();
				Debug.LogWarning(warning);
			}
			result = maxId;
		}
		return result;
	}

	// Token: 0x06001527 RID: 5415 RVA: 0x00082E64 File Offset: 0x00081064
	private static string GetCoreRoot()
	{
		string path = AdventureEditorKit.CorePath;
		bool flag = string.IsNullOrEmpty(path) || !File.Exists(path);
		string result;
		if (flag)
		{
			result = string.Empty;
		}
		else
		{
			FileInfo file = new FileInfo(path);
			result = ((file.Directory != null) ? file.Directory.FullName : string.Empty);
		}
		return result;
	}

	// Token: 0x06001528 RID: 5416 RVA: 0x00082EC0 File Offset: 0x000810C0
	private static string GetCoreDirectory()
	{
		string root = AdventureEditorKit.GetCoreRoot();
		return string.IsNullOrEmpty(root) ? string.Empty : Path.Combine(root, "core");
	}

	// Token: 0x06001529 RID: 5417 RVA: 0x00082EF4 File Offset: 0x000810F4
	private static string GetElementDirectory()
	{
		string root = AdventureEditorKit.GetCoreRoot();
		return string.IsNullOrEmpty(root) ? string.Empty : Path.Combine(root, "element");
	}

	// Token: 0x0600152A RID: 5418 RVA: 0x00082F28 File Offset: 0x00081128
	private static string GetMajorEventDirectory()
	{
		string root = AdventureEditorKit.GetCoreRoot();
		return string.IsNullOrEmpty(root) ? string.Empty : Path.Combine(root, "major-event");
	}

	// Token: 0x0600152B RID: 5419 RVA: 0x00082F5C File Offset: 0x0008115C
	private static string GetNewFilePath(string rootPath, string fileName)
	{
		string fullPath = Path.Combine(rootPath, fileName);
		string nameWithoutExtension = Path.GetFileNameWithoutExtension(fileName);
		string extension = Path.GetExtension(fileName);
		int count = 1;
		while (File.Exists(fullPath))
		{
			fullPath = Path.Combine(rootPath, string.Format("{0}_{1}{2}", nameWithoutExtension, count++, extension));
		}
		return fullPath;
	}

	// Token: 0x0600152C RID: 5420 RVA: 0x00082FB4 File Offset: 0x000811B4
	private static string GetNewDirectoryPath(string rootPath, string directoryName)
	{
		string fullPath = Path.Combine(rootPath, directoryName);
		int count = 1;
		while (Directory.Exists(fullPath))
		{
			fullPath = Path.Combine(rootPath, string.Format("{0}_{1}", directoryName, count++));
		}
		return fullPath;
	}

	// Token: 0x0600152D RID: 5421 RVA: 0x00082FFC File Offset: 0x000811FC
	internal static void FixLayerSortingOrder(GameObject register, Canvas anchorCanvas)
	{
		Dictionary<Canvas, int> dict = new Dictionary<Canvas, int>();
		foreach (UILayer layer in AdventureEditorKit.GetFixLayers())
		{
			Canvas layerCanvas = UIManager.Instance.GetLayer(layer).GetComponent<Canvas>();
			dict[layerCanvas] = layerCanvas.sortingOrder;
			bool flag = null != anchorCanvas;
			if (flag)
			{
				layerCanvas.sortingOrder = anchorCanvas.sortingOrder + 96 + dict.Count;
			}
		}
		AdventureEditorKit.LayerSortingOrderRegister[register] = dict;
	}

	// Token: 0x0600152E RID: 5422 RVA: 0x0008309C File Offset: 0x0008129C
	internal static void RestoreLayerSortingOrder(GameObject register)
	{
		Dictionary<Canvas, int> dict;
		bool flag = !AdventureEditorKit.LayerSortingOrderRegister.TryGetValue(register, out dict);
		if (!flag)
		{
			foreach (KeyValuePair<Canvas, int> pair in dict)
			{
				pair.Key.sortingOrder = pair.Value;
			}
		}
	}

	// Token: 0x0600152F RID: 5423 RVA: 0x00083114 File Offset: 0x00081314
	private static IEnumerable<UILayer> GetFixLayers()
	{
		yield return UILayer.LayerPopUp;
		yield return UILayer.LayerVeryTop;
		yield break;
	}

	// Token: 0x0400118E RID: 4494
	private const string AdventureCorePath = "ConchShip_PresetKey_AdventureCorePath";

	// Token: 0x0400118F RID: 4495
	public const string AdventureCoreDirectorySuffix = "core";

	// Token: 0x04001190 RID: 4496
	public const string AdventureElementDirectorySuffix = "element";

	// Token: 0x04001191 RID: 4497
	public const string AdventureMajorEventDirectorySuffix = "major-event";

	// Token: 0x04001192 RID: 4498
	public const string BlueprintFileExtension = ".advbp";

	// Token: 0x04001193 RID: 4499
	public const string ElementFileExtension = ".adve";

	// Token: 0x04001194 RID: 4500
	public const string MajorEventFileExtension = ".advme";

	// Token: 0x04001195 RID: 4501
	private const string ElementDefaultIcon = "adventure-editor-back-element";

	// Token: 0x04001196 RID: 4502
	public static readonly AdventureEditorBlackBoard BlackBoard = new AdventureEditorBlackBoard();

	// Token: 0x04001197 RID: 4503
	[TupleElementNames(new string[]
	{
		"data",
		"lastLoadTime",
		"path"
	})]
	private static Dictionary<int, ValueTuple<AdventureElementSnapshot, DateTime, string>> _elementCache;

	// Token: 0x04001198 RID: 4504
	private static bool _ignoreUpdateCache;

	// Token: 0x04001199 RID: 4505
	private static readonly Dictionary<GameObject, Dictionary<Canvas, int>> LayerSortingOrderRegister = new Dictionary<GameObject, Dictionary<Canvas, int>>();
}
