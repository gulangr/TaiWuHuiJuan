using System;
using System.Collections.Generic;
using System.Linq;
using Config;
using FrameWork.UISystem.UIElements;
using UnityEngine;
using UnityEngine.UI;

namespace Game.Views.MapBlockCharList
{
	// Token: 0x02000939 RID: 2361
	public class MapBlockCharShortCuts : MonoBehaviour
	{
		// Token: 0x06006E16 RID: 28182 RVA: 0x0032D784 File Offset: 0x0032B984
		public void Init(IEnumerable<int> ie, int characterId, IMapBlockCharShortCutsParent parent)
		{
			this._parent = parent;
			this.charId = characterId;
			bool enabled = false;
			foreach (CImage btn in this.buttonImages)
			{
				btn.gameObject.SetActive(false);
			}
			foreach (ValueTuple<int, int> valueTuple in ie.Select((int item, int i) => new ValueTuple<int, int>(item, i)))
			{
				int item2 = valueTuple.Item1;
				int k = valueTuple.Item2;
				bool flag = this.buttonImages.CheckIndex(k);
				if (!flag)
				{
					break;
				}
				bool flag2 = CharacterMapBlockButton.Instance.Count > item2;
				if (flag2)
				{
					enabled = true;
					CharacterMapBlockButtonItem cfg = CharacterMapBlockButton.Instance[item2];
					TooltipInvoker tooltipInvoker = this.mouseTipDisplayers[k];
					string[] presetParam = this.mouseTipDisplayers[k].PresetParam;
					int num = 0;
					string name = cfg.Name;
					tooltipInvoker.enabled = true;
					presetParam[num] = name;
					SpriteState spriteState = this.buttons[k].spriteState;
					CImage cimage = this.buttonImages[k];
					SpriteState state = spriteState;
					CImage img = cimage;
					img.SetSprite(cfg.IconDisable, false, null);
					state.disabledSprite = img.sprite;
					img.SetSprite(cfg.IconPressed, false, null);
					state.pressedSprite = img.sprite;
					img.SetSprite(cfg.IconHighLight, false, null);
					state.highlightedSprite = img.sprite;
					img.SetSprite(cfg.IconNormal, false, null);
					state.selectedSprite = img.sprite;
					this.buttons[k].spriteState = state;
					this.buttonImages[k].gameObject.SetActive(true);
					sbyte id = cfg.TemplateId;
					this.buttons[k].onClick.ResetListener(delegate()
					{
						this._parent.OnClick((int)id, this.charId);
					});
					this.buttons[k].interactable = this._parent.CanClick((int)id, this.charId);
				}
				else
				{
					this.buttonImages[k].gameObject.SetActive(false);
				}
			}
			base.gameObject.SetActive(enabled);
		}

		// Token: 0x040051B1 RID: 20913
		private IMapBlockCharShortCutsParent _parent;

		// Token: 0x040051B2 RID: 20914
		[SerializeField]
		private int charId;

		// Token: 0x040051B3 RID: 20915
		[SerializeField]
		private CImage[] buttonImages;

		// Token: 0x040051B4 RID: 20916
		[SerializeField]
		private CButton[] buttons;

		// Token: 0x040051B5 RID: 20917
		[SerializeField]
		private TooltipInvoker[] mouseTipDisplayers;

		// Token: 0x040051B6 RID: 20918
		[SerializeField]
		internal PositionFollower follower;
	}
}
