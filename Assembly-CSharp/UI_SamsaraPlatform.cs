using System;
using System.Collections;
using System.Collections.Generic;
using Coffee.UIExtensions;
using Config;
using DG.Tweening;
using FrameWork;
using Game.Components.Avatar;
using GameData.Common;
using GameData.Domains.Building;
using GameData.Domains.Character;
using GameData.Domains.Character.Display;
using GameData.GameDataBridge;
using GameData.Serializer;
using GameData.Utilities;
using Spine.Unity;
using TMPro;
using UnityEngine;

// Token: 0x020001BC RID: 444
public class UI_SamsaraPlatform : UIBase
{
	// Token: 0x06001B80 RID: 7040 RVA: 0x000BC18C File Offset: 0x000BA38C
	public override void OnInit(ArgumentBox argsBox)
	{
		argsBox.Get<BuildingBlockData>("BuildingData", out this._buildingData);
		argsBox.Get<BuildingBlockKey>("BuildingKey", out this._buildingKey);
		this.UpdateBuildingLevel();
		UIElement element = this.Element;
		element.OnListenerIdReady = (Action)Delegate.Combine(element.OnListenerIdReady, new Action(this.OnListenerIdReady));
		UIElement element2 = this.Element;
		element2.OnShowed = (Action)Delegate.Combine(element2.OnShowed, new Action(delegate()
		{
			AudioManager.Instance.StopSound("ui_industry_reincarnation");
			AudioManager.Instance.PlaySound("ui_industry_reincarnation", false, false);
		}));
	}

