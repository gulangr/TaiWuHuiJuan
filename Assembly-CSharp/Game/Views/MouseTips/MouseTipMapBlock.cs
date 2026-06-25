using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Config;
using FrameWork;
using GameData.Domains.Adventure;
using GameData.Domains.Character;
using GameData.Domains.Character.Display;
using GameData.Domains.Extra;
using GameData.Domains.Map;
using GameData.Domains.Merchant;
using GameData.Domains.Organization;
using GameData.Domains.Organization.Display;
using GameData.Domains.Taiwu;
using GameData.Domains.Taiwu.Display;
using GameData.Serializer;
using GameData.Utilities;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Game.Views.MouseTips
{
	// Token: 0x0200086D RID: 2157
	public class MouseTipMapBlock : MouseTipBase
	{
		// Token: 0x17000C75 RID: 3189
		// (get) Token: 0x06006804 RID: 26628 RVA: 0x002F8981 File Offset: 0x002F6B81
		protected override bool CanStick
		{
			get
			{
				return true;
			}
		}

		// Token: 0x06006805 RID: 26629 RVA: 0x002F8984 File Offset: 0x002F6B84
		protected override void Init(ArgumentBox argsBox)
		{
			this.Refresh(argsBox);
		}

		// Token: 0x06006806 RID: 26630 RVA: 0x002F8990 File Offset: 0x002F6B90
		public override void Refresh(ArgumentBox argsBox)
		{
			this._contentReady = false;
			this.EnsureCanvasGroup();
			this._tipCanvasGroup.alpha = 0f;
			this._settlementLayout = base.CGet<GameObject>("SettlementLayout");
			this._populationLabel = base.CGet<TextMeshProUGUI>("PopulationLabel");
			this._cultureLabel = base.CGet<TextMeshProUGUI>("CultureLabel");
			this._safetyLabel = base.CGet<TextMeshProUGUI>("SafetyLabel");
			MapBlockData blockData;
			argsBox.Get<MapBlockData>("MapBlockData", out blockData);
			bool isUseFullName;
			argsBox.Get("IsUseFullName", out isUseFullName);
			AdventureSiteData adventureSiteData;
			argsBox.Get<AdventureSiteData>("AdventureSiteData", out adventureSiteData);
			List<CaravanDisplayData> caravanList;
			argsBox.Get<List<CaravanDisplayData>>("CaravanList", out caravanList);
			VillagerWorkData villagerWorkData;
			argsBox.Get<VillagerWorkData>("VillagerWorkData", out villagerWorkData);
			this.NeedDataListenerId = true;
			base.CGet<TextMeshProUGUI>("Title").text = string.Empty;
			this._settlementLayout.SetActive(false);
			this.darkAsh.SetActive(false);
			int settlementId = this.FindSettlementId(blockData);
			Location location = new Location(blockData.AreaId, blockData.BlockId);
			MapDomainMethod.AsyncCall.GetBlockFullName(this, location, delegate(int offsetData, RawDataPool poolData)
			{
				bool flag9 = this == null;
				if (!flag9)
				{
					FullBlockName fullBlockName = default(FullBlockName);
					Serializer.Deserialize(poolData, offsetData, ref fullBlockName);
					string blockName = SingletonObject.getInstance<WorldMapModel>().GetFullBlockName(fullBlockName, false, true, true, true);
					TextMeshProUGUI title;
					bool flag10 = this.CTryGet<TextMeshProUGUI>("Title", out title);
					if (flag10)
					{
						title.text = blockName;
					}
					this._contentReady = true;
					bool flag11 = this._tipCanvasGroup != null;
					if (flag11)
					{
						this._tipCanvasGroup.alpha = 1f;
					}
				}
			});
			MapBlockItem blockConfig = MapBlock.Instance[blockData.TemplateId];
			StringBuilder sb = EasyPool.Get<StringBuilder>();
			sb.Clear();
			sb.AppendLine(blockConfig.Desc);
			bool destroyed = blockData.Destroyed;
			if (destroyed)
			{
				sb.AppendLine();
				sb.AppendLine(LocalStringManager.Get(LanguageKey.LK_MouseTipBlock_Destroyed));
			}
			else
			{
				bool flag = blockConfig.SubType == EMapBlockSubType.Ruin;
				if (flag)
				{
					sb.AppendLine();
					sb.AppendLine(LocalStringManager.Get(LanguageKey.LK_MouseTipBlock_Ruined));
				}
			}
			base.CGet<TextMeshProUGUI>("Desc").text = sb.ToString().ColorReplace();
			Refers treasureLayout = base.CGet<Refers>("TreasureLayout");
			treasureLayout.gameObject.SetActive(SingletonObject.getInstance<FunctionLockManager>().IsFunctionUnlock(10));
			Location location2Compare = location;
			ExtraDomainMethod.AsyncCall.FindTreasureExpect(this, location, delegate(int offsetData, RawDataPool poolData)
			{
				bool flag9 = this == null;
				if (!flag9)
				{
					try
					{
						TreasureExpectResult expectResult = default(TreasureExpectResult);
						Serializer.Deserialize(poolData, offsetData, ref expectResult);
						bool flag10 = expectResult.Location != location2Compare;
						if (!flag10)
						{
							bool flag11 = expectResult.Chance > 66;
							LanguageKey rate;
							if (flag11)
							{
								rate = LanguageKey.LK_Treasure_Expect_Tips_Rate_66_100;
							}
							else
							{
								bool flag12 = expectResult.Chance > 33;
								if (flag12)
								{
									rate = LanguageKey.LK_Treasure_Expect_Tips_Rate_33_66;
								}
								else
								{
									rate = LanguageKey.LK_Treasure_Expect_Tips_Rate_0_33;
								}
							}
							bool flag13 = expectResult.MaxGrade >= 6;
							LanguageKey grade;
							if (flag13)
							{
								grade = LanguageKey.LK_Treasure_Expect_Tips_Grade_High;
							}
							else
							{
								bool flag14 = expectResult.MaxGrade >= 3;
								if (flag14)
								{
									grade = LanguageKey.LK_Treasure_Expect_Tips_Grade_Middle;
								}
								else
								{
									grade = LanguageKey.LK_Treasure_Expect_Tips_Grade_Low;
								}
							}
							string desc = (expectResult.Chance == 0) ? (expectResult.AnyMaterial ? LocalStringManager.Get(LanguageKey.LK_Treasure_Expect_Tips_Material) : LocalStringManager.Get(LanguageKey.LK_Treasure_Expect_Tips_None)) : LocalStringManager.GetFormat(LanguageKey.LK_Treasure_Expect_Tips_Content, LocalStringManager.Get(rate), LocalStringManager.Get(grade));
							bool flag15 = expectResult.Chance > 0 && expectResult.AnyMaterial;
							if (flag15)
							{
								desc = desc + "\n" + LocalStringManager.Get(LanguageKey.LK_Treasure_Expect_Tips_Material);
							}
							TextMeshProUGUI descText = treasureLayout.CGet<TextMeshProUGUI>("Desc");
							descText.text = desc.ColorReplace();
						}
					}
					catch (Exception e)
					{
						Debug.LogError(e);
					}
				}
			});
			RectTransform resourcesHolder = base.CGet<RectTransform>("ResourceHolder");
			bool flag2 = blockData.IsCityTown() || blockData.CurrResources.GetSum() == 0;
			if (flag2)
			{
				resourcesHolder.parent.gameObject.SetActive(false);
			}
			else
			{
				Refers[] resourceDisplayRefers = new Refers[]
				{
					resourcesHolder.Find("Food").GetComponent<Refers>(),
					null,
					null,
					null,
					null,
					resourcesHolder.Find("Herb").GetComponent<Refers>()
				};
				resourceDisplayRefers[3] = resourcesHolder.Find("Jade").GetComponent<Refers>();
				resourceDisplayRefers[2] = resourcesHolder.Find("Metal").GetComponent<Refers>();
				resourceDisplayRefers[4] = resourcesHolder.Find("Fabric").GetComponent<Refers>();
				resourceDisplayRefers[1] = resourcesHolder.Find("Wood").GetComponent<Refers>();
				for (sbyte resourceType = 0; resourceType < 6; resourceType += 1)
				{
					Refers resourceRefers = resourceDisplayRefers[(int)resourceType];
					short value = blockData.CurrResources.Get((int)resourceType);
					short maxValue = blockData.MaxResources.Get((int)resourceType);
					resourceRefers.CGet<TextMeshProUGUI>("Current").text = LocalStringManager.GetFormat(LanguageKey.LK_Make_Resource_Require_Meet, value, maxValue);
					resourceRefers.CGet<CImagePinned>("ValueBack").gameObject.SetActive(value >= 100);
				}
				resourcesHolder.parent.gameObject.SetActive(true);
			}
			RectTransform caravanLayout = base.CGet<RectTransform>("CaravanLayout");
			bool hasCaravan = caravanList != null && caravanList.Count > 0;
			caravanLayout.gameObject.SetActive(hasCaravan);
			bool flag3 = hasCaravan;
			if (flag3)
			{
				for (int i = 0; i < caravanList.Count; i++)
				{
					int childCount = caravanLayout.childCount;
					bool flag4 = childCount < i + 1;
					Transform child;
					if (flag4)
					{
						Transform lastChild = caravanLayout.GetChild(childCount - 1);
						child = Object.Instantiate<Transform>(lastChild, caravanLayout);
					}
					else
					{
						child = caravanLayout.GetChild(i);
					}
					Refers refer = child.GetComponent<Refers>();
					CaravanDisplayData caravanData = caravanList[i];
					MerchantItem merchantConfig = Merchant.Instance[(int)caravanData.MerchantTemplateId];
					MerchantTypeItem merchantTypeConfig = Config.MerchantType.Instance[merchantConfig.MerchantType];
					refer.CGet<TextMeshProUGUI>("SubTitle").text = LocalStringManager.GetFormat(LanguageKey.LK_Block_Tip_SubTitle, LocalStringManager.Get(LanguageKey.LK_Caravan), merchantTypeConfig.Name).SetColor("caravan");
					bool flag5 = caravanData.PathInArea.MoveNodes.Count >= 2;
					int node;
					if (flag5)
					{
						node = caravanData.PathInArea.MoveNodes[1];
					}
					else
					{
						node = caravanData.PathInArea.MoveNodes.FirstOrDefault<int>();
					}
					bool flag6 = caravanData.PathInArea.FullPath.CheckIndex(node);
					if (flag6)
					{
						int time = (int)caravanData.PathInArea.MoveWaitDays / (TimeManager.ActionPointRecovery / 10);
						time = Math.Max(time, 1);
						location = caravanData.PathInArea.FullPath[node];
						MapDomainMethod.AsyncCall.GetBlockFullName(this, location, delegate(int offsetData, RawDataPool poolData)
						{
							bool flag9 = this == null;
							if (!flag9)
							{
								FullBlockName fullBlockName = default(FullBlockName);
								Serializer.Deserialize(poolData, offsetData, ref fullBlockName);
								string blockName = SingletonObject.getInstance<WorldMapModel>().GetFullBlockName(fullBlockName, false, true, true, true);
								string desc = LocalStringManager.GetFormat(LanguageKey.LK_Block_Tip_Caravan_Time_Desc, time).ColorReplace();
								string desc2 = LocalStringManager.GetFormat((node == 0) ? LanguageKey.LK_Block_Tip_Caravan_Stay_Desc : LanguageKey.LK_Block_Tip_Caravan_Move_Desc, blockName).ColorReplace();
								refer.CGet<TextMeshProUGUI>("Desc").text = desc;
								refer.CGet<TextMeshProUGUI>("Desc_1").text = desc2;
							}
						});
						bool flag7 = !child.gameObject.activeSelf;
						if (flag7)
						{
							child.gameObject.SetActive(true);
						}
					}
					else
					{
						child.gameObject.SetActive(false);
					}
				}
				for (int j = caravanList.Count; j < caravanLayout.childCount; j++)
				{
					Transform child2 = caravanLayout.GetChild(j);
					child2.gameObject.SetActive(false);
				}
			}
			Refers dispatchLayout = base.CGet<Refers>("DispatchLayout");
			Config.AdventureItem adventureConfig = (adventureSiteData != null) ? Adventure.Instance[adventureSiteData.TemplateId] : null;
			bool canCollectTribute = adventureConfig != null && (adventureConfig.Type == 4 || adventureConfig.Type == 5);
			bool flag8 = villagerWorkData != null && villagerWorkData.WorkType != 11 && !canCollectTribute;
			if (flag8)
			{
				string workText = string.Empty;
				switch (villagerWorkData.WorkType)
				{
				case 10:
					workText = LocalStringManager.Get(LanguageKey.LK_Dispatch_Type_Resource);
					break;
				case 12:
					workText = LocalStringManager.Get(LanguageKey.LK_Dispatch_Type_Grave);
					break;
				case 13:
					workText = LocalStringManager.Get(LanguageKey.LK_Dispatch_Type_Idle);
					break;
				case 14:
					workText = LocalStringManager.Get(LanguageKey.LK_Dispatch_Type_Migrate);
					break;
				}
				string subTitle = LocalStringManager.GetFormat(LanguageKey.LK_Block_Tip_SubTitle, LocalStringManager.Get(LanguageKey.LK_Dispatch), workText);
				dispatchLayout.CGet<TextMeshProUGUI>("SubTitle").text = subTitle;
				AsyncMethodCallbackDelegate <>9__4;
				CharacterDomainMethod.AsyncCall.GetCharacterDisplayDataList(this, new List<int>
				{
					villagerWorkData.CharacterId
				}, delegate(int offset, RawDataPool dataPool)
				{
					bool flag9 = this == null;
					if (!flag9)
					{
						List<CharacterDisplayData> charDisplayDataList = EasyPool.Get<List<CharacterDisplayData>>();
						charDisplayDataList.Clear();
						Serializer.Deserialize(dataPool, offset, ref charDisplayDataList);
						bool flag10 = charDisplayDataList != null && charDisplayDataList.Count > 0;
						if (flag10)
						{
							string charName = NameCenter.GetCharMonasticTitleOrNameByDisplayData(charDisplayDataList.First<CharacterDisplayData>(), false, false);
							string desc = LocalStringManager.GetFormat(LanguageKey.LK_Block_Tip_Dispatch_Desc, charName, workText).ColorReplace();
							dispatchLayout.CGet<TextMeshProUGUI>("Desc").text = desc;
							IAsyncMethodRequestHandler requestHandler = null;
							int characterId = villagerWorkData.CharacterId;
							AsyncMethodCallbackDelegate callback;
							if ((callback = <>9__4) == null)
							{
								callback = (<>9__4 = delegate(int offset, RawDataPool dataPool)
								{
									bool flag11 = this == null;
									if (!flag11)
									{
										VillagerRoleCharacterDisplayData displayData = new VillagerRoleCharacterDisplayData();
										Serializer.Deserialize(dataPool, offset, ref displayData);
										bool flag12 = villagerWorkData.WorkType != 14;
										if (flag12)
										{
											string state = (villagerWorkData.ResourceType < 0) ? string.Empty : LocalStringManager.GetFormat(LanguageKey.LK_Block_Tip_Dispatch_Resource, displayData.CollectResourceAmount, Config.ResourceType.Instance[villagerWorkData.ResourceType].Name).ColorReplace();
											dispatchLayout.CGet<TextMeshProUGUI>("State").text = state;
											dispatchLayout.CGet<TextMeshProUGUI>("Rate").transform.parent.gameObject.SetActive(false);
										}
										else
										{
											dispatchLayout.CGet<TextMeshProUGUI>("Rate").transform.parent.gameObject.SetActive(true);
											TaiwuDomainMethod.AsyncCall.GetVillagerFarmerMigrateResourceSuccessRateBonus(null, villagerWorkData.CharacterId, delegate(int offset, RawDataPool dataPool)
											{
												bool flag13 = this == null;
												if (!flag13)
												{
													ValueTuple<int, int, int> bonuses = default(ValueTuple<int, int, int>);
													Serializer.Deserialize(dataPool, offset, ref bonuses);
													StringBuilder rateSb = EasyPool.Get<StringBuilder>();
													rateSb.Clear();
													int successRate = VillagerWorkPanel.CalcFarmerMigrateSuccessRateBaseValue(displayData.Personalities);
													successRate *= bonuses.Item3;
													short hasResource = blockData.CurrResources.Get((int)villagerWorkData.ResourceType);
													short needResource = GlobalConfig.Instance.VillagerRoleFarmerMigrateMinResource;
													bool isEnough = hasResource >= needResource;
													string successRateText = isEnough ? string.Format("{0}%", successRate).SetColor("pinkyellow") : "/";
													string bonusSuccessRate = string.Format("+{0}%", bonuses.Item1).SetColor("brightblue");
													rateSb.Append(successRateText);
													bool flag14 = isEnough;
													if (flag14)
													{
														rateSb.Append(bonusSuccessRate);
													}
													rateSb.Append("\n");
													dispatchLayout.CGet<TextMeshProUGUI>("Rate").text = rateSb.ToString();
													StringBuilder stateSb = EasyPool.Get<StringBuilder>();
													stateSb.Clear();
													stateSb.Append(LocalStringManager.Get(LanguageKey.LK_Migrate_Hint).SetColor("pinkyellow"));
													bool flag15 = bonuses.Item3 > 0;
													if (flag15)
													{
														stateSb.Append("\n");
														stateSb.Append(LocalStringManager.Get(LanguageKey.LK_Migrate_ExtraHint).SetColor("brightblue"));
													}
													stateSb.Append("\n");
													bool flag16 = bonuses.Item2 > 0;
													if (flag16)
													{
														string upgradeString = (bonuses.Item2.ToString() + "%").SetColor("pinkyellow");
														stateSb.AppendLine(LocalStringManager.Get(LanguageKey.LK_VillagerRole_Farmer_UpgradeSuccessRate) + LocalStringManager.Get(LanguageKey.LK_Colon_Symbol) + upgradeString);
													}
													stateSb.Append("<line-height=12px>\n<line-height=100%>");
													stateSb.Append(LocalStringManager.Get(LanguageKey.LK_VillagerWork_MigrateTips_PossibleBuildingCoreItem));
													stateSb.Append(LocalStringManager.Get(LanguageKey.LK_Colon_Symbol));
													ResourceTypeItem resourceTypeConfig = Config.ResourceType.Instance[villagerWorkData.ResourceType];
													for (int k = 0; k < resourceTypeConfig.PossibleBuildingCoreItem.Length; k++)
													{
														stateSb.Append("\n");
														MiscItem miscConfig = Misc.Instance[resourceTypeConfig.PossibleBuildingCoreItem[k]];
														stateSb.Append(TMPTextSpriteHelper.GetStringWithTextSpriteTag("ui9_icon_mousetip_point_0") + miscConfig.Name.SetGradeColor((int)miscConfig.Grade));
													}
													bool flag17 = bonuses.Item2 > 0;
													if (flag17)
													{
														foreach (short matId in resourceTypeConfig.PossibleUpgradedBuildingCoreItem)
														{
															MiscItem miscConfig2 = Misc.Instance[matId];
															stateSb.Append("\n");
															stateSb.Append(TMPTextSpriteHelper.GetStringWithTextSpriteTag("ui9_icon_mousetip_point_0") + miscConfig2.Name.SetGradeColor((int)miscConfig2.Grade));
														}
													}
													TextMeshProUGUI stateLabel = dispatchLayout.CGet<TextMeshProUGUI>("State");
													stateLabel.SetText(stateSb.ToString().ColorReplace(), true);
													stateLabel.GetComponent<TMPTextSpriteHelper>().Parse();
													this.DelayRebuildLayout();
													EasyPool.Free<StringBuilder>(stateSb);
													EasyPool.Free<StringBuilder>(rateSb);
												}
											});
										}
										dispatchLayout.gameObject.SetActive(true);
										this.DelayRebuildLayout();
									}
								});
							}
							TaiwuDomainMethod.AsyncCall.GetVillagerRoleCharacterDisplayData(requestHandler, characterId, callback);
						}
						else
						{
							dispatchLayout.gameObject.SetActive(false);
						}
						EasyPool.Free<List<CharacterDisplayData>>(charDisplayDataList);
					}
				});
			}
			else
			{
				dispatchLayout.gameObject.SetActive(false);
			}
			this.RefreshSettlementInfo(blockData, settlementId);
			this.Element.ShowAfterRefresh();
		}

		// Token: 0x06006807 RID: 26631 RVA: 0x002F9178 File Offset: 0x002F7378
		private void EnsureCanvasGroup()
		{
			bool flag = this._tipCanvasGroup != null;
			if (!flag)
			{
				this._tipCanvasGroup = base.GetComponent<CanvasGroup>();
				bool flag2 = this._tipCanvasGroup == null;
				if (flag2)
				{
					this._tipCanvasGroup = base.gameObject.AddComponent<CanvasGroup>();
				}
			}
		}

		// Token: 0x06006808 RID: 26632 RVA: 0x002F91C5 File Offset: 0x002F73C5
		private void DelayRebuildLayout()
		{
			base.DelayFrameCall(delegate
			{
				LayoutRebuilder.ForceRebuildLayoutImmediate(base.GetComponent<RectTransform>());
			}, 1U);
		}

		// Token: 0x06006809 RID: 26633 RVA: 0x002F91DC File Offset: 0x002F73DC
		private int FindSettlementId(MapBlockData blockData)
		{
			WorldMapModel mapModel = SingletonObject.getInstance<WorldMapModel>();
			MapAreaData areaData = mapModel.Areas[(int)blockData.AreaId];
			MapBlockData rootBlock = blockData.GetRootBlock();
			short? rootBlockId = (rootBlock != null) ? new short?(rootBlock.BlockId) : null;
			for (int i = 0; i < areaData.SettlementInfos.Length; i++)
			{
				int blockId = (int)areaData.SettlementInfos[i].BlockId;
				short? num = rootBlockId;
				int? num2 = (num != null) ? new int?((int)num.GetValueOrDefault()) : null;
				bool flag = blockId == num2.GetValueOrDefault() & num2 != null;
				if (flag)
				{
					return (int)areaData.SettlementInfos[i].SettlementId;
				}
			}
			return -1;
		}

		// Token: 0x0600680A RID: 26634 RVA: 0x002F92A8 File Offset: 0x002F74A8
		private void RefreshSettlementInfo(MapBlockData blockData, int settlementId)
		{
			bool flag = settlementId < 0;
			if (flag)
			{
				this.DelayRebuildLayout();
			}
			else
			{
				OrganizationDomainMethod.AsyncCall.GetDisplayData(this, (short)settlementId, delegate(int offset, RawDataPool pool)
				{
					bool flag2 = this == null;
					if (!flag2)
					{
						SettlementDisplayData displayData = default(SettlementDisplayData);
						Serializer.Deserialize(pool, offset, ref displayData);
						bool flag3 = displayData.SettlementId == settlementId;
						if (flag3)
						{
							this._cultureLabel.text = string.Format("{0}/{1}", displayData.Culture, displayData.MaxCulture);
							this._safetyLabel.text = string.Format("{0}/{1}", displayData.Safety, displayData.MaxSafety);
							MapAreaItem mapAreaItem = MapArea.Instance[displayData.AreaTemplateId];
							bool flag4 = mapAreaItem != null && !mapAreaItem.ShowDarkAshStatus;
							if (flag4)
							{
								this.darkAsh.SetActive(false);
							}
							else
							{
								this.darkAsh.SetActive(true);
								this._populationLabel.text = displayData.DarkAshStatus.Tr().ColorReplace();
								this.darkAshTips.text = displayData.DarkAshTips.Tr().ColorReplace();
							}
							this._settlementLayout.SetActive(true);
						}
					}
				});
				this.DelayRebuildLayout();
			}
		}

		// Token: 0x04004999 RID: 18841
		[SerializeField]
		private TMP_Text darkAshTips;

		// Token: 0x0400499A RID: 18842
		[SerializeField]
		private GameObject darkAsh;

		// Token: 0x0400499B RID: 18843
		private bool _contentReady;

		// Token: 0x0400499C RID: 18844
		private CanvasGroup _tipCanvasGroup;

		// Token: 0x0400499D RID: 18845
		private GameObject _settlementLayout;

		// Token: 0x0400499E RID: 18846
		private TextMeshProUGUI _populationLabel;

		// Token: 0x0400499F RID: 18847
		private TextMeshProUGUI _cultureLabel;

		// Token: 0x040049A0 RID: 18848
		private TextMeshProUGUI _safetyLabel;
	}
}
