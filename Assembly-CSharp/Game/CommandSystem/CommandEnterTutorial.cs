using System;
using System.Runtime.CompilerServices;
using Config;
using FrameWork;
using FrameWork.CommandSystem;
using GameData.Domains.TutorialChapter;
using GameData.GameDataBridge;

namespace Game.CommandSystem
{
	// Token: 0x02000C89 RID: 3209
	public class CommandEnterTutorial : BaseCommand
	{
		// Token: 0x0600A3A2 RID: 41890 RVA: 0x004C8F74 File Offset: 0x004C7174
		public override bool Execute()
		{
			GameApp.Instance.ChangeGameState(EGameState.Loading, EasyPool.Get<ArgumentBox>().SetObject("OnLoadingStart", new Action(this.<Execute>g__OnLoadStarted|2_0)).SetObject("OnLoadingFinish", new Action(this.<Execute>g__OnLoadFinished|2_1)));
			this._finished = false;
			return true;
		}

		// Token: 0x17001114 RID: 4372
		// (get) Token: 0x0600A3A3 RID: 41891 RVA: 0x004C8FCD File Offset: 0x004C71CD
		public override bool Done
		{
			get
			{
				return this._finished;
			}
		}

		// Token: 0x0600A3A4 RID: 41892 RVA: 0x004C8FD5 File Offset: 0x004C71D5
		public override void Reset()
		{
			this.Collect();
			this._finished = false;
		}

		// Token: 0x0600A3A5 RID: 41893 RVA: 0x004C8FE6 File Offset: 0x004C71E6
		public override void Collect()
		{
			this.TutorialChapter = null;
		}

		// Token: 0x0600A3A7 RID: 41895 RVA: 0x004C8FFC File Offset: 0x004C71FC
		[CompilerGenerated]
		private void <Execute>g__OnLoadStarted|2_0()
		{
			GameApp.ResetGameSubPageState();
			UIElement.CharacterMenu.UiBase.OnReset();
			GlobalOperations.EnterTutorialWorld(this.TutorialChapter.TemplateId);
			SingletonObject.getInstance<TutorialChapterModel>();
			GEvent.OnEvent(EEvents.LoadingProgress, EasyPool.Get<ArgumentBox>().Set("Progress", 50));
		}

		// Token: 0x0600A3A8 RID: 41896 RVA: 0x004C9054 File Offset: 0x004C7254
		[CompilerGenerated]
		private void <Execute>g__OnLoadFinished|2_1()
		{
			GameApp.Instance.ChangeGameState(EGameState.InGame, null);
			TutorialChapterDomainMethod.Call.StartChapter((int)this.TutorialChapter.TemplateId);
			this._finished = true;
		}

		// Token: 0x040081A5 RID: 33189
		public TutorialChaptersItem TutorialChapter;

		// Token: 0x040081A6 RID: 33190
		private bool _finished;
	}
}
