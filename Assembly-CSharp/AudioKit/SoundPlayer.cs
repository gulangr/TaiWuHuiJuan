using System;
using UnityEngine;

namespace AudioKit
{
	// Token: 0x02000FDA RID: 4058
	[RequireComponent(typeof(AudioSource))]
	public class SoundPlayer : MonoBehaviour
	{
		// Token: 0x170014F3 RID: 5363
		// (get) Token: 0x0600B987 RID: 47495 RVA: 0x0054952D File Offset: 0x0054772D
		// (set) Token: 0x0600B988 RID: 47496 RVA: 0x00549535 File Offset: 0x00547735
		public bool CanSetPitchByGlobal { get; private set; }

		// Token: 0x0600B989 RID: 47497 RVA: 0x00549540 File Offset: 0x00547740
		private void Awake()
		{
			this.AudioPlayer = base.GetComponent<AudioSource>();
			bool flag = null == this.AudioPlayer;
			if (flag)
			{
				this.AudioPlayer = base.gameObject.AddComponent<AudioSource>();
			}
		}

		// Token: 0x0600B98A RID: 47498 RVA: 0x0054957C File Offset: 0x0054777C
		private void LateUpdate()
		{
			bool working = this._working;
			if (working)
			{
				this._time += Time.deltaTime;
				Action<AudioCommandOnPlayeUpdateParam> onPlayUpdate = this._onPlayUpdate;
				if (onPlayUpdate != null)
				{
					onPlayUpdate(new AudioCommandOnPlayeUpdateParam(this.AudioPlayer, this._time));
				}
				bool flag = !this.AudioPlayer.isPlaying && !this._pause;
				if (flag)
				{
					Action<string> onPlayFinish = this._onPlayFinish;
					if (onPlayFinish != null)
					{
						onPlayFinish(this.AudioPlayer.clip.name);
					}
					this.Stop();
				}
			}
		}

		// Token: 0x0600B98B RID: 47499 RVA: 0x00549614 File Offset: 0x00547814
		public void Play(AudioCommand cmd)
		{
			SoundPlayer.<>c__DisplayClass13_0 CS$<>8__locals1 = new SoundPlayer.<>c__DisplayClass13_0();
			CS$<>8__locals1.<>4__this = this;
			CS$<>8__locals1.cmd = cmd;
			this._time = 0f;
			this._audioName = CS$<>8__locals1.cmd.AudioName;
			this.AudioPlayer.volume = (float)CS$<>8__locals1.cmd.Volume / 100f * AudioManager.Instance.GetSoundVolume();
			this.AudioPlayer.pitch = CS$<>8__locals1.cmd.Pitch;
			this.AudioPlayer.loop = CS$<>8__locals1.cmd.Loop;
			this.CanSetPitchByGlobal = CS$<>8__locals1.cmd.CanSetPitchByGlobal;
			this._onPlayFinish = CS$<>8__locals1.cmd.OnPlayFinish;
			this._onPlayUpdate = CS$<>8__locals1.cmd.OnPlayUpdate;
			AudioClip clip = (CS$<>8__locals1.cmd.Clip != null) ? CS$<>8__locals1.cmd.Clip : AudioInfos.Instance.GetClip(CS$<>8__locals1.cmd.AudioName, "");
			bool flag = null != clip && clip.loadState == AudioDataLoadState.Loaded;
			if (flag)
			{
				this._working = true;
				this.AudioPlayer.clip = clip;
				this.AudioPlayer.mute = AudioManager.Instance.GetEffectiveSoundMute();
				this.AudioPlayer.Play();
			}
			else
			{
				bool audioClip = SingletonObject.getInstance<DlcManager>().GetAudioClip(CS$<>8__locals1.cmd.AudioName, false, new Action<AudioClip>(CS$<>8__locals1.<Play>g__OnGetClip|0));
				if (!audioClip)
				{
					string packageName = AudioInfos.Instance.GetClipPackageName(CS$<>8__locals1.cmd.AudioName);
					bool flag2 = !string.IsNullOrEmpty(packageName);
					if (flag2)
					{
						AudioInfos.Instance.LoadPackage(packageName, delegate
						{
							base.<Play>g__OnGetClip|0(AudioInfos.Instance.GetClip(CS$<>8__locals1.cmd.AudioName, ""));
						});
					}
				}
			}
		}

		// Token: 0x0600B98C RID: 47500 RVA: 0x005497D7 File Offset: 0x005479D7
		public void Stop()
		{
			this._audioName = null;
			this._pause = false;
			this._working = false;
			this.AudioPlayer.clip = null;
			AudioManager.Instance.ReleaseSoundPlayer(this);
		}

		// Token: 0x0600B98D RID: 47501 RVA: 0x00549808 File Offset: 0x00547A08
		public void Pause()
		{
			this._pause = true;
			this.AudioPlayer.Pause();
		}

		// Token: 0x0600B98E RID: 47502 RVA: 0x0054981E File Offset: 0x00547A1E
		public void Resume()
		{
			this._pause = false;
			this.AudioPlayer.UnPause();
		}

		// Token: 0x0600B98F RID: 47503 RVA: 0x00549834 File Offset: 0x00547A34
		public bool IsPlaying(string audioName)
		{
			return !audioName.IsNullOrEmpty() && this._audioName == audioName;
		}

		// Token: 0x04008F98 RID: 36760
		public AudioSource AudioPlayer;

		// Token: 0x04008F9A RID: 36762
		private Action<string> _onPlayFinish;

		// Token: 0x04008F9B RID: 36763
		private Action<AudioCommandOnPlayeUpdateParam> _onPlayUpdate;

		// Token: 0x04008F9C RID: 36764
		private bool _working = false;

		// Token: 0x04008F9D RID: 36765
		private bool _pause = false;

		// Token: 0x04008F9E RID: 36766
		private string _audioName;

		// Token: 0x04008F9F RID: 36767
		private float _time;
	}
}
