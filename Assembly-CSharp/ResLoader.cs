using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using FrameWork;
using FrameWork.AssetBundlePackage;
using FrameWork.ResManager;
using GameData.Utilities;
using UnityEngine;

// Token: 0x0200001E RID: 30
public class ResLoader : MonoBehaviour, ISingletonInit, IDisposable
{
	// Token: 0x060000D7 RID: 215 RVA: 0x00005E5C File Offset: 0x0000405C
	public static void LoadRes<T>(string aPath, Action<T> onLoad, bool mQuickLoad = false, Action<string> onLoadError = null) where T : Object
	{
		ResLoader loader = SingletonObject.getInstance<ResLoader>();
		bool flag = loader == null;
		if (flag)
		{
			if (onLoadError != null)
			{
				onLoadError(aPath);
			}
		}
		else
		{
			loader.InternalLoadRes<T>(aPath, onLoad, mQuickLoad, onLoadError);
		}
	}

	// Token: 0x060000D8 RID: 216 RVA: 0x00005E98 File Offset: 0x00004098
	public static void LoadModOrGameResource<T>(string path, Action<T> onLoad, Action<string> onLoadError = null) where T : Object
	{
		bool flag = typeof(T) == typeof(Texture2D);
		if (flag)
		{
			ResLoader.LoadModOrGameTexture<T>(path, onLoad, onLoadError);
		}
		else
		{
			bool flag2 = typeof(T) == typeof(Sprite);
			if (!flag2)
			{
				throw new ArgumentException(string.Format("type {0} is not supported in LoadModOrGameResource", typeof(T)));
			}
			ResLoader.LoadModOrGameSprite<T>(path, onLoad, onLoadError);
		}
	}

	// Token: 0x060000D9 RID: 217 RVA: 0x00005F14 File Offset: 0x00004114
	private static void LoadModOrGameTexture<T>(string path, Action<T> onLoad, Action<string> onLoadError) where T : Object
	{
		Texture2D modTexture = ResLoader.TryLoadModTexture(path);
		bool flag = modTexture != null;
		if (flag)
		{
			onLoad(modTexture as T);
		}
		else
		{
			string entryPath = "RemakeResources/Textures";
			ResLoader.Load<T>(entryPath + "/" + path, onLoad, onLoadError, false);
		}
	}

	// Token: 0x060000DA RID: 218 RVA: 0x00005F64 File Offset: 0x00004164
	private static void LoadModOrGameSprite<T>(string path, Action<T> onLoad, Action<string> onLoadError) where T : Object
	{
		string entryPath = "RemakeResources/Textures";
		ResLoader.Load<Sprite>(entryPath + "/" + path, delegate(Sprite gameSprite)
		{
			Sprite modSprite = ModManager.GetOverwriteSprite(path, gameSprite);
			bool flag = modSprite;
			if (flag)
			{
				Action<T> onLoad2 = onLoad;
				if (onLoad2 != null)
				{
					onLoad2(modSprite as T);
				}
			}
			else
			{
				Action<T> onLoad3 = onLoad;
				if (onLoad3 != null)
				{
					onLoad3(gameSprite as T);
				}
			}
		}, delegate(string message)
		{
			Sprite modSprite = ModManager.GetOverwriteSprite(path, null);
			bool flag = modSprite;
			if (flag)
			{
				Action<T> onLoad2 = onLoad;
				if (onLoad2 != null)
				{
					onLoad2(modSprite as T);
				}
			}
			else
			{
				Action<string> onLoadError2 = onLoadError;
				if (onLoadError2 != null)
				{
					onLoadError2(message);
				}
			}
		}, false);
	}

	// Token: 0x060000DB RID: 219 RVA: 0x00005FC4 File Offset: 0x000041C4
	private static Texture2D TryLoadModTexture(string path)
	{
		return ModManager.GetOverwriteTexture(path);
	}

	// Token: 0x060000DC RID: 220 RVA: 0x00005FE0 File Offset: 0x000041E0
	public static void Load<T>(string assetPath, Action<T> onLoad, Action<string> onLoadError = null, bool isAsyncInBackGround = false) where T : Object
	{
		bool flag = !Application.isPlaying;
		if (!flag)
		{
			ResLoader loader = SingletonObject.getInstance<ResLoader>();
			bool flag2 = loader == null;
			if (flag2)
			{
				if (onLoadError != null)
				{
					onLoadError(assetPath);
				}
			}
			else if (isAsyncInBackGround)
			{
				loader.LoadAssetBundleAsyncInBackground<T>(assetPath, onLoad, onLoadError);
			}
			else
			{
				loader.LoadAssetBundle<T>(assetPath, onLoad, onLoadError);
			}
		}
	}

	// Token: 0x060000DD RID: 221 RVA: 0x0000603C File Offset: 0x0000423C
	public static T SyncLoad<T>(string assetPath) where T : Object
	{
		bool flag = !Application.isPlaying;
		T result;
		if (flag)
		{
			result = default(T);
		}
		else
		{
			ResLoader loader = SingletonObject.getInstance<ResLoader>();
			bool flag2 = loader == null;
			if (flag2)
			{
				result = default(T);
			}
			else
			{
				result = loader.InternalSyncLoad<T>(assetPath);
			}
		}
		return result;
	}