	// Token: 0x06001B81 RID: 7041 RVA: 0x000BC228 File Offset: 0x000BA428
	private void Awake()
	{
		this._destinyHolder = base.CGet<RectTransform>("DestinyHolder");
		this._mainAttributeHolder = base.CGet<RectTransform>("MainAttributeHolder");
		this._combatSkillHolder = base.CGet<RectTransform>("CombatSkillHolder");
		this._lifeSkillHolder = base.CGet<RectTransform>("LifeSkillHolder");
		sbyte type = 0;
		while ((int)type < DestinyType.Instance.Count)
		{
			sbyte destinyType = type;
			Refers destinyRefers = this._destinyHolder.GetChild((int)type).GetComponent<Refers>();
			CButtonObsolete changeCharBtn = destinyRefers.CGet<CButtonObsolete>("ChangeChar");
			CButtonObsolete clearCharBtn = destinyRefers.CGet<CButtonObsolete>("ClearChar");
			PointerTrigger changeBtnPtrTrigger = changeCharBtn.GetComponent<PointerTrigger>();
			PointerTrigger clearBtnPtrTrigger = clearCharBtn.GetComponent<PointerTrigger>();
			TooltipInvoker characterAvatarTips = destinyRefers.CGet<TooltipInvoker>("AttributeTipDisplayer");
			characterAvatarTips.Type = TipType.DeadCharacterComplete;
			PointerTrigger mainAttrPtrTrigger = characterAvatarTips.GetComponent<PointerTrigger>();
			destinyRefers.CGet<TextMeshProUGUI>("Title").text = DestinyType.Instance[type].Name;
			destinyRefers.CGet<TextMeshProUGUI>("LockedTitle").text = DestinyType.Instance[type].Name;
			destinyRefers.CGet<CImage>("CharInfoBubbleBg").SetSprite(string.Format("{0}{1}", "ui_buildingpopup_samsara_base_bubble_", type), false, null);
			Refers addCharRefers = destinyRefers.CGet<Refers>("AddChar");
			addCharRefers.CGet<CButtonObsolete>("Button").ClearAndAddListener(delegate
			{
				this.ShowSelectCharWindow(destinyType);
			});
			addCharRefers.CGet<CImage>("Normal").SetSprite(string.Format("ui_buildingpopup_samsara_btn_{0}_0", type), false, null);
			addCharRefers.CGet<CImage>("Hover").SetSprite(string.Format("ui_buildingpopup_samsara_btn_{0}_1", type), false, null);
			changeCharBtn.ClearAndAddListener(delegate
			{
				this.SetCharRefersHover(destinyRefers, false);
				this.ShowSelectCharWindow(destinyType);
			});
			clearCharBtn.ClearAndAddListener(delegate
			{
				this.SetCharRefersHover(destinyRefers, false);
				BuildingDomainMethod.Call.SetSamsaraPlatformChar(destinyType, -1);
			});
			CButtonObsolete rebornButton = destinyRefers.CGet<CButtonObsolete>("Reborn");
			rebornButton.ClearAndAddListener(delegate
			{
				rebornButton.interactable = false;
				this.OnClickReborn(destinyType);
			});
			mainAttrPtrTrigger.EnterEvent.AddListener(delegate()
			{
				this.SetCharRefersHover(destinyRefers, true);
				this.ShowAllAttributePreview(destinyType);
			});
			mainAttrPtrTrigger.ExitEvent.AddListener(delegate()
			{
				this.HideAllAttributePreview();
				bool flag = !RectTransformUtility.RectangleContainsScreenPoint(mainAttrPtrTrigger.GetComponent<RectTransform>(), Input.mousePosition, UIManager.Instance.UiCamera);
				if (flag)
				{
					this.SetCharRefersHover(destinyRefers, false);
				}
			});
			changeBtnPtrTrigger.ExitEvent.AddListener(delegate()
			{
				bool flag = !RectTransformUtility.RectangleContainsScreenPoint(mainAttrPtrTrigger.GetComponent<RectTransform>(), Input.mousePosition, UIManager.Instance.UiCamera);
				if (flag)
				{
					this.SetCharRefersHover(destinyRefers, false);
				}
			});
			clearBtnPtrTrigger.ExitEvent.AddListener(delegate()
			{
				bool flag = !RectTransformUtility.RectangleContainsScreenPoint(mainAttrPtrTrigger.GetComponent<RectTransform>(), Input.mousePosition, UIManager.Instance.UiCamera);
				if (flag)
				{
					this.SetCharRefersHover(destinyRefers, false);
				}
			});
			type += 1;
		}
		for (sbyte type2 = 0; type2 < 6; type2 += 1)
		{
			Refers tempRefers = this._mainAttributeHolder.GetChild((int)type2).GetComponent<Refers>();
			tempRefers.CGet<CImage>("Icon").SetSprite(string.Format("sp_icon_attribute_{0}", type2), false, null);
			tempRefers.CGet<TextMeshProUGUI>("AdditionValue").gameObject.SetActive(false);
			switch (type2)
			{
			case 0:
				tempRefers.CGet<TextMeshProUGUI>("Title").text = LocalStringManager.Get(LanguageKey.LK_Main_Attribute_Strength);
				break;
			case 1:
				tempRefers.CGet<TextMeshProUGUI>("Title").text = LocalStringManager.Get(LanguageKey.LK_Main_Attribute_Dexterity);
				break;
			case 2:
				tempRefers.CGet<TextMeshProUGUI>("Title").text = LocalStringManager.Get(LanguageKey.LK_Main_Attribute_Concentration);
				break;
			case 3:
				tempRefers.CGet<TextMeshProUGUI>("Title").text = LocalStringManager.Get(LanguageKey.LK_Main_Attribute_Vitality);
				break;
			case 4:
				tempRefers.CGet<TextMeshProUGUI>("Title").text = LocalStringManager.Get(LanguageKey.LK_Main_Attribute_Energy);
				break;
			case 5:
				tempRefers.CGet<TextMeshProUGUI>("Title").text = LocalStringManager.Get(LanguageKey.LK_Main_Attribute_Intelligence);
				break;
			}
		}
		for (sbyte type3 = 0; type3 < 14; type3 += 1)
		{
			Refers tempRefers2 = this._combatSkillHolder.GetChild((int)type3).GetComponent<Refers>();
			tempRefers2.CGet<TextMeshProUGUI>("Title").text = CombatSkillType.Instance[type3].Name;
			tempRefers2.CGet<CImage>("Icon").SetSprite(string.Format("sp_18_iconwuxuezhanshi_{0}", type3), false, null);
			tempRefers2.CGet<TextMeshProUGUI>("AdditionValue").gameObject.SetActive(false);
		}
		for (sbyte type4 = 0; type4 < 16; type4 += 1)
		{
			Refers tempRefers3 = this._lifeSkillHolder.GetChild((int)type4).GetComponent<Refers>();
			tempRefers3.CGet<TextMeshProUGUI>("Title").text = Config.LifeSkillType.Instance[type4].Name;
			tempRefers3.CGet<CImage>("Icon").SetSprite(string.Format("sp_14_iconjiyizhanshi_{0}", type4), false, null);
			tempRefers3.CGet<TextMeshProUGUI>("AdditionValue").gameObject.SetActive(false);
		}
		SkeletonGraphic cloudAni = base.CGet<SkeletonGraphic>("CloudAni");
		cloudAni.AnimationState.SetAnimation(0, cloudAni.Skeleton.Data.Animations.Items[0].Name, true);
	}

	// Token: 0x06001B82 RID: 7042 RVA: 0x000BC788 File Offset: 0x000BA988
	private void SetCharRefersHover(Refers destinyRefers, bool isHover)
	{
		GameObject charUpdateGroup = destinyRefers.CGet<GameObject>("CharUpdateGroup");
		GameObject hoverBg = destinyRefers.CGet<GameObject>("HoverBg");
		charUpdateGroup.gameObject.SetActive(isHover);
		hoverBg.gameObject.SetActive(isHover);
	}

