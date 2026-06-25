using System;
using Config;
using FrameWork;
using FrameWork.UISystem.UIElements;
using Game.Views.Make;
using GameData.Domains.Item;
using GameData.Domains.Item.Display;
using UnityEngine;

namespace Game.Components.ListStyleGeneralScroll.Item
{
	// Token: 0x02000EAB RID: 3755
	public class RowItemLine : RowItem
	{
		// Token: 0x170013C4 RID: 5060
		// (get) Token: 0x0600AE6C RID: 44652 RVA: 0x004F7724 File Offset: 0x004F5924
		// (set) Token: 0x0600AE6D RID: 44653 RVA: 0x004F772C File Offset: 0x004F592C
		public RowItemMain RowItemMain { get; private set; }

		// Token: 0x170013C5 RID: 5061
		// (get) Token: 0x0600AE6E RID: 44654 RVA: 0x004F7735 File Offset: 0x004F5935
		public ITradeableContent Data
		{
			get
			{
				RowItemMain rowItemMain = this.RowItemMain;
				return (rowItemMain != null) ? rowItemMain.Data : null;
			}
		}

		// Token: 0x0600AE6F RID: 44655 RVA: 0x004F774C File Offset: 0x004F594C
		public virtual void Set(RowItemMain rowItemMain, bool showTip = true)
		{
			this.RowItemMain = rowItemMain;
			this.tipDisplayer.enabled = (showTip && this.Data != null && (this.Data.CharacterId != -1 || this.Data.RealKey.HasTemplate));
			bool enabled = this.tipDisplayer.enabled;
			if (enabled)
			{
				RowItemLine.SetMouseTipDisplayer(true, this.RowItemMain.Data, this.tipDisplayer);
			}
		}

		// Token: 0x0600AE70 RID: 44656 RVA: 0x004F77C7 File Offset: 0x004F59C7
		public void SetLocked(bool locked)
		{
			GameObject gameObject = this.lockedObject;
			if (gameObject != null)
			{
				gameObject.SetActive(locked);
			}
		}

		// Token: 0x0600AE71 RID: 44657 RVA: 0x004F77E0 File Offset: 0x004F59E0
		public void EnterSelectCountMode()
		{
			bool flag = this.confirmObject;
			if (flag)
			{
				this.confirmObject.SetActive(true);
				this.confirmObject.transform.SetAsLastSibling();
			}
			this.buttonConfirm.ClearAndAddListener(delegate
			{
				GEvent.OnEvent(UiEvents.OnConfirmSetSelectCount, null);
			});
		}

		// Token: 0x0600AE72 RID: 44658 RVA: 0x004F7848 File Offset: 0x004F5A48
		public void ExitSelectCountMode()
		{
			GameObject gameObject = this.confirmObject;
			if (gameObject != null)
			{
				gameObject.SetActive(false);
			}
			this.buttonConfirm.onClick.RemoveAllListeners();
		}

