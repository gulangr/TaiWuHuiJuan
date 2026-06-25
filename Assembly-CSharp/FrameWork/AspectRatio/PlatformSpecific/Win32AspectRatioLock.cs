using System;
using System.Runtime.InteropServices;
using System.Text;
using UnityEngine;

namespace FrameWork.AspectRatio.PlatformSpecific
{
	// Token: 0x0200107B RID: 4219
	public class Win32AspectRatioLock : AspectRatioHandlerBase
	{
		// Token: 0x0600BF6D RID: 49005 RVA: 0x00568764 File Offset: 0x00566964
		public Win32AspectRatioLock(AspectRatioDefinition definition) : base(definition)
		{
			Win32AspectRatioLock.EnumThreadWindows(Win32AspectRatioLock.GetCurrentThreadId(), delegate(IntPtr hWnd, IntPtr lParam)
			{
				StringBuilder classText = new StringBuilder("UnityWndClass".Length + 1);
				Win32AspectRatioLock.GetClassName(hWnd, classText, classText.Capacity);
				bool flag = classText.ToString() == "UnityWndClass";
				bool result;
				if (flag)
				{
					this._unityHWnd = hWnd;
					result = false;
				}
				else
				{
					result = true;
				}
				return result;
			}, IntPtr.Zero);
			this._wndProcDelegate = new Win32AspectRatioLock.WndProcDelegate(this.WndProc);
			this._newWndProcPtr = Marshal.GetFunctionPointerForDelegate<Win32AspectRatioLock.WndProcDelegate>(this._wndProcDelegate);
			this._oldWndProcPtr = Win32AspectRatioLock.SetWindowLong(this._unityHWnd, -4, this._newWndProcPtr);
			this.SetWindowStyle();
		}

		// Token: 0x0600BF6E RID: 49006 RVA: 0x005687DC File Offset: 0x005669DC
		private IntPtr WndProc(IntPtr hWnd, uint msg, IntPtr wParam, IntPtr lParam)
		{
			this.EnsureMaximizeButtonDisabled();
			bool flag = msg == 532U;
			if (flag)
			{
				Win32AspectRatioLock.RECT windowRect = default(Win32AspectRatioLock.RECT);
				Win32AspectRatioLock.GetWindowRect(this._unityHWnd, ref windowRect);
				Win32AspectRatioLock.RECT clientRect = default(Win32AspectRatioLock.RECT);
				Win32AspectRatioLock.GetClientRect(this._unityHWnd, ref clientRect);
				int borderWidth = windowRect.Right - windowRect.Left - (clientRect.Right - clientRect.Left);
				int borderHeight = windowRect.Bottom - windowRect.Top - (clientRect.Bottom - clientRect.Top);
				Win32AspectRatioLock.RECT rc = (Win32AspectRatioLock.RECT)Marshal.PtrToStructure(lParam, typeof(Win32AspectRatioLock.RECT));
				rc.Right -= borderWidth;
				rc.Bottom -= borderHeight;
				int newWidth = Mathf.Clamp(rc.Right - rc.Left, this.AspectRatioDefinition.MinWidthPixels, this.AspectRatioDefinition.MaxWidthPixels);
				int newHeight = Mathf.Clamp(rc.Bottom - rc.Top, this.AspectRatioDefinition.MinHeightPixels, this.AspectRatioDefinition.MaxHeightPixels);
				switch (wParam.ToInt32())
				{
				case 1:
					newWidth = newWidth / this.AspectRatioDefinition.Width * this.AspectRatioDefinition.Width;
					rc.Left = rc.Right - newWidth;
					rc.Bottom = rc.Top + this.AspectRatioDefinition.Height * newWidth / this.AspectRatioDefinition.Width;
					break;
				case 2:
					newWidth = newWidth / this.AspectRatioDefinition.Width * this.AspectRatioDefinition.Width;
					rc.Right = rc.Left + newWidth;
					rc.Bottom = rc.Top + this.AspectRatioDefinition.Height * newWidth / this.AspectRatioDefinition.Width;
					break;
				case 3:
					newHeight = newHeight / this.AspectRatioDefinition.Height * this.AspectRatioDefinition.Height;
					rc.Top = rc.Bottom - newHeight;
					rc.Right = rc.Left + this.AspectRatioDefinition.Width * newHeight / this.AspectRatioDefinition.Height;
					break;
				case 4:
					newWidth = newWidth / this.AspectRatioDefinition.Width * this.AspectRatioDefinition.Width;
					rc.Left = rc.Right - newWidth;
					rc.Top = rc.Bottom - this.AspectRatioDefinition.Height * newWidth / this.AspectRatioDefinition.Width;
					break;
				case 5:
					newWidth = newWidth / this.AspectRatioDefinition.Width * this.AspectRatioDefinition.Width;
					rc.Right = rc.Left + newWidth;
					rc.Top = rc.Bottom - this.AspectRatioDefinition.Height * newWidth / this.AspectRatioDefinition.Width;
					break;
				case 6:
					newHeight = newHeight / this.AspectRatioDefinition.Height * this.AspectRatioDefinition.Height;
					rc.Bottom = rc.Top + newHeight;
					rc.Right = rc.Left + this.AspectRatioDefinition.Width * newHeight / this.AspectRatioDefinition.Height;
					break;
				case 7:
					newWidth = newWidth / this.AspectRatioDefinition.Width * this.AspectRatioDefinition.Width;
					rc.Left = rc.Right - newWidth;
					rc.Bottom = rc.Top + this.AspectRatioDefinition.Height * newWidth / this.AspectRatioDefinition.Width;
					break;
				case 8:
					newWidth = newWidth / this.AspectRatioDefinition.Width * this.AspectRatioDefinition.Width;
					rc.Right = rc.Left + newWidth;
					rc.Bottom = rc.Top + this.AspectRatioDefinition.Height * newWidth / this.AspectRatioDefinition.Width;
					break;
				}
				int setWidth = rc.Right - rc.Left;
				int setHeight = rc.Bottom - rc.Top;
				rc.Right += borderWidth;
				rc.Bottom += borderHeight;
				Marshal.StructureToPtr<Win32AspectRatioLock.RECT>(rc, lParam, true);
				SingletonObject.getInstance<GlobalSettings>().Resolution = new Vector2Int(setWidth, setHeight);
			}
			else
			{
				bool flag2 = msg == 163U;
				if (flag2)
				{
					return (IntPtr)0;
				}
			}
			return Win32AspectRatioLock.CallWindowProc(this._oldWndProcPtr, hWnd, msg, wParam, lParam);
		}

