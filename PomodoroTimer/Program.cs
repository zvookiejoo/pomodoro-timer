using System;
using System.Threading;
using System.Windows.Forms;
using Microsoft.Toolkit.Uwp.Notifications;
using System.Collections.Generic;

namespace PomodoroTimer
{
    static class Program
    {
        // Для запрета запуска нескольких копий приложения.
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
        MenuItem runSprint;
        MenuItem stopSprint;
        ContextMenu trayIconMenu;

        System.Timers.Timer mainTimer = null;
        bool Running = false;

        public PomodoroContext()
        {
            runSprint = new MenuItem("Спринт", RunSprint);
            stopSprint = new MenuItem("Стоп", StopSprint) { Enabled = false };

            trayIconMenu = new ContextMenu(new MenuItem[] {
                runSprint,
                stopSprint,
                new MenuItem("-"),
                new MenuItem("Настройки", ShowSettings),
                new MenuItem("Выход", ExitProgram),
            });

            trayIcon = new NotifyIcon()
            {
                Icon = PomodoroTimer.AppIcon,
                ContextMenu = trayIconMenu,
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
            if (Running) return;

            Running = true;
            stopSprint.Enabled = true;
            runSprint.Enabled = false;

            Queue<SprintTimer> sprint = new Queue<SprintTimer>();

            foreach (SprintItem si in Settings.SprintItems)
            {
                sprint.Enqueue(new SprintTimer(si.WorkInterval, "Пора отвлечься", "Делу время - потехе час"));
                sprint.Enqueue(new SprintTimer(si.BreakInterval, "Спринт продолжается", "Успехов в работе"));
            }

            mainTimer = new System.Timers.Timer() { AutoReset = true, Interval = MinuteInterval };

            mainTimer.Elapsed += (s, args) =>
            {
                if (sprint.Count != 0 && sprint.Peek().Interval-- == 0)
                {
                    sprint.Dequeue().ShowToast();

                    if (sprint.Count == 0)
                    {
                        StopSprint(null, null);
                    }
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
            runSprint.Enabled = true;
            stopSprint.Enabled = false;

            new ToastContentBuilder()
                .AddText("Спринт завершён")
                .AddText("Возвращайтесь снова")
                .Show();

            mainTimer.Enabled = false;
            mainTimer.Dispose();
        }
    }
}
