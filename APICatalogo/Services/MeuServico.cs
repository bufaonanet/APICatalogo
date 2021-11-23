using System;

namespace APICatalogo.Services
{
    public class MeuServico : IMeuServico
    {
        public string Saudacao(string nome)
        {
            return $"Saudações {nome} \n\n {DateTime.Now}";
        }
    }
}