using System;
using System.Collections.Generic;
using CharacterDataMonitor;
using Config;
using UICommon.Character.Elements;
using UnityEngine;

namespace UICommon.Character
{
	// Token: 0x020005D2 RID: 1490
	public class CharacterFeatureMonitorFixedScroll : CharacterUIElement
	{
		// Token: 0x170008E4 RID: 2276
		// (get) Token: 0x0600468A RID: 18058 RVA: 0x00211344 File Offset: 0x0020F544
		private FeatureMonitor Item
		{
			get
			{
				return this.MonitorDataItem as FeatureMonitor;
			}
		}

		// Token: 0x170008E5 RID: 2277
		// (get) Token: 0x0600468B RID: 18059 RVA: 0x00211351 File Offset: 0x0020F551
		private Transform currentLayout
		{
			get
			{
				return (this.currentLanguageType == LocalStringManager.LanguageType.CN) ? this._contentHolderCN : this._contentHolderEN;
			}
		}

		// Token: 0x170008E6 RID: 2278
		// (get) Token: 0x0600468C RID: 18060 RVA: 0x0021136C File Offset: 0x0020F56C
		private Refers currentTemplate
		{
			get
			{
				return (this.currentLanguageType == LocalStringManager.LanguageType.CN) ? ((this._scrollItemTemplateGroup != null) ? this._scrollItemTemplateGroup.GetValueOrDefault().CnItemTemplate : null) : ((this._scrollItemTemplateGroup != null) ? this._scrollItemTemplateGroup.GetValueOrDefault().OtherItemTemplate : null);
			}
		}

		// Token: 0x0600468D RID: 18061 RVA: 0x002113BC File Offset: 0x0020F5BC
		public CharacterFeatureMonitorFixedScroll(Transform contentHolderCn, Transform contentHolderEn, bool enableDeleteFeature = false, CharacterFeatureMonitorFixedScroll.ScrollItemTemplateGroup? scrollItemTemplateGroup = null)
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
			this.FillElement();
		}

		// Token: 0x0600468E RID: 18062 RVA: 0x00211430 File Offset: 0x0020F630
		internal override void BindEvent()
		{
			this.Item.AddFeatureListener(new Action(this.FillElement));
		}

		// Token: 0x0600468F RID: 18063 RVA: 0x0021144C File Offset: 0x0020F64C
		public override void UnbindEvent()
		{
			this.Item.RemoveFeatureListener(new Action(this.FillElement));
		}

		// Token: 0x06004690 RID: 18064 RVA: 0x00211468 File Offset: 0x0020F668
		public override void FillElement()
		{
			bool flag = null == this._contentHolderCN || null == this._contentHolderEN;
			if (flag)
			{
				base.CharacterId = -1;
			}
			else
			{
				this._contentHolderCN.gameObject.SetActive(this.currentLanguageType == LocalStringManager.LanguageType.CN);
				this._contentHolderEN.gameObject.SetActive(this.currentLanguageType == LocalStringManager.LanguageType.EN);
				this._showFeatureList.Clear();
				bool flag2 = this.Item == null;
				if (!flag2)
				{
					for (int i = 0; i < this.Item.FeatureIds.Count; i++)
					{
						CharacterFeatureItem config = CharacterFeature.Instance[this.Item.FeatureIds[i]];
						bool flag3 = !config.Hidden;
						if (flag3)
						{
							this._showFeatureList.Add(this.Item.FeatureIds[i]);
						}
					}
					CommonUtils.PrepareEnoughChildren(this.currentLayout, this.currentTemplate.gameObject, this._showFeatureList.Count, null);
					for (int j = 0; j < this._showFeatureList.Count; j++)
					{
						Refers refers = this.currentLayout.GetChild(j).GetComponent<Refers>();
						this.OnFeatureItemRender(j, refers);
					}
				}
			}
		}

