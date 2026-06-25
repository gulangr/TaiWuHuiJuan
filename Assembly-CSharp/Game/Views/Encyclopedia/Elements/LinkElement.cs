using System;
using System.Collections.Generic;
using Config;
using FrameWork;
using Game.Views.Encyclopedia.Event;
using Game.Views.NewGame;
using GameData.Domains.Item.Display;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Game.Views.Encyclopedia.Elements
{
	// Token: 0x02000A8A RID: 2698
	[RequireComponent(typeof(TextMeshProUGUI))]
	public class LinkElement : Element, IPointerClickHandler, IEventSystemHandler, IPointerEnterHandler, IPointerMoveHandler, IPointerExitHandler
	{
		// Token: 0x0600841E RID: 33822 RVA: 0x003D61D6 File Offset: 0x003D43D6
		private void Awake()
		{
			this._linkText = base.GetComponent<TextMeshProUGUI>();
			this._mCamera = base.GetComponentInParent<Canvas>().worldCamera;
		}

		// Token: 0x0600841F RID: 33823 RVA: 0x003D61F8 File Offset: 0x003D43F8
		public void OnPointerClick(PointerEventData eventData)
		{
			int linkIndex = this.GetLinkIndexByEventData(eventData.position);
			bool flag = linkIndex <= -1;
			if (flag)
			{
				this.HideTips();
			}
			else
			{
				EncyclopediaReferenceItem cfg = EncyclopediaReference.Instance[this._linkText.textInfo.linkInfo[linkIndex].GetLinkID()];
				bool flag2 = cfg != null && cfg.InsertType == EEncyclopediaReferenceInsertType.HyperLink;
				if (flag2)
				{
					EventManager.Instance.Dispatch(1, EventArgs<string>.CreateEventArgs(cfg.Param));
				}
			}
		}

		// Token: 0x06008420 RID: 33824 RVA: 0x003D6279 File Offset: 0x003D4479
		public void OnPointerEnter(PointerEventData eventData)
		{
			this._previousKey = -1;
			this._counter = 2;
		}

		// Token: 0x06008421 RID: 33825 RVA: 0x003D628A File Offset: 0x003D448A
		public void OnPointerExit(PointerEventData eventData)
		{
			this._previousKey = 0;
			this.HideTips();
		}

		// Token: 0x06008422 RID: 33826 RVA: 0x003D629B File Offset: 0x003D449B
		public void OnPointerMove(PointerEventData eventData)
		{
			this.ShowOrHideTip(eventData.position);
		}

		// Token: 0x06008423 RID: 33827 RVA: 0x003D62AC File Offset: 0x003D44AC
		public static TipType Ref2Tip(EEncyclopediaReferenceInsertType referenceInsertType)
		{
			if (!true)
			{
			}
			TipType result;
			switch (referenceInsertType)
			{
			case EEncyclopediaReferenceInsertType.Invalid:
			case EEncyclopediaReferenceInsertType.ConfigTable:
			case EEncyclopediaReferenceInsertType.Figure:
			case EEncyclopediaReferenceInsertType.Equation:
			case EEncyclopediaReferenceInsertType.Count:
				result = TipType.Encyclopedia;
				goto IL_140;
			case EEncyclopediaReferenceInsertType.HyperLink:
				result = TipType.Simple;
				goto IL_140;
			case EEncyclopediaReferenceInsertType.FeatureTips:
				result = TipType.Feature;
				goto IL_140;
			case EEncyclopediaReferenceInsertType.WeaponTips:
				result = TipType.Weapon;
				goto IL_140;
			case EEncyclopediaReferenceInsertType.MiscTips:
				result = TipType.Misc;
				goto IL_140;
			case EEncyclopediaReferenceInsertType.AccessoryTips:
				result = TipType.Accessory;
				goto IL_140;
			case EEncyclopediaReferenceInsertType.ArmorTips:
				result = TipType.Armor;
				goto IL_140;
			case EEncyclopediaReferenceInsertType.CarrierTips:
				result = TipType.Carrier;
				goto IL_140;
			case EEncyclopediaReferenceInsertType.ClothingTips:
				result = TipType.Clothing;
				goto IL_140;
			case EEncyclopediaReferenceInsertType.CraftToolTips:
				result = TipType.CraftTool;
				goto IL_140;
			case EEncyclopediaReferenceInsertType.CricketTips:
				result = TipType.Cricket;
				goto IL_140;
			case EEncyclopediaReferenceInsertType.FoodTips:
				result = TipType.Food;
				goto IL_140;
			case EEncyclopediaReferenceInsertType.MaterialTips:
				result = TipType.Material;
				goto IL_140;
			case EEncyclopediaReferenceInsertType.MedicineTips:
				result = TipType.Medicine;
				goto IL_140;
			case EEncyclopediaReferenceInsertType.TeaWineTips:
				result = TipType.TeaWine;
				goto IL_140;
			case EEncyclopediaReferenceInsertType.SkillBookTips:
				result = TipType.SkillBook;
				goto IL_140;
			case EEncyclopediaReferenceInsertType.CombatSkillTips:
				result = TipType.CombatSkill;
				goto IL_140;
			case EEncyclopediaReferenceInsertType.BuildingBlockTips:
				result = TipType.Simple;
				goto IL_140;
			case EEncyclopediaReferenceInsertType.ConsummateLevelTips:
				result = TipType.Simple;
				goto IL_140;
			case EEncyclopediaReferenceInsertType.DebateStrategyTips:
				result = TipType.LifeSkillCombatStrategy;
				goto IL_140;
			case EEncyclopediaReferenceInsertType.NeiliTypeTips:
				result = TipType.FiveElements;
				goto IL_140;
			case EEncyclopediaReferenceInsertType.ProtagonistFeatureTips:
				result = TipType.Simple;
				goto IL_140;
			case EEncyclopediaReferenceInsertType.WorldStateTips:
				result = TipType.Simple;
				goto IL_140;
			case EEncyclopediaReferenceInsertType.LifeSkillTips:
				result = TipType.LifeSkillDetailReadProgress;
				goto IL_140;
			case EEncyclopediaReferenceInsertType.TrickTypeTips:
				result = TipType.TrickType;
				goto IL_140;
			case EEncyclopediaReferenceInsertType.ProfessionTips:
				result = TipType.Profession;
				goto IL_140;
			case EEncyclopediaReferenceInsertType.ProfessionSkillTips:
				result = TipType.ProfessionSkill;
				goto IL_140;
			}
			throw new Exception(string.Format("Not Supported: {0}", referenceInsertType));
			IL_140:
			if (!true)
			{
			}
			return result;
		}

		// Token: 0x06008424 RID: 33828 RVA: 0x003D63FE File Offset: 0x003D45FE
		public void HideTips()
		{
			SingletonObject.getInstance<TooltipManager>().HideTips(TipType.Count, true);
			this._previousKey = -1;
		}

		// Token: 0x06008425 RID: 33829 RVA: 0x003D6419 File Offset: 0x003D4619
		public int GetLinkIndexByEventData(Vector2 posVector)
		{
			return TMP_TextUtilities.FindIntersectingLink(this._linkText, posVector, this._mCamera);
		}

		// Token: 0x06008426 RID: 33830 RVA: 0x003D6434 File Offset: 0x003D4634
		private void ShowOrHideTip(Vector2 position)
		{
			int linkIndex = this.GetLinkIndexByEventData(position);
			bool flag = linkIndex < 0;
			if (flag)
			{
				this.HideTips();
			}
			else
			{
				bool flag2 = linkIndex != this._previousKey || this._counter > 0;
				if (flag2)
				{
					this._counter--;
					TMP_LinkInfo linkInfo = this._linkText.textInfo.linkInfo[linkIndex];
					string linkId = linkInfo.GetLinkID();
					EncyclopediaReferenceItem cfg = EncyclopediaReference.Instance[linkId];
					EncyclopediaReferenceItem newBuildingCfg;
					bool flag3;
					if (cfg.InsertType == EEncyclopediaReferenceInsertType.HyperLink && cfg.Params.CheckIndex(0))
					{
						newBuildingCfg = EncyclopediaReference.Instance[cfg.Params[0]];
						flag3 = (newBuildingCfg != null && newBuildingCfg.InsertType == EEncyclopediaReferenceInsertType.BuildingBlockTips);
					}
					else
					{
						flag3 = false;
					}
					bool flag4 = flag3;
					if (flag4)
					{
						cfg = newBuildingCfg;
					}
					EEncyclopediaReferenceInsertType insertType = cfg.InsertType;
					EEncyclopediaReferenceInsertType eencyclopediaReferenceInsertType = insertType;
					if (eencyclopediaReferenceInsertType != EEncyclopediaReferenceInsertType.HyperLink)
					{
						switch (eencyclopediaReferenceInsertType)
						{
						case EEncyclopediaReferenceInsertType.BuildingBlockTips:
						{
							BuildingBlockItem buildingCfg = BuildingBlock.Instance[int.Parse(cfg.Param)];
							ArgumentBox box = EasyPool.Get<ArgumentBox>().Set("arg0", buildingCfg.Name).Set("arg1", string.IsNullOrWhiteSpace(buildingCfg.FuncDesc) ? buildingCfg.Desc : (buildingCfg.Desc + "\n\n" + buildingCfg.FuncDesc.SetColor("pinkyellow")));
							SingletonObject.getInstance<TooltipManager>().ShowTips(TipType.Simple, box, true, false, false, null);
							EasyPool.Free<ArgumentBox>(box);
							this._previousKey = linkIndex;
							return;
						}
						case EEncyclopediaReferenceInsertType.ConsummateLevelTips:
						{
							ConsummateLevelItem config = ConsummateLevel.Instance[int.Parse(cfg.Param)];
							sbyte consummateLevel = config.TemplateId;
							ArgumentBox box = EasyPool.Get<ArgumentBox>().Set("arg0", config.Name.ColorReplace()).Set("arg1", string.Concat(new string[]
							{
								LocalStringManager.GetFormat(LanguageKey.LK_Consummate_Level_Tips, consummateLevel).ColorReplace(),
								"\n",
								LocalStringManager.GetFormat(LanguageKey.LK_Consummate_Neili_Tips, (int)(20 * consummateLevel)).ColorReplace(),
								"\n\n",
								config.Desc.ColorReplace()
							}));
							SingletonObject.getInstance<TooltipManager>().ShowTips(TipType.Simple, box, true, false, false, null);
							EasyPool.Free<ArgumentBox>(box);
							this._previousKey = linkIndex;
							return;
						}
						case EEncyclopediaReferenceInsertType.ProtagonistFeatureTips:
						{
							ProtagonistFeatureItem protagonistCfg = ProtagonistFeature.Instance[int.Parse(cfg.Param)];
							ValueTuple<string, string> mouseTipContent = NewGameFeatureItem.GetMouseTipContent(protagonistCfg, -1, true, true);
							string title = mouseTipContent.Item1;
							string desc = mouseTipContent.Item2;
							ArgumentBox box = EasyPool.Get<ArgumentBox>().Set("arg0", title).Set("arg1", desc);
							SingletonObject.getInstance<TooltipManager>().ShowTips(TipType.Simple, box, true, false, false, null);
							EasyPool.Free<ArgumentBox>(box);
							this._previousKey = linkIndex;
							return;
						}
						case EEncyclopediaReferenceInsertType.WorldStateTips:
						{
							WorldStateItem worldCfg = WorldState.Instance[int.Parse(cfg.Param)];
							ArgumentBox box = EasyPool.Get<ArgumentBox>().Set("arg0", worldCfg.Name.ColorReplace()).Set("arg1", worldCfg.Desc);
							SingletonObject.getInstance<TooltipManager>().ShowTips(TipType.Simple, box, true, false, false, null);
							EasyPool.Free<ArgumentBox>(box);
							this._previousKey = linkIndex;
							return;
						}
						case EEncyclopediaReferenceInsertType.LifeSkillTips:
						{
							LifeSkillItem lifeSkillCfg = LifeSkill.Instance[int.Parse(cfg.Param)];
							ArgumentBox box = EasyPool.Get<ArgumentBox>().Set("arg0", lifeSkillCfg.Name).Set("arg1", lifeSkillCfg.Desc);
							SingletonObject.getInstance<TooltipManager>().ShowTips(TipType.Simple, box, true, false, false, null);
							EasyPool.Free<ArgumentBox>(box);
							this._previousKey = linkIndex;
							return;
						}
						}
					}
					else
					{
						EncyclopediaReferenceItem newCfg;
						bool flag5;
						if (cfg.Params.CheckIndex(0))
						{
							newCfg = EncyclopediaReference.Instance[cfg.Params[0]];
							flag5 = (newCfg != null && newCfg.InsertType != EEncyclopediaReferenceInsertType.HyperLink);
						}
						else
						{
							flag5 = false;
						}
						bool flag6 = flag5;
						if (flag6)
						{
							cfg = newCfg;
						}
						else
						{
							EEncyclopediaContentLevel level = EncyclopediaContent.Instance[cfg.Param].Level & EEncyclopediaContentLevel.LowMidHigh;
							ValueTuple<bool, bool> conditions = new ValueTuple<bool, bool>(true, cfg.Desc.Length < 2);
							bool flag7 = conditions.Item1 && conditions.Item2;
							if (flag7)
							{
								return;
							}
							ArgumentBox argumentBox = EasyPool.Get<ArgumentBox>().Set("arg0", cfg.Desc[0]);
							if (!true)
							{
							}
							bool item = conditions.Item1;
							string arg;
							if (item)
							{
								bool item2 = conditions.Item2;
								if (item2)
								{
									arg = "";
								}
								else
								{
									arg = cfg.Desc[1];
								}
							}
							else if (!conditions.Item2)
							{
								arg = cfg.Desc[1] + "\n\n" + ((level < EEncyclopediaContentLevel.High) ? LanguageKey.LK_Encyclopedia_LinkTips_Mid_Desc : LanguageKey.LK_Encyclopedia_LinkTips_High_Desc).Tr();
							}
							else
							{
								arg = ((level < EEncyclopediaContentLevel.High) ? LanguageKey.LK_Encyclopedia_LinkTips_Mid_Desc : LanguageKey.LK_Encyclopedia_LinkTips_High_Desc).Tr();
							}
							if (!true)
							{
							}
							ArgumentBox box = argumentBox.Set("arg1", arg);
							SingletonObject.getInstance<TooltipManager>().ShowTips(TipType.Simple, box, true, false, false, null);
							EasyPool.Free<ArgumentBox>(box);
							this._previousKey = linkIndex;
							return;
						}
					}
					ArgumentBox argsBox = EasyPool.Get<ArgumentBox>();
					TipType tipsType = LinkElement.Ref2Tip(cfg.InsertType);
					TipType tipType = tipsType;
					TipType tipType2 = tipType;
					if (tipType2 > TipType.TrickType)
					{
						if (tipType2 <= TipType.Profession)
						{
							if (tipType2 == TipType.FiveElements)
							{
								argsBox.Set("neiliType", (int)NeiliType.Instance[int.Parse(cfg.Param)].TemplateId);
								SingletonObject.getInstance<TooltipManager>().ShowTips(tipsType, argsBox, true, false, false, null);
								goto IL_C26;
							}
							if (tipType2 != TipType.Profession)
							{
								goto IL_C26;
							}
						}
						else
						{
							if (tipType2 == TipType.LifeSkillCombatStrategy)
							{
								argsBox.Set("TemplateId", DebateStrategy.Instance[int.Parse(cfg.Param)].TemplateId);
								argsBox.Set("IsTargetMeet", true);
								argsBox.Set("IsPointMeet", true);
								SingletonObject.getInstance<TooltipManager>().ShowTips(tipsType, argsBox, true, false, false, null);
								goto IL_C26;
							}
							switch (tipType2)
							{
							case TipType.CricketEncyclopedia:
								goto IL_B12;
							case TipType.ProfessionEncyclopedia:
								break;
							case TipType.ProfessionSkillEncyclopedia:
								goto IL_BEC;
							default:
								goto IL_C26;
							}
						}
						argsBox.Set("TemplateDataOnly", true);
						argsBox.Set("ProfessionId", cfg.Param);
						SingletonObject.getInstance<TooltipManager>().ShowTips(TipType.ProfessionEncyclopedia, argsBox, true, false, false, null);
						goto IL_C26;
					}
					switch (tipType2)
					{
					case TipType.CombatSkill:
						argsBox.Set("CombatSkillId", CombatSkill.Instance[int.Parse(cfg.Param)].TemplateId);
						argsBox.Set("CharId", -1);
						argsBox.Set("ShowOnlyTemplateInfo", true);
						SingletonObject.getInstance<TooltipManager>().ShowTips(tipsType, argsBox, true, false, false, null);
						goto IL_C26;
					case (TipType)3:
					case (TipType)9:
					case TipType.LifeRecords:
					case TipType.Character:
					case TipType.Resource:
					case TipType.ResourceHolder:
					case TipType.EatingItems:
					case TipType.MapBlock:
						goto IL_C26;
					case TipType.Weapon:
					case TipType.SkillBook:
					case TipType.CraftTool:
					case TipType.Material:
					case TipType.Armor:
					case TipType.Carrier:
					case TipType.Clothing:
					case TipType.Food:
					case TipType.Medicine:
					case TipType.Misc:
					case TipType.TeaWine:
					case TipType.Accessory:
					{
						if (!true)
						{
						}
						ValueTuple<sbyte, short> valueTuple;
						switch (tipsType)
						{
						case TipType.Weapon:
							valueTuple = new ValueTuple<sbyte, short>(0, Weapon.Instance[int.Parse(cfg.Param)].TemplateId);
							goto IL_99D;
						case TipType.SkillBook:
							valueTuple = new ValueTuple<sbyte, short>(10, SkillBook.Instance[int.Parse(cfg.Param)].TemplateId);
							goto IL_99D;
						case TipType.CraftTool:
							valueTuple = new ValueTuple<sbyte, short>(6, CraftTool.Instance[int.Parse(cfg.Param)].TemplateId);
							goto IL_99D;
						case TipType.Material:
							valueTuple = new ValueTuple<sbyte, short>(5, Config.Material.Instance[int.Parse(cfg.Param)].TemplateId);
							goto IL_99D;
						case TipType.Armor:
							valueTuple = new ValueTuple<sbyte, short>(1, Armor.Instance[int.Parse(cfg.Param)].TemplateId);
							goto IL_99D;
						case TipType.Carrier:
							valueTuple = new ValueTuple<sbyte, short>(4, Carrier.Instance[int.Parse(cfg.Param)].TemplateId);
							goto IL_99D;
						case TipType.Clothing:
							valueTuple = new ValueTuple<sbyte, short>(3, Clothing.Instance[int.Parse(cfg.Param)].TemplateId);
							goto IL_99D;
						case TipType.Food:
							valueTuple = new ValueTuple<sbyte, short>(7, Food.Instance[int.Parse(cfg.Param)].TemplateId);
							goto IL_99D;
						case TipType.Medicine:
							valueTuple = new ValueTuple<sbyte, short>(8, Medicine.Instance[int.Parse(cfg.Param)].TemplateId);
							goto IL_99D;
						case TipType.Misc:
							valueTuple = new ValueTuple<sbyte, short>(12, Misc.Instance[int.Parse(cfg.Param)].TemplateId);
							goto IL_99D;
						case TipType.TeaWine:
							valueTuple = new ValueTuple<sbyte, short>(9, TeaWine.Instance[int.Parse(cfg.Param)].TemplateId);
							goto IL_99D;
						case TipType.Accessory:
							valueTuple = new ValueTuple<sbyte, short>(2, Accessory.Instance[int.Parse(cfg.Param)].TemplateId);
							goto IL_99D;
						}
						throw new Exception(string.Format("not supported: {0}", tipsType));
						IL_99D:
						if (!true)
						{
						}
						ValueTuple<sbyte, short> valueTuple2 = valueTuple;
						sbyte ty = valueTuple2.Item1;
						short id = valueTuple2.Item2;
						argsBox.Set<ItemDisplayData>("ItemData", new ItemDisplayData(ty, id));
						argsBox.Set("TemplateDataOnly", true);
						argsBox.Set("IsInCompareUI", false);
						argsBox.Set("DisableCompare", true);
						string[] cfgParams = cfg.Params;
						bool flag8 = cfgParams != null && cfgParams.Length > 0;
						if (flag8)
						{
							foreach (string argBoxKeyValue in cfgParams)
							{
								string[] keyValue = argBoxKeyValue.Split('=', 2, StringSplitOptions.None);
								string[] keyType;
								bool flag9;
								if (keyValue != null && keyValue.Length > 1)
								{
									keyType = keyValue[0].Split(':', 2, StringSplitOptions.None);
									flag9 = (keyType != null && keyType.Length > 1);
								}
								else
								{
									flag9 = false;
								}
								bool flag10 = flag9;
								if (flag10)
								{
									string text = keyType[1];
									string a = text;
									if (!(a == "bool"))
									{
										Debug.LogWarning("not support type: " + keyType[1] + "\nOriginal Arg: " + argBoxKeyValue);
									}
									else
									{
										argsBox.Set(keyType[0], keyValue[1].Trim().ToLower() == "true");
									}
								}
								else
								{
									Debug.LogWarning("argBoxKeyValue " + argBoxKeyValue + " cannot be parsed due to syntax error.");
								}
							}
						}
						SingletonObject.getInstance<TooltipManager>().ShowTips(tipsType, argsBox, tipsType != TipType.Medicine, false, false, null);
						goto IL_C26;
					}
					case TipType.Cricket:
						break;
					case TipType.Feature:
						argsBox.Set("TemplateDataOnly", true);
						argsBox.Set("FeatureId", CharacterFeature.Instance[int.Parse(cfg.Param)].TemplateId);
						SingletonObject.getInstance<TooltipManager>().ShowTips(TipType.Feature, argsBox, true, false, false, null);
						goto IL_C26;
					default:
					{
						if (tipType2 == TipType.ProfessionSkill)
						{
							goto IL_BEC;
						}
						if (tipType2 != TipType.TrickType)
						{
							goto IL_C26;
						}
						TrickTypeItem trick = TrickType.Instance[int.Parse(cfg.Param)];
						argsBox.Set("TrickType", trick.TemplateId);
						argsBox.Set("IsAvoidTrick", trick.AvoidType != -1);
						SingletonObject.getInstance<TooltipManager>().ShowTips(tipsType, argsBox, true, false, false, null);
						goto IL_C26;
					}
					}
					IL_B12:
					argsBox.Set("Part1", cfg.Params[0]);
					argsBox.Set("Part2", cfg.Params[1]);
					SingletonObject.getInstance<TooltipManager>().ShowTips(TipType.CricketEncyclopedia, argsBox, true, false, false, null);
					goto IL_C26;
					IL_BEC:
					argsBox.Set("TemplateDataOnly", true);
					argsBox.Set("ProfessionSkillId", cfg.Param);
					SingletonObject.getInstance<TooltipManager>().ShowTips(TipType.ProfessionSkillEncyclopedia, argsBox, true, false, false, null);
					IL_C26:
					EasyPool.Free<ArgumentBox>(argsBox);
					this._previousKey = linkIndex;
				}
			}
		}

		// Token: 0x04006531 RID: 25905
		private int _previousKey = -1;

		// Token: 0x04006532 RID: 25906
		private int _counter = 2;

		// Token: 0x04006533 RID: 25907
		private TextLinkBackgroundHandler _backgroundHandler;

		// Token: 0x04006534 RID: 25908
		private Dictionary<string, NodeData> _linkDict;

		// Token: 0x04006535 RID: 25909
		private TextMeshProUGUI _linkText;

		// Token: 0x04006536 RID: 25910
		private Camera _mCamera;
	}
}
