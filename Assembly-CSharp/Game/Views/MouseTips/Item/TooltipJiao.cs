using System;
using System.IO;
using System.Text;
using Config;
using FrameWork;
using Game.Views.MouseTips.Item.Common;
using GameData.DLC.FiveLoong;
using GameData.Domains.Character.Display;
using GameData.Domains.Item;
using GameData.Domains.Item.Display;
using UnityEngine;

namespace Game.Views.MouseTips.Item
{
	// Token: 0x020008A3 RID: 2211
	public class TooltipJiao : TooltipItemBase
	{
		// Token: 0x17000C8F RID: 3215
		// (get) Token: 0x060069B9 RID: 27065 RVA: 0x0030A5F6 File Offset: 0x003087F6
		protected override bool CanStick
		{
			get
			{
				bool result;
				if (UIManager.Instance.CheckPopupElementIsInTop(UIElement.CharacterMenuEquip))
				{
					ItemDisplayData itemData = this._itemData;
					result = (itemData != null && itemData.UsingType == ItemDisplayData.ItemUsingType.Equiped);
				}
				else
				{
					result = true;
				}
				return result;
			}
		}

		// Token: 0x060069BA RID: 27066 RVA: 0x0030A628 File Offset: 0x00308828
		protected override void Init(ArgumentBox argsBox)
		{
			argsBox.Get("HideLoongView", out this._isHideLoongView);
			argsBox.Get("IsInCompareUI", out this._isInCompareUI);
			bool flag = argsBox.Get<ItemDisplayData>("ItemData", out this._itemData);
			if (flag)
			{
				this._displayData = this._itemData.JiaoLoongDisplayData;
			}
			else
			{
				bool flag2 = argsBox.Get<JiaoLoongDisplayData>("JiaoLoongData", out this._displayData);
				if (flag2)
				{
					this._itemData = this._displayData.ItemDisplayData;
				}
			}
			bool flag3 = this._displayData == null || this._itemData == null;
			if (!flag3)
			{
				bool flag4 = !argsBox.Get<CarrierMaxProperty>("CarrierMaxProperty", out this._maxProperty);
				if (flag4)
				{
					this._maxProperty = null;
				}
				bool flag5 = !argsBox.Get("EquipSlot", out this._slot);
				if (flag5)
				{
					this._slot = -1;
				}
				GameObject gameObject = this.carrier.gameObject;
				int slot = this._slot;
				gameObject.SetActive(slot == 11 || slot == 12);
				this.beast.gameObject.SetActive(this._slot == 13);
				this._itemKey = this._itemData.RealKey;
				this._isYouth = (this._itemData.Key.ItemType != 4);
				this._isLoong = !this._displayData.IsJiao;
				this._jiao = this._displayData.Jiao;
				this._loong = this._displayData.Loong;
				bool isYouth = this._isYouth;
				if (isYouth)
				{
					this._miscConfigData = Config.Material.Instance[this._itemData.Key.TemplateId];
					this.InnatePoisons = this._miscConfigData.InnatePoisons;
					this._carrierConfigData = null;
				}
				else
				{
					this._miscConfigData = null;
					this._carrierConfigData = Carrier.Instance[this._itemData.Key.TemplateId];
					this.InnatePoisons = this._carrierConfigData.InnatePoisons;
				}
				bool flag6 = this._isHideLoongView || this._isInCompareUI;
				if (flag6)
				{
					this.rectTransLoong.gameObject.SetActive(false);
				}
				base.Init(argsBox);
				base.PostInit();
				this.Refresh();
			}
		}

		// Token: 0x060069BB RID: 27067 RVA: 0x0030A872 File Offset: 0x00308A72
		public override void SetNewData(ArgumentBox argsBox)
		{
			this.Init(argsBox);
			this.Refresh();
		}

		// Token: 0x060069BC RID: 27068 RVA: 0x0030A884 File Offset: 0x00308A84
		public override void Refresh()
		{
			bool flag = this._displayData == null || this._itemData == null;
			if (!flag)
			{
				base.Refresh();
				this.RefreshBaseProperty();
				this.RefreshCarrierProperty();
				this.RefreshGrowthValue();
				this.RefreshSpecial();
				this.RefreshDetail();
				this.RefreshLoongView(this._carrierConfigData);
				this.tameArea.Refresh(this._itemData, this._slot, this._templateDataOnly, this._isLoong, this._isYouth);
				UIElement element = this.Element;
				if (element != null)
				{
					element.ShowAfterRefresh();
				}
			}
		}

