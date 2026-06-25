using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.Serialization;
using System.Text;
using GameData.Utilities;
using MoonSharp.Interpreter;
using Newtonsoft.Json.Linq;

namespace GameData.Serializer
{
	// Token: 0x02000FD4 RID: 4052
	public static class CommonObjectSerializer
	{
		// Token: 0x0600B94E RID: 47438 RVA: 0x00546DB8 File Offset: 0x00544FB8
		public static void Serialize(object obj, out string marshalData, CommonObjectSerializer.MarshalFormat formatHint)
		{
			switch (formatHint)
			{
			case CommonObjectSerializer.MarshalFormat.Lua:
				CommonObjectSerializer.SerializeAsLuaString(obj, out marshalData, 0);
				break;
			case CommonObjectSerializer.MarshalFormat.LuaWithReturnPrefix:
			{
				string rawMarshalData;
				CommonObjectSerializer.Serialize(obj, out rawMarshalData, CommonObjectSerializer.MarshalFormat.Lua);
				marshalData = "return " + rawMarshalData;
				break;
			}
			case CommonObjectSerializer.MarshalFormat.Json:
				CommonObjectSerializer.SerializeAsJsonString(obj, out marshalData, 0);
				break;
			default:
				throw new ArgumentOutOfRangeException("formatHint", formatHint, null);
			}
		}

		// Token: 0x0600B94F RID: 47439 RVA: 0x00546E1F File Offset: 0x0054501F
		public static void Serialize<T>(T obj, out string marshalData, CommonObjectSerializer.MarshalFormat formatHint)
		{
			CommonObjectSerializer.Serialize(obj, out marshalData, formatHint);
		}

		// Token: 0x0600B950 RID: 47440 RVA: 0x00546E30 File Offset: 0x00545030
		public static void Deserialize<T>(string marshalData, out T obj, CommonObjectSerializer.MarshalFormat formatHint)
		{
			object raw;
			CommonObjectSerializer.Deserialize(marshalData, out raw, typeof(T), formatHint);
			obj = (T)((object)raw);
		}

		// Token: 0x0600B951 RID: 47441 RVA: 0x00546E60 File Offset: 0x00545060
		public static void Deserialize(string marshalData, out object obj, Type typeHint, CommonObjectSerializer.MarshalFormat formatHint)
		{
			switch (formatHint)
			{
			case CommonObjectSerializer.MarshalFormat.Lua:
				CommonObjectSerializer.DeserializeFromLuaValue(global::LuaParser.Parse("return " + marshalData, null), out obj, typeHint);
				break;
			case CommonObjectSerializer.MarshalFormat.LuaWithReturnPrefix:
				CommonObjectSerializer.DeserializeFromLuaValue(global::LuaParser.Parse(marshalData, null), out obj, typeHint);
				break;
			case CommonObjectSerializer.MarshalFormat.Json:
				CommonObjectSerializer.DeserializeFromJsonValue(JToken.Parse(marshalData), out obj, typeHint);
				break;
			default:
				throw new ArgumentOutOfRangeException("formatHint", formatHint, null);
			}
		}

		// Token: 0x0600B952 RID: 47442 RVA: 0x00546ED4 File Offset: 0x005450D4
		internal static void RestoreObjectArray<T>(string marshalData, T[] obj, CommonObjectSerializer.MarshalFormat formatHint)
		{
			if (formatHint > CommonObjectSerializer.MarshalFormat.LuaWithReturnPrefix)
			{
				throw new ArgumentOutOfRangeException("formatHint", formatHint, null);
			}
			DynValue luaValue = global::LuaParser.Parse((formatHint == CommonObjectSerializer.MarshalFormat.Lua) ? ("return " + marshalData) : marshalData, null);
			Table luaTable = luaValue.Table;
			bool flag = luaTable != null;
			if (flag)
			{
				for (int i = 0; i < obj.Length; i++)
				{
					bool flag2 = obj[i] != null;
					if (flag2)
					{
						object raw;
						CommonObjectSerializer.DeserializeFromLuaValue(luaTable.Get(DynValue.NewNumber((double)i)), out raw, obj[i].GetType());
						obj[i] = (T)((object)raw);
					}
				}
			}
		}

		// Token: 0x0600B953 RID: 47443 RVA: 0x00546F9C File Offset: 0x0054519C
		internal static void RestoreObject<T>(string marshalData, T obj, CommonObjectSerializer.MarshalFormat formatHint)
		{
			T template;
			CommonObjectSerializer.Deserialize<T>(marshalData, out template, formatHint);
			Dictionary<string, CommonObjectSerializationMember> dict = new Dictionary<string, CommonObjectSerializationMember>();
			foreach (KeyValuePair<string, CommonObjectSerializationMember> member in CommonObjectSerializer.GetMembers(template, false))
			{
				dict[member.Key] = member.Value;
			}
			foreach (KeyValuePair<string, CommonObjectSerializationMember> member2 in CommonObjectSerializer.GetMembers(obj, true))
			{
				CommonObjectSerializationMember originMember;
				bool flag = dict.TryGetValue(member2.Key, out originMember);
				if (flag)
				{
					member2.Value.Setter(originMember.Getter());
				}
			}
		}

		// Token: 0x0600B954 RID: 47444 RVA: 0x0054708C File Offset: 0x0054528C
		internal static string GetFileExtension(CommonObjectSerializer.MarshalFormat formatHint)
		{
			if (!true)
			{
			}
			string result;
			if (formatHint > CommonObjectSerializer.MarshalFormat.LuaWithReturnPrefix)
			{
				if (formatHint != CommonObjectSerializer.MarshalFormat.Json)
				{
					throw new ArgumentOutOfRangeException("formatHint", formatHint, null);
				}
				result = "json";
			}
			else
			{
				result = "lua";
			}
			if (!true)
			{
			}
			return result;
		}

