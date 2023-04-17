using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace _2Season_StudPractice1.Materials.ConstTempMaterials
{
    public class ServiceConstructor
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public int Cost { get; set; }
        public int OldCost { get; set; }
        public int Discount { get; set; }
        public string DurationInMinuts { get; set; }
        //public string ImagePath { get; set; }
        public Visibility FeaturesAvailable { get; set; }
        public Visibility DiscountExist{ get; set; }
        public Style DiscountColor { get; set; }
        public ImageSource ServiceImage { get; set; }

        public ServiceConstructor()
        {
            Id = -1;
            Title = "Unknown Title";
            Cost = 0;
            Discount = 0;
            OldCost = 0;
            DurationInMinuts = "Unknown duration";
            //ImagePath = "Unknown path";
            FeaturesAvailable = Visibility.Hidden;
            DiscountExist = Visibility.Hidden;
        }                               
    }
}
