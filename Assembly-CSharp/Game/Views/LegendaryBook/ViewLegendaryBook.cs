using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Config;
using Config.ConfigCells.Character;
using DG.Tweening;
using FrameWork;
using FrameWork.UISystem.UIElements;
using Game.Components.Avatar;
using Game.Components.Common;
using Game.Views.Select;
using GameData.Common;
using GameData.Domains.Character;
using GameData.Domains.CombatSkill;
using GameData.Domains.Extra;
using GameData.Domains.Global;
using GameData.Domains.Item;
using GameData.Domains.Item.Display;
using GameData.Domains.LegendaryBook;
using GameData.Domains.Taiwu;
using GameData.GameDataBridge;
using GameData.Serializer;
using GameData.Utilities;
using TMPro;
using UICommon.Character;
using UnityEngine;

namespace Game.Views.LegendaryBook
{
	// Token: 0x0200098E RID: 2446
	public class ViewLegendaryBook : UIBase
	{
		// Token: 0x17000D44 RID: 3396
		// (get) Token: 0x060075BA RID: 30138 RVA: 0x0036D44B File Offset: 0x0036B64B
		private bool _limitionActive
		{
			get
			{
				return SingletonObject.getInstance<BasicGameData>().ChallengeModeData.IsEnabled(EChallengeModeImplement.LegendaryBookLimitation);
			}
		}

		// Token: 0x17000D45 RID: 3397
		// (get) Token: 0x060075BB RID: 30139 RVA: 0x0036D45D File Offset: 0x0036B65D
		private bool _currentInNodePage
		{
			get
			{
				return this.pageNodeView.gameObject.activeSelf;
			}
		}

		// Token: 0x17000D46 RID: 3398
		// (get) Token: 0x060075BC RID: 30140 RVA: 0x0036D46F File Offset: 0x0036B66F
		private bool AnySkillTypeSelected
		{
			get
			{
				return this.skillTypeTogGroupBookPage.CurrentSelected >= 0;
			}
		}

		// Token: 0x060075BD RID: 30141 RVA: 0x0036D484 File Offset: 0x0036B684
		public override void OnInit(ArgumentBox argsBox)
		{
			this._taiwuCharId = SingletonObject.getInstance<BasicGameData>().TaiwuCharId;
			bool flag = this._characterAvatarYin == null;
			if (flag)
			{
				this._characterAvatarYin = new CharacterAvatar(this.refersYin.CGet<Game.Components.Avatar.Avatar>("AvatarSmall"), true);
			}
			bool flag2 = this._characterAvatarYang == null;
			if (flag2)
			{
				this._characterAvatarYang = new CharacterAvatar(this.refersYang.CGet<Game.Components.Avatar.Avatar>("AvatarSmall"), true);
			}
			this._characterAvatarYin.CharacterId = this._taiwuCharId;
			this._characterAvatarYang.CharacterId = this._taiwuCharId;
			UIElement element = this.Element;
			element.OnListenerIdReady = (Action)Delegate.Combine(element.OnListenerIdReady, new Action(this.OnListenerIdReady));
			this.btnReset.gameObject.SetActive(this._limitionActive);
		}

		// Token: 0x060075BE RID: 30142 RVA: 0x0036D558 File Offset: 0x0036B758
		private void Awake()
		{
			this.pageBookView.SetActive(true);
			this.pageNodeView.SetActive(false);
			this.btnClose.ClearAndAddListener(delegate
			{
				this.QuickHide();
			});
			this.skillTypeTogGroupBookPage.CheckCanSelect = new Func<int, bool>(this.CheckCanSelect);
			this.skillTypeTogGroupNodePage.CheckCanSelect = new Func<int, bool>(this.CheckCanSelect);
			this._onCharactersPageClose = delegate()
			{
			};
			this.btnFallen.ClearAndAddListener(delegate
			{
				List<int> list = new List<int>();
				list.AddRange(this._bookData.ShockedList);
				list.AddRange(this._bookData.InsaneList);
				list.AddRange(this._bookData.ConsumedList);
				List<LegendaryBookCharacterRelatedData> dataList = (from t in list
				where this._bookData.CharacterMap.ContainsKey(t)
				select this._bookData.CharacterMap[t]).ToList<LegendaryBookCharacterRelatedData>();
				ArgumentBox argBox = EasyPool.Get<ArgumentBox>();
				argBox.Set("IsCompetitorMode", false);
				argBox.SetObject("CharDataList", dataList);
				argBox.SetObject("OnClose", this._onCharactersPageClose);
				this.effBookView.SetActive(false);
				UIElement.LegendaryBookCharacters.SetOnInitArgs(argBox);
				UIManager.Instance.ShowUI(UIElement.LegendaryBookCharacters, true);
			});
			this.presetListComponent.Setup(delegate()
			{
				LegendaryBookDomainMethod.Call.AddLegendaryBookSkillEmptyPreset();
				this.GetPresetData();
			}, delegate()
			{
				LegendaryBookDomainMethod.Call.DuplicateLegendaryBookSkillPreset((sbyte)this.presetListComponent.CurrentSelectedIndex);
				this.GetPresetData();
			}, delegate()
			{
				LegendaryBookDomainMethod.Call.ResetLegendaryBookSkillPreset((int)((sbyte)this.presetListComponent.CurrentSelectedIndex));
				this.GetPresetData();
			}, delegate()
			{
				LegendaryBookDomainMethod.Call.RemoveLegendaryBookSkillPreset((sbyte)this.presetListComponent.CurrentSelectedIndex);
				this.GetPresetData();
			}, delegate(int _, int _)
			{
				LegendaryBookDomainMethod.Call.SetLegendaryBookSkillPreset((sbyte)this.presetListComponent.CurrentSelectedIndex);
			});
			this.btnCompetitors.ClearAndAddListener(delegate
			{
				List<LegendaryBookCharacterRelatedData> dataList = (from t in this._bookData.ContestList
				where this._bookData.CharacterMap.ContainsKey(t)
				select this._bookData.CharacterMap[t]).ToList<LegendaryBookCharacterRelatedData>();
				ArgumentBox argBox = EasyPool.Get<ArgumentBox>();
				argBox.Set("IsCompetitorMode", true);
				argBox.SetObject("CharDataList", dataList);
				argBox.SetObject("OnClose", this._onCharactersPageClose);
				this.effBookView.SetActive(false);
				UIElement.LegendaryBookCharacters.SetOnInitArgs(argBox);
				UIManager.Instance.ShowUI(UIElement.LegendaryBookCharacters, true);
			});
			this.switchTempObj.gameObject.SetActive(false);
			LegendaryBookUIButtonGroups legendaryBookUIButtonGroups = this.skillTypeTogGroupBookPage;
			legendaryBookUIButtonGroups.onActiveIndexChange = (Action<int, int>)Delegate.Combine(legendaryBookUIButtonGroups.onActiveIndexChange, new Action<int, int>(this.OnSkillTypeTogChange));
			LegendaryBookUIButtonGroups legendaryBookUIButtonGroups2 = this.skillTypeTogGroupNodePage;
			legendaryBookUIButtonGroups2.onActiveIndexChange = (Action<int, int>)Delegate.Combine(legendaryBookUIButtonGroups2.onActiveIndexChange, new Action<int, int>(this.OnSkillTypeTogChange));
			for (int type = 0; type < 14; type++)
			{
				LegendaryBookUIItem togCompBookPage = this.skillTypeTogGroupBookPage.GetByIndex(type).GetComponent<LegendaryBookUIItem>();
				LegendaryBookUIItem togCompNodePage = this.skillTypeTogGroupNodePage.GetByIndex(type).GetComponent<LegendaryBookUIItem>();
				this.SetupBookItem(togCompBookPage, type, false);
				this.SetupBookItem(togCompNodePage, type, true);
			}
			this.InitYinyangRefers(this.refersYin, true);
			this.InitYinyangRefers(this.refersYang, false);
			this._selectSkillArgBox.Set("ShowLifeSkill", false);
			this._selectSkillArgBox.Set("ShowCombatSkill", true);
			this._selectSkillArgBox.Set("ShowQualificationAdd", false);
			this._selectSkillArgBox.Set("ShowNone", true);
			this._selectSkillArgBox.SetObject("UnselectableCombatSkillList", new List<short>());
			this._selectSkillArgBox.SetObject("Callback", new Action<sbyte, short>(this.OnSelectSkill));
			this._confirmUnlockBonusDialogCmd.Type = 1;
			this._confirmUnlockBonusDialogCmd.Title = LocalStringManager.Get(LanguageKey.LK_LegendaryBook_Bonus_Unlock_Tips_Title);
			this._confirmUnlockBonusDialogCmd.Yes = delegate()
			{
				ExtraDomainMethod.Call.UnlockLegendaryBookBonus(this._currCombatSkillType, this._isYinBonusWaitingConfirm);
			};
			this.btnReset.ClearAndAddListener(new Action(this.OnClickReset));
		}

		// Token: 0x060075BF RID: 30143 RVA: 0x0036D7F8 File Offset: 0x0036B9F8
		private bool CheckCanSelect(int index)
		{
			return this.bookItemStatus[index] == ViewLegendaryBook.ELegendaryBookStatus.Owning || this.bookItemStatus[index] == ViewLegendaryBook.ELegendaryBookStatus.keepByCorpses;
		}

		// Token: 0x060075C0 RID: 30144 RVA: 0x0036D82B File Offset: 0x0036BA2B
		private void OnClickReset()
		{
			CommonUtils.ShowConfirmDialog(LanguageKey.LK_LegendaryBook_ResetBonusBodes.Tr(), LanguageKey.LK_LegendaryBook_ResetBonusBodesContent.Tr(), delegate
			{
				LegendaryBookDomainMethod.Call.ResetLegendaryBookBonus(this._currCombatSkillType, true);
				LegendaryBookDomainMethod.Call.ResetLegendaryBookBonus(this._currCombatSkillType, false);
			}, null, EDialogType.None);
		}

