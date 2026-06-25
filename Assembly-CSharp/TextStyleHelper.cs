using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

// Token: 0x02000098 RID: 152
[RequireComponent(typeof(TextMeshProUGUI))]
public class TextStyleHelper : MonoBehaviour
{
	// Token: 0x0600054F RID: 1359 RVA: 0x000240E8 File Offset: 0x000222E8
	private void Awake()
	{
		bool flag = Application.isPlaying && this._originStyle.fontAsset == null;
		if (flag)
		{
			TextMeshProUGUI origin = base.GetComponent<TextMeshProUGUI>();
			this._originStyle.ExtractFrom(origin);
		}
	}

	// Token: 0x06000550 RID: 1360 RVA: 0x0002412B File Offset: 0x0002232B
	public void OnEnable()
	{
		this.RefreshTextStyle();
	}

	// Token: 0x06000551 RID: 1361 RVA: 0x00024135 File Offset: 0x00022335
	public void RefreshTextStyle()
	{
		SingletonObject.getInstance<YieldHelper>().DelayFrameDo(1U, new Action(this.Refresh));
	}

	// Token: 0x06000552 RID: 1362 RVA: 0x00024150 File Offset: 0x00022350
	private void Refresh()
	{
		bool flag = this.Hover != null && this.Hover.activeSelf && this._hoverStyle.fontAsset != null;
		if (flag)
		{
			this.ApplyStyle(this._hoverStyle);
		}
		else
		{
			bool flag2 = this.Selected != null && this.Selected.activeSelf && this._selectedStyle.fontAsset != null;
			if (flag2)
			{
				this.ApplyStyle(this._selectedStyle);
			}
			else
			{
				bool flag3 = this.CanInteract != null && !this.CanInteract.interactable && this._disableStyle.fontAsset != null;
				if (flag3)
				{
					this.ApplyStyle(this._disableStyle);
				}
				else
				{
					this.ApplyStyle(this._originStyle);
				}
			}
		}
	}

	// Token: 0x06000553 RID: 1363 RVA: 0x00024238 File Offset: 0x00022438
	private void ApplyStyle(TextStyleData styleData)
	{
		TextMeshProUGUI textComponent = base.GetComponent<TextMeshProUGUI>();
		styleData.ApplyTo(textComponent);
		textComponent.SetAllDirty();
	}

	// Token: 0x04000455 RID: 1109
	public GameObject Hover;

	// Token: 0x04000456 RID: 1110
	public GameObject Selected;

	// Token: 0x04000457 RID: 1111
	public Selectable CanInteract;

	// Token: 0x04000458 RID: 1112
	[Header("运行时样式数据")]
	[SerializeField]
	private TextStyleData _originStyle = new TextStyleData();

	// Token: 0x04000459 RID: 1113
	[SerializeField]
	private TextStyleData _disableStyle = new TextStyleData();

	// Token: 0x0400045A RID: 1114
	[SerializeField]
	private TextStyleData _hoverStyle = new TextStyleData();

	// Token: 0x0400045B RID: 1115
	[SerializeField]
	private TextStyleData _selectedStyle = new TextStyleData();
}
