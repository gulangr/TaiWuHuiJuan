using System;

namespace CommonSortAndFilterLegacy
{
	// Token: 0x02000431 RID: 1073
	public static class CursorController
	{
		// Token: 0x06003FAB RID: 16299 RVA: 0x001FADBD File Offset: 0x001F8FBD
		public static void SetCursorProgress(float percent)
		{
			ConchShipCursor.Instance.SetWheelProgress(percent);
		}

		// Token: 0x06003FAC RID: 16300 RVA: 0x001FADCC File Offset: 0x001F8FCC
		public static void SwitchToDragCursor()
		{
			ConchShipCursor.Instance.SetCursorImageWithKey("common_detailed_filter_drag", "filter_cursor_move", -1f, -1f);
		}

		// Token: 0x06003FAD RID: 16301 RVA: 0x001FADEE File Offset: 0x001F8FEE
		public static void SwitchToDefaultCursor()
		{
			ConchShipCursor.Instance.SetDefaultCursorAndReleaseKey("common_detailed_filter_drag");
		}
	}
}
