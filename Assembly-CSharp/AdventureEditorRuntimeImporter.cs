using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using GameData.Adventure;
using GameData.Adventure.Editor;
using GameData.Domains.Adventure;
using GameData.Utilities;
using UnityEngine;

// Token: 0x02000182 RID: 386
public static class AdventureEditorRuntimeImporter
{
	// Token: 0x060015E0 RID: 5600 RVA: 0x000873F8 File Offset: 0x000855F8
	public static void ExportToContext(string srcPath)
	{
		AdventureEditorKit.UpdateElementCache();
		AdventureEditorKit.DisableUpdateElementCache();
		AdventureEditorRuntimeImporter.ExportingFiles.Clear();
		AdventureEditorRuntimeImporter.Compiler.LocalStringMappings.Clear();
		string dstPath = AdventureRemakeModel.GetExportPath().Replace("/", "\\");
		DirectoryInfo directory = new DirectoryInfo(srcPath);
		foreach (FileInfo srcFile in directory.GetFiles("*.*", SearchOption.AllDirectories))
		{
			bool flag = srcFile.Extension == ".advbp";
			if (flag)
			{
				AdventureEditorRuntimeImporter.ExportAdventureData(srcFile, dstPath);
			}
			else
			{
				bool flag2 = srcFile.Extension == ".adve";
				if (flag2)
				{
					AdventureEditorRuntimeImporter.ExportAdventureElementData(srcFile, dstPath);
				}
				else
				{
					bool flag3 = srcFile.Extension == ".advme";
					if (flag3)
					{
						AdventureEditorRuntimeImporter.ExportAdventureMajorEventData(srcFile, dstPath);
					}
				}
			}
		}
		AdventureEditorRuntimeImporter.ExportLocalStringMapping();
		AdventureEditorKit.EnableUpdateElementCache();
		string warning = string.Empty;
		foreach (KeyValuePair<string, List<string>> keyValuePair in AdventureEditorRuntimeImporter.ExportingFiles)
		{
			string text;
			List<string> list;
			keyValuePair.Deconstruct(out text, out list);
			string dstFile = text;
			List<string> srcFiles = list;
			bool flag4 = srcFiles.Count > 1;
			if (flag4)
			{
				warning = string.Concat(new string[]
				{
					warning,
					dstFile,
					" has multi source files: \n",
					string.Join("\n", srcFiles),
					"\n\n"
				});
			}
		}
		foreach (string checkExtension in AdventureEditorRuntimeImporter.CheckExtensions)
		{
			foreach (string checkPath in Directory.GetFiles(dstPath, "*" + checkExtension, SearchOption.AllDirectories))
			{
				bool flag5 = !AdventureEditorRuntimeImporter.ExportingFiles.ContainsKey(checkPath) && AdventureEditorRuntimeImporter.DeleteFileAndMeta(checkPath);
				if (flag5)
				{
					warning = warning + checkPath + " has been removed after import\n";
				}
			}
		}
		bool flag6 = string.IsNullOrEmpty(warning);
		if (flag6)
		{
			Debug.Log("Import adventure data successfully.");
		}
		else
		{
			Debug.LogWarning("Import adventure data with warnings:\n" + warning);
		}
		bool isPlaying = Application.isPlaying;
		if (isPlaying)
		{
			GlobalSettings global = SingletonObject.getInstance<GlobalSettings>();
			LocalStringManager.Init(global.Language);
			AdventureRemakeModel.InitializeAdventureCore();
			AdventureDomainMethod.Call.GmCmd_ReloadAdventureCore();
			Debug.Log("Reload adventure core in runtime.");
		}
	}

	// Token: 0x060015E1 RID: 5601 RVA: 0x0008767C File Offset: 0x0008587C
	private static void ExportLocalStringMapping()
	{
		string path = Path.Combine(Application.streamingAssetsPath, "Language_CN");
		path = Path.Combine(path, "AdventureCore_language.txt");
		using (FileStream fs = new FileStream(path, FileMode.Create))
		{
			using (StreamWriter writer = new StreamWriter(fs, Encoding.UTF8))
			{
				writer.NewLine = "\n";
				List<string> keys = new List<string>(AdventureEditorRuntimeImporter.Compiler.LocalStringMappings.Keys);
				keys.Sort();
				foreach (string key in keys)
				{
					writer.WriteLine(key);
					writer.WriteLine(AdventureEditorRuntimeImporter.Compiler.LocalStringMappings[key].Trim().Replace("\n", "\\n"));
				}
			}
		}
	}

