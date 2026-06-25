using System;
using System.Runtime.CompilerServices;
using CharacterDataMonitor;
using FrameWork;
using GameData.Domains.Character;
using TMPro;
using UnityEngine;

namespace UICommon.Character
{
	// Token: 0x020005CE RID: 1486
	public class CharacterDisorderOfQi : CharacterUIElement
	{
		// Token: 0x170008DF RID: 2271
		// (get) Token: 0x06004665 RID: 18021 RVA: 0x00210211 File Offset: 0x0020E411
		private DisorderOfQiMonitor Item
		{
			get
			{
				return this.MonitorDataItem as DisorderOfQiMonitor;
			}
		}

		// Token: 0x06004666 RID: 18022 RVA: 0x00210220 File Offset: 0x0020E420
		public CharacterDisorderOfQi(CSliderLegacy slider, TextMeshProUGUI stateLabel, CImage stateIcon, QiContainer qiContainer = null, TooltipInvoker mouseTip = null)
		{
			bool flag = null == slider && null == stateLabel;
			if (flag)
			{
				throw new Exception("CharacterDisorderOfQi must handle at least one UIElement!");
			}
			this._slider = slider;
			this._stateLabel = stateLabel;
			this._stateIcon = stateIcon;
			this._qiContainer = qiContainer;
			CharacterDisorderOfQi.InitDisorderOfQi(slider, stateLabel);
			this._mouseTip = mouseTip;
			bool flag2 = null != mouseTip;
			if (flag2)
			{
				mouseTip.PresetParam = new string[2];
				mouseTip.PresetParam[0] = LocalStringManager.Get(LanguageKey.LK_Qi_Disorder_StateTitle);
			}
		}

		// Token: 0x06004667 RID: 18023 RVA: 0x002102B4 File Offset: 0x0020E4B4
		public static void InitDisorderOfQi(CSliderLegacy slider, TextMeshProUGUI stateLabel)
		{
			bool flag = null != slider;
			if (flag)
			{
				slider.wholeNumbers = true;
				slider.maxValue = (float)DisorderLevelOfQi.MaxValue;
				slider.value = 0f;
			}
			bool flag2 = null != stateLabel;
			if (flag2)
			{
				stateLabel.text = string.Empty;
			}
		}

		// Token: 0x06004668 RID: 18024 RVA: 0x00210308 File Offset: 0x0020E508
		public override MonitorDataItemBase GetMonitorItem(int charId)
		{
			return SingletonObject.getInstance<CharacterMonitorModel>().GetMonitorItem<DisorderOfQiMonitor>(charId, this.IsDead);
		}

		// Token: 0x06004669 RID: 18025 RVA: 0x0021032B File Offset: 0x0020E52B
		internal override void BindEvent()
		{
			this.Item.AddDisorderOfQiListener(new Action(this.FillElement));
			this.Item.AddChangeOfQiDisorderListener(new Action(this.OnGetChangeOfDisorderOfQi));
		}

		// Token: 0x0600466A RID: 18026 RVA: 0x0021035F File Offset: 0x0020E55F
		public override void UnbindEvent()
		{
			this.Item.RemoveDisorderOfQiListener(new Action(this.FillElement));
			this.Item.RemoveChangeOfQiDisorderListener(new Action(this.OnGetChangeOfDisorderOfQi));
		}

		// Token: 0x0600466B RID: 18027 RVA: 0x00210394 File Offset: 0x0020E594
		public override void FillElement()
		{
			bool flag = null == this._slider && null == this._stateLabel;
			if (flag)
			{
				base.CharacterId = -1;
			}
			else
			{
				bool flag2 = null != this._slider;
				if (flag2)
				{
					this._slider.value = (float)this.Item.DisorderOfQi;
					Action<float> onFillDisorderOfQi = this.OnFillDisorderOfQi;
					if (onFillDisorderOfQi != null)
					{
						onFillDisorderOfQi(this._slider.value / this._slider.maxValue);
					}
				}
				sbyte level = DisorderLevelOfQi.GetDisorderLevelOfQi(this.Item.DisorderOfQi);
				bool flag3 = null != this._stateLabel;
				if (flag3)
				{
					this._stateLabel.text = LocalStringManager.Get(CharacterDisorderOfQi.DisorderOfQiLevelLangKeys[(int)level]);
					this._stateLabel.color = CharacterDisorderOfQi.DisorderOfQiLevelColors[(int)level];
				}
				bool flag4 = null != this._stateIcon;
				if (flag4)
				{
					this._stateIcon.SetSprite(CharacterDisorderOfQi.GetDisorderOfQiLevelIcon(level), false, null);
				}
				QiContainer qiContainer = this._qiContainer;
				if (qiContainer != null)
				{
					qiContainer.OnValueChanged((int)this.Item.DisorderOfQi);
				}
				this.OnGetChangeOfDisorderOfQi();
			}
		}

