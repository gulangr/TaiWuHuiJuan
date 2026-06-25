using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace FrameWork.AssetBundlePackage
{
	// Token: 0x02001076 RID: 4214
	public sealed class PackageAssetBundleController : ScriptableObject
	{
		// Token: 0x0600BF51 RID: 48977 RVA: 0x00567DB0 File Offset: 0x00565FB0
		private void SetAssetInfo(int depth, Dictionary<string, PackageAssetBundleController.FolderInfo> handleMap, string[] pathArray, PackageAssetBundleController.FolderInfo folderInfo, PackageAssetBundleController.AssetInfo assetInfo)
		{
			bool flag = depth >= pathArray.Length - 1 && folderInfo != null;
			if (flag)
			{
				folderInfo.FolderAssetsMap.TryAdd(assetInfo.AssetPath, assetInfo);
			}
			else
			{
				string folderName = pathArray[depth];
				bool flag2 = !handleMap.TryGetValue(folderName, out folderInfo);
				if (flag2)
				{
					folderInfo = new PackageAssetBundleController.FolderInfo(folderName);
					handleMap.Add(folderName, folderInfo);
				}
				this.SetAssetInfo(depth + 1, folderInfo.ChildFolderMap, pathArray, folderInfo, assetInfo);
			}
		}

		// Token: 0x0600BF52 RID: 48978 RVA: 0x00567E2C File Offset: 0x0056602C
		public void Init()
		{
			this._assetStructureMap = new Dictionary<string, PackageAssetBundleController.FolderInfo>();
			this._assetListNameMap = new Dictionary<string, List<PackageAssetBundleController.AssetInfo>>();
			int i = 0;
			int max = this._assetList.Count;
			while (i < max)
			{
				PackageAssetBundleController.AssetInfo info = this._assetList[i];
				string[] pathArray = info.AssetPath.Split('/', StringSplitOptions.None);
				bool flag = pathArray.Length < 2;
				if (flag)
				{
					throw new Exception("can not handle asset at top folder!");
				}
				this.SetAssetInfo(0, this._assetStructureMap, pathArray, null, info);
				string assetName = Path.GetFileNameWithoutExtension(info.AssetPath);
				List<PackageAssetBundleController.AssetInfo> list;
				bool flag2 = !this._assetListNameMap.TryGetValue(assetName, out list);
				if (flag2)
				{
					list = new List<PackageAssetBundleController.AssetInfo>();
					this._assetListNameMap.Add(assetName, list);
				}
				list.Add(info);
				i++;
			}
		}

		// Token: 0x0600BF53 RID: 48979 RVA: 0x00567F04 File Offset: 0x00566104
		public string[] GetAllAssetPaths()
		{
			bool flag = this._assetList == null || this._assetList.Count == 0;
			string[] result2;
			if (flag)
			{
				result2 = Array.Empty<string>();
			}
			else
			{
				string[] result = new string[this._assetList.Count];
				for (int i = 0; i < this._assetList.Count; i++)
				{
					result[i] = this._assetList[i].AssetPath;
				}
				result2 = result;
			}
			return result2;
		}

		// Token: 0x0600BF54 RID: 48980 RVA: 0x00567F80 File Offset: 0x00566180
		public bool CheckDlcAssetBundles(string pathPrefix)
		{
			int i = 0;
			int max = this._assetList.Count;
			while (i < max)
			{
				bool flag = !File.Exists(Path.Combine(pathPrefix, this._assetList[i].AssetBundleName));
				if (flag)
				{
					return false;
				}
				i++;
			}
			return true;
		}

		// Token: 0x0600BF55 RID: 48981 RVA: 0x00567FDC File Offset: 0x005661DC
		public string GetBundleName(string path, Type type)
		{
			string bundleName = string.Empty;
			bool flag = type.IsSubclassOf(typeof(ScriptableObject));
			if (flag)
			{
				type = typeof(ScriptableObject);
			}
			bool flag2 = !path.StartsWith(this.ProjectAssetsRoot);
			if (flag2)
			{
				path = Path.Combine(this.ProjectAssetsRoot, path).PathFix();
			}
			string[] pathArray = path.Split('/', StringSplitOptions.None);
			PackageAssetBundleController.FolderInfo folder;
			bool flag3 = this._assetStructureMap.TryGetValue(pathArray[0], out folder);
			if (flag3)
			{
				for (int i = 1; i < pathArray.Length; i++)
				{
					bool flag4 = folder == null;
					if (flag4)
					{
						break;
					}
					bool flag5 = i < pathArray.Length - 1;
					if (flag5)
					{
						folder.ChildFolderMap.TryGetValue(pathArray[i], out folder);
					}
					else
					{
						PackageAssetBundleController.AssetInfo info;
						bool flag6 = folder.FolderAssetsMap.TryGetValue(path, out info) && info.AssetTypeNameList.Contains(type.ToString());
						if (flag6)
						{
							return info.AssetBundleName;
						}
					}
				}
			}
			return bundleName;
		}

		// Token: 0x0600BF56 RID: 48982 RVA: 0x005680E8 File Offset: 0x005662E8
		public string GetBundleNameByAssetName(string assetName, Type type)
		{
			bool flag = type.IsSubclassOf(typeof(ScriptableObject));
			if (flag)
			{
				type = typeof(ScriptableObject);
			}
			List<PackageAssetBundleController.AssetInfo> list;
			bool flag2 = this._assetListNameMap.TryGetValue(assetName, out list);
			if (flag2)
			{
				int i = 0;
				int max = list.Count;
				while (i < max)
				{
					bool flag3 = list[i].AssetTypeNameList.Contains(type.ToString());
					if (flag3)
					{
						return list[i].AssetBundleName;
					}
					i++;
				}
			}
			return string.Empty;
		}

		// Token: 0x04009292 RID: 37522
		[SerializeField]
		private List<PackageAssetBundleController.AssetInfo> _assetList;

		// Token: 0x04009293 RID: 37523
		public string ProjectAssetsRoot;

		// Token: 0x04009294 RID: 37524
		private Dictionary<string, PackageAssetBundleController.FolderInfo> _assetStructureMap;

		// Token: 0x04009295 RID: 37525
		private Dictionary<string, List<PackageAssetBundleController.AssetInfo>> _assetListNameMap;

		// Token: 0x02002687 RID: 9863
		private class FolderInfo
		{
			// Token: 0x06011C1D RID: 72733 RVA: 0x0068955F File Offset: 0x0068775F
			public FolderInfo(string name)
			{
				this.FolderName = name;
				this.ChildFolderMap = new Dictionary<string, PackageAssetBundleController.FolderInfo>();
				this.FolderAssetsMap = new Dictionary<string, PackageAssetBundleController.AssetInfo>();
			}

			// Token: 0x0400EB02 RID: 60162
			public string FolderName;

			// Token: 0x0400EB03 RID: 60163
			public Dictionary<string, PackageAssetBundleController.FolderInfo> ChildFolderMap;

			// Token: 0x0400EB04 RID: 60164
			public Dictionary<string, PackageAssetBundleController.AssetInfo> FolderAssetsMap;
		}

		// Token: 0x02002688 RID: 9864
		[Serializable]
		private class AssetInfo
		{
			// Token: 0x0400EB05 RID: 60165
			public string AssetPath;

			// Token: 0x0400EB06 RID: 60166
			public string AssetBundleName;

			// Token: 0x0400EB07 RID: 60167
			public List<string> AssetTypeNameList;
		}
	}
}