	// Token: 0x060000DE RID: 222 RVA: 0x0000608C File Offset: 0x0000428C
	public static void LoadByName<T>(string name, Action<T> onLoad, Action<string> onLoadError = null) where T : Object
	{
		bool flag = !Application.isPlaying;
		if (!flag)
		{
			ResLoader loader = SingletonObject.getInstance<ResLoader>();
			bool flag2 = loader == null;
			if (flag2)
			{
				if (onLoadError != null)
				{
					onLoadError(name);
				}
			}
			else
			{
				loader.LoadAssetBundleByAssetName<T>(name, onLoad, onLoadError);
			}
		}
	}

	// Token: 0x060000DF RID: 223 RVA: 0x000060D4 File Offset: 0x000042D4
	private void InternalLoadRes<T>(string aPath, Action<T> onLoad, bool aQuickLoad = false, Action<string> onLoadError = null) where T : Object
	{
		bool flag = ResLoader._mLoadQueue == null;
		if (flag)
		{
			SingletonObject.getInstance<ResLoader>();
		}
		bool flag2 = onLoadError == null;
		if (flag2)
		{
			onLoadError = delegate(string path)
			{
				GLog.Error("Failed to load Resource " + path);
			};
		}
		LoadRequest request = new LoadRequest();
		request.Path = aPath;
		request.BRealLoad = false;
		request.IsBundle = false;
		request.OnLoadFinish = delegate(Object obj)
		{
			Action<T> onLoad2 = onLoad;
			if (onLoad2 != null)
			{
				onLoad2(obj as T);
			}
			bool brealLoad = request.BRealLoad;
			if (brealLoad)
			{
				this.AddLoadedCache(request.Path, obj);
			}
		};
		request.OnLoadError = onLoadError;
		request.ResType = typeof(T);
		ResLoader._mLoadQueue.Enqueue(request);
	}

	// Token: 0x060000E0 RID: 224 RVA: 0x000061B0 File Offset: 0x000043B0
	private T InternalSyncLoad<T>(string assetPath) where T : Object
	{
		List<string> dependenceList = EasyPool.Get<List<string>>();
		string bundleFilePath = string.Empty;
		ResourcePackage package = null;
		for (int i = this._resPackageList.Count - 1; i >= 0; i--)
		{
			ValueTuple<ResourcePackage, string, Object> tuple = this._resPackageList[i].TryGetAssetBundleLoadData(typeof(T), dependenceList, assetPath.PathFix(), "");
			bool flag = null != tuple.Item3;
			if (flag)
			{
				return tuple.Item3 as T;
			}
			bool flag2 = tuple.Item1 != null;
			if (flag2)
			{
				package = tuple.Item1;
				bundleFilePath = tuple.Item2;
				break;
			}
		}
		bool flag3 = bundleFilePath.IsNullOrEmpty();
		if (flag3)
		{
			this.OnResourceLoadErrorDefault(assetPath);
			return default(T);
		}
		bool flag4 = dependenceList.Count > 0;
		if (flag4)
		{
			foreach (string deBundlePath in dependenceList)
			{
				string filePath = deBundlePath;
				bool existFlag = File.Exists(filePath);
				ResourcePackage dependencyResourcePackage = package;
				bool flag5 = !existFlag;
				if (flag5)
				{
					foreach (ResourcePackage resourcePackage in this._resPackageList)
					{
						bool flag6 = existFlag;
						if (flag6)
						{
							break;
						}
						filePath = Path.Combine(resourcePackage.GetDirectory(), Path.GetFileName(filePath));
						existFlag = File.Exists(filePath);
						dependencyResourcePackage = resourcePackage;
					}
					bool flag7 = !existFlag;
					if (flag7)
					{
						this.OnResourceLoadErrorDefault(filePath);
						continue;
					}
				}
				dependencyResourcePackage.AddCache(AssetBundle.LoadFromFile(filePath));
			}
		}
		AssetBundle assetBundle = AssetBundle.LoadFromFile(bundleFilePath);
		bool flag8 = null != assetBundle;
		if (flag8)
		{
			if (package != null)
			{
				package.AddCache(assetBundle);
			}
			return assetBundle.LoadAsset<T>(Path.GetFileNameWithoutExtension(assetPath));
		}
		this.OnResourceLoadErrorDefault(assetPath);
		return default(T);
	}

