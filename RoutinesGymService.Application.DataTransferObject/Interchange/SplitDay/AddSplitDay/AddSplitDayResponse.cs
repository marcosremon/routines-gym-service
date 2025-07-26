using RoutinesGymService.Application.DataTransferObject.Entity;
using RoutinesGymService.Transversal.Common;

namespace RoutinesGymService.Application.DataTransferObject.SplitDay.AddSplitDay
{
    public class AddSplitDayResponse : BaseResponse
    {
        public UserDTO? UserDTO { get; set; }
    }
}