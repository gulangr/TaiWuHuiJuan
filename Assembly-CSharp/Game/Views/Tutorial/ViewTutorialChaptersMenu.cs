using System;
using Config;
using FrameWork;
using FrameWork.CommandSystem;
using FrameWork.UISystem.Components;
using FrameWork.UISystem.UIElements;
using Game.CommandSystem;
using Game.Components.Tutorial;
using UnityEngine;
using UnityEngine.Events;

namespace Game.Views.Tutorial
{
	// Token: 0x0200074A RID: 1866
	public class ViewTutorialChaptersMenu : UIBase
	{
		// Token: 0x06005A7B RID: 23163 RVA: 0x0029FA2C File Offset: 0x0029DC2C
		public override void OnInit(ArgumentBox argsBox)
		{
			this._settingData = SingletonObject.getInstance<GlobalSettings>();
			this.scrollRect.ScrollBar.onValueChanged.AddListener(new UnityAction<float>(this.OnScrollValueChanged));
		}

		// Token: 0x06005A7C RID: 23164 RVA: 0x0029FA5C File Offset: 0x0029DC5C
		private void OnEnable()
		{
			this.RefreshChaptersList();
			this.RefreshSkipTip();
		}

		// Token: 0x06005A7D RID: 23165 RVA: 0x0029FA6D File Offset: 0x0029DC6D
		private void OnDisable()
		{
			SingletonObject.getInstance<GlobalSettings>().SkipTutorialChapters = true;
			this._settingData.SaveSettings();
			ViewMainMenu viewMainMenu = (ViewMainMenu)UIElement.MainMenu.UiBase;
			if (viewMainMenu != null)
			{
				viewMainMenu.UpdateRoleListButtonInteractable();
			}
		}

		// Token: 0x06005A7E RID: 23166 RVA: 0x0029FAA4 File Offset: 0x0029DCA4
		protected override void OnClick(Transform btn)
		{
			string btnName = btn.name;
			bool flag = btnName == "Close";
			if (flag)
			{
				this.QuickHide();
			}
		}

		// Token: 0x06005A7F RID: 23167 RVA: 0x0029FAD4 File Offset: 0x0029DCD4
		public override void QuickHide()
		{
			bool flag = !SingletonObject.getInstance<GlobalSettings>().SkipTutorialChapters;
			if (flag)
			{
				UIElement.NewFunctionUnlock.SetOnInitArgs(EasyPool.Get<ArgumentBox>().Set("FunctionUnlockTemplateId", 25));
				UIManager.Instance.MaskUI(UIElement.NewFunctionUnlock);
			}
			base.QuickHide();
		}

		// Token: 0x06005A80 RID: 23168 RVA: 0x0029FB28 File Offset: 0x0029DD28
		private void RefreshChaptersList()
		{
			this.chaptersContainer.Rebuild<TutorialChapter>(TutorialChapters.Instance.Count, delegate(TutorialChapter tutorialChapter, int index)
			{
				bool visible = index <= this._settingData.CompletedChapters;
				tutorialChapter.enableObject.gameObject.SetActive(visible);
				tutorialChapter.disableObject.gameObject.SetActive(!visible);
				tutorialChapter.pointerTrigger.enabled = visible;
				tutorialChapter.chapterBtn.interactable = visible;
				TutorialChaptersItem tutorialChaptersItem = TutorialChapters.Instance.GetItem((short)index);
				tutorialChapter.chapterName.SetText(tutorialChaptersItem.Name, true);
				tutorialChapter.cardTexture.SetTexture(visible ? ("tutorial_card_chapter_" + index.ToString()) : "tutorial_card_chapter_close");
				tutorialChapter.finishTip.gameObject.SetActive(index < this._settingData.CompletedChapters);
				tutorialChapter.chapterBtn.onClick.ResetListener(delegate()
				{
					CommandManager.AddCommand(new CommandEnterTutorial
					{
						TutorialChapter = tutorialChaptersItem
					}, EPriority.EnterLoading);
				});
				bool flag = visible;
				if (flag)
				{
					tutorialChapter.toggleName.SetText(tutorialChaptersItem.ToggleName, true);
				}
			});
		}

		// Token: 0x06005A81 RID: 23169 RVA: 0x0029FB4D File Offset: 0x0029DD4D
		private void RefreshSkipTip()
		{
		}

		// Token: 0x06005A82 RID: 23170 RVA: 0x0029FB50 File Offset: 0x0029DD50
		private void OnScrollValueChanged(float arg0)
		{
			this.lightLeftObj.SetActive(arg0 != 1f);
			this.lightRightObj.SetActive(arg0 != 0f);
		}

		// Token: 0x04003E5C RID: 15964
		[SerializeField]
		private TemplatedContainerAssemblyNew chaptersContainer;

		// Token: 0x04003E5D RID: 15965
		[SerializeField]
		private CScrollRect scrollRect;

		// Token: 0x04003E5E RID: 15966
		[SerializeField]
		private GameObject lightLeftObj;

		// Token: 0x04003E5F RID: 15967
		[SerializeField]
		private GameObject lightRightObj;

		// Token: 0x04003E60 RID: 15968
		private GlobalSettings _settingData;
	}
}
