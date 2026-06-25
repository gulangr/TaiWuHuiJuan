using System;
using System.Collections.Generic;
using Config;
using DG.Tweening;
using FrameWork;
using FrameWork.UI.LanguageRule;
using FrameWork.UISystem.Components;
using FrameWork.UISystem.Components.EffectPlayer;
using FrameWork.UISystem.UIElements;
using Game.Components.SortAndFilter;
using Game.Components.SortAndFilter.ProfessionSkill;
using Game.Views;
using GameData.Common;
using GameData.Domains.Character;
using GameData.Domains.Global;
using GameData.Domains.Taiwu;
using GameData.Domains.Taiwu.Profession;
using GameData.Domains.TaiwuEvent;
using GameData.GameDataBridge;
using GameData.Serializer;
using GameData.Utilities;
using TMPro;
using UILogic;
using UnityEngine;
using UnityEngine.UI;

// Token: 0x02000408 RID: 1032
public class ViewProfession : UIBase
{
	// Token: 0x1700064C RID: 1612
	// (get) Token: 0x06003D64 RID: 15716 RVA: 0x001ED834 File Offset: 0x001EBA34
	private ProfessionModel Model
	{
		get
		{
			return SingletonObject.getInstance<ProfessionModel>();
		}
	}

	// Token: 0x1700064D RID: 1613
	// (get) Token: 0x06003D65 RID: 15717 RVA: 0x001ED83B File Offset: 0x001EBA3B
	private int CurrentDate
	{
		get
		{
			return SingletonObject.getInstance<BasicGameData>().CurrDate;
		}
	}

	// Token: 0x1700064E RID: 1614
	// (get) Token: 0x06003D66 RID: 15718 RVA: 0x001ED847 File Offset: 0x001EBA47
	private int TaiwuCharId
	{
		get
		{
			return SingletonObject.getInstance<BasicGameData>().TaiwuCharId;
		}
	}

	// Token: 0x06003D67 RID: 15719 RVA: 0x001ED854 File Offset: 0x001EBA54
	public override void OnInit(ArgumentBox argsBox)
	{
		bool flag = !this._inited;
		if (flag)
		{
			this.InitScroll();
			this.InitSortAndFilter();
			this.InitSkillLevelFilter();
			this.InitEquipSkillSlots();
			this.InitBgParticles();
		}
		UIElement element = this.Element;
		element.OnListenerIdReady = (Action)Delegate.Combine(element.OnListenerIdReady, new Action(this.RefreshAllAssumingDataExists));
		this._inited = true;
	}

	// Token: 0x06003D68 RID: 15720 RVA: 0x001ED8C4 File Offset: 0x001EBAC4
	private void InitBgParticles()
	{
		this._bgParticles = new List<ParticleSystemAlphaController>();
		for (int i = 0; i < this.bgParticle.childCount; i++)
		{
			Transform root = this.bgParticle.GetChild(i);
			ParticleSystemAlphaController controller = root.GetComponent<ParticleSystemAlphaController>();
			this._bgParticles.Add(controller);
		}
	}

	// Token: 0x06003D69 RID: 15721 RVA: 0x001ED91A File Offset: 0x001EBB1A
	private void Start()
	{
	}

	// Token: 0x06003D6A RID: 15722 RVA: 0x001ED920 File Offset: 0x001EBB20
	private void InitEquipSkillSlots()
	{
		RectTransform entry = this.equipSkillSlots;
		this._equipSkillItems = new List<List<UILogic.ProfessionSkillItem>>();
		int levelCount = 4;
		for (int i = 0; i < levelCount; i++)
		{
			List<UILogic.ProfessionSkillItem> subList = new List<UILogic.ProfessionSkillItem>();
			int slotCount = levelCount - i;
			for (int j = 0; j < slotCount; j++)
			{
				Transform item = entry.Find(string.Format("EquipSkill_{0}_{1}", i, j));
				subList.Add(item.GetComponent<UILogic.ProfessionSkillItem>());
			}
			this._equipSkillItems.Add(subList);
		}
	}

	// Token: 0x06003D6B RID: 15723 RVA: 0x001ED9B4 File Offset: 0x001EBBB4
	private void InitScroll()
	{
		this.professionScroll.OnItemRender += this.OnProfessionLineRender;
		this.professionSkillScroll.OnItemRender += this.OnProfessionSkillRender;
	}

	// Token: 0x06003D6C RID: 15724 RVA: 0x001ED9E7 File Offset: 0x001EBBE7
	private void InitSortAndFilter()
	{
		this._professionSkillSortAndFilterController = new ProfessionSkillSortAndFilterController(this.sortAndFilter);
		this._professionSkillSortAndFilterController.Init(new Action(this.OnProfessionSkillSortChanged), "Profession2SkillSort");
	}

	// Token: 0x06003D6D RID: 15725 RVA: 0x001EDA18 File Offset: 0x001EBC18
	private void InitSkillLevelFilter()
	{
		bool flag = this.skillLevelToggleGroup == null;
		if (!flag)
		{
			this.skillLevelToggleGroup.Init(0);
			this.skillLevelToggleGroup.OnActiveIndexChange -= this.OnSkillLevelFilterChanged;
			this.skillLevelToggleGroup.OnActiveIndexChange += this.OnSkillLevelFilterChanged;
			this._selectedSkillLevelFilter = -1;
			this.SetSkillListMode(ViewProfession.SkillListMode.ProfessionList);
			for (int i = 0; i < this.skillLevelToggleGroup.transform.childCount; i++)
			{
				Transform toggleTransform = this.skillLevelToggleGroup.transform.GetChild(i);
				CToggle toggle = toggleTransform.GetComponent<CToggle>();
				bool flag2 = toggle == null;
				if (!flag2)
				{
					Graphic targetGraphic = toggle.targetGraphic;
					TooltipInvoker tipDisplayer = targetGraphic.gameObject.GetOrAddComponent<TooltipInvoker>();
					tipDisplayer.Type = TipType.SingleDesc;
					TooltipInvoker tooltipInvoker = tipDisplayer;
					if (tooltipInvoker.RuntimeParam == null)
					{
						tooltipInvoker.RuntimeParam = new ArgumentBox();
					}
					ArgumentBox tipParam = tipDisplayer.RuntimeParam;
					bool flag3 = i == 0;
					string tooltipText;
					if (flag3)
					{
						tooltipText = LocalStringManager.Get(LanguageKey.LK_Profession_Level_Filter_All);
					}
					else
					{
						string chineseI = LocalStringManager.Get(string.Format("LK_Num_{0}", i));
						tooltipText = LocalStringManager.GetFormat(LanguageKey.LK_Profession_Level_Filter_Level, chineseI);
					}
					tipParam.Set("arg0", tooltipText);
				}
			}
		}
	}

	// Token: 0x06003D6E RID: 15726 RVA: 0x001EDB70 File Offset: 0x001EBD70
	private void RefreshExtraSkill()
	{
		bool flag = !this.Model.IsExtraProfessionSkillUnlocked;
		if (flag)
		{
			this.extraSkill.gameObject.SetActive(false);
		}
		else
		{
			bool isExtraProfessionSkillReady = this.Model.IsExtraProfessionSkillReady();
			this.extraSkill.Refresh(isExtraProfessionSkillReady, new Action(ProfessionBottomMenu.ExtraProfessionSkillSelectFeature));
			this.extraSkill.RefreshTip();
		}
	}

