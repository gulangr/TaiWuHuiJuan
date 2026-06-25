using System;
using Config;
using FrameWork;
using Game.Views.MouseTips.Item.Common;
using GameData.Domains.Item;
using GameData.Domains.Item.Display;
using GameData.Serializer;
using GameData.Utilities;
using TMPro;
using UnityEngine;

namespace Game.Views.MouseTips.Item
{
	// Token: 0x02000899 RID: 2201
	public class TooltipBook : TooltipItemBase
	{
		// Token: 0x17000C86 RID: 3206
		// (get) Token: 0x0600695B RID: 26971 RVA: 0x00306C63 File Offset: 0x00304E63
		protected override bool CanStick
		{
			get
			{
				return true;
			}
		}

		// Token: 0x0600695C RID: 26972 RVA: 0x00306C68 File Offset: 0x00304E68
		protected override void Init(ArgumentBox argsBox)
		{
			argsBox.Get<ItemDisplayData>("ItemData", out this._itemData);
			argsBox.Get("ShowPageInfo", out this._showPageInfo);
			argsBox.Get("TemplateDataOnly", out this._templateDataOnly);
			this._itemKey = this._itemData.RealKey;
			this.configData = SkillBook.Instance[this._itemKey.TemplateId];
			this._isCombatSkill = (this.configData.ItemSubType == 1001);
			base.Init(argsBox);
			base.PostInit();
			this.DisableDetail = ((!this.HasSelfPoison && !this.HasAttachedPoison && !this._isCombatSkill) || this._templateDataOnly);
			this.NeedWaitData = this._showPageInfo;
			bool showPageInfo = this._showPageInfo;
			if (showPageInfo)
			{
				ItemDomainMethod.AsyncCall.GetSkillBookPagesInfo(this, this._itemKey, new AsyncMethodCallbackDelegate(this.OnGetPageInfo));
			}
			else
			{
				this.Refresh();
			}
		}

		// Token: 0x0600695D RID: 26973 RVA: 0x00306D60 File Offset: 0x00304F60
		public override void Refresh()
		{
			base.Refresh();
			this.layoutPage.gameObject.SetActive(this._showPageInfo);
			this.rootHide.gameObject.SetActive(!this._showPageInfo);
			this.RefreshDirection(this._showPageInfo && this.configData.CombatSkillTemplateId >= 0);
			UIElement element = this.Element;
			if (element != null)
			{
				element.ShowAfterRefresh();
			}
		}

		// Token: 0x0600695E RID: 26974 RVA: 0x00306DDB File Offset: 0x00304FDB
		protected override void RefreshCommonArea()
		{
			this.commonArea.RefreshBook(this._itemData);
		}

		// Token: 0x0600695F RID: 26975 RVA: 0x00306DF0 File Offset: 0x00304FF0
		private void OnGetPageInfo(int offset, RawDataPool dataPool)
		{
			SkillBookPageDisplayData displayData = null;
			Serializer.Deserialize(dataPool, offset, ref displayData);
			bool flag = displayData == null || !displayData.ItemKey.Equals(this._itemKey);
			if (!flag)
			{
				this.Refresh();
				int count = this._isCombatSkill ? 6 : 5;
				TooltipBookPage[] pages = this.layoutPage.GetComponentsInChildren<TooltipBookPage>(true);
				for (int i = 0; i < count; i++)
				{
					sbyte type = displayData.Type.GetOrDefault(i);
					sbyte progress = displayData.ReadingProgress.GetOrDefault(i);
					sbyte state = displayData.State.GetOrDefault(i);
					pages[i].Refresh(this._isCombatSkill, i, type, progress, state);
					pages[i].gameObject.SetActive(true);
				}
				for (int j = count; j < pages.Length; j++)
				{
					pages[j].gameObject.SetActive(false);
				}
				this.layoutPageDetail.gameObject.SetActive(this._isCombatSkill);
				bool flag2 = !this._isCombatSkill;
				if (!flag2)
				{
					TooltipBookCombatPageDetail[] combatPageDetails = this.layoutPageDetail.GetComponentsInChildren<TooltipBookCombatPageDetail>(true);
					for (int index = 0; index < combatPageDetails.Length; index++)
					{
						TooltipBookCombatPageDetail page = combatPageDetails[index];
						page.Refresh(index, displayData);
					}
				}
			}
		}

		// Token: 0x06006960 RID: 26976 RVA: 0x00306F48 File Offset: 0x00305148
		private void RefreshDirection(bool isShow)
		{
			this.directionArea.SetActive(isShow);
			bool flag = !isShow;
			if (!flag)
			{
				this.directEffectTitleText.text = LanguageKey.LK_CombatSkill_Direct_Effect.Tr();
				this.directEffectIcon.SetSprite("ui9_icon_combat_skill_effect_direction_0", false, null);
				this.reverseEffectTitleText.text = LanguageKey.LK_CombatSkill_Reverse_Effect.Tr();
				this.reverseEffectIcon.SetSprite("ui9_icon_combat_skill_effect_direction_1", false, null);
				CombatSkillItem skillConfig = CombatSkill.Instance[this.configData.CombatSkillTemplateId];
				string directDesc = CommonUtils.GetSpecialEffectDesc(skillConfig.DirectEffectID).SetColor("brightblue");
				string reverseDesc = CommonUtils.GetSpecialEffectDesc(skillConfig.ReverseEffectID).SetColor("brightred");
				this.directEffectDescText.text = "     " + directDesc;
				this.reverseEffectDescText.text = "     " + reverseDesc;
			}
		}

		// Token: 0x06006961 RID: 26977 RVA: 0x00307034 File Offset: 0x00305234
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

		// Token: 0x04004B9D RID: 19357
		[Header("书籍内容")]
		[SerializeField]
		private Transform layoutPage;

		// Token: 0x04004B9E RID: 19358
		[SerializeField]
		private GameObject rootHide;

		// Token: 0x04004B9F RID: 19359
		[Header("正逆练特效")]
		[SerializeField]
		private GameObject directionArea;

		// Token: 0x04004BA0 RID: 19360
		[SerializeField]
		private TextMeshProUGUI directEffectTitleText;

		// Token: 0x04004BA1 RID: 19361
		[SerializeField]
		private CImage directEffectIcon;

		// Token: 0x04004BA2 RID: 19362
		[SerializeField]
		private TextMeshProUGUI directEffectDescText;

		// Token: 0x04004BA3 RID: 19363
		[SerializeField]
		private TextMeshProUGUI reverseEffectTitleText;

		// Token: 0x04004BA4 RID: 19364
		[SerializeField]
		private CImage reverseEffectIcon;

		// Token: 0x04004BA5 RID: 19365
		[SerializeField]
		private TextMeshProUGUI reverseEffectDescText;

		// Token: 0x04004BA6 RID: 19366
		[Header("详细书籍内容")]
		[SerializeField]
		private Transform layoutPageDetail;

		// Token: 0x04004BA7 RID: 19367
		private SkillBookItem configData;

		// Token: 0x04004BA8 RID: 19368
		private bool _isCombatSkill;

		// Token: 0x04004BA9 RID: 19369
		private bool _showPageInfo;
	}
}
