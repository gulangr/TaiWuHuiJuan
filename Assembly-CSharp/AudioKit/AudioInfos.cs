using System;
using System.Collections.Generic;
using UnityEngine;

namespace AudioKit
{
	// Token: 0x02000FD7 RID: 4055
	public class AudioInfos : ScriptableObject
	{
		// Token: 0x170014F0 RID: 5360
		// (get) Token: 0x0600B95E RID: 47454 RVA: 0x005488D1 File Offset: 0x00546AD1
		// (set) Token: 0x0600B95F RID: 47455 RVA: 0x005488D8 File Offset: 0x00546AD8
		public static AudioInfos Instance { get; private set; }

		// Token: 0x0600B960 RID: 47456 RVA: 0x005488E0 File Offset: 0x00546AE0
		public static void Init()
		{
			ResLoader.Load<AudioInfos>("RemakeResources/Audios/AudioInfos", delegate(AudioInfos audioInfos)
			{
				AudioInfos.Instance = audioInfos;
				AudioInfos.Instance.InitSelf();
			}, null, false);
		}

		// Token: 0x0600B961 RID: 47457 RVA: 0x00548910 File Offset: 0x00546B10
		public void InitSelf()
		{
			this._musicInfosDic = new Dictionary<string, string>();
			foreach (MusicInfo info in this.MusicInfos)
			{
				this._musicInfosDic.Add(info.MusicName, info.MusicPath);
			}
			this._soundInfos = new Dictionary<string, SoundPackageDetail>();
			foreach (SoundPackageDetail packageDetail in this.SePackageInfos)
			{
				this._soundInfos.Add(packageDetail.PackageName, packageDetail);
			}
			this._loadedSoundPackage = new Dictionary<string, SoundPackage>();
		}

		// Token: 0x0600B962 RID: 47458 RVA: 0x005489EC File Offset: 0x00546BEC
		private void LoadPreloadPackage()
		{
			foreach (string packageName in this._preLoadPackages)
			{
				this.LoadPackage(packageName, null);
			}
		}

		// Token: 0x0600B963 RID: 47459 RVA: 0x00548A48 File Offset: 0x00546C48
		public bool HasClip(string clipName, bool isMusic)
		{
			bool result;
			if (isMusic)
			{
				Dictionary<string, string> musicInfosDic = this._musicInfosDic;
				result = (musicInfosDic != null && musicInfosDic.ContainsKey(clipName));
			}
			else
			{
				bool flag = this.SePackageInfos != null;
				if (flag)
				{
					int i = 0;
					int max = this.SePackageInfos.Count;
					while (i < max)
					{
						bool flag2 = this.SePackageInfos[i].SoundNames.Contains(clipName);
						if (flag2)
						{
							return true;
						}
						i++;
					}
				}
				result = false;
			}
			return result;
		}

		// Token: 0x0600B964 RID: 47460 RVA: 0x00548AC8 File Offset: 0x00546CC8
		public void LoadPackage(string packageName, Action onLoad)
		{
			bool flag = !this._loadedSoundPackage.ContainsKey(packageName);
			if (flag)
			{
				ResLoader.Load<SoundPackage>(this._audioPackageDir + packageName, delegate(SoundPackage package)
				{
					bool flag2 = !this._loadedSoundPackage.ContainsKey(packageName);
					if (flag2)
					{
						this._loadedSoundPackage.Add(packageName, package);
					}
					Action onLoad3 = onLoad;
					if (onLoad3 != null)
					{
						onLoad3();
					}
				}, null, false);
			}
			else
			{
				Action onLoad2 = onLoad;
				if (onLoad2 != null)
				{
					onLoad2();
				}
			}
		}

		// Token: 0x0600B965 RID: 47461 RVA: 0x00548B44 File Offset: 0x00546D44
		public void UnLoadPackage(string packageName)
		{
			bool flag = this._preLoadPackages.Contains(packageName);
			if (!flag)
			{
				bool flag2 = !packageName.IsNullOrEmpty();
				if (flag2)
				{
					this._loadedSoundPackage.Remove(packageName);
				}
			}
		}

