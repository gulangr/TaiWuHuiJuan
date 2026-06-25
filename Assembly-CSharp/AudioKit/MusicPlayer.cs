using System;
using DG.Tweening;
using UnityEngine;

namespace AudioKit
{
	// Token: 0x02000FD8 RID: 4056
	[RequireComponent(typeof(AudioSource))]
	public class MusicPlayer : MonoBehaviour
	{
		// Token: 0x170014F1 RID: 5361
		// (get) Token: 0x0600B96B RID: 47467 RVA: 0x00548E03 File Offset: 0x00547003
		// (set) Token: 0x0600B96C RID: 47468 RVA: 0x00548E0B File Offset: 0x0054700B
		public float Time { get; set; }

		// Token: 0x170014F2 RID: 5362
		// (get) Token: 0x0600B96D RID: 47469 RVA: 0x00548E14 File Offset: 0x00547014
		// (set) Token: 0x0600B96E RID: 47470 RVA: 0x00548E1C File Offset: 0x0054701C
		public bool SetProgress { get; set; }

		// Token: 0x0600B96F RID: 47471 RVA: 0x00548E28 File Offset: 0x00547028
		private void Awake()
		{
			this.AudioPlayer = base.GetComponent<AudioSource>();
			bool flag = null == this.AudioPlayer;
			if (flag)
			{
				this.AudioPlayer = base.gameObject.AddComponent<AudioSource>();
			}
		}

		// Token: 0x0600B970 RID: 47472 RVA: 0x00548E64 File Offset: 0x00547064
		public void SetVolume(float volume)
		{
			bool flag = null != this.AudioPlayer;
			if (flag)
			{
				this.AudioPlayer.volume = volume;
			}
		}

		// Token: 0x0600B971 RID: 47473 RVA: 0x00548E90 File Offset: 0x00547090
		public void SetVolumeWithFade(float volume, float fadeTime)
		{
			bool flag = null != this.AudioPlayer;
			if (flag)
			{
				DOVirtual.Float(this.AudioPlayer.volume, volume, fadeTime, delegate(float v)
				{
					this.AudioPlayer.volume = v;
				}).SetAutoKill(true);
			}
		}

		// Token: 0x0600B972 RID: 47474 RVA: 0x00548ED8 File Offset: 0x005470D8
		public void Play(AudioCommand cmd, float globalVolume)
		{
			bool flag = !string.IsNullOrEmpty(cmd.AudioName);
			if (flag)
			{
				this._playingCmd = cmd;
			}
			bool flag2 = this._playingCmd != null;
			if (flag2)
			{
				this.internal_Play(globalVolume);
			}
		}

		// Token: 0x0600B973 RID: 47475 RVA: 0x00548F14 File Offset: 0x00547114
		public void Play(AudioCommand cmd)
		{
			float volume = this._isAmbience ? AudioManager.Instance.GetAmbienceVolume() : AudioManager.Instance.GetMusicVolume();
			this.Play(cmd, volume);
		}

		// Token: 0x0600B974 RID: 47476 RVA: 0x00548F4A File Offset: 0x0054714A
		public void Stop()
		{
			this.Time = 0f;
			this.AudioPlayer.Stop();
		}

		// Token: 0x0600B975 RID: 47477 RVA: 0x00548F68 File Offset: 0x00547168
		public void StopWithFade(float fadeTime = 0.3f)
		{
			bool isPlaying = this.AudioPlayer.isPlaying;
			if (isPlaying)
			{
				this.AudioPlayer.DOKill(false);
				Tweener playingFadeInOut = this._playingFadeInOut;
				if (playingFadeInOut != null)
				{
					playingFadeInOut.Kill(false);
				}
				this._playingFadeInOut = DOVirtual.Float(this.AudioPlayer.volume, 0f, fadeTime, delegate(float v)
				{
					this.AudioPlayer.volume = v;
				}).OnComplete(delegate
				{
					this.AudioPlayer.Stop();
					this.AudioPlayer.clip = null;
					this._playingCmd = null;
				}).SetAutoKill(true);
			}
		}

