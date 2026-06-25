using System;
using System.Collections.Generic;
using Config;
using FrameWork;
using FrameWork.UISystem.UIElements;
using GameData.Domains.Character.Display;
using GameData.Domains.Taiwu.Debate;
using TMPro;
using UnityEngine;

namespace Game.Views.Debate
{
	// Token: 0x02000AA0 RID: 2720
	public class DebateRecord : MonoBehaviour
	{
		// Token: 0x17000EAB RID: 3755
		// (get) Token: 0x0600854D RID: 34125 RVA: 0x003DEA63 File Offset: 0x003DCC63
		private LifeSkillCombatModel Model
		{
			get
			{
				return SingletonObject.getInstance<LifeSkillCombatModel>();
			}
		}

		// Token: 0x0600854E RID: 34126 RVA: 0x003DEA6C File Offset: 0x003DCC6C
		private void Awake()
		{
			this.template.gameObject.SetActive(false);
			this.templateLine.gameObject.SetActive(false);
			this.templateFull.gameObject.SetActive(false);
			this.templateLeft.gameObject.SetActive(false);
			this.templateRight.gameObject.SetActive(false);
		}

		// Token: 0x0600854F RID: 34127 RVA: 0x003DEAD4 File Offset: 0x003DCCD4
		public void SetIsTaiwu(bool isTaiwu)
		{
			this._isTaiwu = isTaiwu;
		}

