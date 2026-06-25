using System;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace Game.Views.CharacterMenu.EquipCombatSkill
{
	// Token: 0x02000BBC RID: 3004
	public class EquipCombatSkillLineWithoutScroll : EquipCombatSkillLine
	{
		// Token: 0x1700103D RID: 4157
		// (get) Token: 0x0600975C RID: 38748 RVA: 0x00468798 File Offset: 0x00466998
		public float PivotCenterY
		{
			get
			{
				return this.pivotCenterY;
			}
		}

		// Token: 0x1700103E RID: 4158
		// (get) Token: 0x0600975D RID: 38749 RVA: 0x004687A0 File Offset: 0x004669A0
		// (set) Token: 0x0600975E RID: 38750 RVA: 0x004687A8 File Offset: 0x004669A8
		public bool IsLineSelected { get; private set; }

		// Token: 0x1700103F RID: 4159
		// (get) Token: 0x0600975F RID: 38751 RVA: 0x004687B1 File Offset: 0x004669B1
		public float DesignLineWidth
		{
			get
			{
				return (this.designLineWidth > 0f) ? this.designLineWidth : 543.92f;
			}
		}

		// Token: 0x06009760 RID: 38752 RVA: 0x004687CD File Offset: 0x004669CD
		private void OnDisable()
		{
			this.ResetEnterHoverState();
		}

		// Token: 0x06009761 RID: 38753 RVA: 0x004687D7 File Offset: 0x004669D7
		public override void Set(ViewCharacterMenuEquipCombatSkill parentView)
		{
			this.ResetEnterHoverState();
			base.Set(parentView);
		}

		// Token: 0x06009762 RID: 38754 RVA: 0x004687EC File Offset: 0x004669EC
		public float RecalculateLineWidth()
		{
			bool flag = this.typeRect == null || this.layoutRect == null;
			float result;
			if (flag)
			{
				result = ((RectTransform)base.transform).rect.width;
			}
			else
			{
				this.RebuildLineLayout();
				float width = this.MeasureLineWidth();
				((RectTransform)base.transform).SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, width);
				result = width;
			}
			return result;
		}

		// Token: 0x06009763 RID: 38755 RVA: 0x0046885C File Offset: 0x00466A5C
		private void RebuildLineLayout()
		{
			bool flag = this.skillRoot != null;
			if (flag)
			{
				LayoutRebuilder.ForceRebuildLayoutImmediate(this.skillRoot);
			}
			bool flag2 = this.emptyGridRoot != null;
			if (flag2)
			{
				LayoutRebuilder.ForceRebuildLayoutImmediate(this.emptyGridRoot);
			}
			LayoutRebuilder.ForceRebuildLayoutImmediate(this.layoutRect);
			LayoutRebuilder.ForceRebuildLayoutImmediate(this.typeRect);
			Canvas.ForceUpdateCanvases();
		}

		// Token: 0x06009764 RID: 38756 RVA: 0x004688C0 File Offset: 0x00466AC0
		public float GetWidthBeyondDesign()
		{
			return Mathf.Max(0f, this.RecalculateLineWidth() - this.DesignLineWidth);
		}

		// Token: 0x06009765 RID: 38757 RVA: 0x004688EC File Offset: 0x00466AEC
		private float MeasureLineWidth()
		{
			float spacing = 0f;
			HorizontalLayoutGroup layoutGroup;
			bool flag = this.layoutRect.TryGetComponent<HorizontalLayoutGroup>(out layoutGroup);
			if (flag)
			{
				spacing = layoutGroup.spacing;
			}
			return this.typeRect.rect.width + this.layoutRect.rect.width + spacing;
		}

		// Token: 0x06009766 RID: 38758 RVA: 0x00468948 File Offset: 0x00466B48
		protected override void BindSkillItemToLine(EquipCombatSkillItem equipCombatSkillItem)
		{
			equipCombatSkillItem.SetLineSelectHandler(new Func<bool>(this.TrySelectLineFromChildClick));
			equipCombatSkillItem.SetChildHoverSuppressed(() => !this.IsLineSelected);
			equipCombatSkillItem.AttachLineHoverCallbacks(new Action(this.OnChildPointerEnter), new Action(this.OnChildPointerExit));
		}

		// Token: 0x06009767 RID: 38759 RVA: 0x0046899C File Offset: 0x00466B9C
		protected override void BindEmptyGridPointerHover(PointerTrigger pointerTrigger, GameObject hover)
		{
			pointerTrigger.EnterEvent.AddListener(delegate()
			{
				bool isLineSelected = this.IsLineSelected;
				if (isLineSelected)
				{
					hover.SetActive(true);
				}
			});
			pointerTrigger.ExitEvent.AddListener(delegate()
			{
				hover.SetActive(false);
			});
		}

		// Token: 0x06009768 RID: 38760 RVA: 0x004689EE File Offset: 0x00466BEE
		protected override void OnEmptyGridBound(GameObject obj, PointerTrigger pointerTrigger)
		{
			pointerTrigger.EnterEvent.AddListener(new UnityAction(this.OnChildPointerEnter));
			pointerTrigger.ExitEvent.AddListener(new UnityAction(this.OnChildPointerExit));
		}

		// Token: 0x06009769 RID: 38761 RVA: 0x00468A24 File Offset: 0x00466C24
		protected override void OnLayoutSlotClicked()
		{
			bool flag = this._parentView == null;
			if (!flag)
			{
				this._parentView.CombatSkillLineItemSelected(this);
			}
		}

		// Token: 0x0600976A RID: 38762 RVA: 0x00468A54 File Offset: 0x00466C54
		private bool TrySelectLineFromChildClick()
		{
			bool flag = this.IsLineSelected || this._parentView == null;
			bool result;
			if (flag)
			{
				result = false;
			}
			else
			{
				this._parentView.CombatSkillLineItemSelected(this);
				result = true;
			}
			return result;
		}

		// Token: 0x0600976B RID: 38763 RVA: 0x00468A94 File Offset: 0x00466C94
		private void OnChildPointerEnter()
		{
			bool flag = this.IsLineSelected || this.enterGo == null;
			if (!flag)
			{
				this._childHoverRefCount++;
				this.enterGo.SetActive(true);
			}
		}

		// Token: 0x0600976C RID: 38764 RVA: 0x00468ADC File Offset: 0x00466CDC
		private void OnChildPointerExit()
		{
			bool flag = this.enterGo == null;
			if (!flag)
			{
				this._childHoverRefCount = Math.Max(0, this._childHoverRefCount - 1);
				bool flag2 = this._childHoverRefCount == 0 && !this.IsLineSelected;
				if (flag2)
				{
					this.enterGo.SetActive(false);
				}
			}
		}

		// Token: 0x0600976D RID: 38765 RVA: 0x00468B38 File Offset: 0x00466D38
		private void ResetEnterHoverState()
		{
			this._childHoverRefCount = 0;
			bool flag = this.enterGo != null && !this.IsLineSelected;
			if (flag)
			{
				this.enterGo.SetActive(false);
			}
		}

		// Token: 0x0600976E RID: 38766 RVA: 0x00468B78 File Offset: 0x00466D78
		private void SuppressChildSlotHoverVisuals()
		{
			bool flag = this.emptyGridRoot != null;
			if (flag)
			{
				for (int i = 0; i < this.emptyGridRoot.childCount; i++)
				{
					Transform child = this.emptyGridRoot.GetChild(i);
					bool flag2 = !child.gameObject.activeSelf || child.childCount <= 2;
					if (!flag2)
					{
						child.GetChild(2).gameObject.SetActive(false);
					}
				}
			}
			bool flag3 = this.skillRoot == null;
			if (!flag3)
			{
				for (int j = 0; j < this.skillRoot.childCount; j++)
				{
					Transform child2 = this.skillRoot.GetChild(j);
					bool flag4 = !child2.gameObject.activeSelf;
					if (!flag4)
					{
						EquipCombatSkillItem item;
						bool flag5 = child2.TryGetComponent<EquipCombatSkillItem>(out item);
						if (flag5)
						{
							item.CancelEquippedLineHover();
						}
					}
				}
			}
		}

		// Token: 0x0600976F RID: 38767 RVA: 0x00468C6C File Offset: 0x00466E6C
		public void OnSelected()
		{
			this.IsLineSelected = true;
			bool flag = this.enterGo != null;
			if (flag)
			{
				this.enterGo.SetActive(false);
			}
			this.clickedGo.SetActive(true);
		}

		// Token: 0x06009770 RID: 38768 RVA: 0x00468CAC File Offset: 0x00466EAC
		public void OnSelectedCancel()
		{
			this.IsLineSelected = false;
			this.clickedGo.SetActive(false);
			this.SuppressChildSlotHoverVisuals();
			this.ResetEnterHoverState();
		}

		// Token: 0x04007413 RID: 29715
		[SerializeField]
		private RectTransform typeRect;

		// Token: 0x04007414 RID: 29716
		[SerializeField]
		private RectTransform layoutRect;

		// Token: 0x04007415 RID: 29717
		[SerializeField]
		private GameObject enterGo;

		// Token: 0x04007416 RID: 29718
		[SerializeField]
		private GameObject clickedGo;

		// Token: 0x04007417 RID: 29719
		[SerializeField]
		private float designLineWidth = 543.92f;

		// Token: 0x04007418 RID: 29720
		[SerializeField]
		private float pivotCenterY;

		// Token: 0x04007419 RID: 29721
		private int _childHoverRefCount;
	}
}
