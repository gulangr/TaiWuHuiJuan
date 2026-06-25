using System;
using Config;
using FrameWork;
using FrameWork.UISystem.UIElements;
using TMPro;
using UnityEngine;

namespace Game.Views.MusicPlayer
{
	// Token: 0x0200081D RID: 2077
	public class MusicItem : MonoBehaviour
	{
		// Token: 0x17000C52 RID: 3154
		// (get) Token: 0x06006602 RID: 26114 RVA: 0x002E96A1 File Offset: 0x002E78A1
		private MusicPlayerModel Model
		{
			get
			{
				return SingletonObject.getInstance<MusicPlayerModel>();
			}
		}

		// Token: 0x06006603 RID: 26115 RVA: 0x002E96A8 File Offset: 0x002E78A8
		public void Set(short id)
		{
			this._id = id;
			bool isLock = this.Model.IsMusicLock(id);
			bool isSelected = this.Model.IsMusicSelected(id);
			bool isPlaying = this.Model.IsMusicPlaying(id);
			this.button.interactable = !isLock;
			this.tip.Type = TipType.Music;
			this.tip.enabled = !isLock;
			this.tip.RuntimeParam = EasyPool.Get<ArgumentBox>().Set("MusicId", id);
			this.lockRoot.SetActive(isLock);
			this.unLockRoot.SetActive(!isLock);
			MusicItem musicConfig = Music.Instance[id];
			this.imageIcon.SetSprite(musicConfig.Icon, false, null);
			this.textName.text = (isPlaying ? musicConfig.Name.SetColor("lightblue") : musicConfig.Name);
			string areaName = ViewMusicPlayer.GetMusicAreaName(id);
			this.textArea.text = (isPlaying ? areaName.SetColor("lightblue") : areaName);
			this.textUnlock.text = LanguageKey.LK_MusicPlayer_Content_Unlock.TrFormat(areaName);
			int duration = (int)this.Model.GetMusicDuration(id);
			string durationStr = ViewMusicPlayer.GetTimeStr(duration);
			this.textDuration.text = (isPlaying ? durationStr.SetColor("lightblue") : durationStr);
			string activeStr = isPlaying ? LanguageKey.LK_MusicPlayer_Effect_Active.Tr().SetColor("lightblue") : string.Empty;
			string effectValueStr = ViewMusicPlayer.GetEffectDesc(id, false);
			string effectStr = activeStr + effectValueStr;
			this.textEffect.text = effectStr;
			this.selectedObj.SetActive(isSelected);
			this.RefreshButtonFavorite();
			this.imagePlaying.SetActive(isPlaying);
			bool flag = isPlaying;
			if (flag)
			{
				bool flag2 = !this._objBackPlayingEffect;
				if (flag2)
				{
					this._objBackPlayingEffect = this.backEffectPlayer.PlayEffectAt(this.backEffectPlayer.transform, MusicItem.BackPlayingEffectName, 0f, false);
				}
				bool flag3 = !this._objIconPlayingEffect;
				if (flag3)
				{
					this._objIconPlayingEffect = this.iconEffectPlayer.PlayEffectAt(this.iconEffectPlayer.transform, MusicItem.IconPlayingEffectName, 0f, false);
				}
			}
			else
			{
				bool flag4 = this._objBackPlayingEffect;
				if (flag4)
				{
					this.backEffectPlayer.ReturnEffectObject(this._objBackPlayingEffect);
					this._objBackPlayingEffect = null;
				}
				bool flag5 = this._objIconPlayingEffect;
				if (flag5)
				{
					this.iconEffectPlayer.ReturnEffectObject(this._objIconPlayingEffect);
					this._objIconPlayingEffect = null;
				}
			}
		}

		// Token: 0x06006604 RID: 26116 RVA: 0x002E9948 File Offset: 0x002E7B48
		private void Awake()
		{
			this.button.ClearAndAddListener(new Action(this.OnClick));
			this.buttonAddFavorite.ClearAndAddListener(new Action(this.OnClickAddFavorite));
			this.buttonRemoveFavorite.ClearAndAddListener(new Action(this.OnClickRemoveFavorite));
		}

