using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using System;
using System.IO;
using System.Threading;
using System.Linq;
using OpenQA.Selenium.Support.UI;
using SeleniumExtras.WaitHelpers;

/* 
       │ Author       : extatent
       │ Name         : AutomatedBot
       │ GitHub       : https://github.com/extatent
*/

namespace AutomatedBot
{
    class Program
    {
        static string id;
        static string pass;
        static string service;

        static void Main(string[] args)
        {
            Console.Title = "AutomatedBot | extatent";
            Console.WriteLine("GitHub: https://github.com/extatent");
            Console.WriteLine("You must have a registered account with atleast 1 video added on the services.");

            if (!File.Exists("chromedriver.exe"))
            {
                Console.WriteLine("chromedriver wasn't found. Download chromedriver and put it in the Sub Bot folder's path. URL: https://chromedriver.chromium.org/downloads");
                return;
            }

            if (!File.Exists("login.txt"))
            {
                Console.Write("YouTube Channel URL/ID: ");
                id = Console.ReadLine();

                if (id.Contains("https://www.youtube.com/channel/"))
                {
                    id = id.Replace("https://www.youtube.com/channel/", "");
                }

                Console.Write("Registration Password: ");
                pass = Console.ReadLine();

                File.Create("login.txt").Dispose();
                File.WriteAllText("login.txt", $"{id}|{pass}");
            }
            else
            {
                id = File.ReadAllText("login.txt").Split('|')[0];
                pass = File.ReadAllText("login.txt").Split('|')[1];
            }

            Console.Write("[1] SubPals [2] YTpals [3] SoNuker: ");
            string choice = Console.ReadLine();

            if (choice == "1")
            {
                service = "subpals";
            }
            else if (choice == "2")
            {
                service = "ytpals";
            }
            else if (choice == "3")
            {
                service = "sonuker";
            }
            else
            {
                Console.WriteLine("Wrong key. Press any key to exit.");
                Console.ReadKey();
                Environment.Exit(0);
            }

            Start();
            Console.ReadKey();
        }

