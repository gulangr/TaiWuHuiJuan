using System;
using GameData.Domains.Combat;
using TMPro;
using UnityEngine;

namespace Game.Components.Combat
{
	// Token: 0x02000F08 RID: 3848
	public class CombatDamageTotalItem : MonoBehaviour
	{
		// Token: 0x0600B15E RID: 45406 RVA: 0x0050CF48 File Offset: 0x0050B148
		public void Set(DefeatMarkKey markKey, int value)
		{
			EMarkType type = markKey.Type;
			if (!true)
			{
			}
			string text;
			switch (type)
			{
			case EMarkType.Outer:
				text = "ui9_back_damage_detail_body_" + markKey.BodyPart.ToString() + "_0";
				goto IL_9B;
			case EMarkType.Inner:
				text = "ui9_back_damage_detail_body_" + markKey.BodyPart.ToString() + "_1";
				goto IL_9B;
			case EMarkType.Poison:
				text = "ui9_back_damage_detail_poison_" + markKey.PoisonType.ToString();
				goto IL_9B;
			case EMarkType.Mind:
				text = "ui9_back_damage_detail_mind";
				goto IL_9B;
			}
			text = "ui9_back_damage_detail_fatal";
			IL_9B:
			if (!true)
			{
			}
			string icon = text;
			this.bgImage.SetSprite(icon, false, null);
			this.valueText.text = value.ToString();
		}

		// Token: 0x04008975 RID: 35189
		[SerializeField]
		private CImage bgImage;

		// Token: 0x04008976 RID: 35190
		[SerializeField]
		private TextMeshProUGUI valueText;
	}
}