	// Token: 0x060000E1 RID: 225 RVA: 0x000063EC File Offset: 0x000045EC
	private void LoadAssetBundle<T>(string assetPath, Action<T> onLoad, Action<string> onLoadError = null) where T : Object
	{
		bool flag = ResLoader._mLoadQueue == null;
		if (flag)
		{
			SingletonObject.getInstance<ResLoader>();
		}
		bool flag2 = onLoadError == null;
		if (flag2)
		{
			onLoadError = new Action<string>(this.OnResourceLoadErrorDefault);
		}
		List<string> dependenceList = EasyPool.Get<List<string>>();
		string bundleFilePath = string.Empty;
		ResourcePackage package = null;
		for (int i = this._resPackageList.Count - 1; i >= 0; i--)
		{
			ValueTuple<ResourcePackage, string, Object> tuple = this._resPackageList[i].TryGetAssetBundleLoadData(typeof(T), dependenceList, assetPath.PathFix(), "");
			bool flag3 = null != tuple.Item3;
			if (flag3)
			{
				Action<T> onLoad2 = onLoad;
				if (onLoad2 != null)
				{
					onLoad2(tuple.Item3 as T);
				}
				return;
			}
			bool flag4 = tuple.Item1 != null;
			if (flag4)
			{
				package = tuple.Item1;
				bundleFilePath = tuple.Item2;
				break;
			}
		}
		bool flag5 = bundleFilePath.IsNullOrEmpty();
		if (flag5)
		{
			AdaptableLog.TagError("ResLoader", "Failed to load resource at path:" + assetPath);
			return;
		}
		BundleLoadRequest request = new BundleLoadRequest();
		request.Path = bundleFilePath;
		request.Package = package;
		request.BundleName = Path.GetFileName(bundleFilePath);
		request.BundleKey = Path.GetFileNameWithoutExtension(assetPath);
		request.IsBundle = true;
		request.BRealLoad = false;
		request.ResType = typeof(T);
		request.OnLoadError = onLoadError;
		request.OnLoadFinish = delegate(Object obj)
		{
			AssetBundle bundle = obj as AssetBundle;
			bool flag10 = null != bundle;
			if (flag10)
			{
				bool flag11 = request.ResType == typeof(AssetBundle);
				if (flag11)
				{
					Action<T> onLoad3 = onLoad;
					if (onLoad3 != null)
					{
						onLoad3(obj as T);
					}
				}
				else
				{
					T tObj = bundle.LoadAsset<T>(request.BundleKey);
					Action<T> onLoad4 = onLoad;
					if (onLoad4 != null)
					{
						onLoad4(tObj);
					}
				}
			}
			else
			{
				onLoadError(bundleFilePath);
			}
		};
		bool flag6 = dependenceList.Count > 0;
		if (flag6)
		{
			request.DependenceCount = (byte)dependenceList.Count;
			request.OnAllDependenceLoaded = delegate()
			{
				ResLoader._mLoadQueue.Enqueue(request);
			};
			foreach (string deBundlePath in dependenceList)
			{
				BundleLoadRequest deBundleRequest = new BundleLoadRequest();
				string filePath = deBundlePath;
				bool existFlag = File.Exists(filePath);
				bool flag7 = !existFlag;
				if (flag7)
				{
					foreach (ResourcePackage resourcePackage in this._resPackageList)
					{
						bool flag8 = existFlag;
						if (flag8)
						{
							break;
						}
						filePath = Path.Combine(resourcePackage.GetDirectory(), Path.GetFileName(filePath));
						existFlag = File.Exists(filePath);
						package = resourcePackage;
					}
					bool flag9 = !existFlag;
					if (flag9)
					{
						onLoadError(filePath);
						continue;
					}
				}
				deBundleRequest.Path = filePath;
				deBundleRequest.Package = package;
				deBundleRequest.BundleName = Path.GetFileName(filePath);
				deBundleRequest.BundleKey = Path.GetFileNameWithoutExtension(filePath);
				deBundleRequest.IsBundle = true;
				deBundleRequest.BRealLoad = false;
				deBundleRequest.OnLoadError = onLoadError;
				deBundleRequest.OnLoadFinish = new Action<Object>(request.OnDependenceLoaded);
				ResLoader._mLoadQueue.Enqueue(deBundleRequest);
			}
		}
		else
		{
			ResLoader._mLoadQueue.Enqueue(request);
		}
		this.ContinueLoad(null);
	}

