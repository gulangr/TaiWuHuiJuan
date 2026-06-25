using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using Config;
using DG.Tweening;
using DG.Tweening.Core;
using DG.Tweening.Plugins.Options;
using FrameWork;
using Game.Components.ListStyleGeneralScroll;
using Game.Components.ListStyleGeneralScroll.Item;
using Game.Components.SortAndFilter;
using Game.Components.SortAndFilter.Item.Apply;
using GameData.DLC.FiveLoong;
using GameData.Domains.Extra;
using GameData.Domains.Item;
using GameData.Domains.Item.Display;
using GameData.Domains.Taiwu;
using GameData.GameDataBridge;
using GameData.Serializer;
using GameData.Utilities;
using TMPro;
using UnityEngine;

// Token: 0x020001B6 RID: 438
public class UI_JiaoChangeLoong : UIBase
{
	// Token: 0x06001A26 RID: 6694 RVA: 0x000AC7AC File Offset: 0x000AA9AC
	public override void OnInit(ArgumentBox argsBox)
	{
		this._items.Clear();
		this.InitItemListScroll();
		base.CGet<ParticleSystem>("Eff_Light").gameObject.SetActive(false);
		base.CGet<ParticleSystem>("Eff_Appear").gameObject.SetActive(false);
		base.CGet<ParticleSystem>("Eff_Change").gameObject.SetActive(false);
		ItemDisplayData loongScaleItemDisplayData = new ItemDisplayData
		{
			Key = new ItemKey
			{
				Id = -1,
				ItemType = 12,
				TemplateId = 276
			}
		};
		base.CGet<ItemView>("ItemLoongScale").SetData(loongScaleItemDisplayData, false, -1, false, true, null, false, true);
		this.NeedDataListenerId = true;
		UIElement element = this.Element;
		element.OnListenerIdReady = (Action)Delegate.Combine(element.OnListenerIdReady, new Action(this.OnListenerIdReady));
	}

	// Token: 0x06001A27 RID: 6695 RVA: 0x000AC88C File Offset: 0x000AAA8C
	private void InitItemListScroll()
	{
		base.CGet<ItemScrollView>("ItemScrollView").gameObject.SetActive(false);
		base.CGet<CToggleGroupObsolete>("Filter").gameObject.SetActive(false);
		this._itemListScroll = base.CGet<ItemListScroll>("ItemListScroll");
		this._itemListScroll.Init("UI_JiaoChangeLoong", ESortAndFilterControllerType.Empty, true, new Action<ITradeableContent, RowItemLine>(this.OnRenderItem), new Action<ITradeableContent, RowItemLine>(this.OnItemClick), ItemListScroll.EColumnType.IconAndName | ItemListScroll.EColumnType.Amount | ItemListScroll.EColumnType.Type | ItemListScroll.EColumnType.Weight | ItemListScroll.EColumnType.Value | ItemListScroll.EColumnType.Durability, null, null, null);
		SortAndFilter sortAndFilterView = this._itemListScroll.GetComponentInChildren<SortAndFilter>(true);
		sortAndFilterView.SetEntryButtonForceHidden(true);
		this.ReplaceItemListSortFilter(new JiaoPoolSelectItemSortAndFilterController(sortAndFilterView, false, UI_JiaoChangeLoong.EmptyJiaoMap));
		this._itemListScroll.OnSortAndFilterChangedCallback = new Action(this.RefreshRightArea);
	}

	// Token: 0x06001A28 RID: 6696 RVA: 0x000AC949 File Offset: 0x000AAB49
	private void OnEnable()
	{
		AudioManager.Instance.PlayAmbience("ui_building_jiaochi_hualong_loop", 1f, 100);
		AudioManager.Instance.PlayMusic("ui_building_jiaochi_hualong_music_loop", 1f, 100, null);
	}

	// Token: 0x06001A29 RID: 6697 RVA: 0x000AC97B File Offset: 0x000AAB7B
	private void OnDisable()
	{
		AudioManager.Instance.PlayAmbience(AudioManager.DummyAudioName, 1f, 100);
		AudioManager.Instance.PlayMusic(AudioManager.DummyAudioName, 1f, 100, null);
	}

