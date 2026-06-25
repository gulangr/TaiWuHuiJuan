using System;
using System.Collections.Generic;
using System.Linq;
using FrameWork;
using FrameWork.UISystem.UIElements;
using Game.Components.Character;
using Game.Components.Character.LifeRecord;
using Game.Views.CharacterMenu;
using Game.Views.MapBlockCharList;
using GameData.Domains.Character.AvatarSystem;
using GameData.Domains.Character.Display;
using GameData.Domains.Map;
using GameData.Domains.Taiwu;
using GameData.Serializer;
using GameData.Utilities;
using TMPro;
using UnityEngine;

namespace Game.Views.Following
{
	// Token: 0x02000A24 RID: 2596
	public class ViewFollowing : UIBase, IMapBlockCharDataSource, IMapBlockCharHolder, IMapBlockCharShortCutsParent
	{
		// Token: 0x06007F44 RID: 32580 RVA: 0x003B4961 File Offset: 0x003B2B61
		private void Awake()
		{
			this.quickHide.onClick.ResetListener(new Action(this.QuickHide));
			this.detailButton.onClick.ResetListener(new Action(this.OpenSelectedCharacterDetail));
		}

		// Token: 0x06007F45 RID: 32581 RVA: 0x003B499F File Offset: 0x003B2B9F
		private void OnEnable()
		{
			GEvent.Add(UiEvents.RefreshFollowing, new GEvent.Callback(this.RequestData));
		}

		// Token: 0x06007F46 RID: 32582 RVA: 0x003B49C0 File Offset: 0x003B2BC0
		private void OnDisable()
		{
			GEvent.Remove(UiEvents.RefreshFollowing, new GEvent.Callback(this.RequestData));
			this._data.Clear();
			this.charScroll.SetDataCount(0, false);
			this._filtered.Clear();
			this.lifeRecords.Clear();
			this.lifeRecords.gameObject.SetActive(false);
		}

		// Token: 0x06007F47 RID: 32583 RVA: 0x003B4A30 File Offset: 0x003B2C30
		public override void OnInit(ArgumentBox argsBox)
		{
			this.locationBase.SetActive(false);
			this.charScroll.Init(this);
			this.mapBlockButton.onClick.ResetListener(new Action(this.JumpToLocation));
			this.lifeRecords.gameObject.SetActive(false);
			this.Deselect();
			this.NeedDataListenerId = true;
			this.Element.OnListenerIdReady = new Action(this.RequestData);
			this.searchInput.onEndEdit.ResetListener(delegate(string _)
			{
				this.ApplyFilter();
			});
			this.searchInput.onValueChanged.ResetListener(delegate(string _)
			{
				this.ApplyFilter();
			});
		}

		// Token: 0x06007F48 RID: 32584 RVA: 0x003B4AE6 File Offset: 0x003B2CE6
		private void RequestData(ArgumentBox _)
		{
			this.RequestData();
		}

		// Token: 0x06007F49 RID: 32585 RVA: 0x003B4AEF File Offset: 0x003B2CEF
		public void Deselect()
		{
			this._selectedCharacterId = -1;
			this.RefreshUIState();
		}

		// Token: 0x06007F4A RID: 32586 RVA: 0x003B4B00 File Offset: 0x003B2D00
		public void OnItemRender(int index, GameObject obj)
		{
			bool flag = GameApp.Quiting || GameApp.ReadyToQuit;
			if (!flag)
			{
				Game.Views.MapBlockCharList.FollowingChar component = obj.GetComponent<Game.Views.MapBlockCharList.FollowingChar>();
				switch (this._filtered[index].Display.AliveState)
				{
				case 0:
					component.Set(this, this._filtered[index].Display, false, true);
					component.CharStatus = 0;
					break;
				case 1:
					component.SetAsGrave(this, this._filtered[index].Display);
					component.CharStatus = 1;
					break;
				case 2:
					component.Set(this, null);
					component.CharId = this._filtered[index].Display.CharacterId;
					component.CharStatus = 2;
					break;
				}
				component.Selected = (this._selectedCharacterId == component.CharId);
			}
		}

		// Token: 0x06007F4B RID: 32587 RVA: 0x003B4BE3 File Offset: 0x003B2DE3
		public void OnAliveCharacterClicked(int charId)
		{
			this._isDead = false;
			this.RefreshCommonData(charId);
		}

