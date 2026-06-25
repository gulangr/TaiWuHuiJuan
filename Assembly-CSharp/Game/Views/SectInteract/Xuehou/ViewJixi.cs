using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using Config;
using DG.Tweening;
using FrameWork;
using FrameWork.UISystem.UIElements;
using Game.Components.Avatar;
using Game.Views.CharacterMenu;
using Game.Views.Select;
using GameData.Domains.Character;
using GameData.Domains.Character.Display;
using GameData.Domains.Character.SortFilter;
using GameData.Domains.Extra;
using GameData.Domains.TaiwuEvent;
using GameData.GameDataBridge;
using GameData.Serializer;
using GameData.Utilities;
using Spine.Unity;
using TMPro;
using UICommon.Character;
using UnityEngine;

namespace Game.Views.SectInteract.Xuehou
{
	// Token: 0x020009D6 RID: 2518
	public class ViewJixi : UIBase
	{
		// Token: 0x17000D92 RID: 3474
		// (get) Token: 0x06007AED RID: 31469 RVA: 0x0039193C File Offset: 0x0038FB3C
		public int FixedNeiliProgressPerAllocation
		{
			get
			{
				return this._specialData.FixedNeiliProgressPerAllocation;
			}
		}

		// Token: 0x17000D93 RID: 3475
		// (get) Token: 0x06007AEE RID: 31470 RVA: 0x00391949 File Offset: 0x0038FB49
		private int StatusIndex
		{
			get
			{
				return (this._specialData != null) ? (this._specialData.JixiCurrentTemplateId - 877) : 0;
			}
		}

		// Token: 0x17000D94 RID: 3476
		// (get) Token: 0x06007AEF RID: 31471 RVA: 0x00391967 File Offset: 0x0038FB67
		// (set) Token: 0x06007AF0 RID: 31472 RVA: 0x0039196F File Offset: 0x0038FB6F
		private int TaiwuTargetCharacterId
		{
			get
			{
				return this._taiwuTargetCharacterId;
			}
			set
			{
				this._taiwuTargetCharacterId = value;
			}
		}

		// Token: 0x17000D95 RID: 3477
		// (get) Token: 0x06007AF1 RID: 31473 RVA: 0x00391979 File Offset: 0x0038FB79
		// (set) Token: 0x06007AF2 RID: 31474 RVA: 0x00391981 File Offset: 0x0038FB81
		private int JixiTargetCharacterId
		{
			get
			{
				return this._jixiTargetCharacterId;
			}
			set
			{
				this._jixiTargetCharacterId = value;
			}
		}

		// Token: 0x17000D96 RID: 3478
		// (get) Token: 0x06007AF3 RID: 31475 RVA: 0x0039198B File Offset: 0x0038FB8B
		private int JixiGrowProgressPercent
		{
			get
			{
				return Math.Clamp(this._previewGrowProgress * 100 / 3000, 0, 100);
			}
		}

		// Token: 0x17000D97 RID: 3479
		// (get) Token: 0x06007AF4 RID: 31476 RVA: 0x003919A4 File Offset: 0x0038FBA4
		// (set) Token: 0x06007AF5 RID: 31477 RVA: 0x003919AC File Offset: 0x0038FBAC
		private bool IsShowJixiInfo
		{
			get
			{
				return this._isShowJixiInfo;
			}
			set
			{
				this._isShowJixiInfo = value;
				this.OnIsShowJixiInfoChanged();
			}
		}

		// Token: 0x06007AF6 RID: 31478 RVA: 0x003919C0 File Offset: 0x0038FBC0
		private void OnIsShowJixiInfoChanged()
		{
			this.jixiInfo.gameObject.SetActive(this.IsShowJixiInfo);
			bool isShowJixiInfo = this.IsShowJixiInfo;
			if (isShowJixiInfo)
			{
				this.contentRoot.SetDefaultBlack();
			}
			else
			{
				this.contentRoot.SetDefault();
			}
			UIManager.Instance.SetEscHandler(this.IsShowJixiInfo ? delegate()
			{
				this.IsShowJixiInfo = false;
			} : null);
		}

		// Token: 0x06007AF7 RID: 31479 RVA: 0x00391A30 File Offset: 0x0038FC30
		private void Awake()
		{
			this.taiwuSelectRoot.Init(new Action<int, int>(this.OnTaiwuNeiliAllocationTypeChange), delegate
			{
				this.OpenCharacterMenu(this._taiwuTargetCharacterId);
			}, delegate
			{
				this.OpenSelectCharPanel(true);
			});
			this.jixiSelectRoot.Init(new Action<int, int>(this.OnJixiNeiliAllocationTypeChange), delegate
			{
				this.OpenCharacterMenu(this._jixiTargetCharacterId);
			}, delegate
			{
				this.OpenSelectCharPanel(false);
			});
			for (int i = 0; i < this.taiwuAllocationItems.Length; i++)
			{
				int index = i;
				this.taiwuAllocationItems[i].Init(delegate
				{
					this.OnNeiliAllocateButtonClicked(true, index);
				});
				this.jixiAllocationItems[i].Init(delegate
				{
					this.OnNeiliAllocateButtonClicked(false, index);
				});
			}
		}

		// Token: 0x06007AF8 RID: 31480 RVA: 0x00391AFF File Offset: 0x0038FCFF
		private void OnTaiwuNeiliAllocationTypeChange(int newIndex, int oldIndex)
		{
			ExtraDomainMethod.Call.SetTaiwuTargetFiveElementsType((sbyte)newIndex);
			this.RequestData();
		}

		// Token: 0x06007AF9 RID: 31481 RVA: 0x00391B11 File Offset: 0x0038FD11
		private void OnJixiNeiliAllocationTypeChange(int newIndex, int oldIndex)
		{
			ExtraDomainMethod.Call.SetJixiDrainType((sbyte)newIndex);
			this.RequestData();
		}

		// Token: 0x06007AFA RID: 31482 RVA: 0x00391B24 File Offset: 0x0038FD24
		private void OpenCharacterMenu(int characterId)
		{
			UIElement.CharacterMenu.SetOnInitArgs(EasyPool.Get<ArgumentBox>().Set("CharacterId", characterId).SetObject("ViewCharacterMenuTaretPage", new SubPageIndex(ECharacterSubToggleBase.CharacterBase, ECharacterSubPage.None)));
			UIManager.Instance.ShowUI(UIElement.CharacterMenu, true);
		}

		// Token: 0x06007AFB RID: 31483 RVA: 0x00391B74 File Offset: 0x0038FD74
		private void OpenSelectCharPanel(bool isTaiwu)
		{
			CharacterSortFilterSettings settings = isTaiwu ? this._taiwuTargetCharSortFilterSettings : this._jixiTargetCharSortFilterSettings;
			AsyncMethodCallbackDelegate <>9__1;
			CharacterDomainMethod.AsyncCall.UpdateSortFilterSettings(this, settings, delegate(int offset, RawDataPool dataPool)
			{
				CharacterList charIds;
				Serializer.Deserialize(dataPool, offset, ref charIds);
				IAsyncMethodRequestHandler <>4__this = this;
				List<int> collection = charIds.GetCollection();
				AsyncMethodCallbackDelegate callback;
				if ((callback = <>9__1) == null)
				{
					callback = (<>9__1 = delegate(int offset, RawDataPool dataPool)
					{
						List<CharacterDisplayDataForGeneralScrollList> dataList = new List<CharacterDisplayDataForGeneralScrollList>();
						Serializer.Deserialize(dataPool, offset, ref dataList);
						this.OpenSelectChar(dataList, isTaiwu);
					});
				}
				CharacterDomainMethod.AsyncCall.GetCharacterDisplayDataForGeneralScrollListBatch(<>4__this, collection, callback);
			});
		}

