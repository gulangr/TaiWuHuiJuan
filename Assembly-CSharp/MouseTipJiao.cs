using System;
using System.IO;
using System.Text;
using Config;
using FrameWork;
using GameData.DLC.FiveLoong;
using GameData.Domains.Item;
using GameData.Domains.Item.Display;
using TMPro;
using UnityEngine;

// Token: 0x020002A4 RID: 676
public class MouseTipJiao : MouseTipItem
{
	// Token: 0x1700049B RID: 1179
	// (get) Token: 0x06002A37 RID: 10807 RVA: 0x0014200B File Offset: 0x0014020B
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

	// Token: 0x06002A38 RID: 10808 RVA: 0x0014203C File Offset: 0x0014023C
	protected override void Init(ArgumentBox argsBox)
	{
		base.Init(argsBox);
		argsBox.Get("HideLoongView", out this._isHideLoongView);
		argsBox.Get("IsInCompareUI", out this._isInCompareUI);
		argsBox.Get<JiaoLoongDisplayData>("JiaoLoongData", out this._displayData);
		bool disableCompare;
		argsBox.Get("DisableCompare", out disableCompare);
		MoreInfo2 moreInfo;
		this._canDisplayCompareInfo = (!disableCompare && this.CTryGet<MoreInfo2>("MoreInfo2", out moreInfo));
		this._itemData = this._displayData.ItemDisplayData;
		this._isYouth = (this._itemData.Key.ItemType != 4);
		this._isLoong = !this._displayData.IsJiao;
		this._jiao = this._displayData.Jiao;
		this._loong = this._displayData.Loong;
		this.InitLoongView();
		bool flag = this._isHideLoongView || this._isInCompareUI;
		if (flag)
		{
			this._loongViewRect.gameObject.SetActive(false);
		}
		base.CGet<GameObject>("DriveBeast").SetActive(!this._isYouth);
		base.CGet<GameObject>("Durability").SetActive(!this._isYouth);
		base.CGet<GameObject>("NourishProperty").SetActive(this._isYouth);
		base.CGet<GameObject>("Growth").SetActive(this._isYouth);
		base.CGet<GameObject>("Gender").SetActive(!this._isLoong);
		base.CGet<GameObject>("Height").SetActive(!this._isLoong);
		base.CGet<GameObject>("Weight").SetActive(!this._isLoong);
		base.CGet<GameObject>("Lifespan").SetActive(!this._isLoong);
		base.CGet<GameObject>("GenerationRoot").SetActive(!this._isLoong);
		base.CGet<GameObject>("CanBreed").SetActive(!this._isLoong && !this._isYouth);
		base.CGet<GameObject>("CanHualong").SetActive(!this._isLoong && !this._isYouth);
		bool isYouth = this._isYouth;
		if (isYouth)
		{
			base.gameObject.GetComponent<CImage>().SetSprite("mousetip_di_0", false, null);
			this._miscConfigData = Config.Material.Instance[this._itemData.Key.TemplateId];
			this._carrierConfigData = null;
		}
		else
		{
			base.gameObject.GetComponent<CImage>().SetSprite("mousetip_di_1", false, null);
			this._miscConfigData = null;
			this._carrierConfigData = Carrier.Instance[this._itemData.Key.TemplateId];
		}
		this.InitItemDisableFunctionList(this._itemData);
		base.RefreshDisableFunction();
		this.SetDisplayData();
		this.SetBasicProperty();
		this.SetNourishProperty();
		this.SetCarrierEffect();
		this.SetDrivenBeast();
		this.RefreshLoongView(this._carrierConfigData);
		base.ForceRebuildLayout(2U, null);
		base.PostInit();
	}

	// Token: 0x06002A39 RID: 10809 RVA: 0x00142350 File Offset: 0x00140550
	protected override void OnDisable()
	{
		base.OnDisable();
		GameObject driveBeast = base.CGet<GameObject>("DriveBeast");
		bool activeSelf = driveBeast.activeSelf;
		if (activeSelf)
		{
			driveBeast.SetActive(false);
		}
		base.CGet<RectTransform>("LoongImageView").gameObject.SetActive(false);
	}

