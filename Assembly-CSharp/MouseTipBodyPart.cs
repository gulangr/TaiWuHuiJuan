using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Config;
using FrameWork;
using Game.Views.Encyclopedia;
using GameData.Combat.Math;
using GameData.Domains.Character;
using GameData.Domains.Combat;
using GameData.Utilities;
using TMPro;
using UnityEngine;

// Token: 0x02000273 RID: 627
public class MouseTipBodyPart : MouseTipBase
{
	// Token: 0x06002910 RID: 10512 RVA: 0x00131540 File Offset: 0x0012F740
	protected override void Init(ArgumentBox argsBox)
	{
		this.Element.ForceListenCommand = true;
		this.Clear();
		argsBox.Get("Title", out this._title);
		argsBox.Get("Type", out this._type);
		argsBox.Get<Injuries>("Injury", out this._injuries);
		argsBox.Get<CompleteDamageStepDisplayData>("CompleteDamageStepDisplayData", out this._data);
		argsBox.Get<List<bool>>("AllBodyPartExists", out this._allBodyPartExists);
		int characterId;
		argsBox.Get("CharacterId", out characterId);
		bool flag = string.IsNullOrEmpty(this._type);
		if (!flag)
		{
			GameObject injuryObj = base.CGet<GameObject>("Injury");
			GameObject monthlyHealObj = base.CGet<GameObject>("MonthlyHeal");
			CompleteDamageStepDisplayData data = this._data;
			DamageStepCollection baseSteps = (data != null) ? data.CharacterBaseDamageSteps : null;
			base.CGet<TextMeshProUGUI>("Title").text = this._title;
			ValueTuple<sbyte, sbyte> constants;
			bool flag2 = this._bodyPartConstants.TryGetValue(this._type, out constants);
			if (flag2)
			{
				this._bodyPartType = constants.Item1;
				ValueTuple<sbyte, sbyte> injury = this._injuries.Get(this._bodyPartType);
				injuryObj.GetComponent<Refers>().CGet<TextMeshProUGUI>("Text").text = LocalStringManager.GetFormat(LanguageKey.LK_Body_Part_Tips_Content_New, injury.Item1, injury.Item2).ColorReplace();
				injuryObj.SetActive(true);
				bool isGearMate = SingletonObject.getInstance<CharacterMonitorModel>().IsTaiwuGearMate(characterId);
				monthlyHealObj.SetActive(true);
				monthlyHealObj.GetComponent<Refers>().CGet<TextMeshProUGUI>("Text").text = (isGearMate ? LocalStringManager.Get(LanguageKey.LK_MouseTip_GearMateNoMonthlyHeal) : LocalStringManager.Get("LK_MouseTip_MonthlyHeal_" + this._bodyPartMonthlyHealConstants[this._type])).ColorReplace();
				bool flag3 = baseSteps != null;
				if (flag3)
				{
					bool allBodyPartExist = this._allBodyPartExists.GetOrDefault((int)this._bodyPartType, true);
					this.AddItem(LocalStringManager.Get(LanguageKey.LK_MouseTip_DamageStep_Outer), string.Format("mousetip_waishang_{0}", constants.Item2), baseSteps.OuterDamageSteps[(int)this._bodyPartType], "outterinjury", this._data.BodyPart[(int)this._bodyPartType].Outer, new Func<DamageStepDisplayData, ValueTuple<int, string>>(this.GetCombatSkillOuterDamageStep), !allBodyPartExist);
					this.AddItem(LocalStringManager.Get(LanguageKey.LK_MouseTip_DamageStep_Inner), string.Format("mousetip_neishang_{0}", constants.Item2), baseSteps.InnerDamageSteps[(int)this._bodyPartType], "innerinjury", this._data.BodyPart[(int)this._bodyPartType].Inner, new Func<DamageStepDisplayData, ValueTuple<int, string>>(this.GetCombatSkillInnerDamageStep), !allBodyPartExist);
				}
			}
			else
			{
				injuryObj.SetActive(false);
				monthlyHealObj.SetActive(false);
				bool flag4 = baseSteps != null;
				if (flag4)
				{
					this._consummateLevel = this._data.CharacterConsummateLevel;
					bool flag5 = this._type == "Mind";
					if (flag5)
					{
						this._consummateBonus = ConsummateLevel.Instance[this._data.CharacterConsummateLevel].MindDamageStepAddPercent;
						this.AddItem(LocalStringManager.Get(LanguageKey.LK_MouseTip_DamageStep_Mind), "mousetip_dongxin_0", baseSteps.MindDamageStep, "pinkyellow", this._data.Mind, new Func<DamageStepDisplayData, ValueTuple<int, string>>(this.GetCombatSkillMindDamageStep), false);
					}
					else
					{
						this._consummateBonus = ConsummateLevel.Instance[this._data.CharacterConsummateLevel].FatalDamageStepAddPercent;
						this.AddItem(LocalStringManager.Get(LanguageKey.LK_MouseTip_DamageStep_Fatal), "mousetip_zhongchuang_0", baseSteps.FatalDamageStep, "pinkyellow", this._data.Fatal, new Func<DamageStepDisplayData, ValueTuple<int, string>>(this.GetCombatSkillFatalDamageStep), false);
					}
				}
			}
			base.CGet<GameObject>("Desc").transform.SetAsLastSibling();
			bool activeSelf = monthlyHealObj.activeSelf;
			if (activeSelf)
			{
				monthlyHealObj.transform.SetAsLastSibling();
			}
			base.CGet<GameObject>("MouseTipHotKey").transform.SetAsLastSibling();
		}
	}

