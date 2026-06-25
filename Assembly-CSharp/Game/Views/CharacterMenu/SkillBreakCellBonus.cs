using System;
using System.Collections.Generic;
using FrameWork;
using FrameWork.UISystem.UIElements;
using Game.Components.Avatar;
using GameData.Domains.Character;
using GameData.Domains.Character.Display;
using GameData.Domains.Global;
using GameData.Domains.Item;
using GameData.Domains.Taiwu;
using GameData.Serializer;
using GameData.Utilities;
using TMPro;
using UICommon.Character;
using UnityEngine;

namespace Game.Views.CharacterMenu
{
	// Token: 0x02000B86 RID: 2950
	public class SkillBreakCellBonus : SkillBreakCellBase
	{
		// Token: 0x17000FC8 RID: 4040
		// (get) Token: 0x060091BE RID: 37310 RVA: 0x0043E001 File Offset: 0x0043C201
		public CButton Button
		{
			get
			{
				return this.button;
			}
		}

		// Token: 0x060091BF RID: 37311 RVA: 0x0043E009 File Offset: 0x0043C209
		protected override void OnAwake()
		{
			this.InitAvatarHandler();
		}

		// Token: 0x060091C0 RID: 37312 RVA: 0x0043E013 File Offset: 0x0043C213
		private void InitAvatarHandler()
		{
			this._avatarHandler = new CharacterAvatar(this.character.CGet<Game.Components.Avatar.Avatar>("Avatar"), true)
			{
				CanShowGrave = false
			};
		}

		// Token: 0x060091C1 RID: 37313 RVA: 0x0043E03C File Offset: 0x0043C23C
		protected override void RefreshInternal()
		{
			this.RefreshBonusTips();
			bool hasBonus = base.Bonus.Type > ESkillBreakPlateBonusType.None;
			this.RefreshBonusButton();
			this.nameImage.SetActive(!hasBonus && this._cellData.State != ESkillBreakGridState.Selected);
			bool isLastCanSelect = this._oldPlate != null && this._oldPlate.GetGridAt(this._coordinate).State == ESkillBreakGridState.CanSelect;
			bool emptyMarkActive = !hasBonus && this._cellData.State == ESkillBreakGridState.Selected;
			bool flag = isLastCanSelect && emptyMarkActive;
			if (flag)
			{
				bool flag2 = this._delayCallCoroutine != null;
				if (flag2)
				{
					base.StopCoroutine(this._delayCallCoroutine);
				}
				this._delayCallCoroutine = SingletonObject.getInstance<YieldHelper>().DelaySecondsDo(0.1f, delegate
				{
					this.RefreshBonusCellImage(this._cellData.State);
					this.emptyMark.SetActive(true);
				});
			}
			else
			{
				this.emptyMark.SetActive(emptyMarkActive);
				this.RefreshBonusCellImage(this._cellData.State);
			}
			this.notEmptyAreaList.ForEach(delegate(GameObject obj)
			{
				obj.SetActive(hasBonus);
			});
			base.RefreshPowerLabel(this.powerLabel);
			bool hasBonus2 = hasBonus;
			if (hasBonus2)
			{
				this.RefreshBonusItem();
				this.RefreshBonusGrade();
			}
		}

		// Token: 0x060091C2 RID: 37314 RVA: 0x0043E194 File Offset: 0x0043C394
		private void RefreshBonusTips()
		{
			TooltipInvoker tip = this.button.GetComponent<TooltipInvoker>();
			TooltipInvoker tooltipInvoker = tip;
			if (tooltipInvoker.RuntimeParam == null)
			{
				tooltipInvoker.RuntimeParam = new ArgumentBox();
			}
			List<int> powerList = EasyPool.Get<List<int>>();
			powerList.Clear();
			for (int range = 1; range < 4; range++)
			{
				powerList.Add(this._plate.CalcAddMaxPowerAsBonus(this._coordinate, range));
			}
			tip.RuntimeParam.SetObject("Plate", this._plate).Set("SkillId", this._skillId).SetObject("LifeSkillAttainments", this._lifeSkillAttainments).Set<SkillBreakPlateIndex>("Coord", this._coordinate).Set("NeedExp", base.GetNeedExp()).Set("CurrentExp", this._currentExp).SetObject("PossiblePowerList", powerList);
			tip.Refresh(false, -1);
		}

		// Token: 0x060091C3 RID: 37315 RVA: 0x0043E288 File Offset: 0x0043C488
		private void RefreshBonusButton()
		{
			this.button.interactable = (base.CanSelect() || this._cellData.State == ESkillBreakGridState.Selected);
			bool interactable = this.button.interactable;
			if (interactable)
			{
				GlobalDomainMethod.Call.InvokeGuidingTrigger(235);
			}
			base.BindButtonEvent(this.button);
			this.button.GetComponent<PointerTrigger>().enabled = this.button.interactable;
		}

		// Token: 0x060091C4 RID: 37316 RVA: 0x0043E300 File Offset: 0x0043C500
		private void RefreshBonusCellImage(ESkillBreakGridState cellDataState)
		{
			bool selected = cellDataState == ESkillBreakGridState.Selected;
			this.SetSpriteAutoSize(this.frame, selected ? "ui9_back_charactermenu_skillbreakbonus_1_0" : "ui9_back_charactermenu_skillbreakbonus_0_0");
			this.SetButtonSprite(this.button, selected ? "ui9_back_charactermenu_skillbreakbonus_1_1" : "ui9_back_charactermenu_skillbreakbonus_0_1");
			this.SetSpriteAutoSize(this.bg, selected ? "ui9_back_charactermenu_skillbreakbonus_1_2" : "ui9_back_charactermenu_skillbreakbonus_0_2");
		}

