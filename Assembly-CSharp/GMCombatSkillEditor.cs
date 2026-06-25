using System;
using System.Collections.Generic;
using Config;
using GameData.Common;
using GameData.Domains.Character;
using GameData.Domains.CombatSkill;
using GameData.Domains.Extra;
using GameData.Domains.Taiwu;
using GameData.GameDataBridge;
using GameData.Serializer;
using TMPro;
using UnityEngine;

// Token: 0x0200020F RID: 527
public class GMCombatSkillEditor : Refers
{
	// Token: 0x1700035C RID: 860
	// (get) Token: 0x0600217C RID: 8572 RVA: 0x000F40C4 File Offset: 0x000F22C4
	private List<string> NewEquipTypes
	{
		get
		{
			return new List<string>
			{
				"内功",
				"摧破",
				"轻灵",
				"护体",
				"奇窍",
				"全部"
			};
		}
	}

	// Token: 0x1700035D RID: 861
	// (get) Token: 0x0600217D RID: 8573 RVA: 0x000F4120 File Offset: 0x000F2320
	private List<string> NewGangTypes
	{
		get
		{
			return new List<string>
			{
				"无",
				"少林",
				"峨眉",
				"百花",
				"武当",
				"元山",
				"狮相",
				"然山",
				"璇女",
				"铸剑",
				"空桑",
				"金刚",
				"五仙",
				"界青",
				"伏龙",
				"血犼",
				"全部"
			};
		}
	}

	// Token: 0x0600217E RID: 8574 RVA: 0x000F4200 File Offset: 0x000F2400
	public void Init()
	{
		CButtonObsolete[] btnList = base.GetComponentsInChildren<CButtonObsolete>(true);
		for (int i = 0; i < btnList.Length; i++)
		{
			CButtonObsolete btn = btnList[i];
			bool autoListen = btn.AutoListen;
			if (autoListen)
			{
				btn.onClick.AddListener(delegate()
				{
					this.OnClick(btn);
				});
			}
		}
		this.OnInit();
	}

	// Token: 0x0600217F RID: 8575 RVA: 0x000F4275 File Offset: 0x000F2475
	private void OnDestroy()
	{
		this.OnLeaveWorld();
	}

	// Token: 0x06002180 RID: 8576 RVA: 0x000F4280 File Offset: 0x000F2480
	public void OnWorldDataReady()
	{
		this.OnLeaveWorld();
		this._gameDataListenerId = GameDataBridge.RegisterListener(new GameDataBridge.NotificationHandler(this.OnNotifyGameData));
		GameDataBridge.AddDataMonitor(this._gameDataListenerId, 8, 19, ulong.MaxValue, uint.MaxValue);
		GameDataBridge.AddDataMonitor(this._gameDataListenerId, 8, 16, ulong.MaxValue, uint.MaxValue);
	}

	// Token: 0x06002181 RID: 8577 RVA: 0x000F42D0 File Offset: 0x000F24D0
	public void OnLeaveWorld()
	{
		bool flag = this._gameDataListenerId != -1;
		if (flag)
		{
			GameDataBridge.AddDataUnMonitor(this._gameDataListenerId, 8, 19, ulong.MaxValue, uint.MaxValue);
			GameDataBridge.AddDataUnMonitor(this._gameDataListenerId, 8, 16, ulong.MaxValue, uint.MaxValue);
			bool flag2 = this._editingCharId >= 0;
			if (flag2)
			{
				GameDataBridge.AddDataUnMonitor(this._gameDataListenerId, 4, 0, (ulong)this._editingCharId, 59U);
				this._editingCharId = -1;
			}
			GameDataBridge.UnregisterListener(this._gameDataListenerId);
			this._gameDataListenerId = -1;
		}
	}

	// Token: 0x06002182 RID: 8578 RVA: 0x000F4357 File Offset: 0x000F2557
	public void OnEnable()
	{
		this._taiwuCharId = SingletonObject.getInstance<BasicGameData>().TaiwuCharId;
	}

