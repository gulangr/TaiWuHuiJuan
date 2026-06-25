using System;
using Game.Components.Avatar;
using GameData.Domains.Character;
using GameData.Domains.Character.Display;
using GameData.Serializer;
using GameData.Utilities;
using TMPro;
using UnityEngine;

namespace Game.Components.Character
{
	// Token: 0x02000F11 RID: 3857
	public class AvatarNormalWithName : MonoBehaviour
	{
		// Token: 0x0600B1D4 RID: 45524 RVA: 0x005102A4 File Offset: 0x0050E4A4
		public void Set(IAsyncMethodRequestHandler parent, int charId)
		{
			CharacterDomainMethod.AsyncCall.GetCharacterDisplayData(parent, charId, delegate(int offset, RawDataPool pool)
			{
				CharacterDisplayData cdata = null;
				Serializer.Deserialize(pool, offset, ref cdata);
				bool flag = cdata.AliveState == 0;
				if (flag)
				{
					this.avatar.Refresh(cdata, cdata.AliveState == 0);
				}
				else
				{
					this.avatar.RefreshAsGrave();
				}
				this.nameText.text = NameCenter.GetMonasticTitleOrDisplayName(cdata, SingletonObject.getInstance<BasicGameData>().TaiwuCharId == charId);
			});
		}

		// Token: 0x040089CD RID: 35277
		[SerializeField]
		private Game.Components.Avatar.Avatar avatar;

		// Token: 0x040089CE RID: 35278
		[SerializeField]
		private TMP_Text nameText;
	}
}