		// Token: 0x06007AFC RID: 31484 RVA: 0x00391BC4 File Offset: 0x0038FDC4
		private void OpenSelectChar(List<CharacterDisplayDataForGeneralScrollList> dataList, bool isTaiwu)
		{
			Action<int> onConfirmSelect = delegate(int charId)
			{
				bool isTaiwu2 = isTaiwu;
				if (isTaiwu2)
				{
					ExtraDomainMethod.Call.SetTaiwuTransformFiveElementsTarget(charId);
				}
				else
				{
					ExtraDomainMethod.Call.SetJixiTarget(charId);
				}
				this.RequestData();
			};
			Action onCancelSelect = delegate()
			{
				bool isTaiwu2 = isTaiwu;
				if (isTaiwu2)
				{
					ExtraDomainMethod.Call.SetTaiwuTransformFiveElementsTarget(-1);
				}
				else
				{
					ExtraDomainMethod.Call.SetJixiTarget(-1);
				}
				this.RequestData();
			};
			CommonSelectCharacterConfig config = CommonSelectCharacterConfig.CreateBasicFilterConfig(ESelectCharacterSubPage.None);
			config.InteractionMode = ESelectCharacterInteractionMode.Slot;
			config.SelectionMode = ESelectCharacterSelectionMode.Single;
			config.Title = LocalStringManager.Get(LanguageKey.LK_Jixi_Talk_ChangeStatus);
			int selectedCharId = isTaiwu ? this._taiwuTargetCharacterId : this._jixiTargetCharacterId;
			bool flag = selectedCharId >= 0;
			if (flag)
			{
				config.InitialSelectedCharacterIds = new List<int>
				{
					selectedCharId
				};
			}
			config.MinSelectionCount = 0;
			UIElement.SelectChar.SetOnInitArgs(EasyPool.Get<ArgumentBox>().SetObject("SelectCharacterConfig", config).SetObject("SelectCharacterDataList", dataList).SetObject("SelectCharacterCallback", new SelectCharacterCallback(delegate(List<int> selectedIds)
			{
				bool flag2 = selectedIds != null && selectedIds.Count > 0;
				if (flag2)
				{
					Action<int> onConfirmSelect = onConfirmSelect;
					if (onConfirmSelect != null)
					{
						onConfirmSelect(selectedIds[0]);
					}
				}
				else
				{
					Action onCancelSelect = onCancelSelect;
					if (onCancelSelect != null)
					{
						onCancelSelect();
					}
				}
			})));
			UIManager.Instance.MaskUI(UIElement.SelectChar);
		}

		// Token: 0x06007AFD RID: 31485 RVA: 0x00391CC0 File Offset: 0x0038FEC0
		private void OnNeiliAllocateButtonClicked(bool isTaiwu, int index)
		{
			if (isTaiwu)
			{
				this._neiliTransferCount[index]++;
			}
			else
			{
				this._neiliTransferCount[index]--;
			}
			bool flag = this._neiliTransferCount[index] <= 0;
			if (flag)
			{
				this._previewNeili[0, index] = this._curNeili[0, index] + this._neiliTransferCount[index];
			}
			else
			{
				int ratio = (int)GlobalConfig.Instance.ExtraNeiliAllocationFromProgressRatio;
				int ratioGrowth = (int)GlobalConfig.Instance.ExtraNeiliAllocationFromProgressRatioGrowth;
				int fixedPerAlloc = this.FixedNeiliProgressPerAllocation;
				int baseAlloc = this._curNeiliAllocation[0, index] - 1;
				int transferAlloc = this._neiliTransferCount[index] / fixedPerAlloc;
				float extraProgressDiff = (float)(LoopingCommonUtils.GetExtraNeiliAllocationProgressByExtraNeiliAllocation(baseAlloc + transferAlloc, ratio, ratioGrowth) - LoopingCommonUtils.GetExtraNeiliAllocationProgressByExtraNeiliAllocation(baseAlloc, ratio, ratioGrowth));
				float remainderNeili = (float)(this._neiliTransferCount[index] % fixedPerAlloc * ((baseAlloc + transferAlloc) * ratioGrowth + ratio) / fixedPerAlloc);
				this._previewNeili[0, index] = (int)Math.Round((double)((float)this._curNeili[0, index] + extraProgressDiff + remainderNeili + this._specialData.NeiliAllocProgressTransferRemain[index] / 100f));
			}
			this._previewNeili[1, index] = this._curNeili[1, index] - this._neiliTransferCount[index];
			int neiliDiff = 0;
			for (int i = 0; i < 4; i++)
			{
				neiliDiff += this._neiliTransferCount[i] * -1;
			}
			this._previewGrowProgress = this._specialData.GrowthValue + neiliDiff;
			int favorIncrease = Math.Max(neiliDiff, 0) * 2000 / this.FixedNeiliProgressPerAllocation;
			this._previewFavor = (short)Math.Clamp((int)this._specialData.Favorability + favorIncrease, (int)this._specialData.Favorability, 30000);
			this.UpdateJixiFavorProgress();
			this.UpdateJixiGrowProgress();
			this.UpdateNeiliAllocationItems();
			bool isChange = this._neiliTransferCount.Any((int x) => x != 0);
			bool flag2 = this.IsAnyEffOperate && !isChange;
			if (flag2)
			{
				this.PlayEnterEff();
			}
			this.IsAnyEffOperate = isChange;
		}

		// Token: 0x06007AFE RID: 31486 RVA: 0x00391EF4 File Offset: 0x003900F4
		public override void OnInit(ArgumentBox argsBox)
		{
			this._taiwuId = SingletonObject.getInstance<BasicGameData>().TaiwuCharId;
			this._isFirstShow = true;
			this.jixiEff.gameObject.SetActive(false);
			this.IsShowJixiInfo = false;
			this.taiwuAvatar.gameObject.SetActive(false);
			this.NeedWaitData = true;
			this.NeedDataListenerId = true;
			UIElement element = this.Element;
			element.OnListenerIdReady = (Action)Delegate.Combine(element.OnListenerIdReady, new Action(this.OnListenerIdReady));
			UIElement element2 = this.Element;
			element2.OnShowed = (Action)Delegate.Combine(element2.OnShowed, new Action(this.OnShowed));
			this.taiwuNeiliToggleGroup.Init(-1);
			this.taiwuShowInfoButton.ClearAndAddListener(delegate
			{
				ArgumentBox argBox = EasyPool.Get<ArgumentBox>();
				argBox.Set("CharacterId", this._taiwuCharacterDisplayData.CharacterId);
				argBox.Set("CanOperate", true);
				argBox.Set("PreviousView", 5);
				UIElement.CharacterMenu.SetOnInitArgs(argBox);
				UIManager.Instance.ShowUI(UIElement.CharacterMenu, true);
				ArgumentBox args = new ArgumentBox();
				args.SetObject("TargetPageIndex", ECharacterSubToggleBase.CharacterBase);
				GEvent.OnEvent(UiEvents.OnNeedOpenCharacterMenuSubPage, args);
			});
			this.jixiShowInfoButton.ClearAndAddListener(delegate
			{
				ArgumentBox argBox = EasyPool.Get<ArgumentBox>();
				argBox.Set("CharacterId", this._specialData.JixiCharId);
				argBox.Set("CanOperate", false);
				argBox.Set("PreviousView", 5);
				UIElement.CharacterMenu.SetOnInitArgs(argBox);
				UIManager.Instance.ShowUI(UIElement.CharacterMenu, true);
				ArgumentBox args = new ArgumentBox();
				args.SetObject("TargetPageIndex", ECharacterSubToggleBase.CharacterBase);
				GEvent.OnEvent(UiEvents.OnNeedOpenCharacterMenuSubPage, args);
			});
		}

		// Token: 0x06007AFF RID: 31487 RVA: 0x00391FDE File Offset: 0x003901DE
		private void OnListenerIdReady()
		{
			this.RequestData();
		}

		// Token: 0x06007B00 RID: 31488 RVA: 0x00391FE8 File Offset: 0x003901E8
		private void OnShowed()
		{
			this.PlayEnterEff();
		}

