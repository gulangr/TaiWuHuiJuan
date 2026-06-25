using System;
using FrameWork.UISystem.UIElements;
using Game.Components.Avatar;
using GameData.Domains.Character;
using GameData.Domains.Character.Display;
using GameData.Serializer;
using GameData.Utilities;
using UnityEngine;

namespace Game.Views.Bottom
{
	// Token: 0x02000C2C RID: 3116
	[RequireComponent(typeof(CButton), typeof(CImage))]
	public class AssignedCharacterButton : BlockButton
	{
		// Token: 0x06009E5D RID: 40541 RVA: 0x004A119C File Offset: 0x0049F39C
		public override void Init(IBlockButtonParent parent, byte buttonTemplateId)
		{
			base.Init(parent, buttonTemplateId);
			bool flag = parent.CharId == -1;
			if (!flag)
			{
				CharacterDomainMethod.AsyncCall.GetCharacterDisplayData(parent, parent.CharId, delegate(int offset, RawDataPool dataPool)
				{
					CharacterDisplayData displayData = null;
					Serializer.Deserialize(dataPool, offset, ref displayData);
					bool flag2 = displayData == null;
					if (flag2)
					{
						Debug.LogError(string.Format("cannot got {0}'s display data", parent.CharId));
					}
					else
					{
						this.buttonText.text = NameCenter.GetNameByDisplayData(displayData, false, false);
						this.characterAvatar.Refresh(displayData, true);
					}
				});
			}
		}

		// Token: 0x04007A77 RID: 31351
		[SerializeField]
		private Game.Components.Avatar.Avatar characterAvatar;
	}
}
