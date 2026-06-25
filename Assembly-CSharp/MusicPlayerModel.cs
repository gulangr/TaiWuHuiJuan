using System;
using System.Collections.Generic;
using System.Linq;
using AudioKit;
using Config;
using FrameWork;
using GameData.Common;
using GameData.GameDataBridge;
using GameData.Serializer;

// Token: 0x0200013B RID: 315
public class MusicPlayerModel : ISingletonInit, IDisposable
{
	// Token: 0x170001CC RID: 460
	// (get) Token: 0x0600108A RID: 4234 RVA: 0x0006304F File Offset: 0x0006124F
	// (set) Token: 0x0600108B RID: 4235 RVA: 0x00063057 File Offset: 0x00061257
	public bool IsPaused { get; private set; }

	// Token: 0x170001CD RID: 461
	// (get) Token: 0x0600108C RID: 4236 RVA: 0x00063060 File Offset: 0x00061260
	// (set) Token: 0x0600108D RID: 4237 RVA: 0x00063068 File Offset: 0x00061268
	private bool IsMainStoryControlBgm { get; set; }

	// Token: 0x170001CE RID: 462
	// (get) Token: 0x0600108E RID: 4238 RVA: 0x00063071 File Offset: 0x00061271
	public bool Interactable
	{
		get
		{
			return this._interactable && !this.IsMainStoryControlBgm;
		}
	}

	// Token: 0x170001CF RID: 463
	// (get) Token: 0x0600108F RID: 4239 RVA: 0x00063087 File Offset: 0x00061287
	public MusicItem MusicConfig
	{
		get
		{
			return Music.Instance[this.MusicId];
		}
	}

	// Token: 0x170001D0 RID: 464
	// (get) Token: 0x06001090 RID: 4240 RVA: 0x0006309C File Offset: 0x0006129C
	public bool CanShow
	{
		get
		{
			bool uiCondition = UIManager.Instance.IsFocusElement(UIElement.StateMainWorld) || UIManager.Instance.IsFocusElement(UIElement.StatePartWorldMap) || UIManager.Instance.IsFocusElement(UIElement.StateBuilding) || UIManager.Instance.IsFocusElement(UIElement.TaskPanelTopGroup) || UIManager.Instance.IsFocusElement(UIElement.StateAdventureRemake) || UIManager.Instance.IsFocusElement(UIElement.StateAdventureRemakeSpecialBottom);
			bool inGuiding = SingletonObject.getInstance<TutorialChapterModel>().InGuiding;
			return uiCondition && this.IsEnabled && !inGuiding;
		}
	}

	// Token: 0x170001D1 RID: 465
	// (get) Token: 0x06001091 RID: 4241 RVA: 0x00063133 File Offset: 0x00061333
	// (set) Token: 0x06001092 RID: 4242 RVA: 0x0006313B File Offset: 0x0006133B
	public float MaxTime { get; private set; }

	// Token: 0x170001D2 RID: 466
	// (get) Token: 0x06001093 RID: 4243 RVA: 0x00063144 File Offset: 0x00061344
	public float CurTime
	{
		get
		{
			return AudioManager.Instance.GetMusicPlayerProgress();
		}
	}

	// Token: 0x170001D3 RID: 467
	// (get) Token: 0x06001094 RID: 4244 RVA: 0x00063150 File Offset: 0x00061350
	public List<short> UnlockedMusicList
	{
		get
		{
			return this._unlockedMusicList;
		}
	}

	// Token: 0x170001D4 RID: 468
	// (get) Token: 0x06001095 RID: 4245 RVA: 0x00063158 File Offset: 0x00061358
	// (set) Token: 0x06001096 RID: 4246 RVA: 0x00063160 File Offset: 0x00061360
	public bool IsEnabled
	{
		get
		{
			return this._isEnabled;
		}
		private set
		{
			this._isEnabled = value;
			GameDataBridge.AddDataModification<bool>(19, 69, ulong.MaxValue, uint.MaxValue, this._isEnabled);
		}
	}

