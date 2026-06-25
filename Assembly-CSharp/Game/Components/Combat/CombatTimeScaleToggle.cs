using System;
using System.Collections.Generic;
using System.Diagnostics;
using FrameWork;
using FrameWork.UISystem.UIElements;
using TMPro;
using UnityEngine;

namespace Game.Components.Combat
{
	// Token: 0x02000F04 RID: 3844
	public class CombatTimeScaleToggle : MonoBehaviour
	{
		// Token: 0x0600B131 RID: 45361 RVA: 0x0050C5A8 File Offset: 0x0050A7A8
		public static float ClampTimeScale(float timeScale)
		{
			return (timeScale < CombatTimeScaleToggle.AvailableTimeScales[0]) ? 1f : timeScale;
		}

		// Token: 0x14000089 RID: 137
		// (add) Token: 0x0600B132 RID: 45362 RVA: 0x0050C5BC File Offset: 0x0050A7BC
		// (remove) Token: 0x0600B133 RID: 45363 RVA: 0x0050C5F4 File Offset: 0x0050A7F4
		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		public event Action<bool> OnPauseChangedEvent;

		// Token: 0x1400008A RID: 138
		// (add) Token: 0x0600B134 RID: 45364 RVA: 0x0050C62C File Offset: 0x0050A82C
		// (remove) Token: 0x0600B135 RID: 45365 RVA: 0x0050C664 File Offset: 0x0050A864
		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		public event Action<float, bool> OnDisplayTimeScaleChangedEvent;

		// Token: 0x17001410 RID: 5136
		// (get) Token: 0x0600B136 RID: 45366 RVA: 0x0050C699 File Offset: 0x0050A899
		public bool IsPaused
		{
			get
			{
				return this.toggle.isOn;
			}
		}

		// Token: 0x17001411 RID: 5137
		// (get) Token: 0x0600B137 RID: 45367 RVA: 0x0050C6A6 File Offset: 0x0050A8A6
		// (set) Token: 0x0600B138 RID: 45368 RVA: 0x0050C6B3 File Offset: 0x0050A8B3
		public bool PauseInteractable
		{
			get
			{
				return this.toggle.interactable;
			}
			set
			{
				this.toggle.interactable = value;
			}
		}

		// Token: 0x17001412 RID: 5138
		// (get) Token: 0x0600B139 RID: 45369 RVA: 0x0050C6C2 File Offset: 0x0050A8C2
		public float DisplayTimeScale
		{
			get
			{
				return this._displayTimeScale;
			}
		}

		// Token: 0x17001413 RID: 5139
		// (get) Token: 0x0600B13A RID: 45370 RVA: 0x0050C6CA File Offset: 0x0050A8CA
		public float CurrentTimeScale
		{
			get
			{
				return this.IsPaused ? 0f : this._displayTimeScale;
			}
		}

		// Token: 0x17001414 RID: 5140
		// (get) Token: 0x0600B13B RID: 45371 RVA: 0x0050C6E1 File Offset: 0x0050A8E1
		public bool AreControlsActive
		{
			get
			{
				return this.toggle != null && this.toggle.gameObject.activeSelf;
			}
		}

		// Token: 0x17001415 RID: 5141
		// (get) Token: 0x0600B13C RID: 45372 RVA: 0x0050C704 File Offset: 0x0050A904
		public bool IsSpeedUpInteractable
		{
			get
			{
				return this.speedUp != null && this.speedUp.interactable;
			}
		}

		// Token: 0x17001416 RID: 5142
		// (get) Token: 0x0600B13D RID: 45373 RVA: 0x0050C722 File Offset: 0x0050A922
		public bool IsSpeedDownInteractable
		{
			get
			{
				return this.speedDown != null && this.speedDown.interactable;
			}
		}

		// Token: 0x0600B13E RID: 45374 RVA: 0x0050C740 File Offset: 0x0050A940
		private void Awake()
		{
			this.toggle.onValueChanged.ResetListener(new Action<bool>(this.OnTogglePauseChanged));
			this.ApplyPauseState(this.IsPaused, false);
		}

		// Token: 0x0600B13F RID: 45375 RVA: 0x0050C76E File Offset: 0x0050A96E
		private void OnEnable()
		{
			GEvent.Add(UiEvents.CombatSpeedChanged, new GEvent.Callback(this.OnCombatSpeedChanged));
		}

