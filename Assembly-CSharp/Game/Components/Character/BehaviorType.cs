using System;
using Game.Components.Common;
using GameData.Domains.Character.Display;
using UnityEngine;

namespace Game.Components.Character
{
	// Token: 0x02000F15 RID: 3861
	public class BehaviorType : MonoBehaviour
	{
		// Token: 0x0600B1E8 RID: 45544 RVA: 0x00510968 File Offset: 0x0050EB68
		public void Set(CharacterDisplayData data, bool isShowBack = true)
		{
			bool flag = data == null;
			if (flag)
			{
				this.SetEmpty();
			}
			else
			{
				this.Set(data.BehaviorType);
				this.propertyItem.SetShowBack(isShowBack);
			}
		}

		// Token: 0x0600B1E9 RID: 45545 RVA: 0x005109A4 File Offset: 0x0050EBA4
		public void Set(sbyte behaviorType)
		{
			bool flag = this.propertyItem != null;
			if (flag)
			{
				int iconIndex = CommonUtils.GetBehaviorTypeIconIndex(behaviorType);
				string text = CommonUtils.GetBehaviorString(behaviorType);
				this.propertyItem.Set(this.sprites[iconIndex], LanguageKey.LK_Main_SummaryInfo_Behavior.Tr(), text, null, false);
			}
		}

		// Token: 0x0600B1EA RID: 45546 RVA: 0x005109FC File Offset: 0x0050EBFC
		public void SetEmpty()
		{
			bool flag = this.propertyItem != null;
			if (flag)
			{
				this.propertyItem.Set(string.Empty, string.Empty, string.Empty, null, false);
			}
		}

		// Token: 0x040089E8 RID: 35304
		[Header("立场组件")]
		[SerializeField]
		private PropertyItem propertyItem;

		// Token: 0x040089E9 RID: 35305
		[Header("立场图标")]
		[SerializeField]
		private Sprite[] sprites;
	}
}
