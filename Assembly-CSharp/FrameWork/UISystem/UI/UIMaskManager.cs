using System;
using System.Collections.Generic;
using UnityEngine;

namespace FrameWork.UISystem.UI
{
	// Token: 0x02000FF7 RID: 4087
	public class UIMaskManager : MonoBehaviour, ISingletonInit, IDisposable
	{
		// Token: 0x0600BA7A RID: 47738 RVA: 0x0054EF94 File Offset: 0x0054D194
		public void LegacyRegister(UIMask mask)
		{
			bool flag = this._legacyAssigned.ContainsKey(mask) || this._legacyMasksWithoutSlot.Contains(mask);
			if (!flag)
			{
				bool flag2 = this._legacyAvailableSlots.Count == 0;
				if (flag2)
				{
					bool flag3 = this._legacyMasksWithSlot.Count > 0;
					if (flag3)
					{
						UIMask oldest = this._legacyMasksWithSlot.First.Value;
						UIMaskManager.LegacySlotInfo slot = this._legacyAssigned[oldest];
						this._legacyAssigned.Remove(oldest);
						this._legacyMasksWithSlot.RemoveFirst();
						oldest.AssignGrab(null, null);
						this._legacyMasksWithoutSlot.Add(oldest);
						this._legacyAssigned[mask] = slot;
						this._legacyMasksWithSlot.AddLast(mask);
						mask.AssignGrab(slot.Mat, slot.GrabName);
						mask.DoProcess();
					}
					else
					{
						this._legacyMasksWithoutSlot.Add(mask);
					}
				}
				else
				{
					UIMaskManager.LegacySlotInfo availableSlot = this._legacyAvailableSlots.Dequeue();
					this._legacyAssigned[mask] = availableSlot;
					this._legacyMasksWithSlot.AddLast(mask);
					mask.AssignGrab(availableSlot.Mat, availableSlot.GrabName);
					mask.DoProcess();
				}
			}
		}

		// Token: 0x0600BA7B RID: 47739 RVA: 0x0054F0D4 File Offset: 0x0054D2D4
		public void LegacyUnregister(UIMask mask)
		{
			UIMaskManager.LegacySlotInfo slot;
			bool flag = this._legacyAssigned.TryGetValue(mask, out slot);
			if (flag)
			{
				this._legacyAssigned.Remove(mask);
				this._legacyMasksWithSlot.Remove(mask);
				mask.AssignGrab(null, null);
				bool flag2 = this._legacyMasksWithoutSlot.Count > 0;
				if (flag2)
				{
					UIMask waiting = this._legacyMasksWithoutSlot[0];
					this._legacyMasksWithoutSlot.RemoveAt(0);
					this._legacyAssigned[waiting] = slot;
					this._legacyMasksWithSlot.AddLast(waiting);
					waiting.AssignGrab(slot.Mat, slot.GrabName);
					waiting.DoProcess();
				}
				else
				{
					this._legacyAvailableSlots.Enqueue(slot);
				}
			}
			else
			{
				bool flag3 = this._legacyMasksWithoutSlot.Contains(mask);
				if (flag3)
				{
					this._legacyMasksWithoutSlot.Remove(mask);
				}
			}
		}

		// Token: 0x0600BA7C RID: 47740 RVA: 0x0054F1B0 File Offset: 0x0054D3B0
		private void CreateLegacySlots()
		{
			for (int i = 1; i < UIMaskManager.ShaderNames.Length; i++)
			{
				Shader shader = Shader.Find(UIMaskManager.ShaderNames[i]);
				bool flag = shader == null;
				if (flag)
				{
					Debug.LogError("UIMaskManager: Cannot find legacy shader \"" + UIMaskManager.ShaderNames[i] + "\"");
				}
				else
				{
					Material mat = new Material(shader)
					{
						name = string.Format("UIMaskGrab{0}_Legacy_Generated", i + 1)
					};
					UIMaskManager.LegacySlotInfo info = new UIMaskManager.LegacySlotInfo
					{
						Mat = mat,
						GrabName = UIMaskManager.GrabTextureNames[i]
					};
					this._legacyAllSlots.Add(info);
					this._legacyAvailableSlots.Enqueue(info);
				}
			}
		}

