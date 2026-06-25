using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

// Token: 0x020000B4 RID: 180
public class RaycastAllManager : MonoBehaviour, ISingletonInit, IDisposable
{
	// Token: 0x0600062A RID: 1578 RVA: 0x00029768 File Offset: 0x00027968
	public void Init()
	{
	}

	// Token: 0x0600062B RID: 1579 RVA: 0x0002976B File Offset: 0x0002796B
	public void Dispose()
	{
	}

	// Token: 0x0600062C RID: 1580 RVA: 0x0002976E File Offset: 0x0002796E
	public void GetCurrentFrameResults(List<RaycastResult> results)
	{
		this.EnsureCachedForCurrentFrame();
		results.Clear();
		results.AddRange(this._cachedResults);
	}

	// Token: 0x0600062D RID: 1581 RVA: 0x0002978C File Offset: 0x0002798C
	public List<RaycastResult> GetCurrentFrameResults()
	{
		this.EnsureCachedForCurrentFrame();
		return this._cachedResults;
	}

	// Token: 0x0600062E RID: 1582 RVA: 0x000297AC File Offset: 0x000279AC
	private void EnsureCachedForCurrentFrame()
	{
		int currentFrame = Time.frameCount;
		bool flag = currentFrame == this._lastFrame;
		if (!flag)
		{
			this._lastFrame = currentFrame;
			this._cachedResults.Clear();
			EventSystem eventSystem = EventSystem.current;
			bool flag2 = eventSystem == null;
			if (!flag2)
			{
				bool flag3 = this._pointerEventData == null || this._cachedEventSystem != eventSystem;
				if (flag3)
				{
					this._cachedEventSystem = eventSystem;
					this._pointerEventData = new PointerEventData(eventSystem);
				}
				this._pointerEventData.Reset();
				this._pointerEventData.position = Input.mousePosition;
				eventSystem.RaycastAll(this._pointerEventData, this._cachedResults);
			}
		}
	}

	// Token: 0x0600062F RID: 1583 RVA: 0x0002985F File Offset: 0x00027A5F
	private void LateUpdate()
	{
		this._lastFrame = -1;
		this._cachedResults.Clear();
	}

	// Token: 0x04000510 RID: 1296
	private int _lastFrame = -1;

	// Token: 0x04000511 RID: 1297
	private readonly List<RaycastResult> _cachedResults = new List<RaycastResult>();

	// Token: 0x04000512 RID: 1298
	private PointerEventData _pointerEventData;

	// Token: 0x04000513 RID: 1299
	private EventSystem _cachedEventSystem;
}