		// Token: 0x060091C5 RID: 37317 RVA: 0x0043E367 File Offset: 0x0043C567
		private void SetButtonSprite(CButton button, string spriteName)
		{
			this.SetSpriteAutoSize(button.GetComponent<CImage>(), spriteName);
		}

		// Token: 0x060091C6 RID: 37318 RVA: 0x0043E378 File Offset: 0x0043C578
		private void SetSpriteAutoSize(CImage sprite, string spriteName)
		{
			sprite.SetSprite(spriteName, true, null);
		}

		// Token: 0x060091C7 RID: 37319 RVA: 0x0043E388 File Offset: 0x0043C588
		private void RefreshBonusGrade()
		{
			sbyte grade = base.Bonus.Grade;
			this.gradeBg.SetSprite(ItemView.GetGradeIcon(grade), false, null);
			this.grade.text = LocalStringManager.Get(string.Format("LK_ShortGrade_{0}", grade));
		}

		// Token: 0x060091C8 RID: 37320 RVA: 0x0043E3DC File Offset: 0x0043C5DC
		private void RefreshBonusItem()
		{
			this.exp.gameObject.SetActive(false);
			this.character.gameObject.SetActive(false);
			this.item.gameObject.SetActive(false);
			switch (base.Bonus.Type)
			{
			case ESkillBreakPlateBonusType.Item:
				this.item.gameObject.SetActive(true);
				this.item.SetSprite(ItemTemplateHelper.GetIcon(base.Bonus.ItemType, base.Bonus.ItemTemplateId), false, null);
				break;
			case ESkillBreakPlateBonusType.Relation:
			case ESkillBreakPlateBonusType.Friend:
				this.character.gameObject.SetActive(true);
				CharacterDomainMethod.AsyncCall.GetCharacterDisplayData(this._requestHandler, base.Bonus.RelationCharId, delegate(int offset, RawDataPool pool)
				{
					CharacterDisplayData data = null;
					Serializer.Deserialize(pool, offset, ref data);
					bool flag = this._avatarHandler != null;
					if (flag)
					{
						this._avatarHandler.SetIsDead(data.AliveState != 0);
						this._avatarHandler.CharacterId = data.CharacterId;
					}
				});
				break;
			case ESkillBreakPlateBonusType.Exp:
				this.exp.gameObject.SetActive(true);
				break;
			}
		}

		// Token: 0x060091C9 RID: 37321 RVA: 0x0043E4E0 File Offset: 0x0043C6E0
		public override void SetSwapSelectable(bool selectable)
		{
			bool flag = this.button == null;
			if (!flag)
			{
				PointerTrigger pointer = this.button.GetComponent<PointerTrigger>();
				if (selectable)
				{
					bool flag2 = !this._swapSelectableOverride;
					if (flag2)
					{
						this._swapSelectablePrevInteractable = this.button.interactable;
						this._swapSelectableOverride = true;
					}
					this.button.interactable = true;
					bool flag3 = pointer != null;
					if (flag3)
					{
						pointer.enabled = true;
					}
				}
				else
				{
					bool flag4 = !this._swapSelectableOverride;
					if (!flag4)
					{
						this.button.interactable = this._swapSelectablePrevInteractable;
						bool flag5 = pointer != null;
						if (flag5)
						{
							pointer.enabled = this.button.interactable;
						}
						this._swapSelectableOverride = false;
					}
				}
			}
		}

		// Token: 0x04007040 RID: 28736
		[Header("玄机格子组件")]
		[SerializeField]
		private CImage bg;

		// Token: 0x04007041 RID: 28737
		[SerializeField]
		private CButton button;

		// Token: 0x04007042 RID: 28738
		[SerializeField]
		private CImage frame;

		// Token: 0x04007043 RID: 28739
		[SerializeField]
		private GameObject emptyMark;

		// Token: 0x04007044 RID: 28740
		[SerializeField]
		private GameObject nameImage;

		// Token: 0x04007045 RID: 28741
		[SerializeField]
		private TextMeshProUGUI powerLabel;

		// Token: 0x04007046 RID: 28742
		[SerializeField]
		private CImage exp;

		// Token: 0x04007047 RID: 28743
		[SerializeField]
		private CImage item;

		// Token: 0x04007048 RID: 28744
		[SerializeField]
		private Refers character;

		// Token: 0x04007049 RID: 28745
		[SerializeField]
		private TextMeshProUGUI grade;

		// Token: 0x0400704A RID: 28746
		[SerializeField]
		private CImage gradeBg;

		// Token: 0x0400704B RID: 28747
		[SerializeField]
		private List<GameObject> notEmptyAreaList;

		// Token: 0x0400704C RID: 28748
		private CharacterAvatar _avatarHandler;

		// Token: 0x0400704D RID: 28749
		private Coroutine _delayCallCoroutine;

		// Token: 0x0400704E RID: 28750
		private bool _swapSelectableOverride;

		// Token: 0x0400704F RID: 28751
		private bool _swapSelectablePrevInteractable;
	}
}
