using System;
using Config;
using FrameWork;
using FrameWork.UISystem.Components;
using FrameWork.UISystem.UIElements;
using TMPro;
using UnityEngine;

namespace Game.Views
{
	// Token: 0x02000704 RID: 1796
	public class ViewDevelopmentTeam : UIBase
	{
		// Token: 0x060054E5 RID: 21733 RVA: 0x0027609D File Offset: 0x0027429D
		public override void OnInit(ArgumentBox argsBox)
		{
			this.container.Rebuild<RectTransform>(DevelopmentTeam.Instance.Count, delegate(RectTransform rect, int index)
			{
				DevelopmentTeamItem teamItem = DevelopmentTeam.Instance[index];
				TextMeshProUGUI title = rect.GetChild(0).GetComponent<TextMeshProUGUI>();
				title.SetText(teamItem.Title, true);
				TemplatedContainerAssemblyNew childTeam = rect.GetChild(1).GetComponent<TemplatedContainerAssemblyNew>();
				childTeam.Rebuild<RectTransform>(teamItem.TeamInfo.Length, delegate(RectTransform childRect, int childIndex)
				{
					TextMeshProUGUI nameInfo = childRect.GetComponent<TextMeshProUGUI>();
					nameInfo.SetText(teamItem.TeamInfo[childIndex], true);
				});
			});
		}

		// Token: 0x060054E6 RID: 21734 RVA: 0x002760D5 File Offset: 0x002742D5
		private void Awake()
		{
			this.close.onClick.ResetListener(new Action(this.QuickHide));
		}

		// Token: 0x040039C2 RID: 14786
		[SerializeField]
		public CButton close;

		// Token: 0x040039C3 RID: 14787
		[SerializeField]
		public TemplatedContainerAssemblyNew container;
	}
}