		// Token: 0x06008550 RID: 34128 RVA: 0x003DEAE0 File Offset: 0x003DCCE0
		public void Add(DebateOperation operation)
		{
			bool flag = operation.TemplateId < 0;
			if (flag)
			{
				this.UpdateRound();
			}
			else
			{
				GameObject obj = Object.Instantiate<GameObject>(this.template, this.content.transform);
				TextMeshProUGUI textArea = obj.GetComponentInChildren<TextMeshProUGUI>();
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
					this.scrollbar.value = 1f;
				});
			}
		}

		// Token: 0x06008551 RID: 34129 RVA: 0x003DEBF0 File Offset: 0x003DCDF0
		public void UpdateRound()
		{
			GameObject obj = Object.Instantiate<GameObject>(this.templateLine, this.content.transform);
			obj.SetActive(true);
			obj.transform.SetParent(this.content.transform, false);
			this._lines.Add(obj);
			SingletonObject.getInstance<YieldHelper>().DelayFrameDo(2U, delegate
			{
				this.scrollbar.value = 1f;
			});
		}

		// Token: 0x06008552 RID: 34130 RVA: 0x003DEC5C File Offset: 0x003DCE5C
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
			this.scrollbar.gameObject.SetActive(false);
		}

		// Token: 0x06008553 RID: 34131 RVA: 0x003DED20 File Offset: 0x003DCF20
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
											rendered.Add(this.GetIcon(param, -1) + LocalStringManager.GetFormat(LanguageKey.LK_Debate_Display_SelfPawn_Format, this.GetSelfText(true), LocalStringManager.Get(LanguageKey.LK_LifeSkillCombat_Unit_Tip_Title)));
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

		// Token: 0x06008554 RID: 34132 RVA: 0x003DF218 File Offset: 0x003DD418
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

		// Token: 0x06008555 RID: 34133 RVA: 0x003DF270 File Offset: 0x003DD470
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

		// Token: 0x06008556 RID: 34134 RVA: 0x003DF310 File Offset: 0x003DD510
		private bool GetPawnIsSelf(int pawnId)
		{
			return this.Model.DebateGame.Pawns[pawnId].IsOwnedByTaiwu == this._isTaiwu;
		}

		// Token: 0x06008557 RID: 34135 RVA: 0x003DF348 File Offset: 0x003DD548
		private string GetSelfText(bool isSelf)
		{
			return isSelf ? LocalStringManager.Get(LanguageKey.UI_LifeSkillBattle_House_Self) : LocalStringManager.Get(LanguageKey.UI_LifeSkillBattle_House_Adversary);
		}

		// Token: 0x06008558 RID: 34136 RVA: 0x003DF374 File Offset: 0x003DD574
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

		// Token: 0x06008559 RID: 34137 RVA: 0x003DF4B4 File Offset: 0x003DD6B4
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

		// Token: 0x0600855A RID: 34138 RVA: 0x003DF520 File Offset: 0x003DD720
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
							this.CreateTipObj(this.templateFull, textArea, charInfoStart, charInfoEnd, operation, tipType, tipIndex);
						}
						else
						{
							for (int i = 1; i < linkLength; i++)
							{
								TMP_CharacterInfo charInfo = textArea.textInfo.characterInfo[linkStartIndex + i];
								bool flag4 = charInfo.lineNumber == charInfoStart.lineNumber;
								if (!flag4)
								{
									this.CreateTipObj(this.templateLeft, textArea, charInfoStart, textArea.textInfo.characterInfo[linkStartIndex + i - 1], operation, tipType, tipIndex);
									this.CreateTipObj(this.templateRight, textArea, charInfo, charInfoEnd, operation, tipType, tipIndex);
									break;
								}
							}
						}
					}
				}
			}
		}

		// Token: 0x0600855B RID: 34139 RVA: 0x003DF6A0 File Offset: 0x003DD8A0
		private void CreateTipObj(GameObject template, TextMeshProUGUI textArea, TMP_CharacterInfo charInfoA, TMP_CharacterInfo charInfoB, DebateOperation operation, string tipType, int index)
		{
			Transform parent = textArea.transform.parent.GetChild(0);
			GameObject obj = Object.Instantiate<GameObject>(template, parent);
			TooltipInvoker tip = obj.GetComponent<TooltipInvoker>();
			float btnWidth = charInfoB.topRight.x - charInfoA.topLeft.x + 5f;
			float x = (charInfoA.bottomLeft.x + charInfoB.topRight.x) * 0.5f;
			int space;
			int y = charInfoA.lineNumber * (this._languageSpace.TryGetValue(LocalStringManager.CurLanguageType, out space) ? space : this._languageSpace[LocalStringManager.LanguageType.CN]);
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

		// Token: 0x0400663A RID: 26170
		[SerializeField]
		private GameObject content;

		// Token: 0x0400663B RID: 26171
		[SerializeField]
		private CScrollbar scrollbar;

		// Token: 0x0400663C RID: 26172
		[SerializeField]
		private GameObject template;

		// Token: 0x0400663D RID: 26173
		[SerializeField]
		private GameObject templateLine;

		// Token: 0x0400663E RID: 26174
		[SerializeField]
		private GameObject templateFull;

		// Token: 0x0400663F RID: 26175
		[SerializeField]
		private GameObject templateLeft;

		// Token: 0x04006640 RID: 26176
		[SerializeField]
		private GameObject templateRight;

		// Token: 0x04006641 RID: 26177
		private List<GameObject> _lines = new List<GameObject>();

		// Token: 0x04006642 RID: 26178
		private List<GameObject> _tips = new List<GameObject>();

		// Token: 0x04006643 RID: 26179
		private bool _isTaiwu;

		// Token: 0x04006644 RID: 26180
		private const string TipsLinkPrefix = "tips_";

		// Token: 0x04006645 RID: 26181
		private const string TipsPressure = "Pressure";

		// Token: 0x04006646 RID: 26182
		private const string TipsNodeEffect = "NodeEffect";

		// Token: 0x04006647 RID: 26183
		private const string TipsStrategy = "Strategy";

		// Token: 0x04006648 RID: 26184
		private const string TipsComment = "Comment";

		// Token: 0x04006649 RID: 26185
		private const float WidthOffset = 5f;

		// Token: 0x0400664A RID: 26186
		private readonly Dictionary<LocalStringManager.LanguageType, int> _languageSpace = new Dictionary<LocalStringManager.LanguageType, int>
		{
			{
				LocalStringManager.LanguageType.CN,
				-30
			},
			{
				LocalStringManager.LanguageType.EN,
				-28
			}
		};
	}
}