	// Token: 0x060000E2 RID: 226 RVA: 0x000067B0 File Offset: 0x000049B0
	private void LoadAssetBundleByAssetName<T>(string assetName, Action<T> onLoad, Action<string> onLoadError = null) where T : Object
	{
		bool flag = ResLoader._mLoadQueue == null;
		if (flag)
		{
			SingletonObject.getInstance<ResLoader>();
		}
		bool flag2 = onLoadError == null;
		if (flag2)
		{
			onLoadError = new Action<string>(this.OnResourceLoadErrorDefault);
		}
		List<string> dependenceList = EasyPool.Get<List<string>>();
		string bundleFilePath = string.Empty;
		ResourcePackage package = null;
		for (int i = this._resPackageList.Count - 1; i >= 0; i--)
		{
			ValueTuple<ResourcePackage, string, Object> tuple = this._resPackageList[i].TryGetAssetBundleLoadData(typeof(T), dependenceList, string.Empty, assetName);
			bool flag3 = null != tuple.Item3;
			if (flag3)
			{
				Action<T> onLoad2 = onLoad;
				if (onLoad2 != null)
				{
					onLoad2(tuple.Item3 as T);
				}
				return;
			}
			bool flag4 = tuple.Item1 != null;
			if (flag4)
			{
				package = tuple.Item1;
				bundleFilePath = tuple.Item2;
				break;
			}
		}
		bool flag5 = bundleFilePath.IsNullOrEmpty();
		if (flag5)
		{
			AdaptableLog.TagError("ResLoader", "Failed to load resource with name:" + assetName);
			return;
		}
		BundleLoadRequest request = new BundleLoadRequest();
		request.Path = bundleFilePath;
		request.Package = package;
		request.BundleName = Path.GetFileName(bundleFilePath);
		request.BundleKey = assetName;
		request.IsBundle = true;
		request.BRealLoad = false;
		request.ResType = typeof(T);
		request.OnLoadError = onLoadError;
		request.OnLoadFinish = delegate(Object obj)
		{
			AssetBundle bundle = obj as AssetBundle;
			bool flag9 = null != bundle;
			if (flag9)
			{
				bool flag10 = request.ResType == typeof(AssetBundle);
				if (flag10)
				{
					Action<T> onLoad3 = onLoad;
					if (onLoad3 != null)
					{
						onLoad3(obj as T);
					}
				}
				else
				{
					T tObj = bundle.LoadAsset<T>(request.BundleKey);
					Action<T> onLoad4 = onLoad;
					if (onLoad4 != null)
					{
						onLoad4(tObj);
					}
				}
			}
			else
			{
				onLoadError(bundleFilePath);
			}
		};
		bool flag6 = dependenceList.Count > 0;
		if (flag6)
		{
			request.DependenceCount = (byte)dependenceList.Count;
			request.OnAllDependenceLoaded = delegate()
			{
				ResLoader._mLoadQueue.Enqueue(request);
			};
			foreach (string deBundlePath in dependenceList)
			{
				BundleLoadRequest deBundleRequest = new BundleLoadRequest();
				string filePath = deBundlePath;
				bool flag7 = !File.Exists(filePath);
				if (flag7)
				{
					filePath = Path.Combine(this._resPackageList[0].GetDirectory(), Path.GetFileName(filePath));
					bool flag8 = !File.Exists(filePath);
					if (flag8)
					{
						onLoadError(filePath);
						continue;
					}
					package = this._resPackageList[0];
				}
				deBundleRequest.Path = filePath;
				deBundleRequest.Package = package;
				deBundleRequest.BundleName = Path.GetFileName(filePath);
				deBundleRequest.BundleKey = Path.GetFileNameWithoutExtension(filePath);
				deBundleRequest.IsBundle = true;
				deBundleRequest.BRealLoad = false;
				deBundleRequest.OnLoadError = onLoadError;
				deBundleRequest.OnLoadFinish = new Action<Object>(request.OnDependenceLoaded);
				ResLoader._mLoadQueue.Enqueue(deBundleRequest);
			}
		}
		else
		{
			ResLoader._mLoadQueue.Enqueue(request);
		}
		this.ContinueLoad(null);
	}

	// Token: 0x060000E3 RID: 227 RVA: 0x00006B14 File Offset: 0x00004D14
	public void Init()
	{
		this._resPackageList = new List<ResourcePackage>();
		ResLoader._mLoadQueue = new Queue<LoadRequest>();
		this._mRobotList = new List<LoadRobot>();
		for (int i = 0; i < 5; i++)
		{
			GameObject robotObj = new GameObject("LoadRobot_" + i.ToString());
			robotObj.transform.SetParent(base.transform, false);
			LoadRobot robot = robotObj.AddComponent<LoadRobot>();
			this._mRobotList.Add(robot);
		}
		string path = Path.Combine(Application.dataPath, "GameResources");
		ResourcePackage package = this.AddResourcePackage(path, "assetbundles");
		package.PackageName = "ConchShipTaiwuRes";
		package.SortingOrder = int.MinValue;
		package.StaticBundles.AddRange(this._ConchShipStaticAssetBundles);
		this.RegisterPreSpinePackage();
	}

	// Token: 0x060000E4 RID: 228 RVA: 0x00006BE4 File Offset: 0x00004DE4
	public void Dispose()
	{
		ResLoader._mLoadQueue.Clear();
		ResLoader._mLoadQueue = null;
		this._mRobotList.ForEach(delegate(LoadRobot robot)
		{
			Object.Destroy(robot.gameObject);
		});
		this.UnloadAllCachedAssets();
	}

	// Token: 0x060000E5 RID: 229 RVA: 0x00006C38 File Offset: 0x00004E38
	private void UnloadAllCachedAssets()
	{
		this._loadedCache.Clear();
		for (int i = this._resPackageList.Count - 1; i >= 0; i--)
		{
			this._resPackageList[i].ClearCache();
		}
	}

	// Token: 0x060000E6 RID: 230 RVA: 0x00006C88 File Offset: 0x00004E88
	public void UnloadTargetBundle(AssetBundle bundle)
	{
		for (int i = this._resPackageList.Count - 1; i >= 0; i--)
		{
			this._resPackageList[i].UnloadBundleAndRemoveCache(bundle);
		}
	}