	// Token: 0x06001A2A RID: 6698 RVA: 0x000AC9AD File Offset: 0x000AABAD
	private void OnListenerIdReady()
	{
		ExtraDomainMethod.Call.GetAllEvolvingJiao(this.Element.GameDataListenerId);
		TaiwuDomainMethod.Call.GetItemCount(this.Element.GameDataListenerId, 12, 276);
	}

	// Token: 0x06001A2B RID: 6699 RVA: 0x000AC9DC File Offset: 0x000AABDC
	public override void OnNotifyGameData(List<NotificationWrapper> notifications)
	{
		foreach (NotificationWrapper wrapper in notifications)
		{
			Notification notification = wrapper.Notification;
			byte type = notification.Type;
			byte b = type;
			if (b == 1)
			{
				bool flag = notification.DomainId == 19;
				if (flag)
				{
					bool flag2 = notification.MethodId == 97;
					if (flag2)
					{
						List<ItemDisplayData> adultItems = null;
						Serializer.Deserialize(wrapper.DataPool, notification.ValueOffset, ref adultItems);
						this._items.Clear();
						bool flag3 = adultItems != null;
						if (flag3)
						{
							this._items.AddRange(adultItems);
						}
						this.RefreshItemList();
					}
					else
					{
						bool flag4 = notification.MethodId == 98;
						if (flag4)
						{
							bool needAutoSelect = this._evolutionDisplayDataList == null;
							Serializer.Deserialize(wrapper.DataPool, notification.ValueOffset, ref this._evolutionDisplayDataList);
							this._items.Clear();
							this._selectedEvolutionData = null;
							bool flag5 = this._evolutionDisplayDataList != null;
							if (flag5)
							{
								foreach (JiaoEvolutionDisplayData data in this._evolutionDisplayDataList)
								{
									bool flag6 = needAutoSelect && this._selectedEvolutionData == null;
									if (flag6)
									{
										this._selectedEvolutionData = data;
									}
									this._items.Add(data.ItemDisplayData);
								}
							}
							this.RefreshItemList();
						}
						else
						{
							bool flag7 = notification.MethodId == 102;
							if (flag7)
							{
								JiaoEvolutionDisplayData data2 = null;
								Serializer.Deserialize(wrapper.DataPool, notification.ValueOffset, ref data2);
								bool isFirstTime = this._selectedEvolutionData.SimulationResult < 0;
								int index = this._evolutionDisplayDataList.IndexOf(this._selectedEvolutionData);
								bool flag8 = this._evolutionDisplayDataList.CheckIndex(index);
								if (flag8)
								{
									this._evolutionDisplayDataList[index] = data2;
									this._selectedEvolutionData = data2;
									base.StartCoroutine(this.StartShowEvolution(isFirstTime));
								}
								else
								{
									this._interactBlockFlag = false;
								}
							}
						}
					}
				}
				else
				{
					bool flag9 = notification.DomainId == 5;
					if (flag9)
					{
						bool flag10 = notification.MethodId == 77;
						if (flag10)
						{
							Serializer.Deserialize(wrapper.DataPool, notification.ValueOffset, ref this._scaleCount);
							this.RefreshScaleInfo();
							this.RefreshEvolutionButtons();
							this.Element.ShowAfterRefresh();
						}
					}
				}
			}
		}
	}

	// Token: 0x06001A2C RID: 6700 RVA: 0x000ACC98 File Offset: 0x000AAE98
	private void RefreshItemList()
	{
		bool flag = null == this._itemListScroll;
		if (!flag)
		{
			int selectedIndex = -1;
			bool flag2 = this._selectedEvolutionData != null;
			if (flag2)
			{
				selectedIndex = this._items.FindIndex((ItemDisplayData item) => item.Key.Equals(this._selectedEvolutionData.ItemDisplayData.Key));
			}
			this._itemListScroll.SetItemList(this._items, selectedIndex);
			this.RefreshRightArea();
		}
	}