	// Token: 0x06003D6F RID: 15727 RVA: 0x001EDBD8 File Offset: 0x001EBDD8
	private void OnProfessionLineRender(int index, GameObject refers)
	{
		int professionId = this._professionIds[index];
		ProfessionItem professionConfig = Profession.Instance[professionId];
		BottomProfessionScrollLine professionItem = refers.GetComponent<BottomProfessionScrollLine>();
		CImage professionIcon = professionItem.professionIcon;
		RectTransform skillLayout = professionItem.skillLayout;
		TextMeshProUGUI professionNameLabel = professionItem.professionNameLabel;
		professionIcon.SetSprite(string.Format("Profession_2_{0}", professionId), false, null);
		professionNameLabel.text = professionConfig.Name;
		LanguageRuleTips ruleTip;
		bool flag = professionNameLabel.TryGetComponent<LanguageRuleTips>(out ruleTip);
		if (flag)
		{
			ruleTip.Refresh();
		}
		ProfessionData taiwuProfession = this.Model.GetProfessionData(professionId);
		int skillCount = (professionConfig.ExtraProfessionSkill != -1) ? 4 : 3;
		this.RefreshSkillItems(skillLayout, skillCount, professionId);
		ViewProfession.RefreshProgerss(taiwuProfession, professionItem);
		this.RefreshProfessionTips(professionId, professionIcon.GetComponent<TooltipInvoker>());
	}

	// Token: 0x06003D70 RID: 15728 RVA: 0x001EDC9C File Offset: 0x001EBE9C
	private void OnProfessionSkillRender(int index, GameObject refers)
	{
		UILogic.ProfessionSkillItem item = refers.GetComponent<UILogic.ProfessionSkillItem>();
		ProfessionSkillSortData skillData = this._filteredProfessionSkillDataList[index];
		this.RefreshSkillItem(item, skillData);
	}

	// Token: 0x06003D71 RID: 15729 RVA: 0x001EDCC8 File Offset: 0x001EBEC8
	private void RefreshProfessionTips(int professionId, TooltipInvoker mouseTipDisplayer)
	{
		if (mouseTipDisplayer.RuntimeParam == null)
		{
			mouseTipDisplayer.RuntimeParam = new ArgumentBox();
		}
		mouseTipDisplayer.RuntimeParam.Set("ProfessionId", professionId);
	}

	// Token: 0x06003D72 RID: 15730 RVA: 0x001EDD00 File Offset: 0x001EBF00
	private static void RefreshProgerss(ProfessionData taiwuProfession, BottomProfessionScrollLine line)
	{
		List<CImage> splitList = line.split;
		CImage progressImage = line.progressImage;
		CImage extraProgressImage = line.extraProgressImage;
		TextMeshProUGUI progressLabel = line.progressLabel;
		GameObject progressArea = line.progressArea;
		TooltipInvoker tip = line.progressTip;
		bool isMax = taiwuProfession.Seniority == 3000000;
		bool isExtra = isMax && taiwuProfession.ExtraSeniority > 0;
		int maxSeniority = isExtra ? 1500000 : 3000000;
		int currentSeniority = isExtra ? taiwuProfession.ExtraSeniority : taiwuProfession.Seniority;
		progressLabel.text = string.Format("{0}%", currentSeniority * 100 / maxSeniority);
		progressImage.fillAmount = (float)taiwuProfession.Seniority / 3000000f;
		extraProgressImage.fillAmount = (float)taiwuProfession.ExtraSeniority / 1500000f;
		progressArea.SetActive(true);
		ViewProfession.RefreshProgressSplitItems(splitList, taiwuProfession, isExtra);
		ViewProfession.ProgressTipsHelper.RefreshTips(tip, taiwuProfession, ViewProfession.ProgressTipsHelper.TipsMode.Progress, -1);
	}

	// Token: 0x06003D73 RID: 15731 RVA: 0x001EDDE8 File Offset: 0x001EBFE8
	private static void RefreshProgressSplitItems(List<CImage> splitList, ProfessionData taiwuProfession, bool isExtra)
	{
		float totalWidth = splitList[0].transform.parent.GetComponent<RectTransform>().rect.width;
		for (int i = 0; i < splitList.Count; i++)
		{
			int skillId = GameData.Domains.Taiwu.Profession.SharedMethods.GetSkillId(taiwuProfession.TemplateId, i);
			Config.ProfessionSkillItem skillConfig = ProfessionSkill.Instance[skillId];
			short needPercent = skillConfig.UnlockSeniority;
			float x = totalWidth * (float)needPercent / 100f;
			CImage splitImage = splitList[i];
			splitImage.rectTransform.anchoredPosition = new Vector3(x, 0f, 0f);
			ViewProfession.ProgressTipsHelper.RefreshTips(splitImage.GetComponent<TooltipInvoker>(), taiwuProfession, ViewProfession.ProgressTipsHelper.TipsMode.Segment, i);
		}
	}

	// Token: 0x06003D74 RID: 15732 RVA: 0x001EDEA4 File Offset: 0x001EC0A4
	private void RefreshSkillItems(RectTransform skillLayout, int skillCount, int professionId)
	{
		for (int i = 0; i < skillLayout.childCount; i++)
		{
			Transform skillItemTrans = skillLayout.GetChild(i);
			bool hasSkill = i < skillCount;
			skillItemTrans.gameObject.SetActive(hasSkill);
			bool flag = hasSkill;
			if (flag)
			{
				UILogic.ProfessionSkillItem skillItem = skillItemTrans.GetComponent<UILogic.ProfessionSkillItem>();
				this.RefreshSkillItem(skillItem, professionId, i);
			}
		}
	}

	// Token: 0x06003D75 RID: 15733 RVA: 0x001EDF00 File Offset: 0x001EC100
	private void RefreshSkillItem(UILogic.ProfessionSkillItem skillItem, int professionId, int i)
	{
		ProfessionSkillSortData skillData = this.BuildProfessionSkillSortData(professionId, i);
		this.RefreshSkillItem(skillItem, skillData);
	}

