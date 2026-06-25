using System;
using Config;
using FrameWork;
using FrameWork.UI.LanguageRule;
using Game.Components.Item;
using GameData.Domains.Extra;
using GameData.Domains.Item;
using GameData.Domains.Item.Display;
using GameData.Domains.Taiwu.ExchangeSystem;
using GameDataExtensions;
using JetBrains.Annotations;
using TMPro;
using UnityEngine;

namespace Game.Components.ListStyleGeneralScroll.Item
{
	// Token: 0x02000EAC RID: 3756
	public class RowItemMain : MonoBehaviour, ICellContent<ITradeableContent>, ICellContent
	{
		// Token: 0x170013C6 RID: 5062
		// (get) Token: 0x0600AE79 RID: 44665 RVA: 0x004F80A4 File Offset: 0x004F62A4
		public ItemBack ItemBack
		{
			get
			{
				return this.itemBack;
			}
		}

		// Token: 0x0600AE7A RID: 44666 RVA: 0x004F80AC File Offset: 0x004F62AC
		public void SetEquipStatus(int status)
		{
			this.SetEquipStatus((RowItemMain.EquipStatus)status);
		}

		// Token: 0x0600AE7B RID: 44667 RVA: 0x004F80B8 File Offset: 0x004F62B8
		public void SetEquipStatus(RowItemMain.EquipStatus status)
		{
			CImage cimage = this.equipStatus;
			if (!true)
			{
			}
			string text;
			if (status != RowItemMain.EquipStatus.Preset)
			{
				if (status != RowItemMain.EquipStatus.Equip)
				{
					text = string.Empty;
				}
				else
				{
					text = "ui9_icon_item_state_equip_1";
				}
			}
			else
			{
				text = "ui9_icon_item_state_equip_0";
			}
			if (!true)
			{
			}
			cimage.SetSprite(text, false, null);
			this.equipStatusGo.SetActive(status != RowItemMain.EquipStatus.None);
			TooltipInvoker tip = this.equipStatusGo.GetComponent<TooltipInvoker>();
			bool flag = tip;
			if (flag)
			{
				if (!true)
				{
				}
				if (status != RowItemMain.EquipStatus.Preset)
				{
					if (status != RowItemMain.EquipStatus.Equip)
					{
						text = string.Empty;
					}
					else
					{
						text = LanguageKey.LK_ItemUsingType_Equiping.Tr();
					}
				}
				else
				{
					text = LanguageKey.LK_ItemUsingType_EquipmentPlaned.Tr();
				}
				if (!true)
				{
				}
				string content = text;
				tip.enabled = !content.IsNullOrEmpty();
				bool enabled = tip.enabled;
				if (enabled)
				{
					tip.PresetParam = new string[]
					{
						content
					};
				}
			}
		}

		// Token: 0x0600AE7C RID: 44668 RVA: 0x004F8194 File Offset: 0x004F6394
		public void SetCricketPresetStatus(bool isInCurrentCricketPreset)
		{
			this.cricketPresetStatusGo.SetActive(isInCurrentCricketPreset);
			TooltipInvoker tip = this.cricketPresetStatusGo.GetComponent<TooltipInvoker>();
			bool flag = tip;
			if (flag)
			{
				tip.enabled = isInCurrentCricketPreset;
				if (isInCurrentCricketPreset)
				{
					tip.PresetParam = new string[]
					{
						LanguageKey.LK_ItemUsingType_CricketPreset.Tr()
					};
				}
			}
		}

		// Token: 0x0600AE7D RID: 44669 RVA: 0x004F81ED File Offset: 0x004F63ED
		public void SetReadingStatus(int status)
		{
			this.SetReadingStatus((RowItemMain.ReadingStatus)status);
		}

