using System;
using System.Collections.Generic;
using FrameWork;
using FrameWork.UISystem.UIElements;
using Game.Components.Avatar;
using Game.Components.SortAndFilter.Heal;
using Game.Views.Migrate;
using GameData.Domains.Character;
using GameData.Domains.Character.Display;
using UnityEngine;
using UnityEngine.UI;

// Token: 0x02000386 RID: 902
public class HealChar : MonoBehaviour, ILanguage
{
	// Token: 0x06003595 RID: 13717 RVA: 0x001AEDC4 File Offset: 0x001ACFC4
	private static string GetDisplayStringForHeal(int num, int threshold = 10000)
	{
		bool flag = Math.Abs(num) < threshold;
		string result;
		if (flag)
		{
			result = num.ToString();
		}
		else
		{
			result = string.Format("{0:F1}{1}", (float)num / 10000f, LocalStringManager.Get(LanguageKey.LK_Num_Ten_Thousand));
		}
		return result;
	}

	// Token: 0x06003596 RID: 13718 RVA: 0x001AEE10 File Offset: 0x001AD010
	public void Init()
	{
		bool isInit = this._isInit;
		if (!isInit)
		{
			this.pointerTrigger.EnterEvent.RemoveAllListeners();
			this.pointerTrigger.EnterEvent.AddListener(delegate()
			{
				bool flag = !this._isSelected;
				if (flag)
				{
					this.hover.SetActive(true);
				}
			});
			this.pointerTrigger.ExitEvent.RemoveAllListeners();
			this.pointerTrigger.ExitEvent.AddListener(delegate()
			{
				bool flag = !this._isSelected;
				if (flag)
				{
					this.hover.SetActive(false);
				}
			});
			this.SetIsSelected(false);
			this._isInit = true;
		}
	}

	// Token: 0x06003597 RID: 13719 RVA: 0x001AEE98 File Offset: 0x001AD098
	public void RefreshDoctorForNormal(CharacterDisplayData charData, LifeSkillShorts lifeSkillShorts, CombatResources healCount, HealPatientSortData patientData)
	{
		this._charData = charData;
		this.avatar.Refresh(this._charData.AvatarRelatedData, this._charData.TemplateId);
		this.RefreshName();
		this.RefreshDoctorAttainment(0, 8, lifeSkillShorts, true);
		this.RefreshDoctorAttainment(1, 9, lifeSkillShorts, true);
		this.RefreshDoctorAttainment(2, -1, lifeSkillShorts, false);
		this.RefreshDoctorAttainment(3, -1, lifeSkillShorts, false);
		this.RefreshHealCountNormal(4, UI_Heal.EHealType.Injury, healCount, patientData, true);
		this.RefreshHealCountNormal(5, UI_Heal.EHealType.Poison, healCount, patientData, true);
		this.RefreshHealCountNormal(6, UI_Heal.EHealType.QiDisorder, healCount, patientData, true);
		this.RefreshHealCountNormal(7, UI_Heal.EHealType.Health, healCount, patientData, true);
	}

	// Token: 0x06003598 RID: 13720 RVA: 0x001AEF38 File Offset: 0x001AD138
	public void RefreshDoctorForGear(CharacterDisplayData charData, LifeSkillShorts lifeSkillShorts, GearMateRepairCount healCount, Dictionary<sbyte, int> patientData)
	{
		this._charData = charData;
		this.avatar.Refresh(this._charData.AvatarRelatedData, this._charData.TemplateId);
		this.RefreshName();
		this.RefreshDoctorAttainment(0, 6, lifeSkillShorts, true);
		this.RefreshDoctorAttainment(1, 7, lifeSkillShorts, true);
		this.RefreshDoctorAttainment(2, 11, lifeSkillShorts, true);
		this.RefreshDoctorAttainment(3, 10, lifeSkillShorts, true);
		this.RefreshDoctorAttainment(4, -1, lifeSkillShorts, false);
		this.RefreshDoctorAttainment(5, -1, lifeSkillShorts, false);
		this.RefreshDoctorAttainment(6, -1, lifeSkillShorts, false);
		this.RefreshDoctorAttainment(7, -1, lifeSkillShorts, false);
	}