	// Token: 0x06001B83 RID: 7043 RVA: 0x000BC7C8 File Offset: 0x000BA9C8
	public override void InitMonitorFieldIds()
	{
		sbyte type = 0;
		while ((int)type < DestinyType.Instance.Count)
		{
			this.MonitorFields.Add(new UIBase.MonitorDataField(9, 15, (ulong)((long)type), null));
			type += 1;
		}
		this.MonitorFields.Add(new UIBase.MonitorDataField(9, 12, ulong.MaxValue, null));
		this.MonitorFields.Add(new UIBase.MonitorDataField(9, 13, ulong.MaxValue, null));
		this.MonitorFields.Add(new UIBase.MonitorDataField(9, 14, ulong.MaxValue, null));
		this.MonitorFields.Add(new UIBase.MonitorDataField(4, 0, (ulong)((long)SingletonObject.getInstance<BasicGameData>().TaiwuCharId), new uint[]
		{
			34U
		}));
	}

	// Token: 0x06001B84 RID: 7044 RVA: 0x000BC87C File Offset: 0x000BAA7C
	public void OnListenerIdReady()
	{
		BuildingDomainMethod.Call.GetSamsaraPlatformCharList(this.Element.GameDataListenerId);
		sbyte type = 0;
		while ((int)type < DestinyType.Instance.Count)
		{
			this._samsaraPlatformSlots[(int)type] = new IntPair(-1, 0);
			type += 1;
		}
	}

	// Token: 0x06001B85 RID: 7045 RVA: 0x000BC8CC File Offset: 0x000BAACC
	public override void OnNotifyGameData(List<NotificationWrapper> notifications)
	{
		foreach (NotificationWrapper wrapper in notifications)
		{
			Notification notification = wrapper.Notification;
			byte type2 = notification.Type;
			byte b = type2;
			if (b != 0)
			{
				if (b == 1)
				{
					bool flag = notification.DomainId == 9;
					if (flag)
					{
						bool flag2 = notification.MethodId == 64;
						if (flag2)
						{
							List<SamsaraPlatformCharDisplayData> samsaraCharList = new List<SamsaraPlatformCharDisplayData>();
							Serializer.Deserialize(wrapper.DataPool, notification.ValueOffset, ref samsaraCharList);
							this._charDataDict.Clear();
							foreach (SamsaraPlatformCharDisplayData data in samsaraCharList)
							{
								this._charDataDict[data.Id] = data;
							}
							this._charList.Clear();
							this._charList.AddRange(this._charDataDict.Keys);
							sbyte type = 0;
							while ((int)type < DestinyType.Instance.Count)
							{
								this.UpdateDestinySlot(type);
								type += 1;
							}
							this.Element.ShowAfterRefresh();
						}
						else
						{
							bool flag3 = notification.MethodId == 66;
							if (flag3)
							{
								CharacterDisplayData motherData = new CharacterDisplayData();
								Serializer.Deserialize(wrapper.DataPool, notification.ValueOffset, ref motherData);
								string bornCharName = NameCenter.GetCharMonasticTitleAndNameByNameRelatedData(ref this._charDataDict[this._bornedCharId].Data.NameData, false, false).SetColor("darkbrown");
								bool flag4 = motherData != null;
								if (flag4)
								{
									WorldMapModel mapModel = SingletonObject.getInstance<WorldMapModel>();
									MapAreaItem areaConfig = mapModel.Areas[(int)motherData.Location.AreaId].GetConfig();
									string areaName = areaConfig.Name.SetColor("pinkyellow");
									string stateName = MapState.Instance[areaConfig.StateID].Name.SetColor("pinkyellow");
									string orgInfo = CommonUtils.GetOrganizationGradeString(motherData.OrgInfo, motherData.Gender, motherData.PhysiologicalAge, -1);
									string motherName = NameCenter.GetCharMonasticTitleOrNameByDisplayData(motherData, false, false).SetColor("darkbrown");
									DialogCmd cmd = new DialogCmd
									{
										Title = LocalStringManager.Get(LanguageKey.LK_Samsara_Platform_Success_Title),
										Content = LocalStringManager.GetFormat(LanguageKey.LK_Samsara_Platform_Success_Tips, new object[]
										{
											bornCharName,
											DestinyType.Instance[this._bornedDestiny].Name,
											stateName,
											areaName,
											orgInfo,
											motherName
										}),
										Type = 2
									};
									UIElement.Dialog.SetOnInitArgs(EasyPool.Get<ArgumentBox>().SetObject("Cmd", cmd));
									UIManager.Instance.MaskUI(UIElement.Dialog);
									this._charDataDict.Remove(this._bornedCharId);
									this._charList.Remove(this._bornedCharId);
								}
								else
								{
									bool flag5 = this._charDataDict[this._bornedCharId].NameRelatedData.CharTemplateId == 779;
									if (flag5)
									{
										this.QuickHide();
										break;
									}
									DialogCmd cmd2 = new DialogCmd
									{
										Title = LocalStringManager.Get(LanguageKey.LK_Samsara_Platform_Fail_Title),
										Content = LocalStringManager.GetFormat(LanguageKey.LK_Samsara_Platform_Fail_Tips, bornCharName),
										Type = 2
									};
									UIElement.Dialog.SetOnInitArgs(EasyPool.Get<ArgumentBox>().SetObject("Cmd", cmd2));
									UIManager.Instance.MaskUI(UIElement.Dialog);
								}
							}
						}
					}
				}
			}
			else
			{
				DataUid uid = notification.Uid;
				bool flag6 = uid.DomainId == 9;
				if (flag6)
				{
					bool flag7 = uid.DataId == 15;
					if (flag7)
					{
						IntPair slotInfo = default(IntPair);
						Serializer.Deserialize(wrapper.DataPool, notification.ValueOffset, ref slotInfo);
						this._samsaraPlatformSlots[(int)(checked((IntPtr)uid.SubId0))] = slotInfo;
						bool flag8 = slotInfo.First < 0 || this._charDataDict.ContainsKey(slotInfo.First);
						if (flag8)
						{
							this.UpdateDestinySlot((sbyte)uid.SubId0);
						}
					}
					else
					{
						bool flag9 = uid.DataId == 12;
						if (flag9)
						{
							Serializer.Deserialize(wrapper.DataPool, notification.ValueOffset, ref this._addMainAttributes);
							for (int i = 0; i < 6; i++)
							{
								this._mainAttributeHolder.GetChild(i).GetComponent<Refers>().CGet<TextMeshProUGUI>("Value").text = (ref this._addMainAttributes.Items.FixedElementField + (IntPtr)i * 2).ToString();
							}
						}
						else
						{
							bool flag10 = uid.DataId == 13;
							if (flag10)
							{
								Serializer.Deserialize(wrapper.DataPool, notification.ValueOffset, ref this._addCombatSkillQualifications);
								for (int j = 0; j < 14; j++)
								{
									this._combatSkillHolder.GetChild(j).GetComponent<Refers>().CGet<TextMeshProUGUI>("Value").text = (ref this._addCombatSkillQualifications.Items.FixedElementField + (IntPtr)j * 2).ToString();
								}
							}
							else
							{
								bool flag11 = uid.DataId == 14;
								if (flag11)
								{
									Serializer.Deserialize(wrapper.DataPool, notification.ValueOffset, ref this._addLifeSkillQualifications);
									for (int k = 0; k < 16; k++)
									{
										this._lifeSkillHolder.GetChild(k).GetComponent<Refers>().CGet<TextMeshProUGUI>("Value").text = (ref this._addLifeSkillQualifications.Items.FixedElementField + (IntPtr)k * 2).ToString();
									}
								}
							}
						}
					}
				}
				else
				{
					bool flag12 = uid.DomainId == 4 && uid.DataId == 0;
					if (flag12)
					{
						bool flag13 = uid.SubId1 == 34U;
						if (flag13)
						{
							Serializer.Deserialize(wrapper.DataPool, notification.ValueOffset, ref this._resources);
							this.RefreshUnlock();
						}
					}
				}
			}
		}
	}

