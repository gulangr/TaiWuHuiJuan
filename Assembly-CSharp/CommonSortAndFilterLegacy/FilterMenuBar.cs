using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace CommonSortAndFilterLegacy
{
	// Token: 0x0200043F RID: 1087
	public class FilterMenuBar : Refers
	{
		// Token: 0x1700068E RID: 1678
		// (get) Token: 0x06004005 RID: 16389 RVA: 0x001FCFE0 File Offset: 0x001FB1E0
		private List<TextMeshProUGUI> _labelList
		{
			get
			{
				return base.CGetList<TextMeshProUGUI>("Label");
			}
		}

		// Token: 0x1700068F RID: 1679
		// (get) Token: 0x06004006 RID: 16390 RVA: 0x001FCFED File Offset: 0x001FB1ED
		private List<TextMeshProUGUI> _amountList
		{
			get
			{
				return base.CGetList<TextMeshProUGUI>("Amount");
			}
		}

		// Token: 0x17000690 RID: 1680
		// (get) Token: 0x06004007 RID: 16391 RVA: 0x001FCFFA File Offset: 0x001FB1FA
		private CToggleObsolete _swapToggle
		{
			get
			{
				return base.CGet<CToggleObsolete>("SwapToggle");
			}
		}

		// Token: 0x17000691 RID: 1681
		// (get) Token: 0x06004008 RID: 16392 RVA: 0x001FD007 File Offset: 0x001FB207
		private HSVStyleRoot _hsvStyleRoot
		{
			get
			{
				return base.CGet<HSVStyleRoot>("HSVStyleRoot");
			}
		}

		// Token: 0x17000692 RID: 1682
		// (get) Token: 0x06004009 RID: 16393 RVA: 0x001FD014 File Offset: 0x001FB214
		private RectTransform _statusIcon
		{
			get
			{
				return base.CGet<RectTransform>("StatusIcon");
			}
		}

		// Token: 0x17000693 RID: 1683
		// (get) Token: 0x0600400A RID: 16394 RVA: 0x001FD021 File Offset: 0x001FB221
		private GameObject _selectedRoot
		{
			get
			{
				return base.CGet<GameObject>("SelectedRoot");
			}
		}

		// Token: 0x0600400B RID: 16395 RVA: 0x001FD030 File Offset: 0x001FB230
		public void SetStatusIcon(bool isMenuShow)
		{
			Quaternion targetRotation = (!isMenuShow) ? this._qtArrowDownwards : this._qtArrowUpwards;
			this._statusIcon.transform.rotation = targetRotation;
		}

		// Token: 0x0600400C RID: 16396 RVA: 0x001FD064 File Offset: 0x001FB264
		private void RefreshStyle()
		{
			bool enable = this._enable;
			if (enable)
			{
				this._hsvStyleRoot.SetDefault();
			}
			else
			{
				this._hsvStyleRoot.SetDefaultGrayAndBlack();
			}
			this._selectedRoot.SetActive(this._selected);
		}

		// Token: 0x0600400D RID: 16397 RVA: 0x001FD0A8 File Offset: 0x001FB2A8
		public void SetLabelText(string labelText)
		{
			foreach (TextMeshProUGUI item in this._labelList)
			{
				item.text = labelText;
			}
		}

		// Token: 0x0600400E RID: 16398 RVA: 0x001FD104 File Offset: 0x001FB304
		public void SetSelected(bool selected)
		{
			this._selected = selected;
			this.RefreshStyle();
		}

		// Token: 0x0600400F RID: 16399 RVA: 0x001FD115 File Offset: 0x001FB315
		public void SetEnableState(bool isEnable)
		{
			this._enable = isEnable;
			this.RefreshStyle();
		}

		// Token: 0x06004010 RID: 16400 RVA: 0x001FD128 File Offset: 0x001FB328
		public void SetupSwapToggle(Action<bool> onSwapToggleClicked)
		{
			this._swapToggle.onValueChanged.RemoveAllListeners();
			this._swapToggle.isOn = false;
			this._swapToggle.onValueChanged.AddListener(delegate(bool isOn)
			{
				Action<bool> onSwapToggleClicked2 = onSwapToggleClicked;
				if (onSwapToggleClicked2 != null)
				{
					onSwapToggleClicked2(isOn);
				}
			});
		}

		// Token: 0x06004011 RID: 16401 RVA: 0x001FD180 File Offset: 0x001FB380
		public void SetSwapToggle(bool isOn, bool force = false)
		{
			bool flag = this._swapToggle.isOn != isOn;
			if (flag)
			{
				this._swapToggle.isOn = isOn;
			}
			else if (force)
			{
				Toggle.ToggleEvent onValueChanged = this._swapToggle.onValueChanged;
				if (onValueChanged != null)
				{
					onValueChanged.Invoke(isOn);
				}
			}
		}

		// Token: 0x04002DC4 RID: 11716
		private Quaternion _qtArrowUpwards = Quaternion.Euler(0f, 0f, -179.9f);

		// Token: 0x04002DC5 RID: 11717
		private Quaternion _qtArrowDownwards = Quaternion.Euler(0f, 0f, 0f);

		// Token: 0x04002DC6 RID: 11718
		private bool _selected = false;

		// Token: 0x04002DC7 RID: 11719
		private bool _enable = true;
	}
}
