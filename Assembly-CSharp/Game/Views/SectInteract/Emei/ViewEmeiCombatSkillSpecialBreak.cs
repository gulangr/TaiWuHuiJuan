using System;
using System.Collections.Generic;
using Config;
using FrameWork;
using FrameWork.UISystem.Components;
using FrameWork.UISystem.UIElements;
using Game.Components.Common;
using GameData.Domains.CombatSkill;
using GameData.Domains.Item;
using GameData.Domains.Story;
using GameData.Domains.Story.SectMainStory;
using GameData.Domains.TaiwuEvent;
using GameData.Serializer;
using GameData.Utilities;
using Spine;
using Spine.Unity;
using TMPro;
using UnityEngine;
using UnityEngine.Events;

namespace Game.Views.SectInteract.Emei
{
	// Token: 0x020009F1 RID: 2545
	public class ViewEmeiCombatSkillSpecialBreak : UIBase
	{
		// Token: 0x17000DB2 RID: 3506
		// (get) Token: 0x06007D21 RID: 32033 RVA: 0x003A24BD File Offset: 0x003A06BD
		private bool IsAddingProgress
		{
			get
			{
				return this.typeToggle.GetActiveIndex() == 0;
			}
		}

		// Token: 0x17000DB3 RID: 3507
		// (get) Token: 0x06007D22 RID: 32034 RVA: 0x003A24CD File Offset: 0x003A06CD
		private short SelectedBonus
		{
			get
			{
				return this.IsAddingProgress ? this.emeiAddBonusProgress.SelectedBonus : this.emeiAddBonus.SelectedBonus;
			}
		}

		// Token: 0x06007D23 RID: 32035 RVA: 0x003A24F0 File Offset: 0x003A06F0
		public static string GetColor(sbyte type)
		{
			if (!true)
			{
			}
			string result;
			switch (type)
			{
			case 0:
				result = "assist";
				break;
			case 1:
				result = "white";
				break;
			case 2:
				result = "attack";
				break;
			case 3:
				result = "agile";
				break;
			case 4:
				result = "defense";
				break;
			default:
				throw new ArgumentOutOfRangeException("type", type, null);
			}
			if (!true)
			{
			}
			return result;
		}

		// Token: 0x06007D24 RID: 32036 RVA: 0x003A255C File Offset: 0x003A075C
		public override void OnInit(ArgumentBox argsBox)
		{
		}

		// Token: 0x06007D25 RID: 32037 RVA: 0x003A255F File Offset: 0x003A075F
		private void Awake()
		{
			this.InitTypeToggle();
			this.InitDropDown();
			this.InitAvatar();
			this.InitBonusList();
			this.InitAddProgress();
			this.InitAddBonus();
		}

		// Token: 0x06007D26 RID: 32038 RVA: 0x003A258C File Offset: 0x003A078C
		private void OnEnable()
		{
			this.RequestData();
			this.typeToggle.SetWithoutNotify(0);
			this.emeiAddProgressTipLabelCN.text = "";
			this.emeiAddProgressTipLabelEN.text = "";
			this._emeiAddProgressTipLabel = ((LocalStringManager.CurLanguageType == LocalStringManager.LanguageType.CN) ? this.emeiAddProgressTipLabelCN : this.emeiAddProgressTipLabelEN);
			this._emeiAddProgressTipLabel.text = LanguageKey.LK_CombatSkill_SpecialBreak_ItemSelect_Tip1.TrFormat(Character.DefValue.EmeiWhiteGibbon.GivenName);
		}

		// Token: 0x06007D27 RID: 32039 RVA: 0x003A260B File Offset: 0x003A080B
		private void OnDisable()
		{
			this.particle.gameObject.SetActive(false);
		}

		// Token: 0x06007D28 RID: 32040 RVA: 0x003A2620 File Offset: 0x003A0820
		protected override void OnClick(Transform btn)
		{
			bool flag = btn.name == "ButtonCloseView";
			if (flag)
			{
				this.QuickHide();
			}
		}

