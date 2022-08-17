using Ejercicio3_1.ViewModel;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace Ejercicio3_1.View
{
    public partial class MainPage : ContentPage
    {
        public MainPage()
        {
            InitializeComponent();
            //this.BindingContext = new EmployeeViewModel();
        }

        private async void btnEmployeeList_Clicked(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new EmployeeList());
        }
    }
}
