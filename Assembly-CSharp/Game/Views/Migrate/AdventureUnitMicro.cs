using System;
using System.Collections.Generic;
using Coffee.UIExtensions;
using FrameWork;
using FrameWork.UISystem.Components;
using FrameWork.UISystem.UIElements;
using Game.Views.Adventure;
using GameData.Adventure;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

namespace Game.Views.Migrate
{
	// Token: 0x020008E3 RID: 2275
	public class AdventureUnitMicro : MonoBehaviour
	{
		// Token: 0x17000CB2 RID: 3250
		// (get) Token: 0x06006CDD RID: 27869 RVA: 0x0032380E File Offset: 0x00321A0E
		// (set) Token: 0x06006CDE RID: 27870 RVA: 0x00323816 File Offset: 0x00321A16
		public AdventureBlockIndex RenderBlockIndex
		{
			get
			{
				return this._renderBlockIndex;
			}
			set
			{
				this._renderBlockIndex = value;
				this.UpdateModifiers();
			}
		}

		// Token: 0x06006CDF RID: 27871 RVA: 0x00323828 File Offset: 0x00321A28
		private void UpdateModifiers()
		{
			bool flag = this.groundSurface != null;
			if (flag)
			{
				AdventureVertexModifier modifier = this.groundSurface.GetComponent<AdventureVertexModifier>();
				bool flag2 = modifier == null;
				if (flag2)
				{
					modifier = this.groundSurface.gameObject.AddComponent<AdventureVertexModifier>();
				}
				modifier.GridIndex = this._renderBlockIndex;
			}
			bool flag3 = this.cloud != null;
			if (flag3)
			{
				AdventureVertexModifier modifier2 = this.cloud.gameObject.GetOrAddComponent<AdventureVertexModifier>();
				modifier2.GridIndex = this._renderBlockIndex;
			}
			bool flag4 = this.deleteAnim != null;
			if (flag4)
			{
				AdventureVertexModifier modifier3 = this.deleteAnim.gameObject.GetOrAddComponent<AdventureVertexModifier>();
				modifier3.GridIndex = this._renderBlockIndex;
			}
			BlockVolumeController blockVolumeController = (this.volumeController != null) ? this.volumeController : base.GetComponent<BlockVolumeController>();
			bool flag5 = blockVolumeController != null;
			if (flag5)
			{
				blockVolumeController.SetGridIndex(this._renderBlockIndex);
			}
			this.UpdateExitButtonModifier(this._renderBlockIndex);
		}

		// Token: 0x06006CE0 RID: 27872 RVA: 0x0032392C File Offset: 0x00321B2C
		private void UpdateExitButtonModifier(AdventureBlockIndex index)
		{
			AdventureElementVertexModifier modifier = this.exitBtn.GetComponent<AdventureElementVertexModifier>();
			bool flag = modifier != null;
			if (flag)
			{
				modifier.GridIndex = index;
			}
		}

		// Token: 0x06006CE1 RID: 27873 RVA: 0x0032395C File Offset: 0x00321B5C
		private void Awake()
		{
			this.exitBtn.gameObject.SetActive(false);
			this.elementsHolder.gameObject.SetActive(false);
			this.deleteAnim.gameObject.SetActive(false);
			PointerTrigger pointerTrigger = this.button.GetComponent<PointerTrigger>();
			pointerTrigger.EnterEvent.ResetListener(delegate()
			{
				this.hover.gameObject.SetActive(true);
			});
			pointerTrigger.ExitEvent.ResetListener(delegate()
			{
				this.hover.gameObject.SetActive(false);
			});
		}