		// Token: 0x060075C1 RID: 30145 RVA: 0x0036D858 File Offset: 0x0036BA58
		private void OnHoverFallenButton(bool isHover)
		{
			this.ResetBookHint();
			bool flag = !isHover;
			if (flag)
			{
				this.RefreshNodePageBookItemParents(null);
			}
			else
			{
				this.hoverBlocker.gameObject.SetActive(true);
				HashSet<int> bookIndex = new HashSet<int>();
				foreach (int charId in this._bookData.InsaneList)
				{
					int targetIndex = this._bookOwners.IndexOf(charId);
					bool flag2 = targetIndex < 0;
					if (!flag2)
					{
						bookIndex.Add(targetIndex);
					}
				}
				foreach (int charId2 in this._bookData.ShockedList)
				{
					int targetIndex2 = this._bookOwners.IndexOf(charId2);
					bool flag3 = targetIndex2 < 0;
					if (!flag3)
					{
						bookIndex.Add(targetIndex2);
					}
				}
				foreach (int charId3 in this._bookData.ConsumedList)
				{
					int targetIndex3 = this._bookOwners.IndexOf(charId3);
					bool flag4 = targetIndex3 < 0;
					if (!flag4)
					{
						bookIndex.Add(targetIndex3);
					}
				}
				foreach (int targetIndex4 in bookIndex)
				{
					bool currentInNodePage = this._currentInNodePage;
					if (currentInNodePage)
					{
						LegendaryBookUIItem itemComp2 = this.skillTypeTogGroupNodePage.GetItem(targetIndex4);
						itemComp2.transform.parent = this.hoverBlocker;
						itemComp2.ShowHint(false, LanguageKey.LK_LegendaryBook_OwnerFallen.Tr(), 22);
					}
					else
					{
						LegendaryBookUIItem itemComp3 = this.skillTypeTogGroupBookPage.GetItem(targetIndex4);
						itemComp3.transform.parent = this.hoverBlocker;
						itemComp3.ShowHint(false, LanguageKey.LK_LegendaryBook_OwnerFallen.Tr(), 22);
					}
				}
				this.RefreshNodePageBookItemParents(bookIndex);
			}
		}

		// Token: 0x060075C2 RID: 30146 RVA: 0x0036DAAC File Offset: 0x0036BCAC
		private void RefreshNodePageBookItemParents(HashSet<int> hoverBlockerHighlightIndices = null)
		{
			bool flag = !this._currentInNodePage;
			if (!flag)
			{
				bool inListButtonHover = hoverBlockerHighlightIndices != null;
				sbyte i = 0;
				while ((int)i < this.skillTypeTogGroupNodePage.Count)
				{
					bool flag2 = inListButtonHover && hoverBlockerHighlightIndices.Contains((int)i);
					if (!flag2)
					{
						ViewLegendaryBook.ELegendaryBookStatus status;
						bool flag3 = this.bookItemStatus.TryGetValue((int)i, out status) && status == ViewLegendaryBook.ELegendaryBookStatus.Owning;
						if (flag3)
						{
							bool flag4 = inListButtonHover;
							if (flag4)
							{
								this.skillTypeTogGroupNodePage.ResetItemParent((int)i);
							}
							else
							{
								this.skillTypeTogGroupNodePage.GetByIndex((int)i).transform.SetParent(this.nodeViewCoverBg);
							}
						}
						else
						{
							this.skillTypeTogGroupNodePage.ResetItemParent((int)i);
						}
					}
					i += 1;
				}
			}
		}

		// Token: 0x060075C3 RID: 30147 RVA: 0x0036DB6C File Offset: 0x0036BD6C
		private void OnHoverCompetitorButton(bool isHover)
		{
			this.ResetBookHint();
			bool flag = !isHover;
			if (flag)
			{
				this.RefreshNodePageBookItemParents(null);
			}
			else
			{
				this.hoverBlocker.gameObject.SetActive(true);
				HashSet<int> highlightIndices = new HashSet<int>();
				from charId in this._bookData.ContestList
				group charId by this._bookData.CharacterMap[charId].BookType;
				foreach (KeyValuePair<sbyte, IGrouping<sbyte, int>> item in (from charId in this._bookData.ContestList
				group charId by this._bookData.CharacterMap[charId].BookType).ToDictionary((IGrouping<sbyte, int> t) => t.Key))
				{
					bool flag2 = item.Value.Count<int>() > 0;
					if (flag2)
					{
						highlightIndices.Add((int)item.Key);
						bool currentInNodePage = this._currentInNodePage;
						if (currentInNodePage)
						{
							LegendaryBookUIItem itemComp2 = this.skillTypeTogGroupNodePage.GetItem((int)item.Key);
							itemComp2.transform.parent = this.hoverBlocker;
							itemComp2.ShowHint(true, item.Value.Count<int>().ToString(), 24);
						}
						else
						{
							LegendaryBookUIItem itemComp3 = this.skillTypeTogGroupBookPage.GetItem((int)item.Key);
							itemComp3.transform.parent = this.hoverBlocker;
							itemComp3.ShowHint(true, item.Value.Count<int>().ToString(), 24);
						}
					}
				}
				this.RefreshNodePageBookItemParents(highlightIndices);
			}
		}

		// Token: 0x060075C4 RID: 30148 RVA: 0x0036DD20 File Offset: 0x0036BF20
		private void ResetBookHint()
		{
			this.hoverBlocker.gameObject.SetActive(false);
			this.skillTypeTogGroupBookPage.ResetItemsHint();
			this.skillTypeTogGroupNodePage.ResetItemsHint();
		}

		// Token: 0x060075C5 RID: 30149 RVA: 0x0036DD50 File Offset: 0x0036BF50
		public override void QuickHide()
		{
			AudioManager.Instance.PlaySound("ui_default_click_left", false, false);
			bool activeSelf = this.blocker.activeSelf;
			if (!activeSelf)
			{
				base.QuickHide();
			}
		}

		// Token: 0x060075C6 RID: 30150 RVA: 0x0036DD8C File Offset: 0x0036BF8C
		private void SetupBookItem(LegendaryBookUIItem togComp, int type, bool nodePage)
		{
			MiscItem miscConfig = Misc.Instance[240 + type];
			string skillTypeName = Config.CombatSkillType.Instance[type].Name;
			string bookName = skillTypeName + LanguageKey.LK_Dot_Symbol.Tr() + LocalStringManager.Get(string.Format("LK_LegendaryBook_{0}", type));
			PointerTrigger pointerTrigger = togComp.pointerTrigger;
			pointerTrigger.IgnoreOnDisableTrigger = true;
			togComp.Init(miscConfig, bookName);
			togComp.bookNameEnable.text = bookName;
			togComp.bookNameDisable.text = bookName;
			togComp.bookNameDisable.gameObject.SetActive(false);
			togComp.icon.SetSprite(miscConfig.Icon, false, null);
			sbyte currType = (sbyte)type;
			LegendaryBookUIButtonGroups legendaryBookUIButtonGroups = nodePage ? this.skillTypeTogGroupNodePage : this.skillTypeTogGroupBookPage;
			pointerTrigger.EnterEvent.AddListener(delegate()
			{
				bool kept2 = (this._keptBook & 1 << (int)currType) != 0;
				this.UpdateExtraButtonStatus();
				bool flag2 = this._bookData == null;
				if (!flag2)
				{
					bool flag3 = this._currentInNodePage && this.bookItemStatus[(int)currType] != ViewLegendaryBook.ELegendaryBookStatus.NotShown && this.bookItemStatus[(int)currType] != ViewLegendaryBook.ELegendaryBookStatus.Owning;
					if (flag3)
					{
						this.skillTypeTogGroupNodePage.GetByIndex((int)currType).transform.SetParent(this.nodeViewCoverBg);
					}
				}
			});
			Action <>9__2;
			pointerTrigger.ExitEvent.AddListener(delegate()
			{
				bool kept2 = (this._keptBook & 1 << (int)currType) != 0;
				YieldHelper instance = SingletonObject.getInstance<YieldHelper>();
				uint frame = 1U;
				Action job;
				if ((job = <>9__2) == null)
				{
					job = (<>9__2 = delegate()
					{
						bool flag2 = this == null || this.skillTypeTogGroupNodePage == null || this.bookItemStatus == null;
						if (!flag2)
						{
							bool flag3 = this._currentInNodePage && this.bookItemStatus[(int)currType] != ViewLegendaryBook.ELegendaryBookStatus.Owning;
							if (flag3)
							{
								this.skillTypeTogGroupNodePage.ResetItemParent((int)currType);
							}
						}
					});
				}
				instance.DelayFrameDo(frame, job);
			});
			TooltipInvoker mouseTips = togComp.mouseTip;
			bool kept = (this._keptBook & 1 << (int)currType) != 0;
			bool flag = this._bookData != null && this._bookOwners[(int)currType] != this._taiwuCharId && !kept;
			if (flag)
			{
				mouseTips.enabled = true;
				mouseTips.Type = TipType.LegendaryBookPageItem;
				TooltipInvoker tooltipInvoker = mouseTips;
				if (tooltipInvoker.RuntimeParam == null)
				{
					tooltipInvoker.RuntimeParam = new ArgumentBox();
				}
				mouseTips.RuntimeParam.SetObject("bookData", this._bookData);
				mouseTips.RuntimeParam.SetObject("bookOwners", this._bookOwners);
				mouseTips.RuntimeParam.Set("currType", currType);
			}
		}

		// Token: 0x060075C7 RID: 30151 RVA: 0x0036DF5C File Offset: 0x0036C15C
		private void OnEnable()
		{
			GlobalDomainMethod.Call.InvokeGuidingTrigger(110);
			CanvasGroup togCanvasGroup = this.skillTypeTogGroupBookPage.GetComponent<CanvasGroup>();
			togCanvasGroup.DOKill(false);
			togCanvasGroup.alpha = 0f;
			togCanvasGroup.DOFade(1f, 0.2f);
		}

		// Token: 0x060075C8 RID: 30152 RVA: 0x0036DFA3 File Offset: 0x0036C1A3
		private void OnDisable()
		{
			AudioManager.Instance.StopSound("legendbook_ambience");
			this.ResetBookHint();
		}

