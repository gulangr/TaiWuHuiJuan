using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Config;
using DG.Tweening;
using DG.Tweening.Core;
using DG.Tweening.Plugins.Options;
using DisplayConfig;
using FrameWork;
using Game.Components.Avatar;
using Game.Views.CharacterMenu;
using GameData.Common;
using GameData.Domains.Character;
using GameData.Domains.Character.Display;
using GameData.Domains.Character.SortFilter;
using GameData.Domains.Story;
using GameData.Domains.Story.SectMainStory;
using GameData.Domains.TaiwuEvent;
using GameData.GameDataBridge;
using GameData.Serializer;
using GameData.Utilities;
using TMPro;
using UICommon.Character;
using UnityEngine;

// Token: 0x0200038B RID: 907
public class UI_LifeLink : UIBase
{
	// Token: 0x060035C7 RID: 13767 RVA: 0x001B0792 File Offset: 0x001AE992
	public override void OnInit(ArgumentBox argsBox)
	{
		this._charNeiliProportionOfFiveElementsDict.Clear();
		this._sortFilterSettings = new CharacterSortFilterSettings
		{
			FilterType = 3,
			FilterSubType = 0
		};
		this._neiliTypeInitialized = false;
	}

	// Token: 0x060035C8 RID: 13768 RVA: 0x001B07C4 File Offset: 0x001AE9C4
	private void Awake()
	{
		this._onConfirmSelect = new Action<int>(this.OnConfirmSelect);
		this._openCharacterMenu = new Action<int>(this.OpenCharacterMenu);
		this._disableCondition = new Predicate<CharacterDisplayDataForUltimateSelect>(this.CharacterDisableCondition);
		this._lifeGateCharacterAvatars = new CharacterAvatar[8];
		this._deathGateCharacterAvatars = new CharacterAvatar[8];
		this._lifeGateCharacterNames = new CharacterName[8];
		this._deathGateCharacterNames = new CharacterName[8];
		for (int i = 0; i < 8; i++)
		{
			this._lifeGateCharacterAvatars[i] = new CharacterAvatar(this._lifeGateCharacterRefers[i].CGet<Game.Components.Avatar.Avatar>("Avatar"), true);
			this._lifeGateCharacterNames[i] = new CharacterName(this._lifeGateCharacterRefers[i].CGet<TextMeshProUGUI>("Name"), null, null);
			this.InitButtons(this._lifeGateCharacterRefers[i], true, i);
			this._deathGateCharacterAvatars[i] = new CharacterAvatar(this._deathGateCharacterRefers[i].CGet<Game.Components.Avatar.Avatar>("Avatar"), true);
			this._deathGateCharacterNames[i] = new CharacterName(this._deathGateCharacterRefers[i].CGet<TextMeshProUGUI>("Name"), null, null);
			this.InitButtons(this._deathGateCharacterRefers[i], false, i);
		}
	}

	// Token: 0x060035C9 RID: 13769 RVA: 0x001B08F4 File Offset: 0x001AEAF4
	protected override void OnClick(Transform btn)
	{
		string name = btn.name;
		string a = name;
		if (a == "ButtonClosePopup")
		{
			this.QuickHide();
		}
	}

	// Token: 0x060035CA RID: 13770 RVA: 0x001B0923 File Offset: 0x001AEB23
	private void OnDisable()
	{
		TaiwuEventDomainMethod.Call.TriggerListener("FinishBaihuaSectMainStorySpecial", true);
	}

