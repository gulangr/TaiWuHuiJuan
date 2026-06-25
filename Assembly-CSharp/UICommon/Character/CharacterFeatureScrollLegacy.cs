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
	// Token: 0x020005D4 RID: 1492
	public class CharacterFeatureScrollLegacy : CharacterUIElement
	{
		// Token: 0x170008E8 RID: 2280
		// (get) Token: 0x060046A1 RID: 18081 RVA: 0x00211CC9 File Offset: 0x0020FEC9
		private FeatureMonitor Item
		{
			get
			{
				return this.MonitorDataItem as FeatureMonitor;
			}
		}

		// Token: 0x060046A2 RID: 18082 RVA: 0x00211CD8 File Offset: 0x0020FED8
		public CharacterFeatureScrollLegacy(InfinityScrollLegacy scroll, Refers totalMedalRefers = null, bool enableDeleteFeature = false, CharacterFeatureScrollLegacy.ScrollItemTemplateGroup? scrollItemTemplateGroup = null)
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

		// Token: 0x060046A3 RID: 18083 RVA: 0x00211D56 File Offset: 0x0020FF56
		internal override void BindEvent()
		{
			this.Item.AddFeatureListener(new Action(this.FillElement));
		}

		// Token: 0x060046A4 RID: 18084 RVA: 0x00211D72 File Offset: 0x0020FF72
		public override void UnbindEvent()
		{
			this.Item.RemoveFeatureListener(new Action(this.FillElement));
		}

		// Token: 0x060046A5 RID: 18085 RVA: 0x00211D8E File Offset: 0x0020FF8E
		public void SetShowFeatureListFromOutside(List<short> featureIdList)
		{
			this._showFeatureList.Clear();
			this._showFeatureList.AddRange(featureIdList);
			this._infinityScroll.UpdateData(this._showFeatureList.Count);
		}

		// Token: 0x060046A6 RID: 18086 RVA: 0x00211DC4 File Offset: 0x0020FFC4
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

		// Token: 0x060046A7 RID: 18087 RVA: 0x00211EB4 File Offset: 0x002100B4
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

		// Token: 0x060046A8 RID: 18088 RVA: 0x00211F1C File Offset: 0x0021011C
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

		// Token: 0x060046A9 RID: 18089 RVA: 0x00211FC8 File Offset: 0x002101C8
		public override MonitorDataItemBase GetMonitorItem(int charId)
		{
			return SingletonObject.getInstance<CharacterMonitorModel>().GetMonitorItem<FeatureMonitor>(charId, this.IsDead);
		}

		// Token: 0x060046AA RID: 18090 RVA: 0x00211FEC File Offset: 0x002101EC
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

		// Token: 0x060046AB RID: 18091 RVA: 0x0021207D File Offset: 0x0021027D
		private void UpdateItemPerRowCount(int countPerRow, Refers itemTemplate)
		{
			this._infinityScroll.UpdateStyle(InfinityScrollLegacy.ScrollDirection.FromTop, countPerRow, this._infinityScroll.Gap, this._infinityScroll.Padding, itemTemplate);
		}

		// Token: 0x040030F0 RID: 12528
		private readonly InfinityScrollLegacy _infinityScroll;

		// Token: 0x040030F1 RID: 12529
		private readonly Refers _totalMedalRefers;

		// Token: 0x040030F2 RID: 12530
		private readonly List<short> _showFeatureList;

		// Token: 0x040030F3 RID: 12531
		private bool _enableDeleteFeature = false;

		// Token: 0x040030F4 RID: 12532
		private CharacterFeatureScrollLegacy.ScrollItemTemplateGroup? _scrollItemTemplateGroup;

		// Token: 0x040030F5 RID: 12533
		public const int CnItemPerRowCount = 6;

		// Token: 0x040030F6 RID: 12534
		public const int OtherItemPerRowCount = 3;

		// Token: 0x02001993 RID: 6547
		public struct ScrollItemTemplateGroup
		{
			// Token: 0x0600DBA4 RID: 56228 RVA: 0x005CA859 File Offset: 0x005C8A59
			public ScrollItemTemplateGroup(Refers cnItemTemplate, Refers otherItemTemplate)
			{
				this.CnItemTemplate = cnItemTemplate;
				this.OtherItemTemplate = otherItemTemplate;
			}

			// Token: 0x0400B2BA RID: 45754
			public readonly Refers CnItemTemplate;

			// Token: 0x0400B2BB RID: 45755
			public readonly Refers OtherItemTemplate;
		}
	}
}