		// Token: 0x060075C9 RID: 30153 RVA: 0x0036DFC0 File Offset: 0x0036C1C0
		private void OnListenerIdReady()
		{
			CharacterDomainMethod.Call.GetAllInventoryItems(this.Element.GameDataListenerId, this._taiwuCharId);
			CharacterDomainMethod.Call.GetAllEquipmentItems(this.Element.GameDataListenerId, this._taiwuCharId);
			TaiwuDomainMethod.Call.GetAllWarehouseItems(this.Element.GameDataListenerId);
			LegendaryBookDomainMethod.Call.GetLegendaryBookIncrementData(this.Element.GameDataListenerId);
			this.GetPresetData();
			bool waitingCombatEnd = this._waitingCombatEnd;
			if (waitingCombatEnd)
			{
				this._waitingCombatEnd = false;
			}
			else
			{
				this.ResetUiState();
			}
			this.Element.ShowAfterRefresh();
		}

		// Token: 0x060075CA RID: 30154 RVA: 0x0036E04B File Offset: 0x0036C24B
		private void GetPresetData()
		{
			LegendaryBookDomainMethod.AsyncCall.GetLegendaryBookPresetDisplayData(null, delegate(int offset, RawDataPool dataPool)
			{
				LegendaryBookPresetDisplayData data = new LegendaryBookPresetDisplayData();
				Serializer.Deserialize(dataPool, offset, ref data);
				this.presetListComponent.Setup(data);
			});
		}

		// Token: 0x060075CB RID: 30155 RVA: 0x0036E064 File Offset: 0x0036C264
		private void InitYinyangRefers(Refers refers, bool isYin)
		{
			ViewLegendaryBook.<>c__DisplayClass89_0 CS$<>8__locals1 = new ViewLegendaryBook.<>c__DisplayClass89_0();
			CS$<>8__locals1.<>4__this = this;
			CS$<>8__locals1.isYin = isYin;
			CButton unlockBreakPlate = refers.CGet<CButton>("UnlockBreakPlate");
			RectTransform bonusHolder = refers.CGet<RectTransform>("BonusHolder");
			RectTransform effectHolder = refers.CGet<RectTransform>("EffectHolder");
			LanguageKey key = CS$<>8__locals1.isYin ? LanguageKey.LK_LegendaryBook_Break_Plate_Tips_Yin : LanguageKey.LK_LegendaryBook_Break_Plate_Tips_Yang;
			refers.CGet<TooltipInvoker>("BreakPlate").PresetParam[1] = key.Tr().ColorReplace();
			refers.CGet<TooltipInvoker>("MousetipNotUnlock").PresetParam[1] = key.Tr().ColorReplace();
			unlockBreakPlate.ClearAndAddListener(delegate
			{
				ViewLegendaryBook.<>c__DisplayClass89_1 CS$<>8__locals3 = new ViewLegendaryBook.<>c__DisplayClass89_1();
				CS$<>8__locals3.CS$<>8__locals1 = CS$<>8__locals1;
				CS$<>8__locals1.<>4__this._isYinBreakPlateWaitingConfirm = CS$<>8__locals1.isYin;
				CS$<>8__locals3.timeCost = GlobalConfig.Instance.LegendaryBookUnlockBreakPlateTime;
				UIElement.ViewConfirmDialogLayoutSmall.SetOnInitArgs(EasyPool.Get<ArgumentBox>().SetObject("Cmd", new ConfirmDialogCmd
				{
					Title = LanguageKey.LK_LegendaryBook_Break_Plate_Unlock_Title.Tr(),
					ContentUpper = (CS$<>8__locals1.isYin ? LanguageKey.LK_LegendaryBook_Break_Plate_Unlock_Content_Yin : LanguageKey.LK_LegendaryBook_Break_Plate_Unlock_Content_Yang).Tr(),
					ConfirmDialogCost = new List<ConfirmDialogCost>
					{
						new ConfirmDialogCost
						{
							Type = EConfirmDialogCostType.ActionPoint,
							ValueCost = CS$<>8__locals3.timeCost,
							ValueHave = SingletonObject.getInstance<TimeManager>().GetRemainingActionPointConvertToDays()
						}
					},
					Yes = new Action(CS$<>8__locals3.<InitYinyangRefers>g__OnConfirmUnlockBreakPlate|1)
				}));
				UIManager.Instance.MaskUI(UIElement.ViewConfirmDialogLayoutSmall);
			});
			for (int i = 0; i < bonusHolder.childCount; i++)
			{
				LegendaryBookBonusItem bonusRefers = bonusHolder.GetChild(i).GetComponent<LegendaryBookBonusItem>();
				CButton bonusBtn = bonusRefers.GetComponent<CButton>();
				bonusRefers.GetComponent<TooltipInvoker>().RuntimeParam = new ArgumentBox();
				CButton cbutton = bonusBtn;
				Action action;
				if ((action = CS$<>8__locals1.<>9__3) == null)
				{
					action = (CS$<>8__locals1.<>9__3 = delegate()
					{
						CS$<>8__locals1.<>4__this._isYinBonusWaitingConfirm = CS$<>8__locals1.isYin;
						CS$<>8__locals1.<>4__this._confirmUnlockBonusDialogCmd.Content = LocalStringManager.GetFormat(LanguageKey.LK_LegendaryBook_Bonus_Unlock_Tips_Content, CS$<>8__locals1.<>4__this._unlockNeedExp).ColorReplace();
						bool limitionActive = CS$<>8__locals1.<>4__this._limitionActive;
						if (limitionActive)
						{
							SByteList bonusCount = CS$<>8__locals1.isYin ? CS$<>8__locals1.<>4__this._legendaryBookBonusCountYin : CS$<>8__locals1.<>4__this._legendaryBookBonusCountYang;
							int unlockedCount = (int)((bonusCount.Items != null && bonusCount.Items.Count > 0 && CS$<>8__locals1.<>4__this._ownedCurrBook) ? bonusCount.Items[(int)CS$<>8__locals1.<>4__this._currCombatSkillType] : 0);
							bool flag = unlockedCount == 0;
							if (flag)
							{
								string title = LanguageKey.LK_LegendaryBook_LimitionTitle.Tr();
								string text = LanguageKey.LK_LegendaryBook_LimitionContent.Tr();
								Action onConfirm;
								if ((onConfirm = CS$<>8__locals1.<>9__5) == null)
								{
									onConfirm = (CS$<>8__locals1.<>9__5 = delegate()
									{
										base.<InitYinyangRefers>g__ShowDialog|4();
									});
								}
								CommonUtils.ShowConfirmDialog(title, text, onConfirm, null, EDialogType.None);
							}
							else
							{
								base.<InitYinyangRefers>g__ShowDialog|4();
							}
						}
						else
						{
							base.<InitYinyangRefers>g__ShowDialog|4();
						}
					});
				}
				cbutton.ClearAndAddListener(action);
			}
			for (int j = 0; j < effectHolder.childCount; j++)
			{
				LegendaryBookEffectItem effectRefers = effectHolder.GetChild(j).GetComponent<LegendaryBookEffectItem>();
				CButton effectBtn = effectRefers.BtnMain;
				PointerTrigger pointerTrigger = effectRefers.GetComponent<PointerTrigger>();
				GameObject highlight = effectRefers.Highlight.gameObject;
				effectBtn.ClearAndAddListener(delegate
				{
					CS$<>8__locals1.<>4__this._skillSlotIndex = effectRefers.UserInt;
					bool flag = CS$<>8__locals1.<>4__this._skillSlotIndex >= 0;
					if (flag)
					{
						CS$<>8__locals1.<>4__this.ShowSelectSkill();
					}
					else
					{
						CS$<>8__locals1.<>4__this.ShowSelectItem();
					}
				});
				pointerTrigger.EnterEvent.AddListener(delegate()
				{
					bool interactable = effectBtn.interactable;
					if (interactable)
					{
						highlight.SetActive(true);
					}
				});
				pointerTrigger.ExitEvent.AddListener(delegate()
				{
					highlight.SetActive(false);
				});
			}
		}

		// Token: 0x060075CC RID: 30156 RVA: 0x0036E24C File Offset: 0x0036C44C
		public override void InitMonitorFieldIds()
		{
			this.MonitorFields.Add(new UIBase.MonitorDataField(19, 23, ulong.MaxValue, null));
			this.MonitorFields.Add(new UIBase.MonitorDataField(19, 28, ulong.MaxValue, null));
			this.MonitorFields.Add(new UIBase.MonitorDataField(19, 29, ulong.MaxValue, null));
			this.MonitorFields.Add(new UIBase.MonitorDataField(19, 116, ulong.MaxValue, null));
			this.MonitorFields.Add(new UIBase.MonitorDataField(4, 0, (ulong)this._taiwuCharId, new uint[]
			{
				66U,
				59U
			}));
		}

