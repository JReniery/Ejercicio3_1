using Ejercicio3_1.Model;
using Ejercicio3_1.View;
using Firebase.Database;
using Firebase.Database.Query;
using LiteDB;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.Essentials;
using Xamarin.Forms;

namespace Ejercicio3_1.ViewModel
{
    public class EmployeeListViewModel : BaseViewModel
    {
        FirebaseClient firebaseClient = new FirebaseClient("https://ejercicio3-1-default-rtdb.firebaseio.com/");

        private ObservableCollection<Employee> _employee;
        private Employee _selectedEmployee;

        INavigation Navigation => Application.Current.MainPage.Navigation;

        public ObservableCollection<Employee> EmployeeList
        {
            get { return _employee; }
            set { _employee = value; OnPropertyChanged(); }
        }        

        public Employee SelectedEmployee
        {
            get { return _selectedEmployee; }
            set { _selectedEmployee = value; OnPropertyChanged(); }
        }
        
        public ICommand DeleteCommand { private set; get; }
        public ICommand UpdateCommand { private set; get; }
        

        public EmployeeListViewModel()
        {
            UpdateCommand = new Command(OnUpdateCommandClicked);
            DeleteCommand = new Command(OnDeleteCommandClicked);

            EmployeeList = new ObservableCollection<Employee>();
            
            GetEmployeeList();
        }


        private async void OnDeleteCommandClicked(object obj)
        {            
            if (await Application.Current.MainPage.DisplayAlert("Confirmar", "Deliminar este empleado?", "Sí", "No"))
            {
                var selecteditem = obj as Employee;                
                await DeleteEmployee(selecteditem.id);

                await Application.Current.MainPage.DisplayAlert("Aviso", "Empleado Eliminado", "OK");
                
                GetEmployeeList();
            }
        }

        private async void OnUpdateCommandClicked(object obj)
        {
            var selecteditem = obj as Employee; 
            var page = new MainPage();

            page.BindingContext = new EmployeeViewModel()
            {
                Id = selecteditem.id,
                Name = selecteditem.name,
                LastName = selecteditem.lastName,
                Age = selecteditem.age,
                Address = selecteditem.address,
                Job = selecteditem.job,
                Photo = selecteditem.photo,
                ImgLink = selecteditem.photo,
                OldId = selecteditem.id
            };
            await Navigation.PushAsync(page);
        }

        private void GetEmployeeList()
        {
            EmployeeList.Clear();

            var collection = firebaseClient
                .Child("Employees")
                .AsObservable<Employee>()
                .Subscribe((dbevent) =>
                {
                    if (dbevent.Object != null)
                    {
                        EmployeeList.Add(dbevent.Object);
                    }
                });
        }

        //#3b6c4b

        private async Task DeleteEmployee(string id)
        {
            var toDeleteEmpleado = (await firebaseClient
              .Child("Employees")
              .OnceAsync<Employee>()).Where(a => a.Object.id == id).FirstOrDefault();
            await firebaseClient.Child("Employees").Child(toDeleteEmpleado.Key).DeleteAsync();                        
        }

        /*public async Task UpdateEmployee(string Id, string Name, string LastName, string Age, string Address, string Job, string Photo)
        {
            var toUpdateEmpleado = (await firebaseClient
              .Child("Employees")
              .OnceAsync<Employee>()).Where(a => a.Object.id == Id).FirstOrDefault();

            await firebaseClient
              .Child("Employees")
              .Child(toUpdateEmpleado.Key)
              .PutAsync(new Employee() { id = Id, name = Name, lastName = LastName, age = Age, address = Address, job = Job, photo = Photo });
        }*/

    }    
}