		// Token: 0x0600AE7E RID: 44670 RVA: 0x004F81F8 File Offset: 0x004F63F8
		public void SetReadingStatus(RowItemMain.ReadingStatus status)
		{
			CImage cimage = this.readingStatus;
			if (!true)
			{
			}
			string text;
			switch (status)
			{
			case RowItemMain.ReadingStatus.Reading:
				text = "ui9_icon_item_state_read_0";
				break;
			case RowItemMain.ReadingStatus.Referencing:
				text = "ui9_icon_item_state_read_1";
				break;
			case RowItemMain.ReadingStatus.Done:
				text = "ui9_icon_item_state_read_2";
				break;
			default:
				text = string.Empty;
				break;
			}
			if (!true)
			{
			}
			cimage.SetSprite(text, false, null);
			this.readingStatusGo.SetActive(status != RowItemMain.ReadingStatus.None);
			TooltipInvoker tip = this.readingStatusGo.GetComponent<TooltipInvoker>();
			bool flag = tip;
			if (flag)
			{
				if (!true)
				{
				}
				switch (status)
				{
				case RowItemMain.ReadingStatus.Reading:
					text = LanguageKey.LK_ItemUsingType_Reading.Tr();
					break;
				case RowItemMain.ReadingStatus.Referencing:
					text = LanguageKey.LK_ItemUsingType_Referring.Tr();
					break;
				case RowItemMain.ReadingStatus.Done:
					text = LanguageKey.LK_ItemUsingType_ReadingFinished.Tr();
					break;
				default:
					text = string.Empty;
					break;
				}
				if (!true)
				{
				}
				string content = text;
				tip.enabled = !content.IsNullOrEmpty();
				bool enabled = tip.enabled;
				if (enabled)
				{
					tip.PresetParam = new string[]
					{
						content
					};
				}
			}
		}

		// Token: 0x0600AE7F RID: 44671 RVA: 0x004F82FA File Offset: 0x004F64FA
		public void SetFavoriteStatus(int status)
		{
			this.SetFavoriteStatus((RowItemMain.FavoriteStatus)status);
		}

		// Token: 0x0600AE80 RID: 44672 RVA: 0x004F8308 File Offset: 0x004F6508
		public void SetFavoriteStatus(RowItemMain.FavoriteStatus status)
		{
			CImage cimage = this.loveStatus;
			if (!true)
			{
			}
			string text;
			if (status != RowItemMain.FavoriteStatus.Hate)
			{
				if (status != RowItemMain.FavoriteStatus.Love)
				{
					text = string.Empty;
				}
				else
				{
					text = "ui9_icon_item_state_love_0";
				}
			}
			else
			{
				text = "ui9_icon_item_state_love_1";
			}
			if (!true)
			{
			}
			cimage.SetSprite(text, false, null);
			this.loveStatusGo.SetActive(status != RowItemMain.FavoriteStatus.None);
			TooltipInvoker tip = this.loveStatusGo.GetComponent<TooltipInvoker>();
			bool flag = tip;
			if (flag)
			{
				if (!true)
				{
				}
				if (status != RowItemMain.FavoriteStatus.Hate)
				{
					if (status != RowItemMain.FavoriteStatus.Love)
					{
						text = string.Empty;
					}
					else
					{
						text = LanguageKey.LK_Loving.Tr();
					}
				}
				else
				{
					text = LanguageKey.LK_Hate.Tr();
				}
				if (!true)
				{
				}
				string content = text;
				tip.enabled = !content.IsNullOrEmpty();
				bool enabled = tip.enabled;
				if (enabled)
				{
					tip.PresetParam = new string[]
					{
						content
					};
				}
			}
		}

		// Token: 0x0600AE81 RID: 44673 RVA: 0x004F83E4 File Offset: 0x004F65E4
		public void SetKeepingBookStatus(bool isKeepingBook)
		{
			bool flag = !this.keepingBookStatusGo;
			if (!flag)
			{
				this.keepingBookStatusGo.SetActive(isKeepingBook);
				TooltipInvoker tip = this.keepingBookStatusGo.GetComponent<TooltipInvoker>();
				bool flag2 = tip;
				if (flag2)
				{
					tip.enabled = isKeepingBook;
					if (isKeepingBook)
					{
						tip.PresetParam = new string[]
						{
							LanguageKey.LK_ItemDisplayData_ThreeCorpseKeepingLegendaryBook.Tr()
						};
					}
				}
			}
		}

