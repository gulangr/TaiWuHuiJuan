using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using CharacterDataMonitor;
using Config;
using FrameWork;
using FrameWork.UISystem.UIElements;
using Game.Components.Common;
using Game.Views.Building;
using Game.Views.Debate;
using GameData.Domains.Character;
using GameData.Domains.Character.Creation;
using GameData.Domains.Character.Display;
using GameData.Domains.CombatSkill;
using GameData.Domains.Global;
using GameData.Serializer;
using GameData.Utilities;
using UnityEngine;

namespace Game.Views.CharacterMenu
{
	// Token: 0x02000BA9 RID: 2985
	public class ViewCharacterMenuLifeSkill : UI_CharacterMenuSubPageBase
	{
		// Token: 0x06009596 RID: 38294 RVA: 0x0045BDE4 File Offset: 0x00459FE4
		public override bool CheckState(ECharacterSubToggleBase curSubTogglePage, ECharacterSubPage curSubPage)
		{
			return curSubTogglePage == ECharacterSubToggleBase.AttainmentBase;
		}

		// Token: 0x1700101C RID: 4124
		// (get) Token: 0x06009597 RID: 38295 RVA: 0x0045BDFA File Offset: 0x00459FFA
		public override LanguageKey TitleKey
		{
			get
			{
				return LanguageKey.LK_Attainment;
			}
		}

		// Token: 0x06009598 RID: 38296 RVA: 0x0045BE01 File Offset: 0x0045A001
		public override void OnInit(ArgumentBox argsBox)
		{
			this._attainmentPanelInited = false;
			this._attainmentPlanIds = null;
			UIElement element = this.Element;
			element.OnListenerIdReady = (Action)Delegate.Combine(element.OnListenerIdReady, new Action(delegate()
			{
				bool flag = base.CharacterMenu.CurCharacterId >= 0;
				if (flag)
				{
					this.localLoadingAnim.SetLoadingState(true);
					this.UpdateAvatarMonitor();
					this.RequestData();
				}
			}));
		}

		// Token: 0x06009599 RID: 38297 RVA: 0x0045BE39 File Offset: 0x0045A039
		private void RequestData()
		{
			CharacterDomainMethod.AsyncCall.GetCharacterMenuLifeSkillDisplayData(null, base.CharacterMenu.CurCharacterId, delegate(int offset, RawDataPool pool)
			{
				Serializer.Deserialize(pool, offset, ref this._lifeSkillData);
				this.localLoadingAnim.SetLoadingState(false);
				bool flag = !this.Element.Ready;
				if (flag)
				{
					this.Element.ShowAfterRefresh();
				}
				else
				{
					this.RefreshByCachedData();
				}
			});
		}

		// Token: 0x0600959A RID: 38298 RVA: 0x0045BE5A File Offset: 0x0045A05A
		private void Awake()
		{
			this.Init();
		}

		// Token: 0x0600959B RID: 38299 RVA: 0x0045BE64 File Offset: 0x0045A064
		public void Init()
		{
			bool flag = this.inited;
			if (!flag)
			{
				this._detailTypeTogGroup.Init(-1);
				this._detailTypeTogGroup.OnActiveIndexChange += this.OnLifeSkillTypeTogChange;
				this.inited = true;
			}
		}

		// Token: 0x0600959C RID: 38300 RVA: 0x0045BEAA File Offset: 0x0045A0AA
		private void OnDestroy()
		{
			this._detailTypeTogGroup.OnActiveIndexChange -= this.OnLifeSkillTypeTogChange;
		}

		// Token: 0x0600959D RID: 38301 RVA: 0x0045BEC5 File Offset: 0x0045A0C5
		private void OnLifeSkillTypeTogChange(int newIndex, int oldIndex)
		{
			this.OnLifeSkillTypeTogChange((sbyte)newIndex);
		}

		// Token: 0x0600959E RID: 38302 RVA: 0x0045BED4 File Offset: 0x0045A0D4
		private void OnEnable()
		{
			GlobalDomainMethod.Call.InvokeGuidingTrigger(109);
			GlobalDomainMethod.Call.InvokeGuidingTrigger(141);
			bool flag = this._lifeSkillData == null;
			if (!flag)
			{
				this.RefreshByCachedData();
			}
		}