	// Token: 0x06002911 RID: 10513 RVA: 0x0013193C File Offset: 0x0012FB3C
	private void AddItem(string title, string icon, int baseValue, string color, DamageStepDisplayData data, Func<DamageStepDisplayData, ValueTuple<int, string>> getSkill, bool isNone = false)
	{
		GameObject line = Object.Instantiate<GameObject>(base.CGet<GameObject>("LineType_1"), base.transform);
		Refers refers = line.GetComponent<Refers>();
		int finalValue = baseValue;
		refers.CGet<TextMeshProUGUI>("Text1").text = title;
		refers.CGet<CImage>("Icon").SetSprite(icon, false, null);
		if (isNone)
		{
			refers.CGet<TextMeshProUGUI>("Text2").text = LocalStringManager.Get(LanguageKey.LK_None);
			line.SetActive(true);
			this._clonedObjects.Add(line);
		}
		else
		{
			this.AddLine(LocalStringManager.Get(LanguageKey.LK_MouseTip_DamageStep_Basic), baseValue.ToString());
			bool flag = data.ActivateSkillTemplateId >= 0;
			if (flag)
			{
				ValueTuple<int, string> valueTuple = getSkill(data);
				int combatSkillValue = valueTuple.Item1;
				string combatSkillName = valueTuple.Item2;
				finalValue += combatSkillValue;
				string lineTitle = LocalStringManager.Get(LanguageKey.LK_MouseTip_DamageStep_CombatSkill);
				string lineContent = LocalStringManager.GetFormat(LanguageKey.LK_MouseTip_DamageStep_CombatSkill_Content, combatSkillValue, combatSkillName);
				this.AddLine(lineTitle, lineContent);
			}
			bool flag2 = this._consummateBonus > 0;
			if (flag2)
			{
				int consummateValue = finalValue * this._consummateBonus / 100;
				finalValue += consummateValue;
				string lineTitle2 = LocalStringManager.Get(LanguageKey.LK_MouseTip_DamageStep_ConsummateLevel);
				string consummateLevelName = CommonUtils.GetConsummateLevelShowDataLegacy(this._consummateLevel).Item2;
				string lineContent2 = LocalStringManager.GetFormat(LanguageKey.LK_MouseTip_DamageStep_ConsummateLevel_Content, consummateValue, consummateLevelName);
				this.AddLine(lineTitle2, lineContent2);
			}
			bool flag3 = data.EatingBonusData > 0;
			if (flag3)
			{
				int eatingValue = finalValue * data.EatingBonusData / 100;
				finalValue += eatingValue;
				this.AddLine(LocalStringManager.Get(LanguageKey.LK_MouseTip_DamageStep_Medicine), eatingValue.ToString());
			}
			refers.CGet<TextMeshProUGUI>("Text2").text = finalValue.ToString().SetColor(color);
			line.SetActive(true);
			this._clonedObjects.Add(line);
		}
	}

	// Token: 0x06002912 RID: 10514 RVA: 0x00131B14 File Offset: 0x0012FD14
	private void AddLine(string title, string content)
	{
		GameObject line = Object.Instantiate<GameObject>(base.CGet<GameObject>("LineType_2"), base.transform);
		Refers refers = line.GetComponent<Refers>();
		refers.CGet<TextMeshProUGUI>("Text1").text = title;
		refers.CGet<TextMeshProUGUI>("Text2").text = content;
		line.SetActive(true);
		this._clonedObjects.Add(line);
	}

	// Token: 0x06002913 RID: 10515 RVA: 0x00131B7C File Offset: 0x0012FD7C
	private void Clear()
	{
		foreach (GameObject obj in this._clonedObjects)
		{
			Object.Destroy(obj);
		}
		this._bodyPartType = 0;
		this._consummateLevel = 0;
		this._consummateBonus = 0;
		this._clonedObjects.Clear();
	}

	// Token: 0x06002914 RID: 10516 RVA: 0x00131BF8 File Offset: 0x0012FDF8
	private void Update()
	{
		bool flag = CommonCommandKit.PrimaryInteraction.Check(this.Element, false, false, false, true, false);
		if (flag)
		{
			ViewEncyclopediaPanel.OpenLink(EncyclopediaTipLink.DefValue.Injuries);
		}
	}

