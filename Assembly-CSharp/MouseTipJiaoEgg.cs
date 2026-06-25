using System;
using Config;
using FrameWork;
using GameData.DLC.FiveLoong;
using GameData.Domains.Extra;
using GameData.Domains.Item.Display;
using GameData.Serializer;
using GameData.Utilities;
using TMPro;
using UnityEngine;

// Token: 0x020002A5 RID: 677
public class MouseTipJiaoEgg : MouseTipItem
{
	// Token: 0x1700049C RID: 1180
	// (get) Token: 0x06002A4B RID: 10827 RVA: 0x00143A64 File Offset: 0x00141C64
	protected override bool CanStick
	{
		get
		{
			return true;
		}
	}

	// Token: 0x06002A4C RID: 10828 RVA: 0x00143A68 File Offset: 0x00141C68
	protected override void Init(ArgumentBox argsBox)
	{
		base.Init(argsBox);
		argsBox.Get<ItemDisplayData>("ItemData", out this._itemData);
		this.SetDisplayData();
		this.SetJiaoParentsText();
		this.InitItemDisableFunctionList(this._itemData);
		base.RefreshDisableFunction();
		base.RefreshHoldCount();
		base.RefreshHotkeyDisplayLockItem();
	}

	// Token: 0x06002A4D RID: 10829 RVA: 0x00143AC0 File Offset: 0x00141CC0
	protected override void OnDisable()
	{
		base.OnDisable();
		this.HideJiaoParentsText();
	}

	// Token: 0x06002A4E RID: 10830 RVA: 0x00143AD4 File Offset: 0x00141CD4
	private void SetDisplayData()
	{
		MaterialItem configData = Config.Material.Instance[this._itemData.Key.TemplateId];
		TextMeshProUGUI currDurabilityYellow = base.CGet<TextMeshProUGUI>("CurrDurabilityYellow");
		TextMeshProUGUI currDurabilityRed = base.CGet<TextMeshProUGUI>("CurrDurabilityRed");
		base.CGet<CImage>("GradeBack").SetSprite(ItemView.GetGradeIcon(configData.Grade), false, null);
		base.CGet<TextMeshProUGUI>("GradeName").text = LocalStringManager.Get(string.Format("LK_ShortGrade_{0}", configData.Grade));
		base.CGet<TextMeshProUGUI>("Grade").text = (LocalStringManager.Get(string.Format("LK_Num_{0}", (int)(9 - configData.Grade))) + LocalStringManager.Get(LanguageKey.LK_Item_Grade)).SetColor(Colors.Instance.GradeColors[(int)configData.Grade]);
		base.CGet<JiaoEggView>("JiaoEggView").Refresh(this._itemData.JiaoLoongDisplayData, false);
		base.SetItemDesc(configData.Desc, this._itemData.LoveTokenDataItem);
		base.CGet<TextMeshProUGUI>("Weight").text = NumberFormatUtils.FormatItemWeight(this._itemData.Weight);
		base.CGet<TextMeshProUGUI>("SubType").text = LocalStringManager.Get(string.Format("LK_ItemSubType_{0}", configData.ItemSubType));
		base.CGet<TextMeshProUGUI>("Value").text = this._itemData.Value.ToString();
		base.CGet<GameObject>("Durability").SetActive(this._itemData.Durability > 0);
		bool hasHalfDurability = this._itemData.Durability > this._itemData.MaxDurability / 2;
		currDurabilityYellow.gameObject.SetActive(hasHalfDurability);
		currDurabilityRed.gameObject.SetActive(!hasHalfDurability);
		(hasHalfDurability ? currDurabilityYellow : currDurabilityRed).text = this._itemData.Durability.ToString();
		base.CGet<TextMeshProUGUI>("MaxDurability").text = string.Format("/{0}", this._itemData.MaxDurability);
		base.RefreshPoisons(configData.InnatePoisons, this._itemData);
	}

	// Token: 0x06002A4F RID: 10831 RVA: 0x00143D09 File Offset: 0x00141F09
	private void SetJiaoParentsText()
	{
		this.HideJiaoParentsText();
		ExtraDomainMethod.AsyncCall.GetJiaoByItemKey(this, this._itemData.Key, delegate(int offset, RawDataPool dataPool)
		{
			Serializer.Deserialize(dataPool, offset, ref this._jiao);
			base.CGet<TextMeshProUGUI>("Name").text = this._jiao.GetNameText();
			JiaoItem configData = Config.Jiao.Instance[this._jiao.TemplateId];
			base.CGet<TextMeshProUGUI>("JiaoType").text = configData.Name;
			base.CGet<TextMeshProUGUI>("Gender").text = LocalStringManager.Get(this._jiao.Gender ? LanguageKey.LK_Animal_Male : LanguageKey.LK_Animal_Female).ColorReplace();
			base.CGet<TextMeshProUGUI>("Behavior").text = LocalStringManager.Get(string.Format("LK_Goodness_{0}", this._jiao.Behavior)).ColorReplace();
			base.CGet<TextMeshProUGUI>("Generation").text = LocalStringManager.GetFormat(LanguageKey.LK_Generation_Content, this._jiao.Generation + 1);
			bool flag = this._jiao.FatherId > -1 && this._jiao.MotherId > -1;
			if (flag)
			{
				ExtraDomainMethod.AsyncCall.GetJiaoById(this, this._jiao.FatherId, new AsyncMethodCallbackDelegate(this.HandleJiaoFartherName));
				ExtraDomainMethod.AsyncCall.GetJiaoById(this, this._jiao.MotherId, new AsyncMethodCallbackDelegate(this.HandleJiaoMotherName));
			}
		});
	}

