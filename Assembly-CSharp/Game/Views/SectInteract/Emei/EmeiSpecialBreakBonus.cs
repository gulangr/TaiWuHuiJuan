using System;
using System.Collections.Generic;
using Config;
using FrameWork;
using FrameWork.UISystem.UIElements;
using TMPro;
using UnityEngine;

namespace Game.Views.SectInteract.Emei
{
	// Token: 0x020009F0 RID: 2544
	public class EmeiSpecialBreakBonus : MonoBehaviour
	{
		// Token: 0x06007D1A RID: 32026 RVA: 0x003A216C File Offset: 0x003A036C
		public void Init(Action<short> onSelect)
		{
			PointerTrigger pointerTrigger = base.GetComponent<PointerTrigger>();
			pointerTrigger.EnterEvent.RemoveAllListeners();
			pointerTrigger.EnterEvent.AddListener(delegate()
			{
				this.SetHover(true);
			});
			pointerTrigger.ExitEvent.RemoveAllListeners();
			pointerTrigger.ExitEvent.AddListener(delegate()
			{
				this.SetHover(false);
			});
			base.GetComponent<CButton>().ClearAndAddListener(delegate
			{
				onSelect(this._templateId);
			});
		}

		// Token: 0x06007D1B RID: 32027 RVA: 0x003A21F8 File Offset: 0x003A03F8
		public void Set(short templateId, int progress, int bonusCount, bool setBack = true)
		{
			this._templateId = templateId;
			SkillBreakPlateGridBonusTypeItem config = SkillBreakPlateGridBonusType.Instance[templateId];
			int backPrefix = (bonusCount > 0) ? 0 : 1;
			List<SpecialBreakProperty> properties = CommonUtils.GetBreakEntryPropertiesByTemplateId(templateId);
			if (setBack)
			{
				this.Bg.SetSprite("ui9_back_special_break_" + backPrefix.ToString() + "_" + config.FilterGroup.ToString(), false, null);
				this.Bg.gameObject.SetActive(true);
			}
			else
			{
				bool flag = this.Bg;
				if (flag)
				{
					this.Bg.gameObject.SetActive(false);
				}
			}
			this.title.text = config.Name.SetColor(ViewEmeiCombatSkillSpecialBreak.GetColor(config.FilterGroup));
			this.count.text = bonusCount.ToString().SetColor((bonusCount > 0) ? "brightblue" : "grey");
			this.progressBar.fillAmount = (float)progress / (float)GlobalConfig.Instance.SectStoryEmeiBonusProgressPerCount;
			this.line1.SetActive(properties.Count > 0);
			this.line2.SetActive(properties.Count > 1);
			for (int i = 0; i < this.propertyParent.childCount; i++)
			{
				bool flag2 = i >= properties.Count;
				if (flag2)
				{
					this.propertyParent.GetChild(i).gameObject.SetActive(false);
				}
				else
				{
					Transform obj = this.propertyParent.GetChild(i);
					SpecialBreakProperty property = properties[i];
					obj.GetChild(0).GetComponent<TextMeshProUGUI>().text = property.name;
					obj.GetChild(1).GetComponent<TextMeshProUGUI>().text = property.value;
					obj.gameObject.SetActive(true);
				}
			}
			TooltipInvoker tips = base.GetComponent<TooltipInvoker>();
			TooltipInvoker tooltipInvoker = tips;
			if (tooltipInvoker.RuntimeParam == null)
			{
				tooltipInvoker.RuntimeParam = EasyPool.Get<ArgumentBox>();
			}
			tips.RuntimeParam.Set("arg0", config.Name);
			tips.RuntimeParam.Set("arg1", config.Desc);
		}

		// Token: 0x06007D1C RID: 32028 RVA: 0x003A242C File Offset: 0x003A062C
		public void SetSelected(short templateId)
		{
			this.SetSelected(templateId == this._templateId);
		}

		// Token: 0x06007D1D RID: 32029 RVA: 0x003A243F File Offset: 0x003A063F
		public void SetSelected(bool value)
		{
			this.selected.SetActive(value);
		}

		// Token: 0x06007D1E RID: 32030 RVA: 0x003A2450 File Offset: 0x003A0650
		public void SetCanInteract(bool value)
		{
			base.GetComponent<PointerTrigger>().enabled = value;
			base.GetComponent<CButton>().interactable = value;
			base.GetComponent<DisableStyleRoot>().SetStyleEffect(!value, false);
			base.GetComponent<CanvasGroup>().alpha = (value ? 1f : 0.5f);
		}

		// Token: 0x06007D1F RID: 32031 RVA: 0x003A24A4 File Offset: 0x003A06A4
		public void SetHover(bool value)
		{
			this.hover.SetActive(value);
		}

		// Token: 0x04005F2A RID: 24362
		public GameObject selected;

		// Token: 0x04005F2B RID: 24363
		public GameObject hover;

		// Token: 0x04005F2C RID: 24364
		public TextMeshProUGUI title;

		// Token: 0x04005F2D RID: 24365
		public TextMeshProUGUI count;

		// Token: 0x04005F2E RID: 24366
		public GameObject line1;

		// Token: 0x04005F2F RID: 24367
		public GameObject line2;

		// Token: 0x04005F30 RID: 24368
		public CImage progressBar;

		// Token: 0x04005F31 RID: 24369
		public Transform propertyParent;

		// Token: 0x04005F32 RID: 24370
		public CImage Bg;

		// Token: 0x04005F33 RID: 24371
		private short _templateId;
	}
}