	// Token: 0x06002A3A RID: 10810 RVA: 0x0014239C File Offset: 0x0014059C
	private void Update()
	{
		bool canDisplayCompareInfo = this._canDisplayCompareInfo;
		if (canDisplayCompareInfo)
		{
			base.UpdateCompareCommonPart();
			base.UpdateMoreInfoCtrl();
		}
	}

	// Token: 0x06002A3B RID: 10811 RVA: 0x001423C4 File Offset: 0x001405C4
	private void SetDisplayData()
	{
		TextMeshProUGUI currDurabilityYellow = base.CGet<TextMeshProUGUI>("CurrDurabilityYellow");
		TextMeshProUGUI currDurabilityRed = base.CGet<TextMeshProUGUI>("CurrDurabilityRed");
		base.CGet<TextMeshProUGUI>("Name").text = (this._isLoong ? this._loong.GetNameText() : this._jiao.GetNameText());
		base.CGet<CImage>("GradeBack").SetSprite(ItemView.GetGradeIcon(this._isYouth ? this._miscConfigData.Grade : this._carrierConfigData.Grade), false, null);
		base.CGet<TextMeshProUGUI>("GradeName").text = LocalStringManager.Get(string.Format("LK_ShortGrade_{0}", this._isYouth ? this._miscConfigData.Grade : this._carrierConfigData.Grade));
		base.CGet<TextMeshProUGUI>("Grade").text = (LocalStringManager.Get(string.Format("LK_Num_{0}", (int)(9 - (this._isYouth ? this._miscConfigData.Grade : this._carrierConfigData.Grade)))) + LocalStringManager.Get(LanguageKey.LK_Item_Grade)).SetColor(Colors.Instance.GradeColors[(int)(this._isYouth ? this._miscConfigData.Grade : this._carrierConfigData.Grade)]);
		base.CGet<CImage>("ItemIcon").SetSprite(this._isYouth ? this._miscConfigData.Icon : this._carrierConfigData.Icon, false, null);
		base.SetItemDesc(this._isYouth ? this._miscConfigData.Desc : this._carrierConfigData.Desc, this._itemData.LoveTokenDataItem);
		base.CGet<TextMeshProUGUI>("WeightText").text = NumberFormatUtils.FormatItemWeight(this._itemData.Weight);
		base.CGet<TextMeshProUGUI>("SubType").text = LocalStringManager.Get(string.Format("LK_ItemSubType_{0}", this._isYouth ? this._miscConfigData.ItemSubType : this._carrierConfigData.ItemSubType));
		base.CGet<TextMeshProUGUI>("Type").text = LocalStringManager.Get(string.Format("LK_ItemType_{0}", this._isYouth ? this._miscConfigData.ItemType : this._carrierConfigData.ItemType));
		base.CGet<TextMeshProUGUI>("JiaoType").text = (this._isYouth ? this._miscConfigData.Name : this._carrierConfigData.Name);
		bool isLoong = this._isLoong;
		if (isLoong)
		{
			base.CGet<TextMeshProUGUI>("Value").text = this._loong.Properties.Get(this._loong.JiaoTemplateId, this._displayData.TemplateId, 6).ToString();
		}
		else
		{
			base.CGet<TextMeshProUGUI>("Value").text = this._jiao.Properties.Get(this._jiao.TemplateId, 6).ToString();
		}
		bool hasHalfDurability = this._itemData.Durability > this._itemData.MaxDurability / 2;
		currDurabilityYellow.gameObject.SetActive(hasHalfDurability);
		currDurabilityRed.gameObject.SetActive(!hasHalfDurability);
		(hasHalfDurability ? currDurabilityYellow : currDurabilityRed).text = this._itemData.Durability.ToString();
		base.CGet<TextMeshProUGUI>("MaxDurability").text = string.Format("/{0}", this._itemData.MaxDurability);
		PoisonsAndLevels poison = this._isYouth ? this._miscConfigData.InnatePoisons : this._carrierConfigData.InnatePoisons;
		base.RefreshPoisons(poison, this._itemData);
	}

