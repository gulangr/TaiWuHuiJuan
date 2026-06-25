using System;
using Config;
using FrameWork;
using FrameWork.UISystem.Components;
using UnityEngine;

namespace Game.Views.MapElement
{
	// Token: 0x02000932 RID: 2354
	public class ViewMapElementDisplayRule : UIBase
	{
		// Token: 0x06006DD4 RID: 28116 RVA: 0x0032C3C8 File Offset: 0x0032A5C8
		public override void OnInit(ArgumentBox argsBox)
		{
			this.groupScroll.SetDataCount(MapElementDisplayRuleGroup.Instance.Count);
		}

		// Token: 0x06006DD5 RID: 28117 RVA: 0x0032C3E1 File Offset: 0x0032A5E1
		private void Awake()
		{
			this.groupScroll.OnItemRender += this.OnItemRender;
		}

		// Token: 0x06006DD6 RID: 28118 RVA: 0x0032C3FC File Offset: 0x0032A5FC
		private void OnDestroy()
		{
			this.groupScroll.OnItemRender -= this.OnItemRender;
		}

		// Token: 0x06006DD7 RID: 28119 RVA: 0x0032C417 File Offset: 0x0032A617
		private void OnEnable()
		{
			GEvent.Add(UiEvents.OnForceRefreshAllMapBlock, new GEvent.Callback(this.OnForceRefreshAllMapBlock));
		}

		// Token: 0x06006DD8 RID: 28120 RVA: 0x0032C433 File Offset: 0x0032A633
		private void OnDisable()
		{
			GEvent.Remove(UiEvents.OnForceRefreshAllMapBlock, new GEvent.Callback(this.OnForceRefreshAllMapBlock));
			SingletonObject.getInstance<GlobalSettings>().SaveSettings();
		}

		// Token: 0x06006DD9 RID: 28121 RVA: 0x0032C45A File Offset: 0x0032A65A
		private void OnForceRefreshAllMapBlock(ArgumentBox _)
		{
			this.groupScroll.ReRender();
		}

		// Token: 0x06006DDA RID: 28122 RVA: 0x0032C46C File Offset: 0x0032A66C
		private void OnItemRender(int index, GameObject obj)
		{
			MapElementDisplayRuleGroup group = obj.GetComponent<MapElementDisplayRuleGroup>();
			short id = (short)index;
			group.Refresh(id);
		}

		// Token: 0x06006DDB RID: 28123 RVA: 0x0032C48C File Offset: 0x0032A68C
		protected override void OnClick(Transform btn)
		{
			string name = btn.name;
			string a = name;
			if (a == "ButtonClose")
			{
				this.QuickHide();
			}
		}

		// Token: 0x0400517E RID: 20862
		[SerializeField]
		private InfinityScroll groupScroll;
	}
}