		// Token: 0x06007F4C RID: 32588 RVA: 0x003B4BF5 File Offset: 0x003B2DF5
		public void OnDeadCharacterClicked(int charId)
		{
			this._isDead = true;
			this.RefreshCommonData(charId);
		}

		// Token: 0x06007F4D RID: 32589 RVA: 0x003B4C08 File Offset: 0x003B2E08
		public void RefreshCommonData(int charId)
		{
			bool flag = this._selectedCharacterId != charId;
			if (flag)
			{
				this.lifeRecords.Set(this, this._selectedCharacterId = charId, false);
			}
			TaiwuFollowingDisplayData data = this._data.FirstOrDefault((TaiwuFollowingDisplayData x) => x.Display.CharacterId == charId);
			this._charLocation = ((data != null) ? data.Location.Location : Location.Invalid);
			this.locationItem.Refresh(((data != null) ? data.Location : null) ?? TaiwuFollowingDisplayData.InvalidLocation);
			WorldMapModel model = SingletonObject.getInstance<WorldMapModel>();
			this.mapBlockButton.interactable = (model.CurrentAreaId != -1 && ((model.CurrentStateId != -1) ? (model.GetStateId(this._charLocation.AreaId) == model.CurrentStateId) : (this._charLocation.AreaId == model.CurrentAreaId)) && !UIElement.AdventureRemake.Exist && !UIElement.AdventureMajorEvent.Exist);
			this.mapBlockButtonDisplayer.PresetParam[1] = (this.mapBlockButton.interactable ? "LK_CharacterLocationFind_Tips_Available" : "LK_CharacterLocationFind_Tips_Disabled");
			CharacterInjuryDisplayData injuryData;
			bool flag2;
			if (!this._isDead)
			{
				injuryData = ((data != null) ? data.Injury : null);
				flag2 = (injuryData != null);
			}
			else
			{
				flag2 = false;
			}
			bool flag3 = flag2;
			if (flag3)
			{
				this.injury.Set(injuryData, false);
				this.injury.gameObject.SetActive(true);
			}
			else
			{
				this.injury.gameObject.SetActive(false);
			}
			CharacterDisplayData displayData;
			bool flag4;
			if (this._isDead)
			{
				displayData = ((data != null) ? data.Display : null);
				flag4 = (displayData != null);
			}
			else
			{
				flag4 = false;
			}
			bool flag5 = flag4;
			if (flag5)
			{
				this.deadCharacter.Set(displayData);
				this._graveDuration = displayData.GraveDuration;
				this.deadCharacter.gameObject.SetActive(true);
			}
			else
			{
				this.deadCharacter.gameObject.SetActive(false);
				this._graveDuration = -1;
			}
			this.RefreshUIState();
		}

		// Token: 0x06007F4E RID: 32590 RVA: 0x003B4E14 File Offset: 0x003B3014
		private void RefreshUIState()
		{
			bool hasSelected = this._selectedCharacterId != -1;
			this.lifeRecords.gameObject.SetActive(hasSelected && (!this._isDead || this._charLocation.IsValid()));
			this.injury.gameObject.SetActive(hasSelected && !this._isDead);
			this.locationBase.SetActive(hasSelected);
			this.detailButton.gameObject.SetActive(hasSelected && (!this._isDead || this._graveDuration > 0));
			this.RefreshShortCutForSelected();
		}

		// Token: 0x17000DC4 RID: 3524
		// (get) Token: 0x06007F4F RID: 32591 RVA: 0x003B4EBB File Offset: 0x003B30BB
		private int TaiwuCharId
		{
			get
			{
				return SingletonObject.getInstance<BasicGameData>().TaiwuCharId;
			}
		}

		// Token: 0x17000DC5 RID: 3525
		// (get) Token: 0x06007F50 RID: 32592 RVA: 0x003B4EC7 File Offset: 0x003B30C7
		private CharacterMonitorModel _characterMonitorModel
		{
			get
			{
				return SingletonObject.getInstance<CharacterMonitorModel>();
			}
		}

		// Token: 0x06007F51 RID: 32593 RVA: 0x003B4ED0 File Offset: 0x003B30D0
		public void JumpToLocation()
		{
			bool activeSelf = base.gameObject.activeSelf;
			if (activeSelf)
			{
				this.QuickHide();
			}
			UIManager uiManager = UIManager.Instance;
			uiManager.HideAll();
			uiManager.ChangeToUI(UIElement.StateMainWorld);
			Location location = this._charLocation;
			bool flag = this._characterMonitorModel.IsTaiwuTeamCharacter(this._selectedCharacterId);
			if (flag)
			{
				location = SingletonObject.getInstance<WorldMapModel>().CurrentLocation;
			}
			SingletonObject.getInstance<WorldMapModel>().JumpToTemporaryMark(location, this._isDead ? 3 : 0);
		}

