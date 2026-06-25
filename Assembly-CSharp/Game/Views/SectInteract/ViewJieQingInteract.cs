using System;
using System.Collections.Generic;
using Coffee.UIExtensions;
using FrameWork;
using FrameWork.UISystem.UIElements;
using Game.Components.Common;
using GameData.Common;
using GameData.Domains.Building;
using GameData.Domains.Character;
using GameData.Domains.Character.Display;
using GameData.Domains.World;
using GameData.GameDataBridge;
using GameData.Serializer;
using GameData.Utilities;
using Spine;
using Spine.Unity;
using TMPro;
using UnityEngine;

namespace Game.Views.SectInteract
{
	// Token: 0x020009A6 RID: 2470
	public class ViewJieQingInteract : UIBase
	{
		// Token: 0x17000D58 RID: 3416
		// (get) Token: 0x06007706 RID: 30470 RVA: 0x003765A9 File Offset: 0x003747A9
		private bool _isGood
		{
			get
			{
				return this._pageState == ViewJieQingInteract.EJieQingInteractState.UseFortune;
			}
		}

		// Token: 0x17000D59 RID: 3417
		// (get) Token: 0x06007707 RID: 30471 RVA: 0x003765B4 File Offset: 0x003747B4
		private bool _isBad
		{
			get
			{
				return this._pageState == ViewJieQingInteract.EJieQingInteractState.MurderMap;
			}
		}

		// Token: 0x06007708 RID: 30472 RVA: 0x003765C0 File Offset: 0x003747C0
		public override void OnInit(ArgumentBox argsBox)
		{
			this.InitArguments(argsBox);
			bool flag = !this._inited;
			if (flag)
			{
				this._pageState = ViewJieQingInteract.EJieQingInteractState.UseFortune;
				this.pageToggleGroup.Init(0);
				ToggleGroupHotkeyController.Set(this.Element, this.pageToggleGroup, 0, null);
				this.fortuneLegacyPage.gameObject.SetActive(true);
				this.murderMapPage.gameObject.SetActive(false);
				this.pageToggleGroup.OnActiveIndexChange += delegate(int newTog, int oldTog)
				{
					this.fortuneLegacyPage.gameObject.SetActive(newTog == 0);
					this.murderMapPage.gameObject.SetActive(newTog == 1);
					this._pageState = ((newTog == 0) ? ViewJieQingInteract.EJieQingInteractState.UseFortune : ViewJieQingInteract.EJieQingInteractState.MurderMap);
					this.RefreshPageArtAssets();
				};
				this._inited = true;
			}
			this.fortuneLegacyPage.OnInit(this, this._inited, new Action<bool>(this.OnUseFortune));
			this.murderMapPage.OnInit(this, this._inited);
			this._taiwuCharId = SingletonObject.getInstance<BasicGameData>().TaiwuCharId;
			this.RefreshPageArtAssets();
			UIElement element = this.Element;
			element.OnListenerIdReady = (Action)Delegate.Combine(element.OnListenerIdReady, new Action(delegate()
			{
				int taiwuCharId = SingletonObject.getInstance<BasicGameData>().TaiwuCharId;
				CharacterDomainMethod.Call.GetCharacterDisplayData(this.Element.GameDataListenerId, taiwuCharId);
				BuildingDomainMethod.Call.GetTaiwuVillageResourceBlockEffect(this.Element.GameDataListenerId, EBuildingScaleEffect.LegacyPointBonusFactor);
				WorldDomainMethod.Call.GetWorldCreationInfo(this.Element.GameDataListenerId);
				this.murderMapPage.OnListenerIdReady();
			}));
			this._lastShowBubbleTime = Time.realtimeSinceStartup;
			this.bubble.SetActive(false);
			this.UpdateAnimation(ViewJieQingInteract.BubbleType.Idle, true);
			for (int i = 0; i < this.spineList.Count; i++)
			{
				int ii = i;
				this.spineList[i].AnimationState.Complete += delegate(TrackEntry entry)
				{
					bool flag2 = entry.Animation.Name != "animation";
					if (flag2)
					{
						this.spineList[ii].AnimationState.SetAnimation(0, "animation", true);
					}
				};
			}
		}

