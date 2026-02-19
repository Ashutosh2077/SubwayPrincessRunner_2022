using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Runtime.CompilerServices;
using System.Text;

namespace MiniJSONs
{
	public static class Json
	{
		public static object Deserialize(string json)
		{
			if (json == null)
			{
				return null;
			}
			return Json.Parser.Parse(json);
		}

		public static string Serialize(object obj)
		{
			return Json.Serializer.Serialize(obj);
		}

		private sealed class Parser : IDisposable
		{
			private Parser(string jsonString)
			{
				this.json = new StringReader(jsonString);
			}

			public void Dispose()
			{
				this.json.Dispose();
				this.json = null;
			}

			private void EatWhitespace()
			{
				while (" \t\n\r".IndexOf(this.PeekChar) != -1)
				{
					this.json.Read();
					if (this.json.Peek() == -1)
					{
						break;
					}
				}
			}

			public static object Parse(string jsonString)
			{
				object result;
				using (Json.Parser parser = new Json.Parser(jsonString))
				{
					result = parser.ParseValue();
				}
				return result;
			}

			private List<object> ParseArray()
			{
				List<object> list = new List<object>();
				this.json.Read();
				bool flag = true;
				while (flag)
				{
					Json.Parser.TOKEN nextSymbol = this.NextSymbol;
					if (nextSymbol != Json.Parser.TOKEN.SQUARED_CLOSE)
					{
						if (nextSymbol != Json.Parser.TOKEN.COMMA)
						{
							if (nextSymbol == Json.Parser.TOKEN.NONE)
							{
								return null;
							}
							object item = this.ParseBySymbol(nextSymbol);
							list.Add(item);
						}
					}
					else
					{
						flag = false;
					}
				}
				return list;
			}

			private object ParseBySymbol(Json.Parser.TOKEN symbol)
			{
				switch (symbol)
				{
				case Json.Parser.TOKEN.STRING:
					return this.ParseString();
				case Json.Parser.TOKEN.NUMBER:
					return this.ParseNumber();
				case Json.Parser.TOKEN.TRUE:
					return true;
				case Json.Parser.TOKEN.FALSE:
					return false;
				case Json.Parser.TOKEN.NULL:
					return null;
				default:
					switch (symbol)
					{
					case Json.Parser.TOKEN.CURLY_OPEN:
						return this.ParseObject();
					case Json.Parser.TOKEN.SQUARED_OPEN:
						return this.ParseArray();
					}
					return null;
				}
			}

			private object ParseNumber()
			{
				string nextWord = this.NextWord;
				if (nextWord.IndexOf('.') == -1)
				{
					long num;
					long.TryParse(nextWord, out num);
					return num;
				}
				double num2;
				double.TryParse(nextWord, out num2);
				return num2;
			}

			private Dictionary<string, object> ParseObject()
			{
				Dictionary<string, object> dictionary = new Dictionary<string, object>();
				this.json.Read();
				for (;;)
				{
					Json.Parser.TOKEN nextSymbol = this.NextSymbol;
					if (nextSymbol == Json.Parser.TOKEN.NONE)
					{
						break;
					}
					if (nextSymbol == Json.Parser.TOKEN.CURLY_CLOSE)
					{
						return dictionary;
					}
					if (nextSymbol != Json.Parser.TOKEN.COMMA)
					{
						string text = this.ParseString();
						if (text == null)
						{
							goto Block_4;
						}
						if (this.NextSymbol != Json.Parser.TOKEN.COLON)
						{
							goto Block_5;
						}
						this.json.Read();
						dictionary[text] = this.ParseValue();
					}
				}
				return null;
				Block_4:
				return null;
				Block_5:
				return null;
			}

