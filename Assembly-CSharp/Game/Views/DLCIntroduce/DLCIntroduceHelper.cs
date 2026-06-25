using System;
using System.Collections.Generic;
using System.Linq;
using Config;
using FrameWork;
using FrameWork.UISystem.Components;
using FrameWork.UISystem.UIElements;
using Steamworks;
using TMPro;
using UnityEngine;

namespace Game.Views.DLCIntroduce
{
	// Token: 0x02000A96 RID: 2710
	public class DLCIntroduceHelper : MonoBehaviour
	{
		// Token: 0x17000E86 RID: 3718
		// (get) Token: 0x0600848E RID: 33934 RVA: 0x003DA5D6 File Offset: 0x003D87D6
		// (set) Token: 0x0600848F RID: 33935 RVA: 0x003DA5DE File Offset: 0x003D87DE
		private int HoverIndex
		{
			get
			{
				return this._hoverIndex;
			}
			set
			{
				this._hoverIndex = value;
				this.OnHoverIndexChange();
			}
		}

		// Token: 0x17000E87 RID: 3719
		// (get) Token: 0x06008490 RID: 33936 RVA: 0x003DA5F0 File Offset: 0x003D87F0
		// (set) Token: 0x06008491 RID: 33937 RVA: 0x003DA618 File Offset: 0x003D8818
		public int SelectedIndex
		{
			get
			{
				return this._isHoverMode ? this.GetShowingIndex() : this._selectedIndex;
			}
			set
			{
				this._selectedIndex = value;
				this.OnSelectedIndexChange();
			}
		}

		// Token: 0x06008492 RID: 33938 RVA: 0x003DA62C File Offset: 0x003D882C
		private void Awake()
		{
			GEvent.Add(UiEvents.OnLanguageChange, new GEvent.Callback(this.OnLanguageChange));
			this.infinityScroll.OnItemRender += this.OnItemRender;
			this.infinityScroll.AddOnScrollEvent(new Action(this.OnScroll));
			this.infinityScroll.SetDataCount(DLCIntroduceHelper._dlcItems.Count);
			this.leftButton.ClearAndAddListener(delegate
			{
				bool flag2 = this.SelectedIndex <= 0;
				if (!flag2)
				{
					bool isHoverMode = this._isHoverMode;
					if (isHoverMode)
					{
						this.SelectedIndex = Math.Max(this.SelectedIndex - 4, 0);
					}
					else
					{
						int selectedIndex = this.SelectedIndex;
						this.SelectedIndex = selectedIndex - 1;
					}
				}
			});
			this.rightButton.ClearAndAddListener(delegate
			{
				bool flag2 = this.SelectedIndex >= this.infinityScroll.CurrentDataCount - 1;
				if (!flag2)
				{
					bool isHoverMode = this._isHoverMode;
					if (isHoverMode)
					{
						this.SelectedIndex = Math.Min(this.SelectedIndex + 4, this.infinityScroll.CurrentDataCount - 4);
					}
					else
					{
						int selectedIndex = this.SelectedIndex;
						this.SelectedIndex = selectedIndex + 1;
					}
				}
			});
			bool flag = this.detailsButton != null;
			if (flag)
			{
				this.detailsButton.ClearAndAddListener(new Action(this.OpenDetailsPanel));
			}
		}

		// Token: 0x06008493 RID: 33939 RVA: 0x003DA6F4 File Offset: 0x003D88F4
		private void OnLanguageChange(ArgumentBox argBox)
		{
			DLCIntroduceHelper._dlcItems.Clear();
			DLCIntroduceHelper._dlcItems.AddRange((from x in ImplementedDlc.Instance
			where x.TemplateId > 0
			orderby x.Order descending
			select x).ToList<ImplementedDlcItem>());
		}

		// Token: 0x06008494 RID: 33940 RVA: 0x003DA76A File Offset: 0x003D896A
		private void OnDestroy()
		{
			GEvent.Remove(UiEvents.OnLanguageChange, new GEvent.Callback(this.OnLanguageChange));
		}

