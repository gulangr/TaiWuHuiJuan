using System;
using FrameWork;
using GameData.Domains.CombatSkill;
using UnityEngine;
using UnityEngine.UI;

namespace UILogic.CharacterMenu.CharacterMenuEquipCombatSkill
{
	// Token: 0x020006B1 RID: 1713
	public class EquipCombatSkillSlot : MonoBehaviour
	{
		// Token: 0x06005008 RID: 20488 RVA: 0x00256BA0 File Offset: 0x00254DA0
		public void UpdateSlot(CombatSkillDisplayData skillData, short skillTemplateId, bool canOperate, bool isInEditingMode, bool isTaiwuTeamButNotBeast, sbyte neiliType, Action onActionMenuClick)
		{
			bool hasSkill = skillTemplateId >= 0;
			this.actionMenuButton.interactable = (skillData != null);
			int gridCount = 1;
			this.commonCombatSkill.gameObject.SetActive(hasSkill);
			this.addMark.SetActive(canOperate && !isInEditingMode);
			this.setSkillButton.interactable = (isTaiwuTeamButNotBeast & canOperate);
			bool flag = skillData != null;
			if (flag)
			{
				gridCount = (int)skillData.GridCount;
				this.commonCombatSkill.Refresh(skillData);
				this.commonCombatSkill.toggle.interactable = isTaiwuTeamButNotBeast;
				this.commonCombatSkill.Slot = gridCount;
				this.hoverArea.SetSprite(skillData.Mastered ? "ui_charactermenu_23_base_exercise_0" : "ui_charactermenu_23_base_exercise_1", false, null);
				this.actionMenuButton.ClearAndAddListener(delegate
				{
					bool canOperate2 = canOperate;
					if (canOperate2)
					{
						Action onActionMenuClick2 = onActionMenuClick;
						if (onActionMenuClick2 != null)
						{
							onActionMenuClick2();
						}
					}
				});
				this.commonCombatSkill.UpdateCounter(neiliType);
				this.lockObject.SetActive(false);
			}
			else
			{
				this.lockObject.SetActive(!canOperate);
				this.disablePanel.SetActive(false);
				this.commonCombatSkill.UpdateCounterVisible(false);
			}
			this.counterTipSync.SetActive(this.commonCombatSkill.counterObject.activeSelf);
			this.bonusTipSync.enabled = this.commonCombatSkill.bonusTip.enabled;
			bool enabled = this.commonCombatSkill.bonusTip.enabled;
			if (enabled)
			{
				this.bonusTipSync.RuntimeParam = this.commonCombatSkill.bonusTip.RuntimeParam;
			}
			this.layoutElement.preferredWidth = EquipCombatSkillSlot.SlotSize.x * (float)gridCount;
		}

		// Token: 0x06005009 RID: 20489 RVA: 0x00256D70 File Offset: 0x00254F70
		public void UpdateDisableInfo(CombatSkillDisplayData skillData, sbyte equipType, bool canOperate, bool isInEditingMode, Action onCloseClick, Action onJumpClick, Action onRemovePreview, Action onExitPreview)
		{
			bool disable = !skillData.CanAffect;
			this.disablePanel.SetActive(disable);
			CButtonObsolete button;
			bool flag = equipType != -1 && this.disablePanel.TryGetComponent<CButtonObsolete>(out button);
			if (flag)
			{
				bool flag2 = disable;
				if (flag2)
				{
					button.ClearAndAddListener(delegate
					{
						Action onCloseClick2 = onCloseClick;
						if (onCloseClick2 != null)
						{
							onCloseClick2();
						}
					});
				}
				else
				{
					button.onClick.RemoveAllListeners();
				}
			}
			this.disableCloseButton.gameObject.SetActive(true);
			this.disableCloseButton.ClearAndAddListener(delegate
			{
				Action onCloseClick2 = onCloseClick;
				if (onCloseClick2 != null)
				{
					onCloseClick2();
				}
			});
			this.disableJumpButton.ClearAndAddListener(delegate
			{
				Action onJumpClick2 = onJumpClick;
				if (onJumpClick2 != null)
				{
					onJumpClick2();
				}
			});
			this.disableJumpButton.gameObject.SetActive(canOperate && skillData.Conflicting);
			this.disablePointerTrigger.EnterEvent.RemoveAllListeners();
			this.disablePointerTrigger.EnterEvent.AddListener(delegate()
			{
				bool flag3 = !isInEditingMode || skillData.CanAffect;
				if (!flag3)
				{
					Action onRemovePreview2 = onRemovePreview;
					if (onRemovePreview2 != null)
					{
						onRemovePreview2();
					}
				}
			});
			this.disablePointerTrigger.ExitEvent.RemoveAllListeners();
			this.disablePointerTrigger.ExitEvent.AddListener(delegate()
			{
				bool flag3 = skillData.CanAffect || !this.disablePanel.activeInHierarchy;
				if (!flag3)
				{
					Action onExitPreview2 = onExitPreview;
					if (onExitPreview2 != null)
					{
						onExitPreview2();
					}
				}
			});
			this.UpdateDisableTip(skillData);
		}

