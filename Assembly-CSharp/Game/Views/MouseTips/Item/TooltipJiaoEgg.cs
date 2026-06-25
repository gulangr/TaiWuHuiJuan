using System;
using Config;
using FrameWork;
using Game.Views.MouseTips.Item.Common;
using GameData.DLC.FiveLoong;
using GameData.Domains.Extra;
using GameData.Domains.Item.Display;
using GameData.Serializer;
using GameData.Utilities;
using UnityEngine;

namespace Game.Views.MouseTips.Item
{
	// Token: 0x020008A4 RID: 2212
	public class TooltipJiaoEgg : TooltipItemBase
	{
		// Token: 0x17000C90 RID: 3216
		// (get) Token: 0x060069C9 RID: 27081 RVA: 0x0030BCE2 File Offset: 0x00309EE2
		protected override bool CanStick
		{
			get
			{
				return true;
			}
		}

		// Token: 0x060069CA RID: 27082 RVA: 0x0030BCE8 File Offset: 0x00309EE8
		protected override void Init(ArgumentBox argsBox)
		{
			argsBox.Get<ItemDisplayData>("ItemData", out this._itemData);
			this._itemKey = this._itemData.RealKey;
			this.configData = Config.Material.Instance[this._itemData.Key.TemplateId];
			base.Init(argsBox);
			this.DisableDetail = true;
			this.InnatePoisons = this.configData.InnatePoisons;
			this.jiaoEggView.Refresh(this._itemData.JiaoLoongDisplayData, false);
			this.propertyFatherTitle.Set("", LanguageKey.LK_MouseTip_Jiao_Father.Tr(), "", true);
			this.propertyMotherTitle.Set("", LanguageKey.LK_MouseTip_Jiao_Mother.Tr(), "", true);
			this.SetJiaoParentsText();
			this.Refresh();
		}

		// Token: 0x060069CB RID: 27083 RVA: 0x0030BDC1 File Offset: 0x00309FC1
		public override void SetNewData(ArgumentBox argsBox)
		{
			this.Init(argsBox);
			this.Refresh();
		}

		// Token: 0x060069CC RID: 27084 RVA: 0x0030BDD3 File Offset: 0x00309FD3
		private void SetJiaoParentsText()
		{
			ExtraDomainMethod.AsyncCall.GetJiaoByItemKey(this, this._itemData.Key, delegate(int offset, RawDataPool dataPool)
			{
				Serializer.Deserialize(dataPool, offset, ref this._jiao);
				this.commonArea.SetName(this._jiao.GetNameText());
				JiaoItem configData = Config.Jiao.Instance[this._jiao.TemplateId];
				this.propertyType.Set("", LanguageKey.LK_Type.Tr(), configData.Name, true);
				string genderStr = CommonUtils.GetJiaoGenderString(this._jiao.Gender);
				this.propertyGender.Set("", LanguageKey.LK_Main_SummaryInfo_Gender.Tr(), genderStr, true);
				string behaviorStr = CommonUtils.GetBehaviorString(this._jiao.Behavior);
				this.propertyBehavior.Set("", LanguageKey.LK_Main_SummaryInfo_Behavior.Tr(), behaviorStr, true);
				string generationStr = LanguageKey.LK_Generation_Content.TrFormat(this._jiao.Generation + 1).SetColor("brightyellow");
				this.propertyGeneration.Set("", LanguageKey.LK_Generation_Title.Tr(), generationStr, true);
				bool showFather = this._jiao.FatherId > -1;
				this.rootFather.SetActive(showFather);
				bool flag = showFather;
				if (flag)
				{
					ExtraDomainMethod.AsyncCall.GetJiaoById(this, this._jiao.FatherId, new AsyncMethodCallbackDelegate(this.HandleJiaoFartherName));
				}
				bool showMother = this._jiao.MotherId > -1;
				this.rootMother.SetActive(showMother);
				bool flag2 = showMother;
				if (flag2)
				{
					ExtraDomainMethod.AsyncCall.GetJiaoById(this, this._jiao.MotherId, new AsyncMethodCallbackDelegate(this.HandleJiaoMotherName));
				}
				this.rootParent.SetActive(showFather && showMother);
			});
		}

		// Token: 0x060069CD RID: 27085 RVA: 0x0030BDF4 File Offset: 0x00309FF4
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

		// Token: 0x060069CE RID: 27086 RVA: 0x0030BE54 File Offset: 0x0030A054
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