		// Token: 0x0600AE82 RID: 44674 RVA: 0x004F8454 File Offset: 0x004F6654
		public void SetFavoriteStatus(short lovingItemSubType, short hatingItemSubType)
		{
			short itemSubType = ItemTemplateHelper.GetItemSubType(this.Data.RealKey.ItemType, this.Data.RealKey.TemplateId);
			RowItemMain.FavoriteStatus favoriteStatus = (itemSubType == lovingItemSubType) ? RowItemMain.FavoriteStatus.Love : ((itemSubType == hatingItemSubType) ? RowItemMain.FavoriteStatus.Hate : RowItemMain.FavoriteStatus.None);
			this.SetFavoriteStatus(favoriteStatus);
		}

		// Token: 0x0600AE83 RID: 44675 RVA: 0x004F84A0 File Offset: 0x004F66A0
		public void SetShopItemStatus(ITradeableContent content)
		{
			ItemDisplayData itemData;
			bool flag;
			if (this.isShop)
			{
				itemData = (content as ItemDisplayData);
				flag = (itemData == null);
			}
			else
			{
				flag = true;
			}
			bool flag2 = flag;
			if (flag2)
			{
				this.priceGo.SetActive(false);
				this.extraGo.SetActive(false);
				this.debtGo.SetActive(false);
			}
			else
			{
				this.priceGo.SetActive(itemData.PricePercent != 0);
				bool flag3 = itemData.PricePercent != 0;
				if (flag3)
				{
					this.priceImage.sprite = ((itemData.PricePercent > 0) ? this.priceInc : this.priceDec);
					this.priceTips.PresetParam[0] = ((itemData.PricePercent > 0) ? LanguageKey.LK_Shop_Filter_Price_Detail_Up.TrFormat(itemData.PricePercent) : LanguageKey.LK_Shop_Filter_Price_Detail_Down.TrFormat(-itemData.PricePercent));
				}
				switch (itemData.ExtraGoodsTypeEnum)
				{
				case MerchantExtraGoodsType.Normal:
					this.extraGo.SetActive(true);
					this.extraTips.PresetParam[0] = LanguageKey.LK_Shop_ExtraGoods_Normal_Tip.Tr();
					this.extraImage.sprite = this.normalExtraGoods;
					break;
				case MerchantExtraGoodsType.Capitalist:
					this.extraGo.SetActive(true);
					this.extraTips.PresetParam[0] = LanguageKey.LK_Shop_ExtraGoods_Capitalist_Tip.TrFormat(ProfessionSkill.Instance[63].Name);
					this.extraImage.sprite = this.capitalistExtraGoods;
					break;
				case MerchantExtraGoodsType.Season:
					this.extraGo.SetActive(true);
					this.extraTips.PresetParam[0] = LanguageKey.LK_Shop_ExtraGoods_Season_Tip.TrFormat(LocalStringManager.Get(string.Format("LK_Season_{0}", itemData.SpecialArg)));
					this.extraImage.sprite = this.seasonSprites[itemData.SpecialArg];
					break;
				default:
					this.extraGo.SetActive(false);
					break;
				}
				bool showDebt = (!ShopExchange.IsShopItem(itemData) || this.isShopNeedShowDebt) && !this.isHideDebt;
				int repayLevel = ShopExchange.IsShopItem(itemData) ? ((int)(itemData.ItemSourceType - 10)) : (ShopExchange.IsBuyBackItem(itemData) ? -1 : Math.Min(ShopExchange.GetBaseRepayLevel(itemData.Key), RowItemMain.MaxDebtLevel));
				LocalStringManager.LanguageType curLanguageType = LocalStringManager.CurLanguageType;
				bool flag4 = curLanguageType == LocalStringManager.LanguageType.CN || curLanguageType == LocalStringManager.LanguageType.CNH;
				bool flag5 = ShopExchange.IsBuyItem(itemData);
				if (!true)
				{
				}
				Sprite[] array;
				if (flag4)
				{
					if (!flag5)
					{
						array = this.debtSpritesCnInc;
					}
					else
					{
						array = this.debtSpritesCnDec;
					}
				}
				else if (!flag5)
				{
					array = this.debtSpritesEnInc;
				}
				else
				{
					array = this.debtSpritesEnDec;
				}
				if (!true)
				{
				}
				Sprite[] debtSprites = array;
				bool flag6 = showDebt && debtSprites.CheckIndex(repayLevel) && repayLevel >= (ShopExchange.IsShopItem(itemData) ? (RowItemMain.MinDebtLevel + 1) : RowItemMain.MinDebtShowLevel);
				if (flag6)
				{
					this.debtImage.sprite = debtSprites[repayLevel];
					this.debtGo.SetActive(true);
					this.debtTips.PresetParam[0] = ((content.OwnerCharId == SingletonObject.getInstance<BasicGameData>().TaiwuCharId) ? LanguageKey.LK_Shop_Item_Debt_Repay.Tr() : LanguageKey.LK_Shop_Item_Debt_Add.Tr());
					this.debtTips.enabled = true;
				}
				else
				{
					this.debtGo.SetActive(false);
				}
			}
		}

