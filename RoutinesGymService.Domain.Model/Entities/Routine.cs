using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RoutinesGymService.Domain.Model.Entities
{
    [Table("routines")]
    public class Routine
    {
        [Key]
        [Column("routine_id")]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long RoutineId { get; set; }

        [Column("routine_name")]
        public string RoutineName { get; set; } = string.Empty;

        [Column("routine_description")]
        public string RoutineDescription { get; set; } = string.Empty ;

        [Column("user_id")]
        public long UserId { get; set; }


        [ForeignKey("UserId")]
        [InverseProperty("Routines")]
        public virtual User User { get; set; } = new User();

        [InverseProperty("Routine")]
        public virtual ICollection<SplitDay> SplitDays { get; set; } = new List<SplitDay>();

        [InverseProperty("Routine")]
        public virtual ICollection<ExerciseProgress> ProgressEntries { get; set; } = new List<ExerciseProgress>();

        [InverseProperty("Routine")]
        public virtual ICollection<Exercise> Exercises { get; set; } = new List<Exercise>();
    }
}