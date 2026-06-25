using System;
using FrameWork.UISystem.UIElements;
using UnityEngine;

namespace Game.Views.Exchange
{
	// Token: 0x02000A2C RID: 2604
	public class ShopRefreshButton : MonoBehaviour
	{
		// Token: 0x06007F80 RID: 32640 RVA: 0x003B60D0 File Offset: 0x003B42D0
		private void Awake()
		{
			bool flag = (this._parent = (this.parent as IShopRefresh)) == null;
			if (flag)
			{
				Debug.LogError("Parent is null or invalid");
			}
		}

		// Token: 0x06007F81 RID: 32641 RVA: 0x003B6104 File Offset: 0x003B4304
		private void OnEnable()
		{
			this._parent.InitShopRefresh(new Action(this.RefreshCount), new Action<bool>(this.RefreshActiveStates), new Action<bool>(this.RefreshTips));
			this.mouseTipDisplayer.PresetParam[0] = this._parent.RefreshTipTitle;
			this.RefreshTips(false);
		}

		// Token: 0x06007F82 RID: 32642 RVA: 0x003B6162 File Offset: 0x003B4362
		public void RefreshActiveStates(bool active)
		{
			base.gameObject.SetActive(active);
		}

		// Token: 0x06007F83 RID: 32643 RVA: 0x003B6174 File Offset: 0x003B4374
		public void OnClick(string str)
		{
			bool canRefreshCurrentGoods = this._parent.CanRefreshCurrentGoods;
			if (canRefreshCurrentGoods)
			{
				this._parent.RefreshCurrentGoods();
			}
			else
			{
				this._parent.NoticeCannotRefreshCurrentGoods();
			}
		}

		// Token: 0x06007F84 RID: 32644 RVA: 0x003B61AC File Offset: 0x003B43AC
		public void RefreshTips(bool needClear)
		{
			bool flag = this._parent == null;
			if (!flag)
			{
				this.mouseTipDisplayer.Type = TipType.Simple;
				this.mouseTipDisplayer.PresetParam[1] = this._parent.RefreshTips(needClear);
				this.mouseTipDisplayer.Refresh(false, -1);
				base.gameObject.SetActive(this._parent.CanShow);
			}
		}

		// Token: 0x06007F85 RID: 32645 RVA: 0x003B6213 File Offset: 0x003B4413
		public void RefreshCount()
		{
		}

		// Token: 0x040061DC RID: 25052
		[SerializeField]
		private MonoBehaviour parent;

		// Token: 0x040061DD RID: 25053
		[SerializeField]
		private TooltipInvoker mouseTipDisplayer;

		// Token: 0x040061DE RID: 25054
		[SerializeField]
		private GameObject enable;

		// Token: 0x040061DF RID: 25055
		[SerializeField]
		private GameObject disable;

		// Token: 0x040061E0 RID: 25056
		[SerializeField]
		private CButton button;

		// Token: 0x040061E1 RID: 25057
		private IShopRefresh _parent;
	}
}
