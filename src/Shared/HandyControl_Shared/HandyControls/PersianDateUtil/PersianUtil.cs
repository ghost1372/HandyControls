using System;
using System.Collections.Generic;
using HandyControl.Tools.Extension;

namespace HandyControl.Tools
{
	public class ConvertList
	{
		public long Level { get; set; }
		public long Number { get; set; }
		public ConvertList(long level, long number)
		{
			this.Level = level;
			this.Number = number;
        }
	}

	public static class PersianUtil
	{
		private static Dictionary<long, string> numDic = new Dictionary<long, string>()
		{
			{ 0, "" },{ 1, "یک" },{ 2, "دو" },{ 3, "سه" },{ 4, "چهار" },{ 5, "پنج" },
			{ 6, "شش" },{ 7, "هفت" },{ 8, "هشت" },{ 9, "نه" },{ 10, "ده" },
			{ 11, "یازده" },{ 12, "دوازده" },{ 13, "سیزده" },{ 14, "چهارده" },{ 15, "پانزده" },
			{ 16, "شانزده" },{ 17, "هفده" },{ 18, "هجده" },{ 19, "نوزده" },{ 20, "بیست" },
			{ 30, "سی" },{ 40, "چهل" },{ 50, "پنجاه" },{ 60, "شصت" },{ 70, "هفتاد" },
			{ 80, "هشتاد" },{ 90, "نود" },{ 100, "صد" },{ 110, "صد و ده" },{ 120, "صد و بیست" },
			{ 130, "صد و سی" },{ 140, "صد و چهل" },{ 150, "صد و پنجاه" },{ 160, "صد و شصت" },
			{ 170, "صد و هفتاد" },{ 180, "صد و هشتاد" },{ 190, "صد و نود" },{ 200, "دویست" },
			{ 300, "سیصد" },{ 400, "چهارصد" },{ 500, "پانصد" },{ 600, "ششصد" },{ 700, "هفتصد" },
			{ 800, "هشتصد" },{ 900, "نهصد " },{ 1000, "هزار " },{ 1000000, "میلیون " },{ 1000000000, "میلیارد " },

		};
		private static List<ConvertList> ConList = new List<ConvertList>()
		{
			new ConvertList(1000000000,1000000000),
			new ConvertList(1000000,1000000),
			new ConvertList(100000,1000 ),
			new ConvertList(10000,1000 ),
			new ConvertList(1000,1000 ),
			new ConvertList(100,100 ),
			new ConvertList(21,10)
		};

        public static Int32 ConvertToInt(this string num) =>
			System.Convert.ToInt32(num.ConvertToEnglishDigit());
		public static decimal ConvertToDecimal(this string num) =>
			System.Convert.ToDecimal(num.ConvertToEnglishDigit());

		private static long Remaining(long i, long j)
		{
			return (i - (j * Quotient(i, j)));
		}

		public static string Convert(int i)
		{
			if (i == 0)
				return "صفر";
			return ConvertUltraHuge((long)i).Replace("  ", " ");
		}

		public static string Convert(long i)
		{
			if (i == 0)
				return "صفر";
			return ConvertUltraHuge(i).Replace("  ", "");
		}
		public static string Convert2(long number)
		{
			string result = ConvertToText(0, number);

			return result.Replace("  ", "");
		}
		private static string GetNumDicValue(long key)
		{
			string TempValue;
			return numDic.TryGetValue(key, out TempValue) ? TempValue : String.Empty;
		}
		private static string ConvertToText(int id, long number)
		{
			string TempValue = GetNumDicValue(number);


			if (number < ConList[ConList.Count - 1].Level)
			{
				//return ConvertOne(i);
				return TempValue;
			}
			//long lev = number < 20+1 ? 10 : level;
			if (number < ConList[id].Level)
			{
				ConvertToText(id + 1, number);// numDic.GetValueOrDefault(number);
			}
			if (number < 1000)
			{
				return (ConvertToText(id + 1, Quotient(ConList[id].Number * number, ConList[id].Level))
				+ GetNumDicValue(ConList[id].Level)
				+ Space(ConvertToText(id + 1, Remaining(number, ConList[id].Level)))
				+ ConvertToText(id + 1, Remaining(number, ConList[id].Level)));
			}
			return (ConvertToText(id + 1, Quotient(number, ConList[id].Level))
				+ GetNumDicValue(ConList[id].Level)
				+ Space(ConvertToText(id + 1, Remaining(number, ConList[id].Level)))
				+ ConvertToText(id + 1, Remaining(number, ConList[id].Level)));
		}
		private static string ConvertUltraHuge(long i)
		{
			if (i < 1000000000)
			{
				return ConvertHuge(i);
			}
			return (ConvertHuge(Quotient(i, 1000000000))
				+ GetNumDicValue(1000000000)
				+ Space(ConvertHuge(Remaining(i, 1000000000)))
				+ ConvertHuge(Remaining(i, 1000000000)) + " ريال").Replace("  ", "");
		}
		private static string ConvertHuge(long i)
		{
			if (i < 1000000)
			{
				return ConvertBiger(i);
			}
			return (ConvertBiger(Quotient(i, 1000000))
				+ GetNumDicValue(1000000)
				+ Space(ConvertBiger(Remaining(i, 1000000)))
				+ ConvertBiger(Remaining(i, 1000000)));
		}
		private static string ConvertBiger(long i)
		{
			if (i < 100000)
			{
				return ConvertBig(i);
			}
			return (ConvertLarge(Quotient(i, 1000))
				+ GetNumDicValue(1000)
				+ Space(ConvertBig(Remaining(i, 1000)))
				+ ConvertBig(Remaining(i, 1000)));
		}
		private static string ConvertBig(long i)
		{
			if (i < 10000)
			{
				return ConvertLarger(i);
			}
			return (ConvertLarger(Quotient(i, 1000))
				+ GetNumDicValue(1000)
				+ Space(ConvertLarge(Remaining(i, 1000)))
				+ ConvertLarge(Remaining(i, 1000)));
		}
		private static string ConvertLarger(long i)
		{
			if (i < 1000)
			{
				return ConvertLarge(i);
			}
			return (ConvertLarge(Quotient(i, 1000))
				+ GetNumDicValue(1000)
				+ Space(ConvertLarge(Remaining(i, 1000)))
				+ ConvertLarge(Remaining(i, 1000)));
		}
		private static string ConvertLarge(long i)
		{
			if (i < 100)
			{
				return ConvertMedium(i);
			}
			return (//ConvertHundred(100 * Quotient(i, 100))
				 GetNumDicValue(100 * Quotient(i, 100))
				+ Space(ConvertMedium(Remaining(i, 100)))
				+ ConvertMedium(Remaining(i, 100)));
		}
		private static string ConvertMedium(long i)
		{
			if (i < 21)
			{
				//return ConvertOne(i);
				return GetNumDicValue(i);
			}
			//long num = 10 * Quotient(i, 10);
			return (//ConvertTen(num)
				GetNumDicValue(10 * Quotient(i, 10))
				+ Space(GetNumDicValue(Remaining(i, 10)))
				+ GetNumDicValue(Remaining(i, 10)));
		}

		private static long Quotient(long i, long j)
		{
			return (long)(((double)i) / ((double)j));
		}

		private static string Space(string digit)
		{
			return (((digit == "") || (digit == " ")) ? " " : " و ");
		}

	}
}

