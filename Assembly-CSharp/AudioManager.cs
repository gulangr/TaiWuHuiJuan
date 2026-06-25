using System;
using System.Collections;
using System.Collections.Generic;
using AudioKit;
using DG.Tweening;
using UnityEngine;

// Token: 0x02000014 RID: 20
public class AudioManager : MonoBehaviour
{
	// Token: 0x1700001A RID: 26
	// (get) Token: 0x0600004F RID: 79 RVA: 0x00002E35 File Offset: 0x00001035
	// (set) Token: 0x06000050 RID: 80 RVA: 0x00002E3C File Offset: 0x0000103C
	public static AudioManager Instance { get; private set; }

	// Token: 0x06000051 RID: 81 RVA: 0x00002E44 File Offset: 0x00001044
	private void Awake()
	{
		bool flag = null != AudioManager.Instance;
		if (flag)
		{
			Object.Destroy(base.gameObject);
		}
		else
		{
			AudioManager.Instance = this;
			AudioInfos.Init();
			Object.DontDestroyOnLoad(base.gameObject);
			this._playingSoundPlayers = new List<SoundPlayer>();
			PoolManager.SetSrcObject("soundPlayerPrefab", this.SoundPlayerPrefab.gameObject);
		}
	}

	// Token: 0x06000052 RID: 82 RVA: 0x00002EAC File Offset: 0x000010AC
	public void OnApplicationFocus(bool focus)
	{
		this.IsFocusing = focus;
		GlobalSettings settingData = SingletonObject.getInstance<GlobalSettings>();
		bool flag = !settingData.MuteIfNotFocus;
		if (!flag)
		{
			bool bgmOn = settingData.BgmOn;
			if (bgmOn)
			{
				this.SetMusicMute(!focus);
			}
			bool seOn = settingData.SeOn;
			if (seOn)
			{
				this.SetSoundMute(!focus);
			}
		}
	}

	// Token: 0x06000053 RID: 83 RVA: 0x00002F00 File Offset: 0x00001100
	public void SetMusicMute(bool mute)
	{
		this.MusicPlayer.OnSystemMusicMuteUpdate(mute);
		this.SetMusicVolume(this._musicVolumeRate * (float)SingletonObject.getInstance<GlobalSettings>().BgmVolume / 100f, true);
	}

	// Token: 0x06000054 RID: 84 RVA: 0x00002F30 File Offset: 0x00001130
	public void SetMusicVolume(float volume, bool stopCoroutine = true)
	{
		if (stopCoroutine)
		{
			base.StopAllCoroutines();
		}
		this._musicVolume = Mathf.Clamp01(volume);
		this.MusicPlayer.OnSystemMusicVolumeUpdate();
	}

	// Token: 0x06000055 RID: 85 RVA: 0x00002F62 File Offset: 0x00001162
	public void SetMusicVolumeWithFade(float duration, float delay = 0f)
	{
		base.StopAllCoroutines();
		base.StartCoroutine(this.VolumeChange(this._musicVolumeRate, this._mapMusicVolumeRate, this._mapAmbienceVolumeRate, duration, delay));
	}

	// Token: 0x06000056 RID: 86 RVA: 0x00002F8D File Offset: 0x0000118D
	public void SetMapMusicVolumeRate(float targetVolumeRate)
	{
		this._mapMusicVolumeRate = Mathf.Clamp01(targetVolumeRate);
	}

	// Token: 0x06000057 RID: 87 RVA: 0x00002F9C File Offset: 0x0000119C
	public void SetMapAmbienceVolumeRate(float targetVolumeRate)
	{
		this._mapAmbienceVolumeRate = Mathf.Clamp01(targetVolumeRate);
	}

	// Token: 0x06000058 RID: 88 RVA: 0x00002FAB File Offset: 0x000011AB
	public void EnableMusicVolumeRate(float targetVolumeRate)
	{
		this._musicVolumeRate = Mathf.Clamp01(targetVolumeRate);
	}

	// Token: 0x06000059 RID: 89 RVA: 0x00002FBA File Offset: 0x000011BA
	public void DisableMusicVolumeRate()
	{
		this._musicVolumeRate = 1f;
	}

	// Token: 0x1700001B RID: 27
	// (get) Token: 0x0600005A RID: 90 RVA: 0x00002FC8 File Offset: 0x000011C8
	public bool IsInVideoMode
	{
		get
		{
			return this._inVideoMode;
		}
	}

