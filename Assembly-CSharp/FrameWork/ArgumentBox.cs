using System;
using System.Collections.Generic;
using GameData.Serializer;

namespace FrameWork
{
	// Token: 0x02000FE2 RID: 4066
	public class ArgumentBox : ICloneable
	{
		// Token: 0x0600B9AD RID: 47533 RVA: 0x0054A12C File Offset: 0x0054832C
		private ArgumentBox Set<T>(ref Dictionary<string, T> dict, string key, T value)
		{
			if (dict == null)
			{
				dict = new Dictionary<string, T>();
			}
			dict.Remove(key);
			bool flag = value != null;
			if (flag)
			{
				dict.Add(key, value);
			}
			return this;
		}

		// Token: 0x0600B9AE RID: 47534 RVA: 0x0054A16C File Offset: 0x0054836C
		private bool Get<T>(IReadOnlyDictionary<string, T> dict, string key, out T value)
		{
			bool flag = dict != null;
			bool result;
			if (flag)
			{
				result = dict.TryGetValue(key, out value);
			}
			else
			{
				value = default(T);
				result = false;
			}
			return result;
		}

		// Token: 0x0600B9AF RID: 47535 RVA: 0x0054A19C File Offset: 0x0054839C
		private ArgumentBox Remove<T>(IDictionary<string, T> dict, string key)
		{
			if (dict != null)
			{
				dict.Remove(key);
			}
			return this;
		}

		// Token: 0x0600B9B0 RID: 47536 RVA: 0x0054A1BC File Offset: 0x005483BC
		private void CopyFrom<T>(ref Dictionary<string, T> dst, IReadOnlyDictionary<string, T> src)
		{
			Dictionary<string, T> dictionary = dst;
			if (dictionary != null)
			{
				dictionary.Clear();
			}
			bool flag = src == null || src.Count <= 0;
			if (!flag)
			{
				if (dst == null)
				{
					dst = new Dictionary<string, T>();
				}
				foreach (KeyValuePair<string, T> kvp in src)
				{
					dst[kvp.Key] = kvp.Value;
				}
			}
		}

		// Token: 0x0600B9B1 RID: 47537 RVA: 0x0054A248 File Offset: 0x00548448
		public ArgumentBox SetObject(string key, object arg)
		{
			return this.Set<object>(ref this._objectArgs, key, arg);
		}

		// Token: 0x0600B9B2 RID: 47538 RVA: 0x0054A258 File Offset: 0x00548458
		public ArgumentBox Set<T>(string key, T arg) where T : ISerializableGameData
		{
			return this.Set<object>(ref this._objectArgs, key, arg);
		}

		// Token: 0x0600B9B3 RID: 47539 RVA: 0x0054A26D File Offset: 0x0054846D
		public ArgumentBox Set(string key, Enum arg)
		{
			return this.Set<Enum>(ref this._enumArgs, key, arg);
		}

		// Token: 0x0600B9B4 RID: 47540 RVA: 0x0054A27D File Offset: 0x0054847D
		public ArgumentBox Set(string key, string arg)
		{
			return this.Set<string>(ref this._stringArgs, key, arg);
		}

		// Token: 0x0600B9B5 RID: 47541 RVA: 0x0054A28D File Offset: 0x0054848D
		public ArgumentBox Set(string key, bool arg)
		{
			return this.Set<bool>(ref this._boolArgs, key, arg);
		}

		// Token: 0x0600B9B6 RID: 47542 RVA: 0x0054A29D File Offset: 0x0054849D
		public ArgumentBox Set(string key, float arg)
		{
			return this.Set<float>(ref this._floatArgs, key, arg);
		}

		// Token: 0x0600B9B7 RID: 47543 RVA: 0x0054A2AD File Offset: 0x005484AD
		public ArgumentBox Set(string key, int arg)
		{
			return this.Set<int>(ref this._intArgs, key, arg);
		}

		// Token: 0x0600B9B8 RID: 47544 RVA: 0x0054A2BD File Offset: 0x005484BD
		public ArgumentBox Set(string key, byte arg)
		{
			return this.Set<byte>(ref this._byteArgs, key, arg);
		}

		// Token: 0x0600B9B9 RID: 47545 RVA: 0x0054A2CD File Offset: 0x005484CD
		public ArgumentBox Set(string key, sbyte arg)
		{
			return this.Set<sbyte>(ref this._sbyteArgs, key, arg);
		}

		// Token: 0x0600B9BA RID: 47546 RVA: 0x0054A2DD File Offset: 0x005484DD
		public ArgumentBox Set(string key, short arg)
		{
			return this.Set<short>(ref this._shortArgs, key, arg);
		}

