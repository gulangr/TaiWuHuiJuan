using System;
using Game.Views.Encyclopedia.Event;
using TMPro;
using UnityEngine;
using UnityEngine.Events;

namespace Game.Views.Encyclopedia.Views
{
	// Token: 0x02000A68 RID: 2664
	internal class FavoriteItem : MonoBehaviour
	{
		// Token: 0x06008367 RID: 33639 RVA: 0x003D339C File Offset: 0x003D159C
		private void Awake()
		{
			this.favoriteIcon = base.transform.Find("ThirdTagTitle").Find("FavoriteTag").GetComponent<CToggleObsolete>();
			this.favoriteIcon.onValueChanged.AddListener(new UnityAction<bool>(this.OnFavoriteTypeChanged));
		}

		// Token: 0x06008368 RID: 33640 RVA: 0x003D33EC File Offset: 0x003D15EC
		public void Init(FavoriteInfo favoriteInfo)
		{
			this.favoriteInfo = favoriteInfo;
			this.favoriteName.text = favoriteInfo.Title;
		}

		// Token: 0x06008369 RID: 33641 RVA: 0x003D3408 File Offset: 0x003D1608
		private void OnFavoriteTypeChanged(bool isFavorite)
		{
			FavoriteTypeChangedEventArgs args = new FavoriteTypeChangedEventArgs
			{
				ToFavorite = isFavorite
			};
			IEventArgs arg = EventArgs<FavoriteTypeChangedEventArgs>.CreateEventArgs(args);
			EventManager.Instance.Dispatch(2, arg);
		}

		// Token: 0x04006496 RID: 25750
		[SerializeField]
		private TextMeshProUGUI favoriteName;

		// Token: 0x04006497 RID: 25751
		[SerializeField]
		private CToggleObsolete favoriteIcon;

		// Token: 0x04006498 RID: 25752
		private FavoriteInfo favoriteInfo;
	}
}