	// Token: 0x06001A2D RID: 6701 RVA: 0x000ACCFC File Offset: 0x000AAEFC
	private void OnRenderItem(ITradeableContent itemData, RowItemLine rowItemLine)
	{
		ItemDisplayData data = itemData as ItemDisplayData;
		bool flag = data == null;
		if (!flag)
		{
			JiaoEvolutionDisplayData evolutionDisplayData = this._evolutionDisplayDataList.Find((JiaoEvolutionDisplayData e) => e.ItemDisplayData.Key.Equals(data.Key));
			bool flag2 = evolutionDisplayData == null;
			if (flag2)
			{
				PredefinedLog.Show(17);
				rowItemLine.SetInteractable(false, true);
			}
			else
			{
				RowItemMain rowItemMain = rowItemLine.GetComponentInChildren<RowItemMain>();
				rowItemMain.SetData(data);
				rowItemLine.Set(rowItemMain, true);
				rowItemMain.HideInteractionState();
				rowItemLine.SetInteractable(!this._interactBlockFlag, true);
				rowItemLine.SetSelected(this._selectedEvolutionData != null && data == this._selectedEvolutionData.ItemDisplayData);
			}
		}
	}

	// Token: 0x06001A2E RID: 6702 RVA: 0x000ACDC4 File Offset: 0x000AAFC4
	private void OnItemClick(ITradeableContent itemData, RowItemLine rowItemLine)
	{
		ItemDisplayData data;
		bool flag;
		if (!this._interactBlockFlag)
		{
			data = (itemData as ItemDisplayData);
			flag = (data == null);
		}
		else
		{
			flag = true;
		}
		bool flag2 = flag;
		if (!flag2)
		{
			JiaoEvolutionDisplayData evolutionDisplayData = this._evolutionDisplayDataList.Find((JiaoEvolutionDisplayData e) => e.ItemDisplayData.Key.Equals(data.Key));
			bool flag3 = evolutionDisplayData == null;
			if (!flag3)
			{
				bool flag4 = this._selectedEvolutionData != null && !this._selectedEvolutionData.ItemDisplayData.Key.Equals(data.Key);
				if (flag4)
				{
					RowItemLine rowItemLine2 = this._itemListScroll.FindActiveItem(this._selectedEvolutionData.ItemDisplayData.Key, false);
					if (rowItemLine2 != null)
					{
						rowItemLine2.SetSelected(false);
					}
				}
				rowItemLine.SetSelected(true);
				this._selectedEvolutionData = evolutionDisplayData;
				this.RefreshRightArea();
			}
		}
	}

	// Token: 0x06001A2F RID: 6703 RVA: 0x000ACEA0 File Offset: 0x000AB0A0
	private void ReplaceItemListSortFilter(JiaoPoolSelectItemSortAndFilterController newController)
	{
		Type listType = typeof(ItemListScroll);
		FieldInfo field = listType.GetField("_sortAndFilterController", BindingFlags.Instance | BindingFlags.NonPublic);
		SortAndFilterController<ITradeableContent> oldController = ((field != null) ? field.GetValue(this._itemListScroll) : null) as SortAndFilterController<ITradeableContent>;
		if (oldController != null)
		{
			oldController.UninitForReplace();
		}
		newController.Init(new Action(this.OnItemListSortAndFilterChanged), "UI_JiaoChangeLoong");
		FieldInfo field2 = listType.GetField("_sortAndFilterController", BindingFlags.Instance | BindingFlags.NonPublic);
		if (field2 != null)
		{
			field2.SetValue(this._itemListScroll, newController);
		}
		FieldInfo field3 = listType.GetField("scroll", BindingFlags.Instance | BindingFlags.NonPublic);
		ListStyleGeneralScroll rowScroll = ((field3 != null) ? field3.GetValue(this._itemListScroll) : null) as ListStyleGeneralScroll;
		bool flag = rowScroll != null;
		if (flag)
		{
			rowScroll.SetSortController(newController);
		}
		FieldInfo field4 = listType.GetField("cardScroll", BindingFlags.Instance | BindingFlags.NonPublic);
		CardStyleGeneralScroll cardScroll = ((field4 != null) ? field4.GetValue(this._itemListScroll) : null) as CardStyleGeneralScroll;
		bool flag2 = cardScroll != null;
		if (flag2)
		{
			cardScroll.SetSortController(newController);
		}
	}

	// Token: 0x06001A30 RID: 6704 RVA: 0x000ACF90 File Offset: 0x000AB190
	private void OnItemListSortAndFilterChanged()
	{
		int selectedIndex = -1;
		bool flag = this._selectedEvolutionData != null;
		if (flag)
		{
			selectedIndex = this._items.FindIndex((ItemDisplayData item) => item.Key.Equals(this._selectedEvolutionData.ItemDisplayData.Key));
		}
		this._itemListScroll.SetItemList(this._items, selectedIndex);
		this.RefreshRightArea();
	}

