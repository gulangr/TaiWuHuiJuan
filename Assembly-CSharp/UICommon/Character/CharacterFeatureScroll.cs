using System;
using System.Collections.Generic;
using CharacterDataMonitor;
using Config;
using GameData.Domains.Character;
using GameData.Domains.Character.Display;
using GameData.Serializer;
using GameData.Utilities;
using TMPro;
using UICommon.Character.Elements;
using UnityEngine;

namespace UICommon.Character
{
	// Token: 0x020005D3 RID: 1491
	public class CharacterFeatureScroll : CharacterUIElement
	{
		// Token: 0x170008E7 RID: 2279
		// (get) Token: 0x06004695 RID: 18069 RVA: 0x00211768 File Offset: 0x0020F968
		private FeatureMonitor Item
		{
			get
			{
				return this.MonitorDataItem as FeatureMonitor;
			}
		}

		// Token: 0x06004696 RID: 18070 RVA: 0x00211778 File Offset: 0x0020F978
		public CharacterFeatureScroll(InfinityScrollLegacy scroll, Refers totalMedalRefers = null, bool enableDeleteFeature = false, CharacterFeatureScroll.ScrollItemTemplateGroup? scrollItemTemplateGroup = null)
		{
			bool flag = null == scroll;
			if (flag)
			{
				throw new Exception("scroll can not be null to create CharacterFeatureScroll element!");
			}
			this._infinityScroll = scroll;
			this._infinityScroll.OnItemRender = new Action<int, Refers>(this.OnFeatureItemRender);
			this._showFeatureList = new List<short>();
			this._infinityScroll.SetDataCount(0);
			this._totalMedalRefers = totalMedalRefers;
			this._enableDeleteFeature = enableDeleteFeature;
			this._scrollItemTemplateGroup = scrollItemTemplateGroup;
		}

		// Token: 0x06004697 RID: 18071 RVA: 0x002117F6 File Offset: 0x0020F9F6
		internal override void BindEvent()
		{
			this.Item.AddFeatureListener(new Action(this.FillElement));
		}

		// Token: 0x06004698 RID: 18072 RVA: 0x00211812 File Offset: 0x0020FA12
		public override void UnbindEvent()
		{
			this.Item.RemoveFeatureListener(new Action(this.FillElement));
		}

		// Token: 0x06004699 RID: 18073 RVA: 0x0021182E File Offset: 0x0020FA2E
		public void SetShowFeatureListFromOutside(List<short> featureIdList)
		{
			this._showFeatureList.Clear();
			this._showFeatureList.AddRange(featureIdList);
			this._infinityScroll.UpdateData(this._showFeatureList.Count);
		}

		// Token: 0x0600469A RID: 18074 RVA: 0x00211864 File Offset: 0x0020FA64
		public override void FillElement()
		{
			bool flag = null == this._infinityScroll;
			if (flag)
			{
				base.CharacterId = -1;
			}
			else
			{
				this._showFeatureList.Clear();
				for (int i = 0; i < this.Item.FeatureIds.Count; i++)
				{
					CharacterFeatureItem config = CharacterFeature.Instance[this.Item.FeatureIds[i]];
					bool flag2 = !config.Hidden;
					if (flag2)
					{
						this._showFeatureList.Add(this.Item.FeatureIds[i]);
					}
				}
				this._infinityScroll.UpdateData(this._showFeatureList.Count);
				bool flag3 = this._totalMedalRefers != null;
				if (flag3)
				{
					CharacterDomainMethod.AsyncCall.GetGroupCharDisplayDataList(null, new List<int>
					{
						base.CharacterId
					}, delegate(int offset, RawDataPool dataPool)
					{
						List<GroupCharDisplayData> dataList = null;
						Serializer.Deserialize(dataPool, offset, ref dataList);
						GroupCharDisplayData data = dataList[0];
						Refers attack = this._totalMedalRefers.CGet<Refers>("Attack");
						Refers defence = this._totalMedalRefers.CGet<Refers>("Defence");
						Refers wisdom = this._totalMedalRefers.CGet<Refers>("Wisdom");
						attack.CGet<CImage>("Icon").SetSprite((data.AttackMedal > 0) ? "ui_sp_icon_characteristic_10" : ((data.AttackMedal < 0) ? "ui_sp_icon_characteristic_4" : "ui_sp_icon_characteristic_7"), false, null);
						attack.CGet<TextMeshProUGUI>("Value").text = string.Format(" x{0}", Mathf.Abs(data.AttackMedal));
						defence.CGet<CImage>("Icon").SetSprite((data.DefenceMedal > 0) ? "ui_sp_icon_characteristic_9" : ((data.DefenceMedal < 0) ? "ui_sp_icon_characteristic_3" : "ui_sp_icon_characteristic_6"), false, null);
						defence.CGet<TextMeshProUGUI>("Value").text = string.Format(" x{0}", Mathf.Abs(data.DefenceMedal));
						wisdom.CGet<CImage>("Icon").SetSprite((data.WisdomMedal > 0) ? "ui_sp_icon_characteristic_11" : ((data.WisdomMedal < 0) ? "ui_sp_icon_characteristic_5" : "ui_sp_icon_characteristic_8"), false, null);
						wisdom.CGet<TextMeshProUGUI>("Value").text = string.Format(" x{0}", Mathf.Abs(data.WisdomMedal));
					});
				}
			}
		}

