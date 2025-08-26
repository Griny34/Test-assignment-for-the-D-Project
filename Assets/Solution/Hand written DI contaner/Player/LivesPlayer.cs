namespace SolutionDIcotaner
{
    public class LivesPlayer
    {
        private int _lives;

        public LivesPlayer(int lives)
        {
            _lives = lives;
        }

        public void LoseLive()
        {
            if(_lives > 0)
            {
                _lives--;
            }
            else
            {
                _lives = 0;
            }
        }
    }
}