		// Token: 0x060075CD RID: 30157 RVA: 0x0036E2E4 File Offset: 0x0036C4E4
		public override void OnNotifyGameData(List<NotificationWrapper> notifications)
		{
			foreach (NotificationWrapper wrapper in notifications)
			{
				Notification notification = wrapper.Notification;
				byte type = notification.Type;
				byte b = type;
				if (b != 0)
				{
					if (b == 1)
					{
						bool flag = notification.DomainId == 4;
						if (flag)
						{
							bool flag2 = notification.MethodId == 27;
							if (flag2)
							{
								Serializer.Deserialize(wrapper.DataPool, notification.ValueOffset, ref this._weaponList);
								this._weaponList.RemoveAll((ItemDisplayData item) => item.Key.ItemType != 0);
								this._itemDataDict.Clear();
								foreach (ItemDisplayData itemData in this._weaponList)
								{
									this._itemDataDict.Add(itemData.Key, itemData);
								}
							}
							else
							{
								bool flag3 = notification.MethodId == 29;
								if (flag3)
								{
									List<ItemDisplayData> equipDataList = new List<ItemDisplayData>();
									Serializer.Deserialize(wrapper.DataPool, notification.ValueOffset, ref equipDataList);
									for (sbyte slotIndex = 2; slotIndex >= 0; slotIndex -= 1)
									{
										ItemDisplayData weaponData = equipDataList[(int)slotIndex];
										bool flag4 = weaponData.Key.IsValid();
										if (flag4)
										{
											this._weaponList.Insert(0, weaponData);
											this._itemDataDict.Add(weaponData.Key, weaponData);
										}
									}
								}
							}
						}
						else
						{
							bool flag5 = notification.DomainId == 5 && notification.MethodId == 15;
							if (flag5)
							{
								List<ItemDisplayData> warehouseDataList = new List<ItemDisplayData>();
								Serializer.Deserialize(wrapper.DataPool, notification.ValueOffset, ref warehouseDataList);
								foreach (ItemDisplayData itemData2 in warehouseDataList)
								{
									bool flag6 = itemData2.Key.ItemType == 0;
									if (flag6)
									{
										this._itemDataDict.Add(itemData2.Key, itemData2);
									}
								}
								base.AppendMonitorFieldId(new UIBase.MonitorDataField(19, 24, ulong.MaxValue, null));
							}
							else
							{
								bool flag7 = notification.DomainId == 7 && notification.MethodId == 0;
								if (flag7)
								{
									List<CombatSkillDisplayData> dataList = new List<CombatSkillDisplayData>();
									Serializer.Deserialize(wrapper.DataPool, notification.ValueOffset, ref dataList);
									foreach (CombatSkillDisplayData skillData in dataList)
									{
										this._skillDataDict.Add(skillData.TemplateId, skillData);
									}
									base.AppendMonitorFieldId(new UIBase.MonitorDataField(19, 26, ulong.MaxValue, null));
								}
								else
								{
									bool flag8 = notification.DomainId == 11 && notification.MethodId == 2;
									if (flag8)
									{
										Serializer.Deserialize(wrapper.DataPool, notification.ValueOffset, ref this._bookData);
										this.UpdateExtraButtonStatus();
										this.UpdateBookOwners();
										this.UpdatePageToggleStatus();
									}
								}
							}
						}
					}
				}
				else
				{
					DataUid uid = notification.Uid;
					bool flag9 = uid.DomainId == 19;
					if (flag9)
					{
						bool flag10 = uid.DataId == 23;
						if (flag10)
						{
							Serializer.DeserializeModifications<sbyte>(wrapper.DataPool, notification.ValueOffset, this._legendaryBookBreakPlateCounts);
							bool anySkillTypeSelected = this.AnySkillTypeSelected;
							if (anySkillTypeSelected)
							{
								this.UpdateBreakPlate(true);
								this.UpdateBreakPlate(false);
							}
						}
						else
						{
							bool flag11 = uid.DataId == 24;
							if (flag11)
							{
								Serializer.DeserializeModifications<sbyte>(wrapper.DataPool, notification.ValueOffset, this._legendaryBookWeaponSlot);
								bool anySkillTypeSelected2 = this.AnySkillTypeSelected;
								if (anySkillTypeSelected2)
								{
									this.UpdateWeaponSlot();
								}
							}
							else
							{
								bool flag12 = uid.DataId == 26;
								if (flag12)
								{
									Serializer.DeserializeModifications<sbyte>(wrapper.DataPool, notification.ValueOffset, this._legendaryBookSkillSlot);
									bool anySkillTypeSelected3 = this.AnySkillTypeSelected;
									if (anySkillTypeSelected3)
									{
										this.UpdateAllSkillSlots();
									}
									this.Element.ShowAfterRefresh();
								}
								else
								{
									bool flag13 = uid.DataId == 28;
									if (flag13)
									{
										Serializer.Deserialize(wrapper.DataPool, notification.ValueOffset, ref this._legendaryBookBonusCountYin);
										bool anySkillTypeSelected4 = this.AnySkillTypeSelected;
										if (anySkillTypeSelected4)
										{
											this.UpdateBonusExpTips();
											this.UpdateBonusList(true);
											this.UpdateBonusList(false);
										}
									}
									else
									{
										bool flag14 = uid.DataId == 29;
										if (flag14)
										{
											Serializer.Deserialize(wrapper.DataPool, notification.ValueOffset, ref this._legendaryBookBonusCountYang);
											bool anySkillTypeSelected5 = this.AnySkillTypeSelected;
											if (anySkillTypeSelected5)
											{
												this.UpdateBonusExpTips();
												this.UpdateBonusList(false);
												this.UpdateBonusList(true);
											}
										}
										else
										{
											bool flag15 = uid.DataId == 116;
											if (flag15)
											{
												Serializer.Deserialize(wrapper.DataPool, notification.ValueOffset, ref this._sectRanshanThreeCorpses);
												this.UpdatePageToggleStatus();
											}
										}
									}
								}
							}
						}
					}
					else
					{
						bool flag16 = uid.DomainId == 4 && uid.DataId == 0 && (int)uid.SubId0 == this._taiwuCharId;
						if (flag16)
						{
							bool flag17 = uid.SubId1 == 66U;
							if (flag17)
							{
								Serializer.Deserialize(wrapper.DataPool, notification.ValueOffset, ref this._exp);
								this.txtExp.text = this._exp.ToString();
								bool anySkillTypeSelected6 = this.AnySkillTypeSelected;
								if (anySkillTypeSelected6)
								{
									this.UpdateBonusExpTips();
									this.UpdateBonusList(true);
									this.UpdateBonusList(false);
								}
							}
							else
							{
								bool flag18 = uid.SubId1 == 59U;
								if (flag18)
								{
									Serializer.Deserialize(wrapper.DataPool, notification.ValueOffset, ref this._combatSkillIdList);
									this._skillDataDict.Clear();
									bool flag19 = this._combatSkillIdList.Count > 0;
									if (flag19)
									{
										CombatSkillModel.GetCombatSkillDisplayData(this.Element.GameDataListenerId, this._taiwuCharId, this._combatSkillIdList);
									}
								}
							}
						}
					}
				}
			}
		}

		// Token: 0x060075CE RID: 30158 RVA: 0x0036E970 File Offset: 0x0036CB70
		private void ResetUiState()
		{
			this.txtTitle.text = LocalStringManager.Get(LanguageKey.LK_LegendaryBook);
			bool flag = this._skillTypeChangeSeq != null && this._skillTypeChangeSeq.IsActive();
			if (flag)
			{
				this._skillTypeChangeSeq.Kill(false);
			}
			AudioManager.Instance.PlaySound("legendbook_ambience", true, false);
			this.skillTypeTogGroupBookPage.SetWithoutNotify(-1);
			this.pageBookView.gameObject.SetActive(true);
			this.pageNodeView.gameObject.SetActive(false);
		}

		// Token: 0x060075CF RID: 30159 RVA: 0x0036EA00 File Offset: 0x0036CC00
		private void OnSkillTypeTogChange(int togNew, int togOld)
		{
			ViewLegendaryBook.<>c__DisplayClass93_0 CS$<>8__locals1 = new ViewLegendaryBook.<>c__DisplayClass93_0();
			CS$<>8__locals1.togNew = togNew;
			CS$<>8__locals1.<>4__this = this;
			CS$<>8__locals1.togOld = togOld;
			bool needChangeToNodePage = CS$<>8__locals1.togNew >= 0 && CS$<>8__locals1.togOld < 0;
			bool flag = needChangeToNodePage;
			if (flag)
			{
				this.SwitchByCapture(new Action(CS$<>8__locals1.<OnSkillTypeTogChange>g__HandleSwitch|0));
			}
			else
			{
				CS$<>8__locals1.<OnSkillTypeTogChange>g__HandleSwitch|0();
			}
		}

		// Token: 0x060075D0 RID: 30160 RVA: 0x0036EA68 File Offset: 0x0036CC68
		private void UpdateExtraButtonStatus()
		{
			LegendaryBookIncrementData bookData = this._bookData;
			this.UpdateCompetitorButton(bookData != null && bookData.ContestList.Count > 0);
			LegendaryBookIncrementData bookData2 = this._bookData;
			bool isEnable;
			if (bookData2 == null || bookData2.ShockedList.Count <= 0)
			{
				LegendaryBookIncrementData bookData3 = this._bookData;
				if (bookData3 == null || bookData3.InsaneList.Count <= 0)
				{
					LegendaryBookIncrementData bookData4 = this._bookData;
					isEnable = (bookData4 != null && bookData4.ConsumedList.Count > 0);
					goto IL_78;
				}
			}
			isEnable = true;
			IL_78:
			this.UpdateFallenButton(isEnable);
		}

		// Token: 0x060075D1 RID: 30161 RVA: 0x0036EAF3 File Offset: 0x0036CCF3
		private void UpdateCompetitorButton(bool isEnable)
		{
			this.btnCompetitors.interactable = isEnable;
			this.mouseTipCompetitorsBtn.enabled = isEnable;
		}

		// Token: 0x060075D2 RID: 30162 RVA: 0x0036EB10 File Offset: 0x0036CD10
		public void CharacterListButtonHoverMode(bool enter)
		{
			if (enter)
			{
				bool currentInNodePage = this._currentInNodePage;
				if (currentInNodePage)
				{
					this.nodeViewHolder.gameObject.SetActive(false);
				}
			}
			else
			{
				this.nodeViewHolder.gameObject.SetActive(true);
			}
		}

		// Token: 0x060075D3 RID: 30163 RVA: 0x0036EB57 File Offset: 0x0036CD57
		private void UpdateFallenButton(bool isEnable)
		{
			this.btnFallen.interactable = isEnable;
			this.mouseTipFallenBtn.enabled = isEnable;
		}

		// Token: 0x060075D4 RID: 30164 RVA: 0x0036EB74 File Offset: 0x0036CD74
		private void UpdateBookOwners()
		{
			for (sbyte i = 0; i < 14; i += 1)
			{
				this._bookOwners[(int)i] = -1;
			}
			bool flag = this._bookData != null;
			if (flag)
			{
				foreach (KeyValuePair<sbyte, int> keyValuePair in this._bookData.OwnerMap)
				{
					sbyte b;
					int num;
					keyValuePair.Deconstruct(out b, out num);
					sbyte bookType = b;
					int ownerId = num;
					this._bookOwners[(int)bookType] = ownerId;
				}
			}
		}

