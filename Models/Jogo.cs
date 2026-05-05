namespace EpPaciencia.Models;

public class Jogo
{
    public Baralho Baralho { get; private set; }
    public PilhaPaciencia[] Pilhas { get; private set; }
    public FilaPaciencia Fila { get; private set; }
    public ListaLigadaPaciencia[] ListasLigadas { get; private set; }

    public Jogo(Baralho baralhoEmbaralhado)
    {
        Baralho = baralhoEmbaralhado;
        Pilhas = [.. Baralho.NAIPES.Select(n => new PilhaPaciencia())];
        Fila = new FilaPaciencia(Baralho);
        ListasLigadas = [.. Enumerable.Range(0, 7).Select(_ => new ListaLigadaPaciencia())];

        for (int i = 0; i < 7; i++)
        {
            for (int j = 0; j <= i; j++)
            {
                var carta = Fila.Desenfileirar();
                if (carta != null)
                {
                    ListasLigadas[i].AdicionarCarta(new CartaLigada(carta));
                    if (j == i)
                        carta.VirarCarta();
                }
            }
        }
    }

    public static bool MoverCarta(PilhaPaciencia origem, ListaLigadaPaciencia destino)
    {
        if (origem.Indice == 0)
            return false;

        var carta = origem.Desempilhar();
        if (!destino.AdicionarCarta(new CartaLigada(carta!)))
        {
            origem.Empilhar(carta!);
            return false;
        }

        return true;
    }

    public static bool MoverCarta(ListaLigadaPaciencia origem, PilhaPaciencia destino)
    {
        if (origem.CartaLigada == null)
            return false;

        var cartaAtual = origem.CartaLigada;
        var origemAnterior = cartaAtual.CartaAnterior;
        if (destino.Empilhar(cartaAtual.Carta))
        {
            origem.RefazerReferencia(origemAnterior!);
            return true;
        }

        return false;
    }

    public static bool MoverCarta(FilaPaciencia origem, ListaLigadaPaciencia destino)
    {
        var carta = origem.Desenfileirar();
        if (carta == null)
            return false;

        carta.VirarCarta();
        if (destino.AdicionarCarta(new CartaLigada(carta)))
            return true;

        carta.VirarCarta();
        origem.Enfileirar(carta);
        return false;
    }

    public static bool MoverCarta(ListaLigadaPaciencia origem, int posicaoOrigem, ListaLigadaPaciencia destino)
    {
        if (origem.CartaLigada == null)
            return false;

        var cartaAtual = origem.CartaLigada;
        for (int i = 0; i < posicaoOrigem && cartaAtual != null; i++)
            cartaAtual = cartaAtual.CartaAnterior;

        if (cartaAtual == null || !cartaAtual.Carta.Status)
            return false;

        var origemAnterior = cartaAtual.CartaAnterior;
        if (destino.AdicionarCarta(cartaAtual))
        {
            origem.RefazerReferencia(origemAnterior!);
            return true;
        }

        return false;
    }


    public static bool MoverCarta(FilaPaciencia origem, PilhaPaciencia destino)
    {
        var carta = origem.Desenfileirar();
        if (carta == null)
            return false;

        carta.VirarCarta();
        if (destino.Empilhar(carta))
            return true;

        carta.VirarCarta();
        origem.Enfileirar(carta);
        return false;
    }

    public bool VerificarVitoria()
    {
        return Pilhas.All(p => p.Indice == Baralho.CARTAS_NAIPE);
    }
}