using EpPaciencia.Models;
using System.Text;

namespace EpPaciencia.UI;

public class InterfaceJogo
{
    // ── ANSI escape codes ────────────────────────────────────────────────
    private const string Reset = "\u001b[0m";
    private const string Bold = "\u001b[1m";
    private const string Red = "\u001b[91m";
    private const string White = "\u001b[97m";
    private const string Gray = "\u001b[90m";
    private const string Yellow = "\u001b[93m";
    private const string Cyan = "\u001b[96m";
    private const string Green = "\u001b[92m";
    private const string MsgErr = "\u001b[91m";
    private const string BgGreen = "\u001b[42m";

    // Largura visual fixa de cada célula de carta (6 chars visíveis: [XY♥])
    // O bloco sempre ocupa 8 colunas no terminal: "[" + 2 + naipe + "]" + " "
    private const int CellWidth = 8;
    // Largura total do box (bordas incluídas)
    private const int BoxWidth = 66;

    // ── Estado do jogo ───────────────────────────────────────────────────
    private Jogo _jogo = null!;
    private string _mensagem = string.Empty;
    private bool _mensagemOk = true;

    // ── Ponto de entrada ─────────────────────────────────────────────────
    public void Iniciar()
    {
        Console.OutputEncoding = Encoding.UTF8;
        Console.CursorVisible = false;

        IniciarNovoJogo();

        while (true)
        {
            Desenhar();

            if (_jogo.VerificarVitoria())
            {
                Linha(BoxWidth);
                Caixa("  PARABENS! VOCE GANHOU!  ", Green + Bold, BoxWidth);
                Linha(BoxWidth);
                Console.Write($"\n  {Yellow}Novo jogo? (s/n): {Reset}");
                Console.CursorVisible = true;
                if (Console.ReadLine()?.Trim().ToLower() == "s")
                {
                    Console.CursorVisible = false;
                    IniciarNovoJogo();
                    continue;
                }
                break;
            }

            Console.Write($"\n  {Cyan}>{Reset} ");
            Console.CursorVisible = true;
            var linha = Console.ReadLine()?.Trim() ?? string.Empty;
            Console.CursorVisible = false;
            ProcessarComando(linha.ToLower());
        }
    }

    // ── Novo jogo ────────────────────────────────────────────────────────
    private void IniciarNovoJogo()
    {
        var baralho = new Baralho();
        baralho.EmbaralharRecursivo(1000);
        _jogo = new Jogo(baralho);
        _mensagem = "Novo jogo iniciado! Boa sorte!";
        _mensagemOk = true;
    }

    // Percorre a lista duplamente encadeada da coluna e retorna as cartas
    // da base ao topo. Usa CartaProxima para subir até o topo real (caso
    // CartaLigada não esteja apontando para o último), e depois volta via
    // CartaAnterior para coletar a coluna inteira sem perder referências
    // após movimentos por altura.
    private static List<Carta> CartasDaColuna(ListaLigadaPaciencia lista)
    {
        var cartas = new List<Carta>();
        if (lista.CartaLigada == null)
            return cartas;

        // 1) Sobe até o topo real (último CartaProxima != null)
        var topo = lista.CartaLigada;
        while (topo.CartaProxima != null)
            topo = topo.CartaProxima;

        // 2) Desce de volta coletando da base ao topo
        var no = topo;
        while (no != null)
        {
            cartas.Add(no.Carta);
            no = no.CartaAnterior;
        }
        cartas.Reverse();
        return cartas;
    }

    // ── Helpers de impressão alinhada ────────────────────────────────────

    // Imprime um separador horizontal completo
    private static void Linha(int w, char tl = '╠', char tr = '╣', char h = '═')
    {
        Console.WriteLine($"  {Cyan}{tl}{new string(h, w - 2)}{tr}{Reset}");
    }

    // Imprime uma linha dentro do box com conteúdo visual puro (sem escapes no padding)
    // innerLen = comprimento VISUAL do conteúdo (sem códigos ANSI)
    private static void BoxLinha(string conteudo, int innerLen, int w, string corBorda = Cyan)
    {
        int pad = w - 2 - innerLen;
        if (pad < 0) pad = 0;
        Console.WriteLine($"  {corBorda}║{Reset}{conteudo}{new string(' ', pad)}{corBorda}║{Reset}");
    }

    // Caixa centralizada simples (texto puro, sem escapes dentro)
    private static void Caixa(string texto, string cor, int w)
    {
        int inner = w - 2;
        int totalPad = inner - texto.Length;
        int left = totalPad / 2;
        int right = totalPad - left;
        Console.WriteLine($"  {cor}║{new string(' ', left)}{texto}{new string(' ', right)}║{Reset}");
    }

