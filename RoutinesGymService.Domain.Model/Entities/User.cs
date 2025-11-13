using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RoutinesGymService.Domain.Model.Entities
{
    [Table("users")]
    public class User
    {
        [Key]
        [Column("user_id")]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long UserId { get; set; } 

        [Column("dni")]
        public string Dni { get; set; } = string.Empty;

        [Column("serial_number")]
        public string SerialNumber { get; set; } = string.Empty;

        [Column("username")]
        public string Username { get; set; } = string.Empty;

        [Column("surname")]
        public string Surname { get; set; } = string.Empty;

        [Column("email")]
        public string Email { get; set; } = string.Empty;

        [Column("friend_code")]
        public string FriendCode { get; set; } = string.Empty;

        [Column("password")]
        public byte[] Password { get; set; } = Array.Empty<byte>();

        [Column("role")]
        public int Role { get; set; } = -1;

        [Column("role_string")]
        public string RoleString { get; set; } = string.Empty;

        [Column("inscription_date")]
        public DateTime InscriptionDate { get; set; } = DateTime.MinValue;

        
        public virtual ICollection<Routine> Routines { get; set; } = new List<Routine>();

        [InverseProperty("User")]
        public virtual ICollection<UserFriend> FriendshipsAsUser { get; set; } = new List<UserFriend>();

        [InverseProperty("Friend")]
        public virtual ICollection<UserFriend> FriendshipsAsFriend { get; set; } = new List<UserFriend>();
    }
}