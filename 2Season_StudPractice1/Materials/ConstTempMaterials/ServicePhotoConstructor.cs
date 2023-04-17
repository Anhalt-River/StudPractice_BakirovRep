using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Policy;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace _2Season_StudPractice1.Materials.ConstTempMaterials
{
    public class ServicePhotoConstructor
    {
        public int Id { get; set; }
        public int ServiceId { get; set; }
        public ImageSource ServiceImage { get; set; }

        public ServicePhotoConstructor()
        {
            Id = -1;
            ServiceId = -1;
        }
    }
}
