using System;
using Config;
using FrameWork;
using GameData.Domains.Character.AvatarSystem;
using GameData.Domains.Item.Display;
using GameData.Domains.World;
using TMPro;
using UnityEngine;

// Token: 0x0200027F RID: 639
public class MouseTipClothing : MouseTipItem
{
	// Token: 0x17000487 RID: 1159
	// (get) Token: 0x06002949 RID: 10569 RVA: 0x00134718 File Offset: 0x00132918
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

	// Token: 0x0600294A RID: 10570 RVA: 0x0013474C File Offset: 0x0013294C
	protected override void Init(ArgumentBox argsBox)
	{
		base.Init(argsBox);
		ItemDisplayData itemData;
		argsBox.Get<ItemDisplayData>("ItemData", out itemData);
		bool templateDataOnly;
		argsBox.Get("TemplateDataOnly", out templateDataOnly);
		argsBox.Get("IsInCompareUI", out this._isInCompareUI);
		bool flag = !argsBox.Get("CharId", out this._charId);
		if (flag)
		{
			this._charId = -1;
		}
		this._itemData = itemData;
		ClothingItem configData = Clothing.Instance[itemData.Key.TemplateId];
		base.CGet<TextMeshProUGUI>("Name").text = configData.Name;
		base.CGet<CImage>("GradeBack").SetSprite(ItemView.GetGradeIcon(configData.Grade), false, null);
		base.CGet<TextMeshProUGUI>("GradeName").text = LocalStringManager.Get(string.Format("LK_ShortGrade_{0}", configData.Grade));
		base.CGet<TextMeshProUGUI>("Grade").text = (LocalStringManager.Get(string.Format("LK_Num_{0}", (int)(9 - configData.Grade))) + LocalStringManager.Get(LanguageKey.LK_Item_Grade)).SetColor(Colors.Instance.GradeColors[(int)configData.Grade]);
		base.CGet<TextMeshProUGUI>("Value").text = (templateDataOnly ? configData.BaseValue.ToString() : itemData.Value.ToString());
		base.CGet<GameObject>("Material").SetActive(!templateDataOnly);
		base.CGet<CImage>("ItemIcon").SetSprite(configData.Icon, false, null);
		bool flag2 = SharedMethods.SmallVillageXiangshuProgress() && configData.TemplateId == 66;
		if (flag2)
		{
			base.SetItemDesc(configData.SmallVillageDesc, itemData.LoveTokenDataItem);
		}
		else
		{
			base.SetItemDesc(configData.Desc, itemData.LoveTokenDataItem);
		}
		base.CGet<TextMeshProUGUI>("SubType").text = LocalStringManager.Get(string.Format("LK_ItemSubType_{0}", configData.ItemSubType));
		base.CGet<TextMeshProUGUI>("Weight").text = NumberFormatUtils.FormatItemWeight(itemData.Weight);
		AvatarManager avatarManager = SingletonObject.getInstance<AvatarManager>();
		short maleCharm = avatarManager.GetAsset(3, EAvatarElementsType.Cloth, new short[]
		{
			configData.DisplayId
		}).Config.ElemCharm;
		short femaleCharm = avatarManager.GetAsset(4, EAvatarElementsType.Cloth, new short[]
		{
			configData.DisplayId
		}).Config.ElemCharm;
		base.CGet<TextMeshProUGUI>("AddCharmMale").text = ((maleCharm >= 0) ? string.Format("+{0}", maleCharm) : "");
		base.CGet<TextMeshProUGUI>("ReduceCharmMale").text = ((maleCharm >= 0) ? "" : maleCharm.ToString());
		base.CGet<TextMeshProUGUI>("AddCharmFemale").text = ((femaleCharm >= 0) ? string.Format("+{0}", femaleCharm) : "");
		base.CGet<TextMeshProUGUI>("ReduceCharmFemale").text = ((femaleCharm >= 0) ? "" : femaleCharm.ToString());
		this.InitItemDisableFunctionList(itemData);
		base.RefreshDisableFunction();
		base.RefreshHoldCount();
		base.RefreshHotkeyDisplayLockItem();
		bool isWeaved = this._itemData.IsWeaved;
		base.CGet<GameObject>("Weave").SetActive(isWeaved);
		bool flag3 = isWeaved;
		if (flag3)
		{
			ClothingItem weavedClothingConfig = Clothing.Instance[this._itemData.WeavedClothingTemplateId];
			Color color = Colors.Instance.GradeColors[(int)weavedClothingConfig.Grade];
			string weavedName = weavedClothingConfig.Name.SetColor(color);
			base.CGet<TextMeshProUGUI>("WeaveTarget").text = weavedName;
		}
		base.RefreshEquipmentEffect(this._itemData);
		base.PostInit();
	}

	// Token: 0x0600294B RID: 10571 RVA: 0x00134B09 File Offset: 0x00132D09
	private void Update()
	{
		base.UpdateCompareCommonPart();
		base.UpdateMoreInfoCtrl();
	}

	// Token: 0x0600294C RID: 10572 RVA: 0x00134B1A File Offset: 0x00132D1A
	public override void SetNewData(ArgumentBox argsBox)
	{
		this.Init(argsBox);
	}

	// Token: 0x0600294D RID: 10573 RVA: 0x00134B28 File Offset: 0x00132D28
	protected override void InitItemDisableFunctionList(ItemDisplayData itemDisplayData)
	{
		base.InitItemDisableFunctionList(itemDisplayData);
		ClothingItem configData = Clothing.Instance[itemDisplayData.Key.TemplateId];
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
}