	// Token: 0x060000E7 RID: 231 RVA: 0x00006CCC File Offset: 0x00004ECC
	public int GetCachedAssetsCount()
	{
		return this._loadedCache.Count;
	}

	// Token: 0x060000E8 RID: 232 RVA: 0x00006CE9 File Offset: 0x00004EE9
	private void OnResourceLoadErrorDefault(string path)
	{
		GLog.Error("Failed to load Resource " + path);
	}

	// Token: 0x060000E9 RID: 233 RVA: 0x00006D00 File Offset: 0x00004F00
	private void AddLoadedCache(string key, Object cacheObj)
	{
		bool flag = this._loadedCache.ContainsKey(key) && null != this._loadedCache[key];
		if (flag)
		{
			GLog.TagWarn("ResLoader", key + " has been multi loaded!", Array.Empty<object>());
		}
		else
		{
			this._loadedCache.Add(key, cacheObj);
		}
	}

	// Token: 0x060000EA RID: 234 RVA: 0x00006D64 File Offset: 0x00004F64
	private Object GetLoadedCache(string key)
	{
		Object cacheObj;
		this._loadedCache.TryGetValue(key, out cacheObj);
		return cacheObj;
	}

	// Token: 0x060000EB RID: 235 RVA: 0x00006D88 File Offset: 0x00004F88
	private LoadRobot GetRobot()
	{
		foreach (LoadRobot ro in this._mRobotList)
		{
			bool isFree = ro.IsFree;
			if (isFree)
			{
				return ro;
			}
		}
		return null;
	}

	// Token: 0x060000EC RID: 236 RVA: 0x00006DEC File Offset: 0x00004FEC
	private bool ContinueLoad(LoadRobot robot = null)
	{
		bool flag = ResLoader._mLoadQueue.Count <= 0;
		bool result;
		if (flag)
		{
			result = false;
		}
		else
		{
			bool flag2 = null == robot;
			if (flag2)
			{
				robot = this.GetRobot();
			}
			bool flag3 = null == robot;
			if (flag3)
			{
				result = false;
			}
			else
			{
				LoadRequest request = ResLoader._mLoadQueue.Dequeue();
				bool flag4 = request == null;
				if (flag4)
				{
					result = false;
				}
				else
				{
					bool isBundle = request.IsBundle;
					if (isBundle)
					{
						BundleLoadRequest bundleRequest = request as BundleLoadRequest;
						IEnumerable loadedBundles = AssetBundle.GetAllLoadedAssetBundles();
						foreach (object obj in loadedBundles)
						{
							AssetBundle bundle = (AssetBundle)obj;
							bool flag5 = bundle.name == bundleRequest.BundleName;
							if (flag5)
							{
								Action<Object> onLoadFinish = bundleRequest.OnLoadFinish;
								if (onLoadFinish != null)
								{
									onLoadFinish(bundle);
								}
								return true;
							}
						}
					}
					else
					{
						Object cache = this.GetLoadedCache(request.Path);
						bool flag6 = null != cache;
						if (flag6)
						{
							Action<Object> onLoadFinish2 = request.OnLoadFinish;
							if (onLoadFinish2 != null)
							{
								onLoadFinish2(cache);
							}
							return true;
						}
					}
					robot.Load(request);
					result = true;
				}
			}
		}
		return result;
	}

	// Token: 0x060000ED RID: 237 RVA: 0x00006F40 File Offset: 0x00005140
	private void RegisterPreSpinePackage()
	{
		string preSpineDir = Path.Combine(Application.streamingAssetsPath, "PreSpine");
		bool flag = !Directory.Exists(preSpineDir);
		if (flag)
		{
			Debug.Log("PreSpine directory not found, skipping: " + preSpineDir);
		}
		else
		{
			string manifestPath = Path.Combine(preSpineDir, "PreSpine");
			bool flag2 = !File.Exists(manifestPath);
			if (flag2)
			{
				Debug.LogWarning("PreSpine manifest not found: " + manifestPath);
			}
			else
			{
				AssetBundle manifestBundle = AssetBundle.LoadFromFile(manifestPath);
				bool flag3 = manifestBundle == null;
				if (flag3)
				{
					Debug.LogWarning("Failed to load PreSpine manifest bundle: " + manifestPath);
				}
				else
				{
					AssetBundleManifest manifest = manifestBundle.LoadAsset<AssetBundleManifest>("AssetBundleManifest");
					string controllerFilePath = Path.Combine(preSpineDir, "packagecontroller.uab");
					bool flag4 = !File.Exists(controllerFilePath);
					if (flag4)
					{
						Debug.LogWarning("PreSpine controller not found: " + controllerFilePath);
						manifestBundle.Unload(true);
					}
					else
					{
						AssetBundle controllerBundle = AssetBundle.LoadFromFile(controllerFilePath);
						PackageAssetBundleController controller = controllerBundle.LoadAsset<PackageAssetBundleController>("PackageAssetBundleController");
						controller.Init();
						controllerBundle.Unload(false);
						ResourcePackage package = new ResourcePackage(preSpineDir, manifest, controller);
						this._resPackageList.Add(package);
						Debug.Log("PreSpine package registered from: " + preSpineDir);
					}
				}
			}
		}
	}

