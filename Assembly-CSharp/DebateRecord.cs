using System;
using System.Collections.Generic;
using Config;
using FrameWork;
using GameData.Domains.Character.Display;
using GameData.Domains.Taiwu.Debate;
using TMPro;
using UnityEngine;

// Token: 0x02000240 RID: 576
public class DebateRecord : Refers
{
	// Token: 0x170003E0 RID: 992
	// (get) Token: 0x06002593 RID: 9619 RVA: 0x00114385 File Offset: 0x00112585
	private LifeSkillCombatModel Model
	{
		get
		{
			return SingletonObject.getInstance<LifeSkillCombatModel>();
		}
	}

	// Token: 0x170003E1 RID: 993
	// (get) Token: 0x06002594 RID: 9620 RVA: 0x0011438C File Offset: 0x0011258C
	private GameObject TemplateFull
	{
		get
		{
			return base.CGet<GameObject>("TemplateFull");
		}
	}

	// Token: 0x170003E2 RID: 994
	// (get) Token: 0x06002595 RID: 9621 RVA: 0x00114399 File Offset: 0x00112599
	private GameObject TemplateLeft
	{
		get
		{
			return base.CGet<GameObject>("TemplateLeft");
		}
	}

	// Token: 0x170003E3 RID: 995
	// (get) Token: 0x06002596 RID: 9622 RVA: 0x001143A6 File Offset: 0x001125A6
	private GameObject TemplateRight
	{
		get
		{
			return base.CGet<GameObject>("TemplateRight");
		}
	}

	// Token: 0x170003E4 RID: 996
	// (get) Token: 0x06002597 RID: 9623 RVA: 0x001143B3 File Offset: 0x001125B3
	private CScrollbarLegacy CScrollbar
	{
		get
		{
			return base.CGet<CScrollbarLegacy>("VerticalScrollbar");
		}
	}

	// Token: 0x170003E5 RID: 997
	// (get) Token: 0x06002598 RID: 9624 RVA: 0x001143C0 File Offset: 0x001125C0
	private GameObject Content
	{
		get
		{
			return base.CGet<GameObject>("Content");
		}
	}

	// Token: 0x06002599 RID: 9625 RVA: 0x001143CD File Offset: 0x001125CD
	public void SetIsTaiwu(bool isTaiwu)
	{
		this._isTaiwu = isTaiwu;
	}

	// Token: 0x0600259A RID: 9626 RVA: 0x001143D8 File Offset: 0x001125D8
	public void Add(DebateOperation operation)
	{
		bool flag = operation.TemplateId < 0;
		if (flag)
		{
			this.UpdateRound();
		}
		else
		{
			GameObject obj = Object.Instantiate<GameObject>(base.CGet<GameObject>("Template"), this.Content.transform);
			Refers refers = obj.GetComponent<Refers>();
			TextMeshProUGUI textArea = refers.CGet<TextMeshProUGUI>("TextArea");
			TMPTextSpriteHelper helper = textArea.GetComponent<TMPTextSpriteHelper>();
			string text = this.GetText(operation.TemplateId, operation);
			textArea.text = text;
			bool needIcon = this.GetNeedIcon(operation.TemplateId);
			if (needIcon)
			{
				helper.OnParseComplete = delegate()
				{
					this.AddTips(textArea, operation);
				};
				helper.Parse();
			}
			else
			{
				this.AddTips(textArea, operation);
			}
			obj.SetActive(true);
			this._lines.Add(obj);
			SingletonObject.getInstance<YieldHelper>().DelayFrameDo(2U, delegate
			{
				this.CScrollbar.value = 1f;
			});
		}
	}

	// Token: 0x0600259B RID: 9627 RVA: 0x001144F8 File Offset: 0x001126F8
	public void UpdateRound()
	{
		GameObject obj = Object.Instantiate<GameObject>(base.CGet<GameObject>("Template2"), this.Content.transform);
		obj.SetActive(true);
		obj.transform.SetParent(this.Content.transform, false);
		this._lines.Add(obj);
		SingletonObject.getInstance<YieldHelper>().DelayFrameDo(2U, delegate
		{
			this.CScrollbar.value = 1f;
		});
	}

