using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Config;
using FrameWork;
using Game.Views.CharacterMenu;
using Game.Views.Combat;
using GameData.Common;
using GameData.Domains.Character;
using GameData.Domains.CombatSkill;
using GameData.Domains.Taiwu;
using GameData.GameDataBridge;
using GameData.Serializer;
using GameData.Utilities;
using TMPro;
using UnityEngine;

// Token: 0x020001E4 RID: 484
public class CombatSkillQuickEdit : UIBase
{
	// Token: 0x17000338 RID: 824
	// (get) Token: 0x06001FAE RID: 8110 RVA: 0x000E6B31 File Offset: 0x000E4D31
	public ViewCharacterMenu CharacterMenu
	{
		get
		{
			return UIElement.CharacterMenu.UiBaseAs<ViewCharacterMenu>();
		}
	}

	// Token: 0x06001FAF RID: 8111 RVA: 0x000E6B40 File Offset: 0x000E4D40
	public override void OnInit(ArgumentBox argsBox)
	{
		this.NeedDataListenerId = true;
		bool flag = argsBox != null;
		if (flag)
		{
			argsBox.Get<RectTransform>("ReferenceRect", out this._referenceRect);
			argsBox.Get<CombatSkillDisplayData>("CombatSkillDisplayData", out this._combatSkillDisplayData);
			argsBox.Get("CharId", out this._charId);
		}
		this.InitToggleGroups();
		this.InitOtherSlicesInteract();
		this.SetContentPosition();
		UIElement element = this.Element;
		element.OnListenerIdReady = (Action)Delegate.Combine(element.OnListenerIdReady, new Action(this.OnListenerIdReady));
	}

	// Token: 0x06001FB0 RID: 8112 RVA: 0x000E6BD4 File Offset: 0x000E4DD4
	private void OnListenerIdReady()
	{
		bool flag = this.IsTaiwu();
		if (flag)
		{
			this.RequestBreakPlateData();
		}
		else
		{
			this.RefreshAll();
		}
	}

	// Token: 0x06001FB1 RID: 8113 RVA: 0x000E6C00 File Offset: 0x000E4E00
	private void GetCombatSkillDisplayData()
	{
		bool flag = this._combatSkillDisplayData == null;
		if (!flag)
		{
			CombatSkillDomainMethod.Call.GetCombatSkillDisplayDataOnce(this.Element.GameDataListenerId, this._charId, this._combatSkillDisplayData.TemplateId);
		}
	}

	// Token: 0x06001FB2 RID: 8114 RVA: 0x000E6C40 File Offset: 0x000E4E40
	private void RequestBreakPlateData()
	{
		bool flag = !this.IsTaiwu();
		if (!flag)
		{
			bool flag2 = this._combatSkillDisplayData == null;
			if (!flag2)
			{
				TaiwuDomainMethod.Call.GetBreakPlateData(this.Element.GameDataListenerId, this._combatSkillDisplayData.TemplateId);
			}
		}
	}

	// Token: 0x06001FB3 RID: 8115 RVA: 0x000E6C88 File Offset: 0x000E4E88
	private void RefreshAll()
	{
		this.Element.ShowAfterRefresh();
		this.RefreshCombatSkill();
		bool isTaiwu = this.IsTaiwu();
		bool flag = isTaiwu && this._taiwuCombatSkill.Count == 0;
		if (!flag)
		{
			ushort activateState = this._combatSkillDisplayData.ActivationState;
			bool broken = CombatSkillStateHelper.IsBrokenOut(activateState);
			PracticeSkillPlatePageUtils.RefreshOtherToggles(this._combatSkillDisplayData, this.CharacterMenu.CurCharacterId, isTaiwu, this._breakPlate, this.otherPageToggleGroup);
			this.RefreshPages(activateState, broken);
			this.RefershRequireQualification();
			this.RefreshPresetToggleGroup();
			this.UpdateJumpThresholdItem();
			this.UpdateMasteredItem();
		}
	}

	// Token: 0x06001FB4 RID: 8116 RVA: 0x000E6D28 File Offset: 0x000E4F28
	private void RefershRequireQualification()
	{
		bool flag = this._qualificationRequireValue == null;
		if (!flag)
		{
			ushort activateState = this._combatSkillDisplayData.ActivationState;
			bool broken = CombatSkillStateHelper.IsBrokenOut(activateState);
			this.requireQualification.SetActive(!broken);
			bool flag2 = !broken;
			if (flag2)
			{
				this.requireQualificationValueLabel.text = this._qualificationRequireValue;
				this.requireQualificationTypeIcon.SetSprite(this._qualificationTypeIcon, false, null);
			}
		}
	}

	// Token: 0x06001FB5 RID: 8117 RVA: 0x000E6D98 File Offset: 0x000E4F98
	private void RefreshPresetToggleGroup()
	{
		CombatSkillItem configData = CombatSkill.Instance[this._combatSkillDisplayData.TemplateId];
		bool showBreakPlatePreset = this.CharacterMenu.CurrentCharacterIsTaiwu && this._legendaryBookBreakPlateCounts.ContainsKey(configData.Type) && this._legendaryBookBreakPlateCounts[configData.Type] >= 0;
		this.presetTogGroup.gameObject.SetActive(showBreakPlatePreset);
		bool flag = !showBreakPlatePreset;
		if (!flag)
		{
			sbyte count = this._legendaryBookBreakPlateCounts[configData.Type];
			bool hasThirdPreset = count == 2;
			this.presetTogGroup.Get(2).gameObject.SetActive(hasThirdPreset);
			this._autoSelectingBreakPlate = true;
			this.presetTogGroup.Set((int)this._combatSkillDisplayData.BreakPlateIndex, true, false);
			this._autoSelectingBreakPlate = false;
		}
	}

