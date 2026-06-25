using System;
using System.Collections.Generic;
using FrameWork;
using FrameWork.UISystem.UIElements;
using Game.Components.Avatar;
using Game.Components.Common;
using Game.Views.CharacterMenu;
using GameData.Domains.Character.Display;
using TMPro;
using UnityEngine;

namespace Game.Views.LifeSkillCombat
{
	// Token: 0x02000982 RID: 2434
	public class LifeSkillCombatBeginCharacterItem : MonoBehaviour
	{
		// Token: 0x17000D31 RID: 3377
		// (get) Token: 0x06007509 RID: 29961 RVA: 0x0036885F File Offset: 0x00366A5F
		private ViewLifeSkillCombatBegin Parent
		{
			get
			{
				return UIElement.LifeSkillCombatBegin.UiBaseAs<ViewLifeSkillCombatBegin>();
			}
		}

		// Token: 0x17000D32 RID: 3378
		// (get) Token: 0x0600750A RID: 29962 RVA: 0x0036886B File Offset: 0x00366A6B
		private ELifeSkillCombatBeginStage _currentStage
		{
			get
			{
				return this.Parent.CurrentStage;
			}
		}

		// Token: 0x0600750B RID: 29963 RVA: 0x00368878 File Offset: 0x00366A78
		private void Awake()
		{
			this.charBtn.ClearAndAddListener(delegate
			{
				bool flag = this._characterDisplayData == null;
				if (!flag)
				{
					ArgumentBox argBox = EasyPool.Get<ArgumentBox>();
					argBox.Set("CharacterId", this._characterDisplayData.CharacterId);
					argBox.Set("CanOperate", false);
					argBox.Set("PreviousView", 1);
					argBox.SetObject("ViewCharacterMenuTaretPage", new SubPageIndex(ECharacterSubToggleBase.AttainmentBase, ECharacterSubPage.AttainmentLifeSkill));
					UIElement.CharacterMenu.SetOnInitArgs(argBox);
					UIManager.Instance.ShowUI(UIElement.CharacterMenu, true);
				}
			});
			this.extraBtn.ClearAndAddListener(delegate
			{
				this.Parent.OnCharacterItemExtraButtonClicked(this._isTaiwu);
			});
		}

		// Token: 0x0600750C RID: 29964 RVA: 0x003688AB File Offset: 0x00366AAB
		private void OnEnable()
		{
			this.avatar.gameObject.SetActive(false);
		}

		// Token: 0x0600750D RID: 29965 RVA: 0x003688C0 File Offset: 0x00366AC0
		public void Set(bool isTaiwu, CharacterDisplayData characterDisplayData, List<CharacterDisplayData> teammateDisplayDatas, bool isFirstTurn, int wisdom)
		{
			this._isTaiwu = isTaiwu;
			this._characterDisplayData = characterDisplayData;
			this.avatarHolder.localPosition = this.avatarHolder.localPosition.SetY((float)((this.Parent.CurrentStage == ELifeSkillCombatBeginStage.SelectStrategy) ? 53 : 53));
			this.avatar.Refresh(this._characterDisplayData, true);
			this.avatar.gameObject.SetActive(true);
			this.charName.text = NameCenter.GetMonasticTitleOrDisplayName(this._characterDisplayData, this._isTaiwu);
			this.bubble.Hide();
			this.attainmentHolder.gameObject.SetActive(true);
			string turnString = (this.Parent.CurrentStage != ELifeSkillCombatBeginStage.SelectStrategy) ? LanguageKey.LK_SmallVillage_InfectedOrgAnonymous.Tr().SetColor("lightgrey") : (isFirstTurn ? LanguageKey.UI_LifeSkillBattle_Prepare_Offensive.Tr().SetColor("brightblue") : LanguageKey.UI_LifeSkillBattle_Prepare_Defensive.Tr().SetColor("brightred"));
			this.turnText.text = turnString;
			this.wisdomValue.text = wisdom.ToString();
			bool isSelectSkillAI = this.Parent.CurrentStage == ELifeSkillCombatBeginStage.SelectSkillAI;
			bool isSelectStrategy = this.Parent.CurrentStage == ELifeSkillCombatBeginStage.SelectStrategy;
			this.exclude.gameObject.SetActive(!isSelectStrategy && this._isTaiwu != isSelectSkillAI);
			this.extraBtn.gameObject.SetActive(this._currentStage == ELifeSkillCombatBeginStage.SelectStrategy);
			bool flag = isTaiwu && this._currentStage == ELifeSkillCombatBeginStage.SelectStrategy;
			if (flag)
			{
				this.extraBtn.interactable = !this.Parent.ShouldDisableQuickSelectButton(teammateDisplayDatas);
			}
			else
			{
				this.extraBtn.interactable = !this.Parent.HasForceGiveIn;
			}
			for (int i = 0; i < this.teammateItems.Length; i++)
			{
				bool hasTeammate = i < teammateDisplayDatas.Count && teammateDisplayDatas[i] != null;
				this.teammateItems[i].Set(isTaiwu, i, hasTeammate ? teammateDisplayDatas[i] : null);
				this.teammateItems[i].gameObject.SetActive(this._currentStage == ELifeSkillCombatBeginStage.SelectStrategy);
			}
			TooltipInvoker tip = this.turnText.transform.parent.GetComponent<TooltipInvoker>();
			tip.enabled = (this.Parent.CurrentStage == ELifeSkillCombatBeginStage.SelectStrategy);
			tip.Type = (isFirstTurn ? TipType.LifeSkillCombatFirstMove : TipType.LifeSkillCombatLastMove);
		}

