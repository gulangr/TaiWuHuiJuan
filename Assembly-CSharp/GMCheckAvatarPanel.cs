using System;
using System.Collections.Generic;
using FrameWork;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

// Token: 0x0200020A RID: 522
public class GMCheckAvatarPanel : MonoBehaviour
{
	// Token: 0x06002139 RID: 8505 RVA: 0x000F20D8 File Offset: 0x000F02D8
	private void Awake()
	{
		this.outputButton.onClick.AddListener(new UnityAction(this.OnClickOutputButton));
		this.closeButton.onClick.AddListener(new UnityAction(this.OnClose));
		this.PrepareLines();
		GEvent.Add(UiEvents.OnLanguageChange, new GEvent.Callback(this.OnLanguageChange));
		GEvent.Add(UiEvents.CharacterMenuHide, new GEvent.Callback(this.OnCharacterMenuHide));
		GMCheckAvatarState.StateChanged += this.RefreshView;
		this.RefreshView();
		base.gameObject.SetActive(false);
	}

	// Token: 0x0600213A RID: 8506 RVA: 0x000F2184 File Offset: 0x000F0384
	private void OnDestroy()
	{
		GEvent.Remove(UiEvents.OnLanguageChange, new GEvent.Callback(this.OnLanguageChange));
		GEvent.Remove(UiEvents.CharacterMenuHide, new GEvent.Callback(this.OnCharacterMenuHide));
		GMCheckAvatarState.StateChanged -= this.RefreshView;
	}

	// Token: 0x0600213B RID: 8507 RVA: 0x000F21D9 File Offset: 0x000F03D9
	public void OnWorldDataReady()
	{
		this.RefreshView();
	}

	// Token: 0x0600213C RID: 8508 RVA: 0x000F21E3 File Offset: 0x000F03E3
	public void OnLeaveWorld()
	{
		GMCheckAvatarState.Reset();
	}

	// Token: 0x0600213D RID: 8509 RVA: 0x000F21EC File Offset: 0x000F03EC
	public void Open()
	{
		GMCheckAvatarState.SetEnabled(true);
		base.gameObject.SetActive(true);
		this.RefreshView();
	}

	// Token: 0x0600213E RID: 8510 RVA: 0x000F220A File Offset: 0x000F040A
	public void OnClose()
	{
		GMCheckAvatarState.SetEnabled(false);
		base.gameObject.SetActive(false);
	}

	// Token: 0x0600213F RID: 8511 RVA: 0x000F2221 File Offset: 0x000F0421
	private void OnClickStepButton(GMCheckAvatarComponentType componentType, int direction)
	{
		GMCheckAvatarState.TryStepComponent(componentType, direction);
	}

	// Token: 0x06002140 RID: 8512 RVA: 0x000F222C File Offset: 0x000F042C
	private void OnClickClearButton(GMCheckAvatarComponentType componentType)
	{
		GMCheckAvatarState.TryClearComponent(componentType);
	}

	// Token: 0x06002141 RID: 8513 RVA: 0x000F2236 File Offset: 0x000F0436
	private void OnClickOutputButton()
	{
		UI_GMWindow.Instance.Log(GMCheckAvatarState.BuildCurrentAvatarLog());
	}

	// Token: 0x06002142 RID: 8514 RVA: 0x000F2249 File Offset: 0x000F0449
	private void OnLanguageChange(ArgumentBox argumentBox)
	{
		this.RefreshTexts();
		this.RefreshView();
	}

	// Token: 0x06002143 RID: 8515 RVA: 0x000F225A File Offset: 0x000F045A
	private void OnCharacterMenuHide(ArgumentBox argumentBox)
	{
		GMCheckAvatarState.ClearCurrentCharacter();
	}

	// Token: 0x06002144 RID: 8516 RVA: 0x000F2264 File Offset: 0x000F0464
	private void PrepareLines()
	{
		IReadOnlyList<GMCheckAvatarLineDefinition> definitions = GMCheckAvatarState.GetLineDefinitions();
		CommonUtils.PrepareEnoughChildren(this.lineContainer, this.lineTemplate.gameObject, definitions.Count, null);
		this._lines.Clear();
		for (int i = 0; i < definitions.Count; i++)
		{
			GMCheckAvatarLine line = this.lineContainer.GetChild(i).GetComponent<GMCheckAvatarLine>();
			line.Initialize(definitions[i].ComponentType, new Action<GMCheckAvatarComponentType, int>(this.OnClickStepButton), new Action<GMCheckAvatarComponentType>(this.OnClickClearButton));
			this._lines.Add(line);
		}
	}

	// Token: 0x06002145 RID: 8517 RVA: 0x000F2314 File Offset: 0x000F0514
	private void RefreshTexts()
	{
		for (int i = 0; i < this._lines.Count; i++)
		{
			this._lines[i].RefreshTexts();
		}
	}

	// Token: 0x06002146 RID: 8518 RVA: 0x000F2350 File Offset: 0x000F0550
	private void RefreshView()
	{
		this.RefreshTexts();
		this.currentCharacterText.text = (GMCheckAvatarState.HasCurrentCharacter ? string.Format(LocalStringManager.Get("GM_CheckAvatar_Target"), GMCheckAvatarState.CurrentCharacterId) : LocalStringManager.Get("GM_CheckAvatar_Target_None"));
		bool canOperate = GMCheckAvatarState.Enabled && GMCheckAvatarState.HasCurrentCharacter;
		this.outputButton.interactable = canOperate;
		for (int i = 0; i < this._lines.Count; i++)
		{
			this._lines[i].RefreshInteractable(canOperate);
		}
	}

	// Token: 0x040019AF RID: 6575
	[SerializeField]
	private TextMeshProUGUI currentCharacterText;

	// Token: 0x040019B0 RID: 6576
	[SerializeField]
	private Button outputButton;

	// Token: 0x040019B1 RID: 6577
	[SerializeField]
	private Button closeButton;

	// Token: 0x040019B2 RID: 6578
	[SerializeField]
	private RectTransform lineContainer;

	// Token: 0x040019B3 RID: 6579
	[SerializeField]
	private GMCheckAvatarLine lineTemplate;

	// Token: 0x040019B4 RID: 6580
	private readonly List<GMCheckAvatarLine> _lines = new List<GMCheckAvatarLine>();
}