		// Token: 0x06007D29 RID: 32041 RVA: 0x003A264B File Offset: 0x003A084B
		public override void QuickHide()
		{
			base.QuickHide();
			TaiwuEventDomainMethod.Call.SetListenerEventActionBoolArg("CombatSkillSpecialBreak", "IsOperated", false);
			TaiwuEventDomainMethod.Call.TriggerListener("CombatSkillSpecialBreak", true);
		}

		// Token: 0x06007D2A RID: 32042 RVA: 0x003A2674 File Offset: 0x003A0874
		public void ShowBubbleAndAnimation(ViewEmeiCombatSkillSpecialBreak.BubbleType bubbleType)
		{
			this.ShowBubble(bubbleType);
			bool flag = bubbleType > ViewEmeiCombatSkillSpecialBreak.BubbleType.Idle;
			if (flag)
			{
				this.UpdateAnimation(bubbleType, false);
			}
			bool flag2 = bubbleType == ViewEmeiCombatSkillSpecialBreak.BubbleType.AddProgressEnough || bubbleType == ViewEmeiCombatSkillSpecialBreak.BubbleType.AddProgressNotEnough;
			if (flag2)
			{
				AudioManager.Instance.PlaySound("SFX_specialbreak_baiyuan_zuanyan", false, false);
			}
		}

		// Token: 0x06007D2B RID: 32043 RVA: 0x003A26BE File Offset: 0x003A08BE
		private void RequestData()
		{
			StoryDomainMethod.AsyncCall.GetSectEmeiSpecialBreakDisplayData(this, delegate(int offset1, RawDataPool pool1)
			{
				Serializer.Deserialize(pool1, offset1, ref this._data);
				CombatSkillDomainMethod.AsyncCall.GetCombatSkillDisplayDataForList(null, SingletonObject.getInstance<BasicGameData>().TaiwuCharId, this._data.LearnedCombatSkills, delegate(int offset2, RawDataPool pool2)
				{
					List<CombatSkillDisplayDataForList> listData = new List<CombatSkillDisplayDataForList>();
					Serializer.Deserialize(pool2, offset2, ref listData);
					this.emeiAddBonus.Set(listData, this._data.SectEmeiBreakBonusData, this._data.IsAdvanceUnlocked);
					this.emeiAddBonusProgress.Items = this._data.Items;
					bool flag = this.emeiAddBonusProgress.SelectedBonus >= 0;
					if (flag)
					{
						this.emeiAddBonusProgress.AnimProgressBar(this._data.SectEmeiBreakBonusData[this.emeiAddBonusProgress.SelectedBonus]);
					}
					bool flag2 = this._addingBonusSkillTemplateId >= 0;
					if (flag2)
					{
						this.emeiAddBonus.SetSkill(this._addingBonusSkillTemplateId);
						this._addingBonusSkillTemplateId = -1;
					}
					this.OnOperatingTypeChange(-1, -1);
					this.UpdateBonusList();
				});
			});
		}

		// Token: 0x06007D2C RID: 32044 RVA: 0x003A26D4 File Offset: 0x003A08D4
		private void InitTypeToggle()
		{
			this.typeToggle.Init(-1);
			ToggleGroupHotkeyController.Set(this.Element, this.typeToggle, 0, null);
			this.typeToggle.OnActiveIndexChange += this.OnOperatingTypeChange;
		}

		// Token: 0x06007D2D RID: 32045 RVA: 0x003A2710 File Offset: 0x003A0910
		private void InitDropDown()
		{
			List<string> options = new List<string>();
			options.Add(LanguageKey.LK_Default.Tr());
			options.Add(LanguageKey.LK_CombatSkill_SpecialBreak_Can_Use_Count.Tr());
			options.Add(LanguageKey.LK_CombatSkill_SpecialBreak_ConvertToExp_Content_ProgressValue.Tr());
			this.dropdown.AddOptions(options);
			this.dropdown.onValueChanged.RemoveAllListeners();
			this.dropdown.onValueChanged.AddListener(new UnityAction<int>(this.OnDropdownValueChanged));
		}

		// Token: 0x06007D2E RID: 32046 RVA: 0x003A2792 File Offset: 0x003A0992
		private void InitAvatar()
		{
			this.spine.AnimationState.Complete += delegate(TrackEntry entry)
			{
				bool flag = entry.Animation.Name != "idle";
				if (flag)
				{
					this.spine.AnimationState.SetAnimation(0, "idle", true);
				}
			};
			this.avatarName.text = Character.DefValue.EmeiWhiteGibbon.GivenName;
		}