    // Carta formatada (retorna string com escapes + string visual de comprimento fixo 6)
    // Representa visualmente como "[10♥]" ou "[ A♠]" — sempre 6 chars visíveis
    private static (string ansi, int visualLen) FormatarCarta(Carta carta)
    {
        string num = carta.Numero switch
        {
            1 => " A",
            10 => "10",
            11 => " J",
            12 => " Q",
            13 => " K",
            _ => $" {carta.Numero}"
        };
        var (naipeSimbolo, cor) = carta.Naipe switch
        {
            'C' => ("♥", Red),
            'O' => ("♦", Red),
            'P' => ("♣", White),
            'E' => ("♠", White),
            _ => ("?", White)
        };
        // Visual: [  A♥]  → 6 chars visíveis: '[', num(2), naipe(1), ']' = 5... + espaço separador = 6
        return ($"{Gray}[{cor}{Bold}{num}{naipeSimbolo}{Reset}{Gray}]{Reset}", 6);
    }

    private static (string ansi, int visualLen) CartaVirada() =>
        ($"{Gray}[###]{Reset}", 5);

    private static (string ansi, int visualLen) CartaVazia(string simbolo = "   ") =>
        ($"{Gray}[{simbolo}]{Reset}", 5);

    // ── Desenho principal ─────────────────────────────────────────────────
    private void Desenhar()
    {
        Console.Clear();
        DesenharCabecalho();
        DesenharBaralhoEPilhas();
        DesenharTableau();
        DesenharAjuda();
        DesenharMensagem();
    }

    private void DesenharCabecalho()
    {
        Linha(BoxWidth, '╔', '╗');
        Caixa("  EP PACIENCIA  ", Yellow + Bold, BoxWidth);
        Linha(BoxWidth);
    }

    private void DesenharBaralhoEPilhas()
    {
        int qtd = _jogo.Fila.Quantidade;
        var proximaCarta = _jogo.Fila.Topo();

        // ── Linha do Baralho ────────────────────────────────────────────
        // Formato: " BARALHO [###](24)  PROXIMA [10♥]"
        string baralhoAnsi, proximaAnsi;
        int baralhoVLen, proximaVLen;

        if (qtd > 0)
            (baralhoAnsi, baralhoVLen) = ($"{Gray}[{White}{Bold}###{Reset}{Gray}]{Reset}", 5);
        else
            (baralhoAnsi, baralhoVLen) = ($"{Gray}[   ]{Reset}", 5);

        if (proximaCarta != null)
            (proximaAnsi, proximaVLen) = FormatarCarta(proximaCarta);
        else
        {
            (proximaAnsi, proximaVLen) = ($"{Gray}[   ]{Reset}", 5);
        }

        // Montamos o conteúdo e calculamos seu comprimento visual
        // " BARALHO: " = 10, baralho = 5, "(24) " = 5, "  PROXIMA: " = 11, proxima = 5+1 = 22+5+5 = totais
        string linhaBaralhoTexto =
            $" {Yellow}BARALHO:{Reset} {baralhoAnsi}{Gray}({qtd,2}){Reset}" +
            $"   {Yellow}PROXIMA:{Reset} {proximaAnsi} ";
        int linhaBaralhoVLen = 1 + 8 + 1 + baralhoVLen + 4 + 3 + 9 + 1 + proximaVLen + 1;
        BoxLinha(linhaBaralhoTexto, linhaBaralhoVLen, BoxWidth);

        // ── Linha das Pilhas ────────────────────────────────────────────
        char[] simbolos = ['♥', '♦', '♣', '♠'];
        string[] nomesPilha = ["P1 ♥", "P2 ♦", "P3 ♣", "P4 ♠"];
        var sb = new StringBuilder();
        sb.Append($" {Yellow}PILHAS:{Reset} ");
        int vLen = 1 + 7 + 1; // " PILHAS: "
        for (int i = 0; i < 4; i++)
        {
            var pilha = _jogo.Pilhas[i];
            string vis; int vl;
            if (pilha.Indice > 0)
                (vis, vl) = FormatarCarta(pilha.Cartas[pilha.Indice - 1]);
            else
            {
                string cor = i < 2 ? Red : White;
                vis = $"{Gray}[{cor}{Bold}{simbolos[i]}--{Reset}{Gray}]{Reset}";
                vl = 5;
            }
            sb.Append($"{vis}{Gray}({nomesPilha[i]}){Reset} ");
            vLen += vl + 1 + nomesPilha[i].Length + 2 + 1;
        }
        BoxLinha(sb.ToString(), vLen, BoxWidth);

        Linha(BoxWidth);
    }

