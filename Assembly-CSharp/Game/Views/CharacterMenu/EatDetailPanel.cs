using System;
using System.Linq;
using GameData.Domains.Character;
using GameData.Domains.Character.Display;
using GameData.Domains.Item.Display;
using GameData.Domains.Taiwu;
using GameData.Serializer;
using GameData.Utilities;
using UnityEngine;

namespace Game.Views.CharacterMenu
{
	// Token: 0x02000B6D RID: 2925
	public class EatDetailPanel : MonoBehaviour
	{
		// Token: 0x060090BB RID: 37051 RVA: 0x004376CD File Offset: 0x004358CD
		private void OnDisable()
		{
			this.contentLayout.gameObject.SetActive(false);
		}

		// Token: 0x060090BC RID: 37052 RVA: 0x004376E2 File Offset: 0x004358E2
		public void Setup(int curCharacterId)
		{
			this._curCharaId = curCharacterId;
			CharacterDomainMethod.AsyncCall.GetCharacterInjuryDisplayData(null, curCharacterId, delegate(int offset, RawDataPool pool)
			{
				Serializer.Deserialize(pool, offset, ref this._characterInjuryDisplayData);
				TaiwuDomainMethod.AsyncCall.GetWineTasterBonusPercentage(null, delegate(int offset1, RawDataPool pool1)
				{
					Serializer.Deserialize(pool1, offset1, ref this._addPercent);
					this.RefreshDisplay();
				});
			});
		}

		// Token: 0x060090BD RID: 37053 RVA: 0x00437700 File Offset: 0x00435900
		private void RefreshDisplay()
		{
			int eatAmount = this._characterInjuryDisplayData.EatingItemDisplayDataArray.Count((ItemDisplayData t) => t != null);
			CommonUtils.PrepareEnoughChildren(this.contentLayout, this.eatInfoItemTemplate.gameObject, eatAmount, null);
			this.emptyGo.SetActive(eatAmount == 0);
			this.contentLayout.gameObject.SetActive(true);
			int childIndex = 0;
			for (int i = 0; i < this._characterInjuryDisplayData.EatingItemDisplayDataArray.Length; i++)
			{
				ItemDisplayData itemData = this._characterInjuryDisplayData.EatingItemDisplayDataArray[i];
				bool flag = itemData == null;
				if (!flag)
				{
					short duration = this._characterInjuryDisplayData.EatingItems.GetDuration(i);
					EatInfoItemView comp = this.contentLayout.GetChild(childIndex).GetComponent<EatInfoItemView>();
					comp.Setup(itemData, duration, this._curCharaId, this._addPercent);
					childIndex++;
				}
			}
		}

		// Token: 0x04006F6C RID: 28524
		[SerializeField]
		private RectTransform contentLayout;

		// Token: 0x04006F6D RID: 28525
		[SerializeField]
		private GameObject emptyGo;

		// Token: 0x04006F6E RID: 28526
		[SerializeField]
		private LoadingAnimation loadingAnimation;

		// Token: 0x04006F6F RID: 28527
		[Header("Templates")]
		[SerializeField]
		private EatInfoItemView eatInfoItemTemplate;

		// Token: 0x04006F70 RID: 28528
		private CharacterInjuryDisplayData _characterInjuryDisplayData = null;

		// Token: 0x04006F71 RID: 28529
		private int _addPercent = 0;

		// Token: 0x04006F72 RID: 28530
		private int _curCharaId;
	}
}
