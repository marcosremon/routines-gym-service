using RoutinesGymService.Domain.Model.Enums;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RoutinesGymService.Domain.Model.Entities
{
    [Table("split_days")]
    public class SplitDay
    {
        [Key]
        [Column("split_day_id")]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int SplitDayId { get; set; }

        [Column("day_name")]
        public WeekDay? DayName { get; set; } 

        [Column("routine_id")]
        public int RoutineId { get; set; }

        [Column("day_exercises_description")]
        public string DayExercisesDescription { get; set; } = string.Empty;


        [ForeignKey("RoutineId")]
        [InverseProperty("SplitDays")]
        public virtual Routine? Routine { get; set; }

        [InverseProperty("SplitDay")]
        public virtual ICollection<Exercise> Exercises { get; set; } = new List<Exercise>();
    }
}