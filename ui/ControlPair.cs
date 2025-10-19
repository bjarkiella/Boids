namespace Boids.ui
{
    public class ControlPair<TFirst, TSecond>(TFirst first, TSecond second)
    {
        public TFirst First { get; set; } = first;
        public TSecond Second { get; set; } = second;
    }
}
