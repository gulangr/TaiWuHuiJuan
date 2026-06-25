using System;
using DG.Tweening;
using FrameWork.UISystem.UIElements;
using Game.Components.Character;
using GameData.Domains.Character.Display;
using GameData.Domains.Item.Display;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Game.Views.CharacterMenu
{
	// Token: 0x02000B69 RID: 2921
	public class CharacterMenuSelectableCharacterNormal : CButton
	{
		// Token: 0x0600908B RID: 37003 RVA: 0x00435FDD File Offset: 0x004341DD
		public void Set(CharacterDisplayData characterDisplayData, bool selected)
		{
			this.characterNormal.Set(characterDisplayData);
			this.SetSelected(selected);
		}

		// Token: 0x0600908C RID: 37004 RVA: 0x00435FF8 File Offset: 0x004341F8
		public void SetSelected(bool selected)
		{
			this.selectMark.SetActive(selected);
			this._isSelected = selected;
			bool flag = this.hoverNode != null && this.hoverNode.activeInHierarchy;
			if (flag)
			{
				this.UpdateHoverNodeSprite();
			}
		}

		// Token: 0x0600908D RID: 37005 RVA: 0x00436041 File Offset: 0x00434241
		public void SetName(string charName)
		{
			this.characterNormal.SetName(charName);
		}

		// Token: 0x0600908E RID: 37006 RVA: 0x00436050 File Offset: 0x00434250
		public void SetTransferInfo(ItemDisplayData itemData, CharacterMenuCharacterTransferInfo.TransferData transferData)
		{
			bool flag = this._transferSequence != null;
			if (flag)
			{
				this._transferSequence.Kill(true);
				this._transferSequence = null;
			}
			bool flag2 = transferData == null;
			if (!flag2)
			{
				bool isShow = this.transferInfo.SetData(itemData, transferData);
				bool flag3 = !isShow;
				if (!flag3)
				{
					CanvasGroup canvasGroup = this.transferInfo.GetComponent<CanvasGroup>();
					canvasGroup.alpha = 0f;
					this.transferInfo.gameObject.SetActive(true);
					this._transferSequence = DOTween.Sequence();
					this._transferSequence.Append(canvasGroup.DOFade(1f, 0.3f));
					this._transferSequence.AppendInterval(3f);
					this._transferSequence.Append(canvasGroup.DOFade(0f, 0.3f));
					this._transferSequence.AppendCallback(delegate
					{
						this.transferInfo.gameObject.SetActive(false);
					});
					this._transferSequence.Play<Sequence>();
				}
			}
		}

		// Token: 0x0600908F RID: 37007 RVA: 0x00436150 File Offset: 0x00434350
		public override void OnPointerEnter(PointerEventData eventData)
		{
			base.OnPointerEnter(eventData);
			bool interactable = base.interactable;
			if (interactable)
			{
				this.ShowHoverNode(true);
			}
		}

		// Token: 0x06009090 RID: 37008 RVA: 0x00436178 File Offset: 0x00434378
		public override void OnPointerExit(PointerEventData eventData)
		{
			base.OnPointerExit(eventData);
			this.ShowHoverNode(false);
		}

		// Token: 0x06009091 RID: 37009 RVA: 0x0043618C File Offset: 0x0043438C
		private void ShowHoverNode(bool show)
		{
			bool flag = this.hoverNode == null;
			if (!flag)
			{
				this.hoverNode.SetActive(show);
				if (show)
				{
					this.UpdateHoverNodeSprite();
				}
			}
		}

		// Token: 0x06009092 RID: 37010 RVA: 0x004361C8 File Offset: 0x004343C8
		private void UpdateHoverNodeSprite()
		{
			Image image = this.hoverNode.GetComponent<Image>();
			bool flag = image == null;
			if (!flag)
			{
				image.sprite = (this._isSelected ? this.selectedHoverSprite : this.normalHoverSprite);
			}
		}

		// Token: 0x04006F45 RID: 28485
		[SerializeField]
		protected CharacterNormal characterNormal;

		// Token: 0x04006F46 RID: 28486
		[SerializeField]
		protected GameObject selectMark;

		// Token: 0x04006F47 RID: 28487
		[SerializeField]
		protected CharacterMenuCharacterTransferInfo transferInfo;

		// Token: 0x04006F48 RID: 28488
		[SerializeField]
		private GameObject hoverNode;

		// Token: 0x04006F49 RID: 28489
		[SerializeField]
		private Sprite normalHoverSprite;

		// Token: 0x04006F4A RID: 28490
		[SerializeField]
		private Sprite selectedHoverSprite;

		// Token: 0x04006F4B RID: 28491
		private Sequence _transferSequence;

		// Token: 0x04006F4C RID: 28492
		private bool _isSelected;
	}
}