		// Token: 0x0600BA7D RID: 47741 RVA: 0x0054F274 File Offset: 0x0054D474
		private void CleanupLegacySlots()
		{
			foreach (UIMaskManager.LegacySlotInfo slot in this._legacyAllSlots)
			{
				bool flag = slot.Mat != null;
				if (flag)
				{
					bool isPlaying = Application.isPlaying;
					if (isPlaying)
					{
						Object.Destroy(slot.Mat);
					}
					else
					{
						Object.DestroyImmediate(slot.Mat);
					}
				}
			}
			this._legacyAllSlots.Clear();
			this._legacyAvailableSlots.Clear();
			this._legacyMasksWithSlot.Clear();
			this._legacyMasksWithoutSlot.Clear();
			this._legacyAssigned.Clear();
		}

		// Token: 0x0600BA7E RID: 47742 RVA: 0x0054F338 File Offset: 0x0054D538
		public void SetSharedMaskInstanceActive(UIElement checker, bool active)
		{
			bool flag = checker != null && !UIManager.Instance.IsFocusElement(checker);
			if (!flag)
			{
				this._sharedMaskInstance.SetGrabVisible(active);
				bool flag2 = !active;
				if (flag2)
				{
					this._sharedMaskInstance.SetOutputVisible(false);
				}
			}
		}

		// Token: 0x0600BA7F RID: 47743 RVA: 0x0054F384 File Offset: 0x0054D584
		public void PushNoMaskState()
		{
			bool hasRealOwner = false;
			foreach (Transform owner in this._maskOwnerList)
			{
				bool flag = owner != null;
				if (flag)
				{
					hasRealOwner = true;
					break;
				}
			}
			bool flag2 = !hasRealOwner;
			if (!flag2)
			{
				this._maskOwnerList.Add(null);
				this.CacheCurrentBlurForOwner(this.GetCurrentRealOwner());
				this.HideSharedMask();
				this.HideTransitionDisplay();
			}
		}

		// Token: 0x0600BA80 RID: 47744 RVA: 0x0054F41C File Offset: 0x0054D61C
		public void PopNoMaskState()
		{
			bool flag;
			if (this._maskOwnerList.Count > 0)
			{
				List<Transform> maskOwnerList = this._maskOwnerList;
				flag = (maskOwnerList[maskOwnerList.Count - 1] == null);
			}
			else
			{
				flag = false;
			}
			bool flag2 = flag;
			if (flag2)
			{
				this._maskOwnerList.RemoveAt(this._maskOwnerList.Count - 1);
				this.ReturnToStackTop();
			}
		}

		// Token: 0x0600BA81 RID: 47745 RVA: 0x0054F478 File Offset: 0x0054D678
		public void AttachMaskTo(Transform target)
		{
			bool flag = target == null;
			if (!flag)
			{
				this.EnsureMaskInstance();
				this.CleanupInvalidOwners();
				bool flag2 = this._maskOwnerList.Count > 0 && this._maskOwnerList.Contains(target);
				if (!flag2)
				{
					this.CacheCurrentBlurForOwner(this.GetCurrentRealOwner());
					this._maskOwnerList.Add(target);
					this.MoveMaskToTarget(target, true);
				}
			}
		}

