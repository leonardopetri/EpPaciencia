using System.Runtime.InteropServices;
using System.Text;
using EpPaciencia.Models;
using EpPaciencia.UI;

Console.OutputEncoding = Encoding.UTF8;
HabilitarTerminalVirtualWindows();

var baralho = new Baralho();

static void HabilitarTerminalVirtualWindows()
{
    if (!RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
        return;

    const int STD_OUTPUT_HANDLE = -11;
    const uint ENABLE_VIRTUAL_TERMINAL_PROCESSING = 0x0004;

    var handle = GetStdHandle(STD_OUTPUT_HANDLE);
    if (handle == IntPtr.Zero || handle == new IntPtr(-1))
        return;

    if (!GetConsoleMode(handle, out uint mode))
        return;

    SetConsoleMode(handle, mode | ENABLE_VIRTUAL_TERMINAL_PROCESSING);
}

[DllImport("kernel32.dll", SetLastError = true)]
static extern IntPtr GetStdHandle(int nStdHandle);

[DllImport("kernel32.dll", SetLastError = true)]
static extern bool GetConsoleMode(IntPtr hConsoleHandle, out uint lpMode);

[DllImport("kernel32.dll", SetLastError = true)]
static extern bool SetConsoleMode(IntPtr hConsoleHandle, uint dwMode);

while (true)
{
    Console.WriteLine("Escolha uma opção:");
    Console.WriteLine("1 - Embaralhar");
    Console.WriteLine("2 - EmbaralharRecursivo");
    Console.WriteLine("3 - OrdenarBaralhoBubbleSort");
    Console.WriteLine("4 - OrdenarBaralhoQuickSort");
    Console.WriteLine("5 - OrdenarBaralhoMergeSort");
    Console.WriteLine("6 - EstaEmbaralhadoMonteCarlo");
    Console.WriteLine("7 - EstaEmbaralhado");
    Console.WriteLine("8 - Iniciar Jogo");
    Console.WriteLine("9 - Sair");
    Console.Write("Opção: ");

    var opcao = Console.ReadLine();

    switch (opcao)
    {
        case "1":
            baralho.Embaralhar();
            Console.WriteLine("Embaralhar executado.");
            break;
        case "2":
            baralho.EmbaralharRecursivo(1000);
            Console.WriteLine("EmbaralharRecursivo executado.");
            break;
        case "3":
            baralho.OrdenarBaralhoBubbleSort();
            Console.WriteLine("OrdenarBaralhoBubbleSort executado.");
            break;
        case "4":
            baralho.OrdenarBaralhoQuickSort();
            Console.WriteLine("OrdenarBaralhoQuickSort executado.");
            break;
        case "5":
            baralho.OrdenarBaralhoMergeSort();
            Console.WriteLine("OrdenarBaralhoMergeSort executado.");
            break;
        case "6":
            var resultadoMonteCarlo = baralho.EstaEmbaralhadoMonteCarlo();
            Console.WriteLine($"EstaEmbaralhadoMonteCarlo executado. Resultado: {resultadoMonteCarlo}");
            break;
        case "7":
            var resultadoEmbaralhado = baralho.EstaEmbaralhado();
            Console.WriteLine($"EstaEmbaralhado executado. Resultado: {resultadoEmbaralhado}");
            break;
        case "8":
            var interface_ = new InterfaceJogo();
            interface_.Iniciar();
            return;
        case "9":
            return;
        default:
            Console.WriteLine("Opção inválida.");
            continue;
    }

    Console.WriteLine("\nEstado atual do baralho:");
    Console.WriteLine(baralho.ToString());
}