		// Token: 0x06007709 RID: 30473 RVA: 0x00376734 File Offset: 0x00374934
		private void OnUseFortune(bool hasLegacy)
		{
			bool flag = !hasLegacy;
			if (!flag)
			{
				UIParticle targetParticle = this.fortuneBlinkEffectList[this._isGood ? 0 : 1];
				targetParticle.gameObject.SetActive(true);
				targetParticle.Play();
			}
		}

		// Token: 0x0600770A RID: 30474 RVA: 0x00376778 File Offset: 0x00374978
		private void RefreshPageArtAssets()
		{
			this.spineList[0].gameObject.SetActive(this._isGood);
			this.spineList[1].gameObject.SetActive(this._isBad);
			this.characterEffectList[0].gameObject.SetActive(this._isGood);
			this.characterEffectList[1].gameObject.SetActive(!this._isGood);
			this.characterName.text = this.GetCharacterName();
			this.UpdateAnimation(ViewJieQingInteract.BubbleType.Idle, true);
		}

		// Token: 0x0600770B RID: 30475 RVA: 0x00376818 File Offset: 0x00374A18
		private void InitArguments(ArgumentBox argsBox)
		{
		}

		// Token: 0x0600770C RID: 30476 RVA: 0x0037681B File Offset: 0x00374A1B
		private void Update()
		{
		}

		// Token: 0x0600770D RID: 30477 RVA: 0x0037681E File Offset: 0x00374A1E
		private void OnDisable()
		{
		}

		// Token: 0x0600770E RID: 30478 RVA: 0x00376824 File Offset: 0x00374A24
		public override void InitMonitorFieldIds()
		{
			this.MonitorFields.Add(new UIBase.MonitorDataField(19, 199, ulong.MaxValue, null));
			this.MonitorFields.Add(new UIBase.MonitorDataField(4, 0, (ulong)((long)SingletonObject.getInstance<BasicGameData>().TaiwuCharId), new uint[]
			{
				17U,
				55U
			}));
			this.MonitorFields.Add(new UIBase.MonitorDataField(5, 36, ulong.MaxValue, null));
		}