		// Token: 0x06004691 RID: 18065 RVA: 0x002115D0 File Offset: 0x0020F7D0
		public override void ResetToEmpty()
		{
			List<short> showFeatureList = this._showFeatureList;
			if (showFeatureList != null)
			{
				showFeatureList.Clear();
			}
			this._contentHolderCN.gameObject.SetActive(false);
			this._contentHolderEN.gameObject.SetActive(false);
			List<short> showFeatureList2 = this._showFeatureList;
			if (showFeatureList2 != null)
			{
				showFeatureList2.Clear();
			}
			bool flag = null == this._contentHolderCN || null == this._contentHolderEN;
			if (flag)
			{
				bool flag2 = this.MonitorDataItem != null;
				if (flag2)
				{
					this.UnbindEvent();
					this.MonitorDataItem = null;
				}
			}
		}

		// Token: 0x06004692 RID: 18066 RVA: 0x00211668 File Offset: 0x0020F868
		private void OnFeatureItemRender(int index, Refers refers)
		{
			bool flag = !this._showFeatureList.CheckIndex(index);
			if (!flag)
			{
				CharacterFeatureView featureView = refers.GetComponent<CharacterFeatureView>();
				bool flag2 = this.Item == null;
				if (flag2)
				{
					featureView.Set(CharacterFeature.Instance[this._showFeatureList[index]], -1, false);
				}
				else
				{
					featureView.Set(CharacterFeature.Instance[this._showFeatureList[index]], this.Item.CharacterId, this.Item.IsTaiwu);
				}
				bool enableDeleteFeature = this._enableDeleteFeature;
				if (enableDeleteFeature)
				{
					featureView.RefreshDeleteButton(this.Item.IsTaiwu);
				}
			}
		}

		// Token: 0x06004693 RID: 18067 RVA: 0x00211714 File Offset: 0x0020F914
		public override MonitorDataItemBase GetMonitorItem(int charId)
		{
			return SingletonObject.getInstance<CharacterMonitorModel>().GetMonitorItem<FeatureMonitor>(charId, this.IsDead);
		}

		// Token: 0x06004694 RID: 18068 RVA: 0x00211738 File Offset: 0x0020F938
		public void OnLanguageChange(LocalStringManager.LanguageType languageType)
		{
			bool flag = this._scrollItemTemplateGroup == null;
			if (!flag)
			{
				this.currentLanguageType = languageType;
				this.FillElement();
			}
		}

		// Token: 0x040030E1 RID: 12513
		private readonly Transform _contentHolderCN;

		// Token: 0x040030E2 RID: 12514
		private readonly Transform _contentHolderEN;

		// Token: 0x040030E3 RID: 12515
		private LocalStringManager.LanguageType currentLanguageType = LocalStringManager.LanguageType.CN;

		// Token: 0x040030E4 RID: 12516
		private readonly List<short> _showFeatureList;

		// Token: 0x040030E5 RID: 12517
		private bool _enableDeleteFeature = false;

		// Token: 0x040030E6 RID: 12518
		private CharacterFeatureMonitorFixedScroll.ScrollItemTemplateGroup? _scrollItemTemplateGroup;

		// Token: 0x040030E7 RID: 12519
		public const int CnItemPerRowCount = 6;

		// Token: 0x040030E8 RID: 12520
		public const int OtherItemPerRowCount = 3;

		// Token: 0x02001991 RID: 6545
		public struct ScrollItemTemplateGroup
		{
			// Token: 0x0600DBA2 RID: 56226 RVA: 0x005CA837 File Offset: 0x005C8A37
			public ScrollItemTemplateGroup(Refers cnItemTemplate, Refers otherItemTemplate)
			{
				this.CnItemTemplate = cnItemTemplate;
				this.OtherItemTemplate = otherItemTemplate;
			}

			// Token: 0x0400B2B6 RID: 45750
			public readonly Refers CnItemTemplate;

			// Token: 0x0400B2B7 RID: 45751
			public readonly Refers OtherItemTemplate;
		}
	}
}