	// Token: 0x170001D5 RID: 469
	// (get) Token: 0x06001097 RID: 4247 RVA: 0x0006317D File Offset: 0x0006137D
	// (set) Token: 0x06001098 RID: 4248 RVA: 0x00063188 File Offset: 0x00061388
	public MusicPlayerModel.MusicPlayMode PlayMode
	{
		get
		{
			return this._playMode;
		}
		set
		{
			this._playMode = value;
			GameDataBridge.AddDataModification<sbyte>(19, 67, ulong.MaxValue, uint.MaxValue, this._playMode.ToSbyte());
			bool flag = this._playMode == MusicPlayerModel.MusicPlayMode.Random;
			if (flag)
			{
				this._unlockedMusicRandomList.Shuffle(1);
			}
		}
	}

	// Token: 0x170001D6 RID: 470
	// (get) Token: 0x06001099 RID: 4249 RVA: 0x000631D4 File Offset: 0x000613D4
	// (set) Token: 0x0600109A RID: 4250 RVA: 0x000631DC File Offset: 0x000613DC
	public short MusicId
	{
		get
		{
			return this._musicId;
		}
		private set
		{
			this._musicId = value;
			GameDataBridge.AddDataModification<short>(19, 68, ulong.MaxValue, uint.MaxValue, this._musicId);
		}
	}

	// Token: 0x170001D7 RID: 471
	// (get) Token: 0x0600109B RID: 4251 RVA: 0x000631F9 File Offset: 0x000613F9
	// (set) Token: 0x0600109C RID: 4252 RVA: 0x00063201 File Offset: 0x00061401
	public bool IsPlaying
	{
		get
		{
			return this._isPlaying;
		}
		private set
		{
			this._isPlaying = value;
			GameDataBridge.AddDataModification<bool>(19, 207, ulong.MaxValue, uint.MaxValue, this._isPlaying);
		}
	}

	// Token: 0x170001D8 RID: 472
	// (get) Token: 0x0600109D RID: 4253 RVA: 0x00063221 File Offset: 0x00061421
	public List<short> EvaluatedMusicList
	{
		get
		{
			return this._evaluatedMusicList;
		}
	}

	// Token: 0x170001D9 RID: 473
	// (get) Token: 0x0600109E RID: 4254 RVA: 0x00063229 File Offset: 0x00061429
	public List<short> FavoriteMusicList
	{
		get
		{
			return this._favoriteMusicList;
		}
	}

	// Token: 0x0600109F RID: 4255 RVA: 0x00063234 File Offset: 0x00061434
	public void Init()
	{
		this._musicNameDict.Clear();
		this._musicDurationDict.Clear();
		foreach (MusicItem music in ((IEnumerable<MusicItem>)Music.Instance))
		{
			string musicName = string.Empty;
			bool flag = music.MapBlock > -1;
			if (flag)
			{
				musicName = MapBlock.Instance[music.MapBlock].Bgm;
			}
			else
			{
				bool flag2 = music.MapState > -1;
				if (flag2)
				{
					musicName = MapState.Instance[music.MapState].Bgm;
				}
			}
			this._musicNameDict[music.TemplateId] = musicName;
			float duration = AudioInfos.Instance.GetMusicLength(musicName);
			this._musicDurationDict[music.TemplateId] = duration;
		}
		this._gameDataListenerId = GameDataBridge.RegisterListener(new GameDataBridge.NotificationHandler(this.OnNotifyMapData));
		GameDataBridge.AddDataMonitor(this._gameDataListenerId, 19, 66, ulong.MaxValue, uint.MaxValue);
		GameDataBridge.AddDataMonitor(this._gameDataListenerId, 19, 69, ulong.MaxValue, uint.MaxValue);
		GameDataBridge.AddDataMonitor(this._gameDataListenerId, 19, 68, ulong.MaxValue, uint.MaxValue);
		GameDataBridge.AddDataMonitor(this._gameDataListenerId, 19, 67, ulong.MaxValue, uint.MaxValue);
		GameDataBridge.AddDataMonitor(this._gameDataListenerId, 19, 73, ulong.MaxValue, uint.MaxValue);
		GameDataBridge.AddDataMonitor(this._gameDataListenerId, 19, 206, ulong.MaxValue, uint.MaxValue);
		GameDataBridge.AddDataMonitor(this._gameDataListenerId, 19, 207, ulong.MaxValue, uint.MaxValue);
		GEvent.Add(UiEvents.TopUiChanged, new GEvent.Callback(this.OnTopUiChanged));
	}