		// Token: 0x06007F52 RID: 32594 RVA: 0x003B4F4C File Offset: 0x003B314C
		public void OpenSelectedCharacterDetail()
		{
			bool isDead = this._isDead;
			if (isDead)
			{
				ViewLifeRecords.Show(this._selectedCharacterId);
			}
			else
			{
				UIElement.CharacterMenu.SetOnInitArgs(EasyPool.Get<ArgumentBox>().Set("CharacterId", this._selectedCharacterId).Set("PreviousView", 9).SetObject("ViewCharacterMenuTaretPage", new SubPageIndex(ECharacterSubToggleBase.StoryBase, ECharacterSubPage.None)));
				UIManager.Instance.ShowUI(UIElement.CharacterMenu, true);
			}
		}

		// Token: 0x06007F53 RID: 32595 RVA: 0x003B4FC6 File Offset: 0x003B31C6
		public bool CanClick(DisplayType type, int id)
		{
			return true;
		}

		// Token: 0x06007F54 RID: 32596 RVA: 0x003B4FCC File Offset: 0x003B31CC
		public void OnClick(DisplayType type, int id)
		{
			bool flag = !this._canClick;
			if (flag)
			{
				this._canClick = true;
			}
			else
			{
				TaiwuFollowingDisplayData charData = this._data.FirstOrDefault((TaiwuFollowingDisplayData x) => x.Display.CharacterId == id);
				bool flag2 = charData != null;
				if (flag2)
				{
					this.deadState.SetActive(charData.Display.AliveState == 2);
					this.aliveState.SetActive(charData.Display.AliveState != 2);
					bool flag3 = charData.Display.AliveState == 0;
					if (flag3)
					{
						this.OnAliveCharacterClicked(id);
					}
					else
					{
						bool flag4 = charData.Display.AliveState == 1;
						if (flag4)
						{
							this.OnDeadCharacterClicked(id);
						}
					}
				}
			}
		}

		// Token: 0x06007F55 RID: 32597 RVA: 0x003B50A0 File Offset: 0x003B32A0
		private void RefreshShortCutForSelected()
		{
			bool shortCutNeedRefresh = true;
			RectTransform scroll = this.charScroll.Content;
			int i = scroll.childCount;
			while (i-- > 0)
			{
				Game.Views.MapBlockCharList.FollowingChar mbc = scroll.GetChild(i).GetComponent<Game.Views.MapBlockCharList.FollowingChar>();
				if (mbc == null)
				{
					goto IL_41;
				}
				GameObject gameObject = mbc.gameObject;
				if (gameObject == null)
				{
					goto IL_41;
				}
				int num = gameObject.activeSelf ? 1 : 0;
				IL_42:
				bool flag = num == 0;
				if (flag)
				{
					continue;
				}
				mbc.Selected = (mbc.CharId == this._selectedCharacterId);
				bool flag2 = shortCutNeedRefresh && mbc.CharId == this._selectedCharacterId && this.shortcuts.gameObject.activeSelf;
				if (flag2)
				{
					this.OnHover(mbc.rectTransform, mbc);
					shortCutNeedRefresh = false;
				}
				continue;
				IL_41:
				num = 0;
				goto IL_42;
			}
			bool flag3 = shortCutNeedRefresh;
			if (flag3)
			{
				this.shortcuts.gameObject.SetActive(false);
			}
		}

