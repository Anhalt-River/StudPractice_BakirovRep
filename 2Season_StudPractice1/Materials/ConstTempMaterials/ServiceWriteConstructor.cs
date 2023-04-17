using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace _2Season_StudPractice1.Materials.ConstTempMaterials
{
    public class ServiceWriteConstructor
    {
        public string ServiceName { get; set; }
        public string ClientFullName { get; set; }
        public string ClientEmail { get; set; }
        public string ClientPhone { get; set; }
        public string WriteDateTime { get; set; }
        public DateTime WriteDateTime_forSorting { get; set; }
        public string WriteDateTimeRemains { get; set; }

        public Style RemainTimeStyle { get; set; }

        public ServiceWriteConstructor() 
        {
            ServiceName = "";
            ClientFullName = "";
            ClientEmail = "";
            ClientPhone = "";
            WriteDateTime = "";
            WriteDateTimeRemains = "";
            WriteDateTime_forSorting = new DateTime();
        }
    }
}
