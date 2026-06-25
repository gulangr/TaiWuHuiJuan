using System;
using DG.Tweening;
using DG.Tweening.Core;
using DG.Tweening.Plugins.Options;
using FrameWork.UISystem.UIElements;
using GameData.Domains.Combat;
using GameData.Domains.Global;
using TMPro;
using UnityEngine;

namespace Game.Views.Combat
{
	// Token: 0x02000AFE RID: 2814
	public class CombatMercy : MonoBehaviour, ICombatComponent
	{
		// Token: 0x17000F45 RID: 3909
		// (get) Token: 0x06008A81 RID: 35457 RVA: 0x00401FD6 File Offset: 0x004001D6
		private static float MercyPrepareMaxTime
		{
			get
			{
				return GlobalConfig.Instance.MercyPrepareMaxTime;
			}
		}

		// Token: 0x17000F46 RID: 3910
		// (get) Token: 0x06008A82 RID: 35458 RVA: 0x00401FE2 File Offset: 0x004001E2
		private static float MercyAutoIgnoreTime
		{
			get
			{
				return GlobalConfig.Instance.MercyAutoIgnoreTime;
			}
		}

		// Token: 0x17000F47 RID: 3911
		// (get) Token: 0x06008A83 RID: 35459 RVA: 0x00401FEE File Offset: 0x004001EE
		private static float MercyAutoIgnoreTimeFactor
		{
			get
			{
				return CombatMercy.MercyPrepareMaxTime / CombatMercy.MercyAutoIgnoreTime;
			}
		}

		// Token: 0x17000F48 RID: 3912
		// (get) Token: 0x06008A84 RID: 35460 RVA: 0x00401FFB File Offset: 0x004001FB
		private CombatModel Model
		{
			get
			{
				return SingletonObject.getInstance<CombatModel>();
			}
		}

		// Token: 0x06008A85 RID: 35461 RVA: 0x00402004 File Offset: 0x00400204
		public void Setup()
		{
			this.Model.AddEvent(ECombatEvents.OnShowMercyOptionChanged, new OnCombatEvent(this.OnShowMercyOptionChanged));
			this.yesButton.ClearAndAddListener(new Action(this.OnExecuteClicked));
			this.noButton.ClearAndAddListener(new Action(this.OnMercyClicked));
		}

		// Token: 0x06008A86 RID: 35462 RVA: 0x0040205C File Offset: 0x0040025C
		public void Close()
		{
			this.Model.RemoveEvent(ECombatEvents.OnShowMercyOptionChanged, new OnCombatEvent(this.OnShowMercyOptionChanged));
			this.yesButton.RemoveAllListeners();
			this.noButton.RemoveAllListeners();
		}

		// Token: 0x06008A87 RID: 35463 RVA: 0x00402094 File Offset: 0x00400294
		private void OnShowMercyOptionChanged()
		{
			bool showingMercy = this.Model.ShowingMercy;
			if (showingMercy)
			{
				this.OpenMercyPanel();
			}
			else
			{
				this.CloseMercyPanel();
			}
		}

		// Token: 0x06008A88 RID: 35464 RVA: 0x004020C4 File Offset: 0x004002C4
		private void Update()
		{
			bool flag = !this.Model.ShowingMercyIsAlly || !this.ally;
			if (!flag)
			{
				bool mouseButtonDown = Input.GetMouseButtonDown(1);
				if (mouseButtonDown)
				{
					this.OnMercyClicked();
				}
				else
				{
					bool holding = CombatCommandKit.NormalAttack.Check(UIElement.Combat, true, false, false, true, false) || Input.GetKey(KeyCode.Mouse0);
					this._mercyPrepareTime = Mathf.Max(this._mercyPrepareTime + Time.unscaledDeltaTime / CombatMercy.MercyPrepareMaxTime * (holding ? 1f : (-1f * CombatMercy.MercyAutoIgnoreTimeFactor)), 0f);
					this.Model.RaiseEvent(holding ? ECombatEvents.SetSelfLoopAnimationHolding : ECombatEvents.SetSelfLoopAnimationIdle);
					this.killProgress.fillAmount = this._mercyPrepareTime / CombatMercy.MercyPrepareMaxTime;
					bool flag2 = this._mercyPrepareTime < CombatMercy.MercyPrepareMaxTime && this._mercyPrepareTime > 0f;
					if (!flag2)
					{
						CombatDomainMethod.Call.SelectMercyOption(true, this._mercyPrepareTime <= 0f);
						this.CloseMercyPanel();
					}
				}
			}
		}