		// Token: 0x060075D5 RID: 30165 RVA: 0x0036EC10 File Offset: 0x0036CE10
		private void UpdatePageToggleStatus()
		{
			this._keptBook = 0;
			bool flag = this._sectRanshanThreeCorpses != null;
			if (flag)
			{
				foreach (SectStoryThreeCorpsesCharacter corpse in this._sectRanshanThreeCorpses)
				{
					bool flag2 = corpse.LegendaryBooks == null;
					if (!flag2)
					{
						foreach (sbyte bookType in corpse.LegendaryBooks)
						{
							bool flag3 = bookType != -1;
							if (flag3)
							{
								this._keptBook |= 1 << (int)bookType;
							}
						}
					}
				}
			}
			this.bookItemStatus.Clear();
			ViewLegendaryBook.<>c__DisplayClass99_0 CS$<>8__locals1 = new ViewLegendaryBook.<>c__DisplayClass99_0();
			CS$<>8__locals1.<>4__this = this;
			CS$<>8__locals1.i = 0;
			while (CS$<>8__locals1.i < 14)
			{
				bool ownByTaiwu = this._bookOwners[(int)CS$<>8__locals1.i] == this._taiwuCharId;
				ViewLegendaryBook.<>c__DisplayClass99_1 CS$<>8__locals2;
				CS$<>8__locals2.keepByCorpses = ((this._keptBook & 1 << (int)CS$<>8__locals1.i) != 0);
				bool ownedByOthers = this._bookOwners[(int)CS$<>8__locals1.i] >= 0 && this._bookOwners[(int)CS$<>8__locals1.i] != this._taiwuCharId;
				bool hasAdventure = this._bookData.BookLocationMap.ContainsKey(CS$<>8__locals1.i);
				bool hasCompetitors = this._bookData.ContestList.Count((int t) => CS$<>8__locals1.<>4__this._bookData.CharacterMap[t].BookType == CS$<>8__locals1.i) > 0;
				bool kept = (this._keptBook & 1 << (int)CS$<>8__locals1.i) != 0;
				LegendaryBookUIItem item = this.skillTypeTogGroupBookPage.GetByIndex((int)CS$<>8__locals1.i);
				LegendaryBookUIItem item2 = this.skillTypeTogGroupNodePage.GetByIndex((int)CS$<>8__locals1.i);
				bool flag4 = ownedByOthers;
				if (flag4)
				{
					this.bookItemStatus[(int)CS$<>8__locals1.i] = ViewLegendaryBook.ELegendaryBookStatus.OwnByOthers;
					item.SetupOwnedByOthers();
					item2.SetupOwnedByOthers();
				}
				else
				{
					bool flag5 = hasAdventure;
					if (flag5)
					{
						this.bookItemStatus[(int)CS$<>8__locals1.i] = ViewLegendaryBook.ELegendaryBookStatus.Contesting;
						item.SetupContesting();
						item2.SetupContesting();
					}
					else
					{
						bool keepByCorpses = CS$<>8__locals2.keepByCorpses;
						if (keepByCorpses)
						{
							this.bookItemStatus[(int)CS$<>8__locals1.i] = ViewLegendaryBook.ELegendaryBookStatus.keepByCorpses;
							item.SetupKeepByCorpses(true);
							item2.SetupKeepByCorpses(true);
						}
						else
						{
							bool flag6 = ownByTaiwu;
							if (flag6)
							{
								this.bookItemStatus[(int)CS$<>8__locals1.i] = ViewLegendaryBook.ELegendaryBookStatus.Owning;
								item.SetupNormal(true);
								item2.SetupNormal(true);
							}
							else
							{
								this.bookItemStatus[(int)CS$<>8__locals1.i] = ViewLegendaryBook.ELegendaryBookStatus.NotShown;
								int num;
								bool flag7 = this._bookData.BookDurationMap.TryGetValue(CS$<>8__locals1.i, out num);
								if (flag7)
								{
									item.SetupNoShown(true);
									item2.SetupNoShown(true);
								}
								else
								{
									item.SetupNoShown(false);
									item2.SetupNoShown(false);
								}
							}
						}
					}
				}
				CS$<>8__locals1.<UpdatePageToggleStatus>g__SetupTips|1(item.mouseTip, ref CS$<>8__locals2);
				CS$<>8__locals1.<UpdatePageToggleStatus>g__SetupTips|1(item2.mouseTip, ref CS$<>8__locals2);
				sbyte i = CS$<>8__locals1.i;
				CS$<>8__locals1.i = i + 1;
			}
			this._ownedCurrBook = (this._bookOwners[(int)this._currCombatSkillType] == this._taiwuCharId || (this._keptBook & 1 << (int)this._currCombatSkillType) != 0);
			this.RefreshNodePageBookItemParents(null);
		}

		// Token: 0x060075D6 RID: 30166 RVA: 0x0036EFB0 File Offset: 0x0036D1B0
		private void UpdateBonusAndEffectSlot(bool isYin)
		{
			CombatSkillTypeItem typeConfig = Config.CombatSkillType.Instance[this._currCombatSkillType];
			List<short> bonusList = isYin ? typeConfig.LegendaryBookAddPropertyYin : typeConfig.LegendaryBookAddPropertyYang;
			List<sbyte> slotIndexList = isYin ? typeConfig.LegendaryBookEffectSlotYin : typeConfig.LegendaryBookEffectSlotYang;
			Refers refers = isYin ? this.refersYin : this.refersYang;
			RectTransform bonusHolder = refers.CGet<RectTransform>("BonusHolder");
			RectTransform effectHolder = refers.CGet<RectTransform>("EffectHolder");
			refers.CGet<TooltipInvoker>("BreakPlate").PresetParam[0] = typeConfig.Name;
			refers.CGet<TooltipInvoker>("MousetipNotUnlock").PresetParam[0] = typeConfig.Name;
			for (int i = 0; i < bonusHolder.childCount; i++)
			{
				LegendaryBookBonusItem bonusRefers = bonusHolder.GetChild(i).GetComponent<LegendaryBookBonusItem>();
				TooltipInvoker tipDisplayer = bonusRefers.GetComponent<TooltipInvoker>();
				short bonusType = bonusList[i];
				short slotType = -1;
				bool flag = (i + 1) % 4 == 0;
				if (flag)
				{
					int slotIndex = (int)slotIndexList[(i + 1) / 4 - 1];
					slotType = ((slotIndex < 0) ? typeConfig.LegendaryBookWeaponSlot : typeConfig.LegendaryBookSkillSlots[slotIndex]);
				}
				bool flag2 = i % 4 != 0;
				if (flag2)
				{
					LegendaryBookPropertyBonusTypeItem bonusConfig = LegendaryBookPropertyBonusType.Instance[bonusType];
					PropertyAndValue addProperty = bonusConfig.PropertyBonusList[0];
					bonusRefers.SetNameText(string.Format("+{0}", addProperty.Value));
					bonusRefers.SetSkillTypeIcon(this._currCombatSkillType);
				}
				tipDisplayer.RuntimeParam.Set("SkillType", this._currCombatSkillType);
				tipDisplayer.RuntimeParam.Set("BonusType", bonusType);
				tipDisplayer.RuntimeParam.Set("SlotType", slotType);
			}
			for (int j = 0; j < effectHolder.childCount; j++)
			{
				LegendaryBookEffectItem effectRefers = effectHolder.GetChild(j).GetComponent<LegendaryBookEffectItem>();
				TooltipInvoker tipDisplayer2 = effectRefers.GetComponent<TooltipInvoker>();
				effectRefers.UserInt = (int)slotIndexList[j];
				short effectSlot = (effectRefers.UserInt < 0) ? typeConfig.LegendaryBookWeaponSlot : typeConfig.LegendaryBookSkillSlots[effectRefers.UserInt];
				LegendaryBookSlotItem slotConfig = LegendaryBookSlot.Instance[effectSlot];
				effectRefers.NameEnable.text = slotConfig.Name;
				effectRefers.NameDisable.text = slotConfig.Name;
				tipDisplayer2.PresetParam[0] = slotConfig.Name;
				bool flag3 = effectRefers.UserInt < 0;
				if (flag3)
				{
					this._currWeaponEffectRefers = effectRefers;
				}
			}
		}

		// Token: 0x060075D7 RID: 30167 RVA: 0x0036F240 File Offset: 0x0036D440
		private void UpdateBreakPlate(bool isYin)
		{
			bool flag = !this.AnySkillTypeSelected;
			if (!flag)
			{
				bool unlocked = this.IsBreakPlateUnlocked(isYin);
				Refers refers = isYin ? this.refersYin : this.refersYang;
				CButton unlockButton = refers.CGet<CButton>("UnlockBreakPlate");
				bool flag2 = !unlocked && this._ownedCurrBook;
				unlockButton.interactable = this._ownedCurrBook;
				unlockButton.gameObject.SetActive(!unlocked);
				refers.CGet<GameObject>("UnlockedObj").SetActive(unlocked);
			}
		}