		// Token: 0x06007F56 RID: 32598 RVA: 0x003B5176 File Offset: 0x003B3376
		private void RequestData()
		{
			TaiwuDomainMethod.AsyncCall.RequestFollowingCharacterList(this, delegate(int offset, RawDataPool pool)
			{
				this._data.Clear();
				Serializer.Deserialize(pool, offset, ref this._data);
				foreach (TaiwuFollowingDisplayData data in this._data)
				{
					CharacterDisplayData display = data.Display;
					bool flag = display != null && display.AliveState == 1 && display.GraveDuration <= 0 && data.Display.DeathDate < SingletonObject.getInstance<BasicGameData>().CurrDate - 12;
					if (flag)
					{
						data.Display = new CharacterDisplayData
						{
							CharacterId = data.Display.CharacterId,
							AliveState = 2,
							Location = Location.Invalid,
							AvatarRelatedData = new AvatarRelatedData
							{
								AvatarData = new AvatarData()
							},
							IsFollowedByTaiwu = true,
							NickNameId = -1,
							IdealSect = -1,
							GraveDuration = -1,
							MerchantTemplateId = -1,
							RelationToTaiwu = ushort.MaxValue,
							RelationFromTaiwu = ushort.MaxValue,
							SamsaraCount = -1,
							BountyPunishmentSeverity = -1,
							FeatureIds = new List<short>()
						};
					}
				}
				this.emptyState.gameObject.SetActive(this._data.Count == 0);
				this.content.gameObject.SetActive(this._data.Count != 0);
				bool flag2 = this._data.Count != 0;
				if (flag2)
				{
					this.ApplyFilter();
				}
				this.Element.ShowAfterRefresh();
			});
			TaiwuDomainMethod.AsyncCall.GetFollowingNpcListMaxCount(this, delegate(int offset, RawDataPool pool)
			{
				int maxCount = 0;
				Serializer.Deserialize(pool, offset, ref maxCount);
				this.charCount.text = string.Format("{0}/{1}", this._data.Count, maxCount);
			});
		}

		// Token: 0x06007F57 RID: 32599 RVA: 0x003B51A0 File Offset: 0x003B33A0
		private void ApplyFilter()
		{
			this._filtered.Clear();
			List<TaiwuFollowingDisplayData> filtered = this._filtered;
			IEnumerable<TaiwuFollowingDisplayData> collection;
			if (!string.IsNullOrWhiteSpace(this.searchInput.text))
			{
				collection = from x in this._data
				where NameCenter.GetMonasticTitleOrDisplayName(x.Display, false).Contains(this.searchInput.text)
				select x;
			}
			else
			{
				IEnumerable<TaiwuFollowingDisplayData> data = this._data;
				collection = data;
			}
			filtered.AddRange(collection);
			this.charScroll.SetDataCount(this._filtered.Count, false);
			bool flag = this._filtered.All((TaiwuFollowingDisplayData x) => x.Display.CharacterId != this._selectedCharacterId);
			if (flag)
			{
				bool flag2 = this._filtered.Count > 0;
				if (flag2)
				{
					this.OnClick(DisplayType.Normal, this._filtered[0].Display.CharacterId);
				}
				else
				{
					this.aliveState.gameObject.SetActive(false);
					this.deadCharacter.gameObject.SetActive(false);
					this._selectedCharacterId = -1;
				}
			}
			this.shortcuts.gameObject.SetActive(false);
		}

		// Token: 0x06007F58 RID: 32600 RVA: 0x003B52A0 File Offset: 0x003B34A0
		public void OnHover(RectTransform rectTransform, MapBlockChar charObj)
		{
			this.shortcuts.follower.Target = rectTransform;
			MapBlockCharShortCuts mapBlockCharShortCuts = this.shortcuts;
			int[] ie;
			if (charObj.CharStatus != 0)
			{
				(ie = new int[1])[0] = 4;
			}
			else
			{
				int[] array = new int[2];
				array[0] = 3;
				ie = array;
				array[1] = 4;
			}
			mapBlockCharShortCuts.Init(ie, charObj.CharId, this);
		}

		// Token: 0x06007F59 RID: 32601 RVA: 0x003B52F4 File Offset: 0x003B34F4
		public void OnHoverEnd()
		{
			this.shortcuts.gameObject.SetActive(false);
		}

		// Token: 0x06007F5A RID: 32602 RVA: 0x003B5309 File Offset: 0x003B3509
		public bool CanClick(int id, int charId)
		{
			return true;
		}