		// Token: 0x060069BD RID: 27069 RVA: 0x0030A91F File Offset: 0x00308B1F
		protected override void OnDisable()
		{
			base.OnDisable();
			this.rectTransLoong.gameObject.SetActive(false);
		}

		// Token: 0x060069BE RID: 27070 RVA: 0x0030A93C File Offset: 0x00308B3C
		protected override void RefreshCommonArea()
		{
			string name = this._isLoong ? this._loong.GetNameText() : this._jiao.GetNameText();
			string desc = this._isYouth ? this._miscConfigData.Desc.ColorReplace() : this._carrierConfigData.Desc.ColorReplace();
			string funcDesc = this._isYouth ? this._miscConfigData.FunctionDesc.ColorReplace() : this._carrierConfigData.FunctionDesc.ColorReplace();
			sbyte grade = this._isYouth ? this._miscConfigData.Grade : this._carrierConfigData.Grade;
			string icon = this._isYouth ? this._miscConfigData.Icon : this._carrierConfigData.Icon;
			string itemType = CommonUtils.GetItemTypeName(this._isYouth ? this._miscConfigData.ItemType : this._carrierConfigData.ItemType);
			string value = this._isLoong ? this._loong.Properties.Get(this._loong.JiaoTemplateId, this._displayData.TemplateId, 6).ToString() : this._jiao.Properties.Get(this._jiao.TemplateId, 6).ToString();
			this.commonArea.Refresh(this._itemData, name, desc, funcDesc, grade, icon, itemType, value);
		}

		// Token: 0x060069BF RID: 27071 RVA: 0x0030AAA8 File Offset: 0x00308CA8
		private void RefreshBaseProperty()
		{
			string behaviorStr = CommonUtils.GetBehaviorString(this._isLoong ? this._loong.Behavior : this._jiao.Behavior);
			this.propertyBehavior.Set("", LanguageKey.LK_Main_SummaryInfo_Behavior.Tr(), behaviorStr, true);
			this.propertyBehavior.gameObject.SetActive(true);
			this.propertyGender.gameObject.SetActive(!this._isLoong);
			this.propertyHeight.gameObject.SetActive(!this._isLoong);
			this.propertyWeight.gameObject.SetActive(!this._isLoong);
			this.propertyLifespan.gameObject.SetActive(!this._isLoong);
			this.propertyGeneration.gameObject.SetActive(!this._isLoong);
			bool isLoong = this._isLoong;
			if (!isLoong)
			{
				string genderStr = CommonUtils.GetJiaoGenderString(this._jiao.Gender);
				this.propertyGender.Set("", LanguageKey.LK_Main_SummaryInfo_Gender.Tr(), genderStr, true);
				string heightStr = LanguageKey.LK_Jiao_Height.TrFormat(this._jiao.GetPresentProperty(9)).SetColor("brightyellow");
				this.propertyHeight.Set("", LanguageKey.LK_Height.Tr(), heightStr, true);
				string weightStr = LanguageKey.LK_Jiao_Weight.TrFormat(this._jiao.GetPresentProperty(10)).SetColor("brightyellow");
				this.propertyWeight.Set("", LanguageKey.LK_Mousetip_Jiao_Weight.Tr(), weightStr, true);
				string lifespanStr = LanguageKey.LK_Jiao_LifeSpan.TrFormat(this._jiao.GetPresentProperty(11)).SetColor("brightyellow");
				this.propertyLifespan.Set("", LanguageKey.LK_LifeSpan.Tr(), lifespanStr, true);
				string generationStr = LanguageKey.LK_Generation_Content.TrFormat(this._jiao.Generation + 1).SetColor("brightyellow");
				this.propertyGeneration.Set("", LanguageKey.LK_Generation_Title.Tr(), generationStr, true);
			}
		}

