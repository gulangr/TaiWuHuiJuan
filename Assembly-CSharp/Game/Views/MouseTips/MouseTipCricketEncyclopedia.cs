using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Config;
using FrameWork;
using Game.Views.Cricket;
using Game.Views.Cricket.Combat;
using Game.Views.MouseTips.Common;
using Game.Views.MouseTips.Item.Common;
using GameData.Combat.Cricket;
using GameData.Domains.Item;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Game.Views.MouseTips
{
	// Token: 0x0200084E RID: 2126
	public class MouseTipCricketEncyclopedia : MouseTipBase
	{
		// Token: 0x17000C6B RID: 3179
		// (get) Token: 0x0600673B RID: 26427 RVA: 0x002F126D File Offset: 0x002EF46D
		protected override bool CanStick
		{
			get
			{
				return true;
			}
		}

		// Token: 0x0600673C RID: 26428 RVA: 0x002F1270 File Offset: 0x002EF470
		protected override void Init(ArgumentBox argsBox)
		{
			string colorName;
			argsBox.Get("Part1", out colorName);
			string partName;
			argsBox.Get("Part2", out partName);
			this._isCombineCricket = !string.IsNullOrWhiteSpace(partName);
			bool flag = string.IsNullOrWhiteSpace(partName);
			if (flag)
			{
				this._partConfig = CricketParts.Instance[0];
			}
			else
			{
				int partId;
				bool flag2 = int.TryParse(partName, out partId);
				if (flag2)
				{
					this._partConfig = CricketParts.Instance[partId];
				}
				else
				{
					this._partConfig = CricketParts.Instance[partName];
				}
			}
			int colorId;
			bool flag3 = int.TryParse(colorName, out colorId);
			if (flag3)
			{
				this._colorConfig = CricketParts.Instance[colorId];
			}
			else
			{
				this._colorConfig = CricketParts.Instance[colorName];
			}
			this.Refresh();
			this.ForceRebuildLayout(2U);
		}

		// Token: 0x0600673D RID: 26429 RVA: 0x002F1340 File Offset: 0x002EF540
		public override void Refresh()
		{
			bool flag = this._colorConfig == null || this._partConfig == null;
			if (!flag)
			{
				this.RefreshDlcOnlyObjects();
				this.RefreshConfigInfo();
				this.RefreshDetailMode();
				UIElement element = this.Element;
				if (element != null)
				{
					element.ShowAfterRefresh();
				}
				this.ForceRebuildLayout(2U);
			}
		}

		// Token: 0x0600673E RID: 26430 RVA: 0x002F1398 File Offset: 0x002EF598
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

		// Token: 0x0600673F RID: 26431 RVA: 0x002F13CC File Offset: 0x002EF5CC
		private void RefreshDlcOnlyObjects()
		{
			bool dlcEnabled = CricketPolymorphHelper.IsCricketPolymorphEnabled;
			for (int i = 0; i < this.polymorphOnlyObjects.Length; i++)
			{
				this.polymorphOnlyObjects[i].SetActive(dlcEnabled);
			}
		}

		// Token: 0x06006740 RID: 26432 RVA: 0x002F1408 File Offset: 0x002EF608
		private void RefreshConfigInfo()
		{
			short colorId = this._colorConfig.TemplateId;
			short partId = this._partConfig.TemplateId;
			sbyte grade = new ValueTuple<short, short>(colorId, partId).CalcCricketGrade();
			this.nameText.text = new ValueTuple<short, short>(colorId, partId).CalcCricketName().SetGradeColor((int)grade);
			string gradeStr = LocalStringManager.Get(string.Format("LK_Cricket_Grade_Name{0}", grade));
			string gradeShortStr = LocalStringManager.Get(string.Format("LK_ShortGrade_{0}", grade));
			this.gradeText.text = LanguageKey.LK_Cricket_Tip_Grade_Format.TrFormat(gradeShortStr, gradeStr).SetGradeColor((int)grade);
			this.typeText.text = LocalStringManager.Get(string.Format("LK_ItemSubType_{0}", 1100));
			string ageStr = LanguageKey.LK_Age.TrFormat(this._colorConfig.Life);
			this.ageText.text = LocalStringManager.GetFormat(LanguageKey.LK_MouseTip_Circket_Age_Content, "-", ageStr);
			this.valueText.text = this._partConfig.Value.ToString();
			this.weightText.text = "0.1";
			this.durabilityText.text = "-/-";
			string desc = (this._isCombineCricket ? this._partConfig : this._colorConfig).Desc;
			MouseTip_Util.SetMultiLineAutoHeightText(this.descText, desc);
			this.gradeBackImage.SetSprite(string.Format("{0}{1}", "ui9_mousetip_base_level_", grade), false, null);
			this.cricketView.SetCricketData(colorId, partId, false, null, false);
			this.cricketView.gameObject.SetActive(true);
			this.RefreshPropertyFromConfig();
			this.spiritText.text = "0";
			this.RefreshAllAffixTexts();
			this.RefreshSpiritPropertyConfig();
			this.loseCountText.text = "-";
			this.winCountText.text = "-";
			this.lastEnemyText.text = "-";
			this.statusArea.SetActive(false);
			this.otherArea.Refresh(MouseTipCricketEncyclopedia.EncyclopediaDisableFunctions);
			this.operationArea.RefreshPressToDetail();
			this.operationArea.RefreshHotkeyDisplayViewEncyclopedia(false);
		}

		// Token: 0x06006741 RID: 26433 RVA: 0x002F1644 File Offset: 0x002EF844
		private CricketCore BuildCricketCore()
		{
			CricketCore core = this._colorConfig;
			bool isCombineCricket = this._isCombineCricket;
			if (isCombineCricket)
			{
				core += this._partConfig;
			}
			return core;
		}

		// Token: 0x06006742 RID: 26434 RVA: 0x002F1680 File Offset: 0x002EF880
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

		// Token: 0x06006743 RID: 26435 RVA: 0x002F1744 File Offset: 0x002EF944
		[return: TupleElementNames(new string[]
		{
			"icon",
			"title",
			"value"
		})]
		private List<ValueTuple<string, string, string>> GetPropertyListFromConfig()
		{
			List<ValueTuple<string, string, string>> result = new List<ValueTuple<string, string, string>>();
			CricketCore baseCore = this.BuildCricketCore();
			for (int i = 0; i < MouseTipCricketEncyclopedia.MainProperties.Length; i++)
			{
				ECricketCombatPropertyType type = MouseTipCricketEncyclopedia.MainProperties[i];
				List<ValueTuple<string, string, string>> list = result;
				string str = "ui9_icon_mousetip_cricket_property_big_";
				int num = (int)type;
				list.Add(new ValueTuple<string, string, string>(str + num.ToString(), MouseTipCricketEncyclopedia.GetMainPropertyTitle(type), baseCore.GetProperty(type).ToString()));
			}
			return result;
		}

		// Token: 0x06006744 RID: 26436 RVA: 0x002F17C0 File Offset: 0x002EF9C0
		private void RefreshSpiritPropertyConfig()
		{
			CricketCore baseCore = this.BuildCricketCore();
			List<ValueTuple<string, string, string>> properties = new List<ValueTuple<string, string, string>>();
			foreach (ECricketCombatPropertyType type in MouseTipCricketEncyclopedia.SpiritProperties)
			{
				int baseValue = baseCore.GetProperty(type);
				string str = "ui9_icon_mousetip_cricket_spirit_property_";
				int num = (int)type;
				string icon = str + num.ToString();
				string title = MouseTipCricketEncyclopedia.GetSpiritPropertyTitle(type);
				properties.Add(new ValueTuple<string, string, string>(icon, title, MouseTipCricketEncyclopedia.FormatSpiritValue(baseValue, type)));
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

		// Token: 0x06006745 RID: 26437 RVA: 0x002F18F8 File Offset: 0x002EFAF8
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

		// Token: 0x06006746 RID: 26438 RVA: 0x002F1980 File Offset: 0x002EFB80
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

		// Token: 0x06006747 RID: 26439 RVA: 0x002F1A1C File Offset: 0x002EFC1C
		private static string FormatSpiritValue(int value, ECricketCombatPropertyType type)
		{
			return MouseTipCricketEncyclopedia.NeedPercentDisplay(type) ? string.Format("{0}%", value) : value.ToString();
		}

		// Token: 0x06006748 RID: 26440 RVA: 0x002F1A50 File Offset: 0x002EFC50
		private static bool NeedPercentDisplay(ECricketCombatPropertyType type)
		{
			return type != ECricketCombatPropertyType.Damage && type != ECricketCombatPropertyType.DamageReduce;
		}

		// Token: 0x06006749 RID: 26441 RVA: 0x002F1A74 File Offset: 0x002EFC74
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

		// Token: 0x0600674A RID: 26442 RVA: 0x002F1AA8 File Offset: 0x002EFCA8
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

		// Token: 0x0600674B RID: 26443 RVA: 0x002F1ADC File Offset: 0x002EFCDC
		private void RefreshAllAffixTexts()
		{
			string colorAffixName = MouseTipCricketEncyclopedia.GetAffixName((int)this._colorConfig.Affix);
			string colorAffixDesc = MouseTipCricketEncyclopedia.GetAffixDesc((int)this._colorConfig.Affix);
			this.affixColorText.text = colorAffixName;
			this.affixStateText.text = string.Empty;
			this.rightAffixColorText.text = colorAffixName;
			this.rightAffixColorDescText.text = colorAffixDesc;
			this.rightAffixStateText.text = string.Empty;
			this.rightAffixStateDescText.text = string.Empty;
		}

		// Token: 0x0600674C RID: 26444 RVA: 0x002F1B68 File Offset: 0x002EFD68
		private bool IsDetailMode()
		{
			return Input.GetKey(KeyCode.LeftAlt) || Input.GetKey(KeyCode.RightAlt);
		}

		// Token: 0x0600674D RID: 26445 RVA: 0x002F1B94 File Offset: 0x002EFD94
		private void RefreshDetailMode()
		{
			bool detailMode = this.IsDetailMode();
			this.detailRightArea.SetActive(detailMode);
			bool flag = detailMode;
			if (flag)
			{
				this.operationArea.RefreshCancelDetail();
			}
			else
			{
				this.operationArea.RefreshPressToDetail();
			}
			this.operationArea.RefreshHotkeyDisplayViewEncyclopedia(false);
		}

		// Token: 0x0600674E RID: 26446 RVA: 0x002F1BE4 File Offset: 0x002EFDE4
		private void Update()
		{
			bool flag = this._colorConfig == null;
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

		// Token: 0x06006750 RID: 26448 RVA: 0x002F1C48 File Offset: 0x002EFE48
		// Note: this type is marked as 'beforefieldinit'.
		static MouseTipCricketEncyclopedia()
		{
			ECricketCombatPropertyType[] array = new ECricketCombatPropertyType[5];
			RuntimeHelpers.InitializeArray(array, fieldof(<PrivateImplementationDetails>.E528F4309E1413E6BC35AEA5D8DB8519384D2FCC33F9DD5D1126D73F104CF92A).FieldHandle);
			MouseTipCricketEncyclopedia.MainProperties = array;
			ECricketCombatPropertyType[] array2 = new ECricketCombatPropertyType[6];
			RuntimeHelpers.InitializeArray(array2, fieldof(<PrivateImplementationDetails>.7CCD18F36A831ADC189964971B98375068D9ED4864F88E2FB6631512D88ECC50).FieldHandle);
			MouseTipCricketEncyclopedia.SpiritProperties = array2;
		}

		// Token: 0x040048C3 RID: 18627
		[Header("基础信息")]
		[SerializeField]
		private CricketViewNew cricketView;

		// Token: 0x040048C4 RID: 18628
		[SerializeField]
		private TextMeshProUGUI nameText;

		// Token: 0x040048C5 RID: 18629
		[SerializeField]
		private TextMeshProUGUI gradeText;

		// Token: 0x040048C6 RID: 18630
		[SerializeField]
		private TextMeshProUGUI typeText;

		// Token: 0x040048C7 RID: 18631
		[SerializeField]
		private TextMeshProUGUI ageText;

		// Token: 0x040048C8 RID: 18632
		[SerializeField]
		private TextMeshProUGUI valueText;

		// Token: 0x040048C9 RID: 18633
		[SerializeField]
		private TextMeshProUGUI weightText;

		// Token: 0x040048CA RID: 18634
		[SerializeField]
		private TextMeshProUGUI durabilityText;

		// Token: 0x040048CB RID: 18635
		[SerializeField]
		private TextMeshProUGUI descText;

		// Token: 0x040048CC RID: 18636
		[SerializeField]
		private CImage gradeBackImage;

		// Token: 0x040048CD RID: 18637
		[Header("属性加成")]
		[SerializeField]
		private Transform propertyContainer;

		// Token: 0x040048CE RID: 18638
		[SerializeField]
		private TooltipItemProperty propertyTemplateItem;

		// Token: 0x040048CF RID: 18639
		[Header("促织灵性")]
		[SerializeField]
		private TextMeshProUGUI spiritText;

		// Token: 0x040048D0 RID: 18640
		[SerializeField]
		private TextMeshProUGUI affixColorText;

		// Token: 0x040048D1 RID: 18641
		[SerializeField]
		private TextMeshProUGUI affixStateText;

		// Token: 0x040048D2 RID: 18642
		[Header("灵性成长属性")]
		[SerializeField]
		private Transform spiritPropertyContainer;

		// Token: 0x040048D3 RID: 18643
		[SerializeField]
		private TooltipItemProperty spiritPropertyTemplateItem;

		// Token: 0x040048D4 RID: 18644
		[Header("促织战绩")]
		[SerializeField]
		private TextMeshProUGUI loseCountText;

		// Token: 0x040048D5 RID: 18645
		[SerializeField]
		private TextMeshProUGUI winCountText;

		// Token: 0x040048D6 RID: 18646
		[SerializeField]
		private TextMeshProUGUI lastEnemyText;

		// Token: 0x040048D7 RID: 18647
		[Header("状态")]
		[SerializeField]
		private GameObject statusArea;

		// Token: 0x040048D8 RID: 18648
		[SerializeField]
		private TextMeshProUGUI statusText;

		// Token: 0x040048D9 RID: 18649
		[Header("禁用功能")]
		[SerializeField]
		private TooltipItemOtherArea otherArea;

		// Token: 0x040048DA RID: 18650
		private static readonly List<ItemFunction> EncyclopediaDisableFunctions = new List<ItemFunction>
		{
			ItemFunction.Disassemble,
			ItemFunction.Repairable,
			ItemFunction.Transferable,
			ItemFunction.Poisonable,
			ItemFunction.Refinable
		};

		// Token: 0x040048DB RID: 18651
		[Header("操作区域")]
		[SerializeField]
		private TooltipOperationArea operationArea;

		// Token: 0x040048DC RID: 18652
		[Header("详细模式右侧")]
		[SerializeField]
		private GameObject detailRightArea;

		// Token: 0x040048DD RID: 18653
		[Header("灵性说明")]
		[SerializeField]
		private TextMeshProUGUI spiritDesc3Text;

		// Token: 0x040048DE RID: 18654
		[Header("生地和品种相关说明")]
		[SerializeField]
		private TextMeshProUGUI rightAffixColorText;

		// Token: 0x040048DF RID: 18655
		[SerializeField]
		private TextMeshProUGUI rightAffixColorDescText;

		// Token: 0x040048E0 RID: 18656
		[SerializeField]
		private TextMeshProUGUI rightAffixStateText;

		// Token: 0x040048E1 RID: 18657
		[SerializeField]
		private TextMeshProUGUI rightAffixStateDescText;

		// Token: 0x040048E2 RID: 18658
		[Header("DLC限制")]
		[SerializeField]
		private GameObject[] polymorphOnlyObjects;

		// Token: 0x040048E3 RID: 18659
		private CricketPartsItem _colorConfig;

		// Token: 0x040048E4 RID: 18660
		private CricketPartsItem _partConfig;

		// Token: 0x040048E5 RID: 18661
		private bool _isCombineCricket;

		// Token: 0x040048E6 RID: 18662
		private bool _lastAltDown;

		// Token: 0x040048E7 RID: 18663
		private static readonly ECricketCombatPropertyType[] MainProperties;

		// Token: 0x040048E8 RID: 18664
		private static readonly ECricketCombatPropertyType[] SpiritProperties;
	}
}
