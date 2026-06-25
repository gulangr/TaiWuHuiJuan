using System;
using FrameWork;
using TMPro;
using UICommon.Character.Elements;
using UnityEngine;

namespace CommonSortAndFilterLegacy
{
	// Token: 0x02000425 RID: 1061
	public class CommonFilterToggle : MonoBehaviour
	{
		// Token: 0x17000665 RID: 1637
		// (get) Token: 0x06003EEE RID: 16110 RVA: 0x001F7958 File Offset: 0x001F5B58
		private TooltipInvoker Tip
		{
			get
			{
				return this.toggle.GetComponent<TooltipInvoker>();
			}
		}

		// Token: 0x17000666 RID: 1638
		// (get) Token: 0x06003EEF RID: 16111 RVA: 0x001F7965 File Offset: 0x001F5B65
		public bool IsSelected
		{
			get
			{
				return this.toggle.isOn;
			}
		}

		// Token: 0x06003EF0 RID: 16112 RVA: 0x001F7972 File Offset: 0x001F5B72
		private void Awake()
		{
			this.toggle.OnInteractableChangeReverse.AddListener(delegate(bool interactableReverse)
			{
				bool flag = this.disable;
				if (flag)
				{
					this.disable.gameObject.SetActive(interactableReverse);
				}
			});
		}

		// Token: 0x06003EF1 RID: 16113 RVA: 0x001F7992 File Offset: 0x001F5B92
		public void Refresh(FilterToggleConfig config)
		{
			this.Refresh(config.IconNames, config.TipContent);
		}

		// Token: 0x06003EF2 RID: 16114 RVA: 0x001F79A8 File Offset: 0x001F5BA8
		public void Refresh(ToggleTransitionIconSpriteNames iconNames, StringKey tipContent)
		{
			this.iconTransitionHelper.SetSpriteNames(iconNames);
			this.Tip.Type = TipType.SingleDesc;
			TooltipInvoker tip = this.Tip;
			if (tip.RuntimeParam == null)
			{
				tip.RuntimeParam = new ArgumentBox();
			}
			string tipContentString = tipContent.GetString();
			this.Tip.RuntimeParam.Set("arg0", tipContentString);
		}

		// Token: 0x04002D33 RID: 11571
		public CToggleObsolete toggle;

		// Token: 0x04002D34 RID: 11572
		public CImage icon;

		// Token: 0x04002D35 RID: 11573
		public TextMeshProUGUI nameLabel;

		// Token: 0x04002D36 RID: 11574
		public CImage disable;

		// Token: 0x04002D37 RID: 11575
		[SerializeField]
		private ToggleIconTransitionHelper iconTransitionHelper;
	}
}