    private void DesenharTableau()
    {
        const int CellW = 7;
        const int LeadIn = 1;
        int innerLen = LeadIn + 7 * CellW;

        // Coleta as cartas das 7 colunas (base → topo) percorrendo a lista encadeada.
        var colunas = new List<Carta>[7];
        for (int i = 0; i < 7; i++)
            colunas[i] = CartasDaColuna(_jogo.ListasLigadas[i]);

        // ── Cabeçalho ──────────────────────────────────────────────────
        var sbH = new StringBuilder(new string(' ', LeadIn));
        for (int i = 0; i < 7; i++)
        {
            string label = $"C{i + 1}({colunas[i].Count})";
            string padded = label.PadRight(CellW);
            sbH.Append($"{Yellow}{Bold}{padded}{Reset}");
        }
        BoxLinha(sbH.ToString(), innerLen, BoxWidth);

        // ── Conteúdo ───────────────────────────────────────────────────
        int altura = 0;
        for (int i = 0; i < 7; i++)
            if (colunas[i].Count > altura) altura = colunas[i].Count;
        if (altura == 0) altura = 1;

        for (int row = 0; row < altura; row++)
        {
            var sb = new StringBuilder(new string(' ', LeadIn));
            for (int c = 0; c < 7; c++)
            {
                if (row < colunas[c].Count)
                {
                    var carta = colunas[c][row];
                    var (ansi, _) = carta.Status
                        ? FormatarCarta(carta)
                        : CartaVirada();
                    sb.Append(ansi);
                    sb.Append("  ");
                }
                else if (row == 0 && colunas[c].Count == 0)
                {
                    sb.Append($"{Gray}[---]{Reset}  ");
                }
                else
                {
                    sb.Append(new string(' ', CellW));
                }
            }
            BoxLinha(sb.ToString(), innerLen, BoxWidth);
        }

        Linha(BoxWidth);
    }

    private void DesenharAjuda()
    {
        // Cada linha: comando à esquerda (16 chars visuais) + descrição
        (string cmd, string desc)[] cmds =
        [
            ("bc <c>",       "Mover carta do Baralho para Coluna c (1-7)"),
            ("cc <c1> <c2>", "Mover topo da Coluna c1 para Coluna c2"),
            ("cc <c1> <c2> <h>", "Mover carta na altura h da Coluna c1 (0=topo)"),
            ("cp <c> <p>",   "Mover topo da Coluna c para Pilha p (1-4)"),
            ("pc <p> <c>",   "Mover topo da Pilha p para Coluna c"),
            ("bp <p>",       "Mover proxima carta do Baralho para Pilha p"),
            ("p",            "Passar para a proxima carta do Baralho"),
            ("n",            "Novo jogo     |   s  Sair"),
        ];

        BoxLinha($" {Yellow}COMANDOS:{Reset}", 1 + 9, BoxWidth);

        foreach (var (cmd, desc) in cmds)
        {
            string linha = $"  {Cyan}{cmd,-14}{Reset} {Gray}{desc}{Reset}";
            int vLen = 2 + cmd.Length + (14 - cmd.Length) + 1 + desc.Length;
            BoxLinha(linha, Math.Min(vLen, BoxWidth - 4), BoxWidth);
        }

        BoxLinha("", 0, BoxWidth);
        string naipes = " P1=Copas(♥)  P2=Ouros(♦)  P3=Paus(♣)  P4=Espadas(♠)";
        BoxLinha($"{Yellow}{naipes}{Reset}", naipes.Length, BoxWidth);

        Linha(BoxWidth, '╚', '╝');
    }

    private void DesenharMensagem()
    {
        if (string.IsNullOrEmpty(_mensagem)) return;
        string cor = _mensagemOk ? Green : MsgErr;
        Console.WriteLine($"  {cor}>> {_mensagem}{Reset}");
        _mensagem = string.Empty;
    }