	// Token: 0x06001FB6 RID: 8118 RVA: 0x000E6E6C File Offset: 0x000E506C
	private void RefreshPages(ushort activateState, bool broken)
	{
		this._autoSelectingPage = true;
		HashSet<int> needSetToggles = EasyPool.Get<HashSet<int>>();
		needSetToggles.Clear();
		for (byte i = 1; i <= 5; i += 1)
		{
			byte internalIndex = CombatSkillStateHelper.GetNormalPageInternalIndex(CombatSkillStateHelper.GetPageActiveDirection(activateState, i), i);
			needSetToggles.Add((int)internalIndex);
		}
		if (broken)
		{
			this.outlineToggleGroup.Set((int)CombatSkillStateHelper.GetActiveOutlinePageType(activateState), true, false);
			foreach (int needSetToggle in needSetToggles)
			{
				this.otherPageToggleGroup.Set(needSetToggle, true, false);
			}
		}
		else
		{
			bool flag = this._breakPlate != null && this.CharacterMenu.CurrentCharacterIsTaiwu;
			if (flag)
			{
				this.outlineToggleGroup.Set((int)CombatSkillStateHelper.GetActiveOutlinePageType(this._breakPlate.SelectedPages), true, false);
				for (byte j = 1; j <= 5; j += 1)
				{
					this.outlineToggleGroup.Set((int)CombatSkillStateHelper.GetNormalPageInternalIndex(CombatSkillStateHelper.GetPageActiveDirection(this._breakPlate.SelectedPages, j), j), true, false);
				}
			}
			else
			{
				CToggleObsolete checkedOutline = this.outlineToggleGroup.GetActive();
				bool flag2 = checkedOutline != null;
				if (flag2)
				{
					this.outlineToggleGroup.Set(checkedOutline, false);
				}
				List<CToggleObsolete> checkedPages = this.otherPageToggleGroup.GetAll();
				foreach (CToggleObsolete pageTog in checkedPages)
				{
					bool flag3 = needSetToggles.Contains(pageTog.Key);
					if (!flag3)
					{
						this.otherPageToggleGroup.Set(pageTog, false);
					}
				}
				foreach (int needSetToggle2 in needSetToggles)
				{
					this.otherPageToggleGroup.Set(needSetToggle2, true, false);
				}
			}
		}
		EasyPool.Free<HashSet<int>>(needSetToggles);
		sbyte k = 0;
		while ((int)k < this.outlineToggleGroup.Count())
		{
			PracticeSlice outlinePageRefers = this.outlineToggleGroup.Get((int)k).GetComponent<PracticeSlice>();
			bool isPageRead = CombatSkillStateHelper.IsPageRead(this._combatSkillDisplayData.ReadingState, CombatSkillStateHelper.GetOutlinePageInternalIndex(k));
			bool canClickOutline = this.IsTaiwu() && !broken && !this._combatSkillDisplayData.Revoked && this._breakPlate == null && isPageRead;
			outlinePageRefers.GetComponent<CToggleObsolete>().interactable = canClickOutline;
			bool showPage = !this._combatSkillDisplayData.Revoked && isPageRead;
			outlinePageRefers.SetNameBgActive(showPage);
			outlinePageRefers.SetInteractable(canClickOutline);
			outlinePageRefers.SetPageShow(showPage);
			TooltipInvoker tip = outlinePageRefers.GetPageTip();
			TooltipInvoker noPageTip = outlinePageRefers.GetNoPageTip();
			bool flag4 = broken && this._breakPlate == null && this.IsTaiwu();
			if (flag4)
			{
				PracticeSkillPlatePageUtils.RefreshOutlinePageTipLegacy(tip, this._combatSkillDisplayData.TemplateId, k, showPage);
			}
			else
			{
				PracticeSkillPlatePageUtils.RefreshOutlinePageTip(tip, k, true, showPage);
				PracticeSkillPlatePageUtils.RefreshOutlinePageTip(noPageTip, k, true, showPage);
			}
			k += 1;
		}
		this._autoSelectingPage = false;
	}

	// Token: 0x06001FB7 RID: 8119 RVA: 0x000E71C4 File Offset: 0x000E53C4
	private bool IsTaiwu()
	{
		return this.CharacterMenu.IsTaiwu(this._charId);
	}

