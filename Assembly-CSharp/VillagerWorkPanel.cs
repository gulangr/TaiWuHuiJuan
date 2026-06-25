using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using Config;
using Config.Common;
using FrameWork;
using Game.Components.Avatar;
using Game.Views.Select;
using GameData.Domains.Building;
using GameData.Domains.Character;
using GameData.Domains.Character.Display;
using GameData.Domains.Map;
using GameData.Domains.Taiwu;
using GameData.Domains.Taiwu.Display;
using GameData.Domains.Taiwu.VillagerRole;
using GameData.Serializer;
using GameData.Utilities;
using TMPro;
using UnityEngine;

// Token: 0x020003BA RID: 954
public class VillagerWorkPanel : Refers
{
	// Token: 0x170005D4 RID: 1492
	// (get) Token: 0x060039D2 RID: 14802 RVA: 0x001D6C9A File Offset: 0x001D4E9A
	private Refers SelectorCharacter
	{
		get
		{
			return base.CGet<Refers>("CharacterSelector");
		}
	}

	// Token: 0x170005D5 RID: 1493
	// (get) Token: 0x060039D3 RID: 14803 RVA: 0x001D6CA7 File Offset: 0x001D4EA7
	private RectTransform SelectorResource
	{
		get
		{
			return base.CGet<RectTransform>("ResourceSelector");
		}
	}

	// Token: 0x170005D6 RID: 1494
	// (get) Token: 0x060039D4 RID: 14804 RVA: 0x001D6CB4 File Offset: 0x001D4EB4
	private Game.Components.Avatar.Avatar Avatar
	{
		get
		{
			return this.SelectorCharacter.CGet<Game.Components.Avatar.Avatar>("Avatar");
		}
	}

	// Token: 0x170005D7 RID: 1495
	// (get) Token: 0x060039D5 RID: 14805 RVA: 0x001D6CC6 File Offset: 0x001D4EC6
	private CButtonObsolete SelectButton
	{
		get
		{
			return this.SelectorCharacter.CGet<CButtonObsolete>("Select");
		}
	}

	// Token: 0x170005D8 RID: 1496
	// (get) Token: 0x060039D6 RID: 14806 RVA: 0x001D6CD8 File Offset: 0x001D4ED8
	private CButtonObsolete ExchangeButton
	{
		get
		{
			return this.SelectorCharacter.CGet<CButtonObsolete>("Exchange");
		}
	}

	// Token: 0x170005D9 RID: 1497
	// (get) Token: 0x060039D7 RID: 14807 RVA: 0x001D6CEA File Offset: 0x001D4EEA
	private TextMeshProUGUI NameLabel
	{
		get
		{
			return this.SelectorCharacter.CGet<TextMeshProUGUI>("Name");
		}
	}

	// Token: 0x170005DA RID: 1498
	// (get) Token: 0x060039D8 RID: 14808 RVA: 0x001D6CFC File Offset: 0x001D4EFC
	private GameObject NameBack
	{
		get
		{
			return this.NameLabel.transform.parent.Find("NameBack").gameObject;
		}
	}

	// Token: 0x170005DB RID: 1499
	// (get) Token: 0x060039D9 RID: 14809 RVA: 0x001D6D1D File Offset: 0x001D4F1D
	private TextMeshProUGUI WorkStatusLabelActive
	{
		get
		{
			return this.SelectorCharacter.CGet<TextMeshProUGUI>("WorkStatusActive");
		}
	}

	// Token: 0x170005DC RID: 1500
	// (get) Token: 0x060039DA RID: 14810 RVA: 0x001D6D2F File Offset: 0x001D4F2F
	private TextMeshProUGUI WorkStatusLabelInActive
	{
		get
		{
			return this.SelectorCharacter.CGet<TextMeshProUGUI>("WorkStatusInActive");
		}
	}

	// Token: 0x170005DD RID: 1501
	// (get) Token: 0x060039DB RID: 14811 RVA: 0x001D6D41 File Offset: 0x001D4F41
	private RectTransform WorkButtonHolder
	{
		get
		{
			return this.SelectorCharacter.CGet<RectTransform>("WorkButtonHolder");
		}
	}

	// Token: 0x170005DE RID: 1502
	// (get) Token: 0x060039DC RID: 14812 RVA: 0x001D6D53 File Offset: 0x001D4F53
	private CButtonObsolete WorkButtonIdle
	{
		get
		{
			return this.WorkButtonHolder.Find("Idle").GetComponent<CButtonObsolete>();
		}
	}

	// Token: 0x170005DF RID: 1503
	// (get) Token: 0x060039DD RID: 14813 RVA: 0x001D6D6A File Offset: 0x001D4F6A
	private CButtonObsolete WorkButtonKeepGrave
	{
		get
		{
			return this.WorkButtonHolder.Find("KeepGrave").GetComponent<CButtonObsolete>();
		}
	}

	// Token: 0x170005E0 RID: 1504
	// (get) Token: 0x060039DE RID: 14814 RVA: 0x001D6D81 File Offset: 0x001D4F81
	private CButtonObsolete WorkButtonMigrate
	{
		get
		{
			return this.WorkButtonHolder.Find("Migrate").GetComponent<CButtonObsolete>();
		}
	}

	// Token: 0x170005E1 RID: 1505
	// (get) Token: 0x060039DF RID: 14815 RVA: 0x001D6D98 File Offset: 0x001D4F98
	private CButtonObsolete WorkButtonDevelop
	{
		get
		{
			Transform transform = this.WorkButtonHolder.Find("Develop");
			return (transform != null) ? transform.GetComponent<CButtonObsolete>() : null;
		}
	}

	// Token: 0x170005E2 RID: 1506
	// (get) Token: 0x060039E0 RID: 14816 RVA: 0x001D6DB6 File Offset: 0x001D4FB6
	private CButtonObsolete WorkButtonDebtCollection
	{
		get
		{
			return this.WorkButtonHolder.Find("DebtCollection").GetComponent<CButtonObsolete>();
		}
	}

	// Token: 0x170005E3 RID: 1507
	// (get) Token: 0x060039E1 RID: 14817 RVA: 0x001D6DCD File Offset: 0x001D4FCD
	private GameObject ResultGo
	{
		get
		{
			Transform transform = this.SelectorCharacter.transform.Find("Result");
			return (transform != null) ? transform.gameObject : null;
		}
	}

	// Token: 0x170005E4 RID: 1508
	// (get) Token: 0x060039E2 RID: 14818 RVA: 0x001D6DF0 File Offset: 0x001D4FF0
	private BuildingModel BuildingModel
	{
		get
		{
			return SingletonObject.getInstance<BuildingModel>();
		}
	}

	// Token: 0x060039E3 RID: 14819 RVA: 0x001D6DF8 File Offset: 0x001D4FF8
	public void Refresh(int listenerId, MapBlockData blockData, Dictionary<int, CharacterDisplayData> charDisplayDataDict, Action<int> onExchangeCharacterIdChanged, List<int> unlockedWorkingList)
	{
		VillagerWorkPanel.<>c__DisplayClass50_0 CS$<>8__locals1 = new VillagerWorkPanel.<>c__DisplayClass50_0();
		CS$<>8__locals1.blockData = blockData;
		CS$<>8__locals1.<>4__this = this;
		this._listenerId = listenerId;
		this._blockData = CS$<>8__locals1.blockData;
		this._charDisplayDataDict = charDisplayDataDict;
		this._onExchangeCharacterIdChanged = onExchangeCharacterIdChanged;
		int selectedCharacterId = -1;
		VillagerWorkData workData = null;
		bool flag = CS$<>8__locals1.blockData != null;
		if (flag)
		{
			IEnumerable<KeyValuePair<int, VillagerWorkData>> villagerWork = this.BuildingModel.VillagerWork;
			Func<KeyValuePair<int, VillagerWorkData>, bool> predicate;
			if ((predicate = CS$<>8__locals1.<>9__1) == null)
			{
				predicate = (CS$<>8__locals1.<>9__1 = ((KeyValuePair<int, VillagerWorkData> pair) => pair.Value.AreaId == CS$<>8__locals1.blockData.AreaId && pair.Value.BlockId == CS$<>8__locals1.blockData.BlockId && VillagerWorkType.IsWorkOnMap(pair.Value.WorkType)));
			}
			using (IEnumerator<KeyValuePair<int, VillagerWorkData>> enumerator = villagerWork.Where(predicate).GetEnumerator())
			{
				if (enumerator.MoveNext())
				{
					KeyValuePair<int, VillagerWorkData> pair2 = enumerator.Current;
					selectedCharacterId = pair2.Key;
					workData = pair2.Value;
				}
			}
		}
		this._unlockedWorkingList = unlockedWorkingList;
		this._selectedCharacterId = selectedCharacterId;
		this._workData = workData;
		this._resourceButtons.Clear();
		this._resourceButtons.Add(0, this.SelectorResource.Find("Food").GetComponent<CButtonObsolete>());
		this._resourceButtons.Add(5, this.SelectorResource.Find("Herbal").GetComponent<CButtonObsolete>());
		this._resourceButtons.Add(3, this.SelectorResource.Find("Jade").GetComponent<CButtonObsolete>());
		this._resourceButtons.Add(2, this.SelectorResource.Find("Stone").GetComponent<CButtonObsolete>());
		this._resourceButtons.Add(4, this.SelectorResource.Find("Silk").GetComponent<CButtonObsolete>());
		this._resourceButtons.Add(1, this.SelectorResource.Find("Wood").GetComponent<CButtonObsolete>());
		bool flag2 = this._selectedCharacterId >= 0;
		if (flag2)
		{
			this.RefreshVillagerInfoForFarmer(new Action(CS$<>8__locals1.<Refresh>g__RefreshPanel|0));
		}
		else
		{
			CS$<>8__locals1.<Refresh>g__RefreshPanel|0();
		}
	}

