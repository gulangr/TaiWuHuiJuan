using System;
using Game.Views.Encyclopedia.Event;
using Game.Views.Encyclopedia.Views;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Game.Views.Encyclopedia.Elements
{
	// Token: 0x02000A85 RID: 2693
	public class EncyclopediaTextLinkHelper : MonoBehaviour, IPointerClickHandler, IEventSystemHandler, IPointerEnterHandler, IPointerExitHandler
	{
		// Token: 0x06008412 RID: 33810 RVA: 0x003D5EF0 File Offset: 0x003D40F0
		public void Set(IEncyclopediaSearchableContent content, TMP_InputField inputField)
		{
			this.bg.enabled = false;
			TMP_Text tmp_Text = this.text;
			this._content = content;
			tmp_Text.text = content.DisplayText;
			this.field = inputField;
		}

		// Token: 0x06008413 RID: 33811 RVA: 0x003D5F30 File Offset: 0x003D4130
		public void OnPointerClick(PointerEventData eventData)
		{
			this.ub.Play(eventData.button, true);
			object locker = BasicInfoView.Locker;
			int cnt;
			lock (locker)
			{
				BasicInfoView.Counter -= (cnt = BasicInfoView.Counter);
			}
			TMP_InputField.SubmitEvent onEndEdit = this.field.onEndEdit;
			if (onEndEdit != null)
			{
				TMP_InputField tmp_InputField = this.field;
				IEncyclopediaSearchableContent content = this._content;
				onEndEdit.Invoke(tmp_InputField.text = (((content != null) ? content.Content : null) ?? string.Empty));
			}
			object locker2 = BasicInfoView.Locker;
			lock (locker2)
			{
				BasicInfoView.Counter += cnt;
			}
			bool flag3 = this.text.textInfo.linkCount == 0 || this._content == null;
			if (flag3)
			{
				this.field.DeactivateInputField(false);
			}
			else
			{
				EventManager.Instance.Dispatch(this._content.ClickEventType, this._content.ClickEventArgs);
				this.field.text = this._content.Content;
			}
		}

		// Token: 0x06008414 RID: 33812 RVA: 0x003D607C File Offset: 0x003D427C
		public void OnPointerEnter(PointerEventData eventData)
		{
			object locker = BasicInfoView.Locker;
			lock (locker)
			{
				this._counter++;
				BasicInfoView.Counter++;
			}
			this.bg.enabled = true;
		}

		// Token: 0x06008415 RID: 33813 RVA: 0x003D60E4 File Offset: 0x003D42E4
		public void OnPointerExit(PointerEventData eventData)
		{
			object locker = BasicInfoView.Locker;
			lock (locker)
			{
				BasicInfoView.Counter -= this._counter;
				this._counter = 0;
			}
			this.bg.enabled = false;
		}

		// Token: 0x06008416 RID: 33814 RVA: 0x003D6148 File Offset: 0x003D4348
		private void OnDisable()
		{
			this.OnPointerExit(null);
		}

		// Token: 0x04006523 RID: 25891
		[SerializeField]
		private CImage bg;

		// Token: 0x04006524 RID: 25892
		[SerializeField]
		private TMP_Text text;

		// Token: 0x04006525 RID: 25893
		[SerializeField]
		private UIInteractionBehaviour ub;

		// Token: 0x04006526 RID: 25894
		[ReadOnly]
		[SerializeField]
		private TMP_InputField field;

		// Token: 0x04006527 RID: 25895
		private IEncyclopediaSearchableContent _content;

		// Token: 0x04006528 RID: 25896
		private int _counter;
	}
}