	// Token: 0x06001A31 RID: 6705 RVA: 0x000ACFE0 File Offset: 0x000AB1E0
	private void RefreshScaleInfo()
	{
		base.CGet<TextMeshProUGUI>("ScaleCount").text = this._scaleCount.ToString();
		string colorName = (this._scaleCount >= GlobalConfig.Instance.RequiredLoongScaleForEvolution) ? "brightblue" : "brightred";
		base.CGet<TextMeshProUGUI>("ConsumeScaleCount_1").text = string.Format("-{0}", GlobalConfig.Instance.RequiredLoongScaleForEvolution).SetColor(colorName);
		colorName = ((this._scaleCount >= GlobalConfig.Instance.RequiredLoongScaleForFirstTimeEvolution) ? "brightblue" : "brightred");
		base.CGet<TextMeshProUGUI>("ConsumeScaleCount_2").text = string.Format("-{0}", GlobalConfig.Instance.RequiredLoongScaleForFirstTimeEvolution).SetColor(colorName);
	}

	// Token: 0x06001A32 RID: 6706 RVA: 0x000AD0A8 File Offset: 0x000AB2A8
	private void RefreshRightArea()
	{
		this.RefreshEvolutionResult();
		this.RefreshEvolutionButtons();
	}

	// Token: 0x06001A33 RID: 6707 RVA: 0x000AD0BC File Offset: 0x000AB2BC
	private void RefreshEvolutionButtons()
	{
		base.CGet<GameObject>("ButtonRoot").SetActive(this._selectedEvolutionData != null);
		bool flag = this._selectedEvolutionData == null;
		if (!flag)
		{
			bool scaleMeet = this._scaleCount >= GlobalConfig.Instance.RequiredLoongScaleForFirstTimeEvolution;
			bool hasEvolved = this._selectedEvolutionData.SimulationResult >= 0;
			base.CGet<CButtonObsolete>("ButtonChangeLoong").interactable = (scaleMeet || hasEvolved);
			base.CGet<CButtonObsolete>("ButtonChangeLoong").GetComponentInChildren<TextMeshProUGUI>().color = Colors.Instance[(scaleMeet || hasEvolved) ? "pinkyellow" : "grey"];
			base.CGet<GameObject>("Eff_BtnStart").SetActive(scaleMeet || hasEvolved);
			base.CGet<CButtonObsolete>("ButtonChangeLoong").GetComponent<TooltipInvoker>().enabled = !scaleMeet;
			base.CGet<CButtonObsolete>("ButtonChangeLoong").GetComponentInChildren<TextMeshProUGUI>().text = LocalStringManager.Get(hasEvolved ? LanguageKey.LK_JiaoPool_ChangeLonng_AcceptResult : LanguageKey.LK_JiaoPool_Hualong);
			base.CGet<GameObject>("LoongScaleConsume_2").SetActive(!hasEvolved);
			bool canSwitchResult = hasEvolved && this._selectedEvolutionData.Status == 0;
			base.CGet<GameObject>("Eff_BtnSwitch").SetActive(canSwitchResult);
			base.CGet<CButtonObsolete>("ButtonSwitchResult").interactable = canSwitchResult;
			base.CGet<CButtonObsolete>("ButtonSwitchResult").GetComponentInChildren<TextMeshProUGUI>().color = Colors.Instance[canSwitchResult ? "pinkyellow" : "grey"];
			TooltipInvoker mouseTipDisplayer = base.CGet<CButtonObsolete>("ButtonSwitchResult").GetComponent<TooltipInvoker>();
			base.CGet<CButtonObsolete>("ButtonSwitchResult").GetComponentInChildren<TextMeshProUGUI>().color = Colors.Instance[canSwitchResult ? "pinkyellow" : "grey"];
			mouseTipDisplayer.enabled = !canSwitchResult;
			bool flag2 = !canSwitchResult;
			if (flag2)
			{
				sbyte status = this._selectedEvolutionData.Status;
				if (!true)
				{
				}
				LanguageKey languageKey;
				switch (status)
				{
				case 1:
					languageKey = LanguageKey.LK_JiaoPool_ChangeLonng_DisableTips2;
					break;
				case 2:
					languageKey = LanguageKey.LK_JiaoPool_ChangeLonng_DisableTips1;
					break;
				case 3:
					languageKey = LanguageKey.LK_JiaoEvolute_Scale_Not_Enough_Tip;
					break;
				default:
					languageKey = LanguageKey.Invalid;
					break;
				}
				if (!true)
				{
				}
				LanguageKey langId = languageKey;
				mouseTipDisplayer.PresetParam = new string[]
				{
					LocalStringManager.Get(langId).SetColor("brightred")
				};
			}
		}
	}

