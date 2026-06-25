using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace FrameWork.UISystem.UIElements
{
	// Token: 0x02000FFE RID: 4094
	[ExecuteAlways]
	public class ButtonLabelStyle : MonoBehaviour
	{
		// Token: 0x0600BAC9 RID: 47817 RVA: 0x00551142 File Offset: 0x0054F342
		private void Awake()
		{
			this.CacheTargetButton();
		}

		// Token: 0x0600BACA RID: 47818 RVA: 0x0055114C File Offset: 0x0054F34C
		private void OnEnable()
		{
			this.RefreshLabelColors(true);
		}

		// Token: 0x0600BACB RID: 47819 RVA: 0x00551157 File Offset: 0x0054F357
		private void Update()
		{
			this.RefreshLabelColors(false);
		}

		// Token: 0x0600BACC RID: 47820 RVA: 0x00551164 File Offset: 0x0054F364
		private void RefreshLabelColors(bool forceRefresh)
		{
			bool flag = this.targetButton == null;
			if (!flag)
			{
				bool currentInteractable = this.targetButton.IsInteractable();
				bool flag2 = this._initialized && !forceRefresh && currentInteractable == this._lastInteractable;
				if (!flag2)
				{
					this._lastInteractable = currentInteractable;
					this._initialized = true;
					this.ApplyColors(currentInteractable);
				}
			}
		}

		// Token: 0x0600BACD RID: 47821 RVA: 0x005511C4 File Offset: 0x0054F3C4
		private void ApplyColors(bool isInteractable)
		{
			bool flag = this.labelSettings == null;
			if (!flag)
			{
				for (int i = 0; i < this.labelSettings.Length; i++)
				{
					ButtonLabelStyle.LabelColorSetting setting = this.labelSettings[i];
					bool flag2 = setting.label == null;
					if (!flag2)
					{
						setting.label.color = (isInteractable ? setting.normalColor : setting.disabledColor);
					}
				}
			}
		}

		// Token: 0x0600BACE RID: 47822 RVA: 0x00551238 File Offset: 0x0054F438
		private void CacheTargetButton()
		{
			bool flag = this.targetButton == null;
			if (flag)
			{
				this.targetButton = base.GetComponent<Selectable>();
			}
		}

		// Token: 0x0400904B RID: 36939
		[SerializeField]
		private Selectable targetButton;

		// Token: 0x0400904C RID: 36940
		[SerializeField]
		private ButtonLabelStyle.LabelColorSetting[] labelSettings;

		// Token: 0x0400904D RID: 36941
		private bool _lastInteractable;

		// Token: 0x0400904E RID: 36942
		private bool _initialized;

		// Token: 0x0200263B RID: 9787
		[Serializable]
		public struct LabelColorSetting
		{
			// Token: 0x0400EA02 RID: 59906
			public TextMeshProUGUI label;

			// Token: 0x0400EA03 RID: 59907
			public Color normalColor;

			// Token: 0x0400EA04 RID: 59908
			public Color disabledColor;
		}
	}
}