		// Token: 0x0600B955 RID: 47445 RVA: 0x005470D8 File Offset: 0x005452D8
		private static IReadOnlyDictionary<string, MemberInfo> GetMemberDict(Type type)
		{
			Dictionary<string, MemberInfo> memberDict;
			bool flag = CommonObjectSerializer.CachedMemberDict.TryGetValue(type, out memberDict);
			IReadOnlyDictionary<string, MemberInfo> result;
			if (flag)
			{
				result = memberDict;
			}
			else
			{
				memberDict = new Dictionary<string, MemberInfo>();
				foreach (FieldInfo fieldInfo in type.GetFields(BindingFlags.Instance | BindingFlags.Public))
				{
					memberDict[fieldInfo.Name] = fieldInfo;
				}
				foreach (PropertyInfo propertyInfo in type.GetProperties(BindingFlags.Instance | BindingFlags.Public))
				{
					bool flag2 = !propertyInfo.CanWrite || !propertyInfo.CanRead || propertyInfo.GetIndexParameters().Length != 0;
					if (!flag2)
					{
						memberDict[propertyInfo.Name] = propertyInfo;
					}
				}
				CommonObjectSerializer.CachedMemberDict.Add(type, memberDict);
				result = memberDict;
			}
			return result;
		}

		// Token: 0x0600B956 RID: 47446 RVA: 0x005471A9 File Offset: 0x005453A9
		private static IEnumerable<KeyValuePair<string, CommonObjectSerializationMember>> GetMembers(object obj, bool deserializing)
		{
			CommonObjectSerializer.<>c__DisplayClass10_0 CS$<>8__locals1 = new CommonObjectSerializer.<>c__DisplayClass10_0();
			CS$<>8__locals1.obj = obj;
			foreach (KeyValuePair<string, MemberInfo> keyValuePair in CommonObjectSerializer.GetMemberDict(CS$<>8__locals1.obj.GetType()))
			{
				string text;
				MemberInfo memberInfo;
				keyValuePair.Deconstruct(out text, out memberInfo);
				string name = text;
				MemberInfo member = memberInfo;
				ICommonObjectSerializationAware aware = CS$<>8__locals1.obj as ICommonObjectSerializationAware;
				bool flag = aware != null && aware.SkipMember(member, deserializing);
				if (!flag)
				{
					CommonObjectSerializer.<>c__DisplayClass10_1 CS$<>8__locals2 = new CommonObjectSerializer.<>c__DisplayClass10_1();
					CS$<>8__locals2.CS$<>8__locals3 = CS$<>8__locals1;
					if (!true)
					{
					}
					CS$<>8__locals2.<field>5__2 = (member as FieldInfo);
					KeyValuePair<string, CommonObjectSerializationMember> keyValuePair2;
					if (CS$<>8__locals2.<field>5__2 == null)
					{
						CS$<>8__locals2.<property>5__3 = (member as PropertyInfo);
						if (CS$<>8__locals2.<property>5__3 == null)
						{
							throw new InvalidCastException();
						}
						keyValuePair2 = new KeyValuePair<string, CommonObjectSerializationMember>(name, new CommonObjectSerializationMember(member.Name, () => CS$<>8__locals2.<property>5__3.GetValue(CS$<>8__locals2.CS$<>8__locals3.obj), delegate(object v)
						{
							CS$<>8__locals2.<property>5__3.SetValue(CS$<>8__locals2.CS$<>8__locals3.obj, v);
						}, CS$<>8__locals2.<property>5__3.PropertyType, member));
					}
					else
					{
						keyValuePair2 = new KeyValuePair<string, CommonObjectSerializationMember>(name, new CommonObjectSerializationMember(member.Name, () => CS$<>8__locals2.<field>5__2.GetValue(CS$<>8__locals2.CS$<>8__locals3.obj), delegate(object v)
						{
							CS$<>8__locals2.<field>5__2.SetValue(CS$<>8__locals2.CS$<>8__locals3.obj, v);
						}, CS$<>8__locals2.<field>5__2.FieldType, member));
					}
					if (!true)
					{
					}
					yield return keyValuePair2;
					CS$<>8__locals2 = null;
					aware = null;
					name = null;
					member = null;
				}
			}
			IEnumerator<KeyValuePair<string, MemberInfo>> enumerator = null;
			ICommonObjectSerializationAware aware2 = CS$<>8__locals1.obj as ICommonObjectSerializationAware;
			bool flag2 = aware2 != null;
			if (flag2)
			{
				foreach (CommonObjectSerializationMember extra in aware2.ExtraMembers(deserializing))
				{
					yield return new KeyValuePair<string, CommonObjectSerializationMember>(extra.Name, extra);
					extra = default(CommonObjectSerializationMember);
				}
				IEnumerator<CommonObjectSerializationMember> enumerator2 = null;
			}
			aware2 = null;
			yield break;
			yield break;
		}

