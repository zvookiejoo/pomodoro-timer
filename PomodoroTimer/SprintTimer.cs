using Microsoft.Toolkit.Uwp.Notifications;

namespace PomodoroTimer
{
    class SprintTimer
    {
        public int Interval;
        ToastContentBuilder toast;
        
        public SprintTimer(int minutes, string caption, string text)
        {
            Interval = minutes;

            toast = new ToastContentBuilder();

            toast.AddText(caption);
            toast.AddText(text);
        }

        public void ShowToast()
        {
            toast.Show();
        }
    }
}
