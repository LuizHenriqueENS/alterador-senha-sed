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
    public static ChromeDriver IniciarSessao(string? arg1)
    {

        // baixa automaticamente a versão compatível do ChromeDrive para o pc
        new DriverManager().SetUpDriver(new ChromeConfig(), VersionResolveStrategy.MatchingBrowser);
        ChromeOptions options = new();
        options.AddArgument("silent");
        if(arg1 != null){
        options.AddArgument(arg1);
        }
        ChromeDriverService service = ChromeDriverService.CreateDefaultService();
        service.HideCommandPromptWindow = true;
        ChromeDriver driver = new ChromeDriver(service, options);

        return driver;
    }
    
    public static IWebElement Procurar(ChromeDriver driver, Tipos tipo, string caminho)
    {
        WebDriverWait wait = new WebDriverWait(driver, TimeSpan.FromSeconds(120.0)) { PollingInterval = TimeSpan.FromSeconds(3.0) };

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
         WebDriverWait wait = new WebDriverWait(driver, TimeSpan.FromSeconds(120.0)) { PollingInterval = TimeSpan.FromSeconds(3.0) };

        wait.Until(ExpectedConditions.InvisibilityOfElementLocated(By.XPath("/html/body/div[4]")));
    }
}