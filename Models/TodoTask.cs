namespace To_Do_List_Back.Models
{
    public class TodoTask
    {
        public long Id { get; set; }
        public string? Title { get; set; }
        public bool Completed { get; set; } = false;

        public TodoTask()
        {
            Completed = false;
        }

        public long TodoListId { get; set; }
    }
}