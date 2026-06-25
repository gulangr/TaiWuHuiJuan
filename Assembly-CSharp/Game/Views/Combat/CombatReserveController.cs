using System;
using EasyButtons;
using GameData.Domains.Combat;
using UnityEngine;

namespace Game.Views.Combat
{
	// Token: 0x02000B16 RID: 2838
	public class CombatReserveController : MonoBehaviour
	{
		// Token: 0x17000F64 RID: 3940
		// (get) Token: 0x06008B5B RID: 35675 RVA: 0x00406E7E File Offset: 0x0040507E
		private int ReserveStyleCount
		{
			get
			{
				return 7;
			}
		}

		// Token: 0x06008B5C RID: 35676 RVA: 0x00406E81 File Offset: 0x00405081
		public void OnClickClearReserve()
		{
			CombatDomainMethod.Call.ClearAllReserveAction();
		}

		// Token: 0x06008B5D RID: 35677 RVA: 0x00406E8C File Offset: 0x0040508C
		[Button]
		public void Refresh(CombatReserveController.EReserveType type, RectTransform target)
		{
			if (!true)
			{
			}
			int num;
			switch (type)
			{
			case CombatReserveController.EReserveType.UseSkill:
				num = 2;
				break;
			case CombatReserveController.EReserveType.ShowChangeTrick:
				num = 3;
				break;
			case CombatReserveController.EReserveType.ChangeWeapon:
				num = 1;
				break;
			case CombatReserveController.EReserveType.ChangeWeaponDefault:
				num = 1;
				break;
			case CombatReserveController.EReserveType.UnlockWeapon:
				num = 4;
				break;
			case CombatReserveController.EReserveType.TeammateCommand:
				num = 5;
				break;
			case CombatReserveController.EReserveType.OtherAction:
				num = 0;
				break;
			default:
				throw new ArgumentOutOfRangeException("type", type, null);
			}
			if (!true)
			{
			}
			int index = num;
			RectTransform rt = base.GetComponent<RectTransform>();
			bool flag = type == CombatReserveController.EReserveType.UseSkill || type == CombatReserveController.EReserveType.ChangeWeapon;
			if (flag)
			{
				rt.SetParent(target);
				rt.pivot = this.defaultPivot;
				rt.anchoredPosition = Vector2.zero;
			}
			else
			{
				rt.SetParent(this.defaultParent);
				rt.pivot = target.pivot;
				rt.position = target.position;
			}
			base.GetComponent<Canvas>().overrideSorting = (type > CombatReserveController.EReserveType.UseSkill);
			this.text.anchoredPosition = this.textOffsets[index];
			this.text.gameObject.SetActive(type > CombatReserveController.EReserveType.UseSkill);
			int activateEffectIndex = index;
			bool flag2 = type == CombatReserveController.EReserveType.TeammateCommand || type == CombatReserveController.EReserveType.UnlockWeapon || type == CombatReserveController.EReserveType.ChangeWeaponDefault;
			if (flag2)
			{
				activateEffectIndex = (int)(type + 1);
			}
			for (int i = 0; i < this.ReserveStyleCount; i++)
			{
				Transform effect = base.transform.GetChild(i);
				effect.gameObject.SetActive(i == activateEffectIndex);
			}
		}

		// Token: 0x04006AD7 RID: 27351
		[SerializeField]
		private Vector2[] textOffsets = new Vector2[6];

		// Token: 0x04006AD8 RID: 27352
		[SerializeField]
		private RectTransform text;

		// Token: 0x04006AD9 RID: 27353
		[SerializeField]
		private Transform defaultParent;

		// Token: 0x04006ADA RID: 27354
		[SerializeField]
		private Vector2 defaultPivot;

		// Token: 0x020020D7 RID: 8407
		public enum EReserveType
		{
			// Token: 0x0400D269 RID: 53865
			UseSkill,
			// Token: 0x0400D26A RID: 53866
			ShowChangeTrick,
			// Token: 0x0400D26B RID: 53867
			ChangeWeapon,
			// Token: 0x0400D26C RID: 53868
			ChangeWeaponDefault,
			// Token: 0x0400D26D RID: 53869
			UnlockWeapon,
			// Token: 0x0400D26E RID: 53870
			TeammateCommand,
			// Token: 0x0400D26F RID: 53871
			OtherAction,
			// Token: 0x0400D270 RID: 53872
			Count
		}
	}
}