	// Token: 0x06003599 RID: 13721 RVA: 0x001AEFD0 File Offset: 0x001AD1D0
	private void RefreshDoctorAttainment(int index, sbyte lifeSkillType, LifeSkillShorts lifeSkillShorts, bool show)
	{
		List<Transform> CommonParameterHorizontalTipsSimple = new List<Transform>();
		for (int i = 0; i < this.parameterGrid.childCount; i++)
		{
			bool flag = this.parameterGrid.GetChild(i).GetComponent<HorizontalLayoutGroup>();
			if (flag)
			{
				for (int j = 0; j < this.parameterGrid.GetChild(i).childCount; j++)
				{
					CommonParameterHorizontalTipsSimple.Add(this.parameterGrid.GetChild(i).GetChild(j));
				}
			}
		}
		HealAttributeItem ParameterGridChild = CommonParameterHorizontalTipsSimple[index].GetComponent<HealAttributeItem>();
		ParameterGridChild.gameObject.SetActive(show);
		bool flag2 = !show;
		if (!flag2)
		{
			ParameterGridChild.value.text = HealChar.GetDisplayStringForHeal((int)lifeSkillShorts.Get((int)lifeSkillType), 10000);
			ParameterGridChild.icon.SetSprite("sp_14_iconjiyizhanshi_" + lifeSkillType.ToString(), false, null);
		}
	}

	// Token: 0x0600359A RID: 13722 RVA: 0x001AF0C8 File Offset: 0x001AD2C8
	private void RefreshHealCountNormal(int index, UI_Heal.EHealType healType, CombatResources healCount, HealPatientSortData patientData, bool show)
	{
		List<Transform> CommonParameterHorizontalTipsSimple = new List<Transform>();
		for (int i = 0; i < this.parameterGrid.childCount; i++)
		{
			bool flag = this.parameterGrid.GetChild(i).GetComponent<HorizontalLayoutGroup>();
			if (flag)
			{
				for (int j = 0; j < this.parameterGrid.GetChild(i).childCount; j++)
				{
					CommonParameterHorizontalTipsSimple.Add(this.parameterGrid.GetChild(i).GetChild(j));
				}
			}
		}
		HealAttributeItem ParameterGridChild = CommonParameterHorizontalTipsSimple[index].GetComponent<HealAttributeItem>();
		ParameterGridChild.gameObject.SetActive(show);
		bool flag2 = !show;
		if (!flag2)
		{
			if (!true)
			{
			}
			sbyte b;
			switch (healType)
			{
			case UI_Heal.EHealType.Injury:
				b = healCount.HealingCount;
				goto IL_117;
			case UI_Heal.EHealType.Poison:
				b = healCount.DetoxCount;
				goto IL_117;
			case UI_Heal.EHealType.QiDisorder:
				b = healCount.BreathingCount;
				goto IL_117;
			case UI_Heal.EHealType.Health:
				b = healCount.RecoverCount;
				goto IL_117;
			}
			throw new ArgumentOutOfRangeException("healType", healType, null);
			IL_117:
			if (!true)
			{
			}
			sbyte count = b;
			ParameterGridChild.value.text = HealChar.GetDisplayStringForHeal((int)count, 10000);
			if (!true)
			{
			}
			int num;
			switch (healType)
			{
			case UI_Heal.EHealType.Injury:
				num = patientData.TotalInjuries;
				goto IL_199;
			case UI_Heal.EHealType.Poison:
				num = patientData.TotalPoisons;
				goto IL_199;
			case UI_Heal.EHealType.QiDisorder:
				num = patientData.QiDisorder;
				goto IL_199;
			case UI_Heal.EHealType.Health:
				num = patientData.HealthPercent;
				goto IL_199;
			}
			throw new ArgumentOutOfRangeException("healType", healType, null);
			IL_199:
			if (!true)
			{
			}
			int value = num;
			if (!true)
			{
			}
			string text;
			switch (healType)
			{
			case UI_Heal.EHealType.Injury:
				text = UI_Heal.IconHelper.GetInjuryIcon(value);
				goto IL_20A;
			case UI_Heal.EHealType.Poison:
				text = UI_Heal.IconHelper.GetPoisonIcon(value);
				goto IL_20A;
			case UI_Heal.EHealType.QiDisorder:
				text = UI_Heal.IconHelper.GetQiDisorderIcon(UI_Heal.IsQiDisorderMax(value));
				goto IL_20A;
			case UI_Heal.EHealType.Health:
				text = UI_Heal.IconHelper.GetHealthIcon(UI_Heal.IsDying(value));
				goto IL_20A;
			}
			throw new ArgumentOutOfRangeException("healType", healType, null);
			IL_20A:
			if (!true)
			{
			}
			string icon = text;
			ParameterGridChild.icon.SetSprite(icon, healType == UI_Heal.EHealType.Health, null);
		}
	}