		// Token: 0x06008A89 RID: 35465 RVA: 0x004021D8 File Offset: 0x004003D8
		private void OpenMercyPanel()
		{
			bool flag = this.Model.ShowingMercyIsAlly != this.ally;
			if (!flag)
			{
				bool flag2 = this.ally;
				if (flag2)
				{
					GlobalDomainMethod.Call.InvokeGuidingTrigger(154);
				}
				this.Model.RaiseEvent(ECombatEvents.DisableInteractByMercy);
				this.hintText.text = ((this.Model.ShowingMercyOption == EShowMercyOption.FuyuSword) ? LanguageKey.LK_Combat_FuyuSwordSave.Tr().ColorReplace() : LanguageKey.LK_Combat_Execute.Tr().ColorReplace());
				string killBackSprite = (this.Model.ShowingMercyOption == EShowMercyOption.FuyuSword) ? "combat_xuanfu_hilt_0" : "combat_xuanfu_bisi_0";
				this.killBack.SetSprite(killBackSprite, true, null);
				string killProgressSprite = (this.Model.ShowingMercyOption == EShowMercyOption.FuyuSword) ? "combat_xuanfu_hilt_1" : "combat_xuanfu_bisi_1";
				this.killProgress.SetSprite(killProgressSprite, true, null);
				this.timeBar.fillAmount = 1f;
				this.killProgress.fillAmount = 0.5f;
				base.gameObject.SetActive(true);
				bool showingMercyIsAlly = this.Model.ShowingMercyIsAlly;
				if (showingMercyIsAlly)
				{
					this._mercyPrepareTime = CombatMercy.MercyPrepareMaxTime / 2f;
				}
				else
				{
					this.Model.RaiseEvent(ECombatEvents.SetEnemyLoopAnimationHolding);
					this.killProgress.DOFillAmount(1f, CombatMercy.MercyPrepareMaxTime / 2f).SetDelay(1f).SetEase(Ease.Linear).OnComplete(new TweenCallback(this.EnemySelectExecute));
				}
			}
		}

		// Token: 0x06008A8A RID: 35466 RVA: 0x00402356 File Offset: 0x00400556
		private void EnemySelectExecute()
		{
			CombatDomainMethod.Call.SelectMercyOption(false, false);
			this.CloseMercyPanel();
		}

		// Token: 0x06008A8B RID: 35467 RVA: 0x00402368 File Offset: 0x00400568
		private void CloseMercyPanel()
		{
			bool activeSelf = base.gameObject.activeSelf;
			if (activeSelf)
			{
				base.gameObject.SetActive(false);
			}
		}

		// Token: 0x06008A8C RID: 35468 RVA: 0x00402394 File Offset: 0x00400594
		private void OnExecuteClicked()
		{
			bool flag = !this.Model.ShowingMercyIsAlly || !this.ally;
			if (!flag)
			{
				CombatDomainMethod.Call.SelectMercyOption(true, false);
				this.CloseMercyPanel();
			}
		}

		// Token: 0x06008A8D RID: 35469 RVA: 0x004023D0 File Offset: 0x004005D0
		private void OnMercyClicked()
		{
			bool flag = !this.Model.ShowingMercyIsAlly || !this.ally;
			if (!flag)
			{
				CombatDomainMethod.Call.SelectMercyOption(true, true);
				this.CloseMercyPanel();
			}
		}

		// Token: 0x04006A40 RID: 27200
		private const float MercyPanelShowTime = 1f;

		// Token: 0x04006A41 RID: 27201
		[SerializeField]
		private bool ally;

		// Token: 0x04006A42 RID: 27202
		[SerializeField]
		private TextMeshProUGUI hintText;

		// Token: 0x04006A43 RID: 27203
		[SerializeField]
		private CImage killBack;

		// Token: 0x04006A44 RID: 27204
		[SerializeField]
		private CImage killProgress;

		// Token: 0x04006A45 RID: 27205
		[SerializeField]
		private CImage timeBar;

		// Token: 0x04006A46 RID: 27206
		[SerializeField]
		private CButton yesButton;

		// Token: 0x04006A47 RID: 27207
		[SerializeField]
		private CButton noButton;

		// Token: 0x04006A48 RID: 27208
		private float _mercyPrepareTime;
	}
}