		// Token: 0x0600AE84 RID: 44676 RVA: 0x004F87F0 File Offset: 0x004F69F0
		public void SetExchangeStatus(ITradeableContent content, ExchangeAdvantage advantageSummary, bool isTaiwuGetItem)
		{
			this.priceGo.SetActive(false);
			this.extraGo.SetActive(false);
			bool flag = ((advantageSummary != null) ? advantageSummary.TaskDict : null) == null;
			if (!flag)
			{
				foreach (int task in advantageSummary.TaskId)
				{
					ExchangeTaskItem cfg = ExchangeTask.Instance[task];
					bool flag2 = !advantageSummary.ConditionMeet(cfg, content, isTaiwuGetItem);
					if (!flag2)
					{
						bool flag3 = cfg.Advantage > 0;
						if (flag3)
						{
							this.priceGo.gameObject.SetActive(true);
							this.priceImage.sprite = this.advantageInc;
							this.priceTips.PresetParam[0] = LanguageKey.LK_Exchange_Advantage_Inc.Tr();
						}
						else
						{
							this.extraGo.gameObject.SetActive(true);
							this.extraImage.sprite = this.advantageDec;
							this.extraTips.PresetParam[0] = LanguageKey.LK_Exchange_Advantage_Dec.Tr();
						}
					}
				}
			}
		}

		// Token: 0x0600AE85 RID: 44677 RVA: 0x004F8904 File Offset: 0x004F6B04
		public void ShowVillagerNeedItem(bool isShow)
		{
			this.villagerNeedMarkTip.gameObject.SetActive(isShow);
			bool flag = !isShow;
			if (!flag)
			{
				this.villagerNeedMarkTip.Type = TipType.VillagerNeedItem;
				ItemKey itemKey = ItemKey.Invalid;
				itemKey.ItemType = this.Data.Key.ItemType;
				itemKey.TemplateId = this.Data.Key.TemplateId;
				this.villagerNeedMarkTip.RuntimeParam = EasyPool.Get<ArgumentBox>().Set<ItemKey>("itemKey", itemKey);
				this.villagerNeedMarkTip.RuntimeParam.Set<ItemKey>("itemKeyReal", this.Data.Key);
			}
		}

		// Token: 0x0600AE86 RID: 44678 RVA: 0x004F89B2 File Offset: 0x004F6BB2
		public void SetLockStatus(bool locked)
		{
			this.lockStatusGo.gameObject.SetActive(locked);
		}

