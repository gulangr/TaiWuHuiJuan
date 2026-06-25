using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using Config;
using DG.Tweening;
using DG.Tweening.Core;
using DG.Tweening.Plugins.Options;
using FrameWork;
using FrameWork.CommandSystem;
using FrameWork.UISystem.Components;
using FrameWork.UISystem.UIElements;
using Game.Components.Avatar;
using Game.Components.Character;
using GameData.Domains.Character;
using GameData.Domains.Character.Display;
using GameData.Domains.Item;
using GameData.Domains.Item.Display;
using GameData.Domains.TaiwuEvent;
using GameData.GameDataBridge;
using GameData.Serializer;
using GameData.Utilities;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Game.Views.CharacterMenu
{
	// Token: 0x02000B99 RID: 2969
	public class ViewCharacterMenu : UIBase
	{
		// Token: 0x060092B7 RID: 37559 RVA: 0x0044544C File Offset: 0x0044364C
		public void OnStackViewActivated()
		{
			bool flag = this.closeButton != null;
			if (flag)
			{
				this.closeButton.gameObject.SetActive(false);
			}
		}

		// Token: 0x060092B8 RID: 37560 RVA: 0x0044547C File Offset: 0x0044367C
		public void OnStackViewDeactivated()
		{
			bool flag = this.closeButton != null;
			if (flag)
			{
				this.closeButton.gameObject.SetActive(true);
			}
		}

		// Token: 0x17000FDA RID: 4058
		// (get) Token: 0x060092B9 RID: 37561 RVA: 0x004454AC File Offset: 0x004436AC
		public Attribute Attribute
		{
			get
			{
				return this.attributeAndInjury.Attribute;
			}
		}

		// Token: 0x17000FDB RID: 4059
		// (get) Token: 0x060092BA RID: 37562 RVA: 0x004454B9 File Offset: 0x004436B9
		public Injury Injury
		{
			get
			{
				return this.attributeAndInjury.Injury;
			}
		}

		// Token: 0x17000FDC RID: 4060
		// (get) Token: 0x060092BB RID: 37563 RVA: 0x004454C6 File Offset: 0x004436C6
		public bool IsTaiwuTeam
		{
			get
			{
				ViewCharacterMenuDisplayData viewCharacterMenuDisplayData = this._viewCharacterMenuDisplayData;
				return viewCharacterMenuDisplayData != null && viewCharacterMenuDisplayData.IsTaiwuTeam;
			}
		}

		// Token: 0x17000FDD RID: 4061
		// (get) Token: 0x060092BC RID: 37564 RVA: 0x004454DA File Offset: 0x004436DA
		public ECharacterMenuFunctionControlType CharacterControlType
		{
			get
			{
				return this.CurrentCharacterIsTaiwu ? ECharacterMenuFunctionControlType.Taiwu : (this.CurrentCharacterIsTaiwuTeammate ? ECharacterMenuFunctionControlType.Teammate : ECharacterMenuFunctionControlType.Other);
			}
		}

		// Token: 0x17000FDE RID: 4062
		// (get) Token: 0x060092BD RID: 37565 RVA: 0x004454F3 File Offset: 0x004436F3
		public bool UseAnonymousName
		{
			get
			{
				return this._useAnonymousName;
			}
		}

		// Token: 0x17000FDF RID: 4063
		// (get) Token: 0x060092BE RID: 37566 RVA: 0x004454FC File Offset: 0x004436FC
		// (set) Token: 0x060092BF RID: 37567 RVA: 0x004455BC File Offset: 0x004437BC
		public bool CanOperate
		{
			get
			{
				bool bannedByNegativeTeammateCommand = this.TeamReplacedTeammateCommands != null && this.TeamReplacedTeammateCommands.Any(new Func<sbyte, bool>(ViewCharacterMenu.IsNegativeCommand));
				bool flag = bannedByNegativeTeammateCommand;
				bool result;
				if (flag)
				{
					result = false;
				}
				else
				{
					BuildingModel buildingModel = SingletonObject.getInstance<BuildingModel>();
					bool flag2 = buildingModel.YuanshanThreeVitalsDisplayData != null;
					if (flag2)
					{
						int charId = this.CurCharacterId;
						foreach (CharacterDisplayData data in buildingModel.YuanshanThreeVitalsDisplayData)
						{
							bool flag3 = data.CharacterId == charId;
							if (flag3)
							{
								return false;
							}
						}
					}
					result = this._canOperate;
				}
				return result;
			}
			private set
			{
				this._canOperate = value;
			}
		}

		// Token: 0x17000FE0 RID: 4064
		// (get) Token: 0x060092C0 RID: 37568 RVA: 0x004455C5 File Offset: 0x004437C5
		// (set) Token: 0x060092C1 RID: 37569 RVA: 0x004455CD File Offset: 0x004437CD
		public bool OpenFromCombatPrepare { get; private set; }

		// Token: 0x17000FE1 RID: 4065
		// (get) Token: 0x060092C2 RID: 37570 RVA: 0x004455D6 File Offset: 0x004437D6
		public List<sbyte> TeamReplacedTeammateCommands
		{
			get
			{
				Dictionary<int, List<sbyte>> teamReplacedTeammateCommands = this._teamReplacedTeammateCommands;
				return (teamReplacedTeammateCommands != null) ? teamReplacedTeammateCommands.GetValueOrDefault(this.CurCharacterId) : null;
			}
		}

		// Token: 0x17000FE2 RID: 4066
		// (get) Token: 0x060092C3 RID: 37571 RVA: 0x004455F0 File Offset: 0x004437F0
		public IReadOnlyList<CharacterDisplayData> DisplayCharacters
		{
			get
			{
				ViewCharacterMenuDisplayData viewCharacterMenuDisplayData = this._viewCharacterMenuDisplayData;
				return (viewCharacterMenuDisplayData != null) ? viewCharacterMenuDisplayData.CharacterDisplayDataList : null;
			}
		}

		// Token: 0x17000FE3 RID: 4067
		// (get) Token: 0x060092C4 RID: 37572 RVA: 0x00445604 File Offset: 0x00443804
		public ViewCharacterMenuDisplayData DisplayData
		{
			get
			{
				return this._viewCharacterMenuDisplayData;
			}
		}

		// Token: 0x17000FE4 RID: 4068
		// (get) Token: 0x060092C5 RID: 37573 RVA: 0x0044560C File Offset: 0x0044380C
		public int CurCharacterId
		{
			get
			{
				List<CharacterDisplayData> list = this._filteredCharacterDisplayDataList;
				bool flag = list != null && list.Count > 0 && this._curCharacterIdIndex >= 0 && this._curCharacterIdIndex < list.Count && list[this._curCharacterIdIndex] != null;
				int result;
				if (flag)
				{
					result = list[this._curCharacterIdIndex].CharacterId;
				}
				else
				{
					result = -1;
				}
				return result;
			}
		}

		// Token: 0x060092C6 RID: 37574 RVA: 0x00445674 File Offset: 0x00443874
		private void InitOpenAnim()
		{
			this.mainToggleGroup.gameObject.SetActive(true);
			bool playedInitAnim = this._playedInitAnim;
			if (!playedInitAnim)
			{
				Sequence sequence = base.InitDefaultSequenceIn(false);
				bool flag = this.moveIn != null;
				if (flag)
				{
					List<Tween> tweens = this.moveIn.GetTweens();
					foreach (Tween t in tweens)
					{
						sequence.Join(t);
					}
				}
				UI_CharacterMenuSubPageBase subToggleUIBase;
				int num;
				this.GetRealPageBase(ViewCharacterMenu.CurSubToggleIndex, ViewCharacterMenu.CurSubSubPageIndex, out subToggleUIBase, out num);
				bool flag2 = subToggleUIBase != null && subToggleUIBase.moveIn != null;
				if (flag2)
				{
					List<Tween> tweens2 = subToggleUIBase.moveIn.GetTweens();
					foreach (Tween t2 in tweens2)
					{
						sequence.Join(t2);
					}
				}
				sequence.SetUpdate(true);
				sequence.Play<Sequence>();
				this._playedInitAnim = true;
			}
		}

		// Token: 0x060092C7 RID: 37575 RVA: 0x004457B4 File Offset: 0x004439B4
		private List<int> GetDisplayCharacterIds()
		{
			IReadOnlyList<CharacterDisplayData> list = this.DisplayCharacters;
			List<int> ids = new List<int>();
			bool flag = list == null;
			List<int> result;
			if (flag)
			{
				result = ids;
			}
			else
			{
				for (int i = 0; i < list.Count; i++)
				{
					bool flag2 = list[i] != null;
					if (flag2)
					{
						ids.Add(list[i].CharacterId);
					}
				}
				result = ids;
			}
			return result;
		}

		// Token: 0x060092C8 RID: 37576 RVA: 0x00445824 File Offset: 0x00443A24
		private int IndexOfCharacterId(int charId)
		{
			List<CharacterDisplayData> list = this._filteredCharacterDisplayDataList;
			bool flag = list == null;
			int result;
			if (flag)
			{
				result = -1;
			}
			else
			{
				for (int i = 0; i < list.Count; i++)
				{
					bool flag2 = list[i] != null && list[i].CharacterId == charId;
					if (flag2)
					{
						return i;
					}
				}
				result = -1;
			}
			return result;
		}

		// Token: 0x060092C9 RID: 37577 RVA: 0x00445888 File Offset: 0x00443A88
		public bool IsTaiwu(int charId)
		{
			return SingletonObject.getInstance<BasicGameData>().TaiwuCharId == charId;
		}

		// Token: 0x060092CA RID: 37578 RVA: 0x00445897 File Offset: 0x00443A97
		public bool IsTaiwuSpecialTeammate(int charId)
		{
			ViewCharacterMenuDisplayData viewCharacterMenuDisplayData = this._viewCharacterMenuDisplayData;
			return ((viewCharacterMenuDisplayData != null) ? viewCharacterMenuDisplayData.TaiwuSpecialGroup : null) != null && this._viewCharacterMenuDisplayData.TaiwuSpecialGroup.Contains(charId);
		}

		// Token: 0x060092CB RID: 37579 RVA: 0x004458C1 File Offset: 0x00443AC1
		public bool IsTaiwuGearMate(int charId)
		{
			ViewCharacterMenuDisplayData viewCharacterMenuDisplayData = this._viewCharacterMenuDisplayData;
			return ((viewCharacterMenuDisplayData != null) ? viewCharacterMenuDisplayData.TaiwuGearMateGroup : null) != null && this._viewCharacterMenuDisplayData.TaiwuGearMateGroup.Contains(charId);
		}

		// Token: 0x060092CC RID: 37580 RVA: 0x004458EB File Offset: 0x00443AEB
		public bool IsTaiwuBeastTeammate(int charId)
		{
			return this.IsTaiwuSpecialTeammate(charId) && !this.IsTaiwuGearMate(charId);
		}

		// Token: 0x17000FE5 RID: 4069
		// (get) Token: 0x060092CD RID: 37581 RVA: 0x00445903 File Offset: 0x00443B03
		public bool CurrentCharacterIsTaiwu
		{
			get
			{
				return this.IsTaiwu(this.CurCharacterId);
			}
		}

		// Token: 0x17000FE6 RID: 4070
		// (get) Token: 0x060092CE RID: 37582 RVA: 0x00445911 File Offset: 0x00443B11
		public bool CurrentCharacterIsTaiwuTeammate
		{
			get
			{
				return !this.CurrentCharacterIsTaiwu && this.IsTaiwuTeam;
			}
		}

		// Token: 0x060092CF RID: 37583 RVA: 0x00445924 File Offset: 0x00443B24
		public override void OnReset()
		{
			bool flag = this.childPages != null;
			if (flag)
			{
				this.childPages.anchoredPosition = Vector2.zero;
			}
			this._childPagesHidden = false;
			ViewCharacterMenu._allSubPageElements.ForEach(delegate(int _, CharacterMenuSubPageElement element)
			{
				bool flag2 = null == element.UiBase;
				if (flag2)
				{
					element.PrepareRes(false, null, false);
				}
				bool flag3 = null != element.UiBase;
				if (flag3)
				{
					element.UiBase.OnReset();
				}
				return false;
			});
			DOTweenAnimation[] animations = this.moveIn.GetComponents<DOTweenAnimation>();
			for (int i = 0; i < animations.Length; i++)
			{
				animations[i].DORewind();
			}
		}

		// Token: 0x060092D0 RID: 37584 RVA: 0x004459B0 File Offset: 0x00443BB0
		public override void OnInit(ArgumentBox argsBox)
		{
			this._initArgBox = argsBox;
			this._initedSubPageElements.Clear();
			bool flag = !this.ReadAndCheckArgs(argsBox);
			if (!flag)
			{
				this.InitAllPageArray();
				this.NeedDataListenerId = true;
				ViewCharacterMenu.NeedClear = false;
				UIElement element = this.Element;
				element.OnListenerIdReady = (Action)Delegate.Combine(element.OnListenerIdReady, new Action(this.RequestByStack));
			}
		}

		// Token: 0x060092D1 RID: 37585 RVA: 0x00445A1C File Offset: 0x00443C1C
		private void RequestByStack()
		{
			StackViewContext context = this._characterViewStack.Peek();
			List<int> charIdList = context.Context.charIdList;
			int charIdForRequest = (charIdList != null && charIdList.Count > 0) ? -1 : context.Context.charId;
			CharacterDomainMethod.Call.GetViewCharacterMenuDisplayData(this.Element.GameDataListenerId, charIdForRequest, context.Context.charIdList);
		}

		// Token: 0x060092D2 RID: 37586 RVA: 0x00445A7C File Offset: 0x00443C7C
		private void InitAllPageArray()
		{
			if (this._allSubPages == null)
			{
				this._allSubPages = new UI_CharacterMenuSubPageBase[ViewCharacterMenu._allSubPageElements.Length];
			}
			for (int i = 0; i < ViewCharacterMenu._allSubPageElements.Length; i++)
			{
				CharacterMenuSubPageElement element = ViewCharacterMenu._allSubPageElements[i];
				UI_CharacterMenuSubPageBase subPage = element.UiBaseAs<UI_CharacterMenuSubPageBase>();
				subPage.Key = i;
				bool needClear = ViewCharacterMenu.NeedClear;
				if (needClear)
				{
					subPage.CurTabIndex = 0;
				}
				this._allSubPages[i] = subPage;
			}
		}

		// Token: 0x060092D3 RID: 37587 RVA: 0x00445AF0 File Offset: 0x00443CF0
		private bool ReadAndCheckArgs(ArgumentBox argsBox)
		{
			int charId;
			bool hasCharacterId = argsBox.Get("CharacterId", out charId);
			bool flag = !hasCharacterId;
			if (flag)
			{
				charId = -1;
			}
			List<int> characterIdList;
			bool hasCharacterIdList = argsBox.Get<List<int>>("CharacterIdList", out characterIdList);
			bool flag2 = !hasCharacterId && !hasCharacterIdList;
			bool result;
			if (flag2)
			{
				result = false;
			}
			else
			{
				bool flag3 = !argsBox.Get("PreviousView", out this._characterControlTemplateId);
				if (flag3)
				{
					this._characterControlTemplateId = -1;
				}
				SubPageIndex targetIndex;
				bool hasExplicitTargetPage = argsBox.Get<SubPageIndex>("ViewCharacterMenuTaretPage", out targetIndex) && (targetIndex.SubToggleIndex != ECharacterSubToggleBase.None || targetIndex.SubSubPageIndex > ECharacterSubPage.None);
				bool flag4 = hasExplicitTargetPage;
				if (flag4)
				{
					this._targetPageIndex = targetIndex;
				}
				else
				{
					bool flag5 = ViewCharacterMenu.CurSubToggleIndex != ECharacterSubToggleBase.None || ViewCharacterMenu.CurSubSubPageIndex > ECharacterSubPage.None;
					if (flag5)
					{
						this._targetPageIndex = new SubPageIndex
						{
							SubToggleIndex = ViewCharacterMenu.CurSubToggleIndex,
							SubSubPageIndex = ViewCharacterMenu.CurSubSubPageIndex
						};
					}
					else
					{
						this._targetPageIndex = new SubPageIndex
						{
							SubToggleIndex = ECharacterSubToggleBase.CharacterBase,
							SubSubPageIndex = ECharacterSubPage.Character
						};
					}
				}
				int baseAttributeIndex;
				bool flag6 = argsBox.Get("BaseAttributeIndex", out baseAttributeIndex);
				if (flag6)
				{
					this.SetBaseAttributeState((sbyte)baseAttributeIndex);
				}
				bool flag7 = ViewCharacterMenu.NeedClear && !hasExplicitTargetPage;
				if (flag7)
				{
					this._targetPageIndex = new SubPageIndex
					{
						SubToggleIndex = ECharacterSubToggleBase.CharacterBase,
						SubSubPageIndex = ECharacterSubPage.Character
					};
				}
				bool flag8 = !argsBox.Get("CanOperate", out this._canOperate);
				if (flag8)
				{
					this._canOperate = false;
				}
				bool openFromCombatPrepare;
				argsBox.Get("OpenFromCombatPrepare", out openFromCombatPrepare);
				this.OpenFromCombatPrepare = openFromCombatPrepare;
				argsBox.Get<Dictionary<int, List<sbyte>>>("ReplacedTeammateCommands", out this._teamReplacedTeammateCommands);
				bool anonymous;
				this._useAnonymousName = (argsBox.Get("Anonymous", out anonymous) && anonymous);
				if (this._characterViewStack == null)
				{
					this._characterViewStack = new Stack<StackViewContext>();
				}
				this._characterViewStack.Clear();
				this._characterViewStack.Push(new StackViewContext
				{
					Index = new SubPageIndex
					{
						SubToggleIndex = this._targetPageIndex.SubToggleIndex,
						SubSubPageIndex = this._targetPageIndex.SubSubPageIndex
					},
					Context = new ViewContext
					{
						charId = charId,
						charIdList = characterIdList
					},
					CanOperate = this.CanOperate
				});
				result = true;
			}
			return result;
		}

		// Token: 0x060092D4 RID: 37588 RVA: 0x00445D5C File Offset: 0x00443F5C
		protected override void OnClick(Transform btn)
		{
			string btnName = btn.name;
			bool flag = "CloseButton" == btnName;
			if (flag)
			{
				this.QuickHide();
			}
			else
			{
				bool flag2 = "MaskToFocus" == btnName;
				if (flag2)
				{
					Action onTryClosePage = this.OnTryClosePage;
					if (onTryClosePage != null)
					{
						onTryClosePage();
					}
				}
			}
		}

		// Token: 0x060092D5 RID: 37589 RVA: 0x00445DB0 File Offset: 0x00443FB0
		public override void QuickHide()
		{
			TutorialChapterModel tutorialChapterModel = SingletonObject.getInstance<TutorialChapterModel>();
			bool flag = tutorialChapterModel.InGuiding && this.CurrentCharacterIsTaiwu;
			if (flag)
			{
				bool flag2 = tutorialChapterModel.WaitOpenCharacterNeili || (tutorialChapterModel.NeiliAllocateFitChapter7 && !this.CanClosePageInTutorialChapter7());
				if (flag2)
				{
					DialogCmd cmd = new DialogCmd
					{
						Title = LocalStringManager.Get(LanguageKey.LK_GameName),
						Content = LocalStringManager.Get(LanguageKey.LK_TutorialChapter7_AllocateNeiliTips),
						Type = 2
					};
					ArgumentBox argBox = EasyPool.Get<ArgumentBox>();
					CommandManager.AddCommandShowUI(EPriority.ShowUINormal, UIElement.Dialog, argBox.SetObject("Cmd", cmd));
					return;
				}
			}
			bool flag3 = this.OnTryClosePage != null;
			if (flag3)
			{
				this.OnTryClosePage();
			}
			else
			{
				bool flag4 = this.TryStackBackCharacterView();
				if (!flag4)
				{
					bool activeSelf = this.StackView.gameObject.activeSelf;
					if (activeSelf)
					{
						this.StackView.ResetAndSetNotActive(this);
					}
					else
					{
						TaiwuEventDomainMethod.Call.CloseUI("ViewCharacterMenu", false, this.CurCharacterId);
						base.QuickHide();
					}
				}
			}
		}

		// Token: 0x060092D6 RID: 37590 RVA: 0x00445EBC File Offset: 0x004440BC
		public override void OnNotifyGameData(List<NotificationWrapper> notifications)
		{
			foreach (NotificationWrapper wrapper in notifications)
			{
				Notification notification = wrapper.Notification;
				byte type = notification.Type;
				byte b = type;
				if (b == 1)
				{
					bool flag = notification.DomainId == 4 && notification.MethodId == 208;
					if (flag)
					{
						Serializer.Deserialize(wrapper.DataPool, notification.ValueOffset, ref this._viewCharacterMenuDisplayData);
						this.Refresh();
						this.Element.ShowAfterRefresh();
						this.InitOpenAnim();
					}
					else
					{
						bool flag2 = notification.DomainId == 4 && notification.MethodId == 131;
						if (flag2)
						{
							CharacterDisplayData updatedData = null;
							Serializer.Deserialize(wrapper.DataPool, notification.ValueOffset, ref updatedData);
							this.OnCharacterDisplayDataUpdated(updatedData);
						}
					}
				}
			}
		}

		// Token: 0x060092D7 RID: 37591 RVA: 0x00445FC4 File Offset: 0x004441C4
		private void Awake()
		{
			this.characterScroll.InitPageCount();
			this.characterScroll.OnItemRender += this.OnCharacterItemRender;
			this.StackView.ClearAndSetNotActive();
			this.StackView.InitConfirmButton(this);
			this.Injury.SetAreaInteract(new Action(this.OnClickEatArea));
		}

		// Token: 0x060092D8 RID: 37592 RVA: 0x00446028 File Offset: 0x00444228
		private void OnClickEatArea()
		{
			ArgumentBox argBox = EasyPool.Get<ArgumentBox>();
			argBox.Set("charId", this.CurCharacterId);
			UIElement.EatDetail.SetOnInitArgs(argBox);
			UIManager.Instance.ShowUI(UIElement.EatDetail, true);
		}

		// Token: 0x060092D9 RID: 37593 RVA: 0x0044606C File Offset: 0x0044426C
		private void OnEnable()
		{
			this.mainToggleGroup.gameObject.SetActive(false);
			this.characterScrollNode.SetActive(false);
			this.attributeAndInjury.gameObject.SetActive(false);
			this.attributeAndInjury.OnRequestDataCallBack = delegate()
			{
				this.SetRightLoadingState(false);
			};
			GEvent.Add(UiEvents.HideCharacterMenuAndSubPages, new GEvent.Callback(this.OnHideCharacterMenuAndSubPages));
			GEvent.Add(UiEvents.RestoreCharacterMenuAndSubPages, new GEvent.Callback(this.OnRestoreCharacterMenuAndSubPages));
			GEvent.Add(EEvents.OnFunctionLockStateChange, new GEvent.Callback(this.OnFunctionLockStateChange));
			GEvent.Add(UiEvents.EnterTransferItem, new GEvent.Callback(this.OnEnterTransferItemMode));
			GEvent.Add(UiEvents.OnRefreshCharacterHealUIBottom, new GEvent.Callback(this.OnRefreshCharacterHealUIBottom));
			GEvent.Add(UiEvents.OnNeedOpenCharacterMenuSubPage, new GEvent.Callback(this.OpenTargetPage));
			GEvent.Add(UiEvents.OnRelationButtonClick, new GEvent.Callback(this.OnRelationButtonClick));
			GEvent.Add(UiEvents.OnChangeCharacterClothing, new GEvent.Callback(this.OnChangeCharacterClothing));
			GEvent.Add(UiEvents.MultiplyTransferItemResult, new GEvent.Callback(this.OnMultiplyTransferItemResult));
		}

		// Token: 0x060092DA RID: 37594 RVA: 0x004461B8 File Offset: 0x004443B8
		private void OnDisable()
		{
			this._playedInitAnim = false;
			CharacterMenuToggleGroup characterMenuToggleGroup = this.mainToggleGroup;
			if (characterMenuToggleGroup != null)
			{
				characterMenuToggleGroup.HideDropdownImmediate();
			}
			GEvent.Remove(UiEvents.HideCharacterMenuAndSubPages, new GEvent.Callback(this.OnHideCharacterMenuAndSubPages));
			GEvent.Remove(UiEvents.RestoreCharacterMenuAndSubPages, new GEvent.Callback(this.OnRestoreCharacterMenuAndSubPages));
			this.characterScroll.SetDataCount(0);
			GEvent.Remove(EEvents.OnFunctionLockStateChange, new GEvent.Callback(this.OnFunctionLockStateChange));
			GEvent.Remove(UiEvents.EnterTransferItem, new GEvent.Callback(this.OnEnterTransferItemMode));
			GEvent.Remove(UiEvents.OnNeedOpenCharacterMenuSubPage, new GEvent.Callback(this.OpenTargetPage));
			GEvent.Remove(UiEvents.OnRefreshCharacterHealUIBottom, new GEvent.Callback(this.OnRefreshCharacterHealUIBottom));
			GEvent.Remove(UiEvents.OnRelationButtonClick, new GEvent.Callback(this.OnRelationButtonClick));
			GEvent.Remove(UiEvents.OnChangeCharacterClothing, new GEvent.Callback(this.OnChangeCharacterClothing));
			GEvent.Remove(UiEvents.MultiplyTransferItemResult, new GEvent.Callback(this.OnMultiplyTransferItemResult));
			ViewCharacterMenu._allSubPageElements.ForEach(delegate(int _, CharacterMenuSubPageElement element)
			{
				UI_CharacterMenuSubPageBase subPageBase = element.UiBaseAs<UI_CharacterMenuSubPageBase>();
				bool flag = subPageBase != null && subPageBase.gameObject.activeSelf;
				if (flag)
				{
					subPageBase.SetCharId(-2);
					element.RawHide();
				}
				return false;
			});
			this._curCharacterIdIndex = 0;
			GEvent.OnEvent(UiEvents.CharacterMenuHide, null);
			this.attributeAndInjury.CharacterId = -1;
			this.attributeAndInjury.OnRequestDataCallBack = null;
			this._viewCharacterMenuDisplayData = null;
			this._initedSubPageElements.Clear();
			this._filteredCharacterDisplayDataList.Clear();
		}

		// Token: 0x060092DB RID: 37595 RVA: 0x00446356 File Offset: 0x00444556
		private void OnDestroy()
		{
			Object.Destroy(this.StackView.gameObject);
			ViewCharacterMenu._allSubPageElements.ForEach(delegate(int _, CharacterMenuSubPageElement element)
			{
				element.Destroy();
				return false;
			});
		}

		// Token: 0x060092DC RID: 37596 RVA: 0x00446394 File Offset: 0x00444594
		private void Refresh()
		{
			bool flag = this._viewCharacterMenuDisplayData == null;
			if (!flag)
			{
				bool flag2 = !this._tabDropdownInited;
				if (flag2)
				{
					this.InitializeTabDropdownComponent();
				}
				this.RefreshCharacterScrollByPridiction(this.GetCharacterListFilterPredicate());
				StackViewContext stackTop = this._characterViewStack.Peek();
				this._canOperate = stackTop.CanOperate;
				int targetCharId = stackTop.Context.charId;
				int foundIndex = this.IndexOfCharacterId(targetCharId);
				this._curCharacterIdIndex = ((foundIndex >= 0) ? foundIndex : 0);
				SubPageIndex desiredIndex = ViewCharacterMenu.NormalizePageIndex(stackTop.Index);
				SubPageIndex finalIndex = desiredIndex;
				SubPageIndex fallbackIndex;
				bool flag3 = this.SetTogglesInteractable(desiredIndex, out fallbackIndex);
				if (flag3)
				{
					finalIndex = fallbackIndex;
				}
				this.ApplyPageIndex(finalIndex, false);
				this.RefreshToggleAndPage();
				this.teammateCount.gameObject.SetActive(this._viewCharacterMenuDisplayData.IsTaiwuTeam);
				this.teammateCount.text = LocalStringManager.Get(LanguageKey.LK_CharacterMenu_Char_List_Title) + " " + string.Format("{0}/{1}", this._filteredCharacterDisplayDataList.Count, this._viewCharacterMenuDisplayData.TaiwuGroupMaxDisplayCount);
			}
		}

		// Token: 0x060092DD RID: 37597 RVA: 0x004464AC File Offset: 0x004446AC
		private static SubPageIndex NormalizePageIndex(SubPageIndex index)
		{
			bool flag = index.SubSubPageIndex > ECharacterSubPage.None;
			if (flag)
			{
				index.SubToggleIndex = ECharacterSubToggleBase.None;
			}
			return index;
		}

		// Token: 0x060092DE RID: 37598 RVA: 0x004464D4 File Offset: 0x004446D4
		private void ApplyPageIndex(SubPageIndex targetIndex, bool updateStackTopIndex)
		{
			bool flag = targetIndex.SubToggleIndex == ECharacterSubToggleBase.None && targetIndex.SubSubPageIndex == ECharacterSubPage.None;
			if (flag)
			{
				ECharacterSubToggleBase fallbackToggle = (ViewCharacterMenu.CurSubToggleIndex != ECharacterSubToggleBase.None) ? ViewCharacterMenu.CurSubToggleIndex : ECharacterSubToggleBase.CharacterBase;
				targetIndex = new SubPageIndex
				{
					SubToggleIndex = fallbackToggle,
					SubSubPageIndex = this.GetDefaultDisplaySubpage(fallbackToggle)
				};
			}
			bool flag2 = targetIndex.SubSubPageIndex > ECharacterSubPage.None;
			if (flag2)
			{
				targetIndex.SubToggleIndex = this.GetSubTogglekeyBySubPage(targetIndex.SubSubPageIndex);
			}
			else
			{
				bool flag3 = targetIndex.SubToggleIndex != ECharacterSubToggleBase.None;
				if (flag3)
				{
					targetIndex.SubSubPageIndex = this.GetDefaultDisplaySubpage(targetIndex.SubToggleIndex);
				}
			}
			bool flag4 = targetIndex.SubToggleIndex == ECharacterSubToggleBase.None && targetIndex.SubSubPageIndex == ECharacterSubPage.None;
			if (flag4)
			{
				targetIndex = new SubPageIndex
				{
					SubToggleIndex = ECharacterSubToggleBase.CharacterBase,
					SubSubPageIndex = ECharacterSubPage.Character
				};
			}
			ViewCharacterMenu.CurSubToggleIndex = targetIndex.SubToggleIndex;
			ViewCharacterMenu.CurSubSubPageIndex = targetIndex.SubSubPageIndex;
			bool flag5 = ViewCharacterMenu.CurSubSubPageIndex > ECharacterSubPage.None;
			if (flag5)
			{
				ECharacterSubToggleBase parent = this.GetSubTogglekeyBySubPage(ViewCharacterMenu.CurSubSubPageIndex);
				this._lastSelectedSubPageByToggle[parent] = ViewCharacterMenu.CurSubSubPageIndex;
			}
			bool flag6;
			if (updateStackTopIndex)
			{
				Stack<StackViewContext> characterViewStack = this._characterViewStack;
				flag6 = (characterViewStack != null && characterViewStack.Count > 0);
			}
			else
			{
				flag6 = false;
			}
			bool flag7 = flag6;
			if (flag7)
			{
				StackViewContext context = this._characterViewStack.Pop();
				context.Index = targetIndex;
				this._characterViewStack.Push(context);
			}
			this.UpdateInformationTabVisibility();
		}

		// Token: 0x060092DF RID: 37599 RVA: 0x00446648 File Offset: 0x00444848
		private Predicate<CharacterDisplayData> GetCharacterListFilterPredicate()
		{
			ViewCharacterMenu.<>c__DisplayClass84_0 CS$<>8__locals1 = new ViewCharacterMenu.<>c__DisplayClass84_0();
			bool hideState = SingletonObject.getInstance<BasicGameData>().HideAllTeammates;
			bool flag = !hideState;
			Predicate<CharacterDisplayData> result;
			if (flag)
			{
				result = null;
			}
			else
			{
				ViewCharacterMenu.<>c__DisplayClass84_0 CS$<>8__locals2 = CS$<>8__locals1;
				ViewCharacterMenuDisplayData viewCharacterMenuDisplayData = this._viewCharacterMenuDisplayData;
				CS$<>8__locals2.leaderId = ((viewCharacterMenuDisplayData != null) ? viewCharacterMenuDisplayData.GroupLeaderId : -1);
				bool flag2 = CS$<>8__locals1.leaderId < 0 && this._viewCharacterMenuDisplayData.IsTaiwuTeam;
				if (flag2)
				{
					CS$<>8__locals1.leaderId = SingletonObject.getInstance<BasicGameData>().TaiwuCharId;
				}
				bool flag3 = CS$<>8__locals1.leaderId < 0;
				if (flag3)
				{
					result = null;
				}
				else
				{
					result = ((CharacterDisplayData data) => data != null && data.CharacterId == CS$<>8__locals1.leaderId);
				}
			}
			return result;
		}

		// Token: 0x060092E0 RID: 37600 RVA: 0x004466DC File Offset: 0x004448DC
		private bool SetTogglesInteractable(SubPageIndex currentIndex, out SubPageIndex fallbackIndex)
		{
			fallbackIndex = currentIndex;
			ViewCharacterMenu.<>c__DisplayClass85_0 CS$<>8__locals1;
			CS$<>8__locals1.canViewSocial = this.GetCanViewSocial();
			CS$<>8__locals1.canViewLifeRecord = this.GetCanViewLifeRecord();
			CS$<>8__locals1.canViewRelation = this.GetCanViewRelation();
			ECharacterSubToggleBase currentToggle = (currentIndex.SubSubPageIndex != ECharacterSubPage.None) ? this.GetSubTogglekeyBySubPage(currentIndex.SubSubPageIndex) : currentIndex.SubToggleIndex;
			CS$<>8__locals1.tutorialModel = SingletonObject.getInstance<TutorialChapterModel>();
			CS$<>8__locals1.isInTutorial = SingletonObject.getInstance<TutorialChapterModel>().InGuiding;
			bool needChangePageIndex = !ViewCharacterMenu.<SetTogglesInteractable>g__IsToggleInteractable|85_0(currentToggle, ref CS$<>8__locals1);
			bool flag = !needChangePageIndex && currentIndex.SubSubPageIndex == ECharacterSubPage.Prison;
			if (flag)
			{
				bool flag2 = !SingletonObject.getInstance<FunctionLockManager>().IsFunctionUnlock(19);
				if (flag2)
				{
					needChangePageIndex = true;
				}
			}
			for (int i = 0; i < ViewCharacterMenu.SubToggleInfo.Length; i++)
			{
				bool interactable = ViewCharacterMenu.<SetTogglesInteractable>g__IsToggleInteractable|85_0((ECharacterSubToggleBase)i, ref CS$<>8__locals1);
				CharacterMenuToggleGroup characterMenuToggleGroup = this.mainToggleGroup;
				if (characterMenuToggleGroup != null)
				{
					characterMenuToggleGroup.SetParentInteractable((ECharacterSubToggleBase)i, interactable);
				}
			}
			bool flag3 = needChangePageIndex;
			if (flag3)
			{
				fallbackIndex = new SubPageIndex
				{
					SubToggleIndex = ECharacterSubToggleBase.CharacterBase,
					SubSubPageIndex = ECharacterSubPage.Character
				};
			}
			return needChangePageIndex;
		}

		// Token: 0x060092E1 RID: 37601 RVA: 0x00446800 File Offset: 0x00444A00
		private void RefreshCharacterScrollByPridiction(Predicate<CharacterDisplayData> predicate)
		{
			this._filteredCharacterDisplayDataList.Clear();
			bool flag = predicate == null;
			if (flag)
			{
				this._filteredCharacterDisplayDataList.AddRange(this._viewCharacterMenuDisplayData.CharacterDisplayDataList);
			}
			else
			{
				this._filteredCharacterDisplayDataList.AddRange(from d in this._viewCharacterMenuDisplayData.CharacterDisplayDataList
				where predicate(d)
				select d);
			}
			this.characterScroll.SetDataCount(this._filteredCharacterDisplayDataList.Count);
		}

		// Token: 0x060092E2 RID: 37602 RVA: 0x0044688C File Offset: 0x00444A8C
		public void KickOutCharacter()
		{
			this.RequestByStack();
		}

		// Token: 0x060092E3 RID: 37603 RVA: 0x00446898 File Offset: 0x00444A98
		public void RefreshSubTogglePage()
		{
			UI_CharacterMenuSubPageBase subToggleUIBase;
			int num;
			this.GetRealPageBase(ViewCharacterMenu.CurSubToggleIndex, ViewCharacterMenu.CurSubSubPageIndex, out subToggleUIBase, out num);
			CharacterMenuSubPageElement subToggleUIElement;
			this.GetRealPageElement(ViewCharacterMenu.CurSubToggleIndex, ViewCharacterMenu.CurSubSubPageIndex, out subToggleUIElement);
			this.attributeAndInjury.gameObject.SetActive(subToggleUIBase.ShowBaseAttribute);
			this.SetRightLoadingState(true);
			this.attributeAndInjury.CharacterId = this.CurCharacterId;
			CharacterMenuSubPageElement toShowElement = subToggleUIElement;
			UI_CharacterMenuSubPageBase toShowUiBase = subToggleUIBase;
			int pageIndex = this.GetRealPageIndexBySubToggle(ViewCharacterMenu.CurSubToggleIndex);
			bool flag = pageIndex < 0;
			if (flag)
			{
				pageIndex = this.GetRealPageIndexBySubPage(ViewCharacterMenu.CurSubSubPageIndex);
			}
			bool needInitSubPage = !this._initedSubPageElements.Contains(toShowElement);
			bool flag2 = needInitSubPage;
			if (flag2)
			{
				toShowElement.UiBase.OnInit(this._initArgBox);
				toShowElement.Hide(false);
				bool flag3 = !toShowUiBase.gameObject.activeSelf;
				if (flag3)
				{
					toShowUiBase.gameObject.SetActive(true);
					Action onActive = toShowElement.OnActive;
					if (onActive != null)
					{
						onActive();
					}
					toShowElement.OnActive = null;
				}
				toShowElement.MonitorData();
				toShowUiBase.SetCharId(this.CurCharacterId);
				Action onListenerIdReady = toShowElement.OnListenerIdReady;
				if (onListenerIdReady != null)
				{
					onListenerIdReady();
				}
				toShowElement.OnListenerIdReady = null;
				this._initedSubPageElements.Add(toShowElement);
			}
			else
			{
				bool flag4 = !toShowUiBase.gameObject.activeSelf;
				if (flag4)
				{
					toShowUiBase.gameObject.SetActive(true);
					Action onActive2 = toShowElement.OnActive;
					if (onActive2 != null)
					{
						onActive2();
					}
					toShowElement.OnActive = null;
				}
				toShowUiBase.SetCharId(this.CurCharacterId);
			}
			toShowElement.OnShowed = delegate()
			{
				for (int i = 0; i < ViewCharacterMenu._allSubPageElements.Length; i++)
				{
					CharacterMenuSubPageElement element = ViewCharacterMenu._allSubPageElements[i];
					bool flag6 = element == null || element == toShowElement;
					if (!flag6)
					{
						element.Hide(false);
					}
				}
				bool activeInHierarchy = this.characterScroll.gameObject.activeInHierarchy;
				if (activeInHierarchy)
				{
					this.characterScroll.ReRender();
				}
				toShowElement.SetTransformToLastSibling();
			};
			bool needShowCharacterScroll = toShowElement.UiBaseAs<UI_CharacterMenuSubPageBase>().ShowCharacterList;
			Vector2 anchoredPos = this.characterScroll.GetComponent<RectTransform>().anchoredPosition;
			anchoredPos.x = (float)(needShowCharacterScroll ? 0 : -1500);
			this.characterScroll.GetComponent<RectTransform>().anchoredPosition = anchoredPos;
			bool flag5 = !this.Element.Ready;
			if (flag5)
			{
				this.Element.ShowAfterRefresh();
				base.GetComponent<RectTransform>().localScale = Vector3.one;
				this.InitOpenAnim();
			}
			toShowElement.Show();
			this._forceRefreshSubPage = false;
		}

		// Token: 0x060092E4 RID: 37604 RVA: 0x00446B1C File Offset: 0x00444D1C
		private void SetRightLoadingState(bool isShow)
		{
			UI_CharacterMenuSubPageBase subToggleUIBase;
			int num;
			this.GetRealPageBase(ViewCharacterMenu.CurSubToggleIndex, ViewCharacterMenu.CurSubSubPageIndex, out subToggleUIBase, out num);
			bool flag = subToggleUIBase == null || !subToggleUIBase.ShowBaseAttribute;
			if (flag)
			{
				this.localLoadingAnim.SetLoadingState(false);
			}
			else
			{
				this.localLoadingAnim.SetLoadingState(isShow);
			}
		}

		// Token: 0x060092E5 RID: 37605 RVA: 0x00446B75 File Offset: 0x00444D75
		private IEnumerator DelayHideRightLoading()
		{
			yield return new WaitForSecondsRealtime(0.1f);
			this.SetRightLoadingState(false);
			yield break;
		}

		// Token: 0x060092E6 RID: 37606 RVA: 0x00446B84 File Offset: 0x00444D84
		private LanguageKey GetTitleLanguageKey(ECharacterSubToggleBase subTogglePage, ECharacterSubPage subPage)
		{
			bool flag = subPage > ECharacterSubPage.None;
			if (flag)
			{
				switch (subPage)
				{
				case ECharacterSubPage.Character:
					return LanguageKey.LK_CharacterMenu_Title_Info;
				case ECharacterSubPage.Team:
					return LanguageKey.LK_CharacterMenu_Title_Team;
				case ECharacterSubPage.Prison:
					return LanguageKey.LK_CharacterMenu_Title_Kidnap;
				case ECharacterSubPage.Attainment:
					return LanguageKey.LK_CharacterMenu_Attainments;
				case ECharacterSubPage.AttainmentCombatSkill:
					return LanguageKey.LK_CharacterMenu_Title_CombatSkill;
				case ECharacterSubPage.AttainmentLifeSkill:
					return LanguageKey.LK_CharacterMenu_Title_LifeSkill;
				case ECharacterSubPage.Relationship:
					return LanguageKey.LK_CharacterMenu_Title_RelationShip_Title;
				case ECharacterSubPage.Genealogy:
					return LanguageKey.LK_CharacterMenu_Title_RelationShip_Title2;
				case ECharacterSubPage.Information:
					return LanguageKey.LK_Information;
				case ECharacterSubPage.Secret:
					return LanguageKey.LK_SecretInformation;
				case ECharacterSubPage.Equipment:
					return LanguageKey.LK_CharacterMenu_Title_Equip;
				case ECharacterSubPage.Vehicle:
					return LanguageKey.LK_CarHorse;
				}
			}
			switch (subTogglePage)
			{
			case ECharacterSubToggleBase.EquipmentBase:
				return LanguageKey.LK_CharacterMenu_Title_Equip;
			case ECharacterSubToggleBase.ItemBase:
				return LanguageKey.LK_CharacterMenu_Title_Items;
			case ECharacterSubToggleBase.PracticeBase:
				return LanguageKey.LK_CharacterMenu_Title_Practice;
			case ECharacterSubToggleBase.NeiliBase:
				return LanguageKey.LK_CharacterMenu_Title_Neili;
			case ECharacterSubToggleBase.EquipCombatSkillBase:
				return LanguageKey.LK_CharacterMenu_Title_EquipCombatSkill;
			case ECharacterSubToggleBase.RelationshipBase:
				return LanguageKey.LK_CharacterMenu_Title_RelationShip;
			case ECharacterSubToggleBase.StoryBase:
				return LanguageKey.LK_CharacterMenu_Title_LifeRecords;
			}
			return LanguageKey.Invalid;
		}

		// Token: 0x060092E7 RID: 37607 RVA: 0x00446CC8 File Offset: 0x00444EC8
		private void OnCharacterItemRender(int index, GameObject obj)
		{
			List<CharacterDisplayData> list = this._filteredCharacterDisplayDataList;
			bool flag = list == null || index < 0 || index >= list.Count || list[index] == null;
			if (!flag)
			{
				int charId = list[index].CharacterId;
				CharacterMenuSelectableCharacterNormal item = obj.GetComponent<CharacterMenuSelectableCharacterNormal>();
				item.ClearAndAddListener(delegate
				{
					this.OnCharacterButtonClick(charId, index);
				});
				bool flag2 = obj.name != string.Format("character_{0}", charId);
				if (flag2)
				{
					obj.name = string.Format("character_{0}", charId);
				}
				bool selected = charId == this.CurCharacterId;
				CharacterDisplayData displayData = list[index];
				item.Set(displayData, selected);
				item.interactable = !selected;
			}
		}

		// Token: 0x060092E8 RID: 37608 RVA: 0x00446DD0 File Offset: 0x00444FD0
		private void OnCharacterButtonClick(int charId, int index)
		{
			bool flag = this.OnTryChangeCharacter != null;
			if (flag)
			{
				this.OnTryChangeCharacter(delegate
				{
					this.SelectCharacterAction(charId, index);
				});
			}
			else
			{
				this.SelectCharacterAction(charId, index);
			}
		}

		// Token: 0x060092E9 RID: 37609 RVA: 0x00446E34 File Offset: 0x00445034
		private void SelectCharacterAction(int charId, int index)
		{
			bool flag = charId == this.CurCharacterId;
			if (!flag)
			{
				bool flag2 = this._transferItemData != null;
				if (flag2)
				{
					bool flag3 = charId != this.CurCharacterId;
					if (flag3)
					{
						ItemDisplayData itemData = this._transferItemData.Clone(-1);
						CharacterDomainMethod.AsyncCall.GetCharacterDisplayData(null, charId, delegate(int offset, RawDataPool pool)
						{
							Serializer.Deserialize(pool, offset, ref this._beforeTransferCharacterDisplayData);
						});
						this.OnConfirmTransferItemCharacter(charId);
						CharacterDomainMethod.AsyncCall.GetCharacterDisplayData(null, charId, delegate(int offset, RawDataPool pool)
						{
							Serializer.Deserialize(pool, offset, ref this._afterTransferCharacterDisplayData);
							this.ShowTransferResult(charId, itemData);
						});
						this.characterScroll.ReRender();
					}
				}
				else
				{
					bool isMultiplyTransfer = this.IsMultiplySelect && this._itemOperationType == ItemOperationType.EItemOperationType.Transfer;
					bool flag4 = isMultiplyTransfer;
					if (flag4)
					{
						this.OnConfirmTransferMultiplyItemCharacter(charId);
					}
					this._curCharacterIdIndex = index;
					this.characterScroll.ReRender();
					bool flag5 = this.CurCharacterId != charId;
					if (!flag5)
					{
						SubPageIndex currentIndex = new SubPageIndex
						{
							SubToggleIndex = ViewCharacterMenu.CurSubToggleIndex,
							SubSubPageIndex = ViewCharacterMenu.CurSubSubPageIndex
						};
						SubPageIndex fallbackIndex;
						bool needFallback = this.SetTogglesInteractable(currentIndex, out fallbackIndex);
						bool flag6 = needFallback;
						if (flag6)
						{
							this.ApplyPageIndex(fallbackIndex, true);
						}
						bool flag7 = needFallback;
						if (flag7)
						{
							this.RefreshToggleAndPage();
						}
						else
						{
							bool flag8 = !isMultiplyTransfer;
							if (flag8)
							{
								this.RefreshSubTogglePage();
							}
						}
					}
				}
			}
		}

		// Token: 0x060092EA RID: 37610 RVA: 0x00446FE0 File Offset: 0x004451E0
		private void OnMultiplyTransferItemResult(ArgumentBox argBox)
		{
			int charId;
			bool flag = !argBox.Get("CharId", out charId);
			if (!flag)
			{
				bool flag2 = !argBox.Get<CharacterDisplayData>("BeforeTransferCharacterDisplayData", out this._beforeTransferCharacterDisplayData);
				if (!flag2)
				{
					bool flag3 = !argBox.Get<CharacterDisplayData>("AfterTransferCharacterDisplayData", out this._afterTransferCharacterDisplayData);
					if (!flag3)
					{
						this.ShowTransferResult(charId, null);
					}
				}
			}
		}

		// Token: 0x060092EB RID: 37611 RVA: 0x00447044 File Offset: 0x00445244
		private void ShowTransferResult(int charId, ItemDisplayData itemData)
		{
			bool flag = this._beforeTransferCharacterDisplayData == null || this._afterTransferCharacterDisplayData == null;
			if (!flag)
			{
				GameObject obj = null;
				for (int i = 0; i < this._filteredCharacterDisplayDataList.Count; i++)
				{
					bool flag2 = this._filteredCharacterDisplayDataList[i].CharacterId != charId;
					if (!flag2)
					{
						obj = this.characterScroll.GetActiveCell(i);
						break;
					}
				}
				bool flag3 = obj == null;
				if (!flag3)
				{
					CharacterMenuCharacterTransferInfo.TransferData transferData = new CharacterMenuCharacterTransferInfo.TransferData();
					transferData.OriginalFavor = this._beforeTransferCharacterDisplayData.FavorabilityToTaiwu;
					transferData.OriginalHappiness = this._beforeTransferCharacterDisplayData.Happiness;
					transferData.OriginalAlertness = this._beforeTransferCharacterDisplayData.Alertness;
					transferData.FinalFavor = this._afterTransferCharacterDisplayData.FavorabilityToTaiwu;
					transferData.FinalHappiness = this._afterTransferCharacterDisplayData.Happiness;
					transferData.FinalAlertness = this._afterTransferCharacterDisplayData.Alertness;
					CharacterMenuSelectableCharacterNormal item = obj.GetComponent<CharacterMenuSelectableCharacterNormal>();
					item.SetTransferInfo(itemData, transferData);
				}
			}
		}

		// Token: 0x060092EC RID: 37612 RVA: 0x0044714D File Offset: 0x0044534D
		private void OnRelationButtonClick(ArgumentBox argBox)
		{
			CharacterDomainMethod.Call.GetCharacterDisplayData(this.Element.GameDataListenerId, this.CurCharacterId);
		}

		// Token: 0x060092ED RID: 37613 RVA: 0x00447168 File Offset: 0x00445368
		private void OnFunctionLockStateChange(ArgumentBox argBox)
		{
			byte functionId;
			bool flag = argBox.Get("FunctionId", out functionId) && functionId == 20;
			if (flag)
			{
				CToggle informationToggle = this.mainToggleGroup.GetToggle(ECharacterSubToggleBase.InformationBase);
				informationToggle.gameObject.SetActive(SingletonObject.getInstance<FunctionLockManager>().IsFunctionUnlock(functionId));
			}
		}

		// Token: 0x060092EE RID: 37614 RVA: 0x004471B8 File Offset: 0x004453B8
		private void OnRefreshCharacterHealUIBottom(ArgumentBox args)
		{
			this.attributeAndInjury.CharacterId = this.CurCharacterId;
		}

		// Token: 0x060092EF RID: 37615 RVA: 0x004471D0 File Offset: 0x004453D0
		public void SelectCharacter(int charId)
		{
			IReadOnlyList<CharacterDisplayData> list = this.DisplayCharacters;
			int index = -1;
			bool flag = list != null;
			if (flag)
			{
				for (int i = 0; i < list.Count; i++)
				{
					bool flag2 = list[i] != null && list[i].CharacterId == charId;
					if (flag2)
					{
						index = i;
						break;
					}
				}
			}
			bool flag3 = index >= 0;
			if (flag3)
			{
				this._curCharacterIdIndex = index;
				this.characterScroll.ReRender();
				this.characterScroll.ScrollTo(this._curCharacterIdIndex, 0.3f);
				SubPageIndex currentIndex = new SubPageIndex
				{
					SubToggleIndex = ViewCharacterMenu.CurSubToggleIndex,
					SubSubPageIndex = ViewCharacterMenu.CurSubSubPageIndex
				};
				SubPageIndex fallbackIndex;
				bool needFallback = this.SetTogglesInteractable(currentIndex, out fallbackIndex);
				bool flag4 = needFallback;
				if (flag4)
				{
					this.ApplyPageIndex(fallbackIndex, true);
					this.RefreshToggleAndPage();
				}
			}
		}

		// Token: 0x060092F0 RID: 37616 RVA: 0x004472B4 File Offset: 0x004454B4
		private void OnEnterTransferItemMode(ArgumentBox argBox)
		{
			ItemDisplayData itemDisplayData;
			bool flag = !argBox.Get<ItemDisplayData>("ItemDisplayData", out itemDisplayData);
			if (!flag)
			{
				Transform maskToFocusGroupedItemList;
				bool flag2 = !argBox.Get<Transform>("MaskToFocusGroupedItemList", out maskToFocusGroupedItemList);
				if (!flag2)
				{
					this.RefreshCharacterScrollByPridiction((CharacterDisplayData data) => !this.IsTaiwuBeastTeammate(data.CharacterId) && AgeGroup.GetAgeGroup(data.PhysiologicalAge) > 0);
					List<Transform> focusTransList = new List<Transform>();
					Transform charListTrans = this.characterScrollNode.transform;
					focusTransList.Add(charListTrans);
					this._transferFocusList.Clear();
					this._transferFocusList.Add(new ValueTuple<Transform, Transform, int>(charListTrans, this.childPages, charListTrans.GetSiblingIndex()));
					this.OnTryClosePage = delegate()
					{
						this.ExitTransferItemMode();
						this.OnTryClosePage = null;
					};
					charListTrans.SetParent(maskToFocusGroupedItemList, true);
					this._transferItemData = itemDisplayData;
				}
			}
		}

		// Token: 0x060092F1 RID: 37617 RVA: 0x00447370 File Offset: 0x00445570
		public void FocusCharList()
		{
			ViewCharacterMenu.<>c__DisplayClass102_0 CS$<>8__locals1 = new ViewCharacterMenu.<>c__DisplayClass102_0();
			CS$<>8__locals1.<>4__this = this;
			this._itemOperationType = ItemOperationType.EItemOperationType.Transfer;
			Transform charListTrans = this.characterScrollNode.transform;
			charListTrans.SetParent(this.maskToFocus.transform, true);
			CS$<>8__locals1.taiwuCharId = SingletonObject.getInstance<BasicGameData>().TaiwuCharId;
			this.RefreshCharacterScrollByPridiction((CharacterDisplayData data) => data.CharacterId != CS$<>8__locals1.taiwuCharId && !CS$<>8__locals1.<>4__this.IsTaiwuBeastTeammate(data.CharacterId) && AgeGroup.GetAgeGroup(data.PhysiologicalAge) > 0);
			this.characterScroll.OnRenderEnd += CS$<>8__locals1.<FocusCharList>g__Action|1;
			ArgumentBox argBox = EasyPool.Get<ArgumentBox>();
			List<CharacterDisplayData> filtered = this._filteredCharacterDisplayDataList;
			argBox.Set("CharacterId", (filtered.Count > 0 && filtered[0] != null) ? filtered[0].CharacterId : -1);
			GEvent.OnEvent(UiEvents.OnSelectTransferItemChar, argBox);
		}

		// Token: 0x060092F2 RID: 37618 RVA: 0x00447438 File Offset: 0x00445638
		public void StopFocusCharList()
		{
			this._itemOperationType = ItemOperationType.EItemOperationType.Invalid;
			Transform charListTrans = this.characterScrollNode.transform;
			charListTrans.SetParent(this.childPages, true);
			this.RefreshCharacterScrollByPridiction(null);
			this.characterScroll.OnRenderEnd += this.<StopFocusCharList>g__Action|103_0;
			this.characterScroll.ScrollTo(0, 0.3f);
		}

		// Token: 0x060092F3 RID: 37619 RVA: 0x0044749C File Offset: 0x0044569C
		private void OnConfirmTransferItemCharacter(int charId)
		{
			ArgumentBox argBox = EasyPool.Get<ArgumentBox>();
			argBox.SetObject("ItemDisplayData", this._transferItemData);
			argBox.Set("CharacterId", charId);
			GEvent.OnEvent(UiEvents.OnSelectTransferItemChar, argBox);
			Action onTryClosePage = this.OnTryClosePage;
			if (onTryClosePage != null)
			{
				onTryClosePage();
			}
		}

		// Token: 0x060092F4 RID: 37620 RVA: 0x004474F0 File Offset: 0x004456F0
		private void ExitTransferItemMode()
		{
			foreach (ValueTuple<Transform, Transform, int> tuple in this._transferFocusList)
			{
				tuple.Item1.SetParent(tuple.Item2, true);
				tuple.Item1.SetSiblingIndex(tuple.Item3);
			}
			this._transferItemData = null;
			this.ExitFocusMode(true);
			this.RefreshCharacterScrollByPridiction(null);
		}

		// Token: 0x060092F5 RID: 37621 RVA: 0x00447580 File Offset: 0x00445780
		private void OnConfirmTransferMultiplyItemCharacter(int charId)
		{
			ArgumentBox argBox = EasyPool.Get<ArgumentBox>();
			argBox.Set("CharacterId", charId);
			GEvent.OnEvent(UiEvents.OnSelectTransferItemChar, argBox);
		}

		// Token: 0x060092F6 RID: 37622 RVA: 0x004475B0 File Offset: 0x004457B0
		private void OnChangeCharacterClothing(ArgumentBox argumentBox)
		{
			int characterId;
			argumentBox.Get("CharacterId", out characterId);
			bool flag = characterId != this.CurCharacterId;
			if (!flag)
			{
				CharacterDomainMethod.Call.GetCharacterDisplayData(this.Element.GameDataListenerId, characterId);
			}
		}

		// Token: 0x060092F7 RID: 37623 RVA: 0x004475F0 File Offset: 0x004457F0
		private void OnCharacterDisplayDataUpdated(CharacterDisplayData updatedData)
		{
			bool flag;
			if (updatedData != null)
			{
				ViewCharacterMenuDisplayData viewCharacterMenuDisplayData = this._viewCharacterMenuDisplayData;
				flag = (((viewCharacterMenuDisplayData != null) ? viewCharacterMenuDisplayData.CharacterDisplayDataList : null) == null);
			}
			else
			{
				flag = true;
			}
			bool flag2 = flag;
			if (!flag2)
			{
				List<CharacterDisplayData> sourceList = this._viewCharacterMenuDisplayData.CharacterDisplayDataList;
				for (int i = 0; i < sourceList.Count; i++)
				{
					CharacterDisplayData characterDisplayData = sourceList[i];
					int? num = (characterDisplayData != null) ? new int?(characterDisplayData.CharacterId) : null;
					int characterId = updatedData.CharacterId;
					bool flag3 = num.GetValueOrDefault() == characterId & num != null;
					if (flag3)
					{
						sourceList[i] = updatedData;
						break;
					}
				}
				for (int j = 0; j < this._filteredCharacterDisplayDataList.Count; j++)
				{
					CharacterDisplayData characterDisplayData2 = this._filteredCharacterDisplayDataList[j];
					int? num = (characterDisplayData2 != null) ? new int?(characterDisplayData2.CharacterId) : null;
					int characterId = updatedData.CharacterId;
					bool flag4 = num.GetValueOrDefault() == characterId & num != null;
					if (flag4)
					{
						this._filteredCharacterDisplayDataList[j] = updatedData;
						break;
					}
				}
				bool activeInHierarchy = this.characterScroll.gameObject.activeInHierarchy;
				if (activeInHierarchy)
				{
					this.characterScroll.ReRender();
				}
			}
		}

		// Token: 0x060092F8 RID: 37624 RVA: 0x00447735 File Offset: 0x00445935
		public void RerenderCharacterScroll()
		{
			this.characterScroll.ReRender();
		}

		// Token: 0x060092F9 RID: 37625 RVA: 0x00447744 File Offset: 0x00445944
		public CScrollRect GetCharacterScrollRect()
		{
			return this.characterScroll.Scroll;
		}

		// Token: 0x060092FA RID: 37626 RVA: 0x00447764 File Offset: 0x00445964
		private bool GetCanViewSocial()
		{
			CharacterDisplayData displayData;
			bool flag = this.TryGetDisplayData(this.CurCharacterId, out displayData);
			return flag && displayData.CreatingType == 1;
		}

		// Token: 0x060092FB RID: 37627 RVA: 0x00447798 File Offset: 0x00445998
		private bool GetCanViewRelation()
		{
			CharacterDisplayData displayData;
			bool flag = this.TryGetDisplayData(this.CurCharacterId, out displayData);
			return flag && displayData.TemplateId != 1114;
		}

		// Token: 0x060092FC RID: 37628 RVA: 0x004477D0 File Offset: 0x004459D0
		private bool GetCanViewLifeRecord()
		{
			ViewCharacterMenuDisplayData viewCharacterMenuDisplayData = this._viewCharacterMenuDisplayData;
			return ((viewCharacterMenuDisplayData != null) ? viewCharacterMenuDisplayData.NoNameInfantCharIds : null) == null || !this._viewCharacterMenuDisplayData.NoNameInfantCharIds.Contains(this.CurCharacterId);
		}

		// Token: 0x060092FD RID: 37629 RVA: 0x00447814 File Offset: 0x00445A14
		public void TransitionToStackCharacterState(ViewCharacterMenuStack.StackCharacterState state, bool isUseList = false)
		{
			this._canOperate = state.CanOperate;
			this._characterViewStack.Pop();
			StackViewContext newContext = new StackViewContext
			{
				Index = new SubPageIndex
				{
					SubToggleIndex = (ECharacterSubToggleBase)state.PageId,
					SubSubPageIndex = (ECharacterSubPage)state.SubPageId
				},
				Context = new ViewContext
				{
					charId = state.CharacterId,
					charIdList = state.CharacterIdList.ToList<int>()
				},
				CanOperate = state.CanOperate
			};
			this._characterViewStack.Push(newContext);
			this._forceRefreshSubPage = true;
			this.RequestByStack();
		}

		// Token: 0x060092FE RID: 37630 RVA: 0x004478C8 File Offset: 0x00445AC8
		public ViewCharacterMenuStack.StackCharacterState MakeStackCharacterState()
		{
			int curSubPage = (int)ViewCharacterMenu.CurSubToggleIndex;
			bool flag = curSubPage < 0 || this._allSubPages == null || curSubPage >= this._allSubPages.Length;
			if (flag)
			{
				curSubPage = 0;
			}
			return new ViewCharacterMenuStack.StackCharacterState
			{
				CharacterId = this.CurCharacterId,
				CharacterIdList = this.GetDisplayCharacterIds().ToArray(),
				CanOperate = this.CanOperate,
				PageId = curSubPage,
				SubPageId = (int)ViewCharacterMenu.CurSubSubPageIndex
			};
		}

		// Token: 0x060092FF RID: 37631 RVA: 0x0044794C File Offset: 0x00445B4C
		private bool TryStackBackCharacterView()
		{
			bool flag = this._characterViewStack.Count <= 1;
			bool result;
			if (flag)
			{
				result = false;
			}
			else
			{
				this._characterViewStack.Pop();
				this._forceRefreshSubPage = true;
				this.RequestByStack();
				result = true;
			}
			return result;
		}

		// Token: 0x06009300 RID: 37632 RVA: 0x00447994 File Offset: 0x00445B94
		private bool CanClosePageInTutorialChapter7()
		{
			int i = 0;
			int max = this._allSubPages.Length;
			while (i < max)
			{
				ViewCharacterMenuNeili targetPage = this._allSubPages[i] as ViewCharacterMenuNeili;
				bool flag = targetPage != null;
				if (flag)
				{
					return targetPage.NeiliAllocateOkForTutorial7();
				}
				i++;
			}
			return true;
		}

		// Token: 0x06009301 RID: 37633 RVA: 0x004479E8 File Offset: 0x00445BE8
		public void StackToNewCharacter(int newCharId)
		{
			StackViewContext newContext = new StackViewContext
			{
				Index = this._characterViewStack.Peek().Index,
				Context = new ViewContext
				{
					charId = newCharId,
					charIdList = null
				},
				CanOperate = false
			};
			this._characterViewStack.Push(newContext);
			this._forceRefreshSubPage = true;
			this.RequestByStack();
		}

		// Token: 0x06009302 RID: 37634 RVA: 0x00447A5C File Offset: 0x00445C5C
		public void SetBaseAttributeState(sbyte state)
		{
			switch (state)
			{
			case -2:
				this.attributeAndInjury.gameObject.SetActive(true);
				break;
			case -1:
				this.attributeAndInjury.gameObject.SetActive(false);
				break;
			case 0:
				this.attributeAndInjury.toggleGroup.Init(-1);
				this.attributeAndInjury.SwitchToAttribute();
				break;
			case 1:
				this.attributeAndInjury.toggleGroup.Init(-1);
				this.attributeAndInjury.SwitchToInjury();
				break;
			}
		}

		// Token: 0x06009303 RID: 37635 RVA: 0x00447AF4 File Offset: 0x00445CF4
		public bool HasAttributeToTopical(ItemKey itemKey)
		{
			return this.Injury.HasAttributeToTopical(itemKey);
		}

		// Token: 0x06009304 RID: 37636 RVA: 0x00447B14 File Offset: 0x00445D14
		public void SetEatItemInfectNotice(bool show, ITradeableContent itemDisplayData = null, int amount = 1)
		{
			bool flag = show && itemDisplayData != null;
			if (flag)
			{
				this.Injury.ShowInfectNotice(itemDisplayData, amount);
				this.Injury.ShowEatNotice(itemDisplayData, amount);
			}
			else
			{
				this.Injury.HideNotice(true, true);
			}
		}

		// Token: 0x06009305 RID: 37637 RVA: 0x00447B60 File Offset: 0x00445D60
		public void SetCurPageSubpage(int index)
		{
			ECharacterSubToggleBase parentKey = this.mainToggleGroup.CurrentParentKey;
			this.mainToggleGroup.SyncCurrentSubPage(parentKey, (ECharacterSubPage)index);
		}

		// Token: 0x06009306 RID: 37638 RVA: 0x00447B88 File Offset: 0x00445D88
		public void EnterFocusMode(Transform[] toFocusElements = null, bool isInteractable = true, bool isHideBtnClose = false)
		{
			if (isHideBtnClose)
			{
				this.closeButton.gameObject.SetActive(false);
			}
			this.maskToFocus.interactable = isInteractable;
			UIManager.Instance.MaskComponent((RectTransform)this.maskToFocus.transform);
			UI_CharacterMenuSubPageBase uiBase;
			int num;
			this.GetRealPageBase(ViewCharacterMenu.CurSubToggleIndex, ViewCharacterMenu.CurSubSubPageIndex, out uiBase, out num);
			uiBase.OnEnterFocusMode(this.maskToFocus.transform);
			bool flag = toFocusElements != null;
			if (flag)
			{
				foreach (Transform toFocusElement in toFocusElements)
				{
					toFocusElement.SetParent(this.maskToFocus.transform, true);
					toFocusElement.GetSiblingIndex();
					toFocusElement.SetAsLastSibling();
					toFocusElement.GetSiblingIndex();
				}
			}
			Canvas.ForceUpdateCanvases();
		}

		// Token: 0x06009307 RID: 37639 RVA: 0x00447C58 File Offset: 0x00445E58
		public void ExitFocusMode(bool isInteractable = true)
		{
			this.closeButton.gameObject.SetActive(true);
			bool flag = this._itemOperationType == ItemOperationType.EItemOperationType.Transfer;
			if (flag)
			{
				this.StopFocusCharList();
			}
			this.maskToFocus.interactable = isInteractable;
			UIManager.Instance.UnMaskComponent((RectTransform)this.maskToFocus.transform);
			UI_CharacterMenuSubPageBase uiBase;
			int num;
			this.GetRealPageBase(ViewCharacterMenu.CurSubToggleIndex, ViewCharacterMenu.CurSubSubPageIndex, out uiBase, out num);
			uiBase.OnExitFocusMode();
		}

		// Token: 0x06009308 RID: 37640 RVA: 0x00447CD0 File Offset: 0x00445ED0
		public void SetCharacterListAlpha(float alpha)
		{
			this.characterScrollNode.GetComponent<CanvasGroup>().alpha = alpha;
		}

		// Token: 0x06009309 RID: 37641 RVA: 0x00447CE5 File Offset: 0x00445EE5
		private void OnRestoreCharacterMenuAndSubPages(ArgumentBox _)
		{
			this.RestoreCharacterMenuAndSubPages();
		}

		// Token: 0x0600930A RID: 37642 RVA: 0x00447CEF File Offset: 0x00445EEF
		private void OnHideCharacterMenuAndSubPages(ArgumentBox _)
		{
			this.HideCharacterMenuAndSubPages();
		}

		// Token: 0x0600930B RID: 37643 RVA: 0x00447CFC File Offset: 0x00445EFC
		public void HideCharacterMenuAndSubPages()
		{
			this._subPageVisibleDict.Clear();
			foreach (CharacterMenuSubPageElement element in this._initedSubPageElements)
			{
				UI_CharacterMenuSubPageBase subPage = (element != null) ? element.UiBaseAs<UI_CharacterMenuSubPageBase>() : null;
				bool flag = subPage != null;
				if (flag)
				{
					this._subPageVisibleDict[subPage] = new ViewCharacterMenu.SubPageActiveState
					{
						ActiveSelf = subPage.gameObject.activeSelf,
						CanvasEnabled = subPage.GetComponent<Canvas>().enabled
					};
					subPage.gameObject.SetActive(false);
				}
			}
		}

		// Token: 0x0600930C RID: 37644 RVA: 0x00447DBC File Offset: 0x00445FBC
		public void RestoreCharacterMenuAndSubPages()
		{
			foreach (KeyValuePair<UI_CharacterMenuSubPageBase, ViewCharacterMenu.SubPageActiveState> kv in this._subPageVisibleDict)
			{
				UI_CharacterMenuSubPageBase subPage = kv.Key;
				ViewCharacterMenu.SubPageActiveState state = kv.Value;
				bool flag = subPage != null;
				if (flag)
				{
					subPage.gameObject.SetActive(state.ActiveSelf);
					bool activeSelf = state.ActiveSelf;
					if (activeSelf)
					{
						Canvas canvas = subPage.GetComponent<Canvas>();
						canvas.enabled = state.CanvasEnabled;
						bool canvasEnabled = state.CanvasEnabled;
						if (canvasEnabled)
						{
							subPage.GetComponent<RectTransform>().localPosition = Vector3.zero;
						}
					}
				}
			}
		}

		// Token: 0x0600930D RID: 37645 RVA: 0x00447E84 File Offset: 0x00446084
		public void GotoCharacterWithoutStackView(CharacterDisplayDataForRelations charData, string relationText = null)
		{
			ViewCharacterMenu.<>c__DisplayClass132_0 CS$<>8__locals1 = new ViewCharacterMenu.<>c__DisplayClass132_0();
			CS$<>8__locals1.charData = charData;
			CS$<>8__locals1.<>4__this = this;
			CS$<>8__locals1.stackViewInfo = this.MakeStackCharacterState();
			CS$<>8__locals1.stackViewInfo.RelationText = relationText;
			CS$<>8__locals1.stackViewInfo.CharacterId = CS$<>8__locals1.charData.CharacterId;
			CS$<>8__locals1.stackViewInfo.CanOperate = false;
			CS$<>8__locals1.stackViewInfo.Name = NameCenter.GetMonasticTitleOrDisplayName(ref CS$<>8__locals1.charData.Main.NameData, this.IsTaiwu(CS$<>8__locals1.charData.CharacterId), false);
			CS$<>8__locals1.stackViewInfo.AvatarRefresher = delegate(Game.Components.Avatar.Avatar avatar)
			{
				CommonUtils.CheckForAvatarExtraInfo(CS$<>8__locals1.charData.CharacterId, CS$<>8__locals1.charData.Main.AvatarRelatedData.AvatarData, ref CS$<>8__locals1.charData.Main.AvatarRelatedData.ClothingDisplayId);
				avatar.Refresh(CS$<>8__locals1.charData.Main.AvatarRelatedData, CS$<>8__locals1.charData.Main.NameData.CharTemplateId);
				ArgumentBox args = new ArgumentBox();
				args.SetObject("TargetPageIndex", ECharacterSubToggleBase.RelationshipBase);
				GEvent.OnEvent(UiEvents.OnNeedOpenCharacterMenuSubPage, args);
			};
			ViewCharacterMenuDisplayData viewCharacterMenuDisplayData = this._viewCharacterMenuDisplayData;
			bool flag;
			if (((viewCharacterMenuDisplayData != null) ? viewCharacterMenuDisplayData.TaiwuTeamCharIds : null) == null || !this._viewCharacterMenuDisplayData.TaiwuTeamCharIds.Contains(CS$<>8__locals1.charData.CharacterId))
			{
				ViewCharacterMenuDisplayData viewCharacterMenuDisplayData2 = this._viewCharacterMenuDisplayData;
				flag = (((viewCharacterMenuDisplayData2 != null) ? viewCharacterMenuDisplayData2.TaiwuSpecialGroup : null) != null && this._viewCharacterMenuDisplayData.TaiwuSpecialGroup.Contains(CS$<>8__locals1.charData.CharacterId));
			}
			else
			{
				flag = true;
			}
			bool flag2 = flag;
			if (flag2)
			{
				ViewCharacterMenu.<>c__DisplayClass132_0 CS$<>8__locals2 = CS$<>8__locals1;
				ViewCharacterMenuDisplayData viewCharacterMenuDisplayData3 = this._viewCharacterMenuDisplayData;
				CS$<>8__locals2.stackViewInfo.CharacterIdList = ((((viewCharacterMenuDisplayData3 != null) ? viewCharacterMenuDisplayData3.TaiwuTeamCharIds : null) != null) ? this._viewCharacterMenuDisplayData.TaiwuTeamCharIds.ToArray() : Array.Empty<int>());
				CS$<>8__locals1.<GotoCharacterWithoutStackView>g__Finish|1();
			}
			else
			{
				UIManager.Instance.ShowUI(UIElement.BlockInteract, true);
				CharacterDomainMethod.AsyncCall.GetViewCharacterMenuDisplayData(this, CS$<>8__locals1.charData.CharacterId, null, delegate(int offset, RawDataPool pool)
				{
					ViewCharacterMenuDisplayData displayData = new ViewCharacterMenuDisplayData();
					Serializer.Deserialize(pool, offset, ref displayData);
					bool flag3 = displayData.CharacterDisplayDataList != null;
					if (flag3)
					{
						List<int> tmp = new List<int>(displayData.CharacterDisplayDataList.Count);
						for (int i = 0; i < displayData.CharacterDisplayDataList.Count; i++)
						{
							CharacterDisplayData item = displayData.CharacterDisplayDataList[i];
							bool flag4 = item != null;
							if (flag4)
							{
								tmp.Add(item.CharacterId);
							}
						}
						CS$<>8__locals1.stackViewInfo.CharacterIdList = tmp.ToArray();
					}
					else
					{
						CS$<>8__locals1.stackViewInfo.CharacterIdList = Array.Empty<int>();
					}
					base.<GotoCharacterWithoutStackView>g__Finish|1();
					UIManager.Instance.HideUI(UIElement.BlockInteract);
				});
			}
		}

		// Token: 0x0600930E RID: 37646 RVA: 0x00448008 File Offset: 0x00446208
		public void GotoCharacterWithStackView(CharacterDisplayDataForRelations charData, string relationText = null)
		{
			this.StackView.PushAndActivate(this, charData.CharacterId, NameCenter.GetMonasticTitleOrDisplayName(ref charData.Main.NameData, this.IsTaiwu(charData.CharacterId), false), delegate(Game.Components.Avatar.Avatar avatar)
			{
				CommonUtils.CheckForAvatarExtraInfo(charData.CharacterId, charData.Main.AvatarRelatedData.AvatarData, ref charData.Main.AvatarRelatedData.ClothingDisplayId);
				avatar.Refresh(charData.Main.AvatarRelatedData, charData.Main.NameData.CharTemplateId);
			}, relationText);
		}

		// Token: 0x0600930F RID: 37647 RVA: 0x00448070 File Offset: 0x00446270
		private void InitializeTabDropdownComponent()
		{
			bool tabDropdownInited = this._tabDropdownInited;
			if (!tabDropdownInited)
			{
				this.mainToggleGroup.Initialize(ViewCharacterMenu.SubToggleInfo, (int)ViewCharacterMenu.CurSubToggleIndex, new Action<ECharacterSubToggleBase>(this.HandleParentToggleSelected), new Action<ECharacterSubToggleBase, ECharacterSubPage>(this.HandleSubPageButtonClicked), new Func<ECharacterSubPage, bool>(this.CheckCanSubpageShow));
				this._tabDropdownInited = true;
			}
		}

		// Token: 0x06009310 RID: 37648 RVA: 0x004480CC File Offset: 0x004462CC
		private void HandleParentToggleSelected(ECharacterSubToggleBase parentKey)
		{
			bool flag = ViewCharacterMenu.CurSubToggleIndex == parentKey;
			if (!flag)
			{
				ECharacterSubPage subPage = this.GetRememberedDisplaySubpage(parentKey);
				this.ApplyPageIndex(new SubPageIndex
				{
					SubToggleIndex = parentKey,
					SubSubPageIndex = subPage
				}, true);
				this.RefreshToggleAndPage();
			}
		}

		// Token: 0x06009311 RID: 37649 RVA: 0x00448118 File Offset: 0x00446318
		private void HandleSubPageButtonClicked(ECharacterSubToggleBase parentKey, ECharacterSubPage subPage)
		{
			bool flag = subPage == ViewCharacterMenu.CurSubSubPageIndex && parentKey == ViewCharacterMenu.CurSubToggleIndex;
			if (!flag)
			{
				this.ApplyPageIndex(new SubPageIndex
				{
					SubToggleIndex = parentKey,
					SubSubPageIndex = subPage
				}, true);
				this.RefreshToggleAndPage();
			}
		}

		// Token: 0x06009312 RID: 37650 RVA: 0x00448168 File Offset: 0x00446368
		private void UpdateInformationTabVisibility()
		{
			bool flag = this.mainToggleGroup == null;
			if (!flag)
			{
				CToggle infoToggle = this.mainToggleGroup.GetToggle(ECharacterSubToggleBase.InformationBase);
				bool flag2 = infoToggle == null;
				if (!flag2)
				{
					bool unlocked = SingletonObject.getInstance<FunctionLockManager>().IsFunctionUnlock(20);
					infoToggle.gameObject.SetActive(unlocked);
				}
			}
		}

		// Token: 0x06009313 RID: 37651 RVA: 0x004481C0 File Offset: 0x004463C0
		private ECharacterSubPage GetRememberedDisplaySubpage(ECharacterSubToggleBase parentKey)
		{
			ECharacterSubPage subPage;
			bool flag = this._lastSelectedSubPageByToggle.TryGetValue(parentKey, out subPage);
			if (flag)
			{
				List<ValueTuple<string, ECharacterSubPage>> visibleInfo = this.GetVisibleSubPageInfo(parentKey);
				bool flag2 = visibleInfo != null && visibleInfo.Count > 0;
				if (flag2)
				{
					for (int i = 0; i < visibleInfo.Count; i++)
					{
						bool flag3 = visibleInfo[i].Item2 == subPage;
						if (flag3)
						{
							return subPage;
						}
					}
				}
			}
			return this.GetDefaultDisplaySubpage(parentKey);
		}

		// Token: 0x06009314 RID: 37652 RVA: 0x00448244 File Offset: 0x00446444
		[return: TupleElementNames(new string[]
		{
			"name",
			"originalIndex"
		})]
		private List<ValueTuple<string, ECharacterSubPage>> GetVisibleSubPageInfo(ECharacterSubToggleBase parentKey)
		{
			List<ValueTuple<string, ECharacterSubPage>> cachedInfo;
			bool flag = this._visibleSubPageCache.TryGetValue(parentKey, out cachedInfo);
			List<ValueTuple<string, ECharacterSubPage>> result;
			if (flag)
			{
				result = cachedInfo;
			}
			else
			{
				List<ValueTuple<string, ECharacterSubPage>> visibleInfo = new List<ValueTuple<string, ECharacterSubPage>>();
				LanguageKey[] subPageDisplayNameList = ViewCharacterMenu.SubToggleInfo[(int)parentKey].SubPageNames;
				bool flag2 = subPageDisplayNameList != null && subPageDisplayNameList.Length > 0;
				if (flag2)
				{
					for (int i = 0; i < subPageDisplayNameList.Length; i++)
					{
						bool flag3 = this.CheckCanSubpageShow(ViewCharacterMenu.SubToggleInfo[(int)parentKey].SubPages[i]);
						if (flag3)
						{
							visibleInfo.Add(new ValueTuple<string, ECharacterSubPage>(LocalStringManager.Get(subPageDisplayNameList[i]), ViewCharacterMenu.SubToggleInfo[(int)parentKey].SubPages[i]));
						}
					}
				}
				this._visibleSubPageCache[parentKey] = visibleInfo;
				result = visibleInfo;
			}
			return result;
		}

		// Token: 0x06009315 RID: 37653 RVA: 0x00448304 File Offset: 0x00446504
		private bool CheckCanSubpageShow(ECharacterSubPage eCharacterSubPage)
		{
			return eCharacterSubPage != ECharacterSubPage.Prison || SingletonObject.getInstance<FunctionLockManager>().IsFunctionUnlock(19);
		}

		// Token: 0x06009316 RID: 37654 RVA: 0x00448330 File Offset: 0x00446530
		private void GetRealPageElement(ECharacterSubToggleBase subTogglePage, ECharacterSubPage targetSubPage, out CharacterMenuSubPageElement subToggleUIElement)
		{
			subToggleUIElement = ((targetSubPage == ECharacterSubPage.None) ? this.GetSubToggleUIElement(subTogglePage) : this.GetSubPageUIElement(targetSubPage));
		}

		// Token: 0x06009317 RID: 37655 RVA: 0x00448348 File Offset: 0x00446548
		private void GetRealPageBase(ECharacterSubToggleBase subTogglePage, ECharacterSubPage targetSubPage, out UI_CharacterMenuSubPageBase subToggleUIBase, out int innerIndex)
		{
			innerIndex = -1;
			bool flag = targetSubPage == ECharacterSubPage.None;
			if (flag)
			{
				subToggleUIBase = this.GetSubToggleUIBase(subTogglePage);
				if (subTogglePage != ECharacterSubToggleBase.ItemBase)
				{
					if (subTogglePage == ECharacterSubToggleBase.EquipCombatSkillBase)
					{
						innerIndex = 1;
					}
				}
				else
				{
					innerIndex = 0;
				}
			}
			else
			{
				subToggleUIBase = this.GetSubPageUIBase(targetSubPage);
				switch (targetSubPage)
				{
				case ECharacterSubPage.Attainment:
					innerIndex = 0;
					break;
				case ECharacterSubPage.Information:
				case ECharacterSubPage.Equipment:
					innerIndex = 0;
					break;
				case ECharacterSubPage.Secret:
				case ECharacterSubPage.Vehicle:
					innerIndex = 1;
					break;
				}
			}
		}

		// Token: 0x06009318 RID: 37656 RVA: 0x004483DC File Offset: 0x004465DC
		private void OpenTargetPage(ArgumentBox argBox)
		{
			ECharacterSubPage targetSubPage;
			bool flag = argBox.Get<ECharacterSubPage>("TargetSubPageIndex", out targetSubPage) && targetSubPage > ECharacterSubPage.None;
			ECharacterSubToggleBase targetSubToggle;
			if (flag)
			{
				targetSubToggle = this.GetSubTogglekeyBySubPage(targetSubPage);
			}
			else
			{
				bool flag2 = argBox.Get<ECharacterSubToggleBase>("TargetPageIndex", out targetSubToggle);
				if (!flag2)
				{
					return;
				}
				targetSubPage = this.GetDefaultDisplaySubpage(targetSubToggle);
			}
			this.ApplyPageIndex(new SubPageIndex
			{
				SubToggleIndex = targetSubToggle,
				SubSubPageIndex = targetSubPage
			}, true);
			this.RefreshToggleAndPage();
			int baseAttributeIndex;
			bool flag3 = argBox.Get("BaseAttributeIndex", out baseAttributeIndex);
			if (flag3)
			{
				this.SetBaseAttributeState((sbyte)baseAttributeIndex);
			}
		}

		// Token: 0x06009319 RID: 37657 RVA: 0x00448478 File Offset: 0x00446678
		public void MoveCharacterScrollLeft(float duration = 0.5f, Ease ease = Ease.OutExpo)
		{
			RectTransform rectTransform = this.characterScrollNode.GetComponent<RectTransform>();
			bool flag = duration <= 0.0001f;
			if (flag)
			{
				rectTransform.DOKill(false);
				Vector2 pos = rectTransform.anchoredPosition;
				pos.x = -400f;
				rectTransform.anchoredPosition = pos;
			}
			else
			{
				rectTransform.DOAnchorPosX(-400f, duration, false).SetEase(ease);
			}
		}

		// Token: 0x0600931A RID: 37658 RVA: 0x004484DC File Offset: 0x004466DC
		public void MoveCharacterScrollBack(float duration = 0.5f, Ease ease = Ease.OutExpo)
		{
			RectTransform rectTransform = this.characterScrollNode.GetComponent<RectTransform>();
			bool flag = duration <= 0.0001f;
			if (flag)
			{
				rectTransform.DOKill(false);
				Vector2 pos = rectTransform.anchoredPosition;
				pos.x = 99f;
				rectTransform.anchoredPosition = pos;
			}
			else
			{
				rectTransform.DOAnchorPosX(99f, duration, false).SetEase(ease);
			}
		}

		// Token: 0x0600931B RID: 37659 RVA: 0x00448540 File Offset: 0x00446740
		public bool TryGetFunctionControlConfig(out CharacterMenuFunctionControlItem config)
		{
			bool flag = this._characterControlTemplateId >= 0;
			bool result;
			if (flag)
			{
				config = CharacterMenuFunctionControl.Instance[this._characterControlTemplateId];
				result = true;
			}
			else
			{
				config = null;
				result = false;
			}
			return result;
		}

		// Token: 0x0600931C RID: 37660 RVA: 0x00448580 File Offset: 0x00446780
		public bool IsFunctionBanned(ECharacterMenuFunctionControlType type)
		{
			return (type & this.CharacterControlType) > ECharacterMenuFunctionControlType.None;
		}

		// Token: 0x0600931D RID: 37661 RVA: 0x004485A0 File Offset: 0x004467A0
		private void SetStackViewSorting(int sortingOrder)
		{
			GameObject target = this.StackView.gameObject;
			Canvas canvas = target.GetComponent<Canvas>();
			GraphicRaycaster raycaster = target.GetComponent<GraphicRaycaster>();
			canvas.enabled = true;
			canvas.overrideSorting = true;
			canvas.sortingLayerName = "UI";
			canvas.sortingOrder = sortingOrder;
			canvas.additionalShaderChannels = (AdditionalCanvasShaderChannels.TexCoord1 | AdditionalCanvasShaderChannels.TexCoord2 | AdditionalCanvasShaderChannels.Normal | AdditionalCanvasShaderChannels.Tangent);
			raycaster.enabled = true;
		}

		// Token: 0x0600931E RID: 37662 RVA: 0x00448600 File Offset: 0x00446800
		private void CancelStackViewSorting()
		{
			GameObject target = this.StackView.gameObject;
			Canvas canvas = target.GetComponent<Canvas>();
			canvas.overrideSorting = false;
		}

		// Token: 0x0600931F RID: 37663 RVA: 0x0044862C File Offset: 0x0044682C
		public void SetSorting(int sortingOrder)
		{
			this.SetTargetSorting(this.mainToggleGroup.gameObject, sortingOrder);
			this.SetTargetSorting(this.childPages.gameObject, sortingOrder);
			this.SetStackViewSorting(sortingOrder + 1);
			this.SetTargetSorting(this.titleObj, sortingOrder);
		}

		// Token: 0x06009320 RID: 37664 RVA: 0x00448678 File Offset: 0x00446878
		public void CancelSorting()
		{
			this.CancelTargetSorting(this.mainToggleGroup.gameObject);
			this.CancelTargetSorting(this.childPages.gameObject);
			this.CancelStackViewSorting();
			this.CancelTargetSorting(this.titleObj);
		}

		// Token: 0x06009321 RID: 37665 RVA: 0x004486B4 File Offset: 0x004468B4
		private void SetTargetSorting(GameObject target, int sortingOrder)
		{
			ValueTuple<Canvas, GraphicRaycaster> cached;
			bool flag = !this._canvasCache.TryGetValue(target, out cached);
			if (flag)
			{
				Canvas canvas = target.GetComponent<Canvas>();
				bool flag2 = canvas == null;
				if (flag2)
				{
					canvas = target.AddComponent<Canvas>();
				}
				GraphicRaycaster raycaster = target.GetComponent<GraphicRaycaster>();
				bool flag3 = raycaster == null;
				if (flag3)
				{
					raycaster = target.AddComponent<GraphicRaycaster>();
				}
				cached = new ValueTuple<Canvas, GraphicRaycaster>(canvas, raycaster);
				this._canvasCache[target] = cached;
			}
			cached.Item1.enabled = true;
			cached.Item1.overrideSorting = true;
			cached.Item1.sortingLayerName = "UI";
			cached.Item1.sortingOrder = sortingOrder;
			cached.Item1.additionalShaderChannels = (AdditionalCanvasShaderChannels.TexCoord1 | AdditionalCanvasShaderChannels.TexCoord2 | AdditionalCanvasShaderChannels.Normal | AdditionalCanvasShaderChannels.Tangent);
			cached.Item2.enabled = true;
		}

		// Token: 0x06009322 RID: 37666 RVA: 0x00448780 File Offset: 0x00446980
		private void CancelTargetSorting(GameObject target)
		{
			ValueTuple<Canvas, GraphicRaycaster> cached;
			bool flag = this._canvasCache.TryGetValue(target, out cached);
			if (flag)
			{
				bool flag2 = cached.Item2 != null;
				if (flag2)
				{
					Object.DestroyImmediate(cached.Item2);
				}
				bool flag3 = cached.Item1 != null;
				if (flag3)
				{
					Object.DestroyImmediate(cached.Item1);
				}
				this._canvasCache.Remove(target);
			}
		}

		// Token: 0x06009323 RID: 37667 RVA: 0x004487EC File Offset: 0x004469EC
		private bool TryGetDisplayData(int charId, out CharacterDisplayData displayData)
		{
			foreach (CharacterDisplayData data in this._viewCharacterMenuDisplayData.CharacterDisplayDataList)
			{
				bool flag = data.CharacterId == charId;
				if (flag)
				{
					displayData = data;
					return true;
				}
			}
			displayData = null;
			return false;
		}

		// Token: 0x06009324 RID: 37668 RVA: 0x00448860 File Offset: 0x00446A60
		private static bool IsNegativeCommand(sbyte arg)
		{
			return arg >= 0 && Config.TeammateCommand.Instance[arg].Type == ETeammateCommandType.Negative;
		}

		// Token: 0x06009325 RID: 37669 RVA: 0x0044888C File Offset: 0x00446A8C
		public void SetChildPagesVisible(bool visible)
		{
			bool flag = this.childPages == null;
			if (!flag)
			{
				if (visible)
				{
					bool childPagesHidden = this._childPagesHidden;
					if (childPagesHidden)
					{
						this.childPages.anchoredPosition = Vector2.zero;
						this._childPagesHidden = false;
					}
				}
				else
				{
					bool flag2 = !this._childPagesHidden;
					if (flag2)
					{
						this.childPages.anchoredPosition = new Vector2(10000f, 10000f);
						this._childPagesHidden = true;
					}
				}
			}
		}

		// Token: 0x06009326 RID: 37670 RVA: 0x0044890C File Offset: 0x00446B0C
		public void SwitchToSubToggle(ECharacterSubToggleBase subTogglePage)
		{
			this.ApplyPageIndex(new SubPageIndex
			{
				SubToggleIndex = subTogglePage,
				SubSubPageIndex = this.GetDefaultDisplaySubpage(subTogglePage)
			}, true);
			this.RefreshToggleAndPage();
		}

		// Token: 0x06009327 RID: 37671 RVA: 0x00448948 File Offset: 0x00446B48
		private void RefreshToggleAndPage()
		{
			this._visibleSubPageCache.Clear();
			CharacterMenuToggleGroup characterMenuToggleGroup = this.mainToggleGroup;
			if (characterMenuToggleGroup != null)
			{
				characterMenuToggleGroup.SyncCurrentSubPage(ViewCharacterMenu.CurSubToggleIndex, ViewCharacterMenu.CurSubSubPageIndex);
			}
			this.characterScrollNode.SetActive(ViewCharacterMenu.CurSubSubPageIndex != ECharacterSubPage.Team);
			LanguageKey languageKey = this.GetTitleLanguageKey(ViewCharacterMenu.CurSubToggleIndex, ViewCharacterMenu.CurSubSubPageIndex);
			this.subPageTitle.text = LocalStringManager.Get(languageKey);
			Transform stackViewTransform = this.StackView.transform;
			stackViewTransform.SetParent(base.transform.parent, true);
			stackViewTransform.SetAsLastSibling();
			this.RefreshSubTogglePage();
			this.RefreshSubPage();
		}

		// Token: 0x06009328 RID: 37672 RVA: 0x004489EC File Offset: 0x00446BEC
		private void RefreshSubPage()
		{
			UI_CharacterMenuSubPageBase subToggleUIBase;
			int innerIndex;
			this.GetRealPageBase(ViewCharacterMenu.CurSubToggleIndex, ViewCharacterMenu.CurSubSubPageIndex, out subToggleUIBase, out innerIndex);
			subToggleUIBase.ResetMoveInAnim();
			bool flag = innerIndex >= 0;
			if (flag)
			{
				subToggleUIBase.OnSwitchToSubpage(innerIndex);
			}
		}

		// Token: 0x06009329 RID: 37673 RVA: 0x00448A2C File Offset: 0x00446C2C
		private UI_CharacterMenuSubPageBase GetSubPageUIBase(ECharacterSubPage key)
		{
			int realIndex = this.GetRealPageIndexBySubPage(key);
			bool flag = realIndex < 0 || realIndex >= this._allSubPages.Length;
			UI_CharacterMenuSubPageBase result;
			if (flag)
			{
				result = this._allSubPages[0];
			}
			else
			{
				result = this._allSubPages[realIndex];
			}
			return result;
		}

		// Token: 0x0600932A RID: 37674 RVA: 0x00448A74 File Offset: 0x00446C74
		private UI_CharacterMenuSubPageBase GetSubToggleUIBase(ECharacterSubToggleBase key)
		{
			int realIndex = this.GetRealPageIndexBySubToggle(key);
			bool flag = realIndex < 0 || realIndex >= this._allSubPages.Length;
			UI_CharacterMenuSubPageBase result;
			if (flag)
			{
				result = this._allSubPages[0];
			}
			else
			{
				result = this._allSubPages[realIndex];
			}
			return result;
		}

		// Token: 0x0600932B RID: 37675 RVA: 0x00448ABC File Offset: 0x00446CBC
		private CharacterMenuSubPageElement GetSubPageUIElement(ECharacterSubPage key)
		{
			CharacterMenuSubPageElement result;
			switch (key)
			{
			case ECharacterSubPage.Character:
				result = UIElement.CharacterMenuInfo;
				break;
			case ECharacterSubPage.Team:
				result = UIElement.CharacterMenuTeam;
				break;
			case ECharacterSubPage.Prison:
				result = UIElement.CharacterMenuKidnap;
				break;
			case ECharacterSubPage.Attainment:
				result = UIElement.CharacterMennAttaiment;
				break;
			case ECharacterSubPage.AttainmentCombatSkill:
				result = UIElement.CharacterMenuCombatSkill;
				break;
			case ECharacterSubPage.AttainmentLifeSkill:
				result = UIElement.CharacterMenuLifeSkill;
				break;
			case ECharacterSubPage.Relationship:
				result = UIElement.CharacterMenuRelationShip;
				break;
			case ECharacterSubPage.Genealogy:
				result = UIElement.CharacterMenuGenealogy;
				break;
			case ECharacterSubPage.Information:
				result = UIElement.CharacterMenuInformation;
				break;
			case ECharacterSubPage.Secret:
				result = UIElement.CharacterMenuSecretInformation;
				break;
			case ECharacterSubPage.Equipment:
				result = UIElement.CharacterMenuEquip;
				break;
			case ECharacterSubPage.Vehicle:
				result = UIElement.CharacterMenuEquip;
				break;
			default:
				result = null;
				break;
			}
			return result;
		}

		// Token: 0x0600932C RID: 37676 RVA: 0x00448B70 File Offset: 0x00446D70
		private CharacterMenuSubPageElement GetSubToggleUIElement(ECharacterSubToggleBase key)
		{
			switch (key)
			{
			case ECharacterSubToggleBase.EquipmentBase:
				return UIElement.CharacterMenuEquip;
			case ECharacterSubToggleBase.ItemBase:
				return UIElement.CharacterMenuItems;
			case ECharacterSubToggleBase.PracticeBase:
				return UIElement.CharacterMenuPractice;
			case ECharacterSubToggleBase.NeiliBase:
				return UIElement.CharacterMenuNeili;
			case ECharacterSubToggleBase.EquipCombatSkillBase:
				return UIElement.CharacterMenuEquipCombatSkill;
			case ECharacterSubToggleBase.RelationshipBase:
				return UIElement.CharacterMenuRelationShip;
			case ECharacterSubToggleBase.StoryBase:
				return UIElement.CharacterMenuLifeRecords;
			}
			return null;
		}

		// Token: 0x0600932D RID: 37677 RVA: 0x00448BEC File Offset: 0x00446DEC
		private int GetRealPageIndexBySubPage(ECharacterSubPage key)
		{
			CharacterMenuSubPageElement subPageElement = this.GetSubPageUIElement(key);
			bool flag = subPageElement == null;
			int result;
			if (flag)
			{
				result = -1;
			}
			else
			{
				result = ViewCharacterMenu._allSubPageElements.IndexOf(subPageElement);
			}
			return result;
		}

		// Token: 0x0600932E RID: 37678 RVA: 0x00448C20 File Offset: 0x00446E20
		private int GetRealPageIndexBySubToggle(ECharacterSubToggleBase key)
		{
			CharacterMenuSubPageElement subToggleElement = this.GetSubToggleUIElement(key);
			bool flag = subToggleElement == null;
			int result;
			if (flag)
			{
				result = -1;
			}
			else
			{
				result = ViewCharacterMenu._allSubPageElements.IndexOf(subToggleElement);
			}
			return result;
		}

		// Token: 0x0600932F RID: 37679 RVA: 0x00448C54 File Offset: 0x00446E54
		private void SwitchToSubPage(ECharacterSubPage subPageKey)
		{
			bool flag = subPageKey == ECharacterSubPage.None;
			if (!flag)
			{
				this.ApplyPageIndex(new SubPageIndex
				{
					SubToggleIndex = this.GetSubTogglekeyBySubPage(subPageKey),
					SubSubPageIndex = subPageKey
				}, true);
				this.RefreshToggleAndPage();
			}
		}

		// Token: 0x06009330 RID: 37680 RVA: 0x00448C9C File Offset: 0x00446E9C
		private ECharacterSubToggleBase GetSubTogglekeyBySubPage(ECharacterSubPage subPageKey)
		{
			foreach (ViewCharacterMenu.SubTogglePageInfo item in ViewCharacterMenu.SubToggleInfo)
			{
				bool flag = item.SubPages != null && item.SubPages.IndexOf(subPageKey) >= 0;
				if (flag)
				{
					return item.SubToggleType;
				}
			}
			throw new KeyNotFoundException(string.Format("未找到{0}对应的子页签", subPageKey));
		}

		// Token: 0x06009331 RID: 37681 RVA: 0x00448D0C File Offset: 0x00446F0C
		private ECharacterSubPage GetDefaultDisplaySubpage(ECharacterSubToggleBase subTogglePage)
		{
			List<ValueTuple<string, ECharacterSubPage>> visiblePageInfo = this.GetVisibleSubPageInfo(subTogglePage);
			bool flag = visiblePageInfo == null || visiblePageInfo.Count == 0;
			ECharacterSubPage result;
			if (flag)
			{
				result = ECharacterSubPage.None;
			}
			else
			{
				result = visiblePageInfo[0].Item2;
			}
			return result;
		}

		// Token: 0x06009333 RID: 37683 RVA: 0x00448DD4 File Offset: 0x00446FD4
		// Note: this type is marked as 'beforefieldinit'.
		static ViewCharacterMenu()
		{
			ViewCharacterMenu.SubTogglePageInfo[] array = new ViewCharacterMenu.SubTogglePageInfo[10];
			int num = 0;
			ViewCharacterMenu.SubTogglePageInfo subTogglePageInfo = new ViewCharacterMenu.SubTogglePageInfo();
			subTogglePageInfo.SubToggleType = ECharacterSubToggleBase.CharacterBase;
			subTogglePageInfo.SubPages = new ECharacterSubPage[]
			{
				ECharacterSubPage.Character,
				ECharacterSubPage.Prison,
				ECharacterSubPage.Team
			};
			ViewCharacterMenu.SubTogglePageInfo subTogglePageInfo2 = subTogglePageInfo;
			LanguageKey[] array2 = new LanguageKey[3];
			RuntimeHelpers.InitializeArray(array2, fieldof(<PrivateImplementationDetails>.158E83FD55A0F9C06D4DBC7F076D847F43B1CA896878CDDFF6B998CB695054A6).FieldHandle);
			subTogglePageInfo2.SubPageNames = array2;
			subTogglePageInfo.TitleKey = LanguageKey.LK_CharacterMenu_Tog_Char;
			array[num] = subTogglePageInfo;
			array[1] = new ViewCharacterMenu.SubTogglePageInfo
			{
				SubToggleType = ECharacterSubToggleBase.EquipmentBase,
				SubPages = new ECharacterSubPage[]
				{
					ECharacterSubPage.Equipment,
					ECharacterSubPage.Vehicle
				},
				SubPageNames = new LanguageKey[]
				{
					LanguageKey.LK_CharacterMenu_Title_Equip,
					LanguageKey.LK_CarHorse
				},
				TitleKey = LanguageKey.LK_CharacterMenu_Title_Equip
			};
			array[2] = new ViewCharacterMenu.SubTogglePageInfo
			{
				SubToggleType = ECharacterSubToggleBase.ItemBase,
				SubPages = new ECharacterSubPage[0],
				SubPageNames = new LanguageKey[0],
				TitleKey = LanguageKey.LK_CharacterMenu_Tog_Item
			};
			int num2 = 3;
			subTogglePageInfo = new ViewCharacterMenu.SubTogglePageInfo();
			subTogglePageInfo.SubToggleType = ECharacterSubToggleBase.AttainmentBase;
			subTogglePageInfo.SubPages = new ECharacterSubPage[]
			{
				ECharacterSubPage.Attainment,
				ECharacterSubPage.AttainmentCombatSkill,
				ECharacterSubPage.AttainmentLifeSkill
			};
			ViewCharacterMenu.SubTogglePageInfo subTogglePageInfo3 = subTogglePageInfo;
			LanguageKey[] array3 = new LanguageKey[3];
			RuntimeHelpers.InitializeArray(array3, fieldof(<PrivateImplementationDetails>.01B3C8AF2043C0EBE3A13481B05977779DCCAD9D32729EF6251FD76A239C79F6).FieldHandle);
			subTogglePageInfo3.SubPageNames = array3;
			subTogglePageInfo.TitleKey = LanguageKey.LK_CharacterMenu_Tog_Attainment;
			array[num2] = subTogglePageInfo;
			array[4] = new ViewCharacterMenu.SubTogglePageInfo
			{
				SubToggleType = ECharacterSubToggleBase.PracticeBase,
				SubPages = new ECharacterSubPage[0],
				SubPageNames = new LanguageKey[0],
				TitleKey = LanguageKey.LK_CharacterMenu_Tog_Break
			};
			array[5] = new ViewCharacterMenu.SubTogglePageInfo
			{
				SubToggleType = ECharacterSubToggleBase.NeiliBase,
				SubPages = new ECharacterSubPage[0],
				SubPageNames = new LanguageKey[0],
				TitleKey = LanguageKey.LK_CharacterMenu_Tog_Neili
			};
			array[6] = new ViewCharacterMenu.SubTogglePageInfo
			{
				SubToggleType = ECharacterSubToggleBase.EquipCombatSkillBase,
				SubPages = new ECharacterSubPage[0],
				SubPageNames = new LanguageKey[0],
				TitleKey = LanguageKey.LK_CharacterMenu_Tog_EquipSkill
			};
			array[7] = new ViewCharacterMenu.SubTogglePageInfo
			{
				SubToggleType = ECharacterSubToggleBase.RelationshipBase,
				SubPages = new ECharacterSubPage[]
				{
					ECharacterSubPage.Relationship,
					ECharacterSubPage.Genealogy
				},
				SubPageNames = new LanguageKey[]
				{
					LanguageKey.LK_RelationShip,
					LanguageKey.LK_Genealogy
				},
				TitleKey = LanguageKey.LK_CharacterMenu_Tog_Relation
			};
			array[8] = new ViewCharacterMenu.SubTogglePageInfo
			{
				SubToggleType = ECharacterSubToggleBase.StoryBase,
				SubPages = new ECharacterSubPage[0],
				SubPageNames = new LanguageKey[0],
				TitleKey = LanguageKey.LK_CharacterMenu_Tog_Story
			};
			array[9] = new ViewCharacterMenu.SubTogglePageInfo
			{
				SubToggleType = ECharacterSubToggleBase.InformationBase,
				SubPages = new ECharacterSubPage[]
				{
					ECharacterSubPage.Information,
					ECharacterSubPage.Secret
				},
				SubPageNames = new LanguageKey[]
				{
					LanguageKey.LK_CharacterMenu_Tog_Information,
					LanguageKey.LK_CharacterMenu_Tog_SecretInformation
				},
				TitleKey = LanguageKey.LK_CharacterMenu_Tog_Information
			};
			ViewCharacterMenu.SubToggleInfo = array;
		}

		// Token: 0x06009335 RID: 37685 RVA: 0x00449110 File Offset: 0x00447310
		[CompilerGenerated]
		internal static bool <SetTogglesInteractable>g__IsToggleInteractable|85_0(ECharacterSubToggleBase toggle, ref ViewCharacterMenu.<>c__DisplayClass85_0 A_1)
		{
			bool isInTutorial = A_1.isInTutorial;
			if (isInTutorial)
			{
				bool flag = toggle == ECharacterSubToggleBase.AttainmentBase;
				if (flag)
				{
					return A_1.tutorialModel.GetFunctionStatus(3);
				}
				bool flag2 = toggle == ECharacterSubToggleBase.PracticeBase;
				if (flag2)
				{
					return A_1.tutorialModel.GetFunctionStatus(7);
				}
				bool flag3 = toggle == ECharacterSubToggleBase.NeiliBase;
				if (flag3)
				{
					return A_1.tutorialModel.GetFunctionStatus(5);
				}
				bool flag4 = toggle == ECharacterSubToggleBase.EquipCombatSkillBase;
				if (flag4)
				{
					return A_1.tutorialModel.GetFunctionStatus(4);
				}
				bool flag5 = toggle == ECharacterSubToggleBase.RelationshipBase;
				if (flag5)
				{
					return A_1.tutorialModel.GetFunctionStatus(6);
				}
				bool flag6 = toggle == ECharacterSubToggleBase.StoryBase;
				if (flag6)
				{
					return A_1.tutorialModel.GetFunctionStatus(9);
				}
				bool flag7 = toggle == ECharacterSubToggleBase.InformationBase;
				if (flag7)
				{
					return A_1.tutorialModel.GetFunctionStatus(10);
				}
			}
			bool flag8 = (toggle == ECharacterSubToggleBase.RelationshipBase || toggle == ECharacterSubToggleBase.StoryBase || toggle == ECharacterSubToggleBase.InformationBase) && !A_1.canViewSocial;
			bool result;
			if (flag8)
			{
				result = false;
			}
			else
			{
				bool flag9 = toggle == ECharacterSubToggleBase.RelationshipBase && !A_1.canViewRelation;
				if (flag9)
				{
					result = false;
				}
				else
				{
					bool flag10 = toggle == ECharacterSubToggleBase.StoryBase && !A_1.canViewLifeRecord;
					result = !flag10;
				}
			}
			return result;
		}

		// Token: 0x06009338 RID: 37688 RVA: 0x00449278 File Offset: 0x00447478
		[CompilerGenerated]
		private void <StopFocusCharList>g__Action|103_0()
		{
			IReadOnlyList<CharacterDisplayData> list = this.DisplayCharacters;
			bool flag = list != null && list.Count > 0 && list[0] != null;
			if (flag)
			{
				this.SelectCharacter(list[0].CharacterId);
			}
			this.characterScroll.OnRenderEnd -= this.<StopFocusCharList>g__Action|103_0;
			this.RefreshSubTogglePage();
		}

		// Token: 0x04007122 RID: 28962
		private bool _useAnonymousName;

		// Token: 0x04007123 RID: 28963
		private Dictionary<int, List<sbyte>> _teamReplacedTeammateCommands;

		// Token: 0x04007124 RID: 28964
		private SubPageIndex _targetPageIndex;

		// Token: 0x04007125 RID: 28965
		private UI_CharacterMenuSubPageBase[] _allSubPages;

		// Token: 0x04007126 RID: 28966
		public static ECharacterSubToggleBase CurSubToggleIndex;

		// Token: 0x04007127 RID: 28967
		public static ECharacterSubPage CurSubSubPageIndex;

		// Token: 0x04007128 RID: 28968
		private Stack<StackViewContext> _characterViewStack;

		// Token: 0x04007129 RID: 28969
		public ViewCharacterMenuStack StackView;

		// Token: 0x0400712A RID: 28970
		public static bool NeedClear;

		// Token: 0x0400712B RID: 28971
		private bool _canOperate;

		// Token: 0x0400712C RID: 28972
		private int _curCharacterIdIndex;

		// Token: 0x0400712D RID: 28973
		private ViewCharacterMenuDisplayData _viewCharacterMenuDisplayData;

		// Token: 0x0400712E RID: 28974
		private readonly List<CharacterDisplayData> _filteredCharacterDisplayDataList = new List<CharacterDisplayData>();

		// Token: 0x0400712F RID: 28975
		[TupleElementNames(new string[]
		{
			"canvas",
			"raycaster"
		})]
		private readonly Dictionary<GameObject, ValueTuple<Canvas, GraphicRaycaster>> _canvasCache = new Dictionary<GameObject, ValueTuple<Canvas, GraphicRaycaster>>();

		// Token: 0x04007130 RID: 28976
		private short _characterControlTemplateId = -1;

		// Token: 0x04007132 RID: 28978
		[NonSerialized]
		public Action OnTryClosePage;

		// Token: 0x04007133 RID: 28979
		[NonSerialized]
		public Action<Action> OnTryClosePageWithCallback;

		// Token: 0x04007134 RID: 28980
		[SerializeField]
		private DOTweenAnimation moveIn;

		// Token: 0x04007135 RID: 28981
		private bool _playedInitAnim;

		// Token: 0x04007136 RID: 28982
		private ItemDisplayData _transferItemData;

		// Token: 0x04007137 RID: 28983
		private CharacterDisplayData _beforeTransferCharacterDisplayData;

		// Token: 0x04007138 RID: 28984
		private CharacterDisplayData _afterTransferCharacterDisplayData;

		// Token: 0x04007139 RID: 28985
		public bool IsMultiplySelect;

		// Token: 0x0400713A RID: 28986
		private readonly List<ValueTuple<Transform, Transform, int>> _transferFocusList = new List<ValueTuple<Transform, Transform, int>>();

		// Token: 0x0400713B RID: 28987
		[NonSerialized]
		public Action<Action> OnTryChangeCharacter;

		// Token: 0x0400713C RID: 28988
		private ItemOperationType.EItemOperationType _itemOperationType = ItemOperationType.EItemOperationType.Invalid;

		// Token: 0x0400713D RID: 28989
		private ArgumentBox _initArgBox;

		// Token: 0x0400713E RID: 28990
		private bool _tabDropdownInited;

		// Token: 0x0400713F RID: 28991
		private bool _forceRefreshSubPage;

		// Token: 0x04007140 RID: 28992
		private readonly HashSet<CharacterMenuSubPageElement> _initedSubPageElements = new HashSet<CharacterMenuSubPageElement>();

		// Token: 0x04007141 RID: 28993
		private readonly Dictionary<UI_CharacterMenuSubPageBase, ViewCharacterMenu.SubPageActiveState> _subPageVisibleDict = new Dictionary<UI_CharacterMenuSubPageBase, ViewCharacterMenu.SubPageActiveState>();

		// Token: 0x04007142 RID: 28994
		[TupleElementNames(new string[]
		{
			"name",
			"originalIndex"
		})]
		private readonly Dictionary<ECharacterSubToggleBase, List<ValueTuple<string, ECharacterSubPage>>> _visibleSubPageCache = new Dictionary<ECharacterSubToggleBase, List<ValueTuple<string, ECharacterSubPage>>>();

		// Token: 0x04007143 RID: 28995
		private readonly Dictionary<ECharacterSubToggleBase, ECharacterSubPage> _lastSelectedSubPageByToggle = new Dictionary<ECharacterSubToggleBase, ECharacterSubPage>();

		// Token: 0x04007144 RID: 28996
		private readonly WaitForEndOfFrame _waitFrame = new WaitForEndOfFrame();

		// Token: 0x04007145 RID: 28997
		[SerializeField]
		private InfinityScroll characterScroll;

		// Token: 0x04007146 RID: 28998
		[SerializeField]
		private GameObject characterScrollNode;

		// Token: 0x04007147 RID: 28999
		[SerializeField]
		private CharacterMenuToggleGroup mainToggleGroup;

		// Token: 0x04007148 RID: 29000
		[SerializeField]
		private TextMeshProUGUI subPageTitle;

		// Token: 0x04007149 RID: 29001
		[SerializeField]
		private CButton maskToFocus;

		// Token: 0x0400714A RID: 29002
		[SerializeField]
		private CButton closeButton;

		// Token: 0x0400714B RID: 29003
		[SerializeField]
		private RectTransform childPages;

		// Token: 0x0400714C RID: 29004
		[SerializeField]
		private TextMeshProUGUI teammateCount;

		// Token: 0x0400714D RID: 29005
		[SerializeField]
		private AttributeAndInjuryDynamic attributeAndInjury;

		// Token: 0x0400714E RID: 29006
		[SerializeField]
		private CharacterMenuLocalLoadingAnim localLoadingAnim;

		// Token: 0x0400714F RID: 29007
		[SerializeField]
		private GameObject titleObj;

		// Token: 0x04007150 RID: 29008
		private const float HiddenOffset = 10000f;

		// Token: 0x04007151 RID: 29009
		private bool _childPagesHidden;

		// Token: 0x04007152 RID: 29010
		public sbyte CurrentSelectedLifeSkillType = 0;

		// Token: 0x04007153 RID: 29011
		public sbyte CurrentSelectedCombatSkillType = 0;

		// Token: 0x04007154 RID: 29012
		public const float CharacterScrollMoveDuration = 0.5f;

		// Token: 0x04007155 RID: 29013
		private const float CharacterScrollMoveDistance = 400f;

		// Token: 0x04007156 RID: 29014
		private const float CharacterScrollOriginPosX = 99f;

		// Token: 0x04007157 RID: 29015
		public static readonly CharacterMenuSubPageElement[] _allSubPageElements = new CharacterMenuSubPageElement[]
		{
			UIElement.CharacterMenuInfo,
			UIElement.CharacterMenuTeam,
			UIElement.CharacterMenuEquip,
			UIElement.CharacterMenuItems,
			UIElement.CharacterMennAttaiment,
			UIElement.CharacterMenuPractice,
			UIElement.CharacterMenuEquipCombatSkill,
			UIElement.CharacterMenuRelationShip,
			UIElement.CharacterMenuLifeRecords,
			UIElement.CharacterMenuInformation,
			UIElement.CharacterMenuLifeSkill,
			UIElement.CharacterMenuCombatSkill,
			UIElement.CharacterMenuNeili,
			UIElement.CharacterMenuKidnap,
			UIElement.CharacterMenuSecretInformation,
			UIElement.CharacterMenuGenealogy
		};

		// Token: 0x04007158 RID: 29016
		private static readonly ViewCharacterMenu.SubTogglePageInfo[] SubToggleInfo;

		// Token: 0x020021B5 RID: 8629
		private struct SubPageActiveState
		{
			// Token: 0x0400D6CF RID: 54991
			public bool ActiveSelf;

			// Token: 0x0400D6D0 RID: 54992
			public bool CanvasEnabled;
		}

		// Token: 0x020021B6 RID: 8630
		public class SubTogglePageInfo
		{
			// Token: 0x0400D6D1 RID: 54993
			public ECharacterSubToggleBase SubToggleType;

			// Token: 0x0400D6D2 RID: 54994
			public ECharacterSubPage[] SubPages;

			// Token: 0x0400D6D3 RID: 54995
			public LanguageKey[] SubPageNames;

			// Token: 0x0400D6D4 RID: 54996
			public LanguageKey TitleKey;
		}
	}
}