	// Token: 0x06002A3C RID: 10812 RVA: 0x00142798 File Offset: 0x00140998
	private void SetBasicProperty()
	{
		base.CGet<TextMeshProUGUI>("SubType").text = LocalStringManager.Get(string.Format("LK_ItemSubType_{0}", this._isYouth ? this._miscConfigData.ItemSubType : this._carrierConfigData.ItemSubType));
		bool isLoong = this._isLoong;
		if (isLoong)
		{
			base.CGet<TextMeshProUGUI>("BehaviorText").text = LocalStringManager.Get(string.Format("LK_Goodness_{0}", this._loong.Behavior)).ColorReplace();
		}
		else
		{
			bool gender = this._jiao.Gender;
			if (gender)
			{
				base.CGet<TextMeshProUGUI>("GenderText").text = LocalStringManager.Get("LK_Animal_Male").ColorReplace();
			}
			else
			{
				base.CGet<TextMeshProUGUI>("GenderText").text = LocalStringManager.Get("LK_Animal_Female").ColorReplace();
			}
			base.CGet<TextMeshProUGUI>("BehaviorText").text = LocalStringManager.Get(string.Format("LK_Goodness_{0}", this._jiao.Behavior)).ColorReplace();
			base.CGet<TextMeshProUGUI>("JiaoHeight").text = LocalStringManager.GetFormat("LK_Jiao_Height", this._jiao.GetPresentProperty(9));
			base.CGet<TextMeshProUGUI>("JiaoWeight").text = LocalStringManager.GetFormat("LK_Jiao_Weight", this._jiao.GetPresentProperty(10));
			base.CGet<TextMeshProUGUI>("LifespanText").text = LocalStringManager.GetFormat("LK_Jiao_LifeSpan", this._jiao.GetPresentProperty(11));
			base.CGet<TextMeshProUGUI>("CanBreedText").text = LocalStringManager.GetFormat(this._jiao.CanBreed ? LanguageKey.LK_Nurturing : LanguageKey.LK_Cannot_Nurturing, Array.Empty<object>()).SetColor(this._jiao.CanBreed ? "brightblue" : "brightred");
			base.CGet<TextMeshProUGUI>("Generation").text = LocalStringManager.GetFormat(LanguageKey.LK_Generation_Content, this._jiao.Generation + 1);
		}
	}

	// Token: 0x06002A3D RID: 10813 RVA: 0x001429BC File Offset: 0x00140BBC
	private void SetNourishProperty()
	{
		bool flag = this._isLoong || !this._isYouth;
		if (!flag)
		{
			bool isYouth = this._isYouth;
			if (isYouth)
			{
				StringBuilder strBuilder = new StringBuilder();
				this._maxGrowthMonth = (int)JiaoNurturance.Instance[this._jiao.NurturanceTemplateId].NurturanceCostMonth;
				int hasGrowthMonth = this._maxGrowthMonth - this._jiao.EvolveRemainingMonth;
				bool flag2 = this._maxGrowthMonth != hasGrowthMonth;
				if (flag2)
				{
					strBuilder.Append(hasGrowthMonth.ToString().SetColor("brightred"));
				}
				else
				{
					strBuilder.Append(hasGrowthMonth.ToString().SetColor("brightblue"));
				}
				strBuilder.Append("/" + this._maxGrowthMonth.ToString());
				base.CGet<TextMeshProUGUI>("GrowthValue").text = strBuilder.ToString();
			}
		}
	}

