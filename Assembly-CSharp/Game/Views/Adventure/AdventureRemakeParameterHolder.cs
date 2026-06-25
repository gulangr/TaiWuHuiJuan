using System;
using FrameWork.UISystem.UIElements;
using UnityEngine;

namespace Game.Views.Adventure
{
	// Token: 0x02000C6E RID: 3182
	public class AdventureRemakeParameterHolder : MonoBehaviour
	{
		// Token: 0x0600A1EE RID: 41454 RVA: 0x004BB338 File Offset: 0x004B9538
		private void Awake()
		{
			this.expand.onClick.ResetListener(delegate()
			{
				this.expandContext.SetActive(!this.expandContext.activeSelf);
				this.arrow.localEulerAngles = (this.expandContext.activeSelf ? new Vector3(0f, 0f, 180f) : Vector3.zero);
				this.self.SetHeight(this.expandContext.activeSelf ? 200f : 36f);
			});
		}

		// Token: 0x0600A1EF RID: 41455 RVA: 0x004BB358 File Offset: 0x004B9558
		private void OnEnable()
		{
			this.expandContext.SetActive(true);
			this.self.SetHeight(200f);
			this.arrow.localEulerAngles = (this.expandContext.activeSelf ? new Vector3(0f, 0f, 180f) : Vector3.zero);
		}

		// Token: 0x04007DE1 RID: 32225
		[SerializeField]
		private CButton expand;

		// Token: 0x04007DE2 RID: 32226
		[SerializeField]
		private RectTransform arrow;

		// Token: 0x04007DE3 RID: 32227
		[SerializeField]
		private GameObject expandContext;

		// Token: 0x04007DE4 RID: 32228
		[SerializeField]
		private RectTransform self;
	}
}