		// Token: 0x0600BF6F RID: 49007 RVA: 0x00568C84 File Offset: 0x00566E84
		private static IntPtr SetWindowLong(IntPtr hWnd, int nIndex, IntPtr dwNewLong)
		{
			return (IntPtr.Size == 4) ? Win32AspectRatioLock.SetWindowLong32(hWnd, nIndex, dwNewLong) : Win32AspectRatioLock.SetWindowLongPtr64(hWnd, nIndex, dwNewLong);
		}

		// Token: 0x0600BF70 RID: 49008 RVA: 0x00568CB0 File Offset: 0x00566EB0
		private void SetWindowStyle()
		{
			int stylePtr = Win32AspectRatioLock.GetWindowLong(this._unityHWnd, -16);
			Win32AspectRatioLock.SetWindowLong(this._unityHWnd, -16, new IntPtr((long)stylePtr & -65537L));
		}

		// Token: 0x0600BF71 RID: 49009 RVA: 0x00568CE8 File Offset: 0x00566EE8
		private void EnsureMaximizeButtonDisabled()
		{
			int currentStyle = Win32AspectRatioLock.GetWindowLong(this._unityHWnd, -16);
			bool flag = ((long)currentStyle & 65536L) != 0L;
			if (flag)
			{
				Win32AspectRatioLock.SetWindowLong(this._unityHWnd, -16, new IntPtr((long)currentStyle & -65537L));
			}
		}

		// Token: 0x0600BF72 RID: 49010
		[DllImport("kernel32.dll")]
		private static extern uint GetCurrentThreadId();

