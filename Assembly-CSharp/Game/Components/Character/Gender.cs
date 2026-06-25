using System;
using Game.Components.Common;
using GameData.Domains.Character.Display;
using UnityEngine;

namespace Game.Components.Character
{
	// Token: 0x02000F26 RID: 3878
	public class Gender : MonoBehaviour
	{
		// Token: 0x0600B282 RID: 45698 RVA: 0x00513D70 File Offset: 0x00511F70
		public void Set(CharacterDisplayData data, bool isShowBack = true)
		{
			bool flag = data == null;
			if (flag)
			{
				this.SetEmpty();
			}
			else
			{
				CommonUtils.EDisplayGender displayGender = CommonUtils.GetDisplayGender(data.Gender, data.TemplateId);
				this.Set(displayGender);
				this.propertyItem.SetShowBack(isShowBack);
			}
		}

		// Token: 0x0600B283 RID: 45699 RVA: 0x00513DB8 File Offset: 0x00511FB8
		public void Set(CommonUtils.EDisplayGender displayGender)
		{
			bool flag = this.propertyItem != null;
			if (flag)
			{
				if (!true)
				{
				}
				Sprite sprite;
				switch (displayGender)
				{
				case CommonUtils.EDisplayGender.Male:
					sprite = this.maleIcon;
					break;
				case CommonUtils.EDisplayGender.Female:
					sprite = this.femaleIcon;
					break;
				case CommonUtils.EDisplayGender.Hidden:
					sprite = this.noGenderIcon;
					break;
				default:
					sprite = null;
					break;
				}
				if (!true)
				{
				}
				Sprite icon = sprite;
				this.propertyItem.SetIconEnable(icon != null);
				string text = CommonUtils.GetGenderString(displayGender);
				this.propertyItem.Set(icon, LanguageKey.LK_Main_SummaryInfo_Gender.Tr(), text, null, false);
			}
		}

		// Token: 0x0600B284 RID: 45700 RVA: 0x00513E54 File Offset: 0x00512054
		public void SetEmpty()
		{
			bool flag = this.propertyItem != null;
			if (flag)
			{
				this.propertyItem.Set(string.Empty, string.Empty, string.Empty, null, false);
			}
		}

		// Token: 0x04008A83 RID: 35459
		[Header("性别组件")]
		[SerializeField]
		private PropertyItem propertyItem;

		// Token: 0x04008A84 RID: 35460
		[Header("性别图标")]
		[SerializeField]
		private Sprite femaleIcon;

		// Token: 0x04008A85 RID: 35461
		[SerializeField]
		private Sprite maleIcon;

		// Token: 0x04008A86 RID: 35462
		[SerializeField]
		private Sprite noGenderIcon;
	}
}
