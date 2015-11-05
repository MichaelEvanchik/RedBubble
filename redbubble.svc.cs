
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.Linq;
using System.Net;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.ServiceModel.Web;
using System.Web.Script.Serialization;
using RedBubbleObjects;

namespace redbubble_create
{   
    public class redbubble : IRedBubble_Create
    {
        [WebInvoke(Method = "POST", ResponseFormat = WebMessageFormat.Json, UriTemplate = "create")]
        public oCreateOrderResponse PostOrder(oCreateOrderRequest order)
        {
            long iRefId = 0;
            string sref = String.Empty;
            bool bOrderInserted = false;
            bool bItemsInserted = false;
            long? iDentity = 0;

            //this is if the thumnail is too big(by using a HEAD command), default to this as it makes the PDF document too big
            string stemp_thumb = "https://yourdomain.com/RedBubble/images/na.gif";

            //get custom http header code from config file
            var config = ConfigurationManager.GetSection("applicationSettings/redbubble.Properties.Settings");
            var xAuthToken = ((ClientSettingsSection)config).Settings.Get("XAuthToken").Value.ValueXml.InnerText;

            //get remote information
            IncomingWebRequestContext request = WebOperationContext.Current.IncomingRequest;
            var headers = request.Headers["X-Auth-Token"];
            OperationContext context = OperationContext.Current;
            MessageProperties prop = context.IncomingMessageProperties;
            RemoteEndpointMessageProperty endpoint = prop[RemoteEndpointMessageProperty.Name] as RemoteEndpointMessageProperty;

            string ip = endpoint.Address.ToString();


                if (xAuthToken != headers) 
                {
                    sref = "Auth Failed";
                    LogError(order.external_ref, "AuthFailed", sref, order,ip);
                    throw new System.ServiceModel.Web.WebFaultException(HttpStatusCode.Unauthorized);
                }
                    using (var dc = new redbubbleDataContext())
                    {
                        try
                        {
                            List<items> line_items = order.items;
                            int? iLineItemsCount = line_items.Count; //database might want this for easier count of items so a join is not needed
                            DateTime dtSaleDate;
                            dtSaleDate = Convert.ToDateTime(order.sale_datetime);
                            bool berror = false;

                            #region validate_all_fields
                            if (order.external_ref == 0  || order.external_ref == null)
                            {
                                sref = "missing purchase order external_ref order number";
                                berror = true;
                                LogError(order.external_ref, "RedBubble endpoint", sref, order,ip);
                                throw new System.ServiceModel.Web.WebFaultException(HttpStatusCode.InternalServerError);     
                            }
                            //check more order fields of course
                            
                            foreach (items itmt in line_items)
                            {
                               if(itmt.external_ref == 0 || itmt.external_ref == null)
                               {
                                   sref = "missing order line external_ref item number";
                                   berror = true;
                                   LogError(order.external_ref, "RedBubble endpoint", sref, order,ip);
                                   throw new System.ServiceModel.Web.WebFaultException(HttpStatusCode.InternalServerError);
                               }
                               //check more items of course
                               
                            }//end for items
                            #endregion

                            if (berror == false)
                            {
                                try
                                {
                                    ISingleResult<sp_insert_orders_oheadResult> res = dc.sp_insert_orders_ohead(order.external_ref, dtSaleDate, order.purchase_complete, order.company_ref_id, order.customer_name, order.shipping_address_1, order.shipping_address_2, order.shipping_address_3, order.shipping_address_4, order.shipping_postcode, order.shipping_country, order.shipping_country_code, order.shipping_method, order.phone, iLineItemsCount, ip,ref iDentity);
                                    iDentity = (long?)order.external_ref;
                                    bOrderInserted = true;
                                }
                                catch(Exception ex)
                                {
                                    berror = true;
                                    LogError(order.external_ref, "RedBubble endpoint", ex.Message.ToString(), order,ip);

                                }

                                //insert into orders_oline
                                try
                                {
                                    foreach (items itm in line_items)
                                    {
                                        dc.sp_insert_orders_oline(iDentity, itm.external_ref, itm.sku, itm.description, itm.quantity, itm.external_url, stemp_thumb, itm.artist_name, itm.title, itm.color, itm.size);
                                        bItemsInserted = true;
                                    }
                                }
                                catch(Exception ex2)
                                {
                                    berror = true;
                                    LogError(order.external_ref, "RedBubble endpoint", ex2.Message.ToString(),order,ip);
                                }

                                if(berror == true)
                                {
                                    try
                                    {
                                         if(bOrderInserted == true)
                                         {
                                            dc.sp_delete_orders_ohead(iDentity);
                                         }
 
                                        if(bItemsInserted == true)
                                        {
                                            dc.sp_delete_orders_oline(iDentity);
                                        }
                                    }
                                    catch{}

                                    throw new System.ServiceModel.Web.WebFaultException(HttpStatusCode.InternalServerError);
                                }
                                else
                                {
                                    iRefId = order.external_ref;
                                    sref = iDentity.ToString();
                                }
                                try
                                {
                                    order = null;
                                }
                                catch { }
                            }
                            else
                            {
                                throw new System.ServiceModel.Web.WebFaultException(HttpStatusCode.InternalServerError);
                            }
                        }//end try
                        catch (Exception ex)
                        {
                            try
                            {
                                sref = ex.Message;
                                if(bOrderInserted == true)
                                {
                                    dc.sp_delete_orders_ohead(iDentity);
                                }

                                if(bItemsInserted == true)
                                {
                                    dc.sp_delete_orders_oline(iDentity);
                                }
                                using (var dc2 = new stylusDataContext())
                                {
                                    dc2.sp_insert_orders_error_log(order.external_ref, "RedBubble endpoint", ex.Message.ToString(),ip);
                                }
                            }
                            catch{}
                            throw new System.ServiceModel.Web.WebFaultException(HttpStatusCode.InternalServerError);
                        }
        }//end using
            
            var resp2 = new oCreateOrderResponse();
            resp2.id = iRefId;
            resp2.sref = sref;
            return resp2;
        }

        public bool IsThumbnailValid(string surl,long order_id,oCreateOrderRequest o,string ip2)
        {
            bool berror = false;
            long len = 0;
            try
            {
                HttpWebRequest req = (HttpWebRequest)WebRequest.Create(surl);
                req.Method = "HEAD";
                HttpWebResponse resp = (HttpWebResponse)(req.GetResponse());
                len = resp.ContentLength;
            }
            catch(Exception ex)
            {
                berror = true;
                LogError(order_id, "RedBubble endpoint", ex.Message.ToString(), o, ip2);
            }
            if(berror == true)
            {
                return false;
            }
            if (len < 750000)//750k
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public void LogError(long order_id,string err_subject, string err_msg,oCreateOrderRequest o, string ip)
        {
            try
            {
                email em = new email();
                em.SendEmail("somebody@yourself.com", "somebodyelse@yourself.com", "", "alerts@yourself.com", "RedBubble endpoint", err_subject, err_msg, "smtp.yourself.com", "", "", "", false);
                using (var dc = new redbubbleDataContext())
                {
                    dc.sp_insert_orders_error_log(order_id, err_subject, err_msg, ip);
                }
            }
            catch { }

            try
            {
                var json = new JavaScriptSerializer().Serialize(o);
                using (var dc2 = new redbubbleDataContext())
                {
                    dc2.sp_update_ordrers_err_log_json(order_id, json.ToString());
                }
            }
            catch { }
        }
    }
}