		// Token: 0x0600B140 RID: 45376 RVA: 0x0050C78D File Offset: 0x0050A98D
		private void OnDisable()
		{
			GEvent.Remove(UiEvents.CombatSpeedChanged, new GEvent.Callback(this.OnCombatSpeedChanged));
		}

		// Token: 0x0600B141 RID: 45377 RVA: 0x0050C7AC File Offset: 0x0050A9AC
		private void OnCombatSpeedChanged(ArgumentBox _)
		{
			this.SetDisplayTimeScale(SingletonObject.getInstance<GlobalSettings>().CombatSpeed, false);
		}

		// Token: 0x0600B142 RID: 45378 RVA: 0x0050C7C1 File Offset: 0x0050A9C1
		public void SetWrapTimeScale(bool wrap)
		{
			this._wrapTimeScale = wrap;
		}

		// Token: 0x0600B143 RID: 45379 RVA: 0x0050C7CC File Offset: 0x0050A9CC
		public void AddExtraPauseTip(GameObject tip)
		{
			bool flag = tip != null && !this._extraPauseTips.Contains(tip);
			if (flag)
			{
				this._extraPauseTips.Add(tip);
			}
		}

		// Token: 0x0600B144 RID: 45380 RVA: 0x0050C808 File Offset: 0x0050AA08
		public void SetPause(bool isPaused)
		{
			bool flag = this.toggle.isOn != isPaused;
			if (flag)
			{
				this.toggle.isOn = isPaused;
			}
			else
			{
				this.ApplyPauseState(isPaused, false);
			}
		}

		// Token: 0x0600B145 RID: 45381 RVA: 0x0050C844 File Offset: 0x0050AA44
		public void TogglePause()
		{
			bool flag = !this.PauseInteractable;
			if (!flag)
			{
				this.SetPause(!this.IsPaused);
			}
		}

		// Token: 0x0600B146 RID: 45382 RVA: 0x0050C871 File Offset: 0x0050AA71
		public bool IncreaseSpeed()
		{
			return this.ChangeSpeed(false);
		}

		// Token: 0x0600B147 RID: 45383 RVA: 0x0050C87A File Offset: 0x0050AA7A
		public bool DecreaseSpeed()
		{
			return this.ChangeSpeed(true);
		}

		// Token: 0x0600B148 RID: 45384 RVA: 0x0050C884 File Offset: 0x0050AA84
		public void SetDisplayTimeScale(float timeScale, bool init = false)
		{
			int index = this.GetTimeScaleIndex(timeScale);
			float normalizedTimeScale = CombatTimeScaleToggle.AvailableTimeScales[index];
			bool changed = !Mathf.Approximately(this._displayTimeScale, normalizedTimeScale);
			this._displayTimeScale = normalizedTimeScale;
			this.RefreshDisplayTimeScale(index);
			bool flag = init || changed;
			if (flag)
			{
				Action<float, bool> onDisplayTimeScaleChangedEvent = this.OnDisplayTimeScaleChangedEvent;
				if (onDisplayTimeScaleChangedEvent != null)
				{
					onDisplayTimeScaleChangedEvent(this._displayTimeScale, init);
				}
			}
		}

		// Token: 0x0600B149 RID: 45385 RVA: 0x0050C8E4 File Offset: 0x0050AAE4
		public void SetControlsActive(bool active)
		{
			this.toggle.gameObject.SetActive(active);
			bool flag = this.speedDown != null;
			if (flag)
			{
				this.speedDown.gameObject.SetActive(active);
			}
			bool flag2 = this.speedUp != null;
			if (flag2)
			{
				this.speedUp.gameObject.SetActive(active);
			}
		}

		// Token: 0x0600B14A RID: 45386 RVA: 0x0050C948 File Offset: 0x0050AB48
		private void OnTogglePauseChanged(bool isPaused)
		{
			this.ApplyPauseState(isPaused, true);
		}

		// Token: 0x0600B14B RID: 45387 RVA: 0x0050C954 File Offset: 0x0050AB54
		private void ApplyPauseState(bool isPaused, bool notify)
		{
			bool flag = this.pauseTips != null;
			if (flag)
			{
				this.pauseTips.SetActive(isPaused);
			}
			bool flag2 = this.extraPauseTipsArray != null;
			if (flag2)
			{
				foreach (GameObject extraPauseTip in this.extraPauseTipsArray)
				{
					extraPauseTip.SetActive(isPaused);
				}
			}
			foreach (GameObject extraPauseTip2 in this._extraPauseTips)
			{
				extraPauseTip2.SetActive(isPaused);
			}
			if (notify)
			{
				Action<bool> onPauseChangedEvent = this.OnPauseChangedEvent;
				if (onPauseChangedEvent != null)
				{
					onPauseChangedEvent(isPaused);
				}
			}
		}