	// Token: 0x06001FB8 RID: 8120 RVA: 0x000E71E8 File Offset: 0x000E53E8
	private void RefreshCombatSkill()
	{
		this.commonCombatSkill.Refresh(this._combatSkillDisplayData);
		this.tip.RuntimeParam = this.commonCombatSkill.mouseTip.RuntimeParam;
		CombatSkillItem config = CombatSkill.Instance[this._combatSkillDisplayData.TemplateId];
		int bonusCount = config.SkillBreakPlate.BonusCount;
		int objectCount = this.bonusRefersArray.Length;
		List<sbyte> bonusGradeList = EasyPool.Get<List<sbyte>>();
		bonusGradeList.Clear();
		bool flag = this._combatSkillDisplayData.BreakBonusGrades != null;
		if (flag)
		{
			bonusGradeList.AddRange(this._combatSkillDisplayData.BreakBonusGrades);
		}
		int actualCount = bonusGradeList.Count;
		bonusGradeList.Sort();
		int diff = objectCount - bonusGradeList.Count;
		for (int i = 0; i < diff; i++)
		{
			bonusGradeList.Insert(0, -1);
		}
		bool isTaiwu = this.IsTaiwu();
		List<ValueTuple<SkillBreakPlateIndex, SkillBreakPlateBonus>> bonusList = new List<ValueTuple<SkillBreakPlateIndex, SkillBreakPlateBonus>>();
		bool flag2 = isTaiwu && this._breakPlate != null;
		if (flag2)
		{
			foreach (SkillBreakPlateIndex index in this._breakPlate.GetIndexes())
			{
				SkillBreakPlateGrid cell = this._breakPlate.GetGridAt(index);
				bool flag3 = cell.Template.Type == ESkillBreakGridTypeType.Bonus;
				if (flag3)
				{
					bonusList.Add(new ValueTuple<SkillBreakPlateIndex, SkillBreakPlateBonus>(index, this._breakPlate.GetBonus(index)));
				}
			}
			bonusList.Sort(([TupleElementNames(new string[]
			{
				"coordinate",
				"bonus"
			})] ValueTuple<SkillBreakPlateIndex, SkillBreakPlateBonus> x, [TupleElementNames(new string[]
			{
				"coordinate",
				"bonus"
			})] ValueTuple<SkillBreakPlateIndex, SkillBreakPlateBonus> y) => x.Item2.Grade.CompareTo(y.Item2.Grade));
			int diff2 = objectCount - bonusList.Count;
			for (int j = 0; j < diff2; j++)
			{
				bonusList.Insert(0, new ValueTuple<SkillBreakPlateIndex, SkillBreakPlateBonus>(SkillBreakPlateIndex.Invalid, SkillBreakPlateBonus.Invalid));
			}
		}
		for (int k = 0; k < objectCount; k++)
		{
			bool slotVisible = k > objectCount - 1 - bonusCount;
			CImage bonusImage = this.bonusRefersArray[k].CGet<CImage>("BonusImage");
			bonusImage.gameObject.SetActive(slotVisible);
			bool flag4 = slotVisible;
			if (flag4)
			{
				bonusImage.SetSprite("ui_sp_icon_combatskill_profoundtheory_{0}".GetFormat(((int)(bonusGradeList[k] + 1)).ToString()), false, null);
			}
			CButtonObsolete button = this.bonusRefersArray[k].GetComponent<CButtonObsolete>();
			TooltipInvoker tip = this.bonusRefersArray[k].CGet<TooltipInvoker>("Interact");
			bool interactable = isTaiwu && slotVisible && bonusGradeList[k] > -1 && this._breakPlate != null;
			button.interactable = interactable;
			bool flag5 = interactable;
			if (flag5)
			{
				ValueTuple<SkillBreakPlateIndex, SkillBreakPlateBonus> valueTuple = bonusList[k];
				SkillBreakPlateIndex coordinate = valueTuple.Item1;
				SkillBreakPlateBonus bonus = valueTuple.Item2;
				button.ClearAndAddListener(delegate
				{
					bool flag6 = coordinate != SkillBreakPlateIndex.Invalid;
					if (flag6)
					{
						this.OpenBreakPlateAndBonusCell(coordinate, bonus);
					}
				});
				tip.Type = TipType.CombatSkillOneBonus;
				Behaviour behaviour = tip;
				valueTuple = bonusList[k];
				behaviour.enabled = (valueTuple.Item2.Type > ESkillBreakPlateBonusType.None);
				TooltipInvoker tooltipInvoker = tip;
				if (tooltipInvoker.RuntimeParam == null)
				{
					tooltipInvoker.RuntimeParam = new ArgumentBox();
				}
				tip.RuntimeParam.Set<SkillBreakPlateBonus>("BonusData", bonusList[k].Item2).Set("CharId", this._combatSkillDisplayData.CharId).Set("SkillId", this._combatSkillDisplayData.TemplateId);
			}
			else
			{
				tip.enabled = false;
			}
		}
		EasyPool.Free<List<sbyte>>(bonusGradeList);
	}

	// Token: 0x06001FB9 RID: 8121 RVA: 0x000E75A0 File Offset: 0x000E57A0
	private void OpenBreakPlateAndBonusCell(SkillBreakPlateIndex coordinate, SkillBreakPlateBonus bonus)
	{
		bool broken = CombatSkillStateHelper.IsBrokenOut(this._combatSkillDisplayData.ActivationState);
		bool isReview = broken;
		PracticeSkillPlatePageUtils.OpenBreakPlate(isReview, this._combatSkillDisplayData.TemplateId, this._qualificationTypeName, this._qualificationTypeIcon, this._qualificationRequireValue, this.GetSelectedPage(), null, new SkillBreakPlateIndex?(coordinate));
	}

