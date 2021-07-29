using System;
using System.Collections.Generic;
using ABXY.Layers.Runtime;
using ABXY.Layers.Runtime.Nodes.Playback;

namespace ABXY.Layers.Editor.Timeline_Editor.Variants.PlayNode
{
    public class VirtualAudioOut : AudioOutSource
    {
        public AudioSettingsData GetAudioSettings(Guid eventID, Dictionary<string, object> parameters)
        {
            return AudioSettingsData.defaultAudioSettings;
        }

        public void ReturnAudioSettings(Guid eventID)
        {

        }
    }
}
