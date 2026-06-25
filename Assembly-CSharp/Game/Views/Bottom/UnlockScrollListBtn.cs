using System;
using System.Collections;
using System.Collections.Generic;
using FrameWork;
using FrameWork.UISystem.UIElements;
using Game.Views.GameLineScroll;
using GameData.Domains.Taiwu;
using GameData.Serializer;
using GameData.Utilities;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Game.Views.Bottom
{
	// Token: 0x02000C48 RID: 3144
	public class UnlockScrollListBtn : MonoBehaviour
	{
		// Token: 0x170010D8 RID: 4312
		// (get) Token: 0x06009FC9 RID: 40905 RVA: 0x004AA1CF File Offset: 0x004A83CF
		private ViewBottom Parent
		{
			get
			{
				return UIElement.Bottom.UiBaseAs<ViewBottom>();
			}
		}

		// Token: 0x06009FCA RID: 40906 RVA: 0x004AA1DB File Offset: 0x004A83DB
		private string GetAnimName(bool isLock)
		{
			return isLock ? "eff_huijuan_ui_001_hui" : "eff_huijuan_ui_001_huang";
		}

		// Token: 0x06009FCB RID: 40907 RVA: 0x004AA1EC File Offset: 0x004A83EC
		private void Awake()
		{
			this.pointerTrigger.EnterEvent.ResetListener(delegate()
			{
				this.hover.gameObject.SetActive(this.unlockBtn.interactable);
			});
			this.pointerTrigger.ExitEvent.ResetListener(delegate()
			{
				this.hover.gameObject.SetActive(false);
			});
			this.unlockBtn.ClearAndAddListener(delegate
			{
				UIElement.GameLineScroll.SetOnInitArgs(EasyPool.Get<ArgumentBox>().Set("index", this._unlockScrollList[0].Second).Set("isUnlockScroll", true).Set("targetScrollIndex", 0));
				UIManager.Instance.MaskUI(UIElement.GameLineScroll);
				Game.Views.GameLineScroll.ScrollHelper.ProcessUnlockScrollList(this._unlockScrollList[0].Second, false);
			});
			this.clearBtn.ClearAndAddListener(delegate
			{
				List<IntPair> unlockScrollList = this._unlockScrollList;
				if (unlockScrollList != null)
				{
					unlockScrollList.Clear();
				}
				this.Refresh(false);
				TaiwuDomainMethod.Call.UpdateUnlockScrollList(this._unlockScrollList);
			});
			this._onAdventureEnter = delegate(ArgumentBox v)
			{
				this.OnAdventureRemakeChanged(true);
			};
			this._onAdventureExit = delegate(ArgumentBox v)
			{
				this.OnAdventureRemakeChanged(false);
			};
		}

		// Token: 0x06009FCC RID: 40908 RVA: 0x004AA288 File Offset: 0x004A8488
		private void OnEnable()
		{
			GEvent.Add(UiEvents.NotifyBottomUnlockScrollListChange, new GEvent.Callback(this.OnUnlockScrollListChange));
			GEvent.Add(UiEvents.AdventureRemakeEnter, this._onAdventureEnter);
			GEvent.Add(UiEvents.AdventureRemakeExit, this._onAdventureExit);
			GEvent.Add(UiEvents.OnUIElementHide, new GEvent.Callback(this.OnUIElementHide));
			GEvent.Add(UiEvents.ShowUnlockScrollBtnAnim, new GEvent.Callback(this.ShowUnlockScrollBtnAnim));
			TaiwuDomainMethod.AsyncCall.GetUnlockScrollListForDisplay(this.Parent, delegate(int offset, RawDataPool dataPool)
			{
				Serializer.Deserialize(dataPool, offset, ref this._unlockScrollList);
				this.Refresh(false);
			});
		}

		// Token: 0x06009FCD RID: 40909 RVA: 0x004AA32C File Offset: 0x004A852C
		private void OnUIElementHide(ArgumentBox argBox)
		{
			UIElement element;
			bool flag = !argBox.Get<UIElement>("Element", out element) || element != UIElement.GameLineScroll;
			if (!flag)
			{
				this.Refresh(true);
			}
		}

		// Token: 0x06009FCE RID: 40910 RVA: 0x004AA365 File Offset: 0x004A8565
		private void OnAdventureRemakeChanged(bool isEnter)
		{
			(base.transform as RectTransform).anchoredPosition = (isEnter ? this.adventurteLocation : this.commonLocation);
			this.Refresh(false);
		}

		// Token: 0x06009FCF RID: 40911 RVA: 0x004AA392 File Offset: 0x004A8592
		private void OnUnlockScrollListChange(ArgumentBox argBox)
		{
			argBox.Get<List<IntPair>>("UnlockScrollList", out this._unlockScrollList);
			this.Refresh(false);
		}

		// Token: 0x06009FD0 RID: 40912 RVA: 0x004AA3B0 File Offset: 0x004A85B0
		private void ShowUnlockScrollBtnAnim(ArgumentBox argBox)
		{
			bool flag = !argBox.Get("NeedShowAnim", out this._needShowAnim);
			if (flag)
			{
				this._needShowAnim = false;
			}
		}

		// Token: 0x06009FD1 RID: 40913 RVA: 0x004AA3E0 File Offset: 0x004A85E0
		private void Refresh(bool playAnim = false)
		{
			GameObject gameObject = this.content.gameObject;
			List<IntPair> unlockScrollList = this._unlockScrollList;
			gameObject.SetActive(unlockScrollList != null && unlockScrollList.Count > 0 && (UIManager.Instance.IsFocusElement(UIElement.Bottom) || UIManager.Instance.IsFocusElement(UIElement.AdventureMajorEvent)));
			Selectable selectable = this.unlockBtn;
			unlockScrollList = this._unlockScrollList;
			selectable.interactable = (unlockScrollList != null && unlockScrollList.Count > 0);
			Selectable selectable2 = this.clearBtn;
			unlockScrollList = this._unlockScrollList;
			selectable2.interactable = (unlockScrollList != null && unlockScrollList.Count > 0);
			TMP_Text tmp_Text = this.scrollCount;
			unlockScrollList = this._unlockScrollList;
			string text;
			if (unlockScrollList == null || unlockScrollList.Count > 1)
			{
				LanguageKey languageKey = LanguageKey.LK_Scroll_UnLock_Multiple;
				List<IntPair> unlockScrollList2 = this._unlockScrollList;
				text = languageKey.TrFormat((unlockScrollList2 != null) ? new int?(unlockScrollList2.Count) : null);
			}
			else
			{
				text = LanguageKey.LK_Scroll_UnLock_Single.Tr();
			}
			tmp_Text.text = text;
			unlockScrollList = this._unlockScrollList;
			this.ShowBtn(unlockScrollList != null && unlockScrollList.Count > 0 && this._needShowAnim && playAnim);
		}

		// Token: 0x06009FD2 RID: 40914 RVA: 0x004AA4FC File Offset: 0x004A86FC
		private void ShowBtn(bool playAnim)
		{
			this.unlockAnimation.gameObject.SetActive(playAnim);
			this.unlockBtn.GetComponent<CImage>().enabled = !playAnim;
			if (playAnim)
			{
				this._needShowAnim = false;
				string animName = this.GetAnimName(false);
				float duration = this.unlockAnimation[animName].length;
				this.unlockAnimation.Play(animName);
				base.StartCoroutine(this.WaitForAnimationComplete(duration, delegate
				{
					this.unlockBtn.GetComponent<CImage>().enabled = true;
					this.unlockAnimation.gameObject.SetActive(false);
				}));
			}
		}

		// Token: 0x06009FD3 RID: 40915 RVA: 0x004AA580 File Offset: 0x004A8780
		private IEnumerator WaitForAnimationComplete(float duration, Action onComplete)
		{
			yield return new WaitForSeconds(duration);
			if (onComplete != null)
			{
				onComplete();
			}
			yield break;
		}

		// Token: 0x06009FD4 RID: 40916 RVA: 0x004AA5A0 File Offset: 0x004A87A0
		private void OnDisable()
		{
			GEvent.Remove(UiEvents.NotifyBottomUnlockScrollListChange, new GEvent.Callback(this.OnUnlockScrollListChange));
			GEvent.Remove(UiEvents.AdventureRemakeEnter, this._onAdventureEnter);
			GEvent.Remove(UiEvents.AdventureRemakeExit, this._onAdventureExit);
			GEvent.Remove(UiEvents.ShowUnlockScrollBtnAnim, new GEvent.Callback(this.ShowUnlockScrollBtnAnim));
			GEvent.Remove(UiEvents.OnUIElementHide, new GEvent.Callback(this.OnUIElementHide));
		}

		// Token: 0x04007BA8 RID: 31656
		[SerializeField]
		private RectTransform content;

		// Token: 0x04007BA9 RID: 31657
		[SerializeField]
		private CButton unlockBtn;

		// Token: 0x04007BAA RID: 31658
		[SerializeField]
		private CImage btnImage;

		// Token: 0x04007BAB RID: 31659
		[SerializeField]
		private CImage countBack;

		// Token: 0x04007BAC RID: 31660
		[SerializeField]
		private TextMeshProUGUI scrollCount;

		// Token: 0x04007BAD RID: 31661
		[SerializeField]
		private CImage hover;

		// Token: 0x04007BAE RID: 31662
		[SerializeField]
		private PointerTrigger pointerTrigger;

		// Token: 0x04007BAF RID: 31663
		[SerializeField]
		private CButton clearBtn;

		// Token: 0x04007BB0 RID: 31664
		[SerializeField]
		private Animation unlockAnimation;

		// Token: 0x04007BB1 RID: 31665
		private List<IntPair> _unlockScrollList;

		// Token: 0x04007BB2 RID: 31666
		private readonly Vector2 adventurteLocation = new Vector2(0f, 1035f);

		// Token: 0x04007BB3 RID: 31667
		private readonly Vector2 commonLocation = new Vector2(400f, 170f);

		// Token: 0x04007BB4 RID: 31668
		private bool _needShowAnim = false;

		// Token: 0x04007BB5 RID: 31669
		private GEvent.Callback _onAdventureEnter;

		// Token: 0x04007BB6 RID: 31670
		private GEvent.Callback _onAdventureExit;
	}
}
