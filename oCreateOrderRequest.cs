using System.Collections.Generic;

namespace RedBubbleObjects
{
    public class oCreateOrderRequest
    {
        private long _external_ref;
        private string _sale_datetime;
        private bool _purchase_complete;
        private string _company_ref_id;
        private string _customer_name;
        private string _shipping_address_1;
        private string _shipping_address_2;
        private string _shipping_address_3;
        private string _shipping_address_4;
        private string _shipping_postcode;
        private string _shipping_country;
        private string _shipping_country_code;
        private string _shipping_method;
        private string _phone;
        private List<items> _items;

        public long external_ref
        {
            get { return _external_ref; }
            set { _external_ref = value; }
        }

        public string sale_datetime
        {
            get { return _sale_datetime; }
            set { _sale_datetime = value; }
        }

        public bool purchase_complete
        {
            get { return _purchase_complete; }
            set { _purchase_complete = value; }
        }

        public string company_ref_id
        {
            get { return _company_ref_id; }
            set { _company_ref_id = value; }
        }

        public string customer_name
        {
            get { return _customer_name; }
            set { _customer_name = value; }
        }

        public string shipping_address_1
        {
            get { return _shipping_address_1; }
            set { _shipping_address_1 = value; }
        }

        public string shipping_address_2
        {
            get { return _shipping_address_2; }
            set { _shipping_address_2 = value; }
        }

        public string shipping_address_3
        {
            get { return _shipping_address_3; }
            set { _shipping_address_3 = value; }
        }

        public string shipping_address_4
        {
            get { return _shipping_address_4; }
            set { _shipping_address_4 = value; }
        }

        public string shipping_postcode
        {
            get { return _shipping_postcode; }
            set { _shipping_postcode = value; }
        }

        public string shipping_country
        {
            get { return _shipping_country; }
            set { _shipping_country = value; }
        }

        public string shipping_country_code
        {
            get { return _shipping_country_code; }
            set { _shipping_country_code = value; }
        }

        public string shipping_method
        {
            get { return _shipping_method; }
            set { _shipping_method = value; }
        }

        public string phone
        {
            get { return _phone; }
            set { _phone = value; }
        }

        public List<items> items
        {
            get { return _items; }
            set { _items = value; }
        }
    }
}