	// Token: 0x060010A0 RID: 4256 RVA: 0x000633DC File Offset: 0x000615DC
	public void Dispose()
	{
		GameDataBridge.AddDataUnMonitor(this._gameDataListenerId, 19, 66, ulong.MaxValue, uint.MaxValue);
		GameDataBridge.AddDataUnMonitor(this._gameDataListenerId, 19, 69, ulong.MaxValue, uint.MaxValue);
		GameDataBridge.AddDataUnMonitor(this._gameDataListenerId, 19, 68, ulong.MaxValue, uint.MaxValue);
		GameDataBridge.AddDataUnMonitor(this._gameDataListenerId, 19, 67, ulong.MaxValue, uint.MaxValue);
		GameDataBridge.AddDataUnMonitor(this._gameDataListenerId, 19, 73, ulong.MaxValue, uint.MaxValue);
		GameDataBridge.AddDataUnMonitor(this._gameDataListenerId, 19, 206, ulong.MaxValue, uint.MaxValue);
		GameDataBridge.AddDataUnMonitor(this._gameDataListenerId, 19, 207, ulong.MaxValue, uint.MaxValue);
		GameDataBridge.UnregisterListener(this._gameDataListenerId);
		this._gameDataListenerId = -1;
		this._isEnabled = false;
		this._musicId = -1;
		this._playMode = MusicPlayerModel.MusicPlayMode.List;
		GEvent.Remove(UiEvents.TopUiChanged, new GEvent.Callback(this.OnTopUiChanged));
	}

	// Token: 0x060010A1 RID: 4257 RVA: 0x000634B8 File Offset: 0x000616B8
	private void OnNotifyMapData(List<NotificationWrapper> notifications)
	{
		foreach (NotificationWrapper wrapper in notifications)
		{
			Notification notification = wrapper.Notification;
			byte type = notification.Type;
			byte b = type;
			if (b == 0)
			{
				DataUid uid = notification.Uid;
				ushort domainId = uid.DomainId;
				ushort num = domainId;
				if (num == 19)
				{
					this.HandlerDataExtraDomain(uid, wrapper, notification);
				}
			}
		}
	}

	// Token: 0x060010A2 RID: 4258 RVA: 0x0006354C File Offset: 0x0006174C
	private void HandlerDataExtraDomain(DataUid uid, NotificationWrapper wrapper, Notification notification)
	{
		ushort dataId = uid.DataId;
		ushort num = dataId;
		switch (num)
		{
		case 66:
		{
			List<short> list = new List<short>();
			Serializer.Deserialize(wrapper.DataPool, notification.ValueOffset, ref list);
			list.Sort((short a, short b) => (a < b) ? -1 : 1);
			this._unlockedMusicList.Clear();
			this._unlockedMusicList.AddRange(list);
			this._unlockedMusicRandomList.Clear();
			this._unlockedMusicRandomList.AddRange(list);
			this._unlockedMusicRandomList.Shuffle(1);
			break;
		}
		case 67:
		{
			sbyte playMode = 0;
			Serializer.Deserialize(wrapper.DataPool, notification.ValueOffset, ref playMode);
			this._playMode = (MusicPlayerModel.MusicPlayMode)playMode;
			break;
		}
		case 68:
			Serializer.Deserialize(wrapper.DataPool, notification.ValueOffset, ref this._musicId);
			break;
		case 69:
			Serializer.Deserialize(wrapper.DataPool, notification.ValueOffset, ref this._isEnabled);
			break;
		case 70:
		case 71:
		case 72:
			break;
		case 73:
		{
			List<short> list2 = new List<short>();
			Serializer.Deserialize(wrapper.DataPool, notification.ValueOffset, ref list2);
			this._evaluatedMusicList.Clear();
			this._evaluatedMusicList.AddRange(list2);
			break;
		}
		default:
			if (num != 206)
			{
				if (num == 207)
				{
					bool isPlaying = false;
					Serializer.Deserialize(wrapper.DataPool, notification.ValueOffset, ref isPlaying);
					bool flag = isPlaying && !this._isPlaying;
					if (flag)
					{
						this.PlayMusic(0f);
					}
				}
			}
			else
			{
				List<short> list3 = new List<short>();
				Serializer.Deserialize(wrapper.DataPool, notification.ValueOffset, ref list3);
				this._favoriteMusicList.Clear();
				this._favoriteMusicList.AddRange(list3);
			}
			break;
		}
	}