		// Token: 0x0600B14C RID: 45388 RVA: 0x0050CA18 File Offset: 0x0050AC18
		private bool ChangeSpeed(bool decrease)
		{
			int currentIndex = this.GetTimeScaleIndex(this._displayTimeScale);
			int targetIndex = currentIndex + (decrease ? -1 : 1);
			bool wrapTimeScale = this._wrapTimeScale;
			if (wrapTimeScale)
			{
				bool flag = targetIndex < 0;
				if (flag)
				{
					targetIndex = CombatTimeScaleToggle.AvailableTimeScales.Length - 1;
				}
				else
				{
					bool flag2 = targetIndex >= CombatTimeScaleToggle.AvailableTimeScales.Length;
					if (flag2)
					{
						targetIndex = 0;
					}
				}
			}
			else
			{
				bool flag3 = targetIndex < 0 || targetIndex >= CombatTimeScaleToggle.AvailableTimeScales.Length;
				if (flag3)
				{
					return false;
				}
			}
			this.SetDisplayTimeScale(CombatTimeScaleToggle.AvailableTimeScales[targetIndex], false);
			return true;
		}

		// Token: 0x0600B14D RID: 45389 RVA: 0x0050CAAC File Offset: 0x0050ACAC
		private int GetTimeScaleIndex(float timeScale)
		{
			int index = Array.IndexOf<float>(CombatTimeScaleToggle.AvailableTimeScales, timeScale);
			bool flag = index >= 0;
			int result;
			if (flag)
			{
				result = index;
			}
			else
			{
				float normalizedTimeScale = CombatTimeScaleToggle.ClampTimeScale(timeScale);
				index = Array.IndexOf<float>(CombatTimeScaleToggle.AvailableTimeScales, normalizedTimeScale);
				result = ((index >= 0) ? index : 1);
			}
			return result;
		}

		// Token: 0x0600B14E RID: 45390 RVA: 0x0050CAF4 File Offset: 0x0050ACF4
		private void RefreshDisplayTimeScale(int speedIndex)
		{
			bool flag = this.speed != null;
			if (flag)
			{
				this.speed.text = string.Format("X{0}", this._displayTimeScale);
			}
			bool flag2 = this.speedDown != null;
			if (flag2)
			{
				this.speedDown.interactable = (speedIndex > 0 || this._wrapTimeScale);
				DisableStyleRoot component = this.speedDown.GetComponent<DisableStyleRoot>();
				if (component != null)
				{
					component.SetStyleEffect(!this.speedDown.interactable, false);
				}
			}
			bool flag3 = this.speedUp != null;
			if (flag3)
			{
				this.speedUp.interactable = (speedIndex < CombatTimeScaleToggle.AvailableTimeScales.Length - 1 || this._wrapTimeScale);
				DisableStyleRoot component2 = this.speedUp.GetComponent<DisableStyleRoot>();
				if (component2 != null)
				{
					component2.SetStyleEffect(!this.speedUp.interactable, false);
				}
			}
		}

		// Token: 0x0400895F RID: 35167
		public static readonly float[] AvailableTimeScales = new float[]
		{
			0.5f,
			1f,
			1.5f,
			2f,
			3f
		};

		// Token: 0x04008960 RID: 35168
		[SerializeField]
		private CToggle toggle;

		// Token: 0x04008961 RID: 35169
		[SerializeField]
		private TextMeshProUGUI speed;

		// Token: 0x04008962 RID: 35170
		[SerializeField]
		private CButton speedUp;

		// Token: 0x04008963 RID: 35171
		[SerializeField]
		private CButton speedDown;

		// Token: 0x04008964 RID: 35172
		[SerializeField]
		private GameObject pauseTips;

		// Token: 0x04008965 RID: 35173
		[SerializeField]
		private GameObject[] extraPauseTipsArray;

		// Token: 0x04008966 RID: 35174
		private readonly List<GameObject> _extraPauseTips = new List<GameObject>();

		// Token: 0x04008967 RID: 35175
		private float _displayTimeScale = 1f;

		// Token: 0x04008968 RID: 35176
		private bool _wrapTimeScale;
	}
}