		// Token: 0x0600BA82 RID: 47746 RVA: 0x0054F4E8 File Offset: 0x0054D6E8
		public void DetachMask(Transform target)
		{
			bool flag = this._maskOwnerList.Count == 0;
			if (!flag)
			{
				bool flag2 = target == null;
				if (flag2)
				{
					int beforeCount = this._maskOwnerList.Count;
					this.CleanupInvalidOwners();
					bool flag3 = this._maskOwnerList.Count != beforeCount;
					if (flag3)
					{
						this.ReturnToStackTop();
					}
				}
				else
				{
					bool removed = this._maskOwnerList.Remove(target);
					bool flag4 = !removed;
					if (flag4)
					{
						int beforeCount2 = this._maskOwnerList.Count;
						this.CleanupInvalidOwners();
						bool flag5 = this._maskOwnerList.Count != beforeCount2;
						if (flag5)
						{
							this.ReturnToStackTop();
						}
					}
					else
					{
						this.CleanupInvalidOwners();
						this.ReleaseOwnerBlurCache(target);
						this.ReturnToStackTop();
					}
				}
			}
		}

		// Token: 0x0600BA83 RID: 47747 RVA: 0x0054F5B4 File Offset: 0x0054D7B4
		public void NotifyOwnerSortingChanged(Transform owner)
		{
			bool flag = this._sharedMaskInstance == null || owner == null;
			if (!flag)
			{
				Transform currentOwner = this.GetCurrentRealOwner();
				bool flag2 = currentOwner != owner;
				if (!flag2)
				{
					Canvas maskCanvas = this._sharedMaskInstance.GetComponent<Canvas>();
					UIMaskManager.ApplyCanvasSorting(maskCanvas, owner);
				}
			}
		}

		// Token: 0x0600BA84 RID: 47748 RVA: 0x0054F608 File Offset: 0x0054D808
		private void ReturnToStackTop()
		{
			bool flag = this._maskOwnerList.Count == 0;
			if (flag)
			{
				this.HideSharedMask();
				this.HideTransitionDisplay();
			}
			else
			{
				List<Transform> maskOwnerList = this._maskOwnerList;
				Transform topOwner = maskOwnerList[maskOwnerList.Count - 1];
				bool flag2 = topOwner == null;
				if (flag2)
				{
					this.HideSharedMask();
					this.HideTransitionDisplay();
				}
				else
				{
					this.HideTransitionDisplay();
					this.MoveMaskToTarget(topOwner, false);
				}
			}
		}

		// Token: 0x0600BA85 RID: 47749 RVA: 0x0054F67C File Offset: 0x0054D87C
		private void HideSharedMask()
		{
			bool flag = this._sharedMaskInstance == null;
			if (!flag)
			{
				this._sharedMaskInstance.SetGrabVisible(false);
				this._sharedMaskInstance.SetOutputVisible(false);
			}
		}

		// Token: 0x0600BA86 RID: 47750 RVA: 0x0054F6B8 File Offset: 0x0054D8B8
		public void MoveMaskToSafeParent()
		{
			bool flag = this._sharedMaskInstance == null;
			if (!flag)
			{
				foreach (Transform owner in this._maskOwnerList)
				{
					bool flag2 = owner != null;
					if (flag2)
					{
						return;
					}
				}
				Transform current = this._sharedMaskInstance.transform.parent;
				while (current != null)
				{
					bool flag3 = current.GetComponent<UIBase>() != null;
					if (flag3)
					{
						this._sharedMaskInstance.transform.SetParent(current.parent, false);
						this._sharedMaskInstance.transform.SetAsLastSibling();
						break;
					}
					current = current.parent;
				}
			}
		}

		// Token: 0x0600BA87 RID: 47751 RVA: 0x0054F798 File Offset: 0x0054D998
		private void CleanupInvalidOwners()
		{
			List<Transform> invalidOwners = new List<Transform>();
			foreach (Transform owner in this._ownerBlurCacheDict.Keys)
			{
				bool flag = owner == null && owner != null;
				if (flag)
				{
					invalidOwners.Add(owner);
				}
			}
			foreach (Transform owner2 in invalidOwners)
			{
				this.ReleaseOwnerBlurCache(owner2);
			}
			this._maskOwnerList.RemoveAll((Transform t) => t == null && t != null);
		}