	// Token: 0x06001B86 RID: 7046 RVA: 0x000BCF10 File Offset: 0x000BB110
	protected override void OnClick(Transform btn)
	{
		string btnName = btn.name;
		bool flag = btnName == "Close";
		if (flag)
		{
			this.QuickHide();
		}
		bool flag2 = btnName == "ButtonRecord";
		if (flag2)
		{
			UIManager.Instance.ShowUI(UIElement.SamsaraPlatformRecords, true);
		}
	}

	// Token: 0x06001B87 RID: 7047 RVA: 0x000BCF60 File Offset: 0x000BB160
	public override void QuickHide()
	{
		AudioManager.Instance.StopSound("ui_industry_reincarnation");
		base.QuickHide();
		for (int i = 0; i < this._destinyHolder.childCount; i++)
		{
			this._destinyHolder.GetChild(i).GetComponent<Refers>().CGet<GameObject>("CharUpdateGroup").gameObject.SetActive(false);
		}
	}

	// Token: 0x06001B88 RID: 7048 RVA: 0x000BCFC8 File Offset: 0x000BB1C8
	private void ShowSelectCharWindow(sbyte destinyType)
	{
		this._selectingCharDestiny = destinyType;
		ArgumentBox argBox = EasyPool.Get<ArgumentBox>();
		List<int> canSelectCharList = new List<int>();
		canSelectCharList.Clear();
		canSelectCharList.AddRange(this._charList);
		sbyte type = 0;
		while ((int)type < DestinyType.Instance.Count)
		{
			bool flag = type == destinyType;
			if (!flag)
			{
				canSelectCharList.Remove(this._samsaraPlatformSlots[(int)type].First);
			}
			type += 1;
		}
		argBox.Clear();
		argBox.SetObject("charIdList", canSelectCharList);
		argBox.SetObject("callback", new Action<int>(this.OnSelectChar));
		bool flag2 = this._samsaraPlatformSlots[(int)destinyType].First >= 0;
		if (flag2)
		{
			argBox.SetObject("selectedCharIdList", new List<int>
			{
				this._samsaraPlatformSlots[(int)destinyType].First
			});
		}
		UIElement.SelectCharLegacy.SetOnInitArgs(argBox);
		UIManager.Instance.ShowUI(UIElement.SelectCharLegacy, true);
	}

