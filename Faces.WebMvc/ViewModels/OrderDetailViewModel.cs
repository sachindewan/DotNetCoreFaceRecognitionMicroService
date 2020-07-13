using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Faces.WebMvc.ViewModels
{
    public class OrderDetailViewModel
    {
        [Display(Name ="Order Detail Id:")]
        public int OrderDetailId { get; set; }
        public byte[] FaceData { get; set; }
        public string ImageString { get; set; }
    }
}