	// Token: 0x06003D76 RID: 15734 RVA: 0x001EDF20 File Offset: 0x001EC120
	private void RefreshSkillItem(UILogic.ProfessionSkillItem skillItem, ProfessionSkillSortData skillData)
	{
		UILogic.ProfessionSkillItem.RefreshConfig refreshConfig = new UILogic.ProfessionSkillItem.RefreshConfig
		{
			ProfessionData = skillData.ProfessionData,
			SkillIndexOfProfession = skillData.SkillIndex,
			Style = UILogic.ProfessionSkillItem.Style.Small,
			IsSelected = skillData.IsEquipped,
			IsPendingLearn = skillData.IsPendingLearn,
			OnClicked = delegate()
			{
				bool isPendingLearn2 = skillData.IsPendingLearn;
				if (isPendingLearn2)
				{
					this.OnLearnProfessionSkillClicked(skillData);
				}
				else
				{
					bool isUnlocked2 = skillData.IsUnlocked;
					if (isUnlocked2)
					{
						bool isEquipped = skillData.IsEquipped;
						if (isEquipped)
						{
							this.AutoClickEquipSkillSlotBySkillId(skillData.SkillId);
						}
						else
						{
							this.SetLeftSelectedSkillItem(skillItem);
						}
					}
				}
			},
			OnPointerEnter = delegate()
			{
				bool canShowHover = skillData.IsPendingLearn || skillData.IsEquipped || (skillData.IsUnlocked && skillData.HasEmptyEquipSlotInSameLevel);
				bool flag = !canShowHover;
				if (!flag)
				{
					this.OnLeftSkillItemPointerEnter(skillItem, skillData.IsUnlocked && skillData.HasEmptyEquipSlotInSameLevel);
				}
			},
			OnPointerExit = delegate()
			{
				this.OnLeftSkillItemPointerExit(skillItem);
			}
		};
		skillItem.Refresh(refreshConfig);
		skillItem.SetSkillItemTipsEnable(true);
		bool isPendingLearn = skillData.IsPendingLearn;
		if (isPendingLearn)
		{
			skillItem.SetSkillIconGray(false);
		}
		else
		{
			bool isUnlocked = skillData.IsUnlocked;
			if (isUnlocked)
			{
				skillItem.SetSkillIconGray(!skillData.IsEquipped && !skillData.HasEmptyEquipSlotInSameLevel);
			}
		}
		skillItem.RefreshSkillItemTips(skillData.IsUnlocked && !skillData.IsPendingLearn, skillData.ProfessionId, skillData.ProfessionData, skillData.SkillId, skillData.SkillIndex, this._exp, this._resources);
	}

	// Token: 0x06003D77 RID: 15735 RVA: 0x001EE0B4 File Offset: 0x001EC2B4
	private void OnLearnProfessionSkillClicked(ProfessionSkillSortData skillData)
	{
		int expCost = this.Model.GetProfessionSkillLearnExpCost();
		ConfirmDialogCmd cmd = new ConfirmDialogCmd
		{
			Title = LocalStringManager.Get(LanguageKey.LK_ProfessionSkill_Learn_Title),
			ContentLower = LocalStringManager.GetFormat(LanguageKey.LK_ProfessionSkill_Learn_Content, expCost.ToString()),
			ConfirmDialogCost = new List<ConfirmDialogCost>
			{
				new ConfirmDialogCost
				{
					Type = EConfirmDialogCostType.Exp,
					ValueCost = expCost,
					ValueHave = this._exp
				}
			},
			Yes = delegate()
			{
				TaiwuDomainMethod.Call.LearnProfessionSkill(this.Element.GameDataListenerId, skillData.ProfessionId, skillData.SkillIndex);
			}
		};
		UIElement.ConfirmDialog.SetOnInitArgs(EasyPool.Get<ArgumentBox>().SetObject("Cmd", cmd));
		UIManager.Instance.MaskUI(UIElement.ConfirmDialog);
	}

	// Token: 0x06003D78 RID: 15736 RVA: 0x001EE184 File Offset: 0x001EC384
	private void OnLeftSkillItemPointerExit(UILogic.ProfessionSkillItem skillItem)
	{
		skillItem.HideHover();
		bool flag = this._equipSelectedItem != null;
		if (flag)
		{
			this._equipSelectedItem.SetSelected(false);
		}
		this._equipSelectedItem = null;
		this._equipSelectedSkillId = -1;
		this._equipSelectedSlot = ProfessionModel.SlotIndex.Invalid;
	}

	// Token: 0x06003D79 RID: 15737 RVA: 0x001EE1D0 File Offset: 0x001EC3D0
	private void OnLeftSkillItemPointerEnter(UILogic.ProfessionSkillItem skillItem, bool allowSelect)
	{
		skillItem.ShowHover();
		bool flag = !allowSelect;
		if (!flag)
		{
			ProfessionModel.SlotIndex slotIndex = this.Model.FindEmptyEquipSlotInSameLevelForEquip(skillItem.SkillId);
			bool isValid = slotIndex.IsValid;
			if (isValid)
			{
				this.SetEquipSelectedSkillItem(this._equipSkillItems[slotIndex.Level][slotIndex.Index], slotIndex);
			}
		}
	}

	// Token: 0x06003D7A RID: 15738 RVA: 0x001EE234 File Offset: 0x001EC434
	private void SetLeftSelectedSkillItem(UILogic.ProfessionSkillItem skillItem)
	{
		ProfessionModel.SlotIndex slotIndex = (this._equipSelectedItem != null) ? this._equipSelectedSlot : this.Model.FindEmptyEquipSlotInSameLevelForEquip(skillItem.SkillId);
		bool flag = !slotIndex.IsValid;
		if (!flag)
		{
			Config.ProfessionSkillItem skillConfig = ProfessionSkill.Instance[skillItem.SkillId];
			bool flag2 = skillConfig.TriggerType == EProfessionSkillTriggerType.Passive;
			if (flag2)
			{
				this.ConfirmAndModifyOneSlot(slotIndex.Level, slotIndex.Index, skillItem.SkillId);
			}
			else
			{
				this.ModifyOneEquipSlot(slotIndex.Level, slotIndex.Index, skillItem.SkillId);
			}
		}
	}

	// Token: 0x06003D7B RID: 15739 RVA: 0x001EE2CC File Offset: 0x001EC4CC
	private void SetEquipSelectedSkillItem(UILogic.ProfessionSkillItem skillItem, ProfessionModel.SlotIndex slotIndex)
	{
		bool flag = this._equipSelectedItem != null && this._equipSelectedItem.SkillId == this._equipSelectedSkillId;
		if (flag)
		{
			this._equipSelectedItem.SetSelected(false);
		}
		skillItem.SetSelected(true);
		this._equipSelectedItem = skillItem;
		this._equipSelectedSkillId = skillItem.SkillId;
		this._equipSelectedSlot = slotIndex;
	}

	// Token: 0x06003D7C RID: 15740 RVA: 0x001EE334 File Offset: 0x001EC534
	private void ModifyOneEquipSlot(int level, int index, int skillId)
	{
		this.Model.ModifyOneEquipSlot(level, index, skillId);
		this._pendingEquipEffectList.Add(new ProfessionModel.SlotIndex(level, index));
		bool flag = this._equipSelectedItem != null;
		if (flag)
		{
			this._equipSelectedItem.SetSelected(false);
			this._equipSelectedItem = null;
			this._equipSelectedSkillId = -1;
			this._equipSelectedSlot = ProfessionModel.SlotIndex.Invalid;
		}
		AudioManager.Instance.PlaySound("SFX_ProfessionSkill_take", false, false);
	}

	// Token: 0x06003D7D RID: 15741 RVA: 0x001EE3AE File Offset: 0x001EC5AE
	private void OnProfessionSkillSortChanged()
	{
		this.SortProfessionIds();
		this.RefreshScroll();
	}

	// Token: 0x06003D7E RID: 15742 RVA: 0x001EE3BF File Offset: 0x001EC5BF
	private void OnProfessionListSortChanged()
	{
		this.SortProfessionIds();
		this.RefreshScroll();
	}