		// Token: 0x06007B01 RID: 31489 RVA: 0x00391FF4 File Offset: 0x003901F4
		private void RequestData()
		{
			CharacterDomainMethod.Call.GetCharacterDisplayData(this.Element.GameDataListenerId, this._taiwuId);
			CharacterDomainMethod.Call.GetExtraNeiliAllocationProgress(this.Element.GameDataListenerId, this._taiwuId);
			ExtraDomainMethod.Call.GetJixiSpecialInteractDisplayData(this.Element.GameDataListenerId);
		}

		// Token: 0x06007B02 RID: 31490 RVA: 0x00392044 File Offset: 0x00390244
		public override void OnNotifyGameData(List<NotificationWrapper> notifications)
		{
			foreach (NotificationWrapper wrapper in notifications)
			{
				Notification notification = wrapper.Notification;
				byte type = notification.Type;
				byte b = type;
				if (b == 1)
				{
					this.HandlerMethodReturn(notification, wrapper);
				}
			}
		}

		// Token: 0x06007B03 RID: 31491 RVA: 0x003920B4 File Offset: 0x003902B4
		private void HandlerMethodReturn(Notification notification, NotificationWrapper wrapper)
		{
			ushort domainId = notification.DomainId;
			ushort num = domainId;
			if (num != 4)
			{
				if (num == 19)
				{
					if (notification.MethodId == 225)
					{
						Serializer.Deserialize(wrapper.DataPool, notification.ValueOffset, ref this._specialData);
						bool flag = !this.Element.Ready;
						if (flag)
						{
							this.InitByJixiDisplayData();
						}
						this.HandleJixiDisplayData();
						this.RefreshTaiwuNeli();
						bool flag2 = !this.Element.Ready;
						if (flag2)
						{
							this.Element.ShowAfterRefresh();
						}
					}
				}
			}
			else if (notification.MethodId != 131)
			{
				if (notification.MethodId == 194)
				{
					Serializer.Deserialize(wrapper.DataPool, notification.ValueOffset, ref this._taiwuExtraNeiliAllocationProgress);
				}
			}
			else
			{
				Serializer.Deserialize(wrapper.DataPool, notification.ValueOffset, ref this._taiwuCharacterDisplayData);
				this.taiwuAvatar.Refresh(this._taiwuCharacterDisplayData, true);
				this.taiwuAvatar.gameObject.SetActive(true);
			}
		}

		// Token: 0x06007B04 RID: 31492 RVA: 0x003921CC File Offset: 0x003903CC
		private void InitByJixiDisplayData()
		{
			this.taiwuName.text = NameCenter.GetNameByDisplayData(this._taiwuCharacterDisplayData, true, false);
			this.TaiwuMaxNeiliAllocationProgress = LoopingCommonUtils.GetNeiliAllocationMaxProgress() / 100;
			this.JixiMaxNeiliAllocationProgress = this._specialData.FixedNeiliProgressPerAllocation * (int)GlobalConfig.Instance.MaxExtraNeiliAllocation;
			this.UpdateJixiBubble(LocalStringManager.Get(string.Format("LK_Jixi_Talk_Normal_{0}", this.StatusIndex)));
			bool flag = !this._specialData.JixiDrainNeili;
			if (flag)
			{
				ExtraDomainMethod.Call.SetJixiDrainNeili(true);
			}
		}

		// Token: 0x06007B05 RID: 31493 RVA: 0x0039225C File Offset: 0x0039045C
		private void HandleJixiDisplayData()
		{
			this.JixiTargetCharacterId = this._specialData.JixiTargetCharIdByTaiwu;
			this._jixiTargetCharacterDisplayData = this._specialData.JixiTargetCharacterDisplayData;
			this.TaiwuTargetCharacterId = this._specialData.TaiwuTargetCharacterId;
			this._taiwuTargetCharacterDisplayData = this._specialData.TaiwuTargetCharacterDisplayData;
			this._previewFavor = this._specialData.Favorability;
			this._previewGrowProgress = this._specialData.GrowthValue;
			Array.Fill<int>(this._neiliTransferCount, 0);
			this.UpdateNeiliAllocationData();
			this.UpdateJixiAnimation();
			this.UpdateJixiGrowProgress();
			this.UpdateJixiFavorProgress();
			this.UpdateJixiInfo();
			this.UpdateXuehouSelectRoots();
			this.UpdateNeiliAllocationItems();
			this.UpdateTaiwuHealthInfo();
			this.recoverHealthButton.interactable = (this._specialData.JixiCurrentTemplateId != (int)Character.DefValue.JixiBaby.TemplateId && this._taiwuCharacterDisplayData.Health != this._taiwuCharacterDisplayData.LeftMaxHealth);
			TooltipInvoker mouseTip = this.recoverHealthButton.GetComponent<TooltipInvoker>();
			mouseTip.enabled = !this.recoverHealthButton.interactable;
			TooltipInvoker tooltipInvoker = mouseTip;
			if (tooltipInvoker.RuntimeParam == null)
			{
				tooltipInvoker.RuntimeParam = new ArgumentBox();
			}
			mouseTip.RuntimeParam.Set("arg0", (this._specialData.JixiCurrentTemplateId == (int)Character.DefValue.JixiBaby.TemplateId) ? LanguageKey.LK_Jixi_JixiGrowValueLack.Tr() : LanguageKey.LK_Jixi_TaiwuHealthMax.Tr());
			this.taiwuHealthProgress.fillAmount = (float)this._taiwuCharacterDisplayData.Health / (float)this._taiwuCharacterDisplayData.LeftMaxHealth;
		}

		// Token: 0x06007B06 RID: 31494 RVA: 0x003923F4 File Offset: 0x003905F4
		private void UpdateNeiliAllocationData()
		{
			for (int i = 0; i < this._curNeili.GetLength(1); i++)
			{
				this._curNeili[0, i] = this._taiwuExtraNeiliAllocationProgress[i] / 100;
				this._previewNeili[0, i] = this._taiwuExtraNeiliAllocationProgress[i] / 100;
				this._curNeili[1, i] = this._specialData.NeiliAllocProgressDrained.Items[i];
				this._previewNeili[1, i] = this._specialData.NeiliAllocProgressDrained.Items[i];
				ref int ptr = ref this._previewNeiliAllocation[0, i];
				ref float ptr2 = ref this._neiliAllocationPercent[0, i];
				ValueTuple<int, float> valueTuple = LoopingCommonUtils.CalculateNeiliProgressInfo(this._previewNeili[0, i], (int)GlobalConfig.Instance.ExtraNeiliAllocationFromProgressRatio, (int)GlobalConfig.Instance.ExtraNeiliAllocationFromProgressRatioGrowth);
				ptr = valueTuple.Item1;
				ptr2 = valueTuple.Item2;
				ref int ptr3 = ref this._curNeiliAllocation[0, i];
				valueTuple = LoopingCommonUtils.CalculateNeiliProgressInfo(this._curNeili[0, i], (int)GlobalConfig.Instance.ExtraNeiliAllocationFromProgressRatio, (int)GlobalConfig.Instance.ExtraNeiliAllocationFromProgressRatioGrowth);
				ptr3 = valueTuple.Item1;
				this._curNeiliAllocation[0, i]++;
				ptr = ref this._previewNeiliAllocation[1, i];
				ref float ptr4 = ref this._neiliAllocationPercent[1, i];
				valueTuple = LoopingCommonUtils.CalculateNeiliProgressInfo(this._previewNeili[1, i], (int)GlobalConfig.Instance.ExtraNeiliAllocationFromProgressRatio, 0);
				ptr = valueTuple.Item1;
				ptr4 = valueTuple.Item2;
				ref int ptr5 = ref this._curNeiliAllocation[1, i];
				valueTuple = LoopingCommonUtils.CalculateNeiliProgressInfo(this._curNeili[1, i], (int)GlobalConfig.Instance.ExtraNeiliAllocationFromProgressRatio, 0);
				ptr5 = valueTuple.Item1;
				this._curNeiliAllocation[1, i]++;
			}
		}

