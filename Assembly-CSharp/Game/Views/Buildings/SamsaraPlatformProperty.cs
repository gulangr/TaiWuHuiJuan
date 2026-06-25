using System;
using System.Runtime.CompilerServices;
using GameData.Domains.Building;
using TMPro;
using UnityEngine;

namespace Game.Views.Buildings
{
	// Token: 0x02000BC2 RID: 3010
	public class SamsaraPlatformProperty : MonoBehaviour
	{
		// Token: 0x060097A0 RID: 38816 RVA: 0x0046A243 File Offset: 0x00468443
		public void Reset()
		{
			this.Set(this._base);
		}

		// Token: 0x060097A1 RID: 38817 RVA: 0x0046A254 File Offset: 0x00468454
		public void Set(SamsaraPlatformBonusAttributes baseValue)
		{
			this._base = baseValue;
			int i = 6;
			while (i-- > 0)
			{
				this.mainTitle[i].text = SamsaraPlatformProperty.MainKey[i].Tr();
				this.mainValue[i].text = baseValue.MainAttributes[i].ToString();
			}
			int j = 14;
			while (j-- > 0)
			{
				this.combatTitle[j].text = SamsaraPlatformProperty.CombatKey[j].Tr();
				this.combatValue[j].text = baseValue.CombatSkillShorts[j].ToString();
			}
			int k = 16;
			while (k-- > 0)
			{
				this.lifeTitle[k].text = SamsaraPlatformProperty.LifeKey[k].Tr();
				this.lifeValue[k].text = baseValue.LifeSkillShorts[k].ToString();
			}
		}

		// Token: 0x060097A2 RID: 38818 RVA: 0x0046A358 File Offset: 0x00468558
		public unsafe void SetDelta(SamsaraPlatformBonusAttributes newValue)
		{
			int i = 6;
			while (i-- > 0)
			{
				int value = (int)(*newValue.MainAttributes[i] - *this._base.MainAttributes[i]);
				this.mainValue[i].text = string.Format("{0}{1}", *this._base.MainAttributes[i], (value > 0) ? string.Format("+{0}", value).SetColor("brightblue") : "");
			}
			int j = 14;
			while (j-- > 0)
			{
				int value2 = (int)(*newValue.CombatSkillShorts[j] - *this._base.CombatSkillShorts[j]);
				this.combatValue[j].text = string.Format("{0}{1}", *this._base.CombatSkillShorts[j], (value2 > 0) ? string.Format("+{0}", value2).SetColor("brightblue") : "");
			}
			int k = 16;
			while (k-- > 0)
			{
				int value3 = (int)(*newValue.LifeSkillShorts[k] - *this._base.LifeSkillShorts[k]);
				this.lifeValue[k].text = string.Format("{0}{1}", *this._base.LifeSkillShorts[k], (value3 > 0) ? string.Format("+{0}", value3).SetColor("brightblue") : "");
			}
		}

		// Token: 0x060097A4 RID: 38820 RVA: 0x0046A524 File Offset: 0x00468724
		// Note: this type is marked as 'beforefieldinit'.
		static SamsaraPlatformProperty()
		{
			LanguageKey[] array = new LanguageKey[6];
			RuntimeHelpers.InitializeArray(array, fieldof(<PrivateImplementationDetails>.F3A77FE461DBEB8456EF47F324CA1604FD30692E344179CB0E1796AD5B28EC5A).FieldHandle);
			SamsaraPlatformProperty.MainKey = array;
			LanguageKey[] array2 = new LanguageKey[14];
			RuntimeHelpers.InitializeArray(array2, fieldof(<PrivateImplementationDetails>.F4B96D9860923775FC6E7AD37A50C783C3D4886DC182C31E8BF90C2FAED9170E).FieldHandle);
			SamsaraPlatformProperty.CombatKey = array2;
			LanguageKey[] array3 = new LanguageKey[16];
			RuntimeHelpers.InitializeArray(array3, fieldof(<PrivateImplementationDetails>.74E04CCA11C6DEA7FC7BE7202E4549514BFB4BAB06B503494DA788C755EFC737).FieldHandle);
			SamsaraPlatformProperty.LifeKey = array3;
		}

		// Token: 0x04007454 RID: 29780
		private static readonly LanguageKey[] MainKey;

		// Token: 0x04007455 RID: 29781
		private static readonly LanguageKey[] CombatKey;

		// Token: 0x04007456 RID: 29782
		private static readonly LanguageKey[] LifeKey;

		// Token: 0x04007457 RID: 29783
		[SerializeField]
		private TMP_Text[] mainTitle;

		// Token: 0x04007458 RID: 29784
		[SerializeField]
		private TMP_Text[] combatTitle;

		// Token: 0x04007459 RID: 29785
		[SerializeField]
		private TMP_Text[] lifeTitle;

		// Token: 0x0400745A RID: 29786
		[SerializeField]
		private TMP_Text[] mainValue;

		// Token: 0x0400745B RID: 29787
		[SerializeField]
		private TMP_Text[] combatValue;

		// Token: 0x0400745C RID: 29788
		[SerializeField]
		private TMP_Text[] lifeValue;

		// Token: 0x0400745D RID: 29789
		[SerializeField]
		private CImage[] mainImg;

		// Token: 0x0400745E RID: 29790
		[SerializeField]
		private CImage[] combatImg;

		// Token: 0x0400745F RID: 29791
		[SerializeField]
		private CImage[] lifeImg;

		// Token: 0x04007460 RID: 29792
		private SamsaraPlatformBonusAttributes _base;
	}
}
