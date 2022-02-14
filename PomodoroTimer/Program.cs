using System;
using System.Threading;
using System.Windows.Forms;
using Microsoft.Toolkit.Uwp.Notifications;
using System.Collections.Generic;

namespace PomodoroTimer
{
    static class Program
    {
        static Mutex protector = new Mutex(true, "{F38B6B09-7E57-4627-BDAF-57EB3D332D27}");

        [STAThread]
        static void Main()
        {
            if (protector.WaitOne(TimeSpan.Zero, true))
            {
                Application.EnableVisualStyles();
                Application.SetCompatibleTextRenderingDefault(false);
                Application.Run(new PomodoroContext());
            }
            else
            {
                Application.Exit();
            }
        }

    }

    class PomodoroContext : ApplicationContext
    {
        const double MinuteInterval = 60000;

        NotifyIcon trayIcon;
        System.Timers.Timer mainTimer = null;
        bool Running = false;

        public PomodoroContext()
        {
            trayIcon = new NotifyIcon()
            {
                Icon = PomodoroTimer.AppIcon,
                ContextMenu = new ContextMenu(new MenuItem[] {
                    new MenuItem("Спринт", RunSprint),
                    new MenuItem("Стоп", StopSprint),
                    new MenuItem("-"),
                    new MenuItem("Настройки", ShowSettings),
                    new MenuItem("Выход", ExitProgram),
                }),
                Text = "Pomodoro Timer"
            };

            trayIcon.DoubleClick += ShowSettings;

            trayIcon.Visible = true;

            Settings.Load();
        }

        void ExitProgram(object sender, EventArgs e)
        {
            trayIcon.Visible = false;
            Running = false;

            Application.Exit();
        }

        void ShowSettings(object sender, EventArgs e)
        {
            SettingsForm settingsForm = new SettingsForm();

            settingsForm.Show();
        }

        void RunSprint(object sender, EventArgs e)
        {
            Running = true;

            Queue<SprintTimer> sprint = new Queue<SprintTimer>();

            foreach (SprintItem si in Settings.SprintItems)
            {
                sprint.Enqueue(new SprintTimer(si.WorkInterval, "Пора отвлечься", "Делу время - потехе час"));
                sprint.Enqueue(new SprintTimer(si.BreakInterval, "Спринт продолжается", "Успехов в работе"));
            }

            sprint.Enqueue(new SprintTimer(0, "Спринт завершён", "Хорошо поработали."));

            mainTimer = new System.Timers.Timer() { AutoReset = true, Interval = MinuteInterval };

            mainTimer.Elapsed += (s, args) =>
            {
                if (!Running)
                {
                    mainTimer.Stop();
                    mainTimer.Dispose();
                    sprint.Clear();

                    new ToastContentBuilder()
                        .AddText("Спринт прерван")
                        .AddText("Возвращайтесь снова")
                        .Show();

                    return;
                }

                if (sprint.Count != 0 && sprint.Peek().Interval-- == 0)
                {
                    sprint.Dequeue().ShowToast();
                }
            };

            new ToastContentBuilder()
                .AddText("Спринт запущен.")
                .AddText("Успехов в работе!")
                .Show();

            mainTimer.Start();
        }

        void StopSprint(object sender, EventArgs e)
        {
            Running = false;
        }
    }
}