		// Token: 0x0600B966 RID: 47462 RVA: 0x00548B80 File Offset: 0x00546D80
		public string GetClipPackageName(string clipName)
		{
			bool flag = !clipName.IsNullOrEmpty();
			if (flag)
			{
				foreach (KeyValuePair<string, SoundPackageDetail> pair in this._soundInfos)
				{
					bool flag2 = pair.Value.SoundNames.Contains(clipName);
					if (flag2)
					{
						return pair.Key;
					}
				}
			}
			return string.Empty;
		}

		// Token: 0x0600B967 RID: 47463 RVA: 0x00548C0C File Offset: 0x00546E0C
		public AudioClip GetClip(string clipName, string packageName = "")
		{
			SoundPackage package = null;
			bool flag = !packageName.IsNullOrEmpty() && this._soundInfos.ContainsKey(packageName);
			if (flag)
			{
				bool flag2 = this._soundInfos[packageName].SoundNames.Contains(clipName);
				if (flag2)
				{
					this._loadedSoundPackage.TryGetValue(packageName, out package);
				}
			}
			bool flag3 = null != package;
			AudioClip result;
			if (flag3)
			{
				result = package.SeClips[package.SeNames.IndexOf(clipName)];
			}
			else
			{
				foreach (KeyValuePair<string, SoundPackage> pair in this._loadedSoundPackage)
				{
					bool flag4 = pair.Value.SeNames.Contains(clipName);
					if (flag4)
					{
						return pair.Value.SeClips[pair.Value.SeNames.IndexOf(clipName)];
					}
				}
				result = null;
			}
			return result;
		}

		// Token: 0x0600B968 RID: 47464 RVA: 0x00548D18 File Offset: 0x00546F18
		public void GetMusic(string musicName, Action<AudioClip> onGetMusic, Action onFailed = null)
		{
			bool flag = musicName.IsNullOrEmpty();
			if (!flag)
			{
				string musicPath;
				bool flag2 = this._musicInfosDic.TryGetValue(musicName, out musicPath);
				if (flag2)
				{
					ResLoader.Load<AudioClip>(musicPath, delegate(AudioClip clip)
					{
						Action<AudioClip> onGetMusic2 = onGetMusic;
						if (onGetMusic2 != null)
						{
							onGetMusic2(clip);
						}
					}, delegate(string error)
					{
						Action onFailed3 = onFailed;
						if (onFailed3 != null)
						{
							onFailed3();
						}
					}, false);
				}
				else
				{
					Action onFailed2 = onFailed;
					if (onFailed2 != null)
					{
						onFailed2();
					}
				}
			}
		}

		// Token: 0x0600B969 RID: 47465 RVA: 0x00548D90 File Offset: 0x00546F90
		public float GetMusicLength(string musicName)
		{
			bool flag = musicName.IsNullOrEmpty();
			float result;
			if (flag)
			{
				result = 0f;
			}
			else
			{
				MusicInfo musicInfo = this.MusicInfos.Find((MusicInfo m) => m.MusicName == musicName);
				result = musicInfo.Length;
			}
			return result;
		}

		// Token: 0x04008F86 RID: 36742
		public List<MusicInfo> MusicInfos;

		// Token: 0x04008F87 RID: 36743
		public List<SoundPackageDetail> SePackageInfos;

		// Token: 0x04008F89 RID: 36745
		private Dictionary<string, string> _musicInfosDic;

		// Token: 0x04008F8A RID: 36746
		private Dictionary<string, SoundPackageDetail> _soundInfos;

		// Token: 0x04008F8B RID: 36747
		private Dictionary<string, SoundPackage> _loadedSoundPackage;

		// Token: 0x04008F8C RID: 36748
		private const string AudioInfosFileResPath = "RemakeResources/Audios/AudioInfos";

		// Token: 0x04008F8D RID: 36749
		private string _audioPackageDir = "RemakeResources/Audios/Packages/";

		// Token: 0x04008F8E RID: 36750
		private readonly List<string> _preLoadPackages = new List<string>();
	}
}