	// Token: 0x06002A3E RID: 10814 RVA: 0x00142AA8 File Offset: 0x00140CA8
	private void SetCarrierEffect()
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
		bool isLoong = this._isLoong;
		if (isLoong)
		{
			base.CGet<GameObject>("TravelSpeed").SetActive(travelSpeed > 0);
			bool flag = travelSpeed > 0;
			if (flag)
			{
				base.CGet<TextMeshProUGUI>("AddTravelSpeed").text = string.Format("-{0}%", travelSpeed);
			}
			base.CGet<GameObject>("Inventory").SetActive(inventory > 0);
			bool flag2 = inventory > 0;
			if (flag2)
			{
				base.CGet<TextMeshProUGUI>("AddInventory").text = string.Format("+{0:f1}", (float)inventory / 100f);
			}
			base.CGet<GameObject>("DropRate").SetActive(dropRate > 0);
			bool flag3 = dropRate > 0;
			if (flag3)
			{
				base.CGet<TextMeshProUGUI>("AddDropRate").text = string.Format("+{0}%", dropRate);
			}
			base.CGet<GameObject>("CaptureRate").SetActive(captureRate > 0);
			base.CGet<TextMeshProUGUI>("AddCaptureRate").text = string.Format("+{0}%", captureRate);
			base.CGet<GameObject>("Captives").SetActive(captives > 0);
			base.CGet<TextMeshProUGUI>("AddCaptives").text = string.Format("+{0}", captives);
			base.CGet<GameObject>("ExploreBonusRate").SetActive(exploreBonusRate > 0);
			base.CGet<TextMeshProUGUI>("AddExploreBonusRate").text = string.Format("+{0}%", exploreBonusRate);
			base.CGet<GameObject>("Happiness").SetActive(happiness > 0);
			bool flag4 = happiness > 0;
			if (flag4)
			{
				base.CGet<TextMeshProUGUI>("HappinessChange").text = string.Format("+{0}", happiness);
			}
			base.CGet<GameObject>("Favorability").SetActive(favorability > 0);
			bool flag5 = favorability > 0;
			if (flag5)
			{
				base.CGet<TextMeshProUGUI>("FavorabilityChange").text = string.Format("+{0}", favorability);
			}
			base.CGet<GameObject>("ValueEffect").SetActive(value > 0);
			base.CGet<TextMeshProUGUI>("AddValue").text = string.Format("+{0}", value);
		}
		else
		{
			this.SetEffectText("TravelSpeed", "AddTravelSpeed", -travelSpeed, 0, true, false);
			this.SetEffectText("Inventory", "AddInventory", inventory, 1, false, true);
			this.SetEffectText("DropRate", "AddDropRate", dropRate, 2, true, false);
			this.SetEffectText("CaptureRate", "AddCaptureRate", captureRate, 3, true, false);
			this.SetEffectText("Captives", "AddCaptives", captives, 4, false, false);
			this.SetEffectText("ExploreBonusRate", "AddExploreBonusRate", exploreBonusRate, 5, true, false);
			this.SetEffectText("Happiness", "HappinessChange", happiness, 7, false, false);
			this.SetEffectText("Favorability", "FavorabilityChange", favorability, 8, false, false);
			this.SetEffectText("ValueEffect", "AddValue", value, 6, false, false);
		}
		base.CGet<GameObject>("Space").SetActive(!this._isLoong);
		base.CGet<GameObject>("HualongNotice").SetActive(!this._isLoong);
		base.CGet<TextMeshProUGUI>("HualongNoticeText").text = LocalStringManager.Get(LanguageKey.LK_Mousetip_Jiao_Effect_Notice).ColorReplace();
		base.CGet<TextMeshProUGUI>("HualongNoticeText").GetComponent<TMPTextSpriteHelper>().Parse();
		base.CGet<TextMeshProUGUI>("CanHualongText").text = LocalStringManager.GetFormat((this._displayData.EvolutionChoice > 0) ? LanguageKey.LK_Hualong : LanguageKey.LK_Cannot_Hualong, Array.Empty<object>()).SetColor((this._displayData.EvolutionChoice > 0) ? "brightblue" : "brightred");
	}

	// Token: 0x06002A3F RID: 10815 RVA: 0x00143140 File Offset: 0x00141340
	private void SetDrivenBeast()
	{
		base.CGet<GameObject>("Drive").SetActive(!this._isYouth);
		bool isYouth = this._isYouth;
		if (isYouth)
		{
			base.CGet<GameObject>("DriveBeast").SetActive(true);
			this.SetTameText(this._displayData.TamePoint, this._displayData.MaxTamePoint);
			base.CGet<GameObject>("CombatState").SetActive(false);
		}
		else
		{
			bool hasTame = ItemTemplateHelper.HasCarrierTame(this._carrierConfigData.ItemType, this._carrierConfigData.TemplateId);
			base.CGet<GameObject>("DriveBeast").SetActive(hasTame);
			base.CGet<GameObject>("CombatState").SetActive(hasTame);
			base.CGet<GameObject>("Tame").SetActive(hasTame);
			bool flag = hasTame;
			if (flag)
			{
				base.CGet<TextMeshProUGUI>("DriveTips").text = LocalStringManager.GetFormat(this._isLoong ? LanguageKey.LK_MouseTips_CarrierDriveBeastTip : LanguageKey.LK_MouseTips_Jiao_CarrierDriveBeastTip, this._carrierConfigData.Name, CombatState.Instance[this._carrierConfigData.CombatState].Name);
				base.CGet<TextMeshProUGUI>("CombatStateText").text = CombatState.Instance[this._carrierConfigData.CombatState].Desc.ColorReplace();
				this.SetTameText(this._displayData.TamePoint, this._displayData.MaxTamePoint);
			}
		}
	}

	// Token: 0x06002A40 RID: 10816 RVA: 0x001432B4 File Offset: 0x001414B4
	private void SetTameText(int value, int maxValue)
	{
		StringBuilder sb = new StringBuilder();
		sb.Append("<color=#" + ((value == 100) ? "brightblue" : "brightred") + ">");
		sb.Append((value == -1) ? 0 : value);
		sb.Append("</color>");
		sb.Append(string.Format("<color=#pinkyellow>/{0}</color>", maxValue));
		base.CGet<TextMeshProUGUI>("TameValue").text = sb.ToString().ColorReplace();
	}

	// Token: 0x06002A41 RID: 10817 RVA: 0x00143340 File Offset: 0x00141540
	private void SetEffectText(string gameObejctName, string textObjectName, int value, int index, bool isPercentage, bool isFloat)
	{
		int maxValue = (index == 0) ? (-Config.JiaoProperty.Instance[index].MaxValue) : Config.JiaoProperty.Instance[index].MaxValue;
		bool show = value != 0 && maxValue != 0;
		base.CGet<GameObject>(gameObejctName).SetActive(show);
		bool flag = !show;
		if (!flag)
		{
			float percentage = (float)value / (float)maxValue;
			StringBuilder sb = new StringBuilder();
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
					string prefix = (value > 0) ? "+" : "";
					if (isPercentage)
					{
						sb.Append(string.Format("{0}{1}%", prefix, value).SetColor("brightred"));
					}
					else if (isFloat)
					{
						sb.Append(string.Format("{0}{1:f1}", prefix, (float)value / 100f).SetColor("brightred"));
					}
					else
					{
						sb.Append(string.Format("{0}{1}", prefix, value).SetColor("brightred"));
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
			base.CGet<TextMeshProUGUI>(textObjectName).text = sb.ToString().ColorReplace();
			base.CGet<TextMeshProUGUI>(textObjectName).GetComponent<TMPTextSpriteHelper>().Parse();
		}
	}

	// Token: 0x06002A42 RID: 10818 RVA: 0x00143740 File Offset: 0x00141940
	protected override void InitItemDisableFunctionList(ItemDisplayData itemDisplayData)
	{
		base.InitItemDisableFunctionList(itemDisplayData);
		bool isYouth = this._isYouth;
		if (isYouth)
		{
			MaterialItem configData = this._miscConfigData;
			bool flag = !configData.Repairable;
			if (flag)
			{
				this._disableFunctionList.Add(MouseTipItem.ItemFunction.Repairable);
			}
			bool flag2 = !configData.Transferable;
			if (flag2)
			{
				this._disableFunctionList.Add(MouseTipItem.ItemFunction.Transferable);
			}
			bool flag3 = !configData.Poisonable;
			if (flag3)
			{
				this._disableFunctionList.Add(MouseTipItem.ItemFunction.Poisonable);
			}
			bool flag4 = !configData.Refinable;
			if (flag4)
			{
				this._disableFunctionList.Add(MouseTipItem.ItemFunction.Refinable);
			}
		}
		else
		{
			CarrierItem configData2 = this._carrierConfigData;
			bool flag5 = !configData2.Repairable;
			if (flag5)
			{
				this._disableFunctionList.Add(MouseTipItem.ItemFunction.Repairable);
			}
			bool flag6 = !configData2.Transferable;
			if (flag6)
			{
				this._disableFunctionList.Add(MouseTipItem.ItemFunction.Transferable);
			}
			bool flag7 = !configData2.Poisonable;
			if (flag7)
			{
				this._disableFunctionList.Add(MouseTipItem.ItemFunction.Poisonable);
			}
			bool flag8 = !configData2.Refinable;
			if (flag8)
			{
				this._disableFunctionList.Add(MouseTipItem.ItemFunction.Refinable);
			}
		}
	}

	// Token: 0x06002A43 RID: 10819 RVA: 0x00143858 File Offset: 0x00141A58
	private void RefreshLoongView(CarrierItem carrierItem)
	{
		bool flag = carrierItem == null || carrierItem.StandDisplay.IsNullOrEmpty() || this._isHideLoongView || this._isInCompareUI;
		if (flag)
		{
			this._loongViewRect.gameObject.SetActive(false);
			this.TipAdditionSizeX = 0f;
		}
		else
		{
			ResLoader.Load<Texture2D>(Path.Combine("RemakeResources/Textures/NpcFace/BigFace", carrierItem.StandDisplay), delegate(Texture2D texture)
			{
				bool flag2 = null == this;
				if (!flag2)
				{
					base.CGet<CRawImage>("LoongImage").texture = texture;
					this.SetLoongViewRight();
					this._loongViewRect.gameObject.SetActive(true);
					this.TipAdditionSizeX = this._loongViewRect.sizeDelta.x;
				}
			}, null, false);
		}
	}

	// Token: 0x06002A44 RID: 10820 RVA: 0x001438D0 File Offset: 0x00141AD0
	private void SetLoongViewRight()
	{
		this._loongViewRect.pivot = Vector2.up;
		this._loongViewRect.anchorMin = Vector2.one;
		this._loongViewRect.anchorMax = Vector2.one;
		this._loongViewRect.anchoredPosition = new Vector2(4f, -50f);
	}

	// Token: 0x06002A45 RID: 10821 RVA: 0x0014392C File Offset: 0x00141B2C
	private void SetLoongViewLeft()
	{
		this._loongViewRect.pivot = Vector2.one;
		this._loongViewRect.anchorMin = Vector2.up;
		this._loongViewRect.anchorMax = Vector2.up;
		this._loongViewRect.anchoredPosition = new Vector2(-4f, -50f);
	}

	// Token: 0x06002A46 RID: 10822 RVA: 0x00143988 File Offset: 0x00141B88
	private void InitLoongView()
	{
		this._loongViewRect = base.CGet<RectTransform>("LoongImageView");
		this.SetLoongViewRight();
		this.OnUpdatePos = delegate(bool showOnLeft)
		{
			bool flag = !this._loongViewRect.gameObject.activeSelf;
			if (!flag)
			{
				if (showOnLeft)
				{
					this.SetLoongViewLeft();
				}
				else
				{
					this.SetLoongViewRight();
				}
			}
		};
	}

	// Token: 0x06002A47 RID: 10823 RVA: 0x001439B5 File Offset: 0x00141BB5
	public override void SetNewData(ArgumentBox argsBox)
	{
		this.Init(argsBox);
	}

	// Token: 0x04001E9C RID: 7836
	private JiaoLoongDisplayData _displayData;

	// Token: 0x04001E9D RID: 7837
	private MaterialItem _miscConfigData;

	// Token: 0x04001E9E RID: 7838
	private CarrierItem _carrierConfigData;

	// Token: 0x04001E9F RID: 7839
	private int _maxGrowthMonth;

	// Token: 0x04001EA0 RID: 7840
	private bool _isYouth;

	// Token: 0x04001EA1 RID: 7841
	private bool _isLoong;

	// Token: 0x04001EA2 RID: 7842
	private GameData.DLC.FiveLoong.Jiao _jiao;

	// Token: 0x04001EA3 RID: 7843
	private ChildrenOfLoong _loong;

	// Token: 0x04001EA4 RID: 7844
	private bool _canDisplayCompareInfo;

	// Token: 0x04001EA5 RID: 7845
	private bool _isHideLoongView;

	// Token: 0x04001EA6 RID: 7846
	private const string _loongNpcFacePath = "RemakeResources/Textures/NpcFace/BigFace";

	// Token: 0x04001EA7 RID: 7847
	private RectTransform _loongViewRect;
}