		// Token: 0x060075D8 RID: 30168 RVA: 0x0036F2C4 File Offset: 0x0036D4C4
		private void UpdateBonusList(bool isYin)
		{
			CombatSkillTypeItem typeConfig = Config.CombatSkillType.Instance[this._currCombatSkillType];
			Refers refers = isYin ? this.refersYin : this.refersYang;
			SByteList bonusCount = isYin ? this._legendaryBookBonusCountYin : this._legendaryBookBonusCountYang;
			int unlockedCount = (int)((bonusCount.Items != null && bonusCount.Items.Count > 0 && this._ownedCurrBook) ? bonusCount.Items[(int)this._currCombatSkillType] : 0);
			SByteList bonusCountConflict = isYin ? this._legendaryBookBonusCountYang : this._legendaryBookBonusCountYin;
			int conflictunlockedCount = (int)((bonusCountConflict.Items != null && bonusCountConflict.Items.Count > 0 && this._ownedCurrBook) ? bonusCountConflict.Items[(int)this._currCombatSkillType] : 0);
			bool conflicted = this._limitionActive && conflictunlockedCount > 0;
			RectTransform bonusHolder = refers.CGet<RectTransform>("BonusHolder");
			RectTransform effectHolder = refers.CGet<RectTransform>("EffectHolder");
			RectTransform unlockLightPath = refers.CGet<RectTransform>("UnlockLightPath");
			for (int i = 0; i < bonusHolder.childCount; i++)
			{
				LegendaryBookBonusItem bonusRefers = bonusHolder.GetChild(i).GetComponent<LegendaryBookBonusItem>();
				bool unlocked = unlockedCount > i;
				bool canUnlock = unlockedCount == i && this._exp >= this._unlockNeedExp && !conflicted;
				bonusRefers.DisableObj.SetActive(!unlocked && !canUnlock);
				bonusRefers.Unlocked.SetActive(unlocked);
				bonusRefers.BtnImage.gameObject.SetActive(canUnlock && !unlocked);
				bonusRefers.GetComponent<CButton>().interactable = (!unlocked && canUnlock && this._ownedCurrBook);
				bonusRefers.GetComponent<TooltipInvoker>().RuntimeParam.Set("ShowExpCost", unlockedCount == i);
				bool flag2 = i % 4 == 0;
				if (flag2)
				{
					bool flag3 = bonusRefers.EffActive.gameObject.activeSelf != canUnlock;
					if (flag3)
					{
						bonusRefers.EffActive.gameObject.SetActive(canUnlock);
						bool flag4 = canUnlock;
						if (flag4)
						{
							bonusRefers.EffActive.Play();
						}
					}
					bonusRefers.NameUnlocked.SetActive(unlocked);
					bonusRefers.NameCanUnlock.SetActive(!unlocked && canUnlock);
					bonusRefers.NameCannotUnlock.SetActive(!unlocked && !canUnlock);
				}
				else
				{
					bool needShowEff = !unlocked && canUnlock;
					bool flag5 = bonusRefers.EffActive.gameObject.activeSelf != needShowEff;
					if (flag5)
					{
						bonusRefers.EffActive.gameObject.SetActive(needShowEff);
						bool flag6 = needShowEff;
						if (flag6)
						{
							bonusRefers.EffActive.Play();
						}
					}
					bonusRefers.NameUnlocked.SetActive(unlocked);
					bonusRefers.NameCanUnlock.SetActive(!unlocked && canUnlock);
					bonusRefers.NameCannotUnlock.SetActive(!unlocked && !canUnlock);
				}
			}
			RectTransform areaBgHolder = refers.CGet<RectTransform>("BgHolder");
			for (int j = 0; j < effectHolder.childCount; j++)
			{
				LegendaryBookEffectItem effectRefers = effectHolder.GetChild(j).GetComponent<LegendaryBookEffectItem>();
				bool unlocked2 = unlockedCount / 4 > j;
				bool flag = unlocked2 && this._ownedCurrBook;
				effectRefers.SetCanInteract(flag);
				effectRefers.SetUnlocked(unlocked2);
				effectRefers.NameEnable.gameObject.SetActive(unlocked2);
				effectRefers.NameDisable.gameObject.SetActive(!unlocked2);
				int picIndex = flag ? 1 : 0;
				areaBgHolder.GetChild(j).GetComponent<CImage>().SetSprite(string.Format("ui9_back_legendbook_skillbookbase_{0}_{1}", j, picIndex), false, null);
			}
			for (int k = 1; k < unlockLightPath.childCount; k++)
			{
				unlockLightPath.GetChild(k).gameObject.SetActive(unlockedCount >= k);
			}
		}

		// Token: 0x060075D9 RID: 30169 RVA: 0x0036F6D4 File Offset: 0x0036D8D4
		private string GetBranchLineName(int index, bool unlocked)
		{
			int unlockedIndex = unlocked ? 0 : 1;
			int positionIndex = index % 4;
			string result;
			switch (index / 4)
			{
			case 0:
				result = string.Format("ui9_back_legendbook_branchline_down_{0}_{1}", unlockedIndex, positionIndex);
				break;
			case 1:
				result = string.Format("ui9_back_legendbook_branchline_middle_{0}_{1}", unlockedIndex, positionIndex);
				break;
			case 2:
				result = string.Format("ui9_back_legendbook_branchline_up_{0}_{1}", unlockedIndex, positionIndex);
				break;
			default:
				result = string.Empty;
				break;
			}
			return result;
		}

		// Token: 0x060075DA RID: 30170 RVA: 0x0036F768 File Offset: 0x0036D968
		public void SwitchByCapture(Action onCaptureOver)
		{
			if (onCaptureOver != null)
			{
				onCaptureOver();
			}
			bool activeSelf = this.pageNodeView.activeSelf;
			if (activeSelf)
			{
				bool flag = this._pageChangeSeq != null;
				if (flag)
				{
					this._pageChangeSeq.Kill(false);
					this._pageChangeSeq = null;
				}
				this.yinYangNodesBg.SetActive(false);
				this.yinYangNodes.alpha = 0f;
				this.yinYangNodesEff.SetActive(true);
				this._pageChangeSeq = DOTween.Sequence();
				this._pageChangeSeq.Insert(0.35f, this.yinYangNodes.DOFade(1f, 0.35f));
				this._pageChangeSeq.InsertCallback(1f, delegate
				{
					this.yinYangNodesEff.SetActive(false);
					this.yinYangNodesBg.SetActive(true);
				});
				this._pageChangeSeq.Play<Sequence>();
			}
		}

		// Token: 0x060075DB RID: 30171 RVA: 0x0036F83E File Offset: 0x0036DA3E
		private IEnumerator CaptureCoroutine(Action onCaptureOver)
		{
			yield return new WaitForEndOfFrame();
			Vector3[] corners = new Vector3[4];
			base.RectTransform.GetWorldCorners(corners);
			Vector2 screenBL = RectTransformUtility.WorldToScreenPoint(UIManager.Instance.UiCamera, corners[0]);
			Vector2 screenTR = RectTransformUtility.WorldToScreenPoint(UIManager.Instance.UiCamera, corners[2]);
			int width = Mathf.RoundToInt(screenTR.x - screenBL.x);
			int height = Mathf.RoundToInt(screenTR.y - screenBL.y);
			Texture2D tex = new Texture2D(width, height, TextureFormat.RGB24, false);
			tex.ReadPixels(new Rect(screenBL.x, screenBL.y, (float)width, (float)height), 0, 0);
			tex.Apply();
			float aspectRatio = (float)Screen.width / (float)Screen.height;
			float clampedRatio = Mathf.Min(aspectRatio, this._basicRatio);
			Vector3 newRatio = new Vector3(this._basicRatio, 1f, 1f);
			newRatio *= clampedRatio / this._basicRatio;
			this.effTrans1.localScale = newRatio;
			this.effTrans2.localScale = newRatio;
			this.matSwitch1.SetTexture("_MainTex", tex);
			this.matSwitch2.SetTexture("_MainTex", tex);
			if (onCaptureOver != null)
			{
				onCaptureOver();
			}
			this.switchTempObj.gameObject.SetActive(true);
			this.switchTempObj.Play();
			this.matSwitch1.SetFloat("_MaskProgress", 0f);
			this.matSwitch2.SetFloat("_MaskProgress", 0f);
			float switchDuration = 0.8f;
			Sequence seq = DOTween.Sequence();
			seq.AppendCallback(delegate
			{
				this.blocker.gameObject.SetActive(true);
			});
			seq.AppendCallback(delegate
			{
				this.matSwitch1.DOFloat(1f, "_MaskProgress", switchDuration);
				this.matSwitch2.DOFloat(1f, "_MaskProgress", switchDuration);
			});
			seq.AppendInterval(switchDuration);
			seq.AppendCallback(delegate
			{
				this.blocker.gameObject.SetActive(false);
			});
			seq.AppendCallback(delegate
			{
				this.switchTempObj.gameObject.SetActive(false);
			});
			seq.Play<Sequence>();
			yield break;
		}

		// Token: 0x060075DC RID: 30172 RVA: 0x0036F854 File Offset: 0x0036DA54
		private void UpdateBonusExpTips()
		{
			bool flag = !this.AnySkillTypeSelected;
			if (!flag)
			{
				RectTransform bonusHolderYin = this.refersYin.CGet<RectTransform>("BonusHolder");
				RectTransform bonusHolderYang = this.refersYang.CGet<RectTransform>("BonusHolder");
				int unlockedCountYin = (int)((this._legendaryBookBonusCountYin.Items != null && this._legendaryBookBonusCountYin.Items.Count > 0) ? this._legendaryBookBonusCountYin.Items[(int)this._currCombatSkillType] : 0);
				int unlockedCountYang = (int)((this._legendaryBookBonusCountYang.Items != null && this._legendaryBookBonusCountYang.Items.Count > 0) ? this._legendaryBookBonusCountYang.Items[(int)this._currCombatSkillType] : 0);
				int totalUnlockedCount = unlockedCountYin + unlockedCountYang;
				this._unlockNeedExp = ((totalUnlockedCount < GlobalConfig.Instance.LegendaryBookUnlockExp.Length) ? GlobalConfig.Instance.LegendaryBookUnlockExp[totalUnlockedCount] : 0);
				bool limitionActive = this._limitionActive;
				if (limitionActive)
				{
					this._unlockNeedExp *= 2;
				}
				bool flag2 = unlockedCountYin < bonusHolderYin.childCount;
				if (flag2)
				{
					TooltipInvoker tipDisplayer = bonusHolderYin.GetChild(unlockedCountYin).GetComponent<TooltipInvoker>();
					tipDisplayer.RuntimeParam.Set("NeedExp", this._unlockNeedExp);
					tipDisplayer.RuntimeParam.Set("CurrExp", this._exp);
				}
				bool flag3 = unlockedCountYang < bonusHolderYang.childCount;
				if (flag3)
				{
					TooltipInvoker tipDisplayer2 = bonusHolderYang.GetChild(unlockedCountYang).GetComponent<TooltipInvoker>();
					tipDisplayer2.RuntimeParam.Set("NeedExp", this._unlockNeedExp);
					tipDisplayer2.RuntimeParam.Set("CurrExp", this._exp);
				}
			}
		}

