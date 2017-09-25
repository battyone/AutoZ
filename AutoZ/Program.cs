using System;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using TestStack.White;
using TestStack.White.Factory;
using TestStack.White.UIItems;
using TestStack.White.UIItems.Finders;
using TestStack.White.UIItems.WindowItems;

namespace AutoZ
{
    class Program
    {
        static void Main(string[] args)
        {

            try
            {
                string StartAtUSTime = ConfigurationManager.AppSettings["StartAtUSTime"];
                var splitTime = StartAtUSTime.Split(':');
                DateTime estTime = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, int.Parse(splitTime[0]), int.Parse(splitTime[1]), 0);
                TimeZoneInfo estZone = TimeZoneInfo.FindSystemTimeZoneById("Eastern Standard Time");
                TimeZoneInfo cstZone = TimeZoneInfo.FindSystemTimeZoneById(TimeZone.CurrentTimeZone.StandardName);
                DateTime yourLocalTime = TimeZoneInfo.ConvertTime(estTime, estZone, cstZone);
                Console.WriteLine("Waiting US Time Open Market at " + yourLocalTime.ToShortTimeString());

                if (bool.Parse(ConfigurationManager.AppSettings["DebugSkipWait"]) == false)
                {
                    while (DateTime.Now <= yourLocalTime)
                        Thread.Sleep(60000);
                }

                //Application application = Application.Launch(@"E:\Zorro\Zorro.exe -c IBG REAL");
                System.Diagnostics.ProcessStartInfo si = new ProcessStartInfo();
                //si.Arguments = "-trade Z8 -c IBG REAL";
                si.Arguments = "-c " + ConfigurationManager.AppSettings["Account"];
                si.FileName = ConfigurationManager.AppSettings["ZorroExe"];
                Application application = Application.AttachOrLaunch(si);
                List<Window> windows = application.GetWindows();




                if (windows.Count == 1)
                {

                    TestStack.White.UIItems.ListBoxItems.ComboBox PluginComboBox = windows[0].Get<TestStack.White.UIItems.ListBoxItems.ComboBox>(SearchCriteria.ByClassName("ComboBox").AndIndex(1));
                    PluginComboBox.Select(ConfigurationManager.AppSettings["Plugin"]);

                    TestStack.White.UIItems.ListBoxItems.ComboBox StrategyComboBox = windows[0].Get<TestStack.White.UIItems.ListBoxItems.ComboBox>(SearchCriteria.ByClassName("ComboBox").AndIndex(2));
                    StrategyComboBox.Select(ConfigurationManager.AppSettings["Strategy"]);


                    Thread.Sleep(2000);

                    Button btnTest = windows[0].Get<Button>("Test");
                    try
                    {
                        if (btnTest != null)
                            btnTest.Click();
                        Console.WriteLine("Test Started");
                    }
                    catch (Exception)
                    {
                    }

                    Thread.Sleep(2000);


                    try
                    {
                        Button btnStop = windows[0].Get<Button>("Stop");
                        if (btnStop != null)
                            btnStop.Click();
                        Console.WriteLine("Test Stopped");
                    }
                    catch (Exception)
                    {
                    }

                    Thread.Sleep(2000);
                    TextBox CapitalTextBox = windows[0].Get<TextBox>(SearchCriteria.ByClassName("Edit").AndIndex(4));
                    CapitalTextBox.Text = ConfigurationManager.AppSettings["Capital"];
                    Console.WriteLine("Capital Set to " + ConfigurationManager.AppSettings["Capital"]);

                    Button btnTrade = windows[0].Get<Button>("Trade");
                    try
                    {
                        if (btnTrade != null)
                            btnTrade.Click();
                        Console.WriteLine("Trade Started");
                    }
                    catch (Exception)
                    {
                    }


                    Thread.Sleep(2000);

                    List<Window> modalWindows = windows[0].ModalWindows();
                    while (modalWindows.Count() == 0)
                    {
                        modalWindows = windows[0].ModalWindows();
                        Thread.Sleep(1000);
                    }

                    if (modalWindows.Count() == 1)
                    {
                        Button btnYes = modalWindows[0].Get<Button>("Yes");
                        btnYes.Click();
                        Console.WriteLine("**************  Yes Clicked");
                    }


                }
            }
            catch(Exception ex)
            {
                Console.WriteLine("Error " + ex.Message + " " + ex.InnerException + " " + ex.StackTrace);
            }



        }
    }
}
