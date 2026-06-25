using System;

// Token: 0x0200014B RID: 331
public struct StringKey
{
	// Token: 0x06001281 RID: 4737 RVA: 0x00070D60 File Offset: 0x0006EF60
	private StringKey.Type GetKeyType()
	{
		bool flag = this._key > LanguageKey.EventEditor_Error_DuplicateGroupKey;
		StringKey.Type result;
		if (flag)
		{
			result = StringKey.Type.ShortKey;
		}
		else
		{
			bool flag2 = this._directString != null;
			if (flag2)
			{
				result = StringKey.Type.DirectString;
			}
			else
			{
				result = StringKey.Type.StringKey;
			}
		}
		return result;
	}

	// Token: 0x06001282 RID: 4738 RVA: 0x00070D98 File Offset: 0x0006EF98
	public static StringKey CreateKey(LanguageKey key)
	{
		return new StringKey
		{
			_key = key,
			_directString = null
		};
	}

	// Token: 0x06001283 RID: 4739 RVA: 0x00070DC4 File Offset: 0x0006EFC4
	public static StringKey CreateKey(string stringKey)
	{
		return new StringKey
		{
			_key = LanguageKey.EventEditor_Error_DuplicateGroupKey,
			_directString = null,
			_stringKey = stringKey
		};
	}

	// Token: 0x06001284 RID: 4740 RVA: 0x00070DF8 File Offset: 0x0006EFF8
	public static StringKey CreateDirect(string directString)
	{
		return new StringKey
		{
			_key = LanguageKey.EventEditor_Error_DuplicateGroupKey,
			_directString = directString
		};
	}

	// Token: 0x06001285 RID: 4741 RVA: 0x00070E23 File Offset: 0x0006F023
	public static implicit operator StringKey(LanguageKey key)
	{
		return StringKey.CreateKey(key);
	}

	// Token: 0x06001286 RID: 4742 RVA: 0x00070E2B File Offset: 0x0006F02B
	public static implicit operator StringKey(string directString)
	{
		return StringKey.CreateDirect(directString);
	}

	// Token: 0x06001287 RID: 4743 RVA: 0x00070E34 File Offset: 0x0006F034
	public string GetString()
	{
		StringKey.Type keyType = this.GetKeyType();
		if (!true)
		{
		}
		string result;
		switch (keyType)
		{
		case StringKey.Type.ShortKey:
			result = LocalStringManager.Get(this._key);
			break;
		case StringKey.Type.StringKey:
			result = LocalStringManager.Get(this._stringKey);
			break;
		case StringKey.Type.DirectString:
			result = this._directString;
			break;
		default:
			throw new Exception("Unknown StringKey type");
		}
		if (!true)
		{
		}
		return result;
	}

	// Token: 0x04000F9C RID: 3996
	private LanguageKey _key;

	// Token: 0x04000F9D RID: 3997
	private string _stringKey;

	// Token: 0x04000F9E RID: 3998
	private string _directString;

	// Token: 0x0200122A RID: 4650
	public enum Type
	{
		// Token: 0x040099BB RID: 39355
		ShortKey,
		// Token: 0x040099BC RID: 39356
		StringKey,
		// Token: 0x040099BD RID: 39357
		DirectString
	}
}
