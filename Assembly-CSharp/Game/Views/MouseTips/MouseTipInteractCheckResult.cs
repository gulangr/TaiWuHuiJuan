using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Config;
using FrameWork;
using Game.Views.InteractCheckResult;
using GameData.Domains.TaiwuEvent.DisplayEvent;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Game.Views.MouseTips
{
	// Token: 0x02000867 RID: 2151
	public class MouseTipInteractCheckResult : MouseTipBase
	{
		// Token: 0x17000C6E RID: 3182
		// (get) Token: 0x060067C8 RID: 26568 RVA: 0x002F64E8 File Offset: 0x002F46E8
		protected override bool CanStick
		{
			get
			{
				return true;
			}
		}

		// Token: 0x17000C6F RID: 3183
		// (get) Token: 0x060067C9 RID: 26569 RVA: 0x002F64EB File Offset: 0x002F46EB
		private InteractCheckItem MainConfig
		{
			get
			{
				return InteractCheck.Instance[this._interactData.InteractCheckTemplateId];
			}
		}

		// Token: 0x17000C70 RID: 3184
		// (get) Token: 0x060067CA RID: 26570 RVA: 0x002F6502 File Offset: 0x002F4702
		private InteractCheckTipItem PhaseConfig
		{
			get
			{
				return InteractCheckTip.Instance[this._interactData.IsEscape ? this.MainConfig.EscapePhaseList[this._phaseIndex] : this.MainConfig.ActionPhaseList[this._phaseIndex]];
			}
		}

		// Token: 0x17000C71 RID: 3185
		// (get) Token: 0x060067CB RID: 26571 RVA: 0x002F6541 File Offset: 0x002F4741
		public ViewInteractCheckResult.PhaseState State
		{
			get
			{
				return ViewInteractCheckResult.CalcPhaseState(this._interactData, this._phaseIndex);
			}
		}

		// Token: 0x060067CC RID: 26572 RVA: 0x002F6554 File Offset: 0x002F4754
		protected override void Init(ArgumentBox argsBox)
		{
			this.Refresh(argsBox);
		}

		// Token: 0x060067CD RID: 26573 RVA: 0x002F6560 File Offset: 0x002F4760
		public override void Refresh(ArgumentBox argsBox)
		{
			this.InitRefers();
			this.ReadArgs(argsBox);
			this.title.text = this.PhaseConfig.PhaseName;
			this.desc.text = this.PhaseConfig.PhaseDesc;
			bool isEmpty = this.State == ViewInteractCheckResult.PhaseState.NoNeedCheck;
			this.RefreshAsEmpty(isEmpty);
			bool flag = !isEmpty;
			if (flag)
			{
				this.RefreshCheckContent();
				this.RefreshTable();
				this.RefreshTextLine();
				this.RefreshResult();
			}
		}

		// Token: 0x060067CE RID: 26574 RVA: 0x002F65E3 File Offset: 0x002F47E3
		private void ReadArgs(ArgumentBox argsBox)
		{
			argsBox.Get<EventInteractCheckData>("InteractData", out this._interactData);
			argsBox.Get("PhaseIndex", out this._phaseIndex);
		}

		// Token: 0x060067CF RID: 26575 RVA: 0x002F660C File Offset: 0x002F480C
		private void RefreshCheckContent()
		{
			string desc = this.PhaseConfig.CheckDesc;
			string[] lines = desc.Split('\n', StringSplitOptions.None);
			CommonUtils.PrepareEnoughChildren(this.checkContentLayout.transform, this.checkContentTemplate.gameObject, lines.Length, null);
			for (int i = 0; i < lines.Length; i++)
			{
				string line = lines[i];
				Transform item = this.checkContentLayout.transform.GetChild(i);
				Refers refers = item.GetComponent<Refers>();
				TextMeshProUGUI label = refers.CGet<TextMeshProUGUI>("Label");
				label.text = line.ColorReplace();
				label.GetComponent<TMPTextSpriteHelper>().Parse();
			}
		}

		// Token: 0x060067D0 RID: 26576 RVA: 0x002F66BC File Offset: 0x002F48BC
		private bool NeedShowTable()
		{
			InteractCheckTipItem config = this.PhaseConfig;
			return config.SelfCheckCharacterProperty >= 0 || config.SelfCheckAttainmentCombatSkillType >= 0 || config.SelfCheckAttainmentLifeSkillType >= 0 || config.SpecialValueDisplayType != EInteractCheckTipSpecialValueDisplayType.Invalid;
		}

		// Token: 0x060067D1 RID: 26577 RVA: 0x002F6700 File Offset: 0x002F4900
		private void RefreshTable()
		{
			bool needShowTable = this.NeedShowTable();
			this.checkValueTitle.gameObject.SetActive(needShowTable);
			this.checkValueTable.gameObject.SetActive(needShowTable);
			bool flag = !needShowTable;
			if (!flag)
			{
				this.RefreshTableInner();
			}
		}

		// Token: 0x060067D2 RID: 26578 RVA: 0x002F674C File Offset: 0x002F494C
		private void RefreshTableInner()
		{
			MouseTipInteractCheckResult.<>c__DisplayClass16_0 CS$<>8__locals1;
			CS$<>8__locals1.<>4__this = this;
			Refers refers = this.checkValueTable;
			CS$<>8__locals1.selfValueIcon = refers.CGet<CImage>("SelfValueIcon");
			CS$<>8__locals1.targetValueIcon = refers.CGet<CImage>("TargetValueIcon");
			TextMeshProUGUI selfName = refers.CGet<TextMeshProUGUI>("SelfName");
			CS$<>8__locals1.selfValueLabel = refers.CGet<TextMeshProUGUI>("SelfValueLabel");
			CS$<>8__locals1.selfValueTxt = refers.CGet<TextMeshProUGUI>("SelfValueTxt");
			TextMeshProUGUI targetName = refers.CGet<TextMeshProUGUI>("TargetName");
			CS$<>8__locals1.targetValueLabel = refers.CGet<TextMeshProUGUI>("TargetValueLabel");
			CS$<>8__locals1.targetValueTxt = refers.CGet<TextMeshProUGUI>("TargetValueTxt");
			GameObject selfValueObject = refers.CGet<GameObject>("SelfValue");
			CS$<>8__locals1.targetValueObject = refers.CGet<GameObject>("TargetValue");
			GameObject selfNameBg = refers.CGet<GameObject>("SelfNameBg");
			CS$<>8__locals1.targetNameBg = refers.CGet<GameObject>("TargetNameBg");
			int taiwuCharId = SingletonObject.getInstance<BasicGameData>().TaiwuCharId;
			selfName.text = NameCenter.GetMonasticTitleOrDisplayName(ref this._interactData.SelfNameRelatedData, this._interactData.SelfCharacterId == taiwuCharId, false);
			targetName.text = NameCenter.GetMonasticTitleOrDisplayName(ref this._interactData.TargetNameRelatedData, this._interactData.TargetCharacterId == taiwuCharId, false);
			CS$<>8__locals1.colon = LocalStringManager.Get(LanguageKey.LK_Colon_Symbol);
			sbyte taiwuGender = SingletonObject.getInstance<WorldMapModel>().TaiwuGender;
			CS$<>8__locals1.config = this.PhaseConfig;
			this.<RefreshTableInner>g__RefreshTableByCharacterProperty|16_2(ref CS$<>8__locals1);
			this.<RefreshTableInner>g__RefreshTableByCombatSkill|16_3(ref CS$<>8__locals1);
			this.<RefreshTableInner>g__RefreshTableByLifeSkill|16_4(ref CS$<>8__locals1);
			this.<RefreshTableInner>g__RefreshTableByLifeSkillQualification|16_5(ref CS$<>8__locals1);
			this.<RefreshTableInner>g__RefreshTableByCombatSkillQualification|16_6(ref CS$<>8__locals1);
		}

		// Token: 0x060067D3 RID: 26579 RVA: 0x002F68E4 File Offset: 0x002F4AE4
		private bool NeedShowTextLine()
		{
			InteractCheckTipItem config = this.PhaseConfig;
			string[] factorDesc = config.FactorDesc;
			return factorDesc != null && factorDesc.Length != 0;
		}

		// Token: 0x060067D4 RID: 26580 RVA: 0x002F6910 File Offset: 0x002F4B10
		private void RefreshTextLine()
		{
			bool needShow = this.NeedShowTextLine();
			GameObject gameObject = this.checkTextLineTitle.gameObject;
			bool active;
			if (needShow)
			{
				EInteractCheckTipSpecialLineDisplayType specialLineDisplayType = this.PhaseConfig.SpecialLineDisplayType;
				active = (specialLineDisplayType == EInteractCheckTipSpecialLineDisplayType.LovePure || specialLineDisplayType == EInteractCheckTipSpecialLineDisplayType.LoveSecular);
			}
			else
			{
				active = false;
			}
			gameObject.SetActive(active);
			this.checkTextLineLayout.gameObject.SetActive(needShow);
			bool flag = !needShow;
			if (!flag)
			{
				this.RefreshTextLineInner();
			}
		}

		// Token: 0x060067D5 RID: 26581 RVA: 0x002F697C File Offset: 0x002F4B7C
		private void RefreshResult()
		{
			int resultValue = 0;
			bool flag = !this._interactData.IsEscape && this._interactData.PhaseProbList.Count <= this._phaseIndex;
			if (!flag)
			{
				int num;
				if (!this._interactData.IsEscape)
				{
					num = this._interactData.PhaseProbList[this._phaseIndex];
				}
				else
				{
					List<int> phaseProbList = this._interactData.PhaseProbList;
					num = phaseProbList[phaseProbList.Count - 1];
				}
				resultValue = num;
			}
			resultValue = Math.Min(100, Math.Max(0, resultValue));
			string resultColor = (resultValue == 100) ? "brightblue" : ((resultValue == 0) ? "brightred" : "pinkyellow");
			Refers refers = this.checkResultItem;
			TextMeshProUGUI label = refers.CGet<TextMeshProUGUI>("Label");
			TextMeshProUGUI value = refers.CGet<TextMeshProUGUI>("Value");
			label.text = this.PhaseConfig.CheckResultProb;
			value.text = (resultValue.ToString() + "%").SetColor(resultColor);
		}

		// Token: 0x060067D6 RID: 26582 RVA: 0x002F6A80 File Offset: 0x002F4C80
		private void RefreshTextLineInner()
		{
			List<MouseTipInteractCheckResult.TextLine> lines = this.GenerateTextLines();
			CommonUtils.PrepareEnoughChildren(this.checkTextLineLayout.transform, this.checkTextLineTemplate.gameObject, lines.Count, null);
			for (int i = 0; i < lines.Count; i++)
			{
				MouseTipInteractCheckResult.TextLine line = lines[i];
				Transform item = this.checkTextLineLayout.transform.GetChild(i);
				Refers refers = item.GetComponent<Refers>();
				TextMeshProUGUI labelTxt = refers.CGet<TextMeshProUGUI>("Label");
				TextMeshProUGUI valueTxt = refers.CGet<TextMeshProUGUI>("Value");
				labelTxt.text = line.Label.ColorReplace();
				labelTxt.GetComponent<TMPTextSpriteHelper>().Parse();
				valueTxt.text = line.Value;
			}
			EasyPool.Free<List<MouseTipInteractCheckResult.TextLine>>(lines);
		}

		// Token: 0x060067D7 RID: 26583 RVA: 0x002F6B54 File Offset: 0x002F4D54
		private List<MouseTipInteractCheckResult.TextLine> GenerateTextLines()
		{
			List<MouseTipInteractCheckResult.TextLine> result = EasyPool.Get<List<MouseTipInteractCheckResult.TextLine>>();
			result.Clear();
			switch (this.PhaseConfig.SpecialLineDisplayType)
			{
			case EInteractCheckTipSpecialLineDisplayType.Alert:
				this.GenerateTextLineByAlert(result);
				break;
			case EInteractCheckTipSpecialLineDisplayType.LovePure:
				this.GenerateTextLinesByLove(result, this._interactData.ConfessionLovePureFixProbDict);
				break;
			case EInteractCheckTipSpecialLineDisplayType.LoveSecular:
				this.GenerateTextLinesByLove(result, this._interactData.ConfessionLoveSecularFixProbDict);
				break;
			case EInteractCheckTipSpecialLineDisplayType.AlertAndPower:
				this.GenerateTextLinesByPowerAndAlert(result);
				break;
			}
			return result;
		}

		// Token: 0x060067D8 RID: 26584 RVA: 0x002F6BDC File Offset: 0x002F4DDC
		private static MouseTipInteractCheckResult.TextLine CreateTextLine(string factorDesc, string value)
		{
			return new MouseTipInteractCheckResult.TextLine(string.Format(factorDesc, ""), value);
		}

		// Token: 0x060067D9 RID: 26585 RVA: 0x002F6C00 File Offset: 0x002F4E00
		private void GenerateTextLinesByLove(List<MouseTipInteractCheckResult.TextLine> result, Dictionary<int, int> dict)
		{
			bool flag = dict == null;
			if (flag)
			{
				this.checkTextLineTitle.gameObject.SetActive(false);
				this.checkTextLineLayout.gameObject.SetActive(false);
			}
			else
			{
				for (int i = 0; i < this.PhaseConfig.FactorDesc.Length; i++)
				{
					int value;
					bool flag2 = !dict.TryGetValue(i, out value);
					if (flag2)
					{
						break;
					}
					string valueColor = MouseTipInteractCheckResult.GetCompareColor(value.CompareTo(0));
					bool isSpecialBreakValue = value == int.MinValue;
					string valueStr = (value > 0) ? string.Format("+{0}%", value) : string.Format("{0}%", isSpecialBreakValue ? -999 : value);
					result.Add(MouseTipInteractCheckResult.CreateTextLine(this.PhaseConfig.FactorDesc[i], valueStr.SetColor(valueColor)));
					bool flag3 = i == 0 && isSpecialBreakValue;
					if (flag3)
					{
						break;
					}
				}
			}
		}

		// Token: 0x060067DA RID: 26586 RVA: 0x002F6CF8 File Offset: 0x002F4EF8
		private void GenerateTextLinesByPowerAndAlert(List<MouseTipInteractCheckResult.TextLine> result)
		{
			this.checkValueTitle.gameObject.SetActive(true);
			this.GenerateTextLineByAlert(result);
			string higherName = this._interactData.CombatPowerHigher ? NameCenter.GetMonasticTitleOrDisplayName(ref this._interactData.SelfNameRelatedData, true, false) : NameCenter.GetMonasticTitleOrDisplayName(ref this._interactData.TargetNameRelatedData, false, false);
			result.Add(MouseTipInteractCheckResult.CreateTextLine(this.PhaseConfig.FactorDesc[1], higherName));
		}

		// Token: 0x060067DB RID: 26587 RVA: 0x002F6D70 File Offset: 0x002F4F70
		private void GenerateTextLineByAlert(List<MouseTipInteractCheckResult.TextLine> result)
		{
			LanguageKey alertTextKey;
			string alertTextColor;
			if (this._interactData.TargetAlertFactor <= 220)
			{
				if (this._interactData.TargetAlertFactor <= 150)
				{
					alertTextKey = LanguageKey.LK_Low;
					alertTextColor = "brightblue";
				}
				else
				{
					alertTextKey = LanguageKey.LK_Mid;
					alertTextColor = "pinkyellow";
				}
			}
			else
			{
				alertTextKey = LanguageKey.LK_High;
				alertTextColor = "brightred";
			}
			result.Add(MouseTipInteractCheckResult.CreateTextLine(this.PhaseConfig.FactorDesc[0], LocalStringManager.Get(alertTextKey).SetColor(alertTextColor)));
		}

		// Token: 0x060067DC RID: 26588 RVA: 0x002F6DF0 File Offset: 0x002F4FF0
		private static string GetCompareColor(int compareValue)
		{
			return (compareValue > 0) ? "brightblue" : ((compareValue < 0) ? "brightred" : "pinkyellow");
		}

		// Token: 0x060067DD RID: 26589 RVA: 0x002F6E20 File Offset: 0x002F5020
		private static string ToString([TupleElementNames(new string[]
		{
			"value",
			"format"
		})] ValueTuple<int, MouseTipInteractCheckResult.ShowValueFormat> valueWithFormat)
		{
			MouseTipInteractCheckResult.ShowValueFormat item = valueWithFormat.Item2;
			MouseTipInteractCheckResult.ShowValueFormat showValueFormat = item;
			string result;
			if (showValueFormat != MouseTipInteractCheckResult.ShowValueFormat.Number)
			{
				if (showValueFormat != MouseTipInteractCheckResult.ShowValueFormat.Percent)
				{
					throw new ArgumentOutOfRangeException();
				}
				result = string.Format("{0}%", valueWithFormat.Item1);
			}
			else
			{
				result = valueWithFormat.Item1.ToString();
			}
			return result;
		}

		// Token: 0x060067DE RID: 26590 RVA: 0x002F6E70 File Offset: 0x002F5070
		[return: TupleElementNames(new string[]
		{
			"value",
			"format"
		})]
		private unsafe ValueTuple<int, MouseTipInteractCheckResult.ShowValueFormat>? GetCheckValueByCharacterProperty(short characterPropertyId, bool isSelf)
		{
			bool flag = characterPropertyId < 0;
			ValueTuple<int, MouseTipInteractCheckResult.ShowValueFormat>? result;
			if (flag)
			{
				result = null;
			}
			else
			{
				bool isMainAttribute = characterPropertyId >= 0 && characterPropertyId < 6;
				bool flag2 = isMainAttribute;
				if (flag2)
				{
					result = (isSelf ? new ValueTuple<int, MouseTipInteractCheckResult.ShowValueFormat>?(new ValueTuple<int, MouseTipInteractCheckResult.ShowValueFormat>((int)(*this._interactData.SelfMainAttributes[(int)characterPropertyId]), MouseTipInteractCheckResult.ShowValueFormat.Number)) : null);
				}
				else
				{
					bool isHitValue = characterPropertyId >= 6 && characterPropertyId < 10;
					bool flag3 = isHitValue;
					if (flag3)
					{
						result = new ValueTuple<int, MouseTipInteractCheckResult.ShowValueFormat>?(new ValueTuple<int, MouseTipInteractCheckResult.ShowValueFormat>(isSelf ? this._interactData.SelfHitValues[(int)(characterPropertyId - 6)] : this._interactData.TargetHitValues[(int)(characterPropertyId - 6)], MouseTipInteractCheckResult.ShowValueFormat.Number));
					}
					else
					{
						bool isAttackValue = characterPropertyId >= 10 && characterPropertyId < 12;
						bool flag4 = isAttackValue;
						if (flag4)
						{
							bool isOuter = characterPropertyId - 10 == 0;
							result = new ValueTuple<int, MouseTipInteractCheckResult.ShowValueFormat>?(new ValueTuple<int, MouseTipInteractCheckResult.ShowValueFormat>(isSelf ? (isOuter ? this._interactData.SelfPenetrations.Outer : this._interactData.SelfPenetrations.Inner) : (isOuter ? this._interactData.TargetPenetrations.Outer : this._interactData.TargetPenetrations.Inner), MouseTipInteractCheckResult.ShowValueFormat.Number));
						}
						else
						{
							bool isAvoidValue = characterPropertyId >= 12 && characterPropertyId < 16;
							bool flag5 = isAvoidValue;
							if (flag5)
							{
								result = new ValueTuple<int, MouseTipInteractCheckResult.ShowValueFormat>?(new ValueTuple<int, MouseTipInteractCheckResult.ShowValueFormat>(isSelf ? this._interactData.SelfAvoidValues[(int)(characterPropertyId - 12)] : this._interactData.TargetAvoidValues[(int)(characterPropertyId - 12)], MouseTipInteractCheckResult.ShowValueFormat.Number));
							}
							else
							{
								bool isDefenceValue = characterPropertyId >= 16 && characterPropertyId < 18;
								bool flag6 = isDefenceValue;
								if (flag6)
								{
									bool isOuter2 = characterPropertyId - 16 == 0;
									result = new ValueTuple<int, MouseTipInteractCheckResult.ShowValueFormat>?(new ValueTuple<int, MouseTipInteractCheckResult.ShowValueFormat>(isSelf ? (isOuter2 ? this._interactData.SelfPenetrationResists.Outer : this._interactData.SelfPenetrationResists.Inner) : (isOuter2 ? this._interactData.TargetPenetrationResists.Outer : this._interactData.TargetPenetrationResists.Inner), MouseTipInteractCheckResult.ShowValueFormat.Number));
								}
								else
								{
									bool isSubProperty = characterPropertyId >= 18 && characterPropertyId < 28;
									bool flag7 = isSubProperty;
									if (!flag7)
									{
										throw new IndexOutOfRangeException(string.Format("CharacterPropertyId {0} is out of range", characterPropertyId));
									}
									if (characterPropertyId != 20)
									{
										if (characterPropertyId != 22)
										{
											if (characterPropertyId != 25)
											{
												throw new IndexOutOfRangeException(string.Format("CharacterPropertyId {0} is out of range", characterPropertyId));
											}
											result = new ValueTuple<int, MouseTipInteractCheckResult.ShowValueFormat>?(new ValueTuple<int, MouseTipInteractCheckResult.ShowValueFormat>((int)(isSelf ? this._interactData.SelfAttackSpeed : this._interactData.TargetAttackSpeed), MouseTipInteractCheckResult.ShowValueFormat.Percent));
										}
										else
										{
											result = new ValueTuple<int, MouseTipInteractCheckResult.ShowValueFormat>?(new ValueTuple<int, MouseTipInteractCheckResult.ShowValueFormat>((int)(isSelf ? this._interactData.SelfCastSpeed : this._interactData.TargetCastSpeed), MouseTipInteractCheckResult.ShowValueFormat.Percent));
										}
									}
									else
									{
										result = new ValueTuple<int, MouseTipInteractCheckResult.ShowValueFormat>?(new ValueTuple<int, MouseTipInteractCheckResult.ShowValueFormat>((int)(isSelf ? this._interactData.SelfMoveSpeed : this._interactData.TargetMoveSpeed), MouseTipInteractCheckResult.ShowValueFormat.Percent));
									}
								}
							}
						}
					}
				}
			}
			return result;
		}

		// Token: 0x060067DF RID: 26591 RVA: 0x002F717C File Offset: 0x002F537C
		private void RefreshAsEmpty(bool isEmpty)
		{
			this.emptyLabel.gameObject.SetActive(isEmpty);
			this.checkContentTitle.gameObject.SetActive(!isEmpty);
			this.checkContentLayout.gameObject.SetActive(!isEmpty);
			this.checkValueTitle.gameObject.SetActive(!isEmpty);
			this.checkValueTable.gameObject.SetActive(!isEmpty);
			this.checkTextLineTitle.gameObject.SetActive(!isEmpty);
			this.checkTextLineLayout.gameObject.SetActive(!isEmpty);
			this.checkResultTitle.gameObject.SetActive(!isEmpty);
			this.checkResultLayout.gameObject.SetActive(!isEmpty);
		}

		// Token: 0x060067E0 RID: 26592 RVA: 0x002F7244 File Offset: 0x002F5444
		private void InitRefers()
		{
		}

		// Token: 0x060067E2 RID: 26594 RVA: 0x002F7250 File Offset: 0x002F5450
		[CompilerGenerated]
		private void <RefreshTableInner>g__UseNoTargetStyle|16_0(ref MouseTipInteractCheckResult.<>c__DisplayClass16_0 A_1)
		{
			A_1.targetNameBg.SetActive(false);
			A_1.targetValueObject.SetActive(false);
		}

		// Token: 0x060067E3 RID: 26595 RVA: 0x002F726D File Offset: 0x002F546D
		[CompilerGenerated]
		private void <RefreshTableInner>g__UseNormalStyle|16_1(ref MouseTipInteractCheckResult.<>c__DisplayClass16_0 A_1)
		{
			A_1.targetNameBg.SetActive(true);
			A_1.targetValueObject.SetActive(true);
		}

		// Token: 0x060067E4 RID: 26596 RVA: 0x002F728C File Offset: 0x002F548C
		[CompilerGenerated]
		private void <RefreshTableInner>g__RefreshTableByCharacterProperty|16_2(ref MouseTipInteractCheckResult.<>c__DisplayClass16_0 A_1)
		{
			bool flag = A_1.config.SelfCheckCharacterProperty < 0;
			if (flag)
			{
				this.<RefreshTableInner>g__UseNormalStyle|16_1(ref A_1);
			}
			else
			{
				short selfDisplayId = CharacterPropertyReferenced.Instance[A_1.config.SelfCheckCharacterProperty].DisplayType;
				CharacterPropertyDisplayItem selfDisplayData = CharacterPropertyDisplay.Instance[selfDisplayId];
				A_1.selfValueIcon.SetSpriteOnly(selfDisplayData.TipsBigIcon, false, null);
				ValueTuple<int, MouseTipInteractCheckResult.ShowValueFormat>? selfValueTuple = this.GetCheckValueByCharacterProperty(A_1.config.SelfCheckCharacterProperty, true);
				ValueTuple<int, MouseTipInteractCheckResult.ShowValueFormat>? targetValueTuple = this.GetCheckValueByCharacterProperty(A_1.config.TargetCheckCharacterProperty, false);
				bool flag2 = targetValueTuple != null;
				if (flag2)
				{
					this.<RefreshTableInner>g__UseNormalStyle|16_1(ref A_1);
				}
				else
				{
					this.<RefreshTableInner>g__UseNoTargetStyle|16_0(ref A_1);
				}
				bool flag3 = selfValueTuple != null;
				if (flag3)
				{
					int num;
					if (targetValueTuple == null)
					{
						num = 0;
					}
					else
					{
						ValueTuple<int, MouseTipInteractCheckResult.ShowValueFormat> value = selfValueTuple.Value;
						num = value.Item1.CompareTo(targetValueTuple.Value.Item1);
					}
					int compareValue = num;
					string selfColor = MouseTipInteractCheckResult.GetCompareColor(compareValue);
					A_1.selfValueLabel.text = (selfDisplayData.Name ?? "");
					A_1.selfValueTxt.text = MouseTipInteractCheckResult.ToString(selfValueTuple.Value).SetColor(selfColor);
				}
				bool flag4 = targetValueTuple != null;
				if (flag4)
				{
					short targetDisplayId = CharacterPropertyReferenced.Instance[A_1.config.TargetCheckCharacterProperty].DisplayType;
					CharacterPropertyDisplayItem targetDisplayData = CharacterPropertyDisplay.Instance[targetDisplayId];
					A_1.targetValueIcon.SetSprite(targetDisplayData.TipsIcon, false, null);
					int num2;
					if (selfValueTuple == null)
					{
						num2 = 0;
					}
					else
					{
						ValueTuple<int, MouseTipInteractCheckResult.ShowValueFormat> value = targetValueTuple.Value;
						num2 = value.Item1.CompareTo(selfValueTuple.Value.Item1);
					}
					int compareValue2 = num2;
					string targetColor = MouseTipInteractCheckResult.GetCompareColor(compareValue2);
					A_1.targetValueLabel.text = (targetDisplayData.Name ?? "");
					A_1.targetValueTxt.text = MouseTipInteractCheckResult.ToString(targetValueTuple.Value).SetColor(targetColor);
				}
			}
		}

		// Token: 0x060067E5 RID: 26597 RVA: 0x002F7484 File Offset: 0x002F5684
		[CompilerGenerated]
		private unsafe void <RefreshTableInner>g__RefreshTableByCombatSkill|16_3(ref MouseTipInteractCheckResult.<>c__DisplayClass16_0 A_1)
		{
			bool flag = A_1.config.SelfCheckAttainmentCombatSkillType < 0;
			if (!flag)
			{
				short selfValue = *this._interactData.SelfCombatSkillAttainments[(int)A_1.config.SelfCheckAttainmentCombatSkillType];
				short targetValue = *this._interactData.TargetCombatSkillAttainments[(int)A_1.config.TargetCheckAttainmentCombatSkillType];
				int compareValue = selfValue.CompareTo(targetValue);
				CombatSkillTypeItem selfCombatSkillTypeItem = CombatSkillType.Instance[A_1.config.SelfCheckAttainmentCombatSkillType];
				CombatSkillTypeItem targetCombatSkillTypeItem = CombatSkillType.Instance[A_1.config.TargetCheckAttainmentCombatSkillType];
				string selfColor = MouseTipInteractCheckResult.GetCompareColor(compareValue);
				string targetColor = MouseTipInteractCheckResult.GetCompareColor(-compareValue);
				A_1.selfValueLabel.text = (selfCombatSkillTypeItem.Name ?? "");
				A_1.selfValueTxt.text = selfValue.ToString().SetColor(selfColor);
				A_1.targetValueLabel.text = (targetCombatSkillTypeItem.Name ?? "");
				A_1.targetValueTxt.text = targetValue.ToString().SetColor(targetColor);
				A_1.selfValueIcon.SetSprite(selfCombatSkillTypeItem.DisplayIcon, false, null);
				A_1.targetValueIcon.SetSprite(targetCombatSkillTypeItem.DisplayIcon, false, null);
			}
		}

		// Token: 0x060067E6 RID: 26598 RVA: 0x002F75C0 File Offset: 0x002F57C0
		[CompilerGenerated]
		private unsafe void <RefreshTableInner>g__RefreshTableByLifeSkill|16_4(ref MouseTipInteractCheckResult.<>c__DisplayClass16_0 A_1)
		{
			bool flag = A_1.config.SelfCheckAttainmentLifeSkillType < 0;
			if (!flag)
			{
				short selfValue = *this._interactData.SelfLifeSkillAttainments[(int)A_1.config.SelfCheckAttainmentLifeSkillType];
				short targetValue = *this._interactData.TargetLifeSkillAttainments[(int)A_1.config.TargetCheckAttainmentLifeSkillType];
				int compareValue = selfValue.CompareTo(targetValue);
				LifeSkillTypeItem selfLifeSkillTypeItem = LifeSkillType.Instance[A_1.config.SelfCheckAttainmentLifeSkillType];
				LifeSkillTypeItem targetLifeSkillTypeItem = LifeSkillType.Instance[A_1.config.TargetCheckAttainmentLifeSkillType];
				string selfColor = MouseTipInteractCheckResult.GetCompareColor(compareValue);
				string targetColor = MouseTipInteractCheckResult.GetCompareColor(-compareValue);
				A_1.selfValueLabel.text = (selfLifeSkillTypeItem.Name ?? "");
				A_1.selfValueTxt.text = selfValue.ToString().SetColor(selfColor);
				A_1.targetValueLabel.text = (targetLifeSkillTypeItem.Name ?? "");
				A_1.targetValueTxt.text = targetValue.ToString().SetColor(targetColor);
				A_1.selfValueIcon.SetSprite(selfLifeSkillTypeItem.DisplayIcon, false, null);
				A_1.targetValueIcon.SetSprite(targetLifeSkillTypeItem.DisplayIcon, false, null);
			}
		}

		// Token: 0x060067E7 RID: 26599 RVA: 0x002F76FC File Offset: 0x002F58FC
		[CompilerGenerated]
		private unsafe void <RefreshTableInner>g__RefreshTableByLifeSkillQualification|16_5(ref MouseTipInteractCheckResult.<>c__DisplayClass16_0 A_1)
		{
			bool flag = A_1.config.SpecialValueDisplayType > EInteractCheckTipSpecialValueDisplayType.StealSkillLifeSkillQualities;
			if (!flag)
			{
				short selfValue = *this._interactData.SelfLifeSkillQualities[(int)this._interactData.StealLifeSkillType];
				short targetValue = SkillGradeData.Instance[this._interactData.StealSkillGrade].PracticeQualificationRequirement;
				string selfColor = MouseTipInteractCheckResult.GetCompareColor(selfValue.CompareTo(targetValue));
				LifeSkillTypeItem lifeSkillTypeItem = LifeSkillType.Instance[this._interactData.StealLifeSkillType];
				A_1.selfValueIcon.SetSprite(lifeSkillTypeItem.DisplayIcon, false, null);
				A_1.selfValueLabel.text = (lifeSkillTypeItem.Name ?? "");
				A_1.selfValueTxt.text = selfValue.ToString().SetColor(selfColor);
				string targetColor = MouseTipInteractCheckResult.GetCompareColor(targetValue.CompareTo(selfValue));
				A_1.targetValueLabel.text = (lifeSkillTypeItem.Name ?? "");
				A_1.targetValueTxt.text = targetValue.ToString().SetColor(targetColor);
				A_1.targetValueIcon.SetSprite(lifeSkillTypeItem.DisplayIcon, false, null);
			}
		}

		// Token: 0x060067E8 RID: 26600 RVA: 0x002F7820 File Offset: 0x002F5A20
		[CompilerGenerated]
		private unsafe void <RefreshTableInner>g__RefreshTableByCombatSkillQualification|16_6(ref MouseTipInteractCheckResult.<>c__DisplayClass16_0 A_1)
		{
			bool flag = A_1.config.SpecialValueDisplayType != EInteractCheckTipSpecialValueDisplayType.StealSkillCombatSkillQualities;
			if (!flag)
			{
				short selfValue = *this._interactData.SelfCombatSkillQualities[(int)this._interactData.StealCombatSkillType];
				short targetValue = SkillGradeData.Instance[this._interactData.StealSkillGrade].PracticeQualificationRequirement;
				CombatSkillTypeItem combatSkillTypeItem = CombatSkillType.Instance[this._interactData.StealCombatSkillType];
				A_1.selfValueIcon.SetSprite(combatSkillTypeItem.DisplayIcon, false, null);
				string selfColor = MouseTipInteractCheckResult.GetCompareColor(selfValue.CompareTo(targetValue));
				A_1.selfValueLabel.text = combatSkillTypeItem.Name + A_1.colon + selfValue.ToString().SetColor(selfColor);
				string targetColor = MouseTipInteractCheckResult.GetCompareColor(targetValue.CompareTo(selfValue));
				A_1.targetValueLabel.text = (combatSkillTypeItem.Name ?? "");
				A_1.targetValueTxt.text = targetValue.ToString().SetColor(targetColor);
				A_1.targetValueIcon.SetSprite(combatSkillTypeItem.DisplayIcon, false, null);
			}
		}

		// Token: 0x0400494F RID: 18767
		private EventInteractCheckData _interactData;

		// Token: 0x04004950 RID: 18768
		private int _phaseIndex;

		// Token: 0x04004951 RID: 18769
		[SerializeField]
		private Refers checkContentTemplate;

		// Token: 0x04004952 RID: 18770
		[SerializeField]
		private Refers checkContentTitle;

		// Token: 0x04004953 RID: 18771
		[SerializeField]
		private Refers checkResultItem;

		// Token: 0x04004954 RID: 18772
		[SerializeField]
		private Refers checkResultTitle;

		// Token: 0x04004955 RID: 18773
		[SerializeField]
		private Refers checkTextLineTemplate;

		// Token: 0x04004956 RID: 18774
		[SerializeField]
		private Refers checkTextLineTitle;

		// Token: 0x04004957 RID: 18775
		[SerializeField]
		private Refers checkValueTable;

		// Token: 0x04004958 RID: 18776
		[SerializeField]
		private Refers checkValueTitle;

		// Token: 0x04004959 RID: 18777
		[SerializeField]
		private TextMeshProUGUI desc;

		// Token: 0x0400495A RID: 18778
		[SerializeField]
		private TextMeshProUGUI emptyLabel;

		// Token: 0x0400495B RID: 18779
		[SerializeField]
		private TextMeshProUGUI title;

		// Token: 0x0400495C RID: 18780
		[SerializeField]
		private VerticalLayoutGroup checkContentLayout;

		// Token: 0x0400495D RID: 18781
		[SerializeField]
		private VerticalLayoutGroup checkResultLayout;

		// Token: 0x0400495E RID: 18782
		[SerializeField]
		private GridLayoutGroup checkTextLineLayout;

		// Token: 0x02001D7F RID: 7551
		private readonly struct TextLine
		{
			// Token: 0x0600ED71 RID: 60785 RVA: 0x00608B90 File Offset: 0x00606D90
			public TextLine(string label, string value)
			{
				this.Label = label;
				this.Value = value;
			}

			// Token: 0x0400C657 RID: 50775
			public readonly string Label;

			// Token: 0x0400C658 RID: 50776
			public readonly string Value;
		}

		// Token: 0x02001D80 RID: 7552
		private enum ShowValueFormat
		{
			// Token: 0x0400C65A RID: 50778
			Number,
			// Token: 0x0400C65B RID: 50779
			Percent
		}
	}
}
