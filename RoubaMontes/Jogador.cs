public class Jogador
    {
        public string Nome { get; set; }
        public Stack<Carta> Monte { get; set; }
        public int PosicaoUltimaPartida { get; set; }
        public int QtdCartasUltimaPartida { get; set; }
        public Queue<int> HistoricoRanking { get; set; }

        public Jogador(string nome)
        {
            Nome = nome;
            Monte = new Stack<Carta>();
            HistoricoRanking = new Queue<int>();
        }

        public void AdicionarRanking(int posicao)
        {
            HistoricoRanking.Enqueue(posicao);
            if (HistoricoRanking.Count > 5)
            {
                HistoricoRanking.Dequeue();
            }
        }
    }