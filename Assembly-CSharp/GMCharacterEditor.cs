using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text;
using Config;
using DisplayConfig;
using GameData.Common;
using GameData.Domains.Character;
using GameData.Domains.Character.Display;
using GameData.Domains.Combat;
using GameData.Domains.Extra;
using GameData.Domains.Item;
using GameData.Domains.Map;
using GameData.Domains.Taiwu.Profession;
using GameData.GameDataBridge;
using GameData.Serializer;
using GameData.Utilities;
using GM;
using TMPro;
using UnityEngine;

// Token: 0x02000208 RID: 520
public class GMCharacterEditor : Refers
{
	// Token: 0x06002122 RID: 8482 RVA: 0x000F123C File Offset: 0x000EF43C
	public int GetCharacterId()
	{
		return this._characterId;
	}

	// Token: 0x06002123 RID: 8483 RVA: 0x000F1244 File Offset: 0x000EF444
	public static bool IsCharacterEditorFunc(MethodInfo methodInfo)
	{
		ParameterInfo[] methodParams = methodInfo.GetParameters();
		GMFuncArgAttribute[] args = new GMFuncArgAttribute[methodParams.Length];
		foreach (GMFuncArgAttribute arg in methodInfo.GetCustomAttributes<GMFuncArgAttribute>())
		{
			args[arg.Index] = arg;
		}
		return !methodInfo.Name.Equals("EditCharacterInfo") && args.Length != 0 && args[0] != null && args[0].WidgetType == EWidgetType.CharIdField;
	}

	// Token: 0x06002124 RID: 8484 RVA: 0x000F12D8 File Offset: 0x000EF4D8
	public void Awake()
	{
		base.gameObject.SetActive(false);
	}

