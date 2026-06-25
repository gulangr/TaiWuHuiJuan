using System;
using Config;
using FrameWork.UISystem.UIElements;
using JetBrains.Annotations;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Game.Views.World
{
	// Token: 0x0200072D RID: 1837
	public class StateMask : MonoBehaviour, IPointerEnterHandler, IEventSystemHandler, IPointerExitHandler
	{
		// Token: 0x060057D8 RID: 22488 RVA: 0x0028C61D File Offset: 0x0028A81D
		private void Awake()
		{
			this.referenceBase.alphaHitTestMinimumThreshold = this.raycastAlpha;
			this.OnPointerExitImpl();
		}

		// Token: 0x060057D9 RID: 22489 RVA: 0x0028C639 File Offset: 0x0028A839
		private void OnDisable()
		{
			this.OnPointerExitImpl();
		}

		// Token: 0x060057DA RID: 22490 RVA: 0x0028C644 File Offset: 0x0028A844
		public void ReadLocationFromConfig(MapStateItem config)
		{
			this.referenceBase.transform.localPosition = new Vector2(config.MaskPos[0], config.MaskPos[1]);
			this.hoverDelim.transform.localPosition = new Vector2(config.DelimPos[0], config.DelimPos[1]);
		}

		// Token: 0x060057DB RID: 22491 RVA: 0x0028C6A8 File Offset: 0x0028A8A8
		public void OnPointerEnter(bool isFromParent = true)
		{
			if (isFromParent)
			{
				this._fromParent = true;
			}
			else
			{
				this._fromSelf = true;
			}
			this.OnPointerEnterImpl();
		}

		// Token: 0x060057DC RID: 22492 RVA: 0x0028C6D4 File Offset: 0x0028A8D4
		public void OnPointerExit(bool isFromParent = true)
		{
			if (isFromParent)
			{
				this._fromParent = false;
			}
			else
			{
				this._fromSelf = false;
			}
			bool flag = !this._drag && !this._fromSelf && !this._fromParent;
			if (flag)
			{
				this.OnPointerExitImpl();
			}
		}

		// Token: 0x060057DD RID: 22493 RVA: 0x0028C71E File Offset: 0x0028A91E
		public void OnPointerEnter(PointerEventData _)
		{
			this.OnPointerEnter(false);
		}

		// Token: 0x060057DE RID: 22494 RVA: 0x0028C728 File Offset: 0x0028A928
		public void OnPointerExit(PointerEventData _)
		{
			this.OnPointerExit(false);
		}

		// Token: 0x060057DF RID: 22495 RVA: 0x0028C734 File Offset: 0x0028A934
		public void SetEnabled(bool enable)
		{
			base.gameObject.SetActive(enable);
			if (enable)
			{
				this.hoverDelim.gameObject.SetActive(this.hover.gameObject.activeSelf);
			}
			else
			{
				this.OnPointerExitImpl();
			}
		}

		// Token: 0x17000A95 RID: 2709
		// (get) Token: 0x060057E0 RID: 22496 RVA: 0x0028C77E File Offset: 0x0028A97E
		// (set) Token: 0x060057E1 RID: 22497 RVA: 0x0028C79C File Offset: 0x0028A99C
		public bool Drag
		{
			get
			{
				return (this._fromSelf || this._fromParent) && this._drag;
			}
			set
			{
				SingletonObject.getInstance<YieldHelper>().DelayFrameDo(1U, delegate
				{
					bool flag = !(this._drag = value) && !this._fromSelf && !this._fromParent;
					if (flag)
					{
						this.OnPointerExitImpl();
					}
				});
			}
		}

		// Token: 0x060057E2 RID: 22498 RVA: 0x0028C7D8 File Offset: 0x0028A9D8
		internal void OnPointerEnterImpl()
		{
			State state = this.state;
			if (state != null)
			{
				state.OnMaskHover(true);
			}
			this.hover.gameObject.SetActive(true);
			this.hoverDelim.gameObject.SetActive(base.gameObject.activeSelf);
		}

		// Token: 0x060057E3 RID: 22499 RVA: 0x0028C828 File Offset: 0x0028AA28
		internal void OnPointerExitImpl()
		{
			this._drag = false;
			this._fromParent = false;
			this._fromSelf = false;
			State state = this.state;
			if (state != null)
			{
				state.OnMaskHover(false);
			}
			this.hover.gameObject.SetActive(false);
			this.hoverDelim.gameObject.SetActive(false);
		}

		// Token: 0x04003C46 RID: 15430
		private const float X = 12800f;

		// Token: 0x04003C47 RID: 15431
		private const float Y = 8460f;

		// Token: 0x04003C48 RID: 15432
		[SerializeField]
		private RectTransform self;

		// Token: 0x04003C49 RID: 15433
		[SerializeField]
		private CImage referenceBase;

		// Token: 0x04003C4A RID: 15434
		[SerializeField]
		private CRawImage hover;

		// Token: 0x04003C4B RID: 15435
		[SerializeField]
		internal CRawImage hoverDelim;

		// Token: 0x04003C4C RID: 15436
		[SerializeField]
		private float raycastAlpha = 0.5f;

		// Token: 0x04003C4D RID: 15437
		[SerializeField]
		internal CButton button;

		// Token: 0x04003C4E RID: 15438
		[CanBeNull]
		[SerializeField]
		private State state;

		// Token: 0x04003C4F RID: 15439
		private bool _fromParent;

		// Token: 0x04003C50 RID: 15440
		private bool _fromSelf;

		// Token: 0x04003C51 RID: 15441
		private bool _drag;
	}
}
