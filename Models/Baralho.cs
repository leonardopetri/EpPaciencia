namespace EpPaciencia.Models;

public class Baralho
{
    public const int CARTAS_NAIPE = 13;
    public static readonly char[] NAIPES = ['C', 'O', 'P', 'E'];
    public static readonly char[] NAIPES_VERMELHOS = ['C', 'O'];
    public static readonly char[] NAIPES_PRETOS = ['P', 'E'];

    public Carta[] Cartas { get; private set; }

    public Baralho()
    {
        Cartas = new Carta[CARTAS_NAIPE * NAIPES.Length];
        AdicionarCartasAoBaralho();
    }

    private void AdicionarCartasAoBaralho()
    {
        var counter = 0;
        foreach (var naipe in NAIPES)
        {
            for (int i = 1; i <= CARTAS_NAIPE; i++)
            {
                Cartas[counter] = new Carta(counter + 1, naipe, i);
                counter++;
            }
        }
    }

    public override string ToString()
    {
        var baralhoString = string.Empty;
        foreach (var carta in Cartas)
        {
            baralhoString += carta.ToString() + "\n";
        }
        return baralhoString;
    }

    public bool EstaEmbaralhadoMonteCarlo()
    {
        var random = new Random();
        for (int i = 0; i < 10; i++)
        {
            var testeIndice = random.Next(0, CARTAS_NAIPE * NAIPES.Length);
            if (testeIndice != Cartas[testeIndice].Id - 1)
            {
                return true;
            }
        }

        return false;
    }

    public bool EstaEmbaralhado()
    {
        for (int i = 0; i < this.Cartas.Length; i++)
        {
            if (i != Cartas[i].Id - 1)
            {
                return true;
            }
        }

        return false;
    }
    public void Embaralhar()
    {
        var random = new Random();
        for (int i = 0; i < 1000; i++)
        {
            var from = random.Next(0, Baralho.CARTAS_NAIPE * Baralho.NAIPES.Length);
            var to = random.Next(0, Baralho.CARTAS_NAIPE * Baralho.NAIPES.Length);

            var cartaFrom = Cartas[from];
            var cartaTo = Cartas[to];

            Cartas[from] = cartaTo;
            Cartas[to] = cartaFrom;
        }
    }

    public void EmbaralharRecursivo(int quantidadeEmbaralha)
    {
        if (quantidadeEmbaralha <= 0)
            return;

        var random = new Random();
        var from = random.Next(0, Baralho.CARTAS_NAIPE * Baralho.NAIPES.Length);
        var to = random.Next(0, Baralho.CARTAS_NAIPE * Baralho.NAIPES.Length);

        var cartaFrom = Cartas[from];
        var cartaTo = Cartas[to];

        Cartas[from] = cartaTo;
        Cartas[to] = cartaFrom;

        EmbaralharRecursivo(quantidadeEmbaralha - 1);
    }

    public void OrdenarBaralhoBubbleSort()
    {
        for (int i = 0; i < Cartas.Length - 1; i++)
        {
            var trocou = false;
            for (int j = 0; j < Cartas.Length - i - 1; j++)
            {
                if (Cartas[j].Id > Cartas[j + 1].Id)
                {
                    (Cartas[j + 1], Cartas[j]) = (Cartas[j], Cartas[j + 1]);
                    trocou = true;
                }
            }

            if (!trocou)
            {
                break;
            }
        }
    }

    public void OrdenarBaralhoQuickSort()
    {
        var minimo = 0;
        var maximo = Cartas.Length - 1;

        OrdenarBaralhoQuickSort(Cartas, minimo, maximo);
        OrdenarBaralhoQuickSort(Cartas, minimo, maximo);
    }

    private static void OrdenarBaralhoQuickSort(Carta[] cartas, int minimo, int maximo)
    {
        if (minimo < maximo)
        {
            var pivo = ParticionarQuickSort(cartas, minimo, maximo);

            OrdenarBaralhoQuickSort(cartas, minimo, pivo - 1);
            OrdenarBaralhoQuickSort(cartas, pivo + 1, maximo);
        }
    }

    private static int ParticionarQuickSort(Carta[] cartas, int minimo, int maximo)
    {
        var pivo = cartas[maximo];
        var i = minimo - 1;

        for (int j = minimo; j < maximo; j++)
        {
            if (cartas[j].Id < pivo.Id)
            {
                i++;
                (cartas[j], cartas[i]) = (cartas[i], cartas[j]);
            }
        }

        (cartas[i + 1], cartas[maximo]) = (cartas[maximo], cartas[i + 1]);

        return i + 1;
    }

    public void OrdenarBaralhoMergeSort()
    {
        var inicio = 0;
        var fim = Cartas.Length - 1;

        OrdenarBaralhoMergeSort(Cartas, inicio, fim);
    }

    private static void OrdenarBaralhoMergeSort(Carta[] cartas, int inicio, int fim)
    {
        if (inicio < fim)
        {
            var meio = inicio + (fim - inicio) / 2;

            OrdenarBaralhoMergeSort(cartas, inicio, meio);
            OrdenarBaralhoMergeSort(cartas, meio + 1, fim);

            MesclarMergeSort(cartas, inicio, meio, fim);
        }
    }

    private static void MesclarMergeSort(Carta[] cartas, int inicio, int meio, int fim)
    {
        var i = inicio;
        var j = meio + 1;
        var k = 0;

        var cartasTemp = new Carta[fim - inicio + 1];

        while (i <= meio && j <= fim)
        {
            if (cartas[i].Id <= cartas[j].Id)
            {
                cartasTemp[k++] = cartas[i++];
            }
            else
            {
                cartasTemp[k++] = cartas[j++];
            }
        }

        while (i <= meio)
        {
            cartasTemp[k++] = cartas[i++];
        }

        while (j <= fim)
        {
            cartasTemp[k++] = cartas[j++];
        }

        k = 0;
        for (i = inicio; i <= fim; i++)
        {
            cartas[i] = cartasTemp[k++];
        }
    }
}