using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using FrameWork;
using GameData.Domains.Character;
using GameData.Domains.Character.Display;
using GameData.Domains.Extra;
using GameData.GameDataBridge;
using GameData.Serializer;
using GameData.Utilities;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace GearMate
{
	// Token: 0x02000621 RID: 1569
	public class UI_GearMate : UIBase
	{
		// Token: 0x06004A2F RID: 18991 RVA: 0x0022C068 File Offset: 0x0022A268
		public override void OnInit(ArgumentBox argsBox)
		{
			this.ReadArgs(argsBox);
			bool flag = !this._inited;
			if (flag)
			{
				this.InitRefers();
				this._gearMateAvatar.Init(this);
				this._itemSelector.Init(this);
				this.InitToggles();
				this.InitSubPages();
			}
			this._inited = true;
			this.NeedDataListenerId = true;
			UIElement element = this.Element;
			element.OnListenerIdReady = (Action)Delegate.Combine(element.OnListenerIdReady, new Action(this.OnListenerIdReady));
		}

		// Token: 0x06004A30 RID: 18992 RVA: 0x0022C0F1 File Offset: 0x0022A2F1
		private void ReadArgs(ArgumentBox argsBox)
		{
			argsBox.Get("TargetSubPageIndex", out this._targetSubPageIndex);
			argsBox.Get("CharacterId", out this._gearMateId);
		}

		// Token: 0x06004A31 RID: 18993 RVA: 0x0022C118 File Offset: 0x0022A318
		private void InitSubPages()
		{
			for (int i = 0; i < this._subPageRoot.childCount; i++)
			{
				GearMateSubPageBase subPage = this._subPageRoot.GetChild(i).GetComponent<GearMateSubPageBase>();
				bool flag = subPage != null;
				if (flag)
				{
					subPage.Init(this);
					this._subPages.Add(subPage);
				}
			}
		}

		// Token: 0x06004A32 RID: 18994 RVA: 0x0022C176 File Offset: 0x0022A376
		public void RegisterLeafSubPage(GearMateSubPageBase subPage)
		{
			this._leafSubPages.Add(subPage);
		}

		// Token: 0x06004A33 RID: 18995 RVA: 0x0022C188 File Offset: 0x0022A388
		public void OnCollisionEnter2D(Collision2D collision)
		{
			bool flag = collision != null;
			if (flag)
			{
				Object.Destroy(collision.gameObject);
			}
		}

		// Token: 0x06004A34 RID: 18996 RVA: 0x0022C1AC File Offset: 0x0022A3AC
		private void InitToggles()
		{
			ToggleGroup toggleGroup = this._tabToggleLayout.GetComponent<ToggleGroup>();
			for (int i = 0; i < this._tabToggleLayout.transform.childCount; i++)
			{
				CToggleObsolete toggle = this._tabToggleLayout.transform.GetChild(i).GetComponent<CToggleObsolete>();
				Refers refers = toggle.GetComponent<Refers>();
				CButtonObsolete button = refers.CGet<CButtonObsolete>("Button");
				TextMeshProUGUI nameLabel = refers.CGet<TextMeshProUGUI>("NameLabel");
				GameObject selected = refers.CGet<GameObject>("Selected");
				CImage icon = refers.CGet<CImage>("Icon");
				icon.SetSprite(UI_GearMate.PageToggleIcons[i], false, null);
				nameLabel.text = LocalStringManager.Get("LK_GearMate_Tab_" + i.ToString());
				toggle.group = toggleGroup;
				toggle.onValueChanged.RemoveAllListeners();
				toggle.onValueChanged.AddListener(delegate(bool isOn)
				{
					selected.SetActive(isOn);
				});
				int index = i;
				button.ClearAndAddListener(delegate
				{
					this.SwitchToSubPage(index);
					toggle.isOn = true;
				});
				this._tabButtons.Add(button);
			}
		}

		// Token: 0x06004A35 RID: 18997 RVA: 0x0022C2F0 File Offset: 0x0022A4F0
		private void OnListenerIdReady()
		{
			this._itemSelector.OnListenerIdReady();
			this._leafSubPages.ForEach(delegate(GearMateSubPageBase subPage)
			{
				subPage.OnListenerIdReady();
			});
			this._tabButtons[this._targetSubPageIndex].onClick.Invoke();
			this.RequestForRefreshGearMate();
		}

		// Token: 0x06004A36 RID: 18998 RVA: 0x0022C358 File Offset: 0x0022A558
		public void RequestForRefreshGearMate()
		{
			CharacterDomainMethod.Call.GetCharacterDisplayData(this.Element.GameDataListenerId, this._gearMateId);
			ExtraDomainMethod.Call.GetGearMateById(this.Element.GameDataListenerId, this._gearMateId);
		}

		// Token: 0x06004A37 RID: 18999 RVA: 0x0022C38C File Offset: 0x0022A58C
		public override void InitMonitorFieldIds()
		{
			HashSet<UIBase.MonitorDataField> set = new HashSet<UIBase.MonitorDataField>();
			foreach (GearMateSubPageBase subPage in this._leafSubPages)
			{
				subPage.InitMonitorFields();
			}
		}

		// Token: 0x06004A38 RID: 19000 RVA: 0x0022C3EC File Offset: 0x0022A5EC
		public override void OnNotifyGameData(List<NotificationWrapper> notifications)
		{
			foreach (NotificationWrapper wrapper in notifications)
			{
				Notification notification = wrapper.Notification;
				byte type = notification.Type;
				byte b = type;
				if (b != 0)
				{
					if (b == 1)
					{
						this.HandleMethodReturn(notification, wrapper);
					}
				}
				else
				{
					this.HandleDataModification(notification, wrapper);
				}
			}
		}

		// Token: 0x06004A39 RID: 19001 RVA: 0x0022C470 File Offset: 0x0022A670
		private void HandleMethodReturn(Notification notification, NotificationWrapper wrapper)
		{
			ushort domainId = notification.DomainId;
			ushort methodId = notification.MethodId;
			RawDataPool pool = wrapper.DataPool;
			int offset = notification.ValueOffset;
			bool flag = domainId == 4;
			if (flag)
			{
				bool flag2 = methodId == 131;
				if (flag2)
				{
					Serializer.Deserialize(pool, offset, ref this._gearMateDisplayData);
					this._gearMateAvatar.RefreshCharacter(this._gearMateDisplayData);
					bool flag3 = this._lastGearMateId != this._gearMateDisplayData.CharacterId;
					if (flag3)
					{
						this.OnGearMateDisplayDataChanged();
					}
					this.Element.ShowAfterRefresh();
				}
			}
			else
			{
				bool flag4 = domainId == 19;
				if (flag4)
				{
					bool flag5 = methodId == 147;
					if (flag5)
					{
						Serializer.Deserialize(pool, offset, ref this._gearMate);
						this.OnGearMateDataChanged();
					}
				}
			}
			this._itemSelector.HandleMethodReturn(notification, wrapper);
		}

		// Token: 0x06004A3A RID: 19002 RVA: 0x0022C54C File Offset: 0x0022A74C
		private void OnGearMateDisplayDataChanged()
		{
			foreach (GearMateSubPageBase subPage in this._leafSubPages)
			{
				subPage.OnGearMateCharacterIdChanged(this._lastGearMateId);
			}
		}

		// Token: 0x06004A3B RID: 19003 RVA: 0x0022C5AC File Offset: 0x0022A7AC
		private void OnGearMateDataChanged()
		{
			foreach (GearMateSubPageBase subPage in this._leafSubPages)
			{
				subPage.OnGearMateDataChanged();
			}
		}

		// Token: 0x06004A3C RID: 19004 RVA: 0x0022C604 File Offset: 0x0022A804
		private void HandleDataModification(Notification notification, NotificationWrapper wrapper)
		{
		}

		// Token: 0x06004A3D RID: 19005 RVA: 0x0022C608 File Offset: 0x0022A808
		private void SwitchToSubPage(int targetSubPageIndex)
		{
			bool flag = targetSubPageIndex < 0 || targetSubPageIndex >= this._subPages.Count;
			if (!flag)
			{
				bool flag2 = this._currentSubPageIndex == targetSubPageIndex;
				if (!flag2)
				{
					base.StartCoroutine(this.SwitchSubPageCoroutine(targetSubPageIndex));
				}
			}
		}

		// Token: 0x06004A3E RID: 19006 RVA: 0x0022C652 File Offset: 0x0022A852
		private IEnumerator SwitchSubPageCoroutine(int targetSubPageIndex)
		{
			GearMateSubPageBase targetPage = this._subPages[targetSubPageIndex];
			int num;
			for (int i = 0; i < this._subPages.Count; i = num + 1)
			{
				bool visible = i == targetSubPageIndex;
				bool flag = visible;
				if (flag)
				{
					this._subPages[i].gameObject.SetActive(true);
				}
				num = i;
			}
			bool flag2 = !UI_GearMate.PageEnabled[targetSubPageIndex];
			if (flag2)
			{
				yield return null;
				yield return null;
			}
			UI_GearMate.PageEnabled[targetSubPageIndex] = true;
			for (int j = 0; j < this._subPages.Count; j = num + 1)
			{
				bool visible2 = j == targetSubPageIndex;
				bool flag3 = !visible2;
				if (flag3)
				{
					this._subPages[j].GetComponent<RectTransform>().anchoredPosition = new Vector2(0f, 10000f);
					this._subPages[j].gameObject.SetActive(false);
				}
				else
				{
					this._subPages[j].GetComponent<RectTransform>().anchoredPosition = new Vector2(0f, 0f);
					this._subPages[j].OnEnableBySwitchPage(j);
				}
				num = j;
			}
			bool isLeaf = targetPage.IsLeaf;
			if (isLeaf)
			{
				this.SetCurrentLeafSubPage(targetPage);
			}
			this._currentSubPageIndex = targetSubPageIndex;
			bool flag4 = targetSubPageIndex < 3;
			if (flag4)
			{
				this.ItemSelector.SwitchSelectedItemDict(targetSubPageIndex);
			}
			this.ShowOrHideItemSelectorWhenSwitchSubPage();
			yield break;
		}

		// Token: 0x06004A3F RID: 19007 RVA: 0x0022C668 File Offset: 0x0022A868
		private void ShowOrHideItemSelectorWhenSwitchSubPage()
		{
			SubPage currentSubPageIndex = (SubPage)this._currentSubPageIndex;
			bool needItemSelector = currentSubPageIndex == SubPage.Attribute || currentSubPageIndex == SubPage.Consummate || currentSubPageIndex == SubPage.Feature || currentSubPageIndex == SubPage.Neili;
			this._itemSelector.gameObject.SetActive(needItemSelector);
			base.CGet<GameObject>("TopDeco").SetActive(needItemSelector);
			bool flag = needItemSelector;
			if (flag)
			{
				this._itemSelector.RefreshItemFilter();
			}
		}

		// Token: 0x06004A40 RID: 19008 RVA: 0x0022C6CD File Offset: 0x0022A8CD
		public override void QuickHide()
		{
			this.SetDisableClickActive(false);
			this._itemSelector.ClearSelectedItemDictArray();
			base.QuickHide();
		}

		// Token: 0x06004A41 RID: 19009 RVA: 0x0022C6EC File Offset: 0x0022A8EC
		protected override void OnClick(Transform button)
		{
			string buttonName = button.name;
			bool flag = buttonName == "Close";
			if (flag)
			{
				this.SetDisableClickActive(false);
				this._itemSelector.ClearSelectedItemDictArray();
				base.QuickHide();
			}
			else
			{
				bool flag2 = buttonName == "BtnSelectAll";
				if (flag2)
				{
					this._itemSelector.SelectAll();
				}
				else
				{
					bool flag3 = buttonName == "BtnMultiplyOption";
					if (flag3)
					{
						this._itemSelector.OpenMultiplyOption(button.GetComponent<CButtonObsolete>());
					}
					else
					{
						bool flag4 = buttonName == "ButtonConfirm";
						if (flag4)
						{
							this._itemSelector.Confirm();
							bool flag5 = this._currentSubPageIndex <= 2;
							if (flag5)
							{
								AudioManager.Instance.PlaySound("SFX_GearMate_machine_click", false, false);
							}
						}
					}
				}
			}
		}

		// Token: 0x06004A42 RID: 19010 RVA: 0x0022C7BB File Offset: 0x0022A9BB
		public void SetDisableClickActive(bool active)
		{
			this._disableClick.SetActive(active);
		}

		// Token: 0x06004A43 RID: 19011 RVA: 0x0022C7CC File Offset: 0x0022A9CC
		private void InitRefers()
		{
			this._mainWindow = base.CGet<RectTransform>("MainWindow");
			this._tabToggleLayout = base.CGet<HorizontalLayoutGroup>("TabToggleLayout");
			this._itemSelector = base.CGet<GearMateItemSelector>("ItemSelector");
			this._close = base.CGet<CButtonObsolete>("Close");
			this._subPageRoot = base.CGet<RectTransform>("SubPageRoot");
			this._gearMateAvatar = base.CGet<GearMateAvatar>("GearMateAvatar");
			this._title = base.CGet<TextMeshProUGUI>("Title");
			this._disableClick = base.CGet<GameObject>("DisableClick");
		}

		// Token: 0x1700094D RID: 2381
		// (get) Token: 0x06004A44 RID: 19012 RVA: 0x0022C862 File Offset: 0x0022AA62
		public CharacterDisplayData GearMateDisplayData
		{
			get
			{
				return this._gearMateDisplayData;
			}
		}

		// Token: 0x1700094E RID: 2382
		// (get) Token: 0x06004A45 RID: 19013 RVA: 0x0022C86A File Offset: 0x0022AA6A
		public GearMate GearMate
		{
			get
			{
				return this._gearMate;
			}
		}

		// Token: 0x1700094F RID: 2383
		// (get) Token: 0x06004A46 RID: 19014 RVA: 0x0022C872 File Offset: 0x0022AA72
		public int GearMateId
		{
			get
			{
				return this._gearMateId;
			}
		}

		// Token: 0x17000950 RID: 2384
		// (get) Token: 0x06004A47 RID: 19015 RVA: 0x0022C87A File Offset: 0x0022AA7A
		public GearMateAvatar Avatar
		{
			get
			{
				return this._gearMateAvatar;
			}
		}

		// Token: 0x17000951 RID: 2385
		// (get) Token: 0x06004A48 RID: 19016 RVA: 0x0022C882 File Offset: 0x0022AA82
		public GearMateItemSelector ItemSelector
		{
			get
			{
				return this._itemSelector;
			}
		}

		// Token: 0x17000952 RID: 2386
		// (get) Token: 0x06004A49 RID: 19017 RVA: 0x0022C88A File Offset: 0x0022AA8A
		public int CurrentSubPageIndex
		{
			get
			{
				return this._currentSubPageIndex;
			}
		}

		// Token: 0x17000953 RID: 2387
		// (get) Token: 0x06004A4A RID: 19018 RVA: 0x0022C892 File Offset: 0x0022AA92
		public GearMateSubPageBase CurrentLeafSubPage
		{
			get
			{
				return this._currentLeafSubPage;
			}
		}

		// Token: 0x06004A4B RID: 19019 RVA: 0x0022C89C File Offset: 0x0022AA9C
		public void OpenSelectGearMateView()
		{
			ArgumentBox argBox = EasyPool.Get<ArgumentBox>();
			IReadOnlyCollection<int> gearMates = SingletonObject.getInstance<CharacterMonitorModel>().GetTaiwuGearMateGroup();
			argBox.SetObject("charIdList", gearMates.ToList<int>());
			argBox.Set("selectedCharId", this._gearMateId);
			argBox.SetObject("callback", new Action<int>(this.OnSelectGearMate));
			UIElement.SelectCharLegacy.SetOnInitArgs(argBox);
			UIManager.Instance.ShowUI(UIElement.SelectCharLegacy, true);
		}

		// Token: 0x06004A4C RID: 19020 RVA: 0x0022C914 File Offset: 0x0022AB14
		private void OnSelectGearMate(int charId)
		{
			this._lastGearMateId = this._gearMateId;
			this._gearMateId = charId;
			bool flag = this._lastGearMateId != charId;
			if (flag)
			{
				this.RequestForRefreshGearMate();
				this._gearMateAvatar.HideBubble();
			}
		}

		// Token: 0x06004A4D RID: 19021 RVA: 0x0022C95C File Offset: 0x0022AB5C
		private void Update()
		{
			bool flag = CommonCommandKit.Space.Check(this.Element, false, false, false, true, false) && this.CurrentLeafSubPage.ConfirmButtonInteractable();
			if (flag)
			{
				bool flag2 = this._currentSubPageIndex <= 2;
				if (flag2)
				{
					this._itemSelector.Confirm();
				}
				else
				{
					this.CurrentLeafSubPage.ConfirmClick();
				}
			}
		}

		// Token: 0x06004A4E RID: 19022 RVA: 0x0022C9BF File Offset: 0x0022ABBF
		public void SetCurrentLeafSubPage(GearMateSubPageBase subPage)
		{
			this._currentLeafSubPage = subPage;
		}

		// Token: 0x06004A4F RID: 19023 RVA: 0x0022C9CC File Offset: 0x0022ABCC
		private void OnDisable()
		{
			this._currentLeafSubPage = null;
			this._currentSubPageIndex = -1;
			this._leafSubPages.ForEach(delegate(GearMateSubPageBase subPage)
			{
				subPage.UnMonitorFields();
			});
			for (int i = 0; i < UI_GearMate.PageEnabled.Length; i++)
			{
				UI_GearMate.PageEnabled[i] = false;
			}
		}

		// Token: 0x06004A50 RID: 19024 RVA: 0x0022CA33 File Offset: 0x0022AC33
		private void OnDestroy()
		{
			this._leafSubPages.ForEach(delegate(GearMateSubPageBase subPage)
			{
				subPage.UnRegisterListener();
			});
		}

		// Token: 0x0400336A RID: 13162
		private static readonly string[] PageToggleIcons = new string[]
		{
			"popup_gearmate_icon_attribute",
			"popup_gearmate_icon_characteristic",
			"popup_gearmate_icon_refined",
			"popup_gearmate_icon_neili",
			"popup_gearmate_icon_skill",
			"popup_gearmate_icon_martialarts"
		};

		// Token: 0x0400336B RID: 13163
		private static readonly bool[] PageEnabled = new bool[6];

		// Token: 0x0400336C RID: 13164
		private RectTransform _mainWindow;

		// Token: 0x0400336D RID: 13165
		private HorizontalLayoutGroup _tabToggleLayout;

		// Token: 0x0400336E RID: 13166
		private GearMateItemSelector _itemSelector;

		// Token: 0x0400336F RID: 13167
		private CButtonObsolete _close;

		// Token: 0x04003370 RID: 13168
		private RectTransform _subPageRoot;

		// Token: 0x04003371 RID: 13169
		private GearMateAvatar _gearMateAvatar;

		// Token: 0x04003372 RID: 13170
		private TextMeshProUGUI _title;

		// Token: 0x04003373 RID: 13171
		private GameObject _disableClick;

		// Token: 0x04003374 RID: 13172
		private int _targetSubPageIndex = 0;

		// Token: 0x04003375 RID: 13173
		private bool _inited;

		// Token: 0x04003376 RID: 13174
		private int _gearMateId;

		// Token: 0x04003377 RID: 13175
		private CharacterDisplayData _gearMateDisplayData;

		// Token: 0x04003378 RID: 13176
		private GearMate _gearMate;

		// Token: 0x04003379 RID: 13177
		private int _lastGearMateId = -1;

		// Token: 0x0400337A RID: 13178
		private int _currentSubPageIndex = -1;

		// Token: 0x0400337B RID: 13179
		private List<GearMateSubPageBase> _subPages = new List<GearMateSubPageBase>();

		// Token: 0x0400337C RID: 13180
		private readonly List<GearMateSubPageBase> _leafSubPages = new List<GearMateSubPageBase>();

		// Token: 0x0400337D RID: 13181
		private GearMateSubPageBase _currentLeafSubPage = null;

		// Token: 0x0400337E RID: 13182
		private List<CButtonObsolete> _tabButtons = new List<CButtonObsolete>();
	}
}