	// Token: 0x060015E2 RID: 5602 RVA: 0x00087788 File Offset: 0x00085988
	public static bool TryParseAdvPath(FileInfo srcFile, string dstPath, out string dstFilePath)
	{
		dstFilePath = string.Empty;
		AdventureSnapshot adventureSnapshot;
		bool flag = !AdventureSnapshot.TryLoadFromFile(srcFile.FullName, out adventureSnapshot);
		bool result;
		if (flag)
		{
			Debug.LogWarning("Failed to load adventure snapshot from " + srcFile.FullName);
			result = false;
		}
		else
		{
			bool flag2 = adventureSnapshot.Id <= 0;
			if (flag2)
			{
				Debug.LogWarning("Load invalid adventure snapshot from " + srcFile.FullName);
				result = false;
			}
			else
			{
				dstPath = Path.Combine(dstPath, "core".FirstCharToUpper());
				string srcPath = srcFile.DirectoryName ?? string.Empty;
				string srcFileName = srcFile.Name.Replace(srcFile.Extension, string.Empty);
				string dstFileName = adventureSnapshot.Id.ToString();
				dstFilePath = srcFile.FullName.Replace(srcPath, dstPath).ReplaceLast(srcFileName, dstFileName).Replace(".advbp", ".advd");
				result = true;
			}
		}
		return result;
	}

	// Token: 0x060015E3 RID: 5603 RVA: 0x00087874 File Offset: 0x00085A74
	public static bool TryParseElementPath(FileInfo srcFile, string dstPath, out string dstFilePath)
	{
		dstFilePath = string.Empty;
		AdventureElementSnapshot elementSnapshot;
		bool flag = !AdventureElementSnapshot.TryLoadFromFile(srcFile.FullName, out elementSnapshot);
		bool result;
		if (flag)
		{
			Debug.LogWarning("Failed to load element snapshot from " + srcFile.FullName);
			result = false;
		}
		else
		{
			bool flag2 = elementSnapshot.Id <= 0;
			if (flag2)
			{
				Debug.LogWarning("Load invalid element snapshot from " + srcFile.FullName);
				result = false;
			}
			else
			{
				dstPath = Path.Combine(dstPath, "element".FirstCharToUpper());
				string srcPath = srcFile.DirectoryName ?? string.Empty;
				string srcFileName = srcFile.Name.Replace(srcFile.Extension, string.Empty);
				string dstFileName = elementSnapshot.Id.ToString();
				dstFilePath = srcFile.FullName.Replace(srcPath, dstPath).ReplaceLast(srcFileName, dstFileName).Replace(".adve", ".adved");
				result = true;
			}
		}
		return result;
	}

	// Token: 0x060015E4 RID: 5604 RVA: 0x00087960 File Offset: 0x00085B60
	public static bool TryParseMajorEventPath(FileInfo srcFile, string dstPath, out string dstFilePath)
	{
		dstFilePath = string.Empty;
		AdventureMajorEventSnapshot snapshot;
		bool flag = !AdventureMajorEventSnapshot.TryLoadFromFile(srcFile.FullName, out snapshot);
		bool result;
		if (flag)
		{
			Debug.LogWarning("Failed to load major event snapshot from " + srcFile.FullName);
			result = false;
		}
		else
		{
			bool flag2 = snapshot.Id <= 0;
			if (flag2)
			{
				Debug.LogWarning("Load invalid major event snapshot from " + srcFile.FullName);
				result = false;
			}
			else
			{
				dstPath = Path.Combine(dstPath, "core".FirstCharToUpper());
				string srcPath = srcFile.DirectoryName ?? string.Empty;
				string srcFileName = srcFile.Name.Replace(srcFile.Extension, string.Empty);
				string dstFileName = snapshot.Id.ToString();
				dstFilePath = srcFile.FullName.Replace(srcPath, dstPath).ReplaceLast(srcFileName, dstFileName).Replace(".advme", ".advmed");
				result = true;
			}
		}
		return result;
	}

