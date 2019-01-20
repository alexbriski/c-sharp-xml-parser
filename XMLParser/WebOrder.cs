using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XMLParser
{
    public class WebOrder
    {
        public string Customer;
        public DateTime Date;
        public int Id;
        public List<WebOrderItem> Items;
    }
}
