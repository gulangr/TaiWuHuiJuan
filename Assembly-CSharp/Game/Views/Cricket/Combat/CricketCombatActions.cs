using System;
using FrameWork;
using FrameWork.UISystem.UIElements;
using UnityEngine;

namespace Game.Views.Cricket.Combat
{
	// Token: 0x02000AC1 RID: 2753
	public class CricketCombatActions : MonoBehaviour, ICricketCombatComponent
	{
		// Token: 0x17000EDF RID: 3807
		// (get) Token: 0x06008793 RID: 34707 RVA: 0x003F0AC1 File Offset: 0x003EECC1
		// (set) Token: 0x06008794 RID: 34708 RVA: 0x003F0AC9 File Offset: 0x003EECC9
		public ICricketCombatHandler Handler { get; set; }

		// Token: 0x17000EE0 RID: 3808
		// (get) Token: 0x06008795 RID: 34709 RVA: 0x003F0AD2 File Offset: 0x003EECD2
		// (set) Token: 0x06008796 RID: 34710 RVA: 0x003F0ADA File Offset: 0x003EECDA
		public Action OnCancelRequested { private get; set; }

		// Token: 0x17000EE1 RID: 3809
		// (get) Token: 0x06008797 RID: 34711 RVA: 0x003F0AE3 File Offset: 0x003EECE3
		// (set) Token: 0x06008798 RID: 34712 RVA: 0x003F0AEB File Offset: 0x003EECEB
		public Action OnStartCombat { private get; set; }

		// Token: 0x17000EE2 RID: 3810
		// (get) Token: 0x06008799 RID: 34713 RVA: 0x003F0AF4 File Offset: 0x003EECF4
		// (set) Token: 0x0600879A RID: 34714 RVA: 0x003F0AFC File Offset: 0x003EECFC
		public Action OnForceGiveUp { private get; set; }

		// Token: 0x17000EE3 RID: 3811
		// (get) Token: 0x0600879B RID: 34715 RVA: 0x003F0B05 File Offset: 0x003EED05
		public bool IsPaused
		{
			get
			{
				return this.combatTimeScaleToggle.IsPaused;
			}
		}

		// Token: 0x17000EE4 RID: 3812
		// (get) Token: 0x0600879C RID: 34716 RVA: 0x003F0B12 File Offset: 0x003EED12
		public float CurrentTimeScale
		{
			get
			{
				return this.combatTimeScaleToggle.CurrentTimeScale;
			}
		}

		// Token: 0x0600879D RID: 34717 RVA: 0x003F0B20 File Offset: 0x003EED20
		public void OnEvent(ECricketCombatGlobalEventType type, ArgumentBox argBox)
		{
			bool flag = type == ECricketCombatGlobalEventType.Initialize;
			if (flag)
			{
				this._initialized = true;
				this.ResetButtons();
			}
			else
			{
				bool flag2 = type == ECricketCombatGlobalEventType.CombatStatusChanged;
				if (flag2)
				{
					bool flag3 = CricketCombatKit.Board.Status == ECricketCombatStatus.Combating;
					if (flag3)
					{
						this.startCombat.gameObject.SetActive(false);
					}
					this.SetForceGiveUpInteractable(CricketCombatKit.Board.Status == ECricketCombatStatus.Combating);
				}
				else
				{
					bool flag4 = type == ECricketCombatGlobalEventType.ForceGiveUpCheck;
					if (flag4)
					{
						this.SetForceGiveUpInteractable(false);
					}
					else
					{
						bool flag5 = type == ECricketCombatGlobalEventType.ForceGiveUpRefuse;
						if (flag5)
						{
							this.SetForceGiveUpInteractable(true);
						}
					}
				}
			}
		}

		// Token: 0x0600879E RID: 34718 RVA: 0x003F0BB5 File Offset: 0x003EEDB5
		public void SetInCombat(bool inCombat)
		{
			this.combatTimeScaleToggle.SetInCombat(inCombat);
		}

		// Token: 0x0600879F RID: 34719 RVA: 0x003F0BC4 File Offset: 0x003EEDC4
		public void SetDisplayTimeScale(float speed)
		{
			this.combatTimeScaleToggle.SetDisplayTimeScale(speed, false);
		}

		// Token: 0x060087A0 RID: 34720 RVA: 0x003F0BD4 File Offset: 0x003EEDD4
		public void TogglePause()
		{
			this.combatTimeScaleToggle.TogglePause();
		}

		// Token: 0x060087A1 RID: 34721 RVA: 0x003F0BE2 File Offset: 0x003EEDE2
		public bool IncreaseSpeed()
		{
			return this.combatTimeScaleToggle.IncreaseSpeed();
		}

