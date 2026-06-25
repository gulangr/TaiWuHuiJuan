using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using FrameWork;
using FrameWork.UISystem.UIElements;
using Game.Components.Common;
using GameData.Domains.Character;
using GameData.Domains.Character.Display;
using GameData.GameDataBridge;
using GameData.Serializer;
using Spine;
using Spine.Unity;
using TMPro;
using UnityEngine;

namespace Game.Views.SectInteract.Zhujian
{
	// Token: 0x020009C5 RID: 2501
	public class ViewZhujianGearMate : UIBase
	{
		// Token: 0x17000D79 RID: 3449
		// (get) Token: 0x06007943 RID: 31043 RVA: 0x00385FCB File Offset: 0x003841CB
		public CharacterDisplayData DisplayData
		{
			get
			{
				return this._displayData;
			}
		}

		// Token: 0x17000D7A RID: 3450
		// (get) Token: 0x06007944 RID: 31044 RVA: 0x00385FD3 File Offset: 0x003841D3
		public int GearMateId
		{
			get
			{
				return this._gearMateId;
			}
		}

		// Token: 0x06007945 RID: 31045 RVA: 0x00385FDC File Offset: 0x003841DC
		public override void OnInit(ArgumentBox argsBox)
		{
			int charId;
			bool flag = argsBox.Get("CharacterId", out charId);
			if (flag)
			{
				this._gearMateId = charId;
			}
			this.InitSubPages();
			this.InitToggles();
			this.InitHotKeyCommands();
			this.NeedDataListenerId = true;
			UIElement element = this.Element;
			element.OnListenerIdReady = (Action)Delegate.Combine(element.OnListenerIdReady, new Action(this.OnListenerIdReady));
			this.changeButton.ClearAndAddListener(new Action(this.OnChangeGearMateClicked));
		}

		// Token: 0x06007946 RID: 31046 RVA: 0x00386060 File Offset: 0x00384260
		private void InitHotKeyCommands()
		{
			bool flag = !this._commonHotKeyCommands.ContainsKey(TabSwitchCommandKit.PrevTabLevel1);
			if (flag)
			{
				this._commonHotKeyCommands.Add(TabSwitchCommandKit.PrevTabLevel1, new Action(this.OnClickBtnLeft));
			}
			bool flag2 = !this._commonHotKeyCommands.ContainsKey(TabSwitchCommandKit.NextTabLevel1);
			if (flag2)
			{
				this._commonHotKeyCommands.Add(TabSwitchCommandKit.NextTabLevel1, new Action(this.OnClickBtnRight));
			}
		}

		// Token: 0x06007947 RID: 31047 RVA: 0x003860D8 File Offset: 0x003842D8
		private void InitSubPages()
		{
			foreach (ZhujianGearMateSubPage page in this.subPages)
			{
				page.Init(this);
				page.OnHide();
			}
		}

		// Token: 0x06007948 RID: 31048 RVA: 0x00386138 File Offset: 0x00384338
		private void Update()
		{
			foreach (KeyValuePair<HotKeyCommand, Action> keyValuePair in this._commonHotKeyCommands)
			{
				HotKeyCommand hotKeyCommand;
				Action action2;
				keyValuePair.Deconstruct(out hotKeyCommand, out action2);
				HotKeyCommand command = hotKeyCommand;
				Action action = action2;
				bool flag = command.Check(this.Element, false, false, true, true, false);
				if (flag)
				{
					action();
				}
			}
		}

		// Token: 0x06007949 RID: 31049 RVA: 0x003861B8 File Offset: 0x003843B8
		private void InitToggles()
		{
			bool flag = this.tabToggleGroup.Count() <= 0;
			if (flag)
			{
				CToggle[] toggles = this.tabToggleGroup.GetComponentsInChildren<CToggle>(true);
				foreach (CToggle toggle in toggles)
				{
					this.tabToggleGroup.Add(toggle);
				}
			}
			this.tabToggleGroup.Init(-1);
			ToggleGroupHotkeyController.Set(this.Element, this.tabToggleGroup, 0, null);
			this.tabToggleGroup.OnActiveIndexChange += delegate(int index, int old)
			{
				bool flag2 = index >= 0;
				if (flag2)
				{
					this.SwitchTab(index);
				}
			};
		}

