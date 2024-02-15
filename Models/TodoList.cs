namespace To_Do_List_Back.Models
{
    public class TodoList
    {
        public long Id { get; set; }
        public string Name { get; set; }
        public string Color { get; set; }
        public long UserId { get; set; } 
        public List<TodoTask>? Tasks { get; set; }

        public TodoList()
        {
            Tasks = new List<TodoTask>();
        }
    }
}