using System;
using FrameWork;
using Game.Components.Character;
using GameData.Domains.Character;
using GameData.Domains.Character.Display;
using GameData.Domains.Taiwu.Profession;
using GameData.Serializer;
using GameData.Utilities;
using UnityEngine;

namespace Game.Views.CharacterMenu
{
	// Token: 0x02000B65 RID: 2917
	public class CharacterMenuInfoDetailInfo : MonoBehaviour
	{
		// Token: 0x0600907A RID: 36986 RVA: 0x00435770 File Offset: 0x00433970
		public void Set(CharacterMenuInfoDisplayData menuData)
		{
			bool flag = ((menuData != null) ? menuData.CharacterDisplayData : null) == null;
			if (flag)
			{
				this.SetEmpty();
			}
			else
			{
				CharacterDisplayData data = menuData.CharacterDisplayData;
				bool flag2 = this.genderDisplay != null;
				if (flag2)
				{
					this.genderDisplay.Set(data, true);
				}
				bool flag3 = this.charmDisplay != null;
				if (flag3)
				{
					this.charmDisplay.Set(data, true);
				}
				bool flag4 = this.behaviorDisplay != null;
				if (flag4)
				{
					this.behaviorDisplay.Set(data, true);
				}
				bool flag5 = this.organizationDisplay != null;
				if (flag5)
				{
					this.organizationDisplay.Set(data, true);
				}
				bool flag6 = this.identityDisplay != null;
				if (flag6)
				{
					this.identityDisplay.Set(data, menuData.IsReclusiveChar, true, false);
				}
				bool flag7 = this.fameDisplay != null;
				if (flag7)
				{
					this.fameDisplay.Set(menuData, true);
				}
				bool flag8 = this.happinessDisplay != null;
				if (flag8)
				{
					this.happinessDisplay.Set(data, true);
				}
				bool flag9 = this.favorabilityDisplay != null;
				if (flag9)
				{
					this.favorabilityDisplay.Set(menuData, true);
				}
				bool needShowProfession = data.CreatingType == 1 && SingletonObject.getInstance<BasicGameData>().TaiwuCharId != data.CharacterId;
				bool flag10 = this.professionLineObj != null;
				if (flag10)
				{
					this.professionLineObj.SetActive(this.currentProfession != null && needShowProfession);
				}
				bool flag11 = this.currentProfession != null;
				if (flag11)
				{
					this.currentProfession.gameObject.SetActive(needShowProfession);
					bool flag12 = needShowProfession;
					if (flag12)
					{
						this.currentProfession.Set(data.CurrentProfession, data.CreatingType == 1);
						this.toolTip.Type = TipType.CharacterCurrentProfession;
						TooltipInvoker tooltipInvoker = this.toolTip;
						if (tooltipInvoker.RuntimeParam == null)
						{
							tooltipInvoker.RuntimeParam = EasyPool.Get<ArgumentBox>();
						}
						int tipCharId = data.CharacterId;
						CharacterDomainMethod.AsyncCall.GetCharacterProfessionList(null, tipCharId, delegate(int offset, RawDataPool pool)
						{
							ProfessionAllDisplayData data2 = null;
							Serializer.Deserialize(pool, offset, ref data2);
							this.toolTip.RuntimeParam.SetObject("ProfessionData", data2);
						});
					}
				}
				bool flag13 = this.alertnessDisplay != null;
				if (flag13)
				{
					bool showAlertness = data.CreatingType == 1 && data.CharacterId != SingletonObject.getInstance<BasicGameData>().TaiwuCharId;
					this.alertnessDisplay.gameObject.SetActive(showAlertness);
					bool flag14 = showAlertness;
					if (flag14)
					{
						this.alertnessDisplay.Set(data, true);
					}
				}
			}
		}

		// Token: 0x0600907B RID: 36987 RVA: 0x004359F8 File Offset: 0x00433BF8
		public void SetEmpty()
		{
			bool flag = this.genderDisplay != null;
			if (flag)
			{
				this.genderDisplay.SetEmpty();
			}
			bool flag2 = this.charmDisplay != null;
			if (flag2)
			{
				this.charmDisplay.SetEmpty();
			}
			bool flag3 = this.behaviorDisplay != null;
			if (flag3)
			{
				this.behaviorDisplay.SetEmpty();
			}
			bool flag4 = this.organizationDisplay != null;
			if (flag4)
			{
				this.organizationDisplay.SetEmpty();
			}
			bool flag5 = this.identityDisplay != null;
			if (flag5)
			{
				this.identityDisplay.SetEmpty();
			}
			bool flag6 = this.fameDisplay != null;
			if (flag6)
			{
				this.fameDisplay.SetEmpty();
			}
			bool flag7 = this.happinessDisplay != null;
			if (flag7)
			{
				this.happinessDisplay.SetEmpty();
			}
			bool flag8 = this.favorabilityDisplay != null;
			if (flag8)
			{
				this.favorabilityDisplay.SetEmpty();
			}
			bool flag9 = this.alertnessDisplay != null;
			if (flag9)
			{
				this.alertnessDisplay.SetEmpty();
			}
			bool flag10 = this.currentProfession != null;
			if (flag10)
			{
				this.currentProfession.SetEmpty();
			}
		}

		// Token: 0x04006F24 RID: 28452
		[Header("性别")]
		[SerializeField]
		private Game.Components.Character.Gender genderDisplay;

		// Token: 0x04006F25 RID: 28453
		[Header("魅力")]
		[SerializeField]
		private Charm charmDisplay;

		// Token: 0x04006F26 RID: 28454
		[Header("立场")]
		[SerializeField]
		private Game.Components.Character.BehaviorType behaviorDisplay;

		// Token: 0x04006F27 RID: 28455
		[Header("从属")]
		[SerializeField]
		private Organization organizationDisplay;

		// Token: 0x04006F28 RID: 28456
		[Header("身份")]
		[SerializeField]
		private Identity identityDisplay;

		// Token: 0x04006F29 RID: 28457
		[Header("名誉")]
		[SerializeField]
		private Fame fameDisplay;

		// Token: 0x04006F2A RID: 28458
		[Header("心情")]
		[SerializeField]
		private Happiness happinessDisplay;

		// Token: 0x04006F2B RID: 28459
		[Header("好感")]
		[SerializeField]
		private Favorability favorabilityDisplay;

		// Token: 0x04006F2C RID: 28460
		[Header("戒心")]
		[SerializeField]
		private Alertness alertnessDisplay;

		// Token: 0x04006F2D RID: 28461
		[Header("志向")]
		[SerializeField]
		private CurrentProfession currentProfession;

		// Token: 0x04006F2E RID: 28462
		[SerializeField]
		private GameObject professionLineObj;

		// Token: 0x04006F2F RID: 28463
		[SerializeField]
		private TooltipInvoker toolTip;
	}
}
