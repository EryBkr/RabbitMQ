using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Web.Watermark.Services
{
    //Resmin adresini tuttuğum bir Event classı oluşturdum
    public class ProductImageCreatedEvent
    {
        public string ImageName { get; set; }
    }
}
