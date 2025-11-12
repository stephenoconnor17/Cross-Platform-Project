using Microsoft.Maui.Layouts;

namespace CROSSPLATFORM2DGAME
{
    public partial class MainPage : ContentPage
    {

        public MainPage() {
            InitializeComponent();

        }


        protected override async void OnAppearing() {
            base.OnAppearing();

            this.Dispatcher.Dispatch(() =>
            {
                // --- Screen center ---
                double screenCenterX = this.Width / 2;
                double screenCenterY = this.Height / 2;

                // --- Map center ---
                double mapCenterX = mapLayout.Width / 2;
                double mapCenterY = mapLayout.Height / 2;

                // --- Center player on screen ---
                double playerX = screenCenterX - playerLayout.Width / 2;
                double playerY = screenCenterY - playerLayout.Height / 2;
                AbsoluteLayout.SetLayoutBounds(playerLayout, new Rect(playerX, playerY, playerLayout.Width, playerLayout.Height));
                AbsoluteLayout.SetLayoutFlags(playerLayout, AbsoluteLayoutFlags.None);

                // --- Center map so its middle is under the player ---
                mapLayout.TranslationX = -(mapCenterX - screenCenterX);
                mapLayout.TranslationY = -(mapCenterY - screenCenterY);

                // --- Spawn an enemy above the player ---
                var enemy = new Image {
                    Source = "enemy.png", // make sure this is a visible enemy image
                    WidthRequest = 50,
                    HeightRequest = 100
                };

                // Enemy position relative to the map
                double enemyX = mapCenterX - enemy.WidthRequest / 2;  // horizontally center on map
                double enemyY = mapCenterY - 150;                    // 150px above map center

                AbsoluteLayout.SetLayoutBounds(enemy, new Rect(enemyX, enemyY, enemy.WidthRequest, enemy.HeightRequest));
                AbsoluteLayout.SetLayoutFlags(enemy, AbsoluteLayoutFlags.None);
                mapLayout.Children.Add(enemy);
            });

            //DisplayAlert("HELP", mapCenterX.ToString(), "x");
        }
    }
}
