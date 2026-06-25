using System;
using FrameWork;
using FrameWork.UISystem.UIElements;
using Game.Components.Common;
using Game.Views.Item;
using GameData.Domains.Character;
using GameData.Domains.Character.Display;
using GameData.Domains.Item.Display;
using GameData.Serializer;
using GameData.Utilities;
using TMPro;
using UnityEngine;
using UnityEngine.Events;

namespace Game.Views
{
	// Token: 0x02000710 RID: 1808
	public class ViewSetSelectCount : UIBase
	{
		// Token: 0x17000A6C RID: 2668
		// (get) Token: 0x060055AD RID: 21933 RVA: 0x0027B3A5 File Offset: 0x002795A5
		// (set) Token: 0x060055AE RID: 21934 RVA: 0x0027B3AD File Offset: 0x002795AD
		private int CurCount
		{
			get
			{
				return this._curCount;
			}
			set
			{
				this._curCount = Math.Clamp(value, this._minCount, this.MaxSelectCount);
				this.OnCountChange();
			}
		}

		// Token: 0x17000A6D RID: 2669
		// (get) Token: 0x060055AF RID: 21935 RVA: 0x0027B3CF File Offset: 0x002795CF
		private int MaxSelectCount
		{
			get
			{
				return (this._limitCount <= 0) ? this._maxCount : Math.Min(this._limitCount, this._maxCount);
			}
		}

		// Token: 0x17000A6E RID: 2670
		// (get) Token: 0x060055B0 RID: 21936 RVA: 0x0027B3F3 File Offset: 0x002795F3
		// (set) Token: 0x060055B1 RID: 21937 RVA: 0x0027B3FB File Offset: 0x002795FB
		public string ConfirmSound { get; set; }

		// Token: 0x17000A6F RID: 2671
		// (get) Token: 0x060055B2 RID: 21938 RVA: 0x0027B404 File Offset: 0x00279604
		private CanvasGroup CanvasGroup
		{
			get
			{
				return base.gameObject.GetOrAddComponent<CanvasGroup>();
			}
		}

		// Token: 0x060055B3 RID: 21939 RVA: 0x0027B414 File Offset: 0x00279614
		public override void OnInit(ArgumentBox argsBox)
		{
			this._confirmClicked = false;
			this._onValueChanged = null;
			this._onConfirmSetCount = null;
			this._onCancelSetCount = null;
			this.ConfirmSound = "ui_default_second_select";
			bool flag = !argsBox.Get("MaxCount", out this._maxCount);
			if (flag)
			{
				throw new Exception("max count must specified to init UI_SetSelectCount");
			}
			bool flag2 = !argsBox.Get<Vector2>("FollowOffset", out this._followOffset);
			if (flag2)
			{
				this._followOffset = Vector2.zero;
			}
			argsBox.Get<RectTransform>("ItemRectTrans", out this._itemRectTransform);
			argsBox.Get("ZeroValid", out this._zeroValid);
			bool flag3 = !argsBox.Get("MinCount", out this._minCount);
			if (flag3)
			{
				this._minCount = 0;
			}
			bool flag4 = !argsBox.Get("InitCount", out this._initCount);
			if (flag4)
			{
				this._initCount = this._minCount;
			}
			argsBox.Get<Action<int>>("OnValueChanged", out this._onValueChanged);
			argsBox.Get<Action<int>>("OnConfirmSetCount", out this._onConfirmSetCount);
			argsBox.Get<Action>("OnCancelSetCount", out this._onCancelSetCount);
			argsBox.Get("Loop", out this._loop);
			argsBox.Get("LimitCount", out this._limitCount);
			argsBox.Get("LimitTip", out this._limitTip);
			argsBox.Get("NoCancelOnHide", out this._noCancelOnHide);
			int changeValue;
			bool flag5 = argsBox.Get("ChangeValue", out changeValue);
			if (flag5)
			{
				this._changeValue = changeValue;
			}
			bool flag6 = !argsBox.Get<ItemDisplayData>("TargetItem", out this._targetItem);
			if (flag6)
			{
				this._targetItem = null;
			}
			this.slider.wholeNumbers = true;
			this.slider.maxValue = (float)this._maxCount;
			this.slider.minValue = (float)this._minCount;
			this.inputField.characterLimit = this._maxCount.ToString().Length + 1;
			this.CurCount = Math.Min(this.MaxSelectCount, this._initCount);
			this.propertyChangeAlertnessBack.SetActive(false);
			this.propertyChangeFavorBack.SetActive(false);
			this.propertyChangeHappinessBack.SetActive(false);
			this.HideTip();
			this.CanvasGroup.alpha = 0f;
			this.panel.GetOrAddComponent<CanvasGroup>().alpha = 0f;
			this._refreshCount = 0;
			UIElement element = this.Element;
			element.OnShowed = (Action)Delegate.Combine(element.OnShowed, new Action(delegate()
			{
				bool flag7 = this._targetItem == null || SingletonObject.getInstance<CharacterMonitorModel>().IsTaiwuGearMate(this._targetItem.OwnerCharId);
				if (flag7)
				{
					this.CanvasGroup.alpha = 1f;
					this._refreshCount++;
					this.RefreshPanelPos();
				}
				else
				{
					this.RefreshTransferItemPreview();
				}
			}));
		}

