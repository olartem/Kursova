using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Kursova.Models
{
    public class Game
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public string Id { get; set; }
        public string title { get; set; }
        public uint number { get; set; }
        public string description { get; set; }
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}")]
        public DateTime StartTime { get; set; }
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}")]
        public DateTime EndTime { get; set; }
        public byte[] image { get; set; }
        public List<GameResult> Results { get; set; } = new List<GameResult>();

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