	// Token: 0x0600359B RID: 13723 RVA: 0x001AF2FC File Offset: 0x001AD4FC
	private void RefreshHealCountGear(int index, UI_Heal.EHealType healType, GearMateRepairCount repairCount, Dictionary<sbyte, int> patientData, bool show)
	{
		List<Transform> CommonParameterHorizontalTipsSimple = new List<Transform>();
		for (int i = 0; i < this.parameterGrid.childCount; i++)
		{
			bool flag = this.parameterGrid.GetChild(i).GetComponent<HorizontalLayoutGroup>();
			if (flag)
			{
				for (int j = 0; j < this.parameterGrid.GetChild(i).childCount; j++)
				{
					CommonParameterHorizontalTipsSimple.Add(this.parameterGrid.GetChild(i).GetChild(j));
				}
			}
		}
		HealAttributeItem ParameterGridChild = CommonParameterHorizontalTipsSimple[index].GetComponent<HealAttributeItem>();
		ParameterGridChild.gameObject.SetActive(show);
		bool flag2 = !show;
		if (!flag2)
		{
			if (!true)
			{
			}
			sbyte b;
			switch (healType)
			{
			case UI_Heal.EHealType.OuterInjury:
				b = repairCount.OuterInjuryHealingCount;
				break;
			case UI_Heal.EHealType.InnerInjury:
				b = repairCount.InnerInjuryHealingCount;
				break;
			case UI_Heal.EHealType.Poison:
				b = repairCount.DetoxCount;
				break;
			case UI_Heal.EHealType.QiDisorder:
				b = repairCount.BreathingCount;
				break;
			default:
				throw new ArgumentOutOfRangeException("healType", healType, null);
			}
			if (!true)
			{
			}
			sbyte count = b;
			ParameterGridChild.value.text = HealChar.GetDisplayStringForHeal((int)count, 10000);
			if (!true)
			{
			}
			int num;
			switch (healType)
			{
			case UI_Heal.EHealType.OuterInjury:
				num = patientData[0];
				break;
			case UI_Heal.EHealType.InnerInjury:
				num = patientData[1];
				break;
			case UI_Heal.EHealType.Poison:
				num = patientData[2];
				break;
			case UI_Heal.EHealType.QiDisorder:
				num = patientData[3];
				break;
			default:
				throw new ArgumentOutOfRangeException("healType", healType, null);
			}
			if (!true)
			{
			}
			int value = num;
			if (!true)
			{
			}
			string text;
			switch (healType)
			{
			case UI_Heal.EHealType.OuterInjury:
				text = UI_Heal.IconHelper.GetOutInjuryIcon(value);
				break;
			case UI_Heal.EHealType.InnerInjury:
				text = UI_Heal.IconHelper.GetInnerInjuryIcon(value);
				break;
			case UI_Heal.EHealType.Poison:
				text = UI_Heal.IconHelper.GetPoisonIcon(value);
				break;
			case UI_Heal.EHealType.QiDisorder:
				text = UI_Heal.IconHelper.GetQiDisorderIcon(UI_Heal.IsQiDisorderMax(value));
				break;
			default:
				throw new ArgumentOutOfRangeException("healType", healType, null);
			}
			if (!true)
			{
			}
			string icon = text;
			ParameterGridChild.icon.SetSprite(icon, false, null);
		}
	}

