using Android.Content;
using Android.Media;

namespace Origami
{
    public class Audio
    {
        MediaPlayer ambientPlayer;
        MediaPlayer clickPlayer;

        public Audio(Context context)
        {
            ambientPlayer = MediaPlayer.Create(context, Resource.Raw.ambient);
            ambientPlayer.Looping = true;
            ambientPlayer.Start();

            clickPlayer = MediaPlayer.Create(context, Resource.Raw.click);
        }

        bool mute_ambient = false;
        bool mute_click = false;

        public void SetAmbientEnabled(bool enabled)
        {
            mute_ambient = !enabled;
            if (mute_ambient)
                PauseAmbient();
            else
                ambientPlayer.Start();
        }

        public void SetClickEnabled(bool enabled)
        {
            mute_click = !enabled;
        }

        public void ResumeAmbient()
        {
            if(!mute_ambient)
                ambientPlayer.Start();
        }

        public void PauseAmbient()
        {
            ambientPlayer.Pause();
        }

        public void PlayClick()
        {
            if(!mute_click)
                clickPlayer.Start();
        }
    }
}