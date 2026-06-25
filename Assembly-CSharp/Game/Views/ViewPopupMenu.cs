using System;
using System.Collections.Generic;
using System.Linq;
using FrameWork;
using FrameWork.UISystem.UIElements;
using Game.Components.Common;
using Game.Views.CharacterMenu;
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
	// Token: 0x0200070B RID: 1803
	public class ViewPopupMenu : UIBase
	{
		// Token: 0x17000A67 RID: 2663
		// (get) Token: 0x06005559 RID: 21849 RVA: 0x00278E7D File Offset: 0x0027707D
		// (set) Token: 0x0600555A RID: 21850 RVA: 0x00278E85 File Offset: 0x00277085
		public string ConfirmSound { get; set; } = "ui_default_second_select";

		// Token: 0x0600555B RID: 21851 RVA: 0x00278E90 File Offset: 0x00277090
		public override void OnInit(ArgumentBox argsBox)
		{
			List<ViewPopupMenu.BtnData> btnList;
			argsBox.Get<List<ViewPopupMenu.BtnData>>("BtnInfo", out btnList);
			Vector3 screenPos;
			argsBox.Get<Vector3>("ScreenPos", out screenPos);
			Vector2 itemSize;
			argsBox.Get<Vector2>("ItemSize", out itemSize);
			argsBox.Get<Action>("OnCancel", out this._onCancel);
			bool flag = !argsBox.Get<ItemDisplayData>("TargetItem", out this._targetItem);
			if (flag)
			{
				this._targetItem = null;
			}
			this._btnClicked = false;
			this.ConfirmSound = "ui_default_second_select";
			btnList = (from btn in btnList
			orderby btn.DisplayOrder
			select btn).ToList<ViewPopupMenu.BtnData>();
			for (int i = 0; i < btnList.Count; i++)
			{
				ViewPopupMenu.BtnData btnInfo = btnList[i];
				CButton btn2 = (i < this.btnHolder.childCount) ? this.btnHolder.GetChild(i).GetComponent<CButton>() : Object.Instantiate<CButton>(this.btnPrefab, this.btnHolder, false);
				PointerTrigger pointerTrigger = btn2.GetComponent<PointerTrigger>();
				btn2.interactable = btnInfo.Interactable;
				btn2.ClearAndAddListener(delegate
				{
					this._btnClicked = true;
					this.QuickHide();
					btnInfo.OnClick();
					AudioManager.Instance.PlaySound(this.ConfirmSound, false, false);
				});
				btn2.gameObject.SetActive(true);
				TextMeshProUGUI txt = btn2.GetComponentInChildren<TextMeshProUGUI>();
				txt.text = btnInfo.Name;
				txt.color = (btn2.interactable ? Colors.Instance["yellow"] : Colors.Instance["grey"]);
				pointerTrigger.EnterEvent.RemoveAllListeners();
				pointerTrigger.ExitEvent.RemoveAllListeners();
				bool flag2 = btnInfo.OnEnter != null;
				if (flag2)
				{
					pointerTrigger.EnterEvent.AddListener(btnInfo.OnEnter);
				}
				bool flag3 = btnInfo.OnExit != null;
				if (flag3)
				{
					pointerTrigger.ExitEvent.AddListener(btnInfo.OnExit);
				}
				TooltipInvoker tip = btn2.GetComponent<TooltipInvoker>();
				bool flag4 = string.IsNullOrEmpty(btnInfo.TipContent);
				if (flag4)
				{
					tip.enabled = false;
				}
				else
				{
					bool flag5 = string.IsNullOrEmpty(btnInfo.TipTitle);
					string[] tipPresetParam;
					if (flag5)
					{
						tip.Type = TipType.SingleDesc;
						tipPresetParam = new string[]
						{
							btnInfo.TipContent
						};
					}
					else
					{
						tip.Type = TipType.Simple;
						tipPresetParam = new string[]
						{
							btnInfo.TipTitle,
							btnInfo.TipContent
						};
					}
					bool isTestPoison = btnInfo.IsTestPoison;
					if (isTestPoison)
					{
						tip.encyclopediaConfigTypeId = 98;
					}
					tip.enabled = true;
					tip.PresetParam = tipPresetParam;
				}
			}
			for (int j = btnList.Count; j < this.btnHolder.childCount; j++)
			{
				this.btnHolder.GetChild(j).gameObject.SetActive(false);
			}
			this.menuTransform.position = UIManager.Instance.UiCamera.ScreenToWorldPoint(screenPos);
			this.menuTransform.anchoredPosition += new Vector2(0f, -itemSize.y * 0.5f);
			CanvasGroup canvasGroup = base.gameObject.GetOrAddComponent<CanvasGroup>();
			canvasGroup.alpha = 0f;
			this.propertyChangeAlertnessBack.SetActive(false);
			this.propertyChangeFavorBack.SetActive(false);
			this.propertyChangeHappinessBack.SetActive(false);
			SingletonObject.getInstance<YieldHelper>().DelayFrameDo(2U, delegate
			{
				Rect itemRect = CommonUtils.RectTransToScreenPos(this.menuTransform, UIManager.Instance.UiCamera);
				Rect scrollRect = CommonUtils.RectTransToScreenPos(this.RectTransform, UIManager.Instance.UiCamera);
				bool isOverlap = !scrollRect.ContainsWithBorder(itemRect.min);
				bool flag6 = isOverlap;
				if (flag6)
				{
					this.menuTransform.anchoredPosition += new Vector2(0f, itemSize.y + this.menuTransform.rect.height);
				}
				canvasGroup.alpha = 1f;
			});
		}

		// Token: 0x0600555C RID: 21852 RVA: 0x0027927E File Offset: 0x0027747E
		private void OnEnable()
		{
			GEvent.Add(UiEvents.OnPointEnterItemMenuTake, new GEvent.Callback(this.OnPointEnterItemMenuTake));
			GEvent.Add(UiEvents.OnPointExitItemMenuTake, new GEvent.Callback(this.OnPointExitItemMenuTake));
		}

		// Token: 0x0600555D RID: 21853 RVA: 0x002792B9 File Offset: 0x002774B9
		private void OnDisable()
		{
			GEvent.Remove(UiEvents.OnPointEnterItemMenuTake, new GEvent.Callback(this.OnPointEnterItemMenuTake));
			GEvent.Remove(UiEvents.OnPointExitItemMenuTake, new GEvent.Callback(this.OnPointExitItemMenuTake));
		}

		// Token: 0x0600555E RID: 21854 RVA: 0x002792F4 File Offset: 0x002774F4
		private void OnPointEnterItemMenuTake(ArgumentBox _)
		{
			this.RefreshTransferItemPreview();
		}

		// Token: 0x0600555F RID: 21855 RVA: 0x002792FE File Offset: 0x002774FE
		private void OnPointExitItemMenuTake(ArgumentBox _)
		{
			this.propertyChangeAlertnessBack.SetActive(false);
			this.propertyChangeFavorBack.SetActive(false);
			this.propertyChangeHappinessBack.SetActive(false);
		}

		// Token: 0x06005560 RID: 21856 RVA: 0x00279328 File Offset: 0x00277528
		private void RefreshTransferItemPreview()
		{
			bool flag = this._targetItem == null;
			if (!flag)
			{
				Inventory inventory = new Inventory();
				inventory.OfflineAdd(this._targetItem.RealKey, 1);
				CharacterDomainMethod.AsyncCall.GetTransferItemPreviewDisplayData(this, this._targetItem.OwnerCharId, inventory, false, delegate(int offset, RawDataPool pool)
				{
					TransferItemPreviewDisplayData transferItemPreviewDisplayData = new TransferItemPreviewDisplayData();
					Serializer.Deserialize(pool, offset, ref transferItemPreviewDisplayData);
					ItemMultiplyOperationPanel.RefreshTransferPreview(transferItemPreviewDisplayData, this.propertyChangeAlertness, this.propertyChangeFavor, this.propertyChangeHappiness, this.propertyChangeAlertnessBack, this.propertyChangeFavorBack, this.propertyChangeHappinessBack);
				});
			}
		}

		// Token: 0x06005561 RID: 21857 RVA: 0x00279380 File Offset: 0x00277580
		protected override void OnClick(Transform btn)
		{
			string btnName = btn.name;
			bool flag = "Close" == btnName;
			if (flag)
			{
				this.QuickHide();
			}
		}

		// Token: 0x06005562 RID: 21858 RVA: 0x002793B0 File Offset: 0x002775B0
		public override void QuickHide()
		{
			for (int i = 0; i < this.btnHolder.childCount; i++)
			{
				this.btnHolder.GetChild(i).GetComponent<PointerTrigger>().ExitEvent.RemoveAllListeners();
			}
			bool flag = !this._btnClicked;
			if (flag)
			{
				Action onCancel = this._onCancel;
				if (onCancel != null)
				{
					onCancel();
				}
			}
			base.QuickHide();
		}

		// Token: 0x06005563 RID: 21859 RVA: 0x0027941C File Offset: 0x0027761C
		private void Update()
		{
			bool flag = CommonCommandKit.Esc.Check(this.Element, false, false, false, true, false);
			if (flag)
			{
				this.QuickHide();
			}
		}

		// Token: 0x04003A27 RID: 14887
		public const string TargetItem = "TargetItem";

		// Token: 0x04003A28 RID: 14888
		[SerializeField]
		private RectTransform btnHolder;

		// Token: 0x04003A29 RID: 14889
		[SerializeField]
		private CButton btnPrefab;

		// Token: 0x04003A2A RID: 14890
		[SerializeField]
		private RectTransform menuTransform;

		// Token: 0x04003A2B RID: 14891
		[Header("预览转赠或拿取的效果")]
		[SerializeField]
		private PropertyChange propertyChangeAlertness;

		// Token: 0x04003A2C RID: 14892
		[SerializeField]
		private PropertyChange propertyChangeFavor;

		// Token: 0x04003A2D RID: 14893
		[SerializeField]
		private PropertyChange propertyChangeHappiness;

		// Token: 0x04003A2E RID: 14894
		[SerializeField]
		private GameObject propertyChangeAlertnessBack;

		// Token: 0x04003A2F RID: 14895
		[SerializeField]
		private GameObject propertyChangeFavorBack;

		// Token: 0x04003A30 RID: 14896
		[SerializeField]
		private GameObject propertyChangeHappinessBack;

		// Token: 0x04003A31 RID: 14897
		private Action _onCancel;

		// Token: 0x04003A32 RID: 14898
		private bool _btnClicked;

		// Token: 0x04003A33 RID: 14899
		private ItemDisplayData _targetItem;

		// Token: 0x02001B5A RID: 7002
		public class BtnData
		{
			// Token: 0x0600E1BE RID: 57790 RVA: 0x005DF8C3 File Offset: 0x005DDAC3
			public BtnData(string name, bool interactable, EItemMenuDisplayOrder displayOrder, Action onClick, UnityAction onEnter = null, UnityAction onExit = null, bool isTestPoison = false)
			{
				this.Name = name;
				this.Interactable = interactable;
				this.DisplayOrder = displayOrder;
				this.OnClick = onClick;
				this.OnEnter = onEnter;
				this.OnExit = onExit;
				this.IsTestPoison = isTestPoison;
			}

			// Token: 0x0600E1BF RID: 57791 RVA: 0x005DF902 File Offset: 0x005DDB02
			public void SetTip(string tipTitle, string tipContent)
			{
				this.TipTitle = tipTitle;
				this.TipContent = tipContent;
			}

			// Token: 0x0400BA69 RID: 47721
			public readonly string Name;

			// Token: 0x0400BA6A RID: 47722
			public readonly bool Interactable;

			// Token: 0x0400BA6B RID: 47723
			public readonly Action OnClick;

			// Token: 0x0400BA6C RID: 47724
			public readonly UnityAction OnEnter;

			// Token: 0x0400BA6D RID: 47725
			public readonly UnityAction OnExit;

			// Token: 0x0400BA6E RID: 47726
			public readonly EItemMenuDisplayOrder DisplayOrder;

			// Token: 0x0400BA6F RID: 47727
			public string TipTitle;

			// Token: 0x0400BA70 RID: 47728
			public string TipContent;

			// Token: 0x0400BA71 RID: 47729
			public bool IsTestPoison;
		}
	}
}