		// Token: 0x0600B9BB RID: 47547 RVA: 0x0054A2ED File Offset: 0x005484ED
		public ArgumentBox Set(string key, ushort arg)
		{
			return this.Set<ushort>(ref this._ushortArgs, key, arg);
		}

		// Token: 0x0600B9BC RID: 47548 RVA: 0x0054A2FD File Offset: 0x005484FD
		public ArgumentBox Set(string key, long arg)
		{
			return this.Set<long>(ref this._longArgs, key, arg);
		}

		// Token: 0x0600B9BD RID: 47549 RVA: 0x0054A310 File Offset: 0x00548510
		public bool Get<T>(string key, out T arg)
		{
			object obj;
			bool flag = this._objectArgs != null && this._objectArgs.TryGetValue(key, out obj);
			bool result;
			if (flag)
			{
				arg = (T)((object)obj);
				result = true;
			}
			else
			{
				arg = default(T);
				result = false;
			}
			return result;
		}

		// Token: 0x0600B9BE RID: 47550 RVA: 0x0054A358 File Offset: 0x00548558
		public bool Get(string key, out Enum arg)
		{
			return this.Get<Enum>(this._enumArgs, key, out arg);
		}

		// Token: 0x0600B9BF RID: 47551 RVA: 0x0054A368 File Offset: 0x00548568
		public bool Get(string key, out string arg)
		{
			return this.Get<string>(this._stringArgs, key, out arg);
		}

		// Token: 0x0600B9C0 RID: 47552 RVA: 0x0054A378 File Offset: 0x00548578
		public bool Get(string key, out bool arg)
		{
			return this.Get<bool>(this._boolArgs, key, out arg);
		}

		// Token: 0x0600B9C1 RID: 47553 RVA: 0x0054A388 File Offset: 0x00548588
		public bool Get(string key, out float arg)
		{
			return this.Get<float>(this._floatArgs, key, out arg);
		}

		// Token: 0x0600B9C2 RID: 47554 RVA: 0x0054A398 File Offset: 0x00548598
		public bool Get(string key, out int arg)
		{
			return this.Get<int>(this._intArgs, key, out arg);
		}

		// Token: 0x0600B9C3 RID: 47555 RVA: 0x0054A3A8 File Offset: 0x005485A8
		public bool Get(string key, out byte arg)
		{
			return this.Get<byte>(this._byteArgs, key, out arg);
		}

		// Token: 0x0600B9C4 RID: 47556 RVA: 0x0054A3B8 File Offset: 0x005485B8
		public bool Get(string key, out sbyte arg)
		{
			return this.Get<sbyte>(this._sbyteArgs, key, out arg);
		}

		// Token: 0x0600B9C5 RID: 47557 RVA: 0x0054A3C8 File Offset: 0x005485C8
		public bool Get(string key, out short arg)
		{
			return this.Get<short>(this._shortArgs, key, out arg);
		}

		// Token: 0x0600B9C6 RID: 47558 RVA: 0x0054A3D8 File Offset: 0x005485D8
		public bool Get(string key, out ushort arg)
		{
			return this.Get<ushort>(this._ushortArgs, key, out arg);
		}

		// Token: 0x0600B9C7 RID: 47559 RVA: 0x0054A3E8 File Offset: 0x005485E8
		public bool Get(string key, out long arg)
		{
			return this.Get<long>(this._longArgs, key, out arg);
		}

		// Token: 0x0600B9C8 RID: 47560 RVA: 0x0054A3F8 File Offset: 0x005485F8
		public ArgumentBox RemoveObject(string key)
		{
			return this.Remove<object>(this._objectArgs, key);
		}

		// Token: 0x0600B9C9 RID: 47561 RVA: 0x0054A407 File Offset: 0x00548607
		public ArgumentBox RemoveEnum(string key)
		{
			return this.Remove<Enum>(this._enumArgs, key);
		}

		// Token: 0x0600B9CA RID: 47562 RVA: 0x0054A416 File Offset: 0x00548616
		public ArgumentBox RemoveString(string key)
		{
			return this.Remove<string>(this._stringArgs, key);
		}

		// Token: 0x0600B9CB RID: 47563 RVA: 0x0054A425 File Offset: 0x00548625
		public ArgumentBox RemoveBool(string key)
		{
			return this.Remove<bool>(this._boolArgs, key);
		}

		// Token: 0x0600B9CC RID: 47564 RVA: 0x0054A434 File Offset: 0x00548634
		public ArgumentBox RemoveFloat(string key)
		{
			return this.Remove<float>(this._floatArgs, key);
		}

		// Token: 0x0600B9CD RID: 47565 RVA: 0x0054A443 File Offset: 0x00548643
		public ArgumentBox RemoveInt(string key)
		{
			return this.Remove<string>(this._stringArgs, key);
		}