		// Token: 0x060069C0 RID: 27072 RVA: 0x0030ACE0 File Offset: 0x00308EE0
		private void RefreshCarrierProperty()
		{
			int travelSpeed = this._isLoong ? this._loong.Properties.Get(this._loong.JiaoTemplateId, this._displayData.TemplateId, 0) : this._jiao.Properties.Get(this._jiao.TemplateId, 0);
			int inventory = this._isLoong ? this._loong.Properties.Get(this._loong.JiaoTemplateId, this._displayData.TemplateId, 1) : this._jiao.Properties.Get(this._jiao.TemplateId, 1);
			int dropRate = this._isLoong ? this._loong.Properties.Get(this._loong.JiaoTemplateId, this._displayData.TemplateId, 2) : this._jiao.Properties.Get(this._jiao.TemplateId, 2);
			int captureRate = this._isLoong ? this._loong.Properties.Get(this._loong.JiaoTemplateId, this._displayData.TemplateId, 3) : this._jiao.Properties.Get(this._jiao.TemplateId, 3);
			int captives = this._isLoong ? this._loong.Properties.Get(this._loong.JiaoTemplateId, this._displayData.TemplateId, 4) : this._jiao.Properties.Get(this._jiao.TemplateId, 4);
			int exploreBonusRate = this._isLoong ? this._loong.Properties.Get(this._loong.JiaoTemplateId, this._displayData.TemplateId, 5) : this._jiao.Properties.Get(this._jiao.TemplateId, 5);
			int happiness = this._isLoong ? this._loong.Properties.Get(this._loong.JiaoTemplateId, this._displayData.TemplateId, 7) : this._jiao.Properties.Get(this._jiao.TemplateId, 7);
			int favorability = this._isLoong ? this._loong.Properties.Get(this._loong.JiaoTemplateId, this._displayData.TemplateId, 8) : this._jiao.Properties.Get(this._jiao.TemplateId, 8);
			int value = this._isLoong ? this._loong.Properties.Get(this._loong.JiaoTemplateId, this._displayData.TemplateId, 6) : this._jiao.Properties.Get(this._jiao.TemplateId, 6);
			this.propertyDropRate.gameObject.SetActive(dropRate > 0);
			this.propertyDropRate.Set("", LanguageKey.LK_ItemTips_Carrier_Add_DropRate.Tr(), this.GetEffectText(dropRate, 2, true, false), true);
			DisableStyleRoot styleRoot = this.propertyDropRate.StyleRoot;
			if (styleRoot != null)
			{
				styleRoot.SetInteractable(this._slot != 13 && (this._maxProperty == null || this._maxProperty.WorkingCarrierDropBonus <= dropRate));
			}
			this.propertyExploreBonusRate.gameObject.SetActive(exploreBonusRate > 0);
			this.propertyExploreBonusRate.Set("", LanguageKey.LK_ItemTips_Carrier_Add_ExploreBonusRate.Tr(), this.GetEffectText(exploreBonusRate, 5, true, false), true);
			DisableStyleRoot styleRoot2 = this.propertyExploreBonusRate.StyleRoot;
			if (styleRoot2 != null)
			{
				styleRoot2.SetInteractable(this._slot != 13 && (this._maxProperty == null || this._maxProperty.WorkingCarrierExploreBonusRate <= exploreBonusRate));
			}
			this.propertyCaptureRate.gameObject.SetActive(captureRate > 0);
			this.propertyCaptureRate.Set("", LanguageKey.LK_ItemTips_Carrier_Add_CaptureRate.Tr(), this.GetEffectText(captureRate, 3, true, false), true);
			DisableStyleRoot styleRoot3 = this.propertyCaptureRate.StyleRoot;
			if (styleRoot3 != null)
			{
				styleRoot3.SetInteractable(this._slot != 13 && (this._maxProperty == null || this._maxProperty.WorkingCarrierCaptureRateBonus <= captureRate));
			}
			this.propertyInventory.gameObject.SetActive(inventory > 0);
			this.propertyInventory.Set("", LanguageKey.LK_ItemTips_Carrier_Add_Inventory.Tr(), this.GetEffectText(inventory, 1, false, true), true);
			DisableStyleRoot styleRoot4 = this.propertyInventory.StyleRoot;
			if (styleRoot4 != null)
			{
				styleRoot4.SetInteractable(this._slot != 13 && (this._maxProperty == null || this._maxProperty.WorkingCarrierMaxInventoryLoadBonus <= inventory));
			}
			this.propertyTravelSpeed.gameObject.SetActive(travelSpeed > 0);
			this.propertyTravelSpeed.Set("", LanguageKey.LK_ItemTips_Carrier_Add_TravelSpeed.Tr(), this.GetEffectText(-travelSpeed, 0, true, false), true);
			DisableStyleRoot styleRoot5 = this.propertyTravelSpeed.StyleRoot;
			if (styleRoot5 != null)
			{
				styleRoot5.SetInteractable(this._slot != 13 && (this._maxProperty == null || this._maxProperty.WorkingCarrierTimeBonus <= travelSpeed));
			}
			this.propertyCaptives.gameObject.SetActive(captives > 0);
			this.propertyCaptives.Set("", LanguageKey.LK_Captives_Limit.Tr(), this.GetEffectText(captives, 4, false, false), true);
			DisableStyleRoot styleRoot6 = this.propertyCaptives.StyleRoot;
			if (styleRoot6 != null)
			{
				styleRoot6.SetInteractable(this._slot != 13 && (this._maxProperty == null || this._maxProperty.WorkingCarrierKidnapMaxSlotCount <= captives));
			}
			this.propertyHappiness.gameObject.SetActive(happiness > 0);
			this.propertyHappiness.Set("", LanguageKey.LK_Happiness_Bouns.Tr(), this.GetEffectText(happiness, 7, false, false), true);
			this.propertyFavorability.gameObject.SetActive(favorability > 0);
			this.propertyFavorability.Set("", LanguageKey.LK_Favorable_Bouns.Tr(), this.GetEffectText(favorability, 8, false, false), true);
			this.propertyValueEffect.gameObject.SetActive(value > 0);
			this.propertyValueEffect.Set("", LanguageKey.LK_ItemValue.Tr(), this.GetEffectText(value, 6, false, false), true);
			bool isAnyShow = this.propertyDropRate.gameObject.activeSelf || this.propertyExploreBonusRate.gameObject.activeSelf || this.propertyCaptureRate.gameObject.activeSelf || this.propertyInventory.gameObject.activeSelf || this.propertyTravelSpeed.gameObject.activeSelf || this.propertyCaptives.gameObject.activeSelf || this.propertyHappiness.gameObject.activeSelf || this.propertyFavorability.gameObject.activeSelf || this.propertyValueEffect.gameObject.activeSelf;
			this.rootBaseProperty.SetActive(isAnyShow);
		}