		// Token: 0x0600959F RID: 38303 RVA: 0x0045BF0B File Offset: 0x0045A10B
		private new void OnDisable()
		{
			this.ClearAvatarMonitor();
		}

		// Token: 0x060095A0 RID: 38304 RVA: 0x0045BF15 File Offset: 0x0045A115
		public override void OnSwitchToSubpage(int subPageIndex)
		{
			this.CurTabIndex = subPageIndex;
			this.SelectPage();
		}

		// Token: 0x060095A1 RID: 38305 RVA: 0x0045BF28 File Offset: 0x0045A128
		private void SelectPage()
		{
			bool isNonEvolutionaryType = this.IsNonEvolutionaryType();
			bool flag = base.CharacterMenu.IsTaiwuGearMate(base.CharacterMenu.CurCharacterId) || !isNonEvolutionaryType;
			this._skillDetailRefers.gameObject.SetActive(true);
		}

		// Token: 0x060095A2 RID: 38306 RVA: 0x0045BF74 File Offset: 0x0045A174
		public override void OnSubpageVisible()
		{
			this._subPageVisible = true;
			bool flag = this.CurTabIndex != 1;
			if (flag)
			{
				this.Element.ShowAfterRefresh();
			}
		}

		// Token: 0x060095A3 RID: 38307 RVA: 0x0045BFA5 File Offset: 0x0045A1A5
		public override void OnSubpageInVisible()
		{
			this._subPageVisible = false;
		}

		// Token: 0x060095A4 RID: 38308 RVA: 0x0045BFB0 File Offset: 0x0045A1B0
		public override void OnCurrentCharacterChange(int prevCharacterId)
		{
			bool flag = base.CharacterMenu.CurCharacterId < 0;
			if (!flag)
			{
				this.localLoadingAnim.SetLoadingState(true);
				this.RequestData();
				this.UpdateAvatarMonitor();
				bool flag2 = this.CurTabIndex == 2;
				if (flag2)
				{
					bool currentCharacterIsTaiwuTeammate = base.CharacterMenu.CurrentCharacterIsTaiwuTeammate;
					if (currentCharacterIsTaiwuTeammate)
					{
						CharacterDomainMethod.Call.IsCombatSkillAttainmentLocked(this.Element.GameDataListenerId, base.CharacterMenu.CurCharacterId);
					}
				}
			}
		}

		// Token: 0x060095A5 RID: 38309 RVA: 0x0045C029 File Offset: 0x0045A229
		private void RefreshByCachedData()
		{
			this.OnLearnedLifeSkillUpdate();
			this.OnLifeSkillTypeTogChange(base.CharacterMenu.CurrentSelectedLifeSkillType);
		}

		// Token: 0x060095A6 RID: 38310 RVA: 0x0045C048 File Offset: 0x0045A248
		protected override void OnClick(Transform btn)
		{
			string btnName = btn.name;
			bool flag = btnName == "CenterBtn";
			if (flag)
			{
				base.CharacterMenu.SetCurPageSubpage(1);
			}
		}

		// Token: 0x060095A7 RID: 38311 RVA: 0x0045C07C File Offset: 0x0045A27C
		private void UpdateAvatarMonitor()
		{
			this.RemoveListenerForAvatarMonitor();
			this._avatarInfoMonitor = SingletonObject.getInstance<CharacterMonitorModel>().GetMonitorItem<AvatarInfoMonitor>(base.CharacterMenu.CurCharacterId, false);
			this._avatarInfoMonitor.AddOnCreatingTypeChangeEventListener(new Action(this.SelectPage));
			bool init = this._avatarInfoMonitor.Init;
			if (init)
			{
				this._avatarInfoMonitor.OnDataInit();
			}
		}

		// Token: 0x060095A8 RID: 38312 RVA: 0x0045C0E0 File Offset: 0x0045A2E0
		private void ClearAvatarMonitor()
		{
			this.RemoveListenerForAvatarMonitor();
			this._avatarInfoMonitor = null;
		}

