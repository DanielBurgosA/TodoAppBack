namespace To_Do_List_Back.Models
{
    public class List
    {
        public long Id { get; set; }
        public string Name { get; set; }
        public string Color { get; set; }
        public long UserId { get; set; } 
        public List<TodoTask>? Tasks { get; set; }

        public List()
        {
            Tasks = new List<TodoTask>();
        }
    }
}