	// Token: 0x060039E4 RID: 14820 RVA: 0x001D6FF0 File Offset: 0x001D51F0
	private void RefreshVillagerInfoForFarmer(Action onRefreshVillager)
	{
		TaiwuDomainMethod.AsyncCall.GetVillagerRoleCharacterDisplayData(null, this._selectedCharacterId, delegate(int offset, RawDataPool dataPool)
		{
			Serializer.Deserialize(dataPool, offset, ref this._villagerDisplayData);
			this._isFarmer = (this._villagerDisplayData.RoleTemplateId == 0);
			bool hasAnyResourceEnough = false;
			bool flag = this._notBigSize && this._notDeveloped && hasAnyResourceEnough && this._isFarmer;
			if (flag)
			{
				this.WorkButtonMigrate.interactable = true;
			}
			Action onRefreshVillager2 = onRefreshVillager;
			if (onRefreshVillager2 != null)
			{
				onRefreshVillager2();
			}
			this.RefreshWorkButtonMigrateTips();
		});
	}

	// Token: 0x060039E5 RID: 14821 RVA: 0x001D702C File Offset: 0x001D522C
	private void RefreshBlockInfo()
	{
		this._notBigSize = (MapBlock.Instance[this._blockData.TemplateId].Size <= 1);
		this._notDeveloped = (MapBlock.Instance[this._blockData.TemplateId].Type != EMapBlockType.Developed);
	}

	// Token: 0x060039E6 RID: 14822 RVA: 0x001D7088 File Offset: 0x001D5288
	private void RefreshWork()
	{
		this._currentMigrateResourceType = ((this._workData != null && this._workData.WorkType == 14) ? this._workData.ResourceType : -1);
		bool flag = this._selectedCharacterId >= 0 && this._workData != null;
		if (flag)
		{
			MapBlockItem blockConfig = this._blockData.GetConfigSafe();
			foreach (KeyValuePair<sbyte, CButtonObsolete> pair in this._resourceButtons)
			{
				sbyte resourceType = pair.Key;
				CButtonObsolete button = pair.Value;
				ResourceTypeItem resourceTypeConfig = Config.ResourceType.Instance[resourceType];
				Action <>9__4;
				button.ClearAndAddListener(delegate
				{
					bool flag9 = this._workData.WorkType == 14;
					if (flag9)
					{
						bool flag10 = this._extraMigrateRate.Item1 > 0 && this._currentMigrateResourceType != resourceType;
						if (flag10)
						{
							DialogCmd dialogCmd = new DialogCmd();
							dialogCmd.Title = LocalStringManager.Get(LanguageKey.LK_Migrate_ChangeHintTitle);
							dialogCmd.Content = LocalStringManager.Get(LanguageKey.LK_Migrate_ChangeHint);
							dialogCmd.Type = 1;
							Action yes;
							if ((yes = <>9__4) == null)
							{
								yes = (<>9__4 = delegate()
								{
									SingletonObject.getInstance<WorldMapModel>().RequestStopVillagerWorkOnMap(this._blockData.GetLocation(), false);
									TaiwuDomainMethod.Call.SetVillagerMigrateWork(this._listenerId, this._selectedCharacterId, this._blockData.AreaId, this._blockData.BlockId, resourceType);
									this._currentMigrateResourceType = resourceType;
								});
							}
							dialogCmd.Yes = yes;
							dialogCmd.No = null;
							DialogCmd cmd = dialogCmd;
							UIElement.Dialog.SetOnInitArgs(EasyPool.Get<ArgumentBox>().SetObject("Cmd", cmd));
							UIManager.Instance.MaskUI(UIElement.Dialog);
						}
						else
						{
							SingletonObject.getInstance<WorldMapModel>().RequestStopVillagerWorkOnMap(this._blockData.GetLocation(), false);
							TaiwuDomainMethod.Call.SetVillagerMigrateWork(this._listenerId, this._selectedCharacterId, this._blockData.AreaId, this._blockData.BlockId, resourceType);
							this._currentMigrateResourceType = resourceType;
						}
						foreach (CButtonObsolete resourceButton2 in this._resourceButtons.Values)
						{
							this.RefreshCollectButtonInteract(resourceButton2, resourceButton2.interactable);
						}
					}
					else
					{
						bool flag11 = this._workData.WorkType == 15;
						if (!flag11)
						{
							SingletonObject.getInstance<WorldMapModel>().RequestStopVillagerWorkOnMap(this._blockData.GetLocation(), false);
							TaiwuDomainMethod.Call.SetVillagerCollectResourceWork(this._listenerId, this._selectedCharacterId, this._blockData.AreaId, this._blockData.BlockId, resourceType);
						}
					}
				});
				button.transform.Find("Active").gameObject.SetActive(false);
				Transform transform = button.transform.Find("ActiveBg");
				if (transform != null)
				{
					transform.gameObject.SetActive(false);
				}
				TooltipInvoker tipDisplayer = button.GetComponent<TooltipInvoker>();
				bool flag2 = this._workData.WorkType == 14;
				if (flag2)
				{
					bool flag3 = tipDisplayer != null;
					if (flag3)
					{
						this.RefreshResourceButtonByMigrate(tipDisplayer, resourceType);
					}
					short hasResource = this._blockData.CurrResources.Get((int)resourceType);
					short needResource = GlobalConfig.Instance.VillagerRoleFarmerMigrateMinResource;
					button.interactable = (blockConfig != null && blockConfig.ResourceCollectionType > 0 && hasResource >= needResource);
					foreach (CButtonObsolete resourceButton in this._resourceButtons.Values)
					{
						this.RefreshCollectButtonInteract(resourceButton, resourceButton.interactable);
					}
				}
				else
				{
					bool flag4 = this._workData.WorkType == 15;
					if (!flag4)
					{
						VillagerRoleCharacterDisplayData villagerDisplayData = this._villagerDisplayData;
						int? num = (villagerDisplayData != null) ? new int?(villagerDisplayData.Id) : null;
						int selectedCharacterId = this._selectedCharacterId;
						bool flag5 = num.GetValueOrDefault() == selectedCharacterId & num != null;
						if (flag5)
						{
							this.RefreshCollectButtonInteract(button, blockConfig != null && blockConfig.ResourceCollectionType > 0 && this._villagerDisplayData.RoleTemplateId == 0);
						}
						else
						{
							TaiwuDomainMethod.AsyncCall.GetVillagerRoleCharacterDisplayData(null, this._selectedCharacterId, delegate(int offset, RawDataPool dataPool)
							{
								Serializer.Deserialize(dataPool, offset, ref this._villagerDisplayData);
								this.RefreshCollectButtonInteract(button, blockConfig != null && blockConfig.ResourceCollectionType > 0 && this._villagerDisplayData.RoleTemplateId == 0);
							});
						}
					}
				}
				Transform title = button.transform.Find("Title");
				bool flag6 = title != null;
				if (flag6)
				{
					TextMeshProUGUI label = title.transform.Find("Label").GetComponent<TextMeshProUGUI>();
					TextMeshProUGUI label2 = title.transform.Find("Disable").GetComponent<TextMeshProUGUI>();
					label.text = ((this._workData.WorkType == 15) ? MapBlock.Instance[GameData.Domains.Taiwu.VillagerRole.SharedMethods.DevelopMapBlockTemplateIdList[(int)resourceType]].Name : resourceTypeConfig.Name);
					label2.text = label.text;
				}
				else
				{
					TextMeshProUGUI label3 = button.transform.Find("Enabled").GetComponent<TextMeshProUGUI>();
					TextMeshProUGUI label4 = button.transform.Find("Disabled").GetComponent<TextMeshProUGUI>();
					label3.text = ((this._workData.WorkType == 15) ? MapBlock.Instance[GameData.Domains.Taiwu.VillagerRole.SharedMethods.DevelopMapBlockTemplateIdList[(int)resourceType]].Name : resourceTypeConfig.Name);
					label4.text = label3.text;
				}
				MonoJoint componentInChildren = button.GetComponentInChildren<MonoJoint>(true);
				if (componentInChildren != null)
				{
					componentInChildren.JointSync();
				}
			}
			this.SelectorCharacter.GetComponent<CImage>().sprite = this.SelectorCharacter.CGet<Sprite>("SpriteSelected");
			this.WorkStatusLabelActive.gameObject.SetActive(true);
			this.WorkStatusLabelInActive.gameObject.SetActive(false);
			HashSet<int> characterSet = this._blockData.CharacterSet;
			bool? inPlace = (characterSet != null) ? new bool?(characterSet.Contains(this._selectedCharacterId)) : null;
			bool flag7 = inPlace == null || !inPlace.Value;
			if (flag7)
			{
				this.WorkStatusLabelActive.text = LocalStringManager.Get(LanguageKey.UI_VillagerWork_OnWay);
			}
			CharacterDisplayData displayData;
			bool flag8 = this._charDisplayDataDict.TryGetValue(this._selectedCharacterId, out displayData);
			if (flag8)
			{
				this.NameBack.SetActive(true);
				this.NameLabel.text = NameCenter.GetCharMonasticTitleOrNameByDisplayData(displayData, this._selectedCharacterId == SingletonObject.getInstance<BasicGameData>().TaiwuCharId, false);
				this.Avatar.Refresh(displayData, true);
				this.Avatar.gameObject.SetActive(true);
				this.RefreshAutoDispatchBtn(true);
				this.RefreshAssignRoleButton(true);
			}
			else
			{
				this.RefreshAutoDispatchBtn(false);
				this.RefreshAssignRoleButton(false);
			}
			this.ExchangeButton.ClearAndAddListener(delegate
			{
				Action<int> onExchangeCharacterIdChanged = this._onExchangeCharacterIdChanged;
				if (onExchangeCharacterIdChanged != null)
				{
					onExchangeCharacterIdChanged(this._selectedCharacterId);
				}
			});
			this.SelectButton.gameObject.SetActive(false);
			this.ExchangeButton.gameObject.SetActive(true);
			this.RefreshWorkData();
		}
		else
		{
			this.RefreshWorkButtonDebtCollectionTips();
			foreach (KeyValuePair<sbyte, CButtonObsolete> pair2 in this._resourceButtons)
			{
				CButtonObsolete button2 = pair2.Value;
				button2.transform.Find("Active").gameObject.SetActive(false);
				Transform transform2 = button2.transform.Find("ActiveBg");
				if (transform2 != null)
				{
					transform2.gameObject.SetActive(false);
				}
				button2.interactable = false;
				MonoJoint componentInChildren2 = button2.GetComponentInChildren<MonoJoint>(true);
				if (componentInChildren2 != null)
				{
					componentInChildren2.JointSync();
				}
			}
			this.SelectorCharacter.GetComponent<CImage>().sprite = this.SelectorCharacter.CGet<Sprite>("SpriteUnselected");
			this.WorkStatusLabelActive.gameObject.SetActive(false);
			this.WorkStatusLabelInActive.gameObject.SetActive(true);
			this.WorkStatusLabelInActive.text = LocalStringManager.Get(LanguageKey.UI_VillagerWork_Picking);
			this.SelectButton.ClearAndAddListener(delegate
			{
				Action<int> onExchangeCharacterIdChanged = this._onExchangeCharacterIdChanged;
				if (onExchangeCharacterIdChanged != null)
				{
					onExchangeCharacterIdChanged(-1);
				}
			});
			this.SelectButton.gameObject.SetActive(true);
			this.ExchangeButton.gameObject.SetActive(false);
			this.RefreshAutoDispatchBtn(false);
			this.RefreshAssignRoleButton(false);
		}
		GameObject gameObject = this.WorkButtonIdle.transform.Find("Active").gameObject;
		VillagerWorkData workData = this._workData;
		gameObject.SetActive(workData != null && workData.WorkType == 13);
		GameObject gameObject2 = this.WorkButtonKeepGrave.transform.Find("Active").gameObject;
		workData = this._workData;
		gameObject2.SetActive(workData != null && workData.WorkType == 12);
		GameObject gameObject3 = this.WorkButtonDebtCollection.transform.Find("Active").gameObject;
		workData = this._workData;
		gameObject3.SetActive(workData != null && workData.WorkType == 11);
		GameObject gameObject4 = this.WorkButtonMigrate.transform.Find("Active").gameObject;
		workData = this._workData;
		gameObject4.SetActive(workData != null && workData.WorkType == 14);
	}

