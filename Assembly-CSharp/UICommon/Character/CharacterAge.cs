using System;
using CharacterDataMonitor;
using Config;
using FrameWork;
using GameData.Domains.Character.Creation;
using TMPro;

namespace UICommon.Character
{
	// Token: 0x020005C6 RID: 1478
	public class CharacterAge : CharacterUIElement
	{
		// Token: 0x170008D7 RID: 2263
		// (get) Token: 0x06004624 RID: 17956 RVA: 0x0020E4BE File Offset: 0x0020C6BE
		private AgeHealthMonitor Item
		{
			get
			{
				return this.MonitorDataItem as AgeHealthMonitor;
			}
		}

		// Token: 0x06004625 RID: 17957 RVA: 0x0020E4CC File Offset: 0x0020C6CC
		public CharacterAge(TextMeshProUGUI label, CImage icon = null, TooltipInvoker ageMouseTip = null, TooltipInvoker fiveElemsMouseTip = null, bool hideOnlyNonEvolutionaryType = false, bool onlyAge = false, TextMeshProUGUI fiveElementsTxt = null, TextMeshProUGUI birthMonthTxt = null)
		{
			this._hideOnlyNonEvolutionaryType = hideOnlyNonEvolutionaryType;
			this._ageLabel = label;
			this._onlyAge = onlyAge;
			bool flag = null != this._ageLabel;
			if (flag)
			{
				this._ageLabel.text = string.Empty;
			}
			this._fiveElementsIcon = icon;
			this._fiveElementsTxt = fiveElementsTxt;
			this._birthMonthTxt = birthMonthTxt;
			bool flag2 = null != this._fiveElementsIcon;
			if (flag2)
			{
				this._fiveElementsIcon.SetSprite("", false, null);
			}
			bool flag3 = null != ageMouseTip;
			if (flag3)
			{
				ageMouseTip.Type = TipType.Simple;
				ageMouseTip.IsLanguageKey = false;
				ageMouseTip.enabled = true;
				ageMouseTip.PresetParam = new string[]
				{
					LocalStringManager.Get(LanguageKey.LK_Char_Age),
					LocalStringManager.Get(LanguageKey.LK_Char_Age_TipContent)
				};
			}
			this._fiveElementsMouseTip = fiveElemsMouseTip;
		}

		// Token: 0x06004626 RID: 17958 RVA: 0x0020E5B2 File Offset: 0x0020C7B2
		internal override void BindEvent()
		{
			this.Item.AddOnAgeChangeEventListener(new Action(this.FillElement));
			this.Item.AddTemplateIdListener(new Action(this.FillElement));
		}

		// Token: 0x06004627 RID: 17959 RVA: 0x0020E5E7 File Offset: 0x0020C7E7
		public override void UnbindEvent()
		{
			this.Item.RemoveOnAgeChangeEventListener(new Action(this.FillElement));
			this.Item.RemoveTemplateIdListener(new Action(this.FillElement));
		}