	// Token: 0x0600259C RID: 9628 RVA: 0x00114568 File Offset: 0x00112768
	public void Clear()
	{
		foreach (GameObject tip in this._tips)
		{
			Object.Destroy(tip);
		}
		foreach (GameObject line in this._lines)
		{
			Object.Destroy(line);
		}
		this._tips.Clear();
		this._lines.Clear();
		this.CScrollbar.gameObject.SetActive(false);
	}

	// Token: 0x0600259D RID: 9629 RVA: 0x0011462C File Offset: 0x0011282C
	private string GetText(short templateId, DebateOperation operation)
	{
		DebateRecordItem config = Config.DebateRecord.Instance[templateId];
		string res = Config.DebateRecord.Instance[templateId].Desc.ColorReplace();
		int[] recordParams = operation.RecordParams;
		int count = 0;
		List<string> rendered = new List<string>();
		for (int index = 0; index < config.Parameters.Length; index++)
		{
			EDebateRecordParamType param = config.Parameters[index];
			bool flag = param == EDebateRecordParamType.IntValue;
			if (flag)
			{
				rendered.Add(recordParams[index].ToString());
			}
			else
			{
				bool flag2 = param == EDebateRecordParamType.GamePoint;
				if (flag2)
				{
					rendered.Add(this.GetIcon(param, -1) + LocalStringManager.Get(LanguageKey.LK_LifeSkillCombat_Score));
				}
				else
				{
					bool flag3 = param == EDebateRecordParamType.StrategyPoint;
					if (flag3)
					{
						rendered.Add(this.GetIcon(param, -1) + LocalStringManager.Get(LanguageKey.LK_LifeSkillCombat_StrategyPoint));
					}
					else
					{
						bool flag4 = param == EDebateRecordParamType.Bases;
						if (flag4)
						{
							rendered.Add(this.GetIcon(param, -1) + LocalStringManager.Get(LanguageKey.LK_LifeskillCombat_Bases));
						}
						else
						{
							bool flag5 = param == EDebateRecordParamType.Pressure;
							if (flag5)
							{
								rendered.Add(this.GetIcon(param, -1) + LocalStringManager.Get(LanguageKey.LK_LifeSkillCombat_Stress));
							}
							else
							{
								bool flag6 = param == EDebateRecordParamType.Pawn;
								if (flag6)
								{
									rendered.Add(this.GetIcon(param, -1) + this.GetSelfText(this.GetPawnIsSelf(recordParams[index])) + LocalStringManager.Get(LanguageKey.LK_LifeSkillCombat_Unit_Tip_Title));
								}
								else
								{
									bool flag7 = param == EDebateRecordParamType.SelfPawn;
									if (flag7)
									{
										rendered.Add(this.GetIcon(param, -1) + this.GetSelfText(true) + LocalStringManager.Get(LanguageKey.LK_LifeSkillCombat_Unit_Tip_Title));
									}
									else
									{
										bool flag8 = param == EDebateRecordParamType.OpponentPawn;
										if (flag8)
										{
											rendered.Add(this.GetIcon(param, -1) + this.GetSelfText(false) + LocalStringManager.Get(LanguageKey.LK_LifeSkillCombat_Unit_Tip_Title));
										}
										else
										{
											bool flag9 = param == EDebateRecordParamType.Strategy;
											if (flag9)
											{
												rendered.Add(string.Format("{0}<color=#F28234FF><link=\"{1}{2}_{3}\"> {4} </link></color>", new object[]
												{
													this.GetIcon(param, recordParams[index]),
													"tips_",
													"Strategy",
													index,
													DebateStrategy.Instance[recordParams[index]].Name
												}));
											}
											else
											{
												bool flag10 = param == EDebateRecordParamType.BottomNode;
												if (flag10)
												{
													rendered.Add(this.GetIcon(param, -1) + LocalStringManager.Get(LanguageKey.LK_LifeskillCombat_BottomNode));
												}
												else
												{
													bool flag11 = param == EDebateRecordParamType.NodeEffect;
													if (flag11)
													{
														rendered.Add(string.Format("{0}<color=#F28234FF><link=\"{1}{2}_{3}\"> {4} </link></color>", new object[]
														{
															this.GetIcon(param, this.Model.DebateGame.NodeEffects[recordParams[index]].TemplateId),
															"tips_",
															"NodeEffect",
															index,
															DebateNodeEffect.Instance[this.Model.DebateGame.NodeEffects[recordParams[index]].TemplateId].Name
														}));
													}
													else
													{
														bool flag12 = param == EDebateRecordParamType.Spectator;
														if (flag12)
														{
															rendered.Add(this.GetCharName(recordParams[index]) ?? "");
														}
														else
														{
															bool flag13 = param == EDebateRecordParamType.Character;
															if (flag13)
															{
																rendered.Add(this.GetCharName(recordParams[index]) ?? "");
															}
															else
															{
																bool flag14 = param == EDebateRecordParamType.Comment;
																if (flag14)
																{
																	rendered.Add(string.Format("{0}<color=#F28234FF><link=\"{1}{2}_{3}\"> {4} </link></color>", new object[]
																	{
																		this.GetIcon(param, recordParams[index]),
																		"tips_",
																		"Comment",
																		index,
																		Config.DebateComment.Instance[recordParams[index]].Name
																	}));
																}
																else
																{
																	bool flag15 = param == EDebateRecordParamType.OwnedCards;
																	if (flag15)
																	{
																		rendered.Add(this.GetIcon(param, -1) + this.GetSelfText(recordParams[index] == 0) + LocalStringManager.Get(LanguageKey.LK_LifeSkillCombat_CardGroup_Owned));
																	}
																	else
																	{
																		bool flag16 = param == EDebateRecordParamType.UsedCards;
																		if (flag16)
																		{
																			rendered.Add(this.GetIcon(param, -1) + this.GetSelfText(recordParams[index] == 0) + LocalStringManager.Get(LanguageKey.LK_LifeSkillCombat_CardGroup_Used));
																		}
																		else
																		{
																			bool flag17 = param == EDebateRecordParamType.PawnCount;
																			if (!flag17)
																			{
																				break;
																			}
																			rendered.Add(recordParams[index].ToString());
																		}
																	}
																}
															}
														}
													}
												}
											}
										}
									}
								}
							}
						}
					}
				}
			}
			count++;
		}
		bool flag18 = count == 0;
		string result;
		if (flag18)
		{
			result = res;
		}
		else
		{
			object[] o = new object[rendered.Count];
			for (int index2 = 0; index2 < rendered.Count; index2++)
			{
				o[index2] = rendered[index2];
			}
			result = string.Format(res, o);
		}
		return result;
	}