	// Token: 0x1700001C RID: 28
	// (get) Token: 0x0600005B RID: 91 RVA: 0x00002FD0 File Offset: 0x000011D0
	public bool IsSfxSuppressedByVideo
	{
		get
		{
			return this._sfxSuppressedByVideo;
		}
	}

	// Token: 0x0600005C RID: 92 RVA: 0x00002FD8 File Offset: 0x000011D8
	public void EnterVideoMode()
	{
		bool inVideoMode = this._inVideoMode;
		if (!inVideoMode)
		{
			this._inVideoMode = true;
			this._sfxSuppressedByVideo = true;
			this.EnableMusicVolumeRate(0f);
			this.SetMusicVolumeWithFade(1f, 0f);
			this.SetSoundPlayersMute(true);
		}
	}

	// Token: 0x0600005D RID: 93 RVA: 0x00003028 File Offset: 0x00001228
	public void ExitVideoMode()
	{
		bool flag = !this._inVideoMode;
		if (!flag)
		{
			this._inVideoMode = false;
			this._sfxSuppressedByVideo = false;
			this.DisableMusicVolumeRate();
			this.SetMusicVolumeWithFade(1f, 0f);
			this.SetSoundPlayersMute(this.GetNaturalSoundMute());
		}
	}

	// Token: 0x0600005E RID: 94 RVA: 0x00003078 File Offset: 0x00001278
	private IEnumerator VolumeChange(float targetVolumeRate, float mapMusicVolumeRate, float mapAmbienceVolumeRate, float duration, float delay)
	{
		GlobalSettings settingData = SingletonObject.getInstance<GlobalSettings>();
		bool flag = (!settingData.BgmOn && !settingData.SeOn) || this.GetShouldMuteIfFocus();
		if (flag)
		{
			yield break;
		}
		bool flag2 = delay > 0f;
		if (flag2)
		{
			yield return new WaitForSecondsRealtime(delay);
		}
		WaitForSecondsRealtime wait = new WaitForSecondsRealtime(0.016f);
		float curTime = 0f;
		float targetMusicVolume = targetVolumeRate * mapMusicVolumeRate * (float)settingData.BgmVolume / 100f;
		float curMusicVolume = this.GetMusicVolume();
		float targetAmbienceVolume = targetVolumeRate * mapAmbienceVolumeRate * (float)settingData.SeVolume / 100f;
		float curAmbienceVolume = this.GetAmbienceVolume();
		while (curTime <= duration)
		{
			curTime += 0.016f;
			float musicVolume = Mathf.Lerp(curMusicVolume, targetMusicVolume, curTime / duration);
			this.SetMusicVolume(musicVolume, false);
			float ambienceVolume = Mathf.Lerp(curAmbienceVolume, targetAmbienceVolume, curTime / duration);
			this.SetAmbienceVolume(ambienceVolume);
			yield return wait;
		}
		yield break;
	}

	// Token: 0x0600005F RID: 95 RVA: 0x000030AC File Offset: 0x000012AC
	public float GetMusicVolume()
	{
		return this._musicVolume;
	}

	// Token: 0x06000060 RID: 96 RVA: 0x000030C4 File Offset: 0x000012C4
	public bool GetShouldMuteIfFocus()
	{
		return SingletonObject.getInstance<GlobalSettings>().MuteIfNotFocus && !this.IsFocusing;
	}

	// Token: 0x06000061 RID: 97 RVA: 0x000030EE File Offset: 0x000012EE
	public void SetSoundMute(bool mute)
	{
		this.AmbiencePlayer.OnSystemMusicMuteUpdate(mute);
		this.SetSoundPlayersMute(mute);
	}

	// Token: 0x06000062 RID: 98 RVA: 0x00003108 File Offset: 0x00001308
	private void SetSoundPlayersMute(bool mute)
	{
		for (int i = 0; i < this._playingSoundPlayers.Count; i++)
		{
			SoundPlayer soundPlayer = this._playingSoundPlayers[i];
			bool flag = soundPlayer != null && soundPlayer.AudioPlayer != null;
			if (flag)
			{
				soundPlayer.AudioPlayer.mute = mute;
			}
		}
		foreach (KeyValuePair<AudioSource, float> keyValuePair in this._playingAudioSource)
		{
			AudioSource audioSource2;
			float num;
			keyValuePair.Deconstruct(out audioSource2, out num);
			AudioSource audioSource = audioSource2;
			float volume = num;
			bool flag2 = audioSource != null;
			if (flag2)
			{
				audioSource.DOKill(false);
				audioSource.volume = (mute ? 0f : volume);
			}
		}
	}

