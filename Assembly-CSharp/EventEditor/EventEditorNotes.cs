using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using FrameWork.UISystem.Components;
using Game.Views.Migrate;
using UnityEngine;

namespace EventEditor
{
	// Token: 0x02000646 RID: 1606
	public class EventEditorNotes : EventEditorSubPageBase
	{
		// Token: 0x06004C45 RID: 19525 RVA: 0x00240036 File Offset: 0x0023E236
		public static void Init(EventEditorNotes obj)
		{
			EventEditorNotes.Instance = obj;
			EventEditorNotes.Instance.InternalInit();
		}

		// Token: 0x06004C46 RID: 19526 RVA: 0x0024004C File Offset: 0x0023E24C
		protected override void InternalInit()
		{
			this._allNotes = new List<EventEditorNotes.NoteInfo>();
			this.noteWindow.localScale = new Vector3(1f, 0f, 1f);
			this.notesScroll.OnItemRender += this.OnNoteItemRender;
			this.notesScroll.initSuccess = false;
		}

		// Token: 0x06004C47 RID: 19527 RVA: 0x002400AC File Offset: 0x0023E2AC
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

		// Token: 0x06004C48 RID: 19528 RVA: 0x00240108 File Offset: 0x0023E308
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

		// Token: 0x06004C49 RID: 19529 RVA: 0x0024015A File Offset: 0x0023E35A
		private IEnumerator ScrollToEnd()
		{
			yield return new WaitForEndOfFrame();
			yield return new WaitForEndOfFrame();
			this.notesScroll.ScrollToEnd();
			yield break;
		}

		// Token: 0x06004C4A RID: 19530 RVA: 0x0024016C File Offset: 0x0023E36C
		private void OnNoteItemRender(int dataIndex, GameObject goNoteCell)
		{
			EventEditorNotesNoteCellPrefabInfo noteCellInfo = goNoteCell.GetComponent<EventEditorNotesNoteCellPrefabInfo>();
			noteCellInfo.img.enabled = (dataIndex % 2 == 0);
			noteCellInfo.txtMeshContent.text = this._allNotes[dataIndex].ToString();
		}

		// Token: 0x06004C4B RID: 19531 RVA: 0x002401BC File Offset: 0x0023E3BC
		public void AddNote(string note)
		{
			this._allNotes.Add(new EventEditorNotes.NoteInfo
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

		// Token: 0x040034EF RID: 13551
		public static EventEditorNotes Instance;

		// Token: 0x040034F0 RID: 13552
		[SerializeField]
		private RectTransform noteWindow;

		// Token: 0x040034F1 RID: 13553
		[SerializeField]
		private InfinityScroll notesScroll;

		// Token: 0x040034F2 RID: 13554
		[NonSerialized]
		public Action<string> OnNewNote;

		// Token: 0x040034F3 RID: 13555
		private List<EventEditorNotes.NoteInfo> _allNotes;

		// Token: 0x040034F4 RID: 13556
		private const float TweenTime = 0.25f;

		// Token: 0x040034F5 RID: 13557
		private bool _isShowing;

		// Token: 0x02001A6E RID: 6766
		private struct NoteInfo
		{
			// Token: 0x0600DE27 RID: 56871 RVA: 0x005D4D60 File Offset: 0x005D2F60
			public override string ToString()
			{
				return string.Format("[{0:HH:mm:ss}]:{1}", this.DateTime, this.Note);
			}

			// Token: 0x0400B624 RID: 46628
			public DateTime DateTime;

			// Token: 0x0400B625 RID: 46629
			public string Note;
		}
	}
}