	// Token: 0x0600259E RID: 9630 RVA: 0x00114B1C File Offset: 0x00112D1C
	private bool GetNeedIcon(short templateId)
	{
		EDebateRecordParamType[] param = Config.DebateRecord.Instance[templateId].Parameters;
		foreach (EDebateRecordParamType type in param)
		{
			bool flag = type == EDebateRecordParamType.Strategy || type == EDebateRecordParamType.NodeEffect || type == EDebateRecordParamType.Comment;
			if (flag)
			{
				return true;
			}
		}
		return false;
	}

	// Token: 0x0600259F RID: 9631 RVA: 0x00114B74 File Offset: 0x00112D74
	private string GetIcon(EDebateRecordParamType type, int templateId = -1)
	{
		if (!true)
		{
		}
		string result;
		if (type != EDebateRecordParamType.Strategy)
		{
			if (type != EDebateRecordParamType.NodeEffect)
			{
				if (type != EDebateRecordParamType.Comment)
				{
					result = "";
				}
				else
				{
					result = string.Format("<SpName=mousetip_lichang_{0}>", Config.DebateComment.Instance[templateId].BehaviorType);
				}
			}
			else
			{
				result = string.Format("<SpName=mousetip_lichang_{0}>", DebateNodeEffect.Instance[templateId].BehaviorType);
			}
		}
		else
		{
			result = string.Format("<SpName=lifeskillcombat_cardtype_0_{0}>", DebateStrategy.Instance[templateId].LifeSkillType);
		}
		if (!true)
		{
		}
		return result;
	}

