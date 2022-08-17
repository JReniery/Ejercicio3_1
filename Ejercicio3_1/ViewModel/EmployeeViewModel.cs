using Ejercicio3_1.Model;
using Firebase.Database;
using Firebase.Database.Query;
using Plugin.Media.Abstractions;
using Plugin.Media;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.Forms;
using Firebase.Storage;
using Newtonsoft.Json;
using System.Linq;
using System.Data;

namespace Ejercicio3_1.ViewModel
{
    public class EmployeeViewModel : BaseViewModel
    {
        FirebaseClient firebaseClient = new FirebaseClient("https://ejercicio3-1-default-rtdb.firebaseio.com/");

        public INavigation Navigation { get; set; }

        public string _id;
        public string _name;
        public string _lastName;
        public string _age;
        public string _address;
        public string _job;
        public ImageSource _photo = "DefaultProfile.png";        
        
        private string photoName = "";
        public string _imgLink;
        public string _oldId;



        public EmployeeViewModel()
        {
            SaveEmployeeCommand = new Command(async () => await AddEmployeeClicked());
            UploadPhoto = new Command(() => OnUploadPhotoClicked());
            EmployeeListCommand = new Command(async () => await OnEmployeeListCommand());

            //btnText = "Guardar";
            //isVisibleAlert = false;
        }
        

        public string Id
        {
            get { return this._id; }
            set { this._id = value; OnPropertyChanged(); }
        }

        public string Name
        {
            get { return this._name; }
            set { this._name = value; OnPropertyChanged(); }
        }
        public string LastName
        {
            get { return this._lastName; }
            set { this._lastName = value; OnPropertyChanged(); }
        }
        public string Age
        {
            get { return this._age; }
            set { this._age = value; OnPropertyChanged(); }
        }
        public string Address
        {
            get { return this._address; }
            set { this._address = value; OnPropertyChanged(); }
        }
        public string Job
        {
            get { return this._job; }
            set { this._job = value; OnPropertyChanged(); }
        }
        public ImageSource Photo
        {
            get { return this._photo; }
            set { this._photo = value; OnPropertyChanged(); }
        }
        public string ImgLink
        {
            get { return this._imgLink; }
            set { this._imgLink = value; OnPropertyChanged(); }
        }
        public string OldId
        {
            get { return this._oldId; }
            set { this._oldId = value; OnPropertyChanged(); }
        }

        /*#region CONFIGS ALERT

        public bool _isVisibleAlert;
        public string _bColorAlert;
        public string _textAlert;  
        
        public bool isVisibleAlert
        {
            get { return this._isVisibleAlert; }
            set { this._isVisibleAlert = value; OnPropertyChanged(); }
        }
        public string bColorAlert
        {
            get { return this._bColorAlert; }
            set { this._bColorAlert = value; OnPropertyChanged(); }
        }
        public string textAlert
        {
            get { return this._textAlert; }
            set { this._textAlert = value; OnPropertyChanged(); }
        }
        
        private void AlertTimer()
        {
            int _countSeconds = 5;
            Device.StartTimer(TimeSpan.FromSeconds(1), () =>
            {
                _countSeconds--;
                if (_countSeconds > 0) isVisibleAlert = true; 
                else isVisibleAlert = false;

                return Convert.ToBoolean(_countSeconds);
            });

        }

        #endregion CONFIGS ALERT

        public string _btnText;
        public string btnText
        {
            get { return this._btnText; }
            set { this._btnText = value; OnPropertyChanged(); }
        }*/


        public ICommand SaveEmployeeCommand { get; }
        public ICommand EmployeeListCommand { get; }
        public ICommand UploadPhoto { get; }

        

        private async Task AddEmployeeClicked()
        {           
            if (!string.IsNullOrEmpty(this.Id) && !string.IsNullOrEmpty(this.Name) && !string.IsNullOrEmpty(this.LastName)
            && !string.IsNullOrEmpty(this.Age) && !string.IsNullOrEmpty(this.Address) && !string.IsNullOrEmpty(this.Job)
            && !string.IsNullOrEmpty(this.ImgLink))
            { 
                if (string.IsNullOrEmpty(OldId))
                {
                    await firebaseClient.Child("Employees").PostAsync(new Employee
                    {
                        id = Id,
                        name = Name,
                        lastName = LastName,
                        age = Age,
                        address = Address,
                        job = Job,
                        photo = ImgLink
                    });                    
                    //textAlert = "Usuario Agregado";
                   // AlertTimer();
                }
                else
                {
                    //btnText = "Actualizar";

                    var toUpdateEmpleado = (await firebaseClient
                      .Child("Employees")
                      .OnceAsync<Employee>()).Where(e => e.Object.id == OldId).FirstOrDefault();

                    await firebaseClient
                      .Child("Employees")
                      .Child(toUpdateEmpleado.Key)
                      .PutAsync(new Employee()
                      {
                          id = Id,
                          name = Name,
                          lastName = LastName,
                          age = Age,
                          address = Address,
                          job = Job,
                          photo = ImgLink
                      });
                    //textAlert = "Usuario Actualizado";
                }

                Clear();

                //bColorAlert = "#198754";

                //if (toUpdateEmpleado == null)
                //{

                //await Application.Current.MainPage.DisplayAlert("Aviso", "No Existe", "OK");
                //}
                //else
                //{   

                //await Application.Current.MainPage.DisplayAlert("Aviso", "Existe", "OK");
                //}


            }
            else
            {
                await Application.Current.MainPage.DisplayAlert("Aviso", "Por favor complete todos los datos del usuario", "OK");
            }
        }

        

        private async void OnUploadPhotoClicked()
        {
            string action = await Application.Current.MainPage.DisplayActionSheet("Seleccione una opción:", "Cancelar", null, "Tomar Foto", "Galería");
            MediaFile photoFile = null;
            photoName = "IMG_" + Id + ".png";

            try
            {
                if (action == "Tomar Foto")
                {
                    //photoName = "IMG_" + DateTime.Now.ToString("yyyyMMdd_HHmmss");
                    
                    //cameraOptions.PhotoSize = PhotoSize.Small;            
                    photoFile = await CrossMedia.Current.TakePhotoAsync(new StoreCameraMediaOptions
                    {
                        Name = photoName
                    });                    
                }
                else if (action == "Galería")
                {
                    photoFile = await CrossMedia.Current.PickPhotoAsync();
                    //var photo = await Xamarin.Essentials.MediaPicker.PickPhotoAsync();                    
                }

                if (photoFile != null)
                {
                    var task = new FirebaseStorage("ejercicio3-1.appspot.com",
                    new FirebaseStorageOptions
                    {
                        ThrowOnCancel = true
                    })
                    .Child("EmployeePhotos")
                    .Child(photoName)
                    .PutAsync(photoFile.GetStream());
                    //.PutAsync(await photo.OpenReadAsync());

                    Photo = ImageSource.FromStream(() =>
                    {
                        return photoFile.GetStream();
                    });

                    ImgLink = await task;  
                }


            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

        private async Task OnEmployeeListCommand()
        {
            await Navigation.PushAsync(new View.EmployeeList());
        }


        void Clear()
        {
            Id = string.Empty;
            Name = string.Empty;
            LastName = string.Empty;
            Age = string.Empty;
            Address = string.Empty;
            Job = string.Empty;
            Photo = "DefaultProfile.png";
            ImgLink = string.Empty;
            OldId = string.Empty;
            //btnText = "Guardar";
        }

    }
}
