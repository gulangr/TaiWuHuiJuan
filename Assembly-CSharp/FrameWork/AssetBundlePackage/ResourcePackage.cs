using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.CompilerServices;
using UnityEngine;

namespace FrameWork.AssetBundlePackage
{
	// Token: 0x02001077 RID: 4215
	public class ResourcePackage
	{
		// Token: 0x17001584 RID: 5508
		// (get) Token: 0x0600BF58 RID: 48984 RVA: 0x00568189 File Offset: 0x00566389
		// (set) Token: 0x0600BF59 RID: 48985 RVA: 0x00568191 File Offset: 0x00566391
		public int SortingOrder { get; set; }

		// Token: 0x0600BF5A RID: 48986 RVA: 0x0056819A File Offset: 0x0056639A
		public ResourcePackage(string directory, AssetBundleManifest manifest, PackageAssetBundleController controller)
		{
			this._directory = directory;
			this._manifest = manifest;
			this._controller = controller;
			this._loadedBundleMap = new Dictionary<string, AssetBundle>();
			this.StaticBundles = new List<string>();
		}

		// Token: 0x0600BF5B RID: 48987 RVA: 0x005681D0 File Offset: 0x005663D0
		public AssetBundleManifest GetAssetBundleManifest()
		{
			return this._manifest;
		}

		// Token: 0x0600BF5C RID: 48988 RVA: 0x005681E8 File Offset: 0x005663E8
		public string GetDirectory()
		{
			return this._directory;
		}

		// Token: 0x0600BF5D RID: 48989 RVA: 0x00568200 File Offset: 0x00566400
		public void AddCache(AssetBundle assetBundle)
		{
			string bundleName = assetBundle.name;
			bool flag = !this._loadedBundleMap.ContainsKey(bundleName);
			if (flag)
			{
				this._loadedBundleMap.Add(bundleName, assetBundle);
			}
		}

		// Token: 0x0600BF5E RID: 48990 RVA: 0x00568238 File Offset: 0x00566438
		public bool TryGetCache(string bundleName, out AssetBundle assetBundle)
		{
			return this._loadedBundleMap.TryGetValue(bundleName, out assetBundle);
		}

		// Token: 0x0600BF5F RID: 48991 RVA: 0x00568258 File Offset: 0x00566458
		public void UnloadBundleAndRemoveCache(AssetBundle bundle)
		{
			bool flag = this._loadedBundleMap.ContainsKey(bundle.name);
			if (flag)
			{
				this._loadedBundleMap.Remove(bundle.name);
				bundle.Unload(true);
			}
		}

		// Token: 0x0600BF60 RID: 48992 RVA: 0x00568298 File Offset: 0x00566498
		public void ClearCache()
		{
			List<string> removeList = EasyPool.Get<List<string>>();
			foreach (KeyValuePair<string, AssetBundle> pair in this._loadedBundleMap)
			{
				List<string> staticBundles = this.StaticBundles;
				bool flag = staticBundles == null || !staticBundles.Contains(pair.Key);
				if (flag)
				{
					bool flag2 = null != pair.Value;
					if (flag2)
					{
						pair.Value.Unload(false);
					}
					removeList.Add(pair.Key);
				}
			}
			removeList.ForEach(delegate(string e)
			{
				this._loadedBundleMap.Remove(e);
			});
			EasyPool.Free<List<string>>(removeList);
		}

		// Token: 0x0600BF61 RID: 48993 RVA: 0x0056835C File Offset: 0x0056655C
		public ValueTuple<ResourcePackage, string, Object> TryGetAssetBundleLoadData(Type type, List<string> dependenceList, string assetPath = "", string assetName = "")
		{
			ResourcePackage.<>c__DisplayClass17_0 CS$<>8__locals1 = new ResourcePackage.<>c__DisplayClass17_0();
			CS$<>8__locals1.<>4__this = this;
			CS$<>8__locals1.dependenceList = dependenceList;
			bool flag = CS$<>8__locals1.dependenceList == null;
			if (flag)
			{
				CS$<>8__locals1.dependenceList = EasyPool.Get<List<string>>();
			}
			CS$<>8__locals1.dependenceList.Clear();
			bool flag2 = type == typeof(AssetBundle);
			if (flag2)
			{
				string bundleName = assetPath;
				bool flag3 = !string.IsNullOrEmpty(assetName);
				if (flag3)
				{
					bundleName = assetName;
				}
				string filePath = Path.Combine(this._directory, bundleName).PathFix();
				bool flag4 = !filePath.EndsWith(FrameCommon.AbSuffix);
				if (flag4)
				{
					filePath += FrameCommon.AbSuffix;
				}
				bool flag5 = File.Exists(filePath);
				if (flag5)
				{
					string bundleFileName = Path.GetFileName(filePath);
					CS$<>8__locals1.<TryGetAssetBundleLoadData>g__AddDependence|0(bundleFileName);
					AssetBundle bundle;
					this._loadedBundleMap.TryGetValue(bundleFileName, out bundle);
					return new ValueTuple<ResourcePackage, string, Object>(this, filePath, bundle);
				}
			}
			string targetBundleName = string.Empty;
			bool flag6 = !string.IsNullOrEmpty(assetPath);
			if (flag6)
			{
				targetBundleName = this._controller.GetBundleName(assetPath, type);
			}
			bool flag7 = !string.IsNullOrEmpty(assetName);
			if (flag7)
			{
				targetBundleName = this._controller.GetBundleNameByAssetName(assetName, type);
			}
			bool flag8 = !string.IsNullOrEmpty(targetBundleName);
			ValueTuple<ResourcePackage, string, Object> result;
			if (flag8)
			{
				string bundlePath = Path.Combine(this._directory, targetBundleName).PathFix();
				AssetBundle bundle2;
				bool flag9 = this._loadedBundleMap != null && this._loadedBundleMap.TryGetValue(targetBundleName, out bundle2) && null != bundle2;
				if (flag9)
				{
					bool flag10 = string.IsNullOrEmpty(assetName) && !string.IsNullOrEmpty(assetPath);
					if (flag10)
					{
						assetName = Path.GetFileNameWithoutExtension(assetPath);
					}
					Object obj = bundle2.LoadAsset(assetName, type);
					result = new ValueTuple<ResourcePackage, string, Object>(this, bundlePath, obj);
				}
				else
				{
					bool flag11 = File.Exists(bundlePath);
					if (flag11)
					{
						CS$<>8__locals1.<TryGetAssetBundleLoadData>g__AddDependence|0(targetBundleName);
					}
					result = new ValueTuple<ResourcePackage, string, Object>(this, bundlePath, null);
				}
			}
			else
			{
				result = new ValueTuple<ResourcePackage, string, Object>(null, string.Empty, null);
			}
			return result;
		}

