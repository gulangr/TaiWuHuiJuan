using System;
using System.Runtime.CompilerServices;
using Game.Components.Common;
using GameData.Domains.Character;
using UnityEngine;

namespace Game.Components.Character
{
	// Token: 0x02000F40 RID: 3904
	public class QiState : MonoBehaviour
	{
		// Token: 0x17001456 RID: 5206
		// (get) Token: 0x0600B355 RID: 45909 RVA: 0x0051A3A7 File Offset: 0x005185A7
		public static string[] DisorderOfQiLevelColors
		{
			get
			{
				return new string[]
				{
					"BehaviorType_Just",
					"BehaviorType_Even",
					"BehaviorType_Even",
					"BehaviorType_Even",
					"brightred"
				};
			}
		}

		// Token: 0x0600B356 RID: 45910 RVA: 0x0051A3D7 File Offset: 0x005185D7
		public Sprite GetDisorderOfQiLevelIcon(sbyte level)
		{
			return this.sprites[(level < 2) ? 0 : 1];
		}

		// Token: 0x0600B357 RID: 45911 RVA: 0x0051A3E8 File Offset: 0x005185E8
		public void Set(short disorderOfQi)
		{
			this.Set(DisorderLevelOfQi.GetDisorderLevelOfQi(disorderOfQi));
		}

		// Token: 0x0600B358 RID: 45912 RVA: 0x0051A3F8 File Offset: 0x005185F8
		public void Set(sbyte level)
		{
			bool flag = this.propertyItem;
			if (flag)
			{
				this.propertyItem.Set(this.GetDisorderOfQiLevelIcon(level), LanguageKey.LK_Combat_MarkType_QiDisorder.Tr(), QiState.DisorderOfQiLevelLangKeys[(int)level].Tr().SetColor(QiState.DisorderOfQiLevelColors[(int)level]), null, false);
			}
		}

		// Token: 0x0600B359 RID: 45913 RVA: 0x0051A454 File Offset: 0x00518654
		public void SetEmpty()
		{
			bool flag = this.propertyItem;
			if (flag)
			{
				this.propertyItem.Set(string.Empty, string.Empty, string.Empty, null, false);
			}
		}

		// Token: 0x0600B35B RID: 45915 RVA: 0x0051A49F File Offset: 0x0051869F
		// Note: this type is marked as 'beforefieldinit'.
		static QiState()
		{
			LanguageKey[] array = new LanguageKey[5];
			RuntimeHelpers.InitializeArray(array, fieldof(<PrivateImplementationDetails>.E23492D894508AE955CA047489440C5B44AB511667C0ACFD8B9553E5949F84DA).FieldHandle);
			QiState.DisorderOfQiLevelLangKeys = array;
		}

		// Token: 0x04008B49 RID: 35657
		public static readonly LanguageKey[] DisorderOfQiLevelLangKeys;

		// Token: 0x04008B4A RID: 35658
		[SerializeField]
		private PropertyItem propertyItem;

		// Token: 0x04008B4B RID: 35659
		[Header("内息图标")]
		[SerializeField]
		private Sprite[] sprites;
	}
}
