using System;
using System.Collections.Generic;
using System.Linq;
using Config;
using FrameWork;
using FrameWork.UISystem.UIElements;
using Game.Views.Combat.Migrate;
using GameData.Domains.Combat;
using GameData.Domains.CombatSkill;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace Game.Views.Combat
{
	// Token: 0x02000AF4 RID: 2804
	public class CombatSkillWheel : MonoBehaviour
	{
		// Token: 0x17000F33 RID: 3891
		// (get) Token: 0x060089C4 RID: 35268 RVA: 0x003FC98E File Offset: 0x003FAB8E
		private CombatModel Model
		{
			get
			{
				return SingletonObject.getInstance<CombatModel>();
			}
		}

		// Token: 0x060089C5 RID: 35269 RVA: 0x003FC998 File Offset: 0x003FAB98
		public void Init()
		{
			this._cachedEscHandler = new Action(this.Close);
			this.closeButton.onClick.AddListener(new UnityAction(this.Close));
			this.pagePrev.onClick.AddListener(delegate()
			{
				this.SetPage(this._currentPage - 1);
			});
			this.pageNext.onClick.AddListener(delegate()
			{
				this.SetPage(this._currentPage + 1);
			});
			this.InitializeEquipTypeButtons();
			this.RefreshHotkeyDisplay();
			this.SetEquipType(1);
			this.SetupModelListeners();
			this.wheelRoot.gameObject.SetActive(false);
		}

		// Token: 0x060089C6 RID: 35270 RVA: 0x003FCA40 File Offset: 0x003FAC40
		private void SetupModelListeners()
		{
			CombatModel model = this.Model;
			model.OnCombatSkillCanUseChanged = (OnCombatSkillDataChangedEvent)Delegate.Combine(model.OnCombatSkillCanUseChanged, new OnCombatSkillDataChangedEvent(this.OnCombatSkillStateChanged));
			CombatModel model2 = this.Model;
			model2.OnCombatSkillLeftCdFrameChanged = (OnCombatSkillDataChangedEvent)Delegate.Combine(model2.OnCombatSkillLeftCdFrameChanged, new OnCombatSkillDataChangedEvent(this.OnCombatSkillCdChanged));
			CombatModel model3 = this.Model;
			model3.OnCombatSkillTotalCdFrameChanged = (OnCombatSkillDataChangedEvent)Delegate.Combine(model3.OnCombatSkillTotalCdFrameChanged, new OnCombatSkillDataChangedEvent(this.OnCombatSkillCdChanged));
			CombatModel model4 = this.Model;
			model4.OnCombatSkillBanReasonChanged = (OnCombatSkillDataChangedEvent)Delegate.Combine(model4.OnCombatSkillBanReasonChanged, new OnCombatSkillDataChangedEvent(this.OnCombatSkillStateChanged));
			CombatModel model5 = this.Model;
			model5.OnPreparingSkillIdChanged = (OnDataChangedEvent)Delegate.Combine(model5.OnPreparingSkillIdChanged, new OnDataChangedEvent(this.OnPreparingOrPerformingChanged));
			CombatModel model6 = this.Model;
			model6.OnPerformingSkillIdChanged = (OnDataChangedEvent)Delegate.Combine(model6.OnPerformingSkillIdChanged, new OnDataChangedEvent(this.OnPreparingOrPerformingChanged));
			this.Model.OnGetProactiveSkillList += this.OnGetProactiveSkillList;
		}

		// Token: 0x060089C7 RID: 35271 RVA: 0x003FCB50 File Offset: 0x003FAD50
		private void OnDestroy()
		{
			CombatModel model = this.Model;
			bool flag = model == null;
			if (!flag)
			{
				CombatModel combatModel = model;
				combatModel.OnCombatSkillCanUseChanged = (OnCombatSkillDataChangedEvent)Delegate.Remove(combatModel.OnCombatSkillCanUseChanged, new OnCombatSkillDataChangedEvent(this.OnCombatSkillStateChanged));
				CombatModel combatModel2 = model;
				combatModel2.OnCombatSkillLeftCdFrameChanged = (OnCombatSkillDataChangedEvent)Delegate.Remove(combatModel2.OnCombatSkillLeftCdFrameChanged, new OnCombatSkillDataChangedEvent(this.OnCombatSkillCdChanged));
				CombatModel combatModel3 = model;
				combatModel3.OnCombatSkillTotalCdFrameChanged = (OnCombatSkillDataChangedEvent)Delegate.Remove(combatModel3.OnCombatSkillTotalCdFrameChanged, new OnCombatSkillDataChangedEvent(this.OnCombatSkillCdChanged));
				CombatModel combatModel4 = model;
				combatModel4.OnCombatSkillBanReasonChanged = (OnCombatSkillDataChangedEvent)Delegate.Remove(combatModel4.OnCombatSkillBanReasonChanged, new OnCombatSkillDataChangedEvent(this.OnCombatSkillStateChanged));
				CombatModel combatModel5 = model;
				combatModel5.OnPreparingSkillIdChanged = (OnDataChangedEvent)Delegate.Remove(combatModel5.OnPreparingSkillIdChanged, new OnDataChangedEvent(this.OnPreparingOrPerformingChanged));
				CombatModel combatModel6 = model;
				combatModel6.OnPerformingSkillIdChanged = (OnDataChangedEvent)Delegate.Remove(combatModel6.OnPerformingSkillIdChanged, new OnDataChangedEvent(this.OnPreparingOrPerformingChanged));
				model.OnGetProactiveSkillList -= this.OnGetProactiveSkillList;
			}
		}

		// Token: 0x060089C8 RID: 35272 RVA: 0x003FCC54 File Offset: 0x003FAE54
		private void OnCombatSkillStateChanged(CombatSkillKey combatSkillKey)
		{
			bool flag = !this.IsOpened || combatSkillKey.CharId != this.Model.SelfCharId;
			if (!flag)
			{
				CombatProactiveSkillView slot = this.FindSlotByTemplateId(combatSkillKey.SkillTemplateId);
				bool flag2 = slot == null;
				if (!flag2)
				{
					CombatSubProcessorSkill processorSkill;
					bool flag3 = this.Model.ProcessorSkills.TryGetValue(combatSkillKey, out processorSkill);
					if (flag3)
					{
						slot.SetInteractable(processorSkill.CanUse && this.Model.CanOperateSelfCharacter);
					}
					slot.SetBodyPartBroken();
				}
			}
		}

		// Token: 0x060089C9 RID: 35273 RVA: 0x003FCCE4 File Offset: 0x003FAEE4
		private void OnCombatSkillCdChanged(CombatSkillKey combatSkillKey)
		{
			bool flag = !this.IsOpened || combatSkillKey.CharId != this.Model.SelfCharId;
			if (!flag)
			{
				CombatProactiveSkillView slot = this.FindSlotByTemplateId(combatSkillKey.SkillTemplateId);
				bool flag2 = slot == null;
				if (!flag2)
				{
					slot.UpdateSkillCdAndLock(combatSkillKey.SkillTemplateId);
				}
			}
		}

		// Token: 0x060089CA RID: 35274 RVA: 0x003FCD40 File Offset: 0x003FAF40
		private void OnPreparingOrPerformingChanged(bool isAlly)
		{
			bool flag = !this.IsOpened || !isAlly;
			if (!flag)
			{
				this.RefreshSlots();
			}
		}

		// Token: 0x060089CB RID: 35275 RVA: 0x003FCD6C File Offset: 0x003FAF6C
		private void OnGetProactiveSkillList(int charId)
		{
			bool flag = charId != this.Model.SelfCharId;
			if (!flag)
			{
				this.FilterSkillsByEquipType(this._currentCategory);
				this.RefreshSlots();
				this.UpdatePageButtons();
			}
		}

		// Token: 0x060089CC RID: 35276 RVA: 0x003FCDAC File Offset: 0x003FAFAC
		public void UpdateAffectingSkills(short affectingAgileSkillId, short affectingDefenseSkillId)
		{
			bool flag = !this.IsOpened;
			if (!flag)
			{
				this._affectingAgileSkillId = affectingAgileSkillId;
				this._affectingDefenseSkillId = affectingDefenseSkillId;
				this.RefreshSlots();
			}
		}

		// Token: 0x060089CD RID: 35277 RVA: 0x003FCDE0 File Offset: 0x003FAFE0
		private CombatProactiveSkillView FindSlotByTemplateId(short templateId)
		{
			int startIndex = this._currentPage * 8;
			for (int i = 0; i < this.skillSlots.Length; i++)
			{
				int skillIndex = startIndex + i;
				bool flag = skillIndex >= 0 && skillIndex < this._filteredSkills.Count && this._filteredSkills[skillIndex].TemplateId == templateId && this.skillSlots[i].gameObject.activeSelf;
				if (flag)
				{
					return this.skillSlots[i];
				}
			}
			return null;
		}

		// Token: 0x060089CE RID: 35278 RVA: 0x003FCE6C File Offset: 0x003FB06C
		private void InitializeEquipTypeButtons()
		{
			for (int i = 0; i < this.equipTypeButtons.Length; i++)
			{
				sbyte equipType = CombatSkillWheel.Categories[i];
				SkillWheelEquipTypeButton button = this.equipTypeButtons[i];
				string typeName = CombatSkillWheel.GetEquipTypeName(equipType);
				button.Initialize(equipType, typeName);
				button.OnClicked += this.OnEquipTypeButtonClicked;
			}
		}

		// Token: 0x060089CF RID: 35279 RVA: 0x003FCEC9 File Offset: 0x003FB0C9
		private void OnEquipTypeButtonClicked(SkillWheelEquipTypeButton clickedButton)
		{
			this.SetEquipType(clickedButton.EquipType);
		}

		// Token: 0x17000F34 RID: 3892
		// (get) Token: 0x060089D0 RID: 35280 RVA: 0x003FCED9 File Offset: 0x003FB0D9
		public bool IsOpened
		{
			get
			{
				return this._openFrameCount > 0;
			}
		}

		// Token: 0x060089D1 RID: 35281 RVA: 0x003FCEE4 File Offset: 0x003FB0E4
		public void OpenAtPosition(Vector2 screenPosition, short affectingAgileSkillId, short affectingDefenseSkillId)
		{
			bool flag = this._openFrameCount > 0;
			if (!flag)
			{
				this._affectingAgileSkillId = affectingAgileSkillId;
				this._affectingDefenseSkillId = affectingDefenseSkillId;
				Vector2 localPoint;
				bool flag2 = !RectTransformUtility.ScreenPointToLocalPointInRectangle(this.wheelRoot.parent as RectTransform, screenPosition, UIManager.Instance.UiCamera, out localPoint);
				if (!flag2)
				{
					this.wheelRoot.localPosition = localPoint;
					this.ClampWheelInsideScreen();
					this.wheelRoot.gameObject.SetActive(true);
					UIManager.Instance.MaskComponent(this.wheelRoot);
					UIManager.Instance.SetEscHandler(this._cachedEscHandler);
					this._openFrameCount++;
					Action onSkillWheelOpen = this.OnSkillWheelOpen;
					if (onSkillWheelOpen != null)
					{
						onSkillWheelOpen();
					}
					this.RefreshSlots();
					this.RefreshHotkeyDisplay();
				}
			}
		}

		// Token: 0x060089D2 RID: 35282 RVA: 0x003FCFB8 File Offset: 0x003FB1B8
		public void Close()
		{
			bool flag = this._openFrameCount <= 0;
			if (!flag)
			{
				this.wheelRoot.gameObject.SetActive(false);
				UIManager.Instance.UnMaskComponent(this.wheelRoot);
				bool flag2 = UIManager.Instance.CheckEscHandler(this._cachedEscHandler);
				if (flag2)
				{
					UIManager.Instance.SetEscHandler(null);
				}
				this._openFrameCount = 0;
				this._skillWheelOpened = false;
				this._rightClickHoldTime = 0f;
				this._hoveredSkillId = -1;
				this.ClearInfoPanel();
				this.HideWheelSkillTip();
				this.HideBreaker();
				Action onSkillWheelClose = this.OnSkillWheelClose;
				if (onSkillWheelClose != null)
				{
					onSkillWheelClose();
				}
			}
		}

		// Token: 0x060089D3 RID: 35283 RVA: 0x003FD068 File Offset: 0x003FB268
		public void ForceClose()
		{
			bool flag = UIManager.Instance.CheckEscHandler(this._cachedEscHandler);
			if (flag)
			{
				UIManager.Instance.SetEscHandler(null);
			}
			UIManager.Instance.UnMaskComponent(this.wheelRoot);
			bool activeSelf = this.wheelRoot.gameObject.activeSelf;
			if (activeSelf)
			{
				this.Close();
			}
		}

		// Token: 0x060089D4 RID: 35284 RVA: 0x003FD0C4 File Offset: 0x003FB2C4
		private void Update()
		{
			bool flag = !this.wheelRoot.gameObject.activeSelf;
			if (!flag)
			{
				bool flag2 = this._openFrameCount > 1 && Input.GetMouseButtonDown(1);
				if (flag2)
				{
					this.Close();
				}
				else
				{
					this._openFrameCount++;
					bool flag3 = CombatSkillWheel.IsCommandDown(TabSwitchCommandKit.PrevTabLevel1);
					if (flag3)
					{
						AudioManager.Instance.PlaySound("ui_default_click_left", false, false);
						int currentIndex = Array.IndexOf<sbyte>(CombatSkillWheel.Categories, this._currentCategory);
						int nextIndex = (currentIndex - 1 + CombatSkillWheel.Categories.Length) % CombatSkillWheel.Categories.Length;
						this.SetEquipType(CombatSkillWheel.Categories[nextIndex]);
					}
					else
					{
						bool flag4 = CombatSkillWheel.IsCommandDown(TabSwitchCommandKit.NextTabLevel1);
						if (flag4)
						{
							AudioManager.Instance.PlaySound("ui_default_click_left", false, false);
							int currentIndex2 = Array.IndexOf<sbyte>(CombatSkillWheel.Categories, this._currentCategory);
							int nextIndex2 = (currentIndex2 + 1) % CombatSkillWheel.Categories.Length;
							this.SetEquipType(CombatSkillWheel.Categories[nextIndex2]);
						}
					}
					bool flag5 = CombatSkillWheel.IsCommandDown(TabSwitchCommandKit.PrevTabLevel2);
					if (flag5)
					{
						AudioManager.Instance.PlaySound("ui_default_click_left", false, false);
						this.SetPage(this._currentPage - 1);
					}
					else
					{
						bool flag6 = CombatSkillWheel.IsCommandDown(TabSwitchCommandKit.NextTabLevel2);
						if (flag6)
						{
							AudioManager.Instance.PlaySound("ui_default_click_left", false, false);
							this.SetPage(this._currentPage + 1);
						}
					}
					this.TickWheelSkillTipByHotkey();
				}
			}
		}

		// Token: 0x060089D5 RID: 35285 RVA: 0x003FD23C File Offset: 0x003FB43C
		private void SetEquipType(sbyte equipType)
		{
			bool flag = this._currentCategory == equipType;
			if (!flag)
			{
				this._currentCategory = equipType;
				this._currentPage = 0;
				this.UpdateEquipTypeButtons();
				this.FilterSkillsByEquipType(equipType);
				this.RefreshSlots();
				this.UpdatePageButtons();
			}
		}

		// Token: 0x060089D6 RID: 35286 RVA: 0x003FD284 File Offset: 0x003FB484
		private void SetPage(int page)
		{
			int maxPage = this.GetMaxPage();
			page = Mathf.Clamp(page, 0, maxPage);
			bool flag = this._currentPage == page;
			if (!flag)
			{
				this._currentPage = page;
				this.RefreshSlots();
				this.UpdatePageButtons();
			}
		}

		// Token: 0x060089D7 RID: 35287 RVA: 0x003FD2C8 File Offset: 0x003FB4C8
		private void FilterSkillsByEquipType(sbyte equipType)
		{
			this._filteredSkills.Clear();
			IReadOnlyList<CombatSkillDisplayData> skillList;
			bool flag = !this.Model.ProactiveSkillData.TryGetValue(this.Model.SelfCharId, out skillList);
			if (!flag)
			{
				foreach (CombatSkillDisplayData skill in skillList)
				{
					CombatSkillItem config = CombatSkill.Instance[skill.TemplateId];
					bool flag2 = config.EquipType == equipType;
					if (flag2)
					{
						this._filteredSkills.Add(skill);
					}
				}
			}
		}

		// Token: 0x060089D8 RID: 35288 RVA: 0x003FD374 File Offset: 0x003FB574
		private void RefreshSlots()
		{
			int startIndex = this._currentPage * 8;
			for (int i = 0; i < this.skillSlots.Length; i++)
			{
				int skillIndex = startIndex + i;
				bool hasSkill = skillIndex >= 0 && skillIndex < this._filteredSkills.Count;
				bool flag = hasSkill;
				if (flag)
				{
					CombatSkillDisplayData skillData = this._filteredSkills[skillIndex];
					short templateId = skillData.TemplateId;
					CombatProactiveSkillView slot = this.skillSlots[i];
					slot.gameObject.SetActive(true);
					slot.SetData(skillData, delegate
					{
						this.OnSkillClick(templateId);
					}, null, delegate
					{
						this.OnSkillEnter(templateId);
						CombatProactiveSkillView proactiveSkillView = slot.GetComponent<CombatProactiveSkillView>();
						bool flag4 = proactiveSkillView != null;
						if (flag4)
						{
							proactiveSkillView.OnMouseEnter();
						}
					}, delegate
					{
						this.OnSkillExit();
						CombatProactiveSkillView proactiveSkillView = slot.GetComponent<CombatProactiveSkillView>();
						bool flag4 = proactiveSkillView != null;
						if (flag4)
						{
							proactiveSkillView.OnMouseExit();
						}
					});
					CombatSkillKey combatSkillKey = new CombatSkillKey(this.Model.SelfCharId, templateId);
					CombatSubProcessorSkill processorSkill;
					bool flag2 = this.Model.ProcessorSkills.TryGetValue(combatSkillKey, out processorSkill);
					if (flag2)
					{
						this.skillSlots[i].SetInteractable(processorSkill.CanUse && this.Model.CanOperateSelfCharacter);
					}
					TooltipInvoker mouseTip = this.skillSlots[i].GetHolderComponent<TooltipInvoker>();
					bool flag3 = mouseTip != null;
					if (flag3)
					{
						mouseTip.enabled = false;
					}
				}
				else
				{
					this.skillSlots[i].gameObject.SetActive(false);
				}
			}
			this.UpdateBreaker();
		}

		// Token: 0x060089D9 RID: 35289 RVA: 0x003FD4F7 File Offset: 0x003FB6F7
		private void UpdateBreaker()
		{
			this.UpdateBreakerForSkill(this._affectingAgileSkillId, this.breakerAgile, "Agile");
			this.UpdateBreakerForSkill(this._affectingDefenseSkillId, this.breakerDefense, "Defense");
		}

		// Token: 0x060089DA RID: 35290 RVA: 0x003FD52C File Offset: 0x003FB72C
		private void UpdateBreakerForSkill(short affectingSkillId, GameObject breaker, string breakerTag)
		{
			bool flag = breaker == null;
			if (!flag)
			{
				CombatProactiveSkillView affectingSlot = this.FindSlotByTemplateId(affectingSkillId);
				bool flag2 = affectingSlot == null;
				if (flag2)
				{
					breaker.SetActive(false);
				}
				else
				{
					RectTransform breakerRect = breaker.GetComponent<RectTransform>();
					breakerRect.SetParent(affectingSlot.transform, false);
					breakerRect.anchoredPosition = Vector2.zero;
					breaker.SetActive(true);
					CButton btn = breaker.GetComponent<CButton>();
					bool flag3 = btn != null;
					if (flag3)
					{
						btn.ClearAndAddListener(delegate
						{
							Action<short> onBreakAffectingSkill = this.OnBreakAffectingSkill;
							if (onBreakAffectingSkill != null)
							{
								onBreakAffectingSkill(affectingSkillId);
							}
						});
					}
					PointerTrigger pointerTrigger = breaker.GetComponent<PointerTrigger>();
					bool flag4 = pointerTrigger != null;
					if (flag4)
					{
						pointerTrigger.EnterEvent.RemoveAllListeners();
						pointerTrigger.ExitEvent.RemoveAllListeners();
						pointerTrigger.EnterEvent.AddListener(delegate()
						{
							Transform transform = breaker.transform.Find("OnNormal");
							GameObject onNormal = (transform != null) ? transform.gameObject : null;
							Transform transform2 = breaker.transform.Find("OnHover");
							GameObject onHover = (transform2 != null) ? transform2.gameObject : null;
							bool flag5 = onNormal != null;
							if (flag5)
							{
								onNormal.SetActive(false);
							}
							bool flag6 = onHover != null;
							if (flag6)
							{
								onHover.SetActive(true);
							}
						});
						pointerTrigger.ExitEvent.AddListener(delegate()
						{
							Transform transform = breaker.transform.Find("OnNormal");
							GameObject onNormal = (transform != null) ? transform.gameObject : null;
							Transform transform2 = breaker.transform.Find("OnHover");
							GameObject onHover = (transform2 != null) ? transform2.gameObject : null;
							bool flag5 = onNormal != null;
							if (flag5)
							{
								onNormal.SetActive(true);
							}
							bool flag6 = onHover != null;
							if (flag6)
							{
								onHover.SetActive(false);
							}
						});
					}
				}
			}
		}

		// Token: 0x060089DB RID: 35291 RVA: 0x003FD664 File Offset: 0x003FB864
		private void HideBreaker()
		{
			bool flag = this.breakerAgile != null;
			if (flag)
			{
				this.breakerAgile.SetActive(false);
			}
			bool flag2 = this.breakerDefense != null;
			if (flag2)
			{
				this.breakerDefense.SetActive(false);
			}
		}

		// Token: 0x060089DC RID: 35292 RVA: 0x003FD6AC File Offset: 0x003FB8AC
		private void UpdateEquipTypeButtons()
		{
			for (int i = 0; i < this.equipTypeButtons.Length; i++)
			{
				SkillWheelEquipTypeButton button = this.equipTypeButtons[i];
				bool isSelected = this._currentCategory == button.EquipType;
				button.SetSelected(isSelected);
				bool flag = isSelected;
				if (flag)
				{
					this.centerTypeIcon.sprite = button.TypeIcon;
					this.centerTypeText.text = CombatSkillWheel.GetEquipTypeName(button.EquipType).SetColor(button.TypeTextColor);
				}
			}
		}

		// Token: 0x060089DD RID: 35293 RVA: 0x003FD738 File Offset: 0x003FB938
		private void UpdatePageButtons()
		{
			int maxPage = this.GetMaxPage();
			bool canGoPrev = this._currentPage > 0;
			bool canGoNext = this._currentPage < maxPage;
			bool flag = this.pagePrevContainer != null;
			if (flag)
			{
				this.pagePrevContainer.SetActive(canGoPrev);
			}
			bool flag2 = this.pageNextContainer != null;
			if (flag2)
			{
				this.pageNextContainer.SetActive(canGoNext);
			}
		}

		// Token: 0x060089DE RID: 35294 RVA: 0x003FD7A0 File Offset: 0x003FB9A0
		private int GetMaxPage()
		{
			int maxPage = Mathf.CeilToInt((float)this._filteredSkills.Count / 8f) - 1;
			return Mathf.Max(0, maxPage);
		}

		// Token: 0x060089DF RID: 35295 RVA: 0x003FD7D3 File Offset: 0x003FB9D3
		private void OnSkillClick(short templateId)
		{
			Action<short> onCastSkill = this.OnCastSkill;
			if (onCastSkill != null)
			{
				onCastSkill(templateId);
			}
			this.Close();
		}

		// Token: 0x060089E0 RID: 35296 RVA: 0x003FD7F0 File Offset: 0x003FB9F0
		private void OnSkillEnter(short templateId)
		{
			this._hoveredSkillId = templateId;
			this.RefreshInfoPanel(templateId);
			this.TickWheelSkillTipByHotkey();
		}

		// Token: 0x060089E1 RID: 35297 RVA: 0x003FD809 File Offset: 0x003FBA09
		private void OnSkillExit()
		{
			this._hoveredSkillId = -1;
			this.ClearInfoPanel();
			this.HideWheelSkillTip();
		}

		// Token: 0x060089E2 RID: 35298 RVA: 0x003FD824 File Offset: 0x003FBA24
		private void RefreshInfoPanel(short skillId)
		{
			IReadOnlyList<CombatSkillDisplayData> skillList;
			bool flag = !this.Model.ProactiveSkillData.TryGetValue(this.Model.SelfCharId, out skillList);
			if (!flag)
			{
				CombatSkillDisplayData skillData = skillList.FirstOrDefault((CombatSkillDisplayData s) => s.TemplateId == skillId);
				bool flag2 = skillData == null;
				if (!flag2)
				{
					CombatSkillItem config = CombatSkill.Instance[skillId];
					this.skillNameText.text = config.Name.SetColor(Colors.Instance.GradeColors[(int)config.Grade]);
					this.RefreshTrickRequirements(skillData.CostTricks);
					this.RefreshCostBars(skillData);
					this.infoPanel.SetActive(true);
				}
			}
		}

		// Token: 0x060089E3 RID: 35299 RVA: 0x003FD8E5 File Offset: 0x003FBAE5
		private void ClearInfoPanel()
		{
			this.infoPanel.SetActive(false);
			this.HideWheelSkillTip();
		}

		// Token: 0x060089E4 RID: 35300 RVA: 0x003FD8FC File Offset: 0x003FBAFC
		private void TickWheelSkillTipByHotkey()
		{
			bool altDown = Input.GetKey(KeyCode.LeftAlt) || Input.GetKey(KeyCode.RightAlt);
			bool shouldShow = altDown && this.infoPanel.activeSelf && this._hoveredSkillId >= 0;
			bool flag = !shouldShow;
			if (flag)
			{
				this.HideWheelSkillTip();
			}
			else
			{
				TooltipManager tipManager = SingletonObject.getInstance<TooltipManager>();
				bool shouldRefreshTip = !this._isShowingWheelSkillTip || this._showingWheelSkillTipId != this._hoveredSkillId || !tipManager.IsTipsVisible(TipType.CombatSkill);
				bool flag2 = !shouldRefreshTip;
				if (!flag2)
				{
					float tipFixedPosX = this.CheckShouldShowTipOnLeft() ? this.GetWheelTipFixedPosX(true) : this.GetWheelTipFixedPosX(false);
					ArgumentBox args = EasyPool.Get<ArgumentBox>();
					args.Set("CombatSkillId", this._hoveredSkillId).Set("CharId", this.Model.SelfCharId).Set("UsePracticeLevelInDisplayData", true).Set("TipFixedPosX", tipFixedPosX);
					tipManager.ShowTips(TipType.CombatSkill, args, false, false, false, null);
					this._isShowingWheelSkillTip = true;
					this._showingWheelSkillTipId = this._hoveredSkillId;
				}
			}
		}

		// Token: 0x060089E5 RID: 35301 RVA: 0x003FDA1C File Offset: 0x003FBC1C
		private void HideWheelSkillTip()
		{
			bool flag = !this._isShowingWheelSkillTip;
			if (!flag)
			{
				SingletonObject.getInstance<TooltipManager>().HideTips(TipType.CombatSkill, true);
				this._isShowingWheelSkillTip = false;
				this._showingWheelSkillTipId = -1;
			}
		}

		// Token: 0x060089E6 RID: 35302 RVA: 0x003FDA54 File Offset: 0x003FBC54
		private bool CheckShouldShowTipOnLeft()
		{
			Vector3[] wheelCorners = new Vector3[4];
			this.wheelRoot.GetWorldCorners(wheelCorners);
			Vector2 wheelRightScreenPos = RectTransformUtility.WorldToScreenPoint(UIManager.Instance.UiCamera, wheelCorners[2]);
			Canvas rootCanvas = this.wheelRoot.GetComponentInParent<Canvas>().rootCanvas;
			bool flag = rootCanvas == null;
			bool result;
			if (flag)
			{
				result = false;
			}
			else
			{
				RectTransform canvasRect = rootCanvas.GetComponent<RectTransform>();
				Vector3[] canvasCorners = new Vector3[4];
				canvasRect.GetWorldCorners(canvasCorners);
				Vector2 canvasTopRightScreenPos = RectTransformUtility.WorldToScreenPoint(UIManager.Instance.UiCamera, canvasCorners[2]);
				float screenRightEdge = canvasTopRightScreenPos.x;
				float availableSpace = screenRightEdge - wheelRightScreenPos.x;
				result = (availableSpace < 600f);
			}
			return result;
		}

		// Token: 0x060089E7 RID: 35303 RVA: 0x003FDB04 File Offset: 0x003FBD04
		private float GetWheelTipFixedPosX(bool showOnLeft)
		{
			Vector3[] wheelCorners = new Vector3[4];
			this.wheelRoot.GetWorldCorners(wheelCorners);
			Vector2 edgeScreenPos = showOnLeft ? RectTransformUtility.WorldToScreenPoint(UIManager.Instance.UiCamera, wheelCorners[0]) : RectTransformUtility.WorldToScreenPoint(UIManager.Instance.UiCamera, wheelCorners[2]);
			Canvas rootCanvas = this.wheelRoot.GetComponentInParent<Canvas>().rootCanvas;
			bool flag = rootCanvas == null;
			float result;
			if (flag)
			{
				result = (float)AspectRatioController.ViewSize.x * 0.5f;
			}
			else
			{
				RectTransform canvasRect = rootCanvas.GetComponent<RectTransform>();
				Vector2 localPoint;
				RectTransformUtility.ScreenPointToLocalPointInRectangle(canvasRect, edgeScreenPos, UIManager.Instance.UiCamera, out localPoint);
				float fixedPosX = localPoint.x + (float)AspectRatioController.ViewSize.x * 0.5f;
				if (showOnLeft)
				{
					fixedPosX -= 624f;
				}
				else
				{
					fixedPosX += 24f;
				}
				result = Mathf.Clamp(fixedPosX, 0f, (float)AspectRatioController.ViewSize.x);
			}
			return result;
		}

		// Token: 0x060089E8 RID: 35304 RVA: 0x003FDC0C File Offset: 0x003FBE0C
		private void RefreshTrickRequirements(List<NeedTrick> costTricks)
		{
			for (int i = 0; i < this.trickRequires.Length; i++)
			{
				this.trickRequires[i].gameObject.SetActive(false);
			}
			bool flag = costTricks == null || costTricks.Count == 0;
			if (!flag)
			{
				CombatSubProcessorCharacter processor = this.Model.ProcessorCharacters[this.Model.SelfCharId];
				sbyte[] currentTricks = processor.Tricks.Tricks.Values.ToArray<sbyte>();
				int trickIndex = 0;
				foreach (NeedTrick needTrick in costTricks)
				{
					int currentTrickCount = 0;
					foreach (sbyte trick in currentTricks)
					{
						bool flag2 = trick == needTrick.TrickType;
						if (flag2)
						{
							currentTrickCount++;
						}
					}
					int count = 0;
					while (count < (int)needTrick.NeedCount && trickIndex < this.trickRequires.Length)
					{
						CombatTrickPrefab trickPrefab = this.trickRequires[trickIndex];
						trickPrefab.gameObject.SetActive(true);
						CombatSkillWheel.UpdateSkillTrickIcon(trickPrefab, needTrick.TrickType, count, currentTrickCount);
						trickIndex++;
						count++;
					}
				}
			}
		}

		// Token: 0x060089E9 RID: 35305 RVA: 0x003FDD74 File Offset: 0x003FBF74
		private static void UpdateSkillTrickIcon(CombatTrickPrefab trickPrefab, sbyte trickType, int trickIndex, int currentTrickCount)
		{
			TrickTypeItem trickConfig = Config.TrickType.Instance[trickType];
			trickPrefab.mainImage.SetSprite(trickConfig.BackIcon, false, null);
			trickPrefab.trickName.text = trickConfig.Name.SetColor(trickConfig.FontColor);
			bool isTrickSatisfied = currentTrickCount > trickIndex;
			trickPrefab.currTrickMark.gameObject.SetActive(isTrickSatisfied);
			bool flag = trickPrefab.costPreview != null;
			if (flag)
			{
				trickPrefab.costPreview.SetActive(false);
			}
			TooltipInvoker mouseTip = trickPrefab.GetComponent<TooltipInvoker>();
			mouseTip.RuntimeParam = null;
			bool flag2 = GameData.Domains.Combat.TrickType.NoBodyDamageTrickType.IndexOf(trickType) == -1;
			if (flag2)
			{
				ArgumentBox argumentBox = EasyPool.Get<ArgumentBox>();
				argumentBox.Set("TrickType", trickType);
				argumentBox.Set("IsAvoidTrick", false);
				argumentBox.Set("IsSatisfied", isTrickSatisfied);
				mouseTip.RuntimeParam = argumentBox;
			}
		}

		// Token: 0x060089EA RID: 35306 RVA: 0x003FDE54 File Offset: 0x003FC054
		private void RefreshCostBars(CombatSkillDisplayData skillData)
		{
			CombatSubProcessorCharacter processor = this.Model.ProcessorCharacters[this.Model.SelfCharId];
			float breathPercent = (float)processor.BreathValue / 30000f;
			float stancePercent = (float)processor.StanceValue / 4000f;
			float mobilityPercent = (float)processor.MobilityValue / (float)MoveSpecialConstants.MaxMobility;
			bool flag = skillData.CostBreath > 0;
			if (flag)
			{
				this.breathBar.fillAmount = breathPercent;
				float breathAfterCost = breathPercent - (float)skillData.CostBreath / 100f;
				this.breathPreviewBar.gameObject.SetActive(true);
				this.breathPreviewBar.fillAmount = breathAfterCost;
			}
			else
			{
				this.breathBar.fillAmount = 0f;
				this.breathPreviewBar.gameObject.SetActive(true);
				this.breathPreviewBar.fillAmount = breathPercent;
			}
			bool flag2 = skillData.CostStance > 0;
			if (flag2)
			{
				this.stanceBar.fillAmount = stancePercent;
				float stanceAfterCost = stancePercent - (float)skillData.CostStance / 100f;
				this.stancePreviewBar.gameObject.SetActive(true);
				this.stancePreviewBar.fillAmount = stanceAfterCost;
			}
			else
			{
				this.stanceBar.fillAmount = 0f;
				this.stancePreviewBar.gameObject.SetActive(true);
				this.stancePreviewBar.fillAmount = stancePercent;
			}
			bool flag3 = skillData.CostMobility > 0;
			if (flag3)
			{
				this.mobilityBar.fillAmount = mobilityPercent;
				float costMobility = (float)MoveSpecialConstants.MaxMobility * (float)skillData.CostMobility / 100f;
				float mobilityAfterCost = ((float)processor.MobilityValue - costMobility) / (float)MoveSpecialConstants.MaxMobility;
				this.mobilityPreviewBar.gameObject.SetActive(true);
				this.mobilityPreviewBar.fillAmount = mobilityAfterCost;
			}
			else
			{
				this.mobilityBar.fillAmount = 0f;
				this.mobilityPreviewBar.gameObject.SetActive(true);
				this.mobilityPreviewBar.fillAmount = mobilityPercent;
			}
		}

		// Token: 0x060089EB RID: 35307 RVA: 0x003FE04C File Offset: 0x003FC24C
		private void ClampWheelInsideScreen()
		{
			Canvas canvas = this.wheelRoot.GetComponentInParent<Canvas>();
			RectTransform canvasRect = canvas.GetComponent<RectTransform>();
			Vector3[] screenCorners = new Vector3[4];
			canvasRect.GetWorldCorners(screenCorners);
			RectTransform parentRect = this.wheelRoot.parent as RectTransform;
			Vector3 min = parentRect.InverseTransformPoint(screenCorners[0]);
			Vector3 max = parentRect.InverseTransformPoint(screenCorners[2]);
			Rect rect = this.wheelRoot.rect;
			Vector2 pivot = this.wheelRoot.pivot;
			Vector2 minOffset = new Vector2(rect.width * pivot.x, rect.height * pivot.y);
			Vector2 maxOffset = new Vector2(rect.width * (1f - pivot.x), rect.height * (1f - pivot.y));
			Vector2 anchoredPos = this.wheelRoot.anchoredPosition;
			anchoredPos.x = Mathf.Clamp(anchoredPos.x, min.x + minOffset.x, max.x - maxOffset.x);
			anchoredPos.y = Mathf.Clamp(anchoredPos.y, min.y + minOffset.y, max.y - maxOffset.y);
			this.wheelRoot.anchoredPosition = anchoredPos;
		}

		// Token: 0x060089EC RID: 35308 RVA: 0x003FE198 File Offset: 0x003FC398
		private static string GetEquipTypeName(sbyte equipType)
		{
			if (!true)
			{
			}
			LanguageKey languageKey2;
			switch (equipType)
			{
			case 1:
				languageKey2 = LanguageKey.LK_CombatSkill_EquipType_1;
				break;
			case 2:
				languageKey2 = LanguageKey.LK_CombatSkill_EquipType_2;
				break;
			case 3:
				languageKey2 = LanguageKey.LK_CombatSkill_EquipType_3;
				break;
			default:
				languageKey2 = LanguageKey.Invalid;
				break;
			}
			if (!true)
			{
			}
			LanguageKey languageKey = languageKey2;
			return (languageKey != LanguageKey.Invalid) ? languageKey.Tr() : "";
		}

		// Token: 0x060089ED RID: 35309 RVA: 0x003FE1F8 File Offset: 0x003FC3F8
		private void RefreshHotkeyDisplay()
		{
			bool flag = this.prevEquipTypeHotkeyText != null;
			if (flag)
			{
				this.prevEquipTypeHotkeyText.text = CombatSkillWheel.GetHotkeyBracketText(TabSwitchCommandKit.PrevTabLevel1);
			}
			bool flag2 = this.nextEquipTypeHotkeyText != null;
			if (flag2)
			{
				this.nextEquipTypeHotkeyText.text = CombatSkillWheel.GetHotkeyBracketText(TabSwitchCommandKit.NextTabLevel1);
			}
			bool flag3 = this.prevPageHotkeyText != null;
			if (flag3)
			{
				this.prevPageHotkeyText.text = CombatSkillWheel.GetHotkeyBracketText(TabSwitchCommandKit.PrevTabLevel2);
			}
			bool flag4 = this.nextPageHotkeyText != null;
			if (flag4)
			{
				this.nextPageHotkeyText.text = CombatSkillWheel.GetHotkeyBracketText(TabSwitchCommandKit.NextTabLevel2);
			}
		}

		// Token: 0x060089EE RID: 35310 RVA: 0x003FE2A0 File Offset: 0x003FC4A0
		private static string GetHotkeyBracketText(HotKeyCommand command)
		{
			KeyCode[] keys = command.GetKeyCode(false);
			string keyText = string.Join("+", from s in keys.Select(new Func<KeyCode, string>(command.GetKeyCodeString))
			where !string.IsNullOrEmpty(s)
			select s);
			return "[" + keyText + "]";
		}

		// Token: 0x060089EF RID: 35311 RVA: 0x003FE30C File Offset: 0x003FC50C
		private static bool IsCommandDown(HotKeyCommand command)
		{
			HotKeyGroup keyGroup = command.KeyGroup;
			return CombatSkillWheel.IsKeyCombinationDown(keyGroup.Key, keyGroup.FunctionKey);
		}

		// Token: 0x060089F0 RID: 35312 RVA: 0x003FE338 File Offset: 0x003FC538
		private static bool IsKeyCombinationDown(KeyCode key, KeyCode functionKey)
		{
			bool flag = key == KeyCode.None || !Input.GetKeyDown(key);
			bool result;
			if (flag)
			{
				result = false;
			}
			else
			{
				bool controlDowning = Input.GetKey(KeyCode.LeftControl) || Input.GetKey(KeyCode.RightControl);
				bool shiftDowning = Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift);
				bool altDowning = Input.GetKey(KeyCode.LeftAlt) || Input.GetKey(KeyCode.RightAlt);
				bool flag2 = functionKey == KeyCode.None;
				if (flag2)
				{
					result = (!controlDowning && !shiftDowning && !altDowning);
				}
				else
				{
					if (!true)
					{
					}
					bool flag3;
					switch (functionKey)
					{
					case KeyCode.RightShift:
					case KeyCode.LeftShift:
						flag3 = shiftDowning;
						break;
					case KeyCode.RightControl:
					case KeyCode.LeftControl:
						flag3 = controlDowning;
						break;
					case KeyCode.RightAlt:
					case KeyCode.LeftAlt:
						flag3 = altDowning;
						break;
					default:
						flag3 = false;
						break;
					}
					if (!true)
					{
					}
					result = flag3;
				}
			}
			return result;
		}

		// Token: 0x040069AA RID: 27050
		[Header("轮盘根节点")]
		[SerializeField]
		private RectTransform wheelRoot;

		// Token: 0x040069AB RID: 27051
		[SerializeField]
		private Button closeButton;

		// Token: 0x040069AC RID: 27052
		[Header("中心区域")]
		[SerializeField]
		private CImage centerTypeIcon;

		// Token: 0x040069AD RID: 27053
		[SerializeField]
		private TextMeshProUGUI centerTypeText;

		// Token: 0x040069AE RID: 27054
		[SerializeField]
		private Button pagePrev;

		// Token: 0x040069AF RID: 27055
		[SerializeField]
		private Button pageNext;

		// Token: 0x040069B0 RID: 27056
		[SerializeField]
		private GameObject pagePrevContainer;

		// Token: 0x040069B1 RID: 27057
		[SerializeField]
		private GameObject pageNextContainer;

		// Token: 0x040069B2 RID: 27058
		[SerializeField]
		private TextMeshProUGUI prevEquipTypeHotkeyText;

		// Token: 0x040069B3 RID: 27059
		[SerializeField]
		private TextMeshProUGUI nextEquipTypeHotkeyText;

		// Token: 0x040069B4 RID: 27060
		[SerializeField]
		private TextMeshProUGUI prevPageHotkeyText;

		// Token: 0x040069B5 RID: 27061
		[SerializeField]
		private TextMeshProUGUI nextPageHotkeyText;

		// Token: 0x040069B6 RID: 27062
		[Header("装备类型切换按钮")]
		[SerializeField]
		private SkillWheelEquipTypeButton[] equipTypeButtons;

		// Token: 0x040069B7 RID: 27063
		[Header("功法格子")]
		[SerializeField]
		private CombatProactiveSkillView[] skillSlots;

		// Token: 0x040069B8 RID: 27064
		[Header("运转中功法中断器")]
		[SerializeField]
		private GameObject breakerAgile;

		// Token: 0x040069B9 RID: 27065
		[SerializeField]
		private GameObject breakerDefense;

		// Token: 0x040069BA RID: 27066
		[Header("功法说明面板")]
		[SerializeField]
		private GameObject infoPanel;

		// Token: 0x040069BB RID: 27067
		[SerializeField]
		private TextMeshProUGUI skillNameText;

		// Token: 0x040069BC RID: 27068
		[SerializeField]
		private RectTransform trickRequireHolder;

		// Token: 0x040069BD RID: 27069
		[SerializeField]
		private CombatTrickPrefab[] trickRequires;

		// Token: 0x040069BE RID: 27070
		[SerializeField]
		private CImage breathBar;

		// Token: 0x040069BF RID: 27071
		[SerializeField]
		private CImage breathPreviewBar;

		// Token: 0x040069C0 RID: 27072
		[SerializeField]
		private CImage stanceBar;

		// Token: 0x040069C1 RID: 27073
		[SerializeField]
		private CImage stancePreviewBar;

		// Token: 0x040069C2 RID: 27074
		[SerializeField]
		private CImage mobilityBar;

		// Token: 0x040069C3 RID: 27075
		[SerializeField]
		private CImage mobilityPreviewBar;

		// Token: 0x040069C4 RID: 27076
		public Action<short> OnCastSkill;

		// Token: 0x040069C5 RID: 27077
		public Action OnSkillWheelOpen;

		// Token: 0x040069C6 RID: 27078
		public Action OnSkillWheelClose;

		// Token: 0x040069C7 RID: 27079
		public Action<short> OnBreakAffectingSkill;

		// Token: 0x040069C8 RID: 27080
		private int _openFrameCount;

		// Token: 0x040069C9 RID: 27081
		private sbyte _currentCategory = -1;

		// Token: 0x040069CA RID: 27082
		private int _currentPage;

		// Token: 0x040069CB RID: 27083
		private readonly List<CombatSkillDisplayData> _filteredSkills = new List<CombatSkillDisplayData>();

		// Token: 0x040069CC RID: 27084
		private short _hoveredSkillId = -1;

		// Token: 0x040069CD RID: 27085
		private static readonly sbyte[] Categories = new sbyte[]
		{
			1,
			2,
			3
		};

		// Token: 0x040069CE RID: 27086
		private const int SkillsPerPage = 8;

		// Token: 0x040069CF RID: 27087
		private float _rightClickHoldTime;

		// Token: 0x040069D0 RID: 27088
		private bool _skillWheelOpened;

		// Token: 0x040069D1 RID: 27089
		private const float SkillWheelHoldThreshold = 0.3f;

		// Token: 0x040069D2 RID: 27090
		private bool _isShowingWheelSkillTip;

		// Token: 0x040069D3 RID: 27091
		private short _showingWheelSkillTipId = -1;

		// Token: 0x040069D4 RID: 27092
		private const float EstimatedTipWidth = 600f;

		// Token: 0x040069D5 RID: 27093
		private const float WheelTipOffsetX = 24f;

		// Token: 0x040069D6 RID: 27094
		private short _affectingAgileSkillId = -1;

		// Token: 0x040069D7 RID: 27095
		private short _affectingDefenseSkillId = -1;

		// Token: 0x040069D8 RID: 27096
		private Action _cachedEscHandler;
	}
}
