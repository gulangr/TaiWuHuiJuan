using System;
using System.Runtime.CompilerServices;
using Config;
using FrameWork.UISystem.UIElements;
using Game.Components.Switch;
using TMPro;
using UnityEngine;
using UnityEngine.Events;

namespace Game.Views.NewGame
{
	// Token: 0x02000800 RID: 2048
	public class NewGameSubPageChallengeItem : MonoBehaviour
	{
		// Token: 0x17000C02 RID: 3074
		// (get) Token: 0x060063EF RID: 25583 RVA: 0x002DCEFD File Offset: 0x002DB0FD
		public CToggle SwitchToggle
		{
			get
			{
				return this.switchToggle;
			}
		}

		// Token: 0x17000C03 RID: 3075
		// (get) Token: 0x060063F0 RID: 25584 RVA: 0x002DCF05 File Offset: 0x002DB105
		// (set) Token: 0x060063F1 RID: 25585 RVA: 0x002DCF0D File Offset: 0x002DB10D
		public ChallengeModeItem Config { get; private set; }

		// Token: 0x060063F2 RID: 25586 RVA: 0x002DCF16 File Offset: 0x002DB116
		private void Awake()
		{
			this.switchToggle.onValueChanged.AddListener(new UnityAction<bool>(this.OnSwitchToggleValueChange));
		}

		// Token: 0x060063F3 RID: 25587 RVA: 0x002DCF36 File Offset: 0x002DB136
		private void OnDisable()
		{
			this.ShowEffectIn(false);
		}

		// Token: 0x060063F4 RID: 25588 RVA: 0x002DCF44 File Offset: 0x002DB144
		public void Init(int templateId, bool isPreview, bool isEnabled, Action<NewGameSubPageChallengeItem, bool> onSettingChangedHandler)
		{
			this.Config = ChallengeMode.Instance.GetItem(templateId);
			this.imageIcon.SetSprite(this.Config.Icon, false, null);
			this.textTitle.text = this.Config.Name;
			this.textPoint.text = (isPreview ? string.Empty : ((this.Config.Point > 0) ? string.Format("+{0}", this.Config.Point) : string.Format("{0}", this.Config.Point)));
			this._onSettingChangedHandler = onSettingChangedHandler;
			bool isRequired = this.Config.Type == EChallengeModeType.Required;
			this.switchToggle.SetWithoutNotify(isEnabled);
			this.switchToggle.gameObject.SetActive(!isRequired);
			this.switchToggle.interactable = !isPreview;
			this.tip.Type = TipType.Simple;
			this.tip.PresetParam = new string[]
			{
				this.Config.Name,
				this.Config.Desc
			};
			this.tipSwitchToggle.enabled = (!isRequired && !isPreview);
			bool flag = this.Config.Type == EChallengeModeType.Optional;
			if (flag)
			{
				this.tipSwitchToggle.PresetParam = new string[]
				{
					LanguageKey.LK_NewGame_Challenge_OptionalTip.Tr()
				};
			}
			else
			{
				bool flag2 = this.Config.Type == EChallengeModeType.Bonus;
				if (flag2)
				{
					string content = this.switchToggle.interactable ? LanguageKey.LK_NewGame_Challenge_BonusTip.Tr() : LanguageKey.LK_NewGame_Challenge_BonusTip_Disabled.Tr();
					this.tipSwitchToggle.PresetParam = new string[]
					{
						content
					};
				}
			}
			bool flag3 = !isRequired && !isPreview;
			if (flag3)
			{
				this.ShowEffectLoop(isEnabled);
			}
		}

		// Token: 0x060063F5 RID: 25589 RVA: 0x002DD11D File Offset: 0x002DB31D
		private void OnSwitchToggleValueChange(bool isOn)
		{
			this._onSettingChangedHandler(this, isOn);
			this.SetIsOnWithoutNotify(isOn);
		}

		// Token: 0x060063F6 RID: 25590 RVA: 0x002DD136 File Offset: 0x002DB336
		public void SetIsOnWithoutNotify(bool isOn)
		{
			this.switchToggle.SetWithoutNotify(isOn);
			this.ShowEffectLoop(isOn);
			this.ShowEffectIn(isOn);
		}