		// Token: 0x06006CE2 RID: 27874 RVA: 0x003239DC File Offset: 0x00321BDC
		private void OnEnable()
		{
			this.BindDragEvents();
			CButton cbutton = this.button;
			if (cbutton != null)
			{
				cbutton.onClick.RemoveAllListeners();
			}
			CButton cbutton2 = this.button;
			if (cbutton2 != null)
			{
				cbutton2.onClick.AddListener(delegate()
				{
					GEvent.OnEvent(UiEvents.AdventureUnitMicroClick, EasyPool.Get<ArgumentBox>().SetObject("AdventureUnitMicroIndex", this.RenderBlockIndex));
				});
			}
			CButton cbutton3 = this.exitBtn;
			if (cbutton3 != null)
			{
				cbutton3.onClick.RemoveAllListeners();
			}
			CButton cbutton4 = this.exitBtn;
			if (cbutton4 != null)
			{
				cbutton4.onClick.AddListener(delegate()
				{
					GEvent.OnEvent(UiEvents.AdventureUnitMicroClick, EasyPool.Get<ArgumentBox>().SetObject("AdventureUnitMicroIndex", this.RenderBlockIndex));
				});
			}
			DelayPointerTrigger influencePointerTrigger = this.influenceIcon.GetComponent<DelayPointerTrigger>();
			influencePointerTrigger.enterEvent.ResetListener(new Action(this.OnPointerEnter));
			influencePointerTrigger.exitEvent.ResetListener(new Action(this.OnPointerExit));
		}

		// Token: 0x06006CE3 RID: 27875 RVA: 0x00323AA1 File Offset: 0x00321CA1
		private void OnDisable()
		{
			this.UnbindDragEvents();
			this.deleteAnim.gameObject.SetActive(false);
		}

		// Token: 0x06006CE4 RID: 27876 RVA: 0x00323AC0 File Offset: 0x00321CC0
		private void BindDragEvents()
		{
			CInputEventImage inputGroundSurface = this.groundSurface as CInputEventImage;
			bool flag = inputGroundSurface == null;
			if (!flag)
			{
				UIRectDragMove dragMove = base.GetComponentInParent<UIRectDragMove>();
				bool flag2 = dragMove == null;
				if (!flag2)
				{
					bool flag3 = !AdventureUnitMicro.HasPersistentListener(inputGroundSurface.CallbackOnBeginDrag, dragMove, "OnBeginDrag");
					if (flag3)
					{
						inputGroundSurface.CallbackOnBeginDrag.AddListener(new UnityAction<PointerEventData>(dragMove.OnBeginDrag));
					}
					bool flag4 = !AdventureUnitMicro.HasPersistentListener(inputGroundSurface.CallbackOnDrag, dragMove, "OnDrag");
					if (flag4)
					{
						inputGroundSurface.CallbackOnDrag.AddListener(new UnityAction<PointerEventData>(dragMove.OnDrag));
					}
					bool flag5 = !AdventureUnitMicro.HasPersistentListener(inputGroundSurface.CallbackOnEndDrag, dragMove, "OnEndDrag");
					if (flag5)
					{
						inputGroundSurface.CallbackOnEndDrag.AddListener(new UnityAction<PointerEventData>(dragMove.OnEndDrag));
					}
				}
			}
		}

		// Token: 0x06006CE5 RID: 27877 RVA: 0x00323BA0 File Offset: 0x00321DA0
		private void UnbindDragEvents()
		{
			CInputEventImage inputGroundSurface = this.groundSurface as CInputEventImage;
			bool flag = inputGroundSurface == null;
			if (!flag)
			{
				UIRectDragMove dragMove = base.GetComponentInParent<UIRectDragMove>();
				bool flag2 = dragMove == null;
				if (!flag2)
				{
					inputGroundSurface.CallbackOnBeginDrag.RemoveListener(new UnityAction<PointerEventData>(dragMove.OnBeginDrag));
					inputGroundSurface.CallbackOnDrag.RemoveListener(new UnityAction<PointerEventData>(dragMove.OnDrag));
					inputGroundSurface.CallbackOnEndDrag.RemoveListener(new UnityAction<PointerEventData>(dragMove.OnEndDrag));
				}
			}
		}