		// Token: 0x0600BA88 RID: 47752 RVA: 0x0054F880 File Offset: 0x0054DA80
		private void MoveMaskToTarget(Transform target, bool useSourceTransitionClone)
		{
			bool flag = this._sharedMaskInstance == null || target == null;
			if (!flag)
			{
				Transform maskTransform = this._sharedMaskInstance.transform;
				Transform previousParent = maskTransform.parent;
				int previousSiblingIndex = maskTransform.GetSiblingIndex();
				Transform targetParent = target.parent;
				int targetIndex = target.GetSiblingIndex();
				bool flag2 = previousParent == targetParent && previousSiblingIndex < targetIndex;
				if (flag2)
				{
					targetIndex--;
				}
				maskTransform.SetParent(targetParent, false);
				maskTransform.SetSiblingIndex(targetIndex);
				if (useSourceTransitionClone)
				{
					this._sharedMaskInstance.InvalidateDisplay();
					this._sharedMaskInstance.SetOnePassBlurVisible(true);
				}
				this._sharedMaskInstance.SetGrabVisible(true);
				Canvas maskCanvas = this._sharedMaskInstance.GetComponent<Canvas>();
				UIMaskManager.ApplyCanvasSorting(maskCanvas, target);
				UIMaskManager.EnsureFillScreen(maskTransform as RectTransform);
				if (useSourceTransitionClone)
				{
					this._sharedMaskInstance.DoProcess();
				}
				else
				{
					Transform currentOwner = this.GetCurrentRealOwner();
					UIMaskManager.OwnerBlurCache cache;
					bool flag3 = currentOwner != null && this._sharedMaskInstance.CurrentBlurTexture != null && this._ownerBlurCacheDict.TryGetValue(currentOwner, out cache) && cache.Texture != null;
					if (flag3)
					{
						Graphics.Blit(cache.Texture, this._sharedMaskInstance.CurrentBlurTexture);
						this._sharedMaskInstance.SetOutputVisible(true);
					}
					this._skipSharedDoProcess = true;
				}
			}
		}

		// Token: 0x0600BA89 RID: 47753 RVA: 0x0054F9F0 File Offset: 0x0054DBF0
		public void HideTransitionDisplay()
		{
			bool flag = this._sharedMaskInstance != null;
			if (flag)
			{
				this._sharedMaskInstance.SetOnePassBlurVisible(false);
			}
		}

		// Token: 0x0600BA8A RID: 47754 RVA: 0x0054FA20 File Offset: 0x0054DC20
		private static void EnsureFillScreen(RectTransform maskRect)
		{
			bool flag = maskRect == null;
			if (!flag)
			{
				Canvas canvas = (maskRect.parent != null) ? maskRect.parent.GetComponentInParent<Canvas>() : null;
				bool flag2 = canvas == null;
				if (!flag2)
				{
					RectTransform canvasRect = canvas.GetComponent<RectTransform>();
					maskRect.anchorMin = new Vector2(0.5f, 0.5f);
					maskRect.anchorMax = new Vector2(0.5f, 0.5f);
					maskRect.pivot = new Vector2(0.5f, 0.5f);
					maskRect.position = canvasRect.TransformPoint(canvasRect.rect.center);
					maskRect.rotation = canvasRect.rotation;
					Vector3 canvasScale = canvasRect.lossyScale;
					Vector3 parentScale = (maskRect.parent != null) ? maskRect.parent.lossyScale : Vector3.one;
					float safeParentX = Mathf.Approximately(parentScale.x, 0f) ? 1f : parentScale.x;
					float safeParentY = Mathf.Approximately(parentScale.y, 0f) ? 1f : parentScale.y;
					float safeParentZ = Mathf.Approximately(parentScale.z, 0f) ? 1f : parentScale.z;
					maskRect.localScale = new Vector3(canvasScale.x / safeParentX, canvasScale.y / safeParentY, canvasScale.z / safeParentZ);
				}
			}
		}

