using System;
using System.Collections.Generic;
using FrameWork.UISystem.UIElements;
using Game.Components.Item;
using TMPro;
using UnityEngine;

namespace Game.Views.Main.Reading
{
	// Token: 0x0200096F RID: 2415
	public class ReadingReferenceBookInfoPrefab : MonoBehaviour
	{
		// Token: 0x17000D22 RID: 3362
		// (get) Token: 0x06007389 RID: 29577 RVA: 0x0035A809 File Offset: 0x00358A09
		// (set) Token: 0x0600738A RID: 29578 RVA: 0x0035A811 File Offset: 0x00358A11
		public bool inSelectedBtnPointerTrigger { get; private set; }

		// Token: 0x17000D23 RID: 3363
		// (get) Token: 0x0600738B RID: 29579 RVA: 0x0035A81A File Offset: 0x00358A1A
		// (set) Token: 0x0600738C RID: 29580 RVA: 0x0035A822 File Offset: 0x00358A22
		public bool inRemoveBtnPointerTrigger { get; private set; }

		// Token: 0x0600738D RID: 29581 RVA: 0x0035A82B File Offset: 0x00358A2B
		public void ChangeStateInSelectedBtn(bool newState)
		{
			this.inSelectedBtnPointerTrigger = newState;
		}

		// Token: 0x0600738E RID: 29582 RVA: 0x0035A836 File Offset: 0x00358A36
		public void ChangeStateInRemoveBtn(bool newState)
		{
			this.inRemoveBtnPointerTrigger = newState;
		}

		// Token: 0x040055EE RID: 21998
		public ItemBack item;

		// Token: 0x040055EF RID: 21999
		public TextMeshProUGUI txtName;

		// Token: 0x040055F0 RID: 22000
		public GameObject emptyBack;

		// Token: 0x040055F1 RID: 22001
		public CButton emptyBtn;

		// Token: 0x040055F2 RID: 22002
		public GameObject selectedBack;

		// Token: 0x040055F3 RID: 22003
		public CButton selectedBtn;

		// Token: 0x040055F4 RID: 22004
		public PointerTrigger selectedBtnPointerTrigger;

		// Token: 0x040055F5 RID: 22005
		public TooltipInvoker selectedBtnTooltipInvoker;

		// Token: 0x040055F6 RID: 22006
		public GameObject lockBack;

		// Token: 0x040055F7 RID: 22007
		public CButton removeBtn;

		// Token: 0x040055F8 RID: 22008
		public PointerTrigger removeBtnPointerTrigger;

		// Token: 0x040055F9 RID: 22009
		public TextMeshProUGUI durability;

		// Token: 0x040055FA RID: 22010
		public GameObject strategy;

		// Token: 0x040055FB RID: 22011
		public TextMeshProUGUI efficiency;

		// Token: 0x040055FC RID: 22012
		public TextMeshProUGUI rate;

		// Token: 0x040055FD RID: 22013
		public List<ReadingBookPages> pages;

		// Token: 0x040055FE RID: 22014
		public GameObject specialEffect;

		// Token: 0x040055FF RID: 22015
		public GameObject durabilityTip;

		// Token: 0x04005600 RID: 22016
		public TextMeshProUGUI lockTip;

		// Token: 0x04005601 RID: 22017
		public GameObject efficiencyHolder;

		// Token: 0x04005602 RID: 22018
		public GameObject selectRoot;

		// Token: 0x04005603 RID: 22019
		public GameObject emptySelectedGo;

		// Token: 0x04005604 RID: 22020
		public GameObject selectedBackSelectedGo;

		// Token: 0x04005605 RID: 22021
		public GameObject hoverGo;
	}
}
