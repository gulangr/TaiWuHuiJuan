using System;
using Config;
using DG.Tweening;
using DG.Tweening.Core;
using DG.Tweening.Plugins.Options;
using FrameWork;
using FrameWork.UISystem.UIElements;
using GameData.Domains.CombatSkill;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Game.Views.CharacterMenu
{
	// Token: 0x02000B79 RID: 2937
	public class EquipCombatSkillItem : MonoBehaviour
	{
		// Token: 0x17000FB9 RID: 4025
		// (get) Token: 0x060090F9 RID: 37113 RVA: 0x00439604 File Offset: 0x00437804
		public bool CanOperate
		{
			get
			{
				return this._combatSkillDisplayData != null && (this._combatSkillDisplayData.Mastered ? (this._combatSkillDisplayData.GridCount >= 1) : (this._combatSkillDisplayData.GridCount >= 2)) && (SingletonObject.getInstance<CharacterMonitorModel>().IsTaiwuTeamCharacter(this._combatSkillDisplayData.CharId) || SingletonObject.getInstance<CharacterMonitorModel>().IsTaiwuGearMate(this._combatSkillDisplayData.CharId)) && this._combatSkillDisplayData.TemplateId >= 0;
			}
		}

		// Token: 0x17000FBA RID: 4026
		// (get) Token: 0x060090FA RID: 37114 RVA: 0x0043968E File Offset: 0x0043788E
		protected bool ShouldHideEquippedLineSlotRegionSameAsDeleteButton
		{
			get
			{
				return this._currentType == EEquipCombatSkillItemType.EquippedLine && this._isHover && this._view != null && !this._view.IsInEditingMode;
			}
		}

		// Token: 0x17000FBB RID: 4027
		// (get) Token: 0x060090FB RID: 37115 RVA: 0x004396BF File Offset: 0x004378BF
		// (set) Token: 0x060090FC RID: 37116 RVA: 0x004396C7 File Offset: 0x004378C7
		public int Slot
		{
			get
			{
				return this.slot;
			}
			set
			{
				this.slot = value;
				this.RefreshWidth();
			}
		}

		// Token: 0x17000FBC RID: 4028
		// (get) Token: 0x060090FD RID: 37117 RVA: 0x004396D8 File Offset: 0x004378D8
		// (set) Token: 0x060090FE RID: 37118 RVA: 0x004396E0 File Offset: 0x004378E0
		public EEquipCombatSkillItemType CurrentType
		{
			get
			{
				return this._currentType;
			}
			set
			{
				this._currentType = value;
				this.RefreshByCurrentType();
			}
		}

		// Token: 0x17000FBD RID: 4029
		// (get) Token: 0x060090FF RID: 37119 RVA: 0x004396F1 File Offset: 0x004378F1
		protected CombatSkillDisplayDataCharacterMenuListItem CombatSkillDisplayData
		{
			get
			{
				return this._combatSkillDisplayData;
			}
		}

		// Token: 0x17000FBE RID: 4030
		// (get) Token: 0x06009100 RID: 37120 RVA: 0x004396F9 File Offset: 0x004378F9
		public int SkillTemplateId
		{
			get
			{
				CombatSkillDisplayDataCharacterMenuListItem combatSkillDisplayData = this._combatSkillDisplayData;
				return (int)((combatSkillDisplayData != null) ? combatSkillDisplayData.TemplateId : -1);
			}
		}

		// Token: 0x06009101 RID: 37121 RVA: 0x0043970D File Offset: 0x0043790D
		private void Awake()
		{
			this._view = UIElement.CharacterMenuEquipCombatSkill.UiBaseAs<ViewCharacterMenuEquipCombatSkill>();
		}

		// Token: 0x06009102 RID: 37122 RVA: 0x00439720 File Offset: 0x00437920
		private void OnDisable()
		{
			this._isHover = false;
			bool flag = this._view == null;
			if (!flag)
			{
				bool flag2 = this._view.TriggeredEquipCombatSkillItem != this;
				if (!flag2)
				{
					this.hover.gameObject.SetActive(false);
					this._view.TriggeredEquipCombatSkillItem = null;
					this._view.IsInPreview = false;
				}
			}
		}

		// Token: 0x06009103 RID: 37123 RVA: 0x0043978C File Offset: 0x0043798C
		public void RefreshWidth()
		{
			LayoutElement layoutElement = base.GetComponent<LayoutElement>();
			layoutElement.preferredWidth = (float)(184 * this.slot + 8 * (this.slot - 1));
		}

		// Token: 0x06009104 RID: 37124 RVA: 0x004397C0 File Offset: 0x004379C0
		private void RefreshSlotCountDisplay(int gridCount)
		{
			bool flag = this.masteredLayout != null && this.ShouldHideEquippedLineSlotRegionSameAsDeleteButton;
			if (flag)
			{
				this.SetMasteredLayoutActive(false);
				this.RefreshProvidedGridSlots();
			}
			else
			{
				bool flag2 = this.masteredLayout == null;
				if (flag2)
				{
					bool flag3 = this.slotCount != null;
					if (flag3)
					{
						this.slotCount.text = gridCount.ToString();
						this.slotCount.gameObject.SetActive(true);
					}
					this.RefreshProvidedGridSlots();
				}
				else
				{
					bool flag4 = this.slotCount != null;
					if (flag4)
					{
						this.slotCount.gameObject.SetActive(false);
					}
					this.SetMasteredLayoutActive(true);
					int count = Mathf.Max(0, gridCount);
					for (int i = 0; i < this.masteredLayout.childCount; i++)
					{
						this.masteredLayout.GetChild(i).gameObject.SetActive(i < count);
					}
					this.RefreshProvidedGridSlots();
				}
			}
		}

		// Token: 0x06009105 RID: 37125 RVA: 0x004398C8 File Offset: 0x00437AC8
		private void RefreshMasteredNeedUiFromCurrentState()
		{
			bool flag = this.masteredLayout == null;
			if (!flag)
			{
				bool flag2 = this._combatSkillDisplayData == null;
				if (!flag2)
				{
					this.RefreshSlotCountDisplay((int)this._combatSkillDisplayData.GridCount);
				}
			}
		}

		// Token: 0x06009106 RID: 37126 RVA: 0x0043990C File Offset: 0x00437B0C
		private void SetMasteredLayoutActive(bool active)
		{
			bool flag = this.masteredLayout != null;
			if (flag)
			{
				this.masteredLayout.gameObject.SetActive(active);
			}
		}

		// Token: 0x06009107 RID: 37127 RVA: 0x0043993C File Offset: 0x00437B3C
		private void RefreshProvidedGridSlots()
		{
			bool flag = this.masteredProvideSlotIcons == null || this.masteredProvideSlotIcons.Length == 0;
			if (!flag)
			{
				CombatSkillDisplayDataCharacterMenuListItem data = this.CombatSkillDisplayData;
				bool flag2 = data == null;
				if (flag2)
				{
					this.HideProvidedGridContainer();
				}
				else
				{
					CombatSkillItem config = CombatSkill.Instance[data.TemplateId];
					bool flag3 = config.EquipType != 0;
					if (flag3)
					{
						this.HideProvidedGridContainer();
					}
					else
					{
						sbyte[] grids = config.SpecificGrids;
						sbyte attack = (grids != null && grids.Length != 0) ? grids[0] : 0;
						sbyte agile = (grids != null && grids.Length > 1) ? grids[1] : 0;
						sbyte defence = (grids != null && grids.Length > 2) ? grids[2] : 0;
						sbyte special = (grids != null && grids.Length > 3) ? grids[3] : 0;
						sbyte generic = config.GenericGrid;
						int[] uiCounts = new int[]
						{
							Mathf.Max(0, (int)attack),
							Mathf.Max(0, (int)agile),
							Mathf.Max(0, (int)defence),
							Mathf.Max(0, (int)special),
							Mathf.Max(0, (int)generic)
						};
						bool any = false;
						foreach (int c in uiCounts)
						{
							bool flag4 = c > 0;
							if (flag4)
							{
								any = true;
							}
						}
						bool flag5 = this.masteredProvideLayoutRoot != null;
						if (flag5)
						{
							this.masteredProvideLayoutRoot.SetActive(any);
						}
						bool flag6 = !any;
						if (flag6)
						{
							foreach (GameObject go in this.masteredProvideSlotIcons)
							{
								bool flag7 = go != null;
								if (flag7)
								{
									go.SetActive(false);
								}
							}
						}
						else
						{
							int i = Mathf.Min(this.masteredProvideSlotIcons.Length, 5);
							for (int j = 0; j < i; j++)
							{
								GameObject go2 = this.masteredProvideSlotIcons[j];
								bool flag8 = go2 != null;
								if (flag8)
								{
									go2.SetActive(uiCounts[j] > 0);
								}
							}
							for (int k = i; k < this.masteredProvideSlotIcons.Length; k++)
							{
								GameObject go3 = this.masteredProvideSlotIcons[k];
								bool flag9 = go3 != null;
								if (flag9)
								{
									go3.SetActive(false);
								}
							}
						}
					}
				}
			}
		}

		// Token: 0x06009108 RID: 37128 RVA: 0x00439B78 File Offset: 0x00437D78
		private void HideProvidedGridContainer()
		{
			bool flag = this.masteredProvideLayoutRoot != null;
			if (flag)
			{
				this.masteredProvideLayoutRoot.SetActive(false);
			}
			bool flag2 = this.masteredProvideSlotIcons == null;
			if (!flag2)
			{
				foreach (GameObject go in this.masteredProvideSlotIcons)
				{
					bool flag3 = go != null;
					if (flag3)
					{
						go.SetActive(false);
					}
				}
			}
		}

		// Token: 0x06009109 RID: 37129 RVA: 0x00439BE8 File Offset: 0x00437DE8
		public void Set(ViewCharacterMenuEquipCombatSkill parentView, int index, CombatSkillDisplayDataCharacterMenuListItem combatSkillDisplayData, EEquipCombatSkillItemType eEquipCombatSkillItemType, bool previewCanAffect = true)
		{
			bool flag = combatSkillDisplayData == null;
			if (!flag)
			{
				bool flag2 = this._view == null;
				if (flag2)
				{
					this._view = UIElement.CharacterMenuEquipCombatSkill.UiBaseAs<ViewCharacterMenuEquipCombatSkill>();
				}
				this.Index = index;
				this._combatSkillDisplayData = combatSkillDisplayData;
				this._trySelectLineFromChildClick = null;
				this._suppressChildHover = null;
				this.bgShrink.gameObject.SetActive(combatSkillDisplayData.Mastered);
				this._canAffect = (combatSkillDisplayData.CanAffect && previewCanAffect);
				this.CurrentType = eEquipCombatSkillItemType;
				bool flag3 = this.CurrentType == EEquipCombatSkillItemType.EquippedLine;
				if (flag3)
				{
					this.Slot = (int)this._combatSkillDisplayData.GridCount;
				}
				else
				{
					this.Slot = 1;
				}
				this.RefreshSlotCountDisplay((int)this._combatSkillDisplayData.GridCount);
				CanvasGroup canvasGroup = base.gameObject.GetComponent<CanvasGroup>();
				bool flag4 = canvasGroup;
				if (flag4)
				{
					canvasGroup.alpha = 1f;
				}
				this.RefreshInvalidationTip();
				bool isHover = this._isHover;
				if (isHover)
				{
					this.RefreshHoverState();
				}
			}
		}

		// Token: 0x0600910A RID: 37130 RVA: 0x00439CEA File Offset: 0x00437EEA
		public void SetLineSelectHandler(Func<bool> trySelectLineFromChildClick)
		{
			this._trySelectLineFromChildClick = trySelectLineFromChildClick;
		}

		// Token: 0x0600910B RID: 37131 RVA: 0x00439CF4 File Offset: 0x00437EF4
		public void AttachLineHoverCallbacks(Action onEnter, Action onExit)
		{
			bool flag = this.pointerTrigger == null;
			if (!flag)
			{
				bool flag2 = onEnter != null;
				if (flag2)
				{
					this.pointerTrigger.EnterEvent.AddListener(delegate()
					{
						onEnter();
					});
				}
				bool flag3 = onExit != null;
				if (flag3)
				{
					this.pointerTrigger.ExitEvent.AddListener(delegate()
					{
						onExit();
					});
				}
			}
		}

		// Token: 0x0600910C RID: 37132 RVA: 0x00439D7C File Offset: 0x00437F7C
		public void SetChildHoverSuppressed(Func<bool> suppressChildHover)
		{
			this._suppressChildHover = suppressChildHover;
		}

		// Token: 0x0600910D RID: 37133 RVA: 0x00439D88 File Offset: 0x00437F88
		public void CancelEquippedLineHover()
		{
			bool flag = this._currentType > EEquipCombatSkillItemType.EquippedLine;
			if (!flag)
			{
				this._isHover = false;
				this.HideEquippedLineHover();
			}
		}

		// Token: 0x17000FBF RID: 4031
		// (get) Token: 0x0600910E RID: 37134 RVA: 0x00439DB3 File Offset: 0x00437FB3
		private bool ShouldSuppressChildHover
		{
			get
			{
				Func<bool> suppressChildHover = this._suppressChildHover;
				return suppressChildHover != null && suppressChildHover();
			}
		}

		// Token: 0x0600910F RID: 37135 RVA: 0x00439DC8 File Offset: 0x00437FC8
		private void RefreshHoverState()
		{
			switch (this._currentType)
			{
			case EEquipCombatSkillItemType.EquippedLine:
				this.ShowEquippedLineHover();
				break;
			case EEquipCombatSkillItemType.ScrollEquipped:
				this.ShowScrollEquippedHover();
				break;
			case EEquipCombatSkillItemType.ScrollUnEquipped:
				this.ShowScrollUnEquippedHover();
				break;
			case EEquipCombatSkillItemType.SortPanel:
				this.ShowSortPanelHover();
				break;
			case EEquipCombatSkillItemType.FavoriteMode:
				this.ShowFavoriteModeHover();
				break;
			}
			bool flag = this._view.IsInPreview && this.CurrentType == EEquipCombatSkillItemType.EquippedLine;
			if (flag)
			{
				bool flag2 = this._view.TriggeredEquipCombatSkillItem.SkillTemplateId == this.SkillTemplateId;
				if (flag2)
				{
					this.FadeInOrOut(true, null);
				}
			}
		}

		// Token: 0x06009110 RID: 37136 RVA: 0x00439E6C File Offset: 0x0043806C
		private void HideIcons()
		{
			this.hover.gameObject.SetActive(false);
			this.mask.transform.gameObject.SetActive(false);
			this.hSVStyleRoot.SetDefault();
			this.prompt.gameObject.SetActive(false);
			this.remove.transform.gameObject.SetActive(false);
			this.masteredButton.gameObject.SetActive(false);
			this.deleteButton.transform.gameObject.SetActive(false);
			this.replace.gameObject.SetActive(false);
			this.favoriteIcon.gameObject.SetActive(false);
			bool flag = this.conflictJumpButton != null;
			if (flag)
			{
				this.conflictJumpButton.gameObject.SetActive(false);
			}
		}

		// Token: 0x06009111 RID: 37137 RVA: 0x00439F48 File Offset: 0x00438148
		private void RefreshByCurrentType()
		{
			this.HideIcons();
			this.pointerTrigger.enabled = this._view.CanOperate;
			this.combatSkillButton.enabled = this._view.CanOperate;
			switch (this._currentType)
			{
			case EEquipCombatSkillItemType.EquippedLine:
				this.HandleEquippedLineState();
				break;
			case EEquipCombatSkillItemType.ScrollEquipped:
				this.HandleScrollEquippedState();
				break;
			case EEquipCombatSkillItemType.ScrollUnEquipped:
				this.HandleScrollUnEquippedState();
				break;
			case EEquipCombatSkillItemType.SortPanel:
				this.HandleSortPanel();
				break;
			case EEquipCombatSkillItemType.FavoriteMode:
				this.HandleFavoriteMode();
				break;
			}
		}

		// Token: 0x06009112 RID: 37138 RVA: 0x00439FE0 File Offset: 0x004381E0
		private void HandleEquippedLineState()
		{
			this.mask.gameObject.SetActive(!this._canAffect);
			this.prompt.gameObject.SetActive(!this._canAffect);
			bool canAffect = this._canAffect;
			if (canAffect)
			{
				this.hSVStyleRoot.SetDefault();
			}
			else
			{
				this.hSVStyleRoot.SetValueFactor(0.6f);
			}
			this.pointerTrigger.EnterEvent.RemoveAllListeners();
			this.pointerTrigger.EnterEvent.AddListener(delegate()
			{
				bool shouldSuppressChildHover = this.ShouldSuppressChildHover;
				if (!shouldSuppressChildHover)
				{
					this._isHover = true;
					this.ShowEquippedLineHover();
				}
			});
			this.pointerTrigger.ExitEvent.RemoveAllListeners();
			this.pointerTrigger.ExitEvent.AddListener(delegate()
			{
				this._isHover = false;
				this.HideEquippedLineHover();
			});
			this.settingButton.ClearAndAddListener(new Action(this.OpenCombatSkillPanel));
			this.masteredButton.ClearAndAddListener(delegate
			{
				this._view.MasteredEquippedCombatSkill(this._combatSkillDisplayData.TemplateId, this._combatSkillDisplayData.Mastered);
			});
			this.deleteButton.ClearAndAddListener(delegate
			{
				this.FadeInOrOut(false, delegate
				{
					this._view.RemoveEquippedCombatSkill(this._combatSkillDisplayData.TemplateId);
				});
			});
			this.combatSkillButton.ClearAndAddListener(delegate
			{
				Func<bool> trySelectLineFromChildClick = this._trySelectLineFromChildClick;
				bool flag = trySelectLineFromChildClick != null && trySelectLineFromChildClick();
				if (!flag)
				{
					bool flag2 = !this._view.IsInEditingMode;
					if (flag2)
					{
						this._view.IsInEditingMode = true;
					}
					else
					{
						this.FadeInOrOut(false, delegate
						{
							this._view.RemoveEquippedCombatSkill(this._combatSkillDisplayData.TemplateId);
						});
					}
					CombatSkillItem config = CombatSkill.Instance[this._combatSkillDisplayData.TemplateId];
					this._view.SetCombatSkillFilterType(config.EquipType);
				}
			});
			this.RefreshConflictJumpButton();
		}

		// Token: 0x06009113 RID: 37139 RVA: 0x0043A114 File Offset: 0x00438314
		private void ShowEquippedLineHover()
		{
			this.hover.gameObject.SetActive(this._canAffect);
			this.mask.transform.gameObject.SetActive(true);
			bool currentCharacterIsTaiwu = this._view.CharacterMenu.CurrentCharacterIsTaiwu;
			if (currentCharacterIsTaiwu)
			{
				this.settingButton.transform.gameObject.SetActive(true);
			}
			this.hSVStyleRoot.SetValueFactor(0.6f);
			this.remove.transform.gameObject.SetActive(this._view.IsInEditingMode);
			this.masteredButton.gameObject.SetActive(this._view.IsInEditingMode && this.CanOperate);
			this.deleteButton.transform.gameObject.SetActive(!this._view.IsInEditingMode);
			this.replace.gameObject.SetActive(!this._view.IsInEditingMode && this._canAffect);
			this.RefreshMasteredNeedUiFromCurrentState();
		}

		// Token: 0x06009114 RID: 37140 RVA: 0x0043A228 File Offset: 0x00438428
		private void HideEquippedLineHover()
		{
			this.hover.gameObject.SetActive(false);
			this.mask.transform.gameObject.SetActive(!this._canAffect);
			this.settingButton.transform.gameObject.SetActive(false);
			this.hSVStyleRoot.SetDefault();
			this.remove.transform.gameObject.SetActive(false);
			this.masteredButton.gameObject.SetActive(false);
			this.deleteButton.transform.gameObject.SetActive(false);
			this.replace.gameObject.SetActive(false);
			this.RefreshMasteredNeedUiFromCurrentState();
		}

		// Token: 0x06009115 RID: 37141 RVA: 0x0043A2E4 File Offset: 0x004384E4
		private void SetupMasteredButton()
		{
			this.masteredButton.ClearAndAddListener(delegate
			{
				this._view.MasteredEquippedCombatSkill(this._combatSkillDisplayData.TemplateId, this._combatSkillDisplayData.Mastered);
			});
			this.masteredPointerTrigger.EnterEvent.ResetListener(delegate()
			{
				bool flag = this._currentType == EEquipCombatSkillItemType.ScrollEquipped;
				if (flag)
				{
					this._view.GetMasteredPreview(this._combatSkillDisplayData.TemplateId, this._combatSkillDisplayData.Mastered, true);
				}
			});
			this.masteredPointerTrigger.ExitEvent.ResetListener(delegate()
			{
				bool flag = this._currentType == EEquipCombatSkillItemType.ScrollEquipped;
				if (flag)
				{
					this._view.GetMasteredPreview(this._combatSkillDisplayData.TemplateId, this._combatSkillDisplayData.Mastered, false);
				}
			});
		}

		// Token: 0x06009116 RID: 37142 RVA: 0x0043A344 File Offset: 0x00438544
		private void HandleScrollEquippedState()
		{
			this.SetupMasteredButton();
			this.pointerTrigger.EnterEvent.RemoveAllListeners();
			this.pointerTrigger.EnterEvent.AddListener(delegate()
			{
				this._isHover = true;
				this.ShowScrollEquippedHover();
			});
			this.pointerTrigger.ExitEvent.RemoveAllListeners();
			this.pointerTrigger.ExitEvent.AddListener(delegate()
			{
				this._isHover = false;
				this.HideScrollEquippedHover();
			});
			this.settingButton.ClearAndAddListener(new Action(this.OpenCombatSkillPanel));
			this.combatSkillButton.ClearAndAddListener(delegate
			{
				this._view.RemoveEquippedCombatSkill(this._combatSkillDisplayData.TemplateId);
			});
		}

		// Token: 0x06009117 RID: 37143 RVA: 0x0043A3E8 File Offset: 0x004385E8
		private void ShowScrollEquippedHover()
		{
			bool currentCharacterIsTaiwu = this._view.CharacterMenu.CurrentCharacterIsTaiwu;
			if (currentCharacterIsTaiwu)
			{
				this.settingButton.transform.gameObject.SetActive(true);
			}
			this.hover.gameObject.SetActive(true);
			this.masteredButton.gameObject.SetActive(this.CanOperate);
		}

		// Token: 0x06009118 RID: 37144 RVA: 0x0043A44A File Offset: 0x0043864A
		private void HideScrollEquippedHover()
		{
			this.settingButton.transform.gameObject.SetActive(false);
			this.hover.gameObject.SetActive(false);
			this.masteredButton.gameObject.SetActive(false);
		}

		// Token: 0x06009119 RID: 37145 RVA: 0x0043A488 File Offset: 0x00438688
		private void HandleScrollUnEquippedState()
		{
			this.SetupMasteredButton();
			bool isRevoked = this._combatSkillDisplayData.Revoked;
			bool isRevoked3 = isRevoked;
			if (isRevoked3)
			{
				this.mask.gameObject.SetActive(true);
				this.prompt.gameObject.SetActive(true);
			}
			this.pointerTrigger.EnterEvent.RemoveAllListeners();
			this.pointerTrigger.EnterEvent.AddListener(delegate()
			{
				this._isHover = true;
				this.ShowScrollUnEquippedHover();
			});
			this.pointerTrigger.ExitEvent.RemoveAllListeners();
			this.pointerTrigger.ExitEvent.AddListener(delegate()
			{
				this._isHover = false;
				this.HideScrollUnEquippedHover();
			});
			this.combatSkillButton.ClearAndAddListener(delegate
			{
				bool isRevoked2 = isRevoked;
				if (!isRevoked2)
				{
					this._view.AddEquippedCombatSkill(this._combatSkillDisplayData);
					bool flag = !this._view.IsInEditingMode;
					if (flag)
					{
						this._view.IsInEditingMode = true;
					}
				}
			});
			this.settingButton.ClearAndAddListener(delegate
			{
				bool isRevoked2 = isRevoked;
				if (!isRevoked2)
				{
					this.OpenCombatSkillPanel();
				}
			});
		}

		// Token: 0x0600911A RID: 37146 RVA: 0x0043A578 File Offset: 0x00438778
		private void ShowScrollUnEquippedHover()
		{
			bool revoked = this._combatSkillDisplayData.Revoked;
			if (!revoked)
			{
				bool currentCharacterIsTaiwu = this._view.CharacterMenu.CurrentCharacterIsTaiwu;
				if (currentCharacterIsTaiwu)
				{
					this.settingButton.transform.gameObject.SetActive(true);
				}
				this.hover.gameObject.SetActive(true);
				this.masteredButton.gameObject.SetActive(this.CanOperate);
				this._view.TriggeredEquipCombatSkillItem = this;
				this._view.IsInPreview = true;
				this._view.CombatSkillItemPreview(this._combatSkillDisplayData, this._currentType);
			}
		}

		// Token: 0x0600911B RID: 37147 RVA: 0x0043A620 File Offset: 0x00438820
		private void HideScrollUnEquippedHover()
		{
			bool revoked = this._combatSkillDisplayData.Revoked;
			if (!revoked)
			{
				this.settingButton.transform.gameObject.SetActive(false);
				this.hover.gameObject.SetActive(false);
				this.masteredButton.gameObject.SetActive(false);
				bool flag = this._view.TriggeredEquipCombatSkillItem == this;
				if (flag)
				{
					this._view.IsInPreview = false;
					this._view.TriggeredEquipCombatSkillItem = null;
				}
			}
		}

		// Token: 0x0600911C RID: 37148 RVA: 0x0043A6AC File Offset: 0x004388AC
		private void HandleSortPanel()
		{
			this.pointerTrigger.EnterEvent.RemoveAllListeners();
			this.pointerTrigger.EnterEvent.AddListener(delegate()
			{
				this._isHover = true;
				this.ShowSortPanelHover();
			});
			this.pointerTrigger.ExitEvent.RemoveAllListeners();
			this.pointerTrigger.ExitEvent.AddListener(delegate()
			{
				this._isHover = false;
				this.HideSortPanelHover();
			});
			this.combatSkillButton.ClearAndAddListener(delegate
			{
				int index = this._view.GetIndexInSkillOrderPlan((int)this._combatSkillDisplayData.TemplateId);
				this._view.SetTargetSkillIndexTuple(index);
			});
		}

		// Token: 0x0600911D RID: 37149 RVA: 0x0043A72E File Offset: 0x0043892E
		private void ShowSortPanelHover()
		{
			this.hover.gameObject.SetActive(true);
		}

		// Token: 0x0600911E RID: 37150 RVA: 0x0043A744 File Offset: 0x00438944
		private void HideSortPanelHover()
		{
			int index = this._view.GetIndexInSkillOrderPlan((int)this._combatSkillDisplayData.TemplateId);
			this.hover.gameObject.SetActive(this._view.TargetSkillIndexTuple.Item1 == index);
		}

		// Token: 0x0600911F RID: 37151 RVA: 0x0043A790 File Offset: 0x00438990
		private void HandleFavoriteMode()
		{
			this.pointerTrigger.EnterEvent.ResetListener(delegate()
			{
				this._isHover = true;
				this.ShowFavoriteModeHover();
			});
			this.pointerTrigger.ExitEvent.ResetListener(delegate()
			{
				this._isHover = false;
				this.HideFavoriteModeHover();
			});
			this.favoriteIcon.gameObject.SetActive(this._combatSkillDisplayData.IsFavorite);
			this.combatSkillButton.ClearAndAddListener(delegate
			{
				this._view.FavoriteCombatSkill(this._combatSkillDisplayData.TemplateId, !this._combatSkillDisplayData.IsFavorite);
			});
		}

		// Token: 0x06009120 RID: 37152 RVA: 0x0043A80C File Offset: 0x00438A0C
		private void ShowFavoriteModeHover()
		{
			this.hover.gameObject.SetActive(true);
		}

		// Token: 0x06009121 RID: 37153 RVA: 0x0043A821 File Offset: 0x00438A21
		private void HideFavoriteModeHover()
		{
			this.hover.gameObject.SetActive(false);
		}

		// Token: 0x06009122 RID: 37154 RVA: 0x0043A838 File Offset: 0x00438A38
		private void OpenCombatSkillPanel()
		{
			ArgumentBox argBox = EasyPool.Get<ArgumentBox>();
			argBox.Set("CombatSkillId", this._combatSkillDisplayData.TemplateId);
			UIElement.CombatSkillPanel.OnHide = delegate()
			{
				this._view.RequestData();
				bool isInEditingMode = this._view.IsInEditingMode;
				if (isInEditingMode)
				{
					UIManager.Instance.SetEscHandler(new Action(this._view.ExitCurrentMode));
				}
			};
			UIElement.CombatSkillPanel.SetOnInitArgs(argBox);
			UIManager.Instance.SetEscHandler(null);
			UIManager.Instance.MaskUI(UIElement.CombatSkillPanel);
		}

		// Token: 0x06009123 RID: 37155 RVA: 0x0043A8A4 File Offset: 0x00438AA4
		private void RefreshInvalidationTip()
		{
			TooltipInvoker tip = base.GetComponent<TooltipInvoker>();
			tip.Type = (this._canAffect ? TipType.CombatSkill : TipType.Simple);
			bool canAffect = this._canAffect;
			if (!canAffect)
			{
				TooltipInvoker tooltipInvoker = tip;
				if (tooltipInvoker.RuntimeParam == null)
				{
					tooltipInvoker.RuntimeParam = EasyPool.Get<ArgumentBox>();
				}
				tip.RuntimeParam.Set("arg0", LanguageKey.LK_CombatSkill_Equip_Invalidation.Tr());
				tip.RuntimeParam.Set("arg1", (this._combatSkillDisplayData != null && this._combatSkillDisplayData.Conflicting) ? LanguageKey.LK_CombatSkill_Equip_Invalidation_Conflicting.Tr() : LanguageKey.LK_CombatSkill_Equip_Invalidation_Slot_NotEnough.Tr());
			}
		}

		// Token: 0x06009124 RID: 37156 RVA: 0x0043A948 File Offset: 0x00438B48
		private void RefreshConflictJumpButton()
		{
			bool flag = this.conflictJumpButton == null;
			if (!flag)
			{
				bool show = this._currentType == EEquipCombatSkillItemType.EquippedLine && this._combatSkillDisplayData != null && this._combatSkillDisplayData.Conflicting && this._view != null && this._view.CanOperate;
				this.conflictJumpButton.gameObject.SetActive(show);
				bool flag2 = show;
				if (flag2)
				{
					this.conflictJumpButton.ClearAndAddListener(new Action(this.OpenPracticeForConflict));
				}
			}
		}

		// Token: 0x06009125 RID: 37157 RVA: 0x0043A9D4 File Offset: 0x00438BD4
		private void OpenPracticeForConflict()
		{
			bool flag = this._combatSkillDisplayData == null || this._view == null;
			if (!flag)
			{
				this._view.ExitCurrentMode();
				short templateId = this._combatSkillDisplayData.TemplateId;
				ViewCharacterMenuPractice uiPractice = UIElement.CharacterMenuPractice.UiBaseAs<ViewCharacterMenuPractice>();
				bool flag2 = uiPractice.gameObject.activeSelf && this._view.CharacterMenu.CurrentCharacterIsTaiwu;
				if (flag2)
				{
					uiPractice.SetSkill(templateId, true);
				}
				else
				{
					uiPractice.PresetSkill(templateId);
				}
				ArgumentBox pageArgs = EasyPool.Get<ArgumentBox>();
				pageArgs.SetObject("TargetPageIndex", ECharacterSubToggleBase.PracticeBase);
				GEvent.OnEvent(UiEvents.OnNeedOpenCharacterMenuSubPage, pageArgs);
			}
		}

		// Token: 0x06009126 RID: 37158 RVA: 0x0043AA88 File Offset: 0x00438C88
		public void FadeInOrOut(bool isIn, Action callback = null)
		{
			CanvasGroup canvasGroup = base.gameObject.GetComponent<CanvasGroup>();
			bool flag = canvasGroup == null;
			if (flag)
			{
				canvasGroup = base.gameObject.AddComponent<CanvasGroup>();
			}
			canvasGroup.alpha = (float)(isIn ? 0 : 1);
			canvasGroup.DOFade((float)(isIn ? 1 : 0), 0.35f).OnComplete(delegate
			{
				Action callback2 = callback;
				if (callback2 != null)
				{
					callback2();
				}
			}).SetEase(Ease.InCubic).SetAutoKill<TweenerCore<float, float, FloatOptions>>();
		}

		// Token: 0x04006FC0 RID: 28608
		[SerializeField]
		[Range(1f, 3f)]
		[Header("功法格子数")]
		private int slot = 1;

		// Token: 0x04006FC1 RID: 28609
		public CharacterMenuCombatSkillItem combatSkillItem;

		// Token: 0x04006FC2 RID: 28610
		[SerializeField]
		private CImage mask;

		// Token: 0x04006FC3 RID: 28611
		[SerializeField]
		private CButton settingButton;

		// Token: 0x04006FC4 RID: 28612
		[SerializeField]
		private CButton deleteButton;

		// Token: 0x04006FC5 RID: 28613
		[SerializeField]
		private CButton masteredButton;

		// Token: 0x04006FC6 RID: 28614
		[SerializeField]
		private PointerTrigger masteredPointerTrigger;

		// Token: 0x04006FC7 RID: 28615
		[SerializeField]
		private CImage replace;

		// Token: 0x04006FC8 RID: 28616
		[SerializeField]
		private CImage remove;

		// Token: 0x04006FC9 RID: 28617
		[SerializeField]
		private CImage prompt;

		// Token: 0x04006FCA RID: 28618
		[SerializeField]
		private CButton conflictJumpButton;

		// Token: 0x04006FCB RID: 28619
		[SerializeField]
		private PointerTrigger pointerTrigger;

		// Token: 0x04006FCC RID: 28620
		[SerializeField]
		private CButton combatSkillButton;

		// Token: 0x04006FCD RID: 28621
		[SerializeField]
		private HSVStyleRoot hSVStyleRoot;

		// Token: 0x04006FCE RID: 28622
		[SerializeField]
		private CImage hover;

		// Token: 0x04006FCF RID: 28623
		[SerializeField]
		[Tooltip("未配置 masteredLayout 时用作格数数字显示")]
		protected TextMeshProUGUI slotCount;

		// Token: 0x04006FD0 RID: 28624
		[SerializeField]
		private CImage favoriteIcon;

		// Token: 0x04006FD1 RID: 28625
		[SerializeField]
		private CImage bgShrink;

		// Token: 0x04006FD2 RID: 28626
		[Header("占用格（替代 slotCount 数字）")]
		[Tooltip("带 GridLayoutGroup 的父节点，子物体顺序对应第 1、2、3… 格；与删除互斥时仅隐藏本节点。精通按钮请单独赋给 masteredButton。")]
		[SerializeField]
		private RectTransform masteredLayout;

		// Token: 0x04006FD3 RID: 28627
		[Header("提供格（内功）")]
		[SerializeField]
		private GameObject masteredProvideLayoutRoot;

		// Token: 0x04006FD4 RID: 28628
		[Tooltip("顺序：摧破·轻灵·护体·奇窍·万用（对应 SpecificGrids[0..3] 与 GenericGrid）；格数>0 才 SetActive")]
		[SerializeField]
		private GameObject[] masteredProvideSlotIcons;

		// Token: 0x04006FD5 RID: 28629
		private ViewCharacterMenuEquipCombatSkill _view;

		// Token: 0x04006FD6 RID: 28630
		private bool _isHover;

		// Token: 0x04006FD7 RID: 28631
		private EEquipCombatSkillItemType _currentType;

		// Token: 0x04006FD8 RID: 28632
		private CombatSkillDisplayDataCharacterMenuListItem _combatSkillDisplayData;

		// Token: 0x04006FD9 RID: 28633
		private bool _canAffect;

		// Token: 0x04006FDA RID: 28634
		private Func<bool> _trySelectLineFromChildClick;

		// Token: 0x04006FDB RID: 28635
		private Func<bool> _suppressChildHover;

		// Token: 0x04006FDC RID: 28636
		public int Index = -1;
	}
}