	// Token: 0x060035CB RID: 13771 RVA: 0x001B0934 File Offset: 0x001AEB34
	private void InitButtons(Refers refers, bool isLifeGate, int index)
	{
		UI_LifeLink.<>c__DisplayClass28_0 CS$<>8__locals1 = new UI_LifeLink.<>c__DisplayClass28_0();
		CS$<>8__locals1.<>4__this = this;
		CS$<>8__locals1.isLifeGate = isLifeGate;
		CS$<>8__locals1.index = index;
		Action selectAction = delegate()
		{
			CS$<>8__locals1.<>4__this.OnSelectCharacter(CS$<>8__locals1.isLifeGate, CS$<>8__locals1.index);
		};
		CButtonObsolete selectBtn = refers.CGet<CButtonObsolete>("SelectCharBtn");
		selectBtn.ClearAndAddListener(selectAction);
		CButtonObsolete changeBtn = refers.CGet<CButtonObsolete>("ChangeCharBtn");
		changeBtn.ClearAndAddListener(selectAction);
		CButtonObsolete removeBtn = refers.CGet<CButtonObsolete>("RemoveCharBtn");
		removeBtn.ClearAndAddListener(new Action(CS$<>8__locals1.<InitButtons>g__RemoveAction|1));
		changeBtn.gameObject.SetActive(false);
	}

	// Token: 0x060035CC RID: 13772 RVA: 0x001B09C4 File Offset: 0x001AEBC4
	private void SetNeiliType(sbyte neiliTypeTemplateId)
	{
		NeiliTypeItem neiliTypeCfg = (neiliTypeTemplateId >= 0) ? NeiliType.Instance[neiliTypeTemplateId] : null;
		int fiveElementType = ((int)((neiliTypeCfg != null) ? new byte?(neiliTypeCfg.FiveElements) : null)) ?? -1;
		for (int i = 0; i < 5; i++)
		{
			bool isCurrEffect = i == fiveElementType;
			this._neiliEffects[i].SetActive(isCurrEffect);
			this._backgroundEffects[i].SetActive(isCurrEffect);
			this._flowerEffects[i].SetActive(isCurrEffect);
		}
		base.CGet<TextMeshProUGUI>("NeiliTypeName").text = (((neiliTypeCfg != null) ? neiliTypeCfg.Name : null) ?? string.Empty);
		base.CGet<GameObject>("NeiliTypeNameBack").SetActive(neiliTypeTemplateId >= 0);
		Debug.Log(string.Format("FiveElementType: {0}, Initialized: {1}", fiveElementType, this._neiliTypeInitialized));
		CImage fiveElementBackFill = base.CGet<CImage>("FiveElementBackFill");
		CImage fiveElementBack = fiveElementBackFill.transform.parent.GetComponent<CImage>();
		CImage fiveElementFrontFill = base.CGet<CImage>("FiveElementFrontFill");
		CImage fiveElementFront = fiveElementFrontFill.transform.parent.GetComponent<CImage>();
		bool flag = fiveElementType >= 0;
		if (flag)
		{
			string backImageName = string.Format("popup_lifelink_fiveelements_{0}_1", fiveElementType);
			string frontImageName = string.Format("popup_lifelink_fiveelements_{0}_0", fiveElementType);
			bool neiliTypeInitialized = this._neiliTypeInitialized;
			if (neiliTypeInitialized)
			{
				fiveElementBackFill.fillAmount = 0f;
				fiveElementBackFill.SetSprite(backImageName, false, null);
				fiveElementBackFill.DOFillAmount(1f, 0.5f).OnComplete(delegate
				{
					fiveElementBack.SetSprite(backImageName, false, null);
				});
				fiveElementFrontFill.SetAlpha(0f);
				fiveElementFrontFill.SetSprite(frontImageName, false, null);
				fiveElementFrontFill.DOFade(1f, 0.5f).OnComplete(delegate
				{
					fiveElementFront.SetSprite(frontImageName, false, null);
				});
			}
			else
			{
				fiveElementBackFill.SetSprite(backImageName, false, null);
				fiveElementBackFill.fillAmount = 1f;
				fiveElementBack.SetSprite(backImageName, false, null);
				fiveElementFrontFill.SetSprite(frontImageName, false, null);
				fiveElementFrontFill.SetAlpha(1f);
				fiveElementFront.SetSprite(frontImageName, false, null);
			}
		}
		else
		{
			string backImageName2 = "popup_lifelink_fiveelements_6_1";
			string frontImageName2 = "popup_lifelink_fiveelements_6_0";
			bool neiliTypeInitialized2 = this._neiliTypeInitialized;
			if (neiliTypeInitialized2)
			{
				fiveElementBack.SetSprite(backImageName2, false, null);
				fiveElementBackFill.DOFillAmount(0f, 0.5f);
				fiveElementFront.SetSprite(frontImageName2, false, null);
				fiveElementFrontFill.DOFade(0f, 0.5f);
			}
			else
			{
				fiveElementBackFill.SetSprite(backImageName2, false, null);
				fiveElementBackFill.fillAmount = 1f;
				fiveElementBack.SetSprite(backImageName2, false, null);
				fiveElementFrontFill.SetSprite(frontImageName2, false, null);
				fiveElementFrontFill.SetAlpha(1f);
				fiveElementFront.SetSprite(frontImageName2, false, null);
			}
		}
		TooltipInvoker mouseTipDisplayer = base.CGet<TooltipInvoker>("NeiliTypeTipDisplayer");
		mouseTipDisplayer.enabled = (neiliTypeCfg != null);
		bool flag2 = neiliTypeCfg != null;
		if (flag2)
		{
			this._lifeLinkNeiliTypeTipArgumentBox.Set("NeiliType", neiliTypeTemplateId);
			mouseTipDisplayer.Type = TipType.LifeLinkNeiliType;
			mouseTipDisplayer.RuntimeParam = this._lifeLinkNeiliTypeTipArgumentBox;
		}
		this._neiliTypeInitialized = true;
	}