		// Token: 0x0600BA8B RID: 47755 RVA: 0x0054FB98 File Offset: 0x0054DD98
		private static void SetStretchFill(RectTransform rect)
		{
			bool flag = rect == null;
			if (!flag)
			{
				rect.anchorMin = Vector2.zero;
				rect.anchorMax = Vector2.one;
				rect.pivot = new Vector2(0.5f, 0.5f);
				rect.anchoredPosition = Vector2.zero;
				rect.sizeDelta = Vector2.zero;
				rect.offsetMin = Vector2.zero;
				rect.offsetMax = Vector2.zero;
				rect.localScale = Vector3.one;
				rect.localRotation = Quaternion.identity;
			}
		}

		// Token: 0x0600BA8C RID: 47756 RVA: 0x0054FC2C File Offset: 0x0054DE2C
		private Transform GetCurrentRealOwner()
		{
			for (int i = this._maskOwnerList.Count - 1; i >= 0; i--)
			{
				bool flag = this._maskOwnerList[i] != null;
				if (flag)
				{
					return this._maskOwnerList[i];
				}
			}
			return null;
		}

		// Token: 0x0600BA8D RID: 47757 RVA: 0x0054FC88 File Offset: 0x0054DE88
		private void CacheCurrentBlurForOwner(Transform owner)
		{
			bool flag = owner == null || this._sharedMaskInstance == null || !this._sharedMaskInstance.IsDisplayingBlur;
			if (!flag)
			{
				RenderTexture sourceRT = this._sharedMaskInstance.CurrentBlurTexture;
				bool flag2 = sourceRT == null;
				if (!flag2)
				{
					UIMaskManager.OwnerBlurCache cache;
					bool flag3 = !this._ownerBlurCacheDict.TryGetValue(owner, out cache);
					if (flag3)
					{
						cache = new UIMaskManager.OwnerBlurCache();
						this._ownerBlurCacheDict[owner] = cache;
					}
					bool flag4 = cache.Texture == null || cache.Texture.width != sourceRT.width || cache.Texture.height != sourceRT.height || cache.Texture.format != sourceRT.format;
					if (flag4)
					{
						UIMaskManager.ReleaseRenderTexture(ref cache.Texture);
						cache.Texture = new RenderTexture(sourceRT.width, sourceRT.height, 0, sourceRT.format)
						{
							name = "UIMaskOwnerBackup_" + owner.name,
							filterMode = FilterMode.Bilinear,
							wrapMode = TextureWrapMode.Clamp
						};
					}
					Graphics.Blit(sourceRT, cache.Texture);
				}
			}
		}

		// Token: 0x0600BA8E RID: 47758 RVA: 0x0054FDC4 File Offset: 0x0054DFC4
		private void ReleaseOwnerBlurCache(Transform owner)
		{
			UIMaskManager.OwnerBlurCache cache;
			bool flag = owner == null || !this._ownerBlurCacheDict.TryGetValue(owner, out cache);
			if (!flag)
			{
				UIMaskManager.ReleaseRenderTexture(ref cache.Texture);
				this._ownerBlurCacheDict.Remove(owner);
			}
		}

		// Token: 0x0600BA8F RID: 47759 RVA: 0x0054FE08 File Offset: 0x0054E008
		private static void ReleaseRenderTexture(ref RenderTexture texture)
		{
			bool flag = texture == null;
			if (!flag)
			{
				texture.Release();
				bool isPlaying = Application.isPlaying;
				if (isPlaying)
				{
					Object.Destroy(texture);
				}
				else
				{
					Object.DestroyImmediate(texture);
				}
				texture = null;
			}
		}