	// Token: 0x06002183 RID: 8579 RVA: 0x000F436C File Offset: 0x000F256C
	private void OnInit()
	{
		List<string> equipTypeList = this.NewEquipTypes;
		this.LearnedTypeDropDown = base.CGet<CDropdownLegacy>("LearnedTypeDropdown");
		this.LearnedTypeDropDown.ClearOptions();
		this.LearnedTypeDropDown.AddOptions(equipTypeList);
		this.LearnedTypeDropDown.value = 1;
		this.NotLearnTypeDropDown = base.CGet<CDropdownLegacy>("NotLearnTypeDropdown");
		this.NotLearnTypeDropDown.ClearOptions();
		this.NotLearnTypeDropDown.AddOptions(equipTypeList);
		this.NotLearnTypeDropDown.value = 1;
		List<string> gangList = this.NewGangTypes;
		this.LearnedGangDropDown = base.CGet<CDropdownLegacy>("LearnedGangDropdown");
		this.LearnedGangDropDown.ClearOptions();
		this.LearnedGangDropDown.AddOptions(gangList);
		this.LearnedGangDropDown.value = gangList.Count - 1;
		this.NotLearnGangDropDown = base.CGet<CDropdownLegacy>("NotLearnGangDropdown");
		this.NotLearnGangDropDown.ClearOptions();
		this.NotLearnGangDropDown.AddOptions(gangList);
		this.NotLearnGangDropDown.value = gangList.Count - 1;
		this.LearnedCombatSkillScroll = base.CGet<CScrollRectLegacy>("LearnedSkillScroll");
		this.LearnedCombatSkillToggleGroup = this.LearnedCombatSkillScroll.GetComponent<CToggleGroupObsolete>();
		this.LearnedPracticeLevelInput = base.CGet<TMP_InputField>("LearnedPracticeLevelInput");
		this.NotLearnCombatSkillScroll = base.CGet<CScrollRectLegacy>("NotLearnSkillScroll");
		this.NotLearnCombatSkillToggleGroup = this.NotLearnCombatSkillScroll.GetComponent<CToggleGroupObsolete>();
		this.NotLearnPracticeLevelInput = base.CGet<TMP_InputField>("NotLearnPracticeLevelInput");
		this.ModifyPagePanel = base.CGet<GameObject>("ModifyPagePanel");
		Refers modifyPagePanelRefers = this.ModifyPagePanel.GetComponent<Refers>();
		RectTransform firstPageHolder = modifyPagePanelRefers.CGet<RectTransform>("FirstPageHolder");
		RectTransform directPageHolder = modifyPagePanelRefers.CGet<RectTransform>("DirectPageHolder");
		RectTransform reversePageHolder = modifyPagePanelRefers.CGet<RectTransform>("ReversePageHolder");
		this.ReadTogGroup = modifyPagePanelRefers.CGet<CToggleGroupObsolete>("ReadTogGroup");
		this.FirstPageActiveTogGroup = modifyPagePanelRefers.CGet<CToggleGroupObsolete>("FirstPageActiveTogGroup");
		this.OtherPageActiveTogGroup = modifyPagePanelRefers.CGet<CToggleGroupObsolete>("OtherPageActiveTogGroup");
		this.ReadTogGroup.InitPreOnToggle(-1);
		this.FirstPageActiveTogGroup.InitPreOnToggle(-1);
		this.OtherPageActiveTogGroup.InitPreOnToggle(-1);
		for (sbyte i = 0; i < 5; i += 1)
		{
			Refers pageRefers = firstPageHolder.GetChild((int)i).GetComponent<Refers>();
			CToggleObsolete readTog = pageRefers.CGet<CToggleObsolete>("ReadTog");
			CToggleObsolete activeTog = pageRefers.CGet<CToggleObsolete>("ActiveTog");
			activeTog.Key = (readTog.Key = (int)CombatSkillStateHelper.GetOutlinePageInternalIndex(i));
			this.ReadTogGroup.Add(readTog);
			this.FirstPageActiveTogGroup.Add(activeTog);
			byte directKey = CombatSkillStateHelper.GetNormalPageInternalIndex(0, (byte)(i + 1));
			byte reverseKey = CombatSkillStateHelper.GetNormalPageInternalIndex(1, (byte)(i + 1));
			pageRefers = directPageHolder.GetChild((int)i).GetComponent<Refers>();
			readTog = pageRefers.CGet<CToggleObsolete>("ReadTog");
			activeTog = pageRefers.CGet<CToggleObsolete>("ActiveTog");
			activeTog.Key = (readTog.Key = (int)directKey);
			activeTog.onValueChanged.AddListener(delegate(bool value)
			{
				if (value)
				{
					this.OtherPageActiveTogGroup.SetWithoutNotify((int)reverseKey, false);
				}
			});
			this.ReadTogGroup.Add(readTog);
			this.OtherPageActiveTogGroup.Add(activeTog);
			pageRefers = reversePageHolder.GetChild((int)i).GetComponent<Refers>();
			readTog = pageRefers.CGet<CToggleObsolete>("ReadTog");
			activeTog = pageRefers.CGet<CToggleObsolete>("ActiveTog");
			activeTog.Key = (readTog.Key = (int)reverseKey);
			activeTog.onValueChanged.AddListener(delegate(bool value)
			{
				if (value)
				{
					this.OtherPageActiveTogGroup.SetWithoutNotify((int)directKey, false);
				}
			});
			this.ReadTogGroup.Add(readTog);
			this.OtherPageActiveTogGroup.Add(activeTog);
		}
		this.ReadTogGroup.OnActiveToggleChange = new Action<CToggleObsolete, CToggleObsolete>(this.OnReadPageChange);
		this.FirstPageActiveTogGroup.OnActiveToggleChange = delegate(CToggleObsolete togNew, CToggleObsolete togOld)
		{
			this.UpdateModifyPageCanSave();
		};
		this.OtherPageActiveTogGroup.OnActiveToggleChange = delegate(CToggleObsolete togNew, CToggleObsolete togOld)
		{
			this.UpdateModifyPageCanSave();
		};
		this.LearnedTypeDropDown.onValueChanged.AddListener(delegate(int value)
		{
			this._selectedLearnedList.Clear();
			this.UpdateCombatSkillList(GMCombatSkillEditor.SkillStateType.Learned);
		});
		this.LearnedGangDropDown.onValueChanged.AddListener(delegate(int value)
		{
			this._selectedLearnedList.Clear();
			this.UpdateCombatSkillList(GMCombatSkillEditor.SkillStateType.Learned);
		});
		this.NotLearnTypeDropDown.onValueChanged.AddListener(delegate(int value)
		{
			this._selectedNotLearnList.Clear();
			this.UpdateCombatSkillList(GMCombatSkillEditor.SkillStateType.NotLearn);
		});
		this.NotLearnGangDropDown.onValueChanged.AddListener(delegate(int value)
		{
			this._selectedNotLearnList.Clear();
			this.UpdateCombatSkillList(GMCombatSkillEditor.SkillStateType.NotLearn);
		});
		this.LearnedCombatSkillToggleGroup.OnActiveToggleChange = new Action<CToggleObsolete, CToggleObsolete>(this.OnLearnedTogChanged);
		this.NotLearnCombatSkillToggleGroup.OnActiveToggleChange = new Action<CToggleObsolete, CToggleObsolete>(this.OnNotLearnTogChanged);
		this.LearnedPracticeLevelInput.onEndEdit.AddListener(delegate(string value)
		{
			this.OnEditLearnedCombatSkillPracticeLevel(value);
		});
		this.NotLearnPracticeLevelInput.onEndEdit.AddListener(delegate(string value)
		{
		});
	}