		// Token: 0x06007B07 RID: 31495 RVA: 0x003925C8 File Offset: 0x003907C8
		private void UpdateJixiAnimation()
		{
			bool flag = (int)this._showedJixiTemplateId == this._specialData.JixiCurrentTemplateId;
			if (flag)
			{
				this._isFirstShow = false;
			}
			else
			{
				this._showedJixiTemplateId = (short)this._specialData.JixiCurrentTemplateId;
				bool ready = this.Element.Ready;
				if (ready)
				{
					bool flag2 = !this._isFirstShow;
					if (flag2)
					{
						this.jixiEff.gameObject.SetActive(false);
						this.jixiEff.gameObject.SetActive(true);
					}
					base.DelayCall(new Action(this.<UpdateJixiAnimation>g__RefreshAnimation|118_0), 0.5f);
				}
				else
				{
					this.<UpdateJixiAnimation>g__RefreshAnimation|118_0();
				}
				this._isFirstShow = false;
			}
		}

		// Token: 0x06007B08 RID: 31496 RVA: 0x0039267C File Offset: 0x0039087C
		private void UpdateJixiGrowProgress()
		{
			float currentPercent = (float)this._specialData.GrowthValue * 1f / 3000f;
			float previewPercent = (float)this.JixiGrowProgressPercent / 100f;
			bool flag = this._previewGrowProgress > this._specialData.GrowthValue;
			if (flag)
			{
				this.jixiGrowProgress.fillAmount = currentPercent;
				this.jixiGrowProgressAdd.fillAmount = previewPercent;
				this.jixiGrowProgressAdd.gameObject.SetActive(true);
				this.jixiGrowProgressReduce.gameObject.SetActive(false);
			}
			else
			{
				bool flag2 = this._previewGrowProgress < this._specialData.GrowthValue;
				if (flag2)
				{
					this.jixiGrowProgress.fillAmount = previewPercent;
					this.jixiGrowProgressReduce.fillAmount = currentPercent;
					this.jixiGrowProgressAdd.gameObject.SetActive(false);
					this.jixiGrowProgressReduce.gameObject.SetActive(true);
				}
				else
				{
					this.jixiGrowProgress.fillAmount = previewPercent;
					this.jixiGrowProgressAdd.gameObject.SetActive(false);
					this.jixiGrowProgressReduce.gameObject.SetActive(false);
				}
			}
			this.jixiGrowLabel.text = string.Format("{0}/{1}", this._previewGrowProgress, 3000);
			Transform tagTs = this.jixiGrowProgressTagRoot.transform;
			int[] changeArray = new int[]
			{
				this._specialData.GrowthTotalYoung,
				this._specialData.GrowthTotalAdult
			};
			for (int i = 0; i < tagTs.childCount; i++)
			{
				bool flag3 = i >= changeArray.Length;
				if (flag3)
				{
					tagTs.GetChild(i).gameObject.SetActive(false);
				}
				else
				{
					tagTs.GetChild(i).gameObject.SetActive(true);
					float posXPercent = (float)changeArray[i] * 1f / 3000f;
					float posX = Mathf.Lerp(0f, 500f, posXPercent);
					tagTs.GetChild(i).GetComponent<RectTransform>().anchoredPosition = new Vector2(posX, 0f);
				}
			}
			TooltipInvoker mouseTip = this.jixiGrowProgress.transform.parent.GetComponent<TooltipInvoker>();
			int total = 0;
			for (int j = 0; j < 4; j++)
			{
				total += this._specialData.CurrentFormNeiliAllocProgressDrained.Items[j];
			}
			int jixiProgressValue = (this._specialData.GrowthValue >= this._specialData.GrowthTotalYoung) ? ((this._specialData.GrowthValue * 100 - this._specialData.GrowthTotalYoung * 100) / (this._specialData.GrowthTotalAdult - this._specialData.GrowthTotalYoung)) : (this._specialData.GrowthValue * 100 / this._specialData.GrowthTotalYoung);
			int curStatusPercent = Math.Clamp(jixiProgressValue, 0, 100);
			TooltipInvoker tooltipInvoker = mouseTip;
			if (tooltipInvoker.RuntimeParam == null)
			{
				tooltipInvoker.RuntimeParam = new ArgumentBox();
			}
			mouseTip.RuntimeParam.Set("Title", LanguageKey.LK_Jixi_GrowProgress_Tips_0.Tr()).Set("LineCount", 6).SetObject("LineData1", new GeneralLineData(7, new List<string>
			{
				LanguageKey.LK_Jixi_GrowProgress_Tips_1.Tr()
			}, null)).SetObject("LineData2", new GeneralLineData(8, new List<string>
			{
				LanguageKey.LK_Jixi_GrowProgress_Tips_2.TrFormat(string.Format("{0}", total / this._specialData.FixedNeiliProgressPerAllocation).SetColor("lightyellow")),
				string.Empty,
				string.Empty
			}, null)).SetObject("LineData3", new GeneralLineData(9, new List<string>
			{
				LanguageKey.LK_Jixi_GrowProgress_Tips_3.TrFormat(curStatusPercent.ToString().SetColor("lightyellow")),
				string.Empty,
				string.Empty
			}, null)).SetObject("LineData4", new GeneralLineData(4, null, null)).SetObject("LineData5", new GeneralLineData(8, new List<string>
			{
				LanguageKey.LK_Jixi_GrowProgress_Tips_4.Tr(),
				string.Empty,
				string.Empty
			}, null)).SetObject("LineData6", new GeneralLineData(9, new List<string>
			{
				LanguageKey.LK_Jixi_GrowProgress_Tips_5.TrFormat(this._specialData.CurrentFormMonthlyProgress.ToString().SetColor("lightyellow")),
				string.Empty,
				string.Empty
			}, null));
		}

		// Token: 0x06007B09 RID: 31497 RVA: 0x00392B2C File Offset: 0x00390D2C
		private void UpdateJixiFavorProgress()
		{
			this.jixiFavorLabel.text = CommonUtils.GetFavorString(this._previewFavor);
			this.jixiFavorIcon.SetSprite(CommonUtils.GetFavorabilityIconName(this._previewFavor, true), false, null);
			this.jixiFavorProgress.fillAmount = (float)this._previewFavor / 30000f;
		}

		// Token: 0x06007B0A RID: 31498 RVA: 0x00392B84 File Offset: 0x00390D84
		private void UpdateJixiInfo()
		{
			int jixiCurrentTemplateId = this._specialData.JixiCurrentTemplateId;
			if (!true)
			{
			}
			string text;
			switch (jixiCurrentTemplateId)
			{
			case 877:
				text = LanguageKey.LK_Jixi_Baby.Tr();
				break;
			case 878:
				text = LanguageKey.LK_Jixi_Young.Tr();
				break;
			case 879:
				text = LanguageKey.LK_Jixi_Adult.Tr();
				break;
			default:
				text = string.Empty;
				break;
			}
			if (!true)
			{
			}
			string jixiName = text;
			this.statusText.text = jixiName;
			this.killedText.text = LanguageKey.LK_Jixi_Info_Killed_Content.TrFormat(this._specialData.KillAmount.ToString());
			this.drainNeiliValueText.text = LanguageKey.LK_Jixi_Info_DrainNeili_Content_0.TrFormat(this._specialData.NeiliAllocDrainedTotal.ToString());
			this.drainNeiliFromText.text = LanguageKey.LK_Jixi_Info_DrainNeili_Content_1.TrFormat(this._specialData.TransferFromTaiwuTotal.ToString());
			this.drainNeiliToText.text = LanguageKey.LK_Jixi_Info_DrainNeili_Content_1.TrFormat(this._specialData.TransferToTaiwuTotal.ToString());
			this.killEnemyText.text = LanguageKey.LK_Jixi_Info_Enemy_Content_0.TrFormat(this._specialData.TempalteEnemyKilledTotal.ToString());
			this.killEnemyDebtText.text = LanguageKey.LK_Jixi_Info_Enemy_Content_1.TrFormat(this._specialData.KillTempalteEnemyGainTotal.ToString());
			for (int i = 0; i < 3; i++)
			{
				this.ageContentTexts[i].text = LanguageKey.LK_Jixi_Info_Age_Content.TrFormat(this._specialData.ChangeFormTotal[i].ToString());
			}
			for (int j = 0; j < 5; j++)
			{
				this.FiveElementsTexts[j].text = LocalStringManager.Get(string.Format("LK_FiveElements_Type_{0}", j)) + LanguageKey.LK_Colon_Symbol.Tr() + string.Format("{0}%", this._specialData.TaiwuTransformFiveElementsTotal[j]);
			}
			this.healthText.text = LanguageKey.LK_Jixi_Info_Health_Content.TrFormat(this._specialData.RescueTaiwuTimes.ToString());
		}

