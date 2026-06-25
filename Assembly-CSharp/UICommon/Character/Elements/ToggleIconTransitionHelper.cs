using System;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace UICommon.Character.Elements
{
	// Token: 0x020005FA RID: 1530
	[RequireComponent(typeof(CToggleObsolete), typeof(PointerTrigger))]
	public class ToggleIconTransitionHelper : MonoBehaviour
	{
		// Token: 0x1700090C RID: 2316
		// (get) Token: 0x06004836 RID: 18486 RVA: 0x0021DFE7 File Offset: 0x0021C1E7
		// (set) Token: 0x06004837 RID: 18487 RVA: 0x0021DFEF File Offset: 0x0021C1EF
		public Sprite NormalSprite
		{
			get
			{
				return this.normalSprite;
			}
			set
			{
				this._normalSpriteName = string.Empty;
				this.normalSprite = value;
				this.RefreshIconState();
			}
		}

		// Token: 0x1700090D RID: 2317
		// (get) Token: 0x06004838 RID: 18488 RVA: 0x0021E00B File Offset: 0x0021C20B
		// (set) Token: 0x06004839 RID: 18489 RVA: 0x0021E013 File Offset: 0x0021C213
		public Sprite HighlightedSprite
		{
			get
			{
				return this.highlightedSprite;
			}
			set
			{
				this._highlightedSpriteName = string.Empty;
				this.highlightedSprite = value;
				this.RefreshIconState();
			}
		}

		// Token: 0x1700090E RID: 2318
		// (get) Token: 0x0600483A RID: 18490 RVA: 0x0021E02F File Offset: 0x0021C22F
		// (set) Token: 0x0600483B RID: 18491 RVA: 0x0021E037 File Offset: 0x0021C237
		public Sprite SelectedSprite
		{
			get
			{
				return this.selectedSprite;
			}
			set
			{
				this._selectedSpriteName = string.Empty;
				this.selectedSprite = value;
				this.RefreshIconState();
			}
		}

		// Token: 0x1700090F RID: 2319
		// (get) Token: 0x0600483C RID: 18492 RVA: 0x0021E053 File Offset: 0x0021C253
		// (set) Token: 0x0600483D RID: 18493 RVA: 0x0021E05B File Offset: 0x0021C25B
		public Sprite DisabledSprite
		{
			get
			{
				return this.disabledSprite;
			}
			set
			{
				this._disabledSpriteName = string.Empty;
				this.disabledSprite = value;
				this.RefreshIconState();
			}
		}

		// Token: 0x0600483E RID: 18494 RVA: 0x0021E078 File Offset: 0x0021C278
		public void SetSameSprite(Sprite sprite)
		{
			Image image = this.icon;
			this.disabledSprite = sprite;
			this.selectedSprite = sprite;
			this.highlightedSprite = sprite;
			this.normalSprite = sprite;
			image.sprite = sprite;
		}

		// Token: 0x0600483F RID: 18495 RVA: 0x0021E0B7 File Offset: 0x0021C2B7
		public void SetSpriteNames(ToggleTransitionIconSpriteNames spriteNames)
		{
			this._normalSpriteName = spriteNames.Normal;
			this._highlightedSpriteName = spriteNames.Highlighted;
			this._selectedSpriteName = spriteNames.Selected;
			this._disabledSpriteName = spriteNames.Disabled;
			this.RefreshIconState();
		}

		// Token: 0x06004840 RID: 18496 RVA: 0x0021E0F4 File Offset: 0x0021C2F4
		private void RefreshIconState()
		{
			CToggleObsolete toggle = base.GetComponent<CToggleObsolete>();
			bool flag = !toggle.interactable;
			if (flag)
			{
				this.SetIcon(this._disabledSpriteName, this.disabledSprite);
			}
			else
			{
				bool isOn = toggle.isOn;
				if (isOn)
				{
					this.SetIcon(this._selectedSpriteName, this.selectedSprite);
				}
				else
				{
					bool isPointerOver = this._isPointerOver;
					if (isPointerOver)
					{
						this.SetIcon(this._highlightedSpriteName, this.highlightedSprite);
					}
					else
					{
						this.SetIcon(this._normalSpriteName, this.normalSprite);
					}
				}
			}
		}

		// Token: 0x06004841 RID: 18497 RVA: 0x0021E184 File Offset: 0x0021C384
		private void Awake()
		{
			CToggleObsolete toggle = base.GetComponent<CToggleObsolete>();
			PointerTrigger pointerTrigger = base.GetComponent<PointerTrigger>();
			toggle.onValueChanged.AddListener(new UnityAction<bool>(this.OnToggleValueChanged));
			pointerTrigger.EnterEvent.AddListener(new UnityAction(this.OnPointerEnter));
			pointerTrigger.ExitEvent.AddListener(new UnityAction(this.OnPointerExit));
			toggle.OnInteractableChange.AddListener(new UnityAction<bool>(this.OnInteractableChange));
			toggle.OnInteractableChangeReverse.AddListener(delegate(bool v)
			{
				this.OnInteractableChange(!v);
			});
		}

		// Token: 0x06004842 RID: 18498 RVA: 0x0021E218 File Offset: 0x0021C418
		private void OnInteractableChange(bool interactable)
		{
			if (interactable)
			{
				bool isOn = base.GetComponent<CToggleObsolete>().isOn;
				if (isOn)
				{
					this.SetIcon(this._selectedSpriteName, this.selectedSprite);
				}
				else
				{
					this.SetIcon(this._normalSpriteName, this.normalSprite);
				}
			}
			else
			{
				this.SetIcon(this._disabledSpriteName, this.disabledSprite);
			}
		}

		// Token: 0x06004843 RID: 18499 RVA: 0x0021E280 File Offset: 0x0021C480
		private void OnToggleValueChanged(bool isOn)
		{
			if (isOn)
			{
				this.SetIcon(this._selectedSpriteName, this.selectedSprite);
			}
			else
			{
				this.SetIcon(this._normalSpriteName, this.normalSprite);
			}
		}

		// Token: 0x06004844 RID: 18500 RVA: 0x0021E2C0 File Offset: 0x0021C4C0
		private void OnPointerEnter()
		{
			this._isPointerOver = true;
			bool isOn = base.GetComponent<CToggleObsolete>().isOn;
			if (isOn)
			{
				this.SetIcon(this._selectedSpriteName, this.selectedSprite);
			}
			else
			{
				this.SetIcon(this._highlightedSpriteName, this.highlightedSprite);
			}
		}

		// Token: 0x06004845 RID: 18501 RVA: 0x0021E310 File Offset: 0x0021C510
		private void OnPointerExit()
		{
			this._isPointerOver = false;
			bool isOn = base.GetComponent<CToggleObsolete>().isOn;
			if (isOn)
			{
				this.SetIcon(this._selectedSpriteName, this.selectedSprite);
			}
			else
			{
				this.SetIcon(this._normalSpriteName, this.normalSprite);
			}
		}

		// Token: 0x06004846 RID: 18502 RVA: 0x0021E360 File Offset: 0x0021C560
		private void SetIcon(string spriteName, Sprite sprite)
		{
			bool flag = string.IsNullOrEmpty(spriteName);
			if (flag)
			{
				this.icon.sprite = sprite;
			}
			else
			{
				this.icon.SetSprite(spriteName, false, null);
			}
		}

		// Token: 0x040031E4 RID: 12772
		[SerializeField]
		internal CImage icon;

		// Token: 0x040031E5 RID: 12773
		[SerializeField]
		private Sprite normalSprite;

		// Token: 0x040031E6 RID: 12774
		[SerializeField]
		private Sprite highlightedSprite;

		// Token: 0x040031E7 RID: 12775
		[SerializeField]
		private Sprite selectedSprite;

		// Token: 0x040031E8 RID: 12776
		[SerializeField]
		private Sprite disabledSprite;

		// Token: 0x040031E9 RID: 12777
		private string _normalSpriteName;

		// Token: 0x040031EA RID: 12778
		private string _highlightedSpriteName;

		// Token: 0x040031EB RID: 12779
		private string _selectedSpriteName;

		// Token: 0x040031EC RID: 12780
		private string _disabledSpriteName;

		// Token: 0x040031ED RID: 12781
		private bool _isPointerOver;
	}
}
