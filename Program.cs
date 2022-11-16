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
    }

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
