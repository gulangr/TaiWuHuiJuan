using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace AiEditor
{
	// Token: 0x0200067D RID: 1661
	public class AiNodeRelate : Refers
	{
		// Token: 0x17000992 RID: 2450
		// (get) Token: 0x06004E79 RID: 20089 RVA: 0x0024DBC0 File Offset: 0x0024BDC0
		private TextMeshProUGUI RelateIdText
		{
			get
			{
				return base.CGet<TextMeshProUGUI>("Id");
			}
		}

		// Token: 0x17000993 RID: 2451
		// (get) Token: 0x06004E7A RID: 20090 RVA: 0x0024DBCD File Offset: 0x0024BDCD
		private GameObject Selecting
		{
			get
			{
				return base.CGet<GameObject>("Selecting");
			}
		}

		// Token: 0x17000994 RID: 2452
		// (get) Token: 0x06004E7B RID: 20091 RVA: 0x0024DBDA File Offset: 0x0024BDDA
		// (set) Token: 0x06004E7C RID: 20092 RVA: 0x0024DBE2 File Offset: 0x0024BDE2
		public int RelateId { get; private set; }

		// Token: 0x06004E7D RID: 20093 RVA: 0x0024DBEB File Offset: 0x0024BDEB
		public void Bind(IAiNodeRelateHandler handler)
		{
			this._handler = handler;
		}

		// Token: 0x06004E7E RID: 20094 RVA: 0x0024DBF4 File Offset: 0x0024BDF4
		public void Invoke()
		{
			this._relating = true;
			this._handler.InvokeRelate();
			this.Select();
		}

		// Token: 0x06004E7F RID: 20095 RVA: 0x0024DC14 File Offset: 0x0024BE14
		public void Relate(int relateId)
		{
			bool relating = this._relating;
			if (relating)
			{
				this.Set(relateId);
			}
			this._relating = false;
			this.Unselect();
		}

		// Token: 0x06004E80 RID: 20096 RVA: 0x0024DC42 File Offset: 0x0024BE42
		public void Interrupt()
		{
			this._relating = false;
			this.Unselect();
		}

		// Token: 0x06004E81 RID: 20097 RVA: 0x0024DC53 File Offset: 0x0024BE53
		public void Set(int relateId = -1)
		{
			this.RelateId = relateId;
			this.RelateIdText.text = ((relateId < 0) ? string.Empty : relateId.ToString());
			this.Unselect();
		}

		// Token: 0x06004E82 RID: 20098 RVA: 0x0024DC84 File Offset: 0x0024BE84
		public void Sync(Dictionary<int, int> old2NewIds)
		{
			int newId;
			bool flag = old2NewIds.TryGetValue(this.RelateId, out newId);
			if (flag)
			{
				this.Set(newId);
			}
		}

		// Token: 0x06004E83 RID: 20099 RVA: 0x0024DCAC File Offset: 0x0024BEAC
		private void Select()
		{
			this.Selecting.SetActive(true);
		}

		// Token: 0x06004E84 RID: 20100 RVA: 0x0024DCBB File Offset: 0x0024BEBB
		private void Unselect()
		{
			this.Selecting.SetActive(false);
		}

		// Token: 0x04003641 RID: 13889
		private bool _relating;

		// Token: 0x04003642 RID: 13890
		private IAiNodeRelateHandler _handler;
	}
}
