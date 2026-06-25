using System;
using System.Collections.Generic;
using FrameWork;
using FrameWork.UISystem.UIElements;
using Game.Components.Character;
using Game.Views.Migrate;
using GameData.Domains.CombatSkill;
using GameData.Domains.Global;
using GameData.Domains.Taiwu;
using GameData.Serializer;
using GameData.Utilities;
using UnityEngine;
using UnityEngine.Events;

namespace Game.Views.CharacterMenu
{
	// Token: 0x02000BA0 RID: 2976
	public class ViewCharacterMenuCombatSkill : UI_CharacterMenuSubPageBase
	{
		// Token: 0x06009389 RID: 37769 RVA: 0x0044BEE8 File Offset: 0x0044A0E8
		public override bool CheckState(ECharacterSubToggleBase curSubTogglePage, ECharacterSubPage curSubPage)
		{
			return curSubTogglePage == ECharacterSubToggleBase.AttainmentBase;
		}

		// Token: 0x17000FE9 RID: 4073
		// (get) Token: 0x0600938A RID: 37770 RVA: 0x0044BEFE File Offset: 0x0044A0FE
		public override LanguageKey TitleKey
		{
			get
			{
				return LanguageKey.LK_CombatSkill;
			}
		}

		// Token: 0x0600938B RID: 37771 RVA: 0x0044BF08 File Offset: 0x0044A108
		public override void OnInit(ArgumentBox argsBox)
		{
			this.localLoadingAnim.SetLoadingEvent(delegate
			{
				this._currSelectedSkillId = -1;
				this._initSelectedSkillId = true;
				this.detailSkillScroll.SetCombatSkillList(new List<CombatSkillDisplayDataCharacterMenuListItem>(), true, true, null, new Action<CombatSkillDisplayDataCharacterMenuListItem, CharacterMenuCombatSkillItem>(this.OnRenderSkill), false, false, null, true);
				this.noContentObj.SetActive(false);
				this.selectSkillInfo.SetLoadingState(true);
			}, new Action(this.RefreshByCachedData));
			UIElement element = this.Element;
			element.OnListenerIdReady = (Action)Delegate.Combine(element.OnListenerIdReady, new Action(delegate()
			{
				this.localLoadingAnim.SetLoadingState(true);
				this.GetSkillListData();
			}));
		}

		// Token: 0x0600938C RID: 37772 RVA: 0x0044BF61 File Offset: 0x0044A161
		private void GetSkillListData()
		{
			CombatSkillDomainMethod.AsyncCall.GetCharacterMenuCombatSkillListItemDisplayData(null, base.CharacterMenu.CurCharacterId, delegate(int offset, RawDataPool pool)
			{
				Serializer.Deserialize(pool, offset, ref this._cachedData);
				bool initSelectedSkillId = this._initSelectedSkillId;
				if (initSelectedSkillId)
				{
					this.SetSkillInfo((this._cachedData.Count > 0) ? this._cachedData[0] : null);
					this._initSelectedSkillId = false;
				}
				this.localLoadingAnim.SetLoadingState(false);
				bool flag = !this.Element.Ready;
				if (flag)
				{
					this.Element.ShowAfterRefresh();
				}
			});
		}

		// Token: 0x0600938D RID: 37773 RVA: 0x0044BF82 File Offset: 0x0044A182
		private void RefreshByCachedData()
		{
			this.UpdateDetailCombatSkillList();
		}

		// Token: 0x0600938E RID: 37774 RVA: 0x0044BF8C File Offset: 0x0044A18C
		private void Awake()
		{
			this.Init();
		}

		// Token: 0x0600938F RID: 37775 RVA: 0x0044BF98 File Offset: 0x0044A198
		public void Init()
		{
			bool flag = this.inited;
			if (!flag)
			{
				this.togFavorMode.onValueChanged.AddListener(new UnityAction<bool>(this.OnClickFavorMode));
				this.detailSkillScroll.Init();
				this.detailSkillScroll.SetCombatSkillList(this._cachedData, true, true, "charmenu_combatskill_detail", null, false, false, null, true);
				this.inited = true;
			}
		}