		// Token: 0x060069CF RID: 27087 RVA: 0x0030BEB4 File Offset: 0x0030A0B4
		private void HandleJiaoText(short templateId, bool isFather)
		{
			JiaoItem configData = Config.Jiao.Instance.GetItem(templateId);
			TooltipItemProperty propertyType = isFather ? this.propertyFatherType : this.propertyMotherType;
			propertyType.Set("", LanguageKey.LK_Type.Tr(), configData.Name, true);
			TooltipItemProperty propertyName = isFather ? this.propertyFatherName : this.propertyMotherName;
			string name = isFather ? this._fatherJiao.GetNameText() : this._motherJiao.GetNameText();
			propertyName.Set("", LanguageKey.LK_NickName.Tr(), name, true);
			propertyName.gameObject.SetActive(!name.Equals(configData.Name));
		}

		// Token: 0x060069D0 RID: 27088 RVA: 0x0030BF60 File Offset: 0x0030A160
		private void HandleloongText(short templateId, bool isFather)
		{
			CarrierItem configData = Carrier.Instance.GetItem(templateId);
			TooltipItemProperty propertyType = isFather ? this.propertyFatherType : this.propertyMotherType;
			propertyType.Set("", LanguageKey.LK_Type.Tr(), configData.Name, true);
			TooltipItemProperty propertyName = isFather ? this.propertyFatherName : this.propertyMotherName;
			string name = isFather ? this._fatherLoong.GetNameText() : this._motherLoong.GetNameText();
			propertyName.Set("", LanguageKey.LK_NickName.Tr(), name, true);
			propertyName.gameObject.SetActive(!name.Equals(configData.Name));
		}

		// Token: 0x060069D1 RID: 27089 RVA: 0x0030C00C File Offset: 0x0030A20C
		protected override void InitItemDisableFunctionList()
		{
			base.InitItemDisableFunctionList();
			bool flag = !this.configData.Repairable;
			if (flag)
			{
				this._disableFunctionList.Add(ItemFunction.Repairable);
			}
			bool flag2 = !this.configData.Transferable;
			if (flag2)
			{
				this._disableFunctionList.Add(ItemFunction.Transferable);
			}
			bool flag3 = !this.configData.Poisonable;
			if (flag3)
			{
				this._disableFunctionList.Add(ItemFunction.Poisonable);
			}
			bool flag4 = !this.configData.Refinable;
			if (flag4)
			{
				this._disableFunctionList.Add(ItemFunction.Refinable);
			}
		}

		// Token: 0x04004C21 RID: 19489
		[SerializeField]
		private JiaoEggView jiaoEggView;

		// Token: 0x04004C22 RID: 19490
		[Header("基础属性")]
		[SerializeField]
		private TooltipItemProperty propertyType;

		// Token: 0x04004C23 RID: 19491
		[SerializeField]
		private TooltipItemProperty propertyGender;

		// Token: 0x04004C24 RID: 19492
		[SerializeField]
		private TooltipItemProperty propertyBehavior;

		// Token: 0x04004C25 RID: 19493
		[SerializeField]
		private TooltipItemProperty propertyGeneration;

		// Token: 0x04004C26 RID: 19494
		[Header("父母属性")]
		[SerializeField]
		private GameObject rootParent;

		// Token: 0x04004C27 RID: 19495
		[SerializeField]
		private GameObject rootFather;

		// Token: 0x04004C28 RID: 19496
		[SerializeField]
		private TooltipItemProperty propertyFatherTitle;

		// Token: 0x04004C29 RID: 19497
		[SerializeField]
		private TooltipItemProperty propertyFatherType;

		// Token: 0x04004C2A RID: 19498
		[SerializeField]
		private TooltipItemProperty propertyFatherName;

		// Token: 0x04004C2B RID: 19499
		[SerializeField]
		private GameObject rootMother;

		// Token: 0x04004C2C RID: 19500
		[SerializeField]
		private TooltipItemProperty propertyMotherTitle;

		// Token: 0x04004C2D RID: 19501
		[SerializeField]
		private TooltipItemProperty propertyMotherType;

		// Token: 0x04004C2E RID: 19502
		[SerializeField]
		private TooltipItemProperty propertyMotherName;

		// Token: 0x04004C2F RID: 19503
		private GameData.DLC.FiveLoong.Jiao _jiao;

		// Token: 0x04004C30 RID: 19504
		private GameData.DLC.FiveLoong.Jiao _motherJiao;

		// Token: 0x04004C31 RID: 19505
		private GameData.DLC.FiveLoong.Jiao _fatherJiao;

		// Token: 0x04004C32 RID: 19506
		private ChildrenOfLoong _fatherLoong;

		// Token: 0x04004C33 RID: 19507
		private ChildrenOfLoong _motherLoong;

		// Token: 0x04004C34 RID: 19508
		private MaterialItem configData;
	}
}