		// Token: 0x0600B957 RID: 47447 RVA: 0x005471C0 File Offset: 0x005453C0
		private static void SerializeAsLuaString(object obj, out string luaString, int indent)
		{
			StringWriter sw = new StringWriter();
			CommonObjectSerializer.CodeWriter code = new CommonObjectSerializer.CodeWriter(sw, "\t");
			code.Indent = indent;
			if (obj != null)
			{
				if (obj is bool)
				{
					bool objBool = (bool)obj;
					code.Write(objBool ? "true" : "false");
				}
				else
				{
					Enum objEnum = obj as Enum;
					if (objEnum == null)
					{
						ICollection objCollection = obj as ICollection;
						if (objCollection == null)
						{
							string objString = obj as string;
							if (objString == null)
							{
								IConvertible objConvertible = obj as IConvertible;
								if (objConvertible == null)
								{
									bool flag = indent > 0;
									if (flag)
									{
										code.WriteLine();
									}
									code.WriteLine('{');
									code.Indent++;
									int i = 0;
									foreach (KeyValuePair<string, CommonObjectSerializationMember> keyValuePair in CommonObjectSerializer.GetMembers(obj, false))
									{
										string text;
										CommonObjectSerializationMember commonObjectSerializationMember;
										keyValuePair.Deconstruct(out text, out commonObjectSerializationMember);
										string name = text;
										CommonObjectSerializationMember member = commonObjectSerializationMember;
										bool flag2 = i != 0;
										if (flag2)
										{
											code.WriteLine(',');
										}
										string subData;
										CommonObjectSerializer.SerializeAsLuaString(member.Getter(), out subData, code.Indent);
										int index;
										code.Write(int.TryParse(name, out index) ? string.Format("[{0}] = ", index) : (name + " = "));
										code.Write(subData);
										i++;
									}
									bool flag3 = i != 0;
									if (flag3)
									{
										code.WriteLine();
									}
									code.Indent--;
									code.Write('}');
								}
								else
								{
									code.Write(objConvertible.ToString(CultureInfo.InvariantCulture));
								}
							}
							else
							{
								code.Write('"');
								foreach (char c in objString)
								{
									char c2 = c;
									char c3 = c2;
									if (c3 < ' ')
									{
										switch (c3)
										{
										case '\t':
											code.Write("\\t");
											goto IL_33F;
										case '\n':
											code.Write("\\n");
											goto IL_33F;
										case '\r':
											code.Write("\\r");
											goto IL_33F;
										}
										code.Write("\\x");
										TextWriter textWriter = code;
										int num = (int)c;
										textWriter.Write(num.ToString("X2"));
									}
									else if (c3 != '"')
									{
										if (c3 != '\\')
										{
											code.Write(c);
										}
										else
										{
											code.Write("\\\\");
										}
									}
									else
									{
										code.Write("\\\"");
									}
									IL_33F:;
								}
								code.Write('"');
							}
						}
						else
						{
							bool flag4 = indent > 0;
							if (flag4)
							{
								code.WriteLine();
							}
							code.WriteLine('{');
							code.Indent++;
							IDictionary objDict = obj as IDictionary;
							bool flag5 = objDict != null;
							if (flag5)
							{
								int j = 0;
								foreach (object key in objDict.Keys)
								{
									string keyData;
									CommonObjectSerializer.SerializeAsLuaString(key, out keyData, code.Indent);
									string valueData;
									CommonObjectSerializer.SerializeAsLuaString(objDict[key], out valueData, code.Indent);
									code.Write("[" + keyData + "] = ");
									code.Write(valueData);
									code.WriteLine((j != objDict.Count - 1) ? ',' : string.Empty);
									j++;
								}
							}
							else
							{
								int k = 0;
								foreach (object item in objCollection)
								{
									string subData2;
									CommonObjectSerializer.SerializeAsLuaString(item, out subData2, code.Indent);
									code.Write(string.Format("[{0}] = ", k));
									code.Write(subData2);
									code.WriteLine((k != objCollection.Count - 1) ? ',' : string.Empty);
									k++;
								}
							}
							code.Indent--;
							code.Write('}');
						}
					}
					else
					{
						code.Write(objEnum.ToInt());
					}
				}
			}
			else
			{
				code.Write("nil");
			}
			luaString = sw.ToString();
		}

