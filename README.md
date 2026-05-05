# EP Paciência

Implementação em C# / .NET 10 do clássico jogo de cartas **Paciência (Klondike Solitaire)**, jogado direto no terminal com interface em ANSI colorido.

O projeto também serve como exercício de estruturas de dados: o jogo combina **arrays**, **pilhas**, **filas** e **listas duplamente encadeadas**, além de algoritmos de ordenação e embaralhamento (Bubble Sort, Quick Sort, Merge Sort, embaralhamento iterativo e recursivo).

---

## Como executar

```bash
dotnet run --project EpPaciencia.csproj
```

No menu inicial, escolha a opção **8 - Iniciar Jogo**.

> Dica: as opções 1 a 7 servem para testar os algoritmos de embaralhamento/ordenação no baralho isolado. Para jogar, vá direto na opção **8**.

---

## Objetivo do jogo

Mover **todas as 52 cartas** do tableau (as 7 colunas) para as **4 pilhas de naipe** (fundações), em ordem **crescente** de **Ás (A)** até **Rei (K)**, separadas por naipe:

| Pilha | Naipe       | Símbolo |
|-------|-------------|---------|
| P1    | Copas       | ♥       |
| P2    | Ouros       | ♦       |
| P3    | Paus        | ♣       |
| P4    | Espadas     | ♠       |

Você vence quando as quatro pilhas chegam ao Rei.

---

## Tabuleiro

Logo no início, o programa monta:

- **Tableau**: 7 colunas (`C1` a `C7`). A coluna `Cn` recebe `n` cartas; só a carta do topo de cada coluna começa **virada para cima**.
- **Baralho** (ícone `[###]`): cartas restantes (24 no início).
- **Próxima**: a carta do topo do baralho, disponível para jogar.
- **Pilhas** (`P1` a `P4`): vazias no início, mostrando apenas o símbolo do naipe (`[♥--]`, `[♦--]`, `[♣--]`, `[♠--]`).

Exemplo de tela inicial:

```
╔════════════════════════════════════════════════════════════════╗
║                          EP PACIENCIA                          ║
╠════════════════════════════════════════════════════════════════╣
║ BARALHO: [###](24)   PROXIMA: [ 7♦]                            ║
║ PILHAS:  [♥--](P1 ♥) [♦--](P2 ♦) [♣--](P3 ♣) [♠--](P4 ♠)       ║
╠════════════════════════════════════════════════════════════════╣
║ C1(1) C2(2) C3(3) C4(4) C5(5) C6(6) C7(7)                      ║
║ [ A♠] [###] [###] [###] [###] [###] [###]                      ║
║       [ 5♥] [###] [###] [###] [###] [###]                      ║
║             [10♣] [###] [###] [###] [###]                      ║
║                   [ 3♦] [###] [###] [###]                      ║
║                         [ Q♠] [###] [###]                      ║
║                               [ 2♥] [###]                      ║
║                                     [ K♣]                      ║
╠════════════════════════════════════════════════════════════════╣
```

`[###]` = carta virada para baixo. `[ A♠]` = Ás de Espadas virado para cima.

---

## Regras

### Tableau (colunas `C1`–`C7`)

- Em cima de uma carta, só pode entrar uma carta com **valor 1 a menos** e de **cor oposta**.
  - Exemplo: sobre um `[ 8♥]` (vermelho) você pode colocar um `[ 7♣]` ou `[ 7♠]` (preto).
- **Coluna vazia**: só aceita um **Rei (K)**.
- Quando você move a carta do topo, a carta de baixo (que estava virada) é **automaticamente desvirada**.
- Você pode mover **uma sequência inteira** de cartas (já encadeadas em cor alternada) usando o parâmetro `<h>` (altura).

### Baralho / Próxima

- A carta visível em **PROXIMA** é a única jogável do baralho.
- Use o comando `p` para avançar para a próxima carta sem jogar.
- O baralho é circular: depois da última carta, ele volta para o início.

### Pilhas (`P1`–`P4`)

- Cada pilha aceita só **um naipe**, começando pelo **Ás** e subindo até o **Rei**.
- A carta enviada deve ser a próxima na sequência (`A → 2 → 3 → ... → K`) e do mesmo naipe da pilha.
- É possível **tirar** uma carta da pilha de volta para o tableau (ex.: para liberar uma combinação).

---

## Comandos

| Comando             | Ação                                                                  |
|---------------------|------------------------------------------------------------------------|
| `bc <c>`            | Mover a carta da **PROXIMA** para a coluna `c` (1–7)                  |
| `cc <c1> <c2>`      | Mover a carta do **topo** de `c1` para `c2`                           |
| `cc <c1> <c2> <h>`  | Mover a carta na altura `h` de `c1` (e tudo que está acima) para `c2` |
| `cp <c> <p>`        | Mover topo da coluna `c` para a pilha `p` (1–4)                       |
| `pc <p> <c>`        | Mover topo da pilha `p` para a coluna `c`                             |
| `bp <p>`            | Mover **PROXIMA** carta do baralho direto para a pilha `p`            |
| `p`                 | Passar para a próxima carta do baralho                                |
| `n`                 | Iniciar um **novo jogo**                                              |
| `s`                 | **Sair** do jogo                                                      |