	// Token: 0x060039E7 RID: 14823 RVA: 0x001D78E0 File Offset: 0x001D5AE0
	private void RefreshCollectButtonInteract(CButtonObsolete button, bool canInteract)
	{
		Transform title = button.transform.Find("Title");
		TooltipInvoker tips = button.GetComponent<TooltipInvoker>();
		bool flag = title == null;
		if (flag)
		{
			button.interactable = canInteract;
			Behaviour behaviour = tips;
			bool enabled;
			if (this._selectedCharacterId >= 0)
			{
				VillagerWorkData workData = this._workData;
				enabled = ((workData != null && workData.WorkType == 14) || !this._isFarmer);
			}
			else
			{
				enabled = false;
			}
			behaviour.enabled = enabled;
		}
		else
		{
			Transform enable = title.Find("Label");
			Transform disable = title.Find("Disable");
			if (canInteract)
			{
				button.interactable = true;
				enable.gameObject.SetActive(true);
				disable.gameObject.SetActive(false);
			}
			else
			{
				button.interactable = false;
				enable.gameObject.SetActive(false);
				disable.gameObject.SetActive(true);
			}
			Behaviour behaviour2 = tips;
			bool enabled2;
			if (this._selectedCharacterId >= 0)
			{
				VillagerWorkData workData = this._workData;
				enabled2 = ((workData != null && workData.WorkType == 14) || !this._isFarmer);
			}
			else
			{
				enabled2 = false;
			}
			behaviour2.enabled = enabled2;
		}
	}

	// Token: 0x060039E8 RID: 14824 RVA: 0x001D79F6 File Offset: 0x001D5BF6
	private void RefreshWorkButtonAndTips()
	{
		this.RefreshWorkButtonKeepGraveTips();
		this.RefreshWorkButtonIdleTips();
		this.RefreshWorkButtonDebtCollection();
		this.RefreshWorkButtonKeepGrave();
		this.RefreshWorkButtonIdle();
		this.RefreshWorkButtonMigrate();
	}

	// Token: 0x060039E9 RID: 14825 RVA: 0x001D7A24 File Offset: 0x001D5C24
	private void RefreshWorkData()
	{
		this.WorkButtonIdle.interactable = true;
		switch (this._workData.WorkType)
		{
		case 10:
		{
			this.WorkStatusLabelActive.text = LocalStringManager.Get(LanguageKey.UI_VillagerWork_Collect);
			this.SetResourceButtonActive();
			bool flag = this.ResultGo != null;
			if (flag)
			{
				int amount = this._villagerDisplayData.CollectResourceAmount;
				this.ResultGo.SetActive(true);
				this.ResultGo.GetComponentInChildren<TextMeshProUGUI>().SetText(string.Format("+{0}/{1}", amount, LocalStringManager.Get(LanguageKey.LK_Term)), true);
			}
			break;
		}
		case 11:
			this.WorkStatusLabelActive.text = LocalStringManager.Get(LanguageKey.UI_VillagerWork_Tribute);
			break;
		case 12:
			this.WorkStatusLabelActive.text = LocalStringManager.Get(LanguageKey.UI_VillagerWork_Grave);
			break;
		case 13:
			this.WorkStatusLabelActive.text = LocalStringManager.Get(LanguageKey.UI_VillagerWork_Idle);
			break;
		case 14:
			this.WorkStatusLabelActive.text = LocalStringManager.Get(LanguageKey.UI_VillagerWork_Migrate);
			this.SetResourceButtonActive();
			break;
		case 15:
			this.WorkStatusLabelActive.text = LocalStringManager.Get(LanguageKey.UI_VillagerWork_Develop);
			this.SetResourceButtonActive();
			break;
		}
	}

