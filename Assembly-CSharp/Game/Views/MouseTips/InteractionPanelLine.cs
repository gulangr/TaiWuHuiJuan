using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using FrameWork.UISystem.Components;
using TMPro;
using UnityEngine;

namespace Game.Views.MouseTips
{
	// Token: 0x02000842 RID: 2114
	public class InteractionPanelLine : MonoBehaviour
	{
		// Token: 0x060066E5 RID: 26341 RVA: 0x002EF1D8 File Offset: 0x002ED3D8
		public void Set([TupleElementNames(new string[]
		{
			"name",
			"available"
		})] IReadOnlyList<ValueTuple<string, bool>> render)
		{
			bool flag = render != null && render.Count > 0;
			if (flag)
			{
				base.gameObject.SetActive(true);
				this.content.Rebuild<InteractionPanelLineItem>(render.Count, delegate(InteractionPanelLineItem item, int i)
				{
					item.Set(render[i].Item1, render[i].Item2);
				});
			}
			else
			{
				base.gameObject.SetActive(false);
			}
		}

		// Token: 0x060066E6 RID: 26342 RVA: 0x002EF250 File Offset: 0x002ED450
		public void Set(string typeName, Sprite sprite, [TupleElementNames(new string[]
		{
			"name",
			"available"
		})] IReadOnlyList<ValueTuple<string, bool>> render)
		{
			this.typeNameText.text = typeName;
			this.icon.sprite = sprite;
			this.Set(render);
		}

		// Token: 0x060066E7 RID: 26343 RVA: 0x002EF275 File Offset: 0x002ED475
		public void Set(string typeName, string sprite, [TupleElementNames(new string[]
		{
			"name",
			"available"
		})] IReadOnlyList<ValueTuple<string, bool>> render)
		{
			this.typeNameText.text = typeName;
			this.icon.SetSprite(sprite, false, null);
			this.Set(render);
		}

		// Token: 0x04004864 RID: 18532
		[SerializeField]
		private TMP_Text typeNameText;

		// Token: 0x04004865 RID: 18533
		[SerializeField]
		private CImage icon;

		// Token: 0x04004866 RID: 18534
		[SerializeField]
		private TemplatedContainerAssemblyNew content;
	}
}