			private string ParseString()
			{
				StringBuilder stringBuilder = new StringBuilder();
				this.json.Read();
				bool flag = true;
				while (flag)
				{
					if (this.json.Peek() == -1)
					{
						break;
					}
					char nextChar = this.NextChar;
					char c = nextChar;
					if (c == '"')
					{
						flag = false;
					}
					else if (c != '\\')
					{
						stringBuilder.Append(nextChar);
					}
					else if (this.json.Peek() == -1)
					{
						flag = false;
					}
					else
					{
						nextChar = this.NextChar;
						char c2 = nextChar;
						switch (c2)
						{
						case 'r':
							stringBuilder.Append('\r');
							break;
						default:
							if (c2 != 'n')
							{
								if (c2 != '"' && c2 != '/' && c2 != '\\')
								{
									if (c2 == 'b')
									{
										stringBuilder.Append('\b');
									}
									else if (c2 == 'f')
									{
										stringBuilder.Append('\f');
									}
								}
								else
								{
									stringBuilder.Append(nextChar);
								}
							}
							else
							{
								stringBuilder.Append('\n');
							}
							break;
						case 't':
							stringBuilder.Append('\t');
							break;
						case 'u':
						{
							StringBuilder stringBuilder2 = new StringBuilder();
							for (int i = 0; i < 4; i++)
							{
								stringBuilder2.Append(this.NextChar);
							}
							stringBuilder.Append((char)Convert.ToInt32(stringBuilder2.ToString(), 16));
							break;
						}
						}
					}
				}
				return stringBuilder.ToString();
			}

			private object ParseValue()
			{
				Json.Parser.TOKEN nextSymbol = this.NextSymbol;
				return this.ParseBySymbol(nextSymbol);
			}

			private char NextChar
			{
				get
				{
					return Convert.ToChar(this.json.Read());
				}
			}

			private Json.Parser.TOKEN NextSymbol
			{
				get
				{
					if (this.json.Peek() != -1)
					{
						this.EatWhitespace();
						char peekChar = this.PeekChar;
						switch (peekChar)
						{
						case ',':
							this.json.Read();
							return Json.Parser.TOKEN.COMMA;
						case '-':
						case '0':
						case '1':
						case '2':
						case '3':
						case '4':
						case '5':
						case '6':
						case '7':
						case '8':
						case '9':
							return Json.Parser.TOKEN.NUMBER;
						default:
							switch (peekChar)
							{
							case '[':
								return Json.Parser.TOKEN.SQUARED_OPEN;
							default:
								switch (peekChar)
								{
								case '{':
									return Json.Parser.TOKEN.CURLY_OPEN;
								default:
								{
									if (peekChar == '"')
									{
										return Json.Parser.TOKEN.STRING;
									}
									string nextWord = this.NextWord;
									if (nextWord != null)
									{
										if (Json.Parser.f__switchmap0 == null)
										{
											Json.Parser.f__switchmap0 = new Dictionary<string, int>(3)
											{
												{
													"false",
													0
												},
												{
													"true",
													1
												},
												{
													"null",
													2
												}
											};
										}
										int num;
										if (Json.Parser.f__switchmap0.TryGetValue(nextWord, out num))
										{
											if (num == 0)
											{
												return Json.Parser.TOKEN.FALSE;
											}
											if (num == 1)
											{
												return Json.Parser.TOKEN.TRUE;
											}
											if (num == 2)
											{
												return Json.Parser.TOKEN.NULL;
											}
										}
									}
									break;
								}
								case '}':
									this.json.Read();
									return Json.Parser.TOKEN.CURLY_CLOSE;
								}
								break;
							case ']':
								this.json.Read();
								return Json.Parser.TOKEN.SQUARED_CLOSE;
							}
							break;
						case ':':
							return Json.Parser.TOKEN.COLON;
						}
					}
					return Json.Parser.TOKEN.NONE;
				}
			}

			private string NextWord
			{
				get
				{
					StringBuilder stringBuilder = new StringBuilder();
					while (" \t\n\r{}[],:\"".IndexOf(this.PeekChar) == -1)
					{
						stringBuilder.Append(this.NextChar);
						if (this.json.Peek() == -1)
						{
							break;
						}
					}
					return stringBuilder.ToString();
				}
			}

