namespace SolutionDIcotaner
{
    //Класс сундук в котором лежит пулемёт
    public class Сhest
    {
        private Weapon _machinGun = new Weapon("Пулемёт", 200);

        public Weapon TakeReward ()
        {
            return _machinGun;
        }
    }
}