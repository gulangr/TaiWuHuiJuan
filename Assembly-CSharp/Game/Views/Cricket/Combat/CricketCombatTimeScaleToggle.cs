using System;
using DG.Tweening;
using FrameWork;
using Game.Components.Combat;
using GameData.Combat.Cricket;
using UnityEngine;

namespace Game.Views.Cricket.Combat
{
	// Token: 0x02000AD6 RID: 2774
	[RequireComponent(typeof(CombatTimeScaleToggle))]
	public class CricketCombatTimeScaleToggle : MonoBehaviour, ICricketCombatComponent
	{
		// Token: 0x17000F0A RID: 3850
		// (get) Token: 0x0600888C RID: 34956 RVA: 0x003F4CA9 File Offset: 0x003F2EA9
		private GlobalSettings SettingData
		{
			get
			{
				return SingletonObject.getInstance<GlobalSettings>();
			}
		}

		// Token: 0x17000F0B RID: 3851
		// (get) Token: 0x0600888D RID: 34957 RVA: 0x003F4CB0 File Offset: 0x003F2EB0
		// (set) Token: 0x0600888E RID: 34958 RVA: 0x003F4CB8 File Offset: 0x003F2EB8
		public ICricketCombatHandler Handler { private get; set; }

		// Token: 0x17000F0C RID: 3852
		// (get) Token: 0x0600888F RID: 34959 RVA: 0x003F4CC4 File Offset: 0x003F2EC4
		private CombatTimeScaleToggle Base
		{
			get
			{
				bool flag = this.baseToggle == null;
				if (flag)
				{
					this.baseToggle = base.GetComponent<CombatTimeScaleToggle>();
					this.baseToggle.SetWrapTimeScale(false);
					this.SubscribeEvents();
				}
				return this.baseToggle;
			}
		}

		// Token: 0x17000F0D RID: 3853
		// (get) Token: 0x06008890 RID: 34960 RVA: 0x003F4D0E File Offset: 0x003F2F0E
		public bool IsPaused
		{
			get
			{
				return this.Base.IsPaused;
			}
		}

		// Token: 0x17000F0E RID: 3854
		// (get) Token: 0x06008891 RID: 34961 RVA: 0x003F4D1B File Offset: 0x003F2F1B
		public float CurrentTimeScale
		{
			get
			{
				return this.Base.CurrentTimeScale;
			}
		}

		// Token: 0x17000F0F RID: 3855
		// (get) Token: 0x06008892 RID: 34962 RVA: 0x003F4D28 File Offset: 0x003F2F28
		public bool AreControlsActive
		{
			get
			{
				return this.Base.AreControlsActive;
			}
		}

		// Token: 0x17000F10 RID: 3856
		// (get) Token: 0x06008893 RID: 34963 RVA: 0x003F4D35 File Offset: 0x003F2F35
		public bool PauseInteractable
		{
			get
			{
				return this.Base.PauseInteractable;
			}
		}

		// Token: 0x17000F11 RID: 3857
		// (get) Token: 0x06008894 RID: 34964 RVA: 0x003F4D42 File Offset: 0x003F2F42
		public bool IsSpeedUpInteractable
		{
			get
			{
				return this.Base.IsSpeedUpInteractable;
			}
		}

		// Token: 0x17000F12 RID: 3858
		// (get) Token: 0x06008895 RID: 34965 RVA: 0x003F4D4F File Offset: 0x003F2F4F
		public bool IsSpeedDownInteractable
		{
			get
			{
				return this.Base.IsSpeedDownInteractable;
			}
		}

		// Token: 0x06008896 RID: 34966 RVA: 0x003F4D5C File Offset: 0x003F2F5C
		private void SubscribeEvents()
		{
			bool flag = !this.eventsSubscribed && this.baseToggle != null;
			if (flag)
			{
				this.baseToggle.OnPauseChangedEvent += this.OnPauseChanged;
				this.baseToggle.OnDisplayTimeScaleChangedEvent += this.OnDisplayTimeScaleChanged;
				this.eventsSubscribed = true;
			}
		}

