using System;
using FrameWork;
using Game.Components.Information;
using GameData.Domains.Information;
using GameData.Serializer;
using GameData.Utilities;
using UnityEngine;

namespace Game.Views.CharacterMenu
{
	// Token: 0x02000BAD RID: 2989
	public class ViewCharacterMenuSecret : UI_CharacterMenuSubPageBase
	{
		// Token: 0x1700102E RID: 4142
		// (get) Token: 0x06009668 RID: 38504 RVA: 0x00462D5D File Offset: 0x00460F5D
		public override LanguageKey TitleKey
		{
			get
			{
				return LanguageKey.LK_SecretInformation;
			}
		}

		// Token: 0x06009669 RID: 38505 RVA: 0x00462D64 File Offset: 0x00460F64
		public override bool CheckState(ECharacterSubToggleBase curSubTogglePage, ECharacterSubPage curSubPage)
		{
			return curSubTogglePage == ECharacterSubToggleBase.InformationBase && curSubPage == ECharacterSubPage.Secret;
		}

		// Token: 0x0600966A RID: 38506 RVA: 0x00462D83 File Offset: 0x00460F83
		public override void OnInit(ArgumentBox argsBox)
		{
		}

		// Token: 0x0600966B RID: 38507 RVA: 0x00462D86 File Offset: 0x00460F86
		private void Awake()
		{
			this.panel.Init(null);
		}

		// Token: 0x0600966C RID: 38508 RVA: 0x00462D96 File Offset: 0x00460F96
		public override void OnSubpageInVisible()
		{
			this.panel.dateSelector.openSelector.isOn = false;
			this.panel.levelSettingSwitchToggle.isOn = false;
		}

		// Token: 0x0600966D RID: 38509 RVA: 0x00462DC4 File Offset: 0x00460FC4
		public override void OnCurrentCharacterChange(int prevCharacterId)
		{
			bool flag = base.CharacterMenu.CurCharacterId < 0;
			if (!flag)
			{
				this.panel.SetController(base.CharacterMenu.CurrentCharacterIsTaiwu);
				bool flag2 = prevCharacterId != base.CharacterMenu.CurCharacterId;
				if (flag2)
				{
					this.RequestData();
					this.localLoadingAnim.SetLoadingState(true);
				}
			}
		}

		// Token: 0x0600966E RID: 38510 RVA: 0x00462E28 File Offset: 0x00461028
		private void RequestData()
		{
			this.panel.Clear();
			InformationDomainMethod.AsyncCall.GetSecretInformationDisplayPackageFromCharacter(this, base.CharacterMenu.CurCharacterId, delegate(int offset, RawDataPool pool)
			{
				SecretInformationDisplayPackage pack = null;
				Serializer.Deserialize(pool, offset, ref pack);
				this.GenerateData(pack, 0);
			});
			InformationDomainMethod.AsyncCall.GetSecretInformationDisplayPackageFromBroadcast(this, base.CharacterMenu.CurCharacterId, delegate(int offset, RawDataPool pool)
			{
				SecretInformationDisplayPackage pack = null;
				Serializer.Deserialize(pool, offset, ref pack);
				this.GenerateData(pack, 1);
			});
		}

		// Token: 0x0600966F RID: 38511 RVA: 0x00462E80 File Offset: 0x00461080
		private void GenerateData(SecretInformationDisplayPackage package, int index)
		{
			this.panel.Set(package, index, true);
			bool flag = index == this.panel.ownerToggleGroup.GetActiveIndex();
			if (flag)
			{
				this.Element.ShowAfterRefresh();
				this.localLoadingAnim.SetLoadingState(false);
			}
		}

		// Token: 0x04007369 RID: 29545
		[SerializeField]
		private SecretInformationPanel panel;

		// Token: 0x0400736A RID: 29546
		[SerializeField]
		private CharacterMenuLocalLoadingAnim localLoadingAnim;
	}
}