	// Token: 0x06000063 RID: 99 RVA: 0x000031F0 File Offset: 0x000013F0
	private bool GetNaturalSoundMute()
	{
		GlobalSettings settingData = SingletonObject.getInstance<GlobalSettings>();
		return !settingData.SeOn || this.GetShouldMuteIfFocus();
	}

	// Token: 0x06000064 RID: 100 RVA: 0x0000321C File Offset: 0x0000141C
	public bool GetEffectiveSoundMute()
	{
		GlobalSettings settingData = SingletonObject.getInstance<GlobalSettings>();
		return !settingData.SeOn || this.GetShouldMuteIfFocus() || this._sfxSuppressedByVideo;
	}

	// Token: 0x06000065 RID: 101 RVA: 0x0000324D File Offset: 0x0000144D
	public void SetSoundVolume(float volume)
	{
		this._soundVolume = Mathf.Clamp01(volume);
		this.SetAmbienceVolume(this._soundVolume);
	}

	// Token: 0x06000066 RID: 102 RVA: 0x0000326C File Offset: 0x0000146C
	public float GetSoundVolume()
	{
		return this._soundVolume;
	}

	// Token: 0x06000067 RID: 103 RVA: 0x00003284 File Offset: 0x00001484
	public void SetAmbienceVolume(float volume)
	{
		this._ambienceVolume = Mathf.Clamp01(volume);
		this.AmbiencePlayer.OnSystemMusicVolumeUpdate(this._ambienceVolume);
	}

	// Token: 0x06000068 RID: 104 RVA: 0x000032A8 File Offset: 0x000014A8
	public float GetAmbienceVolume()
	{
		return this._ambienceVolume;
	}

	// Token: 0x06000069 RID: 105 RVA: 0x000032C0 File Offset: 0x000014C0
	public void PlaySound(string soundName, bool loop = false, bool canSetPitchByGlobal = false)
	{
		bool flag = string.IsNullOrEmpty(soundName) || null == this.SoundPlayerPrefab;
		if (!flag)
		{
			this.Play(new AudioCommand
			{
				AudioName = soundName,
				Loop = loop,
				CanSetPitchByGlobal = canSetPitchByGlobal
			});
		}
	}

	// Token: 0x0600006A RID: 106 RVA: 0x0000330C File Offset: 0x0000150C
	public void PlaySoundNoRepeat(string soundName, int volume = 100, bool loop = false, bool canSetPitchByGlobal = false)
	{
		bool flag = string.IsNullOrEmpty(soundName) || null == this.SoundPlayerPrefab;
		if (!flag)
		{
			bool flag2 = this.IsPlayingSound(soundName);
			if (!flag2)
			{
				this.Play(new AudioCommand
				{
					AudioName = soundName,
					Loop = loop,
					Volume = volume,
					CanSetPitchByGlobal = canSetPitchByGlobal
				});
			}
		}
	}

	// Token: 0x0600006B RID: 107 RVA: 0x00003370 File Offset: 0x00001570
	public void PlaySound(AudioClip clip, bool loop = false, int volume = 100)
	{
		bool flag = null == clip;
		if (!flag)
		{
			this.Play(new AudioCommand
			{
				Clip = clip,
				Loop = loop,
				Volume = volume
			});
		}
	}

	// Token: 0x0600006C RID: 108 RVA: 0x000033AC File Offset: 0x000015AC
	public void PlaySound(AudioClip clip, float pitch, bool loop = false, int volume = 100)
	{
		bool flag = null == clip;
		if (!flag)
		{
			this.Play(new AudioCommand
			{
				Clip = clip,
				Pitch = pitch,
				Loop = loop,
				Volume = volume,
				CanSetPitchByGlobal = true
			});
		}
	}

