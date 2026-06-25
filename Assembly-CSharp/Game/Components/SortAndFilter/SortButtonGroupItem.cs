using System;
using FrameWork.UI.LanguageRule;
using FrameWork.UISystem.UIElements;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Game.Components.SortAndFilter
{
	// Token: 0x02000CC8 RID: 3272
	public class SortButtonGroupItem : MonoBehaviour, IPointerEnterHandler, IEventSystemHandler, IPointerExitHandler
	{
		// Token: 0x0600A589 RID: 42377 RVA: 0x004D2AF0 File Offset: 0x004D0CF0
		private void Awake()
		{
			this._backgroundImage = (this.button.targetGraphic as CImage);
			bool flag = this._backgroundImage == null;
			if (flag)
			{
				this._backgroundImage = base.GetComponent<CImage>();
			}
		}

		// Token: 0x0600A58A RID: 42378 RVA: 0x004D2B30 File Offset: 0x004D0D30
		public void SetClickHandler(Action action)
		{
			this.button.ClearAndAddListener(action);
		}

		// Token: 0x0600A58B RID: 42379 RVA: 0x004D2B40 File Offset: 0x004D0D40
		public void Refresh(string labelText, ESortDirection direction, int order)
		{
			this.label.SetText(labelText, true);
			bool flag = this.languageRuleTips;
			if (flag)
			{
				this.languageRuleTips.Refresh();
			}
			this._direction = direction;
			this.RefreshBackgroundSprite();
		}

		// Token: 0x0600A58C RID: 42380 RVA: 0x004D2B85 File Offset: 0x004D0D85
		public void OnPointerEnter(PointerEventData eventData)
		{
			this._isHovered = true;
			this.RefreshBackgroundSprite();
		}

		// Token: 0x0600A58D RID: 42381 RVA: 0x004D2B96 File Offset: 0x004D0D96
		public void OnPointerExit(PointerEventData eventData)
		{
			this._isHovered = false;
			this.RefreshBackgroundSprite();
		}

		// Token: 0x0600A58E RID: 42382 RVA: 0x004D2BA8 File Offset: 0x004D0DA8
		private void RefreshBackgroundSprite()
		{
			bool flag = this._backgroundImage == null;
			if (!flag)
			{
				ESortDirection direction = this._direction;
				if (!true)
				{
				}
				string text;
				if (direction != ESortDirection.Ascending)
				{
					if (direction != ESortDirection.Descending)
					{
						text = "default";
					}
					else
					{
						text = "down";
					}
				}
				else
				{
					text = "up";
				}
				if (!true)
				{
				}
				string directionName = text;
				int spriteStateIndex = this._isHovered ? 1 : 0;
				this._backgroundImage.SetSprite(string.Format("ui9_btn_sort_button_{0}_{1}", directionName, spriteStateIndex), false, null);
			}
		}

		// Token: 0x040082B3 RID: 33459
		[SerializeField]
		private CButton button;

		// Token: 0x040082B4 RID: 33460
		[SerializeField]
		private TextMeshProUGUI label;

		// Token: 0x040082B5 RID: 33461
		[SerializeField]
		private LanguageRuleTips languageRuleTips;

		// Token: 0x040082B6 RID: 33462
		private const string DirectionDefault = "default";

		// Token: 0x040082B7 RID: 33463
		private const string DirectionUp = "up";

		// Token: 0x040082B8 RID: 33464
		private const string DirectionDown = "down";

		// Token: 0x040082B9 RID: 33465
		private CImage _backgroundImage;

		// Token: 0x040082BA RID: 33466
		private ESortDirection _direction;

		// Token: 0x040082BB RID: 33467
		private bool _isHovered;
	}
}