		// Token: 0x0600AE73 RID: 44659 RVA: 0x004F7870 File Offset: 0x004F5A70
		public static void SetMouseTipDisplayer(bool showBookPageInfo, ITradeableContent itemData, TooltipInvoker tip)
		{
			bool templateDataOnly = !itemData.RealKey.IsValid();
			tip.RuntimeParam = null;
			bool flag = itemData.CharacterId != -1;
			if (flag)
			{
				tip.enabled = true;
				tip.Type = TipType.CharacterOnMapBlock;
				tip.RuntimeParam = EasyPool.Get<ArgumentBox>().Set("CharId", itemData.CharacterId);
				tip.Refresh(true, -1);
			}
			else
			{
				bool isRandomMake = MakeSubPageMakeHelper.CheckIsRandomMake(itemData);
				bool flag2 = isRandomMake;
				if (flag2)
				{
					tip.enabled = false;
				}
				else
				{
					tip.enabled = true;
					bool flag3 = itemData.Key.ItemType == 12 && itemData.Key.TemplateId == 475;
					if (flag3)
					{
						RowItemLine.SetPrisonerDisplayerProperties(itemData, tip);
					}
					else
					{
						bool flag4 = itemData.Key.ItemType == 12 && (itemData.Key.TemplateId == 388 || itemData.Key.TemplateId == 389);
						if (flag4)
						{
							tip.enabled = false;
						}
						else
						{
							bool flag5 = itemData.Key.ItemType == 12 && itemData.Key.TemplateId >= 476 && itemData.Key.TemplateId <= 481;
							if (flag5)
							{
								tip.enabled = false;
							}
							else
							{
								bool flag6 = itemData.Key.ItemType == 12 && itemData.Key.TemplateId >= 482 && itemData.Key.TemplateId <= 483;
								if (flag6)
								{
									tip.enabled = false;
								}
								else
								{
									bool flag7 = itemData.Key.ItemType == 10 && itemData.Key.Id == -1 && itemData.IsSpecialInteract && SkillBook.Instance[itemData.Key.TemplateId].CombatSkillTemplateId >= 0;
									if (flag7)
									{
										RowItemLine.SetCombatSkillDisplayerProperties(itemData, tip);
									}
									else
									{
										bool flag8;
										if (itemData.Key.Id >= 0 && itemData.Key.ItemType == 5)
										{
											short templateId = itemData.Key.TemplateId;
											flag8 = (templateId >= 309 && templateId <= 339);
										}
										else
										{
											flag8 = false;
										}
										bool flag9 = flag8;
										if (flag9)
										{
											RowItemLine.SetJiaoTipDisplayerProperties(itemData, tip);
										}
										else
										{
											bool flag10;
											if (itemData.Key.Id >= 0 && itemData.Key.ItemType == 4)
											{
												short templateId = itemData.Key.TemplateId;
												flag10 = (templateId >= 46 && templateId <= 76);
											}
											else
											{
												flag10 = false;
											}
											bool flag11 = flag10;
											if (flag11)
											{
												RowItemLine.SetJiaoTipDisplayerProperties(itemData, tip);
											}
											else
											{
												bool flag12;
												if (itemData.Key.Id >= 0 && itemData.Key.ItemType == 4)
												{
													short templateId = itemData.Key.TemplateId;
													flag12 = (templateId >= 77 && templateId <= 85);
												}
												else
												{
													flag12 = false;
												}
												bool flag13 = flag12;
												if (flag13)
												{
													RowItemLine.SetJiaoTipDisplayerProperties(itemData, tip);
												}
												else
												{
													bool flag14 = itemData.Key.ItemType == 12 && itemData.Key.TemplateId == 239;
													if (flag14)
													{
														tip.Type = TipType.Fuyu;
														tip.NeedRefresh = (UIElement.Combat.Exist && itemData.Key.ItemType == 0 && itemData.UsingType == ItemDisplayData.ItemUsingType.Equiped);
														tip.RuntimeParam = new ArgumentBox().SetObject("ItemData", itemData.Clone(-1));
														tip.RuntimeParam.Set("ShowPageInfo", itemData.Key.ItemType == 10 && showBookPageInfo);
														tip.RuntimeParam.Set("TemplateDataOnly", templateDataOnly);
													}
													else
													{
														bool flag15 = itemData.Key.ItemType == 5 && itemData.Key.TemplateId >= 278 && itemData.Key.TemplateId <= 308;
														if (flag15)
														{
															tip.Type = TipType.JiaoEgg;
															tip.NeedRefresh = (UIElement.Combat.Exist && itemData.Key.ItemType == 0 && itemData.UsingType == ItemDisplayData.ItemUsingType.Equiped);
															tip.RuntimeParam = new ArgumentBox().SetObject("ItemData", itemData.Clone(-1));
														}
														else
														{
															bool flag16 = ItemTemplateHelper.IsEmptyTool(itemData.Key.ItemType, itemData.Key.TemplateId);
															if (flag16)
															{
																tip.Type = TipType.SingleDesc;
																tip.RuntimeParam = new ArgumentBox().Set("arg0", LocalStringManager.Get(LanguageKey.LK_Make_None_Tool_Tip).ColorReplace());
															}
															else
															{
																bool flag17 = ItemTemplateHelper.IsMiscResource(itemData.Key.ItemType, itemData.Key.TemplateId);
																if (flag17)
																{
																	tip.Type = TipType.Resource;
																}
																else
																{
																	bool flag18 = itemData.Key.ItemType == 12 && itemData.Key.TemplateId >= 240 && itemData.Key.TemplateId <= 253;
																	if (flag18)
																	{
																		tip.Type = TipType.LegendaryBook;
																		tip.RuntimeParam = new ArgumentBox().SetObject("ItemData", itemData.Clone(-1));
																	}
																	else
																	{
																		tip.Type = TooltipManager.ItemTypeToTipType[itemData.Key.ItemType];
																		tip.NeedRefresh = (UIElement.Combat.Exist && itemData.Key.ItemType == 0 && itemData.UsingType == ItemDisplayData.ItemUsingType.Equiped);
																		tip.RuntimeParam = new ArgumentBox().SetObject("ItemData", itemData.Clone(-1));
																		tip.RuntimeParam.Set("ShowPageInfo", itemData.Key.ItemType == 10 && showBookPageInfo);
																		tip.RuntimeParam.Set("TemplateDataOnly", templateDataOnly);
																		tip.RuntimeParam.Set("CharId", itemData.OwnerCharId);
																		tip.RuntimeParam.Set("IsNew", false);
																	}
																}
															}
														}
													}
													tip.Refresh(true, -1);
												}
											}
										}
									}
								}
							}
						}
					}
				}
			}
		}