	// Token: 0x060010A3 RID: 4259 RVA: 0x00063739 File Offset: 0x00061939
	public void PlayMusic(short musicId, float progressTime = 0f)
	{
		this.MusicId = musicId;
		this.MaxTime = this._musicDurationDict[musicId];
		this.PlayMusic(this._musicNameDict[musicId], progressTime);
	}

	// Token: 0x060010A4 RID: 4260 RVA: 0x0006376C File Offset: 0x0006196C
	public void PlayMusic(float progressTime = 0f)
	{
		bool flag = this.MusicId == -1;
		if (flag)
		{
			this.PlayNextMusic(null);
		}
		else
		{
			this.PlayMusic(this.MusicId, progressTime);
		}
	}

	// Token: 0x060010A5 RID: 4261 RVA: 0x0006379F File Offset: 0x0006199F
	public void PlayNextMusic(IReadOnlyList<short> scopeList = null)
	{
		this.PlayMusic(this.GetNextMusicId(scopeList), 0f);
	}

	// Token: 0x060010A6 RID: 4262 RVA: 0x000637B5 File Offset: 0x000619B5
	public void PlayLastMusic(IReadOnlyList<short> scopeList = null)
	{
		this.PlayMusic(this.GetLastMusicId(scopeList), 0f);
	}

	// Token: 0x060010A7 RID: 4263 RVA: 0x000637CC File Offset: 0x000619CC
	private short GetNextMusicId(IReadOnlyList<short> scopeList)
	{
		bool flag = scopeList != null && scopeList.Count > 0;
		short result;
		if (flag)
		{
			int index = scopeList.IndexOf(this.MusicId);
			bool flag2 = index < 0;
			if (flag2)
			{
				result = scopeList[0];
			}
			else
			{
				result = scopeList[(index + 1) % scopeList.Count];
			}
		}
		else
		{
			MusicPlayerModel.MusicPlayMode playMode = this.PlayMode;
			MusicPlayerModel.MusicPlayMode musicPlayMode = playMode;
			short id;
			if (musicPlayMode > MusicPlayerModel.MusicPlayMode.Single)
			{
				if (musicPlayMode != MusicPlayerModel.MusicPlayMode.Random)
				{
					throw new ArgumentOutOfRangeException();
				}
				int nextIndex = (this._unlockedMusicRandomList.IndexOf(this.MusicId) + 1) % this._unlockedMusicRandomList.Count;
				id = this._unlockedMusicRandomList[nextIndex];
			}
			else
			{
				bool flag3 = this.MusicId == -1;
				if (flag3)
				{
					id = this._unlockedMusicList.First<short>();
				}
				else
				{
					int nextIndex = (this._unlockedMusicList.IndexOf(this.MusicId) + 1) % this._unlockedMusicList.Count;
					id = this._unlockedMusicList[nextIndex];
				}
			}
			result = id;
		}
		return result;
	}

	// Token: 0x060010A8 RID: 4264 RVA: 0x000638D0 File Offset: 0x00061AD0
	private short GetLastMusicId(IReadOnlyList<short> scopeList)
	{
		bool flag = scopeList != null && scopeList.Count > 0;
		short result;
		if (flag)
		{
			int index = scopeList.IndexOf(this.MusicId);
			bool flag2 = index < 0;
			if (flag2)
			{
				result = scopeList[scopeList.Count - 1];
			}
			else
			{
				int prevIndex = index - 1;
				bool flag3 = prevIndex < 0;
				if (flag3)
				{
					prevIndex = scopeList.Count - 1;
				}
				result = scopeList[prevIndex];
			}
		}
		else
		{
			MusicPlayerModel.MusicPlayMode playMode = this.PlayMode;
			MusicPlayerModel.MusicPlayMode musicPlayMode = playMode;
			short id;
			if (musicPlayMode > MusicPlayerModel.MusicPlayMode.Single)
			{
				if (musicPlayMode != MusicPlayerModel.MusicPlayMode.Random)
				{
					throw new ArgumentOutOfRangeException();
				}
				bool flag4 = this.MusicId == -1;
				int lastIndex;
				if (flag4)
				{
					lastIndex = this._unlockedMusicRandomList.Count - 1;
				}
				else
				{
					lastIndex = this._unlockedMusicRandomList.IndexOf(this.MusicId) - 1;
					bool flag5 = lastIndex < 0;
					if (flag5)
					{
						lastIndex = this._unlockedMusicRandomList.Count - 1;
					}
				}
				id = this._unlockedMusicRandomList[lastIndex];
			}
			else
			{
				bool flag6 = this.MusicId == -1;
				int lastIndex;
				if (flag6)
				{
					lastIndex = this._unlockedMusicList.Count - 1;
				}
				else
				{
					lastIndex = this._unlockedMusicList.IndexOf(this.MusicId) - 1;
					bool flag7 = lastIndex < 0;
					if (flag7)
					{
						lastIndex = this._unlockedMusicList.Count - 1;
					}
				}
				id = this._unlockedMusicList[lastIndex];
			}
			result = id;
		}
		return result;
	}

