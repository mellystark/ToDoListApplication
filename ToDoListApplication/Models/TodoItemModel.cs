using System.ComponentModel.DataAnnotations;

namespace ToDoListApplication.Models
{
    public class TodoItemModel
    {
        
        public int Id { get; set; }

        
        public string Title { get; set; }

        public bool IsCompleted { get; set; }

        // Kullanıcı referansı
        public string? UserId { get; set; }

        public ApplicationUser? User { get; set; }
    }
}