		// Token: 0x0600466C RID: 18028 RVA: 0x002104C4 File Offset: 0x0020E6C4
		private void OnGetChangeOfDisorderOfQi()
		{
			bool flag = null != this._mouseTip;
			if (flag)
			{
				this._mouseTip.enabled = true;
				this._mouseTip.Type = TipType.DisorderOfQi;
				TooltipInvoker mouseTip = this._mouseTip;
				ArgumentBox argumentBox;
				if ((argumentBox = mouseTip.RuntimeParam) == null)
				{
					argumentBox = (mouseTip.RuntimeParam = new ArgumentBox());
				}
				ArgumentBox args = argumentBox;
				args.Clear();
				args.Set("CharId", base.CharacterId);
			}
			Action<short> onFillChangeOfDisorderOfQi = this.OnFillChangeOfDisorderOfQi;
			if (onFillChangeOfDisorderOfQi != null)
			{
				onFillChangeOfDisorderOfQi(this.Item.ChangeOfQiDisorder);
			}
		}

		// Token: 0x0600466D RID: 18029 RVA: 0x00210558 File Offset: 0x0020E758
		public override void ResetToEmpty()
		{
			bool flag = null != this._slider;
			if (flag)
			{
				this._slider.value = 0f;
			}
			bool flag2 = null != this._stateLabel;
			if (flag2)
			{
				this._stateLabel.text = string.Empty;
				this._stateLabel.color = Color.white;
			}
			bool flag3 = null != this._stateIcon;
			if (flag3)
			{
				this._stateIcon.SetSprite(CharacterDisorderOfQi.DisorderOfQiLevelIcon[0], false, null);
			}
			bool flag4 = null != this._mouseTip;
			if (flag4)
			{
				this._mouseTip.enabled = false;
			}
			this.OnFillDisorderOfQi(0f);
		}

		// Token: 0x0600466E RID: 18030 RVA: 0x00210610 File Offset: 0x0020E810
		public static string GetDisorderOfQiLevelIcon(sbyte level)
		{
			return CharacterDisorderOfQi.DisorderOfQiLevelIcon[(level < 2) ? 0 : 1];
		}

		// Token: 0x0600466F RID: 18031 RVA: 0x00210620 File Offset: 0x0020E820
		// Note: this type is marked as 'beforefieldinit'.
		static CharacterDisorderOfQi()
		{
			LanguageKey[] array = new LanguageKey[5];
			RuntimeHelpers.InitializeArray(array, fieldof(<PrivateImplementationDetails>.E23492D894508AE955CA047489440C5B44AB511667C0ACFD8B9553E5949F84DA).FieldHandle);
			CharacterDisorderOfQi.DisorderOfQiLevelLangKeys = array;
			CharacterDisorderOfQi.DisorderOfQiLevelIcon = new string[]
			{
				"sp_combat_icon_qi_1",
				"sp_combat_icon_qi_2"
			};
		}

		// Token: 0x040030C8 RID: 12488
		public static readonly Color[] DisorderOfQiLevelColors = new Color[]
		{
			Colors.Instance["BehaviorType_Just"].ColorToHexString("#").HexStringToColor(),
			Colors.Instance["BehaviorType_Even"].ColorToHexString("#").HexStringToColor(),
			Colors.Instance["BehaviorType_Even"].ColorToHexString("#").HexStringToColor(),
			Colors.Instance["BehaviorType_Even"].ColorToHexString("#").HexStringToColor(),
			Colors.Instance["brightred"].ColorToHexString("#").HexStringToColor()
		};

		// Token: 0x040030C9 RID: 12489
		public static readonly LanguageKey[] DisorderOfQiLevelLangKeys;

		// Token: 0x040030CA RID: 12490
		public static readonly string[] DisorderOfQiLevelIcon;

		// Token: 0x040030CB RID: 12491
		private readonly CSliderLegacy _slider;

		// Token: 0x040030CC RID: 12492
		private readonly TextMeshProUGUI _stateLabel;

		// Token: 0x040030CD RID: 12493
		private readonly CImage _stateIcon;

		// Token: 0x040030CE RID: 12494
		private readonly TooltipInvoker _mouseTip;

		// Token: 0x040030CF RID: 12495
		private readonly QiContainer _qiContainer;

		// Token: 0x040030D0 RID: 12496
		public Action<float> OnFillDisorderOfQi;

		// Token: 0x040030D1 RID: 12497
		public Action<short> OnFillChangeOfDisorderOfQi;
	}
}