		// Token: 0x0600794A RID: 31050 RVA: 0x0038624C File Offset: 0x0038444C
		private void OnListenerIdReady()
		{
			IReadOnlyCollection<int> gearMates = SingletonObject.getInstance<CharacterMonitorModel>().GetTaiwuGearMateGroup();
			this.changeButton.gameObject.SetActive(gearMates.Count > 1);
			CharacterDomainMethod.Call.GetCharacterDisplayData(this.Element.GameDataListenerId, this._gearMateId);
			foreach (ZhujianGearMateSubPage page in this.subPages)
			{
				page.OnListenerIdReady();
			}
			bool flag = this._currentTabIndex == -1 && this.subPages.Count > 0 && this.tabToggleGroup.Count() > 0;
			if (flag)
			{
				this.tabToggleGroup.Set(0, true);
			}
		}

		// Token: 0x0600794B RID: 31051 RVA: 0x0038631C File Offset: 0x0038451C
		public void SwitchTab(int index)
		{
			bool flag = index < 0 || index >= this.subPages.Count;
			if (!flag)
			{
				bool flag2 = this._currentTabIndex >= 0 && this._currentTabIndex < this.subPages.Count;
				if (flag2)
				{
					this.subPages[this._currentTabIndex].OnHide();
				}
				this._currentTabIndex = index;
				this.subPages[this._currentTabIndex].SetGearMateId(this._gearMateId);
				this.subPages[this._currentTabIndex].OnShow();
			}
		}

		// Token: 0x0600794C RID: 31052 RVA: 0x003863C0 File Offset: 0x003845C0
		public void SetGearMateId(int id)
		{
			bool flag = this._gearMateId == id;
			if (!flag)
			{
				this._gearMateId = id;
				bool flag2 = this.Element.GameDataListenerId > 0;
				if (flag2)
				{
					CharacterDomainMethod.Call.GetCharacterDisplayData(this.Element.GameDataListenerId, this._gearMateId);
				}
				bool flag3 = this._currentTabIndex >= 0 && this._currentTabIndex < this.subPages.Count;
				if (flag3)
				{
					this.subPages[this._currentTabIndex].SetGearMateId(this._gearMateId);
				}
			}
		}

		// Token: 0x0600794D RID: 31053 RVA: 0x00386454 File Offset: 0x00384654
		public override void OnNotifyGameData(List<NotificationWrapper> notifications)
		{
			foreach (NotificationWrapper wrapper in notifications)
			{
				Notification notification = wrapper.Notification;
				bool flag = notification.Type == 1;
				if (flag)
				{
					bool flag2 = notification.DomainId == 4 && notification.MethodId == 131;
					if (flag2)
					{
						Serializer.Deserialize(wrapper.DataPool, notification.ValueOffset, ref this._displayData);
						this.RefreshAvatar();
					}
				}
			}
		}

		// Token: 0x0600794E RID: 31054 RVA: 0x003864F8 File Offset: 0x003846F8
		private void RefreshAvatar()
		{
			bool flag = this._displayData == null;
			if (!flag)
			{
				bool isTongsheng = this._displayData.TemplateId == 722;
				this.spineTongsheng.gameObject.SetActive(isTongsheng);
				this.spineJieyun.gameObject.SetActive(!isTongsheng);
				SkeletonGraphic activeSpine = isTongsheng ? this.spineTongsheng : this.spineJieyun;
				activeSpine.AnimationState.Complete -= this.OnSpineAnimationComplete;
				activeSpine.AnimationState.Complete += this.OnSpineAnimationComplete;
				activeSpine.AnimationState.SetAnimation(0, "idle", true);
				this.nameLabel.text = this.GetGearMateName();
			}
		}

		// Token: 0x0600794F RID: 31055 RVA: 0x003865B9 File Offset: 0x003847B9
		public string GetGearMateName()
		{
			return NameCenter.GetMonasticTitleOrDisplayName(this._displayData, false);
		}

		// Token: 0x06007950 RID: 31056 RVA: 0x003865C8 File Offset: 0x003847C8
		private void OnSpineAnimationComplete(TrackEntry entry)
		{
			bool flag = entry.Animation.Name != "idle";
			if (flag)
			{
				SkeletonGraphic activeSpine = this.GetActiveSpine();
				if (activeSpine != null)
				{
					activeSpine.AnimationState.SetAnimation(0, "idle", true);
				}
			}
		}

