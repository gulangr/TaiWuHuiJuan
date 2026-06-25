using System;
using System.Collections.Generic;
using Config;
using FrameWork;
using GameData.Domains.Map;
using GameData.Serializer;
using GameData.Utilities;
using TMPro;
using UnityEngine;

namespace Game.Views.MouseTips
{
	// Token: 0x0200086C RID: 2156
	public class MouseTipMapArea : MouseTipBase
	{
		// Token: 0x060067FE RID: 26622 RVA: 0x002F836C File Offset: 0x002F656C
		protected override void Init(ArgumentBox argsBox)
		{
			this._model = SingletonObject.getInstance<WorldMapModel>();
			argsBox.Get("areaId", out this._areaId);
			argsBox.Get<AreaDisplayData>("displayData", out this._displayData);
			this._cfg = this._model.Areas[(int)this._areaId].GetConfig();
			this.title.text = this._cfg.Name.SetColor(this._displayData.IsBroken ? "brokenarea" : "");
			this.desc.text = (this._displayData.HasSectZhujianSpecialMerchant ? (this._cfg.Desc + LanguageKey.LK_SectZhujian_SpecialMerchant_AreaTip.Tr()) : this._cfg.Desc).ColorReplace();
			this.Set(this.debt, this._model.GetAreaSpiritualDebt(this._areaId).ToString());
			bool show = false;
			show |= this.Set(this.infectedDemon, this._displayData.States, 17, LanguageKey.LK_MouseTip_MapArea_0_0);
			show |= this.Set(this.infected, this._displayData.States, 5, LanguageKey.LK_MouseTip_MapArea_0_1);
			show |= this.Set(this.legendaryBook, this._displayData.States, 2, LanguageKey.LK_MouseTip_MapArea_0_2);
			show |= this.Set(this.oldFriend, this._displayData.States, 6, LanguageKey.LK_MouseTip_MapArea_0_3);
			this.space1.SetActive(show);
			show = false;
			show |= this.Set(this.bamboo, this._displayData.States, 4, LanguageKey.LK_MouseTip_MapArea_1_0);
			show |= this.Set(this.beast, this._displayData.States, 20, LanguageKey.LK_MouseTip_MapArea_1_1);
			show |= this.Set(this.jiao, this._displayData.States, 7, LanguageKey.LK_MouseTip_MapArea_1_2);
			show |= this.Set(this.loong, this._displayData.States, 3, LanguageKey.LK_MouseTip_MapArea_1_3);
			this.space2.SetActive(show);
			show = false;
			show |= this.Set(this.unlocked, this._displayData.IsUnlocked ? LanguageKey.LK_MouseTip_MapArea_2_0.Tr() : null);
			show |= this.Set(this.taiwuVillage, (this._areaId == this._model.GetTaiwuVillageAreaId()) ? LanguageKey.LK_MouseTip_MapArea_2_1.Tr() : null);
			show |= this.Set(this.jieqing, null);
			show |= this.Set(this.destroyed, this._displayData.IsBroken ? LanguageKey.LK_MouseTip_MapArea_2_3.TrFormat(this._displayData.BrokenLevel) : null);
			this.space3.SetActive(show);
			show = false;
			show |= this.Set(this.normalAdv, this._displayData.States, 8, LanguageKey.Invalid);
			show |= this.Set(this.mainAdv, this._displayData.States, 0, LanguageKey.Invalid);
			show |= this.Set(this.sectAdv, this._displayData.States, 1, LanguageKey.Invalid);
			show |= this.Set(this.tombAdv, this._displayData.States, 21, LanguageKey.Invalid);
			show |= this.Set(this.majorAdv, this._displayData.States, 22, LanguageKey.Invalid);
			this.space4.SetActive(show);
			this.travelRoot.gameObject.SetActive(false);
			this.unlockRoot.gameObject.SetActive(!this._displayData.IsUnlocked);
			this.NeedDataListenerId = true;
			UIElement element = this.Element;
			element.OnListenerIdReady = (Action)Delegate.Combine(element.OnListenerIdReady, new Action(this.RequestData));
		}

		// Token: 0x060067FF RID: 26623 RVA: 0x002F8734 File Offset: 0x002F6934
		private void RequestData()
		{
			bool flag = this._areaId != this._model.CurrentAreaId && this._model.CurrentLocation.IsValid();
			if (flag)
			{
				MapDomainMethod.AsyncCall.GetTravelPreview(this, this._areaId, delegate(int offset, RawDataPool pool)
				{
					TravelPreviewDisplayData travelPreview = new TravelPreviewDisplayData();
					Serializer.Deserialize(pool, offset, ref travelPreview);
					bool flag2 = travelPreview.ToAreaId >= 0 && travelPreview.ToAreaId == this._areaId;
					if (flag2)
					{
						bool needUnlock = travelPreview.AuthorityCost > 0;
						bool flag3 = needUnlock;
						if (flag3)
						{
							this.Set(this.authorityCost, LanguageKey.LK_MouseTip_MapArea_5_1.TrFormat(CommonUtils.GetDisplayStringForNum(travelPreview.CurrentAuthority, 100000).SetColor((travelPreview.CurrentAuthority < travelPreview.AuthorityCost) ? "brightred" : "brightblue"), travelPreview.AuthorityCost));
							this.unlockRoot.gameObject.SetActive(true);
							this.authorityNotEnough.gameObject.SetActive(travelPreview.CurrentAuthority < travelPreview.AuthorityCost);
						}
						else
						{
							this.Set(this.moneyCost, LanguageKey.LK_MouseTip_MapArea_4_1.TrFormat(travelPreview.MoneyCost));
							this.Set(this.timeCost, LanguageKey.LK_MouseTip_MapArea_4_2.TrFormat(travelPreview.DaysCost));
							this.travelRoot.gameObject.SetActive(true);
						}
					}
					this.Element.ShowAfterRefresh();
				});
			}
			else
			{
				this.Element.ShowAfterRefresh();
			}
		}