	// Token: 0x06001A34 RID: 6708 RVA: 0x000AD304 File Offset: 0x000AB504
	private void RefreshEvolutionResult()
	{
		GameObject resultCanvasObj = base.CGet<GameObject>("ResultCanvas");
		bool flag = this._selectedEvolutionData == null;
		if (flag)
		{
			resultCanvasObj.SetActive(false);
			base.CGet<CanvasGroup>("LoongNameArea").gameObject.SetActive(false);
		}
		else
		{
			bool flag2 = this._selectedEvolutionData.SimulationResult >= 0;
			if (flag2)
			{
				this.SetSimulateResultInfo();
			}
			else
			{
				this.SetJiaoInfo();
			}
			resultCanvasObj.gameObject.SetActive(true);
		}
	}

	// Token: 0x06001A35 RID: 6709 RVA: 0x000AD380 File Offset: 0x000AB580
	private void SetSimulateResultInfo()
	{
		JiaoItem config = Config.Jiao.Instance.GetItem(this._selectedEvolutionData.SimulationResult);
		CharacterItem characterConfig = Character.Instance[config.IndexOfCharacterTemplate];
		string avatarName = this._selectedEvolutionData.IsOwnedResult ? characterConfig.FixedAvatarName : config.ShadowImage;
		ResLoader.Load<Sprite>("RemakeResources/Textures/NpcFace/BigFace/" + avatarName, delegate(Sprite sprite)
		{
			CImage jiaoImage = this.CGet<CImage>("JiaoImage");
			CImage loongImage = this.CGet<CImage>("LoongImage");
			jiaoImage.enabled = false;
			loongImage.sprite = sprite;
			loongImage.fillAmount = 1f;
			loongImage.enabled = true;
			CanvasGroup nameCanvasGroup = this.CGet<CanvasGroup>("LoongNameArea");
			nameCanvasGroup.alpha = 1f;
			nameCanvasGroup.gameObject.SetActive(true);
			this.CGet<TextMeshProUGUI>("LoongName").text = (this._selectedEvolutionData.IsOwnedResult ? config.Name : "? ? ?");
		}, null, false);
	}

	// Token: 0x06001A36 RID: 6710 RVA: 0x000AD40C File Offset: 0x000AB60C
	private void SetJiaoInfo()
	{
		CImage jiaoImage = base.CGet<CImage>("JiaoImage");
		CImage loongImage = base.CGet<CImage>("LoongImage");
		JiaoItem srcJiaoConfig = this.GetJiaoConfigByItemKey(this._selectedEvolutionData.ItemDisplayData.Key);
		CharacterItem characterConfig = Character.Instance[srcJiaoConfig.IndexOfCharacterTemplate];
		ResLoader.Load<Sprite>("RemakeResources/Textures/NpcFace/BigFace/" + characterConfig.FixedAvatarName, delegate(Sprite sprite)
		{
			loongImage.enabled = false;
			jiaoImage.fillAmount = 1f;
			jiaoImage.color = Color.white;
			jiaoImage.sprite = sprite;
			jiaoImage.enabled = true;
		}, null, false);
		ExtraDomainMethod.AsyncCall.GetJiaoByItemKey(null, this._selectedEvolutionData.ItemDisplayData.Key, delegate(int offset, RawDataPool dataPool)
		{
			GameData.DLC.FiveLoong.Jiao jiao = new GameData.DLC.FiveLoong.Jiao();
			Serializer.Deserialize(dataPool, offset, ref jiao);
			this.CGet<TextMeshProUGUI>("LoongName").text = jiao.GetNameText();
		});
		CanvasGroup nameCanvasGroup = base.CGet<CanvasGroup>("LoongNameArea");
		nameCanvasGroup.alpha = 1f;
		nameCanvasGroup.gameObject.SetActive(true);
	}