			private char PeekChar
			{
				get
				{
					return Convert.ToChar(this.json.Peek());
				}
			}

			[CompilerGenerated]
			private static Dictionary<string, int> f__switchmap0;

			private StringReader json;

			private const string WHITE_SPACE = " \t\n\r";

			private const string WORD_BREAK = " \t\n\r{}[],:\"";

			private enum TOKEN
			{
				NONE,
				CURLY_OPEN,
				CURLY_CLOSE,
				SQUARED_OPEN,
				SQUARED_CLOSE,
				COLON,
				COMMA,
				STRING,
				NUMBER,
				TRUE,
				FALSE,
				NULL
			}
		}

		private sealed class Serializer
		{
			private Serializer()
			{
			}

			public static string Serialize(object obj)
			{
				Json.Serializer serializer = new Json.Serializer();
				serializer.SerializeValue(obj);
				return serializer.builder.ToString();
			}

			private void SerializeArray(IList anArray)
			{
				this.builder.Append('[');
				bool flag = true;
				foreach (object value in anArray)
				{
					if (!flag)
					{
						this.builder.Append(',');
					}
					this.SerializeValue(value);
					flag = false;
				}
				this.builder.Append(']');
			}

			private void SerializeObject(IDictionary obj)
			{
				bool flag = true;
				this.builder.Append('{');
				foreach (object obj2 in obj.Keys)
				{
					if (!flag)
					{
						this.builder.Append(',');
					}
					this.SerializeString(obj2.ToString());
					this.builder.Append(':');
					this.SerializeValue(obj[obj2]);
					flag = false;
				}
				this.builder.Append('}');
			}

			private void SerializeOther(object value)
			{
				if (value is float || value is int || value is uint || value is long || value is double || value is sbyte || value is byte || value is short || value is ushort || value is ulong || value is decimal)
				{
					this.builder.Append(value.ToString());
				}
				else
				{
					this.SerializeString(value.ToString());
				}
			}

			private void SerializeString(string str)
			{
				this.builder.Append('"');
				foreach (char c in str.ToCharArray())
				{
					switch (c)
					{
					case '\b':
						this.builder.Append("\\b");
						break;
					case '\t':
						this.builder.Append("\\t");
						break;
					case '\n':
						this.builder.Append("\\n");
						break;
					default:
						if (c != '"')
						{
							if (c != '\\')
							{
								int num = Convert.ToInt32(c);
								if (num >= 32 && num <= 126)
								{
									this.builder.Append(c);
								}
								else
								{
									this.builder.Append("\\u" + Convert.ToString(num, 16).PadLeft(4, '0'));
								}
							}
							else
							{
								this.builder.Append("\\\\");
							}
						}
						else
						{
							this.builder.Append("\\\"");
						}
						break;
					case '\f':
						this.builder.Append("\\f");
						break;
					case '\r':
						this.builder.Append("\\r");
						break;
					}
				}
				this.builder.Append('"');
			}

			private void SerializeValue(object value)
			{
				if (value == null)
				{
					this.builder.Append("null");
				}
				else
				{
					string text = value as string;
					if (text != null)
					{
						this.SerializeString(text);
					}
					else if (value is bool)
					{
						this.builder.Append(value.ToString().ToLower());
					}
					else
					{
						IList list = value as IList;
						if (list != null)
						{
							this.SerializeArray(list);
						}
						else
						{
							IDictionary dictionary = value as IDictionary;
							if (dictionary != null)
							{
								this.SerializeObject(dictionary);
							}
							else if (value is char)
							{
								this.SerializeString(value.ToString());
							}
							else
							{
								this.SerializeOther(value);
							}
						}
					}
				}
			}

			private StringBuilder builder = new StringBuilder();
		}
	}
}
