using Microsoft.Maui.ApplicationModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CROSSPLATFORM2DGAME {
    internal class mapGenerator {

        AbsoluteLayout mapLayout;
        double maxWidth, maxHeight;
        public List<enemy> enemies = new List<enemy>();

        bool firstTime = true;
        List<(double x, double y)> wallLocation = new List<(double,double)>(); // using this because i am annoyed at parsing vector2.
        public mapGenerator(AbsoluteLayout mapLayout, double maxWidth, double maxHeight) {
            this.mapLayout = mapLayout;
            this.maxWidth = maxWidth - 48; // -48 because cant have something begin drawing at the very edge of the map!
            this.maxHeight = maxHeight - 48;

            generateMap();
        }

        public void spawnItems() {
            var random = new Random();
            //for loops for each item, change the i limit for amount of items.
            for (int i = 0; i < 4; i++) {

                double rx = random.NextDouble() * (this.maxWidth);
                double ry = random.NextDouble() * (this.maxHeight);

                fuelObject fuelTest = new fuelObject(rx, ry);

                this.mapLayout.Children.Add(fuelTest.gameObjectLayout);
            }

            for (int i = 0; i < 8; i++) {

                double rx = random.NextDouble() * (this.maxWidth);
                double ry = random.NextDouble() * (this.maxHeight);

                lootObject lootTest = new lootObject(rx, ry);

                this.mapLayout.Children.Add(lootTest.gameObjectLayout);
            }
        }

        public void spawnEnemies(int amount) {
            var random = new Random();
            for (int i = 0; i < amount; i++) {
                double rx = random.NextDouble() * (this.maxWidth);
                double ry = random.NextDouble() * (this.maxHeight);
                enemy enemyTest = new enemy(rx, ry);
                enemies.Add(enemyTest);
                this.mapLayout.Children.Add(enemyTest.gameObjectLayout);
            }
        }

        public void generateMap() {
            var random = new Random();
            //for loops for each item, change the i limit for amount of items.
            

            for (int i = 0; i < 10; i++) {

                double rx = random.NextDouble() * (this.maxWidth);
                double ry = random.NextDouble() * (this.maxHeight);

                fuelObject fuelTest = new fuelObject(rx,ry);

                this.mapLayout.Children.Add(fuelTest.gameObjectLayout);
            }

            for (int i = 0; i < 20; i++) {

                double rx = random.NextDouble() * (this.maxWidth);
                double ry = random.NextDouble() * (this.maxHeight);

                lootObject lootTest = new lootObject(rx, ry);

                this.mapLayout.Children.Add(lootTest.gameObjectLayout);
            }

            for (int i = 0; i < 15; i++) {
                double rx = 0;
                double ry = 0;
                if (i == 0 && firstTime) {
                    firstTime = false;
                    rx = random.NextDouble() * (this.maxWidth);
                    ry = random.NextDouble() * (this.maxHeight);
                    wallLocation.Add((rx, ry));
                } else {
                    rx = random.NextDouble() * (this.maxWidth);
                    ry = random.NextDouble() * (this.maxHeight);
                    for(int j = 0; j < wallLocation.Count; j++) {
                        do {
                            rx = random.NextDouble() * (this.maxWidth);
                            ry = random.NextDouble() * (this.maxHeight);
                        } while (Math.Abs(rx - wallLocation[j].x) < 48 && Math.Abs(ry - wallLocation[j].y) < 48);
                    }
                }
                wallObject wallTest = new wallObject(rx, ry);
                this.mapLayout.Children.Add(wallTest.gameObjectLayout);
            }

            for(int i = 0; i < 3; i++) {
                double rx = random.NextDouble() * (this.maxWidth);
                double ry = random.NextDouble() * (this.maxHeight);
                enemy enemyTest = new enemy(rx, ry);
                enemies.Add(enemyTest);
                this.mapLayout.Children.Add(enemyTest.gameObjectLayout);


            }


        }
    }
}
