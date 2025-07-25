using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RoutinesGymService.Domain.Model.Entities
{
    [Table("user_friends")]
    public class UserFriend
    {
        [Key]
        [Column("user_friend_id")]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long UserFriendId { get; set; }

        [Column("user_id")]
        public long UserId { get; set; }

        [Column("friend_id")]
        public long FriendId { get; set; }


        [ForeignKey("UserId")]
        [InverseProperty("FriendshipsAsUser")]
        public virtual User? User { get; set; }

        [ForeignKey("FriendId")]
        [InverseProperty("FriendshipsAsFriend")]
        public virtual User? Friend { get; set; }
    }
}