	// Token: 0x06002125 RID: 8485 RVA: 0x000F12E8 File Offset: 0x000EF4E8
	public unsafe void SetCharacterId(int target)
	{
		bool flag = this._characterId >= 0;
		if (flag)
		{
			this._characterId = -1;
		}
		this._characterId = target;
		bool flag2 = target >= 0;
		if (flag2)
		{
			GMCharacterEditor.<>c__DisplayClass11_0 CS$<>8__locals1 = new GMCharacterEditor.<>c__DisplayClass11_0();
			CS$<>8__locals1.<>4__this = this;
			this.GenderDisplay.text = (this.InfoDisplay.text = string.Empty);
			CS$<>8__locals1.sb = new StringBuilder();
			ExtraDomainMethod.AsyncCall.GetCharacterMasteredCombatSkills(null, this._characterId, delegate(int offset, RawDataPool dataPool)
			{
				GameData.Utilities.ShortList value = default(GameData.Utilities.ShortList);
				Serializer.Deserialize(dataPool, offset, ref value);
				CS$<>8__locals1.sb.Clear();
				CS$<>8__locals1.sb.Append(LocalStringManager.Get(LanguageKey.GM_CombatSkill_MasteredCombatSkills) + "：");
				bool flag3 = value.Items == null || value.Items.Count == 0;
				if (flag3)
				{
					CS$<>8__locals1.sb.Append(LocalStringManager.Get(LanguageKey.LK_Common_None) ?? "");
				}
				else
				{
					foreach (short element in value.Items)
					{
						CS$<>8__locals1.sb.Append(CombatSkill.Instance[element].Name + ", ");
					}
				}
				CS$<>8__locals1.sb.AppendLine();
				CS$<>8__locals1.<>4__this._masteredCombatSkillString = CS$<>8__locals1.sb.ToString();
			});
			ValueTuple<ushort, Type>[] dataFieldIds = new ValueTuple<ushort, Type>[]
			{
				new ValueTuple<ushort, Type>(0, typeof(int)),
				new ValueTuple<ushort, Type>(1, typeof(short)),
				new ValueTuple<ushort, Type>(3, typeof(sbyte)),
				new ValueTuple<ushort, Type>(19, typeof(short)),
				new ValueTuple<ushort, Type>(20, typeof(short)),
				new ValueTuple<ushort, Type>(35, typeof(short)),
				new ValueTuple<ushort, Type>(36, typeof(short)),
				new ValueTuple<ushort, Type>(110, typeof(NeiliProportionOfFiveElements)),
				new ValueTuple<ushort, Type>(27, typeof(int)),
				new ValueTuple<ushort, Type>(28, typeof(sbyte)),
				new ValueTuple<ushort, Type>(21, typeof(short)),
				new ValueTuple<ushort, Type>(109, typeof(NeiliAllocation)),
				new ValueTuple<ushort, Type>(4, typeof(short)),
				new ValueTuple<ushort, Type>(5, typeof(sbyte)),
				new ValueTuple<ushort, Type>(6, typeof(sbyte)),
				new ValueTuple<ushort, Type>(7, typeof(short)),
				new ValueTuple<ushort, Type>(13, typeof(bool)),
				new ValueTuple<ushort, Type>(14, typeof(bool)),
				new ValueTuple<ushort, Type>(16, typeof(byte)),
				new ValueTuple<ushort, Type>(17, typeof(List<short>)),
				new ValueTuple<ushort, Type>(18, typeof(MainAttributes)),
				new ValueTuple<ushort, Type>(22, typeof(bool)),
				new ValueTuple<ushort, Type>(23, typeof(bool)),
				new ValueTuple<ushort, Type>(24, typeof(bool)),
				new ValueTuple<ushort, Type>(25, typeof(bool)),
				new ValueTuple<ushort, Type>(26, typeof(Injuries)),
				new ValueTuple<ushort, Type>(30, typeof(LifeSkillShorts)),
				new ValueTuple<ushort, Type>(31, typeof(sbyte)),
				new ValueTuple<ushort, Type>(32, typeof(CombatSkillShorts)),
				new ValueTuple<ushort, Type>(33, typeof(sbyte)),
				new ValueTuple<ushort, Type>(44, typeof(PoisonInts)),
				new ValueTuple<ushort, Type>(57, typeof(Inventory)),
				new ValueTuple<ushort, Type>(64, typeof(byte)),
				new ValueTuple<ushort, Type>(34, typeof(ResourceInts)),
				new ValueTuple<ushort, Type>(116, typeof(CombatSkillEquipment)),
				new ValueTuple<ushort, Type>(66, typeof(int)),
				new ValueTuple<ushort, Type>(9, typeof(sbyte)),
				new ValueTuple<ushort, Type>(10, typeof(sbyte)),
				new ValueTuple<ushort, Type>(11, typeof(sbyte)),
				new ValueTuple<ushort, Type>(12, typeof(sbyte)),
				new ValueTuple<ushort, Type>(48, typeof(NeiliAllocation))
			};
			ValueTuple<ushort, Type>[] cacheFieldIds = new ValueTuple<ushort, Type>[]
			{
				new ValueTuple<ushort, Type>(75, typeof(short)),
				new ValueTuple<ushort, Type>(76, typeof(sbyte)),
				new ValueTuple<ushort, Type>(77, typeof(short)),
				new ValueTuple<ushort, Type>(78, typeof(short)),
				new ValueTuple<ushort, Type>(80, typeof(HitOrAvoidInts)),
				new ValueTuple<ushort, Type>(81, typeof(OuterAndInnerInts)),
				new ValueTuple<ushort, Type>(82, typeof(HitOrAvoidInts)),
				new ValueTuple<ushort, Type>(83, typeof(OuterAndInnerInts)),
				new ValueTuple<ushort, Type>(84, typeof(OuterAndInnerShorts)),
				new ValueTuple<ushort, Type>(85, typeof(short)),
				new ValueTuple<ushort, Type>(86, typeof(short)),
				new ValueTuple<ushort, Type>(87, typeof(short)),
				new ValueTuple<ushort, Type>(88, typeof(short)),
				new ValueTuple<ushort, Type>(89, typeof(short)),
				new ValueTuple<ushort, Type>(90, typeof(short)),
				new ValueTuple<ushort, Type>(91, typeof(short)),
				new ValueTuple<ushort, Type>(92, typeof(short)),
				new ValueTuple<ushort, Type>(93, typeof(PoisonInts)),
				new ValueTuple<ushort, Type>(95, typeof(short)),
				new ValueTuple<ushort, Type>(97, typeof(LifeSkillShorts)),
				new ValueTuple<ushort, Type>(99, typeof(CombatSkillShorts)),
				new ValueTuple<ushort, Type>(96, typeof(LifeSkillShorts)),
				new ValueTuple<ushort, Type>(98, typeof(CombatSkillShorts)),
				new ValueTuple<ushort, Type>(100, typeof(Personalities)),
				new ValueTuple<ushort, Type>(103, typeof(int)),
				new ValueTuple<ushort, Type>(104, typeof(int)),
				new ValueTuple<ushort, Type>(105, typeof(int)),
				new ValueTuple<ushort, Type>(106, typeof(int)),
				new ValueTuple<ushort, Type>(108, typeof(int)),
				new ValueTuple<ushort, Type>(111, typeof(sbyte)),
				new ValueTuple<ushort, Type>(112, typeof(int)),
				new ValueTuple<ushort, Type>(94, typeof(short))
			};
			ValueTuple<DataUid, Type>[] recevingParams = new ValueTuple<DataUid, Type>[dataFieldIds.Length + cacheFieldIds.Length];
			for (int i = 0; i < dataFieldIds.Length; i++)
			{
				recevingParams[i] = new ValueTuple<DataUid, Type>(this.<SetCharacterId>g__CharacterFieldUid|11_5(dataFieldIds[i].Item1), dataFieldIds[i].Item2);
			}
			for (int j = 0; j < cacheFieldIds.Length; j++)
			{
				recevingParams[dataFieldIds.Length + j] = new ValueTuple<DataUid, Type>(this.<SetCharacterId>g__CharacterFieldUid|11_5(cacheFieldIds[j].Item1), cacheFieldIds[j].Item2);
			}
			UI_GMWindow.Instance.RequestGameDataReceiving(delegate(Dictionary<DataUid, object> data)
			{
				CS$<>8__locals1.sb.Clear();
				CS$<>8__locals1.sb.AppendFormat("Id: {0} TemplateId: {1}", (int)data[recevingParams[0].Item1], (short)data[recevingParams[1].Item1]);
				CS$<>8__locals1.sb.AppendLine();
				sbyte gender = (sbyte)data[recevingParams[2].Item1];
				byte monkType = (byte)data[recevingParams[18].Item1];
				bool flag3 = (bool)data[recevingParams[16].Item1];
				if (flag3)
				{
					CS$<>8__locals1.sb.AppendLine(LocalStringManager.Get((gender == 1) ? LanguageKey.UI_NewGame_FemaleLike : LanguageKey.UI_NewGame_MaleLike));
				}
				bool flag4 = (bool)data[recevingParams[17].Item1];
				if (flag4)
				{
					CS$<>8__locals1.sb.AppendLine(LocalStringManager.Get("LK_BiSexual"));
				}
				CS$<>8__locals1.sb.AppendLine(LocalStringManager.Get(string.Format("GM_Enum_MonkType_{0}", monkType)));
				TextMeshProUGUI genderDisplay = CS$<>8__locals1.<>4__this.GenderDisplay;
				genderDisplay.text += string.Format("{0} ({1}: {2})", CommonUtils.GetGenderString(CommonUtils.GetDisplayGender(gender, -1)), LocalStringManager.Get("LK_Char_Age"), (short)data[recevingParams[12].Item1]);
				CS$<>8__locals1.sb.AppendFormat(LocalStringManager.Get("UI_NewGame_BornDateInfo"), Month.Instance[(sbyte)data[recevingParams[13].Item1]].Name);
				CS$<>8__locals1.sb.AppendLine();
				List<short> featureIds = (List<short>)data[recevingParams[19].Item1];
				int k = 0;
				int len = featureIds.Count;
				while (k < len)
				{
					bool flag5 = k != 0;
					if (flag5)
					{
						CS$<>8__locals1.sb.Append(", ");
					}
					else
					{
						CS$<>8__locals1.sb.Append(LocalStringManager.Get("LK_Feature") + ": ");
					}
					CS$<>8__locals1.sb.Append(CharacterFeature.Instance[featureIds[k]].Name);
					bool flag6 = k == len - 1;
					if (flag6)
					{
						CS$<>8__locals1.sb.AppendLine();
					}
					k++;
				}
				MainAttributes baseAttr = (MainAttributes)data[recevingParams[20].Item1];
				int l = 0;
				int len2 = 6;
				while (l < len2)
				{
					bool flag7 = l != 0;
					if (flag7)
					{
						CS$<>8__locals1.sb.Append(", ");
					}
					else
					{
						CS$<>8__locals1.sb.Append(LocalStringManager.Get("GM_FieldName_BaseMainAttributes") + ": ");
					}
					CS$<>8__locals1.sb.Append(string.Format("{0} {1}", LocalStringManager.Get(CS$<>8__locals1.<>4__this.BasicAttributeLanguageKey[l]), *(ref baseAttr.Items.FixedElementField + (IntPtr)l * 2)));
					bool flag8 = l == len2 - 1;
					if (flag8)
					{
						CS$<>8__locals1.sb.AppendLine();
					}
					l++;
				}
				Injuries injuries = (Injuries)data[recevingParams[25].Item1];
				int m = 0;
				int len3 = 7;
				while (m < len3)
				{
					ValueTuple<sbyte, sbyte> injury = injuries.Get((sbyte)m);
					bool flag9 = m != 0;
					if (flag9)
					{
						CS$<>8__locals1.sb.Append(", ");
					}
					CS$<>8__locals1.sb.AppendFormat("{0}[{1}]", BodyPart.Instance[m].Name, string.Format("{0} {1} {2} {3}", new object[]
					{
						LocalStringManager.Get("LK_Out_Injury"),
						injury.Item1,
						LocalStringManager.Get("LK_Inner_Injury"),
						injury.Item2
					}));
					bool flag10 = m == len3 - 1;
					if (flag10)
					{
						CS$<>8__locals1.sb.AppendLine();
					}
					m++;
				}
				PoisonInts poisons = (PoisonInts)data[recevingParams[30].Item1];
				for (sbyte order = 0; order < 6; order += 1)
				{
					sbyte type = PoisonType.GetTypeBySortingOrder(order);
					bool flag11 = type != 0;
					if (flag11)
					{
						CS$<>8__locals1.sb.Append(", ");
					}
					CS$<>8__locals1.sb.AppendFormat("{0}[{1}]", Poison.Instance[type].Name, *(ref poisons.Items.FixedElementField + (IntPtr)type * 4));
					bool flag12 = type == 5;
					if (flag12)
					{
						CS$<>8__locals1.sb.AppendLine();
					}
				}
				LifeSkillShorts values = (LifeSkillShorts)data[recevingParams[26].Item1];
				sbyte b = (sbyte)data[recevingParams[27].Item1];
				sbyte b2 = b;
				LanguageKey growth;
				if (b2 != 1)
				{
					if (b2 != 2)
					{
						growth = LanguageKey.LK_Qualification_Growth_Average;
					}
					else
					{
						growth = LanguageKey.LK_Qualification_Growth_LateBlooming;
					}
				}
				else
				{
					growth = LanguageKey.LK_Qualification_Growth_Precocious;
				}
				int n = 0;
				int len4 = 16;
				while (n < len4)
				{
					bool flag13 = n != 0;
					if (flag13)
					{
						CS$<>8__locals1.sb.Append(", ");
					}
					else
					{
						CS$<>8__locals1.sb.Append(string.Concat(new string[]
						{
							LocalStringManager.Get(LanguageKey.LK_Base),
							LocalStringManager.Get(LanguageKey.LK_Endowments_LifeSkill),
							"(",
							LocalStringManager.Get(growth),
							"): "
						}));
					}
					CS$<>8__locals1.sb.Append(string.Format("{0} {1}", Config.LifeSkillType.Instance[n].Name, *(ref values.Items.FixedElementField + (IntPtr)n * 2)));
					bool flag14 = n == len4 - 1;
					if (flag14)
					{
						CS$<>8__locals1.sb.AppendLine();
					}
					n++;
				}
				CombatSkillShorts values2 = (CombatSkillShorts)data[recevingParams[28].Item1];
				sbyte b3 = (sbyte)data[recevingParams[29].Item1];
				sbyte b4 = b3;
				LanguageKey growth2;
				if (b4 != 1)
				{
					if (b4 != 2)
					{
						growth2 = LanguageKey.LK_Qualification_Growth_Average;
					}
					else
					{
						growth2 = LanguageKey.LK_Qualification_Growth_LateBlooming;
					}
				}
				else
				{
					growth2 = LanguageKey.LK_Qualification_Growth_Precocious;
				}
				int i2 = 0;
				int len5 = 14;
				while (i2 < len5)
				{
					bool flag15 = i2 != 0;
					if (flag15)
					{
						CS$<>8__locals1.sb.Append(", ");
					}
					else
					{
						CS$<>8__locals1.sb.Append(string.Concat(new string[]
						{
							LocalStringManager.Get(LanguageKey.LK_Base),
							LocalStringManager.Get(LanguageKey.LK_Endowments_CombateSkill),
							"(",
							LocalStringManager.Get(growth2),
							"): "
						}));
					}
					CS$<>8__locals1.sb.Append(string.Format("{0} {1}", CombatSkillType.Instance[i2].Name, *(ref values2.Items.FixedElementField + (IntPtr)i2 * 2)));
					bool flag16 = i2 == len5 - 1;
					if (flag16)
					{
						CS$<>8__locals1.sb.AppendLine();
					}
					i2++;
				}
				bool flag17 = !(bool)data[recevingParams[21].Item1];
				if (flag17)
				{
					CS$<>8__locals1.sb.AppendLine(LocalStringManager.Get("GM_FieldName_DHaveLeftArm"));
				}
				bool flag18 = !(bool)data[recevingParams[22].Item1];
				if (flag18)
				{
					CS$<>8__locals1.sb.AppendLine(LocalStringManager.Get("GM_FieldName_DHaveRightArm"));
				}
				bool flag19 = !(bool)data[recevingParams[23].Item1];
				if (flag19)
				{
					CS$<>8__locals1.sb.AppendLine(LocalStringManager.Get("GM_FieldName_DHaveLeftLeg"));
				}
				bool flag20 = !(bool)data[recevingParams[24].Item1];
				if (flag20)
				{
					CS$<>8__locals1.sb.AppendLine(LocalStringManager.Get("GM_FieldName_DHaveRightLeg"));
				}
				CS$<>8__locals1.sb.AppendFormat(LocalStringManager.Get("GM_Message_GMFunc_QueryHealth_Msg"), (short)data[recevingParams[3].Item1], (short)data[recevingParams[4].Item1], (short)data[recevingParams[dataFieldIds.Length + 31].Item1]);
				CS$<>8__locals1.sb.AppendLine();
				sbyte happiness = (sbyte)data[recevingParams[14].Item1];
				short morality = (short)data[recevingParams[15].Item1];
				CS$<>8__locals1.sb.Append(LocalStringManager.Get("LK_Main_SummaryInfo_Happiness"));
				CS$<>8__locals1.sb.AppendLine(string.Format(": {0}({1}) ", happiness, CommonUtils.GetHappinessString(HappinessType.GetHappinessType(happiness))));
				CS$<>8__locals1.sb.Append(LocalStringManager.Get("LK_Base") + LocalStringManager.Get("LK_Main_SummaryInfo_Behavior"));
				CS$<>8__locals1.sb.AppendLine(string.Format(": {0}({1}) ", morality, CommonUtils.GetBehaviorString(GameData.Domains.Character.BehaviorType.GetBehaviorType(morality))));
				CS$<>8__locals1.sb.AppendLine(string.Format("{0}: {1}", LocalStringManager.Get("GM_SetXiangshuInfection_Arg1_Name"), (byte)data[recevingParams[32].Item1]));
				short lovingId = (short)data[recevingParams[5].Item1];
				short hatingId = (short)data[recevingParams[6].Item1];
				string lovingMessage = (lovingId >= 0) ? LocalStringManager.Get(string.Format("LK_ItemSubType_{0}", lovingId)) : LocalStringManager.Get("LK_None");
				string hatingMessage = (hatingId >= 0) ? LocalStringManager.Get(string.Format("LK_ItemSubType_{0}", hatingId)) : LocalStringManager.Get("LK_None");
				CS$<>8__locals1.sb.AppendFormat(LocalStringManager.Get("GM_Message_GMFunc_QueryLovingAndHatingItemSubType_Msg"), new object[]
				{
					lovingId,
					lovingMessage,
					hatingId,
					hatingMessage
				});
				CS$<>8__locals1.sb.AppendLine();
				NeiliProportionOfFiveElements ret = (NeiliProportionOfFiveElements)data[recevingParams[7].Item1];
				string log = LocalStringManager.Get("GM_Message_GMFunc_QueryCharacterNeiliInfo_Msg");
				for (int i3 = 0; i3 < 5; i3++)
				{
					bool flag21 = i3 != 0;
					if (flag21)
					{
						log += ", ";
					}
					log += string.Format(" {0}: {1} ", LocalStringManager.Get(string.Format("LK_FiveElements_Type_{0}", i3)), *(ref ret.Items.FixedElementField + i3));
				}
				log += string.Format(", {0}: {1}", LocalStringManager.Get("LK_ExtraNeili"), (int)data[recevingParams[8].Item1]);
				CS$<>8__locals1.sb.AppendLine(log);
				CS$<>8__locals1.sb.AppendLine(string.Format("{0}: {1}", LocalStringManager.Get("LK_Consummate_Title"), (sbyte)data[recevingParams[9].Item1]));
				CS$<>8__locals1.sb.AppendLine(string.Format("{0}: {1}", LocalStringManager.Get("LK_Qi_Disorder"), (short)data[recevingParams[10].Item1]));
				NeiliAllocation ret2 = (NeiliAllocation)data[recevingParams[11].Item1];
				string log2 = LocalStringManager.Get("GM_Message_GMFunc_QueryCharacterNeiliAllocation_Msg");
				for (int i4 = 0; i4 < 4; i4++)
				{
					bool flag22 = i4 != 0;
					if (flag22)
					{
						log2 += ", ";
					}
					log2 += string.Format(" {0}: {1} ", LocalStringManager.Get(string.Format("LK_Neili_Allocation_Type_{0}", i4)), *(ref ret2.Items.FixedElementField + (IntPtr)i4 * 2));
				}
				CS$<>8__locals1.sb.AppendLine(log2);
				NeiliAllocation ret3 = (NeiliAllocation)data[recevingParams[40].Item1];
				string log3 = LocalStringManager.Get(LanguageKey.GM_Message_GMFunc_QueryCharacterExtraNeiliAllocation_Msg);
				for (int i5 = 0; i5 < 4; i5++)
				{
					bool flag23 = i5 != 0;
					if (flag23)
					{
						log3 += ", ";
					}
					log3 += string.Format(" {0}: {1} ", LocalStringManager.Get(string.Format("LK_Neili_Allocation_Type_{0}", i5)), *(ref ret3.Items.FixedElementField + (IntPtr)i5 * 2));
				}
				CS$<>8__locals1.sb.AppendLine(log3);
				Inventory inventory = (Inventory)data[recevingParams[31].Item1];
				ItemKey[] inventoryKeys = inventory.Items.Keys.ToArray<ItemKey>();
				int i6 = 0;
				int len6 = inventoryKeys.Length;
				while (i6 < len6)
				{
					ItemKey key = inventoryKeys[i6];
					int amount = inventory.Items[key];
					bool flag24 = i6 != 0;
					if (flag24)
					{
						CS$<>8__locals1.sb.Append(", ");
					}
					else
					{
						CS$<>8__locals1.sb.AppendFormat("{0}: ", LocalStringManager.Get(LanguageKey.LK_Inventory));
					}
					CS$<>8__locals1.sb.Append(string.Format("[{0} x {1}]", ItemTemplateHelper.GetName(key.ItemType, key.TemplateId), amount));
					bool flag25 = i6 == len6 - 1;
					if (flag25)
					{
						CS$<>8__locals1.sb.AppendLine();
					}
					i6++;
				}
				CombatSkillEquipment combatSkillEquipment = (CombatSkillEquipment)data[recevingParams[34].Item1];
				CS$<>8__locals1.sb.AppendFormat("{0}: \n", LocalStringManager.Get(LanguageKey.LK_Equip_Main_Neigong_Title));
				for (sbyte equipType = 0; equipType < 5; equipType += 1)
				{
					CS$<>8__locals1.sb.AppendFormat("[{0}]\n", LocalStringManager.Get(string.Format("LK_CombatSkill_EquipType_{0}", equipType)));
					for (int i7 = 0; i7 < combatSkillEquipment[equipType].Count; i7++)
					{
						bool flag26 = i7 != 0;
						if (flag26)
						{
							CS$<>8__locals1.sb.Append(", ");
						}
						short combatSkillId = *combatSkillEquipment[equipType][i7];
						CS$<>8__locals1.sb.Append((combatSkillId >= 0) ? CombatSkill.Instance[combatSkillId].Name : LocalStringManager.Get(LanguageKey.LK_None));
					}
					CS$<>8__locals1.sb.AppendLine();
				}
				CS$<>8__locals1.sb.Append(CS$<>8__locals1.<>4__this._masteredCombatSkillString);
				int exp = (int)data[recevingParams[35].Item1];
				CS$<>8__locals1.sb.AppendLine(string.Format("{0}: {1}", LocalStringManager.Get(LanguageKey.LK_Exp), exp));
				sbyte idealSect = (sbyte)data[recevingParams[36].Item1];
				sbyte lifeSkillTypeInterest = (sbyte)data[recevingParams[37].Item1];
				sbyte combatSkillTypeInterest = (sbyte)data[recevingParams[38].Item1];
				sbyte mainAttributeInterest = (sbyte)data[recevingParams[39].Item1];
				CS$<>8__locals1.sb.AppendLine("idealSect: " + ((idealSect >= 0) ? Organization.Instance[idealSect].Name : LocalStringManager.Get(LanguageKey.LK_None)));
				CS$<>8__locals1.sb.AppendLine("lifeSkillTypeInterest: " + ((lifeSkillTypeInterest >= 0) ? Config.LifeSkillType.Instance[lifeSkillTypeInterest].Name : LocalStringManager.Get(LanguageKey.LK_None)));
				CS$<>8__locals1.sb.AppendLine("combatSkillTypeInterest: " + ((combatSkillTypeInterest >= 0) ? CombatSkillType.Instance[combatSkillTypeInterest].Name : LocalStringManager.Get(LanguageKey.LK_None)));
				CS$<>8__locals1.sb.AppendLine(string.Format("{0}: {1}", "mainAttributeInterest", mainAttributeInterest));
				CS$<>8__locals1.sb.AppendLine();
				CS$<>8__locals1.sb.AppendLine(string.Format("{0}: {1}", LocalStringManager.Get("GM_FieldName_PhysiologicalAge"), (short)data[recevingParams[dataFieldIds.Length].Item1]));
				sbyte fame = (sbyte)data[recevingParams[dataFieldIds.Length + 1].Item1];
				CS$<>8__locals1.sb.Append(LocalStringManager.Get("LK_Main_SummaryInfo_Fame"));
				CS$<>8__locals1.sb.AppendLine(string.Format(": {0}({1})", fame, CommonUtils.GetFameString(FameType.GetFameType(fame))));
				short morality2 = (short)data[recevingParams[dataFieldIds.Length + 2].Item1];
				CS$<>8__locals1.sb.Append(LocalStringManager.Get("LK_Main_SummaryInfo_Behavior"));
				CS$<>8__locals1.sb.AppendLine(string.Format(": {0}({1}) ", morality2, CommonUtils.GetBehaviorString(GameData.Domains.Character.BehaviorType.GetBehaviorType(morality2))));
				short attraction = (short)data[recevingParams[dataFieldIds.Length + 3].Item1];
				sbyte gender2 = (sbyte)data[recevingParams[2].Item1];
				short age = (short)data[recevingParams[dataFieldIds.Length].Item1];
				CS$<>8__locals1.sb.Append(LocalStringManager.Get("LK_Main_SummaryInfo_Charm"));
				CS$<>8__locals1.sb.AppendLine(string.Format(": {0}({1})", attraction, CommonUtils.GetCharmLevelText(attraction, gender2, age, short.MaxValue, false, true)));
				HitOrAvoidInts hits = (HitOrAvoidInts)data[recevingParams[dataFieldIds.Length + 4].Item1];
				OuterAndInnerInts hitPenne = (OuterAndInnerInts)data[recevingParams[dataFieldIds.Length + 5].Item1];
				HitOrAvoidInts avoids = (HitOrAvoidInts)data[recevingParams[dataFieldIds.Length + 6].Item1];
				OuterAndInnerInts avoidPenne = (OuterAndInnerInts)data[recevingParams[dataFieldIds.Length + 7].Item1];
				OuterAndInnerShorts recovery = (OuterAndInnerShorts)data[recevingParams[dataFieldIds.Length + 8].Item1];
				for (int i8 = 0; i8 < 4; i8++)
				{
					bool flag27 = i8 != 0;
					if (flag27)
					{
						CS$<>8__locals1.sb.Append(", ");
					}
					else
					{
						CS$<>8__locals1.sb.Append(LocalStringManager.Get(LanguageKey.LK_WeaponHitRate) + "/" + LocalStringManager.Get(LanguageKey.LK_AvoidType_2) + ": ");
					}
					CS$<>8__locals1.sb.Append(string.Format("{0} {1}/{2} {3}", new object[]
					{
						LocalStringManager.Get(string.Format("LK_HitType_{0}", i8)),
						*(ref hits.Items.FixedElementField + (IntPtr)i8 * 4),
						LocalStringManager.Get(string.Format("LK_AvoidType_{0}", i8)),
						*(ref avoids.Items.FixedElementField + (IntPtr)i8 * 4)
					}));
					bool flag28 = i8 == 3;
					if (flag28)
					{
						CS$<>8__locals1.sb.AppendLine();
					}
				}
				CS$<>8__locals1.sb.AppendLine(string.Format("{0}: {1} {2}: {3}", new object[]
				{
					LocalStringManager.Get(LanguageKey.LK_Penetrate_Outer),
					hitPenne.Outer,
					LocalStringManager.Get(LanguageKey.LK_Penetrate_Inner),
					hitPenne.Inner
				}));
				CS$<>8__locals1.sb.AppendLine(string.Format("{0}: {1} {2}: {3}", new object[]
				{
					LocalStringManager.Get(LanguageKey.LK_Penetrate_Resist_Outer),
					avoidPenne.Outer,
					LocalStringManager.Get(LanguageKey.LK_Penetrate_Resist_Inner),
					avoidPenne.Inner
				}));
				CS$<>8__locals1.sb.AppendLine(string.Format("{0}: {1} {2}: {3}", new object[]
				{
					LocalStringManager.Get(LanguageKey.LK_CombatSkill_Stance),
					recovery.Outer,
					LocalStringManager.Get(LanguageKey.LK_CombatSkill_Breath),
					recovery.Inner
				}));
				short moveSpeed = (short)data[recevingParams[dataFieldIds.Length + 9].Item1];
				short recoveryOfFlaw = (short)data[recevingParams[dataFieldIds.Length + 10].Item1];
				short castSpeed = (short)data[recevingParams[dataFieldIds.Length + 11].Item1];
				short recoveryOfBlockedAcupoint = (short)data[recevingParams[dataFieldIds.Length + 12].Item1];
				short weaponSwitchSpeed = (short)data[recevingParams[dataFieldIds.Length + 13].Item1];
				short attackSpeed = (short)data[recevingParams[dataFieldIds.Length + 14].Item1];
				short innerRatio = (short)data[recevingParams[dataFieldIds.Length + 15].Item1];
				short recoveryOfQiDisorder = (short)data[recevingParams[dataFieldIds.Length + 16].Item1];
				CS$<>8__locals1.sb.AppendLine(string.Format("{0}: {1} {2}: {3}", new object[]
				{
					LocalStringManager.Get(LanguageKey.LK_RecoveryOfMobility),
					moveSpeed,
					LocalStringManager.Get(LanguageKey.LK_CastSpeed),
					castSpeed
				}));
				CS$<>8__locals1.sb.AppendLine(string.Format("{0}: {1} {2}: {3}", new object[]
				{
					LocalStringManager.Get(LanguageKey.LK_RecoveryOfFlaw),
					recoveryOfFlaw,
					LocalStringManager.Get(LanguageKey.LK_RecoveryOfBlockedAcupoint),
					recoveryOfBlockedAcupoint
				}));
				CS$<>8__locals1.sb.AppendLine(string.Format("{0}: {1} {2}: {3}", new object[]
				{
					LocalStringManager.Get(LanguageKey.LK_WeaponSwitchSpeed),
					weaponSwitchSpeed,
					LocalStringManager.Get(LanguageKey.LK_AttackSpeed),
					attackSpeed
				}));
				CS$<>8__locals1.sb.AppendLine(string.Format("{0}: {1} {2}: {3}", new object[]
				{
					LocalStringManager.Get(LanguageKey.LK_InnerRatio),
					innerRatio,
					LocalStringManager.Get(LanguageKey.LK_RecoveryOfQiDisorder),
					recoveryOfQiDisorder
				}));
				PoisonInts poisonResists = (PoisonInts)data[recevingParams[dataFieldIds.Length + 17].Item1];
				short fertility = (short)data[recevingParams[dataFieldIds.Length + 18].Item1];
				CS$<>8__locals1.sb.AppendLine(string.Format("{0}: {1}", LocalStringManager.Get(LanguageKey.LK_Main_Attribute_Birth), fertility));
				for (sbyte order2 = 0; order2 < 6; order2 += 1)
				{
					sbyte type2 = PoisonType.GetTypeBySortingOrder(order2);
					bool flag29 = type2 != 0;
					if (flag29)
					{
						CS$<>8__locals1.sb.Append(", ");
					}
					CS$<>8__locals1.sb.AppendFormat("{0}{2}[{1}]", Poison.Instance[type2].Name, *(ref poisonResists.Items.FixedElementField + (IntPtr)type2 * 4), LocalStringManager.Get(LanguageKey.LK_Kidnap_Resistance_Value));
					bool flag30 = type2 == 5;
					if (flag30)
					{
						CS$<>8__locals1.sb.AppendLine();
					}
				}
				LifeSkillShorts lifeSkillAttainments = (LifeSkillShorts)data[recevingParams[dataFieldIds.Length + 19].Item1];
				CombatSkillShorts combatSkillAttainments = (CombatSkillShorts)data[recevingParams[dataFieldIds.Length + 20].Item1];
				LifeSkillShorts lifeSkillQualifications = (LifeSkillShorts)data[recevingParams[dataFieldIds.Length + 21].Item1];
				CombatSkillShorts combatSkillQualifications = (CombatSkillShorts)data[recevingParams[dataFieldIds.Length + 22].Item1];
				int i9 = 0;
				int len7 = 16;
				while (i9 < len7)
				{
					bool flag31 = i9 != 0;
					if (flag31)
					{
						CS$<>8__locals1.sb.Append(", ");
					}
					else
					{
						CS$<>8__locals1.sb.Append(LocalStringManager.Get(LanguageKey.LK_Endowments_LifeSkill) + "/" + LocalStringManager.Get(LanguageKey.LK_Attainment) + ": ");
					}
					CS$<>8__locals1.sb.Append(string.Format("{0} {1}/{2}", Config.LifeSkillType.Instance[i9].Name, *(ref lifeSkillAttainments.Items.FixedElementField + (IntPtr)i9 * 2), *(ref lifeSkillQualifications.Items.FixedElementField + (IntPtr)i9 * 2)));
					bool flag32 = i9 == len7 - 1;
					if (flag32)
					{
						CS$<>8__locals1.sb.AppendLine();
					}
					i9++;
				}
				int i10 = 0;
				int len8 = 14;
				while (i10 < len8)
				{
					bool flag33 = i10 != 0;
					if (flag33)
					{
						CS$<>8__locals1.sb.Append(", ");
					}
					else
					{
						CS$<>8__locals1.sb.Append(LocalStringManager.Get(LanguageKey.LK_Endowments_CombateSkill) + "/" + LocalStringManager.Get(LanguageKey.LK_Attainment) + ": ");
					}
					CS$<>8__locals1.sb.Append(string.Format("{0} {1}/{2}", CombatSkillType.Instance[i10].Name, *(ref combatSkillAttainments.Items.FixedElementField + (IntPtr)i10 * 2), *(ref combatSkillQualifications.Items.FixedElementField + (IntPtr)i10 * 2)));
					bool flag34 = i10 == len8 - 1;
					if (flag34)
					{
						CS$<>8__locals1.sb.AppendLine();
					}
					i10++;
				}
				Personalities personalities = (Personalities)data[recevingParams[dataFieldIds.Length + 23].Item1];
				int i11 = 0;
				int len9 = 7;
				while (i11 < len9)
				{
					bool flag35 = i11 != 0;
					if (flag35)
					{
						CS$<>8__locals1.sb.Append(", ");
					}
					CS$<>8__locals1.sb.Append(string.Format("{0} {1}", Personality.Instance[i11].Name, *(ref personalities.Items.FixedElementField + i11)));
					bool flag36 = i11 == len9 - 1;
					if (flag36)
					{
						CS$<>8__locals1.sb.AppendLine();
					}
					i11++;
				}
				int maxInventoryLoad = (int)data[recevingParams[dataFieldIds.Length + 24].Item1];
				int inventoryLoad = (int)data[recevingParams[dataFieldIds.Length + 25].Item1];
				int maxEquipmentLoad = (int)data[recevingParams[dataFieldIds.Length + 26].Item1];
				int equipmentLoad = (int)data[recevingParams[dataFieldIds.Length + 27].Item1];
				CS$<>8__locals1.sb.AppendLine(string.Format("{0} {1}/{2}", LocalStringManager.Get(LanguageKey.LK_Inventory_Load_Title), inventoryLoad, maxInventoryLoad));
				CS$<>8__locals1.sb.AppendLine(string.Format("{0} {1}/{2}", LocalStringManager.Get(LanguageKey.LK_Equip_Load_Title), equipmentLoad, maxEquipmentLoad));
				int maxNeili = (int)data[recevingParams[dataFieldIds.Length + 28].Item1];
				sbyte neiliType = (sbyte)data[recevingParams[dataFieldIds.Length + 29].Item1];
				int combatPower = (int)data[recevingParams[dataFieldIds.Length + 30].Item1];
				short maxHealth = (short)data[recevingParams[dataFieldIds.Length + 31].Item1];
				CS$<>8__locals1.sb.AppendLine(NeiliType.Instance[neiliType].Name ?? "");
				CS$<>8__locals1.sb.AppendLine(string.Format("{0}: {1}", LocalStringManager.Get(LanguageKey.LK_HotKeyGroup_CombatSystem), combatPower));
				CS$<>8__locals1.<>4__this.InfoDisplay.text = CS$<>8__locals1.sb.ToString() + CS$<>8__locals1.<>4__this.InfoDisplay.text;
			}, recevingParams);
			CharacterDomainMethod.AsyncCall.GetCharacterDisplayDataList(null, new List<int>
			{
				this._characterId
			}, delegate(int offset, RawDataPool dataPool)
			{
				List<CharacterDisplayData> displays = null;
				Serializer.Deserialize(dataPool, offset, ref displays);
				CharacterDisplayData charData = displays[0];
				this.NameDisplay.text = (NameCenter.GetNameByDisplayData(charData, SingletonObject.getInstance<BasicGameData>().TaiwuCharId == this._characterId, true) ?? "");
				try
				{
					TextMeshProUGUI genderDisplay = this.GenderDisplay;
					genderDisplay.text = genderDisplay.text + "[" + CommonUtils.GetOrganizationGradeString(charData.OrgInfo, charData.Gender, charData.CurrAge, -1) + "]";
					Location location = charData.Location;
					TextMeshProUGUI nameDisplay = this.NameDisplay;
					nameDisplay.text = nameDisplay.text + "(" + MapArea.Instance[SingletonObject.getInstance<WorldMapModel>().Areas[(int)location.AreaId].GetTemplateId()].Name + ")";
					WorldMapModel mapModel = SingletonObject.getInstance<WorldMapModel>();
					ByteCoordinate blockPos = mapModel.GetBlockData(location).GetBlockPos();
					TextMeshProUGUI nameDisplay2 = this.NameDisplay;
					nameDisplay2.text += string.Format("({0}, {1})", blockPos.X, blockPos.Y);
				}
				catch (Exception)
				{
				}
			});
			CharacterDomainMethod.AsyncCall.GmCmd_GetCharacterPregnancyLockEndDates(null, this._characterId, delegate(int offset, RawDataPool dataPool)
			{
				int value = 0;
				Serializer.Deserialize(dataPool, offset, ref value);
				TextMeshProUGUI infoDisplay = this.InfoDisplay;
				infoDisplay.text += string.Format("{0}{1}: {2}\n", LocalStringManager.Get(LanguageKey.LK_Main_Attribute_Birth), LocalStringManager.Get(LanguageKey.LK_Disable), value);
			});
			CharacterDomainMethod.AsyncCall.GmCmd_GetCharacterActualBloodParents(null, this._characterId, delegate(int offset, RawDataPool dataPool)
			{
				List<int> charList = null;
				Serializer.Deserialize(dataPool, offset, ref charList);
				CS$<>8__locals1.sb.Clear();
				bool flag3 = charList != null;
				if (flag3)
				{
					CS$<>8__locals1.sb.Append(string.Format("{0}: {1}({2})", LocalStringManager.Get(LanguageKey.LK_Relation_BloodParent_Father), charList[0], charList[2]));
					CS$<>8__locals1.sb.Append(string.Format("{0}: {1}({2})", LocalStringManager.Get(LanguageKey.LK_Relation_BloodParent_Mother), charList[1], charList[3]));
					CS$<>8__locals1.sb.AppendLine();
				}
				TextMeshProUGUI infoDisplay = CS$<>8__locals1.<>4__this.InfoDisplay;
				infoDisplay.text += CS$<>8__locals1.sb.ToString();
			});
			CharacterDomainMethod.AsyncCall.GetCharacterCurrentProfession(null, this._characterId, delegate(int offset, RawDataPool pool)
			{
				ProfessionData professionData = null;
				Serializer.Deserialize(pool, offset, ref professionData);
				CS$<>8__locals1.sb.Clear();
				bool flag3 = professionData != null;
				if (flag3)
				{
					CS$<>8__locals1.sb.AppendFormat("{0}: {1} ({2})\n", LocalStringManager.Get(LanguageKey.LK_Profession), professionData.GetConfig().Name, professionData.Seniority);
					TextMeshProUGUI infoDisplay = CS$<>8__locals1.<>4__this.InfoDisplay;
					infoDisplay.text += CS$<>8__locals1.sb.ToString();
				}
			});
		}
	}