		// Token: 0x06004628 RID: 17960 RVA: 0x0020E61C File Offset: 0x0020C81C
		public override void FillElement()
		{
			bool flag = null == this._ageLabel;
			if (flag)
			{
				base.CharacterId = -1;
			}
			else
			{
				MonthItem monthConfig = Month.Instance[this.Item.BirthMonth];
				bool flag2 = null != this._ageLabel;
				if (flag2)
				{
					CharacterItem characterConfig = Character.Instance.GetItem(this.Item.TemplateId);
					bool needHide = characterConfig.HideAge;
					bool flag3 = this._hideOnlyNonEvolutionaryType && !CreatingType.IsNonEvolutionaryType(this.Item.CreatingType);
					if (flag3)
					{
						needHide = false;
					}
					bool flag4 = !needHide;
					if (flag4)
					{
						bool flag5 = this.GetAgeText != null;
						if (flag5)
						{
							this._ageLabel.text = this.GetAgeText(this.Item.BirthMonth, this.Item.PhysiologicalAge);
							return;
						}
						this._ageLabel.text = LocalStringManager.GetFormat(LanguageKey.LK_Age, this.Item.PhysiologicalAge);
						bool flag6 = !this._onlyAge;
						if (flag6)
						{
							TextMeshProUGUI ageLabel = this._ageLabel;
							ageLabel.text += LocalStringManager.GetFormat(LanguageKey.UI_NewGame_BornDateInfo, monthConfig.Name);
						}
					}
					else
					{
						this._ageLabel.text = "-";
					}
				}
				bool flag7 = this._birthMonthTxt != null;
				if (flag7)
				{
					this._birthMonthTxt.text = LocalStringManager.GetFormat(LanguageKey.LK_Birth_Tips, monthConfig.Name);
				}
				bool flag8 = null != this._fiveElementsIcon;
				if (flag8)
				{
					this._fiveElementsIcon.SetSprite(CommonUtils.GetFiveElementsIconByType(monthConfig.FiveElementsType), false, null);
				}
				bool flag9 = null != this._fiveElementsTxt;
				if (flag9)
				{
					this._fiveElementsTxt.text = CommonUtils.GetFiveElementsNameByType(monthConfig.FiveElementsType);
				}
				bool flag10 = null != this._fiveElementsMouseTip;
				if (flag10)
				{
					this._fiveElementsMouseTip.Type = TipType.InnateFiveElements;
					this._fiveElementsMouseTip.IsLanguageKey = false;
					this._fiveElementsMouseTip.enabled = true;
					this._fiveElementsMouseTip.RuntimeParam = EasyPool.Get<ArgumentBox>().Set("BirthMonth", (int)monthConfig.TemplateId);
				}
			}
		}

		// Token: 0x06004629 RID: 17961 RVA: 0x0020E860 File Offset: 0x0020CA60
		public override void ResetToEmpty()
		{
			bool flag = null != this._ageLabel;
			if (flag)
			{
				bool flag2 = this.GetAgeText != null;
				if (flag2)
				{
					this._ageLabel.text = this.GetAgeText(-1, -1);
					return;
				}
				this._ageLabel.text = string.Empty;
			}
			bool flag3 = null != this._fiveElementsTxt;
			if (flag3)
			{
				this._fiveElementsTxt.text = string.Empty;
			}
			bool flag4 = null != this._birthMonthTxt;
			if (flag4)
			{
				this._birthMonthTxt.text = string.Empty;
			}
			bool flag5 = null != this._fiveElementsIcon;
			if (flag5)
			{
				this._fiveElementsIcon.SetSprite("", false, null);
			}
			bool flag6 = null != this._fiveElementsMouseTip;
			if (flag6)
			{
				this._fiveElementsMouseTip.enabled = false;
			}
		}

		// Token: 0x0600462A RID: 17962 RVA: 0x0020E948 File Offset: 0x0020CB48
		public override MonitorDataItemBase GetMonitorItem(int charId)
		{
			return SingletonObject.getInstance<CharacterMonitorModel>().GetMonitorItem<AgeHealthMonitor>(charId, this.IsDead);
		}

		// Token: 0x040030A5 RID: 12453
		private readonly TextMeshProUGUI _ageLabel;

		// Token: 0x040030A6 RID: 12454
		private readonly CImage _fiveElementsIcon;

		// Token: 0x040030A7 RID: 12455
		private readonly TextMeshProUGUI _fiveElementsTxt;

		// Token: 0x040030A8 RID: 12456
		private readonly TextMeshProUGUI _birthMonthTxt;

		// Token: 0x040030A9 RID: 12457
		private readonly TooltipInvoker _fiveElementsMouseTip;

		// Token: 0x040030AA RID: 12458
		private bool _hideOnlyNonEvolutionaryType = false;

		// Token: 0x040030AB RID: 12459
		private bool _onlyAge = false;

		// Token: 0x040030AC RID: 12460
		public Func<sbyte, short, string> GetAgeText;
	}
}