	// Token: 0x06003D7F RID: 15743 RVA: 0x001EE3D0 File Offset: 0x001EC5D0
	private int CompareProfession(int left, int right)
	{
		ProfessionSkillSortData leftData = this._professionSortDataList.Find((ProfessionSkillSortData data) => data.ProfessionId == left);
		ProfessionSkillSortData rightData = this._professionSortDataList.Find((ProfessionSkillSortData data) => data.ProfessionId == right);
		bool flag = this._professionSkillSortAndFilterController != null;
		if (flag)
		{
			Comparison<ProfessionSkillSortData> comparer = this._professionSkillSortAndFilterController.GenerateComparer(this._professionSortDataList);
			bool flag2 = comparer != null;
			if (flag2)
			{
				int compare = comparer(leftData, rightData);
				bool flag3 = compare != 0;
				if (flag3)
				{
					return compare;
				}
			}
		}
		return left - right;
	}

	// Token: 0x06003D80 RID: 15744 RVA: 0x001EE480 File Offset: 0x001EC680
	private void SortProfessionIds()
	{
		this._professionIds.Sort(new Comparison<int>(this.CompareProfession));
	}

	// Token: 0x06003D81 RID: 15745 RVA: 0x001EE49B File Offset: 0x001EC69B
	public override void InitMonitorFieldIds()
	{
		this.MonitorFields.Add(new UIBase.MonitorDataField(4, 0, (ulong)this.TaiwuCharId, new uint[]
		{
			66U,
			34U,
			17U
		}));
	}

	// Token: 0x06003D82 RID: 15746 RVA: 0x001EE4CC File Offset: 0x001EC6CC
	protected override void OnClick(Transform btn)
	{
		string btnName = btn.name;
		bool flag = "Close" == btnName;
		if (flag)
		{
			this.QuickHide();
		}
	}

	// Token: 0x06003D83 RID: 15747 RVA: 0x001EE4FC File Offset: 0x001EC6FC
	private void ConfirmAndModifyOneSlot(int level, int index, int skillId)
	{
		DialogCmd cmd = new DialogCmd
		{
			Title = LocalStringManager.Get(LanguageKey.LK_ProfessionSkill_Confirm),
			Content = LocalStringManager.Get(LanguageKey.LK_ProfessionSkill_Confirm_Dialog_Content),
			Yes = delegate()
			{
				this.ModifyOneEquipSlot(level, index, skillId);
			}
		};
		UIElement.Dialog.SetOnInitArgs(EasyPool.Get<ArgumentBox>().SetObject("Cmd", cmd));
		UIManager.Instance.MaskUI(UIElement.Dialog);
	}

	// Token: 0x06003D84 RID: 15748 RVA: 0x001EE590 File Offset: 0x001EC790
	public override void QuickHide()
	{
		base.QuickHide();
		bool inGuiding = SingletonObject.getInstance<TutorialChapterModel>().InGuiding;
		if (inGuiding)
		{
			TaiwuEventDomainMethod.Call.TriggerListener(EventActionKey.DefValue.TutorialExitProfession, false);
		}
	}

	// Token: 0x06003D85 RID: 15749 RVA: 0x001EE5C4 File Offset: 0x001EC7C4
	public unsafe override void OnNotifyGameData(List<NotificationWrapper> notifications)
	{
		foreach (NotificationWrapper wrapper in notifications)
		{
			Notification notification = wrapper.Notification;
			byte type = notification.Type;
			byte b = type;
			if (b == 0)
			{
				DataUid uid = notification.Uid;
				RawDataPool dataPool = wrapper.DataPool;
				int offset = notification.ValueOffset;
				bool flag = uid.DomainId == 4 && uid.DataId == 0 && (int)uid.SubId0 == this.TaiwuCharId;
				if (flag)
				{
					bool flag2 = uid.SubId1 == 34U;
					if (flag2)
					{
						ResourceInts oldResources = this._resources;
						Serializer.Deserialize(wrapper.DataPool, notification.ValueOffset, ref this._resources);
						bool changed = false;
						for (int i = 0; i < 8; i++)
						{
							bool flag3 = *oldResources[i] != *this._resources[i];
							if (flag3)
							{
								changed = true;
								break;
							}
						}
						bool flag4 = changed;
						if (flag4)
						{
							this.RefreshEquipSkillSlots();
							this.RefreshProfessionSkillDataList();
							this.RefreshProfessionSkillScroll();
							this.RefreshScroll();
						}
					}
					else
					{
						bool flag5 = uid.SubId1 == 66U;
						if (flag5)
						{
							int oldExp = this._exp;
							Serializer.Deserialize(wrapper.DataPool, notification.ValueOffset, ref this._exp);
							bool flag6 = oldExp != this._exp;
							if (flag6)
							{
								this.RefreshEquipSkillSlots();
								this.RefreshProfessionSkillDataList();
								this.RefreshProfessionSkillScroll();
								this.RefreshScroll();
							}
						}
						else
						{
							bool flag7 = uid.SubId1 == 17U;
							if (flag7)
							{
								this.RefreshExtraSkill();
							}
						}
					}
				}
			}
		}
	}

	// Token: 0x06003D86 RID: 15750 RVA: 0x001EE7B0 File Offset: 0x001EC9B0
	private void RefreshAvatar()
	{
		List<int> professionIds = this.Model.GetProfessionIdsFromSlots();
		foreach (int newId in professionIds)
		{
			bool flag = !this._lastProfessionList.Contains(newId);
			if (flag)
			{
				AudioManager.Instance.PlaySound(string.Format("SFX_Profession_woodenman_{0}", newId + 1), false, false);
			}
		}
		this._lastProfessionList.Clear();
		this._lastProfessionList.AddRange(professionIds);
		this.uIProfessionAvatar.ExplainSubElements(professionIds);
		EasyPool.Free<List<int>>(professionIds);
	}

	// Token: 0x06003D87 RID: 15751 RVA: 0x001EE86C File Offset: 0x001ECA6C
	private void OnEquipSlotsChanged(bool init = false)
	{
		this.RefreshEquipSkillSlots();
		this.RefreshProfessionSkillDataList();
		this.RefreshAvatar();
		this.RefreshBgParticle(init);
		this.RefreshSlotParticle();
		this.professionScroll.ReRender();
		this.professionSkillScroll.ReRender();
	}

	// Token: 0x06003D88 RID: 15752 RVA: 0x001EE8AC File Offset: 0x001ECAAC
	private void RefreshSlotParticle()
	{
		foreach (ProfessionModel.SlotIndex slotIndex in this._pendingEquipEffectList)
		{
			UILogic.ProfessionSkillItem slot = this._equipSkillItems[slotIndex.Level][slotIndex.Index];
		}
		this._pendingEquipEffectList.Clear();
	}