		// Token: 0x06006CE6 RID: 27878 RVA: 0x00323C28 File Offset: 0x00321E28
		private static bool HasPersistentListener(UnityEventBase eventBase, Object target, string methodName)
		{
			int listenerCount = eventBase.GetPersistentEventCount();
			for (int i = 0; i < listenerCount; i++)
			{
				bool flag = eventBase.GetPersistentTarget(i) == target && eventBase.GetPersistentMethodName(i) == methodName;
				if (flag)
				{
					return true;
				}
			}
			return false;
		}

		// Token: 0x06006CE7 RID: 27879 RVA: 0x00323C80 File Offset: 0x00321E80
		public void SetExit(bool isExit, bool atExit)
		{
			this.exitBtn.gameObject.SetActive(isExit);
			if (isExit)
			{
				this.exitBtn.GetComponent<CImage>().SetSprite(atExit ? "adventure_element_door_01" : "adventure_element_door_02", false, null);
				this.UpdateExitButtonModifier(this._renderBlockIndex);
			}
		}

		// Token: 0x06006CE8 RID: 27880 RVA: 0x00323CD6 File Offset: 0x00321ED6
		public void SetPointerTriggerEnable(bool enable)
		{
			this.button.GetComponent<PointerTrigger>().enabled = enable;
		}

		// Token: 0x06006CE9 RID: 27881 RVA: 0x00323CEB File Offset: 0x00321EEB
		private void OnPointerEnter()
		{
			GEvent.OnEvent(UiEvents.AdventureUnitMicroPointerEnter, EasyPool.Get<ArgumentBox>().SetObject("AdventureUnitMicroIndex", this.RenderBlockIndex));
		}

		// Token: 0x06006CEA RID: 27882 RVA: 0x00323D18 File Offset: 0x00321F18
		private void OnPointerExit()
		{
			GEvent.OnEvent(UiEvents.AdventureUnitMicroPointerExit, EasyPool.Get<ArgumentBox>().SetObject("AdventureUnitMicroIndex", this.RenderBlockIndex));
		}

		// Token: 0x06006CEB RID: 27883 RVA: 0x00323D48 File Offset: 0x00321F48
		public void SetDecorate(List<string> decorates)
		{
			this.blockDecoratesHolder.gameObject.SetActive(decorates != null && decorates.Count > 0);
			bool flag = decorates != null && decorates.Count > 0;
			if (flag)
			{
				this.blockDecoratesHolder.Rebuild<RectTransform>(decorates.Count, delegate(RectTransform goDecorate, int index)
				{
					goDecorate.GetComponent<RectTransform>().localPosition = Vector3.zero;
					CImage imgDecorate = goDecorate.GetComponent<AdventureRemakeDecorateTempInfo>().imgDecorate;
					imgDecorate.SetSprite(decorates[index], false, null);
					AdventureVertexModifier modifier = imgDecorate.gameObject.GetOrAddComponent<AdventureVertexModifier>();
					modifier.GridIndex = this._renderBlockIndex;
				});
			}
		}

		// Token: 0x06006CEC RID: 27884 RVA: 0x00323DD6 File Offset: 0x00321FD6
		public void HideDecorates()
		{
			this.blockDecoratesCanvasGroup.alpha = 0f;
		}

		// Token: 0x06006CED RID: 27885 RVA: 0x00323DEA File Offset: 0x00321FEA
		public void RecoverDecorates()
		{
			this.blockDecoratesCanvasGroup.alpha = 1f;
		}

		// Token: 0x06006CEE RID: 27886 RVA: 0x00323DFE File Offset: 0x00321FFE
		public void UpdateParticleHoldersActiveState()
		{
			AdventureUnitMicro.UpdateHolderState(this.microBlockParticleHolderDown);
			AdventureUnitMicro.UpdateHolderState(this.microBlockParticleHolderTop);
			AdventureUnitMicro.UpdateHolderState(this.microBlockParticleHolderSingle);
		}

