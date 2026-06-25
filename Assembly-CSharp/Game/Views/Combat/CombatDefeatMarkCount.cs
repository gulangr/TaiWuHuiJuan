using System;
using GameData.Domains.Combat;
using TMPro;
using UnityEngine;

namespace Game.Views.Combat
{
	// Token: 0x02000B2A RID: 2858
	public class CombatDefeatMarkCount : MonoBehaviour
	{
		// Token: 0x06008C1A RID: 35866 RVA: 0x0040BD80 File Offset: 0x00409F80
		private void OnValidate()
		{
			this.Reset();
		}

		// Token: 0x06008C1B RID: 35867 RVA: 0x0040BD8C File Offset: 0x00409F8C
		private void Reset()
		{
			foreach (RectTransform child in base.transform.GetComponentsInChildren<RectTransform>())
			{
				bool flag = child.name.Contains("Back");
				if (flag)
				{
					child.localScale = (this.isAlly ? Vector3.one : Vector3.one.SetX(-1f));
				}
			}
		}

		// Token: 0x06008C1C RID: 35868 RVA: 0x0040BDF4 File Offset: 0x00409FF4
		public void Set(CombatType combatType, DefeatMarkCollection markCollection)
		{
			this.markCount.text = markCollection.GetTotalCount().ToString();
			this.requireCount.text = GlobalConfig.NeedDefeatMarkCount[(int)combatType].ToString();
		}

		// Token: 0x04006B3C RID: 27452
		public bool isAlly = true;

		// Token: 0x04006B3D RID: 27453
		[SerializeField]
		private TextMeshProUGUI markCount;

		// Token: 0x04006B3E RID: 27454
		[SerializeField]
		private TextMeshProUGUI requireCount;
	}
}
