using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;


namespace AlteradorDeSenhaSED;
class Program
{
    static void Main(string[] args)
    {
        ChromeDriver driver = Sessao.IniciarSessao(null);

        driver.Navigate().GoToUrl("https://sed.educacao.sp.gov.br");
        FazerLoginNoSED(driver);
        EscolherEnsino(driver);
        driver.Quit();
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
        int nTipos = 1;
        foreach (var item in tiposDeEnsinos)
        {
            string tipo = item.GetAttribute("innerHTML");
            if (tipo != "SELECIONE...")
            {
                System.Console.WriteLine($"[{nTipos}] " + tipo);
                nTipos++;
            }
        }
        System.Console.Write("\nQual deseja alterar? ");
        int escolhaEnsino = int.Parse(Console.ReadLine());

        JSExecutor(driver, $"document.querySelector('#bs-select-6-{escolhaEnsino}').click();");
        System.Console.WriteLine("------------");
        System.Console.WriteLine("Você selecionou " + tiposDeEnsinos[escolhaEnsino].GetAttribute("innerHTML"));
        System.Console.WriteLine($"\nDeseja alterar a senha de: \n[1] Todas as turmas do {tiposDeEnsinos[escolhaEnsino].GetAttribute("innerHTML")} \n[2] Apenas UMA turma");
        System.Console.Write("\nOpção: ");
        int escolhaMetodo = int.Parse(Console.ReadLine());
        System.Console.WriteLine("");

        switch (escolhaMetodo)
        {
            case 1:
                System.Console.WriteLine("Não implementado ainda!");
                driver.Quit();
                break;
            case 2:
                #region Mostrar Turmas
                var listaDeTurmas = Sessao.Procurar(driver, Tipos.XPATH, "//*[@id='filt-turma']");
                var turmas = listaDeTurmas.FindElements(By.TagName("option"));
                int nTurmas = 1;
                foreach (var item in turmas)
                {
                    string turma = item.GetAttribute("innerHTML");
                    if (turma != "SELECIONE...")
                    {
                        System.Console.WriteLine($"[{nTurmas}] " + turma);
                        nTurmas++;
                    }
                }
                #endregion

                System.Console.Write("\nSelecione a turma que deseja alterar a senha: ");
                int escolhaTurma = int.Parse(Console.ReadLine());

                System.Console.WriteLine($"\nVocê selecionou: {turmas[escolhaTurma].GetAttribute("innerHTML")}");
                System.Console.WriteLine("--------------\nAguarde enquanto o programa esta executando...");
                JSExecutor(driver, $"document.querySelector('#bs-select-7-{escolhaTurma}').click();");
                JSExecutor(driver, $"document.querySelector('#btnPesquisar').click();");

                var filtroTurma = Sessao.Procurar(driver, Tipos.XPATH, "//*[@id='tabelaDados_filter']/label/input");
                filtroTurma.SendKeys("ativo");
                Sessao.Procurar(driver, Tipos.XPATH, "//*[@id='tabelaDados_length']/label/select").Click();
                Sessao.Procurar(driver, Tipos.XPATH, "//*[@id='tabelaDados_length']/label/select/option[4]").Click();

                Thread.Sleep(1500);

                List<string> alunos = new();
                List<string> senhas = new();

                //pegar tabela
                var tabelaAlunos = Sessao.Procurar(driver, Tipos.XPATH, @"//*[@id='tabelaDados']/tbody");
                var resetar = tabelaAlunos.FindElements(By.ClassName("colResetSenha"));
                for (int i = 0; i < resetar.Count; i++)
                {
                    resetar[i].FindElement(By.TagName("a")).Click();
                    Sessao.Procurar(driver, Tipos.XPATH, "/html/body/div[5]/section/div/div[2]/button[1]").Click();
                    // RA e SENHA
                    string SenhaERA = Sessao.Procurar(driver, Tipos.XPATH, "/html/body/div[5]/section/div/div[1]/div[2]").Text;
                    string RAAlunos = SenhaERA.Substring(20, 16);
                    string senhaAluno = SenhaERA.Substring(59, 7);
                    alunos.Add(RAAlunos);
                    senhas.Add(senhaAluno);

                    Sessao.Procurar(driver, Tipos.XPATH, "/html/body/div[5]/section/div/div[2]/button").Click();
                }

                Console.Clear();
                System.Console.WriteLine("Iniciando a alteração de senhas em massa");

                for (int i = 0; i < alunos.Count; i++)
                {
                    ChromeDriver driverAluno = Sessao.IniciarSessao("headless");
                    driverAluno.Navigate().GoToUrl("https://sed.educacao.sp.gov.br");

                    var campoLogin = Sessao.Procurar(driverAluno, Tipos.XPATH, "//*[@id='name']");
                    campoLogin.SendKeys(alunos[i]);
                    var campoSenha = Sessao.Procurar(driverAluno, Tipos.XPATH, "//*[@id='senha']");
                    campoSenha.SendKeys(senhas[i]);

                    var botao = Sessao.Procurar(driverAluno, Tipos.XPATH, "//*[@id='botaoEntrar']");
                    botao.Click();

                    var botaoAvisoDaSenha = Sessao.Procurar(driverAluno, Tipos.XPATH, "/html/body/div[7]/section/div/div[2]/button");
                    botaoAvisoDaSenha.Click();

                    var alterarSenha = Sessao.Procurar(driverAluno, Tipos.XPATH, "//*[@id='Senha']");
                    alterarSenha.SendKeys("12345678");
                    var alterarConfirmarSenha = Sessao.Procurar(driverAluno, Tipos.XPATH, "//*[@id='conf_senha']");
                    alterarConfirmarSenha.SendKeys("12345678");

                    var botaoConfirmarNovaSenha = Sessao.Procurar(driverAluno, Tipos.XPATH, "//*[@id='formDefinirSenha']/div[4]/input");
                    botaoConfirmarNovaSenha.Click();

                    var botaoFechar = Sessao.Procurar(driverAluno, Tipos.XPATH, "/html/body/div[5]/section/div/div[2]/button");
                    botaoFechar.Click();
                    
                    driverAluno.Quit();
                    Console.Clear();
                    System.Console.WriteLine("Iniciando a alteração de senhas em massa: ");
                    System.Console.Write("Concluido: ");
                    double porcentagemFeita = ((i +1 )* 100.0) / alunos.Count;
                    System.Console.WriteLine(porcentagemFeita.ToString("F2") + "%");
                }
                Console.Clear();
                System.Console.WriteLine("100.0% concluído");
                System.Console.WriteLine("------------------------------------------");
                System.Console.WriteLine("Senhas alteradas com sucesso!");
                System.Console.WriteLine("FIM DO PROGRAMA!");
                break;
        }

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