		// Token: 0x06007D2F RID: 32047 RVA: 0x003A27C8 File Offset: 0x003A09C8
		private void InitBonusList()
		{
			this.bonusToggle.Init(-1);
			this.bonusToggle.OnActiveIndexChange += this.OnBonusTypeChange;
			this.bonusScroll.InitPageCount();
			this.bonusScroll.OnItemRender += this.OnBonusRender;
		}

		// Token: 0x06007D30 RID: 32048 RVA: 0x003A281F File Offset: 0x003A0A1F
		private void InitAddProgress()
		{
			this.emeiAddBonusProgress.Init(new Action<short, bool, List<ItemKey>>(this.OnConfirmAddProgress));
		}

		// Token: 0x06007D31 RID: 32049 RVA: 0x003A283A File Offset: 0x003A0A3A
		private void InitAddBonus()
		{
			this.emeiAddBonus.Init(new Action<short, short>(this.OnConfirmRemoveBonus), new Action<short, short, short>(this.OnConfirmAddBonus), new Action<short, short, short, short, short>(this.OnConfirmAddBonus), new Action(this.UpdateBonusList));
		}

		// Token: 0x06007D32 RID: 32050 RVA: 0x003A287C File Offset: 0x003A0A7C
		private void UpdateBonusList()
		{
			this._filteredBonusList.Clear();
			int type = this.bonusToggle.GetActiveIndex();
			bool flag = type == 0;
			if (flag)
			{
				foreach (SkillBreakPlateGridBonusTypeItem config in ((IEnumerable<SkillBreakPlateGridBonusTypeItem>)SkillBreakPlateGridBonusType.Instance))
				{
					bool isExtraBonus = config.IsExtraBonus;
					if (isExtraBonus)
					{
						this._filteredBonusList.Add(config.TemplateId);
					}
				}
			}
			else
			{
				foreach (SkillBreakPlateGridBonusTypeItem config2 in ((IEnumerable<SkillBreakPlateGridBonusTypeItem>)SkillBreakPlateGridBonusType.Instance))
				{
					bool flag2 = config2.IsExtraBonus && (int)config2.FilterGroup == type - 1;
					if (flag2)
					{
						this._filteredBonusList.Add(config2.TemplateId);
					}
				}
			}
			switch (this.dropdown.value)
			{
			case 0:
				this._filteredBonusList.Sort(new Comparison<short>(this.CompareByDefault));
				break;
			case 1:
				this._filteredBonusList.Sort(new Comparison<short>(this.CompareByCount));
				break;
			case 2:
				this._filteredBonusList.Sort(new Comparison<short>(this.CompareByProgress));
				break;
			}
			this.bonusScroll.SetDataCount(this._filteredBonusList.Count);
		}

		// Token: 0x06007D33 RID: 32051 RVA: 0x003A2A00 File Offset: 0x003A0C00
		private void UpdateSelectedBonus()
		{
			for (int i = 0; i < this._filteredBonusList.Count; i++)
			{
				GameObject obj = this.bonusScroll.GetActiveCell(i);
				bool flag = obj != null;
				if (flag)
				{
					obj.GetComponent<EmeiSpecialBreakBonus>().SetSelected(this.SelectedBonus);
				}
			}
		}

		// Token: 0x06007D34 RID: 32052 RVA: 0x003A2A54 File Offset: 0x003A0C54
		private void OnOperatingTypeChange(int newIndex, int oldIndex)
		{
			bool flag = newIndex != oldIndex;
			if (flag)
			{
				this.mask.SetActive(false);
				this.particle.gameObject.SetActive(false);
			}
			this.UpdateBonusList();
			bool isAddingProgress = this.IsAddingProgress;
			if (isAddingProgress)
			{
				this.emeiAddBonusProgress.gameObject.SetActive(true);
				this.emeiAddBonus.gameObject.SetActive(false);
				this.emeiAddProgressTip.SetActive(true);
			}
			else
			{
				this.emeiAddBonusProgress.gameObject.SetActive(false);
				this.emeiAddBonus.gameObject.SetActive(true);
				this.emeiAddProgressTip.SetActive(false);
			}
		}

