using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RoutinesGymService.Domain.Model.Entities
{
    [Table("black_list")]
    public class BlackList
    {
        [Key]
        [Column("black_list_id")]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long BlackListId { get; set; }

        [Column("user_id")]
        public long UserId { get; set; }

        [Column("movile_serial_number")]
        public string SerialNumber { get; set; } = string.Empty;

        [Column("description")]
        public string Description { get; set; } = string.Empty;
    }
}