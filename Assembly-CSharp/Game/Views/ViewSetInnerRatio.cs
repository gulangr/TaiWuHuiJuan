using System;
using Config;
using FrameWork;
using FrameWork.UISystem.UIElements;
using GameData.Domains.Combat;
using GameData.Domains.Item.Display;
using GameData.Serializer;
using GameData.Utilities;
using TMPro;
using UnityEngine;
using UnityEngine.Events;

namespace Game.Views
{
	// Token: 0x0200070F RID: 1807
	public class ViewSetInnerRatio : UIBase
	{
		// Token: 0x17000A6B RID: 2667
		// (get) Token: 0x060055A1 RID: 21921 RVA: 0x0027ADE1 File Offset: 0x00278FE1
		private sbyte Value
		{
			get
			{
				return (sbyte)(100f - Math.Clamp(this.slider.value, 0f, 100f));
			}
		}

		// Token: 0x060055A2 RID: 21922 RVA: 0x0027AE04 File Offset: 0x00279004
		public override void OnInit(ArgumentBox argsBox)
		{
			RectTransform itemRectTrans;
			argsBox.Get<RectTransform>("ItemRectTrans", out itemRectTrans);
			argsBox.Get<ItemDisplayData>("TargetItem", out this._targetItem);
			Vector3 itemScreenPos = UIManager.Instance.UiCamera.WorldToScreenPoint(itemRectTrans.position);
			this.panel.position = UIManager.Instance.UiCamera.ScreenToWorldPoint(itemScreenPos);
			Vector2 itemSize = itemRectTrans.rect.size;
			Vector2 panelSize = this.panel.rect.size;
			this.panel.anchoredPosition += new Vector2(0f, (-panelSize.y - itemSize.y) * 0.5f);
			Rect itemRect = CommonUtils.RectTransToScreenPos(this.panel, UIManager.Instance.UiCamera);
			Rect scrollRect = CommonUtils.RectTransToScreenPos(base.RectTransform, UIManager.Instance.UiCamera);
			bool isOverlap = !scrollRect.ContainsWithBorder(itemRect.min);
			bool flag = isOverlap;
			if (flag)
			{
				this.panel.anchoredPosition += new Vector2(0f, itemSize.y + this.panel.rect.height);
			}
		}

		// Token: 0x060055A3 RID: 21923 RVA: 0x0027AF44 File Offset: 0x00279144
		private void Awake()
		{
			this.slider.onValueChanged.RemoveAllListeners();
			this.slider.onValueChanged.AddListener(new UnityAction<float>(this.OnValueChange));
			this.btnMinus.ClearAndAddListener(new Action(this.More));
			this.btnAdd.ClearAndAddListener(new Action(this.Less));
			this.btnClose.ClearAndAddListener(new Action(this.QuickHide));
			this.btnMask.ClearAndAddListener(new Action(this.QuickHide));
			this.btnConfirm.ClearAndAddListener(new Action(this.Confirm));
		}

		// Token: 0x060055A4 RID: 21924 RVA: 0x0027AFFA File Offset: 0x002791FA
		private void OnEnable()
		{
			this.RequestData();
		}

		// Token: 0x060055A5 RID: 21925 RVA: 0x0027B004 File Offset: 0x00279204
		private void RequestData()
		{
			CombatDomainMethod.AsyncCall.GetWeaponExpectInnerRatio(null, this._targetItem.RealKey, delegate(int offset, RawDataPool pool)
			{
				IntPair innerRatio = new IntPair(0, 0);
				Serializer.Deserialize(pool, offset, ref innerRatio);
				WeaponItem weaponConfig = Weapon.Instance[this._targetItem.RealKey.TemplateId];
				int baseRatio = (int)weaponConfig.DefaultInnerRatio;
				int changeRange = (int)weaponConfig.InnerRatioAdjustRange * innerRatio.Second / 100;
				this._innerRatioRange.x = Math.Max(baseRatio - changeRange, 0);
				this._innerRatioRange.y = Math.Min(baseRatio + changeRange, 100);
				this.rangeUp.fillAmount = (float)(100 - this._innerRatioRange.y) / 100f;
				this.rangeDown.fillAmount = (float)this._innerRatioRange.x / 100f;
				this.slider.value = (float)(100 - innerRatio.First);
				this.OnValueChange((float)(100 - innerRatio.First));
			});
		}

