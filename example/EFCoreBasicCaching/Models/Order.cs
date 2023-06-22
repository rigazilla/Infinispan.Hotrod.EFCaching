using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Text.Json;

namespace EFCoreBasicCaching
{
    public class Order{
        [DatabaseGeneratedAttribute(DatabaseGeneratedOption.Identity), Key()]
        public int OrderId { get; set; }
        public DateTime OrderDate { get; set; }

           // public int MenuId { get; set; }
        public double TotalPrice { get; set; }

        public ICollection<Menu> MenuItems { get; set; } = null!;

        public string toJson() {
            return JsonSerializer.Serialize(this);
        }
    }
}