	// Token: 0x06001FBA RID: 8122 RVA: 0x000E75F4 File Offset: 0x000E57F4
	private void SetContentPosition()
	{
		bool flag = this._referenceRect != null && this.contentTrans != null;
		if (flag)
		{
			Vector3[] rectCorners = new Vector3[4];
			this._referenceRect.GetWorldCorners(rectCorners);
			Vector3 centerWorld = (rectCorners[0] + rectCorners[2]) * 0.5f;
			RectTransform parent;
			bool flag2 = base.transform.parent.TryGetComponent<RectTransform>(out parent);
			if (flag2)
			{
				Vector3 centerLocal = parent.InverseTransformPoint(centerWorld);
				centerLocal.x = Mathf.Clamp(centerLocal.x, this.minAnchorX, this.maxAnchorX);
				centerLocal.y = Mathf.Clamp(centerLocal.y, this.minAnchorY, this.maxAnchorY);
				this.contentTrans.anchoredPosition = centerLocal;
			}
		}
	}

	// Token: 0x06001FBB RID: 8123 RVA: 0x000E76CE File Offset: 0x000E58CE
	private void InitToggleGroups()
	{
		this.InitOutlineToggleGroup();
		this.InitOtherPageToggleGroup();
		this.InitPresetToggleGroup();
	}

	// Token: 0x06001FBC RID: 8124 RVA: 0x000E76E6 File Offset: 0x000E58E6
	private void InitPresetToggleGroup()
	{
		this.presetTogGroup.InitPreOnToggle(-1);
		this.presetTogGroup.OnActiveToggleChange = delegate(CToggleObsolete togNew, CToggleObsolete _)
		{
			bool autoSelectingBreakPlate = this._autoSelectingBreakPlate;
			if (!autoSelectingBreakPlate)
			{
				bool flag = this._combatSkillDisplayData == null;
				if (!flag)
				{
					sbyte targetIndex = (sbyte)togNew.Key;
					bool flag2 = this._combatSkillDisplayData.BreakPlateIndex == targetIndex;
					if (!flag2)
					{
						TaiwuDomainMethod.Call.ChangeCombatSkillBreakPlate(this._combatSkillDisplayData.TemplateId, targetIndex);
						this._breakPlate = null;
						this.RequestBreakPlateData();
						this.GetCombatSkillDisplayData();
					}
				}
			}
		};
	}

	// Token: 0x06001FBD RID: 8125 RVA: 0x000E7710 File Offset: 0x000E5910
	private void InitOutlineToggleGroup()
	{
		bool flag = this.outlineToggleGroup == null;
		if (!flag)
		{
			string language = SingletonObject.getInstance<GlobalSettings>().Language.ToLower();
			List<CToggleObsolete> outlineTogList = this.outlineToggleGroup.GetAll();
			for (int index = 0; index < outlineTogList.Count; index++)
			{
				CToggleObsolete tog = outlineTogList[index];
				tog.Key = index;
				PracticeSlice slice = tog.GetComponent<PracticeSlice>();
				slice.AutoRotateNameBg();
				slice.SetNameLabel(string.Format("ui9_text_combat_skill_outline_{0}_cn", index));
				slice.RefreshStyle();
			}
			this.outlineToggleGroup.InitPreOnToggle(-1);
			this.outlineToggleGroup.OnActiveToggleChange = delegate(CToggleObsolete togNew, CToggleObsolete togOld)
			{
				PracticeSlice slice2 = (togNew != null) ? togNew.GetComponent<PracticeSlice>() : null;
				if (slice2 != null)
				{
					slice2.SetSelected(true);
				}
				PracticeSlice sliceOld = (togOld != null) ? togOld.GetComponent<PracticeSlice>() : null;
				if (sliceOld != null)
				{
					sliceOld.SetSelected(false);
				}
				bool flag2 = !this._autoSelectingPage;
				if (flag2)
				{
					bool flag3 = togNew != null;
					if (flag3)
					{
						this.SaveActivationState(togNew.Key, true);
					}
				}
			};
		}
	}

	// Token: 0x06001FBE RID: 8126 RVA: 0x000E77D0 File Offset: 0x000E59D0
	private void InitOtherPageToggleGroup()
	{
		bool flag = this.otherPageToggleGroup == null;
		if (!flag)
		{
			string language = SingletonObject.getInstance<GlobalSettings>().Language.ToLower();
			List<CToggleObsolete> pageTogList = this.otherPageToggleGroup.GetAll();
			for (int index = 0; index < pageTogList.Count; index++)
			{
				CToggleObsolete tog = pageTogList[index];
				PracticeSlice slice = tog.GetComponent<PracticeSlice>();
				slice.AutoRotateNameBg();
				slice.RefreshStyle();
				bool flag2 = index < 5;
				if (flag2)
				{
					slice.SetNameLabel(string.Format("ui9_text_combat_skill_direct_{0}_cn", index));
				}
				else
				{
					slice.SetNameLabel(string.Format("ui9_text_combat_skill_reverse_{0}_cn", index));
				}
			}
			for (byte i = 1; i <= 5; i += 1)
			{
				int directKey = (int)CombatSkillStateHelper.GetNormalPageInternalIndex(0, i);
				int reverseKey = (int)CombatSkillStateHelper.GetNormalPageInternalIndex(1, i);
				pageTogList[(int)(i - 1)].Key = directKey;
				pageTogList[(int)(5 + i - 1)].Key = reverseKey;
				this._reverseOtherPageDict[directKey] = reverseKey;
				this._reverseOtherPageDict[reverseKey] = directKey;
			}
			this.otherPageToggleGroup.InitPreOnToggle(-1);
			this.otherPageToggleGroup.OnActiveToggleChange = delegate(CToggleObsolete togNew, CToggleObsolete togOld)
			{
				PracticeSlice slice2 = (togNew != null) ? togNew.GetComponent<PracticeSlice>() : null;
				if (slice2 != null)
				{
					slice2.SetSelected(true);
				}
				PracticeSlice sliceOld = (togOld != null) ? togOld.GetComponent<PracticeSlice>() : null;
				if (sliceOld != null)
				{
					sliceOld.SetSelected(false);
				}
				bool flag3 = togNew != null;
				if (flag3)
				{
					this.otherPageToggleGroup.Set(this._reverseOtherPageDict[togNew.Key], false, false);
					bool flag4 = !this._autoSelectingPage;
					if (flag4)
					{
						this.SaveActivationState(togNew.Key, true);
					}
				}
			};
		}
	}

