using _2Season_StudPractice1.AdoApp;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;

namespace _2Season_StudPractice1
{
    /// <summary>
    /// Логика взаимодействия для App.xaml
    /// </summary>
    public partial class App : Application
    {
        static public LearnEntities Connection { get; private set; } = new LearnEntities();
        static public bool GlobalData_isEditPageOpen { get; set; } = false;
        static public bool GlobalData_isEditAllWritesOpen { get; set; } = false;
        static public bool GlobalData_isCodeTaked { get; set; } = false;
    }
}
