using System.ComponentModel.DataAnnotations;

namespace Angular_WebSpeechText.Models
{
    // класс модели-представления (view-model)  по этой модели будет типизироваться вьюшка авторизации
    public class LoginModel
    {
        [Required]
        public string? Login { get; set; }

        [Required]
        [DataType(DataType.Password)]
        public string? Password { get; set; }
    }
}
