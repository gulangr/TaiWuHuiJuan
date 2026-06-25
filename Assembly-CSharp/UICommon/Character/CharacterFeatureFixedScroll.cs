using System;
using System.Collections.Generic;
using Config;
using UICommon.Character.Elements;
using UnityEngine;

namespace UICommon.Character
{
	// Token: 0x020005D1 RID: 1489
	public class CharacterFeatureFixedScroll
	{
		// Token: 0x170008E2 RID: 2274
		// (get) Token: 0x06004682 RID: 18050 RVA: 0x00211084 File Offset: 0x0020F284
		private Transform currentLayout
		{
			get
			{
				return (this.currentLanguageType == LocalStringManager.LanguageType.CN) ? this._contentHolderCN : this._contentHolderEN;
			}
		}

		// Token: 0x170008E3 RID: 2275
		// (get) Token: 0x06004683 RID: 18051 RVA: 0x0021109C File Offset: 0x0020F29C
		private Refers currentTemplate
		{
			get
			{
				return (this.currentLanguageType == LocalStringManager.LanguageType.CN) ? ((this._scrollItemTemplateGroup != null) ? this._scrollItemTemplateGroup.GetValueOrDefault().CnItemTemplate : null) : ((this._scrollItemTemplateGroup != null) ? this._scrollItemTemplateGroup.GetValueOrDefault().OtherItemTemplate : null);
			}
		}

		// Token: 0x06004684 RID: 18052 RVA: 0x002110EC File Offset: 0x0020F2EC
		public CharacterFeatureFixedScroll(Transform contentHolderCn, Transform contentHolderEn, bool enableDeleteFeature = false, CharacterFeatureFixedScroll.ScrollItemTemplateGroup? scrollItemTemplateGroup = null)
		{
			bool flag = null == contentHolderCn || null == contentHolderEn;
			if (flag)
			{
				throw new Exception("contentHolder can not be null to create CharacterFeatureScroll element!");
			}
			this._contentHolderCN = contentHolderCn;
			this._contentHolderEN = contentHolderEn;
			this._showFeatureList = new List<short>();
			this._enableDeleteFeature = enableDeleteFeature;
			this._scrollItemTemplateGroup = scrollItemTemplateGroup;
			this.RefreshElement();
		}

		// Token: 0x06004685 RID: 18053 RVA: 0x00211160 File Offset: 0x0020F360
		public void SetShowFeatureListFromOutside(List<short> featureIdList)
		{
			this._showFeatureList.Clear();
			this._showFeatureList.AddRange(featureIdList);
			this.RefreshElement();
		}

		// Token: 0x06004686 RID: 18054 RVA: 0x00211184 File Offset: 0x0020F384
		public void RefreshElement()
		{
			bool flag = null == this._contentHolderCN || null == this._contentHolderEN;
			if (!flag)
			{
				this._contentHolderCN.gameObject.SetActive(this.currentLanguageType == LocalStringManager.LanguageType.CN);
				this._contentHolderEN.gameObject.SetActive(this.currentLanguageType == LocalStringManager.LanguageType.EN);
				List<CharacterFeatureItem> itemConfigs = new List<CharacterFeatureItem>();
				for (int i = 0; i < this._showFeatureList.Count; i++)
				{
					CharacterFeatureItem config = CharacterFeature.Instance[this._showFeatureList[i]];
					bool flag2 = !config.Hidden;
					if (flag2)
					{
						itemConfigs.Add(config);
					}
				}
				CommonUtils.PrepareEnoughChildren(this.currentLayout, this.currentTemplate.gameObject, itemConfigs.Count, null);
				for (int j = 0; j < itemConfigs.Count; j++)
				{
					Refers refers = this.currentLayout.GetChild(j).GetComponent<Refers>();
					this.OnFeatureItemRender((int)itemConfigs[j].TemplateId, refers);
				}
			}
		}

		// Token: 0x06004687 RID: 18055 RVA: 0x002112AD File Offset: 0x0020F4AD
		public void ResetToEmpty()
		{
			this._contentHolderCN.gameObject.SetActive(false);
			this._contentHolderEN.gameObject.SetActive(false);
			List<short> showFeatureList = this._showFeatureList;
			if (showFeatureList != null)
			{
				showFeatureList.Clear();
			}
		}

		// Token: 0x06004688 RID: 18056 RVA: 0x002112E8 File Offset: 0x0020F4E8
		private void OnFeatureItemRender(int templateId, Refers refers)
		{
			CharacterFeatureView featureView = refers.GetComponent<CharacterFeatureView>();
			featureView.Set(CharacterFeature.Instance[templateId], -1, false);
		}

		// Token: 0x06004689 RID: 18057 RVA: 0x00211314 File Offset: 0x0020F514
		public void OnLanguageChange(LocalStringManager.LanguageType languageType)
		{
			bool flag = this._scrollItemTemplateGroup == null;
			if (!flag)
			{
				this.currentLanguageType = languageType;
				this.RefreshElement();
			}
		}

		// Token: 0x040030D9 RID: 12505
		private readonly Transform _contentHolderCN;

		// Token: 0x040030DA RID: 12506
		private readonly Transform _contentHolderEN;

		// Token: 0x040030DB RID: 12507
		private LocalStringManager.LanguageType currentLanguageType = LocalStringManager.LanguageType.CN;

		// Token: 0x040030DC RID: 12508
		private readonly List<short> _showFeatureList;

		// Token: 0x040030DD RID: 12509
		private bool _enableDeleteFeature = false;

		// Token: 0x040030DE RID: 12510
		private CharacterFeatureFixedScroll.ScrollItemTemplateGroup? _scrollItemTemplateGroup;

		// Token: 0x040030DF RID: 12511
		public const int CnItemPerRowCount = 6;

		// Token: 0x040030E0 RID: 12512
		public const int OtherItemPerRowCount = 3;

		// Token: 0x02001990 RID: 6544
		public struct ScrollItemTemplateGroup
		{
			// Token: 0x0600DBA1 RID: 56225 RVA: 0x005CA826 File Offset: 0x005C8A26
			public ScrollItemTemplateGroup(Refers cnItemTemplate, Refers otherItemTemplate)
			{
				this.CnItemTemplate = cnItemTemplate;
				this.OtherItemTemplate = otherItemTemplate;
			}

			// Token: 0x0400B2B4 RID: 45748
			public readonly Refers CnItemTemplate;

			// Token: 0x0400B2B5 RID: 45749
			public readonly Refers OtherItemTemplate;
		}
	}
}