		// Token: 0x0600AE87 RID: 44679 RVA: 0x004F89C8 File Offset: 0x004F6BC8
		public void ShowInteractionStateAttainment(sbyte lifeSkillType, short requiredValue, bool isMeet)
		{
			bool flag = this.interactionState;
			if (flag)
			{
				this.interactionState.SetActive(true);
			}
			LanguageKey key = isMeet ? LanguageKey.LK_Item_Operation_LifeSkill_Require_Meet : LanguageKey.LK_Item_Operation_LifeSkill_Require_Not_Meet;
			string valueStr = requiredValue.ToString().SetColor(isMeet ? "brightblue" : "darkred");
			string content = LocalStringManager.GetFormat(key, LifeSkillType.Instance[lifeSkillType].Name, valueStr);
			bool flag2 = this.textInteractionState;
			if (flag2)
			{
				this.textInteractionState.text = content;
			}
			bool flag3 = this.tipInteractionState;
			if (flag3)
			{
				this.tipInteractionState.enabled = true;
				string[] presetParam = this.tipInteractionState.PresetParam;
				bool flag4 = presetParam == null || presetParam.Length < 1;
				if (flag4)
				{
					this.tipInteractionState.PresetParam = new string[1];
				}
				this.tipInteractionState.PresetParam[0] = content;
			}
		}

		// Token: 0x0600AE88 RID: 44680 RVA: 0x004F8ABC File Offset: 0x004F6CBC
		public void SetItemNotCanSelectReason(string content)
		{
			bool flag = this.interactionState;
			if (flag)
			{
				this.interactionState.SetActive(true);
			}
			bool flag2 = this.textInteractionState;
			if (flag2)
			{
				this.textInteractionState.text = content;
			}
			bool flag3 = this.tipInteractionState;
			if (flag3)
			{
				this.tipInteractionState.enabled = true;
				string[] presetParam = this.tipInteractionState.PresetParam;
				bool flag4 = presetParam == null || presetParam.Length < 1;
				if (flag4)
				{
					this.tipInteractionState.PresetParam = new string[1];
				}
				this.tipInteractionState.PresetParam[0] = content;
			}
		}

		// Token: 0x0600AE89 RID: 44681 RVA: 0x004F8B64 File Offset: 0x004F6D64
		public void HideInteractionState()
		{
			bool flag = this.interactionState;
			if (flag)
			{
				this.interactionState.SetActive(false);
			}
			bool flag2 = this.tipInteractionState;
			if (flag2)
			{
				this.tipInteractionState.enabled = false;
			}
		}

		// Token: 0x0600AE8A RID: 44682 RVA: 0x004F8BAC File Offset: 0x004F6DAC
		public void ShowInteractionStateLocked()
		{
			bool flag = this.interactionState;
			if (flag)
			{
				this.interactionState.SetActive(true);
			}
			string content = LocalStringManager.Get(LanguageKey.LK_Item_Operation_Locked).SetColor("darkred");
			bool flag2 = this.textInteractionState;
			if (flag2)
			{
				this.textInteractionState.text = content;
			}
			bool flag3 = this.tipInteractionState;
			if (flag3)
			{
				this.tipInteractionState.enabled = true;
				string[] presetParam = this.tipInteractionState.PresetParam;
				bool flag4 = presetParam == null || presetParam.Length < 1;
				if (flag4)
				{
					this.tipInteractionState.PresetParam = new string[1];
				}
				this.tipInteractionState.PresetParam[0] = content;
			}
		}

		// Token: 0x0600AE8B RID: 44683 RVA: 0x004F8C6C File Offset: 0x004F6E6C
		public void SetInteractionStateLockText(string content)
		{
			bool flag = this.interactionState;
			if (flag)
			{
				this.interactionState.SetActive(true);
			}
			bool flag2 = this.textInteractionState;
			if (flag2)
			{
				this.textInteractionState.text = content.SetColor("darkred");
			}
			bool flag3 = this.tipInteractionState;
			if (flag3)
			{
				this.tipInteractionState.enabled = true;
				string[] presetParam = this.tipInteractionState.PresetParam;
				bool flag4 = presetParam == null || presetParam.Length < 1;
				if (flag4)
				{
					this.tipInteractionState.PresetParam = new string[1];
				}
				this.tipInteractionState.PresetParam[0] = content.SetColor("darkred");
			}
		}