		// Token: 0x06006800 RID: 26624 RVA: 0x002F8798 File Offset: 0x002F6998
		private bool Set(ImagesAndTexts it, IReadOnlyList<int> arr, int index, LanguageKey lk = LanguageKey.Invalid)
		{
			return this.Set(it, (arr.CheckIndexReadOnly(index) && arr[index] != 0) ? ((lk == LanguageKey.Invalid) ? arr[index].ToString() : lk.TrFormat(arr[index])) : null);
		}

		// Token: 0x06006801 RID: 26625 RVA: 0x002F87EC File Offset: 0x002F69EC
		private bool Set(ImagesAndTexts it, string text)
		{
			bool flag = text != null;
			bool result;
			if (flag)
			{
				it.gameObject.SetActive(true);
				it.texts[0].text = text.ColorReplace();
				result = true;
			}
			else
			{
				it.gameObject.SetActive(false);
				result = false;
			}
			return result;
		}

		// Token: 0x04004977 RID: 18807
		[SerializeField]
		private TMP_Text title;

		// Token: 0x04004978 RID: 18808
		[SerializeField]
		private TMP_Text desc;

		// Token: 0x04004979 RID: 18809
		[SerializeField]
		private ImagesAndTexts debt;

		// Token: 0x0400497A RID: 18810
		[SerializeField]
		private ImagesAndTexts infectedDemon;

		// Token: 0x0400497B RID: 18811
		[SerializeField]
		private ImagesAndTexts infected;

		// Token: 0x0400497C RID: 18812
		[SerializeField]
		private ImagesAndTexts legendaryBook;

		// Token: 0x0400497D RID: 18813
		[SerializeField]
		private ImagesAndTexts oldFriend;

		// Token: 0x0400497E RID: 18814
		[SerializeField]
		private ImagesAndTexts bamboo;

		// Token: 0x0400497F RID: 18815
		[SerializeField]
		private ImagesAndTexts beast;

		// Token: 0x04004980 RID: 18816
		[SerializeField]
		private ImagesAndTexts jiao;

		// Token: 0x04004981 RID: 18817
		[SerializeField]
		private ImagesAndTexts loong;

		// Token: 0x04004982 RID: 18818
		[SerializeField]
		private ImagesAndTexts unlocked;

		// Token: 0x04004983 RID: 18819
		[SerializeField]
		private ImagesAndTexts taiwuVillage;

		// Token: 0x04004984 RID: 18820
		[SerializeField]
		private ImagesAndTexts jieqing;

		// Token: 0x04004985 RID: 18821
		[SerializeField]
		private ImagesAndTexts destroyed;

		// Token: 0x04004986 RID: 18822
		[SerializeField]
		private ImagesAndTexts normalAdv;

		// Token: 0x04004987 RID: 18823
		[SerializeField]
		private ImagesAndTexts mainAdv;

		// Token: 0x04004988 RID: 18824
		[SerializeField]
		private ImagesAndTexts sectAdv;

		// Token: 0x04004989 RID: 18825
		[SerializeField]
		private ImagesAndTexts tombAdv;

		// Token: 0x0400498A RID: 18826
		[SerializeField]
		private ImagesAndTexts majorAdv;

		// Token: 0x0400498B RID: 18827
		[SerializeField]
		private TMP_Text travelRoot;

		// Token: 0x0400498C RID: 18828
		[SerializeField]
		private TMP_Text unlockRoot;

		// Token: 0x0400498D RID: 18829
		[SerializeField]
		private TMP_Text authorityNotEnough;

		// Token: 0x0400498E RID: 18830
		[SerializeField]
		private ImagesAndTexts moneyCost;

		// Token: 0x0400498F RID: 18831
		[SerializeField]
		private ImagesAndTexts timeCost;

		// Token: 0x04004990 RID: 18832
		[SerializeField]
		private ImagesAndTexts authorityCost;

		// Token: 0x04004991 RID: 18833
		[SerializeField]
		private GameObject space1;

		// Token: 0x04004992 RID: 18834
		[SerializeField]
		private GameObject space2;

		// Token: 0x04004993 RID: 18835
		[SerializeField]
		private GameObject space3;

		// Token: 0x04004994 RID: 18836
		[SerializeField]
		private GameObject space4;

		// Token: 0x04004995 RID: 18837
		private short _areaId;

		// Token: 0x04004996 RID: 18838
		private AreaDisplayData _displayData;

		// Token: 0x04004997 RID: 18839
		private MapAreaItem _cfg;

		// Token: 0x04004998 RID: 18840
		private WorldMapModel _model;
	}
}