	// Token: 0x06001A37 RID: 6711 RVA: 0x000AD4E0 File Offset: 0x000AB6E0
	private JiaoItem GetJiaoConfigByItemKey(ItemKey key)
	{
		bool flag = key.ItemType != 4;
		JiaoItem result;
		if (flag)
		{
			result = null;
		}
		else
		{
			JiaoItem config = null;
			Config.Jiao.Instance.Iterate(delegate(JiaoItem e)
			{
				bool flag2 = e.IndexOfCarrierTemplate == key.TemplateId;
				if (flag2)
				{
					config = e;
				}
				return true;
			});
			result = config;
		}
		return result;
	}

	// Token: 0x06001A38 RID: 6712 RVA: 0x000AD53C File Offset: 0x000AB73C
	private IEnumerator StartShowEvolution(bool isFirstTime)
	{
		CanvasGroup nameCanvasGroup = base.CGet<CanvasGroup>("LoongNameArea");
		CImage jiaoImage = base.CGet<CImage>("JiaoImage");
		CImage loongImage = base.CGet<CImage>("LoongImage");
		bool flag = !isFirstTime;
		if (flag)
		{
			jiaoImage.sprite = loongImage.sprite;
		}
		else
		{
			JiaoItem srcJiaoConfig = this.GetJiaoConfigByItemKey(this._selectedEvolutionData.ItemDisplayData.Key);
			CharacterItem srcCharacterConfig = Character.Instance[srcJiaoConfig.IndexOfCharacterTemplate];
			ResLoader.Load<Sprite>("RemakeResources/Textures/NpcFace/BigFace/" + srcCharacterConfig.FixedAvatarName, delegate(Sprite sprite)
			{
				jiaoImage.sprite = sprite;
				jiaoImage.enabled = true;
			}, null, false);
			srcJiaoConfig = null;
			srcCharacterConfig = null;
		}
		JiaoItem config = Config.Jiao.Instance.GetItem(this._selectedEvolutionData.SimulationResult);
		AudioManager.Instance.PlaySound(config.BellowSound, false, false);
		CharacterItem characterConfig = Character.Instance[config.IndexOfCharacterTemplate];
		string avatarName = this._selectedEvolutionData.IsOwnedResult ? characterConfig.FixedAvatarName : config.ShadowImage;
		ResLoader.Load<Sprite>("RemakeResources/Textures/NpcFace/BigFace/" + avatarName, delegate(Sprite sprite)
		{
			loongImage.sprite = sprite;
			loongImage.enabled = true;
			this.CGet<TextMeshProUGUI>("LoongName").text = (this._selectedEvolutionData.IsOwnedResult ? config.Name : "? ? ?");
		}, null, false);
		base.CGet<ParticleSystem>("Eff_Appear").gameObject.SetActive(isFirstTime);
		base.CGet<ParticleSystem>("Eff_Light").gameObject.SetActive(!isFirstTime);
		base.CGet<ParticleSystem>("Eff_Change").gameObject.SetActive(!isFirstTime);
		if (isFirstTime)
		{
			base.CGet<ParticleSystem>("Eff_Appear").Play(true);
		}
		else
		{
			base.CGet<ParticleSystem>("Eff_Light").Play(true);
			base.CGet<ParticleSystem>("Eff_Change").Play(true);
		}
		base.CGet<GameObject>("ResultCanvas").SetActive(true);
		if (isFirstTime)
		{
			AudioManager.Instance.PlaySound("ui_building_jiaochi_hualong_chuxian", false, false);
			jiaoImage.fillAmount = 1f;
			loongImage.fillAmount = 0f;
			loongImage.color = Color.white;
			jiaoImage.DOFade(0f, 1f).SetDelay(0.8f);
			loongImage.DOFillAmount(1f, 1.1f).SetDelay(1.5f);
			nameCanvasGroup.alpha = 0f;
			nameCanvasGroup.gameObject.SetActive(true);
			nameCanvasGroup.DOFade(1f, 1f).SetDelay(2f);
		}
		else
		{
			AudioManager.Instance.PlaySound("ui_building_jiaochi_hualong_again", false, false);
			nameCanvasGroup.gameObject.SetActive(false);
			jiaoImage.fillAmount = 1f;
			loongImage.fillAmount = 0f;
			DOVirtual.Float(0f, 1f, 2f, delegate(float stepValue)
			{
				jiaoImage.fillAmount = 1f - stepValue;
				loongImage.fillAmount = stepValue;
				bool flag2 = stepValue >= 0.5f && !nameCanvasGroup.gameObject.activeSelf;
				if (flag2)
				{
					nameCanvasGroup.gameObject.SetActive(true);
					nameCanvasGroup.alpha = 0f;
					nameCanvasGroup.DOFade(1f, 1f).SetDelay(2f);
				}
			});
		}
		yield return new WaitForSeconds(3f);
		this._interactBlockFlag = false;
		TaiwuDomainMethod.Call.GetItemCount(this.Element.GameDataListenerId, 12, 276);
		yield break;
	}

