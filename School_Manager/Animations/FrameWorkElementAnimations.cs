using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media.Animation;

namespace School_Manager
{
    public static class FrameWorkElementAnimations
    {
        public static async Task IncreaseWidth(this FrameworkElement element, float seconds, double offset)
        {
            //Creare Storyboard
            var sb = new Storyboard();

            //Add slide from right animation
            sb.AddIncreaseWidth(seconds, offset);

            //Add fade in animation
            sb.AddFadeIn(seconds);

            //Start animating 
            sb.Begin(element);

            //wait for it to visible
            await Task.Delay((int)(seconds * 1000));
        }

        public static async Task DecreaseWidth(this FrameworkElement element, float seconds, double offset)
        {
            //Creare Storyboard
            var sb = new Storyboard();

            //Add slide from right animation
            sb.AddDecreaseWidth(seconds,offset);

            //Add fade in animation
            sb.AddFadeOut(seconds);

            //Start animating 
            sb.Begin(element);

            //wait for it to visible
            await Task.Delay((int)(seconds * 1000));
        }
    }
}
