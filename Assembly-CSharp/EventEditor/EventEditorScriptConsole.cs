using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using FrameWork.UISystem.Components;
using Game.Views.Legacy.EventEditor.UIScripts.Migrate;
using UnityEngine;
using UnityEngine.Events;

namespace EventEditor
{
	// Token: 0x02000649 RID: 1609
	public class EventEditorScriptConsole : EventEditorSubPageBase
	{
		// Token: 0x06004CAD RID: 19629 RVA: 0x00243D00 File Offset: 0x00241F00
		public static void Init(EventEditorScriptConsole obj)
		{
			EventEditorScriptConsole.Instance = obj;
			EventEditorScriptConsole.Instance.InternalInit();
		}

		// Token: 0x1700096B RID: 2411
		// (get) Token: 0x06004CAE RID: 19630 RVA: 0x00243D14 File Offset: 0x00241F14
		public bool IsShowing
		{
			get
			{
				return this._isShowing;
			}
		}

		// Token: 0x06004CAF RID: 19631 RVA: 0x00243D1C File Offset: 0x00241F1C
		protected override void InternalInit()
		{
			this._allNotes = new List<EventEditorScriptConsole.NoteInfo>();
			this.noteWindow.localScale = new Vector3(1f, 0f, 1f);
			this.notesScroll.OnItemRender += this.OnNoteItemRender;
			this.notesScroll.initSuccess = false;
			this.notesScroll.GetComponent<PointerTrigger>().ExitEvent.RemoveAllListeners();
			this.notesScroll.GetComponent<PointerTrigger>().ExitEvent.AddListener(new UnityAction(this.Hide));
		}

		// Token: 0x06004CB0 RID: 19632 RVA: 0x00243DB4 File Offset: 0x00241FB4
		public override void Show()
		{
			bool isShowing = this._isShowing;
			if (!isShowing)
			{
				this._isShowing = true;
				DOVirtual.Float(0f, 1f, 0.25f, delegate(float stepValue)
				{
					Vector3 scale = this.noteWindow.localScale;
					scale.y = stepValue;
					this.noteWindow.localScale = scale;
				}).OnComplete(delegate
				{
					this.notesScroll.UpdateData(this._allNotes.Count);
					base.StartCoroutine(this.ScrollToEnd());
				}).SetAutoKill(true);
			}
		}

		// Token: 0x06004CB1 RID: 19633 RVA: 0x00243E10 File Offset: 0x00242010
		public override void Hide()
		{
			base.StopAllCoroutines();
			bool flag = !this._isShowing;
			if (!flag)
			{
				DOVirtual.Float(1f, 0f, 0.25f, delegate(float stepValue)
				{
					Vector3 scale = this.noteWindow.localScale;
					scale.y = stepValue;
					this.noteWindow.localScale = scale;
				}).SetAutoKill(true);
				this._isShowing = false;
			}
		}

		// Token: 0x06004CB2 RID: 19634 RVA: 0x00243E62 File Offset: 0x00242062
		private IEnumerator ScrollToEnd()
		{
			yield return new WaitForEndOfFrame();
			yield return new WaitForEndOfFrame();
			this.notesScroll.ScrollToEnd();
			yield break;
		}

		// Token: 0x06004CB3 RID: 19635 RVA: 0x00243E74 File Offset: 0x00242074
		private void OnNoteItemRender(int dataIndex, GameObject refers)
		{
			EventEditorScriptNoteCellPrefab template = refers.GetComponent<EventEditorScriptNoteCellPrefab>();
			template.image.enabled = (dataIndex % 2 == 0);
			template.content.text = this._allNotes[dataIndex].ToString();
		}

		// Token: 0x06004CB4 RID: 19636 RVA: 0x00243EC4 File Offset: 0x002420C4
		public void AddNote(string note)
		{
			this._allNotes.Add(new EventEditorScriptConsole.NoteInfo
			{
				Note = note,
				DateTime = DateTime.Now
			});
			Action<string> onNewNote = this.OnNewNote;
			if (onNewNote != null)
			{
				onNewNote(note);
			}
		}

		// Token: 0x04003524 RID: 13604
		[SerializeField]
		private RectTransform noteWindow;

		// Token: 0x04003525 RID: 13605
		[SerializeField]
		private InfinityScroll notesScroll;

		// Token: 0x04003526 RID: 13606
		public static EventEditorScriptConsole Instance;

		// Token: 0x04003527 RID: 13607
		[NonSerialized]
		public Action<string> OnNewNote;

		// Token: 0x04003528 RID: 13608
		private List<EventEditorScriptConsole.NoteInfo> _allNotes;

		// Token: 0x04003529 RID: 13609
		private const float TweenTime = 0.25f;

		// Token: 0x0400352A RID: 13610
		private bool _isShowing;

		// Token: 0x02001A7E RID: 6782
		private struct NoteInfo
		{
			// Token: 0x0600DE55 RID: 56917 RVA: 0x005D56B8 File Offset: 0x005D38B8
			public override string ToString()
			{
				return string.Format("[{0:HH:mm:ss}]:{1}", this.DateTime, this.Note);
			}

			// Token: 0x0400B64D RID: 46669
			public DateTime DateTime;

			// Token: 0x0400B64E RID: 46670
			public string Note;
		}
	}
}