		// Token: 0x06007D35 RID: 32053 RVA: 0x003A2B07 File Offset: 0x003A0D07
		private void OnDropdownValueChanged(int _)
		{
			this.UpdateBonusList();
		}

		// Token: 0x06007D36 RID: 32054 RVA: 0x003A2B11 File Offset: 0x003A0D11
		private void OnBonusTypeChange(int _, int __)
		{
			this.UpdateBonusList();
		}

		// Token: 0x06007D37 RID: 32055 RVA: 0x003A2B1C File Offset: 0x003A0D1C
		private void OnBonusRender(int index, GameObject obj)
		{
			EmeiSpecialBreakBonus item = obj.GetComponent<EmeiSpecialBreakBonus>();
			short templateId = this._filteredBonusList[index];
			item.Init(new Action<short>(this.OnSelectBonus));
			SectEmeiBreakBonusData data;
			bool flag = this._data.SectEmeiBreakBonusData.TryGetValue(templateId, out data);
			if (flag)
			{
				item.Set(templateId, data.BonusProgress, data.BonusCount, true);
			}
			else
			{
				item.Set(templateId, 0, 0, true);
			}
			item.SetCanInteract(this.IsAddingProgress || this.emeiAddBonus.CanInteractBonus(templateId, data.BonusCount));
			item.SetSelected(this.SelectedBonus == templateId);
		}

		// Token: 0x06007D38 RID: 32056 RVA: 0x003A2BC0 File Offset: 0x003A0DC0
		private void OnSelectBonus(short templateId)
		{
			bool flag = !this._data.SectEmeiBreakBonusData.ContainsKey(templateId);
			if (flag)
			{
				this._data.SectEmeiBreakBonusData[templateId] = new SectEmeiBreakBonusData
				{
					TemplateId = templateId,
					BonusProgress = 0,
					BonusCount = 0
				};
			}
			bool isAddingProgress = this.IsAddingProgress;
			if (isAddingProgress)
			{
				this.emeiAddBonusProgress.Set(this._data.SectEmeiBreakBonusData[templateId]);
			}
			else
			{
				this.emeiAddBonus.Set(this._data.SectEmeiBreakBonusData[templateId]);
			}
			this.UpdateSelectedBonus();
		}

		// Token: 0x06007D39 RID: 32057 RVA: 0x003A2C68 File Offset: 0x003A0E68
		private void OnConfirmAddProgress(short bonusId, bool isEnough, List<ItemKey> items)
		{
			this.ShowBubbleAndAnimation(isEnough ? ViewEmeiCombatSkillSpecialBreak.BubbleType.AddProgressEnough : ViewEmeiCombatSkillSpecialBreak.BubbleType.AddProgressNotEnough);
			this.particle.gameObject.SetActive(true);
			this.particle.Play();
			StoryDomainMethod.AsyncCall.EmeiTransferBonusProgress(null, bonusId, items, delegate(int offset, RawDataPool pool)
			{
				bool success = false;
				Serializer.Deserialize(pool, offset, ref success);
				this.RequestData();
			});
		}

		// Token: 0x06007D3A RID: 32058 RVA: 0x003A2CB7 File Offset: 0x003A0EB7
		private void OnConfirmRemoveBonus(short skillTemplateId, short bonusTemplateId)
		{
			this.ShowBubbleAndAnimation(ViewEmeiCombatSkillSpecialBreak.BubbleType.RemoveBreakBonus);
			AudioManager.Instance.PlaySound("SFX_specialbreak_take", false, false);
			this._addingBonusSkillTemplateId = skillTemplateId;
			StoryDomainMethod.AsyncCall.RemoveEmeiSkillBreakBonus(null, skillTemplateId, bonusTemplateId, delegate(int offset, RawDataPool pool)
			{
				bool result = false;
				Serializer.Deserialize(pool, offset, ref result);
				this.RequestData();
			});
		}

