namespace Backend.Models
{
    public class LoginModel 
    {
        [Key]
        public string? Document { get; set; }
        [Required(ErrorMessage = "O campo {0} é obrigatório.")]
        public string? Password { get; set; }
        [Required(ErrorMessage = "O campo {0} é obrigatório.")]
    }
}