		// Token: 0x060095A9 RID: 38313 RVA: 0x0045C0F1 File Offset: 0x0045A2F1
		private void RemoveListenerForAvatarMonitor()
		{
			AvatarInfoMonitor avatarInfoMonitor = this._avatarInfoMonitor;
			if (avatarInfoMonitor != null)
			{
				avatarInfoMonitor.RemoveOnCreatingTypeChangeEventListener(new Action(this.SelectPage));
			}
		}

		// Token: 0x060095AA RID: 38314 RVA: 0x0045C114 File Offset: 0x0045A314
		private void OnLearnedLifeSkillUpdate()
		{
			for (sbyte type = 0; type < 16; type += 1)
			{
				int readedBookCount = 0;
				short[] skillIdList = Config.LifeSkillType.Instance[type].SkillList;
				for (int bookGrade = 0; bookGrade < skillIdList.Length; bookGrade++)
				{
					short skillId = skillIdList[bookGrade];
					bool flag = this._lifeSkillData.LearnedLifeSkills != null && this._lifeSkillData.LearnedLifeSkills.Any((GameData.Domains.Character.LifeSkillItem item) => item.SkillTemplateId == skillId && item.IsAnyPagesRead());
					if (flag)
					{
						readedBookCount++;
					}
				}
				this._detailTypeTogGroup.Get((int)type).GetComponent<ToggleStyle>().SetLabelText(readedBookCount.ToString());
			}
		}