	// Token: 0x06002126 RID: 8486 RVA: 0x000F1C0C File Offset: 0x000EFE0C
	public void OnWorldDataReady()
	{
		this.OnLeaveWorld();
		this._gameDataListenerId = GameDataBridge.RegisterListener(new GameDataBridge.NotificationHandler(this.OnNotifyGameData));
	}

	// Token: 0x06002127 RID: 8487 RVA: 0x000F1C30 File Offset: 0x000EFE30
	public void OnLeaveWorld()
	{
		bool flag = this._gameDataListenerId != -1;
		if (flag)
		{
			GameDataBridge.UnregisterListener(this._gameDataListenerId);
			this._gameDataListenerId = -1;
		}
	}

	// Token: 0x06002128 RID: 8488 RVA: 0x000F1C63 File Offset: 0x000EFE63
	private void OnDestroy()
	{
		this.OnLeaveWorld();
	}

	// Token: 0x06002129 RID: 8489 RVA: 0x000F1C6D File Offset: 0x000EFE6D
	public void OnDisable()
	{
		this.SetCharacterId(-1);
	}

	// Token: 0x0600212A RID: 8490 RVA: 0x000F1C78 File Offset: 0x000EFE78
	public void OnClose()
	{
		base.gameObject.SetActive(false);
	}