		// Token: 0x0600BA90 RID: 47760 RVA: 0x0054FE4C File Offset: 0x0054E04C
		private static void ApplyCanvasSorting(Canvas canvas, Transform target)
		{
			Canvas targetCanvas = target.GetComponent<Canvas>();
			bool flag = targetCanvas != null && targetCanvas.overrideSorting;
			if (flag)
			{
				canvas.overrideSorting = true;
				canvas.sortingLayerID = targetCanvas.sortingLayerID;
				canvas.sortingOrder = targetCanvas.sortingOrder - 1;
			}
			else
			{
				canvas.overrideSorting = false;
				canvas.sortingOrder = 0;
				canvas.sortingLayerID = 0;
			}
		}

		// Token: 0x0600BA91 RID: 47761 RVA: 0x0054FEB9 File Offset: 0x0054E0B9
		public bool IsMaskedElement(UIElement element)
		{
			return this._maskedElements.Contains(element);
		}

		// Token: 0x0600BA92 RID: 47762 RVA: 0x0054FEC7 File Offset: 0x0054E0C7
		public void RegisterMaskedElement(UIElement element)
		{
			this._maskedElements.Add(element);
		}

		// Token: 0x0600BA93 RID: 47763 RVA: 0x0054FED6 File Offset: 0x0054E0D6
		public void UnregisterMaskedElement(UIElement element)
		{
			this._maskedElements.Remove(element);
		}

		// Token: 0x0600BA94 RID: 47764 RVA: 0x0054FEE5 File Offset: 0x0054E0E5
		public void CleanupDestroyedMaskedElements()
		{
			this._maskedElements.RemoveWhere((UIElement elem) => elem == null || elem.UiBase == null);
		}

		// Token: 0x0600BA95 RID: 47765 RVA: 0x0054FF14 File Offset: 0x0054E114
		private void EnsureMaskInstance()
		{
			bool flag = this._sharedMaskInstance != null;
			if (!flag)
			{
				this.CreateSharedGrabMaterial();
				ResLoader.Load<GameObject>("RemakeResources/Prefab/Components/Common/UIMask", delegate(GameObject prefab)
				{
					bool flag2 = prefab == null;
					if (flag2)
					{
						Debug.LogError("UIMaskManager: Failed to load UIMask prefab from RemakeResources/Prefab/Components/Common/UIMask");
					}
					else
					{
						GameObject instance = Object.Instantiate<GameObject>(prefab);
						instance.name = "SharedUIMask";
						this._sharedMaskInstance = instance.GetComponent<UIMask>();
						this._sharedMaskInstance.MarkAsSharedManaged();
						this._sharedMaskInstance.AssignGrab(this._sharedGrabMaterial, this._sharedGrabTextureName);
						this._sharedMaskInstance.SetGrabVisible(false);
						this._sharedMaskInstance.SetOutputVisible(false);
						this.CleanupInvalidOwners();
						this.CleanupDestroyedMaskedElements();
						bool flag3 = this._maskOwnerList.Count > 0;
						if (flag3)
						{
							List<Transform> maskOwnerList = this._maskOwnerList;
							this.MoveMaskToTarget(maskOwnerList[maskOwnerList.Count - 1], false);
						}
					}
				}, null, false);
			}
		}

		// Token: 0x0600BA96 RID: 47766 RVA: 0x0054FF54 File Offset: 0x0054E154
		public void Init()
		{
			this.CreateSharedGrabMaterial();
			this.CreateLegacySlots();
		}