	// Token: 0x060000EE RID: 238 RVA: 0x0000707C File Offset: 0x0000527C
	public ResourcePackage AddDlcResourcePackage(string dlcName, string packageResRootDir)
	{
		string controllerFilePath = Path.Combine(packageResRootDir, dlcName + "_PackageController" + FrameCommon.AbSuffix).ToLower();
		bool flag = string.IsNullOrEmpty(controllerFilePath);
		ResourcePackage result;
		if (flag)
		{
			Debug.LogError("can not find controller file from " + packageResRootDir + ", dlc resource package load failed...");
			result = null;
		}
		else
		{
			AssetBundle controllerBundle = AssetBundle.LoadFromFile(controllerFilePath);
			PackageAssetBundleController controller = controllerBundle.LoadAsset<PackageAssetBundleController>("PackageAssetBundleController");
			controller.Init();
			controllerBundle.Unload(false);
			ResourcePackage package = new ResourcePackage(packageResRootDir, this._resPackageList[0].GetAssetBundleManifest(), controller);
			this._resPackageList.Add(package);
			result = package;
		}
		return result;
	}

	// Token: 0x060000EF RID: 239 RVA: 0x00007120 File Offset: 0x00005320
	public ResourcePackage AddResourcePackage(string packageResRootDir, string manifestFileName)
	{
		string manifestFilePath = Path.Combine(packageResRootDir, manifestFileName);
		AssetBundle manifestBundle = AssetBundle.LoadFromFile(manifestFilePath);
		AssetBundleManifest manifest = manifestBundle.LoadAsset<AssetBundleManifest>("AssetBundleManifest");
		string controllerFilePath = Path.Combine(packageResRootDir, "packagecontroller.uab");
		AssetBundle controllerBundle = AssetBundle.LoadFromFile(controllerFilePath);
		PackageAssetBundleController controller = controllerBundle.LoadAsset<PackageAssetBundleController>("PackageAssetBundleController");
		controller.Init();
		controllerBundle.Unload(false);
		ResourcePackage package = new ResourcePackage(packageResRootDir, manifest, controller);
		this._resPackageList.Add(package);
		return package;
	}

	// Token: 0x060000F0 RID: 240 RVA: 0x000071A0 File Offset: 0x000053A0
	private void Update()
	{
		bool flag = ResLoader._mLoadQueue.Count > 0;
		if (flag)
		{
			while (this.ContinueLoad(null))
			{
			}
		}
	}

	// Token: 0x060000F1 RID: 241 RVA: 0x000071D0 File Offset: 0x000053D0
	private void LateUpdate()
	{
		this.ActionQueue.Update();
	}

	// Token: 0x060000F2 RID: 242 RVA: 0x000071E0 File Offset: 0x000053E0
	private void LoadAssetBundleAsyncInBackground<T>(string assetPath, Action<T> onLoad, Action<string> onLoadError = null) where T : Object
	{
		bool flag = ResLoader._mLoadQueue == null;
		if (flag)
		{
			SingletonObject.getInstance<ResLoader>();
		}
		bool flag2 = onLoadError == null;
		if (flag2)
		{
			onLoadError = new Action<string>(this.OnResourceLoadErrorDefault);
		}
		this.EnqueueAsyncLoad<T>(assetPath, typeof(T), onLoad, onLoadError);
	}