	// Token: 0x060025A0 RID: 9632 RVA: 0x00114C14 File Offset: 0x00112E14
	private bool GetPawnIsSelf(int pawnId)
	{
		return this.Model.DebateGame.Pawns[pawnId].IsOwnedByTaiwu == this._isTaiwu;
	}

	// Token: 0x060025A1 RID: 9633 RVA: 0x00114C4C File Offset: 0x00112E4C
	private string GetSelfText(bool isSelf)
	{
		return isSelf ? LocalStringManager.Get(LanguageKey.UI_LifeSkillBattle_House_Self) : LocalStringManager.Get(LanguageKey.UI_LifeSkillBattle_House_Adversary);
	}

	// Token: 0x060025A2 RID: 9634 RVA: 0x00114C78 File Offset: 0x00112E78
	private string GetCharName(int charId)
	{
		bool flag = charId == this.Model.TaiwuCharId;
		string result;
		if (flag)
		{
			result = NameCenter.GetMonasticTitleOrDisplayName(this.Model.TaiwuCharData, true);
		}
		else
		{
			bool flag2 = charId == this.Model.EnemyCharId;
			if (flag2)
			{
				result = NameCenter.GetMonasticTitleOrDisplayName(this.Model.EnemyCharData, false);
			}
			else
			{
				foreach (CharacterDisplayData data in this.Model.SelfAudienceList)
				{
					bool flag3 = data != null && data.CharacterId == charId;
					if (flag3)
					{
						return NameCenter.GetMonasticTitleOrDisplayName(data, false);
					}
				}
				foreach (CharacterDisplayData data2 in this.Model.EnemyAudienceList)
				{
					bool flag4 = data2 != null && data2.CharacterId == charId;
					if (flag4)
					{
						return NameCenter.GetMonasticTitleOrDisplayName(data2, false);
					}
				}
				result = LocalStringManager.Get(LanguageKey.LK_ThreeQuestionMark);
			}
		}
		return result;
	}

	// Token: 0x060025A3 RID: 9635 RVA: 0x00114DB8 File Offset: 0x00112FB8
	private int GetTipData(string data, out string type)
	{
		type = null;
		for (int i = data.Length - 1; i >= 0; i--)
		{
			char c = data[i];
			bool flag = c == '_';
			if (flag)
			{
				type = data.Substring(5, i - 5);
				int index;
				int.TryParse(data.Substring(i + 1), out index);
				return index;
			}
		}
		return -1;
	}

	// Token: 0x060025A4 RID: 9636 RVA: 0x00114E24 File Offset: 0x00113024
	private void AddTips(TextMeshProUGUI textArea, DebateOperation operation)
	{
		bool flag = textArea.textInfo == null;
		if (!flag)
		{
			foreach (TMP_LinkInfo info in textArea.textInfo.linkInfo)
			{
				string id = info.GetLinkID();
				bool flag2 = !id.StartsWith("tips_");
				if (!flag2)
				{
					int linkStartIndex = info.linkTextfirstCharacterIndex + 1;
					int linkLength = info.linkTextLength - 2;
					TMP_CharacterInfo charInfoStart = textArea.textInfo.characterInfo[linkStartIndex];
					TMP_CharacterInfo charInfoEnd = textArea.textInfo.characterInfo[linkStartIndex + linkLength];
					string tipType;
					int tipIndex = this.GetTipData(id, out tipType);
					bool flag3 = charInfoStart.lineNumber == charInfoEnd.lineNumber;
					if (flag3)
					{
						this.CreateTipObj(this.TemplateFull, textArea, charInfoStart, charInfoEnd, operation, tipType, tipIndex);
					}
					else
					{
						for (int i = 1; i < linkLength; i++)
						{
							TMP_CharacterInfo charInfo = textArea.textInfo.characterInfo[linkStartIndex + i];
							bool flag4 = charInfo.lineNumber == charInfoStart.lineNumber;
							if (!flag4)
							{
								this.CreateTipObj(this.TemplateLeft, textArea, charInfoStart, textArea.textInfo.characterInfo[linkStartIndex + i - 1], operation, tipType, tipIndex);
								this.CreateTipObj(this.TemplateRight, textArea, charInfo, charInfoEnd, operation, tipType, tipIndex);
								break;
							}
						}
					}
				}
			}
		}
	}