		// Token: 0x0600B976 RID: 47478 RVA: 0x00548FE6 File Offset: 0x005471E6
		public void Pause()
		{
			this.AudioPlayer.Pause();
		}

		// Token: 0x0600B977 RID: 47479 RVA: 0x00548FF5 File Offset: 0x005471F5
		public void Resume()
		{
			this.AudioPlayer.UnPause();
		}

		// Token: 0x0600B978 RID: 47480 RVA: 0x00549004 File Offset: 0x00547204
		public void OnSystemMusicVolumeUpdate()
		{
			float volume = this._isAmbience ? AudioManager.Instance.GetAmbienceVolume() : AudioManager.Instance.GetMusicVolume();
			this.OnSystemMusicVolumeUpdate(volume);
		}

		// Token: 0x0600B979 RID: 47481 RVA: 0x00549039 File Offset: 0x00547239
		public void OnSystemMusicVolumeUpdate(float globalVolume)
		{
			this.AudioPlayer.volume = ((this._playingCmd != null) ? ((float)this._playingCmd.Volume / 100f * globalVolume) : globalVolume);
		}

		// Token: 0x0600B97A RID: 47482 RVA: 0x00549067 File Offset: 0x00547267
		public void OnSystemMusicMuteUpdate(bool mute)
		{
			this.AudioPlayer.mute = mute;
		}

		// Token: 0x0600B97B RID: 47483 RVA: 0x00549078 File Offset: 0x00547278
		public string GetPlayingMusicName()
		{
			return (this._playingCmd != null) ? this._playingCmd.AudioName : "";
		}

		// Token: 0x0600B97C RID: 47484 RVA: 0x005490A4 File Offset: 0x005472A4
		private void internal_Play(float globalVolume)
		{
			bool flag = null == this.AudioPlayer;
			if (!flag)
			{
				Tweener playingFadeInOut = this._playingFadeInOut;
				if (playingFadeInOut != null)
				{
					playingFadeInOut.Kill(false);
				}
				this._playingFadeInOut = null;
				this._globalVolume = globalVolume;
				float targetVolume = (float)this._playingCmd.Volume / 100f * this._globalVolume;
				bool flag2 = this._playingCmd.AudioName == AudioManager.DummyAudioName;
				if (flag2)
				{
					this.StopWithFade(this._playingCmd.FadeTimeOut);
				}
				else
				{
					bool flag3 = null != this.AudioPlayer.clip && this.AudioPlayer.clip.name == this._playingCmd.AudioName;
					if (flag3)
					{
						Tweener playingFadeInOut2 = this._playingFadeInOut;
						if (playingFadeInOut2 != null)
						{
							playingFadeInOut2.Kill(false);
						}
						this._playingFadeInOut = DOVirtual.Float(this.AudioPlayer.volume, targetVolume, this._playingCmd.FadeTimeIn, delegate(float v)
						{
							this.AudioPlayer.volume = v;
						}).SetAutoKill(true);
					}
					else
					{
						bool audioClip = SingletonObject.getInstance<DlcManager>().GetAudioClip(this._playingCmd.AudioName, true, new Action<AudioClip>(this.OnGetMusicClip));
						if (!audioClip)
						{
							AudioInfos.Instance.GetMusic(this._playingCmd.AudioName, new Action<AudioClip>(this.OnGetMusicClip), delegate
							{
								Tweener playingFadeInOut3 = this._playingFadeInOut;
								if (playingFadeInOut3 != null)
								{
									playingFadeInOut3.Kill(false);
								}
								this._playingFadeInOut = DOVirtual.Float(this.AudioPlayer.volume, 0f, this._playingCmd.FadeTimeIn, delegate(float v)
								{
									this.AudioPlayer.volume = v;
								}).SetAutoKill(true);
							});
						}
					}
				}
			}
		}

