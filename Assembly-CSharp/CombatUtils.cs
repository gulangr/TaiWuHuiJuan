using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using Config;
using DG.Tweening;
using FrameWork;
using GameData.Domains.Character.Display;
using GameData.Domains.Combat;
using GameData.Domains.World;
using UnityEngine;

// Token: 0x0200015E RID: 350
public static class CombatUtils
{
	// Token: 0x17000225 RID: 549
	// (get) Token: 0x06001344 RID: 4932 RVA: 0x000767FE File Offset: 0x000749FE
	public static bool CombatPureOpen
	{
		get
		{
			return SingletonObject.getInstance<GlobalSettings>().CombatPure;
		}
	}

	// Token: 0x06001345 RID: 4933 RVA: 0x0007680C File Offset: 0x00074A0C
	public static void PlayAndHideParticle(ParticleSystem particle, float hideTime = 2f)
	{
		bool activeSelf = particle.gameObject.activeSelf;
		if (!activeSelf)
		{
			particle.gameObject.SetActive(true);
			particle.Play(true);
			DOVirtual.DelayedCall(hideTime, delegate
			{
				particle.gameObject.SetActive(false);
			}, true);
		}
	}

	// Token: 0x06001346 RID: 4934 RVA: 0x00076870 File Offset: 0x00074A70
	public static void UpdateIconHolderVisible(Transform holder)
	{
		for (int i = 0; i < holder.childCount; i++)
		{
			bool activeSelf = holder.GetChild(i).gameObject.activeSelf;
			if (activeSelf)
			{
				holder.gameObject.SetActive(true);
				return;
			}
		}
		holder.gameObject.SetActive(false);
	}

	// Token: 0x06001347 RID: 4935 RVA: 0x000768C8 File Offset: 0x00074AC8
	public static void ShowCharMenu(int charId)
	{
		CombatModel model = SingletonObject.getInstance<CombatModel>();
		bool isAlly = model.SelfTeam.Contains(charId);
		List<int> charMenuIdList = EasyPool.Get<List<int>>();
		charMenuIdList.Clear();
		IReadOnlyList<int> team = isAlly ? model.SelfTeam : model.EnemyTeam;
		bool flag = charId == team[0];
		if (flag)
		{
			charMenuIdList.AddRange(team);
		}
		else
		{
			charMenuIdList.Add(charId);
		}
		ArgumentBox argBox = EasyPool.Get<ArgumentBox>();
		argBox.SetObject("CharacterIdList", charMenuIdList);
		argBox.Set("CharacterId", charId);
		argBox.Set("Anonymous", !isAlly && model.Config.EnemyAnonymous);
		argBox.Set("CanOperate", false);
		argBox.Set("PreviousView", 2);
		Dictionary<int, List<sbyte>> dict = EasyPool.Get<Dictionary<int, List<sbyte>>>();
		dict.Clear();
		foreach (int teammateId in model.SelfTeam)
		{
			CombatSubProcessorCharacter processor;
			bool flag2 = !model.ProcessorCharacters.TryGetValue(teammateId, out processor);
			if (!flag2)
			{
				dict.Add(teammateId, processor.CurrTeammateCommands);
			}
		}
		foreach (int teammateId2 in model.EnemyTeam)
		{
			CombatSubProcessorCharacter processor2;
			bool flag3 = !model.ProcessorCharacters.TryGetValue(teammateId2, out processor2);
			if (!flag3)
			{
				dict.Add(teammateId2, processor2.CurrTeammateCommands);
			}
		}
		argBox.SetObject("ReplacedTeammateCommands", dict);
		UIElement.CharacterMenu.OnShowed = delegate()
		{
			model.RaiseEvent(ECombatEvents.OnCharacterMenuShowed);
		};
		UIElement.CharacterMenu.OnHide = delegate()
		{
			model.RaiseEvent(ECombatEvents.OnCharacterMenuHide);
		};
		UIElement.CharacterMenu.SetOnInitArgs(argBox);
		UIManager.Instance.ShowUI(UIElement.CharacterMenu, true);
		model.RaiseEvent(ECombatEvents.BeforeCharacterMenuShowed);
	}

	// Token: 0x06001348 RID: 4936 RVA: 0x00076B08 File Offset: 0x00074D08
	public static string GetNameString(CharacterDisplayData displayData, bool isAlly)
	{
		CombatModel model = SingletonObject.getInstance<CombatModel>();
		bool isMainChar = displayData.CharacterId == (isAlly ? model.SelfTeam[0] : model.EnemyTeam[0]);
		bool flag = !isAlly && isMainChar && model.Config.EnemyAnonymous;
		string result;
		if (flag)
		{
			result = Character.Instance[displayData.TemplateId].AnonymousTitle;
		}
		else
		{
			bool flag2 = displayData.FullName.Type == 0 && SharedMethods.SmallVillageXiangshu((short)displayData.OrgInfo.OrgTemplateId, false);
			if (flag2)
			{
				result = CommonUtils.GetXiangshuMinion0AnonymousTitle();
			}
			else
			{
				result = NameCenter.GetMonasticTitleOrDisplayName(displayData, isAlly && isMainChar && model.TaiwuInCombat);
			}
		}
		return result;
	}