		// Token: 0x06007D3B RID: 32059 RVA: 0x003A2CF0 File Offset: 0x003A0EF0
		private void OnConfirmAddBonus(short skillTemplateId, short addBonusId, short replaceBonusId)
		{
			this.ShowBubbleAndAnimation(ViewEmeiCombatSkillSpecialBreak.BubbleType.AddBreakBonus);
			AudioManager.Instance.PlaySound("SFX_specialbreak_baiyuan_lingwu", false, false);
			this.particle.gameObject.SetActive(true);
			this.particle.Play();
			this._addingBonusSkillTemplateId = skillTemplateId;
			bool flag = replaceBonusId >= 0;
			if (flag)
			{
				StoryDomainMethod.Call.RemoveEmeiSkillBreakBonus(-1, skillTemplateId, replaceBonusId);
			}
			StoryDomainMethod.AsyncCall.AddEmeiSkillBreakBonus(null, skillTemplateId, addBonusId, delegate(int offset, RawDataPool pool)
			{
				bool result = false;
				Serializer.Deserialize(pool, offset, ref result);
				this.RequestData();
			});
		}

		// Token: 0x06007D3C RID: 32060 RVA: 0x003A2D68 File Offset: 0x003A0F68
		private void OnConfirmAddBonus(short skillTemplateId, short addBonusId, short replaceBonusId, short addBonusId2, short replaceBonusId2)
		{
			this.ShowBubbleAndAnimation(ViewEmeiCombatSkillSpecialBreak.BubbleType.AddBreakBonus);
			AudioManager.Instance.PlaySound("SFX_specialbreak_baiyuan_lingwu", false, false);
			this.particle.gameObject.SetActive(true);
			this.particle.Play();
			this._addingBonusSkillTemplateId = skillTemplateId;
			bool flag = replaceBonusId >= 0;
			if (flag)
			{
				StoryDomainMethod.Call.RemoveEmeiSkillBreakBonus(-1, skillTemplateId, replaceBonusId);
			}
			bool flag2 = replaceBonusId2 >= 0;
			if (flag2)
			{
				StoryDomainMethod.Call.RemoveEmeiSkillBreakBonus(-1, skillTemplateId, replaceBonusId2);
			}
			StoryDomainMethod.AsyncCall.AddEmeiSkillBreakBonus(null, skillTemplateId, addBonusId, delegate(int offset, RawDataPool pool)
			{
				bool result = false;
				Serializer.Deserialize(pool, offset, ref result);
				StoryDomainMethod.AsyncCall.AddEmeiSkillBreakBonus(null, skillTemplateId, addBonusId2, delegate(int offset2, RawDataPool pool2)
				{
					Serializer.Deserialize(pool2, offset2, ref result);
					this.RequestData();
				});
			});
		}

		// Token: 0x06007D3D RID: 32061 RVA: 0x003A2E24 File Offset: 0x003A1024
		private void UpdateAnimation(ViewEmeiCombatSkillSpecialBreak.BubbleType bubbleType, bool loop = false)
		{
			string key = this.GetSpineAnimationKey(bubbleType);
			bool flag = key != null;
			if (flag)
			{
				this.spine.AnimationState.SetAnimation(0, key, loop);
			}
		}

		// Token: 0x06007D3E RID: 32062 RVA: 0x003A2E58 File Offset: 0x003A1058
		private string GetSpineAnimationKey(ViewEmeiCombatSkillSpecialBreak.BubbleType bubbleType)
		{
			string result;
			switch (bubbleType)
			{
			case ViewEmeiCombatSkillSpecialBreak.BubbleType.Idle:
				result = "idle";
				break;
			case ViewEmeiCombatSkillSpecialBreak.BubbleType.AddProgressNotEnough:
				result = "understand";
				break;
			case ViewEmeiCombatSkillSpecialBreak.BubbleType.AddProgressEnough:
				result = "understand";
				break;
			case ViewEmeiCombatSkillSpecialBreak.BubbleType.AddBreakBonus:
				result = "give";
				break;
			default:
				result = null;
				break;
			}
			return result;
		}