	// Token: 0x060000F3 RID: 243 RVA: 0x0000722C File Offset: 0x0000542C
	private void EnqueueAsyncLoad<T>(string assetPath, Type type, Action<T> onLoad, Action<string> onLoadError = null) where T : Object
	{
		List<string> dependenceList = EasyPool.Get<List<string>>();
		string bundleFilePath = string.Empty;
		ResourcePackage package = null;
		for (int i = this._resPackageList.Count - 1; i >= 0; i--)
		{
			ValueTuple<ResourcePackage, string, AssetBundle> tuple = this._resPackageList[i].TryGetAssetBundle(typeof(T), dependenceList, assetPath.PathFix(), "");
			bool flag = null != tuple.Item3;
			if (flag)
			{
				bool flag2 = tuple.Item3 is T;
				if (flag2)
				{
					this.ActionQueue.Enqueue(delegate
					{
						Action<T> onLoad2 = onLoad;
						if (onLoad2 != null)
						{
							onLoad2(tuple.Item3 as T);
						}
					});
				}
				else
				{
					this.ActionQueue.Enqueue(delegate
					{
						AssetBundleRequest asyncOp = tuple.Item3.LoadAssetAsync<T>(Path.GetFileNameWithoutExtension(assetPath));
						Action <>9__6;
						asyncOp.completed += delegate(AsyncOperation op)
						{
							FrameSplitActionQueue actionQueue = this.ActionQueue;
							Action action;
							if ((action = <>9__6) == null)
							{
								action = (<>9__6 = delegate()
								{
									T tObj = asyncOp.asset as T;
									bool flag9 = null != tObj;
									if (flag9)
									{
										Action<T> onLoad2 = onLoad;
										if (onLoad2 != null)
										{
											onLoad2(tObj);
										}
									}
									else
									{
										onLoadError(assetPath);
									}
								});
							}
							actionQueue.Enqueue(action);
						};
					});
				}
				return;
			}
			bool flag3 = tuple.Item1 != null;
			if (flag3)
			{
				package = tuple.Item1;
				bundleFilePath = tuple.Item2;
				break;
			}
		}
		bool flag4 = bundleFilePath.IsNullOrEmpty();
		if (flag4)
		{
			AdaptableLog.TagError("ResLoader", "Failed to load resource at path:" + assetPath);
			return;
		}
		BundleLoadRequest request = new BundleLoadRequest();
		request.IsAsyncLoad = true;
		request.Path = bundleFilePath;
		request.Package = package;
		request.BundleName = Path.GetFileName(bundleFilePath);
		request.BundleKey = Path.GetFileNameWithoutExtension(assetPath);
		request.IsBundle = true;
		request.BRealLoad = false;
		request.ResType = typeof(T);
		request.OnLoadError = onLoadError;
		request.OnLoadFinish = delegate(Object obj)
		{
			AssetBundle bundle = obj as AssetBundle;
			bool flag9 = null != bundle;
			if (flag9)
			{
				bool flag10 = request.ResType == typeof(AssetBundle);
				if (flag10)
				{
					this.ActionQueue.Enqueue(delegate
					{
						Action<T> onLoad2 = onLoad;
						if (onLoad2 != null)
						{
							onLoad2(bundle as T);
						}
					});
				}
				else
				{
					AssetBundleRequest asyncOp = bundle.LoadAssetAsync<T>(request.BundleKey);
					Action <>9__9;
					asyncOp.completed += delegate(AsyncOperation op)
					{
						FrameSplitActionQueue actionQueue = this.ActionQueue;
						Action action;
						if ((action = <>9__9) == null)
						{
							action = (<>9__9 = delegate()
							{
								T tObj = asyncOp.asset as T;
								bool flag11 = null != tObj;
								if (flag11)
								{
									Action<T> onLoad2 = onLoad;
									if (onLoad2 != null)
									{
										onLoad2(tObj);
									}
								}
								else
								{
									onLoadError(assetPath);
								}
							});
						}
						actionQueue.Enqueue(action);
					};
				}
			}
			else
			{
				onLoadError(bundleFilePath);
			}
		};
		bool flag5 = dependenceList.Count > 0;
		if (flag5)
		{
			request.DependenceCount = (byte)dependenceList.Count;
			Action <>9__10;
			request.OnAllDependenceLoaded = delegate()
			{
				FrameSplitActionQueue actionQueue = this.ActionQueue;
				Action action;
				if ((action = <>9__10) == null)
				{
					action = (<>9__10 = delegate()
					{
						new ResLoader.LoadRobotAsync(request).LoadAsync();
					});
				}
				actionQueue.Enqueue(action);
			};
			foreach (string deBundlePath in dependenceList)
			{
				BundleLoadRequest deBundleRequest = new BundleLoadRequest();
				deBundleRequest.IsAsyncLoad = true;
				string filePath = deBundlePath;
				bool existFlag = File.Exists(filePath);
				bool flag6 = !existFlag;
				if (flag6)
				{
					foreach (ResourcePackage resourcePackage in this._resPackageList)
					{
						bool flag7 = existFlag;
						if (flag7)
						{
							break;
						}
						filePath = Path.Combine(resourcePackage.GetDirectory(), Path.GetFileName(filePath));
						existFlag = File.Exists(filePath);
						package = resourcePackage;
					}
					bool flag8 = !existFlag;
					if (flag8)
					{
						onLoadError(filePath);
						continue;
					}
				}
				deBundleRequest.Path = filePath;
				deBundleRequest.Package = package;
				deBundleRequest.BundleName = Path.GetFileName(filePath);
				deBundleRequest.BundleKey = Path.GetFileNameWithoutExtension(filePath);
				deBundleRequest.IsBundle = true;
				deBundleRequest.BRealLoad = false;
				deBundleRequest.OnLoadError = onLoadError;
				deBundleRequest.OnLoadFinish = new Action<Object>(request.OnDependenceLoaded);
				this.ActionQueue.Enqueue(delegate
				{
					new ResLoader.LoadRobotAsync(deBundleRequest).LoadAsync();
				});
			}
			return;
		}
		this.ActionQueue.Enqueue(delegate
		{
			new ResLoader.LoadRobotAsync(request).LoadAsync();
		});
	}

	// Token: 0x04000095 RID: 149
	private static Queue<LoadRequest> _mLoadQueue;

	// Token: 0x04000096 RID: 150
	private List<LoadRobot> _mRobotList;

	// Token: 0x04000097 RID: 151
	private Dictionary<string, Object> _loadedCache = new Dictionary<string, Object>();

	// Token: 0x04000098 RID: 152
	private const int RobotCount = 5;