	// Token: 0x0600359C RID: 13724 RVA: 0x001AF514 File Offset: 0x001AD714
	public unsafe void RefreshPatientForNormal(CharacterDisplayData charData, Injuries injuries, PoisonInts poisons, short qiDisorder, int healthPercent, Dictionary<EHealActionType, int> needAttainments, LifeSkillShorts ownAttainments, bool useDefaultColor = false)
	{
		this._charData = charData;
		this._charId = charData.CharacterId;
		this.avatar.Refresh(this._charData.AvatarRelatedData, this._charData.TemplateId);
		this.RefreshName();
		this.RefreshInjuryTip();
		int injuryValue = injuries.GetSum();
		this.RefreshPatientNeedHeal(0, HealChar.GetDisplayStringForHeal(injuryValue, 10000), UI_Heal.IconHelper.GetInjuryIcon(injuryValue), UI_Heal.EHealType.Injury);
		int poisonValue = poisons.Sum();
		this.RefreshPatientNeedHeal(1, HealChar.GetDisplayStringForHeal(poisonValue, 10000), UI_Heal.IconHelper.GetPoisonIcon(poisonValue), UI_Heal.EHealType.Poison);
		int qiDisorderValue = (int)(qiDisorder / 10);
		bool isQiMax = UI_Heal.IsQiDisorderMax(qiDisorderValue);
		this.RefreshPatientNeedHeal(2, HealChar.GetDisplayStringForHeal(qiDisorderValue, 10000), UI_Heal.IconHelper.GetQiDisorderIcon(isQiMax), UI_Heal.EHealType.QiDisorder);
		bool isDying = UI_Heal.IsDying(healthPercent);
		this.RefreshPatientNeedHeal(3, string.Format("{0}%", healthPercent), UI_Heal.IconHelper.GetHealthIcon(isDying), UI_Heal.EHealType.Health);
		sbyte skillType = UI_Heal.GetMaxAttainmentForNormal(ownAttainments);
		short own = *ownAttainments[(int)skillType];
		this.RefreshPatientNeedAttainment(0, 8, needAttainments[EHealActionType.Healing], (int)(*ownAttainments[8]), useDefaultColor);
		this.RefreshPatientNeedAttainment(1, 9, needAttainments[EHealActionType.Detox], (int)(*ownAttainments[9]), useDefaultColor);
		this.RefreshPatientNeedAttainment(2, skillType, needAttainments[EHealActionType.Breathing], (int)own, useDefaultColor);
		this.RefreshPatientNeedAttainment(3, skillType, needAttainments[EHealActionType.Recover], (int)own, useDefaultColor);
	}

