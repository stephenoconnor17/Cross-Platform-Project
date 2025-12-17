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
        myAudioPlayer drivingsfx_1;
        myAudioPlayer drivingsfx_2;

        public AudioHandler() {
            //SET UP BACKGROUND MUSIC HERE
            setUpMusicAudio();

            //SET UP SOUND EFFECTS HERE
            createSoundEffects();
        }

        public void setUpMusicAudio() {
            backgroundMusic = new myAudioPlayer();

            //ADD SONGS FOR BACKGROUND PLAYING HERE
            backgroundMusic.setUpMusicList();
            backgroundMusic.addToMusicList("d_song_t.mp3");
            backgroundMusic.addToMusicList("d_song_1.mp3");
            backgroundMusic.addToMusicList("d_song_2.mp3");
            backgroundMusic.addToMusicList("d_song_3.mp3");
            backgroundMusic.addToMusicList("d_song_4.mp3");

        }

        public void playBackgroundMusic() {
            backgroundMusic.playMusicFromList();
        }

        public void createSoundEffects() {
            lootsfx = new myAudioPlayer("d_loot.mp3");
            fuelsfx = new myAudioPlayer("d_fuel.mp3");
            collisionsfx = new myAudioPlayer("d_crash.mp3");
            healthsfx = new myAudioPlayer("d_health.mp3");
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
