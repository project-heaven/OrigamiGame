using Android.Content;
using Android.Media;

namespace Origami
{
    public class Audio
    {
        MediaPlayer ambientPlayer;
        MediaPlayer clickPlayer;
        MediaPlayer losePlayer;
        MediaPlayer winPlayer;
        MediaPlayer scrollPlayer;

        public Audio(Context context)
        {
            ambientPlayer = MediaPlayer.Create(context, Resource.Raw.ambient);
            ambientPlayer.Looping = true;
            ambientPlayer.Start();

            clickPlayer = MediaPlayer.Create(context, Resource.Raw.click);
            losePlayer = MediaPlayer.Create(context, Resource.Raw.lose);
            winPlayer = MediaPlayer.Create(context, Resource.Raw.win);
            scrollPlayer = MediaPlayer.Create(context, Resource.Raw.scroll);
        }

        bool mute_ambient = false;
        bool mute_sfx = false;

        public void PlayScroll()
        {
            if (!mute_sfx)
                scrollPlayer.Start();
        }

        public void PlayLose()
        {
            if (!mute_sfx)
                losePlayer.Start();
        }

        public void PlayWin()
        {
            if (!mute_sfx)
                winPlayer.Start();
        }

        public void SetAmbientEnabled(bool enabled)
        {
            mute_ambient = !enabled;
            if (mute_ambient)
                PauseAmbient();
            else
                ambientPlayer.Start();
        }

        public void SetSfxEnabled(bool enabled)
        {
            mute_sfx = !enabled;
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
            if(!mute_sfx)
                clickPlayer.Start();
        }
    }
}