		// Token: 0x060055B4 RID: 21940 RVA: 0x0027B69E File Offset: 0x0027989E
		private void RefreshPanelPos()
		{
			SingletonObject.getInstance<YieldHelper>().DelayFrameDo(1U, delegate
			{
				RectTransform rectTransform = this.panel.transform as RectTransform;
				rectTransform.SetPivot(new Vector2(0.5f, 1f));
				Vector3 itemScreenPos = UIManager.Instance.UiCamera.WorldToScreenPoint(this._itemRectTransform.position);
				rectTransform.position = UIManager.Instance.UiCamera.ScreenToWorldPoint(itemScreenPos);
				Vector2 itemSize = this._itemRectTransform.rect.size;
				bool flag = this._itemRectTransform.pivot == Vector2.one * 0.5f;
				if (flag)
				{
					rectTransform.anchoredPosition += new Vector2(0f, -itemSize.y * 0.5f) + this._followOffset;
				}
				else
				{
					bool flag2 = this._itemRectTransform.pivot == Vector2.up;
					if (flag2)
					{
						rectTransform.anchoredPosition += new Vector2(itemSize.x * 0.5f, -itemSize.y) + this._followOffset;
					}
				}
				Vector3 panelScreenPos = UIManager.Instance.UiCamera.WorldToScreenPoint(rectTransform.position);
				Rect itemRect = CommonUtils.RectTransToScreenPos(rectTransform, UIManager.Instance.UiCamera);
				Rect scrollRect = CommonUtils.RectTransToScreenPos(base.RectTransform, UIManager.Instance.UiCamera);
				bool isExceedLeft = (int)itemRect.xMin < (int)scrollRect.xMin;
				bool isExceedRight = (int)itemRect.xMax > (int)scrollRect.xMax;
				bool isExceedBottom = (int)itemRect.yMin < (int)scrollRect.yMin;
				bool flag3 = isExceedLeft;
				if (flag3)
				{
					panelScreenPos.x += scrollRect.xMin - itemRect.xMin;
				}
				else
				{
					bool flag4 = isExceedRight;
					if (flag4)
					{
						panelScreenPos.x += scrollRect.xMax - itemRect.xMax;
					}
				}
				rectTransform.position = UIManager.Instance.UiCamera.ScreenToWorldPoint(panelScreenPos);
				bool flag5 = isExceedBottom;
				if (flag5)
				{
					rectTransform.anchoredPosition += new Vector2(0f, itemSize.y + rectTransform.rect.height) - this._followOffset;
					bool activeSelf = this.tipRectTransform.gameObject.activeSelf;
					if (activeSelf)
					{
						rectTransform.anchoredPosition += new Vector2(0f, this.tipRectTransform.rect.height);
					}
					rectTransform.SetPivot(new Vector2(0.5f, 0f));
				}
				this._refreshCount--;
				bool flag6 = this._refreshCount <= 0;
				if (flag6)
				{
					this.panel.GetOrAddComponent<CanvasGroup>().alpha = 1f;
				}
			});
		}