		// Token: 0x06008495 RID: 33941 RVA: 0x003DA78C File Offset: 0x003D898C
		private void OnScroll()
		{
			CScrollRect scrollRect = this.infinityScroll.Scroll;
			bool flag = scrollRect.Content.rect.width < scrollRect.Viewport.rect.width;
			if (flag)
			{
				this.leftButton.interactable = false;
				this.rightButton.interactable = false;
			}
			else
			{
				float totalScrollDistance = scrollRect.Content.anchoredPosition.x * -1f + scrollRect.Viewport.rect.width;
				this.leftButton.interactable = (scrollRect.Content.anchoredPosition.x < 0f);
				this.rightButton.interactable = (totalScrollDistance < scrollRect.Content.rect.width);
			}
		}

		// Token: 0x06008496 RID: 33942 RVA: 0x003DA860 File Offset: 0x003D8A60
		private void Update()
		{
			bool flag = this._isHoverMode && this.infinityScroll.GetActiveCell(this.HoverIndex) && UIManager.Instance.IsFocusElement(UIElement.MainMenu) && Application.isFocused;
			if (flag)
			{
				DLCIntroduceHelper.UpdateHoverShowState(this.infinityScroll.transform as RectTransform, this.infoHolder);
			}
		}

		// Token: 0x06008497 RID: 33943 RVA: 0x003DA8CC File Offset: 0x003D8ACC
		private void OnItemRender(int index, GameObject obj)
		{
			DlCIntroduceScrollItem scrollItem = obj.GetComponent<DlCIntroduceScrollItem>();
			scrollItem.Set(DLCIntroduceHelper._dlcItems[index]);
			scrollItem.Toggle.onValueChanged.ResetListener(delegate(bool v)
			{
				if (v)
				{
					bool flag2 = index != this._selectedIndex;
					if (flag2)
					{
						this.SelectedIndex = index;
					}
				}
			});
			scrollItem.Toggle.SetIsOnWithoutNotify(index == this._selectedIndex && !this._isHoverMode);
			bool flag = this._isHoverMode && UIManager.Instance.IsFocusElement(UIElement.MainMenu);
			if (flag)
			{
				scrollItem.PointerTrigger.EnterEvent.ResetListener(delegate()
				{
					this.HoverIndex = index;
				});
			}
		}

		// Token: 0x06008498 RID: 33944 RVA: 0x003DA98C File Offset: 0x003D8B8C
		private void OpenDetailsPanel()
		{
			UIElement.DLCIntroduce.SetOnInitArgs(EasyPool.Get<ArgumentBox>().Set("SelectedIndex", this.HoverIndex));
			UIManager.Instance.MaskUI(UIElement.DLCIntroduce);
		}

		// Token: 0x06008499 RID: 33945 RVA: 0x003DA9C0 File Offset: 0x003D8BC0
		private void OnSelectedIndexChange()
		{
			this.infoHolder.gameObject.SetActive(this._selectedIndex >= 0);
			bool flag = this._selectedIndex >= 0;
			if (flag)
			{
				this.infinityScroll.ReRender();
				CScrollRect scroll = (this.infinityScroll.Scroll == null) ? this.infinityScroll.GetComponent<CScrollRect>() : this.infinityScroll.Scroll;
				scroll.ScrollTo(new Vector2((float)(-1 * this._selectedIndex) * (this.infinityScroll.srcPrefab.transform as RectTransform).rect.width, 0f), 0.3f);
			}
			else
			{
				this.infinityScroll.ReRender();
			}
			bool flag2 = this._selectedIndex < 0 || this._isHoverMode;
			if (!flag2)
			{
				ImplementedDlcItem dlcItem = DLCIntroduceHelper._dlcItems[this._selectedIndex];
				this.steamStoreButton.ClearAndAddListener(delegate
				{
					this.OpenSteamWebPage(dlcItem);
				});
				this.SetDLCSatus(dlcItem);
				bool flag3 = this.mainImageVertical;
				if (flag3)
				{
					ResLoader.Load<Texture2D>("RemakeResources/Textures/RemakeTextures/" + dlcItem.MainImageVertical, delegate(Texture2D texture)
					{
						this.mainImageVertical.texture = texture;
					}, null, false);
				}
				bool flag4 = this.mainImageHorizontal;
				if (flag4)
				{
					ResLoader.Load<Texture2D>("RemakeResources/Textures/RemakeTextures/" + dlcItem.MainImageHorizontal, delegate(Texture2D texture)
					{
						this.mainImageHorizontal.texture = texture;
					}, null, false);
				}
				bool flag5 = this.imagesHolder;
				if (flag5)
				{
					CommonUtils.PrepareEnoughChildren(this.imagesHolder, this.imagesHolder.GetChild(0).gameObject, dlcItem.Screenshots.Length, null);
					for (int i = 0; i < dlcItem.Screenshots.Length; i++)
					{
						CImage image = this.imagesHolder.GetChild(i).GetComponent<CImage>();
						image.SetSprite(dlcItem.Screenshots[i], false, null);
					}
				}
				int descCount = dlcItem.Desc.Length / 2;
				CommonUtils.PrepareEnoughChildren(this.descHolder, this.descHolder.GetChild(this.descHolderExtraCount).gameObject, descCount, new CommonUtils.PrepareExtraItemInfo?(new CommonUtils.PrepareExtraItemInfo
				{
					ExtraItemCount = this.descHolderExtraCount,
					TemplateOrder = CommonUtils.EPrepareTemplateOrder.AfterExtraItems
				}));
				for (int j = this.descHolderExtraCount; j < descCount + this.descHolderExtraCount; j++)
				{
					DlCIntroduceDescItem descItem = this.descHolder.GetChild(j).GetComponent<DlCIntroduceDescItem>();
					descItem.Set(dlcItem.Desc[2 * (j - this.descHolderExtraCount)], dlcItem.Desc[2 * (j - this.descHolderExtraCount) + 1]);
				}
			}
		}