		// Token: 0x060087A2 RID: 34722 RVA: 0x003F0BEF File Offset: 0x003EEDEF
		public bool DecreaseSpeed()
		{
			return this.combatTimeScaleToggle.DecreaseSpeed();
		}

		// Token: 0x060087A3 RID: 34723 RVA: 0x003F0BFC File Offset: 0x003EEDFC
		public void SetForceGiveUpInteractable(bool interactable)
		{
			this.forceGiveUp.interactable = interactable;
			TooltipInvoker tip = this.forceGiveUp.GetComponent<TooltipInvoker>();
			bool flag = tip == null;
			if (!flag)
			{
				tip.enabled = !interactable;
				bool flag2 = !interactable;
				if (flag2)
				{
					tip.Type = TipType.SingleDesc;
					tip.RuntimeParam = new ArgumentBox().Set("arg0", LocalStringManager.Get(LanguageKey.LK_CricketCombat_ForceGiveUp_Tip));
				}
			}
		}

		// Token: 0x060087A4 RID: 34724 RVA: 0x003F0C6C File Offset: 0x003EEE6C
		private void OnEnable()
		{
			bool initialized = this._initialized;
			if (initialized)
			{
				this.ResetButtons();
			}
		}

		// Token: 0x060087A5 RID: 34725 RVA: 0x003F0C8B File Offset: 0x003EEE8B
		private void ResetButtons()
		{
			this.startCombat.gameObject.SetActive(true);
			this.taiwuGiveUp.interactable = true;
			this.SetForceGiveUpInteractable(false);
		}

		// Token: 0x060087A6 RID: 34726 RVA: 0x003F0CB8 File Offset: 0x003EEEB8
		private void Update()
		{
			bool flag = !this._initialized;
			if (!flag)
			{
				bool escHandled = UIManager.Instance.EscHandled;
				if (!escHandled)
				{
					bool exist = UIElement.CharacterMenu.Exist;
					if (!exist)
					{
						bool flag2 = !UIManager.Instance.IsFocusElement(UIElement.CricketCombat);
						if (!flag2)
						{
							bool flag3 = CommonCommandKit.Esc.Check(UIElement.CricketCombat, false, false, false, true, false) || CommonCommandKit.RightMouse.Check(UIElement.CricketCombat, false, false, false, true, false);
							if (flag3)
							{
								Action onCancelRequested = this.OnCancelRequested;
								if (onCancelRequested != null)
								{
									onCancelRequested();
								}
							}
							else
							{
								bool flag4 = CommonCommandKit.Space.Check(UIElement.CricketCombat, false, false, false, true, false);
								if (flag4)
								{
									bool flag5 = this.selectItemPanel != null && this.selectItemPanel.IsShowing;
									if (!flag5)
									{
										bool isInteractable = this.startCombat.IsInteractable;
										if (isInteractable)
										{
											Action onStartCombat = this.OnStartCombat;
											if (onStartCombat != null)
											{
												onStartCombat();
											}
										}
										else
										{
											bool flag6 = this.combatTimeScaleToggle.AreControlsActive && this.combatTimeScaleToggle.PauseInteractable;
											if (flag6)
											{
												this.combatTimeScaleToggle.TogglePause();
											}
										}
									}
								}
								else
								{
									bool flag7 = CombatCommandKit.SpeedUp.Check(UIElement.CricketCombat, false, false, false, false, false) && this.combatTimeScaleToggle.AreControlsActive && this.combatTimeScaleToggle.IsSpeedUpInteractable;
									if (flag7)
									{
										this.combatTimeScaleToggle.IncreaseSpeed();
									}
									else
									{
										bool flag8 = CombatCommandKit.SpeedDown.Check(UIElement.CricketCombat, false, false, false, false, false) && this.combatTimeScaleToggle.AreControlsActive && this.combatTimeScaleToggle.IsSpeedDownInteractable;
										if (flag8)
										{
											this.combatTimeScaleToggle.DecreaseSpeed();
										}
									}
								}
							}
						}
					}
				}
			}
		}

		// Token: 0x0400681E RID: 26654
		[SerializeField]
		private CricketCombatStartButton startCombat;

		// Token: 0x0400681F RID: 26655
		[SerializeField]
		private CButton taiwuGiveUp;

		// Token: 0x04006820 RID: 26656
		[SerializeField]
		private CButton forceGiveUp;

		// Token: 0x04006821 RID: 26657
		[SerializeField]
		private CricketCombatTimeScaleToggle combatTimeScaleToggle;

		// Token: 0x04006822 RID: 26658
		private bool _initialized;

		// Token: 0x04006823 RID: 26659
		[SerializeField]
		private CricketCombatSelectItemPanel selectItemPanel;
	}
}
