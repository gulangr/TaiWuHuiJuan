using System;
using FrameWork;
using GameData.Domains.Character.Display;
using GameData.Domains.Taiwu;
using GameData.Serializer;
using GameData.Utilities;
using UnityEngine;

namespace Game.Views.MapBlockCharList
{
	// Token: 0x02000933 RID: 2355
	public class FollowingChar : MapBlockChar
	{
		// Token: 0x17000CB5 RID: 3253
		// (get) Token: 0x06006DDD RID: 28125 RVA: 0x0032C4C4 File Offset: 0x0032A6C4
		// (set) Token: 0x06006DDE RID: 28126 RVA: 0x0032C4E1 File Offset: 0x0032A6E1
		public bool Selected
		{
			get
			{
				return this.selected && this.selected.activeSelf;
			}
			set
			{
				this.selected.SetActive(value);
			}
		}

		// Token: 0x06006DDF RID: 28127 RVA: 0x0032C4F0 File Offset: 0x0032A6F0
		public void OnSubmitRenameDone(string newName)
		{
			TaiwuDomainMethod.AsyncCall.SetFollowingNpcNickName(this.RequestHandler, this.CharId, newName, delegate(int offset, RawDataPool pool)
			{
				int nickNameKey = -1;
				Serializer.Deserialize(pool, offset, ref nickNameKey);
				GEvent.OnEvent(UiEvents.NickNameChanged, EasyPool.Get<ArgumentBox>().Set("CharacterId", this.CharId).Set("NickNameKey", nickNameKey).Set("NickName", newName));
				this._cd.NickNameId = nickNameKey;
			});
			bool flag = string.IsNullOrWhiteSpace(newName);
			if (flag)
			{
				this.Refresh();
			}
			else
			{
				this.nameText.text = newName;
			}
		}

		// Token: 0x06006DE0 RID: 28128 RVA: 0x0032C560 File Offset: 0x0032A760
		public void Refresh()
		{
			TaiwuDomainMethod.AsyncCall.RequestFollowingCharacter(this.RequestHandler, this.CharId, delegate(int offset, RawDataPool pool)
			{
				TaiwuFollowingDisplayData data = new TaiwuFollowingDisplayData();
				Serializer.Deserialize(pool, offset, ref data);
				this.Set(this._parent, data.Display, false, true);
			});
		}

		// Token: 0x06006DE1 RID: 28129 RVA: 0x0032C584 File Offset: 0x0032A784
		public override void Set(IMapBlockCharHolder parent, CharacterDisplayData charDisplayData, bool isSpecialNpc, bool isActive = true)
		{
			this._cd = charDisplayData;
			this.CharName = NameCenter.GetNameRelatedData(charDisplayData);
			base.Set(parent, charDisplayData, isSpecialNpc, isActive);
		}

		// Token: 0x0400517F RID: 20863
		[SerializeField]
		private GameObject selected;

		// Token: 0x04005180 RID: 20864
		public IAsyncMethodRequestHandler RequestHandler;

		// Token: 0x04005181 RID: 20865
		public NameRelatedData CharName;

		// Token: 0x04005182 RID: 20866
		private CharacterDisplayData _cd;
	}
}
