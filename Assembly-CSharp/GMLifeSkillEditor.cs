using System;
using System.Collections.Generic;
using Config;
using FrameWork;
using GameData.Common;
using GameData.Domains.Character;
using GameData.GameDataBridge;
using GameData.Serializer;
using TMPro;
using UnityEngine;

// Token: 0x02000210 RID: 528
public class GMLifeSkillEditor : MonoBehaviour
{
	// Token: 0x06002198 RID: 8600 RVA: 0x000F5AA0 File Offset: 0x000F3CA0
	private void SetChar(int charId)
	{
		bool flag = this._charId == charId;
		if (!flag)
		{
			bool flag2 = this._charId >= 0;
			if (flag2)
			{
				GameDataBridge.AddDataUnMonitor(this._listenerId, 4, 0, (ulong)((long)this._charId), 29U);
			}
			this._charId = charId;
			bool flag3 = this._charId >= 0;
			if (flag3)
			{
				GameDataBridge.AddDataMonitor(this._listenerId, 4, 0, (ulong)((long)this._charId), 29U);
			}
			else
			{
				this._learnedLifeSkills.Clear();
				this.RefreshScroll();
			}
		}
	}

	// Token: 0x06002199 RID: 8601 RVA: 0x000F5B29 File Offset: 0x000F3D29
	public void OnWorldDataReady()
	{
		this.OnLeaveWorld();
		this._listenerId = GameDataBridge.RegisterListener(new GameDataBridge.NotificationHandler(this.OnNotifyGameData));
	}

	// Token: 0x0600219A RID: 8602 RVA: 0x000F5B4C File Offset: 0x000F3D4C
	public void OnLeaveWorld()
	{
		bool flag = this._listenerId != -1;
		if (flag)
		{
			bool flag2 = this._charId >= 0;
			if (flag2)
			{
				GameDataBridge.AddDataUnMonitor(this._listenerId, 4, 0, (ulong)((long)this._charId), 29U);
				this._charId = -1;
			}
			GameDataBridge.UnregisterListener(this._listenerId);
			this._listenerId = -1;
		}
	}

	// Token: 0x0600219B RID: 8603 RVA: 0x000F5BAF File Offset: 0x000F3DAF
	private void OnDestroy()
	{
		this.OnLeaveWorld();
	}

	// Token: 0x0600219C RID: 8604 RVA: 0x000F5BBC File Offset: 0x000F3DBC
	private void OnNotifyGameData(List<NotificationWrapper> notifications)
	{
		foreach (NotificationWrapper wrapper in notifications)
		{
			Notification notification = wrapper.Notification;
			byte type = notification.Type;
			byte b = type;
			if (b == 0)
			{
				DataUid uid = notification.Uid;
				bool flag = uid.DomainId == 4 && uid.DataId == 0 && uid.SubId0 == (ulong)((long)this._charId);
				if (flag)
				{
					bool flag2 = uid.SubId1 == 29U;
					if (flag2)
					{
						Serializer.Deserialize(wrapper.DataPool, notification.ValueOffset, ref this._learnedLifeSkills);
						this.RefreshScroll();
					}
				}
			}
		}
	}

	// Token: 0x0600219D RID: 8605 RVA: 0x000F5C90 File Offset: 0x000F3E90
	public void Init()
	{
		this.SkillScroll.OnItemRender = new Action<int, Refers>(this.OnSkillItemRender);
		this._lifeSkillTypeIdList = Config.LifeSkillType.Instance.GetAllKeys();
		this.TypeDropdown.ClearOptions();
		this.TypeDropdown.AddOptions(this._lifeSkillTypeIdList.ConvertAll<string>((sbyte e) => Config.LifeSkillType.Instance[e].Name));
		this.TypeDropdown.onValueChanged.AddListener(delegate(int index)
		{
			this._curLifeSkillTypeIndex = index;
			this.RefreshScroll();
		});
		for (int i = 0; i < this.PageToggles.Length; i++)
		{
			int index = i;
			this.PageToggles[index].onValueChanged.AddListener(delegate(bool isOn)
			{
				if (isOn)
				{
					this.PageStateLabels[index].text = this.SetToReadTexts[index];
				}
				else
				{
					this.PageStateLabels[index].text = this.SetToUnReadTexts[index];
				}
			});
		}
		this._showingLifeSkillIdList = new List<short>();
		this._selectedLifeSkillIdList = new List<short>();
		this.RefreshScroll();
	}

	// Token: 0x0600219E RID: 8606 RVA: 0x000F5D92 File Offset: 0x000F3F92
	private void Update()
	{
		this.SetChar(GMFunc.LifeSkillCharId);
	}

