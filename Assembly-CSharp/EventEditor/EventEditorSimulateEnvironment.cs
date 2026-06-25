using System;
using FrameWork.UISystem.Components;
using FrameWork.UISystem.UIElements;
using UnityEngine;

namespace EventEditor
{
	// Token: 0x02000634 RID: 1588
	public class EventEditorSimulateEnvironment : EventEditorSubPageBase
	{
		// Token: 0x06004B1D RID: 19229 RVA: 0x002351D2 File Offset: 0x002333D2
		public static void Init(EventEditorSimulateEnvironment instance)
		{
			EventEditorSimulateEnvironment.Instance = instance;
			EventEditorSimulateEnvironment.Instance.InternalInit();
		}

		// Token: 0x06004B1E RID: 19230 RVA: 0x002351E8 File Offset: 0x002333E8
		protected override void InternalInit()
		{
			this._roleSimulate = this.roleTabPage;
			this._roleSimulate.Init();
			this._itemSimulate = this.itemTabPage;
			this._itemSimulate.Init();
			this.simulateTabBtns.OnActiveIndexChange += this.OnToggleGroupValueChange;
			this.simulateTabBtns.Init(-1);
		}

		// Token: 0x06004B1F RID: 19231 RVA: 0x0023524B File Offset: 0x0023344B
		public override void Show()
		{
		}

		// Token: 0x06004B20 RID: 19232 RVA: 0x0023524E File Offset: 0x0023344E
		public override void Hide()
		{
		}

		// Token: 0x06004B21 RID: 19233 RVA: 0x00235254 File Offset: 0x00233454
		public EventEditorRole GetRole(string key)
		{
			return this._roleSimulate.GetRole(key);
		}

		// Token: 0x06004B22 RID: 19234 RVA: 0x00235274 File Offset: 0x00233474
		public EventEditorItem GetItem(string key)
		{
			return this._itemSimulate.GetItem(key);
		}

		// Token: 0x06004B23 RID: 19235 RVA: 0x00235294 File Offset: 0x00233494
		public EventEditorItem GetItem(string typeString, short itemTemplateId)
		{
			return this._itemSimulate.GetItem(typeString, itemTemplateId);
		}

		// Token: 0x06004B24 RID: 19236 RVA: 0x002352B4 File Offset: 0x002334B4
		private void OnToggleGroupValueChange(int newIndex, int preIndex)
		{
			if (newIndex != 0)
			{
				if (newIndex == 1)
				{
					this._itemSimulate.Refresh();
				}
			}
			else
			{
				this._roleSimulate.Refresh();
			}
		}

		// Token: 0x06004B25 RID: 19237 RVA: 0x002352F0 File Offset: 0x002334F0
		public void OnRoleSimulateUpdate()
		{
			bool rolePageDirty = this.RolePageDirty;
			if (rolePageDirty)
			{
				EventEditorEventPreview.Instance.Refresh(EventEditorEventDetail.Instance.CurEvent);
				this.RolePageDirty = false;
			}
			else
			{
				this._roleSimulate.SendDirtyInfo();
			}
		}

		// Token: 0x06004B26 RID: 19238 RVA: 0x00235334 File Offset: 0x00233534
		private void LateUpdate()
		{
			bool keyUp = Input.GetKeyUp(KeyCode.Mouse0);
			if (keyUp)
			{
				this.OnRoleSimulateUpdate();
			}
		}

		// Token: 0x04003428 RID: 13352
		public static EventEditorSimulateEnvironment Instance;

		// Token: 0x04003429 RID: 13353
		[SerializeField]
		private InfinityScroll roleScroll;

		// Token: 0x0400342A RID: 13354
		[SerializeField]
		private CToggleGroup simulateTabBtns;

		// Token: 0x0400342B RID: 13355
		[SerializeField]
		private RoleSimulate roleTabPage;

		// Token: 0x0400342C RID: 13356
		[SerializeField]
		private ItemSimulate itemTabPage;

		// Token: 0x0400342D RID: 13357
		private RoleSimulate _roleSimulate;

		// Token: 0x0400342E RID: 13358
		public bool RolePageDirty;

		// Token: 0x0400342F RID: 13359
		private ItemSimulate _itemSimulate;

		// Token: 0x04003430 RID: 13360
		public bool ItemPageDirty;
	}
}