		// Token: 0x0600BA97 RID: 47767 RVA: 0x0054FF68 File Offset: 0x0054E168
		public void OnUIBaseHidden(Transform uiBaseTransform)
		{
			bool flag = this._sharedMaskInstance == null || uiBaseTransform == null;
			if (!flag)
			{
				Transform maskTransform = this._sharedMaskInstance.transform;
				bool flag2 = !maskTransform.IsChildOf(uiBaseTransform);
				if (!flag2)
				{
					Debug.LogWarning("[Mask] Triggered Auto clean up for MaskComponent. uiBase: " + uiBaseTransform.name + ".\nUnMaskComponent SHOULD be called before parent uibase close");
					this.CleanupInvalidOwners();
					for (int i = this._maskOwnerList.Count - 1; i >= 0; i--)
					{
						Transform owner = this._maskOwnerList[i];
						bool flag3 = owner != null && owner.IsChildOf(uiBaseTransform);
						if (flag3)
						{
							this.ReleaseOwnerBlurCache(owner);
							this._maskOwnerList.RemoveAt(i);
						}
					}
					this.MoveMaskToSafeParent();
				}
			}
		}

		// Token: 0x0600BA98 RID: 47768 RVA: 0x00550044 File Offset: 0x0054E244
		public void CleanupSharedMaskInstance()
		{
			bool flag = this._sharedMaskInstance != null;
			if (flag)
			{
				this._sharedMaskInstance.SetGrabVisible(false);
				this._sharedMaskInstance.SetOutputVisible(false);
				this._sharedMaskInstance.SetOnePassBlurVisible(false);
				Object.Destroy(this._sharedMaskInstance.gameObject);
				this._sharedMaskInstance = null;
			}
			this._maskOwnerList.Clear();
			List<Transform> ownerList = new List<Transform>(this._ownerBlurCacheDict.Keys);
			foreach (Transform owner in ownerList)
			{
				this.ReleaseOwnerBlurCache(owner);
			}
		}

		// Token: 0x0600BA99 RID: 47769 RVA: 0x00550104 File Offset: 0x0054E304
		public void Dispose()
		{
			this.CleanupSharedMaskInstance();
			this.CleanupSharedGrabMaterial();
			this.CleanupLegacySlots();
		}

		// Token: 0x0600BA9A RID: 47770 RVA: 0x0055011C File Offset: 0x0054E31C
		private void CreateSharedGrabMaterial()
		{
			bool flag = this._sharedGrabMaterial != null;
			if (!flag)
			{
				Shader shader = Shader.Find(UIMaskManager.ShaderNames[0]);
				bool flag2 = shader == null;
				if (flag2)
				{
					Debug.LogError("UIMaskManager: Cannot find shader \"" + UIMaskManager.ShaderNames[0] + "\"");
				}
				else
				{
					this._sharedGrabMaterial = new Material(shader)
					{
						name = "UIMaskGrab1_Shared_Generated"
					};
					this._sharedGrabTextureName = UIMaskManager.GrabTextureNames[0];
				}
			}
		}

		// Token: 0x0600BA9B RID: 47771 RVA: 0x00550198 File Offset: 0x0054E398
		private void CleanupSharedGrabMaterial()
		{
			bool flag = this._sharedGrabMaterial != null;
			if (flag)
			{
				bool isPlaying = Application.isPlaying;
				if (isPlaying)
				{
					Object.Destroy(this._sharedGrabMaterial);
				}
				else
				{
					Object.DestroyImmediate(this._sharedGrabMaterial);
				}
				this._sharedGrabMaterial = null;
			}
		}

		// Token: 0x0600BA9C RID: 47772 RVA: 0x005501E4 File Offset: 0x0054E3E4
		private void LateUpdate()
		{
			bool skipSharedDoProcess = this._skipSharedDoProcess;
			if (skipSharedDoProcess)
			{
				this._skipSharedDoProcess = false;
			}
			else
			{
				bool flag = this._sharedMaskInstance != null && this._sharedMaskInstance.IsGrabActive;
				if (flag)
				{
					this._sharedMaskInstance.DoProcess();
				}
			}
			this.LateUpdateLegacy();
			bool flag2 = this._sharedMaskInstance != null && this._sharedMaskInstance.IsOnePassBlurActive;
			if (flag2)
			{
				Transform currentOwner = this.GetCurrentRealOwner();
				bool flag3 = currentOwner == null || !currentOwner.gameObject.activeInHierarchy;
				if (flag3)
				{
					this._sharedMaskInstance.SetOnePassBlurVisible(false);
				}
			}
		}

