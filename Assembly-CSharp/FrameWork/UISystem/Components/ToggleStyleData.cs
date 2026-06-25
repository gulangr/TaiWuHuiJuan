using System;
using UnityEngine;

namespace FrameWork.UISystem.Components
{
	// Token: 0x02001026 RID: 4134
	[CreateAssetMenu(fileName = "CommonToggleStyle", menuName = "ScriptableObjects/ToggleStyleData")]
	public class ToggleStyleData : ScriptableObject
	{
		// Token: 0x17001552 RID: 5458
		// (get) Token: 0x0600BD2A RID: 48426 RVA: 0x0055F400 File Offset: 0x0055D600
		public TextStyleData LabelTextStyle
		{
			get
			{
				return this._labelTextStyle;
			}
		}

		// Token: 0x0400918E RID: 37262
		[Header("音效")]
		[Tooltip("应用该样式时自动写入到 UIInteractionBehaviour 的点击音效含义")]
		public UIInteractAudioMeaning ClickAudioMeaning = UIInteractAudioMeaning.DefaultSelect;

		// Token: 0x0400918F RID: 37263
		[Header("背景图片")]
		[Tooltip("通常状态背景图")]
		public Sprite NormalSprite;

		// Token: 0x04009190 RID: 37264
		[Tooltip("悬停状态背景图")]
		public Sprite HoverSprite;

		// Token: 0x04009191 RID: 37265
		[Tooltip("按下状态背景图")]
		public Sprite PressedSprite;

		// Token: 0x04009192 RID: 37266
		[Tooltip("选中状态背景图（注意不是isOn）")]
		public Sprite SelectedSprite;

		// Token: 0x04009193 RID: 37267
		[Tooltip("禁用状态背景图")]
		public Sprite DisabledSprite;

		// Token: 0x04009194 RID: 37268
		[Header("勾选图标（可选）")]
		[Tooltip("当 Toggle 勾选时显示的图标 Sprite")]
		public Sprite CheckmarkSprite;

		// Token: 0x04009195 RID: 37269
		[Header("文本样式数据")]
		[Tooltip("Toggle 标签的基础文本样式")]
		[SerializeField]
		private TextStyleData _labelTextStyle = new TextStyleData();
	}
}
