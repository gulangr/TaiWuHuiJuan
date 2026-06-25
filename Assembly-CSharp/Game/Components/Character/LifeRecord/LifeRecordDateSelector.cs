using System;
using FrameWork;
using FrameWork.UISystem.UIElements;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Game.Components.Character.LifeRecord
{
	// Token: 0x02000F4C RID: 3916
	public class LifeRecordDateSelector : MonoBehaviour
	{
		// Token: 0x1700145C RID: 5212
		// (get) Token: 0x0600B3A1 RID: 45985 RVA: 0x0051C3DB File Offset: 0x0051A5DB
		public int CurrDate
		{
			get
			{
				return this.currDate;
			}
		}

		// Token: 0x1700145D RID: 5213
		// (get) Token: 0x0600B3A2 RID: 45986 RVA: 0x0051C3E3 File Offset: 0x0051A5E3
		public float CnBtnWidth
		{
			get
			{
				return (this.cnWidth - this.margin) * 0.25f;
			}
		}

		// Token: 0x1700145E RID: 5214
		// (get) Token: 0x0600B3A3 RID: 45987 RVA: 0x0051C3F8 File Offset: 0x0051A5F8
		public float EnBtnWidth
		{
			get
			{
				return (this.enWidth - this.margin) * 0.25f;
			}
		}

		// Token: 0x0600B3A4 RID: 45988 RVA: 0x0051C410 File Offset: 0x0051A610
		private void Awake()
		{
			this.minus1.ClearAndAddListener(delegate
			{
				this.SetDate(this.year - 1, this.currDate % 12, true, ESelectDateDirection.SelectBefore);
			});
			this.minus10.ClearAndAddListener(delegate
			{
				this.SetDate(this.year - 10, this.currDate % 12, true, ESelectDateDirection.SelectBefore);
			});
			this.plus1.ClearAndAddListener(delegate
			{
				this.SetDate(this.year + 1, this.currDate % 12, true, ESelectDateDirection.SelectAfter);
			});
			this.plus10.ClearAndAddListener(delegate
			{
				this.SetDate(this.year + 10, this.currDate % 12, true, ESelectDateDirection.SelectAfter);
			});
			this.slider.onValueChanged.AddListener(delegate(float percent)
			{
				this.SelectDeltaDate((int)(percent * (float)(this.endDate + 1 - this.startDate)));
			});
			for (int i = 0; i < 12; i++)
			{
				int stored = i;
				this.months[stored].ClearAndAddListener(delegate
				{
					this.SetMonth(stored);
				});
			}
			GEvent.Add(UiEvents.OnLanguageChange, new GEvent.Callback(this.OnLanguageChange));
			this.OnLanguageChange(null);
		}

		// Token: 0x0600B3A5 RID: 45989 RVA: 0x0051C505 File Offset: 0x0051A705
		private void OnDestroy()
		{
			GEvent.Remove(UiEvents.OnLanguageChange, new GEvent.Callback(this.OnLanguageChange));
		}

		// Token: 0x0600B3A6 RID: 45990 RVA: 0x0051C524 File Offset: 0x0051A724
		public void OnDisable()
		{
			this.openSelector.isOn = false;
		}

		// Token: 0x0600B3A7 RID: 45991 RVA: 0x0051C534 File Offset: 0x0051A734
		private void OnLanguageChange(ArgumentBox _)
		{
			bool isCn = LocalStringManager.CurLanguageType == LocalStringManager.LanguageType.CN;
			this.rect.sizeDelta = this.rect.sizeDelta.SetX(isCn ? this.cnWidth : this.enWidth);
			this.grid.cellSize = this.grid.cellSize.SetX(((isCn ? this.cnWidth : this.enWidth) - this.margin) * 0.25f);
		}

		// Token: 0x0600B3A8 RID: 45992 RVA: 0x0051C5B4 File Offset: 0x0051A7B4
		public void SetDate(int newYear, int newMonth, bool scroll, ESelectDateDirection direction = ESelectDateDirection.SelectDefault)
		{
			this.year = newYear;
			this.currDate = newYear * 12 + newMonth;
			bool flag = this.currDate < this.startDate || this.currDate > this.endDate;
			if (flag)
			{
				this.currDate = Math.Clamp(this.currDate, this.startDate, this.endDate);
				this.year = this.currDate / 12;
				newMonth = this.currDate - this.year * 12;
			}
			this.currDisplay.text = LanguageKey.LK_CharacterMenuLifeRecord_Month_Summary.TrFormat(new object[]
			{
				this.year + 1,
				newMonth + 1,
				this.currDate - this.startDate + 1,
				this.endDate - this.startDate + 1
			});
			this.minus1.interactable = (this.minus10.interactable = (this.currDate > this.startDate));
			this.plus1.interactable = (this.plus10.interactable = (this.currDate < this.endDate));
			for (int i = this.year * 12; i < this.year * 12 + 12; i++)
			{
				Selectable selectable = this.months[i - this.year * 12];
				bool interactable;
				if (i >= this.startDate && i <= this.endDate)
				{
					Func<int, bool> dateHasData = this.DateHasData;
					bool? flag2 = (dateHasData != null) ? new bool?(dateHasData(i)) : null;
					interactable = (flag2 == null || flag2.GetValueOrDefault());
				}
				else
				{
					interactable = false;
				}
				selectable.interactable = interactable;
			}
			if (scroll)
			{
				this._scroll = true;
				this._parent.ScrollToMonth(this.currDate, false, false, ESelectDateDirection.SelectDefault);
				this.UpdateScroll(this.currDate);
			}
		}

		// Token: 0x0600B3A9 RID: 45993 RVA: 0x0051C7B4 File Offset: 0x0051A9B4
		public void Set(int curr)
		{
			bool scroll = this._scroll;
			if (scroll)
			{
				this._scroll = false;
			}
			else
			{
				this.Set(curr, false, ESelectDateDirection.SelectDefault);
				this.UpdateScroll(curr);
			}
		}

		// Token: 0x0600B3AA RID: 45994 RVA: 0x0051C7E8 File Offset: 0x0051A9E8
		private void UpdateScroll(int curr)
		{
			float val = Math.Clamp((float)((double)(curr - this.startDate) + 0.5) / (float)(this.endDate - this.startDate + 1), 0f, 1f);
			this.slider.SetValueWithoutNotify(val);
		}

		// Token: 0x0600B3AB RID: 45995 RVA: 0x0051C838 File Offset: 0x0051AA38
		private void Set(int curr, bool scroll, ESelectDateDirection direction = ESelectDateDirection.SelectDefault)
		{
			this.SetDate(curr / 12, curr % 12, scroll, direction);
		}

		// Token: 0x0600B3AC RID: 45996 RVA: 0x0051C84B File Offset: 0x0051AA4B
		public void Set(int start, int end, ILifeRecord parent)
		{
			this.startDate = start;
			this.endDate = end;
			this._parent = parent;
			this.Set(parent.CurrDate, false, ESelectDateDirection.SelectDefault);
			base.gameObject.SetActive(true);
		}

		// Token: 0x0600B3AD RID: 45997 RVA: 0x0051C87F File Offset: 0x0051AA7F
		public void SetMonth(int month)
		{
			this._scroll = true;
			this.SetDate(this.year, month, true, ESelectDateDirection.SelectDefault);
			this.UpdateScroll(this.currDate);
			this.openSelector.isOn = false;
		}

		// Token: 0x0600B3AE RID: 45998 RVA: 0x0051C8B4 File Offset: 0x0051AAB4
		public void SelectDeltaDate(int delta)
		{
			int curr = this.startDate + delta;
			if (!true)
			{
			}
			ESelectDateDirection direction;
			if (delta >= 0)
			{
				if (delta <= 0)
				{
					direction = ESelectDateDirection.SelectDefault;
				}
				else
				{
					direction = ESelectDateDirection.SelectAfter;
				}
			}
			else
			{
				direction = ESelectDateDirection.SelectBefore;
			}
			if (!true)
			{
			}
			this.Set(curr, true, direction);
		}

		// Token: 0x0600B3AF RID: 45999 RVA: 0x0051C8F2 File Offset: 0x0051AAF2
		public void OnChangeingSelectStatus(bool isSelected)
		{
			this.monthPanel.SetActive(isSelected);
			this.buttonTransform.localScale = this.buttonTransform.localScale.SetY(isSelected ? -1f : 1f);
		}

		// Token: 0x04008B98 RID: 35736
		public Func<int, bool> DateHasData = null;

		// Token: 0x04008B99 RID: 35737
		[SerializeField]
		private int currDate;

		// Token: 0x04008B9A RID: 35738
		[SerializeField]
		private int startDate;

		// Token: 0x04008B9B RID: 35739
		[SerializeField]
		private int endDate;

		// Token: 0x04008B9C RID: 35740
		[SerializeField]
		private int year;

		// Token: 0x04008B9D RID: 35741
		[SerializeField]
		private TMP_Text currDisplay;

		// Token: 0x04008B9E RID: 35742
		[SerializeField]
		private CButton minus1;

		// Token: 0x04008B9F RID: 35743
		[SerializeField]
		private CButton minus10;

		// Token: 0x04008BA0 RID: 35744
		[SerializeField]
		private CButton plus1;

		// Token: 0x04008BA1 RID: 35745
		[SerializeField]
		private CButton plus10;

		// Token: 0x04008BA2 RID: 35746
		[SerializeField]
		private CButton[] months;

		// Token: 0x04008BA3 RID: 35747
		public CToggle openSelector;

		// Token: 0x04008BA4 RID: 35748
		[SerializeField]
		private CSlider slider;

		// Token: 0x04008BA5 RID: 35749
		[SerializeField]
		private RectTransform rect;

		// Token: 0x04008BA6 RID: 35750
		[SerializeField]
		private GridLayoutGroup grid;

		// Token: 0x04008BA7 RID: 35751
		[SerializeField]
		private float cnWidth = 396f;

		// Token: 0x04008BA8 RID: 35752
		[SerializeField]
		private float enWidth = 500f;

		// Token: 0x04008BA9 RID: 35753
		[SerializeField]
		private float margin = 36f;

		// Token: 0x04008BAA RID: 35754
		private ILifeRecord _parent;

		// Token: 0x04008BAB RID: 35755
		private bool _scroll = false;

		// Token: 0x04008BAC RID: 35756
		[SerializeField]
		private GameObject monthPanel;

		// Token: 0x04008BAD RID: 35757
		[SerializeField]
		private Transform buttonTransform;
	}
}
