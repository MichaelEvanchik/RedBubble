namespace RedBubbleObjects
{
    public class oCreateOrderResponse
    {
        private long _id;
        private string _sref;

        public long id
        {
            get { return _id; }
            set { _id = value; }
        }

        public string sref
        {
            get { return _sref; }
            set { _sref = value; }
        }
    }
}
