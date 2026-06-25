using System;
using Config;
using GameData.Domains.Combat;

// Token: 0x02000156 RID: 342
public static class CombatConstants
{
	// Token: 0x060012E2 RID: 4834 RVA: 0x00073468 File Offset: 0x00071668
	public static string ParseMarkType(DefeatMarkKey markKey)
	{
		string languageKey = "LK_Combat_MarkType_" + markKey.Type.GetGroup().ToString();
		return LocalStringManager.Get(languageKey);
	}

	// Token: 0x060012E3 RID: 4835 RVA: 0x000734A8 File Offset: 0x000716A8
	public static string ParseMarkName(DefeatMarkKey markKey)
	{
		bool flag = markKey.BodyPart >= 0;
		string ret;
		string contentColor;
		if (flag)
		{
			string bodyPartName = BodyPart.Instance[markKey.BodyPart].Name;
			EMarkType type = markKey.Type;
			if (!true)
			{
			}
			string text;
			switch (type)
			{
			case EMarkType.Outer:
				text = LocalStringManager.Get(LanguageKey.LK_Out_Injury);
				break;
			case EMarkType.Inner:
				text = LocalStringManager.Get(LanguageKey.LK_Inner_Injury);
				break;
			case EMarkType.Flaw:
				text = LocalStringManager.Get(LanguageKey.LK_Combat_Flaw);
				break;
			case EMarkType.Acupoint:
				text = LocalStringManager.Get(LanguageKey.LK_Combat_Acupoint);
				break;
			default:
				text = string.Empty;
				break;
			}
			if (!true)
			{
			}
			string injuryName = text;
			ret = LocalStringManager.GetFormat(LanguageKey.LK_Combat_BodyPartMark, bodyPartName, injuryName);
			EMarkType type2 = markKey.Type;
			contentColor = ((type2 == EMarkType.Outer || type2 == EMarkType.Flaw) ? "outterinjury" : "innerinjury");
		}
		else
		{
			bool flag2 = markKey.PoisonType >= 0;
			if (flag2)
			{
				PoisonItem poisonConfig = Poison.Instance[markKey.PoisonType];
				ret = LocalStringManager.GetFormat(LanguageKey.LK_Combat_PoisonMark, poisonConfig.Name, LocalStringManager.Get(LanguageKey.LK_Mark));
				contentColor = poisonConfig.FontColor;
			}
			else
			{
				string markTypeName = (markKey.Type == EMarkType.Mind) ? LocalStringManager.Get(LanguageKey.LK_Combat_MarkName_Mind) : CombatConstants.ParseMarkType(markKey);
				ret = LocalStringManager.GetFormat(LanguageKey.LK_Combat_GenericMark, markTypeName, LocalStringManager.Get(LanguageKey.LK_Mark));
				EMarkType type3 = markKey.Type;
				if (!true)
				{
				}
				string text;
				switch (type3)
				{
				case EMarkType.Fatal:
					text = "fataldamage";
					break;
				case EMarkType.Die:
					text = "diemark";
					break;
				case EMarkType.Wug:
					text = "wugking";
					break;
				case EMarkType.QiDisorder:
					text = "outterinjury";
					break;
				case EMarkType.State:
					text = "darkred";
					break;
				default:
					text = "pinkyellow";
					break;
				}
				if (!true)
				{
				}
				contentColor = text;
			}
		}
		bool flag3 = !contentColor.IsNullOrEmpty();
		if (flag3)
		{
			ret = ret.SetColor(contentColor);
		}
		return ret;
	}