		// Token: 0x060095AB RID: 38315 RVA: 0x0045C1D0 File Offset: 0x0045A3D0
		private void OnLifeSkillTypeTogChange(sbyte curLifeSkillType)
		{
			this._curLifeSkillType = curLifeSkillType;
			LifeSkillTypeItem skillTypeConfig = Config.LifeSkillType.Instance[this._curLifeSkillType];
			short[] skillIdList = skillTypeConfig.SkillList;
			this._detailTypeTogGroup.SetWithoutNotify((int)curLifeSkillType);
			for (int bookGrade = 0; bookGrade < skillIdList.Length; bookGrade++)
			{
				short skillTemplateId = skillIdList[bookGrade];
				sbyte[] readingProgress = new sbyte[5];
				this.<OnLifeSkillTypeTogChange>g__CalcReadingProgress|47_0(skillTemplateId, readingProgress);
				Config.LifeSkillItem configData = LifeSkill.Instance[skillTemplateId];
				SkillBookItem bookConfig = SkillBook.Instance[configData.SkillBookId];
				CommonLifeSkillBookItemSimple skillRefers = this._skillHolder.GetChild(bookGrade).GetComponent<CommonLifeSkillBookItemSimple>();
				this.<OnLifeSkillTypeTogChange>g__CalcReadingProgress|47_0(skillTemplateId, readingProgress);
				skillRefers.Refresh(new GameData.Domains.Character.LifeSkillItem(skillTemplateId, (from x in readingProgress
				select (int)x).ToArray<int>()), readingProgress);
			}
			LifeSkillTypeItem typeConfig = Config.LifeSkillType.Instance[this._curLifeSkillType];
			ViewCharacterMenuLifeSkill.<>c__DisplayClass47_0 CS$<>8__locals1;
			CS$<>8__locals1.provider = delegate(short templateId)
			{
				sbyte[] readingProgress2 = new sbyte[5];
				this.<OnLifeSkillTypeTogChange>g__CalcReadingProgress|47_0(templateId, readingProgress2);
				return readingProgress2;
			};
			foreach (Config.LifeSkillItem skillConfig in ((IEnumerable<Config.LifeSkillItem>)LifeSkill.Instance))
			{
				bool flag = skillConfig == null || skillConfig.Type != this._curLifeSkillType;
				if (!flag)
				{
					bool shouldBreak = false;
					foreach (Config.ShortList buildingTemplateIds in skillConfig.UnlockBuildingList)
					{
						foreach (short buildingTemplateId in buildingTemplateIds.DataList)
						{
							BuildingBlockItem buildingConfig = BuildingBlock.Instance.GetItem(buildingTemplateId);
							if (buildingConfig == null)
							{
								goto IL_1AB;
							}
							List<short> dependBuildings = buildingConfig.DependBuildings;
							if (dependBuildings == null)
							{
								goto IL_1AB;
							}
							int num = (dependBuildings.Count > 0) ? 1 : 0;
							IL_1AC:
							bool flag2 = num == 0;
							if (flag2)
							{
								continue;
							}
							ViewBuildingArea.SetBuildingIcon(this._buildingIcon, BuildingBlock.Instance[buildingConfig.DependBuildings[0]], true, null);
							shouldBreak = true;
							break;
							IL_1AB:
							num = 0;
							goto IL_1AC;
						}
						bool flag3 = shouldBreak;
						if (flag3)
						{
							break;
						}
					}
					bool flag4 = shouldBreak;
					if (flag4)
					{
						break;
					}
				}
			}
			foreach (DebateStrategyItem cardConfig in ((IEnumerable<DebateStrategyItem>)DebateStrategy.Instance))
			{
				bool flag5 = cardConfig == null || cardConfig.LifeSkillType != this._curLifeSkillType;
				if (!flag5)
				{
					bool locked = true;
					foreach (Config.LifeSkillItem skillConfig2 in ((IEnumerable<Config.LifeSkillItem>)LifeSkill.Instance))
					{
						bool flag6 = skillConfig2 == null || skillConfig2.Type != this._curLifeSkillType || skillConfig2.Grade / 3 + 1 != cardConfig.Level;
						if (!flag6)
						{
							bool flag7 = CS$<>8__locals1.provider(skillConfig2.TemplateId).All((sbyte progress) => progress == 100);
							if (flag7)
							{
								locked = false;
								break;
							}
						}
					}
					int childIndex = (int)(cardConfig.Level - 1);
					DebateCardView card = this._cardLayout.GetChild(childIndex).GetComponent<DebateCardView>();
					card.SetData(cardConfig.TemplateId, -1);
					card.SetSelected(false);
					card.SetInteractable(false);
					card.SetStyleRoot(!locked);
					PointerTrigger pointerTrigger = card.GetComponentInChildren<PointerTrigger>();
					pointerTrigger.EnterEvent.RemoveAllListeners();
					pointerTrigger.ExitEvent.RemoveAllListeners();
					CImage dot = this._cardProgressLayout.GetChild(childIndex).GetComponent<CImage>();
					dot.SetSprite(locked ? "ui9_back_charactermenu_16_progress_unlock_9" : string.Format("{0}{1}", "ui9_back_charactermenu_16_progress_unlock_", childIndex), false, null);
				}
			}
			this.<OnLifeSkillTypeTogChange>g__UpdateProgresses|47_7<Config.ShortList>((Config.LifeSkillItem item) => item.UnlockBuildingList, delegate(Config.ShortList list)
			{
				List<short> dataList = list.DataList;
				return dataList != null && dataList.Count > 0 && list.DataList[0] >= 0;
			}, this._buildingUnlockLayout, true, ref CS$<>8__locals1);
			this.<OnLifeSkillTypeTogChange>g__UpdateProgresses|47_7<sbyte>((Config.LifeSkillItem item) => item.UnlockInformationList, (sbyte level) => level != 0, this._informationUnlockLayuout, true, ref CS$<>8__locals1);
			ViewCharacterMenuLifeSkill.<OnLifeSkillTypeTogChange>g__SetupTips|47_8(this._strategy.GetOrAddComponent<TooltipInvoker>(), TipType.LifeSkillDetailUnlockStrategy, new ArgumentBox().SetObject(MouseTipLifeSkillDetailUnlockInformation.ArgKeyProgressProvider, CS$<>8__locals1.provider).SetObject(MouseTipLifeSkillDetailUnlockInformation.ArgKeyLifeSkillType, typeConfig));
			ViewCharacterMenuLifeSkill.<OnLifeSkillTypeTogChange>g__SetupTips|47_8(this._information.gameObject.GetOrAddComponent<TooltipInvoker>(), TipType.LifeSkillDetailUnlockInformation, new ArgumentBox().SetObject(MouseTipLifeSkillDetailUnlockInformation.ArgKeyProgressProvider, CS$<>8__locals1.provider).SetObject(MouseTipLifeSkillDetailUnlockInformation.ArgKeyLifeSkillType, typeConfig).Set(MouseTipLifeSkillDetailUnlockInformation.ArgKeyCharacterId, base.CharacterMenu.CurCharacterId));
			ViewCharacterMenuLifeSkill.<OnLifeSkillTypeTogChange>g__SetupTips|47_8(this._building.gameObject.GetOrAddComponent<TooltipInvoker>(), TipType.LifeSkillDetailUnlockBuilding, new ArgumentBox().SetObject(MouseTipLifeSkillDetailUnlockInformation.ArgKeyProgressProvider, CS$<>8__locals1.provider).SetObject(MouseTipLifeSkillDetailUnlockInformation.ArgKeyLifeSkillType, typeConfig));
		}

