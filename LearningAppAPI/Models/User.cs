using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LearningApp.Api.Models
{
    [Table("users")]
    public class User
    {
        [Key]
        [Column("id")]
        [MaxLength(36)]
        public string Id { get; set; } = Guid.NewGuid().ToString();

        [Required]
        [Column("email")]
        [MaxLength(255)]
        [EmailAddress]
        public string Email { get; set; }

        [Required]
        [Column("full_name")]
        [MaxLength(255)]
        public string FullName { get; set; }

        [Required]
        [Column("password_hash")]
        [MaxLength(255)]
        public string PasswordHash { get; set; }

        [Column("email_verified")]
        public bool EmailVerified { get; set; } = false;

        [Column("created_at")]
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        [Column("updated_at")]
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

        [Column("last_login")]
        public DateTime? LastLogin { get; set; }

        [Column("is_active")]
        public bool IsActive { get; set; } = true;

        // Navigation property
        public virtual ICollection<RefreshToken> RefreshTokens { get; set; }
    }

    [Table("refresh_tokens")]
    public class RefreshToken
    {
        [Key]
        [Column("id")]
        public int Id { get; set; }

        [Required]
        [Column("user_id")]
        [MaxLength(36)]
        public string UserId { get; set; }

        [Required]
        [Column("token")]
        [MaxLength(500)]
        public string Token { get; set; }

        [Column("expires_at")]
        public DateTime ExpiresAt { get; set; }

        [Column("created_at")]
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        [Column("revoked_at")]
        public DateTime? RevokedAt { get; set; }

        [Column("replaced_by_token")]
        [MaxLength(500)]
        public string? ReplacedByToken { get; set; }

        public bool IsExpired => DateTime.UtcNow >= ExpiresAt;
        public bool IsRevoked => RevokedAt != null;
        public bool IsActive => !IsRevoked && !IsExpired;

        // Navigation property
        public virtual User User { get; set; }
    }

  
}