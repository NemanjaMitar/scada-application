using System.ComponentModel.DataAnnotations;

namespace DataConcentrator.Model
{
    public enum Role
    {
        Admin,
        Operater,
        Student,
        Teacher
    }

    public class User
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [MaxLength(50)]
        public string Username { get; set; }

        [Required]
        public string Password { get; set; } // U realnom sistemu ovdje bi išao HASH lozinke, ali ostavljamo plain text radi jednostavnosti ukoliko nije drugačije traženo

        [Required]
        public Role UserRole { get; set; }
    }
}