		// Token: 0x060069C1 RID: 27073 RVA: 0x0030B408 File Offset: 0x00309608
		private string GetEffectText(int value, int index, bool isPercentage, bool isFloat)
		{
			int maxValue = (index == 0) ? (-Config.JiaoProperty.Instance[index].MaxValue) : Config.JiaoProperty.Instance[index].MaxValue;
			bool show = value != 0 && maxValue != 0;
			bool flag = !show;
			string result;
			if (flag)
			{
				result = string.Empty;
			}
			else
			{
				bool isLoong = this._isLoong;
				if (isLoong)
				{
					string prefix = (value > 0) ? "+" : "";
					if (isPercentage)
					{
						result = string.Format("{0}{1}%", prefix, value).SetColor("brightblue");
					}
					else if (isFloat)
					{
						result = string.Format("{0}{1:f1}", prefix, (float)value / 100f).SetColor("brightblue");
					}
					else
					{
						result = string.Format("{0}{1}", prefix, value).SetColor("brightblue");
					}
				}
				else
				{
					float percentage = (float)value / (float)maxValue;
					StringBuilder sb = EasyPool.Get<StringBuilder>();
					bool flag2 = percentage >= 1f;
					if (flag2)
					{
						if (isPercentage)
						{
							sb.Append((value > 0) ? (LanguageKey.LK_Mousetip_Jiao_Max.TrFormat(value) + "%").SetColor("brightblue") : (LanguageKey.LK_Mousetip_Jiao_Max_Minus.TrFormat(value) + "%").SetColor("brightblue"));
						}
						else if (isFloat)
						{
							string floatValue = string.Format("{0:f1}", (float)value / 100f);
							sb.Append((value > 0) ? LanguageKey.LK_Mousetip_Jiao_Max.TrFormat(floatValue).SetColor("brightblue") : LanguageKey.LK_Mousetip_Jiao_Max_Minus.TrFormat(floatValue).SetColor("brightblue"));
						}
						else
						{
							sb.Append((value > 0) ? LanguageKey.LK_Mousetip_Jiao_Max.TrFormat(value).SetColor("brightblue") : LanguageKey.LK_Mousetip_Jiao_Max_Minus.TrFormat(value).SetColor("brightblue"));
						}
					}
					else
					{
						bool flag3 = (double)percentage >= 0.75;
						if (flag3)
						{
							if (isPercentage)
							{
								sb.Append((value > 0) ? (LanguageKey.LK_Mousetip_Jiao_Excellent.TrFormat(value) + "%").SetColor("yellow") : (LanguageKey.LK_Mousetip_Jiao_Excellent_Minus.TrFormat(value) + "%").SetColor("yellow"));
							}
							else if (isFloat)
							{
								string floatValue2 = string.Format("{0:f1}", (float)value / 100f);
								sb.Append((value > 0) ? LanguageKey.LK_Mousetip_Jiao_Excellent.TrFormat(floatValue2).SetColor("yellow") : LanguageKey.LK_Mousetip_Jiao_Excellent_Minus.TrFormat(floatValue2).SetColor("yellow"));
							}
							else
							{
								sb.Append((value > 0) ? LanguageKey.LK_Mousetip_Jiao_Excellent.TrFormat(value).SetColor("yellow") : LanguageKey.LK_Mousetip_Jiao_Excellent_Minus.TrFormat(value).SetColor("yellow"));
							}
						}
						else
						{
							string prefix2 = (value > 0) ? "+" : "";
							if (isPercentage)
							{
								sb.Append(string.Format("{0}{1}%", prefix2, value).SetColor("brightred"));
							}
							else if (isFloat)
							{
								sb.Append(string.Format("{0}{1:f1}", prefix2, (float)value / 100f).SetColor("brightred"));
							}
							else
							{
								sb.Append(string.Format("{0}{1}", prefix2, value).SetColor("brightred"));
							}
						}
					}
					if (isPercentage)
					{
						sb.Append((" / " + maxValue.ToString() + "%").SetColor("pinkyellow"));
					}
					else if (isFloat)
					{
						string floatValue3 = string.Format("{0:f1}", (float)maxValue / 100f);
						sb.Append((" / " + floatValue3).SetColor("pinkyellow"));
					}
					else
					{
						sb.Append((" / " + maxValue.ToString()).SetColor("pinkyellow"));
					}
					string content = sb.ToString();
					EasyPool.Free<StringBuilder>(sb);
					result = content;
				}
			}
			return result;
		}

