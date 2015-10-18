using System;
using System.Collections.Generic;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Extentions
{
    public static class CommonExtensions
    {
        #region Date and Time
        /// <summary>
        /// Shamsi Date  . Sample: 1393/06/31
        /// </summary>
        /// <returns></returns>
        public static string GetSimpleCurrentDate()
        {
            var currentDate = FarsiLibrary.Utils.PersianDate.Now.ToString(FarsiLibrary.Utils.PersianDateTimeFormatInfo.ShortDatePattern);
            return currentDate;
        }

        public static string GetDetailsCurrentDate()
        {
            var currentDate = string.Empty;

            currentDate += FarsiLibrary.Utils.PersianDateTimeFormatInfo.GetWeekDay(DateTime.Now.DayOfWeek) + " ";
            currentDate += FarsiLibrary.Utils.PersianDate.Now.Day + " ";
            currentDate += FarsiLibrary.Utils.PersianDate.Now.LocalizedMonthName + " ";
            return currentDate += FarsiLibrary.Utils.PersianDate.Now.Year.ToString(CultureInfo.InvariantCulture);
        }

        public static string GetCurrentTime()
        {
            string currentTime = FarsiLibrary.Utils.PersianDate.Now.Time.ToString();

            return currentTime;
        }

        public static bool IsValidDate(string date)
        {
            var pattern = new Regex("^\\d{4}/\\d{2}/\\d{2}$");
            var arrPattern = new[]
            {
                new Regex("^\\d{4}/\\d{2}/\\d{2}$"),
                new Regex("^\\d{4}/\\d{2}/\\d{1}$"),
                new Regex("^\\d{4}/\\d{1}/\\d{2}$"),
                new Regex("^\\d{4}/\\d{1}/\\d{1}$"),
                new Regex("^\\d{2}/\\d{2}/\\d{2}$"),
                new Regex("^\\d{2}/\\d{2}/\\d{1}$"),
                new Regex("^\\d{2}/\\d{1}/\\d{2}$"),
                new Regex("^\\d{2}/\\d{1}/\\d{1}")
            };
            const int kabise = 1387;
            var year = 0;
            var mounth = 0;
            var day = 0;
            var flag = false;
            foreach (var t in arrPattern)
            {
                if (t.IsMatch(date))
                    flag = true;
            }
            if (flag == false) return false;


            var splitDate = date.Split('/', '-', ':');
            year = Convert.ToInt32(splitDate[0]);
            mounth = Convert.ToInt32(splitDate[1]);
            day = Convert.ToInt32(splitDate[2]);
            if (mounth > 12 || mounth <= 0)
                flag = false;
            else
            {
                if (mounth < 7)
                {
                    if (day > 31)
                    {
                        flag = false;
                    }
                }
                if (mounth == 12)
                {
                    var t = (year - kabise) % 4;

                    if ((year - kabise) % 4 == 0)
                    {
                        if (day >= 31)
                            flag = false;
                    }
                }
                if (mounth > 6 && mounth < 12)
                {
                    if (day > 30)
                        flag = false;
                }
            }
            return flag;
        }

        public static bool IsValidTime(string time)
        {
            //var pattern = new Regex("^(?:[01]?[0-9]|2[0-3]):[0-5][0-9]$");

            return true;

        }
        #endregion

        #region Regix Operations

        #endregion

        #region String Operations

        public static string SplitString(string statement, char[] splitCharacters)
        {
            var result = string.Empty;

            var dogits = statement.Split(splitCharacters);

            foreach (var d in dogits)
                result = result + d;

            return result;
        }

        //حذف اعراب از حروف و کلمات
        public static string RemoveDiacritics(string text)
        {
            var normalizedString = text.Normalize(NormalizationForm.FormD);
            var stringBuilder = new StringBuilder();

            foreach (var c in normalizedString)
            {
                var unicodeCategory = CharUnicodeInfo.GetUnicodeCategory(c);
                if (unicodeCategory != UnicodeCategory.NonSpacingMark)
                {
                    stringBuilder.Append(c);
                }
            }

            return stringBuilder.ToString().Normalize(NormalizationForm.FormC);
        }
        #endregion

        #region Data Type Convertion
        // transform object into Identity data type (integer).
        public static int AsId(this object item, int defaultId = -1)
        {
            if (item == null)
                return defaultId;

            int result;
            return !int.TryParse(item.ToString(), out result) ? defaultId : result;
        }

        // transform object into short data type.
        public static short AsShort(this object item, short defaultShort = default(short))
        {
            if (item == null)
                return defaultShort;

            short result;
            return !short.TryParse(item.ToString(), out result) ? defaultShort : result;
        }

        // transform object into byte data type.
        public static byte AsByte(this object item, byte defaultByte = default(byte))
        {
            if (item == null)
                return defaultByte;

            byte result;
            return !byte.TryParse(item.ToString(), out result) ? defaultByte : result;
        }

        // transform object into integer data type.
        public static int AsInt(this object item, int defaultInt = default(int))
        {
            if (item == null)
                return defaultInt;

            int result;
            return !int.TryParse(item.ToString(), out result) ? defaultInt : result;
        }

        // transform object into double data type
        public static double AsDouble(this object item, double defaultDouble = default(double))
        {
            if (item == null) return defaultDouble;
            double result;
            return !double.TryParse(item.ToString(), out result) ? defaultDouble : result;
        }

        // transform object into string data type
        public static string AsString(this object item, string defaultString = default(string))
        {
            if (item == null || item.Equals(DBNull.Value))
                return defaultString;

            return item.ToString().Trim();
        }

        // transform object into DateTime data type.
        public static DateTime AsDateTime(this object item, DateTime defaultDateTime = default(DateTime))
        {
            if (item == null || string.IsNullOrEmpty(item.ToString()))
                return defaultDateTime;

            DateTime result;
            return !DateTime.TryParse(item.ToString(), out result) ? defaultDateTime : result;
        }

        // transform object into bool data type
        public static bool AsBool(this object item, bool defaultBool = default(bool))
        {
            return item == null ? defaultBool : new List<string> { "yes", "y", "true" }.Contains(item.ToString().ToLower());
        }

        // transform string into Base64 Byte array
        public static byte[] AsByteArrayBase64(this string str)
        {
            return string.IsNullOrEmpty(str) ? null : Convert.FromBase64String(str);
        }

        // transform string into byte array
        public static byte[] AsBytes(this string str)
        {
            var bytes = new byte[str.Length * sizeof(char)];
            Buffer.BlockCopy(str.ToCharArray(), 0, bytes, 0, bytes.Length);
            return bytes;
        }

        // transform string into byte array
        public static string GetString(this byte[] bytes)
        {
            var chars = new char[bytes.Length / sizeof(char)];
            Buffer.BlockCopy(bytes, 0, chars, 0, bytes.Length);
            return new string(chars);
        }

        // transform object into base64 string.
        public static string AsBase64String(this object item)
        {
            return item == null ? null : Convert.ToBase64String((byte[])item);
        }

        // transform object into Guid data type
        public static Guid AsGuid(this object item)
        {
            try { return new Guid(item.ToString()); }
            catch { return Guid.Empty; }
        }

        // transform byte[] into Image data type
        public static Image AsImage(this byte[] byteArray)
        {
            var oMemoryStream = new MemoryStream(byteArray);
            var oImage = Image.FromStream(oMemoryStream);

            return oImage;
        }

        // transform binarry into byte[] data type
        public static byte[] AsArrayByte(this object img)
        {
            var oMemoryStream = new MemoryStream();
            var image = new Bitmap((Image)img);

            image.Save(oMemoryStream, System.Drawing.Imaging.ImageFormat.Jpeg);

            return oMemoryStream.ToArray();
        }
        #endregion
    }
}
