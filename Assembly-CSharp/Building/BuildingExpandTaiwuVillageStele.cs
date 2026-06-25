using System;
using DG.Tweening;
using FrameWork;
using UnityEngine;

namespace Building
{
	// Token: 0x0200065F RID: 1631
	public class BuildingExpandTaiwuVillageStele : MonoBehaviour
	{
		// Token: 0x06004DA4 RID: 19876 RVA: 0x00249B1C File Offset: 0x00247D1C
		private void Awake()
		{
			CButtonObsolete button = base.GetComponent<CButtonObsolete>();
			button.ClearAndAddListener(new Action(this.OnClick));
			base.transform.Find("Hover").SetParent(this.hoverRoot, true);
		}

		// Token: 0x06004DA5 RID: 19877 RVA: 0x00249B61 File Offset: 0x00247D61
		public void Bind(IBuildingExpandTaiwuVillageSteleHandler handler)
		{
			this._handler = handler;
		}

		// Token: 0x06004DA6 RID: 19878 RVA: 0x00249B6C File Offset: 0x00247D6C
		public void Set(bool upgraded, sbyte selectedOrgTemplateId)
		{
			string spriteName = this.GetSpriteName(upgraded);
			base.GetComponent<CImage>().SetSprite(spriteName, false, null);
			base.GetComponent<CButtonObsolete>().interactable = !upgraded;
			this.SetMouseTip(upgraded);
			bool selectedActive = this.orgTemplateId == selectedOrgTemplateId;
			bool flag = this.selected.activeSelf != selectedActive;
			if (flag)
			{
				this.selected.SetActive(selectedActive);
			}
			this.animationTransition.DOKill(false);
			bool activeSelf = this.animationTransition.gameObject.activeSelf;
			if (activeSelf)
			{
				this.animationTransition.gameObject.SetActive(false);
			}
		}

		// Token: 0x06004DA7 RID: 19879 RVA: 0x00249C08 File Offset: 0x00247E08
		public void AnimationToUpgraded(float duration)
		{
			base.GetComponent<CImage>().SetSprite(this.GetSpriteName(true), false, null);
			this.animationTransition.DOKill(false);
			this.animationTransition.SetSprite(this.GetSpriteName(false), false, null);
			this.animationTransition.gameObject.SetActive(true);
			this.animationTransition.SetAlpha(1f);
			this.animationTransition.DOFade(0f, duration);
		}

		// Token: 0x06004DA8 RID: 19880 RVA: 0x00249C84 File Offset: 0x00247E84
		private void SetMouseTip(bool upgraded)
		{
			TooltipInvoker mouseTip = base.GetComponent<TooltipInvoker>();
			TooltipInvoker tooltipInvoker = mouseTip;
			ArgumentBox argumentBox;
			if ((argumentBox = tooltipInvoker.RuntimeParam) == null)
			{
				argumentBox = (tooltipInvoker.RuntimeParam = new ArgumentBox());
			}
			ArgumentBox argBox = argumentBox;
			argBox.Set("OrgTemplateId", this.orgTemplateId);
			argBox.Set("Unlocked", upgraded);
			bool showing = mouseTip.Showing;
			if (showing)
			{
				mouseTip.ShowTips();
			}
		}

		// Token: 0x06004DA9 RID: 19881 RVA: 0x00249CE8 File Offset: 0x00247EE8
		private void OnClick()
		{
			bool activeSelf = this.selected.activeSelf;
			if (activeSelf)
			{
				IBuildingExpandTaiwuVillageSteleHandler handler = this._handler;
				if (handler != null)
				{
					handler.Cancel();
				}
			}
			else
			{
				IBuildingExpandTaiwuVillageSteleHandler handler2 = this._handler;
				if (handler2 != null)
				{
					handler2.Handle(this.orgTemplateId);
				}
			}
		}

		// Token: 0x06004DAA RID: 19882 RVA: 0x00249D34 File Offset: 0x00247F34
		private string GetSpriteName(bool upgraded)
		{
			return string.Format("building_vow_stele_{0}_{1}", this.orgTemplateId, upgraded ? 1 : 0);
		}

		// Token: 0x040035D6 RID: 13782
		[SerializeField]
		public sbyte orgTemplateId;

		// Token: 0x040035D7 RID: 13783
		[SerializeField]
		public GameObject selected;

		// Token: 0x040035D8 RID: 13784
		[SerializeField]
		public RectTransform hoverRoot;

		// Token: 0x040035D9 RID: 13785
		[SerializeField]
		public CImage animationTransition;

		// Token: 0x040035DA RID: 13786
		private IBuildingExpandTaiwuVillageSteleHandler _handler;
	}
}