		// Token: 0x170013C7 RID: 5063
		// (get) Token: 0x0600AE8C RID: 44684 RVA: 0x004F8D28 File Offset: 0x004F6F28
		// (set) Token: 0x0600AE8D RID: 44685 RVA: 0x004F8D30 File Offset: 0x004F6F30
		public ITradeableContent Data { get; private set; }

		// Token: 0x0600AE8E RID: 44686 RVA: 0x004F8D3C File Offset: 0x004F6F3C
		public void SetData(ITradeableContent content)
		{
			bool flag = this.coreValue;
			if (flag)
			{
				this.coreValue.SetActive(false);
			}
			this.Data = content;
			this.itemBack.Set(content, false);
			string name = this.showDurability ? LanguageKey.LK_Name_With_Durability_Format.TrFormat(content.GetName(false), content.Durability, content.MaxDurability).ColorReplace() : content.GetName(false);
			this.SetName(name);
			this.SetShopItemStatus(content);
			this.SetFavoriteStatus(RowItemMain.FavoriteStatus.None);
			ItemDisplayData.ItemUsingType usingType = content.UsingType;
			if (!true)
			{
			}
			RowItemMain.EquipStatus equipStatus2;
			if (usingType != ItemDisplayData.ItemUsingType.EquipmentPlaned)
			{
				if (usingType != ItemDisplayData.ItemUsingType.Equiped)
				{
					equipStatus2 = RowItemMain.EquipStatus.None;
				}
				else
				{
					equipStatus2 = RowItemMain.EquipStatus.Equip;
				}
			}
			else
			{
				equipStatus2 = RowItemMain.EquipStatus.Preset;
			}
			if (!true)
			{
			}
			RowItemMain.EquipStatus equipStatus = equipStatus2;
			this.SetEquipStatus(equipStatus);
			this.SetCricketPresetStatus(content.IsInCurrentCricketPreset);
			ItemDisplayData.ItemUsingType usingType2 = content.UsingType;
			if (!true)
			{
			}
			RowItemMain.ReadingStatus readingStatus2;
			if (usingType2 != ItemDisplayData.ItemUsingType.Reading)
			{
				if (usingType2 != ItemDisplayData.ItemUsingType.Referring)
				{
					readingStatus2 = (content.IsReadingFinished ? RowItemMain.ReadingStatus.Done : RowItemMain.ReadingStatus.None);
				}
				else
				{
					readingStatus2 = RowItemMain.ReadingStatus.Referencing;
				}
			}
			else
			{
				readingStatus2 = RowItemMain.ReadingStatus.Reading;
			}
			if (!true)
			{
			}
			RowItemMain.ReadingStatus readingStatus = readingStatus2;
			this.SetReadingStatus(readingStatus);
			this.SetKeepingBookStatus(content.IsThreeCorpseKeepingLegendaryBook);
			this.SetLockStatus(content.IsLocked);
			this.HideInteractionState();
		}

		// Token: 0x0600AE8F RID: 44687 RVA: 0x004F8E7C File Offset: 0x004F707C
		public void SetCoreData(int resourceType, int value)
		{
			bool flag = !this.coreValue || !this.coreValueIcon || !this.coreValueText || !this.coreValueIcons.CheckIndex(resourceType);
			if (!flag)
			{
				this.coreValue.SetActive(true);
				this.coreValueIcon.sprite = this.coreValueIcons[resourceType];
				this.coreValueText.text = value.ToString();
			}
		}

		// Token: 0x0600AE90 RID: 44688 RVA: 0x004F8EFD File Offset: 0x004F70FD
		public void SetName(string name)
		{
			this.textName.text = name;
			LanguageRuleTips languageRuleTips = this.ruleName;
			if (languageRuleTips != null)
			{
				languageRuleTips.Refresh();
			}
		}