		// Token: 0x060095AC RID: 38316 RVA: 0x0045C7C8 File Offset: 0x0045A9C8
		private bool IsNonEvolutionaryType()
		{
			bool flag = this._avatarInfoMonitor == null;
			if (flag)
			{
				this.UpdateAvatarMonitor();
			}
			return CreatingType.IsNonEvolutionaryType(this._avatarInfoMonitor.CreatingType);
		}

		// Token: 0x060095B1 RID: 38321 RVA: 0x0045C8E0 File Offset: 0x0045AAE0
		[CompilerGenerated]
		private void <OnLifeSkillTypeTogChange>g__UpdateProgresses|47_7<T>(Func<Config.LifeSkillItem, IList<T>> unlockItemProvider, Func<T, bool> unlockItemValidator, RectTransform layout, bool setSprite, ref ViewCharacterMenuLifeSkill.<>c__DisplayClass47_0 A_5)
		{
			ValueTuple<bool, bool>[] states = new ValueTuple<bool, bool>[9];
			foreach (Config.LifeSkillItem skillConfig in ((IEnumerable<Config.LifeSkillItem>)LifeSkill.Instance))
			{
				IList<T> pageUnlockList;
				bool flag = skillConfig == null || skillConfig.Type != this._curLifeSkillType || (pageUnlockList = unlockItemProvider(skillConfig)) == null;
				if (!flag)
				{
					sbyte[] progresses = A_5.provider(skillConfig.TemplateId);
					for (int page = 0; page < pageUnlockList.Count; page++)
					{
						bool flag2 = !unlockItemValidator(pageUnlockList[page]);
						if (!flag2)
						{
							bool locked = states[(int)skillConfig.Grade].Item1;
							bool flag3 = progresses[page] != 100;
							if (flag3)
							{
								locked = true;
							}
							states[(int)skillConfig.Grade] = new ValueTuple<bool, bool>(locked, true);
						}
					}
				}
			}
			if (setSprite)
			{
				for (int grade = 0; grade < states.Length; grade++)
				{
					bool flag4 = grade >= layout.childCount;
					if (flag4)
					{
						break;
					}
					CImage child = layout.GetChild(grade).GetComponent<CImage>();
					string targetSprite = states[grade].Item1 ? "ui9_back_charactermenu_16_progress_unlock_9" : string.Format("{0}{1}", "ui9_back_charactermenu_16_progress_unlock_", grade);
					child.SetSprite(targetSprite, false, null);
					child.gameObject.SetActive(states[grade].Item2);
				}
			}
		}

		// Token: 0x060095B2 RID: 38322 RVA: 0x0045CA90 File Offset: 0x0045AC90
		[CompilerGenerated]
		internal static void <OnLifeSkillTypeTogChange>g__SetupTips|47_8(TooltipInvoker owner, TipType type, ArgumentBox args)
		{
			owner.enabled = true;
			owner.triggerByChildRaycast = true;
			owner.Type = type;
			owner.RuntimeParam = args;
			owner.Refresh(true, -1);
		}

