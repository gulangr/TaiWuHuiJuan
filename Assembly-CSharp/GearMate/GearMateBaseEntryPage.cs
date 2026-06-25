using System;
using System.Collections.Generic;
using System.Linq;
using GameData.GameDataBridge;
using UnityEngine;

namespace GearMate
{
	// Token: 0x02000612 RID: 1554
	public class GearMateBaseEntryPage : GearMateSubPageBase
	{
		// Token: 0x1700092B RID: 2347
		// (get) Token: 0x060048DD RID: 18653 RVA: 0x002215E3 File Offset: 0x0021F7E3
		public override bool IsLeaf
		{
			get
			{
				return false;
			}
		}

		// Token: 0x060048DE RID: 18654 RVA: 0x002215E6 File Offset: 0x0021F7E6
		protected override void InitInternal()
		{
			base.InitInternal();
			this.InitRefers();
			this.InitToggles();
			this.InitSubPages();
		}

		// Token: 0x060048DF RID: 18655 RVA: 0x00221605 File Offset: 0x0021F805
		private void InitToggles()
		{
			this.SubTogGroup.InitPreOnToggle(-1);
			this.SubTogGroup.OnActiveToggleChange = new Action<CToggleObsolete, CToggleObsolete>(this.OnActiveToggleChange);
		}

		// Token: 0x060048E0 RID: 18656 RVA: 0x0022162C File Offset: 0x0021F82C
		private void OnActiveToggleChange(CToggleObsolete newToggle, CToggleObsolete oldToggle)
		{
			int index = newToggle.Key;
			for (int i = 0; i < this._currSelectedList.Count; i++)
			{
				this._currSelectedList[i].SetActive(i == index);
			}
			this.SwitchToPage(index);
		}

		// Token: 0x060048E1 RID: 18657 RVA: 0x0022167C File Offset: 0x0021F87C
		private void OnEnable()
		{
			bool hasActive = this._subPages.Any((GearMateSubPageBase t) => t.gameObject.activeSelf);
			bool flag = !hasActive;
			if (flag)
			{
				this.SubTogGroup.Set(0, true, true);
			}
		}

		// Token: 0x060048E2 RID: 18658 RVA: 0x002216CC File Offset: 0x0021F8CC
		protected void SwitchToPage(int index)
		{
			this.CurrIndex = index;
			for (int i = 0; i < this._subPages.Count; i++)
			{
				this._subPages[i].gameObject.SetActive(i == index);
				bool flag = i == index;
				if (flag)
				{
					base.Parent.SetCurrentLeafSubPage(this._subPages[i]);
				}
			}
		}

		// Token: 0x060048E3 RID: 18659 RVA: 0x0022173C File Offset: 0x0021F93C
		private void InitSubPages()
		{
			for (int i = 0; i < this._subPageRoot.childCount; i++)
			{
				GearMateSubPageBase subPage = this._subPageRoot.GetChild(i).GetComponent<GearMateSubPageBase>();
				bool flag = subPage != null;
				if (flag)
				{
					subPage.Init(base.Parent);
					this._subPages.Add(subPage);
				}
			}
		}

		// Token: 0x060048E4 RID: 18660 RVA: 0x002217A0 File Offset: 0x0021F9A0
		public override void HandleMethodReturn(Notification notification, NotificationWrapper wrapper)
		{
			base.HandleMethodReturn(notification, wrapper);
			foreach (GearMateSubPageBase subPage in this._subPages)
			{
				subPage.HandleMethodReturn(notification, wrapper);
			}
		}

		// Token: 0x060048E5 RID: 18661 RVA: 0x00221804 File Offset: 0x0021FA04
		private void InitRefers()
		{
			this._subToggleList = base.CGetList<CToggleObsolete>("SubToggle_");
			this._currSelectedList = base.CGetList<GameObject>("CurrSelected_");
			this._subPageRoot = base.CGet<RectTransform>("SubPageRoot");
			this.SubTogGroup = base.CGet<CToggleGroupObsolete>("SubTogGroup");
		}

		// Token: 0x040032A5 RID: 12965
		protected List<GearMateSubPageBase> _subPages = new List<GearMateSubPageBase>();

		// Token: 0x040032A6 RID: 12966
		public int CurrIndex = 0;

		// Token: 0x040032A7 RID: 12967
		private List<CToggleObsolete> _subToggleList;

		// Token: 0x040032A8 RID: 12968
		private List<GameObject> _currSelectedList;

		// Token: 0x040032A9 RID: 12969
		private RectTransform _subPageRoot;

		// Token: 0x040032AA RID: 12970
		protected CToggleGroupObsolete SubTogGroup;
	}
}