	// Token: 0x06001A39 RID: 6713 RVA: 0x000AD554 File Offset: 0x000AB754
	protected override void OnClick(Transform btn)
	{
		bool interactBlockFlag = this._interactBlockFlag;
		if (!interactBlockFlag)
		{
			string name = btn.name;
			string a = name;
			if (!(a == "ButtonSwitchResult"))
			{
				if (!(a == "ButtonChangeLoong"))
				{
					if (a == "ButtonClose")
					{
						this.QuickHide();
					}
				}
				else
				{
					btn.GetComponent<CButtonObsolete>().interactable = false;
					base.CGet<GameObject>("Eff_BtnStart").SetActive(false);
					bool flag = this._selectedEvolutionData.SimulationResult >= 0;
					if (flag)
					{
						this.OnClickConfirm();
					}
					else
					{
						this.StartChangeOnce();
					}
				}
			}
			else
			{
				btn.GetComponent<CButtonObsolete>().interactable = false;
				base.CGet<GameObject>("Eff_BtnSwitch").SetActive(false);
				this.StartChangeOnce();
			}
		}
	}

	// Token: 0x06001A3A RID: 6714 RVA: 0x000AD61E File Offset: 0x000AB81E
	public override void QuickHide()
	{
		this._interactBlockFlag = false;
		this._selectedEvolutionData = null;
		this._evolutionDisplayDataList = null;
		this._items.Clear();
		base.QuickHide();
	}

	// Token: 0x06001A3B RID: 6715 RVA: 0x000AD64C File Offset: 0x000AB84C
	private void StartChangeOnce()
	{
		string title = LocalStringManager.Get(LanguageKey.LK_JiaoPool_Hualong);
		string content = LocalStringManager.Get(LanguageKey.LK_JiaoPool_ChangeLonng_StartChangeConfirm);
		CommonUtils.ShowConfirmDialog(title, content, delegate
		{
			this._interactBlockFlag = true;
			ExtraDomainMethod.Call.GetNextRandomChildrenOfLoong(this.Element.GameDataListenerId, this._selectedEvolutionData.JiaoId);
		}, new Action(this.RefreshEvolutionButtons), EDialogType.None);
	}

	// Token: 0x06001A3C RID: 6716 RVA: 0x000AD694 File Offset: 0x000AB894
	private void OnClickConfirm()
	{
		string title = LocalStringManager.Get(LanguageKey.LK_JiaoPool_Hualong);
		string content = LocalStringManager.Get(LanguageKey.LK_JiaoPool_Hualong_Confirm);
		CommonUtils.ShowConfirmDialog(title, content, delegate
		{
			base.CGet<ParticleSystem>("Eff_Light").gameObject.SetActive(false);
			base.StartCoroutine(this.ShowConfirmGetChildOfLoong());
		}, new Action(this.RefreshEvolutionButtons), EDialogType.None);
	}

