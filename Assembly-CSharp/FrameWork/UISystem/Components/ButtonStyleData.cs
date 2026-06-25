using System;
using UnityEngine;

namespace FrameWork.UISystem.Components
{
	// Token: 0x02001011 RID: 4113
	[CreateAssetMenu(fileName = "CommonButtonStyle", menuName = "ScriptableObjects/ButtonStyleData")]
	public class ButtonStyleData : ScriptableObject
	{
		// Token: 0x17001538 RID: 5432
		// (get) Token: 0x0600BC0B RID: 48139 RVA: 0x0055858A File Offset: 0x0055678A
		public TextStyleData BaseTextStyle
		{
			get
			{
				return this._baseTextStyle;
			}
		}

		// Token: 0x040090DF RID: 37087
		[Header("音效")]
		[Tooltip("应用该样式时自动写入到 UIInteractionBehaviour 的点击音效含义")]
		public UIInteractAudioMeaning ClickAudioMeaning = UIInteractAudioMeaning.DefaultClickLeft;

		// Token: 0x040090E0 RID: 37088
		[Header("按钮图片")]
		[Tooltip("通常状态图片")]
		public Sprite NormalSprite;

		// Token: 0x040090E1 RID: 37089
		[Tooltip("悬停状态图片")]
		public Sprite HoverSprite;

		// Token: 0x040090E2 RID: 37090
		[Tooltip("按下状态图片")]
		public Sprite PressedSprite;

		// Token: 0x040090E3 RID: 37091
		[Tooltip("禁用状态图片")]
		public Sprite DisabledSprite;

		// Token: 0x040090E4 RID: 37092
		[Header("图标按钮")]
		[Tooltip("当按钮为图标模式时使用的图标Sprite")]
		public Sprite IconSprite;

		// Token: 0x040090E5 RID: 37093
		[Header("文本样式数据")]
		[Tooltip("基础文本样式")]
		[SerializeField]
		private TextStyleData _baseTextStyle = new TextStyleData();
	}
}
