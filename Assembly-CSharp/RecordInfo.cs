using System;
using System.IO;

// Token: 0x020002F8 RID: 760
public class RecordInfo : Refers
{
	// Token: 0x06002C95 RID: 11413 RVA: 0x0015DF7C File Offset: 0x0015C17C
	public string GetRecordFullName(string saveName = null)
	{
		string recordName = saveName ?? this.GetRecordName();
		return (recordName != null && recordName.Length > 0) ? string.Concat(new string[]
		{
			"<color=#F8E6C1>",
			recordName,
			"</color><color=#BDBDBD>(",
			this.CharacterName,
			")</color>"
		}) : ("<color=#F8E6C1>" + this.CharacterName + "</color>");
	}

	// Token: 0x06002C96 RID: 11414 RVA: 0x0015DFE8 File Offset: 0x0015C1E8
	public string GetRecordName()
	{
		string file = Path.Combine(GameApp.GetArchiveDirPath(), string.Format("world_{0}", this.Index), "name");
		if (File.Exists(file))
		{
			string recordName = File.ReadAllText(file).Replace("\r", "").Replace("\n", "");
			if (recordName != null)
			{
				int length = recordName.Length;
				if (length > 0 && length <= 6)
				{
					return recordName;
				}
			}
		}
		return "";
	}

	// Token: 0x06002C97 RID: 11415 RVA: 0x0015E068 File Offset: 0x0015C268
	public void EditRecordName(Renamer renamer, string saveName)
	{
		File.WriteAllText(Path.Combine(GameApp.GetArchiveDirPath(), string.Format("world_{0}", this.Index), "name"), saveName);
		renamer.Name.text = this.GetRecordFullName(saveName);
	}

	// Token: 0x06002C98 RID: 11416 RVA: 0x0015E0B4 File Offset: 0x0015C2B4
	public void OnRenameClicked(Renamer renamer, string nonsense = "")
	{
		renamer.Input.text = this.GetRecordName();
	}

	// Token: 0x04002045 RID: 8261
	public int Index;

	// Token: 0x04002046 RID: 8262
	public string CharacterName;
}