		// Token: 0x0600B958 RID: 47448 RVA: 0x00547690 File Offset: 0x00545890
		private static void SerializeAsJsonString(object obj, out string jsonString, int indent)
		{
			StringWriter sw = new StringWriter();
			CommonObjectSerializer.CodeWriter code = new CommonObjectSerializer.CodeWriter(sw, "\t");
			code.Indent = indent;
			if (obj != null)
			{
				if (obj is bool)
				{
					bool objBool = (bool)obj;
					code.Write(objBool ? "true" : "false");
				}
				else
				{
					IDictionary objDict = obj as IDictionary;
					if (objDict == null)
					{
						Enum objEnum = obj as Enum;
						if (objEnum != null)
						{
							CommonObjectSerializer.SerializeAsJsonString(objEnum.ToString(), out jsonString, indent);
							return;
						}
						ICollection objCollection = obj as ICollection;
						if (objCollection == null)
						{
							string objString = obj as string;
							if (objString == null)
							{
								IConvertible objConvertible = obj as IConvertible;
								if (objConvertible == null)
								{
									bool flag = indent > 0;
									if (flag)
									{
										code.WriteLine();
									}
									code.WriteLine('{');
									code.Indent++;
									int i = 0;
									foreach (KeyValuePair<string, CommonObjectSerializationMember> keyValuePair in CommonObjectSerializer.GetMembers(obj, false))
									{
										string text;
										CommonObjectSerializationMember commonObjectSerializationMember;
										keyValuePair.Deconstruct(out text, out commonObjectSerializationMember);
										string name = text;
										CommonObjectSerializationMember member = commonObjectSerializationMember;
										bool flag2 = i != 0;
										if (flag2)
										{
											code.WriteLine(',');
										}
										string keyData;
										CommonObjectSerializer.SerializeAsJsonString(name, out keyData, code.Indent);
										string subData;
										CommonObjectSerializer.SerializeAsJsonString(member.Getter(), out subData, code.Indent);
										code.Write(keyData + ": ");
										code.Write(subData);
										i++;
									}
									bool flag3 = i != 0;
									if (flag3)
									{
										code.WriteLine();
									}
									code.Indent--;
									code.Write('}');
								}
								else
								{
									code.Write(objConvertible.ToString(CultureInfo.InvariantCulture));
								}
							}
							else
							{
								code.Write('"');
								string text2 = objString;
								int l = 0;
								while (l < text2.Length)
								{
									char c = text2[l];
									char c2 = c;
									char c3 = c2;
									switch (c3)
									{
									case '\b':
										code.Write("\\b");
										break;
									case '\t':
										code.Write("\\t");
										break;
									case '\n':
										code.Write("\\n");
										break;
									case '\v':
										goto IL_346;
									case '\f':
										code.Write("\\f");
										break;
									case '\r':
										code.Write("\\r");
										break;
									default:
										if (c3 != '"')
										{
											if (c3 != '\\')
											{
												goto IL_346;
											}
											code.Write("\\\\");
										}
										else
										{
											code.Write("\\\"");
										}
										break;
									}
									IL_37B:
									l++;
									continue;
									IL_346:
									bool flag4 = c < ' ';
									if (flag4)
									{
										code.Write(string.Format("\\u{0:X4}", (int)c));
									}
									else
									{
										code.Write(c);
									}
									goto IL_37B;
								}
								code.Write('"');
							}
						}
						else
						{
							code.Write('[');
							code.Indent++;
							int j = 0;
							foreach (object item in objCollection)
							{
								string subData2;
								CommonObjectSerializer.SerializeAsJsonString(item, out subData2, code.Indent);
								code.Write(subData2);
								code.Write((j != objCollection.Count - 1) ? ", " : string.Empty);
								j++;
							}
							code.Indent--;
							bool flag5 = indent == 0;
							if (flag5)
							{
								code.WriteLine();
							}
							code.Write(']');
						}
					}
					else
					{
						bool flag6 = indent > 0;
						if (flag6)
						{
							code.WriteLine();
						}
						code.WriteLine('{');
						code.Indent++;
						int k = 0;
						foreach (object key in objDict.Keys)
						{
							string keyData2;
							CommonObjectSerializer.SerializeAsJsonString(key, out keyData2, code.Indent);
							string valueData;
							CommonObjectSerializer.SerializeAsJsonString(objDict[key], out valueData, code.Indent);
							code.Write(keyData2 + ": ");
							code.Write(valueData);
							code.WriteLine((k != objDict.Count - 1) ? ',' : string.Empty);
							k++;
						}
						code.Indent--;
						code.Write('}');
					}
				}
			}
			else
			{
				code.Write("null");
			}
			jsonString = sw.ToString();
		}

