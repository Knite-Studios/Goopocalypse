using UnityEngine;

namespace Utils
{
    public static class PlayerPrefsUtil
    {
        public static float MusicVolume
        {
            get => PlayerPrefs.GetFloat("MusicVolume", 1.0f);
            set
            {
                PlayerPrefs.SetFloat("MusicVolume", value);
                PlayerPrefs.Save();
            }
        }

        public static float SoundFxVolume
        {
            get => PlayerPrefs.GetFloat("SoundFxVolume", 1.0f);
            set
            {
                PlayerPrefs.SetFloat("SoundFxVolume", value);
                PlayerPrefs.Save();
            }
        }
    }
}