		// Token: 0x060063F7 RID: 25591 RVA: 0x002DD158 File Offset: 0x002DB358
		private void ShowEffectIn(bool isShow)
		{
			if (isShow)
			{
				this.<ShowEffectIn>g__RecycleEffectIn|26_0();
				EChallengeModeType type = this.Config.Type;
				if (!true)
				{
				}
				string text;
				if (type != EChallengeModeType.Optional)
				{
					if (type != EChallengeModeType.Bonus)
					{
						throw new ArgumentOutOfRangeException();
					}
					text = "eff_xuanyu_huoyan_ui_in_003";
				}
				else
				{
					text = "eff_xuanyu_huoyan_ui_in_002";
				}
				if (!true)
				{
				}
				string effectName = text;
				this._objEffectIn = this.effectPlayer.PlayEffectAt(base.transform, effectName, 1f, false);
			}
			else
			{
				this.<ShowEffectIn>g__RecycleEffectIn|26_0();
			}
		}

		// Token: 0x060063F8 RID: 25592 RVA: 0x002DD1D8 File Offset: 0x002DB3D8
		private void ShowEffectLoop(bool isShow)
		{
			if (isShow)
			{
				this.<ShowEffectLoop>g__RecycleEffectLoop|27_0();
				EChallengeModeType type = this.Config.Type;
				if (!true)
				{
				}
				string text;
				if (type != EChallengeModeType.Optional)
				{
					if (type != EChallengeModeType.Bonus)
					{
						throw new ArgumentOutOfRangeException();
					}
					text = "eff_xuanyu_huoyan_ui_loop_003";
				}
				else
				{
					text = "eff_xuanyu_huoyan_ui_loop_002";
				}
				if (!true)
				{
				}
				string effectName = text;
				this._objEffectLoop = this.effectPlayer.PlayEffectAt(base.transform, effectName, 0f, false);
			}
			else
			{
				this.<ShowEffectLoop>g__RecycleEffectLoop|27_0();
			}
		}

		// Token: 0x060063FA RID: 25594 RVA: 0x002DD260 File Offset: 0x002DB460
		[CompilerGenerated]
		private void <ShowEffectIn>g__RecycleEffectIn|26_0()
		{
			bool flag = this._objEffectIn == null;
			if (!flag)
			{
				this.effectPlayer.ReturnEffectObject(this._objEffectIn);
				this._objEffectIn = null;
			}
		}

		// Token: 0x060063FB RID: 25595 RVA: 0x002DD298 File Offset: 0x002DB498
		[CompilerGenerated]
		private void <ShowEffectLoop>g__RecycleEffectLoop|27_0()
		{
			bool flag = this._objEffectLoop == null;
			if (!flag)
			{
				this.effectPlayer.ReturnEffectObject(this._objEffectLoop);
				this._objEffectLoop = null;
			}
		}

		// Token: 0x040045D2 RID: 17874
		[SerializeField]
		private CImage imageIcon;

		// Token: 0x040045D3 RID: 17875
		[SerializeField]
		private TextMeshProUGUI textTitle;

		// Token: 0x040045D4 RID: 17876
		[SerializeField]
		private TextMeshProUGUI textPoint;

		// Token: 0x040045D5 RID: 17877
		[SerializeField]
		private SwitchToggleSmall switchToggle;

		// Token: 0x040045D6 RID: 17878
		[SerializeField]
		private TooltipInvoker tip;

		// Token: 0x040045D7 RID: 17879
		[SerializeField]
		private TooltipInvoker tipSwitchToggle;

		// Token: 0x040045D8 RID: 17880
		[SerializeField]
		private EffectPlayer effectPlayer;

		// Token: 0x040045D9 RID: 17881
		private Action<NewGameSubPageChallengeItem, bool> _onSettingChangedHandler;

		// Token: 0x040045DB RID: 17883
		private bool _interactable;

		// Token: 0x040045DC RID: 17884
		private GameObject _objEffectIn;

		// Token: 0x040045DD RID: 17885
		private GameObject _objEffectLoop;

		// Token: 0x040045DE RID: 17886
		private const string OptionalInEffectName = "eff_xuanyu_huoyan_ui_in_002";

		// Token: 0x040045DF RID: 17887
		private const string OptionalLoopEffectName = "eff_xuanyu_huoyan_ui_loop_002";

		// Token: 0x040045E0 RID: 17888
		private const string BonusInEffectName = "eff_xuanyu_huoyan_ui_in_003";

		// Token: 0x040045E1 RID: 17889
		private const string BonusLoopEffectName = "eff_xuanyu_huoyan_ui_loop_003";
	}
}