		// Token: 0x0600B959 RID: 47449 RVA: 0x00547B8C File Offset: 0x00545D8C
		private static void DeserializeFromLuaValue(DynValue luaValue, out object obj, Type typeHint)
		{
			switch (luaValue.Type)
			{
			case DataType.Nil:
				obj = null;
				return;
			case DataType.Boolean:
				obj = ((IConvertible)luaValue.Boolean).ToType(typeHint, CultureInfo.InvariantCulture);
				return;
			case DataType.Number:
				obj = (typeHint.IsEnum ? CommonObjectSerializer.SafeEnumValue((int)luaValue.Number, typeHint) : ((IConvertible)luaValue.Number).ToType(typeHint, CultureInfo.InvariantCulture));
				return;
			case DataType.String:
			{
				string str = luaValue.String;
				object obj2;
				if (!string.IsNullOrEmpty(str) || !typeHint.IsValueType)
				{
					string text = str;
					obj2 = ((text != null) ? ((IConvertible)text).ToType(typeHint, CultureInfo.InvariantCulture) : null);
				}
				else
				{
					obj2 = Activator.CreateInstance(typeHint);
				}
				obj = obj2;
				return;
			}
			case DataType.Table:
			{
				Table luaTable = luaValue.Table;
				bool isArray = typeHint.IsArray;
				if (isArray)
				{
					Type elementType = typeHint.GetElementType() ?? typeof(object);
					int size = (from k in luaTable.Keys
					select (int)k.Number).Prepend(-1).Max() + 1;
					Array objArray = Array.CreateInstance(elementType, size);
					bool noZeroIndex = false;
					int i = 0;
					int len = objArray.Length;
					while (i < len)
					{
						DynValue key = luaTable.RawGet(DynValue.NewNumber((double)i));
						bool flag = key == null && i == 0;
						if (flag)
						{
							noZeroIndex = true;
						}
						else
						{
							object element;
							CommonObjectSerializer.DeserializeFromLuaValue(key ?? DynValue.Nil, out element, elementType);
							objArray.SetValue(element, i);
						}
						i++;
					}
					bool flag2 = noZeroIndex && objArray.Length > 1;
					if (flag2)
					{
						Array raw = objArray;
						objArray = Array.CreateInstance(elementType, size - 1);
						Array.Copy(raw, 1, objArray, 0, objArray.Length);
					}
					obj = objArray;
				}
				else
				{
					bool flag3 = typeof(ITuple).IsAssignableFrom(typeHint);
					if (flag3)
					{
						bool flag4 = typeHint.IsGenericType && (typeHint.GetGenericTypeDefinition() == typeof(ValueTuple<>) || typeHint.GetGenericTypeDefinition() == typeof(ValueTuple<, >) || typeHint.GetGenericTypeDefinition() == typeof(ValueTuple<, , >) || typeHint.GetGenericTypeDefinition() == typeof(ValueTuple<, , , >) || typeHint.GetGenericTypeDefinition() == typeof(ValueTuple<, , , , >) || typeHint.GetGenericTypeDefinition() == typeof(ValueTuple<, , , , , >) || typeHint.GetGenericTypeDefinition() == typeof(ValueTuple<, , , , , , >) || typeHint.GetGenericTypeDefinition() == typeof(ValueTuple<, , , , , , , >));
						if (!flag4)
						{
							throw new NotImplementedException(string.Format("{0} is not supported", typeHint));
						}
						ITuple tuple = (ITuple)Activator.CreateInstance(typeHint);
						Type[] types = typeHint.GetGenericArguments();
						int j = 0;
						int len2 = tuple.Length;
						while (j < len2)
						{
							int tableIndex = j + 1;
							string tableIndexName = string.Format("Item{0}", tableIndex);
							DynValue v = luaTable.RawGet(DynValue.NewNumber((double)tableIndex)) ?? luaTable.RawGet(DynValue.NewString(tableIndexName));
							bool flag5 = v == null;
							if (!flag5)
							{
								object item;
								CommonObjectSerializer.DeserializeFromLuaValue(v, out item, types[j]);
								FieldInfo field = typeHint.GetField(tableIndexName);
								if (field != null)
								{
									field.SetValue(tuple, item);
								}
							}
							j++;
						}
						obj = tuple;
					}
					else
					{
						bool flag6 = typeof(IDictionary).IsAssignableFrom(typeHint);
						if (flag6)
						{
							Type keyType = typeHint.GetGenericArguments()[0];
							Type valueType = typeHint.GetGenericArguments()[1];
							IDictionary objDict = (IDictionary)Activator.CreateInstance(typeHint);
							foreach (DynValue key2 in luaTable.Keys)
							{
								DynValue value = luaTable.Get(key2);
								object keyData;
								CommonObjectSerializer.DeserializeFromLuaValue(key2, out keyData, keyType);
								object valueData;
								CommonObjectSerializer.DeserializeFromLuaValue(value, out valueData, (value.Type == DataType.Table && valueType == typeof(object)) ? typeof(Dictionary<object, object>) : valueType);
								objDict[keyData] = valueData;
							}
							obj = objDict;
						}
						else
						{
							bool flag7 = typeof(IList).IsAssignableFrom(typeHint);
							if (flag7)
							{
								Type elementType2 = typeHint.GetElementType() ?? typeHint.GetGenericArguments()[0];
								int size2 = (from k in luaTable.Keys
								select (int)k.Number).Prepend(-1).Max() + 1;
								IList objList = (IList)Activator.CreateInstance(typeHint);
								for (int l = 0; l < size2; l++)
								{
									DynValue key3 = luaTable.RawGet(DynValue.NewNumber((double)l));
									bool flag8 = key3 == null && l == 0;
									if (!flag8)
									{
										object element2;
										CommonObjectSerializer.DeserializeFromLuaValue(key3 ?? DynValue.Nil, out element2, elementType2);
										objList.Add(element2);
									}
								}
								obj = objList;
							}
							else
							{
								obj = (typeHint.GetConstructors(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic).Any((ConstructorInfo c) => c.GetParameters().Length == 0) ? Activator.CreateInstance(typeHint, true) : FormatterServices.GetUninitializedObject(typeHint));
								ICommonObjectSerializationAware aware = obj as ICommonObjectSerializationAware;
								bool flag9 = aware != null;
								if (flag9)
								{
									aware.InitializeOnDeserializing();
								}
								foreach (KeyValuePair<string, CommonObjectSerializationMember> keyValuePair in CommonObjectSerializer.GetMembers(obj, true))
								{
									string text2;
									CommonObjectSerializationMember commonObjectSerializationMember;
									keyValuePair.Deconstruct(out text2, out commonObjectSerializationMember);
									string name = text2;
									CommonObjectSerializationMember member = commonObjectSerializationMember;
									DynValue value2 = luaTable.RawGet(name);
									bool flag10 = value2 == null || value2 == DynValue.Nil;
									if (flag10)
									{
										ICommonObjectSerializationAware aware2 = obj as ICommonObjectSerializationAware;
										bool flag11 = aware2 != null;
										if (flag11)
										{
											aware2.DeserializingMissingField(member);
										}
									}
									else
									{
										object prop;
										CommonObjectSerializer.DeserializeFromLuaValue(value2, out prop, member.TypeHint);
										member.Setter(prop);
										luaTable.Remove(name);
									}
								}
								ICommonObjectSerializationAware aware3 = obj as ICommonObjectSerializationAware;
								bool flag12 = aware3 != null;
								if (flag12)
								{
									foreach (DynValue key4 in luaTable.Keys)
									{
										bool originalString = key4.Type == DataType.String;
										string name2 = originalString ? key4.String : key4.CastToString();
										CommonObjectSerializationMember member2;
										bool flag13 = !aware3.DeserializingUnknownField(name2, out member2);
										if (!flag13)
										{
											Table table = luaTable;
											object key5;
											if (!originalString)
											{
												IConvertible conv = key4.ToObject() as IConvertible;
												key5 = ((conv != null) ? ((IConvertible)name2).ToType(conv.GetType(), CultureInfo.InvariantCulture) : name2);
											}
											else
											{
												key5 = name2;
											}
											DynValue value3 = table.RawGet(key5);
											bool flag14 = value3 == null;
											if (!flag14)
											{
												object prop2;
												CommonObjectSerializer.DeserializeFromLuaValue(value3, out prop2, member2.TypeHint);
												ICommonObjectDeserializationDirectValue deserializationDirectValue = aware3 as ICommonObjectDeserializationDirectValue;
												bool flag15 = deserializationDirectValue != null;
												if (flag15)
												{
													deserializationDirectValue.OnUnknownFieldGet(name2, prop2);
												}
												else
												{
													member2.Setter(prop2);
												}
											}
										}
									}
									aware3.FinishedDeserialization();
								}
							}
						}
					}
				}
				return;
			}
			case DataType.Tuple:
				CommonObjectSerializer.DeserializeFromLuaValue(DynValue.NewTable(null, luaValue.Tuple), out obj, typeHint);
				return;
			}
			throw new ArgumentOutOfRangeException("Type", luaValue.Type.ToString(), null);
		}

