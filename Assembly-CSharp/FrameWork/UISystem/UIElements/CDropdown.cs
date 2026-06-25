using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace FrameWork.UISystem.UIElements
{
	// Token: 0x02001004 RID: 4100
	[RequireComponent(typeof(RectTransform))]
	[RequireComponent(typeof(UIInteractionBehaviour))]
	public class CDropdown : Selectable, IPointerClickHandler, IEventSystemHandler, ISubmitHandler, ICancelHandler
	{
		// Token: 0x1700150B RID: 5387
		// (get) Token: 0x0600BAEC RID: 47852 RVA: 0x005515B8 File Offset: 0x0054F7B8
		// (set) Token: 0x0600BAED RID: 47853 RVA: 0x005515D0 File Offset: 0x0054F7D0
		public RectTransform template
		{
			get
			{
				return this.m_Template;
			}
			set
			{
				this.m_Template = value;
				this.RefreshShownValue();
			}
		}

		// Token: 0x1700150C RID: 5388
		// (get) Token: 0x0600BAEE RID: 47854 RVA: 0x005515E4 File Offset: 0x0054F7E4
		// (set) Token: 0x0600BAEF RID: 47855 RVA: 0x005515FC File Offset: 0x0054F7FC
		public TMP_Text captionText
		{
			get
			{
				return this.m_CaptionText;
			}
			set
			{
				this.m_CaptionText = value;
				this.RefreshShownValue();
			}
		}

		// Token: 0x1700150D RID: 5389
		// (get) Token: 0x0600BAF0 RID: 47856 RVA: 0x00551610 File Offset: 0x0054F810
		// (set) Token: 0x0600BAF1 RID: 47857 RVA: 0x00551628 File Offset: 0x0054F828
		public Image captionImage
		{
			get
			{
				return this.m_CaptionImage;
			}
			set
			{
				this.m_CaptionImage = value;
				this.RefreshShownValue();
			}
		}

		// Token: 0x1700150E RID: 5390
		// (get) Token: 0x0600BAF2 RID: 47858 RVA: 0x0055163C File Offset: 0x0054F83C
		// (set) Token: 0x0600BAF3 RID: 47859 RVA: 0x00551654 File Offset: 0x0054F854
		public Graphic placeholder
		{
			get
			{
				return this.m_Placeholder;
			}
			set
			{
				this.m_Placeholder = value;
				this.RefreshShownValue();
			}
		}

		// Token: 0x1700150F RID: 5391
		// (get) Token: 0x0600BAF4 RID: 47860 RVA: 0x00551668 File Offset: 0x0054F868
		// (set) Token: 0x0600BAF5 RID: 47861 RVA: 0x00551680 File Offset: 0x0054F880
		public TMP_Text itemText
		{
			get
			{
				return this.m_ItemText;
			}
			set
			{
				this.m_ItemText = value;
				this.RefreshShownValue();
			}
		}

		// Token: 0x17001510 RID: 5392
		// (get) Token: 0x0600BAF6 RID: 47862 RVA: 0x00551694 File Offset: 0x0054F894
		// (set) Token: 0x0600BAF7 RID: 47863 RVA: 0x005516AC File Offset: 0x0054F8AC
		public Image itemImage
		{
			get
			{
				return this.m_ItemImage;
			}
			set
			{
				this.m_ItemImage = value;
				this.RefreshShownValue();
			}
		}

		// Token: 0x17001511 RID: 5393
		// (get) Token: 0x0600BAF8 RID: 47864 RVA: 0x005516C0 File Offset: 0x0054F8C0
		// (set) Token: 0x0600BAF9 RID: 47865 RVA: 0x005516DD File Offset: 0x0054F8DD
		public List<CDropdown.OptionData> options
		{
			get
			{
				return this.m_Options.options;
			}
			set
			{
				this.m_Options.options = value;
				this.RefreshShownValue();
			}
		}

		// Token: 0x17001512 RID: 5394
		// (get) Token: 0x0600BAFA RID: 47866 RVA: 0x005516F4 File Offset: 0x0054F8F4
		// (set) Token: 0x0600BAFB RID: 47867 RVA: 0x0055170C File Offset: 0x0054F90C
		public CDropdown.DropdownEvent onValueChanged
		{
			get
			{
				return this.m_OnValueChanged;
			}
			set
			{
				this.m_OnValueChanged = value;
			}
		}

		// Token: 0x17001513 RID: 5395
		// (get) Token: 0x0600BAFC RID: 47868 RVA: 0x00551718 File Offset: 0x0054F918
		// (set) Token: 0x0600BAFD RID: 47869 RVA: 0x00551730 File Offset: 0x0054F930
		public CDropdown.DropdownEvent onSelect
		{
			get
			{
				return this.m_OnSelect;
			}
			set
			{
				this.m_OnSelect = value;
			}
		}

		// Token: 0x17001514 RID: 5396
		// (get) Token: 0x0600BAFE RID: 47870 RVA: 0x0055173C File Offset: 0x0054F93C
		// (set) Token: 0x0600BAFF RID: 47871 RVA: 0x00551754 File Offset: 0x0054F954
		public float alphaFadeSpeed
		{
			get
			{
				return this.m_AlphaFadeSpeed;
			}
			set
			{
				this.m_AlphaFadeSpeed = value;
			}
		}

		// Token: 0x17001515 RID: 5397
		// (get) Token: 0x0600BB00 RID: 47872 RVA: 0x00551760 File Offset: 0x0054F960
		// (set) Token: 0x0600BB01 RID: 47873 RVA: 0x00551778 File Offset: 0x0054F978
		public int value
		{
			get
			{
				return this.m_Value;
			}
			set
			{
				this.SetValue(value, true);
			}
		}

		// Token: 0x0600BB02 RID: 47874 RVA: 0x00551784 File Offset: 0x0054F984
		public void SetValueWithoutNotify(int input)
		{
			this.SetValue(input, false);
		}

		// Token: 0x0600BB03 RID: 47875 RVA: 0x00551790 File Offset: 0x0054F990
		private void SetValue(int value, bool sendCallback = true)
		{
			bool flag = Application.isPlaying && this.options.Count == 0;
			if (!flag)
			{
				int previousValue = this.m_Value;
				this.m_Value = Mathf.Clamp(value, this.m_Placeholder ? -1 : 0, this.options.Count - 1);
				this.RefreshShownValue();
				if (sendCallback)
				{
					this.m_OnSelect.Invoke(this.m_Value);
					bool flag2 = previousValue != this.m_Value;
					if (flag2)
					{
						this.m_OnValueChanged.Invoke(this.m_Value);
					}
				}
			}
		}

		// Token: 0x17001516 RID: 5398
		// (get) Token: 0x0600BB04 RID: 47876 RVA: 0x00551834 File Offset: 0x0054FA34
		public bool IsExpanded
		{
			get
			{
				return this.m_Dropdown != null;
			}
		}

		// Token: 0x0600BB05 RID: 47877 RVA: 0x00551854 File Offset: 0x0054FA54
		protected CDropdown()
		{
		}

		// Token: 0x0600BB06 RID: 47878 RVA: 0x005518BC File Offset: 0x0054FABC
		protected override void Awake()
		{
			base.Awake();
			this.m_AlphaTweenRunner = new TweenRunner<FloatTween>();
			this.m_AlphaTweenRunner.Init(this);
			bool flag = this.m_CaptionImage;
			if (flag)
			{
				this.m_CaptionImage.enabled = (this.m_CaptionImage.sprite != null);
			}
			bool flag2 = this.m_Template;
			if (flag2)
			{
				this.m_Template.gameObject.SetActive(false);
			}
		}

		// Token: 0x0600BB07 RID: 47879 RVA: 0x00551936 File Offset: 0x0054FB36
		protected override void Start()
		{
			base.Start();
			this.RefreshShownValue();
		}

		// Token: 0x0600BB08 RID: 47880 RVA: 0x00551948 File Offset: 0x0054FB48
		protected override void OnDisable()
		{
			this.ImmediateDestroyDropdownList();
			bool flag = this.m_Blocker != null;
			if (flag)
			{
				this.DestroyBlocker(this.m_Blocker);
			}
			this.m_Blocker = null;
			base.OnDisable();
		}

		// Token: 0x0600BB09 RID: 47881 RVA: 0x00551988 File Offset: 0x0054FB88
		public override void OnPointerDown(PointerEventData eventData)
		{
			base.OnPointerDown(eventData);
			base.GetComponent<UIInteractionBehaviour>().Play(base.interactable);
		}

		// Token: 0x0600BB0A RID: 47882 RVA: 0x005519A8 File Offset: 0x0054FBA8
		public void RefreshShownValue()
		{
			CDropdown.OptionData data = CDropdown.s_NoOptionData;
			bool flag = this.options.Count > 0 && this.m_Value >= 0;
			if (flag)
			{
				data = this.options[Mathf.Clamp(this.m_Value, 0, this.options.Count - 1)];
			}
			bool flag2 = this.m_CaptionText;
			if (flag2)
			{
				bool flag3 = data != null && data.text != null;
				if (flag3)
				{
					this.m_CaptionText.text = data.text;
				}
				else
				{
					this.m_CaptionText.text = "";
				}
			}
			bool flag4 = this.m_CaptionImage;
			if (flag4)
			{
				bool flag5 = data != null;
				if (flag5)
				{
					this.m_CaptionImage.sprite = data.image;
				}
				else
				{
					this.m_CaptionImage.sprite = null;
				}
				this.m_CaptionImage.enabled = (this.m_CaptionImage.sprite != null);
			}
			bool flag6 = this.m_Placeholder;
			if (flag6)
			{
				this.m_Placeholder.enabled = (this.options.Count == 0 || this.m_Value == -1);
			}
		}

		// Token: 0x0600BB0B RID: 47883 RVA: 0x00551ADE File Offset: 0x0054FCDE
		public void AddOptions(List<CDropdown.OptionData> options)
		{
			this.options.AddRange(options);
			this.RefreshShownValue();
		}

		// Token: 0x0600BB0C RID: 47884 RVA: 0x00551AF8 File Offset: 0x0054FCF8
		public void AddOptions(List<string> options)
		{
			for (int i = 0; i < options.Count; i++)
			{
				this.options.Add(new CDropdown.OptionData(options[i]));
			}
			this.RefreshShownValue();
		}

		// Token: 0x0600BB0D RID: 47885 RVA: 0x00551B3C File Offset: 0x0054FD3C
		public void AddOptions(List<Sprite> options)
		{
			for (int i = 0; i < options.Count; i++)
			{
				this.options.Add(new CDropdown.OptionData(options[i]));
			}
			this.RefreshShownValue();
		}

		// Token: 0x0600BB0E RID: 47886 RVA: 0x00551B7E File Offset: 0x0054FD7E
		public void ClearOptions()
		{
			this.options.Clear();
			this.m_Value = (this.m_Placeholder ? -1 : 0);
			this.RefreshShownValue();
		}

		// Token: 0x0600BB0F RID: 47887 RVA: 0x00551BAC File Offset: 0x0054FDAC
		private void SetupTemplate()
		{
			this.validTemplate = false;
			bool flag = !this.m_Template;
			if (flag)
			{
				Debug.LogError("The dropdown template is not assigned. The template needs to be assigned and must have a child GameObject with a Toggle component serving as the item.", this);
			}
			else
			{
				GameObject templateGo = this.m_Template.gameObject;
				templateGo.SetActive(true);
				Toggle itemToggle = this.m_Template.GetComponentInChildren<Toggle>();
				this.validTemplate = true;
				bool flag2 = !itemToggle || itemToggle.transform == this.template;
				if (flag2)
				{
					this.validTemplate = false;
					Debug.LogError("The dropdown template is not valid. The template must have a child GameObject with a Toggle component serving as the item.", this.template);
				}
				else
				{
					bool flag3 = !(itemToggle.transform.parent is RectTransform);
					if (flag3)
					{
						this.validTemplate = false;
						Debug.LogError("The dropdown template is not valid. The child GameObject with a Toggle component (the item) must have a RectTransform on its parent.", this.template);
					}
					else
					{
						bool flag4 = this.itemText != null && !this.itemText.transform.IsChildOf(itemToggle.transform);
						if (flag4)
						{
							this.validTemplate = false;
							Debug.LogError("The dropdown template is not valid. The Item Text must be on the item GameObject or children of it.", this.template);
						}
						else
						{
							bool flag5 = this.itemImage != null && !this.itemImage.transform.IsChildOf(itemToggle.transform);
							if (flag5)
							{
								this.validTemplate = false;
								Debug.LogError("The dropdown template is not valid. The Item Image must be on the item GameObject or children of it.", this.template);
							}
						}
					}
				}
				bool flag6 = !this.validTemplate;
				if (flag6)
				{
					templateGo.SetActive(false);
				}
				else
				{
					CDropdown.DropdownItem item = itemToggle.gameObject.AddComponent<CDropdown.DropdownItem>();
					item.text = this.m_ItemText;
					item.image = this.m_ItemImage;
					item.toggle = itemToggle;
					item.rectTransform = (RectTransform)itemToggle.transform;
					Canvas parentCanvas = null;
					Transform parentTransform = this.m_Template.parent;
					while (parentTransform != null)
					{
						parentCanvas = parentTransform.GetComponent<Canvas>();
						bool flag7 = parentCanvas != null;
						if (flag7)
						{
							break;
						}
						parentTransform = parentTransform.parent;
					}
					Canvas popupCanvas = CDropdown.GetOrAddComponent<Canvas>(templateGo);
					popupCanvas.overrideSorting = true;
					popupCanvas.sortingOrder = this.m_sortingOrder;
					bool flag8 = parentCanvas != null;
					if (flag8)
					{
						BaseRaycaster[] components = parentCanvas.GetComponents<BaseRaycaster>();
						for (int i = 0; i < components.Length; i++)
						{
							Type raycasterType = components[i].GetType();
							bool flag9 = templateGo.GetComponent(raycasterType) == null;
							if (flag9)
							{
								templateGo.AddComponent(raycasterType);
							}
						}
					}
					else
					{
						CDropdown.GetOrAddComponent<GraphicRaycaster>(templateGo);
					}
					CDropdown.GetOrAddComponent<CanvasGroup>(templateGo);
					templateGo.SetActive(false);
					this.validTemplate = true;
				}
			}
		}

		// Token: 0x0600BB10 RID: 47888 RVA: 0x00551E5C File Offset: 0x0055005C
		private static T GetOrAddComponent<T>(GameObject go) where T : Component
		{
			T comp = go.GetComponent<T>();
			bool flag = !comp;
			if (flag)
			{
				comp = go.AddComponent<T>();
			}
			return comp;
		}

		// Token: 0x0600BB11 RID: 47889 RVA: 0x00551E8F File Offset: 0x0055008F
		public virtual void OnPointerClick(PointerEventData eventData)
		{
			this.Show();
		}

		// Token: 0x0600BB12 RID: 47890 RVA: 0x00551E99 File Offset: 0x00550099
		public virtual void OnSubmit(BaseEventData eventData)
		{
			this.Show();
		}

		// Token: 0x0600BB13 RID: 47891 RVA: 0x00551EA3 File Offset: 0x005500A3
		public virtual void OnCancel(BaseEventData eventData)
		{
			this.Hide();
		}

		// Token: 0x0600BB14 RID: 47892 RVA: 0x00551EB0 File Offset: 0x005500B0
		public void Show()
		{
			bool flag = this.m_Coroutine != null;
			if (flag)
			{
				base.StopCoroutine(this.m_Coroutine);
				this.ImmediateDestroyDropdownList();
			}
			bool flag2 = !this.IsActive() || !this.IsInteractable() || this.m_Dropdown != null;
			if (!flag2)
			{
				List<Canvas> list = new List<Canvas>();
				base.gameObject.GetComponentsInParent<Canvas>(false, list);
				bool flag3 = list.Count == 0;
				if (!flag3)
				{
					Canvas rootCanvas = list[list.Count - 1];
					for (int i = 0; i < list.Count; i++)
					{
						bool isRootCanvas = list[i].isRootCanvas;
						if (isRootCanvas)
						{
							rootCanvas = list[i];
							break;
						}
					}
					bool flag4 = !this.validTemplate;
					if (flag4)
					{
						this.SetupTemplate();
						bool flag5 = !this.validTemplate;
						if (flag5)
						{
							return;
						}
					}
					this.m_Template.gameObject.SetActive(true);
					this.m_Template.GetComponent<Canvas>().sortingLayerID = rootCanvas.sortingLayerID;
					this.m_Dropdown = this.CreateDropdownList(this.m_Template.gameObject);
					this.m_Dropdown.name = "Dropdown List";
					this.m_Dropdown.SetActive(true);
					RectTransform dropdownRectTransform = this.m_Dropdown.transform as RectTransform;
					dropdownRectTransform.SetParent(this.m_Template.transform.parent, false);
					CDropdown.DropdownItem itemTemplate = this.m_Dropdown.GetComponentInChildren<CDropdown.DropdownItem>();
					GameObject content = itemTemplate.rectTransform.parent.gameObject;
					RectTransform contentRectTransform = content.transform as RectTransform;
					itemTemplate.rectTransform.gameObject.SetActive(true);
					Rect dropdownContentRect = contentRectTransform.rect;
					Rect itemTemplateRect = itemTemplate.rectTransform.rect;
					Vector2 offsetMin = itemTemplateRect.min - dropdownContentRect.min + itemTemplate.rectTransform.localPosition;
					Vector2 offsetMax = itemTemplateRect.max - dropdownContentRect.max + itemTemplate.rectTransform.localPosition;
					Vector2 itemSize = itemTemplateRect.size;
					this.m_Items.Clear();
					Toggle prev = null;
					for (int j = 0; j < this.options.Count; j++)
					{
						CDropdown.OptionData data = this.options[j];
						CDropdown.DropdownItem item = this.AddItem(data, this.value == j, itemTemplate, this.m_Items);
						bool flag6 = item == null;
						if (!flag6)
						{
							item.toggle.isOn = (this.value == j);
							item.toggle.onValueChanged.AddListener(delegate(bool x)
							{
								this.OnSelectItem(item.toggle);
							});
							bool isOn = item.toggle.isOn;
							if (isOn)
							{
								item.toggle.Select();
							}
							bool flag7 = prev != null;
							if (flag7)
							{
								Navigation prevNav = prev.navigation;
								Navigation toggleNav = item.toggle.navigation;
								prevNav.mode = Navigation.Mode.Explicit;
								toggleNav.mode = Navigation.Mode.Explicit;
								prevNav.selectOnDown = item.toggle;
								prevNav.selectOnRight = item.toggle;
								toggleNav.selectOnLeft = prev;
								toggleNav.selectOnUp = prev;
								prev.navigation = prevNav;
								item.toggle.navigation = toggleNav;
							}
							prev = item.toggle;
						}
					}
					Vector2 sizeDelta = contentRectTransform.sizeDelta;
					sizeDelta.y = itemSize.y * (float)this.m_Items.Count + offsetMin.y - offsetMax.y;
					contentRectTransform.sizeDelta = sizeDelta;
					float extraSpace = dropdownRectTransform.rect.height - contentRectTransform.rect.height;
					bool flag8 = extraSpace > 0f;
					if (flag8)
					{
						dropdownRectTransform.sizeDelta = new Vector2(dropdownRectTransform.sizeDelta.x, dropdownRectTransform.sizeDelta.y - extraSpace);
					}
					Vector3[] corners = new Vector3[4];
					dropdownRectTransform.GetWorldCorners(corners);
					RectTransform rootCanvasRectTransform = rootCanvas.transform as RectTransform;
					Rect rootCanvasRect = rootCanvasRectTransform.rect;
					for (int axis = 0; axis < 2; axis++)
					{
						bool outside = false;
						for (int k = 0; k < 4; k++)
						{
							Vector3 corner = rootCanvasRectTransform.InverseTransformPoint(corners[k]);
							bool flag9 = (corner[axis] < rootCanvasRect.min[axis] && !Mathf.Approximately(corner[axis], rootCanvasRect.min[axis])) || (corner[axis] > rootCanvasRect.max[axis] && !Mathf.Approximately(corner[axis], rootCanvasRect.max[axis]));
							if (flag9)
							{
								outside = true;
								break;
							}
						}
						bool flag10 = outside;
						if (flag10)
						{
							RectTransformUtility.FlipLayoutOnAxis(dropdownRectTransform, axis, false, false);
						}
					}
					for (int l = 0; l < this.m_Items.Count; l++)
					{
						RectTransform itemRect = this.m_Items[l].rectTransform;
						itemRect.anchorMin = new Vector2(itemRect.anchorMin.x, 0f);
						itemRect.anchorMax = new Vector2(itemRect.anchorMax.x, 0f);
						itemRect.anchoredPosition = new Vector2(itemRect.anchoredPosition.x, offsetMin.y + itemSize.y * (float)(this.m_Items.Count - 1 - l) + itemSize.y * itemRect.pivot.y);
						itemRect.sizeDelta = new Vector2(itemRect.sizeDelta.x, itemSize.y);
					}
					this.AlphaFadeList(this.m_AlphaFadeSpeed, 0f, 1f);
					this.m_Template.gameObject.SetActive(false);
					itemTemplate.gameObject.SetActive(false);
					this.m_Blocker = this.CreateBlocker(rootCanvas);
				}
			}
		}

		// Token: 0x0600BB15 RID: 47893 RVA: 0x00552560 File Offset: 0x00550760
		protected virtual GameObject CreateBlocker(Canvas rootCanvas)
		{
			GameObject blocker = new GameObject("Blocker");
			RectTransform blockerRect = blocker.AddComponent<RectTransform>();
			blockerRect.SetParent(rootCanvas.transform, false);
			blockerRect.anchorMin = Vector3.zero;
			blockerRect.anchorMax = Vector3.one;
			blockerRect.sizeDelta = Vector2.zero;
			Canvas blockerCanvas = blocker.AddComponent<Canvas>();
			blockerCanvas.overrideSorting = true;
			Canvas dropdownCanvas = this.m_Dropdown.GetComponent<Canvas>();
			blockerCanvas.sortingLayerID = dropdownCanvas.sortingLayerID;
			blockerCanvas.sortingOrder = dropdownCanvas.sortingOrder - 1;
			Canvas parentCanvas = null;
			Transform parentTransform = this.m_Template.parent;
			while (parentTransform != null)
			{
				parentCanvas = parentTransform.GetComponent<Canvas>();
				bool flag = parentCanvas != null;
				if (flag)
				{
					break;
				}
				parentTransform = parentTransform.parent;
			}
			bool flag2 = parentCanvas != null;
			if (flag2)
			{
				BaseRaycaster[] components = parentCanvas.GetComponents<BaseRaycaster>();
				for (int i = 0; i < components.Length; i++)
				{
					Type raycasterType = components[i].GetType();
					bool flag3 = blocker.GetComponent(raycasterType) == null;
					if (flag3)
					{
						blocker.AddComponent(raycasterType);
					}
				}
			}
			else
			{
				CDropdown.GetOrAddComponent<GraphicRaycaster>(blocker);
			}
			Image blockerImage = blocker.AddComponent<Image>();
			blockerImage.color = Color.clear;
			Button blockerButton = blocker.AddComponent<Button>();
			UIInteractionBehaviour interaction = base.GetComponent<UIInteractionBehaviour>();
			blockerButton.onClick.AddListener(delegate()
			{
				UIInteractionBehaviour interaction = interaction;
				if (interaction != null)
				{
					interaction.Play(this.interactable);
				}
				this.Hide();
			});
			return blocker;
		}

		// Token: 0x0600BB16 RID: 47894 RVA: 0x005526FA File Offset: 0x005508FA
		protected virtual void DestroyBlocker(GameObject blocker)
		{
			Object.Destroy(blocker);
		}

		// Token: 0x0600BB17 RID: 47895 RVA: 0x00552704 File Offset: 0x00550904
		protected virtual GameObject CreateDropdownList(GameObject template)
		{
			return Object.Instantiate<GameObject>(template);
		}

		// Token: 0x0600BB18 RID: 47896 RVA: 0x0055271C File Offset: 0x0055091C
		protected virtual void DestroyDropdownList(GameObject dropdownList)
		{
			Object.Destroy(dropdownList);
		}

		// Token: 0x0600BB19 RID: 47897 RVA: 0x00552728 File Offset: 0x00550928
		protected virtual CDropdown.DropdownItem CreateItem(CDropdown.DropdownItem itemTemplate)
		{
			return Object.Instantiate<CDropdown.DropdownItem>(itemTemplate);
		}

		// Token: 0x0600BB1A RID: 47898 RVA: 0x00552740 File Offset: 0x00550940
		protected virtual void DestroyItem(CDropdown.DropdownItem item)
		{
		}

		// Token: 0x0600BB1B RID: 47899 RVA: 0x00552744 File Offset: 0x00550944
		private CDropdown.DropdownItem AddItem(CDropdown.OptionData data, bool selected, CDropdown.DropdownItem itemTemplate, List<CDropdown.DropdownItem> items)
		{
			CDropdown.DropdownItem item = this.CreateItem(itemTemplate);
			item.rectTransform.SetParent(itemTemplate.rectTransform.parent, false);
			int index = items.Count;
			item.gameObject.SetActive(true);
			item.gameObject.name = "Item " + index.ToString() + ((data.text != null) ? (": " + data.text) : "");
			bool flag = item.toggle != null;
			if (flag)
			{
				item.toggle.isOn = false;
			}
			bool flag2 = item.text;
			if (flag2)
			{
				item.text.text = data.text;
			}
			bool flag3 = item.image;
			if (flag3)
			{
				item.image.sprite = data.image;
				item.image.enabled = (item.image.sprite != null);
			}
			items.Add(item);
			Action<int, RectTransform> onItemAdded = this.OnItemAdded;
			if (onItemAdded != null)
			{
				onItemAdded(index, item.rectTransform);
			}
			return item;
		}

		// Token: 0x0600BB1C RID: 47900 RVA: 0x00552870 File Offset: 0x00550A70
		private void AlphaFadeList(float duration, float alpha)
		{
			CanvasGroup group = this.m_Dropdown.GetComponent<CanvasGroup>();
			this.AlphaFadeList(duration, group.alpha, alpha);
		}

		// Token: 0x0600BB1D RID: 47901 RVA: 0x0055289C File Offset: 0x00550A9C
		private void AlphaFadeList(float duration, float start, float end)
		{
			bool flag = end.Equals(start);
			if (!flag)
			{
				FloatTween tween = new FloatTween
				{
					duration = duration,
					startValue = start,
					targetValue = end
				};
				tween.AddOnChangedCallback(new UnityAction<float>(this.SetAlpha));
				tween.ignoreTimeScale = true;
				this.m_AlphaTweenRunner.StartTween(tween);
			}
		}

		// Token: 0x0600BB1E RID: 47902 RVA: 0x00552908 File Offset: 0x00550B08
		private void SetAlpha(float alpha)
		{
			bool flag = !this.m_Dropdown;
			if (!flag)
			{
				CanvasGroup group = this.m_Dropdown.GetComponent<CanvasGroup>();
				group.alpha = alpha;
			}
		}

		// Token: 0x0600BB1F RID: 47903 RVA: 0x00552940 File Offset: 0x00550B40
		public void Hide()
		{
			bool flag = this.m_Coroutine == null;
			if (flag)
			{
				bool flag2 = this.m_Dropdown != null;
				if (flag2)
				{
					this.AlphaFadeList(this.m_AlphaFadeSpeed, 0f);
					bool flag3 = this.IsActive();
					if (flag3)
					{
						this.m_Coroutine = base.StartCoroutine(this.DelayedDestroyDropdownList(this.m_AlphaFadeSpeed));
					}
				}
				bool flag4 = this.m_Blocker != null;
				if (flag4)
				{
					this.DestroyBlocker(this.m_Blocker);
				}
				this.m_Blocker = null;
			}
		}

		// Token: 0x0600BB20 RID: 47904 RVA: 0x005529C7 File Offset: 0x00550BC7
		private IEnumerator DelayedDestroyDropdownList(float delay)
		{
			yield return new WaitForSecondsRealtime(delay);
			this.ImmediateDestroyDropdownList();
			yield break;
		}

		// Token: 0x0600BB21 RID: 47905 RVA: 0x005529E0 File Offset: 0x00550BE0
		private void ImmediateDestroyDropdownList()
		{
			for (int i = 0; i < this.m_Items.Count; i++)
			{
				bool flag = this.m_Items[i] != null;
				if (flag)
				{
					this.DestroyItem(this.m_Items[i]);
				}
			}
			this.m_Items.Clear();
			bool flag2 = this.m_Dropdown != null;
			if (flag2)
			{
				this.DestroyDropdownList(this.m_Dropdown);
			}
			bool flag3 = this.m_AlphaTweenRunner != null;
			if (flag3)
			{
				this.m_AlphaTweenRunner.StopTween();
			}
			this.m_Dropdown = null;
			this.m_Coroutine = null;
		}

		// Token: 0x0600BB22 RID: 47906 RVA: 0x00552A88 File Offset: 0x00550C88
		private void OnSelectItem(Toggle toggle)
		{
			bool flag = !toggle.isOn;
			if (flag)
			{
				toggle.SetIsOnWithoutNotify(true);
			}
			int selectedIndex = -1;
			Transform tr = toggle.transform;
			Transform parent = tr.parent;
			for (int i = 0; i < parent.childCount; i++)
			{
				bool flag2 = parent.GetChild(i) == tr;
				if (flag2)
				{
					selectedIndex = i - 1;
					break;
				}
			}
			bool flag3 = selectedIndex < 0;
			if (!flag3)
			{
				this.value = selectedIndex;
				this.Hide();
			}
		}

		// Token: 0x04009059 RID: 36953
		[SerializeField]
		private RectTransform m_Template;

		// Token: 0x0400905A RID: 36954
		[SerializeField]
		private TMP_Text m_CaptionText;

		// Token: 0x0400905B RID: 36955
		[SerializeField]
		private Image m_CaptionImage;

		// Token: 0x0400905C RID: 36956
		[SerializeField]
		private Graphic m_Placeholder;

		// Token: 0x0400905D RID: 36957
		[Space]
		[SerializeField]
		private TMP_Text m_ItemText;

		// Token: 0x0400905E RID: 36958
		[SerializeField]
		private Image m_ItemImage;

		// Token: 0x0400905F RID: 36959
		[Space]
		[SerializeField]
		private int m_Value;

		// Token: 0x04009060 RID: 36960
		[Space]
		[SerializeField]
		private CDropdown.OptionDataList m_Options = new CDropdown.OptionDataList();

		// Token: 0x04009061 RID: 36961
		[Space]
		[SerializeField]
		private CDropdown.DropdownEvent m_OnValueChanged = new CDropdown.DropdownEvent();

		// Token: 0x04009062 RID: 36962
		[SerializeField]
		private CDropdown.DropdownEvent m_OnSelect = new CDropdown.DropdownEvent();

		// Token: 0x04009063 RID: 36963
		[SerializeField]
		private float m_AlphaFadeSpeed = 0.15f;

		// Token: 0x04009064 RID: 36964
		[SerializeField]
		private int m_sortingOrder = 30000;

		// Token: 0x04009065 RID: 36965
		private GameObject m_Dropdown;

		// Token: 0x04009066 RID: 36966
		private GameObject m_Blocker;

		// Token: 0x04009067 RID: 36967
		private List<CDropdown.DropdownItem> m_Items = new List<CDropdown.DropdownItem>();

		// Token: 0x04009068 RID: 36968
		private TweenRunner<FloatTween> m_AlphaTweenRunner;

		// Token: 0x04009069 RID: 36969
		private bool validTemplate = false;

		// Token: 0x0400906A RID: 36970
		private Coroutine m_Coroutine = null;

		// Token: 0x0400906B RID: 36971
		private static CDropdown.OptionData s_NoOptionData = new CDropdown.OptionData();

		// Token: 0x0400906C RID: 36972
		public Action<int, RectTransform> OnItemAdded;

		// Token: 0x0200263E RID: 9790
		protected internal class DropdownItem : MonoBehaviour, IPointerEnterHandler, IEventSystemHandler, IPointerExitHandler, ICancelHandler
		{
			// Token: 0x17001BAD RID: 7085
			// (get) Token: 0x06011B59 RID: 72537 RVA: 0x00687378 File Offset: 0x00685578
			// (set) Token: 0x06011B5A RID: 72538 RVA: 0x00687390 File Offset: 0x00685590
			public TMP_Text text
			{
				get
				{
					return this.m_Text;
				}
				set
				{
					this.m_Text = value;
				}
			}

			// Token: 0x17001BAE RID: 7086
			// (get) Token: 0x06011B5B RID: 72539 RVA: 0x0068739C File Offset: 0x0068559C
			// (set) Token: 0x06011B5C RID: 72540 RVA: 0x006873B4 File Offset: 0x006855B4
			public Image image
			{
				get
				{
					return this.m_Image;
				}
				set
				{
					this.m_Image = value;
				}
			}

			// Token: 0x17001BAF RID: 7087
			// (get) Token: 0x06011B5D RID: 72541 RVA: 0x006873C0 File Offset: 0x006855C0
			// (set) Token: 0x06011B5E RID: 72542 RVA: 0x006873D8 File Offset: 0x006855D8
			public RectTransform rectTransform
			{
				get
				{
					return this.m_RectTransform;
				}
				set
				{
					this.m_RectTransform = value;
				}
			}

			// Token: 0x17001BB0 RID: 7088
			// (get) Token: 0x06011B5F RID: 72543 RVA: 0x006873E4 File Offset: 0x006855E4
			// (set) Token: 0x06011B60 RID: 72544 RVA: 0x006873FC File Offset: 0x006855FC
			public Toggle toggle
			{
				get
				{
					return this.m_Toggle;
				}
				set
				{
					this.m_Toggle = value;
				}
			}

			// Token: 0x06011B61 RID: 72545 RVA: 0x00687406 File Offset: 0x00685606
			public virtual void OnPointerEnter(PointerEventData eventData)
			{
				EventSystem.current.SetSelectedGameObject(base.gameObject);
			}

			// Token: 0x06011B62 RID: 72546 RVA: 0x0068741C File Offset: 0x0068561C
			public virtual void OnPointerExit(PointerEventData eventData)
			{
				bool flag = EventSystem.current.currentSelectedGameObject == base.gameObject;
				if (flag)
				{
					EventSystem.current.SetSelectedGameObject(null);
				}
			}

			// Token: 0x06011B63 RID: 72547 RVA: 0x00687450 File Offset: 0x00685650
			public virtual void OnCancel(BaseEventData eventData)
			{
				CDropdown dropdown = base.GetComponentInParent<CDropdown>();
				bool flag = dropdown;
				if (flag)
				{
					dropdown.Hide();
				}
			}

			// Token: 0x0400EA0A RID: 59914
			[SerializeField]
			private TMP_Text m_Text;

			// Token: 0x0400EA0B RID: 59915
			[SerializeField]
			private Image m_Image;

			// Token: 0x0400EA0C RID: 59916
			[SerializeField]
			private RectTransform m_RectTransform;

			// Token: 0x0400EA0D RID: 59917
			[SerializeField]
			private Toggle m_Toggle;
		}

		// Token: 0x0200263F RID: 9791
		[Serializable]
		public class OptionData
		{
			// Token: 0x17001BB1 RID: 7089
			// (get) Token: 0x06011B65 RID: 72549 RVA: 0x00687480 File Offset: 0x00685680
			// (set) Token: 0x06011B66 RID: 72550 RVA: 0x00687498 File Offset: 0x00685698
			public string text
			{
				get
				{
					return this.m_Text;
				}
				set
				{
					this.m_Text = value;
				}
			}

			// Token: 0x17001BB2 RID: 7090
			// (get) Token: 0x06011B67 RID: 72551 RVA: 0x006874A4 File Offset: 0x006856A4
			// (set) Token: 0x06011B68 RID: 72552 RVA: 0x006874BC File Offset: 0x006856BC
			public Sprite image
			{
				get
				{
					return this.m_Image;
				}
				set
				{
					this.m_Image = value;
				}
			}

			// Token: 0x06011B69 RID: 72553 RVA: 0x006874C6 File Offset: 0x006856C6
			public OptionData()
			{
			}

			// Token: 0x06011B6A RID: 72554 RVA: 0x006874D0 File Offset: 0x006856D0
			public OptionData(string text)
			{
				this.text = text;
			}

			// Token: 0x06011B6B RID: 72555 RVA: 0x006874E2 File Offset: 0x006856E2
			public OptionData(Sprite image)
			{
				this.image = image;
			}

			// Token: 0x06011B6C RID: 72556 RVA: 0x006874F4 File Offset: 0x006856F4
			public OptionData(string text, Sprite image)
			{
				this.text = text;
				this.image = image;
			}

			// Token: 0x0400EA0E RID: 59918
			[SerializeField]
			private string m_Text;

			// Token: 0x0400EA0F RID: 59919
			[SerializeField]
			private Sprite m_Image;
		}

		// Token: 0x02002640 RID: 9792
		[Serializable]
		public class OptionDataList
		{
			// Token: 0x17001BB3 RID: 7091
			// (get) Token: 0x06011B6D RID: 72557 RVA: 0x00687510 File Offset: 0x00685710
			// (set) Token: 0x06011B6E RID: 72558 RVA: 0x00687528 File Offset: 0x00685728
			public List<CDropdown.OptionData> options
			{
				get
				{
					return this.m_Options;
				}
				set
				{
					this.m_Options = value;
				}
			}

			// Token: 0x06011B6F RID: 72559 RVA: 0x00687532 File Offset: 0x00685732
			public OptionDataList()
			{
				this.options = new List<CDropdown.OptionData>();
			}

			// Token: 0x0400EA10 RID: 59920
			[SerializeField]
			private List<CDropdown.OptionData> m_Options;
		}

		// Token: 0x02002641 RID: 9793
		[Serializable]
		public class DropdownEvent : UnityEvent<int>
		{
		}
	}
}
