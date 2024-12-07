namespace Angular_WebSpeechText.Models
{
    //создаем отношение "один ко многим" между пользователями и сообщениями
    public class User
    {
        public int Id { get; set; }
        public string? FirstName { get; set; }

        public string? LastName { get; set; }

        public string? Login { get; set; }

        public string? Password { get; set; }

        public string? Salt { get; set; }
      
    }
}