	// Token: 0x060015E5 RID: 5605 RVA: 0x00087A4C File Offset: 0x00085C4C
	private static void ExportAdventureData(FileInfo srcFile, string dstPath)
	{
		string dstFilePath;
		bool flag = !AdventureEditorRuntimeImporter.TryParseAdvPath(srcFile, dstPath, out dstFilePath);
		if (!flag)
		{
			FileInfo dstFile = new FileInfo(dstFilePath);
			DirectoryInfo directory = dstFile.Directory;
			bool flag2 = directory != null && !directory.Exists;
			if (flag2)
			{
				Directory.CreateDirectory(dstFile.Directory.FullName);
			}
			AdventureEditorRuntimeImporter.ExportingFiles.GetOrNew(dstFile.FullName).Add(srcFile.FullName);
			AdventureSnapshot srcData = AdventureSnapshot.LoadFromFile(srcFile.FullName);
			AdventureEditorRuntimeImporter.FixInvalidElement(srcData);
			AdventureData data;
			bool flag3 = AdventureRemakeModel.Core.TryGetAdventureData(srcData.Id, out data) && data.MinorVersion > srcData.MinorVersion;
			if (flag3)
			{
				Debug.LogError(string.Format("Export adventure failed at {0}({1})", srcData.Name.Value, srcData.Id));
			}
			else
			{
				AdventureData dstData = srcData.CompileToRuntime(AdventureEditorRuntimeImporter.Compiler);
				dstData.Save(dstFile.FullName);
			}
		}
	}

	// Token: 0x060015E6 RID: 5606 RVA: 0x00087B48 File Offset: 0x00085D48
	private static void ExportAdventureElementData(FileInfo srcFile, string dstPath)
	{
		string dstFilePath;
		bool flag = !AdventureEditorRuntimeImporter.TryParseElementPath(srcFile, dstPath, out dstFilePath);
		if (!flag)
		{
			FileInfo dstFile = new FileInfo(dstFilePath);
			DirectoryInfo directory = dstFile.Directory;
			bool flag2 = directory != null && !directory.Exists;
			if (flag2)
			{
				Directory.CreateDirectory(dstFile.Directory.FullName);
			}
			AdventureEditorRuntimeImporter.ExportingFiles.GetOrNew(dstFile.FullName).Add(srcFile.FullName);
			AdventureElementSnapshot srcData = AdventureElementSnapshot.LoadFromFile(srcFile.FullName).Clone();
			AdventureEditorRuntimeImporter.CalcInheritElement(srcData, 0U);
			AdventureElementData dstData = srcData.CompileToRuntime(AdventureEditorRuntimeImporter.Compiler);
			dstData.Save(dstFile.FullName);
		}
	}

	// Token: 0x060015E7 RID: 5607 RVA: 0x00087BF4 File Offset: 0x00085DF4
	private static void ExportAdventureMajorEventData(FileInfo srcFile, string dstPath)
	{
		string dstFilePath;
		bool flag = !AdventureEditorRuntimeImporter.TryParseMajorEventPath(srcFile, dstPath, out dstFilePath);
		if (!flag)
		{
			FileInfo dstFile = new FileInfo(dstFilePath);
			DirectoryInfo directory = dstFile.Directory;
			bool flag2 = directory != null && !directory.Exists;
			if (flag2)
			{
				Directory.CreateDirectory(dstFile.Directory.FullName);
			}
			AdventureEditorRuntimeImporter.ExportingFiles.GetOrNew(dstFile.FullName).Add(srcFile.FullName);
			AdventureMajorEventSnapshot srcData = AdventureMajorEventSnapshot.LoadFromFile(srcFile.FullName);
			AdventureMajorEventData dstData = srcData.CompileToRuntime(AdventureEditorRuntimeImporter.Compiler);
			dstData.Save(dstFile.FullName);
		}
	}

	// Token: 0x060015E8 RID: 5608 RVA: 0x00087C90 File Offset: 0x00085E90
	private static void FixInvalidElement(AdventureSnapshot data)
	{
		foreach (AdventureBlockSnapshot block in data.Blocks)
		{
			for (int i = block.ElementCoreIds.Count - 1; i >= 0; i--)
			{
				int elementId = block.ElementCoreIds[i];
				bool flag = AdventureEditorKit.GetElementFromCache(elementId) != null;
				if (!flag)
				{
					block.ElementCoreIds.RemoveAt(i);
					Debug.LogError(string.Format("Auto fixed invalid element {0} at {1} {2} {3}", new object[]
					{
						elementId,
						data.Id,
						data.Name,
						block.Index
					}));
				}
			}
		}
	}