	// Token: 0x06002915 RID: 10517 RVA: 0x00131C2C File Offset: 0x0012FE2C
	private ValueTuple<int, string> GetCombatSkillOuterDamageStep(DamageStepDisplayData displayData)
	{
		CombatSkillItem config = CombatSkill.Instance[displayData.ActivateSkillTemplateId];
		int result = config.OuterDamageSteps[(int)this._bodyPartType];
		CValuePercentBonus bonus = displayData.ActivateSkillBonusData.OuterInjuryStepBonus;
		result *= bonus;
		return new ValueTuple<int, string>(result, config.Name.SetColor(Colors.Instance.GradeColors[(int)config.Grade]));
	}

	// Token: 0x06002916 RID: 10518 RVA: 0x00131C9C File Offset: 0x0012FE9C
	private ValueTuple<int, string> GetCombatSkillInnerDamageStep(DamageStepDisplayData displayData)
	{
		CombatSkillItem config = CombatSkill.Instance[displayData.ActivateSkillTemplateId];
		int result = config.InnerDamageSteps[(int)this._bodyPartType];
		CValuePercentBonus bonus = displayData.ActivateSkillBonusData.InnerInjuryStepBonus;
		result *= bonus;
		return new ValueTuple<int, string>(result, config.Name.SetColor(Colors.Instance.GradeColors[(int)config.Grade]));
	}

	// Token: 0x06002917 RID: 10519 RVA: 0x00131D0C File Offset: 0x0012FF0C
	private ValueTuple<int, string> GetCombatSkillMindDamageStep(DamageStepDisplayData displayData)
	{
		CombatSkillItem config = CombatSkill.Instance[displayData.ActivateSkillTemplateId];
		int result = config.MindDamageStep;
		CValuePercentBonus bonus = displayData.ActivateSkillBonusData.MindStepBonus;
		result *= bonus;
		return new ValueTuple<int, string>(result, config.Name.SetColor(Colors.Instance.GradeColors[(int)config.Grade]));
	}

	// Token: 0x06002918 RID: 10520 RVA: 0x00131D78 File Offset: 0x0012FF78
	private ValueTuple<int, string> GetCombatSkillFatalDamageStep(DamageStepDisplayData displayData)
	{
		CombatSkillItem config = CombatSkill.Instance[displayData.ActivateSkillTemplateId];
		int result = config.FatalDamageStep;
		CValuePercentBonus bonus = displayData.ActivateSkillBonusData.FatalStepBonus;
		result *= bonus;
		return new ValueTuple<int, string>(result, config.Name.SetColor(Colors.Instance.GradeColors[(int)config.Grade]));
	}

	// Token: 0x04001DEB RID: 7659
	[TupleElementNames(new string[]
	{
		"BodyPartType",
		"BodyPartUiIndex"
	})]
	private readonly Dictionary<string, ValueTuple<sbyte, sbyte>> _bodyPartConstants = new Dictionary<string, ValueTuple<sbyte, sbyte>>
	{
		{
			"Chest",
			new ValueTuple<sbyte, sbyte>(0, 1)
		},
		{
			"Belly",
			new ValueTuple<sbyte, sbyte>(1, 2)
		},
		{
			"Head",
			new ValueTuple<sbyte, sbyte>(2, 0)
		},
		{
			"LeftHand",
			new ValueTuple<sbyte, sbyte>(3, 3)
		},
		{
			"RightHand",
			new ValueTuple<sbyte, sbyte>(4, 4)
		},
		{
			"LeftLeg",
			new ValueTuple<sbyte, sbyte>(5, 5)
		},
		{
			"RightLeg",
			new ValueTuple<sbyte, sbyte>(6, 6)
		}
	};

	// Token: 0x04001DEC RID: 7660
	private readonly Dictionary<string, string> _bodyPartMonthlyHealConstants = new Dictionary<string, string>
	{
		{
			"Chest",
			"Chest"
		},
		{
			"Belly",
			"Belly"
		},
		{
			"Head",
			"Head"
		},
		{
			"LeftHand",
			"Hands"
		},
		{
			"RightHand",
			"Hands"
		},
		{
			"LeftLeg",
			"Feet"
		},
		{
			"RightLeg",
			"Feet"
		}
	};

	// Token: 0x04001DED RID: 7661
	private readonly List<GameObject> _clonedObjects = new List<GameObject>();

	// Token: 0x04001DEE RID: 7662
	private sbyte _bodyPartType;

	// Token: 0x04001DEF RID: 7663
	private sbyte _consummateLevel;

	// Token: 0x04001DF0 RID: 7664
	private int _consummateBonus;

	// Token: 0x04001DF1 RID: 7665
	private string _title;

	// Token: 0x04001DF2 RID: 7666
	private string _type;

	// Token: 0x04001DF3 RID: 7667
	private Injuries _injuries;

	// Token: 0x04001DF4 RID: 7668
	private CompleteDamageStepDisplayData _data;

	// Token: 0x04001DF5 RID: 7669
	private List<bool> _allBodyPartExists;
}