	// Token: 0x06001349 RID: 4937 RVA: 0x00076BBC File Offset: 0x00074DBC
	public static void SetCombatSkillTips(TooltipInvoker mouseTip, int charId, short skillId)
	{
		ArgumentBox argumentBox;
		if ((argumentBox = mouseTip.RuntimeParam) == null)
		{
			argumentBox = (mouseTip.RuntimeParam = new ArgumentBox());
		}
		ArgumentBox param = argumentBox;
		param.Clear();
		param.Set("CombatSkillId", skillId).Set("CharId", charId).Set("UsePracticeLevelInDisplayData", true);
		bool showing = mouseTip.Showing;
		if (showing)
		{
			mouseTip.Refresh(false, -1);
		}
	}

	// Token: 0x0600134A RID: 4938 RVA: 0x00076C24 File Offset: 0x00074E24
	public static string StyleCountDownText(float cd, bool permanentLock)
	{
		string result;
		if (permanentLock)
		{
			result = "∞";
		}
		else
		{
			bool flag = cd > 10f;
			if (flag)
			{
				result = Math.Round((double)cd, 0).ToString(CultureInfo.CurrentCulture);
			}
			else
			{
				result = cd.ToString("0.0");
			}
		}
		return result;
	}

	// Token: 0x0600134B RID: 4939 RVA: 0x00076C74 File Offset: 0x00074E74
	public static bool FindTransferProportionEffectFromCollection(SkillEffectCollection effectCollection, out int count, out CombatSkillItem existCombatSkillItem, out SpecialEffectItem existSpecialEffectItem)
	{
		bool exist = false;
		count = 0;
		existCombatSkillItem = null;
		existSpecialEffectItem = null;
		bool flag = effectCollection.EffectDict != null;
		if (flag)
		{
			foreach (KeyValuePair<SkillEffectKey, short> pair in effectCollection.EffectDict)
			{
				CombatSkillItem combatSkillItem = CombatSkill.Instance[pair.Key.SkillId];
				int effectId = pair.Key.IsDirect ? combatSkillItem.DirectEffectID : combatSkillItem.ReverseEffectID;
				SpecialEffectItem specialEffectItem = SpecialEffect.Instance[effectId];
				bool flag2 = specialEffectItem.TransferProportion > 0;
				if (flag2)
				{
					exist = true;
					count = (int)pair.Value;
					existCombatSkillItem = combatSkillItem;
					existSpecialEffectItem = specialEffectItem;
				}
			}
		}
		return exist;
	}

	// Token: 0x0600134C RID: 4940 RVA: 0x00076D54 File Offset: 0x00074F54
	public static void UpdateInjuryTips(TooltipInvoker tip, int charId)
	{
		if (tip.RuntimeParam == null)
		{
			tip.RuntimeParam = new ArgumentBox();
		}
		tip.RuntimeParam.Set("characterId", charId);
		tip.Refresh(false, -1);
	}

	// Token: 0x0600134D RID: 4941 RVA: 0x00076D98 File Offset: 0x00074F98
	public static float ChangeCharacterWaitTime()
	{
		return 0.5f;
	}

	// Token: 0x0400102C RID: 4140
	public const string DefeatMarkPrefabKey = "ui_Combat_DefeatMarkPrefab";

	// Token: 0x0400102D RID: 4141
	public const string DefeatMarkSeparatorPrefabKey = "ui_Combat_DefeatMarkSeparatorPrefab";

	// Token: 0x0400102E RID: 4142
	public const string TextTipsPrefabKey = "ui_Combat_TextTipsPrefab";

	// Token: 0x0400102F RID: 4143
	public const string IconTipsPrefab = "ui_Combat_IconTipsPrefab";

	// Token: 0x04001030 RID: 4144
	public const string EffectTipsPrefabKey = "ui_Combat_EffectTipsPrefab";

	// Token: 0x04001031 RID: 4145
	public const string CommandBubblePrefabKey = "ui_Combat_CommandBubblePrefab";

	// Token: 0x04001032 RID: 4146
	public const string TrickPrefabKey = "ui_Combat_TrickPrefab";

	// Token: 0x04001033 RID: 4147
	public const string DamageNumPrefabKey = "ui_Combat_Damage_Num";

	// Token: 0x04001034 RID: 4148
	public const string FatalDamageNumPrefabKey = "ui_Combat_Fatal_Damage_Num";
}