		// Token: 0x0600BF62 RID: 48994 RVA: 0x00568550 File Offset: 0x00566750
		[return: TupleElementNames(new string[]
		{
			"resPackage",
			"path",
			"bundle"
		})]
		public ValueTuple<ResourcePackage, string, AssetBundle> TryGetAssetBundle(Type type, List<string> dependenceList, string assetPath = "", string assetName = "")
		{
			ResourcePackage.<>c__DisplayClass18_0 CS$<>8__locals1 = new ResourcePackage.<>c__DisplayClass18_0();
			CS$<>8__locals1.<>4__this = this;
			CS$<>8__locals1.dependenceList = dependenceList;
			bool flag = CS$<>8__locals1.dependenceList == null;
			if (flag)
			{
				CS$<>8__locals1.dependenceList = EasyPool.Get<List<string>>();
			}
			CS$<>8__locals1.dependenceList.Clear();
			bool flag2 = type == typeof(AssetBundle);
			if (flag2)
			{
				string bundleName = assetPath;
				bool flag3 = !string.IsNullOrEmpty(assetName);
				if (flag3)
				{
					bundleName = assetName;
				}
				string filePath = Path.Combine(this._directory, bundleName).PathFix();
				bool flag4 = !filePath.EndsWith(FrameCommon.AbSuffix);
				if (flag4)
				{
					filePath += FrameCommon.AbSuffix;
				}
				bool flag5 = File.Exists(filePath);
				if (flag5)
				{
					string bundleFileName = Path.GetFileName(filePath);
					CS$<>8__locals1.<TryGetAssetBundle>g__AddDependence|0(bundleFileName);
					AssetBundle bundle;
					this._loadedBundleMap.TryGetValue(bundleFileName, out bundle);
					return new ValueTuple<ResourcePackage, string, AssetBundle>(this, filePath, bundle);
				}
			}
			string targetBundleName = string.Empty;
			bool flag6 = !string.IsNullOrEmpty(assetPath);
			if (flag6)
			{
				targetBundleName = this._controller.GetBundleName(assetPath, type);
			}
			bool flag7 = !string.IsNullOrEmpty(assetName);
			if (flag7)
			{
				targetBundleName = this._controller.GetBundleNameByAssetName(assetName, type);
			}
			bool flag8 = !string.IsNullOrEmpty(targetBundleName);
			ValueTuple<ResourcePackage, string, AssetBundle> result;
			if (flag8)
			{
				string bundlePath = Path.Combine(this._directory, targetBundleName).PathFix();
				AssetBundle bundle2;
				bool flag9 = this._loadedBundleMap != null && this._loadedBundleMap.TryGetValue(targetBundleName, out bundle2) && null != bundle2;
				if (flag9)
				{
					result = new ValueTuple<ResourcePackage, string, AssetBundle>(this, bundlePath, bundle2);
				}
				else
				{
					bool flag10 = File.Exists(bundlePath);
					if (flag10)
					{
						CS$<>8__locals1.<TryGetAssetBundle>g__AddDependence|0(targetBundleName);
					}
					result = new ValueTuple<ResourcePackage, string, AssetBundle>(this, bundlePath, null);
				}
			}
			else
			{
				result = new ValueTuple<ResourcePackage, string, AssetBundle>(null, string.Empty, null);
			}
			return result;
		}

		// Token: 0x04009296 RID: 37526
		private readonly AssetBundleManifest _manifest;

		// Token: 0x04009297 RID: 37527
		private readonly PackageAssetBundleController _controller;

		// Token: 0x04009298 RID: 37528
		private readonly string _directory;

		// Token: 0x0400929A RID: 37530
		public string PackageName;

		// Token: 0x0400929B RID: 37531
		private readonly Dictionary<string, AssetBundle> _loadedBundleMap;

		// Token: 0x0400929C RID: 37532
		public readonly List<string> StaticBundles;
	}
}
