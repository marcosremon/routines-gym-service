using RoutinesGymService.Domain.Model.Entities;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RoutinesGymApp.Domain.Entities
{
    [Table("stats")]
    public class Step
    {
        [Key]
        [Column("stats_id")]
        public long StatsId { get; set; }

        [Column("user_id")]
        [ForeignKey("User")]
        public long UserId { get; set; }

        [Column("date")]
        public DateTime? Date { get; set; } = DateTime.MinValue;

        [Column("steps")]
        public int? Steps { get; set; } = 0;

        [Column("daily_steps_goal")]
        public int? DailyStepsGoal { get; set; }

        public virtual User? User { get; set; }
    }
}