    // ── Processamento de comandos ────────────────────────────────────────
    private void ProcessarComando(string entrada)
    {
        var p = entrada.Split(' ', StringSplitOptions.RemoveEmptyEntries);
        if (p.Length == 0) return;

        switch (p[0])
        {
            case "n":
                IniciarNovoJogo();
                return;

            case "s":
            case "sair":
                Console.CursorVisible = true;
                Console.Clear();
                Environment.Exit(0);
                return;

            case "p":
                PularCarta();
                return;

            case "bc" when p.Length == 2 && ParseCol(p[1], out int cBc):
                MoverBaralhoColuna(cBc);
                return;

            case "cc" when p.Length == 3 && ParseCol(p[1], out int c1) && ParseCol(p[2], out int c2):
                MoverColunaColuna(c1, c2, 0);
                return;

            case "cc" when p.Length == 4 && ParseCol(p[1], out int c1h) && ParseCol(p[2], out int c2h) && ParseAltura(p[3], out int h):
                MoverColunaColuna(c1h, c2h, h);
                return;

            case "cp" when p.Length == 3 && ParseCol(p[1], out int cCp) && ParsePilha(p[2], out int pCp):
                MoverColunaPilha(cCp, pCp);
                return;

            case "pc" when p.Length == 3 && ParsePilha(p[1], out int pPc) && ParseCol(p[2], out int cPc):
                MoverPilhaColuna(pPc, cPc);
                return;

            case "bp" when p.Length == 2 && ParsePilha(p[1], out int pBp):
                MoverBaralhoPilha(pBp);
                return;

            default:
                _mensagem = $"Comando invalido: '{entrada}'. Veja os comandos acima.";
                _mensagemOk = false;
                return;
        }
    }

    private static bool ParseCol(string s, out int idx)
    {
        if (int.TryParse(s, out int v) && v >= 1 && v <= 7) { idx = v - 1; return true; }
        idx = -1; return false;
    }

    private static bool ParsePilha(string s, out int idx)
    {
        if (int.TryParse(s, out int v) && v >= 1 && v <= 4) { idx = v - 1; return true; }
        idx = -1; return false;
    }

    private static bool ParseAltura(string s, out int altura)
    {
        if (int.TryParse(s, out int v) && v >= 0 && v < Baralho.CARTAS_NAIPE * Baralho.NAIPES.Length)
        { altura = v; return true; }
        altura = -1; return false;
    }

    // ── Ações de jogo ────────────────────────────────────────────────────
    private void PularCarta()
    {
        var carta = _jogo.Fila.Desenfileirar();
        if (carta == null)
        {
            _mensagem = "O baralho esta vazio!";
            _mensagemOk = false;
            return;
        }
        _jogo.Fila.Enfileirar(carta);
        _mensagem = "Carta avancada no baralho.";
        _mensagemOk = true;
    }

    private void MoverBaralhoColuna(int col)
    {
        bool ok = Jogo.MoverCarta(_jogo.Fila, _jogo.ListasLigadas[col]);
        _mensagemOk = ok;
        _mensagem = ok
            ? $"Carta do baralho movida para C{col + 1}."
            : $"Não foi possível mover do baralho para C{col + 1}.";
    }

    private void MoverColunaColuna(int origem, int destino, int altura)
    {
        bool ok = Jogo.MoverCarta(_jogo.ListasLigadas[origem], altura, _jogo.ListasLigadas[destino]);
        _mensagemOk = ok;
        string sufixo = altura == 0 ? "" : $" (altura {altura})";
        _mensagem = ok
            ? $"Carta movida de C{origem + 1} para C{destino + 1}{sufixo}."
            : $"Não foi possível mover de C{origem + 1} para C{destino + 1}{sufixo}.";
    }

    private void MoverColunaPilha(int col, int pilha)
    {
        bool ok = Jogo.MoverCarta(_jogo.ListasLigadas[col], _jogo.Pilhas[pilha]);
        _mensagemOk = ok;
        _mensagem = ok
            ? $"Carta de C{col + 1} enviada para P{pilha + 1}."
            : $"Não foi possível mover C{col + 1} para P{pilha + 1}.";
    }

    private void MoverPilhaColuna(int pilha, int col)
    {
        bool ok = Jogo.MoverCarta(_jogo.Pilhas[pilha], _jogo.ListasLigadas[col]);
        _mensagemOk = ok;
        _mensagem = ok
            ? $"Carta de P{pilha + 1} movida para C{col + 1}."
            : $"Não foi possível mover P{pilha + 1} para C{col + 1}.";
    }

    private void MoverBaralhoPilha(int pilha)
    {
        bool ok = Jogo.MoverCarta(_jogo.Fila, _jogo.Pilhas[pilha]);
        _mensagemOk = ok;
        _mensagem = ok
            ? $"Carta do Baralho movida para P{pilha + 1}."
            : $"Não foi possível mover a carta do Baralho para P{pilha + 1}.";
    }
}