	// Token: 0x06002A50 RID: 10832 RVA: 0x00143D34 File Offset: 0x00141F34
	private void HandleJiaoFartherName(int offset, RawDataPool dataPool)
	{
		Serializer.Deserialize(dataPool2, offset2, ref this._fatherJiao);
		bool flag = this._fatherJiao == null;
		if (flag)
		{
			ExtraDomainMethod.AsyncCall.GetChildrenOfLoongById(this, this._jiao.FatherId, delegate(int offset, RawDataPool dataPool)
			{
				Serializer.Deserialize(dataPool, offset, ref this._fatherLoong);
				bool flag2 = this._fatherLoong == null;
				if (!flag2)
				{
					this.HandleloongText(this._fatherLoong.Key.TemplateId, true);
				}
			});
		}
		else
		{
			this.HandleJiaoText(this._fatherJiao.TemplateId, true);
		}
	}

	// Token: 0x06002A51 RID: 10833 RVA: 0x00143D94 File Offset: 0x00141F94
	private void HandleJiaoMotherName(int offset, RawDataPool dataPool)
	{
		Serializer.Deserialize(dataPool2, offset2, ref this._motherJiao);
		bool flag = this._motherJiao == null;
		if (flag)
		{
			ExtraDomainMethod.AsyncCall.GetChildrenOfLoongById(this, this._jiao.MotherId, delegate(int offset, RawDataPool dataPool)
			{
				Serializer.Deserialize(dataPool, offset, ref this._motherLoong);
				bool flag2 = this._motherLoong == null;
				if (!flag2)
				{
					this.HandleloongText(this._motherLoong.Key.TemplateId, false);
				}
			});
		}
		else
		{
			this.HandleJiaoText(this._motherJiao.TemplateId, false);
		}
	}

	// Token: 0x06002A52 RID: 10834 RVA: 0x00143DF4 File Offset: 0x00141FF4
	private void HandleJiaoText(short templateId, bool isFather)
	{
		JiaoItem configData = Config.Jiao.Instance.GetItem(templateId);
		base.CGet<TextMeshProUGUI>(isFather ? "FatherType" : "MotherType").text = configData.Name;
		string name = isFather ? this._fatherJiao.GetNameText() : this._motherJiao.GetNameText();
		base.CGet<TextMeshProUGUI>(isFather ? "FatherNickNameText" : "MotherNickNameText").text = name;
		base.CGet<GameObject>(isFather ? "FatherNickName" : "MotherNickName").SetActive(!name.Equals(configData.Name));
		base.CGet<GameObject>(isFather ? "Father" : "Mother").SetActive(true);
	}

	// Token: 0x06002A53 RID: 10835 RVA: 0x00143EB0 File Offset: 0x001420B0
	private void HandleloongText(short templateId, bool isFather)
	{
		CarrierItem configData = Carrier.Instance.GetItem(templateId);
		base.CGet<TextMeshProUGUI>(isFather ? "FatherType" : "MotherType").text = configData.Name;
		string name = isFather ? this._fatherLoong.GetNameText() : this._motherLoong.GetNameText();
		base.CGet<TextMeshProUGUI>(isFather ? "FatherNickNameText" : "MotherNickNameText").text = name;
		base.CGet<GameObject>(isFather ? "FatherNickName" : "MotherNickName").SetActive(!name.Equals(configData.Name));
		base.CGet<GameObject>(isFather ? "Father" : "Mother").SetActive(true);
	}

	// Token: 0x06002A54 RID: 10836 RVA: 0x00143F69 File Offset: 0x00142169
	private void HideJiaoParentsText()
	{
		base.CGet<GameObject>("Father").SetActive(false);
		base.CGet<GameObject>("Mother").SetActive(false);
	}

	// Token: 0x06002A55 RID: 10837 RVA: 0x00143F90 File Offset: 0x00142190
	protected override void InitItemDisableFunctionList(ItemDisplayData itemDisplayData)
	{
		base.InitItemDisableFunctionList(itemDisplayData);
		MaterialItem configData = Config.Material.Instance[itemDisplayData.Key.TemplateId];
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

	// Token: 0x04001EA8 RID: 7848
	private GameData.DLC.FiveLoong.Jiao _jiao;

	// Token: 0x04001EA9 RID: 7849
	private GameData.DLC.FiveLoong.Jiao _motherJiao;

	// Token: 0x04001EAA RID: 7850
	private GameData.DLC.FiveLoong.Jiao _fatherJiao;

	// Token: 0x04001EAB RID: 7851
	private ChildrenOfLoong _fatherLoong;

	// Token: 0x04001EAC RID: 7852
	private ChildrenOfLoong _motherLoong;
}