	// Token: 0x0600212B RID: 8491 RVA: 0x000F1C88 File Offset: 0x000EFE88
	public void OnRefresh()
	{
		this.SetCharacterId(this._characterId);
	}

	// Token: 0x0600212C RID: 8492 RVA: 0x000F1C98 File Offset: 0x000EFE98
	private void OnNotifyGameData(List<NotificationWrapper> notifications)
	{
		foreach (NotificationWrapper wrapper in notifications)
		{
			Notification notification = wrapper.Notification;
			byte type = notification.Type;
			byte b = type;
			if (b == 0)
			{
				DataUid uid = notification.Uid;
			}
		}
	}

	// Token: 0x0600212E RID: 8494 RVA: 0x000F1D7D File Offset: 0x000EFF7D
	[CompilerGenerated]
	private DataUid <SetCharacterId>g__CharacterFieldUid|11_5(ushort field)
	{
		return new DataUid(4, 0, (ulong)((long)this._characterId), (uint)field);
	}

	// Token: 0x040019A0 RID: 6560
	public TextMeshProUGUI NameDisplay;

	// Token: 0x040019A1 RID: 6561
	public TextMeshProUGUI GenderDisplay;

	// Token: 0x040019A2 RID: 6562
	public TextMeshProUGUI InfoDisplay;

	// Token: 0x040019A3 RID: 6563
	public Transform List;

	// Token: 0x040019A4 RID: 6564
	private int _gameDataListenerId = -1;

	// Token: 0x040019A5 RID: 6565
	private int _characterId = -1;

	// Token: 0x040019A6 RID: 6566
	private string _masteredCombatSkillString;

	// Token: 0x040019A7 RID: 6567
	private readonly List<LanguageKey> BasicAttributeLanguageKey = new List<LanguageKey>
	{
		LanguageKey.LK_Main_Attribute_Strength,
		LanguageKey.LK_Main_Attribute_Dexterity,
		LanguageKey.LK_Main_Attribute_Concentration,
		LanguageKey.LK_Main_Attribute_Vitality,
		LanguageKey.LK_Main_Attribute_Energy,
		LanguageKey.LK_Main_Attribute_Intelligence
	};
}