		// Token: 0x060055B5 RID: 21941 RVA: 0x0027B6BC File Offset: 0x002798BC
		private void RefreshTransferItemPreview()
		{
			bool flag = this._targetItem == null;
			if (flag)
			{
				this.CanvasGroup.alpha = 1f;
			}
			else
			{
				bool isTaiwuGearMate = SingletonObject.getInstance<CharacterMonitorModel>().IsTaiwuGearMate(this._targetItem.OwnerCharId);
				bool flag2 = isTaiwuGearMate;
				if (flag2)
				{
					this.CanvasGroup.alpha = 1f;
				}
				else
				{
					this._refreshCount++;
					Inventory inventory = new Inventory();
					inventory.OfflineAdd(this._targetItem.RealKey, this.CurCount);
					CharacterDomainMethod.AsyncCall.GetTransferItemPreviewDisplayData(this, this._targetItem.OwnerCharId, inventory, false, delegate(int offset, RawDataPool pool)
					{
						TransferItemPreviewDisplayData transferItemPreviewDisplayData = new TransferItemPreviewDisplayData();
						Serializer.Deserialize(pool, offset, ref transferItemPreviewDisplayData);
						ItemMultiplyOperationPanel.RefreshTransferPreview(transferItemPreviewDisplayData, this.propertyChangeAlertness, this.propertyChangeFavor, this.propertyChangeHappiness, this.propertyChangeAlertnessBack, this.propertyChangeFavorBack, this.propertyChangeHappinessBack);
						this.CanvasGroup.alpha = 1f;
						this.RefreshPanelPos();
					});
				}
			}
		}

		// Token: 0x060055B6 RID: 21942 RVA: 0x0027B768 File Offset: 0x00279968
		private void Awake()
		{
			this.inputField.onValueChanged.RemoveAllListeners();
			this.inputField.onValueChanged.AddListener(new UnityAction<string>(this.OnEndEdit));
			this.inputField.onEndEdit.RemoveAllListeners();
			this.inputField.onEndEdit.AddListener(new UnityAction<string>(this.OnEndEdit));
			this.slider.onValueChanged.RemoveAllListeners();
			this.slider.onValueChanged.AddListener(delegate(float value)
			{
				this.CurCount = (int)value;
			});
			this.buttonMore.ClearAndAddListener(new Action(this.More));
			this.buttonLess.ClearAndAddListener(new Action(this.Less));
		}

		// Token: 0x060055B7 RID: 21943 RVA: 0x0027B830 File Offset: 0x00279A30
		private void OnEnable()
		{
			GEvent.Add(UiEvents.OnConfirmSetSelectCount, new GEvent.Callback(this.OnConfirmSetSelectCount));
		}

		// Token: 0x060055B8 RID: 21944 RVA: 0x0027B84F File Offset: 0x00279A4F
		private void OnDisable()
		{
			GEvent.Remove(UiEvents.OnConfirmSetSelectCount, new GEvent.Callback(this.OnConfirmSetSelectCount));
		}

		// Token: 0x060055B9 RID: 21945 RVA: 0x0027B870 File Offset: 0x00279A70
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

		// Token: 0x060055BA RID: 21946 RVA: 0x0027B8C4 File Offset: 0x00279AC4
		private void OnEndEdit(string value)
		{
			int result;
			int.TryParse(value, out result);
			this.CurCount = result;
		}

		// Token: 0x060055BB RID: 21947 RVA: 0x0027B8E4 File Offset: 0x00279AE4
		protected override void OnClick(Transform btn)
		{
			string name = btn.name;
			string a = name;
			if (!(a == "ButtonCancel") && !(a == "ButtonClose"))
			{
				if (a == "ButtonConfirm")
				{
					this.Confirm();
				}
			}
			else
			{
				this.Hide();
			}
		}