	// Token: 0x060035CD RID: 13773 RVA: 0x001B0D76 File Offset: 0x001AEF76
	public override void InitMonitorFieldIds()
	{
		this.MonitorFields.Add(new UIBase.MonitorDataField(19, 117, ulong.MaxValue, null));
	}

	// Token: 0x060035CE RID: 13774 RVA: 0x001B0D94 File Offset: 0x001AEF94
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
					this.HandleMethodReturn(notification.DomainId, notification.MethodId, notification.ValueOffset, wrapper.DataPool);
				}
			}
			else
			{
				this.HandleDataMonitor(notification.Uid, notification.ValueOffset, wrapper.DataPool);
			}
		}
	}

	// Token: 0x060035CF RID: 13775 RVA: 0x001B0E3C File Offset: 0x001AF03C
	private void HandleMethodReturn(ushort domainId, ushort methodId, int offset, RawDataPool dataPool)
	{
		bool flag = domainId == 20 && methodId == 3;
		if (flag)
		{
			sbyte neiliType = -1;
			Serializer.Deserialize(dataPool, offset, ref neiliType);
			this.SetNeiliType(neiliType);
			this.Element.ShowAfterRefresh();
		}
	}

	// Token: 0x060035D0 RID: 13776 RVA: 0x001B0E80 File Offset: 0x001AF080
	private void HandleDataMonitor(DataUid uid, int offset, RawDataPool dataPool)
	{
		ushort domainId = uid.DomainId;
		ushort num = domainId;
		if (num != 4)
		{
			if (num == 19)
			{
				if (uid.DataId == 117)
				{
					Serializer.Deserialize(dataPool, offset, ref this._lifeLinkData);
					this.RefreshCooldownTip();
					this._charNeiliProportionOfFiveElementsDict.Clear();
					int[] lifeGateCharIds = this._lifeLinkData.LifeGateCharIds;
					int unlockedCount = (lifeGateCharIds != null) ? lifeGateCharIds.Length : 0;
					int i = 0;
					for (;;)
					{
						int num2 = i;
						int[] lifeGateCharIds2 = this._lifeLinkData.LifeGateCharIds;
						int? num3 = (lifeGateCharIds2 != null) ? new int?(lifeGateCharIds2.Length) : null;
						if (!(num2 < num3.GetValueOrDefault() & num3 != null))
						{
							break;
						}
						this.AssignSlot(true, i, this._lifeLinkData.LifeGateCharIds[i], true);
						i++;
					}
					for (int j = unlockedCount; j < 8; j++)
					{
						this.AssignSlot(true, j, -1, false);
					}
					int[] deathGateCharIds = this._lifeLinkData.DeathGateCharIds;
					unlockedCount = ((deathGateCharIds != null) ? deathGateCharIds.Length : 0);
					int k = 0;
					for (;;)
					{
						int num4 = k;
						int[] deathGateCharIds2 = this._lifeLinkData.DeathGateCharIds;
						int? num3 = (deathGateCharIds2 != null) ? new int?(deathGateCharIds2.Length) : null;
						if (!(num4 < num3.GetValueOrDefault() & num3 != null))
						{
							break;
						}
						this.AssignSlot(false, k, this._lifeLinkData.DeathGateCharIds[k], true);
						k++;
					}
					for (int l = unlockedCount; l < 8; l++)
					{
						this.AssignSlot(false, l, -1, false);
					}
					StoryDomainMethod.Call.GetBaihuaLifeLinkNeiliType(this.Element.GameDataListenerId);
				}
			}
		}
		else if (uid.DataId == 0 && uid.SubId1 == 110U)
		{
			NeiliProportionOfFiveElements fiveElements = default(NeiliProportionOfFiveElements);
			Serializer.Deserialize(dataPool, offset, ref fiveElements);
			int charId = (int)uid.SubId0;
			this._charNeiliProportionOfFiveElementsDict[charId] = fiveElements;
			GameDataBridge.AddDataUnMonitor(this.Element.GameDataListenerId, 4, 0, uid.SubId0, 110U);
			int num5;
			if (!this._lifeLinkData.LifeGateCharIds.Exist((int id) => id >= 0))
			{
				num5 = -1;
			}
			else
			{
				num5 = this._lifeLinkData.LifeGateCharIds.Last((int id) => id >= 0);
			}
			int lastLifeCharId = num5;
			int num6;
			if (!this._lifeLinkData.DeathGateCharIds.Exist((int id) => id >= 0))
			{
				num6 = -1;
			}
			else
			{
				num6 = this._lifeLinkData.DeathGateCharIds.Last((int id) => id >= 0);
			}
			int lastDeathCharId = num6;
			bool flag = charId == lastDeathCharId || (lastDeathCharId < 0 && charId == lastLifeCharId);
			if (flag)
			{
				List<NeiliProportionOfFiveElements> list = this._charNeiliProportionOfFiveElementsDict.Values.ToList<NeiliProportionOfFiveElements>();
				Span<NeiliProportionOfFiveElements> array = list.ToArray();
				Span<int> sum = this._sumNeiliProportionOfFiveElements;
				NeiliProportionOfFiveElements.GetSum(array, sum);
				this._totalNeiliProportionOfFiveElements = NeiliProportionOfFiveElements.GetTotal(array);
				this._lifeLinkNeiliTypeTipArgumentBox.Set<NeiliProportionOfFiveElements>("Total", this._totalNeiliProportionOfFiveElements);
				for (int m = 0; m < this._lifeLinkData.LifeGateCharIds.Length; m++)
				{
					int id3 = this._lifeLinkData.LifeGateCharIds[m];
					bool flag2 = id3 >= 0;
					if (flag2)
					{
						UI_LifeLink.SlotInfo slotInfo = this.GetSlot(true, m);
						this.InitFiveElementTips(slotInfo, id3, true);
					}
				}
				for (int n = 0; n < this._lifeLinkData.DeathGateCharIds.Length; n++)
				{
					int id2 = this._lifeLinkData.DeathGateCharIds[n];
					bool flag3 = id2 >= 0;
					if (flag3)
					{
						UI_LifeLink.SlotInfo slotInfo2 = this.GetSlot(false, n);
						this.InitFiveElementTips(slotInfo2, id2, false);
					}
				}
			}
		}
	}

	// Token: 0x060035D1 RID: 13777 RVA: 0x001B1280 File Offset: 0x001AF480
	private void RefreshCooldownTip()
	{
		Refers refers = base.CGet<Refers>("CooldownTip");
		bool showTip = this._lifeLinkData.Cooldown > 0;
		refers.gameObject.SetActive(showTip);
		bool flag = showTip;
		if (flag)
		{
			string cooldownStr = this._lifeLinkData.Cooldown.ToString().SetColor("pinkyellow");
			refers.CGet<TextMeshProUGUI>("Tips").text = LocalStringManager.GetFormat(LanguageKey.LK_Baihua_LifeLink_Cooldown_Tip, cooldownStr);
			refers.CGet<TMPTextSpriteHelper>("Helper").Parse();
		}
	}

	// Token: 0x060035D2 RID: 13778 RVA: 0x001B1308 File Offset: 0x001AF508
	private void InitFiveElementTips(UI_LifeLink.SlotInfo slotInfo, int charId, bool isLife)
	{
		NeiliProportionOfFiveElements fiveElements = this._charNeiliProportionOfFiveElementsDict[charId];
		sbyte neiliType = fiveElements.GetNeiliType(SingletonObject.getInstance<TimeManager>().GetMonthInCurrYear());
		NeiliTypeItem neiliTypeCfg = NeiliType.Instance[neiliType];
		sbyte fiveElementType = (sbyte)neiliTypeCfg.FiveElements;
		string fiveElementName = CommonUtils.GetFiveElementsNameByType(fiveElementType);
		string fiveElementIcon = CommonUtils.GetFiveElementsIconByType(fiveElementType);
		slotInfo.Refers.CGet<TextMeshProUGUI>("FiveElementTypeLabel").SetText(fiveElementName, true);
		CImage iconImg = slotInfo.Refers.CGet<CImage>("FiveElementTypeIcon");
		iconImg.SetSprite(fiveElementIcon, false, null);
		TooltipInvoker mouseTipDisplayer = iconImg.GetComponent<TooltipInvoker>();
		mouseTipDisplayer.Type = TipType.Simple;
		string[] presetParam = mouseTipDisplayer.PresetParam;
		bool flag = presetParam == null || presetParam.Length != 2;
		if (flag)
		{
			mouseTipDisplayer.PresetParam = new string[2];
		}
		mouseTipDisplayer.PresetParam[0] = LocalStringManager.Get(LanguageKey.LK_NeiliProportionOfFiveElements);
		mouseTipDisplayer.PresetParam[1] = UI_LifeLink.GetFiveElementsTips(fiveElements, this._sumNeiliProportionOfFiveElements);
		if (isLife)
		{
			Transform lineEffectRoot = slotInfo.Refers.CGet<GameObject>("LineEffects").transform;
			for (int i = 0; i < lineEffectRoot.childCount; i++)
			{
				lineEffectRoot.GetChild(i).gameObject.SetActive(i == (int)fiveElementType);
			}
		}
	}

	// Token: 0x060035D3 RID: 13779 RVA: 0x001B144C File Offset: 0x001AF64C
	public unsafe static string GetFiveElementsTips(NeiliProportionOfFiveElements fiveElements, int[] sumElements = null)
	{
		StringBuilder stringBuilder = EasyPool.Get<StringBuilder>();
		int sum = (sumElements != null) ? sumElements.Sum() : 0;
		for (sbyte fiveElementType = 0; fiveElementType < 5; fiveElementType += 1)
		{
			FiveElementItem config = FiveElement.Instance[(int)fiveElementType];
			sbyte value = *fiveElements[(int)fiveElementType];
			stringBuilder.Append(LocalStringManager.Get(LanguageKey.LK_Dot_Symbol));
			stringBuilder.AppendFormat("<SpName={0}>", config.Icon);
			stringBuilder.Append(config.Name);
			stringBuilder.Append(LocalStringManager.Get(LanguageKey.LK_Colon_Symbol));
			stringBuilder.Append(value);
			stringBuilder.Append('%');
			bool flag = value > 0 && sum > 0;
			if (flag)
			{
				int rate = (int)(value * 100) / sum;
				bool flag2 = rate > 0;
				if (flag2)
				{
					string rateStr = LocalStringManager.GetFormat(LanguageKey.LK_Brackets_Fix, string.Format("{0}%", rate)).SetColor("brightblue");
					stringBuilder.Append(rateStr);
				}
			}
			stringBuilder.AppendLine();
		}
		string result = stringBuilder.ToString();
		EasyPool.Free<StringBuilder>(stringBuilder);
		return result;
	}

	// Token: 0x060035D4 RID: 13780 RVA: 0x001B156C File Offset: 0x001AF76C
	private string GetHealthTips(sbyte healthType)
	{
		if (this._stringBuilder == null)
		{
			this._stringBuilder = new StringBuilder();
		}
		this._stringBuilder.Clear();
		this._stringBuilder.Append(LocalStringManager.Get(LanguageKey.LK_Dot_Symbol));
		this._stringBuilder.Append("<SpName=mousetip_jiankang>");
		this._stringBuilder.Append(LocalStringManager.Get(LanguageKey.LK_CharacterHealth));
		this._stringBuilder.Append(LocalStringManager.Get(LanguageKey.LK_Colon_Symbol));
		this._stringBuilder.Append(CommonUtils.GetHealthString(healthType));
		return this._stringBuilder.ToString();
	}

	// Token: 0x060035D5 RID: 13781 RVA: 0x001B1610 File Offset: 0x001AF810
	private void SetSlotCharTip(UI_LifeLink.SlotInfo slotInfo, int charId)
	{
		CharacterDomainMethod.AsyncCall.GetHealthType(this, charId, delegate(int offset, RawDataPool dataPool)
		{
			sbyte healthType = 0;
			Serializer.Deserialize(dataPool, offset, ref healthType);
			TooltipInvoker tipDisplayer = slotInfo.Refers.CGet<GameObject>("BuildingAvatarFrame").GetComponent<TooltipInvoker>();
			bool flag = tipDisplayer == null;
			if (!flag)
			{
				tipDisplayer.Type = TipType.Simple;
				string[] presetParam = tipDisplayer.PresetParam;
				bool flag2 = presetParam == null || presetParam.Length != 2;
				if (flag2)
				{
					tipDisplayer.PresetParam = new string[2];
				}
				tipDisplayer.PresetParam[0] = LocalStringManager.Get(LanguageKey.LK_Health);
				tipDisplayer.PresetParam[1] = this.GetHealthTips(healthType);
			}
		});
	}

	// Token: 0x060035D6 RID: 13782 RVA: 0x001B1648 File Offset: 0x001AF848
	private string GetCalculatedNeiliTypeTip(sbyte neiliType)
	{
		if (this._stringBuilder == null)
		{
			this._stringBuilder = new StringBuilder();
		}
		this._stringBuilder.Clear();
		string colon = LocalStringManager.Get(LanguageKey.LK_Colon_Symbol);
		NeiliTypeItem neiliTypeCfg = NeiliType.Instance[neiliType];
		short[] array = neiliTypeCfg.LifeGateFeatures;
		bool flag = array != null && array.Length > 0;
		if (flag)
		{
			this._stringBuilder.AppendLine(LocalStringManager.Get(LanguageKey.LK_Baihua_LifeLink_LifeGate).SetColor("darkbrown"));
			foreach (short featureId in neiliTypeCfg.LifeGateFeatures)
			{
				CharacterFeatureItem featureCfg = CharacterFeature.Instance[featureId];
				this._stringBuilder.Append(featureCfg.Name);
				this._stringBuilder.Append(colon);
				this._stringBuilder.AppendLine(featureCfg.Desc);
				this._stringBuilder.AppendLine(featureCfg.EffectDesc);
			}
		}
		array = neiliTypeCfg.DeathGateFeatures;
		bool flag2 = array != null && array.Length > 0;
		if (flag2)
		{
			this._stringBuilder.AppendLine(LocalStringManager.Get(LanguageKey.LK_Baihua_LifeLink_DeathGate).SetColor("darkbrown"));
			foreach (short featureId2 in neiliTypeCfg.DeathGateFeatures)
			{
				CharacterFeatureItem featureCfg2 = CharacterFeature.Instance[featureId2];
				this._stringBuilder.Append(featureCfg2.Name);
				this._stringBuilder.Append(colon);
				this._stringBuilder.AppendLine(featureCfg2.Desc);
				this._stringBuilder.AppendLine(featureCfg2.EffectDesc);
			}
		}
		return this._stringBuilder.ToString();
	}

	// Token: 0x060035D7 RID: 13783 RVA: 0x001B180C File Offset: 0x001AFA0C
	private void AssignSlot(bool isLifeGate, int index, int charId, bool isUnlocked)
	{
		UI_LifeLink.SlotInfo slotInfo = this.GetSlot(isLifeGate, index);
		this.AssignSlot(slotInfo, charId, isUnlocked);
	}

	// Token: 0x060035D8 RID: 13784 RVA: 0x001B1830 File Offset: 0x001AFA30
	private void AssignSlot(UI_LifeLink.SlotInfo slotInfo, int charId, bool isUnlocked)
	{
		slotInfo.Avatar.CharacterId = charId;
		slotInfo.Name.CharacterId = charId;
		slotInfo.Refers.CGet<GameObject>("CharacterInfo").SetActive(charId >= 0);
		slotInfo.Refers.CGet<GameObject>("Flower").SetActive(isUnlocked);
		CButtonObsolete selectCharBtn = slotInfo.Refers.CGet<CButtonObsolete>("SelectCharBtn");
		selectCharBtn.interactable = isUnlocked;
		selectCharBtn.GetComponent<PointerTrigger>().enabled = (isUnlocked && charId < 0);
		bool flag = charId >= 0;
		if (flag)
		{
			GameDataBridge.AddDataMonitor(this.Element.GameDataListenerId, 4, 0, (ulong)((long)charId), 110U);
			this.SetSlotCharTip(slotInfo, charId);
		}
	}

	// Token: 0x060035D9 RID: 13785 RVA: 0x001B18E8 File Offset: 0x001AFAE8
	private UI_LifeLink.SlotInfo GetSlot(bool isLifeGate, int index)
	{
		return isLifeGate ? new UI_LifeLink.SlotInfo
		{
			Avatar = this._lifeGateCharacterAvatars[index],
			Name = this._lifeGateCharacterNames[index],
			Refers = this._lifeGateCharacterRefers[index]
		} : new UI_LifeLink.SlotInfo
		{
			Avatar = this._deathGateCharacterAvatars[index],
			Name = this._deathGateCharacterNames[index],
			Refers = this._deathGateCharacterRefers[index]
		};
	}

	// Token: 0x060035DA RID: 13786 RVA: 0x001B196C File Offset: 0x001AFB6C
	private void OnSelectCharacter(bool isLifeGate, int index)
	{
		this._currOperatingIndex = index;
		this._isOperatingLifeGate = isLifeGate;
		this._xiangshuProgress = SingletonObject.getInstance<BasicGameData>().XiangshuProgress;
		CharacterDomainMethod.Call.InitializeCharacterSortFilter(-1, this._sortFilterSettings);
		ArgumentBox argBox = EasyPool.Get<ArgumentBox>().Set("Title", LocalStringManager.Get(LanguageKey.LK_Baihua_LifeLink_Title)).Set("DisableTips", LocalStringManager.Get(LanguageKey.LK_Baihua_LifeLink_Disable_Tips)).Set("SimpleViewStatType", 119).SetObject("SortFilterSettings", this._sortFilterSettings).SetObject("OnConfirmSelect", this._onConfirmSelect).SetObject("DisableCondition", this._disableCondition).SetObject("OnClickChar", this._openCharacterMenu).Set("ShowFiveElementType", true);
		UIElement.UltimateSelectCharacter.SetOnInitArgs(argBox);
		UIManager.Instance.StackToUI(UIElement.UltimateSelectCharacter);
	}

	// Token: 0x060035DB RID: 13787 RVA: 0x001B1A46 File Offset: 0x001AFC46
	private void OnConfirmSelect(int charId)
	{
		StoryDomainMethod.Call.SetLifeLinkCharacter(charId, this._currOperatingIndex, this._isOperatingLifeGate);
		this._currOperatingIndex = -1;
	}

	// Token: 0x060035DC RID: 13788 RVA: 0x001B1A64 File Offset: 0x001AFC64
	private void OpenCharacterMenu(int charId)
	{
		ArgumentBox argBox = EasyPool.Get<ArgumentBox>();
		argBox.Set("CharacterId", charId);
		argBox.SetObject("ViewCharacterMenuTaretPage", new SubPageIndex(ECharacterSubToggleBase.CharacterBase, ECharacterSubPage.Character));
		UIElement.CharacterMenu.SetOnInitArgs(argBox);
		UIManager.Instance.ShowUI(UIElement.CharacterMenu, true);
	}

	// Token: 0x060035DD RID: 13789 RVA: 0x001B1ABC File Offset: 0x001AFCBC
	private bool CharacterDisableCondition(CharacterDisplayDataForUltimateSelect character)
	{
		return character.ConsummateLevel - (int)this._xiangshuProgress >= 4;
	}

	// Token: 0x04002700 RID: 9984
	private SectBaihuaLifeLinkData _lifeLinkData;

	// Token: 0x04002701 RID: 9985
	private CharacterAvatar[] _lifeGateCharacterAvatars;

	// Token: 0x04002702 RID: 9986
	private CharacterAvatar[] _deathGateCharacterAvatars;

	// Token: 0x04002703 RID: 9987
	private CharacterName[] _lifeGateCharacterNames;

	// Token: 0x04002704 RID: 9988
	private CharacterName[] _deathGateCharacterNames;

	// Token: 0x04002705 RID: 9989
	[SerializeField]
	private Refers[] _lifeGateCharacterRefers;

	// Token: 0x04002706 RID: 9990
	[SerializeField]
	private Refers[] _deathGateCharacterRefers;

	// Token: 0x04002707 RID: 9991
	[SerializeField]
	private GameObject[] _neiliEffects;

	// Token: 0x04002708 RID: 9992
	[SerializeField]
	private GameObject[] _backgroundEffects;

	// Token: 0x04002709 RID: 9993
	[SerializeField]
	private GameObject[] _flowerEffects;

	// Token: 0x0400270A RID: 9994
	private sbyte _xiangshuProgress;

	// Token: 0x0400270B RID: 9995
	private int _currOperatingIndex;

	// Token: 0x0400270C RID: 9996
	private bool _isOperatingLifeGate;

	// Token: 0x0400270D RID: 9997
	private Action<int> _onConfirmSelect;

	// Token: 0x0400270E RID: 9998
	private Action<int> _openCharacterMenu;

	// Token: 0x0400270F RID: 9999
	private Predicate<CharacterDisplayDataForUltimateSelect> _disableCondition;

	// Token: 0x04002710 RID: 10000
	private CharacterSortFilterSettings _sortFilterSettings;

	// Token: 0x04002711 RID: 10001
	private bool _neiliTypeInitialized;

	// Token: 0x04002712 RID: 10002
	private StringBuilder _stringBuilder;

	// Token: 0x04002713 RID: 10003
	private readonly Dictionary<int, NeiliProportionOfFiveElements> _charNeiliProportionOfFiveElementsDict = new Dictionary<int, NeiliProportionOfFiveElements>();

	// Token: 0x04002714 RID: 10004
	private ArgumentBox _lifeLinkNeiliTypeTipArgumentBox = new ArgumentBox();

	// Token: 0x04002715 RID: 10005
	private NeiliProportionOfFiveElements _totalNeiliProportionOfFiveElements = default(NeiliProportionOfFiveElements);

	// Token: 0x04002716 RID: 10006
	private readonly int[] _sumNeiliProportionOfFiveElements = new int[5];

	// Token: 0x020017A4 RID: 6052
	private struct SlotInfo
	{
		// Token: 0x0400AC31 RID: 44081
		public Refers Refers;

		// Token: 0x0400AC32 RID: 44082
		public CharacterAvatar Avatar;

		// Token: 0x0400AC33 RID: 44083
		public CharacterName Name;
	}
}
