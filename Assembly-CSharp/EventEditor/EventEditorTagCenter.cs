using System;
using TMPro;
using UnityEngine;

namespace EventEditor
{
	// Token: 0x0200064F RID: 1615
	public class EventEditorTagCenter : EventEditorSubPageBase
	{
		// Token: 0x06004CE4 RID: 19684 RVA: 0x00244D8C File Offset: 0x00242F8C
		public static void Init(EventEditorTagCenter instance)
		{
			EventEditorTagCenter.Instance = instance;
		}

		// Token: 0x06004CE5 RID: 19685 RVA: 0x00244D95 File Offset: 0x00242F95
		public void Copy(TextMeshProUGUI valueLabel)
		{
			GUIUtility.systemCopyBuffer = valueLabel.text.Replace("<mark>", string.Empty).Replace("</mark>", string.Empty);
		}

		// Token: 0x06004CE6 RID: 19686 RVA: 0x00244DC2 File Offset: 0x00242FC2
		protected override void InternalInit()
		{
		}

		// Token: 0x06004CE7 RID: 19687 RVA: 0x00244DC5 File Offset: 0x00242FC5
		public override void Show()
		{
		}

		// Token: 0x06004CE8 RID: 19688 RVA: 0x00244DC8 File Offset: 0x00242FC8
		public override void Hide()
		{
		}

		// Token: 0x0400354B RID: 13643
		public static EventEditorTagCenter Instance;
	}
}
