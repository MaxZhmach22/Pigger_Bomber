using UniRx;

namespace PiggerBomber
{
    internal interface IEatApple
    {
        ISubject<int> OnAppleEat { get; }
    }
}