	// Token: 0x0600006D RID: 109 RVA: 0x000033F8 File Offset: 0x000015F8
	public void PlayMusic(string musicName = "", float fade = 1f, int volume = 100, Action<string> onPlayFinish = null)
	{
		bool flag = SingletonObject.IsCreatedInstance<MusicPlayerModel>();
		if (flag)
		{
			SingletonObject.getInstance<MusicPlayerModel>().PauseMusic(false);
		}
		this.Play(new AudioCommand
		{
			AudioType = SEType.Music,
			AudioName = musicName,
			FadeTimeIn = fade,
			FadeTimeOut = fade,
			Volume = volume,
			OnPlayFinish = onPlayFinish,
			Loop = true
		});
	}

	// Token: 0x0600006E RID: 110 RVA: 0x0000345C File Offset: 0x0000165C
	public void PlayMusicForPlayer(string musicName = "", float fade = 1f, int volume = 100, Action<string> onPlayFinish = null, float progressTime = 0f)
	{
		this.Play(new AudioCommand
		{
			AudioType = SEType.Music,
			AudioName = musicName,
			FadeTimeIn = fade,
			FadeTimeOut = fade,
			Volume = volume,
			OnPlayFinish = onPlayFinish,
			Loop = false,
			ProgressTime = progressTime
		});
	}

	// Token: 0x0600006F RID: 111 RVA: 0x000034B0 File Offset: 0x000016B0
	public void PlayLoopSoundWithAmbience(string soundName)
	{
		AudioCommand cmd = new AudioCommand
		{
			AudioType = SEType.Sound,
			Loop = true,
			AudioName = soundName,
			OnPlayUpdate = new Action<AudioCommandOnPlayeUpdateParam>(this.OnUpdateLoopSoundWithAmbience)
		};
		this.Play(cmd);
	}

	// Token: 0x06000070 RID: 112 RVA: 0x000034F3 File Offset: 0x000016F3
	public void StopLoopSoundWithAmbience(string soundName)
	{
		this.StopAllSound(soundName);
	}

	// Token: 0x06000071 RID: 113 RVA: 0x000034FE File Offset: 0x000016FE
	private void OnUpdateLoopSoundWithAmbience(AudioCommandOnPlayeUpdateParam param)
	{
		param.player.volume = this._ambienceVolume;
	}

	// Token: 0x06000072 RID: 114 RVA: 0x00003513 File Offset: 0x00001713
	public void PlayAmbience(string ambienceName = "", float fade = 1f, int volume = 100)
	{
		this.Play(new AudioCommand
		{
			AudioType = SEType.Ambience,
			AudioName = ambienceName,
			FadeTimeIn = fade,
			FadeTimeOut = fade,
			Volume = volume
		});
	}

	// Token: 0x06000073 RID: 115 RVA: 0x00003548 File Offset: 0x00001748
	public void Play(AudioCommand cmd)
	{
		bool flag = cmd.AudioType == SEType.Music;
		if (flag)
		{
			bool flag2 = cmd.ProgressTime > 0f;
			if (flag2)
			{
				this.MusicPlayer.SetProgress = true;
				this.MusicPlayer.Time = cmd.ProgressTime;
			}
			else
			{
				this.MusicPlayer.SetProgress = false;
			}
			this.MusicPlayer.Play(cmd);
		}
		else
		{
			bool flag3 = cmd.AudioType == SEType.Ambience;
			if (flag3)
			{
				this.AmbiencePlayer.Play(cmd, this._ambienceVolume);
			}
			else
			{
				bool flag4 = cmd.AudioType == SEType.Sound;
				if (flag4)
				{
					SoundPlayer soundPlayer = PoolManager.GetObject<SoundPlayer>("soundPlayerPrefab");
					bool flag5 = null == soundPlayer;
					if (!flag5)
					{
						soundPlayer.transform.SetParent(base.transform, false);
						this._playingSoundPlayers.Add(soundPlayer);
						soundPlayer.Play(cmd);
					}
				}
			}
		}
	}

	// Token: 0x06000074 RID: 116 RVA: 0x00003634 File Offset: 0x00001834
	public void StopSound(string soundName)
	{
		for (int i = 0; i < this._playingSoundPlayers.Count; i++)
		{
			SoundPlayer soundPlayer = this._playingSoundPlayers[i];
			bool flag = soundPlayer == null || soundPlayer.AudioPlayer == null;
			if (!flag)
			{
				AudioClip clip = soundPlayer.AudioPlayer.clip;
				bool flag2 = clip != null && clip.name == soundName;
				if (flag2)
				{
					soundPlayer.Stop();
					break;
				}
			}
		}
	}