		// Token: 0x06006CEF RID: 27887 RVA: 0x00323E28 File Offset: 0x00322028
		private static void UpdateHolderState(UIParticle holder)
		{
			bool flag = holder != null;
			if (flag)
			{
				holder.enabled = (holder.transform.childCount > 0);
			}
		}

		// Token: 0x06006CF0 RID: 27888 RVA: 0x00323E58 File Offset: 0x00322058
		public void SetDialogPosY()
		{
			this.dialogSlot.localPosition = this.dialogSlot.localPosition.SetY((this.paramProgressTempInfo.gameObject.activeSelf || this.alertAnimPos.activeSelf) ? 128f : 83f);
		}

		// Token: 0x04004F11 RID: 20241
		[SerializeField]
		public CImage groundSurface;

		// Token: 0x04004F12 RID: 20242
		[SerializeField]
		public CImage cloud;

		// Token: 0x04004F13 RID: 20243
		[SerializeField]
		public CImage influenceIcon;

		// Token: 0x04004F14 RID: 20244
		[SerializeField]
		public CImage deleteAnim;

		// Token: 0x04004F15 RID: 20245
		[SerializeField]
		public RectTransform selectedHolder;

		// Token: 0x04004F16 RID: 20246
		[SerializeField]
		public RectTransform jumpTarget;

		// Token: 0x04004F17 RID: 20247
		[SerializeField]
		public TemplatedContainerAssemblyNew elementsHolder;

		// Token: 0x04004F18 RID: 20248
		[SerializeField]
		public CButton button;

		// Token: 0x04004F19 RID: 20249
		[SerializeField]
		public CButton exitBtn;

		// Token: 0x04004F1A RID: 20250
		[SerializeField]
		public CanvasGroup alertAnim;

		// Token: 0x04004F1B RID: 20251
		[SerializeField]
		public CanvasGroup blockDecoratesCanvasGroup;

		// Token: 0x04004F1C RID: 20252
		[SerializeField]
		public TemplatedContainerAssemblyNew blockDecoratesHolder;

		// Token: 0x04004F1D RID: 20253
		[SerializeField]
		public TemplatedContainerAssemblyNew paramStateHolder;

		// Token: 0x04004F1E RID: 20254
		[SerializeField]
		public RectTransform dialogSlot;

		// Token: 0x04004F1F RID: 20255
		[SerializeField]
		public RectTransform overlapSlot;

		// Token: 0x04004F20 RID: 20256
		[SerializeField]
		public GameObject elementRoot;

		// Token: 0x04004F21 RID: 20257
		[SerializeField]
		public GameObject alertAnimPos;

		// Token: 0x04004F22 RID: 20258
		[SerializeField]
		public GameObject hover;

		// Token: 0x04004F23 RID: 20259
		[SerializeField]
		public GameObject element;

		// Token: 0x04004F24 RID: 20260
		[SerializeField]
		public UIParticle microBlockParticleHolderDown;

		// Token: 0x04004F25 RID: 20261
		[SerializeField]
		public UIParticle microBlockParticleHolderTop;

		// Token: 0x04004F26 RID: 20262
		[SerializeField]
		public UIParticle microBlockParticleHolderSingle;

		// Token: 0x04004F27 RID: 20263
		[SerializeField]
		public AdventureRemakeParamProgressTempInfo paramProgressTempInfo;

		// Token: 0x04004F28 RID: 20264
		[SerializeField]
		public BlockVolumeController volumeController;

		// Token: 0x04004F29 RID: 20265
		[SerializeField]
		public AdventurePointLight microPointLight;

		// Token: 0x04004F2A RID: 20266
		private AdventureBlockIndex _renderBlockIndex;

		// Token: 0x04004F2B RID: 20267
		private const float DialogPosY0 = 83f;

		// Token: 0x04004F2C RID: 20268
		private const float DialogPosY1 = 128f;
	}
}