		// Token: 0x0600750E RID: 29966 RVA: 0x00368B43 File Offset: 0x00366D43
		public void ShowBubble(string str)
		{
			this.bubble.SetText(str, true);
			SingletonObject.getInstance<YieldHelper>().DelaySecondsDo(1f, new Action(this.bubble.Hide));
		}

		// Token: 0x0600750F RID: 29967 RVA: 0x00368B78 File Offset: 0x00366D78
		public void ShowAttainment(sbyte lifeSkillType, int value)
		{
			bool flag = lifeSkillType == -1;
			if (!flag)
			{
				this.lifeSkillIcon.SetSprite("ui9_back_attainments_life_3_" + lifeSkillType.ToString(), false, null);
				this.lifeSkillName.text = LocalStringManager.GetFormat(LanguageKey.LK_Debate_Prepare_Attainments_Format, LocalStringManager.Get(string.Format("LK_LifeSkillType_{0}", lifeSkillType)), LanguageKey.LK_Attainment.Tr());
				this.lifeSkillValue.text = value.ToString();
			}
		}

		// Token: 0x040057B1 RID: 22449
		[SerializeField]
		private RectTransform avatarHolder;

		// Token: 0x040057B2 RID: 22450
		[SerializeField]
		private Game.Components.Avatar.Avatar avatar;

		// Token: 0x040057B3 RID: 22451
		[SerializeField]
		private TextMeshProUGUI charName;

		// Token: 0x040057B4 RID: 22452
		[SerializeField]
		private CButton charBtn;

		// Token: 0x040057B5 RID: 22453
		[SerializeField]
		private CButton extraBtn;

		// Token: 0x040057B6 RID: 22454
		[SerializeField]
		private Game.Components.Common.Bubble bubble;

		// Token: 0x040057B7 RID: 22455
		[SerializeField]
		private RectTransform attainmentHolder;

		// Token: 0x040057B8 RID: 22456
		[SerializeField]
		private CImage lifeSkillIcon;

		// Token: 0x040057B9 RID: 22457
		[SerializeField]
		private TextMeshProUGUI lifeSkillName;

		// Token: 0x040057BA RID: 22458
		[SerializeField]
		private TextMeshProUGUI lifeSkillValue;

		// Token: 0x040057BB RID: 22459
		[SerializeField]
		private TextMeshProUGUI turnText;

		// Token: 0x040057BC RID: 22460
		[SerializeField]
		private TextMeshProUGUI wisdomValue;

		// Token: 0x040057BD RID: 22461
		[SerializeField]
		private TextMeshProUGUI exclude;

		// Token: 0x040057BE RID: 22462
		[SerializeField]
		private LifeSkillCombatBeginTeammateItem[] teammateItems;

		// Token: 0x040057BF RID: 22463
		private const int _posYWithAttainment = 53;

		// Token: 0x040057C0 RID: 22464
		private const int _posYWithOutAttainment = 53;

		// Token: 0x040057C1 RID: 22465
		private bool _isTaiwu;

		// Token: 0x040057C2 RID: 22466
		private CharacterDisplayData _characterDisplayData;
	}
}
