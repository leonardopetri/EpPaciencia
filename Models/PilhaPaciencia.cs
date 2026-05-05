namespace EpPaciencia.Models;

public class PilhaPaciencia
{
    public Carta[] Cartas { get; private set; } = new Carta[Baralho.CARTAS_NAIPE];
    public int Indice { get; private set; } = 0;

    public bool Empilhar(Carta carta)
    {
        if (Indice == Baralho.CARTAS_NAIPE)
            return false;

        if (Indice > 0 && carta.Naipe != Cartas[Indice - 1].Naipe)
            return false;

        if (carta.Numero != Indice + 1)
            return false;

        Cartas[Indice++] = carta;
        return true;
    }

    public Carta? Desempilhar()
    {
        if (Indice == 0)
            return null;

        var carta = Cartas[Indice - 1];
        Cartas[--Indice] = null!;
        return carta;
    }
}