	// Token: 0x06001B89 RID: 7049 RVA: 0x000BD0CC File Offset: 0x000BB2CC
	private void OnSelectChar(int charId)
	{
		bool flag = charId == this._samsaraPlatformSlots[(int)this._selectingCharDestiny].First;
		if (!flag)
		{
			BuildingDomainMethod.Call.SetSamsaraPlatformChar(this._selectingCharDestiny, charId);
		}
	}

	// Token: 0x06001B8A RID: 7050 RVA: 0x000BD106 File Offset: 0x000BB306
	private void ShowAllAttributePreview(sbyte destinyType)
	{
		this.ShowMainAttributePreview(destinyType);
		this.ShowCombatSkillQualificationPreview(destinyType);
		this.ShowLifeSkillQualificationPreview(destinyType);
	}

	// Token: 0x06001B8B RID: 7051 RVA: 0x000BD121 File Offset: 0x000BB321
	private void HideAllAttributePreview()
	{
		this.HideMainAttributePreview();
		this.HideCombatSkillQualificationPreview();
		this.HideLifeSkillQualificationPreview();
	}

	// Token: 0x06001B8C RID: 7052 RVA: 0x000BD13C File Offset: 0x000BB33C
	private unsafe void ShowMainAttributePreview(sbyte destinyType)
	{
		MainAttributes mainAttributes = this._charDataDict[this._samsaraPlatformSlots[(int)destinyType].First].MainAttributes;
		int addPercent = (int)(GlobalConfig.Instance.SamsaraPlatformAddBasePercent + GlobalConfig.Instance.SamsaraPlatformAddPercentPerLevel * this._buildingLevel);
		for (sbyte type = 0; type < 6; type += 1)
		{
			Refers attributeRefers = this._mainAttributeHolder.GetChild((int)type).GetComponent<Refers>();
			int addValue = Mathf.Max((int)(*(ref mainAttributes.Items.FixedElementField + (IntPtr)type * 2)) * addPercent / 100 - (int)(*(ref this._addMainAttributes.Items.FixedElementField + (IntPtr)type * 2)), 0);
			attributeRefers.CGet<TextMeshProUGUI>("AdditionValue").text = string.Format("+{0}", addValue);
			attributeRefers.CGet<TextMeshProUGUI>("AdditionValue").gameObject.SetActive(true);
		}
	}

	// Token: 0x06001B8D RID: 7053 RVA: 0x000BD228 File Offset: 0x000BB428
	private void HideMainAttributePreview()
	{
		for (sbyte type = 0; type < 6; type += 1)
		{
			this._mainAttributeHolder.GetChild((int)type).GetComponent<Refers>().CGet<TextMeshProUGUI>("AdditionValue").gameObject.SetActive(false);
		}
	}

	// Token: 0x06001B8E RID: 7054 RVA: 0x000BD270 File Offset: 0x000BB470
	private unsafe void ShowCombatSkillQualificationPreview(sbyte destinyType)
	{
		CombatSkillShorts combatSkillQualifications = this._charDataDict[this._samsaraPlatformSlots[(int)destinyType].First].CombatSkillQualifications;
		int addPercent = (int)(GlobalConfig.Instance.SamsaraPlatformAddBasePercent + GlobalConfig.Instance.SamsaraPlatformAddPercentPerLevel * this._buildingLevel);
		for (sbyte type = 0; type < 14; type += 1)
		{
			Refers skillTypeRefers = this._combatSkillHolder.GetChild((int)type).GetComponent<Refers>();
			int addValue = Mathf.Max((int)(*(ref combatSkillQualifications.Items.FixedElementField + (IntPtr)type * 2)) * addPercent / 100 - (int)(*(ref this._addCombatSkillQualifications.Items.FixedElementField + (IntPtr)type * 2)), 0);
			skillTypeRefers.CGet<TextMeshProUGUI>("AdditionValue").text = string.Format("+{0}", addValue);
			skillTypeRefers.CGet<TextMeshProUGUI>("AdditionValue").gameObject.SetActive(true);
		}
	}