		// Token: 0x0600770F RID: 30479 RVA: 0x00376894 File Offset: 0x00374A94
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
						bool flag = notification.DomainId == 4 && notification.MethodId == 131;
						if (flag)
						{
							Serializer.Deserialize(wrapper.DataPool, notification.ValueOffset, ref this._characterDisplayData);
							this.fortuneLegacyPage.HandleCharacterData(this._characterDisplayData);
						}
						else
						{
							bool flag2 = notification.DomainId == 4 && notification.MethodId == 183;
							if (flag2)
							{
								List<short> taiwuFeatureIds = new List<short>();
								Serializer.Deserialize(wrapper.DataPool, notification.ValueOffset, ref taiwuFeatureIds);
								this.fortuneLegacyPage.HandleFeatureIds(taiwuFeatureIds);
							}
							else
							{
								bool flag3 = notification.DomainId == 9 && notification.MethodId == 160;
								if (flag3)
								{
									int _legacyPointBonusFactor = 0;
									Serializer.Deserialize(wrapper.DataPool, notification.ValueOffset, ref _legacyPointBonusFactor);
									this.fortuneLegacyPage.HandleLegacyPointBonusFactor(_legacyPointBonusFactor);
								}
								else
								{
									bool flag4 = notification.DomainId == 1 && notification.MethodId == 2;
									if (flag4)
									{
										Serializer.Deserialize(wrapper.DataPool, notification.ValueOffset, ref this._worldCreationInfo);
										this.fortuneLegacyPage.HandleWorldCreationInfo(this._worldCreationInfo);
										this.Element.ShowAfterRefresh();
									}
								}
							}
						}
					}
				}
				else
				{
					DataUid uid = notification.Uid;
					bool flag5 = uid.DomainId == 5;
					if (flag5)
					{
						this.HandleTaiwuData(uid, wrapper.DataPool, notification.ValueOffset);
					}
					bool flag6 = uid.DomainId == 19 && uid.DataId == 199;
					if (flag6)
					{
						this.HandleStarFortune(wrapper.DataPool, notification.ValueOffset);
					}
					else
					{
						bool flag7;
						if (uid.DomainId == 4)
						{
							uint subId = uid.SubId1;
							flag7 = (subId == 17U || subId == 55U);
						}
						else
						{
							flag7 = false;
						}
						bool flag8 = flag7;
						if (flag8)
						{
							CharacterDomainMethod.Call.GetAvailableFeature(this.Element.GameDataListenerId, SingletonObject.getInstance<BasicGameData>().TaiwuCharId);
						}
					}
				}
			}
		}

		// Token: 0x06007710 RID: 30480 RVA: 0x00376B0C File Offset: 0x00374D0C
		private void HandleStarFortune(RawDataPool dataPool, int valueOffset)
		{
			Serializer.Deserialize(dataPool, valueOffset, ref this._starFortunePoints);
			this.fortuneLegacyPage.HandleStarFortune(this._starFortunePoints);
		}

		// Token: 0x06007711 RID: 30481 RVA: 0x00376B30 File Offset: 0x00374D30
		private void HandleTaiwuData(DataUid uid, RawDataPool dataPool, int valueOffset)
		{
			ushort dataId = uid.DataId;
			ushort num = dataId;
			if (num == 36)
			{
				List<short> availableLegacyList = new List<short>();
				Serializer.Deserialize(dataPool, valueOffset, ref availableLegacyList);
				this.fortuneLegacyPage.HandleAvailableLegacyList(availableLegacyList);
			}
		}

		// Token: 0x06007712 RID: 30482 RVA: 0x00376B70 File Offset: 0x00374D70
		public string GetCharacterName()
		{
			bool flag = this._pageState == ViewJieQingInteract.EJieQingInteractState.MurderMap;
			string result;
			if (flag)
			{
				result = LocalStringManager.Get(LanguageKey.LK_SectMainStory_JieQing_WanEWeng);
			}
			else
			{
				result = LocalStringManager.Get(LanguageKey.LK_SectMainStory_JieQing_WanShanWeng);
			}
			return result;
		}

		// Token: 0x06007713 RID: 30483 RVA: 0x00376BA8 File Offset: 0x00374DA8
		public void ShowBubbleAndAnimation(ViewJieQingInteract.BubbleType bubbleType)
		{
			this.ShowBubble(bubbleType);
			bool flag = bubbleType > ViewJieQingInteract.BubbleType.Idle;
			if (flag)
			{
				this.UpdateAnimation(bubbleType, false);
			}
		}

		// Token: 0x06007714 RID: 30484 RVA: 0x00376BD0 File Offset: 0x00374DD0
		private void ShowBubble(ViewJieQingInteract.BubbleType bubbleType)
		{
			this.bubble.SetActive(true);
			string text = LocalStringManager.Get(this.GetBubbleTextKey(bubbleType));
			this.bubbleText.text = text;
			this._lastShowBubbleTime = Time.realtimeSinceStartup;
			this._isShowingBubble = true;
			bool flag = this._closeBubbleCoroutine != null;
			if (flag)
			{
				SingletonObject.getInstance<YieldHelper>().StopYield(this._closeBubbleCoroutine);
			}
			this._closeBubbleCoroutine = SingletonObject.getInstance<YieldHelper>().DelaySecondsDo(5f, delegate
			{
				bool flag2 = this.bubble != null;
				if (flag2)
				{
					this.bubble.SetActive(false);
				}
				this._isShowingBubble = false;
			});
		}

		// Token: 0x06007715 RID: 30485 RVA: 0x00376C58 File Offset: 0x00374E58
		private string GetBubbleTextKey(ViewJieQingInteract.BubbleType bubbleType)
		{
			string result;
			if (bubbleType != ViewJieQingInteract.BubbleType.UseFortune)
			{
				result = null;
			}
			else
			{
				result = "LK_JieqingInteract_UseFortune_Bubble";
			}
			return result;
		}

		// Token: 0x06007716 RID: 30486 RVA: 0x00376C80 File Offset: 0x00374E80
		private void UpdateAnimation(ViewJieQingInteract.BubbleType bubbleType, bool loop = false)
		{
			string key = this.GetSpineAnimationKey(bubbleType);
			foreach (SkeletonGraphic spine in this.spineList)
			{
				bool flag = spine.gameObject.activeSelf && key != null;
				if (flag)
				{
					spine.AnimationState.SetAnimation(0, key, loop);
				}
			}
		}

		// Token: 0x06007717 RID: 30487 RVA: 0x00376D04 File Offset: 0x00374F04
		private string GetSpineAnimationKey(ViewJieQingInteract.BubbleType bubbleType)
		{
			string result;
			if (bubbleType != ViewJieQingInteract.BubbleType.Idle)
			{
				result = null;
			}
			else
			{
				result = "animation";
			}
			return result;
		}

		// Token: 0x06007718 RID: 30488 RVA: 0x00376D28 File Offset: 0x00374F28
		public void OnCancel()
		{
			this.QuickHide();
		}

		// Token: 0x06007719 RID: 30489 RVA: 0x00376D34 File Offset: 0x00374F34
		protected override void OnClick(Transform btn)
		{
			bool flag = btn.name == "ButtonClosePopup";
			if (flag)
			{
				this.QuickHide();
			}
		}

		// Token: 0x0600771A RID: 30490 RVA: 0x00376D5F File Offset: 0x00374F5F
		public override void QuickHide()
		{
			base.QuickHide();
			this.MonitorFields.Clear();
		}

		// Token: 0x040059D4 RID: 22996
		[SerializeField]
		private GameObject bubble;

		// Token: 0x040059D5 RID: 22997
		[SerializeField]
		private TextMeshProUGUI bubbleText;

		// Token: 0x040059D6 RID: 22998
		[SerializeField]
		private CToggleGroup pageToggleGroup;

		// Token: 0x040059D7 RID: 22999
		[SerializeField]
		private TextMeshProUGUI characterName;

		// Token: 0x040059D8 RID: 23000
		[SerializeField]
		private JieQingUseFortune fortuneLegacyPage;

		// Token: 0x040059D9 RID: 23001
		[SerializeField]
		private JieQingMurderMap murderMapPage;

		// Token: 0x040059DA RID: 23002
		[SerializeField]
		private List<UIParticle> characterEffectList;

		// Token: 0x040059DB RID: 23003
		[SerializeField]
		private List<SkeletonGraphic> spineList;

		// Token: 0x040059DC RID: 23004
		[SerializeField]
		private List<UIParticle> fortuneBlinkEffectList;

		// Token: 0x040059DD RID: 23005
		[SerializeField]
		private List<UIParticle> fortuneLineEffectList;

		// Token: 0x040059DE RID: 23006
		private CharacterDisplayData _characterDisplayData;

		// Token: 0x040059DF RID: 23007
		private int _taiwuCharId;

		// Token: 0x040059E0 RID: 23008
		private int _starFortunePoints;

		// Token: 0x040059E1 RID: 23009
		private WorldCreationInfo _worldCreationInfo;

		// Token: 0x040059E2 RID: 23010
		private ViewJieQingInteract.EJieQingInteractState _pageState = ViewJieQingInteract.EJieQingInteractState.None;

		// Token: 0x040059E3 RID: 23011
		private bool _inited = false;

		// Token: 0x040059E4 RID: 23012
		private const string defaultAnimationName = "animation";

		// Token: 0x040059E5 RID: 23013
		private readonly Dictionary<UIParticle, Coroutine> _playOneParticleCoroutines = new Dictionary<UIParticle, Coroutine>();

		// Token: 0x040059E6 RID: 23014
		private Coroutine _closeBubbleCoroutine;

		// Token: 0x040059E7 RID: 23015
		private float _lastShowBubbleTime = 0f;

		// Token: 0x040059E8 RID: 23016
		private bool _isShowingBubble = false;

		// Token: 0x02001ED1 RID: 7889
		public enum BubbleType
		{
			// Token: 0x0400CB2B RID: 52011
			Idle,
			// Token: 0x0400CB2C RID: 52012
			UseFortune
		}

		// Token: 0x02001ED2 RID: 7890
		private enum EJieQingInteractState
		{
			// Token: 0x0400CB2E RID: 52014
			None,
			// Token: 0x0400CB2F RID: 52015
			UseFortune,
			// Token: 0x0400CB30 RID: 52016
			MurderMap
		}
	}
}
