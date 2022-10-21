using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Animation;

namespace School_Manager
{
    public static class PageAnimations
    {
        /// <summary>
        /// Slides a page in from right
        /// </summary>
        /// <param name="page"></param>
        /// <param name="seconds"></param>
        /// <returns></returns>
        public static async Task SlideAndFadeFromRight(this Page page,float seconds)
        {
            //Creare Storyboard
            var sb = new Storyboard();

            //Add slide from right animation
            sb.AddSlideFromRight(seconds, page.WindowWidth);

            //Add fade in animation
            sb.AddFadeIn(seconds);

            //Start animating 
            sb.Begin(page);

            //make page visible
            page.Visibility = Visibility.Visible;

            //wait for it to visible
            await Task.Delay((int)(seconds * 1000));
        }

        /// <summary>
        /// Slides a page out to the left
        /// </summary>
        /// <param name="page"></param>
        /// <param name="seconds"></param>
        /// <returns></returns>
        public static async Task SlideAndFadeOutToLeft(this Page page, float seconds)
        {
            //Creare Storyboard
            var sb = new Storyboard();

            //Add slide from right animation
            sb.AddSlideToLeft(seconds, page.WindowWidth);

            //Add fade in animation
            sb.AddFadeOut(seconds);

            //Start animating 
            sb.Begin(page);

            //make page visible
            page.Visibility = Visibility.Visible;

            //wait for it to visible
            await Task.Delay((int)(seconds * 1000));
        }

    }
}
