using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace RoubaMontesGame
{
    class RoubaMontes
    {
        static Random random = new Random();

        static void Main(string[] args)
        {
            List<Jogador> todosJogadores = new List<Jogador>();
            
            Console.Write("Digite a quantidade de jogadores: ");
            int qtdJogadores = int.Parse(Console.ReadLine());

            for (int i = 0; i < qtdJogadores; i++)
            {
                Console.Write($"Nome do jogador {i + 1}: ");
                todosJogadores.Add(new Jogador(Console.ReadLine()));
            }

            bool jogarNovamente = true;

            while (jogarNovamente)
            {
                Console.Write("Digite a quantidade de baralhos (Definir o total de cartas): ");
                int qtdBaralhos = int.Parse(Console.ReadLine());

                Stack<Carta> monteCompra = CriarBaralho(qtdBaralhos);
                List<Carta> areaDescarte = new List<Carta>();
                
                Queue<Jogador> filaJogadores = new Queue<Jogador>(todosJogadores);
                foreach(Jogador j in todosJogadores) j.Monte.Clear();

                string logFileName = $"log_partida_{DateTime.Now:yyyyMMdd_HHmmss}.txt";
                using (StreamWriter logWriter = new StreamWriter(logFileName))
                {
                    logWriter.WriteLine($"O baralho foi criado com {monteCompra.Count} cartas.");
                    logWriter.WriteLine($"Jogadores da partida: {string.Join(", ", todosJogadores.Select(j => j.Nome))}");

                    Console.WriteLine("\n--- Início da Partida ---");
                    
                    while (monteCompra.Count > 0)
                    {
                        Jogador jogadorAtual = filaJogadores.Dequeue();
                        bool continuaJogada = true;

                        logWriter.WriteLine($"Vez do jogador: {jogadorAtual.Nome}");

                        while (continuaJogada && monteCompra.Count > 0)
                        {
                            Carta cartaDaVez = monteCompra.Pop();
                            logWriter.WriteLine($"Jogador {jogadorAtual.Nome} retirou a carta: {cartaDaVez} do monte de compras.");
                            Console.WriteLine($"{jogadorAtual.Nome} retirou {cartaDaVez} do monte de compras.");

                            bool acaoRealizada = false;

                            List<Jogador> alvosPossiveis = new List<Jogador>();
                            foreach (Jogador oponente in filaJogadores) 
                            {
                                if (oponente.Monte.Count > 0 && oponente.Monte.Peek().Numero == cartaDaVez.Numero)
                                {
                                    alvosPossiveis.Add(oponente);
                                }
                            }

                            if (alvosPossiveis.Count > 0)
                            {
                                int maxCartas = alvosPossiveis.Max(j => j.Monte.Count);
                                List<Jogador> maioresMontes = alvosPossiveis.Where(j => j.Monte.Count == maxCartas).ToList();
                                
                                Jogador alvo;
                                if (maioresMontes.Count > 1)
                                {
                                    alvo = maioresMontes[random.Next(maioresMontes.Count)];
                                }
                                else
                                {
                                    alvo = maioresMontes[0];
                                }

                                while (alvo.Monte.Count > 0)
                                {
                                    jogadorAtual.Monte.Push(alvo.Monte.Pop());
                                }
                                
                                jogadorAtual.Monte.Push(cartaDaVez);
                                logWriter.WriteLine($"Jogador {jogadorAtual.Nome} ROUBOU o monte de {alvo.Nome}.");
                                Console.WriteLine($"{jogadorAtual.Nome} ROUBOU o monte de {alvo.Nome}!");
                                acaoRealizada = true;
                            }

                            if (!acaoRealizada)
                            {
                                Carta cartaDescarte = areaDescarte.FirstOrDefault(c => c.Numero == cartaDaVez.Numero);
                                if (cartaDescarte != null)
                                {
                                    areaDescarte.Remove(cartaDescarte);
                                    jogadorAtual.Monte.Push(cartaDescarte);
                                    jogadorAtual.Monte.Push(cartaDaVez);
                                    logWriter.WriteLine($"Jogador {jogadorAtual.Nome} PEGOU {cartaDescarte} da área de descarte.");
                                    Console.WriteLine($"{jogadorAtual.Nome} PEGOU carta da área de descarte.");
                                    acaoRealizada = true;
                                }
                            }

                            if (!acaoRealizada)
                            {
                                if (jogadorAtual.Monte.Count > 0 && jogadorAtual.Monte.Peek().Numero == cartaDaVez.Numero)
                                {
                                    jogadorAtual.Monte.Push(cartaDaVez);
                                    logWriter.WriteLine($"Jogador {jogadorAtual.Nome} COLOCOU a carta no próprio monte.");
                                    Console.WriteLine($"{jogadorAtual.Nome} COLOCOU no próprio monte.");
                                    acaoRealizada = true;
                                }
                            }

                            if (!acaoRealizada)
                            {
                                areaDescarte.Add(cartaDaVez);
                                logWriter.WriteLine($"Jogador {jogadorAtual.Nome} descartou a carta na mesa.");
                                Console.WriteLine($"{jogadorAtual.Nome} descartou.");
                                continuaJogada = false;
                            }
                        }

                        filaJogadores.Enqueue(jogadorAtual);
                    }
                    
                    logWriter.WriteLine("Fim das cartas no monte de compras.");
                }

                Console.WriteLine("\n--- Fim da Partida ---");
                
                List<Jogador> ranking = todosJogadores.OrderByDescending(j => j.Monte.Count).ToList();
                int maxCartasVitoria = ranking.First().Monte.Count;
                List<Jogador> ganhadores = ranking.Where(j => j.Monte.Count == maxCartasVitoria).ToList();

                Console.WriteLine("Ganhador(es):");
                foreach (Jogador g in ganhadores)
                {
                    Console.WriteLine($"Nome: {g.Nome} - Cartas: {g.Monte.Count}");
                }

                Console.WriteLine("\nRanking da Partida:");
                int pos = 1;
                foreach (Jogador j in ranking)
                {
                    Console.WriteLine($"{pos}º - {j.Nome}: {j.Monte.Count} cartas");
                    j.PosicaoUltimaPartida = pos;
                    j.QtdCartasUltimaPartida = j.Monte.Count;
                    j.AdicionarRanking(pos);
                    pos++;
                }
                Console.WriteLine($"\nCartas no monte {monteCompra.Count}");
                Console.WriteLine($"Cartas no descarte {areaDescarte.Count}");
                Console.WriteLine($"\nLog da partida salvo em: {logFileName}");

                Console.WriteLine("\nDeseja ver o histórico de algum jogador? (Digite um nome ou ENTER para pular)");
                string buscaHistorico = Console.ReadLine();
                if (!string.IsNullOrWhiteSpace(buscaHistorico))
                {
                    Jogador jog = todosJogadores.FirstOrDefault(j => j.Nome.Equals(buscaHistorico, StringComparison.OrdinalIgnoreCase));
                    if (jog != null)
                    {
                        Console.WriteLine($"Histórico de {jog.Nome} (Ultimas posições): {string.Join(", ", jog.HistoricoRanking)}");
                    }
                    else
                    {
                        Console.WriteLine("Jogador não encontrado.");
                    }
                }

                Console.Write("\nDeseja iniciar uma nova partida? (s/n): ");
                string resp = Console.ReadLine();
                jogarNovamente = resp.ToLower() == "s";
            }
        }

        static Stack<Carta> CriarBaralho(int qtdBaralhos)
        {
            List<Carta> cartas = new List<Carta>();
            string[] naipes = { "Copas", "Ouros", "Paus", "Espadas" };
            
            for (int b = 0; b < qtdBaralhos; b++)
            {
                foreach (string naipe in naipes)
                {
                    for (int i = 1; i <= 13; i++)
                    {
                        cartas.Add(new Carta(i, naipe));
                    }
                }
            }

            Stack<Carta> monteEmbaralhado = new Stack<Carta>();
            while (cartas.Count > 0)
            {
                int index = random.Next(cartas.Count);
                monteEmbaralhado.Push(cartas[index]);
                cartas.RemoveAt(index);
            }
            return monteEmbaralhado;
        }
    }
}