using System;
using TMPro;
using UnityEngine;

namespace UICommon.Character.Elements
{
	// Token: 0x020005F3 RID: 1523
	public class CharacterHealthBar : MonoBehaviour
	{
		// Token: 0x060047D1 RID: 18385 RVA: 0x0021A8C0 File Offset: 0x00218AC0
		public void Refresh(short health, short leftHealth, short healthRecovery = 0, int characterId = -1)
		{
			ValueTuple<string, float, int> info = CommonUtils.GetCharacterHealthInfo(health, leftHealth, characterId);
			bool flag = null != this.Progress;
			if (flag)
			{
				this.Progress.fillAmount = info.Item2;
			}
			bool flag2 = null != this.StateLabel;
			if (flag2)
			{
				this.StateLabel.text = ((this.GetHealthString != null) ? this.GetHealthString(new short[]
				{
					health,
					leftHealth,
					healthRecovery
				}, characterId) : info.Item1);
			}
		}

		// Token: 0x060047D2 RID: 18386 RVA: 0x0021A948 File Offset: 0x00218B48
		public void RefreshProgress(short health, short leftHealth)
		{
			ValueTuple<string, float, int> info = CommonUtils.GetCharacterHealthInfo(health, leftHealth, -1);
			bool flag = null != this.Progress;
			if (flag)
			{
				this.Progress.fillAmount = info.Item2;
			}
		}

		// Token: 0x04003198 RID: 12696
		public CImage Progress;

		// Token: 0x04003199 RID: 12697
		public TextMeshProUGUI StateLabel;

		// Token: 0x0400319A RID: 12698
		public Func<short[], int, string> GetHealthString;
	}
}
