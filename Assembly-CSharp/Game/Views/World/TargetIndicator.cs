using System;
using FrameWork.UISystem.UIElements;
using UnityEngine;

namespace Game.Views.World
{
	// Token: 0x0200072E RID: 1838
	public class TargetIndicator : MonoBehaviour
	{
		// Token: 0x060057E5 RID: 22501 RVA: 0x0028C896 File Offset: 0x0028AA96
		public void ResetListener(Action callback)
		{
			this.button.onClick.ResetListener(callback);
		}

		// Token: 0x060057E6 RID: 22502 RVA: 0x0028C8AC File Offset: 0x0028AAAC
		public void UpdateTaiwuVillage(Transform target)
		{
			Vector3 localPos = this.rootRect.InverseTransformPoint(target.position);
			bool flag = this.rootRect.rect.Contains(localPos);
			if (flag)
			{
				bool activeSelf = this.button.gameObject.activeSelf;
				if (activeSelf)
				{
					this.button.gameObject.SetActive(false);
				}
			}
			else
			{
				bool flag2 = !this.button.gameObject.activeSelf;
				if (flag2)
				{
					this.button.gameObject.SetActive(true);
				}
				Rect rect = this.rootRect.rect;
				Vector2 center = rect.center;
				Vector2 targetLocal = new Vector2(localPos.x, localPos.y);
				Vector2 delta = targetLocal - center;
				float halfW = rect.width * 0.5f;
				float halfH = rect.height * 0.5f;
				float absDx = Mathf.Abs(delta.x);
				float absDy = Mathf.Abs(delta.y);
				bool flag3 = absDx < 1E-06f && absDy < 1E-06f;
				if (flag3)
				{
					this.button.transform.localPosition = center + new Vector2(halfW, 0f);
				}
				else
				{
					bool flag4 = absDx / halfW > absDy / halfH;
					Vector2 intersectPoint;
					if (flag4)
					{
						float x = Mathf.Sign(delta.x) * halfW;
						float y = delta.y / delta.x * x;
						intersectPoint = center + new Vector2(x, y);
					}
					else
					{
						float y2 = Mathf.Sign(delta.y) * halfH;
						float x2 = delta.x / delta.y * y2;
						intersectPoint = center + new Vector2(x2, y2);
					}
					this.button.transform.localPosition = new Vector3(intersectPoint.x, intersectPoint.y, 0f);
				}
				this.direction.rotation = Quaternion.LookRotation(Vector3.forward, this.buttonRectTrans.localPosition - localPos);
			}
		}

		// Token: 0x04003C52 RID: 15442
		[SerializeField]
		private CButton button;

		// Token: 0x04003C53 RID: 15443
		[SerializeField]
		private RectTransform buttonRectTrans;

		// Token: 0x04003C54 RID: 15444
		[SerializeField]
		private RectTransform direction;

		// Token: 0x04003C55 RID: 15445
		[SerializeField]
		private RectTransform rootRect;
	}
}