		// Token: 0x0600500A RID: 20490 RVA: 0x00256EEC File Offset: 0x002550EC
		private void UpdateDisableTip(CombatSkillDisplayData skillData)
		{
			ArgumentBox argumentBox = this.disableTip.RuntimeParam ?? EasyPool.Get<ArgumentBox>();
			argumentBox.Clear();
			bool conflicting = skillData.Conflicting;
			if (conflicting)
			{
				argumentBox.Set("arg0", LocalStringManager.Get(LanguageKey.LK_CombatSkill_Equip_Invalidation));
				argumentBox.Set("arg1", LocalStringManager.Get(LanguageKey.LK_CombatSkill_Equip_Invalidation_Conflicting));
			}
			else
			{
				argumentBox.Set("arg0", LocalStringManager.Get(LanguageKey.LK_CombatSkill_Equip_Invalidation));
				argumentBox.Set("arg1", LocalStringManager.Get(LanguageKey.LK_CombatSkill_Equip_Invalidation_Slot_NotEnough));
			}
			this.disableTip.RuntimeParam = argumentBox;
		}

		// Token: 0x0600500B RID: 20491 RVA: 0x00256F8C File Offset: 0x0025518C
		public void Setup(sbyte equipType, int index, bool isInEditingMode, UI_CharacterMenuEquipCombatSkill.ShowEquippedSkillConfig? slotConfig, bool isDisablingPreviewParticle, bool equipped, Action onClickSlot, Action onClickDelete, Action onEnterPreviewAdd, Action onExitPreview, Action onEnterPreviewRemove, CombatSkillDisplayData skillData = null, Action<CombatSkillDisplayData> onMasteryChanged = null, Func<bool> isEditingModeGetter = null)
		{
			this.setSkillButton.ClearAndAddListener(onClickSlot);
			this.combatSkillButton.ClearAndAddListener(onClickSlot);
			this.deleteButton.ClearAndAddListener(onClickDelete);
			this._isEditingModeGetter = isEditingModeGetter;
			this.SetupMasteryButton(skillData, isInEditingMode, equipped, onMasteryChanged);
			PointerTrigger setSkillPointerTrigger = this.setSkillButton.GetComponent<PointerTrigger>();
			setSkillPointerTrigger.enabled = !equipped;
			setSkillPointerTrigger.EnterEvent.RemoveAllListeners();
			if (isInEditingMode)
			{
				setSkillPointerTrigger.EnterEvent.AddListener(delegate()
				{
					bool flag = slotConfig == null;
					if (!flag)
					{
						UI_CharacterMenuEquipCombatSkill.ShowEquippedSkillLineConfig line = slotConfig.Value.Lines[(int)equipType];
						bool flag2 = index < 0 || index >= line.SlotConfigs.Count;
						if (!flag2)
						{
							bool flag3 = !isDisablingPreviewParticle;
							if (flag3)
							{
								Action onEnterPreviewAdd2 = onEnterPreviewAdd;
								if (onEnterPreviewAdd2 != null)
								{
									onEnterPreviewAdd2();
								}
							}
						}
					}
				});
			}
			setSkillPointerTrigger.ExitEvent.RemoveAllListeners();
			if (isInEditingMode)
			{
				setSkillPointerTrigger.ExitEvent.AddListener(delegate()
				{
					Action onExitPreview2 = onExitPreview;
					if (onExitPreview2 != null)
					{
						onExitPreview2();
					}
				});
			}
			PointerTrigger pointerTrigger = this.combatSkillButton.GetComponent<PointerTrigger>();
			if (equipped)
			{
				pointerTrigger.EnterEvent.RemoveAllListeners();
				pointerTrigger.EnterEvent.AddListener(delegate()
				{
					this.RefreshEditingModeVisual();
					bool flag = !this.IsEditingModeNow();
					if (!flag)
					{
						Action onEnterPreviewRemove2 = onEnterPreviewRemove;
						if (onEnterPreviewRemove2 != null)
						{
							onEnterPreviewRemove2();
						}
					}
				});
				pointerTrigger.ExitEvent.RemoveAllListeners();
				pointerTrigger.ExitEvent.AddListener(delegate()
				{
					bool flag = !this.IsEditingModeNow();
					if (!flag)
					{
						Action onExitPreview2 = onExitPreview;
						if (onExitPreview2 != null)
						{
							onExitPreview2();
						}
					}
				});
			}
			else
			{
				pointerTrigger.EnterEvent.RemoveAllListeners();
				pointerTrigger.ExitEvent.RemoveAllListeners();
			}
		}

