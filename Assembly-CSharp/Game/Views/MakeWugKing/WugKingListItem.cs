using System;
using Config;
using GameData.Domains.Item;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Game.Views.MakeWugKing
{
	// Token: 0x0200094C RID: 2380
	public class WugKingListItem : MonoBehaviour
	{
		// Token: 0x06007080 RID: 28800 RVA: 0x003419B8 File Offset: 0x0033FBB8
		public void OnItemRender(int index)
		{
			this.topPlaceHolder.minHeight = 0f;
			this.bottomPlaceHolder.minHeight = 0f;
			WugKingItem wugKingConfig = WugKing.Instance[index];
			MedicineItem medicineConfig = Medicine.Instance[wugKingConfig.WugMedicine];
			this.wugName.text = medicineConfig.Name;
			this.wugIcon.SetSprite(medicineConfig.Icon, false, null);
			this.wugDesc.text = medicineConfig.Desc;
			Refers template = this.poisonTypeTemplate;
			RectTransform poisonLayout = this.poisonTypeLayout;
			CommonUtils.PrepareEnoughChildren(poisonLayout, template.gameObject, wugKingConfig.RefiningPoisons.Count, null);
			for (int i = 0; i < wugKingConfig.RefiningPoisons.Count; i++)
			{
				Refers poisonRefers = poisonLayout.GetChild(i).GetComponent<Refers>();
				poisonRefers.CGet<CImage>("Icon").SetSprite(string.Format("{0}{1}", "ui9_back_poison_big_7_", wugKingConfig.RefiningPoisons[i]), false, null);
				poisonRefers.CGet<TextMeshProUGUI>("Name").text = LocalStringManager.Get(string.Format("LK_Poison_Name_{0}", wugKingConfig.RefiningPoisons[i]));
			}
			ItemKey itemKey = new ItemKey(8, 0, medicineConfig.TemplateId, -1);
			this.wugKingEffect.Refresh(itemKey, -1, false);
			RectTransform rectTransform = this.wugKingEffect.transform as RectTransform;
			LayoutRebuilder.ForceRebuildLayoutImmediate(rectTransform);
		}

		// Token: 0x06007081 RID: 28801 RVA: 0x00341B44 File Offset: 0x0033FD44
		public float GetTopHeight()
		{
			return this.topRect.sizeDelta.y;
		}

		// Token: 0x06007082 RID: 28802 RVA: 0x00341B68 File Offset: 0x0033FD68
		public float GetBottomHeight()
		{
			return this.bottomRect.sizeDelta.y;
		}

		// Token: 0x06007083 RID: 28803 RVA: 0x00341B8A File Offset: 0x0033FD8A
		public void SetPlaceHolder(float maxTopHeight, float maxBottomHeight)
		{
			this.topPlaceHolder.minHeight = maxTopHeight - this.GetTopHeight();
			this.bottomPlaceHolder.minHeight = maxBottomHeight - this.GetBottomHeight();
		}

		// Token: 0x04005365 RID: 21349
		[SerializeField]
		private TextMeshProUGUI wugName;

		// Token: 0x04005366 RID: 21350
		[SerializeField]
		private CImage wugIcon;

		// Token: 0x04005367 RID: 21351
		[SerializeField]
		private TextMeshProUGUI wugDesc;

		// Token: 0x04005368 RID: 21352
		[SerializeField]
		private Refers poisonTypeTemplate;

		// Token: 0x04005369 RID: 21353
		[SerializeField]
		private RectTransform poisonTypeLayout;

		// Token: 0x0400536A RID: 21354
		[SerializeField]
		private WugKingEffectListItem wugKingEffect;

		// Token: 0x0400536B RID: 21355
		[SerializeField]
		private LayoutElement topPlaceHolder;

		// Token: 0x0400536C RID: 21356
		[SerializeField]
		private LayoutElement bottomPlaceHolder;

		// Token: 0x0400536D RID: 21357
		[SerializeField]
		private RectTransform topRect;

		// Token: 0x0400536E RID: 21358
		[SerializeField]
		private RectTransform bottomRect;
	}
}