		// Token: 0x0600AE91 RID: 44689 RVA: 0x004F8F20 File Offset: 0x004F7120
		public bool SetDisabled(bool disabled)
		{
			HSVStyleRoot hsvStyleRoot = this.ItemBack.GetComponent<HSVStyleRoot>();
			bool flag = !hsvStyleRoot;
			bool result;
			if (flag)
			{
				result = false;
			}
			else
			{
				if (disabled)
				{
					hsvStyleRoot.SetDefaultGrayAndBlack();
				}
				else
				{
					hsvStyleRoot.SetDefault();
				}
				result = false;
			}
			return result;
		}

		// Token: 0x0600AE92 RID: 44690 RVA: 0x004F8F68 File Offset: 0x004F7168
		public void SetExtendStatus(bool isActive, string spriteName = "")
		{
			bool flag = this.extendGo == null || this.extendImage == null;
			if (!flag)
			{
				bool flag2 = !spriteName.IsNullOrEmpty();
				if (flag2)
				{
					this.extendImage.SetSprite(spriteName, false, null);
				}
				this.extendGo.SetActive(isActive);
			}
		}

		// Token: 0x040086C2 RID: 34498
		public const int ResourceTypeMoney = 6;

		// Token: 0x040086C3 RID: 34499
		public const int ResourceTypeAuthority = 7;

		// Token: 0x040086C4 RID: 34500
		public const int ResourceTypeExp = 8;

		// Token: 0x040086C5 RID: 34501
		public const int ResourceTypeExchangeAdvantage = 9;

		// Token: 0x040086C6 RID: 34502
		public const int ResourceTypeDebt = 10;

		// Token: 0x040086C7 RID: 34503
		public static int MinDebtLevel;

		// Token: 0x040086C8 RID: 34504
		public static int MaxDebtLevel;

		// Token: 0x040086C9 RID: 34505
		public static int MinDebtShowLevel;

		// Token: 0x040086CA RID: 34506
		[SerializeField]
		private bool isShop;

		// Token: 0x040086CB RID: 34507
		[SerializeField]
		private bool isShopNeedShowDebt;

		// Token: 0x040086CC RID: 34508
		[SerializeField]
		private bool isHideDebt;

		// Token: 0x040086CD RID: 34509
		[SerializeField]
		private CImage equipStatus;

		// Token: 0x040086CE RID: 34510
		[SerializeField]
		private CImage readingStatus;

		// Token: 0x040086CF RID: 34511
		[SerializeField]
		private CImage loveStatus;

		// Token: 0x040086D0 RID: 34512
		[SerializeField]
		private CImage lockStatus;

		// Token: 0x040086D1 RID: 34513
		[SerializeField]
		private GameObject equipStatusGo;

		// Token: 0x040086D2 RID: 34514
		[SerializeField]
		private GameObject readingStatusGo;

		// Token: 0x040086D3 RID: 34515
		[SerializeField]
		private GameObject loveStatusGo;

		// Token: 0x040086D4 RID: 34516
		[SerializeField]
		private GameObject lockStatusGo;

		// Token: 0x040086D5 RID: 34517
		[SerializeField]
		private GameObject keepingBookStatusGo;

		// Token: 0x040086D6 RID: 34518
		[SerializeField]
		private GameObject cricketPresetStatusGo;

		// Token: 0x040086D7 RID: 34519
		[SerializeField]
		private TooltipInvoker warningTip;

		// Token: 0x040086D8 RID: 34520
		[SerializeField]
		private ItemBack itemBack;

		// Token: 0x040086D9 RID: 34521
		[SerializeField]
		private TextMeshProUGUI textName;

		// Token: 0x040086DA RID: 34522
		[SerializeField]
		private LanguageRuleTips ruleName;

		// Token: 0x040086DB RID: 34523
		[SerializeField]
		private GameObject interactionState;

		// Token: 0x040086DC RID: 34524
		[SerializeField]
		private TextMeshProUGUI textInteractionState;

		// Token: 0x040086DD RID: 34525
		[SerializeField]
		private TooltipInvoker tipInteractionState;

		// Token: 0x040086DE RID: 34526
		[Tooltip("以 物品名(当前耐久/最大耐久) 的形式显示物品名")]
		[SerializeField]
		private bool showDurability;