	// Token: 0x06001FBF RID: 8127 RVA: 0x000E7924 File Offset: 0x000E5B24
	private void InitOtherSlicesInteract()
	{
		foreach (Refers refers in this.bonusRefersArray)
		{
			this.InitOtherSliceInteract(refers.gameObject);
		}
		this.InitOtherSliceInteract(this.jumpThresholdSliceButton.gameObject);
		this.InitOtherSliceInteract(this.masteredSliceButton.gameObject);
	}

	// Token: 0x06001FC0 RID: 8128 RVA: 0x000E7980 File Offset: 0x000E5B80
	private void InitOtherSliceInteract(GameObject slice)
	{
		PointerTrigger pointerTrigger = slice.GetComponent<PointerTrigger>();
		CImage image = slice.GetComponent<CImage>();
		image.color = new Color(0.39f, 0.37f, 0.32f);
		pointerTrigger.EnterEvent.RemoveAllListeners();
		pointerTrigger.EnterEvent.AddListener(delegate()
		{
			image.color = new Color(0.66f, 0.64f, 0.59f);
		});
		pointerTrigger.ExitEvent.RemoveAllListeners();
		pointerTrigger.ExitEvent.AddListener(delegate()
		{
			image.color = new Color(0.39f, 0.37f, 0.32f);
		});
	}

	// Token: 0x06001FC1 RID: 8129 RVA: 0x000E7A10 File Offset: 0x000E5C10
	private void SaveActivationState(int togNewKey, bool isActive)
	{
		bool flag = this._combatSkillDisplayData == null;
		if (!flag)
		{
			bool broken = CombatSkillStateHelper.IsBrokenOut(this._combatSkillDisplayData.ActivationState);
			bool flag2 = !broken;
			if (!flag2)
			{
				byte pageId = (byte)((togNewKey - 5) % 5 + 1);
				sbyte direction = (sbyte)((togNewKey - 5) / 5);
				if (isActive)
				{
					CombatSkillDomainMethod.Call.SetActivePage(this.Element.GameDataListenerId, this.CharacterMenu.CurCharacterId, this._combatSkillDisplayData.TemplateId, pageId, direction);
				}
				else
				{
					bool flag3 = !broken;
					if (flag3)
					{
						CombatSkillDomainMethod.Call.DeActivePage(this.Element.GameDataListenerId, this.CharacterMenu.CurCharacterId, this._combatSkillDisplayData.TemplateId, pageId, direction);
					}
				}
				byte internalIndex = CombatSkillStateHelper.GetNormalPageInternalIndex(direction, pageId);
				byte oppositeInternalIndex = CombatSkillStateHelper.GetNormalPageInternalIndex((direction == 0) ? 1 : 0, pageId);
				if (isActive)
				{
					bool flag4 = broken;
					if (flag4)
					{
						this._combatSkillDisplayData.ActivationState = CombatSkillStateHelper.SwitchNormalPageDirect(this._combatSkillDisplayData.ActivationState, (int)(pageId - 1));
						CToggleObsolete oppositeToggle = this.FindOtherToggle(pageId, direction);
						PracticeSlice oppositeSlice = oppositeToggle.GetComponent<PracticeSlice>();
						oppositeSlice.SetInteractable(true);
						PracticeSlice slice = this.otherPageToggleGroup.Get(togNewKey).GetComponent<PracticeSlice>();
						slice.SetInteractable(false);
					}
					else
					{
						this._combatSkillDisplayData.ActivationState = CombatSkillStateHelper.SetPageActive(this._combatSkillDisplayData.ActivationState, internalIndex);
						this._combatSkillDisplayData.ActivationState = CombatSkillStateHelper.SetPageInactive(this._combatSkillDisplayData.ActivationState, oppositeInternalIndex);
					}
				}
				else
				{
					this._combatSkillDisplayData.ActivationState = CombatSkillStateHelper.SetPageInactive(this._combatSkillDisplayData.ActivationState, internalIndex);
				}
				this.GetCombatSkillDisplayData();
				ArgumentBox args = new ArgumentBox();
				args.Set("CharId", this._charId);
				args.Set("SkillId", this._combatSkillDisplayData.TemplateId);
				GEvent.OnEvent(UiEvents.OnCombatSkillMasteryChanged, args);
			}
		}
	}