	// Token: 0x06003D89 RID: 15753 RVA: 0x001EE928 File Offset: 0x001ECB28
	private void RefreshBgParticle(bool init)
	{
		int fullCount = this.Model.CountFullSlotLevels();
		bool flag = this._lastFullCount != fullCount;
		if (flag)
		{
			bool flag2 = !init;
			if (flag2)
			{
				bool flag3 = this._lastFullCount != 0;
				if (flag3)
				{
					this.PlayBgParticleSound(this._lastFullCount - 1, ViewProfession.BgParticleAction.Out);
				}
				bool flag4 = fullCount != 0;
				if (flag4)
				{
					this.PlayBgParticleSound(fullCount - 1, ViewProfession.BgParticleAction.In);
				}
			}
			bool flag5 = this._lastFullCount != 4 && fullCount == 4;
			if (flag5)
			{
				this.SwitchLoopSound("SFX_ProfessionSkill_fourth_loop", true);
			}
			bool flag6 = fullCount != 4;
			if (flag6)
			{
				this.SwitchLoopSound("SFX_ProfessionSkill_fourth_loop", false);
			}
		}
		for (int i = 0; i < this._bgParticles.Count; i++)
		{
			if (init)
			{
				this._bgParticles[i].SetActiveNoTween(i == fullCount - 1);
			}
			else
			{
				this._bgParticles[i].SetActiveWithTween(i == fullCount - 1, 0.9f, Ease.InOutQuad);
			}
		}
		this._lastFullCount = fullCount;
	}

	// Token: 0x06003D8A RID: 15754 RVA: 0x001EEA40 File Offset: 0x001ECC40
	private void CheckBgParticleLoopSound(int fullCount)
	{
		bool flag = fullCount == 4;
		if (flag)
		{
			this.SwitchLoopSound("SFX_ProfessionSkill_fourth_loop", true);
		}
		else
		{
			this.SwitchLoopSound("SFX_ProfessionSkill_fourth_loop", false);
		}
	}

	// Token: 0x06003D8B RID: 15755 RVA: 0x001EEA76 File Offset: 0x001ECC76
	private void PlayBgParticleSound(int level, ViewProfession.BgParticleAction action)
	{
		AudioManager.Instance.PlaySound(string.Format("SFX_ProfessionSkill_{0}_{1}", ViewProfession.BgParticleLevelName[level], ViewProfession.BgParticleActionName[(int)action]), false, false);
	}

	// Token: 0x06003D8C RID: 15756 RVA: 0x001EEAA0 File Offset: 0x001ECCA0
	private void SwitchLoopSound(string name, bool isOn)
	{
		if (isOn)
		{
			bool flag = !this._loopingSoundSet.Contains(name);
			if (flag)
			{
				AudioManager.Instance.PlaySound(name, true, false);
				this._loopingSoundSet.Add(name);
			}
		}
		else
		{
			bool flag2 = this._loopingSoundSet.Contains(name);
			if (flag2)
			{
				AudioManager.Instance.StopSound(name);
				this._loopingSoundSet.Remove(name);
			}
		}
	}

	// Token: 0x06003D8D RID: 15757 RVA: 0x001EEB14 File Offset: 0x001ECD14
	private void RefreshEquipSkillSlots()
	{
		for (int i = 0; i < this._equipSkillItems.Count; i++)
		{
			int index = i;
			List<UILogic.ProfessionSkillItem> subList = this._equipSkillItems[i];
			for (int j = 0; j < subList.Count; j++)
			{
				UILogic.ProfessionSkillItem skillItem = subList[j];
				int skillId = this.Model.GetProfessionSkillFromSlot(i, j);
				ValueTuple<ProfessionData, int> valueTuple = this.Model.FindProfessionDataBySkillId(skillId);
				ProfessionData professionData = valueTuple.Item1;
				int skillIndex = valueTuple.Item2;
				UILogic.ProfessionSkillItem.RefreshConfig refreshConfig = new UILogic.ProfessionSkillItem.RefreshConfig
				{
					ProfessionData = professionData,
					SkillIndexOfProfession = skillIndex,
					Style = UILogic.ProfessionSkillItem.Style.Large,
					IsSelected = (skillItem == this._equipSelectedItem),
					IsEmpty = (skillId == -1),
					OnClicked = delegate()
					{
						this.OnEquipSlotClick(index, professionData, skillItem, skillIndex, true);
					},
					OnPointerEnter = delegate()
					{
						this.OnEquipSlotPointerEnter(professionData, skillItem, skillIndex);
					},
					OnPointerExit = delegate()
					{
						this.OnEquipSlotPointerExit(skillItem);
					}
				};
				ProfessionData professionData2 = professionData;
				bool unlocked = professionData2 != null && professionData2.IsSkillUnlocked(skillIndex);
				skillItem.Refresh(refreshConfig);
				bool tipEnabled = skillId != -1 && professionData != null;
				skillItem.SetSkillItemTipsEnable(tipEnabled);
				bool flag = tipEnabled;
				if (flag)
				{
					skillItem.RefreshSkillItemTips(unlocked, professionData.TemplateId, professionData, skillId, skillIndex, this._exp, this._resources);
				}
			}
		}
	}

	// Token: 0x06003D8E RID: 15758 RVA: 0x001EECFC File Offset: 0x001ECEFC
	private void AutoClickEquipSkillSlotBySkillId(int skillId)
	{
		ValueTuple<ProfessionData, int> valueTuple = this.Model.FindProfessionDataBySkillId(skillId);
		ProfessionData professionData = valueTuple.Item1;
		int skillIndex = valueTuple.Item2;
		for (int i = 0; i < this._equipSkillItems.Count; i++)
		{
			List<UILogic.ProfessionSkillItem> subList = this._equipSkillItems[i];
			for (int j = 0; j < subList.Count; j++)
			{
				UILogic.ProfessionSkillItem skillItem = subList[j];
				bool flag = skillItem.SkillId == skillId;
				if (flag)
				{
					this.OnEquipSlotClick(i, professionData, skillItem, skillIndex, false);
					return;
				}
			}
		}
	}

	// Token: 0x06003D8F RID: 15759 RVA: 0x001EED95 File Offset: 0x001ECF95
	private void OnEquipSlotPointerExit(UILogic.ProfessionSkillItem skillItem)
	{
		skillItem.HideHover();
	}

	// Token: 0x06003D90 RID: 15760 RVA: 0x001EEDA0 File Offset: 0x001ECFA0
	private void OnEquipSlotPointerEnter(ProfessionData professionData, UILogic.ProfessionSkillItem skillItem, int skillIndex)
	{
		int skillId = skillItem.SkillId;
		bool flag = skillId == -1;
		if (flag)
		{
			skillItem.ShowHover();
		}
		else
		{
			bool flag2 = professionData.IsSkillCooldown(this.CurrentDate, skillIndex);
			if (!flag2)
			{
				skillItem.ShowHover();
			}
		}
	}

	// Token: 0x06003D91 RID: 15761 RVA: 0x001EEDE4 File Offset: 0x001ECFE4
	private void OnEquipSlotClick(int level, ProfessionData professionData, UILogic.ProfessionSkillItem skillItem, int skillIndex, bool hideTips)
	{
		int skillId = skillItem.SkillId;
		bool flag = skillId == -1;
		if (flag)
		{
			this.SetSkillLevelFilter(level);
		}
		else
		{
			bool flag2 = professionData.IsSkillCooldown(this.CurrentDate, skillIndex);
			if (!flag2)
			{
				this.RemoveEquippedSkill(skillId);
				if (hideTips)
				{
					skillItem.HideTips();
				}
				AudioManager.Instance.PlaySound("SFX_ProfessionSkill_put", false, false);
			}
		}
	}

