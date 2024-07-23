namespace WorkFlowStages.Model
{
    public class ResponceModel
    {
        public int Code { get; set; }
        public string Message { get; set; }
    }

    public class ResponceModel<T> : ResponceModel
    {
        public T? Data { get; set; }
    }
}
