public class Carta
    {
        public int Numero { get; set; }
        public string Naipe { get; set; }

        public Carta(int numero, string naipe)
        {
            Numero = numero;
            Naipe = naipe;
        }

        public override string ToString()
        {
            string valorStr = Numero.ToString();
            if (Numero == 1) valorStr = "Ás";
            else if (Numero == 11) valorStr = "Dama";
            else if (Numero == 12) valorStr = "Valete";
            else if (Numero == 13) valorStr = "Rei";
            
            return $"{valorStr} de {Naipe}";
        }
    }