		// Token: 0x060069C2 RID: 27074 RVA: 0x0030B888 File Offset: 0x00309A88
		private void RefreshGrowthValue()
		{
			bool isShow = !this._isLoong && this._isYouth;
			this.rootGrowthValue.SetActive(isShow);
			bool flag = !isShow;
			if (!flag)
			{
				this._maxGrowthMonth = (int)JiaoNurturance.Instance[this._jiao.NurturanceTemplateId].NurturanceCostMonth;
				int hasGrowthMonth = this._maxGrowthMonth - this._jiao.EvolveRemainingMonth;
				string color = (this._maxGrowthMonth != hasGrowthMonth) ? "brightred" : "brightblue";
				string content = string.Format("{0}/{1}", hasGrowthMonth.ToString().SetColor(color), this._maxGrowthMonth);
				this.propertyGrowthValue.SetValue(content);
			}
		}

		// Token: 0x060069C3 RID: 27075 RVA: 0x0030B93C File Offset: 0x00309B3C
		private void RefreshSpecial()
		{
			bool showCanBreed = !this._isLoong && !this._isYouth;
			this.propertyCanBreed.gameObject.SetActive(showCanBreed);
			bool flag = showCanBreed;
			if (flag)
			{
				string canBreed = LocalStringManager.GetFormat(this._jiao.CanBreed ? LanguageKey.LK_Nurturing : LanguageKey.LK_Cannot_Nurturing, Array.Empty<object>()).SetColor(this._jiao.CanBreed ? "brightblue" : "brightred");
				this.propertyCanBreed.SetValue(canBreed);
			}
			bool showCanHualong = !this._isLoong && !this._isYouth;
			this.propertyCanHualong.gameObject.SetActive(showCanHualong);
			bool flag2 = showCanHualong;
			if (flag2)
			{
				string canHualong = LocalStringManager.GetFormat((this._displayData.EvolutionChoice > 0) ? LanguageKey.LK_Hualong : LanguageKey.LK_Cannot_Hualong, Array.Empty<object>()).SetColor((this._displayData.EvolutionChoice > 0) ? "brightblue" : "brightred");
				this.propertyCanHualong.SetValue(canHualong);
			}
			this.rootState.SetActive(showCanBreed && showCanHualong);
		}

