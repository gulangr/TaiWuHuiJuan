using System;
using FrameWork;
using Game.Components.Common;
using GameData.Domains.Character.Alertness;
using GameData.Domains.Character.Display;
using UnityEngine;

namespace Game.Components.Character
{
	// Token: 0x02000F0B RID: 3851
	public class Alertness : MonoBehaviour
	{
		// Token: 0x0600B169 RID: 45417 RVA: 0x0050D21C File Offset: 0x0050B41C
		public void Set(CharacterDisplayData data, bool isShowBack = true)
		{
			bool flag = data == null;
			if (flag)
			{
				this.SetEmpty();
			}
			else
			{
				this.Set(data.Alertness, isShowBack);
				this.tip.Type = TipType.Alertness;
				TooltipInvoker tooltipInvoker = this.tip;
				if (tooltipInvoker.RuntimeParam == null)
				{
					tooltipInvoker.RuntimeParam = EasyPool.Get<ArgumentBox>();
				}
				this.tip.RuntimeParam.Clear();
				this.tip.RuntimeParam.Set("charId", data.CharacterId);
			}
		}

		// Token: 0x0600B16A RID: 45418 RVA: 0x0050D2A4 File Offset: 0x0050B4A4
		public void Set(int alertness, bool isShowBack = true)
		{
			sbyte level = CharacterAlertnessData.GetLevel(alertness);
			string levelName = CommonUtils.GetAlertnessName((int)level);
			string levelIcon = CommonUtils.GetAlertnessIcon((int)level);
			string title = LanguageKey.LK_Alertness.Tr();
			this.propertyItem.Set(levelIcon, title, levelName, null, false);
			this.propertyItem.SetShowBack(isShowBack);
		}

		// Token: 0x0600B16B RID: 45419 RVA: 0x0050D2FC File Offset: 0x0050B4FC
		public void SetEmpty()
		{
			bool flag = this.propertyItem != null;
			if (flag)
			{
				this.propertyItem.Set(string.Empty, string.Empty, "-", null, false);
			}
		}

		// Token: 0x0400897C RID: 35196
		[Header("戒心组件")]
		[SerializeField]
		private PropertyItem propertyItem;

		// Token: 0x0400897D RID: 35197
		[SerializeField]
		private TooltipInvoker tip;
	}
}
