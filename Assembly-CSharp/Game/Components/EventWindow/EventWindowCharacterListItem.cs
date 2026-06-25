using System;
using FrameWork.UISystem.UIElements;
using Game.Components.Avatar;
using GameData.Domains.Character.Display;
using TMPro;
using UnityEngine;

namespace Game.Components.EventWindow
{
	// Token: 0x02000EFE RID: 3838
	public class EventWindowCharacterListItem : MonoBehaviour
	{
		// Token: 0x0600B109 RID: 45321 RVA: 0x0050BDB7 File Offset: 0x00509FB7
		private void Awake()
		{
			this.btnSearch.ClearAndAddListener(delegate
			{
				Action actionOnSearch = this._actionOnSearch;
				if (actionOnSearch != null)
				{
					actionOnSearch();
				}
			});
			this.btnDelete.ClearAndAddListener(delegate
			{
				Action actionOnDelete = this._actionOnDelete;
				if (actionOnDelete != null)
				{
					actionOnDelete();
				}
			});
		}

		// Token: 0x0600B10A RID: 45322 RVA: 0x0050BDEA File Offset: 0x00509FEA
		public void SetEmpty()
		{
			this.avatar.ResetToBlank(false);
			this.characterName.text = string.Empty;
		}

		// Token: 0x0600B10B RID: 45323 RVA: 0x0050BE0C File Offset: 0x0050A00C
		public void Set(CharacterDisplayData data, Action actionOnSearch, Action actionOnDelete)
		{
			this._actionOnSearch = actionOnSearch;
			this._actionOnDelete = actionOnDelete;
			bool flag = data == null;
			if (flag)
			{
				this.SetEmpty();
			}
			else
			{
				this.SetBasic(data);
			}
		}

		// Token: 0x0600B10C RID: 45324 RVA: 0x0050BE44 File Offset: 0x0050A044
		public void SetApprove(CharacterDisplayData data, short approve, Action actionOnSearch, Action actionOnDelete)
		{
			this._actionOnSearch = actionOnSearch;
			this._actionOnDelete = actionOnDelete;
			bool flag = data == null;
			if (flag)
			{
				this.SetEmpty();
			}
			else
			{
				this.SetBasic(data);
			}
			this.characterName.text = approve.ToString();
		}

		// Token: 0x0600B10D RID: 45325 RVA: 0x0050BE8D File Offset: 0x0050A08D
		public void SetBasic(AvatarRelatedData avatarData, string nameData)
		{
			this.avatar.Refresh(avatarData);
			this.characterName.text = nameData;
		}

		// Token: 0x0600B10E RID: 45326 RVA: 0x0050BEAA File Offset: 0x0050A0AA
		public void SetName(string charName)
		{
			this.characterName.text = charName;
		}

		// Token: 0x0600B10F RID: 45327 RVA: 0x0050BEB9 File Offset: 0x0050A0B9
		private void SetBasic(CharacterDisplayData data)
		{
			this.avatar.Refresh(data, true);
			this.characterName.text = NameCenter.GetMonasticTitleOrDisplayName(data, data.CharacterId == SingletonObject.getInstance<BasicGameData>().TaiwuCharId);
		}

		// Token: 0x04008913 RID: 35091
		[SerializeField]
		protected Game.Components.Avatar.Avatar avatar;

		// Token: 0x04008914 RID: 35092
		[SerializeField]
		protected TextMeshProUGUI characterName;

		// Token: 0x04008915 RID: 35093
		[SerializeField]
		public TooltipInvoker mouseTip;

		// Token: 0x04008916 RID: 35094
		[SerializeField]
		public CButton btnDelete;

		// Token: 0x04008917 RID: 35095
		[SerializeField]
		public CButton btnSearch;

		// Token: 0x04008918 RID: 35096
		private Action _actionOnSearch;

		// Token: 0x04008919 RID: 35097
		private Action _actionOnDelete;
	}
}