		// Token: 0x06007B0B RID: 31499 RVA: 0x00392DB0 File Offset: 0x00390FB0
		private void UpdateXuehouSelectRoots()
		{
			this.taiwuSelectRoot.Set(this._taiwuTargetCharacterDisplayData, this._specialData.TaiwuTargetCharacterFiveElements, (int)this._specialData.TaiwuTargetFiveElementsType, this._specialData.TaiwuTransformFiveElementsCurrent);
			this.jixiSelectRoot.Set(this._jixiTargetCharacterDisplayData, this._specialData.BaseNeiliAllocation, this._specialData.ExtraNeiliAllocation, (int)this._specialData.DrainTargetNeiliAllocType, this._specialData.CurrentTargetNeiliAllocProgressDrained, this.FixedNeiliProgressPerAllocation);
		}

		// Token: 0x06007B0C RID: 31500 RVA: 0x00392E38 File Offset: 0x00391038
		private void UpdateNeiliAllocationItems()
		{
			for (int i = 0; i < this.taiwuAllocationItems.Length; i++)
			{
				ref int ptr = ref this._previewNeiliAllocation[0, i];
				ref float ptr2 = ref this._neiliAllocationPercent[0, i];
				ValueTuple<int, float> valueTuple = LoopingCommonUtils.CalculateNeiliProgressInfo(this._previewNeili[0, i], (int)GlobalConfig.Instance.ExtraNeiliAllocationFromProgressRatio, (int)GlobalConfig.Instance.ExtraNeiliAllocationFromProgressRatioGrowth);
				ptr = valueTuple.Item1;
				ptr2 = valueTuple.Item2;
				bool taiwuBtnEnable = (this._previewNeiliAllocation[1, i] > 0 || this._neiliAllocationPercent[1, i] > 0f || this._curNeili[0, i] - this._previewNeili[0, i] > 0) && this._previewNeiliAllocation[0, i] < (int)GlobalConfig.Instance.MaxExtraNeiliAllocation;
				ValueTuple<int, float> valueTuple2 = LoopingCommonUtils.CalculateNeiliProgressInfo(this._curNeili[0, i], (int)GlobalConfig.Instance.ExtraNeiliAllocationFromProgressRatio, (int)GlobalConfig.Instance.ExtraNeiliAllocationFromProgressRatioGrowth);
				int taiwuCurrentPoint = valueTuple2.Item1;
				float taiwuCurrentPercent = valueTuple2.Item2;
				taiwuCurrentPercent = ((taiwuCurrentPoint < (int)GlobalConfig.Instance.MaxExtraNeiliAllocation) ? taiwuCurrentPercent : 1f);
				int taiwuCurrentNeLi = Math.Min(taiwuCurrentPoint, (int)GlobalConfig.Instance.MaxExtraNeiliAllocation);
				int taiwuPreviewNeLi = this._previewNeiliAllocation[0, i];
				LanguageKey taiwuLanguageKey = (taiwuPreviewNeLi < (int)GlobalConfig.Instance.MaxExtraNeiliAllocation) ? LanguageKey.LK_Jixi_TaiwuNeiliLack : LanguageKey.LK_Jixi_TaiwuNeiliMax;
				float taiwuPreviewPercent = (taiwuPreviewNeLi < (int)GlobalConfig.Instance.MaxExtraNeiliAllocation) ? this._neiliAllocationPercent[0, i] : 1f;
				this.taiwuAllocationItems[i].Set(taiwuCurrentNeLi, taiwuCurrentPercent, taiwuPreviewNeLi, taiwuPreviewPercent, taiwuBtnEnable, taiwuLanguageKey);
				ptr = ref this._previewNeiliAllocation[1, i];
				ref float ptr3 = ref this._neiliAllocationPercent[1, i];
				valueTuple = LoopingCommonUtils.CalculateNeiliProgressInfo(this._previewNeili[1, i], (int)GlobalConfig.Instance.ExtraNeiliAllocationFromProgressRatio, (int)GlobalConfig.Instance.ExtraNeiliAllocationFromProgressRatioGrowth);
				ptr = valueTuple.Item1;
				ptr3 = valueTuple.Item2;
				bool jixiBtnEnable = (this._previewNeiliAllocation[0, i] > 0 || this._neiliAllocationPercent[0, i] > 0f) && this.JixiGrowProgressPercent < 100;
				LanguageKey jixiLanguageKey = (this.JixiGrowProgressPercent == 100) ? LanguageKey.LK_Jixi_JixiMax : LanguageKey.LK_Jixi_TaiwuNeiliLack;
				int jixiPreviewNeili = this._previewNeiliAllocation[1, i];
				float jixiPreviewPercent = (jixiPreviewNeili < (int)GlobalConfig.Instance.MaxExtraNeiliAllocation) ? this._neiliAllocationPercent[1, i] : 1f;
				ValueTuple<int, float> valueTuple3 = LoopingCommonUtils.CalculateNeiliProgressInfo(this._curNeili[1, i], (int)GlobalConfig.Instance.ExtraNeiliAllocationFromProgressRatio, (int)GlobalConfig.Instance.ExtraNeiliAllocationFromProgressRatioGrowth);
				int jixiCurPoint = valueTuple3.Item1;
				float jixiCurPercent = valueTuple3.Item2;
				jixiCurPercent = ((jixiCurPoint < (int)GlobalConfig.Instance.MaxExtraNeiliAllocation) ? jixiCurPercent : 1f);
				int jixiCurNeili = jixiCurPoint;
				this.jixiAllocationItems[i].Set(jixiCurNeili, jixiCurPercent, jixiPreviewNeili, jixiPreviewPercent, jixiBtnEnable, jixiLanguageKey);
			}
			for (int j = 0; j < this._previewNeili.GetLength(1); j++)
			{
				bool flag = this._previewNeili[0, j] != this._curNeili[0, j] || this._previewNeili[1, j] != this._curNeili[1, j];
				if (flag)
				{
					this.translateNeiliButton.interactable = true;
					return;
				}
			}
			this.translateNeiliButton.interactable = false;
		}

		// Token: 0x06007B0D RID: 31501 RVA: 0x003931A8 File Offset: 0x003913A8
		private void UpdateTaiwuHealthInfo()
		{
			ValueTuple<string, float, int> characterHealthInfo = CommonUtils.GetCharacterHealthInfo(this._taiwuCharacterDisplayData.Health, this._taiwuCharacterDisplayData.LeftMaxHealth, this._taiwuCharacterDisplayData.CharacterId);
			string content = characterHealthInfo.Item1;
			float progress = characterHealthInfo.Item2;
			int type = characterHealthInfo.Item3;
			this.taiwuHealthLabel.text = content + string.Format(" {0}%", (int)(progress * 100f));
			this.taiwuHealthProgress.fillAmount = progress;
		}

