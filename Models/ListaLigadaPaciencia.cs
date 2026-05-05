namespace EpPaciencia.Models;

public class ListaLigadaPaciencia
{
    public CartaLigada? CartaLigada { get; private set; }

    public ListaLigadaPaciencia()
    {
        CartaLigada = null;
    }

    public bool RefazerReferencia(CartaLigada cartaAnteriorRemovida)
    {
        if (CartaLigada == null)
            return false;

        CartaLigada = cartaAnteriorRemovida;
        CartaLigada?.CartaProxima = null;
        if (CartaLigada != null && !CartaLigada.Carta.Status)
            CartaLigada.Carta.VirarCarta();

        return true;
    }

    public bool AdicionarCarta(CartaLigada cartaInserida)
    {
        if (cartaInserida.Carta.Status)
        {
            if (cartaInserida.Carta.Numero == 13 && CartaLigada == null)
            {
                cartaInserida.CartaAnterior = null;

                var cartaAuxiliar1 = cartaInserida;
                while (cartaAuxiliar1.CartaProxima != null)
                {
                    cartaAuxiliar1 = cartaAuxiliar1.CartaProxima;
                }

                CartaLigada = cartaAuxiliar1;
                return true;
            }

            if (CartaLigada == null)
                return false;

            if (CartaLigada.Carta.EVermelha() && cartaInserida.Carta.EVermelha() || CartaLigada.Carta.EPreta() && cartaInserida.Carta.EPreta())
                return false;

            if (CartaLigada.Carta.Numero - 1 != cartaInserida.Carta.Numero)
                return false;
        }

        if (CartaLigada == null)
        {
            CartaLigada = cartaInserida;
            cartaInserida.CartaAnterior = null;
            cartaInserida.CartaProxima = null;
            return true;
        }

        CartaLigada!.CartaProxima = cartaInserida;
        cartaInserida.CartaAnterior = CartaLigada;

        var cartaAuxiliar = cartaInserida;
        while (cartaAuxiliar.CartaProxima != null)
        {
            cartaAuxiliar = cartaAuxiliar.CartaProxima;
        }

        CartaLigada = cartaAuxiliar;
        return true;
    }
}

public class CartaLigada(Carta carta)
{
    public Carta Carta { get; private set; } = carta;
    public CartaLigada? CartaAnterior { get; set; } = null;
    public CartaLigada? CartaProxima { get; set; } = null;
}