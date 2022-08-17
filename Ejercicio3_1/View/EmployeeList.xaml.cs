using Ejercicio3_1.Model;
using Ejercicio3_1.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Ejercicio3_1.View
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class EmployeeList : ContentPage
    {
        public EmployeeList()
        {
            InitializeComponent();
            BindingContext = new EmployeeListViewModel();
        }       
    }
}