	// Token: 0x06002184 RID: 8580 RVA: 0x000F4854 File Offset: 0x000F2A54
	public void OnClick(CButtonObsolete button)
	{
		string btnName = button.name;
		bool flag = btnName == "Forget";
		if (flag)
		{
			foreach (short skillTemplateId in this._selectedLearnedList)
			{
				CharacterDomainMethod.Call.GmCmd_ForgetCombatSkill(this._editingCharId, skillTemplateId);
				this._displayDataDict.Remove(skillTemplateId);
			}
			this._selectedLearnedList.Clear();
		}
		else
		{
			bool flag2 = btnName == "Learn";
			if (flag2)
			{
				ushort readingState = ushort.MaxValue;
				foreach (short skillTemplateId2 in this._selectedNotLearnList)
				{
					CharacterDomainMethod.Call.LearnCombatSkill(this._editingCharId, skillTemplateId2, readingState);
					bool flag3 = SingletonObject.getInstance<BasicGameData>().TaiwuCharId == this._editingCharId;
					if (flag3)
					{
						TaiwuDomainMethod.Call.GmCmd_SetTaiwuCombatSkillActiveState(skillTemplateId2, 996, base.CGet<CToggleObsolete>("Bonus").isOn);
					}
					CombatSkillModel.GetCombatSkillDisplayData(this._gameDataListenerId, this._editingCharId, new List<short>
					{
						skillTemplateId2
					});
				}
				this._selectedNotLearnList.Clear();
			}
			else
			{
				bool flag4 = btnName == "Revoke";
				if (flag4)
				{
					CharacterDomainMethod.Call.GmCmd_RevokeCombatSkill(this._editingCharId, this._selectedLearnedList);
				}
				else
				{
					bool flag5 = btnName == "ModifyPage";
					if (flag5)
					{
						bool flag6 = this.LearnedCombatSkillToggleGroup.GetActive() == null;
						if (!flag6)
						{
							short skillTemplateId3 = (short)this.LearnedCombatSkillToggleGroup.GetActive().Key;
							CombatSkillDisplayData displayData = this._displayDataDict[skillTemplateId3];
							List<CToggleObsolete> readTogList = this.ReadTogGroup.GetAll();
							List<CToggleObsolete> firstPageActiveTogList = this.FirstPageActiveTogGroup.GetAll();
							List<CToggleObsolete> otherPageActiveTogList = this.OtherPageActiveTogGroup.GetAll();
							base.CGet<TextMeshProUGUI>("ModifyPageSkillName").text = CombatSkill.Instance[skillTemplateId3].Name;
							foreach (CToggleObsolete readTog in readTogList)
							{
								this.ReadTogGroup.Set(readTog, CombatSkillStateHelper.IsPageRead(displayData.ReadingState, (byte)readTog.Key));
							}
							foreach (CToggleObsolete activeTog in firstPageActiveTogList)
							{
								this.FirstPageActiveTogGroup.Set(activeTog, CombatSkillStateHelper.IsPageActive(displayData.ActivationState, (byte)activeTog.Key));
								activeTog.interactable = CombatSkillStateHelper.IsPageRead(displayData.ReadingState, (byte)activeTog.Key);
								activeTog.isOn = CombatSkillStateHelper.IsPageActive(displayData.ActivationState, (byte)activeTog.Key);
							}
							foreach (CToggleObsolete activeTog2 in otherPageActiveTogList)
							{
								this.OtherPageActiveTogGroup.Set(activeTog2, CombatSkillStateHelper.IsPageActive(displayData.ActivationState, (byte)activeTog2.Key));
								activeTog2.interactable = CombatSkillStateHelper.IsPageRead(displayData.ReadingState, (byte)activeTog2.Key);
								activeTog2.isOn = CombatSkillStateHelper.IsPageActive(displayData.ActivationState, (byte)activeTog2.Key);
							}
							this.ModifyPagePanel.SetActive(true);
						}
					}
					else
					{
						bool flag7 = btnName == "ConfirmModifyPage";
						if (flag7)
						{
							List<CToggleObsolete> selectedSkills = this.LearnedCombatSkillToggleGroup.GetAllActive();
							for (int i = 0; i < selectedSkills.Count; i++)
							{
								short skillTemplateId4 = (short)selectedSkills[i].Key;
								CombatSkillDisplayData displayData2 = this._displayDataDict[skillTemplateId4];
								List<CToggleObsolete> readTogList2 = this.ReadTogGroup.GetAll();
								List<CToggleObsolete> firstPageActiveTogList2 = this.FirstPageActiveTogGroup.GetAll();
								List<CToggleObsolete> otherPageActiveTogList2 = this.OtherPageActiveTogGroup.GetAll();
								foreach (CToggleObsolete readTog2 in readTogList2)
								{
									displayData2.ReadingState = (readTog2.isOn ? CombatSkillStateHelper.SetPageRead(displayData2.ReadingState, (byte)readTog2.Key) : CombatSkillStateHelper.SetPageUnread(displayData2.ReadingState, (byte)readTog2.Key));
								}
								foreach (CToggleObsolete activeTog3 in firstPageActiveTogList2)
								{
									displayData2.ActivationState = (activeTog3.isOn ? CombatSkillStateHelper.SetPageActive(displayData2.ActivationState, (byte)activeTog3.Key) : CombatSkillStateHelper.SetPageInactive(displayData2.ActivationState, (byte)activeTog3.Key));
								}
								foreach (CToggleObsolete activeTog4 in otherPageActiveTogList2)
								{
									displayData2.ActivationState = (activeTog4.isOn ? CombatSkillStateHelper.SetPageActive(displayData2.ActivationState, (byte)activeTog4.Key) : CombatSkillStateHelper.SetPageInactive(displayData2.ActivationState, (byte)activeTog4.Key));
								}
								GameDataBridge.AddDataModification<ushort>(7, 0, (ulong)new CombatSkillKey(this._editingCharId, skillTemplateId4), 1U, displayData2.ReadingState);
								bool flag8 = this._editingCharId == this._taiwuCharId;
								if (flag8)
								{
									TaiwuDomainMethod.Call.GmCmd_SetTaiwuCombatSkillActiveState(skillTemplateId4, displayData2.ActivationState, base.CGet<CToggleObsolete>("Bonus").isOn);
								}
								else
								{
									GameDataBridge.AddDataModification<ushort>(7, 0, (ulong)new CombatSkillKey(this._editingCharId, skillTemplateId4), 2U, displayData2.ActivationState);
								}
								this.ModifyPagePanel.SetActive(false);
								GEvent.OnEvent(UiEvents.TopUiChanged, null);
							}
						}
						else
						{
							bool flag9 = btnName == "CancelModifyPage";
							if (flag9)
							{
								this.ModifyPagePanel.SetActive(false);
							}
							else
							{
								bool flag10 = btnName == "Close";
								if (flag10)
								{
									base.gameObject.SetActive(false);
								}
							}
						}
					}
				}
			}
		}
	}