		// Token: 0x060095B3 RID: 38323 RVA: 0x0045CABC File Offset: 0x0045ACBC
		[CompilerGenerated]
		private void <OnLifeSkillTypeTogChange>g__CalcReadingProgress|47_0(short skillTemplateId, sbyte[] result)
		{
			int index = -1;
			GameData.Domains.Character.LifeSkillItem skillData = default(GameData.Domains.Character.LifeSkillItem);
			bool flag = this._lifeSkillData.LearnedLifeSkills != null;
			if (flag)
			{
				index = this._lifeSkillData.LearnedLifeSkills.FindIndex((GameData.Domains.Character.LifeSkillItem skillItem) => skillItem.SkillTemplateId == skillTemplateId);
				bool flag2 = index >= 0;
				if (flag2)
				{
					skillData = this._lifeSkillData.LearnedLifeSkills[index];
				}
			}
			bool currentCharacterIsTaiwu = base.CharacterMenu.CurrentCharacterIsTaiwu;
			if (currentCharacterIsTaiwu)
			{
				for (byte i = 0; i < 5; i += 1)
				{
					bool flag3 = this._lifeSkillData.TaiwuLifeSkills.ContainsKey(skillTemplateId);
					if (flag3)
					{
						result[(int)i] = this._lifeSkillData.TaiwuLifeSkills[skillTemplateId].GetBookPageReadingProgress(i);
					}
					else
					{
						bool flag4 = this._lifeSkillData.TaiwuNotLearnLifeSkills.ContainsKey(skillTemplateId);
						if (flag4)
						{
							result[(int)i] = this._lifeSkillData.TaiwuNotLearnLifeSkills[skillTemplateId].GetBookPageReadingProgress(i);
						}
						else
						{
							result[(int)i] = ((index >= 0 && skillData.IsPageRead(i)) ? 100 : 0);
						}
					}
				}
			}
			else
			{
				for (byte j = 0; j < 5; j += 1)
				{
					result[(int)j] = ((index >= 0 && skillData.IsPageRead(j)) ? 100 : 0);
				}
			}
		}

		// Token: 0x040072BD RID: 29373
		private CharacterMenuLifeSkillDisplayData _lifeSkillData;

		// Token: 0x040072BE RID: 29374
		private sbyte[] _attainmentPlanIds;

		// Token: 0x040072BF RID: 29375
		private bool _subPageVisible;

		// Token: 0x040072C0 RID: 29376
		private bool _skillDataInited;

		// Token: 0x040072C1 RID: 29377
		private bool _attainmentPanelInited;

		// Token: 0x040072C2 RID: 29378
		private readonly List<CombatSkillDisplayData> _attainmentSkillList = new List<CombatSkillDisplayData>();

		// Token: 0x040072C3 RID: 29379
		private readonly List<GameData.Domains.Character.CombatSkillHelper.AttainmentSectInfo> _sectInfos = new List<GameData.Domains.Character.CombatSkillHelper.AttainmentSectInfo>(9);

		// Token: 0x040072C4 RID: 29380
		private bool _autoSettingTypeOrSkillTog;

		// Token: 0x040072C5 RID: 29381
		private bool _autoSettingBookTog;

		// Token: 0x040072C6 RID: 29382
		[SerializeField]
		private Refers _skillDetailRefers;

		// Token: 0x040072C7 RID: 29383
		[SerializeField]
		private RectTransform _skillHolder;

		// Token: 0x040072C8 RID: 29384
		[Header("UnlockContent")]
		[SerializeField]
		private RectTransform _building;

		// Token: 0x040072C9 RID: 29385
		[SerializeField]
		private RectTransform _buildingUnlockLayout;

		// Token: 0x040072CA RID: 29386
		[SerializeField]
		private CImage _buildingIcon;

		// Token: 0x040072CB RID: 29387
		[SerializeField]
		private GameObject _strategy;

		// Token: 0x040072CC RID: 29388
		[SerializeField]
		private RectTransform _information;

		// Token: 0x040072CD RID: 29389
		[SerializeField]
		private RectTransform _informationUnlockLayuout;

		// Token: 0x040072CE RID: 29390
		[SerializeField]
		private RectTransform _cardLayout;

		// Token: 0x040072CF RID: 29391
		[SerializeField]
		private RectTransform _cardProgressLayout;

		// Token: 0x040072D0 RID: 29392
		[SerializeField]
		private CToggleGroup _detailTypeTogGroup;

		// Token: 0x040072D1 RID: 29393
		[SerializeField]
		private CToggle _lifeSkillTypeTogglePrefab;

		// Token: 0x040072D2 RID: 29394
		[SerializeField]
		private CharacterMenuLocalLoadingAnim localLoadingAnim;

		// Token: 0x040072D3 RID: 29395
		private AvatarInfoMonitor _avatarInfoMonitor;

		// Token: 0x040072D4 RID: 29396
		private bool inited = false;

		// Token: 0x040072D5 RID: 29397
		private sbyte _curLifeSkillType;
	}
}
