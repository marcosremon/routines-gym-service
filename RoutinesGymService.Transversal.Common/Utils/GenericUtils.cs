using RoutinesGymService.Domain.Model.Enums;
using System.Text;
using System.Text.RegularExpressions;

namespace RoutinesGymService.Transversal.Common.Utils
{
    public class GenericUtils
    {
        private readonly CacheUtils _cacheUtils;

        public GenericUtils(CacheUtils cacheUtils)
        {
            _cacheUtils = cacheUtils;
        }

        #region Week day interchanges
        public static string ChangeDayLanguage(string dayName)
        {
            switch (dayName)
            {
                case "Lunes": dayName = "Monday"; break;
                case "Martes": dayName = "Tuesday"; break;
                case "Miércoles": dayName = "Wednesday"; break;
                case "Jueves": dayName = "Thursday"; break;
                case "Viernes": dayName = "Friday"; break;
                case "Sábado": dayName = "Saturday"; break;
                case "Domingo": dayName = "Sunday"; break;
            }

            return dayName;
        }

        public static WeekDay ChangeIntToEnumOnDayName(int day)
        {
            switch (day)
            {
                case 1: return WeekDay.MONDAY; 
                case 2: return WeekDay.TUESDAY; 
                case 3: return WeekDay.WEDNESDAY; 
                case 4: return WeekDay.THURSDAY;
                case 5: return WeekDay.FRIDAY; 
                case 6: return WeekDay.SATURDAY; 
                case 7: return WeekDay.SUNDAY; 
            }

            return WeekDay.MONDAY;
        }

        public static int ChangeEnumToIntOnDayName(WeekDay day)
        {
            switch (day)
            {
                case WeekDay.MONDAY: return 1;
                case WeekDay.TUESDAY: return 2;
                case WeekDay.WEDNESDAY: return 3;
                case WeekDay.THURSDAY: return 4;
                case WeekDay.FRIDAY: return 5;
                case WeekDay.SATURDAY: return 6;
                case WeekDay.SUNDAY: return 7;
            }

            return 0;
        }

        public static WeekDay ChangeStringToEnumOnDayName(string dayName)
        {
            dayName = ChangeDayLanguage(dayName);
            switch (dayName.ToLower())
            {
                case "monday": return WeekDay.MONDAY;
                case "tuesday": return WeekDay.TUESDAY;
                case "wednesday": return WeekDay.WEDNESDAY;
                case "thursday": return WeekDay.THURSDAY;
                case "friday": return WeekDay.FRIDAY;
                case "saturday": return WeekDay.SATURDAY;
                case "sunday": return WeekDay.SUNDAY;
            }

            return WeekDay.MONDAY;
        }
        #endregion

        #region Role interchanges
        public static Role ChangeIntToEnumOnRole(int role)
        {
            switch (role)
            {
                case 0: return Role.USER;
                case 1: return Role.ADMIN;
            }

            return Role.USER;
        }

        public static int ChangeEnumToIntOnRole(Role role)
        {
            switch (role)
            {
                case Role.USER: return 0;
                case Role.ADMIN: return 1;
            }

            return 0;
        }
        #endregion

        #region Is dni valid
        public static bool IsDniValid(string dni)
        {
            return !string.IsNullOrEmpty(dni) && Regex.IsMatch(dni, @"^\d{8}[A-Z]$");
        }
        #endregion

        #region Create friend code
        public static string CreateFriendCode(int length)
        {
            const string valid = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ1234567890";
            StringBuilder res = new StringBuilder();
            Random rnd = new Random();
            while (0 < length--)
            {
                res.Append(valid[rnd.Next(valid.Length)]);
            }
            return res.ToString();
        }
        #endregion

        #region Clear cache
        public void ClearCache(string prefix)
        {
            List<string> keys = _cacheUtils.GetAllKeys()
                                    .Where(k => k.StartsWith(prefix))
                                    .ToList();
            foreach (string key in keys)
            {
               _cacheUtils.Remove(key); 
            }
        }

        #endregion
    }
}