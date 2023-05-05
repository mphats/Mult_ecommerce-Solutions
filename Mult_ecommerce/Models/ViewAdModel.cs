using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Mult_ecommerce.Models
{
    public class ViewAdModel
    {
        public int ID { get; set; }
        public string Pro_name { get; set; }
        public string Pro_image { get; set; }
        public Nullable<int> Pro_price { get; set; }
        public string Pro_des { get; set; }
        public Nullable<int> Pro_fk_cat { get; set; }
        public Nullable<int> Pro_fk_user { get; set; }

        public string Cat_name { get; set; }
        public Nullable<int> Cat_fk_ad { get; set; }

        public string U_name { get; set; }
        public string U_email { get; set; }
        public string U_image { get; set; }
        public string U_contact { get; set; }
    }
}