namespace EpPaciencia.Models;

public class FilaPaciencia
{
    private static readonly int CAPACIDADE = Baralho.CARTAS_NAIPE * Baralho.NAIPES.Length + 1;
    private int _indiceInicio = 0;
    private int _indiceFim = 0;
    public Carta[] Cartas { get; private set; } = new Carta[CAPACIDADE];

    public FilaPaciencia(Baralho baralho)
    {
        foreach (var carta in baralho.Cartas)
            Enfileirar(carta);
    }

    public Carta? Topo()
    {
        if (_indiceInicio == _indiceFim)
            return null;
        return Cartas[_indiceInicio];
    }

    public int Quantidade
    {
        get
        {
            if (_indiceFim >= _indiceInicio)
                return _indiceFim - _indiceInicio;
            return CAPACIDADE - _indiceInicio + _indiceFim;
        }
    }

    private bool EstaVazia()
    {
        return _indiceInicio == _indiceFim;
    }

    private bool EstaCheia()
    {
        return (_indiceFim + 1) % CAPACIDADE == _indiceInicio;
    }

    public bool Enfileirar(Carta carta)
    {
        if (EstaCheia())
            return false;

        Cartas[_indiceFim] = carta;
        _indiceFim = (_indiceFim + 1) % CAPACIDADE;
        return true;
    }

    public Carta? Desenfileirar()
    {
        if (EstaVazia())
            return null;

        var carta = Cartas[_indiceInicio];
        Cartas[_indiceInicio] = null!;
        _indiceInicio = (_indiceInicio + 1) % CAPACIDADE;
        return carta;
    }
}