		// Token: 0x0600B95A RID: 47450 RVA: 0x00548384 File Offset: 0x00546584
		private static void DeserializeFromJsonValue(JToken jsonValue, out object obj, Type typeHint)
		{
			if (jsonValue != null)
			{
				JValue jValue = jsonValue as JValue;
				if (jValue == null)
				{
					JObject jObject = jsonValue as JObject;
					if (jObject == null)
					{
						JArray jArray = jsonValue as JArray;
						if (jArray == null)
						{
							throw new ArgumentOutOfRangeException("Type", jsonValue.Type.ToString(), null);
						}
						bool isArray = typeHint.IsArray;
						if (isArray)
						{
							Type elementType = typeHint.GetElementType() ?? typeof(object);
							Array objArray = Array.CreateInstance(elementType, jArray.Count);
							int i = 0;
							int len = objArray.Length;
							while (i < len)
							{
								object element;
								CommonObjectSerializer.DeserializeFromJsonValue(jArray[i], out element, elementType);
								objArray.SetValue(element, i);
								i++;
							}
							obj = objArray;
						}
						else
						{
							bool flag = typeof(IList).IsAssignableFrom(typeHint);
							if (!flag)
							{
								throw new InvalidCastException();
							}
							Type elementType2 = typeHint.GetElementType() ?? typeHint.GetGenericArguments()[0];
							IList objList = (IList)Activator.CreateInstance(typeHint);
							foreach (JToken t in jArray)
							{
								object element2;
								CommonObjectSerializer.DeserializeFromJsonValue(t, out element2, elementType2);
								objList.Add(element2);
							}
							obj = objList;
						}
					}
					else
					{
						bool flag2 = typeof(IDictionary).IsAssignableFrom(typeHint);
						if (flag2)
						{
							Type valueType = typeHint.GetGenericArguments()[1];
							IDictionary objDict = (IDictionary)Activator.CreateInstance(typeHint);
							foreach (KeyValuePair<string, JToken> keyValuePair in jObject)
							{
								string text;
								JToken jtoken;
								keyValuePair.Deconstruct(out text, out jtoken);
								string key = text;
								JToken value = jtoken;
								object valueData;
								CommonObjectSerializer.DeserializeFromJsonValue(value, out valueData, (value is JObject && valueType == typeof(object)) ? typeof(Dictionary<object, object>) : valueType);
								objDict[key] = valueData;
							}
							obj = objDict;
						}
						else
						{
							obj = (typeHint.GetConstructors(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic).Any((ConstructorInfo c) => c.GetParameters().Length == 0) ? Activator.CreateInstance(typeHint, true) : FormatterServices.GetUninitializedObject(typeHint));
							ICommonObjectSerializationAware aware = obj as ICommonObjectSerializationAware;
							bool flag3 = aware != null;
							if (flag3)
							{
								aware.InitializeOnDeserializing();
							}
							foreach (KeyValuePair<string, CommonObjectSerializationMember> keyValuePair2 in CommonObjectSerializer.GetMembers(obj, true))
							{
								string text;
								CommonObjectSerializationMember commonObjectSerializationMember;
								keyValuePair2.Deconstruct(out text, out commonObjectSerializationMember);
								string name = text;
								CommonObjectSerializationMember member = commonObjectSerializationMember;
								JToken value2;
								bool flag4 = !jObject.TryGetValue(name, out value2);
								if (flag4)
								{
									ICommonObjectSerializationAware aware2 = obj as ICommonObjectSerializationAware;
									bool flag5 = aware2 != null;
									if (flag5)
									{
										aware2.DeserializingMissingField(member);
									}
								}
								else
								{
									object prop;
									CommonObjectSerializer.DeserializeFromJsonValue(value2, out prop, member.TypeHint);
									member.Setter(prop);
									jObject.Remove(name);
								}
							}
							ICommonObjectSerializationAware aware3 = obj as ICommonObjectSerializationAware;
							bool flag6 = aware3 != null;
							if (flag6)
							{
								foreach (KeyValuePair<string, JToken> keyValuePair in jObject)
								{
									string text;
									JToken jtoken;
									keyValuePair.Deconstruct(out text, out jtoken);
									string name2 = text;
									JToken value3 = jtoken;
									CommonObjectSerializationMember member2;
									bool flag7 = !aware3.DeserializingUnknownField(name2, out member2);
									if (!flag7)
									{
										object prop2;
										CommonObjectSerializer.DeserializeFromJsonValue(value3, out prop2, member2.TypeHint);
										ICommonObjectDeserializationDirectValue deserializationDirectValue = aware3 as ICommonObjectDeserializationDirectValue;
										bool flag8 = deserializationDirectValue != null;
										if (flag8)
										{
											deserializationDirectValue.OnUnknownFieldGet(name2, prop2);
										}
										else
										{
											member2.Setter(prop2);
										}
									}
								}
								aware3.FinishedDeserialization();
							}
						}
					}
				}
				else
				{
					string str;
					bool flag9;
					if (typeHint.IsEnum)
					{
						str = (jValue.Value as string);
						flag9 = (str != null);
					}
					else
					{
						flag9 = false;
					}
					bool flag10 = flag9;
					if (flag10)
					{
						obj = Enum.Parse(typeHint, str);
					}
					else
					{
						IConvertible convertible = (IConvertible)jValue.Value;
						obj = ((convertible != null) ? convertible.ToType(typeHint, CultureInfo.InvariantCulture) : null);
					}
				}
			}
			else
			{
				obj = null;
			}
		}