		// Token: 0x06007B0E RID: 31502 RVA: 0x00393226 File Offset: 0x00391426
		private unsafe void RefreshTaiwuNeli()
		{
			CharacterDomainMethod.AsyncCall.GetCharacterDisplayDataForNeiliPage(null, this._taiwuCharacterDisplayData.CharacterId, delegate(int offset, RawDataPool dataPool)
			{
				Serializer.Deserialize(dataPool, offset, ref this._data);
				bool flag = this._data == null;
				if (!flag)
				{
					NeiliProportionOfFiveElements currValue = this._data.NeiliPercent;
					for (sbyte i = 0; i < 5; i += 1)
					{
						this.taiwuNeiliTextList[(int)i].text = string.Format("{0}%", *currValue[(int)i]).ToString();
					}
				}
			});
		}

		// Token: 0x06007B0F RID: 31503 RVA: 0x00393248 File Offset: 0x00391448
		protected override void OnClick(Transform btn)
		{
			string name = btn.name;
			string a = name;
			if (!(a == "ButtonCloseView"))
			{
				if (!(a == "StatusButton"))
				{
					if (!(a == "RecoverHealthButton"))
					{
						if (a == "TranslateNeiliButton")
						{
							int count = 0;
							byte i = 0;
							while ((int)i < this._neiliTransferCount.Length)
							{
								bool flag = this._neiliTransferCount[(int)i] < 0;
								if (flag)
								{
									ExtraDomainMethod.Call.TaiwuTransferNeiliAllocToJixi(-1 * this._neiliTransferCount[(int)i], i);
								}
								bool flag2 = this._neiliTransferCount[(int)i] > 0;
								if (flag2)
								{
									this._neiliTransferCount[(int)i] = Math.Min(this._neiliTransferCount[(int)i], this._curNeili[1, (int)i]);
									ExtraDomainMethod.Call.JixiTransferNeiliAllocToTaiwu(this._neiliTransferCount[(int)i], i);
								}
								count += this._neiliTransferCount[(int)i];
								i += 1;
							}
							bool flag3 = count < 0;
							if (flag3)
							{
								this.UpdateJixiBubble(LocalStringManager.Get(string.Format("LK_Jixi_Talk_TransToJixi_{0}", this.StatusIndex)));
							}
							else
							{
								this.UpdateJixiBubble(LocalStringManager.Get(string.Format("LK_Jixi_Talk_TransToTaiwu_{0}", this.StatusIndex)));
							}
							base.StartCoroutine(this.PlayConfirmEff());
							this.RequestData();
						}
					}
					else
					{
						DialogCmd cmd = new DialogCmd
						{
							Title = LanguageKey.LK_Jixi_Talk_RecoverHealth_Title.Tr(),
							Content = LanguageKey.LK_Jixi_Talk_RecoverHealth_Content.Tr(),
							Type = 1,
							Yes = new Action(this.<OnClick>g__OnConfirm|127_0)
						};
						UIElement.Dialog.SetOnInitArgs(EasyPool.Get<ArgumentBox>().SetObject("Cmd", cmd));
						UIManager.Instance.MaskUI(UIElement.Dialog);
					}
				}
				else
				{
					this.IsShowJixiInfo = !this.IsShowJixiInfo;
				}
			}
			else
			{
				this.QuickHide();
			}
		}

		// Token: 0x06007B10 RID: 31504 RVA: 0x00393439 File Offset: 0x00391639
		public override void QuickHide()
		{
			base.QuickHide();
			TaiwuEventDomainMethod.Call.SetListenerEventActionIntArg("ShowSectMainStorySpecialInteract", "CharacterId", this._specialData.JixiCharId);
			TaiwuEventDomainMethod.Call.TriggerListener("ShowSectMainStorySpecialInteract", true);
		}

		// Token: 0x17000D98 RID: 3480
		// (get) Token: 0x06007B11 RID: 31505 RVA: 0x0039346A File Offset: 0x0039166A
		// (set) Token: 0x06007B12 RID: 31506 RVA: 0x00393474 File Offset: 0x00391674
		public bool IsAnyEffOperate
		{
			get
			{
				return this._isAnyEffOperate;
			}
			set
			{
				bool flag = value && !this._isAnyEffOperate;
				if (flag)
				{
					base.StartCoroutine(this.PlayAnyOperateEff());
				}
				this._isAnyEffOperate = value;
			}
		}

		// Token: 0x06007B13 RID: 31507 RVA: 0x003934AC File Offset: 0x003916AC
		private void PlayEnterEff()
		{
			this._isAnyEffOperate = false;
			this.handClosed.gameObject.SetActive(true);
			this.illustration.gameObject.SetActive(false);
			this.eff_xuehou_ui_dancixian1.gameObject.SetActive(false);
			this.eff_xuehou_ui_huanrao1.gameObject.SetActive(false);
			this.eff_xuehou_ui_lianyi1.gameObject.SetActive(false);
			this.eff_xuehou_ui_lianyi2.gameObject.SetActive(false);
			this.eff_xuehou_ui_shoudakai.gameObject.SetActive(false);
			this.eff_xuehou_ui_shouhe.gameObject.SetActive(false);
			this.eff_xuehou_ui_yin.gameObject.SetActive(false);
			this.eff_xuehou_ui_zhakai.gameObject.SetActive(false);
		}

		// Token: 0x06007B14 RID: 31508 RVA: 0x00393575 File Offset: 0x00391775
		private IEnumerator PlayAnyOperateEff()
		{
			this.handClosed.gameObject.SetActive(false);
			foreach (ParticleSystem item in this.eff_xuehou_ui_shoudakai.GetComponentsInChildren<ParticleSystem>())
			{
				ParticleSystem.MainModule main = item.main;
				main.simulationSpeed = 1f;
				main = default(ParticleSystem.MainModule);
				item = null;
			}
			ParticleSystem[] array = null;
			this.eff_xuehou_ui_shoudakai.gameObject.SetActive(true);
			this.illustration.gameObject.SetActive(true);
			this.eff_xuehou_ui_lianyi1.gameObject.SetActive(true);
			yield return ViewJixi._waitForSeconds04;
			this.effHolderForAlphaControl.alpha = 0f;
			DOTween.To(() => this.effHolderForAlphaControl.alpha, delegate(float x)
			{
				this.effHolderForAlphaControl.alpha = x;
			}, 1f, 1f);
			this.eff_xuehou_ui_huanrao1.gameObject.SetActive(true);
			yield return ViewJixi._waitForSeconds08;
			foreach (ParticleSystem item2 in this.eff_xuehou_ui_shoudakai.GetComponentsInChildren<ParticleSystem>())
			{
				ParticleSystem.MainModule main2 = item2.main;
				main2.simulationSpeed = 0f;
				main2 = default(ParticleSystem.MainModule);
				item2 = null;
			}
			ParticleSystem[] array2 = null;
			yield return null;
			yield break;
		}

		// Token: 0x06007B15 RID: 31509 RVA: 0x00393584 File Offset: 0x00391784
		private IEnumerator PlayConfirmEff()
		{
			this._isAnyEffOperate = true;
			this.eff_xuehou_ui_dancixian1.gameObject.SetActive(true);
			DOTween.To(() => this.effHolderForAlphaControl.alpha, delegate(float x)
			{
				this.effHolderForAlphaControl.alpha = x;
			}, 0f, 0.8f);
			yield return ViewJixi._waitForSeconds06;
			this.eff_xuehou_ui_lianyi1.gameObject.SetActive(false);
			this.eff_xuehou_ui_yin.gameObject.SetActive(true);
			yield return ViewJixi._waitForSeconds1;
			this.eff_xuehou_ui_zhakai.gameObject.SetActive(true);
			this.eff_xuehou_ui_lianyi2.gameObject.SetActive(true);
			yield return ViewJixi._waitForSeconds04;
			this.eff_xuehou_ui_shouhe.gameObject.SetActive(true);
			this.eff_xuehou_ui_dancixian1.gameObject.SetActive(false);
			this.handClosed.gameObject.SetActive(true);
			CRawImage handClosedImage = this.handClosed.GetComponent<CRawImage>();
			bool flag = handClosedImage != null;
			if (flag)
			{
				this.illustration.gameObject.SetActive(false);
				handClosedImage.color = new Color(1f, 1f, 1f, 0f);
				handClosedImage.DOFade(1f, 0.2f);
			}
			yield return ViewJixi._waitForSeconds02;
			this.PlayEnterEff();
			yield return null;
			yield break;
		}