		// Token: 0x0600BF73 RID: 49011
		[DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
		private static extern int GetClassName(IntPtr hWnd, StringBuilder lpString, int nMaxCount);

		// Token: 0x0600BF74 RID: 49012
		[DllImport("user32.dll")]
		private static extern bool EnumThreadWindows(uint dwThreadId, Win32AspectRatioLock.EnumWindowsProc lpEnumFunc, IntPtr lParam);

		// Token: 0x0600BF75 RID: 49013
		[DllImport("user32.dll")]
		private static extern IntPtr CallWindowProc(IntPtr lpPrevWndFunc, IntPtr hWnd, uint Msg, IntPtr wParam, IntPtr lParam);

		// Token: 0x0600BF76 RID: 49014
		[DllImport("user32.dll", SetLastError = true)]
		internal static extern bool GetWindowRect(IntPtr hwnd, ref Win32AspectRatioLock.RECT lpRect);

		// Token: 0x0600BF77 RID: 49015
		[DllImport("user32.dll")]
		internal static extern bool GetClientRect(IntPtr hWnd, ref Win32AspectRatioLock.RECT lpRect);

		// Token: 0x0600BF78 RID: 49016
		[DllImport("user32.dll", CharSet = CharSet.Auto, EntryPoint = "SetWindowLong")]
		internal static extern IntPtr SetWindowLong32(IntPtr hWnd, int nIndex, IntPtr dwNewLong);

		// Token: 0x0600BF79 RID: 49017
		[DllImport("user32.dll", CharSet = CharSet.Auto, EntryPoint = "SetWindowLongPtr")]
		internal static extern IntPtr SetWindowLongPtr64(IntPtr hWnd, int nIndex, IntPtr dwNewLong);

		// Token: 0x0600BF7A RID: 49018
		[DllImport("user32.dll", SetLastError = true)]
		internal static extern int GetWindowLong(IntPtr hWnd, int nIndex);

		// Token: 0x0600BF7B RID: 49019 RVA: 0x00568D33 File Offset: 0x00566F33
		public override void OnResolutionChanged(int width, int height, bool fullScreen)
		{
			SingletonObject.getInstance<YieldHelper>().DelayFrameDo(2U, new Action(this.SetWindowStyle));
		}

		// Token: 0x0600BF7C RID: 49020 RVA: 0x00568D50 File Offset: 0x00566F50
		public override void OnFullScreenChanged(int width, int height, bool fullScreen)
		{
			bool flag = !fullScreen;
			if (flag)
			{
				SingletonObject.getInstance<YieldHelper>().DelayFrameDo(2U, new Action(this.SetWindowStyle));
			}
		}

		// Token: 0x0600BF7D RID: 49021 RVA: 0x00568D7E File Offset: 0x00566F7E
		public override void OnApplicationQuit()
		{
			Win32AspectRatioLock.SetWindowLong(this._unityHWnd, -4, this._oldWndProcPtr);
		}

		// Token: 0x040092A4 RID: 37540
		private const int WM_SIZING = 532;

		// Token: 0x040092A5 RID: 37541
		private const int WM_NCLBUTTONDBLCLK = 163;

		// Token: 0x040092A6 RID: 37542
		private const long WS_MAXIMIZEBOX = 65536L;

		// Token: 0x040092A7 RID: 37543
		private const int WMSZ_LEFT = 1;

		// Token: 0x040092A8 RID: 37544
		private const int WMSZ_RIGHT = 2;

		// Token: 0x040092A9 RID: 37545
		private const int WMSZ_TOP = 3;

		// Token: 0x040092AA RID: 37546
		private const int WMSZ_BOTTOM = 6;

		// Token: 0x040092AB RID: 37547
		private const int GWLP_WNDPROC = -4;

		// Token: 0x040092AC RID: 37548
		private const int GWL_STYLE = -16;

		// Token: 0x040092AD RID: 37549
		private Win32AspectRatioLock.WndProcDelegate _wndProcDelegate;

		// Token: 0x040092AE RID: 37550
		private const string UNITY_WND_CLASSNAME = "UnityWndClass";

		// Token: 0x040092AF RID: 37551
		private IntPtr _unityHWnd;

		// Token: 0x040092B0 RID: 37552
		private IntPtr _oldWndProcPtr;

		// Token: 0x040092B1 RID: 37553
		private IntPtr _newWndProcPtr;

		// Token: 0x0200268B RID: 9867
		// (Invoke) Token: 0x06011C26 RID: 72742
		private delegate IntPtr WndProcDelegate(IntPtr hWnd, uint msg, IntPtr wParam, IntPtr lParam);

		// Token: 0x0200268C RID: 9868
		// (Invoke) Token: 0x06011C2A RID: 72746
		private delegate bool EnumWindowsProc(IntPtr hWnd, IntPtr lParam);

		// Token: 0x0200268D RID: 9869
		public struct RECT
		{
			// Token: 0x0400EB0C RID: 60172
			public int Left;

			// Token: 0x0400EB0D RID: 60173
			public int Top;

			// Token: 0x0400EB0E RID: 60174
			public int Right;

			// Token: 0x0400EB0F RID: 60175
			public int Bottom;
		}
	}
}
