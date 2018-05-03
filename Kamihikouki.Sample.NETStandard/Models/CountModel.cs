namespace Kamihikouki.Sample.NETStandard.Models
{
    public class CountModel : BaseModel
    {
        private int _count;
        public int Count {
            get => _count;
            set => SetProperty(ref _count, value);
        }

        public void Increment()
        {
            Count++;
        }

        public void Decrement()
        {
            Count--;
        }
    }
}