	// Token: 0x060012E4 RID: 4836 RVA: 0x00073694 File Offset: 0x00071894
	public static string ParseMarkIcon(DefeatMarkKey markKey)
	{
		EMarkType type = markKey.Type;
		if (!true)
		{
		}
		string result;
		switch (type)
		{
		case EMarkType.Outer:
		case EMarkType.Flaw:
			result = string.Format("mousetip_waishang_{0}", markKey.UiIncorrectBodyPart);
			break;
		case EMarkType.Inner:
		case EMarkType.Acupoint:
			result = string.Format("mousetip_neishang_{0}", markKey.UiIncorrectBodyPart);
			break;
		case EMarkType.Poison:
			result = string.Format("mousetip_duxing_circular_{0}", markKey.PoisonType);
			break;
		case EMarkType.Mind:
			result = "mousetip_dongxin_0";
			break;
		case EMarkType.Fatal:
			result = "mousetip_zhongchuang_0";
			break;
		case EMarkType.Die:
			result = "mousetip_die";
			break;
		case EMarkType.Wug:
			result = "mousetip_wugking";
			break;
		case EMarkType.QiDisorder:
			result = "mousetip_qi_0";
			break;
		case EMarkType.State:
			result = "mousetip_state_0";
			break;
		case EMarkType.NeiliAllocation:
			result = "mousetip_zhenqi_4";
			break;
		case EMarkType.Health:
			result = "mousetip_jiankang";
			break;
		case EMarkType.Scar:
			result = "mousetip_scar_0";
			break;
		case EMarkType.Tired:
			result = "mousetip_tired";
			break;
		default:
			throw new Exception(string.Format("Cannot analysis mark key type {0}", markKey.Type));
		}
		if (!true)
		{
		}
		return result;
	}

	// Token: 0x060012E5 RID: 4837 RVA: 0x000737BC File Offset: 0x000719BC
	public static string ParseDamageMarkName(DefeatMarkKey markKey)
	{
		EMarkType type = markKey.Type;
		if (!true)
		{
		}
		string result;
		if (type != EMarkType.Outer)
		{
			if (type != EMarkType.Inner)
			{
				result = CombatConstants.ParseMarkName(markKey);
			}
			else
			{
				result = (LocalStringManager.Get(LanguageKey.LK_Inner_Injury) + LocalStringManager.Get(LanguageKey.LK_Mark)).SetColor("innerinjury");
			}
		}
		else
		{
			result = (LocalStringManager.Get(LanguageKey.LK_Out_Injury) + LocalStringManager.Get(LanguageKey.LK_Mark)).SetColor("outterinjury");
		}
		if (!true)
		{
		}
		return result;
	}

	// Token: 0x060012E6 RID: 4838 RVA: 0x00073840 File Offset: 0x00071A40
	public static string ParseDamageValueName(DefeatMarkKey markKey)
	{
		EMarkType type = markKey.Type;
		if (!true)
		{
		}
		string result;
		switch (type)
		{
		case EMarkType.Outer:
		case EMarkType.Inner:
			result = LocalStringManager.Get(LanguageKey.LK_Combat_DamageValue_Progress_Injury);
			goto IL_98;
		case EMarkType.Poison:
			result = LocalStringManager.Get(LanguageKey.LK_Combat_DamageValue_Progress_Poison);
			goto IL_98;
		case EMarkType.Mind:
			result = LocalStringManager.Get(LanguageKey.LK_Combat_DamageValue_Progress_Mind);
			goto IL_98;
		case EMarkType.Fatal:
			result = LocalStringManager.Get(LanguageKey.LK_Combat_DamageValue_Progress_Fatal);
			goto IL_98;
		case EMarkType.State:
			result = LocalStringManager.Get(LanguageKey.LK_Combat_DamageValue_Progress_State);
			goto IL_98;
		}
		throw new ArgumentOutOfRangeException("markKey", markKey, "Cannot analysis mark key type");
		IL_98:
		if (!true)
		{
		}
		return result;
	}

	// Token: 0x060012E7 RID: 4839 RVA: 0x000738F0 File Offset: 0x00071AF0
	public static string ParseDamageValueTitle(DefeatMarkKey markKey)
	{
		EMarkType type = markKey.Type;
		if (!true)
		{
		}
		string result;
		switch (type)
		{
		case EMarkType.Outer:
		case EMarkType.Inner:
			result = BodyPart.Instance[markKey.BodyPart].Name;
			goto IL_B0;
		case EMarkType.Poison:
			result = Poison.Instance[markKey.PoisonType].Name;
			goto IL_B0;
		case EMarkType.Mind:
			result = LocalStringManager.Get(LanguageKey.LK_Combat_MarkName_Mind);
			goto IL_B0;
		case EMarkType.Fatal:
			result = LocalStringManager.Get(LanguageKey.LK_Combat_MarkType_Fatal);
			goto IL_B0;
		case EMarkType.State:
			result = LocalStringManager.Get(LanguageKey.LK_Combat_DamageValue_Title_State);
			goto IL_B0;
		}
		throw new ArgumentOutOfRangeException("markKey", markKey, "Cannot analysis mark key type");
		IL_B0:
		if (!true)
		{
		}
		return result;
	}
}
