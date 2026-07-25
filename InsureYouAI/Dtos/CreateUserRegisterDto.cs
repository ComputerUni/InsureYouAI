using InsureYouAI.Validations;
using System.ComponentModel.DataAnnotations;

namespace InsureYouAI.Dtos
{
    public class CreateUserRegisterDto
    {
        public string Name { get; set; }
        public string Surname { get; set; }
        public string Username { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }

        [MustBeTrue(ErrorMessage = "Kullanım şartlarını kabul etmeniz gerekiyor.")]
        public bool AcceptTerms { get; set; }
    }
}