	// Token: 0x06003D92 RID: 15762 RVA: 0x001EEE47 File Offset: 0x001ED047
	private void RemoveEquippedSkill(int skillId)
	{
		this.Model.RemoveSkillIdFromSlot(skillId);
	}

	// Token: 0x06003D93 RID: 15763 RVA: 0x001EEE57 File Offset: 0x001ED057
	private void RefreshScroll()
	{
		this.professionScroll.SetDataCount(this._professionIds.Count);
	}

	// Token: 0x06003D94 RID: 15764 RVA: 0x001EEE74 File Offset: 0x001ED074
	private void RefreshProfessionSkillScroll()
	{
		this._filteredProfessionSkillDataList.Clear();
		bool flag = this._selectedSkillLevelFilter < 0;
		if (flag)
		{
			this.professionSkillScroll.SetDataCount(0);
		}
		else
		{
			foreach (ProfessionSkillSortData skillData in this._professionSkillDataList)
			{
				bool flag2 = skillData.SkillIndex != this._selectedSkillLevelFilter;
				if (!flag2)
				{
					this._filteredProfessionSkillDataList.Add(skillData);
				}
			}
			this.professionSkillScroll.SetDataCount(this._filteredProfessionSkillDataList.Count);
		}
	}

	// Token: 0x06003D95 RID: 15765 RVA: 0x001EEF2C File Offset: 0x001ED12C
	private void RefreshProfessionSkillDataList()
	{
		this._professionSortDataList.Clear();
		this._professionSkillDataList.Clear();
		foreach (int professionId in this._professionIds)
		{
			this._professionSortDataList.Add(this.BuildProfessionSortData(professionId));
			ProfessionItem professionConfig = Profession.Instance[professionId];
			int skillCount = (professionConfig.ExtraProfessionSkill != -1) ? 4 : 3;
			for (int i = 0; i < skillCount; i++)
			{
				this._professionSkillDataList.Add(this.BuildProfessionSkillSortData(professionId, i));
			}
		}
	}

	// Token: 0x06003D96 RID: 15766 RVA: 0x001EEFF0 File Offset: 0x001ED1F0
	private ProfessionSkillSortData BuildProfessionSortData(int professionId)
	{
		ProfessionData professionData = this.Model.GetProfessionData(professionId);
		return new ProfessionSkillSortData
		{
			ProfessionId = professionId,
			ProfessionData = professionData,
			Seniority = professionData.Seniority,
			SkillName = Profession.Instance[professionId].Name
		};
	}

	// Token: 0x06003D97 RID: 15767 RVA: 0x001EF044 File Offset: 0x001ED244
	private ProfessionSkillSortData BuildProfessionSkillSortData(int professionId, int skillIndex)
	{
		ProfessionData professionData = this.Model.GetProfessionData(professionId);
		ProfessionItem professionConfig = Profession.Instance[professionId];
		int skillId = (skillIndex == 3) ? professionConfig.ExtraProfessionSkill : professionConfig.ProfessionSkills[skillIndex];
		bool unlocked = professionData.IsSkillUnlocked(skillIndex);
		bool equipped = this.Model.IsSkillEquipped(skillId);
		bool isPendingLearn = this.Model.IsProfessionalSkillPendingLearn(professionId, skillIndex);
		return new ProfessionSkillSortData
		{
			ProfessionId = professionId,
			ProfessionData = professionData,
			SkillIndex = skillIndex,
			SkillId = skillId,
			SkillName = ProfessionSkill.Instance[skillId].Name,
			Seniority = professionData.Seniority,
			IsUnlocked = unlocked,
			IsEquipped = equipped,
			HasEmptyEquipSlotInSameLevel = this.Model.HasEmptyEquipSlotInSameLevel(skillId),
			IsPendingLearn = isPendingLearn
		};
	}

	// Token: 0x06003D98 RID: 15768 RVA: 0x001EF118 File Offset: 0x001ED318
	private void OnSkillLevelFilterChanged(int newIndex, int oldIndex)
	{
		this._selectedSkillLevelFilter = ((newIndex <= 0) ? -1 : (newIndex - 1));
		this.SetSkillListMode((this._selectedSkillLevelFilter < 0) ? ViewProfession.SkillListMode.ProfessionList : ViewProfession.SkillListMode.SkillList);
		bool flag = this._selectedSkillLevelFilter < 0;
		if (flag)
		{
			this.SortProfessionIds();
		}
		this.RefreshProfessionSkillScroll();
		this.RefreshScroll();
	}

	// Token: 0x06003D99 RID: 15769 RVA: 0x001EF16C File Offset: 0x001ED36C
	private void SetSkillLevelFilter(int level)
	{
		bool flag = this.skillLevelToggleGroup == null;
		if (!flag)
		{
			this.skillLevelToggleGroup.Set(level + 1, false);
		}
	}

	// Token: 0x06003D9A RID: 15770 RVA: 0x001EF19C File Offset: 0x001ED39C
	private void SetSkillListMode(ViewProfession.SkillListMode skillListMode)
	{
		this._skillListMode = skillListMode;
		this.professionScroll.gameObject.SetActive(this._skillListMode == ViewProfession.SkillListMode.ProfessionList);
		this.professionSkillScroll.gameObject.SetActive(this._skillListMode == ViewProfession.SkillListMode.SkillList);
		this.sortAndFilter.gameObject.SetActive(this._skillListMode == ViewProfession.SkillListMode.ProfessionList);
	}

	// Token: 0x06003D9B RID: 15771 RVA: 0x001EF200 File Offset: 0x001ED400
	private void RefreshAllAssumingDataExists()
	{
		this._professionIds.Clear();
		this._professionIds.AddRange(this.Model.TaiwuProfessions.Keys);
		this.RefreshProfessionSkillDataList();
		this.SortProfessionIds();
		this.RefreshScroll();
		this.RefreshProfessionSkillScroll();
		this.OnEquipSlotsChanged(true);
		this.CheckBgParticleLoopSound(this.Model.CountFullSlotLevels());
		this.RefreshExtraSkill();
		this.Element.ShowAfterRefresh();
		bool hasUnlockedSkill = false;
		foreach (int professionId in this._professionIds)
		{
			ProfessionData professionData = this.Model.GetProfessionData(professionId);
			bool flag = professionData == null;
			if (!flag)
			{
				ProfessionItem professionConfig = Profession.Instance[professionId];
				int skillCount = (professionConfig.ExtraProfessionSkill != -1) ? 4 : 3;
				for (int i = 0; i < skillCount; i++)
				{
					bool flag2 = professionData.IsSkillUnlocked(i);
					if (flag2)
					{
						hasUnlockedSkill = true;
						break;
					}
				}
				bool flag3 = hasUnlockedSkill;
				if (flag3)
				{
					break;
				}
			}
		}
		bool flag4 = hasUnlockedSkill;
		if (flag4)
		{
			GlobalDomainMethod.Call.InvokeGuidingTrigger(253);
		}
	}

