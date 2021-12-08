using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace Kursova.Models
{
    public class Product
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public string Id { get; set; }
        public string title { get; set; }
        public string description { get; set; }
        public int price { get; set; }
        public byte[] image { get; set; }
        public List<Purchase> purchases { get; set; } = new List<Purchase>();

        public string imageUrl()
        {
            string imageData;
            if (image != null)
            {
                string imageBase64Data =
                    Convert.ToBase64String(image);
                imageData =
                    string.Format("data:image/*;base64,{0}",
                        imageBase64Data);
            }
            else
            {
                imageData = "/noimage.jpg";
            }

            return imageData;
        }
    }
}

