using System;
using FrameWork.AssetBundlePackage;
using UnityEngine;

namespace FrameWork.ResManager
{
	// Token: 0x02001041 RID: 4161
	internal class LoadRobot : MonoBehaviour
	{
		// Token: 0x0600BDD2 RID: 48594 RVA: 0x00562F32 File Offset: 0x00561132
		public void Load(LoadRequest request)
		{
			this.IsFree = false;
			this._loadingRequest = request;
			this.internal_Load();
		}

		// Token: 0x0600BDD3 RID: 48595 RVA: 0x00562F4C File Offset: 0x0056114C
		public bool IsLoadingAsset(string path, bool isBundle)
		{
			return this._loadingRequest.Path.PathFix() == path.PathFix() && this._loadingRequest.IsBundle == isBundle;
		}

		// Token: 0x0600BDD4 RID: 48596 RVA: 0x00562F8C File Offset: 0x0056118C
		public void AppendLoadingFinishAction(Action<Object> finishAction)
		{
			bool flag = this._loadingRequest != null;
			if (flag)
			{
				LoadRequest loadingRequest = this._loadingRequest;
				loadingRequest.OnLoadFinish = (Action<Object>)Delegate.Combine(loadingRequest.OnLoadFinish, finishAction);
			}
		}

		// Token: 0x0600BDD5 RID: 48597 RVA: 0x00562FC4 File Offset: 0x005611C4
		private void internal_Load()
		{
			bool flag = !this._loadingRequest.IsBundle;
			if (flag)
			{
				Object asset = Resources.Load(this._loadingRequest.Path, this._loadingRequest.ResType);
				bool flag2 = null != asset;
				if (flag2)
				{
					this._loadingRequest.BRealLoad = true;
					this._loadingRequest.OnLoadFinish(asset);
				}
				else
				{
					Action<string> onLoadError = this._loadingRequest.OnLoadError;
					if (onLoadError != null)
					{
						onLoadError(this._loadingRequest.Path);
					}
				}
				this._loadingRequest = null;
				this.IsFree = true;
			}
			else
			{
				this._loadingRequest.BRealLoad = true;
				bool flag3 = ResLoader.LoadingBundleRequestMap.Count > 0;
				if (flag3)
				{
					string fixedPath = this._loadingRequest.Path.PathFix();
					AssetBundleCreateRequest loadingBundleRequest;
					bool flag4 = ResLoader.LoadingBundleRequestMap.TryGetValue(fixedPath, out loadingBundleRequest);
					if (flag4)
					{
						loadingBundleRequest.completed += delegate(AsyncOperation asyncOperation)
						{
							this.FinishLoad(loadingBundleRequest.assetBundle);
						};
						return;
					}
				}
				AssetBundle targetBundle = AssetBundle.LoadFromFile(this._loadingRequest.Path);
				this.FinishLoad(targetBundle);
			}
		}

		// Token: 0x0600BDD6 RID: 48598 RVA: 0x005630FC File Offset: 0x005612FC
		private void FinishLoad(AssetBundle targetBundle)
		{
			bool flag = this._loadingRequest.OnLoadFinish != null;
			if (flag)
			{
				bool flag2 = null == targetBundle;
				if (flag2)
				{
					GLog.TagWarn("LoadRobot", this._loadingRequest.Path + " error!", Array.Empty<object>());
				}
				else
				{
					BundleLoadRequest bundleLoadRequest = this._loadingRequest as BundleLoadRequest;
					if (bundleLoadRequest != null)
					{
						ResourcePackage package = bundleLoadRequest.Package;
						if (package != null)
						{
							package.AddCache(targetBundle);
						}
					}
				}
				Action<Object> onLoadFinish = this._loadingRequest.OnLoadFinish;
				if (onLoadFinish != null)
				{
					onLoadFinish(targetBundle);
				}
			}
			this._loadingRequest = null;
			this.IsFree = true;
		}

		// Token: 0x04009218 RID: 37400
		public bool IsFree = true;

		// Token: 0x04009219 RID: 37401
		private LoadRequest _loadingRequest;
	}
}