	// Token: 0x06003D9C RID: 15772 RVA: 0x001EF344 File Offset: 0x001ED544
	private void OnProfessionDataChange(ArgumentBox argbox)
	{
		this._professionIds.Clear();
		this._professionIds.AddRange(this.Model.TaiwuProfessions.Keys);
		this.RefreshProfessionSkillDataList();
		this.SortProfessionIds();
		this.RefreshScroll();
		this.RefreshProfessionSkillScroll();
		this.OnEquipSlotsChanged(false);
	}

	// Token: 0x06003D9D RID: 15773 RVA: 0x001EF39E File Offset: 0x001ED59E
	private void OnProfessionSlotsChange(ArgumentBox argbox)
	{
		this.RefreshProfessionSkillDataList();
		this.RefreshProfessionSkillScroll();
		this.OnEquipSlotsChanged(false);
	}

	// Token: 0x06003D9E RID: 15774 RVA: 0x001EF3B8 File Offset: 0x001ED5B8
	private void OnTopUiChanged(ArgumentBox argumentBox)
	{
		bool flag = UIManager.Instance.IsFocusElement(UIElement.EventWindow);
		if (flag)
		{
			this.QuickHide();
		}
	}

	// Token: 0x06003D9F RID: 15775 RVA: 0x001EF3E4 File Offset: 0x001ED5E4
	private void OnEnable()
	{
		GEvent.Add(UiEvents.OnProfessionDataChange, new GEvent.Callback(this.OnProfessionDataChange));
		GEvent.Add(UiEvents.OnProfessionSlotsChange, new GEvent.Callback(this.OnProfessionSlotsChange));
		GEvent.Add(UiEvents.TopUiChanged, new GEvent.Callback(this.OnTopUiChanged));
	}

	// Token: 0x06003DA0 RID: 15776 RVA: 0x001EF444 File Offset: 0x001ED644
	private void OnDisable()
	{
		GEvent.Remove(UiEvents.OnProfessionDataChange, new GEvent.Callback(this.OnProfessionDataChange));
		GEvent.Remove(UiEvents.OnProfessionSlotsChange, new GEvent.Callback(this.OnProfessionSlotsChange));
		GEvent.Remove(UiEvents.TopUiChanged, new GEvent.Callback(this.OnTopUiChanged));
		this._equipSelectedItem = null;
		this._equipSelectedSkillId = -1;
		this._equipSelectedSlot = ProfessionModel.SlotIndex.Invalid;
		this.SwitchLoopSound("SFX_ProfessionSkill_fourth_loop", false);
	}

	// Token: 0x04002C35 RID: 11317
	[SerializeField]
	private CButton buttonCancel;

	// Token: 0x04002C36 RID: 11318
	[SerializeField]
	private ProfessionAvatarController uIProfessionAvatar;

	// Token: 0x04002C37 RID: 11319
	[SerializeField]
	private RectTransform bgParticle;

	// Token: 0x04002C38 RID: 11320
	[SerializeField]
	private InfinityScroll professionScroll;

	// Token: 0x04002C39 RID: 11321
	[SerializeField]
	private SortAndFilter sortAndFilter;

	// Token: 0x04002C3A RID: 11322
	[SerializeField]
	private CToggleGroup skillLevelToggleGroup;

	// Token: 0x04002C3B RID: 11323
	[SerializeField]
	private InfinityScroll professionSkillScroll;

	// Token: 0x04002C3C RID: 11324
	[SerializeField]
	private RectTransform equipSkillSlots;

	// Token: 0x04002C3D RID: 11325
	[SerializeField]
	private BottomExtraSkill extraSkill;

	// Token: 0x04002C3E RID: 11326
	private readonly List<int> _professionIds = new List<int>();

	// Token: 0x04002C3F RID: 11327
	private readonly List<ProfessionSkillSortData> _professionSortDataList = new List<ProfessionSkillSortData>();

	// Token: 0x04002C40 RID: 11328
	private readonly List<ProfessionSkillSortData> _professionSkillDataList = new List<ProfessionSkillSortData>();

	// Token: 0x04002C41 RID: 11329
	private readonly List<ProfessionSkillSortData> _filteredProfessionSkillDataList = new List<ProfessionSkillSortData>();

	// Token: 0x04002C42 RID: 11330
	private bool _inited = false;

	// Token: 0x04002C43 RID: 11331
	private UILogic.ProfessionSkillItem _equipSelectedItem;

	// Token: 0x04002C44 RID: 11332
	private int _equipSelectedSkillId = -1;

	// Token: 0x04002C45 RID: 11333
	private ProfessionModel.SlotIndex _equipSelectedSlot = ProfessionModel.SlotIndex.Invalid;

	// Token: 0x04002C46 RID: 11334
	private List<List<UILogic.ProfessionSkillItem>> _equipSkillItems;

	// Token: 0x04002C47 RID: 11335
	private List<ParticleSystemAlphaController> _bgParticles;

	// Token: 0x04002C48 RID: 11336
	private ResourceInts _resources;

	// Token: 0x04002C49 RID: 11337
	private int _exp;

	// Token: 0x04002C4A RID: 11338
	private ProfessionSkillSortAndFilterController _professionSkillSortAndFilterController;

	// Token: 0x04002C4B RID: 11339
	private ViewProfession.SkillListMode _skillListMode = ViewProfession.SkillListMode.ProfessionList;

	// Token: 0x04002C4C RID: 11340
	private int _selectedSkillLevelFilter = -1;

	// Token: 0x04002C4D RID: 11341
	private UIParticlePlayHelper _particlePlayHelper = new UIParticlePlayHelper();

	// Token: 0x04002C4E RID: 11342
	private readonly List<ProfessionModel.SlotIndex> _pendingEquipEffectList = new List<ProfessionModel.SlotIndex>();

	// Token: 0x04002C4F RID: 11343
	private const string BgParticleSoundName = "SFX_ProfessionSkill_{0}_{1}";

	// Token: 0x04002C50 RID: 11344
	private static readonly string[] BgParticleLevelName = new string[]
	{
		"first",
		"second",
		"third",
		"fourth"
	};

	// Token: 0x04002C51 RID: 11345
	private static readonly string[] BgParticleActionName = new string[]
	{
		"shine",
		"out"
	};

	// Token: 0x04002C52 RID: 11346
	private int _lastFullCount = 0;

	// Token: 0x04002C53 RID: 11347
	private readonly List<int> _lastProfessionList = new List<int>();

	// Token: 0x04002C54 RID: 11348
	private readonly HashSet<string> _loopingSoundSet = new HashSet<string>();

	// Token: 0x02001890 RID: 6288
	private enum BgParticleAction
	{
		// Token: 0x0400AF06 RID: 44806
		In,
		// Token: 0x0400AF07 RID: 44807
		Out
	}

	// Token: 0x02001891 RID: 6289
	private enum SkillListMode
	{
		// Token: 0x0400AF09 RID: 44809
		ProfessionList,
		// Token: 0x0400AF0A RID: 44810
		SkillList
	}

