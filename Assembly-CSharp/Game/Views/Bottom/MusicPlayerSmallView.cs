using System;
using Config;
using FrameWork;
using FrameWork.UISystem.UIElements;
using TMPro;
using UnityEngine;

namespace Game.Views.Bottom
{
	// Token: 0x02000C3F RID: 3135
	public class MusicPlayerSmallView : MonoBehaviour
	{
		// Token: 0x170010CE RID: 4302
		// (get) Token: 0x06009F3E RID: 40766 RVA: 0x004A6BFF File Offset: 0x004A4DFF
		private MusicPlayerModel Model
		{
			get
			{
				return SingletonObject.getInstance<MusicPlayerModel>();
			}
		}

		// Token: 0x06009F3F RID: 40767 RVA: 0x004A6C08 File Offset: 0x004A4E08
		public void Init()
		{
			this.buttonOpen.ClearAndAddListener(delegate
			{
				UIManager.Instance.MaskUI(UIElement.MusicPlayer);
			});
			this.buttonPlay.ClearAndAddListener(delegate
			{
				this.buttonPlay.interactable = false;
				this.Model.PlayMusic(this.Model.PausedMusicProgress);
			});
			this.buttonPause.ClearAndAddListener(delegate
			{
				this.buttonPause.interactable = false;
				this.Model.StopMusic();
			});
			this.buttonLast.ClearAndAddListener(delegate
			{
				this.buttonLast.interactable = false;
				this.Model.PlayLastMusic(null);
			});
			this.buttonNext.ClearAndAddListener(delegate
			{
				this.buttonNext.interactable = false;
				this.Model.PlayNextMusic(null);
			});
			bool flag = !this.Model.ResumeMusic();
			if (flag)
			{
				this.RefreshPlayState();
			}
		}

		// Token: 0x06009F40 RID: 40768 RVA: 0x004A6CBC File Offset: 0x004A4EBC
		public void RefreshPlayState()
		{
			MusicItem musicConfig = this.Model.MusicConfig;
			string name = ((musicConfig != null) ? musicConfig.Name : null) ?? string.Empty;
			string color = this.Model.IsPlaying ? "brightblue" : "brightyellow";
			this.title.text = name.SetColor(color);
			this.buttonPlay.gameObject.SetActive(!this.Model.IsPlaying);
			this.buttonPause.gameObject.SetActive(this.Model.IsPlaying);
			this.buttonOpen.interactable = this.Model.Interactable;
			this.RefreshButton(this.buttonPlay, this.Model.Interactable);
			this.RefreshButton(this.buttonPause, this.Model.Interactable);
			this.RefreshButton(this.buttonLast, this.Model.Interactable);
			this.RefreshButton(this.buttonNext, this.Model.Interactable);
			this.tip.enabled = true;
			this.tip.Type = TipType.Music;
			this.tip.RuntimeParam = EasyPool.Get<ArgumentBox>().Set("MusicId", this.Model.MusicId);
		}

		// Token: 0x06009F41 RID: 40769 RVA: 0x004A6E0A File Offset: 0x004A500A
		private void RefreshButton(CButton button, bool interactable)
		{
			button.interactable = interactable;
		}

		// Token: 0x06009F42 RID: 40770 RVA: 0x004A6E15 File Offset: 0x004A5015
		public void RefreshShowState()
		{
			base.gameObject.SetActive(this.Model.CanShow);
		}

		// Token: 0x04007B22 RID: 31522
		[SerializeField]
		private TextMeshProUGUI title;

		// Token: 0x04007B23 RID: 31523
		[SerializeField]
		private TooltipInvoker tip;

		// Token: 0x04007B24 RID: 31524
		[SerializeField]
		private CButton buttonOpen;

		// Token: 0x04007B25 RID: 31525
		[SerializeField]
		private CButton buttonPlay;

		// Token: 0x04007B26 RID: 31526
		[SerializeField]
		private CButton buttonPause;

		// Token: 0x04007B27 RID: 31527
		[SerializeField]
		private CButton buttonLast;

		// Token: 0x04007B28 RID: 31528
		[SerializeField]
		private CButton buttonNext;
	}
}
