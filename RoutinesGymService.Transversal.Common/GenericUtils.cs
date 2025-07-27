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
    }
}
