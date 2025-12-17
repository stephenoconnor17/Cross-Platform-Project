using Plugin.Maui.Audio;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CROSSPLATFORM2DGAME {
    internal class myAudioPlayer {
        private IAudioPlayer backgroundMusicPlayer;
        private string filename;
        private List<string> musicList;
        double currentVolume = 1.0;



        public myAudioPlayer() { }

        public async Task InitializeAsync(string filename) {
            this.filename = filename;
            backgroundMusicPlayer = AudioManager.Current.CreatePlayer(await FileSystem.OpenAppPackageFileAsync(filename));
            backgroundMusicPlayer.Volume = currentVolume;

        }

        public void setUpMusicList() {
            this.musicList = new List<string>();
        }

        public void setVolume(double volume) {
            currentVolume = volume;
            if (backgroundMusicPlayer != null)
                backgroundMusicPlayer.Volume = currentVolume;
        }

        public void addToMusicList(string filename) {
            this.musicList.Add(filename);
        }

        int indexer = 0;
        public void playMusicFromList() {
            if(musicList.Count == 0) { //no music to play, which shouldn't happen >:(
                return;
            }
            backgroundMusicPlayer = AudioManager.Current.CreatePlayer(FileSystem.OpenAppPackageFileAsync(musicList[indexer]).Result);
            backgroundMusicPlayer.Volume = currentVolume;
            backgroundMusicPlayer.PlaybackEnded += (s, e) => playBack();
            backgroundMusicPlayer.Play();

        }

        public void playBack() {
            if (indexer == musicList.Count - 1) {
                indexer = 0;
            } else {
                indexer++;
            }

            if (musicList.Count >= 1) {
                
                backgroundMusicPlayer = AudioManager.Current.CreatePlayer(FileSystem.OpenAppPackageFileAsync(musicList[indexer]).Result);
                backgroundMusicPlayer.PlaybackEnded += (s, e) => playBack(); //recursion! I always thought it was a gimmick.
                backgroundMusicPlayer.Volume = currentVolume;
                backgroundMusicPlayer.Play();
            }
        }

        public void setFileName(string filename) {
            this.filename = filename;
        }

        //We have load here because we want a sound effect to be preloaded before playing it.

        public void PlaySoundEffect() {
            backgroundMusicPlayer.Play();
        }

    }
}
