using Plugin.Media;
using PM02E18124.Models;
using PM02E18124.Views;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Essentials;
using Xamarin.Forms;

namespace PM02E18124
{
    public partial class MainPage : ContentPage
    {
        public MainPage()
        {
            InitializeComponent();
        }


        public async void LoadCoord()
        {
            try
            {
                var georequest = new GeolocationRequest(GeolocationAccuracy.Best, TimeSpan.FromSeconds(10));
                var tokendecancelacion = new System.Threading.CancellationTokenSource();
                var location = await Geolocation.GetLocationAsync(georequest, tokendecancelacion.Token);
                if (location != null)
                {

                    var lon = location.Longitude;
                    var lat = location.Latitude;

                    txtLat.Text = lat.ToString();
                    txtLon.Text = lon.ToString();
                }
            }
            catch (FeatureNotSupportedException fnsEx)
            {
                await DisplayAlert("Atencion", "Este dispositivo no soporta GPS" + fnsEx, "Ok");
            }
            catch (FeatureNotEnabledException fneEx)
            {
                await DisplayAlert("Atencion", "Error de Dispositivo, favor validar si su GPS esta activo", "Ok");
                System.Diagnostics.Process.GetCurrentProcess().Kill();

            }
            catch (PermissionException pEx)
            {
                await DisplayAlert("Atencion", "Sin Permisos de Geolocalizacion" + pEx, "Ok");
            }
            catch (Exception ex)
            {
                await DisplayAlert("Atencion", "Sin Ubicacion " + ex, "Ok");
            }
        }

        Plugin.Media.Abstractions.MediaFile Filefoto = null;

        private Byte[] ConvertImageToByteArray()
        {
            if (Filefoto != null)
            {
                using (MemoryStream memory = new MemoryStream()) 
                {
                    Stream stream = Filefoto.GetStream();
                    stream.CopyTo(memory);
                    return memory.ToArray();
                }

            }
            return null;

        }
        private async void btnAdd_Clicked(object sender, EventArgs e)
        {
            if (Filefoto == null)
            {
                await this.DisplayAlert("Atencion", "Debe tomar una foto", "OK");
            }
            else if (string.IsNullOrEmpty(txtDescripcion.Text))
            {
                await this.DisplayAlert("Atencion", "El campo descripcion es obligatorio.", "OK");

            }
            else if (string.IsNullOrEmpty(txtLat.Text) && string.IsNullOrEmpty(txtLon.Text))
            {
                await this.DisplayAlert("Atencion", "No fue posible agregar registro. Faltan coordenadas.", "OK");

                LoadCoord();

            }
            else
            {
                var sitio = new Models.sitios
                {
                    id = 0,
                    latitud = txtLat.Text,
                    longitud = txtLon.Text,
                    descripcion = txtDescripcion.Text,
                    foto = ConvertImageToByteArray(),
                };

                var result = await App.DBase.SitioSave(sitio);

                //super clase
                if (result > 0)
                {
                    await DisplayAlert("Atencion", "Sitio Registrado", "OK");
                    Clear();

                }
                else
                {
                    await DisplayAlert("Atencion", "Error al Registrar", "OK");
                }
            }
        }

        private async void btnList_Clicked(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new ListaSitios());
        }

        private async void btnFoto_Clicked(object sender, EventArgs e)
        {
            Filefoto = await CrossMedia.Current.TakePhotoAsync(new Plugin.Media.Abstractions.StoreCameraMediaOptions
            {
                Directory = "MisFotos",
                Name = "test.jpg",
                SaveToAlbum = true,
            });


           

            if (Filefoto != null)
            {
                fotoSitio.Source = ImageSource.FromStream(() =>
                {
                    return Filefoto.GetStream();
                });
            }
        }

        protected async override void OnAppearing()
        {
            base.OnAppearing();

            LoadCoord();
            try
            {
                var georequest = new GeolocationRequest(GeolocationAccuracy.Best, TimeSpan.FromSeconds(10));
                var tokendecancelacion = new System.Threading.CancellationTokenSource();
                var location = await Geolocation.GetLocationAsync(georequest, tokendecancelacion.Token);
                if (location != null)
                {

                    var lon = location.Longitude;
                    var lat = location.Latitude;

                    txtLat.Text = lat.ToString();
                    txtLon.Text = lon.ToString();
                }
            }
            catch (FeatureNotSupportedException fnsEx)
            {
                await DisplayAlert("Atencion", "Este dispositivo no soporta GPS" + fnsEx, "Ok");
            }
            catch (FeatureNotEnabledException fneEx)
            {
                await DisplayAlert("Atencion", "Error del dispositivo, favor validar si su GPS esta activo", "Ok");
                System.Diagnostics.Process.GetCurrentProcess().Kill(); //cerramos la aplicacion hasta que el usuario active el GPS

            }
            catch (PermissionException pEx)
            {
                await DisplayAlert("Atencion", "Sin Permisos de Geolocalizacion" + pEx, "Ok");
            }
            catch (Exception ex)
            {
                await DisplayAlert("Atencion", "Sin Ubicacion " + ex, "Ok");
            }
        }

        private async void btnSalir_Clicked(object sender, EventArgs e)
        {
            bool answer = await DisplayAlert("Confirmacion", "¿Quiere cerrar la aplicacion?", "Si", "No");
            Debug.WriteLine("Answer: " + answer);
            if (answer == true)
            {
 
                    System.Environment.Exit(0);
                    System.Environment.Exit(1);
   
            };

        }

        private void Clear()
        {
            txtLat.Text = "";
            txtLon.Text = "";
            txtDescripcion.Text = "";
        }
    }
}
