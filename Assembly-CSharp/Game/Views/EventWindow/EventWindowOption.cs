using System;
using System.Collections.Generic;
using System.Text;
using Config;
using FrameWork;
using FrameWork.UISystem.UIElements;
using Game.Views.MouseTips;
using GameData.Domains.Character;
using GameData.Domains.Character.Display;
using GameData.Domains.TaiwuEvent.DisplayEvent;
using GameData.Domains.TaiwuEvent.EventOption;
using GameData.Utilities;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Game.Views.EventWindow
{
	// Token: 0x02000A40 RID: 2624
	public class EventWindowOption : MonoBehaviour
	{
		// Token: 0x17000E2E RID: 3630
		// (get) Token: 0x0600817E RID: 33150 RVA: 0x003C42E0 File Offset: 0x003C24E0
		private float wholeWidth
		{
			get
			{
				return this.rect.sizeDelta.x;
			}
		}

		// Token: 0x17000E2F RID: 3631
		// (get) Token: 0x0600817F RID: 33151 RVA: 0x003C42F4 File Offset: 0x003C24F4
		private EventModel Model
		{
			get
			{
				bool flag = this._model == null;
				if (flag)
				{
					this._model = SingletonObject.getInstance<EventModel>();
				}
				return this._model;
			}
		}

		// Token: 0x17000E30 RID: 3632
		// (get) Token: 0x06008180 RID: 33152 RVA: 0x003C4324 File Offset: 0x003C2524
		private TaiwuEventDisplayData Data
		{
			get
			{
				return this.Model.DisplayingEventData;
			}
		}

		// Token: 0x06008181 RID: 33153 RVA: 0x003C4334 File Offset: 0x003C2534
		private void ResetConsumeLayoutState()
		{
			this._consumeUseSecondLine = false;
			this._consumeSplitLines = null;
			this._consumeSplitTipNames = null;
			this.consummeInfo.transform.SetParent(this.firstLineConsummeInfoParent, false);
			this.secondLineLayout.gameObject.SetActive(false);
			for (int i = this.secondLineConsummeInfoParent.childCount - 1; i >= 0; i--)
			{
				Transform child = this.secondLineConsummeInfoParent.GetChild(i);
				bool flag = child == this.consummeInfo.transform;
				if (!flag)
				{
					Object.DestroyImmediate(child.gameObject);
				}
			}
		}

		// Token: 0x06008182 RID: 33154 RVA: 0x003C43D4 File Offset: 0x003C25D4
		private void ApplyConsumeSplitLines()
		{
			bool flag = this._consumeSplitLines == null || this._consumeSplitTipNames == null;
			if (!flag)
			{
				this.secondLineLayout.gameObject.SetActive(true);
				this.consummeInfo.transform.SetParent(this.secondLineConsummeInfoParent, false);
				CommonUtils.PrepareEnoughChildren(this.secondLineConsummeInfoParent, this.consummeInfo.gameObject, this._consumeSplitLines.Count, null);
				for (int i = 0; i < this._consumeSplitLines.Count; i++)
				{
					bool flag2 = i >= this._consumeSplitTipNames.Count;
					if (flag2)
					{
						break;
					}
					TextMeshProUGUI txtComp = this.secondLineConsummeInfoParent.GetChild(i).GetComponent<TextMeshProUGUI>();
					bool flag3 = txtComp == null;
					if (!flag3)
					{
						txtComp.alignment = TextAlignmentOptions.Left;
						txtComp.text = this._consumeSplitLines[i];
						this.ParseConsumeInfoWithTips(txtComp, this._consumeSplitTipNames[i]);
					}
				}
			}
		}

		// Token: 0x06008183 RID: 33155 RVA: 0x003C44E4 File Offset: 0x003C26E4
		private void CheckLayout(List<ConsumeInfoDisplay> consumeInfoList, int setupGen = -1)
		{
			bool flag = setupGen >= 0 && setupGen != this._layoutSetupGen;
			if (!flag)
			{
				bool flag2 = !this || !base.isActiveAndEnabled || consumeInfoList == null || this.consummeInfo == null || this.firstLineConsummeInfoParent == null || this.secondLineConsummeInfoParent == null || this.secondLineLayout == null || this.firstLineLeftLayout == null || this.rect == null;
				if (!flag2)
				{
					this.consummeInfo.alignment = TextAlignmentOptions.Right;
					this.firstLineConsummeInfoParent.gameObject.SetActive(consumeInfoList.Count > 0);
					bool flag3 = consumeInfoList.Count == 0;
					if (flag3)
					{
						this.ResetConsumeLayoutState();
					}
					else
					{
						bool flag4 = this._consumeSplitLines != null;
						if (flag4)
						{
							this.secondLineLayout.gameObject.SetActive(true);
							bool flag5 = this.secondLineConsummeInfoParent.childCount < this._consumeSplitLines.Count || this.consummeInfo.transform.parent != this.secondLineConsummeInfoParent;
							if (flag5)
							{
								this.ApplyConsumeSplitLines();
							}
						}
						else
						{
							bool consumeUseSecondLine = this._consumeUseSecondLine;
							if (consumeUseSecondLine)
							{
								this.secondLineLayout.gameObject.SetActive(true);
								this.consummeInfo.transform.SetParent(this.secondLineConsummeInfoParent, false);
							}
							else
							{
								this.consummeInfo.transform.SetParent(this.firstLineConsummeInfoParent, false);
								float singleLineWidth = this.consummeInfo.preferredWidth + this.firstLineLeftLayout.preferredWidth;
								bool needSecondLine = this._consumeUseSecondLine || (singleLineWidth > this.wholeWidth && consumeInfoList.Count > 0);
								bool flag6 = needSecondLine;
								if (flag6)
								{
									this._consumeUseSecondLine = true;
									this.consummeInfo.transform.SetParent(this.secondLineConsummeInfoParent);
									this.secondLineLayout.gameObject.SetActive(true);
									bool flag7 = this.consummeInfo.preferredWidth > this.wholeWidth;
									if (flag7)
									{
										StringBuilder sb = new StringBuilder();
										List<string> result = new List<string>();
										List<List<string>> resultTipNames = new List<List<string>>();
										List<string> currentTipNames = new List<string>();
										TMPTextSpriteHelper measureHelper = this.consummeInfo.GetComponent<TMPTextSpriteHelper>();
										bool flag8 = measureHelper == null;
										if (!flag8)
										{
											Action savedParseComplete = measureHelper.OnParseComplete;
											measureHelper.OnParseComplete = null;
											int i = 0;
											while (i < consumeInfoList.Count)
											{
												int phaseStartIndex = sb.Length;
												bool flag9 = sb.Length > 0;
												if (flag9)
												{
													sb.Append("    ");
												}
												sb.Append(consumeInfoList[i].Text);
												currentTipNames.Add(consumeInfoList[i].TipName ?? string.Empty);
												int phaseLength = sb.Length - phaseStartIndex;
												this.consummeInfo.text = sb.ToString();
												measureHelper.Parse();
												bool flag10 = sb.Length > 0 && this.consummeInfo.preferredWidth > this.wholeWidth;
												if (flag10)
												{
													sb.Remove(phaseStartIndex, phaseLength);
													bool flag11 = currentTipNames.Count > 0;
													if (flag11)
													{
														currentTipNames.RemoveAt(currentTipNames.Count - 1);
													}
													bool flag12 = sb.Length == 0;
													if (flag12)
													{
														result.Add(consumeInfoList[i].Text);
														resultTipNames.Add(new List<string>
														{
															consumeInfoList[i].TipName ?? string.Empty
														});
														sb.Clear();
														currentTipNames.Clear();
													}
													else
													{
														result.Add(sb.ToString());
														resultTipNames.Add(new List<string>(currentTipNames));
														sb.Clear();
														currentTipNames.Clear();
														i--;
													}
												}
												IL_3EB:
												i++;
												continue;
												goto IL_3EB;
											}
											bool flag13 = sb.Length != 0;
											if (flag13)
											{
												result.Add(sb.ToString());
												resultTipNames.Add(currentTipNames);
											}
											measureHelper.OnParseComplete = savedParseComplete;
											this._consumeSplitLines = result;
											this._consumeSplitTipNames = resultTipNames;
											this.ApplyConsumeSplitLines();
										}
									}
								}
								else
								{
									bool flag14 = this.firstLineLeftLayout.preferredWidth <= 0f;
									if (!flag14)
									{
										this.secondLineLayout.gameObject.SetActive(false);
										for (int j = this.secondLineConsummeInfoParent.childCount - 1; j >= 0; j--)
										{
											Transform child = this.secondLineConsummeInfoParent.GetChild(j);
											bool flag15 = child == this.consummeInfo.transform;
											if (!flag15)
											{
												Object.Destroy(child.gameObject);
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

		// Token: 0x06008184 RID: 33156 RVA: 0x003C49D8 File Offset: 0x003C2BD8
		private void ParseConsumeInfoWithTips(TextMeshProUGUI textComp, IReadOnlyList<string> tipNames)
		{
			bool flag = textComp == null || tipNames == null || tipNames.Count == 0;
			if (!flag)
			{
				TMPTextSpriteHelper helper = textComp.GetComponent<TMPTextSpriteHelper>();
				bool flag2 = helper == null;
				if (!flag2)
				{
					helper.OnParseComplete = delegate()
					{
						bool flag3 = !this || !textComp;
						if (!flag3)
						{
							EventWindowOption.SetupConsumeIconTips(textComp, tipNames);
						}
					};
					helper.Parse();
				}
			}
		}

		// Token: 0x06008185 RID: 33157 RVA: 0x003C4A60 File Offset: 0x003C2C60
		private static void SetupConsumeIconTips(TextMeshProUGUI textComp, IReadOnlyList<string> tipNames)
		{
			bool flag = !textComp || tipNames == null || tipNames.Count == 0;
			if (!flag)
			{
				List<CImage> images = EventWindowOption.GetOrderedConsumeIcons(textComp);
				int count = Mathf.Min(tipNames.Count, images.Count);
				for (int i = 0; i < count; i++)
				{
					CImage image = images[i];
					bool flag2 = !image;
					if (!flag2)
					{
						string tipText = tipNames[i];
						bool flag3 = string.IsNullOrEmpty(tipText);
						if (!flag3)
						{
							image.raycastTarget = true;
							TooltipInvoker tip = image.gameObject.GetOrAddComponent<TooltipInvoker>();
							tip.enabled = true;
							tip.Type = TipType.SingleDesc;
							tip.triggerByChildRaycast = true;
							TooltipInvoker tooltipInvoker = tip;
							if (tooltipInvoker.RuntimeParam == null)
							{
								tooltipInvoker.RuntimeParam = EasyPool.Get<ArgumentBox>();
							}
							tip.RuntimeParam.Set("arg0", tipText);
						}
					}
				}
				for (int j = count; j < images.Count; j++)
				{
					TooltipInvoker tip2;
					bool flag4 = images[j] != null && images[j].TryGetComponent<TooltipInvoker>(out tip2);
					if (flag4)
					{
						tip2.enabled = false;
					}
				}
			}
		}

		// Token: 0x06008186 RID: 33158 RVA: 0x003C4BA4 File Offset: 0x003C2DA4
		private static List<CImage> GetOrderedConsumeIcons(TextMeshProUGUI textComp)
		{
			List<CImage> result = new List<CImage>();
			bool flag = !textComp;
			List<CImage> result2;
			if (flag)
			{
				result2 = result;
			}
			else
			{
				foreach (CImage image in textComp.transform.GetComponentsInTopChildren(true))
				{
					bool flag2 = image != null && image.gameObject.activeSelf;
					if (flag2)
					{
						result.Add(image);
					}
				}
				result.Sort(delegate(CImage a, CImage b)
				{
					bool flag3 = !a || !b;
					int result3;
					if (flag3)
					{
						result3 = 0;
					}
					else
					{
						result3 = a.rectTransform.localPosition.x.CompareTo(b.rectTransform.localPosition.x);
					}
					return result3;
				});
				result2 = result;
			}
			return result2;
		}

		// Token: 0x06008187 RID: 33159 RVA: 0x003C4C44 File Offset: 0x003C2E44
		public void Setup(EventOptionInfo optionInfo, string commandKey, int strMaxCount, sbyte taiwuBehaviorType, short behavior, Action<EventOptionInfo> selectOption)
		{
			EventWindowOption.<>c__DisplayClass39_0 CS$<>8__locals1 = new EventWindowOption.<>c__DisplayClass39_0();
			CS$<>8__locals1.selectOption = selectOption;
			CS$<>8__locals1.optionInfo = optionInfo;
			CS$<>8__locals1.<>4__this = this;
			EventWindowOption.<>c__DisplayClass39_0 CS$<>8__locals2 = CS$<>8__locals1;
			int num = this._layoutSetupGen + 1;
			this._layoutSetupGen = num;
			CS$<>8__locals2.setupGen = num;
			this.ResetConsumeLayoutState();
			TextMeshProUGUI hotKeyLabel = this.optionIndex;
			hotKeyLabel.text = (commandKey.IsNullOrEmpty() ? string.Empty : ("[" + commandKey.ToUpper() + "]"));
			LayoutElement layoutElement = this.hotKeyContainer;
			layoutElement.gameObject.SetActive(!commandKey.IsNullOrEmpty());
			this.importantSign.SetActive(CS$<>8__locals1.optionInfo.Important);
			sbyte formatBehavior = ViewEventWindow.OptionBehaviorToCharacterBehavior(CS$<>8__locals1.optionInfo.Behavior);
			bool optionBehaviorForbid = CS$<>8__locals1.optionInfo.Behavior != 0 && !GameData.Domains.Character.BehaviorType.IsCloseOrSame(formatBehavior, taiwuBehaviorType);
			List<OptionAvailableInfo> optionAvailableConditions = CS$<>8__locals1.optionInfo.OptionAvailableConditions;
			bool flag;
			if (optionAvailableConditions == null || optionAvailableConditions.Count <= 0)
			{
				List<OptionAvailableConditionInfo> optionAvailableConditionInfos = CS$<>8__locals1.optionInfo.OptionAvailableConditionInfos;
				flag = (optionAvailableConditionInfos != null && optionAvailableConditionInfos.Count > 0);
			}
			else
			{
				flag = true;
			}
			bool showHelp = flag;
			bool ignoreOptionBehavior = EventModel.IgnoreEventBehavior || !SingletonObject.getInstance<BasicGameData>().RestrictOptionsBehaviorType;
			bool flag2 = !ignoreOptionBehavior && optionBehaviorForbid;
			if (flag2)
			{
				showHelp = true;
			}
			bool hasHelpTips = false;
			string finalTipsDesc = "";
			bool flag3 = showHelp;
			if (flag3)
			{
				string orWord = LocalStringManager.Get(LanguageKey.LK_Event_Or);
				List<string> allAvailableInfoContents = EasyPool.Get<List<string>>();
				int i = 0;
				for (;;)
				{
					int num2 = i;
					List<OptionAvailableInfo> optionAvailableConditions2 = CS$<>8__locals1.optionInfo.OptionAvailableConditions;
					int? num3 = (optionAvailableConditions2 != null) ? new int?(optionAvailableConditions2.Count) : null;
					if (!(num2 < num3.GetValueOrDefault() & num3 != null))
					{
						break;
					}
					OptionAvailableInfo info = CS$<>8__locals1.optionInfo.OptionAvailableConditions[i];
					string start = LocalStringManager.Get(LanguageKey.LK_Dot_Symbol);
					List<string> cacheList = EasyPool.Get<List<string>>();
					for (int j = 0; j < info.Data.Length; j++)
					{
						OptionAvailableInfoMinimumElement element = info.Data[j];
						string conditionCellContent = this.Model.GetOptionConditionContent(element.ConditionId);
						string str = conditionCellContent;
						string[] formatArgs2 = element.FormatArgs;
						object[] array;
						if (formatArgs2 == null)
						{
							array = null;
						}
						else
						{
							array = formatArgs2.ChangeArrType((string e) => e);
						}
						conditionCellContent = str.GetFormat(array ?? Array.Empty<object>());
						conditionCellContent = ((!element.Pass) ? ("<color=#brightred>" + conditionCellContent + "</color>") : ("<color=#brightblue>" + conditionCellContent + "</color>"));
						bool flag4 = element.ConditionId != 4 || !element.Pass;
						if (flag4)
						{
							cacheList.Add(conditionCellContent);
						}
						hasHelpTips = true;
					}
					bool flag5 = cacheList.Count > 0;
					if (flag5)
					{
						allAvailableInfoContents.Add(start + string.Join(orWord, cacheList));
					}
					EasyPool.Free<List<string>>(cacheList);
					i++;
				}
				int k = 0;
				for (;;)
				{
					int num4 = k;
					List<OptionAvailableConditionInfo> optionAvailableConditionInfos2 = CS$<>8__locals1.optionInfo.OptionAvailableConditionInfos;
					int? num3 = (optionAvailableConditionInfos2 != null) ? new int?(optionAvailableConditionInfos2.Count) : null;
					if (!(num4 < num3.GetValueOrDefault() & num3 != null))
					{
						break;
					}
					OptionAvailableConditionInfo info2 = CS$<>8__locals1.optionInfo.OptionAvailableConditionInfos[k];
					string start2 = LocalStringManager.Get(LanguageKey.LK_Dot_Symbol);
					string inGameHint = EventFunction.Instance[info2.EventFunctionId].InGameHint;
					string[] args = info2.Args;
					string text2;
					if (args == null || args.Length <= 0)
					{
						text2 = inGameHint;
					}
					else
					{
						string str2 = inGameHint;
						object[] array2 = info2.Args;
						text2 = str2.GetFormat(array2);
					}
					string text = text2;
					allAvailableInfoContents.Add(info2.Pass ? (start2 + "<color=#brightblue>" + text + "</color>") : (start2 + "<color=#brightred>" + text + "</color>"));
					hasHelpTips = true;
					k++;
				}
				bool flag6 = optionBehaviorForbid;
				if (flag6)
				{
					string forbidReason = EventWindowOption.GetCharacterBehaviorConditionStr(formatBehavior, true);
					allAvailableInfoContents.Add(forbidReason);
					hasHelpTips = true;
				}
				bool flag7 = hasHelpTips;
				if (flag7)
				{
					finalTipsDesc = string.Join("\n", allAvailableInfoContents);
				}
				EasyPool.Free<List<string>>(allAvailableInfoContents);
			}
			short templateId = 0;
			bool hasTipContent = CS$<>8__locals1.optionInfo.OptionGuid != null && EventModel.EventOptionTipInfo.TryGetValue(CS$<>8__locals1.optionInfo.OptionGuid, out templateId);
			bool canShow = hasHelpTips || hasTipContent;
			this.optionHelp.gameObject.SetActive(canShow);
			bool flag8 = canShow;
			if (flag8)
			{
				CommonTipSimpleRuntime runtime = CommonTip.DefValue.EventOption.BuildSimple();
				bool flag9 = hasHelpTips && hasTipContent;
				if (flag9)
				{
					runtime.ShowParagraph("Space");
				}
				else
				{
					runtime.HideParagraph("Space");
				}
				bool flag10 = !hasHelpTips;
				if (flag10)
				{
					runtime.HideParagraph("Condition");
				}
				else
				{
					runtime.ShowParagraph("Condition");
					runtime.Set("EventOptionCondition", finalTipsDesc);
				}
				bool flag11 = !hasTipContent;
				if (flag11)
				{
					runtime.HideParagraph("Desc");
					runtime.Set("EventOptionTitle", LanguageKey.LK_Event_Need.Tr());
				}
				else
				{
					runtime.ShowParagraph("Desc");
					EventOptionTipsInfoItem config = EventOptionTipsInfo.Instance[(int)templateId];
					runtime.Set("EventOptionTitle", config.Title);
					runtime.Set("EventOptionDesc", config.Desc);
				}
				TooltipInvoker tooltipInvoker = this.optionHelp;
				if (tooltipInvoker.RuntimeParam == null)
				{
					tooltipInvoker.RuntimeParam = new ArgumentBox();
				}
				this.optionHelp.RuntimeParam.SetObject("Runtime", runtime);
				this.optionHelp.Type = TipType.CommonTip;
			}
			this.ProcessSpecialEventOption(CS$<>8__locals1.optionInfo);
			bool canSelect = CS$<>8__locals1.optionInfo.OptionState != -1 && (!optionBehaviorForbid || ignoreOptionBehavior);
			string optionDesc = CS$<>8__locals1.optionInfo.OptionContent;
			bool flag12 = CS$<>8__locals1.optionInfo.ExtraFormatLanguageKeys != null && CS$<>8__locals1.optionInfo.ExtraFormatLanguageKeys.Count > 0;
			if (flag12)
			{
				object[] array2 = CS$<>8__locals1.optionInfo.ExtraFormatLanguageKeys.ConvertAll<string>(new Converter<string, string>(LocalStringManager.Get)).ToArray();
				object[] formatArgs = array2;
				optionDesc = optionDesc.GetFormat(formatArgs);
			}
			bool hasBehavior = CS$<>8__locals1.optionInfo.Behavior != 0;
			this.behaviorIconContainer.SetActive(hasBehavior);
			bool flag13 = hasBehavior;
			if (flag13)
			{
				BehaviorTypeItem behaviorConfig = Config.BehaviorType.Instance.GetItem((short)formatBehavior);
				this.optionBehaviorIcon.SetSprite("ui9_icon_behavior_type_" + formatBehavior.ToString(), false, null);
				string[] behaviorTipsData = new string[]
				{
					behaviorConfig.Name + LocalStringManager.Get(LanguageKey.LK_Main_SummaryInfo_Behavior),
					LocalStringManager.Get(LanguageKey.UI_EventWindow_OptionBehavior_DiffTips)
				};
				bool flag14 = canSelect;
				if (flag14)
				{
					bool flag15 = optionBehaviorForbid;
					if (flag15)
					{
						optionDesc = optionDesc.SetColor(Colors.Instance["brightred"]);
					}
					else
					{
						bool flag16 = (short)formatBehavior == behavior;
						if (flag16)
						{
							optionDesc = optionDesc.SetColor(Colors.Instance["brightblue"]);
							behaviorTipsData[1] = LocalStringManager.Get(LanguageKey.UI_EventWindow_OptionBehavior_SameTips);
						}
						else
						{
							optionDesc = optionDesc.SetColor(Colors.Instance["supportyellow"]);
							behaviorTipsData[1] = LocalStringManager.Get(LanguageKey.UI_EventWindow_OptionBehavior_NearTips);
						}
					}
				}
				this.behaviorIconTips.PresetParam = behaviorTipsData;
				optionDesc = "「" + LocalStringManager.Get(ViewEventWindow.BehaviorNameKeyList[(int)formatBehavior]) + "」" + optionDesc;
			}
			CS$<>8__locals1.consumeInfoList = new List<ConsumeInfoDisplay>();
			List<OptionConsumeInfo> optionConsumeInfos = CS$<>8__locals1.optionInfo.OptionConsumeInfos;
			bool hasOptionConsumeInfos = optionConsumeInfos != null && optionConsumeInfos.Count > 0;
			this.consummeInfo.transform.gameObject.SetActive(hasOptionConsumeInfos);
			bool flag17 = hasOptionConsumeInfos;
			if (flag17)
			{
				for (int l = 0; l < CS$<>8__locals1.optionInfo.OptionConsumeInfos.Count; l++)
				{
					OptionConsumeInfo consumeInfo = CS$<>8__locals1.optionInfo.OptionConsumeInfos[l];
					CS$<>8__locals1.consumeInfoList.Add(new ConsumeInfoDisplay
					{
						Text = this.Model.GetOptionConsumeInfoTextMeshProSpriteString(consumeInfo),
						TipName = this.Model.GetOptionConsumeInfoDisplayName(consumeInfo.ConsumeType)
					});
				}
				this._consumeInfoStr = string.Join("    ", CS$<>8__locals1.consumeInfoList.ConvertAll<string>((ConsumeInfoDisplay e) => e.Text)).ColorReplace();
				this.consummeInfo.SetText(this._consumeInfoStr, true);
				this.ParseConsumeInfoWithTips(this.consummeInfo, CS$<>8__locals1.consumeInfoList.ConvertAll<string>((ConsumeInfoDisplay e) => e.TipName));
			}
			else
			{
				this._consumeInfoStr = string.Empty;
				this.consummeInfo.SetText(string.Empty, true);
				TMPTextSpriteHelper helper = this.consummeInfo.GetComponent<TMPTextSpriteHelper>();
				bool flag18 = helper != null;
				if (flag18)
				{
					helper.OnParseComplete = null;
				}
			}
			this.readStateContainer.SetActive(CS$<>8__locals1.optionInfo.OptionState == 1 || CS$<>8__locals1.optionInfo.OptionState == 2);
			this.readObject.SetActive(CS$<>8__locals1.optionInfo.OptionState == 2);
			this.unReadObject.SetActive(CS$<>8__locals1.optionInfo.OptionState == 1);
			this.btnMain.interactable = canSelect;
			TextMeshProUGUI contentLabel = this.txtOptionDesc;
			bool flag19 = !canSelect;
			if (flag19)
			{
				string info3 = "<color=#grey>" + optionDesc + "</color>";
				contentLabel.text = info3.ColorReplace();
			}
			else
			{
				contentLabel.text = optionDesc.ColorReplace();
			}
			contentLabel.GetComponent<TMPTextSpriteHelper>().Parse();
			this.btnMain.ClearAndAddListener(delegate
			{
				Action<EventOptionInfo> selectOption2 = CS$<>8__locals1.selectOption;
				if (selectOption2 != null)
				{
					selectOption2(CS$<>8__locals1.optionInfo);
				}
			});
			this.CheckLayout(CS$<>8__locals1.consumeInfoList, CS$<>8__locals1.setupGen);
			SingletonObject.getInstance<YieldHelper>().DelayFrameDo(2U, delegate
			{
				CS$<>8__locals1.<>4__this.CheckLayout(CS$<>8__locals1.consumeInfoList, CS$<>8__locals1.setupGen);
			});
			SingletonObject.getInstance<YieldHelper>().DelayFrameDo(4U, delegate
			{
				CS$<>8__locals1.<>4__this.CheckLayout(CS$<>8__locals1.consumeInfoList, CS$<>8__locals1.setupGen);
			});
		}

		// Token: 0x06008188 RID: 33160 RVA: 0x003C5688 File Offset: 0x003C3888
		private void ProcessSpecialEventOption(EventOptionInfo info)
		{
			CharacterDisplayData displayData = this.Data.TargetCharacter;
			TooltipInvoker mouseTipDisplayer = this.specialOptionIcon.GetComponent<TooltipInvoker>();
			bool flag = info.OptionType == 4;
			if (flag)
			{
				this.specialOptionIcon.gameObject.SetActive(true);
				bool flag2 = displayData != null && displayData.AvatarRelatedData.HasNewGoods;
				if (flag2)
				{
					this.specialOptionIcon.SetSprite("blockchar_icon_shanghui_2", false, null);
					this.specialOptionText.text = LocalStringManager.Get(LanguageKey.LK_Word_New);
					mouseTipDisplayer.enabled = true;
					mouseTipDisplayer.PresetParam = new string[]
					{
						LocalStringManager.Get(LanguageKey.LK_NewGoods_TipTitle),
						LocalStringManager.Get(LanguageKey.LK_NewGoods_TipContent)
					};
				}
				else
				{
					this.specialOptionIcon.SetSprite("sp_icon_shanghui", false, null);
					this.specialOptionText.text = string.Empty;
					mouseTipDisplayer.enabled = false;
				}
			}
			else
			{
				this.specialOptionIcon.gameObject.SetActive(false);
			}
		}

		// Token: 0x06008189 RID: 33161 RVA: 0x003C5784 File Offset: 0x003C3984
		public static string GetCharacterBehaviorConditionStr(sbyte formatBehavior, bool hasDot = true)
		{
			StringBuilder sb = EasyPool.Get<StringBuilder>();
			if (hasDot)
			{
				sb.Append(LocalStringManager.Get(LanguageKey.LK_Dot_Symbol));
			}
			sb.Append(LocalStringManager.Get(LanguageKey.LK_OptionNeedBehavior));
			int startIndex = Mathf.Max((int)(formatBehavior - 1), 0);
			int endIndex = Mathf.Min((int)(formatBehavior + 1), 4);
			for (int index = startIndex; index <= endIndex; index++)
			{
				string behaviorName = LocalStringManager.Get(ViewEventWindow.BehaviorNameKeyList[index]).ColorReplace();
				sb.Append(behaviorName);
				bool flag = index < endIndex;
				if (flag)
				{
					sb.Append(LocalStringManager.Get(LanguageKey.LK_Split_Symbol));
				}
			}
			string forbidReason = sb.ToString();
			EasyPool.Free<StringBuilder>(sb);
			return forbidReason;
		}

		// Token: 0x040062EE RID: 25326
		[SerializeField]
		private RectTransform rect;

		// Token: 0x040062EF RID: 25327
		[SerializeField]
		private GameObject secondLineLayout;

		// Token: 0x040062F0 RID: 25328
		[SerializeField]
		private Transform secondLineConsummeInfoParent;

		// Token: 0x040062F1 RID: 25329
		[SerializeField]
		private Transform firstLineConsummeInfoParent;

		// Token: 0x040062F2 RID: 25330
		[SerializeField]
		private HorizontalLayoutGroup firstLineLeftLayout;

		// Token: 0x040062F3 RID: 25331
		[Header("重要选项底 （此处为特效，暂无）")]
		[SerializeField]
		private GameObject importantSign;

		// Token: 0x040062F4 RID: 25332
		[Header("原Refers")]
		[SerializeField]
		private GameObject readStateContainer;

		// Token: 0x040062F5 RID: 25333
		[SerializeField]
		private GameObject readObject;

		// Token: 0x040062F6 RID: 25334
		[SerializeField]
		private GameObject unReadObject;

		// Token: 0x040062F7 RID: 25335
		[SerializeField]
		private TextMeshProUGUI optionIndex;

		// Token: 0x040062F8 RID: 25336
		[SerializeField]
		private LayoutElement hotKeyContainer;

		// Token: 0x040062F9 RID: 25337
		[SerializeField]
		private TooltipInvoker optionHelp;

		// Token: 0x040062FA RID: 25338
		[SerializeField]
		private GameObject behaviorIconContainer;

		// Token: 0x040062FB RID: 25339
		[SerializeField]
		private CImage optionBehaviorIcon;

		// Token: 0x040062FC RID: 25340
		[SerializeField]
		private CImage specialOptionIcon;

		// Token: 0x040062FD RID: 25341
		[SerializeField]
		private TextMeshProUGUI specialOptionText;

		// Token: 0x040062FE RID: 25342
		[SerializeField]
		private TooltipInvoker behaviorIconTips;

		// Token: 0x040062FF RID: 25343
		[SerializeField]
		private TextMeshProUGUI consummeInfo;

		// Token: 0x04006300 RID: 25344
		[SerializeField]
		private CButton btnMain;

		// Token: 0x04006301 RID: 25345
		[SerializeField]
		private TextMeshProUGUI txtOptionDesc;

		// Token: 0x04006302 RID: 25346
		private const string _consumeInfoSpace = "    ";

		// Token: 0x04006303 RID: 25347
		private string _consumeInfoStr;

		// Token: 0x04006304 RID: 25348
		private bool _consumeUseSecondLine;

		// Token: 0x04006305 RID: 25349
		private List<string> _consumeSplitLines;

		// Token: 0x04006306 RID: 25350
		private List<List<string>> _consumeSplitTipNames;

		// Token: 0x04006307 RID: 25351
		private int _layoutSetupGen;

		// Token: 0x04006308 RID: 25352
		private EventModel _model;
	}
}
