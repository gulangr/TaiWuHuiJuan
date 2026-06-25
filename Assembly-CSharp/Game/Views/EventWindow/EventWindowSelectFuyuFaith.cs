using System;
using Game.Components.ListStyleGeneralScroll.Item;
using GameData.Domains.Item.Display;
using GameData.Domains.TaiwuEvent.DisplayEvent;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Game.Views.EventWindow
{
	// Token: 0x02000A43 RID: 2627
	public class EventWindowSelectFuyuFaith : MonoBehaviour
	{
		// Token: 0x17000E36 RID: 3638
		// (get) Token: 0x060081B3 RID: 33203 RVA: 0x003C6CF4 File Offset: 0x003C4EF4
		public int Value
		{
			get
			{
				return this.bar.CurCount;
			}
		}

		// Token: 0x17000E37 RID: 3639
		// (get) Token: 0x060081B4 RID: 33204 RVA: 0x003C6D01 File Offset: 0x003C4F01
		// (set) Token: 0x060081B5 RID: 33205 RVA: 0x003C6D0C File Offset: 0x003C4F0C
		public int From
		{
			get
			{
				return this._from;
			}
			set
			{
				TMP_Text tmp_Text = this.from;
				this._from = value;
				int num = value;
				tmp_Text.text = num.ToString();
			}
		}

		// Token: 0x17000E38 RID: 3640
		// (get) Token: 0x060081B6 RID: 33206 RVA: 0x003C6D37 File Offset: 0x003C4F37
		// (set) Token: 0x060081B7 RID: 33207 RVA: 0x003C6D40 File Offset: 0x003C4F40
		public int To
		{
			get
			{
				return this._to;
			}
			set
			{
				TMP_Text tmp_Text = this.to;
				this._to = value;
				int num = value;
				tmp_Text.text = num.ToString();
			}
		}

		// Token: 0x17000E39 RID: 3641
		// (get) Token: 0x060081B8 RID: 33208 RVA: 0x003C6D6B File Offset: 0x003C4F6B
		// (set) Token: 0x060081B9 RID: 33209 RVA: 0x003C6D74 File Offset: 0x003C4F74
		public int Curr
		{
			get
			{
				return this._curr;
			}
			set
			{
				TMP_Text tmp_Text = this.curr;
				this._curr = value;
				int num = value;
				tmp_Text.text = num.ToString();
			}
		}

		// Token: 0x060081BA RID: 33210 RVA: 0x003C6DA0 File Offset: 0x003C4FA0
		public void RefreshBar(bool resetValue = true)
		{
			int selectable = Math.Min(this._max, this._curr);
			this.bar.Rerfresh(selectable, 0, 1, true, false, 1, delegate(int newValue)
			{
				this.UpdateValue(newValue);
			});
			if (resetValue)
			{
				this.bar.SetCurrentCount(1);
			}
		}

		// Token: 0x060081BB RID: 33211 RVA: 0x003C6DF0 File Offset: 0x003C4FF0
		public void RefershFuyuSword()
		{
			this.fuyuSword.SetData(new ItemDisplayData(12, 239));
		}

		// Token: 0x060081BC RID: 33212 RVA: 0x003C6E0A File Offset: 0x003C500A
		private void OnEnable()
		{
			this.RefreshBar(true);
		}

		// Token: 0x060081BD RID: 33213 RVA: 0x003C6E14 File Offset: 0x003C5014
		public void Refresh(bool resetValue = true)
		{
			this.RefershFuyuSword();
			this.RefreshBar(resetValue);
			LayoutRebuilder.ForceRebuildLayoutImmediate(base.transform as RectTransform);
		}

		// Token: 0x060081BE RID: 33214 RVA: 0x003C6E38 File Offset: 0x003C5038
		public void UpdateValue(int value)
		{
			this.curr.text = (this._curr - value).ToString();
			this.To = this._from + value;
			LayoutRebuilder.ForceRebuildLayoutImmediate(this.curr.transform.parent as RectTransform);
		}

		// Token: 0x060081BF RID: 33215 RVA: 0x003C6E8C File Offset: 0x003C508C
		public void SetData(EventSelectFuyuFaithCountData data)
		{
			bool flag = data == null;
			if (flag)
			{
				base.gameObject.SetActive(false);
			}
			else
			{
				this.Curr = data.Curr;
				this.From = (this.To = data.Counter.Total);
				int[] fuyuFaithLevel = GlobalConfig.FuyuFaithLevel;
				this._max = fuyuFaithLevel[fuyuFaithLevel.Length - 1] - data.Counter.Tips3;
				this.Refresh(true);
			}
		}

		// Token: 0x0400632A RID: 25386
		[SerializeField]
		private EventWindowSetSelectAmount bar;

		// Token: 0x0400632B RID: 25387
		[SerializeField]
		private RowItemMain fuyuSword;

		// Token: 0x0400632C RID: 25388
		[SerializeField]
		private TMP_Text from;

		// Token: 0x0400632D RID: 25389
		[SerializeField]
		private TMP_Text to;

		// Token: 0x0400632E RID: 25390
		[SerializeField]
		private TMP_Text curr;

		// Token: 0x0400632F RID: 25391
		private int _max;

		// Token: 0x04006330 RID: 25392
		private int _from;

		// Token: 0x04006331 RID: 25393
		private int _to;

		// Token: 0x04006332 RID: 25394
		private int _curr;
	}
}
