using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Runtime.CompilerServices;
using FrameWork.UISystem.UIElements;
using Game.Components.Common;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace Game.Views.CharacterMenu
{
	// Token: 0x02000B6B RID: 2923
	[DisallowMultipleComponent]
	public class CharacterMenuToggleGroup : MonoBehaviour
	{
		// Token: 0x0600909D RID: 37021 RVA: 0x004363DC File Offset: 0x004345DC
		private void OnEnable()
		{
			bool initialized = this._initialized;
			if (initialized)
			{
				this.RefreshToggleLabels();
			}
		}

		// Token: 0x0600909E RID: 37022 RVA: 0x004363FC File Offset: 0x004345FC
		private void Awake()
		{
			bool flag = this.dropdownRoot != null;
			if (flag)
			{
				this._cachedCanvas = this.dropdownRoot.GetComponentInParent<Canvas>();
				this.dropdownRoot.gameObject.SetActive(false);
			}
			bool flag2 = this.dropdownPointerTrigger != null;
			if (flag2)
			{
				PointerTrigger pointerTrigger = this.dropdownPointerTrigger;
				if (pointerTrigger.EnterEvent == null)
				{
					pointerTrigger.EnterEvent = new UnityEvent();
				}
				pointerTrigger = this.dropdownPointerTrigger;
				if (pointerTrigger.ExitEvent == null)
				{
					pointerTrigger.ExitEvent = new UnityEvent();
				}
				this.dropdownPointerTrigger.EnterEvent.AddListener(delegate()
				{
					this._dropdownPointerInside = true;
				});
				this.dropdownPointerTrigger.ExitEvent.AddListener(delegate()
				{
					this._dropdownPointerInside = false;
					this.TryHideDropdown();
				});
			}
		}

		// Token: 0x0600909F RID: 37023 RVA: 0x004364C0 File Offset: 0x004346C0
		public void Initialize(ViewCharacterMenu.SubTogglePageInfo[] subToggleInfos, int defaultIndex, Action<ECharacterSubToggleBase> onParentSelected, Action<ECharacterSubToggleBase, ECharacterSubPage> onSubPageSelected, Func<ECharacterSubPage, bool> subPageFilter = null)
		{
			bool flag = this.toggleGroup == null;
			if (flag)
			{
				throw new InvalidOperationException("CharacterMenuTabDropdown requires a CToggleGroup reference.");
			}
			this._onParentSelected = onParentSelected;
			this._onSubPageSelected = onSubPageSelected;
			this._subPageFilter = subPageFilter;
			bool initialized = this._initialized;
			if (initialized)
			{
				this.RefreshToggleLabels();
			}
			else
			{
				this.toggleGroup.Init(defaultIndex);
				ToggleGroupHotkeyController.Set(UIElement.CharacterMenu, this.toggleGroup, 0, null);
				this.toggleGroup.OnActiveIndexChange += this.OnToggeleGroupActiveIndexChange;
				this._tabLookup.Clear();
				bool flag2 = subToggleInfos == null;
				if (flag2)
				{
					this._initialized = true;
				}
				else
				{
					for (int i = 0; i < subToggleInfos.Length; i++)
					{
						ViewCharacterMenu.SubTogglePageInfo info = subToggleInfos[i];
						CToggle toggle = this.toggleGroup.Get((int)info.SubToggleType);
						bool flag3 = toggle == null;
						if (!flag3)
						{
							CharacterMenuToggleGroup.TabRuntime runtime = new CharacterMenuToggleGroup.TabRuntime
							{
								Info = info,
								Toggle = toggle,
								PointerTrigger = CharacterMenuToggleGroup.EnsurePointerTrigger(toggle)
							};
							this._tabLookup[info.SubToggleType] = runtime;
							LanguageKey titleKey = (info.SubPageNames != null && info.SubPageNames.Length != 0) ? info.SubPageNames[0] : info.TitleKey;
							string displayName = LocalStringManager.Get(titleKey);
							bool flag4 = !string.IsNullOrEmpty(displayName);
							if (flag4)
							{
								ToggleStyle toggleStyle = toggle.GetComponent<ToggleStyle>();
								if (toggleStyle != null)
								{
									toggleStyle.SetLabelText(displayName);
								}
							}
							PointerTrigger pointerTrigger = runtime.PointerTrigger;
							if (pointerTrigger.EnterEvent == null)
							{
								pointerTrigger.EnterEvent = new UnityEvent();
							}
							pointerTrigger = runtime.PointerTrigger;
							if (pointerTrigger.ExitEvent == null)
							{
								pointerTrigger.ExitEvent = new UnityEvent();
							}
							runtime.PointerTrigger.EnterEvent.AddListener(delegate()
							{
								this.OnTogglePointerEnter(runtime);
							});
							runtime.PointerTrigger.ExitEvent.AddListener(delegate()
							{
								this.OnTogglePointerExit(runtime);
							});
						}
					}
					this._initialized = true;
				}
			}
		}

		// Token: 0x060090A0 RID: 37024 RVA: 0x004366FC File Offset: 0x004348FC
		private void RefreshToggleLabels()
		{
			foreach (CharacterMenuToggleGroup.TabRuntime runtime in this._tabLookup.Values)
			{
				ViewCharacterMenu.SubTogglePageInfo info = runtime.Info;
				bool flag = runtime.Toggle == null;
				if (!flag)
				{
					int index = runtime.LastSubPageIndex;
					bool flag2 = info.SubPageNames == null || index < 0 || index >= info.SubPageNames.Length;
					if (flag2)
					{
						index = 0;
					}
					int visibleCount = 0;
					bool flag3 = info.SubPages != null;
					if (flag3)
					{
						for (int i = 0; i < info.SubPages.Length; i++)
						{
							bool flag4 = this._subPageFilter == null || this._subPageFilter(info.SubPages[i]);
							if (flag4)
							{
								visibleCount++;
							}
						}
					}
					bool flag5 = visibleCount > 1 && info.SubPageNames != null && index < info.SubPageNames.Length;
					LanguageKey targetKey;
					if (flag5)
					{
						targetKey = info.SubPageNames[index];
					}
					else
					{
						targetKey = ((info.SubPageNames != null && info.SubPageNames.Length != 0) ? info.SubPageNames[0] : info.TitleKey);
					}
					string displayName = LocalStringManager.Get(targetKey);
					bool flag6 = !string.IsNullOrEmpty(displayName);
					if (flag6)
					{
						ToggleStyle toggleStyle = runtime.Toggle.GetComponent<ToggleStyle>();
						if (toggleStyle != null)
						{
							toggleStyle.SetLabelText(displayName);
						}
					}
				}
			}
		}

		// Token: 0x060090A1 RID: 37025 RVA: 0x00436894 File Offset: 0x00434A94
		private void OnToggeleGroupActiveIndexChange(int newIndex, int oldIndex)
		{
			bool suppressParentCallback = this._suppressParentCallback;
			if (!suppressParentCallback)
			{
				Action<ECharacterSubToggleBase> onParentSelected = this._onParentSelected;
				if (onParentSelected != null)
				{
					onParentSelected((ECharacterSubToggleBase)newIndex);
				}
			}
		}

		// Token: 0x060090A2 RID: 37026 RVA: 0x004368C4 File Offset: 0x00434AC4
		public void SetParent(ECharacterSubToggleBase key, bool notify = true)
		{
			bool flag = this.toggleGroup == null;
			if (!flag)
			{
				this._suppressParentCallback = !notify;
				if (notify)
				{
					this.toggleGroup.Set((int)key, false);
				}
				else
				{
					this.toggleGroup.SetWithoutNotify((int)key);
				}
				this._suppressParentCallback = false;
			}
		}

		// Token: 0x060090A3 RID: 37027 RVA: 0x00436918 File Offset: 0x00434B18
		public void SyncCurrentSubPage(ECharacterSubToggleBase parentKey, ECharacterSubPage subPage)
		{
			CharacterMenuToggleGroup.TabRuntime runtime;
			bool flag = !this._tabLookup.TryGetValue(parentKey, out runtime);
			if (!flag)
			{
				this.SetParent(parentKey, false);
				ViewCharacterMenu.SubTogglePageInfo info = runtime.Info;
				ToggleStyle toggleStyle;
				bool flag2 = runtime.Toggle.TryGetComponent<ToggleStyle>(out toggleStyle);
				if (flag2)
				{
					int originalIndex = (info.SubPages != null) ? Array.IndexOf<ECharacterSubPage>(info.SubPages, subPage) : -1;
					runtime.LastSubPageIndex = ((originalIndex >= 0) ? originalIndex : 0);
					int visibleCount = 0;
					bool flag3 = info.SubPages != null;
					if (flag3)
					{
						for (int i = 0; i < info.SubPages.Length; i++)
						{
							bool flag4 = this._subPageFilter == null || this._subPageFilter(info.SubPages[i]);
							if (flag4)
							{
								visibleCount++;
							}
						}
					}
					bool flag5 = visibleCount > 1 && originalIndex >= 0 && originalIndex < info.SubPageNames.Length;
					if (flag5)
					{
						toggleStyle.SetLabelText(LocalStringManager.Get(info.SubPageNames[originalIndex]));
					}
					else
					{
						LanguageKey titleKey = (info.SubPageNames != null && info.SubPageNames.Length != 0) ? info.SubPageNames[0] : info.TitleKey;
						toggleStyle.SetLabelText(LocalStringManager.Get(titleKey));
					}
				}
			}
		}

		// Token: 0x060090A4 RID: 37028 RVA: 0x00436A5C File Offset: 0x00434C5C
		public void SetParentInteractable(ECharacterSubToggleBase key, bool interactable)
		{
			CharacterMenuToggleGroup.TabRuntime runtime;
			bool flag = this._tabLookup.TryGetValue(key, out runtime);
			if (flag)
			{
				runtime.Toggle.interactable = interactable;
				ToggleStyle toggleStyle;
				bool flag2 = runtime.Toggle.TryGetComponent<ToggleStyle>(out toggleStyle);
				if (flag2)
				{
					toggleStyle.RefreshInteractableRelatedDisplay();
				}
			}
		}

		// Token: 0x060090A5 RID: 37029 RVA: 0x00436AA4 File Offset: 0x00434CA4
		public CToggle GetToggle(ECharacterSubToggleBase key)
		{
			CharacterMenuToggleGroup.TabRuntime runtime;
			return this._tabLookup.TryGetValue(key, out runtime) ? runtime.Toggle : null;
		}

		// Token: 0x17000FB8 RID: 4024
		// (get) Token: 0x060090A6 RID: 37030 RVA: 0x00436AD0 File Offset: 0x00434CD0
		public ECharacterSubToggleBase CurrentParentKey
		{
			get
			{
				bool flag = this.toggleGroup == null;
				ECharacterSubToggleBase result;
				if (flag)
				{
					result = ECharacterSubToggleBase.None;
				}
				else
				{
					int index = this.toggleGroup.GetActiveIndex();
					result = (ECharacterSubToggleBase)((index < 0) ? -1 : index);
				}
				return result;
			}
		}

		// Token: 0x060090A7 RID: 37031 RVA: 0x00436B0C File Offset: 0x00434D0C
		public void HideDropdownImmediate()
		{
			bool flag = this.dropdownRoot != null;
			if (flag)
			{
				this.dropdownRoot.gameObject.SetActive(false);
			}
			this._hoveredTab = null;
			this._dropdownPointerInside = false;
		}

		// Token: 0x060090A8 RID: 37032 RVA: 0x00436B4C File Offset: 0x00434D4C
		private static PointerTrigger EnsurePointerTrigger(CToggle toggle)
		{
			PointerTrigger trigger = toggle.GetComponent<PointerTrigger>();
			trigger.Selectable = toggle;
			trigger.Toggle = null;
			trigger.OnlyResponseWhenToggleEnabled = true;
			trigger.IgnoreOnDisableTrigger = true;
			return trigger;
		}

		// Token: 0x060090A9 RID: 37033 RVA: 0x00436B84 File Offset: 0x00434D84
		private void OnTogglePointerEnter(CharacterMenuToggleGroup.TabRuntime runtime)
		{
			runtime.PointerInside = true;
			this._hoveredTab = runtime;
			bool flag = !this.RefreshDropdownEntries(runtime);
			if (flag)
			{
				RectTransform rectTransform = this.dropdownRoot;
				if (rectTransform != null)
				{
					rectTransform.gameObject.SetActive(false);
				}
			}
			else
			{
				this.AlignDropdownWith(runtime.Toggle.transform as RectTransform);
				Vector2 anchoredPos = this.dropdownRoot.anchoredPosition;
				float tempWidth = this.dropdownButtonPrefab.GetComponent<RectTransform>().rect.width;
				float panelHalfWidth = ((float)this._currentDropdownEntries.Count * tempWidth + (float)(this._currentDropdownEntries.Count - 1) * 2f) * 0.5f;
				float groupHalfWidth = base.transform.GetComponent<RectTransform>().rect.width * 0.5f;
				bool flag2 = anchoredPos.x > 0f && anchoredPos.x + panelHalfWidth >= groupHalfWidth;
				if (flag2)
				{
					anchoredPos.x = groupHalfWidth - panelHalfWidth;
					this.dropdownRoot.anchoredPosition = anchoredPos;
				}
				bool flag3 = anchoredPos.x < 0f && anchoredPos.x - panelHalfWidth <= -1f * groupHalfWidth;
				if (flag3)
				{
					anchoredPos.x = -1f * groupHalfWidth + panelHalfWidth;
					this.dropdownRoot.anchoredPosition = anchoredPos;
				}
				RectTransform rectTransform2 = this.dropdownRoot;
				if (rectTransform2 != null)
				{
					rectTransform2.gameObject.SetActive(true);
				}
			}
		}

		// Token: 0x060090AA RID: 37034 RVA: 0x00436CF4 File Offset: 0x00434EF4
		private void OnTogglePointerExit(CharacterMenuToggleGroup.TabRuntime runtime)
		{
			runtime.PointerInside = false;
			bool flag = this._hoveredTab == runtime;
			if (flag)
			{
				base.StartCoroutine(this.DelayTryHideDropdown());
			}
		}

		// Token: 0x060090AB RID: 37035 RVA: 0x00436D25 File Offset: 0x00434F25
		private IEnumerator DelayTryHideDropdown()
		{
			yield return null;
			this.TryHideDropdown();
			yield break;
		}

		// Token: 0x060090AC RID: 37036 RVA: 0x00436D34 File Offset: 0x00434F34
		private bool RefreshDropdownEntries(CharacterMenuToggleGroup.TabRuntime runtime)
		{
			bool flag = this.dropdownRoot == null || this.dropdownButtonPrefab == null || this.dropdownButtonContainer == null;
			bool result;
			if (flag)
			{
				result = false;
			}
			else
			{
				ViewCharacterMenu.SubTogglePageInfo info = runtime.Info;
				bool flag2 = info.SubPages == null || info.SubPages.Length <= 1;
				if (flag2)
				{
					this._currentDropdownEntries.Clear();
					result = false;
				}
				else
				{
					this._currentDropdownEntries.Clear();
					for (int i = 0; i < info.SubPages.Length; i++)
					{
						ECharacterSubPage subPage = info.SubPages[i];
						bool flag3 = this._subPageFilter != null && !this._subPageFilter(subPage);
						if (!flag3)
						{
							string displayName = LocalStringManager.Get(info.SubPageNames[i]);
							this._currentDropdownEntries.Add(new ValueTuple<string, ECharacterSubPage>(displayName, subPage));
						}
					}
					bool flag4 = this._currentDropdownEntries.Count <= 1;
					if (flag4)
					{
						this._currentDropdownEntries.Clear();
						result = false;
					}
					else
					{
						CommonUtils.PrepareEnoughChildren(this.dropdownButtonContainer, this.dropdownButtonPrefab.gameObject, this._currentDropdownEntries.Count, null);
						while (this._buttonPool.Count < this.dropdownButtonContainer.childCount)
						{
							Transform child = this.dropdownButtonContainer.GetChild(this._buttonPool.Count);
							CButton button = child.GetComponent<CButton>();
							TextMeshProUGUI label = button.GetComponentInChildren<TextMeshProUGUI>(true);
							this._buttonPool.Add(new CharacterMenuToggleGroup.ButtonEntry
							{
								Button = button,
								Label = label
							});
						}
						int j = 0;
						while (j < this._currentDropdownEntries.Count)
						{
							int index = j;
							CharacterMenuToggleGroup.ButtonEntry entry = this._buttonPool[j];
							ValueTuple<string, ECharacterSubPage> valueTuple = this._currentDropdownEntries[j];
							string displayName2 = valueTuple.Item1;
							ECharacterSubPage eSubPage = valueTuple.Item2;
							bool flag5 = entry.Label != null;
							if (flag5)
							{
								entry.Label.text = displayName2;
							}
							bool flag6 = entry.Button != null;
							if (flag6)
							{
								string spriteRes = "";
								switch (eSubPage)
								{
								case ECharacterSubPage.None:
									Debug.LogError(string.Format("Can't Find Execute Target SubPage: {0} {1}", eSubPage, displayName2));
									goto IL_37F;
								case ECharacterSubPage.Character:
									spriteRes = "ui9_btn_character_{0}_{1}";
									break;
								case ECharacterSubPage.Team:
									spriteRes = "ui9_btn_team_{0}_{1}";
									break;
								case ECharacterSubPage.Prison:
									spriteRes = "ui9_btn_detain_{0}_{1}";
									break;
								case ECharacterSubPage.Attainment:
									spriteRes = "ui9_btn_mastery_{0}_{1}";
									break;
								case ECharacterSubPage.AttainmentCombatSkill:
									spriteRes = "ui9_btn_skill_{0}_{1}";
									break;
								case ECharacterSubPage.AttainmentLifeSkill:
									spriteRes = "ui9_btn_artistry_{0}_{1}";
									break;
								case ECharacterSubPage.Relationship:
									spriteRes = "ui9_btn_relation_{0}_{1}";
									break;
								case ECharacterSubPage.Genealogy:
									spriteRes = "ui9_btn_pedigree_{0}_{1}";
									break;
								case ECharacterSubPage.Information:
									spriteRes = "ui9_btn_experience_{0}_{1}";
									break;
								case ECharacterSubPage.Secret:
									spriteRes = "ui9_btn_secret_{0}_{1}";
									break;
								case ECharacterSubPage.Equipment:
									spriteRes = "ui9_btn_equipment_{0}_{1}";
									break;
								case ECharacterSubPage.Vehicle:
									spriteRes = "ui9_btn_vehicle_{0}_{1}";
									break;
								}
								string resourcePath = Path.Combine("RemakeResources/UIGraphics5.0/Ui9CharacterMenu/", spriteRes);
								this.LoadDropdownEntryButtonSprite(entry.Button, resourcePath, index == runtime.LastSubPageIndex);
								goto IL_364;
							}
							goto IL_364;
							IL_37F:
							j++;
							continue;
							IL_364:
							entry.Button.ClearAndAddListener(delegate
							{
								this.OnDropdownButtonClicked(runtime, index);
							});
							goto IL_37F;
						}
						result = true;
					}
				}
			}
			return result;
		}

		// Token: 0x060090AD RID: 37037 RVA: 0x004370E4 File Offset: 0x004352E4
		private void LoadDropdownEntryButtonSprite(CButton btn, string path, bool isCurrent)
		{
			CImage btnImg = btn.GetComponent<CImage>();
			SpriteState spriteState = default(SpriteState);
			ResLoader.Load<Sprite>(string.Format(path, SingletonObject.getInstance<GlobalSettings>().Language.ToLower(), isCurrent ? 2 : 0), delegate(Sprite normalSprite)
			{
				btnImg.sprite = normalSprite;
			}, null, false);
			Action<Sprite> <>9__3;
			Action<Sprite> <>9__2;
			ResLoader.Load<Sprite>(string.Format(path, SingletonObject.getInstance<GlobalSettings>().Language.ToLower(), isCurrent ? 2 : 1), delegate(Sprite selectSprite)
			{
				spriteState.highlightedSprite = selectSprite;
				spriteState.selectedSprite = selectSprite;
				string assetPath = string.Format(path, SingletonObject.getInstance<GlobalSettings>().Language.ToLower(), 0);
				Action<Sprite> onLoad;
				if ((onLoad = <>9__2) == null)
				{
					onLoad = (<>9__2 = delegate(Sprite pressSprite)
					{
						spriteState.pressedSprite = pressSprite;
						string assetPath2 = string.Format(path, SingletonObject.getInstance<GlobalSettings>().Language.ToLower(), 3);
						Action<Sprite> onLoad2;
						if ((onLoad2 = <>9__3) == null)
						{
							onLoad2 = (<>9__3 = delegate(Sprite disableSprite)
							{
								spriteState.disabledSprite = disableSprite;
								btn.spriteState = spriteState;
							});
						}
						ResLoader.Load<Sprite>(assetPath2, onLoad2, null, false);
					});
				}
				ResLoader.Load<Sprite>(assetPath, onLoad, null, false);
			}, null, false);
		}

		// Token: 0x060090AE RID: 37038 RVA: 0x00437198 File Offset: 0x00435398
		private void OnDropdownButtonClicked(CharacterMenuToggleGroup.TabRuntime runtime, int index)
		{
			bool flag = !this._currentDropdownEntries.CheckIndex(index);
			if (!flag)
			{
				ValueTuple<string, ECharacterSubPage> valueTuple = this._currentDropdownEntries[index];
				string displayName = valueTuple.Item1;
				ECharacterSubPage page = valueTuple.Item2;
				ToggleStyle toggleStyle = runtime.Toggle.GetComponent<ToggleStyle>();
				if (toggleStyle != null)
				{
					toggleStyle.SetLabelText(displayName);
				}
				this.SetParent(runtime.Info.SubToggleType, false);
				runtime.LastSubPageIndex = index;
				Action<ECharacterSubToggleBase, ECharacterSubPage> onSubPageSelected = this._onSubPageSelected;
				if (onSubPageSelected != null)
				{
					onSubPageSelected(runtime.Info.SubToggleType, page);
				}
				this.HideDropdownImmediate();
			}
		}

		// Token: 0x060090AF RID: 37039 RVA: 0x0043722C File Offset: 0x0043542C
		private void AlignDropdownWith(RectTransform toggleRect)
		{
			bool flag = this.dropdownRoot == null || toggleRect == null;
			if (!flag)
			{
				toggleRect.GetWorldCorners(this._toggleCorners);
				Vector3 bottomLeft = this._toggleCorners[0];
				Vector3 bottomRight = this._toggleCorners[3];
				RectTransform parentRect = this.dropdownRoot.parent as RectTransform;
				bool flag2 = parentRect == null;
				if (!flag2)
				{
					Camera camera = (this._cachedCanvas != null && this._cachedCanvas.renderMode != RenderMode.ScreenSpaceOverlay) ? this._cachedCanvas.worldCamera : null;
					Vector2 localBottomLeft;
					RectTransformUtility.ScreenPointToLocalPointInRectangle(parentRect, RectTransformUtility.WorldToScreenPoint(camera, bottomLeft), camera, out localBottomLeft);
					Vector2 localBottomRight;
					RectTransformUtility.ScreenPointToLocalPointInRectangle(parentRect, RectTransformUtility.WorldToScreenPoint(camera, bottomRight), camera, out localBottomRight);
					Vector2 anchored = this.dropdownRoot.anchoredPosition;
					anchored.x = (localBottomLeft.x + localBottomRight.x) * 0.5f;
					this.dropdownRoot.anchoredPosition = anchored;
					this.dropdownRoot.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, Mathf.Abs(localBottomRight.x - localBottomLeft.x));
				}
			}
		}

		// Token: 0x060090B0 RID: 37040 RVA: 0x0043734C File Offset: 0x0043554C
		private void TryHideDropdown()
		{
			bool dropdownPointerInside = this._dropdownPointerInside;
			if (!dropdownPointerInside)
			{
				bool flag = this._hoveredTab != null && this._hoveredTab.PointerInside;
				if (!flag)
				{
					RectTransform rectTransform = this.dropdownRoot;
					if (rectTransform != null)
					{
						rectTransform.gameObject.SetActive(false);
					}
					this._hoveredTab = null;
				}
			}
		}

		// Token: 0x04006F53 RID: 28499
		[SerializeField]
		private CToggleGroup toggleGroup;

		// Token: 0x04006F54 RID: 28500
		[SerializeField]
		private RectTransform dropdownRoot;

		// Token: 0x04006F55 RID: 28501
		[SerializeField]
		private Transform dropdownButtonContainer;

		// Token: 0x04006F56 RID: 28502
		[SerializeField]
		private CButton dropdownButtonPrefab;

		// Token: 0x04006F57 RID: 28503
		[SerializeField]
		private PointerTrigger dropdownPointerTrigger;

		// Token: 0x04006F58 RID: 28504
		private readonly Dictionary<ECharacterSubToggleBase, CharacterMenuToggleGroup.TabRuntime> _tabLookup = new Dictionary<ECharacterSubToggleBase, CharacterMenuToggleGroup.TabRuntime>();

		// Token: 0x04006F59 RID: 28505
		private readonly List<CharacterMenuToggleGroup.ButtonEntry> _buttonPool = new List<CharacterMenuToggleGroup.ButtonEntry>();

		// Token: 0x04006F5A RID: 28506
		[TupleElementNames(new string[]
		{
			"displayName",
			"page"
		})]
		private readonly List<ValueTuple<string, ECharacterSubPage>> _currentDropdownEntries = new List<ValueTuple<string, ECharacterSubPage>>();

		// Token: 0x04006F5B RID: 28507
		private readonly Vector3[] _toggleCorners = new Vector3[4];

		// Token: 0x04006F5C RID: 28508
		private Action<ECharacterSubToggleBase> _onParentSelected;

		// Token: 0x04006F5D RID: 28509
		private Action<ECharacterSubToggleBase, ECharacterSubPage> _onSubPageSelected;

		// Token: 0x04006F5E RID: 28510
		private Func<ECharacterSubPage, bool> _subPageFilter;

		// Token: 0x04006F5F RID: 28511
		private Canvas _cachedCanvas;

		// Token: 0x04006F60 RID: 28512
		private CharacterMenuToggleGroup.TabRuntime _hoveredTab;

		// Token: 0x04006F61 RID: 28513
		private bool _dropdownPointerInside;

		// Token: 0x04006F62 RID: 28514
		private bool _suppressParentCallback;

		// Token: 0x04006F63 RID: 28515
		private bool _initialized;

		// Token: 0x0200217C RID: 8572
		private sealed class TabRuntime
		{
			// Token: 0x0400D602 RID: 54786
			public ViewCharacterMenu.SubTogglePageInfo Info;

			// Token: 0x0400D603 RID: 54787
			public CToggle Toggle;

			// Token: 0x0400D604 RID: 54788
			public PointerTrigger PointerTrigger;

			// Token: 0x0400D605 RID: 54789
			public bool PointerInside;

			// Token: 0x0400D606 RID: 54790
			public int LastSubPageIndex;
		}

		// Token: 0x0200217D RID: 8573
		private sealed class ButtonEntry
		{
			// Token: 0x0400D607 RID: 54791
			public CButton Button;

			// Token: 0x0400D608 RID: 54792
			public TextMeshProUGUI Label;
		}
	}
}