	// Token: 0x060039EA RID: 14826 RVA: 0x001D7B74 File Offset: 0x001D5D74
	private void SetResourceButtonActive()
	{
		CButtonObsolete button;
		bool flag = this._resourceButtons.TryGetValue(this._workData.ResourceType, out button);
		if (flag)
		{
			button.transform.Find("Active").gameObject.SetActive(true);
			Transform transform = button.transform.Find("ActiveBg");
			if (transform != null)
			{
				transform.gameObject.SetActive(true);
			}
		}
	}

	// Token: 0x060039EB RID: 14827 RVA: 0x001D7BE0 File Offset: 0x001D5DE0
	private void RefreshResourceButtonByMigrate(TooltipInvoker tipDisplayer, sbyte resourceType)
	{
		TaiwuDomainMethod.AsyncCall.GetVillagerRoleCharacterDisplayData(null, this._selectedCharacterId, delegate(int offset, RawDataPool dataPool)
		{
			Serializer.Deserialize(dataPool, offset, ref this._villagerDisplayData);
			short hasResource = this._blockData.CurrResources.Get((int)resourceType);
			short needResource = GlobalConfig.Instance.VillagerRoleFarmerMigrateMinResource;
			bool isEnough = hasResource >= needResource;
			ResourceTypeItem resourceTypeConfig = Config.ResourceType.Instance[resourceType];
			string hasResourceText = hasResource.ToString().SetColor(isEnough ? "brightblue" : "brightred");
			TaiwuDomainMethod.AsyncCall.GetVillagerFarmerMigrateResourceSuccessRateBonus(null, this._selectedCharacterId, delegate(int offset, RawDataPool dataPool)
			{
				int lineCount = 0;
				ValueTuple<int, int, int> bonuses = default(ValueTuple<int, int, int>);
				Serializer.Deserialize(dataPool, offset, ref bonuses);
				tipDisplayer.Type = TipType.GeneralLines;
				TooltipInvoker tipDisplayer2 = tipDisplayer;
				if (tipDisplayer2.RuntimeParam == null)
				{
					tipDisplayer2.RuntimeParam = new ArgumentBox();
				}
				tipDisplayer.RuntimeParam.SetObject(string.Format("LineData{0}", ++lineCount), new GeneralLineData(5, new List<string>
				{
					string.Format("<SpName=mousetip_ziyuan_{0}>{1}{2}{3}/{4}", new object[]
					{
						resourceType,
						resourceTypeConfig.Name,
						LocalStringManager.Get(LanguageKey.LK_Colon_Symbol),
						hasResourceText,
						needResource.ToString().SetColor("pinkyellow")
					})
				}, null));
				int successRate = VillagerWorkPanel.CalcFarmerMigrateSuccessRateBaseValue(this._villagerDisplayData.Personalities);
				successRate *= bonuses.Item3;
				string successRateText = isEnough ? string.Format("{0}%", successRate).SetColor("pinkyellow") : "/";
				bool flag = this._currentMigrateResourceType == resourceType;
				if (flag)
				{
					string bonusSuccessRate = string.Format("+{0}%", bonuses.Item1).SetColor("brightblue");
					this._extraMigrateRate = bonuses;
					successRateText += bonusSuccessRate;
				}
				tipDisplayer.RuntimeParam.SetObject(string.Format("LineData{0}", ++lineCount), new GeneralLineData(5, new List<string>
				{
					LocalStringManager.Get(LanguageKey.LK_VillagerRole_EffectTip_Farmer_MigrateResource) + LocalStringManager.Get(LanguageKey.LK_Colon_Symbol) + successRateText
				}, null));
				tipDisplayer.RuntimeParam.SetObject(string.Format("LineData{0}", ++lineCount), new GeneralLineData(3, new List<string>
				{
					LocalStringManager.Get(LanguageKey.LK_Migrate_Hint) ?? ""
				}, null));
				bool flag2 = bonuses.Item3 > 0;
				if (flag2)
				{
					tipDisplayer.RuntimeParam.SetObject(string.Format("LineData{0}", ++lineCount), new GeneralLineData(3, new List<string>
					{
						LocalStringManager.Get(LanguageKey.LK_Migrate_ExtraHint) ?? ""
					}, null));
				}
				StringBuilder sb = EasyPool.Get<StringBuilder>();
				sb.Clear();
				for (int i = 0; i < resourceTypeConfig.PossibleBuildingCoreItem.Length; i++)
				{
					bool flag3 = i > 0;
					if (flag3)
					{
						sb.Append(LocalStringManager.Get(LanguageKey.LK_Split_Symbol));
					}
					MiscItem miscConfig = Misc.Instance[resourceTypeConfig.PossibleBuildingCoreItem[i]];
					sb.Append(miscConfig.Name.SetGradeColor((int)miscConfig.Grade));
				}
				bool flag4 = bonuses.Item2 > 0;
				if (flag4)
				{
					foreach (short matId in resourceTypeConfig.PossibleUpgradedBuildingCoreItem)
					{
						MiscItem miscConfig2 = Misc.Instance[matId];
						sb.Append(LocalStringManager.Get(LanguageKey.LK_Split_Symbol));
						sb.Append(miscConfig2.Name.SetGradeColor((int)miscConfig2.Grade));
					}
					string upgradeString = (bonuses.Item2.ToString() + "%").SetColor("pinkyellow");
					tipDisplayer.RuntimeParam.SetObject(string.Format("LineData{0}", ++lineCount), new GeneralLineData(5, new List<string>
					{
						LocalStringManager.Get(LanguageKey.LK_VillagerRole_Farmer_UpgradeSuccessRate) + LocalStringManager.Get(LanguageKey.LK_Colon_Symbol) + upgradeString
					}, null));
				}
				tipDisplayer.RuntimeParam.SetObject(string.Format("LineData{0}", ++lineCount), new GeneralLineData(5, new List<string>
				{
					string.Format("{0}{1}{2}", LocalStringManager.Get(LanguageKey.LK_VillagerWork_MigrateTips_PossibleBuildingCoreItem), LocalStringManager.Get(LanguageKey.LK_Colon_Symbol), sb)
				}, null));
				bool flag5 = !isEnough;
				if (flag5)
				{
					tipDisplayer.RuntimeParam.SetObject(string.Format("LineData{0}", ++lineCount), new GeneralLineData
					{
						Type = 4,
						PreferredHeight = 10f
					});
					tipDisplayer.RuntimeParam.SetObject(string.Format("LineData{0}", ++lineCount), new GeneralLineData(3, new List<string>
					{
						"<color=#brightred>" + LocalStringManager.Get(LanguageKey.LK_VillagerWork_Cannot_Migrate_ResourceNotEnough) + "</color>"
					}, null));
				}
				tipDisplayer.RuntimeParam.Set("Title", LocalStringManager.Get(LanguageKey.LK_Migrate));
				tipDisplayer.RuntimeParam.Set("LineCount", lineCount);
			});
		});
	}

	// Token: 0x060039EC RID: 14828 RVA: 0x001D7C24 File Offset: 0x001D5E24
	public unsafe static int CalcFarmerMigrateSuccessRateBaseValue(Personalities personalities)
	{
		int templateId = 2;
		VillagerRoleFormulaItem formula = VillagerRoleFormula.Instance[templateId];
		sbyte personalityType = VillagerRole.Instance[0].PersonalityType;
		return Math.Clamp(formula.Calculate((int)(*personalities[(int)personalityType])), 0, 100);
	}

	// Token: 0x060039ED RID: 14829 RVA: 0x001D7C6C File Offset: 0x001D5E6C
	private void RefreshWorkButtonMigrate()
	{
		this.WorkButtonMigrate.interactable = false;
		bool flag = this._selectedCharacterId >= 0;
		if (flag)
		{
			sbyte firstEnoughResourceType = -1;
			bool hasAnyResourceEnough = false;
			foreach (KeyValuePair<sbyte, CButtonObsolete> pair in this._resourceButtons)
			{
				short hasResource = this._blockData.CurrResources.Get((int)pair.Key);
				short needResource = GlobalConfig.Instance.VillagerRoleFarmerMigrateMinResource;
				bool isEnough = hasResource >= needResource;
				bool flag2 = isEnough;
				if (flag2)
				{
					hasAnyResourceEnough = true;
					firstEnoughResourceType = pair.Key;
					break;
				}
			}
			bool flag3 = this._notBigSize && this._notDeveloped && hasAnyResourceEnough && this._isFarmer;
			if (flag3)
			{
				this.WorkButtonMigrate.interactable = true;
				this.WorkButtonMigrate.ClearAndAddListener(delegate
				{
					bool flag4 = this._workData != null && this._workData.WorkType == 14;
					if (flag4)
					{
						Debug.LogWarning("Already Migrating");
					}
					else
					{
						TaiwuDomainMethod.Call.SetVillagerMigrateWork(this._listenerId, this._selectedCharacterId, this._blockData.AreaId, this._blockData.BlockId, firstEnoughResourceType);
					}
				});
			}
			this.RefreshWorkButtonMigrateTips();
		}
		else
		{
			this._isFarmer = false;
			this.RefreshWorkButtonMigrateTips();
		}
	}

