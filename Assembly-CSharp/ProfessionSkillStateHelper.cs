using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Config;
using FrameWork;
using GameData.Domains.Character;
using GameData.Domains.Extra;
using GameData.Domains.Item;
using GameData.Domains.Item.Display;
using GameData.Domains.Map;
using GameData.Domains.Taiwu;
using GameData.Domains.Taiwu.Profession;
using GameData.Serializer;
using GameData.Utilities;

// Token: 0x020002F4 RID: 756
public static class ProfessionSkillStateHelper
{
	// Token: 0x06002C33 RID: 11315 RVA: 0x00159ED8 File Offset: 0x001580D8
	public static void AsyncGetSkillUseState(IAsyncMethodRequestHandler requestHandler, int skillId, ResourceInts resources, int exp, ProfessionSkillStateHelper.OnGetResult onGetResult)
	{
		ProfessionSkillStateHelper.<>c__DisplayClass2_0 CS$<>8__locals1 = new ProfessionSkillStateHelper.<>c__DisplayClass2_0();
		CS$<>8__locals1.resources = resources;
		CS$<>8__locals1.exp = exp;
		CS$<>8__locals1.onGetResult = onGetResult;
		CS$<>8__locals1.skillConfig = ProfessionSkill.Instance[skillId];
		CS$<>8__locals1.extraLineStringBuilder = EasyPool.Get<StringBuilder>();
		CS$<>8__locals1.extraLineStringBuilder.Clear();
		CS$<>8__locals1.lackLineStringBuilder = EasyPool.Get<StringBuilder>();
		CS$<>8__locals1.lackLineStringBuilder.Clear();
		bool flag = CS$<>8__locals1.skillConfig.Type == EProfessionSkillType.Passive;
		if (flag)
		{
			CS$<>8__locals1.onGetResult(new ProfessionSkillStateHelper.Result
			{
				CanUse = false,
				ExtraLineStringBuilder = CS$<>8__locals1.extraLineStringBuilder,
				LackLineStringBuilder = CS$<>8__locals1.lackLineStringBuilder
			});
		}
		else
		{
			ProfessionModel model = SingletonObject.getInstance<ProfessionModel>();
			ValueTuple<ProfessionData, int> valueTuple = model.FindProfessionDataBySkillId(skillId);
			CS$<>8__locals1.professionData = valueTuple.Item1;
			CS$<>8__locals1.skillIndex = valueTuple.Item2;
			MapBlockData currentBlockData = SingletonObject.getInstance<WorldMapModel>().CurrentBlockData;
			CS$<>8__locals1.isSettlementBlock = (currentBlockData != null && WorldMapModel.IsSettlementBlock(currentBlockData.GetConfig()));
			CS$<>8__locals1.canUse = true;
			if (skillId <= 31)
			{
				switch (skillId)
				{
				case 1:
					ExtraDomainMethod.AsyncCall.CheckSpecialCondition_SavageSkill_1(requestHandler, CS$<>8__locals1.professionData, delegate(int offset, RawDataPool dataPool)
					{
						bool check = false;
						Serializer.Deserialize(dataPool, offset, ref check);
						bool flag3 = !check;
						if (flag3)
						{
							CS$<>8__locals1.extraLineStringBuilder.AppendLine(LocalStringManager.Get(LanguageKey.LK_ProfessionSkill_SavageSkill1_WrongBlock).ColorReplace());
							CS$<>8__locals1.canUse = false;
						}
						base.<AsyncGetSkillUseState>g__Finish|0();
					});
					goto IL_3A0;
				case 2:
				case 4:
				case 5:
					break;
				case 3:
					ExtraDomainMethod.AsyncCall.FindTreasureExpect(requestHandler, SingletonObject.getInstance<WorldMapModel>().CurrentLocation, delegate(int offsetData, RawDataPool poolData)
					{
						TreasureExpectResult expectResult = default(TreasureExpectResult);
						Serializer.Deserialize(poolData, offsetData, ref expectResult);
						bool flag3 = !expectResult.AnyNormalItem;
						if (flag3)
						{
							CS$<>8__locals1.extraLineStringBuilder.AppendLine(LocalStringManager.Get(LanguageKey.LK_ProfessionSkill_SavageSkill3_NoTreasure).ColorReplace());
							CS$<>8__locals1.canUse = false;
						}
						base.<AsyncGetSkillUseState>g__Finish|0();
					});
					goto IL_3A0;
				case 6:
					ExtraDomainMethod.AsyncCall.CheckSpecialCondition_HunterSkill2(null, delegate(int offset, RawDataPool dataPool)
					{
						bool conditionIsMeet = false;
						Serializer.Deserialize(dataPool, offset, ref conditionIsMeet);
						bool flag3 = !conditionIsMeet;
						if (flag3)
						{
							CS$<>8__locals1.extraLineStringBuilder.AppendLine(LocalStringManager.Get(LanguageKey.LK_ProfessionSkill_HunterSkill2_WrongBlock).ColorReplace());
							CS$<>8__locals1.canUse = false;
						}
						base.<AsyncGetSkillUseState>g__Finish|0();
					});
					goto IL_3A0;
				case 7:
				{
					CharacterMonitorModel cmodel = SingletonObject.getInstance<CharacterMonitorModel>();
					List<int> team = cmodel.GetTaiwuTeamCharIds();
					int maxCount = cmodel.GetTaiwuGroupMaxCount();
					bool flag2 = maxCount <= team.Count - 1;
					if (flag2)
					{
						CS$<>8__locals1.extraLineStringBuilder.AppendLine(LocalStringManager.Get(LanguageKey.LK_HunterSkill3_Disable_TeamIsFull).ColorReplace());
					}
					TaiwuDomainMethod.AsyncCall.GetAllItems(requestHandler, ItemSourceType.Inventory, false, delegate(int offset, RawDataPool pool)
					{
						ValueTuple<ItemSourceType, List<ItemDisplayData>> tuple = default(ValueTuple<ItemSourceType, List<ItemDisplayData>>);
						Serializer.Deserialize(pool, offset, ref tuple);
						bool hasItem = tuple.Item2.Any(delegate(ItemDisplayData d)
						{
							short subType = ItemTemplateHelper.GetItemSubType(d.Key.ItemType, d.Key.TemplateId);
							return d.Key.ItemType == 4 && subType == 402;
						});
						bool flag3 = !hasItem;
						if (flag3)
						{
							CS$<>8__locals1.extraLineStringBuilder.AppendLine(LocalStringManager.Get(LanguageKey.LK_HunterSkill3_Disable_NoTarget).ColorReplace());
							CS$<>8__locals1.canUse = false;
						}
						base.<AsyncGetSkillUseState>g__Finish|0();
					});
					goto IL_3A0;
				}
				default:
					switch (skillId)
					{
					case 13:
					{
						ProfessionData martialArtistProfessionData = SingletonObject.getInstance<ProfessionModel>().GetProfessionData(3);
						ExtraDomainMethod.AsyncCall.CheckSpecialCondition(null, martialArtistProfessionData.TemplateId, 1, delegate(int offset, RawDataPool dataPool)
						{
							bool conditionIsMeet = false;
							Serializer.Deserialize(dataPool, offset, ref conditionIsMeet);
							bool flag3 = !conditionIsMeet;
							if (flag3)
							{
								bool flag4 = !CS$<>8__locals1.isSettlementBlock;
								if (flag4)
								{
									CS$<>8__locals1.extraLineStringBuilder.AppendLine(LocalStringManager.Get(LanguageKey.LK_ProfessionSkill_ProfessionSkill_WrongBlock).ColorReplace());
								}
								else
								{
									CS$<>8__locals1.extraLineStringBuilder.AppendLine(LocalStringManager.Get(LanguageKey.LK_ProfessionSkill_MartialArtistSkill1_NoNeed).ColorReplace());
								}
								CS$<>8__locals1.canUse = false;
							}
							base.<AsyncGetSkillUseState>g__Finish|0();
						});
						goto IL_3A0;
					}
					case 14:
						ExtraDomainMethod.AsyncCall.CheckSpecialCondition(null, 3, 2, delegate(int offset, RawDataPool dataPool)
						{
							bool conditionIsMeet = false;
							Serializer.Deserialize(dataPool, offset, ref conditionIsMeet);
							bool flag3 = !conditionIsMeet;
							if (flag3)
							{
								bool flag4 = !CS$<>8__locals1.isSettlementBlock;
								if (flag4)
								{
									CS$<>8__locals1.extraLineStringBuilder.AppendLine(LocalStringManager.Get(LanguageKey.LK_ProfessionSkill_ProfessionSkill_WrongBlock).ColorReplace());
								}
								else
								{
									CS$<>8__locals1.extraLineStringBuilder.AppendLine(LocalStringManager.GetFormat(LanguageKey.LK_ProfessionSkill_MartialArtistSkill2_NoMeet, 25).ColorReplace());
								}
								CS$<>8__locals1.canUse = false;
							}
							base.<AsyncGetSkillUseState>g__Finish|0();
						});
						goto IL_3A0;
					case 15:
					case 16:
						break;
					case 17:
					{
						ProfessionData literatiProfessionData = SingletonObject.getInstance<ProfessionModel>().GetProfessionData(4);
						ExtraDomainMethod.AsyncCall.CheckSpecialCondition(null, literatiProfessionData.TemplateId, 1, delegate(int offset, RawDataPool dataPool)
						{
							bool conditionIsMeet = false;
							Serializer.Deserialize(dataPool, offset, ref conditionIsMeet);
							bool flag3 = !conditionIsMeet;
							if (flag3)
							{
								bool flag4 = !CS$<>8__locals1.isSettlementBlock;
								if (flag4)
								{
									CS$<>8__locals1.extraLineStringBuilder.AppendLine(LocalStringManager.Get(LanguageKey.LK_ProfessionSkill_ProfessionSkill_WrongBlock).ColorReplace());
								}
								else
								{
									CS$<>8__locals1.extraLineStringBuilder.AppendLine(LocalStringManager.Get(LanguageKey.LK_ProfessionSkill_LiteratiSkill1_NoNeed).ColorReplace());
								}
								CS$<>8__locals1.canUse = false;
							}
							base.<AsyncGetSkillUseState>g__Finish|0();
						});
						goto IL_3A0;
					}
					case 18:
						ExtraDomainMethod.AsyncCall.CheckSpecialCondition(null, 4, 2, delegate(int offset, RawDataPool dataPool)
						{
							bool conditionIsMeet = false;
							Serializer.Deserialize(dataPool, offset, ref conditionIsMeet);
							bool flag3 = !conditionIsMeet;
							if (flag3)
							{
								CS$<>8__locals1.extraLineStringBuilder.AppendLine(LocalStringManager.Get(LanguageKey.LK_ProfessionSkill_LiteratiSkill2_NoMeet).ColorReplace());
								CS$<>8__locals1.canUse = false;
							}
							base.<AsyncGetSkillUseState>g__Finish|0();
						});
						goto IL_3A0;
					default:
						if (skillId == 31)
						{
							ExtraDomainMethod.AsyncCall.CheckTasterUltimateSpecialCondition(null, true, delegate(int offset, RawDataPool dataPool)
							{
								int condition = 0;
								Serializer.Deserialize(dataPool, offset, ref condition);
								bool flag3 = ProfessionSkillInvalidType.Contains(condition, 1);
								if (flag3)
								{
									CS$<>8__locals1.extraLineStringBuilder.AppendLine(LocalStringManager.Get(LanguageKey.LK_ProfessionSkill_ProfessionSkill_WrongBlock).ColorReplace());
									CS$<>8__locals1.canUse = false;
								}
								else
								{
									bool flag4 = ProfessionSkillInvalidType.Contains(condition, 2);
									if (flag4)
									{
										CS$<>8__locals1.extraLineStringBuilder.AppendLine(LocalStringManager.Get(LanguageKey.LK_ProfessionTips_Taster_NoCharacter).ColorReplace());
										CS$<>8__locals1.canUse = false;
									}
								}
								bool flag5 = ProfessionSkillInvalidType.Contains(condition, 4);
								if (flag5)
								{
									CS$<>8__locals1.extraLineStringBuilder.AppendLine(LocalStringManager.Get(LanguageKey.LK_ProfessionTips_Taster_NoBook).ColorReplace());
									CS$<>8__locals1.canUse = false;
								}
								base.<AsyncGetSkillUseState>g__Finish|0();
							});
							goto IL_3A0;
						}
						break;
					}
					break;
				}
			}
			else
			{
				switch (skillId)
				{
				case 35:
					ExtraDomainMethod.AsyncCall.CheckAristocratUltimateSpecialCondition(null, delegate(int offset, RawDataPool dataPool)
					{
						int condition = 0;
						Serializer.Deserialize(dataPool, offset, ref condition);
						bool flag3 = ProfessionSkillInvalidType.Contains(condition, 1);
						if (flag3)
						{
							CS$<>8__locals1.extraLineStringBuilder.AppendLine(LocalStringManager.Get(LanguageKey.LK_ProfessionSkill_ProfessionSkill_WrongBlock).ColorReplace());
							CS$<>8__locals1.canUse = false;
						}
						bool flag4 = ProfessionSkillInvalidType.Contains(condition, 8);
						if (flag4)
						{
							CS$<>8__locals1.extraLineStringBuilder.AppendLine(LocalStringManager.Get(LanguageKey.LK_ProfessionTips_Aristocrat_NoPrisoner).ColorReplace());
							CS$<>8__locals1.canUse = false;
						}
						base.<AsyncGetSkillUseState>g__Finish|0();
					});
					goto IL_3A0;
				case 36:
					ExtraDomainMethod.AsyncCall.CheckSpecialCondition(null, 9, 0, delegate(int offset, RawDataPool dataPool)
					{
						bool conditionIsMeet = false;
						Serializer.Deserialize(dataPool, offset, ref conditionIsMeet);
						bool flag3 = !conditionIsMeet;
						if (flag3)
						{
							CS$<>8__locals1.extraLineStringBuilder.AppendLine(LocalStringManager.Get(LanguageKey.LK_ProfessionSkill_BeggarSkill0_IsNotMeet).ColorReplace());
							CS$<>8__locals1.canUse = false;
						}
						base.<AsyncGetSkillUseState>g__Finish|0();
					});
					goto IL_3A0;
				case 37:
				case 38:
				case 40:
					break;
				case 39:
					ExtraDomainMethod.AsyncCall.CheckBeggarUltimateSpecialCondition(null, delegate(int offset, RawDataPool dataPool)
					{
						int condition = 0;
						Serializer.Deserialize(dataPool, offset, ref condition);
						bool flag3 = ProfessionSkillInvalidType.Contains(condition, 1);
						if (flag3)
						{
							CS$<>8__locals1.extraLineStringBuilder.AppendLine(LocalStringManager.Get(LanguageKey.LK_ProfessionTips_Beggar_NoCharacter).ColorReplace());
							CS$<>8__locals1.canUse = false;
						}
						bool flag4 = ProfessionSkillInvalidType.Contains(condition, 16);
						if (flag4)
						{
							CS$<>8__locals1.extraLineStringBuilder.AppendLine(LocalStringManager.Get(LanguageKey.LK_ProfessionTips_Beggar_NoEmptyEatingSlot).ColorReplace());
							CS$<>8__locals1.canUse = false;
						}
						base.<AsyncGetSkillUseState>g__Finish|0();
					});
					goto IL_3A0;
				case 41:
					ExtraDomainMethod.AsyncCall.CheckSpecialCondition(null, 10, 1, delegate(int offset, RawDataPool dataPool)
					{
						bool conditionIsMeet = false;
						Serializer.Deserialize(dataPool, offset, ref conditionIsMeet);
						bool flag3 = !conditionIsMeet;
						if (flag3)
						{
							CS$<>8__locals1.extraLineStringBuilder.AppendLine(LocalStringManager.Get(LanguageKey.LK_ProfessionSkill_CivilianSkill1_NoNeed).ColorReplace());
							CS$<>8__locals1.canUse = false;
						}
						base.<AsyncGetSkillUseState>g__Finish|0();
					});
					goto IL_3A0;
				default:
					if (skillId == 53)
					{
						ExtraDomainMethod.AsyncCall.CheckSpecialCondition(null, 13, 1, delegate(int offset, RawDataPool dataPool)
						{
							bool conditionIsMeet = false;
							Serializer.Deserialize(dataPool, offset, ref conditionIsMeet);
							bool flag3 = !conditionIsMeet;
							if (flag3)
							{
								CS$<>8__locals1.extraLineStringBuilder.AppendLine(LocalStringManager.Get(LanguageKey.LK_ProfessionSkill_DoctorSkill0_IsNotMeet).ColorReplace());
								CS$<>8__locals1.canUse = false;
							}
							base.<AsyncGetSkillUseState>g__Finish|0();
						});
						goto IL_3A0;
					}
					if (skillId == 67)
					{
						ExtraDomainMethod.AsyncCall.CheckTasterUltimateSpecialCondition(null, false, delegate(int offset, RawDataPool dataPool)
						{
							int condition = 0;
							Serializer.Deserialize(dataPool, offset, ref condition);
							bool flag3 = ProfessionSkillInvalidType.Contains(condition, 1);
							if (flag3)
							{
								CS$<>8__locals1.extraLineStringBuilder.AppendLine(LocalStringManager.Get(LanguageKey.LK_ProfessionSkill_ProfessionSkill_WrongBlock).ColorReplace());
								CS$<>8__locals1.canUse = false;
							}
							else
							{
								bool flag4 = ProfessionSkillInvalidType.Contains(condition, 2);
								if (flag4)
								{
									CS$<>8__locals1.extraLineStringBuilder.AppendLine(LocalStringManager.Get(LanguageKey.LK_ProfessionTips_Taster_NoCharacter).ColorReplace());
									CS$<>8__locals1.canUse = false;
								}
							}
							bool flag5 = ProfessionSkillInvalidType.Contains(condition, 4);
							if (flag5)
							{
								CS$<>8__locals1.extraLineStringBuilder.AppendLine(LocalStringManager.Get(LanguageKey.LK_ProfessionTips_Taster_NoBook).ColorReplace());
								CS$<>8__locals1.canUse = false;
							}
							base.<AsyncGetSkillUseState>g__Finish|0();
						});
						goto IL_3A0;
					}
					break;
				}
			}
			CS$<>8__locals1.<AsyncGetSkillUseState>g__Finish|0();
			IL_3A0:;
		}
	}

	// Token: 0x02001647 RID: 5703
	public struct Result
	{
		// Token: 0x0400A76E RID: 42862
		public bool CanUse;

		// Token: 0x0400A76F RID: 42863
		public StringBuilder ExtraLineStringBuilder;

		// Token: 0x0400A770 RID: 42864
		public StringBuilder LackLineStringBuilder;
	}

	// Token: 0x02001648 RID: 5704
	// (Invoke) Token: 0x0600D15F RID: 53599
	public delegate void OnGetResult(ProfessionSkillStateHelper.Result result);
}
