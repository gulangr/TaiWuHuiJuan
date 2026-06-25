using System;
using System.Collections;
using FrameWork;
using FrameWork.AspectRatio;
using FrameWork.AspectRatio.PlatformSpecific;
using UnityEngine;
using UnityEngine.Events;

// Token: 0x020000DA RID: 218
public class AspectRatioController : MonoBehaviour
{
	// Token: 0x170000BD RID: 189
	// (get) Token: 0x060007C6 RID: 1990 RVA: 0x000365E4 File Offset: 0x000347E4
	public int AspectRatioWidth
	{
		get
		{
			return this.aspectRatioWidth;
		}
	}

	// Token: 0x170000BE RID: 190
	// (get) Token: 0x060007C7 RID: 1991 RVA: 0x000365EC File Offset: 0x000347EC
	public int AspectRatioHeight
	{
		get
		{
			return this.aspectRatioHeight;
		}
	}

	// Token: 0x060007C8 RID: 1992 RVA: 0x000365F4 File Offset: 0x000347F4
	private void Start()
	{
		AspectRatioController.Instance = this;
		GEvent.Add(EEvents.OnScreenResolutionChange, new GEvent.Callback(this.OnScreenResolutionChange));
		GEvent.Add(EEvents.OnFullScreenChange, new GEvent.Callback(this.OnFullscreenChange));
		AspectRatioDefinition definition = new AspectRatioDefinition
		{
			Width = this.aspectRatioWidth,
			Height = this.aspectRatioHeight,
			MinWidthPixels = this.minWidthPixel,
			MinHeightPixels = this.minHeightPixel,
			MaxWidthPixels = this.maxWidthPixel,
			MaxHeightPixels = this.maxHeightPixel
		};
		this._aspectRatioLock = new Win32AspectRatioLock(definition);
		Application.wantsToQuit += this.ApplicationWantsToQuit;
		this.SetAspectRatio(this.aspectRatioWidth, this.aspectRatioHeight, true);
		this._started = true;
	}

	// Token: 0x060007C9 RID: 1993 RVA: 0x000366CC File Offset: 0x000348CC
	public void SetAspectRatio(int newAspectWidth, int newAspectHeight, bool apply)
	{
		this.aspectRatioWidth = newAspectWidth;
		this.aspectRatioHeight = newAspectHeight;
		this._aspect = (float)this.aspectRatioWidth / (float)this.aspectRatioHeight;
		this._aspectRatioLock.SetAspectRatio(this.aspectRatioWidth, this.aspectRatioHeight);
		if (apply)
		{
			Screen.SetResolution(Screen.width, Mathf.RoundToInt((float)Screen.width / this._aspect), Screen.fullScreen);
		}
	}

	// Token: 0x060007CA RID: 1994 RVA: 0x00036740 File Offset: 0x00034940
	public Vector2Int FormatResolution(int width, int height)
	{
		width = Mathf.Min(width, Screen.width);
		width = width / this.aspectRatioWidth * this.aspectRatioWidth;
		height = this.aspectRatioHeight * width / this.aspectRatioWidth;
		bool flag = height > Screen.height;
		if (flag)
		{
			height = Screen.height / this.aspectRatioHeight * this.aspectRatioHeight;
			width = this.aspectRatioWidth * height / this.aspectRatioHeight;
		}
		return new Vector2Int(width, height);
	}

	// Token: 0x060007CB RID: 1995 RVA: 0x000367BC File Offset: 0x000349BC
	public Vector2Int FormatResolutionWitoutScreen(int width, int height)
	{
		width = width / this.aspectRatioWidth * this.aspectRatioWidth;
		height = this.aspectRatioHeight * width / this.aspectRatioWidth;
		return new Vector2Int(width, height);
	}

	// Token: 0x060007CC RID: 1996 RVA: 0x000367F7 File Offset: 0x000349F7
	private void OnScreenResolutionChange(ArgumentBox argBox)
	{
		this._aspectRatioLock.OnResolutionChanged(Screen.width, Screen.height, Screen.fullScreen);
	}

	// Token: 0x060007CD RID: 1997 RVA: 0x00036818 File Offset: 0x00034A18
	private void OnFullscreenChange(ArgumentBox argBox)
	{
		bool fullScreen;
		bool flag = argBox.Get("FullScreen", out fullScreen);
		if (flag)
		{
			this._aspectRatioLock.OnFullScreenChanged(Screen.width, Screen.height, fullScreen);
		}
	}

	// Token: 0x060007CE RID: 1998 RVA: 0x00036850 File Offset: 0x00034A50
	private bool ApplicationWantsToQuit()
	{
		bool flag = !this._started;
		bool result;
		if (flag)
		{
			result = false;
		}
		else
		{
			bool flag2 = !this._quitStarted;
			if (flag2)
			{
				base.StartCoroutine(this.DelayedQuit());
				result = false;
			}
			else
			{
				result = true;
			}
		}
		return result;
	}

	// Token: 0x060007CF RID: 1999 RVA: 0x00036893 File Offset: 0x00034A93
	private IEnumerator DelayedQuit()
	{
		this._aspectRatioLock.OnApplicationQuit();
		yield return new WaitForEndOfFrame();
		this._quitStarted = true;
		Application.Quit();
		yield break;
	}

	// Token: 0x040007EA RID: 2026
	public AspectRatioController.ResolutionChangedEvent resolutionChangedEvent;

	// Token: 0x040007EB RID: 2027
	public static AspectRatioController Instance;

	// Token: 0x040007EC RID: 2028
	public static readonly Vector2Int ViewSize = new Vector2Int(2560, 1440);

	// Token: 0x040007ED RID: 2029
	[SerializeField]
	private bool allowFullscreen = true;

	// Token: 0x040007EE RID: 2030
	[SerializeField]
	private int aspectRatioWidth = 16;

	// Token: 0x040007EF RID: 2031
	[SerializeField]
	private int aspectRatioHeight = 9;

	// Token: 0x040007F0 RID: 2032
	[SerializeField]
	private int minWidthPixel = 512;

	// Token: 0x040007F1 RID: 2033
	[SerializeField]
	private int minHeightPixel = 512;

	// Token: 0x040007F2 RID: 2034
	[SerializeField]
	private int maxWidthPixel = 2048;

	// Token: 0x040007F3 RID: 2035
	[SerializeField]
	private int maxHeightPixel = 2048;

	// Token: 0x040007F4 RID: 2036
	private float _aspect;

	// Token: 0x040007F5 RID: 2037
	private bool _started;

	// Token: 0x040007F6 RID: 2038
	private bool _quitStarted;

	// Token: 0x040007F7 RID: 2039
	private AspectRatioHandlerBase _aspectRatioLock;

	// Token: 0x02001136 RID: 4406
	[Serializable]
	public class ResolutionChangedEvent : UnityEvent<int, int, bool>
	{
	}
}
