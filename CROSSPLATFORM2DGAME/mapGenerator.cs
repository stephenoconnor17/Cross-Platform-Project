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
        Dictionary<gameObject, int>objectIndexMap = new Dictionary<gameObject, int>();


        bool firstTime = true;
        List<(double x, double y)> objectLocation = new List<(double,double)>(); // using this because i am annoyed at parsing vector2.
        public mapGenerator(AbsoluteLayout mapLayout, double maxWidth, double maxHeight) {
            this.mapLayout = mapLayout;
            this.maxWidth = maxWidth - 48; // -48 because cant have something begin drawing at the very edge of the map!
            this.maxHeight = maxHeight - 48;

            generateMap();
        }

        public void RemoveObject(gameObject obj) {
            if (objectIndexMap.TryGetValue(obj, out int index)) {

                // Remove location from list
                objectLocation.RemoveAt(index);

                // Remove object from dictionary
                objectIndexMap.Remove(obj);

                // Update indices in dictionary for objects after the removed one
                foreach (var key in objectIndexMap.Keys.ToList()) {
                    if (objectIndexMap[key] > index)
                        objectIndexMap[key]--;
                }
            }
        }
        
        //SPAWN OF ITEM ONCE TIME HAS PASSED
        public void spawnItems() {
            var random = new Random();
            //for loops for each item, change the i limit for amount of items.
            //FUEL LOOP
            for (int i = 0; i < 6; i++) {
                double rx, ry;
                bool validPosition;

                do {
                    validPosition = true;
                    rx = random.NextDouble() * (this.maxWidth);
                    ry = random.NextDouble() * (this.maxHeight);

                    // Check against all existing objects
                    foreach (var loc in objectLocation) {
                        if (Math.Abs(rx - loc.x) < 48 && Math.Abs(ry - loc.y) < 48) {
                            validPosition = false;
                            break;
                        }
                    }
                } while (!validPosition);

                objectLocation.Add((rx, ry));
                fuelObject fuelTest = new fuelObject(rx, ry);
                objectIndexMap.Add(fuelTest, objectLocation.Count - 1);
                this.mapLayout.Children.Add(fuelTest.gameObjectLayout);
            }

            //LOOT LOOP
            for (int i = 0; i < 14; i++) {
                double rx, ry;
                bool validPosition;

                do {
                    validPosition = true;
                    rx = random.NextDouble() * (this.maxWidth);
                    ry = random.NextDouble() * (this.maxHeight);

                    // Check against all existing objects
                    foreach (var loc in objectLocation) {
                        if (Math.Abs(rx - loc.x) < 48 && Math.Abs(ry - loc.y) < 48) {
                            validPosition = false;
                            break;
                        }
                    }
                } while (!validPosition);

                objectLocation.Add((rx, ry));
                lootObject lootTest = new lootObject(rx, ry);
                objectIndexMap.Add(lootTest, objectLocation.Count - 1);
                this.mapLayout.Children.Add(lootTest.gameObjectLayout);
            }

            //HEALTH LOOP
            for (int i = 0; i < 2; i++) {
                double rx, ry;
                bool validPosition;

                do {
                    validPosition = true;
                    rx = random.NextDouble() * (this.maxWidth);
                    ry = random.NextDouble() * (this.maxHeight);

                    // Check against all existing objects
                    foreach (var loc in objectLocation) {
                        if (Math.Abs(rx - loc.x) < 48 && Math.Abs(ry - loc.y) < 48) {
                            validPosition = false;
                            break;
                        }
                    }
                } while (!validPosition);

                objectLocation.Add((rx, ry));
                healthObject healthTest = new healthObject(rx, ry);
                objectIndexMap.Add(healthTest, objectLocation.Count - 1);
                this.mapLayout.Children.Add(healthTest.gameObjectLayout);
            }

           
        }

        public void spawnEnemies(int amount) {
            var random = new Random();
            for (int i = 0; i < amount; i++) {
                double rx = random.NextDouble() * (this.maxWidth - 144);
                double ry = random.NextDouble() * (this.maxHeight - 144);
                //No need to check enemy spawn against anything. Doesnt properly collide with anything other than player.
               
                enemy enemyTest = new enemy(rx, ry);
                enemies.Add(enemyTest);
                this.mapLayout.Children.Add(enemyTest.gameObjectLayout);
            }
        }

        //INITIAL SPAWN OF ITEMS
        //((Math.Abs(rx - objectLocation[j].x) < 48 && Math.Abs(ry - objectLocation[j].y) < 48) || (Math.Abs(rx - ((this.maxWidth / 2) - 96)) < 48 && Math.Abs(ry - ((this.maxHeight / 2) - 96)) < 48))
        public void generateMap() {
            var random = new Random();
            //for loops for each item, change the i limit for amount of items.

            //WALL LOOP
            for (int i = 0; i < 15; i++) {
                double rx, ry;
                bool validPosition;

                do {
                    validPosition = true;
                    rx = 72 + random.NextDouble() * (this.maxWidth - 144);
                    ry = 72 + random.NextDouble() * (this.maxHeight - 144);

                    // Check if position is on the player spawn
                    if ((Math.Abs(rx - ((this.maxWidth / 2) - 96)) < 48 && Math.Abs(ry - ((this.maxHeight / 2) - 96)) < 48)) {
                        validPosition = false;
                        continue;
                    }

                    // Check against all existing objects
                    foreach (var loc in objectLocation) {
                        if (Math.Abs(rx - loc.x) < 48 && Math.Abs(ry - loc.y) < 48) {
                            validPosition = false;
                            break;
                        }
                    }
                } while (!validPosition);

                objectLocation.Add((rx, ry));
                wallObject wallTest = new wallObject(rx, ry);
                objectIndexMap.Add(wallTest, objectLocation.Count - 1);
                this.mapLayout.Children.Add(wallTest.gameObjectLayout);
            }
            
            //FUEL LOOP
            for (int i = 0; i < 10; i++) {
                double rx, ry;
                bool validPosition;

                do {
                    validPosition = true;
                    rx = random.NextDouble() * (this.maxWidth);
                    ry = random.NextDouble() * (this.maxHeight);

                    // Check against all existing objects
                    foreach (var loc in objectLocation) {
                        if (Math.Abs(rx - loc.x) < 48 && Math.Abs(ry - loc.y) < 48) {
                            validPosition = false;
                            break;
                        }
                    }
                } while (!validPosition);

                objectLocation.Add((rx, ry));
                fuelObject fuelTest = new fuelObject(rx, ry);
                objectIndexMap.Add(fuelTest, objectLocation.Count - 1);
                this.mapLayout.Children.Add(fuelTest.gameObjectLayout);
            }

            //LOOT LOOP
            for (int i = 0; i < 20; i++) {
                double rx, ry;
                bool validPosition;

                do {
                    validPosition = true;
                    rx = random.NextDouble() * (this.maxWidth);
                    ry = random.NextDouble() * (this.maxHeight);

                    // Check against all existing objects
                    foreach (var loc in objectLocation) {
                        if (Math.Abs(rx - loc.x) < 48 && Math.Abs(ry - loc.y) < 48) {
                            validPosition = false;
                            break;
                        }
                    }
                } while (!validPosition);

                objectLocation.Add((rx, ry));
                lootObject lootTest = new lootObject(rx, ry);
                objectIndexMap.Add(lootTest, objectLocation.Count - 1);
                this.mapLayout.Children.Add(lootTest.gameObjectLayout);
            }

            //HEALTH LOOP
            for (int i = 0; i < 5; i++) {
                double rx, ry;
                bool validPosition;

                do {
                    validPosition = true;
                    rx = random.NextDouble() * (this.maxWidth);
                    ry = random.NextDouble() * (this.maxHeight);

                    // Check against all existing objects
                    foreach (var loc in objectLocation) {
                        if (Math.Abs(rx - loc.x) < 48 && Math.Abs(ry - loc.y) < 48) {
                            validPosition = false;
                            break;
                        }
                    }
                } while (!validPosition);

                objectLocation.Add((rx, ry));
                healthObject healthTest = new healthObject(rx, ry);
                objectIndexMap.Add(healthTest, objectLocation.Count - 1);
                this.mapLayout.Children.Add(healthTest.gameObjectLayout);
            }

            //ENEMY SPAWN
            spawnEnemies(3);

        }


    }
}
