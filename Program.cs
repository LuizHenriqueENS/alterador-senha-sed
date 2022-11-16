using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using System.Linq;

namespace AlteradorDeSenhaSED;
class Program
{
    static void Main(string[] args)
    {
        ChromeDriver driver = Sessao.IniciarSessao();

        driver.Navigate().GoToUrl("https://sed.educacao.sp.gov.br");
        FazerLoginNoSED(driver);
        EscolherEnsino(driver);
    }


    private static void JSExecutor(ChromeDriver driver, string comando)
    {
        IJavaScriptExecutor js = (IJavaScriptExecutor)driver;
        string title = (string)js.ExecuteScript(comando);
    }

    #region ESCOLHER ENSINO
    private static void EscolherEnsino(ChromeDriver driver)
    {
        // ir para turmas de alunos
        JSExecutor(driver, "document.querySelector('#decorAsidePopup > li:nth-child(2) > ul > li:nth-child(3) > ul > li > a').click();");
        Console.Clear();
        Sessao.BlockUI(driver);

        System.Console.WriteLine("Ensino disponíveis: ");

        var listaDeTiposDeEnsino = Sessao.Procurar(driver, Tipos.XPATH, "//*[@id='filt-tipoEnsino']");
        var tiposDeEnsinos = listaDeTiposDeEnsino.FindElements(By.TagName("option"));
        int n = 1;
        foreach (var item in tiposDeEnsinos)
        {
            string tipo = item.GetAttribute("innerHTML");
            if (tipo != "SELECIONE...")
            {
                System.Console.WriteLine($"[{n}] " + tipo);
                n++;
            }
        }
        System.Console.Write("\nQual deseja alterar? ");
        int escolhaEnsino = int.Parse(Console.ReadLine());

        JSExecutor(driver, $"document.querySelector('#bs-select-6-{escolhaEnsino}').click();");
        System.Console.WriteLine("Você selecionou " + tiposDeEnsinos[n - 1].GetAttribute("innerHTML"));
        System.Console.WriteLine($"\nDeseja alterar a senha de: \n[1] Todas as turmas do {tiposDeEnsinos[n - 1].GetAttribute("innerHTML")} \n[2] Apenas UMA turma");
    }
    #endregion
    #region LOGIN
    private static void FazerLoginNoSED(ChromeDriver driver)
    {
        // fazer o login
        System.Console.Write("Digite seu login: ");
        string? login = Console.ReadLine();

        while (login == "")
        {
            System.Console.Write("Você precisa digitar o login: ");
            login = Console.ReadLine();
        }

        var campoLogin = Sessao.Procurar(driver, Tipos.XPATH, "//*[@id='name']");
        campoLogin.SendKeys(login);

        // SENHA
        System.Console.Write("Digite seu senha: ");
        string? senha = Console.ReadLine();

        while (senha == "")
        {
            System.Console.Write("Você precisa digitar o login: ");
            senha = Console.ReadLine();
        }

        var campoSenha = Sessao.Procurar(driver, Tipos.XPATH, "//*[@id='senha']");
        campoSenha.SendKeys(senha);

        // confirmar e entrar
        var botao = Sessao.Procurar(driver, Tipos.XPATH, "//*[@id='botaoEntrar']");
        botao.Click();

        var modalCargosUL = Sessao.Procurar(driver, Tipos.XPATH, "//*[@id='sedUiModalWrapper_1body']/ul");
        var cargos = modalCargosUL.FindElements(By.TagName("li"));
        var proatec = cargos.First(x => x.FindElement(By.TagName("a")).GetAttribute("innerHTML") == "PROATEC");
        proatec.Click();

        System.Console.WriteLine("Login feito com sucesso!");
    }
    #endregion
}
