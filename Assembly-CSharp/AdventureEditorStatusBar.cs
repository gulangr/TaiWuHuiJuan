using System;
using System.Collections;
using System.Runtime.CompilerServices;
using DG.Tweening;
using FrameWork;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

// Token: 0x02000185 RID: 389
public class AdventureEditorStatusBar : UIBehaviour
{
	// Token: 0x060015FD RID: 5629 RVA: 0x00088763 File Offset: 0x00086963
	protected override void Start()
	{
		base.Start();
		this.Perform(LanguageKey.LK_AdventureEditor_Entry.Tr());
	}

	// Token: 0x060015FE RID: 5630 RVA: 0x00088780 File Offset: 0x00086980
	protected override void OnEnable()
	{
		base.OnEnable();
		GEvent.Add(UiEvents.AdventureRemakeEditorStatusLoaded, new GEvent.Callback(this.PerformLoaded));
		GEvent.Add(UiEvents.AdventureRemakeEditorStatusSaved, new GEvent.Callback(this.PerformSaved));
		GEvent.Add(UiEvents.AdventureRemakeEditorStatusReverted, new GEvent.Callback(this.PerformReverted));
		GEvent.Add(UiEvents.AdventureRemakeEditorStatusEvolved, new GEvent.Callback(this.PerformEvolved));
	}

	// Token: 0x060015FF RID: 5631 RVA: 0x00088808 File Offset: 0x00086A08
	protected override void OnDisable()
	{
		base.OnDisable();
		GEvent.Remove(UiEvents.AdventureRemakeEditorStatusLoaded, new GEvent.Callback(this.PerformLoaded));
		GEvent.Remove(UiEvents.AdventureRemakeEditorStatusSaved, new GEvent.Callback(this.PerformSaved));
		GEvent.Remove(UiEvents.AdventureRemakeEditorStatusReverted, new GEvent.Callback(this.PerformReverted));
		GEvent.Remove(UiEvents.AdventureRemakeEditorStatusEvolved, new GEvent.Callback(this.PerformEvolved));
	}

	// Token: 0x06001600 RID: 5632 RVA: 0x00088890 File Offset: 0x00086A90
	private void Perform(string msg)
	{
		bool flag = this._performing != null;
		if (flag)
		{
			base.StopCoroutine(this._performing);
		}
		base.StartCoroutine(this._performing = this.<Perform>g__Process|7_0(msg));
	}

	// Token: 0x06001601 RID: 5633 RVA: 0x000888D4 File Offset: 0x00086AD4
	public void PerformSaved(ArgumentBox args)
	{
		string path;
		args.Get("Path", out path);
		this.Perform(string.Concat(new string[]
		{
			"[",
			path,
			"] ",
			LanguageKey.UI_EventEditor_Dialog_Save.Tr(),
			" : ",
			LanguageKey.LK_Success.Tr()
		}));
	}

	// Token: 0x06001602 RID: 5634 RVA: 0x00088938 File Offset: 0x00086B38
	public void PerformLoaded(ArgumentBox args)
	{
	}

	// Token: 0x06001603 RID: 5635 RVA: 0x0008893B File Offset: 0x00086B3B
	public void PerformReverted(ArgumentBox args)
	{
		this.Perform(LanguageKey.LK_Adventure_CtxMenu_Undo.Tr() + " : " + LanguageKey.LK_Success.Tr());
	}

	// Token: 0x06001604 RID: 5636 RVA: 0x00088963 File Offset: 0x00086B63
	public void PerformEvolved(ArgumentBox args)
	{
		this.Perform(LanguageKey.LK_Adventure_CtxMenu_Redo.Tr() + " : " + LanguageKey.LK_Success.Tr());
	}

	// Token: 0x06001606 RID: 5638 RVA: 0x000889AC File Offset: 0x00086BAC
	[CompilerGenerated]
	private IEnumerator <Perform>g__Process|7_0(string src)
	{
		CanvasGroup canvasGroup = base.gameObject.GetOrAddComponent<CanvasGroup>();
		this.text.text = string.Empty;
		canvasGroup.DOFade(1f, this.performTextSecondPerCharacter * (float)src.Length * 0.1f);
		foreach (char ch in src)
		{
			TextMeshProUGUI textMeshProUGUI = this.text;
			textMeshProUGUI.text += ch.ToString();
			yield return new WaitForSeconds(this.performTextSecondPerCharacter);
		}
		string text = null;
		float halfStay = this.performTextSecondStay / 2f;
		yield return new WaitForSeconds(halfStay);
		canvasGroup.DOFade(0f, halfStay);
		yield break;
	}

	// Token: 0x04001204 RID: 4612
	[SerializeField]
	private TextMeshProUGUI text;

	// Token: 0x04001205 RID: 4613
	[SerializeField]
	private float performTextSecondPerCharacter = 0.05f;

	// Token: 0x04001206 RID: 4614
	[SerializeField]
	private float performTextSecondStay = 1f;

	// Token: 0x04001207 RID: 4615
	private IEnumerator _performing;
}
