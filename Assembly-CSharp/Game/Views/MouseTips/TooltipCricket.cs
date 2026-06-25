using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text;
using Config;
using FrameWork;
using Game.Views.Cricket;
using Game.Views.Cricket.Combat;
using Game.Views.MouseTips.Common;
using Game.Views.MouseTips.Item.Common;
using GameData.Combat.Cricket;
using GameData.Domains.Item;
using GameData.Domains.Item.Display;
using GameData.Serializer;
using GameData.Utilities;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Game.Views.MouseTips
{
	// Token: 0x0200088F RID: 2191
	public class TooltipCricket : MouseTipBase
	{
		// Token: 0x17000C83 RID: 3203
		// (get) Token: 0x06006903 RID: 26883 RVA: 0x00303A1E File Offset: 0x00301C1E
		protected override bool CanStick
		{
			get
			{
				return true;
			}
		}

		// Token: 0x06006904 RID: 26884 RVA: 0x00303A24 File Offset: 0x00301C24
		protected override void Init(ArgumentBox argsBox)
		{
			ItemDisplayData itemData;
			argsBox.Get<ItemDisplayData>("ItemData", out itemData);
			argsBox.Get("TemplateDataOnly", out this._templateDataOnly);
			this._itemData = itemData;
			this._colorConfig = CricketParts.Instance[this._itemData.CricketColorId];
			this._partConfig = CricketParts.Instance[this._itemData.CricketPartId];
			this._isCombineCricket = (this._itemData.CricketPartId > 0);
			UIElement element = this.Element;
			element.OnListenerIdReady = (Action)Delegate.Combine(element.OnListenerIdReady, new Action(this.OnListenerIdReady));
		}

		// Token: 0x06006905 RID: 26885 RVA: 0x00303ACC File Offset: 0x00301CCC
		private void OnListenerIdReady()
		{
			bool flag = !this._templateDataOnly;
			if (flag)
			{
				ItemDomainMethod.AsyncCall.GetCricketData(this, this._itemData.Key.Id, new AsyncMethodCallbackDelegate(this.OnGetCricketData));
			}
			else
			{
				this.ForceRebuildLayout(2U);
			}
		}

		// Token: 0x06006906 RID: 26886 RVA: 0x00303B18 File Offset: 0x00301D18
		private void OnGetCricketData(int offset, RawDataPool dataPool)
		{
			bool flag = this == null;
			if (!flag)
			{
				Serializer.Deserialize(dataPool, offset, ref this._cricketData);
				this.Refresh();
			}
		}

		// Token: 0x06006907 RID: 26887 RVA: 0x00303B48 File Offset: 0x00301D48
		public override void Refresh()
		{
			bool flag = this._itemData == null;
			if (!flag)
			{
				this.RefreshDlcOnlyObjects();
				bool flag2 = this._cricketData != null;
				if (flag2)
				{
					this.RefreshConfigOnlyInfo();
					this.RefreshDataDependentInfo();
				}
				else
				{
					this.RefreshConfigOnlyInfo();
				}
				this.ForceRebuildLayout(2U);
			}
		}

		// Token: 0x06006908 RID: 26888 RVA: 0x00303B9C File Offset: 0x00301D9C
		private void ForceRebuildLayout(uint delayFrame = 2U)
		{
			bool flag = this == null;
			if (!flag)
			{
				SingletonObject.getInstance<YieldHelper>().DelayFrameDo(delayFrame, delegate
				{
					LayoutRebuilder.ForceRebuildLayoutImmediate(base.GetComponent<RectTransform>());
				});
			}
		}

		// Token: 0x06006909 RID: 26889 RVA: 0x00303BD0 File Offset: 0x00301DD0
		private void RefreshDlcOnlyObjects()
		{
			bool dlcEnabled = CricketPolymorphHelper.IsCricketPolymorphEnabled;
			for (int i = 0; i < this.polymorphOnlyObjects.Length; i++)
			{
				this.polymorphOnlyObjects[i].SetActive(dlcEnabled);
			}
		}

		// Token: 0x0600690A RID: 26890 RVA: 0x00303C0C File Offset: 0x00301E0C
		private void RefreshConfigOnlyInfo()
		{
			bool flag = this._itemData == null;
			if (!flag)
			{
				sbyte grade = new ValueTuple<short, short>(this._itemData.CricketColorId, this._itemData.CricketPartId).CalcCricketGrade();
				this.nameText.text = this._itemData.CalcCricketName().SetGradeColor((int)grade);
				string gradeStr = LocalStringManager.Get(string.Format("LK_Cricket_Grade_Name{0}", grade));
				string gradeShortStr = LocalStringManager.Get(string.Format("LK_ShortGrade_{0}", grade));
				this.gradeText.text = LanguageKey.LK_Cricket_Tip_Grade_Format.TrFormat(gradeShortStr, gradeStr).SetGradeColor((int)grade);
				short itemSubType = ItemTemplateHelper.GetItemSubType(this._itemData.Key.ItemType, this._itemData.Key.TemplateId);
				this.typeText.text = CommonUtils.GetItemSubTypeName(itemSubType);
				string ageStr = LanguageKey.LK_Age.TrFormat(this._colorConfig.Life);
				this.ageText.text = LocalStringManager.GetFormat(LanguageKey.LK_MouseTip_Circket_Age_Content, "-", ageStr);
				this.valueText.text = this._itemData.Value.ToString();
				this.weightText.text = NumberFormatUtils.FormatItemWeight(this._itemData.Weight);
				this.durabilityText.text = string.Format("{0}/{1}", this._itemData.Durability, this._itemData.MaxDurability);
				string desc = (this._isCombineCricket ? this._partConfig : this._colorConfig).Desc;
				MouseTip_Util.SetMultiLineAutoHeightText(this.descText, desc);
				this.gradeBackImage.SetSprite(string.Format("{0}{1}", "ui9_mousetip_base_level_", grade), false, null);
				this.cricketView.SetCricketData(this._itemData.CricketColorId, this._itemData.CricketPartId, false, null, false);
				this.cricketView.gameObject.SetActive(true);
				this.RefreshPropertyFromConfig();
				this.spiritText.text = "0";
				this.RefreshAllAffixTexts();
				this.RefreshSpiritPropertyConfig();
				this.loseCountText.text = "-";
				this.winCountText.text = "-";
				this.lastEnemyText.text = "-";
				this.statusArea.SetActive(false);
				this.otherArea.gameObject.SetActive(false);
				this.detailRightArea.SetActive(false);
				this.operationArea.RefreshPressToDetail();
				this.operationArea.RefreshHotkeyDisplayViewEncyclopedia(false);
				this.RefreshDetailMode();
			}
		}

		// Token: 0x0600690B RID: 26891 RVA: 0x00303EC0 File Offset: 0x003020C0
		private void RefreshPropertyFromConfig()
		{
			List<ValueTuple<string, string, string>> properties = this.GetPropertyListFromConfig();
			bool hasProperty = properties.Count > 0;
			this.propertyContainer.parent.gameObject.SetActive(hasProperty);
			this.propertyTemplateItem.gameObject.SetActive(hasProperty);
			CommonUtils.PrepareEnoughChildren(this.propertyContainer, this.propertyTemplateItem.gameObject, properties.Count, null);
			for (int i = 0; i < properties.Count; i++)
			{
				ValueTuple<string, string, string> valueTuple = properties[i];
				string icon = valueTuple.Item1;
				string title = valueTuple.Item2;
				string value = valueTuple.Item3;
				this.propertyContainer.GetChild(i).GetComponent<TooltipItemProperty>().Set(icon, title, value, true);
			}
		}

		// Token: 0x0600690C RID: 26892 RVA: 0x00303F84 File Offset: 0x00302184
		[return: TupleElementNames(new string[]
		{
			"icon",
			"title",
			"value"
		})]
		private List<ValueTuple<string, string, string>> GetPropertyListFromConfig()
		{
			List<ValueTuple<string, string, string>> result = new List<ValueTuple<string, string, string>>();
			CricketCore baseCore = CricketCoreUtils.BuildCricketBasePropertyForTip(this._itemData);
			for (int i = 0; i < TooltipCricket.MainProperties.Length; i++)
			{
				ECricketCombatPropertyType type = TooltipCricket.MainProperties[i];
				List<ValueTuple<string, string, string>> list = result;
				string str = "ui9_icon_mousetip_cricket_property_big_";
				int num = (int)type;
				list.Add(new ValueTuple<string, string, string>(str + num.ToString(), TooltipCricket.GetMainPropertyTitle(type), baseCore.GetProperty(type).ToString()));
			}
			return result;
		}

		// Token: 0x0600690D RID: 26893 RVA: 0x00304004 File Offset: 0x00302204
		private void RefreshDataDependentInfo()
		{
			bool flag = this._cricketData == null;
			if (!flag)
			{
				this.valueText.text = this._cricketData.CricketValue.ToString();
				this.ageText.text = LocalStringManager.GetFormat(LanguageKey.LK_MouseTip_Circket_Age_Content, this._cricketData.AgeStr, this._cricketData.MaxAge);
				this.RefreshPropertyFromData();
				this.spiritText.text = this._cricketData.Spirit.ToString();
				this.RefreshSpiritProperty(this.IsDetailMode());
				this.RefreshCombatRecord();
				this.RefreshStatusArea();
				this.otherArea.Refresh(TooltipCricket.CricketDisableFunctions);
				this.RefreshSpiritDesc();
				this.RefreshAllAffixTexts();
				this.RefreshDetailMode();
			}
		}

		// Token: 0x0600690E RID: 26894 RVA: 0x003040D8 File Offset: 0x003022D8
		private void RefreshPropertyFromData()
		{
			bool detailMode = this.IsDetailMode();
			List<ValueTuple<string, string, string>> properties = this.GetPropertyListFromData(detailMode);
			bool hasProperty = properties.Count > 0;
			this.propertyContainer.parent.gameObject.SetActive(hasProperty);
			this.propertyTemplateItem.gameObject.SetActive(hasProperty);
			CommonUtils.PrepareEnoughChildren(this.propertyContainer, this.propertyTemplateItem.gameObject, properties.Count, null);
			for (int i = 0; i < properties.Count; i++)
			{
				ValueTuple<string, string, string> valueTuple = properties[i];
				string icon = valueTuple.Item1;
				string title = valueTuple.Item2;
				string value = valueTuple.Item3;
				this.propertyContainer.GetChild(i).GetComponent<TooltipItemProperty>().Set(icon, title, value, true);
			}
		}

		// Token: 0x0600690F RID: 26895 RVA: 0x003041AC File Offset: 0x003023AC
		[return: TupleElementNames(new string[]
		{
			"icon",
			"title",
			"value"
		})]
		private List<ValueTuple<string, string, string>> GetPropertyListFromData(bool detailMode)
		{
			List<ValueTuple<string, string, string>> result = new List<ValueTuple<string, string, string>>();
			CricketCore baseCore = CricketCoreUtils.BuildCricketBasePropertyForTip(this._itemData);
			for (int i = 0; i < TooltipCricket.MainProperties.Length; i++)
			{
				ECricketCombatPropertyType type = TooltipCricket.MainProperties[i];
				int baseValue = baseCore.GetProperty(type);
				int spiritBonus = this.GetSpiritPropertyBonus(type);
				int injury = this.GetInjuryValue(type);
				int finalValue = Mathf.Max(baseValue + spiritBonus - injury, 0);
				string valueText = TooltipCricket.FormatDetailValue(baseValue, spiritBonus, injury, finalValue, detailMode);
				List<ValueTuple<string, string, string>> list = result;
				string str = "ui9_icon_mousetip_cricket_property_big_";
				int num = (int)type;
				list.Add(new ValueTuple<string, string, string>(str + num.ToString(), TooltipCricket.GetMainPropertyTitle(type), valueText));
			}
			return result;
		}

		// Token: 0x06006910 RID: 26896 RVA: 0x0030425C File Offset: 0x0030245C
		private int GetSpiritPropertyBonus(ECricketCombatPropertyType type)
		{
			CricketData cricketData = this._cricketData;
			int? num;
			if (cricketData == null)
			{
				num = null;
			}
			else
			{
				CricketSpiritProperty spiritAddProperties = cricketData.SpiritAddProperties;
				if (spiritAddProperties == null)
				{
					num = null;
				}
				else
				{
					Dictionary<ECricketCombatPropertyType, int> propertyAddValues = spiritAddProperties.PropertyAddValues;
					num = ((propertyAddValues != null) ? new int?(propertyAddValues.GetOrDefault(type)) : null);
				}
			}
			int? num2 = num;
			return num2.GetValueOrDefault();
		}

		// Token: 0x06006911 RID: 26897 RVA: 0x003042C0 File Offset: 0x003024C0
		private int GetInjuryValue(ECricketCombatPropertyType type)
		{
			if (!true)
			{
			}
			int result;
			switch (type)
			{
			case ECricketCombatPropertyType.Hp:
				result = (int)this._cricketData.InjuryHp;
				break;
			case ECricketCombatPropertyType.Sp:
				result = (int)this._cricketData.InjurySp;
				break;
			case ECricketCombatPropertyType.Vigor:
				result = (int)this._cricketData.InjuryVigor;
				break;
			case ECricketCombatPropertyType.Strength:
				result = (int)this._cricketData.InjuryStrength;
				break;
			case ECricketCombatPropertyType.Bite:
				result = (int)this._cricketData.InjuryBite;
				break;
			default:
				result = 0;
				break;
			}
			if (!true)
			{
			}
			return result;
		}

		// Token: 0x06006912 RID: 26898 RVA: 0x00304344 File Offset: 0x00302544
		private static string FormatDetailValue(int baseValue, int bonus, int injury, int finalValue, bool detailMode)
		{
			bool flag = !detailMode;
			string result;
			if (flag)
			{
				result = finalValue.ToString();
			}
			else
			{
				bool flag2 = bonus == 0 && injury <= 0;
				if (flag2)
				{
					result = baseValue.ToString();
				}
				else
				{
					string bonusText = (bonus > 0) ? string.Format("<color=#{0}>+{1}</color>", "brightblue", bonus) : string.Empty;
					string injuryText = (injury > 0) ? string.Format("<color=#{0}>-{1}</color>", "brightred", injury) : string.Empty;
					result = string.Format("{0}{1}{2}", baseValue, bonusText, injuryText).ColorReplace();
				}
			}
			return result;
		}

		// Token: 0x06006913 RID: 26899 RVA: 0x003043E0 File Offset: 0x003025E0
		private static string GetMainPropertyTitle(ECricketCombatPropertyType type)
		{
			if (!true)
			{
			}
			string result;
			switch (type)
			{
			case ECricketCombatPropertyType.Hp:
				result = LanguageKey.LK_Cricket_Property_Hp.Tr();
				break;
			case ECricketCombatPropertyType.Sp:
				result = LanguageKey.LK_Cricket_Property_Sp.Tr();
				break;
			case ECricketCombatPropertyType.Vigor:
				result = LanguageKey.LK_Cricket_Property_Vigor.Tr();
				break;
			case ECricketCombatPropertyType.Strength:
				result = LanguageKey.LK_Cricket_Property_Strength.Tr();
				break;
			case ECricketCombatPropertyType.Bite:
				result = LanguageKey.LK_Cricket_Property_Bite.Tr();
				break;
			default:
				result = type.ToString();
				break;
			}
			if (!true)
			{
			}
			return result;
		}

		// Token: 0x06006914 RID: 26900 RVA: 0x00304468 File Offset: 0x00302668
		private sbyte GetOriginState()
		{
			bool flag = this._cricketData != null;
			sbyte result;
			if (flag)
			{
				result = this._cricketData.OriginState;
			}
			else
			{
				ItemDisplayData itemData = this._itemData;
				bool flag2 = ((itemData != null) ? itemData.CricketData : null) != null;
				if (flag2)
				{
					result = this._itemData.CricketData.OriginState;
				}
				else
				{
					result = -1;
				}
			}
			return result;
		}

		// Token: 0x06006915 RID: 26901 RVA: 0x003044C4 File Offset: 0x003026C4
		private CricketAffixesItem GetOriginStateAffixConfig()
		{
			int originState = (int)this.GetOriginState();
			bool flag = originState < 0;
			CricketAffixesItem result;
			if (flag)
			{
				result = null;
			}
			else
			{
				MapStateItem stateConfig = MapState.Instance[originState];
				bool flag2 = stateConfig.CricketAffix < 0;
				if (flag2)
				{
					result = null;
				}
				else
				{
					result = CricketAffixes.Instance[stateConfig.CricketAffix];
				}
			}
			return result;
		}

		// Token: 0x06006916 RID: 26902 RVA: 0x00304518 File Offset: 0x00302718
		private static string GetAffixName(int affixId)
		{
			bool flag = affixId < 0;
			string result;
			if (flag)
			{
				result = string.Empty;
			}
			else
			{
				result = CricketAffixes.Instance[affixId].Name;
			}
			return result;
		}

		// Token: 0x06006917 RID: 26903 RVA: 0x0030454C File Offset: 0x0030274C
		private static string GetAffixDesc(int affixId)
		{
			bool flag = affixId < 0;
			string result;
			if (flag)
			{
				result = string.Empty;
			}
			else
			{
				result = CricketAffixes.Instance[affixId].Desc;
			}
			return result;
		}

		// Token: 0x06006918 RID: 26904 RVA: 0x00304580 File Offset: 0x00302780
		private static string GetAffixName(CricketAffixesItem affixConfig)
		{
			return ((affixConfig != null) ? affixConfig.Name : null) ?? string.Empty;
		}

		// Token: 0x06006919 RID: 26905 RVA: 0x003045A8 File Offset: 0x003027A8
		private static string GetAffixDesc(CricketAffixesItem affixConfig)
		{
			return ((affixConfig != null) ? affixConfig.Desc : null) ?? string.Empty;
		}

		// Token: 0x0600691A RID: 26906 RVA: 0x003045D0 File Offset: 0x003027D0
		private void RefreshSpiritPropertyConfig()
		{
			CricketCore baseCore = CricketCoreUtils.BuildCricketBasePropertyForTip(this._itemData);
			List<ValueTuple<string, string, string>> properties = new List<ValueTuple<string, string, string>>();
			foreach (ECricketCombatPropertyType type in TooltipCricket.SpiritProperties)
			{
				int baseValue = baseCore.GetProperty(type);
				string str = "ui9_icon_mousetip_cricket_spirit_property_";
				int num = (int)type;
				string icon = str + num.ToString();
				string title = TooltipCricket.GetSpiritPropertyTitle(type);
				properties.Add(new ValueTuple<string, string, string>(icon, title, TooltipCricket.FormatSpiritValue(baseValue, type)));
			}
			bool hasProperty = properties.Count > 0;
			this.spiritPropertyContainer.parent.gameObject.SetActive(hasProperty);
			this.spiritPropertyTemplateItem.gameObject.SetActive(hasProperty);
			CommonUtils.PrepareEnoughChildren(this.spiritPropertyContainer, this.spiritPropertyTemplateItem.gameObject, properties.Count, null);
			for (int i = 0; i < properties.Count; i++)
			{
				ValueTuple<string, string, string> valueTuple = properties[i];
				string icon2 = valueTuple.Item1;
				string title2 = valueTuple.Item2;
				string value = valueTuple.Item3;
				this.spiritPropertyContainer.GetChild(i).GetComponent<TooltipItemProperty>().Set(icon2, title2, value, true);
			}
		}

		// Token: 0x0600691B RID: 26907 RVA: 0x0030470C File Offset: 0x0030290C
		private void RefreshSpiritProperty(bool detailMode)
		{
			List<ValueTuple<string, string, string>> properties = this.GetSpiritPropertyList(detailMode);
			bool hasProperty = properties.Count > 0;
			this.spiritPropertyContainer.parent.gameObject.SetActive(hasProperty);
			this.spiritPropertyTemplateItem.gameObject.SetActive(hasProperty);
			CommonUtils.PrepareEnoughChildren(this.spiritPropertyContainer, this.spiritPropertyTemplateItem.gameObject, properties.Count, null);
			for (int i = 0; i < properties.Count; i++)
			{
				ValueTuple<string, string, string> valueTuple = properties[i];
				string icon = valueTuple.Item1;
				string title = valueTuple.Item2;
				string value = valueTuple.Item3;
				this.spiritPropertyContainer.GetChild(i).GetComponent<TooltipItemProperty>().Set(icon, title, value, true);
			}
		}

		// Token: 0x0600691C RID: 26908 RVA: 0x003047D0 File Offset: 0x003029D0
		[return: TupleElementNames(new string[]
		{
			"icon",
			"title",
			"value"
		})]
		private List<ValueTuple<string, string, string>> GetSpiritPropertyList(bool detailMode)
		{
			List<ValueTuple<string, string, string>> result = new List<ValueTuple<string, string, string>>();
			CricketCore baseCore = CricketCoreUtils.BuildCricketBasePropertyForTip(this._itemData);
			foreach (ECricketCombatPropertyType type in TooltipCricket.SpiritProperties)
			{
				int baseValue = baseCore.GetProperty(type);
				int spiritBonus = this.GetSpiritPropertyBonus(type);
				int finalValue = baseValue + spiritBonus;
				string valueText = TooltipCricket.FormatSpiritDetailValue(type, baseValue, spiritBonus, finalValue, detailMode);
				string str = "ui9_icon_mousetip_cricket_spirit_property_";
				int num = (int)type;
				string icon = str + num.ToString();
				string title = TooltipCricket.GetSpiritPropertyTitle(type);
				result.Add(new ValueTuple<string, string, string>(icon, title, valueText));
			}
			return result;
		}

		// Token: 0x0600691D RID: 26909 RVA: 0x00304874 File Offset: 0x00302A74
		private static string FormatSpiritDetailValue(ECricketCombatPropertyType type, int baseValue, int bonus, int finalValue, bool detailMode)
		{
			bool flag = !detailMode;
			string result;
			if (flag)
			{
				result = TooltipCricket.FormatSpiritValue(finalValue, type);
			}
			else
			{
				bool flag2 = bonus == 0;
				if (flag2)
				{
					result = TooltipCricket.FormatSpiritValue(baseValue, type);
				}
				else
				{
					string baseText = TooltipCricket.FormatSpiritValue(baseValue, type);
					string bonusText = (bonus > 0) ? ("<color=#brightblue>" + TooltipCricket.FormatSpiritDelta(bonus, type) + "</color>") : ("<color=#brightred>" + TooltipCricket.FormatSpiritDelta(bonus, type) + "</color>");
					result = (baseText + bonusText).ColorReplace();
				}
			}
			return result;
		}

		// Token: 0x0600691E RID: 26910 RVA: 0x003048F4 File Offset: 0x00302AF4
		private static string FormatSpiritValue(int value, ECricketCombatPropertyType type)
		{
			return TooltipCricket.NeedPercentDisplay(type) ? string.Format("{0}%", value) : value.ToString();
		}

		// Token: 0x0600691F RID: 26911 RVA: 0x00304928 File Offset: 0x00302B28
		private static string FormatSpiritDelta(int delta, ECricketCombatPropertyType type)
		{
			string prefix = (delta > 0) ? "+" : string.Empty;
			return TooltipCricket.NeedPercentDisplay(type) ? string.Format("{0}{1}%", prefix, delta) : string.Format("{0}{1}", prefix, delta);
		}

		// Token: 0x06006920 RID: 26912 RVA: 0x00304978 File Offset: 0x00302B78
		private static bool NeedPercentDisplay(ECricketCombatPropertyType type)
		{
			return type != ECricketCombatPropertyType.Damage && type != ECricketCombatPropertyType.DamageReduce;
		}

		// Token: 0x06006921 RID: 26913 RVA: 0x0030499C File Offset: 0x00302B9C
		private static string GetSpiritPropertyTitle(ECricketCombatPropertyType type)
		{
			if (!true)
			{
			}
			string result;
			switch (type)
			{
			case ECricketCombatPropertyType.Deadliness:
				result = LanguageKey.LK_Cricket_HiddenProperty_Deadliness.Tr();
				break;
			case ECricketCombatPropertyType.Damage:
				result = LanguageKey.LK_Cricket_HiddenProperty_Damage.Tr();
				break;
			case ECricketCombatPropertyType.Cripple:
				result = LanguageKey.LK_Cricket_HiddenProperty_Cripple.Tr();
				break;
			case ECricketCombatPropertyType.Defense:
				result = LanguageKey.LK_Cricket_HiddenProperty_Defense.Tr();
				break;
			case ECricketCombatPropertyType.DamageReduce:
				result = LanguageKey.LK_Cricket_HiddenProperty_DamageReduce.Tr();
				break;
			case ECricketCombatPropertyType.Counter:
				result = LanguageKey.LK_Cricket_HiddenProperty_Counter.Tr();
				break;
			default:
				result = type.ToString();
				break;
			}
			if (!true)
			{
			}
			return result;
		}

		// Token: 0x06006922 RID: 26914 RVA: 0x00304A38 File Offset: 0x00302C38
		private void RefreshCombatRecord()
		{
			this.loseCountText.text = this._cricketData.LossesCount.ToString();
			this.winCountText.text = this._cricketData.WinsCount.ToString();
			bool flag = this._cricketData.BestEnemyColorId > 0;
			if (flag)
			{
				sbyte enemyGrade = new ValueTuple<short, short>(this._cricketData.BestEnemyColorId, this._cricketData.BestEnemyPartId).CalcCricketGrade();
				this.lastEnemyText.text = new ValueTuple<short, short>(this._cricketData.BestEnemyColorId, this._cricketData.BestEnemyPartId).CalcCricketName().SetGradeColor((int)enemyGrade);
			}
			else
			{
				this.lastEnemyText.text = LanguageKey.LK_Common_None.Tr();
			}
		}

		// Token: 0x06006923 RID: 26915 RVA: 0x00304B00 File Offset: 0x00302D00
		private void RefreshStatusArea()
		{
			this._strBuilder.Clear();
			bool hasAny = false;
			bool naturalDeath = this._cricketData.NaturalDeath;
			if (naturalDeath)
			{
				this._strBuilder.Append(LanguageKey.LK_Cricket_Die_Tips.Tr().SetColor("brightred"));
				hasAny = true;
			}
			bool flag = this._cricketData.IsSmart && this._cricketData.IsIdentified;
			if (flag)
			{
				bool flag2 = hasAny;
				if (flag2)
				{
					this._strBuilder.Append("、");
				}
				this._strBuilder.Append(LanguageKey.LK_Cricket_Smart_Tips.Tr().SetColor("brightblue"));
				hasAny = true;
			}
			bool flag3 = this._cricketData.IsIdentified && !this._cricketData.IsSmart;
			if (flag3)
			{
				bool flag4 = hasAny;
				if (flag4)
				{
					this._strBuilder.Append("、");
				}
				this._strBuilder.Append(LanguageKey.LK_Cricket_Ordinary.Tr().SetColor("brightyellow"));
				hasAny = true;
			}
			this.statusArea.SetActive(hasAny);
			bool flag5 = hasAny;
			if (flag5)
			{
				this.statusText.text = this._strBuilder.ToString().ColorReplace();
			}
		}

		// Token: 0x06006924 RID: 26916 RVA: 0x00304C38 File Offset: 0x00302E38
		private string GetAffixColorText()
		{
			return TooltipCricket.GetAffixName((int)this._colorConfig.Affix);
		}

		// Token: 0x06006925 RID: 26917 RVA: 0x00304C5C File Offset: 0x00302E5C
		private string GetAffixOriginStateText()
		{
			return TooltipCricket.GetAffixName(this.GetOriginStateAffixConfig());
		}

		// Token: 0x06006926 RID: 26918 RVA: 0x00304C7C File Offset: 0x00302E7C
		private string GetAffixColorDesc()
		{
			return TooltipCricket.GetAffixDesc((int)this._colorConfig.Affix);
		}

		// Token: 0x06006927 RID: 26919 RVA: 0x00304CA0 File Offset: 0x00302EA0
		private string GetAffixOriginStateDesc()
		{
			return TooltipCricket.GetAffixDesc(this.GetOriginStateAffixConfig());
		}

		// Token: 0x06006928 RID: 26920 RVA: 0x00304CC0 File Offset: 0x00302EC0
		private void RefreshSpiritDesc()
		{
			bool flag = this._cricketData == null;
			if (!flag)
			{
				int spiritUnit = GlobalConfig.Instance.CricketSpiritUnit;
				int spiritMax = GlobalConfig.Instance.CricketSpiritMax;
				int currentSpirit = this._cricketData.Spirit;
				CricketSpiritProperty spiritAddProperties = this._cricketData.SpiritAddProperties;
				int growthCount = (spiritAddProperties != null) ? spiritAddProperties.GrowthCount : 0;
				int nextGrowthSpirit = (growthCount + 1) * spiritUnit;
				int spiritNeeded = (nextGrowthSpirit <= spiritMax) ? (nextGrowthSpirit - currentSpirit) : 0;
				spiritNeeded = Mathf.Max(0, spiritNeeded);
				this.spiritDesc3Text.text = string.Format(LanguageKey.LK_Cricket_Spirit_Desc3.Tr(), spiritNeeded);
			}
		}

		// Token: 0x06006929 RID: 26921 RVA: 0x00304D5C File Offset: 0x00302F5C
		private void RefreshAllAffixTexts()
		{
			this.affixColorText.text = this.GetAffixColorText();
			this.affixStateText.text = this.GetAffixOriginStateText();
			this.rightAffixColorText.text = this.GetAffixColorText();
			this.rightAffixColorDescText.text = this.GetAffixColorDesc();
			this.rightAffixStateText.text = this.GetAffixOriginStateText();
			this.rightAffixStateDescText.text = this.GetAffixOriginStateDesc();
		}

		// Token: 0x0600692A RID: 26922 RVA: 0x00304DD8 File Offset: 0x00302FD8
		private bool IsDetailMode()
		{
			return Input.GetKey(KeyCode.LeftAlt) || Input.GetKey(KeyCode.RightAlt);
		}

		// Token: 0x0600692B RID: 26923 RVA: 0x00304E04 File Offset: 0x00303004
		private void RefreshDetailMode()
		{
			bool detailMode = this.IsDetailMode();
			this.detailRightArea.SetActive(detailMode);
			bool flag = this._cricketData != null;
			if (flag)
			{
				this.RefreshPropertyFromData();
				this.RefreshSpiritProperty(detailMode);
			}
			this.RefreshOperationArea(detailMode);
		}

		// Token: 0x0600692C RID: 26924 RVA: 0x00304E4C File Offset: 0x0030304C
		private void RefreshOperationArea(bool detailMode)
		{
			if (detailMode)
			{
				this.operationArea.RefreshCancelDetail();
			}
			else
			{
				this.operationArea.RefreshPressToDetail();
			}
			this.operationArea.RefreshHotkeyDisplayLockItem(this._itemData);
			this.operationArea.RefreshHotkeyDisplayViewEncyclopedia(false);
		}

		// Token: 0x0600692D RID: 26925 RVA: 0x00304E98 File Offset: 0x00303098
		private void Update()
		{
			bool flag = this._itemData == null;
			if (!flag)
			{
				bool hasStick = this.HasStick;
				if (!hasStick)
				{
					bool altDown = this.IsDetailMode();
					bool flag2 = altDown != this._lastAltDown;
					if (flag2)
					{
						this._lastAltDown = altDown;
						this.RefreshDetailMode();
						this.ForceRebuildLayout(2U);
					}
				}
			}
		}

		// Token: 0x0600692F RID: 26927 RVA: 0x00304F04 File Offset: 0x00303104
		// Note: this type is marked as 'beforefieldinit'.
		static TooltipCricket()
		{
			ECricketCombatPropertyType[] array = new ECricketCombatPropertyType[5];
			RuntimeHelpers.InitializeArray(array, fieldof(<PrivateImplementationDetails>.E528F4309E1413E6BC35AEA5D8DB8519384D2FCC33F9DD5D1126D73F104CF92A).FieldHandle);
			TooltipCricket.MainProperties = array;
			ECricketCombatPropertyType[] array2 = new ECricketCombatPropertyType[6];
			RuntimeHelpers.InitializeArray(array2, fieldof(<PrivateImplementationDetails>.7CCD18F36A831ADC189964971B98375068D9ED4864F88E2FB6631512D88ECC50).FieldHandle);
			TooltipCricket.SpiritProperties = array2;
		}

		// Token: 0x04004B2B RID: 19243
		[Header("基础信息")]
		[SerializeField]
		private CricketViewNew cricketView;

		// Token: 0x04004B2C RID: 19244
		[SerializeField]
		private TextMeshProUGUI nameText;

		// Token: 0x04004B2D RID: 19245
		[SerializeField]
		private TextMeshProUGUI gradeText;

		// Token: 0x04004B2E RID: 19246
		[SerializeField]
		private TextMeshProUGUI typeText;

		// Token: 0x04004B2F RID: 19247
		[SerializeField]
		private TextMeshProUGUI ageText;

		// Token: 0x04004B30 RID: 19248
		[SerializeField]
		private TextMeshProUGUI valueText;

		// Token: 0x04004B31 RID: 19249
		[SerializeField]
		private TextMeshProUGUI weightText;

		// Token: 0x04004B32 RID: 19250
		[SerializeField]
		private TextMeshProUGUI durabilityText;

		// Token: 0x04004B33 RID: 19251
		[SerializeField]
		private TextMeshProUGUI descText;

		// Token: 0x04004B34 RID: 19252
		[SerializeField]
		private CImage gradeBackImage;

		// Token: 0x04004B35 RID: 19253
		[Header("属性加成")]
		[SerializeField]
		private Transform propertyContainer;

		// Token: 0x04004B36 RID: 19254
		[SerializeField]
		private TooltipItemProperty propertyTemplateItem;

		// Token: 0x04004B37 RID: 19255
		[Header("促织灵性")]
		[SerializeField]
		private TextMeshProUGUI spiritText;

		// Token: 0x04004B38 RID: 19256
		[SerializeField]
		private TextMeshProUGUI affixColorText;

		// Token: 0x04004B39 RID: 19257
		[SerializeField]
		private TextMeshProUGUI affixStateText;

		// Token: 0x04004B3A RID: 19258
		[Header("灵性成长属性")]
		[SerializeField]
		private Transform spiritPropertyContainer;

		// Token: 0x04004B3B RID: 19259
		[SerializeField]
		private TooltipItemProperty spiritPropertyTemplateItem;

		// Token: 0x04004B3C RID: 19260
		[Header("促织战绩")]
		[SerializeField]
		private TextMeshProUGUI loseCountText;

		// Token: 0x04004B3D RID: 19261
		[SerializeField]
		private TextMeshProUGUI winCountText;

		// Token: 0x04004B3E RID: 19262
		[SerializeField]
		private TextMeshProUGUI lastEnemyText;

		// Token: 0x04004B3F RID: 19263
		[Header("状态")]
		[SerializeField]
		private GameObject statusArea;

		// Token: 0x04004B40 RID: 19264
		[SerializeField]
		private TextMeshProUGUI statusText;

		// Token: 0x04004B41 RID: 19265
		[Header("禁用功能")]
		[SerializeField]
		private TooltipItemOtherArea otherArea;

		// Token: 0x04004B42 RID: 19266
		private static readonly List<ItemFunction> CricketDisableFunctions = new List<ItemFunction>
		{
			ItemFunction.Repairable,
			ItemFunction.Transferable,
			ItemFunction.Poisonable,
			ItemFunction.Refinable
		};

		// Token: 0x04004B43 RID: 19267
		[Header("操作区域")]
		[SerializeField]
		private TooltipOperationArea operationArea;

		// Token: 0x04004B44 RID: 19268
		[Header("详细模式右侧")]
		[SerializeField]
		private GameObject detailRightArea;

		// Token: 0x04004B45 RID: 19269
		[Header("灵性说明")]
		[SerializeField]
		private TextMeshProUGUI spiritDesc3Text;

		// Token: 0x04004B46 RID: 19270
		[Header("生地和品种相关说明")]
		[SerializeField]
		private TextMeshProUGUI rightAffixColorText;

		// Token: 0x04004B47 RID: 19271
		[SerializeField]
		private TextMeshProUGUI rightAffixColorDescText;

		// Token: 0x04004B48 RID: 19272
		[SerializeField]
		private TextMeshProUGUI rightAffixStateText;

		// Token: 0x04004B49 RID: 19273
		[SerializeField]
		private TextMeshProUGUI rightAffixStateDescText;

		// Token: 0x04004B4A RID: 19274
		[Header("DLC限制")]
		[SerializeField]
		private GameObject[] polymorphOnlyObjects;

		// Token: 0x04004B4B RID: 19275
		private ItemDisplayData _itemData;

		// Token: 0x04004B4C RID: 19276
		private CricketData _cricketData;

		// Token: 0x04004B4D RID: 19277
		private CricketPartsItem _colorConfig;

		// Token: 0x04004B4E RID: 19278
		private CricketPartsItem _partConfig;

		// Token: 0x04004B4F RID: 19279
		private bool _isCombineCricket;

		// Token: 0x04004B50 RID: 19280
		private bool _templateDataOnly;

		// Token: 0x04004B51 RID: 19281
		private bool _lastAltDown;

		// Token: 0x04004B52 RID: 19282
		private readonly StringBuilder _strBuilder = new StringBuilder();

		// Token: 0x04004B53 RID: 19283
		private static readonly ECricketCombatPropertyType[] MainProperties;

		// Token: 0x04004B54 RID: 19284
		private static readonly ECricketCombatPropertyType[] SpiritProperties;
	}
}