		// Token: 0x0600B95B RID: 47451 RVA: 0x0054880C File Offset: 0x00546A0C
		private static Enum SafeEnumValue(int value, Type enumType)
		{
			Array enums = Enum.GetValues(enumType);
			foreach (object obj in enums)
			{
				Enum e = (Enum)obj;
				bool flag = e.ToInt() == value;
				if (flag)
				{
					return e;
				}
			}
			AdaptableLog.TagInfo("SafeEnumValue", string.Format("{0} is not a valid {1}", value, enumType.Name));
			return (enums.Length > 0) ? (enums.GetValue(0) as Enum) : null;
		}

		// Token: 0x04008F80 RID: 36736
		private static readonly Dictionary<Type, Dictionary<string, MemberInfo>> CachedMemberDict = new Dictionary<Type, Dictionary<string, MemberInfo>>();

		// Token: 0x0200261F RID: 9759
		public enum MarshalFormat
		{
			// Token: 0x0400E9AD RID: 59821
			Lua,
			// Token: 0x0400E9AE RID: 59822
			LuaWithReturnPrefix,
			// Token: 0x0400E9AF RID: 59823
			Json
		}

		// Token: 0x02002620 RID: 9760
		private sealed class CodeWriter : TextWriter
		{
			// Token: 0x06011ADF RID: 72415 RVA: 0x00686283 File Offset: 0x00684483
			public CodeWriter(TextWriter writer, string tabString = "\t") : base(CultureInfo.InvariantCulture)
			{
				this.InnerWriter = writer;
				this._tabString = tabString;
				this._indentLevel = 0;
				this._tabsPending = false;
			}

			// Token: 0x17001BA0 RID: 7072
			// (get) Token: 0x06011AE0 RID: 72416 RVA: 0x006862AE File Offset: 0x006844AE
			public override Encoding Encoding
			{
				get
				{
					return this.InnerWriter.Encoding;
				}
			}

			// Token: 0x17001BA1 RID: 7073
			// (get) Token: 0x06011AE1 RID: 72417 RVA: 0x006862BB File Offset: 0x006844BB
			// (set) Token: 0x06011AE2 RID: 72418 RVA: 0x006862C8 File Offset: 0x006844C8
			public override string NewLine
			{
				get
				{
					return this.InnerWriter.NewLine;
				}
				set
				{
					this.InnerWriter.NewLine = value;
				}
			}

			// Token: 0x17001BA2 RID: 7074
			// (get) Token: 0x06011AE3 RID: 72419 RVA: 0x006862D7 File Offset: 0x006844D7
			// (set) Token: 0x06011AE4 RID: 72420 RVA: 0x006862E0 File Offset: 0x006844E0
			public int Indent
			{
				get
				{
					return this._indentLevel;
				}
				set
				{
					bool flag = value < 0;
					if (flag)
					{
						value = 0;
					}
					this._indentLevel = value;
				}
			}

			// Token: 0x17001BA3 RID: 7075
			// (get) Token: 0x06011AE5 RID: 72421 RVA: 0x00686300 File Offset: 0x00684500
			private TextWriter InnerWriter { get; }

			// Token: 0x06011AE6 RID: 72422 RVA: 0x00686308 File Offset: 0x00684508
			public override void Close()
			{
				this.InnerWriter.Close();
			}

			// Token: 0x06011AE7 RID: 72423 RVA: 0x00686316 File Offset: 0x00684516
			public override void Flush()
			{
				this.InnerWriter.Flush();
			}

			// Token: 0x06011AE8 RID: 72424 RVA: 0x00686324 File Offset: 0x00684524
			private void OutputTabs()
			{
				bool flag = !this._tabsPending;
				if (!flag)
				{
					for (int index = 0; index < this._indentLevel; index++)
					{
						this.InnerWriter.Write(this._tabString);
					}
					this._tabsPending = false;
				}
			}

			// Token: 0x06011AE9 RID: 72425 RVA: 0x0068636F File Offset: 0x0068456F
			public override void Write(string s)
			{
				this.OutputTabs();
				this.InnerWriter.Write(s);
			}

			// Token: 0x06011AEA RID: 72426 RVA: 0x00686386 File Offset: 0x00684586
			public override void Write(bool value)
			{
				this.OutputTabs();
				this.InnerWriter.Write(value);
			}

			// Token: 0x06011AEB RID: 72427 RVA: 0x0068639D File Offset: 0x0068459D
			public override void Write(char value)
			{
				this.OutputTabs();
				this.InnerWriter.Write(value);
			}

			// Token: 0x06011AEC RID: 72428 RVA: 0x006863B4 File Offset: 0x006845B4
			public override void Write(char[] buffer)
			{
				this.OutputTabs();
				this.InnerWriter.Write(buffer);
			}

			// Token: 0x06011AED RID: 72429 RVA: 0x006863CB File Offset: 0x006845CB
			public override void Write(char[] buffer, int index, int count)
			{
				this.OutputTabs();
				this.InnerWriter.Write(buffer, index, count);
			}

			// Token: 0x06011AEE RID: 72430 RVA: 0x006863E4 File Offset: 0x006845E4
			public override void Write(double value)
			{
				this.OutputTabs();
				this.InnerWriter.Write(value);
			}

			// Token: 0x06011AEF RID: 72431 RVA: 0x006863FB File Offset: 0x006845FB
			public override void Write(float value)
			{
				this.OutputTabs();
				this.InnerWriter.Write(value);
			}

			// Token: 0x06011AF0 RID: 72432 RVA: 0x00686412 File Offset: 0x00684612
			public override void Write(int value)
			{
				this.OutputTabs();
				this.InnerWriter.Write(value);
			}