	// Token: 0x060015E9 RID: 5609 RVA: 0x00087D7C File Offset: 0x00085F7C
	private static void CalcInheritElement(AdventureElementSnapshot data, uint depth = 0U)
	{
		bool flag = depth >= 10U;
		if (flag)
		{
			throw new Exception("Element inherit overflow.\nDepth=" + AdventureEditorRuntimeImporter._inheritDepth + "\nData=\n" + data.SaveToJson());
		}
		bool flag2 = data.InheritId <= 0;
		if (!flag2)
		{
			bool flag3 = depth == 0U;
			if (flag3)
			{
				AdventureEditorRuntimeImporter._inheritDepth = data.Id.ToString() + "(" + data.Name + ")";
			}
			AdventureEditorRuntimeImporter._inheritDepth = AdventureEditorRuntimeImporter._inheritDepth + " => " + data.InheritId.ToString();
			AdventureElementSnapshot inherit = AdventureEditorKit.GetElementFromCache(data.InheritId);
			bool flag4 = inherit != null;
			if (flag4)
			{
				AdventureEditorRuntimeImporter._inheritDepth = AdventureEditorRuntimeImporter._inheritDepth + "(" + inherit.Name + ")";
				inherit = inherit.Clone();
				AdventureEditorRuntimeImporter.CalcInheritElement(inherit, depth + 1U);
				foreach (object overrideTypeObj in Enum.GetValues(typeof(EAdventureElementOverrideType)))
				{
					EAdventureElementOverrideType overrideType = (EAdventureElementOverrideType)overrideTypeObj;
					bool flag5 = data.IsOverride(overrideType);
					if (!flag5)
					{
						switch (overrideType)
						{
						case EAdventureElementOverrideType.Name:
							data.Name = inherit.Name;
							break;
						case EAdventureElementOverrideType.Desc:
							data.Desc = inherit.Desc;
							break;
						case EAdventureElementOverrideType.Icon:
							data.Icon = inherit.Icon;
							break;
						case EAdventureElementOverrideType.CreatingType:
							data.CreatingType = inherit.CreatingType;
							break;
						case EAdventureElementOverrideType.TimeCost:
							data.TimeCost = inherit.TimeCost;
							break;
						case EAdventureElementOverrideType.Parameters:
							data.Parameters.ClearAndAddRange(inherit.Parameters);
							break;
						case EAdventureElementOverrideType.MoveData:
							data.MoveData.ClearAndAddRange(inherit.MoveData);
							break;
						case EAdventureElementOverrideType.VisibleCondition:
							data.VisibleCondition.ClearAndAddRange(inherit.VisibleCondition);
							break;
						case EAdventureElementOverrideType.Events:
							data.Events.ClearAndAddRange(inherit.Events);
							break;
						case EAdventureElementOverrideType.Character:
							data.CharacterId = inherit.CharacterId;
							data.CharacterData = inherit.CharacterData;
							break;
						case EAdventureElementOverrideType.Tags:
							data.Tags.ClearAndAddRange(inherit.Tags);
							break;
						case EAdventureElementOverrideType.VisiblePriority:
							data.VisiblePriority = inherit.VisiblePriority;
							break;
						case EAdventureElementOverrideType.VisibleIgnoreSorting:
							data.VisibleIgnoreSorting = inherit.VisibleIgnoreSorting;
							break;
						}
					}
				}
			}
			else
			{
				Debug.LogError("CalcInheritElement failed at " + AdventureEditorRuntimeImporter._inheritDepth);
			}
		}
	}

	// Token: 0x060015EA RID: 5610 RVA: 0x00088044 File Offset: 0x00086244
	private static bool DeleteFileAndMeta(string path)
	{
		bool anyChanged = false;
		bool flag = File.Exists(path);
		if (flag)
		{
			File.Delete(path);
			anyChanged = true;
		}
		string metaFile = path + ".meta";
		bool flag2 = File.Exists(metaFile);
		if (flag2)
		{
			File.Delete(metaFile);
			anyChanged = true;
		}
		return anyChanged;
	}

	// Token: 0x040011F6 RID: 4598
	private static readonly Dictionary<string, List<string>> ExportingFiles = new Dictionary<string, List<string>>();

	// Token: 0x040011F7 RID: 4599
	private static string _inheritDepth;

	// Token: 0x040011F8 RID: 4600
	private static readonly IReadOnlyList<string> CheckExtensions = new string[]
	{
		".advd",
		".adved",
		".advmed"
	};

	// Token: 0x040011F9 RID: 4601
	private static readonly AdventureDataCompiler Compiler = new AdventureDataCompiler();
}
