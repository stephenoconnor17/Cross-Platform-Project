using Microsoft.Maui.Layouts;
using System.Diagnostics;

namespace CROSSPLATFORM2DGAME
{
    public partial class MainPage : ContentPage
    {
        Image playerImg;
        Image enemyImg;
        AbsoluteLayout mapLayout;
        AbsoluteLayout playerLayout;
        AbsoluteLayout enemyLayout;


        public MainPage()
        {
            InitializeComponent();
        }

        private void initializeImgObj() 
        {
            playerImg = new Image {
                Source = "player.png",
                WidthRequest = 50,
                HeightRequest = 50
            };
            enemyImg = new Image {
                Source = "player.png",
                WidthRequest = 50,
                HeightRequest = 50
            };
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();
            // Initialize layouts and images
            mapLayout = new AbsoluteLayout
            {
                BackgroundColor = Colors.LightGreen,
                WidthRequest = 800,
                HeightRequest = 400
            };

            Content = mapLayout;

            mapLayout.SizeChanged += (s, e) => 
            {

                initializeImgObj();

                double playerCenterX = mapLayout.Width / 2 - playerImg.Width/2;
                double playerCenterY = mapLayout.Height / 2 - playerImg.Height/2;

                playerLayout = new AbsoluteLayout();
                enemyLayout = new AbsoluteLayout();
                AbsoluteLayout.SetLayoutBounds(playerImg, new Rect(playerCenterX, playerCenterY, 50, 50));
                AbsoluteLayout.SetLayoutFlags(playerImg, AbsoluteLayoutFlags.None);
                playerLayout.Children.Add(playerImg);
                AbsoluteLayout.SetLayoutBounds(enemyImg, new Rect(400, 300, 50, 50));
                AbsoluteLayout.SetLayoutFlags(enemyImg, AbsoluteLayoutFlags.None);
                enemyLayout.Children.Add(enemyImg);
                mapLayout.Children.Add(playerLayout);
                mapLayout.Children.Add(enemyLayout);
            };
           
            // Start game loop
            //Device.StartTimer(TimeSpan.FromMilliseconds(16), GameLoop);
        }
    }
}