	// Token: 0x06000075 RID: 117 RVA: 0x000036C0 File Offset: 0x000018C0
	public void StopAllSound(string soundName)
	{
		for (int i = 0; i < this._playingSoundPlayers.Count; i++)
		{
			SoundPlayer soundPlayer = this._playingSoundPlayers[i];
			bool flag = soundPlayer == null || soundPlayer.AudioPlayer == null;
			if (!flag)
			{
				AudioClip clip = soundPlayer.AudioPlayer.clip;
				bool flag2 = clip != null && clip.name == soundName;
				if (flag2)
				{
					soundPlayer.Stop();
				}
			}
		}
	}

	// Token: 0x06000076 RID: 118 RVA: 0x0000374B File Offset: 0x0000194B
	public void StopMusic()
	{
		this.MusicPlayer.Stop();
	}

	// Token: 0x06000077 RID: 119 RVA: 0x0000375A File Offset: 0x0000195A
	public void StopMusicWithFade(float fadeTime = 0.3f)
	{
		this.MusicPlayer.StopWithFade(fadeTime);
	}

	// Token: 0x06000078 RID: 120 RVA: 0x0000376A File Offset: 0x0000196A
	public void StopAmbience()
	{
		this.AmbiencePlayer.Stop();
	}

	// Token: 0x06000079 RID: 121 RVA: 0x00003779 File Offset: 0x00001979
	public void StopAmbienceWithFade(float fadeTime = 1f)
	{
		this.AmbiencePlayer.StopWithFade(fadeTime);
	}

	// Token: 0x0600007A RID: 122 RVA: 0x0000378C File Offset: 0x0000198C
	public string GetPlayingMusic()
	{
		return this.MusicPlayer.GetPlayingMusicName();
	}

	// Token: 0x0600007B RID: 123 RVA: 0x000037AC File Offset: 0x000019AC
	public string GetPlayingAmbience()
	{
		return this.AmbiencePlayer.GetPlayingMusicName();
	}

	// Token: 0x0600007C RID: 124 RVA: 0x000037CC File Offset: 0x000019CC
	public void StopAll()
	{
		this.MusicPlayer.Stop();
		this.AmbiencePlayer.Stop();
		List<SoundPlayer> playersToStop = new List<SoundPlayer>(this._playingSoundPlayers);
		foreach (SoundPlayer player in playersToStop)
		{
			bool flag = player != null;
			if (flag)
			{
				player.Stop();
			}
		}
		this._playingSoundPlayers.Clear();
	}

	// Token: 0x0600007D RID: 125 RVA: 0x0000385C File Offset: 0x00001A5C
	public void Pause()
	{
		this.MusicPlayer.Pause();
		this.AmbiencePlayer.Pause();
		foreach (SoundPlayer player in this._playingSoundPlayers)
		{
			bool flag = player != null;
			if (flag)
			{
				player.Pause();
			}
		}
	}

	// Token: 0x0600007E RID: 126 RVA: 0x000038D8 File Offset: 0x00001AD8
	public void Resume()
	{
		this.MusicPlayer.Resume();
		this.AmbiencePlayer.Resume();
		foreach (SoundPlayer player in this._playingSoundPlayers)
		{
			bool flag = player != null;
			if (flag)
			{
				player.Resume();
			}
		}
	}

	// Token: 0x0600007F RID: 127 RVA: 0x00003954 File Offset: 0x00001B54
	public void SetSoundTimeScale(float timeScale)
	{
		foreach (SoundPlayer player in this._playingSoundPlayers)
		{
			bool flag = player != null && player.AudioPlayer != null && player.CanSetPitchByGlobal;
			if (flag)
			{
				player.AudioPlayer.pitch = timeScale;
			}
		}
	}

	// Token: 0x06000080 RID: 128 RVA: 0x000039D4 File Offset: 0x00001BD4
	public void ReleaseSoundPlayer(SoundPlayer player)
	{
		bool flag = null == player;
		if (!flag)
		{
			List<SoundPlayer> playingSoundPlayers = this._playingSoundPlayers;
			if (playingSoundPlayers != null)
			{
				playingSoundPlayers.Remove(player);
			}
			bool flag2 = player.gameObject != null;
			if (flag2)
			{
				PoolManager.Destroy("soundPlayerPrefab", player.gameObject);
			}
		}
	}

