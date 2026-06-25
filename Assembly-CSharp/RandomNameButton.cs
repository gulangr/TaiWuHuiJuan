using System;
using GameData.Domains.Character;
using GameData.Domains.Character.Display;
using GameData.Serializer;
using GameData.Utilities;
using TMPro;
using UnityEngine.EventSystems;

// Token: 0x0200008E RID: 142
public class RandomNameButton : CButtonObsolete
{
	// Token: 0x0600050E RID: 1294 RVA: 0x00022C90 File Offset: 0x00020E90
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

	// Token: 0x0600050F RID: 1295 RVA: 0x00022CE4 File Offset: 0x00020EE4
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
				}
			});
			base.OnPointerClick(eventData);
		}
	}

	// Token: 0x04000414 RID: 1044
	private TMP_InputField _inputField;

	// Token: 0x04000415 RID: 1045
	private sbyte _eventInputDataType;
}