		// Token: 0x0600849A RID: 33946 RVA: 0x003DACC8 File Offset: 0x003D8EC8
		private void OnHoverIndexChange()
		{
			this.infoHolder.gameObject.SetActive(this.HoverIndex >= 0);
			bool flag = this.HoverIndex < 0;
			if (!flag)
			{
				ImplementedDlcItem dlcItem = DLCIntroduceHelper._dlcItems[this.HoverIndex];
				this.steamStoreButton.ClearAndAddListener(delegate
				{
					this.OpenSteamWebPage(dlcItem);
				});
				this.SetDLCSatus(dlcItem);
				bool flag2 = this.mainImageVertical;
				if (flag2)
				{
					ResLoader.Load<Texture2D>("RemakeResources/Textures/RemakeTextures/" + dlcItem.MainImageVertical, delegate(Texture2D texture)
					{
						this.mainImageVertical.texture = texture;
					}, null, false);
				}
				bool flag3 = this.mainImageHorizontal;
				if (flag3)
				{
					ResLoader.Load<Texture2D>("RemakeResources/Textures/RemakeTextures/" + dlcItem.MainImageHorizontal, delegate(Texture2D texture)
					{
						this.mainImageHorizontal.texture = texture;
					}, null, false);
				}
				bool flag4 = this.imagesHolder;
				if (flag4)
				{
					CommonUtils.PrepareEnoughChildren(this.imagesHolder, this.imagesHolder.GetChild(0).gameObject, dlcItem.Screenshots.Length, null);
					for (int i = 0; i < dlcItem.Screenshots.Length; i++)
					{
						CImage image = this.imagesHolder.GetChild(i).GetComponent<CImage>();
						image.SetSprite(dlcItem.Screenshots[i], false, null);
					}
				}
				int descCount = dlcItem.Desc.Length / 2;
				CommonUtils.PrepareEnoughChildren(this.descHolder, this.descHolder.GetChild(this.descHolderExtraCount).gameObject, descCount, new CommonUtils.PrepareExtraItemInfo?(new CommonUtils.PrepareExtraItemInfo
				{
					ExtraItemCount = this.descHolderExtraCount,
					TemplateOrder = CommonUtils.EPrepareTemplateOrder.AfterExtraItems
				}));
				for (int j = this.descHolderExtraCount; j < descCount + this.descHolderExtraCount; j++)
				{
					DlCIntroduceDescItem descItem = this.descHolder.GetChild(j).GetComponent<DlCIntroduceDescItem>();
					descItem.Set(dlcItem.Desc[2 * (j - this.descHolderExtraCount)], dlcItem.Desc[2 * (j - this.descHolderExtraCount) + 1]);
				}
			}
		}

