using RoutinesGymService.Application.DataTransferObject.Entity;
using RoutinesGymService.Transversal.Common.Responses;

namespace RoutinesGymService.Application.DataTransferObject.SplitDay.UpdateSplitDay
{
    public class UpdateSplitDayResponse : BaseResponse
    {
        public UserDTO UserDTO { get; set; } = new UserDTO();
    }
}