		// Token: 0x060055BC RID: 21948 RVA: 0x0027B938 File Offset: 0x00279B38
		public void Hide()
		{
			base.QuickHide();
			bool flag = !this._confirmClicked && !this._noCancelOnHide;
			if (flag)
			{
				this.CurCount = this._initCount;
			}
			bool flag2 = !this._noCancelOnHide;
			if (flag2)
			{
				Action<int> onValueChanged = this._onValueChanged;
				if (onValueChanged != null)
				{
					onValueChanged(0);
				}
			}
			bool flag3 = this._confirmClicked && (this._zeroValid || this.CurCount > 0);
			if (flag3)
			{
				Action<int> onConfirmSetCount = this._onConfirmSetCount;
				if (onConfirmSetCount != null)
				{
					onConfirmSetCount(this.CurCount);
				}
				AudioManager.Instance.PlaySound(this.ConfirmSound, false, false);
			}
			else
			{
				Action onCancelSetCount = this._onCancelSetCount;
				if (onCancelSetCount != null)
				{
					onCancelSetCount();
				}
			}
		}

		// Token: 0x060055BD RID: 21949 RVA: 0x0027B9F6 File Offset: 0x00279BF6
		public override void QuickHide()
		{
			this.Hide();
		}

		// Token: 0x060055BE RID: 21950 RVA: 0x0027BA00 File Offset: 0x00279C00
		private void OnConfirmSetSelectCount(ArgumentBox _)
		{
			this.Confirm();
		}

		// Token: 0x060055BF RID: 21951 RVA: 0x0027BA09 File Offset: 0x00279C09
		private void Confirm()
		{
			this.OnEndEdit(this.inputField.text);
			this._confirmClicked = true;
			this.Hide();
		}

		// Token: 0x060055C0 RID: 21952 RVA: 0x0027BA2C File Offset: 0x00279C2C
		private void More()
		{
			bool flag = this.CurCount >= this.MaxSelectCount;
			if (flag)
			{
				bool flag2 = !this._loop;
				if (!flag2)
				{
					this.CurCount = this._minCount;
				}
			}
			else
			{
				bool flag3 = this.CurCount < this._changeValue;
				if (flag3)
				{
					this.CurCount = this._changeValue;
				}
				else
				{
					this.CurCount += this._changeValue;
				}
			}
		}

		// Token: 0x060055C1 RID: 21953 RVA: 0x0027BAA8 File Offset: 0x00279CA8
		private void Less()
		{
			bool flag = this.CurCount <= this._minCount;
			if (flag)
			{
				bool flag2 = !this._loop;
				if (!flag2)
				{
					this.CurCount = this.MaxSelectCount;
				}
			}
			else
			{
				this.CurCount -= this._changeValue;
			}
		}

		// Token: 0x060055C2 RID: 21954 RVA: 0x0027BAFF File Offset: 0x00279CFF
		private void OnCountChange()
		{
			this.UpdateButtonState();
			Action<int> onValueChanged = this._onValueChanged;
			if (onValueChanged != null)
			{
				onValueChanged(this.CurCount);
			}
			this.UpdateContent();
			this.RefreshTransferItemPreview();
		}

		// Token: 0x060055C3 RID: 21955 RVA: 0x0027BB30 File Offset: 0x00279D30
		private void UpdateContent()
		{
			this.inputField.SetTextWithoutNotify(this.CurCount.ToString());
			this.slider.SetValueWithoutNotify((float)this.CurCount);
			this.textMax.text = string.Format("/{0}", this._maxCount);
		}

		// Token: 0x060055C4 RID: 21956 RVA: 0x0027BB8C File Offset: 0x00279D8C
		private void UpdateButtonState()
		{
			bool flag = this.buttonMore;
			if (flag)
			{
				this.buttonMore.interactable = (this.CurCount < this.MaxSelectCount);
				this.buttonMore.GetComponent<TooltipInvoker>().enabled = (this.CurCount >= this._limitCount && !this._limitTip.IsNullOrEmpty());
			}
			bool flag2 = this.buttonLess;
			if (flag2)
			{
				this.buttonLess.interactable = (this.CurCount > this._minCount);
			}
		}

