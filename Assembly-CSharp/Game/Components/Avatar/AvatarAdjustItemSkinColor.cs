using System;
using System.Collections.Generic;
using UnityEngine;

namespace Game.Components.Avatar
{
	// Token: 0x02000F7F RID: 3967
	public class AvatarAdjustItemSkinColor : AvatarAdjustItemBase
	{
		// Token: 0x0600B631 RID: 46641 RVA: 0x0052F5AD File Offset: 0x0052D7AD
		protected override void Awake()
		{
			base.Awake();
			this.Closed = false;
		}

		// Token: 0x0600B632 RID: 46642 RVA: 0x0052F5BE File Offset: 0x0052D7BE
		protected override void Start()
		{
			this.OnQuickAdjustTriggered(0);
			this.Refers.CGet<InfinityScrollLegacy>("ColorScroll").GetComponent<CToggleGroupObsolete>().OnActiveToggleChange = delegate(CToggleObsolete n, CToggleObsolete o)
			{
				this.OnQuickAdjustTriggered(0);
			};
			this.OnOpen(false);
		}

		// Token: 0x0600B633 RID: 46643 RVA: 0x0052F5F7 File Offset: 0x0052D7F7
		public override void OnOpen(bool anim)
		{
			base.OnOpen(anim);
			this.Refers.CGet<InfinityScrollLegacy>("ColorScroll").ReRender();
		}

		// Token: 0x0600B634 RID: 46644 RVA: 0x0052F618 File Offset: 0x0052D818
		public override void BindArgUpdate()
		{
			base.RegisterOnArgUpdateListener(new Action(this.ArgsUpdateCallback));
			List<ValueTuple<byte, Color>> skinColorList = AvatarAdjustController.SkinColors;
			bool flag = this.CustomAssetsFilterHandler != null;
			if (flag)
			{
				skinColorList = this.CustomAssetsFilterHandler(skinColorList);
			}
			this._scrollIndexToRealIndexMap.Clear();
			for (int i = 0; i < skinColorList.Count; i++)
			{
				this._scrollIndexToRealIndexMap[i] = skinColorList[i].Item1;
			}
			base.UpdateColorScroll(this.Refers.CGet<InfinityScrollLegacy>("ColorScroll"), skinColorList);
		}

		// Token: 0x0600B635 RID: 46645 RVA: 0x0052F6AC File Offset: 0x0052D8AC
		private void ArgsUpdateCallback()
		{
			foreach (Avatar avatar in this.Controller.AvatarList)
			{
				bool flag = this.Controller.GetAge() < 16;
				if (flag)
				{
					avatar.Refresh();
				}
				else
				{
					avatar.UpdateCloth();
					avatar.UpdateHead();
				}
			}
			this.OnQuickAdjustTriggered(0);
		}

		// Token: 0x0600B636 RID: 46646 RVA: 0x0052F738 File Offset: 0x0052D938
		public override void SetColorIndex(int index)
		{
			base.Data.ColorSkinId = AvatarAdjustController.SkinColors[(int)this._scrollIndexToRealIndexMap[index]].Item1;
			Action onArgsUpdate = this.OnArgsUpdate;
			if (onArgsUpdate != null)
			{
				onArgsUpdate();
			}
		}

		// Token: 0x0600B637 RID: 46647 RVA: 0x0052F774 File Offset: 0x0052D974
		protected override int GetColorIndex()
		{
			for (int i = 0; i < AvatarAdjustController.SkinColors.Count; i++)
			{
				bool flag = AvatarAdjustController.SkinColors[i].Item1 == base.Data.ColorSkinId;
				if (flag)
				{
					return i;
				}
			}
			return 0;
		}

		// Token: 0x0600B638 RID: 46648 RVA: 0x0052F7C8 File Offset: 0x0052D9C8
		public override void OnQuickAdjustTriggered(int delta)
		{
			int currIndex = this.GetColorIndex();
			bool flag = delta < 0 && currIndex > 0;
			if (flag)
			{
				this.SetColorIndex(currIndex - 1);
			}
			else
			{
				bool flag2 = delta > 0 && currIndex < AvatarAdjustController.SkinColors.Count - 1;
				if (flag2)
				{
					this.SetColorIndex(currIndex + 1);
				}
			}
			bool flag3 = this.Refers.Names.Contains("ColorPrefab");
			if (flag3)
			{
				Refers color = this.Refers.CGet<Refers>("ColorPrefab");
				bool flag4 = color != null;
				if (flag4)
				{
					base.OnColorPrefabRender(currIndex, color);
				}
			}
			this.Refers.CGet<InfinityScrollLegacy>("ColorScroll").ReRender();
		}

		// Token: 0x04008D69 RID: 36201
		public Func<List<ValueTuple<byte, Color>>, List<ValueTuple<byte, Color>>> CustomAssetsFilterHandler;

		// Token: 0x04008D6A RID: 36202
		private Dictionary<int, byte> _scrollIndexToRealIndexMap = new Dictionary<int, byte>();
	}
}