		// Token: 0x060075DD RID: 30173 RVA: 0x0036F9F0 File Offset: 0x0036DBF0
		private void UpdateWeaponSlot()
		{
			bool flag = this._currWeaponEffectRefers == null;
			if (!flag)
			{
				bool hasItem = this._legendaryBookWeaponSlot.ContainsKey(this._currCombatSkillType) && this._legendaryBookWeaponSlot[this._currCombatSkillType].IsValid() && this._ownedCurrBook;
				TooltipInvoker tipDisplayer = this._currWeaponEffectRefers.GetComponent<TooltipInvoker>();
				CImage itemIcon = this._currWeaponEffectRefers.ItemIcon;
				CombatSkillTypeItem typeConfig = Config.CombatSkillType.Instance[this._currCombatSkillType];
				LegendaryBookSlotItem slotConfig = LegendaryBookSlot.Instance[typeConfig.LegendaryBookWeaponSlot];
				StringBuilder strBuilder = EasyPool.Get<StringBuilder>();
				strBuilder.Clear();
				strBuilder.Append(slotConfig.Desc);
				bool flag2 = hasItem;
				if (flag2)
				{
					ItemKey weaponKey = this._legendaryBookWeaponSlot[this._currCombatSkillType];
					this._currWeaponEffectRefers.SetWeapon(weaponKey);
					WeaponItem itemConfig = Weapon.Instance[weaponKey.TemplateId];
					strBuilder.Append("\n\n");
					strBuilder.Append(LocalStringManager.Get(LanguageKey.LK_LegendaryBook_Affect_Weapon_Tips));
					strBuilder.Append(LocalStringManager.Get(LanguageKey.LK_Colon_Symbol));
					strBuilder.Append(itemConfig.Name.SetColor(Colors.Instance.GradeColors[(int)itemConfig.Grade]));
					itemIcon.SetSprite(ItemTemplateHelper.GetIcon(weaponKey.ItemType, weaponKey.TemplateId), false, null);
				}
				else
				{
					this._currWeaponEffectRefers.SetWeapon(ItemKey.Invalid);
				}
				tipDisplayer.PresetParam[1] = strBuilder.ToString();
				EasyPool.Free<StringBuilder>(strBuilder);
			}
		}

		// Token: 0x060075DE RID: 30174 RVA: 0x0036FB8C File Offset: 0x0036DD8C
		private void UpdateAllSkillSlots()
		{
			List<short> skillIdList = (this._legendaryBookSkillSlot.ContainsKey(this._currCombatSkillType) && this._ownedCurrBook) ? this._legendaryBookSkillSlot[this._currCombatSkillType].Items : null;
			RectTransform effectHolder = this.refersYin.CGet<RectTransform>("EffectHolder");
			for (int i = 0; i < effectHolder.childCount; i++)
			{
				LegendaryBookEffectItem effectRefers = effectHolder.GetChild(i).GetComponent<LegendaryBookEffectItem>();
				bool flag = effectRefers.UserInt < 0;
				if (!flag)
				{
					this.UpdateSkillSlot(effectRefers, (skillIdList != null) ? skillIdList[effectRefers.UserInt] : -1);
				}
			}
			effectHolder = this.refersYang.CGet<RectTransform>("EffectHolder");
			for (int j = 0; j < effectHolder.childCount; j++)
			{
				LegendaryBookEffectItem effectRefers2 = effectHolder.GetChild(j).GetComponent<LegendaryBookEffectItem>();
				bool flag2 = effectRefers2.UserInt < 0;
				if (!flag2)
				{
					this.UpdateSkillSlot(effectRefers2, (skillIdList != null) ? skillIdList[effectRefers2.UserInt] : -1);
				}
			}
		}

		// Token: 0x060075DF RID: 30175 RVA: 0x0036FCA0 File Offset: 0x0036DEA0
		private void UpdateSkillSlot(LegendaryBookEffectItem slotRefers, short skillId)
		{
			bool hasSkill = skillId >= 0;
			TooltipInvoker tipDisplayer = slotRefers.GetComponent<TooltipInvoker>();
			CImage itemIcon = slotRefers.ItemIcon;
			CombatSkillTypeItem typeConfig = Config.CombatSkillType.Instance[this._currCombatSkillType];
			LegendaryBookSlotItem slotConfig = LegendaryBookSlot.Instance[typeConfig.LegendaryBookSkillSlots[slotRefers.UserInt]];
			StringBuilder strBuilder = EasyPool.Get<StringBuilder>();
			slotRefers.SetSkill(skillId);
			strBuilder.Clear();
			strBuilder.Append(slotConfig.Desc);
			bool flag = hasSkill;
			if (flag)
			{
				CombatSkillItem skillConfig = CombatSkill.Instance[skillId];
				strBuilder.Append("\n\n");
				strBuilder.Append(LocalStringManager.Get(LanguageKey.LK_LegendaryBook_Affect_Skill_Tips));
				strBuilder.Append(LocalStringManager.Get(LanguageKey.LK_Colon_Symbol));
				strBuilder.Append(skillConfig.Name.SetColor(Colors.Instance.GradeColors[(int)skillConfig.Grade]));
				itemIcon.SetSprite(skillConfig.Icon, false, null);
			}
			tipDisplayer.PresetParam[1] = strBuilder.ToString();
			EasyPool.Free<StringBuilder>(strBuilder);
		}

		// Token: 0x060075E0 RID: 30176 RVA: 0x0036FDB0 File Offset: 0x0036DFB0
		private void ShowSelectSkill()
		{
			this._canSelectCombatSkillIdList.Clear();
			foreach (short skillId in this._combatSkillIdList)
			{
				bool flag = this.IsRequiredSkillType(skillId) && (!this._legendaryBookSkillSlot.ContainsKey(this._currCombatSkillType) || !this._legendaryBookSkillSlot[this._currCombatSkillType].Items.Contains(skillId));
				if (flag)
				{
					this._canSelectCombatSkillIdList.Add(skillId);
				}
			}
			List<short> skillIdList = (this._legendaryBookSkillSlot.ContainsKey(this._currCombatSkillType) && this._ownedCurrBook) ? this._legendaryBookSkillSlot[this._currCombatSkillType].Items : null;
			short prevCombatSkillId = (skillIdList != null) ? skillIdList[this._skillSlotIndex] : -1;
			bool flag2 = prevCombatSkillId >= 0;
			if (flag2)
			{
				this._canSelectCombatSkillIdList.Insert(0, prevCombatSkillId);
			}
			this._selectSkillArgBox.Set("CharId", this._taiwuCharId);
			this._selectSkillArgBox.Set("SaveToggleKey", false);
			this._selectSkillArgBox.SetObject("CombatSkillIdList", this._canSelectCombatSkillIdList);
			this._selectSkillArgBox.Set("IsShowNeiLiFinish", false);
			this._selectSkillArgBox.Set("ShowNone", true);
			this._selectSkillArgBox.Set("PrevCombatSkillId", prevCombatSkillId);
			UIElement.SelectSkill.SetOnInitArgs(this._selectSkillArgBox);
			UIManager.Instance.MaskUI(UIElement.SelectSkill);
		}

		// Token: 0x060075E1 RID: 30177 RVA: 0x0036FF60 File Offset: 0x0036E160
		private void OnSelectSkill(sbyte type, short skillTemplateId)
		{
			LegendaryBookDomainMethod.Call.SaveLegendaryBookSkillPresetSlotCurrent(this._currCombatSkillType, this._skillSlotIndex, skillTemplateId);
		}

		// Token: 0x060075E2 RID: 30178 RVA: 0x0036FF78 File Offset: 0x0036E178
		private void ShowSelectItem()
		{
			List<short> canSelectSubTypes = Config.CombatSkillType.Instance[this._currCombatSkillType].LegendaryBookWeaponSlotItemSubTypes;
			this._canSelectWeaponList.Clear();
			ItemKey itemKey;
			this._legendaryBookWeaponSlot.TryGetValue(this._currCombatSkillType, out itemKey);
			ItemDisplayData currentSelectedData = null;
			foreach (ItemDisplayData itemData in this._weaponList)
			{
				short subType = ItemTemplateHelper.GetItemSubType(itemData.Key.ItemType, itemData.Key.TemplateId);
				bool flag = canSelectSubTypes == null || canSelectSubTypes.Contains(subType);
				if (flag)
				{
					this._canSelectWeaponList.Add(itemData);
				}
				bool flag2 = itemKey == itemData.Key;
				if (flag2)
				{
					currentSelectedData = itemData;
				}
			}
			SelectItemConfig config = SelectItemConfig.CreateSingleSelectConfig(new SelectItemRules
			{
				OnlyFromInventory = true
			}, delegate(List<SelectedItemData> itemSelected)
			{
				this.OnSelectItem(itemSelected);
			}, "", null);
			config.MaxSelectCount = 1;
			bool flag3 = currentSelectedData != null;
			if (flag3)
			{
				config.InitialSelectedItems = new List<SelectedItemData>
				{
					new SelectedItemData(currentSelectedData, 1)
				};
			}
			config.AllowEmpty = true;
			config.ExternalItems = this._canSelectWeaponList;
			config.ShowQuickButton = false;
			config.OperationMode = ESelectItemOperationMode.Slot;
			config.SelectItemMode = ESelectItemMode.ItemSelect;
			config.CanSelectLockedItem = true;
			config.HideSourceToggles = true;
			UIElement.SelectItem.SetOnInitArgs(EasyPool.Get<ArgumentBox>().SetObject("SelectItemConfig", config));
			UIManager.Instance.MaskUI(UIElement.SelectItem);
		}

		// Token: 0x060075E3 RID: 30179 RVA: 0x00370120 File Offset: 0x0036E320
		private void OnSelectItem(List<SelectedItemData> itemSelected)
		{
			ItemKey itemKey = (itemSelected.Count > 0 && itemSelected[0].ItemData != null) ? itemSelected[0].ItemData.Key : ItemKey.Invalid;
			LegendaryBookDomainMethod.Call.SaveLegendaryBookWeaponPresetSlotCurrent(this._currCombatSkillType, itemKey);
		}