		// Token: 0x06006605 RID: 26117 RVA: 0x002E99A0 File Offset: 0x002E7BA0
		private void OnClick()
		{
			bool flag = this.Model.MusicId == this._id;
			if (flag)
			{
				bool isPlaying = this.Model.IsPlaying;
				if (isPlaying)
				{
					this.Model.PauseMusic(false);
				}
				else
				{
					bool isPaused = this.Model.IsPaused;
					if (isPaused)
					{
						this.Model.ResumeMusic();
					}
					else
					{
						this.Model.PlayMusic(this._id, 0f);
					}
				}
			}
			else
			{
				this.Model.PlayMusic(this._id, 0f);
			}
		}

		// Token: 0x06006606 RID: 26118 RVA: 0x002E9A31 File Offset: 0x002E7C31
		private void OnClickAddFavorite()
		{
			this.Model.AddFavorite(this._id);
			this.RefreshButtonFavorite();
			ViewMusicPlayer.TryRefreshMusicRowAfterFavoriteChange(this._id);
		}

		// Token: 0x06006607 RID: 26119 RVA: 0x002E9A59 File Offset: 0x002E7C59
		private void OnClickRemoveFavorite()
		{
			this.Model.RemoveFavorite(this._id);
			this.RefreshButtonFavorite();
			ViewMusicPlayer.TryRefreshMusicRowAfterFavoriteChange(this._id);
		}

		// Token: 0x06006608 RID: 26120 RVA: 0x002E9A84 File Offset: 0x002E7C84
		private void RefreshButtonFavorite()
		{
			bool isFavorite = this.Model.IsFavorite(this._id);
			this.buttonAddFavorite.gameObject.SetActive(!isFavorite);
			this.buttonRemoveFavorite.gameObject.SetActive(isFavorite);
		}

		// Token: 0x06006609 RID: 26121 RVA: 0x002E9ACC File Offset: 0x002E7CCC
		public void RefreshSelectedSummary(short id)
		{
			this._id = id;
			bool flag = id < 0;
			if (flag)
			{
				base.gameObject.SetActive(false);
			}
			else
			{
				base.gameObject.SetActive(true);
				MusicItem musicConfig = Music.Instance[id];
				this.imageIcon.SetSprite(musicConfig.Icon, false, null);
				this.textName.text = musicConfig.Name;
				string areaName = ViewMusicPlayer.GetMusicAreaName(id);
				this.textArea.text = areaName;
				this.RefreshButtonFavorite();
			}
		}

		// Token: 0x0400473E RID: 18238
		[SerializeField]
		private CButton button;

		// Token: 0x0400473F RID: 18239
		[SerializeField]
		private TooltipInvoker tip;

		// Token: 0x04004740 RID: 18240
		[SerializeField]
		private GameObject lockRoot;

		// Token: 0x04004741 RID: 18241
		[SerializeField]
		private GameObject unLockRoot;

		// Token: 0x04004742 RID: 18242
		[SerializeField]
		private CImage imageIcon;

		// Token: 0x04004743 RID: 18243
		[SerializeField]
		private TextMeshProUGUI textName;

		// Token: 0x04004744 RID: 18244
		[SerializeField]
		private TextMeshProUGUI textArea;

		// Token: 0x04004745 RID: 18245
		[SerializeField]
		private TextMeshProUGUI textUnlock;

		// Token: 0x04004746 RID: 18246
		[SerializeField]
		private TextMeshProUGUI textEffect;

		// Token: 0x04004747 RID: 18247
		[SerializeField]
		private TextMeshProUGUI textDuration;

		// Token: 0x04004748 RID: 18248
		[SerializeField]
		private CButton buttonAddFavorite;

		// Token: 0x04004749 RID: 18249
		[SerializeField]
		private CButton buttonRemoveFavorite;

		// Token: 0x0400474A RID: 18250
		[SerializeField]
		private GameObject selectedObj;

		// Token: 0x0400474B RID: 18251
		[SerializeField]
		private GameObject imagePlaying;

		// Token: 0x0400474C RID: 18252
		[SerializeField]
		private EffectPlayer backEffectPlayer;

		// Token: 0x0400474D RID: 18253
		[SerializeField]
		private EffectPlayer iconEffectPlayer;

		// Token: 0x0400474E RID: 18254
		private static readonly string BackPlayingEffectName = "eff_xuannv_ui_beijingwenli";

		// Token: 0x0400474F RID: 18255
		private static readonly string IconPlayingEffectName = "eff_xuannv_ui_bofangqi";

		// Token: 0x04004750 RID: 18256
		private GameObject _objBackPlayingEffect;

		// Token: 0x04004751 RID: 18257
		private GameObject _objIconPlayingEffect;

		// Token: 0x04004752 RID: 18258
		private short _id;
	}
}