	// Token: 0x0600219F RID: 8607 RVA: 0x000F5DA4 File Offset: 0x000F3FA4
	private void RefreshScroll()
	{
		this._showingLifeSkillIdList.Clear();
		this._selectedLifeSkillIdList.Clear();
		LifeSkill.Instance.Iterate(delegate(Config.LifeSkillItem item)
		{
			bool flag = item.Type == this._lifeSkillTypeIdList[this._curLifeSkillTypeIndex];
			if (flag)
			{
				this._showingLifeSkillIdList.Add(item.TemplateId);
			}
			return true;
		});
		this.SkillScroll.UpdateData(this._showingLifeSkillIdList.Count);
		this.RefreshUnlockInfos();
		this.RefreshBookReadState();
	}

	// Token: 0x060021A0 RID: 8608 RVA: 0x000F5E08 File Offset: 0x000F4008
	private void RefreshUnlockInfos()
	{
		bool flag = this._selectedLifeSkillIdList.Count <= 0;
		if (flag)
		{
			this.UnlockTips.text = "先点击左侧列表选择要编辑的技艺书籍";
		}
		else
		{
			bool flag2 = this._selectedLifeSkillIdList.Count == 1;
			if (flag2)
			{
				Config.LifeSkillItem config = LifeSkill.Instance[this._selectedLifeSkillIdList[0]];
				List<string> unlockBuildings2 = new List<string>();
				List<short> idList = CommonUtils.GetUnlockBuildingListFromConfig(config, EasyPool.Get<List<short>>());
				using (List<short>.Enumerator enumerator = idList.GetEnumerator())
				{
					while (enumerator.MoveNext())
					{
						int buildingTemplateId = (int)enumerator.Current;
						unlockBuildings2.Add(BuildingBlock.Instance[buildingTemplateId].Name);
					}
				}
				EasyPool.Free<List<short>>(idList);
				this.UnlockTips.text = config.Name + ":可以解锁以下建筑：" + string.Join("、", unlockBuildings2);
			}
			else
			{
				List<string> unlockBuildings = new List<string>();
				this._selectedLifeSkillIdList.ForEach(delegate(short id)
				{
					Config.LifeSkillItem config2 = LifeSkill.Instance[id];
					List<short> idList2 = CommonUtils.GetUnlockBuildingListFromConfig(config2, EasyPool.Get<List<short>>());
					using (List<short>.Enumerator enumerator2 = idList2.GetEnumerator())
					{
						while (enumerator2.MoveNext())
						{
							int buildingTemplateId2 = (int)enumerator2.Current;
							unlockBuildings.Add(BuildingBlock.Instance[buildingTemplateId2].Name);
						}
					}
					EasyPool.Free<List<short>>(idList2);
				});
				this.UnlockTips.text = "选择了多个建筑，所有关联解锁建筑如下：" + string.Join("、", unlockBuildings);
			}
		}
	}

	// Token: 0x060021A1 RID: 8609 RVA: 0x000F5F60 File Offset: 0x000F4160
	private void RefreshBookReadState()
	{
		bool flag = this._selectedLifeSkillIdList.Count == 0 || this._learnedLifeSkills == null;
		if (flag)
		{
			this.RightArea.SetActive(false);
		}
		else
		{
			this.RightArea.SetActive(true);
			for (byte i = 0; i < 5; i += 1)
			{
				bool result = true;
				bool meetFlag = false;
				foreach (short id in this._selectedLifeSkillIdList)
				{
					for (int j = 0; j < this._learnedLifeSkills.Count; j++)
					{
						bool flag2 = this._learnedLifeSkills[j].SkillTemplateId == id;
						if (flag2)
						{
							meetFlag = true;
							result &= this._learnedLifeSkills[j].IsPageRead(i);
							break;
						}
					}
				}
				this.PageToggles[(int)i].isOn = (meetFlag && result);
			}
		}
	}