		// Token: 0x060069C4 RID: 27076 RVA: 0x0030BA5C File Offset: 0x00309C5C
		private void RefreshDetail()
		{
			bool hasTame = !this._templateDataOnly && !this._isYouth && ItemTemplateHelper.HasCarrierTame(this._carrierConfigData.ItemType, this._carrierConfigData.TemplateId);
			this.propertyCombatState.gameObject.SetActive(hasTame);
			bool flag = hasTame;
			if (flag)
			{
				CombatStateItem combatStateConfig = CombatState.Instance[this._carrierConfigData.CombatState];
				this.propertyCombatState.Set("", combatStateConfig.Name, combatStateConfig.Desc.ColorReplace(), true);
			}
			this.propertyHualongNotice.gameObject.SetActive(!this._isLoong);
			this.propertyHualongNotice.SetValue(LanguageKey.LK_Mousetip_Jiao_Effect_Notice.Tr());
		}

		// Token: 0x060069C5 RID: 27077 RVA: 0x0030BB1C File Offset: 0x00309D1C
		protected override void InitItemDisableFunctionList()
		{
			base.InitItemDisableFunctionList();
			bool isYouth = this._isYouth;
			if (isYouth)
			{
				MaterialItem configData = this._miscConfigData;
				bool flag = !configData.Repairable;
				if (flag)
				{
					this._disableFunctionList.Add(ItemFunction.Repairable);
				}
				bool flag2 = !configData.Transferable;
				if (flag2)
				{
					this._disableFunctionList.Add(ItemFunction.Transferable);
				}
				bool flag3 = !configData.Poisonable;
				if (flag3)
				{
					this._disableFunctionList.Add(ItemFunction.Poisonable);
				}
				bool flag4 = !configData.Refinable;
				if (flag4)
				{
					this._disableFunctionList.Add(ItemFunction.Refinable);
				}
			}
			else
			{
				CarrierItem configData2 = this._carrierConfigData;
				bool flag5 = !configData2.Repairable;
				if (flag5)
				{
					this._disableFunctionList.Add(ItemFunction.Repairable);
				}
				bool flag6 = !configData2.Transferable;
				if (flag6)
				{
					this._disableFunctionList.Add(ItemFunction.Transferable);
				}
				bool flag7 = !configData2.Poisonable;
				if (flag7)
				{
					this._disableFunctionList.Add(ItemFunction.Poisonable);
				}
				bool flag8 = !configData2.Refinable;
				if (flag8)
				{
					this._disableFunctionList.Add(ItemFunction.Refinable);
				}
			}
		}

		// Token: 0x060069C6 RID: 27078 RVA: 0x0030BC30 File Offset: 0x00309E30
		private void RefreshLoongView(CarrierItem carrierItem)
		{
			bool flag = carrierItem == null || carrierItem.StandDisplay.IsNullOrEmpty() || this._isHideLoongView || this._isInCompareUI;
			if (flag)
			{
				this.rectTransLoong.gameObject.SetActive(false);
			}
			else
			{
				ResLoader.Load<Texture2D>(Path.Combine("RemakeResources/Textures/NpcFace/BigFace", carrierItem.StandDisplay), delegate(Texture2D texture)
				{
					bool flag2 = null == this;
					if (!flag2)
					{
						this.imageLoong.texture = texture;
						this.rectTransLoong.gameObject.SetActive(true);
					}
				}, null, false);
			}
		}

		// Token: 0x04004BF9 RID: 19449
		[Header("基础属性")]
		[SerializeField]
		private TooltipItemProperty propertyGender;

		// Token: 0x04004BFA RID: 19450
		[SerializeField]
		private TooltipItemProperty propertyBehavior;

		// Token: 0x04004BFB RID: 19451
		[SerializeField]
		private TooltipItemProperty propertyHeight;

		// Token: 0x04004BFC RID: 19452
		[SerializeField]
		private TooltipItemProperty propertyWeight;