	// Token: 0x06001B8F RID: 7055 RVA: 0x000BD35C File Offset: 0x000BB55C
	private void HideCombatSkillQualificationPreview()
	{
		for (sbyte type = 0; type < 14; type += 1)
		{
			this._combatSkillHolder.GetChild((int)type).GetComponent<Refers>().CGet<TextMeshProUGUI>("AdditionValue").gameObject.SetActive(false);
		}
	}

	// Token: 0x06001B90 RID: 7056 RVA: 0x000BD3A4 File Offset: 0x000BB5A4
	private unsafe void ShowLifeSkillQualificationPreview(sbyte destinyType)
	{
		LifeSkillShorts lifeSkillQualifications = this._charDataDict[this._samsaraPlatformSlots[(int)destinyType].First].LifeSkillQualifications;
		int addPercent = (int)(GlobalConfig.Instance.SamsaraPlatformAddBasePercent + GlobalConfig.Instance.SamsaraPlatformAddPercentPerLevel * this._buildingLevel);
		for (sbyte type = 0; type < 16; type += 1)
		{
			Refers skillTypeRefers = this._lifeSkillHolder.GetChild((int)type).GetComponent<Refers>();
			int addValue = Mathf.Max((int)(*(ref lifeSkillQualifications.Items.FixedElementField + (IntPtr)type * 2)) * addPercent / 100 - (int)(*(ref this._addLifeSkillQualifications.Items.FixedElementField + (IntPtr)type * 2)), 0);
			skillTypeRefers.CGet<TextMeshProUGUI>("AdditionValue").text = string.Format("+{0}", addValue);
			skillTypeRefers.CGet<TextMeshProUGUI>("AdditionValue").gameObject.SetActive(true);
		}
	}

	// Token: 0x06001B91 RID: 7057 RVA: 0x000BD490 File Offset: 0x000BB690
	private void HideLifeSkillQualificationPreview()
	{
		for (sbyte type = 0; type < 16; type += 1)
		{
			this._lifeSkillHolder.GetChild((int)type).GetComponent<Refers>().CGet<TextMeshProUGUI>("AdditionValue").gameObject.SetActive(false);
		}
	}

	// Token: 0x06001B92 RID: 7058 RVA: 0x000BD4D8 File Offset: 0x000BB6D8
	private void OnClickReborn(sbyte destinyType)
	{
		this._bornedDestiny = destinyType;
		this._bornedCharId = this._samsaraPlatformSlots[(int)destinyType].First;
		BuildingDomainMethod.Call.SamsaraPlatformReborn(this.Element.GameDataListenerId, destinyType);
		ParticleSystem particle = this._destinyHolder.GetChild((int)destinyType).GetComponent<Refers>().CGet<ParticleSystem>("RebornParticle");
		particle.gameObject.SetActive(true);
		particle.Play(true);
		DOVirtual.DelayedCall(particle.main.duration, delegate
		{
			particle.gameObject.SetActive(false);
		}, true);
		GEvent.OnEvent(UiEvents.SamsaraPlatformRecordDataChange, null);
	}

	// Token: 0x06001B93 RID: 7059 RVA: 0x000BD590 File Offset: 0x000BB790
	private void UpdateDestinySlot(sbyte destinyType)
	{
		int charId = this._samsaraPlatformSlots[(int)destinyType].First;
		Refers destinyRefers = this._destinyHolder.GetChild((int)destinyType).GetComponent<Refers>();
		destinyRefers.CGet<GameObject>("CharInfo").SetActive(charId >= 0);
		destinyRefers.CGet<Refers>("AddChar").gameObject.SetActive(charId < 0);
		bool flag = charId >= 0;
		if (flag)
		{
			SamsaraPlatformCharDisplayData displayData = this._charDataDict[charId];
			int progress = this._samsaraPlatformSlots[(int)destinyType].Second;
			bool canReborn = progress >= (int)GlobalConfig.Instance.SamsaraPlatformMaxProgress;
			string charName = NameCenter.GetCharMonasticTitleAndNameByNameRelatedData(ref displayData.Data.NameData, false, false);
			TooltipInvoker mainAttributeTips = destinyRefers.CGet<TooltipInvoker>("AttributeTipDisplayer");
			Game.Components.Avatar.Avatar avatar = destinyRefers.CGet<Game.Components.Avatar.Avatar>("Avatar");
			avatar.Refresh(displayData.AvatarRelatedData, displayData.TemplateId);
			destinyRefers.CGet<TextMeshProUGUI>("Name").text = charName;
			destinyRefers.CGet<CImage>("ProgressBar").fillAmount = (float)progress / (float)GlobalConfig.Instance.SamsaraPlatformMaxProgress;
			destinyRefers.CGet<TextMeshProUGUI>("Progress").text = string.Format("{0}/{1}", progress, GlobalConfig.Instance.SamsaraPlatformMaxProgress);
			bool flag2 = displayData.NameRelatedData.CharTemplateId == 779;
			if (flag2)
			{
				destinyRefers.CGet<TextMeshProUGUI>("Progress").text = string.Format("{0}/1", progress);
				canReborn = (progress >= 1);
				destinyRefers.CGet<CImage>("ProgressBar").fillAmount = (float)progress / 1f;
			}
			destinyRefers.CGet<CButtonObsolete>("Reborn").interactable = canReborn;
			bool flag3 = mainAttributeTips.RuntimeParam == null;
			if (flag3)
			{
				mainAttributeTips.RuntimeParam = new ArgumentBox();
			}
			mainAttributeTips.RuntimeParam.Set("CharId", charId);
		}
		TooltipInvoker tip = destinyRefers.CGet<TooltipInvoker>("TipArea");
		tip.Type = TipType.Destiny;
		tip.RuntimeParam = EasyPool.Get<ArgumentBox>().Set("DestinyType", destinyType).Set("ScreenWidth", this.MouseTipScreenWidth);
	}