	// Token: 0x060021A2 RID: 8610 RVA: 0x000F6080 File Offset: 0x000F4280
	private void OnSkillItemRender(int index, Refers refers)
	{
		short templateId = this._showingLifeSkillIdList[index];
		CToggleObsolete toggle = refers.GetComponent<CToggleObsolete>();
		toggle.onValueChanged.RemoveAllListeners();
		toggle.isOn = this._selectedLifeSkillIdList.Contains(templateId);
		toggle.interactable = (this._charId >= 0);
		toggle.onValueChanged.AddListener(delegate(bool isOn)
		{
			if (isOn)
			{
				bool key = Input.GetKey(KeyCode.LeftControl);
				if (key)
				{
					bool flag = !this._selectedLifeSkillIdList.Contains(templateId);
					if (flag)
					{
						this._selectedLifeSkillIdList.Add(templateId);
					}
				}
				else
				{
					bool key2 = Input.GetKey(KeyCode.LeftShift);
					if (key2)
					{
						for (int i = index; i >= 0; i--)
						{
							short id = this._showingLifeSkillIdList[i];
							bool flag2 = this._selectedLifeSkillIdList.Contains(id);
							if (flag2)
							{
								break;
							}
							this._selectedLifeSkillIdList.Add(id);
						}
					}
					else
					{
						this._selectedLifeSkillIdList.Clear();
						this._selectedLifeSkillIdList.Add(templateId);
					}
				}
				this.RefreshBookReadState();
			}
			else
			{
				this._selectedLifeSkillIdList.Remove(templateId);
			}
			this.RefreshUnlockInfos();
			this.SkillScroll.ReRender();
		});
		refers.CGet<TextMeshProUGUI>("SkillName").text = LifeSkill.Instance[templateId].Name;
	}

	// Token: 0x060021A3 RID: 8611 RVA: 0x000F6138 File Offset: 0x000F4338
	public void OnConfirm()
	{
		for (int i = 0; i < this._selectedLifeSkillIdList.Count; i++)
		{
			short templateId = this._selectedLifeSkillIdList[i];
			GameData.Domains.Character.LifeSkillItem item = new GameData.Domains.Character.LifeSkillItem(templateId);
			bool meetFlag = false;
			for (int j = 0; j < this._learnedLifeSkills.Count; j++)
			{
				bool flag = this._learnedLifeSkills[j].SkillTemplateId == templateId;
				if (flag)
				{
					meetFlag = true;
					item = this._learnedLifeSkills[j];
					break;
				}
			}
			for (byte s = 0; s < 5; s += 1)
			{
				bool isOn = this.PageToggles[(int)s].isOn;
				if (isOn)
				{
					item.SetPageRead(s);
				}
				else
				{
					item.SetPageUnread(s);
				}
			}
			bool flag2 = !meetFlag;
			if (flag2)
			{
				this._learnedLifeSkills.Add(item);
			}
			this._learnedLifeSkills[this._learnedLifeSkills.FindIndex((GameData.Domains.Character.LifeSkillItem a) => a.SkillTemplateId == item.SkillTemplateId)] = item;
		}
		for (int k = this._learnedLifeSkills.Count - 1; k >= 0; k--)
		{
			GameData.Domains.Character.LifeSkillItem lifeSkill = this._learnedLifeSkills[k];
			bool flag3 = lifeSkill.GetReadPagesCount() <= 0 && this._selectedLifeSkillIdList.Contains(lifeSkill.SkillTemplateId);
			if (flag3)
			{
				this._learnedLifeSkills.RemoveAt(k);
			}
		}
		CharacterDomainMethod.Call.GmCmd_SetLearnedLifeSkills(this._charId, this._learnedLifeSkills);
	}

	// Token: 0x040019F9 RID: 6649
	public GameObject RightArea;

	// Token: 0x040019FA RID: 6650
	public CDropdownLegacy TypeDropdown;

	// Token: 0x040019FB RID: 6651
	public InfinityScrollLegacy SkillScroll;

	// Token: 0x040019FC RID: 6652
	public TextMeshProUGUI UnlockTips;

	// Token: 0x040019FD RID: 6653
	public CToggleObsolete[] PageToggles;

	// Token: 0x040019FE RID: 6654
	public TextMeshProUGUI[] PageStateLabels;

	// Token: 0x040019FF RID: 6655
	public string[] SetToReadTexts;

	// Token: 0x04001A00 RID: 6656
	public string[] SetToUnReadTexts;

	// Token: 0x04001A01 RID: 6657
	private int _listenerId = -1;

	// Token: 0x04001A02 RID: 6658
	private int _charId = -1;

	// Token: 0x04001A03 RID: 6659
	private List<sbyte> _lifeSkillTypeIdList;

	// Token: 0x04001A04 RID: 6660
	private int _curLifeSkillTypeIndex;

	// Token: 0x04001A05 RID: 6661
	private List<short> _showingLifeSkillIdList;

	// Token: 0x04001A06 RID: 6662
	private List<short> _selectedLifeSkillIdList;

	// Token: 0x04001A07 RID: 6663
	private List<GameData.Domains.Character.LifeSkillItem> _learnedLifeSkills = new List<GameData.Domains.Character.LifeSkillItem>();
}