	// Token: 0x060039EE RID: 14830 RVA: 0x001D7DA4 File Offset: 0x001D5FA4
	private void RefreshWorkButtonMigrateTips()
	{
		bool hasWorker = this._selectedCharacterId >= 0 && this._workData != null;
		bool hasAnyResourceEnough = false;
		foreach (KeyValuePair<sbyte, CButtonObsolete> pair in this._resourceButtons)
		{
			short hasResource = this._blockData.CurrResources.Get((int)pair.Key);
			short needResource = GlobalConfig.Instance.VillagerRoleFarmerMigrateMinResource;
			bool isEnough = hasResource >= needResource;
			bool flag = isEnough;
			if (flag)
			{
				hasAnyResourceEnough = true;
				break;
			}
		}
		int lineCount = 0;
		TooltipInvoker tips = this.WorkButtonMigrate.gameObject.GetOrAddComponent<TooltipInvoker>();
		tips.Type = TipType.GeneralLines;
		TooltipInvoker tooltipInvoker = tips;
		if (tooltipInvoker.RuntimeParam == null)
		{
			tooltipInvoker.RuntimeParam = new ArgumentBox();
		}
		tips.RuntimeParam.SetObject(string.Format("LineData{0}", ++lineCount), new GeneralLineData
		{
			Type = 11,
			Args = new List<string>
			{
				LocalStringManager.Get(LanguageKey.LK_VillagerWork_Tips_Migrate)
			}
		});
		tips.RuntimeParam.SetObject(string.Format("LineData{0}", ++lineCount), new GeneralLineData
		{
			Type = 4,
			PreferredHeight = 10f
		});
		tips.RuntimeParam.SetObject(string.Format("LineData{0}", ++lineCount), new GeneralLineData
		{
			Type = 11,
			Args = new List<string>
			{
				LocalStringManager.Get(LanguageKey.LK_VillagerWork_Tips_MigrateWarning)
			}
		});
		bool flag2 = !hasWorker || !this._notBigSize || !this._notDeveloped || !hasAnyResourceEnough || !this._isFarmer;
		if (flag2)
		{
			tips.RuntimeParam.SetObject(string.Format("LineData{0}", ++lineCount), new GeneralLineData
			{
				Type = 4,
				PreferredHeight = 10f
			});
			StringBuilder sb = new StringBuilder();
			bool flag3 = !hasWorker;
			if (flag3)
			{
				sb.AppendLine(LocalStringManager.Get(LanguageKey.LK_VillagerWork_Tips_NoWorker));
			}
			bool flag4 = !this._notBigSize;
			if (flag4)
			{
				sb.AppendLine(LocalStringManager.Get(LanguageKey.LK_VillagerRole_Migrate_Tips_LargeBlock));
			}
			bool flag5 = !this._notDeveloped;
			if (flag5)
			{
				sb.AppendLine(LocalStringManager.Get(LanguageKey.LK_VillagerWork_Cannot_Migrate_NeedNotDeveloped));
			}
			bool flag6 = !hasAnyResourceEnough;
			if (flag6)
			{
				sb.AppendLine(LocalStringManager.Get(LanguageKey.LK_VillagerWork_Cannot_Migrate_ResourceNotEnough));
			}
			bool flag7 = !this._isFarmer;
			if (flag7)
			{
				sb.AppendLine(LocalStringManager.Get(LanguageKey.LK_VillagerWork_Cannot_Migrate_NotFarmer));
			}
			tips.RuntimeParam.SetObject(string.Format("LineData{0}", ++lineCount), new GeneralLineData
			{
				Type = 11,
				Args = new List<string>
				{
					sb.ToString()
				}
			});
		}
		tips.RuntimeParam.Set("Title", LocalStringManager.Get(LanguageKey.LK_WorkType_14));
		tips.RuntimeParam.Set("LineCount", lineCount);
		tips.RuntimeParam.Set("DisableRaycastTarget", true);
		tips.Refresh(false, -1);
	}

	// Token: 0x060039EF RID: 14831 RVA: 0x001D80E0 File Offset: 0x001D62E0
	private void RefreshWorkButtonKeepGraveTips()
	{
		HashSet<int> graveSet = this._blockData.GraveSet;
		bool hasGrave = graveSet != null && graveSet.Count > 0;
		bool hasWorker = this._selectedCharacterId >= 0 && this._workData != null;
		int lineCount = 0;
		TooltipInvoker tips = this.WorkButtonKeepGrave.gameObject.GetOrAddComponent<TooltipInvoker>();
		tips.Type = TipType.GeneralLines;
		TooltipInvoker tooltipInvoker = tips;
		if (tooltipInvoker.RuntimeParam == null)
		{
			tooltipInvoker.RuntimeParam = new ArgumentBox();
		}
		tips.RuntimeParam.SetObject(string.Format("LineData{0}", ++lineCount), new GeneralLineData
		{
			Type = 11,
			Args = new List<string>
			{
				LocalStringManager.Get(LanguageKey.LK_VillagerWork_Tips_KeepGrave)
			}
		});
		bool flag = !hasWorker || !hasGrave;
		if (flag)
		{
			tips.RuntimeParam.SetObject(string.Format("LineData{0}", ++lineCount), new GeneralLineData
			{
				Type = 4,
				PreferredHeight = 10f
			});
			StringBuilder sb = new StringBuilder();
			bool flag2 = !hasWorker;
			if (flag2)
			{
				sb.AppendLine(LocalStringManager.Get(LanguageKey.LK_VillagerWork_Tips_NoWorker));
			}
			bool flag3 = !hasGrave;
			if (flag3)
			{
				sb.AppendLine(LocalStringManager.Get(LanguageKey.LK_VillagerWork_Tips_NoGrave));
			}
			tips.RuntimeParam.SetObject(string.Format("LineData{0}", ++lineCount), new GeneralLineData
			{
				Type = 11,
				Args = new List<string>
				{
					sb.ToString()
				}
			});
		}
		tips.RuntimeParam.Set("Title", LocalStringManager.Get(LanguageKey.LK_WorkType_12));
		tips.RuntimeParam.Set("LineCount", lineCount);
		tips.RuntimeParam.Set("DisableRaycastTarget", true);
		tips.Refresh(false, -1);
	}

	// Token: 0x060039F0 RID: 14832 RVA: 0x001D82C4 File Offset: 0x001D64C4
	private void RefreshWorkButtonDebtCollectionTips()
	{
		bool hasWorker = this._selectedCharacterId >= 0 && this._workData != null;
		int lineCount = 0;
		TooltipInvoker tips = this.WorkButtonDebtCollection.gameObject.GetOrAddComponent<TooltipInvoker>();
		tips.Type = TipType.GeneralLines;
		TooltipInvoker tooltipInvoker = tips;
		if (tooltipInvoker.RuntimeParam == null)
		{
			tooltipInvoker.RuntimeParam = new ArgumentBox();
		}
		tips.RuntimeParam.SetObject(string.Format("LineData{0}", ++lineCount), new GeneralLineData
		{
			Type = 11,
			Args = new List<string>
			{
				LocalStringManager.Get(LanguageKey.LK_VillagerWork_Tips_DebtCollection)
			}
		});
		bool flag = !hasWorker || !this._hasDebt;
		if (flag)
		{
			tips.RuntimeParam.SetObject(string.Format("LineData{0}", ++lineCount), new GeneralLineData
			{
				Type = 4,
				PreferredHeight = 10f
			});
			StringBuilder sb = new StringBuilder();
			bool flag2 = !hasWorker;
			if (flag2)
			{
				sb.AppendLine(LocalStringManager.Get(LanguageKey.LK_VillagerWork_Tips_NoWorker));
			}
			bool flag3 = !this._hasDebt;
			if (flag3)
			{
				sb.AppendLine(LocalStringManager.Get(LanguageKey.LK_VillagerWork_Tips_NoNest));
			}
			tips.RuntimeParam.SetObject(string.Format("LineData{0}", ++lineCount), new GeneralLineData
			{
				Type = 11,
				Args = new List<string>
				{
					sb.ToString()
				}
			});
		}
		tips.RuntimeParam.Set("Title", LocalStringManager.Get(LanguageKey.LK_Bottom_Work_Supervise));
		tips.RuntimeParam.Set("LineCount", lineCount);
		tips.RuntimeParam.Set("DisableRaycastTarget", true);
		tips.Refresh(false, -1);
	}

