using System.Text.RegularExpressions;

namespace FCG.Users.Application.Validators
{
    public class UsuarioValidators
    {
        public bool EmailValido(string email)
        {
            if (string.IsNullOrWhiteSpace(email))
                return false;

            var regex = new Regex(@"^[^@\s]+@[^@\s]+\.[^@\s]+$");
            return regex.IsMatch(email);
        }

        public bool SenhaValida(string senha)
        {
            if (string.IsNullOrWhiteSpace(senha))
                return false;

            var regex = new Regex(@"^(?=.*[A-Za-z])(?=.*\d)(?=.*[\W_]).{8,}$");
            return regex.IsMatch(senha);
        }

        public bool TamanhoMaximo(string texto, int max)
        {
            if (texto == null)
                return false;

            return texto.Length <= max;
        }
    }
}
