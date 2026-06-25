using System;
using GameData.Domains.Character;
using GameData.Domains.Character.Display;
using GameData.Serializer;
using GameData.Utilities;
using TMPro;
using UnityEngine.EventSystems;

namespace FrameWork.UISystem.UIElements
{
	// Token: 0x02000FF9 RID: 4089
	public class ButtonRandomName : CButton
	{
		// Token: 0x0600BAB5 RID: 47797 RVA: 0x00550DA4 File Offset: 0x0054EFA4
		public void Refresh(TMP_InputField field, sbyte type)
		{
			bool flag = (type != 2 && type != 4 && type != 3) || field == null;
			if (flag)
			{
				base.gameObject.SetActive(false);
			}
			else
			{
				this._inputField = field;
				this._eventInputDataType = type;
				base.gameObject.SetActive(true);
			}
		}

		// Token: 0x0600BAB6 RID: 47798 RVA: 0x00550DF8 File Offset: 0x0054EFF8
		public void Refresh(TMP_InputField field, TMP_InputField field2, sbyte type)
		{
			bool flag = type != 5 || field == null || field2 == null;
			if (flag)
			{
				base.gameObject.SetActive(false);
			}
			else
			{
				this._inputField = field;
				this._inputFieldSecondary = field2;
				this._eventInputDataType = type;
				base.gameObject.SetActive(true);
			}
		}

		// Token: 0x0600BAB7 RID: 47799 RVA: 0x00550E54 File Offset: 0x0054F054
		public override void OnPointerClick(PointerEventData eventData)
		{
			bool flag = !UGUIUtils.IsScreenAreaInteract() || eventData.button > PointerEventData.InputButton.Left;
			if (!flag)
			{
				CharacterDomainMethod.AsyncCall.GenerateRandomName(null, delegate(int offset, RawDataPool dataPool)
				{
					NameRelatedData data = new NameRelatedData();
					Serializer.Deserialize(dataPool, offset, ref data);
					ValueTuple<string, string> name = data.FullName.GetName(data.Gender, null);
					string surName = name.Item1;
					string givenName = name.Item2;
					switch (this._eventInputDataType)
					{
					case 2:
						this._inputField.text = NameCenter.FormatName(surName, givenName);
						break;
					case 3:
						this._inputField.text = givenName;
						break;
					case 4:
						this._inputField.text = surName;
						break;
					case 5:
						this._inputField.text = surName;
						this._inputFieldSecondary.text = givenName;
						break;
					}
				});
				base.OnPointerClick(eventData);
			}
		}

		// Token: 0x0400903F RID: 36927
		private TMP_InputField _inputField;

		// Token: 0x04009040 RID: 36928
		private sbyte _eventInputDataType;

		// Token: 0x04009041 RID: 36929
		private TMP_InputField _inputFieldSecondary;
	}
}
