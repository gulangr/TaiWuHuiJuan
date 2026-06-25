using System;
using GameData.Domains.Item.Display;
using TMPro;
using UnityEngine;

namespace Game.Views.Cricket.Combat
{
	// Token: 0x02000ADE RID: 2782
	public class CricketName : MonoBehaviour
	{
		// Token: 0x17000F1C RID: 3868
		// (get) Token: 0x060088EC RID: 35052 RVA: 0x003F6754 File Offset: 0x003F4954
		public CanvasGroup Canvas
		{
			get
			{
				return this.textMeshForName.GetComponent<CanvasGroup>();
			}
		}

		// Token: 0x060088ED RID: 35053 RVA: 0x003F6764 File Offset: 0x003F4964
		public void SetNoCricket(bool noCricket)
		{
			if (noCricket)
			{
				this._nameText = LocalStringManager.Get(LanguageKey.LK_UI_CricketCombat_NoCricket);
				this.textMeshForName.text = this._nameText;
			}
		}

		// Token: 0x060088EE RID: 35054 RVA: 0x003F679B File Offset: 0x003F499B
		public void Set(ItemDisplayData data, int order)
		{
			this.SetOrder(order);
			this._nameText = CricketCombatKit.GetCricketName(data);
			this.textMeshForName.text = this._nameText;
		}

		// Token: 0x060088EF RID: 35055 RVA: 0x003F67C4 File Offset: 0x003F49C4
		public void SetVisible(bool visible)
		{
			this.textMeshForName.text = (visible ? this._nameText : LocalStringManager.Get(LanguageKey.LK_CricketCombat_UnknownName));
		}

		// Token: 0x060088F0 RID: 35056 RVA: 0x003F67E8 File Offset: 0x003F49E8
		public void Hide()
		{
			this.textMeshForName.text = string.Empty;
		}

		// Token: 0x060088F1 RID: 35057 RVA: 0x003F67FC File Offset: 0x003F49FC
		public void SetOrder(int order)
		{
			order = Mathf.Clamp(order, 0, 2);
			this.orderIcon.SetSprite(string.Format("{0}{1}", "ui9_icon_cricketcombat_1_", order), false, null);
			bool flag = this.orderLabel == null;
			if (flag)
			{
				this.orderLabel = this.FindOrderLabel();
			}
			bool flag2 = this.orderLabel != null;
			if (flag2)
			{
				this.orderLabel.text = LocalStringManager.Get(string.Format("LK_CricketBattle_Round{0}", order));
			}
		}

		// Token: 0x060088F2 RID: 35058 RVA: 0x003F6884 File Offset: 0x003F4A84
		private TextMeshProUGUI FindOrderLabel()
		{
			TextMeshProUGUI[] labels = base.GetComponentsInChildren<TextMeshProUGUI>(true);
			foreach (TextMeshProUGUI label in labels)
			{
				bool flag = label == this.textMeshForName;
				if (!flag)
				{
					bool flag2 = label.gameObject.name == "OrderLabel";
					if (flag2)
					{
						return label;
					}
				}
			}
			return null;
		}

		// Token: 0x040068D4 RID: 26836
		[SerializeField]
		private TextMeshProUGUI textMeshForName;

		// Token: 0x040068D5 RID: 26837
		[SerializeField]
		private CImage orderIcon;

		// Token: 0x040068D6 RID: 26838
		[SerializeField]
		private TextMeshProUGUI orderLabel;

		// Token: 0x040068D7 RID: 26839
		private string _nameText;
	}
}