	// Token: 0x0600359D RID: 13725 RVA: 0x001AF678 File Offset: 0x001AD878
	public unsafe void RefreshPatientForGear(CharacterDisplayData charData, Injuries injuries, PoisonInts poisons, short qiDisorder, LifeSkillShorts needAttainments, LifeSkillShorts ownAttainments)
	{
		this._charData = charData;
		this._charId = charData.CharacterId;
		this.avatar.Refresh(this._charData.AvatarRelatedData, this._charData.TemplateId);
		this.RefreshName();
		this.RefreshInjuryTip();
		ValueTuple<sbyte, sbyte> injuriesSum = injuries.GetBothSum();
		this.RefreshPatientNeedHeal(0, HealChar.GetDisplayStringForHeal((int)injuriesSum.Item1, 10000), UI_Heal.IconHelper.GetOutInjuryIcon((int)injuriesSum.Item1), UI_Heal.EHealType.OuterInjury);
		this.RefreshPatientNeedHeal(1, HealChar.GetDisplayStringForHeal((int)injuriesSum.Item2, 10000), UI_Heal.IconHelper.GetInnerInjuryIcon((int)injuriesSum.Item2), UI_Heal.EHealType.InnerInjury);
		int poisonValue = poisons.Sum();
		this.RefreshPatientNeedHeal(2, HealChar.GetDisplayStringForHeal(poisonValue, 10000), UI_Heal.IconHelper.GetPoisonIcon(poisonValue), UI_Heal.EHealType.Poison);
		int qiDisorderValue = (int)(qiDisorder / 10);
		bool isQiMax = UI_Heal.IsQiDisorderMax(qiDisorderValue);
		this.RefreshPatientNeedHeal(3, HealChar.GetDisplayStringForHeal(qiDisorderValue, 10000), UI_Heal.IconHelper.GetQiDisorderIcon(isQiMax), UI_Heal.EHealType.QiDisorder);
		this.RefreshPatientNeedAttainment(0, 6, (int)(*needAttainments[6]), (int)(*ownAttainments[6]), false);
		this.RefreshPatientNeedAttainment(1, 7, (int)(*needAttainments[7]), (int)(*ownAttainments[7]), false);
		this.RefreshPatientNeedAttainment(2, 11, (int)(*needAttainments[11]), (int)(*ownAttainments[11]), false);
		this.RefreshPatientNeedAttainment(3, 10, (int)(*needAttainments[10]), (int)(*ownAttainments[10]), false);
	}

	// Token: 0x0600359E RID: 13726 RVA: 0x001AF7D8 File Offset: 0x001AD9D8
	private void RefreshPatientNeedHeal(int index, string value, string icon, UI_Heal.EHealType healType)
	{
		int realIndex = index * 2;
		List<Transform> CommonParameterHorizontalTipsSimple = new List<Transform>();
		for (int i = 0; i < this.parameterGrid.childCount; i++)
		{
			bool flag = this.parameterGrid.GetChild(i).GetComponent<HorizontalLayoutGroup>();
			if (flag)
			{
				for (int j = 0; j < this.parameterGrid.GetChild(i).childCount; j++)
				{
					CommonParameterHorizontalTipsSimple.Add(this.parameterGrid.GetChild(i).GetChild(j));
				}
			}
		}
		HealAttributeItem ParameterGridChild = CommonParameterHorizontalTipsSimple[realIndex].GetComponent<HealAttributeItem>();
		ParameterGridChild.value.text = value;
		ParameterGridChild.icon.SetSprite(icon, healType == UI_Heal.EHealType.Health, null);
	}

	// Token: 0x0600359F RID: 13727 RVA: 0x001AF89C File Offset: 0x001ADA9C
	private void RefreshPatientNeedAttainment(int index, sbyte type, int need, int own, bool useDefaultColor = false)
	{
		int realIndex = index * 2 + 1;
		List<Transform> CommonParameterHorizontalTipsSimple = new List<Transform>();
		for (int i = 0; i < this.parameterGrid.childCount; i++)
		{
			bool flag = this.parameterGrid.GetChild(i).GetComponent<HorizontalLayoutGroup>();
			if (flag)
			{
				for (int j = 0; j < this.parameterGrid.GetChild(i).childCount; j++)
				{
					CommonParameterHorizontalTipsSimple.Add(this.parameterGrid.GetChild(i).GetChild(j));
				}
			}
		}
		HealAttributeItem ParameterGridChild = CommonParameterHorizontalTipsSimple[realIndex].GetComponent<HealAttributeItem>();
		string displayValue = (need == int.MaxValue) ? "-" : HealChar.GetDisplayStringForHeal(need, 10000);
		string content = (need == 0 || useDefaultColor) ? displayValue.SetColor("brightyellow") : displayValue.SetColor((own >= need) ? "brightblue" : "brightred");
		ParameterGridChild.value.text = content;
		ParameterGridChild.icon.SetSprite("sp_14_iconjiyizhanshi_" + type.ToString(), false, null);
	}

