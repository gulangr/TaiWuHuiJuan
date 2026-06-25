using System;
using FrameWork;
using FrameWork.UISystem.UIElements;
using Game.Components.Avatar;
using GameData.Domains.Character;
using GameData.Domains.Character.Display;
using GameData.Serializer;
using GameData.Utilities;
using TMPro;
using UnityEngine;

namespace Game.Views.MonthNotify
{
	// Token: 0x020008BC RID: 2236
	public class MonthNotifyFirstPageCharacterItem : MonoBehaviour
	{
		// Token: 0x06006AA5 RID: 27301 RVA: 0x00313D40 File Offset: 0x00311F40
		public void Set(int charId, NameRelatedData nameData, AvatarRelatedData avatarData)
		{
			this.characterName.text = NameCenter.GetMonasticTitleOrDisplayName(ref nameData, false, false);
			this.avatar.Refresh(avatarData);
			this.btn.enabled = false;
			CharacterDomainMethod.AsyncCall.GetCharacterDisplayData(null, charId, delegate(int offset, RawDataPool pool)
			{
				CharacterDisplayData data = null;
				Serializer.Deserialize(pool, offset, ref data);
				bool flag = data.AliveState != 0;
				if (!flag)
				{
					bool flag2 = this == null;
					if (!flag2)
					{
						this.tips.NeedRefresh = true;
						this.tips.RuntimeParam = new ArgumentBox().Set("charId", data.CharacterId);
						this.btn.ClearAndAddListener(delegate
						{
							this.ShowCharacterMenu(data.CharacterId);
						});
						this.btn.enabled = true;
					}
				}
			});
		}

		// Token: 0x06006AA6 RID: 27302 RVA: 0x00313D94 File Offset: 0x00311F94
		private void ShowCharacterMenu(int id)
		{
			bool flag = id >= 0;
			if (flag)
			{
				ArgumentBox argBox = EasyPool.Get<ArgumentBox>();
				argBox.Set("CharacterId", id);
				argBox.Set("CanOperate", false);
				UIElement.CharacterMenu.SetOnInitArgs(argBox);
				UIManager.Instance.StackToUI(UIElement.CharacterMenu);
			}
		}

		// Token: 0x04004D0F RID: 19727
		[SerializeField]
		private TextMeshProUGUI characterName;

		// Token: 0x04004D10 RID: 19728
		[SerializeField]
		private Game.Components.Avatar.Avatar avatar;

		// Token: 0x04004D11 RID: 19729
		[SerializeField]
		private TooltipInvoker tips;

		// Token: 0x04004D12 RID: 19730
		[SerializeField]
		private CButton btn;
	}
}
