using RoutinesGymService.Domain.Model.Enums;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RoutinesGymService.Domain.Model.Entities
{
    [Table("exercise_progress")]
    public class ExerciseProgress
    {
        [Key]
        [Column("progress_id")]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long ProgressId { get; set; }

        [Column("exercise_id")]
        public long ExerciseId { get; set; }

        [Column("routine_id")]
        public long RoutineId { get; set; }

        [Column("day_name")]
        public string DayName { get; set; } = string.Empty; 

        [Column("sets")]
        public int Sets { get; set; } 

        [Column("reps")]
        public int Reps { get; set; } 

        [Column("weight")]
        public float Weight { get; set; } 

        [Column("performed_at")]
        public DateTime PerformedAt { get; set; }


        [ForeignKey("ExerciseId")]
        [InverseProperty("ProgressEntries")]
        public virtual Exercise? Exercise { get; set; }

        [ForeignKey("RoutineId")]
        [InverseProperty("ProgressEntries")]
        public virtual Routine? Routine { get; set; }
    }
}