		// Token: 0x0600B97D RID: 47485 RVA: 0x00549214 File Offset: 0x00547414
		private void OnGetMusicClip(AudioClip clip)
		{
			float targetVolume = (float)this._playingCmd.Volume / 100f * this._globalVolume;
			bool flag = clip.name != this._playingCmd.AudioName;
			if (!flag)
			{
				bool flag2 = null == this.AudioPlayer.clip;
				if (flag2)
				{
					this.AudioPlayer.clip = clip;
					this.AudioPlayer.Play();
					bool setProgress = this.SetProgress;
					if (setProgress)
					{
						this.AudioPlayer.time = this.Time;
					}
					else
					{
						this.Time = 0f;
					}
					Tweener playingFadeInOut = this._playingFadeInOut;
					if (playingFadeInOut != null)
					{
						playingFadeInOut.Kill(false);
					}
					this._playingFadeInOut = DOVirtual.Float(this.AudioPlayer.volume, targetVolume, this._playingCmd.FadeTimeIn, delegate(float v)
					{
						this.AudioPlayer.volume = v;
					}).SetAutoKill(true);
				}
				else
				{
					Tweener playingFadeInOut2 = this._playingFadeInOut;
					if (playingFadeInOut2 != null)
					{
						playingFadeInOut2.Kill(false);
					}
					TweenCallback<float> <>9__3;
					this._playingFadeInOut = DOVirtual.Float(this.AudioPlayer.volume, 0f, this._playingCmd.FadeTimeOut, delegate(float stepValue)
					{
						this.AudioPlayer.volume = stepValue;
					}).OnComplete(delegate
					{
						bool flag3 = this._playingCmd == null;
						if (!flag3)
						{
							bool flag4 = clip.name == this._playingCmd.AudioName;
							if (flag4)
							{
								this.AudioPlayer.clip = clip;
								this.AudioPlayer.Play();
								bool setProgress2 = this.SetProgress;
								if (setProgress2)
								{
									this.AudioPlayer.time = this.Time;
								}
								else
								{
									this.Time = 0f;
								}
							}
							Tweener playingFadeInOut3 = this._playingFadeInOut;
							if (playingFadeInOut3 != null)
							{
								playingFadeInOut3.Kill(false);
							}
							MusicPlayer <>4__this = this;
							float volume = this.AudioPlayer.volume;
							float targetVolume = targetVolume;
							float fadeTimeIn = this._playingCmd.FadeTimeIn;
							TweenCallback<float> onVirtualUpdate;
							if ((onVirtualUpdate = <>9__3) == null)
							{
								onVirtualUpdate = (<>9__3 = delegate(float v)
								{
									this.AudioPlayer.volume = v;
								});
							}
							<>4__this._playingFadeInOut = DOVirtual.Float(volume, targetVolume, fadeTimeIn, onVirtualUpdate);
						}
					}).SetAutoKill(true);
				}
			}
		}

		// Token: 0x0600B97E RID: 47486 RVA: 0x00549388 File Offset: 0x00547588
		private void LateUpdate()
		{
			bool isPlaying = this.AudioPlayer.isPlaying;
			if (isPlaying)
			{
				this.Time += UnityEngine.Time.deltaTime;
				Action<AudioCommandOnPlayeUpdateParam> onPlayUpdate = this._playingCmd.OnPlayUpdate;
				if (onPlayUpdate != null)
				{
					onPlayUpdate(new AudioCommandOnPlayeUpdateParam(this.AudioPlayer, this.Time));
				}
				bool flag = this.Time >= this.AudioPlayer.clip.length;
				if (flag)
				{
					this.Time = 0f;
					bool flag2 = this._playingCmd != null && this._playingCmd.OnPlayFinish != null;
					if (flag2)
					{
						Action<string> onFinish = this._playingCmd.OnPlayFinish;
						this._playingCmd.OnPlayFinish = null;
						onFinish(this.AudioPlayer.clip.name);
					}
				}
			}
		}

		// Token: 0x04008F8F RID: 36751
		public AudioSource AudioPlayer;

		// Token: 0x04008F90 RID: 36752
		private AudioCommand _playingCmd;

		// Token: 0x04008F91 RID: 36753
		[SerializeField]
		private bool _isAmbience;

		// Token: 0x04008F92 RID: 36754
		private float _globalVolume;

		// Token: 0x04008F93 RID: 36755
		private Tweener _playingFadeInOut;
	}
}