	// Token: 0x060010A9 RID: 4265 RVA: 0x00063A30 File Offset: 0x00061C30
	private void PlayMusic(string musicName, float progressTime = 0f)
	{
		this.IsPlaying = true;
		this.IsPaused = false;
		AudioManager.Instance.PlayMusicForPlayer(AudioManager.DummyAudioName, 1f, 100, null, 0f);
		AudioManager.Instance.PlayMusicForPlayer(musicName, (progressTime > 0f) ? 1f : 0.3f, 100, delegate(string _)
		{
			bool flag = this.PlayMode == MusicPlayerModel.MusicPlayMode.Single;
			if (flag)
			{
				this.PlayMusic(0f);
			}
			else
			{
				this.PlayNextMusic(null);
			}
		}, progressTime);
		GEvent.OnEvent(UiEvents.OnMusicPlayerPlayStateChange, null);
	}

	// Token: 0x060010AA RID: 4266 RVA: 0x00063AAC File Offset: 0x00061CAC
	public void PauseMusic(bool refreshBGM = false)
	{
		bool flag = !this.IsEnabled || !this.IsPlaying || this.IsPaused;
		if (!flag)
		{
			this.IsPlaying = false;
			this.IsPaused = true;
			this.PausedMusicProgress = AudioManager.Instance.GetMusicPlayerProgress();
			AudioManager.Instance.PlayMusicForPlayer(AudioManager.DummyAudioName, 1f, 100, null, 0f);
			GEvent.OnEvent(UiEvents.OnMusicPlayerPlayStateChange, null);
		}
	}

	// Token: 0x060010AB RID: 4267 RVA: 0x00063B28 File Offset: 0x00061D28
	public bool ResumeMusic()
	{
		bool flag = !this.IsPaused || !this.IsEnabled || this.IsPlaying;
		bool result;
		if (flag)
		{
			result = false;
		}
		else
		{
			this.PlayMusic(this.PausedMusicProgress);
			result = true;
		}
		return result;
	}

	// Token: 0x060010AC RID: 4268 RVA: 0x00063B6C File Offset: 0x00061D6C
	public void StopMusic()
	{
		bool flag = !this.IsEnabled || !this.IsPlaying;
		if (!flag)
		{
			this.IsPlaying = false;
			this.IsPaused = false;
			this.PausedMusicProgress = 0f;
			AudioManager.Instance.PlayMusicForPlayer(AudioManager.DummyAudioName, 1f, 100, null, 0f);
			SingletonObject.getInstance<WorldMapModel>().UpdateBgm();
			GEvent.OnEvent(UiEvents.OnMusicPlayerPlayStateChange, null);
		}
	}

	// Token: 0x060010AD RID: 4269 RVA: 0x00063BE8 File Offset: 0x00061DE8
	public void EnableMusicPlayer()
	{
		this.IsEnabled = true;
		GEvent.OnEvent(UiEvents.OnMusicPlayerEnabledStateChange, null);
		GEvent.OnEvent(UiEvents.OnMusicPlayerPlayStateChange, null);
	}

	// Token: 0x060010AE RID: 4270 RVA: 0x00063C15 File Offset: 0x00061E15
	public void DisableMusicPlayer()
	{
		this.StopMusic();
		this.IsEnabled = false;
		GEvent.OnEvent(UiEvents.OnMusicPlayerEnabledStateChange, null);
	}

	// Token: 0x060010AF RID: 4271 RVA: 0x00063C38 File Offset: 0x00061E38
	public void SetIsMainStoryControlBgm(bool isMainStoryControlBgm)
	{
		this.IsMainStoryControlBgm = isMainStoryControlBgm;
	}

