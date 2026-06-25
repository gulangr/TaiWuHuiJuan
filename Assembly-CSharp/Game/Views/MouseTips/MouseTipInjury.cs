using System;
using FrameWork;
using Game.Components.Character;
using GameData.Domains.Character;
using GameData.Domains.Character.Display;
using GameData.Domains.Item.Display;
using GameData.Serializer;
using GameData.Utilities;
using UnityEngine;

namespace Game.Views.MouseTips
{
	// Token: 0x02000866 RID: 2150
	public class MouseTipInjury : MouseTipBase
	{
		// Token: 0x060067C5 RID: 26565 RVA: 0x002F6454 File Offset: 0x002F4654
		protected override void Init(ArgumentBox argsBox)
		{
			bool flag = !argsBox.Get("characterId", out this._characterId);
			if (flag)
			{
				this._characterId = -1;
			}
			else
			{
				ItemDisplayData selectedMedicine;
				argsBox.Get<ItemDisplayData>("selectedMedicine", out selectedMedicine);
				CharacterDomainMethod.AsyncCall.GetCharacterInjuryDisplayData(null, this._characterId, delegate(int offset, RawDataPool pool)
				{
					CharacterInjuryDisplayData displayData = null;
					Serializer.Deserialize(pool, offset, ref displayData);
					this.injury.Injury.Set(displayData, true);
					bool flag2 = selectedMedicine == null;
					if (flag2)
					{
						this.injury.Injury.HideNotice(true, true);
					}
					else
					{
						int doctorId = SingletonObject.getInstance<BasicGameData>().TaiwuCharId;
						this.injury.Injury.ShowInfectNoticeWithDoctor(selectedMedicine, 1, doctorId);
					}
				});
			}
		}

		// Token: 0x060067C6 RID: 26566 RVA: 0x002F64BC File Offset: 0x002F46BC
		public override bool CanShowWithArgumentBox(ArgumentBox argumentBox)
		{
			return argumentBox.Get("characterId", out this._characterId);
		}

		// Token: 0x0400494D RID: 18765
		[SerializeField]
		private InjuryDynamic injury;

		// Token: 0x0400494E RID: 18766
		private int _characterId;
	}
}