		// Token: 0x0600AE74 RID: 44660 RVA: 0x004F7E88 File Offset: 0x004F6088
		private static void SetJiaoTipDisplayerProperties(ITradeableContent itemDisplayData, TooltipInvoker tip)
		{
			tip.Type = TipType.Jiao;
			tip.NeedRefresh = (UIElement.Combat.Exist && itemDisplayData.Key.ItemType == 0 && itemDisplayData.UsingType == ItemDisplayData.ItemUsingType.Equiped);
			tip.RuntimeParam = new ArgumentBox().SetObject("ItemData", itemDisplayData.Clone(-1));
			tip.Refresh(true, -1);
		}

		// Token: 0x0600AE75 RID: 44661 RVA: 0x004F7EEF File Offset: 0x004F60EF
		private static void SetPrisonerDisplayerProperties(ITradeableContent itemDisplayData, TooltipInvoker tip)
		{
			tip.Type = TipType.Character;
			tip.RuntimeParam = EasyPool.Get<ArgumentBox>();
			tip.RuntimeParam.Set("charId", itemDisplayData.CharacterId);
			tip.Refresh(true, -1);
		}

		// Token: 0x0600AE76 RID: 44662 RVA: 0x004F7F28 File Offset: 0x004F6128
		private static void SetCombatSkillDisplayerProperties(ITradeableContent itemDisplayData, TooltipInvoker tip)
		{
			short skillId = SkillBook.Instance[itemDisplayData.Key.TemplateId].CombatSkillTemplateId;
			tip.Type = TipType.CombatSkill;
			tip.RuntimeParam = EasyPool.Get<ArgumentBox>();
			tip.RuntimeParam.Set("CombatSkillId", skillId);
			tip.RuntimeParam.Set("CharId", -1);
			tip.RuntimeParam.Set("ShowOnlyTemplateInfo", true);
			tip.Refresh(true, -1);
		}

		// Token: 0x0600AE77 RID: 44663 RVA: 0x004F7FA4 File Offset: 0x004F61A4
		public static void SetResourceTip(ITradeableContent itemDisplayData, TooltipInvoker tip, string charName, bool showDetail, bool isTaiwu = false)
		{
			bool flag = !itemDisplayData.IsResource;
			if (!flag)
			{
				tip.Type = TipType.Resource;
				sbyte resourceType = ItemTemplateHelper.GetMiscResourceType(itemDisplayData.Key.ItemType, itemDisplayData.Key.TemplateId);
				ArgumentBox argumentBox = EasyPool.Get<ArgumentBox>();
				argumentBox.Set("CharName", charName);
				argumentBox.Set("ResourceType", resourceType);
				argumentBox.Set("ResourceCount", itemDisplayData.Amount);
				bool flag2 = showDetail && ItemTemplateHelper.MiscResourceCanChoosy(itemDisplayData.Key.ItemType, itemDisplayData.Key.TemplateId);
				if (flag2)
				{
					int materialResourceCountMax = SingletonObject.getInstance<BasicGameData>().MaterialResourceMaxCount;
					argumentBox.Set("ResourceCountMax", materialResourceCountMax);
				}
				argumentBox.Set("ShowDetailChange", showDetail);
				argumentBox.Set("ShowOfferUpChange", showDetail);
				if (isTaiwu)
				{
					argumentBox.SetObject("ResourceDict", SingletonObject.getInstance<BuildingModel>().ResourceDict);
				}
				tip.RuntimeParam = argumentBox;
			}
		}

		// Token: 0x040086BE RID: 34494
		[SerializeField]
		private GameObject lockedObject;

		// Token: 0x040086BF RID: 34495
		[SerializeField]
		private GameObject confirmObject;

		// Token: 0x040086C0 RID: 34496
		[SerializeField]
		private CButton buttonConfirm;
	}
}