		// Token: 0x04004BFD RID: 19453
		[SerializeField]
		private TooltipItemProperty propertyLifespan;

		// Token: 0x04004BFE RID: 19454
		[SerializeField]
		private TooltipItemProperty propertyGeneration;

		// Token: 0x04004BFF RID: 19455
		[Header("代步属性")]
		[SerializeField]
		private GameObject rootBaseProperty;

		// Token: 0x04004C00 RID: 19456
		[SerializeField]
		private TooltipItemProperty propertyDropRate;

		// Token: 0x04004C01 RID: 19457
		[SerializeField]
		private TooltipItemProperty propertyExploreBonusRate;

		// Token: 0x04004C02 RID: 19458
		[SerializeField]
		private TooltipItemProperty propertyCaptureRate;

		// Token: 0x04004C03 RID: 19459
		[SerializeField]
		private TooltipItemProperty propertyInventory;

		// Token: 0x04004C04 RID: 19460
		[SerializeField]
		private TooltipItemProperty propertyTravelSpeed;

		// Token: 0x04004C05 RID: 19461
		[SerializeField]
		private TooltipItemProperty propertyCaptives;

		// Token: 0x04004C06 RID: 19462
		[SerializeField]
		private TooltipItemProperty propertyHappiness;

		// Token: 0x04004C07 RID: 19463
		[SerializeField]
		private TooltipItemProperty propertyFavorability;

		// Token: 0x04004C08 RID: 19464
		[SerializeField]
		private TooltipItemProperty propertyValueEffect;

		// Token: 0x04004C09 RID: 19465
		[Header("驱使野兽")]
		[SerializeField]
		private TooltipCarrierTameArea tameArea;

		// Token: 0x04004C0A RID: 19466
		[Header("特殊状态")]
		[SerializeField]
		private GameObject rootState;

		// Token: 0x04004C0B RID: 19467
		[SerializeField]
		private TooltipItemProperty propertyCanBreed;

		// Token: 0x04004C0C RID: 19468
		[SerializeField]
		private TooltipItemProperty propertyCanHualong;

		// Token: 0x04004C0D RID: 19469
		[Header("养育属性")]
		[SerializeField]
		private GameObject rootGrowthValue;

		// Token: 0x04004C0E RID: 19470
		[SerializeField]
		private TooltipItemProperty propertyGrowthValue;

		// Token: 0x04004C0F RID: 19471
		[Header("详细模式")]
		[SerializeField]
		private TooltipItemProperty propertyCombatState;

		// Token: 0x04004C10 RID: 19472
		[SerializeField]
		private TooltipItemProperty propertyHualongNotice;

		// Token: 0x04004C11 RID: 19473
		[Header("立绘")]
		[SerializeField]
		private RectTransform rectTransLoong;

		// Token: 0x04004C12 RID: 19474
		[SerializeField]
		private CRawImage imageLoong;

		// Token: 0x04004C13 RID: 19475
		private JiaoLoongDisplayData _displayData;

		// Token: 0x04004C14 RID: 19476
		private MaterialItem _miscConfigData;

		// Token: 0x04004C15 RID: 19477
		private CarrierItem _carrierConfigData;

		// Token: 0x04004C16 RID: 19478
		private int _maxGrowthMonth;

		// Token: 0x04004C17 RID: 19479
		private bool _isYouth;

		// Token: 0x04004C18 RID: 19480
		private bool _isLoong;

		// Token: 0x04004C19 RID: 19481
		private GameData.DLC.FiveLoong.Jiao _jiao;

		// Token: 0x04004C1A RID: 19482
		private ChildrenOfLoong _loong;

		// Token: 0x04004C1B RID: 19483
		[SerializeField]
		private TooltipItemProperty carrier;

		// Token: 0x04004C1C RID: 19484
		[SerializeField]
		private TooltipItemProperty beast;

		// Token: 0x04004C1D RID: 19485
		private CarrierMaxProperty _maxProperty;

		// Token: 0x04004C1E RID: 19486
		private bool _isHideLoongView;

		// Token: 0x04004C1F RID: 19487
		private const string _loongNpcFacePath = "RemakeResources/Textures/NpcFace/BigFace";

		// Token: 0x04004C20 RID: 19488
		private int _slot;
	}
}