	// Token: 0x06002185 RID: 8581 RVA: 0x000F4EEC File Offset: 0x000F30EC
	private void OnNotifyGameData(List<NotificationWrapper> notifications)
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
					bool flag = notification.DomainId == 7 && notification.MethodId == 0;
					if (flag)
					{
						List<CombatSkillDisplayData> dataList = null;
						Serializer.Deserialize(wrapper.DataPool, notification.ValueOffset, ref dataList);
						foreach (CombatSkillDisplayData data in dataList)
						{
							bool flag2 = this._displayDataDict.ContainsKey(data.TemplateId);
							if (flag2)
							{
								this._displayDataDict[data.TemplateId] = data;
							}
							else
							{
								this._displayDataDict.Add(data.TemplateId, data);
							}
						}
						this.UpdateCombatSkillList(GMCombatSkillEditor.SkillStateType.All);
					}
				}
			}
			else
			{
				DataUid uid = notification.Uid;
				bool flag3 = uid.DomainId == 4 && uid.DataId == 0;
				if (flag3)
				{
					bool flag4 = (int)uid.SubId0 == this._editingCharId && uid.SubId1 == 59U;
					if (flag4)
					{
						Serializer.Deserialize(wrapper.DataPool, notification.ValueOffset, ref this._combatSkillTemplateIdList);
						this._displayDataDict.Clear();
						bool flag5 = this._combatSkillTemplateIdList.Count > 0;
						if (flag5)
						{
							CombatSkillModel.GetCombatSkillDisplayData(this._gameDataListenerId, this._editingCharId, this._combatSkillTemplateIdList);
						}
						else
						{
							this.UpdateCombatSkillList(GMCombatSkillEditor.SkillStateType.All);
						}
					}
				}
			}
		}
	}

	// Token: 0x06002186 RID: 8582 RVA: 0x000F50E8 File Offset: 0x000F32E8
	private void OnLearnedTogChanged(CToggleObsolete togOn, CToggleObsolete togOff)
	{
		bool flag = togOn != null;
		if (flag)
		{
			bool key = Input.GetKey(KeyCode.LeftShift);
			if (key)
			{
				List<CToggleObsolete> list = this.LearnedCombatSkillToggleGroup.GetAll();
				for (int i = list.IndexOf(togOn); i >= 0; i--)
				{
					short id = (short)list[i].Key;
					bool flag2 = this._selectedLearnedList.Contains(id);
					if (flag2)
					{
						break;
					}
					this.LearnedCombatSkillToggleGroup.Set((int)id, true, false);
				}
			}
			this.LearnedPracticeLevelInput.text = "100";
			this._selectedLearnedList.Add((short)togOn.Key);
		}
		bool flag3 = togOff != null;
		if (flag3)
		{
			this._selectedLearnedList.Remove((short)togOff.Key);
		}
		bool anySelected = this._selectedLearnedList.Count > 0;
		this.LearnedPracticeLevelInput.interactable = anySelected;
		base.CGet<CButtonObsolete>("Forget").interactable = anySelected;
		base.CGet<CButtonObsolete>("Revoke").interactable = anySelected;
		base.CGet<CButtonObsolete>("ModifyPage").interactable = anySelected;
	}

	// Token: 0x06002187 RID: 8583 RVA: 0x000F5210 File Offset: 0x000F3410
	private void OnNotLearnTogChanged(CToggleObsolete togOn, CToggleObsolete togOff)
	{
		bool flag = togOn != null;
		if (flag)
		{
			bool key = Input.GetKey(KeyCode.LeftShift);
			if (key)
			{
				List<CToggleObsolete> list = this.NotLearnCombatSkillToggleGroup.GetAll();
				for (int i = list.IndexOf(togOn); i >= 0; i--)
				{
					short id = (short)list[i].Key;
					bool flag2 = this._selectedNotLearnList.Contains(id);
					if (flag2)
					{
						break;
					}
					this.NotLearnCombatSkillToggleGroup.Set((int)id, true, false);
				}
			}
			this._selectedNotLearnList.Add((short)togOn.Key);
		}
		bool flag3 = togOff != null;
		if (flag3)
		{
			this._selectedNotLearnList.Remove((short)togOff.Key);
		}
		base.CGet<CButtonObsolete>("Learn").interactable = (this._selectedNotLearnList.Count > 0);
	}

	// Token: 0x06002188 RID: 8584 RVA: 0x000F52F0 File Offset: 0x000F34F0
	private void OnEditLearnedCombatSkillPracticeLevel(string value)
	{
		int result;
		int.TryParse(value, out result);
		foreach (short skillTemplateId in this._selectedLearnedList)
		{
			ExtraDomainMethod.Call.GmCmd_SetCharacterProficiencies(this._editingCharId, skillTemplateId, result);
		}
	}

	// Token: 0x06002189 RID: 8585 RVA: 0x000F5358 File Offset: 0x000F3558
	private void UpdateCombatSkillList(GMCombatSkillEditor.SkillStateType type = GMCombatSkillEditor.SkillStateType.All)
	{
		bool flag = this._editingCharId < 0;
		if (flag)
		{
			this._selectedLearnedList.Clear();
			this._selectedNotLearnList.Clear();
		}
		bool flag2 = type == GMCombatSkillEditor.SkillStateType.Learned || type == GMCombatSkillEditor.SkillStateType.All;
		if (flag2)
		{
			int equipType2 = this.LearnedTypeDropDown.value;
			int gangType2 = this.LearnedGangDropDown.value;
			this._learnedCombatSkillList.Clear();
			foreach (short skillTemplateId2 in this._displayDataDict.Keys)
			{
				bool flag3 = (equipType2 == this.LearnedTypeDropDown.options.Count - 1 || equipType2 == (int)CombatSkill.Instance[skillTemplateId2].EquipType) && (gangType2 == this.LearnedGangDropDown.options.Count - 1 || gangType2 == (int)CombatSkill.Instance[skillTemplateId2].SectId) && (string.IsNullOrEmpty(this._searchingName) || CombatSkill.Instance[skillTemplateId2].Name.Contains(this._searchingName));
				if (flag3)
				{
					this._learnedCombatSkillList.Add(skillTemplateId2);
				}
			}
			this.UpdateCombatSkillScrollList(this._learnedCombatSkillList, this._selectedLearnedList, this.LearnedCombatSkillScroll);
			base.CGet<CButtonObsolete>("Forget").interactable = (this._selectedLearnedList.Count > 0);
			base.CGet<CButtonObsolete>("Revoke").interactable = (this._selectedLearnedList.Count > 0);
		}
		bool flag4 = type == GMCombatSkillEditor.SkillStateType.NotLearn || type == GMCombatSkillEditor.SkillStateType.All;
		if (flag4)
		{
			int equipType = this.NotLearnTypeDropDown.value;
			int gangType = this.NotLearnGangDropDown.value;
			this._notLearnCombatSkillList = CombatSkill.Instance.GetAllKeys().FindAll((short skillTemplateId) => (equipType == this.NotLearnTypeDropDown.options.Count - 1 || equipType == (int)CombatSkill.Instance[skillTemplateId].EquipType) && (gangType == this.NotLearnGangDropDown.options.Count - 1 || gangType == (int)CombatSkill.Instance[skillTemplateId].SectId) && (string.IsNullOrEmpty(this._searchingName) || CombatSkill.Instance[skillTemplateId].Name.Contains(this._searchingName)) && !this._displayDataDict.ContainsKey(skillTemplateId));
			this.UpdateCombatSkillScrollList(this._notLearnCombatSkillList, this._selectedNotLearnList, this.NotLearnCombatSkillScroll);
			base.CGet<CButtonObsolete>("Learn").interactable = (this._selectedNotLearnList.Count > 0);
		}
	}

	// Token: 0x0600218A RID: 8586 RVA: 0x000F55A4 File Offset: 0x000F37A4
	private void UpdateCombatSkillScrollList(List<short> skillIdList, List<short> selectedList, CScrollRectLegacy skillScroll)
	{
		GameObject skillPrefab = base.CGet<GameObject>("CombatSkillPrefab");
		CToggleGroupObsolete togGroup = skillScroll.GetComponent<CToggleGroupObsolete>();
		int childCount = skillScroll.Content.childCount;
		togGroup.Clear();
		for (int i = 0; i < skillIdList.Count; i++)
		{
			short skillTemplateId = skillIdList[i];
			GameObject skillObj = (i < childCount) ? skillScroll.Content.GetChild(i).gameObject : Object.Instantiate<GameObject>(skillPrefab, skillScroll.Content);
			Refers skillRefers = skillObj.GetComponent<Refers>();
			CToggleObsolete tog = skillRefers.GetComponent<CToggleObsolete>();
			skillRefers.CGet<TextMeshProUGUI>("NameText").text = CombatSkill.Instance[skillTemplateId].Name;
			tog.Key = (int)skillTemplateId;
			tog.isOn = selectedList.Contains(skillTemplateId);
			tog.interactable = (this._editingCharId >= 0);
			togGroup.Add(tog);
			skillObj.SetActive(true);
		}
		for (int j = skillIdList.Count; j < childCount; j++)
		{
			skillScroll.Content.GetChild(j).gameObject.SetActive(false);
		}
	}

	// Token: 0x0600218B RID: 8587 RVA: 0x000F56CD File Offset: 0x000F38CD
	public void Update()
	{
		this.ChangeCharacter(GMFunc.CombatSkillCharId);
		this.SearchTarget(GMFunc.CombatSkillTarget);
	}

	// Token: 0x0600218C RID: 8588 RVA: 0x000F56E8 File Offset: 0x000F38E8
	private void ChangeCharacter(int charId)
	{
		bool flag = charId == this._editingCharId;
		if (!flag)
		{
			this._selectedLearnedList.Clear();
			this._selectedNotLearnList.Clear();
			bool flag2 = this._editingCharId >= 0;
			if (flag2)
			{
				GameDataBridge.AddDataUnMonitor(this._gameDataListenerId, 4, 0, (ulong)this._editingCharId, 59U);
			}
			this._editingCharId = charId;
			bool flag3 = this._editingCharId >= 0;
			if (flag3)
			{
				GameDataBridge.AddDataMonitor(this._gameDataListenerId, 4, 0, (ulong)this._editingCharId, 59U);
			}
			else
			{
				this._combatSkillTemplateIdList.Clear();
				this._displayDataDict.Clear();
				this.UpdateCombatSkillList(GMCombatSkillEditor.SkillStateType.All);
			}
		}
	}

	// Token: 0x0600218D RID: 8589 RVA: 0x000F579C File Offset: 0x000F399C
	private void SearchTarget(string targetName)
	{
		bool flag = targetName == this._searchingName;
		if (!flag)
		{
			this._searchingName = targetName;
			bool flag2 = string.IsNullOrEmpty(this._searchingName);
			if (flag2)
			{
				this.UpdateCombatSkillList(GMCombatSkillEditor.SkillStateType.All);
			}
			else
			{
				this._selectedLearnedList.Clear();
				this._selectedNotLearnList.Clear();
				this.LearnedTypeDropDown.value = this.NewEquipTypes.Count - 1;
				this.NotLearnTypeDropDown.value = this.NewEquipTypes.Count - 1;
				this.LearnedGangDropDown.value = this.NewGangTypes.Count - 1;
				this.NotLearnGangDropDown.value = this.NewGangTypes.Count - 1;
				this.UpdateCombatSkillList(GMCombatSkillEditor.SkillStateType.All);
			}
		}
	}

	// Token: 0x0600218E RID: 8590 RVA: 0x000F5868 File Offset: 0x000F3A68
	private void OnReadPageChange(CToggleObsolete togOn, CToggleObsolete togOff)
	{
		bool flag = togOn != null;
		if (flag)
		{
			bool flag2 = this.FirstPageActiveTogGroup.Get(togOn.Key) != null;
			if (flag2)
			{
				this.FirstPageActiveTogGroup.Get(togOn.Key).interactable = true;
			}
			else
			{
				this.OtherPageActiveTogGroup.Get(togOn.Key).interactable = true;
			}
		}
		bool flag3 = togOff != null;
		if (flag3)
		{
			bool flag4 = this.FirstPageActiveTogGroup.Get(togOff.Key) != null;
			if (flag4)
			{
				this.FirstPageActiveTogGroup.Set(togOff.Key, false, false);
				this.FirstPageActiveTogGroup.Get(togOff.Key).interactable = false;
			}
			else
			{
				this.OtherPageActiveTogGroup.Set(togOff.Key, false, false);
				this.OtherPageActiveTogGroup.Get(togOff.Key).interactable = false;
			}
		}
	}

	// Token: 0x0600218F RID: 8591 RVA: 0x000F5958 File Offset: 0x000F3B58
	private void UpdateModifyPageCanSave()
	{
		int firstPageActiveCount = this.FirstPageActiveTogGroup.GetAllActive().Count;
		int otherPageActiveCount = this.OtherPageActiveTogGroup.GetAllActive().Count;
		base.CGet<CButtonObsolete>("ConfirmModifyPage").interactable = ((firstPageActiveCount == 0 && otherPageActiveCount == 0) || (firstPageActiveCount == 1 && otherPageActiveCount == 5));
	}

	// Token: 0x040019E1 RID: 6625
	private List<short> _learnedCombatSkillList = new List<short>();

	// Token: 0x040019E2 RID: 6626
	private List<short> _notLearnCombatSkillList = new List<short>();

	// Token: 0x040019E3 RID: 6627
	private readonly List<short> _selectedLearnedList = new List<short>();

	// Token: 0x040019E4 RID: 6628
	private readonly List<short> _selectedNotLearnList = new List<short>();

	// Token: 0x040019E5 RID: 6629
	private int _gameDataListenerId = -1;

	// Token: 0x040019E6 RID: 6630
	private int _taiwuCharId = -1;

	// Token: 0x040019E7 RID: 6631
	private int _editingCharId = -1;

	// Token: 0x040019E8 RID: 6632
	private string _searchingName = string.Empty;

	// Token: 0x040019E9 RID: 6633
	private List<short> _combatSkillTemplateIdList = new List<short>();

	// Token: 0x040019EA RID: 6634
	private readonly Dictionary<short, CombatSkillDisplayData> _displayDataDict = new Dictionary<short, CombatSkillDisplayData>();

	// Token: 0x040019EB RID: 6635
	public CDropdownLegacy LearnedTypeDropDown;

	// Token: 0x040019EC RID: 6636
	public CDropdownLegacy LearnedGangDropDown;

	// Token: 0x040019ED RID: 6637
	public CScrollRectLegacy LearnedCombatSkillScroll;

	// Token: 0x040019EE RID: 6638
	public CToggleGroupObsolete LearnedCombatSkillToggleGroup;

	// Token: 0x040019EF RID: 6639
	public TMP_InputField LearnedPracticeLevelInput;

	// Token: 0x040019F0 RID: 6640
	public CDropdownLegacy NotLearnTypeDropDown;

	// Token: 0x040019F1 RID: 6641
	public CDropdownLegacy NotLearnGangDropDown;

	// Token: 0x040019F2 RID: 6642
	public CScrollRectLegacy NotLearnCombatSkillScroll;

	// Token: 0x040019F3 RID: 6643
	public CToggleGroupObsolete NotLearnCombatSkillToggleGroup;

	// Token: 0x040019F4 RID: 6644
	public TMP_InputField NotLearnPracticeLevelInput;

	// Token: 0x040019F5 RID: 6645
	public GameObject ModifyPagePanel;

	// Token: 0x040019F6 RID: 6646
	public CToggleGroupObsolete ReadTogGroup;

	// Token: 0x040019F7 RID: 6647
	public CToggleGroupObsolete FirstPageActiveTogGroup;

	// Token: 0x040019F8 RID: 6648
	public CToggleGroupObsolete OtherPageActiveTogGroup;

	// Token: 0x0200149A RID: 5274
	private enum SkillStateType
	{
		// Token: 0x0400A1D2 RID: 41426
		Learned,
		// Token: 0x0400A1D3 RID: 41427
		NotLearn,
		// Token: 0x0400A1D4 RID: 41428
		All
	}
}