	// Token: 0x060025A5 RID: 9637 RVA: 0x00114FA4 File Offset: 0x001131A4
	private void CreateTipObj(GameObject template, TextMeshProUGUI textArea, TMP_CharacterInfo charInfoA, TMP_CharacterInfo charInfoB, DebateOperation operation, string tipType, int index)
	{
		Transform parent = textArea.transform.parent.GetChild(0);
		GameObject obj = Object.Instantiate<GameObject>(template, parent);
		TooltipInvoker tip = obj.GetComponent<TooltipInvoker>();
		float btnWidth = charInfoB.topRight.x - charInfoA.topLeft.x + 5f;
		float x = (charInfoA.bottomLeft.x + charInfoB.topRight.x) * 0.5f;
		int y = charInfoA.lineNumber * -30;
		((RectTransform)obj.transform).SetWidth(btnWidth);
		((RectTransform)obj.transform).anchoredPosition = new Vector2(x, (float)y);
		if (!(tipType == "Pressure"))
		{
			if (!(tipType == "NodeEffect"))
			{
				if (!(tipType == "Strategy"))
				{
					if (tipType == "Comment")
					{
						DebateCommentItem config = Config.DebateComment.Instance[operation.RecordParams[index]];
						tip.Type = TipType.Simple;
						tip.PresetParam = new string[2];
						tip.PresetParam[0] = config.Name;
						tip.PresetParam[1] = config.ResultTip;
					}
				}
				else
				{
					tip.Type = TipType.LifeSkillCombatStrategy;
					TooltipInvoker tooltipInvoker = tip;
					if (tooltipInvoker.RuntimeParam == null)
					{
						tooltipInvoker.RuntimeParam = EasyPool.Get<ArgumentBox>();
					}
					tip.RuntimeParam.Set("TemplateId", (short)operation.RecordParams[index]);
					tip.RuntimeParam.Set("IsPointMeet", true);
					tip.RuntimeParam.Set("IsTargetMeet", true);
				}
			}
			else
			{
				tip.Type = TipType.LifeSkillCombatBlock;
				TooltipInvoker tooltipInvoker = tip;
				if (tooltipInvoker.RuntimeParam == null)
				{
					tooltipInvoker.RuntimeParam = EasyPool.Get<ArgumentBox>();
				}
				tip.RuntimeParam.Set<DebateNodeEffectState>("EffectState", this.Model.DebateGame.NodeEffects[operation.RecordParams[index]]);
			}
		}
		else
		{
			tip.Type = TipType.LifeSkillCombatStress;
			TooltipInvoker tooltipInvoker = tip;
			if (tooltipInvoker.RuntimeParam == null)
			{
				tooltipInvoker.RuntimeParam = EasyPool.Get<ArgumentBox>();
			}
			tip.RuntimeParam.Set("IsTaiwu", operation.IsTaiwu);
		}
		obj.SetActive(true);
	}

	// Token: 0x04001BEB RID: 7147
	private List<GameObject> _lines = new List<GameObject>();

	// Token: 0x04001BEC RID: 7148
	private List<GameObject> _tips = new List<GameObject>();

	// Token: 0x04001BED RID: 7149
	private bool _isTaiwu;

	// Token: 0x04001BEE RID: 7150
	private const string TipsLinkPrefix = "tips_";

	// Token: 0x04001BEF RID: 7151
	private const string TipsPressure = "Pressure";

	// Token: 0x04001BF0 RID: 7152
	private const string TipsNodeEffect = "NodeEffect";

	// Token: 0x04001BF1 RID: 7153
	private const string TipsStrategy = "Strategy";

	// Token: 0x04001BF2 RID: 7154
	private const string TipsComment = "Comment";

	// Token: 0x04001BF3 RID: 7155
	private const float WidthOffset = 5f;
}
