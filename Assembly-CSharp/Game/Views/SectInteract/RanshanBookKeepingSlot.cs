using System;
using System.Collections.Generic;
using Config;
using FrameWork;
using FrameWork.UISystem.UIElements;
using TMPro;
using UnityEngine;

namespace Game.Views.SectInteract
{
	// Token: 0x020009B4 RID: 2484
	public class RanshanBookKeepingSlot : MonoBehaviour
	{
		// Token: 0x17000D66 RID: 3430
		// (get) Token: 0x06007853 RID: 30803 RVA: 0x0037FC0E File Offset: 0x0037DE0E
		// (set) Token: 0x06007854 RID: 30804 RVA: 0x0037FC16 File Offset: 0x0037DE16
		public int CorpseId { get; private set; }

		// Token: 0x17000D67 RID: 3431
		// (get) Token: 0x06007855 RID: 30805 RVA: 0x0037FC1F File Offset: 0x0037DE1F
		// (set) Token: 0x06007856 RID: 30806 RVA: 0x0037FC27 File Offset: 0x0037DE27
		public int CorpseIndex { get; private set; }

		// Token: 0x17000D68 RID: 3432
		// (get) Token: 0x06007857 RID: 30807 RVA: 0x0037FC30 File Offset: 0x0037DE30
		// (set) Token: 0x06007858 RID: 30808 RVA: 0x0037FC38 File Offset: 0x0037DE38
		public int SlotIndex { get; private set; }

		// Token: 0x17000D69 RID: 3433
		// (get) Token: 0x06007859 RID: 30809 RVA: 0x0037FC41 File Offset: 0x0037DE41
		// (set) Token: 0x0600785A RID: 30810 RVA: 0x0037FC49 File Offset: 0x0037DE49
		public short CurrentTemplateId { get; private set; } = -1;

		// Token: 0x0600785B RID: 30811 RVA: 0x0037FC54 File Offset: 0x0037DE54
		public void Init(int corpseIndex, int slotIndex, List<GeneralLineData> bookKeepTipsData, Action<int> onSelect)
		{
			this.CorpseIndex = corpseIndex;
			this.SlotIndex = slotIndex;
			this._onSelect = onSelect;
			base.GetComponent<CButton>().ClearAndAddListener(new Action(this.OnSelectedBook));
			TooltipInvoker tips = base.GetComponent<TooltipInvoker>();
			TooltipInvoker tooltipInvoker = tips;
			if (tooltipInvoker.RuntimeParam == null)
			{
				tooltipInvoker.RuntimeParam = new ArgumentBox();
			}
			tips.RuntimeParam.Set("Title", LocalStringManager.Get(LanguageKey.LK_LegendaryBook_Keeping));
			for (int i = 0; i < bookKeepTipsData.Count; i++)
			{
				tips.RuntimeParam.SetObject(string.Format("LineData{0}", i + 1), bookKeepTipsData[i]);
			}
			tips.RuntimeParam.Set("LineCount", bookKeepTipsData.Count);
			tips.Refresh(false, -1);
		}

		// Token: 0x0600785C RID: 30812 RVA: 0x0037FD28 File Offset: 0x0037DF28
		public void Set(int corpseId)
		{
			this.CorpseId = corpseId;
		}

		// Token: 0x0600785D RID: 30813 RVA: 0x0037FD34 File Offset: 0x0037DF34
		public void Refresh(short templateId, bool locked, bool isInvalid)
		{
			this.CurrentTemplateId = templateId;
			bool flag = templateId >= 0;
			if (flag)
			{
				this.SetItem(templateId);
			}
			else if (locked)
			{
				this.SetLock();
			}
			else if (isInvalid)
			{
				this.SetInvalid();
			}
			else
			{
				this.SetEmpty();
			}
		}

		// Token: 0x0600785E RID: 30814 RVA: 0x0037FD82 File Offset: 0x0037DF82
		public void OnSelectedBook()
		{
			Action<int> onSelect = this._onSelect;
			if (onSelect != null)
			{
				onSelect(this.SlotIndex);
			}
		}

		// Token: 0x0600785F RID: 30815 RVA: 0x0037FDA0 File Offset: 0x0037DFA0
		private void SetEmpty()
		{
			this.itemIcon.SetActive(false);
			this.locker.SetActive(false);
			this.label.SetText(LanguageKey.LK_Ranshan_KeepingBook_Add.Tr(), true);
			base.GetComponent<CImage>().enabled = true;
			base.GetComponent<CButton>().interactable = true;
		}

		// Token: 0x06007860 RID: 30816 RVA: 0x0037FDFC File Offset: 0x0037DFFC
		private void SetItem(short templateId)
		{
			MiscItem config = Misc.Instance[templateId];
			this.itemIcon.SetActive(true);
			this.locker.SetActive(false);
			this.label.SetText(config.Name.SetGradeColor((int)config.Grade), true);
			this.itemIcon.GetComponent<CImage>().SetSprite(config.Icon, false, null);
			base.GetComponent<CImage>().enabled = true;
			base.GetComponent<CButton>().interactable = true;
		}

		// Token: 0x06007861 RID: 30817 RVA: 0x0037FE84 File Offset: 0x0037E084
		private void SetLock()
		{
			this.itemIcon.SetActive(false);
			this.locker.SetActive(true);
			this.label.SetText(LanguageKey.LK_Ranshan_KeepingBook_Locked.Tr().ColorReplace(), true);
			base.GetComponent<CImage>().enabled = false;
			base.GetComponent<CButton>().interactable = false;
		}

		// Token: 0x06007862 RID: 30818 RVA: 0x0037FEE4 File Offset: 0x0037E0E4
		private void SetInvalid()
		{
			this.itemIcon.SetActive(false);
			this.locker.SetActive(false);
			this.label.SetText(LanguageKey.LK_Ranshan_KeepingBook_Invalid.Tr().ColorReplace(), true);
			base.GetComponent<CImage>().enabled = true;
			base.GetComponent<CButton>().interactable = false;
		}

		// Token: 0x04005AF6 RID: 23286
		public TextMeshProUGUI label;

		// Token: 0x04005AF7 RID: 23287
		public GameObject itemIcon;

		// Token: 0x04005AF8 RID: 23288
		public GameObject locker;

		// Token: 0x04005AFD RID: 23293
		private Action<int> _onSelect;
	}
}
