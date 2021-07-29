using System.Collections.Generic;

namespace ABXY.Layers.Runtime.Nodes.Playback
{
    public interface AudioOutSource
    {
        AudioSettingsData GetAudioSettings(System.Guid eventID, Dictionary<string, object> parameters);
        void ReturnAudioSettings(System.Guid eventID);
    }
}