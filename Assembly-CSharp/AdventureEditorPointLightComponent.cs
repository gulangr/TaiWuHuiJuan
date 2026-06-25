using System;
using Game.Views.Legacy.AdventureEditor.Migrate;
using GameData.Adventure;
using GameData.Adventure.Editor;
using TMPro;
using UnityEngine;
using UnityEngine.Events;

// Token: 0x02000180 RID: 384
public class AdventureEditorPointLightComponent : MonoBehaviour
{
	// Token: 0x060015CF RID: 5583 RVA: 0x00086E04 File Offset: 0x00085004
	protected virtual void OnInit()
	{
		this.colorInput.onEndEdit.AddListener(new UnityAction<string>(this.OnColorChanged));
		this.intensityInput.OnValueChanged += this.OnIntensityChanged;
		this.virtualZInput.OnValueChanged += this.OnVirtualZChanged;
	}

	// Token: 0x060015D0 RID: 5584 RVA: 0x00086E5F File Offset: 0x0008505F
	private void Awake()
	{
		this.OnInit();
	}

	// Token: 0x060015D1 RID: 5585 RVA: 0x00086E6C File Offset: 0x0008506C
	protected virtual AdventureLightData GetTargetLightData()
	{
		return AdventureEditorKit.BlackBoard.Editing.LightingTaiwu;
	}

	// Token: 0x060015D2 RID: 5586 RVA: 0x00086E90 File Offset: 0x00085090
	protected virtual void ApplyColor(string hex)
	{
		AdventureEditorKit.BlackBoard.MakeEdit(delegate(AdventureSnapshot s)
		{
			s.LightingTaiwu.ColorInHex = hex;
		}, EAdventureEditType.Basic);
	}

	// Token: 0x060015D3 RID: 5587 RVA: 0x00086EC4 File Offset: 0x000850C4
	protected virtual void ApplyIntensity(float value)
	{
		AdventureEditorKit.BlackBoard.MakeEdit(delegate(AdventureSnapshot s)
		{
			s.LightingTaiwu.Strength = value;
		}, EAdventureEditType.Basic);
	}

	// Token: 0x060015D4 RID: 5588 RVA: 0x00086EF8 File Offset: 0x000850F8
	protected virtual void ApplyVirtualZ(float value)
	{
		AdventureEditorKit.BlackBoard.MakeEdit(delegate(AdventureSnapshot s)
		{
			s.LightingTaiwu.Height = value;
		}, EAdventureEditType.Basic);
	}

	// Token: 0x060015D5 RID: 5589 RVA: 0x00086F2C File Offset: 0x0008512C
	public virtual void ReloadFromBlackBoard()
	{
		AdventureLightData data = this.GetTargetLightData();
		this._isRefreshing = true;
		this.colorInput.SetTextWithoutNotify(((data != null) ? data.ColorInHex : null) ?? "");
		this.intensityInput.Set((data != null) ? data.Strength : 0f);
		this.virtualZInput.Set((data != null) ? data.Height : 0f);
		this._isRefreshing = false;
	}

	// Token: 0x060015D6 RID: 5590 RVA: 0x00086FA8 File Offset: 0x000851A8
	private void OnColorChanged(string value)
	{
		bool isRefreshing = this._isRefreshing;
		if (!isRefreshing)
		{
			string normalized;
			bool flag = !AdventureEditorLightingPanel.TryNormalizeColorHex(value, out normalized);
			if (flag)
			{
				AdventureLightData targetLightData = this.GetTargetLightData();
				string revert = ((targetLightData != null) ? targetLightData.ColorInHex : null) ?? "";
				this.colorInput.SetTextWithoutNotify(revert);
			}
			else
			{
				this.ApplyColor(normalized);
				this.colorInput.SetTextWithoutNotify(normalized);
			}
		}
	}

	// Token: 0x060015D7 RID: 5591 RVA: 0x00087014 File Offset: 0x00085214
	private void OnIntensityChanged()
	{
		bool isRefreshing = this._isRefreshing;
		if (!isRefreshing)
		{
			this.ApplyIntensity(this.intensityInput.Value);
		}
	}

	// Token: 0x060015D8 RID: 5592 RVA: 0x00087040 File Offset: 0x00085240
	private void OnVirtualZChanged()
	{
		bool isRefreshing = this._isRefreshing;
		if (!isRefreshing)
		{
			this.ApplyVirtualZ(this.virtualZInput.Value);
		}
	}

	// Token: 0x040011E2 RID: 4578
	[SerializeField]
	private TMP_InputField colorInput;

	// Token: 0x040011E3 RID: 4579
	[SerializeField]
	private InputCSlider intensityInput;

	// Token: 0x040011E4 RID: 4580
	[SerializeField]
	private InputCSlider virtualZInput;

	// Token: 0x040011E5 RID: 4581
	protected bool _isRefreshing;
}