		// Token: 0x06007B16 RID: 31510 RVA: 0x00393594 File Offset: 0x00391794
		public void ChangeConnectEffStatus(bool isTaiwu, bool isShow, int index)
		{
			for (int i = 0; i < (isTaiwu ? this.eff_xuehou_ui_lianTaiwu.Length : this.eff_xuehou_ui_lianJixi.Length); i++)
			{
				(isTaiwu ? this.eff_xuehou_ui_lianTaiwu[i] : this.eff_xuehou_ui_lianJixi[i]).gameObject.SetActive(isShow && i == index);
			}
		}

		// Token: 0x06007B17 RID: 31511 RVA: 0x003935F4 File Offset: 0x003917F4
		private void UpdateJixiBubble(string content)
		{
			this.jixiBubble.SetText(content, true);
			bool flag = this._bubbleCoroutine != null;
			if (flag)
			{
				base.StopCoroutine(this._bubbleCoroutine);
			}
			this._bubbleCoroutine = base.StartCoroutine(this.HideBubble());
		}

		// Token: 0x06007B18 RID: 31512 RVA: 0x0039363C File Offset: 0x0039183C
		private IEnumerator HideBubble()
		{
			yield return ViewJixi._waitForSecondsHideBubbleDelay;
			this.jixiBubble.Hide();
			yield break;
		}

		// Token: 0x06007B22 RID: 31522 RVA: 0x003938C8 File Offset: 0x00391AC8
		[CompilerGenerated]
		private void <UpdateJixiAnimation>g__RefreshAnimation|118_0()
		{
			string aniPath = "RemakeResources/SpineAnimations/Xuehou/" + this.<UpdateJixiAnimation>g__GetStatusName|118_2();
			ResLoader.Load<SkeletonDataAsset>(aniPath, delegate(SkeletonDataAsset aniData)
			{
				this.jixiAnimation.skeletonDataAsset = aniData;
				this.jixiAnimation.Initialize(true);
				this.jixiAnimation.AnimationState.SetAnimation(0, "idle", true);
			}, null, false);
			this.jixiAnimation.transform.SetParent(this.jixiAnimationHolder.GetChild(this.StatusIndex), false);
		}

		// Token: 0x06007B24 RID: 31524 RVA: 0x00393954 File Offset: 0x00391B54
		[CompilerGenerated]
		private string <UpdateJixiAnimation>g__GetStatusName|118_2()
		{
			int statusIndex = this.StatusIndex;
			if (!true)
			{
			}
			string result;
			switch (statusIndex)
			{
			case 0:
				result = "InhaleGenuineQi_jixi_xiao_SkeletonData";
				break;
			case 1:
				result = "InhaleGenuineQi_jixi_zhong_SkeletonData";
				break;
			case 2:
				result = "InhaleGenuineQi_jixi_da_SkeletonData";
				break;
			default:
				result = "InhaleGenuineQi_jixi_xiao_SkeletonData";
				break;
			}
			if (!true)
			{
			}
			return result;
		}

		// Token: 0x06007B26 RID: 31526 RVA: 0x00393A25 File Offset: 0x00391C25
		[CompilerGenerated]
		private void <OnClick>g__OnConfirm|127_0()
		{
			ExtraDomainMethod.Call.JixiRescueTaiwu();
			this.RequestData();
			this.UpdateJixiBubble(LocalStringManager.Get(string.Format("LK_Jixi_Talk_RecoverHealth_{0}", this.StatusIndex)));
		}

		// Token: 0x04005D1C RID: 23836
		[SerializeField]
		private CButton closeButton;

		// Token: 0x04005D1D RID: 23837
		[SerializeField]
		private HSVStyleRoot contentRoot;

		// Token: 0x04005D1E RID: 23838
		[SerializeField]
		private Game.Components.Avatar.Avatar taiwuAvatar;

		// Token: 0x04005D1F RID: 23839
		[SerializeField]
		private TextMeshProUGUI taiwuName;

		// Token: 0x04005D20 RID: 23840
		[SerializeField]
		private TextMeshProUGUI taiwuHealthLabel;

		// Token: 0x04005D21 RID: 23841
		[SerializeField]
		private CImage taiwuHealthProgress;

		// Token: 0x04005D22 RID: 23842
		[SerializeField]
		private Bubble taiwuBubble;

		// Token: 0x04005D23 RID: 23843
		[SerializeField]
		private TextMeshProUGUI taiwuBubbleLabel;

		// Token: 0x04005D24 RID: 23844
		[SerializeField]
		private XuehouSelectRoot taiwuSelectRoot;

		// Token: 0x04005D25 RID: 23845
		[SerializeField]
		private CButton taiwuShowInfoButton;

		// Token: 0x04005D26 RID: 23846
		[SerializeField]
		private CToggleGroup taiwuNeiliToggleGroup;

		// Token: 0x04005D27 RID: 23847
		[SerializeField]
		private List<TextMeshProUGUI> taiwuNeiliTextList;

		// Token: 0x04005D28 RID: 23848
		[SerializeField]
		private TextMeshProUGUI jixiName;

		// Token: 0x04005D29 RID: 23849
		[SerializeField]
		private RectTransform jixiAnimationHolder;

		// Token: 0x04005D2A RID: 23850
		[SerializeField]
		private SkeletonGraphic jixiAnimation;

		// Token: 0x04005D2B RID: 23851
		[SerializeField]
		private Bubble jixiBubble;

		// Token: 0x04005D2C RID: 23852
		[SerializeField]
		private TextMeshProUGUI jixiBubbleLabel;

		// Token: 0x04005D2D RID: 23853
		[SerializeField]
		private TextMeshProUGUI jixiGrowLabel;

		// Token: 0x04005D2E RID: 23854
		[SerializeField]
		private CImage jixiGrowProgress;

		// Token: 0x04005D2F RID: 23855
		[SerializeField]
		private CImage jixiGrowProgressAdd;

		// Token: 0x04005D30 RID: 23856
		[SerializeField]
		private CImage jixiGrowProgressReduce;

		// Token: 0x04005D31 RID: 23857
		[SerializeField]
		private GameObject jixiGrowProgressTagRoot;

		// Token: 0x04005D32 RID: 23858
		[SerializeField]
		private TextMeshProUGUI jixiFavorLabel;

		// Token: 0x04005D33 RID: 23859
		[SerializeField]
		private CImage jixiFavorProgress;

		// Token: 0x04005D34 RID: 23860
		[SerializeField]
		private CImage jixiFavorIcon;

		// Token: 0x04005D35 RID: 23861
		[SerializeField]
		private XuehouSelectRoot jixiSelectRoot;

		// Token: 0x04005D36 RID: 23862
		[SerializeField]
		private ParticleSystem jixiEff;

		// Token: 0x04005D37 RID: 23863
		[SerializeField]
		private CButton jixiShowInfoButton;

		// Token: 0x04005D38 RID: 23864
		[SerializeField]
		private CButton translateNeiliButton;

		// Token: 0x04005D39 RID: 23865
		[SerializeField]
		private CButton recoverHealthButton;

		// Token: 0x04005D3A RID: 23866
		[SerializeField]
		private CButton statusButton;

		// Token: 0x04005D3B RID: 23867
		[SerializeField]
		private RectTransform jixiInfo;

		// Token: 0x04005D3C RID: 23868
		[SerializeField]
		private TextMeshProUGUI statusText;