	// Token: 0x06000081 RID: 129 RVA: 0x00003A28 File Offset: 0x00001C28
	public bool IsPlayingSound(string audioName)
	{
		bool flag = this._playingSoundPlayers != null;
		if (flag)
		{
			foreach (SoundPlayer soundPlayer in this._playingSoundPlayers)
			{
				bool flag2 = soundPlayer != null && soundPlayer.IsPlaying(audioName);
				if (flag2)
				{
					return true;
				}
			}
		}
		return false;
	}

	// Token: 0x06000082 RID: 130 RVA: 0x00003AAC File Offset: 0x00001CAC
	public float GetMusicPlayerProgress()
	{
		return this.MusicPlayer.Time;
	}

	// Token: 0x06000083 RID: 131 RVA: 0x00003AC9 File Offset: 0x00001CC9
	public void AddAudioSource(AudioSource audioSource, float volume)
	{
		this._playingAudioSource.TryAdd(audioSource, volume);
		this._playingAudioSource[audioSource] = volume;
	}

	// Token: 0x06000084 RID: 132 RVA: 0x00003AE8 File Offset: 0x00001CE8
	public void RemoveAudioSource(AudioSource audioSource)
	{
		bool flag = audioSource != null && this._playingAudioSource.ContainsKey(audioSource);
		if (flag)
		{
			this._playingAudioSource.Remove(audioSource);
		}
	}

	// Token: 0x06000085 RID: 133 RVA: 0x00003B20 File Offset: 0x00001D20
	public void PauseAllSound()
	{
		this.AmbiencePlayer.Pause();
		foreach (SoundPlayer player in this._playingSoundPlayers)
		{
			bool flag = player != null;
			if (flag)
			{
				player.Pause();
			}
		}
	}

	// Token: 0x06000086 RID: 134 RVA: 0x00003B90 File Offset: 0x00001D90
	public void ResumeAllSound()
	{
		this.AmbiencePlayer.Resume();
		foreach (SoundPlayer player in this._playingSoundPlayers)
		{
			bool flag = player != null;
			if (flag)
			{
				player.Resume();
			}
		}
	}

	// Token: 0x0400002C RID: 44
	public static readonly string[] MainMenuBgmRandomPool = new string[]
	{
		"sect_shaolinpai_zaoke",
		"sect_emeipai",
		"sect_baihuagu",
		"sect_wudangpai_tiangang",
		"sect_yuanshanpai_daxiaoyuanshan",
		"sect_ranshanpai",
		"sect_xuannvpai_xuannvfeng"
	};

	// Token: 0x0400002D RID: 45
	public static readonly string DummyAudioName = "Dummy";

	// Token: 0x0400002F RID: 47
	public MusicPlayer MusicPlayer;

	// Token: 0x04000030 RID: 48
	public MusicPlayer AmbiencePlayer;

	// Token: 0x04000031 RID: 49
	public SoundPlayer SoundPlayerPrefab;

	// Token: 0x04000032 RID: 50
	[NonSerialized]
	public bool IsFocusing = true;

	// Token: 0x04000033 RID: 51
	private float _musicVolume = 1f;

	// Token: 0x04000034 RID: 52
	private float _soundVolume = 1f;

	// Token: 0x04000035 RID: 53
	private float _ambienceVolume = 1f;

	// Token: 0x04000036 RID: 54
	public const float NormalChangeDuration = 1f;

	// Token: 0x04000037 RID: 55
	public const float SettingChangeDuration = 5f;

	// Token: 0x04000038 RID: 56
	public const float MusicRateOnOpenSystemOption = 0.2f;

	// Token: 0x04000039 RID: 57
	private float _musicVolumeRate = 1f;

	// Token: 0x0400003A RID: 58
	private float _mapMusicVolumeRate = 1f;

	// Token: 0x0400003B RID: 59
	private float _mapAmbienceVolumeRate = 1f;

	// Token: 0x0400003C RID: 60
	private bool _inVideoMode;

	// Token: 0x0400003D RID: 61
	private bool _sfxSuppressedByVideo;

	// Token: 0x0400003E RID: 62
	private const string SePlayerPrefabKey = "soundPlayerPrefab";

	// Token: 0x0400003F RID: 63
	private List<SoundPlayer> _playingSoundPlayers;

	// Token: 0x04000040 RID: 64
	private Dictionary<AudioSource, float> _playingAudioSource = new Dictionary<AudioSource, float>();
}
