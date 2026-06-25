using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

namespace AiEditor
{
	// Token: 0x02000676 RID: 1654
	public class AiEditorRaycast : MonoBehaviour
	{
		// Token: 0x06004E2E RID: 20014 RVA: 0x0024CA94 File Offset: 0x0024AC94
		public bool IsPointerOver(RectTransform target)
		{
			this.AutoRaycast();
			bool flag = this._cachedRaycastResults.Count == 0;
			bool result;
			if (flag)
			{
				result = false;
			}
			else
			{
				Transform pointerOver = this._cachedRaycastResults[0].gameObject.transform;
				bool flag2 = pointerOver == target;
				if (flag2)
				{
					result = true;
				}
				else
				{
					Transform parent = pointerOver.parent;
					while (parent != null)
					{
						bool flag3 = parent == target;
						if (flag3)
						{
							return true;
						}
						parent = parent.parent;
					}
					result = false;
				}
			}
			return result;
		}

		// Token: 0x06004E2F RID: 20015 RVA: 0x0024CB20 File Offset: 0x0024AD20
		private void AutoRaycast()
		{
			bool flag = this._updated && this.allowCache;
			if (!flag)
			{
				this._updated = true;
				EventSystem eventSystem = EventSystem.current;
				bool flag2 = this._cachedEventData == null || this._cachedEventData.currentInputModule != eventSystem.currentInputModule;
				if (flag2)
				{
					this._cachedEventData = new PointerEventData(eventSystem);
				}
				this._cachedEventData.position = Input.mousePosition;
				RaycastAllManager raycastManager = SingletonObject.getInstance<RaycastAllManager>();
				bool flag3 = raycastManager == null;
				if (!flag3)
				{
					this._cachedRaycastResults.Clear();
					raycastManager.GetCurrentFrameResults(this._cachedRaycastResults);
				}
			}
		}

		// Token: 0x06004E30 RID: 20016 RVA: 0x0024CBCA File Offset: 0x0024ADCA
		private void LateUpdate()
		{
			this._updated = false;
		}

		// Token: 0x04003617 RID: 13847
		[SerializeField]
		private bool allowCache = true;

		// Token: 0x04003618 RID: 13848
		private bool _updated;

		// Token: 0x04003619 RID: 13849
		private PointerEventData _cachedEventData;

		// Token: 0x0400361A RID: 13850
		private readonly List<RaycastResult> _cachedRaycastResults = new List<RaycastResult>();
	}
}
