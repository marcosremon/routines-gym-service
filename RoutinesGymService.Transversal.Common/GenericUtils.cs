using RoutinesGymService.Domain.Model.Enums;
using System.Text;

namespace RoutinesGymService.Transversal.Common
{
    public class GenericUtils
    {
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

        public static WeekDay ChangeStringToEnumOnDayName(string dayName)
        {
            dayName = ChangeDayLanguage(dayName);
            switch (dayName)
            {
                case "Monday": return WeekDay.MONDAY; 
                case "Tuesday": return WeekDay.TUESDAY; 
                case "Wednesday": return WeekDay.WEDNESDAY; 
                case "Thursday": return WeekDay.THUESDAY; 
                case "Friday": return WeekDay.FRIDAY; 
                case "Saturday": return WeekDay.SATURDAY; 
                case "Sunday": return WeekDay.SUNDAY; 
            }

            return WeekDay.MONDAY; // Default to Monday if no match found
        }

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
    }
}