	// Token: 0x060010B0 RID: 4272 RVA: 0x00063C43 File Offset: 0x00061E43
	private void OnTopUiChanged(ArgumentBox argBox)
	{
		this._interactable = (!UIManager.Instance.IsFocusElement(UIElement.StateAdventureRemake) && !UIManager.Instance.IsFocusElement(UIElement.StateAdventureRemakeSpecialBottom));
	}

	// Token: 0x060010B1 RID: 4273 RVA: 0x00063C72 File Offset: 0x00061E72
	public float GetMusicDuration(short id)
	{
		return this._musicDurationDict.GetValueOrDefault(id);
	}

	// Token: 0x060010B2 RID: 4274 RVA: 0x00063C80 File Offset: 0x00061E80
	public bool IsMusicLock(short id)
	{
		return !this._unlockedMusicList.Contains(id);
	}

	// Token: 0x060010B3 RID: 4275 RVA: 0x00063C91 File Offset: 0x00061E91
	public bool IsMusicPlaying(short id)
	{
		return this.MusicId == id && this.IsPlaying;
	}

	// Token: 0x060010B4 RID: 4276 RVA: 0x00063CA5 File Offset: 0x00061EA5
	public bool IsMusicSelected(short id)
	{
		return this.MusicId == id;
	}

	// Token: 0x060010B5 RID: 4277 RVA: 0x00063CB0 File Offset: 0x00061EB0
	public void AddFavorite(short musicId)
	{
		bool flag = !this.IsFavorite(musicId);
		if (flag)
		{
			this._favoriteMusicList.Add(musicId);
			this.SetFavorite();
		}
	}

	// Token: 0x060010B6 RID: 4278 RVA: 0x00063CE4 File Offset: 0x00061EE4
	public void RemoveFavorite(short musicId)
	{
		bool flag = this.IsFavorite(musicId);
		if (flag)
		{
			this._favoriteMusicList.Remove(musicId);
			this.SetFavorite();
		}
	}

	// Token: 0x060010B7 RID: 4279 RVA: 0x00063D14 File Offset: 0x00061F14
	private void SetFavorite()
	{
		this._favoriteMusicList.Sort((short a, short b) => a.CompareTo(b));
		GameDataBridge.AddDataModification<List<short>>(19, 206, ulong.MaxValue, uint.MaxValue, this._favoriteMusicList);
	}

	// Token: 0x060010B8 RID: 4280 RVA: 0x00063D63 File Offset: 0x00061F63
	public bool IsFavorite(short musicId)
	{
		return this._favoriteMusicList.Contains(musicId);
	}

	// Token: 0x04000ED9 RID: 3801
	private int _gameDataListenerId = -1;

	// Token: 0x04000EDA RID: 3802
	private readonly Dictionary<short, string> _musicNameDict = new Dictionary<short, string>();

	// Token: 0x04000EDB RID: 3803
	private readonly Dictionary<short, float> _musicDurationDict = new Dictionary<short, float>();

	// Token: 0x04000EDC RID: 3804
	private readonly List<short> _unlockedMusicRandomList = new List<short>();

	// Token: 0x04000EDE RID: 3806
	public float PausedMusicProgress;

	// Token: 0x04000EE0 RID: 3808
	private bool _interactable;

	// Token: 0x04000EE2 RID: 3810
	private readonly List<short> _unlockedMusicList = new List<short>();

	// Token: 0x04000EE3 RID: 3811
	private bool _isEnabled;

	// Token: 0x04000EE4 RID: 3812
	private MusicPlayerModel.MusicPlayMode _playMode;

	// Token: 0x04000EE5 RID: 3813
	private short _musicId = -1;

	// Token: 0x04000EE6 RID: 3814
	private bool _isPlaying;

	// Token: 0x04000EE7 RID: 3815
	private readonly List<short> _evaluatedMusicList = new List<short>();

	// Token: 0x04000EE8 RID: 3816
	private readonly List<short> _favoriteMusicList = new List<short>();

	// Token: 0x020011F1 RID: 4593
	public enum MusicPlayMode
	{
		// Token: 0x040098D7 RID: 39127
		List,
		// Token: 0x040098D8 RID: 39128
		Single,
		// Token: 0x040098D9 RID: 39129
		Random,
		// Token: 0x040098DA RID: 39130
		Count
	}
}