		// Token: 0x06007951 RID: 31057 RVA: 0x00386610 File Offset: 0x00384810
		public void DoGearMateAnimation(string animationName)
		{
			SkeletonGraphic activeSpine = this.GetActiveSpine();
			bool flag = activeSpine != null && activeSpine.gameObject.activeSelf;
			if (flag)
			{
				activeSpine.AnimationState.SetAnimation(0, animationName, false);
			}
		}

		// Token: 0x06007952 RID: 31058 RVA: 0x00386654 File Offset: 0x00384854
		private SkeletonGraphic GetActiveSpine()
		{
			bool activeSelf = this.spineTongsheng.gameObject.activeSelf;
			SkeletonGraphic result;
			if (activeSelf)
			{
				result = this.spineTongsheng;
			}
			else
			{
				bool activeSelf2 = this.spineJieyun.gameObject.activeSelf;
				if (activeSelf2)
				{
					result = this.spineJieyun;
				}
				else
				{
					result = null;
				}
			}
			return result;
		}

		// Token: 0x06007953 RID: 31059 RVA: 0x003866A4 File Offset: 0x003848A4
		private void OnChangeGearMateClicked()
		{
			IReadOnlyCollection<int> gearMates = SingletonObject.getInstance<CharacterMonitorModel>().GetTaiwuGearMateGroup();
			List<int> charIds = gearMates.ToList<int>();
			bool flag = charIds.Count <= 1;
			if (!flag)
			{
				int currentIndex = charIds.IndexOf(this._gearMateId);
				int nextIndex = (currentIndex + 1) % charIds.Count;
				this.SetGearMateId(charIds[nextIndex]);
			}
		}

		// Token: 0x06007954 RID: 31060 RVA: 0x003866FE File Offset: 0x003848FE
		public void SetChangeButtonInteractable(bool interactable)
		{
			this.changeButton.interactable = interactable;
			this.tabToggleGroup.SetInteractable(interactable);
		}

		// Token: 0x06007955 RID: 31061 RVA: 0x0038671B File Offset: 0x0038491B
		public void ShowLoading(bool show)
		{
			this.loadingNode.SetActive(show);
		}

		// Token: 0x06007956 RID: 31062 RVA: 0x0038672C File Offset: 0x0038492C
		public void OnSubPageReady(ZhujianGearMateSubPage page)
		{
			bool flag = this._currentTabIndex < 0 || this._currentTabIndex >= this.subPages.Count;
			if (!flag)
			{
				bool flag2 = this.subPages[this._currentTabIndex] != page;
				if (!flag2)
				{
					this.ShowLoading(false);
					bool isFirstLoad = this._isFirstLoad;
					if (isFirstLoad)
					{
						this._isFirstLoad = false;
						bool flag3 = this.Element != null;
						if (flag3)
						{
							this.Element.ShowAfterRefresh();
						}
					}
				}
			}
		}

		// Token: 0x06007957 RID: 31063 RVA: 0x003867B4 File Offset: 0x003849B4
		public void ShowBubble(string bubbleTextContent, float duration)
		{
			this.bubbleText.text = bubbleTextContent;
			this.bubble.SetActive(true);
			bool flag = this._closeBubbleCoroutine != null;
			if (flag)
			{
				base.StopCoroutine(this._closeBubbleCoroutine);
			}
			this._closeBubbleCoroutine = base.StartCoroutine(this.CloseBubbleAfterDelay(duration));
		}

		// Token: 0x06007958 RID: 31064 RVA: 0x0038680C File Offset: 0x00384A0C
		public void HideBubble()
		{
			this.bubble.SetActive(false);
			bool flag = this._closeBubbleCoroutine != null;
			if (flag)
			{
				base.StopCoroutine(this._closeBubbleCoroutine);
				this._closeBubbleCoroutine = null;
			}
		}

		// Token: 0x06007959 RID: 31065 RVA: 0x0038684C File Offset: 0x00384A4C
		public bool CanQuickHideCurrentPage()
		{
			bool flag = this._currentTabIndex < 0 || this._currentTabIndex >= this.subPages.Count;
			return flag || this.subPages[this._currentTabIndex].CanQuickHide();
		}

