using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace CROSSPLATFORM2DGAME {
    internal class AudioHandler {
        myAudioPlayer backgroundMusic;
        myAudioPlayer lootsfx;
        myAudioPlayer fuelsfx;
        myAudioPlayer collisionsfx;
        myAudioPlayer healthsfx;
        public double musicVolume = 1.0;
        public double soundEffectsVolume = 1.0;
        //myAudioPlayer drivingsfx_1;
        //myAudioPlayer drivingsfx_2;

        public AudioHandler() {
        }

        public void setMusicVolume(double volume) {
            musicVolume = volume;
            backgroundMusic.setVolume(musicVolume);
        }

        public void setSoundEffectsVolume(double volume) {
            soundEffectsVolume = volume;
            lootsfx.setVolume(volume);
            fuelsfx.setVolume(volume);
            collisionsfx.setVolume(volume);
            healthsfx.setVolume(volume);
        }

        public void playBackgroundMusic() {
            backgroundMusic.playMusicFromList();
        }

        public async Task InitializeAsync() {
            backgroundMusic = new myAudioPlayer();
            backgroundMusic.setUpMusicList();
            backgroundMusic.addToMusicList("d_song_t.mp3");
            backgroundMusic.addToMusicList("d_song_1.mp3");
            backgroundMusic.addToMusicList("d_song_2.mp3");
            backgroundMusic.addToMusicList("d_song_3.mp3");
            backgroundMusic.addToMusicList("d_song_4.mp3");
            

            lootsfx = new myAudioPlayer();
            await lootsfx.InitializeAsync("d_loot.mp3");

            fuelsfx = new myAudioPlayer();
            await fuelsfx.InitializeAsync("d_fuel.mp3");

            collisionsfx = new myAudioPlayer();
            await collisionsfx.InitializeAsync("d_crash.mp3");

            healthsfx = new myAudioPlayer();
            await healthsfx.InitializeAsync("d_health.mp3");

            setMusicVolume(1);
            setSoundEffectsVolume(1);
        }

        public void playSoundEffect(string type) {
            if(type == "loot") {
                lootsfx.PlaySoundEffect();
            } else if(type == "fuel") {
                fuelsfx.PlaySoundEffect();
            } else if(type == "collision") {
                collisionsfx.PlaySoundEffect();
            } else if(type == "health") {
                healthsfx.PlaySoundEffect();
            }
        }
    }
}
