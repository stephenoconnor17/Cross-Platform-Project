using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CROSSPLATFORM2DGAME {
    internal class gameSaveData {
        public int HighestScore { get; set; }
        public double MusicVolume { get; set; }
        public double sfxVolume { get; set; }

        public gameSaveData() {
            HighestScore = 0;
            MusicVolume = 1.0;
            sfxVolume = 1.0;
        }

    }
}