		// Token: 0x04005D3D RID: 23869
		[SerializeField]
		private TextMeshProUGUI killedText;

		// Token: 0x04005D3E RID: 23870
		[SerializeField]
		private TextMeshProUGUI drainNeiliValueText;

		// Token: 0x04005D3F RID: 23871
		[SerializeField]
		private TextMeshProUGUI drainNeiliFromText;

		// Token: 0x04005D40 RID: 23872
		[SerializeField]
		private TextMeshProUGUI drainNeiliToText;

		// Token: 0x04005D41 RID: 23873
		[SerializeField]
		private TextMeshProUGUI killEnemyText;

		// Token: 0x04005D42 RID: 23874
		[SerializeField]
		private TextMeshProUGUI killEnemyDebtText;

		// Token: 0x04005D43 RID: 23875
		[SerializeField]
		private TextMeshProUGUI[] ageContentTexts;

		// Token: 0x04005D44 RID: 23876
		[SerializeField]
		private TextMeshProUGUI healthText;

		// Token: 0x04005D45 RID: 23877
		[SerializeField]
		private TextMeshProUGUI[] FiveElementsTexts;

		// Token: 0x04005D46 RID: 23878
		[SerializeField]
		private XuehouNeiliAllocationItem[] taiwuAllocationItems;

		// Token: 0x04005D47 RID: 23879
		[SerializeField]
		private XuehouNeiliAllocationItem[] jixiAllocationItems;

		// Token: 0x04005D48 RID: 23880
		[SerializeField]
		private ParticleSystem eff_xuehou_ui_dancixian1;

		// Token: 0x04005D49 RID: 23881
		[SerializeField]
		private ParticleSystem eff_xuehou_ui_huanrao1;

		// Token: 0x04005D4A RID: 23882
		[SerializeField]
		private ParticleSystem eff_xuehou_ui_lianyi1;

		// Token: 0x04005D4B RID: 23883
		[SerializeField]
		private ParticleSystem eff_xuehou_ui_lianyi2;

		// Token: 0x04005D4C RID: 23884
		[SerializeField]
		private ParticleSystem eff_xuehou_ui_shoudakai;

		// Token: 0x04005D4D RID: 23885
		[SerializeField]
		private ParticleSystem eff_xuehou_ui_shouhe;

		// Token: 0x04005D4E RID: 23886
		[SerializeField]
		private ParticleSystem eff_xuehou_ui_yin;

		// Token: 0x04005D4F RID: 23887
		[SerializeField]
		private ParticleSystem eff_xuehou_ui_zhakai;

		// Token: 0x04005D50 RID: 23888
		[SerializeField]
		private ParticleSystem[] eff_xuehou_ui_lianTaiwu;

		// Token: 0x04005D51 RID: 23889
		[SerializeField]
		private ParticleSystem[] eff_xuehou_ui_lianJixi;

		// Token: 0x04005D52 RID: 23890
		[SerializeField]
		private CanvasGroup effHolderForAlphaControl;

		// Token: 0x04005D53 RID: 23891
		[SerializeField]
		private RectTransform handClosed;

		// Token: 0x04005D54 RID: 23892
		[SerializeField]
		private RectTransform illustration;

		// Token: 0x04005D55 RID: 23893
		private JixiSpecialInteractDisplayData _specialData;

		// Token: 0x04005D56 RID: 23894
		private short _showedJixiTemplateId = -1;

		// Token: 0x04005D57 RID: 23895
		private int _taiwuId = -1;

		// Token: 0x04005D58 RID: 23896
		private CharacterDisplayData _taiwuCharacterDisplayData;

		// Token: 0x04005D59 RID: 23897
		private CharacterAvatar _taiwuCharacterAvatar;

		// Token: 0x04005D5A RID: 23898
		private int _taiwuTargetCharacterId = -1;

		// Token: 0x04005D5B RID: 23899
		private CharacterDisplayData _taiwuTargetCharacterDisplayData;

		// Token: 0x04005D5C RID: 23900
		private CharacterAvatar _taiwuTargetCharacterAvatar;

		// Token: 0x04005D5D RID: 23901
		private int TaiwuMaxNeiliAllocationProgress;

		// Token: 0x04005D5E RID: 23902
		private int[] _taiwuExtraNeiliAllocationProgress;

		// Token: 0x04005D5F RID: 23903
		private readonly CharacterSortFilterSettings _taiwuTargetCharSortFilterSettings = new CharacterSortFilterSettings
		{
			FilterType = 7,
			FilterSubType = 0
		};

		// Token: 0x04005D60 RID: 23904
		private int _jixiTargetCharacterId = -1;

		// Token: 0x04005D61 RID: 23905
		private CharacterDisplayData _jixiTargetCharacterDisplayData;

		// Token: 0x04005D62 RID: 23906
		private CharacterAvatar _jixiTargetCharacterAvatar;

		// Token: 0x04005D63 RID: 23907
		private bool _isFirstShow;

		// Token: 0x04005D64 RID: 23908
		private const float GrowProgressTagRootWidth = 500f;

		// Token: 0x04005D65 RID: 23909
		private const int MaxGrowProgressNum = 3000;

		// Token: 0x04005D66 RID: 23910
		private int JixiMaxNeiliAllocationProgress;

		// Token: 0x04005D67 RID: 23911
		private const short ProgressFavorFactor = 2000;

		// Token: 0x04005D68 RID: 23912
		private int _previewGrowProgress;

		// Token: 0x04005D69 RID: 23913
		private short _previewFavor;

		// Token: 0x04005D6A RID: 23914
		private readonly CharacterSortFilterSettings _jixiTargetCharSortFilterSettings = new CharacterSortFilterSettings
		{
			FilterType = 6,
			FilterSubType = 0
		};

		// Token: 0x04005D6B RID: 23915
		private readonly int[,] _curNeili = new int[2, 4];

		// Token: 0x04005D6C RID: 23916
		private readonly int[,] _previewNeili = new int[2, 4];

		// Token: 0x04005D6D RID: 23917
		private readonly int[,] _curNeiliAllocation = new int[2, 4];

		// Token: 0x04005D6E RID: 23918
		private readonly int[,] _previewNeiliAllocation = new int[2, 4];

		// Token: 0x04005D6F RID: 23919
		private readonly float[,] _neiliAllocationPercent = new float[2, 4];

		// Token: 0x04005D70 RID: 23920
		private readonly int[] _neiliTransferCount = new int[4];

		// Token: 0x04005D71 RID: 23921
		private bool _isShowJixiInfo;

		// Token: 0x04005D72 RID: 23922
		private CharacterDisplayDataForNeiliPage _data;

		// Token: 0x04005D73 RID: 23923
		private bool _isAnyEffOperate;

		// Token: 0x04005D74 RID: 23924
		private static readonly WaitForSeconds _waitForSeconds04 = new WaitForSeconds(0.4f);

		// Token: 0x04005D75 RID: 23925
		private static readonly WaitForSeconds _waitForSeconds06 = new WaitForSeconds(0.6f);

		// Token: 0x04005D76 RID: 23926
		private static readonly WaitForSeconds _waitForSeconds08 = new WaitForSeconds(0.8f);

		// Token: 0x04005D77 RID: 23927
		private static readonly WaitForSeconds _waitForSeconds1 = new WaitForSeconds(1f);

		// Token: 0x04005D78 RID: 23928
		private static readonly WaitForSeconds _waitForSeconds02 = new WaitForSeconds(0.2f);

		// Token: 0x04005D79 RID: 23929
		private static readonly WaitForSeconds _waitForSeconds05 = new WaitForSeconds(0.5f);

		// Token: 0x04005D7A RID: 23930
		private Coroutine _bubbleCoroutine;

		// Token: 0x04005D7B RID: 23931
		private const float delay = 3f;

		// Token: 0x04005D7C RID: 23932
		private static readonly WaitForSeconds _waitForSecondsHideBubbleDelay = new WaitForSeconds(3f);
	}
}