		// Token: 0x0600795A RID: 31066 RVA: 0x0038689F File Offset: 0x00384A9F
		private IEnumerator CloseBubbleAfterDelay(float delay)
		{
			yield return new WaitForSeconds(delay);
			this.bubble.SetActive(false);
			this._closeBubbleCoroutine = null;
			yield break;
		}

		// Token: 0x0600795B RID: 31067 RVA: 0x003868B8 File Offset: 0x00384AB8
		public override void NotifyUIHide()
		{
			base.NotifyUIHide();
			this._isFirstLoad = true;
			this._currentTabIndex = -1;
			this.tabToggleGroup.DeSelectWithoutNotify();
			bool flag = this.subChangeAnim != null;
			if (flag)
			{
				this.subChangeAnim.ResetSubState();
			}
			this.HideBubble();
		}

		// Token: 0x0600795C RID: 31068 RVA: 0x0038690C File Offset: 0x00384B0C
		protected override void OnClick(Transform btn)
		{
			bool flag = btn.name == "ButtonCloseView";
			if (flag)
			{
				this.QuickHide();
			}
		}

		// Token: 0x0600795D RID: 31069 RVA: 0x00386938 File Offset: 0x00384B38
		public override void QuickHide()
		{
			bool flag = !this.CanQuickHideCurrentPage();
			if (!flag)
			{
				base.QuickHide();
			}
		}

		// Token: 0x0600795E RID: 31070 RVA: 0x00386960 File Offset: 0x00384B60
		private void OnClickBtnLeft()
		{
			bool flag = this._currentTabIndex == 0;
			if (!flag)
			{
				AudioManager.Instance.PlaySound("ui_default_click_left", false, false);
				int num = this._currentTabIndex - 1;
				this._currentTabIndex = num;
				this._currentTabIndex = Mathf.Max(num, 0);
				this.tabToggleGroup.Set(this._currentTabIndex, true);
			}
		}

		// Token: 0x0600795F RID: 31071 RVA: 0x003869C0 File Offset: 0x00384BC0
		private void OnClickBtnRight()
		{
			bool flag = this._currentTabIndex == this.subPages.Count - 1;
			if (!flag)
			{
				AudioManager.Instance.PlaySound("ui_default_click_left", false, false);
				int num = this._currentTabIndex + 1;
				this._currentTabIndex = num;
				this._currentTabIndex = Mathf.Min(num, this.subPages.Count - 1);
				this.tabToggleGroup.Set(this._currentTabIndex, true);
			}
		}

		// Token: 0x04005BF3 RID: 23539
		[SerializeField]
		private List<ZhujianGearMateSubPage> subPages;

		// Token: 0x04005BF4 RID: 23540
		[SerializeField]
		private CToggleGroup tabToggleGroup;

		// Token: 0x04005BF5 RID: 23541
		[SerializeField]
		private CommonSecondToggleSubChangeAnim subChangeAnim;

		// Token: 0x04005BF6 RID: 23542
		[SerializeField]
		private Transform subPageRoot;

		// Token: 0x04005BF7 RID: 23543
		[SerializeField]
		private SkeletonGraphic spineTongsheng;

		// Token: 0x04005BF8 RID: 23544
		[SerializeField]
		private SkeletonGraphic spineJieyun;

		// Token: 0x04005BF9 RID: 23545
		[SerializeField]
		private TextMeshProUGUI nameLabel;

		// Token: 0x04005BFA RID: 23546
		[SerializeField]
		private CButton changeButton;

		// Token: 0x04005BFB RID: 23547
		[SerializeField]
		private GameObject loadingNode;

		// Token: 0x04005BFC RID: 23548
		[SerializeField]
		private GameObject bubble;

		// Token: 0x04005BFD RID: 23549
		[SerializeField]
		private TextMeshProUGUI bubbleText;

		// Token: 0x04005BFE RID: 23550
		private int _gearMateId;

		// Token: 0x04005BFF RID: 23551
		private int _currentTabIndex = -1;

		// Token: 0x04005C00 RID: 23552
		private CharacterDisplayData _displayData;

		// Token: 0x04005C01 RID: 23553
		private readonly Dictionary<HotKeyCommand, Action> _commonHotKeyCommands = new Dictionary<HotKeyCommand, Action>();

		// Token: 0x04005C02 RID: 23554
		private bool _isFirstLoad = true;

		// Token: 0x04005C03 RID: 23555
		private Coroutine _closeBubbleCoroutine;
	}
}