		// Token: 0x06009390 RID: 37776 RVA: 0x0044C000 File Offset: 0x0044A200
		private void OnClickFavorMode(bool inFavorMode)
		{
			this._favorMode = inFavorMode;
			this.OnFavoriteModeChanged();
		}

		// Token: 0x06009391 RID: 37777 RVA: 0x0044C014 File Offset: 0x0044A214
		private void OnFavoriteModeChanged()
		{
			bool flag = !base.CharacterMenu.CanOperate;
			if (flag)
			{
				this._favorMode = false;
			}
			bool favorMode = this._favorMode;
			if (favorMode)
			{
				UIManager.Instance.MaskComponent(this.favorModeRoot);
			}
			else
			{
				UIManager.Instance.UnMaskComponent(this.favorModeRoot);
			}
			this.favorModeRoot.gameObject.SetActive(true);
			this.togFavorMode.gameObject.SetActive(base.CharacterMenu.CurrentCharacterIsTaiwu && base.CharacterMenu.CanOperate);
			this.togFavorMode.SetIsOnWithoutNotify(this._favorMode);
			this.RefreshByCachedData();
			bool favorMode2 = this._favorMode;
			if (favorMode2)
			{
				UIManager.Instance.SetEscHandler(new Action(this.CheckEscClose));
			}
			else
			{
				UIManager.Instance.SetEscHandler(null);
			}
		}

		// Token: 0x06009392 RID: 37778 RVA: 0x0044C0F0 File Offset: 0x0044A2F0
		public void FavoriteCombatSkill(short skillId, bool isFavorite)
		{
			if (isFavorite)
			{
				TaiwuDomainMethod.Call.AddFavoriteCombatSkill(skillId);
			}
			else
			{
				TaiwuDomainMethod.Call.RemoveFavoriteCombatSkill(skillId);
			}
			this.GetSkillListData();
		}

		// Token: 0x06009393 RID: 37779 RVA: 0x0044C11E File Offset: 0x0044A31E
		private void OnEnable()
		{
			this.RefreshByCachedData();
			GlobalDomainMethod.Call.InvokeGuidingTrigger(109);
			GlobalDomainMethod.Call.InvokeGuidingTrigger(141);
		}

		// Token: 0x06009394 RID: 37780 RVA: 0x0044C13C File Offset: 0x0044A33C
		private void SetEscHandler(bool active)
		{
			bool flag = !active && !UIManager.Instance.CheckEscHandler(new Action(this.CheckEscClose));
			if (!flag)
			{
				UIManager.Instance.SetEscHandler(active ? new Action(this.CheckEscClose) : null);
			}
		}

		// Token: 0x06009395 RID: 37781 RVA: 0x0044C190 File Offset: 0x0044A390
		private void CheckEscClose()
		{
			bool favorMode = this._favorMode;
			if (favorMode)
			{
				this._favorMode = false;
				this.OnFavoriteModeChanged();
			}
			else
			{
				base.CharacterMenu.QuickHide();
			}
		}

		// Token: 0x06009396 RID: 37782 RVA: 0x0044C1C8 File Offset: 0x0044A3C8
		public override void OnCurrentCharacterChange(int prevCharacterId)
		{
			bool flag = base.CharacterMenu.CurCharacterId < 0;
			if (!flag)
			{
				this.localLoadingAnim.SetLoadingState(true);
				this.GetSkillListData();
			}
		}

		// Token: 0x06009397 RID: 37783 RVA: 0x0044C200 File Offset: 0x0044A400
		private void UpdateDetailCombatSkillList()
		{
			this.detailSkillScroll.SetCombatSkillList(this._cachedData, true, true, null, new Action<CombatSkillDisplayDataCharacterMenuListItem, CharacterMenuCombatSkillItem>(this.OnRenderSkill), false, false, null, true);
			this.noContentObj.SetActive(this.detailSkillScroll.filteredData != null && this.detailSkillScroll.filteredData.Count == 0);
		}