	// Token: 0x06001B94 RID: 7060 RVA: 0x000BD7C0 File Offset: 0x000BB9C0
	private void BuildingBlockDataChange(ArgumentBox argbox)
	{
		this._buildingData = SingletonObject.getInstance<BuildingModel>().GetTaiwuBuildingData(this._buildingKey);
		this.UpdateBuildingLevel();
		this.RefreshUnlock();
	}

	// Token: 0x06001B95 RID: 7061 RVA: 0x000BD7E7 File Offset: 0x000BB9E7
	private void OnEnable()
	{
		GEvent.Add(UiEvents.BuildingBlockDataChange, new GEvent.Callback(this.BuildingBlockDataChange));
	}

	// Token: 0x06001B96 RID: 7062 RVA: 0x000BD803 File Offset: 0x000BBA03
	private void OnDisable()
	{
		GEvent.Remove(UiEvents.BuildingBlockDataChange, new GEvent.Callback(this.BuildingBlockDataChange));
	}

	// Token: 0x06001B97 RID: 7063 RVA: 0x000BD81F File Offset: 0x000BBA1F
	private void UpdateBuildingLevel()
	{
		this._buildingLevel = this._buildingData.CalcUnlockedLevelCount();
	}

	// Token: 0x06001B98 RID: 7064 RVA: 0x000BD834 File Offset: 0x000BBA34
	private unsafe void RefreshUnlock()
	{
		sbyte type = 0;
		while ((int)type < DestinyType.Instance.Count)
		{
			bool flag = this._samsaraPlatformSlots[(int)type].First >= 0;
			if (!flag)
			{
				bool unlocked = this._buildingData.SlotIsUnlocked((int)type);
				Refers destinyRefers = this._destinyHolder.GetChild((int)type).GetComponent<Refers>();
				DestinyTypeItem destinyTypeItem = DestinyType.Instance[type];
				CImage back = destinyRefers.CGet<CImage>("Back");
				back.gameObject.SetActive(unlocked);
				destinyRefers.CGet<GameObject>("LockElement").SetActive(!unlocked);
				destinyRefers.CGet<Refers>("AddChar").gameObject.SetActive(unlocked);
				Refers unlockRefer = destinyRefers.CGet<Refers>("UnlockHolder");
				unlockRefer.gameObject.SetActive(!unlocked);
				bool flag2 = !unlocked;
				if (flag2)
				{
					short unlockResourceType = 0;
					int unlockCost = 0;
					short i = 0;
					while ((int)i < destinyTypeItem.UnlockCost.Length)
					{
						bool flag3 = destinyTypeItem.UnlockCost[(int)i] > 0;
						if (flag3)
						{
							unlockResourceType = i;
							unlockCost = (int)destinyTypeItem.UnlockCost[(int)i];
							break;
						}
						i += 1;
					}
					ResourceTypeItem resourceTypeItem = Config.ResourceType.Instance[(int)unlockResourceType];
					unlockRefer.CGet<CImage>("ResourceIcon").SetSprite(resourceTypeItem.Icon, false, null);
					unlockRefer.CGet<TextMeshProUGUI>("ResourceName").SetText(resourceTypeItem.Name, true);
					TextMeshProUGUI resourceCount = unlockRefer.CGet<TextMeshProUGUI>("ResourceCount");
					bool isEnough = *(ref this._resources.Items.FixedElementField + (IntPtr)unlockResourceType * 4) >= unlockCost;
					resourceCount.SetText(CommonUtils.GetDisplayStringForNum(*(ref this._resources.Items.FixedElementField + (IntPtr)unlockResourceType * 4), 100000).SetColor(isEnough ? "brightblue" : "brightred") + "/" + unlockCost.ToString().SetColor("pinkyellow"), true);
					CButtonObsolete unlockBtn = unlockRefer.CGet<CButtonObsolete>("UnlockBtn");
					unlockBtn.interactable = isEnough;
					int index = (int)type;
					unlockBtn.ClearAndAddListener(delegate
					{
						this.StartCoroutine(this.PlayUnlockEffect(destinyRefers, index));
					});
				}
			}
			type += 1;
		}
	}

