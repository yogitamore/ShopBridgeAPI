using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace ShopBridgeAPI.Entities
{
    public class ProductDetails
    {
        [Key]
        public int ID { get; set; }

        [Required(ErrorMessage = "Please Enter Item name..")]
       
        public string ItemName { get; set; }

        public string Description { get; set; }

        public decimal Price { get; set; }

        public string FilePath { get; set; }
    }
}
