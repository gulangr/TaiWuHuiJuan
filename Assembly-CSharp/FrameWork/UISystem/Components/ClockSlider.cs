using System;
using EasyButtons;
using UnityEngine;
using UnityEngine.UI;

namespace FrameWork.UISystem.Components
{
	// Token: 0x02001012 RID: 4114
	public class ClockSlider : MonoBehaviour
	{
		// Token: 0x0600BC0D RID: 48141 RVA: 0x005585B0 File Offset: 0x005567B0
		[Button]
		public void Set(float value)
		{
			value = Mathf.Clamp01(value);
			this.fill.fillAmount = value;
			bool flag = this.fill.fillMethod == Image.FillMethod.Radial180;
			if (flag)
			{
				float rotationZ = -90f + 180f * value;
				bool fillClockwise = this.fill.fillClockwise;
				if (fillClockwise)
				{
					rotationZ *= -1f;
				}
				rotationZ = Mathf.Clamp(rotationZ, -90f, 90f);
				this.handleRoot.rotation = Quaternion.Euler(0f, 0f, rotationZ);
				bool active = !this.autoHideHandleInVertex || (!Mathf.Approximately(value, 0f) && !Mathf.Approximately(value, 1f));
				bool flag2 = active != this.handleRoot.gameObject.activeSelf;
				if (flag2)
				{
					this.handleRoot.gameObject.SetActive(active);
				}
				return;
			}
			throw new NotSupportedException(string.Format("ClockSlider not supported fill method {0}", this.fill.fillMethod));
		}

		// Token: 0x040090E6 RID: 37094
		[SerializeField]
		private Image fill;

		// Token: 0x040090E7 RID: 37095
		[SerializeField]
		private RectTransform handleRoot;

		// Token: 0x040090E8 RID: 37096
		[SerializeField]
		private bool autoHideHandleInVertex;
	}
}
