using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using System.Diagnostics;
using WebDriverManager;
using WebDriverManager.DriverConfigs.Impl;

Console.WriteLine("Notiz entferner startet jetzt...");

Console.WriteLine("Gib deine Email ein:");
var email = Console.ReadLine();

Console.WriteLine("Gib dein Passwort ein:");
var pw = Console.ReadLine();

new DriverManager().SetUpDriver(new ChromeConfig());

var driver = new ChromeDriver();

driver.Navigate().GoToUrl("https://read.amazon.com/notebook?ref_=kcr_notebook_lib&language=en-US");

var tbEmail = driver.FindElement(By.Id("ap_email"));
var tbPw = driver.FindElement(By.Id("ap_password"));

tbEmail.SendKeys(email);
tbPw.SendKeys(pw);

var submitButton = driver.FindElement(By.Id("signInSubmit"));
submitButton.Click();

driver.Manage().Timeouts().ImplicitWait = TimeSpan.FromMilliseconds(500);

var source = driver.PageSource;

if (source.Contains("Two-Step Verification"))
{
    Thread.Sleep(250);
    Console.WriteLine("Bitte gib den 2FA ein:");
    Thread.Sleep(500);
    var twoFa = Console.ReadLine();

    var tb2Fa = driver.FindElement(By.Id("auth-mfa-otpcode"));
    tb2Fa.SendKeys(twoFa);

    var twoBtn = driver.FindElement(By.Id("auth-signin-button"));
    twoBtn.Click();
    Thread.Sleep(1500);
}

driver.Manage().Timeouts().ImplicitWait = TimeSpan.FromMilliseconds(2000);

Thread.Sleep(1500);

source = driver.PageSource; // refresh source

if (source.Contains("Your Notes and Highlights"))
{
    var library = driver.FindElement(By.Id("kp-notebook-library"));
    var books = library.FindElements(By.ClassName("kp-notebook-library-each-book"));
    foreach (var book in books)
    {
        var span = book.FindElement(By.TagName("span"));
        var a = span.FindElement(By.TagName("a"));
        var h2 = a.FindElement(By.TagName("h2")).Text;

        Console.WriteLine($"Löschen der Notizen von {h2}");
        Thread.Sleep(500);
        a.Click();
        Thread.Sleep(1500);

        Debug.Print(driver.FindElement(By.Id("kp-notebook-highlights-count")).Text);

        var countHighlights = Convert.ToInt32(driver.FindElement(By.Id("kp-notebook-highlights-count")).Text);
        if (countHighlights > 0)
        {

            var rows = driver.FindElements(By.XPath("(//div[@class='a-column a-span10 kp-notebook-row-separator'])"));

            for (int i = 1; i < rows.Count; i++)
            {
                Thread.Sleep(500);
                var optionslink = rows[i].FindElement(By.XPath("(//a[@role='button'][normalize-space()='Options'])"));
                Thread.Sleep(500);
                optionslink.Click();
                Thread.Sleep(500);
                var deleteMenu = driver.FindElement(By.XPath("//div[@aria-busy='true']//div[@class='a-popover-inner']"));
                Thread.Sleep(500);
                try
                {
                    var deleteLink = deleteMenu.FindElement(By.XPath("//a[normalize-space()='Delete highlight']"));
                    Thread.Sleep(500);
                    deleteLink.Click();
                    Thread.Sleep(500);
                    var deleteBtn = driver.FindElement(By.XPath("//input[@aria-labelledby='deleteHighlight-announce']"));
                    Thread.Sleep(250);
                    if (deleteBtn != null)
                    {
                        Thread.Sleep(250);
                        deleteBtn.Click();
                    }
                }
                catch
                {
                }

                try
                {
                    var deleteLink = deleteMenu.FindElement(By.XPath("//a[normalize-space()='Delete note']"));
                    Thread.Sleep(500);
                    deleteLink.Click();
                    Thread.Sleep(500);
                    var deleteBtn = driver.FindElement(By.XPath("//input[@aria-labelledby='deleteNote-announce']"));
                    Thread.Sleep(250);
                    if (deleteBtn != null)
                    {
                        Thread.Sleep(250);
                        deleteBtn.Click();
                    }
                }
                catch
                {
                }
            }
        }
        Thread.Sleep(1500);
    }
}