	// Token: 0x04000099 RID: 153
	private List<ResourcePackage> _resPackageList;

	// Token: 0x0400009A RID: 154
	private const string TaiwuResourcePackageKey = "ConchShipTaiwuRes";

	// Token: 0x0400009B RID: 155
	private const string PreSpineDirName = "PreSpine";

	// Token: 0x0400009C RID: 156
	private readonly List<string> _ConchShipStaticAssetBundles = new List<string>
	{
		"fonts.uab",
		"audioinfos.uab",
		"atlasinfos.uab"
	};

	// Token: 0x0400009D RID: 157
	public static readonly Dictionary<string, AssetBundleCreateRequest> LoadingBundleRequestMap = new Dictionary<string, AssetBundleCreateRequest>();

	// Token: 0x0400009E RID: 158
	public readonly FrameSplitActionQueue ActionQueue = new FrameSplitActionQueue
	{
		MaxActionCountPerFrame = 100,
		MaxActionExecuteDurationPerFrameMs = 10
	};

	// Token: 0x02001088 RID: 4232
	private struct LoadRobotAsync
	{
		// Token: 0x0600BFAD RID: 49069 RVA: 0x00569722 File Offset: 0x00567922
		public LoadRobotAsync(LoadRequest request)
		{
			this._loadingRequest = request;
		}

		// Token: 0x0600BFAE RID: 49070 RVA: 0x0056972C File Offset: 0x0056792C
		public void LoadAsync()
		{
			LoadRequest loadingReq = this._loadingRequest;
			bool flag = !loadingReq.IsBundle;
			if (flag)
			{
				ResourceRequest op = Resources.LoadAsync(loadingReq.Path, loadingReq.ResType);
				op.completed += delegate(AsyncOperation asyncOp)
				{
					Object asset = op.asset;
					bool flag4 = null != asset;
					if (flag4)
					{
						loadingReq.BRealLoad = true;
						loadingReq.OnLoadFinish(asset);
					}
					else
					{
						Action<string> onLoadError = loadingReq.OnLoadError;
						if (onLoadError != null)
						{
							onLoadError(loadingReq.Path);
						}
					}
					loadingReq = null;
				};
			}
			else
			{
				loadingReq.BRealLoad = true;
				AssetBundle cachedBundle = this.GetCachedBundle();
				bool flag2 = null != cachedBundle;
				if (flag2)
				{
					Action<Object> onLoadFinish = loadingReq.OnLoadFinish;
					if (onLoadFinish != null)
					{
						onLoadFinish(cachedBundle);
					}
					loadingReq = null;
				}
				else
				{
					string fixedPath = loadingReq.Path.PathFix();
					AssetBundleCreateRequest op;
					bool flag3 = !ResLoader.LoadingBundleRequestMap.TryGetValue(fixedPath, out op);
					if (flag3)
					{
						op = AssetBundle.LoadFromFileAsync(fixedPath);
						ResLoader.LoadingBundleRequestMap.Add(fixedPath, op);
					}
					op.completed += delegate(AsyncOperation asyncOp)
					{
						ResLoader.LoadingBundleRequestMap.Remove(fixedPath);
						AssetBundle targetBundle = op.assetBundle;
						bool flag4 = loadingReq.OnLoadFinish != null;
						if (flag4)
						{
							bool flag5 = null == targetBundle;
							if (flag5)
							{
								GLog.TagWarn("LoadRobot", loadingReq.Path + " error!", Array.Empty<object>());
							}
							else
							{
								BundleLoadRequest bundleLoadRequest = loadingReq as BundleLoadRequest;
								if (bundleLoadRequest != null)
								{
									ResourcePackage package = bundleLoadRequest.Package;
									if (package != null)
									{
										package.AddCache(targetBundle);
									}
								}
							}
							Action<Object> onLoadFinish2 = loadingReq.OnLoadFinish;
							if (onLoadFinish2 != null)
							{
								onLoadFinish2(targetBundle);
							}
						}
						loadingReq = null;
					};
				}
			}
		}

		// Token: 0x0600BFAF RID: 49071 RVA: 0x00569894 File Offset: 0x00567A94
		private AssetBundle GetCachedBundle()
		{
			BundleLoadRequest bundleLoadRequest = this._loadingRequest as BundleLoadRequest;
			bool flag = bundleLoadRequest != null;
			if (flag)
			{
				bool flag2 = bundleLoadRequest.Package == null;
				if (flag2)
				{
					return null;
				}
				AssetBundle bundle;
				bool flag3 = bundleLoadRequest.Package.TryGetCache(bundleLoadRequest.BundleName, out bundle);
				if (flag3)
				{
					return bundle;
				}
				string bundleName = bundleLoadRequest.BundleName;
				IEnumerable<AssetBundle> loadedBundles = AssetBundle.GetAllLoadedAssetBundles();
				foreach (AssetBundle item in loadedBundles)
				{
					bool flag4 = item.name == bundleName;
					if (flag4)
					{
						return item;
					}
				}
			}
			return null;
		}

		// Token: 0x04009391 RID: 37777
		private LoadRequest _loadingRequest;
	}
}
