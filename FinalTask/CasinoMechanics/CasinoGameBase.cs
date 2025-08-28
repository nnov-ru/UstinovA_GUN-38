namespace FinalTask.CasinoMechanics
{
    public abstract class CasinoGameBase
    {
        public event Action<int> OnWin;
        public event Action<int> OnLose;
        public event Action<int> OnDraw;
        protected void OnWinInvoke(int bet)
        {
            OnWin?.Invoke(bet);
        }
        protected void OnLooseInvoke(int bet)
        {
            OnLose?.Invoke(bet);
        }
        protected void OnDrawInvoke(int bet)
        {
            OnDraw?.Invoke(bet);
        }
        protected abstract void FactoryMethod();
        protected CasinoGameBase() 
        {
            FactoryMethod();
        }
        public abstract void PlayGame(int bet);
    }
}