		// Token: 0x0600849B RID: 33947 RVA: 0x003DAF28 File Offset: 0x003D9128
		public void Set(int selectedIndex = -1, bool isHoverMode = false)
		{
			bool flag = !DLCIntroduceHelper._inited;
			if (flag)
			{
				DLCIntroduceHelper._dlcItems.AddRange((from x in ImplementedDlc.Instance
				where x.TemplateId > 0
				orderby x.Order descending
				select x).ToList<ImplementedDlcItem>());
				DLCIntroduceHelper._inited = true;
			}
			this.SelectedIndex = selectedIndex;
			this._isHoverMode = isHoverMode;
		}

		// Token: 0x0600849C RID: 33948 RVA: 0x003DAFB8 File Offset: 0x003D91B8
		public static int GetDlcIndex(ulong appID)
		{
			bool flag = !DLCIntroduceHelper._inited;
			if (flag)
			{
				DLCIntroduceHelper._dlcItems.AddRange((from x in ImplementedDlc.Instance
				where x.TemplateId > 0
				orderby x.Order descending
				select x).ToList<ImplementedDlcItem>());
				DLCIntroduceHelper._inited = true;
			}
			return DLCIntroduceHelper._dlcItems.FindIndex((ImplementedDlcItem x) => (ulong)x.AppId == appID);
		}

		// Token: 0x0600849D RID: 33949 RVA: 0x003DB060 File Offset: 0x003D9260
		private void OpenSteamWebPage(ImplementedDlcItem dlcItem)
		{
			string targetURL = this.GetSteamStorePageURL(dlcItem.AppId);
			bool initialized = SteamManager.Initialized;
			if (initialized)
			{
				SteamFriends.ActivateGameOverlayToWebPage(targetURL, EActivateGameOverlayToWebPageMode.k_EActivateGameOverlayToWebPageMode_Default);
			}
			else
			{
				Application.OpenURL(targetURL);
			}
		}

		// Token: 0x0600849E RID: 33950 RVA: 0x003DB09C File Offset: 0x003D929C
		private string GetSteamStorePageURL(uint appID)
		{
			return string.Format("https://store.steampowered.com/app/{0}", appID);
		}

		// Token: 0x0600849F RID: 33951 RVA: 0x003DB0C0 File Offset: 0x003D92C0
		private EDLCStatus GetDLCStatus(ImplementedDlcItem dlcItem)
		{
			bool flag = !SteamManager.Initialized;
			EDLCStatus result;
			if (flag)
			{
				result = EDLCStatus.UnSubscribed;
			}
			else
			{
				AppId_t dlcAppId = new AppId_t(dlcItem.AppId);
				bool ownsDlC = SteamApps.BIsDlcInstalled(dlcAppId);
				bool flag2 = ownsDlC;
				if (flag2)
				{
					bool canAccess = SteamApps.BIsSubscribedApp(dlcAppId);
					bool flag3 = canAccess;
					if (flag3)
					{
						result = EDLCStatus.Installed;
					}
					else
					{
						result = EDLCStatus.Subscribed;
					}
				}
				else
				{
					result = EDLCStatus.UnSubscribed;
				}
			}
			return result;
		}

		// Token: 0x060084A0 RID: 33952 RVA: 0x003DB11C File Offset: 0x003D931C
		private void SetDLCSatus(ImplementedDlcItem dlcItem)
		{
			EDLCStatus dlcStatus = this.GetDLCStatus(dlcItem);
			this.dlcStatusIcon.sprite = this.dlcStatusSprites[(int)dlcStatus];
			bool flag = this.dlcStatusBack;
			if (flag)
			{
				this.dlcStatusBack.gameObject.SetActive(dlcStatus == EDLCStatus.Installed);
			}
			TextMeshProUGUI textMeshProUGUI = this.dlcStatusLabel;
			if (!true)
			{
			}
			string text;
			switch (dlcStatus)
			{
			case EDLCStatus.UnSubscribed:
				text = LanguageKey.LK_DLCIntroduce_DLCStatus_UnSubscribed.Tr();
				break;
			case EDLCStatus.Subscribed:
				text = LanguageKey.LK_DLCIntroduce_DLCStatus_Subscribed.Tr();
				break;
			case EDLCStatus.Installed:
				text = LanguageKey.LK_DLCIntroduce_DLCStatus_Installed.Tr();
				break;
			default:
				if (!true)
				{
				}
				<PrivateImplementationDetails>.ThrowSwitchExpressionException(dlcStatus);
				break;
			}
			if (!true)
			{
			}
			textMeshProUGUI.text = text;
		}