		// Token: 0x060055A6 RID: 21926 RVA: 0x0027B028 File Offset: 0x00279228
		private void Update()
		{
			bool flag = CommonCommandKit.Space.Check(this.Element, false, false, false, true, false);
			if (flag)
			{
				this.Confirm();
			}
			else
			{
				bool flag2 = CommonCommandKit.Esc.Check(this.Element, false, false, false, true, false);
				if (flag2)
				{
					this.QuickHide();
				}
			}
		}

		// Token: 0x060055A7 RID: 21927 RVA: 0x0027B078 File Offset: 0x00279278
		private void OnValueChange(float rawValue)
		{
			sbyte innerValue = this.Value;
			this.btnMinus.interactable = (rawValue > 0f);
			this.btnAdd.interactable = (rawValue < 100f);
			bool flag = this._innerRatioRange.x < 100 && (int)this.Value < this._innerRatioRange.x;
			if (flag)
			{
				this.expect.GetComponent<RectTransform>().anchoredPosition = new Vector2(this.slider.GetComponent<RectTransform>().rect.width * (1f - this.rangeDown.fillAmount), 0f);
				this.expect.SetActive(true);
				innerValue = (sbyte)this._innerRatioRange.x;
			}
			else
			{
				bool flag2 = this._innerRatioRange.y > 0 && (int)this.Value > this._innerRatioRange.y;
				if (flag2)
				{
					this.expect.GetComponent<RectTransform>().anchoredPosition = new Vector2(this.slider.GetComponent<RectTransform>().rect.width * this.rangeUp.fillAmount, 0f);
					this.expect.SetActive(true);
					innerValue = (sbyte)this._innerRatioRange.y;
				}
				else
				{
					this.expect.SetActive(false);
				}
			}
			this.textInner.SetText(LanguageKey.LK_Skill_InnerRatio.TrFormat(innerValue), true);
			this.textOuter.SetText(LanguageKey.LK_Skill_OuterRatio.TrFormat((int)(100 - innerValue)), true);
			this.textOuterSlider.text = ((int)(100 - this.Value)).ToString();
			this.textInnerSlider.text = this.Value.ToString();
		}

		// Token: 0x060055A8 RID: 21928 RVA: 0x0027B24E File Offset: 0x0027944E
		private void Confirm()
		{
			CombatDomainMethod.Call.ChangeTaiwuWeaponInnerRatioByWeaponKey(this._targetItem.RealKey, this.Value);
			GEvent.OnEvent(UiEvents.OnRefreshCharacterMenuItem, null);
			this.QuickHide();
		}

		// Token: 0x060055A9 RID: 21929 RVA: 0x0027B280 File Offset: 0x00279480
		private void More()
		{
			this.slider.value -= 1f;
		}

		// Token: 0x060055AA RID: 21930 RVA: 0x0027B29B File Offset: 0x0027949B
		private void Less()
		{
			this.slider.value += 1f;
		}

		// Token: 0x04003A6D RID: 14957
		[SerializeField]
		private RectTransform panel;

		// Token: 0x04003A6E RID: 14958
		[SerializeField]
		private CSlider slider;

		// Token: 0x04003A6F RID: 14959
		[SerializeField]
		private CButton btnMinus;

		// Token: 0x04003A70 RID: 14960
		[SerializeField]
		private CButton btnAdd;

		// Token: 0x04003A71 RID: 14961
		[SerializeField]
		private CButton btnClose;

		// Token: 0x04003A72 RID: 14962
		[SerializeField]
		private CButton btnConfirm;

		// Token: 0x04003A73 RID: 14963
		[SerializeField]
		private CButton btnMask;

		// Token: 0x04003A74 RID: 14964
		[SerializeField]
		private TextMeshProUGUI textOuter;

		// Token: 0x04003A75 RID: 14965
		[SerializeField]
		private TextMeshProUGUI textInner;

		// Token: 0x04003A76 RID: 14966
		[SerializeField]
		private TextMeshProUGUI textOuterSlider;

		// Token: 0x04003A77 RID: 14967
		[SerializeField]
		private TextMeshProUGUI textInnerSlider;

		// Token: 0x04003A78 RID: 14968
		[SerializeField]
		private CImage rangeUp;

		// Token: 0x04003A79 RID: 14969
		[SerializeField]
		private CImage rangeDown;

		// Token: 0x04003A7A RID: 14970
		[SerializeField]
		private GameObject expect;

		// Token: 0x04003A7B RID: 14971
		private ItemDisplayData _targetItem;

		// Token: 0x04003A7C RID: 14972
		private Vector2Int _innerRatioRange;
	}
}