	// Token: 0x06001A3D RID: 6717 RVA: 0x000AD6D9 File Offset: 0x000AB8D9
	private IEnumerator ShowConfirmGetChildOfLoong()
	{
		RectTransform resultRoot = base.CGet<GameObject>("ResultCanvas").transform as RectTransform;
		CImage jiaoImage = base.CGet<CImage>("JiaoImage");
		CImage loongImage = base.CGet<CImage>("LoongImage");
		jiaoImage.sprite = loongImage.sprite;
		jiaoImage.fillAmount = 1f;
		jiaoImage.color = Color.white;
		loongImage.color = new Color(1f, 1f, 1f, 0f);
		JiaoItem config = Config.Jiao.Instance.GetItem(this._selectedEvolutionData.SimulationResult);
		CharacterItem characterConfig = Character.Instance[config.IndexOfCharacterTemplate];
		ResLoader.Load<Sprite>("RemakeResources/Textures/NpcFace/BigFace/" + characterConfig.FixedAvatarName, delegate(Sprite sprite)
		{
			loongImage.sprite = sprite;
			loongImage.enabled = true;
		}, null, false);
		base.CGet<CanvasGroup>("LoongNameArea").alpha = 0f;
		Sequence sequence = DOTween.Sequence();
		sequence.Append(resultRoot.DOScale(Vector3.one * 0.8f, 0.5f).SetDelay(0.2f));
		bool flag = !this._selectedEvolutionData.IsOwnedResult;
		if (flag)
		{
			sequence.Join(jiaoImage.DOFade(0f, 0.7f));
			sequence.Join(loongImage.DOFade(1f, 0.5f).SetDelay(0.4f));
			sequence.AppendCallback(delegate
			{
				this.CGet<TextMeshProUGUI>("LoongName").text = config.Name;
				this.CGet<CanvasGroup>("LoongNameArea").alpha = 1f;
			});
		}
		sequence.Append(resultRoot.DOScale(Vector3.one, 0.5f));
		sequence.Play<Sequence>();
		AudioManager.Instance.PlaySound("ui_building_jiaochi_hualong_again_confirm", false, false);
		base.CGet<ParticleSystem>("Eff_Confirm").gameObject.SetActive(true);
		base.CGet<ParticleSystem>("Eff_Confirm").Play(true);
		yield return new WaitForSeconds(1.2f);
		base.CGet<ParticleSystem>("Eff_Confirm").gameObject.SetActive(false);
		bool flag2 = !this._selectedEvolutionData.IsOwnedResult;
		if (flag2)
		{
			yield return new WaitForSeconds(1f);
		}
		ExtraDomainMethod.Call.JiaoEvolveToChildOfLoong(this._selectedEvolutionData.ItemDisplayData.Key, this._selectedEvolutionData.SimulationResult);
		bool flag3 = this._evolutionDisplayDataList.Count == 1;
		if (flag3)
		{
			this.QuickHide();
		}
		else
		{
			ExtraDomainMethod.Call.GetAllEvolvingJiao(this.Element.GameDataListenerId);
		}
		jiaoImage.color = Color.white;
		loongImage.color = Color.white;
		yield break;
	}

	// Token: 0x06001A3E RID: 6718 RVA: 0x000AD6E8 File Offset: 0x000AB8E8
	public void PlaySound(string soundName)
	{
		AudioManager.Instance.PlaySound(soundName, false, false);
	}

	// Token: 0x040014A8 RID: 5288
	private const string SortSaveKey = "UI_JiaoChangeLoong";

	// Token: 0x040014A9 RID: 5289
	private const BindingFlags ItemListNonPublic = BindingFlags.Instance | BindingFlags.NonPublic;

	// Token: 0x040014AA RID: 5290
	private static readonly IReadOnlyDictionary<ItemKey, GameData.DLC.FiveLoong.Jiao> EmptyJiaoMap = new Dictionary<ItemKey, GameData.DLC.FiveLoong.Jiao>();

	// Token: 0x040014AB RID: 5291
	private int _scaleCount;

	// Token: 0x040014AC RID: 5292
	private ItemListScroll _itemListScroll;

	// Token: 0x040014AD RID: 5293
	private List<JiaoEvolutionDisplayData> _evolutionDisplayDataList;

	// Token: 0x040014AE RID: 5294
	private readonly List<ItemDisplayData> _items = new List<ItemDisplayData>();

	// Token: 0x040014AF RID: 5295
	private JiaoEvolutionDisplayData _selectedEvolutionData;

	// Token: 0x040014B0 RID: 5296
	private bool _interactBlockFlag;

	// Token: 0x040014B1 RID: 5297
	private const string JiaoFaceFolderPath = "RemakeResources/Textures/NpcFace/BigFace/";
}
