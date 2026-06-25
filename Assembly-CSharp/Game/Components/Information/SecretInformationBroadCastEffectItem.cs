using System;
using System.Text;
using Config;
using Game.Components.Avatar;
using GameData.Domains.Character;
using GameData.Domains.Character.Display;
using GameData.Domains.Information;
using GameData.Utilities;
using TMPro;
using UnityEngine;

namespace Game.Components.Information
{
	// Token: 0x02000EF8 RID: 3832
	public class SecretInformationBroadCastEffectItem : MonoBehaviour
	{
		// Token: 0x0600B07F RID: 45183 RVA: 0x00506E98 File Offset: 0x00505098
		public void Set(SecretInformationEffectData effectData, CharacterDisplayData data)
		{
			this.avatar.Refresh(data, true);
			this.nameLabel.text = NameCenter.GetMonasticTitleOrDisplayName(data, data.CharacterId == SingletonObject.getInstance<BasicGameData>().TaiwuCharId);
			this.typeLabel.text = ((effectData.Type < 0) ? LanguageKey.LK_SecretInformation_Detail_Resource.Tr() : ((effectData.Type == 1) ? LanguageKey.LK_SecretInformation_Detail_Target_Main.Tr() : LanguageKey.LK_SecretInformation_Detail_Target_Second_Main.Tr()));
			StringBuilder sb = new StringBuilder();
			bool isFirst = true;
			bool flag = effectData.HappinessDelta > 0;
			if (flag)
			{
				sb.Append(LanguageKey.LK_SecretInformation_Detail_Effect_HappinessUp.Tr().SetColor("brightblue"));
				isFirst = false;
			}
			else
			{
				bool flag2 = effectData.HappinessDelta < 0;
				if (flag2)
				{
					sb.Append(LanguageKey.LK_SecretInformation_Detail_Effect_HappinessDown.Tr().SetColor("brightred"));
					isFirst = false;
				}
			}
			bool flag3 = effectData.FavorCount > 0;
			if (flag3)
			{
				bool flag4 = effectData.FavorDelta > 0;
				if (flag4)
				{
					bool flag5 = !isFirst;
					if (flag5)
					{
						sb.Append(LanguageKey.LK_Separator.Tr());
					}
					sb.Append(LanguageKey.LK_SecretInformation_Detail_Effect_FavorUp.Tr().SetColor("brightblue"));
					isFirst = false;
				}
				else
				{
					bool flag6 = effectData.FavorDelta < 0;
					if (flag6)
					{
						bool flag7 = !isFirst;
						if (flag7)
						{
							sb.Append(LanguageKey.LK_Separator.Tr());
						}
						sb.Append(LanguageKey.LK_SecretInformation_Detail_Effect_FavorDown.Tr().SetColor("brightred"));
						isFirst = false;
					}
					else
					{
						bool flag8 = !isFirst;
						if (flag8)
						{
							sb.Append(LanguageKey.LK_Separator.Tr());
						}
						sb.Append(LanguageKey.LK_SecretInformation_Detail_Effect_FavorUpAndDown.Tr());
						isFirst = false;
					}
				}
			}
			bool hasEnemy = effectData.HasEnemy;
			if (hasEnemy)
			{
				bool flag9 = !isFirst;
				if (flag9)
				{
					sb.Append(LanguageKey.LK_Separator.Tr());
				}
				sb.Append(LanguageKey.LK_SecretInformation_Detail_Effect_Enemy.Tr().SetColor("brightred"));
				isFirst = false;
			}
			foreach (short id in effectData.Fame)
			{
				bool flag10 = !isFirst;
				if (flag10)
				{
					sb.Append(LanguageKey.LK_Separator.Tr());
				}
				sb.Append(CommonUtils.GetFameString(FameType.GetFameType(FameAction.Instance[id].Fame)));
				isFirst = false;
			}
			foreach (ShortPair pair in effectData.Punish)
			{
				bool flag11 = !isFirst;
				if (flag11)
				{
					sb.Append(LanguageKey.LK_Separator.Tr());
				}
				sb.Append(PunishmentType.Instance[pair.First].ShortName.SetColor(PunishmentSeverity.Instance[(int)pair.Second].NameColor));
				isFirst = false;
			}
			this.descLabel.text = sb.ToString();
		}

		// Token: 0x04008874 RID: 34932
		[SerializeField]
		protected Game.Components.Avatar.Avatar avatar;

		// Token: 0x04008875 RID: 34933
		[SerializeField]
		protected TextMeshProUGUI nameLabel;

		// Token: 0x04008876 RID: 34934
		[SerializeField]
		protected TextMeshProUGUI typeLabel;

		// Token: 0x04008877 RID: 34935
		[SerializeField]
		protected TextMeshProUGUI descLabel;
	}
}
