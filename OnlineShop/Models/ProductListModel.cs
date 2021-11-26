using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace OnlineShop.Models
{
    public class ProductListModel
    {
        public int Index { get; set; }
        public string Title { get; set; }
        public List<Product> Products { get; set; }
    }
}