	// Token: 0x060039F1 RID: 14833 RVA: 0x001D8488 File Offset: 0x001D6688
	private void RefreshWorkButtonIdleTips()
	{
		int lineCount = 0;
		bool hasWorker = this._selectedCharacterId >= 0 && this._workData != null;
		TooltipInvoker tips = this.WorkButtonIdle.gameObject.GetOrAddComponent<TooltipInvoker>();
		tips.Type = TipType.GeneralLines;
		TooltipInvoker tooltipInvoker = tips;
		if (tooltipInvoker.RuntimeParam == null)
		{
			tooltipInvoker.RuntimeParam = new ArgumentBox();
		}
		tips.RuntimeParam.SetObject(string.Format("LineData{0}", ++lineCount), new GeneralLineData
		{
			Type = 11,
			Args = new List<string>
			{
				LocalStringManager.Get(LanguageKey.LK_VillagerWork_Tips_Idle)
			}
		});
		bool flag = !hasWorker;
		if (flag)
		{
			tips.RuntimeParam.SetObject(string.Format("LineData{0}", ++lineCount), new GeneralLineData
			{
				Type = 4,
				PreferredHeight = 10f
			});
			tips.RuntimeParam.SetObject(string.Format("LineData{0}", ++lineCount), new GeneralLineData
			{
				Type = 11,
				Args = new List<string>
				{
					LocalStringManager.Get(LanguageKey.LK_VillagerWork_Tips_NoWorker)
				}
			});
		}
		tips.RuntimeParam.Set("Title", LocalStringManager.Get(LanguageKey.LK_WorkType_13));
		tips.RuntimeParam.Set("LineCount", lineCount);
		tips.RuntimeParam.Set("DisableRaycastTarget", true);
		tips.Refresh(false, -1);
	}

	// Token: 0x060039F2 RID: 14834 RVA: 0x001D8600 File Offset: 0x001D6800
	private void RefreshAutoDispatchBtn(bool curBlockHaveChar)
	{
		GameObject btnAutoDispatch = base.CGet<CButtonObsolete>("BtnDispatch").gameObject;
		bool flag = !this.ShowAutoDispatchButton;
		if (flag)
		{
			btnAutoDispatch.gameObject.SetActive(false);
		}
		else
		{
			btnAutoDispatch.SetActive(true);
			TaiwuDomainMethod.AsyncCall.GetAllVillagersAvailableForWork(null, true, delegate(int offset, RawDataPool dataPool)
			{
				bool flag2 = btnAutoDispatch == null;
				if (!flag2)
				{
					List<int> charIdList = new List<int>();
					Serializer.Deserialize(dataPool, offset, ref charIdList);
					bool canAutoDispatch = charIdList.Count > 0 && !curBlockHaveChar;
					btnAutoDispatch.GetComponent<CButtonObsolete>().interactable = canAutoDispatch;
					TooltipInvoker mouseTipDisplayer = btnAutoDispatch.GetComponent<TooltipInvoker>();
					mouseTipDisplayer.enabled = !canAutoDispatch;
					bool flag3 = !canAutoDispatch;
					if (flag3)
					{
						TooltipInvoker tooltipInvoker = mouseTipDisplayer;
						if (tooltipInvoker.RuntimeParam == null)
						{
							tooltipInvoker.RuntimeParam = new ArgumentBox();
						}
						mouseTipDisplayer.RuntimeParam.Set("arg0", curBlockHaveChar ? LocalStringManager.Get(LanguageKey.LK_Building_QuickArrangeTip_Disable_AlreadyHere) : LocalStringManager.Get(LanguageKey.LK_UI_VillagerWork_DispatchButton_Tips));
					}
					btnAutoDispatch.GetComponent<PointerTrigger>().enabled = canAutoDispatch;
				}
			});
		}
	}

	// Token: 0x060039F3 RID: 14835 RVA: 0x001D8674 File Offset: 0x001D6874
	private void RefreshAssignRoleButton(bool curBlockHaveChar)
	{
		CButtonObsolete assignRoleButton;
		bool flag = !this.CTryGet<CButtonObsolete>("BtnAssignRole", out assignRoleButton);
		if (!flag)
		{
			bool flag2 = !this.ShowAssignRoleButton;
			if (flag2)
			{
				assignRoleButton.gameObject.SetActive(false);
			}
			else
			{
				assignRoleButton.gameObject.SetActive(true);
				bool flag3 = !curBlockHaveChar;
				if (flag3)
				{
					assignRoleButton.interactable = false;
					assignRoleButton.transform.Find("Image").GetComponent<CImage>().SetSprite("bottom_icon_identity_0", false, null);
					assignRoleButton.transform.Find("ImageOff").GetComponent<CImage>().SetSprite("bottom_icon_identity_1", false, null);
				}
				else
				{
					TooltipInvoker tip = assignRoleButton.GetComponent<TooltipInvoker>();
					TooltipInvoker tooltipInvoker = tip;
					if (tooltipInvoker.RuntimeParam == null)
					{
						tooltipInvoker.RuntimeParam = new ArgumentBox();
					}
					tip.RuntimeParam.Set("arg0", LocalStringManager.Get(this._isFarmer ? LanguageKey.LK_Building_CancelRole : LanguageKey.LK_Building_AssignRole));
					assignRoleButton.interactable = true;
					TaiwuDomainMethod.AsyncCall.GetVillagerRoleCharacterDisplayData(null, this._selectedCharacterId, delegate(int offset, RawDataPool dataPool)
					{
						VillagerRoleCharacterDisplayData displayData = new VillagerRoleCharacterDisplayData();
						Serializer.Deserialize(dataPool, offset, ref displayData);
						short roleTemplateId = displayData.RoleTemplateId;
						bool isFarmer = roleTemplateId == 0;
						assignRoleButton.transform.Find("Image").GetComponent<CImage>().SetSprite(isFarmer ? "bottom_icon_dismissal_0" : "bottom_icon_identity_0", false, null);
						assignRoleButton.transform.Find("ImageOff").GetComponent<CImage>().SetSprite(isFarmer ? "bottom_icon_dismissal_1" : "bottom_icon_identity_1", false, null);
						assignRoleButton.ClearAndAddListener(delegate
						{
							int targetRole = isFarmer ? -1 : 0;
							VillagerRoleUtils.ConfirmAndAssignRole(this._selectedCharacterId, (short)targetRole, null, null, null);
						});
					});
				}
			}
		}
	}

	// Token: 0x060039F4 RID: 14836 RVA: 0x001D87C4 File Offset: 0x001D69C4
	private void RefreshWorkButtonIdle()
	{
		bool interactable = this.WorkButtonIdle.interactable;
		if (interactable)
		{
			this.WorkButtonIdle.ClearAndAddListener(delegate
			{
				SingletonObject.getInstance<WorldMapModel>().RequestStopVillagerWorkOnMap(this._blockData.GetLocation(), false);
				TaiwuDomainMethod.Call.SetVillagerIdleWork(this._listenerId, this._selectedCharacterId, this._blockData.AreaId, this._blockData.BlockId);
			});
		}
	}

	// Token: 0x060039F5 RID: 14837 RVA: 0x001D87FC File Offset: 0x001D69FC
	private void RefreshWorkButtonKeepGrave()
	{
		HashSet<int> graveSet = this._blockData.GraveSet;
		this.WorkButtonKeepGrave.interactable = false;
		bool flag = graveSet != null && graveSet.Count > 0 && this._selectedCharacterId >= 0;
		if (flag)
		{
			this.WorkButtonKeepGrave.interactable = true;
			Action <>9__2;
			this.WorkButtonKeepGrave.ClearAndAddListener(delegate
			{
				List<int> list = graveSet.ToList<int>();
				SelectCharacterCallback onSelectGrave = new SelectCharacterCallback(base.<RefreshWorkButtonKeepGrave>g__OnSelectChar|3);
				Action cancelCallback;
				if ((cancelCallback = <>9__2) == null)
				{
					cancelCallback = (<>9__2 = delegate()
					{
						base.<RefreshWorkButtonKeepGrave>g__OnSelectGrave|1(-1);
					});
				}
				SelectCharacterConfigHelper.ShowSelctGraveCharacter(list, onSelectGrave, cancelCallback);
			});
		}
	}