		// Token: 0x06007D3F RID: 32063 RVA: 0x003A2EA8 File Offset: 0x003A10A8
		private void ShowBubble(ViewEmeiCombatSkillSpecialBreak.BubbleType bubbleType)
		{
			this.bubble.SetActive(true);
			if (!true)
			{
			}
			string text2;
			switch (bubbleType)
			{
			case ViewEmeiCombatSkillSpecialBreak.BubbleType.Idle:
				text2 = LanguageKey.LK_CombatSkill_SpecialBreak_Bubble_Idle_Baiyuan.Tr();
				break;
			case ViewEmeiCombatSkillSpecialBreak.BubbleType.AddProgressNotEnough:
				text2 = LanguageKey.LK_CombatSkill_SpecialBreak_Bubble_AddBonusProgress_Baiyuan_1.Tr();
				break;
			case ViewEmeiCombatSkillSpecialBreak.BubbleType.AddProgressEnough:
				text2 = LanguageKey.LK_CombatSkill_SpecialBreak_Bubble_AddBonusProgress_Baiyuan_2.Tr();
				break;
			case ViewEmeiCombatSkillSpecialBreak.BubbleType.AddBreakBonus:
				text2 = LanguageKey.LK_CombatSkill_SpecialBreak_Bubble_AddBreakBonus_Baiyuan.Tr();
				break;
			case ViewEmeiCombatSkillSpecialBreak.BubbleType.RemoveBreakBonus:
				text2 = LanguageKey.LK_CombatSkill_SpecialBreak_Bubble_RemoveBreakBonus_Baiyuan.Tr();
				break;
			default:
				text2 = "";
				break;
			}
			if (!true)
			{
			}
			string text = text2;
			this.bubbleText.text = text;
			bool flag = this._closeBubbleCoroutine != null;
			if (flag)
			{
				SingletonObject.getInstance<YieldHelper>().StopYield(this._closeBubbleCoroutine);
			}
			this._closeBubbleCoroutine = SingletonObject.getInstance<YieldHelper>().DelaySecondsDo(5f, delegate
			{
				bool flag2 = this.bubble != null;
				if (flag2)
				{
					this.bubble.SetActive(false);
				}
			});
		}

		// Token: 0x06007D40 RID: 32064 RVA: 0x003A2F80 File Offset: 0x003A1180
		private int CompareCanUse(short a, short b)
		{
			bool canUseA = this.emeiAddBonus.CanInteractBonusPure(a);
			bool canUseB = this.emeiAddBonus.CanInteractBonusPure(b);
			return (canUseA == canUseB) ? 0 : (canUseA ? -1 : 1);
		}

		// Token: 0x06007D41 RID: 32065 RVA: 0x003A2FBC File Offset: 0x003A11BC
		private int CompareByDefault(short a, short b)
		{
			bool flag = !this.IsAddingProgress;
			if (flag)
			{
				int compareCanUse = this.CompareCanUse(a, b);
				bool flag2 = compareCanUse != 0;
				if (flag2)
				{
					return compareCanUse;
				}
			}
			return a.CompareTo(b);
		}

		// Token: 0x06007D42 RID: 32066 RVA: 0x003A2FFC File Offset: 0x003A11FC
		private int CompareByCount(short a, short b)
		{
			bool flag = !this.IsAddingProgress;
			if (flag)
			{
				int compareCanUse = this.CompareCanUse(a, b);
				bool flag2 = compareCanUse != 0;
				if (flag2)
				{
					return compareCanUse;
				}
			}
			SectEmeiBreakBonusData dataA;
			bool flag3 = !this._data.SectEmeiBreakBonusData.TryGetValue(a, out dataA);
			if (flag3)
			{
				dataA = new SectEmeiBreakBonusData
				{
					BonusCount = 0,
					BonusProgress = 0
				};
			}
			SectEmeiBreakBonusData dataB;
			bool flag4 = !this._data.SectEmeiBreakBonusData.TryGetValue(b, out dataB);
			if (flag4)
			{
				dataB = new SectEmeiBreakBonusData
				{
					BonusCount = 0,
					BonusProgress = 0
				};
			}
			bool flag5 = dataA.BonusCount == dataB.BonusCount;
			int result;
			if (flag5)
			{
				result = a.CompareTo(b);
			}
			else
			{
				result = -dataA.BonusCount.CompareTo(dataB.BonusCount);
			}
			return result;
		}

