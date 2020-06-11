using Android.App;
using Android.Content;
using Android.Gms.Ads;
using Android.Gms.Ads.Rewarded;

namespace Origami
{
    public static class Ads
    {
        const string interstial_id = "ca-app-pub-3824185688302776/4118893272";
        const string rewarded_id = "ca-app-pub-3824185688302776/8199838582";

        static RewardedAd rewardedAd;
        static Activity rewardedAdsActivity;

        static InterstitialAd interestitialAd;

        public static void Init(Context context)
        {
            interestitialAd = new InterstitialAd(context);
            interestitialAd.AdUnitId = interstial_id;
            interestitialAd.LoadAd(new AdRequest.Builder().Build());
            interestitialAd.RewardedVideoAdClosed += InterestitialClosed;
        }

        static void InterestitialClosed(object sender, System.EventArgs e)
        {
            MainMenuActivity.audioPlayer.ResumeAmbient();
            interestitialAd.LoadAd(new AdRequest.Builder().Build());
        }

        public static void ShowInterestitial()
        {
            if (interestitialAd.IsLoaded)
            {
                MainMenuActivity.audioPlayer.PauseAmbient();
                interestitialAd.Show();
            }
            else
                interestitialAd.LoadAd(new AdRequest.Builder().Build());
        }

        class RewardAdLoadCallback : RewardedAdLoadCallback
        {
            public override void OnRewardedAdLoaded()
            {
                MainMenuActivity.audioPlayer.PauseAmbient();
                rewardedAd.Show(rewardedAdsActivity, new RewardAdCallback());
            }

            public override void OnRewardedAdFailedToLoad(int p0)
            {
                GameActivity.Instance.AdState = GameActivity.RewardAdState.FAIILED;
            }
        }

        class RewardAdCallback : RewardedAdCallback
        {
            public override void OnUserEarnedReward(IRewardItem p0)
            {
                GameActivity.Instance.AdState = GameActivity.RewardAdState.REWARDED;
                was_earned_reward = true;
            }

            static bool was_earned_reward = false;

            public override void OnRewardedAdClosed()
            {
                MainMenuActivity.audioPlayer.ResumeAmbient();

                if (!was_earned_reward)
                    GameActivity.Instance.AdState = GameActivity.RewardAdState.FAIILED;

                was_earned_reward = false;
            }

            public override void OnRewardedAdFailedToShow(int p0)
            {
                MainMenuActivity.audioPlayer.ResumeAmbient();
                GameActivity.Instance.AdState = GameActivity.RewardAdState.FAIILED;
            }
        }

        public static void ShowAds(Activity ads_activity)
        {
            rewardedAdsActivity = ads_activity;

            rewardedAd = new RewardedAd(ads_activity, rewarded_id);

            GameActivity.Instance.AdState = GameActivity.RewardAdState.LOADING;
            rewardedAd.LoadAd(new AdRequest.Builder().Build(), new RewardAdLoadCallback());
        }
    }
}