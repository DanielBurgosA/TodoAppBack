using Microsoft.EntityFrameworkCore;
using To_Do_List_Back.Models;

namespace To_Do_List_Back.Models
{
    public class ListContext : DbContext
    {
        public ListContext(DbContextOptions<ListContext> options) : base(options)
        {
        }

        public DbSet<TodoList> List { get; set; }
        public DbSet<TodoTask> Task { get; set; }
        public DbSet<User> User { get; set; }
    }
}