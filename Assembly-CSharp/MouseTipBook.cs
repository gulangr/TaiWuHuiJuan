using System;
using System.Collections.Generic;
using Config;
using FrameWork;
using GameData.Domains.Item;
using GameData.Domains.Item.Display;
using GameData.Serializer;
using GameData.Utilities;
using TMPro;
using UnityEngine;

// Token: 0x02000274 RID: 628
public class MouseTipBook : MouseTipItem
{
	// Token: 0x1700047F RID: 1151
	// (get) Token: 0x0600291A RID: 10522 RVA: 0x00131F15 File Offset: 0x00130115
	protected override bool CanStick
	{
		get
		{
			return true;
		}
	}

	// Token: 0x0600291B RID: 10523 RVA: 0x00131F18 File Offset: 0x00130118
	protected override void Init(ArgumentBox argsBox)
	{
		base.Init(argsBox);
		this.Refresh(argsBox);
	}

	// Token: 0x0600291C RID: 10524 RVA: 0x00131F2C File Offset: 0x0013012C
	public override void Refresh(ArgumentBox argsBox)
	{
		ItemDisplayData itemData;
		argsBox.Get<ItemDisplayData>("ItemData", out itemData);
		bool showPageInfo;
		argsBox.Get("ShowPageInfo", out showPageInfo);
		bool templateDataOnly;
		argsBox.Get("TemplateDataOnly", out templateDataOnly);
		SkillBookItem configData = SkillBook.Instance[itemData.Key.TemplateId];
		this._itemKey = itemData.Key;
		this._itemData = itemData;
		this._isCombatSkill = (configData.ItemSubType == 1001);
		string skillTypeName = this._isCombatSkill ? CombatSkillType.Instance[configData.CombatSkillType].Name : LifeSkillType.Instance[configData.LifeSkillType].Name;
		base.CGet<TextMeshProUGUI>("Name").text = configData.Name;
		base.CGet<CImage>("GradeBack").SetSprite(ItemView.GetGradeIcon(configData.Grade), false, null);
		base.CGet<TextMeshProUGUI>("GradeName").text = LocalStringManager.Get(string.Format("LK_ShortGrade_{0}", configData.Grade));
		base.CGet<TextMeshProUGUI>("Grade").text = (LocalStringManager.Get(string.Format("LK_Num_{0}", (int)(9 - configData.Grade))) + LocalStringManager.Get(LanguageKey.LK_Item_Grade)).SetColor(Colors.Instance.GradeColors[(int)configData.Grade]);
		base.CGet<TextMeshProUGUI>("Value").text = (templateDataOnly ? configData.BaseValue.ToString() : itemData.Value.ToString());
		base.CGet<GameObject>("Durability").SetActive(!templateDataOnly);
		base.CGet<GameObject>("Material").SetActive(!templateDataOnly);
		base.CGet<CImage>("ItemIcon").SetSprite(configData.Icon, false, null);
		base.SetItemDesc(configData.Desc, itemData.LoveTokenDataItem);
		base.CGet<GameObject>("Sect").SetActive(this._isCombatSkill);
		bool isCombatSkill = this._isCombatSkill;
		if (isCombatSkill)
		{
			base.CGet<TextMeshProUGUI>("SectName").text = Organization.Instance[CombatSkill.Instance[configData.CombatSkillTemplateId].SectId].Name;
		}
		base.CGet<TextMeshProUGUI>("SubType").text = skillTypeName + LocalStringManager.Get(string.Format("LK_ItemSubType_{0}", configData.ItemSubType));
		base.CGet<TextMeshProUGUI>("Weight").text = NumberFormatUtils.FormatItemWeight(itemData.Weight);
		base.CGet<TextMeshProUGUI>("DurabilityText").text = MouseTip_Util.GetItemDurabilityColorString((int)itemData.Durability, (int)itemData.MaxDurability);
		this.InitItemDisableFunctionList(itemData);
		base.RefreshDisableFunction();
		base.CGet<GameObject>("LifeSkill").SetActive(showPageInfo && !this._isCombatSkill);
		base.CGet<GameObject>("CombatSkill").SetActive(showPageInfo && this._isCombatSkill);
		base.CGet<GameObject>("HidePageInfo").SetActive(!showPageInfo);
		this.NeedWaitData = showPageInfo;
		bool flag = showPageInfo;
		if (flag)
		{
			ItemDomainMethod.AsyncCall.GetSkillBookPagesInfo(this, this._itemKey, new AsyncMethodCallbackDelegate(this.OnGetPageInfo));
		}
		base.RefreshPoisons(default(PoisonsAndLevels), itemData);
		base.RefreshHoldCount();
		base.RefreshHotkeyDisplayLockItem();
	}