		// Token: 0x06007D43 RID: 32067 RVA: 0x003A30E0 File Offset: 0x003A12E0
		private int CompareByProgress(short a, short b)
		{
			bool flag = !this.IsAddingProgress;
			if (flag)
			{
				int compareCanUse = this.CompareCanUse(a, b);
				bool flag2 = compareCanUse != 0;
				if (flag2)
				{
					return compareCanUse;
				}
			}
			SectEmeiBreakBonusData dataA;
			bool flag3 = !this._data.SectEmeiBreakBonusData.TryGetValue(a, out dataA);
			if (flag3)
			{
				dataA = new SectEmeiBreakBonusData
				{
					BonusCount = 0,
					BonusProgress = 0
				};
			}
			SectEmeiBreakBonusData dataB;
			bool flag4 = !this._data.SectEmeiBreakBonusData.TryGetValue(b, out dataB);
			if (flag4)
			{
				dataB = new SectEmeiBreakBonusData
				{
					BonusCount = 0,
					BonusProgress = 0
				};
			}
			bool flag5 = dataA.BonusProgress == dataB.BonusProgress;
			int result;
			if (flag5)
			{
				result = a.CompareTo(b);
			}
			else
			{
				result = -dataA.BonusProgress.CompareTo(dataB.BonusProgress);
			}
			return result;
		}

		// Token: 0x04005F34 RID: 24372
		[SerializeField]
		private CToggleGroup typeToggle;

		// Token: 0x04005F35 RID: 24373
		[SerializeField]
		private CDropdown dropdown;

		// Token: 0x04005F36 RID: 24374
		[SerializeField]
		private SkeletonGraphic spine;

		// Token: 0x04005F37 RID: 24375
		[SerializeField]
		private TextMeshProUGUI avatarName;

		// Token: 0x04005F38 RID: 24376
		[SerializeField]
		private GameObject bubble;

		// Token: 0x04005F39 RID: 24377
		[SerializeField]
		private TextMeshProUGUI bubbleText;

		// Token: 0x04005F3A RID: 24378
		[SerializeField]
		private CToggleGroup bonusToggle;

		// Token: 0x04005F3B RID: 24379
		[SerializeField]
		private InfinityScroll bonusScroll;

		// Token: 0x04005F3C RID: 24380
		[SerializeField]
		private EmeiAddBonusProgress emeiAddBonusProgress;

		// Token: 0x04005F3D RID: 24381
		[SerializeField]
		private EmeiAddBonus emeiAddBonus;

		// Token: 0x04005F3E RID: 24382
		[SerializeField]
		private GameObject emeiAddProgressTip;

		// Token: 0x04005F3F RID: 24383
		[SerializeField]
		private TextMeshProUGUI emeiAddProgressTipLabelCN;

		// Token: 0x04005F40 RID: 24384
		[SerializeField]
		private TextMeshProUGUI emeiAddProgressTipLabelEN;

		// Token: 0x04005F41 RID: 24385
		[SerializeField]
		private ParticleSystem particle;

		// Token: 0x04005F42 RID: 24386
		[SerializeField]
		private GameObject mask;

		// Token: 0x04005F43 RID: 24387
		public static readonly SectEmeiBreakBonusData InvalidBonus = new SectEmeiBreakBonusData
		{
			TemplateId = -1,
			BonusCount = 0,
			BonusProgress = 0
		};

		// Token: 0x04005F44 RID: 24388
		private TextMeshProUGUI _emeiAddProgressTipLabel;

		// Token: 0x04005F45 RID: 24389
		private SectEmeiSpecialBreakDisplayData _data;

		// Token: 0x04005F46 RID: 24390
		private List<short> _filteredBonusList = new List<short>();

		// Token: 0x04005F47 RID: 24391
		private short _addingBonusSkillTemplateId = -1;

		// Token: 0x04005F48 RID: 24392
		private Coroutine _closeBubbleCoroutine;

		// Token: 0x02001F80 RID: 8064
		public enum BubbleType
		{
			// Token: 0x0400CDC8 RID: 52680
			Idle,
			// Token: 0x0400CDC9 RID: 52681
			AddProgressNotEnough,
			// Token: 0x0400CDCA RID: 52682
			AddProgressEnough,
			// Token: 0x0400CDCB RID: 52683
			AddBreakBonus,
			// Token: 0x0400CDCC RID: 52684
			RemoveBreakBonus
		}
	}
}
