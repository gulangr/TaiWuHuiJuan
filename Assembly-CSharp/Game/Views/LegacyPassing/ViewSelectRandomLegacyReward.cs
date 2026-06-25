using System;
using System.Collections.Generic;
using FrameWork;
using GameData.Domains.Taiwu;
using GameData.Serializer;
using GameData.Utilities;
using UnityEngine;

namespace Game.Views.LegacyPassing
{
	// Token: 0x0200099A RID: 2458
	public class ViewSelectRandomLegacyReward : UIBase
	{
		// Token: 0x06007664 RID: 30308 RVA: 0x003732BE File Offset: 0x003714BE
		private void Awake()
		{
			this.container.Selected = delegate(LegacyItem item, bool b)
			{
				short legacyId = item.TemplateId;
				base.QuickHide();
				UIElement.GetItem.SetOnInitArgs(EasyPool.Get<ArgumentBox>().SetObject("LegacyList", new List<short>
				{
					legacyId
				}).Set("IsFree", true).SetObject("CloseAction", new Action(delegate
				{
					this._onSelectReward(legacyId);
				})));
				UIManager.Instance.MaskUI(UIElement.GetItem);
			};
		}

		// Token: 0x06007665 RID: 30309 RVA: 0x003732D8 File Offset: 0x003714D8
		public override void OnInit(ArgumentBox argsBox)
		{
			int groupId;
			argsBox.Get("GroupId", out groupId);
			argsBox.Get<Action<short>>("OnSelectLegacy", out this._onSelectReward);
			if (this._legacies == null)
			{
				this._legacies = new List<short>();
			}
			TaiwuDomainMethod.AsyncCall.GetRandomLegaciesInGroup(this, (sbyte)groupId, 3, delegate(int offset, RawDataPool pool)
			{
				Serializer.Deserialize(pool, offset, ref this._legacies);
				this.container.RefreshItemsForFree(this._legacies, true);
			});
		}

		// Token: 0x06007666 RID: 30310 RVA: 0x0037332F File Offset: 0x0037152F
		public override void QuickHide()
		{
			CommonUtils.ShowConfirmDialog(LanguageKey.LK_Legacy_Reward_GiveUp_Title.Tr(), LanguageKey.LK_Legacy_Reward_GiveUp_Content.Tr(), new Action(base.QuickHide), null, EDialogType.None);
		}

		// Token: 0x0400594C RID: 22860
		[SerializeField]
		private LegacyContainer container;

		// Token: 0x0400594D RID: 22861
		private List<short> _legacies;

		// Token: 0x0400594E RID: 22862
		private Action<short> _onSelectReward;

		// Token: 0x0400594F RID: 22863
		private const int LegacyCount = 3;
	}
}