		// Token: 0x0600500C RID: 20492 RVA: 0x00257108 File Offset: 0x00255308
		private void SetupMasteryButton(CombatSkillDisplayData skillData, bool isInEditingMode, bool equipped, Action<CombatSkillDisplayData> onMasteryChanged)
		{
			bool flag = this.masteryButton == null;
			if (!flag)
			{
				bool flag2 = !isInEditingMode || !equipped || skillData == null;
				if (flag2)
				{
					this.masteryButton.gameObject.SetActive(false);
				}
				else
				{
					if (this._masteredHelper == null)
					{
						this._masteredHelper = new CombatSkillViewMasteredHelper();
					}
					this._masteredHelper.SetData(skillData);
					this._masteredHelper.OnMasteredStatusChanged = onMasteryChanged;
					bool canOperate = this._masteredHelper.CanOperate;
					this.masteryButton.gameObject.SetActive(canOperate);
					this.masteryButton.interactable = canOperate;
					bool flag3 = canOperate;
					if (flag3)
					{
						this.masteryButton.ClearAndAddListener(delegate
						{
							this._masteredHelper.OnClickChangeMastered();
						});
					}
				}
			}
		}

		// Token: 0x0600500D RID: 20493 RVA: 0x002571C8 File Offset: 0x002553C8
		public void OnMouseInOut(bool isIn)
		{
			if (isIn)
			{
				this.RefreshEditingModeVisual();
			}
			this.hoverArea.gameObject.SetActive(isIn);
		}

		// Token: 0x0600500E RID: 20494 RVA: 0x002571F6 File Offset: 0x002553F6
		public void OnHideAllGridHoverArea(bool isInEditingMode)
		{
			this.hoverArea.gameObject.SetActive(false);
			this.RefreshEditingModeVisual(isInEditingMode);
		}

		// Token: 0x0600500F RID: 20495 RVA: 0x00257213 File Offset: 0x00255413
		public void SetHoverAreaActive(bool active)
		{
			this.hoverArea.gameObject.SetActive(active);
		}

		// Token: 0x06005010 RID: 20496 RVA: 0x00257228 File Offset: 0x00255428
		public void RefreshDeleteButton(bool isIn, RectTransform viewport)
		{
			bool isInViewport = true;
			bool flag = viewport != null;
			if (flag)
			{
				Vector3[] viewportWorldCorners = new Vector3[4];
				viewport.GetWorldCorners(viewportWorldCorners);
				Vector3[] deleteButtonWorldCorners = new Vector3[4];
				RectTransform deleteRect = this.deleteButton.GetComponent<RectTransform>();
				deleteRect.GetWorldCorners(deleteButtonWorldCorners);
				float btnWidth = deleteButtonWorldCorners[2].x - deleteButtonWorldCorners[0].x;
				float halfBtnWidth = btnWidth * 0.5f;
				isInViewport = (deleteButtonWorldCorners[0].x >= viewportWorldCorners[0].x && deleteButtonWorldCorners[2].x <= viewportWorldCorners[2].x + halfBtnWidth);
			}
			this.deleteButton.gameObject.SetActive(isIn && isInViewport && this.commonCombatSkill.gameObject.activeSelf);
		}

