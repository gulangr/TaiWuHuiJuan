using System;
using Coffee.UIExtensions;
using Config;
using Game.Components.Avatar;
using TMPro;
using UICommon.Character;
using UnityEngine;

namespace Game.Views.Villager
{
	// Token: 0x0200073E RID: 1854
	public class SwordTombPiece : MonoBehaviour
	{
		// Token: 0x060059CB RID: 22987 RVA: 0x0029A290 File Offset: 0x00298490
		public void Set(CharacterItem charCfg, bool isSwordTomb)
		{
			ResLoader.LoadModOrGameResource<Texture2D>(CharacterAvatar.GetNpcFaceResPath(CharacterAvatar.GetAvatarSizeFolder(this.avatar.Size), charCfg.FixedAvatarName), new Action<Texture2D>(this.avatar.Refresh), null);
			this.tombName.text = NameCenter.FormatName(charCfg.Surname, charCfg.GivenName);
			this.active.SetActive(isSwordTomb);
			this.deactive.SetActive(!isSwordTomb);
		}

		// Token: 0x060059CC RID: 22988 RVA: 0x0029A30B File Offset: 0x0029850B
		public void SetCount(int cnt)
		{
			this.count.text = cnt.ToString();
		}

		// Token: 0x060059CD RID: 22989 RVA: 0x0029A320 File Offset: 0x00298520
		public void SetRemoved(string str)
		{
			this.removed.text = str;
		}

		// Token: 0x04003DB7 RID: 15799
		[SerializeField]
		private TMP_Text tombName;

		// Token: 0x04003DB8 RID: 15800
		[SerializeField]
		private TMP_Text count;

		// Token: 0x04003DB9 RID: 15801
		[SerializeField]
		private TMP_Text removed;

		// Token: 0x04003DBA RID: 15802
		[SerializeField]
		private Game.Components.Avatar.Avatar avatar;

		// Token: 0x04003DBB RID: 15803
		[SerializeField]
		private GameObject active;

		// Token: 0x04003DBC RID: 15804
		[SerializeField]
		private GameObject deactive;

		// Token: 0x04003DBD RID: 15805
		[SerializeField]
		private UIParticle border;
	}
}
