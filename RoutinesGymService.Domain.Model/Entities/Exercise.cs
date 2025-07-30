using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RoutinesGymService.Domain.Model.Entities
{
    [Table("exercises")]
    public class Exercise
    {
        [Key]
        [Column("exercise_id")]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long ExerciseId { get; set; }

        [Column("exercise_name")]
        public string ExerciseName { get; set; } = string.Empty;

        [Column("routine_id")]
        public long RoutineId { get; set; }

        [Column("split_day_id")]
        public long SplitDayId { get; set; }


        [ForeignKey("SplitDayId")]
        [InverseProperty("Exercises")]
        public virtual SplitDay? SplitDay { get; set; }

        [ForeignKey("RoutineId")]
        [InverseProperty("Exercises")]
        public virtual Routine? Routine { get; set; }

        [InverseProperty("Exercise")]
        public virtual ICollection<ExerciseProgress> ProgressEntries { get; set; } = new List<ExerciseProgress>();
    }
}