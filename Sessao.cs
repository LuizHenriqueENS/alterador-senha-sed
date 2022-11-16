using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Support.UI;
using SeleniumExtras.WaitHelpers;
using WebDriverManager;
using WebDriverManager.DriverConfigs.Impl;
using WebDriverManager.Helpers;

class Sessao
{
    // inicia e configura sessao do ChromeDriver
    public static ChromeDriver IniciarSessao()
    {

        // baixa automaticamente a versão compatível do ChromeDrive para o pc
        new DriverManager().SetUpDriver(new ChromeConfig(), VersionResolveStrategy.MatchingBrowser);
        ChromeDriver driver = new ChromeDriver();

        return driver;
    }
    
    public static IWebElement Procurar(ChromeDriver driver, Tipos tipo, string caminho)
    {
        WebDriverWait wait = new WebDriverWait(driver, TimeSpan.FromSeconds(60.0)) { PollingInterval = TimeSpan.FromSeconds(5.0) };

        IWebElement? elemento = null;

        switch (tipo)
        {
            case Tipos.XPATH:
                elemento = wait.Until(x => x.FindElement(By.XPath(caminho)));
                break;
        }
        return elemento;
    }


    public static void BlockUI(ChromeDriver driver){
         WebDriverWait wait = new WebDriverWait(driver, TimeSpan.FromSeconds(60.0)) { PollingInterval = TimeSpan.FromSeconds(5.0) };

        wait.Until(ExpectedConditions.InvisibilityOfElementLocated(By.XPath("/html/body/div[4]")));
    }
}