		// Token: 0x0600469B RID: 18075 RVA: 0x00211954 File Offset: 0x0020FB54
		public override void ResetToEmpty()
		{
			List<short> showFeatureList = this._showFeatureList;
			if (showFeatureList != null)
			{
				showFeatureList.Clear();
			}
			bool flag = null == this._infinityScroll;
			if (flag)
			{
				bool flag2 = this.MonitorDataItem != null;
				if (flag2)
				{
					this.UnbindEvent();
					this.MonitorDataItem = null;
				}
			}
			else
			{
				this._infinityScroll.UpdateData(this._showFeatureList.Count);
			}
		}

		// Token: 0x0600469C RID: 18076 RVA: 0x002119BC File Offset: 0x0020FBBC
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

		// Token: 0x0600469D RID: 18077 RVA: 0x00211A68 File Offset: 0x0020FC68
		public override MonitorDataItemBase GetMonitorItem(int charId)
		{
			return SingletonObject.getInstance<CharacterMonitorModel>().GetMonitorItem<FeatureMonitor>(charId, this.IsDead);
		}

		// Token: 0x0600469E RID: 18078 RVA: 0x00211A8C File Offset: 0x0020FC8C
		public void OnLanguageChange(LocalStringManager.LanguageType languageType)
		{
			bool flag = this._scrollItemTemplateGroup == null;
			if (!flag)
			{
				int count = (languageType == LocalStringManager.LanguageType.CN) ? 6 : 3;
				Refers itemTemplate = (languageType == LocalStringManager.LanguageType.CN) ? this._scrollItemTemplateGroup.Value.CnItemTemplate : this._scrollItemTemplateGroup.Value.OtherItemTemplate;
				this.UpdateItemPerRowCount(count, itemTemplate);
				this._scrollItemTemplateGroup.Value.CnItemTemplate.gameObject.SetActive(false);
				this._scrollItemTemplateGroup.Value.OtherItemTemplate.gameObject.SetActive(false);
			}
		}

		// Token: 0x0600469F RID: 18079 RVA: 0x00211B1D File Offset: 0x0020FD1D
		private void UpdateItemPerRowCount(int countPerRow, Refers itemTemplate)
		{
			this._infinityScroll.UpdateStyle(InfinityScrollLegacy.ScrollDirection.FromTop, countPerRow, this._infinityScroll.Gap, this._infinityScroll.Padding, itemTemplate);
		}

		// Token: 0x040030E9 RID: 12521
		private readonly InfinityScrollLegacy _infinityScroll;

		// Token: 0x040030EA RID: 12522
		private readonly Refers _totalMedalRefers;

		// Token: 0x040030EB RID: 12523
		private readonly List<short> _showFeatureList;

		// Token: 0x040030EC RID: 12524
		private bool _enableDeleteFeature = false;

		// Token: 0x040030ED RID: 12525
		private CharacterFeatureScroll.ScrollItemTemplateGroup? _scrollItemTemplateGroup;

		// Token: 0x040030EE RID: 12526
		public const int CnItemPerRowCount = 6;

		// Token: 0x040030EF RID: 12527
		public const int OtherItemPerRowCount = 3;

		// Token: 0x02001992 RID: 6546
		public struct ScrollItemTemplateGroup
		{
			// Token: 0x0600DBA3 RID: 56227 RVA: 0x005CA848 File Offset: 0x005C8A48
			public ScrollItemTemplateGroup(Refers cnItemTemplate, Refers otherItemTemplate)
			{
				this.CnItemTemplate = cnItemTemplate;
				this.OtherItemTemplate = otherItemTemplate;
			}

			// Token: 0x0400B2B8 RID: 45752
			public readonly Refers CnItemTemplate;

			// Token: 0x0400B2B9 RID: 45753
			public readonly Refers OtherItemTemplate;
		}
	}
}