        static void Start()
        {
            try
            {
                Console.Clear();
                Console.WriteLine("Wait");

                // Disabling logs
                ChromeDriverService service = ChromeDriverService.CreateDefaultService();
                service.EnableVerboseLogging = false;
                service.SuppressInitialDiagnosticInformation = true;
                service.HideCommandPromptWindow = true;

                // Arguments
                ChromeOptions options = new ChromeOptions();
                options.AddArgument("--disable-logging");
                options.AddArguments("--mute-audio");
                options.AddArguments("--disable-extensions");
                options.AddArguments("--disable-notifications");
                options.AddArguments("--disable-application-cache");
                options.AddArguments("--no-sandbox");
                options.AddArgument("--disable-crash-reporter");
                options.AddArguments("--disable-dev-shm-usage");
                options.AddArguments("--disable-gpu");
                options.AddArgument("--ignore-certificate-errors");
                options.AddArguments("--disable-infobars");
                options.AddArgument("--window-size=1920,1080");
                options.AddArgument("--start-maximized");
                options.AddArgument("--headless");
                options.AddArgument("--silent");

                // Setupping the driver and the url
                IWebDriver driver = new ChromeDriver(service, options);
                driver.Url = $"https://www.{Program.service}.com/login/final/{id}/";

                // Logging in
                try
                {
                    driver.FindElement(By.Name("password")).SendKeys(pass);
                    driver.FindElement(By.XPath("//button[@type='submit']")).Click();
                    Console.Clear();
                    Console.WriteLine("Logged in");
                }
                catch
                {
                    Console.WriteLine("Failed to login. Press any key to exit.");
                    Console.ReadKey();
                    Environment.Exit(0);
                }

                // Activating the plan
                try
                {
                    if (Program.service == "sonuker")
                    {
                        IJavaScriptExecutor executor = driver as IJavaScriptExecutor;
                        var button = driver.FindElement(By.XPath("/html/body/div[1]/section/div/div[1]/div[2]/div/div/div[1]/div[2]/div[1]/div/div[2]/div[2]/form/a"));
                        executor.ExecuteScript("arguments[0].click();", button);
                    }
                    else
                    {
                        IJavaScriptExecutor executor = driver as IJavaScriptExecutor;
                        var button = driver.FindElement(By.XPath("/html/body/div[1]/section/div/div[1]/div[2]/div/div/div[2]/div[2]/div[1]/div/div[2]/div[2]/form/a"));
                        executor.ExecuteScript("arguments[0].click();", button);
                    }
                    Console.WriteLine("Activated plan");
                }
                catch
                {
                    Console.WriteLine("Plan is already activated, continuing");
                }

                // Pressing the red button, switching back to the default page, waiting X seconds and pressing the green button, repeating the same X times

                if (Program.service == "sonuker")
                {
                    var count = int.Parse(driver.FindElement(By.XPath("/html/body/div[1]/section/div/div/div/div/div/div/div[1]/h2/span/div")).Text);

                    foreach (int times in Enumerable.Range(0, count))
                    {
                        Console.WriteLine($"Videos remaining: {int.Parse(driver.FindElement(By.XPath("/html/body/div[1]/section/div/div/div/div/div/div/div[1]/h2/span/div")).Text)}");

                        var wait = new WebDriverWait(driver, TimeSpan.FromSeconds(20));

                        try
                        {
                            driver.FindElement(By.XPath("/html/body/div[1]/section/div/div/div/div/div/div/div[2]/div[2]/div/div[2]/div[1]/a")).Click();
                            driver.SwitchTo().Window(driver.WindowHandles.First());
                            var time = int.Parse(driver.FindElement(By.XPath("/html/body/div[1]/section/div/div/div/div/div/div/div[1]/h3/span/font")).Text);
                            var ms = time * 1000;
                            Thread.Sleep(ms);
                            driver.FindElement(By.XPath("/html/body/div[1]/section/div/div/div/div/div/div/div[2]/div[2]/div/div[2]/div[3]/a")).Click();
                            wait.Until(ExpectedConditions.ElementToBeClickable(By.XPath("/html/body/div[1]/section/div/div/div/div/div/div/div[2]/div[2]/div/div[2]/div[1]/a")));
                        }
                        catch
                        {
                            Console.WriteLine("Done. Press any key to exit.");
                            driver.Quit();
                            Console.ReadKey();
                            Environment.Exit(0);
                        }
                    }
                }
                else
                {
                    var count = int.Parse(driver.FindElement(By.XPath("/html/body/div[1]/section/div/div/div/div/div/div[2]/div[1]/h2/span/div")).Text);

                    foreach (int times in Enumerable.Range(0, count))
                    {
                        Console.WriteLine($"Videos remaining: {int.Parse(driver.FindElement(By.XPath("/html/body/div[1]/section/div/div/div/div/div/div[2]/div[1]/h2/span/div")).Text)}");

                        var wait = new WebDriverWait(driver, TimeSpan.FromSeconds(20));

                        try
                        {
                            driver.FindElement(By.XPath("/html/body/div[1]/section/div/div/div/div/div/div[2]/div[2]/div[2]/div/div[2]/div[1]/a")).Click();
                            driver.SwitchTo().Window(driver.WindowHandles.First());
                            var time = int.Parse(driver.FindElement(By.XPath("/html/body/div[1]/section/div/div/div/div/div/div[2]/div[1]/h3/span/font")).Text);
                            var ms = time*1000;
                            Thread.Sleep(ms);
                            driver.FindElement(By.XPath("/html/body/div[1]/section/div/div/div/div/div/div[2]/div[2]/div[2]/div/div[2]/div[3]/a")).Click();
                            wait.Until(ExpectedConditions.ElementToBeClickable(By.XPath("/html/body/div[1]/section/div/div/div/div/div/div[2]/div[2]/div[2]/div/div[2]/div[1]/a")));
                        }
                        catch
                        {
                            Console.WriteLine("Done. Press any key to exit.");
                            driver.Quit();
                            Console.ReadKey();
                            Environment.Exit(0);
                        }
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                Console.ReadKey();
                Environment.Exit(0);
            }
        }
    }
}