	// Token: 0x060039F6 RID: 14838 RVA: 0x001D8885 File Offset: 0x001D6A85
	private void RefreshWorkButtonDebtCollection()
	{
		this.WorkButtonDebtCollection.interactable = false;
	}

	// Token: 0x060039F7 RID: 14839 RVA: 0x001D8898 File Offset: 0x001D6A98
	private void RefreshFunctionHolder()
	{
		bool flag = !this.Names.Contains("FunctionHolder");
		if (!flag)
		{
			base.CGet<GameObject>("FunctionHolder").SetActive(this._selectedCharacterId != -1);
			bool flag2 = this._selectedCharacterId == -1;
			if (!flag2)
			{
				base.CGet<CButtonObsolete>("ShowMainCharacterMenu").ClearAndAddListener(delegate
				{
					ArgumentBox argBox = EasyPool.Get<ArgumentBox>();
					argBox.Set("CharacterId", this._selectedCharacterId);
					UIElement.CharacterMenu.SetOnInitArgs(argBox);
					UIManager.Instance.ShowUI(UIElement.CharacterMenu, true);
				});
				CToggleObsolete lockToggle = base.CGet<CToggleObsolete>("LockToggle");
				lockToggle.onValueChanged.RemoveAllListeners();
				lockToggle.isOn = !this._unlockedWorkingList.Contains(this._selectedCharacterId);
				lockToggle.onValueChanged.AddListener(delegate(bool isOn)
				{
					BuildingDomainMethod.Call.SetUnlockedWorkingVillagers(this._selectedCharacterId, !isOn);
				});
			}
		}
	}

	// Token: 0x170005E5 RID: 1509
	// (get) Token: 0x060039F8 RID: 14840 RVA: 0x001D8956 File Offset: 0x001D6B56
	private GameObject DevelopFocusHolder
	{
		get
		{
			return base.CGet<GameObject>("DevelopFocusHolder");
		}
	}

	// Token: 0x170005E6 RID: 1510
	// (get) Token: 0x060039F9 RID: 14841 RVA: 0x001D8963 File Offset: 0x001D6B63
	private Refers DevelopFocusHolderRefers
	{
		get
		{
			return this.DevelopFocusHolder.GetComponent<Refers>();
		}
	}

	// Token: 0x170005E7 RID: 1511
	// (get) Token: 0x060039FA RID: 14842 RVA: 0x001D8970 File Offset: 0x001D6B70
	private CButtonObsolete DevelopFocusMask
	{
		get
		{
			return base.CGet<CButtonObsolete>("DevelopFocusMask");
		}
	}

	// Token: 0x170005E8 RID: 1512
	// (get) Token: 0x060039FB RID: 14843 RVA: 0x001D897D File Offset: 0x001D6B7D
	private CToggleGroupObsolete DevelopFocusToggleGroup
	{
		get
		{
			return base.CGet<CToggleGroupObsolete>("DevelopFocusToggleGroup");
		}
	}

	// Token: 0x060039FC RID: 14844 RVA: 0x001D898C File Offset: 0x001D6B8C
	private void RefreshWorkButtonDevelop()
	{
		this.WorkButtonDevelop.gameObject.SetActive(false);
		this.DevelopFocusToggleGroup.InitPreOnToggle(-1);
		this.DevelopFocusToggleGroup.OnActiveToggleChange = delegate(CToggleObsolete togNew, CToggleObsolete togOld)
		{
			this._developSelectedBlockIndex = (short)togNew.Key;
		};
		this.WorkButtonDevelop.interactable = false;
		bool flag = this._selectedCharacterId >= 0;
		if (flag)
		{
			TaiwuDomainMethod.AsyncCall.GetVillagerRoleCharacterDisplayData(null, this._selectedCharacterId, delegate(int offset, RawDataPool dataPool)
			{
				Serializer.Deserialize(dataPool, offset, ref this._villagerDisplayData);
				this._isFarmer = (this._villagerDisplayData.RoleTemplateId == 0);
				ValueTuple<bool, sbyte> valueTuple = this.CalcResourceCanDevelop(this._villagerDisplayData);
				bool hasAnyResourceEnough = valueTuple.Item1;
				sbyte firstEnoughResourceType = valueTuple.Item2;
				this._developSelectedResourceType = firstEnoughResourceType;
				bool canDevelop = GameData.Domains.Taiwu.VillagerRole.SharedMethods.IsMapBlockCanDevelop(this._blockData.TemplateId);
				bool flag2 = canDevelop && this._notDeveloped && this._isFarmer && hasAnyResourceEnough;
				if (flag2)
				{
					this.WorkButtonDevelop.interactable = true;
					this.WorkButtonDevelop.ClearAndAddListener(delegate
					{
						bool flag3 = this._workData.WorkType != 15;
						if (flag3)
						{
							TaiwuDomainMethod.Call.SetVillagerDevelopWork(this._listenerId, this._selectedCharacterId, this._blockData.AreaId, this._blockData.BlockId, this._developSelectedResourceType, this._developSelectedBlockIndex);
						}
					});
				}
				this.RefreshWorkButtonDevelopTips(this._villagerDisplayData);
			});
		}
		else
		{
			this._isFarmer = false;
			this.RefreshWorkButtonDevelopTips(null);
		}
	}

	// Token: 0x060039FD RID: 14845 RVA: 0x001D8A1C File Offset: 0x001D6C1C
	private void RefreshResourceButtonByDevelop(TooltipInvoker tipDisplayer, sbyte resourceType)
	{
		TaiwuDomainMethod.AsyncCall.GetVillagerRoleCharacterDisplayData(null, this._selectedCharacterId, delegate(int offset, RawDataPool dataPool)
		{
			Serializer.Deserialize(dataPool, offset, ref this._villagerDisplayData);
			ResourceTypeItem resourceTypeConfig = Config.ResourceType.Instance[resourceType];
			short maxResource = this._blockData.MaxResources.Get((int)resourceType);
			short needMaxResource = GlobalConfig.Instance.VillagerRoleFarmerMigrateMinResource;
			int taiwuResourceCount = this.BuildingModel.GetResourceCount(resourceType);
			int needResource = GameData.Domains.Taiwu.VillagerRole.SharedMethods.GetDevelopNeedResource(this._villagerDisplayData.Personalities);
			tipDisplayer.Type = TipType.GeneralLines;
			TooltipInvoker tipDisplayer2 = tipDisplayer;
			if (tipDisplayer2.RuntimeParam == null)
			{
				tipDisplayer2.RuntimeParam = new ArgumentBox();
			}
			tipDisplayer.RuntimeParam.Set("Title", LocalStringManager.Get(LanguageKey.LK_WorkType_15));
			int lineCount = 0;
			string desc = MapBlock.Instance[GameData.Domains.Taiwu.VillagerRole.SharedMethods.DevelopMapBlockTemplateIdList[(int)resourceType]].Desc;
			tipDisplayer.RuntimeParam.SetObject(string.Format("LineData{0}", ++lineCount), new GeneralLineData
			{
				Type = 11,
				Args = new List<string>
				{
					desc
				}
			});
			tipDisplayer.RuntimeParam.SetObject(string.Format("LineData{0}", ++lineCount), new GeneralLineData
			{
				Type = 4,
				PreferredHeight = 10f
			});
			tipDisplayer.RuntimeParam.SetObject(string.Format("LineData{0}", ++lineCount), new GeneralLineData(1, new List<string>
			{
				LocalStringManager.GetFormat(LanguageKey.LK_Brackets_Symbol, LocalStringManager.Get(LanguageKey.LK_Resource_Cost))
			}, null));
			string hasResourceText = taiwuResourceCount.ToString().SetColor((taiwuResourceCount >= needResource) ? "brightblue" : "brightred");
			tipDisplayer.RuntimeParam.SetObject(string.Format("LineData{0}", ++lineCount), new GeneralLineData(3, new List<string>
			{
				"   -" + string.Format("<SpName=mousetip_ziyuan_{0}>{1}{2}{3}/{4}", new object[]
				{
					resourceType,
					resourceTypeConfig.Name,
					LocalStringManager.Get(LanguageKey.LK_Colon_Symbol),
					hasResourceText,
					needResource.ToString().SetColor("pinkyellow")
				})
			}, null));
			StringBuilder sb = new StringBuilder();
			bool flag = taiwuResourceCount < needResource;
			if (flag)
			{
				sb.AppendLine(LocalStringManager.Get(LanguageKey.LK_VillagerWork_Cannot_Develop_TaiwuResourceNotEnough));
			}
			bool flag2 = maxResource < needMaxResource;
			if (flag2)
			{
				sb.AppendLine(LocalStringManager.Get(LanguageKey.LK_VillagerWork_Cannot_Develop_MaxResourceNotEnough));
			}
			tipDisplayer.RuntimeParam.SetObject(string.Format("LineData{0}", ++lineCount), new GeneralLineData
			{
				Type = 11,
				Args = new List<string>
				{
					sb.ToString()
				}
			});
			tipDisplayer.RuntimeParam.Set("LineCount", lineCount);
		});
	}

