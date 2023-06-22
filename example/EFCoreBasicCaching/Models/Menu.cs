using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Text.Json;

namespace EFCoreBasicCaching
{
    public class Menu
    {
        [DatabaseGeneratedAttribute(DatabaseGeneratedOption.Identity), Key()]
        public int MenuId { get; set; }
        public string MenuName { get; set; } = string.Empty;
        public string MenuDescription { get; set; } = null!;
        public Double Price { get; set; }

        public string toJson() {
            return JsonSerializer.Serialize(this);
        }
    }
}