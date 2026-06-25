using System;
using Config;
using UnityEngine;

namespace Game.Views.Debate
{
	// Token: 0x02000A9C RID: 2716
	public class DebateCardItem : MonoBehaviour
	{
		// Token: 0x17000E95 RID: 3733
		// (get) Token: 0x060084E7 RID: 34023 RVA: 0x003DC514 File Offset: 0x003DA714
		public DebateCardView CardView
		{
			get
			{
				return this.cardView;
			}
		}

		// Token: 0x17000E96 RID: 3734
		// (get) Token: 0x060084E8 RID: 34024 RVA: 0x003DC51C File Offset: 0x003DA71C
		public DebateStrategyItem CardConfig
		{
			get
			{
				return this.CardView.CardConfig;
			}
		}

		// Token: 0x17000E97 RID: 3735
		// (get) Token: 0x060084E9 RID: 34025 RVA: 0x003DC529 File Offset: 0x003DA729
		public RectTransform RectTrans
		{
			get
			{
				return base.transform as RectTransform;
			}
		}

		// Token: 0x17000E98 RID: 3736
		// (get) Token: 0x060084EA RID: 34026 RVA: 0x003DC536 File Offset: 0x003DA736
		public bool CanSelect
		{
			get
			{
				return this.CardView.Button.interactable;
			}
		}

		// Token: 0x17000E99 RID: 3737
		// (get) Token: 0x060084EB RID: 34027 RVA: 0x003DC548 File Offset: 0x003DA748
		// (set) Token: 0x060084EC RID: 34028 RVA: 0x003DC550 File Offset: 0x003DA750
		public bool Selected { get; private set; }

		// Token: 0x17000E9A RID: 3738
		// (get) Token: 0x060084ED RID: 34029 RVA: 0x003DC559 File Offset: 0x003DA759
		// (set) Token: 0x060084EE RID: 34030 RVA: 0x003DC561 File Offset: 0x003DA761
		public bool Visible { get; private set; }

		// Token: 0x060084EF RID: 34031 RVA: 0x003DC56C File Offset: 0x003DA76C
		public void SetVisible(bool visible, bool includeView = true)
		{
			this.Visible = visible;
			base.gameObject.SetActive(visible);
			bool flag = !this.Visible;
			if (flag)
			{
				bool selected = this.Selected;
				if (selected)
				{
					this.SetSelect(false, true);
				}
			}
			if (includeView)
			{
				this.CardView.gameObject.SetActive(visible);
			}
		}

		// Token: 0x060084F0 RID: 34032 RVA: 0x003DC5C8 File Offset: 0x003DA7C8
		public void SetSelect(bool select, bool hasScale = true)
		{
			this.CardView.SetSelected(select, hasScale);
			this.Selected = select;
		}

		// Token: 0x040065E0 RID: 26080
		[SerializeField]
		private DebateCardView cardView;
	}
}