		// Token: 0x0600BA9D RID: 47773 RVA: 0x00550294 File Offset: 0x0054E494
		private void LateUpdateLegacy()
		{
			bool flag = this._legacyMasksWithSlot.Count == 0;
			if (!flag)
			{
				int frameSlot = Time.frameCount % 4;
				int index = 0;
				foreach (UIMask mask in this._legacyMasksWithSlot)
				{
					bool flag2 = index == frameSlot;
					if (flag2)
					{
						mask.DoProcess();
						break;
					}
					index++;
				}
			}
		}

		// Token: 0x04009028 RID: 36904
		private static readonly string[] GrabTextureNames = new string[]
		{
			"_UIMaskGrabTexture1",
			"_UIMaskGrabTexture2",
			"_UIMaskGrabTexture3",
			"_UIMaskGrabTexture4"
		};

		// Token: 0x04009029 RID: 36905
		private static readonly string[] ShaderNames = new string[]
		{
			"UI/UIMaskGrab1",
			"UI/UIMaskGrab2",
			"UI/UIMaskGrab3",
			"UI/UIMaskGrab4"
		};

		// Token: 0x0400902A RID: 36906
		private const int SharedSlotIndex = 0;

		// Token: 0x0400902B RID: 36907
		private const int LegacySlotStart = 1;

		// Token: 0x0400902C RID: 36908
		private Material _sharedGrabMaterial;

		// Token: 0x0400902D RID: 36909
		private string _sharedGrabTextureName;

		// Token: 0x0400902E RID: 36910
		private readonly Queue<UIMaskManager.LegacySlotInfo> _legacyAvailableSlots = new Queue<UIMaskManager.LegacySlotInfo>();

		// Token: 0x0400902F RID: 36911
		private readonly List<UIMaskManager.LegacySlotInfo> _legacyAllSlots = new List<UIMaskManager.LegacySlotInfo>();

		// Token: 0x04009030 RID: 36912
		private readonly Dictionary<UIMask, UIMaskManager.LegacySlotInfo> _legacyAssigned = new Dictionary<UIMask, UIMaskManager.LegacySlotInfo>();

		// Token: 0x04009031 RID: 36913
		private readonly LinkedList<UIMask> _legacyMasksWithSlot = new LinkedList<UIMask>();

		// Token: 0x04009032 RID: 36914
		private readonly List<UIMask> _legacyMasksWithoutSlot = new List<UIMask>();

		// Token: 0x04009033 RID: 36915
		private const string UIMaskPrefabPath = "RemakeResources/Prefab/Components/Common/UIMask";

		// Token: 0x04009034 RID: 36916
		private UIMask _sharedMaskInstance;

		// Token: 0x04009035 RID: 36917
		private readonly List<Transform> _maskOwnerList = new List<Transform>();

		// Token: 0x04009036 RID: 36918
		private readonly Dictionary<Transform, UIMaskManager.OwnerBlurCache> _ownerBlurCacheDict = new Dictionary<Transform, UIMaskManager.OwnerBlurCache>();

		// Token: 0x04009037 RID: 36919
		private bool _skipSharedDoProcess;

		// Token: 0x04009038 RID: 36920
		private readonly HashSet<UIElement> _maskedElements = new HashSet<UIElement>();

		// Token: 0x02002637 RID: 9783
		private sealed class OwnerBlurCache
		{
			// Token: 0x0400E9FA RID: 59898
			public RenderTexture Texture;
		}

		// Token: 0x02002638 RID: 9784
		private struct LegacySlotInfo
		{
			// Token: 0x0400E9FB RID: 59899
			public Material Mat;

			// Token: 0x0400E9FC RID: 59900
			public string GrabName;
		}
	}
}