		// Token: 0x06005011 RID: 20497 RVA: 0x00257300 File Offset: 0x00255500
		private bool IsEditingModeNow()
		{
			bool flag = this._isEditingModeGetter != null;
			return flag && this._isEditingModeGetter();
		}

		// Token: 0x06005012 RID: 20498 RVA: 0x00257330 File Offset: 0x00255530
		private void RefreshEditingModeVisual()
		{
			bool editing = this.IsEditingModeNow();
			this.editingMode.SetActive(editing);
			this.notEditingMode.SetActive(!editing);
		}

		// Token: 0x06005013 RID: 20499 RVA: 0x00257364 File Offset: 0x00255564
		private void RefreshEditingModeVisual(bool isEditingMode)
		{
			bool flag = this._isEditingModeGetter == null;
			if (flag)
			{
				this.editingMode.SetActive(isEditingMode);
				this.notEditingMode.SetActive(!isEditingMode);
			}
			else
			{
				this.RefreshEditingModeVisual();
			}
		}

		// Token: 0x04003728 RID: 14120
		[Header("Main UI References")]
		[SerializeField]
		private CommonCombatSkill commonCombatSkill;

		// Token: 0x04003729 RID: 14121
		[SerializeField]
		private CButtonObsolete combatSkillButton;

		// Token: 0x0400372A RID: 14122
		[SerializeField]
		private GameObject addMark;

		// Token: 0x0400372B RID: 14123
		[SerializeField]
		private CButtonObsolete actionMenuButton;

		// Token: 0x0400372C RID: 14124
		[SerializeField]
		private CButtonObsolete setSkillButton;

		// Token: 0x0400372D RID: 14125
		[SerializeField]
		private GameObject lockObject;

		// Token: 0x0400372E RID: 14126
		[SerializeField]
		private CImage hoverArea;

		// Token: 0x0400372F RID: 14127
		[SerializeField]
		private GameObject counterTipSync;

		// Token: 0x04003730 RID: 14128
		[SerializeField]
		private TooltipInvoker bonusTipSync;

		// Token: 0x04003731 RID: 14129
		[SerializeField]
		private LayoutElement layoutElement;

		// Token: 0x04003732 RID: 14130
		[Header("Disable Panel")]
		[SerializeField]
		private GameObject disablePanel;

		// Token: 0x04003733 RID: 14131
		[SerializeField]
		private CButtonObsolete disableCloseButton;

		// Token: 0x04003734 RID: 14132
		[SerializeField]
		private CButtonObsolete disableJumpButton;

		// Token: 0x04003735 RID: 14133
		[SerializeField]
		private PointerTrigger disablePointerTrigger;

		// Token: 0x04003736 RID: 14134
		[SerializeField]
		private TooltipInvoker disableTip;

		// Token: 0x04003737 RID: 14135
		[Header("Additional UI References")]
		[SerializeField]
		private CButtonObsolete deleteButton;

		// Token: 0x04003738 RID: 14136
		[SerializeField]
		private GameObject editingMode;

		// Token: 0x04003739 RID: 14137
		[SerializeField]
		private GameObject notEditingMode;

		// Token: 0x0400373A RID: 14138
		[SerializeField]
		private CButtonObsolete masteryButton;

		// Token: 0x0400373B RID: 14139
		private CombatSkillViewMasteredHelper _masteredHelper;

		// Token: 0x0400373C RID: 14140
		private Func<bool> _isEditingModeGetter;

		// Token: 0x0400373D RID: 14141
		private static readonly Vector2 SlotSize = new Vector2(186f, 200f);
	}
}