	// Token: 0x060039FE RID: 14846 RVA: 0x001D8A60 File Offset: 0x001D6C60
	private void RefreshWorkButtonDevelopTips(VillagerRoleCharacterDisplayData displayData)
	{
		bool hasWorker = this._selectedCharacterId >= 0 && this._workData != null;
		bool hasAnyResourceEnough = false;
		bool flag = displayData != null;
		if (flag)
		{
			hasAnyResourceEnough = this.CalcResourceCanDevelop(displayData).Item1;
		}
		int lineCount = 0;
		TooltipInvoker tips = this.WorkButtonDevelop.gameObject.GetOrAddComponent<TooltipInvoker>();
		tips.Type = TipType.GeneralLines;
		TooltipInvoker tooltipInvoker = tips;
		if (tooltipInvoker.RuntimeParam == null)
		{
			tooltipInvoker.RuntimeParam = new ArgumentBox();
		}
		tips.RuntimeParam.SetObject(string.Format("LineData{0}", ++lineCount), new GeneralLineData
		{
			Type = 11,
			Args = new List<string>
			{
				LocalStringManager.Get(LanguageKey.LK_VillagerWork_Tips_Develop)
			}
		});
		tips.RuntimeParam.SetObject(string.Format("LineData{0}", ++lineCount), new GeneralLineData
		{
			Type = 4,
			PreferredHeight = 10f
		});
		bool canDevelop = GameData.Domains.Taiwu.VillagerRole.SharedMethods.IsMapBlockCanDevelop(this._blockData.TemplateId);
		bool flag2 = !hasWorker || !canDevelop || !this._notDeveloped || !hasAnyResourceEnough || !this._isFarmer;
		if (flag2)
		{
			tips.RuntimeParam.SetObject(string.Format("LineData{0}", ++lineCount), new GeneralLineData
			{
				Type = 4,
				PreferredHeight = 10f
			});
			StringBuilder sb = new StringBuilder();
			bool flag3 = !hasWorker;
			if (flag3)
			{
				sb.AppendLine(LocalStringManager.Get(LanguageKey.LK_VillagerWork_Tips_NoWorker));
			}
			bool flag4 = !canDevelop;
			if (flag4)
			{
				sb.AppendLine(LocalStringManager.Get(LanguageKey.LK_VillagerWork_Cannot_Develop_TypeNotMeet));
			}
			bool flag5 = !this._notDeveloped;
			if (flag5)
			{
				sb.AppendLine(LocalStringManager.Get(LanguageKey.LK_VillagerWork_Cannot_Develop_NeedNotDeveloped));
			}
			bool flag6 = !hasAnyResourceEnough;
			if (flag6)
			{
				sb.AppendLine(LocalStringManager.Get(LanguageKey.LK_VillagerWork_Cannot_Develop_TaiwuResourceNotEnough));
				sb.AppendLine(LocalStringManager.Get(LanguageKey.LK_VillagerWork_Cannot_Develop_MaxResourceNotEnough));
			}
			bool flag7 = !this._isFarmer;
			if (flag7)
			{
				sb.AppendLine(LocalStringManager.Get(LanguageKey.LK_VillagerWork_Cannot_Migrate_NotFarmer));
			}
			tips.RuntimeParam.SetObject(string.Format("LineData{0}", ++lineCount), new GeneralLineData
			{
				Type = 11,
				Args = new List<string>
				{
					sb.ToString()
				}
			});
		}
		tips.RuntimeParam.Set("Title", LocalStringManager.Get(LanguageKey.LK_WorkType_15));
		tips.RuntimeParam.Set("LineCount", lineCount);
		tips.RuntimeParam.Set("DisableRaycastTarget", true);
		tips.Refresh(false, -1);
	}

	// Token: 0x060039FF RID: 14847 RVA: 0x001D8D08 File Offset: 0x001D6F08
	private ValueTuple<bool, sbyte> CalcResourceCanDevelop(VillagerRoleCharacterDisplayData displayData)
	{
		sbyte firstEnoughResourceType = -1;
		bool hasAnyResourceEnough = false;
		foreach (KeyValuePair<sbyte, CButtonObsolete> pair in this._resourceButtons)
		{
			sbyte resourceType = pair.Key;
			int taiwuResourceCount = this.BuildingModel.GetResourceCount(resourceType);
			int needResource = GameData.Domains.Taiwu.VillagerRole.SharedMethods.GetDevelopNeedResource(displayData.Personalities);
			bool isEnough = taiwuResourceCount >= needResource && this._blockData.MaxResources.Get((int)pair.Key) > GlobalConfig.Instance.VillagerRoleFarmerMigrateMinResource;
			bool flag = isEnough;
			if (flag)
			{
				hasAnyResourceEnough = true;
				firstEnoughResourceType = pair.Key;
				break;
			}
		}
		return new ValueTuple<bool, sbyte>(hasAnyResourceEnough, firstEnoughResourceType);
	}

	// Token: 0x06003A00 RID: 14848 RVA: 0x001D8DD4 File Offset: 0x001D6FD4
	private void DevelopFocusToggleGroupFollowBtn(Transform btnTransform, sbyte resourceType)
	{
		bool isLeft = resourceType <= 2;
		RectTransform toggleTransform = this.DevelopFocusToggleGroup.transform.GetComponent<RectTransform>();
		RectTransform position = this.DevelopFocusHolderRefers.CGet<RectTransform>(isLeft ? "LeftPosition" : "RightPosition");
		toggleTransform.SetParent(position, true);
		toggleTransform.localPosition = Vector3.zero;
		this.DevelopFocusHolder.transform.Find("BgLeft").gameObject.SetActive(isLeft);
		this.DevelopFocusHolder.transform.Find("BgRight").gameObject.SetActive(!isLeft);
	}

	// Token: 0x06003A01 RID: 14849 RVA: 0x001D8E70 File Offset: 0x001D7070
	private void OnDisable()
	{
	}

	// Token: 0x06003A02 RID: 14850 RVA: 0x001D8E73 File Offset: 0x001D7073
	private void OnEnable()
	{
	}

	// Token: 0x040029C5 RID: 10693
	[HideInInspector]
	public bool ShowAutoDispatchButton = true;

	// Token: 0x040029C6 RID: 10694
	[HideInInspector]
	public bool ShowAssignRoleButton = true;

	// Token: 0x040029C7 RID: 10695
	private int _listenerId;

	// Token: 0x040029C8 RID: 10696
	private MapBlockData _blockData;

	// Token: 0x040029C9 RID: 10697
	private Dictionary<int, CharacterDisplayData> _charDisplayDataDict;

	// Token: 0x040029CA RID: 10698
	private Action<int> _onExchangeCharacterIdChanged;

	// Token: 0x040029CB RID: 10699
	private List<int> _unlockedWorkingList;

	// Token: 0x040029CC RID: 10700
	private int _selectedCharacterId;

	// Token: 0x040029CD RID: 10701
	[TupleElementNames(new string[]
	{
		"successRateBonus",
		"chickenSuccess",
		"MigrateSpeedBonusFactor"
	})]
	private ValueTuple<int, int, int> _extraMigrateRate = default(ValueTuple<int, int, int>);

	// Token: 0x040029CE RID: 10702
	private sbyte _currentMigrateResourceType;

	// Token: 0x040029CF RID: 10703
	private VillagerWorkData _workData;

	// Token: 0x040029D0 RID: 10704
	private readonly Dictionary<sbyte, CButtonObsolete> _resourceButtons = new Dictionary<sbyte, CButtonObsolete>();

	// Token: 0x040029D1 RID: 10705
	private bool _hasDebt = false;

	// Token: 0x040029D2 RID: 10706
	private bool _isFarmer = false;

	// Token: 0x040029D3 RID: 10707
	private bool _notBigSize = false;

	// Token: 0x040029D4 RID: 10708
	private bool _notDeveloped = false;

	// Token: 0x040029D5 RID: 10709
	private sbyte _developSelectedResourceType = -1;

	// Token: 0x040029D6 RID: 10710
	private short _developSelectedBlockIndex = 0;

	// Token: 0x040029D7 RID: 10711
	private VillagerRoleCharacterDisplayData _villagerDisplayData;
}