		// Token: 0x06008897 RID: 34967 RVA: 0x003F4DC0 File Offset: 0x003F2FC0
		private void OnDestroy()
		{
			bool flag = this.baseToggle != null;
			if (flag)
			{
				this.baseToggle.OnPauseChangedEvent -= this.OnPauseChanged;
				this.baseToggle.OnDisplayTimeScaleChangedEvent -= this.OnDisplayTimeScaleChanged;
			}
		}

		// Token: 0x06008898 RID: 34968 RVA: 0x003F4E10 File Offset: 0x003F3010
		public void OnEvent(ECricketCombatGlobalEventType type, ArgumentBox argBox)
		{
			bool flag = type == ECricketCombatGlobalEventType.Initialize;
			if (flag)
			{
				this.ResetState();
			}
		}

		// Token: 0x06008899 RID: 34969 RVA: 0x003F4E30 File Offset: 0x003F3030
		public Sequence HandleLog(CricketCombatLog log, Sequence sequence)
		{
			bool flag = log.Type == ECricketCombatLogEventType.CombatStart;
			if (flag)
			{
				this.Base.PauseInteractable = true;
			}
			else
			{
				bool flag2 = log.Type == ECricketCombatLogEventType.CombatEnd;
				if (flag2)
				{
					this.Base.PauseInteractable = false;
					this.Base.SetPause(false);
				}
			}
			return sequence;
		}

		// Token: 0x0600889A RID: 34970 RVA: 0x003F4E88 File Offset: 0x003F3088
		public void SetInCombat(bool inCombat)
		{
			this.Base.SetControlsActive(inCombat);
		}

		// Token: 0x0600889B RID: 34971 RVA: 0x003F4E97 File Offset: 0x003F3097
		public void SetDisplayTimeScale(float timeScale, bool init = false)
		{
			this.Base.SetDisplayTimeScale(timeScale, init);
		}

		// Token: 0x0600889C RID: 34972 RVA: 0x003F4EA7 File Offset: 0x003F30A7
		public void TogglePause()
		{
			this.Base.TogglePause();
		}

		// Token: 0x0600889D RID: 34973 RVA: 0x003F4EB5 File Offset: 0x003F30B5
		public bool IncreaseSpeed()
		{
			return this.Base.IncreaseSpeed();
		}

		// Token: 0x0600889E RID: 34974 RVA: 0x003F4EC2 File Offset: 0x003F30C2
		public bool DecreaseSpeed()
		{
			return this.Base.DecreaseSpeed();
		}

		// Token: 0x0600889F RID: 34975 RVA: 0x003F4ECF File Offset: 0x003F30CF
		private void ResetState()
		{
			this.Base.SetPause(false);
			this.Base.SetDisplayTimeScale(1f, true);
			this.Base.PauseInteractable = false;
			this.Base.SetControlsActive(false);
		}

		// Token: 0x060088A0 RID: 34976 RVA: 0x003F4F0C File Offset: 0x003F310C
		private void OnPauseChanged(bool isPaused)
		{
			Time.timeScale = (isPaused ? 0f : this.baseToggle.DisplayTimeScale);
			bool flag = this.Handler == null;
			if (!flag)
			{
				ArgumentBox argBox = EasyPool.Get<ArgumentBox>();
				argBox.Set("NowPauseState", isPaused);
				this.Handler.OnEvent(ECricketCombatGlobalEventType.PauseStateChanged, argBox);
				EasyPool.Free<ArgumentBox>(argBox);
			}
		}

		// Token: 0x060088A1 RID: 34977 RVA: 0x003F4F70 File Offset: 0x003F3170
		private void OnDisplayTimeScaleChanged(float timeScale, bool init)
		{
			bool flag = !this.baseToggle.IsPaused;
			if (flag)
			{
				Time.timeScale = timeScale;
			}
			bool flag2 = !init && this.SettingData.AutoSaveCricketCombatSpeed;
			if (flag2)
			{
				this.SettingData.CricketCombatSpeed = timeScale;
			}
		}

		// Token: 0x04006896 RID: 26774
		private CombatTimeScaleToggle baseToggle;

		// Token: 0x04006897 RID: 26775
		private bool eventsSubscribed;
	}
}
