using System;
using UnityEngine;

namespace Game.Views.Adventure
{
	// Token: 0x02000C79 RID: 3193
	public class RotateHelper : MonoBehaviour
	{
		// Token: 0x0600A224 RID: 41508 RVA: 0x004BC362 File Offset: 0x004BA562
		private void Awake()
		{
			this.rectTransform = base.GetComponent<RectTransform>();
		}

		// Token: 0x0600A225 RID: 41509 RVA: 0x004BC374 File Offset: 0x004BA574
		private void Update()
		{
			float angle = this.rotationSpeed * Time.deltaTime;
			bool flag = this.counterClockwise;
			if (flag)
			{
				angle = -angle;
			}
			this.rectTransform.Rotate(Vector3.forward, angle);
		}

		// Token: 0x04007E10 RID: 32272
		[Tooltip("旋转速度（度/秒）")]
		[SerializeField]
		public float rotationSpeed = 90f;

		// Token: 0x04007E11 RID: 32273
		[Tooltip("是否逆时针旋转（true = 逆时针，false = 顺时针）")]
		[SerializeField]
		public bool counterClockwise = false;

		// Token: 0x04007E12 RID: 32274
		private RectTransform rectTransform;
	}
}