		// Token: 0x06007F5B RID: 32603 RVA: 0x003B530C File Offset: 0x003B350C
		public void OnClick(int id, int charId)
		{
			bool flag = id == 4;
			if (flag)
			{
				UIElement.Dialog.SetOnInitArgs(EasyPool.Get<ArgumentBox>().SetObject("Cmd", new DialogCmd
				{
					Title = LanguageKey.LK_UnFollowing_Confirm_Title.Tr(),
					Content = LanguageKey.LK_UnFollowing_Confirm_Desc.TrFormat(Array.Empty<object>()),
					Yes = delegate()
					{
						TaiwuDomainMethod.Call.TaiwuUnfollowNpc(charId);
						GEvent.OnEvent(UiEvents.NickNameChanged, EasyPool.Get<ArgumentBox>().Set("CharacterId", charId).Set("NickNameKey", -1).Set("NickName", ""));
						this.RequestData();
					}
				}));
				UIManager.Instance.MaskUI(UIElement.Dialog);
			}
			else
			{
				bool flag2 = id == 3;
				if (flag2)
				{
					RectTransform scroll = this.charScroll.Content;
					int i = scroll.childCount;
					while (i-- > 0)
					{
						Game.Views.MapBlockCharList.FollowingChar mbc = scroll.GetChild(i).GetComponent<Game.Views.MapBlockCharList.FollowingChar>();
						bool flag3 = mbc != null && mbc.CharId == charId;
						if (flag3)
						{
							new RenameCfg
							{
								Title = LanguageKey.LK_UI_Following_Rename_Title.Tr(),
								Description = LanguageKey.LK_UI_Following_Rename_Desc.TrFormat(NameCenter.GetMonasticTitleOrDisplayName(ref mbc.CharName, false, true)),
								EmptyDesc = LanguageKey.LK_UI_Following_Rename_Empty.Tr(),
								Default = mbc.nameText.text,
								Submit = new Action<string>(mbc.OnSubmitRenameDone),
								CharCount = 6
							}.Show();
							return;
						}
					}
					Debug.LogError(string.Format("cannot find char with id {0}", charId));
				}
			}
		}

		// Token: 0x17000DC6 RID: 3526
		// (get) Token: 0x06007F5C RID: 32604 RVA: 0x003B549A File Offset: 0x003B369A
		// (set) Token: 0x06007F5D RID: 32605 RVA: 0x003B54A2 File Offset: 0x003B36A2
		bool IMapBlockCharDataSource.CanClick
		{
			get
			{
				return this._canClick;
			}
			set
			{
				this._canClick = value;
			}
		}

		// Token: 0x04006167 RID: 24935
		[SerializeField]
		private CButton quickHide;

		// Token: 0x04006168 RID: 24936
		[SerializeField]
		private GameObject emptyState;

		// Token: 0x04006169 RID: 24937
		[SerializeField]
		private GameObject aliveState;

		// Token: 0x0400616A RID: 24938
		[SerializeField]
		private GameObject deadState;

		// Token: 0x0400616B RID: 24939
		[SerializeField]
		private GameObject content;

		// Token: 0x0400616C RID: 24940
		[SerializeField]
		private MapBlockCharScroll charScroll;

		// Token: 0x0400616D RID: 24941
		[SerializeField]
		private Injury injury;

		// Token: 0x0400616E RID: 24942
		[SerializeField]
		private DeadCharacterDisplay deadCharacter;

		// Token: 0x0400616F RID: 24943
		[SerializeField]
		private LifeRecord lifeRecords;

		// Token: 0x04006170 RID: 24944
		[SerializeField]
		private CButton detailButton;

		// Token: 0x04006171 RID: 24945
		[SerializeField]
		private CharacterLocationItem locationItem;

		// Token: 0x04006172 RID: 24946
		[SerializeField]
		private GameObject locationBase;

		// Token: 0x04006173 RID: 24947
		[SerializeField]
		private CButton mapBlockButton;

		// Token: 0x04006174 RID: 24948
		[SerializeField]
		private TooltipInvoker mapBlockButtonDisplayer;

		// Token: 0x04006175 RID: 24949
		private List<TaiwuFollowingDisplayData> _filtered = new List<TaiwuFollowingDisplayData>();

		// Token: 0x04006176 RID: 24950
		private List<TaiwuFollowingDisplayData> _data = new List<TaiwuFollowingDisplayData>();

		// Token: 0x04006177 RID: 24951
		private int _graveDuration = -1;

		// Token: 0x04006178 RID: 24952
		private int _selectedCharacterId = -1;

		// Token: 0x04006179 RID: 24953
		private Location _charLocation = Location.Invalid;

		// Token: 0x0400617A RID: 24954
		private bool _isDead = false;

		// Token: 0x0400617B RID: 24955
		private const int TogKeyNormal = 0;

		// Token: 0x0400617C RID: 24956
		private const int TogKeyGrave = 3;

		// Token: 0x0400617D RID: 24957
		[SerializeField]
		private TMP_Text charCount;

		// Token: 0x0400617E RID: 24958
		[SerializeField]
		private TMP_InputField searchInput;

		// Token: 0x0400617F RID: 24959
		[SerializeField]
		private MapBlockCharShortCuts shortcuts;

		// Token: 0x04006180 RID: 24960
		private bool _canClick;
	}
}