		// Token: 0x040086DF RID: 34527
		[Tooltip("卡片模式下物品核心属性")]
		[CanBeNull]
		[SerializeField]
		private GameObject coreValue;

		// Token: 0x040086E0 RID: 34528
		[CanBeNull]
		[SerializeField]
		private CImage coreValueIcon;

		// Token: 0x040086E1 RID: 34529
		[CanBeNull]
		[SerializeField]
		private TMP_Text coreValueText;

		// Token: 0x040086E2 RID: 34530
		[SerializeField]
		private Sprite[] coreValueIcons;

		// Token: 0x040086E3 RID: 34531
		[SerializeField]
		private GameObject priceGo;

		// Token: 0x040086E4 RID: 34532
		[SerializeField]
		private GameObject extraGo;

		// Token: 0x040086E5 RID: 34533
		[SerializeField]
		private GameObject debtGo;

		// Token: 0x040086E6 RID: 34534
		[SerializeField]
		private CImage priceImage;

		// Token: 0x040086E7 RID: 34535
		[SerializeField]
		private CImage extraImage;

		// Token: 0x040086E8 RID: 34536
		[SerializeField]
		private CImage debtImage;

		// Token: 0x040086E9 RID: 34537
		[SerializeField]
		private TooltipInvoker priceTips;

		// Token: 0x040086EA RID: 34538
		[SerializeField]
		private TooltipInvoker extraTips;

		// Token: 0x040086EB RID: 34539
		[SerializeField]
		private TooltipInvoker debtTips;

		// Token: 0x040086EC RID: 34540
		[SerializeField]
		private Sprite priceInc;

		// Token: 0x040086ED RID: 34541
		[SerializeField]
		private Sprite priceDec;

		// Token: 0x040086EE RID: 34542
		[SerializeField]
		private Sprite normalExtraGoods;

		// Token: 0x040086EF RID: 34543
		[SerializeField]
		private Sprite capitalistExtraGoods;

		// Token: 0x040086F0 RID: 34544
		[SerializeField]
		private Sprite advantageInc;

		// Token: 0x040086F1 RID: 34545
		[SerializeField]
		private Sprite advantageDec;

		// Token: 0x040086F2 RID: 34546
		[SerializeField]
		private Sprite[] debtSpritesCnInc;

		// Token: 0x040086F3 RID: 34547
		[SerializeField]
		private Sprite[] debtSpritesEnInc;

		// Token: 0x040086F4 RID: 34548
		[SerializeField]
		private Sprite[] debtSpritesCnDec;

		// Token: 0x040086F5 RID: 34549
		[SerializeField]
		private Sprite[] debtSpritesEnDec;

		// Token: 0x040086F6 RID: 34550
		[SerializeField]
		private Sprite[] seasonSprites;

		// Token: 0x040086F7 RID: 34551
		[SerializeField]
		private TooltipInvoker villagerNeedMarkTip;

		// Token: 0x040086F9 RID: 34553
		[SerializeField]
		private GameObject extendGo;

		// Token: 0x040086FA RID: 34554
		[SerializeField]
		private CImage extendImage;

		// Token: 0x02002527 RID: 9511
		public enum EquipStatus : sbyte
		{
			// Token: 0x0400E72A RID: 59178
			None = -1,
			// Token: 0x0400E72B RID: 59179
			Preset,
			// Token: 0x0400E72C RID: 59180
			Equip
		}

		// Token: 0x02002528 RID: 9512
		public enum ReadingStatus : sbyte
		{
			// Token: 0x0400E72E RID: 59182
			None = -1,
			// Token: 0x0400E72F RID: 59183
			Reading,
			// Token: 0x0400E730 RID: 59184
			Referencing,
			// Token: 0x0400E731 RID: 59185
			Done
		}

		// Token: 0x02002529 RID: 9513
		public enum FavoriteStatus : sbyte
		{
			// Token: 0x0400E733 RID: 59187
			None = -1,
			// Token: 0x0400E734 RID: 59188
			Hate,
			// Token: 0x0400E735 RID: 59189
			Love
		}
	}
}