	// Token: 0x02001892 RID: 6290
	private static class ProgressTipsHelper
	{
		// Token: 0x0600D70F RID: 55055 RVA: 0x005BFA40 File Offset: 0x005BDC40
		public static void RefreshTips(TooltipInvoker tipDisplayer, ProfessionData professionData, ViewProfession.ProgressTipsHelper.TipsMode tipsMode, int segmentIndex = -1)
		{
			tipDisplayer.Type = TipType.GeneralLines;
			if (tipDisplayer.RuntimeParam == null)
			{
				tipDisplayer.RuntimeParam = new ArgumentBox();
			}
			ArgumentBox tipParam = tipDisplayer.RuntimeParam;
			tipDisplayer.Type = TipType.GeneralLines;
			tipParam.Set("Title", LocalStringManager.Get(LanguageKey.LK_SeniorityTips_Title));
			int lineCount = 0;
			bool isExtra = professionData.ExtraSeniority > 0;
			string professionName = Profession.Instance[professionData.TemplateId].Name;
			ViewProfession.ProgressTipsHelper.AddNode(tipParam, ViewProfession.ProgressTipsHelper.MakeCurrentSeniorityNode(professionData), ref lineCount);
			bool flag = tipsMode == ViewProfession.ProgressTipsHelper.TipsMode.Segment;
			if (flag)
			{
				ViewProfession.ProgressTipsHelper.AddNode(tipParam, ViewProfession.ProgressTipsHelper.MakeSegmentNode(professionData, segmentIndex), ref lineCount);
				ViewProfession.ProgressTipsHelper.AddNode(tipParam, ViewProfession.ProgressTipsHelper.MakeSpace(16f), ref lineCount);
				ViewProfession.ProgressTipsHelper.AddNode(tipParam, ViewProfession.ProgressTipsHelper.MakeSegmentTitleNode(), ref lineCount);
				bool flag2 = isExtra;
				if (flag2)
				{
					ViewProfession.ProgressTipsHelper.AddNode(tipParam, ViewProfession.ProgressTipsHelper.MakeSegmentIndentLine(LanguageKey.LK_SeniorityTips_Effect_2, new object[]
					{
						professionName,
						"1"
					}), ref lineCount);
				}
				else
				{
					bool flag3 = segmentIndex != -1;
					if (flag3)
					{
						ViewProfession.ProgressTipsHelper.AddNode(tipParam, ViewProfession.ProgressTipsHelper.MakeSegmentNameNode(professionData, segmentIndex), ref lineCount);
					}
					bool flag4 = segmentIndex == 0;
					if (flag4)
					{
						ViewProfession.ProgressTipsHelper.AddNode(tipParam, ViewProfession.ProgressTipsHelper.MakeSegmentIndentLine(LanguageKey.LK_SeniorityTips_Effect_1, new object[]
						{
							professionName
						}), ref lineCount);
					}
					ViewProfession.ProgressTipsHelper.AddNode(tipParam, ViewProfession.ProgressTipsHelper.MakeSegmentIndentLine(LanguageKey.LK_SeniorityTips_Effect_2, new object[]
					{
						professionName,
						"3"
					}), ref lineCount);
				}
			}
			tipParam.Set("LineCount", lineCount);
		}

		// Token: 0x0600D710 RID: 55056 RVA: 0x005BFBB4 File Offset: 0x005BDDB4
		private static void AddNode(ArgumentBox tipParam, GeneralLineData lineData, ref int lineCount)
		{
			string format = "LineData{0}";
			int num = lineCount + 1;
			lineCount = num;
			tipParam.SetObject(string.Format(format, num), lineData);
		}

		// Token: 0x0600D711 RID: 55057 RVA: 0x005BFBE4 File Offset: 0x005BDDE4
		private static GeneralLineData MakeCurrentSeniorityNode(ProfessionData professionData)
		{
			int current = (professionData.ExtraSeniority > 0) ? professionData.ExtraSeniority : professionData.Seniority;
			int max = (professionData.ExtraSeniority > 0) ? 1500000 : 3000000;
			string s3 = string.Format("{0}/{1}", current, max);
			string text = LocalStringManager.GetFormat(LanguageKey.LK_SeniorityTips_Current, s3).ColorReplace();
			return new GeneralLineData
			{
				Type = 5,
				Args = new List<string>
				{
					text
				}
			};
		}

		// Token: 0x0600D712 RID: 55058 RVA: 0x005BFC70 File Offset: 0x005BDE70
		private static GeneralLineData MakeSegmentNode(ProfessionData professionData, int segmentIndex)
		{
			int skillIndex = (segmentIndex == 3) ? Profession.Instance[professionData.TemplateId].ExtraProfessionSkill : Profession.Instance[professionData.TemplateId].ProfessionSkills[segmentIndex];
			Config.ProfessionSkillItem skillConfig = ProfessionSkill.Instance[skillIndex];
			short percent = skillConfig.UnlockSeniority;
			string text = LocalStringManager.GetFormat(LanguageKey.LK_SeniorityTips_Segment, string.Format("{0}%", percent)).ColorReplace();
			return new GeneralLineData
			{
				Type = 5,
				Args = new List<string>
				{
					text
				}
			};
		}

		// Token: 0x0600D713 RID: 55059 RVA: 0x005BFD0C File Offset: 0x005BDF0C
		private static GeneralLineData MakeSegmentNameNode(ProfessionData professionData, int segmentIndex)
		{
			int skillIndex = (segmentIndex == 3) ? Profession.Instance[professionData.TemplateId].ExtraProfessionSkill : Profession.Instance[professionData.TemplateId].ProfessionSkills[segmentIndex];
			Config.ProfessionSkillItem skillConfig = ProfessionSkill.Instance[skillIndex];
			return ViewProfession.ProgressTipsHelper.MakeSegmentIndentLine(LanguageKey.LK_SeniorityTips_Effect_0, new object[]
			{
				skillConfig.Name
			});
		}

		// Token: 0x0600D714 RID: 55060 RVA: 0x005BFD78 File Offset: 0x005BDF78
		private static GeneralLineData MakeSegmentTitleNode()
		{
			return new GeneralLineData
			{
				Type = 1,
				Args = new List<string>
				{
					LocalStringManager.Get(LanguageKey.LK_SeniorityTips_EffectTitle)
				}
			};
		}

		// Token: 0x0600D715 RID: 55061 RVA: 0x005BFDB4 File Offset: 0x005BDFB4
		private static GeneralLineData MakeSegmentIndentLine(LanguageKey key, params object[] args)
		{
			return new GeneralLineData
			{
				Type = 5,
				Args = new List<string>
				{
					LocalStringManager.GetFormat(key, args)
				},
				ExtraArgs = new List<object>
				{
					20
				}
			};
		}

		// Token: 0x0600D716 RID: 55062 RVA: 0x005BFE04 File Offset: 0x005BE004
		private static GeneralLineData MakeSpace(float size)
		{
			return new GeneralLineData
			{
				Type = 4,
				PreferredHeight = size
			};
		}

		// Token: 0x020026C3 RID: 9923
		public enum TipsMode
		{
			// Token: 0x0400EB72 RID: 60274
			Progress,
			// Token: 0x0400EB73 RID: 60275
			Segment
		}
	}
}
