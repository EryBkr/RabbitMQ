using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RabbitMQHelloWorld.Shared
{
    //Complex Type örneği için oluşturmuş olduğumuz yapı
    public class Product
    {
        public int Id { get; set; }
        public int Price { get; set; }
        public string Name { get; set; }
    }
}