		// Token: 0x0600B9CE RID: 47566 RVA: 0x0054A452 File Offset: 0x00548652
		public ArgumentBox RemoveByte(string key)
		{
			return this.Remove<byte>(this._byteArgs, key);
		}

		// Token: 0x0600B9CF RID: 47567 RVA: 0x0054A461 File Offset: 0x00548661
		public ArgumentBox RemoveSbyte(string key)
		{
			return this.Remove<sbyte>(this._sbyteArgs, key);
		}

		// Token: 0x0600B9D0 RID: 47568 RVA: 0x0054A470 File Offset: 0x00548670
		public ArgumentBox RemoveShort(string key)
		{
			return this.Remove<short>(this._shortArgs, key);
		}

		// Token: 0x0600B9D1 RID: 47569 RVA: 0x0054A47F File Offset: 0x0054867F
		public ArgumentBox RemoveUshort(string key)
		{
			return this.Remove<short>(this._shortArgs, key);
		}

		// Token: 0x0600B9D2 RID: 47570 RVA: 0x0054A48E File Offset: 0x0054868E
		public ArgumentBox RemoveLong(string key)
		{
			return this.Remove<long>(this._longArgs, key);
		}

		// Token: 0x0600B9D3 RID: 47571 RVA: 0x0054A4A0 File Offset: 0x005486A0
		public ArgumentBox Remove<T>(string key)
		{
			bool flag = typeof(T) == typeof(Enum);
			ArgumentBox result;
			if (flag)
			{
				result = this.RemoveEnum(key);
			}
			else
			{
				bool flag2 = typeof(T) == typeof(string);
				if (flag2)
				{
					result = this.RemoveString(key);
				}
				else
				{
					bool flag3 = typeof(T) == typeof(bool);
					if (flag3)
					{
						result = this.RemoveBool(key);
					}
					else
					{
						bool flag4 = typeof(T) == typeof(float);
						if (flag4)
						{
							result = this.RemoveFloat(key);
						}
						else
						{
							bool flag5 = typeof(T) == typeof(int);
							if (flag5)
							{
								result = this.RemoveInt(key);
							}
							else
							{
								bool flag6 = typeof(T) == typeof(byte);
								if (flag6)
								{
									result = this.RemoveByte(key);
								}
								else
								{
									bool flag7 = typeof(T) == typeof(sbyte);
									if (flag7)
									{
										result = this.RemoveSbyte(key);
									}
									else
									{
										bool flag8 = typeof(T) == typeof(short);
										if (flag8)
										{
											result = this.RemoveShort(key);
										}
										else
										{
											bool flag9 = typeof(T) == typeof(ushort);
											if (flag9)
											{
												result = this.RemoveUshort(key);
											}
											else
											{
												bool flag10 = typeof(T) == typeof(long);
												if (flag10)
												{
													result = this.RemoveLong(key);
												}
												else
												{
													result = this;
												}
											}
										}
									}
								}
							}
						}
					}
				}
			}
			return result;
		}

		// Token: 0x0600B9D4 RID: 47572 RVA: 0x0054A65C File Offset: 0x0054885C
		public ArgumentBox RemoveAll(string key)
		{
			return this.RemoveInt(key).RemoveString(key).RemoveFloat(key).RemoveEnum(key).RemoveBool(key).RemoveByte(key).RemoveSbyte(key).RemoveShort(key).RemoveUshort(key).RemoveLong(key).RemoveObject(key);
		}

		// Token: 0x0600B9D5 RID: 47573 RVA: 0x0054A6B4 File Offset: 0x005488B4
		public void Clear()
		{
			Dictionary<string, object> objectArgs = this._objectArgs;
			if (objectArgs != null)
			{
				objectArgs.Clear();
			}
			Dictionary<string, Enum> enumArgs = this._enumArgs;
			if (enumArgs != null)
			{
				enumArgs.Clear();
			}
			Dictionary<string, string> stringArgs = this._stringArgs;
			if (stringArgs != null)
			{
				stringArgs.Clear();
			}
			Dictionary<string, bool> boolArgs = this._boolArgs;
			if (boolArgs != null)
			{
				boolArgs.Clear();
			}
			Dictionary<string, float> floatArgs = this._floatArgs;
			if (floatArgs != null)
			{
				floatArgs.Clear();
			}
			Dictionary<string, int> intArgs = this._intArgs;
			if (intArgs != null)
			{
				intArgs.Clear();
			}
			Dictionary<string, byte> byteArgs = this._byteArgs;
			if (byteArgs != null)
			{
				byteArgs.Clear();
			}
			Dictionary<string, sbyte> sbyteArgs = this._sbyteArgs;
			if (sbyteArgs != null)
			{
				sbyteArgs.Clear();
			}
			Dictionary<string, short> shortArgs = this._shortArgs;
			if (shortArgs != null)
			{
				shortArgs.Clear();
			}
			Dictionary<string, ushort> ushortArgs = this._ushortArgs;
			if (ushortArgs != null)
			{
				ushortArgs.Clear();
			}
			Dictionary<string, long> longArgs = this._longArgs;
			if (longArgs != null)
			{
				longArgs.Clear();
			}
		}