		// Token: 0x060055C5 RID: 21957 RVA: 0x0027BC20 File Offset: 0x00279E20
		public void ShowTip()
		{
			bool flag = this.tipRectTransform != null;
			if (flag)
			{
				this.tipRectTransform.gameObject.SetActive(true);
			}
			this._refreshCount++;
			this.RefreshPanelPos();
		}

		// Token: 0x060055C6 RID: 21958 RVA: 0x0027BC68 File Offset: 0x00279E68
		public void HideTip()
		{
			bool flag = this.tipRectTransform != null;
			if (flag)
			{
				this.tipRectTransform.gameObject.SetActive(false);
			}
		}

		// Token: 0x060055C7 RID: 21959 RVA: 0x0027BC98 File Offset: 0x00279E98
		public void SetTipPosition(RectTransform target)
		{
			bool flag = this.tipRectTransform == null || target == null;
			if (!flag)
			{
				this.tipRectTransform.gameObject.GetOrAddComponent<CanvasGroup>().alpha = 0f;
				SingletonObject.getInstance<YieldHelper>().DelayFrameDo(1U, delegate
				{
					RectTransform tipRect = this.tipRectTransform;
					RectTransform targetRect = target;
					Transform parent = targetRect.parent;
					tipRect.SetParent(parent, false);
					Vector3[] targetCorners = new Vector3[4];
					targetRect.GetWorldCorners(targetCorners);
					Vector3 targetTopCenter = (targetCorners[1] + targetCorners[2]) * 0.5f;
					Vector3[] tipCorners = new Vector3[4];
					tipRect.GetWorldCorners(tipCorners);
					Vector3 tipBottomCenter = (tipCorners[0] + tipCorners[3]) * 0.5f;
					float tipHeight = tipCorners[2].y - tipCorners[0].y;
					float yOffset = targetTopCenter.y - tipBottomCenter.y + tipHeight * 0.5f;
					Vector3 newPos = new Vector3(targetTopCenter.x, tipBottomCenter.y + yOffset, tipCorners[0].z);
					Vector3 localPos = parent.InverseTransformPoint(newPos);
					tipRect.anchoredPosition = new Vector2(localPos.x, localPos.y);
					Vector3 tipScreenPos = UIManager.Instance.UiCamera.WorldToScreenPoint(tipRect.position);
					Rect itemRect = CommonUtils.RectTransToScreenPos(tipRect, UIManager.Instance.UiCamera);
					Rect scrollRect = CommonUtils.RectTransToScreenPos(this.RectTransform, UIManager.Instance.UiCamera);
					bool isExceedLeft = (int)itemRect.xMin < (int)scrollRect.xMin;
					bool isExceedRight = (int)itemRect.xMax > (int)scrollRect.xMax;
					bool flag2 = isExceedLeft;
					if (flag2)
					{
						tipScreenPos.x += scrollRect.xMin - itemRect.xMin;
					}
					else
					{
						bool flag3 = isExceedRight;
						if (flag3)
						{
							tipScreenPos.x += scrollRect.xMax - itemRect.xMax;
						}
					}
					tipRect.position = UIManager.Instance.UiCamera.ScreenToWorldPoint(tipScreenPos);
					this.tipRectTransform.gameObject.GetOrAddComponent<CanvasGroup>().alpha = 1f;
				});
			}
		}

		// Token: 0x060055C8 RID: 21960 RVA: 0x0027BD10 File Offset: 0x00279F10
		public void ResetTipPosition()
		{
			bool flag = this.tipRectTransform == null;
			if (!flag)
			{
				this.tipRectTransform.SetParent(base.transform, false);
				this.tipRectTransform.anchoredPosition = Vector2.zero;
			}
		}

		// Token: 0x04003A7D RID: 14973
		public const string InitCountKey = "InitCount";

		// Token: 0x04003A7E RID: 14974
		public const string MaxCountKey = "MaxCount";

		// Token: 0x04003A7F RID: 14975
		public const string LimitCountKey = "LimitCount";

		// Token: 0x04003A80 RID: 14976
		public const string LimitTipKey = "LimitTip";

		// Token: 0x04003A81 RID: 14977
		public const string MinCountKey = "MinCount";