	// Token: 0x060035A0 RID: 13728 RVA: 0x001AF9C3 File Offset: 0x001ADBC3
	public void SetOnClick(Action onClick)
	{
		this.button.ClearAndAddListener(onClick);
	}

	// Token: 0x060035A1 RID: 13729 RVA: 0x001AF9D4 File Offset: 0x001ADBD4
	public void SetIsSelected(bool isSelected)
	{
		this._isSelected = isSelected;
		this.SetSelected(isSelected);
		if (isSelected)
		{
			this.hover.SetActive(false);
		}
	}

	// Token: 0x060035A2 RID: 13730 RVA: 0x001AFA03 File Offset: 0x001ADC03
	private void SetSelected(bool enable)
	{
		this.selectedBg.SetActive(enable);
		this.selectedBack.SetActive(enable);
		this.selectedFrame.SetActive(enable);
	}

	// Token: 0x060035A3 RID: 13731 RVA: 0x001AFA30 File Offset: 0x001ADC30
	private void RefreshName()
	{
		int taiwuCharId = SingletonObject.getInstance<BasicGameData>().TaiwuCharId;
		bool isTaiwu = this._charData.CharacterId == taiwuCharId;
		string charName = NameCenter.GetMonasticTitleOrDisplayName(this._charData, isTaiwu);
		this.nameFrame.SetName(charName);
	}

	// Token: 0x060035A4 RID: 13732 RVA: 0x001AFA72 File Offset: 0x001ADC72
	private void RefreshInjuryTip()
	{
		this.mouseTip.Type = TipType.Injury;
		this.mouseTip.RuntimeParam = EasyPool.Get<ArgumentBox>().Set("characterId", this._charId);
	}

	// Token: 0x060035A5 RID: 13733 RVA: 0x001AFAA3 File Offset: 0x001ADCA3
	public void OnLanguageChange(LocalStringManager.LanguageType languageType)
	{
		this.nameFrame.OnLanguageChange(languageType);
		this.RefreshName();
	}

	// Token: 0x040026E6 RID: 9958
	[SerializeField]
	private Game.Components.Avatar.Avatar avatar;

	// Token: 0x040026E7 RID: 9959
	[SerializeField]
	private CommonCharacterNameFrame nameFrame;

	// Token: 0x040026E8 RID: 9960
	[SerializeField]
	private Transform parameterGrid;

	// Token: 0x040026E9 RID: 9961
	[SerializeField]
	private CButton button;

	// Token: 0x040026EA RID: 9962
	[SerializeField]
	private GameObject selectedBack;

	// Token: 0x040026EB RID: 9963
	[SerializeField]
	private GameObject selectedBg;

	// Token: 0x040026EC RID: 9964
	[SerializeField]
	private GameObject selectedFrame;

	// Token: 0x040026ED RID: 9965
	[SerializeField]
	private GameObject hover;

	// Token: 0x040026EE RID: 9966
	[SerializeField]
	private PointerTrigger pointerTrigger;

	// Token: 0x040026EF RID: 9967
	[SerializeField]
	private TooltipInvoker mouseTip;

	// Token: 0x040026F0 RID: 9968
	private CharacterDisplayData _charData;

	// Token: 0x040026F1 RID: 9969
	private bool _isInit;

	// Token: 0x040026F2 RID: 9970
	private bool _isSelected;

	// Token: 0x040026F3 RID: 9971
	private int _charId;

	// Token: 0x040026F4 RID: 9972
	private const int TenThousand = 10000;
}