		// Token: 0x0600B9D6 RID: 47574 RVA: 0x0054A788 File Offset: 0x00548988
		public ArgumentBox CopyFrom(ArgumentBox other)
		{
			this.CopyFrom<object>(ref this._objectArgs, other._objectArgs);
			this.CopyFrom<Enum>(ref this._enumArgs, other._enumArgs);
			this.CopyFrom<string>(ref this._stringArgs, other._stringArgs);
			this.CopyFrom<bool>(ref this._boolArgs, other._boolArgs);
			this.CopyFrom<float>(ref this._floatArgs, other._floatArgs);
			this.CopyFrom<int>(ref this._intArgs, other._intArgs);
			this.CopyFrom<byte>(ref this._byteArgs, other._byteArgs);
			this.CopyFrom<sbyte>(ref this._sbyteArgs, other._sbyteArgs);
			this.CopyFrom<short>(ref this._shortArgs, other._shortArgs);
			this.CopyFrom<ushort>(ref this._ushortArgs, other._ushortArgs);
			this.CopyFrom<long>(ref this._longArgs, other._longArgs);
			return this;
		}

		// Token: 0x0600B9D7 RID: 47575 RVA: 0x0054A86C File Offset: 0x00548A6C
		public object Clone()
		{
			ArgumentBox box = new ArgumentBox();
			bool flag = this._intArgs != null;
			if (flag)
			{
				box._intArgs = new Dictionary<string, int>(this._intArgs);
			}
			bool flag2 = this._stringArgs != null;
			if (flag2)
			{
				box._stringArgs = new Dictionary<string, string>(this._stringArgs);
			}
			bool flag3 = this._floatArgs != null;
			if (flag3)
			{
				box._floatArgs = new Dictionary<string, float>(this._floatArgs);
			}
			bool flag4 = this._boolArgs != null;
			if (flag4)
			{
				box._boolArgs = new Dictionary<string, bool>(this._boolArgs);
			}
			bool flag5 = this._enumArgs != null;
			if (flag5)
			{
				box._enumArgs = new Dictionary<string, Enum>(this._enumArgs);
			}
			bool flag6 = this._byteArgs != null;
			if (flag6)
			{
				box._byteArgs = new Dictionary<string, byte>(this._byteArgs);
			}
			bool flag7 = this._sbyteArgs != null;
			if (flag7)
			{
				box._sbyteArgs = new Dictionary<string, sbyte>(this._sbyteArgs);
			}
			bool flag8 = this._shortArgs != null;
			if (flag8)
			{
				box._shortArgs = new Dictionary<string, short>(this._shortArgs);
			}
			bool flag9 = this._ushortArgs != null;
			if (flag9)
			{
				box._ushortArgs = new Dictionary<string, ushort>(this._ushortArgs);
			}
			bool flag10 = this._objectArgs != null;
			if (flag10)
			{
				box._objectArgs = new Dictionary<string, object>(this._objectArgs);
			}
			bool flag11 = this._longArgs != null;
			if (flag11)
			{
				box._longArgs = new Dictionary<string, long>(this._longArgs);
			}
			return box;
		}

		// Token: 0x04008FB1 RID: 36785
		private Dictionary<string, object> _objectArgs;

		// Token: 0x04008FB2 RID: 36786
		private Dictionary<string, Enum> _enumArgs;

		// Token: 0x04008FB3 RID: 36787
		private Dictionary<string, string> _stringArgs;

		// Token: 0x04008FB4 RID: 36788
		private Dictionary<string, bool> _boolArgs;

		// Token: 0x04008FB5 RID: 36789
		private Dictionary<string, float> _floatArgs;

		// Token: 0x04008FB6 RID: 36790
		private Dictionary<string, int> _intArgs;

		// Token: 0x04008FB7 RID: 36791
		private Dictionary<string, byte> _byteArgs;

		// Token: 0x04008FB8 RID: 36792
		private Dictionary<string, sbyte> _sbyteArgs;

		// Token: 0x04008FB9 RID: 36793
		private Dictionary<string, short> _shortArgs;

		// Token: 0x04008FBA RID: 36794
		private Dictionary<string, ushort> _ushortArgs;

		// Token: 0x04008FBB RID: 36795
		private Dictionary<string, long> _longArgs;
	}
}