		// Token: 0x04003A82 RID: 14978
		public const string Loop = "Loop";

		// Token: 0x04003A83 RID: 14979
		public const string OnValueChanged = "OnValueChanged";

		// Token: 0x04003A84 RID: 14980
		public const string OnConfirmSetCount = "OnConfirmSetCount";

		// Token: 0x04003A85 RID: 14981
		public const string OnCancelSetCount = "OnCancelSetCount";

		// Token: 0x04003A86 RID: 14982
		public const string FollowOffset = "FollowOffset";

		// Token: 0x04003A87 RID: 14983
		public const string ChangeValue = "ChangeValue";

		// Token: 0x04003A88 RID: 14984
		public const string ZeroValid = "ZeroValid";

		// Token: 0x04003A89 RID: 14985
		public const string ItemRectTrans = "ItemRectTrans";

		// Token: 0x04003A8A RID: 14986
		public const string TargetItem = "TargetItem";

		// Token: 0x04003A8B RID: 14987
		public const string NoCancelOnHide = "NoCancelOnHide";

		// Token: 0x04003A8C RID: 14988
		private int _initCount;

		// Token: 0x04003A8D RID: 14989
		private int _curCount;

		// Token: 0x04003A8E RID: 14990
		private bool _noCancelOnHide;

		// Token: 0x04003A8F RID: 14991
		private int _maxCount;

		// Token: 0x04003A90 RID: 14992
		private int _limitCount;

		// Token: 0x04003A91 RID: 14993
		private string _limitTip;

		// Token: 0x04003A92 RID: 14994
		private int _minCount;

		// Token: 0x04003A93 RID: 14995
		private bool _zeroValid;

		// Token: 0x04003A94 RID: 14996
		private bool _loop;

		// Token: 0x04003A95 RID: 14997
		private bool _confirmClicked;

		// Token: 0x04003A96 RID: 14998
		private Action<int> _onValueChanged;

		// Token: 0x04003A97 RID: 14999
		private Action<int> _onConfirmSetCount;

		// Token: 0x04003A98 RID: 15000
		private Action _onCancelSetCount;

		// Token: 0x04003A99 RID: 15001
		private int _changeValue = 1;

		// Token: 0x04003A9A RID: 15002
		private ItemDisplayData _targetItem;

		// Token: 0x04003A9C RID: 15004
		private RectTransform _itemRectTransform;

		// Token: 0x04003A9D RID: 15005
		private Vector2 _followOffset;

		// Token: 0x04003A9E RID: 15006
		private int _refreshCount;

		// Token: 0x04003A9F RID: 15007
		[SerializeField]
		private GameObject panel;

		// Token: 0x04003AA0 RID: 15008
		[SerializeField]
		private CButton buttonMore;

		// Token: 0x04003AA1 RID: 15009
		[SerializeField]
		private CButton buttonLess;

		// Token: 0x04003AA2 RID: 15010
		[SerializeField]
		private CSlider slider;

		// Token: 0x04003AA3 RID: 15011
		[SerializeField]
		private TMP_InputField inputField;

		// Token: 0x04003AA4 RID: 15012
		[SerializeField]
		private TextMeshProUGUI textMax;

		// Token: 0x04003AA5 RID: 15013
		[Header("预览转赠或拿取的效果")]
		[SerializeField]
		private PropertyChange propertyChangeAlertness;

		// Token: 0x04003AA6 RID: 15014
		[SerializeField]
		private PropertyChange propertyChangeFavor;

		// Token: 0x04003AA7 RID: 15015
		[SerializeField]
		private PropertyChange propertyChangeHappiness;

		// Token: 0x04003AA8 RID: 15016
		[SerializeField]
		private GameObject propertyChangeAlertnessBack;

		// Token: 0x04003AA9 RID: 15017
		[SerializeField]
		private GameObject propertyChangeFavorBack;

		// Token: 0x04003AAA RID: 15018
		[SerializeField]
		private GameObject propertyChangeHappinessBack;

		// Token: 0x04003AAB RID: 15019
		[SerializeField]
		private RectTransform tipRectTransform;
	}
}
