namespace FinalTask.Exceptions
{
        public class WrongDiceNumberException : Exception
        {
            public WrongDiceNumberException(int minAllowed, int maxAllowed)
                : base($"Min or Max or both are out of range from {minAllowed} to {maxAllowed}, or Min > Max. Please check!")
            {
            }
        }
}