	// Token: 0x06001B99 RID: 7065 RVA: 0x000BDAA6 File Offset: 0x000BBCA6
	private IEnumerator PlayUnlockEffect(Refers refers, int index)
	{
		UIParticle unlockParticleBack = refers.CGet<UIParticle>("UnlockParticleBack");
		UIParticle unlockParticleLock = refers.CGet<UIParticle>("UnlockParticleLock");
		GameObject lockObj = refers.CGet<GameObject>("LockElement");
		CImage back = refers.CGet<CImage>("Back");
		Refers unlockRefer = refers.CGet<Refers>("UnlockHolder");
		CommandKitBase.SetDisable(true);
		lockObj.SetActive(false);
		unlockRefer.gameObject.SetActive(false);
		unlockParticleLock.gameObject.SetActive(true);
		unlockParticleLock.Play();
		AudioManager.Instance.PlaySound("reincarnation_unlock", false, false);
		yield return new WaitForSeconds(0.85f);
		back.gameObject.SetActive(true);
		unlockParticleBack.gameObject.SetActive(true);
		unlockParticleBack.Play();
		yield return new WaitForSeconds(0.7f);
		BuildingDomainMethod.Call.UnlockBuildingLevelSlot(this.Element.GameDataListenerId, this._buildingKey, index);
		CommandKitBase.SetDisable(false);
		unlockParticleLock.gameObject.SetActive(false);
		unlockParticleBack.gameObject.SetActive(false);
		yield break;
	}

	// Token: 0x04001579 RID: 5497
	private const string AddCharIconNormalPrefix = "ui_buildingpopup_samsara_btn_{0}_0";

	// Token: 0x0400157A RID: 5498
	private const string AddCharIconHoverPrefix = "ui_buildingpopup_samsara_btn_{0}_1";

	// Token: 0x0400157B RID: 5499
	private const string CharInfoBubbleInfoBgPrefix = "ui_buildingpopup_samsara_base_bubble_";

	// Token: 0x0400157C RID: 5500
	private sbyte _buildingLevel;

	// Token: 0x0400157D RID: 5501
	private BuildingBlockData _buildingData;

	// Token: 0x0400157E RID: 5502
	private BuildingBlockKey _buildingKey;

	// Token: 0x0400157F RID: 5503
	private ResourceInts _resources;

	// Token: 0x04001580 RID: 5504
	private MainAttributes _addMainAttributes;

	// Token: 0x04001581 RID: 5505
	private CombatSkillShorts _addCombatSkillQualifications;

	// Token: 0x04001582 RID: 5506
	private LifeSkillShorts _addLifeSkillQualifications;

	// Token: 0x04001583 RID: 5507
	private readonly IntPair[] _samsaraPlatformSlots = new IntPair[6];

	// Token: 0x04001584 RID: 5508
	private readonly List<int> _charList = new List<int>();

	// Token: 0x04001585 RID: 5509
	private readonly Dictionary<int, SamsaraPlatformCharDisplayData> _charDataDict = new Dictionary<int, SamsaraPlatformCharDisplayData>();

	// Token: 0x04001586 RID: 5510
	private sbyte _selectingCharDestiny;

	// Token: 0x04001587 RID: 5511
	private sbyte _bornedDestiny;

	// Token: 0x04001588 RID: 5512
	private int _bornedCharId;

	// Token: 0x04001589 RID: 5513
	private RectTransform _destinyHolder;

	// Token: 0x0400158A RID: 5514
	private RectTransform _mainAttributeHolder;

	// Token: 0x0400158B RID: 5515
	private RectTransform _combatSkillHolder;

	// Token: 0x0400158C RID: 5516
	private RectTransform _lifeSkillHolder;

	// Token: 0x0400158D RID: 5517
	private readonly List<LanguageKey> BasicAttributeLanguageKey = new List<LanguageKey>
	{
		LanguageKey.LK_Main_Attribute_Strength,
		LanguageKey.LK_Main_Attribute_Dexterity,
		LanguageKey.LK_Main_Attribute_Concentration,
		LanguageKey.LK_Main_Attribute_Vitality,
		LanguageKey.LK_Main_Attribute_Energy,
		LanguageKey.LK_Main_Attribute_Intelligence
	};

	// Token: 0x0400158E RID: 5518
	[HideInInspector]
	public float MouseTipScreenWidth = 1860f;
}