	// Token: 0x0600291D RID: 10525 RVA: 0x00132280 File Offset: 0x00130480
	private void OnGetPageInfo(int offset, RawDataPool dataPool)
	{
		SkillBookPageDisplayData displayData = null;
		Serializer.Deserialize(dataPool, offset, ref displayData);
		bool flag = this._itemKey.Equals(displayData.ItemKey);
		if (flag)
		{
			RectTransform pageHolder = base.CGet<RectTransform>(this._isCombatSkill ? "CombatSkillPageHolder" : "LifeSkillPageHolder");
			for (int i = 0; i < displayData.ReadingProgress.Length; i++)
			{
				sbyte pageState = displayData.State[i];
				bool readed = displayData.ReadingProgress[i] == 100;
				Refers pageRefers = pageHolder.GetChild(i).GetComponent<Refers>();
				TextMeshProUGUI progressComplete = pageRefers.CGet<TextMeshProUGUI>("ProgressComplete");
				TextMeshProUGUI progressIncomplete = pageRefers.CGet<TextMeshProUGUI>("ProgressIncomplete");
				pageRefers.CGet<GameObject>("CompleteTips").SetActive(pageState == 0);
				pageRefers.CGet<GameObject>("IncompleteTips").SetActive(pageState == 1);
				pageRefers.CGet<GameObject>("IncompleteTips").GetComponent<TextMeshProUGUI>().SetText(LocalStringManager.Get(LanguageKey.LK_Book_Page_State_Incomplete), true);
				pageRefers.CGet<GameObject>("LostTips").SetActive(pageState == 2);
				pageRefers.CGet<GameObject>("ReadedTips").SetActive(readed);
				pageRefers.CGet<GameObject>("NotReadTips").SetActive(!readed);
				progressComplete.gameObject.SetActive(readed);
				progressIncomplete.gameObject.SetActive(!readed);
				(readed ? progressComplete : progressIncomplete).text = string.Format("{0}%", displayData.ReadingProgress[i]);
				bool isCombatSkill = this._isCombatSkill;
				if (isCombatSkill)
				{
					bool flag2 = i == 0;
					if (flag2)
					{
						bool flag3 = MouseTipBook.ColorMap.ContainsKey((int)displayData.Type[0]);
						if (flag3)
						{
							string colorName = MouseTipBook.ColorMap[(int)displayData.Type[0]];
							pageRefers.CGet<TextMeshProUGUI>("PageName").text = LocalStringManager.Get(string.Format("LK_CombatSkill_First_Page_Type_{0}", displayData.Type[0])).SetColor(colorName);
						}
					}
					else
					{
						bool isDirect = displayData.Type[i] == 0;
						TextMeshProUGUI pageNameDirect = pageRefers.CGet<TextMeshProUGUI>("PageNameDirect");
						TextMeshProUGUI pageNameReverse = pageRefers.CGet<TextMeshProUGUI>("PageNameReverse");
						pageNameDirect.gameObject.SetActive(isDirect);
						pageNameReverse.gameObject.SetActive(!isDirect);
						bool flag4 = isDirect;
						if (flag4)
						{
							pageNameDirect.text = LocalStringManager.Get(string.Format("LK_CombatSkill_Direct_Page_{0}", i - 1));
						}
						else
						{
							pageNameReverse.text = LocalStringManager.Get(string.Format("LK_CombatSkill_Reverse_Page_{0}", i - 1));
						}
					}
				}
			}
			UIElement element = this.Element;
			if (element != null)
			{
				element.ShowAfterRefresh();
			}
		}
	}

	// Token: 0x0600291E RID: 10526 RVA: 0x00132538 File Offset: 0x00130738
	protected override void InitItemDisableFunctionList(ItemDisplayData itemDisplayData)
	{
		base.InitItemDisableFunctionList(itemDisplayData);
		SkillBookItem configData = SkillBook.Instance[itemDisplayData.Key.TemplateId];
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

	// Token: 0x04001DF6 RID: 7670
	private bool _isCombatSkill;

	// Token: 0x04001DF7 RID: 7671
	private static readonly Dictionary<int, string> ColorMap = new Dictionary<int, string>
	{
		{
			0,
			"BehaviorType_Just"
		},
		{
			1,
			"BehaviorType_Kind"
		},
		{
			2,
			"BehaviorType_Even"
		},
		{
			3,
			"BehaviorType_Rebel"
		},
		{
			4,
			"BehaviorType_Egoistic"
		}
	};
}