	// Token: 0x06001FC2 RID: 8130 RVA: 0x000E7BF4 File Offset: 0x000E5DF4
	private CToggleObsolete FindOtherToggle(byte pageId, sbyte direction)
	{
		return this.otherPageToggleGroup.Get((int)(pageId - 1 + ((direction == 0) ? 10 : 5)));
	}

	// Token: 0x06001FC3 RID: 8131 RVA: 0x000E7C20 File Offset: 0x000E5E20
	private ushort GetSelectedPage()
	{
		return PracticeSkillPlatePageUtils.GetSelectedPage(this.outlineToggleGroup, this.otherPageToggleGroup);
	}

	// Token: 0x06001FC4 RID: 8132 RVA: 0x000E7C44 File Offset: 0x000E5E44
	private void UpdateQualificationData()
	{
		ValueTuple<string, string, string> valueTuple = PracticeSkillPlatePageUtils.CalcQualificationInfo(this._combatSkillDisplayData.TemplateId, this._combatSkillQualifications, this._lifeSkillQualifications);
		this._qualificationTypeName = valueTuple.Item1;
		this._qualificationTypeIcon = valueTuple.Item2;
		this._qualificationRequireValue = valueTuple.Item3;
	}

	// Token: 0x06001FC5 RID: 8133 RVA: 0x000E7C94 File Offset: 0x000E5E94
	private void Update()
	{
		bool isIn = RectTransformUtility.RectangleContainsScreenPoint(this.commonCombatSkill.GetComponent<RectTransform>(), Input.mousePosition, UIManager.Instance.UiCamera);
		this.combatSkillHover.SetActive(isIn);
	}

	// Token: 0x06001FC6 RID: 8134 RVA: 0x000E7CD4 File Offset: 0x000E5ED4
	public override void InitMonitorFieldIds()
	{
		this.MonitorFields.Add(new UIBase.MonitorDataField(4, 0, (ulong)this._charId, new uint[]
		{
			96U,
			98U,
			46U
		}));
		this.MonitorFields.Add(new UIBase.MonitorDataField(19, 23, ulong.MaxValue, null));
		this.MonitorFields.Add(new UIBase.MonitorDataField(5, 13, ulong.MaxValue, null));
	}

