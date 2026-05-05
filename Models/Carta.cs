namespace EpPaciencia.Models;

public class Carta(int id, char naipe, int numero)
{
    public int Id { get; private set; } = id;
    public int Numero { get; private set; } = numero;
    public char Naipe { get; private set; } = naipe;
    public bool Status { get; private set; } = false;

    public void VirarCarta()
    {
        Status = !Status;
    }

    public override string ToString()
    {
        return $"Id: {Id} - Numero: {Numero} - Naipe: {Naipe} - Status: {Status}";
    }

    public bool EVermelha()
    {
        return Baralho.NAIPES_VERMELHOS.Contains(Naipe);
    }

    public bool EPreta()
    {
        return Baralho.NAIPES_PRETOS.Contains(Naipe);
    }
}