> Sobre a altura `h` em `cc`: `0` é o **topo** da coluna; `1` é a carta logo abaixo do topo; e assim por diante. Apenas cartas **viradas para cima** podem ser movidas.

---

## Exemplos de jogadas

Suponha o seguinte estado simplificado:

```
PROXIMA: [ 7♦]
PILHAS:  [♥--] [♦--] [♣--] [♠--]

C1: [ A♠]
C2: [###] [ 5♥]
C3: [###] [###] [10♣]
C4: [###] [###] [###] [ 3♦]
C5: [###] [###] [###] [###] [ Q♠]
C6: [###] [###] [###] [###] [###] [ 2♥]
C7: [###] [###] [###] [###] [###] [###] [ K♣]
```

### 1) Enviar um Ás para a fundação

`C1` tem o Ás de Espadas. Como Espadas é a pilha **P4**:

```
> cp 1 4
```

Resultado: `P4` passa de `[♠--]` para `[ A♠]`. `C1` fica vazia (`[---]`).

### 2) Mover uma carta entre colunas (cor alternada, valor −1)

A `[ Q♠]` (preta) em `C5` aceita uma carta vermelha de valor 11 (J♥/J♦) por cima. Suponha que apareça `[ J♦]` na PROXIMA:

```
> bc 5
```

A `[ J♦]` sai do baralho e vai para o topo de `C5`, formando `... [ Q♠] [ J♦]`.

### 3) Aproveitar a PROXIMA do baralho

Com `[ 7♦]` na PROXIMA e um `[ 8♣]` no topo de alguma coluna (digamos `C3`):

```
> bc 3
```

`[ 7♦]` (vermelha) entra sobre `[ 8♣]` (preta) — valor −1, cor oposta: jogada válida.

Se a jogada não for permitida (cor igual ou valor errado), aparece a mensagem **“Não foi possível mover…”** e a carta volta para a fila do baralho.

### 4) Passar para a próxima carta do baralho

Quando a `PROXIMA` não serve em lugar nenhum:

```
> p
```

Ela vai para o fim da fila e a próxima carta do baralho é exibida.

### 5) Mover uma sequência de cartas (parâmetro de altura `h`)

Se em `C2` você tem a sequência (do fundo para o topo):

```
C2: ... [ 6♣] [ 5♥] [ 4♣]
```

E em `C5` o topo é `[ 7♦]` (vermelho), você quer levar `[ 6♣] [ 5♥] [ 4♣]` inteiro para `C5`. O `[ 6♣]` está na altura **2** (topo = 0, `[ 5♥]` = 1, `[ 6♣]` = 2):

```
> cc 2 5 2
```

Toda a sequência é encaixada sobre `[ 7♦]`.

### 6) Liberar uma coluna para mover um Rei

Se `C1` ficou vazia e em `C7` o topo é `[ K♣]`:

```
> cc 7 1
```

O Rei de Paus vai ocupar a coluna vazia. Use isso para destravar cartas presas embaixo de Reis.

### 7) Mandar uma carta da PROXIMA direto para a fundação

Se a PROXIMA é `[ A♥]`:

```
> bp 1
```

O Ás de Copas entra em `P1`. Depois, quando aparecer o `2♥`, repita:

```
> bp 1
```

E assim por diante até o Rei.

### 8) Devolver uma carta da pilha para o tableau

Às vezes é útil “puxar de volta” uma carta da fundação para liberar uma jogada no tableau. Por exemplo, se `P3` tem topo `[ 5♣]` e em alguma coluna o topo é `[ 6♦]` (vermelho):

```
> pc 3 <coluna>
```

O `[ 5♣]` (preto) volta ao tableau sobre o `[ 6♦]`.

### 9) Fluxo típico no início de uma partida

```
> cp 1 4    # joga Ás de Espadas para P4
> bc 5      # encaixa PROXIMA (J♦) sobre Q♠ em C5
> p         # avança baralho
> p         # avança baralho
> bp 1      # joga A♥ direto na pilha de Copas
> cc 6 5    # leva 2♥ de C6 para cima do 3♣? (válido se cor opos. e valor -1)
> n         # caso queira reiniciar
```

---

## Estrutura do projeto

```
EpPaciencia/
├── Program.cs                       # Menu inicial e testes de baralho
├── Models/
│   ├── Baralho.cs                   # Geração, embaralhamento e ordenações
│   ├── Carta.cs                     # Representação de uma carta
│   ├── FilaPaciencia.cs             # Fila circular = baralho de compra
│   ├── PilhaPaciencia.cs            # Pilhas de fundação (1 por naipe)
│   ├── ListaLigadaPaciencia.cs      # Lista duplamente encadeada = colunas
│   └── Jogo.cs                      # Regras e movimentos válidos
└── UI/
    └── InterfaceJogo.cs             # Render ANSI e parser de comandos
```

---

## Dicas de estratégia

- Priorize **virar cartas** das colunas mais profundas (mais cartas viradas para baixo).
- Não suba para a fundação cedo demais: cartas baixas (2, 3, 4) ainda podem ser úteis para encaixar sequências no tableau.
- Esvazie colunas com cuidado — só Reis ocupam coluna vazia, então segure pelo menos um Rei livre.
- Use `bp` sempre que possível para Ases e 2 — eles raramente são úteis no tableau.
- Quando travar, lembre-se do `pc` para devolver uma carta da pilha à coluna.

Boa sorte e boa paciência!
