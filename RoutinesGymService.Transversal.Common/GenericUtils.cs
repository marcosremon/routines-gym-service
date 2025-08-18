using RoutinesGymService.Domain.Model.Enums;
using System.Text;

namespace RoutinesGymService.Transversal.Common
{
    public class GenericUtils
    {
        #region week day
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
                default: dayName = "Day"; break;
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
            switch (dayName)
            {
                case "Monday": return WeekDay.MONDAY;
                case "Tuesday": return WeekDay.TUESDAY;
                case "Wednesday": return WeekDay.WEDNESDAY;
                case "Thursday": return WeekDay.THURSDAY;
                case "Friday": return WeekDay.FRIDAY;
                case "Saturday": return WeekDay.SATURDAY;
                case "Sunday": return WeekDay.SUNDAY;
            }

            return WeekDay.MONDAY;
        }
        #endregion

        #region role
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

        #region other
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
    }
}