		// Token: 0x06009398 RID: 37784 RVA: 0x0044C264 File Offset: 0x0044A464
		private void OnRenderSkill(CombatSkillDisplayDataCharacterMenuListItem skillData, CharacterMenuCombatSkillItem skillView)
		{
			bool canOperateCurrentCharacter = base.CharacterMenu.CurrentCharacterIsTaiwu && base.CharacterMenu.CanOperate;
			skillView.SetFavorIcon(skillData.IsFavorite && this._favorMode);
			skillView.SetSelectedIcon(skillData.TemplateId == this._currSelectedSkillId);
			skillView.GetComponent<CButton>().ClearAndAddListener(delegate
			{
				this.SetSkillInfo(skillData);
				bool flag3 = !canOperateCurrentCharacter;
				if (!flag3)
				{
					this.FavoriteCombatSkill(skillData.TemplateId, !skillData.IsFavorite);
				}
			});
			int count = skillView.transform.childCount - 1;
			CButton btnSetting = null;
			for (int i = count; i >= 0; i--)
			{
				Transform obj = skillView.transform.GetChild(i);
				bool flag = obj.name == "Setting" && obj.TryGetComponent<CButton>(out btnSetting);
				if (flag)
				{
					break;
				}
			}
			bool flag2 = btnSetting == null;
			if (!flag2)
			{
				PointerTrigger pointerTrigger = skillView.GetComponent<PointerTrigger>();
				pointerTrigger.EnterEvent.RemoveAllListeners();
				pointerTrigger.EnterEvent.AddListener(delegate()
				{
					skillView.SetHoverIcon(true);
					bool canOperateCurrentCharacter = canOperateCurrentCharacter;
					if (canOperateCurrentCharacter)
					{
						btnSetting.gameObject.SetActive(true);
					}
				});
				pointerTrigger.ExitEvent.RemoveAllListeners();
				pointerTrigger.ExitEvent.AddListener(delegate()
				{
					skillView.SetHoverIcon(false);
					btnSetting.gameObject.SetActive(false);
				});
				btnSetting.ClearAndAddListener(delegate
				{
					bool flag3 = !canOperateCurrentCharacter;
					if (!flag3)
					{
						ArgumentBox argBox = EasyPool.Get<ArgumentBox>();
						argBox.Set("CombatSkillId", skillData.TemplateId);
						UIElement.CombatSkillPanel.OnHide = new Action(this.GetSkillListData);
						UIElement.CombatSkillPanel.SetOnInitArgs(argBox);
						UIManager.Instance.MaskUI(UIElement.CombatSkillPanel);
					}
				});
			}
		}

		// Token: 0x06009399 RID: 37785 RVA: 0x0044C3FC File Offset: 0x0044A5FC
		private void SetSkillInfo(CombatSkillDisplayDataCharacterMenuListItem skillData)
		{
			bool flag = skillData == null || this._currSelectedSkillId == skillData.TemplateId;
			if (flag)
			{
				this._currSelectedSkillId = -1;
				this.selectSkillInfo.Clear();
			}
			else
			{
				this._currSelectedSkillId = skillData.TemplateId;
				this.selectSkillInfo.Refresh(skillData.TemplateId, skillData.CharId, false, false);
			}
			this.detailSkillScroll.ReRender();
		}

		// Token: 0x0400719F RID: 29087
		[SerializeField]
		private CharacterMenuCombatSkillScrollView detailSkillScroll;

		// Token: 0x040071A0 RID: 29088
		[SerializeField]
		private GameObject noContentObj;

		// Token: 0x040071A1 RID: 29089
		[SerializeField]
		private CToggle togFavorMode;

		// Token: 0x040071A2 RID: 29090
		[SerializeField]
		private RectTransform favorModeRoot;

		// Token: 0x040071A3 RID: 29091
		[SerializeField]
		private SelectSkillInfo selectSkillInfo;

		// Token: 0x040071A4 RID: 29092
		[SerializeField]
		private CharacterMenuLocalLoadingAnim localLoadingAnim;

		// Token: 0x040071A5 RID: 29093
		private bool _favorMode;

		// Token: 0x040071A6 RID: 29094
		private List<CombatSkillDisplayDataCharacterMenuListItem> _cachedData = new List<CombatSkillDisplayDataCharacterMenuListItem>();

		// Token: 0x040071A7 RID: 29095
		private short _currSelectedSkillId;

		// Token: 0x040071A8 RID: 29096
		private bool _initSelectedSkillId;

		// Token: 0x040071A9 RID: 29097
		private bool inited = false;
	}
}