	// Token: 0x06001FC7 RID: 8135 RVA: 0x000E7D3C File Offset: 0x000E5F3C
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
					bool flag = notification.DomainId == 5 && notification.MethodId == 50;
					if (flag)
					{
						Serializer.Deserialize(wrapper.DataPool, notification.ValueOffset, ref this._breakPlate);
						bool flag2 = this.IsTaiwu();
						if (flag2)
						{
							this.RefreshAll();
						}
					}
					bool flag3 = notification.DomainId == 7 && notification.MethodId == 3;
					if (flag3)
					{
						Serializer.Deserialize(wrapper.DataPool, notification.ValueOffset, ref this._combatSkillDisplayData);
						this.RefreshAll();
					}
				}
			}
			else
			{
				DataUid uid = notification.Uid;
				bool flag4 = uid.DomainId == 5 && uid.DataId == 13;
				if (flag4)
				{
					Serializer.DeserializeModifications<short>(wrapper.DataPool, notification.ValueOffset, this._taiwuCombatSkill);
					bool flag5 = this.IsTaiwu();
					if (flag5)
					{
						this.RefreshAll();
					}
				}
				else
				{
					bool flag6 = uid.SubId1 == 96U;
					if (flag6)
					{
						Serializer.Deserialize(wrapper.DataPool, notification.ValueOffset, ref this._lifeSkillQualifications);
					}
					else
					{
						bool flag7 = uid.SubId1 == 98U;
						if (flag7)
						{
							Serializer.Deserialize(wrapper.DataPool, notification.ValueOffset, ref this._combatSkillQualifications);
							this.UpdateQualificationData();
						}
						else
						{
							bool flag8 = uid.SubId1 == 46U;
							if (flag8)
							{
								Serializer.Deserialize(wrapper.DataPool, notification.ValueOffset, ref this._currLoopingNeigongSkillId);
							}
							else
							{
								bool flag9 = uid.DomainId == 19 && uid.DataId == 23;
								if (flag9)
								{
									Serializer.DeserializeModifications<sbyte>(wrapper.DataPool, notification.ValueOffset, this._legendaryBookBreakPlateCounts);
								}
							}
						}
					}
				}
			}
		}
	}

	// Token: 0x06001FC8 RID: 8136 RVA: 0x000E7F68 File Offset: 0x000E6168
	private void UpdateJumpThresholdItem()
	{
		CombatSkillDisplayData skillData;
		bool flag = !this.TryGetJumpThresholdSkillDisplayData(out skillData);
		if (flag)
		{
			this.jumpThresholdItem.SetActive(false);
			this.jumpThresholdSliceButton.interactable = false;
		}
		else
		{
			this.jumpThresholdItem.SetActive(true);
			this.jumpThresholdSliceButton.interactable = true;
			this.jumpThresholdSliceButton.ClearAndAddListener(new Action(this.OnClickJumpSetting));
			this.UpdateJumpThresholdValue((int)skillData.JumpThreshold);
		}
	}

	// Token: 0x06001FC9 RID: 8137 RVA: 0x000E7FE4 File Offset: 0x000E61E4
	private void UpdateJumpThresholdValue(int jumpThreshold)
	{
		this.jumpThresholdValueLabel.text = ((float)jumpThreshold / 10f).ToString("F1");
	}

	// Token: 0x06001FCA RID: 8138 RVA: 0x000E8014 File Offset: 0x000E6214
	private bool TryGetJumpThresholdSkillDisplayData(out CombatSkillDisplayData displayData)
	{
		displayData = null;
		bool flag = !this.IsTaiwu() || this._combatSkillDisplayData == null;
		bool result;
		if (flag)
		{
			result = false;
		}
		else
		{
			bool flag2 = this._combatSkillDisplayData.JumpThreshold < 0;
			if (flag2)
			{
				result = false;
			}
			else
			{
				displayData = this._combatSkillDisplayData;
				result = true;
			}
		}
		return result;
	}

	// Token: 0x06001FCB RID: 8139 RVA: 0x000E8064 File Offset: 0x000E6264
	private void OnClickJumpSetting()
	{
		CombatSkillDisplayData skillData;
		bool flag = !this.TryGetJumpThresholdSkillDisplayData(out skillData);
		if (!flag)
		{
			this.jumpThresholdPanel.SetActive(true);
			this.jumpThresholdSetting.Refresh(null, skillData);
			this.jumpThresholdSetting.OnConfirm = new Action(this.OnExitJumpSetting);
			this.jumpThresholdSetting.OnCancel = new Action(this.OnExitJumpSetting);
		}
	}

	// Token: 0x06001FCC RID: 8140 RVA: 0x000E80D0 File Offset: 0x000E62D0
	private void OnExitJumpSetting()
	{
		this.jumpThresholdPanel.SetActive(false);
		CombatSkillDisplayData skillData;
		bool flag = this.TryGetJumpThresholdSkillDisplayData(out skillData);
		if (flag)
		{
			this.UpdateJumpThresholdValue((int)skillData.JumpThreshold);
		}
	}

	// Token: 0x06001FCD RID: 8141 RVA: 0x000E8104 File Offset: 0x000E6304
	private void UpdateMasteredItem()
	{
		bool flag = this._combatSkillDisplayData == null;
		if (flag)
		{
			this.ClearMasteredItem();
		}
		else
		{
			if (this._masteredHelper == null)
			{
				this._masteredHelper = new CombatSkillViewMasteredHelper();
			}
			this._masteredHelper.SetData(this._combatSkillDisplayData);
			this._masteredHelper.OnMasteredStatusChanged = new Action<CombatSkillDisplayData>(this.OnSkillMasteredStateChanged);
			bool flag2 = !this._masteredHelper.CanOperate;
			if (flag2)
			{
				this.ClearMasteredItem();
			}
			else
			{
				this.masteredItem.SetActive(true);
				this.masteredSliceButton.interactable = true;
				this.masteredIcon.SetSpriteOnly(this._combatSkillDisplayData.Mastered ? "charactermenu3_21_tuan_jingjie_1" : "charactermenu3_21_tuan_jingjie_0", false, null);
				this.masteredValueLabel.text = this._combatSkillDisplayData.GridCount.ToString();
				this.masteredSliceButton.ClearAndAddListener(delegate
				{
					this._masteredHelper.OnClickChangeMastered();
				});
			}
		}
	}

	// Token: 0x06001FCE RID: 8142 RVA: 0x000E81F9 File Offset: 0x000E63F9
	private void ClearMasteredItem()
	{
		this.masteredItem.SetActive(false);
		this.masteredSliceButton.interactable = false;
	}

	// Token: 0x06001FCF RID: 8143 RVA: 0x000E8218 File Offset: 0x000E6418
	private void OnSkillMasteredStateChanged(CombatSkillDisplayData skillData)
	{
		this._combatSkillDisplayData = skillData;
		this.masteredIcon.SetSpriteOnly(skillData.Mastered ? "charactermenu3_21_tuan_jingjie_1" : "charactermenu3_21_tuan_jingjie_0", false, null);
		this.masteredValueLabel.text = skillData.GridCount.ToString();
		ArgumentBox args = new ArgumentBox();
		args.Set("CharId", this._charId);
		args.Set("SkillId", skillData.TemplateId);
		GEvent.OnEvent(UiEvents.OnCombatSkillMasteryChanged, args);
		this.GetCombatSkillDisplayData();
	}

	// Token: 0x06001FD0 RID: 8144 RVA: 0x000E82A8 File Offset: 0x000E64A8
	protected override void OnClick(Transform btn)
	{
		string name = btn.name;
		string a = name;
		if (!(a == "CloseButton"))
		{
			if (!(a == "CloseButton2"))
			{
				if (!(a == "BreakButton"))
				{
					if (a == "JumpThresholdPanelCloseButton")
					{
						this.OnExitJumpSetting();
					}
				}
				else
				{
					this.GotoMenuPractice();
				}
			}
			else
			{
				this.QuickHide();
			}
		}
		else
		{
			this.QuickHide();
		}
	}

	// Token: 0x06001FD1 RID: 8145 RVA: 0x000E831C File Offset: 0x000E651C
	private void GotoMenuPractice()
	{
		this.QuickHide();
		ArgumentBox args = new ArgumentBox();
		args.SetObject("TargetPageIndex", ECharacterSubToggleBase.PracticeBase);
		GEvent.OnEvent(UiEvents.OnNeedOpenCharacterMenuSubPage, args);
		bool flag = this._combatSkillDisplayData != null;
		if (flag)
		{
			ArgumentBox skillArgs = new ArgumentBox();
			skillArgs.Set("TemplateId", this._combatSkillDisplayData.TemplateId);
			GEvent.OnEvent(UiEvents.EnterCharacterMenuPractice, skillArgs);
		}
	}

	// Token: 0x06001FD2 RID: 8146 RVA: 0x000E8397 File Offset: 0x000E6597
	public override void QuickHide()
	{
		this.OnExitJumpSetting();
		base.QuickHide();
	}

	// Token: 0x040017D6 RID: 6102
	private RectTransform _referenceRect;

	// Token: 0x040017D7 RID: 6103
	private CombatSkillDisplayData _combatSkillDisplayData;

	// Token: 0x040017D8 RID: 6104
	private int _charId;

	// Token: 0x040017D9 RID: 6105
	[SerializeField]
	private float maxAnchorX = 1000f;

	// Token: 0x040017DA RID: 6106
	[SerializeField]
	private float minAnchorX = -1000f;

	// Token: 0x040017DB RID: 6107
	[SerializeField]
	private float maxAnchorY = 1000f;

	// Token: 0x040017DC RID: 6108
	[SerializeField]
	private float minAnchorY = -1000f;

	// Token: 0x040017DD RID: 6109
	private const int NormalPagesCount = 5;

	// Token: 0x040017DE RID: 6110
	private const int OutlinePagesCount = 5;

	// Token: 0x040017DF RID: 6111
	private readonly Dictionary<int, int> _reverseOtherPageDict = new Dictionary<int, int>();

	// Token: 0x040017E0 RID: 6112
	private bool _autoSelectingPage;

	// Token: 0x040017E1 RID: 6113
	private bool _autoSelectingBreakPlate;

	// Token: 0x040017E2 RID: 6114
	private string _qualificationTypeName;

	// Token: 0x040017E3 RID: 6115
	private string _qualificationTypeIcon;

	// Token: 0x040017E4 RID: 6116
	private string _qualificationRequireValue;

	// Token: 0x040017E5 RID: 6117
	private GameData.Domains.Taiwu.SkillBreakPlate _breakPlate;

	// Token: 0x040017E6 RID: 6118
	private readonly Dictionary<short, TaiwuCombatSkill> _taiwuCombatSkill = new Dictionary<short, TaiwuCombatSkill>();

	// Token: 0x040017E7 RID: 6119
	private LifeSkillShorts _lifeSkillQualifications;

	// Token: 0x040017E8 RID: 6120
	private CombatSkillShorts _combatSkillQualifications;

	// Token: 0x040017E9 RID: 6121
	private short _currLoopingNeigongSkillId = -1;

	// Token: 0x040017EA RID: 6122
	private readonly Dictionary<sbyte, sbyte> _legendaryBookBreakPlateCounts = new Dictionary<sbyte, sbyte>();

	// Token: 0x040017EB RID: 6123
	private CombatSkillViewMasteredHelper _masteredHelper;

	// Token: 0x040017EC RID: 6124
	[SerializeField]
	private RectTransform contentTrans;

	// Token: 0x040017ED RID: 6125
	[SerializeField]
	private CommonCombatSkill commonCombatSkill;

	// Token: 0x040017EE RID: 6126
	[SerializeField]
	private TooltipInvoker tip;

	// Token: 0x040017EF RID: 6127
	[SerializeField]
	private GameObject combatSkillHover;

	// Token: 0x040017F0 RID: 6128
	[SerializeField]
	private CToggleGroupObsolete outlineToggleGroup;

	// Token: 0x040017F1 RID: 6129
	[SerializeField]
	private CToggleGroupObsolete otherPageToggleGroup;

	// Token: 0x040017F2 RID: 6130
	[SerializeField]
	private GameObject requireQualification;

	// Token: 0x040017F3 RID: 6131
	[SerializeField]
	private CImage requireQualificationTypeIcon;

	// Token: 0x040017F4 RID: 6132
	[SerializeField]
	private TextMeshProUGUI requireQualificationValueLabel;

	// Token: 0x040017F5 RID: 6133
	[SerializeField]
	private CToggleGroupObsolete presetTogGroup;

	// Token: 0x040017F6 RID: 6134
	[SerializeField]
	private Refers[] bonusRefersArray;

	// Token: 0x040017F7 RID: 6135
	[SerializeField]
	private CButtonObsolete jumpThresholdSliceButton;

	// Token: 0x040017F8 RID: 6136
	[SerializeField]
	private GameObject jumpThresholdItem;

	// Token: 0x040017F9 RID: 6137
	[SerializeField]
	private TextMeshProUGUI jumpThresholdValueLabel;

	// Token: 0x040017FA RID: 6138
	[SerializeField]
	private GameObject jumpThresholdPanel;

	// Token: 0x040017FB RID: 6139
	[SerializeField]
	private CombatJumpThreshold jumpThresholdSetting;

	// Token: 0x040017FC RID: 6140
	[SerializeField]
	private CButtonObsolete masteredSliceButton;

	// Token: 0x040017FD RID: 6141
	[SerializeField]
	private GameObject masteredItem;

	// Token: 0x040017FE RID: 6142
	[SerializeField]
	private CImage masteredIcon;

	// Token: 0x040017FF RID: 6143
	[SerializeField]
	private TextMeshProUGUI masteredValueLabel;
}
