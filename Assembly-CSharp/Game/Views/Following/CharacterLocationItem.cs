using System;
using GameData.Domains.Character.Display;
using TMPro;
using UnityEngine;

namespace Game.Views.Following
{
	// Token: 0x02000A22 RID: 2594
	public class CharacterLocationItem : MonoBehaviour
	{
		// Token: 0x06007F3F RID: 32575 RVA: 0x003B46E0 File Offset: 0x003B28E0
		public void RefreshImpl(CharacterLocationDisplayData locationDisplayData, Func<string, string> areaName)
		{
			bool flag = locationDisplayData.BlockData != null || locationDisplayData.RootBlockData != null;
			if (flag)
			{
				this.unknown.SetActive(false);
				this.mapBlockView.gameObject.SetActive(true);
				this.mapBlockView.Refresh(locationDisplayData.BlockData, locationDisplayData.RootBlockData);
			}
			else
			{
				this.unknown.SetActive(true);
				this.mapBlockView.gameObject.SetActive(false);
			}
			this.characterInfoLabel.text = areaName(CommonUtils.GetLocationName(locationDisplayData));
			this.characterInfoLabelHelper.Parse();
		}

		// Token: 0x06007F40 RID: 32576 RVA: 0x003B4788 File Offset: 0x003B2988
		public void Refresh(CharacterLocationDisplayData locationDisplayData)
		{
			this.RefreshImpl(locationDisplayData, delegate(string locationName)
			{
				CharacterLocationDisplayData.EDisplayType displayType = (CharacterLocationDisplayData.EDisplayType)locationDisplayData.DisplayType;
				if (!true)
				{
				}
				string result;
				switch (displayType)
				{
				case CharacterLocationDisplayData.EDisplayType.Normal:
					result = LanguageKey.LK_LocationItem_Normal.TrFormat(locationName).ColorReplace();
					goto IL_A9;
				case CharacterLocationDisplayData.EDisplayType.Kidnapped:
					result = LanguageKey.LK_LocationItem_Kidnapped.TrFormat(locationName, (locationDisplayData.Kidnapper == null) ? "" : NameCenter.GetMonasticTitleOrDisplayName(locationDisplayData.Kidnapper, locationDisplayData.Kidnapper.CharacterId == SingletonObject.getInstance<BasicGameData>().TaiwuCharId)).ColorReplace();
					goto IL_A9;
				case CharacterLocationDisplayData.EDisplayType.Buried:
					result = LanguageKey.LK_LocationItem_Dead.TrFormat(locationName).ColorReplace();
					goto IL_A9;
				}
				result = "";
				IL_A9:
				if (!true)
				{
				}
				return result;
			});
		}

		// Token: 0x04006159 RID: 24921
		[SerializeField]
		private MapBlockView mapBlockView;

		// Token: 0x0400615A RID: 24922
		[SerializeField]
		private TMP_Text characterInfoLabel;

		// Token: 0x0400615B RID: 24923
		[SerializeField]
		private GameObject unknown;

		// Token: 0x0400615C RID: 24924
		[SerializeField]
		private TMPTextSpriteHelper characterInfoLabelHelper;
	}
}