		// Token: 0x060075E4 RID: 30180 RVA: 0x0037016C File Offset: 0x0036E36C
		private bool IsBreakPlateUnlocked(bool isYin)
		{
			sbyte breakePlateCountState = (this._legendaryBookBreakPlateCounts.ContainsKey(this._currCombatSkillType) && this._ownedCurrBook) ? this._legendaryBookBreakPlateCounts[this._currCombatSkillType] : -1;
			return breakePlateCountState == 2 || breakePlateCountState == (isYin ? 0 : 1);
		}

		// Token: 0x060075E5 RID: 30181 RVA: 0x003701C0 File Offset: 0x0036E3C0
		private bool IsRequiredSkillType(short skillId)
		{
			CombatSkillItem skillConfig = CombatSkill.Instance[skillId];
			bool flag = skillConfig.Type != this._currCombatSkillType;
			bool result;
			if (flag)
			{
				result = false;
			}
			else
			{
				bool flag2 = skillConfig.Type != 2 || ViewLegendaryBook.StuntAllTypeSlot.Contains(this._skillSlotIndex);
				if (flag2)
				{
					result = true;
				}
				else
				{
					bool isDefenseSkill = skillConfig.EquipType == 3;
					result = (ViewLegendaryBook.StuntDefenseTypeSlot.Contains(this._skillSlotIndex) == isDefenseSkill);
				}
			}
			return result;
		}

		// Token: 0x04005882 RID: 22658
		[SerializeField]
		private GameObject legendaryBookOwnerTips;

		// Token: 0x04005883 RID: 22659
		[SerializeField]
		private Refers refersYin;

		// Token: 0x04005884 RID: 22660
		[SerializeField]
		private Refers refersYang;

		// Token: 0x04005885 RID: 22661
		[SerializeField]
		private TextMeshProUGUI txtTitle;

		// Token: 0x04005886 RID: 22662
		[SerializeField]
		private TextMeshProUGUI txtExp;

		// Token: 0x04005887 RID: 22663
		[SerializeField]
		private DisableStyleRoot disableStyleRoot;

		// Token: 0x04005888 RID: 22664
		[Header("页面")]
		[SerializeField]
		private GameObject pageBookView;

		// Token: 0x04005889 RID: 22665
		[SerializeField]
		private GameObject pageNodeView;

		// Token: 0x0400588A RID: 22666
		[SerializeField]
		private GameObject yinYangNodesBg;

		// Token: 0x0400588B RID: 22667
		[SerializeField]
		private CanvasGroup yinYangNodes;

		// Token: 0x0400588C RID: 22668
		[SerializeField]
		private GameObject yinYangNodesEff;

		// Token: 0x0400588D RID: 22669
		[SerializeField]
		private RectTransform nodeViewCoverBg;

		// Token: 0x0400588E RID: 22670
		[SerializeField]
		private GameObject effBookView;

		// Token: 0x0400588F RID: 22671
		[SerializeField]
		private GameObject nodeViewHolder;

		// Token: 0x04005890 RID: 22672
		[Header("交互")]
		[SerializeField]
		private CButton btnClose;

		// Token: 0x04005891 RID: 22673
		[SerializeField]
		private CButton btnCompetitors;

		// Token: 0x04005892 RID: 22674
		[SerializeField]
		private CButton btnFallen;

		// Token: 0x04005893 RID: 22675
		[SerializeField]
		private CButton btnReset;

		// Token: 0x04005894 RID: 22676
		[SerializeField]
		private TooltipInvoker mouseTipFallenBtn;

		// Token: 0x04005895 RID: 22677
		[SerializeField]
		private TooltipInvoker mouseTipCompetitorsBtn;

		// Token: 0x04005896 RID: 22678
		[SerializeField]
		private PointerTrigger pointerTriggerCompetitor;

		// Token: 0x04005897 RID: 22679
		[SerializeField]
		private PointerTrigger pointerTriggerFallen;

		// Token: 0x04005898 RID: 22680
		[SerializeField]
		private LegendaryBookUIButtonGroups skillTypeTogGroupBookPage;

		// Token: 0x04005899 RID: 22681
		[SerializeField]
		private LegendaryBookUIButtonGroups skillTypeTogGroupNodePage;

		// Token: 0x0400589A RID: 22682
		[SerializeField]
		private PresetListWithButtons presetListComponent;

		// Token: 0x0400589B RID: 22683
		[Header("按钮悬停时显示的遮罩")]
		[SerializeField]
		private RectTransform hoverBlocker;

		// Token: 0x0400589C RID: 22684
		[Header("切换时的Material")]
		[SerializeField]
		private ParticleSystem switchTempObj;

		// Token: 0x0400589D RID: 22685
		[SerializeField]
		private UnityEngine.Material matSwitch1;

		// Token: 0x0400589E RID: 22686
		[SerializeField]
		private UnityEngine.Material matSwitch2;

		// Token: 0x0400589F RID: 22687
		[SerializeField]
		private GameObject blocker;

		// Token: 0x040058A0 RID: 22688
		[SerializeField]
		private Transform effTrans1;

		// Token: 0x040058A1 RID: 22689
		[SerializeField]
		private Transform effTrans2;

		// Token: 0x040058A2 RID: 22690
		private float _basicRatio = 1.7777778f;

		// Token: 0x040058A3 RID: 22691
		private static readonly List<int> StuntAllTypeSlot = new List<int>
		{
			0,
			4,
			5
		};

		// Token: 0x040058A4 RID: 22692
		private static readonly List<int> StuntDefenseTypeSlot = new List<int>
		{
			2,
			3
		};

		// Token: 0x040058A5 RID: 22693
		private int _taiwuCharId;

		// Token: 0x040058A6 RID: 22694
		private sbyte _currCombatSkillType;

		// Token: 0x040058A7 RID: 22695
		private bool _ownedCurrBook;

		// Token: 0x040058A8 RID: 22696
		private int _keptBook;

		// Token: 0x040058A9 RID: 22697
		private readonly int[] _bookOwners = new int[14];

		// Token: 0x040058AA RID: 22698
		private List<SectStoryThreeCorpsesCharacter> _sectRanshanThreeCorpses;

		// Token: 0x040058AB RID: 22699
		private LegendaryBookIncrementData _bookData;

		// Token: 0x040058AC RID: 22700
		private int _exp;

		// Token: 0x040058AD RID: 22701
		private int _unlockNeedExp;

		// Token: 0x040058AE RID: 22702
		private List<short> _combatSkillIdList = new List<short>();

		// Token: 0x040058AF RID: 22703
		private readonly List<short> _canSelectCombatSkillIdList = new List<short>();

		// Token: 0x040058B0 RID: 22704
		private readonly Dictionary<short, CombatSkillDisplayData> _skillDataDict = new Dictionary<short, CombatSkillDisplayData>();

		// Token: 0x040058B1 RID: 22705
		private List<ItemDisplayData> _weaponList = new List<ItemDisplayData>();

		// Token: 0x040058B2 RID: 22706
		private readonly List<ItemDisplayData> _canSelectWeaponList = new List<ItemDisplayData>();

		// Token: 0x040058B3 RID: 22707
		private readonly Dictionary<ItemKey, ItemDisplayData> _itemDataDict = new Dictionary<ItemKey, ItemDisplayData>();

		// Token: 0x040058B4 RID: 22708
		private readonly Dictionary<sbyte, sbyte> _legendaryBookBreakPlateCounts = new Dictionary<sbyte, sbyte>();

		// Token: 0x040058B5 RID: 22709
		private readonly Dictionary<sbyte, ItemKey> _legendaryBookWeaponSlot = new Dictionary<sbyte, ItemKey>();

		// Token: 0x040058B6 RID: 22710
		private readonly Dictionary<sbyte, GameData.Utilities.ShortList> _legendaryBookSkillSlot = new Dictionary<sbyte, GameData.Utilities.ShortList>();

		// Token: 0x040058B7 RID: 22711
		private SByteList _legendaryBookBonusCountYin = SByteList.Create();

		// Token: 0x040058B8 RID: 22712
		private SByteList _legendaryBookBonusCountYang = SByteList.Create();

		// Token: 0x040058B9 RID: 22713
		private LegendaryBookEffectItem _currWeaponEffectRefers;

		// Token: 0x040058BA RID: 22714
		private bool _isYinBonusWaitingConfirm;

		// Token: 0x040058BB RID: 22715
		private bool _isYinBreakPlateWaitingConfirm;

		// Token: 0x040058BC RID: 22716
		private int _skillSlotIndex;

		// Token: 0x040058BD RID: 22717
		private bool _waitingCombatEnd;

		// Token: 0x040058BE RID: 22718
		private Sequence _skillTypeChangeSeq;

		// Token: 0x040058BF RID: 22719
		private Sequence _pageChangeSeq;

		// Token: 0x040058C0 RID: 22720
		private readonly ArgumentBox _selectSkillArgBox = new ArgumentBox();

		// Token: 0x040058C1 RID: 22721
		private readonly DialogCmd _confirmUnlockBonusDialogCmd = new DialogCmd();

		// Token: 0x040058C2 RID: 22722
		public Action _onCharactersPageClose;

		// Token: 0x040058C3 RID: 22723
		public static readonly short[] LegendaryBookAdventures = new short[]
		{
			187,
			184,
			185,
			194,
			195,
			183,
			186,
			192,
			190,
			196,
			193,
			188,
			191,
			189
		};

		// Token: 0x040058C4 RID: 22724
		private Dictionary<int, ViewLegendaryBook.ELegendaryBookStatus> bookItemStatus = new Dictionary<int, ViewLegendaryBook.ELegendaryBookStatus>();

		// Token: 0x040058C5 RID: 22725
		private CharacterAvatar _characterAvatarYin;

		// Token: 0x040058C6 RID: 22726
		private CharacterAvatar _characterAvatarYang;

		// Token: 0x02001EB2 RID: 7858
		private enum ELegendaryBookStatus
		{
			// Token: 0x0400CAC6 RID: 51910
			NotShown,
			// Token: 0x0400CAC7 RID: 51911
			OwnByOthers,
			// Token: 0x0400CAC8 RID: 51912
			Contesting,
			// Token: 0x0400CAC9 RID: 51913
			keepByCorpses,
			// Token: 0x0400CACA RID: 51914
			Owning
		}
	}
}