		// Token: 0x060084A1 RID: 33953 RVA: 0x003DB1D0 File Offset: 0x003D93D0
		public static void UpdateHoverShowState(RectTransform triggerElement, RectTransform targetElement)
		{
			Camera uiCamera = UIManager.Instance.UiCamera;
			bool isHoverTrigger = DLCIntroduceHelper.IsMouseHovering(triggerElement, uiCamera);
			bool isHoverTarget = DLCIntroduceHelper.IsMouseHovering(targetElement, uiCamera);
			bool shouldShow = isHoverTrigger || isHoverTarget;
			bool flag = targetElement.gameObject.activeSelf != shouldShow;
			if (flag)
			{
				targetElement.gameObject.SetActive(shouldShow);
			}
		}

		// Token: 0x060084A2 RID: 33954 RVA: 0x003DB224 File Offset: 0x003D9424
		public static bool IsMouseHovering(RectTransform rectTransform, Camera uiCamera)
		{
			Vector2 localPos;
			return rectTransform.gameObject.activeSelf && RectTransformUtility.ScreenPointToLocalPointInRectangle(rectTransform, Input.mousePosition, uiCamera, out localPos) && rectTransform.rect.Contains(localPos);
		}

		// Token: 0x060084A3 RID: 33955 RVA: 0x003DB26C File Offset: 0x003D946C
		private int GetShowingIndex()
		{
			float contentX = Math.Abs(this.infinityScroll.Scroll.Content.anchoredPosition.x);
			float cellWidth = (this.infinityScroll.srcPrefab.transform as RectTransform).rect.width;
			return (int)(contentX / cellWidth);
		}

		// Token: 0x040065A4 RID: 26020
		[SerializeField]
		private InfinityScroll infinityScroll;

		// Token: 0x040065A5 RID: 26021
		[SerializeField]
		private CButton leftButton;

		// Token: 0x040065A6 RID: 26022
		[SerializeField]
		private CButton rightButton;

		// Token: 0x040065A7 RID: 26023
		[SerializeField]
		private RectTransform infoHolder;

		// Token: 0x040065A8 RID: 26024
		[SerializeField]
		private RectTransform descHolder;

		// Token: 0x040065A9 RID: 26025
		[SerializeField]
		private CRawImage mainImageHorizontal;

		// Token: 0x040065AA RID: 26026
		[SerializeField]
		private CRawImage mainImageVertical;

		// Token: 0x040065AB RID: 26027
		[SerializeField]
		private RectTransform imagesHolder;

		// Token: 0x040065AC RID: 26028
		[SerializeField]
		private CButton steamStoreButton;

		// Token: 0x040065AD RID: 26029
		[SerializeField]
		private CButton detailsButton;

		// Token: 0x040065AE RID: 26030
		[SerializeField]
		private CImage dlcStatusIcon;

		// Token: 0x040065AF RID: 26031
		[SerializeField]
		private TextMeshProUGUI dlcStatusLabel;

		// Token: 0x040065B0 RID: 26032
		[SerializeField]
		private Sprite[] dlcStatusSprites;

		// Token: 0x040065B1 RID: 26033
		[SerializeField]
		private CImage dlcStatusBack;

		// Token: 0x040065B2 RID: 26034
		[SerializeField]
		private int descHolderExtraCount = 2;

		// Token: 0x040065B3 RID: 26035
		private static bool _inited;

		// Token: 0x040065B4 RID: 26036
		private bool _isHoverMode;

		// Token: 0x040065B5 RID: 26037
		private int _hoverIndex;

		// Token: 0x040065B6 RID: 26038
		private int _selectedIndex;

		// Token: 0x040065B7 RID: 26039
		private static readonly List<ImplementedDlcItem> _dlcItems = new List<ImplementedDlcItem>();
	}
}