			// Token: 0x06011AF1 RID: 72433 RVA: 0x00686429 File Offset: 0x00684629
			public override void Write(long value)
			{
				this.OutputTabs();
				this.InnerWriter.Write(value);
			}

			// Token: 0x06011AF2 RID: 72434 RVA: 0x00686440 File Offset: 0x00684640
			public override void Write(object value)
			{
				this.OutputTabs();
				this.InnerWriter.Write(value);
			}

			// Token: 0x06011AF3 RID: 72435 RVA: 0x00686457 File Offset: 0x00684657
			public override void Write(string format, object arg0)
			{
				this.OutputTabs();
				this.InnerWriter.Write(format, arg0);
			}

			// Token: 0x06011AF4 RID: 72436 RVA: 0x0068646F File Offset: 0x0068466F
			public override void Write(string format, object arg0, object arg1)
			{
				this.OutputTabs();
				this.InnerWriter.Write(format, arg0, arg1);
			}

			// Token: 0x06011AF5 RID: 72437 RVA: 0x00686488 File Offset: 0x00684688
			public override void Write(string format, params object[] arg)
			{
				this.OutputTabs();
				this.InnerWriter.Write(format, arg);
			}

			// Token: 0x06011AF6 RID: 72438 RVA: 0x006864A0 File Offset: 0x006846A0
			public override void WriteLine(string s)
			{
				this.OutputTabs();
				this.InnerWriter.WriteLine(s);
				this._tabsPending = true;
			}

			// Token: 0x06011AF7 RID: 72439 RVA: 0x006864BE File Offset: 0x006846BE
			public override void WriteLine()
			{
				this.OutputTabs();
				this.InnerWriter.WriteLine();
				this._tabsPending = true;
			}

			// Token: 0x06011AF8 RID: 72440 RVA: 0x006864DB File Offset: 0x006846DB
			public override void WriteLine(bool value)
			{
				this.OutputTabs();
				this.InnerWriter.WriteLine(value);
				this._tabsPending = true;
			}

			// Token: 0x06011AF9 RID: 72441 RVA: 0x006864F9 File Offset: 0x006846F9
			public override void WriteLine(char value)
			{
				this.OutputTabs();
				this.InnerWriter.WriteLine(value);
				this._tabsPending = true;
			}

			// Token: 0x06011AFA RID: 72442 RVA: 0x00686517 File Offset: 0x00684717
			public override void WriteLine(char[] buffer)
			{
				this.OutputTabs();
				this.InnerWriter.WriteLine(buffer);
				this._tabsPending = true;
			}

			// Token: 0x06011AFB RID: 72443 RVA: 0x00686535 File Offset: 0x00684735
			public override void WriteLine(char[] buffer, int index, int count)
			{
				this.OutputTabs();
				this.InnerWriter.WriteLine(buffer, index, count);
				this._tabsPending = true;
			}

			// Token: 0x06011AFC RID: 72444 RVA: 0x00686555 File Offset: 0x00684755
			public override void WriteLine(double value)
			{
				this.OutputTabs();
				this.InnerWriter.WriteLine(value);
				this._tabsPending = true;
			}

			// Token: 0x06011AFD RID: 72445 RVA: 0x00686573 File Offset: 0x00684773
			public override void WriteLine(float value)
			{
				this.OutputTabs();
				this.InnerWriter.WriteLine(value);
				this._tabsPending = true;
			}

			// Token: 0x06011AFE RID: 72446 RVA: 0x00686591 File Offset: 0x00684791
			public override void WriteLine(int value)
			{
				this.OutputTabs();
				this.InnerWriter.WriteLine(value);
				this._tabsPending = true;
			}

			// Token: 0x06011AFF RID: 72447 RVA: 0x006865AF File Offset: 0x006847AF
			public override void WriteLine(long value)
			{
				this.OutputTabs();
				this.InnerWriter.WriteLine(value);
				this._tabsPending = true;
			}

			// Token: 0x06011B00 RID: 72448 RVA: 0x006865CD File Offset: 0x006847CD
			public override void WriteLine(object value)
			{
				this.OutputTabs();
				this.InnerWriter.WriteLine(value);
				this._tabsPending = true;
			}

			// Token: 0x06011B01 RID: 72449 RVA: 0x006865EB File Offset: 0x006847EB
			public override void WriteLine(string format, object arg0)
			{
				this.OutputTabs();
				this.InnerWriter.WriteLine(format, arg0);
				this._tabsPending = true;
			}

			// Token: 0x06011B02 RID: 72450 RVA: 0x0068660A File Offset: 0x0068480A
			public override void WriteLine(string format, object arg0, object arg1)
			{
				this.OutputTabs();
				this.InnerWriter.WriteLine(format, arg0, arg1);
				this._tabsPending = true;
			}

			// Token: 0x06011B03 RID: 72451 RVA: 0x0068662A File Offset: 0x0068482A
			public override void WriteLine(string format, params object[] arg)
			{
				this.OutputTabs();
				this.InnerWriter.WriteLine(format, arg);
				this._tabsPending = true;
			}

			// Token: 0x06011B04 RID: 72452 RVA: 0x00686649 File Offset: 0x00684849
			[CLSCompliant(false)]
			public override void WriteLine(uint value)
			{
				this.OutputTabs();
				this.InnerWriter.WriteLine(value);
				this._tabsPending = true;
			}

			// Token: 0x0400E9B0 RID: 59824
			private int _indentLevel;

			// Token: 0x0400E9B1 RID: 59825
			private bool _tabsPending;

			// Token: 0x0400E9B2 RID: 59826
			private readonly string _tabString;
		}
	}
}
